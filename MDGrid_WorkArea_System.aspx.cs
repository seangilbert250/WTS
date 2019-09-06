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


public partial class MDGrid_WorkArea_System : System.Web.UI.Page
{
	protected DataTable _dtWorkArea = null;
    protected DataTable _dtALLOCATION = null;
	protected DataTable _dtSystem = null;
    protected DataTable _dtAllocationGroup;
	protected DataColumnCollection DCC;
    protected DataColumnCollection DCCAA;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfWorkAreaID = 0;
    protected int _qfALLOCATIONID = 0;
    protected int _qfSystemID = 0;
    protected String _qfSystems = "";
    protected int _qfSystemSuiteID = 0;

    protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;
    protected int CurrentLevel = 0;

    protected bool CanView = false;
	protected bool CanEdit = false;
	protected bool IsAdmin = false;

    protected string cvValue = "0";

    protected void Page_Load(object sender, EventArgs e)
    {
        cvValue = Request.QueryString["ChildView"] == null ? "0" : Request.QueryString["ChildView"].Trim();
        this.IsAdmin = UserManagement.UserIsInRole("Admin");
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
		this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.MasterData));

		readQueryString();

        if (cvValue == "1")
        {
            //From parent
            //ChildView == 1, 'Allocation Assignment' selected
            initControlsAA();
            loadGridDataAA();
        }
        else {
            //From parent
            //ChildView == 0, 'Work Area' selected
            initControls();
            loadGridData();
        }
    }

	private void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}
		if (Request.QueryString["WorkAreaID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["WorkAreaID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["WorkAreaID"].ToString()), out this._qfWorkAreaID);
            int.TryParse(Server.UrlDecode(Request.QueryString["WorkAreaID"].ToString()), out this._qfALLOCATIONID);
        }
		if (Request.QueryString["SystemID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["SystemID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["SystemID"].ToString()), out this._qfSystemID);
		}
		if (Request.QueryString["SelectedSystems"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSystems"].ToString()))
		{
            this._qfSystems = Server.UrlDecode(Request.QueryString["SelectedSystems"].ToString());
		}
        if (Request.QueryString["SystemSuiteID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["SystemSuiteID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["SystemSuiteID"].ToString()), out this._qfSystemSuiteID);
        }

        if (Request.QueryString["CurrentLevel"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"]))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["CurrentLevel"]), out this.CurrentLevel);
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

    private void initControlsAA()
    {
        grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBoundAA;
        grdMD.GridRowDataBound += grdMD_GridRowDataBoundAA;
        grdMD.GridPageIndexChanging += grdMD_GridPageIndexChangingAA;
    }

    private void loadGridDataAA()
    {
        _dtSystem = MasterData.SystemList_Get(includeArchive: false, cv: cvValue);
        if (_dtSystem != null && _qfSystemID != 0)
        {
            _dtSystem.DefaultView.RowFilter = string.Format(" WTS_SYSTEMID IN (0, {0}) ", _qfSystemID.ToString());
            _dtSystem = _dtSystem.DefaultView.ToTable();
        }

        _dtALLOCATION = MasterData.WorkAreaList_Get(includeArchive: false, cv: cvValue);
        if (_dtALLOCATION != null && _qfWorkAreaID != 0)
        {
            _dtALLOCATION.DefaultView.RowFilter = string.Format(" ALLOCATIONID = {0} ", _qfALLOCATIONID.ToString());
            _dtALLOCATION = _dtALLOCATION.DefaultView.ToTable();
        }

        _dtAllocationGroup = MasterData.AllocationGroup_Get();

        DataTable dt = MasterData.WorkArea_SystemList_Get(workAreaID: this._qfALLOCATIONID, systemID: this._qfSystemID, cv: cvValue);

        if (dt != null)
        {
            //if (_qfSystemID > 0)
            //{
            //    dt.Columns["WTS_SYSTEM"].SetOrdinal(dt.Columns["ALLOCATIONID"].Ordinal);
            //    dt.Columns["WTS_SYSTEMID"].SetOrdinal(dt.Columns["WTS_SYSTEM"].Ordinal);
            //    dt.AcceptChanges();
            //}

            this.DCCAA = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCCAA, Newtonsoft.Json.Formatting.None));

            ListItem item = null;
            foreach (DataRow row in _dtALLOCATION.Rows)
            {
                item = ddlQF.Items.FindByValue(row["ALLOCATIONID"].ToString());
                if (item == null)
                {
                    ddlQF.Items.Add(new ListItem(row["ALLOCATION"].ToString(), row["ALLOCATIONID"].ToString()));
                }
            }
            item = ddlQF.Items.FindByValue(_qfWorkAreaID.ToString());
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

            InitializeColumnDataAA(ref dt);
            dt.AcceptChanges();
        }

        if (_qfWorkAreaID != 0 && dt != null && dt.Rows.Count > 0)
        {
            dt.DefaultView.RowFilter = string.Format(" ALLOCATIONID =  {0} OR (ALLOCATIONID = 0 AND WTS_SYSTEMID = 0) ", _qfWorkAreaID.ToString());
            dt = dt.DefaultView.ToTable();
        }
        if (_qfSystemID != 0 && dt != null && dt.Rows.Count > 0)
        {
            dt.DefaultView.RowFilter = string.Format(" WTS_SYSTEMID =  {0} OR (WTS_SYSTEMID = 0 AND ALLOCATIONID = 0) OR WTS_SYSTEMID IS NULL ", _qfSystemID.ToString());
            dt = dt.DefaultView.ToTable();
        }
        int count = dt.Rows.Count;
        count = count > 0 ? count - 1 : count; //need to subtract the empty row
        spanRowCount.InnerText = count.ToString();

        grdMD.DataSource = dt;
        grdMD.DataBind();
    }

    private void loadGridData()
	{
		_dtSystem = MasterData.SystemList_Get(includeArchive: false, cv: cvValue);
		if (_dtSystem != null && _qfSystemID != 0)
		{
			_dtSystem.DefaultView.RowFilter = string.Format(" WTS_SYSTEMID IN (0, {0}) ", _qfSystemID.ToString());
			_dtSystem = _dtSystem.DefaultView.ToTable();
		}

        _dtWorkArea = MasterData.WorkAreaList_Get(includeArchive: false, workArea_SystemID: _qfSystemID);
		if (_dtWorkArea != null && _qfWorkAreaID != 0)
		{
			_dtWorkArea.DefaultView.RowFilter = string.Format(" WorkAreaID = {0} ", _qfWorkAreaID.ToString());
			_dtWorkArea = _dtWorkArea.DefaultView.ToTable();
		}

		DataTable dt = MasterData.WorkArea_SystemList_Get(workAreaID: this._qfWorkAreaID, systemID: this._qfSystemID, systemSuiteID: this._qfSystemSuiteID, systemIDs: _qfSystems);

		if (dt != null)
		{
			if (_qfSystemID > 0)
			{
				dt.Columns["WTS_SYSTEM"].SetOrdinal(dt.Columns["WorkAreaID"].Ordinal);
				dt.Columns["WTS_SYSTEMID"].SetOrdinal(dt.Columns["WTS_SYSTEM"].Ordinal);
				dt.AcceptChanges();
			}

            this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			ListItem item = null;
			foreach (DataRow row in _dtWorkArea.Rows)
			{
				item = ddlQF.Items.FindByValue(row["WorkAreaID"].ToString());
				if (item == null)
				{
					ddlQF.Items.Add(new ListItem(row["WorkArea"].ToString(), row["WorkAreaID"].ToString()));
				}
			}
			item = ddlQF.Items.FindByValue(_qfWorkAreaID.ToString());
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

		if (_qfWorkAreaID != 0 && dt != null && dt.Rows.Count > 0)
		{
			dt.DefaultView.RowFilter = string.Format(" WorkAreaID =  {0} OR (WorkAreaID = 0 AND WTS_SYSTEMID = 0) ", _qfWorkAreaID.ToString());
			dt = dt.DefaultView.ToTable();
		}
        if (_qfSystemID != 0 && dt != null && dt.Rows.Count > 0)
		{
			dt.DefaultView.RowFilter = string.Format(" WTS_SYSTEMID =  {0} OR (WTS_SYSTEMID = 0 AND WorkAreaID = 0) OR WTS_SYSTEMID IS NULL ", _qfSystemID.ToString());
			dt = dt.DefaultView.ToTable();
		}
		int count = dt.Rows.Count;
		count = count > 0 ? count - 1 : count; //need to subtract the empty row
		spanRowCount.InnerText = count.ToString();

		grdMD.DataSource = dt;
		grdMD.DataBind();
	}

    protected void InitializeColumnDataAA(ref DataTable dt)
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
                        blnVisible = true;
                        break;
                    case "Allocation_SystemId":
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "WTS_SYSTEMID":
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "WTS_SYSTEM":
                        displayName = "System(Task)";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "AllocationGroup":
                        displayName = "Allocation Group";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ALLOCATIONID":
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "ALLOCATION":
                        displayName = "Allocation";
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
                        displayName = "Primary Tasks";
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
						blnVisible = true;
						break;
					case "WorkArea_SystemID":
						blnVisible = false;
						blnSortable = false;
						break;
					case "WorkAreaID":
						blnVisible = false;
						blnSortable = false;
						break;
					case "WorkArea":
						displayName = "Work Area";
						blnVisible = true;
						blnSortable = true;
						break;
					case "WTS_SYSTEMID":
						blnVisible = false;
						blnSortable = false;
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
						displayName = "Primary Tasks";
						blnVisible = true;
						blnSortable = true;
						break;
                    case "CREATEDDATE":
                        displayName = "Associated Date";
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

    void grdMD_GridHeaderRowDataBoundAA(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplayAA(ref row);
        row.Cells[DCCAA.IndexOf("X")].Text = "&nbsp;";
    }

    void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridHeader(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
	}

    void grdMD_GridRowDataBoundAA(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplayAA(ref row);

        //add edit link
        string itemId = row.Cells[DCCAA.IndexOf("Allocation_SystemId")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            string selectedId = row.Cells[DCCAA.IndexOf("ALLOCATIONID")].Text;
            string selectedText = row.Cells[DCCAA.IndexOf("ALLOCATION")].Text;
            if (itemId == "0" && _qfWorkAreaID != 0)
            {
                selectedId = _qfWorkAreaID.ToString();
                selectedText = string.Empty;
            }
            DropDownList ddl = null;
            ddl = WTSUtility.CreateGridDropdownList(_dtALLOCATION, "ALLOCATION", "ALLOCATION", "ALLOCATIONID", itemId, selectedId, selectedText, null);
            ddl.Enabled = (_qfWorkAreaID == 0);
            row.Cells[DCCAA.IndexOf("ALLOCATION")].Controls.Add(ddl);
            //row.Cells[DCCAA.IndexOf("AllocationGroup")].Controls.Add(WTSUtility.CreateGridDropdownList(_dtAllocationGroup, "AllocationGroup", "ALLOCATIONGROUP", "ALLOCATIONGROUPID", itemId, row.Cells[DCCAA.IndexOf("AllocationGroupID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCCAA.IndexOf("AllocationGroup")].Text.Replace("&nbsp;", " ").Trim(), null));
            selectedId = row.Cells[DCCAA.IndexOf("WTS_SYSTEMID")].Text.Replace("&nbsp;", " ").Trim();
            selectedText = row.Cells[DCCAA.IndexOf("WTS_SYSTEM")].Text.Replace("&nbsp;", " ").Trim();
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
            row.Cells[DCCAA.IndexOf("WTS_SYSTEM")].Controls.Add(ddl);
            row.Cells[DCCAA.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCCAA.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;", " ").Trim())));

            row.Cells[DCCAA.IndexOf("ProposedPriority")].Controls.Add(WTSUtility.CreateGridTextBox("ProposedPriority", itemId, Server.HtmlDecode(row.Cells[DCCAA.IndexOf("ProposedPriority")].Text.Replace("&nbsp;", " ").Trim()), true));
            if (IsAdmin)
            {
                row.Cells[DCCAA.IndexOf("ApprovedPriority")].Controls.Add(WTSUtility.CreateGridTextBox("ApprovedPriority", itemId, Server.HtmlDecode(row.Cells[DCCAA.IndexOf("ApprovedPriority")].Text.Replace("&nbsp;", " ").Trim()), true));
            }

            bool archive = false;
            if (row.Cells[DCCAA.IndexOf("ARCHIVE")].HasControls()
                && row.Cells[DCCAA.IndexOf("ARCHIVE")].Controls[0] is CheckBox)
            {
                archive = ((CheckBox)row.Cells[DCCAA.IndexOf("ARCHIVE")].Controls[0]).Checked;
            }
            else if (row.Cells[DCCAA.IndexOf("ARCHIVE")].Text == "1")
            {
                archive = true;
            }
            row.Cells[DCCAA.IndexOf("ARCHIVE")].Controls.Clear();
            row.Cells[DCCAA.IndexOf("ARCHIVE")].Controls.Add(WTSUtility.CreateGridCheckBox("Archive", itemId, archive));
        }

        if (!CanEdit)
        {
            Image imgBlank = new Image();
            imgBlank.Height = 10;
            imgBlank.Width = 10;
            imgBlank.ImageUrl = "Images/Icons/blank.png";
            imgBlank.AlternateText = "";
            row.Cells[DCCAA["X"].Ordinal].Controls.Add(imgBlank);
        }
        else
        {
            row.Cells[DCCAA["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCCAA.IndexOf("WTS_SYSTEM")].Text.Trim() + " - " + row.Cells[DCCAA.IndexOf("ALLOCATION")].Text.Trim()));
        }
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("WorkArea_SystemID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			string selectedId = row.Cells[DCC.IndexOf("WorkAreaID")].Text;
			string selectedText = row.Cells[DCC.IndexOf("WorkArea")].Text;
			if (itemId == "0" && _qfWorkAreaID != 0)
			{
				selectedId = _qfWorkAreaID.ToString();
				selectedText = string.Empty;
			}
			DropDownList ddl = null;
			ddl = WTSUtility.CreateGridDropdownList(_dtWorkArea, "WorkArea", "WorkArea", "WorkAreaID", itemId, selectedId, selectedText, null);
			ddl.Enabled = (_qfWorkAreaID == 0);
			row.Cells[DCC.IndexOf("WorkArea")].Controls.Add(ddl);

			selectedId = row.Cells[DCC.IndexOf("WTS_SYSTEMID")].Text.Replace("&nbsp;", " ").Trim();
			selectedText = row.Cells[DCC.IndexOf("WTS_SYSTEM")].Text.Replace("&nbsp;"," ").Trim();
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
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WTS_SYSTEM")].Text.Trim() + " - " + row.Cells[DCC.IndexOf("WorkArea")].Text.Trim()));
		}
	}

    void grdMD_GridPageIndexChangingAA(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtMD_WorkArea_System"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkArea_System"];
        }
    }

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_WorkArea_System"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkArea_System"];
		}
	}

    void formatColumnDisplayAA(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCCAA.IndexOf("X")
                && i != DCCAA.IndexOf("ALLOCATION")
                && i != DCCAA.IndexOf("WTS_SYSTEM")
                && i != DCCAA.IndexOf("ProposedPriority")
                && i != DCCAA.IndexOf("ApprovedPriority")
                && i != DCCAA.IndexOf("WorkItem_Count")
                && i != DCCAA.IndexOf("ARCHIVE"))
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
        row.Cells[DCCAA.IndexOf("X")].Style["width"] = "12px";
        row.Cells[DCCAA.IndexOf("WTS_SYSTEM")].Style["width"] = "150px";
        row.Cells[DCCAA.IndexOf("ALLOCATION")].Style["width"] = "225px";
        row.Cells[DCCAA.IndexOf("WorkItem_Count")].Style["width"] = "85px";
        row.Cells[DCCAA.IndexOf("ProposedPriority")].Style["width"] = "75px";
        row.Cells[DCCAA.IndexOf("ApprovedPriority")].Style["width"] = "75px";
        row.Cells[DCCAA.IndexOf("ARCHIVE")].Style["width"] = "55px";
    }

    void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("WorkArea")
				&& i != DCC.IndexOf("WTS_SYSTEM")
				&& i != DCC.IndexOf("ProposedPriority")
				&& i != DCC.IndexOf("ApprovedPriority")
				&& i != DCC.IndexOf("WorkItem_Count")
				&& i != DCC.IndexOf("CREATEDDATE")
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
		row.Cells[DCC.IndexOf("WTS_SYSTEM")].Style["width"] = "150px";
		row.Cells[DCC.IndexOf("WorkArea")].Style["width"] = "225px";
		row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "85px";
		row.Cells[DCC.IndexOf("ProposedPriority")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("ApprovedPriority")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("CREATEDDATE")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
    }

	#endregion Grid


	[WebMethod(true)]
	public static string SaveChanges(string rows, string cvValue)
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
                if (cvValue == "0")
                {
                    int id = 0, workAreaID = 0, proposedPriority = 0, approvedPriority = 0, systemID = 0, archive = 0;  //, ALLOCATIONGROUPID = 0;
                    bool duplicate = false;
                    string description = string.Empty;

                    HttpServerUtility server = HttpContext.Current.Server;
                    //save
                    foreach (DataRow dr in dtjson.Rows)
                    {
                        id = systemID = workAreaID = 0;
                        proposedPriority = approvedPriority = archive = 0;  // ALLOCATIONGROUPID = 0;
                        description = string.Empty;
                        archive = 0;
                        duplicate = false;

                        tempMsg = string.Empty;
                        int.TryParse(dr["WorkArea_SystemID"].ToString(), out id);
                        int.TryParse(dr["WorkArea"].ToString(), out workAreaID);
                        int.TryParse(dr["WTS_SYSTEM"].ToString(), out systemID);
                        description = server.UrlDecode(dr["DESCRIPTION"].ToString());
                        int.TryParse(dr["ProposedPriority"].ToString(), out proposedPriority);
                        int.TryParse(dr["ApprovedPriority"].ToString(), out approvedPriority);
                        int.TryParse(dr["ARCHIVE"].ToString(), out archive);
                        //int.TryParse(dr["ALLOCATIONGROUPID"].ToString(), out ALLOCATIONGROUPID);

                        if (workAreaID == 0 || systemID == 0)
                        {
                            tempMsg = "You must specify a Work Area and a System(Task).";
                            saved = false;
                        }
                        else
                        {
                            if (id == 0)
                            {
                                exists = false;
                                saved = MasterData.WorkArea_System_Add(workAreaID: workAreaID
                                    , systemID: systemID
                                    , description: description
                                    , proposedPriority: proposedPriority
                                    , approvedPriority: approvedPriority
                                    //, ALLOCATIONGROUPID : ALLOCATIONGROUPID
                                    , cv: cvValue
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
                                saved = MasterData.WorkArea_System_Update(workAreaSystemID: id
                                    , workAreaID: workAreaID
                                    , systemID: systemID
                                    , description: description
                                    , proposedPriority: proposedPriority
                                    , approvedPriority: approvedPriority
                                    , cv: cvValue
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
                else
                {
                    int id = 0, ALLOCATIONID = 0, proposedPriority = 0, approvedPriority = 0, systemID = 0, archive = 0;
                    bool duplicate = false;
                    string description = string.Empty;

                    HttpServerUtility server = HttpContext.Current.Server;
                    //save
                    foreach (DataRow dr in dtjson.Rows)
                    {
                        id = systemID = ALLOCATIONID = 0;
                        proposedPriority = approvedPriority = archive = 0;
                        description = string.Empty;
                        archive = 0;
                        duplicate = false;

                        tempMsg = string.Empty;
                        int.TryParse(dr["Allocation_SystemId"].ToString(), out id);
                        int.TryParse(dr["ALLOCATION"].ToString(), out ALLOCATIONID);
                        int.TryParse(dr["WTS_SYSTEM"].ToString(), out systemID);
                        description = server.UrlDecode(dr["DESCRIPTION"].ToString());
                        int.TryParse(dr["ProposedPriority"].ToString(), out proposedPriority);
                        int.TryParse(dr["ApprovedPriority"].ToString(), out approvedPriority);
                        int.TryParse(dr["ARCHIVE"].ToString(), out archive);

                        if (ALLOCATIONID == 0)
                        {
                            tempMsg = "You must specify a Allocation.";
                            saved = false;
                        }
                        else
                        {
                            if (id == 0)
                            {
                                exists = false;
                                saved = MasterData.WorkArea_System_Add(workAreaID: ALLOCATIONID
                                    , systemID: systemID
                                    , description: description
                                    , proposedPriority: proposedPriority
                                    , approvedPriority: approvedPriority
                                    , cv: cvValue
                                    , exists: out exists
                                    , newID: out id
                                    , errorMsg: out tempMsg);
                                if (exists)
                                {
                                    saved = false;
                                    tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Allocation record.");
                                }
                            }
                            else
                            {
                                saved = MasterData.WorkArea_System_Update(workAreaSystemID: id
                                    , workAreaID: ALLOCATIONID
                                    , systemID: systemID
                                    , description: description
                                    , proposedPriority: proposedPriority
                                    , approvedPriority: approvedPriority
                                    , cv: cvValue
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
	public static string DeleteItem(int itemId, string item, string cvValue)
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
				deleted = MasterData.WorkArea_System_Delete(workAreaSystemID: itemId, cv:cvValue, exists: out exists, errorMsg: out errorMsg);
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