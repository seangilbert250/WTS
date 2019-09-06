using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WTS.Reports
{
    public class ReportFactory
    {
        public static IReport CreateReport(int reportTypeID)
        {
            IReport rpt = null;

            switch (reportTypeID)
            {
                case (int)ReportTypeEnum.WorkloadSummaryReport:
                    rpt = new WorkloadSummaryReport();
                    break;
                case (int)ReportTypeEnum.CRReport:
                    rpt = new CRReport();
                    break;
                case (int)ReportTypeEnum.TaskReport:
                    rpt = new TaskReport();
                    break;
                case (int)ReportTypeEnum.ReleaseDSEReport:
                    rpt = new ReleaseDSEReport();
                    break;
            }

            return rpt;
        }
    }
}