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

using Newtonsoft.Json;
using System.Web.UI.HtmlControls;

public partial class MDGrid_SystemSuite_Assignment : System.Web.UI.Page
{
    protected DataTable _dtWorkArea = null;
    protected DataTable _dtSystem = null;
    protected DataTable _dtUser = null;
    protected DataTable _dtSystem_Unused = null;
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;
    protected bool _export = false;

    protected int _qfSystemSuiteID = 0;
    protected int _qfSystemID = 0;

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
        if (Request.QueryString["WTS_SYSTEM_SUITEID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["WTS_SYSTEM_SUITEID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["WTS_SYSTEM_SUITEID"].ToString()), out this._qfSystemSuiteID);
        }

        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
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
        if (_refreshData || Session["WTS_System"] == null)
        {
            dt = MasterData.SystemList_Get(includeArchive: true);
            dt.DefaultView.RowFilter = "Isnull(WTS_SystemSuiteID, -1) = -1";
            _dtSystem_Unused = dt.DefaultView.ToTable();
            dt.DefaultView.RowFilter = "WTS_SystemSuiteID = " + _qfSystemSuiteID + "OR WTS_SystemID = 0";
            dt = dt.DefaultView.ToTable();
            HttpContext.Current.Session["WTS_System"] = dt;
            HttpContext.Current.Session["System_Unused"] = _dtSystem_Unused;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["WTS_System"];
            _dtSystem_Unused = (DataTable)HttpContext.Current.Session["System_Unused"];
        }

        Page.ClientScript.RegisterArrayDeclaration("_system_unused", JsonConvert.SerializeObject(_dtSystem_Unused, Newtonsoft.Json.Formatting.None));

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
                        displayName = "";
                        blnVisible = true;
                        break;
                    case "WTS_SYSTEM":
                        displayName = "System";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "WTS_SystemID":
                        displayName = "SystemID";
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "DESCRIPTION":
                        displayName = "Description";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "CREATEDDATE":
                        displayName = "Associated Date";
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
                columnData.Columns.Item(columnData.Columns.Count - 1).GroupName = groupName;
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
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        //add edit link
        string itemId = row.Cells[DCC.IndexOf("WTS_SystemID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            iti_Tools_Sharp.DropDownList eddl = new iti_Tools_Sharp.DropDownList();
            eddl.ID = "eddl" + itemId;
            ListItem item = new ListItem(string.IsNullOrWhiteSpace(Server.HtmlDecode(row.Cells[DCC.IndexOf("WTS_SYSTEM")].Text.Replace("&nbsp;", " ").Trim())) ? row.Cells[DCC.IndexOf("WTS_SystemID")].Text.Replace("&nbsp;", " ").Trim() : Server.HtmlDecode(row.Cells[DCC.IndexOf("WTS_SYSTEM")].Text.Replace("&nbsp;", " ").Trim()), row.Cells[DCC.IndexOf("WTS_System")].Text.Replace("&nbsp;", " ").Trim());
            eddl.Items.Insert(0, item);
            eddl.Style["width"] = "98%";
            eddl.Enabled = true;

            row.Cells[DCC.IndexOf("WTS_SYSTEM")].Controls.Add(eddl);
            row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("SORT_ORDER")].Controls.Add(WTSUtility.CreateGridTextBox("SORT_ORDER", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SORT_ORDER")].Text.Replace("&nbsp;", " ").Trim())));
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

        string dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkItem_Count")].Text).Trim();
        int count = 0, waCount = 0;
        int.TryParse(dependencies, out count);
        string workAreas = Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkArea_Count")].Text).Trim();
        int.TryParse(workAreas, out waCount);

        if (!CanEdit
            || count > 0
            || waCount > 0)
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
            row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WTS_System")].Text.Trim()));
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
            divChildCount.InnerText = string.Format("({0})", waCount.ToString());
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
            row.Attributes.Add("systemId", itemId);
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
                else if (i == DCC["WTS_SYSTEM"].Ordinal)
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


    static void formatChild(ref DataTable dt)
    {
            dt.Columns.Remove("WorkArea_SystemID");
            dt.Columns.Remove("WorkAreaID");
            dt.Columns.Remove("WTS_SYSTEM");
            dt.Columns.Remove("X");
            dt.Columns.Remove("CREATEDBY");
            dt.Columns.Remove("CREATEDDATE");
            dt.Columns.Remove("UPDATEDBY");
            dt.Columns.Remove("UPDATEDDATE");
            dt.Columns["ARCHIVE"].ColumnName = "Work Area Archive";
            dt.Columns["WorkArea"].ColumnName = "Work Area";
            dt.Columns["DESCRIPTION"].ColumnName = "Work Area Description";
            dt.Columns["ProposedPriority"].ColumnName = "Work Area Proposed Priority";
            dt.Columns["ApprovedPriority"].ColumnName = "Work Area Approved Priority";
            dt.Columns["WorkItem_Count"].ColumnName = "Work Area # Items";
    }


    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtMD_WorkArea_System"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkArea_System"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        row.Cells[DCC.IndexOf("A")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("WTS_SYSTEM")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "25px";
        row.Cells[DCC.IndexOf("ARCHIVE")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("CREATEDDATE")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("CREATEDDATE")].Style["text-align"] = "center";
    }
    #endregion Grid

    [WebMethod(true)]
    public static string SaveChanges(int parentID, string rows)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
		int savedQty = 0, failedQty = 0;
        string ids = string.Empty, failedIds = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
            if (dtjson.Rows.Count == 0)
            {
                errorMsg = "Unable to save. An invalid list of changes was provided.";
                saved = false;
            }
            else
            {


                int id = 0, sortOrder = 0, archive = 0;
                string WTS_SYSTEM = string.Empty, description = string.Empty;

                HttpServerUtility server = HttpContext.Current.Server;

                foreach (DataRow dr in dtjson.Rows)
                {
                    if (dr[1].ToString() != "0")
                    {
                        id = sortOrder = archive = 0;
                        WTS_SYSTEM = description = string.Empty;

                        tempMsg = string.Empty;
                        int.TryParse(dr["WTS_SYSTEMID"].ToString(), out id);
                        WTS_SYSTEM = server.UrlDecode(dr["WTS_SYSTEM"].ToString());
                        description = server.UrlDecode(dr["DESCRIPTION"].ToString());
                        int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
                        int.TryParse(dr["ARCHIVE"].ToString(), out archive);

                        if (string.IsNullOrWhiteSpace(WTS_SYSTEM))
                        {
                            tempMsg = "You must specify a value for System.";
                            saved = false;
                        }
                        else
                        {
                            if (id == 0)
                            {
                                exists = false;
                                saved = MasterData.System_Add(WTS_SYSTEM, description, sortOrder, 0, 0, archive == 1, out exists, out id, out tempMsg);
                                if (exists)
                                {
                                    saved = false;
                                    tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "<br>Cannot add duplicate System record [" + WTS_SYSTEM + "].");
                                }
                                else
                                {
                                    saved = MasterData.System_Update(id, WTS_SYSTEM, description, sortOrder, -1, -1, archive == 1 ? true : false, out errorMsg, WTS_SYSTEM_SUITEID: parentID);
                                }
                            }
                            else
                            {
                                saved = MasterData.System_Update(id, WTS_SYSTEM, description, sortOrder, -1, -1, archive == 1 ? true : false, out errorMsg, WTS_SYSTEM_SUITEID: parentID);

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
                            break;
                        }
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

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.SYSTEM_SUITE);
        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WTS_SYSTEM);

        return JsonConvert.SerializeObject(result, Formatting.None);
    }


    [WebMethod(true)]
    public static string RemoveSystemFromSuite(int systemID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "id", systemID.ToString() }
            , { "exists", "" }
            , { "saved", "" }
            , { "error", "" } };
        bool saved = false; bool exists = false;
        string errorMsg = string.Empty;
        try
        {
            //deleted = MasterData.System_Delete(systemID, out exists, out exists, out archived, out errorMsg);
            saved = MasterData.System_Remove_Suite(systemID, out exists, out errorMsg);

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            saved = false;
            errorMsg = ex.Message;
        }

        result["exists"] = exists.ToString();
        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.SYSTEM_SUITE);
        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WTS_SYSTEM);

        return JsonConvert.SerializeObject(result, Formatting.None);
    }
}