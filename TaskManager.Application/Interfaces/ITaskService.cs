
using TaskManager.Application.DTOs.Task;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync();
    Task<TaskResponseDto?> GetTaskByIdAsync(int id);
    Task<CreateTaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);
    Task UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
    Task DeleteTaskAsync(int id);
}