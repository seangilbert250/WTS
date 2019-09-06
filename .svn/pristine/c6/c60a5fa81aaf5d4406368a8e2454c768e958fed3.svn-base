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


public partial class Admin_UserProfile_Hardware : System.Web.UI.Page
{
	#region Properties

	protected DataTable DtHT = null;
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
		grdHardware.RowDataBound += grdHardware_RowDataBound;

		WTS_User u = null;
		LoadData();
	}

	protected void LoadData()
	{
		DataTable dt = null;
		if (_refreshData || Session["dtUserHardware"] == null)
		{
			WTS_User u = new WTS_User(this.UserId);
			dt = u.HardwareList_Get();
			HttpContext.Current.Session["dtUserHardware"] = dt;
		}
		else
		{
			dt = (DataTable)HttpContext.Current.Session["dtUserHardware"];
		}

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			DtHT = dt.Copy();
			DtHT = DtHT.DefaultView.ToTable(true, new string[] { "HardwareTypeID", "HardwareType" });

			InitializeColumnData(ref dt);
			dt.AcceptChanges();

			int count = dt.Rows.Count;
			count = count > 0 ? count - 1 : count; //need to subtract the empty row
			spanRowCount.InnerText = count.ToString();
		}

		grdHardware.DataSource = dt;
		grdHardware.DataBind();
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
					case "HardwareType":
						displayName = "Device Type";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DeviceSN_Tag":
						displayName = "Serial Number/Tag";
						blnVisible = true;
						blnSortable = true;
						break;
					case "DeviceName":
						displayName = "Device Name";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Description":
						displayName = "Description";
						blnVisible = true;
						blnSortable = true;
						break;
					case "HasDevice":
						displayName = "Has Device";
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

	#region Grid
	
	void grdHardware_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.Header)
		{
			grdHardware_GridHeaderRowDataBound(sender, e);
		}
		else if (e.Row.RowType == DataControlRowType.DataRow)
		{
			grdHardware_GridRowDataBound(sender, e);
		}
	}

	void grdHardware_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridHeader(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);
	}

	void grdHardware_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		string itemId = row.Cells[DCC.IndexOf("WTS_Resource_HardwareID")].Text.Trim();
		row.Attributes.Add("itemID", itemId);

		if (CanEdit)
		{
			//add edit controls
			row.Cells[DCC.IndexOf("HardwareType")].Controls.Add(createDDL(DtHT, "HardwareType", "HardwareTypeID", itemId, row.Cells[DCC.IndexOf("HardwareTypeID")].Text.Trim(), row.Cells[DCC.IndexOf("HardwareType")].Text.Trim()));
						
			row.Cells[DCC.IndexOf("DeviceName")].Controls.Add(WTSUtility.CreateGridTextBox("DeviceName", itemId, row.Cells[DCC.IndexOf("DeviceName")].Text.Replace("&nbsp;", " ").Trim()));
			row.Cells[DCC.IndexOf("DeviceSN_Tag")].Controls.Add(WTSUtility.CreateGridTextBox("DeviceSN_Tag", itemId, row.Cells[DCC.IndexOf("DeviceSN_Tag")].Text.Replace("&nbsp;", " ").Trim()));
			row.Cells[DCC.IndexOf("Description")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, row.Cells[DCC.IndexOf("Description")].Text.Replace("&nbsp;", " ").Trim()));

			bool hasDevice = false;
			if (row.Cells[DCC.IndexOf("HasDevice")].HasControls()
				&& row.Cells[DCC.IndexOf("HasDevice")].Controls[0] is CheckBox)
			{
				hasDevice = ((CheckBox)row.Cells[DCC.IndexOf("HasDevice")].Controls[0]).Checked;
			}
			else if (row.Cells[DCC.IndexOf("HasDevice")].Text == "1")
			{
				hasDevice = true;
			}
			row.Cells[DCC.IndexOf("HasDevice")].Controls.Clear();
			row.Cells[DCC.IndexOf("HasDevice")].Controls.Add(WTSUtility.CreateGridCheckBox("HasDevice", itemId, hasDevice));

			row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("DeviceName")].Text.Trim(), 1));
		}
		else
		{
			Image imgBlank = new Image();
			imgBlank.Height = 10;
			imgBlank.Width = 10;
			imgBlank.ImageUrl = "../Images/Icons/blank.png";
			imgBlank.AlternateText = "";
			row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
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
				&& i != DCC.IndexOf("HasDevice"))
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
		row.Cells[DCC.IndexOf("X")].Style["width"] = "14px";
		row.Cells[DCC.IndexOf("HardwareType")].Style["width"] = "100px";
		row.Cells[DCC.IndexOf("DeviceName")].Style["width"] = "125px";
		row.Cells[DCC.IndexOf("DeviceSN_Tag")].Style["width"] = "125px";
		row.Cells[DCC.IndexOf("HasDevice")].Style["width"] = "55px";
	}


	DropDownList createDDL(DataTable dt, string textField, string valueField, string itemId, string value, string text = "")
	{
		DropDownList ddl = new DropDownList();
		ddl.ID = string.Format("ddl{0}_{1}", textField.Trim().Replace(" ", ""), itemId);
		ddl.Attributes["name"] = ddl.ID;
		ddl.Attributes.Add("itemId", itemId);
		ddl.Attributes.Add("original_value", value.Replace("&nbsp;", ""));
		ddl.Style["width"] = "99%";
		ddl.Style["background-color"] = "#F5F6CE";

		ddl.DataSource = dt;
		ddl.DataTextField = textField;
		ddl.DataValueField = valueField;
		ddl.DataBind();

		WTSUtility.SelectDdlItem(ddl, value, text);

		return ddl;
	}

	#endregion Grid


	[WebMethod(true)]
	public static string SaveChanges(int userID, string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
		bool saved = false, duplicate = false;
		string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

		try
		{
			DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjson.Rows.Count == 0)
			{
				errorMsg = "Unable to save. An invalid list of changes was provided.";
				saved = false;
			}

			WTS_User u = new WTS_User(userID);
			u.Load();

			int id = 0, typeId = 0, hasDevice = 0;
			string deviceName = string.Empty, deviceSN_Tag = string.Empty, description = string.Empty;

			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
				typeId = 0;
				deviceName = string.Empty;
				deviceSN_Tag = string.Empty;
				description = string.Empty;
				hasDevice = 0;

				tempMsg = string.Empty;
				int.TryParse(dr["WTS_Resource_HardwareID"].ToString(), out id);
				int.TryParse(dr["HardwareType"].ToString(), out typeId);
				deviceName = dr["DeviceName"].ToString();
				deviceSN_Tag = dr["DeviceSN_Tag"].ToString();
				description = dr["Description"].ToString();
				int.TryParse(dr["HasDevice"].ToString(), out hasDevice);

				if (string.IsNullOrWhiteSpace(deviceName) 
					&& string.IsNullOrEmpty(deviceSN_Tag))
				{
					tempMsg = "You must specify a value for Device Name or Device Serial Number / Tag.";
					saved = false;
				}
				else
				{
					if (id == 0)
					{
						saved = u.Hardware_Add(typeId, deviceName, deviceSN_Tag, description, (hasDevice == 1), out id, out tempMsg);
					}
					else
					{
						saved = u.Hardware_Update(id, typeId, deviceName, deviceSN_Tag, description, (hasDevice == 1), out duplicate, out errorMsg);
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
	public static string DeleteItem(int userID, int itemId, string item)
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
				WTS_User u = new WTS_User(userID);
				u.Load();

				deleted = u.Hardware_Delete(itemId, out exists);
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