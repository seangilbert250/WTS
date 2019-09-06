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


public partial class MDGrid_System_Resource : System.Web.UI.Page
{
    protected DataTable _dtResource = null;
    protected DataTable _dtRole = null;
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;

    protected int _qfSystemID = 0;
    protected int _qfReleaseID = 0;

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
        if (Request.QueryString["SystemID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["SystemID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["SystemID"].ToString()), out this._qfSystemID);
        }
        if (Request.QueryString["ReleaseID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ReleaseID"].ToString()), out this._qfReleaseID);
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
        _dtResource = MasterData.WTS_Resource_Get(includeArchive: false);
        _dtRole = AOR.AORRoleList_Get();

        DataTable dt = MasterData.WTS_System_ResourceList_Get(WTS_SYSTEMID: this._qfSystemID, ProductVersionID: this._qfReleaseID);

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
                    case "USERNAME":
                        displayName = "Resource";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "AORRoleName":
                        displayName = "Role";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Allocation":
                        displayName = "Allocation %";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "EnterpriseAllocation":
                        displayName = "Enterprise Allocation %";
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
        string itemId = row.Cells[DCC.IndexOf("WTS_SYSTEM_RESOURCEID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            string selectedId = row.Cells[DCC.IndexOf("WTS_RESOURCEID")].Text;
            string selectedText = row.Cells[DCC.IndexOf("USERNAME")].Text;
            DropDownList ddl = null;
            ddl = WTSUtility.CreateGridDropdownList(_dtResource, "USERNAME", "USERNAME", "WTS_RESOURCEID", itemId, selectedId, selectedText, null);
            row.Cells[DCC.IndexOf("USERNAME")].Controls.Add(ddl);

            selectedId = row.Cells[DCC.IndexOf("AORRoleID")].Text;
            selectedText = row.Cells[DCC.IndexOf("AORRoleName")].Text;
            ddl = WTSUtility.CreateGridDropdownList(_dtRole, "Role", "Role", "AORRole_ID", itemId, selectedId, selectedText, null);
            row.Cells[DCC.IndexOf("AORRoleName")].Controls.Add(ddl);
            row.Cells[DCC.IndexOf("Allocation")].Controls.Add(WTSUtility.CreateGridTextBox("Allocation", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Allocation")].Text.Trim()), true));

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

        row.Cells[DCC.IndexOf("EnterpriseAllocation")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("EnterpriseAllocation")].Text, row.Cells[DCC.IndexOf("WTS_RESOURCEID")].Text, row.Cells[DCC.IndexOf("USERNAME")].Text));

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
            row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("USERNAME")].Text.Trim() + " - " + row.Cells[DCC.IndexOf("AORRoleName")].Text.Trim()));
        }
    }

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtMD_System_Resource"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_System_Resource"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("USERNAME")
                && i != DCC.IndexOf("AORRoleName")
                && i != DCC.IndexOf("Allocation")
                && i != DCC.IndexOf("EnterpriseAllocation")
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
        row.Cells[DCC.IndexOf("USERNAME")].Style["width"] = "200px";
        row.Cells[DCC.IndexOf("AORRoleName")].Style["width"] = "200px";
        row.Cells[DCC.IndexOf("Allocation")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("EnterpriseAllocation")].Style["width"] = "85px";
        row.Cells[DCC.IndexOf("Archive")].Style["width"] = "55px";
    }

    private LinkButton CreateLink(string EnterpriseAllocation, string ResourceID, string Resource)
    {
        LinkButton lb = new LinkButton();

        lb.Text = EnterpriseAllocation;
        lb.Attributes["onclick"] = string.Format("openSystemResource({0}, {1}, '{2}'); return false;", this._qfReleaseID, ResourceID, Resource);

        return lb;
    }
    #endregion Grid


    [WebMethod(true)]
    public static string SaveChanges(string systemID, string releaseID, string rows)
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
                int system_ID = 0, release_ID = 0, id = 0, WTS_RESOURCEID = 0, AORRoleID = 0, Allocation = 0, archive = 0;
                bool duplicate = false;

                //save
                foreach (DataRow dr in dtjson.Rows)
                {
                    id = WTS_RESOURCEID = AORRoleID = Allocation = 0;
                    archive = 0;
                    duplicate = false;

                    tempMsg = string.Empty;
                    int.TryParse(systemID, out system_ID);
                    int.TryParse(releaseID, out release_ID);
                    int.TryParse(dr["WTS_SYSTEM_RESOURCEID"].ToString(), out id);
                    int.TryParse(dr["USERNAME"].ToString(), out WTS_RESOURCEID);
                    int.TryParse(dr["AORRoleName"].ToString(), out AORRoleID);
                    int.TryParse(dr["Allocation"].ToString(), out Allocation);
                    int.TryParse(dr["Archive"].ToString(), out archive);

                    if (WTS_RESOURCEID == 0)
                    {
                        tempMsg = "You must specify a Resource.";
                        saved = false;
                    }
                    else
                    {
                        if (id == 0)
                        {
                            exists = false;
                            saved = MasterData.WTS_System_Resource_Add(WTS_SYSTEMID: system_ID
                                , ProductVersionID: release_ID
                                , WTS_RESOURCEID: WTS_RESOURCEID
                                , ActionTeam: 0
                                , AORRoleID: AORRoleID
                                , Allocation: Allocation
                                , exists: out exists
                                , newID: out id
                                , errorMsg: out tempMsg);
                            if (exists)
                            {
                                saved = false;
                                tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate System - Resource record.");
                            }
                        }
                        else
                        {
                            saved = MasterData.WTS_System_Resource_Update(WTS_SYSTEMID: system_ID
                                , ProductVersionID: release_ID
                                , WTS_SYSTEM_RESOURCEID: id
                                , WTS_RESOURCEID: WTS_RESOURCEID
                                , AORRoleID: AORRoleID
                                , Allocation: Allocation
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
                deleted = MasterData.WTS_System_Resource_Delete(WTS_SYSTEM_RESOURCEID: itemId, exists: out exists, errorMsg: out errorMsg);
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