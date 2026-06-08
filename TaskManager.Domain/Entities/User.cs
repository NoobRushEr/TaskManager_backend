using System.ComponentModel.DataAnnotations;
using TaskManager.Domain.Enums;
namespace TaskManager.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required List<Role_> Role { get; set; }

    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}