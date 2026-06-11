using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Task;

public class TaskResponseDto
{
    public int TaskId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public int? CategoryId { get; set; }
    public int UserId { get; set; }
}