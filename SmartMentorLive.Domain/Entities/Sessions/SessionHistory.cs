using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Domain.Entities.Sessions
{
    public class SessionHistory:BaseEntity
    {
        //who attended
        public int StudentProfileId {  get; set; }
        public StudentProfile Student {  get; set; }

        public int MentorProfileId {  get; set; }
        public MentorProfile Mentor { get; set; }

        //which session ths belongs
        public int? MentorshipSessionId { get; set; }
        public MentorshipSessionBooking? MentorshipSession {  get; set; }

        public int? GroupSessionId { get; set; }
        public GroupSession? GroupSession { get; set; }


    }
}
