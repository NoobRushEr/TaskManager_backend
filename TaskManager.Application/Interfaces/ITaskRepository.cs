using TaskManager.Domain.Entities;
namespace TaskManager.Application.Interfaces;

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetCompleteTaskListAsync();
    Task<IEnumerable<TaskItem>> GetTasksByUserAsync(int userId);
}