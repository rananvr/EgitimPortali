using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EgitimPortali.Data;
using EgitimPortali.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace EgitimPortali.Controllers
{
    [Authorize(Roles = "Admin, Egitmen")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; 
        private readonly UserManager<IdentityUser> _userManager;  

        public CoursesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        // 1. KURSLARI LİSTELE
        public async Task<IActionResult> Index()
        {
            var kurslar = await _context.Courses.Include(c => c.Category).ToListAsync();
            return View(kurslar);
        }

        // 2. KURS EKLEME SAYFASI (GET)
        public IActionResult Create()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Genel Eğitimler" });
                _context.SaveChanges();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // --- DÜZENLEME SAYFASI (GET) ---
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // --- DÜZENLEME İŞLEMİ (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course, IFormFile? resimDosyasi)
        {
            if (id != course.Id) return NotFound();

            ModelState.Remove("ImageUrl");
            ModelState.Remove("InstructorId");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCourse = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

                    if (resimDosyasi != null)
                    {
                        string uzanti = Path.GetExtension(resimDosyasi.FileName);
                        string yeniIsim = Guid.NewGuid().ToString() + uzanti;
                        string yol = Path.Combine(_webHostEnvironment.WebRootPath, "img", yeniIsim);
                        using (var stream = new FileStream(yol, FileMode.Create)) { await resimDosyasi.CopyToAsync(stream); }
                        course.ImageUrl = "/img/" + yeniIsim;
                    }
                    else
                    {
                        course.ImageUrl = existingCourse.ImageUrl;
                    }

                    course.InstructorId = existingCourse.InstructorId;
                    course.CreatedDate = existingCourse.CreatedDate;

                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Courses.Any(e => e.Id == course.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // --- AJAX SİLME İŞLEMİ (JSON DÖNER) ---
        [HttpPost]
        public async Task<IActionResult> DeleteCourseAjax(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return Json(new { success = false, message = "Kurs bulunamadı." });
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Kurs başarıyla silindi." });
        }


        // 3. KURS EKLEME İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course, IFormFile? resimDosyasi)
        {
            var user = await _userManager.GetUserAsync(User);

            ModelState.Remove("ImageUrl");
            ModelState.Remove("InstructorId");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                // A) RESİM YÜKLEME
                if (resimDosyasi != null)
                {
                    string uzanti = Path.GetExtension(resimDosyasi.FileName);
                    string yeniIsim = Guid.NewGuid().ToString() + uzanti;
                    string klasor = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                    if (!Directory.Exists(klasor)) Directory.CreateDirectory(klasor);

                    using (var stream = new FileStream(Path.Combine(klasor, yeniIsim), FileMode.Create))
                    {
                        await resimDosyasi.CopyToAsync(stream);
                    }

                    course.ImageUrl = "/img/" + yeniIsim;
                }
                else
                {
                    course.ImageUrl = "/img/default.jpg"; 
                }

                course.InstructorId = user.Id; 
                course.CreatedDate = DateTime.Now;

                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

           
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }
    }
}