using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.admin;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Domain.Enums;

namespace SmartMentorLive.Domain.Entities.Subscriptions
{
    //records the student purchase
    public class PaymentTransaction:BaseEntity
    {
        //stud who maid this payment
        public int StudentProfileId {  get; set; }
        public StudentProfile StudentProfile { get; set; }


        public int StudentSubscriptionId { get; set; }
        public StudentSubscription StudentSubscription { get; set; }



        public decimal Amount {  get; set; }
        public string Currency { get; set; } = "INR";
        public DateTime PaidAt { get; set; }


        //link to mentor earning
        //each transactin generates one earning recorde for mentor
        public MentorEarning MentorEarning { get; set; }
    }
}
