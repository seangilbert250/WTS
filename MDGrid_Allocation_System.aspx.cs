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


public partial class MDGrid_Allocation_System : System.Web.UI.Page
{
	protected DataTable _dtAllocation = null;
	protected DataTable _dtSystem = null;
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfAllocationID = 0;
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
		if (Request.QueryString["AllocationID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["AllocationID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["AllocationID"].ToString()), out this._qfAllocationID);
		}
		if (Request.QueryString["SystemID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["SystemID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["SystemID"].ToString()), out this._qfSystemID);
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
		if (_dtSystem != null && _qfSystemID != 0)
		{
			_dtSystem.DefaultView.RowFilter = string.Format(" WTS_SYSTEMID IN (0, {0}) ", _qfSystemID.ToString());
			_dtSystem = _dtSystem.DefaultView.ToTable();
		}


		DataSet dsAllocation = MasterData.AllocationList_Get(includeArchive: false);
		if(dsAllocation != null && dsAllocation.Tables.Count > 0){
			_dtAllocation = dsAllocation.Tables[0];
		}
		if (_dtAllocation != null && _qfAllocationID != 0)
		{
			_dtAllocation.DefaultView.RowFilter = string.Format(" AllocationID = {0} ", _qfAllocationID.ToString());
			_dtAllocation = _dtAllocation.DefaultView.ToTable();
		}

		DataTable dt = MasterData.Allocation_SystemList_Get(allocationID: this._qfAllocationID, systemID: this._qfSystemID);

		if (dt != null)
		{
			if (_qfSystemID > 0)
			{
				dt.Columns["WTS_SYSTEM"].SetOrdinal(dt.Columns["AllocationID"].Ordinal);
				dt.Columns["WTS_SYSTEMID"].SetOrdinal(dt.Columns["WTS_SYSTEM"].Ordinal);
				dt.AcceptChanges();
			}

			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			ListItem item = null;
			foreach (DataRow row in _dtAllocation.Rows)
			{
				item = ddlQF.Items.FindByValue(row["AllocationID"].ToString());
				if (item == null)
				{
					ddlQF.Items.Add(new ListItem(row["Allocation"].ToString(), row["AllocationID"].ToString()));
				}
			}
			item = ddlQF.Items.FindByValue(_qfAllocationID.ToString());
			if (item != null)
			{
				item.Selected = true;
			}

			foreach (DataRow row in _dtSystem.Rows)
			{
				item = ddlQF_System.Items.FindByValue(row["WTS_SYSTEMID"].ToString());
				if (item == null)
				{
					ddlQF_System.Items.Add(new ListItem(row["WTS_SYSTEM"].ToString(), row["WTS_SYSTEMID"].ToString()));
				}
			}
			item = ddlQF_System.Items.FindByValue(_qfSystemID.ToString());
			if (item != null)
			{
				item.Selected = true;
			}

			InitializeColumnData(ref dt);
			dt.AcceptChanges();
		}

		if (_qfAllocationID != 0 && dt != null && dt.Rows.Count > 0)
		{
			dt.DefaultView.RowFilter = string.Format(" AllocationID =  {0} OR (AllocationID = 0 AND WTS_SYSTEMID = 0) ", _qfAllocationID.ToString());
			dt = dt.DefaultView.ToTable();
		}
		if (_qfSystemID != 0 && dt != null && dt.Rows.Count > 0)
		{
			dt.DefaultView.RowFilter = string.Format(" WTS_SYSTEMID =  {0} OR (WTS_SYSTEMID = 0 AND AllocationID = 0) OR WTS_SYSTEMID IS NULL ", _qfSystemID.ToString());
			dt = dt.DefaultView.ToTable();
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
					case "A":
						blnVisible = true;
						break;
					case "X":
						blnVisible = true;
						break;
					case "Y":
						blnVisible = true;
						break;
					case "Allocation_SystemID":
						blnVisible = false;
						blnSortable = false;
						break;
					case "ALLOCATIONID":
						blnVisible = false;
						blnSortable = false;
						break;
					case "ALLOCATION":
						displayName = "Allocation Assignment";
						blnVisible = true;
						blnSortable = true;
						break;
					case "WTS_SYSTEMID":
						blnVisible = false;
						blnSortable = false;
						break;
					case "WTS_SYSTEM":
						displayName = "System";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = true;
						break;
					case "ProposedPriority":
						displayName = "Proposed Priority";
						blnVisible = true;
						blnSortable = true;
						break;
					case "ApprovedPriority":
						displayName = "Approved Priority";
						blnVisible = true;
						blnSortable = true;
						break;
					case "WorkItem_Count":
						displayName = "Tasks";
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

		row.Cells[DCC.IndexOf("A")].Text = "&nbsp;";

		Image imgSort = new Image();
		imgSort.Height = 14;
		imgSort.Width = 14;
		imgSort.ImageUrl = "Images/Icons/page_sort_a_z.png";
		imgSort.AlternateText = "";
		imgSort.Style["padding"] = "2px";
		imgSort.Style["cursor"] = "pointer";
		imgSort.Attributes.Add("onclick", "imgSort_click();");
		row.Cells[DCC["A"].Ordinal].Controls.Add(imgSort);

		Image imgRefresh = new Image();
		imgRefresh.Height = 14;
		imgRefresh.Width = 14;
		imgRefresh.ImageUrl = "Images/Icons/arrow_refresh_blue.png";
		imgRefresh.AlternateText = "";
		imgRefresh.Style["padding"] = "2px";
		imgRefresh.Style["cursor"] = "pointer";
		imgRefresh.Attributes.Add("onclick", "refreshPage();");
		row.Cells[DCC["A"].Ordinal].Controls.Add(imgRefresh);

		row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";

		row.Cells[DCC.IndexOf("Y")].Text = "&nbsp;";
		if (!CanEdit)
		{
			Image imgBlank = new Image();
			imgBlank.Height = 12;
			imgBlank.Width = 12;
			imgBlank.ImageUrl = "Images/Icons/blank.png";
			imgBlank.AlternateText = "";
			row.Cells[DCC["Y"].Ordinal].Controls.Add(imgBlank);
		}
		else
		{
			Image imgSave = new Image();
			imgSave.ID = "imgSave";
			imgSave.Height = 12;
			imgSave.Width = 12;
			imgSave.ImageUrl = "Images/Icons/disk.png";
			imgSave.AlternateText = "Save Subtask Updates";
			imgSave.Style["cursor"] = "pointer";
			imgSave.Attributes.Add("onclick", "buttonSave_click();");
			row.Cells[DCC["Y"].Ordinal].Controls.Add(imgSave);
            row.Cells[DCC.IndexOf("X")].Controls.Add(createAddLink());
        }

	}

	void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("Allocation_SystemID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			string selectedId = row.Cells[DCC.IndexOf("ALLOCATIONID")].Text;
			string selectedText = row.Cells[DCC.IndexOf("ALLOCATION")].Text;
			if (itemId == "0" && _qfAllocationID != 0)
			{
				selectedId = _qfAllocationID.ToString();
				selectedText = string.Empty;
			}
			DropDownList ddl = null;
			ddl = WTSUtility.CreateGridDropdownList(_dtAllocation, "ALLOCATION", "ALLOCATION", "ALLOCATIONID", itemId, selectedId, selectedText, null);
			ddl.Enabled = (_qfAllocationID == 0);
			row.Cells[DCC.IndexOf("ALLOCATION")].Controls.Add(ddl);

			selectedId = row.Cells[DCC.IndexOf("WTS_SYSTEMID")].Text.Replace("&nbsp;", " ").Trim();
			selectedText = row.Cells[DCC.IndexOf("WTS_SYSTEM")].Text.Replace("&nbsp;", " ").Trim();
			if (itemId == "0" && _qfSystemID != 0)
			{
				selectedId = _qfSystemID.ToString();
				selectedText = string.Empty;
			}

			if (string.IsNullOrWhiteSpace(selectedId))
			{
				selectedId = "0";
			}

			ddl = WTSUtility.CreateGridDropdownList(_dtSystem, "WTS_SYSTEM", "WTS_SYSTEM", "WTS_SYSTEMID", itemId, selectedId, selectedText, null);
			ddl.Enabled = (_qfSystemID == 0);
			row.Cells[DCC.IndexOf("WTS_SYSTEM")].Controls.Add(ddl);
			row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;", " ").Trim())));

			row.Cells[DCC.IndexOf("ProposedPriority")].Controls.Add(WTSUtility.CreateGridTextBox("ProposedPriority", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ProposedPriority")].Text.Replace("&nbsp;", " ").Trim()), true));
			if (IsAdmin)
			{
				row.Cells[DCC.IndexOf("ApprovedPriority")].Controls.Add(WTSUtility.CreateGridTextBox("ApprovedPriority", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ApprovedPriority")].Text.Replace("&nbsp;", " ").Trim()), true));
			}

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
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WTS_SYSTEM")].Text.Trim() + " - " + row.Cells[DCC.IndexOf("Allocation")].Text.Trim()));
		}
	}

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_Allocation_System"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_Allocation_System"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("A")
				&& i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("Y")
				&& i != DCC.IndexOf("ProposedPriority")
				&& i != DCC.IndexOf("ApprovedPriority")
				&& i != DCC.IndexOf("WorkItem_Count")
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
		row.Cells[DCC.IndexOf("A")].Style["border-left"] = "1px solid grey";
		row.Cells[DCC.IndexOf("A")].Style["width"] = "40px";
		row.Cells[DCC.IndexOf("X")].Style["width"] = "30px";
		row.Cells[DCC.IndexOf("Y")].Style["width"] = "14px";
		row.Cells[DCC.IndexOf("WTS_SYSTEM")].Style["width"] = "150px";
		//row.Cells[DCC.IndexOf("ALLOCATION")].Style["width"] = "225px";
		row.Cells[DCC.IndexOf("ProposedPriority")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("ApprovedPriority")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "60px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
	}


	LinkButton createAddLink()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("buttonNew_click();return false;");

		LinkButton lb = new LinkButton();
		lb.ID = "lbAddTask";
		lb.ToolTip = "Add System";
		lb.Text = "New";
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}

	#endregion Grid


	[WebMethod(true)]
	public static string SaveChanges(string rows)
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
				int id = 0, allocationID = 0, proposedPriority = 0, approvedPriority = 0, systemID = 0, archive = 0;
				bool duplicate = false;
				string description = string.Empty;

				HttpServerUtility server = HttpContext.Current.Server;
				//save
				foreach (DataRow dr in dtjson.Rows)
				{
					id = systemID = allocationID = 0;
					proposedPriority = approvedPriority = archive = 0;
					description = string.Empty;
					archive = 0;
					duplicate = false;

					tempMsg = string.Empty;
					int.TryParse(dr["Allocation_SystemID"].ToString(), out id);
					int.TryParse(dr["ALLOCATION"].ToString(), out allocationID);
					int.TryParse(dr["WTS_SYSTEM"].ToString(), out systemID);
					description = server.UrlDecode(dr["DESCRIPTION"].ToString());
					int.TryParse(dr["ProposedPriority"].ToString(), out proposedPriority);
					int.TryParse(dr["ApprovedPriority"].ToString(), out approvedPriority);
					int.TryParse(dr["ARCHIVE"].ToString(), out archive);

					if (allocationID == 0)
					{
						tempMsg = "You must specify a Allocation Assignment.";
						saved = false;
					}
					else
					{
						if (id == 0)
						{
							exists = false;
							saved = MasterData.Allocation_System_Add(allocationID: allocationID
								, systemID: systemID
								, description: description
								, proposedPriority: proposedPriority
								, approvedPriority: approvedPriority
								, exists: out exists
								, newID: out id
								, errorMsg: out tempMsg);
							if (exists)
							{
								saved = false;
								tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate WorkType - Status record.");
							}
						}
						else
						{
							saved = MasterData.Allocation_System_Update(allocationSystemID: id
								, allocationID: allocationID
								, systemID: systemID
								, description: description
								, proposedPriority: proposedPriority
								, approvedPriority: approvedPriority
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
				deleted = MasterData.Allocation_System_Delete(allocationSystemID: itemId, exists: out exists, errorMsg: out errorMsg);
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