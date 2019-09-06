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

public partial class MDGrid_WorkloadAllocation : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();
    protected DataTable _dtContract = new DataTable();

    protected bool _refreshData = false;
    protected bool _export = false;

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;

    protected bool CanView = false;
    protected bool CanEdit = false;
    protected bool IsAdmin = false;

    protected string cvValue = "0";
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

        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }

        if (Request.QueryString["Export"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["Export"].ToString()) && Request.QueryString["Export"] == "1")
        {
            _export = true;
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
                default:
                    cvValue = "0";
                    break;
            }
        }

        ddlChildView.Items.FindByValue(cvValue).Selected = true;
    }

    private void initControls()
    {
        grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
        grdMD.GridRowDataBound += grdMD_GridRowDataBound;
        grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
    }

    private void loadGridData()
    {
        _dtContract = MasterData.ContractList_Get();
        DataTable dt = null;
        if (_refreshData || Session["WorkloadAllocation"] == null)
        {
            dt = MasterData.WorkloadAllocationList_Get(includeArchive: 1);
            HttpContext.Current.Session["WorkloadAllocation"] = dt;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["WorkloadAllocation"];
        }

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
            spanRowCount.InnerText = dt.Rows.Count.ToString();

            InitializeColumnData(ref dt);
            dt.AcceptChanges();

            int count = dt.Rows.Count;
            count = count > 0 ? count - 1 : count;
            spanRowCount.InnerText = count.ToString();
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
                    //case "X":
                    //    blnVisible = true;
                    //    break;
                    case "WorkloadAllocationID":
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "Abbreviation":
                        displayName = "Abbr.";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "WorkloadAllocation":
                        displayName = "Workload Allocation";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DESCRIPTION":
                        displayName = "Description";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    //case "Contract":
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    break;
                    //case "ContractDescription":
                    //    displayName = "Contract Description";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    break;
                    case "SORT":
                        displayName = "Sort Order";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ARCHIVE":
                        displayName = "Archive";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Y":
                        blnVisible = true;
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
        row.Cells[DCC.IndexOf("Y")].Text = "&nbsp;";
        row.Cells[DCC.IndexOf("ContractDescription")].Style["text-align"] = "center";

        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (row.RowIndex > 0)
            {
                row.Cells[i].Style["border-top"] = "1px solid grey";
            }
        }
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        //add edit link
        string itemId = row.Cells[DCC.IndexOf("WorkloadAllocationID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            TextBox abbr = WTSUtility.CreateGridTextBox("Abbreviation", itemId, row.Cells[DCC.IndexOf("Abbreviation")].Text.Replace("&nbsp;", "").Trim());
            abbr.MaxLength = 2;
            row.Cells[DCC.IndexOf("Abbreviation")].Controls.Add(abbr);
            row.Cells[DCC.IndexOf("WorkloadAllocation")].Controls.Add(WTSUtility.CreateGridTextBox("WorkloadAllocation", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkloadAllocation")].Text.Trim())));
            row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("DESCRIPTION", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Trim())));

            DropDownList ddlContract = WTSUtility.CreateGridDropdownList(_dtContract, "Contract", "Contract", "ContractID", itemId, row.Cells[DCC.IndexOf("ContractID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("Contract")].Text.Replace("&nbsp;", " ").Trim(), null);
            ddlContract.Attributes.Add("field", "Contract");
            row.Cells[DCC.IndexOf("CONTRACT")].Controls.Add(ddlContract);

            row.Cells[DCC.IndexOf("SORT")].Controls.Add(WTSUtility.CreateGridTextBox("SORT", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SORT")].Text.Trim()), true));
            bool archive = false;
            if (row.Cells[DCC.IndexOf("ARCHIVE")].HasControls()
                && row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0] is CheckBox)
            {
                archive = ((CheckBox)row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0]).Checked;
            }
            else if (row.Cells[DCC.IndexOf("ARCHIVE")].Text == "1")
            {
                archive = true;
            }
            row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Clear();
            row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Add(WTSUtility.CreateGridCheckBox("Archive", itemId, archive));
        }

        string dependencies;
        string childCounts;
        int childCount = 0;
        if (ddlChildView.SelectedValue == "0")
        {
            dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("Status_Count")].Text).Trim();
            int.TryParse(dependencies, out childCount);
        }
        else if (ddlChildView.SelectedValue == "1")
        {
            childCounts = Server.HtmlDecode(row.Cells[DCC.IndexOf("Contract_Count")].Text).Trim();
            int.TryParse(childCounts, out childCount);
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
        if (HttpContext.Current.Session["WorkloadAllocation"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["WorkloadAllocation"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("WorkloadAllocationID")
                && i != DCC.IndexOf("Abbreviation")
                && i != DCC.IndexOf("WorkloadAllocation")
                && i != DCC.IndexOf("DESCRIPTION")
                && i != DCC.IndexOf("Contract")
                && i != DCC.IndexOf("SORT")
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
        row.Cells[DCC.IndexOf("X")].Style["width"] = "35px";
        row.Cells[DCC.IndexOf("WorkloadAllocationID")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Abbreviation")].Style["width"] = "40px";
        row.Cells[DCC.IndexOf("WorkloadAllocation")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("Contract")].Style["width"] = "250px";
        row.Cells[DCC.IndexOf("ContractDescription")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("SORT")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
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
        img.ToolTip = string.Format("{0} - for [{1}]", direction, itemId);
        img.AlternateText = string.Format("{0} - for [{1}]", direction, itemId);
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
            row.Attributes.Add("WorkloadAllocationID", itemId);
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
                else if (i == DCC["WorkloadAllocation"].Ordinal)
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
        childFrame.Attributes.Add("WorkloadAllocationID", itemId);
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["height"] = "30px";
        childFrame.Style["width"] = "100%";
        childFrame.Style["border"] = "none";

        return childFrame;
    }

    #endregion Grid

    #region excel
    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        formatParent(ref copydt);
        String strName = "Master Grid - Workload Allocation";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        wb.Worksheets.Add();
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[1];
        ws.Name = "Workload Allocation";
        Worksheet wsRaw = wb.Worksheets[0];
        wsRaw.Name = "Workload Allocation Raw";
        int rowCount = 0;
        DataTable Raw = null, parentRaw = null, childRaw = null;
        foreach (DataRow parentRow in copydt.Rows)
        {
            if (parentRow.Field<int>("WorkloadAllocationID") != 0)
            {
                WTSUtility.importDataRow(ref parentRaw, parentRow);
                printParentHeader(ws, ref rowCount, copydt.Columns);
                printParent(parentRow, ws, ref rowCount);
                rowCount++;
                /*printChildRows(parentRow, ws, ref rowCount, ref childRaw);
                rowCount++;*/
            }
        }

        Raw = WTSUtility.JoinDataTables(parentRaw, childRaw, (row1, row2) =>
                      row1.Field<string>("WorkloadAllocationID") == row2.Field<string>("WorkloadAllocationID"));

        formatRaw(Raw);
        ws.Cells.DeleteColumn(0);
        ws.AutoFitColumns();
        wsRaw.Cells.ImportDataTable(Raw, true, "A1");
        wsRaw.AutoFitColumns();
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
    private void printChildRows(DataRow parentRow, Worksheet ws, ref int rowCount, ref DataTable childRaw)
    {
        DataTable child = null;
        int ID = parentRow.Field<int>("WorkloadAllocationID");
        child = MasterData.SystemList_Get();
        child.DefaultView.RowFilter = "WorkloadAllocationID = " + ID;
        child = child.DefaultView.ToTable();
        int i = 0, j = 1;
        formatChild(child);
        printChildHeader(ws, ref rowCount, child.Columns);
        foreach (DataRow row in child.Rows)
        {
            WTSUtility.importDataRow(ref childRaw, row);
            j = 2;
            foreach (object value in row.ItemArray)
            {
                ws.Cells[rowCount + i, j].PutValue(value);
                j++;
            }
            i++;
        }
        rowCount += child.Rows.Count;
    }

    private void formatChild(DataTable child)
    {
        child.Columns.Remove("A");
        child.Columns.Remove("X");
        child.Columns.Remove("CREATEDBY");
        child.Columns.Remove("CREATEDDATE");
        child.Columns.Remove("UPDATEDBY");
        child.Columns.Remove("UPDATEDDATE");
        child.Columns["WorkloadAllocation"].ColumnName = "WorkloadAllocation";
        child.AcceptChanges();
    }

    private void formatRaw(DataTable Raw)
    {
        if (Raw.Columns.Contains("WorkloadAllocationID")) Raw.Columns.Remove("WorkloadAllocationID");
        if (Raw.Columns.Contains("WorkloadAllocation")) Raw.Columns.Remove("WorkloadAllocation");
        Raw.AcceptChanges();
    }

    private void printChildHeader(Worksheet ws, ref int rowCount, DataColumnCollection columns)
    {
        if (object.ReferenceEquals(columns, null) || columns.Count < 1) { return; }
        Aspose.Cells.Style style = new Aspose.Cells.Style();
        style.Pattern = BackgroundType.Solid;
        style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("lightBlue");
        for (int i = 0; i < columns.Count; i++)
        {
            ws.Cells[rowCount, i + 2].PutValue(columns[i].ColumnName);
            ws.Cells[rowCount, i + 2].SetStyle(style);
        }
        rowCount++;
    }

    private void printParent(DataRow parentRow, Worksheet ws, ref int rowCount)
    {
        int i = 0;
        foreach (object value in parentRow.ItemArray)
        {
            ws.Cells[rowCount, i].PutValue(value);
            i++;
        }
        rowCount++;
    }

    private static void formatParent(ref DataTable dt)
    {
        dt.Columns.Remove("X");
        dt.Columns.Remove("CREATEDBY");
        dt.Columns.Remove("CREATEDDATE");
        dt.Columns.Remove("UPDATEDBY");
        dt.Columns.Remove("UPDATEDDATE");
        dt.Columns["DESCRIPTION"].ColumnName = "Workload Allocation Description";
        dt.Columns["ARCHIVE"].ColumnName = "Workload Allocation Archive";
        dt.Columns["WorkloadAllocation"].ColumnName = "Workload Allocation";
        dt.Columns["SORT"].ColumnName = "Workload Allocation Sort Order";
    }

    #endregion excel

    [WebMethod(true)]
    public static string SaveChanges(string rows)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;
        int newID = 0;

        try
        {
            DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
            if (dtjson.Rows.Count == 0)
            {
                errorMsg = "Unable to save. An invalid list of changes was provided.";
                saved = false;
            }

            int id = 0, contract, SortOrder = 0, archive = 0;
            string workloadAllocation = string.Empty, abbreviation = string.Empty, description = string.Empty;

            HttpServerUtility server = HttpContext.Current.Server;

            foreach (DataRow dr in dtjson.Rows)
            {
                id = 0;
                workloadAllocation = string.Empty;
                abbreviation = string.Empty;
                description = string.Empty;
                contract = 0;
                SortOrder = 0;
                archive = 0;

                tempMsg = string.Empty;
                int.TryParse(dr["WorkloadAllocationID"].ToString(), out id);
                workloadAllocation = Uri.UnescapeDataString(dr["WorkloadAllocation"].ToString());
                abbreviation = Uri.UnescapeDataString(dr["Abbreviation"].ToString());
                description = Uri.UnescapeDataString(dr["DESCRIPTION"].ToString());
                int.TryParse(dr["Contract"].ToString(), out contract);
                int.TryParse(dr["SORT"].ToString(), out SortOrder);
                int.TryParse(dr["ARCHIVE"].ToString(), out archive);

                if (string.IsNullOrWhiteSpace(workloadAllocation))
                {
                    if (id != 0)
                    {
                        tempMsg = "You must specify a value for the workload allocation.";
                        saved = false;
                    }
                }
                else
                {
                    if (id == 0)
                    {
                        exists = false;
                        saved = MasterData.WorkloadAllocation_Add(workloadAllocation, abbreviation, description, contract, SortOrder, archive == 1 ? true : false, out exists, out newID, out tempMsg);
                        if (exists)
                        {
                            saved = false;
                            tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate workload Allocation record [" + workloadAllocation + "].");
                        }
                    }
                    else if (id > 0)
                    {
                        saved = MasterData.WorkloadAllocation_Update(id, workloadAllocation, abbreviation, description, contract, SortOrder, archive == 1 ? true : false, out tempMsg);
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


    //TODO Handle Delete
    [WebMethod(true)]
    public static string DeleteItem(int itemId)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() {
            { "id", itemId.ToString() }
            , { "exists", "" }
            , { "hasDependencies", "" }
            , { "deleted", "" }
            , { "archived", "" }
            , { "error", "" } };
        bool exists = false, deleted = false, archived = false, hasDependencies = false;
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
                deleted = MasterData.WorkloadAllocation_Delete(itemId, out exists, out archived, out hasDependencies, out errorMsg);

            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            deleted = false;
            errorMsg = ex.Message;
        }

        result["exists"] = exists.ToString();
        result["deleted"] = deleted.ToString();
        result["archived"] = archived.ToString();
        result["hasDependencies"] = hasDependencies.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }
}