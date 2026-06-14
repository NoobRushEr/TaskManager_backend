using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<GenericResponseDto<LoginResponseDto>> AuthenticateUserAsync(string email, string password);
        public Task<GenericResponseDto> RegisterUserAsync(string firstName, string? lastName, string email, string password, List<Role_> roles);
    }
}