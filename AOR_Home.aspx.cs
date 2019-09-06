using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_Home : System.Web.UI.Page
{
    #region Variables
    protected bool CanEditAOR = false;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);

        LoadControls();
        LoadData();
    }

    private void ReadQueryString()
    {
        
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
    }

    private void LoadData()
    {
        DataTable dtCurrentRelease = AOR.AORCurrentRelease_Get();

        if (dtCurrentRelease != null && dtCurrentRelease.Rows.Count > 0)
        {
            ddlCurrentRelease.SelectedValue = dtCurrentRelease.Rows[0]["ProductVersionID"].ToString();
        }
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string Save(string productVersion)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            int productVersion_ID = 0;

            int.TryParse(productVersion, out productVersion_ID);

            saved = AOR.AORCurrentRelease_Save(ProductVersionID: productVersion_ID);
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