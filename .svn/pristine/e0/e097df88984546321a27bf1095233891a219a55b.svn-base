using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Linq;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;


public partial class WorkItemGrid_QM : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _myData = true;
	protected bool _includeArchive = false;
	protected bool ShowClosed = false;
	protected bool _refreshData = false;
	protected int _pageIndex = 0;
	protected bool _export = false;

	protected string SortableColumns = string.Empty;
	protected string SortOrder = string.Empty;
	protected string DefaultColumnOrder = string.Empty;
	protected string SelectedColumnOrder = string.Empty;
	protected string ColumnOrder = string.Empty;
	protected bool ColumnOrderChanged = false;

	protected bool CanEditWorkItem = false;


	protected void Page_Load(object sender, EventArgs e)
	{
		this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);

		readQueryString();

		init();

		if (DCC != null)
		{
			this.DCC.Clear();
		}

		if (!IsPostBack) loadGridData();
	}

	private void readQueryString()
	{
		if (Page.IsPostBack)
		{
			_refreshData = false;
		}
		else
		{
			if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
				|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
			{
				_refreshData = true;
			}
		}

		if (Request.QueryString["PageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["PageIndex"]))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["PageIndex"]).Trim(), out _pageIndex);
		}

		if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
			|| Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
		{
			_myData = true;
		}
		else
		{
			_myData = false;
		}

		if (Request.QueryString["ShowClosed"] == null || string.IsNullOrWhiteSpace(Request.QueryString["ShowClosed"]))
		{
			ShowClosed = false;
		}
		else
		{
			if (Request.QueryString["ShowClosed"].Trim() == "1" || Request.QueryString["ShowClosed"].Trim().ToUpper() == "TRUE")
			{
				ShowClosed = true;
			}
		}

		if (Request.QueryString["IncludeArchive"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["IncludeArchive"])
			&& (Request.QueryString["IncludeArchive"].Trim() == "1" || Request.QueryString["IncludeArchive"].Trim().ToUpper() == "TRUE"))
		{
			_includeArchive = true;
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
	}

	private void init()
	{
		initAttributeOptions();

		grdWorkload.GridHeaderRowDataBound += grdWorkload_GridHeaderRowDataBound;
		grdWorkload.GridRowDataBound += grdWorkload_GridRowDataBound;
		grdWorkload.GridPageIndexChanging += grdWorkload_GridPageIndexChanging;
	}
	private void initAttributeOptions()
	{
		try
		{
			DataSet dsOptions = WorkloadItem.GetAvailableOptions();

			if (dsOptions != null && dsOptions.Tables.Count > 0)
			{
                if (dsOptions.Tables.Contains("WorkRequest"))
                {
                    Page.ClientScript.RegisterArrayDeclaration("_WorkRequestList", JsonConvert.SerializeObject(dsOptions.Tables["WorkRequest"], Newtonsoft.Json.Formatting.None));
                }
                if (dsOptions.Tables.Contains("System"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_SystemList", JsonConvert.SerializeObject(dsOptions.Tables["System"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("Allocation"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_AllocationList", JsonConvert.SerializeObject(dsOptions.Tables["Allocation"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("ProductVersion"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_ProductVersionList", JsonConvert.SerializeObject(dsOptions.Tables["ProductVersion"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("Priority"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_PriorityList", JsonConvert.SerializeObject(dsOptions.Tables["Priority"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("WorkItemType"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_WorkItemTypeList", JsonConvert.SerializeObject(dsOptions.Tables["WorkItemType"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("User"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_UserList", JsonConvert.SerializeObject(dsOptions.Tables["User"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("WorkType"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_WorkTypeList", JsonConvert.SerializeObject(dsOptions.Tables["WorkType"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("Status"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_StatusList", JsonConvert.SerializeObject(dsOptions.Tables["Status"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("PercentComplete"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_PercentList", JsonConvert.SerializeObject(dsOptions.Tables["PercentComplete"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("PriorityRank"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_PriorityRankList", JsonConvert.SerializeObject(dsOptions.Tables["PriorityRank"], Newtonsoft.Json.Formatting.None));
				}

				if (dsOptions.Tables.Contains("MenuType"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_MenuTypeList", JsonConvert.SerializeObject(dsOptions.Tables["MenuType"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("Menu"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_MenuList", JsonConvert.SerializeObject(dsOptions.Tables["Menu"], Newtonsoft.Json.Formatting.None));
				}
				if (dsOptions.Tables.Contains("ProductionStatus"))
				{
					Page.ClientScript.RegisterArrayDeclaration("_ProductionStatusList", JsonConvert.SerializeObject(dsOptions.Tables["ProductionStatus"], Newtonsoft.Json.Formatting.None));
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}


	private void loadGridData(bool bind = true)
	{
		DataTable dtWork = null;

		if (_refreshData || Session["dtWorkItem_QM"] == null)
		{
			dtWork = WorkloadItem.WorkItemList_Get(workRequestID: 0, showArchived: _includeArchive ? 1 : 0, columnListOnly: 0, myData: _myData);
			HttpContext.Current.Session["dtWorkItem_QM"] = dtWork;
		}
		else
		{
			dtWork = (DataTable)HttpContext.Current.Session["dtWorkItem_QM"];
		}

		if (dtWork != null)
		{
			if (!ShowClosed)
			{
				dtWork.DefaultView.RowFilter = " STATUS NOT IN ('Closed', 'Approved/Closed') ";
				dtWork = dtWork.DefaultView.ToTable();
			}

			dtWork.Columns["TITLE"].SetOrdinal(dtWork.Columns["ItemID"].Ordinal + 1);
			dtWork.Columns["WORKREQUEST"].SetOrdinal(dtWork.Columns["TITLE"].Ordinal + 1);
			spanRowCount.InnerText = dtWork.Rows.Count.ToString();

			InitializeColumnData_WorkItem(ref dtWork);
			dtWork.AcceptChanges();

			using (DataTable dtTemp = dtWork.Clone())
			{
				this.DCC = dtTemp.Columns;
				string json = JsonConvert.SerializeObject(DCC, Formatting.None);
				Page.ClientScript.RegisterArrayDeclaration("_dcc", json);
			}
		}

		if (_pageIndex > 0)
		{
			grdWorkload.PageIndex = _pageIndex;
		}
		grdWorkload.DataSource = dtWork;
		if (bind) grdWorkload.DataBind();
	}
	protected void InitializeColumnData_WorkItem(ref DataTable dt)
	{
		try
		{
			string displayName = string.Empty, groupName = string.Empty;
			bool blnVisible = false, isViewable = false, blnSortable = false, blnOrderable = false;

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
					//case "WORKREQUESTID":
					//	displayName = "Request Number";
					//	blnVisible = false;
					//	isViewable = true;
					//	blnSortable = false;
					//	blnOrderable = false;
					//	break;
					//case "WORKREQUEST":
					//	displayName = "Request";
					//	blnVisible = true;
					//	isViewable = true;
					//	blnSortable = true;
					//	break;
					case "ItemID":
						displayName = "Task Number";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "WORKITEMTYPE":
						displayName = "Work Activity";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					case "Websystem":
						displayName = "System(Task)";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					case "TITLE":
						displayName = "Description";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					//case "ALLOCATION":
					//	displayName = "Allocation Assignment";
					//	blnVisible = true;
					//	isViewable = true;
					//	blnSortable = true;
					//	break;
					case "ProductionStatus":
						displayName = "Production Status";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					case "Version":
						displayName = "Version";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					case "PRIORITY":
						displayName = "Priority";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					case "Primary_Analyst":
						displayName = "Primary Analyst";
						blnVisible = false;
						isViewable = true;
						blnSortable = false;
						break;
					case "Primary_Developer":
						displayName = "Primary Developer";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					case "Assigned":
						displayName = "Assigned";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					case "SubmittedBy":
						displayName = "Submitted";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "WorkType":
						displayName = "Resource Group";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					case "STATUS":
						displayName = "Status";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					case "Progress":
						displayName = "Progress";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
					case "ARCHIVE":
						displayName = "Archive";
						blnVisible = true;
						isViewable = true;
						blnSortable = true;
						break;
				}

				columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
				columnData.Columns.Item(columnData.Columns.Count - 1).Viewable = isViewable;
			}

			//Initialize the columnData
			columnData.Initialize(ref dt, ";", "~", "|");

			//Get sortable columns and default column order
			SortableColumns = columnData.SortableColumnsToString();
			DefaultColumnOrder = columnData.DefaultColumnOrderToString();
			//Sort and Reorder Columns
			columnData.ReorderDataTable(ref dt, ColumnOrder);

            SortOrder = Workload.GetSortValuesFromDB(1, "WORKITEMGRID_QM.ASPX");

            columnData.SortDataTable(ref dt, SortOrder);
			SelectedColumnOrder = columnData.CurrentColumnOrderToString();
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}


	void grdWorkload_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridHeader(e.Row);
		GridViewRow row = e.Row;

		formatColumnDisplay(ref row);

		row.Cells[DCC.IndexOf("ItemID")].Text = "Primary Task";
		row.Cells[DCC.IndexOf("Task_Count")].Text = "# Sub-Tasks";
	}

	void grdWorkload_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);

		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		string itemId = row.Cells[DCC.IndexOf("ItemID")].Text.Trim();
		string workTypeId = row.Cells[DCC.IndexOf("WorkTypeID")].Text.Trim();
		row.Attributes.Add("ITEMID", itemId);
		row.Attributes.Add("WorkTypeID", workTypeId);

		string title = row.Cells[DCC["TITLE"].Ordinal].Text.Trim();
		row.Cells[DCC["ItemID"].Ordinal].Controls.Add(createEditLink_WorkItem(itemId, Server.HtmlDecode(title)));
		row.Cells[DCC["ItemID"].Ordinal].ToolTip = Server.HtmlDecode(title);
		row.Cells[DCC["TITLE"].Ordinal].ToolTip = Server.HtmlDecode(title);

		//add QM controls
		if (this.CanEditWorkItem)
		{
			row.Cells[DCC.IndexOf("WORKREQUEST")].Controls.Add(WTSUtility.CreateGridDropdownList("WORKREQUEST", itemId, row.Cells[DCC.IndexOf("WORKREQUEST")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("WORKREQUESTID")].Text.Replace("&nbsp;", " ").Trim(), 180));
			row.Cells[DCC.IndexOf("WORKITEMTYPE")].Controls.Add(WTSUtility.CreateGridDropdownList("WORKITEMTYPE", itemId, row.Cells[DCC.IndexOf("WORKITEMTYPE")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("WORKITEMTYPEID")].Text.Replace("&nbsp;", " ").Trim(), 0));
			row.Cells[DCC.IndexOf("Websystem")].Controls.Add(WTSUtility.CreateGridDropdownList("Websystem", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Websystem")].Text).Trim(), row.Cells[DCC.IndexOf("WTS_SYSTEMID")].Text.Replace("&nbsp;", " ").Trim(), 120));
			row.Cells[DCC.IndexOf("ALLOCATION")].Controls.Add(WTSUtility.CreateGridDropdownList("ALLOCATION", itemId, row.Cells[DCC.IndexOf("ALLOCATION")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("ALLOCATIONID")].Text.Replace("&nbsp;", " ").Trim(), 195));
			//row.Cells[DCC.IndexOf("Version")].Controls.Add(WTSUtility.CreateGridDropdownList("ProductVersion", itemId, row.Cells[DCC.IndexOf("Version")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("ProductVersionID")].Text.Replace("&nbsp;", " ").Trim(), 0));
			row.Cells[DCC.IndexOf("PRIORITY")].Controls.Add(WTSUtility.CreateGridDropdownList("PRIORITY", itemId, row.Cells[DCC.IndexOf("PRIORITY")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("PRIORITYID")].Text.Replace("&nbsp;", " ").Trim(), 0));
			row.Cells[DCC.IndexOf("Primary_Developer")].Controls.Add(WTSUtility.CreateGridDropdownList("Primary_Developer", itemId, row.Cells[DCC.IndexOf("Primary_Developer")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("PRIMARYRESOURCEID")].Text.Replace("&nbsp;", " ").Trim(), 0));
			row.Cells[DCC.IndexOf("Assigned")].Controls.Add(WTSUtility.CreateGridDropdownList("Assigned", itemId, row.Cells[DCC.IndexOf("Assigned")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("ASSIGNEDRESOURCEID")].Text.Replace("&nbsp;", " ").Trim(), 0));

			row.Cells[DCC.IndexOf("SubmittedBy")].Text = row.Cells[DCC.IndexOf("SubmittedBy")].Text.Trim();// +"<br/>" + row.Cells[DCC.IndexOf("CREATEDDATE")].Text.Trim();

			row.Cells[DCC.IndexOf("WorkType")].Controls.Add(WTSUtility.CreateGridDropdownList("WorkType", itemId, row.Cells[DCC.IndexOf("WorkType")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("WorkTypeID")].Text.Replace("&nbsp;", " ").Trim(), 0));

			bool allow = true;
			string status = row.Cells[DCC.IndexOf("STATUS")].Text.Replace("&nbsp;", " ").Trim();
			if (status.ToUpper() == "NEW" || status.ToUpper() == "ON HOLD" || status.ToUpper() == "IN PROGRESS")
			{
				int WorkItemID = 0;
				int.TryParse(itemId, out WorkItemID);

				if (WorkItemID > 0)
				{
					DataTable dt = WorkloadItem.WorkItem_GetTaskList(workItemID: WorkItemID);

					if (dt != null)
					{
						try
						{
							int Sub_Task_New_Count = (from r in dt.AsEnumerable()
													  where r.Field<string>("STATUS").Trim().ToUpper() == "NEW"
													  select r).Count();
							if (status.ToUpper() == "NEW" && dt.Rows.Count == Sub_Task_New_Count) allow = false;

							int Sub_Task_OnHold_Count = (from r in dt.AsEnumerable()
														 where r.Field<string>("STATUS").Trim().ToUpper() == "ON HOLD"
														 select r).Count();
							if (status.ToUpper() == "ON HOLD" && dt.Rows.Count == Sub_Task_OnHold_Count) allow = false;

							int Sub_Task_Closed_Count = (from r in dt.AsEnumerable()
														 where r.Field<string>("STATUS").Trim().ToUpper() == "CLOSED"
														 select r).Count();
							if (status.ToUpper() == "IN PROGRESS" && dt.Rows.Count != Sub_Task_Closed_Count) allow = false;
						}
						catch (Exception) { }
					}
				}
			}
			row.Cells[DCC.IndexOf("STATUS")].Controls.Add(WTSUtility.CreateGridDropdownList("STATUS", itemId, status, row.Cells[DCC.IndexOf("STATUSID")].Text.Replace("&nbsp;", " ").Trim(), 0));
			try
			{
				DropDownList ddlStatus = (DropDownList)row.Cells[DCC.IndexOf("STATUS")].Controls[0];
				if (!allow) ddlStatus.Enabled = false;
			}
			catch (Exception) { }

			row.Cells[DCC.IndexOf("Progress")].Controls.Add(WTSUtility.CreateGridDropdownList("Progress", itemId, row.Cells[DCC.IndexOf("Progress")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("Progress")].Text.Replace("&nbsp;", " ").Trim(), 0));

			bool check = false;
			if (row.Cells[DCC.IndexOf("Production")].HasControls()
				&& row.Cells[DCC.IndexOf("Production")].Controls[0] is CheckBox)
			{
				check = ((CheckBox)row.Cells[DCC.IndexOf("Production")].Controls[0]).Checked;
			}
			else if (row.Cells[DCC.IndexOf("Production")].Text == "1")
			{
				check = true;
			}
			row.Cells[DCC.IndexOf("Production")].Controls.Clear();
			row.Cells[DCC.IndexOf("Production")].Controls.Add(WTSUtility.CreateGridCheckBox("Production", itemId, check));
			
			check = false;
			if (row.Cells[DCC.IndexOf("ARCHIVE")].HasControls()
				&& row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0] is CheckBox)
			{
				check = ((CheckBox)row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0]).Checked;
			}
			else if (row.Cells[DCC.IndexOf("ARCHIVE")].Text == "1")
			{
				check = true;
			}
			row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Clear();
			row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Add(WTSUtility.CreateGridCheckBox("ARCHIVE", itemId, check));

			row.Cells[DCC.IndexOf("ProductionStatus")].Controls.Add(WTSUtility.CreateGridDropdownList("ProductionStatus", itemId, row.Cells[DCC.IndexOf("ProductionStatus")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("ProductionStatusID")].Text.Replace("&nbsp;", " ").Trim(), 0));
		}
	}

	void grdWorkload_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdWorkload.PageIndex = e.NewPageIndex;
		loadGridData(false);
		/*if (HttpContext.Current.Session["dtWorkItem_QM"] == null)
		{
			loadGridData();
		}
		else
		{
			grdWorkload.DataSource = (DataTable)HttpContext.Current.Session["dtWorkItem_QM"];
		}*/
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i == DCC.IndexOf("WORKREQUEST")
				|| i == DCC.IndexOf("WORKITEMTYPE")
				|| i == DCC.IndexOf("TITLE")
				|| i == DCC.IndexOf("Websystem")
				|| i == DCC.IndexOf("Allocation")
				|| i == DCC.IndexOf("Primary_Analyst")
				|| i == DCC.IndexOf("Primary_Developer")
				|| i == DCC.IndexOf("Assigned")
				|| i == DCC.IndexOf("SubmittedBy")
				|| i == DCC.IndexOf("ProductionStatus"))
			{
				row.Cells[i].Style["text-align"] = "left";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "center";
			}
		}

		//more column formatting
		row.Cells[DCC.IndexOf("ItemID")].Style["width"] = "46px";
		row.Cells[DCC.IndexOf("TITLE")].Style["width"] = "200px";
		row.Cells[DCC.IndexOf("WORKITEMTYPE")].Style["width"] = "115px";
		row.Cells[DCC.IndexOf("WORKREQUEST")].Style["width"] = "182px";
		row.Cells[DCC.IndexOf("Websystem")].Style["width"] = "125px";
		row.Cells[DCC.IndexOf("Allocation")].Style["width"] = "200px";
		row.Cells[DCC.IndexOf("Production")].Style["width"] = "35px";
		row.Cells[DCC.IndexOf("Version")].Style["width"] = "60px";
		row.Cells[DCC.IndexOf("Priority")].Style["width"] = "55px";
		//row.Cells[DCC.IndexOf("Primary_Analyst")].Style["width"] = "85px";
		row.Cells[DCC.IndexOf("Primary_Developer")].Style["width"] = "135px";
		row.Cells[DCC.IndexOf("Assigned")].Style["width"] = "135px";
		row.Cells[DCC.IndexOf("SubmittedBy")].Style["width"] = "135px";
		row.Cells[DCC.IndexOf("WorkType")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("Status")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("Progress")].Style["width"] = "55px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "50px";
		row.Cells[DCC.IndexOf("ProductionStatus")].Style["width"] = "60px";
	}

	LinkButton createEditLink_WorkItem(string workItemId = "", string tooltip = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEditWorkItem_click('{0}');return false;", workItemId);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEditWorkItem_{0}", workItemId);
		lb.Attributes["name"] = string.Format("lbEditWorkItem_{0}", workItemId);
		lb.ToolTip = string.IsNullOrEmpty(tooltip) ? string.Format("Edit Work Item [{0}]", workItemId) : tooltip;
		lb.Text = workItemId;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}
	
	
	[WebMethod(true)]
	public static string SaveChanges(string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
			, { "failed", "0" }
			, { "savedIds", "" }
			, { "failedIds", "" }
			, { "error", "" } };
		bool saved = false;
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

				//save
				foreach (DataRow dr in dtjson.Rows)
				{
					id = 0;
					tempMsg = string.Empty;
					int.TryParse(dr["ITEMID"].ToString(), out id);

					WorkloadItem wi = WorkloadItem.WorkItem_GetObject(id);
					//update object with new values
					wi = parseDataRow(wi, dtjson.Clone(), dr);

					saved = WorkloadItem.WorkItem_QM_Update(wi, out tempMsg);

					if (saved)
					{
						ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
						savedQty += 1;
						Workload.SendWorkloadEmail("WorkItem", false, id);
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

	private static WorkloadItem parseDataRow(WorkloadItem wi, DataTable dt, DataRow dr)
	{
		int tempId = 0;

		//HttpServerUtility server = HttpContext.Current.Server;
		
		try
		{	        
			if (dt.Columns.Contains("WORKREQUEST"))
			{
				wi.WorkRequestID = int.TryParse(dr["WORKREQUEST"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("WORKITEMTYPE"))
			{
				wi.WorkItemTypeID = int.TryParse(dr["WORKITEMTYPE"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("Websystem"))
			{
				wi.WTS_SystemID = int.TryParse(dr["Websystem"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("ALLOCATION"))
			{
				wi.AllocationID = int.TryParse(dr["ALLOCATION"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("Version"))
			{
				wi.ProductVersionID = int.TryParse(dr["Version"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("Production"))
			{
				wi.Production = int.TryParse(dr["Production"].ToString(), out tempId) ? (tempId == 1) : false;
			}
			if (dt.Columns.Contains("PRIORITY"))
			{
				wi.PriorityID = int.TryParse(dr["PRIORITY"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("Primary_Developer"))
			{
				wi.PrimaryResourceID = int.TryParse(dr["Primary_Developer"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("Assigned"))
			{
				wi.AssignedResourceID = int.TryParse(dr["Assigned"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("WorkType"))
			{
				wi.WorkTypeID = int.TryParse(dr["WorkType"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("STATUS"))
			{
				wi.StatusID = int.TryParse(dr["STATUS"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("Progress"))
			{
				wi.CompletionPercent = int.TryParse(dr["Progress"].ToString(), out tempId) ? tempId : 0;
			}
			if (dt.Columns.Contains("ARCHIVE"))
			{
				wi.Archive = int.TryParse(dr["ARCHIVE"].ToString(), out tempId) ? (tempId == 1) : false;
			}
			if (dt.Columns.Contains("ProductionStatus"))
			{
				wi.ProductionStatusID = int.TryParse(dr["ProductionStatus"].ToString(), out tempId) ? tempId : 0;
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			return null;
		}
			
		return wi;
	}

    [WebMethod(true)]
    public static int WorkItem_TaskID_Get(int itemID, int taskNumber)
    {
        try
        {
            return WorkItem_Task.WorkItem_TaskID_Get(itemID, taskNumber);
        }
        catch (Exception) { return -1; }
    }
}