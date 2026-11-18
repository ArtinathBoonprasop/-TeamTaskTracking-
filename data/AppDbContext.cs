using Microsoft.EntityFrameworkCore;
using TeamTaskTracking.Web.Models;

namespace TeamTaskTracking.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasIndex(x => new { x.TaskID, x.UserID })
                .IsUnique()
                .HasDatabaseName("UQ_Task_User");

            entity.HasOne<TaskItem>()
                .WithMany()
                .HasForeignKey(x => x.TaskID)
                .HasConstraintName("FK_Assignments_Tasks");

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserID)
                .HasConstraintName("FK_Assignments_Users");
        });
    }
}