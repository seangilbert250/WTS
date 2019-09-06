﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;


using System.Web.SessionState;

/// <summary>
/// Summary description for UserManagement
/// </summary>
public sealed class UserManagement
{
    public enum ResourceType
    {
        Business_Analyst = 1,
        Programmer_Analyst = 2,
        Cyber_Team = 3,
        Not_People = 4
    }
    public enum Organization
	{
		Unauthorized = 1,
		Folsom_Dev = 2,
		Business_Team = 3,
		Executive = 4,
		RCS = 5,
		SIST = 6,
		View = 7
	}

	public enum PasswordResetRequestType
	{
		Username,
		EmailAddress
	}


	/// <summary>
	/// Adds the Default theme to a page
	/// </summary>
	public static void AddDefaultTheme(System.Web.UI.Page Page, WTS_User user)
	{
		if (user == null || string.IsNullOrWhiteSpace(user.DefaultTheme))
		{
			Page.Header.Controls.Add(new System.Web.UI.LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Page.ResolveUrl("Default") + "\" />"));
		}
		else
		{
			Page.Header.Controls.Add(new System.Web.UI.LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Page.ResolveUrl(user.DefaultTheme) + "\" />"));
		}
	}

	/// <summary>
	/// Get name of Theme that is saved for the specified user
	/// - If user is not specified then theme will be retrieve for the currently logged in user
	/// - If no user is found then the default them will be returned
	/// </summary>
	/// <param name="username">username to get theme for</param>
	/// <returns>name of saved Theme</returns>
	public static string GetUserTheme(string username = "")
	{
		string themeName = "Default", defaultTheme = "Default";

		#region Get Username from MembershipUser if necessary
		if (string.IsNullOrWhiteSpace(username))
		{
			MembershipUser mu = Membership.GetUser();
			if (mu == null)
			{
				return defaultTheme;
			}
			else
			{
				username = mu.UserName;
			}
		}
		#endregion

		string funcName = "SELECT dbo.GetUserThemeName(@username)";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(funcName, cn))
			{
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@username", username);

				try
				{
					string name = cmd.ExecuteScalar().ToString();
					if (string.IsNullOrWhiteSpace(name))
					{
						themeName = defaultTheme;
					}
					else
					{
						themeName = name;
					}
				}
				catch (Exception ex)
				{ //log exception
					LogUtility.LogException(ex);
					themeName = defaultTheme;
				}
			}
		}

		return themeName;
	}

	/// <summary>
	/// Load list of available themes
	/// </summary>
	/// <param name="includeArchive">Include Archived themes</param>
	/// <returns>List of Theme records</returns>
	public static DataTable LoadThemeList(bool includeArchive = false)
	{
		string procName = "THEMELIST_GET";

		using (DataTable dt = new DataTable("THEME"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Decimal).Value = includeArchive ? 1 : 0;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						throw;
					}
				}
			}
		}
	}

	/// <summary>
	/// Gets the user's "UserName"
	/// </summary>
	public static string UserName
	{
		get
		{
			return HttpContext.Current.User.Identity.Name;
		}
	}

	public static DataTable GET_USER_ROLES(int userNMID, int websysID)
	{
		return null;
		try
		{
			DataTable dt = new DataTable();
			
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			return null;
		}
	}

	public static void LoadLicenses()
	{
		try
		{
			Aspose.Cells.License license = new Aspose.Cells.License();
			license.SetLicense("Aspose.Cells.licx");
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}

	/// <summary>
	/// Load list of Organizations available in the system
	/// - developer type should be excluded when queried by non-developer users
	/// </summary>
	/// <param name="excludeDeveloper">exclude developer Organization</param>
	/// <param name="includeArchive">include archived Organizations</param>
	/// <returns>list of Organization records</returns>
	public static DataTable LoadOrganizationList(bool includeArchive = false)
	{
		string procName = "ORGANIZATIONLIST_GET";

		using (DataTable dt = new DataTable("ORGANIZATION"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@ShowArchived", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						throw;
					}
				}
			}
		}
	}

    public static DataTable LoadResourceTypeList(bool includeArchive = false)
    {
        string procName = "RESOURCE_TYPELIST_GET";

        using (DataTable dt = new DataTable("RESOURCE_TYPE"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtility.LogException(ex);
                        throw;
                    }
                }
            }
        }
    }


    #region System profiles(not membership profiles)

    /// <summary>
    /// Get logged in user's system UserID from username
    /// </summary>
    /// <param name="username">if tempty use current session's username</param>
    /// <returns></returns>
    public static int GetUserId_FromUsername(string username = "")
	{
		int id = 0;
		if(string.IsNullOrWhiteSpace(username))
		{
			username = HttpContext.Current.User.Identity.Name;
		}

		MembershipUser mu = GetMembershipUser(email: "", username: username);
		if (mu != null)
		{
			WTS_User u = new WTS_User(membership_UserId: mu.ProviderUserKey.ToString());
			u.Load_GUID();
			id = u.ID;
		}

		return id;
	}

	/// <summary>
	/// Load User Profile record based on MembershipUser ID
	/// - ID string must be a valid GUID
	/// </summary>
	/// <param name="membershipUserId">MembershipUser ID(GUID)</param>
	/// <returns>User Profile ID</returns>
	public static int GetUserId(string membershipUserId)
	{
		WTS_User u = new WTS_User(membershipUserId);
		u.Load_GUID();

		return u.ID;
	}

	/// <summary>
	/// Search for User Profile record based on corresponding MembershipUser username value
	/// </summary>
	/// <param name="username">username to match</param>
	/// <returns>true if profile exists</returns>
	public static bool ProfileUserExists(string username)
	{
		string funcName = "SELECT dbo.UsernameExists(@username)";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(funcName, cn))
			{
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@username", username);

				try
				{
					return (bool)cmd.ExecuteScalar();
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					throw;
				}
			}
		}
	}

	/// <summary>
	/// Find all users that have a profile that do not have a corresponding membershipUser record
	/// - This should NOT return any records based on system design and implemented business rules
	/// </summary>
	/// <param name="userName">username search string</param>
	/// <param name="email">email search string</param>
	/// <returns>list of User Profile records without corresponding MembershipUser records</returns>
	public static DataTable FindUnregisteredUsers(string userName = "", string email = "")
	{
		string procName = "Find_Unregistered_Users";

		using (DataTable dt = new DataTable("USER"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 50).Value = userName;
					cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = email;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						throw;
					}
				}
			}
		}
	}

	/// <summary>
	/// Load list of user profile records
	/// - will contain some membership values as well
    /// - contains AOR Enterprise allocation %
    /// - [TODO] contain SYSTEM Enterprise allocation % ?
	/// </summary>
	/// <param name="organizationId">filter by Organization ID</param>
	/// <param name="loadArchived">include Archived users</param>
	/// <param name="userNameSearch">search for users containing specified search string</param>
	/// <returns>list of user profile records</returns>
	public static DataTable LoadUserList(int organizationId = 0, bool excludeDeveloper = true, bool loadArchived = false, string userNameSearch = "", bool excludeNotPeople = false, int AORReleaseID = 0)
	{
		string procName = "WTS_RESOURCELIST_GET";

		using (DataTable dt = new DataTable("USER"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@OrganizationID", SqlDbType.Int).Value = organizationId == 0 ? (object)DBNull.Value : organizationId;
					cmd.Parameters.Add("@LoadArchived", SqlDbType.Bit).Value = loadArchived ? 1 : 0;
					cmd.Parameters.Add("@UserNameSearch", SqlDbType.NVarChar, 255).Value = userNameSearch.Trim();
                    cmd.Parameters.Add("@ExcludeNotPeople", SqlDbType.Bit).Value = excludeNotPeople;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;


                    try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);

								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						throw;
					}
				}
			}
		}
	}

	/// <summary>
	/// Get default system filters based on configured settings
	/// </summary>
	/// <returns></returns>
	public static DataTable LoadDefaultFilters()
	{
		DataTable dtFilters = new DataTable();



		return dtFilters;
	}
	/// <summary>
	/// Get list of Available Filters
	/// </summary>
	/// <returns>DataTable containing list of Filters</returns>
	public static DataSet LOAD_ROLE_FILTERS()
	{
		return new DataSet();
	}

	#endregion


	/// <summary>
	/// Return MembershipUser object based on either email address or username
	/// - email address and username are both unique per user
	/// </summary>
	/// <param name="email">user's email address</param>
	/// <param name="username">user's username</param>
	/// <returns>MembershipUser object</returns>
	public static MembershipUser GetMembershipUser(string email = "", string username = "")
	{
		MembershipUserCollection muc = Membership.FindUsersByEmail(email);
		MembershipUser mu = null;
		if (muc == null || muc.Count == 0)
		{
			muc = Membership.FindUsersByName(username);
		}

		if (muc == null || muc.Count == 0)
		{
			mu = null;
		}
		else
		{
			foreach (MembershipUser tempMU in muc)
			{
				mu = tempMU;
				break; //only need first because username should be unique in membership collection
			}
		}

		return mu;
	}

	/// <summary>
	/// Retrieve all users that have explicitly been assigned the User Admin role
	/// - Admin role assumes User Admin role
	/// - this does not check for roles inherint to Organization
	/// </summary>
	/// <returns>datatable of user records with User Admin role</returns>
	public static DataTable GetUserAdminList()
	{
		//get all users with User Admin role
		DataTable dtHasRole = LoadUserList(organizationId: 0, excludeDeveloper: true, loadArchived: false);
		StringBuilder rowFilter = new StringBuilder();
		//filter to ALL admin and resourcemanagement roles
		rowFilter.Append(" (");
		rowFilter.Append(" Roles LIKE '%Admin%' ");
		rowFilter.Append(" OR Roles LIKE '%ResourceManagement%' ");
		rowFilter.Append(") ");
		//filter out view only roles
		rowFilter.Append(" AND Roles NOT LIKE 'View:%' ");

		if (dtHasRole.Rows.Count > 0)
		{
			dtHasRole.DefaultView.RowFilter = rowFilter.ToString();
			dtHasRole = dtHasRole.DefaultView.ToTable();
		}

		return dtHasRole;
	}


	#region Roles

	/// <summary>
	/// Retrieve list of roles assigned to the currently logged in user
	/// - only gets membership roles. Does not return roles inherint to Organization
	/// </summary>
	/// <returns>List of roles that the user has been assigned</returns>
	public static string[] GetUserRoles_Membership()
	{
		return Roles.GetRolesForUser();
	}
	/// <summary>
	/// Retrieve list of roles assigned to the specified
	/// - only gets membership roles. Does not return roles inherint to Organization
	/// </summary>
	/// <param name="username">username to get roles for</param>
	/// <returns>List of roles that the user has been assigned</returns>
	public static string[] GetUserRoles_Membership(string username)
	{
		return Roles.GetRolesForUser(username);
	}

	/// <summary>
	/// Get all roles for logged in user.  
	/// Includes roles assigned through membership profile 
	/// and roles assumed based on Organization
	/// </summary>
	/// <returns>List of roles for user</returns>
	public static List<string> GetUserRoles_All()
	{
		if (Membership.GetUser() == null) { return new List<string>(); }

		return GetUserRoles_All(Membership.GetUser().UserName);
	}
	/// <summary>
	/// Get all roles for specified user.  
	/// Includes roles assigned through membership profile 
	/// and roles assumed based on Organization
	/// </summary>
	/// <param name="username">membership and profile username</param>
	/// <returns>List of roles for user</returns>
	public static List<string> GetUserRoles_All(string username)
	{
		MembershipUser mu = Membership.GetUser(username);
		if (mu == null) { return new List<string>(); }

		WTS_User u = new WTS_User(mu.ProviderUserKey.ToString());
		u.Load_GUID();

		if (u == null) { return new List<string>(); }

		List<string> userRoles = new List<string>();

		switch ((UserManagement.Organization)u.OrganizationID)
		{
			case UserManagement.Organization.Unauthorized:
				return new List<string>();
			case UserManagement.Organization.Folsom_Dev:
			//case UserManagement.Organization.Business_Team:
			//	//developer and admin Organizations get all roles
			//	//TODO: allow dev or admin to have roles removed?
			//	string[] allRoles = Roles.GetAllRoles();
			//	foreach (string r in allRoles)
			//	{
			//		userRoles.Add(r);
			//	}
			//	break;
			default:
				//for users that are not developer or admin Organization, load individually assigned roles
				string[] assignedRoles = Roles.GetRolesForUser(username);
				foreach (string r in assignedRoles)
				{
					if (userRoles.Contains(r)) { continue; }

					userRoles.Add(r);
				}
				break;
		}

		return userRoles;
	}

	/// <summary>
	/// Check if user is a User Administrator
	/// - with no profileId the system will only check membership roles
	/// </summary>
	/// <param name="userProfileId"></param>
	/// <returns></returns>
	public static bool UserIsUserAdmin(int userProfileId = 0)
	{
		string username = string.Empty;
		MembershipUser mu = Membership.GetUser();
		if (mu != null)
		{
			username = mu.UserName;
		}

		return UserIsUserAdmin(username, userProfileId);
	}
	/// <summary>
	/// Check if user is a User Administrator
	/// - with no profileId the system will only check membership roles
	/// </summary>
	/// <param name="username"></param>
	/// <param name="userProfileId"></param>
	/// <returns></returns>
	public static bool UserIsUserAdmin(string username, int userProfileId = 0)
	{
		//check user roles
		if (Roles.IsUserInRole(username, "Admin")
			|| Roles.IsUserInRole(username, "Administration")
			|| Roles.IsUserInRole(username, "ResourceManagement"))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	/// <summary>
	/// Verify if user has specified role
	/// - only checks membership roles. Does not verify based on Organization's inherint roles
	/// - if no username is specified this will check for the currently logged in user
	/// </summary>
	/// <param name="roleName">role to check for</param>
	/// <param name="username">username to verify</param>
	/// <returns>true if user has specified role</returns>
	public static bool UserIsInRole(string roleName, string username = "")
	{
		if (string.IsNullOrWhiteSpace(username))
		{
			return Roles.IsUserInRole(roleName);
		}
		else
		{
			return Roles.IsUserInRole(username, roleName);
		}
	}

	public static bool UserCanEdit(WTSModuleOption option, string username = "")
	{
		bool canEdit = false;

		List<string> roles = GetUserRoles_All();
		if(roles.Contains("Admin"))
		{
			return true;
		}

		switch (option)
		{
			case WTSModuleOption.WorkRequest:
				if (roles.Contains("WorkRequest"))
				{
					canEdit = true;
				}
				break;
			case WTSModuleOption.WorkItem:
				if (roles.Contains("WorkItem"))
				{
					canEdit = true;
				}
				break;
			case WTSModuleOption.WorkItemTask:
				if (roles.Contains("WorkItem") || roles.Contains("Task"))
				{
					canEdit = true;
				}
				break;
			case WTSModuleOption.SustainmentRequest:
				if (roles.Contains("SustainmentRequest"))
				{
					canEdit = true;
				}
				break;
			case WTSModuleOption.AORReport:
				if(roles.Contains("Reports") 
					&& (roles.Contains("ResourceManagement") || roles.Contains("Administration")))
				{
					canEdit = true;
				}
				break;
			case WTSModuleOption.MasterData:
				if (roles.Contains("MasterData"))
				{
					canEdit = true;
				}
				break;
			case WTSModuleOption.ResourceAdmin:
			case WTSModuleOption.OrganizationAdmin:
				if (roles.Contains("ResourceManagement") || roles.Contains("Administration"))
				{
					canEdit = true;
				}
				break;
			case WTSModuleOption.Dashboard:
				if (roles.Contains("ResourceManagement") || roles.Contains("Dashboard"))
				{
					canEdit = true;
				}
				break;
			case WTSModuleOption.News:
				if (roles.Contains("News") || roles.Contains("Administration"))
				{
					canEdit = true;
				}
				break;
            case WTSModuleOption.AOR:
                if (roles.Contains("AOR"))
                {
                    canEdit = true;
                }
                break;
            case WTSModuleOption.Meeting:
                if (roles.Contains("Meeting"))
                {
                    canEdit = true;
                }
                break;
            case WTSModuleOption.CR:
                if (roles.Contains("CR"))
                {
                    canEdit = true;
                }
                break;
            case WTSModuleOption.RQMT:
                if (roles.Contains("RQMT"))
                {
                    canEdit = true;
                }
                break;
            case WTSModuleOption.Deployment:
                if (roles.Contains("Deployment"))
                {
                    canEdit = true;
                }
                break;
            case WTSModuleOption.WorkloadMGMT:
                if (roles.Contains("Workload MGMT"))
                {
                    canEdit = true;
                }
                break;
            default:
				canEdit = false;
				break;
		}

		return canEdit;
	}

	public static bool UserCanView(WTSModuleOption option, string username = "")
	{
		bool canView = false;

		List<string> roles = GetUserRoles_All();
		if (roles.Contains("Admin"))
		{
			return true;
		}

		switch (option)
		{
			case WTSModuleOption.WorkRequest:
				if (roles.Contains("WorkRequest") 
					|| roles.Contains("View:WorkRequest"))
				{
					canView = true;
				}
				break;
			case WTSModuleOption.WorkItem:
				if (roles.Contains("WorkItem") 
					|| roles.Contains("View:WorkItem"))
				{
					canView = true;
				}
				break;
			case WTSModuleOption.WorkItemTask:
				if (roles.Contains("WorkItem") || roles.Contains("Task")
					|| roles.Contains("View:WorkItem") || roles.Contains("View:Task"))
				{
					canView = true;
				}
				break;
			case WTSModuleOption.SustainmentRequest:
				if (roles.Contains("SustainmentRequest") 
					|| roles.Contains("View:SustainmentRequest"))
				{
					canView = true;
				}
				break;
			case WTSModuleOption.AORReport:
				if (roles.Contains("Reports") || roles.Contains("View-Reports")
					&& (
						roles.Contains("ResourceManagement") || roles.Contains("Administration")
						|| roles.Contains("View:ResourceManagement") || roles.Contains("View:Administration"))
					)
				{
					canView = true;
				}
				break;
			case WTSModuleOption.MasterData:
				if (roles.Contains("MasterData") 
					|| roles.Contains("View:MasterData"))
				{
					canView = true;
				}
				break;
			case WTSModuleOption.ResourceAdmin:
			case WTSModuleOption.OrganizationAdmin:
				if (roles.Contains("ResourceManagement") || roles.Contains("Administration") 
					|| roles.Contains("View:ResourceManagement") || roles.Contains("View:Administration"))
				{
					canView = true;
				}
				break;
			case WTSModuleOption.Dashboard:
				if (roles.Contains("Dashboard") 
					|| roles.Contains("View:Dashboard"))
				{
					canView = true;
				}
				break;
			case WTSModuleOption.News:
				if (roles.Contains("News") || roles.Contains("Administration") 
					|| roles.Contains("View:News"))
				{
					canView = true;
				}
				break;
            case WTSModuleOption.AOR:
                if (roles.Contains("AOR") || roles.Contains("View:AOR"))
                {
                    canView = true;
                }
                break;
            case WTSModuleOption.Meeting:
                if (roles.Contains("Meeting") || roles.Contains("View:Meeting"))
                {
                    canView = true;
                }
                break;
            case WTSModuleOption.CR:
                if (roles.Contains("CR") || roles.Contains("View:CR"))
                {
                    canView = true;
                }
                break;
            case WTSModuleOption.RQMT:
                if (roles.Contains("RQMT") || roles.Contains("View:RQMT"))
                {
                    canView = true;
                }
                break;
            case WTSModuleOption.Deployment:
                if (roles.Contains("Deployment") || roles.Contains("View:Deployment"))
                {
                    canView = true;
                }
                break;
            default:
				canView = false;
				break;
		}

		return canView;
	}

	#endregion


	#region Password reset and generation

	/// <summary>
	/// Get list of available security questions
	/// </summary>
	/// <returns>datatable with ids and questions</returns>
	public static DataTable GetPasswordQuestions()
	{
		string commandText = "SELECT PASSWORD_QUESTIONID, PASSWORD_QUESTION FROM PASSWORD_QUESTION";

		using (DataTable dt = new DataTable("QUESTION"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(commandText, cn))
				{
					cmd.CommandType = CommandType.Text;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						throw;
					}
				}
			}
		}
	}

	/// <summary>
	/// Get details of password reset request record
	/// - includes expiration date and status
	/// - user id and reset request id must be linked in database
	/// </summary>
	/// <param name="resetCode">reset request id</param>
	/// <param name="userId">user id</param>
	/// <returns>Datatable of reset request records</returns>
	public static DataTable GetPasswordResetRequest(Guid resetCode, Guid userId)
	{
		string procName = "PasswordResetRequest_Load";

		using (DataTable dt = new DataTable("RESET_REQUEST"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@resetcode", SqlDbType.UniqueIdentifier).Value = resetCode;
					cmd.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = userId;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						throw;
					}
				}
			}
		}
	}

	/// <summary>
	/// Delete Password Reset Request from database
	/// </summary>
	/// <param name="userId">User Id for which to delete requests</param>
	/// <returns>true if delete was successful</returns>
	public static bool ClearPasswordResetRequests(Guid userId)
	{
		int cleared = 0;

		try
		{
			string procName = "PasswordResetRequest_Clear";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier).Value = userId;

					SqlParameter paramSaved = new SqlParameter("@cleared", SqlDbType.Decimal);
					paramSaved.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(paramSaved);

					cmd.ExecuteNonQuery();

					cleared = paramSaved.Value == DBNull.Value ? 0 : int.Parse(paramSaved.Value.ToString());
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			cleared = 0;
		}
		return (cleared > 0);
	}

	/// <summary>
	/// Send email with password reset link and instructions
	/// </summary>
	/// <param name="email">email address</param>
	/// <param name="username">system username</param>
	/// <param name="resetCode">reset code needed for link</param>
	/// <param name="userId">system user id</param>
	/// <param name="newUser">flag if email is for new user</param>
	/// <returns>true if sent successfully</returns>
	public static bool SendResetEmail(string email, string username, Guid resetCode, Guid userId, bool newUser = false)
	{
		bool sent = false;

		try
		{
			string subject = string.Empty;
			string msgBody = string.Empty;
			if (newUser)
			{
				subject = string.Format("New {0} account created - password instructions", WTSConfiguration.AppName);
				msgBody = GetNewAccountEmailtext(resetCode.ToString(), userId.ToString());
			}
			else
			{
				subject = string.Format("{0} Password Reset Request", WTSConfiguration.AppName);
				msgBody = GetPasswordResetEmailText(resetCode.ToString(), userId.ToString());
			}

			sent = WTSUtility.Send_Email(email, username, subject, msgBody);
		}
		catch (Exception ex)
		{
			sent = false;
			LogUtility.LogException(ex);
		}

		return sent;
	}

	/// <summary>
	/// Send email to user administrators notifying of newly registered user
	/// </summary>
	/// <param name="username">new username</param>
	/// <param name="firstName">new user's first name</param>
	/// <param name="lastName">new user's last name</param>
	/// <returns>true if email sent successfully</returns>
	public static bool SendRegistrationNotificationEmail(string username, string firstName, string lastName)
	{
		bool sent = false;

		string fromAddress = "", fromName = "";
		WTSConfiguration.GetEmailFromAddress(out fromAddress, out fromName);
		Dictionary<string, string> toAddresses = new Dictionary<string, string>();
		Dictionary<string, string> ccAddresses = WTSConfiguration.LoadEmailAddressList((int)WTSConfiguration.ConfigSetting_Type.RegistrationEmailTo);
		DataTable dt = GetUserAdminList();
		foreach (DataRow row in dt.Rows)
		{
			toAddresses.Add(row["Email"].ToString(), string.Format("{0} {1}", row["First_Name"].ToString(), row["Last_Name"].ToString()));
		}
		if (toAddresses.Count == 0)
		{
			toAddresses = WTSConfiguration.LoadEmailAddressList((int)WTSConfiguration.ConfigSetting_Type.RegistrationEmailTo);
		}
		Dictionary<string, string> bccAddresses = WTSConfiguration.LoadEmailAddressList((int)WTSConfiguration.ConfigSetting_Type.RegistrationEmailBcc);

		if (!string.IsNullOrWhiteSpace(fromAddress)
			&& toAddresses.Count > 0)
		{
			fromName = string.IsNullOrWhiteSpace(fromName) ? fromAddress : fromName.Trim();
			//send the email
			try
			{
				string subject = string.Format("New user has requested access - {0}, {1}", lastName, firstName);
				string msgBody = GetRegistrationNotificationEmailText(username, firstName, lastName);

				sent = WTSUtility.Send_Email(toAddresses, ccAddresses, bccAddresses, subject, msgBody, fromAddress, fromName, true, System.Net.Mail.MailPriority.Normal);
			}
			catch (Exception ex)
			{
				LogUtility.LogException(ex);
				sent = false;
			}
		}

		return sent;
	}

	/// <summary>
	/// Get email body text for password reset email
	/// </summary>
	/// <param name="resetCode">unique code to use in reset link</param>
	/// <param name="userId">user id to use in reset link</param>
	/// <returns>string containing email body text</returns>
	public static string GetPasswordResetEmailText(string resetCode, string userId)
	{
		string resetUrl = string.Format(@"{0}/{1}/Account/Reset.aspx", WTSConfiguration.Host.ToLower(), WTSConfiguration.AppName);

		StringBuilder sbMsgText = new StringBuilder();
		sbMsgText.AppendFormat("A Password Reset Request was submitted for your {0} account.", WTSConfiguration.AppTitleAbbreviation);
		sbMsgText.AppendLine();
		sbMsgText.AppendLine();
		sbMsgText.AppendLine("If you did not request a password reset then please contact your system administrator.");
		sbMsgText.AppendLine();
		sbMsgText.AppendLine("Your password will not be changed unless you follow the secure link below.");
		sbMsgText.AppendLine();
		sbMsgText.AppendLine();
		sbMsgText.Append("To complete your password change request, go to ");
		sbMsgText.AppendFormat("{0}?resetCode={1}&userId={2}", resetUrl, resetCode, userId);
		sbMsgText.Append(" by either clicking the secure link or pasting the Url into your browser.");
		sbMsgText.AppendLine();
		sbMsgText.AppendLine();
		sbMsgText.AppendLine();
		sbMsgText.AppendFormat("The password reset code will expire {0} minutes after the time when the request was submitted.", WTSConfiguration.PasswordResetExpiration.ToString());
		sbMsgText.AppendLine();

		return sbMsgText.ToString();
	}

	/// <summary>
	/// Get email body text for new account password creation email
	/// </summary>
	/// <param name="resetCode">unique code to use in reset link</param>
	/// <param name="userId">user id to use in reset link</param>
	/// <returns>string containing email body text</returns>
	public static string GetNewAccountEmailtext(string resetCode, string userId)
	{
		string resetUrl = string.Format(@"{0}/{1}/Account/Reset.aspx", WTSConfiguration.Host.ToLower(), WTSConfiguration.AppName);
		StringBuilder sbMsgText = new StringBuilder();
		sbMsgText.AppendLine("Welcome!");
		sbMsgText.AppendLine();
		sbMsgText.AppendFormat("A new {0} account has been created for you.", WTSConfiguration.AppTitleAbbreviation);
		sbMsgText.AppendLine();
		sbMsgText.AppendLine();
		sbMsgText.Append("To set your password and complete the registration process, go to ");
		sbMsgText.AppendFormat("{0}?resetCode={1}&userId={2}", resetUrl, resetCode, userId);
		sbMsgText.Append(" by either clicking the secure link or pasting the Url into your browser.");
		sbMsgText.AppendLine();
		sbMsgText.AppendLine();
		sbMsgText.AppendLine();
		sbMsgText.AppendFormat("The password reset code will expire {0} minutes after the time when the request was submitted.", WTSConfiguration.PasswordResetExpiration.ToString());
		sbMsgText.AppendLine();


		return sbMsgText.ToString();
	}

	/// <summary>
	/// Get email body text for new account registration notRuby
	/// </summary>
	/// <param name="username">username of new user account</param>
	/// <param name="firstName">First name of new user</param>
	/// <param name="lastName">Last name of new user</param>
	/// <returns>string containing email body text</returns>
	public static string GetRegistrationNotificationEmailText(string username, string firstName, string lastName)
	{
		StringBuilder sbMsgText = new StringBuilder();
		sbMsgText.AppendLine(string.Format("A new user [{0} {1} ({2})] has requested access.", firstName.Trim(), lastName.Trim(), username.Trim()));
		sbMsgText.AppendLine();
		sbMsgText.AppendLine("The user must be approved and have roles granted prior to using the system");
		sbMsgText.AppendLine();
		sbMsgText.AppendLine();

		return sbMsgText.ToString();
	}
	#endregion
    
}


