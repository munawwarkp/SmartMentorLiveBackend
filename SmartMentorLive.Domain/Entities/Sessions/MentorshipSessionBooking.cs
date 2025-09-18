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
    public class MentorshipSessionBooking:BaseEntity
    {
        public int MentorAvailabilityId {  get; set; }
        public MentorAvalability MentorAvalability { get; set; }

        public int StudentProfileId {  get; set; }
        public StudentProfile StudentProfile { get; set; }


        public DateTime BookingTime { get; set; }
        public BookingStatus Status {  get; set; }


        //meeting url
        public string? MeetingUrl {  get; set; }


        //cancellation and reshedule tracking
        public DateTime? CancelledAt { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? RescheduleTo {  get; set; }

        
        //cancel the booking
        public void Cancel(string cancelledBy)
        {
            Status = BookingStatus.Cancelled;
            CancelledAt = DateTime.Now;
            CancelledBy = cancelledBy;
        }
        //reshedule the booking
        public void Reschedule(DateTime newTime)
        {
            RescheduleTo = newTime;
            Status = BookingStatus.Pending; //require confimation again
        }

        //confirm the booking
        public void Confirm()
        {
            Status = BookingStatus.Confirmed;
        }

    }
}
