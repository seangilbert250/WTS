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
using System.Web.UI.WebControls;

using Newtonsoft.Json;


public partial class WorkItemEditParent : System.Web.UI.Page
{
	protected string RootUrl = string.Empty;

	protected int WorkItemID = 0;
	protected int FirstWorkItemID = 0;
	protected int LastWorkItemID = 0;
	protected int PreviousWorkItemID = 0;
	protected int NextWorkItemID = 0;

	protected int WorkRequestID = 0;

	protected int Comment_Count = 0;
	protected int Attachment_Count = 0;
	protected int WR_Attachment_Count = 0;
    protected int SourceWorkItemID = 0;

    protected string SelectedAssigned;
    protected string SelectedStatuses;
    protected bool CanEdit = false;
    protected bool UseLocal = false;

    protected bool CopySubTasks = false;

    protected bool ShowPageContentInfo = true;

    protected void Page_Load(object sender, EventArgs e)
    {
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);

		readQueryString();

        if (this.WorkItemID > 0)
		{
			loadWorkItem();
		}
    }

	private void readQueryString()
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

		if (Request.QueryString["workItemID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["workItemID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["workItemID"].ToString()), out this.WorkItemID);
		}

        if (Request.QueryString["UseLocal"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["UseLocal"].ToString()))
        {
            if (Request.QueryString["UseLocal"].ToLower() == "true")
            {
                this.UseLocal = true;  
            }
        }

        // 12817 - 7 If Need source ID for subtasks
        if (Request.QueryString["SourceWorkItemID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["SourceWorkItemID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["SourceWorkItemID"].ToString()), out this.SourceWorkItemID);
        }
        if (Request.QueryString["SelectedAssigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAssigned"].ToString()))
        {
            SelectedAssigned = Server.UrlDecode(Request.QueryString["SelectedAssigned"].Trim());
        }
        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"].ToString()))
        {
            SelectedStatuses = Server.UrlDecode(Request.QueryString["SelectedStatuses"].Trim());
        }

        if (Request.QueryString["ShowPageContentInfo"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ShowPageContentInfo"].ToString()))
        {
            bool.TryParse(Request.QueryString["ShowPageContentInfo"], out ShowPageContentInfo);
        }
    }

    private void loadWorkItem()
	{
		DataTable dt = WorkloadItem.WorkItem_Get(workItemID: this.WorkItemID);

		if (dt != null && dt.Rows.Count > 0)
		{
			this.labelWorkItemNumber.InnerText = dt.Rows[0]["WORKITEMID"].ToString();
			int.TryParse(dt.Rows[0]["WORKREQUESTID"].ToString(), out this.WorkRequestID);

			int.TryParse(dt.Rows[0]["Comment_Count"].ToString(), out this.Comment_Count);
			int.TryParse(dt.Rows[0]["Attachment_Count"].ToString(), out this.Attachment_Count);
			int.TryParse(dt.Rows[0]["WorkRequest_Attachment_Count"].ToString(), out this.WR_Attachment_Count);
		}
	}

    [WebMethod(true)]
    public static int WorkItem_TaskID_Get(int itemID, int taskNumber)
    {
        try
        {
            return WorkItem_Task.WorkItem_TaskID_Get(itemID, taskNumber);
        }
        catch (Exception) { return -1; }
    }
}