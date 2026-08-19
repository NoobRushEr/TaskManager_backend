using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs.Task;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using Xunit;

namespace TaskManager.Api.IntegrationTests
{
    public class TaskIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public TaskIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

            // Seed base requirements like the test user
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            using var db = _factory.CreateDbContext();

            // Clean tables to start with a fresh slate for this test class
            db.Database.ExecuteSqlRaw("TRUNCATE TABLE tasks RESTART IDENTITY CASCADE;");
            db.Database.ExecuteSqlRaw("TRUNCATE TABLE users RESTART IDENTITY CASCADE;");

            // Seed user with ID = 1 (matches ClaimTypes.NameIdentifier in TestAuthHandler)
            var user = new User
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                PasswordHash = "password_hash_dummy_value",
                Roles = new List<Role_> { Role_.User }
            };

            db.Users.Add(user);
            db.SaveChanges();

            // Clear the in-memory cache to prevent cross-test pollution
            var cache = _factory.Services.GetService(typeof(Microsoft.Extensions.Caching.Memory.IMemoryCache)) as Microsoft.Extensions.Caching.Memory.IMemoryCache;
            if (cache is Microsoft.Extensions.Caching.Memory.MemoryCache concreteCache)
            {
                concreteCache.Compact(1.0); // Evict 100% of cached entries
            }
        }

        public void Dispose()
        {
            // Optional: clean up database after class run
        }

        [Fact]
        public async Task GetMyTasks_ReturnsSuccessAndSeededTasks()
        {
            // Arrange
            using (var db = _factory.CreateDbContext())
            {
                var testTask = new TaskItem
                {
                    Title = "Integration Test Task",
                    Description = "Verify API behaves correctly with database integration",
                    Status = Status_.NotStarted,
                    Priority = Priority_.Medium,
                    UserId = 1, // Matches our seeded User
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5)
                };
                db.Tasks.Add(testTask);
                db.SaveChanges();
            }

            // Act
            var response = await _client.GetAsync("/api/Task/tasks");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<TaskResponseDto>>();
            Assert.NotNull(tasks);
            var taskList = Assert.Single(tasks);
            Assert.Equal("Integration Test Task", taskList.Title);
            Assert.Equal("NotStarted", taskList.Status);
        }

        [Fact]
        public async Task GetSoftdeleteedTasks_Test()
        {
            int taskId;
            // Given (Arrange)
            using (var db = _factory.CreateDbContext())
            {
                var testTask = new TaskItem
                {
                    Title = "Integration Test Task Soft Delete",
                    Description = "Verify API behaves correctly with database integration",
                    Status = Status_.NotStarted,
                    Priority = Priority_.Medium,
                    UserId = 1, // Matches our seeded User
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5)
                };
                db.Tasks.Add(testTask);
                db.SaveChanges();
                taskId = testTask.Task_Id;
            }


            // When (Act)
            var DeleteResponse = await _client.DeleteAsync($"/api/Task/soft-delete/{taskId}");

            Assert.Equal(HttpStatusCode.OK, DeleteResponse.StatusCode);

            var response = await _client.GetAsync("/api/Task/tasks?includeDeleted=true");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Then (Assert)

            var task = await response.Content.ReadFromJsonAsync<IEnumerable<TaskResponseDto>>();
            Assert.NotNull(task);
            var taskList = Assert.Single(task);
            Assert.Equal("Integration Test Task Soft Delete", taskList.Title);
        }

        [Fact]
        public async Task GetTasksExcludeArchivedtasks_Test()
        {
            int taskId;
            // Given (Arrange)
            using (var db = _factory.CreateDbContext())
            {
                var testTask = new TaskItem
                {
                    Title = "Integration Test Task Soft Delete",
                    Description = "Verify API behaves correctly with database integration",
                    Status = Status_.NotStarted,
                    Priority = Priority_.Medium,
                    IsArchived = true,
                    UserId = 1, // Matches our seeded User
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5)
                };

                db.Tasks.Add(testTask);
                db.SaveChanges();
                taskId = testTask.Task_Id;
            }


            // When (Act)
            var response = await _client.GetAsync("/api/Task/tasks");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Then (Assert)

            var task = await response.Content.ReadFromJsonAsync<IEnumerable<TaskResponseDto>>();
            Assert.NotNull(task);
            Assert.Empty(task);
        }

        [Fact]
        public async Task GetTasksShouldExcludeDeletedAndArchivedTasks_Test()
        {
            // Given (Arrange)
            using (var db = _factory.CreateDbContext())
            {
                var testTask1 = new TaskItem
                {
                    Title = "Integration Test Task Archived",
                    Description = "Verify API behaves correctly with database integration",
                    Status = Status_.NotStarted,
                    Priority = Priority_.Medium,
                    IsArchived = true,
                    UserId = 1, // Matches our seeded User
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5)
                };

                var testTask2 = new TaskItem
                {
                    Title = "Integration Test Task Soft Deleted",
                    Description = "Verify API behaves correctly with database integration",
                    Status = Status_.NotStarted,
                    Priority = Priority_.Medium,
                    IsDeleted = true,
                    UserId = 1, // Matches our seeded User
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5)
                };

                var testTask3 = new TaskItem
                {
                    Title = "Integration Test Task Active",
                    Description = "Verify API behaves correctly with database integration",
                    Status = Status_.NotStarted,
                    Priority = Priority_.Medium,
                    UserId = 1, // Matches our seeded User
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5)
                };

                db.Tasks.AddRange(testTask1, testTask2, testTask3);
                db.SaveChanges();
            }


            // When (Act)
            var response = await _client.GetAsync("/api/Task/tasks");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Then (Assert)

            var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<TaskResponseDto>>();
            Assert.NotNull(tasks);
            var taskList = Assert.Single(tasks);
            Assert.Equal("Integration Test Task Active", taskList.Title);
        }

        [Fact]
        public async Task DbIgnoreQueryFilters_Test()
        {
            // Given (Arrange)
            using (var db = _factory.CreateDbContext())
            {
                var testTask1 = new TaskItem
                {
                    Title = "Integration Test Task Archived",
                    Description = "Verify API behaves correctly with database integration",
                    Status = Status_.NotStarted,
                    Priority = Priority_.Medium,
                    IsArchived = true,
                    UserId = 1, // Matches our seeded User
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5)
                };

                var testTask2 = new TaskItem
                {
                    Title = "Integration Test Task Soft Deleted",
                    Description = "Verify API behaves correctly with database integration",
                    Status = Status_.NotStarted,
                    Priority = Priority_.Medium,
                    IsDeleted = true,
                    UserId = 1, // Matches our seeded User
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5)
                };

                db.Tasks.AddRange(testTask1, testTask2);
                db.SaveChanges();
            }


            // When (Act)
            using (var dbContext = _factory.CreateDbContext())
            {
                var allTasksIgnoringFilters = await dbContext.Tasks.IgnoreQueryFilters().ToListAsync();

                // Then (Assert)
                Assert.Equal(2, allTasksIgnoringFilters.Count);
            }
        }

    }
}
