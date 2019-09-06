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
using System.Web.UI.WebControls;

using Newtonsoft.Json;


public partial class MDGrid_AllocationGroup_Assignment : System.Web.UI.Page
{
    protected DataTable _dtWorkArea = null;
    protected DataTable _dtSystem = null;
    protected DataTable _dtUser = null;
    protected DataTable _dtAllocation_Unused = null;
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;
    protected bool _export = false;

    protected int _qfWorkAreaID = 0;
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
        if (Request.QueryString["AllocationGroupID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["AllocationGroupID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["AllocationGroupID"].ToString()), out this._qfWorkAreaID);
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
        _dtUser = UserManagement.LoadUserList(organizationId: 0, excludeDeveloper: false, loadArchived: false, userNameSearch: "");
        Page.ClientScript.RegisterArrayDeclaration("_userList", JsonConvert.SerializeObject(_dtUser, Newtonsoft.Json.Formatting.None));
        _dtAllocation_Unused = MasterData.Allocation_Get_All_Unused();
        Page.ClientScript.RegisterArrayDeclaration("_Allocation_Unused", JsonConvert.SerializeObject(_dtAllocation_Unused, Newtonsoft.Json.Formatting.None));
        DataTable dt = null;
        if (_refreshData || Session["AllocationGroup_Assignment"] == null)
        {
            dt = MasterData.AllocationGroup_Assignment_Get(this._qfWorkAreaID);
            HttpContext.Current.Session["AllocationGroup_Assignment"] = dt;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["AllocationGroup_Assignment"];
        }

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
            spanRowCount.InnerText = dt.Rows.Count.ToString();


            InitializeColumnData(ref dt);
            dt.AcceptChanges();
            iti_Tools_Sharp.DynamicHeader head = WTSUtility.CreateGridMultiHeader(dt);
            if (head != null)
            {
                grdMD.DynamicHeader = head;
            }

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
                    case "ALLOCATIONID":
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "AllocationCategoryID":
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "ALLOCATION":
                        displayName = "Allocation";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DESCRIPTION":
                        displayName = "Description";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "SORT_ORDER":
                        displayName = "Sort Order";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DefaultSME":
                        displayName = "SME";
                        groupName = "Default Resources";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DefaultBusinessResource":
                        displayName = "Business Resource";
                        groupName = "Default Resources";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DefaultTechnicalResource":
                        displayName = "Technical Resource";
                        groupName = "Default Resources";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DefaultAssignedTo":
                        displayName = "Assigned To";
                        groupName = "Default Resources";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ARCHIVE":
                        displayName = "Archive";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ALLOCATIONGROUPID":
                        blnVisible = false;
                        blnSortable = false;
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

