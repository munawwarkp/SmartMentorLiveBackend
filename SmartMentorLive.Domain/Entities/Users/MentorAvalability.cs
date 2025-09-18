using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Sessions;
using SmartMentorLive.Domain.Enums;

namespace SmartMentorLive.Domain.Entities.Users
{
    public class MentorAvalability:BaseEntity
    {
        public int MentorProfileId {  get; set; }
        public MentorProfile MentorProfile { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool IsRecurring {  get; set; }
        public ICollection<MentorshipSessionBooking>? MentorshipSessionBookings { get; set; }

        //check if the slot is availble
        public bool IsAvailable => MentorshipSessionBookings == null ||
                                   !MentorshipSessionBookings.Any(b => b.Status == BookingStatus.Confirmed);
    }
}
