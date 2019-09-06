﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Newtonsoft.Json;
using Aspose.Cells;
using System.IO;

public partial class MDGrid_ItemType : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();
    DataTable _dtPD2TDRPhase = null;
    DataTable _dtWorkloadAllocation = null;
    DataTable _dtWorkActivityGroup = null;

    protected bool _refreshData = false;
    protected bool _export = false;

    protected int _qfItemID = 0;

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder; 
    protected string SelectedColumnOrder;
    protected string ColumnOrder;

    protected bool CanView = false;
    protected bool CanEdit = false;
    protected bool IsAdmin = false;

    protected string cvValue = "2";

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
        if (Request.QueryString["Export"] != null
           && !string.IsNullOrWhiteSpace(Request.QueryString["Export"].ToString()) && Request.QueryString["Export"] == "1")
        {
            _export = true;
        }

        if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
    || Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
        {
            _refreshData = true;
        }
        if (Request.QueryString["TypeID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["TypeID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["TypeID"].ToString()), out this._qfItemID);
        }

        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["ChildView"]))
        {
            switch (Request.QueryString["ChildView"])
            {
                case "0":
                    cvValue = "0";
                    break;
                case "1":
                    cvValue = "1";
                    break;
                case "2":
                    cvValue = "2";
                    break;
                default:
                    cvValue = "2";
                    break;
            }
        }

        ddlChildView.Items.FindByValue(cvValue).Selected = true;
    }

    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        String strName = "Master Grid - Work Activity";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[0];
        formatTable(ref copydt);
        printHeader(ref copydt, ws);
        printTable(ref copydt, ws);
        ws.Cells.DeleteColumn(0);
        ws.AutoFitColumns();
        wb.Save(ms, SaveFormat.Xlsx);
        Response.BufferOutput = true;
        Response.ContentType = "application/xlsx";
        Response.AddHeader("content-disposition", "attachment;  filename=" + strName + ".xlsx");
        Response.BinaryWrite(ms.ToArray());
        Response.End();
    }

    private void printTable(ref DataTable copydt, Worksheet ws)
    {
        for (int i = 0, rowCount = 1; i < copydt.Rows.Count; i++)
        {
            if (copydt.Rows[i].Field<int>("WORKITEMTYPEID") != 0)
            {
                for (int j = 0; j < copydt.Columns.Count; j++)
                {
                    ws.Cells[rowCount, j].PutValue(copydt.Rows[i].ItemArray[j]);
                }
                rowCount++;
            }
        }
    }

    private void printHeader(ref DataTable copydt, Worksheet ws)
    {
        Aspose.Cells.Style style = new Aspose.Cells.Style();
        style.Pattern = BackgroundType.Solid;
        style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
        for (int i = 0; i < copydt.Columns.Count; i++)
        {
            ws.Cells[0, i].PutValue(copydt.Columns[i].ColumnName);
            ws.Cells[0, i].SetStyle(style);
        }
    }

    private void formatTable(ref DataTable copydt)
    {
        copydt.Columns["Item Type"].ColumnName = "Work Activity";
    }

    private void initControls()
    {
        grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
        grdMD.GridRowDataBound += grdMD_GridRowDataBound;
        grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
    }
    private void loadGridData()
    {
        DataTable dt = null;
        _dtPD2TDRPhase = MasterData.PhaseList_Get(includeArchive: false);
        _dtWorkloadAllocation = MasterData.WorkloadAllocationList_Get(includeArchive: 0);
        _dtWorkActivityGroup = MasterData.WorkActivityGroupList_Get(includeArchive: false);

        if (_refreshData || Session["dtMD_ItemType"] == null)
        {
            dt = MasterData.ItemTypeList_Get();
            HttpContext.Current.Session["dtMD_ItemType"] = dt;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["dtMD_ItemType"];
        }

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
            spanRowCount.InnerText = dt.Rows.Count.ToString();

            ddlQF.DataSource = dt;
            ddlQF.DataTextField = "Item Type";
            ddlQF.DataValueField = "WORKITEMTYPEID";
            ddlQF.DataBind();

            ListItem item = ddlQF.Items.FindByValue(_qfItemID.ToString());
            if (item != null)
            {
                item.Selected = true;
            }

            InitializeColumnData(ref dt);
            dt.AcceptChanges();

            if (_qfItemID != 0 && dt != null && dt.Rows.Count > 0)
            {
                dt.DefaultView.RowFilter = string.Format("WORKITEMTYPEID =  {0}", _qfItemID.ToString());
                dt = dt.DefaultView.ToTable();
            }
            int count = dt.Rows.Count;
            spanRowCount.InnerText = count.ToString();

            DataTable dtWorkActivity = dt.Copy();
            msWorkActivity.Items = dtWorkActivity;
        }

        if (_export && dt != null && CanView)
        {
            exportExcel(dt);
        }

        grdMD.DataSource = dt;
        grdMD.DataBind();
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
                    case "A":
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                    case "X":
                        displayName = "&nbsp";
                        blnVisible = true;
                        break;
                    case "WORKITEMTYPEID":
                        displayName = "&nbsp;";
                        blnVisible = false;
                        break;
                    case "Item Type":
                        displayName = "Work Activity";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Description":
                        displayName = "Description";
                        blnVisible = true;
                        break;
                    case "PDDTDR_PHASEID":
                        displayName = "Work Phase";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "WorkloadAllocationID":
                        displayName = "Workload Allocation";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "WorkActivityGroupID":
                        displayName = "Work Activity Group";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Sort Order":
                        displayName = "Sort Order";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Archive":
                        displayName = "Archive";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Work Items":
                        displayName = "Work Tasks";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Open Items":
                        displayName = "Open Work Tasks";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Closed Items":
                        displayName = "Closed Work Tasks";
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
    void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);
    }

    private void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("Item Type")
                && i != DCC.IndexOf("Description")
                && i != DCC.IndexOf("PDDTDR_PHASEID")
                && i != DCC.IndexOf("WorkloadAllocationID")
                && i != DCC.IndexOf("WorkActivityGroupID")
                && i != DCC.IndexOf("Sort Order"))
            {
                row.Cells[i].Style["text-align"] = "center";
                row.Cells[i].Style["padding-left"] = "0px";
            }
            else
            {
                row.Cells[i].Style["text-align"] = "left";
                row.Cells[i].Style["padding-left"] = "5px";
            }
        }

        //more column formatting
        row.Cells[DCC.IndexOf("A")].Style["width"] = "35px";
        row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("PDDTDR_PHASEID")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("WorkloadAllocationID")].Style["width"] = "135px";
        row.Cells[DCC.IndexOf("WorkActivityGroupID")].Style["width"] = "175px";
        row.Cells[DCC.IndexOf("Item Type")].Style["width"] = "275px";
        row.Cells[DCC.IndexOf("Work Items")].Style["width"] = "85px";
        row.Cells[DCC.IndexOf("Open Items")].Style["width"] = "85px";
        row.Cells[DCC.IndexOf("Closed Items")].Style["width"] = "85px";
        row.Cells[DCC.IndexOf("Sort Order")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("Archive")].Style["width"] = "55px";
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        string itemId = row.Cells[DCC.IndexOf("WORKITEMTYPEID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            row.Cells[DCC.IndexOf("Item Type")].Controls.Add(WTSUtility.CreateGridTextBox("Item Type", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Item Type")].Text.Trim())));
            row.Cells[DCC.IndexOf("Description")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Description")].Text.Trim())));
            row.Cells[DCC.IndexOf("PDDTDR_PHASEID")].Controls.Add(WTSUtility.CreateGridDropdownList(_dtPD2TDRPhase, "PDDTDR_PHASE", "PDDTDR_PHASE", "PDDTDR_PHASEID", itemId, row.Cells[DCC.IndexOf("PDDTDR_PHASEID")].Text.Replace("&nbsp;", " ").Trim(), " ", null));
            row.Cells[DCC.IndexOf("WorkloadAllocationID")].Controls.Add(WTSUtility.CreateGridDropdownList(_dtWorkloadAllocation, "WorkloadAllocation", "WorkloadAllocation", "WorkloadAllocationID", itemId, row.Cells[DCC.IndexOf("WorkloadAllocationID")].Text.Replace("&nbsp;", " ").Trim(), " ", null));
            row.Cells[DCC.IndexOf("WorkActivityGroupID")].Controls.Add(WTSUtility.CreateGridDropdownList(_dtWorkActivityGroup, "WorkActivityGroup", "WorkActivityGroup", "WorkActivityGroupID", itemId, row.Cells[DCC.IndexOf("WorkActivityGroupID")].Text.Replace("&nbsp;", " ").Trim(), " ", null));
            row.Cells[DCC.IndexOf("Sort Order")].Controls.Add(WTSUtility.CreateGridTextBox("Sort Order", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Sort Order")].Text.Trim()), true));
            bool archive = false;
            if (row.Cells[DCC.IndexOf("Archive")].HasControls()
                && row.Cells[DCC.IndexOf("Archive")].Controls[0] is CheckBox)
            {
                archive = ((CheckBox)row.Cells[DCC.IndexOf("Archive")].Controls[0]).Checked;
            }
            else if (row.Cells[DCC.IndexOf("Archive")].Text == "1")
            {
                archive = true;
            }
            row.Cells[DCC.IndexOf("Archive")].Controls.Clear();
            row.Cells[DCC.IndexOf("Archive")].Controls.Add(WTSUtility.CreateGridCheckBox("Archive", itemId, archive));
        }

        string dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("Work Items")].Text).Trim();
        int count = 0, childCount = 0;
        if (ddlChildView.SelectedValue == "0")
        {
            string statuses = Server.HtmlDecode(row.Cells[DCC.IndexOf("Status_Count")].Text).Trim();
            int.TryParse(statuses, out childCount);
        }
        else if (ddlChildView.SelectedValue == "1")
        {
            string resourceTypes = Server.HtmlDecode(row.Cells[DCC.IndexOf("Resource_Type_Count")].Text).Trim();
            int.TryParse(resourceTypes, out childCount);
        }
        else if (ddlChildView.SelectedValue == "2")
        {
            string resources = Server.HtmlDecode(row.Cells[DCC.IndexOf("Resource_Count")].Text).Trim();
            int.TryParse(resources, out childCount);
        }

        if (!CanEdit
            || !int.TryParse(dependencies, out count)
            || count > 0)
        {
            Image imgBlank = new Image();
            imgBlank.Height = 10;
            imgBlank.Width = 10;
            imgBlank.ImageUrl = "Images/Icons/blank.png";
            imgBlank.AlternateText = "";
            row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
        }
        else
        {
            row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("Item Type")].Text.Trim()));
        }

        if (!string.IsNullOrEmpty(itemId) && itemId != "0")
        {

            //add expand/collapse buttons
            HtmlGenericControl divChildren = new HtmlGenericControl();
            divChildren.Style["display"] = "table-row";
            divChildren.Style["text-align"] = "right";
            HtmlGenericControl divChildrenButtons = new HtmlGenericControl();
            divChildrenButtons.Style["display"] = "table-cell";
            divChildrenButtons.Controls.Add(createShowHideButton(true, "Show", itemId));
            divChildrenButtons.Controls.Add(createShowHideButton(false, "Hide", itemId));
            HtmlGenericControl divChildCount = new HtmlGenericControl();
            divChildCount.InnerText = string.Format("({0})", childCount.ToString());
            divChildCount.Style["display"] = "table-cell";
            divChildCount.Style["padding-left"] = "2px";
            divChildren.Controls.Add(divChildrenButtons);
            divChildren.Controls.Add(divChildCount);
            //buttons to show/hide child grid
            row.Cells[DCC["A"].Ordinal].Controls.Clear();
            row.Cells[DCC["A"].Ordinal].Controls.Add(divChildren);

            //add child grid row for Task Items
            Table table = (Table)row.Parent;
            GridViewRow childRow = createChildRow(itemId);
            table.Rows.AddAt(table.Rows.Count, childRow);
        }
    }
    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtMD_ItemType"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_ItemType"];
        }
    }

    Image createShowHideButton(bool show = false, string direction = "Show", string itemId = "ALL")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgShowHideChildren_click(this,'{0}','{1}');", direction, itemId);

        Image img = new Image();
        img.ID = string.Format("img{0}Children_{1}", direction, itemId);
        img.Style["display"] = show ? "block" : "none";
        img.Style["cursor"] = "pointer";
        img.Attributes.Add("Name", string.Format("img{0}", direction));
        img.Attributes.Add("itemId", itemId);
        img.Height = 10;
        img.Width = 10;
        img.ImageUrl = direction.ToUpper() == "SHOW"
            ? "Images/Icons/add_blue.png"
            : "Images/Icons/minus_blue.png";
        img.ToolTip = string.Format("{0} Items for [{1}]", direction, itemId);
        img.AlternateText = string.Format("{0} Items for [{1}]", direction, itemId);
        img.Attributes.Add("Onclick", sb.ToString());

        return img;
    }

    GridViewRow createChildRow(string itemId = "")
    {
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
        TableCell tableCell = null;

        try
        {
            row.CssClass = "gridBody";
            row.Style["display"] = "none";
            row.ID = string.Format("gridChild_{0}", itemId);
            row.Attributes.Add("workAreaId", itemId);
            row.Attributes.Add("Name", string.Format("gridChild_{0}", itemId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC["A"].Ordinal)
                {
                    //set width to match parent
                    tableCell.Style["width"] = "32px";
                    tableCell.Style["border-right"] = "1px solid transparent";

                }
                else if (i == DCC["Item Type"].Ordinal)
                {
                    tableCell.Style["padding-top"] = "10px";
                    tableCell.Style["padding-right"] = "10px";
                    tableCell.Style["padding-bottom"] = "0px";
                    tableCell.Style["padding-left"] = "0px";
                    tableCell.Style["vertical-align"] = "top";
                    tableCell.ColumnSpan = DCC.Count - 1;
                    //add the frame here
                    tableCell.Controls.Add(createChildFrame(itemId: itemId));
                }
                else
                {
                    tableCell.Style["display"] = "none";
                }

                row.Cells.Add(tableCell);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            row = null;
        }

        return row;
    }

    HtmlIframe createChildFrame(string itemId = "")
    {
        HtmlIframe childFrame = new HtmlIframe();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        childFrame.ID = string.Format("frameChild_{0}", itemId);
        childFrame.Attributes.Add("itemTypeId", itemId);
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["height"] = "30px";
        childFrame.Style["width"] = "100%";
        childFrame.Style["border"] = "none";

        return childFrame;
    }


    [WebMethod(true)]
    public static string SaveChanges(string rows)
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

            int id = 0, pddtdr = 0, workloadAllocation = 0, sortOrder = 0, archive = 0, workActivityGroupID = 0;
            string itemType = string.Empty, description = string.Empty;

            HttpServerUtility server = HttpContext.Current.Server;
            //save
            foreach (DataRow dr in dtjson.Rows)
            {
                id = 0;
                sortOrder = 0;
                pddtdr = 0;
                itemType = string.Empty;
                description = string.Empty;
                workActivityGroupID = 0;
                archive = 0;

                tempMsg = string.Empty;
                int.TryParse(dr["WORKITEMTYPEID"].ToString(), out id);
                int.TryParse(dr["PDDTDR_PHASEID"].ToString(), out pddtdr);
                int.TryParse(dr["WorkloadAllocationID"].ToString(), out workloadAllocation);
                itemType = Uri.UnescapeDataString(dr["Item Type"].ToString());
                description = Uri.UnescapeDataString(dr["Description"].ToString());
                int.TryParse(dr["WorkActivityGroupID"].ToString(), out workActivityGroupID);
                int.TryParse(dr["Sort Order"].ToString(), out sortOrder);
                int.TryParse(dr["Archive"].ToString(), out archive);

                if (string.IsNullOrWhiteSpace(itemType))
                {
                    tempMsg = "You must specify a value for Work Activity.";
                    saved = false;
                }
                else
                {
                    if (id == 0)
                    {
                        exists = false;
                        saved = MasterData.ItemType_Add(itemType, description, pddtdr, workloadAllocation, workActivityGroupID, sortOrder, archive == 1, out exists, out id, out tempMsg);
                        if (exists)
                        {
                            saved = false;
                            tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Work Activity record [" + itemType + "].");
                        }
                    }
                    else
                    {
                        saved = MasterData.ItemType_Update(id, itemType, description, pddtdr, workloadAllocation, workActivityGroupID, sortOrder, archive == 1, out tempMsg);
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
            LogUtility.LogException(ex);
            saved = false;
            errorMsg = ex.Message;
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
    public static string SaveResourceType(string workActivities)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            string[] WorkActivities = workActivities.Split(',');
            if (WorkActivities.Length == 0)
            {
                errorMsg = "Unable to save. An invalid list of Work Activities was provided.";
                saved = false;
            }

            int id = 0;
            HttpServerUtility server = HttpContext.Current.Server;
            //save
            for (int x = 0; x < WorkActivities.Length; x++)
            {
                int workActivityID = 0;
                int.TryParse(WorkActivities[x].ToString(), out workActivityID);

                if (workActivityID > 0)
                {
                    saved = MasterData.WORKITEMTYPE_ResourceType_Sync(WORKITEMTYPEID: workActivityID, exists: out exists, newID: out id, errorMsg: out tempMsg);

                    if (saved)
                    {
                        ids += string.Format("{0}{1}", x < WorkActivities.Length - 1 ? "," : "", id.ToString());
                    }
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
    public static string SaveResource(string workActivities)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            string[] WorkActivities = workActivities.Split(',');
            if (WorkActivities.Length == 0)
            {
                errorMsg = "Unable to save. An invalid list of Work Activities was provided.";
                saved = false;
            }

            int id = 0;
            HttpServerUtility server = HttpContext.Current.Server;
            //save
            for (int x = 0; x < WorkActivities.Length; x++)
            {
                int workActivityID = 0;
                int.TryParse(WorkActivities[x].ToString(), out workActivityID);

                if (workActivityID > 0)
                {
                    saved = MasterData.WORKITEMTYPE_Resource_Sync(WORKITEMTYPEID: workActivityID, exists: out exists, newID: out id, errorMsg: out tempMsg);

                    if (saved)
                    {
                        ids += string.Format("{0}{1}", x < WorkActivities.Length - 1 ? "," : "", id.ToString());
                    }
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
                deleted = MasterData.ItemType_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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
}