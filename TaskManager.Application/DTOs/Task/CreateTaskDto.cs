
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Task;

public class CreateTaskDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public int? CategoryId { get; set; }
}