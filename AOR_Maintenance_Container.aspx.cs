using System;

public partial class AOR_Maintenance_Container : System.Web.UI.Page
{
    #region Variables
    protected string GridType = "AOR";
    protected string MenuType = "";
    #endregion

    #region Page
    protected void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["GridType"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridType"].ToString()))
        {
            this.GridType = Request.QueryString["GridType"];
        }
        if (Request.QueryString["MenuType"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["MenuType"].ToString()))
        {
            this.MenuType = Request.QueryString["MenuType"];
        }
    }
    #endregion
}