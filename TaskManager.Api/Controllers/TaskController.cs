using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Task;
using TaskManager.Application.Extensions;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAllTasks()
        {
            IEnumerable<TaskResponseDto?> tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }


        [HttpGet("paged")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasksPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            IEnumerable<TaskResponseDto?> tasks = await _taskService.GetTasksPaginatedAsync(page, pageSize);
            return Ok(tasks);
        }


        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<TaskResponseDto>> GetTaskById(int id)
        {
            TaskResponseDto? task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }



        [HttpPost]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (createTaskDto == null) throw new ArgumentNullException(nameof(createTaskDto));

            int userId = User.GetUserId();

            TaskResponseDto createdTask = await _taskService.CreateTaskAsync(userId, createTaskDto);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.TaskId }, createdTask);
        }

        [HttpPut("{task_id}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<TaskResponseDto>> UpdateTask(int task_id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            int userId = User.GetUserId();
            bool isAdmin = User.IsInRole("Admin") ? true : false;
            TaskResponseDto? updatedTask = await _taskService.UpdateTaskAsync(task_id, updateTaskDto, userId, isAdmin);
            if (updatedTask == null)
            {
                return NotFound();
            }
            return Ok(updatedTask);
        }

        [HttpDelete("{task_id}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult> DeleteTask(int task_id)
        {
            int userId = User.GetUserId();
            bool isAdmin = User.IsInRole("Admin") ? true : false;
            await _taskService.DeleteTaskAsync(task_id, userId, isAdmin);
            return NoContent();
        }


        [HttpGet("user/{userId}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasksByUserId(int userId)
        {
            IEnumerable<TaskResponseDto> tasks = await _taskService.GetTasksByUserIdAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("tasks")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetMyTasks([FromQuery] bool includeDeleted = false)
        {
            int userId = User.GetUserId();
            IEnumerable<TaskResponseDto> tasks = await _taskService.GetMyTasksAsync(userId, includeDeleted);
            return Ok(tasks);
        }

        [HttpPut("update-status")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> UpdateTaskStatus([FromBody] UpdateTaskStatusDto updateTaskStatusDto)
        {
            int userId = User.GetUserId();
            bool isAdmin = User.IsInRole("Admin") ? true : false;
            Console.WriteLine($"UserId: {userId}, IsAdmin: {isAdmin}, TaskId: {updateTaskStatusDto.task_id}, NewStatus: {updateTaskStatusDto.Status}");
            var updatedTasks = await _taskService.UpdateTaskStatusAsync(updateTaskStatusDto.task_id, updateTaskStatusDto.Status, userId, isAdmin);
            return Ok(updatedTasks);
        }

        // Dashboard endpoint to get task statistics
        [HttpGet("dashboard")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<TaskStatisticsDto>> GetTaskStatistics(){

            int userId = User.GetUserId();
            var taskStatistics = await _taskService.GetTaskStatisticsAsync(userId);
            return Ok(taskStatistics);
        }

        [HttpGet("tasks-count")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<TaskCountByStatusDto>> GetTasksCount()
        {
            int userId = User.GetUserId();
            var tasksCount = await _taskService.GetTasksCountAsync(userId);
            return Ok(tasksCount);
        }

        [HttpGet("tasks-by-status/{status}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasksByStatus([FromRoute] Status_ status)
        {
            IEnumerable<TaskResponseDto> tasks = await _taskService.GetTasksByStatusAsync(status);
            return Ok(tasks);
        }

        [HttpGet("tasks-count-by-category")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<IEnumerable<TaskCountByCategoryDto>>> GetTasksByCategory()
        {
            int userId = User.GetUserId();
            IEnumerable<TaskCountByCategoryDto> tasks = await _taskService.GetTasksCountByCategoryAsync(userId);
            return Ok(tasks);
        }

        //soft delete endpoint
        [HttpDelete("soft-delete/{task_id}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult> SoftDeleteTask([FromRoute] int task_id)
        {
            int userId = User.GetUserId();
            bool isAdmin = User.IsInRole("Admin") ? true : false;
            await _taskService.SoftDeleteTaskAsync(task_id, userId, isAdmin);
            return Ok();
        }

        [HttpPut("restore/{task_id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> RestoreTask([FromRoute] int task_id)
        {
            bool isAdmin = User.IsInRole("Admin") ? true : false;
            await _taskService.RestoreTaskAsync(task_id, isAdmin);
            return Ok();
        }

    }
}