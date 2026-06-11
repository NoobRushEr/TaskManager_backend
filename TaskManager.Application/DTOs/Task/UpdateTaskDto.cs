using TaskManager.Domain.Enums;
namespace TaskManager.Application.DTOs.Task;

public class UpdateTaskDto
{
    public int TaskId { get; set; }
    public string? Title { get; set; } 
    public string? Description { get; set; }
    public Priority_? Priority { get; set; }
    public Status_? Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public int? CategoryId { get; set; }
}