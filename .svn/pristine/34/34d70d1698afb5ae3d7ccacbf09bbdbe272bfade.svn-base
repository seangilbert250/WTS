using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SiteMaster : MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    protected void Page_Init(object sender, EventArgs e)
    {
        // The code below helps to protect against XSRF attacks
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // Use the Anti-XSRF token from the cookie
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // Generate a new Anti-XSRF token and save to the cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;
    }

    void master_Page_PreLoad(object sender, EventArgs e)
	{
		Page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl("~/Styles/jquery-ui.css") + "\" />"));

		ClientScriptManager cs = Page.ClientScript;
		if (!cs.IsClientScriptIncludeRegistered("jquery"))
		{
			cs.RegisterClientScriptInclude("jquery", ResolveClientUrl("Scripts/jquery-1.11.2.min.js"));
		}
		if (!cs.IsClientScriptIncludeRegistered("jquery-ui"))
		{
			cs.RegisterClientScriptInclude("jquery-ui", ResolveClientUrl("Scripts/jquery-ui.min.js"));
		}
		if (!cs.IsClientScriptIncludeRegistered("common"))
		{
			cs.RegisterClientScriptInclude("common", ResolveClientUrl("Scripts/common.js"));
		}
		if (!cs.IsClientScriptIncludeRegistered("shell"))
		{
			cs.RegisterClientScriptInclude("shell", ResolveClientUrl("Scripts/shell.js"));
		}
		if (!cs.IsClientScriptIncludeRegistered("popupWindow"))
		{
			cs.RegisterClientScriptInclude("popupWindow", ResolveClientUrl("Scripts/popupWindow.js"));
		}

        if (!IsPostBack)
        {
            // Set Anti-XSRF token
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // Validate the Anti-XSRF token
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}