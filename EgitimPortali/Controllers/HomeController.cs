using EgitimPortali.Data;
using EgitimPortali.Hubs;
using EgitimPortali.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EgitimPortali.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<SatisHub> _hubContext; 

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<SatisHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext; 
        }

        // 1. ANA SAYFA: Giriþ yapmýþsa kayýtlý kurslarý göstermez
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

         
            var kurslarQuery = _context.Courses.Include(c => c.Category).AsQueryable();

            if (userId != null)
            {
                var kayitliKursIdleri = await _context.Enrollments
                                                .Where(x => x.UserId == userId)
                                                .Select(x => x.CourseId)
                                                .ToListAsync();

                kurslarQuery = kurslarQuery.Where(c => !kayitliKursIdleri.Contains(c.Id));
            }

            var kurslar = await kurslarQuery.ToListAsync();
            return View(kurslar);
        }

        // 2. KURSLARIM SAYFASI: Sadece kayýt olunanlarý gösterir
        [Authorize]
        public async Task<IActionResult> MyCourses()
        {
            var userId = _userManager.GetUserId(User);

            var userEnrollments = await _context.Enrollments
                                                .Include(e => e.Course) 
                                                .Where(e => e.UserId == userId)
                                                .ToListAsync();

            var myCourses = new List<Course>();
            foreach (var enrollment in userEnrollments)
            {
                if (enrollment.Course != null)
                {
                    myCourses.Add(enrollment.Course);
                }
            }

            return View(myCourses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // --- DETAY SAYFASI ---
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }

        // --- SATIN ALMA (KAYIT OLMA) ÝÞLEMÝ ---
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Enroll(int id)
        {
            var userId = _userManager.GetUserId(User);

            var varMi = await _context.Enrollments
                .AnyAsync(x => x.UserId == userId && x.CourseId == id);
            await _hubContext.Clients.All.SendAsync("SatisYapildi", "Yeni bir kurs kaydý oluþturuldu!");
            if (varMi)
            {
                return RedirectToAction("MyCourses");
            }

            // Yeni Kayýt Oluþtur
            var yeniKayit = new Enrollment
            {
                CourseId = id,
                UserId = userId,
                EnrollmentDate = DateTime.Now
            };

            _context.Enrollments.Add(yeniKayit);
            await _context.SaveChangesAsync();

          
            return RedirectToAction("MyCourses");
        }
    }
}