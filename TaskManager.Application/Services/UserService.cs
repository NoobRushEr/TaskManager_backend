using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Category;
using TaskManager.Application.DTOs.Task;
using TaskManager.Application.DTOs.User;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                return Enumerable.Empty<UserResponseDto>();
            }
            return users.Select(user => new UserResponseDto
            {
                Id = user!.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            });
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (createUserDto == null) throw new ArgumentNullException(nameof(createUserDto));

            var user = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email
            };
            await _userRepository.AddAsync(user);
            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null) throw new ArgumentNullException(nameof(updateUserDto));

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) throw new Exception("User not found");

            existingUser.FirstName = updateUserDto.FirstName ?? existingUser.FirstName;
            existingUser.LastName = updateUserDto.LastName ?? existingUser.LastName;
            existingUser.Email = updateUserDto.Email ?? existingUser.Email;

            await _userRepository.UpdateAsync(existingUser);
            return new UserResponseDto
            {
                Id = existingUser.Id,
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                Email = existingUser.Email
            };
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetCategoriesByUserIdAsync(int userId)
        {
            var users = await _userRepository.GetCategoriesByUserAsync(userId);
            if (users == null || !users.Any())
            {
                return Enumerable.Empty<CategoryResponseDto>();
            }
            return users.SelectMany(user => user!.Categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                UserId = user.Id
            }));
        }


        public async Task<UserTaskCategoryResponseDto?> GetTaskByUserIdAndTaskAsync(int userId, int taskId)
        {
            var user = await _userRepository.GetTaskByUserIdAndTaskIdAsync(userId, taskId);

            if (user == null) return null;

            // ✅ Unpack task from nested categories
            var category = user.Categories.FirstOrDefault();
            if (category == null) return null;

            var task = category.Tasks.FirstOrDefault();
            if (task == null) return null;

            return new UserTaskCategoryResponseDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                TaskTitle = task.Title,
                TaskDescription = task.Description,
                TaskDueDate = task.DueDate,
                TaskCreatedAt = task.CreatedAt,
                CategoryName = category.CategoryName
            };
        }
    }
}