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

public partial class MDGrid_ProductVersion_Session : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int ProductVersionID = 0;
    protected string PlannedEndDate = "";
    protected string QFSystem = string.Empty;
    protected string QFAOR = string.Empty;
    protected string QFContract = string.Empty;

    protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

	protected bool CanView = false;
	protected bool CanEdit = false;
	protected bool IsAdmin = false;

    protected bool hasData = false;

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
        String strName = "Master Grid -  Release Session";
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
        copydt.Columns.Remove("StartDate");
        copydt.Columns.Remove("EndDate");
        copydt.Columns.Remove("Z");
        copydt.Columns.Remove("Productivity");
        copydt.Columns["ReleaseSessionID"].ColumnName = "Session ID";
        copydt.Columns["ReleaseSession"].ColumnName = "Session";
        copydt.Columns["Duration"].ColumnName = "Duration";
        copydt.Columns["ARCHIVE"].ColumnName = "Archive";
        copydt.Columns["Sort"].ColumnName = "Sort";
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

        if (Request.QueryString["PlannedEndDate"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["PlannedEndDate"]))
        {
            this.PlannedEndDate = Request.QueryString["PlannedEndDate"];
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
		dt = MasterData.ReleaseSessionList_Get(productVersionID: ProductVersionID, includeArchive: true, qfSystem: QFSystem, qfContract: QFContract, qfAOR: QFAOR);

        //If coming from Projects/Deployments check for Planned End Date Filter
        if (PlannedEndDate != "" || PlannedEndDate.Length > 0)
        {
            DateTime plannedDate = new DateTime();
            DateTime.TryParse(PlannedEndDate, out plannedDate);
            dt.DefaultView.RowFilter = "EndDate < '" + plannedDate + "'";
            dt = dt.DefaultView.ToTable();
        }

        //determine productivity from previous session
        dt.Columns.Add("Productivity");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int currentClosed = 0;
            int previousClosed = 0;

            int.TryParse(dt.Rows[i]["Total Closed"].ToString(), out currentClosed);

            if (i < dt.Rows.Count - 1 && dt.Rows[i]["ReleaseSessionID"].ToString() != "0")
            {
                int.TryParse(dt.Rows[i + 1]["Total Closed"].ToString(), out previousClosed);

                if (currentClosed > previousClosed)
                {
                    dt.Rows[i]["Productivity"] = "Up";
                }
                else if (currentClosed < previousClosed)
                {
                    dt.Rows[i]["Productivity"] = "Down";
                }
            }
        }

		if (dt != null)
		{
            if (dt.Rows.Count > 1) hasData = true;

			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			InitializeColumnData(ref dt);
            SetupMultiHeader(dt);
            dt.AcceptChanges();
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
                    case "ReleaseSessionID":
                        displayName = "Session ID";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ReleaseSession":
						displayName = "Session Name";
						blnVisible = true;
						blnSortable = true;
						break;
                    case "SessionNarrative":
                        displayName = "Narrative";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "StartDate":
						displayName = "Session Start Date";
						blnVisible = true;
						blnSortable = true;
						break;
                    case "EndDate":
                        displayName = "Session End Date";
                        blnVisible = true;
                        break;
                    case "Duration":
						displayName = "Session Duration";
						blnVisible = true;
						blnSortable = true;
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
                    case "Sort":
						displayName = "Sort Order";
						blnVisible = true;
						blnSortable = true;
						break;
					case "ARCHIVE":
						displayName = "Archive";
						blnVisible = true;
						blnSortable = true;
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

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("ReleaseSessionID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			bool chked = false;

            TextBox txtName = WTSUtility.CreateGridTextBox("ReleaseSession", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ReleaseSession")].Text.Replace("&nbsp;", " ").Trim()));
            txtName.Enabled = false;
            row.Cells[DCC.IndexOf("ReleaseSession")].Controls.Add(txtName);

            row.Cells[DCC.IndexOf("SessionNarrative")].Controls.Add(CreateLink("SessionNarrative", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SessionNarrative")].Text.Replace("&nbsp;", " ").Trim())));
            Literal narrative = new Literal();
            narrative.Text = "<span id=\"" + itemId + "\" style=\"display: none;\">" + Server.HtmlDecode(row.Cells[DCC.IndexOf("SessionNarrative")].Text.Replace("&nbsp;", " ").Trim() + "</span>");
            row.Cells[DCC.IndexOf("SessionNarrative")].Controls.Add(narrative);

            DateTime nDate = new DateTime();
            DateTime.TryParse(Server.HtmlDecode(row.Cells[DCC.IndexOf("StartDate")].Text.Replace("&nbsp;", " ").Trim()), out nDate);
            row.Cells[DCC.IndexOf("StartDate")].Controls.Add(WTSUtility.CreateGridTextBox("StartDate", itemId, Server.HtmlDecode(String.Format("{0:MM/dd/yyyy}", nDate))));

            int duration = 0;
            int.TryParse(Server.HtmlDecode(row.Cells[DCC.IndexOf("Duration")].Text.Replace("&nbsp;", " ").Trim()), out duration);

            nDate = nDate.AddDays(duration);
            row.Cells[DCC.IndexOf("EndDate")].Controls.Add(WTSUtility.CreateGridTextBox("EndDate", itemId, Server.HtmlDecode(String.Format("{0:MM/dd/yyyy}", nDate))));

            TextBox txtDuration = WTSUtility.CreateGridTextBox("Duration", itemId, duration.ToString(), true);
            txtDuration.Attributes.Add("maxlength", "4");
            row.Cells[DCC.IndexOf("Duration")].Controls.Add(txtDuration);

            row.Cells[DCC.IndexOf("Total Closed")].Controls.Add(CreateLink("Total Closed", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Total Closed")].Text.Replace("&nbsp;", " ").Trim()), Server.HtmlDecode(row.Cells[DCC.IndexOf("ReleaseSession")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("Total Open")].Controls.Add(CreateLink("Total Open", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Total Open")].Text.Replace("&nbsp;", " ").Trim()), Server.HtmlDecode(row.Cells[DCC.IndexOf("ReleaseSession")].Text.Replace("&nbsp;", " ").Trim())));

            row.Cells[DCC.IndexOf("Sort")].Controls.Add(WTSUtility.CreateGridTextBox("Sort", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Sort")].Text.Replace("&nbsp;", " ").Trim()), true));
            chked = false;
			if (row.Cells[DCC.IndexOf("ARCHIVE")].HasControls()
				&& row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0] is CheckBox)
			{
				chked = ((CheckBox)row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0]).Checked;
			}
			else if (row.Cells[DCC.IndexOf("ARCHIVE")].Text == "1")
			{
				chked = true;
			}
			row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Clear();
			row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Add(WTSUtility.CreateGridCheckBox("Archive", itemId, chked));
        }

        if (row.Cells[DCC.IndexOf("Productivity")].Text == "Up")
        {
            HtmlGenericControl divUp = new HtmlGenericControl();
            divUp.InnerHtml = "<div class=\"upArrow\"></div>";
            row.Cells[DCC.IndexOf("Total Closed")].Controls.Add(divUp);
        }
        else if (row.Cells[DCC.IndexOf("Productivity")].Text == "Down")
        {
            HtmlGenericControl divDown = new HtmlGenericControl();
            divDown.InnerHtml = "<div class=\"downArrow\"></div>";
            row.Cells[DCC.IndexOf("Total Closed")].Controls.Add(divDown);
        }

        if (!string.IsNullOrEmpty(itemId) && itemId != "0")
        {
            //add expand/collapse buttons
            HtmlGenericControl divChildren = new HtmlGenericControl();
            divChildren.Style["display"] = "table-row";
            divChildren.Style["text-align"] = "right";
            HtmlGenericControl divChildrenButtons = new HtmlGenericControl();
            divChildrenButtons.Style["display"] = "table-cell";
            divChildrenButtons.Controls.Add(createShowHideButton(true, "Show", itemId));
            divChildrenButtons.Controls.Add(createShowHideButton(false, "Hide", itemId));
            HtmlGenericControl divChildCount = new HtmlGenericControl();
            //divChildCount.InnerText = string.Format("({0})", row.Cells[DCC.IndexOf("SessionBreakout_Count")].Text);
            divChildCount.Style["display"] = "table-cell";
            divChildCount.Style["padding-left"] = "2px";
            divChildren.Controls.Add(divChildrenButtons);
            divChildren.Controls.Add(divChildCount);
            //buttons to show/hide child grid
            row.Cells[DCC["X"].Ordinal].Controls.Clear();
            row.Cells[DCC["X"].Ordinal].Controls.Add(divChildren);

            //add child grid row for Task Items
            Table table = (Table)row.Parent;
            GridViewRow childRow = createChildRow(itemId);
            table.Rows.AddAt(table.Rows.Count, childRow);
        }

        //add Total row to top of grid (as part of header to keep fixed)
        if (row.RowIndex == 0 && hasData)
        {
            FixedTotalRow(row);
        }
    }

    private void FixedTotalRow(GridViewRow row)
    {
        try
        {
            DataTable dt = grdMD.DataSource;
            // Add total row for each page
            //total row is the first row in datatable.Using datatable instead of grid row because if I use grid row it won't be visible on paging.
            if (dt.Rows[0]["ReleaseSessionID"].ToString() != "0")
            {
                return;
            }

            GridViewRow nRow = new GridViewRow(row.RowIndex, row.RowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);
            TableHeaderCell nCell = new TableHeaderCell();
            nRow.Style["height"] = "25px";
            nRow.Attributes.Add("rowID", "total");
            int intColspan = 6;
            nCell = new TableHeaderCell();
            nCell.Text = "AVERAGES";
            nCell.ColumnSpan = intColspan;
            nCell.Style["background"] = "#d7e8fc";
            nRow.Cells.Add(nCell);

            int columnCount = 0;

            for (int i = 0; i <= DCC.Count - 1; i++)
            {
                string columnDBName = DCC[i].ColumnName.ToString();
                Boolean visible = DCC[i].ColumnName == "Productivity" ? false : true;

                if (visible)
                {
                    columnCount = columnCount + 1;
                }

                if (columnCount > intColspan && visible)
                {
                    nCell = new TableHeaderCell();
                    nCell.Text = DCC[i].ColumnName == "Sort" || DCC[i].ColumnName == "ARCHIVE" || Convert.IsDBNull(dt.Rows[0][columnDBName].ToString()) ? "&nbsp;" : dt.Rows[0][columnDBName].ToString();
                    nCell.Style["background"] = "#d7e8fc";
                    nRow.Cells.Add(nCell);
                }
            }

            grdMD.Controls[1].Controls[1].Controls[0].Controls[0].Controls.AddAt(2, nRow);
        }
        catch (Exception ex)
        {
        }
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
			if (i != DCC.IndexOf("ReleaseSession")
				&& i != DCC.IndexOf("SessionNarrative")
				&& i != DCC.IndexOf("StartDate")
                && i != DCC.IndexOf("EndDate")
                && i != DCC.IndexOf("Duration")
                && i != DCC.IndexOf("Total Tasks")
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
                && i != DCC.IndexOf("Total Resources")
                && i != DCC.IndexOf("Sort")
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
		row.Cells[DCC.IndexOf("ReleaseSessionID")].Style["width"] = "65px";
		row.Cells[DCC.IndexOf("ReleaseSession")].Style["width"] = "215px";
		row.Cells[DCC.IndexOf("SessionNarrative")].Style["width"] = "120px";
        row.Cells[DCC.IndexOf("StartDate")].Style["width"] = "70px";
        row.Cells[DCC.IndexOf("EndDate")].Style["width"] = "70px";
        row.Cells[DCC.IndexOf("Duration")].Style["width"] = "55px";
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
        row.Cells[DCC.IndexOf("Sort")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
    }

    Image createShowHideButton(bool show = false, string direction = "Show", string itemId = "ALL")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgShowHideChildren_click(this,'{0}','{1}');", direction, itemId);

        Image img = new Image();
        img.ID = string.Format("img{0}Children_{1}", direction, itemId);
        img.Style["display"] = show ? "block" : "none";
        img.Style["cursor"] = "pointer";
        img.Attributes.Add("Name", string.Format("img{0}", direction));
        img.Attributes.Add("itemId", itemId);
        img.Height = 10;
        img.Width = 10;
        img.ImageUrl = direction.ToUpper() == "SHOW"
            ? "Images/Icons/add_blue.png"
            : "Images/Icons/minus_blue.png";
        img.ToolTip = string.Format("{0} Items", direction);
        img.AlternateText = string.Format("{0} Items", direction);
        img.Attributes.Add("Onclick", sb.ToString());

        return img;
    }

    GridViewRow createChildRow(string itemId = "")
    {
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
        TableCell tableCell = null;

        try
        {
            row.CssClass = "gridBody";
            row.Style["display"] = "none";
            row.ID = string.Format("gridChild_{0}", itemId);
            row.Attributes.Add("ReleaseSessionID", itemId);
            row.Attributes.Add("Name", string.Format("gridChild_{0}", itemId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC["X"].Ordinal)
                {
                    //set width to match parent
                    tableCell.Style["width"] = "15px";
                    tableCell.Style["border-right"] = "1px solid transparent";
                }
                else if (i == DCC["ReleaseSessionID"].Ordinal)
                {
                    tableCell.Style["padding-top"] = "10px";
                    tableCell.Style["padding-right"] = "10px";
                    tableCell.Style["padding-bottom"] = "0px";
                    tableCell.Style["padding-left"] = "0px";
                    tableCell.Style["vertical-align"] = "top";
                    tableCell.ColumnSpan = DCC.Count - 1;
                    //add the frame here
                    tableCell.Controls.Add(createChildFrame(itemId: itemId));
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

    HtmlIframe createChildFrame(string itemId = "")
    {
        HtmlIframe childFrame = new HtmlIframe();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        childFrame.ID = string.Format("frameChild_{0}", itemId);
        childFrame.Attributes.Add("ReleaseSessionID", itemId);
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["height"] = "30px";
        childFrame.Style["width"] = "100%";
        childFrame.Style["border"] = "none";

        return childFrame;
    }

    DateTime AddBusinessDays(DateTime startDate, int days)
    {
        if (days == 0) return startDate;
        if (startDate.DayOfWeek == DayOfWeek.Saturday)
        {
            startDate = startDate.AddDays(2);
            days -= 1;
        } else if (startDate.DayOfWeek == DayOfWeek.Saturday)
        {
            startDate = startDate.AddDays(1);
            days -= 1;
        }

        startDate = startDate.AddDays(days / 5 * 7);
        int extraDays = days % 5;

        if ((int)startDate.DayOfWeek + extraDays > 5)
        {
            extraDays += 2;
        }

        return startDate.AddDays(extraDays);
    }

    private LinkButton CreateLink(string type, string id, string value, string value2 = "")
    {
        LinkButton lb = new LinkButton();

        if (type == "SessionNarrative" && value.Length == 0) lb.Text = "Add Narrative";
        else if (value.Length > 15) lb.Text = value.Substring(0, 12) + "...";
        else lb.Text = value;

        switch (type)
        {
            case "SessionNarrative":
                lb.Attributes["onclick"] = string.Format("openNarrative('{0}'); return false;", id);
                break;
            case "Total Closed":
            case "Total Open":
                lb.Attributes["onclick"] = string.Format("openSessionWorkTasks('{0}','{1}','{2}'); return false;", id, value2, type);
                break;
        }

        return lb;
    }
    #endregion Grid


    [WebMethod(true)]
	public static string SaveChanges(int productVersionID, string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
		bool exists = false, saved = false;
		string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

		try
		{
			DataTable dtjsonReverse = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjsonReverse.Rows.Count == 0)
			{
				errorMsg = "Unable to save. An invalid list of changes was provided.";
				saved = false;
			}

            DataTable dtjson = dtjsonReverse.Clone();
            // Flip table so that the session IDs match with the dates when using the session wizard
            for (int i = dtjsonReverse.Rows.Count - 1; i >= 0; i--)
            {
                dtjson.ImportRow(dtjsonReverse.Rows[i]);
            }

			int id = 0, duration = 0, sort = 0, archive = 0;
			string releaseSession = string.Empty, sessionNarrative = string.Empty, startDate = string.Empty;

			HttpServerUtility server = HttpContext.Current.Server;
			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
                duration = 0;
                sort = 0;
                releaseSession = string.Empty;
                sessionNarrative = string.Empty;
                startDate = string.Empty;
                archive = 0;

				tempMsg = string.Empty;
				int.TryParse(dr["ReleaseSessionID"].ToString(), out id);
                releaseSession = server.UrlDecode(dr["ReleaseSession"].ToString());
                sessionNarrative = server.UrlDecode(dr["SessionNarrative"].ToString());
                startDate = server.UrlDecode(dr["StartDate"].ToString());
                int.TryParse(dr["Duration"].ToString(), out duration);
				int.TryParse(dr["Sort"].ToString(), out sort);
				int.TryParse(dr["ARCHIVE"].ToString(), out archive);

				if (string.IsNullOrWhiteSpace(releaseSession))
				{
                    tempMsg = "You must specify a Session Name.";
                    saved = false;
                }
				else
				{
					if (id == 0)
					{
						exists = false;
						saved = MasterData.ReleaseSession_Add(releaseSession, sessionNarrative, productVersionID, startDate, duration, sort, (archive == 1), out exists, out id, out tempMsg);
						if (exists)
						{
							saved = false;
							tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Session record [" + releaseSession + "].");
						}
					}
					else
					{
                        saved = MasterData.ReleaseSession_Update(id, releaseSession, sessionNarrative, productVersionID, startDate, duration, sort, (archive == 1), out tempMsg);
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
	public static string DeleteItem(int itemId)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "id", itemId.ToString() }
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
				deleted = MasterData.ReleaseSession_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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