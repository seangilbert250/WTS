using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

public partial class Workload_Change_History : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected bool _refreshData = false;
    protected string _type = string.Empty;
    protected string _workItemID = string.Empty;
    protected string _workItemTaskID = string.Empty;
    protected bool CanEditAOR = false;
    protected int FieldChangedIndex = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        readQueryString();
        init();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);

        if (DCC != null)
        {
            this.DCC.Clear();
        }

        loadGridData();
    }

    private void readQueryString()
    {
        if (Page.IsPostBack)
        {
            _refreshData = false;
        }
        else
        {
            if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
                || Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
            {
                _refreshData = true;
            }
        }

        if (Request.QueryString["type"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["type"].ToString()))
        {
            _type = Server.UrlDecode(Request.QueryString["type"]);
        }

        if (Request.QueryString["workItemID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["workItemID"].ToString()))
        {
            _workItemID = Server.UrlDecode(Request.QueryString["workItemID"]);
        }

        if (Request.QueryString["workItemTaskID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["workItemTaskID"].ToString()))
        {
            _workItemTaskID = Server.UrlDecode(Request.QueryString["workItemTaskID"]);
        }
    }

    private void init()
    {
        grdChangeHistory.GridHeaderRowDataBound += grdChangeHistory_GridHeaderRowDataBound;
        grdChangeHistory.GridRowDataBound += grdChangeHistory_GridRowDataBound;
        grdChangeHistory.GridPageIndexChanging += grdChangeHistory_GridPageIndexChanging;
    }

    private void loadGridData()
    {
        DataTable dtChangeHistory = null;

        if (_refreshData || Session["dtWorkload_Change_History"] == null)
        {
            if (_type == "WorkItemTask") {
                dtChangeHistory = Workload.WorkItem_Task_History_Get(_workItemTaskID, "Update");
            }
            else {
                dtChangeHistory = Workload.WorkItem_History_Get(_workItemID, "Update");
            }
            
            HttpContext.Current.Session["dtWorkload_Change_History"] = dtChangeHistory;
        }
        else
        {
            dtChangeHistory = (DataTable)HttpContext.Current.Session["dtWorkload_Change_History"];
        }

        if (dtChangeHistory != null)
        {
            spanRowCount.InnerText = dtChangeHistory.Rows.Count.ToString();
            this.DCC = dtChangeHistory.Columns;

            if (DCC.Contains("FieldChanged")) this.FieldChangedIndex = DCC.IndexOf("FieldChanged");
        }

        grdChangeHistory.DataSource = dtChangeHistory;
        grdChangeHistory.DataBind();
    }

    void grdChangeHistory_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        row.Cells[DCC.IndexOf("FieldChanged")].Text = "Field Changed";
        row.Cells[DCC.IndexOf("OldValue")].Text = "Old Value";
        row.Cells[DCC.IndexOf("NewValue")].Text = "New Value";
        row.Cells[DCC.IndexOf("UPDATEDBY")].Text = "Updated";
        row.Cells[DCC.IndexOf("UPDATEDDATE")].Text = "Updated Date";
    }

    void grdChangeHistory_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        if (DCC.Contains("WorkItem_HistoryID"))
        {
            row.Attributes.Add("workitemhistory_id", row.Cells[DCC.IndexOf("WorkItem_HistoryID")].Text);
        }
        else if (DCC.Contains("WORKITEM_TASK_HISTORYID"))
        {
            row.Attributes.Add("workitemtaskhistory_id", row.Cells[DCC.IndexOf("WORKITEM_TASK_HISTORYID")].Text);
        }

        row.Style["vertical-align"] = "top";
        row.Cells[DCC.IndexOf("OldValue")].Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("OldValue")].Text));
        row.Cells[DCC.IndexOf("NewValue")].Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("NewValue")].Text));
        //add div for html instead of text
        if (row.Cells[DCC.IndexOf("FieldChanged")].Text == "Description")
		{
			row.Cells[DCC.IndexOf("OldValue")].Controls.Add(createDivForHtml(row.Cells[DCC.IndexOf("OldValue")].Text.Trim()));
			row.Cells[DCC.IndexOf("NewValue")].Controls.Add(createDivForHtml(row.Cells[DCC.IndexOf("NewValue")].Text.Trim()));
		}

		row.Cells[DCC.IndexOf("UPDATEDBY")].Text += "<br/>" + row.Cells[DCC.IndexOf("UPDATEDDATE")].Text.Trim();
    }

    void grdChangeHistory_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdChangeHistory.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtWorkload_Change_History"] == null)
        {
            loadGridData();
        }
        else
        {
            grdChangeHistory.DataSource = (DataTable)HttpContext.Current.Session["dtWorkload_Change_History"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("FieldChanged")
                && i != DCC.IndexOf("OldValue")
                && i != DCC.IndexOf("NewValue")
                && i != DCC.IndexOf("UPDATEDBY"))
            {
                row.Cells[i].Style["display"] = "none";
            }
        }

        //row.Cells[DCC.IndexOf("FieldChanged")].Style["width"] = "105px";
        row.Cells[DCC.IndexOf("OldValue")].Style["width"] = "350px";
        row.Cells[DCC.IndexOf("NewValue")].Style["width"] = "350px";
        row.Cells[DCC.IndexOf("UPDATEDBY")].Style["width"] = "140px";
    }


	HtmlGenericControl createDivForHtml(string htmlContents)
	{
		HtmlGenericControl div = new HtmlGenericControl("div");
		div.Style["width"] = "345px";
		div.Style["height"] = "50px";
		div.Style["text-align"] = "left";
		div.Style["vertical-align"] = "top";
        div.Style["overflow"] = "auto";
        div.InnerHtml = Server.HtmlDecode(htmlContents);

		return div;
	}

    #region AJAX
    [WebMethod()]
    public static string Delete(string type, string history)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int history_ID = 0;
            int.TryParse(history, out history_ID);

            if (type == "WorkItemTask")
            {
                deleted = Workload.WorkItem_Task_History_Delete(WORKITEM_TASK_HISTORYID: history_ID);
            }
            else
            {
                deleted = Workload.WorkItem_History_Delete(WorkItem_HistoryID: history_ID);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            deleted = false;
            errorMsg = ex.Message;
        }

        result["deleted"] = deleted.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}