using Identity.Core.Entities;
using Identity.Core.Repositories;

namespace Identity.Core.Services
{
    public interface IUserService : IRepository<User>
    {
    }
}
