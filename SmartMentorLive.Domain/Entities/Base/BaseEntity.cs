using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace SmartMentorLive.Domain.Entities.Base
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        //audit field
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }

        //tracks any update to the entity(name change, password reset, role update, profile changes, etc.).
        public DateTime? LastModifiedDate { get; set; }

        //soft delete
        public bool IsDeleted { get; set; } = false;
        //public DateTime? DeletedDate { get; set; }
        //public string? DeletedBy { get; set; }
    }
}
