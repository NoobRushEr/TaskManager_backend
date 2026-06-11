using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<TaskItem>> GetTasksByCategoryAsync(int categoryId)
        {
            return await _dbSet.Include(c => c.Tasks)
                .Where(c => c.CategoryId == categoryId)
                .SelectMany(c => c.Tasks)
                .ToListAsync();
        }
    }
}