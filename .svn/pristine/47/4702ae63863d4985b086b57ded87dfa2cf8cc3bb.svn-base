using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace WTS.Events
{
    /// <summary>
    /// Base class for all events.
    /// </summary>
    public abstract class EventBase : IEvent
    {
        public long EventQueueID { get; set; }
        public int EventTypeID { get; set; }
        public int EventStatusID { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public string Payload { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }

        
        public bool RescheduleOnException { get; set; }
        public int RescheduleOnExceptionWaitPeriod { get; set; }

        public EventBase()
        {
            RescheduleOnException = false;
            RescheduleOnExceptionWaitPeriod = 5;
        }

        public virtual string SerializePayload()
        {
            return null;
        }

        public virtual void ParsePayload(string payload)
        {
        }

        public virtual int Execute()
        {
            return (int)EventStatusEnum.Error;
        }

        public IEvent CopyEvent(IEvent evt)
        {
            // returns a cloned event with the value types copied
            // IMPORTANT: value fields like eventqueueid, status, and type are fully copied, but reference variables such as Attachments are passed by reference and after the clone the new
            // Attachments list will refer to the same as the original list; to do a deep copy, the subclass should override this event and handle on a case by case basis, if necessary
            // NOTE: this method is only called for simple rescheduling, so in most cases, a deep copy isn't need to save a new instance in the database
            return (IEvent)this.MemberwiseClone();
        }

        public virtual void Load(DataRow dr)
        {
            EventQueueID = (long)dr["EventQueueID"];
            EventStatusID = (int)dr["EVENT_STATUSID"];
            ScheduledDate = (DateTime)dr["ScheduledDate"];
            CompletedDate = dr["CompletedDate"] != DBNull.Value ? (DateTime)dr["CompletedDate"] : DateTime.MinValue;
            CreatedBy = (string)dr["CreatedBy"];
            CreatedDate = (DateTime)dr["CreatedDate"];
            Result = dr["Result"] != DBNull.Value ? (string)dr["Result"] : null;
            Error = dr["Error"] != DBNull.Value ? (string)dr["Error"] : null;

            ParsePayload(dr["Payload"] != DBNull.Value ? (string)dr["Payload"] : null);
        }
    }
}