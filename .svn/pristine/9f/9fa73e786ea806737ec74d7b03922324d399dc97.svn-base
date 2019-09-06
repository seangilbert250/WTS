using System;

public partial class RQMT_Maintenance_Container : System.Web.UI.Page
{
    #region Variables
    protected string GridType = "RQMT";
    protected string _lastGridPageIndex = "";
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

        _lastGridPageIndex = Session["rqmt.last.gridpageindex"] != null ? (string)Session["rqmt.last.gridpageindex"] : "0";
    }
    #endregion
}