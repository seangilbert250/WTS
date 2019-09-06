﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Popup : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
		Page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl("~/Styles/jquery-ui.css") + "\" />"));

        ClientScriptManager cs = Page.ClientScript;
        if (!cs.IsClientScriptIncludeRegistered("jquery"))
        {
			cs.RegisterClientScriptInclude("jquery", ResolveClientUrl("Scripts/jquery-1.11.2.min.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered("jquery-ui"))
        {
            cs.RegisterClientScriptInclude("jquery-ui", ResolveClientUrl("Scripts/jquery-ui.min.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered("common"))
        {
            cs.RegisterClientScriptInclude("common", ResolveClientUrl("Scripts/common.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered("shell"))
        {
            cs.RegisterClientScriptInclude("shell", ResolveClientUrl("Scripts/shell.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered("popupWindow"))
        {
            cs.RegisterClientScriptInclude("popupWindow", ResolveClientUrl("Scripts/popupWindow.js"));
        }
    }
}
