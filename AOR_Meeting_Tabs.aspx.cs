using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;

using Newtonsoft.Json;

using WTS;
using WTS.Enums;

public partial class AOR_Meeting_Tabs : System.Web.UI.Page
{
    #region Variables
    protected bool NewAORMeeting = false;
    protected int AORMeetingID = 0;
    protected int MeetingInstanceCount = 0;
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
        if (Request.QueryString["NewAORMeeting"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewAORMeeting"]))
        {
            bool.TryParse(Request.QueryString["NewAORMeeting"], out this.NewAORMeeting);
        }

        if (Request.QueryString["AORMeetingID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingID"]))
        {
            int.TryParse(Request.QueryString["AORMeetingID"], out this.AORMeetingID);
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
                if (dsMenu.Tables.Contains("MeetingGridRelatedItem"))
                {
                    menuRelatedItems.DataSource = dsMenu.Tables["MeetingGridRelatedItem"];
                    menuRelatedItems.DataValueField = "URL";
                    menuRelatedItems.DataTextField = "Text";
                    menuRelatedItems.DataIDField = "id";
                    if (dsMenu.Tables["MeetingGridRelatedItem"].Columns.Contains("MeetingGridRelatedItem_id_0"))
                    {
                        menuRelatedItems.DataParentIDField = "MeetingGridRelatedItem_id_0";
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
        if (!this.NewAORMeeting)
        {
            DataTable dt = AOR.AORMeetingList_Get(AORMeetingID: this.AORMeetingID, AORID: 0, AORReleaseID: 0);

            if (dt != null && dt.Rows.Count > 0)
            {
                lblAORMeeting.Text = dt.Rows[0]["AOR Meeting Name"].ToString();
            }

            DataTable dtMeetingInstance = AOR.AORMeetingInstanceList_Get(AORMeetingID: this.AORMeetingID, AORMeetingInstanceID: 0, InstanceFilterID: 0);

            if (dtMeetingInstance != null) this.MeetingInstanceCount = dtMeetingInstance.Rows.Count;
        }
    }
    #endregion

    [WebMethod()]
    public static string GetMeetingMetrics(int AORMeetingID)
    {
        return AOR.GetMeetingMetricsResult(AORMeetingID, 0);
    }
}