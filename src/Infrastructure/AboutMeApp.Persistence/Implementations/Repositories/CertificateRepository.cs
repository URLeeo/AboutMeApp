using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Domain.Entities;
using AboutMeApp.Persistence.Contexts;

namespace AboutMeApp.Persistence.Implementations.Repositories;

public class CertificateRepository : Repository<Certificate>, ICertificateRepository
{
    public CertificateRepository(AboutMeAppDbContext context) : base(context)
    {
    }
}
