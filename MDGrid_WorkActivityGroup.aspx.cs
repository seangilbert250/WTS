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

public partial class MDGrid_WorkActivityGroup : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

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
        String strName = "Master Grid - PD2TDR Phase";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[0];
        int rowCount = 0;
        foreach (DataRow parentRow in copydt.Rows)
        {
            if (parentRow.Field<int>("PDDTDR_PhaseID") != 0)
            {
                if (rowCount == 0) printParentHeader(ws, ref rowCount, copydt.Columns);
                printParent(parentRow, ws, ref rowCount);
                if (rowCount == 0) rowCount++;
                //printChildRows(parentRow, ws, ref rowCount);
                //rowCount++;
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
        int ID = parentRow.Field<int>("PDDTDR_PhaseID");
        child = MasterData.WorkType_PhaseList_Get(phaseID: ID);
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
        child.Columns.Remove("PDDTDR_PHASEID");
        child.Columns.Remove("Phase_SORT_ORDER");
        child.Columns.Remove("WorkType_PHASEID");
        child.Columns.Remove("WorkTypeID");
        child.Columns.Remove("WorkType_SORT_ORDER");
        child.Columns["DESCRIPTION"].ColumnName = "Description";
        child.Columns["ARCHIVE"].ColumnName = "Archive";
        child.Columns["PDDTDR_PHASE"].ColumnName = "PD2TDR Phase";
        child.Columns["WorkType"].ColumnName = "Work Activity";
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
        dt.Columns.Remove("WorkType_Count");
        dt.Columns.Remove("WorkItem_Count");
        dt.Columns["DESCRIPTION"].ColumnName = "Description";
        dt.Columns["ARCHIVE"].ColumnName = "Archive";
        dt.Columns["PDDTDR_PHASE"].ColumnName = "PD2TDR Phase";
        //dt.Columns["WorkType_Count"].ColumnName = "Work Activities";
        //dt.Columns["WorkItem_Count"].ColumnName = "Work Items";
        dt.Columns["SORT_ORDER"].ColumnName = "Sort Order";
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
		DataTable dt = null;
		if (_refreshData || Session["dtMD_WorkActivityGroup"] == null)
		{
			dt = MasterData.WorkActivityGroupList_Get(includeArchive: true);
			HttpContext.Current.Session["dtMD_WorkActivityGroup"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtMD_WorkActivityGroup"];
		}

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
			
			InitializeColumnData(ref dt);
			dt.AcceptChanges();

			int count = dt.Rows.Count;
			count = count > 0 ? count - 1 : count; //need to subtract the empty row
			spanRowCount.InnerText = count.ToString();
		}

        //if (_export && dt != null && CanView)
        //{
        //    exportExcel(dt);
        //}

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
					case "WorkActivityGroup":
						displayName = "Work Activity Group";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = true;
						break;
					case "WorkActivity_Count":
						displayName = "Work Activities";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Sort_Order":
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
		string itemId = row.Cells[DCC.IndexOf("WorkActivityGroupID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			row.Cells[DCC.IndexOf("WorkActivityGroup")].Controls.Add(WTSUtility.CreateGridTextBox("WorkActivityGroup", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkActivityGroup")].Text.Trim())));
			row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Trim())));
			row.Cells[DCC.IndexOf("Sort_Order")].Controls.Add(WTSUtility.CreateGridTextBox("Sort_Order", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Sort_Order")].Text.Trim()), true));
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

		string dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkActivity_Count")].Text).Trim();
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
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WorkActivityGroup")].Text.Trim()));
		}
	}

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_WorkActivityGroup"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkActivityGroup"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("WorkActivityGroupID")
				&& i != DCC.IndexOf("WorkActivityGroup")
				&& i != DCC.IndexOf("DESCRIPTION")
				&& i != DCC.IndexOf("WorkActivity_Count")
                && i != DCC.IndexOf("Sort_Order")
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
		row.Cells[DCC.IndexOf("WorkActivityGroup")].Style["width"] = "150px";
		row.Cells[DCC.IndexOf("WorkActivity_Count")].Style["width"] = "100px";
		row.Cells[DCC.IndexOf("Sort_Order")].Style["width"] = "75px";
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

			int id = 0, sortOrder = 0, archive = 0;
			string WorkActivityGroup = string.Empty, description = string.Empty;

			HttpServerUtility server = HttpContext.Current.Server;
			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
				sortOrder = 0;
                WorkActivityGroup = string.Empty;
				description = string.Empty;
				archive = 0;

				tempMsg = string.Empty;
				int.TryParse(dr["WorkActivityGroupID"].ToString(), out id);
                WorkActivityGroup = server.UrlDecode(dr["WorkActivityGroup"].ToString());
				description = server.UrlDecode(dr["DESCRIPTION"].ToString());
				int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
				int.TryParse(dr["ARCHIVE"].ToString(), out archive);

				if (string.IsNullOrWhiteSpace(WorkActivityGroup))
				{
					tempMsg = "You must specify a value for Work Activity Group.";
					saved = false;
				}
				else
				{
					if (id == 0)
					{
						exists = false;
						saved = MasterData.WorkActivityGroup_Add(WorkActivityGroup, description, sortOrder, archive == 1, out exists, out id, out tempMsg);
						if (exists)
						{
							saved = false;
							tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Work Activity Group record [" + WorkActivityGroup + "].");
						}
					}
					else
					{
						saved = MasterData.WorkActivityGroup_Update(id, WorkActivityGroup, description, sortOrder, archive == 1, out tempMsg);
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
				deleted = MasterData.WorkActivityGroup_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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