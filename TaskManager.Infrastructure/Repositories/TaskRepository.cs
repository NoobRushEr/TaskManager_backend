
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    // Implement repository methods here
    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        // Let's ask EF Core to count the rows BEFORE it tries to convert them.
        var totalRows = await _context.Tasks.CountAsync();

        // Put a breakpoint on this line, or just let it crash so you can see if totalRows > 1
        Console.WriteLine($"EF Core sees {totalRows} tasks in the database.");

        return await _context.Tasks.
                        AsNoTracking().
                        Include(t => t.Category).
                        ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        return await _context.Tasks.AsNoTracking().Include(t => t.Category).FirstOrDefaultAsync(t => t.Task_Id == id);
    }

    public async Task CreateTaskAsync(TaskItem createTaskDto)
    {
        _context.Tasks.Add(createTaskDto);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTaskAsync(TaskItem updateTaskDto)
    {
        _context.Tasks.Update(updateTaskDto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int id)
    {
        var task = await GetTaskByIdAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

}