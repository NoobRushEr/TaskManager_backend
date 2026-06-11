using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Task;
using TaskManager.Application.Interfaces;

namespace TaskManager.Api.Controllers
{
    [Route("[controller]")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAllTasks()
        {
            IEnumerable<TaskResponseDto?> tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponseDto>> GetTaskById(int id)
        {
            TaskResponseDto? task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<TaskResponseDto>> CreateTask(int userId, [FromBody] CreateTaskDto createTaskDto)
        {
            if (createTaskDto == null) throw new ArgumentNullException(nameof(createTaskDto));

            TaskResponseDto createdTask = await _taskService.CreateTaskAsync(userId, createTaskDto);
            return CreatedAtAction(nameof(GetAllTasks), new { id = createdTask.TaskId }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskResponseDto>> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            TaskResponseDto updatedTask = await _taskService.UpdateTaskAsync(id, updateTaskDto);
            return CreatedAtAction(nameof(GetTaskById), new { id = updatedTask.TaskId }, updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }

        [HttpGet("complete")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetCompleteTasks()
        {
            IEnumerable<TaskResponseDto> tasks = await _taskService.GetCompleteTaskstAsync();
            return Ok(tasks);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasksByUserId(int userId)
        {
            IEnumerable<TaskResponseDto> tasks = await _taskService.GetTasksByUserIdAsync(userId);
            return Ok(tasks);
        }
    }
}