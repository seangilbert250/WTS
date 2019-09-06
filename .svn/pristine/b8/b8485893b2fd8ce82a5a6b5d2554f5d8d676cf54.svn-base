using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;


public partial class Admin_UserProfile_Certifications : System.Web.UI.Page
{
	#region Properties

	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

	protected bool _refreshData = false;
	protected bool _export = false;


	protected bool IsUserAdmin = false;

	protected bool IsPopup = false;
	protected bool IsAdmin = true;
	protected bool CanView = false;
	protected bool CanEdit = false;

	protected bool IsCurrentUser = false;

	protected int UserId = 0;
	protected string MembershipUserId = string.Empty;

	protected string LoggedInMembership_UserId { get; set; }
	protected int LoggedInProfileId { get; set; }

	protected WTS_User _loggedInUser;
	protected MembershipUser _loggedInMembershipUser;

	#endregion Properties


	protected void Page_PreInit(object sender, EventArgs e)
	{
		//load theme for user
		Page.Theme = WTSUtility.ThemeName;
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.ResourceAdmin);
		this.CanView = CanEdit || UserManagement.UserCanView(WTSModuleOption.ResourceAdmin);

		#region QueryString

		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}
		if (Request.QueryString["popup"] == null || string.IsNullOrWhiteSpace(Request.QueryString["popup"]))
		{
			this.IsPopup = false;
		}
		else
		{
			bool.TryParse(this.Server.UrlDecode(Request.QueryString["popup"]).ToString().ToLower(), out this.IsPopup);
		}

		if (Request.QueryString["CurrentUser"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CurrentUser"]))
		{
			bool.TryParse(this.Server.UrlDecode(Request.QueryString["CurrentUser"]).ToString().ToLower(), out this.IsCurrentUser);
		}

		#endregion QueryString

		#region Logged In User details

		_loggedInMembershipUser = Membership.GetUser();
		this.LoggedInMembership_UserId = _loggedInMembershipUser.ProviderUserKey.ToString();

		_loggedInUser = new WTS_User();
		_loggedInUser.Load(this.LoggedInMembership_UserId);
		this.LoggedInProfileId = UserManagement.GetUserId(this.LoggedInMembership_UserId);
		IsUserAdmin = UserManagement.UserIsUserAdmin(this.LoggedInProfileId);

		#endregion Logged In User details

		if (this.IsCurrentUser)
		{
			this.UserId = _loggedInUser.ID;
			this.MembershipUserId = this.LoggedInMembership_UserId;
		}
		else
		{
			if (Request.QueryString["UserID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["UserID"]))
			{
				int.TryParse(this.Server.UrlDecode(this.Request.QueryString["UserID"]), out this.UserId);
			}
		}

		InitControls();
    }


	private void InitControls()
	{
		grdCerts.RowDataBound += grdCerts_RowDataBound;

		WTS_User u = null;
		LoadData();
	}

	void grdCerts_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.Header)
		{
			grdCerts_GridHeaderRowDataBound(sender, e);
		}
		else if (e.Row.RowType == DataControlRowType.DataRow)
		{
			grdCerts_GridRowDataBound(sender, e);
		}
	}

	protected void LoadData()
	{
		DataTable dt = null;
		if (_refreshData || Session["dtUserCerts"] == null)
		{
			WTS_User u = new WTS_User(this.UserId);
			dt = u.CertificationList_Get();			
			HttpContext.Current.Session["dtUserCerts"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtUserCerts"];
		}

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
			int count = dt.Rows.Count;
			count = count > 0 ? count - 1 : count; //need to subtract the empty row
			spanRowCount.InnerText = count.ToString();

			InitializeColumnData(ref dt);
			dt.AcceptChanges();
		}

		grdCerts.DataSource = dt;
		grdCerts.DataBind();
	}

	protected void InitializeColumnData(ref DataTable dt)
	{
		try
		{
			string displayName = string.Empty, groupName = string.Empty;
			bool blnVisible = false, blnSortable = false, blnOrderable = false;

			foreach (DataColumn gridColumn in dt.Columns)
			{
				displayName = gridColumn.ColumnName;
				blnVisible = false;
				blnSortable = false;
				blnOrderable = false;
				groupName = string.Empty;

				switch (gridColumn.ColumnName)
				{
					case "X":
						displayName = "&nbsp;";
						blnVisible = true;
						break;
					case "Resource_Certification":
						displayName = "Certification";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DESCRIPTION":
						displayName = "Description";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Expiration_Date":
						displayName = "Expiration Date";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Expired":
						displayName = "Expired";
						blnVisible = true;
						blnSortable = true;
						break;
				}

				columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
			}

			//Initialize the columnData
			columnData.Initialize(ref dt, ";", "~", "|");

			//Get sortable columns and default column order
			SortableColumns = columnData.SortableColumnsToString();
			DefaultColumnOrder = columnData.DefaultColumnOrderToString();
			//Sort and Reorder Columns
			columnData.ReorderDataTable(ref dt, ColumnOrder);
			columnData.SortDataTable(ref dt, SortOrder);
			SelectedColumnOrder = columnData.CurrentColumnOrderToString();

		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}


	void grdCerts_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridHeader(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);
	}

	void grdCerts_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		string itemId = row.Cells[DCC.IndexOf("Resource_CertificationID")].Text.Trim();
		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			//add textboxes
			row.Cells[DCC.IndexOf("Resource_Certification")].Controls.Add(WTSUtility.CreateGridTextBox("Resource_Certification", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Resource_Certification")].Text.Trim())));
			row.Cells[DCC.IndexOf("Description")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, row.Cells[DCC.IndexOf("Description")].Text.Replace("&nbsp;"," ").Trim()));
			TextBox txt = WTSUtility.CreateGridTextBox("Expiration", itemId, row.Cells[DCC.IndexOf("Expiration_Date")].Text.Replace("&nbsp;","").Trim());
			txt.Attributes.Add("date", "true");
			txt.Style["width"] = "80px";
			txt.CssClass = "date";
			row.Cells[DCC.IndexOf("Expiration_Date")].Style["vertical-align"] = "bottom";
			row.Cells[DCC.IndexOf("Expiration_Date")].Controls.Add(txt);

			bool expired = false;
			if (row.Cells[DCC.IndexOf("Expired")].HasControls()
				&& row.Cells[DCC.IndexOf("Expired")].Controls[0] is CheckBox)
			{
				expired = ((CheckBox)row.Cells[DCC.IndexOf("Expired")].Controls[0]).Checked;
			}
			row.Cells[DCC.IndexOf("Expired")].Controls.Clear();
			row.Cells[DCC.IndexOf("Expired")].Controls.Add(WTSUtility.CreateGridCheckBox("Expired", itemId, expired));
		}

		if (!CanEdit)
		{
			Image imgBlank = new Image();
			imgBlank.Height = 10;
			imgBlank.Width = 10;
			imgBlank.ImageUrl = "../Images/Icons/blank.png";
			imgBlank.AlternateText = "";
			row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
		}
		else
		{
			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("Resource_Certification")].Text.Trim(), 1));
		}

		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("Expired"))
			{
				row.Cells[i].Style["text-align"] = "left";
				row.Cells[i].Style["padding-left"] = "5px";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "center";
				row.Cells[i].Style["padding-left"] = "0px";
			}
		}

		//more column formatting
		row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
		row.Cells[DCC.IndexOf("Resource_Certification")].Style["width"] = "200px";
		row.Cells[DCC.IndexOf("Expiration_Date")].Style["width"] = "90px";
		row.Cells[DCC.IndexOf("Expired")].Style["width"] = "55px";
	}
	

	void grdCerts_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdCerts.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtUserCerts"] == null)
		{
			LoadData();
		}
		else
		{
			grdCerts.DataSource = (DataTable)HttpContext.Current.Session["dtUserCerts"];
		}
	}


	[WebMethod(true)]
	public static string AddCertification(int userId, string certification, string description, string expirationDate, bool expired = false)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "id", "" }, { "error", "" } };
		bool saved = false;
		int id = 0;
		string errorMsg = string.Empty;

		try
		{
			WTS_User u = new WTS_User(userId);
			u.Load();

			saved = u.Certification_Add(certification, description, expirationDate, expired, out id, out errorMsg);
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
			errorMsg = ex.Message;
		}

		result["id"] = id.ToString();
		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	
	[WebMethod(true)]
	public static string SaveChanges(int userID, string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }
			, { "ids", "" }
			, { "error", "" } };
		bool saved = false, duplicate = false;
		string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

		try
		{
			DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjson.Rows.Count == 0)
			{
				errorMsg = "Unable to save. No list of changes was provided.";
				saved = false;
			}
			else
			{
				WTS_User u = new WTS_User(userID);
				u.Load();

				int id = 0, expired = 0;
				string certification = string.Empty, description = string.Empty, expirationDate = string.Empty;
			
				//save
				foreach (DataRow dr in dtjson.Rows)
				{
					id = 0;
					certification = string.Empty;
					description = string.Empty;
					expirationDate = string.Empty;
					expired = 0;
					duplicate = false;

					tempMsg = string.Empty;
					int.TryParse(dr["Resource_CertificationID"].ToString(), out id);
					certification = dr["Resource_Certification"].ToString();
					description = dr["Description"].ToString();
					expirationDate = dr["Expiration_Date"].ToString();
					int.TryParse(dr["Expired"].ToString(), out expired);

					if (string.IsNullOrWhiteSpace(certification))
					{
						tempMsg = "You must specify a value for Certification.";
						saved = false;
					}
					else
					{
						if (id == 0)
						{
							saved = u.Certification_Add(certification, description, expirationDate, (expired == 1), out id, out tempMsg);
						}
						else
						{
							saved = u.Certification_Update(id, certification, description, expirationDate, (expired == 1), out duplicate, out tempMsg);
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
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
			errorMsg = ex.Message;
		}

		result["ids"] = ids;
		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}


	[WebMethod(true)]
	public static string DeleteItem(int userId, int itemId, string item)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "id", itemId.ToString() }
			, { "item", item }
			, { "exists", "" }
			, { "deleted", "" }
			, { "error", "" } };
		bool exists = false, deleted = false;
		string errorMsg = string.Empty;

		try
		{
			//delete
			if (itemId == 0)
			{
				errorMsg = "You must specify an item to delete.";
			}
			else
			{
				WTS_User u = new WTS_User(userId);
				u.Load();

				deleted = u.Certification_Delete(itemId, out exists);
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			deleted = false;
			errorMsg = ex.Message;
		}

		result["exists"] = exists.ToString();
		result["deleted"] = deleted.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
}