using TaskManager.Application.DTOs.Task;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
namespace TaskManager.Application.Interfaces;

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Status_ status);
    Task<IEnumerable<TaskItem>> GetTasksByUserAsync(int userId);
    Task<IEnumerable<TaskItem>> GetTasksPaginatedAsync(int page, int pageSize);
    Task<IEnumerable<TaskItem>> GetMyTasksAsync(int userId);
    Task<IEnumerable<TaskCountByCategoryDto>> GetTasksCountByCategoryAsync(int userId);
}