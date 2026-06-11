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

        [HttpPost]
        public async Task<ActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (createTaskDto == null) throw new ArgumentNullException(nameof(createTaskDto));

            await _taskService.CreateTaskAsync(createTaskDto);
            return CreatedAtAction(nameof(GetAllTasks), createTaskDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            await _taskService.UpdateTaskAsync(id,updateTaskDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}