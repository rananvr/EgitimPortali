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
        
        private readonly IRepository<Category> _categoryRepo;

      
        public CategoriesController(IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

       
        public IActionResult Index()
        {
          
            var categories = _categoryRepo.GetAll();
            return View(categories);
        }

      
        public IActionResult Details(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        
        public IActionResult Create()
        {
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name")] Category category)
        {
      
            ModelState.Remove("Courses");

            if (ModelState.IsValid)
            {
             
                _categoryRepo.Add(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

 
        public IActionResult Edit(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

      
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
//AJAX 
        [HttpPost]
        public IActionResult DeleteAjax(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı!" });
            }

            
            _categoryRepo.Delete(id);

            return Json(new { success = true, message = "Kategori başarıyla silindi." });
        }
        // AJAX İLE EKLEME İŞLEMİ
        [HttpPost]
        public IActionResult CreateAjax(Category category)
        {
            ModelState.Remove("Courses");

            if (ModelState.IsValid)
            {
                _categoryRepo.Add(category); 
                return Json(new { success = true, message = "Kategori başarıyla eklendi! 🎉" });
            }

            // Hata varsa
            return Json(new { success = false, message = "Bir hata oluştu, lütfen bilgileri kontrol edin." });
        }
    }
}