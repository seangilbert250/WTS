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

public partial class WorkRequest_Comments : System.Web.UI.Page
{
	protected int WorkRequestID = 0;
	protected int SaveComplete = 0;
	protected bool IsAdmin = false;
	protected bool CanEdit = false;

	protected void Page_Load(object sender, EventArgs e)
	{
		this.IsAdmin = UserManagement.UserIsInRole("Admin");
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkRequest);

		readQueryString();

		loadComments();
	}

	private void readQueryString()
	{
		if (Request.QueryString["workRequestID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["workRequestID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["workRequestID"].ToString()), out this.WorkRequestID);
		}

		if (Request.QueryString["Saved"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["Saved"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["Saved"].ToString()), out this.SaveComplete);
		}
	}

	private void loadComments()
	{
		DataTable dt = WorkRequest.WorkRequest_GetCommentList(workRequestID: this.WorkRequestID);

		Page.ClientScript.RegisterArrayDeclaration("arrComments", JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None));
	}

	[WebMethod()]
	public static string SaveComments(int workRequestID, dynamic comments)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };

		bool saved = false;
		string errorMsg = string.Empty, ids = string.Empty;
		int commentId = 0, parentCommentId = 0;
		string commentText = string.Empty;

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
					saved = WorkRequest.WorkRequest_Comment_Add(out commentId, out errorMsg, workRequestID, parentCommentId, commentText);
				}
				else
				{
					saved = WorkRequest.WorkRequest_Comment_Update(out errorMsg, commentId, commentText);
				}


				if (saved)
				{
					ids += commentId.ToString() + ",";
				}
			}
			catch (Exception ex)
			{
				LogUtility.LogException(ex);
				errorMsg += ": " + ex.Message;
			}
		}

		ids = ids.TrimEnd(new char[] { ',' });
		if (ids.Length > 0)
		{
			saved = true;
		}

		result["saved"] = saved.ToString();
		result["ids"] = ids;
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod()]
	public static string DeleteComments(int workRequestID, dynamic arrCommentIDsToDelete)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "ids", "" }, { "error", "" } };

		bool deleted = false;
		string errorMsg = string.Empty, ids = string.Empty;
		int commentId = 0;

		foreach (dynamic id in arrCommentIDsToDelete)
		{
			commentId = 0;

			try
			{
				if (int.TryParse(id.ToString(), out commentId))
				{
					deleted = WorkRequest.WorkRequest_Comment_Delete(out errorMsg, workRequestID, commentId);

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