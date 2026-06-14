using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using BCrypt.Net;


namespace TaskManager.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IServiceProvider _serviceProvider;

        public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, IServiceProvider serviceProvider)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _serviceProvider = serviceProvider;
        }

        public async Task<GenericResponseDto<LoginResponseDto>> AuthenticateUserAsync(string email, string password)
        {
            var _loginValidator = _serviceProvider.GetService(typeof(IValidator<LoginRequestDto>)) as IValidator<LoginRequestDto>;
            var validationResult = await _loginValidator!.ValidateAsync(new LoginRequestDto { Email = email, Password = password });
            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid login request.";
                return new GenericResponseDto<LoginResponseDto> { IsSuccess = false, Message = firstError };
            }

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return new GenericResponseDto<LoginResponseDto> { IsSuccess = false, Message = "Invalid email or password." };
            }

            var token = _jwtTokenGenerator.GenerateToken(user.FirstName, user.Email, user.Roles);
            return new GenericResponseDto<LoginResponseDto> { IsSuccess = true, Message = "Login successful.", Data = new LoginResponseDto { Token = token, Expiration = DateTime.UtcNow.AddHours(1) } };
        }

        public async Task<GenericResponseDto> RegisterUserAsync(string firstName, string? lastName, string email, string password, List<Role_> roles)
        {
            var _registerValidator = _serviceProvider.GetService(typeof(IValidator<RegisterRequestDto>)) as IValidator<RegisterRequestDto>;
            var validationResult = await _registerValidator!.ValidateAsync(new RegisterRequestDto
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                Roles = roles
            });

            if (!validationResult.IsValid)
            {
                return new GenericResponseDto { IsSuccess = false, Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)) };
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                return new GenericResponseDto { IsSuccess = false, Message = "Email already exists." };
            }
            
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash =  passwordHash,
                Roles = new List<Role_>(roles)
            };

            await _userRepository.AddAsync(newUser);
            return new GenericResponseDto { IsSuccess = true, Message = "User registered successfully." };
        }
    }
}