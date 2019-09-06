using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WTS.Reports
{
    /// <summary>
    /// Summary description for IReport
    /// </summary>
    public interface IReport
    {
        bool ExecuteReport(Dictionary<string, string> reportParameters, int WTS_RESOURCEID, ref string errors, ref string outFileName, ref byte[] outFile);
        Dictionary<string, string> GetFilterFields();
        string GetPageTitle();
        string GetDefaultFilters();
        DataSet GetLevelFields();
    }
}