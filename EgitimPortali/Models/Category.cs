using System.ComponentModel.DataAnnotations;

namespace EgitimPortali.Models
{
    public class Category
    {
        [Key] // Birincil anahtar (Primary Key)
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir.")]
        public string Name { get; set; } // Örn: Yazılım, Tasarım

        // İlişki: Bir kategoride birden fazla eğitim olabilir.
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    }
}