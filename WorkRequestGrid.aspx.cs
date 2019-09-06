﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;


public partial class WorkRequestGrid : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected int MyView = 0; //Enterprise, My Data

	protected bool _myData = true;
	protected int _showArchived = 0;

	protected bool _refreshData = false;
	protected bool _export = false;

	protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

	protected bool CanViewRequest = false;
	protected bool CanViewSR = false;
	protected bool CanViewWorkItem = false;

	protected bool CanEditRequest = false;
	protected bool CanEditSR = false;
	protected bool CanEditWorkItem = false;


	protected void Page_Load(object sender, EventArgs e)
	{
		this.CanEditRequest = UserManagement.UserCanEdit(WTSModuleOption.WorkRequest);
		this.CanEditSR = UserManagement.UserCanEdit(WTSModuleOption.SustainmentRequest);
		this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
		this.CanViewRequest = CanEditRequest || UserManagement.UserCanView(WTSModuleOption.WorkRequest);
		this.CanViewSR = CanEditSR || UserManagement.UserCanView(WTSModuleOption.SustainmentRequest);
		this.CanViewWorkItem = CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

		readQueryString();

		initControls();
		loadMenus();
		loadRequests();
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

		if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
			|| Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
		{
			_myData = true;
		}
		else
		{
			_myData = false;
		}

		if (Request.QueryString["ShowArchived"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"].ToString()), out this._showArchived);
		}

		if (Request.QueryString["View"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["View"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["View"].ToString()), out this.MyView);
		}

		if (Request.QueryString["sortOrder"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
		{
			this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
		}
		if (Request.QueryString["Export"] != null &&
			!string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
		{
			bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);
		}
	}

	private void initControls()
	{
		grdRequest.GridHeaderRowDataBound += grdRequest_GridHeaderRowDataBound;
		grdRequest.GridRowDataBound += grdRequest_GridRowDataBound;
		grdRequest.GridPageIndexChanging += grdRequest_GridPageIndexChanging;

		WTSUtility.SelectDdlItem(ddlView, this.MyView.ToString());
	}

	private void loadMenus()
	{

	}


	#region Data

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
					case "ROW_ID":
						displayName = "NO.";
						blnVisible = true;
						break;
					case "WORKREQUESTID":
						displayName = "Request Number";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "CONTRACT":
						displayName = "Contract";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "ORGANIZATION":
						displayName = "Department /<br/>Organization";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "Request_Type":
						displayName = "Request Type";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "Work_Scope":
						displayName = "Scope";
						blnVisible = true;
						blnSortable = false;
						blnOrderable = false;
						break;
					case "TITLE":
						displayName = "Title";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "SR_Count":
						displayName = "SRs";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						break;
					case "WorkItem_Count":
						displayName = "Work Items";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						break;
					case "PRIORITY":
						displayName = "Operations<br/>Priority";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "SME":
						displayName = "SME";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "Lead_Tech_Writer":
						displayName = "Lead Tech<br/>Writer";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "Lead_Resource":
						displayName = "Lead Resource";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "PD2_TDR_Phase":
						displayName = "PD2 TDR Phase";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "My_Role":
						displayName = "My Role";
						blnVisible = true;
						blnSortable = false;
						blnOrderable = false;
						break;
					case "Submitted_By":
						displayName = "Submitted By";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
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

	private void loadRequests()
	{
		DataTable dt = null;
		if (_refreshData || Session["dtWorkItemRequest"] == null)
		{
			dt = WorkRequest.WorkRequestList_Get(typeID: 0, showArchived: _showArchived, requestGroupID: 0, myData: _myData);
			HttpContext.Current.Session["dtWorkItemRequest"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtWorkItemRequest"];
		}

		if (dt != null)
		{
			this.DCC = dt.Columns;
			spanRowCount.InnerText = dt.Rows.Count.ToString();

			InitializeColumnData(ref dt);
			dt.AcceptChanges();
		}

		if (_export)
		{
			ExportExcel(dt);
		}
		else
		{
			grdRequest.DataSource = dt;
			grdRequest.DataBind();
		}
	}

	#endregion Data


	#region Grid

	void grdRequest_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridHeader(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		row.Cells[DCC.IndexOf("WORKREQUESTID")].Text = "Request #";
		row.Cells[DCC.IndexOf("SR_Count")].Text = "# SRs";
		row.Cells[DCC.IndexOf("WorkItem_Count")].Text = "# Work<br />Items";
	}

	void grdRequest_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		string requestId = row.Cells[DCC.IndexOf("WorkRequestID")].Text.Trim();
		int srCount = 0, workItemCount = 0;
		int.TryParse(row.Cells[DCC.IndexOf("SR_Count")].Text.Trim().Replace("&nbsp", "0"), out srCount);
		int.TryParse(row.Cells[DCC["WorkItem_Count"].Ordinal].Text.Trim().Replace("&nbsp", "0"), out workItemCount);

		if (CanViewRequest || CanEditRequest)
		{
			row.Cells[DCC.IndexOf("WorkRequestID")].Controls.Add(createEditLink(requestId, requestId));
		}
		if (srCount > 0 && CanViewSR || (workItemCount > 0 && CanViewWorkItem))
		{
			//buttons to show/hide child grid
			row.Cells[DCC["X"].Ordinal].Controls.Clear();
			row.Cells[DCC["X"].Ordinal].Controls.Add(createShowHideButton(true, "Show", requestId));
			row.Cells[DCC["X"].Ordinal].Controls.Add(createShowHideButton(false, "Hide", requestId));

			if (srCount > 0)
			{
				row.Cells[DCC.IndexOf("SR_Count")].Controls.Add(createSRsLink(requestId, srCount.ToString()));

				//add child grid row for SR Items
				Table table = (Table)row.Parent;
				GridViewRow childRow = createChildRow(requestId, ChildType.SR);
				table.Rows.AddAt(table.Rows.Count, childRow);
			}
			if (workItemCount > 0)
			{
				//add child grid row for Work Items
				Table table = (Table)row.Parent;
				GridViewRow childRow = createChildRow(requestId, ChildType.WorkItem);
				table.Rows.AddAt(table.Rows.Count, childRow);
			}
		}
		else
		{
			Image imgBlank = new Image();
			imgBlank.Height = 10;
			imgBlank.Width = 10;
			imgBlank.ImageUrl = "Images/Icons/blank.png";
			imgBlank.AlternateText = "";
			row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
		}

		row.Cells[DCC.IndexOf("Title")].ToolTip = row.Cells[DCC.IndexOf("Description")].Text.Replace("&nbsp;", "");

		//for (int i = 0; i < row.Cells.Count; i++)
		//{
		//	row.Cells[i].Style["vertical-align"] = "top";
		//}
	}

	void grdRequest_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdRequest.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtWorkItemRequest"] == null)
		{
			loadRequests();
		}
		else
		{
			grdRequest.DataSource = (DataTable)HttpContext.Current.Session["dtWorkItemRequest"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			row.Cells[i].Style["text-align"] = "left";
			if (i != DCC.IndexOf("X") && i != DCC.IndexOf("WORKREQUESTID") && i != DCC.IndexOf("SR_Count") && i != DCC.IndexOf("Priority"))
			{
				row.Cells[i].Style["padding-left"] = "5px";
			}
		}

		//more column formatting
		row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
		row.Cells[DCC.IndexOf("WORKREQUESTID")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("WORKREQUESTID")].Style["width"] = "63px";
		row.Cells[DCC.IndexOf("Contract")].Style["width"] = "74px";
		row.Cells[DCC.IndexOf("Organization")].Style["width"] = "100px";
		row.Cells[DCC.IndexOf("Request_Type")].Style["width"] = "53px";
		row.Cells[DCC.IndexOf("Work_Scope")].Style["width"] = "85px";
		row.Cells[DCC.IndexOf("Effort")].Style["width"] = "85px";
		//row.Cells[DCC.IndexOf("Title")].Style["width"] = "75px";
		//row.Cells[DCC.IndexOf("Description")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("SR_Count")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("SR_Count")].Style["width"] = "40px";
		row.Cells[DCC.IndexOf("WorkItem_Count")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "50px";
		row.Cells[DCC.IndexOf("Priority")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("Priority")].Style["width"] = "65px";
		row.Cells[DCC.IndexOf("SME")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("Lead_Tech_Writer")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("Lead_Resource")].Style["width"] = "75px";
		//row.Cells[DCC.IndexOf("My_Role")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("Submitted_By")].Style["width"] = "75px";
	}


	LinkButton createEditLink(string requestId = "", string requestNum = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEditRequest_click('{0}');return false;", requestId);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEditRequest_{0}", requestId);
		lb.Attributes["name"] = string.Format("lbEditRequest_{0}", requestId);
		lb.ToolTip = string.Format("Edit Work Request [{0}]", requestNum);
		lb.Text = requestNum;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}

	LinkButton createSRsLink(string requestId = "", string count = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbShowSRs_click('{0}');return false;", requestId);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbShowSRs_{0}", requestId);
		lb.Attributes["name"] = string.Format("lbShowSRs_{0}", requestId);
		lb.ToolTip = string.Format("View SRs for Request [{0}]", requestId);
		lb.Text = count;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}


	Image createShowHideButton(bool show = false, string direction = "Show", string requestId = "ALL")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("imgShowHideChildren_click(this,'{0}','{1}');", direction, requestId);

		Image img = new Image();
		img.ID = string.Format("img{0}Children_{1}", direction, requestId);
		img.Style["display"] = show ? "block" : "none";
		img.Style["cursor"] = "pointer";
		img.Attributes.Add("Name", string.Format("img{0}", direction));
		img.Attributes.Add("requestId", requestId);
		img.Height = 10;
		img.Width = 10;
		img.ImageUrl = direction.ToUpper() == "SHOW"
			? "Images/Icons/add_blue.png"
			: "Images/Icons/minus_blue.png";
		img.ToolTip = string.Format("{0} SRs/Work Items for [{1}]", direction, requestId);
		img.AlternateText = string.Format("{0} SRs/Work Items for [{1}]", direction, requestId);
		img.Attributes.Add("Onclick", sb.ToString());

		return img;
	}
	GridViewRow createChildRow(string requestId = "", ChildType childType = ChildType.WorkItem)
	{
		GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
		TableCell tableCell = null;

		try
		{
			row.CssClass = "gridBody";
			row.Style["display"] = "none";
			row.ID = string.Format("gridChild_{0}_{1}", requestId, childType.ToString());
			row.Attributes.Add("requestId", requestId);
			row.Attributes.Add("childType", childType.ToString());
			row.Attributes.Add("Name", string.Format("gridChild_{0}", requestId));

			//add the table cells
			for (int i = 0; i < DCC.Count; i++)
			{
				tableCell = new TableCell();
				tableCell.Text = "&nbsp;";

				if (i == DCC.IndexOf("X"))
				{
					//set width to match parent
					tableCell.Style["width"] = "12px";
				}
				else if (i == DCC.IndexOf("WORKREQUESTID"))
				{
					tableCell.Style["padding"] = "0px";
					tableCell.Style["vertical-align"] = "top";
					tableCell.ColumnSpan = DCC.Count - 1;
					//add the frame here
					tableCell.Controls.Add(createChildFrame(requestId: requestId, childType: childType));
				}
				else
				{
					tableCell.Style["display"] = "none";
				}

				row.Cells.Add(tableCell);
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			row = null;
		}

		return row;
	}
	HtmlIframe createChildFrame(string requestId = "", ChildType childType = ChildType.WorkItem)
	{
		HtmlIframe childFrame = new HtmlIframe();

		if (string.IsNullOrWhiteSpace(requestId))
		{
			return null;
		}

		childFrame.ID = string.Format("frameChild_{0}_{1}", requestId, childType.ToString());
		childFrame.Attributes.Add("requestId", requestId);
		childFrame.Attributes.Add("childType", childType.ToString());
		childFrame.Attributes["frameborder"] = "0";
		childFrame.Attributes["scrolling"] = "no";
		childFrame.Attributes["src"] = "javascript:''";
		childFrame.Style["height"] = "30px";
		childFrame.Style["width"] = "100%";
		childFrame.Style["border"] = "none";

		return childFrame;
	}
	

	private void RemoveExcelColumns(ref DataTable dt)
	{
		try
		{
			//this has to be done in reverse order (RemoveAt) 
			//OR by name(Remove) or it will have undesired result
			dt.Columns.Remove("X");
			dt.Columns.Remove("RequestGroupID");
			dt.Columns.Remove("RequestGroup");
			dt.Columns.Remove("CONTRACTID");
			dt.Columns.Remove("ORGANIZATIONID");
			dt.Columns.Remove("REQUESTTYPEID");
			dt.Columns.Remove("WTS_SCOPEID");
			dt.Columns.Remove("EFFORTID");
			dt.Columns.Remove("EFFORT");
			dt.Columns.Remove("DESCRIPTION");
			dt.Columns.Remove("SR_Date");
			dt.Columns.Remove("WorkItem_Date");
			dt.Columns.Remove("OP_PRIORITYID");
			dt.Columns.Remove("SMEID");
			dt.Columns.Remove("LEAD_IA_TWID");
			dt.Columns.Remove("LEAD_RESOURCEID");
			dt.Columns.Remove("SUBMITTEDBY");
			dt.Columns.Remove("ARCHIVE");

			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}
	private void RenameExcelColumns(ref DataTable dt)
	{
		if (dt.Columns.Contains("WORKREQUESTID"))
		{
			dt.Columns["WORKREQUESTID"].ColumnName = "Request #";
		}
		if (dt.Columns.Contains("CONTRACT"))
		{
			dt.Columns["CONTRACT"].ColumnName = "Contract";
		}
		if (dt.Columns.Contains("ORGANIZATION"))
		{
			dt.Columns["ORGANIZATION"].ColumnName = "Department / Organization";
		}
		if (dt.Columns.Contains("Request_Type"))
		{
			dt.Columns["Request_Type"].ColumnName = "Request Type";
		}
		if (dt.Columns.Contains("Work_Scope"))
		{
			dt.Columns["Work_Scope"].ColumnName = "Scope";
		}
		if (dt.Columns.Contains("TITLE"))
		{
			dt.Columns["TITLE"].ColumnName = "Title";
		}
		if (dt.Columns.Contains("WorkItem_Count"))
		{
			dt.Columns["WorkItem_Count"].ColumnName = "# Work Items";
		}
		if (dt.Columns.Contains("SR_Count"))
		{
			dt.Columns["SR_Count"].ColumnName = "# SRs";
		}
		if (dt.Columns.Contains("PRIORITY"))
		{
			dt.Columns["PRIORITY"].ColumnName = "Operations Priority";
		}
		if (dt.Columns.Contains("Lead_Tech_Writer"))
		{
			dt.Columns["Lead_Tech_Writer"].ColumnName = "Lead Tech Writer";
		}
		if (dt.Columns.Contains("Lead_Resource"))
		{
			dt.Columns["Lead_Resource"].ColumnName = "Lead Resource";
		}
		if (dt.Columns.Contains("My_Role"))
		{
			dt.Columns["My_Role"].ColumnName = "My Role";
		}
		if (dt.Columns.Contains("Submitted_By"))
		{
			dt.Columns["Submitted_By"].ColumnName = "Submitted By";
		}

		dt.AcceptChanges();
	}

	private bool ExportExcel(DataTable dt)
	{
		bool success = false;
		string errorMsg = string.Empty;

		try
		{
			Workbook wb = new Workbook(FileFormatType.Xlsx);
			Worksheet ws = wb.Worksheets[0];
			StyleFlag flag = new StyleFlag() { All = true };
			Aspose.Cells.Style style = new Aspose.Cells.Style();
			Aspose.Cells.Style style2 = new Aspose.Cells.Style();
			Aspose.Cells.Style style3 = new Aspose.Cells.Style();

			style.Pattern = BackgroundType.Solid;
			style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
			style2.Pattern = BackgroundType.Solid;
			style2.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightGreen");
			style3.Pattern = BackgroundType.Solid;
			style3.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightBlue");
			
			DataTable dtDrilldown = WorkloadItem.WorkItemList_Get();
			DataTable dtExcel = new DataTable();

			RemoveExcelColumns(ref dt);
			RenameExcelColumns(ref dt);
			dtExcel = dt.Clone();

			for (int i = 0; i <= dtExcel.Columns.Count - 1; i++) {
				dtExcel.Columns[i].DataType = typeof(string);
				dtExcel.Columns[i].MaxLength = 255;
				dtExcel.Columns[i].AllowDBNull = true;
			}

			foreach (DataRow dr in dt.Rows)
			{
				dtExcel.ImportRow(dr);

				if (dtDrilldown != null && dtDrilldown.Rows.Count > 0)
				{
					DataTable dtTemp = new DataTable();

					dtTemp = dtDrilldown.Copy();
					dtTemp.DefaultView.RowFilter = "WORKREQUESTID = " + dr["Request #"];
					dtTemp = dtTemp.DefaultView.ToTable();

					int count = 0;
					foreach (DataRow drTemp in dtTemp.Rows)
					{
						if (count == 0)
						{
							dtExcel.Rows.Add("", "#", "System", "Status", "Description", "Allocation Assignment", "Production", "Version", "Priority", "Primary Analyst", "Primary Developer", "Assigned", "Progress", "");
						}

						dtExcel.Rows.Add("", drTemp["ItemID"].ToString(), drTemp["Websystem"], drTemp["STATUS"], drTemp["TITLE"], drTemp["ALLOCATION"], (drTemp["Production"].ToString().Trim().ToUpper() == "TRUE" ? "Yes" : "No"), drTemp["Version"], drTemp["PRIORITY"], drTemp["Primary_Analyst"], drTemp["Primary_Developer"], drTemp["Assigned"], drTemp["Progress"].ToString(), "");

						int itemID = 0;
						int.TryParse(drTemp["ItemID"].ToString(), out itemID);
						if (itemID > 0)
						{
							DataTable dtDrilldownTask = WorkloadItem.WorkItem_GetTaskList(workItemID: itemID, showArchived: _showArchived);
							if (dtDrilldownTask != null && dtDrilldownTask.Rows.Count > 0)
							{
								int countTask = 0;
								foreach (DataRow drChild in dtDrilldownTask.Rows)
								{
									if (countTask == 0)
									{
										dtExcel.Rows.Add("", "", "Task #", "Title", "Planned Start", "Actual Start", "Planned Hours", "Actual Hours", "Actual End", "Assigned", "Status", "Progress");
									}

									dtExcel.Rows.Add("", "", drChild["WORKITEMID"].ToString() + " - " + drChild["TASK_NUMBER"].ToString(), drChild["Title"], String.Format("{0:M/d/yyyy}", drChild["ESTIMATEDSTARTDATE"].ToString()), String.Format("{0:M/d/yyyy}", drChild["ACTUALSTARTDATE"].ToString()), drChild["PLANNEDHOURS"].ToString(), drChild["ACTUALHOURS"].ToString(), String.Format("{0:M/d/yyyy}", drChild["ACTUALENDDATE"].ToString()), drChild["AssignedResource"], drChild["STATUS"], drChild["COMPLETIONPERCENT"].ToString());
									countTask++;
								}
							}
						}

						count++;
					}
				}
			}

			string name = "Work Request";
			ws.Cells.ImportDataTable(dtExcel, true, 0, 0, false, false);

			for (int j = 0; j <= ws.Cells.Rows.Count - 1; j++)
			{
				if (ws.Cells.Rows[j][0].Value == "Request #")
				{
					Range range = ws.Cells.CreateRange(j, 0, 1, 15);
					range.ApplyStyle(style, flag);
				}

				if (ws.Cells.Rows[j][1].Value == "#")
				{
					Range range = ws.Cells.CreateRange(j, 1, 1, 12);
					range.ApplyStyle(style2, flag);
				}

				if (ws.Cells.Rows[j][2].Value == "Task #")
				{
					Range range = ws.Cells.CreateRange(j, 2, 1, 10);
					range.ApplyStyle(style3, flag);
				}
			}

			//WTSUtility.FormatWorkbookHeader(ref wb, ref ws, 0, 0, 1, dt.Columns.Count);

			ws.AutoFitColumns();
			MemoryStream ms = new MemoryStream();
			wb.Save(ms, SaveFormat.Xlsx);

			Response.ContentType = "application/xlsx";
			Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
			Response.BinaryWrite(ms.ToArray());
			Response.End();

			success = true;
		}
		catch (Exception ex)
		{
			success = false;
			errorMsg += Environment.NewLine + ex.Message;
		}

		return success;
	}

	#endregion Grid

	[WebMethod(true)]
	public static bool ItemExists(int itemID, string type)
	{
		try
		{
			return Workload.ItemExists(itemID, -1, type);
		}
		catch (Exception) { return false; }
	}
}