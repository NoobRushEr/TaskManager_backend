
using TaskManager.Application.DTOs.Task;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync();
    Task<IEnumerable<TaskResponseDto>> GetTasksPaginatedAsync(int page, int pageSize);
    Task<TaskResponseDto?> GetTaskByIdAsync(int id);
    Task<TaskResponseDto> CreateTaskAsync(int userId, CreateTaskDto createTaskDto);
    Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
    Task DeleteTaskAsync(int id);
    
    Task<IEnumerable<TaskResponseDto>> GetCompletedTasksAsync();
    Task<IEnumerable<TaskResponseDto>> GetTasksByUserIdAsync(int userId);
}