using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.AspNet.Membership.OpenAuth;

public partial class Account_Manage : System.Web.UI.Page
{
    protected string SuccessMessage { get; private set; }

    protected bool CanRemoveExternalLogins { get; private set; }

    protected void Page_Load()
    {
        initControls();

        if (!IsPostBack)
        {
            // Determine the sections to render
            var hasLocalPassword = true;// OpenAuth.HasLocalPassword(User.Identity.Name);
            setPassword.Visible = !hasLocalPassword;
            changePassword.Visible = hasLocalPassword;

            CanRemoveExternalLogins = hasLocalPassword;

            // Render success message
            var message = Request.QueryString["m"];
            if (message != null)
            {
                // Strip the query string from action
                Form.Action = ResolveUrl("~/Account/Manage");

                SuccessMessage =
                    message == "ChangePwdSuccess" ? "Your password has been changed."
                    : message == "SetPwdSuccess" ? "Your password has been set."
                    : message == "RemoveLoginSuccess" ? "The external login was removed."
                    : message == "SetQuestionSuccess" ? "Your security question and answer have been changed."
                    : String.Empty;
                successMessage.Visible = !String.IsNullOrWhiteSpace(SuccessMessage);
            }
        }
        
    }

    protected void initControls()
    {
        DataTable dt = UserManagement.GetPasswordQuestions();

        DropDownList ddlQuestion = (DropDownList)ChangePassword1.Controls[0].FindControl("Question");

        ddlQuestion.DataTextField = "PASSWORD_QUESTION";
		ddlQuestion.DataValueField = "PASSWORD_QUESTION";
        ddlQuestion.DataSource = dt;
        ddlQuestion.DataBind();

        MembershipUser mu = Membership.GetUser();
        if (mu == null)
        {
            //show message
            return;
        }
        else
        {
            ListItem item;
            HiddenField txtQuestion = (HiddenField)ChangePassword1.Controls[0].FindControl("txtQuestion");
            if (txtQuestion != null && !string.IsNullOrWhiteSpace(txtQuestion.Value))
            {
                item = ddlQuestion.Items.FindByText(txtQuestion.Value);
            }
            else
            {
                txtQuestion.Value = mu.PasswordQuestion;
                item = ddlQuestion.Items.FindByText(mu.PasswordQuestion);
            }

            if (item != null)
            {
                item.Selected = true;
            }
        }
    }

    protected void setPassword_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            var result = OpenAuth.AddLocalPassword(User.Identity.Name, password.Text);
            if (result.IsSuccessful)
            {
                Response.Redirect("~/Account/Manage.aspx?m=SetPwdSuccess");
            }
            else
            {
                ModelState.AddModelError("NewPassword", result.ErrorMessage);
            }
        }
    }

    
    public IEnumerable<OpenAuthAccountData> GetExternalLogins()
    {
        var accounts = OpenAuth.GetAccountsForUser(User.Identity.Name);
        //CanRemoveExternalLogins = CanRemoveExternalLogins || accounts.Count() > 1;
        return accounts;
    }

    public void RemoveExternalLogin(string providerName, string providerUserId)
    {
        var m = OpenAuth.DeleteAccount(User.Identity.Name, providerName, providerUserId)
            ? "?m=RemoveLoginSuccess"
            : String.Empty;
        Response.Redirect("~/Account/Manage.aspx" + m);
    }
    

    protected static string ConvertToDisplayDateTime(DateTime? utcDateTime)
    {
        // You can change this method to convert the UTC date time into the desired display
        // offset and format. Here we're converting it to the server timezone and formatting
        // as a short date and a long time string, using the current thread culture.
        return utcDateTime.HasValue ? utcDateTime.Value.ToLocalTime().ToString("G") : "[never]";
    }
    protected void buttonChangeQuestionAnswer_Click(object sender, EventArgs e)
    {
        //if (Page.IsValid)
        //{
            string password = "", question = "", answer = "";
            TextBox txt = (TextBox)ChangePassword1.Controls[0].FindControl("CurrentPassword");
            password = txt.Text;

            HiddenField hf = (HiddenField)ChangePassword1.Controls[0].FindControl("txtQuestion");
            question = hf.Value;

            txt = (TextBox)ChangePassword1.Controls[0].FindControl("Answer");
            answer = txt.Text;

            MembershipUser mu = Membership.GetUser();
            if (mu.ChangePasswordQuestionAndAnswer(password, question, answer))
            {
                Response.Redirect("~/Account/Manage.aspx?m=SetQuestionSuccess");
            }            
        //}
    }
}