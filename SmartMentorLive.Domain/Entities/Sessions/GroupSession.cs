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
    public class GroupSession:BaseEntity
    {
        //mentor hosting the sesion
        public int MentorProfileId {  get; set; }
        public MentorProfile MentorProfile { get; set; }

        //session details
        public string Title {  get; set; }
        public string? Description {  get; set; }
        public DateTime StartTime { get;set; }
        public DateTime EndTime { get; set; }

        // Optional thumbnail (for frontend cards)
        public string? Thumbnail { get; set; }

        public int Capacity { get; set; } = 20;
        public bool IsRecurring {  get; set; }


        //meeting link
        public string ? MeetingUrl {  get; set; }


        public SessionStatus Status { get; set; } = SessionStatus.Scheduled;

        //enrolledStudent
        public ICollection<GroupSessionEnrollment> Enrollments { get; set; } = new List<GroupSessionEnrollment>();

        //check if sessoin is full
        public bool IsFull => Enrollments.Count >= Capacity;

    }
}
