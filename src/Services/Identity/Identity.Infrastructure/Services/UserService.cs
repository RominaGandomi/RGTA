using Identity.Core.Entities;
using Identity.Core.Services;
using Identity.Infrastructure.Data;

namespace Identity.Infrastructure.Services
{
    public class UserService : Repository<User>, IUserService
    {
        public UserService(IdentityDbContext context) : base(context)
        {
        }
    }
}
