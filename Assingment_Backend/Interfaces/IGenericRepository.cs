using System.Collections.Generic;

namespace Assingment_Backend.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        //Task<T?> GetByIdAsync(int id);
        //Task<( int , IEnumerable<T> )> GetAllAsync( int currentPage );
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }
}
