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


public partial class Admin_UserProfile_Roles : System.Web.UI.Page
{
	#region Properties

	protected bool IsUserAdmin = false;
	protected bool ViewOnly = false;

	protected bool IsNew = true;
	public string Action
	{
		get
		{
			return this.IsNew ? "Add" : "Edit";
		}
	}

	protected bool IsPopup = false;
	protected bool IsAdmin = true;

	protected bool IsCurrentUser = false;

	protected int UserId = 0;
	protected string MembershipUserId = string.Empty;

	protected string LoggedInMembership_UserId { get; set; }
	protected int LoggedInProfileId { get; set; }

	protected WTS_User _loggedInUser;
	protected MembershipUser _loggedInMembershipUser;

	protected int Archive = 0;

	#endregion


	protected void Page_PreInit(object sender, EventArgs e)
	{
		//load theme for user
		Page.Theme = WTSUtility.ThemeName;
	}
    protected void Page_Load(object sender, EventArgs e)
    {
		#region QueryString

		if (Request.QueryString["popup"] == null || string.IsNullOrWhiteSpace(Request.QueryString["popup"]))
		{
			this.IsPopup = false;
		}
		else
		{
			bool.TryParse(this.Server.UrlDecode(Request.QueryString["popup"]).ToString().ToLower(), out this.IsPopup);
		}

		if (Request.QueryString["View"] == null || string.IsNullOrWhiteSpace(Request.QueryString["View"]))
		{
			ViewOnly = false;
		}
		else
		{
			bool.TryParse(this.Server.UrlDecode(Request.QueryString["View"]).ToString().ToLower(), out this.ViewOnly);
		}

		if (Request.QueryString["CurrentUser"] == null || string.IsNullOrWhiteSpace(Request.QueryString["CurrentUser"]))
		{
			ViewOnly = false;
		}
		else
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


		if (this.IsCurrentUser || this.UserId > 0)
		{
			IsNew = false;
		}

		InitControls();
    }


	private void InitControls()
	{
		LoadOrganizations();
		LoadAvailableRoles();

		WTS_User u = null;
		if (!IsNew)
		{
			LoadUserValues(out u);
		}
		else
		{
			chkApproved.Visible = false;
			chkApproved.Disabled = true;
			labelForChkApproved.Visible = false;
			chkLocked.Visible = false;
			chkLocked.Disabled = true;
			labelForChkLocked.Visible = false;
			buttonUnlock.Visible = false;
			buttonUnlock.Disabled = true;
			chkArchive.Visible = false;
			chkArchive.Disabled = true;
			labelForChkArchive.Visible = false;
			WTSUtility.SelectDdlItem(this.ddlOrganization, ((int)UserManagement.Organization.Unauthorized).ToString());
		}

		if (this.IsUserAdmin)
		{
			ddlOrganization.Enabled = true;
			if (!IsNew)
			{
				chkApproved.Disabled = false;
				if (u.IsLocked)
				{
					buttonUnlock.Visible = true;
					buttonUnlock.Disabled = false;
				}
				else
				{
					buttonUnlock.Visible = false;
					buttonUnlock.Disabled = true;
				}
			}
		}
		else
		{
			ddlOrganization.Enabled = false;
			chkApproved.Disabled = true;
			chkLocked.Disabled = true;
			buttonUnlock.Visible = false;
			chkArchive.Disabled = true;
		}

		if (ViewOnly)
		{
			DisableEdit();
		}
	}


