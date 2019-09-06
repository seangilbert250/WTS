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

public partial class AOR_Tasks_Add : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected int AORID = 0;
    protected int AORReleaseID = 0;
    protected int SRID = 0;
    protected string Type = string.Empty;
    protected string SubType = string.Empty;
    private DataColumnCollection DCC;
    private DataColumnCollection DCCselected;
    protected string Filters = string.Empty;

    protected int MaxSelections = 0;

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
        if (dt != null)
        {
            this.DCC = dt.Columns;
        }

        grdData.DataSource = dt;
        grdData.DataBind();

        if (this.Type == "Task" || this.Type == "Task List") {
            DataTable dt2 = LoadData2();

            if (dt2 != null)
            {
                this.DCCselected = dt2.Columns;
            }
            selectedData.DataSource = dt2;
            selectedData.DataBind();
        }
    }

    private DataTable LoadData2()
    {
        DataTable dt = new DataTable();
        dt = dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID, SRID: this.SRID, CRID: 0, DeliverableID: 0, Type: this.Type, Filters: null, CRStatus: string.Empty, CRContract: string.Empty, TaskID: string.Empty, GetColumns: true);

        return dt;
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

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.AORReleaseID);
        }

        if (Request.QueryString["SRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SRID"]))
        {
            int.TryParse(Request.QueryString["SRID"], out this.SRID);
        }

        if (Request.QueryString["Type"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Type"]))
        {
            this.Type = Request.QueryString["Type"];
        }

        if (Request.QueryString["Filters"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Filters"]) && !IsPostBack) // we don't load pre-selected filters on postbacks
        {
            this.Filters = Request.QueryString["Filters"];
        }

        if (Request.QueryString["SubType"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SubType"]))
        {
            this.SubType = Request.QueryString["SubType"];

            if (this.SubType == "SelectTask")
            {
                MaxSelections = 1;
            }
        }
    }

    private void InitializeEvents()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;

        selectedData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        selectedData.GridRowDataBound += grdData_GridRowDataBound;
        selectedData.GridPageIndexChanging += grdData_GridPageIndexChanging;

    }
    #endregion

    #region Data
    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

        if (IsPostBack && txtPostBackType.Text == "LoadGrid")
        {
            dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>((dynamic)txtAppliedFilters.Text);

            dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID, SRID: this.SRID, CRID: 0, DeliverableID: 0, Type: this.Type, Filters: fields, CRStatus: string.Empty, CRContract: string.Empty, TaskID: txtTaskSearch.Text);
            txtPostBackType.Text = string.Empty;
        }
        else if (IsPostBack && Session["dtAORAddTask"] != null)
        {
            dt = (DataTable)Session["dtAORAddTask"];
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(this.Filters))
            {
                txtAppliedFilters.Text = this.Filters;
                dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>((dynamic)this.Filters);

                dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID, SRID: this.SRID, CRID: 0, DeliverableID: 0, Type: this.Type, Filters: fields, CRStatus: string.Empty, CRContract: string.Empty, TaskID: string.Empty);
            }
            else
            {
                dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID, SRID: this.SRID, CRID: 0, DeliverableID: 0, Type: this.Type, Filters: null, CRStatus: string.Empty, CRContract: string.Empty, TaskID: string.Empty);
            }
        }

            Session["dtAORAddTask"] = dt;

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

        DataColumnCollection DCCcurrent;

		//Find out if we are binding the grdData or selectedData
        if (row.Cells.Count == DCC.Count)
        {
            DCCcurrent = DCC;
        }
        else
        {
            DCCcurrent = DCCselected;
        }

        string itemId = row.Cells[DCC.IndexOf("Task_ID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }
        else
        {
            if (DCCcurrent.Contains("X") && DCCcurrent.Contains("Work Task"))
            {
                if (this.CanEditAOR && this.Type == "MoveWorkTask")
                {
                    row.Cells[DCCcurrent.IndexOf("X")].Style["text-align"] = "center";
                    row.Cells[DCCcurrent.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCCcurrent.IndexOf("Work Task")].Text));
                }
                else if (this.CanEditAOR || (this.Type == "MoveSubTask" && this.CanEditWorkItem))
                {
                    row.Cells[DCCcurrent.IndexOf("X")].Style["text-align"] = "center";
                    row.Cells[DCCcurrent.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCCcurrent.IndexOf("Work Task")].Text));
                }
                if (this.CanViewWorkItem)
                {
                    row.Cells[DCCcurrent.IndexOf("Work Task")].Style["text-align"] = "center";
                    row.Cells[DCCcurrent.IndexOf("Work Task")].Controls.Add(CreateLink(row.Cells[DCCcurrent.IndexOf("Task_ID")].Text, row.Cells[DCCcurrent.IndexOf("Work Task")].Text));
                }
            }
        }
    }

    private void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdData.PageIndex = e.NewPageIndex;
    }

    private void FormatHeaderRowDisplay(ref GridViewRow row)
    {
        DataColumnCollection DCCcurrent;
		
		//Find out if we are binding the grdData or selectedData
        if (row.Cells.Count == DCC.Count)
        {
            DCCcurrent = DCC;
        }
        else
        {
            DCCcurrent = DCCselected;
        }

        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCCcurrent[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
        }

        if (DCCcurrent.Contains("X"))
        {
            row.Cells[DCCcurrent.IndexOf("X")].Text = "";
            row.Cells[DCCcurrent.IndexOf("X")].Style["width"] = "35px";
        }

        if (this.Type == "Task List" && DCCcurrent.Contains("X")) row.Cells[DCCcurrent.IndexOf("X")].Style["display"] = "none";

        if (DCCcurrent.Contains("Work Task")) row.Cells[DCCcurrent.IndexOf("Work Task")].Style["width"] = "75px";
        if (DCCcurrent.Contains("System(Task)")) row.Cells[DCCcurrent.IndexOf("System(Task)")].Style["width"] = "100px";
        if (DCCcurrent.Contains("Product Version")) row.Cells[DCCcurrent.IndexOf("Product Version")].Style["width"] = "60px";
        if (DCCcurrent.Contains("Production Status")) row.Cells[DCCcurrent.IndexOf("Production Status")].Style["width"] = "75px";
        if (DCCcurrent.Contains("Priority")) row.Cells[DCCcurrent.IndexOf("Priority")].Style["width"] = "55px";
        if (DCCcurrent.Contains("SR Number")) row.Cells[DCCcurrent.IndexOf("SR Number")].Style["width"] = "55px";
        if (DCCcurrent.Contains("Assigned To")) row.Cells[DCCcurrent.IndexOf("Assigned To")].Style["width"] = "110px";
        if (DCCcurrent.Contains("Primary Resource")) row.Cells[DCCcurrent.IndexOf("Primary Resource")].Style["width"] = "110px";
        if (DCCcurrent.Contains("Secondary Tech. Resource")) row.Cells[DCCcurrent.IndexOf("Secondary Tech. Resource")].Style["width"] = "110px";
        if (DCCcurrent.Contains("Primary Bus. Resource")) row.Cells[DCCcurrent.IndexOf("Primary Bus. Resource")].Style["width"] = "110px";
        if (DCCcurrent.Contains("Secondary Bus. Resource")) row.Cells[DCCcurrent.IndexOf("Secondary Bus. Resource")].Style["width"] = "110px";
        if (DCCcurrent.Contains("Status")) row.Cells[DCCcurrent.IndexOf("Status")].Style["width"] = "100px";
        if (DCCcurrent.Contains("Percent Complete")) row.Cells[DCCcurrent.IndexOf("Percent Complete")].Style["width"] = "65px";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        DataColumnCollection DCCcurrent;
		
		//Find out if we are binding the grdData or selectedData
        if (row.Cells.Count == DCC.Count)
        {
            DCCcurrent = DCC;
        }
        else
        {
            DCCcurrent = DCCselected;
        }
        
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCCcurrent[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }
        if (this.Type == "Task List" && DCCcurrent.Contains("X")) row.Cells[DCCcurrent.IndexOf("X")].Style["display"] = "none";

        if (DCCcurrent.Contains("Product Version")) row.Cells[DCCcurrent.IndexOf("Product Version")].Style["text-align"] = "center";
    }

    private CheckBox CreateCheckBox(string value)
    {
        CheckBox chk = new CheckBox();
        if (this.Type == "MoveSubTask") {
            chk.Attributes["onchange"] = "chkTask_change(this);";  
        }
        else {
            chk.Attributes["onchange"] = "input_change(this);";
        }
        chk.Attributes.Add("task_id", value);

        return chk;
    }

    private LinkButton CreateLink(string workItem_ID, string workItem)
    {
        LinkButton lb = new LinkButton();
        string workItemID = workItem_ID;
        string taskNumber = string.Empty;
        string taskID = string.Empty;
        string blnSubTask = "0";

        lb.Text = workItem;
        if (workItem.Contains(" - "))
        {
            string[] arrWorkItem = workItem.Split('-');

            workItemID = arrWorkItem[0].Trim();
            taskNumber = arrWorkItem[1].Trim();
            taskID = workItem_ID;
            blnSubTask = "1";
            lb.Attributes["onclick"] = string.Format("openTask('{0}', '{1}', '{2}', {3}); return false;", workItemID, taskNumber, taskID, blnSubTask);
        }
        else
        {

            lb.Attributes["onclick"] = string.Format("openTask('{0}', '{1}', '{2}', {3}); return false;", workItemID, taskNumber, taskID, blnSubTask);
        }
            return lb;
    }
    #endregion
}