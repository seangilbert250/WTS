﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;


public partial class _Default : System.Web.UI.Page
{
	#region Properties and Variables
    protected string FromServer = "";
	protected string UserName = string.Empty;
	protected int UserId = 0;
	protected bool IsUserAdmin = false;

	protected string DefaultSystemFilters = string.Empty;
	protected string WTSFilters = string.Empty;
	protected string SessionID = string.Empty;
	protected bool IsAuthorized = false;

	private WTS_User _loggedInProfile = null;
	protected string _useAnimations = "false";
    protected string defaultStartModule = "";

    protected string defaultStartGrid = "";
    //protected int defaultWorkloadGrid = 3;
    protected string defaultWorkloadGrid = "";
    protected string defaultCrosswalkGrid = "";
    protected string defaultWorkRequestGrid = "";
    protected string defaultHotlistGrid = "";

    protected string ApplicationHost = ConfigurationManager.AppSettings["ApplicationHost"];

    protected bool showSysNotification = false;
	/// <summary>
	/// used to find correct URL for ajax calls
	/// </summary>
	protected string RootUrl = string.Empty;

	#region User Settings

	private enum StartUpPage
	{
		News = 1,
		Metrics = 2
	}
	protected int StartupPageId = (int)StartUpPage.News;
	protected bool WebsystemsInNewWindow = true;
	protected string Environment = "";

	#endregion

	protected string ViewInfo = string.Empty;

	#endregion Properties and Variables


	protected void Page_Load(object sender, EventArgs e)
	{
		readQueryString();

		if (!Page.IsPostBack)
		{
			LoadCopyRight();
			SetProdTestInfo();
			UserManagement.LoadLicenses();

			Login();
			checkStartupPage();

            //11626 - 2:
            LoadUserPrefs();
            LoadSystemNofitication();

            LoginNameControl.Attributes.Add("onclick", "showUserProfile();");
			LoadHelpMenu();
			LoadCVTMenu();
		}
	}

	private void readQueryString()
	{
		RootUrl = Request.Url.GetLeftPart(UriPartial.Authority);// +Request.ApplicationPath;
	}

    protected void LoadUserPrefs()
    {
		// 11626 - 2:
		// Set defaults first, just in case.
		Session["defaultAORGrid"] = "Default";
		Session["defaultStartModule"] = "My Data";
        Session["defaultCrosswalkGrid"] = "Default";
        Session["defaultWorkloadGrid"] = "3"; // Workload

        DataTable dt = WTSData.GetUserPreferences(this.UserId);
        if (dt != null)
        {
            DataTableReader reader = new DataTableReader(dt);

            while (reader.Read())
            {
                switch (reader["GridNameID"].ToString())
                {
                    case "9":  // Default
                        Session["defaultStartModule"] = reader["ViewName"].ToString();
                        break;

                    case "10":  // Crosswalk 
                        Session["defaultCrosswalkGrid"] = reader["ViewName"].ToString();
                        break;

                    case "1":  // View
                        Session["defaultWorkloadGrid"] = reader["SettingValue"].ToString();
                        break;

					case "11":  // View
						Session["defaultAORGrid"] = reader["SettingValue"].ToString();
						break;
				}
			}
        }
        else
        {
            // SCB TODO: Alert use to new preference options?
                        
        }
    }

    protected void LoadSystemNofitication()
    {
        DataTable dt = new DataTable();
        dt = WTSNews.GetNews(newsId: 0, sysNotification: 1);

        if (dt != null && dt.Rows.Count > 0)
        {
            StringBuilder summarytable = new StringBuilder();
            summarytable.AppendFormat("<table cellspacing='0' cellpadding='0' width='98%'>");
            summarytable.AppendFormat("<tr class='gridHeader'><td style='padding-right: 10px'>Date(s)</td><td>Title</td></tr>");

            foreach (DataRow dr in dt.Rows)
            {
                summarytable.AppendFormat("<tr class='gridBody' style='cursor:pointer'>");
                for (int x = 0; x <= dt.Columns.Count - 1; x++)
                {
                    //'value field
                    summarytable.AppendFormat("<td style='color:black'>");

                    summarytable.AppendFormat(dr[x].ToString());
                    summarytable.AppendFormat("</td>");
                }

                summarytable.AppendFormat("</tr>");
            }

            summarytable.AppendFormat("</table>");
            divSystemNoteInfo.InnerHtml = summarytable.ToString();

            showSysNotification = true;
        }
    }
    protected void SetProdTestInfo()
	{
		if (WTSConfiguration.Environment.ToUpper() == "TEST")
		{
			this.Environment = "test";

                try
		        {
		            FromServer = WTSConfiguration.ErrorEmailFromName;
		        }
		        catch (Exception ex)
		        {
		           FromServer="TEST SYSTEM";
		        }


	
			this.divTestSystemIndicator.Style["display"] = "block";
		}
	}

