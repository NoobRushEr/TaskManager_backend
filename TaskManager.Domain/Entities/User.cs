using System.ComponentModel.DataAnnotations;
using TaskManager.Domain.Enums;
namespace TaskManager.Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required List<Role_> Roles { get; set; } = new List<Role_>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}