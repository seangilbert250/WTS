using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Web.Services;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using WTS;
using System.Text;

public partial class CRReportBuilder : ReportPage
{
    #region Variables
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected int ReleaseID = 0;
    protected int ContractID = 0;
    protected int Visible = 1;
    protected string Type = string.Empty;

    protected string ReleaseOptions = string.Empty;
    protected string ContractOptions = string.Empty;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);

        switch (this.Type)
        {

        }

        LoadControls();
        LoadRelatedItemsMenu();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["Type"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Type"]))
        {
            this.Type = Request.QueryString["Type"];
        }
        if (Request.QueryString["ReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseID"]))
        {
            int.TryParse(Request.QueryString["ReleaseID"], out this.ReleaseID);
        }
        if (Request.QueryString["ContractID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ContractID"]))
        {
            int.TryParse(Request.QueryString["ContractID"], out this.ContractID);
        }
        if (Request.QueryString["Visible"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Visible"]))
        {
            int.TryParse(Request.QueryString["Visible"], out this.Visible);
        }
    }

    private void InitializeEvents()
    {

    }
    #endregion

    #region Data
    private void LoadControls()
    {
        // Release ddl
        DataTable dtRelease = MasterData.ProductVersionList_Get(false);
        PopulateDDLFromDataTable(ddlRelease, dtRelease, "ProductVersionID", "ProductVersion", null, null, false);
        ReleaseOptions = CreateOptionStringFromDataTable(dtRelease, "ProductVersionID", "ProductVersion", "0", "", true);

        // Contracts
        DataTable dtContract = MasterData.ContractList_Get(includeArchive: false);
        PopulateDDLFromDataTable(ddlContract, dtContract, "CONTRACTID", "CONTRACT", null, null, false);
        ContractOptions = CreateOptionStringFromDataTable(dtContract, "CONTRACTID", "CONTRACT", "0", "", true);

        DataTable dtWorkloadAllocation = MasterData.WorkloadAllocationList_Get(includeArchive: 0);

        if (dtWorkloadAllocation != null && dtWorkloadAllocation.Rows.Count > 0)
        {
            StringBuilder sbWorkloadAllocations = new StringBuilder();
            sbWorkloadAllocations.Append("<table style=\"width: 100%; \">");

            foreach(DataRow dr in dtWorkloadAllocation.Rows)
            {
                if (dr["WorkloadAllocationID"].ToString() != "0")
                {
                    sbWorkloadAllocations.Append("<tr><td id=\"wa" + dr["WorkloadAllocation"].ToString().Replace(" ", "") + "\" workloadallocation=\"" + dr["WorkloadAllocation"].ToString().Replace(" ", "") + "\" workloadallocationID=\"" + dr["WorkloadAllocationID"].ToString() + "\" workloadallocationsort=\"" + dr["SORT"].ToString() + "\" class=\"droppableCR\" colspan=\"2\" style=\"height: 75px; vertical-align: text-top; border: 1px solid #ffffff;\">");
                    sbWorkloadAllocations.Append("<strong>Workload Allocation: </strong>" + dr["WorkloadAllocation"].ToString() + " <div id=\"div" + dr["WorkloadAllocation"].ToString().Replace(" ", "") + "DropCRs\" style=\"padding-left: 5px;\"></div>");
                    sbWorkloadAllocations.Append("<div id=\"div" + dr["WorkloadAllocation"].ToString().Replace(" ", "") + "DropCRsEmpty\" style=\"padding-left: 20px; font-size: 50px; opacity: 0.25; display: none;\">Drag CRs Here</div>");
                    sbWorkloadAllocations.Append("</td></tr>");
                }
            }
            
            sbWorkloadAllocations.Append("</table>");
            divCRReportBuilder.InnerHtml = sbWorkloadAllocations.ToString();
        }
    }

    private void LoadRelatedItemsMenu()
    {
        try
        {
            DataSet dsMenu = new DataSet();
            dsMenu.ReadXml(this.Server.MapPath("XML/WTS_Menus.xml"));

            if (dsMenu.Tables.Count > 0 && dsMenu.Tables[0].Rows.Count > 0)
            {
                if (dsMenu.Tables.Contains("ReportBuilderRelatedItem"))
                {
                    menuRelatedItems.DataSource = dsMenu.Tables["ReportBuilderRelatedItem"];
                    menuRelatedItems.DataValueField = "URL";
                    menuRelatedItems.DataTextField = "Text";
                    menuRelatedItems.DataIDField = "id";
                    if (dsMenu.Tables["ReportBuilderRelatedItem"].Columns.Contains("ReportBuilderRelatedItem_id_0"))
                    {
                        menuRelatedItems.DataParentIDField = "ReportBuilderRelatedItem_id_0";
                    }
                    menuRelatedItems.DataImageField = "ImageType";
                    menuRelatedItems.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

        return dt;
    }
    #endregion

    #region AJAX

    [WebMethod()]
    public static string GetCRReportData(string ReleaseID, string ContractID, bool visible)
    {
        DataSet ds = new DataSet();

        int releaseID = 0, contractID = 0;
        int.TryParse(ReleaseID, out releaseID);
        int.TryParse(ContractID, out contractID);

        ds = AOR.CRReportBuilderList_Get(ReleaseID: releaseID, ContractID: contractID, VisibleToCustomer: visible);

        return JsonConvert.SerializeObject(ds, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod(true)]
    public static string Save(string releaseID, string contractID, string builder)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() {
        { "saved", "" }
        , { "id", "0" }
        , { "error", "" } };
        int ReleaseID = 0, ContractID = 0;
        bool saved = false;
        string errorMsg = string.Empty;

        int.TryParse(releaseID, out ReleaseID);
        int.TryParse(contractID, out ContractID);
        try
        {
            XmlDocument docBuilder = (XmlDocument)JsonConvert.DeserializeXmlNode(builder, "builder");

            if (docBuilder != null)
            {
                saved = AOR.CRReportBuilder_Save(ReleaseID: ReleaseID, ContractID: ContractID, Builder: docBuilder);
            }
        }
        catch (Exception ex)
        {
            saved = false;
            errorMsg = ex.Message;
            LogUtility.LogException(ex);
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}