	protected void checkStartupPage()
	{
		DataTable dt = null;
		//dt = WTSNews.LoadNewsArticles("21");
		//Session["dtNews"] = dt;

		if (dt == null || dt.Rows.Count == 0)
		{
			StartupPageId = (int)StartUpPage.Metrics;
		}
		else
		{
			dt.DefaultView.RowFilter = " VIEW_DT IS NULL ";
			dt = dt.DefaultView.ToTable();
			if (dt == null || dt.Rows.Count == 0)
			{
				StartupPageId = (int)StartUpPage.Metrics;
			}
			else
			{
				StartupPageId = (int)StartUpPage.News;
			}
		}
	}

	protected void Login()
	{
		this.IsAuthorized = true;
		LogUtility.LogInfo("checking if membership user is logged in and authenticated");
		MembershipUser u = Membership.GetUser();
		if (u != null)
		{
			this.IsAuthorized = u.IsApproved;
			_loggedInProfile = new WTS_User(u.ProviderUserKey.ToString());
			_loggedInProfile.Load_GUID();

			//Always include the default theme
			UserManagement.AddDefaultTheme(Page, _loggedInProfile);

            //11626 - 2:
            LoadUserPrefs();

            //SetAnimations
            _useAnimations = _loggedInProfile.EnableAnimations.ToString();

			#region Debug Logging
			if (WTSConfiguration.DebugMode)
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine();
				sb.AppendLine("ALL available roles: ");
				string[] availRoles = Roles.GetAllRoles();
				if (availRoles.Length > 0)
				{
					foreach (string r in availRoles)
					{
						sb.AppendLine("    " + r);
					}
				}

				sb.AppendLine();
				sb.AppendLine();

				sb.AppendFormat("User {0} is logged in and is {1}approved.", u.UserName, u.IsApproved ? "" : "NOT ");
				sb.AppendLine();
				sb.AppendLine(string.Format("Organization = {0}", _loggedInProfile.Organization));
				sb.AppendLine();
				string[] roles = Roles.GetRolesForUser();
				if (roles.Length > 0)
				{
					sb.AppendLine("User is in roles:  ");
					foreach (string r in roles)
					{
						sb.AppendLine("    " + r);
					}
				}
				else
				{
					sb.AppendLine("User has no roles assigned.");
				}
				LogUtility.LogInfo(sb.ToString());
			}
			#endregion
		}
		else
		{
			if (WTSConfiguration.DebugMode) { LogUtility.LogInfo("User was not found or is not logged in."); }
		}

		this.UserName = HttpContext.Current.User.Identity.Name;
		this.UserId = _loggedInProfile.ID;
		this.IsUserAdmin = UserManagement.UserIsUserAdmin(username: UserName);
		this.SessionID = HttpContext.Current.Session.SessionID;

		lblUser.Text = "Welcome to WTS -";

		//Load Filters
		LoadFilters();

        //11626 - 2:
        LoadUserPrefs();

