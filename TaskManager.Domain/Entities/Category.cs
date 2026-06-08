using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Domain.Entities;

public class Category
{
    [Key]
    public Guid Category_Id { get; set; }

    [Required]
    public required string CategoryName { get; set; }

    public Guid? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public ICollection<Task>? Tasks { get; set; }

}