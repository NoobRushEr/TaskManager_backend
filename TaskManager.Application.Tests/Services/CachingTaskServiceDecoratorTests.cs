using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using TaskManager.Application.DTOs.Task;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Enums;
using Xunit;

namespace TaskManager.Application.Tests.Services
{
    public class CachingTaskServiceDecoratorTests : IDisposable
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<ITaskService> _mockInnerService;
        private readonly IMemoryCache _cache;
        private readonly CachingTaskServiceDecorator _decorator;

        public CachingTaskServiceDecoratorTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _mockInnerService = _mockRepository.Create<ITaskService>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _decorator = new CachingTaskServiceDecorator(_mockInnerService.Object, _cache);
        }

        public void Dispose()
        {
            _cache.Dispose();
        }

        [Fact]
        public async Task GetTaskByIdAsync_CacheMiss_CallsInnerServiceAndCachesResult()
        {
            // Arrange
            int taskId = 1;
            var expectedTask = new TaskResponseDto { TaskId = taskId, Title = "Test Task" };
            
            _mockInnerService.Setup(s => s.GetTaskByIdAsync(taskId))
                .ReturnsAsync(expectedTask);

            // Act
            var result1 = await _decorator.GetTaskByIdAsync(taskId);
            var result2 = await _decorator.GetTaskByIdAsync(taskId); // Second call should hit the cache

            // Assert
            Assert.NotNull(result1);
            Assert.Equal(taskId, result1.TaskId);
            Assert.Same(result1, result2); // Verify same cached instance is returned

            _mockInnerService.Verify(s => s.GetTaskByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_Success_EvictsCacheAndCallsInnerService()
        {
            // Arrange
            int taskId = 1;
            int userId = 100;
            var updateDto = new UpdateTaskDto { Title = "Updated Task" };
            var updatedTask = new TaskResponseDto { TaskId = taskId, Title = "Updated Task" };

            // Prime the cache
            string cacheKey = $"tasks:id:{taskId}";
            _cache.Set(cacheKey, new TaskResponseDto { TaskId = taskId, Title = "Old Task" });

            _mockInnerService.Setup(s => s.UpdateTaskAsync(taskId, updateDto, userId, false))
                .ReturnsAsync(updatedTask);

            // Act
            var result = await _decorator.UpdateTaskAsync(taskId, updateDto, userId, false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Task", result.Title);
            
            // Verify cache eviction
            Assert.False(_cache.TryGetValue(cacheKey, out _));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetTaskByIdAsync_InnerServiceThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            int taskId = 1;
            var mockHttpException = new HttpRequestException("HTTP Error calling internal service");

            _mockInnerService.Setup(s => s.GetTaskByIdAsync(taskId))
                .ThrowsAsync(mockHttpException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _decorator.GetTaskByIdAsync(taskId));
            Assert.Equal("HTTP Error calling internal service", exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetTaskByIdAsync_InnerServiceThrowsDbException_PropagatesException()
        {
            // Arrange
            int taskId = 1;
            // Simulating a DB concurrency or connection exception
            var mockDbException = new InvalidOperationException("Database connection timed out");

            _mockInnerService.Setup(s => s.GetTaskByIdAsync(taskId))
                .ThrowsAsync(mockDbException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _decorator.GetTaskByIdAsync(taskId));
            Assert.Equal("Database connection timed out", exception.Message);
            _mockRepository.VerifyAll();
        }
    }
}