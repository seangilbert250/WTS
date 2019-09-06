using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class SR_Edit : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditSR = false;
    protected bool NewSR = false;
    protected int SRID = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEditSR = UserManagement.UserCanEdit(WTSModuleOption.SustainmentRequest);

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

        if (Request.QueryString["NewSR"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewSR"]))
        {
            bool.TryParse(Request.QueryString["NewSR"], out this.NewSR);
        }

        if (Request.QueryString["SRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SRID"]))
        {
            int.TryParse(Request.QueryString["SRID"], out this.SRID);
        }
    }
    #endregion

    #region Data
    private void LoadControls()
    {
        DataTable dtStatus = MasterData.StatusList_Get(includeArchive: false);

        dtStatus.DefaultView.RowFilter = "StatusType IN ('', 'SR')";
        dtStatus = dtStatus.DefaultView.ToTable();

        if (dtStatus != null && dtStatus.Rows.Count > 0)
        {
            dtStatus.Columns["Status"].ReadOnly = false;
            dtStatus.Rows[0]["Status"] = "-Select-";
        }

        ddlStatus.DataSource = dtStatus;
        ddlStatus.DataValueField = "StatusID";
        ddlStatus.DataTextField = "Status";
        ddlStatus.DataBind();

        DataTable dtType = MasterData.SRTypeList_Get(includeArchive: false);

        if (dtType != null && dtType.Rows.Count > 0)
        {
            dtType.Columns["SRType"].ReadOnly = false;
            dtType.Rows[0]["SRType"] = "-Select-";
        }

        ddlType.DataSource = dtType;
        ddlType.DataValueField = "SRTypeID";
        ddlType.DataTextField = "SRType";
        ddlType.DataBind();

        DataTable dtPriority = MasterData.PriorityList_Get(includeArchive: false);
        DataTable dtSRRank = dtPriority;
        dtPriority.DefaultView.RowFilter = "PRIORITYTYPE IN ('', 'SR')";
        dtPriority = dtPriority.DefaultView.ToTable();

        if (dtPriority != null && dtPriority.Rows.Count > 0)
        {
            dtPriority.Columns["Priority"].ReadOnly = false;
            dtPriority.Rows[0]["Priority"] = "-Select-";
        }

        dtSRRank.DefaultView.RowFilter = "PRIORITYTYPE IN ('', 'SR Rank')";
        dtSRRank = dtSRRank.DefaultView.ToTable();

        if (dtSRRank != null && dtSRRank.Rows.Count > 0)
        {
            dtSRRank.Columns["Priority"].ReadOnly = false;
            dtSRRank.Rows[0]["Priority"] = "-Select-";
        }

        ddlSRRank.DataSource = dtSRRank;
        ddlSRRank.DataValueField = "PriorityID";
        ddlSRRank.DataTextField = "Priority";
        ddlSRRank.DataBind();

        ddlPriority.DataSource = dtPriority;
        ddlPriority.DataValueField = "PriorityID";
        ddlPriority.DataTextField = "Priority";
        ddlPriority.DataBind();

        ddlINVPriority.DataSource = dtPriority;
        ddlINVPriority.DataValueField = "PriorityID";
        ddlINVPriority.DataTextField = "Priority";
        ddlINVPriority.DataBind();
    }

    private void LoadData()
    {
        if (!this.NewSR)
        {
            DataTable dtSR = SR.SRList_Get(SRID: this.SRID);

            if (dtSR != null && dtSR.Rows.Count > 0)
            {
                spnSR.InnerText = dtSR.Rows[0]["SR #"].ToString();

                string createdDateDisplay = string.Empty, updatedDateDisplay = string.Empty;
                DateTime nCreatedDate = new DateTime(), nUpdatedDate = new DateTime();

                if (DateTime.TryParse(dtSR.Rows[0]["CreatedDate_ID"].ToString(), out nCreatedDate)) createdDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nCreatedDate);
                if (DateTime.TryParse(dtSR.Rows[0]["UpdatedDate_ID"].ToString(), out nUpdatedDate)) updatedDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nUpdatedDate);

                spnCreated.InnerText = "Submitted: " + dtSR.Rows[0]["CreatedBy_ID"].ToString() + " - " + createdDateDisplay;
                spnUpdated.InnerText = "Updated: " + dtSR.Rows[0]["UpdatedBy_ID"].ToString() + " - " + updatedDateDisplay;
                ddlStatus.SelectedValue = dtSR.Rows[0]["StatusID"].ToString();
                ddlType.SelectedValue = dtSR.Rows[0]["Type_ID"].ToString();
                ddlPriority.SelectedValue = dtSR.Rows[0]["Priority_ID"].ToString();
                ddlINVPriority.SelectedValue = dtSR.Rows[0]["INVPriorityID"].ToString();
                ddlSRRank.SelectedValue = dtSR.Rows[0]["SRRankID"].ToString();
                txtDescription.Text = dtSR.Rows[0]["Description"].ToString();
                txtDescription.Attributes.Add("original_value", dtSR.Rows[0]["Description"].ToString());
            }
        }
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string Save(string blnNewSR, string srNum, string status, string type, string priority, string invPriority, string srRank, string description)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

        try
        {
            bool New_SR = false;
            int SR_ID = 0, status_ID = 0, SRType_ID = 0, priority_ID = 0, inv_priority_ID = 0, sr_rank_ID = 0;

            bool.TryParse(blnNewSR, out New_SR);
            int.TryParse(srNum, out SR_ID);
            int.TryParse(status, out status_ID);
            int.TryParse(type, out SRType_ID);
            int.TryParse(priority, out priority_ID);
            int.TryParse(invPriority, out inv_priority_ID);
            int.TryParse(srRank, out sr_rank_ID);

            result = SR.SR_Save(NewSR: New_SR, SRID: SR_ID, StatusID: status_ID, SRTypeID: SRType_ID, PriorityID: priority_ID, INVPriorityID: inv_priority_ID, SRRankID: sr_rank_ID, Description: description);
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