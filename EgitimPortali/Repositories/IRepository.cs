using System.Collections.Generic;

namespace EgitimPortali.Repositories
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll(string? includeProps = null);
        T GetById(int id);          
        void Add(T entity);        
        void Update(T entity);      
        void Delete(int id);        
    }
}