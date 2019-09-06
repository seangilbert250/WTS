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


public partial class UserProfileEditParent : System.Web.UI.Page
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
			if (this.IsPopup)
			{
				this.buttonBackToGrid.InnerText = "Close";
			}
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
			labelUserName.InnerText = this._loggedInMembershipUser.UserName;
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

		//get user details
		if (!IsNew)
		{
			WTS_User u = null;
			LoadUserValues(out u);
		}
    }


	private void LoadUserValues(out WTS_User u)
	{
		u = new WTS_User();
		u.Load(this.UserId);
		
		if (!string.IsNullOrWhiteSpace(u.Username))
		{
			labelUserName.InnerText = u.Username;
		}
	}

}