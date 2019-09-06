using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using Aspose.Cells;
using System.IO;

public partial class MDGrid_RQMT_Description_Type : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols _columnData = new GridCols();

    protected bool _refreshData = false;
    protected bool _export = false;

    protected int _qfID = 0;

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

        if (DCC != null)
        {
            this.DCC.Clear();
        }

        loadGridData();
    }
    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        formatParent(ref copydt);
        String strName = "Master Grid - RQMT Description Type";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[0];
        int rowCount = 0;
        printParentHeader(ws, ref rowCount, copydt.Columns);
        foreach (DataRow parentRow in copydt.Rows)
        {
            if (parentRow.Field<int>("RQMTDescriptionTypeID") != 0)
            {
                printParent(parentRow, ws, ref rowCount, copydt);
                rowCount++;
                //printChildRows(parentRow, ws, ref rowCount);
                //rowCount++;
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
    //private void printChildRows(DataRow parentRow, Worksheet ws, ref int rowCount)
    //{
    //    DataTable child = null;
    //    int ID = parentRow.Field<int>("AORWorkTypeID");
    //    child = MasterData.EffortArea_SizeList_Get(effortAreaID: ID);
    //    int i = 0, j = 1;
    //    formatChild(child);
    //    printChildHeader(ws, ref rowCount, child.Columns);
    //    foreach (DataRow row in child.Rows)
    //    {
    //        j = 2;
    //        foreach (object value in row.ItemArray)
    //        {
    //            ws.Cells[rowCount + i, j].PutValue(value);
    //            j++;
    //        }
    //        i++;
    //    }
    //    rowCount += child.Rows.Count;
    //}

    //private void formatChild(DataTable child)
    //{
    //    child.Columns.Remove("EffortArea_SORT_ORDER");
    //    child.Columns.Remove("CREATEDBY");
    //    child.Columns.Remove("CREATEDDATE");
    //    child.Columns.Remove("UPDATEDBY");
    //    child.Columns.Remove("UPDATEDDATE");
    //    child.Columns.Remove("EffortArea_SizeID");
    //    child.Columns.Remove("EffortAreaID");
    //    child.Columns.Remove("EffortSizeID");
    //    child.Columns.Remove("EffortSize_SORT_ORDER");
    //    child.Columns.Remove("SORT_ORDER");
    //    child.Columns.Remove("ARCHIVE");
    //    child.Columns.Remove("X");
    //    child.Columns["DESCRIPTION"].ColumnName = "Description";
    //    child.Columns["EffortArea"].ColumnName = "Area of Effort";
    //    child.Columns["EffortSize"].ColumnName = "Size of Effort";
    //    child.Columns["MinValue"].ColumnName = "Minimum";
    //    child.Columns["MaxValue"].ColumnName = "Maximum";
    //    child.Columns["WorkItem_Count"].ColumnName = "Assigned";
    //    child.Rows[0].Delete();
    //    child.AcceptChanges();
    //}


    //private void printChildHeader(Worksheet ws, ref int rowCount, DataColumnCollection columns)
    //{
    //    if (object.ReferenceEquals(columns, null) || columns.Count < 1) { return; }
    //    Aspose.Cells.Style style = new Aspose.Cells.Style();
    //    style.Pattern = BackgroundType.Solid;
    //    style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("lightBlue");
    //    for (int i = 0; i < columns.Count; i++)
    //    {
    //        ws.Cells[rowCount, i + 2].PutValue(columns[i].ColumnName);
    //        ws.Cells[rowCount, i + 2].SetStyle(style);
    //    }
    //    rowCount++;
    //}

    private void printParent(DataRow parentRow, Worksheet ws, ref int rowCount, DataTable dt)
    {
        int i = 0;
        foreach (object value in parentRow.ItemArray)
        {          
            if (i == dt.Columns["Archive"].Ordinal)
            {

                int tempInt = 0;
                int.TryParse(value.ToString(), out tempInt);
                string tempVar = (tempInt == 1) ? "Yes" : "No";
                ws.Cells[rowCount, i].PutValue(tempVar);          
            }
            else
            {
                ws.Cells[rowCount, i].PutValue(value);
            }
            i++;
        }
        //rowCount++;
    }

    private static void formatParent(ref DataTable dt)
    {
        dt.Columns.Remove("A");
        dt.Columns.Remove("CREATEDBY");
        dt.Columns.Remove("CREATEDDATE");
        dt.Columns.Remove("UPDATEDBY");
        dt.Columns.Remove("UPDATEDDATE");
        dt.Columns.Remove("Sort");
        //dt.Columns.Remove("Size_Count");
        //dt.Columns.Remove("SORT_ORDER");
        dt.Columns.Remove("X");
        dt.Columns["DESCRIPTION"].ColumnName = "Description";
        dt.Columns["ARCHIVE"].ColumnName = "Archive";
        dt.Columns["RQMTDescriptionType"].ColumnName = "RQMT Description Type";
    }
    private void loadGridData()
    {
        DataTable dt;

        if (_refreshData || Session["dtMD_RQMT_Description_Type"] == null)
        {
            dt = MasterData.RQMT_Description_TypeList_Get(includeArchive: true);
            HttpContext.Current.Session["dtMD_RQMT_Description_Type"] = dt;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["dtMD_RQMT_Description_Type"];
        }

        int count = 0;

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

            InitializeColumnData(ref dt);
            dt.AcceptChanges();

            count = dt.Rows.Count;
            count = count > 0 ? count - 1 : count; //need to subtract the empty row
        }
        spanRowCount.InnerText = count.ToString();

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
                    case "X":
                        blnVisible = true;
                        break;
                    case "RQMTDescriptionTypeID":
                        blnVisible = false;
                        break;
                    case "RQMTDescriptionType":
                        displayName = "RQMT Description Type";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Description":
                        displayName = "Description";
                        blnVisible = true;
                        break;
                    case "Sort":
                        displayName = "Sort";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ARCHIVE":
                        displayName = "Archive";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                }

                _columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
                _columnData.Columns.Item(_columnData.Columns.Count - 1).CanReorder = blnOrderable;
            }

            //Initialize the columnData
            _columnData.Initialize(ref dt, ";", "~", "|");

            //Get sortable columns and default column order
            SortableColumns = _columnData.SortableColumnsToString();
            DefaultColumnOrder = _columnData.DefaultColumnOrderToString();
            //Sort and Reorder Columns
            _columnData.ReorderDataTable(ref dt, ColumnOrder);
            _columnData.SortDataTable(ref dt, SortOrder);
            SelectedColumnOrder = _columnData.CurrentColumnOrderToString();

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
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


    #region Grid

    void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        _columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        row.Cells[DCC.IndexOf("A")].Text = "&nbsp;";
        row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        _columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        //add edit link
        string itemId = row.Cells[DCC.IndexOf("RQMTDescriptionTypeID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            row.Cells[DCC.IndexOf("RQMTDescriptionType")].Controls.Add(WTSUtility.CreateGridTextBox("RQMTDescriptionType", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("RQMTDescriptionType")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("Description")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Description")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("Sort")].Controls.Add(WTSUtility.CreateGridTextBox("Sort", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Sort")].Text.Trim()), true));
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

        //string size_count = Server.HtmlDecode(row.Cells[DCC.IndexOf("Size_Count")].Text).Trim();
        //int count = 0;
        //if (!CanEdit
        //    || !int.TryParse(size_count, out count))
        //{
        //    Image imgBlank = new Image();
        //    imgBlank.Height = 10;
        //    imgBlank.Width = 10;
        //    imgBlank.ImageUrl = "Images/Icons/blank.png";
        //    imgBlank.AlternateText = "";
        //    row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
        //}
        //else
        //{
        //    Image imgBlank = new Image();
        //    imgBlank.Height = 10;
        //    imgBlank.Width = 10;
        //    imgBlank.ImageUrl = "Images/Icons/blank.png";
        //    imgBlank.AlternateText = "";
        //    row.Cells[DCC["A"].Ordinal].Controls.Add(imgBlank);

           row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("RQMTDescriptionType")].Text.Trim()));
        //}

        //HtmlGenericControl divChildren = new HtmlGenericControl();
        //divChildren.Style["display"] = "table-row";
        //divChildren.Style["text-align"] = "right";
        //HtmlGenericControl divChildrenButtons = new HtmlGenericControl();
        //divChildrenButtons.Style["display"] = "table-cell";
        //divChildrenButtons.Controls.Add(createShowHideButton(true, "Show", itemId));
        //divChildrenButtons.Controls.Add(createShowHideButton(false, "Hide", itemId));
        //HtmlGenericControl divChildCount = new HtmlGenericControl();
        //divChildCount.InnerText = string.Format("({0})", size_count.ToString());
        //divChildCount.Style["display"] = "table-cell";
        //divChildCount.Style["padding-left"] = "2px";
        //divChildren.Controls.Add(divChildrenButtons);
        //divChildren.Controls.Add(divChildCount);
        //buttons to show/hide child grid
        row.Cells[DCC["A"].Ordinal].Controls.Clear();
        //row.Cells[DCC["A"].Ordinal].Controls.Add(divChildren);

        //add child grid row for Task Items
        //Table table = (Table)row.Parent;
        //GridViewRow childRow = createChildRow(itemId);
        //table.Rows.AddAt(table.Rows.Count, childRow);
    }

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtMD_RQMT_Description_Type"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_RQMT_Description_Type"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("X")
                && i != DCC.IndexOf("Size_Count")
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
        row.Cells[DCC.IndexOf("A")].Style["width"] = "32px";
        row.Cells[DCC.IndexOf("RQMTDescriptionType")].Style["width"] = "100px";
        //row.Cells[DCC.IndexOf("Size_Count")].Style["width"] = "60px";
        //row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "35px";
        row.Cells[DCC.IndexOf("Sort")].Style["width"] = "35px";
        row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
    }

    //Image createShowHideButton(bool show = false, string direction = "Show", string itemId = "ALL")
    //{
    //    StringBuilder sb = new StringBuilder();
    //    sb.AppendFormat("imgShowHideChildren_click(this,'{0}','{1}');", direction, itemId);

    //    Image img = new Image();
    //    img.ID = string.Format("img{0}Children_{1}", direction, itemId);
    //    img.Style["display"] = show ? "block" : "none";
    //    img.Style["cursor"] = "pointer";
    //    img.Attributes.Add("Name", string.Format("img{0}", direction));
    //    img.Attributes.Add("itemId", itemId);
    //    img.Height = 10;
    //    img.Width = 10;
    //    img.ImageUrl = direction.ToUpper() == "SHOW"
    //        ? "Images/Icons/add_blue.png"
    //        : "Images/Icons/minus_blue.png";
    //    img.ToolTip = string.Format("{0} Sizes for [{1}]", direction, itemId);
    //    img.AlternateText = string.Format("{0} Sizes for [{1}]", direction, itemId);
    //    img.Attributes.Add("Onclick", sb.ToString());

    //    return img;
    //}

    //GridViewRow createChildRow(string itemId = "")
    //{
    //    GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
    //    TableCell tableCell = null;

    //    try
    //    {
    //        row.CssClass = "gridBody";
    //        row.Style["display"] = "none";
    //        row.ID = string.Format("gridChild_{0}", itemId);
    //        row.Attributes.Add("effortAreaId", itemId);
    //        row.Attributes.Add("Name", string.Format("gridChild_{0}", itemId));

    //        //add the table cells
    //        for (int i = 0; i < DCC.Count; i++)
    //        {
    //            tableCell = new TableCell();
    //            tableCell.Text = "&nbsp;";

    //            if (i == DCC["EffortAreaID"].Ordinal)
    //            {
    //                //set width to match parent
    //                tableCell.Style["width"] = "32px";
    //                tableCell.Style["border-right"] = "1px solid transparent";
    //            }
    //            else if (i == DCC["EffortArea"].Ordinal)
    //            {
    //                tableCell.Style["padding-top"] = "10px";
    //                tableCell.Style["padding-right"] = "10px";
    //                tableCell.Style["padding-bottom"] = "0px";
    //                tableCell.Style["padding-left"] = "0px";
    //                tableCell.Style["vertical-align"] = "top";
    //                tableCell.ColumnSpan = DCC.Count - 2;
    //                //add the frame here
    //                tableCell.Controls.Add(createChildFrame(itemId: itemId));
    //            }
    //            else
    //            {
    //                tableCell.Style["display"] = "none";
    //            }

    //            row.Cells.Add(tableCell);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        LogUtility.LogException(ex);
    //        row = null;
    //    }

    //    return row;
    //}

    //HtmlIframe createChildFrame(string itemId = "")
    //{
    //    HtmlIframe childFrame = new HtmlIframe();

    //    if (string.IsNullOrWhiteSpace(itemId))
    //    {
    //        return null;
    //    }

    //    childFrame.ID = string.Format("frameChild_{0}", itemId);
    //    childFrame.Attributes.Add("effortAreaId", itemId);
    //    childFrame.Attributes["frameborder"] = "0";
    //    childFrame.Attributes["scrolling"] = "no";
    //    childFrame.Attributes["src"] = "javascript:''";
    //    childFrame.Style["height"] = "30px";
    //    childFrame.Style["width"] = "100%";
    //    childFrame.Style["border"] = "none";

    //    return childFrame;
    //}

    #endregion Grid


    [WebMethod(true)]
    public static string SaveChanges(string rows)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false, duplicate = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
            if (dtjson.Rows.Count == 0)
            {
                errorMsg = "Unable to save. An invalid list of changes was provided.";
                saved = false;
            }

            int id = 0, sortOrder = 0, archive = 0;
            string RQMTDescriptionType = string.Empty, description = string.Empty;

            HttpServerUtility server = HttpContext.Current.Server;
            //save
            foreach (DataRow dr in dtjson.Rows)
            {
                id = 0;
                sortOrder = 0;
                RQMTDescriptionType = string.Empty;
                description = string.Empty;
                archive = 0;

                tempMsg = string.Empty;
                int.TryParse(dr["RQMTDescriptionTypeID"].ToString(), out id);
                RQMTDescriptionType = server.UrlDecode(dr["RQMTDescriptionType"].ToString());
                description = server.UrlDecode(dr["Description"].ToString());
                int.TryParse(dr["Sort"].ToString(), out sortOrder);
                int.TryParse(dr["ARCHIVE"].ToString(), out archive);

                if (string.IsNullOrWhiteSpace(RQMTDescriptionType))
                {
                    tempMsg = "You must specify a value for RQMT Description Type.";
                    saved = false;
                }
                else
                {
                    if (id == 0)
                    {
                        exists = false;
                        saved = MasterData.RQMTDescriptionType_Add(RQMTDescriptionType: RQMTDescriptionType, description: description, sort: sortOrder, archive: (archive == 1), exists: out exists, newID: out id, errorMsg: out tempMsg);
                        if (exists)
                        {
                            saved = false;
                            tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate RQMT Description Type record [" + RQMTDescriptionType + "].");
                        }
                    }
                    else
                    {
                        saved = MasterData.RQMTDescriptionType_Update(RQMTDescriptionTypeID: id, RQMTDescriptionType: RQMTDescriptionType, description: description, sort: sortOrder, archive: (archive == 1), duplicate: out duplicate, errorMsg: out tempMsg);
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
                deleted = MasterData.RQMTDescriptionType_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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