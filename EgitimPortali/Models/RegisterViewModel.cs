using System.ComponentModel.DataAnnotations;

namespace EgitimPortali.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre Tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}
