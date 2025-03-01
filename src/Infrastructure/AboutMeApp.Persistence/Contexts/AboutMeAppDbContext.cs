using AboutMeApp.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AboutMeApp.Persistence.Contexts;

public class AboutMeAppDbContext : IdentityDbContext<User, Role, Guid>
{
    public AboutMeAppDbContext(DbContextOptions<AboutMeAppDbContext> options) : base(options)
    {
    }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<SocialMedia> SocialMedias { get; set; }
    public DbSet<Template> Templates { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
}
