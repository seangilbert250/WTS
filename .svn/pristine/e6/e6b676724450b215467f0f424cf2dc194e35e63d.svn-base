using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Account_Reset : System.Web.UI.Page
{
    private Guid _resetCode = Guid.Empty;
    private Guid _userId = Guid.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string requestReset = string.Empty, resetCodeString = string.Empty, userIdString = string.Empty;
            string requestType = "username", email = string.Empty;

            if (Request.QueryString["requestReset"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["requestReset"].ToString()))
            {
                if (Request.QueryString["requestType"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["requestType"].ToString()))
                {
                    requestType = Server.UrlDecode(Request.QueryString["requestType"].ToString().Trim());
                    if (requestType.ToUpper() == "EMAIL")
                    {
                        this.ucRPR.RequestType = UserManagement.PasswordResetRequestType.EmailAddress;
                    }
                    else
                    {
                        this.ucRPR.RequestType = UserManagement.PasswordResetRequestType.Username;
                    }
                }
                if (Request.QueryString["email"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["email"].ToString()))
                {
                    this.ucRPR.EmailAddress = Server.UrlDecode(Request.QueryString["email"].ToString().Trim());
                }
                if (Request.QueryString["username"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["username"].ToString()))
                {
                    this.ucRPR.Username = Server.UrlDecode(Request.QueryString["username"].ToString().Trim());
                }

                requestReset = Server.UrlDecode(Request.QueryString["requestReset"]);
                if (!string.IsNullOrWhiteSpace(requestReset) && requestReset.ToUpper() == "TRUE")
                {
                    divExpired.Style["display"] = "block";
                    divPasswordFields.Style["display"] = "none";
                    return;
                }
            }
            if (Request.QueryString["resetcode"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["resetcode"].ToString()))
            {
                resetCodeString = HttpUtility.UrlEncode(Request.QueryString["resetcode"]);
                this.hiddenResetCode.Value = resetCodeString;
            }
            if (Request.QueryString["userid"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["userid"].ToString()))
            {
                userIdString = HttpUtility.UrlEncode(Request.QueryString["userid"]);
                this.hiddenUserId.Value = userIdString;
            }

            string errorMsg = string.Empty;
            if (!ValidateReset(resetCodeString, userIdString, out errorMsg))
            {
                //show message
                //allow resubmitting request
                this.RequestValidationFailureText.Text = errorMsg;
                divExpired.Style["display"] = "block";
                divPasswordFields.Style["display"] = "none";
            }
        }
    }

    protected bool ValidateReset(string resetCodeString, string userIdString, out string errorMsg)
    {
        errorMsg = string.Empty;

        if (string.IsNullOrWhiteSpace(resetCodeString) || !Guid.TryParse(resetCodeString, out _resetCode)
            || string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out _userId))
        {
            errorMsg = "Invalid password reset request. You may resend your request below.";
            return false;
        }

        //validate userid exists
        MembershipUser u = Membership.GetUser(_userId);
        if (u == null || !ValidateRequestValues(out errorMsg))
        {
            return false;
        }

        return true;
    }

    protected bool ValidateRequestValues(out string errorMsg)
    {
        errorMsg = string.Empty;

        //check database for valid resetcode and userid combination
        DataTable dt = UserManagement.GetPasswordResetRequest(_resetCode, _userId);
        if (dt == null || dt.Rows.Count == 0)
        {
            errorMsg = "Invalid password reset request. You may resend your request below.";
            return false;
        }
        else
        {
            long sentDateTicks = 0;
            long.TryParse(dt.Rows[0]["requestDateTicks"].ToString(), out sentDateTicks);
            if (!ValidateExpiration(sentDateTicks))
            {
                errorMsg = "Your password reset request has expired. Please resend."
                    + "<br/>" 
                    + "A new email will be sent to your registered email address.";
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    protected bool ValidateExpiration(long requestDateTicks)
    {
        int expirationMinutes = WTSConfiguration.PasswordResetExpiration;

        DateTime requestDate = new DateTime(requestDateTicks);
        DateTime expirationDate = requestDate.AddMinutes(expirationMinutes);
        if (DateTime.Now.ToUniversalTime() >= expirationDate)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected void buttonSetPassword_Click(object sender, EventArgs e)
    {
        if (!this.RequiredFieldValidator4.IsValid 
            || !this.RequiredFieldValidator5.IsValid
            || !this.CompareValidator2.IsValid)
        {
            return;
        }

        this.buttonSetPassword.Enabled = false;

        string password = NewPassword.Text.Trim();
        string confirmPassword = ConfirmNewPassword.Text.Trim();

        Guid userId;
        Guid.TryParse(hiddenUserId.Value.Trim(), out userId);

		try
		{
			MembershipUser mu = Membership.GetUser(userId);
			if (mu != null)
			{
				string tempPassword = mu.ResetPassword();
				if (mu.ChangePassword(tempPassword, password))
				{
					//delete reset request
					ClearResetRequest((Guid)mu.ProviderUserKey);

					Response.Redirect("~/Default.aspx");
				}
			}
		}
		catch (System.Threading.ThreadAbortException)
		{
			//expected. do nothing
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			throw;
		}
    }

    private bool ClearResetRequest(Guid userId)
    {
        try
        {
            return UserManagement.ClearPasswordResetRequests(userId);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            return false;
        }
    }
}