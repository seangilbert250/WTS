using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

public partial class Task_Comments : System.Web.UI.Page
{
	protected int WorkItemID = 0;
	protected int WorkItem_TaskID = 0;
	protected int SaveComplete = 0;
	protected bool IsAdmin = false;
	protected bool CanEdit = false;
    protected bool ReadOnly = false;

	protected int Comment_Count = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
		this.IsAdmin = UserManagement.UserIsInRole("Admin");
		this.CanEdit = (UserManagement.UserCanEdit(WTSModuleOption.WorkItem) || UserManagement.UserCanEdit(WTSModuleOption.WorkItemTask));

		readQueryString();

        if (ReadOnly)
        {
            this.CanEdit = false;
        }

        loadComments();
    }

    private void readQueryString()
	{
		if (Request.QueryString["workItemID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["workItemID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["workItemID"].ToString()), out this.WorkItemID);
		}
		if (Request.QueryString["taskID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["taskID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["taskID"].ToString()), out this.WorkItem_TaskID);
		}

		if (Request.QueryString["Saved"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["Saved"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["Saved"].ToString()), out this.SaveComplete);
		}

        if (Request.QueryString["ReadOnly"] != null)
        {
            ReadOnly = Request.QueryString["ReadOnly"].ToLower() == "true" || Request.QueryString["ReadOnly"] == "1";
        }
    }

	private void loadComments()
	{
		WorkItem_Task task = new WorkItem_Task(taskID: this.WorkItem_TaskID);

        if (task == null || !task.Load())
        {
            return;
        }

        txtWorkloadNumber.Text = string.Format("{0} - {1}", task.WorkItemID.ToString(), task.Task_Number.ToString());
        txtTitle.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(task.Title.Replace("&nbsp;", "").Trim()));

        DataTable dt = task.GetComments();
		if (dt != null && dt.Rows.Count > 0)
		{
			Comment_Count = dt.Rows.Count;
		}

        Page.ClientScript.RegisterArrayDeclaration("arrComments", JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None));
	}

	[WebMethod()]
	public static string SaveComments(int taskID, dynamic comments)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
			, { "failed", "0" }
			, { "savedIds", "" }
			, { "failedIds", "" }
			, { "error", "" } };

		bool saved = false;
		int savedQty = 0, failedQty = 0;
		string errorMsg = string.Empty, tempMsg = string.Empty, ids = string.Empty, failedIds = string.Empty;
		int commentId = 0, parentCommentId = 0;
		string commentText = string.Empty;
		WorkItem_Task task = new WorkItem_Task(taskID);

		foreach (dynamic comment in comments)
		{
			commentId = 0;
			parentCommentId = 0;
			//comment[0]=commentid, comment[1]=parentid, comment[2]=text
			try
			{
				commentText = comment[2].ToString();
				if (string.IsNullOrWhiteSpace(comment[0].ToString())
					|| !int.TryParse(comment[0].ToString(), out commentId))
				{
					int.TryParse(comment[1].ToString(), out parentCommentId);
					saved = task.AddComment(newID: out commentId, errorMsg: out tempMsg, parentCommentID: 0, comment_text: commentText);
				}
				else
				{
					saved = task.UpdateComment(errorMsg: out tempMsg, commentId: commentId, comment_text: commentText);
				}


				if (saved)
				{
					ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", commentId.ToString());
					savedQty += 1;
				}
				else
				{
					failedQty += 1;
				}

				if (tempMsg.Length > 0)
				{
					errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
				}
			}
			catch (Exception ex)
			{
				LogUtility.LogException(ex);
				errorMsg += ": " + ex.Message;
			}
		}

		result["savedIds"] = ids;
		result["failedIds"] = failedIds;
		result["saved"] = savedQty.ToString();
		result["failed"] = failedQty.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod()]
	public static string DeleteComments(int taskID, dynamic arrCommentIDsToDelete)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }
			, { "ids", "" }
			, { "error", "" } };

		bool deleted = false;
		string errorMsg = string.Empty, ids = string.Empty;
		int commentId = 0;
		WorkItem_Task task = new WorkItem_Task(taskID);

		foreach (dynamic id in arrCommentIDsToDelete)
		{
			commentId = 0;

			try
			{
				if (int.TryParse(id.ToString(), out commentId))
				{
					deleted = task.DeleteComment(errorMsg: out errorMsg, commentId: commentId);

					if (deleted)
					{
						ids += "," + commentId.ToString();
					}
				}
			}
			catch (Exception ex)
			{
				LogUtility.LogException(ex);
				errorMsg += ": " + ex.Message;
				deleted = false;
			}
		}

		ids = ids.TrimEnd(new char[] { ',' });
		if (ids.Length > 0)
		{
			deleted = true;
		}

		result["deleted"] = deleted.ToString();
		result["ids"] = ids;
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

}