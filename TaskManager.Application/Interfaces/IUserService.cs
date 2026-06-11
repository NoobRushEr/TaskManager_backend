using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.User;
using TaskManager.Application.DTOs.Category;
using TaskManager.Application.DTOs.Task;

namespace TaskManager.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task DeleteUserAsync(int id);
        Task<IEnumerable<CategoryResponseDto>> GetCategoriesByUserIdAsync(int userId);

        Task<UserTaskCategoryResponseDto?> GetTaskByUserIdAndTaskAsync(int userId, int taskId);
    }
}