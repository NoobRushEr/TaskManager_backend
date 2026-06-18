
using TaskManager.Application.DTOs.Task;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Services;

using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

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
            CompletedAt = task.CompletedAt,
            DueDate = task.DueDate,
            Priority = task.Priority?.ToString(),
            Status = task.Status?.ToString(),
            CategoryId = task.CategoryId,
            UserId = task.UserId
        });
    }

    public async Task<IEnumerable<TaskResponseDto>> GetTasksPaginatedAsync(int page, int pageSize)
    {
        var tasks = await _taskRepository.GetTasksPaginatedAsync(page, pageSize);

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
            CompletedAt = task.CompletedAt,
            DueDate = task.DueDate,
            Priority = task.Priority?.ToString(),
            Status = task.Status?.ToString(),
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
            Priority = Enum.TryParse<Priority_>(createTaskDto.Priority, true, out var priority) ? priority : Priority_.Low,
            Status = Enum.TryParse<Status_>(createTaskDto.Status, true, out var status) ? status : Status_.NotStarted,
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
            CompletedAt = task.CompletedAt,
            DueDate = task.DueDate,
            Priority = task.Priority?.ToString(),
            Status = task.Status?.ToString(),
            CategoryId = task.CategoryId,
            UserId = task.UserId
        };
    }

    public async Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId, bool isAdmin)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask == null) return null;

        existingTask.Title = updateTaskDto.Title ?? existingTask.Title;
        existingTask.Description = updateTaskDto.Description ?? existingTask.Description;
        existingTask.Priority = updateTaskDto.Priority is null
            ? existingTask.Priority
            : Enum.TryParse<Priority_>(updateTaskDto.Priority.ToString(), true, out var priority)
                ? priority
                : existingTask.Priority;
        existingTask.Status = updateTaskDto.Status is null
            ? existingTask.Status
            : Enum.TryParse<Status_>(updateTaskDto.Status.ToString(), true, out var status)
                ? status
                : existingTask.Status;
        existingTask.CompletedAt = updateTaskDto.CompletedAt ?? existingTask.CompletedAt;
        existingTask.DueDate = updateTaskDto.DueDate ?? existingTask.DueDate;
        existingTask.CategoryId = updateTaskDto.CategoryId ?? existingTask.CategoryId;

        if (!isAdmin && existingTask.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not the owner of this task Nor an administrator.");
        }

        await _taskRepository.UpdateAsync(existingTask);
        return new TaskResponseDto
        {
            TaskId = existingTask.Task_Id,
            Title = existingTask.Title,
            Description = existingTask.Description,
            CreatedAt = existingTask.CreatedAt,
            CompletedAt = existingTask.CompletedAt,
            DueDate = existingTask.DueDate,
            Priority = existingTask.Priority?.ToString(),
            Status = existingTask.Status?.ToString(),
            CategoryId = existingTask.CategoryId,
            UserId = existingTask.UserId
        };
    }

    public async Task DeleteTaskAsync(int id, int userId, bool isAdmin)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null) return;

        if (!isAdmin && task.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not the owner of this task Nor an administrator.");
        }

        await _taskRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TaskResponseDto>> GetTasksByStatusAsync(Status_ status)
    {
        var tasks = await _taskRepository.GetTasksByStatusAsync(status);
        return tasks.Select(task => new TaskResponseDto
        {
            TaskId = task.Task_Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            DueDate = task.DueDate,
            Priority = task.Priority?.ToString(),
            Status = task.Status?.ToString(),
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
            CompletedAt = task.CompletedAt,
            DueDate = task.DueDate,
            Priority = task.Priority?.ToString(),
            Status = task.Status?.ToString(),
            CategoryId = task.CategoryId,
            UserId = task.UserId
        });
    }

    public async Task<IEnumerable<TaskResponseDto>> GetMyTasksAsync(int userId, bool includeDeleted = false)
    {
        var tasks = await _taskRepository.GetMyTasksAsync(userId, includeDeleted);
        return tasks.Select(task => new TaskResponseDto
        {
            TaskId = task.Task_Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            DueDate = task.DueDate,
            Priority = task.Priority?.ToString(),
            Status = task.Status?.ToString(),
            CategoryId = task.CategoryId,
            UserId = task.UserId
        });
    }

    public async Task<IEnumerable<TaskResponseDto>> UpdateTaskStatusAsync(int taskId, Status_ newStatus, int userId, bool isAdmin)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        Console.WriteLine($"UserId: {userId}, IsAdmin: {isAdmin}, TaskId: {taskId}, NewStatus: {newStatus}, CurrentStatus: {task.Status}");

        if (!isAdmin && task.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not the owner of this task Nor an administrator.");
        }

        Status_ currentStatus = task.Status ?? Status_.NotStarted;
        Console.WriteLine($"Current Status: {currentStatus}, New Status: {newStatus}");

        if (currentStatus == Status_.InProgress && newStatus == Status_.NotStarted)
        {
            throw new InvalidOperationException("Cannot change status from InProgress to NotStarted.");
        }
        else if (currentStatus == Status_.Completed && (newStatus == Status_.InProgress || newStatus == Status_.NotStarted || newStatus == Status_.OnHold))
        {
            throw new InvalidOperationException("Cannot change status from Completed to InProgress or NotStarted or OnHold.");
        }
        else if (currentStatus == Status_.OnHold && newStatus == Status_.NotStarted)
        {
            throw new InvalidOperationException("Cannot change status from OnHold to NotStarted.");
        }
        else if (currentStatus == Status_.NotStarted && newStatus == Status_.Completed)
        {
            throw new InvalidOperationException("Cannot change status from NotStarted to Completed.");
        }

        if (newStatus == Status_.Completed && currentStatus != Status_.Completed)
        {
            task.CompletedAt = DateTime.UtcNow;
        }


        task.Status = newStatus;
        await _taskRepository.UpdateAsync(task);

        return
        [
            new TaskResponseDto
            {
                TaskId = task.Task_Id,
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                CompletedAt = task.CompletedAt,
                DueDate = task.DueDate,
                Priority = task.Priority?.ToString(),
                Status = task.Status?.ToString(),
                CategoryId = task.CategoryId,
                UserId = task.UserId
            }
        ];
    }

    // Dashboard related methods

    public async Task<TaskStatisticsDto> GetTaskStatisticsAsync(int userId)
    {
        var tasks = await _taskRepository.GetTasksByUserAsync(userId);

        var totalTasks = tasks.Count();
        TaskCountByStatusDto countByStatus = new TaskCountByStatusDto
        {
            Completed = tasks.Count(t => t.Status == Status_.Completed),
            InProgress = tasks.Count(t => t.Status == Status_.InProgress),
            NotStarted = tasks.Count(t => t.Status == Status_.NotStarted),
            OnHold = tasks.Count(t => t.Status == Status_.OnHold)
        };

        TaskCountByPriorityDto countByPriority = new TaskCountByPriorityDto
        {
            High = tasks.Count(t => t.Priority == Priority_.High),
            Medium = tasks.Count(t => t.Priority == Priority_.Medium),
            Low = tasks.Count(t => t.Priority == Priority_.Low)
        };


        return new TaskStatisticsDto
        {
            TotalTasks = totalTasks,
            TaskCountByStatus = countByStatus,
            TaskCountByPriority = countByPriority,
            TaskCountByCategory = (List<TaskCountByCategoryDto>)await _taskRepository.GetTasksCountByCategoryAsync(userId)
        };
    }


    public async Task<TaskCountByStatusDto> GetTasksCountAsync(int userId)
    {
        var tasks = await _taskRepository.GetTasksByUserAsync(userId);
        var countByStatus = tasks
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionary(x => x.Status?.ToString() ?? "Unknown", x => x.Count);

        return new TaskCountByStatusDto
        {
            Completed = countByStatus.GetValueOrDefault("Completed", 0),
            InProgress = countByStatus.GetValueOrDefault("InProgress", 0),
            NotStarted = countByStatus.GetValueOrDefault("NotStarted", 0),
            OnHold = countByStatus.GetValueOrDefault("OnHold", 0)
        };
    }

    public async Task<IEnumerable<TaskCountByCategoryDto>> GetTasksCountByCategoryAsync(int userId)
    {
        var tasks = await _taskRepository.GetTasksCountByCategoryAsync(userId);

        return tasks;

    }

    public async Task SoftDeleteTaskAsync(int taskId, int userId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) return;

        if (task.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not the owner of this task.");
        }

        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;
        await _taskRepository.UpdateAsync(task);
    }

    public async Task RestoreTaskAsync(int taskId, bool isAdmin)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) return;

        if (!isAdmin)
        {
            throw new UnauthorizedAccessException("Only administrators can restore tasks.");
        }

        if (!task.IsDeleted)
        {
            throw new InvalidOperationException("Task is not deleted and cannot be restored.");
        }

        task.IsDeleted = false;
        task.DeletedAt = null;
        await _taskRepository.UpdateAsync(task);
    }

}
