﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Newtonsoft.Json;

public partial class AOR_Edit : Page
{
	#region Variables
	private bool MyData = true;
	protected bool CanEditAOR = false;
	protected bool NewAOR = false;
	protected int AORID = 0;
    protected int AORReleaseID = 0;
    protected int systemCount = 0;
    protected int primarySystemContractID = 0;
    protected string SystemOptions = string.Empty;
    protected int _showOverride = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
	{
		ReadQueryString();

        this.CanEditAOR = (UserManagement.UserCanEdit(WTSModuleOption.AOR) && AOR.AORReleaseCurrent(AORID: this.AORID, AORReleaseID: this.AORReleaseID));

        LoadControls();
		LoadData();
        LoadAOREstimation();

    }

	private void ReadQueryString()
	{
		if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
			|| Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
		{
			MyData = true;
		}
		else
		{
			MyData = false;
		}

		if (Request.QueryString["NewAOR"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewAOR"]))
		{
			bool.TryParse(Request.QueryString["NewAOR"], out NewAOR);
		}

		if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
		{
			int.TryParse(Request.QueryString["AORID"], out AORID);
		}

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.AORReleaseID);
        }
    }
	#endregion

	#region Data
	private void LoadControls()
	{
		DataTable dtEstimatedEffort = MasterData.EffortSizeList_Get(includeArchive: false);

		ddlCodingEffort.DataSource = dtEstimatedEffort;
        ddlCodingEffort.DataValueField = "EffortSizeID";
        ddlCodingEffort.DataTextField = "EffortSize";
        ddlCodingEffort.DataBind();

        ddlTestingEffort.DataSource = dtEstimatedEffort;
        ddlTestingEffort.DataValueField = "EffortSizeID";
        ddlTestingEffort.DataTextField = "EffortSize";
        ddlTestingEffort.DataBind();

        ddlTrainingSupportEffort.DataSource = dtEstimatedEffort;
        ddlTrainingSupportEffort.DataValueField = "EffortSizeID";
        ddlTrainingSupportEffort.DataTextField = "EffortSize";
        ddlTrainingSupportEffort.DataBind();

        DataTable dtProductVersion = MasterData.ProductVersionList_Get(includeArchive: false);

		ddlProductVersion.DataSource = dtProductVersion;
		ddlProductVersion.DataValueField = "ProductVersionID";
		ddlProductVersion.DataTextField = "ProductVersion";
		ddlProductVersion.DataBind();

        DataTable dtStatus = MasterData.StatusList_Get(includeArchive: false);

        dtStatus.Columns.Add("StatusDescription");

        foreach (DataRow dr in dtStatus.Rows)
        {
            if (dr["StatusType"].ToString() != "") dr["StatusDescription"] = dr["Status"].ToString() + " - " + dr["DESCRIPTION"].ToString();
        }

        dtStatus.AcceptChanges();

        DataTable dtSystemContract = new DataTable();
        string currentContractID = null;
        int insertCount = 0;

        dtSystemContract = MasterData.WTS_System_ContractList_Get();

        ddlPrimarySystem.DataSource = dtSystemContract;
        ddlPrimarySystem.DataValueField = "WTS_SystemID";
        ddlPrimarySystem.DataTextField = "WTS_System";
        ddlPrimarySystem.DataBind();

        foreach (DataRow dr in dtSystemContract.Rows)
        {
            if (dr["CONTRACTID"].ToString() != currentContractID && dr["CONTRACTID"].ToString() != "0")
            {
                ListItem suiteLi = new ListItem(dr["CONTRACT"].ToString());
                suiteLi.Attributes.Add("disabled", "disabled");
                suiteLi.Attributes.Add("style", "background-color: white");
                ddlPrimarySystem.Items.Insert(dtSystemContract.Rows.IndexOf(dr) + insertCount, suiteLi);
                currentContractID = dr["CONTRACTID"].ToString();
                insertCount++;
            }
        }

        DataTable dtAORSystems = AOR.AORSystemList_Get(AORID: AORID, AORReleaseID: AORReleaseID);
        dtAORSystems.DefaultView.RowFilter = "Primary IN (True)";
        dtAORSystems = dtAORSystems.DefaultView.ToTable();

        if (dtAORSystems != null && dtAORSystems.Rows.Count > 0)
        {
            int systemID = 0;
            int.TryParse(dtAORSystems.Rows[0]["WTS_SYSTEM_ID"].ToString(), out systemID);
            dtSystemContract.DefaultView.RowFilter = "WTS_SYSTEMID IN ('0', " + systemID + ")";
            dtSystemContract = dtSystemContract.DefaultView.ToTable();

            ddlPrimarySystem.SelectedValue = systemID.ToString();
        }

        primarySystemContractID = 0;
        if (dtSystemContract != null && dtSystemContract.Rows.Count > 1)
        {
            int.TryParse(dtSystemContract.Rows[1]["CONTRACTID"].ToString(), out primarySystemContractID);
        }

        DataTable dtWorkloadAllocation = MasterData.WorkloadAllocationList_Get(includeArchive: 0);
        //DataTable dtWorkloadAllocationContract = MasterData.WorkloadAllocation_ContractList_Get();
        //dtWorkloadAllocationContract.DefaultView.RowFilter = "CONTRACTID IN (0, " + primarySystemContractID + ")";
        //dtWorkloadAllocationContract.DefaultView.Sort = "ContractID";
        //dtWorkloadAllocationContract = dtWorkloadAllocationContract.DefaultView.ToTable();

        ddlWorkloadAllocation.DataSource = dtWorkloadAllocation;
        ddlWorkloadAllocation.DataValueField = "WorkloadAllocationID";
        ddlWorkloadAllocation.DataTextField = "WorkloadAllocation";
        ddlWorkloadAllocation.DataBind();

        //string currentContract = string.Empty;
        //insertCount = 0;
        //foreach (DataRow waContract in dtWorkloadAllocationContract.Rows)
        //{
        //    if (waContract["ContractID"].ToString() != currentContract && waContract["ContractID"].ToString() != "0")
        //    {
        //        ListItem contractLi = new ListItem(waContract["Contract"].ToString());
        //        contractLi.Attributes.Add("disabled", "disabled");
        //        contractLi.Attributes.Add("style", "background-color: white");
        //        ddlWorkloadAllocation.Items.Insert(dtWorkloadAllocationContract.Rows.IndexOf(waContract) + insertCount, contractLi);
        //        currentContract = waContract["ContractID"].ToString();
        //        insertCount++;
        //    }
        //}

        DataTable dtIP = dtStatus.Copy();

        dtIP.DefaultView.RowFilter = "StatusType IN ('', 'IP')";
        dtIP = dtIP.DefaultView.ToTable();
        ddlIP1.DataSource = dtIP;
        ddlIP1.DataValueField = "StatusID";
        ddlIP1.DataTextField = "Status";
        ddlIP1.DataBind();

        ddlIP2.DataSource = dtIP;
        ddlIP2.DataValueField = "StatusID";
        ddlIP2.DataTextField = "Status";
        ddlIP2.DataBind();

        ddlIP3.DataSource = dtIP;
        ddlIP3.DataValueField = "StatusID";
        ddlIP3.DataTextField = "Status";
        ddlIP3.DataBind();

        DataTable dtCMMI = dtStatus.Copy();

		dtCMMI.DefaultView.RowFilter = "StatusType IN ('', 'CMMI')";
		dtCMMI = dtCMMI.DefaultView.ToTable();
        dtCMMI.Columns.Add("Text");

        foreach (DataRow dr in dtCMMI.Rows)
        {
            if (dr["StatusType"].ToString() != "") dr["Text"] = dr["Status"].ToString() + " (" + dr["DESCRIPTION"].ToString() + ")";
        }
        ddlCMMI.DataSource = dtCMMI;
		ddlCMMI.DataValueField = "StatusID";
		ddlCMMI.DataTextField = "Text";
		ddlCMMI.DataBind();

        DataTable dtCyber = dtStatus.Copy();

        dtCyber.DefaultView.RowFilter = "StatusType IN ('CR')";
        dtCyber = dtCyber.DefaultView.ToTable();

        dtCyber.Columns.Add("Text");
        
        foreach (DataRow dr in dtCyber.Rows)
        {
            if (dr["StatusType"].ToString() != "") dr["Text"] = dr["SORT_ORDER"].ToString() + " - " + dr["Status"].ToString() + " (" + dr["DESCRIPTION"].ToString() + ")";
        }
        ddlCyber.DataSource = dtCyber;
        ddlCyber.DataValueField = "StatusID";
        ddlCyber.DataTextField = "Text";
        ddlCyber.DataBind();

        DataTable dtStopLight = dtStatus.Copy();

        dtStopLight.DefaultView.RowFilter = "StatusType IN ('', 'StopLight')";
        dtStopLight = dtStopLight.DefaultView.ToTable();

        ddlStopLight.DataSource = dtStopLight;
        ddlStopLight.DataValueField = "StatusID";
        ddlStopLight.DataTextField = "Status";
        ddlStopLight.DataBind();

        DataTable dtAORStatus = dtStatus.Copy();

        dtAORStatus.DefaultView.RowFilter = "StatusType IN ('', 'AORStatus')";
        dtAORStatus = dtAORStatus.DefaultView.ToTable();

        ddlAORStatus.DataSource = dtAORStatus;
        ddlAORStatus.DataValueField = "StatusID";
        ddlAORStatus.DataTextField = "Status";
        ddlAORStatus.DataBind();

        DataTable dtTeam = AOR.AORTeamList_Get();

		ddlCriticalPathTeam.DataSource = dtTeam;
		ddlCriticalPathTeam.DataValueField = "AORTeam_ID";
		ddlCriticalPathTeam.DataTextField = "Team";
		ddlCriticalPathTeam.DataBind();

		ListItem li = new ListItem();

		li.Value = "0";
		li.Text = "";

		ddlCriticalPathTeam.Items.Insert(0, li);

		DataTable dtWorkType = AOR.AORWorkTypeList_Get();

		ddlWorkType.DataSource = dtWorkType;
		ddlWorkType.DataValueField = "AORWorkType_ID";
		ddlWorkType.DataTextField = "Work Type";
		ddlWorkType.DataBind();

		li = new ListItem();

		li.Value = "0";
		li.Text = "";

		ddlWorkType.Items.Insert(0, li);

        DataTable dtInvestigation = dtStatus.Copy();

        dtInvestigation.DefaultView.RowFilter = "StatusType IN ('', 'Inv')";
        dtInvestigation = dtInvestigation.DefaultView.ToTable();
        ddlInvestigation.DataSource = dtInvestigation;
        ddlInvestigation.DataValueField = "StatusID";
        ddlInvestigation.DataTextField = "StatusDescription";
        ddlInvestigation.DataBind();

        DataTable dtTechnical = dtStatus.Copy();

        dtTechnical.DefaultView.RowFilter = "StatusType IN ('', 'TD')";
        dtTechnical = dtTechnical.DefaultView.ToTable();
        ddlTechnical.DataSource = dtTechnical;
        ddlTechnical.DataValueField = "StatusID";
        ddlTechnical.DataTextField = "StatusDescription";
        ddlTechnical.DataBind();

        DataTable dtCustomerDesign = dtStatus.Copy();

        dtCustomerDesign.DefaultView.RowFilter = "StatusType IN ('', 'CD')";
        dtCustomerDesign = dtCustomerDesign.DefaultView.ToTable();
        ddlCustomerDesign.DataSource = dtCustomerDesign;
        ddlCustomerDesign.DataValueField = "StatusID";
        ddlCustomerDesign.DataTextField = "StatusDescription";
        ddlCustomerDesign.DataBind();

        DataTable dtCoding = dtStatus.Copy();

        dtCoding.DefaultView.RowFilter = "StatusType IN ('', 'C')";
        dtCoding = dtCoding.DefaultView.ToTable();
        ddlCoding.DataSource = dtCoding;
        ddlCoding.DataValueField = "StatusID";
        ddlCoding.DataTextField = "StatusDescription";
        ddlCoding.DataBind();

        DataTable dtInternalTesting = dtStatus.Copy();

        dtInternalTesting.DefaultView.RowFilter = "StatusType IN ('', 'IT')";
        dtInternalTesting = dtInternalTesting.DefaultView.ToTable();
        ddlInternalTesting.DataSource = dtInternalTesting;
        ddlInternalTesting.DataValueField = "StatusID";
        ddlInternalTesting.DataTextField = "StatusDescription";
        ddlInternalTesting.DataBind();

        DataTable dtCustomerVerificationTesting = dtStatus.Copy();

        dtCustomerVerificationTesting.DefaultView.RowFilter = "StatusType IN ('', 'CVT')";
        dtCustomerVerificationTesting = dtCustomerVerificationTesting.DefaultView.ToTable();
        ddlCustomerVerificationTesting.DataSource = dtCustomerVerificationTesting;
        ddlCustomerVerificationTesting.DataValueField = "StatusID";
        ddlCustomerVerificationTesting.DataTextField = "StatusDescription";
        ddlCustomerVerificationTesting.DataBind();

        DataTable dtAdoption = dtStatus.Copy();

        dtAdoption.DefaultView.RowFilter = "StatusType IN ('', 'Adopt')";
        dtAdoption = dtAdoption.DefaultView.ToTable();
        ddlAdoption.DataSource = dtAdoption;
        ddlAdoption.DataValueField = "StatusID";
        ddlAdoption.DataTextField = "StatusDescription";
        ddlAdoption.DataBind();

        DataTable dtPriority = MasterData.PriorityList_Get(includeArchive: false);

        dtPriority.DefaultView.RowFilter = "PRIORITYTYPE IN ('', 'AOR')";
        dtPriority = dtPriority.DefaultView.ToTable();
        ddlCriticality.DataSource = dtPriority;
        ddlCriticality.DataValueField = "PriorityID";
        ddlCriticality.DataTextField = "Priority";
        ddlCriticality.DataBind();

        ddlCustomerValue.DataSource = dtPriority;
        ddlCustomerValue.DataValueField = "PriorityID";
        ddlCustomerValue.DataTextField = "Priority";
        ddlCustomerValue.DataBind();

        ddlRisk.DataSource = dtPriority;
        ddlRisk.DataValueField = "PriorityID";
        ddlRisk.DataTextField = "Priority";
        ddlRisk.DataBind();

        ddlLevelOfEffort.DataSource = dtPriority;
        ddlLevelOfEffort.DataValueField = "PriorityID";
        ddlLevelOfEffort.DataTextField = "Priority";
        ddlLevelOfEffort.DataBind();

        var sessionMethods = new SessionMethods();
        bool chk = false;
        if (sessionMethods.Session["AOREditSystems"] != null && sessionMethods.Session["AOREditSystems"].ToString().Length > 0)
        {
            bool.TryParse(sessionMethods.Session["AOREditSystems"].ToString(), out chk);
            chkSettingsAORSystems.Checked = chk;
        }

        if (sessionMethods.Session["AOREditDetails"] != null && sessionMethods.Session["AOREditDetails"].ToString().Length > 0)
        {
            bool.TryParse(sessionMethods.Session["AOREditDetails"].ToString(), out chk);
            chkSettingsAORDetails.Checked = chk;
        }

        if (sessionMethods.Session["AOREditEstimation"] != null && sessionMethods.Session["AOREditEstimation"].ToString().Length > 0)
        {
            bool.TryParse(sessionMethods.Session["AOREditEstimation"].ToString(), out chk);
            chkSettingsAOREstimation.Checked = chk;
        }

        if (sessionMethods.Session["AOREditCMMI"] != null && sessionMethods.Session["AOREditCMMI"].ToString().Length > 0)
        {
            bool.TryParse(sessionMethods.Session["AOREditCMMI"].ToString(), out chk);
            chkSettingsCMMIDetails.Checked = chk;
        }

        if (sessionMethods.Session["AOREditCRs"] != null && sessionMethods.Session["AOREditCRs"].ToString().Length > 0)
        {
            bool.TryParse(sessionMethods.Session["AOREditCRs"].ToString(), out chk);
            chkSettingsCRs.Checked = chk;
        }

        if (sessionMethods.Session["AOREditTasks"] != null && sessionMethods.Session["AOREditTasks"].ToString().Length > 0)
        {
            bool.TryParse(sessionMethods.Session["AOREditTasks"].ToString(), out chk);
            chkSettingsTasks.Checked = chk;
        }

        if (sessionMethods.Session["AOREditResources"] != null && sessionMethods.Session["AOREditResources"].ToString().Length > 0)
        {
            bool.TryParse(sessionMethods.Session["AOREditResources"].ToString(), out chk);
            chkSettingsResources.Checked = chk;
        }

        if (sessionMethods.Session["AOREditHistory"] != null && sessionMethods.Session["AOREditHistory"].ToString().Length > 0)
        {
            bool.TryParse(sessionMethods.Session["AOREditHistory"].ToString(), out chk);
            chkSettingsHistory.Checked = chk;
        }

        if (sessionMethods.Session["AOREditSystems"] == null &&
            sessionMethods.Session["AOREditDetails"] == null &&
            sessionMethods.Session["AOREditCMMI"] == null &&
            sessionMethods.Session["AOREditCRs"] == null &&
            sessionMethods.Session["AOREditTasks"] == null &&
            sessionMethods.Session["AOREditResources"] == null &&
            sessionMethods.Session["AOREditHistory"] == null)
        {
            chkSettingsAORSystems.Checked = true;
            chkSettingsAORDetails.Checked = true;
        }
    }

