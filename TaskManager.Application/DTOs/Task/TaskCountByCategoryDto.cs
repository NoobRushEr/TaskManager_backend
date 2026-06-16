using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Task
{
    public class TaskCountByCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}