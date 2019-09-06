﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.Win32;

using Aspose.Cells;
using Aspose.Cells.Rendering;

using Infinite.Security;

using System.Xml;

public enum ChildType
{
	WorkItem = 0,
	SR
}

public enum WTSModuleOption
{
	WorkRequest = 0,
	WorkItem,
	WorkItemTask,
	SustainmentRequest,
	AORReport,
	MasterData,
	ResourceAdmin,
	OrganizationAdmin,
	Dashboard,
	News,
	Metrics,
	CVTMetrics,
    AOR,
    Meeting,
    CR,
    RQMT,
    Deployment,
    WorkloadMGMT
}

public enum UserSettingType
{
	GridView = 1
}

public enum WTSGridName
{
	Workload = 1,
	QM_Workload = 2,
	Work_Request = 3,
	Hotlist = 4,
	SR = 5,
	User = 6,
	Organization = 7,
	Work = 8,
	Default = 9,
	Workload_Crosswalk = 10,
	AOR = 11,
	AOR_Meeting = 12,
    RQMT_Grid = 13
}

public enum SettingParameterType
{
	View = 1,
	Tier1Columns,
	Tier1ColumnOrder,
	Tier1SortOrder,
	Tier1RollupGroup,
	Tier2Columns,
	Tier2ColumnOrder,
	Tier2SortOrder,
	Tier2RollupGroup,
	Tier3Columns,
	Tier3ColumnOrder,
	Tier3SortOrder
}


/// <summary>
/// Common utilities for project
/// </summary>
public sealed class WTSUtility
{
	public static void SelectDdlItem(DropDownList ddl, string val, string text = "")
	{
		ListItem item = null;

		if (string.IsNullOrWhiteSpace(val) && string.IsNullOrWhiteSpace(text))
		{
			val = "0";
			text = "-Select-";
		}

		item = ddl.Items.FindByValue(val);
		if (item == null)
		{
			item = new ListItem(string.IsNullOrWhiteSpace(text) ? val : text, val);
			ddl.Items.Insert(0, item);
		}

		item.Selected = true;
	}

    public static Image CreateGridImageButton(string itemId = "", string item = "", string id = "", string image = "", int folderLevel = 0, string onClick = "", string toolTip = "", string altText = "", string style = "")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgDelete_click('{0}','{1}');return false;", itemId, item);

        string imgUrl = "";
        if (folderLevel > 0)
        {
            for (int i = 1; i <= folderLevel; i++)
            {
                imgUrl += "../";
            }
        }

        imgUrl += image;

        Image img = new Image();
        img.Style["cursor"] = "pointer";
        img.Height = 12;
        img.Width = 12;
        img.ImageUrl = imgUrl;
        img.ID = id;
        img.Attributes["name"] = img.ID;
        img.Attributes.Add("itemId", itemId.ToString());
        img.ToolTip = string.IsNullOrWhiteSpace(toolTip) ? img.ID : toolTip;
        img.AlternateText = string.IsNullOrWhiteSpace(altText) ? toolTip : altText;
        img.Attributes.Add("Onclick", onClick);
        img.Attributes["style"] = style;

