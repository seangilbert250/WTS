﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public partial class Download_Attachment : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Page_LoadComplete(object sender, EventArgs e)  //Complete
    {
		int attachmentID = 0;
		if(Request.QueryString["attachmentID"] == null || string.IsNullOrWhiteSpace(Request.QueryString["attachmentID"]))
		{
			return;
		}

		int.TryParse(Server.UrlDecode(Request.QueryString["attachmentID"].ToString()), out attachmentID);
        DataTable dt = WTSData.DOWNLOAD_ATTACHMENT(attachmentID);
        
		if (dt == null || dt.Rows.Count == 0)
		{
			return;
		}

		byte[] fByte;
        fByte = (byte[])dt.Rows[0]["FileData"];
        if(fByte.Length > 0)
        {
            Response.Clear();
			Response.AddHeader("Content-Disposition", "attachment; filename=\"" + dt.Rows[0]["FileName"].ToString() + "\"");
            Response.AddHeader("Content-Length", fByte.Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.OutputStream.Write(fByte, 0, fByte.Length);
            Response.End();
        }
        else
        {
            Response.Write("This file does not exist.");
        }
    }
}