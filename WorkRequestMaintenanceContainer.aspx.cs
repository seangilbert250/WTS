using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WorkRequestMaintenanceContainer : System.Web.UI.Page
{
	protected bool CanView = false;
	protected bool CanEdit = false;

	protected string GridType = "Request";
	protected bool Refresh = false;

    protected void Page_Load(object sender, EventArgs e)
    {
		this.CanView = UserManagement.UserCanView(WTSModuleOption.WorkRequest);
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkRequest);

		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			Refresh = true;
		}
		if (Request.QueryString["GridType"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["GridType"].ToString()))
		{
			this.GridType = Server.UrlDecode(Request.QueryString["GridType"]);
		}
    }
}