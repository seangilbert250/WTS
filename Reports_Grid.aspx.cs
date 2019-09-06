using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;
using WTS.Reports;

using Newtonsoft.Json;


public partial class Reports_Grid : ReportPage
{
    protected enum ColIdx
    {
        Idx = 0,
        ReportQueueID,
        Download,
        Name,
        Type,
        CreatedDate,
        CreatedBy,      
        OutFileSize,
        Duration,        
        AvgTime,
        Status,
        WTS_RESOURCEID,        
        Archive,
        Delete
    }

    protected bool AllowEdit = false;
    protected bool AllowDelete = false;

    protected bool ShowArchived = false;

	private Dictionary<string, string> _sortFieldsCollection = new Dictionary<string, string>();
	protected string SortFields = string.Empty;

	protected string SelectedSortFieldName = "Name";
	protected string SelectedSortDirection = "ASC";

    protected int WTS_RESOURCEID = 0;

    #region Events
    protected void Page_PreInit(object sender, EventArgs e)
    {
        //load theme for user
		Page.Theme = WTSUtility.ThemeName;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        bool export = false;

		#region QueryString
		if (Request.QueryString["SortField"] != null &&
			!string.IsNullOrWhiteSpace(Request.QueryString["SortField"]))
		{
			this.SelectedSortFieldName = Server.UrlDecode(Request.QueryString["SortField"]);
		}
		if (Request.QueryString["SortDirection"] != null &&
			!string.IsNullOrWhiteSpace(Request.QueryString["SortDirection"]))
		{
			this.SelectedSortDirection = Server.UrlDecode(Request.QueryString["SortDirection"]);
		}
        if (Request.QueryString["ShowArchived"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"]), out ShowArchived);
        }

        if (Request.QueryString["Export"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out export);
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["WTS_RESOURCEID"]))
        {
            WTS_RESOURCEID = Int32.Parse(Request.QueryString["WTS_RESOURCEID"]);
        }
        else
        {
            WTS_RESOURCEID = LoggedInUserID;
        }
        #endregion

        if (Request.QueryString["Download"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Download"]))
        {
            DownloadReport(Convert.ToInt64(Request.QueryString["ReportQueueID"]));
        }
        else
        {
            CheckRoles();
            initControls();
            LoadData(export: export);
        }
    }

    private void LoadData(bool export = false)
    {
        List<QueuedReport> reports = ReportQueue.Instance.GetReports(0, null, WTS_RESOURCEID, null, null, DateTime.MinValue, false, ShowArchived, true);
        reports.Reverse(); // show newest reports at top of list

        DataTable dt = new DataTable();
        dt.Columns.Add("#");
        dt.Columns.Add("Download"); // download
        dt.Columns.Add("ReportQueueID");
        dt.Columns.Add("Name");
        dt.Columns.Add("Type");
        dt.Columns.Add("Created Date");
        dt.Columns.Add("Created By");
        dt.Columns.Add("OutFileSize");
        dt.Columns.Add("Duration");
        dt.Columns.Add("AvgTime");
        
        dt.Columns.Add("Status");        
        dt.Columns.Add("WTS_RESOURCEID");        
        dt.Columns.Add("Archive");
        dt.Columns.Add("Delete"); // delete

        int idx = 1;
        foreach (var rpt in reports)
        {
            DataRow dr = dt.NewRow();
            dr[(int)ColIdx.Idx] = idx++;
            dr[(int)ColIdx.Download] = rpt.OutFileExists;
            dr[(int)ColIdx.ReportQueueID] = rpt.ReportQueueID;
            dr[(int)ColIdx.Name] = rpt.ReportName;
            dr[(int)ColIdx.Type] = rpt.ReportType;
            dr[(int)ColIdx.Status] = rpt.ReportStatus;
            dr[(int)ColIdx.CreatedBy] = rpt.ResourceFirstName + " " + rpt.ResourceLastName;
            dr[(int)ColIdx.WTS_RESOURCEID] = rpt.WTS_RESOURCEID;
            dr[(int)ColIdx.CreatedDate] = rpt.CreatedDate.ToString("MM/dd/yyyy hh:mm tt");
            dr[(int)ColIdx.Archive] = rpt.Archive;
            dr[(int)ColIdx.OutFileSize] = rpt.OutFileSize > 0 ? (rpt.OutFileSize/ 1000).ToString("#,#") + " KB" : "-- KB";
            dr[(int)ColIdx.AvgTime] = rpt.AverageTime != 0 ? (rpt.AverageTime / (decimal)1000.0).ToString("#.###") + "s" : "--";

            if (rpt.OutFileExists)
            {
                TimeSpan ts = rpt.CompletedDate - rpt.ExecutionStartDate;
                dr[(int)ColIdx.Duration] = ts.ToString("g");
            }
            else
            {
                dr[(int)ColIdx.Duration] = "-:--:--.---";
            }

            dt.Rows.Add(dr);
        }

        Session["dtReports"] = dt;

        if (export)
        {
            ExportExcel(dt);
        }
        else
        {
            gridReports.DataSource = dt;
            gridReports.DataBind();
        }
    }

    private void initControls()
    {
        ctlResource.CustomOptions = "My Reports=" + LoggedInUserID;
        ctlResource.DefaultValue = WTS_RESOURCEID.ToString();

        gridReports.GridHeaderRowDataBound += gridOrganizations_GridHeaderRowDataBound;
        gridReports.GridRowDataBound += gridOrganizations_GridRowDataBound;
        gridReports.GridPageIndexChanging += gridOrganizations_GridPageIndexChanging;
    }

    #region Grid
    void gridOrganizations_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gridReports.PageIndex = e.NewPageIndex;
        //rebind the data
        if (Session["dtReports"] == null)
        {
            LoadData();
        }
        else
        {
            gridReports.DataSource = (DataTable)Session["dtreports"];
            gridReports.DataBind();
        }
    }

    void gridOrganizations_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;        
        DataRow dr = ((DataRowView)row.DataItem).Row;
        row.Attributes.Add("ReportQueueID", Convert.ToString(dr[(int)ColIdx.ReportQueueID]));

        formatColumnDisplay(ref row);

        if (Convert.ToBoolean(dr[(int)ColIdx.Download]))
        {
            row.Cells[(int)ColIdx.Download].Controls.Add(WTSUtility.CreateGridDownloadHyperLink(Convert.ToString(dr[(int)ColIdx.ReportQueueID]), dr[(int)ColIdx.Name].ToString().Replace("'", "")));
        }
        else
        {
            row.Cells[(int)ColIdx.Download].Text = "";

            string status = row.Cells[(int)ColIdx.Status].Text;
            if (status == "Queued")
            {
                row.Cells[(int)ColIdx.Download].Text = row.Cells[(int)ColIdx.Status].Text;
                row.Cells[(int)ColIdx.Download].Style["color"] = "green";
            }
            else if (status == "Executing")
            {
                row.Cells[(int)ColIdx.Download].Text = "<img src=\"Images/Loaders/loader.gif\" width=\"16px\" height=\"16px\">";
            }
            else if (status == "Error")
            {
                row.Cells[(int)ColIdx.Download].Text = row.Cells[(int)ColIdx.Status].Text;
                row.Cells[(int)ColIdx.Download].Style["color"] = "red";
            }

        }

        if (LoggedInUserID == Convert.ToInt64(dr[(int)ColIdx.WTS_RESOURCEID]) || UserCanArchiveReports())
        {
            row.Cells[(int)ColIdx.Archive].Controls.Add(WTSUtility.CreateGridCheckBox("Archive", Convert.ToString(dr[(int)ColIdx.ReportQueueID]), Convert.ToBoolean(dr[(int)ColIdx.Archive])));
        }
        else
        {
            row.Cells[(int)ColIdx.Archive].Text = "";
        }

        if (LoggedInUserID == Convert.ToInt64(dr[(int)ColIdx.WTS_RESOURCEID]) || UserCanDeleteReports())
        {
            string cleanName = dr[(int)ColIdx.Name].ToString().Replace("'", "");
            string reportQueueID = Convert.ToString(dr[(int)ColIdx.ReportQueueID]);

            if (Convert.ToBoolean(dr[(int)ColIdx.Download]))
            {
                row.Cells[(int)ColIdx.Delete].Controls.Add(WTSUtility.CreateGridImageButton(reportQueueID, cleanName, "imgmailreport_" + reportQueueID, "Images/Icons/email.png", 0, "imgMail_click(" + reportQueueID + ", '" + cleanName + "')", "Email Report " + cleanName, "Email Report " + cleanName, "padding-right:5px;cursor:pointer;"));
            }
            row.Cells[(int)ColIdx.Delete].Controls.Add(WTSUtility.CreateGridDeleteButton(reportQueueID, cleanName, 0, "Images/Icons/cross.png"));
        }
        else
        {
            row.Cells[(int)ColIdx.Delete].Text = "";
        }            
    }

