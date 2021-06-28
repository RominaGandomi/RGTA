using Identity.Core.Entities;
using Identity.Core.Services;
using Identity.Infrastructure.Data;

namespace Identity.Infrastructure.Services
{
    public class TenantService : Repository<Tenant>, ITenantService
    {
        public TenantService(IdentityDbContext context) : base(context)
        {
        }
    }
}
