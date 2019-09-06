using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace WTS.Events
{
    /// <summary>
    /// Summary description for EventQueueService
    /// </summary>
    public class EventQueueService
    {
        private static EventQueueService service;
        private static Thread t;
        private static object lockObj = new object();
        public static int EventQueueServiceCount { get; set; }
        public static string EventQueueServiceInstanceID { get; set; }

        private bool stopped = false;

        public EventQueueService()
        {
            EventQueueServiceCount++;
            EventQueueServiceInstanceID = DateTime.Now.Ticks.ToString();
            EventQueueServiceInstanceID = EventQueueServiceInstanceID.Substring(EventQueueServiceInstanceID.Length - 6);
        }

        ~EventQueueService()
        {
            EventQueueServiceCount--;

            if (service != null)
            {
                Stop();
            }
            EventQueue.Reset();
        }

        public static string StartService()
        {
            if (!WTSConfiguration.EventQueueEnabled)
            {
                LogUtility.Log("EventQueue is disabled in app settings.");

                return "SERVICEDISABLED";
            }

            if (t == null)
            {
                lock (lockObj)
                {
                    if (t == null)
                    {
                        t = new Thread(new ThreadStart(Start));
                        t.Start();

                        return "SERVICESTARTED";
                    }
                    else
                    {
                        return "SERVICEALREADYSTARTED";
                    }
                }
            }
            else
            {
                return "SERVICEALREADYSTARTED";
            }
        }

        public static void Start()
        {
            LogUtility.Log("Starting EventQueueService...");

            service = new EventQueueService();
            service.RunJob();
        }

        public static void Stop()
        {
            LogUtility.Log("Stopping EventQueueService...");

            if (service != null)
            {
                service.stopped = true;
                service = null;
            }
        }

        public void RunJob()
        {
            int frequency = WTSConfiguration.EventQueueRunFrequencySeconds;
            
            LogUtility.Log("EventQueueService started (" + EventQueueServiceInstanceID + ").");

            do
            {
                // process event queue
                try
                {
                    if (!stopped) // needed because someone can step stop BEFORE the queue processes but AFTER the last while loop (ensures we don't run one extra time after a stop is issued)
                    {
                        EventQueue.Instance.ProcessEventQueue();
                    }                    
                }
                catch (Exception ex)
                {
                    LogUtility.LogError("EventQueueService encountered an error (" + EventQueueServiceInstanceID + "). " + ex.Message + ". " + ex.StackTrace);
                }

                (new ManualResetEvent(false)).WaitOne(frequency * 1000);
            } while (!stopped);

            LogUtility.Log("EventQueueService stopped (" + EventQueueServiceInstanceID + ").");
        }
    }
}