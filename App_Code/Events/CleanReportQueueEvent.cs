using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WTS.Reports;

namespace WTS.Events
{
    /// <summary>
    /// Summary description for CleanReportQueueEvent
    /// </summary>
    public class CleanReportQueueEvent : EventBase
    {
        public CleanReportQueueEvent() : base()
        {
            EventTypeID = (int)EventTypeEnum.CleanReportQueue;
        }

        public override int Execute()
        {
            ReportQueue.Instance.CleanReports(WTSConfiguration.ReportQueueCleanMaxAgeHours, true);

            DateTime tomorrow = DateTime.Now.AddDays(1.0);
            EventQueue.Instance.RescheduleEvent(this, new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day));

            return (int)EventStatusEnum.Complete;
        }
    }
}