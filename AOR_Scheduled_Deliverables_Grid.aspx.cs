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


public partial class AOR_Scheduled_Deliverables_Grid : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();
    DataTable _dtUser = null;

    protected bool _refreshData = false;
    protected bool _export = false;

    protected int _qfSystemID = 0;
    protected int _qfReleaseID = 0;
    DataTable _dtStatus = null;

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;

    protected bool CanView = false;
    protected bool CanEdit = false;
    protected bool IsAdmin = false;
    protected bool CanEditWorkloadMGMT = false;

    protected string cvValue = "2";

    protected void Page_Load(object sender, EventArgs e)
    {
        this.IsAdmin = UserManagement.UserIsInRole("Admin");
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
        this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.MasterData));
        this.CanEditWorkloadMGMT = UserManagement.UserCanEdit(WTSModuleOption.WorkloadMGMT);

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
        if (Request.QueryString["SystemID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["SystemID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["SystemID"].ToString()), out this._qfSystemID);
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
                case "3":
                    cvValue = "3";
                    break;
                default:
                    cvValue = "2";
                    break;
            }
        }

        if (Request.QueryString["ReleaseID"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ReleaseID"].ToString()), out this._qfReleaseID);
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
        _dtStatus = MasterData.StatusList_Get(true);

        DataTable dt = null;
        if (_refreshData || Session["dtRelease"] == null)
        {
            dt = MasterData.ReleaseSchedule_ReleaseList_Get(includeArchive: false);
            HttpContext.Current.Session["dtRelease"] = dt;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["dtRelease"];
        }

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

            if (dt.Rows.Count > 0)
            {
                string filterSystem = !String.IsNullOrEmpty(_qfSystemID.ToString()) && _qfSystemID > 0 ? string.Format(" WTS_SystemID =  {0}", _qfSystemID.ToString()) : "";

                dt.DefaultView.RowFilter = filterSystem;
                dt = dt.DefaultView.ToTable();
            }

            InitializeColumnData(ref dt);
            dt.AcceptChanges();
        }

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
                blnVisible = false;
                blnSortable = false;
                blnOrderable = false;
                groupName = string.Empty;

                switch (gridColumn.ColumnName)
                {
                    case "X":
                        blnVisible = true;
                        break;
                    case "ProductVersion":
                        displayName = "Release Version";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DESCRIPTION":
                        displayName = "Description";
                        blnVisible = true;
                        break;
                    case "Narrative":
                        blnVisible = true;
                        break;
                    case "STATUS":
                        displayName = "Status";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "SORT_ORDER":
                        displayName = "Sort Order";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ARCHIVE":
                        displayName = "Archive";
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

    #region Grid

    void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
        row.Cells[DCC.IndexOf("X")].Style["width"] = "36px";
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        //add edit link
        string itemId = row.Cells[DCC.IndexOf("ProductVersionID")].Text.Trim();

        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        //if (CanView)
        //{
            //row.Cells[DCC.IndexOf("ProductVersion")].Controls.Add(WTSUtility.CreateGridTextBox("ProductVersion", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ProductVersion")].Text.Replace("&nbsp;", " ").Trim())));
            //row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;", " ").Trim())));
            //row.Cells[DCC.IndexOf("SORT_ORDER")].Controls.Add(WTSUtility.CreateGridTextBox("Sort_Order", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SORT_ORDER")].Text.Replace("&nbsp;", " ").Trim()), true));

            //Status
            //DropDownList ddl = WTSUtility.CreateGridDropdownList(_dtStatus, "Status", "Status", "StatusID", itemId, row.Cells[DCC.IndexOf("StatusID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("Status")].Text.Replace("&nbsp;", " ").Trim(), null);
            //row.Cells[DCC.IndexOf("Status")].Controls.Add(ddl);

            //bool chked = false;
            //if (row.Cells[DCC.IndexOf("ARCHIVE")].HasControls()
            //    && row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0] is CheckBox)
            //{
            //    chked = ((CheckBox)row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0]).Checked;
            //}
            //else if (row.Cells[DCC.IndexOf("ARCHIVE")].Text == "1")
            //{
            //    chked = true;
            //}
            //row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Clear();
            //row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Add(WTSUtility.CreateGridCheckBox("Archive", itemId, chked));

        //}

        TextBox obj = WTSUtility.CreateGridTextBox("Narrative", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Narrative")].Text.Replace("&nbsp;", " ").Trim()), false, true);
        obj.ReadOnly = true;
        obj.ForeColor = System.Drawing.Color.Gray;
        row.Cells[DCC.IndexOf("Narrative")].Controls.Add(obj);

        string dependencies = "0";
        int count = 0, deliverableCount = 0;
        int.TryParse(dependencies, out count);
        string deliverables = "0";
        DataTable dtDeliverable = MasterData.ReleaseSchedule_DeliverableList_Get(int.Parse(row.Cells[DCC.IndexOf("ProductVersionID")].Text));
        if (dtDeliverable != null) deliverables = dtDeliverable.Rows.Count.ToString();
        int.TryParse(deliverables, out deliverableCount);
        deliverableCount--;

        if (!CanEdit
            || count > 0
            || deliverableCount > 0)
        {
            //don't allow delete
            Image imgBlank = new Image();
            imgBlank.Height = 10;
            imgBlank.Width = 10;
            imgBlank.ImageUrl = "Images/Icons/blank.png";
            imgBlank.AlternateText = "";
            row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
        }
        else
        {
            //add delete button
            row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("ProductVersion")].Text.Trim()));
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
            divChildCount.InnerText = string.Format("({0})", deliverableCount.ToString());
            divChildCount.Style["display"] = "table-cell";
            divChildCount.Style["padding-left"] = "2px";
            divChildren.Controls.Add(divChildrenButtons);
            divChildren.Controls.Add(divChildCount);
            //buttons to show/hide child grid
            row.Cells[DCC["X"].Ordinal].Controls.Clear();
            row.Cells[DCC["X"].Ordinal].Controls.Add(divChildren);

            //add child grid row for Task Items
            Table table = (Table)row.Parent;
            GridViewRow childRow = createChildRow(itemId);
            table.Rows.AddAt(table.Rows.Count, childRow);
        }
    }

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtRelease"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtRelease"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("ProductVersion")
                && i != DCC.IndexOf("Status")
                && i != DCC.IndexOf("SORT_ORDER")
                && i != DCC.IndexOf("ARCHIVE"))
            {
                row.Cells[i].Style["text-align"] = "left";
                row.Cells[i].Style["padding-left"] = "5px";
            }
            else
            {
                row.Cells[i].Style["text-align"] = "center";
                row.Cells[i].Style["padding-left"] = "0px";
            }
        }

        //more column formatting
        row.Cells[DCC.IndexOf("X")].Style["padding-left"] = "4px";
        row.Cells[DCC.IndexOf("ProductVersion")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("ProductVersion")].Enabled = false;
        row.Cells[DCC.IndexOf("Description")].Enabled = false;
        row.Cells[DCC.IndexOf("DESCRIPTION")].Style["width"] = "400px";
        row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("SORT_ORDER")].Enabled = false;
        row.Cells[DCC.IndexOf("Status")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("Status")].Enabled = false;
        row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
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
            row.Attributes.Add("ProductVersionID", itemId);
            row.Attributes.Add("Name", string.Format("gridChild_{0}", itemId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC["X"].Ordinal)
                {
                    //set width to match parent
                    tableCell.Style["width"] = "32px";
                    tableCell.Style["border-right"] = "1px solid transparent";
                }
                else if (i == DCC["ProductVersion"].Ordinal)
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
        childFrame.Attributes.Add("ProductVersionID", itemId);
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["height"] = "30px";
        childFrame.Style["width"] = "100%";
        childFrame.Style["border"] = "none";

        return childFrame;
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
                if (cols.ItemByColumnName(col.ColumnName) == null && col.ColumnName != "ProductVersionID" && col.ColumnName.Trim() != "")
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
        dt.Columns.Add("  ");
        if (dt.Columns.Contains("ProductVersionID")) dt.Columns["ProductVersionID"].SetOrdinal(dt.Columns.Count - 1);

        dt.AcceptChanges();
    }

    static void formatParent(ref DataTable dt)
    {
        dt.Columns.Remove(dt.Columns[9]);
        dt.Columns.Remove(dt.Columns[9]);
        dt.Columns.Remove("Work Areas");
        dt.Columns["Archive"].ColumnName = "System Archive";
        dt.Columns["Description"].ColumnName = "System Description";
    }

    static void formatChild(ref DataTable dt, string cvValue)
    {
        if (cvValue == "1")
        {
            dt.Columns.Remove("ProductVersion");
            dt.Columns.Remove("X");
            dt.Columns.Remove("CREATEDBY");
            dt.Columns.Remove("CREATEDDATE");
            dt.Columns.Remove("UPDATEDBY");
            dt.Columns.Remove("UPDATEDDATE");
            dt.Columns["ARCHIVE"].ColumnName = "Allocation Archive";
            dt.Columns["DESCRIPTION"].ColumnName = "Allocation Description";
        }
        else if (cvValue == "0")
        {
            dt.Columns.Remove("ProductVersion");
            dt.Columns.Remove("X");
            dt.Columns.Remove("CREATEDBY");
            dt.Columns.Remove("CREATEDDATE");
            dt.Columns.Remove("UPDATEDBY");
            dt.Columns.Remove("UPDATEDDATE");
            dt.Columns["ARCHIVE"].ColumnName = "Work Area Archive";
            dt.Columns["DESCRIPTION"].ColumnName = "Work Area Description";
        }
    }

    static void formatRaw(ref DataTable dt)
    {
        if (dt.Columns.Contains("ProductVersionID")) dt.Columns.Remove("ProductVersionID");
    }

    private bool ExportExcel(DataTable dt)
    {
        bool success = false;
        string errorMsg = string.Empty;

        try
        {
            Workbook wb = new Workbook(FileFormatType.Xlsx);
            Worksheet ws = wb.Worksheets[0];
            wb.Worksheets.Add();
            Worksheet wsRaw = wb.Worksheets[1];
            StyleFlag flag = new StyleFlag() { All = true };
            Aspose.Cells.Style style = new Aspose.Cells.Style();
            Aspose.Cells.Style style2 = new Aspose.Cells.Style();

            style.Pattern = BackgroundType.Solid;
            style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
            style2.Pattern = BackgroundType.Solid;
            style2.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightBlue");

            DataTable dtExcel_Formatted = new DataTable();
            DataTable dtRaw = new DataTable();
            DataTable systemRaw = new DataTable();
            DataTable workAreaRaw = new DataTable();

            FormatExcelRows(ref dt);
            RemoveExcelColumns(ref dt);
            RenameExcelColumns(ref dt);
            dtExcel_Formatted = dt.Clone();

            for (int i = 0; i <= dtExcel_Formatted.Columns.Count - 1; i++)
            {
                dtExcel_Formatted.Columns[i].DataType = typeof(string);
                dtExcel_Formatted.Columns[i].MaxLength = 255;
                dtExcel_Formatted.Columns[i].AllowDBNull = true;
            }

            foreach (DataRow dr in dt.Rows)
            {
                WTSUtility.importDataRow(ref systemRaw, dr);
                int wtsSystemID = 0;
                int.TryParse(dr["WTS_SystemID"].ToString(), out wtsSystemID);

                if (wtsSystemID > 0)
                {
                    dtExcel_Formatted.ImportRow(dr);
                    DataTable dtDrilldown = MasterData.WorkArea_SystemList_Get(systemID: wtsSystemID, cv: cvValue);

                    if (dtDrilldown != null && dtDrilldown.Rows.Count > 1)
                    {
                        int count = 0;
                        foreach (DataRow drChild in dtDrilldown.Rows)
                        {
                            WTSUtility.importDataRow(ref workAreaRaw, drChild);
                            if (count == 0)
                            {
                                DataRow drSpacer = dtExcel_Formatted.NewRow();
                                drSpacer[0] = "spacer";

                                dtExcel_Formatted.Rows.Add(drSpacer);
                                if (cvValue == "0") { dtExcel_Formatted.Rows.Add("", "System(Task)", "Work Area", "Description", "Proposed Priority", "Approved Priority", "Tasks", "Archive"); }
                                else { dtExcel_Formatted.Rows.Add("", "System(Task)", "Allocation Group", "Allocation", "Description", "Proposed Priority", "Approved Priority", "Tasks", "Archive"); }
                            }

                            int drilldownWorkAreaID = 0;

                            if (cvValue == "0")
                            {
                                int.TryParse(drChild["WorkAreaID"].ToString(), out drilldownWorkAreaID);
                                if (drilldownWorkAreaID > 0) dtExcel_Formatted.Rows.Add("", drChild["WTS_SYSTEM"].ToString(), drChild["WorkArea"].ToString(), drChild["DESCRIPTION"].ToString(), drChild["ProposedPriority"].ToString(), drChild["ApprovedPriority"].ToString(), drChild["WorkItem_Count"].ToString(), (drChild["ARCHIVE"].ToString() == "1" ? "Yes" : "No"));
                            }
                            else
                            {
                                int.TryParse(drChild["ALLOCATIONID"].ToString(), out drilldownWorkAreaID);
                                if (drilldownWorkAreaID > 0) dtExcel_Formatted.Rows.Add("", drChild["WTS_SYSTEM"].ToString(), drChild["AllocationGroup"].ToString(), drChild["ALLOCATION"].ToString(), drChild["DESCRIPTION"].ToString(), drChild["ProposedPriority"].ToString(), drChild["ApprovedPriority"].ToString(), drChild["WorkItem_Count"].ToString(), (drChild["ARCHIVE"].ToString() == "1" ? "Yes" : "No"));
                            }

                            if (count == dtDrilldown.Rows.Count - 1)
                            {
                                DataRow drSpacer = dtExcel_Formatted.NewRow();
                                drSpacer[0] = "spacer";
                                dtExcel_Formatted.Rows.Add(drSpacer);
                            }
                            count++;
                        }
                    }
                }
            }

            if (dtExcel_Formatted.Columns.Contains("WTS_SystemID")) dtExcel_Formatted.Columns.Remove("WTS_SystemID");

            string name = "Master Grid - System(Task)";
            ws.Cells.ImportDataTable(dtExcel_Formatted, true, 0, 0, false, false);

            for (int j = 0; j <= ws.Cells.Rows.Count - 1; j++)
            {
                if (ws.Cells.Rows[j][0].Value == "spacer")
                {
                    ws.Cells.Rows[j][0].Value = "";
                    Range spacer = ws.Cells.CreateRange(j, 0, 1, 8);
                    spacer.Merge();
                }

                if (ws.Cells.Rows[j][0].Value == "System(Task)")
                {
                    Range range = ws.Cells.CreateRange(j, 0, 1, 7);
                    range.ApplyStyle(style, flag);
                }

                if (ws.Cells.Rows[j][1].Value == "System(Task)")
                {
                    Range range = ws.Cells.CreateRange(j, 1, 1, 8);
                    range.ApplyStyle(style2, flag);
                }
            }

            formatParent(ref systemRaw);
            formatChild(ref workAreaRaw, cvValue);

            dtRaw = WTSUtility.JoinDataTables(systemRaw, workAreaRaw, (row1, row2) =>
                       row1.Field<string>("WTS_SystemID") == row2.Field<string>("WTS_SYSTEMID"));

            formatRaw(ref dtRaw);

            wsRaw.Cells.ImportDataTable(dtRaw, true, 0, 0, false, false);
            wsRaw.AutoFitColumns();
            ws.AutoFitColumns();
            ws.Name = "Master Grid -System";
            wsRaw.Name = "Systems Raw";
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

    #endregion Grid


    [WebMethod(true)]
    public static string SaveChanges(string rows)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
            , { "failed", "0" }
            , { "savedIds", "" }
            , { "failedIds", "" }
            , { "error", "" } };
        bool exists = false, saved = false, duplicate = false;
        int savedQty = 0, failedQty = 0;
        string ids = string.Empty, failedIds = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
            if (dtjson.Rows.Count == 0)
            {
                errorMsg = "Unable to save. An invalid list of changes was provided.";
            }
            else
            {
                int id = 0, sortOrder = 0, busWorkloadManagerID = 0, devWorkloadManagerID = 0, archive = 0;
                string system = string.Empty, description = string.Empty;

                HttpServerUtility server = HttpContext.Current.Server;
                //save
                foreach (DataRow dr in dtjson.Rows)
                {
                    id = sortOrder = 0;
                    system = description = string.Empty;
                    archive = 0;

                    tempMsg = string.Empty;
                    int.TryParse(dr["WTS_SystemID"].ToString(), out id);
                    system = server.UrlDecode(dr["WTS_System"].ToString());
                    description = server.UrlDecode(dr["DESCRIPTION"].ToString());
                    int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
                    int.TryParse(dr["BusWorkloadManager"].ToString(), out busWorkloadManagerID);
                    int.TryParse(dr["DevWorkloadManager"].ToString(), out devWorkloadManagerID);
                    int.TryParse(dr["ARCHIVE"].ToString(), out archive);

                    if (string.IsNullOrWhiteSpace(system))
                    {
                        if (id == 0)
                        {
                            continue;
                        }
                        tempMsg = "You must specify a value for System.";
                        saved = false;
                    }
                    else
                    {
                        if (id == 0)
                        {
                            exists = false;
                            saved = MasterData.System_Add(system, description, sortOrder, busWorkloadManagerID, devWorkloadManagerID, archive == 1, out exists, out id, out tempMsg);
                            if (exists)
                            {
                                saved = false;
                                tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate System record [" + system + "].");
                            }
                        }
                        else
                        {
                            saved = MasterData.System_Update(id, system, description, sortOrder, busWorkloadManagerID, devWorkloadManagerID, archive == 1, out tempMsg);
                        }
                    }

                    if (saved)
                    {
                        savedQty += 1;
                        ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
                    }
                    else
                    {
                        failedQty += 1;
                        failedIds += string.Format("{0}{1}", failedIds.Length > 0 ? "," : "", failedIds.ToString());
                    }

                    if (tempMsg.Length > 0)
                    {
                        errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            saved = false;
            errorMsg = ex.Message;
        }

        result["savedIds"] = ids;
        result["failedIds"] = failedIds;
        result["saved"] = savedQty.ToString();
        result["failed"] = failedQty.ToString();
        result["error"] = errorMsg;

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
                deleted = MasterData.System_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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