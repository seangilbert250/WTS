using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;

/// <summary>
/// Utility for logging and emailing messages
/// </summary>
public sealed class LogUtility
{

	#region Properties 

	/// <summary>
	/// Gets the level at which the log information will be saved.
	/// </summary>
	/// <returns></returns>
	public static TraceLevel LogLevel
	{
		get
		{
			string level = System.Configuration.ConfigurationManager.AppSettings["LogLevel"];
			if(!string.IsNullOrEmpty(level))
			{
				try 
				{	        
					TraceLevel log = (TraceLevel)(Enum.Parse(typeof(TraceLevel), level));
					if(Enum.IsDefined(typeof(TraceLevel), log))
					{
						return log;
					}
				}
				catch (Exception) { }
			}
			return TraceLevel.Error;
		}
	}

	/// <summary>
	/// Gets the current session ID.
	/// </summary>
	public static string SessionID
	{
		get
		{
			return HttpContext.Current != null && HttpContext.Current.Session != null ? HttpContext.Current.Session.SessionID : "[NONE]";
		}
	}

	#endregion

	/// <summary>
	/// Attempts to retrieve the name of the current user.
	/// </summary>
	/// <param name="context">The context from which to retrieve the username.</param>
	/// <returns>The name of the current user.</returns>
	private static string GetCurrentUserName(HttpContext context = null)
	{
        if (context == null || context.User == null)
        {
            return "WTS_User";
        }
		return context.User.Identity.Name;
	}

