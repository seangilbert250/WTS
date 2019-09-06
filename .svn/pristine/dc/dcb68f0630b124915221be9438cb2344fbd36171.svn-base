using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Apply_Filters : System.Web.UI.Page
{
    protected string strFilter = "";
    protected string rqmtRPTID = "";
    protected string strUseFilter = "1";
    protected string skipClear = "false";
        
    protected void Page_Load(object sender, EventArgs e)
    {
		strFilter = Request.Form["strFilter"];
        strUseFilter = Request.Form["strUseFilter"];
        skipClear = Request.Form["skipClear"];

		//Filtering.APPLY_USER_FILTER(strFilter, strUseFilter, skipClear);
    }
}