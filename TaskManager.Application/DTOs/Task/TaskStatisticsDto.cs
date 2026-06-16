using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Task
{
    public class TaskStatisticsDto
    {
        public int TotalTasks { get; set; }
        public TaskCountByStatusDto TaskCountByStatus { get; set; } = new TaskCountByStatusDto();        
        public TaskCountByPriorityDto TaskCountByPriority { get; set; } = new TaskCountByPriorityDto(); 
        public List<TaskCountByCategoryDto> TaskCountByCategory { get; set; } = new List<TaskCountByCategoryDto>();
    }
}