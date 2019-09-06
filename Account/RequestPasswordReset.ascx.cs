using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Account_RequestPasswordReset : System.Web.UI.UserControl
{
    public UserManagement.PasswordResetRequestType RequestType
    {
        get { return (UserManagement.PasswordResetRequestType)Enum.Parse(typeof(UserManagement.PasswordResetRequestType), this.txtRequestType.Value.ToString().Trim()); }
        set { txtRequestType.Value = value.ToString(); }
    }
    
    public string Username
    {
        get { return this.txtResetUsername.Text.Trim(); }
        set { this.txtResetUsername.Text = value.Trim(); }
    }
    public string EmailAddress
    {
        get { return this.txtResetEmail.Text.Trim(); }
        set { this.txtResetEmail.Text = value.Trim(); }
    }

    
    protected void Page_Load(object sender, EventArgs e)
    {
        ClientScriptManager cs = Page.ClientScript;
        if (!cs.IsClientScriptIncludeRegistered("common"))
        {
            cs.RegisterClientScriptInclude("common", ResolveClientUrl("~/Scripts/common.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered("jquery"))
        {
			cs.RegisterClientScriptInclude("jquery", ResolveClientUrl("~/Scripts/jquery-1.11.2.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered("shell"))
        {
            cs.RegisterClientScriptInclude("shell", ResolveClientUrl("~/Scripts/shell.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered("popupWindow"))
        {
            cs.RegisterClientScriptInclude("popupWindow", ResolveClientUrl("~/Scripts/popupWindow.js"));
        }
    }

    protected void buttonRequestReset_Click(object sender, EventArgs e)
    {
        if (this.txtRequestType.Value.ToString() == UserManagement.PasswordResetRequestType.Username.ToString() 
            && string.IsNullOrWhiteSpace(txtResetUsername.Text))
        {
            this.buttonRequestReset.Enabled = true;
            FailureText.Text = "Invalid username.";
            this.divMessages.Style["display"] = "block";
            this.divMessages.Style["color"] = "red";
            this.divFormDisabler.Style["display"] = "none";
            return;
        }
        else if (this.txtRequestType.Value.ToString() == UserManagement.PasswordResetRequestType.EmailAddress.ToString()
            && string.IsNullOrWhiteSpace(txtResetEmail.Text))
        {
            this.buttonRequestReset.Enabled = true;
            FailureText.Text = "Invalid email address.";
            this.divMessages.Style["display"] = "block";
            this.divMessages.Style["color"] = "red";
            this.divFormDisabler.Style["display"] = "none";
            return;
        }

        this.divMessages.Style["display"] = "none";

        labelResultMessage.Text = string.Empty;

        if (this.txtRequestType.Value.ToString() == UserManagement.PasswordResetRequestType.Username.ToString())
        {
            SubmitRequest_Username();
        }
        else
        {
            SubmitRequest_Email();
        }
    }

    private void SubmitRequest_Email()
    {
        try
        {
            string msg = string.Empty;
            string email = txtResetEmail.Text.Trim();
            string username = string.Empty;
            MembershipUserCollection muc = Membership.FindUsersByEmail(email);
            MembershipUser mu = null;
            foreach (MembershipUser tempMU in muc)
            {
                mu = tempMU;
                email = tempMU.Email;
            }
            if (mu == null)
            {
                FailureText.Text = "An invalid email was specified.";
                return;
            }

            bool submitted = false, emailed = false;
            SubmitRequest(mu, out submitted, out emailed);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
        finally
        {
            this.divFormDisabler.Style["display"] = "none";
        }
    }

    private void SubmitRequest_Username()
    {
        try
        {
            string msg = string.Empty;
            string username = txtResetUsername.Text.Trim();
            MembershipUser mu = Membership.GetUser(username);
            if (mu == null)
            {
                FailureText.Text = "An invalid username was specified.";
                return;
            }

            bool submitted = false, emailed = false;
            SubmitRequest(mu, out submitted, out emailed);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
        finally
        {
            this.divFormDisabler.Style["display"] = "none";
        }
    }

    private void SubmitRequest(MembershipUser mu, out bool submitted, out bool emailed)
    {
        submitted = false;
        emailed = false;

        WTS_User u = new WTS_User(mu.ProviderUserKey.ToString());
        u.Load_GUID();
        Guid resetCode = Guid.Empty;
        string errorMsg = string.Empty;
        submitted = u.RequestPasswordReset(DateTime.Now.ToUniversalTime().Ticks, out resetCode, out errorMsg);

        string msg = string.Empty;
        if (submitted)
        {
            //show confirmation message
            emailed = UserManagement.SendResetEmail(mu.Email, mu.UserName, resetCode, (Guid)mu.ProviderUserKey);
            msg = "An email with steps to follow will be sent to your registered email address.";
        }
        else
        {
            msg = "We were unable to submit your password reset request.";
        }
        labelResultMessage.Text = msg;
        this.divMessages.Style["display"] = "block";
    }
}