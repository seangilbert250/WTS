using System;
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
using System.Drawing;
using System.Web.UI.HtmlControls;

public partial class MDGrid_WorkType : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfWorkTypeID = 0;

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

	private void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}
		if (Request.QueryString["WorkTypeID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["WorkTypeID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["WorkTypeID"].ToString()), out this._qfWorkTypeID);
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

    #region Excel
    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        formatParent(ref copydt);
        String strName = "Master Grid - Resource Group";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[0];
        int rowCount = 0;
        foreach (DataRow parentRow in copydt.Rows)
        {
            if (parentRow.Field<int>("WorkTypeID") != 0)
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
        //DataTable Phases = null;
        DataTable Statuses = null;
        int ID = parentRow.Field<int>("WorkTypeID");
        //Phases = MasterData.WorkType_PhaseList_Get(workTypeID: ID);
        Statuses = MasterData.WorkType_StatusList_Get(workTypeID: ID);
        int i = 0, j = 1;
        //formatPhases(Phases);
        formatStatuses(Statuses);
        //printChildHeader(ws, ref rowCount, Phases.Columns, System.Drawing.ColorTranslator.FromHtml("lightBlue"));
        //foreach(DataRow row in Phases.Rows)
        //{
        //    j = 2;
        //    foreach(object value in row.ItemArray)
        //    {
        //        ws.Cells[rowCount + i, j].PutValue(value);
        //        j++;
        //    }
        //    i++;
        //}
        //rowCount += Phases.Rows.Count;
        printChildHeader(ws, ref rowCount, Statuses.Columns, System.Drawing.ColorTranslator.FromHtml("lightGreen"));
        i = 0; j = 0;
        foreach (DataRow row in Statuses.Rows)
        {
            j = 2;
            foreach (object value in row.ItemArray)
            {
                ws.Cells[rowCount + i, j].PutValue(value);
                j++;
            }
            i++;
        }
        rowCount += Statuses.Rows.Count;
    }

    private void formatStatuses(DataTable statuses)
    {
        statuses.Columns.Remove("X");
        statuses.Columns.Remove("Status_WorkTypeID");
        statuses.Columns.Remove("StatusID");
        statuses.Columns.Remove("Status_SORT_ORDER");
        statuses.Columns.Remove("WorkTypeID");
        statuses.Columns.Remove("WorkType_SORT_ORDER");
        statuses.Columns["WorkType"].ColumnName = "Resource Group";
        statuses.Columns["ARCHIVE"].ColumnName = "Archive";
        statuses.Columns["DESCRIPTION"].ColumnName = "Description";
        statuses.Rows[0].Delete();
        statuses.AcceptChanges();

    }

    private void formatPhases(DataTable Phases)
    {
        Phases.Columns.Remove("X");
        Phases.Columns.Remove("PDDTDR_PHASEID");
        Phases.Columns.Remove("Phase_SORT_ORDER");
        Phases.Columns.Remove("WorkType_PHASEID");
        Phases.Columns.Remove("WorkType_SORT_ORDER");
        Phases.Columns.Remove("WorkTypeID");
        Phases.Columns["PDDTDR_PHASE"].ColumnName = "Phase";
        Phases.Columns["WorkType"].ColumnName = "Resource Group";
        Phases.Columns["DESCRIPTION"].ColumnName = "Description";
        Phases.Columns["ARCHIVE"].ColumnName = "Archive";
        Phases.Rows[0].Delete();
        Phases.AcceptChanges();
    }

    private void printChildHeader(Worksheet ws, ref int rowCount, DataColumnCollection columns, Color c)
    {
        if(object.ReferenceEquals(columns, null) || columns.Count < 1) { return; }
        Aspose.Cells.Style style = new Aspose.Cells.Style();
        style.Pattern = BackgroundType.Solid;
        style.ForegroundColor = c;
        for (int i = 0; i < columns.Count; i++)
        {
            ws.Cells[rowCount, i+2].PutValue(columns[i].ColumnName);
            ws.Cells[rowCount, i+2].SetStyle(style);
        }
        rowCount++;
    }

    private void printParent(DataRow parentRow, Worksheet ws, ref int rowCount)
    {
        int i = 0;
        foreach(object value in parentRow.ItemArray)
        {
            ws.Cells[rowCount, i].PutValue(value);
            i++;        
        }
        rowCount++;
    }

    private static void formatParent(ref DataTable dt)
    {
        dt.Columns.Remove("X");
        dt.Columns.Remove("CREATEDDATE");
        dt.Columns.Remove("CREATEDBY");
        dt.Columns.Remove("UPDATEDBY");
        dt.Columns.Remove("UPDATEDDATE");
        dt.Columns.Remove("Phase_Count");
        dt.Columns["WorkType"].ColumnName = "Resource Group";
        //dt.Columns["Phase_Count"].ColumnName = "Phases";
        dt.Columns["Status_Count"].ColumnName = "Statuses";
        dt.Columns["WorkItem_Count"].ColumnName = "Work Items";
        dt.Columns["SORT_ORDER"].ColumnName = "Sort Order";
        dt.Columns["ARCHIVE"].ColumnName = "Archive";
    }

    #endregion

    private void initControls()
	{
		grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
		grdMD.GridRowDataBound += grdMD_GridRowDataBound;
		grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
	}


	private void loadGridData()
	{
		DataTable dt = null;
		if (_refreshData || Session["dtMD_WorkType"] == null)
		{
			dt = MasterData.WorkTypeList_Get(includeArchive: true);
			HttpContext.Current.Session["dtMD_WorkType"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtMD_WorkType"];
		}

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
			spanRowCount.InnerText = dt.Rows.Count.ToString();

			ddlQF.DataSource = dt;
			ddlQF.DataTextField = "WorkType";
			ddlQF.DataValueField = "WorkTypeID";
			ddlQF.DataBind();

			ListItem item = ddlQF.Items.FindByValue(_qfWorkTypeID.ToString());
			if (item != null)
			{
				item.Selected = true;
			}

			InitializeColumnData(ref dt);
			dt.AcceptChanges();

			if (_qfWorkTypeID != 0 && dt != null && dt.Rows.Count > 0)
			{
				dt.DefaultView.RowFilter = string.Format(" WorkTypeID =  {0}", _qfWorkTypeID.ToString());
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
                    case "Y":
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                    case "X":
						displayName = "&nbsp;";
						blnVisible = true;
						break;
					case "WorkType":
						displayName = "Resource Group";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Description":
						displayName = "Description";
						blnVisible = true;
						break;
					//case "Phase_Count":
					//	displayName = "Phases";
					//	blnVisible = true;
					//	blnSortable = true;
					//	break;
					case "Status_Count":
						displayName = "Statuses";
						blnVisible = true;
						blnSortable = true;
						break;
                    case "ResourceType_Count":
                        displayName = "Resource Types";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Organization_Count":
                        displayName = "Organizations";
                        blnVisible = true;
                        blnSortable = true;
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
		string itemId = row.Cells[DCC.IndexOf("WorkTypeID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			row.Cells[DCC.IndexOf("WorkType")].Controls.Add(WTSUtility.CreateGridTextBox("WorkType", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkType")].Text.Trim())));
			row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Trim())));
			row.Cells[DCC.IndexOf("Phase_Count")].Controls.Add(createIntersectLink(itemId, row.Cells[DCC.IndexOf("Phase_Count")].Text.Replace("&nbsp;", " ").Trim(), "Phases"));
			row.Cells[DCC.IndexOf("Status_Count")].Controls.Add(createIntersectLink(itemId, row.Cells[DCC.IndexOf("Status_Count")].Text.Replace("&nbsp;", " ").Trim(), "Statuses")); 
			row.Cells[DCC.IndexOf("ResourceType_Count")].Controls.Add(createIntersectLink(itemId, row.Cells[DCC.IndexOf("ResourceType_Count")].Text.Replace("&nbsp;", " ").Trim(), "ResourceTypes")); 
			row.Cells[DCC.IndexOf("Organization_Count")].Controls.Add(createIntersectLink(itemId, row.Cells[DCC.IndexOf("Organization_Count")].Text.Replace("&nbsp;", " ").Trim(), "Organizations"));
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

        string dependencies;
        int count = 0;
        dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkType_WTS_RESOURCE_Count")].Text).Trim();
        int.TryParse(dependencies, out count);

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
            divChildCount.InnerText = string.Format("({0})", count.ToString());
            divChildCount.Style["display"] = "table-cell";
            divChildCount.Style["padding-left"] = "2px";
            divChildren.Controls.Add(divChildrenButtons);
            divChildren.Controls.Add(divChildCount);
            //buttons to show/hide child grid
            row.Cells[DCC["Y"].Ordinal].Controls.Clear();
            row.Cells[DCC["Y"].Ordinal].Controls.Add(divChildren);

            //add child grid row for Task Items
            Table table = (Table)row.Parent;
            GridViewRow childRow = createChildRow(itemId);
            table.Rows.AddAt(table.Rows.Count, childRow);
        }
        else
		{
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WorkType")].Text.Trim()));
		}
	}

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_WorkType"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkType"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("Y")
				&& i != DCC.IndexOf("WorkTypeID")
                && i != DCC.IndexOf("Phase_Count")
				&& i != DCC.IndexOf("Status_Count")
				&& i != DCC.IndexOf("ResourceType_Count")
				&& i != DCC.IndexOf("Organization_Count")
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
		row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
		row.Cells[DCC.IndexOf("Y")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("WorkType")].Style["width"] = "150px";
		row.Cells[DCC.IndexOf("Phase_Count")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("Status_Count")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("ResourceType_Count")].Style["width"] = "105px";
		row.Cells[DCC.IndexOf("Organization_Count")].Style["width"] = "85px";
        row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "85px";
		row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
	}


	LinkButton createIntersectLink(string itemId = "", string item = "", string intersectTo = "Phases")
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

    System.Web.UI.WebControls.Image createShowHideButton(bool show = false, string direction = "Show", string itemId = "ALL")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgShowHideChildren_click(this,'{0}','{1}');", direction, itemId);

        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
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
        img.ToolTip = string.Format("{0} Resources for [{1}]", direction, itemId);
        img.AlternateText = string.Format("{0} Resources for [{1}]", direction, itemId);
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
            row.Attributes.Add("WorkTypeID", itemId);
            row.Attributes.Add("Name", string.Format("gridChild_{0}", itemId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC["Y"].Ordinal)
                {
                    //set width to match parent
                    tableCell.Style["width"] = "32px";
                    tableCell.Style["border-right"] = "1px solid transparent";
                }
                else if (i == DCC["WorkType"].Ordinal)
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
        childFrame.Attributes.Add("workTypeId", itemId);
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
			string WorkType = string.Empty, description = string.Empty;

			HttpServerUtility server = HttpContext.Current.Server;
			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
				sortOrder = 0;
				WorkType = string.Empty;
				description = string.Empty;
				archive = 0;

				tempMsg = string.Empty;
				int.TryParse(dr["WorkTypeID"].ToString(), out id);
				WorkType = server.UrlDecode(dr["WorkType"].ToString());
				description = server.UrlDecode(dr["Description"].ToString());
				int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
				int.TryParse(dr["ARCHIVE"].ToString(), out archive);

				if (string.IsNullOrWhiteSpace(WorkType))
				{
					tempMsg = "You must specify a value for Resource Group.";
					saved = false;
				}
				else
				{
					if (id == 0)
					{
						exists = false;
						saved = MasterData.WorkType_Add(WorkType, description, sortOrder, archive == 1, out exists, out id, out tempMsg);
						if (exists)
						{
							saved = false;
							tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate WorkType record [" + WorkType + "].");
						}
					}
					else
					{
						saved = MasterData.WorkType_Update(id, WorkType, description, sortOrder, archive == 1, out tempMsg);
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
    public static string SaveResource()
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            //save
            saved = MasterData.WorkType_Resource_Sync(exists: out exists, errorMsg: out tempMsg);

            if (tempMsg.Length > 0)
            {
                errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
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

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WORK_AREA);

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
				deleted = MasterData.WorkType_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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