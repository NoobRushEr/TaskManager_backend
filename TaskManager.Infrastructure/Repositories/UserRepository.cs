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
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<User>> GetCategoriesByUserAsync(int userId)
        {
            return await Task.FromResult(
                _dbSet
                .Include(u => u.Categories)
                .Where(u => u.Id == userId)
                .AsNoTracking()
                .ToList());
        }

        public async Task<User?> GetTaskByUserIdAndTaskIdAsync(int userId, int taskId)
        {

            var user = await _dbSet
                    .Include(u => u.Categories)
                    .ThenInclude(c => c.Tasks)
                    .Where(u => u.Id == userId)
                    .Where(u => u.Categories.Any(c => c.Tasks.Any(t => t.Task_Id == taskId)))
                    .FirstOrDefaultAsync();

            Console.WriteLine($"User with ID {userId} and Task ID {taskId}: {(user != null ? "Found" : "Not Found")}");


            if (user == null) return null;

            // ✅ Filter in memory after fetch
            user.Categories = user.Categories
                .Select(c =>
                {
                    c.Tasks = c.Tasks
                        .Where(t => t.Task_Id == taskId)
                        .ToList();
                    return c;
                })
                .Where(c => c.Tasks.Any())
                .ToList();

            return user;

        }
    }
}