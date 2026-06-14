using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Category;
using TaskManager.Application.DTOs.Task;
using TaskManager.Application.DTOs.User;
using TaskManager.Application.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }


        [HttpGet("{id}/categories")]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategoriesByUserId(int id)
        {
            var categories = await _userService.GetCategoriesByUserIdAsync(id);
            return Ok(categories);
        }

        [HttpGet("{user_id}/task/{taskId}")]
        public async Task<ActionResult<UserTaskCategoryResponseDto>> GetTaskByUserIdAndTaskId(int user_id, int taskId)
        {
            var task = await _userService.GetTaskByUserIdAndTaskAsync(user_id, taskId);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }
    }

}