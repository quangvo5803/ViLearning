using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FFMpegCore;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using NuGet.Protocol.Plugins;
using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Models.ViewModels;
using ViLearning.Services.Repository;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace ViLearning.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = SD.Role_User_Teacher)]
    public class LessonsController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly BlobStorageService _blobStorageService;
        private readonly IHostEnvironment _hostingEnvironment;
        public LessonsController(IUnitOfWork unitOfWork ,BlobStorageService blobStorageService, IHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _blobStorageService = blobStorageService;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Teacher/Lessons
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _unitOfWork.Lesson.GetAll(includeProperties:"Course");
            return View( applicationDBContext);
        }

        // GET: Teacher/Lessons/Details/5
        [HttpGet("Teacher/{CourseName}/{LessonName}/Detail")]
        public async Task<IActionResult> Details(string CourseName, string LessonName, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lesson = _unitOfWork.Lesson.Get(m => m.LessonId == id,includeProperties:"Course");
            _unitOfWork.Lesson.LoadCourse(lesson);
            if (lesson == null)
            {
                return NotFound();
            }
            return View(lesson);
        }

        // GET: Teacher/Lessons/Create
        [HttpGet("Teacher/{name}/Lessons/Create")]
        public IActionResult Create(string name)
        {

            ViewData["CourseId"] = new SelectList
                    (_unitOfWork.Course
                    .GetRange(c => c.CourseName.Equals(name)), "CourseId", "CourseName");
            var course = _unitOfWork.Course.Get(c => c.CourseName.Equals(name));
            ViewBag.Size = _unitOfWork.Lesson.GetRange(l => l.CourseId == course.CourseId).Count() + 1;
            return View();
        }

        [TempData]
        public string StatusMessage {get; set; }
        [HttpPost("Teacher/{name}/Lessons/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, 
            [Bind("LessonId,LessonName,LessonNo,Content,Video,TotalQuestions,EasyQuestions,MediumQuestions,HardQuestions,TestDuration,CourseId")] 
            Lesson lesson, 
            IFormFile? Video)
        {
            int courseId = _unitOfWork.Course.Get(c => c.CourseName.Equals(name)).CourseId;
            ModelState.Remove("Comments");
            if (ModelState.IsValid)
            {
                try
                {
                    if (Video != null)
                    {
                        // Generate unique video id
                        var videoId = Guid.NewGuid().ToString();


                        //Save video to temp file
                        var uploadPath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "uploads");
                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        var filePath = Path.Combine(uploadPath, Video.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Video.CopyToAsync(fileStream);
                        }
                        if (!Directory.Exists(Path.Combine(uploadPath, "hls", Video.FileName)))
                            Directory.CreateDirectory(Path.Combine(uploadPath, "hls", Video.FileName));



                        var outputDirectory = Path.Combine(uploadPath, "hls");
                        var outputPattern = Path.Combine(outputDirectory, "slice_%03d.ts");
                        var playlistFile = Path.Combine(outputDirectory, "playlist.m3u8");

                        // ffmpeg command to generate hls segments and playlist
                        var ffmpegArgs = $"ffmpeg -i \"{filePath}\" -codec: copy -start_number 0 -hls_time 10 -hls_list_size 0 \"{playlistFile}\"";
                        var proc1 = new ProcessStartInfo();
                        proc1.UseShellExecute = false;


                        proc1.WorkingDirectory = outputDirectory;


                        proc1.FileName = @"C:\Windows\System32\cmd.exe";
                        proc1.Arguments = "/c " + ffmpegArgs;
                        proc1.RedirectStandardError = true;
                        proc1.RedirectStandardOutput = true;
                        proc1.CreateNoWindow = true;



                        using (var proc = new Process { StartInfo = proc1 })
                        {
                            proc.Start();
                            var outputTask = Task.Run(() => proc.StandardOutput.ReadToEndAsync());
                            var errorTask = Task.Run(() => proc.StandardError.ReadToEndAsync());
                            var output = await outputTask;
                            var error = await errorTask;
                            bool exited = proc.WaitForExit(6000);
                            if (!exited)
                            {
                                proc.Kill();

                            }
                            if (proc.ExitCode != 0)
                            {
                                throw new Exception($"FFmpeg failed with error: {error}");
                            }
                        }

                        var files = Directory.GetFiles(outputDirectory);
                        foreach (var file in files)

                        {
                            var blobName = $"{videoId}/{Path.GetFileName(file)}";
                            using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
                            {
                                string url = await _blobStorageService.UploadFileAsync("lesson-video", blobName, fs);
                                if (url[url.Length - 1] == '8') lesson.Video = url;
                            }
                        }


                        //Clean temp files 
                        //System.IO.File.Delete(tempFilePath);
                        System.IO.File.Delete(filePath);
                        Directory.Delete(outputDirectory, true);

                        //store lesson to db
                        var playlistUrl = _blobStorageService.GetPlaylistBlobName("lesson-video", $"{videoId}/playlist.m3u8");
                        _unitOfWork.Lesson.Add(lesson);
                        _unitOfWork.Save();
                        return RedirectToAction("Details", "Courses", new { id = courseId });
                } else
                {
                    _unitOfWork.Lesson.Add(lesson);
                    _unitOfWork.Save();
                    return RedirectToAction("Details", "Courses", new { id = courseId });
                }
            }
                catch (Exception ex)
                {
                    StatusMessage = $"Error uploading file: {ex.Message}";
                    
                    Course course = _unitOfWork.Course.Get(c => c.CourseName.Equals(name));
                    _unitOfWork.Course.LoadCourse(course);
                    ViewBag.Size = _unitOfWork.Lesson.GetRange(l => l.CourseId == course.CourseId).Count() + 1;

                    ViewData["CourseId"] = new SelectList(_unitOfWork.Course.GetRange(c => c.CourseName.Equals(name)), "CourseId", "CourseName", lesson.CourseId);
                    return View(lesson);
                }
            }

            return View(lesson);
        }

        // GET: Teacher/Lessons/Edit/5
        [HttpGet("Teacher/{CourseName}/{LessonName}/Edit/{id}")]
        public async Task<IActionResult> Edit(string CourseName, string LessonName, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = _unitOfWork.Lesson.Get(c => c.LessonId == id);
            if (lesson == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_unitOfWork.Course.GetAll(), "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }
        
        // POST: Teacher/Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Teacher/{CourseName}/{LessonName}/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string CourseName, string LessonName, int id, [Bind("LessonId,LessonName,LessonNo,Content,Video,TotalQuestions,EasyQuestions,MediumQuestions,HardQuestions,TestDuration,CourseId")] Lesson lesson, IFormFile? Video)
        {
            if (id != lesson.LessonId)
            {
                return NotFound();
            }
            ModelState.Remove("Comments");
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                    int courseId = _unitOfWork.Course.Get(c => c.CourseName.Equals(CourseName)).CourseId;
                if (Video!=null)
                {
                    
                    try
                    {
                        // Generate unique video id
                        var videoId = Guid.NewGuid().ToString();
                        //Save video to temp file
                        var uploadPath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "uploads");
                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);


                        var filePath = Path.Combine(uploadPath, Video.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Video.CopyToAsync(fileStream);
                        }
                        if (!Directory.Exists(Path.Combine(uploadPath, "hls", Video.FileName)))
                            Directory.CreateDirectory(Path.Combine(uploadPath, "hls", Video.FileName));


                        var outputDirectory = Path.Combine(uploadPath, "hls");
                        var outputPattern = Path.Combine(outputDirectory, "slice_%03d.ts");
                        var playlistFile = Path.Combine(outputDirectory, "playlist.m3u8");

                        // ffmpeg command to generate hls segments and playlist
                        var ffmpegArgs = $"ffmpeg -i \"{filePath}\" -codec: copy -start_number 0 -hls_time 10 -hls_list_size 0 \"{playlistFile}\"";
                        var proc1 = new ProcessStartInfo();
                        proc1.UseShellExecute = false;

                        proc1.WorkingDirectory = outputDirectory;


                        proc1.FileName = @"C:\Windows\System32\cmd.exe";
                        proc1.Arguments = "/c " + ffmpegArgs;
                        proc1.RedirectStandardError = true;
                        proc1.RedirectStandardOutput = true;
                        proc1.CreateNoWindow = true;



                        using (var proc = new Process { StartInfo = proc1 })
                        {
                            proc.Start();
                            var outputTask = Task.Run(() => proc.StandardOutput.ReadToEndAsync());
                            var errorTask = Task.Run(() => proc.StandardError.ReadToEndAsync());
                            var output = await outputTask;
                            var error = await errorTask;
                            bool exited = proc.WaitForExit(6000);
                            if (!exited)
                            {
                                proc.Kill();

                            }
                            if (proc.ExitCode != 0)
                            {
                                throw new Exception($"FFmpeg failed with error: {error}");
                            }
                        }

                        var files = Directory.GetFiles(outputDirectory);
                        foreach (var file in files)
                        {
                            var blobName = $"{videoId}/{Path.GetFileName(file)}";
                            using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
                            {
                                string url = await _blobStorageService.UploadFileAsync("lesson-video", blobName, fs);
                                if (url[url.Length - 1] == '8') lesson.Video = url;
                            }
                        }


                        //Clean temp files 
                        //System.IO.File.Delete(tempFilePath);
                        System.IO.File.Delete(filePath);
                        Directory.Delete(outputDirectory, true);

                        //store lesson to db
                        var playlistUrl = _blobStorageService.GetPlaylistBlobName("lesson-video", $"{videoId}/playlist.m3u8");
                        _unitOfWork.Lesson.Update(lesson);
                        _unitOfWork.Save();
                        return RedirectToAction("Details", "Courses", new { id = courseId });
                    } catch (Exception ex)
                    {
                        StatusMessage = $"Error uploading file: {ex.Message}";
                        return View(lesson);
                    }
                } else
                {
                    var oldLesson = _unitOfWork.Lesson.Get(l => l.LessonId == lesson.LessonId);
                    lesson.Video = oldLesson.Video;
                    _unitOfWork.Lesson.UnDetachLesson(oldLesson);
                    _unitOfWork.Lesson.Update(lesson);
                    _unitOfWork.Save();
                    return RedirectToAction("Details", "Courses", new { id = courseId });
                }
            }

            return View(lesson);
        }

        // GET: Teacher/Lessons/Delete/5
        [HttpGet("Teacher/{CourseName}/{LessonName}/Delete/{id}")]
        public async Task<IActionResult> Delete(string CourseName, string LessonName,int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lesson = _unitOfWork.Lesson.Get(m => m.LessonId == id, includeProperties: "Course");
            /*            var lesson = await _context.Lessons
                            .Include(l => l.Course)
                            .FirstOrDefaultAsync(m => m.LessonId == id);*/
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Teacher/Lessons/Delete/5
        [HttpPost("Teacher/{CourseName}/{LessonName}/Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string CourseName, string LessonName, int id)
        {
            var lesson = _unitOfWork.Lesson.Get(l => l.LessonId == id, includeProperties:"Course");
            if (lesson != null)
            {
                _unitOfWork.Lesson.Remove(lesson);
            }

            _unitOfWork.Save();
            
            return RedirectToAction("Details", "Courses", new { id = lesson.Course.CourseId });
        }

        private bool LessonExists(int id)
        {
            return (_unitOfWork.Lesson.Get(l => l.LessonId == id) != null);
        }
    }
}
