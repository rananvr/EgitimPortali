using Microsoft.AspNetCore.Identity; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EgitimPortali.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        public string UserId { get; set; } 

        [ForeignKey("UserId")]
     
        public virtual IdentityUser? User { get; set; }

        public DateTime EnrollmentDate { get; set; }
    }
}