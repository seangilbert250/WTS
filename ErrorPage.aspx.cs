using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ErrorPage : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        //load theme for user
		Page.Theme = UserManagement.GetUserTheme();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        String errorNumber = Request.QueryString["errorNumber"];
        String errorDescription = Request.QueryString["errorDescription"];
        String errorPage = Request.QueryString["errorPage"];
        String errorFunction = Request.QueryString["errorFunction"];

        lblNumber.Text = errorNumber;
        txtDescription.Text = errorDescription;
        lblPage.Text = errorPage;
        lblFunction.Text = errorFunction;
    }
}