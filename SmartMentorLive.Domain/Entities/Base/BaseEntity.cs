using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentorLive.Domain.Entities.Base
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        //audit field
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        //soft delete
        public bool IsDeleted { get; set; } = false;
        //public DateTime? DeletedDate { get; set; }
        //public string? DeletedBy { get; set; }
    }
}
