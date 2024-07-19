using Microsoft.AspNetCore.Routing.Template;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Services.Repository
{
    public class LearningProgressRepository : Repository<LearningProgress>, ILearningProgressRepository
    {
        private ApplicationDBContext _db;
        private readonly BlobStorageService _blobStorageService;
        public LearningProgressRepository(ApplicationDBContext db, BlobStorageService blobStorageService) : base(db)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public void LoadCourse(LearningProgress learningProgress)
        {
            _db.LearningProgresses.Entry(learningProgress)
                .Reference(l => l.Course)
                .Load();
        }

        public void Update(LearningProgress learningProgress)
        {
            _db.LearningProgresses.Update(learningProgress);
        }

        public async Task<string> AssignCertificate(LearningProgress learningProgress)
        {
            string templatePath = Path.Combine("wwwroot", "images", "ViLearning_Certificate_Template.pdf");
            
            // Check if File Exist
            if (!System.IO.File.Exists(templatePath))
            {
                return null;
            }
            PdfDocument document = PdfReader.Open(templatePath, PdfDocumentOpenMode.Modify);
            PdfPage page = document.Pages[0];

            // Tạo đồ họa để vẽ
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Định nghĩa font và kích thước
            XFont font12 = new XFont("Arial", 12);
            XFont font24 = new XFont("Arial", 24);
            XFont font32 = new XFont("Arial", 32);

            // Định vị trí để thêm văn bản vào PDF (tọa độ x, y)
            XPoint datePosition = new XPoint(70, 75); // Vị trí của Complete Date
            XPoint namePosition = new XPoint(70, 128); // Vị trí của Student’s Name
            XPoint coursePosition = new XPoint(70, 192); // Vị trí của Course’s Name
            XPoint teacherPosition = new XPoint(290, 212); // Vị trí của Teacher’s Name

            // Thêm ngày hoàn thành
            gfx.DrawString(learningProgress.CompletionDate.ToString(), font12, XBrushes.Black, datePosition);

            // Thêm tên học viên
            gfx.DrawString(learningProgress.User.UserName, font32, XBrushes.Black, namePosition);

            // Thêm tên khóa học
            gfx.DrawString(learningProgress.Course.CourseName, font24, XBrushes.Black, coursePosition);

            // Thêm tên giáo viên
            gfx.DrawString(learningProgress.Course.ApplicationUser.UserName, font12, XBrushes.Black, teacherPosition);

            // Lưu tệp PDF mới lên Blob Storage
            // document.Save(outputPath);
            // Lưu tài liệu PDF vào MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0; // Đặt vị trí của stream về đầu

            // Tải file lên Blob Storage
            string containerName = "student-certificates-img";
            string fileName = $"{Guid.NewGuid()}.pdf";
            string fileUrl = await _blobStorageService.UploadFileAsync(containerName, fileName, stream);

            return fileUrl;
        }

    }
}
