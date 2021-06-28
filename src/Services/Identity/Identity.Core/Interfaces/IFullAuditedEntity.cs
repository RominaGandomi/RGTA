using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Core.Interfaces
{
    public interface IFullAuditedEntity : IAuditEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        int VersionId { get; set; }

    }
}
