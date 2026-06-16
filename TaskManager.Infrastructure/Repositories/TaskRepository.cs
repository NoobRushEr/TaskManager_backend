
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs.Task;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Status_ status)
    {
        return await _context.Tasks.
                    Where(t => t.Status == status).
                    OrderBy(t => t.DueDate).
                    ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetTasksPaginatedAsync(int page, int pageSize)
    {
        return await _context.Tasks.
                    OrderBy(t => t.DueDate).
                    Skip((page - 1) * pageSize).
                    Take(pageSize).
                    ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByUserAsync(int userId)
    {
        return await _context.Tasks.
                    Where(t => t.UserId == userId).
                    OrderBy(t => t.DueDate).
                    ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetMyTasksAsync(int userId)
    {
        return await _context.Tasks.
                    Where(t => t.UserId == userId).
                    OrderBy(t => t.DueDate).
                    ToListAsync();
    }


    // Dashboard related methods
    public async Task<IEnumerable<TaskCountByCategoryDto>> GetTasksCountByCategoryAsync(int userId)
    {
        return await _context.Tasks
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .GroupBy(t => t.CategoryId)
            .Select(g => new TaskCountByCategoryDto
            {
                Category = g.Select(t => t.Category.CategoryName).FirstOrDefault() ?? string.Empty,
                Count = g.Count()
            })
            .ToListAsync();
    }

}