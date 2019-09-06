using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WTS.Events
{
    /// <summary>
    /// Summary description for CleanEventQueueEvent
    /// </summary>
    public class CleanEventQueueEvent : EventBase
    {
        public CleanEventQueueEvent() : base()
        {
            EventTypeID = (int)EventTypeEnum.CleanEventQueue;
        }

        public override int Execute()
        {
            EventQueue.Instance.CleanEvents(WTSConfiguration.EventQueueCleanMaxAgeHours);

            DateTime tomorrow = DateTime.Now.AddDays(1.0);
            EventQueue.Instance.RescheduleEvent(this, new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day));

            return (int)EventStatusEnum.Complete;
        }
    }
}