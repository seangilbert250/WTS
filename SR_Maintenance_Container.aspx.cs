using System;

public partial class SR_Maintenance_Container : System.Web.UI.Page
{
    #region Variables
    protected string GridType = "SR";
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
    }
    #endregion
}