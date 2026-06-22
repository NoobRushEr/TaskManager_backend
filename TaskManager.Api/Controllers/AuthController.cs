using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Application.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<GenericResponseDto<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.AuthenticateUserAsync(request.Email, request.Password);
            if (response == null)
            {
                return Unauthorized();
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<GenericResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterUserAsync(request.FirstName, request.LastName, request.Email, request.Password);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}