/// <summary>
/// User Profile containing user properties and C.R.U.D. methods
/// </summary>
public class WTS_User
{
	#region Properties

	/// <summary>
	/// User ID in database
	/// </summary>
	public int ID { get; set; }
	/// <summary>
	/// user id from membership "system"
	/// </summary>
	public string Membership_UserID { get; set; }
	/// <summary>
	/// user is an approvied aspnet user
	/// </summary>
	public bool IsApproved { get; set; }
	/// <summary>
	/// user has been locked out of system
	/// </summary>
	public bool IsLocked { get; set; }
    /// <summary>
	/// ID of Resource Type in database
	/// </summary>
	public int ResourceTypeID { get; set; }
    /// <summary>
    /// Resource Type 
    /// Program Analyst, Business Analyst, Cyber Team
    /// </summary>
    public string ResourceType { get; private set; }
    /// <summary>
    /// ID of Organization in database
    /// </summary>
    public int OrganizationID { get; set; }
	/// <summary>
	/// Organization name
	/// Admin, Developer, Business Team, Executive, etc.
	/// </summary>
	public string Organization { get; private set; }
	/// <summary>
	/// system username
	/// </summary>
	public string Username { get; set; }
	/// <summary>
	/// Id of css theme for system display
	/// </summary>
	public int ThemeId { get; set; }
	/// <summary>
	/// CSS Theme name for system display
	/// </summary>
	public string Theme { get; private set; }
	/// <summary>
	/// Option for enabling/disabling page animations such as sliding
	/// </summary>
	public bool EnableAnimations { get; set; }
	/// <summary>
	/// First Name
	/// </summary>
	public string First_Name { get; set; }
	/// <summary>
	/// Last Name
	/// </summary>
	public string Last_Name { get; set; }
	/// <summary>
	/// Middle Name
	/// </summary>
	public string Middle_Name { get; set; }
	/// <summary>
	/// Name Prefix
	/// </summary>
	public string Prefix { get; set; }
	/// <summary>
	/// Name Suffix
	/// </summary>
	public string Suffix { get; set; }
	/// <summary>
	/// OffRuby phone number
	/// </summary>
	public string Phone_Office { get; set; }
	/// <summary>
	/// Mobile phone number
	/// </summary>
	public string Phone_Mobile { get; set; }
	/// <summary>
	/// Miscellaneous phone number
	/// </summary>
	public string Phone_Misc { get; set; }
	/// <summary>
	/// Fax number
	/// </summary>
	public string Fax { get; set; }
	/// <summary>
	/// Primary email address
	/// </summary>
	public string Email { get; set; }
	/// <summary>
	/// 2nd Email address
	/// </summary>
	public string Email2 { get; set; }
	/// <summary>
	/// Street Address
	/// </summary>
	public string Address { get; set; }
	/// <summary>
	/// Street Address part 2
	/// </summary>
	public string Address2 { get; set; }
	public string City { get; set; }
	/// <summary>
	/// Address State
	/// </summary>
	public string State { get; set; }
	/// <summary>
	/// Address Country
	/// </summary>
	public string Country { get; set; }
	/// <summary>
	/// Address Postal Code
	/// </summary>
	public string PostalCode { get; set; }
	/// <summary>
	/// Notes about user
	/// </summary>
	public string Notes { get; set; }
	/// <summary>
	/// Bit flags for user's on/off attribute values
	/// </summary>
	public string AttributeFlags { get; set; }
    /// <summary>
    /// flag if user is archived
    /// </summary>
	public string DomainName { get; set; }
    /// <summary>
    /// flag if user is archived
    /// </summary>
    public bool Archive { get; set; }

