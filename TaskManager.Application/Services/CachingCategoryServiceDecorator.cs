using TaskManager.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using TaskManager.Application.DTOs.Category;
using TaskManager.Domain.Entities;
using TaskManager.Application.DTOs.Task;

namespace TaskManager.Application.Services
{
    public class CachingCategoryServiceDecorator : ICategoryService
    {
        private readonly ICategoryService _innercategoryService;
        private readonly IMemoryCache _cache;
        private static TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);
        private const string _cacheKey = "categories";


        public CachingCategoryServiceDecorator(ICategoryService categoryService, IMemoryCache cacheService)
        {
            _innercategoryService = categoryService;
            _cache = cacheService;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            if (!_cache.TryGetValue(_cacheKey, out IEnumerable<CategoryResponseDto>? categories))
            {
                categories = await _innercategoryService.GetAllCategoriesAsync();
                _cache.Set(_cacheKey, categories, _cacheDuration);
            }

            return categories!;
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            string cacheKey = $"{_cacheKey}:{id}";
            if (!_cache.TryGetValue(cacheKey, out CategoryResponseDto? category))
            {
                category = await _innercategoryService.GetCategoryByIdAsync(id);
                if (category != null)
                {
                    _cache.Set(cacheKey, category, _cacheDuration);
                }
            }
            return category!;
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null) throw new ArgumentNullException(nameof(createCategoryDto));

            // Delegate creation to inner service to get the generated CategoryId
            var result = await _innercategoryService.CreateCategoryAsync(createCategoryDto);

            // Invalidate the cache for all categories
            _cache.Remove(_cacheKey); 
            
            return result;
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            if (updateCategoryDto == null) throw new ArgumentNullException(nameof(updateCategoryDto));

            await _innercategoryService.UpdateCategoryAsync(id, updateCategoryDto);
            _cache.Remove($"{_cacheKey}:{id}"); // Invalidate the cache for the specific category
            _cache.Remove(_cacheKey); // Invalidate the cache for all categories
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _innercategoryService.DeleteCategoryAsync(id);
            _cache.Remove($"{_cacheKey}:{id}"); // Invalidate the cache for the specific category
            _cache.Remove(_cacheKey); // Invalidate the cache for all categories
        }

        public async Task<IEnumerable<TaskResponseDto>> GetTasksByCategoryIdAsync(int categoryId)
        {
            return await _innercategoryService.GetTasksByCategoryIdAsync(categoryId);
        }

    }
}