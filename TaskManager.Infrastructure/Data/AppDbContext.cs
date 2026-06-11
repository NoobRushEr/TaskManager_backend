using Microsoft.EntityFrameworkCore;
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

            // Foreign keys
            entity.Property(t => t.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(t => t.CategoryId).HasColumnName("category_id").IsRequired(false);

            // Shadow property for CreatedAt with default value
            entity.Property<DateTime>("CreatedAt").HasDefaultValueSql("NOW()");
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
            .OnDelete(DeleteBehavior.SetNull);
    }
}