using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentorLive.Domain.Enums
{
    public enum NotificationType
    {
        General,
        SessionBooked,
        SessionCancelled,
        PaymentRecieved,
        MentorApproved,
        MentorRejected,
        SubscriptionUpdated,
        Other = 99
    }
}
