using Newtonsoft.Json;
using System;
using System.Data;
using System.Web.Services;

public partial class AOR_CR_Tabs : System.Web.UI.Page
{
    #region Variables
    protected bool NewCR = false;
    protected int CRID = 0;
    protected int AORCount = 0;
    protected int SRCount = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        LoadRelatedItemsMenu();
        LoadData();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["NewCR"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewCR"]))
        {
            bool.TryParse(Request.QueryString["NewCR"], out this.NewCR);
        }

        if (Request.QueryString["CRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CRID"]))
        {
            int.TryParse(Request.QueryString["CRID"], out this.CRID);
        }
    }

    /// <summary>
    /// This will load our Related Items Menu with the data from our WTS_Menus.xml file on our server
    /// </summary>
    private void LoadRelatedItemsMenu()
    {
        try
        {
            DataSet dsMenu = new DataSet();
            dsMenu.ReadXml(this.Server.MapPath("XML/WTS_Menus.xml"));

            if (dsMenu.Tables.Count > 0 && dsMenu.Tables[0].Rows.Count > 0)
            {
                if (dsMenu.Tables.Contains("CRGridRelatedItem"))
                {
                    menuRelatedItems.DataSource = dsMenu.Tables["CRGridRelatedItem"];
                    menuRelatedItems.DataValueField = "URL";
                    menuRelatedItems.DataTextField = "Text";
                    menuRelatedItems.DataIDField = "id";
                    if (dsMenu.Tables["CRGridRelatedItem"].Columns.Contains("CRGridRelatedItem_id_0"))
                    {
                        menuRelatedItems.DataParentIDField = "CRGridRelatedItem_id_0";
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
        this.Server.MapPath(null);
    }
    #endregion

    #region Data
    private void LoadData()
    {
        if (!this.NewCR)
        {
            DataTable dt = AOR.AORCRLookupList_Get(CRID: CRID);

            if (dt != null && dt.Rows.Count > 0)
            {
                lblCR.Text = Uri.UnescapeDataString(dt.Rows[0]["CRName"].ToString());
            }

            DataTable dtAOR = AOR.AORCRList_Get(AORID: 0, AORReleaseID: 0, CRID: this.CRID);

            if (dtAOR != null) this.AORCount = dtAOR.Rows.Count;

            DataTable dtSR = AOR.AORSRList_Get(CRID: this.CRID);

            if (dtSR != null) this.SRCount = dtSR.Rows.Count;
        }
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string Search(string cr)
    {
        DataTable dt = new DataTable();
        int primarySR = -1;
        try
        {
            int.TryParse(cr, out primarySR);

            dt = AOR.AORCRList_Search(PrimarySR: primarySR);
            dt = dt.DefaultView.ToTable();

            dt.AcceptChanges();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}