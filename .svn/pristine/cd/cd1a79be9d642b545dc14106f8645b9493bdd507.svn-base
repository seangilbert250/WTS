using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WTS.Events
{
    /// <summary>
    /// Summary description for EventFactory
    /// </summary>
    public class EventFactory
    {
        public static IEvent CreateEvent(int eventTypeID) {
            IEvent e = null;

            switch (eventTypeID) {
                case (int)EventTypeEnum.Email:
                    e = new EmailEvent();
                    break;
                case (int)EventTypeEnum.CleanEventQueue:
                    e = new CleanEventQueueEvent();
                    break;
                case (int)EventTypeEnum.RunReport:
                    e = new RunReportEvent();
                    break;
                case (int)EventTypeEnum.CleanReportQueue:
                    e = new CleanReportQueueEvent();
                    break;
                default:
                    break;
            }

            if (e == null)
            {
                LogUtility.LogError("Invalid EVENT_TYPEID (" + eventTypeID + "). Unable to create event.");
                return null;
            }

            e.EventTypeID = eventTypeID;
            e.EventStatusID = (int)EventStatusEnum.Scheduled;
            e.CreatedBy = null; // system(?)
            e.CreatedDate = DateTime.Now;


            return e;
        }
    }
}