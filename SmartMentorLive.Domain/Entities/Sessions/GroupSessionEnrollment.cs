using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Domain.Enums;

namespace SmartMentorLive.Domain.Entities.Sessions
{
    public class GroupSessionEnrollment:BaseEntity
    {
        public int GroupSessionId {  get; set; }
        public GroupSession GroupSession { get; set; }

        public int StudentProfileId {  get; set; }
        public StudentProfile StudentProfile { get; set; }

        public EnrollmentStatus EnrollmentStatus { get; set; } = EnrollmentStatus.Enrolled;
    }
}
