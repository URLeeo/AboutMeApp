using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Domain.Entities;
using AboutMeApp.Persistence.Contexts;

namespace AboutMeApp.Persistence.Implementations.Repositories;

public class ExperienceRepository : Repository<Experience>, IExperienceRepository
{
    public ExperienceRepository(AboutMeAppDbContext context) : base(context)
    {
    }
}
