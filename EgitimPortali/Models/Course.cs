using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EgitimPortali.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Eğitim adı zorunludur.")]
        public string Title { get; set; }

        public string Description { get; set; } // Açıklama

        public decimal Price { get; set; } // Fiyat

        public string? ImageUrl { get; set; } // Eğitim görseli (Dosya yolu tutulacak)

        // İlişki: Hangi kategoriye ait?
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // Hangi Eğitmen? (Users tablosuna bağlanıyoruz)
        public int? InstructorId { get; set; }
        public virtual Users? Instructor { get; set; }
    }
}