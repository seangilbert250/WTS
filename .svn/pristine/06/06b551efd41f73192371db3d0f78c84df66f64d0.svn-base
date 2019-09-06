using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AOR_Release_Assessment_Edit : System.Web.UI.Page
{
    #region Variables
    protected bool CanEdit = false;
    protected int AssessmentID = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEdit = true; // (UserManagement.UserCanEdit(WTSModuleOption.AOR));

        LoadData();
        ddlRelease.Style["background-color"] = "#F5F6CE";
        ddlContract.Style["background-color"] = "#F5F6CE";

        if(CanEdit)
        {
            ddlRelease.Enabled = true;
            ddlContract.Enabled = true;
            txtReviewNarrative.Enabled = true;
            txtMitigationNarrative.Enabled = true;
            chkMitigation.Enabled = true;
            chkReviewed.Enabled = true;
        }
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["ReleaseAssessmentID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseAssessmentID"]))
        {
            int.TryParse(Request.QueryString["ReleaseAssessmentID"], out AssessmentID);
        }
    }
    #endregion

    #region Data
    private void LoadData()
    {
        DataTable dtRelease = MasterData.ProductVersionList_Get();

        ddlRelease.DataSource = dtRelease;
        ddlRelease.DataValueField = "ProductVersionID";
        ddlRelease.DataTextField = "ProductVersion";
        ddlRelease.DataBind();

        DataTable dtContract = MasterData.ContractList_Get();

        ddlContract.DataSource = dtContract;
        ddlContract.DataValueField = "ContractID";
        ddlContract.DataTextField = "Contract";
        ddlContract.DataBind();

        if (AssessmentID > 0)
        {
            DataTable dt = AOR.ReleaseAssessment_Get(ReleaseAssessmentID: AssessmentID);

            if (dt != null && dt.Rows.Count > 0)
            {
                ddlRelease.SelectedValue = dt.Rows[0]["ProductVersionID"].ToString();
                ddlContract.SelectedValue = dt.Rows[0]["CONTRACTID"].ToString();
                txtReviewNarrative.Text = dt.Rows[0]["ReviewNarrative"].ToString();
                chkMitigation.Checked = dt.Rows[0]["Mitigation"].ToString() == "True" ? true : false;
                txtMitigationNarrative.Text = dt.Rows[0]["MitigationNarrative"].ToString();
                chkReviewed.Checked = dt.Rows[0]["Reviewed"].ToString() == "True" ? true : false;
            }
        }
    }
    #endregion

    #region AJAX
    [WebMethod]
    public static string Save(int AssessmentID, int ReleaseID, int ContractID, string ReviewNarrative, bool Mitigation, string MitigationNarrative, bool Reviewed)
    {
        Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool exists = false, saved = false;
        int newID = 0;
        string errorMsg = string.Empty;

        try
        {
            if (AssessmentID == 0)
            {
                saved = AOR.ReleaseAssessment_Add(ReleaseID, ContractID, ReviewNarrative, Mitigation, MitigationNarrative, Reviewed, out exists, out newID, out errorMsg);
            }
            else
            {
                saved = AOR.ReleaseAssessment_Update(AssessmentID, ReleaseID, ContractID, ReviewNarrative, Mitigation, MitigationNarrative, Reviewed);
            }

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }
        result["saved"] = saved.ToString();
        result["newID"] = newID.ToString();

        return JsonConvert.SerializeObject(result);
    }
    #endregion
    
}