        return img;
    }

    public static Image CreateGridDeleteButton(string itemId = "", string item = "", int folderLevel = 0, string altImage = null)
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("imgDelete_click('{0}','{1}');return false;", itemId, item);

		string imgUrl = "";
		if (folderLevel > 0)
		{
			for (int i = 1; i <= folderLevel; i++)
			{
				imgUrl += "../";
			}
		}

        if (altImage != null)
        {
            imgUrl += altImage;
        }
        else
        {
            imgUrl += "Images/Icons/delete.png";
        }

		Image imgDelete = new Image();
		imgDelete.Style["cursor"] = "pointer";
		imgDelete.Height = 12;
		imgDelete.Width = 12;
		imgDelete.ImageUrl = imgUrl;
		imgDelete.ID = string.Format("imgDelete_{0}", itemId);
		imgDelete.Attributes["name"] = string.Format("imgDelete_{0}", itemId);
		imgDelete.Attributes.Add("itemId", itemId.ToString());
		imgDelete.ToolTip = string.Format("Delete Item [{0}]", item);
		imgDelete.AlternateText = "Delete Item";
		imgDelete.Attributes.Add("Onclick", sb.ToString());

		return imgDelete;
	}

    public static Image CreateGridDownloadButton(string itemId = "", string item = "", int folderLevel = 0)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgDownload_click('{0}','{1}');return false;", itemId, item);

        string imgUrl = "";
        if (folderLevel > 0)
        {
            for (int i = 1; i <= folderLevel; i++)
            {
                imgUrl += "../";
            }
        }
        imgUrl += "Images/Icons/disk.png";

        Image imgDownload = new Image();
        imgDownload.Style["cursor"] = "pointer";
        imgDownload.Height = 12;
        imgDownload.Width = 12;
        imgDownload.ImageUrl = imgUrl;
        imgDownload.ID = string.Format("imgDownload_{0}", itemId);
        imgDownload.Attributes["name"] = string.Format("imgDownload_{0}", itemId);
        imgDownload.Attributes.Add("itemId", itemId.ToString());
        imgDownload.ToolTip = string.Format("Download Item [{0}]", item);
        imgDownload.AlternateText = "Download Item";
        imgDownload.Attributes.Add("Onclick", sb.ToString());

        return imgDownload;
    }

    public static HyperLink CreateGridDownloadHyperLink(string itemId = "", string item = "", string linkText = "<u>Download</u>", string href = "", string onClick = "DEFAULTONCLICK")
    {
        HyperLink link = new HyperLink();
        link.ID = string.Format("lnkDownload_{0}", itemId);
        link.Text = linkText;
        link.Attributes["name"] = string.Format("imgDownload_{0}", itemId);
        link.Attributes.Add("itemId", itemId.ToString());
        link.ToolTip = string.Format("Download Item [{0}]", item);

        if (!string.IsNullOrWhiteSpace(onClick))
        {
            if (onClick == "DEFAULTONCLICK")
            {
                onClick = string.Format("lnkDownload_click('{0}','{1}');return false;", itemId, item);
            }

            link.Attributes.Add("onclick", onClick);
        }

        if (!string.IsNullOrWhiteSpace(href))
        {
            link.NavigateUrl = href;
        }

        link.Style.Add("cursor", "pointer");

        return link;

    }

    public static TextBox CreateGridTextBox(string field, string itemId, string text, bool isNumber = false, bool multiLine = false, int maxLength = 0)
	{
		TextBox txt = new TextBox();

        if (multiLine)
        {
            txt.Wrap = true;
            txt.TextMode = TextBoxMode.MultiLine;
        }

        txt.ID = string.Format("txt{0}_{1}", field.Trim().Replace(" ", ""), itemId);
		txt.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(text));
		txt.Attributes.Add("itemId", itemId);
		txt.Attributes.Add("original_value", text);
		txt.Attributes["name"] = txt.ID;
		txt.Style["width"] = "95%";
		txt.Style["background-color"] = "#F5F6CE";
		txt.Style["font-family"] = "arial";
		txt.Style["font-size"] = "12px";
        txt.ToolTip = text;
        if (maxLength > 0)
        {
            txt.MaxLength = maxLength;
        }
        if (isNumber)
		{
			txt.Style["text-align"] = "right";
			txt.TextMode = TextBoxMode.Number;
			txt.Style["width"] = "90%";
		}

		return txt;
	}

	public static CheckBox CreateGridCheckBox(string field, string itemId, bool check = false)
	{
		CheckBox chk = new CheckBox();
		chk.ID = string.Format("chk{0}_{1}", field.Trim().Replace(" ", ""), itemId);
		chk.Attributes["name"] = chk.ID;
		chk.Checked = check;
		chk.Attributes.Add("original_value", check ? "1" : "0");
		chk.Attributes.Add("itemId", itemId);

		return chk;
	}

	public static DropDownList CreateGridDropdownList(string field, string itemId, string text, string value, int width = 0)
	{
		DropDownList ddl = new DropDownList();

		try
		{
			ddl.ID = string.Format("ddl{0}_{1}", field.Replace(" ", "_").Trim(), itemId);
			ddl.Style["background-color"] = "#F5F6CE";
			ddl.Style["font-size"] = "12px";
			ddl.Style["font-family"] = "Arial";
			ddl.Attributes.Add("field", field);
			ddl.Attributes.Add("original_value", value);
			if (width > 0)
			{
				ddl.Style["width"] = width.ToString() + "px";
			}
			else
			{
				ddl.Style["width"] = "98%";
			}

			WTSUtility.SelectDdlItem(ddl, value, text);
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}

		return ddl;
	}

	public static DropDownList CreateGridDropdownList(DataTable dt, string field
		, string textField, string valueField
		, string itemId, string value, string text = ""
		, List<string> attributes = null)
	{
		if (string.IsNullOrWhiteSpace(textField) || string.IsNullOrWhiteSpace(valueField))
		{
			return new DropDownList();
		}

		DropDownList ddl = new DropDownList();
		ddl.ID = string.Format("ddl{0}_{1}", field.Replace(" ", ""), itemId);
		ddl.Attributes["name"] = ddl.ID;
		ddl.Attributes.Add("itemId", itemId);
		ddl.Attributes.Add("original_value", value);
		ddl.Style["width"] = "99%";
		ddl.Style["background-color"] = "#F5F6CE";
		ddl.Style["font-size"] = "12px";
		ddl.Style["font-family"] = "Arial";
		//ddl.Items.Add(new ListItem("", "0"));

		if (dt == null)
		{
			return ddl;
		}

		ListItem item = null;
		foreach (DataRow row in dt.Rows)
		{
			item = new ListItem();
			item.Text = row[textField].ToString().Replace("&nbsp;"," ").Trim();
			item.Value = row[valueField].ToString();
			if (attributes != null && attributes.Count > 0)
			{
				foreach (string key in attributes)
				{
					item.Attributes.Add(key.Trim(), key.Trim());
				}
			}

			if (ddl.Items.FindByValue(item.Value) == null)
			{
				ddl.Items.Add(item);
			}
		}

		WTSUtility.SelectDdlItem(ddl, string.IsNullOrWhiteSpace(value) ? "0" : value, text);

		return ddl;
	}

	public static iti_Tools_Sharp.DynamicHeader CreateGridMultiHeader(DataTable dt)
	{
		iti_Tools_Sharp.DynamicHeader dynamicHeader = null;

		try
		{
			dynamicHeader = new iti_Tools_Sharp.DynamicHeader();
			iti_Tools_Sharp.DynamicHeaderRow firstHeaderRow = new iti_Tools_Sharp.DynamicHeaderRow();
			dynamicHeader.Rows.Add(firstHeaderRow);

			//'Create a second row
			iti_Tools_Sharp.DynamicHeaderRow secondHeaderRow = new iti_Tools_Sharp.DynamicHeaderRow();
			dynamicHeader.Rows.Add(secondHeaderRow);

			for (int i = 0; i < dt.Columns.Count; i++)
			{
				iti_Tools_Sharp.DynamicHeaderCell dynamicHeaderCell = new iti_Tools_Sharp.DynamicHeaderCell();
				dynamicHeaderCell.Text = dt.Columns[i].ColumnName;
				firstHeaderRow.Cells.Add(dynamicHeaderCell);

				dynamicHeaderCell = new iti_Tools_Sharp.DynamicHeaderCell();
				dynamicHeaderCell.Text = dt.Columns[i].ColumnName;
				secondHeaderRow.Cells.Add(dynamicHeaderCell);
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
		
		return dynamicHeader;
	}

	/// <summary>
	/// Return theme to use based on database setting for the logged in user
	/// </summary>
	public static string ThemeName
	{
		get
		{
			return UserManagement.GetUserTheme();
		}
	}


	/// <summary>
	/// Functionality for sending email message
	/// </summary>
	/// <param name="toAddress">direct recipient of email</param>
	/// <param name="displayName">display name of recipient</param>
	/// <param name="subject">email subject text</param>
	/// <param name="body">email body text</param>
	/// <param name="formatAsHtml">flag for formatting</param>
	/// <param name="priority">message priority</param>
	/// <returns>true if email sent succesfully</returns>
	public static bool Send_Email(string toAddress
		, string displayName
		, string subject
		, string body
		, string fromAddress = ""
		, string fromDisplayName = ""
		, bool formatAsHtml = true
		, MailPriority priority = MailPriority.Normal
        , List<Attachment> attachments = null
        , bool sendCcToSystemEmail = true)
	{
		if (string.IsNullOrWhiteSpace(toAddress.Trim())) { return false; }
		MailAddress toMailAddress = new MailAddress(toAddress, string.IsNullOrWhiteSpace(displayName) ? toAddress : displayName.Trim());

		MailAddress fromMailAddress = new MailAddress(string.IsNullOrWhiteSpace(fromAddress) ? WTSConfiguration.EmailFrom : fromAddress
			, string.IsNullOrWhiteSpace(fromDisplayName) ? WTSConfiguration.EmailFromName : fromDisplayName);

		return Send_Email(new List<MailAddress>() { toMailAddress }
			, new List<MailAddress>()
			, new List<MailAddress>()
			, subject
			, body
			, fromMailAddress
			, formatAsHtml
			, priority
            , attachments
            , sendCcToSystemEmail);
	}

	public static bool Send_Email(Dictionary<string, string> toAddresses
		, Dictionary<string, string> ccAddresses
		, Dictionary<string, string> bccAddresses
		, string subject
		, string body
		, string fromAddress = ""
		, string fromDisplayName = ""
		, bool formatAsHtml = true
		, MailPriority priority = MailPriority.Normal
        , List<Attachment> attachments = null
        , bool sendCcToSystemEmail = true)
	{
		List<MailAddress> toMailAddresses = new List<MailAddress>();
		List<MailAddress> ccMailAddresses = new List<MailAddress>();
		List<MailAddress> bccMailAddresses = new List<MailAddress>();
		MailAddress fromMailAddress = null;

		if (toAddresses == null
			|| toAddresses.Count == 0)
		{
			return false;
		}

		foreach (string toKey in toAddresses.Keys)
		{
			toMailAddresses.Add(new MailAddress(toKey, toAddresses[toKey]));
		}

		if (ccAddresses != null)
		{
			foreach (string ccKey in ccAddresses.Keys)
			{
				ccMailAddresses.Add(new MailAddress(ccKey, ccAddresses[ccKey]));
			}
		}

		if (bccAddresses != null)
		{
			foreach (string bccKey in bccAddresses.Keys)
			{
				bccMailAddresses.Add(new MailAddress(bccKey, bccAddresses[bccKey]));
			}
		}

		fromMailAddress = new MailAddress(string.IsNullOrWhiteSpace(fromAddress) ? WTSConfiguration.EmailFrom : fromAddress
			, string.IsNullOrWhiteSpace(fromDisplayName) ? WTSConfiguration.EmailFromName : fromDisplayName);

		return Send_Email(toMailAddresses
			, ccMailAddresses
			, bccMailAddresses
			, subject
			, body
			, fromMailAddress
			, formatAsHtml
			, System.Net.Mail.MailPriority.Normal
            , attachments
            , sendCcToSystemEmail);
	}

	public static bool Send_Email(List<MailAddress> toAddresses
		, List<MailAddress> ccAddresses
		, List<MailAddress> bccAddresses
		, string subject
		, string body
		, System.Net.Mail.MailAddress fromAddress
		, bool formatAsHtml = true
		, MailPriority priority = MailPriority.Normal
        , List<Attachment> attachments = null
        , bool sendCcToSystemEmail = true)
	{
		MailMessage msg = new MailMessage();
		msg.From = fromAddress;

		if (toAddresses == null
			|| toAddresses.Count == 0)
		{
			return false;
		}

        StringBuilder originalAddresses = new StringBuilder();

		foreach (MailAddress toAddr in toAddresses)
		{
			msg.To.Add(toAddr);
            originalAddresses.Append(toAddr.Address + ";");

        }
		if (ccAddresses != null
			&& ccAddresses.Count > 0)
		{
			foreach (MailAddress ccAddr in ccAddresses)
			{
				msg.CC.Add(ccAddr);
                originalAddresses.Append(ccAddr.Address + ";");
            }
		}
        if (sendCcToSystemEmail)
        {
            msg.CC.Add(new System.Net.Mail.MailAddress(WTSConfiguration.EmailFrom, WTSConfiguration.EmailFromName));
            originalAddresses.Append(WTSConfiguration.EmailFrom + ";");
        }

		if (bccAddresses != null
			&& bccAddresses.Count > 0)
		{
			foreach (MailAddress bccAddr in bccAddresses)
			{
				msg.Bcc.Add(bccAddr);
                originalAddresses.Append(bccAddr.Address + ";");
            }
		}

		if (!subject.StartsWith(WTSConfiguration.ApplicationAcronym, StringComparison.CurrentCultureIgnoreCase))
		{
			subject = string.Format("{0}: {1}", WTSConfiguration.ApplicationAcronym, subject);
		}
		msg.Subject = subject.Trim();
		msg.Priority = priority;

		msg.BodyEncoding = Encoding.UTF8;
		msg.IsBodyHtml = formatAsHtml;
		if (formatAsHtml)
		{
			body = string.Format("<pre>{0}</pre>", body);
		}
		msg.Body = body;

        if (attachments != null) attachments.ForEach(att => msg.Attachments.Add(att));

		try
		{
			SmtpClient client = new SmtpClient();

			System.Net.Configuration.SmtpSection smtpSec = (System.Net.Configuration.SmtpSection)System.Configuration.ConfigurationManager.GetSection("system.net/mailSettings/smtp");

			if (smtpSec.Network.Port == 465
					|| smtpSec.Network.Port == 587)
			{
				client.EnableSsl = true;
			}
			else
			{
				client.EnableSsl = false;
			}

			// Ignore certificate errors
			System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            if (WTSConfiguration.EmailEnabled)
            {
                if (!string.IsNullOrWhiteSpace(WTSConfiguration.EmailOverride))
                {
                    msg.Subject += " ----- EMAIL OVERRIDE";
                    msg.Body = "----- EMAIL OVERRIDE ----- " + originalAddresses.ToString() + " ----- " + (formatAsHtml ? "<br /><br />" : "\n\n") + msg.Body;

                    msg.To.Clear();
                    msg.CC.Clear();
                    msg.Bcc.Clear();
                    msg.To.Add(WTSConfiguration.EmailOverride);
                }

                client.Send(msg);
            }

			return true;
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			return false;
		}
	}

	public static DataTable JoinDataTables(DataTable t1, DataTable t2, params Func<DataRow, DataRow, bool>[] joinOn) //allows you to perform simple table joins in the .NET side. Utility function. 
	{
		if (t1 == null || t1.Rows.Count == 0)
		{
			return t2;
		}

		if (t2 == null || t2.Rows.Count == 0)
		{
			return t1;
		}

		DataTable result = new DataTable();
		foreach (DataColumn col in t1.Columns)
		{
			if (result.Columns[col.ColumnName] == null)
				result.Columns.Add(col.ColumnName, col.DataType);
		}
		foreach (DataColumn col in t2.Columns)
		{
			if (result.Columns[col.ColumnName] == null)
				result.Columns.Add(col.ColumnName, col.DataType);
		}
		foreach (DataRow row1 in t1.Rows)
		{
			var joinRows = t2.AsEnumerable().Where(row2 =>
			{
				foreach (var parameter in joinOn)
				{
					if (!parameter(row1, row2)) return false;
				}
				return true;
			});
			foreach (DataRow fromRow in joinRows)
			{
				DataRow insertRow = result.NewRow();
				foreach (DataColumn col1 in t1.Columns)
				{
					insertRow[col1.ColumnName] = row1[col1.ColumnName];
				}
				foreach (DataColumn col2 in t2.Columns)
				{
					insertRow[col2.ColumnName] = fromRow[col2.ColumnName];
				}
				result.Rows.Add(insertRow);
			}
		}
		return result;
	}
	public static void importDataRow(ref DataTable dt, DataRow dr) //simple utility function that imports a given data row, and changes it column type to string. Intended to be used for excel output. 
	{
		if (dt == null || dt.Rows.Count == 0)
		{
			dt = dr.Table.Clone();
			foreach (DataColumn c in dt.Columns)
			{
				c.DataType = typeof(string);
				c.AllowDBNull = true;
			}
		}
		dt.ImportRow(dr);
	}

    public static byte[] ConvertDataTableToImage(DataTable dt, int intImageWidth = 1000)
    {
        try
        {
            License lic = new License();

            lic.SetLicense("Aspose.Cells.licx");
        }
        catch(Exception ex)
        {
            LogUtility.LogException(ex);
        }

        try
        {
            Workbook workbook = new Workbook(FileFormatType.Excel97To2003);
            Worksheet ws = workbook.Worksheets[0];
            Aspose.Cells.Style ww = new Aspose.Cells.Style();
            StyleFlag sf = new StyleFlag();
            int intColumnWidth = intImageWidth / dt.Columns.Count;
            sf.All = true;
            ww.IsTextWrapped = true;
            ww.HorizontalAlignment = TextAlignmentType.Left;
            ww.VerticalAlignment = TextAlignmentType.Top;
            ww.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, System.Drawing.Color.Black);
            ww.SetBorder(BorderType.TopBorder, CellBorderType.Thin, System.Drawing.Color.Black);
            ww.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, System.Drawing.Color.Black);
            ww.SetBorder(BorderType.RightBorder, CellBorderType.Thin, System.Drawing.Color.Black);
            ws.Cells.ImportDataTable(dt, true, "A1");
            //ws.Cells.DeleteRow(0);
            ws.Cells.ApplyStyle(ww, sf);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                //ws.Cells.SetColumnWidthPixel(i, intColumnWidth);
                switch (dt.Columns[i].ColumnName)
                {
                    case "AOR #":
                        ws.Cells.SetColumnWidthPixel(i, 50);
                        break;
                    case "AOR Name":
                        ws.Cells.SetColumnWidthPixel(i, 350);
                        break;
                    case "INV":
                    case "TD":
                    case "CD":
                    case "C":
                    case "IT":
                    case "CVT":
                    case "A":
                        ws.Cells.SetColumnWidthPixel(i, 35);
                        break;
                }
            }
            ws.AutoFitRows();
            ws.PageSetup.LeftMargin = 0;
            ws.PageSetup.RightMargin = 0;
            ws.PageSetup.BottomMargin = 0;
            ws.PageSetup.TopMargin = 0;
            foreach (Cell c in ws.Cells)
            {
                if (c.StringValue == "&nbsp;") c.Value = "";
            }
            ImageOrPrintOptions imgOptions = new ImageOrPrintOptions();
            imgOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Png;
            imgOptions.HorizontalResolution = 600;
            imgOptions.VerticalResolution = 600;
            imgOptions.OnePagePerSheet = true;
            imgOptions.IsCellAutoFit = true;
            SheetRender sr = new SheetRender(ws, imgOptions);
            System.Drawing.Bitmap bitmap = sr.ToImage(0);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            //bitmap.Save("C:/inetpub/wwwroot/Dev/WTS/Images/test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            return ms.GetBuffer();
        }
        catch (Exception)
        {
            return null;
        }
    }
}


