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
using Aspose.Cells;
using System.IO;

public partial class MDGrid_PDD_TDR_Status : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();
	DataTable _dtStatusType = null;

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfID = 0;

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
    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        formatParent(ref copydt);
        String strName = "Master Grid - Status";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[0];
        int rowCount = 0;
        foreach (DataRow parentRow in copydt.Rows)
        {
            if (parentRow.Field<int>("StatusID") != 0)
            {
                printParentHeader(ws, ref rowCount, copydt.Columns);
                printParent(parentRow, ws, ref rowCount);
                rowCount++;
                printChildRows(parentRow, ws, ref rowCount);
                rowCount++;
            }
        }
        ws.Cells.DeleteColumn(0);
        ws.AutoFitColumns();
        wb.Save(ms, SaveFormat.Xlsx);
        Response.BufferOutput = true;
        Response.ContentType = "application/xlsx";
        Response.AddHeader("content-disposition", "attachment;  filename=" + strName + ".xlsx");
        Response.BinaryWrite(ms.ToArray());
        Response.End();
    }

    private void printParentHeader(Worksheet ws, ref int rowCount, DataColumnCollection columns)
    {
        Aspose.Cells.Style style = new Aspose.Cells.Style();
        style.Pattern = BackgroundType.Solid;
        style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
        for (int i = 0; i < columns.Count; i++)
        {
            ws.Cells[rowCount, i].PutValue(columns[i].ColumnName);
            ws.Cells[rowCount, i].SetStyle(style);
        }
        rowCount++;
    }
    private void printChildRows(DataRow parentRow, Worksheet ws, ref int rowCount)
    {
        DataTable child = null;
        int ID = parentRow.Field<int>("StatusID");
        child = MasterData.WorkType_StatusList_Get(statusID: ID);
        int i = 0, j = 1;
        formatChild(child);
        printChildHeader(ws, ref rowCount, child.Columns);
        foreach (DataRow row in child.Rows)
        {
            j = 2;
            foreach (object value in row.ItemArray)
            {
                ws.Cells[rowCount + i, j].PutValue(value);
                j++;
            }
            i++;
        }
        rowCount += child.Rows.Count;
    }

    private void formatChild(DataTable child)
    {
        child.Columns.Remove("X");
        child.Columns.Remove("Status_WorkTypeID");
        child.Columns.Remove("WorkTypeID");
        child.Columns.Remove("WorkType_SORT_ORDER");
        child.Columns.Remove("StatusID");
        child.Columns.Remove("Status_SORT_ORDER");
        child.Columns["DESCRIPTION"].ColumnName = "Description";
        child.Columns["ARCHIVE"].ColumnName = "Archive";
        child.Columns["WorkType"].ColumnName = "Resource Group";
        child.Rows[0].Delete();
        child.AcceptChanges();
    }


    private void printChildHeader(Worksheet ws, ref int rowCount, DataColumnCollection columns)
    {
        if (object.ReferenceEquals(columns, null) || columns.Count < 1) { return; }
        Aspose.Cells.Style style = new Aspose.Cells.Style();
        style.Pattern = BackgroundType.Solid;
        style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("lightBlue");
        for (int i = 0; i < columns.Count; i++)
        {
            ws.Cells[rowCount, i + 2].PutValue(columns[i].ColumnName);
            ws.Cells[rowCount, i + 2].SetStyle(style);
        }
        rowCount++;
    }

    private void printParent(DataRow parentRow, Worksheet ws, ref int rowCount)
    {
        int i = 0;
        foreach (object value in parentRow.ItemArray)
        {
            ws.Cells[rowCount, i].PutValue(value);
            i++;
        }
        rowCount++;
    }

    private static void formatParent(ref DataTable dt)
    {
        dt.Columns.Remove("X");
        dt.Columns.Remove("CREATEDBY");
        dt.Columns.Remove("CREATEDDATE");
        dt.Columns.Remove("UPDATEDBY");
        dt.Columns.Remove("UPDATEDDATE");
        dt.Columns.Remove("StatusTypeID");
        dt.Columns.Remove("StatusType_SORT_ORDER");
        dt.Columns["DESCRIPTION"].ColumnName = "Description";
        dt.Columns["ARCHIVE"].ColumnName = "Archive";
        dt.Columns["StatusType"].ColumnName = "Status Type";
        dt.Columns["WorkItem_Count"].ColumnName = "Primary Tasks";
        dt.Columns["Task_Count"].ColumnName = "Subtasks";
        dt.Columns["WorkType_Count"].ColumnName = "Resource Groups";
        dt.Columns["SORT_ORDER"].ColumnName = "Sort Order";
    }
    private void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}
		if (Request.QueryString["QFID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["QFID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["QFID"].ToString()), out this._qfID);
		}

		if (Request.QueryString["sortOrder"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
		{
			this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
		}

        if (Request.QueryString["Export"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["Export"].ToString()) && Request.QueryString["Export"] == "1")
        {
            _export = true;
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
		_dtStatusType = MasterData.StatusTypeList_Get();

		DataTable dt = null;
		if (_refreshData || Session["dtMD_Status"] == null)
		{
			dt = MasterData.StatusList_Get(includeArchive: true);
			HttpContext.Current.Session["dtMD_Status"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtMD_Status"];
		}

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
			spanRowCount.InnerText = dt.Rows.Count.ToString();

			using (DataTable dtTemp = dt.DefaultView.ToTable(true, "STATUSTYPEID", "STATUSTYPE"))
			{
				ddlQF.DataSource = dtTemp;
				ddlQF.DataTextField = "StatusType";
				ddlQF.DataValueField = "StatusTypeID";
				ddlQF.DataBind();
			}

			ListItem item = ddlQF.Items.FindByValue(_qfID.ToString());
			if (item != null)
			{
				item.Selected = true;
			}

			InitializeColumnData(ref dt);
			dt.AcceptChanges();

			if (_qfID != 0 && dt != null && dt.Rows.Count > 0)
			{
				dt.DefaultView.RowFilter = string.Format(" StatusTypeID =  {0}", _qfID.ToString());
				dt = dt.DefaultView.ToTable();
			}
			int count = dt.Rows.Count;
			count = count > 0 ? count - 1 : count; //need to subtract the empty row
			spanRowCount.InnerText = count.ToString();
		}

        if (_export && dt != null && CanView)
        {
            exportExcel(dt);
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
					case "StatusType":
						displayName = "Status Type";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Status":
						displayName = "Status";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = true;
						break;
					case "WorkItem_Count":
						displayName = "Primary Tasks";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Task_Count":
						displayName = "Subtasks";
						blnVisible = true;
						blnSortable = true;
						break;
					case "WorkType_Count":
						displayName = "Resource Groups";
						blnVisible = true;
						blnSortable = true;
						break;
					case "SORT_ORDER":
						displayName = "Sort Order";
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
	}

	void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("StatusID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			//Status Type
			DropDownList ddl = WTSUtility.CreateGridDropdownList(_dtStatusType, "StatusType", "StatusType", "StatusTypeID", itemId, row.Cells[DCC.IndexOf("StatusTypeID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("StatusType")].Text.Replace("&nbsp;", " ").Trim(), null);
			row.Cells[DCC.IndexOf("StatusType")].Controls.Add(ddl);

			row.Cells[DCC.IndexOf("Status")].Controls.Add(WTSUtility.CreateGridTextBox("Status", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Status")].Text.Trim())));
			row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Trim())));
			row.Cells[DCC.IndexOf("WorkType_Count")].Controls.Add(createIntersectLink(itemId, row.Cells[DCC.IndexOf("WorkType_Count")].Text.Replace("&nbsp;", " ").Trim(), "WorkTypes"));
			row.Cells[DCC.IndexOf("SORT_ORDER")].Controls.Add(WTSUtility.CreateGridTextBox("Sort_Order", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SORT_ORDER")].Text.Trim()), true));
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

		string dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkItem_Count")].Text).Trim();
		int count = 0;
		if (!CanEdit
			|| !int.TryParse(dependencies, out count)
			|| count > 0)
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
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("Status")].Text.Trim()));
		}
	}

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_Status"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_Status"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("StatusID")
				&& i != DCC.IndexOf("WorkItem_Count")
				&& i != DCC.IndexOf("Task_Count")
				&& i != DCC.IndexOf("WorkType_Count")
				&& i != DCC.IndexOf("SORT_ORDER")
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
		row.Cells[DCC.IndexOf("StatusType")].Style["width"] = "250px";
		row.Cells[DCC.IndexOf("Status")].Style["width"] = "150px";
		row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "85px";
		row.Cells[DCC.IndexOf("Task_Count")].Style["width"] = "65px";
		row.Cells[DCC.IndexOf("WorkType_Count")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
	}


	LinkButton createIntersectLink(string itemId = "", string item = "", string intersectTo = "WorkTypes")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEdit{0}_click('{1}');return false;", intersectTo, itemId);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEdit{0}_{1}", intersectTo, itemId);
		lb.Attributes["name"] = string.Format("lbEdit{0}_{1}", intersectTo, itemId);
		lb.ToolTip = string.Format("Edit {0} for [{1}]", intersectTo, item);
		lb.Text = item;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
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

			int id = 0, typeId = 0, sortOrder = 0, archive = 0;
			string status = string.Empty, description = string.Empty;

			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
				typeId = 0;
				sortOrder = 0;
				status = string.Empty;
				description = string.Empty;
				archive = 0;

				tempMsg = string.Empty;
				int.TryParse(dr["StatusID"].ToString(), out id);
				int.TryParse(dr["StatusType"].ToString(), out typeId);
				status = Uri.UnescapeDataString(dr["Status"].ToString());
				description = Uri.UnescapeDataString(dr["DESCRIPTION"].ToString());
				int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
				int.TryParse(dr["ARCHIVE"].ToString(), out archive);

				if (id == 0 && string.IsNullOrWhiteSpace(status))
				{
					continue;
				}
				else if(id != 0 && string.IsNullOrWhiteSpace(status))
				{
					tempMsg = "You must specify a value for Status.";
					saved = false;
				}
				else
				{
					if (id == 0)
					{
						exists = false;
						saved = MasterData.Status_Add(typeId, status, description, sortOrder, archive == 1, out exists, out id, out tempMsg);
						if (exists)
						{
							saved = false;
							tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Status record [" + status + "].");
						}
					}
					else
					{
						saved = MasterData.Status_Update(id, typeId, status, description, sortOrder, archive == 1, out tempMsg);
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
			LogUtility.LogException(ex);
			saved = false;
			errorMsg = ex.Message;
		}

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
				deleted = MasterData.Status_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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