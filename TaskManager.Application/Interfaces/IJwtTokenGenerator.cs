using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(int userId, string firstName, string email, List<Role_> roles);
    }
}