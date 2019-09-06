using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

using WTS;
using WTS.Events;
using WTS.Reports;

/// <summary>
/// Summary description for ReportPage
/// </summary>
public class ReportPage : WTSContentPage
{
    private static List<string> FinishedDownloads = new List<string>();
    protected static string reportSession = "";
    protected static string reportUserNM = "";
    protected static string brochureNm = "";
    protected static string FileFormat = "";
    protected static string userName = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        ClientScriptManager cs = Page.ClientScript;

        if (!cs.IsClientScriptIncludeRegistered("reports"))
        {
            cs.RegisterClientScriptInclude("reports", ResolveClientUrl("Scripts/reports.js"));
        }
    }

    protected string ReportKey
    {
        get
        {
            return Request["reportkey"];
        }
    }

    [WebMethod]
    public static string IsReportComplete(string reportKey)
    {
        return FinishedDownloads != null && FinishedDownloads.Contains(reportKey) ? "true" : "false";
    }

    public static void SetDownloadFinished(string reportKey)
    {
        if (FinishedDownloads == null)
        {
            FinishedDownloads = new List<string>();
        }

        if (FinishedDownloads.Count > 1000)
        {
            FinishedDownloads.RemoveRange(0, 100);
        }

        FinishedDownloads.Add(reportKey);
    }

    [WebMethod(true)]
    public static Dictionary<string, string> QueueReport(
            int REPORT_TYPEID,
            Dictionary<string, string> reportParameters,
            string scheduledDate)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "false" }, { "guid", "" }, { "error", "" } };

        DateTime sd = DateTime.MinValue;
        DateTime.TryParse(scheduledDate, out sd);

        try
        {
            var loggedInMembershipUser = Membership.GetUser();
            var loggedInMembershipUserId = loggedInMembershipUser.ProviderUserKey.ToString();

            var loggedInUser = new WTS_User();
            loggedInUser.Load(loggedInMembershipUserId);

            QueuedReport rpt = ReportQueue.Instance.QueueReport(Guid.NewGuid().ToString(), loggedInUser.ID, REPORT_TYPEID, sd, reportParameters, reportParameters != null && reportParameters.ContainsKey("Title") ? reportParameters["Title"] : null);
            IEvent evt = null;

            if (rpt != null && rpt.ReportQueueID > 0)
            {
                evt = EventQueue.Instance.QueueRunReportEvent(rpt.ReportQueueID, DateTime.MinValue);
            }

            if (rpt != null && evt != null)
            {
                result["success"] = "true";
                result["id"] = rpt.ReportQueueID.ToString();
                result["guid"] = rpt.Guid;
            }
            else
            {
                LogUtility.Log("Unable to create report." + (rpt == null || rpt.ReportQueueID == 0 ? " Queued Report cannot be saved." : "") + (evt == null ? " Event could not be created." : ""));
            }
        }
        catch (Exception ex)
        {
            result["error"] = ex.Message + " " + ex.StackTrace;
            LogUtility.LogException(ex);
        }

        return result;
    }

    [WebMethod(true)]
    public static Dictionary<string, object> getEmptyFilters(Dictionary<string, object> reportParameters, string reportTypeID)
    {
        IReport report = ReportFactory.CreateReport(Int32.Parse(reportTypeID));
        Dictionary<string, string> reportFilters = report.GetFilterFields();
        foreach (var filter in reportFilters)
        {
            if (!reportParameters.ContainsKey(filter.Key))
            {
                DataTable dt = Filtering.LOAD_FILTER_PARAMS("Reports", filter.Key, 1, null, true);
                if (dt != null && dt.Rows.Count > 0)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    string dataValues = string.Empty;
                    string dataText = string.Empty;

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                        {
                            dataValues += dr.ItemArray[0] + ",";
                            dataText += dr.ItemArray[1] + ",";
                        }
                        else
                        {
                            dataValues += dr.ItemArray[0];
                            dataText += dr.ItemArray[1];
                        }
                        
                    }
                    data["value"] = dataValues;
                    data["text"] = dataText;
                    reportParameters[filter.Key] = data;
                }
            }
        }
        return reportParameters;
    }

    [WebMethod(true)]
    public static string CheckCustomView(
    int REPORT_TYPEID,
    string viewName,
    Dictionary<string, string> reportParameters,
    string reportLevels
        )
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "exists", "false" }, { "error", "" } };
        bool exists = false;
        string errorMsg = string.Empty;

        try
        {
            var loggedInMembershipUser = Membership.GetUser();
            var loggedInMembershipUserId = loggedInMembershipUser.ProviderUserKey.ToString();

            var loggedInUser = new WTS_User();
            loggedInUser.Load(loggedInMembershipUserId);

            exists = Filtering.CheckReportViewExist(loggedInUser.ID, REPORT_TYPEID, viewName);

        }
        catch (Exception ex)
        {
            result["error"] = ex.Message + " " + ex.StackTrace;
            LogUtility.LogException(ex);
        }
        result["viewName"] = viewName;
        result["reportParameters"] = JsonConvert.SerializeObject(reportParameters, Newtonsoft.Json.Formatting.None);
        result["reportLevels"] = reportLevels;
        result["exists"] = exists.ToString();

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod(true)]
    public static string SaveCustomView(
        int reportViewID,
        string viewName,
        int REPORT_TYPEID,
        int processView,
        Dictionary<string, string> reportParameters,
        string reportLevels)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "viewName", viewName }, { "viewID", reportViewID.ToString() }, { "customFilter", "" }, { "error", "" } };
        int savedID = -1;
        string errorMsg = string.Empty;

        try
        {
            var loggedInMembershipUser = Membership.GetUser();
            var loggedInMembershipUserId = loggedInMembershipUser.ProviderUserKey.ToString();

            var loggedInUser = new WTS_User();
            loggedInUser.Load(loggedInMembershipUserId);

            if (reportParameters != null)
            {
                savedID = Filtering.SaveCustomReportView((processView == 1 ? 0 : loggedInUser.ID), reportViewID, REPORT_TYPEID, viewName, reportParameters, reportLevels);
            }
            if (savedID > -1)
            {
                result["viewid"] = savedID.ToString();
                result["saved"] = "True";
            }
        }
        catch (Exception ex)
        {
            result["error"] = ex.Message + " " + ex.StackTrace;
            LogUtility.LogException(ex);
        }

        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod(true)]
    public static string loadCustomView(int viewID, int REPORT_TYPEID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "viewID", viewID.ToString() }, { "customFilter", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            DataTable dt = new DataTable();

            if (viewID > 0)
            {
                dt = Filtering.LoadReportViews(viewID, REPORT_TYPEID);
            }
            foreach (DataRow row in dt.Rows)
            {
                result["Report Parameters"] = row["ReportParameters"].ToString();
                result["Report Levels"] = row["ReportLevels"].ToString();
            }
        }
        catch (Exception ex)
        {
            result["error"] = ex.Message + " " + ex.StackTrace;
            LogUtility.LogException(ex);
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod(true)]
    public static string DeleteView(int viewID, int REPORT_TYPEID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "false" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            if (viewID > 0)
            {
                deleted = Filtering.DeleteReportView(viewID, REPORT_TYPEID, out errorMsg);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            deleted = false;
        }

        result["deleted"] = deleted.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
}