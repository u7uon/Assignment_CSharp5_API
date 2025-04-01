using Assingment_Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Assingment_Backend.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected  readonly DbContext  _context ;
        protected readonly DbSet<T> _dbSet;
        private  const int PAGE_SIZE = 10;

        public GenericRepository( DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity); 
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity); 
        }

        //public virtual async Task< (int , IEnumerable<T> )> GetAllAsync(int currentPage)
        //{
        //    var total = await _dbSet.CountAsync();  

        //    return (total, await _dbSet.Order().Skip( ( currentPage -1 ) * PAGE_SIZE ).Take(PAGE_SIZE).ToListAsync() ) ; 
        //}

        //public async Task<T?> GetByIdAsync(int id)
        //{
        //    return await _dbSet.FindAsync(id);
        //}

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(T entity)
        {
              _dbSet.Update(entity); 
        }
    }
}
