using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Grid_Order : System.Web.UI.Page
{
    protected string strFields = "";
    protected string[] strSplitFields;

    protected void Page_Load(object sender, EventArgs e)
    {
        PopulateSortContainer();
    }

    protected void PopulateSortContainer()
    {
        try
        {
            if (Request.Form["columnOrder"] != "")
            {
                strFields = Server.HtmlDecode(Request.Form["columnOrder"]);
                if (strFields != null) strSplitFields = strFields.Split('~');
            }
        }
        catch (Exception ex)
        {

        }
    }
}
