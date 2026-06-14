using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public required string Token { get; set; }
        public required DateTime Expiration { get; set; }
    }
}