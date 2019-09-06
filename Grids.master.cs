﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Grids : System.Web.UI.MasterPage
{
    protected void Page_PreInit(object sender, EventArgs e)
    {

    }
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
            cs.RegisterClientScriptInclude("common", ResolveClientUrl("Scripts/common.js?=6"));
        }
        if (!cs.IsClientScriptIncludeRegistered("shell"))
        {
            cs.RegisterClientScriptInclude("shell", ResolveClientUrl("Scripts/shell.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered("popupWindow"))
        {
            cs.RegisterClientScriptInclude("popupWindow", ResolveClientUrl("Scripts/popupWindow.js"));
		}

        if (!cs.IsClientScriptBlockRegistered("underscore"))
        {
            cs.RegisterClientScriptInclude("underscore", ResolveClientUrl("Scripts/underscore-min.js"));
        }

        if (!cs.IsClientScriptBlockRegistered("underscorestring"))
        {
            cs.RegisterClientScriptInclude("underscorestring", ResolveClientUrl("Scripts/underscore.string.min.js"));
        }

        if (!cs.IsClientScriptBlockRegistered("moment"))
        {
            cs.RegisterClientScriptInclude("moment", ResolveClientUrl("Scripts/moment.min.js"));
        }

        if (!cs.IsClientScriptBlockRegistered("chart"))
        {
            cs.RegisterClientScriptInclude("chart", ResolveClientUrl("Scripts/chart.min.js"));
        }
    }
}