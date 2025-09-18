using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Domain.Entities.Industries
{
    public class MentorIndustry:BaseEntity
    {
        public int MentorProfileId { get; set; }
        public MentorProfile MentorProfile { get; set; } 

        public int IndustryId { get; set; }
        public Industry Industry { get; set; }
    }
}
