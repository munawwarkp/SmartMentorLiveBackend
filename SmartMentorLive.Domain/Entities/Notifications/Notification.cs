using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Domain.Enums;

namespace SmartMentorLive.Domain.Entities.Notifications
{
    public class Notification:BaseEntity
    {
        public int UserId {  get; set; } //reciepient
        public User User { get; set; }


        //Notification content
        public string Title {  get; set; }
        public string Message {  get; set; }
        //users give direct link to the relevent action
        public string? Url {  get; set; }

        //type and priority
        public NotificationType NotificationType { get; set; }
        public NotificationPriority Priority { get; set; }

        public bool SendEmail { get; set; } = false;
    }
}
