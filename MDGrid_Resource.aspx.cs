﻿using System;
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


public partial class MDGrid_Resource : System.Web.UI.Page
{
    protected DataTable _dtResource = null;
    protected DataTable _dtAORRole = null;
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;

    protected int _qfSystemSuiteID = 0;
    protected int _qfProductVersionID = 0;
    protected int CurrentLevel = 0;

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
		if (Request.QueryString["sortOrder"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
		{
			this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
            HttpContext.Current.Session["suiteResourceSort"] = SortOrder;
        }
        else
        {
            if (HttpContext.Current.Session["suiteResourceSort"] == null)
            {
                this.SortOrder = "Intake Team|2~Resource Type|1~Resource|1~|1~false";
            } else
            {
                this.SortOrder = HttpContext.Current.Session["suiteResourceSort"].ToString();
            }
        }
        if (Request.QueryString["CurrentLevel"] != null && 
            !string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"]))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["CurrentLevel"]), out this.CurrentLevel);
        }
        if (Request.QueryString["WTS_SYSTEM_SUITEID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["WTS_SYSTEM_SUITEID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["WTS_SYSTEM_SUITEID"].ToString()), out this._qfSystemSuiteID);
        }
        if (Request.QueryString["ProductVersionID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ProductVersionID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ProductVersionID"].ToString()), out this._qfProductVersionID);
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
        _dtAORRole = AOR.AORRoleList_Get();
        _dtResource = UserManagement.LoadUserList();
        DataSet ds = MasterData.WTS_System_Suite_ResourceList_Get(SystemSuiteID: _qfSystemSuiteID, ProductVersionID: _qfProductVersionID);
        DataTable dt = ds.Tables["SuiteGrid"];
		HttpContext.Current.Session["dtMD_Resource"] = dt;

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			InitializeColumnData(ref dt);
			dt.AcceptChanges();

