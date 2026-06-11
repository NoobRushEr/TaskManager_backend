
namespace TaskManager.Application.DTOs.Task;

public class UpdateTaskDto
{
    public int Id { get; set; }
    public string? Title { get; set; } 
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int? CategoryId { get; set; }
}