using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;

namespace SmartMentorLive.Domain.Entities.Users
{
    public class Role:BaseEntity
    {
        public string Name { get; set; }

        // Business rule: can the user choose this role during signup?
        public bool IsRegistrable { get; set; } = false;

        public ICollection<User> Users { get; set; }
    }
}
