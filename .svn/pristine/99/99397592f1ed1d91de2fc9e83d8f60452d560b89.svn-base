using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

public partial class MDGrid_Narrative_Add : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected bool _refreshData = false;
    protected string _type = string.Empty;
    protected int _productVersion = 0;
    protected int _contractID = 0;
    protected bool CanEditAOR = false;
    protected int missionNarrativeID = 0;
    protected int programMGMTNarrativeID = 0;
    protected int deploymentNarrativeID = 0;
    protected int productionNarrativeID = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        readQueryString();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);

        if (DCC != null)
        {
            this.DCC.Clear();
        }

        loadData();
    }

    private void readQueryString()
    {
        if (Page.IsPostBack)
        {
            _refreshData = false;
        }
        else
        {
            if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
                || Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
            {
                _refreshData = true;
            }
        }

        if (Request.QueryString["type"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["type"].ToString()))
        {
            _type = Server.UrlDecode(Request.QueryString["type"]);
        }

        if (Request.QueryString["ProductVersionID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ProductVersionID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ProductVersionID"]), out _productVersion);
        }

        if (Request.QueryString["ContractID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ContractID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ContractID"]), out _contractID);
        }
    }

    private void loadData()
    {
        DataTable dtProductVersion = null;
        DataTable dtContract = null;
        DataTable dtNarrative = null;
        DataTable dtWorkloadAllocation = null;
        DataTable dtImage = null;
        dtProductVersion = MasterData.ProductVersionList_Get();
        dtContract = MasterData.ContractList_Get();
        dtWorkloadAllocation = MasterData.WorkloadAllocationList_Get(includeArchive: 0);
        dtImage = MasterData.ImageList_Get();

        ddlRelease.DataSource = dtProductVersion;
        ddlRelease.DataValueField = "ProductVersionID";
        ddlRelease.DataTextField = "ProductVersion";
        ddlRelease.SelectedValue = _productVersion.ToString();
        ddlRelease.DataBind();

        ddlContract.DataSource = dtContract;
        ddlContract.DataValueField = "ContractID";
        ddlContract.DataTextField = "Contract";
        ddlContract.SelectedValue = _contractID.ToString();
        ddlContract.DataBind();

        ddlMissionImage.DataSource = dtImage;
        ddlMissionImage.DataValueField = "ImageID";
        ddlMissionImage.DataTextField = "ImageName";
        ddlMissionImage.DataBind();

        ddlProgramMGMTImage.DataSource = dtImage;
        ddlProgramMGMTImage.DataValueField = "ImageID";
        ddlProgramMGMTImage.DataTextField = "ImageName";
        ddlProgramMGMTImage.DataBind();

        ddlDeploymentImage.DataSource = dtImage;
        ddlDeploymentImage.DataValueField = "ImageID";
        ddlDeploymentImage.DataTextField = "ImageName";
        ddlDeploymentImage.DataBind();

        ddlProductionImage.DataSource = dtImage;
        ddlProductionImage.DataValueField = "ImageID";
        ddlProductionImage.DataTextField = "ImageName";
        ddlProductionImage.DataBind();

        if (_productVersion > 0 && _contractID > 0)
        {
            dtNarrative = MasterData.NarrativeList_Get(productVersionID: _productVersion, contractID: _contractID, includeArchive: false);

            if (dtNarrative != null && dtNarrative.Rows.Count > 0)
            {
                foreach(DataRow dr in dtNarrative.Rows)
                {
                    if (dr["NarrativeID"].ToString() != "0")
                    {
                        switch (dr["WorkloadAllocationType"].ToString())
                        {
                            case "Mission":
                                int.TryParse(dr["NarrativeID"].ToString(), out missionNarrativeID);
                                txtMission.Text = dr["NarrativeDescription"].ToString();
                                ddlMissionImage.SelectedValue = dr["ImageID"].ToString();
                                break;
                            case "Program MGMT":
                                int.TryParse(dr["NarrativeID"].ToString(), out programMGMTNarrativeID);
                                txtProgramMGMT.Text = dr["NarrativeDescription"].ToString();
                                ddlProgramMGMTImage.SelectedValue = dr["ImageID"].ToString();
                                break;
                            case "Deployment":
                                int.TryParse(dr["NarrativeID"].ToString(), out deploymentNarrativeID);
                                txtDeployment.Text = dr["NarrativeDescription"].ToString();
                                ddlDeploymentImage.SelectedValue = dr["ImageID"].ToString();
                                break;
                            case "Production":
                                int.TryParse(dr["NarrativeID"].ToString(), out productionNarrativeID);
                                txtProduction.Text = dr["NarrativeDescription"].ToString();
                                ddlProductionImage.SelectedValue = dr["ImageID"].ToString();
                                break;
                        }
                    }
                }
            }
        }
    }

    #region AJAX
    [WebMethod(true)]
    public static string Save(int releaseID, int contractID, 
        int missionNarrativeID, string missionNarrative, int missionImageID,
        int programMGMTNarrativeID, string programMGMTNarrative, int programMGMTImageID,
        int deploymentNarrativeID, string deploymentNarrative, int deploymentImageID,
        int productionNarrativeID, string productionNarrative, int productionImageID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        int newID = 0;
        string errorMsg = string.Empty;

        try
        {
            saved = MasterData.Narrative_Add(
                releaseID, contractID,
                missionNarrativeID, missionNarrative, missionImageID,
                programMGMTNarrativeID, programMGMTNarrative, programMGMTImageID,
                deploymentNarrativeID, deploymentNarrative, deploymentImageID,
                productionNarrativeID, productionNarrative, productionImageID, 
                false, out exists, out newID, out errorMsg);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            result["error"] = ex.Message;
        }

        result["saved"] = saved.ToString();

        return JsonConvert.SerializeObject(result, Formatting.None);
    }
    #endregion
}