	/// <summary>
	/// Attempts to email the information about the <see cref="Exception">Exception</see> that was thrown.
	/// </summary>
	/// <param name="ex">The <see cref="Exception">Exception</see> that was thrown.</param>
	/// <param name="errorid">The ID of the error.</param>
	/// <returns>A value indicating whether the <see cref="Exception">Exception</see> information was emailed.</returns>
	public static bool EmailException(Exception ex, long errorid = -1)
	{
		while (ex != null && ex.GetType() == typeof(HttpUnhandledException))
		{
			ex = ex.InnerException;
		}

		if (ex == null) { return false; }

		HttpContext context = HttpContext.Current;
		string username = GetCurrentUserName(context);

		string[] to = WTSConfiguration.ErrorEmailTo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		List<MailAddress> toAddresses = new List<MailAddress>();
		MailAddress addr, fromAddr;
		foreach (string s in to)
		{
			//create new to address
			addr = new MailAddress(s);
			toAddresses.Add(addr);
		}
		string from = WTSConfiguration.ErrorEmailFrom;
		fromAddr = new MailAddress(from, WTSConfiguration.ErrorEmailFromName);

		string subject = "";
		if (string.IsNullOrEmpty(username))
		{
			subject = String.Format("Unhandled Exception - {0}", ex.GetType().FullName);
		}
		else
		{
			subject = String.Format("Unhandled Exception - {0} ({1})", ex.GetType().FullName, username);
		}

		StringBuilder html = new StringBuilder(@"<style type=""text/css""><!-- ");
		html.Append("body,table{font-family:sans-serif;font-size:12px}h3{margin:20px 0 5px 2px}div{background-color:#ddd;padding:.5em}th,td{padding:0}");
		html.Append("th{padding-right:20px;text-align:left;vertical-align:top;white-space:nowrap}th.sub-title{font-size:1.3em;padding:15px 0 3px 5px;text-decoration:underline}pre{margin:0}");
		html.Append(" --></style>");

		html.AppendFormat("<p>An unhandled Exception occurred in the <b>{0}</b> application. ", System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
		html.Append("The following information may help resolve this issue:</p>");

		#region General Information
		html.Append(@"<h3>General Information</h3><div><table cellspacing=""0"">");
		if (errorid > 0)
		{
			html.Append(WriteLine("Error ID:", errorid.ToString()));
		}
		html.Append(WriteLine("Date and Time:", DateTime.Now.ToString("F")));
		html.Append(WriteLine("Machine Name:", Environment.MachineName));
		try
		{
			html.Append(WriteLine("Process User:", System.Security.Principal.WindowsIdentity.GetCurrent().Name));
		}
		catch (Exception exP)
		{
			html.Append(WriteLine("Process User:", String.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName)));
		}

		if (!string.IsNullOrEmpty(username))
		{
			html.Append(WriteLine("Remote User:", username));
		}
		html.Append(WriteLine("Remote Address:", context != null ? context.Request.ServerVariables["REMOTE_ADDR"] : "LOCAL"));
		html.Append(WriteLine("Remote Host:", context != null ? context.Request.ServerVariables["REMOTE_HOST"] : "LOCAL"));
		html.Append(WriteLine("URL:", String.Format(@"<a href=""{0}"">{0}</a>", context != null ? context.Request.Url : null), false));
		html.Append("</table></div>");
		//Exception Information
		html.Append(@"<h3>Exception Information</h3><div><table cellspacing=""0"">");
		html.Append(WriteLine("Exception Type:", ex.GetType().FullName));
		html.Append(WriteLine("Exception Message:", ex.Message));
		html.Append(WriteLine("Exception Source:", ex.Source));
		if (ex.TargetSite != null && !string.IsNullOrEmpty(ex.TargetSite.Name))
		{
			html.Append(WriteLine("Exception Target Site:", ex.TargetSite.Name));
		}
		if (!string.IsNullOrEmpty(ex.StackTrace))
		{
			html.Append(WriteLine("Stack Trace:", String.Format("<pre>{0}</pre>", ex.StackTrace), false));
		}
		if (ex.Data != null && ex.Data.Count > 0)
		{
			html.Append(WriteLine("Exception Data"));
			foreach (object key in ex.Data.Keys)
			{
				html.AppendFormat(WriteLine(key.ToString(), ex.Data[key].ToString()));
			}
		}
		#endregion

		#region Attempt to get the location in the file
		try
		{
			string[] locationInfo = ex.StackTrace.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
			string path = locationInfo[0][locationInfo[0].Length - 1] + ":" + locationInfo[1];
			int line = Int32.Parse(locationInfo[2].Substring(5));
			string[] lines = System.IO.File.ReadAllLines(path);
			StringBuilder fileLines = new StringBuilder(@"<pre>");
			if (line > 1)
			{
				fileLines.Append(WriteErrorLine((line - 2).ToString(), lines, false));
				fileLines.Append(WriteErrorLine((line - 1).ToString(), lines, false));
			}
			else if (line > 0)
			{
				fileLines.Append(WriteErrorLine((line - 1).ToString(), lines, false));
			}
			fileLines.Append(WriteErrorLine((line).ToString(), lines, true));
			if (line < lines.Length - 1)
			{
				fileLines.Append(WriteErrorLine((line + 1).ToString(), lines, false));
				fileLines.Append(WriteErrorLine((line + 2).ToString(), lines, false));
			}
			else if (line < lines.Length)
			{
				fileLines.Append(WriteErrorLine((line + 1).ToString(), lines, false));
			}
			fileLines.Append(@"</pre>");
			html.Append(WriteLine("File Info:", fileLines.ToString(), false));
		}
		catch (Exception)
		{
			//Don't really need to do anything here.
		}
		#endregion

		#region Grab any inner Exceptions so we get ALL the information.
		Exception innerEx = ex.InnerException;
		while (innerEx != null)
		{
			html.Append(WriteLine("Inner Exception"));
			html.Append(WriteLine("Exception Type:", innerEx.GetType().FullName));
			html.Append(WriteLine("Exception Message:", innerEx.Message));
			if (innerEx.TargetSite != null && !string.IsNullOrEmpty(innerEx.TargetSite.Name))
			{
				html.Append(WriteLine("Exception Target Site:", innerEx.TargetSite.Name));
			}
			if (!string.IsNullOrEmpty(innerEx.StackTrace))
			{
				html.Append(WriteLine("Stack Trace:", String.Format("<pre>{0}</pre>", innerEx.StackTrace), false));
			}
			if (innerEx.Data != null && innerEx.Data.Count > 0)
			{
				html.Append(WriteLine("Exception Data"));
				foreach (object key in innerEx.Data.Keys)
				{
					html.AppendFormat(WriteLine(key.ToString(), innerEx.Data[key].ToString()));
				}
			}
			innerEx = innerEx.InnerException;
		}
		html.Append("</table></div>");
		#endregion
			
		#region Collections Information
		//Collections Information
		html.Append(@"<h3>ASP.NET Collections</h3><div><table cellspacing=""0"">");
		if(context != null && context.Request.QueryString.Count > 0)
		{
			html.Append(WriteLine("QueryString"));
			foreach(string key in context.Request.QueryString.Keys)
			{
				html.AppendFormat(WriteLine(key, context.Request.QueryString[key]));
			}
		}
		if(context != null && context.Request.Form.Count > 0)
		{
			html.Append(WriteLine("Form"));
			foreach(string key in context.Request.Form.Keys)
			{
				html.AppendFormat(WriteLine(key, context.Request.Form[key]));
			}
		}
		if(context != null && context.Request.Cookies != null && context.Request.Cookies.Count > 0)
		{
			html.Append(WriteLine("Cookies"));
			foreach(string key in context.Request.Cookies.Keys)
			{
				if(context.Request.Cookies[key] == null)
				{
					html.AppendFormat(WriteLine(key, "&nbsp;"));
				}
				else
				{
					html.AppendFormat(WriteLine(key, context.Request.Cookies[key].Value));
				}
			}
		}
		if(context != null && context.Session != null && context.Session.Count > 0)
		{
			html.Append(WriteLine("Session"));
			foreach(string key in context.Session.Keys)
			{
				if(context.Session[key] == null)
				{
					html.AppendFormat(WriteLine(key, "&nbsp;"));
				}
				else
				{
					html.AppendFormat(WriteLine(key, context.Session[key].ToString()));
				}
			}
		}
		if(context != null && context.Application == null && context.Application.Count > 0)
		{
			html.Append(WriteLine("Application"));
			foreach(string key in context.Application.Keys)
			{
				if(context.Application[key] == null)
				{
					html.AppendFormat(WriteLine(key, "&nbsp;"));
				}
				else
				{
					html.AppendFormat(WriteLine(key, context.Application[key].ToString()));
				}
			}
		}
		if(context != null && context.Cache == null && context.Cache.Count > 0)
		{
			html.Append(WriteLine("Cache"));
			foreach(DictionaryEntry de in context.Cache)
			{
				if(de.Value == null)
				{
					html.AppendFormat(WriteLine(de.Key.ToString(), "&nbsp;"));
				}
				else
				{
					html.AppendFormat(WriteLine(de.Key.ToString(), de.Value.ToString()));
				}
			}
		}
		if(context != null && context.Request.ServerVariables.Count > 0)
		{
			html.Append(WriteLine("ServerVariables"));
			foreach(string key in context.Request.ServerVariables)
			{
				if(key != "ALL_HTTP" && key != "ALL_RAW")
				{
					html.AppendFormat(WriteLine(key, context.Request.ServerVariables[key]));
				}
			}
		}
		html.Append(@"</table></div>");
		#endregion Collections Information
			
		string body = html.ToString();

		return WTSUtility.Send_Email(toAddresses, null, null
			, subject, body, fromAddr, true, MailPriority.Normal);
			//#region Send the message
			//try 
			//{	        
			//	if(!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Error-EmailUsername"]))
			//	{
			//		Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\ITI\WTS", false);
			//		Byte[] pw = Convert.FromBase64String(rk.GetValue("Emailer").ToString());
			//		string password = Encoding.ASCII.GetString(ProtectedData.Unprotect(pw, null, WTSConfiguration.StoreLocation));
			//		client.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Error-EmailUsername"], password);
			//	}
			//	client.Send(message);
			//	return true;
			//}
			//catch (Exception)
			//{
			//	return false;
			//}
			//#endregion
	}

