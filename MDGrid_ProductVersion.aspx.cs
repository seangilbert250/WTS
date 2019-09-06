﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

public partial class MDGrid_ProductVersion : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();
	DataTable _dtStatus = null;

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfStatusID = 0;
    protected string[] QFSystem = { };
    protected string[] QFContract = { };

	protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

	protected bool CanView = false;
	protected bool CanEdit = false;
	protected bool IsAdmin = false;
    protected bool CanEditWorkloadMGMT = false;

    protected void Page_Load(object sender, EventArgs e)
    {
		this.IsAdmin = UserManagement.UserIsInRole("Admin");
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
		this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.MasterData));
        this.CanEditWorkloadMGMT = UserManagement.UserCanEdit(WTSModuleOption.WorkloadMGMT);

		readQueryString();

		initControls();

        loadQF();

		loadGridData();
    }
    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        String strName = "Master Grid -  Product Version";
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
            if (copydt.Rows[i].Field<int>("ProductVersionID") != 0)
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
        copydt.Columns.Remove("StatusID");
        copydt.Columns.Remove("X");
        copydt.Columns.Remove("CREATEDBY");
        copydt.Columns.Remove("CREATEDDATE");
        copydt.Columns.Remove("UPDATEDBY");
        copydt.Columns.Remove("UPDATEDDATE");
        copydt.Columns.Remove("Status_SORT_ORDER");
        copydt.Columns["DESCRIPTION"].ColumnName = "Description";
        copydt.Columns["ARCHIVE"].ColumnName = "Archive";
        copydt.Columns["SORT_ORDER"].ColumnName = "Sort Order";
        copydt.Columns["WorkItem_Count"].ColumnName = "Total Tasks";
        copydt.Columns["Open_Items"].ColumnName = "Open Tasks";
        copydt.Columns["Closed_Items"].ColumnName = "Closed Tasks";
        copydt.Columns["DefaultSelection"].ColumnName = "Default Selection";
        copydt.Columns["STATUS"].ColumnName = "Status";

        //copydt.Columns["INCLUDEINHOTLIST"].ColumnName = "Include In Hot List";
    }
    private void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}
		if (Request.QueryString["StatusID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["StatusID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["StatusID"].ToString()), out this._qfStatusID);
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
            this.QFSystem = Request.QueryString["SelectedSystems"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["SelectedContracts"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedContracts"]))
        {
            this.QFContract = Request.QueryString["SelectedContracts"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

	private void initControls()
	{
		grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
		grdMD.GridRowDataBound += grdMD_GridRowDataBound;
		grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
	}

    private void loadQF()
    {
        DataTable dtSystem = MasterData.SystemList_Get(includeArchive: true);
        DataTable dtContract = MasterData.ContractList_Get(includeArchive: true);

        HtmlSelect ddlSystem = (HtmlSelect)Page.Master.FindControl("ms_Item0");
        HtmlSelect ddlContract = (HtmlSelect)Page.Master.FindControl("ms_Item1");

        Label lblSystem = (Label)Page.Master.FindControl("lblms_Item0");
        lblSystem.Text = "Suite/System: ";
        lblSystem.Style["width"] = "150px";

        Label lblContract = (Label)Page.Master.FindControl("lblms_Item1");
        lblContract.Text = "Contract: ";
        lblContract.Style["width"] = "150px";

        if (dtSystem != null)
        {
            ddlSystem.Items.Clear();

            foreach (DataRow dr in dtSystem.Rows)
            {
                ListItem li = new ListItem(dr["WTS_SYSTEM"].ToString(), dr["WTS_SystemID"].ToString());
                li.Selected = (QFSystem.Count() == 0 || QFSystem.Contains(dr["WTS_SystemID"].ToString()));
                li.Attributes.Add("OptionGroup", dr["WTS_SystemSuite"].ToString());
                ddlSystem.Items.Add(li);
            }
        }

        if (dtContract != null)
        {
            ddlContract.Items.Clear();

            foreach (DataRow dr in dtContract.Rows)
            {
                ListItem li = new ListItem(dr["CONTRACT"].ToString(), dr["CONTRACTID"].ToString());
                li.Selected = (QFContract.Count() == 0 || QFContract.Contains(dr["CONTRACTID"].ToString()));

                ddlContract.Items.Add(li);
            }
        }
    }

    private void loadGridData()
	{
		_dtStatus = MasterData.StatusList_Get(true);
        _dtStatus.DefaultView.RowFilter = "StatusTypeID IN (0, 9)";
        _dtStatus = _dtStatus.DefaultView.ToTable();

		DataTable dt = null;
		if (_refreshData || Session["dtMD_ProductVersion"] == null)
		{
            HtmlSelect ddlSystem = (HtmlSelect)Page.Master.FindControl("ms_Item0");
            HtmlSelect ddlContract = (HtmlSelect)Page.Master.FindControl("ms_Item1");
            List<string> listSystem = new List<string>();
            List<string> listContract = new List<string>();

            if (ddlSystem != null && ddlSystem.Items.Count > 0)
            {
                foreach (ListItem li in ddlSystem.Items)
                {
                    if (li.Selected) listSystem.Add(li.Value);
                }
            }

            if (ddlContract != null && ddlContract.Items.Count > 0)
            {
                foreach (ListItem li in ddlContract.Items)
                {
                    if (li.Selected) listContract.Add(li.Value);
                }
            }

            dt = MasterData.ProductVersionList_Get(includeArchive: true, qfSystem: String.Join(",", listSystem), qfContract: String.Join(",", listContract));
            HttpContext.Current.Session["dtMD_ProductVersion"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtMD_ProductVersion"];
		}

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
			spanRowCount.InnerText = dt.Rows.Count.ToString();

			DataTable dtCopy = dt.Copy();
			dtCopy = dtCopy.DefaultView.ToTable(true, new string[] { "StatusID", "Status" });
			ddlQF.DataSource = dtCopy;
			ddlQF.DataTextField = "Status";
			ddlQF.DataValueField = "StatusID";
			ddlQF.DataBind();

			ListItem item = ddlQF.Items.FindByValue(_qfStatusID.ToString());
			if (item != null)
			{
				item.Selected = true;
			}

			InitializeColumnData(ref dt);
			dt.AcceptChanges();

			if (_qfStatusID != 0 && dt != null && dt.Rows.Count > 0)
			{
				dt.DefaultView.RowFilter = string.Format(" StatusID =  {0}", _qfStatusID.ToString());
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
                    case "Z":
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                    case "ProductVersion":
						displayName = "Release Version";
						blnVisible = true;
						blnSortable = true;
						break;
					case "STATUS":
						displayName = "Status";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = true;
						break;
                    case "Narrative":
                        blnVisible = true;
                        break;
                    case "StartDate":
                        displayName = "Release Start Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "EndDate":
                        displayName = "Release End Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "WorkItem_Count":
						displayName = "Total Work Tasks";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Open_Items":
						displayName = "Open Work Tasks";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Closed_Items":
						displayName = "Closed Work Tasks";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DefaultSelection":
						displayName = "Default Selection";
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

                    //case "INCLUDEINHOTLIST":
                    //    displayName = "IncludeInHotList";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    break;

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
		string itemId = row.Cells[DCC.IndexOf("ProductVersionID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			bool chked = false;

            row.Cells[DCC.IndexOf("ProductVersion")].Controls.Add(WTSUtility.CreateGridTextBox("ProductVersion", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ProductVersion")].Text.Replace("&nbsp;", " ").Trim())));
			row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;"," ").Trim())));
			row.Cells[DCC.IndexOf("SORT_ORDER")].Controls.Add(WTSUtility.CreateGridTextBox("Sort_Order", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SORT_ORDER")].Text.Replace("&nbsp;", " ").Trim()), true));

            DateTime nDate = new DateTime();
            DateTime.TryParse(Server.HtmlDecode(row.Cells[DCC.IndexOf("StartDate")].Text.Replace("&nbsp;", " ").Trim()), out nDate);
            row.Cells[DCC.IndexOf("StartDate")].Controls.Add(WTSUtility.CreateGridTextBox("StartDate", itemId, nDate.ToString() == "1/1/0001 12:00:00 AM" ? "" : Server.HtmlDecode(String.Format("{0:MM/dd/yyyy}", nDate))));

            DateTime.TryParse(Server.HtmlDecode(row.Cells[DCC.IndexOf("EndDate")].Text.Replace("&nbsp;", " ").Trim()), out nDate);
            row.Cells[DCC.IndexOf("EndDate")].Controls.Add(WTSUtility.CreateGridTextBox("EndDate", itemId, nDate.ToString() == "1/1/0001 12:00:00 AM" ? "" : Server.HtmlDecode(String.Format("{0:MM/dd/yyyy}", nDate))));

            if (row.Cells[DCC.IndexOf("DefaultSelection")].HasControls()
				&& row.Cells[DCC.IndexOf("DefaultSelection")].Controls[0] is CheckBox)
			{
				chked = ((CheckBox)row.Cells[DCC.IndexOf("DefaultSelection")].Controls[0]).Checked;
			}
			else if (row.Cells[DCC.IndexOf("DefaultSelection")].Text == "1")
			{
				chked = true;
			}
			row.Cells[DCC.IndexOf("DefaultSelection")].Controls.Clear();
			row.Cells[DCC.IndexOf("DefaultSelection")].Controls.Add(WTSUtility.CreateGridCheckBox("DefaultSelection", itemId, chked));

			//Status
			DropDownList ddl = WTSUtility.CreateGridDropdownList(_dtStatus, "Status", "Status", "StatusID", itemId, row.Cells[DCC.IndexOf("StatusID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("Status")].Text.Replace("&nbsp;", " ").Trim(), null);
			row.Cells[DCC.IndexOf("Status")].Controls.Add(ddl);

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

            //chked = false;
            //if (row.Cells[DCC.IndexOf("INCLUDEINHOTLIST")].HasControls()
            //    && row.Cells[DCC.IndexOf("INCLUDEINHOTLIST")].Controls[0] is CheckBox)
            //{
            //    chked = ((CheckBox)row.Cells[DCC.IndexOf("INCLUDEINHOTLIST")].Controls[0]).Checked;
            //}
            //else if (row.Cells[DCC.IndexOf("INCLUDEINHOTLIST")].Text == "1")
            //{
            //    chked = true;
            //}
            //row.Cells[DCC.IndexOf("INCLUDEINHOTLIST")].Controls.Clear();
            //row.Cells[DCC.IndexOf("INCLUDEINHOTLIST")].Controls.Add(WTSUtility.CreateGridCheckBox("Include In Hot List", itemId, chked));

        }

        TextBox obj = WTSUtility.CreateGridTextBox("Narrative", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Narrative")].Text.Replace("&nbsp;", " ").Trim()), false, true);
        if (!CanView)
        {
            obj.ReadOnly = true;
            obj.ForeColor = System.Drawing.Color.Gray;
        }
        row.Cells[DCC.IndexOf("Narrative")].Controls.Add(obj);

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
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("ProductVersion")].Text.Trim()));
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
            divChildCount.InnerText = string.Format("({0})", row.Cells[DCC.IndexOf("Session_Count")].Text);
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
		if (HttpContext.Current.Session["dtMD_ProductVersion"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_ProductVersion"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("Status")
				&& i != DCC.IndexOf("WorkItem_Count")
				&& i != DCC.IndexOf("Open_Items")
				&& i != DCC.IndexOf("Closed_Items") 
				&& i != DCC.IndexOf("DefaultSelection")
				&& i != DCC.IndexOf("SORT_ORDER")
				&& i != DCC.IndexOf("ARCHIVE"))
                //&& i != DCC.IndexOf("INCLUDEINHOTLIST"))
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
		row.Cells[DCC.IndexOf("Z")].Style["width"] = "32px";
		row.Cells[DCC.IndexOf("ProductVersion")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("DESCRIPTION")].Style["width"] = "400px";
        row.Cells[DCC.IndexOf("StartDate")].Style["width"] = "70px";
        row.Cells[DCC.IndexOf("EndDate")].Style["width"] = "70px";
        row.Cells[DCC.IndexOf("Status")].Style["width"] = "100px";
		row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "70px";
		row.Cells[DCC.IndexOf("Open_Items")].Style["width"] = "70px";
		row.Cells[DCC.IndexOf("Closed_Items")].Style["width"] = "70px";
		row.Cells[DCC.IndexOf("DefaultSelection")].Style["width"] = "55px";
		row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "55px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";

        //row.Cells[DCC.IndexOf("INCLUDEINHOTLIST")].Style["width"] = "85px";
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
            row.Attributes.Add("ProductVersionID", itemId);
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
                else if (i == DCC["ProductVersion"].Ordinal)
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
        childFrame.Attributes.Add("ProductVersionID", itemId);
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

			int id = 0, defaultSelection = 0, sortOrder = 0, statusId = 0, archive = 0;
			string productVersion = string.Empty, description = string.Empty, narrative = string.Empty, startDate = string.Empty, endDate = string.Empty;

            HttpServerUtility server = HttpContext.Current.Server;
			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
				defaultSelection = 0;
				sortOrder = 0;
				statusId = 0;
				productVersion = string.Empty;
				description = string.Empty;
                narrative = string.Empty;
                startDate = string.Empty;
                endDate = string.Empty;
                archive = 0;

				tempMsg = string.Empty;
				int.TryParse(dr["ProductVersionID"].ToString(), out id);
				productVersion = server.UrlDecode(dr["ProductVersion"].ToString());
				description = server.UrlDecode(dr["DESCRIPTION"].ToString());
                narrative = server.UrlDecode(dr["Narrative"].ToString());
                startDate = server.UrlDecode(dr["StartDate"].ToString());
                endDate = server.UrlDecode(dr["EndDate"].ToString());
                int.TryParse(dr["DefaultSelection"].ToString(), out defaultSelection);
				int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
				int.TryParse(dr["STATUS"].ToString(), out statusId);
				int.TryParse(dr["ARCHIVE"].ToString(), out archive);

				if (string.IsNullOrWhiteSpace(productVersion))
				{
                    tempMsg = "You must specify a Product Version.";
                    saved = false;
                }
				else if(statusId == 0)
				{
					tempMsg = "You must specify a value for Status.";
					saved = false;
				}
				else
				{
					if (id == 0)
					{
						exists = false;
						saved = MasterData.ProductVersion_Add(productVersion, description, narrative, startDate, endDate, (defaultSelection == 1), sortOrder, statusId, (archive == 1), out exists, out id, out tempMsg);
						if (exists)
						{
							saved = false;
							tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Product Version record [" + productVersion + "].");
						}
					}
					else
					{
						saved = MasterData.ProductVersion_Update(id, productVersion, description, narrative, startDate, endDate, (defaultSelection == 1), sortOrder, statusId, (archive == 1), out tempMsg);
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
				deleted = MasterData.ProductVersion_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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