    public bool ReceiveSREMail { get; set; }
    public bool IncludeInSRCounts { get; set; }

    public bool IsDeveloper { get; set; }
    public bool IsBusAnalyst { get; set; }
    public bool IsAMCGEO { get; set; }
    public bool IsCASUser { get; set; }
    public bool IsALODUser { get; set; }

    public string DefaultTheme
	{
		get
		{
			if (EnableAnimations)
			{
				return System.Web.Configuration.WebConfigurationManager.AppSettings["AnimationsTheme"];
			}
			else
			{
				return System.Web.Configuration.WebConfigurationManager.AppSettings["NoAnimationsTheme"];
			}
		}
	}

    public bool AORResourceTeam { get; set; }
	#endregion

	#region Constructors
	public WTS_User() { }
	public WTS_User(int id)
	{
		this.ID = id;
	}
	public WTS_User(string membership_UserId)
	{
		this.Membership_UserID = membership_UserId;
	}
	#endregion

	/// <summary>
	/// Add new user to the database with assigned values
	/// </summary>
	/// <param name="saved">flag if save was successful</param>
	/// <param name="errorMsg">any relevant error messages</param>
	/// <returns>new user id</returns>
	public int Add(out bool saved, out string errorMsg)
	{
		saved = false; errorMsg = string.Empty;
		SqlParameter newID = null;

		try
		{
			string procName = "WTS_RESOURCE_ADD";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ResourceTypeID", SqlDbType.Decimal).Value = this.ResourceTypeID == 0 ? (int)UserManagement.ResourceType.Programmer_Analyst : this.ResourceTypeID;
                    cmd.Parameters.Add("@OrganizationID", SqlDbType.Decimal).Value = this.OrganizationID == 0 ? (int)UserManagement.Organization.Unauthorized : this.OrganizationID;
					cmd.Parameters.Add("@Membership_UserID", SqlDbType.UniqueIdentifier).Value = string.IsNullOrWhiteSpace(this.Membership_UserID) ? Guid.Empty : new Guid(this.Membership_UserID);
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = this.Username;
					cmd.Parameters.Add("@ThemeId", SqlDbType.Decimal).Value = this.ThemeId == 0 ? 1 : this.ThemeId;
					cmd.Parameters.Add("@EnableAnimations", SqlDbType.NVarChar).Value = this.EnableAnimations ? 1 : 0;
					cmd.Parameters.Add("@First_Name", SqlDbType.NVarChar).Value = this.First_Name;
					cmd.Parameters.Add("@Last_Name", SqlDbType.NVarChar).Value = this.Last_Name;
					cmd.Parameters.Add("@Middle_Name", SqlDbType.NVarChar).Value = this.Middle_Name;
					cmd.Parameters.Add("@Prefix", SqlDbType.NVarChar).Value = this.Prefix;
					cmd.Parameters.Add("@Suffix", SqlDbType.NVarChar).Value = this.Suffix;
					cmd.Parameters.Add("@Phone_Office", SqlDbType.NVarChar).Value = this.Phone_Office;
					cmd.Parameters.Add("@Phone_Mobile", SqlDbType.NVarChar).Value = this.Phone_Mobile;
					cmd.Parameters.Add("@Phone_Misc", SqlDbType.NVarChar).Value = this.Phone_Misc;
					cmd.Parameters.Add("@Fax", SqlDbType.NVarChar).Value = this.Fax;
					cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = this.Email;
					cmd.Parameters.Add("@Email2", SqlDbType.NVarChar).Value = this.Email2;
					cmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = this.Address;
					cmd.Parameters.Add("@Address2", SqlDbType.NVarChar).Value = this.Address2;
					cmd.Parameters.Add("@City", SqlDbType.NVarChar).Value = this.City;
					cmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = this.State;
					cmd.Parameters.Add("@Country", SqlDbType.NVarChar).Value = this.Country;
					cmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar).Value = this.PostalCode;
					cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = this.Notes;
					cmd.Parameters.Add("@AttributeFlags", SqlDbType.NVarChar).Value = this.AttributeFlags;
					cmd.Parameters.Add("@Archive", SqlDbType.NVarChar).Value = this.Archive ? 1 : 0;
                    cmd.Parameters.Add("@ReceiveSREMail", SqlDbType.NVarChar).Value = this.ReceiveSREMail ? 1 : 0;
                    cmd.Parameters.Add("@IncludeInSRCounts", SqlDbType.NVarChar).Value = this.IncludeInSRCounts ? 1 : 0;


                    cmd.Parameters.Add("@IsDeveloper", SqlDbType.NVarChar).Value = this.IsDeveloper ? 1 : 0;
                    cmd.Parameters.Add("@IsBusAnalyst", SqlDbType.NVarChar).Value = this.IsBusAnalyst ? 1 : 0;
                    cmd.Parameters.Add("@IsAMCGEO", SqlDbType.NVarChar).Value = this.IsAMCGEO ? 1 : 0;
                    cmd.Parameters.Add("@IsCASUser", SqlDbType.NVarChar).Value = this.IsCASUser ? 1 : 0;
                    cmd.Parameters.Add("@IsALODUser", SqlDbType.NVarChar).Value = this.IsALODUser ? 1 : 0;

                    cmd.Parameters.Add("@DomainName", SqlDbType.NVarChar).Value = this.DomainName;

                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? "SYSTEM_ADMIN" : HttpContext.Current.User.Identity.Name;

					newID = new SqlParameter("@newID", SqlDbType.Decimal);
					newID.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(newID);

					cmd.ExecuteNonQuery();

					this.ID = newID.Value == DBNull.Value ? 0 : int.Parse(newID.Value.ToString());
					if (this.ID > 0)
					{
						saved = true;
						errorMsg = string.Empty;
					}
					else
					{
						saved = false;
						errorMsg = "Failed to create new User.";
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			this.ID = 0;
			saved = false;
			errorMsg = ex.Message.ToString();
		}
		return this.ID;
	}

