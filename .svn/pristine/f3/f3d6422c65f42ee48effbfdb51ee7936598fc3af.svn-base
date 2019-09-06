using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.AspNet.Membership.OpenAuth;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;
using System.Configuration;

public partial class Account_WTS_Login : System.Web.UI.Page
{
	protected string Environment = "";

    protected void Page_Load(object sender, EventArgs e)
    {
		string returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
		if (!String.IsNullOrWhiteSpace(returnUrl))
		{
			RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
		}
        if (ConfigurationManager.AppSettings["EnableCAFDExSSO"] == "true")
        {
            if (Request.Cookies["CAFDExSSO"] != null)
            {

                //LogUtility.LogInfo("checking if membership user is logged in and authenticated");
                string CAFDExUserName = Request.Cookies["CAFDExSSO"].Value;
                if (ConfigurationManager.AppSettings["TestCAFDExSSO"] == "true")
                {
                    CAFDExUserName = string.IsNullOrEmpty(CAFDExUserName) ? "DERIK.J.HARRIS" : CAFDExUserName;
                }
                string reformattedUserName = "";
                if (!string.IsNullOrEmpty(CAFDExUserName))
                {
                    string[] sUserName = CAFDExUserName.Split('.');
                    if (sUserName.Length == 3)
                    {
                        reformattedUserName = sUserName[0] + "." + sUserName[2];
                    }
                    else
                    {
                        reformattedUserName = CAFDExUserName;
                    }
                }
                MembershipUser mu = Membership.GetUser(reformattedUserName);

                if (mu != null)
                {
                    FormsAuthentication.SetAuthCookie(mu.UserName, false);
                    Response.Redirect("~/Default.aspx");

                }
            }
        }

        RegisterHyperLink.NavigateUrl = "Register.aspx";

		if (User.Identity.IsAuthenticated == true)
		{
			Response.Redirect(returnUrl);
		}

		Page.Form.DefaultFocus = LoginCtrl.FindControl("UserName").ClientID;

		if (IsPostBack)
		{
			System.Threading.Thread.Sleep(1500); //timeout needed to properly slide the menu bar

			var provider = Request.Form["provider"];
			if (provider == null) { return; }

			//var redirectUrl = "~/ExternalLoginLandingPage.aspx";
			var redirectUrl = "~/WTS_Login.aspx";
			if (!String.IsNullOrWhiteSpace(returnUrl))
			{
				var resolvedReturnUrl = ResolveUrl(returnUrl); //returnUrl;
				redirectUrl += "?ReturnUrl=" + HttpUtility.UrlEncode(resolvedReturnUrl);
			}

			OpenAuth.RequestAuthentication(provider, redirectUrl);
		}
    }

	protected void SetProdTestInfo()
	{
		if (WTSConfiguration.Environment.ToUpper() == "TEST")
		{
			this.Environment = "test";

			this.testSystemIndicator.Style["display"] = "block";
		}
	}


	public IEnumerable<ProviderDetails> GetProviderNames()
	{
		return OpenAuth.AuthenticationClients.GetAll();
	}

	public Guid StringToGUID(string value)
	{
		var md5Hasher = MD5.Create();
		var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
		return new Guid(data);
	}
	
	protected void LoadCopyright()
	{
		lblCopyRight.Text = WTSConfiguration.CopyrightYear.ToString();
		lblContractorNm.Text = WTSConfiguration.CopyrightContractor.Trim();
	}
}