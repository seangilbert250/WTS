﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;

/// <summary>
/// Summary description for WTSConfiguration
/// </summary>
public sealed class WTSConfiguration
{
	public enum ConfigSetting_Type
	{
		EmailServer = 1,
		EmailFrom,
		EmailFromName,
		ErrorEmailTo,
		ErrorEmailToName,
		ErrorEmailFrom,
		ErrorEmailFromName,
		TechSupport_Email,
		TechSupport_Phone,
		TechSupport_Fax,
		PasswordResetExpiration,
		RegistrationEmailTo,
		RegistrationEmailToName,
		RegistrationEmailBcc,
		RegistrationEmailBccName
	}

	/// <summary>
	/// Gets the location of the data store.
	/// </summary>
	public static DataProtectionScope StoreLocation
	{
		get
		{
			string location = ConfigurationManager.AppSettings["StoreLocation"];
			int compare = string.Compare(location, "CurrentUser", true, CultureInfo.InvariantCulture);
			if (compare == 0)
			{
				return DataProtectionScope.CurrentUser;
			}

			return DataProtectionScope.LocalMachine;
		}
	}
	/// <summary>
	/// Gets a value indicating whether the web site is in debug mode.
	/// </summary>
	public static bool DebugMode
	{
		get
		{
			return HttpContext.Current.IsDebuggingEnabled;
		}
	}

	/// <summary>
	/// Copyright Year
	/// </summary>
	public static int CopyrightYear
	{
		get
		{
			return 2015;
		}
	}
	/// <summary>
	/// Company name for contract
	/// </summary>
	public static string CopyrightContractor
	{
		get
		{
			return "Infinite Technologies, Inc.";
		}
	}

	/// <summary>
	/// Gets a value for the instance specific title of this system.
	/// </summary>
	public static string AppTitle
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("AppTitle").ToString().ToUpper();
		}
	}

	/// <summary>
	/// Gets a value for the instance specific abbreviated title of this system.
	/// </summary>
	public static string AppTitleAbbreviation
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("AppTitleAbbreviation").ToString().ToUpper();
		}
	}

	/// <summary>
	/// Gets a value for the instance specific name of this system.
	/// </summary>
	public static string AppName
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("AppName").ToString().ToUpper();
		}
	}

	/// <summary>
	/// Gets a value for the instance specific URL of this system.
	/// </summary>
	public static string Host
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("Host").ToString();
		}
	}

	/// <summary>
	/// Gets the application acronym as stored in CAFDEx.
	/// </summary>
	public static string ApplicationAcronym
	{
		get
		{
			return ConfigurationManager.AppSettings["Application-Acronym"].ToString();
		}
	}

	/// <summary>
	/// Gets the current version of this web application.
	/// </summary>
	public static string AppVersion
	{
		get
		{
			string version = ConfigurationManager.AppSettings["Application-Version"].ToString();
			return version.Substring(0, version.LastIndexOf('.'));
		}
	}

	/// <summary>
	/// Gets a value indicating whether the system is in a test environment.
	/// </summary>
	public static string Environment
	{
		get
		{
			return ConfigurationManager.AppSettings["Environment"].ToString().Trim().ToUpper();
		}
	}

	/// <summary>
	/// Gets a value indicating whether the authentication will be handled through ITI Portal.
	/// </summary>
	public static bool Commercial
	{
		get
		{
			return (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Commercial"].ToString())
				&& ConfigurationManager.AppSettings["Commercial"].ToString().ToLower() == "true");
		}
	}

	/// <summary>
	/// Number of minutes after reset request is sent before the code expires
	/// </summary>
	public static int PasswordResetExpiration
	{
		get
		{
			string text = System.Configuration.ConfigurationManager.AppSettings.Get("PasswordResetExpiration").ToString();
			int val = 0;
			int.TryParse(text, out val);
			return val;
		}
	}

	public static string EmailServer
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("EmailServer").ToString();
		}
	}

	public static string EmailFrom
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("EmailFrom").ToString();
		}
	}

    public static bool EmailEnabled
    {
        get
        {
            return string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings.Get("EmailEnabled")) || System.Configuration.ConfigurationManager.AppSettings.Get("EmailEnabled").ToLower() == "true";
        }
    }

    public static string EmailOverride
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get("EmailOverride").ToString();
        }
    }

    public static void GetEmailFromAddress(out string fromAddress, out string fromName)
	{
		fromAddress = "";
		fromName = "";

		string procName = "Load_EmailAddress_List";

		using (DataTable dt = new DataTable("EmailAddress"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@ConfigSetting_TypeID", SqlDbType.Int).Value = (int)ConfigSetting_Type.EmailFrom;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dr.Read();
								fromAddress = dr["Address"].ToString().Trim();
								fromName = dr["Name"].ToString().Trim();
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


	public static string EmailFromName
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("EmailFromName").ToString();
		}
	}

	public static string ErrorEmailTo
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailTo").ToString();
		}
	}

	public static string ErrorEmailToName
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailToName").ToString();
		}
	}

	public static string ErrorEmailFrom
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailFrom").ToString();
		}
	}

	public static string ErrorEmailFromName
	{
		get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailFromName").ToString();
		}
	}



	public static Dictionary<string, string> LoadEmailAddressList(int configSettingId = (int)WTSConfiguration.ConfigSetting_Type.EmailFrom)
	{
		Dictionary<string, string> addressNameList = new Dictionary<string, string>();

		string procName = "Load_EmailAddress_List";

		using (DataTable dt = new DataTable("EmailAddress"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@ConfigSetting_TypeID", SqlDbType.Int).Value = configSettingId;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								while (dr.Read())
								{
									if (!addressNameList.ContainsKey(dr["Address"].ToString().Trim()))
									{
										addressNameList.Add(dr["Address"].ToString().Trim(), dr["Name"].ToString().Trim());
									}
								}
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

		return addressNameList;
	}

    public static bool EventQueueEnabled
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get("EventQueueEnabled").ToString().ToLower() == "true";
        }
    }

    public static int EventQueueRunFrequencySeconds
    {
        get
        {
            int seconds = 5;

            if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("EventQueueRunFrequencySeconds"), out seconds))
            {
                seconds = 5;
            }

            if (seconds < 0 || seconds > (60 * 60 * 24))
            {
                seconds = 5;
            }

            return seconds;
        }
    }

    public static int EventQueueCleanMaxAgeHours
    {
        get
        {
            int hours = 24 * 7;

            if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("EventQueueCleanMaxAgeHours"), out hours))
            {
                hours = 24 * 7;
            }

            if (hours < 0 || hours > (24 * 31))
            {
                hours = 24 * 7;
            }

            return hours;
        }
    }

    public static int ReportQueueCleanMaxAgeHours
    {
        get
        {
            int hours = 24 * 7;

            if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("ReportQueueCleanMaxAgeHours"), out hours))
            {
                hours = 24 * 7;
            }

            if (hours < 0 || hours > (24 * 31))
            {
                hours = 24 * 7;
            }

            return hours;
        }
    }

}
