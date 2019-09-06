using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Membership.OpenAuth;

using Newtonsoft.Json;

public partial class Account_Register : System.Web.UI.Page
{
    protected bool DuplicateEmail = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        initControls();

        RegisterUser.ContinueDestinationPageUrl = Request.QueryString["ReturnUrl"];
        if (!CustomEmailValidator.IsValid)
        {
            DuplicateEmail = true;
        }

        Page.Form.DefaultFocus = this.txtFirstName.ClientID;
    }

    protected void initControls()
    {
        DataTable dt = UserManagement.GetPasswordQuestions();

        DropDownList ddlQuestion = (DropDownList)RegisterUserWizardStep.Controls[0].FindControl("Question");

		ddlQuestion.DataTextField = "PASSWORD_QUESTION";
		ddlQuestion.DataValueField = "PASSWORD_QUESTION";
        ddlQuestion.DataSource = dt;
        ddlQuestion.DataBind();
    }

    protected void RegisterUser_CreatedUser(object sender, EventArgs e)
    {
        FormsAuthentication.SetAuthCookie(RegisterUser.UserName, createPersistentCookie: false);

        string userName = (sender as CreateUserWizard).UserName.Replace(" ", "");
        string email = (sender as CreateUserWizard).Email.Replace(" ", "");
        MembershipUser u = Membership.GetUser(userName);
        u.IsApproved = false;
        Membership.UpdateUser(u);

        string userId = u.ProviderUserKey.ToString();
        int profileId = 0;
        int.TryParse(this.txtProfileUserId.Value.Trim(), out profileId);

        SaveProfile(userName: userName, email: email, userId: userId, profileId: profileId);

        string continueUrl = RegisterUser.ContinueDestinationPageUrl;
        if (!OpenAuth.IsLocalUrl(continueUrl))
        {
            continueUrl = "~/";
        }

        //send email to user admin to notify of new user to activate
        bool sent = UserManagement.SendRegistrationNotificationEmail(username: userName, firstName: this.txtFirstName.Text.Trim(), lastName: this.txtLastName.Text.Trim());

        Response.Redirect(continueUrl);
    }

    protected bool SaveProfile(string userName = "", string email = "", string userId = "", int profileId = 0)
    {
        string errorMsg = string.Empty;
        bool saved = false;

		//if (profileId == 0)
		//{
		//	//search for existing profile
		//	DataTable dtUsers = UserManagement.FindUnregisteredUsers(userName.Trim(), email.Trim());
		//	if (dtUsers != null && dtUsers.Rows.Count > 0)
		//	{
		//		int.TryParse(dtUsers.Rows[0]["Id"].ToString(), out profileId);
		//	}
		//}

		bool isNew = (profileId == 0);
		WTS_User u = new WTS_User();
		if (!isNew)
		{
			u.Load(profileId);
		}

		//u.Username = userName;
		//u.Membership_UserID = userId;
		//u.Email = email;
		//LoadProfileValues(ref u);
        
		//if (isNew)
		//{
		//	profileId = u.Add(out saved, out errorMsg);
		//}
		//else
		//{
		//	saved = u.Update(out errorMsg);
		//}

        return saved;
    }

	//private void LoadProfileValues(ref WTS_User u)
	//{
	//	u.Prefix = this.txtPrefix.Text.Trim();
	//	u.First_Name = this.txtFirstName.Text.Trim();
	//	u.Middle_Name = this.txtMiddleInit.Text.Trim();
	//	u.Last_Name = this.txtLastName.Text.Trim();
	//	u.Suffix = this.txtSuffix.Text.Trim();
	//	u.Email2 = this.txtEmail2.Text.Trim();
	//	u.Phone_Office = this.txtPhone.Text.Trim();
	//	u.Phone_Mobile = this.txtPhone_Mobile.Text.Trim();
	//	u.Address = this.txtAddress.Text.Trim();
	//	u.Address2 = this.txtAddress2.Text.Trim();
	//	u.City = this.txtCity.Text.Trim();
	//	u.State = this.txtState.Text.Trim();
	//	u.PostalCode = this.txtPostalCode.Text.Trim();
	//	u.Country = this.txtCountry.Text.Trim();
	//}

    private static bool UserExists(string username)
    {
        if (Membership.GetUser(username) != null) { return true; }

        return false;
    }

    private static bool UserExistsWithEmail(string email, out string username)
    {
        MembershipUserCollection memUsers = Membership.FindUsersByEmail(email);
        username = string.Empty;

        if (memUsers.Count > 0)
        {
            foreach (MembershipUser u in memUsers)
            {
                username = u.UserName;
                return true;
            }
        }

        return false;
    }

    protected void RegisterUser_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        switch (e.CurrentStepIndex)
        {
            case 0:
                Page.Validate("Profile");
                //finishing the profile step
                if (!RequiredFieldValidatorFirstName.IsValid
                    || !RequiredFieldValidatorLastName.IsValid
                    || !RequiredFieldValidatorProfileEmail.IsValid
                    || !CustomEmailValidator.IsValid)
                {
                    e.Cancel = true;
                    return;
                }

                string userName = txtProfileUserName.Text.Trim();// string.Format("{0}.{1}", txtFirstName.Text.Trim(), txtLastName.Text.Trim());
                if (UserExists(userName))
                {
                    var lastChar = userName.Substring(userName.Length - 1, 1);
                    var num = 1;
                    if (int.TryParse(lastChar, out num))
                    {
                        num += 1;
                        userName = userName.Substring(0, userName.Length - 1);
                    }
                    else
                    {
                        num = 1;
                    }
                    userName += num.ToString();
                }

                (sender as CreateUserWizard).UserName = userName;

                string email = txtProfileEmail.Text.Trim();
                (sender as CreateUserWizard).Email = email;

                e.Cancel = false;
                break;
        }
    }

    [WebMethod()]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string FindUnregisteredUsers(string userName = "", string email = "")
    {
		DataTable dtUsers = null;// UserManagement.FindUnregisteredUsers(userName.Trim(), email.Trim());
        
        return JsonConvert.SerializeObject(dtUsers, Formatting.None);
    }

    [WebMethod()]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string FindRegisteredUser(string email = "")
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        string username = string.Empty;
        if (UserExistsWithEmail(email, out username))
        {
            result.Add("exists", "true");
        }
        else
        {
            result.Add("exists", "false");
        }
        result.Add("username", username);
        
        return JsonConvert.SerializeObject(result, Formatting.None);
    }
    protected void CustomEmailValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string username = string.Empty;
        bool exists = UserExistsWithEmail(txtProfileEmail.Text.Trim(), out username);
        if (exists)
        {
            txtExistingUsername.Value = username;
            args.IsValid = false;
        }
    }
}