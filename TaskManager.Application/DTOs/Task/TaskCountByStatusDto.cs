using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Task
{
    public class TaskCountByStatusDto
    {
        public int Completed { get; set; }
        public int InProgress { get; set; }
        public int NotStarted { get; set; }
        public int OnHold { get; set; }
    }
}