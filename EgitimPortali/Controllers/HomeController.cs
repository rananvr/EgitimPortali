using EgitimPortali.Data;
using EgitimPortali.Hubs;
using EgitimPortali.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using EgitimPortali.Repositories; 

namespace EgitimPortali.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<SatisHub> _hubContext;

        
        private readonly IRepository<Enrollment> _enrollmentRepository;

        public HomeController(
              ApplicationDbContext context,
              UserManager<IdentityUser> userManager,
              IHubContext<SatisHub> hubContext,
              IRepository<Enrollment> enrollmentRepository)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
            _enrollmentRepository = enrollmentRepository;
        }

        // 1. ANA SAYFA
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

            return View(await kurslarQuery.ToListAsync());
        }

        // 2. AJAX METODU (Kategoriler için)
        public async Task<IActionResult> KurslariGetir(int? kategoriId)
        {
            var kurslar = _context.Courses.Include(c => c.Category).AsQueryable();

            if (kategoriId.HasValue && kategoriId > 0)
            {
                kurslar = kurslar.Where(c => c.CategoryId == kategoriId);
            }

            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var kayitliKursIdleri = await _context.Enrollments
                                               .Where(x => x.UserId == userId)
                                               .Select(x => x.CourseId)
                                               .ToListAsync();
                kurslar = kurslar.Where(c => !kayitliKursIdleri.Contains(c.Id));
            }

            return PartialView("_KursListesi", await kurslar.ToListAsync());
        }

        // 3. KURSLARIM SAYFASI
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
                if (enrollment.Course != null) myCourses.Add(enrollment.Course);
            }
            return View(myCourses);
        }

        // 4. DETAY SAYFASI
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.Include(c => c.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (course == null) return NotFound();
            return View(course);
        }

        // 5. SATIN ALMA 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Enroll(int id)
        {
            var userId = _userManager.GetUserId(User);
            var varMi = await _context.Enrollments.AnyAsync(x => x.UserId == userId && x.CourseId == id);

            if (varMi) return RedirectToAction("MyCourses");

            var yeniKayit = new Enrollment
            {
                CourseId = id,
                UserId = userId,
                EnrollmentDate = DateTime.Now
            };

         
            _enrollmentRepository.Add(yeniKayit);
            await _context.SaveChangesAsync();

            
            await _hubContext.Clients.All.SendAsync("SatisYapildi", "Yeni bir kurs kaydý oluþturuldu!");

            return RedirectToAction("MyCourses");
        }

        public IActionResult Privacy() => View();
    }
}