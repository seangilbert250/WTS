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

public partial class AOR_Release_Builder : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool CanEditCR = false;
    protected bool CanViewCR = false;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);
        this.CanEditCR = UserManagement.UserCanEdit(WTSModuleOption.CR);
        this.CanViewCR = this.CanEditCR || UserManagement.UserCanView(WTSModuleOption.CR);
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewWorkItem = this.CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

        LoadControls();
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
    }
    #endregion

    #region Data
    private void LoadControls()
    {
        DataTable dtProductVersion = MasterData.ProductVersionList_Get(includeArchive: false);

        ddlCurrentRelease.DataSource = dtProductVersion;
        ddlCurrentRelease.DataValueField = "ProductVersionID";
        ddlCurrentRelease.DataTextField = "ProductVersion";
        ddlCurrentRelease.DataBind();

        DataTable dtCurrentRelease = AOR.AORCurrentRelease_Get();

        if (dtCurrentRelease != null && dtCurrentRelease.Rows.Count > 0)
        {
            ddlCurrentRelease.SelectedValue = dtCurrentRelease.Rows[0]["ProductVersionID"].ToString();
        }

        ddlNewRelease.DataSource = dtProductVersion;
        ddlNewRelease.DataValueField = "ProductVersionID";
        ddlNewRelease.DataTextField = "ProductVersion";
        ddlNewRelease.DataBind();

        DataTable dtAssignedToRank = MasterData.PriorityList_Get(includeArchive: false, includeDefaultRow: false, PRIORITYTYPEID: 0);

        dtAssignedToRank.DefaultView.RowFilter = "PRIORITYTYPE = 'Rank' AND Priority <> '6 - Closed Workload'";
        dtAssignedToRank = dtAssignedToRank.DefaultView.ToTable();

        for (int i = 0; i < dtAssignedToRank.Rows.Count; i++)
        {
            ListItem li = new ListItem();

            li.Value = dtAssignedToRank.Rows[i]["PriorityID"].ToString();
            li.Text = dtAssignedToRank.Rows[i]["Priority"].ToString();
            li.Selected = true;

            cblAssignedToRank.Items.Add(li);
        }
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string GetAORs(string currentRelease)
    {
        DataSet ds = new DataSet();

        try
        {
            int CurrentRelease_ID = 0;

            int.TryParse(currentRelease, out CurrentRelease_ID);

            ds = AOR.AORReleaseBuilderList_Get(CurrentReleaseID: CurrentRelease_ID);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(ds, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string Save(string currentRelease, string newRelease, string assignedToRankIDs, string additions)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            int CurrentRelease_ID = 0, NewRelease_ID = 0;

            int.TryParse(currentRelease, out CurrentRelease_ID);
            int.TryParse(newRelease, out NewRelease_ID);
            XmlDocument docAdditions = (XmlDocument)JsonConvert.DeserializeXmlNode(additions, "additions");
            
            saved = AOR.AORReleaseBuilder_Save(CurrentReleaseID: CurrentRelease_ID, NewReleaseID: NewRelease_ID, AssignedToRankIDs: assignedToRankIDs, Additions: docAdditions);
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