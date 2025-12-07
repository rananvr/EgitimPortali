using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Include için gerekli
using EgitimPortali.Data; // Kendi proje ismine göre düzenle
using EgitimPortali.Models; // Kendi proje ismine göre düzenle

namespace EgitimPortali.Controllers
{
    public class HomeController : Controller
    {
        // 1. Veritabaný baðlantýsýný tanýmlýyoruz
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // GÜVENLÝK KONTROLÜ: Giriþ yapmamýþsa Login sayfasýna þutla!
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Hem Kategoriyi hem de Eðitmeni (Instructor) dahil ederek getiriyoruz
            var kurslar = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .ToList();
            return View(kurslar);
        }
        // KURS DETAY SAYFASI
        public IActionResult Details(int id)
        {
            // Güvenlik kontrolü...
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Account");

            var course = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor) // <-- ÝÞTE BUNU EKLEYECEKSÝN (Eðitmeni dahil et)
                .FirstOrDefault(c => c.Id == id);

            if (course == null) return RedirectToAction("Index");

            return View(course);
        }
        // HAKKIMIZDA SAYFASI METODU
        public IActionResult Privacy()
        {
            // Güvenlik: Giriþ yapmamýþsa Login'e gönder
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        // 1. SATIN ALMA ÝÞLEMÝ
        [HttpPost] // Bu iþlem gizli yapýlmalý (Post)
        public IActionResult BuyCourse(int id)
        {
            // Önce kullanýcý giriþ yapmýþ mý kontrol et
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Giriþ yapmamýþsa Login sayfasýna yolla
                return RedirectToAction("Login", "Account");
            }

            // Daha önce almýþ mý kontrol et?
            var varMi = _context.Enrollments.Any(x => x.UserId == userId && x.CourseId == id);
            if (varMi)
            {
                // Zaten almýþsa direkt kurslarýma gönder
                return RedirectToAction("MyCourses");
            }

            // Satýn almayý kaydet
            var yeniKayit = new Enrollment
            {
                UserId = userId.Value,
                CourseId = id,
                EnrollmentDate = DateTime.Now
            };

            _context.Enrollments.Add(yeniKayit);
            _context.SaveChanges();

            // Ýþlem bitince "Kurslarým" sayfasýna git
            return RedirectToAction("MyCourses");
        }

        // 2. KURSLARIM SAYFASI (Satýn aldýklarýmý listele)
        public IActionResult MyCourses()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Giriþ yapan kiþinin aldýðý kurslarý bul (Include ile kurs detaylarýný da getir)
            var alinanKurslar = _context.Enrollments
                .Include(x => x.Course) // Kurs detayýný getir
                .Where(x => x.UserId == userId) // Sadece bu kullanýcýnýnkileri al
                .Select(x => x.Course) // Bize Enrollment deðil, içindeki Course lazým
                .ToList();

            return View(alinanKurslar);
        }
    }
}