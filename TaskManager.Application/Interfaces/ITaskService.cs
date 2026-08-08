
using TaskManager.Application.DTOs.Task;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync();
    Task<IEnumerable<TaskResponseDto>> GetTasksPaginatedAsync(int page, int pageSize);
    Task<TaskResponseDto?> GetTaskByIdAsync(int id);
    Task<TaskResponseDto> CreateTaskAsync(int userId, CreateTaskDto createTaskDto);
    Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId, bool isAdmin);
    Task DeleteTaskAsync(int id, int userId, bool isAdmin);
    
    Task<IEnumerable<TaskResponseDto>> GetTasksByUserIdAsync(int userId);
    Task<IEnumerable<TaskResponseDto>> GetMyTasksAsync(int userId, bool includeDeleted = false);

    Task<IEnumerable<TaskResponseDto>> UpdateTaskStatusAsync(int taskId, Status_ newStatus, int userId, bool isAdmin);
    
    
    Task<TaskStatisticsDto> GetTaskStatisticsAsync(int userId);
    Task<IEnumerable<TaskResponseDto>> GetTasksByStatusAsync(Status_ status);
    Task<TaskCountByStatusDto> GetTasksCountAsync(int userId);
    Task<IEnumerable<TaskCountByCategoryDto>> GetTasksCountByCategoryAsync(int userId);
    Task SoftDeleteTaskAsync(int taskId, int userId, bool isAdmin);
    Task RestoreTaskAsync(int taskId, bool isAdmin);

    Task PurgeSoftDeletedTasksAsync(CancellationToken cancellationToken = default);
}