	private void LoadUserValues(out WTS_User u)
	{
		u = new WTS_User();
		MembershipUser mu;

		if (this.IsCurrentUser)
		{
			u = _loggedInUser;
			mu = _loggedInMembershipUser;
			LoadUserRoles(mu);
		}
		else
		{
			u.Load(this.UserId);
			if (!string.IsNullOrWhiteSpace(u.Membership_UserID) && u.Membership_UserID != Guid.Empty.ToString())
			{
				mu = Membership.GetUser(new Guid(u.Membership_UserID));
				LoadUserRoles(mu);
			}
			else
			{
				mu = UserManagement.GetMembershipUser(u.Email, u.Username);
				if (mu != null)
				{
					u.Membership_UserID = mu.ProviderUserKey.ToString();
					LoadUserRoles(mu);
				}
			}
		}
		this.MembershipUserId = u.Membership_UserID;
		
		if (ddlOrganization.Items.FindByValue(u.OrganizationID.ToString()) == null)
		{
			ddlOrganization.Items.Add(new ListItem(u.Organization, u.OrganizationID.ToString()));
		}
		WTSUtility.SelectDdlItem(ddlOrganization, u.OrganizationID.ToString());
		chkApproved.Checked = u.IsApproved;
		chkLocked.Checked = u.IsLocked;
		chkArchive.Checked = u.Archive;
	}

	private void LoadUserRoles(MembershipUser user)
	{
		if (user != null)
		{
			string currentOrganization = _loggedInUser.Organization;
			string[] roles = Roles.GetRolesForUser(user.UserName);
			ListItem item = null;
			foreach (string r in roles)
			{
				if (r.ToLower() == "admin")
				{
					if (!currentOrganization.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)
						&& !Roles.IsUserInRole(_loggedInMembershipUser.UserName, "Admin"))
					{
						continue;
					}
				}
				if (r.ToLower() == "administration")
				{
					if (!currentOrganization.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)
						&& !Roles.IsUserInRole(_loggedInMembershipUser.UserName, "Admin")
						&& !Roles.IsUserInRole(_loggedInMembershipUser.UserName, "Administration")
						&& !Roles.IsUserInRole(_loggedInMembershipUser.UserName, "ResourceManagement"))
					{
						continue;
					}
				}
				if (r.ToLower() == "resourcemanagement")
				{
					if (!currentOrganization.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)
						&& !Roles.IsUserInRole(_loggedInMembershipUser.UserName, "Admin")
						&& !Roles.IsUserInRole(_loggedInMembershipUser.UserName, "Administration")
						&& !Roles.IsUserInRole(_loggedInMembershipUser.UserName, "ResourceManagement"))
					{
						continue;
					}
				}

