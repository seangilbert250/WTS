﻿using Newtonsoft.Json;
using System;
using System.Data;
using System.Web.Services;

public partial class AOR_Tabs : System.Web.UI.Page
{
    #region Variables
    protected bool CanEditAOR = false;
    protected bool NewAOR = false;
    protected int AORID = 0;
    protected int AORReleaseID = 0;
    protected string Source = string.Empty;
    protected int AttachmentCount = 0;
    protected int MeetingCount = 0;
    protected int CRCount = 0;
    protected int TaskCount = 0;
    protected int ResourceTeamCount = 0;
    protected int ActionTeamCount = 0;
    protected string Tab = string.Empty;
    protected bool Current = false;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.Current = AOR.AORReleaseCurrent(AORID: this.AORID, AORReleaseID: this.AORReleaseID);
        this.CanEditAOR = (UserManagement.UserCanEdit(WTSModuleOption.AOR) && Current);
        LoadRelatedItemsMenu();
        LoadData();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["NewAOR"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewAOR"]))
        {
            bool.TryParse(Request.QueryString["NewAOR"], out this.NewAOR);
        }

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.AORID);
        }

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.AORReleaseID);
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
                if (dsMenu.Tables.Contains("AORGridRelatedItem"))
                {
                    menuRelatedItems.DataSource = dsMenu.Tables["AORGridRelatedItem"];
                    menuRelatedItems.DataValueField = "URL";
                    menuRelatedItems.DataTextField = "Text";
                    menuRelatedItems.DataIDField = "id";
                    if (dsMenu.Tables["AORGridRelatedItem"].Columns.Contains("AORGridRelatedItem_id_0"))
                    {
                        menuRelatedItems.DataParentIDField = "AORGridRelatedItem_id_0";
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
        if (!this.NewAOR)
        {
            DataTable dt = AOR.AORList_Get(AORID: AORID, includeArchive: 1);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    if (dr["AORRelease_ID"].ToString() == AORReleaseID.ToString())
                    {
                        lblAOR.Text = AORID + " - " + dr["AOR Name"].ToString() + (!this.Current ? " (Past Release)" : "");
                    }
                }
                ddlRelease.DataSource = dt;
                ddlRelease.DataValueField = "AORRelease_ID";
                ddlRelease.DataTextField = "Current Release";
                ddlRelease.DataBind();
                ddlRelease.SelectedValue = this.AORReleaseID.ToString();
            }

            DataTable dtAttachment = AOR.AORAttachmentList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID);

            if (dtAttachment != null) this.AttachmentCount = dtAttachment.Rows.Count;

            DataTable dtMeeting = AOR.AORMeetingList_Get(AORMeetingID: 0, AORID: this.AORID, AORReleaseID: this.AORReleaseID);

            if (dtMeeting != null) this.MeetingCount = dtMeeting.Rows.Count;

            DataTable dtAlert = AOR.AORAlertList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID);

            if (dtAlert != null && dtAlert.Rows.Count > 0) imgAlert.Style["display"] = "inline";
        }
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string Search(string aor)
    {
        DataTable dt = new DataTable();
        int aorID = -1;
        try
        {
            int.TryParse(aor, out aorID);
            dt = AOR.AORList_Get(AORID: aorID, includeArchive: 1);
            dt.DefaultView.RowFilter = "Current_ID = True";
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
    #endregion
}