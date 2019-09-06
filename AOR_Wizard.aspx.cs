using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_Wizard : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected string Filter = string.Empty;
    protected string AORName = string.Empty;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected string currentReleaseID = "0";
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);

        LoadControls();
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

        if (Request.QueryString["Filter"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Filter"]))
        {
            this.Filter = Request.QueryString["Filter"];
        }

        if (Request.QueryString["AORName"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORName"]))
        {
            this.AORName = Request.QueryString["AORName"];
        }
    }
    #endregion

    #region Data
    private void LoadControls()
    {
        this.txtAORName.Text = this.AORName;

        DataTable dtProductVersion = MasterData.ProductVersionList_Get(includeArchive: false);
        DataTable dtCurrentRelease = AOR.AORCurrentRelease_Get();

        if (dtCurrentRelease != null && dtCurrentRelease.Rows.Count > 0)
            currentReleaseID = dtCurrentRelease.Rows[0]["ProductVersionID"].ToString();

        ddlProductVersion.DataSource = dtProductVersion;
        ddlProductVersion.DataValueField = "ProductVersionID";
        ddlProductVersion.DataTextField = "ProductVersion";
        ddlProductVersion.DataBind();

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

        DataTable dtWorkloadAllocation = MasterData.WorkloadAllocationList_Get(includeArchive: 0);

        ddlWorkloadAllocation.DataSource = dtWorkloadAllocation;
        ddlWorkloadAllocation.DataValueField = "WorkloadAllocationID";
        ddlWorkloadAllocation.DataTextField = "WorkloadAllocation";
        ddlWorkloadAllocation.DataBind();

        DataTable dtWorkType = AOR.AORWorkTypeList_Get();

        ddlWorkType.DataSource = dtWorkType;
        ddlWorkType.DataValueField = "AORWorkType_ID";
        ddlWorkType.DataTextField = "Work Type";
        ddlWorkType.DataBind();

        ListItem li = new ListItem();

        li.Value = "0";
        li.Text = "";

        ddlWorkType.Items.Insert(0, li);

        DataTable dtSystems = MasterData.SystemList_Get(includeArchive: false, cv: "0");
        StringBuilder sbSystems = new StringBuilder();

        dtSystems.DefaultView.RowFilter = "WTS_SYSTEM <> ''";
        dtSystems.DefaultView.Sort = "WTS_SYSTEM";
        dtSystems = dtSystems.DefaultView.ToTable();

        sbSystems.Append("<table style=\"border-collapse: collapse; width: 100%;\">");
        sbSystems.Append("<tr class=\"gridHeader\">");
        sbSystems.Append("<th style=\"text-align: center; width: 25px;\"></th>");
        sbSystems.Append("<th style=\"text-align: center;\">System</th>");
        sbSystems.Append("</tr>");

        var rowIdx = 0;
        foreach (DataRow dr in dtSystems.Rows)
        {
            sbSystems.Append("<tr class=\"gridBody\" sysrow=\"1\" origsort=\"" + rowIdx++ + "\">");
            sbSystems.Append("<td style=\"text-align: center;\">");
            sbSystems.Append("<input type=\"checkbox\" systemid=\"" + dr["WTS_SYSTEMID"] + "\" field=\"System\" />");
            sbSystems.Append("</td>");
            sbSystems.Append("<td>" + dr["WTS_SYSTEM"] + "</td>");
            sbSystems.Append("</tr>");
        }

        sbSystems.Append("</table>");
        divAORSystems.InnerHtml = sbSystems.ToString();

        DataTable dtResource = MasterData.WTS_Resource_Get(includeArchive: false);
        StringBuilder sbResource = new StringBuilder();

        sbResource.Append("<table style=\"border-collapse: collapse; width: 100%;\">");
        sbResource.Append("<tr class=\"gridHeader\">");
        sbResource.Append("<th style=\"text-align: center; width: 25px;\"></th>");
        sbResource.Append("<th style=\"text-align: center;\">Resource</th>");
        sbResource.Append("</tr>");

        rowIdx = 0;
        foreach (DataRow dr in dtResource.Rows)
        {
            sbResource.Append("<tr class=\"gridBody\" rscrow=\"1\" origsort=\"" + rowIdx++ + "\">");
            sbResource.Append("<td style=\"text-align: center;\">");
            sbResource.Append("<input type=\"checkbox\" resourceid=\"" + dr["WTS_ResourceID"] + "\" field=\"Resource\" />");
            sbResource.Append("</td>");
            sbResource.Append("<td>" + dr["USERNAME"] + "</td>");
            sbResource.Append("</tr>");
        }

        sbResource.Append("</table>");
        divAORResources.InnerHtml = sbResource.ToString();

        DataTable dtCR = AOR.AORCRLookupList_Get(CRID: 0);
        DataTable dtCRStatus = dtCR.Copy();
        DataTable dtCRContract = dtCR.Copy();

        dtCRStatus.DefaultView.Sort = "STATUS";
        dtCRStatus = dtCRStatus.DefaultView.ToTable(true, new string[] { "StatusID", "STATUS" });
        dtCRContract.DefaultView.Sort = "CONTRACT";
        dtCRContract = dtCRContract.DefaultView.ToTable(true, new string[] { "CONTRACTID", "CONTRACT" });

        if (dtCRStatus != null)
        {
            foreach (DataRow dr in dtCRStatus.Rows)
            {
                ListItem liCRStatus = new ListItem(dr["STATUS"].ToString(), dr["StatusID"].ToString() == "" ? "0" : dr["StatusID"].ToString());

                if (liCRStatus.Text.ToUpper() != "RESOLVED") liCRStatus.Selected = true;

                ddlCRStatusQF.Items.Add(liCRStatus);
            }
        }

        if (dtCRContract != null)
        {
            foreach (DataRow dr in dtCRContract.Rows)
            {
                ListItem liCRContract = new ListItem(dr["CONTRACT"].ToString(), dr["CONTRACTID"].ToString() == "" ? "0" : dr["CONTRACTID"].ToString());

                liCRContract.Selected = true;

                ddlCRContractQF.Items.Add(liCRContract);
            }
        }
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string Search(string aorName)
    {
        DataTable dt = new DataTable();

        try
        {
            dt = AOR.AORList_Get(AORID: 0);
            dt.DefaultView.RowFilter = "[AOR Name] LIKE '%" + aorName.Replace("'", "''") + "%'";
            dt = dt.DefaultView.ToTable();
            dt.Columns["AOR #"].ColumnName = "AORID";
            dt.Columns["AOR Name"].ColumnName = "AORName";

            dt.AcceptChanges();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static bool AORExists(string aorName)
    {
        bool exists = false;

        try
        {
            DataTable dt = AOR.AORList_Get(AORID: 0); //todo: check all, even archived

            exists = (from row in dt.AsEnumerable()
                      where row.Field<string>("AOR Name").Trim().ToUpper() == aorName.Trim().ToUpper()
                      select row).Count() > 0;
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return exists;
    }

    [WebMethod()]
    public static string GetAOR(string aor)
    {
        DataTable dt = new DataTable();

        try
        {
            int AOR_ID = 0;

            int.TryParse(aor, out AOR_ID);

            dt = AOR.AORList_Get(AORID: AOR_ID);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetWorkloadAllocation (string aor, string aorRelease)
    {
        DataTable dtWorkloadAllocationContract = new DataTable();

        try
        {
            int AOR_ID = 0;
            int AORRelease_ID = 0;

            int.TryParse(aor, out AOR_ID);
            int.TryParse(aorRelease, out AORRelease_ID);

            DataTable dtAORSystems = AOR.AORSystemList_Get(AORID: AOR_ID, AORReleaseID: AORRelease_ID);
            DataTable dtSystemContract = new DataTable();
            dtAORSystems.DefaultView.RowFilter = "Primary IN (True)";
            dtAORSystems = dtAORSystems.DefaultView.ToTable();

            if (dtAORSystems != null && dtAORSystems.Rows.Count > 0)
            {
                int systemID = 0;
                int.TryParse(dtAORSystems.Rows[0]["WTS_SYSTEM_ID"].ToString(), out systemID);
                dtSystemContract = MasterData.WTS_System_ContractList_Get(systemID);
            }

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

    [WebMethod()]
    public static string GetPrimarySystem()
    {
        DataTable dtSystems = new DataTable();

        try
        {
            dtSystems = MasterData.SystemList_Get(includeArchive: false, cv: "0");
            dtSystems.DefaultView.Sort = "WTS_SystemSuite, WTS_SYSTEM";
            dtSystems = dtSystems.DefaultView.ToTable();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dtSystems, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetSystems(string aor)
    {
        DataTable dt = new DataTable();

        try
        {
            int AOR_ID = 0;

            int.TryParse(aor, out AOR_ID);

            dt = AOR.AORSystemList_Get(AORID: AOR_ID, AORReleaseID: 0);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetResources(string aor)
    {
        DataTable dt = new DataTable();

        try
        {
            int AOR_ID = 0;

            int.TryParse(aor, out AOR_ID);

            dt = AOR.AORResourceList_Get(AORID: AOR_ID, AORReleaseID: 0);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetCRs(string aor)
    {
        DataTable dt = new DataTable();

        try
        {
            int AOR_ID = 0;

            int.TryParse(aor, out AOR_ID);

            dt = AOR.AORCRList_Get(AORID: AOR_ID, AORReleaseID: 0, CRID: 0);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetTasks(string aor)
    {
        DataTable dt = new DataTable();

        try
        {
            int AOR_ID = 0;

            int.TryParse(aor, out AOR_ID);

            dt = AOR.AORTaskList_Get(AORID: AOR_ID, AORReleaseID: 0);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod]
    public static string Save(string aor, string aorName, string description, string productVersion, string workloadAllocation, string workType, string systems, string resources, string crs, string tasks)
    {
        Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

        try
        {
            int AOR_ID = 0, productVersionID = 0, workloadAllocation_ID = 0, AORWorkType_ID = 0;
            XmlDocument docSystems = (XmlDocument)JsonConvert.DeserializeXmlNode(systems, "systems");
            XmlDocument docResources = (XmlDocument)JsonConvert.DeserializeXmlNode(resources, "resources");
            XmlDocument docCRs = (XmlDocument)JsonConvert.DeserializeXmlNode(crs, "crs");
            XmlDocument docTasks = (XmlDocument)JsonConvert.DeserializeXmlNode(tasks, "tasks");

            int.TryParse(aor, out AOR_ID);
            int.TryParse(productVersion, out productVersionID);
            int.TryParse(workloadAllocation, out workloadAllocation_ID);
            int.TryParse(workType, out AORWorkType_ID);

            result = AOR.AORWizard_Save(AORID: AOR_ID, AORName: aorName, Description: description, ProductVersionID: productVersionID, WorkloadAllocationID: workloadAllocation_ID, AORWorkTypeID: AORWorkType_ID,
                Systems: docSystems, Resources: docResources, CRs: docCRs, Tasks: docTasks);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result);
    }
    #endregion
}