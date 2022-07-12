using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToyTrucks.Catalog.Api.DbContexts;
using ToyTrucks.Catalog.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToyTrucks.Catalog.Api.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CatalogDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(CatalogDbContext context, ILogger<CategoryRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddCategory(Category category)
        {
            if (category == null)
            {
                _logger.LogError("Category to add is null");
                throw new ArgumentNullException(nameof(category));
            }
            await _context.Categories.AddAsync(category);

        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesBySize(bool isMini = false)
        {
            return await _context.Categories.Where(c => c.IsMiniTruck == isMini)
                                            .ToListAsync();
        }

        public async Task<Category> GetCategory(int categoryId)
        {
            return await _context.Categories.Where(c => c.CategoryId == categoryId)
                                            .FirstOrDefaultAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void UpdateCategory(Category category)
        {
            if (category == null)
            {
                _logger.LogError("Category to add is null");
                throw new ArgumentNullException(nameof(category));
            }

        }
    }
}
