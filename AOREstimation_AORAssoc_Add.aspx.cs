﻿using Aspose.Cells;  //for exporting to excel
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;
using Row = System.Data.Common.CommandTrees.ExpressionBuilder.Row;

public partial class AOREstimation_AORAssoc_Add : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool CanEditWorkItem = false;
    protected string AORName = string.Empty;
    protected int AORID = 0;
    protected int SRID = 0;
    protected int TaskID = 0;
    protected int CRID = 0;
    protected int DeliverableID = 0;
    protected int ReleaseID = 0;
    protected int _AOREstimation_AORReleaseID = 0;
    protected string Type = string.Empty;
    protected string SubType = string.Empty;
    protected int ReleaseFilterID = 0;
    protected string FieldChangedFilter = "0";
    protected string[] QFSystem = { };
    protected string[] QFRelease = { };
    protected string[] QFCRContract = { };
    protected string[] QFStatus = { };
    protected string QFName = "";
    protected bool _export = false;
    protected string ReleaseOptions = string.Empty;
    private DataColumnCollection DCC;
    protected string CurrentSystemID;
    protected string CurrentReleaseID;
    private XmlDocument Levels = new XmlDocument();
    protected int LevelCount = 0;
    protected int dtColumnCnt = 0;
    protected int rowCount = 0;
    protected int uniA = (int)'A';
    protected int dtRowCount = 0;
    protected string AddCallbackFunction = string.Empty;
    protected string SelectCallbackFunction = string.Empty;
    protected string SelectedTasks = string.Empty;
    protected string SelectedSubTasks = string.Empty;

    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        var tempVal = Request.QueryString;

        ReadQueryString();
        InitializeEvents();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);


        DataTable dtCRAOR = LoadData();

        if (dtCRAOR != null) this.DCC = dtCRAOR.Columns;

        grdData.AllowPaging = false;
        grdData.AlternatingRowColor = System.Drawing.Color.White;
        grdData.DataSource = dtCRAOR;
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
        
        if (Request.QueryString["AOREstimation_AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AOREstimation_AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AOREstimation_AORReleaseID"], out this._AOREstimation_AORReleaseID);
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

        if (Request.QueryString["CRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CRID"]))
        {
            int.TryParse(Request.QueryString["CRID"], out this.CRID);
        }

        if (Request.QueryString["DeliverableID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["DeliverableID"]))
        {
            int.TryParse(Request.QueryString["DeliverableID"], out this.DeliverableID);
        }

        if (Request.QueryString["Type"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Type"]))
        {
            this.Type = Request.QueryString["Type"];
        }

        if (Request.QueryString["SubType"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SubType"]))
        {
            this.SubType = Request.QueryString["SubType"];
        }

        if (Request.QueryString["AddCallback"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AddCallback"]))
        {
            this.AddCallbackFunction = Request.QueryString["AddCallback"];
        }

        if (Request.QueryString["SelectCallback"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectCallback"]))
        {
            this.SelectCallbackFunction = Request.QueryString["SelectCallback"];
        }

        if (Request.QueryString["ReleaseFilterID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseFilterID"]))
        {
            int.TryParse(Request.QueryString["ReleaseFilterID"], out this.ReleaseFilterID);
        }

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.ReleaseID);
        }

        if (Request.QueryString["FieldChangedFilter"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["FieldChangedFilter"]))
        {
            this.FieldChangedFilter = Request.QueryString["FieldChangedFilter"];
        }

        if (Request.QueryString["SelectedSystems"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSystems"]))
            this.QFSystem = Request.QueryString["SelectedSystems"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["SelectedReleases"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedReleases"]))
            this.QFRelease = Request.QueryString["SelectedReleases"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["SelectedCRContracts"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedCRContracts"]))
            this.QFCRContract = Request.QueryString["SelectedCRContracts"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"]))
            this.QFStatus = Request.QueryString["SelectedStatuses"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["txtSearch"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["txtSearch"]))
        {
            this.QFName = Request.QueryString["txtSearch"].Trim();
            txtSearch.Text = Request.QueryString["txtSearch"].Trim(); // this just resets the search box back to default value
        }

        if (Request.QueryString["Export"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);

        if (Request.QueryString["SelectedTasks"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedTasks"]))
            this.SelectedTasks = Request.QueryString["SelectedTasks"];

        if (Request.QueryString["SelectedSubTasks"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSubTasks"]))
            this.SelectedSubTasks = Request.QueryString["SelectedSubTasks"];

        if (HttpContext.Current.Session["filters_AorWork"] != null && !string.IsNullOrWhiteSpace(HttpContext.Current.Session["filters_AorWork"].ToString()))
            Page.ClientScript.RegisterArrayDeclaration("filters_AorWork", JsonConvert.SerializeObject(HttpContext.Current.Session["filters_AorWork"], Newtonsoft.Json.Formatting.None));

        if (Request.QueryString["AORName"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORName"]))
        {
            this.AORName = Request.QueryString["AORName"];
            this.txtAORName.Text = this.AORName;
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
        var dt = AOR.AOREstimation_Assoc_AORList(AOREstimation_AORReleaseID: _AOREstimation_AORReleaseID);

        if (txtSearch.Text != "") dt.FilterColNames("AOR Name", txtSearch.Text);
        if (_export) ExportExcel("AORs", dt.DefaultView.ToTable());

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

        if ((this.Type == "CR AOR" || this.Type == "Release Schedule AOR") && this.CurrentSystemID != row.Cells[DCC.IndexOf("WTS_SYSTEM_ID")].Text)
        {
            CreateRow(row);
            this.CurrentSystemID = row.Cells[DCC.IndexOf("WTS_SYSTEM_ID")].Text;
            this.CurrentReleaseID = row.Cells[DCC.IndexOf("ProductVersion_ID")].Text;
        }

        if ((this.Type == "CR AOR" || this.Type == "Release Schedule AOR" || this.Type == "Previous Attachment") && this.CurrentReleaseID != row.Cells[DCC.IndexOf("ProductVersion_ID")].Text)
        {
            //for (int i = 0; i < row.Cells.Count; i++)
            //{
            //    row.Cells[i].Style["border-top"] = "1px solid blue";
            //}

            this.CurrentReleaseID = row.Cells[DCC.IndexOf("ProductVersion_ID")].Text;
        }

        FormatRowDisplay(ref row);

        if ((this.Type == "CR AOR" || this.Type == "Release Schedule AOR") && this.CanEditAOR && DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("AOR #")].Text));
        }

        if (this.CanViewAOR && DCC.Contains("AOR #") && DCC.Contains("AORRelease_ID"))
        {
            row.Cells[DCC.IndexOf("AOR #")].Controls.Add(CreateLink("AOR", row.Cells[DCC.IndexOf("AOR #")].Text, row.Cells[DCC.IndexOf("AORRelease_ID")].Text));
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
            if (this.Type == "Contract") row.Cells[i].Style["display"] = "none";
        }

        if (this.Type != "CR AOR" && this.Type != "Release Schedule AOR" && DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["display"] = "none";
        if ((this.Type == "CR AOR" || this.Type == "Release Schedule AOR") && DCC.Contains("System")) row.Cells[DCC.IndexOf("System")].Style["display"] = "none";

        if (this.Type == "Contract" && DCC.Contains("Contract"))
        {
            row.Cells[DCC.IndexOf("Z")].Style["display"] = "";
            row.Cells[DCC.IndexOf("Z")].Style["width"] = "35px";
            row.Cells[DCC.IndexOf("Contract")].Style["display"] = "";
        }

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = "35px";
        }

        if (DCC.Contains("Release")) row.Cells[DCC.IndexOf("Release")].Style["display"] = "none";
        if (DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "45px";
        if (DCC.Contains("Z")) row.Cells[DCC.IndexOf("Z")].Text = "";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
            if (this.Type == "Contract") row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }

        if (this.Type != "CR AOR" && this.Type != "Release Schedule AOR" && DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["display"] = "none";
        if ((this.Type == "CR AOR" || this.Type == "Release Schedule AOR") && DCC.Contains("System")) row.Cells[DCC.IndexOf("System")].Style["display"] = "none";
        if (this.Type == "Contract" && DCC.Contains("Contract") && row.RowIndex > 0)
        {
            row.Cells[DCC.IndexOf("Z")].Style["display"] = "";
            row.Cells[DCC.IndexOf("Contract")].Style["display"] = "";
            row.Cells[DCC.IndexOf("X")].Style["display"] = "";
        }

        if (DCC.Contains("Date")) row.Cells[DCC.IndexOf("Date")].Style["text-align"] = "center";
        if (DCC.Contains("Work Task")) row.Cells[DCC.IndexOf("Work Task")].Style["text-align"] = "center";
        if (DCC.Contains("Change Date")) row.Cells[DCC.IndexOf("Change Date")].Style["text-align"] = "center";
        if (DCC.Contains("Release")) row.Cells[DCC.IndexOf("Release")].Style["text-align"] = "center";
        if (DCC.Contains("USERNAME")) row.Cells[DCC.IndexOf("USERNAME")].Style["display"] = "none";
        if (DCC.Contains("ApprovedByID")) row.Cells[DCC.IndexOf("ApprovedByID")].Style["display"] = "none";
        if (DCC.Contains("ApprovedDate")) row.Cells[DCC.IndexOf("ApprovedDate")].Style["display"] = "none";
        if (DCC.Contains("Added Date")) row.Cells[DCC.IndexOf("Added Date")].Style["display"] = "none";
        if (DCC.Contains("Updated By")) row.Cells[DCC.IndexOf("Updated By")].Style["display"] = "none";
        if (DCC.Contains("Updated Date")) row.Cells[DCC.IndexOf("Updated Date")].Style["display"] = "none";
        if (DCC.Contains("Release")) row.Cells[DCC.IndexOf("Release")].Style["display"] = "none";

    }

    private void CreateRow(GridViewRow row)
    {
        Table nTable = (Table)row.Parent;
        GridViewRow nRow = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
        TableCell nCell = new TableCell();

        nCell.BackColor = System.Drawing.Color.LightGray;
        nCell.ColumnSpan = 4;
        nCell.Text = row.Cells[DCC.IndexOf("System")].Text;

        nRow.Cells.Add(nCell);
        nTable.Rows.AddAt(nTable.Rows.Count - 1, nRow);
    }

    private CheckBox CreateCheckBox(string value)
    {
        CheckBox chk = new CheckBox();

        chk.Attributes["onchange"] = "input_change(this);";

        switch (this.Type)
        {
            case "Release Schedule AOR":
            case "CR AOR":
                chk.Attributes.Add("aorid", value);
                break;
        }

        return chk;
    }

    private LinkButton CreateTextLink(string txt, int sub)
    {
        LinkButton lb = new LinkButton();

        lb.Text = sub == 0 ? "View" : txt.Substring(0, sub) + "...";

        if (sub != 0) lb.ToolTip = txt;

        lb.Attributes["onclick"] = string.Format("showText('{0}', {1}); return false;", Uri.EscapeDataString(txt), sub == 0 ? 1 : 0);

        return lb;
    }

    private LinkButton CreateLink(string type, string value, string value2)
    {
        LinkButton lb = new LinkButton();

        lb.Text = value;

        switch (type)
        {
            case "AOR":
                lb.Attributes["onclick"] = string.Format("openAOR('{0}', '{1}'); return false;", value, value2);
                break;
            case "File":
                lb.Text = value2;
                lb.Attributes["onclick"] = string.Format("downloadAORAttachment('{0}'); return false;", value);
                break;
        }

        return lb;
    }
    #endregion

    #region Excel
    private bool ExportExcel(string name, DataTable dt)
    {
        bool success = false;
        string errorMsg = string.Empty;

        try
        {
            if (dt.Columns.Contains("X")) dt.Columns["X"].ColumnName = " ";
            if (dt.Columns.Contains("AORRelease_ID")) dt.Columns.Remove("AORRelease_ID");
            if (dt.Columns.Contains("WTS_SYSTEM_ID")) dt.Columns.Remove("WTS_SYSTEM_ID");
            dt.AcceptChanges();

            var wb = new Workbook(FileFormatType.Xlsx);
            var ws = wb.Worksheets[0];
            var style1 = ws.Cells[0, 0].GetStyle();
            var style2 = ws.Cells[0, 0].GetStyle();

            foreach (DataRow row in dt.Rows)
            {
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    if (row[i] is string)
                    {
                        row[i] = string.IsNullOrWhiteSpace((string)row[i]) ? row[i] : System.Web.HttpUtility.UrlDecode((string)row[i]);
                    }
                }
            }

            ws.Cells.ImportDataTable(dt, true, 0, 0, false, false);

            style1.ForegroundColor = ColorTranslator.FromHtml("#D9DCF3");
            style1.BackgroundColor = ColorTranslator.FromHtml("#D9DCF3");
            style1.Pattern = BackgroundType.VerticalStripe;
            style1.Font.Color = Color.Black;
            style1.Font.IsBold = true;
            style1.Font.Size = 11;

            style2.ForegroundColor = ColorTranslator.FromHtml("#D3D3D3");
            style2.BackgroundColor = ColorTranslator.FromHtml("#D3D3D3");
            style2.Pattern = BackgroundType.VerticalStripe;
            style2.Font.Color = Color.Black;
            style2.Font.IsBold = true;

            for (int i = 0; i < 7; i++)
                ws.Cells[0, i].SetStyle(style1);

            ws.AutoFitColumns();

            if (ws.Cells.MaxDataRow > 0)
            {
                Range rn = ws.Cells.CreateRange("E1:E" + ws.Cells.MaxDataRow * 2);
                rn.Name = "newRange";

                Range range = wb.Worksheets.GetRangeByName("newRange");
                IEnumerator e = range.GetEnumerator();

                var rowCount = 0;
                var prevCelllValue = "System";

                while (e.MoveNext())
                {
                    var c = (Cell)e.Current;
                    if (c.StringValue != prevCelllValue && !string.IsNullOrEmpty(c.StringValue))
                    {
                        if (c.StringValue.IndexOf("%") != -1)
                        {
                            int x = 5;
                        }
                        ws.Cells.InsertRows(rowCount, 1);
                        ws.Cells[rowCount, 0].Value = c.StringValue;
                        for (int i = 0; i < 7; i++) ws.Cells[rowCount, i].SetStyle(style2);
                    }

                    prevCelllValue = c.StringValue;
                    rowCount++;
                }

                ws.Cells.DeleteColumn(ws.Cells.MaxDataColumn);
            }

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
            LogUtility.LogException(ex);
        }

        return success;
    }

    #endregion

    #region AJAX
    [WebMethod()]
    public static string AORSearch(string aorName, string AORWorkType)
    {
        DataTable dt = new DataTable();

        try
        {
            dt = AOR.AORList_Get(AORID: 0);
            dt.DefaultView.RowFilter = "[AOR Name] LIKE '%" + aorName.Replace("'", "''") + "%'";
            dt = dt.DefaultView.ToTable();
            dt.Columns["AOR #"].ColumnName = "AORID";
            dt.Columns["AOR Name"].ColumnName = "AORName";
            dt.Columns["AOR Workload Type"].ColumnName = "AORWorkType";

            if (AORWorkType != "")
            {
                dt.DefaultView.RowFilter = "[AORWorkType] LIKE '%" + AORWorkType + "%'";
                dt = dt.DefaultView.ToTable();
            }

            dt.AcceptChanges();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string Add(string AOREstimation_AORReleaseID, string additions)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            int AOREstimation_AORRelease_ID = 0;
            int.TryParse(AOREstimation_AORReleaseID, out AOREstimation_AORRelease_ID);

            XmlDocument docAdditions = (XmlDocument)JsonConvert.DeserializeXmlNode(additions, "additions");

            saved = AOR.AOREstimation_Assoc_Add(AOREst_AORReleaseID: AOREstimation_AORRelease_ID, Additions: docAdditions);
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

    [WebMethod(true)]
    public static string SetFilterSession(dynamic filters)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        string sessionid = HttpContext.Current.Session.SessionID;
        try
        {
            if (filters != null)
            {
                HttpContext.Current.Session["filters_AorWork"] = JsonConvert.DeserializeObject<Dictionary<string, object>>(filters);
            }
            else
            {
                HttpContext.Current.Session["filters_AorWork"] = null;
            }

            saved = true;
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