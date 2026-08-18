using System;
using System.Collections.Generic;
using TaskManager.Application.Interfaces;
using Xunit;
using Moq;
using TaskManager.Application.Services;
using TaskManager.Application.DTOs.Task;
using FluentAssertions;


namespace TaskManager.Application.Tests.Services
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task GetAllTasksAsync_hasTasks()
        {
            var mockTaskRepository = new Mock<ITaskRepository>();
            var tasks = new List<TaskManager.Domain.Entities.TaskItem>
            {
                new TaskManager.Domain.Entities.TaskItem { Task_Id = 1, Title = "Task 1", Description = "Description 1", CreatedAt = DateTime.Now},
                new TaskManager.Domain.Entities.TaskItem { Task_Id = 2, Title = "Task 2", Description = "Description 2", CreatedAt = DateTime.Now}
            };
            mockTaskRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tasks);

            var taskService = new TaskService(mockTaskRepository.Object);

            var result = await taskService.GetAllTasksAsync() as IEnumerable<TaskResponseDto>;

            Assert.NotNull(result);
            result.Should().HaveCount(2);
            result.First().Title.Should().Be("Task 1");
            result.First().Description.Should().Be("Description 1");

        }

        [Fact]
        public async Task GetAllTasksAsync_noTasks()
        {
            var mockTaskRepository = new Mock<ITaskRepository>();
            var tasks = new List<TaskManager.Domain.Entities.TaskItem>();
            mockTaskRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tasks);

            var taskService = new TaskService(mockTaskRepository.Object);

            var result = await taskService.GetAllTasksAsync() as IEnumerable<TaskResponseDto>;

            Assert.NotNull(result);
            result.Should().BeEmpty();
        }

        public async Task GetTaskByIdAsync_taskExists()
        {
            var mockTaskRepository = new Mock<ITaskRepository>();
            var task = new TaskManager.Domain.Entities.TaskItem { Task_Id = 1, Title = "Task 1", Description = "Description 1", CreatedAt = DateTime.Now };
            mockTaskRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(task);

            var taskService = new TaskService(mockTaskRepository.Object);

            var result = await taskService.GetTaskByIdAsync(1) as TaskResponseDto;

            Assert.NotNull(result);
            result.Title.Should().Be("Task 1");
            result.Description.Should().Be("Description 1");
        }

        public async Task GetTaskByIdAsync_taskDoesNotExist()
        {
            var mockTaskRepository = new Mock<ITaskRepository>();
            mockTaskRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((TaskManager.Domain.Entities.TaskItem?)null);

            var taskService = new TaskService(mockTaskRepository.Object);

            var result = await taskService.GetTaskByIdAsync(1) as TaskResponseDto;

            Assert.Null(result);

        }
    }
}