using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Subscriptions;

namespace SmartMentorLive.Domain.Entities.Users
{
    public class StudentProfile:BaseEntity
    {
        //link to user
        public int UserId {  get; set; }
        public User User { get; set; }

        //student-specific field
        public string? Phone {  get; set; }
        public DateTime ? DateOfBirth { get; set; }
        public string? Gender {  get; set; }
        public string? ProfileImageUrl {  get; set; }


        public ICollection<StudentSubscription> StudentSubscriptions { get; set; } = new List<StudentSubscription>();

        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    }
}
