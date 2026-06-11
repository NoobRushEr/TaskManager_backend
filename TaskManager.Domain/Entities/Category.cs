using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Domain.Entities;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    public required string CategoryName { get; set; }

    public int? UserId { get; set; } = null;

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

}