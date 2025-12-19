using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EgitimPortali.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kurs Başlığı")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        [Display(Name = "Kapak Resmi")]
        public string? ImageUrl { get; set; }

        
        public string? InstructorId { get; set; }

       
        public DateTime CreatedDate { get; set; } = DateTime.Now;

      
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}