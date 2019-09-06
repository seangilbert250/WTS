using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using WTS.Events;

namespace WTS
{
    public class EventEngineController : ApiController
    {
        [HttpGet]
        [Route("eventengine/start/{Id}")]
        public string Start(int Id = 0)
        {
            if (Id > 0)
            {
                EventQueue.Instance.ProcessEventQueue();

                return "Ran ProcessEventQueue() one time.";
            }
            else
            {
                return EventQueueService.StartService();
            }
        }

        [HttpGet]
        [Route("eventengine/log/{Id}")]
        public HttpResponseMessage Log(int Id = 0)
        {
            List<Tuple<DateTime, string>> log = EventQueue.Instance.GetLog(Id);

            StringBuilder str = new StringBuilder();

            if (log.Count > 0)
            {
                int idx = 1;

                foreach (var t in log)
                {
                    str.Append(idx++ + ". " + t.Item1.ToString("MM/dd/yyyy HH:mm:ss.fff") + ": " + t.Item2 + "<br>");
                }
            }
            else
            {
                str.Append("No log entries found.");
            }


            return new HttpResponseMessage()
                {
                    Content = new StringContent(
                        str.ToString(),
                        Encoding.UTF8,
                        "text/html"
                    )
                };
        }

        [HttpGet]
        [Route("eventengine/service/{Id}")]
        public HttpResponseMessage Service(int Id = 0)
        {
            int serviceInstances = EventQueueService.EventQueueServiceCount;
            int queueInstances = EventQueue.EventQueueInstanceCount;

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                        "EventQueueServiceCount = " + serviceInstances + "<br>" + "EventQueueInstanceCount = " + queueInstances,
                        Encoding.UTF8,
                        "text/html"
                    )
            };
        }
    }
}
