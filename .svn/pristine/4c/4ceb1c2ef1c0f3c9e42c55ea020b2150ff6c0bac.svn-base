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


public partial class MDGrid_Image_CONTRACT : System.Web.UI.Page
{
    protected DataTable _dtProductVersion = null;
    protected DataTable _dtCONTRACT = null;
    protected DataTable _dtWorkloadAllocation = null;
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;
    protected bool _export = false;

    protected int _imageID = 0;

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
        if (Request.QueryString["ImageID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ImageID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ImageID"].ToString()), out this._imageID);
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
        _dtProductVersion = MasterData.ProductVersionList_Get(includeArchive: false);
        _dtCONTRACT = MasterData.ContractList_Get(includeArchive: false);
        _dtWorkloadAllocation = MasterData.WorkloadAllocationList_Get(includeArchive: 0);

        DataTable dt = null;
        if (_refreshData || Session["Image_CONTRACT"] == null)
        {
            dt = MasterData.Image_CONTRACTList_Get(imageID: _imageID);
            HttpContext.Current.Session["Image_CONTRACT" + _imageID] = dt;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["Image_CONTRACT" + _imageID];
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
                    case "ProductVersion":
                        displayName = "Release";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "CONTRACT":
                        displayName = "Contract";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "WorkloadAllocation":
                        displayName = "Workload Allocation";
                        blnVisible = true;
                        blnSortable = true;
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
        string itemId = row.Cells[DCC.IndexOf("Image_CONTRACTID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanEdit)
        {
            DropDownList ddl = null;
            ddl = WTSUtility.CreateGridDropdownList(_dtProductVersion, "ProductVersion", "ProductVersion", "ProductVersionID", itemId, row.Cells[DCC.IndexOf("ProductVersionID")].Text, row.Cells[DCC.IndexOf("ProductVersion")].Text.Replace("&nbsp;", " ").Trim(), null);
            row.Cells[DCC.IndexOf("ProductVersion")].Controls.Add(ddl);

            ddl = WTSUtility.CreateGridDropdownList(_dtCONTRACT, "CONTRACT", "CONTRACT", "CONTRACTID", itemId, row.Cells[DCC.IndexOf("CONTRACTID")].Text, row.Cells[DCC.IndexOf("CONTRACT")].Text.Replace("&nbsp;", " ").Trim(), null);
            row.Cells[DCC.IndexOf("CONTRACT")].Controls.Add(ddl);

            ddl = WTSUtility.CreateGridDropdownList(_dtWorkloadAllocation, "WorkloadAllocation", "WorkloadAllocation", "WorkloadAllocationID", itemId, row.Cells[DCC.IndexOf("WorkloadAllocationID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("WorkloadAllocation")].Text.Replace("&nbsp;", " ").Trim(), null);
            row.Cells[DCC.IndexOf("WorkloadAllocation")].Controls.Add(ddl);

            row.Cells[DCC.IndexOf("Sort")].Controls.Add(WTSUtility.CreateGridTextBox("Sort", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Sort")].Text.Replace("&nbsp;", " ").Trim())));
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
    }

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["Image_CONTRACT" + _imageID] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["Image_CONTRACT" + _imageID];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        row.Cells[DCC.IndexOf("ProductVersion")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("ProductVersion")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("CONTRACT")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("CONTRACT")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("WorkloadAllocation")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("WorkloadAllocation")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Sort")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Sort")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Archive")].Style["width"] = "25px";
        row.Cells[DCC.IndexOf("Archive")].Style["text-align"] = "center";
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
                int id = 0, ProductVersionID = 0, CONTRACTID = 0, WorkloadAllocationID = 0, sortOrder = 0, archive = 0;

                HttpServerUtility server = HttpContext.Current.Server;

                foreach (DataRow dr in dtjson.Rows)
                {
                    if (dr[1].ToString() != "0")
                    {
                        id = CONTRACTID = WorkloadAllocationID = sortOrder = archive = 0;

                        tempMsg = string.Empty;
                        int.TryParse(dr["Image_CONTRACTID"].ToString(), out id);
                        int.TryParse(dr["ProductVersion"].ToString(), out ProductVersionID);
                        int.TryParse(dr["CONTRACT"].ToString(), out CONTRACTID);
                        int.TryParse(dr["WorkloadAllocation"].ToString(), out WorkloadAllocationID);
                        int.TryParse(dr["Sort"].ToString(), out sortOrder);
                        int.TryParse(dr["Archive"].ToString(), out archive);

                        if (CONTRACTID == 0 || ProductVersionID == 0)
                        {
                            tempMsg = "You must specify a value for Contract and Product Version.";
                            saved = false;
                        }
                        else
                        {
                            if (id == 0)
                            {
                                exists = false;
                                saved = MasterData.Image_CONTRACT_Add(parentID, ProductVersionID, CONTRACTID, WorkloadAllocationID, sortOrder, archive == 1, out exists, out id, out tempMsg);
                                if (exists)
                                {
                                    saved = false;
                                    tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "<br>Cannot add duplicate Contract record.");
                                }
                            }
                            else
                            {
                                exists = false;
                                saved = MasterData.Image_CONTRACT_Update(id, parentID, ProductVersionID, CONTRACTID, WorkloadAllocationID,sortOrder, archive == 1 ? true : false, out exists, out errorMsg);
                                if (exists)
                                {
                                    saved = false;
                                    tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "<br>Cannot add duplicate Contract record.");
                                }
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

        return JsonConvert.SerializeObject(result, Formatting.None);
    }


    [WebMethod(true)]
    public static string DeleteChild(int itemId)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "id", itemId.ToString() }
            , { "exists", "" }
            , { "deleted", "" }
            , { "archived", "" }
            , { "error", "" } };
        bool deleted = false; bool exists = false, archived = false;
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
                deleted = MasterData.Image_CONTRACT_Delete(Image_CONTRACTID: itemId, exists: out exists, errorMsg: out errorMsg);
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