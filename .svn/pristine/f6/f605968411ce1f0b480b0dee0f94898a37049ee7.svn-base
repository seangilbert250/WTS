using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WorkRequestEditParent : System.Web.UI.Page
{
	protected int WorkRequestID = 0;
	protected int FirstWorkRequestID = 0;
	protected int LastWorkRequestID = 0;
	protected int PreviousWorkRequestID = 0;
	protected int NextWorkRequestID = 0;

	protected bool CanView = false;
	protected bool CanEdit = false;

    protected void Page_Load(object sender, EventArgs e)
    {
		this.CanView = UserManagement.UserCanView(WTSModuleOption.WorkRequest);
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkRequest);

		readQueryString();
		loadWorkRequest();
    }
	private void readQueryString()
	{
		if (Request.QueryString["workRequestID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["workRequestID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["workRequestID"].ToString()), out this.WorkRequestID);
		}
	}

	private void loadWorkRequest()
	{
		DataTable dt = WorkRequest.WorkRequest_Get(workRequestID: this.WorkRequestID);

		if (dt != null && dt.Rows.Count > 0)
		{
			this.labelWorkRequestNumber.InnerText = dt.Rows[0]["WORKREQUESTID"].ToString();
		}
	}

}