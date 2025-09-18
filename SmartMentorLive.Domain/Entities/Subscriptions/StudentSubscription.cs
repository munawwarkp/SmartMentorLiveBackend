using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Domain.Enums;

namespace SmartMentorLive.Domain.Entities.Subscriptions
{
    public class StudentSubscription:BaseEntity
    {
        //fk - student
        public int StudentProfileId {  get; set; }
        public StudentProfile StudentProfile { get; set; }


        //fk-subscriptionplan
        public int SubscriptionPlanId {  get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }

        //public int PaymentTransactionId {  get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

        //subscription lifecycle
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;

        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;


    }
}
