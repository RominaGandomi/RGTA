using Identity.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Core.Entities
{
    public class User : IdentityUser<int>, IAuditEntity, IMultiTenant
    {
        public User()
        {
            Guid = System.Guid.Empty.ToString();
        }
        public string Guid { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string KnownAs { get; set; }
        public string Title { get; set; }

        public string NationalIdentificationNumber { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime LastEnterance { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantId { get; set; }
    }
}
