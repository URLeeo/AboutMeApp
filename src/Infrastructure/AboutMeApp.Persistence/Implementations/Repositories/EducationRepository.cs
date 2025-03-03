using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Domain.Entities;
using AboutMeApp.Persistence.Contexts;

namespace AboutMeApp.Persistence.Implementations.Repositories;

public class EducationRepository : Repository<Education>, IEducationRepository
{
    public EducationRepository(AboutMeAppDbContext context) : base(context)
    {
    }
}