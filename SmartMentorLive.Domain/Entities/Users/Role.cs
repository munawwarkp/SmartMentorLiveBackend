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

        public ICollection<User> Users { get; set; }
    }
}
