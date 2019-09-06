﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
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

public partial class MDGrid_Image : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

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

    protected int ImageID = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.IsAdmin = UserManagement.UserIsInRole("Admin");
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
        this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.MasterData));

        readQueryString();

        initControls();

        if (this.ImageID > 0)
        {
            DataTable dt = MasterData.Image_Get(ImageID: this.ImageID);

            if (dt == null || dt.Rows.Count == 0 || DBNull.Value.Equals(dt.Rows[0]["FileData"]))
            {
                return;
            }

            byte[] fileData = (byte[])dt.Rows[0]["FileData"];

            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + dt.Rows[0]["FileName"].ToString() + "\"");
            Response.AddHeader("Content-Length", fileData.Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.OutputStream.Write(fileData, 0, fileData.Length);
            Response.End();
        }
        else
        {
            if (!Page.IsPostBack) loadGridData();
        }
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

        if (Request.QueryString["ImageID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ImageID"]))
        {
            int.TryParse(Request.QueryString["ImageID"], out this.ImageID);
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
        if (_refreshData || Session["Image"] == null)
        {
            dt = MasterData.ImageList_Get();
            HttpContext.Current.Session["Image"] = dt;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["Image"];
        }

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
            spanRowCount.InnerText = dt.Rows.Count.ToString();



            InitializeColumnData(ref dt);
            dt.AcceptChanges();
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
                    case "ImageName":
                        displayName = "Image Name";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Description":
                        blnVisible = true;
                        break;
                    case "FileName":
                        displayName = "File";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Image":
                        blnVisible = true;
                        break;
                    case "Sort":
                        displayName = "Sort Order";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Archive":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Z":
                        displayName = "";
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
        string itemId = row.Cells[DCC.IndexOf("ImageID")].Text.Trim();
        if (itemId == "-1" || itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanEdit)
        {
            row.Cells[DCC.IndexOf("ImageName")].Controls.Add(WTSUtility.CreateGridTextBox("ImageName", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ImageName")].Text.Trim())));
            row.Cells[DCC.IndexOf("Sort")].Controls.Add(WTSUtility.CreateGridTextBox("Sort", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Sort")].Text.Trim()), true));
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

        TextBox obj = WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Description")].Text.Replace("&nbsp;", " ").Trim()), false, true);
        if (!CanView)
        {
            obj.ReadOnly = true;
            obj.ForeColor = System.Drawing.Color.Gray;
        }
        row.Cells[DCC.IndexOf("Description")].Controls.Add(obj);

        if (CanView)
        {
            row.Cells[DCC.IndexOf("FileName")].Controls.Add(CreateLink(itemId, row.Cells[DCC.IndexOf("FileName")].Text));
        }

        row.Cells[DCC.IndexOf("Image")].Controls.Add(CreateImage(row.Cells[DCC.IndexOf("ImageID")].Text, row.Cells[DCC.IndexOf("FileName")].Text));

        string dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("Contract_Count")].Text).Trim();
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
        if (HttpContext.Current.Session["Image"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["Image"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("X")
                && i != DCC.IndexOf("ImageName")
                && i != DCC.IndexOf("Description")
                && i != DCC.IndexOf("FileName")
                && i != DCC.IndexOf("Image")
                && i != DCC.IndexOf("Sort")
                && i != DCC.IndexOf("Archive"))
            {
                row.Cells[i].Style["text-align"] = "left";
                row.Cells[i].Style["padding-left"] = "5px";
            }
            else
            {
                row.Cells[i].Style["text-align"] = "center";
                row.Cells[i].Style["vertical-align"] = "top";
                row.Cells[i].Style["padding-top"] = "5px";
            }
        }

        //more column formatting
        row.Cells[DCC.IndexOf("X")].Style["width"] = "35px";
        row.Cells[DCC.IndexOf("ImageName")].Style["width"] = "200px";
        row.Cells[DCC.IndexOf("Description")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("FileName")].Style["width"] = "200px";
        row.Cells[DCC.IndexOf("Image")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("Sort")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Archive")].Style["width"] = "55px";
    }

    private LinkButton CreateLink(string Image_ID, string FileName)
    {
        LinkButton lb = new LinkButton();

        lb.Text = FileName;
        lb.Attributes["onclick"] = string.Format("downloadImage('{0}'); return false;", Image_ID);

        return lb;
    }

    private Image CreateImage(string imageID, string fileName)
    {
        Image img = new Image();

        img.ImageUrl = "ImageView.ashx?id=" + imageID;

        img.Attributes["title"] = fileName;
        img.Attributes["alt"] = fileName;
        img.Attributes["style"] = "max-height:100px;height: expression(this.height > 100 ? 100: true); max-width:100px;width: expression(this.width > 100 ? 100: true);";

        return img;
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
        img.ToolTip = string.Format("{0} Contracts", direction);
        img.AlternateText = string.Format("{0} Contracts", direction);
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
            row.Attributes.Add("ImageID", itemId);
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
                else if (i == DCC["ImageName"].Ordinal)
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
        childFrame.Attributes.Add("ImageID", itemId);
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
        String strName = "Master Grid - Image";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        wb.Worksheets.Add();
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[1];
        ws.Name = "Master Grid - Image";
        Worksheet wsRaw = wb.Worksheets[0];
        wsRaw.Name = "Image Raw";
        int rowCount = 0;
        DataTable Raw = null, parentRaw = null, childRaw = null;
        foreach (DataRow parentRow in copydt.Rows)
        {
            if (parentRow.Field<int>("ImageID") != -1)
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
                      row1.Field<string>("ImageID") == row2.Field<string>("ImageID"));

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
        int ID = parentRow.Field<int>("ImageID");
        child = MasterData.SystemList_Get();
        child.DefaultView.RowFilter = "ImageID = " + ID;
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
        //child.Columns.Remove("A");
        //child.Columns.Remove("X");
        //child.Columns.Remove("CREATEDBY");
        //child.Columns.Remove("CREATEDDATE");
        //child.Columns.Remove("UPDATEDBY");
        //child.Columns.Remove("UPDATEDDATE");
        //child.Columns.Remove("SuiteSort_Order");
        //child.Columns.Remove("WTS_SystemID");
        //child.Columns["DESCRIPTION"].ColumnName = "System Description";
        //child.Columns["ARCHIVE"].ColumnName = "System Archive";
        //child.Columns["WTS_SystemSuite"].ColumnName = "System Suite";
        //child.Columns["WTS_SYSTEM"].ColumnName = "System";
        //child.Columns["SORT_ORDER"].ColumnName = " System Sort Order";
        //child.AcceptChanges();
    }

    private void formatRaw(DataTable Raw)
    {
        if (Raw.Columns.Contains("ImageID")) Raw.Columns.Remove("ImageID");
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
        //dt.Columns.Remove("X");
        //dt.Columns.Remove("CREATEDBY");
        //dt.Columns.Remove("CREATEDDATE");
        //dt.Columns.Remove("UPDATEDBY");
        //dt.Columns.Remove("UPDATEDDATE");
        //dt.Columns.Remove("System_Count");
        //dt.Columns["DESCRIPTION"].ColumnName = "Suite Description";
        //dt.Columns["ARCHIVE"].ColumnName = "Suite Archive";
        //dt.Columns["WTS_SYSTEM_SUITE"].ColumnName = "System Suite";
        //dt.Columns["SORTORDER"].ColumnName = "Suite Sort Order";
    }

    #endregion excel

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

                int id = -1, SortOrder = 0, archive = 0;
                string imageName = string.Empty, description = string.Empty;

            HttpServerUtility server = HttpContext.Current.Server;

            foreach (DataRow dr in dtjson.Rows)
            {
                id = -1;
                imageName = string.Empty;
                description = string.Empty;
                SortOrder = 0;
                archive = 0;

                tempMsg = string.Empty;
                int.TryParse(dr["ImageID"].ToString(), out id);
                imageName = Uri.UnescapeDataString(dr["ImageName"].ToString());
                description = Uri.UnescapeDataString(dr["Description"].ToString());
                int.TryParse(dr["Sort"].ToString(), out SortOrder);
                int.TryParse(dr["Archive"].ToString(), out archive);

                if (id > 0)
                {
                    exists = false;
                    saved = MasterData.Image_Update(id, imageName, description, SortOrder, archive == 1 ? true : false, out exists, out tempMsg);
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
    public static string DeleteItem(int itemId)//, string item)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() {
            { "id", itemId.ToString() }
            , { "exists", "" }
            , { "deleted", "" }
            , { "archived", "" }
            , { "error", "" } };
        bool exists = false, deleted = false, archived = false;
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
                deleted = MasterData.Image_Delete(itemId, out exists, out archived, out errorMsg);

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