	/// <summary>
	/// Update user in the database with assigned values
	/// </summary>
	/// <param name="errorMsg">any relevant error messages</param>
	/// <returns>flag if save was successful</returns>
	public bool Update(out string errorMsg)
	{
		int saved = 0;
		errorMsg = string.Empty;
		SqlParameter paramSaved = null, paramFlagsUpdated = null;

		try
		{
			string procName = "WTS_RESOURCE_UPDATE";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@UserID", SqlDbType.Decimal).Value = this.ID;
					cmd.Parameters.Add("@Membership_UserID", SqlDbType.UniqueIdentifier).Value = string.IsNullOrWhiteSpace(this.Membership_UserID) ? Guid.Empty : new Guid(this.Membership_UserID);
                    cmd.Parameters.Add("@ResourceTypeID", SqlDbType.Decimal).Value = this.ResourceTypeID;
                    cmd.Parameters.Add("@OrganizationID", SqlDbType.Decimal).Value = this.OrganizationID;
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = this.Username;
					cmd.Parameters.Add("@ThemeId", SqlDbType.Decimal).Value = this.ThemeId == 0 ? 1 : this.ThemeId;
					cmd.Parameters.Add("@EnableAnimations", SqlDbType.NVarChar).Value = this.EnableAnimations ? 1 : 0;
					cmd.Parameters.Add("@First_Name", SqlDbType.NVarChar).Value = this.First_Name;
					cmd.Parameters.Add("@Last_Name", SqlDbType.NVarChar).Value = this.Last_Name;
					cmd.Parameters.Add("@Middle_Name", SqlDbType.NVarChar).Value = this.Middle_Name;
					cmd.Parameters.Add("@Prefix", SqlDbType.NVarChar).Value = this.Prefix;
					cmd.Parameters.Add("@Suffix", SqlDbType.NVarChar).Value = this.Suffix;
					cmd.Parameters.Add("@Phone_Office", SqlDbType.NVarChar).Value = this.Phone_Office;
					cmd.Parameters.Add("@Phone_Mobile", SqlDbType.NVarChar).Value = this.Phone_Mobile;
					cmd.Parameters.Add("@Phone_Misc", SqlDbType.NVarChar).Value = this.Phone_Misc;
					cmd.Parameters.Add("@Fax", SqlDbType.NVarChar).Value = this.Fax;
					cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = this.Email;
					cmd.Parameters.Add("@Email2", SqlDbType.NVarChar).Value = this.Email2;
					cmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = this.Address;
					cmd.Parameters.Add("@Address2", SqlDbType.NVarChar).Value = this.Address2;
					cmd.Parameters.Add("@City", SqlDbType.NVarChar).Value = this.City;
					cmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = this.State;
					cmd.Parameters.Add("@Country", SqlDbType.NVarChar).Value = this.Country;
					cmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar).Value = this.PostalCode;
					cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = this.Notes;
					cmd.Parameters.Add("@AttributeFlags", SqlDbType.NVarChar).Value = this.AttributeFlags;
					cmd.Parameters.Add("@Archive", SqlDbType.Decimal).Value = this.Archive ? 1 : 0;
                    cmd.Parameters.Add("@ReceiveSREMail", SqlDbType.Decimal).Value = this.ReceiveSREMail ? 1 : 0;
                    cmd.Parameters.Add("@IncludeInSRCounts", SqlDbType.NVarChar).Value = this.IncludeInSRCounts ? 1 : 0;


                    cmd.Parameters.Add("@IsDeveloper", SqlDbType.NVarChar).Value = this.IsDeveloper ? 1 : 0;
                    cmd.Parameters.Add("@IsBusAnalyst", SqlDbType.NVarChar).Value = this.IsBusAnalyst ? 1 : 0;
                    cmd.Parameters.Add("@IsAMCGEO", SqlDbType.NVarChar).Value = this.IsAMCGEO ? 1 : 0;
                    cmd.Parameters.Add("@IsCASUser", SqlDbType.NVarChar).Value = this.IsCASUser ? 1 : 0;
                    cmd.Parameters.Add("@IsALODUser", SqlDbType.NVarChar).Value = this.IsALODUser ? 1 : 0;

                    cmd.Parameters.Add("@DomainName", SqlDbType.NVarChar).Value = this.DomainName;

                    cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

					paramSaved = new SqlParameter("@saved", SqlDbType.Decimal);
					paramSaved.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(paramSaved);
					paramFlagsUpdated = new SqlParameter("@flagsUpdated", SqlDbType.Decimal);
					paramFlagsUpdated.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(paramFlagsUpdated);


					cmd.ExecuteNonQuery();

					saved = paramSaved.Value == DBNull.Value ? 0 : int.Parse(paramSaved.Value.ToString());
					if (saved > 0)
					{
						errorMsg = string.Empty;
						string msg = paramFlagsUpdated.Value.ToString();
					}
					else
					{
						errorMsg = "Failed to update User.";
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = 0;
			errorMsg = ex.Message.ToString();
		}
		return (saved > 0);
	}

