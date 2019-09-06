﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_CR_Narratives : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditCR = false;
    protected int CRID = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEditCR = UserManagement.UserCanEdit(WTSModuleOption.CR);

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

        if (Request.QueryString["CRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CRID"]))
        {
            int.TryParse(Request.QueryString["CRID"], out this.CRID);
        }
    }
    #endregion

    #region Data
    private void LoadData()
    {
        DataTable dtCR = AOR.AORCRLookupList_Get(CRID: this.CRID);

        if (dtCR != null && dtCR.Rows.Count > 0)
        {
            txtBasisOfRisk.Text = dtCR.Rows[0]["BasisOfRisk"].ToString();
            txtBasisOfUrgency.Text = dtCR.Rows[0]["BasisOfUrgency"].ToString();
            txtCustomerImpact.Text = dtCR.Rows[0]["CustomerImpact"].ToString();
            txtIssue.Text = dtCR.Rows[0]["Issue"].ToString();
            txtProposedSolution.Text = dtCR.Rows[0]["ProposedSolution"].ToString();
            txtRationale.Text = dtCR.Rows[0]["Rationale"].ToString();
            txtWorkloadPriority.Text = dtCR.Rows[0]["WorkloadPriority"].ToString();
            txtNotes.Text = Uri.UnescapeDataString(dtCR.Rows[0]["Notes"].ToString());
        }
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string Save(string cr, string notes, string basisOfRisk, string basisOfUrgency, string customerImpact, string issue, string proposedSolution, string rationale, string workloadPriority)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            int CR_ID = 0;

            int.TryParse(cr, out CR_ID);

            saved = AOR.AORCRLookupNarrative_Save(CRID: CR_ID, Notes: notes, BasisOfRisk: basisOfRisk, BasisOfUrgency: basisOfUrgency, CustomerImpact: customerImpact, Issue: issue, ProposedSolution: proposedSolution, Rationale: rationale, WorkloadPriority: workloadPriority);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            saved = false;
            errorMsg = ex.Message;
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}