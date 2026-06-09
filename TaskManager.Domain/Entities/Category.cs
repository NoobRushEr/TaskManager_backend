using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Domain.Entities;

public class Category
{
    [Key]
    public int Category_Id { get; set; }

    [Required]
    public required string CategoryName { get; set; }

    public int? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public ICollection<Task>? Tasks { get; set; }

}