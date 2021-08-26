using System;

namespace TwitterAPIDemo.Core.Models
{
    public interface IAuditEntity
    {
        DateTime CreatedDate { get; set; }
        DateTime? UpdatedDate { get; set; }
    }
}
