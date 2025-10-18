using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operon.Domain.Common.Interfaces
{
    public interface IEntity
    {
        long Id { get; set; }
        bool IsDeleted { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        long? CreatedBy { get; set; }
        long? UpdatedBy { get; set; }
    }
}