            if (_dtResource != null && _dtResource.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _dtResource.DefaultView.RowFilter = string.Format("NOT WTS_RESOURCEID = {0} ", dr["WTS_RESOURCEID"].ToString());
                    _dtResource = _dtResource.DefaultView.ToTable();
                }
            }
        }

        DataTable dtSystems = MasterData.SystemList_Get(WTS_SYSTEM_SUITEID: _qfSystemSuiteID);
        dtSystems.DefaultView.RowFilter = "WTS_SYSTEMID NOT IN (0)";
        dtSystems = dtSystems.DefaultView.ToTable();
        msResourceSystems.Items = dtSystems;

        ddlResource.DataSource = _dtResource;
        ddlResource.DataValueField = "WTS_RESOURCEID";
        ddlResource.DataTextField = "USERNAME";
        ddlResource.DataBind();

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
				blnVisible = true;
				blnSortable = true;
				blnOrderable = true;
				groupName = string.Empty;

				switch (gridColumn.ColumnName)
				{
                    case "WTS_RESOURCEID":
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = false;
                        break;
                    case "USERNAME":
						displayName = "Resource";
						break;
                    case "WTS_RESOURCE_TYPE":
                        displayName = "Resource Type";
                        break;
                    case "CreatedBy":
                        displayName = "Associated By";
                        break;
                    case "CreatedDate":
                        displayName = "Associated Date";
                        break;
                    case "ActionTeam":
                        displayName = "Intake Team";
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
        row.Cells[DCC.IndexOf("USERNAME")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("WTS_RESOURCE_TYPE")].Style["text-align"] = "center";
    }

	void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("WTS_RESOURCEID")].Text.Trim();

        if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
            string selectedId = row.Cells[DCC.IndexOf("WTS_RESOURCEID")].Text;
            string selectedText = Server.HtmlDecode(row.Cells[DCC.IndexOf("USERNAME")].Text);
            DropDownList ddl = null;
            ddl = WTSUtility.CreateGridDropdownList(_dtResource, "USERNAME", "USERNAME", "WTS_RESOURCEID", itemId, selectedId, selectedText, null);
            row.Cells[DCC.IndexOf("USERNAME")].Controls.Add(ddl);

            bool actionTeam = false;
            if (row.Cells[DCC.IndexOf("ActionTeam")].HasControls()
                && row.Cells[DCC.IndexOf("ActionTeam")].Controls[0] is CheckBox)
            {
                actionTeam = ((CheckBox)row.Cells[DCC.IndexOf("ActionTeam")].Controls[0]).Checked;
            }
            else if (row.Cells[DCC.IndexOf("ActionTeam")].Text == "1")
            {
                actionTeam = true;
            }

            row.Cells[DCC.IndexOf("ActionTeam")].Controls.Clear();
            row.Cells[DCC.IndexOf("ActionTeam")].Controls.Add(CreateCheckBox(Server.HtmlDecode(row.Cells[DCC.IndexOf("WTS_RESOURCEID")].Text.Replace("&nbsp;", " ")), actionTeam ? "1" : "0"));

            for (int x = 7; x < row.Cells.Count - 1; x++)
            {
                row.Cells[x].Controls.Add(CreateCheckBox(Server.HtmlDecode(row.Cells[DCC.IndexOf("WTS_RESOURCEID")].Text.Replace("&nbsp;", " ")), Server.HtmlDecode(row.Cells[x].Text)));
            }
        }
    }

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_Resource"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_Resource"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
                && i != DCC.IndexOf("USERNAME"))
			{
                row.Cells[i].Style["text-align"] = "center";
                row.Cells[i].Style["padding-left"] = "0px";
                row.Cells[i].Style["width"] = "95px";
            }
            else
			{
                row.Cells[i].Style["text-align"] = "left";
                row.Cells[i].Style["padding-left"] = "5px";
            }
		}

		//more column formatting
		row.Cells[DCC.IndexOf("USERNAME")].Style["width"] = "150px";
        row.Cells[DCC.IndexOf("WTS_RESOURCE_TYPE")].Style["width"] = "125px";
        row.Cells[DCC.IndexOf("Workload Priority")].Style["width"] = "150px";
        row.Cells[DCC.IndexOf("CreatedBy")].Style["width"] = "150px";
        row.Cells[DCC.IndexOf("CreatedDate")].Style["width"] = "150px";
    }

    private CheckBox CreateCheckBox(string id, string value)
    {
        CheckBox chk = new CheckBox();

        chk.Attributes["onchange"] = "txt_change(this);";
        chk.Attributes.Add("resourceid", id);

        if (value == "1") chk.Checked = true;

        return chk;
    }
    #endregion Grid

    [WebMethod(true)]
    public static string SaveChanges(string rows, int systemSuiteID = 0, int productVersionID = 0)
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

            int id = 0, actionTeam = 0, resourceID = 0, newID = 0;
            string systems = string.Empty;

            HttpServerUtility server = HttpContext.Current.Server;
            //save
            foreach (DataRow dr in dtjson.Rows)
            {
                id = 0;
                actionTeam = 0;
                resourceID = 0;

                tempMsg = string.Empty;
                int.TryParse(dr["WTS_RESOURCEID"].ToString(), out resourceID);
                int.TryParse(dr["USERNAME"].ToString(), out id);
                int.TryParse(dr["ActionTeam"].ToString(), out actionTeam);

                for (int x = 7; x < dtjson.Columns.Count; x++)
                {
                    if (dr[dtjson.Columns[x].ToString()].ToString() == "1") systems += "'" + dtjson.Columns[x].ToString() + "',";
                }

                if (systemSuiteID > 0)
                {
                    DataSet ds = MasterData.WTS_System_Suite_ResourceList_Get(SystemSuiteID: systemSuiteID, ProductVersionID: productVersionID);
                    DataTable dtSystem = ds.Tables["SystemResourceList"];
                    dtSystem.DefaultView.RowFilter = "(WTS_RESOURCEID = " + resourceID + ")";
                    dtSystem = dtSystem.DefaultView.ToTable();

                    foreach (DataRow drSystem in dtSystem.Rows)
                    {
                        int systemID = 0;
                        int.TryParse(drSystem["WTS_SYSTEM_RESOURCEID"].ToString(), out systemID);
                        if (systemID > 0)
                        {
                            MasterData.WTS_System_Resource_Delete(WTS_SYSTEM_RESOURCEID: systemID, exists: out exists, errorMsg: out errorMsg);
                        }
                    }

                    if (systems.Length > 0)
                    {
                        dtSystem = MasterData.SystemList_Get(includeArchive: true);
                        dtSystem.DefaultView.RowFilter = "(WTS_SystemSuiteID = " + systemSuiteID + "OR WTS_SystemID = 0) AND ARCHIVE = 0 AND WTS_SYSTEM IN (" + systems + ")";
                        dtSystem = dtSystem.DefaultView.ToTable();
                        foreach (DataRow drSystem in dtSystem.Rows)
                        {
                            int systemID = 0, waSystemID = 0;
                            int.TryParse(drSystem["WTS_SystemID"].ToString(), out systemID);
                            if (systemID > 0)
                            {
                                saved = MasterData.WTS_System_Resource_Add(WTS_SYSTEMID: systemID, ProductVersionID: productVersionID, WTS_RESOURCEID: id, ActionTeam: actionTeam, AORRoleID: 0, Allocation: 0, exists: out exists, newID: out waSystemID, errorMsg: out tempMsg);
                            }
                        }
                    }
                    else
                    {
                        saved = true;
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
    public static string SaveResource(string systems, int resource, bool actionTeam, int systemSuiteID, int productionVersionID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            string[] Systems = systems.Split(',');
            if (Systems.Length == 0)
            {
                errorMsg = "Unable to save. An invalid list of Systems was provided.";
                saved = false;
            }

            HttpServerUtility server = HttpContext.Current.Server;
            //save
            for (int x = 0; x < Systems.Length; x++)
            {
                if (resource == 0)
                {
                    tempMsg = "You must specify a Work Area.";
                    saved = false;
                }
                else
                {
                    exists = false;
                    int systemID = 0, waSystemID = 0;
                    int.TryParse(Systems[x].ToString(), out systemID);
                    if (systemID > 0)
                    {
                        saved = MasterData.WTS_System_Resource_Add(WTS_SYSTEMID: systemID, ProductVersionID: productionVersionID, WTS_RESOURCEID: resource, ActionTeam: actionTeam ? 1 : 0, AORRoleID: 0, Allocation: 0, exists: out exists, newID: out waSystemID, errorMsg: out tempMsg);
                    }
                }

                if (saved)
                {
                    ids += string.Format("{0}{1}", x < Systems.Length - 1 ? "," : "", resource.ToString());
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

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WORK_AREA);

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
    public static string RemoveItem(int itemId, int systemSuiteID, int productVersionID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "id", itemId.ToString() }
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
                errorMsg = "You must specify an item to remove systems.";
            }
            else
            {
                if (systemSuiteID > 0)
                {
                    DataSet ds = MasterData.WTS_System_Suite_ResourceList_Get(SystemSuiteID: systemSuiteID, ProductVersionID: productVersionID);
                    DataTable dtSystem = ds.Tables["SystemResourceList"];
                    dtSystem.DefaultView.RowFilter = "(WTS_RESOURCEID = " + itemId + ")";
                    dtSystem = dtSystem.DefaultView.ToTable();

                    foreach (DataRow drSystem in dtSystem.Rows)
                    {
                        int systemID = 0;
                        int.TryParse(drSystem["WTS_SYSTEM_RESOURCEID"].ToString(), out systemID);
                        if (systemID > 0)
                        {
                            deleted = MasterData.WTS_System_Resource_Delete(WTS_SYSTEM_RESOURCEID: systemID, exists: out exists, errorMsg: out errorMsg);
                        }
                    }
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