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


public partial class MDGrid_WorkType_ResourceType : System.Web.UI.Page
{
	protected DataTable _dtWorkType = null;
	protected DataTable _dtResourceType = null;
    protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfWorkTypeID = 0;

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
		if (Request.QueryString["WorkTypeID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["WorkTypeID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["WorkTypeID"].ToString()), out this._qfWorkTypeID);
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
		_dtWorkType = MasterData.WorkTypeList_Get(includeArchive: false);
		_dtResourceType = MasterData.ResourceTypeList_Get(includeArchive: false);
        DataTable dt = MasterData.WorkType_ResourceTypeList_Get(workTypeID: this._qfWorkTypeID);

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			ListItem item = null;
			foreach (DataRow row in _dtWorkType.Rows)
			{
				item = ddlQF_WorkType.Items.FindByValue(row["WorkTypeID"].ToString());
				if (item == null)
				{
					ddlQF_WorkType.Items.Add(new ListItem(row["WorkType"].ToString(), row["WorkTypeID"].ToString()));
				}
			}
			item = ddlQF_WorkType.Items.FindByValue(_qfWorkTypeID.ToString());
			if (item != null)
			{
				item.Selected = true;
			}

			InitializeColumnData(ref dt);
			dt.AcceptChanges();
		}

		int count = dt.Rows.Count;
		count = count > 0 ? count - 1 : count; //need to subtract the empty row
		spanRowCount.InnerText = count.ToString();

        _dtResourceType.DefaultView.RowFilter = "WTS_RESOURCE_TYPEID NOT IN ('4')";
        _dtResourceType = _dtResourceType.DefaultView.ToTable();

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
					case "WorkType_WTS_RESOURCE_TYPEID":
						displayName = "WorkType_WTS_RESOURCE_TYPEID";
						blnVisible = false;
						blnSortable = false;
						break;
					case "WorkTypeID":
						displayName = "WorkTypeID";
						blnVisible = false;
						blnSortable = false;
						break;
					case "WorkType":
						displayName = "Resource Group";
						blnVisible = true;
						blnSortable = true;
						break;
					case "WorkType_SORT_ORDER":
						displayName = "WorkType_SORT_ORDER";
						blnVisible = false;
						blnSortable = true;
						break;
					case "WTS_RESOURCE_TYPEID":
						displayName = "WTS_RESOURCE_TYPEID";
						blnVisible = false;
						blnSortable = false;
						break;
					case "WTS_RESOURCE_TYPE":
						displayName = "Resource Type";
						blnVisible = true;
						blnSortable = true;
						break;
					case "WTS_RESOURCE_TYPE_SORT_ORDER":
						displayName = "WTS_RESOURCE_TYPE_SORT_ORDER";
						blnVisible = false;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = true;
						break;
					case "WorkItem_Count":
						displayName = "Work Items";
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

        row.Cells[DCC.IndexOf("Description")].Style["text-align"] = "center";
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("WorkType_WTS_RESOURCE_TYPEID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			string selectedId = row.Cells[DCC.IndexOf("WorkTypeID")].Text;
			string selectedText = row.Cells[DCC.IndexOf("WorkType")].Text;
			if (itemId == "0" && _qfWorkTypeID != 0)
			{
				selectedId = _qfWorkTypeID.ToString();
				selectedText = string.Empty;
			}
			DropDownList ddl = null;
			ddl = WTSUtility.CreateGridDropdownList(_dtWorkType, "WorkType", "WorkType", "WorkTypeID", itemId, selectedId, selectedText, null);
			ddl.Enabled = (_qfWorkTypeID == 0);
			row.Cells[DCC.IndexOf("WorkType")].Controls.Add(ddl);

			selectedId = row.Cells[DCC.IndexOf("WTS_RESOURCE_TYPEID")].Text;
			selectedText = row.Cells[DCC.IndexOf("WTS_RESOURCE_TYPE")].Text;

            ddl = WTSUtility.CreateGridDropdownList(_dtResourceType, "WTS_RESOURCE_TYPE", "WTS_RESOURCE_TYPE", "WTS_RESOURCE_TYPEID", itemId, selectedId, selectedText, null);
            row.Cells[DCC.IndexOf("WTS_RESOURCE_TYPE")].Controls.Add(ddl);

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

		//string dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkItem_Count")].Text).Trim();
		//int count = 0;
		if (!CanEdit)
		//|| !int.TryParse(dependencies, out count)
		//|| count > 0)
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
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WTS_RESOURCE_TYPE")].Text.Trim() + " - " + row.Cells[DCC.IndexOf("WorkType")].Text.Trim()));
		}
	}

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_WorkType_WTS_RESOURCE_TYPE"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkType_WTS_RESOURCE_TYPE"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("WorkType")
				&& i != DCC.IndexOf("WTS_RESOURCE_TYPE")
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
		row.Cells[DCC.IndexOf("WorkType")].Style["width"] = "150px";
		row.Cells[DCC.IndexOf("WTS_RESOURCE_TYPE")].Style["width"] = "150px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
	}

	#endregion Grid


	[WebMethod(true)]
	public static string SaveChanges(string rows)
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

			int id = 0, resourceTypeID = 0, workTypeID = 0, archive = 0;
			string allocation = string.Empty, description = string.Empty;

			HttpServerUtility server = HttpContext.Current.Server;
			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
                resourceTypeID = 0;
				workTypeID = 0;
				description = string.Empty;
				archive = 0;

				tempMsg = string.Empty;
				int.TryParse(dr["WorkType_WTS_RESOURCE_TYPEID"].ToString(), out id);
				int.TryParse(dr["WTS_RESOURCE_TYPE"].ToString(), out resourceTypeID);
				int.TryParse(dr["WorkType"].ToString(), out workTypeID);
				int.TryParse(dr["ARCHIVE"].ToString(), out archive);

				if (resourceTypeID == 0 || workTypeID == 0)
				{
					tempMsg = "You must specify a Resource Type and a Work Type.";
					saved = false;
				}
				else
				{
					if (id == 0)
					{
						exists = false;
						saved = MasterData.WorkType_ResourceType_Add(resourceTypeID: resourceTypeID, workTypeID: workTypeID, exists: out exists, newID: out id, errorMsg: out tempMsg);
						if (exists)
						{
							saved = false;
							tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate WorkType - Resource Type record.");
						}
					}
					else
					{
						saved = MasterData.WorkType_ResourceType_Update(workTypeResourceTypeID: id, resourceTypeID: resourceTypeID, workTypeID: workTypeID, archive: (archive == 1), errorMsg: out tempMsg);
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
				deleted = MasterData.WorkType_ResourceType_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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