using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;


public partial class RequestGroupGrid_Requests : System.Web.UI.Page
{
	protected bool CanView = false;
	protected bool CanEdit = false;

	protected bool _refreshData = false;
	protected bool _export = false;
	protected int _showArchived = 0;

	protected int RequestGroupID = 0;

	protected DataColumnCollection DCC;
	DataTable _dtPriority = null, _dtUsers = null, _dtTDStatus = null, _dtCDStatus = null, _dtCStatus = null, _dtITStatus = null, _dtCVTStatus = null, _dtAStatus = null, _dtCRStatus = null;

	protected string TDStatusHelp = string.Empty, CDStatusHelp = string.Empty, CStatusHelp = string.Empty, ITStatusHelp = string.Empty, CVTStatusHelp = string.Empty, AStatusHelp = string.Empty, CRStatusHelp = string.Empty;

	protected void Page_Load(object sender, EventArgs e)
	{
		this.CanView = UserManagement.UserCanView(WTSModuleOption.WorkItem);
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);

		initGrid();
		readQueryString();

		if (!Page.IsPostBack)
		{
			loadGridData();
		}
	}

	void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}
		if (Request.QueryString["ShowArchived"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"].ToString()), out this._showArchived);
		}
		if (Request.QueryString["RequestGroupID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["RequestGroupID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["RequestGroupID"].ToString()), out this.RequestGroupID);
		}
	}

	private void loadGridData()
	{
		loadGridOptions();
		DataTable dt = null;
		//if (_refreshData || Session["dtGroup_Requests"] == null)
		//{
		dt = RequestGroup.WorkRequestList_Get(requestGroupID: this.RequestGroupID, typeID: 0, showArchived: _showArchived);
		//	HttpContext.Current.Session["dtGroup_Requests"] = dt;
		//}
		//else
		//{
		//	dt = (DataTable)HttpContext.Current.Session["dtGroup_Requests"];
		//}

		if (dt != null)
		{
			using (DataTable dtTemp = dt.Clone())
			{
				this.DCC = dtTemp.Columns;
				Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
			}
		}

		gridRequest.DataSource = dt;
		gridRequest.DataBind();
	}

	private void loadGridOptions()
	{
		DataSet dsOptions = WorkRequest.GetAvailableOptions();
		if (dsOptions != null && dsOptions.Tables.Count > 0)
		{
			if (dsOptions.Tables.Contains("Priority"))
			{
				_dtPriority = dsOptions.Tables["Priority"];
			}
			if (dsOptions.Tables.Contains("User"))
			{
				_dtUsers = dsOptions.Tables["User"];
			}
			if (dsOptions.Tables.Contains("Status"))
			{
				_dtTDStatus = dsOptions.Tables["Status"].Copy();
				_dtTDStatus.DefaultView.RowFilter = " StatusType = 'TD' ";
				_dtTDStatus = _dtTDStatus.DefaultView.ToTable();
				Page.ClientScript.RegisterArrayDeclaration("_dtTDStatus", JsonConvert.SerializeObject(_dtTDStatus, Newtonsoft.Json.Formatting.None));

				_dtCDStatus = dsOptions.Tables["Status"].Copy();
				_dtCDStatus.DefaultView.RowFilter = " StatusType = 'CD' ";
				_dtCDStatus = _dtCDStatus.DefaultView.ToTable();
				Page.ClientScript.RegisterArrayDeclaration("_dtCDStatus", JsonConvert.SerializeObject(_dtCDStatus, Newtonsoft.Json.Formatting.None));

				_dtCStatus = dsOptions.Tables["Status"].Copy();
				_dtCStatus.DefaultView.RowFilter = " StatusType = 'C' ";
				_dtCStatus = _dtCStatus.DefaultView.ToTable();
				Page.ClientScript.RegisterArrayDeclaration("_dtCStatus", JsonConvert.SerializeObject(_dtCStatus, Newtonsoft.Json.Formatting.None));

				_dtITStatus = dsOptions.Tables["Status"].Copy();
				_dtITStatus.DefaultView.RowFilter = " StatusType = 'IT' ";
				_dtITStatus = _dtITStatus.DefaultView.ToTable();
				Page.ClientScript.RegisterArrayDeclaration("_dtITStatus", JsonConvert.SerializeObject(_dtITStatus, Newtonsoft.Json.Formatting.None));

				_dtCVTStatus = dsOptions.Tables["Status"].Copy();
				_dtCVTStatus.DefaultView.RowFilter = " StatusType = 'CVT' ";
				_dtCVTStatus = _dtCVTStatus.DefaultView.ToTable();
				Page.ClientScript.RegisterArrayDeclaration("_dtCVTStatus", JsonConvert.SerializeObject(_dtCVTStatus, Newtonsoft.Json.Formatting.None));

				_dtAStatus = dsOptions.Tables["Status"].Copy();
				_dtAStatus.DefaultView.RowFilter = " StatusType = 'Adopt' ";
				_dtAStatus = _dtAStatus.DefaultView.ToTable();
				Page.ClientScript.RegisterArrayDeclaration("_dtAStatus", JsonConvert.SerializeObject(_dtAStatus, Newtonsoft.Json.Formatting.None));

				_dtCRStatus = dsOptions.Tables["Status"].Copy();
				_dtCRStatus.DefaultView.RowFilter = " StatusType = 'CR' ";
				_dtCRStatus = _dtCRStatus.DefaultView.ToTable();
				Page.ClientScript.RegisterArrayDeclaration("_dtCRStatus", JsonConvert.SerializeObject(_dtCRStatus, Newtonsoft.Json.Formatting.None));
			}
		}
	}

	#region Grid

	void initGrid()
	{
		gridRequest.RowDataBound += gridRequest_RowDataBound;
		gridRequest.PageIndexChanging += gridRequest_PageIndexChanging;
	}

	void gridRequest_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.Header)
		{
			gridRequest_GridHeaderRowDataBound(sender, e);
		}
		else if (e.Row.RowType == DataControlRowType.DataRow)
		{
			gridRequest_GridRowDataBound(sender, e);
		}
	}

	void gridRequest_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		for (int i = 0; i < row.Cells.Count; i++)
		{
			row.Cells[i].Text = row.Cells[i].Text.Replace("_", " ");
			row.Cells[i].Style["vertical-align"] = "middle";
		}

		row.Cells[DCC["WORKREQUESTID"].Ordinal].Text = "#";
		row.Cells[DCC["ReleaseVersion"].Ordinal].Text = "Rel";
		row.Cells[DCC["MSG"].Ordinal].Text = "MSG";
		row.Cells[DCC["PRIORITY"].Ordinal].Text = "Pri";
		row.Cells[DCC["TITLE"].Ordinal].Text = "MS";
		row.Cells[DCC["WORKITEM_COUNT"].Ordinal].Text = "# Work Items";
		row.Cells[DCC["DESCRIPTION"].Ordinal].Text = "Notes";

		row.Cells[DCC["Last_Meeting"].Ordinal].Text = "Last<br />Meeting";
		row.Cells[DCC["Next_Meeting"].Ordinal].Text = "Next<br />Meeting";
		row.Cells[DCC["Dev_Start"].Ordinal].Text = "Dev<br />Start";
		row.Cells[DCC["CIA_Risk"].Ordinal].Text = "CIA<br />Risk";

		row.Cells[DCC["SME"].Ordinal].Text = "SME";
		row.Cells[DCC["Lead_Tech_Writer"].Ordinal].Text = "IA / TW";
		row.Cells[DCC["Lead_Resource"].Ordinal].Text = "Primary<br />Dev";
		row.Cells[DCC["SME"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["SME"].Ordinal].Style["padding-left"] = "0px";
		row.Cells[DCC["Lead_Tech_Writer"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["Lead_Tech_Writer"].Ordinal].Style["padding-left"] = "0px";
		row.Cells[DCC["Lead_Resource"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["Lead_Resource"].Ordinal].Style["padding-left"] = "0px";

		row.Cells[DCC["TD_STATUS"].Ordinal].Text = "TD";
		row.Cells[DCC["TD_STATUS"].Ordinal].Controls.Add(createHeaderTable("TD_STATUS", "TD"));
		row.Cells[DCC["CD_STATUS"].Ordinal].Text = "CD";
		row.Cells[DCC["CD_STATUS"].Ordinal].Controls.Add(createHeaderTable("CD_STATUS", "CD"));
		row.Cells[DCC["C_STATUS"].Ordinal].Text = "C";
		row.Cells[DCC["C_STATUS"].Ordinal].Controls.Add(createHeaderTable("C_STATUS", "C"));
		row.Cells[DCC["IT_STATUS"].Ordinal].Text = "IT";
		row.Cells[DCC["IT_STATUS"].Ordinal].Controls.Add(createHeaderTable("IT_STATUS", "IT"));
		row.Cells[DCC["CVT_STATUS"].Ordinal].Text = "CVT";
		row.Cells[DCC["CVT_STATUS"].Ordinal].Controls.Add(createHeaderTable("CVT_STATUS", "CVT"));
		row.Cells[DCC["A_STATUS"].Ordinal].Text = "Adopt";
		row.Cells[DCC["A_STATUS"].Ordinal].Controls.Add(createHeaderTable("A_STATUS", "Adopt"));
		row.Cells[DCC["CR_STATUS"].Ordinal].Text = "Cyber<br />Rev";
		row.Cells[DCC["CR_STATUS"].Ordinal].Controls.Add(createHeaderTable("CR_STATUS", "Cyber<br />Rev"));

		row.Cells[DCC["Y"].Ordinal].Text = "&nbsp;";

		if (!CanEdit)
		{
			Image imgBlank = new Image();
			imgBlank.Height = 12;
			imgBlank.Width = 12;
			imgBlank.ImageUrl = "Images/Icons/blank.png";
			imgBlank.AlternateText = "";
			row.Cells[DCC["Y"].Ordinal].Controls.Add(imgBlank);
		}
		else
		{
			Image imgSave = new Image();
			imgSave.ID = "imgSave";
			imgSave.Height = 12;
			imgSave.Width = 12;
			imgSave.ImageUrl = "Images/Icons/disk.png";
			imgSave.AlternateText = "Save Request Updates";
			imgSave.ToolTip = "Save Request Updates";
			imgSave.Style["cursor"] = "default";
			row.Cells[DCC["Y"].Ordinal].Controls.Add(imgSave);
		}
	}

	void gridRequest_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		string itemId = row.Cells[DCC.IndexOf("WORKREQUESTID")].Text.Trim();
		string request = row.Cells[DCC.IndexOf("TITLE")].Text.Trim();

		for (int i = 0; i < row.Cells.Count; i++)
		{
			row.Cells[i].Style["background-color"] = "#FFFFFF";
		}

		row.Cells[DCC["WORKREQUESTID"].Ordinal].Controls.Add(createEditLink(itemId, itemId, "Edit Request - [" + request + "]"));
		row.Cells[DCC["WORKREQUESTID"].Ordinal].ToolTip = request;

		row.Cells[DCC["ReleaseVersion"].Ordinal].Text = row.Cells[DCC["ReleaseVersion"].Ordinal].Text.Replace(",", "<br />");
		row.Cells[DCC["MSG"].Ordinal].Text = row.Cells[DCC["MSG"].Ordinal].Text.Replace(",", "<br />");

		TextBox txt = null;
		//add edit controls
		txt = WTSUtility.CreateGridTextBox("TITLE", itemId, request, false);
		row.Cells[DCC["TITLE"].Ordinal].Controls.Add(txt);
		txt = WTSUtility.CreateGridTextBox("DESCRIPTION", itemId, row.Cells[DCC["DESCRIPTION"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), false);
		//txt.TextMode = TextBoxMode.MultiLine;
		//txt.Rows = 5;
		row.Cells[DCC["DESCRIPTION"].Ordinal].Controls.Add(txt);
		
		txt = WTSUtility.CreateGridTextBox("Last_Meeting", itemId, row.Cells[DCC.IndexOf("Last_Meeting")].Text.Replace("&nbsp;", " ").Trim());
		txt.Attributes.Add("date", "true");
		txt.Style["width"] = "70px";
		txt.CssClass = "date";
		row.Cells[DCC.IndexOf("Last_Meeting")].Controls.Add(txt);

		txt = WTSUtility.CreateGridTextBox("Next_Meeting", itemId, row.Cells[DCC.IndexOf("Next_Meeting")].Text.Replace("&nbsp;", " ").Trim());
		txt.Attributes.Add("date", "true");
		txt.Style["width"] = "70px";
		txt.CssClass = "date";
		row.Cells[DCC.IndexOf("Next_Meeting")].Controls.Add(txt);

		txt = WTSUtility.CreateGridTextBox("Dev_Start", itemId, row.Cells[DCC.IndexOf("Dev_Start")].Text.Replace("&nbsp;", " ").Trim());
		txt.Attributes.Add("date", "true");
		txt.Style["width"] = "70px";
		txt.CssClass = "date";
		row.Cells[DCC.IndexOf("Dev_Start")].Controls.Add(txt);

		string changeEvent = string.Format("ddl_change(this);return false;");

		DropDownList ddl = null;

		//priority
		ddl = WTSUtility.CreateGridDropdownList(_dtPriority, "PRIORITY", "SORT_ORDER", "PRIORITYID", itemId, row.Cells[DCC.IndexOf("OP_PRIORITYID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("PRIORITY")].Text.Replace("&nbsp;", " ").Trim(), null);
		row.Cells[DCC.IndexOf("PRIORITY")].Controls.Add(ddl);
		//resources
		ddl = WTSUtility.CreateGridDropdownList(_dtUsers, "SME", "USERNAME", "WTS_RESOURCEID", itemId, row.Cells[DCC.IndexOf("SMEID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("SME")].Text.Replace("&nbsp;", " ").Trim(), new List<string> { {"OrganizationID"} });
		row.Cells[DCC.IndexOf("SME")].Controls.Add(ddl);
		ddl = WTSUtility.CreateGridDropdownList(_dtUsers, "Lead_Tech_Writer", "USERNAME", "WTS_RESOURCEID", itemId, row.Cells[DCC.IndexOf("LEAD_IA_TWID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("Lead_Tech_Writer")].Text.Replace("&nbsp;", " ").Trim(), new List<string> { { "OrganizationID" } });
		row.Cells[DCC.IndexOf("Lead_Tech_Writer")].Controls.Add(ddl);
		ddl = WTSUtility.CreateGridDropdownList(_dtUsers, "Lead_Resource", "USERNAME", "WTS_RESOURCEID", itemId, row.Cells[DCC.IndexOf("LEAD_RESOURCEID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("Lead_Resource")].Text.Replace("&nbsp;", " ").Trim(), new List<string> { { "OrganizationID" } });
		row.Cells[DCC.IndexOf("Lead_Resource")].Controls.Add(ddl);
			
		//statuses
		ddl = WTSUtility.CreateGridDropdownList(_dtTDStatus, "TD_STATUS", "STATUS", "STATUSID", itemId, row.Cells[DCC.IndexOf("TD_STATUSID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("TD_STATUS")].Text.Replace("&nbsp;", " ").Trim(), null);
		row.Cells[DCC.IndexOf("TD_STATUS")].Controls.Add(ddl);
		ddl = WTSUtility.CreateGridDropdownList(_dtCDStatus, "CD_STATUS", "STATUS", "STATUSID", itemId, row.Cells[DCC.IndexOf("CD_STATUSID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("CD_STATUS")].Text.Replace("&nbsp;", " ").Trim(), null);
		row.Cells[DCC.IndexOf("CD_STATUS")].Controls.Add(ddl);
		ddl = WTSUtility.CreateGridDropdownList(_dtCStatus, "C_STATUS", "STATUS", "STATUSID", itemId, row.Cells[DCC.IndexOf("C_STATUSID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("C_STATUS")].Text.Replace("&nbsp;", " ").Trim(), null);
		row.Cells[DCC.IndexOf("C_STATUS")].Controls.Add(ddl);
		ddl = WTSUtility.CreateGridDropdownList(_dtITStatus, "IT_STATUS", "STATUS", "STATUSID", itemId, row.Cells[DCC.IndexOf("IT_STATUSID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("IT_STATUS")].Text.Replace("&nbsp;", " ").Trim(), null);
		row.Cells[DCC.IndexOf("IT_STATUS")].Controls.Add(ddl);
		ddl = WTSUtility.CreateGridDropdownList(_dtCVTStatus, "CVT_STATUS", "STATUS", "STATUSID", itemId, row.Cells[DCC.IndexOf("CVT_STATUSID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("CVT_STATUS")].Text.Replace("&nbsp;", " ").Trim(), null);
		row.Cells[DCC.IndexOf("CVT_STATUS")].Controls.Add(ddl);
		ddl = WTSUtility.CreateGridDropdownList(_dtAStatus, "A_STATUS", "STATUS", "STATUSID", itemId, row.Cells[DCC.IndexOf("A_STATUSID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("A_STATUS")].Text.Replace("&nbsp;", " ").Trim(), null);
		row.Cells[DCC.IndexOf("A_STATUS")].Controls.Add(ddl);
		ddl = WTSUtility.CreateGridDropdownList(_dtCRStatus, "CR_STATUS", "STATUS", "STATUSID", itemId, row.Cells[DCC.IndexOf("CR_STATUSID")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("CR_STATUS")].Text.Replace("&nbsp;", " ").Trim(), null);
		row.Cells[DCC.IndexOf("CR_STATUS")].Controls.Add(ddl);
	}

	void gridRequest_PageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		gridRequest.PageIndex = e.NewPageIndex;
		//if (HttpContext.Current.Session["dtGroup_Requests"] == null)
		//{
			loadGridData();
		//}
		//else
		//{
		//	gridRequest.DataSource = (DataTable)HttpContext.Current.Session["dtGroup_Requests"];
		//	gridRequest.DataBind();
		//}
	}

	private void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC["WORKREQUESTID"].Ordinal
				&& i != DCC["ReleaseVersion"].Ordinal
				&& i != DCC["MSG"].Ordinal
				&& i != DCC["PRIORITY"].Ordinal
				&& i != DCC["TITLE"].Ordinal
				&& i != DCC["WorkItem_Count"].Ordinal
				&& i != DCC["DESCRIPTION"].Ordinal
				&& i != DCC["Last_Meeting"].Ordinal
				&& i != DCC["Next_Meeting"].Ordinal
				&& i != DCC["Dev_Start"].Ordinal
				&& i != DCC["CIA_Risk"].Ordinal
				&& i != DCC["CMMI"].Ordinal
				&& i != DCC["SME"].Ordinal
				&& i != DCC["Lead_Tech_Writer"].Ordinal
				&& i != DCC["Lead_Resource"].Ordinal
				&& i != DCC["TD_STATUS"].Ordinal
				&& i != DCC["CD_STATUS"].Ordinal
				&& i != DCC["C_STATUS"].Ordinal
				&& i != DCC["IT_STATUS"].Ordinal
				&& i != DCC["CVT_STATUS"].Ordinal
				&& i != DCC["A_STATUS"].Ordinal
				&& i != DCC["CR_STATUS"].Ordinal
				&& i != DCC["Y"].Ordinal)
			{
				row.Cells[i].Style["display"] = "none";
			}
			else
			{
				row.Cells[i].Style["vertical-align"] = "top";
			}
		}

		row.Cells[DCC["WORKREQUESTID"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["WORKREQUESTID"].Ordinal].Style["width"] = "25px";

		row.Cells[DCC["ReleaseVersion"].Ordinal].Style["text-align"] = "left";
		row.Cells[DCC["MSG"].Ordinal].Style["text-align"] = "left";
		row.Cells[DCC["DESCRIPTION"].Ordinal].Style["padding-left"] = "5px";
		row.Cells[DCC["PRIORITY"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["TITLE"].Ordinal].Style["text-align"] = "left";
		row.Cells[DCC["TITLE"].Ordinal].Style["padding-left"] = "5px";
		row.Cells[DCC["WorkItem_Count"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["DESCRIPTION"].Ordinal].Style["text-align"] = "left";
		row.Cells[DCC["DESCRIPTION"].Ordinal].Style["padding-left"] = "5px";
		row.Cells[DCC["Last_Meeting"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["Next_Meeting"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["Dev_Start"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["CIA_Risk"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["CMMI"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["SME"].Ordinal].Style["text-align"] = "left";
		row.Cells[DCC["SME"].Ordinal].Style["padding-left"] = "5px";
		row.Cells[DCC["Lead_Tech_Writer"].Ordinal].Style["text-align"] = "left";
		row.Cells[DCC["Lead_Tech_Writer"].Ordinal].Style["padding-left"] = "5px";
		row.Cells[DCC["Lead_Resource"].Ordinal].Style["text-align"] = "left";
		row.Cells[DCC["Lead_Resource"].Ordinal].Style["padding-left"] = "5px";

		row.Cells[DCC["TD_STATUS"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["CD_STATUS"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["C_STATUS"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["IT_STATUS"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["CVT_STATUS"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["A_STATUS"].Ordinal].Style["text-align"] = "center";
		row.Cells[DCC["CR_STATUS"].Ordinal].Style["text-align"] = "center";

		row.Cells[DCC["Y"].Ordinal].Style["text-align"] = "center";

		//set widths

		row.Cells[DCC["WORKREQUESTID"].Ordinal].Style["width"] = "33px";
		row.Cells[DCC["ReleaseVersion"].Ordinal].Style["width"] = "50px";
		row.Cells[DCC["MSG"].Ordinal].Style["width"] = "50px";
		row.Cells[DCC["PRIORITY"].Ordinal].Style["width"] = "50px";
		row.Cells[DCC["TITLE"].Ordinal].Style["width"] = "150px";
		row.Cells[DCC["WorkItem_Count"].Ordinal].Style["width"] = "50px";
		row.Cells[DCC["Last_Meeting"].Ordinal].Style["width"] = "55px";
		row.Cells[DCC["Next_Meeting"].Ordinal].Style["width"] = "55px";
		row.Cells[DCC["Dev_Start"].Ordinal].Style["width"] = "55px";
		row.Cells[DCC["CIA_Risk"].Ordinal].Style["width"] = "40px";
		row.Cells[DCC["CMMI"].Ordinal].Style["width"] = "40px";

		row.Cells[DCC["SME"].Ordinal].Style["width"] = "65px";
		row.Cells[DCC["Lead_Tech_Writer"].Ordinal].Style["width"] = "65px";
		row.Cells[DCC["Lead_Resource"].Ordinal].Style["width"] = "65px";

		row.Cells[DCC["TD_STATUS"].Ordinal].Style["width"] = "50px";
		row.Cells[DCC["CD_STATUS"].Ordinal].Style["width"] = "55px";
		row.Cells[DCC["C_STATUS"].Ordinal].Style["width"] = "50px";
		row.Cells[DCC["IT_STATUS"].Ordinal].Style["width"] = "50px";
		row.Cells[DCC["CVT_STATUS"].Ordinal].Style["width"] = "50px";
		row.Cells[DCC["A_STATUS"].Ordinal].Style["width"] = "55px";
		row.Cells[DCC["CR_STATUS"].Ordinal].Style["width"] = "55px";

		row.Cells[DCC["Y"].Ordinal].Style["width"] = "14px";
		row.Cells[DCC["Y"].Ordinal].Style["border-right"] = "0px";
	}

	Image createAddButton()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("imgAddRequest_click();return false;");

		Image imgAdd = new Image();
		imgAdd.Style["cursor"] = "pointer";
		imgAdd.Height = 10;
		imgAdd.Width = 10;
		imgAdd.ImageUrl = "Images/Icons/add_blue.png";
		imgAdd.ID = "imgAddRequest";
		imgAdd.AlternateText = "Add Request";
		imgAdd.Attributes.Add("Onclick", sb.ToString());

		return imgAdd;
	}

	LinkButton createEditLink(string requestId = "", string request = "", string tooltip = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEditRequest_click('{0}','{1}');return false;", requestId, requestId);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEditRequest_{0}", requestId);
		lb.Attributes["name"] = string.Format("lbEditRequest_{0}", requestId);
		lb.ToolTip = string.IsNullOrWhiteSpace(tooltip) ? string.Format("Edit Request [{0}]", request) : tooltip;
		lb.Text = request;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}

	Image createDeleteButton(string requestId = "", string request = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("imgDeleteRequest_click('{0}','{1}');return false;", requestId, request);

		Image imgDelete = new Image();
		imgDelete.Style["cursor"] = "pointer";
		imgDelete.Height = 10;
		imgDelete.Width = 10;
		imgDelete.ImageUrl = "Images/Icons/delete.png";
		imgDelete.ID = string.Format("imgDeleteRequest_{0}", requestId);
		imgDelete.Attributes["name"] = string.Format("imgDeleteRequest_{0}", requestId);
		imgDelete.Attributes.Add("requestId", requestId.ToString());
		imgDelete.ToolTip = string.Format("Delete Request [{0}]", request);
		imgDelete.AlternateText = "Delete Request";
		imgDelete.Attributes.Add("Onclick", sb.ToString());

		return imgDelete;
	}

	Table createHeaderTable(string field, string text)
	{
		Table tableHeader = new Table();
		tableHeader.Style["width"] = "100%";
		tableHeader.CellPadding = 0;
		tableHeader.CellSpacing = 0;
		tableHeader.BorderStyle = BorderStyle.None;
		TableRow row = new TableRow();
		TableCell textCell = new TableCell();
		textCell.Style["border"] = "none";
		TableCell imgCell = new TableCell();
		imgCell.Style["border"] = "none";

		tableHeader.Rows.Add(row);
		row.Cells.Add(textCell);
		row.Cells.Add(imgCell);

		textCell.Text = text;
		imgCell.Controls.Add(createStatusHelpIcon(field));

		return tableHeader;
	}

	Image createStatusHelpIcon(string field)
	{
		DataTable dt = null;

		switch (field.ToUpper())
		{
			case "TD_STATUS":
				dt = _dtTDStatus;
				break;
			case "CD_STATUS":
				dt = _dtCDStatus;
				break;
			case "C_STATUS":
				dt = _dtCStatus;
				break;
			case "IT_STATUS":
				dt = _dtITStatus;
				break;
			case "CVT_STATUS":
				dt = _dtCVTStatus;
				break;
			case "A_STATUS":
				dt = _dtAStatus;
				break;
			case "CR_STATUS":
				dt = _dtCRStatus;
				break;
		}

		Image imgHelp = new Image();
		imgHelp.Style["cursor"] = "pointer";
		imgHelp.Style["border"] = "none";
		imgHelp.Height = 10;
		imgHelp.Width = 10;
		imgHelp.ImageUrl = "Images/Icons/help.png";
		imgHelp.ID = "imgAddRequest";
		imgHelp.AlternateText = "Status Help";
		imgHelp.ToolTip = "Click for Status descriptions.";
		imgHelp.Attributes.Add("Onclick", "showStatusHelp(this, '" + field + "');");

		return imgHelp;
	}

	#endregion Grid


	[WebMethod(true)]
	public static string SaveChanges(string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
		bool saved = false;
		string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

		try
		{
			DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjson.Rows.Count == 0)
			{
				errorMsg = "Unable to save. An invalid list of changes was provided.";
				saved = false;
			}

			int id = 0;
			string title = string.Empty, description = string.Empty
				, lastMeeting = string.Empty, nextMeeting = string.Empty, devStart = string.Empty;
			int priorityID = 0, smeID = 0, leadIATWID = 0, leadResourceID = 0, submittedByID = 0;
			int td = 0, cd = 0, c = 0, it = 0, cvt = 0, adopt = 0, cr = 0, archive = 0;
			int hasSlides = 0, workStoppage = 0;

			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
				title = string.Empty;
				description = string.Empty;
				archive = 0;
				lastMeeting = string.Empty;
				nextMeeting = string.Empty;
				devStart = string.Empty;
				smeID = 0;
				leadIATWID = 0;
				leadResourceID = 0;
				submittedByID = 0;

				tempMsg = string.Empty;
				title = dr["TITLE"].ToString();
				if (string.IsNullOrWhiteSpace(title))
				{
					tempMsg = "You must specify a value for MS.";
					saved = false;
				}
				else
				{
					int.TryParse(dr["WORKREQUESTID"].ToString(), out id);
					description = dr["DESCRIPTION"].ToString();
					lastMeeting = dr["Last_Meeting"].ToString();
					nextMeeting = dr["Next_Meeting"].ToString();
					devStart = dr["Dev_Start"].ToString();
					int.TryParse(dr["PRIORITY"].ToString(), out priorityID);
					//resources
					int.TryParse(dr["SME"].ToString(), out smeID);
					int.TryParse(dr["Lead_Tech_Writer"].ToString(), out leadIATWID);
					int.TryParse(dr["Lead_Resource"].ToString(), out leadResourceID);
					//int.TryParse(dr["SUBMITTEDBY"].ToString(), out submittedByID);
					//statuses
					int.TryParse(dr["TD_STATUS"].ToString(), out td);
					int.TryParse(dr["CD_STATUS"].ToString(), out cd);
					int.TryParse(dr["C_STATUS"].ToString(), out c);
					int.TryParse(dr["IT_STATUS"].ToString(), out it);
					int.TryParse(dr["CVT_STATUS"].ToString(), out cvt);
					int.TryParse(dr["A_STATUS"].ToString(), out adopt);
					int.TryParse(dr["CR_STATUS"].ToString(), out cr);

					int.TryParse(dr["ARCHIVE"].ToString(), out archive);

					if (id == 0)
					{
						saved = false;
						tempMsg = "No Work Request was specified to update.";
					}
					else
					{
						WorkRequest wr = WorkRequest.WorkRequest_GetObject(id);
						wr.OP_PRIORITYID = priorityID;
						wr.Title = title;
						wr.Description = description;
						wr.Last_Meeting = lastMeeting;
						wr.Next_Meeting = nextMeeting;
						wr.Dev_Start = devStart;
						wr.CIA_Risk = string.Empty;
						wr.CMMI = string.Empty;
						wr.SMEID = smeID;
						wr.LEAD_IA_TWID = leadIATWID;
						wr.LEAD_RESOURCEID = leadResourceID;
						//wr.SubmittedByID = submittedByID;
						wr.TD_StatusID = td;
						wr.CD_StatusID = cd;
						wr.C_StatusID = c;
						wr.IT_StatusID = it;
						wr.CVT_StatusID = cvt;
						wr.A_StatusID = adopt;
						wr.CR_StatusID = cr;
						wr.HasSlides = hasSlides;
						wr.WorkStoppage = workStoppage;
						wr.Archive = (archive == 1);

						saved = WorkRequest.WorkRequest_QM_Update(wr, out tempMsg);
					}
				}

				if (saved)
				{
					ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
				}

				if (tempMsg.Length > 0)
				{
					errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
				}
			}
		}
		catch (Exception ex)
		{
			saved = false;
			errorMsg = ex.Message;
			LogUtility.LogException(ex);
		}

		result["ids"] = ids;
		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

}