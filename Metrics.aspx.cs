﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

public partial class Metrics : System.Web.UI.Page
{
	protected List<string> userRoles;
	protected bool CanViewRequest = false;
	protected bool CanViewWorkload = false;

	protected DataColumnCollection DCC_Workload;
	protected DataColumnCollection DCC_Request;
	protected GridCols columnData_Workload = new GridCols();
	protected GridCols columnData_Request = new GridCols();

	protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

	protected string NewDescription = string.Empty;
	protected string InProgressDescription = string.Empty;
	protected string ReOpenedDescription = string.Empty;
	protected string InfoRequestedDescription = string.Empty;
	protected string CheckedInDescription = string.Empty;
	protected string DeployedDescription = string.Empty;
	protected string ClosedDescription = string.Empty;

	protected string InvestigationDescription = string.Empty;
	protected string PlanningDescription = string.Empty;
	protected string DesignDescription = string.Empty;
	protected string DevelopDescription = string.Empty;
	protected string TestingDescription = string.Empty;
	protected string DeployDescription = string.Empty;
	protected string ReviewDescription = string.Empty;

	protected void Page_Load(object sender, EventArgs e)
	{
		string[] roles = UserManagement.GetUserRoles_Membership();
		this.userRoles = new List<string>(roles);
		this.CanViewRequest = UserManagement.UserCanView(WTSModuleOption.WorkRequest);
		this.CanViewWorkload = UserManagement.UserCanView(WTSModuleOption.WorkItem);

		readQueryString();
		init();

		loadMetrics();
	}
	private void readQueryString()
	{
		
	}

	private void init()
	{
		gridWorkloadMetrics.GridHeaderRowDataBound += gridWorkloadMetrics_GridHeaderRowDataBound;
		gridWorkloadMetrics.GridRowDataBound += grdWorkloadMetrics_GridRowDataBound;

		//gridRequestMetrics.GridHeaderRowDataBound += gridRequestMetrics_GridHeaderRowDataBound;
		//gridRequestMetrics.GridRowDataBound += grdRequestMetrics_GridRowDataBound;
	}

	void gridWorkloadMetrics_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData_Workload.SetupGridHeader(e.Row);
		GridViewRow row = e.Row;

