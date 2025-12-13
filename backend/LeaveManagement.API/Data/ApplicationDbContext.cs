using LeaveManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LeaveManagement.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - Each represents a table in the database
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId); // Primary Key
                entity.Property(e => e.EmployeeId).HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique(); // Email must be unique
                entity.Property(e => e.Department).HasMaxLength(50);
                entity.Property(e => e.Role).HasMaxLength(50);
                entity.Property(e => e.ManagerId).HasMaxLength(50);
            });

            // Configure LeaveRequest
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EmployeeId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LeaveType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);

                // Foreign Key relationship
                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.LeaveRequests)
                      .HasForeignKey(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure LeaveBalance
            modelBuilder.Entity<LeaveBalance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EmployeeId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LeaveType).IsRequired().HasMaxLength(50);

                // Foreign Key relationship
                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.LeaveBalances)
                      .HasForeignKey(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint: One balance per employee per leave type
                entity.HasIndex(e => new { e.EmployeeId, e.LeaveType }).IsUnique();
            });

            // Configure Holiday
            modelBuilder.Entity<Holiday>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Date); // Index for faster date queries
            });

            // Configure Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EmployeeId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status).HasMaxLength(20);

                // Foreign Key relationship
                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.Notifications)
                      .HasForeignKey(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}