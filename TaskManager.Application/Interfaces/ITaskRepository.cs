
namespace TaskManager.Application.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskManager.Domain.Entities.TaskItem>> GetAllTasksAsync();
    Task<TaskManager.Domain.Entities.TaskItem?> GetTaskByIdAsync(int id);
    Task CreateTaskAsync(TaskManager.Domain.Entities.TaskItem createTaskDto);
    Task UpdateTaskAsync(TaskManager.Domain.Entities.TaskItem updateTaskDto);
    Task DeleteTaskAsync(int id);
}