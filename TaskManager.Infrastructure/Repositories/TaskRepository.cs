
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TaskItem>> GetCompleteTaskListAsync()
    {
        return await _context.Tasks.
                    Where(t => t.CompletedAt.HasValue).
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
}