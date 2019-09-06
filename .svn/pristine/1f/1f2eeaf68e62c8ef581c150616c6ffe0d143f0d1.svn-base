using System;
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

public partial class MDGrid_AllocationGroup : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;
    protected bool _export = false;

    protected int _qfItemTypeID = 0;

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
    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        formatParent(ref copydt);
        String strName = "Master Grid - Allocation Group";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        wb.Worksheets.Add();
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[1];
        ws.Name = "Allocation Group";
        Worksheet wsRaw = wb.Worksheets[0];
        wsRaw.Name = "Allocation Group Raw";
        int rowCount = 0;
        DataTable Raw = null, parentRaw = null, childRaw = null;
        foreach (DataRow parentRow in copydt.Rows)
        {
            if (parentRow.Field<int>("ALLOCATIONGROUPID") != 0)
            {
                WTSUtility.importDataRow(ref parentRaw, parentRow);
                printParentHeader(ws, ref rowCount, copydt.Columns);
                printParent(parentRow, ws, ref rowCount);
                rowCount++;
                printChildRows(parentRow, ws, ref rowCount, ref childRaw);
                rowCount++;
            }
        }



        Raw = WTSUtility.JoinDataTables(parentRaw, childRaw, (row1, row2) =>
                      row1.Field<string>("ALLOCATIONGROUPID") == row2.Field<string>("Group ID"));

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
        int ID = parentRow.Field<int>("ALLOCATIONGROUPID");
        child = MasterData.AllocationGroup_Assignment_Get(parentID: ID);
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
        child.Columns.Remove("CREATEDBY");
        child.Columns.Remove("CREATEDDATE");
        child.Columns.Remove("UPDATEDBY");
        child.Columns.Remove("UPDATEDDATE");
        child.Columns.Remove("ALLOCATIONID");
        child.Columns.Remove("AllocationCategoryID");
        child.Columns.Remove("DefaultSMEID");
        child.Columns.Remove("DefaultBusinessResourceID");
        child.Columns.Remove("DefaultTechnicalResourceID");
        child.Columns.Remove("DefaultAssignedToID");
        child.Columns["ALLOCATIONGROUPID"].ColumnName = "Group ID";
        child.Columns["DESCRIPTION"].ColumnName = "Allocation Description";
        child.Columns["ARCHIVE"].ColumnName = "Allocation Archive";
        child.Columns["ALLOCATION"].ColumnName = "Allocation";
        child.Columns["SORT_ORDER"].ColumnName = "Allocation Sort Order";
        child.Columns["DefaultSME"].ColumnName = "SME";
        child.Columns["DefaultBusinessResource"].ColumnName = "Business Resource";
        child.Columns["DefaultTechnicalResource"].ColumnName = "Technical Resource";
        child.Columns["DefaultAssignedTo"].ColumnName = "Assigned To";
        child.Rows[0].Delete();
        child.AcceptChanges();
    }

    private void formatRaw(DataTable Raw)
    {
        if (Raw.Columns.Contains("ALLOCATIONGROUPID")) Raw.Columns.Remove("ALLOCATIONGROUPID");
        if (Raw.Columns.Contains("Group ID")) Raw.Columns.Remove("Group ID");
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
        dt.Columns.Remove("A");
        dt.Columns.Remove("CREATEDBY");
        dt.Columns.Remove("CREATEDDATE");
        dt.Columns.Remove("UPDATEDBY");
        dt.Columns.Remove("UPDATEDDATE");
        dt.Columns.Remove("CHILDCOUNT");
        dt.Columns["DESCRIPTION"].ColumnName = "Group Description";
        dt.Columns["ARCHIVE"].ColumnName = "Group Archive";
        dt.Columns["ALLOCATIONGROUP"].ColumnName = "Allocation Group";
        dt.Columns["NOTES"].ColumnName = "Notes";
        dt.Columns["PRIORTY"].ColumnName = "Group Priority";
        dt.Columns["DAILYMEETINGS"].ColumnName = "Daily Meetings";
    }
    private void readQueryString()
    {
        if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
            || Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
        {
            _refreshData = true;
        }
        if (Request.QueryString["AllocationGroup"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["AllocationGroup"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["AllocationGroup"].ToString()), out this._qfItemTypeID);
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
        if (_refreshData || Session["AllocationGroup"] == null)
        {
            dt = MasterData.AllocationGroup_Get();
            HttpContext.Current.Session["AllocationGroup"] = dt;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["AllocationGroup"];
        }

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
            spanRowCount.InnerText = dt.Rows.Count.ToString();

            

            InitializeColumnData(ref dt);
            dt.AcceptChanges();

            int count = dt.Rows.Count;
            count = count > 0 ? count - 1: count; 
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
                    case "A":
                        blnVisible = true;
						break;
                    case "ALLOCATIONGROUPID":
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "ALLOCATIONGROUP":
                        displayName = "Allocation Group";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DESCRIPTION":
                        displayName = "Description";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "NOTES":
                        displayName = "Notes";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "PRIORTY":
                        displayName = "Priorty";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DAILYMEETINGS":
                        displayName = "Daily Meetings";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ARCHIVE":
                        displayName = "Archive";
                        blnVisible = true;
                        blnSortable = false;
                        break;
                    case "CHILDCOUNT":
                        blnVisible = false;
                        blnSortable = false;
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

        row.Cells[DCC.IndexOf("A")].Text = "&nbsp;";
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
        string itemId = row.Cells[DCC.IndexOf("ALLOCATIONGROUPID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            row.Cells[DCC.IndexOf("ALLOCATIONGROUP")].Controls.Add(WTSUtility.CreateGridTextBox("ALLOCATIONGROUP", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ALLOCATIONGROUP")].Text.Trim())));
            row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("DESCRIPTION", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Trim())));
            row.Cells[DCC.IndexOf("NOTES")].Controls.Add(WTSUtility.CreateGridTextBox("NOTES", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("NOTES")].Text.Trim())));
            row.Cells[DCC.IndexOf("PRIORTY")].Controls.Add(WTSUtility.CreateGridTextBox("PRIORTY", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("PRIORTY")].Text.Trim()), true));
            bool dailyMeetings = false;
            if (row.Cells[DCC.IndexOf("DAILYMEETINGS")].HasControls()
                && row.Cells[DCC.IndexOf("DAILYMEETINGS")].Controls[0] is CheckBox)
            {
                dailyMeetings = ((CheckBox)row.Cells[DCC.IndexOf("DAILYMEETINGS")].Controls[0]).Checked;
            }
            else if (row.Cells[DCC.IndexOf("DAILYMEETINGS")].Text == "1")
            {
                dailyMeetings = true;
            }
            row.Cells[DCC.IndexOf("DAILYMEETINGS")].Controls.Clear();
            row.Cells[DCC.IndexOf("DAILYMEETINGS")].Controls.Add(WTSUtility.CreateGridCheckBox("DAILYMEETINGS", itemId, dailyMeetings));
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

        string dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("CHILDCOUNT")].Text).Trim();
        int count = 0;
        int.TryParse(dependencies, out count);

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
            divChildCount.InnerText = string.Format("({0})", count.ToString());
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
        if (HttpContext.Current.Session["AllocationGroup"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["AllocationGroup"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("ALLOCATIONGROUPID")
                && i != DCC.IndexOf("ALLOCATIONGROUP")
                && i != DCC.IndexOf("DESCRIPTION")
                && i != DCC.IndexOf("NOTES")
                && i != DCC.IndexOf("PRIORTY")
                && i != DCC.IndexOf("DAILYMEETINGS")
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
        row.Cells[DCC.IndexOf("A")].Style["width"] = "15px";
        row.Cells[DCC.IndexOf("ALLOCATIONGROUPID")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("ALLOCATIONGROUP")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("DESCRIPTION")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("NOTES")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("PRIORTY")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("DAILYMEETINGS")].Style["width"] = "80px";
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
        img.ToolTip = string.Format("{0} Systems for [{1}]", direction, itemId);
        img.AlternateText = string.Format("{0} Systems for [{1}]", direction, itemId);
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
            row.Attributes.Add("allocationId", itemId);
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
                else if (i == DCC["ALLOCATIONGROUP"].Ordinal)
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
        childFrame.Attributes.Add("systemId", itemId);
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["height"] = "30px";
        childFrame.Style["width"] = "100%";
        childFrame.Style["border"] = "none";

        return childFrame;
    }

    #endregion Grid

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

            int id = 0, priorty = 0, dailyMeetings = 0, archive = 0;
            string allocationGroup = string.Empty, description = string.Empty, notes = string.Empty;

            HttpServerUtility server = HttpContext.Current.Server;
            //save
            foreach (DataRow dr in dtjson.Rows)
            {
                id = 0;
                allocationGroup = string.Empty;
                description = string.Empty;
                notes = string.Empty;
                priorty = 0;
                dailyMeetings = 0;
                archive = 0;

                tempMsg = string.Empty;
                int.TryParse(dr["ALLOCATIONGROUPID"].ToString(), out id);
                allocationGroup = Uri.UnescapeDataString(dr["ALLOCATIONGROUP"].ToString());
                description = Uri.UnescapeDataString(dr["DESCRIPTION"].ToString());
                notes = Uri.UnescapeDataString(dr["NOTES"].ToString());
                int.TryParse(dr["PRIORTY"].ToString(), out priorty);
                int.TryParse(dr["DAILYMEETINGS"].ToString(), out dailyMeetings);
                int.TryParse(dr["ARCHIVE"].ToString(), out archive);

                if (string.IsNullOrWhiteSpace(allocationGroup))
                {
                    tempMsg = "You must specify a value for allocation group.";
                    saved = false;
                }
                else
                {
                    if (id == 0)
                    {
                        exists = false;
                        saved = MasterData.AllocationGroup_Add(allocationGroup, description, notes, priorty, dailyMeetings == 1, archive == 1, out exists, out id, out tempMsg);
                        if (exists)
                        {
                            saved = false;
                            tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Allocation Group record [" + allocationGroup + "].");
                        }
                    }
                    else
                    {
                        saved = MasterData.AllocationGroup_Update(id, allocationGroup, description, notes, priorty, dailyMeetings == 1, archive == 1, out tempMsg);
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
    public static string DeleteItem(int itemId)//, string item)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { 
            { "id", itemId.ToString() }
            , { "exists", "" }
            , { "hasDependencies", "" }
            , { "deleted", "" }
            , { "archived", "" }
            , { "error", "" } };
        bool exists = false, deleted = false, archived = false; //hasDependencies = false, deleted = false, archived = false;
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
                deleted = MasterData.AllocatonGroup_Delete(itemId, out exists, out archived, out errorMsg);
                
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
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }
}