				item = chkListRoles.Items.FindByText(r);
				if (item != null)
				{
					item.Selected = true;
				}
			}
		}
	}

	private void DisableEdit()
	{
		ddlOrganization.Enabled = false;
		
		chkApproved.Disabled = true;
		chkLocked.Disabled = true;
		buttonUnlock.Disabled = true;
		buttonUnlock.Visible = false;
		chkArchive.Disabled = true;
	}

	private void LoadOrganizations()
	{
		DataTable dt = UserManagement.LoadOrganizationList(includeArchive: false);

		ListItem item = new ListItem("-Select-", "-1");
		ddlOrganization.Items.Add(item);

		if (dt == null || dt.Rows.Count == 0) { return; }

		foreach (DataRow row in dt.Rows)
		{
			item = new ListItem(row["ORGANIZATION"].ToString(), row["ORGANIZATIONID"].ToString());
			ddlOrganization.Items.Add(item);
		}
	}

	private void LoadAvailableRoles()
	{
		ListItem item = null;

		try
		{
			string currentOrganization = _loggedInUser.Organization;
			string[] roles = Roles.GetAllRoles();
			foreach (string r in roles)
			{
				switch (r.ToLower())
				{
					case "admin":
					case "administration":
					case "resourcemanagement":
						if (!currentOrganization.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)
							&& !Roles.IsUserInRole(_loggedInMembershipUser.UserName, "Admin")
							&& !Roles.IsUserInRole(_loggedInMembershipUser.UserName, "Administration")
							&& !Roles.IsUserInRole(_loggedInMembershipUser.UserName, "ResourceManagement"))
						{
							continue;
						}
						break;
				}

				item = new ListItem(r, r, IsUserAdmin);
				this.chkListRoles.Items.Add(item);
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			throw;
		}
	}



	/// <summary>
	/// This will remove ALL membership roles and re-add only those that are selected
	/// </summary>
	/// <param name="username">registered username</param>
	/// <param name="rolesList">arrow of roles names</param>
	private static void UpdateUserRoles(string username, string[] rolesList)
	{
		string[] currentRoles = Roles.GetRolesForUser(username);
		if (currentRoles.Length > 0)
		{
			Roles.RemoveUserFromRoles(username, Roles.GetRolesForUser(username));
		}
		foreach (string r in rolesList)
		{
			Roles.AddUserToRole(username, r);
		}
	}


	[WebMethod(true)]
	public static string SaveProfile(int userId = 0, string membershipUserId = ""
		, int organizationId = 0, string roles = "", bool archive = false)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "id", "" }, { "error", "" }, { "username", "" }, { "changedTheme", "" } };
		string[] rolesList = roles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		bool saved = false;
		string errorMsg = string.Empty;

		try
		{
			//perform the save
			WTS_User u = new WTS_User();
			if (userId > 0)
			{
				u.ID = userId;
				u.Load();
			}

			u.OrganizationID = organizationId == 0 ? (int)UserManagement.Organization.Unauthorized : organizationId;
			u.Archive = archive;

			if (userId == 0)
			{
				//error message
				errorMsg = "No user to update was specified.";
			}
			else
			{
				saved = u.Update(out errorMsg);
				MembershipUser mu = null;
				mu = UserManagement.GetMembershipUser(u.Email, u.Username);

				UpdateUserRoles(mu.UserName, rolesList);
			}
		}
		catch (Exception ex)
		{
			saved = false;
			errorMsg = ex.Message.ToString();
			LogUtility.LogException(ex);
		}

		result["saved"] = saved.ToString();
		result["id"] = userId.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	/// <summary>
	/// Change approval status of selected user
	/// </summary>
	/// <param name="userId">Membership userid</param>
	/// <param name="approved">Approval status</param>
	/// <returns></returns>
	[WebMethod(true)]
	public static string ChangeUserApproval(string userId = "", bool approved = false)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "" }, { "approved", "" }, { "error", "" } };
		bool success = false;
		string errorMsg = string.Empty;

		Guid userGuid = Guid.Empty;
		if (!Guid.TryParse(userId.Trim(), out userGuid)
			|| userGuid == Guid.Empty)
		{
			success = false;
			errorMsg = "Registered user does not exist.";
		}
		else
		{
			MembershipUser mu = Membership.GetUser(userGuid);
			if (mu == null)
			{
				success = false;
				errorMsg = "Registered user does not exist.";
			}
			else
			{
				mu.IsApproved = approved;
				Membership.UpdateUser(mu);
				success = false;
				errorMsg = string.Empty;
			}
		}

		result["success"] = success.ToString();
		result["approved"] = approved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	/// <summary>
	/// Unlock membership user account
	/// </summary>
	/// <param name="userId">membership userid (GUID)</param>
	/// <returns></returns>
	[WebMethod(true)]
	public static string UnlockUser(string userId = "")
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "" }, { "error", "" } };
		bool success = false;
		string errorMsg = string.Empty;
		Guid userGuid = Guid.Empty;

		if (!Guid.TryParse(userId.Trim(), out userGuid)
			|| userGuid == Guid.Empty)
		{
			success = false;
			errorMsg = "Registered user does not exist.";
		}
		else
		{
			MembershipUser mu = Membership.GetUser(userGuid);
			if (mu == null)
			{
				success = false;
				errorMsg = "Registered user does not exist.";
			}
			else
			{
				success = mu.UnlockUser();
				if (!success)
				{
					errorMsg = "Failed to unlock account.";
				}
			}
		}

		result["success"] = success.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod(true)]
	public static string GetOrganizationDefaultRoles(int organizationId = 1)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "" }, { "roles", "" }, { "error", "" } };
		string roles = string.Empty, errorMsg = string.Empty;
		bool success = false;

		try
		{
			Organization ut = new Organization(organizationId);
			success = ut.Load();

			if (success && ut != null)
			{
				roles = ut.DefaultRolesComma;
				errorMsg = string.Empty;
			}
			else
			{
				roles = string.Empty;
				errorMsg = "unable to load organization roles";
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			success = false;
			roles = string.Empty;
			errorMsg = ex.Message;
		}

		result["success"] = success.ToString();
		result["roles"] = roles;
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
}