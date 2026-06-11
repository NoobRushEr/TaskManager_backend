using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Category
{
    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public int? UserId { get; set; }
    }
}