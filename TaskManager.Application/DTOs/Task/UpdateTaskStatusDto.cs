using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Task
{
    public class UpdateTaskStatusDto
    {
        public int task_id { get; set; }
        public Status_ Status { get; set; }
    }
}