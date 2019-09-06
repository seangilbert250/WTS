using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

public partial class Admin_Organization_AddEdit : System.Web.UI.Page
{
    #region Properties
	protected bool IsUserAdmin = false;
	protected bool IsAdmin { get; set; }

	protected bool ViewOnly = false;
	protected bool IsNew = true;

    protected int OrganizationId = 0;

    public string LoggedInMembership_UserId { get; set; }
    public int LoggedInProfileId { get; set; }

    protected WTS_User _loggedInUser;
    protected MembershipUser _loggedInMembershipUser;

    #endregion


    #region Events
    protected void Page_PreInit(object sender, EventArgs e)
    {
        //load theme for user
		Page.Theme = WTSUtility.ThemeName;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        #region QueryString

        if (Request.QueryString["View"] == null || string.IsNullOrWhiteSpace(Request.QueryString["View"]))
        {
            ViewOnly = false;
        }
        else
        {
            bool.TryParse(this.Server.UrlDecode(Request.QueryString["View"]).ToLower(), out ViewOnly);
        }

        if (Request.QueryString["OrganizationID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["OrganizationID"]))
        {
            int.TryParse(this.Server.UrlDecode(this.Request.QueryString["OrganizationId"]), out OrganizationId);
        }

        if (OrganizationId > 0)
        {
            this.txtOrganizationId.Value = OrganizationId.ToString();
            IsNew = false;
        }

        #endregion
        
        #region Logged In User details

        _loggedInMembershipUser = Membership.GetUser();
        this.LoggedInMembership_UserId = _loggedInMembershipUser.ProviderUserKey.ToString();
        this.IsAdmin = Roles.IsUserInRole("Admin");

		_loggedInUser = new WTS_User();
        _loggedInUser.Load(this.LoggedInMembership_UserId);
        this.LoggedInProfileId = UserManagement.GetUserId(this.LoggedInMembership_UserId);
        IsUserAdmin = UserManagement.UserIsUserAdmin(this.LoggedInProfileId);

        #endregion Logged In User details


        Page.Form.DefaultFocus = txtOrganization.ClientID;

        InitControls();
    }
    #endregion Events


    #region Utility
    private void InitControls()
    {
        LoadAvailableRoles();

        if (!IsNew && OrganizationId > 0)
        {
            Organization ut = new Organization(OrganizationId);
            ut.Load();

            txtOrganization.Text = ut.OrganizationNm;
            txtDescription.Text = ut.Description;
            SelectDefaultRoles(ut);
            chkArchive.Checked = ut.Archive;
            ListItem item = new ListItem();
            foreach (int id in ut.Users.Keys)
            {
                item = new ListItem(ut.Users[id].Trim(), id.ToString());
                lstUsers.Items.Add(item);
            }
        }

        if (ViewOnly)
        {
            txtOrganization.Enabled = false;
            txtDescription.Enabled = false;
            chkArchive.Disabled = true;
        }

        lstUsers.Enabled = false;
    }

    private void LoadAvailableRoles()
    {
        ListItem item = null;

        try
        {
            UserManagement.Organization currentOrganization = (UserManagement.Organization)_loggedInUser.OrganizationID;
            string[] roles = Roles.GetAllRoles();
            foreach (string r in roles)
            {
                switch (r.ToLower())
                {
                    case "admin":
					case "administration":
					case "view:administration":
					case "resourcemanagement":
					case "view:resourcemanagement":
						if (!IsUserAdmin
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
        }
    }

    private void SelectDefaultRoles(Organization ut)
    {
        ListItem item = null;
        foreach (string r in ut.DefaultRoles)
        {
            item = chkListRoles.Items.FindByValue(r);
            if (item != null)
            {
                item.Selected = true;
            }
        }
    }

    #endregion


    #region Save

    [WebMethod(true)]
    public static string SaveOrganization(int OrganizationId = 0, string Organization = ""
        , string description = "", bool archive = false
        , string roles = "")
    {
		string errorMsg = string.Empty;
		bool saved = false;
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "id", OrganizationId.ToString() }, { "error", "" }, { "Organization", "" } };

		try
		{
			#region Assign Organization object properties
			Organization o = new Organization();
			if (OrganizationId > 0)
			{
				o.Load(OrganizationId);
			}
			o.OrganizationNm = Organization.Trim();
			o.Description = description.Trim();
			o.Archive = archive;

			o.DefaultRoles.Clear();
			foreach (string r in roles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				o.AddDefaultRole(r);
			}

			#endregion

			string validationMsg = string.Empty;
			if (!ValidateFields(o, out validationMsg))
			{
				saved = false;
				errorMsg = validationMsg;
			}
			else
			{
				if (OrganizationId == 0)
				{
					OrganizationId = o.Add(out saved, out errorMsg);
				}
				else
				{
					saved = o.Update(out errorMsg);
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
			errorMsg += Environment.NewLine + ex.ToString();
		}

		result["saved"] = saved.ToString();
		result["id"] = OrganizationId.ToString();
		result["error"] = errorMsg;
		result["Organization"] = Organization;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    private static bool ValidateFields(Organization o, out string validationMsg)
    {
        bool valid = true;
        StringBuilder sbMessages = new StringBuilder();
        validationMsg = string.Empty;

        try
        {
            #region Required fields
            //Organization
            if (string.IsNullOrWhiteSpace(o.OrganizationNm))
            {
                sbMessages.AppendLine("Organization is required.");
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
    #endregion Save
}