	private void LoadData()
	{
        DataTable dtAORSystems = new DataTable();
        StringBuilder sbSystems = new StringBuilder();
        DataTable dtSystems = new DataTable();
		DataTable dtUsers = new DataTable();
        DataTable dtRoles = new DataTable();

        string currentSuiteID = null;

        sbSystems.Append("<table style=\"border-collapse: collapse;\"><tr class=\"gridHeader\" style=\"border-top: 0px;\">");
        
        if (CanEditAOR)
		{
            dtSystems = MasterData.SystemList_Get(includeArchive: false, cv: "0");
            dtSystems.DefaultView.RowFilter = "WTS_SYSTEM <> ''";
            dtSystems.DefaultView.RowFilter = "WTS_SYSTEMID NOT IN (0, " + ddlPrimarySystem.SelectedValue + ")";
            dtSystems.DefaultView.Sort = "WTS_SystemSuite, WTS_SYSTEM";
            dtSystems = dtSystems.DefaultView.ToTable();

            foreach (DataRow dr in dtSystems.Rows)
            {
                if (dr["WTS_SystemSuiteID"].ToString() != currentSuiteID)
                {
                    SystemOptions += "<option style=\"background-color: white;\" disabled>" + Uri.EscapeDataString(dr["WTS_SystemSuite"].ToString()) + "</option>";
                }

                SystemOptions += "<option value=\"" + dr["WTS_SYSTEMID"] + "\">" + Uri.EscapeDataString(dr["WTS_SYSTEM"].ToString()) + "</option>";
                currentSuiteID = dr["WTS_SystemSuiteID"].ToString();
            }

            sbSystems.Append("<th style=\"text-align: center; width: 50px;\"><a href=\"\" onclick=\"addSystemLinkClicked(); return false;\" style=\"color: blue;\">Add</a>"); //anchor
            sbSystems.Append("</th><th style=\"border-top: 0px; border-right: 0px; text-align: center; width: 350px;\">");
		}
		else
		{
            sbSystems.Append("<th style=\"text-align: center; border-right: 0px; width: 350px;\">");
		}

        sbSystems.Append("Other Systems</th></tr>");

        if (!NewAOR)
        {
            dtAORSystems = AOR.AORSystemList_Get(AORID: AORID, AORReleaseID: AORReleaseID);
            systemCount = dtAORSystems.Rows.Count;
            dtAORSystems.DefaultView.RowFilter = "Primary IN (False)";
            dtAORSystems = dtAORSystems.DefaultView.ToTable();
        }

        if (dtAORSystems != null && dtAORSystems.Rows.Count > 0)
        {
            for (int i = 0; i < dtAORSystems.Rows.Count; i++)
            {
                sbSystems.Append("<tr class=\"gridBody\">");

                if (CanEditAOR)
                {
                    sbSystems.Append("<td style=\"text-align: center;" + (i == dtAORSystems.Rows.Count - 1 ? "border-bottom: 0px;" : "") + "\"><a href=\"\" onclick=\"removeSystem(this); return false;\" style=\"color: blue;\">Remove</a>"); //anchor
                    sbSystems.Append("</td><td style=\"text-align: center;" + (i == dtAORSystems.Rows.Count - 1 ? "border-bottom: 0px;" : "") + "\">");
                    sbSystems.Append("<select field=\"System\" original_value=\"" + dtAORSystems.Rows[i]["WTS_SYSTEM_ID"] + "\" style=\"width: 99%; background-color: #F5F6CE;\">"); //select

                    currentSuiteID = null;

                    foreach (DataRow dr in dtSystems.Rows)
                    {
                        if (dr["WTS_SystemSuiteID"].ToString() != currentSuiteID)
                        {
                            sbSystems.Append("<option style=\"background-color: white;\" disabled>" + dr["WTS_SystemSuite"].ToString() + "</option>");
                        }

                        sbSystems.Append("<option value=\"" + dr["WTS_SYSTEMID"] + "\"");

                        if (dr["WTS_SYSTEMID"].ToString() == dtAORSystems.Rows[i]["WTS_SYSTEM_ID"].ToString()) sbSystems.Append(" selected");

                        sbSystems.Append(">" + dr["WTS_SYSTEM"] + "</option>");

                        currentSuiteID = dr["WTS_SystemSuiteID"].ToString();
                    }

                    sbSystems.Append("</select>");
                }
                else
                {
                    sbSystems.Append("<td>" + dtAORSystems.Rows[i]["System"]);
                }

                sbSystems.Append("</td></tr>");
            }
        }
        else
        {
            if (CanEditAOR)
            {
                sbSystems.Append("<tr class=\"gridBody\"><td colspan=\"3\" style=\"border-top: 1px solid grey; border-bottom: 0px; text-align: center;\">No Systems</td></tr>");
            }
            else
            {
                sbSystems.Append("<tr class=\"gridBody\"><td colspan=\"2\" style=\"border-top: 1px solid grey; border-bottom: 0px; border-right: 0px; text-align: center;\">No Systems</td></tr>");
            }
        }

        sbSystems.Append("</table>");
        divAORSystems.InnerHtml = sbSystems.ToString();
        
        if (!NewAOR)
		{
			DataTable dtHistory = AOR.AORRelease_History_Get(aorReleaseID: AORReleaseID.ToString());

            if (dtHistory != null && dtHistory.Rows.Count > 0)
            {
                StringBuilder sbHistory = new StringBuilder();

                sbHistory.Append("<table style=\"border-collapse: collapse; text-align: center;\">");
                sbHistory.Append("<tr class=\"gridHeader\"><th style=\"border-top: 1px solid grey; border-left: 1px solid grey;\">Field Changed</th><th style=\"border-top: 1px solid grey;\">Old Value</th><th style=\"border-top: 1px solid grey;\">New Value</th><th style=\"border-top: 1px solid grey;\">Updated</th></tr>");

                for (int i = 0; i < dtHistory.Rows.Count; i++)
                {
                    string dateDisplay = string.Empty;
                    DateTime nDate = new DateTime();

                    if (DateTime.TryParse(dtHistory.Rows[i]["CREATEDDATE"].ToString(), out nDate))
                    {
                        dateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    }

                    sbHistory.Append("<tr class=\"gridBody\"><td style=\"border-left: 1px solid grey;\">" + dtHistory.Rows[i]["FieldChanged"] + "</td><td style=\"width: 40%;\">" + dtHistory.Rows[i]["OldValue"] + "</td><td style=\"width: 40%;\">" + dtHistory.Rows[i]["NewValue"] + "</td><td style=\"text-align: center;\">" + dtHistory.Rows[i]["CREATEDBY"] + "<br>" + dateDisplay + "</td></tr>");
                }

                sbHistory.Append("</table>");
                divAORHistory.InnerHtml = sbHistory.ToString();
            }

            dtHistory = AOR.AORList_Get(AORID: AORID, includeArchive: 1);

            if (dtHistory != null && dtHistory.Rows.Count > 0)
            {
                dtHistory.DefaultView.RowFilter = (AORReleaseID > 0 ? ("AORRelease_ID = " + AORReleaseID) : "Current_ID = 1");
                dtHistory = dtHistory.DefaultView.ToTable();

				if (dtHistory.Rows.Count > 0)
				{
					spnAOR.InnerText = dtHistory.Rows[0]["AOR #"].ToString();

                    string createdDateDisplay = string.Empty, updatedDateDisplay = string.Empty;
                    DateTime nCreatedDate = new DateTime(), nUpdatedDate = new DateTime();

                    if (DateTime.TryParse(dtHistory.Rows[0]["CreatedDate_ID"].ToString(), out nCreatedDate)) createdDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nCreatedDate);
                    if (DateTime.TryParse(dtHistory.Rows[0]["UpdatedDate_ID"].ToString(), out nUpdatedDate)) updatedDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nUpdatedDate);

                    spnCreated.InnerText = "Created: " + dtHistory.Rows[0]["CreatedBy_ID"].ToString() + " - " + createdDateDisplay;
                    spnUpdated.InnerText = "Updated: " + dtHistory.Rows[0]["UpdatedBy_ID"].ToString() + " - " + updatedDateDisplay;
                    txtAORName.Text = dtHistory.Rows[0]["AOR Name"].ToString();
                    txtAORName.Attributes.Add("original_value", dtHistory.Rows[0]["AOR Name"].ToString());
                    txtDescription.Text = dtHistory.Rows[0]["Description"].ToString();
					txtNotes.Text = dtHistory.Rows[0]["Notes_ID"].ToString();
                    chkApproved.Checked = dtHistory.Rows[0]["Approved_ID"].ToString() == "True";

                    if (chkApproved.Checked)
                    {
                        lblApprovedBy.Text = "<b>By:</b> " + dtHistory.Rows[0]["Approved By"].ToString();

                        DateTime nApprovedDate = new DateTime();

                        if (DateTime.TryParse(dtHistory.Rows[0]["Approved Date"].ToString(), out nApprovedDate))
                        {
                            lblApprovedDate.Text = "<b>On:</b> " + String.Format("{0:M/d/yyyy h:mm tt}", nApprovedDate);
                        }
                    }
                    
                    ddlCodingEffort.SelectedValue = dtHistory.Rows[0]["CodingEffort_ID"].ToString();
                    ddlTestingEffort.SelectedValue = dtHistory.Rows[0]["TestingEffort_ID"].ToString();
                    ddlTrainingSupportEffort.SelectedValue = dtHistory.Rows[0]["TrainingSupportEffort_ID"].ToString();
                    ddlStagePriority.SelectedValue = dtHistory.Rows[0]["Stage Priority"].ToString();
					ddlProductVersion.SelectedValue = dtHistory.Rows[0]["ProductVersion_ID"].ToString();
					spnStatus.InnerText = dtHistory.Rows[0]["SourceProductVersion_ID"].ToString() != "" ? "Carry In from " + dtHistory.Rows[0]["Carry In"] : "New";
                    ddlWorkloadAllocation.SelectedValue = dtHistory.Rows[0]["WorkloadAllocation_ID"].ToString();
                    if (ddlWorkloadAllocation.SelectedValue != dtHistory.Rows[0]["workloadAllocation_ID"].ToString())
                    {
                        ListItem waLi = new ListItem(dtHistory.Rows[0]["Workload Allocation"].ToString(), dtHistory.Rows[0]["WorkloadAllocation_ID"].ToString());
                        ddlWorkloadAllocation.Items.Insert(0, waLi);
                        ddlWorkloadAllocation.SelectedValue = dtHistory.Rows[0]["WorkloadAllocation_ID"].ToString();
                    }
                    ddlTier.SelectedValue = dtHistory.Rows[0]["Tier_ID"].ToString();

                    if (dtHistory.Rows[0]["Rank_ID"].ToString() != "")
                    {
                        int rankID = 0;
                        int.TryParse(dtHistory.Rows[0]["Rank_ID"].ToString(), out rankID);

                        txtRank.Text = rankID.ToString("D2");
                    }

                    ddlIP1.SelectedValue = dtHistory.Rows[0]["IP1Status_ID"].ToString();
                    ddlIP2.SelectedValue = dtHistory.Rows[0]["IP2Status_ID"].ToString();
                    ddlIP3.SelectedValue = dtHistory.Rows[0]["IP3Status_ID"].ToString();
                    ddlStopLight.SelectedValue = dtHistory.Rows[0]["StopLightStatus_ID"].ToString();
                    ddlAORStatus.SelectedValue = dtHistory.Rows[0]["AORStatus_ID"].ToString();
                    chkAORRequiresPD2TDR.Checked = dtHistory.Rows[0]["AORRequiresPD2TDR"].ToString() == "True";
                    txtROI.Text = dtHistory.Rows[0]["ROI_ID"].ToString();
                    ddlCMMI.SelectedValue = dtHistory.Rows[0]["CMMIStatus_ID"].ToString();
                    ddlCyber.SelectedValue = dtHistory.Rows[0]["Cyber_ID"].ToString();
                    txtCyberNarrative.Text = dtHistory.Rows[0]["CyberNarrative_ID"].ToString();
                    ddlCriticalPathTeam.SelectedValue = dtHistory.Rows[0]["CriticalPathAORTeam_ID"].ToString();
					ddlWorkType.SelectedValue = dtHistory.Rows[0]["AORWorkType_ID"].ToString();
					chkCascadeAOR.Checked = dtHistory.Rows[0]["CascadeAOR"].ToString() == "True";
					chkAORCustomerFlagship.Checked = dtHistory.Rows[0]["AORCustomerFlagship_ID"].ToString() == "True";
                    ddlInvestigation.SelectedValue = dtHistory.Rows[0]["InvestigationStatus_ID"].ToString();
                    ddlTechnical.SelectedValue = dtHistory.Rows[0]["TechnicalStatus_ID"].ToString();
                    ddlCustomerDesign.SelectedValue = dtHistory.Rows[0]["CustomerDesignStatus_ID"].ToString();
                    ddlCoding.SelectedValue = dtHistory.Rows[0]["CodingStatus_ID"].ToString();
                    ddlInternalTesting.SelectedValue = dtHistory.Rows[0]["InternalTestingStatus_ID"].ToString();
                    ddlCustomerVerificationTesting.SelectedValue = dtHistory.Rows[0]["CustomerValidationTestingStatus_ID"].ToString();
                    ddlAdoption.SelectedValue = dtHistory.Rows[0]["AdoptionStatus_ID"].ToString();
                    ddlCriticality.SelectedValue = dtHistory.Rows[0]["Criticality_ID"].ToString();
                    ddlCustomerValue.SelectedValue = dtHistory.Rows[0]["CustomerValue_ID"].ToString();
                    ddlRisk.SelectedValue = dtHistory.Rows[0]["Risk_ID"].ToString();
                    ddlLevelOfEffort.SelectedValue = dtHistory.Rows[0]["LevelOfEffort_ID"].ToString();
                    txtHoursToFix.Text = dtHistory.Rows[0]["HoursToFix"].ToString();
                    chkCyberISMT.Checked = dtHistory.Rows[0]["CyberISMT"].ToString() == "True";

                    DateTime nDate = new DateTime();

                    if (DateTime.TryParse(dtHistory.Rows[0]["PlannedStartDate"].ToString(), out nDate)) txtPlannedStart.Text = String.Format("{0:M/d/yyyy}", nDate);
                    if (DateTime.TryParse(dtHistory.Rows[0]["PlannedEndDate"].ToString(), out nDate)) txtPlannedEnd.Text = String.Format("{0:M/d/yyyy}", nDate);
                    if (DateTime.TryParse(dtHistory.Rows[0]["ActualStartDate"].ToString(), out nDate)) spnActualStart.InnerText = String.Format("{0:M/d/yyyy}", nDate);
                    if (DateTime.TryParse(dtHistory.Rows[0]["ActualEndDate"].ToString(), out nDate)) spnActualEnd.InnerText = String.Format("{0:M/d/yyyy}", nDate);
                }
			}
		}
        else
        {
            DataTable dtCurrentRelease = AOR.AORCurrentRelease_Get();

            if (dtCurrentRelease != null && dtCurrentRelease.Rows.Count > 0)
            {
                ddlProductVersion.SelectedValue = dtCurrentRelease.Rows[0]["ProductVersionID"].ToString();
            }
        }

        string strContracts = AOR.GetAORContracts(AORReleaseID);
        spnContracts.InnerHtml = strContracts;

        int productVersionID = 0;
        int.TryParse(ddlProductVersion.SelectedValue, out productVersionID);

        DataTable dtSystemsResources = MasterData.WTS_SystemList_GetWithResources(productVersionID, primarySystemContractID, false, false);
        if (dtSystemsResources != null)
        {
            dtSystemsResources.DefaultView.RowFilter = "WTS_SYSTEMID NOT IN (0, " + ddlPrimarySystem.SelectedValue + ")";
            dtSystemsResources = dtSystemsResources.DefaultView.ToTable();
            msAORSystems.Items = dtSystemsResources;
        } else
        {
            msAORSystems.Label = "No Primary System Found";
        }

        if (itisettings.Value == "")
		{
			var dt = WTSData.GetViewOptions(UserManagement.GetUserId_FromUsername(), "AOR");

			var gridViewId = (from DataRow dr in dt.Rows
							  where (bool) dr["DefaultSelection"]
							  select (int) dr["GridViewID"]).FirstOrDefault();

			var settings = (from DataRow dr in dt.Rows
						 where (int) dr["GridViewID"] == gridViewId
						 select (string) dr["Tier1Columns"]).FirstOrDefault();

			itisettings.Value = settings;
	    }

        DataSet dsPD2TDRStatus = AOR.AORPD2TDRStatus_Get(AORReleaseID: AORReleaseID);

        if (dsPD2TDRStatus != null)
        {
            DataTable dtPD2TDRPhase = dsPD2TDRStatus.Tables[0];
            StringBuilder sbPD2TDRStatus = new StringBuilder();
            string statusStyle = string.Empty;

            sbPD2TDRStatus.Append("<table style=\"border-collapse: collapse; text-align: center; margin-top: 10px;\">");
            sbPD2TDRStatus.Append("<tr class=\"gridHeader\"><th style=\"border-top: 1px solid grey; border-left: 1px solid grey;\"><img src=\"Images/Icons/add_blue.png\" title=\"Show Work Activities\" alt=\"Show Work Activities\" height=\"12\" width=\"12\" blnAll=\"1\" onclick=\"togglePhase(this);\" style=\"cursor: pointer;\" /></th><th style=\"border-top: 1px solid grey;\">PD2TDR Phase</th><th style=\"border-top: 1px solid grey;\">Workload Priority</th><th style=\"border-top: 1px solid grey;\">PD2TDR Derived Status</th></tr>");

            for (int i = 0; i < dtPD2TDRPhase.Rows.Count; i++)
            {
                switch (dtPD2TDRPhase.Rows[i]["PD2TDR Status"].ToString())
                {
                    case "Complete":
                    case "Progressing/In Work":
                    case "Progressing/In Work (Healthy Progress)":
                        statusStyle = "background-color: green; color: white;";
                        break;
                    case "Not Ready":
                        statusStyle = "background-color: red; color: white;";
                        break;
                    default:
                        statusStyle = "";
                        break;
                }

                //top border
                if (i > 0)
                {
                    sbPD2TDRStatus.Append("<tr class=\"gridBody\"><td style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center;\"><img class=\"phase\" src=\"Images/Icons/add_blue.png\" title=\"Show Work Activities\" alt=\"Show Work Activities\" blnAll=\"0\" height=\"12\" width=\"12\" onclick=\"togglePhase(this);\" style=\"cursor: pointer;\" /></td><td style=\"border-top: 1px solid grey;\">" + dtPD2TDRPhase.Rows[i]["PDDTDR_PHASE"] + "</td><td style=\"border-top: 1px solid grey;\">" + dtPD2TDRPhase.Rows[i]["Workload Priority"] + "</td><td style=\"border-top: 1px solid grey;" + statusStyle + "\">" + dtPD2TDRPhase.Rows[i]["PD2TDR Status"] + "</td></tr>");
                }
                else
                {
                    sbPD2TDRStatus.Append("<tr class=\"gridBody\"><td style=\"border-left: 1px solid grey; text-align: center;\"><img class=\"phase\" src=\"Images/Icons/add_blue.png\" title=\"Show Work Activities\" alt=\"Show Work Activities\" blnAll=\"0\" height=\"12\" width=\"12\" onclick=\"togglePhase(this);\" style=\"cursor: pointer;\" /></td><td>" + dtPD2TDRPhase.Rows[i]["PDDTDR_PHASE"] + "</td><td>" + dtPD2TDRPhase.Rows[i]["Workload Priority"] + "</td><td style=\"" + statusStyle + "\">" + dtPD2TDRPhase.Rows[i]["PD2TDR Status"] + "</td></tr>");
                }

                DataTable dtWorkActivity = dsPD2TDRStatus.Tables[1].Copy();

                dtWorkActivity.DefaultView.RowFilter = "isnull(PDDTDR_PHASEID, 0) = " + (dtPD2TDRPhase.Rows[i]["PDDTDR_PHASEID"].ToString() == "" ? "0" : dtPD2TDRPhase.Rows[i]["PDDTDR_PHASEID"]);
                dtWorkActivity = dtWorkActivity.DefaultView.ToTable();

                if (dtWorkActivity.Rows.Count > 0)
                {
                    sbPD2TDRStatus.Append("<tr style=\"display: none;\"><td colspan=\"4\" style=\"padding: 5px 0px 5px 25px;\">");
                    sbPD2TDRStatus.Append("<table style=\"border-collapse: collapse; text-align: center; width: 100%;\">");
                    sbPD2TDRStatus.Append("<tr class=\"gridHeader\"><th style=\"border-top: 1px solid grey; border-left: 1px solid grey;\">Work Activity</th><th style=\"border-top: 1px solid grey;\">Workload Priority</th><th style=\"border-top: 1px solid grey;\">PD2TDR Status</th></tr>");

                    for (int j = 0; j < dtWorkActivity.Rows.Count; j++)
                    {
                        statusStyle = "";
                        //switch (dtWorkActivity.Rows[j]["PD2TDR Status"].ToString())
                        //{
                        //    case "Complete":
                        //    case "Progressing/In Work":
                        //    case "Progressing/In Work (Healthy Progress)":
                        //        statusStyle = "background-color: green;";
                        //        break;
                        //    case "Not Ready":
                        //        statusStyle = "background-color: red; color: white;";
                        //        break;
                        //    default:
                        //        statusStyle = "";
                        //        break;
                        //}

                        sbPD2TDRStatus.Append("<tr class=\"gridBody\"><td style=\"border-left: 1px solid grey;\">" + dtWorkActivity.Rows[j]["WORKITEMTYPE"] + "</td><td>" + dtWorkActivity.Rows[j]["Workload Priority"] + "</td><td style=\"" + statusStyle + "\">" + dtWorkActivity.Rows[j]["PD2TDR Status"] + "</td></tr>");
                    }

                    sbPD2TDRStatus.Append("</table></td></tr>");
                }
            }

            sbPD2TDRStatus.Append("</table>");

            divPD2TDRStatus.InnerHtml = sbPD2TDRStatus.ToString();
        }
    }

    private void LoadAOREstimation()
    {
        //Gather Risk Values
        DataTable dtRisk = MasterData.PriorityList_Get(includeArchive: false);
        dtRisk.DefaultView.RowFilter = "PRIORITYTYPE = 'AOR Estimation'";
        dtRisk = dtRisk.DefaultView.ToTable();

        //Estimation Override
        int insertCount = 0;

        foreach (DataRow dr in dtRisk.Rows)
        {
            string strClassColor = "";
            if (dr["PRIORITY"].ToString() == "Low (Routine)") { strClassColor = "RiskLowColor"; }
            if (dr["PRIORITY"].ToString() == "Moderate (Acceptable)") { strClassColor = "RiskModerateColor"; }
            if (dr["PRIORITY"].ToString() == "High (Emergency)") { strClassColor = "RiskHighColor"; }

            ListItem li = new ListItem();
            li.Value = dr["PRIORITYID"].ToString();
            li.Text = dr["PRIORITY"].ToString();
            li.Attributes.Add("class", strClassColor);
            ddlEstimationOverride.Items.Insert(dtRisk.Rows.IndexOf(dr), li);
            insertCount++;
        }

        ListItem liSelect = new ListItem();
        liSelect.Value = "-1";
        liSelect.Text = "--Select--";
        ddlEstimationOverride.Items.Insert(0, liSelect);

        DataTable dtOverride = AOR.AORReleaseOverride_Get(AORReleaseID: AORReleaseID, includeArchive: 0);
        if (dtOverride.Rows.Count > 0)
        {
            _showOverride = 1;
            ddlEstimationOverride.SelectedValue = dtOverride.Rows[0]["PriorityID"].ToString();
            txtOverrideJustification.Text = dtOverride.Rows[0]["Justification"].ToString();
            lblOverrideJustificationCreated.Text = "Created: " + dtOverride.Rows[0]["CreatedBy"].ToString() + "-" + String.Format("{0:M/d/yyyy h:mm tt}", dtOverride.Rows[0]["CreatedDate"].ToString());
            lblOverrideJustificationUpdated.Text = "Updated: " + dtOverride.Rows[0]["UpdatedBy"].ToString() + "-" + String.Format("{0:M/d/yyyy h:mm tt}", dtOverride.Rows[0]["UpdatedDate"].ToString());
            if (dtOverride.Rows[0]["Bln_SignOff"].ToString() == "True")
            {
                chkOverrideSignOff.Checked = true;
                lblOverrideSignOffBy.Text = dtOverride.Rows[0]["SignOffBy"].ToString() + ": " + String.Format("{0:M/d/yyyy h:mm tt}", dtOverride.Rows[0]["SignOffDate"].ToString());
            }
            int countHistory = 0;
            int.TryParse(dtOverride.Rows[0]["Count_OverrideHistory"].ToString(), out countHistory);
            if (countHistory > 0)
            {
                linkOverrideHistory.InnerHtml = dtOverride.Rows[0]["Count_OverrideHistory"].ToString();
                linkOverrideHistory.Attributes.Add("onclick", "linkOverrideHistory_onClick('" + AORReleaseID + "')");
            }
        }

        //Estimated Resources
        decimal decNumResources = AOR.AOREstimatedNetResources(AORReleaseID: AORReleaseID);
        txtAOREst_NumResources.Value = decNumResources.ToString();

        //Build AOR Risk Estimation
        DataColumnCollection _dcc;
        DataTable dtAOR = new DataTable();
        dtAOR = AOR.AOREstimation_AORRelease_Get(AORReleaseID: AORReleaseID, includeArchive: 0);
        _dcc = dtAOR.Columns;

        if (dtAOR.Rows.Count > 0)
        {
            int aorAssocCount = 0;

            StringBuilder sbGrid = new StringBuilder();
            StringBuilder sbMainTable = new StringBuilder();
            sbMainTable.Append("<table cellspacing=\"0\" cellpadding=\"0\">");

            StringBuilder sbTrHeader = new StringBuilder();
            StringBuilder sbTr = new StringBuilder();
            StringBuilder sbTd = new StringBuilder();

            sbTrHeader.Append("<tr class=\"gridHeader\">");
            sbTrHeader.Append("<td style=\"display: none\">AOREstimation_AORReleaseID</td>");
            sbTrHeader.Append("<td style=\"display: none\">AOREstimationID</td>");
            sbTrHeader.Append("<td style=\"width: 200px; font-weight: bold\">Basis of Estimate</td>");
            sbTrHeader.Append("<td style=\"width: 75px; font-weight: bold\">Weight (%)</td>");
            sbTrHeader.Append("<td style=\"width: 100px; font-weight: bold\">Risk</td>");
            sbTrHeader.Append("<td style=\"font-weight: bold\">Details</td>");
            sbTrHeader.Append("<td style=\"font-weight: bold\">Mitigation Plan</td>");
            sbTrHeader.Append("</tr>");

            for (int i = 0; i < dtAOR.Rows.Count; i++)
            {
                //enable AOR Assoc button if Familiarity Estimation exists
                if (dtAOR.Select("AOREstimationName='Familiarity' AND AOREstimation_AORReleaseID IS NOT NULL").Length > 0)
                {
                    btnAddEditAORAssoc.Disabled = false;
                }

                string estimationName = dtAOR.Rows[i]["AOREstimationName"].ToString();
                sbTr.Append("<tr class=\"gridBody\">");
                sbTd.Clear();

                for (int j = 0; j < dtAOR.Columns.Count; j++)
                {
                    string columnName = dtAOR.Columns[j].ToString();
                    if (columnName == "AOREstimation_AORReleaseID")
                    {
                        sbTd.Append("<td id=\"tdAOREstimation_AORReleaseID\" style=\"text-align: center; display: none\">");
                        sbTd.Append("<input name=\"txtAOREstimation_AORReleaseID\" type=\"number\" value=\"" + dtAOR.Rows[i][j].ToString() + "\" />");
                        sbTd.Append("</td>");
                    }
                    if (columnName == "AOREstimationID")
                    {
                        sbTd.Append("<td id=\"tdBOEID_" + estimationName + "\" style=\"text-align: center; display: none\">");
                        sbTd.Append("<input name=\"txtBOEID_" + estimationName + "\" type=\"number\" value=\"" + dtAOR.Rows[i][j].ToString() + "\" />");
                        sbTd.Append("</td>");
                    }
                    if (columnName == "AOREstimationName")
                    {
                        sbTd.Append("<td id=\"tdBOE_" + dtAOR.Rows[i][j].ToString() + "\" style=\"width: 200px; font-weight: bold\">" + dtAOR.Rows[i][j].ToString() + "</td>");
                        if (dtAOR.Rows[i][j].ToString() == "Familiarity")
                        {
                            int intAOREst_AORReleaseID;
                            DataTable dtAORAssoc = new DataTable();
                            int.TryParse(dtAOR.Rows[i][dtAOR.Columns["AOREstimation_AORReleaseID"].Ordinal].ToString(), out intAOREst_AORReleaseID);
                            dtAORAssoc = AOR.AOREstimation_Assoc_Get(AOREstimation_AORReleaseID: intAOREst_AORReleaseID);

                            if (dtAORAssoc.Rows.Count > 0)
                            {
                                aorAssocCount = dtAORAssoc.Rows.Count;
                            }
                        }
                    }
                    if (columnName == "Weight")
                    {
                        sbTd.Append("<td id=\"tdWeight_" + estimationName + "\" style=\"text-align: center\">");
                        sbTd.Append("<input name=\"txtWeight_" + estimationName + "\" type=\"number\" orig_val=\"" + dtAOR.Rows[i][j].ToString() + "\" style=\"text-align: right\" maxlength=\"3\" size=\"3\" value=\"" + dtAOR.Rows[i][j].ToString() + "\" />");
                        sbTd.Append("</td>");
                        //+dtAOR.Rows[i][j].ToString() + "</td>"
                    }
                    if (columnName == "PriorityID")
                    {
                        sbTd.Append("<td id=\"tdRisk_" + estimationName + "\" style=\"text-align: center\">");
                        sbTd.Append("<select orig_val=\"" + dtAOR.Rows[i]["PRIORITYID"].ToString() + "\" name =\"ddlRisk_" + estimationName + "\">"); //select

                        sbTd.Append("<option value=\"-1\"");
                        sbTd.Append(">--Select--</option>");

                        foreach (DataRow dr in dtRisk.Rows)
                        {
                            string strClassColor = "";
                            if (dr["PRIORITY"].ToString() == "Low (Routine)") { strClassColor = "RiskLowColor"; }
                            if (dr["PRIORITY"].ToString() == "Moderate (Acceptable)") { strClassColor = "RiskModerateColor"; }
                            if (dr["PRIORITY"].ToString() == "High (Emergency)") { strClassColor = "RiskHighColor"; }

                            sbTd.Append("<option value=\"" + dr["PRIORITYID"] + "\" class=\"" + strClassColor + " \" ");
                            if (dr["PRIORITYID"].ToString() == dtAOR.Rows[i]["PRIORITYID"].ToString()) sbTd.Append(" selected");
                            
                            //If Not AOR Assoc for Familiarity, default Risk/PriorityID = Moderate (Acceptable)
                            if (dtAOR.Rows[i]["PRIORITYID"].ToString().Length == 0) {
                                if (dtAOR.Rows[i][dtAOR.Columns["AOREstimationName"].Ordinal].ToString() == "Familiarity" && dr["PRIORITY"].ToString() == "Moderate (Acceptable)")
                                {
                                    sbTd.Append(" selected");
                                }
                            }

                            sbTd.Append(">" + dr["PRIORITY"] + "</option>");
                        }
                        sbTd.Append("</select></td>");
                    }
                    if (columnName == "Details")
                    {
                        sbTd.Append("<td id=\"tdDetails_" + estimationName + "\" style=\"width: 700px\" >");
                        sbTd.Append("<input name=\"txtDetails_" + estimationName + "\" type=\"text\" value=\"" + dtAOR.Rows[i][j].ToString() + "\" style=\"width: 99%\" />");
                        sbTd.Append("</td>");
                    }
                    if (columnName == "MitigationPlan")
                    {
                        sbTd.Append("<td id=\"tdMitigation_" + estimationName + "\" style=\"width: 700px\" >");
                        sbTd.Append("<input name=\"txtMitigation_" + estimationName + "\" type=\"text\" value=\"" + dtAOR.Rows[i][j].ToString() + "\" style=\"width: 99%\" />");
                        sbTd.Append("</td>");
                    }
                }
                sbTr.Append(sbTd + "</tr>");
            }

            ////Add Total Row
            sbTr.Append("<tr id=\"trAORE_Total\" class=\"gridBody\" style=\"background-color: lightgray\">");
            sbTr.Append("<td id=\"tdBOE_Total\" style=\"font-weight: bold\"> Total </td>");
            sbTr.Append("<td id=\"tdWeight_Total\" style=\"font-weight: bold; text-align: center\"><label for=\"lblWeight_Total\" /></td>");
            sbTr.Append("<td id=\"tdRisk_Total\" style=\"font-weight: bold; text-align: center\"><label for=\"lblRisk_Total\" /></td>");
            sbTr.Append("<td colspan =\"3\"></td></tr>");

            sbGrid.Append(sbMainTable);
            sbGrid.Append(sbTrHeader);
            sbGrid.Append(sbTr + "</table>");

            divEstimationBody.InnerHtml = sbGrid.ToString();

            btnAddEditAORAssoc.Value = "View AOR Assoc (" + aorAssocCount + ")";

        }
    }
    #endregion

    #region AJAX
    [WebMethod]
	public static string Save(string blnNewAOR, string aor, string aorRelease, string aorName, string description, string notes, int approved, string codingEffort, string testingEffort, string trainingSupportEffort,
        string stagePriority, string productVersion, string workloadAllocation, string tier, string rank, string ip1, string ip2, string ip3,
        string roi, string cmmi, string cyber, string cyberNarrative, string criticalPathTeam, string workType, int cascadeAOR, int aorCustomerFlagship,
        string investigation, string technical, string customerDesign, string coding, string internalTesting, string customerValidationTesting, string adoption, string stopLight, string AORStatus, string AORRequiresPD2TDR,
        string criticality, string customerValue, string risk, string levelOfEffort, string hoursToFix, int cyberIsmt, string plannedStart, string plannedEnd, 
        string AOREstimations,
        string AORE_NetResources,
        string AORE_OverrideRisk, string AORE_OverrideJustification, string AORE_OverrideSignOff,
        string systems
        )
	{
		Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

		try
		{
			bool New_AOR = false;
			int AOR_ID = 0, AORRelease_ID = 0, codingEffortID = 0, testingEffortID = 0, trainingSupportEffortID = 0, stagePriorityID = 0, productVersionID = 0, workloadAllocation_ID = 0, tierID = 0, rankID = -999,
                IP1Status_ID = 0, IP2Status_ID = 0, IP3Status_ID = 0, CMMIStatus_ID = 0, cyberID = -1, criticalPathAORTeamID = 0, AORWorkType_ID = 0,
                investigationStatus_ID = 0, technicalStatus_ID = 0, customerDesignStatus_ID = 0, codingStatus_ID = 0, internalTestingStatus_ID = 0, customerValidationTestingStatus_ID = 0, adoptionStatus_ID = 0, stopLightStatus_ID = 0,
                aorStatus_ID = 0, aorRequiresPD2TDR = 0, criticality_ID = 0, customerValue_ID = 0, risk_ID = 0, levelOfEffort_ID = 0, nHoursToFix = -999,
                aoreOverrideRisk_ID = 0, aoreOverridSignOff = 0;
            DateTime nPlannedStart, nPlannedEnd;
            XmlDocument docSystems = (XmlDocument)JsonConvert.DeserializeXmlNode(systems, "systems");

			bool.TryParse(blnNewAOR, out New_AOR);
			int.TryParse(aor, out AOR_ID);
			int.TryParse(aorRelease, out AORRelease_ID);
            int.TryParse(codingEffort, out codingEffortID);
            int.TryParse(testingEffort, out testingEffortID);
            int.TryParse(trainingSupportEffort, out trainingSupportEffortID);
            int.TryParse(stagePriority, out stagePriorityID);
			int.TryParse(productVersion, out productVersionID);
            int.TryParse(workloadAllocation, out workloadAllocation_ID);
            int.TryParse(tier, out tierID);
            int.TryParse(ip1, out IP1Status_ID);
            int.TryParse(ip2, out IP2Status_ID);
            int.TryParse(ip3, out IP3Status_ID);
            int.TryParse(cmmi, out CMMIStatus_ID);
			int.TryParse(cyber, out cyberID);
			int.TryParse(criticalPathTeam, out criticalPathAORTeamID);
			int.TryParse(workType, out AORWorkType_ID);
            int.TryParse(investigation, out investigationStatus_ID);
            int.TryParse(technical, out technicalStatus_ID);
            int.TryParse(customerDesign, out customerDesignStatus_ID);
            int.TryParse(coding, out codingStatus_ID);
            int.TryParse(internalTesting, out internalTestingStatus_ID);
            int.TryParse(customerValidationTesting, out customerValidationTestingStatus_ID);
            int.TryParse(adoption, out adoptionStatus_ID);
            int.TryParse(stopLight, out stopLightStatus_ID);
            int.TryParse(AORStatus, out aorStatus_ID);
            int.TryParse(AORRequiresPD2TDR, out aorRequiresPD2TDR);
            int.TryParse(criticality, out criticality_ID);
            int.TryParse(customerValue, out customerValue_ID);
            int.TryParse(risk, out risk_ID);
            int.TryParse(levelOfEffort, out levelOfEffort_ID);
            int.TryParse(AORE_OverrideRisk, out aoreOverrideRisk_ID);
            int.TryParse(AORE_OverrideSignOff, out aoreOverridSignOff);

            if (rank != "") int.TryParse(rank, out rankID);
            if (hoursToFix != "") int.TryParse(hoursToFix, out nHoursToFix);

            DateTime.TryParse(plannedStart, out nPlannedStart);
            DateTime.TryParse(plannedEnd, out nPlannedEnd);

            //AOR Estimate
            XmlDocument docAOREstimations = (XmlDocument)JsonConvert.DeserializeXmlNode(AOREstimations, "AOREstimations");

            decimal decNetResources = 0;
            decimal.TryParse(AORE_NetResources, out decNetResources);

            result = AOR.AOR_Save(NewAOR: New_AOR, AORID: AOR_ID, AORName: aorName, Description: description, Notes: notes, Approved: approved, CodingEffortID: codingEffortID, TestingEffortID: testingEffortID, TrainingSupportEffortID: trainingSupportEffortID,
                StagePriorityID: stagePriorityID, ProductVersionID: productVersionID, WorkloadAllocationID: workloadAllocation_ID, TierID: tierID, RankID: rankID, IP1StatusID: IP1Status_ID, IP2StatusID: IP2Status_ID, IP3StatusID: IP3Status_ID,
                ROI: roi, CMMIStatusID: CMMIStatus_ID, CyberID: cyberID, CyberNarrative: cyberNarrative, CriticalPathAORTeamID: criticalPathAORTeamID, AORWorkTypeID: AORWorkType_ID, CascadeAOR: cascadeAOR, AORCustomerFlagship: aorCustomerFlagship,
                InvestigationStatusID: investigationStatus_ID, TechnicalStatusID: technicalStatus_ID, CustomerDesignStatusID: customerDesignStatus_ID, CodingStatusID: codingStatus_ID, InternalTestingStatusID: internalTestingStatus_ID, CustomerValidationTestingStatusID: customerValidationTestingStatus_ID, AdoptionStatusID: adoptionStatus_ID, 
                StopLightStatusID: stopLightStatus_ID, AORStatusID: aorStatus_ID, AORRequiresPD2TDR: aorRequiresPD2TDR,
                CriticalityID: criticality_ID, CustomerValueID: customerValue_ID, RiskID: risk_ID, LevelOfEffortID: levelOfEffort_ID, HoursToFix: nHoursToFix, CyberISMT: cyberIsmt, PlannedStart: nPlannedStart, PlannedEnd: nPlannedEnd,
                AORReleaseID: AORRelease_ID,
                Estimations: docAOREstimations,
                AORENetResources: decNetResources,
                AOREOverrideRiskID: aoreOverrideRisk_ID, AOREOverrideJustification: AORE_OverrideJustification, AOREOverrideSignOff: aoreOverridSignOff,
                Systems: docSystems
                );
            if (result["saved"] == "True")
            {
                DataTable dtTask = AOR.AORTaskList_Get(AORID: AOR_ID, AORReleaseID: AORRelease_ID);
                if (dtTask.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtTask.Rows)
                    {
                        bool savedAORTask = false;
                        string aors = "{save:[";
                        int task = 0;
                        int.TryParse(dr["Task_ID"].ToString(), out task);

                        DataTable dtTaskAORs = AOR.AORTaskAORList_Get(TaskID: task);
                        if (dtTaskAORs.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtTaskAORs.Rows)
                            {
                                if (row["AORWorkTypeID"].ToString() == "2")
                                {
                                    aors += "{\"aorreleaseid\":\"" + row["AORReleaseID"].ToString() + "\"},";
                                }
                            }
                        }
                        aors += "{\"aorreleaseid\":\"" + AORRelease_ID + "\"}]}";
                        XmlDocument docAORs = (XmlDocument)JsonConvert.DeserializeXmlNode(aors, "aors");

                        savedAORTask = AOR.AORTask_Save(TaskID: task, AORs: docAORs, Add: 0, CascadeAOR: cascadeAOR == 1 ? true : false);

                        //if (savedAORTask)
                        //{
                        //    DataTable dtSubTask = WorkloadItem.WorkItem_GetTaskList(workItemID: task, showArchived: 0, showBacklog: false);
                        //    if (dtSubTask != null && dtSubTask.Rows.Count > 0)
                        //    {
                        //        foreach (DataRow row in dtSubTask.Rows)
                        //        {
                        //            int subtaskID = 0;
                        //            int.TryParse(row["WORKITEM_TASKID"].ToString(), out subtaskID);
                        //            aors = "{save:[{\"aorreleaseid\":\"" + AORRelease_ID + "\"}]}";
                        //            docAORs = (XmlDocument)JsonConvert.DeserializeXmlNode(aors, "aors");
                        //            savedAORTask = AOR.AORSubTask_Save(TaskID: subtaskID, AORs: docAORs, Add: 0, CascadeAOR: cascadeAOR == 1 ? true : false);
                        //        }
                        //    }
                        //}
                    }
                }
            }
        }
		catch (Exception ex)
		{
			LogUtility.LogException(ex);

			result["error"] = ex.Message;
		}

		return JsonConvert.SerializeObject(result);
	}

    [WebMethod]
    public static string loadWorkloadAllocation(int primarySystem)
    {
        DataTable dtWorkloadAllocationContract = new DataTable();

        try
        {
            DataTable dtSystemContract = new DataTable();
             dtSystemContract = MasterData.WTS_System_ContractList_Get(primarySystem);

            int primarySystemContractID = 0;
            if (dtSystemContract != null && dtSystemContract.Rows.Count > 1)
            {
                int.TryParse(dtSystemContract.Rows[1]["CONTRACTID"].ToString(), out primarySystemContractID);
            }

            dtWorkloadAllocationContract = MasterData.WorkloadAllocation_ContractList_Get();
            dtWorkloadAllocationContract.DefaultView.RowFilter = "CONTRACTID IN (0, " + primarySystemContractID + ")";
            dtWorkloadAllocationContract.DefaultView.Sort = "ContractID";
            dtWorkloadAllocationContract = dtWorkloadAllocationContract.DefaultView.ToTable();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dtWorkloadAllocationContract, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod]
    public static string getPrimarySystemResources(int primarySystem, int productVersion)
    {
        DataTable dtSystemResource = new DataTable();

        try
        {
            dtSystemResource = MasterData.WTS_System_ResourceList_Get(primarySystem, productVersion);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dtSystemResource, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod]
    public static string SaveResources(string aor, string resources)
    {
        Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

        try
        {
            int AOR_ID = 0;
            XmlDocument docResources = (XmlDocument)JsonConvert.DeserializeXmlNode(resources, "resources");
            XmlDocument docActionTeam = (XmlDocument)JsonConvert.DeserializeXmlNode("{}", "empty");

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

    [WebMethod]
    public static void setAORSettingsSession(bool aorSystems, bool aorDetails, bool estimation, bool cmmiDetails, bool crs, bool tasks, bool resources, bool history)
    {
        var sessionMethods = new SessionMethods();
        sessionMethods.Session["AOREditSystems"] = aorSystems;
        sessionMethods.Session["AOREditDetails"] = aorDetails;
        sessionMethods.Session["AOREditEstimation"] = estimation;
        sessionMethods.Session["AOREditCMMI"] = cmmiDetails;
        sessionMethods.Session["AOREditCRs"] = crs;
        sessionMethods.Session["AOREditTasks"] = tasks;
        sessionMethods.Session["AOREditResources"] = resources;
        sessionMethods.Session["AOREditHistory"] = history;
    }

    
    [WebMethod()]
    public static string GetOverrideHistory(string aorReleaseID)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORReleaseID = 0;

            int.TryParse(aorReleaseID, out AORReleaseID);

            dt = AOR.AORReleaseOverrideHist_Get(AORReleaseID: AORReleaseID, includeArchive: 0);
            
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}