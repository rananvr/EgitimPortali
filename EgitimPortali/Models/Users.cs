using System.ComponentModel.DataAnnotations;

namespace EgitimPortali.Models
{
    public class Users
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Password { get; set; }


        public string Role { get; set; } = "Ogrenci";
    }
}