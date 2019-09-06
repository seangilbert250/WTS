﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;


public partial class MDGrid_SystemSuite_WorkArea : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;
    protected bool _export = false;

    protected int _qfID = 0;
    protected int _qfSystemSuiteID = 0;
    protected int _qfTaskStatus = 0;

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;

    protected bool CanView = false;
    protected bool CanEdit = false;
    protected bool IsAdmin = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.IsAdmin = UserManagement.UserIsInRole("Admin");
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
        this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.MasterData));

        readQueryString();

        initControls();

        loadGridData();
    }

    private void readQueryString()
    {
        if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
            || Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
        {
            _refreshData = true;
        }
        if (Request.QueryString["QFID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["QFID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["QFID"].ToString()), out this._qfID);
        }

        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }
        if (Request.QueryString["Export"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);
        }
        if (Request.QueryString["WTS_SYSTEM_SUITEID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["WTS_SYSTEM_SUITEID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["WTS_SYSTEM_SUITEID"].ToString()), out this._qfSystemSuiteID);
        }
        if (Request.QueryString["TaskStatus"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["TaskStatus"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["TaskStatus"].ToString()), out this._qfTaskStatus);
            ddlTaskStatus.Items.FindByValue(_qfTaskStatus.ToString()).Selected = true;
        }
    }

    private void initControls()
    {
        grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
        grdMD.GridRowDataBound += grdMD_GridRowDataBound;
        grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
    }


    private void loadGridData()
    {
        DataTable dt = MasterData.SystemSuite_WorkAreaList_Get(includeArchive: true, systemSuiteID: _qfSystemSuiteID, workTaskStatus: _qfTaskStatus);
        HttpContext.Current.Session["dtMD_WorkArea"] = dt;

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

            InitializeColumnData(ref dt);
            dt.AcceptChanges();
        }

        DataTable dtSystems = MasterData.SystemList_Get(WTS_SYSTEM_SUITEID: _qfSystemSuiteID);
        dtSystems.DefaultView.RowFilter = "WTS_SYSTEMID NOT IN (0)";
        dtSystems = dtSystems.DefaultView.ToTable();
        msWorkAreaSystems.Items = dtSystems;

        DataTable dtWorkAreas = MasterData.WorkAreaList_Get();
        string workAreaIDs = string.Empty;
        if (dt != null && dt.Rows.Count > 1)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkAreaID"].ToString() != "0") workAreaIDs += dr["WorkAreaID"].ToString() + ",";
            }
            dtWorkAreas.DefaultView.RowFilter = "WorkAreaID NOT IN (" + workAreaIDs + ")";
            dtWorkAreas = dtWorkAreas.DefaultView.ToTable();
        }

        ddlWorkArea.DataSource = dtWorkAreas;
        ddlWorkArea.DataValueField = "WorkAreaID";
        ddlWorkArea.DataTextField = "WorkArea";
        ddlWorkArea.DataBind();

        int count = dt.Rows.Count;
        count = count > 0 ? count - 1 : count; //need to subtract the empty row
        spanRowCount.InnerText = count.ToString();

        if (_export)
        {
            ExportExcel(dt);
        }
        else
        {
            grdMD.DataSource = dt;
            grdMD.DataBind();
        }
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
                blnVisible = true;
                blnSortable = true;
                blnOrderable = true;
                groupName = string.Empty;

                switch (gridColumn.ColumnName)
                {
                    case "WorkAreaID":
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = false;
                        break;
                    case "WorkArea":
                        displayName = "Work Area";
                        break;
                    case "WorkItem_Count":
                        displayName = "Work Tasks";
                        break;
                    case "RQMT_Count":
                        displayName = "RQMTs";
                        break;
                    case "ProposedPriorityRank":
                        displayName = "Proposed Priority";
                        break;
                    case "ActualPriorityRank":
                        displayName = "Approved Priority";
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

    #region Grid

    void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        row.Cells[DCC.IndexOf("WorkArea")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Description")].Style["text-align"] = "center";
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        //add edit link
        string itemId = row.Cells[DCC.IndexOf("WorkAreaID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }
        
        row.Attributes.Add("itemID", itemId);
        
        if (CanView)
        {
            for (int x = 3; x < DCC.Count - 4; x++)
            {
                row.Cells[x].Controls.Add(CreateCheckBox(Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkAreaID")].Text.Replace("&nbsp;", " ")), Server.HtmlDecode(row.Cells[x].Text)));
            }

            row.Cells[DCC.IndexOf("ProposedPriorityRank")].Controls.Add(WTSUtility.CreateGridTextBox("Sort_Order", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ProposedPriorityRank")].Text.Replace("&nbsp;", " ").Trim()), true));
            if (row.Cells[DCC.IndexOf("WorkItem_Count")].Text != "0") row.Cells[DCC.IndexOf("WorkItem_Count")].Controls.Add(CreateLink("Task", itemId, row.Cells[DCC.IndexOf("WorkItem_Count")].Text));
            if (row.Cells[DCC.IndexOf("RQMT_Count")].Text != "0") row.Cells[DCC.IndexOf("RQMT_Count")].Controls.Add(CreateLink("RQMT", itemId, row.Cells[DCC.IndexOf("RQMT_Count")].Text));
        }
    }

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtMD_WorkArea"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkArea"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("WorkArea")
                && i != DCC.IndexOf("Description"))
            {
                row.Cells[i].Style["text-align"] = "center";
                row.Cells[i].Style["padding-left"] = "0px";
                row.Cells[i].Style["width"] = "7%";
            }
            else
            {
                row.Cells[i].Style["padding-left"] = "5px";
                row.Cells[i].Style["text-align"] = "left";
            }
        }

        row.Cells[DCC.IndexOf("WorkArea")].Style["width"] = "200px";
        row.Cells[DCC.IndexOf("ProposedPriorityRank")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("ActualPriorityRank")].Style["width"] = "75px";
    }

    private CheckBox CreateCheckBox(string id, string value)
    {
        CheckBox chk = new CheckBox();

        chk.Attributes["onchange"] = "txt_change(this);";
        chk.Attributes.Add("workareaid", id);

        if (value == "1") chk.Checked = true;

        return chk;
    }

    private void RemoveExcelColumns(ref DataTable dt)
    {
        try
        {
            GridColsCollection cols = columnData.VisibleColumns();
            DataColumn col = null;

            //this has to be done in reverse order (RemoveAt) 
            //OR by name(Remove) or it will have undesired result
            for (int i = dt.Columns.Count - 1; i >= 0; i--)
            {
                col = dt.Columns[i];
                if (cols.ItemByColumnName(col.ColumnName) == null && col.ColumnName != "WorkAreaID" && col.ColumnName != " ")
                {
                    dt.Columns.Remove(col);
                }
            }

            if (dt.Columns.Contains("A")) dt.Columns.Remove("A");
            if (dt.Columns.Contains("X")) dt.Columns.Remove("X");

            dt.AcceptChanges();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }
    private void RenameExcelColumns(ref DataTable dt)
    {
        GridColsCollection cols = columnData.VisibleColumns();
        GridColsColumn gcCol = null;
        DataColumn col = null;

        for (int i = dt.Columns.Count - 1; i >= 0; i--)
        {
            col = dt.Columns[i];

            gcCol = cols.ItemByColumnName(col.ColumnName);
            if (gcCol != null)
            {
                dt.Columns[col.ColumnName].ColumnName = gcCol.DisplayName;
            }
        }

        dt.AcceptChanges();
    }
    private void FormatExcelRows(ref DataTable dt)
    {
        GridColsCollection cols = columnData.VisibleColumns();

        bool hasArchive = false;
        hasArchive = (cols.ItemByColumnName("ARCHIVE") != null);

        if (hasArchive)
        {
            dt.Columns.Add("ARCHIVE_NEW");
        }

        int flag = 0;
        if (hasArchive)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (hasArchive)
                {
                    if (int.TryParse(row["ARCHIVE"].ToString(), out flag) && flag == 1)
                    {
                        row["ARCHIVE_NEW"] = "Yes";
                    }
                    else
                    {
                        row["ARCHIVE_NEW"] = "No";
                    }
                }
            }

            dt.Columns["ARCHIVE_NEW"].SetOrdinal(dt.Columns["ARCHIVE"].Ordinal);
            dt.Columns.Remove("ARCHIVE");
            dt.Columns["ARCHIVE_NEW"].ColumnName = "ARCHIVE";
        }

        dt.Columns.Add(" ");
        if (dt.Columns.Contains("WorkAreaID")) dt.Columns["WorkAreaID"].SetOrdinal(dt.Columns.Count - 1);

        dt.AcceptChanges();
    }
    private bool ExportExcel(DataTable dt)
    {
        bool success = false;
        string errorMsg = string.Empty;

        try
        {
            Workbook wb = new Workbook(FileFormatType.Xlsx);
            Worksheet ws = wb.Worksheets[0];
            StyleFlag flag = new StyleFlag() { All = true };
            Aspose.Cells.Style style = new Aspose.Cells.Style();
            Aspose.Cells.Style style2 = new Aspose.Cells.Style();

            style.Pattern = BackgroundType.Solid;
            style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
            style2.Pattern = BackgroundType.Solid;
            style2.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightBlue");

            DataTable dtExcel = new DataTable();

            FormatExcelRows(ref dt);
            RemoveExcelColumns(ref dt);
            RenameExcelColumns(ref dt);
            dtExcel = dt.Clone();

            for (int i = 0; i <= dtExcel.Columns.Count - 1; i++)
            {
                dtExcel.Columns[i].DataType = typeof(string);
                dtExcel.Columns[i].AllowDBNull = true;
            }

            foreach (DataRow dr in dt.Rows)
            {
                int workAreaID = 0;
                int.TryParse(dr["WorkAreaID"].ToString(), out workAreaID);

                if (workAreaID > 0)
                {
                    dtExcel.ImportRow(dr);
                    DataTable dtDrilldown = MasterData.WorkArea_SystemList_Get(workAreaID: workAreaID);

                    if (dtDrilldown != null && dtDrilldown.Rows.Count > 1)
                    {
                        int count = 0;
                        foreach (DataRow drChild in dtDrilldown.Rows)
                        {
                            if (count == 0)
                            {
                                DataRow drSpacer = dtExcel.NewRow();
                                drSpacer[0] = "spacer";

                                dtExcel.Rows.Add(drSpacer);
                                dtExcel.Rows.Add("", "Work Area", "System", "Description", "Proposed Priority", "Approved Priority", "Tasks", "Archive");
                            }

                            int drilldownWorkAreaID = 0;
                            int.TryParse(drChild["WorkAreaID"].ToString(), out drilldownWorkAreaID);
                            if (drilldownWorkAreaID > 0) dtExcel.Rows.Add("", drChild["WorkArea"].ToString(), drChild["WTS_SYSTEM"].ToString(), drChild["DESCRIPTION"].ToString(), drChild["ProposedPriority"].ToString(), drChild["ApprovedPriority"].ToString(), drChild["WorkItem_Count"].ToString(), (drChild["ARCHIVE"].ToString() == "1" ? "Yes" : "No"));

                            if (count == dtDrilldown.Rows.Count - 1)
                            {
                                DataRow drSpacer = dtExcel.NewRow();
                                drSpacer[0] = "spacer";
                                dtExcel.Rows.Add(drSpacer);
                            }

                            count++;
                        }
                    }
                }
            }

            if (dtExcel.Columns.Contains("WorkAreaID")) dtExcel.Columns.Remove("WorkAreaID");

            string name = "Master Grid - Work Area";
            ws.Cells.ImportDataTable(dtExcel, true, 0, 0, false, false);

            for (int j = 0; j <= ws.Cells.Rows.Count - 1; j++)
            {
                if (ws.Cells.Rows[j][0].Value == "spacer")
                {
                    ws.Cells.Rows[j][0].Value = "";
                    Range spacer = ws.Cells.CreateRange(j, 0, 1, 8);
                    spacer.Merge();
                }

                if (ws.Cells.Rows[j][0].Value == "Work Area")
                {
                    Range range = ws.Cells.CreateRange(j, 0, 1, 7);
                    range.ApplyStyle(style, flag);
                }

                if (ws.Cells.Rows[j][1].Value == "Work Area")
                {
                    Range range = ws.Cells.CreateRange(j, 1, 1, 7);
                    range.ApplyStyle(style2, flag);
                }
            }

            ws.AutoFitColumns();
            MemoryStream ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Xlsx);

            Response.ContentType = "application/xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
            Response.BinaryWrite(ms.ToArray());
            success = true;

            Response.End();
        }
        catch (System.Threading.ThreadAbortException)
        {
            //expected. do nothing
        }
        catch (Exception ex)
        {
            success = false;
            errorMsg += Environment.NewLine + ex.Message;
        }

        return success;
    }

    private LinkButton CreateLink(string type, string type_ID, string type_Text)
    {
        LinkButton lb = new LinkButton();

        lb.Text = type_Text;

        switch (type)
        {
            case "RQMT":
                lb.Attributes["onclick"] = string.Format("openRQMTGrid('{0}'); return false;", type_ID);
                break;
            case "Task":
                lb.Attributes["onclick"] = string.Format("openTaskGrid({0}); return false;", type_ID);
                break;
        }

        return lb;
    }

    #endregion Grid



    [WebMethod(true)]
    public static string SaveChanges(string rows, int systemSuiteID = 0)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
            if (dtjson.Rows.Count == 0)
            {
                errorMsg = "Unable to save. An invalid list of changes was provided.";
                saved = false;
            }

            int id = 0, proposedPriorityRank = 0, approvedPriorityRank = 0, archive = 0;
            string WorkArea = string.Empty, description = string.Empty;

            string systems = string.Empty;
            

            HttpServerUtility server = HttpContext.Current.Server;
            //save
            foreach (DataRow dr in dtjson.Rows)
            {
                id = 0;
                proposedPriorityRank = 0;
                approvedPriorityRank = 0;
                WorkArea = string.Empty;
                description = string.Empty;
                systems = string.Empty;
                archive = 0;

                tempMsg = string.Empty;
                int.TryParse(dr["WorkAreaID"].ToString(), out id);
                WorkArea = server.UrlDecode(dr["WorkArea"].ToString());
                int.TryParse(dr["ProposedPriorityRank"].ToString(), out proposedPriorityRank);
                int.TryParse(dr["ActualPriorityRank"].ToString(), out approvedPriorityRank);

                for (int x = 3; x < dtjson.Columns.Count - 4; x++)
                {
                    if (dr[dtjson.Columns[x].ToString()].ToString() == "1") systems += "'" + dtjson.Columns[x].ToString() + "',";
                }

                if (string.IsNullOrWhiteSpace(WorkArea))
                {
                    tempMsg = "You must specify a value for Allocation.";
                    saved = false;
                }
                else
                {
                    DataTable dtSystem = MasterData.WorkArea_SystemList_Get(workAreaID: id, systemSuiteID: systemSuiteID);
                    foreach (DataRow drSystem in dtSystem.Rows)
                    {
                        int systemID = 0;
                        int.TryParse(drSystem["WorkArea_SystemID"].ToString(), out systemID);
                        if (systemID > 0)
                        {
                            MasterData.WorkArea_System_Delete(workAreaSystemID: systemID, cv: "0", exists: out exists, errorMsg: out errorMsg);
                        }
                    }

                    if (systems.Length > 0)
                    {
                        dtSystem = MasterData.SystemList_Get(includeArchive: true);
                        dtSystem.DefaultView.RowFilter = "(WTS_SystemSuiteID = " + systemSuiteID + "OR WTS_SystemID = 0) AND ARCHIVE = 0 AND WTS_SYSTEM IN (" + systems + ")";
                        dtSystem = dtSystem.DefaultView.ToTable();
                        foreach (DataRow drSystem in dtSystem.Rows)
                        {
                            int systemID = 0, waSystemID = 0;
                            int.TryParse(drSystem["WTS_SystemID"].ToString(), out systemID);
                            if (systemID > 0)
                            {
                                saved = MasterData.WorkArea_System_Add(workAreaID: id, systemID: systemID, description: "", proposedPriority: proposedPriorityRank, approvedPriority: 0, cv: "0", exists: out exists, newID: out waSystemID, errorMsg: out tempMsg);
                            }
                        }
                    }
                    else
                    {
                        saved = true;
                    }
                }

                if (saved)
                {
                    ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
                }

                if (tempMsg.Length > 0)
                {
                    errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
                }
            }
        }
        catch (Exception ex)
        {
            saved = false;
            errorMsg = ex.Message;
            LogUtility.LogException(ex);
        }

        result["ids"] = ids;
        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WORK_AREA);

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
    public static string SaveWorkArea(string systems, int workArea, int systemSuiteID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            string[] Systems = systems.Split(',');
            if (Systems.Length == 0)
            {
                errorMsg = "Unable to save. An invalid list of Systems was provided.";
                saved = false;
            }

            HttpServerUtility server = HttpContext.Current.Server;
            //save
            for (int x = 0; x < Systems.Length; x++)
            {
                if (workArea == 0)
                {
                    tempMsg = "You must specify a Work Area.";
                    saved = false;
                }
                else
                {
                    exists = false;
                    int systemID = 0, waSystemID = 0;
                    int.TryParse(Systems[x].ToString(), out systemID);
                    if (systemID > 0)
                    {
                        saved = MasterData.WorkArea_System_Add(workAreaID: workArea, systemID: systemID, description: "", proposedPriority: 0, approvedPriority: 0, cv: "0", exists: out exists, newID: out waSystemID, errorMsg: out tempMsg);
                    }
                }

                if (saved)
                {
                    ids += string.Format("{0}{1}", x < Systems.Length - 1 ? "," : "", workArea.ToString());
                }

                if (tempMsg.Length > 0)
                {
                    errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
                }
            }
        }
        catch (Exception ex)
        {
            saved = false;
            errorMsg = ex.Message;
            LogUtility.LogException(ex);
        }

        result["ids"] = ids;
        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WORK_AREA);

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
    public static string DeleteItem(int itemId, string item)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "id", itemId.ToString() }
            , { "item", item }
            , { "exists", "" }
            , { "hasDependencies", "" }
            , { "deleted", "" }
            , { "archived", "" }
            , { "error", "" } };
        bool exists = false, hasDependencies = false, deleted = false, archived = false;
        string errorMsg = string.Empty;

        try
        {
            //delete
            if (itemId == 0)
            {
                errorMsg = "You must specify an item to delete.";
            }
            else
            {
                deleted = MasterData.WorkArea_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
                if (hasDependencies && errorMsg.Length == 0)
                {
                    errorMsg = "Record has dependencies and could not be permanently deleted. It has been archived instead.";
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            deleted = false;
            errorMsg = ex.Message;
        }

        result["exists"] = exists.ToString();
        result["hasDependencies"] = hasDependencies.ToString();
        result["deleted"] = deleted.ToString();
        result["archived"] = archived.ToString();
        result["error"] = errorMsg;

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WORK_AREA);

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
    public static string RemoveItem(int itemId, int systemSuiteID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "id", itemId.ToString() }
            , { "exists", "" }
            , { "hasDependencies", "" }
            , { "deleted", "" }
            , { "archived", "" }
            , { "error", "" } };
        bool exists = false, hasDependencies = false, deleted = false, archived = false;
        string errorMsg = string.Empty;

        try
        {
            //delete
            if (itemId == 0)
            {
                errorMsg = "You must specify an item to remove systems.";
            }
            else
            {
                if (systemSuiteID > 0)
                {
                    DataTable dtSystem = MasterData.WorkArea_SystemList_Get(workAreaID: itemId, systemSuiteID: systemSuiteID);
                    foreach (DataRow drSystem in dtSystem.Rows)
                    {
                        int systemID = 0;
                        int.TryParse(drSystem["WorkArea_SystemID"].ToString(), out systemID);
                        if (systemID > 0)
                        {
                            deleted = MasterData.WorkArea_System_Delete(workAreaSystemID: systemID, cv: "0", exists: out exists, errorMsg: out errorMsg);
                        }
                    }
                }
                if (hasDependencies && errorMsg.Length == 0)
                {
                    errorMsg = "Record has dependencies and could not be permanently deleted. It has been archived instead.";
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            deleted = false;
            errorMsg = ex.Message;
        }

        result["exists"] = exists.ToString();
        result["hasDependencies"] = hasDependencies.ToString();
        result["deleted"] = deleted.ToString();
        result["archived"] = archived.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
    public static string SetFilterSession(bool getData, bool loadStartPage, string module, dynamic filters, bool myData)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "getData", getData.ToString() }, { "loadStartPage", loadStartPage.ToString() }, { "module", module }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        string name = module;
        if (module == "DeveloperReview" || module == "DailyReview" || module == "AoR")
        {
            name = "Work";
        }

        string sessionid = HttpContext.Current.Session.SessionID;
        try
        {
            if (filters != null)
            {
                dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>(filters);

                if (module == "Work" || module == "DeveloperReview" || module == "DailyReview" || module == "AoR" || module == "RQMT")
                {
                    saved = Filtering.SaveWorkFilters(module: module, filterModule: module, filters: fields, myData: myData, xml: "");
                }

                HttpContext.Current.Session["filters_" + name] = JsonConvert.DeserializeObject<Dictionary<string, object>>(filters);
            }
            else
            {
                saved = Filtering.SaveWorkFilters(module: module, filterModule: module, filters: null, myData: myData, xml: "");
                HttpContext.Current.Session["filters_" + name] = null;
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

        return JsonConvert.SerializeObject(result, Formatting.None);
    }
}