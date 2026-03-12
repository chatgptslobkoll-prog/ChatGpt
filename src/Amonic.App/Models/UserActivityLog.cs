using System;

namespace Amonic.App.Models
{
    public class UserActivityLog
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTime LoginAt { get; set; }
        public DateTime? LogoutAt { get; set; }
        public string CrashReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
