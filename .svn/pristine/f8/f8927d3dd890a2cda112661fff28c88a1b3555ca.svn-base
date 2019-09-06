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


public partial class UserProfile_AddEdit : System.Web.UI.Page
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
        //TODO: load theme for user
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

        Page.Form.DefaultFocus = txtFirstName.ClientID;

        if (this.IsCurrentUser)
        {
            txtMembership_UserId.Value = this.LoggedInMembership_UserId.ToString();
            this.UserId = _loggedInUser.ID;
            txtUserId.Value = this.UserId.ToString();
        }
        else
        {
            if (Request.QueryString["UserID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["UserID"]))
            {
                int.TryParse(this.Server.UrlDecode(this.Request.QueryString["UserID"]), out this.UserId);
                this.txtUserId.Value = this.UserId.ToString();
            }
        }


        if (this.IsCurrentUser || this.UserId > 0)
        {
            IsNew = false;
        }

        InitControls();
    }

    #region Utility
    private void InitControls()
    {
        LoadThemes();
        LoadResourceTypes();
        LoadOrganizations();
		LoadAvailableAttributes();

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
            WTSUtility.SelectDdlItem(this.ddlResourceType, ((int)UserManagement.ResourceType.Programmer_Analyst).ToString());
            WTSUtility.SelectDdlItem(this.ddlOrganization, ((int)UserManagement.Organization.Unauthorized).ToString());
			WTSUtility.SelectDdlItem(this.ddlTheme, "1");
        }

        if (this.IsUserAdmin)
        {
            ddlResourceType.Enabled = true;
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
            ddlResourceType.Enabled = false;
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

        chkIsDeveloper.Disabled = false;
        chkIsBusAnalyst.Disabled = false;
        chkAMCGEO.Disabled = false;
        chkCASUser.Disabled = false;
        chkALODUser.Disabled = false;

    }

    private void LoadUserValues(out WTS_User u)
    {
		u = new WTS_User();
        MembershipUser mu;
        
        if (this.IsCurrentUser)
        {
            u = _loggedInUser;
            mu = _loggedInMembershipUser;
			LoadUserAttributes(u);
        }
        else
        {
            u.Load(this.UserId);
            if (!string.IsNullOrWhiteSpace(u.Membership_UserID) && u.Membership_UserID != Guid.Empty.ToString())
            {
                mu = Membership.GetUser(new Guid(u.Membership_UserID));
				LoadUserAttributes(u);
            }
            else
            {
                mu = UserManagement.GetMembershipUser(u.Email, u.Username);
                if (mu != null)
                {
                    u.Membership_UserID = mu.ProviderUserKey.ToString();
                }
            }
        }
        this.txtMembership_UserId.Value = u.Membership_UserID;

        if (ddlTheme.Items.FindByValue(u.ThemeId.ToString()) == null)
        {
            ddlTheme.Items.Add(new ListItem(u.Theme, u.ThemeId.ToString()));
        }
        WTSUtility.SelectDdlItem(ddlTheme, u.ThemeId.ToString());
        if (ddlOrganization.Items.FindByValue(u.OrganizationID.ToString()) == null)
        {
			ddlOrganization.Items.Add(new ListItem(u.Organization, u.OrganizationID.ToString()));
        }
        if (ddlResourceType.Items.FindByValue(u.ResourceTypeID.ToString()) == null)
        {
            ddlResourceType.Items.Add(new ListItem(u.ResourceType, u.ResourceTypeID.ToString()));
        }
        WTSUtility.SelectDdlItem(ddlResourceType, u.ResourceTypeID.ToString());
        chkAnimations.Checked = u.EnableAnimations;
		WTSUtility.SelectDdlItem(ddlOrganization, u.OrganizationID.ToString());
        chkApproved.Checked = u.IsApproved;
        chkLocked.Checked = u.IsLocked;

        txtMembership_UserId.Value = u.Membership_UserID.ToString();
        txtUsername.Text = u.Username;
        chkArchive.Checked = u.Archive;
        txtPrefix.Text = u.Prefix;
        txtFirstName.Text = u.First_Name;
        txtMiddleInit.Text = u.Middle_Name;
        txtLastName.Text = u.Last_Name;
        txtSuffix.Text = u.Suffix;
        txtPhone.Text = u.Phone_Office;
        txtPhone_Mobile.Text = u.Phone_Mobile;
        txtFax.Text = u.Fax;
        txtEmail.Text = u.Email;
        if (!string.IsNullOrWhiteSpace(u.Email))
        {
            spanEmail.InnerHtml = string.Format("<a href='mailto:{0}' title='Contact Email'>{0}</a>", u.Email);
        }
        txtEmail2.Text = u.Email2;
        if (!string.IsNullOrWhiteSpace(u.Email2))
        {
            spanEmail2.InnerHtml = string.Format("<a href='mailto:{0}' title='Contact Email2'>{0}</a>", u.Email2);
        }
        txtAddress.Text = u.Address;
        txtAddress2.Text = u.Address2;
        txtCity.Text = u.City;
        txtState.Text = u.State;
        txtCountry.Text = u.Country;
        txtPostalCode.Text = u.PostalCode;
        txtNotes.Text = u.Notes;
        chkArchive.Checked = u.Archive;
        chkReceiveSREMail.Checked = u.ReceiveSREMail;
        chkIncludeInSRCounts.Checked = u.IncludeInSRCounts;

        chkIsDeveloper.Checked = u.IsDeveloper;
        chkIsBusAnalyst.Checked = u.IsBusAnalyst;
        chkAMCGEO.Checked = u.IsAMCGEO;
        chkCASUser.Checked = u.IsCASUser;
        chkALODUser.Checked = u.IsALODUser;

    }

	private void LoadUserAttributes(WTS_User u)
	{
		//get user's Resource_Attribute records
		ListItem item = null;

		DataTable dt = u.Resource_AttributeList_Get();
		if (dt == null)
		{
			return;
		}

		string attrID = "", attr = "", resource_AttrID = "";

		foreach (DataRow row in dt.Rows)
		{
			attrID = row["AttributeID"].ToString();
			attr = row["Attribute"].ToString();
			resource_AttrID = row["Attribute"].ToString();

			if (row["Checked"].ToString().ToUpper() == "TRUE"
				|| row["Checked"].ToString() == "1")
			{
				item = chkListAttributes.Items.FindByText(attr);
				if (item != null)
				{
					item.Selected = true;
					item.Attributes.Add("Resource_AttributeID", resource_AttrID);
				}
			}
		}
	}

    private void DisableEdit()
    {
        ddlResourceType.Enabled = false;
        ddlOrganization.Enabled = false;
        txtPrefix.Enabled = false;
        txtFirstName.Enabled = false;
        txtMiddleInit.Enabled = false;
        txtLastName.Enabled = false;
        txtSuffix.Enabled = false;
        txtPhone.Enabled = false;
        txtPhone_Mobile.Enabled = false;
        txtFax.Enabled = false;
        txtEmail.Enabled = false;
        spanEmail.Style["display"] = "block";
        txtEmail2.Enabled = false;
        spanEmail2.Style["display"] = "block";
        txtAddress.Enabled = false;
        txtAddress2.Enabled = false;
        txtCity.Enabled = false;
        txtState.Enabled = false;
        txtCountry.Enabled = false;
        txtPostalCode.Enabled = false;
        txtNotes.Enabled = false;

        chkApproved.Disabled = true;
        chkLocked.Disabled = true;
        buttonUnlock.Disabled = true;
        buttonUnlock.Visible = false;
        chkArchive.Disabled = true;

        buttonSave.Style["display"] = "none";
    }

    private void LoadThemes()
    {
        ddlTheme.Items.Clear();

        DataTable dt = UserManagement.LoadThemeList(includeArchive:false);

        ListItem item = new ListItem("-Select-", "-1");
        ddlTheme.Items.Add(item);

        if (dt == null || dt.Rows.Count == 0) { return; }

        foreach (DataRow row in dt.Rows)
        {
            item = new ListItem(row["THEME"].ToString(), row["THEMEID"].ToString());
            ddlTheme.Items.Add(item);
        }
    }

    private void LoadResourceTypes()
    {
        DataTable dt = UserManagement.LoadResourceTypeList(includeArchive: false);

        ListItem item = new ListItem("-Select-", "-1");
        ddlResourceType.Items.Add(item);

        if (dt == null || dt.Rows.Count == 0) { return; }

        foreach (DataRow row in dt.Rows)
        {
            item = new ListItem(row["WTS_RESOURCE_TYPE"].ToString(), row["WTS_RESOURCE_TYPEID"].ToString());
            ddlResourceType.Items.Add(item);
        }
    }

    private void LoadOrganizations()
    {
        DataTable dt = UserManagement.LoadOrganizationList(includeArchive:false);

        ListItem item = new ListItem("-Select-", "-1");
        ddlOrganization.Items.Add(item);

        if (dt == null || dt.Rows.Count == 0) { return; }

        foreach (DataRow row in dt.Rows)
        {
			item = new ListItem(row["ORGANIZATION"].ToString(), row["ORGANIZATIONID"].ToString());
            ddlOrganization.Items.Add(item);
        }
    }

	private void LoadAvailableAttributes()
	{
		ListItem item = null;

		try
		{
			DataTable dt = WTSData.AttributeList_Get(1); //1 = Resource

			if (dt != null && dt.Rows.Count > 0)
			{
				foreach (DataRow row in dt.Rows)
				{
					item = new ListItem(row["Attribute"].ToString(), row["AttributeId"].ToString());
					item.Attributes.Add("AttributeID", row["AttributeId"].ToString());
					item.Attributes.Add("Attribute", row["Attribute"].ToString());
					this.chkListAttributes.Items.Add(item);
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			throw;
		}
	}

    #endregion Utility

    /// <summary>
    /// Get Registered User from membership "collection"
    /// Search will check email first, then username if that fails
    /// </summary>
    /// <param name="username">username to search with</param>
    /// <param name="email">email to search with</param>
    /// <returns>dictionary containing exists, id, email</returns>
    [WebMethod(true)]
    public static string GetRegisteredUser(string email = "", string username = "")
    {
		Dictionary<string, string> result = new Dictionary<string, string>() { { "exists", "" }, { "id", "" }, { "email", email }, { "username", username }, { "error", "" } };
		bool exists = false;
		string id = string.Empty, errorMsg = string.Empty;
        try
        {
            MembershipUser mu = UserManagement.GetMembershipUser(email, username);
            if (mu != null)
            {
				exists = true;
				id = mu.ProviderUserKey.ToString();
				email = mu.Email;
				username = mu.UserName;
				errorMsg = string.Empty;
            }
            else
            {
				exists = false;
				id = string.Empty;
				email = string.Empty;
				username = string.Empty;
				errorMsg = "Membership user does not exist";
            }
        }
        catch (Exception ex)
        {
			LogUtility.LogException(ex);
			exists = false;
			id = string.Empty;
			email = string.Empty;
			username = string.Empty;
			errorMsg = ex.Message;
        }

		result["exists"] = exists.ToString();
		result["id"] = id.ToString();
		result["email"] = email;
		result["username"] = username;
		result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

	/// <summary>
	/// Save WTS User and corresponding Membership user
	/// </summary>
	/// <returns>dictionary containing "saved", "id", "error", "username", "changedTheme"</returns>
	[WebMethod(true)]
	public static string SaveProfile(int userId = 0, bool registerNew = false
		, string membership_UserId = "", string username = "", int resourcetypeID = 0
        , int organizationId = 0, int themeId = 1, bool enableAnimations = true
		, string prefix = "", string firstName = "", string middleName = "", string lastName = "", string suffix = ""
		, string phone_Office = "", string phone_Mobile = "", string phone_Misc = "", string fax = "", string email = "", string email2 = ""
		, string address = "", string address2 = "", string city = "", string state = "", string country = "USA", string postalCode = ""
		, string notes = "", string attributeFlags = "", bool archive = false, bool receivesremail = false, bool includeInSRCounts = false
        , bool IsDeveloper = false, bool IsBusAnalyst = false, bool IsAMCGEO = false, bool IsCASUser = false, bool IsALODUser = false)
    {
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "id", "" }, { "error", "" }, { "username", "" }, { "changedTheme", "" } };
		string[] flagsList = attributeFlags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		bool saved = false, changedTheme = false;
		string errorMsg = string.Empty;
        int amp = email.IndexOf('@');
        string domainName = email.Substring(0, amp);

        int id = 0;

		try
		{
			//perform the save
			WTS_User u = new WTS_User();
			if (userId > 0)
			{
				u.ID = userId;
				u.Load();
			}

			#region Assign WTS_User object's properties

			u.Membership_UserID = membership_UserId;
			u.Username = username.Replace(" ", "");
            u.ResourceTypeID = resourcetypeID == 0 ? 1 : resourcetypeID;
            u.OrganizationID = organizationId == 0 ? 1 : organizationId;

			if (u.ThemeId != themeId)
			{
				changedTheme = true;
			}
			u.ThemeId = themeId;
			u.EnableAnimations = enableAnimations;
			u.Prefix = prefix;
			u.First_Name = firstName;
			u.Middle_Name = middleName;
			u.Last_Name = lastName;
			u.Suffix = suffix;
			u.Phone_Office = phone_Office;
			u.Phone_Mobile = phone_Mobile;
			u.Phone_Misc = phone_Misc;
			u.Fax = fax;
			u.Email = email;
			u.Email2 = email2;
			u.Address = address;
			u.Address2 = address2;
			u.City = city;
			u.State = state;
			u.Country = country;
			u.PostalCode = postalCode;
			u.Notes = notes;
			u.AttributeFlags = attributeFlags;
			u.Archive = archive;
            u.ReceiveSREMail = receivesremail;
            u.IncludeInSRCounts = includeInSRCounts;

            u.IsDeveloper = IsDeveloper;
            u.IsBusAnalyst = IsBusAnalyst;
            u.IsAMCGEO = IsAMCGEO;
            u.IsCASUser = IsCASUser;
            u.IsALODUser = IsALODUser;

		    u.DomainName = domainName;


            #endregion Assign WTS_User object's properties

            string validationMsg = string.Empty;
			if (!ValidateFields(u, out validationMsg))
			{
				saved = false;
				errorMsg = validationMsg;
				id = -1;
			}
			else
			{
				if (userId == 0)
				{
					#region Add
					if (registerNew)
					{
						//register user before adding profile
						MembershipCreateStatus status = MembershipCreateStatus.Success;
						MembershipUser newMu = Membership.CreateUser(u.Username.Replace(" ", ""), new Guid().ToString(), u.Email, "Please specify a new password question.", "Please specify a new password answer.", true, out status);
						if (status == MembershipCreateStatus.Success)
						{
							//get new registered userid
							u.Membership_UserID = newMu.ProviderUserKey.ToString();
						}
					}
					else
					{
						if (!string.IsNullOrWhiteSpace(u.Membership_UserID)
							&& u.Membership_UserID != Guid.Empty.ToString())
						{
							MembershipUser mu = Membership.GetUser(Guid.Parse(u.Membership_UserID));
						}
					}
					id = u.Add(out saved, out errorMsg);
					username = u.Username;
					#endregion Add
				}
				else
				{
					#region Update
					MembershipUser mu = null;
					if (registerNew)
					{
						MembershipCreateStatus status = MembershipCreateStatus.Success;
						mu = Membership.CreateUser(u.Username, new Guid().ToString(), u.Email, "Please specify a new password question.", "Please specify a new password answer.", true, out status);
						if (status == MembershipCreateStatus.Success)
						{
							u.Membership_UserID = mu.ProviderUserKey.ToString();
						}
					}

                    saved = u.Update(out errorMsg);
					if (saved)
					{
						id = u.ID;
						username = u.Username;
						//if email got updated, then change login email as well
						if (mu == null)
						{
							mu = UserManagement.GetMembershipUser(u.Email, u.Username);
						}

						//update membership user if it exists.
						//it won't exist if updating a profile without registered membership where add login is 'canceled'
						if (mu != null)
						{
							mu.Email = u.Email;
							Membership.UpdateUser(mu);
						}
					}
					#endregion Update
				}
			}

			if (saved &&registerNew)
			{
				Guid resetCode;
				bool requestSubmitted = u.RequestPasswordReset(DateTime.Now.ToUniversalTime().Ticks, out resetCode, out errorMsg);
				if (requestSubmitted)
				{
					UserManagement.SendResetEmail(u.Email, u.Username, resetCode, Guid.Parse(u.Membership_UserID), true);
				}
			}
		}
		catch (Exception ex)
		{
			saved = false;
			errorMsg = ex.Message.ToString();
			id = -1;
			LogUtility.LogException(ex);
		}

		result["saved"] = saved.ToString();
		result["id"] = id.ToString();
		result["error"] = errorMsg;
		result["username"] = username;
		result["changedTheme"] = changedTheme.ToString();

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

    private static bool ValidateFields(WTS_User u, out string validationMsg)
    {
        bool valid = true;
        StringBuilder sbMessages = new StringBuilder();
        validationMsg = string.Empty;

        try
        {
            #region Required fields
            //first name
            if (string.IsNullOrWhiteSpace(u.First_Name))
            {
                sbMessages.AppendLine("First Name is required.");
                valid = false;
            }
            //last name
            if (string.IsNullOrWhiteSpace(u.Last_Name))
            {
                sbMessages.AppendLine("Last Name is required.");
                valid = false;
            }
            //phone number(s)
            if (string.IsNullOrWhiteSpace(u.Phone_Office) 
                && string.IsNullOrWhiteSpace(u.Phone_Mobile))
            {
                sbMessages.AppendLine("A Phone number or Mobile number is required.");
                valid = false;
            }
            //email address
            if (string.IsNullOrWhiteSpace(u.Email))
            {
                sbMessages.AppendLine("Email address is required.");
                valid = false;
            }
            //Resource Type
            if (u.ResourceTypeID <= 0)
            {
                sbMessages.AppendLine("Resource Type must be specified.");
                valid = false;
            }
            //Organization
            if (u.OrganizationID <= 0)
            {
				sbMessages.AppendLine("Organization must be specified.");
                valid = false;
            }
            //theme
            if (u.ThemeId <= 0)
            {
                sbMessages.AppendLine("Please select a display Theme.");
                valid = false;
            }
            #endregion
            
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            sbMessages.AppendLine();
            sbMessages.AppendFormat("An exception was encountered: {0}", ex.Message);
            valid = false;
        }

        validationMsg = sbMessages.ToString();

        return valid;
    }

    /// <summary>
    /// developer only functionality to allow manually setting a user's password
    /// so they don't have to do a password reset email
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void buttonSetPassword_Click(object sender, EventArgs e)
    {
        Guid userId = Guid.Empty;
        string newPW = txtNewPassword.Text.Trim();
        if (string.IsNullOrWhiteSpace(newPW)) { return; }
            
        bool updatedPW = false;

        if (Guid.TryParse(this.txtMembership_UserId.Value.Trim(), out userId) 
            && userId != Guid.Empty){
                MembershipUser mu = Membership.GetUser(userId);
                updatedPW = mu.ChangePassword(mu.ResetPassword(), newPW);
        }
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

}