/// <summary>
/// Commonly used project functionality
/// </summary>
public sealed class WTSCommon
{
	/// <summary>
	/// WTS database connection string
	/// </summary>
	public static string ConnectionString
	{
		get
		{
			string conn = System.Configuration.ConfigurationManager.ConnectionStrings["WTS"].ConnectionString;
			if (!conn.Contains("{"))
			{
				return conn;
			}
			string db = System.Configuration.ConfigurationManager.AppSettings["DatabaseName"];
			string userName = "WTS_USER";

			//Grab the encrypted password.
			RegistryKey rk = null;

			if (WTSConfiguration.StoreLocation == DataProtectionScope.CurrentUser)
			{
				rk = Registry.CurrentUser.OpenSubKey(@"Software\ITI\WTS", false);
			}
			else
			{
				rk = Registry.LocalMachine.OpenSubKey(@"Software\ITI\WTS", false);
			}
			Byte[] pw = Convert.FromBase64String(rk.GetValue(userName).ToString());

			//Decrypt the password.
			string password = Encoding.ASCII.GetString(Infinite.Security.DataProtector.Decrypt(pw));

			//return the formatted connection string
			return string.Format(conn, db, userName, password);
		}
	}


	/// <summary>
	/// SQL Server SQL Authentication Connection String
	/// </summary>
	public static string WTS_ConnectionString
	{
		get
		{
			string authType = ConfigurationManager.AppSettings.Get("AuthType");

			if (!string.IsNullOrWhiteSpace(authType) && authType.ToUpper() == "WIN")
			{
				return WTS_ConnectionString_WinAuth;
			}

			string server = ConfigurationManager.AppSettings.Get("Server");
			string conn = ConfigurationManager.ConnectionStrings["WTS_SQLAuth"].ConnectionString;
			string db = ConfigurationManager.AppSettings.Get("DatabaseName");
			string user = "WTS_USER";
			if (!conn.Contains("{"))
			{
				return conn;
			}
			

			//Get encrypted password
			RegistryKey rk = null;

			if (WTSConfiguration.StoreLocation == DataProtectionScope.CurrentUser)
			{
				rk = Registry.CurrentUser.OpenSubKey(@"Software\ITI\WTS", false);
			}
			else
			{
				rk = Registry.LocalMachine.OpenSubKey(@"Software\ITI\WTS", false);
			}
			Byte[] pw = Convert.FromBase64String(rk.GetValue(user).ToString());

			//Decrypt the password.
			string password = Encoding.ASCII.GetString(Infinite.Security.DataProtector.Decrypt(pw));

            ////TEMPORARY:
            //password = "!QAZ2wsx";

            //Build the connection string.
            return String.Format(conn, server, db, user, password);
            //return "Server = sqlservervm1; database = WTS; Integrated Security = SSPI; Persist Security Info = False;";

        }
	}
	/// <summary>
	/// SQL Server Windows Authentication Connection String
	/// </summary>
	public static string WTS_ConnectionString_WinAuth
	{
		get
		{
			string server = ConfigurationManager.AppSettings.Get("Server");
			string conn = ConfigurationManager.ConnectionStrings["WTS_WinAuth"].ConnectionString;
			string db = ConfigurationManager.AppSettings.Get("DatabaseName");


			//Build the connection string.
			return String.Format(conn, server, db);
		}
	}

	
	public static string GetImageTypeURL(string imageType)
	{
		string strURL = "";
		switch (imageType.ToUpper())
		{
			case "HELP":
				strURL = "images/icons/help.png";
				break;
			case "DOC":
			case "DOCX":
				strURL = "images/icons/doc_16.png";
				break;
			case "XLS":
			case "XLSX":
				strURL = "images/icons/xls_16.png";
				break;
			case "PPT":
			case "PPTX":
				strURL = "images/icons/ppt_16.png";
				break;
			case "PDF":
				strURL = "images/icons/pdf_16.png";
				break;
			case "WEBSYSTEM":
			case "ABOUT":
				strURL = "images/infintech_black.png";
				break;
			case "FOLDER":
				strURL = "images/folder.gif";
				break;
			default:
				strURL = "images/icons/blank.png";
				break;
		}

		return strURL;
	}

