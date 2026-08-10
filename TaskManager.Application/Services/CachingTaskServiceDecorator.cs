using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using TaskManager.Application.DTOs.Task;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Services
{
    /// <summary>
    /// Decorator that implements Cache-Aside pattern for Task service read queries
    /// and handles cache invalidation on write/update operations.
    /// </summary>
    public class CachingTaskServiceDecorator : ITaskService
    {
        private readonly ITaskService _innerTaskService;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private const string CacheKeyPrefix = "tasks";

        public CachingTaskServiceDecorator(ITaskService taskService, IMemoryCache cache)
        {
            _innerTaskService = taskService;
            _cache = cache;
        }

        // ==========================================
        // Cached Read Operations (Cache-Aside)
        // ==========================================

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int id)
        {
            string cacheKey = $"{CacheKeyPrefix}:id:{id}";
            if (!_cache.TryGetValue(cacheKey, out TaskResponseDto? task))
            {
                // Cache miss: delegate to DB retrieve and populate cache
                task = await _innerTaskService.GetTaskByIdAsync(id);
                if (task != null)
                {
                    _cache.Set(cacheKey, task, CacheDuration);
                }
            }
            return task;
        }

        public async Task<IEnumerable<TaskResponseDto>> GetMyTasksAsync(int userId, bool includeDeleted = false, bool includeArchived = false)
        {
            string cacheKey = $"{CacheKeyPrefix}:user:{userId}:deleted:{includeDeleted}:archived:{includeArchived}";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<TaskResponseDto>? tasks))
            {
                // Cache miss: delegate list retrieval and cache
                tasks = await _innerTaskService.GetMyTasksAsync(userId, includeDeleted, includeArchived);
                _cache.Set(cacheKey, tasks, CacheDuration);
            }
            return tasks!;
        }

        // ==========================================
        // Direct Delegation (Reads without caching)
        // ==========================================

        public Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync()
        {
            return _innerTaskService.GetAllTasksAsync();
        }

        public Task<IEnumerable<TaskResponseDto>> GetTasksPaginatedAsync(int page, int pageSize)
        {
            return _innerTaskService.GetTasksPaginatedAsync(page, pageSize);
        }

        public Task<IEnumerable<TaskResponseDto>> GetTasksByUserIdAsync(int userId)
        {
            return _innerTaskService.GetTasksByUserIdAsync(userId);
        }

        public Task<IEnumerable<TaskResponseDto>> GetTasksByStatusAsync(Status_ status)
        {
            return _innerTaskService.GetTasksByStatusAsync(status);
        }

        // ==========================================
        // Write Operations (Eviction / Invalidation)
        // ==========================================

        public async Task<TaskResponseDto> CreateTaskAsync(int userId, CreateTaskDto createTaskDto)
        {
            var result = await _innerTaskService.CreateTaskAsync(userId, createTaskDto);

            // Invalidate query cache for user's task lists
            InvalidateUserCache(userId);

            return result;
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId, bool isAdmin)
        {
            var result = await _innerTaskService.UpdateTaskAsync(id, updateTaskDto, userId, isAdmin);
            if (result != null)
            {
                // Evict the single cached task and associated list caches
                _cache.Remove($"{CacheKeyPrefix}:id:{id}");
                InvalidateUserCache(userId);
            }
            return result;
        }

        public async Task DeleteTaskAsync(int id, int userId, bool isAdmin)
        {
            await _innerTaskService.DeleteTaskAsync(id, userId, isAdmin);

            // Invalidate cached instances
            _cache.Remove($"{CacheKeyPrefix}:id:{id}");
            InvalidateUserCache(userId);
        }

        public async Task<IEnumerable<TaskResponseDto>> UpdateTaskStatusAsync(int taskId, Status_ newStatus, int userId, bool isAdmin)
        {
            var result = await _innerTaskService.UpdateTaskStatusAsync(taskId, newStatus, userId, isAdmin);

            // Invalidate cached instances
            _cache.Remove($"{CacheKeyPrefix}:id:{taskId}");
            InvalidateUserCache(userId);

            return result;
        }

        public async Task SoftDeleteTaskAsync(int taskId, int userId, bool isAdmin)
        {
            await _innerTaskService.SoftDeleteTaskAsync(taskId, userId, isAdmin);

            // Invalidate cached instances
            _cache.Remove($"{CacheKeyPrefix}:id:{taskId}");
            InvalidateUserCache(userId);
        }

        public async Task RestoreTaskAsync(int taskId, bool isAdmin)
        {
            await _innerTaskService.RestoreTaskAsync(taskId, isAdmin);

            // Evict single task cache
            _cache.Remove($"{CacheKeyPrefix}:id:{taskId}");
        }

        // ==========================================
        // Dashboard Queries (Available for caching implementation)
        // ==========================================

        public Task<TaskStatisticsDto> GetTaskStatisticsAsync(int userId)
        {
            if (!_cache.TryGetValue($"{CacheKeyPrefix}:user:{userId}:statistics", out TaskStatisticsDto? statistics))
            {
                statistics = _innerTaskService.GetTaskStatisticsAsync(userId).Result;
                _cache.Set($"{CacheKeyPrefix}:user:{userId}:statistics", statistics, CacheDuration);
            }
            return Task.FromResult(statistics!);
        }

        public Task<TaskCountByStatusDto> GetTasksCountAsync(int userId)
        {
            if (!_cache.TryGetValue($"{CacheKeyPrefix}:user:{userId}:count", out TaskCountByStatusDto? count))
            {
                count = _innerTaskService.GetTasksCountAsync(userId).Result;
                _cache.Set($"{CacheKeyPrefix}:user:{userId}:count", count, CacheDuration);
            }
            return Task.FromResult(count!);
        }

        public Task<IEnumerable<TaskCountByCategoryDto>> GetTasksCountByCategoryAsync(int userId)
        {
            if (!_cache.TryGetValue($"{CacheKeyPrefix}:user:{userId}:count-by-category", out IEnumerable<TaskCountByCategoryDto>? countByCategory))
            {
                countByCategory = _innerTaskService.GetTasksCountByCategoryAsync(userId).Result;
                _cache.Set($"{CacheKeyPrefix}:user:{userId}:count-by-category", countByCategory, CacheDuration);
            }
            return Task.FromResult(countByCategory!);
        }


        public async Task PurgeSoftDeletedTasksAsync(CancellationToken cancellationToken = default)
        {
            await _innerTaskService.PurgeSoftDeletedTasksAsync(cancellationToken);
        }

        public async Task ArchiveCompletedTasksAsync(CancellationToken cancellationToken = default)
        {
            await _innerTaskService.ArchiveCompletedTasksAsync(cancellationToken);
        }

        private void InvalidateUserCache(int userId)
        {
            _cache.Remove($"{CacheKeyPrefix}:user:{userId}:deleted:true:archived:true");
            _cache.Remove($"{CacheKeyPrefix}:user:{userId}:deleted:true:archived:false");
            _cache.Remove($"{CacheKeyPrefix}:user:{userId}:deleted:false:archived:true");
            _cache.Remove($"{CacheKeyPrefix}:user:{userId}:deleted:false:archived:false");
        }
    }
}