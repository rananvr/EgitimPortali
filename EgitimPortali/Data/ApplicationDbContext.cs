using EgitimPortali.Models;
using Microsoft.EntityFrameworkCore;
using EgitimPortali.Models; // Buraya kendi proje ismimi yazıyorum

namespace EgitimPortali.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // İşte modellerini buraya ekliyoruz. 
        // Her biri veritabanında bir tablo olacak.


        public DbSet<Course> Courses { get; set; }      // Kurslar tablosu
        public DbSet<Category> Categories { get; set; } // Kategoriler tablosu
        public DbSet<Enrollment> Enrollments { get; set; } //Satın almalar tablosu
        public DbSet<Users> Users { get; set; }         // Kullanıcılar tablosu
    }
}