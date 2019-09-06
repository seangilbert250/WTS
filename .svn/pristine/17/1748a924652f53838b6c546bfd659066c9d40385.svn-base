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


public partial class Admin_UserProfile_Options : System.Web.UI.Page
{
	#region Properties

	protected bool IsUserAdmin = false;
	protected bool IsCurrentUser = false;

	protected int UserId = 0;
	protected string MembershipUserId = string.Empty;

	protected string LoggedInMembership_UserId { get; set; }
	protected int LoggedInProfileId { get; set; }

	protected WTS_User _loggedInUser;
	protected MembershipUser _loggedInMembershipUser;

	protected bool ViewOnly = false;

	#endregion Properties


	protected void Page_PreInit(object sender, EventArgs e)
	{
		//load theme for user
		Page.Theme = WTSUtility.ThemeName;
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		#region Logged In User details

		_loggedInMembershipUser = Membership.GetUser();
		this.LoggedInMembership_UserId = _loggedInMembershipUser.ProviderUserKey.ToString();

		_loggedInUser = new WTS_User();
		_loggedInUser.Load(this.LoggedInMembership_UserId);
		this.LoggedInProfileId = UserManagement.GetUserId(this.LoggedInMembership_UserId);
		IsUserAdmin = UserManagement.UserIsUserAdmin(this.LoggedInProfileId);

		#endregion Logged In User details

		if (Request.QueryString["UserID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["UserID"]))
		{
			int.TryParse(this.Server.UrlDecode(this.Request.QueryString["UserID"]), out this.UserId);
		}

		if (this.LoggedInProfileId == this.UserId)
		{
			this.IsCurrentUser = true;
		}

		init();

		loadUserData();

		ddlMainView.Attributes.Add("original_value", ddlMainView.SelectedValue);
		ddlView_Main.Attributes.Add("original_value", ddlView_Main.SelectedValue);
		ddlView_Workload.Attributes.Add("original_value", ddlView_Workload.SelectedValue);
		ddlView_Crosswalk.Attributes.Add("original_value", ddlView_Crosswalk.SelectedValue);
		ddlView_Request.Attributes.Add("original_value", ddlView_Request.SelectedValue);
		ddlView_Hotlist.Attributes.Add("original_value", ddlView_Hotlist.SelectedValue);
	}

	private void init()
	{
		int settingType = (int)UserSettingType.GridView;

		ddlMainView.Attributes.Add("UserSettingTypeID", settingType.ToString());
		ddlView_Main.Attributes.Add("UserSettingTypeID", settingType.ToString());
		ddlView_Workload.Attributes.Add("UserSettingTypeID", settingType.ToString());
		ddlView_Crosswalk.Attributes.Add("UserSettingTypeID", settingType.ToString());
		ddlView_Request.Attributes.Add("UserSettingTypeID", settingType.ToString());
		ddlView_Hotlist.Attributes.Add("UserSettingTypeID", settingType.ToString());
		ddlView_AOR.Attributes.Add("UserSettingTypeID", settingType.ToString());

		ddlMainView.Attributes.Add("GridNameID", "0");
		ddlView_Main.Attributes.Add("GridNameID", "0");
		ddlView_Workload.Attributes.Add("GridNameID", "0");
		ddlView_Crosswalk.Attributes.Add("GridNameID", "0");
		ddlView_Request.Attributes.Add("GridNameID", "0");
		ddlView_Hotlist.Attributes.Add("GridNameID", "0");
		ddlView_AOR.Attributes.Add("GridNameID", "0");

		loadLookupData();
	}

	private void loadLookupData()
	{
		DataTable dt = WTSData.GetViewOptions(userId: this.UserId, gridName: "");
		
		if (dt == null || dt.Rows.Count == 0)
		{
			return;
		}

		ListItem item = null;
		//default view options
		DataRow[] rows = dt.Select("GridName='MainView'");
		if (rows != null && rows.Length > 0)
		{
			ddlMainView.Items.Clear();
			ddlMainView.Attributes["GridNameID"] = rows[0]["GridNameID"].ToString();
			foreach (DataRow row in rows)
			{
				item = new ListItem();
				item.Text = row["ViewName"].ToString();
				item.Value = row["GridViewID"].ToString();
				if (ddlMainView.Items.FindByText(row["ViewName"].ToString()) == null)
				{
					ddlMainView.Items.Add(item);
				}
			}
		}

		rows = dt.Select("GridName='Default'");
		if (rows != null && rows.Length > 0)
		{
			ddlView_Main.Items.Clear();
			ddlView_Main.Attributes["GridNameID"] = rows[0]["GridNameID"].ToString();
			foreach (DataRow row in rows)
			{
				item = new ListItem();
				item.Text = row["ViewName"].ToString();
				item.Value = row["GridViewID"].ToString();
				if (ddlView_Main.Items.FindByText(row["ViewName"].ToString()) == null)
				{
					ddlView_Main.Items.Add(item);
				}
			}
		}

		rows = dt.Select("GridName='Workload'");
		if (rows != null && rows.Length > 0)
		{
			ddlView_Workload.Items.Clear();
			ddlView_Workload.Attributes["GridNameID"] = rows[0]["GridNameID"].ToString();
			foreach (DataRow row in rows)
			{
				item = new ListItem();
				item.Text = row["ViewName"].ToString();
				item.Value = row["GridViewID"].ToString();
				if (ddlView_Workload.Items.FindByText(row["ViewName"].ToString()) == null)
				{
					ddlView_Workload.Items.Add(item);
				}
			}
		}

		rows = dt.Select("GridName='Workload Crosswalk'");
		if (rows != null && rows.Length > 0)
		{
			ddlView_Crosswalk.Items.Clear();
			ddlView_Crosswalk.Attributes["GridNameID"] = rows[0]["GridNameID"].ToString();
			foreach (DataRow row in rows)
			{
                if (row["ViewName"].ToString() != "-- New Gridview --")
                {
                    item = new ListItem();
                    item.Text = row["ViewName"].ToString();
                    item.Value = row["GridViewID"].ToString();
                    if (ddlView_Crosswalk.Items.FindByText(row["ViewName"].ToString()) == null)
                    {
                        ddlView_Crosswalk.Items.Add(item);
                    }
                }
			}
		}

		rows = dt.Select("GridName='Work Request'");
		if (rows != null && rows.Length > 0)
		{
			ddlView_Request.Items.Clear();
			ddlView_Request.Attributes["GridNameID"] = rows[0]["GridNameID"].ToString();
			foreach (DataRow row in rows)
			{
				item = new ListItem();
				item.Text = row["ViewName"].ToString();
				item.Value = row["GridViewID"].ToString();
				if (ddlView_Request.Items.FindByText(row["ViewName"].ToString()) == null)
				{
					ddlView_Request.Items.Add(item);
				}
			}
		}

		rows = dt.Select("GridName='Hotlist'");
		if (rows != null && rows.Length > 0)
		{
			ddlView_Hotlist.Items.Clear();
			ddlView_Hotlist.Attributes["GridNameID"] = rows[0]["GridNameID"].ToString();
			foreach (DataRow row in rows)
			{
				item = new ListItem();
				item.Text = row["ViewName"].ToString();
				item.Value = row["GridViewID"].ToString();
				if (ddlView_Hotlist.Items.FindByText(row["ViewName"].ToString()) == null)
				{
					ddlView_Hotlist.Items.Add(item);
				}
			}
		}

		rows = dt.Select("GridName='AOR'");
		if (rows != null && rows.Length > 0)
		{
			ddlView_AOR.Items.Clear();
			ddlView_AOR.Attributes["GridNameID"] = rows[0]["GridNameID"].ToString();
			foreach (DataRow row in rows)
			{
                if((int)row["viewType"] == 1 && row["ViewName"].ToString() != "-- New Gridview --")
                {
				    item = new ListItem();
				    item.Text = row["ViewName"].ToString();
				    item.Value = row["GridViewID"].ToString();
				    if (ddlView_AOR.Items.FindByText(row["ViewName"].ToString()) == null)
					    ddlView_AOR.Items.Add(item);
                }
			}
		}
	}

	private void loadUserData()
	{
		DataTable dt = _loggedInUser.UserSettingList_Get(this.UserId, (int)UserSettingType.GridView, 0);

		if (dt == null || dt.Rows.Count == 0)
		{
			return;
		}

		//ListItem item = null;

		//default view options
		DataRow[] rows;

		rows = dt.Select(string.Format("GridNameID='{0}'", ddlMainView.Attributes["GridNameID"].ToString()));
		if (rows.Length > 0)
		{
			ddlMainView.Attributes.Add("UserSettingID", rows[0]["UserSettingID"].ToString());
			WTSUtility.SelectDdlItem(ddlMainView, rows[0]["SettingValue"].ToString().Trim());
		}
		else
		{
			ddlMainView.Attributes.Add("UserSettingID", "0");
		}

		rows = dt.Select(string.Format("GridNameID='{0}'", ddlView_Main.Attributes["GridNameID"].ToString()));
		if (rows.Length > 0)
		{
			ddlView_Main.Attributes.Add("UserSettingID", rows[0]["UserSettingID"].ToString());
			WTSUtility.SelectDdlItem(ddlView_Main, rows[0]["SettingValue"].ToString().Trim());
		}
		else
		{
			ddlView_Main.Attributes.Add("UserSettingID", "0");
		}

		//Workload view
		rows = dt.Select(string.Format("GridNameID='{0}'", ddlView_Workload.Attributes["GridNameID"].ToString()));
		if (rows.Length > 0)
		{
			ddlView_Workload.Attributes.Add("UserSettingID", rows[0]["UserSettingID"].ToString());
			WTSUtility.SelectDdlItem(ddlView_Workload, rows[0]["SettingValue"].ToString().Trim());
		}
		else
		{
			ddlView_Workload.Attributes.Add("UserSettingID", "0");
		}

		//Crosswalk view
		rows = dt.Select(string.Format("GridNameID='{0}'", ddlView_Crosswalk.Attributes["GridNameID"].ToString()));
		if (rows.Length > 0)
		{
			ddlView_Crosswalk.Attributes.Add("UserSettingID", rows[0]["UserSettingID"].ToString());
			WTSUtility.SelectDdlItem(ddlView_Crosswalk, rows[0]["SettingValue"].ToString().Trim());
		}
		else
		{
			ddlView_Crosswalk.Attributes.Add("UserSettingID", "0");
		}



		//AOR view
		rows = dt.Select(string.Format("GridNameID='{0}'", ddlView_AOR.Attributes["GridNameID"].ToString()));
		if (rows.Length > 0)
		{
			ddlView_AOR.Attributes.Add("UserSettingID", rows[0]["UserSettingID"].ToString());
			WTSUtility.SelectDdlItem(ddlView_AOR, rows[0]["SettingValue"].ToString().Trim());
		}
		else
		{
			ddlView_AOR.Attributes.Add("UserSettingID", "0");
		}


		//Work Request view
		rows = dt.Select(string.Format("GridNameID='{0}'", ddlView_Request.Attributes["GridNameID"].ToString()));
		if (rows.Length > 0)
		{
			ddlView_Request.Attributes.Add("UserSettingID", rows[0]["UserSettingID"].ToString());
			WTSUtility.SelectDdlItem(ddlView_Request, rows[0]["SettingValue"].ToString().Trim());
		}
		else
		{
			ddlView_Request.Attributes.Add("UserSettingID", "0");
		}

		//Hotlist view
		rows = dt.Select(string.Format("GridNameID='{0}'", ddlView_Hotlist.Attributes["GridNameID"].ToString()));
		if (rows.Length > 0)
		{
			ddlView_Hotlist.Attributes.Add("UserSettingID", rows[0]["UserSettingID"].ToString());
			WTSUtility.SelectDdlItem(ddlView_Hotlist, rows[0]["SettingValue"].ToString().Trim());
		}
		else
		{
			ddlView_Hotlist.Attributes.Add("UserSettingID", "0");
		}
	}


	[WebMethod(true)]
	public static string SaveUserOptions(int userId, string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }, { "failed", "0" }, { "savedIds", "" }, { "failedIds", "" }, { "error", "" } };
		bool exists = false, saved = false;
		string savedIds = string.Empty, failedIds = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;
		int savedQty = 0, failedQty = 0;

		try
		{
			WTS_User u = new WTS_User(userId);
			u.Load();

			DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjson.Rows.Count == 0)
			{
				errorMsg = "No changes have been made.";
				saved = false;
			}
			else if (u == null || u.ID == 0)
			{
				errorMsg = "";
				saved = false;
			}
			else
			{
				int id = 0, userSettingTypeID = 0, gridNameID = 0;
				string settingValue = string.Empty;

				foreach (DataRow dr in dtjson.Rows)
				{
					id = 0;
					userSettingTypeID = 0;
					gridNameID = 0;
					settingValue = string.Empty;
					exists = false;
					tempMsg = string.Empty;

					int.TryParse(dr["UserSettingID"].ToString(), out id);
					int.TryParse(dr["UserSettingTypeID"].ToString(), out userSettingTypeID);
					int.TryParse(dr["GridNameID"].ToString(), out gridNameID);
					settingValue = dr["SettingValue"].ToString().Trim();

					saved = u.UserSetting_Update(userSettingID: id
							, userSettingTypeID: userSettingTypeID
							, gridNameID: gridNameID
							, settingValue: settingValue
							, duplicate: out exists
							, errorMsg: out tempMsg);

					savedQty += 1;

					if (tempMsg.Length > 0)
					{
						errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
					}
				}
			}
		}
		catch (Exception ex)
		{
			saved = false;
			errorMsg = ex.Message;
			LogUtility.LogException(ex);
			failedQty += 1;

		}

		result["saved"] = savedQty.ToString();
		result["failed"] = failedQty.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
}