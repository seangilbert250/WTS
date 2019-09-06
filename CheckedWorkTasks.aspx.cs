﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CheckedWorkTasks : System.Web.UI.Page
{
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected int AORID = 431;
    protected int AORReleaseID = 621;
    protected int SRID = 0;
    protected string Type = string.Empty;
    protected string SubType = string.Empty;
    private DataColumnCollection DCC;
    protected string Filters = string.Empty;

    protected int MaxSelections = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dt = LoadData();
        if (dt != null) this.DCC = dt.Columns;

        grdData.DataSource = dt;
        grdData.DataBind();


    }



    private void InitializeEvents()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;

        //selectedData.GridHeaderRowDataBound += selectedData_GridHeaderRowDataBound;
        //selectedData.GridRowDataBound += selectedData_GridRowDataBound;
        //selectedData.GridPageIndexChanging += selectedData_GridPageIndexChanging;

    }

    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

        //if (IsPostBack && txtPostBackType.Text == "LoadGrid")
        //{
        //    dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>((dynamic)txtAppliedFilters.Text);

        //    dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID, SRID: this.SRID, CRID: 0, DeliverableID: 0, Type: this.Type, Filters: fields, CRStatus: string.Empty, CRContract: string.Empty, TaskID: txtTaskSearch.Text);
        //    txtPostBackType.Text = string.Empty;
        //}
        //else if (IsPostBack && Session["dtAORAddTask"] != null)
        //{
        //    dt = (DataTable)Session["dtAORAddTask"];
        //}
        //else
        //{
        //    if (!string.IsNullOrWhiteSpace(this.Filters))
        //    {
        //        txtAppliedFilters.Text = this.Filters;
        //        dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>((dynamic)this.Filters);

        //        dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID, SRID: this.SRID, CRID: 0, DeliverableID: 0, Type: this.Type, Filters: fields, CRStatus: string.Empty, CRContract: string.Empty, TaskID: string.Empty);
        //    }
        //    else
        //    {
        //        dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID, SRID: this.SRID, CRID: 0, DeliverableID: 0, Type: this.Type, Filters: null, CRStatus: string.Empty, CRContract: string.Empty, TaskID: string.Empty);
        //    }
        //}

        dynamic fields = null;

        dt = dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID, SRID: this.SRID, CRID: 0, DeliverableID: 0, Type: this.Type, Filters: null, CRStatus: string.Empty, CRContract: string.Empty, TaskID: string.Empty, GetColumns: true);


        //Session["dtAORAddTask"] = dt;

        return dt;
    }

    private void FormatHeaderRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
        }

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = "35px";
        }

        if (DCC.Contains("Work Task")) row.Cells[DCC.IndexOf("Work Task")].Style["width"] = "75px";
        if (DCC.Contains("System(Task)")) row.Cells[DCC.IndexOf("System(Task)")].Style["width"] = "100px";
        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["width"] = "60px";
        if (DCC.Contains("Production Status")) row.Cells[DCC.IndexOf("Production Status")].Style["width"] = "75px";
        if (DCC.Contains("Priority")) row.Cells[DCC.IndexOf("Priority")].Style["width"] = "55px";
        if (DCC.Contains("SR Number")) row.Cells[DCC.IndexOf("SR Number")].Style["width"] = "55px";
        if (DCC.Contains("Assigned To")) row.Cells[DCC.IndexOf("Assigned To")].Style["width"] = "110px";
        if (DCC.Contains("Primary Resource")) row.Cells[DCC.IndexOf("Primary Resource")].Style["width"] = "110px";
        if (DCC.Contains("Secondary Tech. Resource")) row.Cells[DCC.IndexOf("Secondary Tech. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Primary Bus. Resource")) row.Cells[DCC.IndexOf("Primary Bus. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Secondary Bus. Resource")) row.Cells[DCC.IndexOf("Secondary Bus. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Status")) row.Cells[DCC.IndexOf("Status")].Style["width"] = "100px";
        if (DCC.Contains("Percent Complete")) row.Cells[DCC.IndexOf("Percent Complete")].Style["width"] = "65px";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }

        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["text-align"] = "center";
    }


    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatRowDisplay(ref row);

        if (DCC.Contains("X") && DCC.Contains("Work Task"))
        {
            if (this.CanEditAOR && this.Type == "MoveWorkTask")
            {
                row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("Work Task")].Text));
            }
            else if (this.CanEditAOR || (this.Type == "MoveSubTask" && this.CanEditWorkItem))
            {
                row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("Work Task")].Text));
            }
            if (this.CanViewWorkItem)
            {
                row.Cells[DCC.IndexOf("Work Task")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Work Task")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("Task_ID")].Text, row.Cells[DCC.IndexOf("Work Task")].Text));
            }
        }
    }

    private void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdData.PageIndex = e.NewPageIndex;
    }


    private CheckBox CreateCheckBox(string value)
    {
        CheckBox chk = new CheckBox();
        if (this.Type == "MoveSubTask")
        {
            chk.Attributes["onchange"] = "chkTask_change(this);";
        }
        else
        {
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




}