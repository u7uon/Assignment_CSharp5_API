﻿using Assignment_Backend.Data;
using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assingment_Backend.DTOs;
using Assingment_Backend.Models;
using Assingment_Backend.Repository;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Assignment_Backend.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        //private readonly ApplicationContext _context;
        private const int PAGE_SIZE = 10;
        public ProductRepository(ApplicationContext context) : base(context) 
        {

        }

        public async Task<(int, IEnumerable<ProductGetDto>)> GetAllAsync(int currentPage, bool status)
        {
            var total = await _dbSet.Where(p => p.IsActive == status).CountAsync();

            var products = await _dbSet
                .Where(p => p.IsActive == status)
                .Select(x => new ProductGetDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    Image = x.Image,
                    CategoryName = x.Category.Name, // Chỉ lấy tên Category
                    BrandName = x.Brand.Name // Chỉ lấy tên Brand
                })
                .Skip((currentPage - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToListAsync();

            return (total, products);
        }

        public async Task<IEnumerable<ProductViewDTO>> GetLatest()
        {
            return await _dbSet.AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select
                (
                    x => new ProductViewDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ImageUrl = x.Image,
                        Price = x.Price
                    }
                ).Take(10).ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }





        public IQueryable<Product> SearchByName (string keyword)
        {
            var productQuery = _dbSet.Include(x => x.Brand).Include(x => x.Category).Where(x => x.Name.ToLower().Contains(keyword.ToLower())).AsNoTracking();

            return productQuery;
        }

       








    }
}
