using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WTS.Events
{
    /// <summary>
    /// Summary description for EventType
    /// </summary>
    public enum EventTypeEnum
    {
        Email = 1,
        CleanEventQueue = 2,
        RunReport = 3,
        CleanReportQueue = 4
    }
}