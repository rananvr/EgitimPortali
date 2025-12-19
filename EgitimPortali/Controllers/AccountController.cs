using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EgitimPortali.Models;
using System.Security.Claims;

namespace EgitimPortali.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // --- GİRİŞ YAP ---
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded) return RedirectToAction("Index", "Home");
            }

            ViewBag.Hata = "Email veya şifre hatalı!";
            return View(model);
        }

        // --- KAYIT OL ---
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Rolleri oluştur (eğer yoksa)
                if (!await _roleManager.RoleExistsAsync("Ogrenci")) await _roleManager.CreateAsync(new IdentityRole("Ogrenci"));
                if (!await _roleManager.RoleExistsAsync("Egitmen")) await _roleManager.CreateAsync(new IdentityRole("Egitmen"));
                if (!await _roleManager.RoleExistsAsync("Admin")) await _roleManager.CreateAsync(new IdentityRole("Admin"));

                await _userManager.AddToRoleAsync(user, "Ogrenci");
                await _userManager.AddClaimAsync(user, new Claim("FullName", model.FullName));

                return RedirectToAction("Login");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

        
            return View(user);
        }
        // --- EĞİTMEN OLMA SAYFASI (GET) ---
        [HttpGet]
        public IActionResult RegisterInstructor()
        {
            return View();
        }

        // --- EĞİTMEN OLMA İŞLEMİ (POST) ---
        [HttpPost]
        public async Task<IActionResult> RegisterInstructor(RegisterViewModel model)
        {
       
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
               
                if (!await _roleManager.RoleExistsAsync("Egitmen"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Egitmen"));
                }

              
                await _userManager.AddToRoleAsync(user, "Egitmen");

               
                await _userManager.AddClaimAsync(user, new Claim("FullName", model.FullName));

            
                return RedirectToAction("Login");
            }

            return View(model);
        }
    }
}