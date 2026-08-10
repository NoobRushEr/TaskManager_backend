
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

    public async Task<IEnumerable<TaskItem>> GetMyTasksAsync(int userId, bool includeDeleted = false, bool includeArchived = false)
    {
        IQueryable<TaskItem> query = _context.Tasks.Where(t => t.UserId == userId);

        if (includeDeleted || includeArchived)
        {
            query = query.IgnoreQueryFilters();

            if (!includeDeleted)
            {
                query = query.Where(t => !t.IsDeleted);
            }
            if (!includeArchived)
            {
                query = query.Where(t => !t.IsArchived);
            }
        }

        return await query.OrderBy(t => t.DueDate).ToListAsync();
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

    public async Task PurgeSoftDeletedTasksAsync(CancellationToken cancellationToken = default)
    {
        // Only purge tasks that have been soft-deleted for more than 30 days
        var cutoffDate = DateTime.UtcNow.AddDays(-30);

        var softDeletedTasks = await _context.Tasks
            .IgnoreQueryFilters()
            .Where(t => t.IsDeleted && t.DeletedAt < cutoffDate)
            .ToListAsync(cancellationToken);

        if (softDeletedTasks.Any())
        {
            _context.Tasks.RemoveRange(softDeletedTasks);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task ArchiveCompletedTasksAsync(CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        int rowsAffected;
        do
        {
            // ExecuteUpdateAsync updates directly in DB, bypassing tracking
            rowsAffected = await _context.Tasks
                .IgnoreQueryFilters()
                .Where(t => t.Status == Status_.Completed && !t.IsArchived && t.CompletedAt < cutoffDate)
                .Take(1000)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.IsArchived, true)
                    .SetProperty(t => t.ArchivedAt, DateTime.UtcNow), cancellationToken);

            if (rowsAffected > 0)
            {
                await Task.Delay(100, cancellationToken); // Breather window for locking
            }
        } while (rowsAffected > 0);
    }
}