	/// <summary>
	/// load user details from database using passed in membership user id
	/// </summary>
	/// <param name="userId">membership user id in database</param>
	/// <returns>flag if user was loaded successfully</returns>
	public bool Load(string membershipUserId)
	{
		this.Membership_UserID = membershipUserId;
		return Load_GUID();
	}
	/// <summary>
	/// load user details from database using passed in user id
	/// </summary>
	/// <param name="userId">user id in database</param>
	/// <returns>flag if user was loaded successfully</returns>
	public bool Load(int userId)
	{
		this.ID = userId;
		return Load();
	}
	/// <summary>
	/// load user details from database based on assigned ID value
	/// </summary>
	/// <returns>flag if user was loaded successfully</returns>
	public bool Load()
	{
		string procName = "WTS_RESOURCE_LOAD";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@UserID", SqlDbType.Decimal).Value = this.ID;

				try
				{
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);

					if (dr.HasRows)
					{ //populate properties
						dr.Read();

						this.ID = dr["WTS_RESOURCEID"] == DBNull.Value ? 0 : int.Parse(dr["WTS_RESOURCEID"].ToString());
						this.Membership_UserID = dr["Membership_UserId"].ToString();
						this.IsApproved = dr["IsApproved"] == DBNull.Value ? false : bool.Parse(dr["IsApproved"].ToString());
						this.IsLocked = dr["IsLockedOut"] == DBNull.Value ? false : bool.Parse(dr["IsLockedOut"].ToString());
						this.Username = dr["Username"].ToString();
                        this.ResourceTypeID = dr["WTS_RESOURCE_TYPEID"] == DBNull.Value ? 0 : int.Parse(dr["WTS_RESOURCE_TYPEID"].ToString());
                        this.ResourceType = dr["WTS_RESOURCE_TYPE"].ToString();
                        this.OrganizationID = dr["ORGANIZATIONID"] == DBNull.Value ? 0 : int.Parse(dr["ORGANIZATIONID"].ToString());
						this.Organization = dr["ORGANIZATION"].ToString();
						this.ThemeId = dr["ThemeId"] == DBNull.Value ? 0 : int.Parse(dr["ThemeId"].ToString());
						this.Theme = string.IsNullOrWhiteSpace(dr["Theme"].ToString()) ? "Default" : dr["Theme"].ToString();
						this.EnableAnimations = dr["EnableAnimations"] == DBNull.Value ? false : bool.Parse(dr["EnableAnimations"].ToString());
						this.First_Name = dr["First_Name"].ToString();
						this.Last_Name = dr["Last_Name"].ToString();
						this.Middle_Name = dr["Middle_Name"].ToString();
						this.Prefix = dr["Prefix"].ToString();
						this.Suffix = dr["Suffix"].ToString();
						this.Phone_Office = dr["Phone_Office"].ToString();
						this.Phone_Mobile = dr["Phone_Mobile"].ToString();
						this.Phone_Misc = dr["Phone_Misc"].ToString();
						this.Fax = dr["Fax"].ToString();
						this.Email = dr["Email"].ToString();
						this.Email2 = dr["Email2"].ToString();
						this.Address = dr["Address"].ToString();
						this.Address2 = dr["Address2"].ToString();
						this.City = dr["City"].ToString();
						this.State = dr["State"].ToString();
						this.Country = dr["Country"].ToString();
						this.PostalCode = dr["PostalCode"].ToString();
						this.Notes = dr["Notes"].ToString();
						this.Archive = dr["Archive"] == DBNull.Value ? false : bool.Parse(dr["Archive"].ToString());
                        this.ReceiveSREMail = dr["ReceiveSREMail"] == DBNull.Value ? false : bool.Parse(dr["ReceiveSREMail"].ToString());
                        this.IncludeInSRCounts = dr["IncludeInSRCounts"] == DBNull.Value ? false : bool.Parse(dr["IncludeInSRCounts"].ToString());

                        this.IsDeveloper = dr["IsDeveloper"] == DBNull.Value ? false : bool.Parse(dr["IsDeveloper"].ToString());
                        this.IsBusAnalyst = dr["IsBusAnalyst"] == DBNull.Value ? false : bool.Parse(dr["IsBusAnalyst"].ToString());
                        this.IsAMCGEO = dr["IsAMCGEO"] == DBNull.Value ? false : bool.Parse(dr["IsAMCGEO"].ToString());
                        this.IsCASUser = dr["IsCASUser"] == DBNull.Value ? false : bool.Parse(dr["IsCASUser"].ToString());
                        this.IsALODUser = dr["IsALODUser"] == DBNull.Value ? false : bool.Parse(dr["IsALODUser"].ToString());

                        this.AORResourceTeam = dr["AORResourceTeam"] == DBNull.Value ? false : bool.Parse(dr["AORResourceTeam"].ToString());

                        return true;
					}
					else
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					throw;
				}
			}
		}
	}
	/// <summary>
	/// load user details from database based on assigned ID value
	/// </summary>
	/// <returns>flag if user was loaded successfully</returns>
	public bool Load_GUID()
	{
		string procName = "WTS_RESOURCE_LOAD_GUID";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@Membership_UserID", SqlDbType.UniqueIdentifier).Value = new Guid(this.Membership_UserID);

				try
				{
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);

					if (dr.HasRows)
					{ //populate properties
						dr.Read();

						this.ID = dr["WTS_RESOURCEID"] == DBNull.Value ? 0 : int.Parse(dr["WTS_RESOURCEID"].ToString());
						this.Membership_UserID = dr["Membership_UserId"].ToString();
						this.IsApproved = dr["IsApproved"] == DBNull.Value ? false : bool.Parse(dr["IsApproved"].ToString());
						this.IsLocked = dr["IsLockedOut"] == DBNull.Value ? false : bool.Parse(dr["IsLockedOut"].ToString());
						this.Username = dr["Username"].ToString();
                        this.ResourceTypeID = dr["WTS_RESOURCE_TYPEID"] == DBNull.Value ? 0 : int.Parse(dr["WTS_RESOURCE_TYPEID"].ToString());
                        this.ResourceType = dr["WTS_RESOURCE_TYPE"].ToString();
                        this.OrganizationID = dr["ORGANIZATIONID"] == DBNull.Value ? 0 : int.Parse(dr["ORGANIZATIONID"].ToString());
						this.Organization = dr["ORGANIZATION"].ToString();
						this.ThemeId = dr["ThemeId"] == DBNull.Value ? 0 : int.Parse(dr["ThemeId"].ToString());
						this.Theme = string.IsNullOrWhiteSpace(dr["Theme"].ToString()) ? "Default" : dr["Theme"].ToString();
						this.EnableAnimations = dr["EnableAnimations"] == DBNull.Value ? false : bool.Parse(dr["EnableAnimations"].ToString());
						this.First_Name = dr["First_Name"].ToString();
						this.Last_Name = dr["Last_Name"].ToString();
						this.Middle_Name = dr["Middle_Name"].ToString();
						this.Prefix = dr["Prefix"].ToString();
						this.Suffix = dr["Suffix"].ToString();
						this.Phone_Office = dr["Phone_Office"].ToString();
						this.Phone_Mobile = dr["Phone_Mobile"].ToString();
						this.Phone_Misc = dr["Phone_Misc"].ToString();
						this.Fax = dr["Fax"].ToString();
						this.Email = dr["Email"].ToString();
						this.Email2 = dr["Email2"].ToString();
						this.Address = dr["Address"].ToString();
						this.Address2 = dr["Address2"].ToString();
						this.City = dr["City"].ToString();
						this.State = dr["State"].ToString();
						this.Country = dr["Country"].ToString();
						this.PostalCode = dr["PostalCode"].ToString();
						this.Notes = dr["Notes"].ToString();
						this.Archive = dr["Archive"] == DBNull.Value ? false : bool.Parse(dr["Archive"].ToString());
                        this.ReceiveSREMail = dr["ReceiveSREMail"] == DBNull.Value ? false : bool.Parse(dr["ReceiveSREMail"].ToString());
                        this.IncludeInSRCounts = dr["IncludeInSRCounts"] == DBNull.Value ? false : bool.Parse(dr["IncludeInSRCounts"].ToString());
                        this.IsDeveloper = dr["IsDeveloper"] == DBNull.Value ? false : bool.Parse(dr["IsDeveloper"].ToString());
                        this.IsBusAnalyst = dr["IsBusAnalyst"] == DBNull.Value ? false : bool.Parse(dr["IsBusAnalyst"].ToString());
                        this.IsAMCGEO = dr["IsAMCGEO"] == DBNull.Value ? false : bool.Parse(dr["IsAMCGEO"].ToString());
                        this.IsCASUser = dr["IsCASUser"] == DBNull.Value ? false : bool.Parse(dr["IsCASUser"].ToString());
                        this.IsALODUser = dr["IsALODUser"] == DBNull.Value ? false : bool.Parse(dr["IsALODUser"].ToString());
                        
                        return true;
					}
					else
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					throw;
				}
			}
		}
	}

	/// <summary>
	/// Delete this user object from the database
	/// If there are dependencies(user is assigned to other items) then it will be archived instead
	/// </summary>
	/// <param name="exists">User exists in database</param>
	/// <param name="hasDependencies">True if user has dependent database objects</param>
	/// <param name="archived">True if user was archived</param>
	/// <returns>True if user was permanently deleted</returns>
	public bool Delete(out bool exists, out bool hasDependencies, out bool archived, out bool membershipDeleted)
	{
		string procName = "WTS_RESOURCE_DELETE";
		exists = false;
		hasDependencies = false;
		archived = false;
		membershipDeleted = false;
		int deleted = 0;

		DataTable dt = new DataTable();
		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@UserID", SqlDbType.Decimal).Value = this.ID;

				SqlParameter paramExists = new SqlParameter("@exists", SqlDbType.Decimal);
				paramExists.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramExists);

				SqlParameter paramHasDependencies = new SqlParameter("@hasDependencies", SqlDbType.Decimal);
				paramHasDependencies.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramHasDependencies);

				SqlParameter paramDeleted = new SqlParameter("@deleted", SqlDbType.Decimal);
				paramDeleted.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramDeleted);

				SqlParameter paramArchived = new SqlParameter("@archived", SqlDbType.Decimal);
				paramArchived.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramArchived);

				try
				{
					cmd.ExecuteNonQuery();

					exists = paramExists.Value == DBNull.Value ? false : (int.Parse(paramExists.Value.ToString()) > 0);
					hasDependencies = paramHasDependencies.Value == DBNull.Value ? false : (int.Parse(paramHasDependencies.Value.ToString()) > 0);
					deleted = paramDeleted.Value == DBNull.Value ? 0 : int.Parse(paramDeleted.Value.ToString());
					archived = paramArchived.Value == DBNull.Value ? false : (int.Parse(paramArchived.Value.ToString()) > 0);

					if (deleted > 0)
					{
						Guid memId = Guid.Empty;
						if (Guid.TryParse(this.Membership_UserID, out memId))
						{
							UserManagement.ClearPasswordResetRequests(memId);
							MembershipUser mu = Membership.GetUser(memId);
							membershipDeleted = Membership.DeleteUser(mu.UserName, true);
						}
					}
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					deleted = 0;
					throw;
				}
				return (deleted > 0);
			}
		}
	}

	public bool HasDependencies()
	{
		string funcName = "Resource_HasDependencies";

		DataTable dt = new DataTable();
		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(funcName, cn))
			{
				cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 50).Value = this.ID;

				try
				{
					return (bool)cmd.ExecuteScalar();
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					throw;
				}
			}
		}
	}

	/// <summary>
	/// Insert new password reset request into database
	/// </summary>
	/// <param name="resetCode">GUID representing new record</param>
	/// <param name="requestTimeUTC">request date in UTC time (can be used for request expiration)</param>
	/// <param name="errorMsg">any relevant error messages</param>
	/// <returns>true if record was successfully added</returns>
	public bool RequestPasswordReset(long requestDateTicks, out Guid resetCode, out string errorMsg)
	{
		bool saved = false;
		errorMsg = string.Empty;

		resetCode = new Guid();

		//save to database
		try
		{
			string procName = "PasswordResetRequest_Add";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier).Value = string.IsNullOrWhiteSpace(this.Membership_UserID) ? Guid.Empty : new Guid(this.Membership_UserID);
					cmd.Parameters.Add("@requestDateTicks", SqlDbType.BigInt).Value = requestDateTicks;

					SqlParameter paramResetCode = new SqlParameter("@resetcode", SqlDbType.UniqueIdentifier);
					paramResetCode.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(paramResetCode);

					cmd.ExecuteNonQuery();
					resetCode = string.IsNullOrWhiteSpace(paramResetCode.Value.ToString()) ? Guid.Empty : (Guid)paramResetCode.Value;

					if (resetCode != Guid.Empty)
					{
						saved = true;
					}
					else
					{
						saved = false;
						errorMsg = "Failed to create new password reset request.";
						resetCode = Guid.Empty;
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			resetCode = Guid.Empty;
			saved = false;
			errorMsg = ex.Message.ToString();
		}

		return saved;
	}


	#region Attributes

	public DataTable Resource_AttributeList_Get()
	{
		string procName = "Resource_AttributeList_Get";

		using (DataTable dt = new DataTable("Attribute"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = this.ID;

					using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
					{
						if (dr != null && dr.HasRows)
						{
							dt.Load(dr);
							return dt;
						}
						else
						{
							return null;
						}
					}
				}
			}
		}
	}

	public bool Resource_Flags_Update(string attributeFlags, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "Resource_Flags_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = this.ID;
				cmd.Parameters.Add("@AttributeFlags", SqlDbType.NVarChar).Value = attributeFlags;
				
				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	#endregion Attributes


	#region Custom User Options/Settings

	public DataTable Resource_OptionList_Get()
	{
		string procName = "Resource_OptionList_Get";

		using (DataTable dt = new DataTable("Option"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = this.ID;

					using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
					{
						if (dr != null && dr.HasRows)
						{
							dt.Load(dr);
							return dt;
						}
						else
						{
							return null;
						}
					}
				}
			}
		}
	}

	public DataTable UserSettingList_Get(int userID = 0, int userSettingTypeID = 1, int gridNameID = 0)
	{
		string procName = "UserSettingList_Get";

		using (DataTable dt = new DataTable("Setting"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = userID; // this.ID;
					cmd.Parameters.Add("@UserSettingTypeID", SqlDbType.Int).Value = userSettingTypeID;
					cmd.Parameters.Add("@GridNameID", SqlDbType.Int).Value = gridNameID == 0 ? (object)DBNull.Value : gridNameID;

					using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
					{
						if (dr != null && dr.HasRows)
						{
							dt.Load(dr);
							return dt;
						}
						else
						{
							return null;
						}
					}
				}
			}
		}
	}

	public bool UserSetting_Add(int userSettingTypeID
		, int gridNameID
		, string settingValue
		, out bool exists
		, out int newID
		, out string errorMsg)
	{
		newID = 0;
		errorMsg = string.Empty;
		exists = false;
		int existsVal = 0;
		bool saved = false;

		try
		{
			string procName = "UserSetting_Add";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Decimal).Value = this.ID;
					cmd.Parameters.Add("@UserSettingTypeID", SqlDbType.Decimal).Value = userSettingTypeID;
					cmd.Parameters.Add("@GridNameID", SqlDbType.Decimal).Value = gridNameID == 0 ? (object)DBNull.Value : gridNameID;
					cmd.Parameters.Add("@SettingValue", SqlDbType.NVarChar).Value = settingValue.Trim();
					cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

					cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
					cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

					cmd.ExecuteNonQuery();

					SqlParameter paramExists = cmd.Parameters["@exists"];
					if (paramExists != null && int.TryParse(paramExists.Value.ToString(), out existsVal))
					{
						exists = (existsVal >= 1);
						if (exists)
						{
							saved = false;
							errorMsg = "The specified setting already exists.";
						}
					}

					SqlParameter paramNewID = cmd.Parameters["@newID"];
					if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
					{
						saved = true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			newID = 0;
			saved = false;
			errorMsg = ex.Message.ToString();
		}


        // 11626 - 2 > Store saved preferences to session variables.
        string defaultStartGrid = "";
        string defaultCrosswalkGrid = "";
        int defaultWorkloadGrid = 0;

        switch (gridNameID)
        {
            case 9:  // Default
                switch (settingValue.Trim())
                {
                    case "12":
                        defaultStartGrid = "Enterprise";
                        break;
                    case "13":
                        defaultStartGrid = "My Data";
                        break;
                    default:
                        defaultStartGrid = "My Data";
                        break;
                }
                break;

            case 10:  // Crosswalk 
                switch (settingValue.Trim())
                {
                    case "91":
                        defaultCrosswalkGrid = "BUS Stand Up";
                        break;
                    case "90":
                        defaultCrosswalkGrid = "DEV Stand Up";
                        break;
                    case "14":
                        defaultCrosswalkGrid = "Default";
                        break;
                    default:
                        defaultCrosswalkGrid = "BUS Stand Up";
                        break;
                }
                break;

            case 1:  // Workload
                switch (settingValue.Trim())
                {
                    case "1":
                        defaultWorkloadGrid = 1;
                        break;
                    case "2":
                        defaultWorkloadGrid = 2;
                        break;
                    case "3":
                        defaultWorkloadGrid = 3;
                        break;
                    default:
                        defaultWorkloadGrid = 2;
                        break;
                }
                break;


            default:
                defaultStartGrid = "My Data";
                defaultCrosswalkGrid = "BUS Stand Up";
                defaultWorkloadGrid = 2;
                break;
        }

        HttpContext.Current.Session["defaultStartGrid"] = defaultStartGrid;
        HttpContext.Current.Session["defaultCrosswalkGrid"] = defaultCrosswalkGrid;
        HttpContext.Current.Session["defaultWorkloadGrid"] = defaultWorkloadGrid;

        return saved;
	}

	public bool UserSetting_Update(int userSettingID
		, int userSettingTypeID
		, int gridNameID
		, string settingValue
		, out bool duplicate
		, out string errorMsg)
	{
		duplicate = false;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "UserSetting_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@UserSettingID", SqlDbType.Int).Value = userSettingID;
				cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Decimal).Value = this.ID;
				cmd.Parameters.Add("@UserSettingTypeID", SqlDbType.Decimal).Value = userSettingTypeID;
				cmd.Parameters.Add("@GridNameID", SqlDbType.Decimal).Value = gridNameID == 0 ? (object)DBNull.Value : gridNameID;
				cmd.Parameters.Add("@SettingValue", SqlDbType.NVarChar).Value = settingValue.Trim();
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDuplicate = cmd.Parameters["@exists"];
				if (paramDuplicate != null)
				{
					bool.TryParse(paramDuplicate.Value.ToString(), out duplicate);
				}
				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

        // 11626 - 2 > Store saved preferences to session variables.
        string defaultStartGrid = "My Data";
        string defaultCrosswalkGrid = "Default";
        int defaultWorkloadGrid = 3;
        string sqlCommand = "";

        switch (gridNameID)
        {
            case 9:  // Default
                sqlCommand = "SELECT ViewName FROM GridView WHERE GridNameID = " + gridNameID + " AND GridViewID = " + settingValue.Trim();
                defaultStartGrid = WTSData.GetScalarString(sqlCommand);
                break;

            case 10:  // Crosswalk 
                sqlCommand = "SELECT ViewName FROM GridView WHERE GridNameID = " + gridNameID + " AND GridViewID = " + settingValue.Trim();
                defaultCrosswalkGrid = WTSData.GetScalarString(sqlCommand);
                break;

            case 1:  // Workload
                Int32.TryParse(settingValue.Trim(), out defaultWorkloadGrid);
                break;

            default:
                break;
        }

        HttpContext.Current.Session["defaultStartGrid"] = defaultStartGrid;
        HttpContext.Current.Session["defaultCrosswalkGrid"] = defaultCrosswalkGrid;
        HttpContext.Current.Session["defaultWorkloadGrid"] = defaultWorkloadGrid;

        return saved;
	}

	public bool UserSetting_Delete(int userSettingID, out bool exists)
	{
		string procName = "UserSetting_Delete";
		exists = false;
		int deleted = 0;

		DataTable dt = new DataTable();
		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@UserSettingID", SqlDbType.Decimal).Value = userSettingID;


				SqlParameter paramExists = new SqlParameter("@exists", SqlDbType.Decimal);
				paramExists.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramExists);

				SqlParameter paramDeleted = new SqlParameter("@deleted", SqlDbType.Decimal);
				paramDeleted.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramDeleted);

				try
				{
					cmd.ExecuteNonQuery();

					exists = paramExists.Value == DBNull.Value ? false : (int.Parse(paramExists.Value.ToString()) > 0);
					deleted = paramDeleted.Value == DBNull.Value ? 0 : int.Parse(paramDeleted.Value.ToString());
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					deleted = 0;
					throw;
				}
				return (deleted > 0);
			}
		}
	}

	#endregion Custom User Options/Settings


	#region Certifications

	public DataTable CertificationList_Get()
	{
		string procName = "Resource_CertificationList_Get";

		using (DataTable dt = new DataTable("Certification"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WTS_RescourceID", SqlDbType.Int).Value = this.ID;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						throw;
					}
				}
			}
		}
	}

	public bool Certification_Add(string certification
		, string description
		, string expirationDate
		, bool expired
		, out int newID
		, out string errorMsg)
	{
		newID = 0;
		errorMsg = string.Empty;
		int exists = 0;
		bool saved = false;
		DateTime dtDate;

		try
		{
			string procName = "Resource_Certification_Add";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Decimal).Value = this.ID;
					cmd.Parameters.Add("@Certification", SqlDbType.NVarChar).Value = certification;
					cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
					cmd.Parameters.Add("@Expiration_Date", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(expirationDate) || !DateTime.TryParse(expirationDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
					cmd.Parameters.Add("@Expired", SqlDbType.NVarChar).Value = expired ? 1 : 0;
					cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

					cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
					cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

					cmd.ExecuteNonQuery();

					SqlParameter paramExists = cmd.Parameters["@exists"];
					if (paramExists != null && int.TryParse(paramExists.Value.ToString(), out exists))
					{
						saved = false;
					}

					SqlParameter paramNewID = cmd.Parameters["@newID"];
					if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
					{
						saved = true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			newID = 0;
			saved = false;
			errorMsg = ex.Message.ToString();
		}
		return saved;
	}

	public bool Certification_Update(int certificationID
		, string certification
		, string description
		, string expirationDate
		, bool expired
		, out bool duplicate
		, out string errorMsg)
	{
		duplicate = false;
		errorMsg = string.Empty;
		bool saved = false;
		DateTime dtDate;

		string procName = "Resource_Certification_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@Resource_CertificationID", SqlDbType.Int).Value = certificationID;
				cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = this.ID;
				cmd.Parameters.Add("@Certification", SqlDbType.NVarChar).Value = certification.Trim();
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : description;
				cmd.Parameters.Add("@Expiration_Date", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(expirationDate) || !DateTime.TryParse(expirationDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@Expired", SqlDbType.Bit).Value = expired ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDuplicate = cmd.Parameters["@duplicate"];
				if (paramDuplicate != null)
				{
					bool.TryParse(paramDuplicate.Value.ToString(), out duplicate);
				}
				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	public bool Certification_Delete(int certificationID, out bool exists)
	{
		string procName = "Resource_Certification_Delete";
		exists = false;
		int deleted = 0;

		DataTable dt = new DataTable();
		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@Resource_CertificationID", SqlDbType.Decimal).Value = certificationID;


				SqlParameter paramExists = new SqlParameter("@exists", SqlDbType.Decimal);
				paramExists.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramExists);

				SqlParameter paramDeleted = new SqlParameter("@deleted", SqlDbType.Decimal);
				paramDeleted.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramDeleted);

				try
				{
					cmd.ExecuteNonQuery();

					exists = paramExists.Value == DBNull.Value ? false : (int.Parse(paramExists.Value.ToString()) > 0);
					deleted = paramDeleted.Value == DBNull.Value ? 0 : int.Parse(paramDeleted.Value.ToString());
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					deleted = 0;
					throw;
				}
				return (deleted > 0);
			}
		}
	}

	#endregion Certifications


	#region Hardware
	
	public DataTable HardwareList_Get()
	{
		string procName = "WTS_Resource_HardwareList_Get";

		using (DataTable dt = new DataTable("Hardware"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WTS_RescourceID", SqlDbType.Int).Value = this.ID;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						throw;
					}
				}
			}
		}
	}

	public bool Hardware_Add(int hardwareTypeID
		, string deviceName
		, string deviceSN_Tag
		, string description
		, bool hasDevice
		, out int newID
		, out string errorMsg)
	{
		newID = 0;
		errorMsg = string.Empty;
		int exists = 0;
		bool saved = false;

		try
		{
			string procName = "WTS_Resource_Hardware_Add";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Decimal).Value = this.ID;
					cmd.Parameters.Add("@HardwareTypeID", SqlDbType.Int).Value = hardwareTypeID == 0 ? (object)DBNull.Value : hardwareTypeID;
					cmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar).Value = deviceName.Trim();
					cmd.Parameters.Add("@DeviceSN_Tag", SqlDbType.NVarChar).Value = deviceSN_Tag.Trim();
					cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : description.Trim();
					cmd.Parameters.Add("@HasDevice", SqlDbType.Bit).Value = hasDevice ? 1 : 0;
					cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

					cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
					cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

					cmd.ExecuteNonQuery();

					SqlParameter paramExists = cmd.Parameters["@exists"];
					if (paramExists != null && int.TryParse(paramExists.Value.ToString(), out exists))
					{
						saved = false;
					}

					SqlParameter paramNewID = cmd.Parameters["@newID"];
					if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
					{
						saved = true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			newID = 0;
			saved = false;
			errorMsg = ex.Message.ToString();
		}
		return saved;
	}

	public bool Hardware_Update(int resourceHardwareID
		, int hardwareTypeID
		, string deviceName
		, string deviceSN_Tag
		, string description
		, bool hasDevice
		, out bool duplicate
		, out string errorMsg)
	{
		duplicate = false;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WTS_Resource_Hardware_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WTS_Resource_HardwareID", SqlDbType.Int).Value = resourceHardwareID;
				cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = this.ID;
				cmd.Parameters.Add("@HardwareTypeID", SqlDbType.Int).Value = hardwareTypeID == 0 ? (object)DBNull.Value : hardwareTypeID;
				cmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar).Value = deviceName.Trim();
				cmd.Parameters.Add("@DeviceSN_Tag", SqlDbType.NVarChar).Value = deviceSN_Tag.Trim();
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : description.Trim();
				cmd.Parameters.Add("@HasDevice", SqlDbType.Bit).Value = hasDevice ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	public bool Hardware_Delete(int resourceHardwareID, out bool exists)
	{
		string procName = "WTS_Resource_Hardware_Delete";
		exists = false;
		int deleted = 0;

		DataTable dt = new DataTable();
		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WTS_Resource_HardwareID", SqlDbType.Decimal).Value = resourceHardwareID;


				SqlParameter paramExists = new SqlParameter("@exists", SqlDbType.Decimal);
				paramExists.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramExists);

				SqlParameter paramDeleted = new SqlParameter("@deleted", SqlDbType.Decimal);
				paramDeleted.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramDeleted);

				try
				{
					cmd.ExecuteNonQuery();

					exists = paramExists.Value == DBNull.Value ? false : (int.Parse(paramExists.Value.ToString()) > 0);
					deleted = paramDeleted.Value == DBNull.Value ? 0 : int.Parse(paramDeleted.Value.ToString());
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					deleted = 0;
					throw;
				}
				return (deleted > 0);
			}
		}
	}

	#endregion Hardware
	
}