	/// <summary>
	/// Clear all "expired" session view settings
	/// </summary>
	/// <returns></returns>
	public static bool CleanUserViews()
	{
		bool deleted = false;
		string msg = string.Empty;

		string procName = "Clean_User_SessionViews";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
					cmd.ExecuteNonQuery();

					SqlParameter paramSaved = cmd.Parameters["@deleted"];
					if (paramSaved != null)
					{
						bool.TryParse(paramSaved.Value.ToString(), out deleted);
					}
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
				}
			}
		}

		return deleted;
	}
}


public class GridView
{
	#region Properties

	public int ID { get; set; }
	public int GridNameID { get; set; }
	private string _gridName;
	/// <summary>
	/// Grid which this applies to
	/// </summary>
	public string GridName
	{
		get { return _gridName; }
		set { _gridName = value; }
	}

    public string ViewName { get; set; }

	private string _name = string.Empty;
	/// <summary>
	/// User specified name of saved GridSettings object
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { _name = value; }
	}
	/// <summary>
	/// Save for specified user
	/// </summary>
	public int UserID { get; set; }
	private string _sessionId = string.Empty;
	/// <summary>
	/// Save for specified session
	/// </summary>
	public string SessionID
	{
		get { return _sessionId; }
		set { _sessionId = value; }
	}

	private string _tier1Columns = string.Empty;
	/// <summary>
	/// top-level selected grid columns
	/// </summary>
	public string Tier1Columns
	{
		get { return _tier1Columns; }
		set { _tier1Columns = value; }
	}
	private string _tier1ColumnOrder = string.Empty;
	/// <summary>
	/// top-level grid columns order
	/// </summary>
	public string Tier1ColumnOrder
	{
		get { return _tier1ColumnOrder; }
		set { _tier1ColumnOrder = value; }
	}
	private string _tier1SortOrder = string.Empty;
	/// <summary>
	/// top-level grid sort order
	/// </summary>
	public string Tier1SortOrder
	{
		get { return _tier1SortOrder; }
		set { _tier1SortOrder = value; }
	}
	private string _tier1RollupGroup = string.Empty;
	/// <summary>
	/// top-level grid rollup group
	/// </summary>
	public string Tier1RollupGroup
	{
		get { return _tier1RollupGroup; }
		set { _tier1RollupGroup = value; }
	}

	private string _tier2Columns = string.Empty;
	/// <summary>
	/// second-level selected grid columns
	/// </summary>
	public string Tier2Columns
	{
		get { return _tier2Columns; }
		set { _tier2Columns = value; }
	}
	private string _tier2ColumnOrder = string.Empty;
	/// <summary>
	/// second-level grid columns order
	/// </summary>
	public string Tier2ColumnOrder
	{
		get { return _tier2ColumnOrder; }
		set { _tier2ColumnOrder = value; }
	}
	private string _tier2SortOrder = string.Empty;
	/// <summary>
	/// second-level grid sort order
	/// </summary>
	public string Tier2SortOrder
	{
		get { return _tier2SortOrder; }
		set { _tier2SortOrder = value; }
	}
	private string _tier2RollupGroup = string.Empty;
	/// <summary>
	/// second-level grid rollup group
	/// </summary>
	public string Tier2RollupGroup
	{
		get { return _tier2RollupGroup; }
		set { _tier2RollupGroup = value; }
	}

	private string _tier3Columns = string.Empty;
	/// <summary>
	/// third-level selected grid columns
	/// </summary>
	public string Tier3Columns
	{
		get { return _tier3Columns; }
		set { _tier3Columns = value; }
	}
	private string _tier3ColumnOrder = string.Empty;
	/// <summary>
	/// third-level grid columns order
	/// </summary>
	public string Tier3ColumnOrder
	{
		get { return _tier3ColumnOrder; }
		set { _tier3ColumnOrder = value; }
	}
	private string _tier3SortOrder = string.Empty;
	/// <summary>
	/// third-level grid sort order
	/// </summary>
	public string Tier3SortOrder
	{
		get { return _tier3SortOrder; }
		set { _tier3SortOrder = value; }
	}

	/// <summary>
	/// Order to load in dropdowns
	/// </summary>
	public int SORT_ORDER { get; set; }

	/// <summary>
	/// WTS Resource that created this view
	/// </summary>
	public string CreatedBy { get; set; }
	/// <summary>
	/// Date on which this view was created
	/// </summary>
	public string CreatedDate { get; set; }
	private string _updatedBy = string.Empty;
	/// <summary>
	/// WTS Resource that last updated this view
	/// </summary>
	public string UpdatedBy
	{
		get { return _updatedBy; }
		set { _updatedBy = value; }
	}
	private string _updatedDate = string.Empty;
	/// <summary>
	/// date on which this was Last updated
	/// </summary>
	public string UpdatedDate
	{
		get { return _updatedDate; }
		set { _updatedDate = value; }
	}

    public int WTS_RESOURCEID { get; set; }
	
	private System.Xml.XmlDocument _SectionsXML;
	/// <summary>
	/// Crosswalk Sections and Breakouts, in XML
	/// </summary>
	public System.Xml.XmlDocument SectionsXML
	{
		get { return _SectionsXML; }
		set { _SectionsXML = value; }
	}

    public int ViewType { get; set; }

	#endregion Properties


	public GridView() { }
	public GridView(int gridViewID)
	{
		this.ID = gridViewID;
	}
	public GridView(string sessionID, int userID)
	{
		this.SessionID = sessionID;
		this.UserID = userID;
	}

	public bool Load()
	{
		DataTable dt = GetGridViewData();

		if (dt != null && dt.Rows.Count > 0)
		{
			int tempId = 0;
			DataRow dr = dt.Rows[0];

            this.ID = (int)dr["GridViewID"];
            this.ViewName = (string)dr["ViewName"];

            this.GridNameID = int.TryParse(dr["GridNameID"].ToString(), out tempId) ? tempId : 0;
			this.GridName = dr["GridName"].ToString();
			this.UserID = int.TryParse(dr["WTS_RESOURCEID"].ToString(), out tempId) ? tempId : 0;
			this.SessionID = dr["SessionID"].ToString();
			this.Name = dr["ViewName"].ToString();
			this.Tier1Columns = dr["Tier1Columns"].ToString();
			this.Tier1ColumnOrder = dr["Tier1ColumnOrder"].ToString();
			this.Tier1SortOrder = dr["Tier1SortOrder"].ToString();
			this.Tier1RollupGroup = dr["Tier1RollupGroup"].ToString();
			this.Tier2Columns = dr["Tier2Columns"].ToString();
			this.Tier2ColumnOrder = dr["Tier2ColumnOrder"].ToString();
			this.Tier2SortOrder = dr["Tier2SortOrder"].ToString();
			this.Tier2RollupGroup = dr["Tier2RollupGroup"].ToString();
			this.Tier3Columns = dr["Tier3Columns"].ToString();
			this.Tier3ColumnOrder = dr["Tier3ColumnOrder"].ToString();
			this.Tier3SortOrder = dr["Tier3SortOrder"].ToString();
			this.SORT_ORDER = int.TryParse(dr["SORT_ORDER"].ToString(), out tempId) ? tempId : 0;
			this.CreatedBy = dr["CREATEDBY"].ToString();
			this.CreatedDate = dr["CREATEDDATE"].ToString();
			this.UpdatedBy = dr["UPDATEDBY"].ToString();
			this.UpdatedDate = dr["UPDATEDDATE"].ToString();

            string sectionsXML = dr["SectionsXML"] != DBNull.Value ? (string)dr["SectionsXML"] : null;
            if (!string.IsNullOrWhiteSpace(sectionsXML))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.InnerXml = sectionsXML;

                this.SectionsXML = xmlDoc;
            }
		    this.ViewType = Convert.ToInt32(dr["ViewType"]);
			return true;
		}
		else
		{
			return false;
		}
	}
	public bool Load_ByUserSession()
	{
		return false;
	}

	private DataTable GetGridViewData()
	{
		string procName = "GridView_Get";

		using (DataTable dt = new DataTable("GridView"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@GridViewID", SqlDbType.Int).Value = this.ID == 0 ? (object)DBNull.Value : this.ID;
					cmd.Parameters.Add("@ViewName", SqlDbType.VarChar).Value = this.ViewName;
                    cmd.Parameters.Add("@GridNameID", SqlDbType.Int).Value = this.GridNameID == 0 ? (object)DBNull.Value : this.GridNameID;

                    try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						throw;
					}
				}
			}
		}
	}

	public bool Save(out string errorMsg)
	{
		errorMsg = string.Empty;

		if (this.ID > 0)
		{
			return this.Update(out errorMsg);
		}
		else
		{
			return this.Add(out errorMsg);
		}
	}

	private bool Add(out string errorMsg)
	{
		errorMsg = string.Empty;
		int newID = 0;
		bool saved = false;

		string procName = "GridView_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@GridNameID", SqlDbType.Int).Value = this.GridNameID;
				cmd.Parameters.Add("@ViewName", SqlDbType.NVarChar).Value = this.Name.Trim();
				cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.SessionID) ? (object) DBNull.Value : this.SessionID.Trim();
				cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = this.UserID == 0 ? (object) DBNull.Value : this.UserID;
				cmd.Parameters.Add("@Tier1Columns", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1Columns) ? (object) DBNull.Value : this.Tier1Columns;
				cmd.Parameters.Add("@Tier1ColumnOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1ColumnOrder) ? (object) DBNull.Value : this.Tier1ColumnOrder;
				cmd.Parameters.Add("@Tier1SortOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1SortOrder) ? (object) DBNull.Value : this.Tier1SortOrder;
				cmd.Parameters.Add("@Tier1RollupGroup", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1RollupGroup) ? (object) DBNull.Value : this.Tier1RollupGroup;
				cmd.Parameters.Add("@Tier2Columns", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2Columns) ? (object) DBNull.Value : this.Tier2Columns;
				cmd.Parameters.Add("@Tier2ColumnOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2ColumnOrder) ? (object) DBNull.Value : this.Tier2ColumnOrder;
				cmd.Parameters.Add("@Tier2SortOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2SortOrder) ? (object) DBNull.Value : this.Tier2SortOrder;
				cmd.Parameters.Add("@Tier2RollupGroup", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2RollupGroup) ? (object) DBNull.Value : this.Tier2RollupGroup;
				cmd.Parameters.Add("@Tier3Columns", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier3Columns) ? (object) DBNull.Value : this.Tier3Columns;
				cmd.Parameters.Add("@Tier3ColumnOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier3ColumnOrder) ? (object) DBNull.Value : this.Tier3ColumnOrder;
				cmd.Parameters.Add("@Tier3SortOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier3SortOrder) ? (object) DBNull.Value : this.Tier3SortOrder;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = this.SORT_ORDER;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@ViewType", SqlDbType.Int).Value = this.ViewType;

                if (this.SectionsXML != null) cmd.Parameters.Add("@SectionsXML", SqlDbType.Xml).Value = this.SectionsXML.InnerXml;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd.ExecuteNonQuery();

				SqlParameter paramNewID = cmd.Parameters["@newID"];
				if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
				{
					this.ID = newID;
					saved = true;
				}
			}
		}
		return saved;
	}

	private bool Update(out string errorMsg)
	{
		errorMsg = string.Empty;
		bool duplicate = false, saved = false;

		string procName = "GridView_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@GridViewID", SqlDbType.Int).Value = this.ID;
				cmd.Parameters.Add("@GridNameID", SqlDbType.Int).Value = this.GridNameID;
				cmd.Parameters.Add("@ViewName", SqlDbType.NVarChar).Value = this.Name.Trim();
				cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.SessionID) ? (object)DBNull.Value : this.SessionID.Trim();
				cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = this.UserID == 0 ? (object)DBNull.Value : this.UserID;
				cmd.Parameters.Add("@Tier1Columns", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1Columns) ? (object)DBNull.Value : this.Tier1Columns;
				cmd.Parameters.Add("@Tier1ColumnOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1ColumnOrder) ? (object)DBNull.Value : this.Tier1ColumnOrder;
				cmd.Parameters.Add("@Tier1SortOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1SortOrder) ? (object)DBNull.Value : this.Tier1SortOrder;
				cmd.Parameters.Add("@Tier1RollupGroup", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1RollupGroup) ? (object)DBNull.Value : this.Tier1RollupGroup;
				cmd.Parameters.Add("@Tier2Columns", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2Columns) ? (object)DBNull.Value : this.Tier2Columns;
				cmd.Parameters.Add("@Tier2ColumnOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2ColumnOrder) ? (object)DBNull.Value : this.Tier2ColumnOrder;
				cmd.Parameters.Add("@Tier2SortOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2SortOrder) ? (object)DBNull.Value : this.Tier2SortOrder;
				cmd.Parameters.Add("@Tier2RollupGroup", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2RollupGroup) ? (object)DBNull.Value : this.Tier2RollupGroup;
				cmd.Parameters.Add("@Tier3Columns", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier3Columns) ? (object)DBNull.Value : this.Tier3Columns;
				cmd.Parameters.Add("@Tier3ColumnOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier3ColumnOrder) ? (object)DBNull.Value : this.Tier3ColumnOrder;
				cmd.Parameters.Add("@Tier3SortOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier3SortOrder) ? (object)DBNull.Value : this.Tier3SortOrder;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = this.SORT_ORDER;
			    cmd.Parameters.Add("@ViewType", SqlDbType.Int).Value = this.ViewType;

                if (this.SectionsXML != null) cmd.Parameters.Add("@SectionsXML", SqlDbType.Xml).Value = this.SectionsXML.InnerXml;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
				cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDuplicate = cmd.Parameters["@duplicate"];
				if (paramDuplicate != null)
				{
					bool.TryParse(paramDuplicate.Value.ToString(), out duplicate);
				}
				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	private bool UpdateTESTSections(out string errorMsg)
	{
		errorMsg = string.Empty;
		bool duplicate = false, saved = false;

		string procName = "GridView_UpdateSections";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@GridViewID", SqlDbType.Int).Value = this.ID;
				cmd.Parameters.Add("@GridNameID", SqlDbType.Int).Value = this.GridNameID;
				cmd.Parameters.Add("@ViewName", SqlDbType.NVarChar).Value = this.Name.Trim();
				cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.SessionID) ? (object)DBNull.Value : this.SessionID.Trim();
				cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = this.UserID == 0 ? (object)DBNull.Value : this.UserID;
				cmd.Parameters.Add("@Tier1Columns", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1Columns) ? (object)DBNull.Value : this.Tier1Columns;
				cmd.Parameters.Add("@Tier1ColumnOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1ColumnOrder) ? (object)DBNull.Value : this.Tier1ColumnOrder;
				cmd.Parameters.Add("@Tier1SortOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1SortOrder) ? (object)DBNull.Value : this.Tier1SortOrder;
				cmd.Parameters.Add("@Tier1RollupGroup", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier1RollupGroup) ? (object)DBNull.Value : this.Tier1RollupGroup;
				cmd.Parameters.Add("@Tier2Columns", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2Columns) ? (object)DBNull.Value : this.Tier2Columns;
				cmd.Parameters.Add("@Tier2ColumnOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2ColumnOrder) ? (object)DBNull.Value : this.Tier2ColumnOrder;
				cmd.Parameters.Add("@Tier2SortOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2SortOrder) ? (object)DBNull.Value : this.Tier2SortOrder;
				cmd.Parameters.Add("@Tier2RollupGroup", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier2RollupGroup) ? (object)DBNull.Value : this.Tier2RollupGroup;
				cmd.Parameters.Add("@Tier3Columns", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier3Columns) ? (object)DBNull.Value : this.Tier3Columns;
				cmd.Parameters.Add("@Tier3ColumnOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier3ColumnOrder) ? (object)DBNull.Value : this.Tier3ColumnOrder;
				cmd.Parameters.Add("@Tier3SortOrder", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Tier3SortOrder) ? (object)DBNull.Value : this.Tier3SortOrder;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = this.SORT_ORDER;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@SectionsXML", SqlDbType.Xml).Value = string.IsNullOrWhiteSpace(this.Tier3SortOrder) ? (object)DBNull.Value : this.SectionsXML;

				cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDuplicate = cmd.Parameters["@duplicate"];
				if (paramDuplicate != null)
				{
					bool.TryParse(paramDuplicate.Value.ToString(), out duplicate);
				}
				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	public bool Delete(out bool exists, out string errorMsg)
	{
		exists = false;
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "GridView_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@GridViewID", SqlDbType.Int).Value = this.ID;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;
				
				cmd.ExecuteNonQuery();

				SqlParameter paramExists = cmd.Parameters["@exists"];
				if (paramExists != null)
				{
					bool.TryParse(paramExists.Value.ToString(), out exists);
					if (!exists)
					{
						errorMsg = "Priority record could not be found.";
						return false;
					}
				}				
				SqlParameter paramDeleted = cmd.Parameters["@deleted"];
				if (paramDeleted != null)
				{
					bool.TryParse(paramDeleted.Value.ToString(), out deleted);
				}
			}
		}

		return deleted;
	}
}