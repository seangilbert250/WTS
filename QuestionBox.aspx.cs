using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class QuestionBox : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        //TODO: load theme for user
        Page.Theme = UserManagement.GetUserTheme();
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        //Always include the default theme
        Page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl("~/App_Themes/Default/Default.css") + "\" />"));
    }
}