using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Task
{
    public class UserTaskCategoryResponseDto
    {
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public required string Email { get; set; }
        public required string TaskTitle { get; set; }
        public string? TaskDescription { get; set; }
        public DateTime? TaskDueDate { get; set; }
        public required DateTime TaskCreatedAt { get; set; }
        public required string CategoryName { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchivedAt { get; set; }
    }
}