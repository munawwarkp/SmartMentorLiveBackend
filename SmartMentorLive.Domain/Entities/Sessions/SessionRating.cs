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
    public class SessionRating:BaseEntity
    {
        //who gave the rating
        public int StudentProfileId {  get; set; }
        public StudentProfile StudentProfile { get; set; }

        //which session is being rated
        public int? MentorshipSessionBookingId { get; set; }
        public MentorshipSessionBooking? MentorshipSession { get; set; }

        //group session
        public int? GroupSessionId { get; set; }
        public GroupSession? GroupSession { get; set; }

        //rating detials
        public SessionRatingValue Rating {  get; set; }
        public string? Feedback {  get; set; }

    }
}
