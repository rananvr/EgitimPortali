using System.Collections.Generic;

namespace EgitimPortali.Repositories
{
    // <T> demek: Bu depo her türlü tablo (Course, Category, User) ile çalışabilir demek.
    public interface IRepository<T> where T : class
    {
        // Parantez içine parametre ekledik:
        List<T> GetAll(string? includeProps = null);
        T GetById(int id);          // ID ile Bul
        void Add(T entity);         // Ekle
        void Update(T entity);      // Güncelle
        void Delete(int id);        // Sil
    }
}