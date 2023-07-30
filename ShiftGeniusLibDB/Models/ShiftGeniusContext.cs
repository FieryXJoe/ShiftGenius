using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ShiftGeniusLibDB.Models
{
    public partial class ShiftGeniusContext : DbContext
    {
        public ShiftGeniusContext()
        {
        }

        public ShiftGeniusContext(DbContextOptions<ShiftGeniusContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; } = null!;
        public virtual DbSet<EmployeeScheduled> EmployeeScheduleds { get; set; } = null!;
        public virtual DbSet<Organization> Organizations { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<ScheduleDay> ScheduleDays { get; set; } = null!;
        public virtual DbSet<ScheduleRule> ScheduleRules { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=shiftgeniusmsft.cgkhjaoqz22z.us-east-1.rds.amazonaws.com,1433;Database=ShiftGenius;User Id=admin;Password=Cc4JsFvC3xN6lPctMfYw;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.Email)
                    .HasMaxLength(320)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.Password)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_Employee_Organization");
            });

            modelBuilder.Entity<EmployeeRole>(entity =>
            {
                entity.ToTable("EmployeeRole");

                entity.Property(e => e.EmployeeRoleId).HasColumnName("EmployeeRoleID");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeRoles)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Role_Employee");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.EmployeeRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Role_Role");
            });

            modelBuilder.Entity<EmployeeScheduled>(entity =>
            {
                entity.ToTable("EmployeeScheduled");

                entity.Property(e => e.EmployeeScheduledId).HasColumnName("EmployeeScheduledID");

                entity.Property(e => e.EmployeeRoleId).HasColumnName("EmployeeRoleID");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.ScheduleDayId).HasColumnName("ScheduleDayID");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.EmployeeRole)
                    .WithMany(p => p.EmployeeScheduleds)
                    .HasForeignKey(d => d.EmployeeRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Scheduled_Employee");

                entity.HasOne(d => d.ScheduleDay)
                    .WithMany(p => p.EmployeeScheduleds)
                    .HasForeignKey(d => d.ScheduleDayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Scheduled_Day");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.SubscriptionEnd).HasColumnType("datetime");

                entity.HasOne(d => d.OwnerNavigation)
                    .WithMany(p => p.Organizations)
                    .HasForeignKey(d => d.Owner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organization_Owner");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("RoleID");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            });

            modelBuilder.Entity<ScheduleDay>(entity =>
            {
                entity.ToTable("ScheduleDay");

                entity.Property(e => e.ScheduleDayId).HasColumnName("ScheduleDayID");

                entity.Property(e => e.Day).HasColumnType("datetime");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ScheduleDays)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Schedule_Day_Organization");
            });

            modelBuilder.Entity<ScheduleRule>(entity =>
            {
                entity.ToTable("ScheduleRule");

                entity.Property(e => e.ScheduleRuleId).HasColumnName("ScheduleRuleID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ScheduleRules)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Schedule_Rule_CreatedBy");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.ScheduleRules)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_Schedule_Rule_Employee");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ScheduleRules)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Schedule_Rule_Organization");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
