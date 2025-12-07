using Microsoft.AspNetCore.Mvc;
using EgitimPortali.Data;
using EgitimPortali.Models;

namespace EgitimPortali.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. KAYIT OL (SAYFAYI GÖSTER)
        public IActionResult Register()
        {
            return View();
        }

        // 1. KAYIT OL (İŞLEMİ YAP)
        [HttpPost]
        public IActionResult Register(Users user)
        {
            // Yeni kayıt olan herkes "Ogrenci" olsun
            user.Role = "Ogrenci";

            // Bu email veritabanında var mı kontrolü
            if (_context.Users.Any(x => x.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Bu email adresi zaten kullanılıyor!");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // 2. GİRİŞ YAP (SAYFAYI GÖSTER)
        public IActionResult Login()
        {
            return View();
        }

        // 2. GİRİŞ YAP (İŞLEMİ YAP)
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Veritabanında bu email ve şifreye sahip biri var mı?
            var user = _context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);

            if (user != null)
            {
                // Kullanıcı bulundu! Oturum (Session) başlatalım.
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.FullName);

                // ROL KONTROLÜ: Eğitmense Panele, Öğrenciyse Anasayfaya
                if (user.Role == "Egitmen")
                {
                    return RedirectToAction("Index", "Courses"); // Admin Paneli
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Vitrin
                }
            }

            ViewBag.Hata = "Email veya şifre hatalı!";
            return View();
        }
        // --- EĞİTMEN KAYIT KISMI ---

        // 1. Eğitmen Kayıt Sayfasını Göster
        public IActionResult RegisterInstructor()
        {
            return View();
        }

        // 2. Eğitmen Kaydını İşle
        [HttpPost]
        public IActionResult RegisterInstructor(Users user)
        {

            // KONTROL: Aynı kontrolü buraya da ekliyoruz
            if (_context.Users.Any(x => x.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Bu email adresi zaten sistemde kayıtlı.");
                return View(user);
            }

            // Burası kritik nokta: Rolü el ile "Egitmen" yapıyoruz.
            user.Role = "Egitmen";

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                // Kayıt başarılı, giriş sayfasına yönlendir
                return RedirectToAction("Login");
            }
            return View(user);
        }
        // 3. ÇIKIŞ YAP
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Oturumu temizle
            return RedirectToAction("Index", "Home");
        }
        // --- PROFİL İŞLEMLERİ ---

        // 1. Profil Sayfasını Göster
        public IActionResult Profile()
        {
            // Giriş yapanın ID'sini bul
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            // Veritabanından o kişiyi getir
            var user = _context.Users.Find(userId);
            return View(user);
        }

        // 2. Profil Güncelleme İşlemi
        [HttpPost]
        public IActionResult Profile(Users gelenUser)
        {
            // Veritabanındaki gerçek kullanıcıyı bul
            var veritabanindakiUser = _context.Users.Find(gelenUser.Id);

            if (veritabanindakiUser != null)
            {
                // Sadece izin verdiğimiz alanları güncelle
                // (Email ve Rolü değiştirmiyoruz, güvenlik için sabit kalsın)
                veritabanindakiUser.FullName = gelenUser.FullName;
                veritabanindakiUser.Password = gelenUser.Password;

                _context.SaveChanges();

                // Session'daki ismi de güncelle ki menüde hemen değişsin
                HttpContext.Session.SetString("UserName", veritabanindakiUser.FullName);

                ViewBag.Mesaj = "Bilgileriniz başarıyla güncellendi! ✅";

                return View(veritabanindakiUser);
            }

            return RedirectToAction("Login");
        }
    }
}