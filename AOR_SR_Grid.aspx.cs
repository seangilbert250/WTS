﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_SR_Grid : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected int CRID = 0;
    protected int CurrentLevel = 1;
    protected string Filter = string.Empty;
    private XmlDocument Levels = new XmlDocument();
    protected DataColumnCollection DCC;
    protected int LevelCount = 0;
    protected int GridPageIndex = 0;
    protected int RowCount = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadData();
        ReadQueryString();
        InitializeEvents();

        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewWorkItem = this.CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

        DataTable dt = LoadData();

        if (dt != null)
        {
            this.DCC = dt.Columns;
            this.RowCount = dt.Rows.Count;
        }

        grdData.DataSource = dt;

        if (!Page.IsPostBack && this.GridPageIndex > 0 && this.GridPageIndex < ((decimal)dt.Rows.Count / (decimal)25)) grdData.PageIndex = this.GridPageIndex;

        grdData.DataBind();
    }

    private void ReadData()
    {
        XmlDocument nDoc = new XmlDocument();
        string nData = string.Empty;

        nData = "<crosswalkparameters><level>";
        nData += "<breakout><column>SR #</column><sort>Descending</sort></breakout>";
        nData += "<breakout><column>SUBMITTED BY</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>SUBMITTED DATE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>KEYWORDS</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>SR WEBSYSTEM</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>STATUS</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>SR TYPE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>PRIORITY</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>LCMB</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>ITI</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>SR ITI POC</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>DESCRIPTION</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>LAST REPLY</column><sort>Ascending</sort></breakout>";
        nData += "</level><level>";
        nData += "<breakout><column>PRIMARY TASK</column><sort>Descending</sort></breakout>";
        nData += "<breakout><column>PRIMARY TASK TITLE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>SYSTEM(TASK)</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>PRODUCT VERSION</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>PRODUCTION STATUS</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>TASK PRIORITY</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>ASSIGNED TO</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>PRIMARY RESOURCE</column><sort>Ascending</sort></breakout>";
        //nData += "<breakout><column>SECONDARY TECH. RESOURCE</column><sort>Ascending</sort></breakout>";
        //nData += "<breakout><column>PRIMARY BUS. RESOURCE</column><sort>Ascending</sort></breakout>";
        //nData += "<breakout><column>SECONDARY BUS. RESOURCE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>TASK STATUS</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>PERCENT COMPLETE</column><sort>Ascending</sort></breakout>";
        nData += "</level></crosswalkparameters>";

        nDoc.InnerXml = nData;

        this.Levels = nDoc;
        this.LevelCount = this.Levels.SelectNodes("crosswalkparameters/level").Count;
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

        if (Request.QueryString["CRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CRID"]))
        {
            int.TryParse(Request.QueryString["CRID"], out this.CRID);
        }

        if (Request.QueryString["CurrentLevel"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"]))
        {
            int.TryParse(Request.QueryString["CurrentLevel"], out this.CurrentLevel);
        }

        if (Request.QueryString["Filter"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Filter"]))
        {
            this.Filter = Uri.UnescapeDataString(Request.QueryString["Filter"]);
        }

        if (Request.QueryString["GridPageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndex"]))
        {
            int.TryParse(Request.QueryString["GridPageIndex"], out this.GridPageIndex);
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

        if (IsPostBack && this.CurrentLevel == 1 && Session["dtSR" + this.CurrentLevel] != null)
        {
            dt = (DataTable)Session["dtSR" + this.CurrentLevel];
        }
        else
        {
            XmlDocument docLevel = new XmlDocument();
            XmlElement rootLevel = (XmlElement)docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
            XmlNode nodeLevel = this.Levels.SelectNodes("crosswalkparameters/level")[this.CurrentLevel - 1];
            XmlNode nodeImport = docLevel.ImportNode(nodeLevel, true);
            rootLevel.AppendChild(nodeImport);

            XmlDocument docFilters = new XmlDocument();
            XmlElement rootFilters = (XmlElement)docFilters.AppendChild(docFilters.CreateElement("filters"));

            if (this.Filter != string.Empty)
            {
                string[] arrFilter = this.Filter.Split('|');

                for (int j = 0; j < arrFilter.Length; j++)
                {
                    XmlElement filter = docFilters.CreateElement("filter");
                    XmlElement field = docFilters.CreateElement("field");
                    XmlElement value = docFilters.CreateElement("id");
                    string[] arrValues = arrFilter[j].Split('=');

                    field.InnerText = arrValues[0].ToString();
                    value.InnerText = (arrValues[1].ToString().Trim() == "" ? "0" : arrValues[1].ToString().Trim());

                    filter.AppendChild(field);
                    filter.AppendChild(value);
                    rootFilters.AppendChild(filter);
                }
            }

            dt = AOR.AOR_CR_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, AORID: 0, AORReleaseID: 0, CRID: this.CRID, CRStatus: string.Empty, CRRelatedRel: string.Empty, SRStatus: string.Empty, CRContract: string.Empty);

            if (dt.Columns.Contains("SR #"))
            {
                dt.DefaultView.RowFilter = "[SR #] IS NOT NULL";
                dt = dt.DefaultView.ToTable();
            }
            
            Session["dtSR" + this.CurrentLevel] = dt;
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

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage());

            if (DCC.Contains("TaskCount"))
            {
                HtmlGenericControl spn = new HtmlGenericControl("span");

                spn.InnerHtml = "&nbsp;(" + row.Cells[DCC.IndexOf("TaskCount")].Text.Replace("&nbsp;", "0") + ")";
                row.Cells[DCC.IndexOf("X")].Controls.Add(spn);
            }
        }
        if (DCC.Contains("SR #"))
        {
            row.Attributes.Add("sr_id", row.Cells[DCC.IndexOf("SR #")].Text);
        }
        if (DCC.Contains("Primary Task"))
        {
            row.Attributes.Add("task_id", row.Cells[DCC.IndexOf("Primary Task")].Text);
        }

        if (DCC.Contains("Description"))
        {
            string txtDescription = Server.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("Description")].Text)).Trim();

            if (txtDescription.Length > 125)
            {
                row.Cells[DCC.IndexOf("Description")].Controls.Add(CreateTextLink(txtDescription, 125));
            }
            else
            {
                row.Cells[DCC.IndexOf("Description")].Text = txtDescription;
            }
        }

        if (DCC.Contains("Last Reply"))
        {
            string txtLastReply = Server.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("Last Reply")].Text)).Trim();

            if (txtLastReply.Length > 90)
            {
                row.Cells[DCC.IndexOf("Last Reply")].Controls.Add(CreateTextLink(txtLastReply, 90));
            }
            else
            {
                row.Cells[DCC.IndexOf("Last Reply")].Text = txtLastReply;
            }
        }

        if (DCC.Contains("Primary Task") && this.CanViewWorkItem && row.Cells[DCC.IndexOf("Primary Task")].Text != "&nbsp;")
        {
            row.Cells[DCC.IndexOf("Primary Task")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("Primary Task")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("Primary Task")].Text));
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

        if (DCC.Contains("TaskCount")) row.Cells[DCC.IndexOf("TaskCount")].Style["display"] = "none";

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = DCC.Contains("TaskCount") ? "45px" : "15px";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("SR #")) row.Cells[DCC.IndexOf("SR #")].Style["width"] = "40px";
        if (DCC.Contains("Submitted By")) row.Cells[DCC.IndexOf("Submitted By")].Style["width"] = "120px";
        if (DCC.Contains("Submitted Date")) row.Cells[DCC.IndexOf("Submitted Date")].Style["width"] = "70px";
        if (DCC.Contains("Keywords")) row.Cells[DCC.IndexOf("Keywords")].Style["width"] = "110px";
        if (DCC.Contains("SR Websystem")) row.Cells[DCC.IndexOf("SR Websystem")].Style["width"] = "90px";
        if (DCC.Contains("Status")) row.Cells[DCC.IndexOf("Status")].Style["width"] = "120px";
        if (DCC.Contains("SR Type")) row.Cells[DCC.IndexOf("SR Type")].Style["width"] = "55px";
        if (DCC.Contains("Priority")) row.Cells[DCC.IndexOf("Priority")].Style["width"] = "55px";
        if (DCC.Contains("LCMB")) row.Cells[DCC.IndexOf("LCMB")].Style["width"] = "40px";
        if (DCC.Contains("ITI")) row.Cells[DCC.IndexOf("ITI")].Style["width"] = "35px";
        if (DCC.Contains("SR ITI POC")) row.Cells[DCC.IndexOf("SR ITI POC")].Style["width"] = "120px";
        if (DCC.Contains("Description")) row.Cells[DCC.IndexOf("Description")].Style["width"] = "400px";
        if (DCC.Contains("Last Reply")) row.Cells[DCC.IndexOf("Last Reply")].Style["width"] = "230px";
        if (DCC.Contains("Last Reply")) row.Cells[DCC.IndexOf("Last Reply")].Style["width"] = "300px";
        if (DCC.Contains("Primary Task")) row.Cells[DCC.IndexOf("Primary Task")].Style["width"] = "55px";
        if (DCC.Contains("Primary Task Title")) row.Cells[DCC.IndexOf("Primary Task Title")].Style["width"] = "435px";
        if (DCC.Contains("System(Task)")) row.Cells[DCC.IndexOf("System(Task)")].Style["width"] = "100px";
        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["width"] = "60px";
        if (DCC.Contains("Production Status")) row.Cells[DCC.IndexOf("Production Status")].Style["width"] = "75px";
        if (DCC.Contains("Task Priority")) row.Cells[DCC.IndexOf("Task Priority")].Style["width"] = "55px";
        if (DCC.Contains("Assigned To")) row.Cells[DCC.IndexOf("Assigned To")].Style["width"] = "110px";
        if (DCC.Contains("Primary Resource")) row.Cells[DCC.IndexOf("Primary Resource")].Style["width"] = "110px";
        if (DCC.Contains("Secondary Tech. Resource")) row.Cells[DCC.IndexOf("Secondary Tech. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Primary Bus. Resource")) row.Cells[DCC.IndexOf("Primary Bus. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Secondary Bus. Resource")) row.Cells[DCC.IndexOf("Secondary Bus. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Task Status")) row.Cells[DCC.IndexOf("Task Status")].Style["width"] = "100px";
        if (DCC.Contains("Percent Complete")) row.Cells[DCC.IndexOf("Percent Complete")].Style["width"] = "65px";

        if (DCC.Contains("Z")) row.Cells[DCC.IndexOf("Z")].Text = "";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        if (DCC.Contains("Keywords")) row.Cells[DCC.IndexOf("Keywords")].Text = Uri.UnescapeDataString(row.Cells[DCC.IndexOf("Keywords")].Text);

        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }

        if (DCC.Contains("TaskCount")) row.Cells[DCC.IndexOf("TaskCount")].Style["display"] = "none";

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Style["width"] = "15px";
            row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("Submitted Date")) row.Cells[DCC.IndexOf("Submitted Date")].Style["text-align"] = "center";
        if (DCC.Contains("LCMB")) row.Cells[DCC.IndexOf("LCMB")].Style["text-align"] = "center";
        if (DCC.Contains("ITI")) row.Cells[DCC.IndexOf("ITI")].Style["text-align"] = "center";
        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["text-align"] = "center";
    }

    private Image CreateImage()
    {
        Image img = new Image();

        if (this.CurrentLevel != this.LevelCount)
        {
            img.Attributes["src"] = "Images/Icons/add_blue.png";
            img.Attributes["title"] = "Expand";
            img.Attributes["alt"] = "Expand";
            img.Attributes["onclick"] = "displayNextRow(this);";
        }

        img.Attributes["height"] = "12";
        img.Attributes["width"] = "12";
        img.Style["cursor"] = "pointer";

        return img;
    }

    private LinkButton CreateTextLink(string txt, int sub)
    {
        LinkButton lb = new LinkButton();

        lb.Text = txt.Substring(0, sub) + "...";
        lb.ToolTip = txt;
        lb.Attributes["onclick"] = string.Format("showText('{0}'); return false;", Uri.EscapeDataString(txt));

        return lb;
    }

    private LinkButton CreateLink(string TaskNumber)
    {
        LinkButton lb = new LinkButton();

        lb.Text = TaskNumber;
        lb.Attributes["onclick"] = string.Format("openTask({0}); return false;", TaskNumber);

        return lb;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string DeleteTask(string task)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int Task_ID = 0;
            int.TryParse(task, out Task_ID);

            deleted = AOR.AORSRTask_Delete(TaskID: Task_ID);
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