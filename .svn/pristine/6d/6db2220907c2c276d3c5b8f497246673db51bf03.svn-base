using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WTS.Events
{
    /// <summary>
    /// Interface for all events.
    /// </summary>
    public interface IEvent
    {
        // properties set for each event
        long EventQueueID { get; set; }
        int EventTypeID { get; set; }
        int EventStatusID { get; set; }
        DateTime ScheduledDate { get; set; }
        DateTime CompletedDate { get; set; }
        string Payload { get; set; }
        string CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        string Result { get; set; }
        string Error { get; set; }


        bool RescheduleOnException { get; set; }
        int RescheduleOnExceptionWaitPeriod { get; set; } // minutes

        string SerializePayload();
        void ParsePayload(string payload);
        void Load(DataRow dr);
        IEvent CopyEvent(IEvent evt);

        int Execute();
    }
}