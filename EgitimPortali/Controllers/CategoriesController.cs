using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EgitimPortali.Data;
using EgitimPortali.Models;
using EgitimPortali.Repositories; // Repository klasörünü eklemeyi unutma!

namespace EgitimPortali.Controllers
{
    public class CategoriesController : Controller
    {
        // ARTIK DBCONTEXT YOK, IREPOSITORY VAR
        private readonly IRepository<Category> _categoryRepo;

        // Constructor'da Repository istiyoruz
        public CategoriesController(IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // GET: Categories
        public IActionResult Index()
        {
            // _context.Categories.ToListAsync() yerine:
            var categories = _categoryRepo.GetAll();
            return View(categories);
        }

        // GET: Categories/Details/5
        public IActionResult Details(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name")] Category category)
        {
            // Kategori eklerken "Courses" listesi boş diye hata vermesin
            ModelState.Remove("Courses");

            if (ModelState.IsValid)
            {
                // _context.Add yerine:
                _categoryRepo.Add(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Courses");

            if (ModelState.IsValid)
            {
                try
                {
                    // _context.Update yerine:
                    _categoryRepo.Update(category);
                }
                catch (Exception)
                {
                    if (_categoryRepo.GetById(category.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // SİLME İŞLEMİ (AJAX İLE YAPILDIĞI İÇİN ESKİ DELETE SAYFALARINI KALDIRDIK)
        // Yönergeye uygun olan AJAX metodumuz:
        [HttpPost]
        public IActionResult DeleteAjax(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı!" });
            }

            // Repository üzerinden siliyoruz
            _categoryRepo.Delete(id);

            return Json(new { success = true, message = "Kategori başarıyla silindi." });
        }
        // AJAX İLE EKLEME İŞLEMİ
        [HttpPost]
        public IActionResult CreateAjax(Category category)
        {
            // Modelde "Courses" listesi zorunlu görünüyor ama boş gelebilir, hatayı siliyoruz
            ModelState.Remove("Courses");

            if (ModelState.IsValid)
            {
                _categoryRepo.Add(category); // Repository ile ekle
                return Json(new { success = true, message = "Kategori başarıyla eklendi! 🎉" });
            }

            // Hata varsa
            return Json(new { success = false, message = "Bir hata oluştu, lütfen bilgileri kontrol edin." });
        }
    }
}