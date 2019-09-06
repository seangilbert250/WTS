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
using System.Web.UI.HtmlControls;

public partial class MDGrid_ProductVersion_Session_Breakout : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int ProductVersionID = 0;
    protected int ReleaseSessionID = 0;
    protected string QFSystem = string.Empty;
    protected string QFAOR = string.Empty;
    protected string QFContract = string.Empty;

    protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

    protected string CurrentBreakoutType = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
		readQueryString();

        initControls();

		loadGridData();
    }

    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        String strName = "Master Grid -  Release Session Breakout";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[0];
        formatTable(ref copydt);
        printHeader(ref copydt, ws);
        printTable(ref copydt, ws);
        ws.Cells.DeleteColumn(0);
        ws.AutoFitColumns();
        wb.Save(ms, SaveFormat.Xlsx);
        Response.BufferOutput = true;
        Response.ContentType = "application/xlsx";
        Response.AddHeader("content-disposition", "attachment;  filename=" + strName + ".xlsx");
        Response.BinaryWrite(ms.ToArray());
        Response.End();
    }

    private void printTable(ref DataTable copydt, Worksheet ws)
    {
        for (int i = 0, rowCount = 1; i < copydt.Rows.Count; i++)
        {
            if (!string.IsNullOrWhiteSpace(copydt.Rows[i].Field<string>("Session")))
            {
                for (int j = 0; j < copydt.Columns.Count; j++)
                {
                    ws.Cells[rowCount, j].PutValue(copydt.Rows[i].ItemArray[j]);
                }
                rowCount++;
            }
        }
    }

    private void printHeader(ref DataTable copydt, Worksheet ws)
    {
        Aspose.Cells.Style style = new Aspose.Cells.Style();
        style.Pattern = BackgroundType.Solid;
        style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
        for (int i = 0; i < copydt.Columns.Count; i++)
        {
            ws.Cells[0, i].PutValue(copydt.Columns[i].ColumnName);
            ws.Cells[0, i].SetStyle(style);
        }
    }

    private void formatTable(ref DataTable copydt)
    {
        copydt.Columns.Remove("ReleaseSessionID");
        copydt.Columns.Remove("StartDate");
        copydt.Columns.Remove("EndDate");
        copydt.Columns.Remove("Duration");
        copydt.Columns.Remove("Sort");
        copydt.Columns.Remove("ARCHIVE");
        copydt.Columns.Remove("Z");

        copydt.Columns["Y"].ColumnName = "Breakout";
        copydt.Columns["ReleaseSession"].ColumnName = "Session";
    }

    private void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}

		if (Request.QueryString["ProductVersionID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["ProductVersionID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["ProductVersionID"].ToString()), out this.ProductVersionID);
		}

        if (Request.QueryString["ReleaseSessionID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseSessionID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ReleaseSessionID"].ToString()), out this.ReleaseSessionID);
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

        if (Request.QueryString["SelectedSystems"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSystems"]))
        {
            this.QFSystem = Request.QueryString["SelectedSystems"];
        }

        if (Request.QueryString["SelectedAORs"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAORs"]))
        {
            this.QFAOR = Request.QueryString["SelectedAORs"];
        }

        if (Request.QueryString["SelectedContracts"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedContracts"]))
        {
            this.QFContract = Request.QueryString["SelectedContracts"];
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
        dt = MasterData.ReleaseSessionBreakoutList_Get(productVersionID: ProductVersionID, releaseSessionID: ReleaseSessionID, includeArchive: true, qfSystem: QFSystem, qfContract: QFContract, qfAOR: QFAOR);

		if (dt != null)
		{
			this.DCC = dt.Columns;

			InitializeColumnData(ref dt);
            SetupMultiHeader(dt);
            dt.AcceptChanges();
		}

        if (_export && dt != null)
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
                    case "Y":
                        displayName = "Breakout";
                        blnVisible = true;
                        break;
                    case "Total Tasks":
                        displayName = "Total Work Tasks";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Percent Open":
                        displayName = "% Open Work Tasks";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Percent Closed":
                        displayName = "% Closed Work Tasks";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Total Closed":
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Closed Work Tasks";
                        break;
                    case "Carry-In (Closed)":
                        displayName = "Carry-In";
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Closed Work Tasks";
                        break;
                    case "New (Closed)":
                        displayName = "New";
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Closed Work Tasks";
                        break;
                    case "Carry-Out (Closed)":
                        displayName = "Carry-Out";
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Closed Work Tasks";
                        break;
                    case "Total Open":
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Open Work Tasks";
                        break;
                    case "Carry-In (Open)":
                        displayName = "Carry-In";
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Open Work Tasks";
                        break;
                    case "New (Open)":
                        displayName = "New";
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Open Work Tasks";
                        break;
                    case "Carry-Out (Open)":
                        displayName = "Carry-Out";
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Open Work Tasks";
                        break;
                    case "Dev (Resources)":
                        displayName = "Dev";
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Resources";
                        break;
                    case "Biz (Resources)":
                        displayName = "Biz";
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Resources";
                        break;
                    case "Total Resources":
                        blnVisible = true;
                        blnSortable = true;
                        groupName = "Resources";
                        break;
                    case "Z":
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                }

                columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
                columnData.Columns.Item(columnData.Columns.Count - 1).GroupName = groupName;
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

    protected void SetupMultiHeader(DataTable dt)
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
        secondHeaderRow.CssClass = "metricsHeader"; //"metricsSubHeader";

        for (int i = 0; i < dt.Columns.Count; i++)
        {
            iti_Tools_Sharp.DynamicHeaderCell cell = new iti_Tools_Sharp.DynamicHeaderCell();
            cell.Text = dt.Columns[i].ColumnName;
            firstHeaderRow.Cells.Add(cell);
            cell.CssClass = "metricsHeader";

            cell = new iti_Tools_Sharp.DynamicHeaderCell();
            cell.Text = dt.Columns[i].ColumnName;
            secondHeaderRow.Cells.Add(cell);
            cell.CssClass = "metricsHeader"; //"metricsSubHeader";
            cell.Style["border-top"] = "1px solid grey";
        }

        grdMD.DynamicHeader = dynamicHeader;
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

        if (CurrentBreakoutType != row.Cells[DCC.IndexOf("X")].Text) CreateRow(row);

        CurrentBreakoutType = row.Cells[DCC.IndexOf("X")].Text;
    }

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		loadGridData();
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("Total Tasks")
                && i != DCC.IndexOf("Percent Open")
                && i != DCC.IndexOf("Percent Closed")
                && i != DCC.IndexOf("Total Closed")
                && i != DCC.IndexOf("Carry-In (Closed)")
                && i != DCC.IndexOf("New (Closed)")
                && i != DCC.IndexOf("Carry-Out (Closed)")
                && i != DCC.IndexOf("Total Open")
                && i != DCC.IndexOf("Carry-In (Open)")
                && i != DCC.IndexOf("New (Open)")
                && i != DCC.IndexOf("Carry-Out (Open)")
                && i != DCC.IndexOf("Dev (Resources)")
                && i != DCC.IndexOf("Biz (Resources)")
                && i != DCC.IndexOf("Total Resources"))
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
        row.Cells[DCC.IndexOf("Y")].Style["width"] = "350px";
        row.Cells[DCC.IndexOf("Total Tasks")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Percent Open")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Percent Closed")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Total Closed")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Carry-In (Closed)")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("New (Closed)")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Carry-Out (Closed)")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Total Open")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Carry-In (Open)")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("New (Open)")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Carry-Out (Open)")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Dev (Resources)")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Biz (Resources)")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Total Resources")].Style["width"] = "55px";
    }

    private void CreateRow(GridViewRow row)
    {
        Table nTable = (Table)row.Parent;
        GridViewRow nRow = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
        TableCell nCell = new TableCell();

        nCell.Style["background"] = "#d7e8fc";
        nCell.Style["font-weight"] = "bold";
        nCell.ColumnSpan = 16;
        nCell.Text = row.Cells[DCC.IndexOf("X")].Text;

        nRow.Cells.Add(nCell);
        nTable.Rows.AddAt(nTable.Rows.Count - 1, nRow);
    }

    #endregion Grid

}