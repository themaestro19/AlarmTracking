using AlarmTracking.DataAccess.Data.Context;
using AlarmTracking.DataAccess.Services;
using AlarmTracking.Application.Entities;
using AlarmTracking.Application.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Check if we already have users
                if (await context.Users.AnyAsync())
                {
                    logger.LogInformation("Database already seeded");
                    return;
                }

                logger.LogInformation("Seeding database...");

                var passwordHasher = new PasswordHasher();

                // Create SuperAdmin user
                var superAdmin = User.Create(
                    username: "superadmin",
                    email: "superadmin@alarmtracking.com",
                    passwordHash: passwordHasher.Hash("SuperAdmin123!"),
                    firstName: "Super",
                    lastName: "Admin",
                    department: "System",
                    role: UserRole.SuperAdmin
                );

                // Create Admin user
                var admin = User.Create(
                    username: "admin",
                    email: "admin@alarmtracking.com",
                    passwordHash: passwordHasher.Hash("Admin123!"),
                    firstName: "Admin",
                    lastName: "User",
                    department: "Administration",
                    role: UserRole.Admin
                );

                // Create regular user
                var user = User.Create(
                    username: "testuser",
                    email: "user@alarmtracking.com",
                    passwordHash: passwordHasher.Hash("User123!"),
                    firstName: "Test",
                    lastName: "User",
                    department: "IT",
                    role: UserRole.User
                );

                context.Users.AddRange(superAdmin, admin, user);
                await context.SaveChangesAsync();

                logger.LogInformation("Database seeded successfully");
                logger.LogInformation("Default users created:");
                logger.LogInformation("SuperAdmin: superadmin@alarmtracking.com / SuperAdmin123!");
                logger.LogInformation("Admin: admin@alarmtracking.com / Admin123!");
                logger.LogInformation("User: user@alarmtracking.com / User123!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
}
