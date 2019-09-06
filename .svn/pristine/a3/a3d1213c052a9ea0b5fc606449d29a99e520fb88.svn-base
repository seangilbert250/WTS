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


public partial class MDGrid_Contract_System : System.Web.UI.Page
{
    protected DataTable _dtSystem = null;
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;

    protected int _qfContractID = 0;

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
        if (Request.QueryString["ContractID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ContractID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ContractID"].ToString()), out this._qfContractID);
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
        _dtSystem = MasterData.SystemList_Get(includeArchive: false);

        DataTable dt = MasterData.WTS_Contract_SystemList_Get(CONTRACTID: this._qfContractID);

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

            InitializeColumnData(ref dt);
            dt.AcceptChanges();
        }

        int count = dt.Rows.Count;
        count = count > 0 ? count - 1 : count; //need to subtract the empty row
        spanRowCount.InnerText = count.ToString();

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
                    case "Suite":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "System":
                        displayName = "System(Task)";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Primary":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Archive":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "X":
                        blnVisible = true;
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
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        //add edit link
        string itemId = row.Cells[DCC.IndexOf("WTS_SYSTEM_CONTRACTID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            string selectedId = row.Cells[DCC.IndexOf("WTS_SYSTEMID")].Text;
            string selectedText = row.Cells[DCC.IndexOf("System")].Text;
            DropDownList ddl = null;
            DataTable _dtSystemAdd = MasterData.SystemList_Get(includeArchive: false, ContractID: this._qfContractID);
            ddl = WTSUtility.CreateGridDropdownList(_dtSystemAdd, "System", "WTS_SYSTEM", "WTS_SYSTEMID", itemId, selectedId, Server.HtmlDecode(selectedText), null);
            ddl.Attributes.Add("field", "System");
            row.Cells[DCC.IndexOf("System")].Controls.Add(ddl);

            bool primary = false;
            if (row.Cells[DCC.IndexOf("Primary")].HasControls()
                && row.Cells[DCC.IndexOf("Primary")].Controls[0] is CheckBox)
            {
                primary = ((CheckBox)row.Cells[DCC.IndexOf("Primary")].Controls[0]).Checked;
            }
            else if (row.Cells[DCC.IndexOf("Primary")].Text == "1")
            {
                primary = true;
            }
            row.Cells[DCC.IndexOf("Primary")].Controls.Clear();
            CheckBox chk = WTSUtility.CreateGridCheckBox("Primary", itemId, primary);
            chk.Attributes.Add("field", "Primary");
            chk.Enabled = false;
            row.Cells[DCC.IndexOf("Primary")].Controls.Add(chk);

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

        if (!CanEdit)
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
            row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("System")].Text.Trim()));
        }
    }

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtMD_Contract_System"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_Contract_System"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("System")
                && i != DCC.IndexOf("Primary")
                && i != DCC.IndexOf("Archive")
                && i != DCC.IndexOf("X")
                && i != DCC.IndexOf("Y"))
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
        row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("Suite")].Style["width"] = "250px";
        row.Cells[DCC.IndexOf("System")].Style["width"] = "200px";
        row.Cells[DCC.IndexOf("Primary")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Archive")].Style["width"] = "55px";
    }
    #endregion Grid


    [WebMethod(true)]
    public static string SaveChanges(string contractID, string rows)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
            , { "failed", "0" }
            , { "savedIds", "" }
            , { "failedIds", "" }
            , { "error", "" } };
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
                int contract_ID = 0, id = 0, WTS_SYSTEMID = 0, primary = 0, archive = 0;
                bool duplicate = false;

                //save
                foreach (DataRow dr in dtjson.Rows)
                {
                    id = WTS_SYSTEMID = 0;
                    primary = 0;
                    archive = 0;
                    duplicate = false;

                    tempMsg = string.Empty;
                    int.TryParse(contractID, out contract_ID);
                    int.TryParse(dr["WTS_SYSTEM_CONTRACTID"].ToString(), out id);
                    int.TryParse(dr["System"].ToString(), out WTS_SYSTEMID);
                    int.TryParse(dr["Primary"].ToString(), out primary);
                    int.TryParse(dr["Archive"].ToString(), out archive);

                    if (WTS_SYSTEMID == 0)
                    {
                        tempMsg = "You must specify a System.";
                        saved = false;
                    }
                    else
                    {
                        if (id == 0)
                        {
                            exists = false;
                            saved = MasterData.WTS_System_Contract_Add(WTS_SYSTEMID: WTS_SYSTEMID
                                , CONTRACTID: contract_ID
                                , primary: (primary == 1)
                                , exists: out exists
                                , newID: out id
                                , errorMsg: out tempMsg);
                            if (exists)
                            {
                                saved = false;
                                tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate System - Contract record.");
                            }
                        }
                        else
                        {
                            saved = MasterData.WTS_System_Contract_Update(WTS_SYSTEMID: WTS_SYSTEMID
                                , WTS_SYSTEM_CONTRACTID: id
                                , CONTRACTID: contract_ID
                                , primary: (primary == 1)
                                , archive: (archive == 1)
                                , duplicate: out duplicate
                                , errorMsg: out tempMsg);
                        }
                    }

                    if (saved)
                    {
                        ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
                        savedQty += 1;
                    }
                    else
                    {
                        failedQty += 1;
                    }

                    if (tempMsg.Length > 0)
                    {
                        errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
                    }

                }

                string cleanupError = string.Empty;
                bool cleanup = MasterData.WTS_Contract_System_Cleanup(errorMsg: out cleanupError);
            }
        }
        catch (Exception ex)
        {
            saved = false;
            errorMsg = ex.Message;
            LogUtility.LogException(ex);
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
                deleted = MasterData.WTS_System_Contract_Delete(WTS_SYSTEM_CONTRACTID: itemId, exists: out exists, errorMsg: out errorMsg);
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