using System.ComponentModel.DataAnnotations;
namespace EgitimPortali.Models
{
    public class ProfileViewModel
    {
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string? ConfirmPassword { get; set; }
    }
}