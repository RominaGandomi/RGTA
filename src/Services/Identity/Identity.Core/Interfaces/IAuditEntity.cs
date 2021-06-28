using System;

namespace Identity.Core.Interfaces
{
    public interface IAuditEntity : IBaseEntity
    {
        string CreatedBy { get; set; }
        DateTime? CreatedOn { get; set; }
        string UpdatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
        string DeletedBy { get; set; }
        DateTime? DeletedOn { get; set; }
    }
}
