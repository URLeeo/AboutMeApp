using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Domain.Entities;
using AboutMeApp.Persistence.Contexts;

namespace AboutMeApp.Persistence.Implementations.Repositories;

public class SocialMediaRepository : Repository<SocialMedia>, ISocialMediaRepository
{
    public SocialMediaRepository(AboutMeAppDbContext context) : base(context)
    {
    }
}
