using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.DTOs.Category;

public class UpdateCategoryDto
{
    [Required]
    [MaxLength(50)]
    public required string CategoryName { get; set; }
}
