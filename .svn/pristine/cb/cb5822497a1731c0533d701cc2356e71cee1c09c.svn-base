using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_CR_Edit : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditCR = false;
    protected bool NewCR = false;
    protected int CRID = 0;
    protected bool Imported = false;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEditCR = UserManagement.UserCanEdit(WTSModuleOption.CR);

        LoadControls();
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

        if (Request.QueryString["NewCR"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewCR"]))
        {
            bool.TryParse(Request.QueryString["NewCR"], out this.NewCR);
        }

        if (Request.QueryString["CRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CRID"]))
        {
            int.TryParse(Request.QueryString["CRID"], out this.CRID);
        }
    }
    #endregion

    #region Data
    private void LoadControls()
    {
        DataTable dtWebsystem = MasterData.SystemList_Get(includeArchive: false, cv: "0", ProductVersionID: 0);
        string currentSuiteID = null;
        ListItem li;

        dtWebsystem.DefaultView.Sort = "WTS_SystemSuite, WTS_SYSTEM";
        dtWebsystem = dtWebsystem.DefaultView.ToTable();

        foreach (DataRow row in dtWebsystem.Rows)
        {
            if (row["WTS_SystemSuiteID"].ToString() != currentSuiteID && row["WTS_SystemSuiteID"].ToString() != "0")
            {
                li = new ListItem();
                li.Text = row["WTS_SystemSuite"].ToString();
                li.Attributes["disabled"] = "disabled";
                li.Attributes["style"] = "background-color: white";
                ddlWebsystem.Items.Add(li);
            }

            li = new ListItem(row["WTS_SYSTEM"].ToString());
            ddlWebsystem.Items.Add(li);

            currentSuiteID = row["WTS_SystemSuiteID"].ToString();
        }

        ddlSubgroup.DataSource = dtWebsystem;
        ddlSubgroup.DataValueField = "WTS_SYSTEM";
        ddlSubgroup.DataBind();

        ddlCPList.DataSource = dtWebsystem;
        ddlCPList.DataValueField = "WTS_SYSTEM";
        ddlCPList.DataBind();

        DataTable dtProductVersion = MasterData.ProductVersionList_Get(includeArchive: false);

        ddlRelatedRelease.DataSource = dtProductVersion;
        ddlRelatedRelease.DataTextField = "ProductVersion";
        ddlRelatedRelease.DataBind();

        ddlDesignReview.Items.Add("");
        ddlDesignReview.Items.Add("Y");
        ddlDesignReview.Items.Add("NDI");
        ddlDesignReview.Items.Add("P");

        DataTable dtResource = MasterData.WTS_Resource_Get(includeArchive: false);

        ddlITIPOC.DataSource = dtResource;
        ddlITIPOC.DataTextField = "USERNAME";
        ddlITIPOC.DataBind();
        ddlITIPOC.Items.Insert(0, "");

        if (!this.NewCR)
        {
            DataTable dtSR = AOR.AORSRList_Get(CRID: this.CRID);

            ddlPrimarySR.DataSource = dtSR;
            ddlPrimarySR.DataValueField = "SRID";
            ddlPrimarySR.DataTextField = "SRID";
            ddlPrimarySR.DataBind();
        }

        li = new ListItem();
        li.Value = "0";
        li.Text = "";

        ddlPrimarySR.Items.Insert(0, li);

        DataTable dtStatus = MasterData.StatusList_Get(includeArchive: false);

        dtStatus.DefaultView.RowFilter = "StatusType IN ('', 'AORCR')";
        dtStatus = dtStatus.DefaultView.ToTable();
        ddlStatus.DataSource = dtStatus;
        ddlStatus.DataValueField = "StatusID";
        ddlStatus.DataTextField = "Status";
        ddlStatus.DataBind();
    }

    private void LoadData()
    {
        if (!this.NewCR)
        {
            DataTable dtCR = AOR.AORCRLookupList_Get(CRID: this.CRID);

            if (dtCR != null && dtCR.Rows.Count > 0)
            {
                ListItem li;

                this.Imported = dtCR.Rows[0]["Imported"].ToString() == "True";

                string createdDateDisplay = string.Empty, updatedDateDisplay = string.Empty;
                DateTime nCreatedDate = new DateTime(), nUpdatedDate = new DateTime();

                if (DateTime.TryParse(dtCR.Rows[0]["CreatedDate_ID"].ToString(), out nCreatedDate)) createdDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nCreatedDate);
                if (DateTime.TryParse(dtCR.Rows[0]["UpdatedDate_ID"].ToString(), out nUpdatedDate)) updatedDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nUpdatedDate);

                spnCreated.InnerText = "Created: " + dtCR.Rows[0]["CreatedBy_ID"].ToString() + " - " + createdDateDisplay;
                spnUpdated.InnerText = "Updated: " + dtCR.Rows[0]["UpdatedBy_ID"].ToString() + " - " + updatedDateDisplay;
                txtCRName.Text = Uri.UnescapeDataString(dtCR.Rows[0]["CRName"].ToString());
                txtCRName.Attributes.Add("original_value", Uri.UnescapeDataString(dtCR.Rows[0]["CRName"].ToString()));
                txtTitle.Text = Uri.UnescapeDataString(dtCR.Rows[0]["Title"].ToString());
                txtNotes.Text = Uri.UnescapeDataString(dtCR.Rows[0]["Notes"].ToString());

                li = new ListItem(dtCR.Rows[0]["Websystem"].ToString());
                if (ddlWebsystem.Items.Contains(li)) ddlWebsystem.SelectedValue = li.Value;
                else ddlWebsystem.Items.Insert(0, li);

                chkCSDRequiredNow.Checked = dtCR.Rows[0]["CSDRequiredNow"].ToString() == "1";

                li = new ListItem(dtCR.Rows[0]["RelatedRelease"].ToString());
                if (ddlRelatedRelease.Items.Contains(li)) ddlRelatedRelease.SelectedValue = li.Value;
                else ddlRelatedRelease.Items.Insert(0, li);

                li = new ListItem(dtCR.Rows[0]["Subgroup"].ToString());
                if (ddlSubgroup.Items.Contains(li)) ddlSubgroup.SelectedValue = li.Value;
                else ddlSubgroup.Items.Insert(0, li);

                li = new ListItem(dtCR.Rows[0]["DesignReview"].ToString());
                if (ddlDesignReview.Items.Contains(li)) ddlDesignReview.SelectedValue = li.Value;
                else ddlDesignReview.Items.Insert(0, li);

                li = new ListItem(dtCR.Rows[0]["ITIPOC"].ToString());
                if (ddlITIPOC.Items.Contains(li)) ddlITIPOC.SelectedValue = li.Value;
                else ddlITIPOC.Items.Insert(0, li);

                li = new ListItem(dtCR.Rows[0]["CustomerPriorityList"].ToString());
                if (ddlCPList.Items.Contains(li)) ddlCPList.SelectedValue = li.Value;
                else ddlCPList.Items.Insert(0, li);

                txtGovernmentCSRD.Text = dtCR.Rows[0]["GovernmentCSRD"].ToString();
                ddlPrimarySR.SelectedValue = dtCR.Rows[0]["PrimarySR"].ToString();
                txtCAMPriority.Text = dtCR.Rows[0]["CAMPriority"].ToString();
                txtLCMBPriority.Text = dtCR.Rows[0]["LCMBPriority"].ToString();
                txtAirstaffPriority.Text = dtCR.Rows[0]["AirstaffPriority"].ToString();
                txtCustomerPriority.Text = dtCR.Rows[0]["CustomerPriority"].ToString();
                txtITIPriority.Text = dtCR.Rows[0]["ITIPriority"].ToString();
                txtRiskOfPTS.Text = dtCR.Rows[0]["RiskOfPTS"].ToString();
                ddlStatus.SelectedValue = dtCR.Rows[0]["StatusID"].ToString();

                DateTime nDate = new DateTime();

                if (DateTime.TryParse(dtCR.Rows[0]["LCMBSubmittedDate"].ToString(), out nDate)) txtLCMBSubmitted.Text = String.Format("{0:M/d/yyyy}", nDate);
                if (DateTime.TryParse(dtCR.Rows[0]["LCMBApprovedDate"].ToString(), out nDate)) txtLCMBApproved.Text = String.Format("{0:M/d/yyyy}", nDate);
                if (DateTime.TryParse(dtCR.Rows[0]["ERBISMTSubmittedDate"].ToString(), out nDate)) txtERBISMTSubmitted.Text = String.Format("{0:M/d/yyyy}", nDate);
                if (DateTime.TryParse(dtCR.Rows[0]["ERBISMTApprovedDate"].ToString(), out nDate)) txtERBISMTApproved.Text = String.Format("{0:M/d/yyyy}", nDate);
            }
        }
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string Save(int Altered, string blnNewCR, string cr, string crName, string title, string notes, string websystem, int csdRequiredNow, string relatedRelease, string subgroup, string designReview, string itiPOC, string customerPriorityList, string governmentCSRD, string primarySR,
        string camPriority, string lcmbPriority, string airstaffPriority, string customerPriority, string itiPriority, string riskOfPTS,
        string status, string lcmbSubmitted, string lcmbApproved, string erbismtSubmitted, string erbismtApproved)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

        try
        {
            bool New_CR = false;
            int CR_ID = 0, primarySR_ID = 0, status_ID = 0;
            int nGovernmentCSRD = -999, nCAMPriority = -999, nLCMBPriority = -999, nAirstaffPriority = -999, nCustomerPriority = -999, nITIPriority = -999, nRiskOfPTS = -999;
            DateTime nLCMBSubmitted, nLCMBApproved, nERBISMTSubmitted, nERBISMTApproved;

            bool.TryParse(blnNewCR, out New_CR);
            int.TryParse(cr, out CR_ID);
            int.TryParse(primarySR, out primarySR_ID);
            int.TryParse(status, out status_ID);

            if (governmentCSRD != "") int.TryParse(governmentCSRD, out nGovernmentCSRD);
            if (camPriority != "") int.TryParse(camPriority, out nCAMPriority);
            if (lcmbPriority != "") int.TryParse(lcmbPriority, out nLCMBPriority);
            if (airstaffPriority != "") int.TryParse(airstaffPriority, out nAirstaffPriority);
            if (customerPriority != "") int.TryParse(customerPriority, out nCustomerPriority);
            if (itiPriority != "") int.TryParse(itiPriority, out nITIPriority);
            if (riskOfPTS != "") int.TryParse(riskOfPTS, out nRiskOfPTS);

            DateTime.TryParse(lcmbSubmitted, out nLCMBSubmitted);
            DateTime.TryParse(lcmbApproved, out nLCMBApproved);
            DateTime.TryParse(erbismtSubmitted, out nERBISMTSubmitted);
            DateTime.TryParse(erbismtApproved, out nERBISMTApproved);

            result = AOR.AORCRLookup_Save(Altered: Altered, NewCR: New_CR, CRID: CR_ID, CRName: crName, Title: title, Notes: notes, Websystem: websystem, CSDRequiredNow: csdRequiredNow, RelatedRelease: relatedRelease, Subgroup: subgroup, DesignReview: designReview, ITIPOC: itiPOC, CustomerPriorityList: customerPriorityList, GovernmentCSRD: nGovernmentCSRD, PrimarySRID: primarySR_ID,
                CAMPriority: nCAMPriority, LCMBPriority: nLCMBPriority, AirstaffPriority: nAirstaffPriority, CustomerPriority: nCustomerPriority, ITIPriority: nITIPriority, RiskOfPTS: nRiskOfPTS,
                StatusID: status_ID, LCMBSubmitted: nLCMBSubmitted, LCMBApproved: nLCMBApproved, ERBISMTSubmitted: nERBISMTSubmitted, ERBISMTApproved: nERBISMTApproved);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}