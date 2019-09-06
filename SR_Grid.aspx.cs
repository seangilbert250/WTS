﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;
using Aspose.Cells;
using Newtonsoft.Json;

public partial class SR_Grid : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditSR = false;
    protected bool CanViewSR = false;
    protected bool _export = false;
    protected string[] _selectedSubmittedBy = new string[0];
    protected string[] _selectedStatuses = new string[0];
    protected string[] _selectedReasonings = new string[0];
    protected string[] _selectedSystems = new string[0];
    protected int _goToSRID = 0;
    protected string _srWorkloadPriority = string.Empty;
    protected DataTable dtStatus;
    protected DataTable dtType;
    protected DataTable dtPriority;
    protected DataTable dtSRRank;
    protected DataColumnCollection DCC;
    protected int GridPageIndex = 0;
    protected int TotalCount = 0;
    protected bool searchMode = false;

    protected GridCols columnData = new GridCols();

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();        
        InitializeEvents();

        this.CanEditSR = UserManagement.UserCanEdit(WTSModuleOption.SustainmentRequest);
        this.CanViewSR = this.CanEditSR || UserManagement.UserCanView(WTSModuleOption.SustainmentRequest);

        this.dtStatus = MasterData.StatusList_Get(includeArchive: false);
        this.dtStatus.DefaultView.RowFilter = "StatusType IN ('', 'SR')";
        this.dtStatus = this.dtStatus.DefaultView.ToTable();

        if (dtStatus != null && dtStatus.Rows.Count > 0)
        {
            dtStatus.Columns["Status"].ReadOnly = false;
            dtStatus.Rows[0]["Status"] = "-Select-";
        }

        this.dtType = MasterData.SRTypeList_Get(includeArchive: false);

        if (dtType != null && dtType.Rows.Count > 0)
        {
            dtType.Columns["SRType"].ReadOnly = false;
            dtType.Rows[0]["SRType"] = "-Select-";
        }

        this.dtPriority = MasterData.PriorityList_Get(includeArchive: false);
        this.dtSRRank = this.dtPriority;
        this.dtPriority.DefaultView.RowFilter = "PRIORITYTYPE IN ('', 'SR')";
        this.dtPriority = this.dtPriority.DefaultView.ToTable();

        if (dtPriority != null && dtPriority.Rows.Count > 0)
        {
            dtPriority.Columns["Priority"].ReadOnly = false;
            dtPriority.Rows[0]["Priority"] = "-Select-";
        }

        this.dtSRRank.DefaultView.RowFilter = "PRIORITYTYPE IN ('', 'SR Rank')";
        this.dtSRRank = this.dtSRRank.DefaultView.ToTable();

        if (dtSRRank != null && dtSRRank.Rows.Count > 0)
        {
            dtSRRank.Columns["Priority"].ReadOnly = false;
            dtSRRank.Rows[0]["Priority"] = "-Select-";
        }

        if (searchMode)
        {
            InitializeSearchModeDisplay();
        }

        DataTable dt = LoadData();
        if (dt != null)
        {
            this.DCC = dt.Columns;
            this.TotalCount = dt.Rows.Count;
        }

        if (_export && dt != null)
        {
            exportExcel(dt);
        }

        grdData.DataSource = dt;

        if (!Page.IsPostBack && this.GridPageIndex > 0 && this.GridPageIndex < ((decimal)dt.Rows.Count / (decimal)25)) grdData.PageIndex = this.GridPageIndex;

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

        if (Request.QueryString["SubmittedBy"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["SubmittedBy"].ToString()))
        {
            _selectedSubmittedBy = Server.UrlDecode(Request.QueryString["SubmittedBy"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["StatusIDs"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["StatusIDs"].ToString()))
        {
            _selectedStatuses = Server.UrlDecode(Request.QueryString["StatusIDs"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["ReasoningIDs"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["ReasoningIDs"].ToString()))
        {
            _selectedReasonings = Server.UrlDecode(Request.QueryString["ReasoningIDs"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["Systems"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["Systems"].ToString()))
        {
            _selectedSystems = Server.UrlDecode(Request.QueryString["Systems"].Trim()).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }

        if (Request.QueryString["GoToSRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GoToSRID"]))
        {
            this._goToSRID = Int32.Parse(Request.QueryString["GoToSRID"]);
        }

        if (Request.QueryString["Export"] != null
        && !string.IsNullOrWhiteSpace(Request.QueryString["Export"].ToString()) && Request.QueryString["Export"] == "1")
        {
            _export = true;
        }

        if (Request.QueryString["Mode"] == "Search")
        {
            searchMode = true;
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

    protected void InitializeColumnData(ref DataTable dt)
    {
        try
        {
            string displayName = string.Empty, groupName = string.Empty;
            bool blnVisible = false, blnSortable = false, blnOrderable = false;

            foreach (DataColumn gridColumn in dt.Columns)
            {
                displayName = gridColumn.ColumnName;
                blnVisible = false;
                blnSortable = false;
                blnOrderable = false;
                groupName = string.Empty;

                switch (gridColumn.ColumnName)
                {
                    case "SR #":
                        blnVisible = true;
                        break;
                    case "SRRankID":
                        displayName = "SR Rank";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Submitted By":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Submitted Date":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "StatusID":
                        displayName = "Status";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Reasoning":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "User's Priority":
                        displayName = "User Priority";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "INVPriorityID":
                        displayName = "Investigation Priority";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "System":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Description":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "TaskData":
                        displayName = "Work Task";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Sort":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                }

                columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
                columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
            }

            //Initialize the columnData
            columnData.Initialize(ref dt, ";", "~", "|");

            //Get sortable columns and default column order
            SortableColumns = columnData.SortableColumnsToString();
            DefaultColumnOrder = columnData.DefaultColumnOrderToString();
            //Sort and Reorder Columns
            columnData.ReorderDataTable(ref dt, ColumnOrder);
            columnData.SortDataTable(ref dt, SortOrder);
            SelectedColumnOrder = columnData.CurrentColumnOrderToString();

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    private void InitializeSearchModeDisplay()
    {
        Master.FindControl("ContentPlaceHolderHeader").Visible = false;

        DataTable dt = SR.AORSRWebsystemList_Get(true);
        msWebsystems.Items = dt;
        msWebsystems.SelectedItems = _selectedSystems != null ? string.Join(",", _selectedSystems) : "";

        msStatus.Items = dtStatus;
        msStatus.SelectedItems = _selectedStatuses != null ? string.Join(",", _selectedStatuses) : "";

        msReasoning.Items = dtType;
        msReasoning.SelectedItems = _selectedReasonings != null ? string.Join(",", _selectedReasonings) : "";
    }
    #endregion

    #region Data
    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

        if (IsPostBack && Session["dtSustainmentRequest"] != null)
        {
            dt = (DataTable)Session["dtSustainmentRequest"];
        }
        else
        {
            dt = SR.SRList_Get(SRID: _goToSRID, SubmittedBy: String.Join(",", _selectedSubmittedBy), StatusIDs: String.Join(",", _selectedStatuses), ReasoningIDs: String.Join(",", _selectedReasonings), Systems: String.Join(",,", _selectedSystems));

            Session["dtSustainmentRequest"] = dt;
        }

        _srWorkloadPriority = calculateSRWorkloadPriority(dt);

        InitializeColumnData(ref dt);
        dt.AcceptChanges();

        return dt;
    }

    private string calculateSRWorkloadPriority(DataTable sr)
    {
        int ptsCount = 0, releaseCount = 0, recurCount = 0, stagedCount = 0, unpriorCount = 0, closeCount = 0, openCount = 0, percentComplete = 0;

        foreach (DataRow dr in sr.Rows)
        {
            switch (dr["SRRankID"].ToString())
            {
                case "39":
                    ptsCount++;
                    break;
                case "40":
                    releaseCount++;
                    break;
                case "41":
                    recurCount++;
                    break;
                case "42":
                    stagedCount++;
                    break;
                case "43":
                    unpriorCount++;
                    break;
                case "44":
                    closeCount++;
                    break;
            }
        }

        openCount = ptsCount + releaseCount + recurCount + stagedCount + unpriorCount;
        if (openCount + closeCount > 0) percentComplete = (closeCount*100)/(openCount + closeCount);

        return "" + ptsCount + "." + releaseCount + "." + recurCount + "." + stagedCount + "." + unpriorCount + "." + closeCount + " (" + openCount + ", " + percentComplete +"%)";
    }
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        FormatHeaderRowDisplay(ref row);
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        FormatRowDisplay(ref row);

        if (DCC.Contains("SR_ID"))
        {
            row.Attributes.Add("sr_id", row.Cells[DCC.IndexOf("SR_ID")].Text);
            row.Attributes.Add("sr_external", row.Cells[DCC.IndexOf("System")].Text != "WTS" ? "1" : "0");

            if (searchMode)
            {
                var tdsr = row.Cells[DCC.IndexOf("SR #")];
                tdsr.Style["text-decoration"] = "underline";
                tdsr.Style["color"] = "blue";
                tdsr.Style["cursor"] = "pointer";
                tdsr.Attributes["tdsrnum"] = "1";
            }

            string[] submittedByString = row.Cells[DCC.IndexOf("Submitted By")].Text.Split('.');
            if (submittedByString.Length == 2)
            {
                string firstName = submittedByString[0];
                string lastName = submittedByString[1];
                row.Cells[DCC.IndexOf("Submitted By")].Text = char.ToUpper(firstName[0]) + firstName.Substring(1).ToLower() + "." + char.ToUpper(lastName[0]) + lastName.Substring(1).ToLower();
            } else if (submittedByString.Length == 3)
            {
                string firstName = submittedByString[0];
                string middleName = submittedByString[1];
                string lastName = submittedByString[2];
                row.Cells[DCC.IndexOf("Submitted By")].Text = char.ToUpper(firstName[0]) + firstName.Substring(1).ToLower() + "." + char.ToUpper(middleName[0]) + "." + char.ToUpper(lastName[0]) + lastName.Substring(1).ToLower();
            }

            if (this.CanEditSR)
            {
                if (DCC.Contains("StatusID"))
                {
                    row.Cells[DCC.IndexOf("StatusID")].Style["text-align"] = "center";
                    DropDownList statusDDL = CreateDropDownList("SR", row.Cells[DCC.IndexOf("SR_ID")].Text, this.dtStatus, "Status", "Status", "StatusID", row.Cells[DCC.IndexOf("StatusID")].Text, "", null);
                    statusDDL.Enabled = row.Cells[DCC.IndexOf("System")].Text == "WTS" ? true : false;
                    row.Cells[DCC.IndexOf("StatusID")].Controls.Add(statusDDL);
                }

                if (DCC.Contains("Reasoning"))
                {
                    row.Cells[DCC.IndexOf("Reasoning")].Style["text-align"] = "center";
                    DropDownList reasoningDDL = CreateDropDownList("SR", row.Cells[DCC.IndexOf("SR_ID")].Text, this.dtType, "Reasoning", "SRType", "SRTypeID", row.Cells[DCC.IndexOf("Type_ID")].Text, row.Cells[DCC.IndexOf("Reasoning")].Text, null);
                    reasoningDDL.Enabled = row.Cells[DCC.IndexOf("System")].Text == "WTS" ? true : false;
                    row.Cells[DCC.IndexOf("Reasoning")].Controls.Add(reasoningDDL);
                }

                if (DCC.Contains("User's Priority"))
                {
                    row.Cells[DCC.IndexOf("User's Priority")].Style["text-align"] = "center";
                    DropDownList userPriorityDDL = CreateDropDownList("SR", row.Cells[DCC.IndexOf("SR_ID")].Text, this.dtPriority, "User's Priority", "Priority", "PriorityID", row.Cells[DCC.IndexOf("Priority_ID")].Text, row.Cells[DCC.IndexOf("User's Priority")].Text, null);
                    userPriorityDDL.Enabled = row.Cells[DCC.IndexOf("System")].Text == "WTS" ? true : false;
                    row.Cells[DCC.IndexOf("User's Priority")].Controls.Add(userPriorityDDL);
                }

                if (DCC.Contains("INVPriorityID"))
                {
                    row.Cells[DCC.IndexOf("INVPriorityID")].Style["text-align"] = "center";
                    DropDownList INVPriorityDDL = CreateDropDownList("SR", row.Cells[DCC.IndexOf("SR_ID")].Text, this.dtPriority, "Investigation Priority", "Priority", "PriorityID", row.Cells[DCC.IndexOf("INVPriorityID")].Text, row.Cells[DCC.IndexOf("INVPriority")].Text, null);
                    INVPriorityDDL.Enabled = row.Cells[DCC.IndexOf("System")].Text == "WTS" ? true : false;
                    row.Cells[DCC.IndexOf("INVPriorityID")].Controls.Add(INVPriorityDDL);
                }

                if (DCC.Contains("SRRankID"))
                {
                    row.Cells[DCC.IndexOf("SRRankID")].Style["text-align"] = "center";
                    DropDownList srRankDDL = CreateDropDownList("SR", row.Cells[DCC.IndexOf("SR_ID")].Text, this.dtSRRank, "SRRankID", "Priority", "PriorityID", row.Cells[DCC.IndexOf("SRRankID")].Text, "", null);
                    srRankDDL.Enabled = row.Cells[DCC.IndexOf("System")].Text == "WTS" ? true : false;
                    row.Cells[DCC.IndexOf("SRRankID")].Controls.Add(srRankDDL);
                }

                if (DCC.Contains("Sort"))
                {
                    row.Cells[DCC.IndexOf("Sort")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Sort")].Controls.Add(CreateTextBox("SR", row.Cells[DCC.IndexOf("SR_ID")].Text, "Sort", row.Cells[DCC.IndexOf("Sort")].Text, true));
                }

                if (DCC.Contains("TaskData"))
                {
                    row.Cells[DCC.IndexOf("TaskData")].Style["text-align"] = "center";
                    string txtTask = Server.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("TaskData")].Text)).Trim();
                    row.Cells[DCC.IndexOf("TaskData")].Controls.Add(CreateTaskLink(txtTask));
                }
            }
        }

        if (DCC.Contains("Submitted Date"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Submitted Date")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Submitted Date")].Text = String.Format("{0:M/d/yyyy}", nDate);
            }
        }

        if (DCC.Contains("Description"))
        {
            string txtDescription = Server.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("Description")].Text)).Trim();

            if (txtDescription.Length > 175)
            {
                row.Cells[DCC.IndexOf("Description")].Controls.Add(CreateTextLink(txtDescription, 175));
            }
            else
            {
                row.Cells[DCC.IndexOf("Description")].Text = txtDescription;
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

        if (DCC.Contains("SR #")) row.Cells[DCC.IndexOf("SR #")].Style["width"] = "45px";
        if (DCC.Contains("SRRankID")) row.Cells[DCC.IndexOf("SRRankID")].Style["width"] = "205px";
        if (DCC.Contains("Submitted By")) row.Cells[DCC.IndexOf("Submitted By")].Style["width"] = "150px";
        if (DCC.Contains("Submitted Date")) row.Cells[DCC.IndexOf("Submitted Date")].Style["width"] = "100px";
        if (DCC.Contains("StatusID")) row.Cells[DCC.IndexOf("StatusID")].Style["width"] = "150px";
        if (DCC.Contains("Reasoning")) row.Cells[DCC.IndexOf("Reasoning")].Style["width"] = "150px";
        if (DCC.Contains("User's Priority")) row.Cells[DCC.IndexOf("User's Priority")].Style["width"] = "125px";
        if (DCC.Contains("INVPriorityID")) row.Cells[DCC.IndexOf("INVPriorityID")].Style["width"] = "125px";
        if (DCC.Contains("System")) row.Cells[DCC.IndexOf("System")].Style["width"] = "90px";
        if (DCC.Contains("Description")) row.Cells[DCC.IndexOf("Description")].Style["width"] = "600px";
        if (DCC.Contains("TaskData")) row.Cells[DCC.IndexOf("TaskData")].Style["width"] = "95px";
        if (DCC.Contains("Sort")) row.Cells[DCC.IndexOf("Sort")].Style["width"] = "45px";

        if (DCC.Contains("Z")) row.Cells[DCC.IndexOf("Z")].Text = "";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";

            if (DCC.Contains("System")) row.Cells[DCC.IndexOf("System")].Style["text-align"] = "center";
        }
    }

    private TextBox CreateTextBox(string typeName, string typeID, string field, string value, bool isNumber)
    {
        string txtValue = Server.HtmlDecode(Uri.UnescapeDataString(value)).Trim();
        TextBox txt = new TextBox();

        txt.Text = txtValue;
        txt.MaxLength = 50;
        txt.Width = new Unit(field == "Sort" ? 90 : 95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("typeName", typeName);
        txt.Attributes.Add("typeID", typeID);
        txt.Attributes.Add("field", field);
        txt.Attributes.Add("original_value", txtValue);

        if (isNumber)
        {
            txt.MaxLength = 5;
            txt.Style["text-align"] = "center";
        }

        if (searchMode)
        {
            txt.Attributes["disabled"] = "1";
        }

        return txt;
    }

    private DropDownList CreateDropDownList(string typeName, string typeID
        , DataTable dt, string field
        , string textField, string valueField
        , string value, string text = ""
        , List<string> attributes = null)
    {
        DropDownList ddl = new DropDownList();

        textField = Server.HtmlDecode(textField).Trim();
        valueField = Server.HtmlDecode(valueField).Trim();
        value = Server.HtmlDecode(value).Trim();
        text = Server.HtmlDecode(text).Trim();

        ddl.Width = new Unit(95, UnitType.Percentage);
        ddl.Attributes["class"] = "saveable";
        ddl.Attributes["onchange"] = "input_change(this);";
        ddl.Attributes.Add("typeName", typeName);
        ddl.Attributes.Add("typeID", typeID);
        ddl.Attributes.Add("field", field);
        ddl.Attributes.Add("original_value", value);
        ddl.Style["background-color"] = "#F5F6CE";

        if (dt == null)
        {
            return ddl;
        }

        ListItem item = null;
        foreach (DataRow row in dt.Rows)
        {
            item = new ListItem();
            item.Text = row[textField].ToString();
            item.Value = row[valueField].ToString();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (string key in attributes)
                {
                    item.Attributes.Add(key.Trim(), key.Trim());
                }
            }

            if (ddl.Items.FindByValue(item.Value) == null)
            {
                ddl.Items.Add(item);
            }
        }

        WTSUtility.SelectDdlItem(ddl, string.IsNullOrWhiteSpace(value) ? "0" : value, text);

        if (searchMode)
        {
            ddl.Attributes["disabled"] = "1";
        }

        return ddl;
    }

    private LinkButton CreateTextLink(string txt, int sub)
    {
        LinkButton lb = new LinkButton();

        lb.Text = txt.Substring(0, sub) + "...";
        lb.ToolTip = txt;
        lb.Attributes["onclick"] = string.Format("showText('{0}'); return false;", Uri.EscapeDataString(txt));

        return lb;
    }

    private LinkButton CreateTaskLink(string txtTask)
    {
        LinkButton lb = new LinkButton();
        string[] taskData = txtTask.Split('-');
        if (taskData.Length == 1)
        {
            lb.Text = taskData[0];
            lb.ToolTip = taskData[0];
            lb.Attributes["onclick"] = string.Format("openTask('{0}'); return false;", Uri.EscapeDataString(taskData[0]));
        } else if (taskData.Length == 3)
        {
            lb.Text = taskData[0] + " - " + taskData[2];
            lb.ToolTip = taskData[0] + " - " + taskData[2];
            lb.Attributes["onclick"] = string.Format("openTask('{0}', '{1}', '{2}'); return false;", Uri.EscapeDataString(taskData[0]), Uri.EscapeDataString(taskData[1]), Uri.EscapeDataString(taskData[2]));
        }

        return lb;
    }
    #endregion

    #region Excel
    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        formatParent(ref copydt);
        String strName = "Sustainment Requests";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[0];
        int rowCount = 0;
        printParentHeader(ws, ref rowCount, copydt.Columns);
        foreach (DataRow parentRow in copydt.Rows)
        {
            if (parentRow.Field<int>("SR_ID") != 0)
            {
                printParent(parentRow, ws, ref rowCount, copydt);
                rowCount++;
            }
        }
        ws.Cells.DeleteColumn(0);
        ws.AutoFitColumns();
        wb.Save(ms, SaveFormat.Xlsx);
        Response.BufferOutput = true;
        Response.ContentType = "application/xlsx";
        Response.AddHeader("content-disposition", "attachment;  filename=" + strName + ".xlsx");
        Response.BinaryWrite(ms.ToArray());
        Response.End();
    }

    private void printParentHeader(Worksheet ws, ref int rowCount, DataColumnCollection columns)
    {
        Aspose.Cells.Style style = new Aspose.Cells.Style();
        style.Pattern = BackgroundType.Solid;
        style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
        for (int i = 0; i < columns.Count; i++)
        {
            ws.Cells[rowCount, i].PutValue(columns[i].ColumnName);
            ws.Cells[rowCount, i].SetStyle(style);
        }
        rowCount++;
    }
    private void printParent(DataRow parentRow, Worksheet ws, ref int rowCount, DataTable dt)
    {
        int i = 0;
        foreach (object value in parentRow.ItemArray)
        {
            ws.Cells[rowCount, i].PutValue(Server.HtmlDecode(Uri.UnescapeDataString(value.ToString())));
            i++;
        }
    }

    private static void formatParent(ref DataTable dt)
    {
        dt.Columns.Remove("Sort");
        dt.Columns.Remove("CREATEDBY_ID");
        dt.Columns.Remove("CREATEDDATE_ID");
        dt.Columns.Remove("UPDATEDBY_ID");
        dt.Columns.Remove("UPDATEDDATE_ID");
        dt.Columns.Remove("Priority_ID");
        dt.Columns.Remove("Type_ID");
        dt.Columns.Remove("Status_ID");
        dt.Columns.Remove("INVPriorityID");
        dt.Columns.Remove("Z");
        dt.Columns["INVPriority"].ColumnName = "Investigation Priority";
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static bool verifySRExists(string id)
    {
        DataTable dtSRs = SR.SRList_Get();
        foreach(DataRow dr in dtSRs.Rows)
        {
            if (dr["SR_ID"].ToString() == id) return true;
        }

        return false;
    }

    [WebMethod()]
    public static string DeleteSR(string sr)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int SR_ID = 0;
            int.TryParse(sr, out SR_ID);

            deleted = SR.SR_Delete(SRID: SR_ID);
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

    [WebMethod()]
    public static string SaveChanges(string changes)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            XmlDocument docChanges = (XmlDocument)JsonConvert.DeserializeXmlNode(changes, "changes");

            saved = SR.SR_Update(Changes: docChanges);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            saved = false;
            errorMsg = ex.Message;
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}