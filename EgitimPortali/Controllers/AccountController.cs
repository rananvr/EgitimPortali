using EgitimPortali.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        // --- GİRİŞ VE KAYIT KISMI (STANDART) ---
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
            ViewBag.Hata = "Hatalı giriş!";
            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
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
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult RegisterInstructor() => View();

        [HttpPost]
        public async Task<IActionResult> RegisterInstructor(RegisterViewModel model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Egitmen")) await _roleManager.CreateAsync(new IdentityRole("Egitmen"));
                await _userManager.AddToRoleAsync(user, "Egitmen");
                await _userManager.AddClaimAsync(user, new Claim("FullName", model.FullName));
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // --- YENİ EKLENEN PROFİL KISMI ---
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");
            return View(new ProfileViewModel { Email = user.Email });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                if (model.Email != user.Email)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    await _userManager.UpdateAsync(user);
                }

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    if (string.IsNullOrEmpty(model.CurrentPassword))
                    {
                        ModelState.AddModelError("", "Mevcut şifrenizi girmelisiniz.");
                        return View(model);
                    }
                    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
                        return View(model);
                    }
                }
                TempData["SuccessMessage"] = "Profil güncellendi.";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Profile");
            }
            return View(model);
        }
    }
}