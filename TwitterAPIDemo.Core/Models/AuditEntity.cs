using System;

namespace TwitterAPIDemo.Core.Models
{
    public abstract class AuditEntity : EntityBase, IAuditEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
