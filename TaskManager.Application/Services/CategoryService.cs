using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Category;
using TaskManager.Application.DTOs.Task;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(category => new CategoryResponseDto
            {
                CategoryId = category!.CategoryId,
                CategoryName = category.CategoryName,
                UserId = category.UserId
            });
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryResponseDto
            {
                CategoryId = category!.CategoryId,
                CategoryName = category.CategoryName,
                UserId = category.UserId
            };
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null) throw new ArgumentNullException(nameof(createCategoryDto));

            var category = new Category
            {
                CategoryName = createCategoryDto.CategoryName,
                UserId = createCategoryDto.UserId
            };

            await _categoryRepository.AddAsync(category);
            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                UserId = category.UserId
            };
        }

        public async Task UpdateCategoryAsync(int id, string categoryName)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) throw new KeyNotFoundException($"Category with id {id} not found.");

            category.CategoryName = categoryName;
            await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) throw new KeyNotFoundException($"Category with id {id} not found.");

            await _categoryRepository.DeleteAsync(category.CategoryId);
        }

        public async Task<IEnumerable<TaskResponseDto>> GetTasksByCategoryIdAsync(int categoryId)
        {
            var tasks = await _categoryRepository.GetTasksByCategoryAsync(categoryId);
            return tasks.Select(task => new TaskResponseDto
            {
                TaskId = task.Task_Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                CategoryId = task.CategoryId,
                UserId = task.UserId
            });
        }
    }
}