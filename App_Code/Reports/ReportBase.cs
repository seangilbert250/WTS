using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WTS.Reports
{
    /// <summary>
    /// Summary description for ReportBase
    /// </summary>
    public class ReportBase : IReport
    {
        public ReportBase()
        {
    
        }

        public virtual Dictionary<string, string> GetFilterFields()
        {
            return new Dictionary<string, string>();
        }

        public virtual string GetPageTitle()
        {
            return "";
        }

        public virtual string GetDefaultFilters()
        {
            return "";
        }

        public virtual DataSet GetLevelFields()
        {
            return new DataSet();
        }

        public virtual bool ExecuteReport(Dictionary<string, string> reportParameters, int WTS_RESOURCEID, ref string errors, ref string outFileName, ref byte[] outFile)
        {
            return false;
        }
    }
}