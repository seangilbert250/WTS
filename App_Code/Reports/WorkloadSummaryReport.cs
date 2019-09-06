using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WTS.Reports
{
    /// <summary>
    /// Summary description for WorkloadSummaryReport
    /// </summary>
    public class WorkloadSummaryReport : ReportBase
    {
        public WorkloadSummaryReport()
        {

        }

        public override bool ExecuteReport(Dictionary<string, string> reportParameters, int WTS_RESOURCEID, ref string errors, ref string outFileName, ref byte[] outFile)
        {
            return false;
        }
    }
}