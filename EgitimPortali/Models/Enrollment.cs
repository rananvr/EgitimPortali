using System;

namespace EgitimPortali.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int CourseId { get; set; } // Hangi Kurs?
        public virtual Course Course { get; set; }

        public int UserId { get; set; } // Hangi Öğrenci?
        public virtual Users User { get; set; }

        public DateTime EnrollmentDate { get; set; } // Ne zaman aldı?
    }
}