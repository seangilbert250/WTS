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

public partial class MDGrid_AOREstimation : System.Web.UI.Page
{
    #region "Member Variables"
    protected DataTable _dtAOREstimate = null;
    protected DataColumnCollection DCC;
    protected GridCols _columnData = new GridCols();

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
    #endregion

    #region "Page Methods"
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
            if (dtjson == null || dtjson.Rows.Count == 0)
            {
                errorMsg = "Unable to save. No list of changes was provided.";
            }
            else
            {
                int id = 0;
                string name = string.Empty, description = string.Empty, notes = string.Empty;

                HttpServerUtility server = HttpContext.Current.Server;
                //save
                foreach (DataRow dr in dtjson.Rows)
                {
                    id = 0;
                    name = string.Empty;
                    description = string.Empty;
                    notes = string.Empty;

                    tempMsg = string.Empty;
                    int.TryParse(dr["AOREstimationID"].ToString(), out id);
                    name = server.UrlDecode(dr["AOREstimationName"].ToString());
                    description = server.UrlDecode(dr["Description"].ToString());
                    notes = server.UrlDecode(dr["Notes"].ToString());

                    if (id == 0)
                    {
                        exists = false;
                        saved = MasterData.AOREstimation_Add(
                             AorEstimationID: id, AOREstimationName: name, Descr: description, Notes: notes, exists: out exists, newID: out id, errorMsg: out tempMsg);
                        if (exists)
                        {
                            saved = false;
                            tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate AOR Estimation record.");
                        }
                    }
                    else
                    {
                        saved = MasterData.AOREstimation_Update(
                            AorEstimationID: id, AOREstimationName: name, Descr: description, Notes: notes, duplicate: out duplicate, errorMsg: out tempMsg);
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
                deleted = MasterData.AOREstimation_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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

    #endregion

    #region "Page_Load"
    protected void Page_Load(object sender, EventArgs e)
    {
        this.IsAdmin = UserManagement.UserIsInRole("Admin");
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
        this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.MasterData));

        initControls();

        loadGridData();

    }

    private void initControls()
    {
        grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
        grdMD.GridRowDataBound += grdMD_GridRowDataBound;
        grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
    }
    #endregion

    #region "LoadData"
    private void loadGridData()
    {
        DataTable dt = MasterData.AOREstimation_Get(0, false);

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

            InitializeColumnData(ref dt);
            dt.AcceptChanges();

            spanRowCount.InnerHtml = dt.Rows.Count.ToString();
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
                    case "X":
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                    case "AOREstimationID":
                        displayName = "AOREstimationID";
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "AOREstimationName":
                        displayName = "Name";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Description":
                        displayName = "Description";
                        blnVisible = true;
                        blnSortable = false;
                        break;
                    case "Notes":
                        displayName = "Notes";
                        blnVisible = true;
                        blnSortable = false;
                        break;
                    case "CreatedBy":
                        displayName = "Created By";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "UpdatedBy":
                        displayName = "Updated By";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    //case "Archive":
                    //    displayName = "Archive";
                    //    blnVisible = false;
                    //    blnSortable = false;
                    //    break;
                    case "Y":
                        displayName = "&nbsp;";
                        blnVisible = true;
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
    #endregion

    #region Grid

    void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        _columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        _columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        //add edit link
        string itemId = row.Cells[DCC.IndexOf("AOREstimationID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            row.Cells[DCC.IndexOf("AOREstimationName")].Controls.Add(WTSUtility.CreateGridTextBox(field: "AOREstimationName", itemId: itemId, text: Server.HtmlDecode(row.Cells[DCC.IndexOf("AOREstimationName")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("Description")].Controls.Add(WTSUtility.CreateGridTextBox(field: "Description", itemId: itemId, text: Server.HtmlDecode(row.Cells[DCC.IndexOf("Description")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("Notes")].Controls.Add(WTSUtility.CreateGridTextBox(field: "Notes", itemId: itemId, text: Server.HtmlDecode(row.Cells[DCC.IndexOf("Notes")].Text.Replace("&nbsp;", " ").Trim())));

            //bool archive = false;
            //if (row.Cells[DCC.IndexOf("ARCHIVE")].HasControls()
            //    && row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0] is CheckBox)
            //{
            //    archive = ((CheckBox)row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0]).Checked;
            //}
            //else if (row.Cells[DCC.IndexOf("ARCHIVE")].Text == "1")
            //{
            //    archive = true;
            //}
            //row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Clear();
            //row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Add(WTSUtility.CreateGridCheckBox("Archive", itemId, archive));
        }
        if (CanEdit)
        {
            row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("AOREstimationName")].Text.Trim()));
        }
    }

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtMD_Effort"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_Effort"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("X")
                && i != DCC.IndexOf("AOREstimationID")
                && i != DCC.IndexOf("Archive")
                && i != DCC.IndexOf("Y")
                )
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
        row.Cells[DCC.IndexOf("AOREstimationID")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("AOREstimationName")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("Description")].Style["width"] = "200px";
        row.Cells[DCC.IndexOf("Notes")].Style["width"] = "200px";
        row.Cells[DCC.IndexOf("CreatedBy")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("UpdatedBy")].Style["width"] = "100px";
        //row.Cells[DCC.IndexOf("Archive")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("Y")].Style["width"] = "12px";
    }

    #endregion Grid
}