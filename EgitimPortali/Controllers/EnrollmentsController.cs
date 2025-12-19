using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EgitimPortali.Data;
using EgitimPortali.Models;

namespace EgitimPortali.Controllers
{
    [Authorize(Roles = "Admin, Egitmen")]
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Enrollments.Include(e => e.Course).Include(e => e.User);
            return View(await applicationDbContext.ToListAsync());
        }
    }
}