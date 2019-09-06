using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;


public partial class WorkloadGrid_WorkItems : System.Web.UI.Page
{
	protected string RootUrl = string.Empty;
	protected bool _myData = true;
	protected int _showArchived = 0;

	protected bool CanView = false;
	protected bool CanEdit = false;

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int RequestID = 0;
    protected string SelectedAssigned;
    protected string SelectedStatuses;
    protected DataColumnCollection DCC;


	protected void Page_Load(object sender, EventArgs e)
	{
		this.CanView = UserManagement.UserCanView(WTSModuleOption.WorkItem);
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);

		initGrid();
		readQueryString();

		loadWorkItems();
	}

	void readQueryString()
	{
		#region Get Root URL for JSON calls
		int idxQueryString = Request.Url.OriginalString.IndexOf(Request.Url.Query);
		if (idxQueryString > 0)
		{
			RootUrl = Request.Url.OriginalString.Substring(0, idxQueryString);
		}
		else
		{
			RootUrl = Request.Url.OriginalString;
		}
        #endregion

        if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
            || Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
        {
            _refreshData = true;
        }

        //      if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
        //	|| Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
        //{
        //	_myData = true;
        //}
        //else
        //{
        //	_myData = false;
        //}
        if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"]))
        {
            _myData = false;
        }
        else if (Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
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
		if (Request.QueryString["requestID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["requestID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["requestID"].ToString()), out this.RequestID);
		}
        if(Request.QueryString["SelectedAssigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAssigned"].ToString()))
        {
            SelectedAssigned = Server.UrlDecode(Request.QueryString["SelectedAssigned"].Trim());
        }
        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"].ToString()))
        {
            SelectedStatuses = Server.UrlDecode(Request.QueryString["SelectedStatuses"].Trim());
        }
    }

	void initGrid()
	{
		gridWork.RowDataBound += gridWork_RowDataBound;
		gridWork.PageIndexChanging += gridWork_PageIndexChanging;
	}


	#region Grid

	void gridWork_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.Header)
		{
			gridWork_GridHeaderRowDataBound(sender, e);
		}
		else if (e.Row.RowType == DataControlRowType.DataRow)
		{
			gridWork_GridRowDataBound(sender, e);
		}
	}

	void gridWork_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		for (int i = 0; i < row.Cells.Count; i++)
		{
			row.Cells[i].Style["background-color"] = "#E6E6E6";
			row.Cells[i].Text = row.Cells[i].Text.Replace("_", " ");
		}

		row.Cells[DCC.IndexOf("WorkRequestID")].Text = "&nbsp;";
		row.Cells[DCC.IndexOf("ItemID")].Text = "Task #";
		row.Cells[DCC.IndexOf("Websystem")].Text = "System";
		row.Cells[DCC.IndexOf("Title")].Text = "Description";
		row.Cells[DCC.IndexOf("Allocation")].Text = "Allocation Assignment";
		row.Cells[DCC.IndexOf("Production")].Text = "Production";
		row.Cells[DCC.IndexOf("Version")].Text = "Version";
		row.Cells[DCC.IndexOf("Priority")].Text = "Priority";
		row.Cells[DCC.IndexOf("Primary_Analyst")].Text = "Primary Analyst";
		row.Cells[DCC.IndexOf("Primary_Developer")].Text = "Primary Developer";
		row.Cells[DCC.IndexOf("Assigned")].Text = "Assigned";
		row.Cells[DCC.IndexOf("Status")].Text = "Status";
		row.Cells[DCC.IndexOf("Progress")].Text = "Progress";
		
	}

	void gridWork_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		string itemId = row.Cells[DCC.IndexOf("ItemID")].Text.Trim();
		int taskCount = 0;
		int.TryParse(row.Cells[DCC["Task_Count"].Ordinal].Text.Trim().Replace("&nbsp", "0"), out taskCount);

		if (CanEdit || CanView)
		{
			row.Cells[DCC["ItemID"].Ordinal].Controls.Add(createEditLink(itemId));
		}
		row.Cells[DCC["TITLE"].Ordinal].ToolTip = row.Cells[DCC["DESCRIPTION"].Ordinal].Text;

		if (taskCount > 0)
		{
			//buttons to show/hide child grid
			row.Cells[DCC["WorkRequestID"].Ordinal].Controls.Clear();
			row.Cells[DCC["WorkRequestID"].Ordinal].Controls.Add(createShowHideButton(true, "Show", itemId));
			row.Cells[DCC["WorkRequestID"].Ordinal].Controls.Add(createShowHideButton(false, "Hide", itemId));

			//add child grid row for SR Items
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
			row.Cells[DCC["WorkRequestID"].Ordinal].Controls.Add(imgBlank);
		}
	}

	void gridWork_PageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		gridWork.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtWorkItem"] == null)
		{
			loadWorkItems();
		}
		else
		{
			gridWork.DataSource = (DataTable)HttpContext.Current.Session["dtWorkItem"];
			gridWork.DataBind();
		}
	}


	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			//only show desired columns
			if (i != DCC.IndexOf("WorkRequestID")
				&& i != DCC.IndexOf("ItemID")
				&& i != DCC.IndexOf("Websystem")
				&& i != DCC.IndexOf("TITLE")
				//&& i != DCC.IndexOf("Description")
				&& i != DCC.IndexOf("Allocation")
				&& i != DCC.IndexOf("Production")
				&& i != DCC.IndexOf("Version")
				&& i != DCC.IndexOf("Priority")
				&& i != DCC.IndexOf("Primary_Analyst")
				&& i != DCC.IndexOf("Primary_Developer")
				&& i != DCC.IndexOf("Assigned")
				&& i != DCC.IndexOf("Status")
				&& i != DCC.IndexOf("Progress"))
			{
				row.Cells[i].Style["display"] = "none";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "left";
				if (i != DCC.IndexOf("WORKREQUESTID") && i != DCC.IndexOf("ItemID") && i != DCC.IndexOf("Production") && i != DCC.IndexOf("Progress"))
				{
					row.Cells[i].Style["padding-left"] = "5px";
				}
			}
		}

		//more column formatting
		row.Cells[DCC.IndexOf("WORKREQUESTID")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("WORKREQUESTID")].Style["width"] = "12px";
		row.Cells[DCC.IndexOf("ItemID")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("ItemID")].Style["width"] = "46px";
		row.Cells[DCC.IndexOf("Websystem")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("Allocation")].Style["width"] = "125px";
		row.Cells[DCC.IndexOf("Production")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("Production")].Style["width"] = "60px";
		row.Cells[DCC.IndexOf("Version")].Style["width"] = "45px";
		row.Cells[DCC.IndexOf("Priority")].Style["width"] = "45px";
		row.Cells[DCC.IndexOf("Primary_Analyst")].Style["width"] = "85px";
		row.Cells[DCC.IndexOf("Primary_Developer")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("Assigned")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("Status")].Style["width"] = "55px";
		row.Cells[DCC.IndexOf("Progress")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("Progress")].Style["width"] = "61px";
		row.Cells[DCC.IndexOf("Progress")].Style["border-right"] = "none";
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
		img.Attributes.Add("requestId", itemId);
		img.Height = 10;
		img.Width = 10;
		img.ImageUrl = direction.ToUpper() == "SHOW"
			? "Images/Icons/add_blue.png"
			: "Images/Icons/minus_blue.png";
		img.ToolTip = string.Format("{0} Sub-Tasks for [{1}]", direction, itemId);
		img.AlternateText = string.Format("{0} Sub-Tasks for [{1}]", direction, itemId);
		img.Attributes.Add("Onclick", sb.ToString());

		return img;
	}

	LinkButton createEditLink(string workItemId = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEditWorkItem_click('{0}');return false;", workItemId);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEditWorkItem_{0}", workItemId);
		lb.Attributes["name"] = string.Format("lbEditWorkItem_{0}", workItemId);
		lb.ToolTip = string.Format("Edit Workload Task [{0}]", workItemId);
		lb.Text = workItemId;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}

	GridViewRow createChildRow(string workItemId = "")
	{
		GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
		TableCell tableCell = null;

		try
		{
			row.CssClass = "gridBody";
			row.Style["display"] = "none";
			row.ID = string.Format("gridChild_{0}", workItemId);
			row.Attributes.Add("workItemId", workItemId);
			row.Attributes.Add("Name", string.Format("gridChild_{0}", workItemId));

			//add the table cells
			for (int i = 0; i < DCC.Count; i++)
			{
				tableCell = new TableCell();
				tableCell.Text = "&nbsp;";

				if (i == DCC["WorkRequestID"].Ordinal)
				{
					//set width to match parent
					tableCell.Style["width"] = "12px";
					tableCell.Style["border-right"] = "1px solid transparent";
				}
				else if (i == DCC["ItemID"].Ordinal)
				{
					//set width to match parent
					tableCell.Style["width"] = "46px";
				}
				else if (i == DCC["Description"].Ordinal)
				{
					tableCell.Style["padding"] = "0px";
					tableCell.Style["border-right"] = "0px solid transparent";
					tableCell.Style["vertical-align"] = "top";
					tableCell.ColumnSpan = DCC.Count - 2;
					//add the frame here
					tableCell.Controls.Add(createChildFrame(workItemId: workItemId));
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

	HtmlIframe createChildFrame(string workItemId = "")
	{
		HtmlIframe childFrame = new HtmlIframe();

		if (string.IsNullOrWhiteSpace(workItemId))
		{
			return null;
		}

		childFrame.ID = string.Format("frameChild_{0}", workItemId);
		childFrame.Attributes.Add("workItemId", workItemId);
		childFrame.Attributes["frameborder"] = "0";
		childFrame.Attributes["scrolling"] = "no";
		childFrame.Attributes["src"] = "javascript:''";
		childFrame.Style["height"] = "30px";
		childFrame.Style["width"] = "100%";
		childFrame.Style["border"] = "none";

		return childFrame;
	}

	#endregion Grid
	


	private void loadWorkItems()
	{
		DataTable dt = null;
		if (_refreshData || Session["dtWorkItem"] == null)
		{
			dt = GetWorkItems(this.RequestID, this._showArchived, myData: _myData);
			HttpContext.Current.Session["dtWorkItem"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtWorkItem"];
		}

		if (dt != null)
		{
			this.DCC = dt.Columns;
		}

		gridWork.DataSource = dt;
		gridWork.DataBind();
	}

	protected static DataTable GetWorkItems(int requestID = 0, int showArchived = 0, bool myData = false)
	{
		DataTable dt = null;

		try
		{
			dt = WorkloadItem.WorkItemList_Get(workRequestID: requestID, showArchived: showArchived, columnListOnly: 0, myData: myData);
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			dt = null;
		}

		return dt;
	}

}