/// <summary>
/// Organization class containing Organization properties and C.R.U.D. methods
/// </summary>
public class Organization
{
	#region Properties
	/// <summary>
	/// database id of Organization
	/// </summary>
	public int ID { get; set; }
	/// <summary>
	/// name of Organization
	/// </summary>
	public string OrganizationNm { get; set; }

	/// <summary>
	/// comma delimited list of default role ids
	/// </summary>
	public string DefaultRolesComma
	{
		get
		{
			StringBuilder sbRoles = new StringBuilder();
			foreach (string s in _defaultRoles)
			{
				if (sbRoles.Length > 0) { sbRoles.Append(","); }
				sbRoles.Append(s);
			}
			return sbRoles.ToString();
		}
	}
	private List<string> _defaultRoles = new List<string>();
	/// <summary>
	/// List of default roles for this Organization
	/// </summary>
	public List<string> DefaultRoles
	{
		get { return _defaultRoles; }
		private set
		{
			_defaultRoles = value;

		}
	}

	/// <summary>
	/// comma delimited list of users
	/// </summary>
	public string UsernamesComma
	{
		get
		{
			StringBuilder sbUsers = new StringBuilder();
			foreach (string s in _usernames.Values)
			{
				if (sbUsers.Length > 0) { sbUsers.Append(","); }
				sbUsers.Append(s);
			}
			return sbUsers.ToString();
		}
	}
	private Dictionary<int, string> _usernames = new Dictionary<int, string>();
	/// <summary>
	/// List of users assigned this Organization
	/// </summary>
	public Dictionary<int, string> Users
	{
		get { return _usernames; }
		private set
		{
			_usernames = value;

		}
	}

