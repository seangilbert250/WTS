using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FilterSearch : System.Web.UI.Page
{
	protected string searchField = "";
	protected void Page_Load(object sender, EventArgs e)
	{
		searchField = Server.UrlDecode(HttpContext.Current.Request.QueryString["field"].ToString());

		lblSearch.Text = searchField + ":";
	}
}