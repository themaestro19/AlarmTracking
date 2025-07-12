using AlarmTracking.Application.Contracts.Infrastructure;
using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Repositories;
using AlarmTracking.Application.Services;
using AlarmTracking.DataAccess.Data.Context;
using AlarmTracking.DataAccess.Repositories;
using AlarmTracking.DataAccess.Repositories.Dapper;
using AlarmTracking.DataAccess.Repositories.EntityFramework;
using AlarmTracking.DataAccess.Repositories.Stubs;
using AlarmTracking.DataAccess.Services;
using AlarmTracking.Domain.Repositories;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlarmTracking.DataAccess.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Entity Framework with SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // User repositories
            services.AddScoped<EfUserRepository>();
            services.AddScoped<DapperUserRepository>();
            services.AddScoped<HybridUserRepository>();
            services.AddScoped<IUserRepository>(provider => provider.GetRequiredService<HybridUserRepository>());

            // Refresh Token repositories
            services.AddScoped<EfRefreshTokenRepository>();
            services.AddScoped<DapperRefreshTokenRepository>();
            services.AddScoped<HybridRefreshTokenRepository>();
            services.AddScoped<IRefreshTokenRepository>(provider => provider.GetRequiredService<HybridRefreshTokenRepository>());

            // Production entity repositories (using existing ones you have)
            services.AddScoped<IProductionOrderRepository, DapperProductionOrderRepository>();
            services.AddScoped<IProductionScheduleRepository, DapperProductionScheduleRepository>();
            services.AddScoped<IBanburyMachineRepository, DapperBanburyMachineRepository>();
            services.AddScoped<IMaterialReservationRepository, DapperMaterialReservationRepository>();

            // Use stubs for missing repositories
            services.AddScoped<IProductSpecificationRepository, StubProductSpecificationRepository>();
            services.AddScoped<IMaterialRepository, StubMaterialRepository>();
            services.AddScoped<IBillOfMaterialRepository, StubBillOfMaterialRepository>();
            services.AddScoped<IOperatorRepository, StubOperatorRepository>();

            // Infrastructure services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}