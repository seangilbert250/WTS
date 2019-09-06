using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using WTS;
using WTS.Events;

public partial class Admin_Utility : System.Web.UI.Page
{
    public string output = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.QueryString["action"];
        string id = Request.QueryString["id"];
        if (string.IsNullOrWhiteSpace(id)) id = "0";

        switch (action) {
            case "start":
                output = Start(Int32.Parse(id));
                break;
            case "log":
                output = Log(Int32.Parse(id));
                break;
            case "service":
                output = Service(Int32.Parse(id));
                break;
        }

        if (output == null)
        {
            output = "[NO OUTPUT]";
        }
    }

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

    public string Log(int Id = 0)
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

        return str.ToString();
    }

    public string Service(int Id = 0)
    {
        int serviceInstances = EventQueueService.EventQueueServiceCount;
        int queueInstances = EventQueue.EventQueueInstanceCount;

        return "Service Instances: " + serviceInstances + "<br>Queue Instances:" + queueInstances;
    }
}