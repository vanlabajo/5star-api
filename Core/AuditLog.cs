using System;

namespace Core
{
    public class AuditLog
    {
        public DateTime CreatedTime { get; private set; }
        public DateTime ModifiedTime { get; private set; }
        public string CreatedBy { get; private set; }
        public string ModifiedBy { get; private set; }

        public AuditLog(string createdBy)
        {
            var timeStamp = DateTime.UtcNow;
            CreatedBy = createdBy;
            CreatedTime = timeStamp;
        }

        public void Modified(string modifiedBy)
        {
            var timeStamp = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
            ModifiedTime = timeStamp;
        }
    }
}
