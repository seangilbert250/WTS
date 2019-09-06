using System;
using System.Data;
using System.Web.UI.WebControls;

public partial class ReleaseSession_WorkTasks : System.Web.UI.Page
{
    #region Variables
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected int ReleaseSessionID = 0;
    protected string ReleaseSession = string.Empty;
    protected string ViewType = string.Empty;
    protected string SelectedSystems = string.Empty;
    protected string SelectedContracts = string.Empty;
    protected string SelectedAORs = string.Empty;
    private DataColumnCollection DCC;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewWorkItem = this.CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

        lblHeader.Text = this.ReleaseSession + ": " + this.ViewType;

        DataTable dtWorkTasks = LoadData();

        if (dtWorkTasks != null)
        {
            this.DCC = dtWorkTasks.Columns;
            lblHeader.Text += " (" + dtWorkTasks.Rows.Count + ")";
        }

        grdData.DataSource = dtWorkTasks;
        grdData.DataBind();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["ReleaseSessionID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseSessionID"]))
        {
            int.TryParse(Request.QueryString["ReleaseSessionID"], out this.ReleaseSessionID);
        }

        if (Request.QueryString["ReleaseSession"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseSession"]))
        {
            this.ReleaseSession = Request.QueryString["ReleaseSession"];
        }

        if (Request.QueryString["ViewType"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ViewType"]))
        {
            this.ViewType = Request.QueryString["ViewType"];
        }

        if (Request.QueryString["SelectedSystems"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSystems"]))
        {
            this.SelectedSystems = Request.QueryString["SelectedSystems"];
        }

        if (Request.QueryString["SelectedContracts"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedContracts"]))
        {
            this.SelectedContracts = Request.QueryString["SelectedContracts"];
        }

        if (Request.QueryString["SelectedAORs"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAORs"]))
        {
            this.SelectedAORs = Request.QueryString["SelectedAORs"];
        }
    }

    private void InitializeEvents()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;
    }
    #endregion

    #region Data
    private DataTable LoadData()
    {
        DataTable dt = MasterData.ReleaseSessionWorkTaskList_Get(ReleaseSessionID: this.ReleaseSessionID, ViewType: this.ViewType, QFSystem: this.SelectedSystems, QFContract: this.SelectedContracts, QFAOR: this.SelectedAORs);

        return dt;
    }
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatRowDisplay(ref row);

        row.Cells[DCC.IndexOf("Work Task")].Controls.Add(CreateLink("Work Task", Server.HtmlDecode(row.Cells[DCC.IndexOf("Work Task")].Text).Trim(), Server.HtmlDecode(row.Cells[DCC.IndexOf("WORKITEM_TASKID")].Text).Trim(), Server.HtmlDecode(row.Cells[DCC.IndexOf("WORKITEMID")].Text).Trim(), Server.HtmlDecode(row.Cells[DCC.IndexOf("TASK_NUMBER")].Text).Trim()));
    }

    private void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdData.PageIndex = e.NewPageIndex;
    }

    private void FormatHeaderRowDisplay(ref GridViewRow row)
    {
        if (DCC.Contains("WORKITEM_TASKID")) row.Cells[DCC.IndexOf("WORKITEM_TASKID")].Style["display"] = "none";
        if (DCC.Contains("WORKITEMID")) row.Cells[DCC.IndexOf("WORKITEMID")].Style["display"] = "none";
        if (DCC.Contains("TASK_NUMBER")) row.Cells[DCC.IndexOf("TASK_NUMBER")].Style["display"] = "none";

        if (DCC.Contains("TITLE")) row.Cells[DCC.IndexOf("TITLE")].Text = "Title";

        if (DCC.Contains("Work Task")) row.Cells[DCC.IndexOf("Work Task")].Style["width"] = "90px";
        if (DCC.Contains("New In This Session")) row.Cells[DCC.IndexOf("New In This Session")].Style["width"] = "80px";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        if (DCC.Contains("WORKITEM_TASKID")) row.Cells[DCC.IndexOf("WORKITEM_TASKID")].Style["display"] = "none";
        if (DCC.Contains("WORKITEMID")) row.Cells[DCC.IndexOf("WORKITEMID")].Style["display"] = "none";
        if (DCC.Contains("TASK_NUMBER")) row.Cells[DCC.IndexOf("TASK_NUMBER")].Style["display"] = "none";

        if (DCC.Contains("Work Task")) row.Cells[DCC.IndexOf("Work Task")].Style["text-align"] = "center";
        if (DCC.Contains("New In This Session")) row.Cells[DCC.IndexOf("New In This Session")].Style["text-align"] = "center";
    }

    private LinkButton CreateLink(string type, string value, string value2 = "", string value3 = "", string value4 = "")
    {
        LinkButton lb = new LinkButton();

        lb.Text = value;

        switch (type)
        {
            case "Work Task":
                lb.Attributes["onclick"] = string.Format("openWorkTask('{0}','{1}','{2}'); return false;", value2, value3, value4);
                break;
        }

        return lb;
    }
    #endregion
}