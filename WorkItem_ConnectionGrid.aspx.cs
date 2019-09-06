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


public partial class WorkItem_ConnectionGrid : System.Web.UI.Page
{
	DataSet _dsOptions = null;
	protected DataColumnCollection DCC;
	protected GridCols _columnData = new GridCols();

	protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;
	protected bool ColumnOrderChanged = false;

	protected int ShowArchived = 0;
	protected bool _refreshData = false;
	protected bool _export = false;

	protected string SourceType = "WorkItem";
	protected int WorkItemID = 0;
	protected int TestItemID = 0;

	protected bool CanView = false;
	protected bool CanEdit = false;
	protected bool IsAdmin = false;


    protected void Page_Load(object sender, EventArgs e)
    {
		this.IsAdmin = UserManagement.UserIsInRole("Admin");
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
		this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.WorkItem));

		readQueryString();

		init();

		loadGridData();
    }

	private void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}

		if (Request.QueryString["WorkItemID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["WorkItemID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["WorkItemID"].ToString()), out this.WorkItemID);
			if (this.WorkItemID > 0)
			{
				this.SourceType = "WorkItem";
			}
		}
		if (Request.QueryString["TestItemID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["TestItemID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["TestItemID"].ToString()), out this.TestItemID);
			if (this.TestItemID > 0)
			{
				this.SourceType = "TestItem";
			}
		}

		if (Request.QueryString["ShowArchived"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"].ToString()), out this.ShowArchived);
		}

		if (Request.QueryString["sortOrder"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
		{
			this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
		}
		if (Request.QueryString["columnOrder"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["columnOrder"].ToString()))
		{
			this.ColumnOrder = Server.UrlDecode(Request.QueryString["columnOrder"]);
		}
		if (Request.QueryString["columnOrderChanged"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["columnOrderChanged"].ToString()))
		{
			bool.TryParse(Server.UrlDecode(Request.QueryString["columnOrderChanged"]), out this.ColumnOrderChanged);
		}
		if (Request.QueryString["Export"] != null &&
			!string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
		{
			bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);
		}
	}

	private void init()
	{
		_dsOptions = WorkloadItem.GetAvailableOptions();

		grdWorkItem.GridHeaderRowDataBound += grdWorkItem_GridHeaderRowDataBound;
		grdWorkItem.GridRowDataBound += grdWorkItem_GridRowDataBound;
		grdWorkItem.GridPageIndexChanging += grdWorkItem_GridPageIndexChanging;
	}


	#region Data

	private void loadGridData()
	{
		DataTable dt = null;

		if (_refreshData || Session["dtWorkItem_TestItem"] == null)
		{
			dt = WorkloadItem.WorkItem_TestItemList_Get(WorkItemID == 0 ? TestItemID : WorkItemID, this.SourceType);
			HttpContext.Current.Session["dtWorkItem_TestItem"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtWorkItem_TestItem"];
		}

		if (dt != null)
		{
			if (dt.Rows.Count > 0)
			{
				spanRowCount.InnerText = (dt.Rows.Count - 1).ToString();
			}

			initializeColumnData(ref dt);
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
		}

		grdWorkItem.DataSource = dt;
		grdWorkItem.DataBind();
	}

	private void initializeColumnData(ref DataTable dt)
	{
		try
		{
			string displayName = string.Empty, groupName = string.Empty;
			bool blnVisible = false, isViewable = false, blnSortable = false, blnOrderable = false;
			bool showWorkItem = (this.WorkItemID == 0), showTestItem = (this.TestItemID == 0);
			//showWorkItem = showTestItem = true;

			foreach (DataColumn gridColumn in dt.Columns)
			{
				displayName = gridColumn.ColumnName;
				blnVisible = false;
				blnSortable = false;
				blnOrderable = true;
				isViewable = false;
				groupName = string.Empty;

				switch (gridColumn.ColumnName)
				{
					case "X":
						displayName = "&nbsp;";
						blnVisible = true;
						isViewable = true;
						blnSortable = false;
						blnOrderable = false;
						groupName = "";
						break;
					case "WorkItem_TestItemID":
						displayName = "WorkItem_TestItemID";
						blnSortable = blnVisible = false;
						isViewable = false;
						blnOrderable = false;
						groupName = "";
						break;
					case "WorkItem_Number":
						displayName = "Primary Task";
						blnSortable = blnVisible = true;
						isViewable = true;
						blnOrderable = false;
						groupName = "Work Item";
						break;
					case "WorkItem_WorkType":
						displayName = "Work Type";
						blnSortable = blnVisible = showWorkItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Work Item";
						break;
					case "WorkItem_System":
						displayName = "System";
						blnSortable = blnVisible = showWorkItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Work Item";
						break;
					case "WorkItem_Title":
						displayName = "Work Item";
						blnVisible = showWorkItem;
						blnSortable = (this.SourceType != "WorkItem");
						isViewable = true;
						blnOrderable = true;
						groupName = "Work Item";
						break;
					case "WorkItem_STATUS":
						displayName = "Work Item<br/>Status";
						blnSortable = blnVisible = showWorkItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Work Item";
						break;
					case "WorkItem_Progress":
						displayName = "Work Item<br/>Progress";
						blnSortable = blnVisible = showWorkItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Work Item";
						break;
					case "WorkItem_AssignedTo":
						displayName = "Work Item<br/>Assigned To";
						blnSortable = blnVisible = showWorkItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Work Item";
						break;
					case "WorkItem_Primary_Resource":
						displayName = "Work Item<br/>Primary Resource";
						blnSortable = blnVisible = showWorkItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Work Item";
						break;
					case "WorkItem_Tester":
						displayName = "Work Item Tester";
						blnSortable = blnVisible = showWorkItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Work Item";
						break;
					case "TestItem_Number":
						displayName = "Test Task";
						blnSortable = blnVisible = true;
						isViewable = true;
						blnOrderable = false;
						groupName = "Test Item";
						break;
					case "TestItem_WorkType":
						displayName = "Test Task<br/>Type";
						blnVisible = true;
						blnSortable = showTestItem;
						isViewable = false;
						blnOrderable = false;
						groupName = "Test Item";
						break;
					case "TestItem_Title":
						displayName = "Test Task Title";
						blnSortable = blnVisible = (this.SourceType == "WorkItem");
						isViewable = true;
						blnOrderable = true;
						groupName = "Test Item";
						break;
					case "TestItem_STATUS":
						displayName = "Test Task Status";
						blnSortable = blnVisible = showTestItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Test Item";
						break;
					case "TestItem_Progress":
						displayName = "Test Task<br/>Progress";
						blnSortable = blnVisible = showTestItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Test Item";
						break;
					case "TestItem_AssignedTo":
						displayName = "Test Task<br/>Assigned To";
						blnSortable = blnVisible = showTestItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Test Item";
						break;
					case "TestItem_Primary_Resource":
						displayName = "Test Task<br/>Primary Resource";
						blnSortable = blnVisible = showTestItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Test Item";
						break;
					case "TestItem_Tester":
						displayName = "Test Task Tester";
						blnSortable = blnVisible = showTestItem;
						isViewable = true;
						blnOrderable = true;
						groupName = "Test Item";
						break;
					case "ARCHIVE":
						displayName = "Archive";
						blnSortable = blnVisible = true;
						isViewable = true;
						blnOrderable = true;
						groupName = "";
						break;
					case "CreatedBy":
						displayName = "Created By";
						blnSortable = blnVisible = false;
						isViewable = true;
						blnOrderable = true;
						groupName = "";
						break;
					case "CreatedDate":
						displayName = "Created Date";
						blnSortable = blnVisible = false;
						isViewable = true;
						blnOrderable = true;
						groupName = "";
						break;
					case "Y":
						displayName = "Y";
						blnVisible = true;
						isViewable = true;
						blnSortable = false;
						blnOrderable = false;
						groupName = "";
						break;
				}

				_columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				_columnData.Columns.Item(_columnData.Columns.Count - 1).CanReorder = blnOrderable;
				_columnData.Columns.Item(_columnData.Columns.Count - 1).Viewable = isViewable;
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

	#endregion Data


	#region Grid

	void grdWorkItem_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		_columnData.SetupGridHeader(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		row.Cells[DCC.IndexOf("Y")].Text = "&nbsp;";
	}
	void grdWorkItem_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		_columnData.SetupGridBody(e.Row);

		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		string itemId = row.Cells[DCC.IndexOf("WorkItem_TestItemID")].Text.Trim();
		int workItemID = 0, testItemID = 0;
		int.TryParse(row.Cells[DCC["WorkItem_Number"].Ordinal].Text.Trim(), out workItemID);
		int.TryParse(row.Cells[DCC["TestItem_Number"].Ordinal].Text.Trim(), out testItemID);
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);
		row.Attributes.Add("workItemID", workItemID.ToString());
		row.Attributes.Add("testItemID", testItemID.ToString());
		row.Attributes.Add("sourceType", this.SourceType);
		row.Cells[DCC.IndexOf("WorkItem_TestItemID")].Attributes.Add("sourceType", this.SourceType);

		string title = row.Cells[DCC["WorkItem_Title"].Ordinal].Text.Trim();
		string tooltip = "";
		if (workItemID == 0)
		{
			//row.Cells[DCC["WorkItem_Number"].Ordinal].Text = "&nbsp;";
			TextBox txt = WTSUtility.CreateGridTextBox("WorkItem_Number", itemId, "", true);
			txt.Style["width"] = "75%";
			row.Cells[DCC["WorkItem_Number"].Ordinal].Controls.Add(txt);
			//row.Cells[DCC["WorkItem_Number"].Ordinal].Controls.Add(createRefreshButton("WorkItem", itemId, ""));
		}
		else
		{
			tooltip = string.Format("Work Item [{0}] - {1}", workItemID.ToString(), Server.HtmlDecode(title));
			row.Cells[DCC["WorkItem_Number"].Ordinal].Controls.Add(createEditLink_WorkItem(workItemID.ToString(), Server.HtmlDecode(title)));
			row.Cells[DCC["WorkItem_Number"].Ordinal].ToolTip = tooltip;
			//row.Cells[DCC["WorkItem_Number"].Ordinal].Controls.Add(createRefreshButton("WorkItem", itemId, ""));
		}

		title = row.Cells[DCC["TestItem_Title"].Ordinal].Text.Trim();
		if (testItemID == 0)
		{
			//row.Cells[DCC["TestItem_Number"].Ordinal].Text = "&nbsp;";
			row.Cells[DCC["TestItem_Number"].Ordinal].Controls.Add(WTSUtility.CreateGridTextBox("TestItem_Number", itemId, "", true));
			//row.Cells[DCC["TestItem_Number"].Ordinal].Controls.Add(createRefreshButton("TestItem", itemId, ""));
		}
		else
		{
			tooltip = string.Format("Test Item [{0}] - {1}", testItemID.ToString(), Server.HtmlDecode(title));
			row.Cells[DCC["TestItem_Number"].Ordinal].Controls.Add(createEditLink_WorkItem(testItemID.ToString(), Server.HtmlDecode(title)));
			row.Cells[DCC["TestItem_Number"].Ordinal].ToolTip = tooltip;
			//row.Cells[DCC["TestItem_Number"].Ordinal].Controls.Add(createRefreshButton("TestItem", itemId, ""));
		}

		if (_dsOptions != null && _dsOptions.Tables.Count > 0)
		{
			DropDownList ddl = null;
			if (_dsOptions.Tables.Contains("User"))
			{
				string resourceId = string.Empty, resource = string.Empty;

				resourceId = row.Cells[DCC["WorkItem_AssignedToID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				resourceId = string.IsNullOrWhiteSpace(resourceId) ? "0" : resourceId;
				resource = row.Cells[DCC["WorkItem_AssignedTo"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				resource = string.IsNullOrWhiteSpace(resource) ? "-Select-" : resource;

				ddl = WTSUtility.CreateGridDropdownList(_dsOptions.Tables["User"], "WorkItem_AssignedTo", "USERNAME", "WTS_RESOURCEID", itemId, resourceId, resource);
				row.Cells[DCC["WorkItem_AssignedTo"].Ordinal].Controls.Add(ddl);

				resourceId = row.Cells[DCC["WorkItem_Primary_ResourceID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				resourceId = string.IsNullOrWhiteSpace(resourceId) ? "0" : resourceId;
				resource = row.Cells[DCC["WorkItem_Primary_Resource"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				resource = string.IsNullOrWhiteSpace(resource) ? "-Select-" : resource;

				ddl = WTSUtility.CreateGridDropdownList(_dsOptions.Tables["User"], "WorkItem_Primary_Resource", "USERNAME", "WTS_RESOURCEID", itemId, resourceId, resource);
				row.Cells[DCC["WorkItem_Primary_Resource"].Ordinal].Controls.Add(ddl);

				resourceId = row.Cells[DCC["WorkItem_TesterID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				resourceId = string.IsNullOrWhiteSpace(resourceId) ? "0" : resourceId;
				resource = row.Cells[DCC["WorkItem_Tester"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				resource = string.IsNullOrWhiteSpace(resource) ? "-Select-" : resource;

				ddl = WTSUtility.CreateGridDropdownList(_dsOptions.Tables["User"], "WorkItem_Tester", "USERNAME", "WTS_RESOURCEID", itemId, resourceId, resource);
				row.Cells[DCC["WorkItem_Tester"].Ordinal].Controls.Add(ddl);

				resourceId = row.Cells[DCC["TestItem_AssignedToID"].Ordinal].Text.Replace("&nbsp;"," ").Trim();
				resourceId = string.IsNullOrWhiteSpace(resourceId) ? "0" : resourceId;
				resource = row.Cells[DCC["TestItem_AssignedTo"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				resource = string.IsNullOrWhiteSpace(resource) ? "-Select-" : resource;

				ddl = WTSUtility.CreateGridDropdownList(_dsOptions.Tables["User"], "TestItem_AssignedTo", "USERNAME", "WTS_RESOURCEID", itemId, resourceId, resource);
				row.Cells[DCC["TestItem_AssignedTo"].Ordinal].Controls.Add(ddl);

				resourceId = row.Cells[DCC["TestItem_Primary_ResourceID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				resourceId = string.IsNullOrWhiteSpace(resourceId) ? "0" : resourceId;
				resource = row.Cells[DCC["TestItem_Primary_Resource"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				resource = string.IsNullOrWhiteSpace(resource) ? "-Select-" : resource;

				ddl = WTSUtility.CreateGridDropdownList(_dsOptions.Tables["User"], "TestItem_Primary_Resource", "USERNAME", "WTS_RESOURCEID", itemId, resourceId, resource);
				row.Cells[DCC["TestItem_Primary_Resource"].Ordinal].Controls.Add(ddl);

                resourceId = row.Cells[DCC["TestItem_TesterID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
                resourceId = string.IsNullOrWhiteSpace(resourceId) ? "0" : resourceId;
                resource = row.Cells[DCC["TestItem_Tester"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
                resource = string.IsNullOrWhiteSpace(resource) ? "-Select-" : resource;

                ddl = WTSUtility.CreateGridDropdownList(_dsOptions.Tables["User"], "TestItem_Tester", "USERNAME", "WTS_RESOURCEID", itemId, resourceId, resource);
                row.Cells[DCC["TestItem_Tester"].Ordinal].Controls.Add(ddl);
            }

            if (_dsOptions.Tables.Contains("Status"))
			{
				if (_dsOptions.Tables["Status"] != null && _dsOptions.Tables["Status"].Rows.Count > 0)
				{
					DataTable dtTemp = _dsOptions.Tables["Status"];
					//Work Item
					string typeID = row.Cells[DCC["WorkItem_WorkTypeID"].Ordinal].Text.Replace("&nbsp;", "");
					if (!string.IsNullOrWhiteSpace(typeID) && typeID != "0")
					{
						dtTemp.DefaultView.RowFilter = string.Format(" WorkTypeID = {0} ", row.Cells[DCC["WorkItem_WorkTypeID"].Ordinal].Text.Replace("&nbsp;", "").Trim());
						dtTemp = dtTemp.DefaultView.ToTable();
					}
					else
					{
						dtTemp = dtTemp.DefaultView.ToTable(true, "STATUSID", "STATUS");
					}
					string id = string.Empty, value = string.Empty;

					id = row.Cells[DCC["WorkItem_STATUSID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
					id = string.IsNullOrWhiteSpace(id) ? "0" : id;
					value = row.Cells[DCC["WorkItem_STATUS"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
					value = string.IsNullOrWhiteSpace(value) ? "-Select-" : id;

					ddl = WTSUtility.CreateGridDropdownList(dtTemp, "WorkItem_STATUS", "STATUS", "STATUSID", itemId, id, value);
					row.Cells[DCC["WorkItem_STATUS"].Ordinal].Controls.Add(ddl);

					//Test Item
					dtTemp = _dsOptions.Tables["Status"];
					typeID = row.Cells[DCC["TestItem_WorkTypeID"].Ordinal].Text.Replace("&nbsp;", "");
					if (!string.IsNullOrWhiteSpace(typeID) && typeID != "0")
					{
						dtTemp.DefaultView.RowFilter = string.Format(" WorkTypeID = {0} ", row.Cells[DCC["TestItem_WorkTypeID"].Ordinal].Text.Replace("&nbsp;", "").Trim());
						dtTemp = dtTemp.DefaultView.ToTable();
					}
					else
					{
						dtTemp = dtTemp.DefaultView.ToTable(true, "STATUSID", "STATUS");
					}

					id = row.Cells[DCC["TestItem_STATUSID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
					id = string.IsNullOrWhiteSpace(id) ? "0" : id;
					value = row.Cells[DCC["TestItem_STATUS"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
					value = string.IsNullOrWhiteSpace(value) ? "-Select-" : id;

					ddl = WTSUtility.CreateGridDropdownList(dtTemp, "TestItem_STATUS", "STATUS", "STATUSID", itemId, id, value);
					row.Cells[DCC["TestItem_STATUS"].Ordinal].Controls.Add(ddl);
				}
			}

			if (_dsOptions.Tables.Contains("PercentComplete"))
			{
				row.Cells[DCC["WorkItem_Progress"].Ordinal].Controls.Add(
					WTSUtility.CreateGridDropdownList(_dsOptions.Tables["PercentComplete"], "WorkItem_Progress", "Percent", "Percent", itemId, row.Cells[DCC["WorkItem_Progress"].Ordinal].Text.Trim(), row.Cells[DCC.IndexOf("WorkItem_Progress")].Text.Trim()));
				row.Cells[DCC["TestItem_Progress"].Ordinal].Controls.Add(
					WTSUtility.CreateGridDropdownList(_dsOptions.Tables["PercentComplete"], "TestItem_Progress", "Percent", "Percent", itemId, row.Cells[DCC["TestItem_Progress"].Ordinal].Text.Trim(), row.Cells[DCC.IndexOf("TestItem_Progress")].Text.Trim()));
			}

			if (!CanEdit)
			{
				Image imgBlank = new Image();
				imgBlank.Height = 10;
				imgBlank.Width = 10;
				imgBlank.ImageUrl = "Images/Icons/blank.png";
				imgBlank.AlternateText = "";
				row.Cells[DCC["Y"].Ordinal].Controls.Add(imgBlank);
			}
			else
			{
				row.Cells[DCC["Y"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId));
			}
		}
	}
	void grdWorkItem_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdWorkItem.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtWorkItem_TestItem"] == null)
		{
			loadGridData();
		}
		else
		{
			grdWorkItem.DataSource = (DataTable)HttpContext.Current.Session["dtWorkItem_TestItem"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (DCC[i].ColumnName.StartsWith("WorkItem", StringComparison.CurrentCultureIgnoreCase))
			{
				row.Cells[i].Attributes.Add("sourceType","WorkItem");
			}
			else if (DCC[i].ColumnName.StartsWith("TestItem", StringComparison.CurrentCultureIgnoreCase))
			{
				row.Cells[i].Attributes.Add("sourceType", "TestItem");
			}

			if ((DCC.Contains("X") && i == DCC.IndexOf("X"))
				|| (DCC.Contains("WorkItem_TestItemID") && i == DCC.IndexOf("WorkItem_TestItemID"))
				|| (DCC.Contains("WorkItem_Number") && i == DCC.IndexOf("WorkItem_Number"))
				|| (DCC.Contains("WorkItem_WorkType") && i == DCC.IndexOf("WorkItem_WorkType"))
				|| (DCC.Contains("WorkItem_STATUS") && i == DCC.IndexOf("WorkItem_STATUS"))
				|| (DCC.Contains("WorkItem_Progress") && i == DCC.IndexOf("WorkItem_Progress"))
				|| (DCC.Contains("TestItem_Number") && i == DCC.IndexOf("TestItem_Number"))
				|| (DCC.Contains("TestItem_WorkType") && i == DCC.IndexOf("TestItem_WorkType"))
				|| (DCC.Contains("TestItem_STATUS") && i == DCC.IndexOf("TestItem_STATUS"))
				|| (DCC.Contains("TestItem_Progress") && i == DCC.IndexOf("TestItem_Progress"))
				|| (DCC.Contains("ARCHIVE") && i == DCC.IndexOf("ARCHIVE"))
				|| (DCC.Contains("Y") && i == DCC.IndexOf("Y")))
			{
				row.Cells[i].Style["text-align"] = "center";
				row.Cells[i].Style["padding-left"] = "0px";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "left";
				row.Cells[i].Style["padding-left"] = "5px";
			}
		}

		//more column formatting
		row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
		row.Cells[DCC.IndexOf("Y")].Style["width"] = "12px";
		if (DCC.Contains("WorkItem_WorkType"))
		{
			row.Cells[DCC.IndexOf("WorkItem_WorkType")].Style["width"] = "70px";
		}
		if (DCC.Contains("WorkItem_Number"))
		{
			row.Cells[DCC.IndexOf("WorkItem_Number")].Style["width"] = "75px";
		}
		if (DCC.Contains("WorkItem_System"))
		{
			row.Cells[DCC.IndexOf("WorkItem_System")].Style["width"] = "65px";
		}
		if (DCC.Contains("WorkItem_STATUS"))
		{
			row.Cells[DCC.IndexOf("WorkItem_STATUS")].Style["width"] = "100px";
		}
		if (DCC.Contains("WorkItem_Progress"))
		{
			row.Cells[DCC.IndexOf("WorkItem_Progress")].Style["width"] = "65px";
		}
		if (DCC.Contains("WorkItem_AssignedTo"))
		{
			row.Cells[DCC.IndexOf("WorkItem_AssignedTo")].Style["width"] = "120px";
		}
		if (DCC.Contains("WorkItem_Primary_Resource"))
		{
			row.Cells[DCC.IndexOf("WorkItem_Primary_Resource")].Style["width"] = "120px";
		}
		if (DCC.Contains("WorkItem_Tester"))
		{
			row.Cells[DCC.IndexOf("WorkItem_Tester")].Style["width"] = "120px";
		}

		if (DCC.Contains("TestItem_WorkType"))
		{
			row.Cells[DCC.IndexOf("TestItem_WorkType")].Style["width"] = "70px";
		}
		if (DCC.Contains("TestItem_Number"))
		{
			row.Cells[DCC.IndexOf("TestItem_Number")].Style["width"] = "75px";
		}
		if (DCC.Contains("TestItem_System"))
		{
			row.Cells[DCC.IndexOf("TestItem_System")].Style["width"] = "65px";
		}
		if (DCC.Contains("TestItem_STATUS"))
		{
			row.Cells[DCC.IndexOf("TestItem_STATUS")].Style["width"] = "100px";
		}
		if (DCC.Contains("TestItem_Progress"))
		{
			row.Cells[DCC.IndexOf("TestItem_Progress")].Style["width"] = "65px";
		}
		if (DCC.Contains("TestItem_AssignedTo"))
		{
			row.Cells[DCC.IndexOf("TestItem_AssignedTo")].Style["width"] = "120px";
		}
		if (DCC.Contains("TestItem_Primary_Resource"))
		{
			row.Cells[DCC.IndexOf("TestItem_Primary_Resource")].Style["width"] = "120px";
		}
		if (DCC.Contains("TestItem_Tester"))
		{
			row.Cells[DCC.IndexOf("TestItem_Tester")].Style["width"] = "120px";
		}

		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
	}

	LinkButton createEditLink_WorkItem(string workItemId = "", string title = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEditWorkItem_click('{0}');return false;", workItemId);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEditWorkItem_{0}", workItemId);
		lb.Attributes["name"] = string.Format("lbEditWorkItem_{0}", workItemId);
		lb.ToolTip = string.Format("Edit Primary Task [{0}] - {1}", workItemId, title);
		lb.Text = workItemId;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}

	Image createRefreshButton(string sourceType = "", string itemId = "", string tooltip = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("refreshWorkItemDetails(this,'{0}','{1}');", sourceType, itemId);

		Image img = new Image();
		img.ImageUrl = "Images/Icons/arrow_refresh_blue.png";
		img.ID = string.Format("imgRefresh_{0}_{1}", sourceType, itemId);
		img.Style["cursor"] = "pointer";
		img.Style["padding-left"] = "3px";
		img.Height = 10;
		img.Width = 10;
		img.ToolTip = string.Format("Refresh details for {0}", (sourceType.ToUpper() == "WORKITEM") ? "Work Item" : "Test Item");
		img.AlternateText = string.Format("Refresh details for {0}", (sourceType.ToUpper() == "WORKITEM") ? "Work Item" : "Test Item");
		img.Attributes.Add("Onclick", sb.ToString());

		return img;
	}

	#endregion Grid


	#region AJAX

	[WebMethod(true)]
	public static string SaveChanges(string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
			, { "failed", "0" }
			, { "savedIds", "" }
			, { "failedIds", "" }
			, { "error", "" } };
		int savedQty = 0, failedQty = 0;
		string ids = string.Empty, failedIds = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;
        int savedInt = 0;
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
				int workItemID = 0, workItemStatusID = 0, workItemProgress = 0, workItemAssignedID = 0, workItemPrimaryID = 0, workItemTesterID = 0;
				int testItemID = 0, testItemStatusID = 0, testItemProgress = 0, testItemAssignedID = 0, testItemPrimaryID = 0, testItemTesterID = 0;
				bool saved = false, duplicate = false;

				WorkloadItem item = null, testItem = null;

				//save
				foreach (DataRow dr in dtjson.Rows)
				{
					saved = duplicate = false;
					id = workItemID = testItemID = 0;
					workItemStatusID = workItemProgress = workItemAssignedID = workItemPrimaryID = workItemTesterID = 0;
					testItemStatusID = testItemProgress = testItemAssignedID = testItemPrimaryID = testItemTesterID = 0;
					tempMsg = string.Empty;

					int.TryParse(dr["WorkItem_TestItemID"].ToString(), out id);
					int.TryParse(dr["WorkItem_Number"].ToString(), out workItemID);
					int.TryParse(dr["WorkItem_STATUS"].ToString(), out workItemStatusID);
					int.TryParse(dr["WorkItem_Progress"].ToString(), out workItemProgress);
                    // SCB - Added "ID" to the end of these two - didn't match up with the datatable.
                    int.TryParse(dr["WorkItem_AssignedToID"].ToString(), out workItemAssignedID);
					int.TryParse(dr["WorkItem_Primary_ResourceID"].ToString(), out workItemPrimaryID);
					int.TryParse(dr["WorkItem_TesterID"].ToString(), out workItemTesterID);

					int.TryParse(dr["TestItem_Number"].ToString(), out testItemID);

					int.TryParse(dr["TestItem_STATUS"].ToString(), out testItemStatusID);
					int.TryParse(dr["TestItem_Progress"].ToString(), out testItemProgress);
					int.TryParse(dr["TestItem_AssignedTo"].ToString(), out testItemAssignedID);
					int.TryParse(dr["TestItem_Primary_Resource"].ToString(), out testItemPrimaryID);
					int.TryParse(dr["TestItem_Tester"].ToString(), out testItemTesterID);

					if (workItemID == 0 || testItemID == 0)
					{
						saved = false;
						failedQty += 1;
						tempMsg = "You must specify both a Work Item and a Test Item.";
					}
					else
					{
						if (id == 0)
						{
							if (!Workload.ItemExists(workItemID, -1, "Primary Task"))
							{
								saved = false;
								tempMsg = string.Format("Unable to find Primary Task with ID = [{0}]", workItemID.ToString());
							}
							else if (!Workload.ItemExists(testItemID, -1, "Primary Task"))
							{
								saved = false;
								tempMsg = string.Format("Unable to find Test Task with ID = [{0}]", testItemID.ToString());
							}
							else
							{
								saved = WorkloadItem.WorkItem_TestItem_Add(workItemID: workItemID, testItemID: testItemID, archive: false, duplicate: out duplicate, newID: out id, errorMsg: out tempMsg);
							}
						}
						else
						{
							item = WorkloadItem.WorkItem_GetObject(workItemID);
							testItem = WorkloadItem.WorkItem_GetObject(testItemID);

							if(itemChanged(ref item, workItemStatusID, workItemProgress, workItemAssignedID, workItemPrimaryID, workItemTesterID))
							{
                                savedInt = WorkloadItem.WorkItem_Update(item, out tempMsg);
							}
							if (itemChanged(ref testItem, testItemStatusID, testItemProgress, testItemAssignedID, testItemPrimaryID, testItemTesterID))
							{
                                savedInt = WorkloadItem.WorkItem_Update(testItem, out tempMsg);
							}

                            if (savedInt == 0)
                            {
                                saved = false;
                            }
                            else
                            {
                                saved = true;
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
							errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			errorMsg = errorMsg.Length > 0 ? ", " + Environment.NewLine + ex.Message : ex.Message;
		}

		result["savedIds"] = ids;
		result["failedIds"] = failedIds;
		result["saved"] = savedQty.ToString();
		result["failed"] = failedQty.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	protected static bool itemChanged(ref WorkloadItem item, int statusID, int progress, int assignedID, int primaryID, int testerID)
	{
		bool changed = true;

		if (item.StatusID != statusID)
		{
			item.StatusID = statusID;
			changed = true;
		}
		if (item.CompletionPercent != progress)
		{
			item.CompletionPercent = progress;
			changed = true;
		}
        if (item.AssignedResourceID != assignedID)
		{
			item.AssignedResourceID = assignedID;
			changed = true;
		}
        if (item.PrimaryResourceID != primaryID)
		{
			item.PrimaryResourceID = primaryID;
			changed = true;
		}
        if (item.TesterID != testerID)
		{
			item.TesterID = testerID;
			changed = true;
		}

		return changed;
	}

	[WebMethod(true)]
	public static string DeleteItem(int itemId)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "id", itemId.ToString() }
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
				deleted = WorkloadItem.WorkItem_TestItem_Delete(itemId, out exists, out errorMsg);
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

	#endregion AJAX
}