using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;

namespace SmartMentorLive.Domain.Entities.Users
{
    public class Skill:BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<MentorProfile> Mentors { get; set; } = new List<MentorProfile>();
    }

}
