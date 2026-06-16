using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Task
{
    public class TaskCountByPriorityDto
    {
        public int High{ get; set; }
        public int Medium { get; set; }
        public int Low { get; set; }
    }
}