	/// <summary>
	/// description of Organization
	/// </summary>
	public string Description { get; set; }
	/// <summary>
	/// flag if Organization is archived
	/// </summary>
	public bool Archive { get; set; }
	#endregion

	#region Constructors
	public Organization() { _defaultRoles = new List<string>(); }
	public Organization(int id)
	{
		_defaultRoles = new List<string>();
		this.ID = id;
	}
	#endregion

	public int Add(out bool saved, out string errorMsg)
	{
		saved = false; errorMsg = string.Empty;
		SqlParameter exists = null, newID = null, rolesAdded = null;

		try
		{
			string procName = "ORGANIZATION_ADD";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@Organization", SqlDbType.NVarChar).Value = this.OrganizationNm;
					cmd.Parameters.Add("@DefaultRoles", SqlDbType.NVarChar).Value = this.DefaultRolesComma;
					cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = this.Description;
					cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = this.Archive;
					cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? "SYSTEM_ADMIN" : HttpContext.Current.User.Identity.Name;

					exists = new SqlParameter("@exists", SqlDbType.Int);
					exists.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(exists);

					newID = new SqlParameter("@newID", SqlDbType.Int);
					newID.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(newID);

					rolesAdded = new SqlParameter("@rolesAdded", SqlDbType.Int);
					rolesAdded.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(rolesAdded);

					cmd.ExecuteNonQuery();

					int existing = exists.Value == DBNull.Value ? 0 : int.Parse(exists.Value.ToString());
					if (existing > 0)
					{
						this.ID = 0;
						saved = false;
						errorMsg = "Organization already exists with the same name.";
						return 0;
					}

					this.ID = newID.Value == DBNull.Value ? 0 : int.Parse(newID.Value.ToString());
					if (this.ID > 0)
					{
						saved = true;
						errorMsg = string.Empty;
					}
					else
					{
						saved = false;
						errorMsg = "Failed to create new Organization.";
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			this.ID = 0;
			saved = false;
			errorMsg = ex.Message.ToString();
		}
		return this.ID;
	}

	public bool Update(out string errorMsg)
	{
		int saved = 0;
		errorMsg = string.Empty;
		SqlParameter paramSaved = null, rolesUpdated = null;

		try
		{
			string procName = "ORGANIZATION_UPDATE";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@OrganizationID", SqlDbType.Decimal).Value = this.ID;
					cmd.Parameters.Add("@Organization", SqlDbType.NVarChar).Value = this.OrganizationNm;
					cmd.Parameters.Add("@DefaultRoles", SqlDbType.NVarChar).Value = this.DefaultRolesComma;
					cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = this.Description;
					cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = this.Archive;
					cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? "SYSTEM_ADMIN" : HttpContext.Current.User.Identity.Name;

					paramSaved = new SqlParameter("@saved", SqlDbType.Decimal);
					paramSaved.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(paramSaved);

					rolesUpdated = new SqlParameter("@rolesUpdated", SqlDbType.Int);
					rolesUpdated.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(rolesUpdated);

					cmd.ExecuteNonQuery();

					saved = paramSaved.Value == DBNull.Value ? 0 : int.Parse(paramSaved.Value.ToString());
					if (saved > 0)
					{
						errorMsg = string.Empty;
					}
					else
					{
						errorMsg = "Failed to update Organization.";
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = 0;
			errorMsg = ex.Message.ToString();
		}
		return (saved > 0);
	}

	public bool Load(int organizationId)
	{
		this.ID = organizationId;
		return Load();
	}
	public bool Load()
	{
		string procName = "ORGANIZATION_LOAD";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@OrganizationID", SqlDbType.Decimal).Value = this.ID;

				try
				{
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows)
					{ //populate properties
						dr.Read();

						this.ID = dr["ORGANIZATIONID"] == DBNull.Value ? 0 : int.Parse(dr["ORGANIZATIONID"].ToString());
						this.OrganizationNm = dr["ORGANIZATION"].ToString();
						string defaultRolesComma = dr["DefaultRoles"].ToString();
						foreach (string role in defaultRolesComma.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
						{
							this.AddDefaultRole(role);
						}
						string usersComma = dr["Users"].ToString();
						foreach (string user in usersComma.Split(new char[] { ',' }, StringSplitOptions.None))
						{
							this.AddUser(user);
						}
						this.Description = dr["DESCRIPTION"].ToString();
						this.Archive = dr["Archive"] == DBNull.Value ? false : bool.Parse(dr["Archive"].ToString());

						return true;
					}
					else
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					throw;
				}
			}
		}
	}

