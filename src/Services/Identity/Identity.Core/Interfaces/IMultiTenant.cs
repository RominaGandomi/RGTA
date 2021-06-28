using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Interfaces
{
    public interface IMultiTenant
    {
        [Required]
        [Range(1, 5)]
        int TenantId { get; set; }
    }
}
