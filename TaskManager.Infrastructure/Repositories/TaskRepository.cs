
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context){}

    public async Task<IEnumerable<TaskItem>> GetCompleteTaskListAsync()
    {
        return await _context.Tasks.
                    Where(t => t.CompletedAt > DateTime.UtcNow).
                    OrderBy(t => t.DueDate).
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