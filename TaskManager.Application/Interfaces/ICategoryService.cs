using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Category;
using TaskManager.Application.DTOs.Task;

namespace TaskManager.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task DeleteCategoryAsync(int id);
        Task<IEnumerable<TaskResponseDto>> GetTasksByCategoryIdAsync(int categoryId);
    }
}