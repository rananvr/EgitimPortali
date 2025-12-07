using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EgitimPortali.Models;
using EgitimPortali.Repositories;

namespace EgitimPortali.Controllers
{
    public class CoursesController : Controller
    {
        // İKİ REPOSITORY LAZIM: Biri Kurslar için, biri Kategoriler için (Dropdown için)
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<Category> _categoryRepo;

        public CoursesController(IRepository<Course> courseRepo, IRepository<Category> categoryRepo)
        {
            _courseRepo = courseRepo;
            _categoryRepo = categoryRepo;
        }

        // 1. LİSTELEME
        public IActionResult Index()
        {
            // "Category" diyerek ilişkili tabloyu da getiriyoruz
            var kurslar = _courseRepo.GetAll("Category");
            return View(kurslar);
        }

        // 2. DETAY
        public IActionResult Details(int id)
        {
            var course = _courseRepo.GetById(id);
            if (course == null) return NotFound();
            return View(course);
        }

        // 3. EKLEME (Sayfa)
        public IActionResult Create()
        {
            // Dropdown için kategorileri Repository'den çekiyoruz
            ViewData["CategoryId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name");
            return View();
        }

        // 3. EKLEME İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course, IFormFile? resimDosyasi)
        {
            // Hata vermesin diye zorunlu alanlardan bunları çıkarıyoruz
            ModelState.Remove("Category");
            ModelState.Remove("Instructor"); // <--- YENİ: Eğitmen nesnesi boş geleceği için hatayı siliyoruz

            if (ModelState.IsValid)
            {
                // --- 1. RESİM YÜKLEME İŞLEMİ (Aynı kalıyor) ---
                if (resimDosyasi != null)
                {
                    string uzanti = Path.GetExtension(resimDosyasi.FileName);
                    string yeniIsim = Guid.NewGuid().ToString() + uzanti;
                    string yol = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", yeniIsim);

                    using (var stream = new FileStream(yol, FileMode.Create))
                    {
                        await resimDosyasi.CopyToAsync(stream);
                    }
                    course.ImageUrl = "/img/" + yeniIsim;
                }

                // --- 2. EĞİTMENİ EKLEME (SENİN İSTEDİĞİN KISIM BURASI) ---
                // O an sisteme giriş yapmış kişinin ID'sini Session'dan alıyoruz
                int? egitmenId = HttpContext.Session.GetInt32("UserId");

                if (egitmenId != null)
                {
                    course.InstructorId = egitmenId; // Kursun sahibini belirliyoruz
                }
                // ---------------------------------------------------------

                _courseRepo.Add(course); // Veritabanına kaydediyoruz
                return RedirectToAction(nameof(Index));
            }

            // Hata varsa formu tekrar doldur
            ViewData["CategoryId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name", course.CategoryId);
            return View(course);
        }

        // 4. GÜNCELLEME (Sayfa)
        public IActionResult Edit(int id)
        {
            var course = _courseRepo.GetById(id);
            if (course == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name", course.CategoryId);
            return View(course);
        }

        // 4. GÜNCELLEME (İşlem)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Course course)
        {
            if (id != course.Id) return NotFound();

            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                // Güncelleme yaparken mevcut resim kaybolmasın diye basit update
                _courseRepo.Update(course);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name", course.CategoryId);
            return View(course);
        }

        // 5. AJAX SİLME METODU (İşte istediğin yer)
        [HttpPost]
        public IActionResult DeleteAjax(int id)
        {
            var course = _courseRepo.GetById(id);
            if (course == null)
            {
                return Json(new { success = false, message = "Kurs bulunamadı!" });
            }

            _courseRepo.Delete(id);
            return Json(new { success = true, message = "Kurs başarıyla silindi." });
        }
    }
}