using Identity.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Core.Entities
{
    public class Tenant : BaseEntity, IMultiTenant
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
    }
}
