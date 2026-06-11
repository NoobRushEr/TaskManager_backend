using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetCategoriesByUserAsync(int userId);

        Task<User?> GetTaskByUserIdAndTaskIdAsync(int userId, int taskId);
    }
}