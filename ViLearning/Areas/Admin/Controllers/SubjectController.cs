using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Services.Repository;

namespace ViLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public SubjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Subject> objSubjectList = _unitOfWork.Subject.GetAll().ToList();
            return View(objSubjectList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Subject obj)
        {
            foreach (Subject subject in _unitOfWork.Subject.GetAll().ToList())
            {
                if (subject.Name == obj.Name)
                {
                    ModelState.AddModelError("name", "Đã có môn học này");
                }
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Subject.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Thêm môn học thành công";
                return RedirectToAction("Index", "Subject");
            }
            return View();
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var subject = _unitOfWork.Subject.Get(x => x.Id == id);
            if (subject == null)
            {
                return Json(new { success = false, message = "Có lỗi khi xóa" });
            }
            _unitOfWork.Subject.Remove(subject);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa thành công" });
        }
    }
}