        //Load menu and module options for the logged in user
        LoadAvailableOptions(u);
		LoadCustomUserFilters();
	}

	protected void LoadAvailableOptions(MembershipUser u)
	{
		trAdmin.Visible = false;
		string[] roles = UserManagement.GetUserRoles_Membership();
		List<string> userRoles = new List<string>(roles);

        //Get View options
        DataTable dt = WTSData.GetViewOptions(userId: this.UserId, gridName: "Default");

		if (dt != null && dt.Rows.Count > 0)
		{
			ddlView_Work.DataTextField = "ViewName";
			ddlView_Work.DataValueField = "GridViewID";
			ddlView_Work.DataSource = dt;
			ddlView_Work.DataBind();

			if (dt.Columns.Contains("ViewName") && dt.Columns.Contains("ViewDescription"))
			{
				ViewInfo = string.Empty;
				foreach (DataRow row in dt.Rows) ViewInfo += string.Format("{0}:<br />{1}<br /><br />", row["ViewName"].ToString(), row["ViewDescription"].ToString());
			}
		}
        // 11626 - 2: get saved View Option for this user
        ListItem item = ddlView_Work.Items.FindByText(Session["defaultStartModule"].ToString());
        //ListItem item = ddlView_Work.Items.FindByText(defaultStartGrid);
        if (item != null)
		{
			item.Selected = true;
		}
		else
		{
			ddlView_Work.SelectedIndex = 0;
		}

		setAdminOptions(userRoles);
		setMasterDataOptions(userRoles);
		setReportsOptions(userRoles);
		setWorkOptions(userRoles);
        setMeetingOptions(userRoles);
        setAOROptions(userRoles);
        setRQMTOptions(userRoles);
        setSROptions(userRoles);
    }

	protected void setAdminOptions(List<string> userRoles)
	{
		if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) >= 0
			|| userRoles.FindIndex(s => s.StartsWith("resourcemanagement", StringComparison.CurrentCultureIgnoreCase)) >= 0)
		{
			trAdmin.Visible = true;
			//which options

		}
		else
		{
			trAdmin.Visible = false;
		}
	}

	protected void setMasterDataOptions(List<string> userRoles)
	{
		if (userRoles.Contains("Admin")
			|| userRoles.FindIndex(s => s.StartsWith("masterdata", StringComparison.CurrentCultureIgnoreCase)) >= 0
			|| userRoles.FindIndex(s => s.StartsWith("view:masterdata", StringComparison.CurrentCultureIgnoreCase)) >= 0)
		{
			trMasterData.Visible = true;
			//which options

		}
		else
		{
			trMasterData.Visible = false;
		}
	}

	protected void setReportsOptions(List<string> userRoles)
	{
		if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) >= 0
			|| userRoles.FindIndex(s => s.StartsWith("reports", StringComparison.CurrentCultureIgnoreCase)) >= 0)
		{
			trReports.Visible = true;
			//which options

		}
		else
		{
			trReports.Visible = false;
		}
	}

    protected void setMeetingOptions(List<string> userRoles)
    {
        if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) >= 0
            || userRoles.FindIndex(s => s.Contains("Meeting")) >= 0)
        {
            trMeetingModule.Visible = true;
        }
        else
        {
            trMeetingModule.Visible = false;
        }
    }

    protected void setWorkOptions(List<string> userRoles)
	{
		ListItem item = null;
		if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) >= 0
			//|| userRoles.FindIndex(s => s.Contains("WorkRequest")) >= 0
			//|| userRoles.FindIndex(s => s.Contains("SustainmentRequest")) >= 0
			|| userRoles.FindIndex(s => s.Contains("WorkItem")) >= 0
			|| userRoles.FindIndex(s => s.Contains("Task")) >= 0)
		{
			trWork.Visible = true;
            //trDeveloperReview.Visible = true;
            //trDailyReview.Visible = true;
        }
        else
		{
			trWork.Visible = false;
            trDeveloperReview.Visible = false;
			trDailyReview.Visible = false;
		}
	}

    protected void setAOROptions(List<string> userRoles)
    {
        ListItem li;

        if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) >= 0
            || userRoles.FindIndex(s => s.Contains("AOR")) >= 0
            || userRoles.FindIndex(s => s.Contains("Deployment")) >= 0
            || userRoles.FindIndex(s => s.Contains("CR")) >= 0)
        {
            trAoR.Visible = true;

            if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) == -1 && userRoles.FindIndex(s => s.Contains("AOR")) == -1)
            {
                li = lstAoR.Items.FindByText("AOR");
                lstAoR.Items.Remove(li);
            }
            if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) == -1 && userRoles.FindIndex(s => s.Contains("Deployment")) == -1)
            {
                li = lstAoR.Items.FindByText("Deployments");
                lstAoR.Items.Remove(li);

                li = lstAoR.Items.FindByText("Release Assessment");
                lstAoR.Items.Remove(li);
            }
            if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) == -1 && userRoles.FindIndex(s => s.Contains("CR")) == -1)
            {
                li = lstAoR.Items.FindByText("CR");
                lstAoR.Items.Remove(li);
            }
        }
        else
        {
            trAoR.Visible = false;
        }
    }

    protected void setRQMTOptions(List<string> userRoles)
    {
        //ListItem li;

        if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) >= 0
            || userRoles.FindIndex(s => s.Contains("RQMT")) >= 0)
        {
            trRQMT.Visible = true;

            //if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) == -1 && userRoles.FindIndex(s => s.Contains("RQMT")) == -1)
            //{
            //    li = lstRQMT.Items.FindByText("RQMT");
            //    lstRQMT.Items.Remove(li);
            //}

            //if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) == -1 && userRoles.FindIndex(s => s.Contains("RQMT")) == -1)
            //{
            //    li = lstRQMT.Items.FindByText("RQMT Description");
            //    lstRQMT.Items.Remove(li);
            //}
        }
        else
        {
            trRQMT.Visible = false;
        }
    }

    protected void setSROptions(List<string> userRoles)
    {
        if (userRoles.FindIndex(s => s.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)) >= 0
            || userRoles.FindIndex(s => s.Contains("SustainmentRequest")) >= 0)
        {
            trSR.Visible = true;
        }
        else
        {
            trSR.Visible = false;
        }
    }

    /// <summary>
    /// Load Default system filters for ALL users. 
    /// - Only include Active(new development) release versions and 1 Warranty release version
    /// </summary>
    /// <returns></returns>
    protected DataTable LoadDefaultFilters()
	{
		//get only Active and Warranty product versions
		DataTable dtPV = MasterData.ProductVersionList_Get(false);
		if (dtPV != null && dtPV.Rows.Count > 0)
		{
			dtPV.DefaultView.RowFilter = " Status LIKE 'Active%' OR Status LIKE 'Adoption%' OR Status LIKE 'Warranty%' ";
			dtPV = dtPV.DefaultView.ToTable();
		}

		return dtPV;
	}
	/// <summary>
	/// Load User Management filters for logged in user
	/// </summary>
	/// <returns></returns>
	protected DataTable LoadUserManagementFilters()
	{
		DataTable dtFilters = new DataTable();

		return dtFilters;
	}
	/// <summary>
	/// Load ALL applicable filters for logged in user. 
	/// - Includes system default filters 
	/// AND user's User Management filters
	/// </summary>
	protected void LoadFilters()
	{
		try
		{
			LoadSearchOptions();

			DataTable dtDefault = null; //LoadDefaultFilters();

			string tempFilters = string.Empty, appliedFilters = string.Empty;
			string filterType = "System Default", affiliationID = "0", affiliationName = "Default", roleID = "0", roleName = "Default";
			string filterLabel = "Release Version", fieldName = "ProductVersion", filterID = string.Empty, filterName = string.Empty;
			
			if (dtDefault != null)
			{
				foreach (DataRow row in dtDefault.Rows)
				{
					filterID = row["ProductVersionID"].ToString();
					filterName = row["ProductVersion"].ToString();
					tempFilters = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", filterType, affiliationID, affiliationName, roleID, roleName, filterLabel, fieldName, filterID, filterName);
					if (appliedFilters.Length == 0)
					{
						appliedFilters = tempFilters;
					}
					else
					{
						appliedFilters = string.Format("{0}`{1}", appliedFilters, tempFilters);
					}
				}
			}
			DefaultSystemFilters = appliedFilters;

			//TODO: get UM filters
			tempFilters = string.Empty;
			appliedFilters = string.Empty;
			filterType = "User Management";
			affiliationID = "1";
			affiliationName = "UserManagement";
			roleID = "1";
			roleName = "UserManagement";
			filterLabel = string.Empty;
			fieldName = string.Empty;
			filterID = string.Empty;
			filterName = string.Empty;

			DataTable dtUM = LoadUserManagementFilters();
			if (dtUM != null)
			{
				foreach (DataRow row in dtUM.Rows)
				{
					filterLabel = row["FilterLabel"].ToString();
					fieldName = row["FieldName"].ToString();
					filterID = row["FilterIDs"].ToString();
					filterName = row["FilterNames"].ToString();
					tempFilters = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", filterType, affiliationID, affiliationName, roleID, roleName, filterLabel, fieldName, filterID, filterName);
					if (appliedFilters.Length == 0)
					{
						appliedFilters = tempFilters;
					}
					else
					{
						appliedFilters = string.Format("{0}`{1}", appliedFilters, tempFilters);
					}
				}
			}
			WTSFilters = appliedFilters;

		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}

	protected static DataTable BuildCustomFilterTable(DataTable dtFilters)
	{
		try
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("CollectionName");
			dt.Columns.Add("FilterString");
			dt.AcceptChanges();

			if (dtFilters != null)
			{
				foreach (DataRow dRow in dtFilters.Rows)
				{
					DataRow nRow = GetCustomFilterRow(dt, dRow["CollectionName"].ToString());
					string filterTextName = dRow["FilterName"].ToString();
					string filterIDName = dRow["FilterName"].ToString();
					string filterParamID = filterTextName.ToUpper().Contains("CONTAINS") ? dRow["FilterText"].ToString() : dRow["FilterID"].ToString();
					string filterParamName = dRow["FilterText"].ToString();

					if (nRow[1] == "")
					{
						nRow[1] = filterTextName + "|" + filterIDName + "|" + filterParamID + "|" + filterParamName;
					}
					else
					{
						nRow[1] = nRow[1] + "`" + filterTextName + "|" + filterIDName + "|" + filterParamID + "|" + filterParamName;
					}
				}
			}

			return dt;
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			return null;
		}
	}
	protected static DataRow GetCustomFilterRow(DataTable dt, string strFilterName)
	{
		try
		{
			foreach (DataRow dRow in dt.Rows)
			{
				if (dRow[0].ToString() == strFilterName)
				{
					return dRow;
				}
			}
			return dt.Rows.Add(strFilterName, "");
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			return null;
		}
	}
	protected void LoadCustomUserFilters()
	{
		try
		{
			DataTable dtCustomFilter = Filtering.GetCustomFilters();
			dtCustomFilter = BuildCustomFilterTable(dtCustomFilter);

			lstCustomFilters.DataSource = dtCustomFilter;
			lstCustomFilters.DataValueField = "FilterString";
			lstCustomFilters.DataTextField = "CollectionName";
			lstCustomFilters.DataBind();
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}

	protected void LoadSearchOptions()
	{
		ListItem item = new ListItem("- Select -", "NA");
		item.Attributes.Add("type", "All");
		lstSearch.Items.Add(item);

		item = new ListItem("Primary Task Number");
		item.Attributes.Add("type", "Work");
		lstSearch.Items.Add(item);

		//item = new ListItem("Request Number");
		//item.Attributes.Add("type", "Work");
		//lstSearch.Items.Add(item);

		item = new ListItem("Item Title/Description");
		item.Attributes.Add("type", "Work");
		lstSearch.Items.Add(item);

		//item = new ListItem("Request");
		//item.Attributes.Add("type", "Work");
		//lstSearch.Items.Add(item);

		//item = new ListItem("Request Group");
		//item.Attributes.Add("type", "Work");
		//lstSearch.Items.Add(item);

        item = new ListItem("SR Number");
        item.Attributes.Add("type", "Work");
        lstSearch.Items.Add(item);
    }

	protected void LoadCopyRight()
	{
		lblCopyRight.Text = WTSConfiguration.CopyrightYear.ToString();
		lblContractorNm.Text = WTSConfiguration.CopyrightContractor.Trim();
	}


	#region Menus

	protected void LoadCVTMenu()
	{
		try
		{
			DataSet dsCVT = new DataSet();
			dsCVT.ReadXml(this.Server.MapPath("XML/WTS_CVT.xml"));
			if (dsCVT != null && dsCVT.Tables.Count > 0)
			{
				DataTable dt = dsCVT.Tables[0];
				ReformatMenuURLs(dt, "URL", "MenuItem_ID");

				if (dt != null)
				{
					foreach (DataRow dRow in dt.Rows)
					{
						dRow["ImageType"] = WTSCommon.GetImageTypeURL(dRow["ImageType"].ToString());
					}
				}

				menuCVT.DataSource = dt;
				menuCVT.DataValueField = "URL";
				menuCVT.DataTextField = "Text";
				menuCVT.DataIDField = "MenuItem_ID";
				menuCVT.DataParentIDField = "MenuItem_ID_0";
				menuCVT.DataImageField = "ImageType";
				menuCVT.DataBind();
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}

	protected void LoadHelpMenu()
	{
		try
		{
			DataSet dsHelp = new DataSet();
			dsHelp.ReadXml(this.Server.MapPath("XML/WTS_Help.xml"));
			if (dsHelp != null && dsHelp.Tables.Count > 0)
			{
				DataTable dt = dsHelp.Tables[0];
				ReformatMenuURLs(dt, "URL", "MenuItem_ID");

				if (dt != null)
				{
					foreach (DataRow dRow in dt.Rows)
					{
						dRow["ImageType"] = WTSCommon.GetImageTypeURL(dRow["ImageType"].ToString());
					}
				}
				
				menuHelp.DataSource = dt;
				menuHelp.DataValueField = "URL";
				menuHelp.DataTextField = "Text";
				menuHelp.DataIDField = "MenuItem_ID";
				menuHelp.DataParentIDField = "MenuItem_ID_0";
				menuHelp.DataImageField = "ImageType";
				menuHelp.DataBind();
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}

	protected void ReformatMenuURLs(DataTable dt, string strURLColName, string strIDColName)
	{
		string hostURL = WTSConfiguration.Host;

		foreach (DataRow rRow in dt.Rows)
		{
			if (rRow[strURLColName].ToString() != "")
			{
				string strURL = rRow[strURLColName].ToString();
				if (strURL.IndexOf('[') > -1 && strURL.IndexOf(']') > -1)
				{
					string webConfigKeyName = strURL.Substring(strURL.IndexOf('[') + 1, strURL.IndexOf(']') - 1);
					string webConfigValue = ConfigurationManager.AppSettings[webConfigKeyName];
					webConfigValue = webConfigValue.Replace(hostURL, "");
					if (strURL.IndexOf(']') + 1 != strURL.Length)
					{
						rRow[strURLColName] = RootUrl + "/" + webConfigValue + strURL.Substring(strURL.IndexOf(']') + 1);
					}
					else
					{
						rRow[strURLColName] = RootUrl + "/" + webConfigValue;
					}
				}
			}
		}
	}

    #endregion Menus

    [WebMethod(true)]
    public static string ClearViewDataSession()
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "getData", false.ToString() } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            HttpContext.Current.Session.Remove("Assigned");
            HttpContext.Current.Session.Remove("SelectedAssigned");
            HttpContext.Current.Session.Remove("SelectedAssigned1");
            HttpContext.Current.Session.Remove("dtWorkItem");
            saved = true;
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            saved = false;
            errorMsg = ex.Message;
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
	public static string ClearFilterSession(string module)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "getData", false.ToString() }, { "module", module } };
		bool saved = false;
		string errorMsg = string.Empty;

		try
		{
			HttpContext.Current.Session["filters_" + module] = null;
			saved = true;
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
			errorMsg = ex.Message;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod(true)]
	public static string SetFilterSession(bool getData, bool loadStartPage, string module, dynamic filters, bool myData)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "getData", getData.ToString() }, { "loadStartPage", loadStartPage.ToString() }, { "module", module }, { "error", "" } };
		bool saved = false;
		string errorMsg = string.Empty;

        string name = module;
        if (module == "DeveloperReview" || module == "DailyReview" || module == "AoR")
        {
            name = "Work";
        }

        string sessionid = HttpContext.Current.Session.SessionID;
		try
		{
			if (filters != null)
			{
				dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>(filters);
					
				if (module == "Work" || module == "DeveloperReview" || module == "DailyReview" || module == "AoR" || module == "RQMT")
				{
					saved = Filtering.SaveWorkFilters(module: module, filterModule: module, filters: fields, myData: myData, xml: "");
				}

                HttpContext.Current.Session["filters_" + name] = JsonConvert.DeserializeObject<Dictionary<string, object>>(filters);
			}
			else
			{
				saved = Filtering.SaveWorkFilters(module: module, filterModule: module, filters: null, myData: myData, xml: "");
				HttpContext.Current.Session["filters_" + name] = null;
			}

			saved = true;
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
			errorMsg = ex.Message;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod(true)]
	public static string SaveCustomFilter(string collectionName, bool deleteFilter, string module, dynamic filters)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "collectionName", collectionName }, { "customFilter", "" }, { "error", "" } };
		bool saved = false;
		string errorMsg = string.Empty;

		try
		{
			if (filters != null)
			{
				dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>(filters);

				saved = Filtering.SaveCustomFilters(collectionName: collectionName, deleteFilter: deleteFilter, module: module, filters: fields);
			}

			if (!deleteFilter)
			{
				DataTable dtCustomFilter = Filtering.GetCustomFilters(collectionName, module);
				dtCustomFilter = BuildCustomFilterTable(dtCustomFilter);
				result["collectionName"] = dtCustomFilter.Rows[0]["CollectionName"].ToString();
				result["customFilter"] = dtCustomFilter.Rows[0]["FilterString"].ToString();
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
			errorMsg = ex.Message;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
}