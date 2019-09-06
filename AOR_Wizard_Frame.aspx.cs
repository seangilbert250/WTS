﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_Wizard_Frame : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected string Type = string.Empty;
    protected int AORID = 0;
    protected int WorkloadTypeID = 0;
    private DataColumnCollection DCC;
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

        if (Request.QueryString["Type"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Type"]))
        {
            this.Type = Request.QueryString["Type"];
        }

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.AORID);
        }

        if (Request.QueryString["WorkloadTypeID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["WorkloadTypeID"]))
        {
            int.TryParse(Request.QueryString["WorkloadTypeID"], out this.WorkloadTypeID);
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

        switch (this.Type)
        {
            case "CR":
                if (IsPostBack && txtPostBackType.Text == "LoadGrid")
                {
                    dt = AOR.AORAddList_Get(AORID: 0, AORReleaseID: 0, SRID: 0, CRID: 0, DeliverableID: 0, Type: "AORWizardCR", Filters: null, CRStatus: txtCRStatusQF.Text, CRContract: txtCRContractQF.Text, TaskID: string.Empty);
                    txtPostBackType.Text = string.Empty;
                }
                else if (IsPostBack && Session["dtAORWizardCR"] != null)
                {
                    dt = (DataTable)Session["dtAORWizardCR"];
                }
                else
                {
                    dt = AOR.AORAddList_Get(AORID: 0, AORReleaseID: 0, SRID: 0, CRID: 0, DeliverableID: 0, Type: "AORWizardCR", Filters: null, CRStatus: string.Empty, CRContract: string.Empty, TaskID: string.Empty);
                }
                grdData.PageSize = 500;

                Session["dtAORWizardCR"] = dt;
                break;
            case "Task":
                if (IsPostBack && txtPostBackType.Text == "LoadGrid")
                {
                    dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>((dynamic)txtAppliedFilters.Text);

                    dt = AOR.AORAddList_Get(AORID: AORID, AORReleaseID: 0, SRID: 0, CRID: 0, DeliverableID: 0, Type: "AORWizardTask", Filters: fields, CRStatus: string.Empty, CRContract: string.Empty, TaskID: txtTaskSearch.Text, AORWorkTypeID: WorkloadTypeID);
                    txtPostBackType.Text = string.Empty;
                }
                else if (IsPostBack && Session["dtAORWizardTask"] != null)
                {
                    dt = (DataTable)Session["dtAORWizardTask"];
                }
                else
                {
                    dt = AOR.AORAddList_Get(AORID: AORID, AORReleaseID: 0, SRID: 0, CRID: 0, DeliverableID: 0, Type: "AORWizardTask", Filters: null, CRStatus: string.Empty, CRContract: string.Empty, TaskID: string.Empty, AORWorkTypeID: WorkloadTypeID);
                }

                Session["dtAORWizardTask"] = dt;
                break;
        }

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

        if (DCC.Contains("X") && DCC.Contains("CR_ID"))
        {
            if (this.CanEditAOR)
            {
                row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("CR_ID")].Text));
            }
        }

        if (DCC.Contains("X") && DCC.Contains("Work Task"))
        {
            if (this.CanEditAOR)
            {
                row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("Work Task")].Text));
            }

            if (this.CanViewWorkItem)
            {
                row.Cells[DCC.IndexOf("Work Task")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Work Task")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("Work Task")].Text));
            }
        }
    }

    private void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdData.PageIndex = e.NewPageIndex;
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

        if (DCC.Contains("CR Customer Title")) row.Cells[DCC.IndexOf("CR Customer Title")].Style["width"] = "300px";
        if (DCC.Contains("CR Contract")) row.Cells[DCC.IndexOf("CR Contract")].Style["width"] = "90px";
        if (DCC.Contains("Related Release")) row.Cells[DCC.IndexOf("Related Release")].Style["width"] = "100px";
        if (DCC.Contains("Criticality")) row.Cells[DCC.IndexOf("Criticality")].Style["width"] = "60px";
        if (DCC.Contains("CR Coordination")) row.Cells[DCC.IndexOf("CR Coordination")].Style["width"] = "85px";
        if (DCC.Contains("Work Task")) row.Cells[DCC.IndexOf("Work Task")].Style["width"] = "55px";
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
        if (DCC.Contains("CR Customer Title")) row.Cells[DCC.IndexOf("CR Customer Title")].Text = Uri.UnescapeDataString(row.Cells[DCC.IndexOf("CR Customer Title")].Text);
        if (DCC.Contains("CR Internal Title")) row.Cells[DCC.IndexOf("CR Internal Title")].Text = Uri.UnescapeDataString(row.Cells[DCC.IndexOf("CR Internal Title")].Text);

        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }

        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["text-align"] = "center";
    }

    private CheckBox CreateCheckBox(string value)
    {
        CheckBox chk = new CheckBox();

        chk.Attributes["onchange"] = "input_change(this);";

        switch (this.Type)
        {
            case "CR":
                chk.Attributes.Add("cr_id", value);
                break;
            case "Task":
                chk.Attributes.Add("task_id", value);
                break;
        }

        return chk;
    }

    private LinkButton CreateLink(string taskID)
    {
        LinkButton lb = new LinkButton();

        lb.Text = taskID;
        lb.Attributes["onclick"] = string.Format("openTask('{0}'); return false;", taskID);

        return lb;
    }
    #endregion
}