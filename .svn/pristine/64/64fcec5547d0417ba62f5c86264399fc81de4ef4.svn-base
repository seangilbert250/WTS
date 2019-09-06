using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterDataContainer : System.Web.UI.Page
{
	protected string MDType = string.Empty;
	protected bool Refresh = false;


    protected void Page_Load(object sender, EventArgs e)
    {
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			Refresh = true;
		}
		if (Request.QueryString["MDType"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["MDType"].ToString()))
		{
			this.MDType = Server.UrlDecode(Request.QueryString["MDType"]);
		}
    }
}