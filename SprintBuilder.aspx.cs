﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using System.Linq;
using System.IO;

using Newtonsoft.Json;

public partial class SprintBuilder : System.Web.UI.Page
{
    #region "Member Variables"
    protected bool CanEditWorkloadMGMT = false;
    protected int _productID = 0;
    protected int _sessionID = 0;
    protected string[] _msSystems = { };
    protected string _selectedSystems = ",31,";
    #endregion

    #region "Page Methods"
    [WebMethod]
    public static string Save(string strSprintSave)
    {
        var result = new Dictionary<string, string> { { "saved", "" }, { "error", "" } };
        var saved = false;
        var errorMsg = string.Empty;

        try
        {

            XmlDocument docSprintBuilder = (XmlDocument)JsonConvert.DeserializeXmlNode(strSprintSave, "sprint");

            saved = AOR.SprintBuilder_Save(SprintBuilderSave: docSprintBuilder);

            result["saved"] = saved.ToString();
            result["error"] = errorMsg;
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result);
    }

    [WebMethod()]
    public static string GetHistory(string strWorkItemID, string strWorkItemTaskID)
    {
        DataTable dt = new DataTable();

        try
        {
            int intWorkItemID = -1;
            int intWorkItemTaskID = -1;

            if (strWorkItemID != "") { int.TryParse(strWorkItemID, out intWorkItemID); }
            if (strWorkItemTaskID != "") { int.TryParse(strWorkItemTaskID, out intWorkItemTaskID); }

            dt = AOR.SprintBuilderHistory_Get(WORKITEMID: intWorkItemID, WORKITEM_TASKID: intWorkItemTaskID);

            dt.Columns.Add("CREATEDDATESTRING");
            dt.Columns.Add("UPDATEDDATESTRING");

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                DateTime nDate = new DateTime();

                if (DateTime.TryParse(dt.Rows[i]["CREATEDDATE"].ToString(), out nDate))
                {
                    dt.Rows[i]["CREATEDDATESTRING"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                }

                if (DateTime.TryParse(dt.Rows[i]["UPDATEDDATE"].ToString(), out nDate))
                {
                    dt.Rows[i]["UPDATEDDATESTRING"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }
    #endregion

    #region "Page_Load"
    private void readQueryString()
    {
        if (Request.QueryString["productID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["productID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["productID"].ToString()), out this._productID);
        }
        if (Request.QueryString["sessionID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sessionID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["sessionID"].ToString()), out this._sessionID);
        }
        if (Request.QueryString["selectedSystems"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["selectedSystems"].ToString()))
        {
            this._msSystems = Request.QueryString["selectedSystems"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            _selectedSystems = Request.QueryString["selectedSystems"].ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.CanEditWorkloadMGMT = UserManagement.UserCanEdit(WTSModuleOption.WorkloadMGMT);

        readQueryString();
        LoadControls();
        LoadData();
    }
    #endregion

    #region "Load Data"
    private void LoadControls()
    {
        DataTable dtProductVersion = MasterData.ProductVersionList_Get(includeArchive: false);

        ddlProductVersion.DataSource = dtProductVersion;
        ddlProductVersion.DataValueField = "ProductVersionID";
        ddlProductVersion.DataTextField = "ProductVersion";
        ddlProductVersion.DataBind();
        ddlProductVersion.SelectedValue = _productID.ToString();

        DataTable dtSession = MasterData.ReleaseSessionList_Get(productVersionID: _productID, includeArchive: false);

        dtSession.Columns["ReleaseSession"].ReadOnly = false;

        if (dtSession.Rows.Count > 0 && dtSession.Rows[0]["ReleaseSession"].ToString() == "") dtSession.Rows[0]["ReleaseSession"] = "Any or not included in a session";

        ddlSession.DataSource = dtSession;
        ddlSession.DataValueField = "ReleaseSessionID";
        ddlSession.DataTextField = "ReleaseSession";
        ddlSession.DataBind();
        ddlSession.SelectedValue = _sessionID.ToString();

        DataTable dtSystem = MasterData.SystemList_Get(includeArchive: false, cv: "0", ProductVersionID: _productID);

        dtSystem.DefaultView.RowFilter = "WTS_SystemID <> 0";
        dtSystem = dtSystem.DefaultView.ToTable();

 

        if (dtSystem != null)
        {
            msWebsys.Items.Clear();

            foreach (DataRow dr in dtSystem.Rows)
            {
                ListItem li = new ListItem(dr["WTS_SYSTEM"].ToString(), dr["WTS_SystemID"].ToString());
                if (_msSystems.Length == 0)
                {
                    if (dr["WTS_SystemID"].ToString() == "31")
                    {
                        li.Selected = true;
                    }
                }
                else
                {
                    li.Selected = (_msSystems.Count() == 0 || _msSystems.Contains(dr["WTS_SystemID"].ToString()));

                }
                li.Attributes.Add("OptionGroup", dr["WTS_SystemSuite"].ToString());
                msWebsys.Items.Add(li);
            }
        }
    }
    private void LoadData()
    {
        DataTable dt = AOR.SprintBuilder_Get(ProductVersionID: _productID, ReleaseSessionID: _sessionID, WebsysIDs: String.Join(",", _selectedSystems));
        DataView dvWorkloadMgmt = new DataView(dt);
        DataTable dtWorkloadMgmt = dvWorkloadMgmt.ToTable(true, "AORReleaseID", "AORName");

        StringBuilder sbGrid = new StringBuilder();
        StringBuilder sbMainTable = new StringBuilder();
        StringBuilder sbWorkloadMgmt = new StringBuilder();
        StringBuilder sbWebsystem = new StringBuilder();
        StringBuilder sbTask = new StringBuilder();

        StringBuilder sbTr = new StringBuilder();
        StringBuilder sbTd = new StringBuilder();

        StringBuilder sbTrTaskHeaders = new StringBuilder();
        sbTrTaskHeaders.Append("<tr class=\"gridHeader\">");
        sbTrTaskHeaders.Append("<td style=\"display: none\">WORKITEMID</td>");
        sbTrTaskHeaders.Append("<td style=\"display: none\">WORKITEM_TASKID</td>");
        sbTrTaskHeaders.Append("<td style=\"width: 25px;\"></td>");
        sbTrTaskHeaders.Append("<td style=\"width: 75px; font-weight: bold\">Work Task #</td>");
        sbTrTaskHeaders.Append("<td style=\"width: 300px; font-weight: bold\">Title</td>");
        sbTrTaskHeaders.Append("<td style=\"width: 100px; font-weight: bold\">Workload MGMT AOR</td>");
        sbTrTaskHeaders.Append("<td style=\"width: 50px; font-weight: bold\">Status</td>");

        if (_sessionID > 0)
        {
            sbTrTaskHeaders.Append("<td style=\"width: 50px; font-weight: bold\">Closed In This Session</td>");
            sbTrTaskHeaders.Append("<td style=\"width: 50px; font-weight: bold\">New In This Session</td>");
        }
        
        sbTrTaskHeaders.Append("<td style=\"font-weight: bold\">Justification</td>");
        sbTrTaskHeaders.Append("</tr>");

        sbMainTable.Append("<table cellspacing=\"0\" cellpadding=\"0\" style=\"width: 98% \">");

        for (int i = 0; i < dtWorkloadMgmt.Rows.Count; i++) //Loop through distinct Workload MGMT AOR
        {
            DataTable dtFilterWorkMgmt = dt.Copy(); //Filter datatable by Workload MGMT Aor
            dtFilterWorkMgmt.DefaultView.RowFilter = "AORReleaseID = '" + dtWorkloadMgmt.Rows[i]["AORReleaseID"] + "'";
            dtFilterWorkMgmt = dtFilterWorkMgmt.DefaultView.ToTable();

            DataView dvWebsystem = new DataView(dtFilterWorkMgmt);
            DataTable dtWebsystem = dvWebsystem.ToTable(true, new string[] { "WTS_SYSTEMID", "WTS_SYSTEM" });

            sbTr.Clear();
            sbTd.Clear();

            sbTr.Append("<tr class=\"gridHeader\" id=\"trWorkMgmt_" + dtWorkloadMgmt.Rows[i]["AORReleaseID"].ToString() + "\">");
            sbTd.Append("<td id=\"tdAORReleaseID\" style=\"display: none \">" + dtWorkloadMgmt.Rows[i]["AORReleaseID"] + "</td>");
            sbTd.Append("<td id=\"tdWorkMgmt_" + dtWorkloadMgmt.Rows[i]["AORName"].ToString() + "\" colspan=\"3\">");
            sbTd.Append("<span style=\"font-weight: bold\">" + dtWorkloadMgmt.Rows[i]["AORName"].ToString() + "</span>");
            sbTd.Append("</td>");

            sbMainTable.Append(sbTr);
            sbMainTable.Append(sbTd);
            sbMainTable.Append("</tr>");

            for (int j = 0; j < dtWebsystem.Rows.Count; j++) //Loop through distinct Websystem from Workload MGMT AOR
            {
                DataTable dtFilterWebsystem = dtFilterWorkMgmt.Copy();
                dtFilterWebsystem.DefaultView.RowFilter = "WTS_SYSTEMID = " + dtWebsystem.Rows[j]["WTS_SYSTEMID"];
                dtFilterWebsystem = dtFilterWebsystem.DefaultView.ToTable();

                sbTr.Clear();
                sbTd.Clear();

                sbTr.Append("<tr class=\"gridBody\" id=\"trWebsys_" + dtWorkloadMgmt.Rows[i]["AORName"].ToString() + "_" + dtWebsystem.Rows[j]["WTS_SYSTEMID"] + "\">");
                sbTd.Append("<td id=\"tdExpandCollapse\" releaseID=\"" + dtWorkloadMgmt.Rows[i]["AORReleaseID"] + "\" systemID=\"" + dtWebsystem.Rows[j]["WTS_SYSTEMID"] + "\" style=\"width: 25px\"><img class=\"toggleSection\" src=\"Images/Icons/add_blue.png\" title=\"Show Section\" alt=\"Show Section\" height=\"12\" width=\"12\" data-section=\"Systems\" style=\"cursor: pointer;\" /></td>");
                sbTd.Append("<td id=\"tdReleaseID\" style=\"display: none\">" + dtWorkloadMgmt.Rows[i]["AORReleaseID"] + "</td>");
                sbTd.Append("<td id=\"tdWTS_SYSTEMID\" style=\"display: none\">" + dtWebsystem.Rows[j]["WTS_SYSTEMID"].ToString() + "</td>");
                sbTd.Append("<td id=\"tdWebsys_" + dtWorkloadMgmt.Rows[i]["AORName"].ToString() + "_" + dtWebsystem.Rows[j]["WTS_SYSTEMID"] + "\">");
                sbTd.Append("<span>" + dtWebsystem.Rows[j]["WTS_SYSTEM"].ToString() + "</span>");
                sbTd.Append("</td>");

                sbMainTable.Append(sbTr);
                sbMainTable.Append(sbTd);
                sbMainTable.Append("</tr>");

                StringBuilder sbWebsysDiv = new StringBuilder();
                sbWebsysDiv.Append("<div id=\"divMgmtWebsys\" releaseID=\"" + dtWorkloadMgmt.Rows[i]["AORReleaseID"] + "\" systemID=\"" + dtWebsystem.Rows[j]["WTS_SYSTEMID"] + "\" style =\"height: 300px; width: 99%; overflow: auto; display: none\">");
                sbWebsysDiv.Append("<table cellspacing=\"0\" cellpadding=\"0\">");
                sbMainTable.Append("<tr><td colspan=\"3\">" + sbWebsysDiv);

                sbMainTable.Append(sbTrTaskHeaders);

                for (int k = 0; k < dtFilterWebsystem.Rows.Count; k++) //Loop through distinct Tasks by Websystem for Workload MGMT AOR
                {
                    string strTaskNumber;

                    if (dtFilterWebsystem.Rows[k]["TASK_NUMBER"].ToString().Length > 0)
                    {
                        strTaskNumber = dtFilterWebsystem.Rows[k]["TASK_NUMBER"].ToString();
                    }
                    else
                    {
                        strTaskNumber = dtFilterWebsystem.Rows[k]["WORKITEMID"].ToString();
                    }

                    sbTr.Clear();
                    sbTd.Clear();

                    sbTr.Append("<tr class=\"gridBody\" id=\"trTasks_" + dtWorkloadMgmt.Rows[i]["AORName"].ToString() + "_" + dtWebsystem.Rows[j]["WTS_SYSTEMID"] + "\">");
                    sbTd.Append("<td>&nbsp;&nbsp;</td>");
                    sbTd.Append("<td id=\"tdWORKITEMID\" style=\"display: none\">" + dtFilterWebsystem.Rows[k]["WORKITEMID"] + "</td>");
                    sbTd.Append("<td id=\"tdWORKITEM_TASKID\" style=\"display: none\">" + dtFilterWebsystem.Rows[k]["WORKITEM_TASKID"] + "</td>");
                    sbTd.Append("<td id=\"tdWorkTaskNo\" style=\" text-align: center\">");
                    sbTd.Append("<span if=\"spnWorkTaskNo\" style=\"cursor: pointer; text-decoration:underline; color: blue\" onclick=\"spnWorkTaskNo(this) \">" + strTaskNumber + "</span>");
                    sbTd.Append("</td>");
                    sbTd.Append("<td id=\"tdTitle_" + k + "\">");
                    sbTd.Append("<span>" + dtFilterWebsystem.Rows[k]["TITLE"] + "</span>");
                    sbTd.Append("</td>");

                    sbTd.Append("<td style=\"text-align: center\">");

                    if (dtFilterWebsystem.Rows[k]["WORKITEM_TASKID"].ToString() != "" && dtFilterWebsystem.Rows[k]["CascadeAOR"].ToString() == "True")
                    {
                        sbTd.Append(dtWorkloadMgmt.Rows[i]["AORName"].ToString() + " (Cascaded)");
                    }
                    else
                    {
                        //Workload Mgmt Dropdown List
                        sbTd.Append("<select name=\"ddlWorkMgmt_" + dtWorkloadMgmt.Rows[i]["AORName"].ToString() + "_" + dtWebsystem.Rows[j]["WTS_SYSTEMID"] + " \">"); //select

                        sbTd.Append("<option value=\"-1\"");
                        sbTd.Append(">--Select--</option>");

                        foreach (DataRow dr in dtWorkloadMgmt.Rows)
                        {
                            sbTd.Append("<option value=\"" + dr["AORReleaseID"] + "\" ");
                            if (dr["AORReleaseID"].ToString() == dtWorkloadMgmt.Rows[i]["AORReleaseID"].ToString()) sbTd.Append(" selected");
                            sbTd.Append(">" + dr["AORName"] + "</option>");
                        }

                        sbTd.Append("</select></td>");
                        //End Workload Mgmt Dropdown list
                    }

                    sbTd.Append("<td id=\"tdStatus_" + k + "\">");
                    sbTd.Append("<span>" + dtFilterWebsystem.Rows[k]["Status"] + "</span>");
                    sbTd.Append("</td>");

                    if (_sessionID > 0)
                    {
                        sbTd.Append("<td id=\"tdClosedInSession_" + k + "\" style=\"text-align: center\">");
                        sbTd.Append("<span>" + dtFilterWebsystem.Rows[k]["ClosedInSession"] + "</span>");
                        sbTd.Append("</td>");
                        sbTd.Append("<td id=\"tdNewInSession_" + k + "\" style=\"text-align: center\">");
                        sbTd.Append("<span>" + dtFilterWebsystem.Rows[k]["NewInSession"] + "</span>");
                        sbTd.Append("</td>");
                    }
                    
                    sbTd.Append("<td id=\"tdJustification_" + k + "\" style=\"width: 700px\" >");
                    sbTd.Append("<input name=\"txtJustification_" + k + "\" type=\"text\" value=\"" + dtFilterWebsystem.Rows[k]["Justification"] + "\" style=\"width: 99%\" />");
                    sbTd.Append("</td>");
                    sbTd.Append("<td id=\"tdAORReleaseTaskID\" style=\"display: none\">" + dtFilterWebsystem.Rows[k]["AORReleaseTaskID"] + "</td>");
                    sbTd.Append("<td id=\"tdAORReleaseSubTaskID\" style=\"display: none\">" + dtFilterWebsystem.Rows[k]["AORReleaseSubTaskID"] + "</td>");

                    sbMainTable.Append(sbTr);
                    sbMainTable.Append(sbTd);
                    sbMainTable.Append("</tr>");
                }
                sbMainTable.Append("</table></div></td></tr>");
            }
        }

        sbGrid.Append(sbMainTable + "</table>");
        divGrid.InnerHtml = sbGrid.ToString();
    }

    #endregion
}