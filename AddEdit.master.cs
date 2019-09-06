﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AddEdit : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Always include the default theme
        Page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl("~/App_Themes/Default/Default.css") + "\" />"));

        ClientScriptManager cs = Page.ClientScript;
        if (!cs.IsClientScriptIncludeRegistered("common"))
        {
            cs.RegisterClientScriptInclude("common", ResolveClientUrl("Scripts/common.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered("jquery"))
        {
            cs.RegisterClientScriptInclude("jquery", ResolveClientUrl("Scripts/jquery-1.11.2.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered("shell"))
        {
            cs.RegisterClientScriptInclude("shell", ResolveClientUrl("Scripts/shell.js"));
        }


        if (!cs.IsClientScriptIncludeRegistered("popupWindow"))
        {
            Random rnd = new Random();
            cs.RegisterClientScriptInclude("popupWindow", ResolveClientUrl("Scripts/popupWindow.js?" + rnd.Next(1,1000).ToString()));
        }
    }
}
