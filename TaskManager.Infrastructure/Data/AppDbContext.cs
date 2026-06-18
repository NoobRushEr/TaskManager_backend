using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;


namespace TaskManager.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Category> Categories { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>().HasQueryFilter(t => !t.IsDeleted);

        // ======================== Table & Column Names ========================

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");
            entity.HasKey(t => t.Task_Id);
            entity.Property(t => t.Task_Id).HasColumnName("task_id");
            entity.Property(t => t.Title).HasColumnName("title").IsRequired().HasMaxLength(100);
            entity.Property(t => t.Description).HasColumnName("description").HasMaxLength(250);
            entity.Property(t => t.CompletedAt).HasColumnName("completed_at");
            entity.Property(t => t.DueDate).HasColumnName("due_date");
            entity.Property(t => t.Priority).HasColumnName("priority").HasConversion<string>();
            entity.Property(t => t.Status).HasColumnName("status").HasConversion<string>();
            entity.Property(t => t.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            entity.Property(t => t.DeletedAt).HasColumnName("deleted_at");

            // Foreign keys
            entity.Property(t => t.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(t => t.CategoryId).HasColumnName("category_id").IsRequired(false);

            entity.Property(t => t.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("user_id");
            entity.Property(u => u.FirstName).HasColumnName("first_name").IsRequired().HasMaxLength(50);
            entity.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(50);
            entity.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        var rolesComparer = new ValueComparer<List<Role_>>(
            (c1, c2) => c1!.SequenceEqual(c2!), // Compares the actual elements in the lists to check if they match
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Generates a clean hash code based on elements
            c => c.ToList() // Creates a shallow copy snapshot for tracking comparison
        );

        modelBuilder.Entity<User>()
            .Property(u => u.Roles)
            .HasConversion(
                v => string.Join(',', v.Select(r => r.ToString())), // Convert List<Role_> to comma-separated string
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => Enum.Parse<Role_>(r)).ToList() // Convert comma-separated string back to List<Role_>
            )
            .Metadata.SetValueComparer(rolesComparer);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(c => c.CategoryId);
            entity.Property(c => c.CategoryId).HasColumnName("category_id");
            entity.Property(c => c.CategoryName).HasColumnName("category_name").IsRequired().HasMaxLength(50);

            // Foreign key
            entity.Property(c => c.UserId).HasColumnName("user_id").IsRequired(false);
        });


        // ======================== Relationships ========================

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.User)
            .WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}