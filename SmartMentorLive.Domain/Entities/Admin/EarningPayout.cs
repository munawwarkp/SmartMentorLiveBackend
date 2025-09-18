using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Subscriptions;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Domain.Enums;

namespace SmartMentorLive.Domain.Entities.admin
{
    //batch settlment
    //record when the payment was paid out to the mentor
    public class EarningPayout:BaseEntity
    {

        // Mentor who will receive this earning
        public int MentorProfileId { get; set; }
        public MentorProfile MentorProfile { get; set; }


        public ICollection<MentorEarning> MentorEarnings { get; set; }=new List<MentorEarning>();


        // Financial details
        public decimal TotalGross { get; set; }        // Total paid by student
        public decimal TotalCommission { get; set; }         // Platform fee
        public decimal NetAmount { get; set; }          // Mentor’s earning

        // Payout tracking
        public PayoutStatus Status { get; set; } = PayoutStatus.Pending; // Pending / Paid / Failed
        public DateTime? PaidAt { get; private set; }           // When payout was done


        public void MarkAsPaid()
        {
            if (Status == PayoutStatus.Paid)
                throw new InvalidOperationException("Payout already processed.");
            if (Status == PayoutStatus.Failed)
                throw new InvalidOperationException("Cannot mark a failed payout as paid.");

            Status = PayoutStatus.Paid;
            PaidAt = DateTime.UtcNow;
        }

        public void MarkAsFailed()
        {
            if (Status == PayoutStatus.Paid)
                throw new InvalidOperationException("Cannot fail an already paid payout.");

            Status = PayoutStatus.Failed;
        }

    }
}
