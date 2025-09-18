using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.admin;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Subscriptions;

namespace SmartMentorLive.Domain.Entities.Users
{
    //history per payment
    public class MentorEarning:BaseEntity
    {
        //student payment
        public int PaymentTransactionId {  get; set; }
        public PaymentTransaction PaymentTransaction { get; set; }

        //mentro who earned from this transaction
        public int MentorProfileId {  get; set; }
        public MentorProfile MentorProfile { get; set; }

        //fin details
        public decimal GrossAmount { get; set; }    // full payment amount
        public decimal Commission { get; set; }    // platform cut
        public decimal NetAmount { get; set; }     // what mentor actually earns

        //settlement
        public int? EarningPayoutId {  get; set; }
        public EarningPayout? EarningPayout { get; set; }   
    }
}
