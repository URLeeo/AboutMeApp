using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Domain.Entities;
using AboutMeApp.Persistence.Contexts;

namespace AboutMeApp.Persistence.Implementations.Repositories;

public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(AboutMeAppDbContext context) : base(context)
    {
    }
}
