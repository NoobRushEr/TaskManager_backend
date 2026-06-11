
using TaskManager.Application.DTOs.Task;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Services;
using TaskManager.Domain.Entities;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    // Implement service methods that use _taskRepository to perform operations

    public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync()
    {
        var tasks = await _taskRepository.GetAllTasksAsync();

        if (tasks == null || !tasks.Any())
        {
            return Enumerable.Empty<TaskResponseDto>();
        }

        // Map tasks to TaskResponseDto and return
        return tasks.Select(t => new TaskResponseDto
        {
            Id = t.Task_Id,
            Title = t.Title,
            Description = t.Description,
            DueDate = t.DueDate,
            CategoryId = t.CategoryId,
            CategoryName = t.Category?.CategoryName,
            UserId = t.UserId
        });
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(int id)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id);
        if (task == null) return null;

        return new TaskResponseDto
        {
            Id = task.Task_Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            CategoryId = task.CategoryId,
            CategoryName = task.Category?.CategoryName,
            UserId = task.UserId
        };
    }

    public async Task<CreateTaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        if (createTaskDto == null) throw new ArgumentNullException(nameof(createTaskDto));

        var task = new TaskItem
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,            
            DueDate = createTaskDto.DueDate,
            CategoryId = createTaskDto.CategoryId
        };
        await _taskRepository.CreateTaskAsync(task);
        return createTaskDto;
    }

    public async Task UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
    {
        var existingTask = await _taskRepository.GetTaskByIdAsync(id);
        if (existingTask == null) throw new Exception("Task not found");

        existingTask.Title = updateTaskDto.Title ?? existingTask.Title;
        existingTask.Description = updateTaskDto.Description ?? existingTask.Description;
        existingTask.DueDate = updateTaskDto.DueDate ?? existingTask.DueDate;
        existingTask.CategoryId = updateTaskDto.CategoryId ?? existingTask.CategoryId;

        await _taskRepository.UpdateTaskAsync(existingTask);
    }

    public async Task DeleteTaskAsync(int id)
    {
        await _taskRepository.DeleteTaskAsync(id);
    }

}
