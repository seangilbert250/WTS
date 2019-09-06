using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RQMT_ParameterPage : System.Web.UI.Page
{
    protected string TabToLoad = "Metrics";

    protected void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["TabToLoad"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["TabToLoad"]))
        {
            this.TabToLoad = Request.QueryString["TabToLoad"];
        }
    }
}