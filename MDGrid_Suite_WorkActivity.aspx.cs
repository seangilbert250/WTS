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


public partial class MDGrid_Suite_WorkActivity : System.Web.UI.Page
{
    protected DataTable _dtWorkActivity = null;
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;

    protected int _qfSystemSuiteID = 0;
    protected int CurrentLevel = 0;

    protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;
    protected string CurrentResource;

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
        if (Request.QueryString["CurrentLevel"] != null && 
            !string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"]))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["CurrentLevel"]), out this.CurrentLevel);
        }
        if (Request.QueryString["WTS_SYSTEM_SUITEID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["WTS_SYSTEM_SUITEID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["WTS_SYSTEM_SUITEID"].ToString()), out this._qfSystemSuiteID);
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
        _dtWorkActivity = MasterData.ItemTypeList_Get();
        DataTable dt = MasterData.WTS_System_Suite_WorkActivityList_Get(SystemSuiteID: _qfSystemSuiteID);
		HttpContext.Current.Session["dtMD_WorkActivity"] = dt;

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			InitializeColumnData(ref dt);
			dt.AcceptChanges();

            if (_dtWorkActivity != null && _dtWorkActivity.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _dtWorkActivity.DefaultView.RowFilter = string.Format("NOT WORKITEMTYPEID = {0} ", dr["WORKITEMTYPEID"].ToString());
                    _dtWorkActivity = _dtWorkActivity.DefaultView.ToTable();
                }
            }
        }

        int count = dt.Rows.Count;
		count = count > 0 ? count - 1 : count; //need to subtract the empty row
		spanRowCount.InnerText = count.ToString();

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
					case "A":
						blnVisible = true;
						break;
					case "X":
						blnVisible = true;
						break;
                    case "WORKITEMTYPE":
						displayName = "Work Activity";
						blnVisible = true;
						blnSortable = true;
						break;
                    case "DESCRIPTION":
                        displayName = "Description";
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

		row.Cells[DCC.IndexOf("A")].Text = "&nbsp;";
		row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
    }

	void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("WORKITEMTYPEID")].Text.Trim();

        if (itemId == "0" || itemId == CurrentResource)
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
            string selectedId = row.Cells[DCC.IndexOf("WORKITEMTYPEID")].Text;
            string selectedText = row.Cells[DCC.IndexOf("WORKITEMTYPE")].Text;
            DropDownList ddl = null;
            ddl = WTSUtility.CreateGridDropdownList(_dtWorkActivity, "Item Type", "Item Type", "WORKITEMTYPEID", itemId, selectedId, selectedText, null);
            ddl.Enabled = (itemId == "0");
            row.Cells[DCC.IndexOf("WORKITEMTYPE")].Controls.Add(ddl);

            row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("DESCRIPTION", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Trim())));

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

        string dependencies = "0";
		int count = 0, systemCount = 0;
		int.TryParse(dependencies, out count);
		string systems = Server.HtmlDecode(row.Cells[DCC.IndexOf("System_Count")].Text).Trim();
		int.TryParse(systems, out systemCount);

		if (!CanEdit
			|| count > 0
			|| systemCount > 0)
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
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WORKITEMTYPEID")].Text.Trim()));
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
			divChildCount.InnerText = string.Format("({0})", systemCount.ToString());
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
        CurrentResource = itemId;

    }

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_WorkActivity"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkActivity"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("WORKITEMTYPE")
                && i != DCC.IndexOf("DESCRIPTION")
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
		row.Cells[DCC.IndexOf("WORKITEMTYPE")].Style["width"] = "200px";
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
		img.ToolTip = string.Format("{0} Systems for [{1}]", direction, itemId);
		img.AlternateText = string.Format("{0} Systems for [{1}]", direction, itemId);
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
			row.Attributes.Add("WORKITEMTYPEID", itemId);
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
				else if (i == DCC["WorkItemTypeID"].Ordinal)
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
    #endregion Grid

    [WebMethod(true)]
    public static string SaveChanges(string rows, int systemSuiteID = 0)
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

            int id = 0, archive = 0;

			HttpServerUtility server = HttpContext.Current.Server;
            //save
            foreach (DataRow dr in dtjson.Rows)
            {
                id = 0;
                archive = 0;

                tempMsg = string.Empty;
                int.TryParse(dr["WORKITEMTYPE"].ToString(), out id);
                int.TryParse(dr["ARCHIVE"].ToString(), out archive);
                DataTable dtSystem = MasterData.SystemList_Get(includeArchive: true);
                dtSystem.DefaultView.RowFilter = "(WTS_SystemSuiteID = " + systemSuiteID + "OR WTS_SystemID = 0) AND ARCHIVE = 0";
                dtSystem = dtSystem.DefaultView.ToTable();
                foreach (DataRow drSystem in dtSystem.Rows)
                {
                    int systemID = 0, rSystemID = 0;
                    int.TryParse(drSystem["WTS_SystemID"].ToString(), out systemID);
                    if (systemID > 0)
                    {
                        saved = MasterData.WTS_System_WorkActivity_Add(WTS_SYSTEMID: systemID
                                , WTS_WORKACTIVITYID: id
                                , exists: out exists
                                , newID: out rSystemID
                                , errorMsg: out tempMsg);
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
    public static string RemoveItem(int itemId, int systemSuiteID)
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
                errorMsg = "You must specify an item to remove systems.";
            }
            else
            {
                if (systemSuiteID > 0)
                {
                    DataTable dtSystem = MasterData.ItemType_SystemList_Get(WorkItemTypeID: itemId, SystemSuiteID: systemSuiteID);
                    foreach (DataRow drSystem in dtSystem.Rows)
                    {
                        int systemID = 0;
                        int.TryParse(drSystem["WTS_SYSTEM_WORKACTIVITYID"].ToString(), out systemID);
                        if (systemID > 0)
                        {
                            deleted = MasterData.WTS_System_WorkActivity_Delete(WTS_SYSTEM_WORKACTIVITYID: systemID, exists: out exists, errorMsg: out errorMsg);
                        }
                    }
                }
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