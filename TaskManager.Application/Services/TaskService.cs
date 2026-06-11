
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


    public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();

        if (tasks == null || !tasks.Any())
        {
            return Enumerable.Empty<TaskResponseDto>();
        }
        return tasks.Select(task => new TaskResponseDto
        {
            TaskId = task!.Task_Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            CategoryId = task.CategoryId,
            UserId = task.UserId
        });
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null) return null;

        return new TaskResponseDto
        {
            TaskId = task.Task_Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            CategoryId = task.CategoryId,
            UserId = task.UserId
        };
    }

    public async Task<TaskResponseDto> CreateTaskAsync(int userId, CreateTaskDto createTaskDto)
    {
        if (createTaskDto == null) throw new ArgumentNullException(nameof(createTaskDto));

        var task = new TaskItem
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,            
            DueDate = createTaskDto.DueDate,
            CategoryId = createTaskDto.CategoryId,
            UserId = userId
        };
        await _taskRepository.AddAsync(task);
        return new TaskResponseDto
        {
            TaskId = task.Task_Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            CategoryId = task.CategoryId,
            UserId = task.UserId
        };
    }

    public async Task<TaskResponseDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask == null) throw new Exception("Task not found");

        existingTask.Title = updateTaskDto.Title ?? existingTask.Title;
        existingTask.Description = updateTaskDto.Description ?? existingTask.Description;
        existingTask.DueDate = updateTaskDto.DueDate ?? existingTask.DueDate;
        existingTask.CategoryId = updateTaskDto.CategoryId ?? existingTask.CategoryId;

        await _taskRepository.UpdateAsync(existingTask);
        return new TaskResponseDto
        {
            TaskId = existingTask.Task_Id,
            Title = existingTask.Title,
            Description = existingTask.Description,
            CreatedAt = existingTask.CreatedAt,
            DueDate = existingTask.DueDate,
            CategoryId = existingTask.CategoryId,
            UserId = existingTask.UserId
        };
    }

    public async Task DeleteTaskAsync(int id)
    {
        await _taskRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TaskResponseDto>> GetCompleteTaskstAsync()
    {
        var tasks = await _taskRepository.GetCompleteTaskListAsync();
        return tasks.Select(task => new TaskResponseDto
        {
            TaskId = task.Task_Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            CategoryId = task.CategoryId,
            UserId = task.UserId
        });
    }

    public async Task<IEnumerable<TaskResponseDto>> GetTasksByUserIdAsync(int userId)
    {
        var tasks = await _taskRepository.GetTasksByUserAsync(userId);
        return tasks.Select(task => new TaskResponseDto
        {
            TaskId = task.Task_Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            CategoryId = task.CategoryId,
            UserId = task.UserId
        });
    }

}
