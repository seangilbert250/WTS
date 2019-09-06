﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;


public partial class MDGrid_System : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();
    DataTable _dtUser = null;
    DataTable _dtContract = null;

    protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfSystemID = 0;
    protected int _qfSuiteID = 0;
    protected int _qfReleaseID = 0;
    protected int PageSize = 3;

    protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;
    protected bool ColumnOrderChanged = false;

    protected bool CanView = false;
	protected bool CanEdit = false;
	protected bool IsAdmin = false;

    protected string cvValue = "0";

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
		if (Request.QueryString["SystemID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["SystemID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["SystemID"].ToString()), out this._qfSystemID);
		}
        if (Request.QueryString["SuiteID"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["SuiteID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["SuiteID"].ToString()), out this._qfSuiteID);
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

        if (!string.IsNullOrWhiteSpace(Request.QueryString["ChildView"])) {
            switch (Request.QueryString["ChildView"])
            {
                case "0":
                    cvValue = "0";
                    break;
                case "1":
                    cvValue = "1";
                    break;
                case "2":
                    cvValue = "2";
                    break;
                case "3":
                    cvValue = "3";
                    break;
                default:
                    cvValue = "0";
                    break;
            }
        }

        ddlChildView.Items.FindByValue(cvValue).Selected = true;

        if (Request.QueryString["ReleaseID"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ReleaseID"].ToString()), out this._qfReleaseID);
        }

        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }
        if (Request.QueryString["columnOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["columnOrder"].ToString()))
        {
            ColumnOrder = Server.UrlDecode(Request.QueryString["columnOrder"]);
        }
        if (Request.QueryString["columnOrderChanged"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["columnOrderChanged"].ToString()))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["columnOrderChanged"]), out this.ColumnOrderChanged);
        }

        if (Request.QueryString["PageSize"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["PageSize"]))
        {
            int.TryParse(Request.QueryString["PageSize"], out this.PageSize);
        }
    }

	private void initControls()
	{
        if (!Page.IsPostBack)
        {
            DataTable dtRelease = MasterData.ProductVersionList_Get(includeArchive: false);
            DataTable dtCurrentRelease = AOR.AORCurrentRelease_Get();

            dtRelease.DefaultView.RowFilter = "ProductVersion <> ''";
            dtRelease = dtRelease.DefaultView.ToTable();

            ddlRelease.DataSource = dtRelease;
            ddlRelease.DataValueField = "ProductVersionID";
            ddlRelease.DataTextField = "ProductVersion";
            ddlRelease.DataBind();

            if (this._qfReleaseID > 0)
            {
                ddlRelease.SelectedValue = this._qfReleaseID.ToString();
            }
            else
            {
                if (dtCurrentRelease != null && dtCurrentRelease.Rows.Count > 0) ddlRelease.SelectedValue = dtCurrentRelease.Rows[0]["ProductVersionID"].ToString();
            }
        }

		grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
		grdMD.GridRowDataBound += grdMD_GridRowDataBound;
		grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;

        ddlPageSize.SelectedValue = this.PageSize.ToString();
        int pageSizeAmount = 50;
        int.TryParse(ddlPageSize.SelectedItem.ToString(), out pageSizeAmount);
        grdMD.PageSize = pageSizeAmount;
    }


    private void loadGridData()
	{
        _dtUser = MasterData.WTS_Resource_Get(includeArchive: false);
        DataRow dr = _dtUser.NewRow();
        dr["WTS_ResourceID"] = 0;
        dr["USERNAME"] = "";
        _dtUser.Rows.InsertAt(dr, 0);

        _dtContract = MasterData.ContractList_Get(includeArchive: false);

        DataTable dt = null, dt_SuiteList = null;
		if (_refreshData || Session["dtMD_System"] == null)
		{
            int releaseID = 0;
            int.TryParse(ddlRelease.SelectedValue.ToString(), out releaseID);
			dt = MasterData.SystemList_Get(includeArchive: true, cv: cvValue, ProductVersionID: releaseID);
			HttpContext.Current.Session["dtMD_System"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtMD_System"];
		}

        if (_refreshData || Session["dtMD_System_SuiteList"] == null)
        {

            dt_SuiteList = MasterData.SystemSuiteList_Get();
            HttpContext.Current.Session["dtMD_System_SuiteList"] = dt_SuiteList;
        }
        else
        {
            dt_SuiteList = (DataTable)HttpContext.Current.Session["dtMD_System_SuiteList"];
        }


        if (dt != null)
        {
            spanRowCount.InnerText = dt.Rows.Count.ToString();

            ddlQF_System.DataSource = dt;
            ddlQF_System.DataTextField = "WTS_System";
            ddlQF_System.DataValueField = "WTS_SystemID";
            ddlQF_System.DataBind();

            ListItem item = ddlQF_System.Items.FindByValue(_qfSystemID.ToString());
            if (item != null)
            {
                item.Selected = true;
            }

            if (dt.Rows.Count > 0)
            {
                string filterSystem = !String.IsNullOrEmpty(_qfSystemID.ToString()) && _qfSystemID > 0 ? string.Format(" WTS_SystemID =  {0}", _qfSystemID.ToString()) : "";
                string filterSuite = !String.IsNullOrEmpty(_qfSuiteID.ToString()) && _qfSuiteID > 0 ? string.Format(" WTS_SystemSuiteID =  {0}", _qfSuiteID.ToString()) : "";
                string filter = string.Empty;

                if (filterSystem.Length > 0 && filterSuite.Length >0)
                {
                    filter = filterSystem + " AND " + filterSuite;
                }
                else
                {
                    filter = filterSystem + filterSuite;
                }
                 
                dt.DefaultView.RowFilter = filter;
                dt = dt.DefaultView.ToTable();
            }
            int count = dt.Rows.Count;
            count = count > 0 ? count - 1 : count; //need to subtract the empty row
            spanRowCount.InnerText = count.ToString();

            InitializeColumnData(ref dt);
            dt.AcceptChanges();

            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
        }

        if (dt_SuiteList != null)
        {
            ddlQF_Suite.DataSource = dt_SuiteList;
            ddlQF_Suite.DataTextField = "WTS_SYSTEM_SUITE";
            ddlQF_Suite.DataValueField = "WTS_SYSTEM_SUITEID";
            ddlQF_Suite.DataBind();

            ListItem item = ddlQF_Suite.Items.FindByValue(_qfSuiteID.ToString());

            if (item != null)
            {
                item.Selected = true;
            }
        }

		if (_export)
		{
			ExportExcel(dt);
		}
		else
		{
			grdMD.DataSource = dt;
			grdMD.DataBind();
		}
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
				blnOrderable = true;
				groupName = string.Empty;

				switch (gridColumn.ColumnName)
				{
					case "A":
                        displayName = ddlChildView.SelectedItem.ToString() + "s";
						blnVisible = true;
                        blnOrderable = false;
                        break;
					case "X":
                        displayName = "Delete";
						blnVisible = true;
                        blnOrderable = false;
                        break;
                    case "WTS_SystemSuite":
                        displayName = "Suite";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "WTS_SYSTEM":
						displayName = "System(Task)";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = true;
						break;
                    case "CONTRACT":
                        displayName = "Contract";
                        blnVisible = true;
                        break;
                    case "BusWorkloadManager":
                        displayName = "Bus Workload Manager";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DevWorkloadManager":
                        displayName = "Dev Workload Manager";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "WorkArea_Count":
                        displayName = "Work Areas";
                        if (cvValue == "0")
                        {
                            blnVisible = true;
                            blnSortable = true;
                        }
                        break;
                    case "Work Area Added":
                        displayName = "Last Work Area Added";
                        if (cvValue == "0")
                        {
                            blnVisible = true;
                            blnSortable = true;
                        }
                        break;
                    case "Work Area Review":
                        displayName = "Work Areas Reviewed";
                        if (cvValue == "0")
                        {
                            blnVisible = true;
                            blnSortable = true;
                        }
                        break;
                    case "WorkItem_Count":
						displayName = "Primary Tasks";
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
                columnData.Columns.Item(columnData.Columns.Count - 1).Viewable = blnVisible;
            }

            //Initialize the columnData
            columnData.Initialize(ref dt, ";", "~", "|");

			//Get sortable columns and default column order
			SortableColumns = columnData.SortableColumnsToString();
			DefaultColumnOrder = columnData.DefaultColumnOrderToString();
			//Sort and Reorder Columns
			columnData.ReorderDataTable(ref dt, ColumnOrder);

            SortOrder = Workload.GetSortValuesFromDB(1, "MDGRID_SYSTEM.ASPX");

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
		row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
	}

	void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("WTS_SystemID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			row.Cells[DCC.IndexOf("WTS_System")].Controls.Add(WTSUtility.CreateGridTextBox("WTS_System", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("WTS_System")].Text.Trim())));
            row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Trim())));
			row.Cells[DCC.IndexOf("SORT_ORDER")].Controls.Add(WTSUtility.CreateGridTextBox("Sort_Order", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SORT_ORDER")].Text.Trim()), true));

            if (row.Cells[DCC.IndexOf("WTS_SystemID")].Text == "81")
            {
                row.Cells[DCC.IndexOf("CONTRACT")].Controls.Add(CreateLink("Contract", row.Cells[DCC.IndexOf("WTS_SystemID")].Text));
            }
            else
            {
                DropDownList ddlContract = WTSUtility.CreateGridDropdownList(_dtContract, "Contract", "Contract", "ContractID", itemId, row.Cells[DCC.IndexOf("ContractID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("Contract")].Text.Replace("&nbsp;", " ").Trim(), null);
                row.Cells[DCC.IndexOf("Contract")].Controls.Add(ddlContract);
            }
            if (DCC.IndexOf("Work Area Review") > 0) row.Cells[DCC.IndexOf("Work Area Review")].Controls.Add(new LiteralControl(row.Cells[DCC.IndexOf("Work Area Review")].Text + " "));
            if (DCC.IndexOf("Work Area Review") > 0) row.Cells[DCC.IndexOf("Work Area Review")].Controls.Add(CreateLink("Review", row.Cells[DCC.IndexOf("WTS_SystemID")].Text));

            DropDownList ddlBus = WTSUtility.CreateGridDropdownList(_dtUser, "BusWorkloadManager", "USERNAME", "WTS_ResourceID", itemId, row.Cells[DCC.IndexOf("BusWorkloadManagerID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("BusWorkloadManager")].Text.Replace("&nbsp;", " ").Trim(), null);
            row.Cells[DCC.IndexOf("BusWorkloadManager")].Controls.Add(ddlBus);

            DropDownList ddlDev = WTSUtility.CreateGridDropdownList(_dtUser, "DevWorkloadManager", "USERNAME", "WTS_ResourceID", itemId, row.Cells[DCC.IndexOf("DevWorkloadManagerID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DevWorkloadManager")].Text.Replace("&nbsp;", " ").Trim(), null);
            row.Cells[DCC.IndexOf("DevWorkloadManager")].Controls.Add(ddlDev);

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
		int count = 0, waCount = 0;
		int.TryParse(dependencies, out count);
		string workAreas = Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkArea_Count")].Text).Trim();
		int.TryParse(workAreas, out waCount);

		if (!CanEdit
			|| count > 0
			|| waCount > 0)
		{
			//don't allow delete
			Image imgBlank = new Image();
			imgBlank.Height = 10;
			imgBlank.Width = 10;
			imgBlank.ImageUrl = "Images/Icons/blank.png";
			imgBlank.AlternateText = "";
			row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
		}
		else
		{
			//add delete button
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WTS_System")].Text.Trim()));
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
			divChildCount.InnerText = string.Format("({0})", waCount.ToString());
			divChildCount.Style["display"] = "table-cell";
			divChildCount.Style["padding-left"] = "2px";
			divChildren.Controls.Add(divChildrenButtons);
			divChildren.Controls.Add(divChildCount);
			//buttons to show/hide child grid
			row.Cells[DCC["A"].Ordinal].Controls.Clear();
			row.Cells[DCC["A"].Ordinal].Controls.Add(divChildren);

			//add child grid row for Task Items
			Table table = (Table)row.Parent;
			GridViewRow childRow = createChildRow(itemId);
			table.Rows.AddAt(table.Rows.Count, childRow);
		}
	}

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_System"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_System"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("WTS_SystemID")
				&& i != DCC.IndexOf("WorkArea_Count")
                && i != DCC.IndexOf("Work Area Added")
                && i != DCC.IndexOf("Work Area Review")
                && i != DCC.IndexOf("WorkItem_Count")
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
		row.Cells[DCC.IndexOf("A")].Style["width"] = "32px";
		row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("WTS_SystemSuite")].Style["width"] = "250px";
        row.Cells[DCC.IndexOf("WTS_System")].Style["width"] = "250px";
        row.Cells[DCC.IndexOf("Contract")].Style["width"] = "160px";
        if (cvValue == "0")
        {
            row.Cells[DCC.IndexOf("WorkArea_Count")].Style["width"] = "45px";
            row.Cells[DCC.IndexOf("Work Area Added")].Style["width"] = "80px";
            row.Cells[DCC.IndexOf("Work Area Review")].Style["width"] = "80px";
        } else
        {
            row.Cells[DCC.IndexOf("WorkArea_Count")].Style["display"] = "none";
        }
        row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "85px";
		row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("BusWorkloadManager")].Style["width"] = "150px";
        row.Cells[DCC.IndexOf("DevWorkloadManager")].Style["width"] = "150px";
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
		img.ToolTip = string.Format("{0} Items for [{1}]", direction, itemId);
		img.AlternateText = string.Format("{0} Items for [{1}]", direction, itemId);
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
			row.Attributes.Add("systemId", itemId);
			row.Attributes.Add("Name", string.Format("gridChild_{0}", itemId));

			//add the table cells
			for (int i = 0; i < DCC.Count; i++)
			{
				tableCell = new TableCell();
				tableCell.Text = "&nbsp;";

				if (i == DCC["A"].Ordinal)
				{
					//set width to match parent
					tableCell.Style["width"] = "32px";
					tableCell.Style["border-right"] = "1px solid transparent";
				}
				else if (i == DCC["WTS_SYSTEM"].Ordinal)
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
		childFrame.Attributes.Add("systemId", itemId);
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
			GridColsCollection cols = columnData.VisibleColumns();
			DataColumn col = null;

			//this has to be done in reverse order (RemoveAt) 
			//OR by name(Remove) or it will have undesired result
			for (int i = dt.Columns.Count - 1; i >= 0; i--)
			{
				col = dt.Columns[i];
				if (cols.ItemByColumnName(col.ColumnName) == null && col.ColumnName != "WTS_SystemID" && col.ColumnName.Trim() != "")
				{
					dt.Columns.Remove(col);
				}
			}

			if (dt.Columns.Contains("A")) dt.Columns.Remove("A");
			if (dt.Columns.Contains("X")) dt.Columns.Remove("X");

			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}
	private void RenameExcelColumns(ref DataTable dt)
	{
		GridColsCollection cols = columnData.VisibleColumns();
		GridColsColumn gcCol = null;
		DataColumn col = null;

		for (int i = dt.Columns.Count - 1; i >= 0; i--)
		{
			col = dt.Columns[i];

			gcCol = cols.ItemByColumnName(col.ColumnName);
			if (gcCol != null)
			{
				dt.Columns[col.ColumnName].ColumnName = gcCol.DisplayName;
			}
		}

		dt.AcceptChanges();
	}
	private void FormatExcelRows(ref DataTable dt)
	{
		GridColsCollection cols = columnData.VisibleColumns();

		bool hasArchive = false;
		hasArchive = (cols.ItemByColumnName("ARCHIVE") != null);

		if (hasArchive)
		{
			dt.Columns.Add("ARCHIVE_NEW");
		}

		int flag = 0;
		if (hasArchive)
		{
			foreach (DataRow row in dt.Rows)
			{
				if (hasArchive)
				{
					if (int.TryParse(row["ARCHIVE"].ToString(), out flag) && flag == 1)
					{
						row["ARCHIVE_NEW"] = "Yes";
					}
					else
					{
						row["ARCHIVE_NEW"] = "No";
					}
				}
			}

			dt.Columns["ARCHIVE_NEW"].SetOrdinal(dt.Columns["ARCHIVE"].Ordinal);
			dt.Columns.Remove("ARCHIVE");
			dt.Columns["ARCHIVE_NEW"].ColumnName = "ARCHIVE";
		}

		dt.Columns.Add(" ");
		dt.Columns.Add("  ");
		if (dt.Columns.Contains("WTS_SystemID")) dt.Columns["WTS_SystemID"].SetOrdinal(dt.Columns.Count - 1);

		dt.AcceptChanges();
	}

    static void formatParent(ref DataTable dt)
    {
        dt.Columns.Remove(dt.Columns[9]);
        dt.Columns.Remove(dt.Columns[9]);
        dt.Columns.Remove("Work Areas");
        //dt.Columns["Archive"].ColumnName = "System Archive";
        dt.Columns["Description"].ColumnName = "System Description";
    }

    static void formatChild(ref DataTable dt, string cvValue)
    {
        if (cvValue == "1")
        {
            dt.Columns.Remove("WTS_SYSTEM");
            dt.Columns.Remove("X");
            dt.Columns.Remove("CREATEDBY");
            dt.Columns.Remove("CREATEDDATE");
            dt.Columns.Remove("UPDATEDBY");
            dt.Columns.Remove("UPDATEDDATE");
            dt.Columns.Remove("Allocation_SystemID");
            dt.Columns.Remove("AllocationGroupID");
            dt.Columns.Remove("ALLOCATIONID");
            dt.Columns["ARCHIVE"].ColumnName = "Allocation Archive";
            dt.Columns["ALLOCATION"].ColumnName = "Allocation";
            dt.Columns["AllocationGroup"].ColumnName = "Allocation Group";
            dt.Columns["DESCRIPTION"].ColumnName = "Allocation Description";
            dt.Columns["ProposedPriority"].ColumnName = "Allocation Proposed Priority";
            dt.Columns["ApprovedPriority"].ColumnName = "Allocation Approved Priority";
            dt.Columns["WorkItem_Count"].ColumnName = "Allocation # Items";
        }
        else if (cvValue == "0")
        {
            dt.Columns.Remove("WorkArea_SystemID");
            dt.Columns.Remove("WorkAreaID");
            dt.Columns.Remove("WTS_SYSTEM");
            dt.Columns.Remove("X");
            dt.Columns.Remove("CREATEDBY");
            dt.Columns.Remove("CREATEDDATE");
            dt.Columns.Remove("UPDATEDBY");
            dt.Columns.Remove("UPDATEDDATE");
            dt.Columns["ARCHIVE"].ColumnName = "Work Area Archive";
            dt.Columns["WorkArea"].ColumnName = "Work Area";
            dt.Columns["DESCRIPTION"].ColumnName = "Work Area Description";
            dt.Columns["ProposedPriority"].ColumnName = "Work Area Proposed Priority";
            dt.Columns["ApprovedPriority"].ColumnName = "Work Area Approved Priority";
            dt.Columns["WorkItem_Count"].ColumnName = "Work Area # Items";
        } 
    }

    static void formatRaw(ref DataTable dt)
    {
        if (dt.Columns.Contains("WTS_SystemID")) dt.Columns.Remove("WTS_SystemID");
    }

    private bool ExportExcel(DataTable dt)
	{
		bool success = false;
		string errorMsg = string.Empty;

		try
		{
			Workbook wb = new Workbook(FileFormatType.Xlsx);
			Worksheet ws = wb.Worksheets[0];
            wb.Worksheets.Add();
            Worksheet wsRaw = wb.Worksheets[1];
			StyleFlag flag = new StyleFlag() { All = true };
			Aspose.Cells.Style style = new Aspose.Cells.Style();
			Aspose.Cells.Style style2 = new Aspose.Cells.Style();

			style.Pattern = BackgroundType.Solid;
			style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
			style2.Pattern = BackgroundType.Solid;
			style2.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightBlue");

			DataTable dtExcel_Formatted = new DataTable();
            DataTable dtRaw = new DataTable();
            DataTable systemRaw = new DataTable();
            DataTable workAreaRaw = new DataTable();

			FormatExcelRows(ref dt);
			RemoveExcelColumns(ref dt);
			RenameExcelColumns(ref dt);
			dtExcel_Formatted = dt.Clone();

			for (int i = 0; i <= dtExcel_Formatted.Columns.Count - 1; i++)
			{
				dtExcel_Formatted.Columns[i].DataType = typeof(string);
				dtExcel_Formatted.Columns[i].MaxLength = 255;
				dtExcel_Formatted.Columns[i].AllowDBNull = true;
			}

			foreach (DataRow dr in dt.Rows)
			{
                WTSUtility.importDataRow(ref systemRaw, dr);
				int wtsSystemID = 0;
				int.TryParse(dr["WTS_SystemID"].ToString(), out wtsSystemID);

				if (wtsSystemID > 0)
				{
					dtExcel_Formatted.ImportRow(dr);
					DataTable dtDrilldown = MasterData.WorkArea_SystemList_Get(systemID: wtsSystemID, cv:cvValue);

					if (dtDrilldown != null && dtDrilldown.Rows.Count > 1)
					{
						int count = 0;
						foreach (DataRow drChild in dtDrilldown.Rows)
						{
                            WTSUtility.importDataRow(ref workAreaRaw, drChild);
                            if (count == 0)
							{
								DataRow drSpacer = dtExcel_Formatted.NewRow();
								drSpacer[0] = "spacer";

								dtExcel_Formatted.Rows.Add(drSpacer);
                                if (cvValue == "0") { dtExcel_Formatted.Rows.Add("", "System(Task)", "Work Area", "Description", "Proposed Priority", "Approved Priority", "Tasks", "Archive"); }
                                else { dtExcel_Formatted.Rows.Add("", "System(Task)", "Allocation Group", "Allocation", "Description", "Proposed Priority", "Approved Priority", "Tasks", "Archive"); }
							}

							int drilldownWorkAreaID = 0;

                            if (cvValue == "0")
                            {
                                int.TryParse(drChild["WorkAreaID"].ToString(), out drilldownWorkAreaID);
                                if (drilldownWorkAreaID > 0) dtExcel_Formatted.Rows.Add("", drChild["WTS_SYSTEM"].ToString(), drChild["WorkArea"].ToString(), drChild["DESCRIPTION"].ToString(), drChild["ProposedPriority"].ToString(), drChild["ApprovedPriority"].ToString(), drChild["WorkItem_Count"].ToString(), (drChild["ARCHIVE"].ToString() == "1" ? "Yes" : "No"));
                            }
                            else{
                                int.TryParse(drChild["ALLOCATIONID"].ToString(), out drilldownWorkAreaID);
                                if (drilldownWorkAreaID > 0) dtExcel_Formatted.Rows.Add("", drChild["WTS_SYSTEM"].ToString(), drChild["AllocationGroup"].ToString(), drChild["ALLOCATION"].ToString(), drChild["DESCRIPTION"].ToString(), drChild["ProposedPriority"].ToString(), drChild["ApprovedPriority"].ToString(), drChild["WorkItem_Count"].ToString(), (drChild["ARCHIVE"].ToString() == "1" ? "Yes" : "No"));
                            }

                            if (count == dtDrilldown.Rows.Count - 1)
							{
								DataRow drSpacer = dtExcel_Formatted.NewRow();
								drSpacer[0] = "spacer";
								dtExcel_Formatted.Rows.Add(drSpacer);
							}
							count++;
						}
					}
				}
			}

			if (dtExcel_Formatted.Columns.Contains("WTS_SystemID")) dtExcel_Formatted.Columns.Remove("WTS_SystemID");

			string name = "Master Grid - System(Task)";
			ws.Cells.ImportDataTable(dtExcel_Formatted, true, 0, 0, false, false);

			for (int j = 0; j <= ws.Cells.Rows.Count - 1; j++)
			{
				if (ws.Cells.Rows[j][0].Value == "spacer")
				{
					ws.Cells.Rows[j][0].Value = "";
					Range spacer = ws.Cells.CreateRange(j, 0, 1, 8);
					spacer.Merge();
				}

				if (ws.Cells.Rows[j][0].Value == "System(Task)")
				{
					Range range = ws.Cells.CreateRange(j, 0, 1,7);
					range.ApplyStyle(style, flag);
				}

				if (ws.Cells.Rows[j][1].Value == "System(Task)")
				{
					Range range = ws.Cells.CreateRange(j, 1, 1, 8);
					range.ApplyStyle(style2, flag);
				}
			}

            formatParent(ref systemRaw);
            formatChild(ref workAreaRaw, cvValue);

            dtRaw = WTSUtility.JoinDataTables(systemRaw, workAreaRaw, (row1, row2) =>
                       row1.Field<string>("WTS_SystemID") == row2.Field<string>("WTS_SYSTEMID"));

            formatRaw(ref dtRaw);

            wsRaw.Cells.ImportDataTable(dtRaw, true, 0, 0, false, false);
            wsRaw.AutoFitColumns();
			ws.AutoFitColumns();
            ws.Name = "Master Grid -System";
            wsRaw.Name = "Systems Raw";
			MemoryStream ms = new MemoryStream();
			wb.Save(ms, SaveFormat.Xlsx);

			Response.ContentType = "application/xlsx";
			Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
			Response.BinaryWrite(ms.ToArray());
			success = true;

			Response.End();
		}
		catch (System.Threading.ThreadAbortException)
		{
			//expected. do nothing
		}
		catch (Exception ex)
		{
			success = false;
			errorMsg += Environment.NewLine + ex.Message;
		}

		return success;
	}

    private LinkButton CreateLink(string type, string System_ID)
    {
        LinkButton lb = new LinkButton();

        switch (type)
        {
            case "Contract":
                lb.Text = "View/Edit";
                lb.Attributes["onclick"] = string.Format("openSystemContracts('{0}'); return false;", System_ID);
                break;
            case "Review":
                lb.Text = "Review";
                lb.Attributes["onclick"] = string.Format("reviewWorkAreas('{0}'); return false;", System_ID);
                break;
        }

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
		bool exists = false, saved = false, duplicate = false;
		int savedQty = 0, failedQty = 0;
		string ids = string.Empty, failedIds = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

		try
		{
			DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjson.Rows.Count == 0)
			{
				errorMsg = "Unable to save. An invalid list of changes was provided.";
			}
			else
			{
				int id = 0, sortOrder = 0, contract = 0, busWorkloadManagerID = 0, devWorkloadManagerID = 0, archive = 0;
				string system = string.Empty, description = string.Empty;

				HttpServerUtility server = HttpContext.Current.Server;
				//save
				foreach (DataRow dr in dtjson.Rows)
				{
					id = sortOrder = contract = 0;
					system = description = string.Empty;
					archive = 0;

					tempMsg = string.Empty;
					int.TryParse(dr["WTS_SystemID"].ToString(), out id);
					system = server.UrlDecode(dr["WTS_System"].ToString());
					description = server.UrlDecode(dr["DESCRIPTION"].ToString());
					int.TryParse(dr["CONTRACT"].ToString(), out contract);
					int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
                    int.TryParse(dr["BusWorkloadManager"].ToString(), out busWorkloadManagerID);
                    int.TryParse(dr["DevWorkloadManager"].ToString(), out devWorkloadManagerID);
                    int.TryParse(dr["ARCHIVE"].ToString(), out archive);

					if (string.IsNullOrWhiteSpace(system))
					{
						if (id == 0)
						{
							continue;
						}
						tempMsg = "You must specify a value for System.";
						saved = false;
					}
					else
					{
						if (id == 0)
						{
							exists = false;
							saved = MasterData.System_Add(system, description, sortOrder, busWorkloadManagerID, devWorkloadManagerID, archive == 1, out exists, out id, out tempMsg, contractID: contract);
							if (exists)
							{
								saved = false;
								tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate System record [" + system + "].");
							}
						}
						else
						{
							saved = MasterData.System_Update(id, system, description, sortOrder, busWorkloadManagerID, devWorkloadManagerID, archive == 1, out tempMsg, contractID: contract);
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
						failedIds += string.Format("{0}{1}", failedIds.Length > 0 ? "," : "", failedIds.ToString());
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

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WTS_SYSTEM);

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
				deleted = MasterData.System_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WTS_SYSTEM);

        return JsonConvert.SerializeObject(result, Formatting.None);
	}

    [WebMethod(true)]
    public static string ReviewWorkAreas(string systemID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
            , { "id", "" }
            , { "error", "" } };
        bool saved = false;
        int SystemID = 0;
        string ids = systemID, failedIds = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            int.TryParse(systemID, out SystemID);

            if (SystemID == 0)
            {
                tempMsg = "Unable to find System.";
                saved = false;
            }
            else
            {
                    saved = MasterData.System_ReviewWorkAreas(SystemID, out tempMsg);
            }

            if (tempMsg.Length > 0)
            {
                errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            saved = false;
            errorMsg = ex.Message;
        }

        result["savedIds"] = ids;
        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WTS_SYSTEM);

        return JsonConvert.SerializeObject(result, Formatting.None);
    }
}