        //row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        //add edit link
        string itemId = row.Cells[DCC.IndexOf("ALLOCATIONID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            //string selectedId = row.Cells[DCC.IndexOf("WorkAreaID")].Text;
            //string selectedText = row.Cells[DCC.IndexOf("WorkArea")].Text;
            //if (itemId == "0" && _qfWorkAreaID != 0)
            //{
            //    selectedId = _qfWorkAreaID.ToString();
            //    selectedText = string.Empty;
            //}
            //DropDownList ddl = null;
            //ddl = WTSUtility.CreateGridDropdownList(_dtWorkArea, "WorkArea", "WorkArea", "WorkAreaID", itemId, selectedId, selectedText, null);
            //ddl.Enabled = (_qfWorkAreaID == 0);
            //row.Cells[DCC.IndexOf("WorkArea")].Controls.Add(ddl);

            //selectedId = row.Cells[DCC.IndexOf("WTS_SYSTEMID")].Text.Replace("&nbsp;", " ").Trim();
            //selectedText = row.Cells[DCC.IndexOf("WTS_SYSTEM")].Text.Replace("&nbsp;", " ").Trim();
            //if (itemId == "0" && _qfSystemID != 0)
            //{
            //    selectedId = _qfSystemID.ToString();
            //    selectedText = string.Empty;
            //}

            //if (string.IsNullOrWhiteSpace(selectedId))
            //{
            //    selectedId = "0";
            //}

            //ddl = WTSUtility.CreateGridDropdownList(_dtSystem, "WTS_SYSTEM", "WTS_SYSTEM", "WTS_SYSTEMID", itemId, selectedId, selectedText, null);
            //ddl.Enabled = (_qfSystemID == 0);
            //row.Cells[DCC.IndexOf("WTS_SYSTEM")].Controls.Add(ddl);
            DropDownList ddl = null;
            DataTable dt = null;
            ddl = WTSUtility.CreateGridDropdownList("ALLOCATION", itemId, row.Cells[DCC.IndexOf("ALLOCATION")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("ALLOCATION")].Text.Replace("&nbsp;", " ").Trim(), 0);
            ddl.Enabled = (_qfWorkAreaID == 0);
            row.Cells[DCC.IndexOf("ALLOCATION")].Controls.Add(ddl);
            
            //row.Cells[DCC.IndexOf("ALLOCATION")].Controls.Add(WTSUtility.CreateGridTextBox("Allocation Assignment", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ALLOCATION")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("SORT_ORDER")].Controls.Add(WTSUtility.CreateGridTextBox("SORT_ORDER", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SORT_ORDER")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("DefaultSME")].Controls.Add(WTSUtility.CreateGridDropdownList("DefaultSME", itemId, row.Cells[DCC.IndexOf("DefaultSME")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultSMEID")].Text.Replace("&nbsp;", " ").Trim(), 0));
            row.Cells[DCC.IndexOf("DefaultBusinessResource")].Controls.Add(WTSUtility.CreateGridDropdownList("DefaultBusinessResource", itemId, row.Cells[DCC.IndexOf("DefaultBusinessResource")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultBusinessResourceID")].Text.Replace("&nbsp;", " ").Trim(), 0));
            row.Cells[DCC.IndexOf("DefaultTechnicalResource")].Controls.Add(WTSUtility.CreateGridDropdownList("DefaultTechnicalResource", itemId, row.Cells[DCC.IndexOf("DefaultTechnicalResource")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultTechnicalResourceID")].Text.Replace("&nbsp;", " ").Trim(), 0));
            row.Cells[DCC.IndexOf("DefaultAssignedTo")].Controls.Add(WTSUtility.CreateGridDropdownList("DefaultAssignedTo", itemId, row.Cells[DCC.IndexOf("DefaultAssignedTo")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultAssignedToID")].Text.Replace("&nbsp;", " ").Trim(), 0));
            //row.Cells[DCC.IndexOf("ProposedPriority")].Controls.Add(WTSUtility.CreateGridTextBox("ProposedPriority", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ProposedPriority")].Text.Replace("&nbsp;", " ").Trim()), true));
            //if (IsAdmin)
            //{
            //    row.Cells[DCC.IndexOf("ApprovedPriority")].Controls.Add(WTSUtility.CreateGridTextBox("ApprovedPriority", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ApprovedPriority")].Text.Replace("&nbsp;", " ").Trim()), true));
            //}

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

        if (!CanEdit)
        {
            //Image imgBlank = new Image();
            //imgBlank.Height = 10;
            //imgBlank.Width = 10;
            //imgBlank.ImageUrl = "Images/Icons/blank.png";
            //imgBlank.AlternateText = "";
            //row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
        }
        else
        {
            //row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WTS_SYSTEM")].Text.Trim() + " - " + row.Cells[DCC.IndexOf("WorkArea")].Text.Trim()));
        }
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
        //for (int i = 0; i < row.Cells.Count; i++)
        //{
        //    if (i != DCC.IndexOf("X") &&
        //        i != DCC.IndexOf("WorkArea")
        //        && i != DCC.IndexOf("WTS_SYSTEM")
        //        && i != DCC.IndexOf("ProposedPriority")
        //        && i != DCC.IndexOf("ApprovedPriority")
        //        && i != DCC.IndexOf("WorkItem_Count")
        //        && i != DCC.IndexOf("ARCHIVE"))
        //    {
        //        row.Cells[i].Style["text-align"] = "left";
        //        row.Cells[i].Style["padding-left"] = "5px";
        //    }
        //    else
        //    {
        //        row.Cells[i].Style["text-align"] = "center";
        //        row.Cells[i].Style["padding-left"] = "0px";
        //    }
        //}

        //more column formatting
        row.Cells[DCC.IndexOf("A")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("ALLOCATION")].Style["width"] = "150px";
        row.Cells[DCC.IndexOf("DESCRIPTION")].Style["width"] = "225px";
        row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("DefaultSME")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("DefaultBusinessResource")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("DefaultTechnicalResource")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("DefaultAssignedTo")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "25px";
        row.Cells[DCC.IndexOf("ARCHIVE")].Style["text-align"] = "center";
    }

    #endregion Grid


    [WebMethod(true)]
    public static string SaveChanges(int parentID, string rows)
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

            int id = 0, categoryId = 0, sortOrder = 0, archive = 0;
            int assignedToID = 0, smeID = 0, busResourceID = 0, techResourceID = 0;
            string allocation = string.Empty, description = string.Empty;

            HttpServerUtility server = HttpContext.Current.Server;
            //save
            foreach (DataRow dr in dtjson.Rows)
            {
                id = categoryId = sortOrder = archive = 0;
                assignedToID = smeID = busResourceID = techResourceID = 0;
                allocation = description = string.Empty;

                tempMsg = string.Empty;
                int.TryParse(dr["ALLOCATIONID"].ToString(), out id);
                allocation = server.UrlDecode(dr["ALLOCATION"].ToString());
                description = server.UrlDecode(dr["DESCRIPTION"].ToString());
                int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
                int.TryParse(dr["ARCHIVE"].ToString(), out archive);
                int.TryParse(dr["DefaultAssignedTo"].ToString(), out assignedToID);
                int.TryParse(dr["DefaultSME"].ToString(), out smeID);
                int.TryParse(dr["DefaultBusinessResource"].ToString(), out busResourceID);
                int.TryParse(dr["DefaultTechnicalResource"].ToString(), out techResourceID);

                if (string.IsNullOrWhiteSpace(allocation))
                {
                    tempMsg = "You must specify a value for Allocation.";
                    saved = false;
                }
                else
                {
                    if (id == 0)
                    {
                        exists = false;
                        saved = MasterData.AllocationGroup_Assignment_Add(allocationID: allocation, description: description, sortOrder: sortOrder, archive: archive == 1,
                            defaultSMEID: smeID, defaultBusinessResourceID: busResourceID, defaultTechnicalResourceID: techResourceID, defaultAssignedToID: assignedToID,
                            AllocationGroupID: parentID, exists: out exists, newID: out id, errorMsg: out tempMsg);
                        if (exists)
                        {
                            saved = false;
                            tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Allocation record [" + allocation + "].");
                        }
                    }
                    else
                    {
                        saved = MasterData.AllocationGroup_Assignment_Update(id, allocation: allocation, description: description, sortOrder: sortOrder, archive: archive == 1, defaultSMEID: smeID, defaultAssignedToID: assignedToID, defaultBusinessResourceID: busResourceID, defaultTechnicalResourceID: techResourceID, errorMsg: out tempMsg);
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

        return JsonConvert.SerializeObject(result, Formatting.None);
    }


    [WebMethod(true)]
    public static bool DeleteChild(int itemId){
        bool saved = false;
        try
        {
            saved = MasterData.AllocationGroup_DeleteChild(itemId);
        }
        catch (Exception ex)
        {

        }
        return saved;
    }


    [WebMethod(true)]
    public static string DeleteItem(int itemId, string item)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "id", itemId.ToString() }
			, { "item", item }
			, { "exists", "" }
			, { "deleted", "" }
			, { "error", "" } };
        bool exists = false, deleted = false;
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
                deleted = MasterData.WorkArea_System_Delete(workAreaSystemID: itemId, cv: "0", exists: out exists, errorMsg: out errorMsg);
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
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }
}