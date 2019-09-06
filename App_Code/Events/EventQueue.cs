using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace WTS.Events
{
    /// <summary>
    /// Summary description for EventQueue
    /// </summary>
    public class EventQueue
    {
        private static EventQueue instance = null;
        private static object lockObj = new object();
        private static bool initialized = false;
        public static int EventQueueInstanceCount { get; set; }
        public static string EventQueueInstanceID { get; set; }

        // we don't want to completely spam the database with event queue log info every 5 seconds,
        // so instead we compromise by having an in-memory log; we can view the log view the
        // event engine controller
        private List<Tuple<DateTime, string>> log = new List<Tuple<DateTime, string>>();

        public static EventQueue Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj) // we lock AFTER the instance==null check for performance reasons - if instance is not null we don't face a performance hit
                    {
                        if (instance == null) // it's possible for 2 calls to get passed the first instance==null check, so we do another check inside the lock block just to ensure instance isn't recreated again (the lock block is guaranteed to only run by one caller at a time)
                        {
                            instance = new EventQueue();
                        }
                    }
                }

                return instance;
            }
        }

        public static void Reset()
        {
            instance = null;
        }

        private EventQueue()
        {
            if (!initialized)
            {
                Log("Initializing EventQueue.");
                EventQueueInstanceCount++;
                EventQueueInstanceID = DateTime.Now.Ticks.ToString();
                EventQueueInstanceID = EventQueueInstanceID.Substring(EventQueueInstanceID.Length - 6);

                AddDefaultQueueEvents();
                initialized = true;
            }
        }

        ~EventQueue()
        {
            EventQueueInstanceCount--;
        }

        public IEvent QueueEvent(IEvent evt)
        {
            Log("Queuing event: " + Enum.GetName(typeof(EventTypeEnum), evt.EventTypeID) + ".");

            SaveEvent(evt);

            return evt;
        }

        #region Event Logic

        public void Log(string s)
        {
            if (log == null)
            {
                log = new List<Tuple<DateTime, string>>();
            }

            log.Add(new Tuple<DateTime, string>(DateTime.Now, s + " (" + EventQueueInstanceID + ")"));

            if (log.Count > 1000)
            {
                log.RemoveRange(0, 100);
            }
        }

        public List<Tuple<DateTime, string>> GetLog(int count)
        {
            List<Tuple<DateTime, string>> list = new List<Tuple<DateTime, string>>();

            if (count == 0)
            {
                list = new List<Tuple<DateTime, string>>(log);
                list.Reverse();

                return list;
            }
            else
            {
                for (int i = log.Count - 1; i >= 0 && i >= (log.Count - count); i--)
                {
                    list.Add(log[i]);
                }
            }

            return list;
        }

        private void AddDefaultProperties(IEvent evt)
        {
            evt.EventStatusID = (int)EventStatusEnum.Scheduled;
            evt.CreatedDate = DateTime.Now;
            evt.ScheduledDate = evt.CreatedDate;
            evt.CreatedBy = HttpContext.Current.User.Identity.Name;
            evt.Payload = evt.SerializePayload();
        }

        public void ProcessEventQueue()
        {
            Log("Processing EventQueue. Loading events...");

            List<IEvent> events = GetEvents((int)EventStatusEnum.Scheduled, 0);

            Log("Found " + events.Count + " scheduled events.");

            foreach (IEvent evt in events)
            {
                Log("Processing event " + evt.EventQueueID + " (" + Enum.GetName(typeof(EventTypeEnum), evt.EventTypeID) + ").");
                UpdateEventStatus(evt.EventQueueID, (int)EventStatusEnum.Executing, DateTime.MinValue, null, null);

                try
                {
                    int newStatusID = evt.Execute();

                    Log("Event " + evt.EventQueueID + " finished executing and returned status " + (newStatusID > 0 ? Enum.GetName(typeof(EventStatusEnum), newStatusID) : "[EXECUTING]") + ".");

                    // do nothing - some event types set their own statuses at a later date and return 0 for now
                    if (newStatusID > 0)
                    {
                        UpdateEventStatus(evt.EventQueueID, newStatusID, DateTime.Now, ((EventStatusEnum)newStatusID).ToString(), null);
                    }   
                }
                catch (Exception ex)
                {
                    Log("Exception encountered executing event " + evt.EventQueueID + ". " + ex.Message + " " + ex.StackTrace);
                    LogUtility.LogException(ex);
                    UpdateEventStatus(evt.EventQueueID, (int)EventStatusEnum.Error, DateTime.Now, "Exception encountered executing event.", ex.Message + " " + ex.StackTrace);

                    if (evt.RescheduleOnException) // default implementation of events is to not reschedule; events can change this by overriding default properties
                    {
                        RescheduleEvent(evt, DateTime.Now.AddMinutes(evt.RescheduleOnExceptionWaitPeriod));
                    }
                }
            }
        }

        public IEvent RescheduleEvent(IEvent evt, DateTime rescheduledDate)
        {
            IEvent reschEvt = evt.CopyEvent(evt);

            reschEvt.EventQueueID = 0;
            reschEvt.EventStatusID = (int)EventStatusEnum.Scheduled;
            reschEvt.CompletedDate = DateTime.MinValue;
            reschEvt.ScheduledDate = rescheduledDate;
            reschEvt.Result = null;
            reschEvt.Error = null;

            SaveEvent(reschEvt);

            Log("Event " + evt.EventQueueID + " rescheduled for " + rescheduledDate.ToString("MM/dd/yyyy HH:mm:ss") + " (" + reschEvt.EventQueueID + ").");

            return reschEvt;
        }

        /// <summary>
        /// This method checks to see if certain default events are active and scheduled, and if not, it adds them
        /// </summary>
        public void AddDefaultQueueEvents()
        {
            List<IEvent> events = GetEvents((int)EventStatusEnum.Scheduled, (int)EventTypeEnum.CleanEventQueue, true);
            if (events.Count == 0)
            {
                IEvent evt = EventFactory.CreateEvent((int)EventTypeEnum.CleanEventQueue);
                DateTime tomorrow = DateTime.Now.AddDays(1.0);
                evt.ScheduledDate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day);
                SaveEvent(evt);
            }

            events = GetEvents((int)EventStatusEnum.Scheduled, (int)EventTypeEnum.CleanReportQueue, true);
            if (events.Count == 0)
            {
                IEvent evt = EventFactory.CreateEvent((int)EventTypeEnum.CleanReportQueue);
                DateTime tomorrow = DateTime.Now.AddDays(1.0);
                evt.ScheduledDate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day);
                SaveEvent(evt);
            }
        }

        #endregion

        #region Data Access

        public IEvent SaveEvent(IEvent evt)
        {        
            string procName = "EventQueue_Save";

            try
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        
                        cmd.Parameters.Add("@EventQueueID", SqlDbType.BigInt).Value = evt.EventQueueID;
                        cmd.Parameters["@EventQueueID"].Direction = ParameterDirection.InputOutput;
                        cmd.Parameters.Add("@EVENT_TYPEID", SqlDbType.Int).Value = evt.EventTypeID;
                        cmd.Parameters.Add("@EVENT_STATUSID", SqlDbType.Int).Value = evt.EventStatusID;
                        cmd.Parameters.Add("@ScheduledDate", SqlDbType.DateTime).Value = evt.ScheduledDate;
                        cmd.Parameters.Add("@CompletedDate", SqlDbType.DateTime).Value = evt.CompletedDate != DateTime.MinValue ? evt.CompletedDate : (object)DBNull.Value;
                        cmd.Parameters.Add("@Payload", SqlDbType.NVarChar).Value = evt.Payload ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = evt.CreatedBy ?? "SYSTEM";
                        cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = evt.CreatedDate;
                        cmd.Parameters.Add("@Result", SqlDbType.NVarChar).Value = evt.Result ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Error", SqlDbType.NVarChar).Value = evt.Error ?? (object)DBNull.Value;
                        
                        cmd.ExecuteNonQuery();

                        SqlParameter eventID = cmd.Parameters["@EventQueueID"];

                        evt.EventQueueID = (long)eventID.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
            }

            return evt;
        }

        public bool UpdateEventStatus(long eventQueueID, int eventStatusID, DateTime completedDate, string result, string error)
        {
            string procName = "EventQueue_UpdateStatus";

            try
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@EventQueueID", SqlDbType.BigInt).Value = eventQueueID;
                        cmd.Parameters.Add("@EVENT_STATUSID", SqlDbType.Int).Value = eventStatusID;
                        cmd.Parameters.Add("@CompletedDate", SqlDbType.DateTime).Value = completedDate != DateTime.MinValue ? completedDate : (object)DBNull.Value;
                        cmd.Parameters.Add("@Result", SqlDbType.NVarChar).Value = result ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Error", SqlDbType.NVarChar).Value = error ?? (object)DBNull.Value;

                        int cnt = cmd.ExecuteNonQuery();

                        if (cnt == 0)
                        {
                            Log("Invalid EventQueueID (" + eventQueueID + "). Status could not be updated.");
                            LogUtility.Log("Invalid EventQueueID. Status could not be updated.");
                        }
                        else
                        {
                            Log("UpdateEventStatus(" + eventQueueID + ", " + Enum.GetName(typeof(EventStatusEnum), eventStatusID) + ", " + (completedDate != DateTime.MinValue ? completedDate.ToString("MM/dd/yyyy HH:mm:ss.fff") : "NOT COMPLETE") + ", " + result + ", " + error + ".");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
                return false;
            }

            return true;
        }        

        public List<IEvent> GetEvents(int eventStatusID, int eventTypeID, bool includeFutureEvents = false)
        {
            List<IEvent> events = new List<IEvent>();

            string procName = "EventQueue_Get";

            using (DataTable dt = new DataTable("Data"))
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@EVENT_STATUSID", SqlDbType.Int).Value = eventStatusID;
                        cmd.Parameters.Add("@EVENT_TYPEID", SqlDbType.Int).Value = eventTypeID;
                        cmd.Parameters.Add("@MaxDate", SqlDbType.DateTime).Value = includeFutureEvents ? (object)DBNull.Value : DateTime.Now;

                        try
                        {
                            using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                            {
                                if (dr != null)
                                {
                                    dt.Load(dr);

                                    foreach (DataRow row in dt.Rows)
                                    {
                                        int loadedEventTypeID = (int)row["EVENT_TYPEID"];
                                        long eventQueueID = (long)row["EventQueueID"];

                                        IEvent evt = EventFactory.CreateEvent(loadedEventTypeID);

                                        if (evt != null)
                                        {
                                            try
                                            {
                                                evt.Load(row);
                                                events.Add(evt);
                                            }
                                            catch (Exception ex)
                                            {
                                                LogUtility.LogException(ex);
                                                UpdateEventStatus(eventQueueID, (int)EventStatusEnum.Error, DateTime.Now, "Unable to parse event.", ex.Message + " " + ex.StackTrace);
                                            }
                                        }
                                        else
                                        {
                                            LogUtility.LogError("Unable to parse event. Invalid EVENT_TYPEID (" + loadedEventTypeID + ").");
                                            UpdateEventStatus(eventQueueID, (int)EventStatusEnum.Error, DateTime.Now, "Unable to parse event.", "Unable to parse event. Invalid EVENT_TYPEID (" + loadedEventTypeID + ").");
                                        }
                                    }
                                }
                                else
                                {
                                    return new List<IEvent>();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogException(ex);
                            throw;
                        }
                    }
                }
            }

            return events;
        }

        public void CleanEvents(int maxHours, bool cleanErrors = false)
        {
            string procName = "EventQueue_Clean";

            try
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@MaxHours", SqlDbType.Int).Value = maxHours;
                        cmd.Parameters.Add("@CleanErrors", SqlDbType.Bit).Value = cleanErrors;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
            }
        }

        #endregion

        #region Event Helpers

        public IEvent QueueEmailEvent(Dictionary<string, string> toAddresses
            , Dictionary<string, string> ccAddresses = null
            , Dictionary<string, string> bccAddresses = null
            , string subject = ""
            , string body = ""
            , string fromAddress = null
            , string fromDisplayName = null
            , bool formatAsHtml = true
            , MailPriority priority = MailPriority.Normal
            , Dictionary<string, byte[]> attachments = null
            , bool sendCcToSystemEmail = true)
        {
            EmailEvent evt = (EmailEvent)EventFactory.CreateEvent((int)EventTypeEnum.Email);

            evt.ToAddresses = toAddresses;
            evt.CcAddresses = ccAddresses;
            evt.BccAddresses = bccAddresses;
            evt.Subject = subject;
            evt.Body = body;
            evt.FromAddress = fromAddress;
            evt.FromDisplayName = fromDisplayName;
            evt.FormatAsHtml = formatAsHtml;
            evt.Priority = priority;
            evt.Attachments = attachments;
            evt.SendCcToSystemEmail = sendCcToSystemEmail;

            AddDefaultProperties(evt);

            return QueueEvent(evt);            
        }

        public IEvent QueueRunReportEvent(long reportQueueID, DateTime scheduledDate)
        {
            RunReportEvent evt = (RunReportEvent)EventFactory.CreateEvent((int)EventTypeEnum.RunReport);

            evt.ReportQueueID = reportQueueID;

            AddDefaultProperties(evt);

            if (scheduledDate != DateTime.MinValue)
            {
                evt.ScheduledDate = scheduledDate;
            }

            return QueueEvent(evt);            
        }

        #endregion
    }
}
 