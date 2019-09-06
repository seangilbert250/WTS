using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;


public partial class Hotlist_Grid : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected int MyView = 0;

	protected bool _myData = true;
	protected int _showArchived = 0;

	protected bool _refreshData = false;
	protected bool _export = false;

	protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

	protected bool CanViewGroup = false;
	protected bool CanEditGroup = false;
	protected bool CanViewRequest = false;
	protected bool CanEditRequest = false;


	protected void Page_Load(object sender, EventArgs e)
	{
		this.CanEditRequest = UserManagement.UserCanEdit(WTSModuleOption.WorkRequest);
		this.CanViewRequest = CanEditRequest || UserManagement.UserCanView(WTSModuleOption.WorkRequest);

		this.CanEditGroup = this.CanEditRequest;
		this.CanViewGroup = this.CanEditGroup;

		readQueryString();

		init();
		loadMenus();

		if (DCC != null)
		{
			this.DCC.Clear();
		}

		loadGridData();
	}

	private void readQueryString()
	{
		if (Page.IsPostBack)
		{
			_refreshData = false;
		}
		else
		{
			if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
				|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
			{
				_refreshData = true;
			}
		}

		if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
			|| Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
		{
			_myData = true;
		}
		else
		{
			_myData = false;
		}

		if (Request.QueryString["ShowArchived"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"].ToString()), out this._showArchived);
		}

		if (Request.QueryString["View"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["View"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["View"].ToString()), out this.MyView);
		}

		if (Request.QueryString["sortOrder"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
		{
			this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
		}
	}

	private void init()
	{
		grdHotlist.GridHeaderRowDataBound += grdHotlist_GridHeaderRowDataBound;
		grdHotlist.GridRowDataBound += grdHotlist_GridRowDataBound;
		grdHotlist.GridPageIndexChanging += grdHotlist_GridPageIndexChanging;

		ListItem item = null;
		item = ddlView.Items.FindByValue(this.MyView.ToString());
		if (item != null)
		{
			item.Selected = true;
		}
	}

	private void loadMenus()
	{
		try
		{
			DataSet dsMenu = new DataSet();
			dsMenu.ReadXml(this.Server.MapPath("XML/WTS_Menus.xml"));

			if (dsMenu.Tables.Count > 0 && dsMenu.Tables[0].Rows.Count > 0)
			{
				if (dsMenu.Tables.Contains("HostlistGridReports"))
				{
					menuReports.DataSource = dsMenu.Tables["HostlistGridReports"];
					menuReports.DataValueField = "URL";
					menuReports.DataTextField = "Text";
					menuReports.DataIDField = "id";
					//menuReports.DataParentIDField = "id_0";
					menuReports.DataImageField = "ImageType";
					menuReports.DataBind();
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}

	private void loadGridData()
	{
		DataTable dt;
		if (_refreshData || Session["dtHotlist_Group"] == null)
		{
			dt = RequestGroup.Hotlist_RequestGroupList_Get(showArchived: _showArchived, myData: _myData);
			HttpContext.Current.Session["dtHotlist_Group"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtHotlist_Group"];
		}

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			InitializeColumnData(ref dt);
			dt.AcceptChanges();

			int count = dt.Rows.Count;
			count = count > 0 ? count - 1 : count; //need to subtract the empty row
			spanRowCount.InnerText = count.ToString();
		}

		grdHotlist.DataSource = dt;
		grdHotlist.DataBind();
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
						displayName = "&nbsp;";
						blnVisible = true;
						blnSortable = false;
						break;
					case "RequestGroupID":
						displayName = "Group Number";
						blnVisible = false;
						blnSortable = false;
						break;
					case "SORT_ORDER":
						displayName = "Sort";
						blnVisible = true;
						blnSortable = true;
						break;
					case "RequestGroup":
						displayName = "Request Group";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = false;
						break;
					case "WorkRequest_Count":
						displayName = "Requests";
						blnVisible = true;
						blnSortable = true;
						break;
					case "ARCHIVE":
						displayName = "Archive";
						blnVisible = true;
						blnSortable = true;
						break;
					case "X":
						displayName = "&nbsp;";
						blnVisible = true;
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

	void grdHotlist_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridHeader(e.Row);
		GridViewRow row = e.Row;

		formatColumnDisplay(ref row);
	}

	void grdHotlist_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);

		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("RequestGroupID")].Text.Trim();
		string title = row.Cells[DCC.IndexOf("RequestGroup")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		string dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("WorkRequest_Count")].Text).Trim();
		int count = 0;
		int.TryParse(dependencies, out count);

		if (count > 0)
		{
			//buttons to show/hide child grid
			row.Cells[DCC["A"].Ordinal].Controls.Clear();
			row.Cells[DCC["A"].Ordinal].Controls.Add(createShowHideButton(true, "Show", title, itemId));
			row.Cells[DCC["A"].Ordinal].Controls.Add(createShowHideButton(false, "Hide", title, itemId));

			//add child grid row for Task Items
			Table table = (Table)row.Parent;
			GridViewRow childRow = createChildRow(itemId);
			table.Rows.AddAt(table.Rows.Count, childRow);
		}
		else
		{
			Image imgBlank = new Image();
			imgBlank.Height = 10;
			imgBlank.Width = 10;
			imgBlank.ImageUrl = "Images/Icons/blank.png";
			imgBlank.AlternateText = "";
			row.Cells[DCC["A"].Ordinal].Controls.Add(imgBlank);
		}

		if (!CanEditGroup)
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
			if (count > 0)
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
				row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("RequestGroup")].Text.Trim()));
			}

			row.Cells[DCC.IndexOf("SORT_ORDER")].Controls.Add(WTSUtility.CreateGridTextBox("Sort_Order", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SORT_ORDER")].Text.Replace("&nbsp;", " ").Trim()), true));
			TextBox txt = WTSUtility.CreateGridTextBox("RequestGroup", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("RequestGroup")].Text.Replace("&nbsp;", " ").Trim()));
			txt.Style["width"] = "500px";
			row.Cells[DCC.IndexOf("RequestGroup")].Controls.Add(txt);
			row.Cells[DCC.IndexOf("RequestGroup")].ToolTip = row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;", " ").Trim();
			row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;", " ").Trim())));
			
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
	}

	void grdHotlist_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdHotlist.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtHotlist_Group"] == null)
		{
			loadGridData();
		}
		else
		{
			grdHotlist.DataSource = (DataTable)HttpContext.Current.Session["dtHotlist_Group"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			row.Cells[i].Style["text-align"] = "left";
			if (i != DCC.IndexOf("A")
				&& i != DCC.IndexOf("SORT_ORDER")
				&& i != DCC.IndexOf("WorkRequest_Count")
				&& i != DCC.IndexOf("ARCHIVE")
				&& i != DCC.IndexOf("X"))
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

		row.Cells[DCC.IndexOf("A")].Style["width"] = "12px";
		row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "35px";
		//row.Cells[DCC.IndexOf("RequestGroup")].Style["width"] = "300px";
		row.Cells[DCC.IndexOf("WorkRequest_Count")].Style["width"] = "65px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
		row.Cells[DCC.IndexOf("X")].Style["width"] = "15px";
	}

	Image createShowHideButton(bool show = false, string direction = "Hide", string parent = "ALL", string parentId = "ALL")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("imgShowHideChildren_click(this,'{0}','{1}');", direction, parentId);

		Image img = new Image();
		img.ID = string.Format("img{0}Children_{1}", direction, parentId);
		img.Style["display"] = show ? "block" : "none";
		img.Style["cursor"] = "pointer";
		img.Attributes.Add("Name", string.Format("img{0}", direction));
		img.Attributes.Add("parentId", parentId);
		img.Height = 10;
		img.Width = 10;
		img.ImageUrl = direction.ToUpper() == "SHOW"
			? "Images/Icons/add_blue.png"
			: "Images/Icons/minus_blue.png";
		img.ToolTip = string.Format("{0} Work Requests for [{1}]", direction, parent);
		img.AlternateText = string.Format("{0} Work Requests for [{1}]", direction, parent);
		img.Attributes.Add("Onclick", sb.ToString());

		return img;
	}

	GridViewRow createChildRow(string parentId = "")
	{
		GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
		TableCell tableCell = null;

		try
		{
			row.CssClass = "gridBody";
			row.Style["display"] = "none";
			row.ID = string.Format("gridChild_{0}", parentId);
			row.Attributes.Add("parentId", parentId);
			row.Attributes.Add("Name", string.Format("gridChild_{0}", parentId));

			//add the table cells
			for (int i = 0; i < DCC.Count; i++)
			{
				tableCell = new TableCell();
				tableCell.Text = "&nbsp;";

				if (i == DCC.IndexOf("RequestGroupID"))
				{
					//set width to match parent
					tableCell.Style["width"] = "12px";
				}
				else if (i == DCC.IndexOf("SORT_ORDER"))
				{
					tableCell.Style["padding"] = "0px";
					tableCell.Style["vertical-align"] = "top";
					tableCell.ColumnSpan = DCC.Count - 1;
					//add the frame here
					tableCell.Controls.Add(createChildFrame(parentId: parentId));
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

	HtmlIframe createChildFrame(string parentId = "")
	{
		HtmlIframe childFrame = new HtmlIframe();

		if (string.IsNullOrWhiteSpace(parentId))
		{
			return null;
		}

		childFrame.ID = string.Format("frameChild_{0}", parentId);
		childFrame.Attributes.Add("parentId", parentId);
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
		bool saved = false;
		string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

		try
		{
			DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjson.Rows.Count == 0)
			{
				errorMsg = "Unable to save. An invalid list of changes was provided.";
				saved = false;
			}

			int id = 0;
			string title = string.Empty, description = string.Empty;
			int sortOrder = 0, archive = 0;

			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
				title = string.Empty;
				description = string.Empty;
				archive = 0;
				sortOrder = 0;

				tempMsg = string.Empty;
				title = dr["RequestGroup"].ToString();
				if (string.IsNullOrWhiteSpace(title))
				{
					tempMsg = "You must specify a value for Request Group.";
					saved = false;
				}
				else
				{
					int.TryParse(dr["RequestGroupID"].ToString(), out id);
					int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
					description = dr["DESCRIPTION"].ToString();					
					int.TryParse(dr["ARCHIVE"].ToString(), out archive);

					if (id == 0)
					{
						saved = false;
						tempMsg = "No Work Request was specified to update.";
					}
					else
					{
						saved = RequestGroup.RequestGroup_Update(id, title, description, sortOrder, (archive == 1), out tempMsg);
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

}