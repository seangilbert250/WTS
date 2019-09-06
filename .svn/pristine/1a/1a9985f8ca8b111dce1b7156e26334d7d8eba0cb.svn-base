using System;
using System.Data;

public partial class AOR_Scheduled_Deliverables_Tabs : System.Web.UI.Page
{
    #region Variables
    protected bool CanEditAOR = false;
    protected bool NewDeliverable = false;
    protected int DeliverableID = 0;
    protected int ReleaseID = 0;
    protected string Source = string.Empty;
    protected int AORCount = 0;
    protected int ContractCount = 0;
    protected string Tab = string.Empty;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEditAOR = (UserManagement.UserCanEdit(WTSModuleOption.AOR));

        LoadData();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["NewDeliverable"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewDeliverable"]))
        {
            bool.TryParse(Request.QueryString["NewDeliverable"], out this.NewDeliverable);
        }

        if (Request.QueryString["DeliverableID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["DeliverableID"]))
        {
            int.TryParse(Request.QueryString["DeliverableID"], out this.DeliverableID);
        }

        if (Request.QueryString["ReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseID"]))
        {
            int.TryParse(Request.QueryString["ReleaseID"], out this.ReleaseID);
        }

        if (Request.QueryString["Source"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Source"]))
        {
            this.Source = Request.QueryString["Source"];
        }

        if (Request.QueryString["Tab"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Tab"]))
        {
            this.Tab = Request.QueryString["Tab"];
        }
    }
    #endregion

    #region Data
    private void LoadData()
    {
        if (!this.NewDeliverable)
        {

            DataTable dtAOR = AOR.AORDeliverableList_Get(DeliverableID: this.DeliverableID);

            if (dtAOR != null) this.AORCount = dtAOR.Rows.Count-1;

            DataTable dtContract = MasterData.DeploymentContractList_Get(DeliverableID: this.DeliverableID);

            if (dtContract != null) this.ContractCount = dtContract.Rows.Count;
        }
    }
    #endregion
}