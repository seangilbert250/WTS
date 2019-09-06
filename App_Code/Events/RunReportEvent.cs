using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Newtonsoft.Json;

using WTS.Reports;

namespace WTS.Events
{
    /// <summary>
    /// Summary description for CleanEventQueueEvent
    /// </summary>
    public class RunReportEvent : EventBase
    {
        public long ReportQueueID { get; set; }
        
        public RunReportEvent() : base()
        {
            EventTypeID = (int)EventTypeEnum.RunReport;            
        }

        public override string SerializePayload()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("ReportQueueID", ReportQueueID.ToString());

            return JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.None);
        }

        public override void ParsePayload(string payload)
        {
            if (payload == null)
            {
                return;
            }

            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(payload);

            ReportQueueID = Convert.ToInt64(dict["ReportQueueID"]);
        }

        public override int Execute()
        {
            // when this event executes, it tries to run the specified report in a separate thread
            // we return 0 for the result so that the event isn't closed out yet (we let the new run report thread close out the event)

            Thread t = new Thread(new ThreadStart(RunReport));
            t.Start();

            return 0;
        }

        public void RunReport()
        {
            try
            {
                QueuedReport rpt = ReportQueue.Instance.GetReport(ReportQueueID, null, false);

                if (rpt != null)
                {
                    string errors = rpt.RunReport();

                    if (string.IsNullOrWhiteSpace(errors))
                    {
                        EventQueue.Instance.UpdateEventStatus(this.EventQueueID, (int)EventStatusEnum.Complete, DateTime.Now, EventStatusEnum.Complete.ToString(), null);
                    }
                    else
                    {
                        EventQueue.Instance.UpdateEventStatus(this.EventQueueID, (int)EventStatusEnum.Error, DateTime.Now, EventStatusEnum.Error.ToString(), errors);
                    }
                }
                else
                {
                    LogUtility.LogError("Unable to load queued report (" + ReportQueueID + "). Report may have been deleted prior to the event executing.");
                    EventQueue.Instance.UpdateEventStatus(this.EventQueueID, (int)EventStatusEnum.Error, DateTime.Now, EventStatusEnum.Error.ToString(), "Unable to load queued report (" + ReportQueueID + "). Report may have been deleted prior to the event executing.");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
                EventQueue.Instance.UpdateEventStatus(this.EventQueueID, (int)EventStatusEnum.Error, DateTime.Now, EventStatusEnum.Error.ToString(), ex.Message + " " + ex.StackTrace);
            }
        }
    }
}