		formatHeaderColumnDisplay_Workload(ref row);
	}
	void grdWorkloadMetrics_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData_Workload.SetupGridBody(e.Row);

		GridViewRow row = e.Row;
		formatColumnDisplay_Workload(ref row);

		//change color if total row
		if (row.Cells[DCC_Workload["PriorityLabel"].Ordinal].Text.IndexOf("TOTAL", StringComparison.CurrentCultureIgnoreCase) > -1)
		{
			row.CssClass = "metricsRow_Total";
			for (int i = 0; i < row.Cells.Count; i++)
			{
				row.Cells[i].Style["background-color"] = "#E6E6E6";
			}
		}

		if (row.Cells[DCC_Workload["PriorityLabel"].Ordinal].Text.IndexOf("Description", StringComparison.CurrentCultureIgnoreCase) > -1)
		{
			row.Style["display"] = "none";
		}

		for (int j = 0; j < row.Cells.Count; j++)
		{
			try
			{
				string columnType = string.Empty;
				string rowType = string.Empty;
				string cellValue = string.Empty;

				if (j == DCC_Workload["PriorityLabel"].Ordinal)
				{
					columnType = "Priority";
				}
				else if (j == DCC_Workload["New"].Ordinal)
				{
					columnType = "New";
				}
				else if (j == DCC_Workload["In_Progress"].Ordinal)
				{
					columnType = "In-Progress";
				}
				else if (j == DCC_Workload["Re_Opened"].Ordinal)
				{
					columnType = "Re-Opened";
				}
				else if (j == DCC_Workload["Info_Requested"].Ordinal)
				{
					columnType = "Info Requested";
				}
				else if (j == DCC_Workload["Checked_In"].Ordinal)
				{
					columnType = "Checked-In";
				}
				else if (j == DCC_Workload["Deployed"].Ordinal)
				{
					columnType = "Deployed";
				}
				else if (j == DCC_Workload["Closed"].Ordinal)
				{
					columnType = "Closed";
				}
				else
				{
					continue;
				}

				rowType = row.Cells[DCC_Workload["PriorityLabel"].Ordinal].Text.Substring(0, row.Cells[DCC_Workload["PriorityLabel"].Ordinal].Text.IndexOf(" ("));
				cellValue = row.Cells[j].Text;

				if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) >= 0
					|| userRoles.FindIndex(s => s.Contains("WorkRequest")) >= 0
					|| userRoles.FindIndex(s => s.Contains("SustainmentRequest")) >= 0
					|| userRoles.FindIndex(s => s.Contains("WorkItem")) >= 0
					|| userRoles.FindIndex(s => s.Contains("Task")) >= 0)
				{
					row.Cells[j].Controls.Add(createLink("Work Tasks", columnType, rowType, cellValue));
				}
			}
			catch (Exception) { }
		}
	}

	void formatHeaderColumnDisplay_Workload(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i == DCC_Workload["PriorityLabel"].Ordinal)
			{
				row.Cells[i].Style["text-align"] = "left";
				row.Cells[i].Style["padding-left"] = "5px";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "center";

				if (row.RowIndex == 1)
				{
					HtmlGenericControl title = new HtmlGenericControl();
					title.InnerText = row.Cells[i].Text;
					title.Style["padding-right"] = "2px";
					row.Cells[i].Controls.Add(title);
					try { row.Cells[i].Controls.Add(createHelpIcon(row.Cells[i].Text)); }
					catch (Exception) { }
				}
			}
		}


	}
	void formatColumnDisplay_Workload(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i == DCC_Workload["PriorityLabel"].Ordinal)
			{
				row.Cells[i].Style["text-align"] = "left";
				row.Cells[i].Style["padding-left"] = "5px";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "center";
			}
		}

		//column widths
		row.Cells[DCC_Workload["PriorityLabel"].Ordinal].Style["width"] = "130px";
		row.Cells[DCC_Workload["New"].Ordinal].Style["width"] = "65px";
		row.Cells[DCC_Workload["In_Progress"].Ordinal].Style["width"] = "85px";
		row.Cells[DCC_Workload["Re_Opened"].Ordinal].Style["width"] = "80px";
		row.Cells[DCC_Workload["Info_Requested"].Ordinal].Style["width"] = "105px";
		row.Cells[DCC_Workload["Info_Provided"].Ordinal].Style["width"] = "85px";
		row.Cells[DCC_Workload["Checked_In"].Ordinal].Style["width"] = "80px";
		row.Cells[DCC_Workload["Deployed"].Ordinal].Style["width"] = "105px";
		row.Cells[DCC_Workload["Closed"].Ordinal].Style["width"] = "80px";
	}
	

	//void gridRequestMetrics_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	//{
	//	columnData_Request.SetupGridHeader(e.Row);
	//	GridViewRow row = e.Row;

	//	formatHeaderColumnDisplay_Request(ref row);
	//}
	void grdRequestMetrics_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData_Request.SetupGridBody(e.Row);

		GridViewRow row = e.Row;
		formatColumnDisplay_Request(ref row);

		//change color if total row
		if (row.Cells[DCC_Request["TypeLabel"].Ordinal].Text.IndexOf("TOTAL", StringComparison.CurrentCultureIgnoreCase) > -1)
		{
			row.CssClass = "metricsRow_Total";
			for (int i = 0; i < row.Cells.Count; i++)
			{
				row.Cells[i].Style["background-color"] = "#E6E6E6";
			}
		}

		if (row.Cells[DCC_Workload["PriorityLabel"].Ordinal].Text.IndexOf("Description", StringComparison.CurrentCultureIgnoreCase) > -1)
		{
			row.Style["display"] = "none";
		}

		for (int j = 0; j < row.Cells.Count; j++)
		{
			try
			{
				string columnType = string.Empty;
				string rowType = string.Empty;
				string cellValue = string.Empty;

				if (j == DCC_Request["TypeLabel"].Ordinal)
				{
					columnType = "Request Type";
				}
				else if (j == DCC_Request["Investigation"].Ordinal)
				{
					columnType = "Investigation";
				}
				else if (j == DCC_Request["Planning"].Ordinal)
				{
					columnType = "Planning";
				}
				else if (j == DCC_Request["Design"].Ordinal)
				{
					columnType = "Design";
				}
				else if (j == DCC_Request["Develop"].Ordinal)
				{
					columnType = "Develop";
				}
				else if (j == DCC_Request["Testing"].Ordinal)
				{
					columnType = "Testing";
				}
				else if (j == DCC_Request["Deploy"].Ordinal)
				{
					columnType = "Deploy";
				}
				else if (j == DCC_Request["Review"].Ordinal)
				{
					columnType = "Review";
				}
				else
				{
					continue;
				}

				rowType = row.Cells[DCC_Request["TypeLabel"].Ordinal].Text.Substring(0, row.Cells[DCC_Request["TypeLabel"].Ordinal].Text.IndexOf(" ("));
				cellValue = row.Cells[j].Text;
			}
			catch (Exception) { }
		}
	}

	void formatHeaderColumnDisplay_Request(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i == DCC_Request["TypeLabel"].Ordinal)
			{
				row.Cells[i].Style["text-align"] = "left";
				row.Cells[i].Style["padding-left"] = "5px";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "center";

				if (row.RowIndex == 1)
				{
					HtmlGenericControl title = new HtmlGenericControl();
					title.InnerText = row.Cells[i].Text;
					title.Style["padding-right"] = "2px";
					row.Cells[i].Controls.Add(title);
					try { row.Cells[i].Controls.Add(createHelpIcon(row.Cells[i].Text)); }
					catch (Exception) { }
				}
			}
		}


	}
	void formatColumnDisplay_Request(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i == DCC_Request["TypeLabel"].Ordinal)
			{
				row.Cells[i].Style["text-align"] = "left";
				row.Cells[i].Style["padding-left"] = "5px";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "center";
			}
		}

		//column widths
		row.Cells[DCC_Request["TypeLabel"].Ordinal].Style["width"] = "100px";
		row.Cells[DCC_Request["Investigation"].Ordinal].Style["width"] = "90px";
		row.Cells[DCC_Request["Planning"].Ordinal].Style["width"] = "70px";
		row.Cells[DCC_Request["Design"].Ordinal].Style["width"] = "55px";
		row.Cells[DCC_Request["Develop"].Ordinal].Style["width"] = "60px";
		row.Cells[DCC_Request["Testing"].Ordinal].Style["width"] = "55px";
		row.Cells[DCC_Request["Deploy"].Ordinal].Style["width"] = "50px";
		row.Cells[DCC_Request["Review"].Ordinal].Style["width"] = "55px";
	}
	


	private void loadMetrics()
	{
		DataSet ds = WorkloadItem.Metrics_Get();
		DataTable dtDescription = new DataTable();

		if (ds.Tables.Contains("Workload"))
		{
			DataTable dtItemMetrics = ds.Tables["Workload"];
			if (dtItemMetrics != null)
			{
				DCC_Workload = dtItemMetrics.Columns;

				InitializeColumnData_Workload(ref dtItemMetrics);
				SetupMultiHeader_Workload(dtItemMetrics);
				dtItemMetrics.AcceptChanges();

				try
				{
					dtDescription = dtItemMetrics.Copy();
					dtDescription.DefaultView.RowFilter = "PriorityLabel = 'Description'";
					dtDescription = dtDescription.DefaultView.ToTable();

					if (dtDescription != null && dtDescription.Rows.Count > 0)
					{
						NewDescription = dtDescription.Rows[0]["New"].ToString();
						InProgressDescription = dtDescription.Rows[0]["In_Progress"].ToString();
						ReOpenedDescription = dtDescription.Rows[0]["Re_Opened"].ToString();
						InfoRequestedDescription = dtDescription.Rows[0]["Info_Requested"].ToString();
						CheckedInDescription = dtDescription.Rows[0]["Checked_In"].ToString();
						DeployedDescription = dtDescription.Rows[0]["Deployed"].ToString();
						ClosedDescription = dtDescription.Rows[0]["Closed"].ToString();
					}
				}
				catch (Exception) { }
			}

			gridWorkloadMetrics.DataSource = dtItemMetrics;
			gridWorkloadMetrics.DataBind();
		}

		//if (ds.Tables.Contains("Request"))
		//{
		//	DataTable dtRequestMetrics = ds.Tables["Request"];
		//	if (dtRequestMetrics != null)
		//	{
		//		DCC_Request = dtRequestMetrics.Columns;

		//		InitializeColumnData_Request(ref dtRequestMetrics);
		//		SetupMultiHeader_Request(dtRequestMetrics);
		//		dtRequestMetrics.AcceptChanges();

		//		try
		//		{
		//			dtDescription = dtRequestMetrics.Copy();
		//			dtDescription.DefaultView.RowFilter = "TypeLabel = 'Description'";
		//			dtDescription = dtDescription.DefaultView.ToTable();

		//			if (dtDescription != null && dtDescription.Rows.Count > 0)
		//			{
		//				InvestigationDescription = dtDescription.Rows[0]["Investigation"].ToString();
		//				PlanningDescription = dtDescription.Rows[0]["Planning"].ToString();
		//				DesignDescription = dtDescription.Rows[0]["Design"].ToString();
		//				DevelopDescription = dtDescription.Rows[0]["Develop"].ToString();
		//				TestingDescription = dtDescription.Rows[0]["Testing"].ToString();
		//				DeployDescription = dtDescription.Rows[0]["Deploy"].ToString();
		//				ReviewDescription = dtDescription.Rows[0]["Review"].ToString();
		//			}
		//		}
		//		catch (Exception) { }
		//	}

		//	gridRequestMetrics.DataSource = dtRequestMetrics;
		//	gridRequestMetrics.DataBind();
		//}
	}


	protected void InitializeColumnData_Workload(ref DataTable dt)
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
					case "PriorityLabel":
						displayName = "Priority";
						groupName = "Work Tasks";
						blnVisible = true;
						break;
					case "New":
						displayName = "New";
						groupName = "Open";
						blnVisible = true;
						break;
					case "In_Progress":
						displayName = "In-Progress";
						groupName = "Open";
						blnVisible = true;
						break;
					case "Re_Opened":
						displayName = "Re-Opened";
						groupName = "Open";
						blnVisible = true;
						break;
					case "Info_Requested":
						displayName = "Info Requested";
						groupName = "Open";
						blnVisible = true;
						break;
					case "Info Provided":
						displayName = "Info Provided";
						groupName = "Open";
						blnVisible = true;
						break;
					case "Checked_In":
						displayName = "Checked-In";
						groupName = "Open";
						blnVisible = true;
						break;
					case "Deployed":
						displayName = "Deployed";
						groupName = "Awaiting Closure";
						blnVisible = true;
						break;
					case "Closed":
						displayName = "Closed";
						groupName = "Closed";
						blnVisible = true;
						break;
					case "X":
						displayName = "&nbsp;";
						blnVisible = false;
						break;
				}

				columnData_Workload.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				columnData_Workload.Columns.Item(columnData_Workload.Columns.Count - 1).CanReorder = blnOrderable;
				columnData_Workload.Columns.Item(columnData_Workload.Columns.Count - 1).GroupName = groupName;
			}

			//Initialize the columnData
			columnData_Workload.Initialize(ref dt, ";", "~", "|");

			//Get sortable columns and default column order
			SortableColumns = columnData_Workload.SortableColumnsToString();
			DefaultColumnOrder = columnData_Workload.DefaultColumnOrderToString();
			//Sort and Reorder Columns
			columnData_Workload.ReorderDataTable(ref dt, ColumnOrder);
			columnData_Workload.SortDataTable(ref dt, SortOrder);
			SelectedColumnOrder = columnData_Workload.CurrentColumnOrderToString();
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}
	protected void SetupMultiHeader_Workload(DataTable dt)
	{
		iti_Tools_Sharp.DynamicHeader dynamicHeader = new iti_Tools_Sharp.DynamicHeader();
		//If you want to start with the original header
		//copy the Columns of the dt into the dynamic header
		//else you can just create the rows you want to add
		iti_Tools_Sharp.DynamicHeaderRow firstHeaderRow = new iti_Tools_Sharp.DynamicHeaderRow();
		dynamicHeader.Rows.Add(firstHeaderRow);
		firstHeaderRow.CssClass = "metricsHeader";

		iti_Tools_Sharp.DynamicHeaderRow secondHeaderRow = new iti_Tools_Sharp.DynamicHeaderRow();
		dynamicHeader.Rows.Add(secondHeaderRow);
		secondHeaderRow.CssClass = "metricsSubHeader";

		for (int i = 0; i < dt.Columns.Count; i++)
		{
			iti_Tools_Sharp.DynamicHeaderCell cell = new iti_Tools_Sharp.DynamicHeaderCell();
			cell.Text = dt.Columns[i].ColumnName;
			firstHeaderRow.Cells.Add(cell);
			cell.CssClass = "metricsHeader";

			cell = new iti_Tools_Sharp.DynamicHeaderCell();
			cell.Text = dt.Columns[i].ColumnName;
			secondHeaderRow.Cells.Add(cell);
			cell.CssClass = "metricsSubHeader";
			cell.Style["border-top"] = "1px solid grey";
		}

		gridWorkloadMetrics.DynamicHeader = dynamicHeader;
	}

	protected void InitializeColumnData_Request(ref DataTable dt)
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
					case "TypeLabel":
						groupName = "Work Requests";
						displayName = "Request Type";
						blnVisible = true;
						break;
					case "Investigation":
						groupName = "Open";
						displayName = "Investigation";
						blnVisible = true;
						break;
					case "Planning":
						groupName = "Open";
						displayName = "Planning";
						blnVisible = true;
						break;
					case "Investigation_Planning":
						groupName = "Open";
						displayName = "Investigation/Planning";
						blnVisible = true;
						break;
					case "Design":
						groupName = "Open";
						displayName = "Design";
						blnVisible = true;
						break;
					case "Develop":
						groupName = "Open";
						displayName = "Develop";
						blnVisible = true;
						break;
					case "Testing":
						groupName = "Open";
						displayName = "Testing";
						blnVisible = true;
						break;
					case "Deploy":
						groupName = "Open";
						displayName = "Deploy";
						blnVisible = true;
						break;
					case "Review":
						groupName = "Open";
						displayName = "Review";
						blnVisible = true;
						break;
				}

				columnData_Request.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				columnData_Request.Columns.Item(columnData_Request.Columns.Count - 1).CanReorder = blnOrderable;
				columnData_Request.Columns.Item(columnData_Request.Columns.Count - 1).GroupName = groupName;
			}

			//Initialize the columnData
			columnData_Request.Initialize(ref dt, ";", "~", "|");

			//Get sortable columns and default column order
			SortableColumns = columnData_Request.SortableColumnsToString();
			DefaultColumnOrder = columnData_Request.DefaultColumnOrderToString();
			//Sort and Reorder Columns
			columnData_Request.ReorderDataTable(ref dt, ColumnOrder);
			columnData_Request.SortDataTable(ref dt, SortOrder);
			SelectedColumnOrder = columnData_Request.CurrentColumnOrderToString();
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}
	//protected void SetupMultiHeader_Request(DataTable dt)
	//{
	//	iti_Tools_Sharp.DynamicHeader dynamicHeader = new iti_Tools_Sharp.DynamicHeader();
	//	//If you want to start with the original header
	//	//copy the Columns of the dt into the dynamic header
	//	//else you can just create the rows you want to add
	//	iti_Tools_Sharp.DynamicHeaderRow firstHeaderRow = new iti_Tools_Sharp.DynamicHeaderRow();
	//	dynamicHeader.Rows.Add(firstHeaderRow);
	//	firstHeaderRow.CssClass = "metricsHeader";

	//	iti_Tools_Sharp.DynamicHeaderRow secondHeaderRow = new iti_Tools_Sharp.DynamicHeaderRow();
	//	dynamicHeader.Rows.Add(secondHeaderRow);
	//	secondHeaderRow.CssClass = "metricsSubHeader";

	//	for (int i = 0; i < dt.Columns.Count; i++)
	//	{
	//		iti_Tools_Sharp.DynamicHeaderCell cell = new iti_Tools_Sharp.DynamicHeaderCell();
	//		cell.Text = dt.Columns[i].ColumnName;
	//		firstHeaderRow.Cells.Add(cell);
	//		cell.CssClass = "metricsHeader";

	//		cell = new iti_Tools_Sharp.DynamicHeaderCell();
	//		cell.Text = dt.Columns[i].ColumnName;
	//		secondHeaderRow.Cells.Add(cell);
	//		cell.CssClass = "metricsSubHeader";
	//		cell.Style["border-top"] = "1px solid grey";
	//	}

	//	gridRequestMetrics.DynamicHeader = dynamicHeader;
	//}

	HtmlGenericControl createLink(string metricType, string columnType, string rowType, string cellValue)
	{
		try
		{
			HtmlGenericControl html = new HtmlGenericControl();
			string[] splitValue = {};

			if (columnType == "Request Type" || columnType == "Priority")
			{
				splitValue = Regex.Match(cellValue, @"\(([^)]*)\)").Groups[1].Value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				html.InnerHtml = rowType;
				html.InnerHtml += " (";
			}
			else
			{
				splitValue = cellValue.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				html.InnerHtml += "(";
			}

			if (int.Parse(splitValue[0]) > 0)
			{
				html.InnerHtml += "<a onclick='lbGoToData_click(\"" + metricType + "\",\"" + columnType + "\",\"" + rowType + "\",\"Enterprise\"); return false;'";
				html.InnerHtml += " title='Go to " + metricType + "'";
				html.InnerHtml += " style='text-decoration:underline; color:blue; cursor:pointer;'>" + splitValue[0] + "</a>";
			}
			else
			{
				html.InnerHtml += splitValue[0];
			}

			html.InnerHtml += " / ";

			if (int.Parse(splitValue[1]) > 0)
			{
				html.InnerHtml += "<a onclick='lbGoToData_click(\"" + metricType + "\",\"" + columnType + "\",\"" + rowType + "\",\"My Data\"); return false;'";
				html.InnerHtml += " title='Go to " + metricType + "'";
				html.InnerHtml += " style='text-decoration:underline; color:blue; cursor:pointer;'>" + splitValue[1] + "</a>";
			}
			else
			{
				html.InnerHtml += splitValue[1];
			}

			html.InnerHtml += ")";

			return html;
		}
		catch (Exception)
		{
			return null;
		}
	}

	Image createHelpIcon(string field)
	{
		Image imgHelp = new Image();
		string description = string.Empty;

		switch (field)
		{
			case "New":
				description = NewDescription;
				break;
			case "In-Progress":
				description = InProgressDescription;
				break;
			case "Re-Opened":
				description = ReOpenedDescription;
				break;
			case "Info Requested":
				description = InfoRequestedDescription;
				break;
			case "Checked-In":
				description = CheckedInDescription;
				break;
			case "Deployed":
				description = DeployedDescription;
				break;
			case "Closed":
				description = ClosedDescription;
				break;
			case "Investigation":
				description = InvestigationDescription;
				break;
			case "Planning":
				description = PlanningDescription;
				break;
			case "Design":
				description = DesignDescription;
				break;
			case "Develop":
				description = DevelopDescription;
				break;
			case "Testing":
				description = TestingDescription;
				break;
			case "Deploy":
				description = DeployDescription;
				break;
			case "Review":
				description = ReviewDescription;
				break;
			default:
				return null;
		}

		imgHelp.Style["cursor"] = "pointer";
		imgHelp.Style["border"] = "none";
		imgHelp.Height = 12;
		imgHelp.Width = 12;
		imgHelp.ImageUrl = "Images/Icons/help.png";
		imgHelp.AlternateText = description;
		imgHelp.ToolTip = description;
		imgHelp.Attributes.Add("Onclick", "MessageBox('" + description + "');");

		return imgHelp;
	}
}