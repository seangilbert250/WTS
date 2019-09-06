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


public partial class MDGrid_Narrative_CONTRACT : System.Web.UI.Page
{
    protected DataTable _dtProductVersion = null;
    protected DataTable _dtCONTRACT = null;
    protected DataTable _dtWorkloadAllocation = null;
    protected DataTable _dtNarrative = null;
    protected DataTable _dtImage = null;
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;
    protected bool _export = false;

    protected int _narrativeID = 0;
    protected int _productVersion = 0;
    protected int _qfContractID = 0;
    protected bool _qfShowArchive = false;

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
        if (Request.QueryString["NarrativeID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["NarrativeID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["NarrativeID"].ToString()), out this._narrativeID);
        }
        if (Request.QueryString["ContractID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ContractID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ContractID"].ToString()), out this._qfContractID);
        }
        if (Request.QueryString["ShowArchive"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchive"].ToString()))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["ShowArchive"].ToString()), out this._qfShowArchive);
        }

        if (Request.QueryString["ProductVersionID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ProductVersionID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ProductVersionID"]), out _productVersion);
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
        _dtImage = MasterData.ImageList_Get(includeArchive: 1);

        DataTable dt = null;
        if (_refreshData || Session["Narrative_CONTRACT"] == null)
        {
            dt = MasterData.Narrative_CONTRACTList_Get(productVersionID: _productVersion, contractID: _qfContractID, includeArchive: _qfShowArchive);
            HttpContext.Current.Session["Narrative_CONTRACT" + _narrativeID] = dt;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["Narrative_CONTRACT" + _narrativeID];
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

                if (_narrativeID > 0)
                {
                    switch (gridColumn.ColumnName)
                    {
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
                        case "ImageName":
                            displayName = "Image";
                            blnVisible = true;
                            blnSortable = true;
                            break;
                        case "Sort":
                            displayName = "Location";
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
                } else if (_qfContractID > 0)
                {
                    switch (gridColumn.ColumnName)
                    {
                        case "ProductVersion":
                            displayName = "Release";
                            blnVisible = true;
                            blnSortable = true;
                            break;
                        case "WorkloadAllocation":
                            displayName = "Workload Allocation";
                            blnVisible = true;
                            blnSortable = true;
                            break;
                        case "ImageName":
                            displayName = "Image";
                            blnVisible = true;
                            blnSortable = true;
                            break;
                        case "Narrative":
                            displayName = "CR Report Narrative Title";
                            blnVisible = true;
                            blnSortable = true;
                            break;
                        case "Description":
                            displayName = "CR Report Narrative";
                            blnVisible = true;
                            blnSortable = true;
                            break;
                        case "Sort":
                            displayName = "Location";
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
        string itemId = row.Cells[DCC.IndexOf("Narrative_CONTRACTID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);
        row.Attributes.Add("narrativeID", _narrativeID.ToString());
        row.Attributes.Add("productVersionID", _productVersion.ToString());

        if (CanEdit)
        {
            DropDownList ddl = null;
            ddl = WTSUtility.CreateGridDropdownList(_dtProductVersion, "ProductVersion", "ProductVersion", "ProductVersionID", itemId, row.Cells[DCC.IndexOf("ProductVersionID")].Text, row.Cells[DCC.IndexOf("ProductVersion")].Text.Replace("&nbsp;", " ").Trim(), null);
            row.Cells[DCC.IndexOf("ProductVersion")].Controls.Add(ddl);

            ddl = WTSUtility.CreateGridDropdownList(_dtCONTRACT, "CONTRACT", "CONTRACT", "CONTRACTID", itemId, row.Cells[DCC.IndexOf("CONTRACTID")].Text, row.Cells[DCC.IndexOf("CONTRACT")].Text.Replace("&nbsp;", " ").Trim(), null);
            row.Cells[DCC.IndexOf("CONTRACT")].Controls.Add(ddl);

            ddl = WTSUtility.CreateGridDropdownList(_dtWorkloadAllocation, "WorkloadAllocation", "WorkloadAllocation", "WorkloadAllocationID", itemId, row.Cells[DCC.IndexOf("WorkloadAllocationID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("WorkloadAllocation")].Text.Replace("&nbsp;", " ").Trim(), null);
            row.Cells[DCC.IndexOf("WorkloadAllocation")].Controls.Add(ddl);

            ddl = WTSUtility.CreateGridDropdownList(_dtImage, "ImageName", "ImageName", "ImageID", itemId, row.Cells[DCC.IndexOf("ImageID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("ImageName")].Text.Replace("&nbsp;", " ").Trim(), null);
            row.Cells[DCC.IndexOf("ImageName")].Controls.Add(ddl);

            row.Cells[DCC.IndexOf("Narrative")].Controls.Add(WTSUtility.CreateGridTextBox("Narrative", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Narrative")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("Description")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Description")].Text.Replace("&nbsp;", " ").Trim()), false, true));

            DataTable dtSort = new DataTable("dtSort");
            dtSort.Columns.Add("Sort");
            dtSort.Columns.Add("SortID");
            dtSort.Rows.Add("1", "1");
            dtSort.Rows.Add("2", "2");
            dtSort.Rows.Add("3", "3");
            dtSort.Rows.Add("4", "4");
            ddl = WTSUtility.CreateGridDropdownList(dtSort, "Sort", "Sort", "SortID", itemId, row.Cells[DCC.IndexOf("Sort")].Text, row.Cells[DCC.IndexOf("Sort")].Text.Replace("&nbsp;", " ").Trim(), null);
            row.Cells[DCC.IndexOf("Sort")].Controls.Add(ddl);
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
        if (HttpContext.Current.Session["Narrative_CONTRACT" + _narrativeID] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["Narrative_CONTRACT" + _narrativeID];
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
        row.Cells[DCC.IndexOf("ImageName")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("ImageName")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Narrative")].Style["width"] = "300px";
        row.Cells[DCC.IndexOf("Narrative")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Sort")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Sort")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Archive")].Style["width"] = "25px";
        row.Cells[DCC.IndexOf("Archive")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Z")].Style["width"] = "12px";
    }
    #endregion Grid

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
                deleted = MasterData.Narrative_CONTRACT_Delete(Narrative_CONTRACTID: itemId, exists: out exists, errorMsg: out errorMsg);
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