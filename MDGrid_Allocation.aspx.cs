using System;
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


public partial class MDGrid_Allocation : System.Web.UI.Page
{
	protected DataTable _dtCat = null;
    protected DataTable _dtGroup = null;  // 12902 - 3
    protected DataTable _dtResources = null;  // 12902 - 3 - Also, Masterdata addition.

    protected DataTable _dtUser = null;
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfAllocationCategoryID = 0;

    protected int _qfAllocationGroupID = 0;


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

		if (!IsPostBack) loadGridData();
	}

	private void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}
		if (Request.QueryString["AllocationID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["AllocationID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["AllocationID"].ToString()), out this._qfAllocationCategoryID);
		}
        // 12902 - 3:
        if (Request.QueryString["AllocationGroupID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["AllocationGroupID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["AllocationGroupID"].ToString()), out this._qfAllocationGroupID);
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
		grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
		grdMD.GridRowDataBound += grdMD_GridRowDataBound;
		grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
	}

    private void loadGridData(bool bind = true)
	{
        _dtResources = MasterData.WTS_Resource_Get(includeArchive: false);


		_dtUser = UserManagement.LoadUserList(organizationId: 0, excludeDeveloper: false, loadArchived: false, userNameSearch: "");
		Page.ClientScript.RegisterArrayDeclaration("_userList", JsonConvert.SerializeObject(_dtUser, Newtonsoft.Json.Formatting.None));
        DataTable dt = null, dtTemp = null;
        DataSet ds = null;
        if (_refreshData || Session["dtMD_AllocationDS"] == null)
        {
            ds = MasterData.AllocationList_Get(includeArchive: true);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables.Contains("Allocation"))
                {
                    dt = ds.Tables["Allocation"];
                    HttpContext.Current.Session["dtMD_AllocationDS"] = ds;
                }
            }

        }
        else
        {
            ds = (DataSet) HttpContext.Current.Session["dtMD_AllocationDS"];
            if (ds.Tables.Contains("Allocation"))
            {
                dt = ds.Tables["Allocation"];
            }
        }

        if (dt != null)
		{
            // 12902 - 3:
            dtTemp = dt.DefaultView.ToTable(true, new string[] { "AllocationGroupID", "AllocationGroup", "AllocationCategoryID", "AllocationCategory" });
            dtTemp.DefaultView.RowFilter = "AllocationGroupID IS NOT NULL";

            //dtTemp = dt.DefaultView.ToTable(true, new string[] { "AllocationCategoryID", "AllocationCategory" });
            //dtTemp.DefaultView.RowFilter = "AllocationCategoryID IS NOT NULL";
            dtTemp = dtTemp.DefaultView.ToTable();

			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			if (ds.Tables.Contains("Category"))
			{
				_dtCat = ds.Tables["Category"];
				Page.ClientScript.RegisterArrayDeclaration("_AllocationCategoryList", JsonConvert.SerializeObject(_dtCat, Newtonsoft.Json.Formatting.None));
			}
            //12092 - 3:
            if (ds.Tables.Contains("Group"))
            {
                _dtGroup = ds.Tables["Group"];
                Page.ClientScript.RegisterArrayDeclaration("_AllocationGroupList", JsonConvert.SerializeObject(_dtGroup, Newtonsoft.Json.Formatting.None));
            }

            ListItem item = null;
			foreach (DataRow row in dtTemp.Rows)
			{
				item = ddlQF.Items.FindByValue(row["AllocationCategoryID"].ToString());
				if (item == null)
				{
					ddlQF.Items.Add(new ListItem(row["AllocationCategory"].ToString(), row["AllocationCategoryID"].ToString()));
				}

                // 12902 - 3:
                item = ddlQFGroup.Items.FindByValue(row["AllocationGroupID"].ToString());
                if (item == null)
                {
                    ddlQFGroup.Items.Add(new ListItem(row["AllocationGroup"].ToString(), row["AllocationGroupID"].ToString()));
                }

            }

            // 12092 - 3 - Remarked out:
   //         item = ddlQF.Items.FindByValue(_qfAllocationCategoryID.ToString());
			//if (item != null)
			//{
			//	item.Selected = true;
			//}

            // 12092 - 3:
            item = ddlQFGroup.Items.FindByValue(_qfAllocationGroupID.ToString());
            if (item != null)
            {
                item.Selected = true;
            }

            InitializeColumnData(ref dt);
			dt.AcceptChanges();

            iti_Tools_Sharp.DynamicHeader head = WTSUtility.CreateGridMultiHeader(dt);
			if (head != null)
			{
				grdMD.DynamicHeader = head;
			}
		}

        // 12902 - 3:
		if (_qfAllocationGroupID != 0 && dt != null && dt.Rows.Count > 0)
		{
			dt.DefaultView.RowFilter = string.Format(" AllocationGroupID =  {0}", _qfAllocationGroupID.ToString());
			dt = dt.DefaultView.ToTable();
		}
        //if (_qfAllocationCategoryID != 0 && dt != null && dt.Rows.Count > 0)
        //{
        //    dt.DefaultView.RowFilter = string.Format(" AllocationCategoryID =  {0}", _qfAllocationCategoryID.ToString());
        //    dt = dt.DefaultView.ToTable();
        //}

        int count = dt.Rows.Count;
		count = count > 0 ? count - 1 : count; //need to subtract the empty row
		spanRowCount.InnerText = count.ToString();

        if (_export)
        {
            exportExcel(dt);
        }

        grdMD.DataSource = dt;
		if (bind) grdMD.DataBind();
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
					//case "AllocationCategory":
					//	displayName = "Allocation Category";
					//	blnVisible = true;
					//	blnSortable = true;
					//	break;
                        // 12902 - 3:
                    case "AllocationGroup":
                        displayName = "Allocation Group";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ALLOCATION":
						displayName = "Allocation Assignment";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = true;
						break;
					case "DefaultAssignedTo":
						displayName = "Assigned To";
						groupName = "Default Resources";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DefaultSME":
						displayName = "SME";
						groupName = "Default Resources";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DefaultBusinessResource":
						displayName = "Business Resource";
						groupName = "Default Resources";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DefaultTechnicalResource":
						displayName = "Technical Resource";
						groupName = "Default Resources";
						blnVisible = true;
						blnSortable = true;
						break;
					case "System_Count":
						displayName = "Systems";
						blnVisible = false;
						blnSortable = true;
						break;
					case "WorkItem_Count":
						displayName = "Tasks";
						blnVisible = true;
						blnSortable = true;
						break;
					case "SORT_ORDER":
						displayName = "Rank";
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

	#region Grid

	void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		columnData.SetupGridHeader(e.Row);
		row.Attributes.Add("ID", string.Format("GridHeaderRow{0}", row.RowIndex == -1 ? "" : row.RowIndex.ToString()));

		formatColumnDisplay(ref row);

		row.Cells[DCC.IndexOf("A")].Text = "&nbsp;";
		row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";

		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (row.RowIndex > 0)
			{
				row.Cells[i].Style["border-top"] = "1px solid grey";
			}
		}
	}

	void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("ALLOCATIONID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}
		
		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			row.Cells[DCC.IndexOf("AllocationCategory")].Controls.Add(WTSUtility.CreateGridDropdownList("AllocationCategory", itemId, row.Cells[DCC.IndexOf("AllocationCategory")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("AllocationCategoryID")].Text.Replace("&nbsp;", " ").Trim(), 195));
            // 12902 - 3:  _dtGroup -> Changing to the following line fixed a few problems with the drop down behaviour (Drops down on first click, selection saves, etc).
            row.Cells[DCC.IndexOf("AllocationGroup")].Controls.Add(WTSUtility.CreateGridDropdownList(_dtGroup, "AllocationGroup", "AllocationGroup", "AllocationGroupID", itemId, row.Cells[DCC.IndexOf("AllocationGroupID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("AllocationGroup")].Text.Replace("&nbsp;", " ").Trim(), null));
            //row.Cells[DCC.IndexOf("AllocationGroup")].Controls.Add(WTSUtility.CreateGridDropdownList("AllocationGroup", itemId, row.Cells[DCC.IndexOf("AllocationGroup")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("AllocationGroupID")].Text.Replace("&nbsp;", " ").Trim(), 195));

            row.Cells[DCC.IndexOf("ALLOCATION")].Controls.Add(WTSUtility.CreateGridTextBox("Allocation", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ALLOCATION")].Text.Replace("&nbsp;", " ").Trim())));
			row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;", " ").Trim())));


            // SCB - Trying to fix drop down behaviour.  Doesn't save properly.
            //row.Cells[DCC.IndexOf("DefaultAssignedTo")].Controls.Add(WTSUtility.CreateGridDropdownList(_dtResources, "USERNAME", "USERNAME", "WTS_ResourceID", itemId, row.Cells[DCC.IndexOf("DefaultAssignedTo")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultAssignedTo")].Text.Replace("&nbsp;", " ").Trim(), null));
            //row.Cells[DCC.IndexOf("DefaultSME")].Controls.Add(WTSUtility.CreateGridDropdownList(_dtResources, "USERNAME", "USERNAME", "WTS_ResourceID", itemId, row.Cells[DCC.IndexOf("DefaultSME")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultSME")].Text.Replace("&nbsp;", " ").Trim(), null));
            //row.Cells[DCC.IndexOf("DefaultBusinessResource")].Controls.Add(WTSUtility.CreateGridDropdownList(_dtResources, "USERNAME", "USERNAME", "WTS_ResourceID", itemId, row.Cells[DCC.IndexOf("DefaultBusinessResource")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultBusinessResource")].Text.Replace("&nbsp;", " ").Trim(), null));
            //row.Cells[DCC.IndexOf("DefaultTechnicalResource")].Controls.Add(WTSUtility.CreateGridDropdownList(_dtResources, "USERNAME", "USERNAME", "WTS_ResourceID", itemId, row.Cells[DCC.IndexOf("DefaultTechnicalResource")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultTechnicalResource")].Text.Replace("&nbsp;", " ").Trim(), null));


            row.Cells[DCC.IndexOf("DefaultAssignedTo")].Controls.Add(WTSUtility.CreateGridDropdownList("DefaultAssignedTo", itemId, row.Cells[DCC.IndexOf("DefaultAssignedTo")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultAssignedToID")].Text.Replace("&nbsp;", " ").Trim(), 0));
            row.Cells[DCC.IndexOf("DefaultSME")].Controls.Add(WTSUtility.CreateGridDropdownList("DefaultSME", itemId, row.Cells[DCC.IndexOf("DefaultSME")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultSMEID")].Text.Replace("&nbsp;", " ").Trim(), 0));
            row.Cells[DCC.IndexOf("DefaultBusinessResource")].Controls.Add(WTSUtility.CreateGridDropdownList("DefaultBusinessResource", itemId, row.Cells[DCC.IndexOf("DefaultBusinessResource")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultBusinessResourceID")].Text.Replace("&nbsp;", " ").Trim(), 0));
            row.Cells[DCC.IndexOf("DefaultTechnicalResource")].Controls.Add(WTSUtility.CreateGridDropdownList("DefaultTechnicalResource", itemId, row.Cells[DCC.IndexOf("DefaultTechnicalResource")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("DefaultTechnicalResourceID")].Text.Replace("&nbsp;", " ").Trim(), 0));


            row.Cells[DCC.IndexOf("SORT_ORDER")].Controls.Add(WTSUtility.CreateGridTextBox("Sort_Order", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SORT_ORDER")].Text.Replace("&nbsp;", " ").Trim()), true));
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
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("ALLOCATION")].Text.Trim()));
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
	}

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		loadGridData(false);
		//if (HttpContext.Current.Session["dtMD_Allocation"] == null)
		//{
		//	loadGridData();
		//}
		//else
		//{
		//	grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_Allocation"];
		//}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("ALLOCATIONID")
                && i != DCC.IndexOf("ALLOCATIONGROUPID")  // 12902 - 3
                && i != DCC.IndexOf("System_Count")
				&& i != DCC.IndexOf("WorkItem_Count")
				&& i != DCC.IndexOf("DefaultAssignedTo")
				&& i != DCC.IndexOf("DefaultSME")
				&& i != DCC.IndexOf("DefaultBusinessResource")
				&& i != DCC.IndexOf("DefaultTechnicalResource")
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
		row.Cells[DCC.IndexOf("AllocationCategory")].Style["width"] = "125px";
        row.Cells[DCC.IndexOf("AllocationGroup")].Style["width"] = "240px";  // 12902 - 3
        row.Cells[DCC.IndexOf("ALLOCATION")].Style["width"] = "300px";
		row.Cells[DCC.IndexOf("System_Count")].Style["width"] = "60px";
		row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "45px";
		row.Cells[DCC.IndexOf("DefaultAssignedTo")].Style["width"] = "145px";
		row.Cells[DCC.IndexOf("DefaultSME")].Style["width"] = "145px";
		row.Cells[DCC.IndexOf("DefaultBusinessResource")].Style["width"] = "145px";
		row.Cells[DCC.IndexOf("DefaultTechnicalResource")].Style["width"] = "145px";
		row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "40px";
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
			row.Attributes.Add("allocationId", itemId);
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
				else if (i == DCC["AllocationCategory"].Ordinal)
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
                else if (i == DCC["AllocationGroup"].Ordinal) // 12902 - 3
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

    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        formatParent(ref copydt);
        String strName = "Master Grid - Allocation";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        wb.Worksheets.Add();
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[1];
        ws.Name = "Master Grid - Allocation";
        Worksheet wsRaw = wb.Worksheets[0];
        wsRaw.Name = "Allocation Raw";
        int rowCount = 0;
        DataTable Raw = null, parentRaw = null, childRaw = null;
        foreach (DataRow parentRow in copydt.Rows)
        {
            if (parentRow.Field<int>("ALLOCATIONID") != 0)
            {
                WTSUtility.importDataRow(ref parentRaw, parentRow);
                printParentHeader(ws, ref rowCount, copydt.Columns);
                printParent(parentRow, ws, ref rowCount);
                rowCount++;
                printChildRows(parentRow, ws, ref rowCount, ref childRaw);
                rowCount++;
            }
        }

        Raw = WTSUtility.JoinDataTables(parentRaw, childRaw, (row1, row2) =>
                      row1.Field<string>("ALLOCATIONID") == row2.Field<string>("ID"));

        formatRaw(Raw);
        ws.Cells.DeleteColumn(0);
        ws.AutoFitColumns();
        wsRaw.Cells.ImportDataTable(Raw, true, "A1");
        wsRaw.AutoFitColumns();
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
    private void printChildRows(DataRow parentRow, Worksheet ws, ref int rowCount, ref DataTable childRaw)
    {
        DataTable child = null;
        int ID = parentRow.Field<int>("ALLOCATIONID");
        child = MasterData.Allocation_SystemList_Get(allocationID: ID);
        int i = 0, j = 1;
        formatChild(child);
        printChildHeader(ws, ref rowCount, child.Columns);
        foreach (DataRow row in child.Rows)
        {
            WTSUtility.importDataRow(ref childRaw, row);
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

    private void formatRaw(DataTable Raw)
    {
        if (Raw.Columns.Contains("ALLOCATIONID")) Raw.Columns.Remove("ALLOCATIONID");
        if (Raw.Columns.Contains("ID")) Raw.Columns.Remove("ID");
        Raw.AcceptChanges();
    }

    private void formatChild(DataTable child)
    {
        child.Columns.Remove("A");
        child.Columns.Remove("X");
        child.Columns.Remove("Y");
        child.Columns.Remove("CREATEDBY");
        child.Columns.Remove("CREATEDDATE");
        child.Columns.Remove("UPDATEDBY");
        child.Columns.Remove("UPDATEDDATE");
        child.Columns.Remove("Allocation_SystemID");
        child.Columns.Remove("WTS_SYSTEMID");
        child.Columns.Remove("SORT_ORDER");
        child.Columns["ALLOCATIONID"].ColumnName = "ID";
        child.Columns["ARCHIVE"].ColumnName = "Archive";
        child.Columns["ALLOCATION"].ColumnName = "Allocation Assignment";
        child.Columns["WTS_SYSTEM"].ColumnName = "System";
        child.Columns["ProposedPriority"].ColumnName = "Proprosed Priority";
        child.Columns["ApprovedPriority"].ColumnName = "Approved Priority";
        child.Columns["WorkItem_Count"].ColumnName = "Tasks";
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
        dt.Columns.Remove("A");
        dt.Columns.Remove("CREATEDBY");
        dt.Columns.Remove("CREATEDDATE");
        dt.Columns.Remove("UPDATEDBY");
        dt.Columns.Remove("UPDATEDDATE");
        dt.Columns.Remove("AllocationCategoryID");
        // 12902 - 3:
        dt.Columns.Remove("AllocationGroupID");

        dt.Columns.Remove("System_Count");
        dt.Columns.Remove("DefaultAssignedToID");
        dt.Columns.Remove("DefaultSMEID");
        dt.Columns.Remove("DefaultBusinessResourceID");
        dt.Columns.Remove("DefaultTechnicalResourceID");
        dt.Columns.Remove("X");
        dt.Columns["ALLOCATIONID"].SetOrdinal(0);
        dt.AcceptChanges();
        dt.Columns["DESCRIPTION"].ColumnName = "Description";
        dt.Columns["ARCHIVE"].ColumnName = "Archive";
        dt.Columns["AllocationCategory"].ColumnName = "Allocation Category";
        // 12902 - 3:
        dt.Columns["AllocationGroup"].ColumnName = "Allocation Group";
        dt.Columns["ALLOCATION"].ColumnName = "Allocation Assignment";
        dt.Columns["WorkItem_Count"].ColumnName = "Tasks";
        dt.Columns["DefaultAssignedTo"].ColumnName = "Assigned To";
        dt.Columns["DefaultSME"].ColumnName = "SME";
        dt.Columns["DefaultBusinessResource"].ColumnName = "Business Resource";
        dt.Columns["DefaultTechnicalResource"].ColumnName = "Technical Resource";
        dt.Columns["SORT_ORDER"].ColumnName = "Rank";
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

			int id = 0, categoryId = 0, groupId = 0, sortOrder = 0, archive = 0;
			int assignedToID = 0, smeID = 0, busResourceID = 0, techResourceID = 0;
			string allocation = string.Empty, description = string.Empty;

			HttpServerUtility server = HttpContext.Current.Server;
			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = categoryId = sortOrder = archive = 0;
				assignedToID = smeID = busResourceID = techResourceID = 0;
				allocation = description = string.Empty;
				
				tempMsg = string.Empty;
				int.TryParse(dr["AllocationCategory"].ToString(), out categoryId);
                int.TryParse(dr["AllocationGroup"].ToString(), out groupId);
                int.TryParse(dr["ALLOCATIONID"].ToString(), out id);
				allocation = server.UrlDecode(dr["ALLOCATION"].ToString());
				description = server.UrlDecode(dr["DESCRIPTION"].ToString());
				int.TryParse(dr["DefaultAssignedTo"].ToString(), out assignedToID);
				int.TryParse(dr["DefaultSME"].ToString(), out smeID);
				int.TryParse(dr["DefaultBusinessResource"].ToString(), out busResourceID);
				int.TryParse(dr["DefaultTechnicalResource"].ToString(), out techResourceID);
				int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
				int.TryParse(dr["ARCHIVE"].ToString(), out archive);

				if (string.IsNullOrWhiteSpace(allocation))
				{
					tempMsg = "You must specify a value for Allocation.";
					saved = false;
				}
				else
				{
					if (id == 0)
					{
						exists = false;
						saved = MasterData.Allocation_Add(categoryID: categoryId, groupID: groupId, allocation: allocation, description: description
							, defaultAssignedToID: assignedToID, defaultSMEID: smeID, defaultBusinessResourceID: busResourceID, defaultTechnicalResourceID: techResourceID
							, sortOrder: sortOrder, archive: archive == 1, exists: out exists, newID: out id, errorMsg: out tempMsg);
						if (exists)
						{
							saved = false;
							tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Allocation record [" + allocation + "].");
						}
					}
					else
					{
						saved = MasterData.Allocation_Update(id, categoryID: categoryId, groupID: groupId, allocation: allocation, description: description
							, defaultAssignedToID: assignedToID, defaultSMEID: smeID, defaultBusinessResourceID: busResourceID, defaultTechnicalResourceID: techResourceID
							, sortOrder: sortOrder, archive: archive == 1, errorMsg: out tempMsg);
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
				deleted = MasterData.Allocation_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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