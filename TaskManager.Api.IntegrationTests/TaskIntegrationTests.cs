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
    }
}
