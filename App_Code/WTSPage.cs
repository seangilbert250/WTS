using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace WTS
{
    public class WTSPage : System.Web.UI.Page
    {
        protected MembershipUser LoggedInMembershipUser;
        protected string LoggedInMembershipUserID;
        protected int LoggedInUserID;
        protected bool IsAdmin;
        protected bool IsUserAdmin;

        protected WTS_User LoggedInUser;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            LoggedInMembershipUser = Membership.GetUser();
            LoggedInMembershipUserID = LoggedInMembershipUser.ProviderUserKey.ToString();
            IsAdmin = Roles.IsUserInRole("Admin");

            LoggedInUser = new WTS_User();
            LoggedInUser.Load(LoggedInMembershipUserID);
            LoggedInUserID = LoggedInUser.ID;
            IsUserAdmin = UserManagement.UserIsUserAdmin(LoggedInUserID);

        }

        public static int GetLoggedInUserID()
        {               
            return GetLoggedInUser().ID;
        }

        public static WTS_User GetLoggedInUser()
        {
            MembershipUser LoggedInMembershipUserStatic = Membership.GetUser();
            string LoggedInMembershipUserIDStatic = LoggedInMembershipUserStatic.ProviderUserKey.ToString();
            WTS_User LoggedInUserStatic = new WTS_User();
            LoggedInUserStatic.Load(LoggedInMembershipUserIDStatic);

            return LoggedInUserStatic;
        }

        public static Dictionary<string, string> CreateDefaultResult()
        {
            Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "false" }, { "error", "" } };

            return result;
        }

        public static string SerializeResult(Dictionary<string, string> result)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
        }

        public static string SerializeResult(Dictionary<string, object> result)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
        }

        public static string SerializeResult(DataTable dt)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
        }

        public static string SerializeResult(DataSet ds)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(ds, Newtonsoft.Json.Formatting.None);
        }

        protected void PopulateDDLFromDataTable(object ddl, DataTable dt, string valueColumn, string textColumn, string defaultValue, string defaultText, bool excludeZeroOrBlankValueFromDataTable, string groupColumn = null)
        {
            if (ddl != null)
            {
                if (defaultValue != null && defaultText != null) // we don't check whitespace because user may want to have a blank default text / value item
                {
                    ListItem li = new ListItem(defaultText, defaultValue);

                    if (ddl is HtmlSelect)
                    {
                        ((HtmlSelect)ddl).Items.Add(li);
                    }
                    else if (ddl is DropDownList)
                    {
                        ((DropDownList)ddl).Items.Add(li);
                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    string lastGroup = null;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];

                        string value = row[valueColumn].ToString();
                        string text = row[textColumn].ToString();
                        string group = groupColumn != null ? row[groupColumn].ToString() : null;

                        if (excludeZeroOrBlankValueFromDataTable && (value == "0" || value == ""))
                        {
                            continue;
                        }

                        if (groupColumn != null)
                        {
                            if (!string.IsNullOrWhiteSpace(group) && group != lastGroup)
                            {
                                ListItem gi = new ListItem(group, "0");
                                gi.Attributes["style"] = "color:#aaaaaa";
                                if (ddl is HtmlSelect)
                                {
                                    ((HtmlSelect)ddl).Items.Add(gi);
                                }
                                else if (ddl is DropDownList)
                                {
                                    ((DropDownList)ddl).Items.Add(gi);
                                }
                            }

                            lastGroup = group;
                        }

                        ListItem li = new ListItem(text, value);
                        if (ddl is HtmlSelect)
                        {
                            ((HtmlSelect)ddl).Items.Add(li);
                        }
                        else if (ddl is DropDownList)
                        {
                            ((DropDownList)ddl).Items.Add(li);
                        }
                    }
                }
            }
        }

        protected string CreateOptionStringFromDataTable(DataTable dt, string valueColumn, string textColumn, string defaultValue, string defaultText, bool excludeZeroOrBlankValueFromDataTable, string groupColumn = null)
        {
            StringBuilder options = new StringBuilder();

            if (defaultValue != null && defaultText != null) // we don't check whitespace because user may want to have a blank default text / value item
            {
                options.Append("<option value=\"" + defaultValue + "\">").Append(defaultText).Append("</option>");
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                string lastGroup = null;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];

                    string value = row[valueColumn].ToString();
                    string text = row[textColumn].ToString();
                    string group = groupColumn != null ? row[groupColumn].ToString() : null;

                    if (excludeZeroOrBlankValueFromDataTable && (value == "0" || value == "-1" || value == ""))
                    {
                        continue;
                    }

                    if (groupColumn != null)
                    {
                        if (!string.IsNullOrWhiteSpace(group) && group != lastGroup)
                        {
                            options.Append("<option value=\"0\" style=\"background-color:#ffffff;color:#aaaaaa;\" disabled>").Append(WTS.Util.StringUtil.StripHTML(group)).Append("</option>");            
                        }

                        lastGroup = group;
                    }

                    options.Append("<option value=\"" + value + "\">").Append(WTS.Util.StringUtil.StripHTML(text)).Append("</option>");
                }
            }

            return options.ToString();
        }

        protected string CreateCheckBoxStringFromDataTable(DataTable dt, string valueColumn, string textColumn, bool excludeZeroOrBlankValueFromDataTable, int cols, string groupColumn)
        {
            StringBuilder cbs = new StringBuilder();
                        
            if (dt != null && dt.Rows.Count > 0)
            {
                string lastGroup = null;
                int colCount = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];

                    string value = row[valueColumn].ToString();
                    string text = row[textColumn].ToString();
                    string group = groupColumn != null ? row[groupColumn].ToString() : null;

                    if (excludeZeroOrBlankValueFromDataTable && (value == "0" || value == "-1" || value == ""))
                    {
                        continue;
                    }

                    if (i > 0)
                    {
                        cbs.Append("<br />");
                    }
                    
                    if (groupColumn != null)
                    {
                        if (!string.IsNullOrWhiteSpace(group) && group != lastGroup)
                        {
                            cbs.Append("<nobr>").Append(group).Append("</nobr><br />");
                        }

                        lastGroup = group;
                    }

                    cbs.Append("<nobr><input type=\"checkbox\" value=\"" + value + "\">&nbsp;<span onclick=\"$(this).prev('input').click();\">").Append(WTS.Util.StringUtil.StripHTML(text)).Append("</span></nobr>");
                    colCount++;
                }
            }

            return cbs.ToString();
        }
    }
}