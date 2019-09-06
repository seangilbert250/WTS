﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Home : System.Web.UI.Page
{
	protected bool CanViewRequest = false;
	protected bool CanViewWorkItem = false;

	protected string TabToLoad = "Metrics";
	protected string Module = "Work";

    protected void Page_Load(object sender, EventArgs e)
	{
		this.CanViewRequest = UserManagement.UserCanView(WTSModuleOption.WorkRequest);
		this.CanViewWorkItem = UserManagement.UserCanView(WTSModuleOption.WorkItem);

		if (this.CanViewRequest || this.CanViewWorkItem)
		{
			this.spanWorkShortcuts.Style["display"] = "inline-block";
		}
		else
		{
			this.spanWorkShortcuts.Style["display"] = "none";
		}

		if (Request.QueryString["TabToLoad"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["TabToLoad"]))
		{
			this.TabToLoad = Server.UrlDecode(Request.QueryString["TabToLoad"]);
		}
		if (Request.QueryString["Module"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Module"]))
		{
			this.Module = Server.UrlDecode(Request.QueryString["Module"]);
		}

		DateTime today = DateTime.Now;
		this.lblToday.Text = today.ToString("dddd, dd MMMM yyyy");
    }

    [WebMethod(true)]
    public static int WorkItem_TaskID_Get(int itemID, int taskNumber)
    {
        try
        {
            return WorkItem_Task.WorkItem_TaskID_Get(itemID, taskNumber);
        }
        catch (Exception) { return -1; }
    }
}