	/// <summary>
	/// Logs the message (for verbose output).
	/// </summary>
	/// <param name="message">The message to log.</param>
	public static void Log(string message)
	{
		LogMessage(message, TraceLevel.Info);
	}

	/// <summary>
	/// Logs the specified message.
	/// </summary>
	/// <param name="message">The message to log.</param>
	/// <param name="level">The level at which the message will be logged.</param>
	public static void LogMessage(string message, TraceLevel level)
	{
		if(LogLevel == TraceLevel.Off || level == TraceLevel.Off || string.IsNullOrEmpty(message)) { return; }

		if(level <= LogLevel)
		{
			//Make sure the user is one for which we want to create a log.
			string username = GetCurrentUserName(HttpContext.Current);
			if(string.IsNullOrEmpty(username))
			{
				username = "_anonymous_";
			}
			else
			{
				username = username.ToLower();
			}
			string userList = System.Configuration.ConfigurationManager.AppSettings["LogUsers"];
			if(!string.IsNullOrEmpty(userList))
			{
				List<string> users = new List<string>(userList.ToLower().Split(new Char[] {','}, StringSplitOptions.RemoveEmptyEntries));
				if(!users.Contains(username))
				{
					return;
				}
			}

			try 
			{	        
				
				string path = HttpContext.Current != null ? HttpContext.Current.Server.MapPath(@"~/Logs/" + username) : System.Web.Hosting.HostingEnvironment.MapPath(@"~/Logs/" + username);
				if(!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				path = String.Format(@"{0}\{1}.log", path, DateTime.Today.ToString("yyyyMMdd"));
				StackFrame[] frames = new StackTrace().GetFrames();
				int index = 1;
				while(true)
				{
					if(frames[index].GetMethod().DeclaringType != typeof(LogUtility))
					{
						break;
					}
					index += 1;
				}
				MethodBase method = frames[index].GetMethod();
				StringBuilder methodInfo = new StringBuilder();
				methodInfo.AppendFormat(@"{0} - {1}.{2}(", method.Module.Name, method.DeclaringType, method.Name);
				ParameterInfo[] parameters = method.GetParameters();
				for(int i = 0; i < parameters.Length; i++)
				{
					if(i > 0)
					{
						methodInfo.Append(", ");
					}
					methodInfo.Append(parameters[i].ParameterType.ToString());
				}
				methodInfo.Append(")");

                string newLine = Environment.NewLine != null ? Environment.NewLine : "\r\n";
                string url = HttpContext.Current != null && HttpContext.Current.Request != null ? HttpContext.Current.Request.Url.ToString() : "[INTERNAL]";
                string dt = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff");

                message = string.Format(@"{1} [{2}] - {3} - {4} - {5} : {6}{0}", newLine, dt, SessionID, level, url, methodInfo.ToString(), message);
                File.AppendAllText(path, message);
			}
			catch (Exception)
			{
				//Do nothing if it fails.
			}
		}
	}

	/// <summary>
	/// Logs the message (for informational output).
	/// </summary>
	/// <param name="message">The message to log.</param>
	public static void LogInfo(string message)
	{
		LogMessage(message, TraceLevel.Info);
	}

	/// <summary>
	/// Logs the message (for warning output).
	/// </summary>
	/// <param name="message">The message to log.</param>
	public static void LogWarning(string message)
	{
		LogMessage(message, TraceLevel.Warning);
	}

	/// <summary>
	/// Logs the message (for error output).
	/// </summary>
	/// <param name="message">The message to log.</param>
	public static void LogError(string message)
	{
		LogMessage(message, TraceLevel.Error);
	}

	/// <summary>
	/// Logs the exception.
	/// </summary>
	/// <param name="ex">The exception to log.</param>
	/// <returns>id of exception logged (-1 if log fails)</returns>
	public static long LogException(Exception ex, bool sendEmail = true)
	{
		long id = -1, lastid = -1;;

		try
		{
			while (ex != null && typeof(Exception) == typeof(HttpUnhandledException))
			{
				ex = ex.InnerException;
			}

			if (ex == null)
			{
				return -1;
			}

			//We need to grab this value so we can email it.
			Exception origEx = ex;

			//Some temp variables to remember the IDs.
			bool saved = false;
			string errorMsg = string.Empty;

			//Make sure the user is one for which we want to create a log.
			string username = GetCurrentUserName(HttpContext.Current);
			if (string.IsNullOrWhiteSpace(username))
			{
				username = "_anonymous_";
			}
			else
			{
				username = username.ToLower();
			}

			LogMessage lm = null;
			StringBuilder message = new StringBuilder();
            if (HttpContext.Current != null)
            {
                message.AppendLine(HttpContext.Current.Request.Url.ToString());
            }

			while (ex != null)
			{
				//Prepare the message to be logged to the database.
				lm = new LogMessage();
				lm.LogType = 5; //exception
				if (lastid > 0)
				{
					lm.ParentId = lastid;
				}
				lm.Username = username;
				lm.MessageDate = DateTime.Now.ToString();
				lm.ExceptionType = ex.GetType().ToString();
				lm.Message = ex.Message;
				lm.StackTrace = ex.StackTrace;
				lm.MessageSource = ex.Source;
				lm.AppVersion = string.Empty;
				lm.Url = HttpContext.Current != null ? HttpContext.Current.Request.Path : null;

				lastid = lm.Add(out saved, out errorMsg);
				if (id <= 0) { id = lastid; }

				//Prepare the message to be emailed
				message.AppendLine(ex.GetType().ToString());
				message.AppendLine(ex.Message);
				message.AppendLine(ex.StackTrace);
				ex = ex.InnerException;
				if (ex != null)
				{
					message.AppendLine();
				}
			}

			//email the exception if requested
			if (sendEmail)
			{
				EmailException(origEx, id);
			}
		}
		catch (Exception)
		{
		}
		
		return id;
	}

	/// <summary>
	/// Formats a string for the line of code that caused/threw the <see cref="Exception"/>.
	/// </summary>
	/// <param name="lineNumber">The line number to read.</param>
	/// <param name="allLines">All the lines in the file.</param>
	/// <param name="mainLine">A value indicating whether this is the line that threw the <see cref="Exception"/>.</param>
	/// <returns>A line for the email.</returns>
	private static string WriteErrorLine(string lineNumber, string[] allLines, bool	mainLine = false)
	{
		string space = string.Empty, spanStart = string.Empty, spanEnd = string.Empty;

		//This will format the numbers so that they all line up (in cases such as 99 and 100).
		int totalLength = allLines.Length.ToString().Length;
		int numLength = lineNumber.ToString().Length;
		while( numLength < totalLength)
		{
			space += " ";
			numLength += 1;
		}

		if(mainLine)
		{
			spanStart = @"<span style=""color:#ff0000"">";
			spanEnd = @"</span>";
		}
		
		return string.Format(@"{0}Line {1}{2}: {3}{4}{5}", spanStart, space, lineNumber, HttpUtility.HtmlEncode(allLines[int.Parse(lineNumber) - 1].Replace((char)9, '	')), spanEnd, Environment.NewLine);
	}

	/// <summary>
	/// Formats a line for the email.
	/// </summary>
	/// <param name="title">The sub-title of the next group of lines.</param>
	/// <returns>A line for the email.</returns>
	private static string WriteLine(string title)
	{
		return string.Format(@"<tr><th class=""sub-title"" colspan=""2"">{0}</th></tr>", title);
	}

	/// <summary>
	/// Formats a line for the email.
	/// </summary>
	/// <param name="key">The key of the value.</param>
	/// <param name="value">The value.</param>
	/// <param name="replaceTags">A value indicating whether to HTML-encode the value.</param>
	/// <returns>A line for the email.</returns>
	private static string WriteLine(string key, string value, bool replaceTags = true)
	{
		if(string.IsNullOrEmpty(value))
		{
			return string.Empty;
		}
		if( replaceTags)
		{            
			value = HttpUtility.HtmlEncode(value);
		}
		return string.Format("<tr><th>{0}</th><td>{1}</td></tr>", key, value);
	}

	/// <summary>
	/// Logs the email.
	/// </summary>
	/// <param name="ex">The email to log.</param>
	/// <returns>id of email logged (-1 if log fails)</returns>
	public static long LogEmail(string toAddresses, string ccAddresses, string bccAddresses, string subject, string body, string fromAddress, string procedureUsed, string errorMessage)
	{
		long id = -1;

		try
		{
			//Some temp variables to remember the IDs.
			bool saved = false;
			string errorMsg = string.Empty;

			LogEmail le = null;

			//Prepare the email to be logged to the database.
			le = new LogEmail();

			le.StatusId = 1;
			le.Sender = fromAddress;
			le.ToAddresses = toAddresses;
			le.CcAddresses = ccAddresses;
			le.BccAddresses = bccAddresses;
			le.Subject = subject;
			le.Body = body;
			le.Procedure_Used = procedureUsed;
			le.ErrorMessage = errorMessage;

			id = le.Add(out saved, out errorMsg);
		}
		catch (Exception)
		{
		}

		return id;
	}
}


public class LogMessage
{
	#region Properties