	public bool Delete(out bool exists, out bool hasDependencies, out bool archived)
	{
		string procName = "ORGANIZATION_DELETE";
		exists = false;
		hasDependencies = false;
		archived = false;
		bool deleted = false;

		DataTable dt = new DataTable();
		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("OrganizationID", SqlDbType.Decimal).Value = this.ID;


				SqlParameter paramExists = new SqlParameter("@exists", SqlDbType.Int);
				paramExists.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramExists);

				SqlParameter paramHasDependencies = new SqlParameter("@hasDependencies", SqlDbType.Int);
				paramHasDependencies.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramHasDependencies);

				SqlParameter paramDeleted = new SqlParameter("@deleted", SqlDbType.Int);
				paramDeleted.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramDeleted);

				SqlParameter paramArchived = new SqlParameter("@archived", SqlDbType.Int);
				paramArchived.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(paramArchived);

				try
				{
					cmd.ExecuteNonQuery();

					exists = paramExists.Value == DBNull.Value ? false : (int.Parse(paramExists.Value.ToString()) > 0);
					hasDependencies = paramHasDependencies.Value == DBNull.Value ? false : (int.Parse(paramHasDependencies.Value.ToString()) > 0);
					deleted = paramDeleted.Value == DBNull.Value ? false : (int.Parse(paramDeleted.Value.ToString()) > 0);
					archived = paramArchived.Value == DBNull.Value ? false : (int.Parse(paramArchived.Value.ToString()) > 0);
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					deleted = false;
				}
			}
		}

		return deleted;
	}

	public bool HasDependencies()
	{
		string funcName = "SELECT dbo.Organization_HasDependencies(@OrganizationID)";

		DataTable dt = new DataTable();
		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(funcName, cn))
			{
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add("@OrganizationID", SqlDbType.NVarChar, 50).Value = this.ID;

				try
				{
					return (bool)cmd.ExecuteScalar();
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					throw;
				}
			}
		}
	}

	public List<string> AddDefaultRole(string roleName)
	{
		if (this.DefaultRoles == null) { this.DefaultRoles = new List<string>(); }

		if (this.DefaultRoles.FindIndex(s => s.StartsWith(roleName.Trim(), StringComparison.CurrentCultureIgnoreCase)) <= 0)
		{
			this.DefaultRoles.Add(roleName);
		}

		return this.DefaultRoles;
	}

	/// <summary>
	/// Add user to list assigned this Organization
	/// </summary>
	/// <param name="user">id:username</param>
	/// <returns></returns>
	public Dictionary<int, string> AddUser(string idNameColon)
	{
		if (this.Users == null) { this.Users = new Dictionary<int, string>(); }

		string[] user = idNameColon.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
		if (user.Length >= 2)
		{
			int id = 0;
			if (int.TryParse(user[0], out id)
				&& id > 0)
			{
				if (!this.Users.ContainsKey(id))
				{
					this.Users.Add(id, user[1]);
				}
			}
		}

		return this.Users;
	}
}