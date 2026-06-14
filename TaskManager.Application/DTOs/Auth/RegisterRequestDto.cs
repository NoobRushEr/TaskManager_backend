using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required List<Role_> Roles { get; set; }
    }
}