using AlarmTracking.Application.Common;
using AlarmTracking.Application.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<ProductionOrder> ProductionOrders { get; set; }
        public DbSet<ProductionSchedule> ProductionSchedules { get; set; }
        public DbSet<BanburyMachine> BanburyMachines { get; set; }
        public DbSet<MaterialReservation> MaterialReservations { get; set; }
        public DbSet<WorkInstruction> WorkInstructions { get; set; }
        public DbSet<ShopfloorTask> ShopfloorTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductionOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Ignore(e => e.DomainEvents);
            });

            modelBuilder.Entity<ProductionSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasConversion<string>();
            });

            // COMMENT OUT THIS ENTIRE SECTION TEMPORARILY
            /*
            modelBuilder.Entity<BanburyMachine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasConversion<string>();
                entity.OwnsOne(e => e.Capabilities, capabilities =>
                {
                    // ... capabilities configuration
                });
            });
            */

            // Add simple BanburyMachine configuration without Capabilities
            modelBuilder.Entity<BanburyMachine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Ignore(e => e.Capabilities); // Ignore the problematic property
            });

            modelBuilder.Entity<MaterialReservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasConversion<string>();
            });

            modelBuilder.Entity<WorkInstruction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Ignore(e => e.Parameters);
                entity.Ignore(e => e.QualityChecks);
            });

            modelBuilder.Entity<ShopfloorTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TaskType).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
