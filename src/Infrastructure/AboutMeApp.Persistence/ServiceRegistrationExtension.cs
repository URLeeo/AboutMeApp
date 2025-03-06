using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Profiles;
using AboutMeApp.Application.Validations.Certificate;
using AboutMeApp.Domain.Entities;
using AboutMeApp.Persistence.Contexts;
using AboutMeApp.Persistence.Implementations.Repositories;
using AboutMeApp.Persistence.Implementations.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AboutMeApp.Persistence;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AboutMeAppDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Default")));
        services.AddIdentityCore<User>(opt =>
        {
            opt.Password.RequiredLength = 8;
            opt.Password.RequireNonAlphanumeric = true;
            opt.Password.RequireDigit = true;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireUppercase = true;
            opt.User.RequireUniqueEmail = true;
            opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-";
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<AboutMeAppDbContext>();

        services.AddScoped<ICertificateRepository, CertificateRepository>();
        services.AddScoped<ICertificateService, CertificateService>();

        services.AddScoped<IEducationRepository, EducationRepository>();
        services.AddScoped<IEducationService, EducationService>();

        services.AddAutoMapper(typeof(CertificateProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<CertificateUpdateValidator>();

        return services;
    }
}
