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
using System.Linq;

public partial class MDGrid_WorkArea : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfID = 0;
    protected string[] QFSystem = { };
    protected int _qfSystemSuiteID = 0;
    protected int CurrentLevel = 0;

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
        loadQF();

        loadGridData();
	}

	private void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}
		if (Request.QueryString["QFID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["QFID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["QFID"].ToString()), out this._qfID);
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
        if (Request.QueryString["SelectedSystems"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSystems"]))
        {
            this.QFSystem = Request.QueryString["SelectedSystems"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
        DataTable dtSystem = MasterData.SystemList_Get(WTS_SYSTEM_SUITEID: _qfSystemSuiteID);

        HtmlSelect ddlSystem = (HtmlSelect)Page.Master.FindControl("ms_Item0");

        Label lblSystem = (Label)Page.Master.FindControl("lblms_Item0");
        lblSystem.Text = "Suite/System: ";
        lblSystem.Style["width"] = "150px";

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
    }

        private void loadGridData()
	{

        HtmlSelect ddlSystem = (HtmlSelect)Page.Master.FindControl("ms_Item0");
        List<string> listSystem = new List<string>();

        if (ddlSystem != null && ddlSystem.Items.Count > 0)
        {
            foreach (ListItem li in ddlSystem.Items)
            {
                if (li.Selected) listSystem.Add(li.Value);
            }
        }

        DataTable dt = MasterData.WorkAreaList_Get(includeArchive: true, systemSuiteID: _qfSystemSuiteID, systemIDs: String.Join(",", listSystem));
		HttpContext.Current.Session["dtMD_WorkArea"] = dt;

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			InitializeColumnData(ref dt);
			dt.AcceptChanges();
		}

		//if (_qfID != 0 && dt != null && dt.Rows.Count > 0)
		//{
		//	dt.DefaultView.RowFilter = string.Format(" WorkAreaID =  {0}", _qfID.ToString());
		//	dt = dt.DefaultView.ToTable();
		//}
		int count = dt.Rows.Count;
		count = count > 0 ? count - 1 : count; //need to subtract the empty row
		spanRowCount.InnerText = count.ToString();

        if (CurrentLevel > 0)
        {

            DataTable dtWorkAreas = MasterData.WorkAreaList_Get();
            string workAreaIDs = string.Empty;
            if (dt != null && dt.Rows.Count > 1)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    if (dr["WorkAreaID"].ToString() != "0") workAreaIDs += dr["WorkAreaID"].ToString() + ",";
                }
                dtWorkAreas.DefaultView.RowFilter = "WorkAreaID NOT IN (" + workAreaIDs + ")";
                dtWorkAreas = dtWorkAreas.DefaultView.ToTable();
            }

            ddlWorkArea.DataSource = dtWorkAreas;
            ddlWorkArea.DataValueField = "WorkAreaID";
            ddlWorkArea.DataTextField = "WorkArea";
            ddlWorkArea.DataBind();
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
					case "WorkArea":
						displayName = "Work Area";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Description":
						displayName = "Description";
						blnVisible = true;
						break;
					case "WorkItem_Count":
						displayName = "Primary Tasks";
						blnVisible = true;
						blnSortable = true;
						break;
					case "ProposedPriorityRank":
						displayName = "Proposed Priority";
						blnVisible = true;
						blnSortable = true;
						break;
					case "ActualPriorityRank":
						displayName = "Approved Priority";
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
		string itemId = row.Cells[DCC.IndexOf("WorkAreaID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			row.Cells[DCC.IndexOf("WorkArea")].Controls.Add(WTSUtility.CreateGridTextBox("Allocation", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkArea")].Text.Replace("&nbsp;", " ").Trim())));
			row.Cells[DCC.IndexOf("Description")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Description")].Text.Replace("&nbsp;", " ").Trim())));
			row.Cells[DCC.IndexOf("ProposedPriorityRank")].Controls.Add(WTSUtility.CreateGridTextBox("Sort_Order", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("ProposedPriorityRank")].Text.Replace("&nbsp;", " ").Trim()), true));
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
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WorkArea")].Text.Trim()));
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
		if (HttpContext.Current.Session["dtMD_WorkArea"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkArea"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("WorkAreaID")
				&& i != DCC.IndexOf("WorkItem_Count")
				&& i != DCC.IndexOf("ProposedPriorityRank")
				&& i != DCC.IndexOf("ActualPriorityRank")
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
		row.Cells[DCC.IndexOf("WorkArea")].Style["width"] = "300px";
		row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "85px";
		row.Cells[DCC.IndexOf("ProposedPriorityRank")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("ActualPriorityRank")].Style["width"] = "75px";
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
			row.Attributes.Add("workAreaId", itemId);
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
				else if (i == DCC["WorkArea"].Ordinal)
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
				if (cols.ItemByColumnName(col.ColumnName) == null && col.ColumnName != "WorkAreaID" && col.ColumnName != " ")
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
		if (dt.Columns.Contains("WorkAreaID")) dt.Columns["WorkAreaID"].SetOrdinal(dt.Columns.Count - 1);

		dt.AcceptChanges();
	}
	private bool ExportExcel(DataTable dt)
	{
		bool success = false;
		string errorMsg = string.Empty;

		try
		{
			Workbook wb = new Workbook(FileFormatType.Xlsx);
			Worksheet ws = wb.Worksheets[0];
			StyleFlag flag = new StyleFlag() { All = true };
			Aspose.Cells.Style style = new Aspose.Cells.Style();
			Aspose.Cells.Style style2 = new Aspose.Cells.Style();

			style.Pattern = BackgroundType.Solid;
			style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
			style2.Pattern = BackgroundType.Solid;
			style2.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightBlue");

			DataTable dtExcel = new DataTable();

			FormatExcelRows(ref dt);
			RemoveExcelColumns(ref dt);
			RenameExcelColumns(ref dt);
			dtExcel = dt.Clone();

			for (int i = 0; i <= dtExcel.Columns.Count - 1; i++)
			{
				dtExcel.Columns[i].DataType = typeof(string);
				dtExcel.Columns[i].AllowDBNull = true;
			}

			foreach (DataRow dr in dt.Rows)
			{
				int workAreaID = 0;
				int.TryParse(dr["WorkAreaID"].ToString(), out workAreaID);

				if (workAreaID > 0)
				{
					dtExcel.ImportRow(dr);
					DataTable dtDrilldown = MasterData.WorkArea_SystemList_Get(workAreaID: workAreaID);

					if (dtDrilldown != null && dtDrilldown.Rows.Count > 1)
					{
						int count = 0;
						foreach (DataRow drChild in dtDrilldown.Rows)
						{
							if (count == 0)
							{
								DataRow drSpacer = dtExcel.NewRow();
								drSpacer[0] = "spacer";

								dtExcel.Rows.Add(drSpacer);
								dtExcel.Rows.Add("", "Work Area", "System", "Description", "Proposed Priority", "Approved Priority", "Tasks", "Archive");
							}

							int drilldownWorkAreaID = 0;
							int.TryParse(drChild["WorkAreaID"].ToString(), out drilldownWorkAreaID);
							if (drilldownWorkAreaID > 0) dtExcel.Rows.Add("", drChild["WorkArea"].ToString(), drChild["WTS_SYSTEM"].ToString(), drChild["DESCRIPTION"].ToString(), drChild["ProposedPriority"].ToString(), drChild["ApprovedPriority"].ToString(), drChild["WorkItem_Count"].ToString(), (drChild["ARCHIVE"].ToString() == "1" ? "Yes" : "No"));

							if (count == dtDrilldown.Rows.Count - 1)
							{
								DataRow drSpacer = dtExcel.NewRow();
								drSpacer[0] = "spacer";
								dtExcel.Rows.Add(drSpacer);
							}

							count++;
						}
					}
				}
			}

			if (dtExcel.Columns.Contains("WorkAreaID")) dtExcel.Columns.Remove("WorkAreaID");

			string name = "Master Grid - Work Area";
			ws.Cells.ImportDataTable(dtExcel, true, 0, 0, false, false);

			for (int j = 0; j <= ws.Cells.Rows.Count - 1; j++)
			{
				if (ws.Cells.Rows[j][0].Value == "spacer")
				{
					ws.Cells.Rows[j][0].Value = "";
					Range spacer = ws.Cells.CreateRange(j, 0, 1, 8);
					spacer.Merge();
				}

				if (ws.Cells.Rows[j][0].Value == "Work Area")
				{
					Range range = ws.Cells.CreateRange(j, 0, 1, 7);
					range.ApplyStyle(style, flag);
				}

				if (ws.Cells.Rows[j][1].Value == "Work Area")
				{
					Range range = ws.Cells.CreateRange(j, 1, 1, 7);
					range.ApplyStyle(style2, flag);
				}
			}

			ws.AutoFitColumns();
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

			int id = 0, proposedPriorityRank = 0, approvedPriorityRank = 0, archive = 0;
			string WorkArea = string.Empty, description = string.Empty;

			HttpServerUtility server = HttpContext.Current.Server;
			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
				proposedPriorityRank = 0;
				approvedPriorityRank = 0;
				WorkArea = string.Empty;
				description = string.Empty;
				archive = 0;

				tempMsg = string.Empty;
				int.TryParse(dr["WorkAreaID"].ToString(), out id);
				WorkArea = server.UrlDecode(dr["WorkArea"].ToString());
				description = server.UrlDecode(dr["Description"].ToString());
				int.TryParse(dr["ProposedPriorityRank"].ToString(), out proposedPriorityRank);
				int.TryParse(dr["ActualPriorityRank"].ToString(), out approvedPriorityRank);
				int.TryParse(dr["ARCHIVE"].ToString(), out archive);

				if (string.IsNullOrWhiteSpace(WorkArea))
				{
					tempMsg = "You must specify a value for Allocation.";
					saved = false;
				}
				else
				{
					if (id == 0)
					{
						exists = false;
						saved = MasterData.WorkArea_Add(WorkArea: WorkArea, description: description, proposedPriorityRank: proposedPriorityRank, exists: out exists, newID: out id, errorMsg: out tempMsg);
						if (exists)
						{
							saved = false;
							tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Allocation record [" + WorkArea + "].");
						} else
                        {
                            if (systemSuiteID > 0)
                            {
                                DataTable dtSystem = MasterData.SystemList_Get(includeArchive: true);
                                dtSystem.DefaultView.RowFilter = "(WTS_SystemSuiteID = " + systemSuiteID + "OR WTS_SystemID = 0) AND ARCHIVE = 0";
                                dtSystem = dtSystem.DefaultView.ToTable();
                                foreach (DataRow drSystem in dtSystem.Rows)
                                {
                                    int systemID = 0, waSystemID = 0;
                                    int.TryParse(drSystem["WTS_SystemID"].ToString(), out systemID);
                                    if (systemID > 0)
                                    {
                                        MasterData.WorkArea_System_Add(workAreaID: id, systemID: systemID, description: "", proposedPriority: proposedPriorityRank, approvedPriority: 0, cv: "0", exists: out exists, newID: out waSystemID, errorMsg: out tempMsg);
                                    }
                                }
                            }
                        }
					}
					else
					{
						saved = MasterData.WorkArea_Update(WorkAreaID: id, WorkArea: WorkArea, description: description, proposedPriorityRank: proposedPriorityRank, archive: (archive == 1), errorMsg: out tempMsg);
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

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WORK_AREA);

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

    [WebMethod(true)]
    public static string SaveWorkArea(string systems, int workArea, int systemSuiteID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            string[] Systems = systems.Split(',');
            if (Systems.Length == 0)
            {
                errorMsg = "Unable to save. An invalid list of Systems was provided.";
                saved = false;
            }

            HttpServerUtility server = HttpContext.Current.Server;
            //save
            for(int x = 0; x < Systems.Length; x++)
            {
                if (workArea == 0)
                {
                    tempMsg = "You must specify a Work Area.";
                    saved = false;
                }
                else
                {
                    exists = false;
                    int systemID = 0, waSystemID = 0;
                    int.TryParse(Systems[x].ToString(), out systemID);
                    if (systemID > 0)
                    {
                        saved = MasterData.WorkArea_System_Add(workAreaID: workArea, systemID: systemID, description: "", proposedPriority: 0, approvedPriority: 0, cv: "0", exists: out exists, newID: out waSystemID, errorMsg: out tempMsg);
                    }
                }

                if (saved)
                {
                    ids += string.Format("{0}{1}", x < Systems.Length - 1 ? "," : "", workArea.ToString());
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
				deleted = MasterData.WorkArea_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
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

        WTS.Caching.WTSCache.Instance.ClearCache(WTSCacheType.WORK_AREA);

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
                    DataTable dtSystem = MasterData.WorkArea_SystemList_Get(workAreaID: itemId, systemSuiteID: systemSuiteID);
                    foreach (DataRow drSystem in dtSystem.Rows)
                    {
                        int systemID = 0;
                        int.TryParse(drSystem["WorkArea_SystemID"].ToString(), out systemID);
                        if (systemID > 0)
                        {
                            deleted = MasterData.WorkArea_System_Delete(workAreaSystemID: systemID, cv: "0", exists: out exists, errorMsg: out errorMsg);
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