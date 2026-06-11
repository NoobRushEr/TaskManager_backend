namespace TaskManager.Application.DTOs.Task;

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int UserId { get; set; }
}