using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Domain.Entities;
using AboutMeApp.Persistence.Contexts;

namespace AboutMeApp.Persistence.Implementations.Repositories;

public class TemplateRepository : Repository<Template>, ITemplateRepository
{
    public TemplateRepository(AboutMeAppDbContext context) : base(context)
    {
    }
}
