using Identity.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Core.Entities
{
    public abstract class BaseEntity : IBaseEntity
    {
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
