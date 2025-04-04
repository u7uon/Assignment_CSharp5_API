using Assignment_Backend.Data;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.Data.SqlClient;
using Assignment_Backend.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Assingment_Backend.DTOs;
using Assingment_Backend.Repository;

namespace Assignment_Backend.Repository
{
    public class CategoryRepository : GenericRepository<Category> ,  ICategoryRepository
    {
        //private readonly ApplicationContext context;

        public CategoryRepository(ApplicationContext context) :base (context)
        {

        }


        public async Task< (int , IEnumerable<Category> )> GetCategoriesAsync(int currentPage)
        {
            if (currentPage < 1)
                currentPage = 1;

            var total = await _dbSet.CountAsync();

            var categories = await _dbSet
                .OrderBy(x => x.Id).
                Skip((currentPage - 1) * PAGE_SIZE).
                Take(PAGE_SIZE).
                Select(
                    x => new Category
                    {
                        Id = x.Id ,
                        Name = x.Name,
                        Description = x.Description
                    }
                )
                .AsNoTracking()
                .ToListAsync();
            return (total, categories );
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            return await _dbSet
                .Include(x => x.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        }
    }

