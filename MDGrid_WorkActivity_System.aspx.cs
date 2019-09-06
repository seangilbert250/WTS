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


public partial class MDGrid_WorkActivity_System : System.Web.UI.Page
{
	protected DataTable _dtWorkActivity = null;
	protected DataTable _dtSystem = null;

    protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;

	protected int _qfWorkItemTypeID = 0;
    protected int _qfSystemSuiteID = 0;

    protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

	protected bool CanView = false;
	protected bool CanEdit = false;
	protected bool IsAdmin = false;

    protected string cvValue = "0";

    protected void Page_Load(object sender, EventArgs e)
    {
        cvValue = Request.QueryString["ChildView"] == null ? "0" : Request.QueryString["ChildView"].Trim();
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
		if (Request.QueryString["WorkItemTypeID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["WorkItemTypeID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["WorkItemTypeID"].ToString()), out this._qfWorkItemTypeID);
        }
        if (Request.QueryString["SystemSuiteID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["SystemSuiteID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["SystemSuiteID"].ToString()), out this._qfSystemSuiteID);
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
		_dtSystem = MasterData.SystemList_Get(includeArchive: false, WTS_SYSTEM_SUITEID: _qfSystemSuiteID);
        _dtWorkActivity = MasterData.ItemTypeList_Get();

        DataTable dt = MasterData.ItemType_SystemList_Get(WorkItemTypeID: this._qfWorkItemTypeID, SystemSuiteID: this._qfSystemSuiteID);

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
					case "X":
						blnVisible = true;
						break;
					case "WTS_SYSTEM_WORKACTIVITYID":
						blnVisible = false;
						blnSortable = false;
						break;
					case "WORKITEMTYPEID":
						blnVisible = false;
						blnSortable = false;
						break;
					case "WORKITEMTYPE":
						displayName = "Work Activity";
						blnVisible = true;
						blnSortable = true;
						break;
                    case "WTS_SYSTEMID":
						blnVisible = false;
						blnSortable = false;
						break;
					case "WTS_SYSTEM":
						displayName = "System(Task)";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = true;
						break;
                    case "ARCHIVE":
						displayName = "Archive";
						blnVisible = true;
						blnSortable = true;
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
	}

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("WORKITEMTYPEID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			string selectedId = row.Cells[DCC.IndexOf("WORKITEMTYPEID")].Text;
			string selectedText = row.Cells[DCC.IndexOf("WORKITEMTYPE")].Text;
			DropDownList ddl = null;
			ddl = WTSUtility.CreateGridDropdownList(_dtWorkActivity, "Item Type", "Item Type", "WORKITEMTYPEID", itemId, _qfWorkItemTypeID.ToString(), selectedText, null);
			ddl.Enabled = (_qfWorkItemTypeID == 0);
			row.Cells[DCC.IndexOf("WORKITEMTYPE")].Controls.Add(ddl);

            selectedId = row.Cells[DCC.IndexOf("WTS_SYSTEMID")].Text.Replace("&nbsp;", " ").Trim();
            selectedText = row.Cells[DCC.IndexOf("WTS_SYSTEM")].Text.Replace("&nbsp;", " ").Trim();

            if (string.IsNullOrWhiteSpace(selectedId))
            {
                selectedId = "0";
            }
            ddl = WTSUtility.CreateGridDropdownList(_dtSystem, "WTS_SYSTEM", "WTS_SYSTEM", "WTS_SYSTEMID", itemId, selectedId, Server.HtmlDecode(selectedText), null);
			row.Cells[DCC.IndexOf("WTS_SYSTEM")].Controls.Add(ddl);
            TextBox descriptionBox = null;
            descriptionBox = WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;", " ").Trim()));
            descriptionBox.Enabled = false;
            row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(descriptionBox);

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
			Image imgBlank = new Image();
			imgBlank.Height = 10;
			imgBlank.Width = 10;
			imgBlank.ImageUrl = "Images/Icons/blank.png";
			imgBlank.AlternateText = "";
			row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
		}
		else
		{
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(row.Cells[DCC.IndexOf("WTS_SYSTEM_WORKACTIVITYID")].Text.Trim(), row.Cells[DCC.IndexOf("WTS_SYSTEM_WORKACTIVITYID")].Text.Trim() + " - " + row.Cells[DCC.IndexOf("WORKITEMTYPE")].Text.Trim()));
		}
	}

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_WorkActivity_System"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkActivity_System"];
		}
	}

    void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("WORKITEMTYPE")
				&& i != DCC.IndexOf("WTS_SYSTEM")
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
		row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
		row.Cells[DCC.IndexOf("WTS_SYSTEM")].Style["width"] = "150px";
		row.Cells[DCC.IndexOf("WORKITEMTYPE")].Style["width"] = "225px";
        row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
	}

    #endregion Grid

    [WebMethod(true)]
	public static string SaveChanges(string WorkItemTypeID, string rows, string cvValue)
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
                int id = 0, workItemType_ID = 0, systemID = 0, archive = 0;
                bool duplicate = false;

                HttpServerUtility server = HttpContext.Current.Server;
                //save
                foreach (DataRow dr in dtjson.Rows)
                {
                    id = systemID = workItemType_ID = archive = 0;
                    duplicate = false;

                    tempMsg = string.Empty;
                    int.TryParse(dr["WTS_SYSTEM_WORKACTIVITYID"].ToString(), out id);
                    int.TryParse(dr["WTS_SYSTEM"].ToString(), out systemID);
                    int.TryParse(WorkItemTypeID, out workItemType_ID);
                    int.TryParse(dr["ARCHIVE"].ToString(), out archive);

                    if (workItemType_ID == 0)
                    {
                        tempMsg = "You must specify a Resource.";
                        saved = false;
                    }
                    else
                    {
                        if (id == 0)
                        {
                            exists = false;
                            saved = MasterData.WTS_System_WorkActivity_Add(WTS_SYSTEMID: systemID
                                , WTS_WORKACTIVITYID: workItemType_ID
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
                            saved = MasterData.WTS_System_WorkActivity_Update(WTS_SYSTEMID: systemID
                                , WTS_SYSTEM_WORKACTIVITYID: id
                                , WTS_WORKACTIVITYID: workItemType_ID
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
                deleted = MasterData.WTS_System_WorkActivity_Delete(WTS_SYSTEM_WORKACTIVITYID: itemId, exists: out exists, errorMsg: out errorMsg);
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