using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Identity.Core.Entities
{
    public class Role:IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
