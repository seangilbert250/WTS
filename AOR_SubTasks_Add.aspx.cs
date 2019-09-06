using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_SubTasks_Add : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected int AORID = 0;
    protected int SRID = 0;
    protected string Type = string.Empty;
    protected string SubType = string.Empty;
    private DataColumnCollection DCC;
    protected string SelectedStatuses = "";
    protected string SelectedSystems = "";

    protected int TaskID = 0;

    protected int MaxSelections = 0;

    private Dictionary<string, List<string>> _ColumnFormats = null;

    private Dictionary<string, List<string>> ColumnFormats
    {
        get
        {
            if (_ColumnFormats == null)
            {
                _ColumnFormats = new Dictionary<string, List<string>>();

                string hidden = "WORKITEM_TASKID,TASK_NUMBER,SORT_ORDER,AssignedToRankID,PRIORITYID,PRIORITYIDSORTED,DESCRIPTION,ASSIGNEDRESOURCEID," +
                    "PrimaryResource," +
                "PRIMARYRESOURCEID,SECONDARYRESOURCEID,SecondaryResource,PRIMARYBUSRESOURCEID,PrimaryBusResource,SECONDARYBUSRESOURCEID," +
                "SecondaryBusResource,SubmittedByID,SubmittedBy,ESTIMATEDSTARTDATE,ACTUALSTARTDATE,EstimatedEffortID,PLANNEDHOURS,ActualEffortID," +
                "ACTUALHOURS,ACTUALENDDATE,WorkTypeID,WorkType,PDDTDR_PHASEID,STATUSID,SRNumber,CREATEDBY,CREATEDDATE,UPDATEDBY,UPDATEDDATE," +
                "ReOpenedCount,WTS_SYSTEMID,Y";

                string centered = "X,WORKITEMID,BusinessRank,AssignedToRank,PRIORITY,Version,COMPLETIONPERCENT";

                string width = "X=35,WORKITEMID=75,BusinessRank=75,AssignedToRank=75,PRIORITY=75,TITLE=250,COMPLETIONPERCENT=50,STATUS=75,AssignedResource=75,Version=50";

                string rename = "WORKITEMID=SubTask,BusinessRank=Customer Rank,AssignedToRank=Assigned To Rank,PRIORITY=Priority,TITLE=Title,AssignedResource=Assigned To,COMPLETIONPERCENT=% Comp,STATUS=Status";

                string allowWrap = "BusinessRank,AssignedToRank,TITLE";

                _ColumnFormats.Add("hidden", new List<string>(hidden.Split(',')));
                _ColumnFormats.Add("centered", new List<string>(centered.Split(',')));
                _ColumnFormats.Add("width", new List<string>(width.Split(',')));
                _ColumnFormats.Add("rename", new List<string>(rename.Split(',')));
                _ColumnFormats.Add("allowwrap", new List<string>(allowWrap.Split(',')));
            }

            return _ColumnFormats;
        }
    }

    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewWorkItem = this.CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

        DataTable dt = LoadData();
        if (dt != null) this.DCC = dt.Columns;

        grdData.DataSource = dt;
        grdData.DataBind();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
            || Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
        {
            this.MyData = true;
        }
        else
        {
            this.MyData = false;
        }

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.AORID);
        }

        if (Request.QueryString["SRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SRID"]))
        {
            int.TryParse(Request.QueryString["SRID"], out this.SRID);
        }

        if (Request.QueryString["TaskID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["TaskID"]))
        {
            int.TryParse(Request.QueryString["TaskID"], out this.TaskID);
        }

        if (Request.QueryString["Type"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Type"]))
        {
            this.Type = Request.QueryString["Type"];
        }

        if (Request.QueryString["SubType"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SubType"]))
        {
            this.SubType = Request.QueryString["SubType"];

            if (this.SubType == "SelectSubTask")
            {
                MaxSelections = 1;
            }
        }

        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"]))
        {
            this.SelectedStatuses = Request.QueryString["SelectedStatuses"];
        }

        if (Request.QueryString["SelectedSystems"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSystems"]))
        {
            this.SelectedSystems = Request.QueryString["SelectedSystems"];
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
        DataTable dt = new DataTable();

        if (IsPostBack && Session["dtAORAddSubTask"] != null)
        {
            dt = (DataTable)Session["dtAORAddSubTask"];
        }
        else
        {
            dt = WorkloadItem.WorkItem_GetTaskList(workItemID: TaskID, showArchived: 0, showBacklog: false, statusList: SelectedStatuses, systemList: SelectedSystems);
        }
                
        // apply status filters

        Session["dtAORAddSubTask"] = dt;

        return dt;
    }
    #endregion

    #region Grid
    private void ApplyFormatsToRow(GridViewRow row, bool header)
    {
        List<string> hidden = ColumnFormats["hidden"];
        List<string> centered = ColumnFormats["centered"];
        List<string> width = ColumnFormats["width"];
        List<string> rename = ColumnFormats["rename"];
        List<string> allowWrap = ColumnFormats["allowwrap"];

        foreach (string col in hidden)
        {
            if (DCC.Contains(col)) row.Cells[DCC.IndexOf(col)].Style["display"] = "none";
        }

        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (centered.Contains(DCC[i].ColumnName))
            {
                row.Cells[i].Style["text-align"] = "center";
            }
            else
            {
                row.Cells[i].Style["text-align"] = "left";
            }
        }

        foreach (string col in centered)
        {
            if (DCC.Contains(col)) row.Cells[DCC.IndexOf(col)].Style["text-align"] = "center";
        }

        foreach (string col in width)
        {
            string[] tokens = col.Split('=');
            
            if (DCC.Contains(tokens[0])) row.Cells[DCC.IndexOf(tokens[0])].Style["width"] = tokens[1] + "px";
        }

        for (int i = 0; i < row.Cells.Count; i++)
        {
            row.Cells[i].Style["white-space"] = allowWrap.Contains(DCC[i].ColumnName) ? "normal" : "nowrap";          
        }

        if (header)
        {
            foreach (string col in rename)
            {
                string[] tokens = col.Split('=');

                if (DCC.Contains(tokens[0])) row.Cells[DCC.IndexOf(tokens[0])].Text = tokens[1];
            }
        }
    }

    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        ApplyFormatsToRow(row, true);
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        ApplyFormatsToRow(row, false);

        if (DCC.Contains("X") && DCC.Contains("TASK_NUMBER"))
        {
            if (this.Type == "SubTask" && this.SubType == "SelectSubTask")
            {
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("WORKITEM_TASKID")].Text, row.Cells[DCC.IndexOf("TASK_NUMBER")].Text));
            }            
        }

        if (DCC.Contains("WORKITEMID"))
        {
            string taskID = row.Cells[DCC.IndexOf("WORKITEMID")].Text;
            string taskNumber = row.Cells[DCC.IndexOf("TASK_NUMBER")].Text;
            string subTaskID = row.Cells[DCC.IndexOf("WORKITEM_TASKID")].Text;
            
            LinkButton lb = CreateLink(taskID + "-" + taskNumber, subTaskID, taskID);
            row.Cells[DCC.IndexOf("WORKITEMID")].Controls.Clear();
            row.Cells[DCC.IndexOf("WORKITEMID")].Controls.Add(lb);
        }
    }

    private void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdData.PageIndex = e.NewPageIndex;
    }

    private CheckBox CreateCheckBox(string value, string value2)
    {
        CheckBox chk = new CheckBox();
        chk.Attributes["onchange"] = "input_change(this);";
        chk.Attributes.Add("subtask_id", value);
        chk.Attributes.Add("subtask_number", value2);

        return chk;
    }

    private LinkButton CreateLink(string text, string subTaskID, string workItemID)
    {
        LinkButton lb = new LinkButton();
        
        lb.Text = text;
        lb.Attributes["onclick"] = string.Format("openSubTask('{0}', '{1}'); return false;", subTaskID, workItemID);

        return lb;
    }
    #endregion
}