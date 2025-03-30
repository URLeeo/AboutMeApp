using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Validations.Certificate;
using AboutMeApp.Application.Validations.Education;
using AboutMeApp.Application.Validations.Experience;
using AboutMeApp.Application.Validations.SocialMedia;
using AboutMeApp.Application.Validations.Template;
using AboutMeApp.Application.Validations.UserProfile;
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

        services.AddScoped<IExperienceRepository, ExperienceRepository>();
        services.AddScoped<IExperienceService, ExperienceService>();

        services.AddScoped<ISocialMediaRepository, SocialMediaRepository>();
        services.AddScoped<ISocialMediaService, SocialMediaService>();

        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<ITemplateService, TemplateService>();

        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUserProfileService, UserProfileService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddValidatorsFromAssemblyContaining<CertificateUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<CertificateCreateValidator>();

        services.AddValidatorsFromAssemblyContaining<EducationUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<EducationCreateValidator>();

        services.AddValidatorsFromAssemblyContaining<ExperienceUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<ExperienceCreateValidator>();

        services.AddValidatorsFromAssemblyContaining<SocialMediaUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<SocialMediaCreateValidator>();

        services.AddValidatorsFromAssemblyContaining<TemplateUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<TemplateCreateValidator>();

        services.AddValidatorsFromAssemblyContaining<UserProfileUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<UserProfileCreateValidator>();

        return services;
    }
}