    void gridOrganizations_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        row.Cells[(int)ColIdx.Download].Text = "STATUS";        
        row.Cells[(int)ColIdx.Name].Text = "REPORT";
        row.Cells[(int)ColIdx.Type].Text = "TYPE";
        row.Cells[(int)ColIdx.CreatedDate].Text = "REQUESTED";
        row.Cells[(int)ColIdx.CreatedBy].Text = "REQUESTED BY";
        row.Cells[(int)ColIdx.Delete].Text = "OPTIONS";
        row.Cells[(int)ColIdx.OutFileSize].Text = "SIZE";
        row.Cells[(int)ColIdx.Duration].Text = "TIME";
        row.Cells[(int)ColIdx.AvgTime].Text = "AVG";
        row.Cells[(int)ColIdx.Archive].Text = "ARCHIVE";

        formatColumnDisplay(ref row);
    }

    private void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            
        }

        // hide columns
        row.Cells[(int)ColIdx.ReportQueueID].Style["display"] = "none";
        row.Cells[(int)ColIdx.WTS_RESOURCEID].Style["display"] = "none";
        row.Cells[(int)ColIdx.Status].Style["display"] = "none";
        row.Cells[(int)ColIdx.Status].Attributes["status"] = "1";
        row.Cells[(int)ColIdx.Download].Attributes["download"] = "1";

        //size columns
        row.Cells[(int)ColIdx.Idx].Style["width"] = "25px";
        row.Cells[(int)ColIdx.Download].Style["width"] = "50px";
        row.Cells[(int)ColIdx.Name].Style["width"] = "150px";
        row.Cells[(int)ColIdx.Type].Style["width"] = "100px";        
        row.Cells[(int)ColIdx.CreatedBy].Style["width"] = "100px";
        row.Cells[(int)ColIdx.CreatedDate].Style["width"] = "100px";
        row.Cells[(int)ColIdx.Archive].Style["width"] = "35px";
        row.Cells[(int)ColIdx.Delete].Style["width"] = "15px";
        row.Cells[(int)ColIdx.OutFileSize].Style["width"] = "75px";
        row.Cells[(int)ColIdx.Duration].Style["width"] = "75px";
        row.Cells[(int)ColIdx.AvgTime].Style["width"] = "75px";

        //align columns
        row.Cells[(int)ColIdx.Idx].Style["text-align"] = "center";
        row.Cells[(int)ColIdx.Download].Style["text-align"] = "center";
        row.Cells[(int)ColIdx.Name].Style["text-align"] = "left";
        row.Cells[(int)ColIdx.Type].Style["text-align"] = "center";
        row.Cells[(int)ColIdx.Status].Style["text-align"] = "center";
        row.Cells[(int)ColIdx.CreatedBy].Style["text-align"] = "center";
        row.Cells[(int)ColIdx.CreatedDate].Style["text-align"] = "center";
        row.Cells[(int)ColIdx.Archive].Style["text-align"] = "center";
        row.Cells[(int)ColIdx.Delete].Style["text-align"] = "center";
        row.Cells[(int)ColIdx.OutFileSize].Style["text-align"] = "center";
        row.Cells[(int)ColIdx.Duration].Style["text-align"] = "center";
        row.Cells[(int)ColIdx.AvgTime].Style["text-align"] = "center";

        // bold columns
        row.Cells[(int)ColIdx.Name].Style["font-weight"] = "bold";
    }
    
    #endregion Grid

    #endregion

    [WebMethod(true)]
    public static string DeleteReport(long reportQueueID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { 
			{ "Exists", "" }
			, { "Deleted", "" }
			, { "Archived", "" }
			, { "Error", "" } };
        bool exists = false, deleted = false, archived = false;
        string errorMsg = string.Empty;

        QueuedReport rpt = null;
        
		if (!validateID(reportQueueID, true, false, false, out rpt, out errorMsg))
        {
			exists = false;
			deleted = false;
			archived = false;
        }
        else
        {
            try
            {
                exists = true;
                deleted = ReportQueue.Instance.DeleteReport(reportQueueID);
            }
            catch (Exception ex)
            {
				LogUtility.LogException(ex);
				exists = false;
				deleted = false;
				archived = false;
				errorMsg = ex.Message;
            }
        }

		result["Exists"] = exists.ToString();
		result["Deleted"] = deleted.ToString();
		result["Archived"] = archived.ToString();
		result["Error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod]
    public static string SaveReports(string changes)
    {       
        Dictionary<string, string> result = new Dictionary<string, string>() { { "Success", "true" } , { "Error", "" } };

        if (!string.IsNullOrWhiteSpace(changes))
        {
            string[] chgArr = changes.Split(';');

            foreach (var chg in chgArr)
            {
                string[] nv = chg.Split('=');

                if (nv.Length == 2)
                {
                    string id = nv[0];
                    string newValue = nv[1];

                    QueuedReport rpt = null;

                    string errorMsg = null;
                    if (validateID(Convert.ToInt64(id), false, true, false, out rpt, out errorMsg))
                    {
                        ReportQueue.Instance.UpdateReportStatus(Convert.ToInt64(id), rpt.REPORT_STATUSID, rpt.ExecutionStartDate, rpt.CompletedDate, rpt.Result, rpt.Error, rpt.OutFileName, null, rpt.OutFileSize, newValue == "1", false);
                    }
                    else
                    {
                        result["Error"] = "Unable to modify report (" + id + "). " + errorMsg;
                        result["Success"] = "false";
                        break;
                    }
                }
                else
                {
                    result["Error"] = "Invalid tokens in change array.";
                    result["Success"] = "false";
                    break;
                }
            }
        }

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod]
    public static string EmailReport(int reportQueueID, string userIDs, string names, string emails)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "Success", "false" }, { "Error", "" } };

        QueuedReport rpt = null;
        string errors = null;

        try
        {

            // first validate that user can see this report; if they can, then we allow them to email it
            if (validateID(reportQueueID, false, false, true, out rpt, out errors))
            {
                if (rpt.OutFile != null)
                {
                    string[] emailArr = emails.Split(',');
                    string[] nameArr = names.Split(',');

                    Dictionary<string, string> toAddresses = new Dictionary<string, string>();

                    for (int i = 0; i < emailArr.Length; i++)
                    {
                        if (!toAddresses.ContainsKey(emailArr[i]))
                        {
                            toAddresses.Add(emailArr[i], nameArr[i]);
                        }
                    }

                    string subject = rpt.ReportName;
                    WTS_User loggedInUser = GetLoggedInUser();
                    string body = "You have been sent the following report from " + loggedInUser.First_Name + " " + loggedInUser.Last_Name + ": " + rpt.ReportName + " (" + rpt.OutFileName + ")";
                    string from = WTSConfiguration.EmailFrom;
                    string fromName = WTSConfiguration.EmailFromName;

                    Dictionary<string, byte[]> attachments = new Dictionary<string, byte[]>();
                    attachments.Add(rpt.OutFileName, rpt.OutFile);

                    WTS.Events.EventQueue.Instance.QueueEmailEvent(toAddresses, null, null, subject, body, from, fromName, false, System.Net.Mail.MailPriority.Normal, attachments, false);

                    result["Success"] = "true";
                }
                else
                {
                    result["Error"] = "Report (" + reportQueueID + ") contains no data.";
                }
            }
            else
            {
                result["Error"] = "User cannot view this report (" + (rpt != null ? rpt.ReportName : "") + ").";
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            result["Error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    private static bool validateID(long reportQueueID, bool validateDelete, bool validateArchive, bool loadData, out QueuedReport rpt, out string errorMsg)
    {		
        errorMsg = string.Empty;
        rpt = null;

        var loggedInMembershipUser = Membership.GetUser();
        var loggedInUser = new WTS_User();
        loggedInUser.Load(loggedInMembershipUser.ProviderUserKey.ToString());

        if (reportQueueID == 0)
        {
            errorMsg = "ReportQueueID missing or invalid.";
            return false;
        }
        else
        {
            rpt = ReportQueue.Instance.GetReport(reportQueueID, null, loadData);

            if (rpt == null)
            {
                errorMsg = "Report (" + reportQueueID + ") is invalid.";
                return false;
            }

            if (validateDelete && rpt.WTS_RESOURCEID != loggedInUser.ID && !UserCanDeleteReports())
            {
                errorMsg = "User cannot delete this report.";
                return false;
            }

            if (validateArchive && rpt.WTS_RESOURCEID != loggedInUser.ID && !UserCanArchiveReports())
            {
                errorMsg = "User cannot archive this report.";
                return false;
            }
        }

        return true;
    }

    private void CheckRoles()
    {
        #region Logged In User details

        #endregion Logged In User details

        //enable/disable buttons
        if (LoggedInUser.Organization.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)
            || User.IsInRole("Admin")
			|| User.IsInRole("Administration")
			|| User.IsInRole("ResourceManagement"))
        {
            this.AllowDelete = true;
        }
    }

    private void FormatExcelColumnValues(ref DataTable dt)
    {
        string[] roles, users;
        foreach (DataColumn dc in dt.Columns)
        {
            dc.ReadOnly = false;
        }

        char[] delims = new char[] { ',' };
        foreach (DataRow row in dt.Rows)
        {
            roles = row["DefaultRoles"].ToString().Split(delims, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < roles.Length; i++)
            {
                string role = roles[i].Trim();
                role = role.Substring(role.IndexOf(':') + 1);
                roles[i] = role.Trim();
            }

            row["DefaultRoles"] = string.Join(", ", roles);
        }

        foreach (DataRow row in dt.Rows)
        {
            users = row["Users"].ToString().Split(delims, StringSplitOptions.RemoveEmptyEntries);
            row["Users"] = users.Length > 0 ? users.Length.ToString() : "";
        }
    }
    private void RemoveExcelColumns(ref DataTable dt)
    {
        try
        {
            //this has to be done in reverse order (RemoveAt) 
            //OR by name(Remove) or it will have undesired result
            dt.Columns.Remove("UpdatedDate");
            dt.Columns.Remove("UpdatedBy");
            dt.Columns.Remove("CreatedDate");
            dt.Columns.Remove("CreatedBy");
            dt.Columns.Remove("Id");

            dt.AcceptChanges();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }
    private void RenameExcelColumns(ref DataTable dt)
    {
        if (dt.Columns.Contains("User_Type"))
        {
            dt.Columns["User_Type"].ColumnName = "Organization";
        }
        if (dt.Columns.Contains("DefaultRoles"))
        {
            dt.Columns["DefaultRoles"].ColumnName = "Default Roles";
        }
        if (dt.Columns.Contains("User_Type_Descr"))
        {
            dt.Columns["User_Type_Descr"].ColumnName = "Description";
        }
        if (dt.Columns.Contains("CreatedDate"))
        {
            dt.Columns["CreatedDate"].ColumnName = "Created";
        }
        if (dt.Columns.Contains("UpdatedBy"))
        {
            dt.Columns["UpdatedBy"].ColumnName = "Updated";
        }

        dt.AcceptChanges();
    }

    private bool ExportExcel(DataTable dt)
    {
        bool success = false;
        string errorMsg = string.Empty;

        try
        {
            FormatExcelColumnValues(ref dt);
            RemoveExcelColumns(ref dt);
            RenameExcelColumns(ref dt);

            string name = "Organizations";
            Workbook wb = new Workbook(FileFormatType.Xlsx);
            Worksheet ws = wb.Worksheets[0];
            ws.Cells.ImportDataTable(dt, true, 0, 0, false, false);

			//WTSUtility.FormatWorkbookHeader(ref wb, ref ws, 0, 0, 1, dt.Columns.Count);
            
            ws.AutoFitColumns();
            MemoryStream ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Xlsx);

            Response.ContentType = "application/xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
            Response.BinaryWrite(ms.ToArray());
            Response.End();

            success = true;
        }
        catch (Exception ex)
        {
            success = false;
            errorMsg += Environment.NewLine + ex.Message;
        }

        return success;
    }

    public void DownloadReport(long reportQueueID)
    {
        QueuedReport rpt = null;
        string errors = null;

        // make sure user can view this report before downloading it
        if (validateID(reportQueueID, false, false, true, out rpt, out errors))
        {
            Response.Clear();

            if (rpt.OutFile != null)
            {
                Response.AppendHeader("content-disposition", "attachment; filename=\"" + rpt.OutFileName ?? rpt.ReportType + "\"");
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(rpt.OutFile);
            }
            else
            {
                Response.Write("Report has no data (" + reportQueueID + ").");
            }
        }
        else
        {
            Response.Write("User cannot view this report (" + (rpt != null ? rpt.ReportName : "") + ").");
        }

        SetDownloadFinished(ReportKey);
        Response.Flush();
        Response.End();
    }

    // helpers for webmethods
    public static bool UserCanDeleteReports()
    {        
        return HttpContext.Current.User.IsInRole("Admin")
           || HttpContext.Current.User.IsInRole("Administration");
    }

    public static bool UserCanArchiveReports() {
        return HttpContext.Current.User.IsInRole("Admin")
            || HttpContext.Current.User.IsInRole("Administration");
    }
}