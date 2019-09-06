﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_Resource_Team : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected int AORID = 0;
    protected int AORReleaseID = 0;
    protected int ReleaseID = 0;
    protected int RowCount = 0;
    protected string UsersOptions = string.Empty;
    protected string RoleOptions = string.Empty;
    protected string[] QFRelease = { };
    protected string QFReleaseGroups = string.Empty;

    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEditAOR = (UserManagement.UserCanEdit(WTSModuleOption.AOR) && AOR.AORReleaseCurrent(AORID: this.AORID, AORReleaseID: this.AORReleaseID));

        LoadData();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
            || Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
        {
            this.MyData = true;
        }
        else
        {
            this.MyData = false;
        }

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.AORID);
        }

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.AORReleaseID);
        }
    }

    #endregion

    #region Data
    private void LoadData()
    {
        DataTable dtResources = new DataTable();
        DataTable dtUsers = new DataTable();
        DataTable dtRoles = new DataTable();
        StringBuilder sbResources = new StringBuilder();
        StringBuilder sbActionTeam = new StringBuilder();
        StringBuilder sbAddActionTeamGrid = new StringBuilder();


        sbResources.Append("<table style=\"border-collapse: collapse;\"><tr class=\"gridHeader\">");
        sbActionTeam.Append("<table style=\"border-collapse: collapse;\"><tr class=\"gridHeader\">");
        sbAddActionTeamGrid.Append("<div style=\"text-align:right;padding-bottom:3px;padding-top:3px;padding-right:3px;\"><a href=\"javascript: popupManager.GetPopupByName('AddResource').Opener.closeResourceSelections();\" style =\"padding-right: 3px;\"><input type=\"button\" value=\"Cancel\"></a>");
        sbAddActionTeamGrid.Append("<a href=\"javascript: popupManager.GetPopupByName('AddResource').Opener.actionTeamPopupSelectionsSaved();\"><input type=\"button\" value=\"Save Selections\"></a></div>");
        sbAddActionTeamGrid.Append("<div style=\"height:100%;overflow:auto;background-color:#ffffff;\">");

        if (CanEditAOR)
        {
            dtUsers = UserManagement.LoadUserList(0, false, true, "", true, AORReleaseID);
            DataRow row = dtUsers.NewRow();
            row["WTS_RESOURCEID"] = 0;
            row["IsApproved"] = true;
            row["IsLockedOut"] = true;
            row["ORGANIZATIONID"] = 0;
            row["ORGANIZATION"] = "";
            row["RESOURCETYPEID"] = 0;
            row["RESOURCETYPE"] = "";
            row["First_Name"] = "";
            row["Last_Name"] = "";
            row["Archive"] = 0;
            row["CreatedBy"] = "WTS";
            row["CreatedDate"] = DateTime.Today.ToString();
            row["UpdatedBy"] = "WTS";
            row["UpdatedDate"] = DateTime.Today.ToString();
            row["Resource"] = "";
            dtUsers.Rows.InsertAt(row, 0);
            dtRoles = AOR.AORRoleList_Get();

            foreach (DataRow dr in dtUsers.Rows)
            {
                UsersOptions += "<option value=\"" + dr["WTS_ResourceID"] + "\">" + Uri.EscapeDataString(dr["USERNAME"].ToString()) + "</option>";
            }

            foreach (DataRow dr in dtRoles.Rows)
            {
                if (string.IsNullOrWhiteSpace(RoleOptions))
                {
                    RoleOptions += "<option value=\"0\"></option>";
                }

                RoleOptions += "<option value=\"" + dr["AORRole_ID"] + "\">" + Uri.EscapeDataString(dr["Role"].ToString()) + "</option>";
            }

            sbActionTeam.Append("<th style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 50px;\"><a href=\"\" onclick=\"addActionTeamLinkClicked(); return false;\" style=\"color: blue;\"></a>"); //anchor
        }
        else
        {
            sbActionTeam.Append("<th style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 155px;\">");
        }
        sbResources.Append("<th style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 155px;\">Resource");
        sbResources.Append("</th>");
        sbResources.Append("<th style=\"border-top: 1px solid grey; text-align: center; width: 200px;\">Resource Type</th>");
        sbResources.Append("</tr>");

        sbActionTeam.Append("<th style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 155px;\">Resource</th>");
        sbActionTeam.Append("<th style=\"border-top: 1px solid grey; text-align: center; width: 200px;\">Resource Type</th>");
        sbActionTeam.Append("<th style=\"border-top: 1px solid grey; text-align: center; width: 200px;\">System</th>");
        sbActionTeam.Append("<th style=\"border-top: 1px solid grey; text-align: center; width: 125px;\">How Maintained</th>");
        sbActionTeam.Append("</tr>");

        sbAddActionTeamGrid.Append("<table style=\"border-collapse: collapse;border:1px solid grey;\" width=\"100%\" id=\"divAORActionTeamPopupTable\">");
        sbAddActionTeamGrid.Append("<tr class=\"gridHeader\">");
        sbAddActionTeamGrid.Append("<th style=\"border-top: 1px solid grey; text-align: center; width: 25px;\">&nbsp;</th>");
        sbAddActionTeamGrid.Append("<th style=\"border-top: 1px solid grey; text-align: left; width: 150px;padding-left:3px;\">Resource</th>");
        sbAddActionTeamGrid.Append("<th style=\"border-top: 1px solid grey; text-align: left; width: 150px;padding-left:3px;\">Resource Type</th>");
        sbAddActionTeamGrid.Append("</tr>");
        sbAddActionTeamGrid.Append("<tr><th colspan=\"3\" style=\"border-bottom: 1px solid grey; text-align: center;\">AOR Associated Resources</th></tr>");
        sbAddActionTeamGrid.Append("<tr><th colspan=\"3\" style=\"border-bottom: 1px solid grey; text-align: center;\">Non-AOR Associated Resources</th></tr>");

        dtResources = AOR.AORResourceList_Get(AORID: AORID, AORReleaseID: AORReleaseID);
        HashSet<string> aorResources = new HashSet<string>();
        Dictionary<string, DataRow> assignedResources = new Dictionary<string, DataRow>();

        if (dtResources != null && dtResources.Rows.Count > 0)
        {
            int.TryParse(dtResources.Rows[0]["ProductVersionID"].ToString(), out ReleaseID);
            for (int i = 0; i < dtResources.Rows.Count; i++)
            {
                sbResources.Append("<tr class=\"gridBody\">");

                if (CanEditAOR)
                {
                    sbResources.Append("<td style=\"border-left: 1px solid grey; text-align: center;\">" + dtResources.Rows[i]["Resource"]);
                    sbResources.Append("</td>");

                    sbResources.Append("<td style=\"text-align:center\">" + dtResources.Rows[i]["Resource Type"]);
                    sbResources.Append("</td>");
                }
                else
                {
                    sbResources.Append("<td style=\"border-left: 1px solid grey;\">" + dtResources.Rows[i]["Resource"] + "</td><td>" + dtResources.Rows[i]["Resource Type"] + "</td>");
                }

                sbResources.Append("</td></tr>");
                aorResources.Add(dtResources.Rows[i]["WTS_RESOURCE_ID"].ToString());
            }
        }
        else
        {
            sbResources.Append("<tr class=\"gridBody\"><td colspan=\"5\" style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center;\">No Resources</td></tr>");
        }

        int rowIdx = 0;
        dtResources = AOR.AORResourceTeamList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID);
        assignedResources.Clear();

        rowIdx = 0;
        foreach (DataRow dr in dtUsers.Rows)
        {
            DataRow arRow = assignedResources.ContainsKey(dr["WTS_ResourceID"].ToString()) ? assignedResources[dr["WTS_ResourceID"].ToString()] : null;
            if ((bool)dr["Archive"] && arRow == null || dr["WTS_RESOURCEID"].ToString() == "0") // only show archived resources if they were previously assigned to this aor
            {
                continue;
            }

            sbAddActionTeamGrid.Append("<tr origsort=\"" + rowIdx + "\" class=\"gridBody\">");
            sbAddActionTeamGrid.Append("<td style=\"text-align:center\"><input name=\"cbAddActionTeam\" type=\"checkbox\" value=\"" + dr["WTS_ResourceID"] + "\" onchange=\"popupManager.GetPopupByName('AddResource').Opener.sortResourceCheckboxes($(this).closest('table'))\"></td>");
            sbAddActionTeamGrid.Append("<td style=\"text-align:left;padding-left:3px;\">" + dr["USERNAME"] + "</td>");
            sbAddActionTeamGrid.Append("<td style=\"text-align:left;padding-left:3px;\">" + dr["RESOURCETYPE"] + "</td>");
            sbAddActionTeamGrid.Append("</tr>");

            rowIdx++;
        }

        if (dtResources != null && dtResources.Rows.Count > 0)
        {
            if (dtUsers != null && dtUsers.Rows.Count > 0)
            {
                dtUsers.DefaultView.RowFilter = string.Format("NOT WTS_RESOURCEID = 0 ");
                dtUsers = dtUsers.DefaultView.ToTable();

                foreach (DataRow dr in dtResources.Rows)
                {
                    dtUsers.DefaultView.RowFilter = string.Format("NOT WTS_RESOURCEID = {0} ", dr["WTS_RESOURCEID"].ToString());
                    dtUsers = dtUsers.DefaultView.ToTable();
                }
            }

            for (int i = 0; i < dtResources.Rows.Count; i++)
            {
                assignedResources.Add(dtResources.Rows[i]["WTS_RESOURCEID"].ToString(), dtResources.Rows[i]);
                aorResources.Add(dtResources.Rows[i]["WTS_RESOURCEID"].ToString());

                sbActionTeam.Append("<tr class=\"gridBody\">");

                if (CanEditAOR)
                {
                    sbActionTeam.Append("<td style=\"border-left: 1px solid grey; text-align: center;\"><a href=\"\" onclick=\"removeResource(this, true); return false;\" style=\"color: blue; " + (dtResources.Rows[i]["System"].ToString().Length > 0 ? "display: none;" : "") + "\">Remove</a>"); //anchor
                    sbActionTeam.Append("</td><td style=\"text-align: center;\">");
                    sbActionTeam.Append("<select field=\"ActionTeam\" row=\"" + i + "\" original_value=\"" + dtResources.Rows[i]["WTS_RESOURCEID"] + "\" style=\"width: 95%; background-color: #F5F6CE;\">"); //select
                    sbActionTeam.Append("<option value=\"" + dtResources.Rows[i]["WTS_RESOURCEID"] + "\" selected>" + dtResources.Rows[i]["Resource"] + "</option>");

                    foreach (DataRow dr in dtUsers.Rows)
                    {
                        sbActionTeam.Append("<option value=\"" + dr["WTS_ResourceID"] + "\"");
                        sbActionTeam.Append(">" + dr["USERNAME"] + "</option>");
                    }

                    sbActionTeam.Append("</select>");
                    sbActionTeam.Append("</td>");

                    sbActionTeam.Append("<td style=\"border-left: 1px solid grey;\">" + dtResources.Rows[i]["Resource Type"]);
                    sbActionTeam.Append("<td style=\"border-left: 1px solid grey;\">" + dtResources.Rows[i]["System"]);

                    if (dtResources.Rows[i]["ResourceSync"].ToString() == "1") sbActionTeam.Append("<td style=\"border-left: 1px solid grey; text-align: center;\">Sync");
                    else if (dtResources.Rows[i]["System"].ToString().Length > 0) sbActionTeam.Append("<td style=\"border-left: 1px solid grey; text-align: center;\">Intake Team");
                    else sbActionTeam.Append("<td style=\"border-left: 1px solid grey; text-align: center;\">Manually Added");


                }
                else
                {
                    sbActionTeam.Append("<td style=\"border-left: 1px solid grey;\">" + dtResources.Rows[i]["Resource"]);
                }

                sbActionTeam.Append("</td></tr>");
            }
        }
        else
        {
            if (CanEditAOR)
            {
                sbActionTeam.Append("<tr class=\"gridBody\"><td colspan=\"5\" style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center;\">No Resources</td></tr>");
            }
            else
            {
                sbActionTeam.Append("<tr class=\"gridBody\"><td colspan=\"5\" style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center;\">No Resources</td></tr>");
            }
        }

        RowCount = aorResources.Count;

        sbResources.Append("</table>");
        sbActionTeam.Append("</table>");
        sbAddActionTeamGrid.Append("</table></div>");
        divAORResources.InnerHtml = sbResources.ToString();
        divAORActionTeam.InnerHtml = sbActionTeam.ToString();
        divAORActionTeamPopup.InnerHtml = sbAddActionTeamGrid.ToString();
    }
    #endregion

    #region AJAX
    [WebMethod]
    public static string Save(string aor, string resources, string actionTeam)
    {
        Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

        try
        {
            int AOR_ID = 0;
            XmlDocument docResources = (XmlDocument)JsonConvert.DeserializeXmlNode(resources, "resources");
            XmlDocument docActionTeam = (XmlDocument)JsonConvert.DeserializeXmlNode(actionTeam, "actionteam");

            int.TryParse(aor, out AOR_ID);

            result = AOR.AOR_Resource_Save(AORID: AOR_ID, Resources: docResources, ActionTeam: docActionTeam);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result);
    }

    [WebMethod(true)]
    public static string SyncActionTeam(int AORReleaseID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            if (AORReleaseID > 0)
            {
                result = AOR.AOR_Resource_Sync(AORReleaseID: AORReleaseID);
            }
        }
        catch (Exception ex)
        {
            saved = false;
            errorMsg = ex.Message;
            LogUtility.LogException(ex);
            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result);
    }
    #endregion
}