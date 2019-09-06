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


public partial class WorkItemGrid_Tasks : System.Web.UI.Page
{
	protected string RootUrl = string.Empty;

	protected bool CanView = false;
	protected bool CanEdit = false;

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int WorkItemID = 0;

	protected DataColumnCollection DCC;


    protected void Page_Load(object sender, EventArgs e)
    {
		this.CanView = UserManagement.UserCanView(WTSModuleOption.WorkItem);
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);

		initGrid();
		readQueryString();

		loadTasks();
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
		if (Request.QueryString["workItemID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["workItemID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["workItemID"].ToString()), out this.WorkItemID);
		}
	}

	void initGrid()
	{
		gridTask.RowDataBound += gridTask_RowDataBound;
		gridTask.PageIndexChanging += gridTask_PageIndexChanging;
	}


	#region Grid

	void gridTask_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.Header)
		{
			gridTask_GridHeaderRowDataBound(sender, e);
		}
		else if (e.Row.RowType == DataControlRowType.DataRow)
		{
			gridTask_GridRowDataBound(sender, e);
		}
	}

	void gridTask_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		for (int i = 0; i < row.Cells.Count; i++)
		{
			row.Cells[i].Text = row.Cells[i].Text.Replace("_", " ");
		}

		row.Cells[DCC["WORKITEM_TASKID"].Ordinal].Text = "Sub-Task #";
		row.Cells[DCC["TITLE"].Ordinal].Text = "Title";
		row.Cells[DCC["ESTIMATEDSTARTDATE"].Ordinal].Text = "Planned Start";
		row.Cells[DCC["ACTUALSTARTDATE"].Ordinal].Text = "Actual Start";
		row.Cells[DCC["ACTUALENDDATE"].Ordinal].Text = "Actual End";
		row.Cells[DCC["PLANNEDHOURS"].Ordinal].Text = "Planned Hours";
		row.Cells[DCC["ACTUALHOURS"].Ordinal].Text = "Actual Hours";
		row.Cells[DCC["AssignedResource"].Ordinal].Text = "Assigned";
		row.Cells[DCC["Status"].Ordinal].Text = "Status";
		row.Cells[DCC["COMPLETIONPERCENT"].Ordinal].Text = "Progress";
	}

	void gridTask_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		string itemId = row.Cells[DCC.IndexOf("WORKITEMID")].Text.Trim();
		string taskId = row.Cells[DCC.IndexOf("WORKITEM_TASKID")].Text.Trim();

		for (int i = 0; i < row.Cells.Count; i++)
		{
			row.Cells[i].Style["background-color"] = "#FFFFFF";
		}

		row.Cells[DCC["WORKITEM_TASKID"].Ordinal].Controls.Add(createEditLink(taskId, itemId + " - " + taskId));


	}

	void gridTask_PageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		gridTask.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtTask"] == null)
		{
			loadTasks();
		}
		else
		{
			gridTask.DataSource = (DataTable)HttpContext.Current.Session["dtTask"];
			gridTask.DataBind();
		}
	}


	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			//only show desired columns
			if (i != DCC.IndexOf("WORKITEM_TASKID")
				&& i != DCC.IndexOf("Title")
				&& i != DCC.IndexOf("ASSIGNEDRESOURCE")
				&& i != DCC.IndexOf("ESTIMATEDSTARTDATE")
				&& i != DCC.IndexOf("ACTUALSTARTDATE")
				&& i != DCC.IndexOf("ACTUALENDDATE")
				&& i != DCC.IndexOf("PLANNEDHOURS")
				&& i != DCC.IndexOf("ACTUALHOURS")
				&& i != DCC.IndexOf("COMPLETIONPERCENT")
				&& i != DCC.IndexOf("Status"))
			{
				row.Cells[i].Style["display"] = "none";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "left";
				if (i != DCC.IndexOf("WORKITEM_TASKID")
					&& i != DCC.IndexOf("PLANNEDHOURS")
					&& i != DCC.IndexOf("ACTUALHOURS")
					&& i != DCC.IndexOf("COMPLETIONPERCENT") 
					&& i != DCC.IndexOf("Sort_Order"))
				{
					row.Cells[i].Style["padding-left"] = "5px";
				}
			}
		}

		//more column formatting
		row.Cells[DCC.IndexOf("WORKITEM_TASKID")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("WORKITEM_TASKID")].Style["width"] = "77px";
		row.Cells[DCC.IndexOf("ESTIMATEDSTARTDATE")].Style["width"] = "80px";
		row.Cells[DCC.IndexOf("ACTUALSTARTDATE")].Style["width"] = "80px";
		row.Cells[DCC.IndexOf("ACTUALENDDATE")].Style["width"] = "80px";
		row.Cells[DCC.IndexOf("PLANNEDHOURS")].Style["width"] = "85px";
		row.Cells[DCC.IndexOf("PLANNEDHOURS")].Style["text-align"] = "right";
		row.Cells[DCC.IndexOf("PLANNEDHOURS")].Style["padding-right"] = "5px";
		row.Cells[DCC.IndexOf("ACTUALHOURS")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("ACTUALHOURS")].Style["text-align"] = "right";
		row.Cells[DCC.IndexOf("ACTUALHOURS")].Style["padding-right"] = "5px";
		row.Cells[DCC.IndexOf("AssignedResource")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("Status")].Style["width"] = "55px";
		row.Cells[DCC.IndexOf("COMPLETIONPERCENT")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("COMPLETIONPERCENT")].Style["width"] = "61px";
		row.Cells[DCC.IndexOf("COMPLETIONPERCENT")].Style["border-right"] = "none";
		row.Cells[DCC.IndexOf("Sort_Order")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("Sort_Order")].Style["width"] = "45px";
	}

	LinkButton createEditLink(string taskId = "", string text = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEditTask_click('{0}');return false;", taskId);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEditTask_{0}", taskId);
		lb.Attributes["name"] = string.Format("lbEditTask_{0}", taskId);
		lb.ToolTip = string.Format("Edit Sub-Task [{0}]", taskId);
		lb.Text = text;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}

	#endregion Grid


	private void loadTasks()
	{
		DataTable dt = null;
		if (_refreshData || Session["dtTask"] == null)
		{
			dt = WorkloadItem.WorkItem_GetTaskList(workItemID: this.WorkItemID);
			HttpContext.Current.Session["dtTask"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtWorkItem"];
		}

		if (dt != null)
		{
			dt.Columns["AssignedResource"].SetOrdinal(dt.Columns["ACTUALENDDATE"].Ordinal);
			dt.Columns["Status"].SetOrdinal(dt.Columns["COMPLETIONPERCENT"].Ordinal);
			this.DCC = dt.Columns;
		}

		gridTask.DataSource = dt;
		gridTask.DataBind();
	}
}