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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using Aspose.Cells;
using System.IO;

public partial class MDGrid_Contract : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();
	protected DataTable _dtContractType = null;
    protected DataTable _dtCRReportViews = null;

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfContractTypeID = 0;

	protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

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

    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        String strName = "Master Grid - Contract";
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
            if (copydt.Rows[i].Field<int>("ContractID") != 0)
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
        copydt.Columns.Remove("Z");
        copydt.Columns.Remove("ContractTypeID");
        copydt.Columns.Remove("X");
        copydt.Columns.Remove("CREATEDBY");
        copydt.Columns.Remove("CREATEDDATE");
        copydt.Columns.Remove("UPDATEDBY");
        copydt.Columns.Remove("UPDATEDDATE");
        copydt.Columns["ContractType"].ColumnName = "Contract Type";
        copydt.Columns["DESCRIPTION"].ColumnName = "Description";
        copydt.Columns["WorkRequest_Count"].ColumnName = "Work Requests";
        copydt.Columns["ARCHIVE"].ColumnName = "Archive";
        copydt.Columns["SORT_ORDER"].ColumnName = "Sort Order";
    }

    private void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}
		if (Request.QueryString["ContractID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["ContractID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["ContractID"].ToString()), out this._qfContractTypeID);
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

        if (!string.IsNullOrWhiteSpace(Request.QueryString["ChildView"]))
        {
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
                default:
                    cvValue = "0";
                    break;
            }
        }

        ddlChildView.Items.FindByValue(cvValue).Selected = true;
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
		if (_refreshData || Session["dtMD_Contract"] == null)
		{
			dt = MasterData.ContractList_Get(includeArchive: true);
			HttpContext.Current.Session["dtMD_Contract"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtMD_Contract"];
		}
		_dtContractType = MasterData.ContractTypeList_Get(includeArchive: false);
        _dtCRReportViews = Filtering.LoadReportViews(-1, 2);

        if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			InitializeColumnData(ref dt);
			dt.AcceptChanges();

			using (DataTable dtTemp = dt.DefaultView.ToTable(true, new string[] { "ContractTypeID", "ContractType" }))
			{
				if (dtTemp != null)
				{
					ddlQF.DataSource = dtTemp;
					ddlQF.DataTextField = "ContractType";
					ddlQF.DataValueField = "ContractTypeID";
					ddlQF.DataBind();
				}

				ListItem item = ddlQF.Items.FindByValue(_qfContractTypeID.ToString());
				if (item != null)
				{
					item.Selected = true;
				}
			}

			if (_qfContractTypeID != 0 && dt != null && dt.Rows.Count > 0)
			{
				dt.DefaultView.RowFilter = string.Format(" ContractTypeID =  {0}", _qfContractTypeID.ToString());
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
                    case "Z":
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                    case "X":
						displayName = "&nbsp;";
						blnVisible = true;
						break;
					case "Contract":
						displayName = "Contract";
						blnVisible = true;
						blnSortable = true;
						break;
					case "ContractType":
						displayName = "Contract Type";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = true;
						break;
                    case "CRReportViews":
                        displayName = "CR Report Views";
                        blnVisible = true;
                        break;
                    //case "WorkRequest_Count":
                    //	displayName = "Work Requests";
                    //	blnVisible = true;
                    //	blnSortable = true;
                    //	break;
                    case "CRREPORTLASTRUNBY":
                        displayName = "Last CR Report Run By";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "CRREPORTLASTRUNDATE":
                        displayName = "Date of Last CR Report";
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
		string itemId = row.Cells[DCC.IndexOf("ContractID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			row.Cells[DCC.IndexOf("ContractType")].Controls.Add(WTSUtility.CreateGridDropdownList(_dtContractType, "ContractType", "ContractType", "ContractTypeID", itemId, row.Cells[DCC.IndexOf("ContractTypeID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("ContractType")].Text.Replace("&nbsp;", " ").Trim(), null));
			row.Cells[DCC.IndexOf("Contract")].Controls.Add(WTSUtility.CreateGridTextBox("Contract", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Contract")].Text.Trim())));
			row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Trim())));
            DataTable curViews = _dtCRReportViews.Clone();
            curViews.Clear();
            string viewName = "-", viewID = "-1";
            foreach (DataRow views in _dtCRReportViews.Rows)
            {
                string param = views["ReportParameters"].ToString();
                if (param.Contains(row.Cells[DCC.IndexOf("Contract")].Text.Replace("&nbsp;", " ").Trim()))
                {
                    curViews.Rows.Add(views.ItemArray);
                    if (viewName == "-") viewName = views["ViewName"].ToString();
                    if (viewID == "-1") viewID = views["UserReportViewID"].ToString();
                }
            }
            row.Cells[DCC.IndexOf("CRReportViews")].Controls.Add(WTSUtility.CreateGridDropdownList(curViews, "ViewName", "ViewName", "UserReportViewID", itemId, viewID, viewName, null));

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

		string dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkRequest_Count")].Text).Trim();
		int count = 0;

        string childCounts;
        int childCount = 0;
        if (ddlChildView.SelectedValue == "0")
        {
            childCounts = Server.HtmlDecode(row.Cells[DCC.IndexOf("System_Count")].Text).Trim();
            int.TryParse(childCounts, out childCount);
        }
        else if (ddlChildView.SelectedValue == "1")
        {
            childCounts = Server.HtmlDecode(row.Cells[DCC.IndexOf("Narrative_Count")].Text).Trim();
            int.TryParse(childCounts, out childCount);
        }
        else if (ddlChildView.SelectedValue == "2")
        {
            childCounts = Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkloadAllocation_Count")].Text).Trim();
            int.TryParse(childCounts, out childCount);
        }

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
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("Contract")].Text.Trim()));
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
            divChildCount.InnerText = string.Format("({0})", childCount.ToString());
            divChildCount.Style["display"] = "table-cell";
            divChildCount.Style["padding-left"] = "2px";
            divChildren.Controls.Add(divChildrenButtons);
            divChildren.Controls.Add(divChildCount);
            //buttons to show/hide child grid
            row.Cells[DCC["Z"].Ordinal].Controls.Clear();
            row.Cells[DCC["Z"].Ordinal].Controls.Add(divChildren);

            //add child grid row for Task Items
            Table table = (Table)row.Parent;
            GridViewRow childRow = createChildRow(itemId);
            table.Rows.AddAt(table.Rows.Count, childRow);
        }
    }

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_Contract"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_Contract"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("ContractID")
				&& i != DCC.IndexOf("WorkRequest_Count")
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
        row.Cells[DCC.IndexOf("Z")].Style["width"] = "35px";
        row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
		row.Cells[DCC.IndexOf("ContractType")].Style["width"] = "135px";
		row.Cells[DCC.IndexOf("Contract")].Style["width"] = "300px";
		row.Cells[DCC.IndexOf("CRReportViews")].Style["width"] = "175px";
        row.Cells[DCC.IndexOf("WorkRequest_Count")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("CRREPORTLASTRUNBY")].Style["width"] = "145px";
        row.Cells[DCC.IndexOf("CRREPORTLASTRUNDATE")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
	}


	LinkButton createEditLink(string itemId = "", string item = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEdit_click('{0}');return false;", itemId);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEdit_{0}", itemId);
		lb.Attributes["name"] = string.Format("lbEdit_{0}", itemId);
		lb.ToolTip = string.Format("Edit Item [{0}]", item);
		lb.Text = item;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
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
        //img.ToolTip = string.Format("{0} Systems for [{1}]", direction, itemId);
        //img.AlternateText = string.Format("{0} Systems for [{1}]", direction, itemId);
        img.ToolTip = string.Format("{0} Systems", direction);
        img.AlternateText = string.Format("{0} Systems", direction);
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
            row.Attributes.Add("contractID", itemId);
            row.Attributes.Add("Name", string.Format("gridChild_{0}", itemId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC["Z"].Ordinal)
                {
                    //set width to match parent
                    tableCell.Style["width"] = "32px";
                    tableCell.Style["border-right"] = "1px solid transparent";
                }
                else if (i == DCC["ContractType"].Ordinal)
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
        childFrame.Attributes.Add("contractId", itemId);
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

			int id = 0, typeId = 0, sortOrder = 0, archive = 0;
			string Contract = string.Empty, description = string.Empty;

			HttpServerUtility server = HttpContext.Current.Server;
			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
				typeId = 0;
				sortOrder = 0;
				Contract = string.Empty;
				description = string.Empty;
				archive = 0;

				tempMsg = string.Empty;
				int.TryParse(dr["ContractID"].ToString(), out id);
				int.TryParse(dr["ContractType"].ToString(), out typeId);
				Contract = server.UrlDecode(dr["Contract"].ToString());
				description = server.UrlDecode(dr["DESCRIPTION"].ToString());
				int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
				int.TryParse(dr["ARCHIVE"].ToString(), out archive);

				if (string.IsNullOrWhiteSpace(Contract))
				{
					tempMsg = "You must specify a value for Contract.";
					saved = false;
				}
				else
				{
					if (id == 0)
					{
						exists = false;
						saved = MasterData.Contract_Add(typeId, Contract, description, sortOrder, archive == 1, out exists, out id, out tempMsg);
						if (exists)
						{
							saved = false;
							tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Contract record [" + Contract + "].");
						}
					}
					else
					{
						saved = MasterData.Contract_Update(id, typeId, Contract, description, sortOrder, archive == 1, out tempMsg);
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
				deleted = MasterData.Contract_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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