	public int ID { get; set; }
	public int LogType { get; set; }
	public long ParentId { get; set; }
	public string Username { get; set; }
	public string MessageDate { get; set; }
	public string ExceptionType { get; set; }
	public string Message { get; set; }
	public string StackTrace { get; set; }
	public string MessageSource { get; set; }
	public string AppVersion { get; set; }
	public string Url { get; set; }
	public string AdditionalInfo { get; set; }
	public string MachineName { get; set; }
	public string ProcessName { get; set; }
	public string CreatedBy { get; set; }
	public string CreatedDate { get; set; }

	#endregion

	public LogMessage() { }
	/// <summary>
	/// Supplying an id value will automatically load the message if it exists in the database
	/// </summary>
	/// <param name="id"></param>
	public LogMessage(int id)
	{
		this.ID = id;
		this.Load();
	}


	public int Add(out bool saved, out string errorMsg)
	{
		saved = false; errorMsg = string.Empty;
		SqlParameter newID = null;

		try
		{
			string procName = "LogMessage_Add";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@Log_TypeID", SqlDbType.Decimal).Value = (int)this.LogType;
					cmd.Parameters.Add("@ParentMessageId", SqlDbType.Decimal).Value = this.ParentId;
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = this.Username;
					cmd.Parameters.Add("@MessageDate", SqlDbType.DateTime2).Value = DateTime.Now;
					cmd.Parameters.Add("@ExceptionType", SqlDbType.NVarChar).Value = this.ExceptionType;
					cmd.Parameters.Add("@Message", SqlDbType.Text).Value = this.Message;
					cmd.Parameters.Add("@StackTrace", SqlDbType.Text).Value = this.StackTrace;
					cmd.Parameters.Add("@MessageSource", SqlDbType.NVarChar).Value = this.MessageSource;
					cmd.Parameters.Add("@AppVersion", SqlDbType.NVarChar).Value = this.AppVersion;
					cmd.Parameters.Add("@Url", SqlDbType.NVarChar).Value = this.Url;
					cmd.Parameters.Add("@AdditionalInfo", SqlDbType.Text).Value = this.AdditionalInfo;
					cmd.Parameters.Add("@MachineName", SqlDbType.NVarChar).Value = this.MachineName;
					cmd.Parameters.Add("@ProcessName", SqlDbType.NVarChar).Value = this.ProcessName;
					cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current == null || string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? "SYSTEM_ADMIN" : HttpContext.Current.User.Identity.Name;

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
						errorMsg = "Failed to create new LogMessage.";
					}
				}
			}
		}
		catch (Exception ex)
		{ //log error message
			//LogUtility.LogException(ex);
			this.ID = 0;
			saved = false;
			errorMsg = ex.Message.ToString();
		}
		return this.ID;
	}

	public bool Load(int id)
	{
		this.ID = id;
		return this.Load();
	}
	public bool Load()
	{
		string procName = "LogMessage_Load";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@Log_MessageId", SqlDbType.Decimal).Value = this.ID;

				try
				{
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows)
					{ //populate properties
						dr.Read();

						this.LogType = dr["LOG_TYPEID"] == DBNull.Value ? 2 : int.Parse(dr["LOG_TYPEID"].ToString());
						this.ParentId = dr["ParentMessageId"] == DBNull.Value ? 0 : int.Parse(dr["ParentMessageId"].ToString());
						this.Username = dr["Username"].ToString();
						this.MessageDate = dr["MessageDate"].ToString();
						this.ExceptionType = dr["ExceptionType"].ToString();
						this.Message = dr["Message"].ToString();
						this.StackTrace = dr["StackTrace"].ToString();
						this.MessageSource = dr["MessageSource"].ToString();
						this.AppVersion = dr["AppVersion"].ToString();
						this.Url = dr["Url"].ToString();
						this.AdditionalInfo = dr["AdditionalInfo"].ToString();
						this.MachineName = dr["MachineName"].ToString();
						this.ProcessName = dr["ProcessName"].ToString();
						this.CreatedBy = dr["CreatedBy"].ToString();
						this.CreatedDate = dr["CreatedDate"].ToString();

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
}

public class LogEmail
{
	#region Properties

	public int ID { get; set; }
	public int StatusId { get; set; }
	public string Sender { get; set; }
	public string ToAddresses { get; set; }
	public string CcAddresses { get; set; }
	public string BccAddresses { get; set; }
	public string Subject { get; set; }
	public string Body { get; set; }
	public string Procedure_Used { get; set; }
	public string ErrorMessage { get; set; }
	public string CreatedBy { get; set; }
	public string CreatedDate { get; set; }

	#endregion

	public LogEmail() { }
	/// <summary>
	/// Supplying an id value will automatically load the email if it exists in the database
	/// </summary>
	/// <param name="id"></param>
	public LogEmail(int id)
	{
		this.ID = id;
		this.Load();
	}


	public int Add(out bool saved, out string errorMsg)
	{
		saved = false; errorMsg = string.Empty;
		SqlParameter newID = null;

		try
		{
			string procName = "LogEmail_Add";

			DataTable dt = new DataTable();
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@StatusId", SqlDbType.Decimal).Value = (int)this.StatusId;
					cmd.Parameters.Add("@Sender", SqlDbType.NVarChar).Value = this.Sender;
					cmd.Parameters.Add("@ToAddresses", SqlDbType.NVarChar).Value = this.ToAddresses;
					cmd.Parameters.Add("@CcAddresses", SqlDbType.NVarChar).Value = this.CcAddresses;
					cmd.Parameters.Add("@BccAddresses", SqlDbType.NVarChar).Value = this.BccAddresses;
					cmd.Parameters.Add("@Subject", SqlDbType.NVarChar).Value = this.Subject;
					cmd.Parameters.Add("@Body", SqlDbType.Text).Value = this.Body;
					cmd.Parameters.Add("@SentDate", SqlDbType.DateTime2).Value = DateTime.Now;
					cmd.Parameters.Add("@Procedure_Used", SqlDbType.NVarChar).Value = this.Procedure_Used;
					cmd.Parameters.Add("@ErrorMessage", SqlDbType.Text).Value = this.ErrorMessage;
					cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current != null ? HttpContext.Current.User.Identity.Name : "SYSTEM";

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
						errorMsg = "Failed to create new LogEmail.";
					}
				}
			}
		}
		catch (Exception ex)
		{ //log error message
			//LogUtility.LogException(ex);
			this.ID = 0;
			saved = false;
			errorMsg = ex.Message.ToString();
		}
		return this.ID;
	}

	public bool Load(int id)
	{
		this.ID = id;
		return this.Load();
	}
	public bool Load()
	{
		string procName = "LogEmail_Load";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@Log_EmailId", SqlDbType.Decimal).Value = this.ID;

				try
				{
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows)
					{ //populate properties
						dr.Read();

						this.StatusId = dr["STATUSID"] == DBNull.Value ? 0 : int.Parse(dr["STATUSID"].ToString());
						this.Sender = dr["Sender"].ToString();
						this.ToAddresses = dr["ToAddresses"].ToString();
						this.CcAddresses = dr["CcAddresses"].ToString();
						this.BccAddresses = dr["BccAddresses"].ToString();
						this.Subject = dr["Subject"].ToString();
						this.Body = dr["Body"].ToString();
						this.Procedure_Used = dr["Procedure_Used"].ToString();
						this.ErrorMessage = dr["ErrorMessage"].ToString();
						this.CreatedBy = dr["CreatedBy"].ToString();
						this.CreatedDate = dr["CreatedDate"].ToString();

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
}