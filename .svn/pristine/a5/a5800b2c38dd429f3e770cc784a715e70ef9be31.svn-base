using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_Meeting_Edit : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAORMeeting = false;
    protected bool NewAORMeeting = false;
    protected int AORMeetingID = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEditAORMeeting = UserManagement.UserCanEdit(WTSModuleOption.Meeting);

        LoadControls();
        LoadData();
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

        if (Request.QueryString["NewAORMeeting"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewAORMeeting"]))
        {
            bool.TryParse(Request.QueryString["NewAORMeeting"], out this.NewAORMeeting);
        }

        if (Request.QueryString["AORMeetingID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingID"]))
        {
            int.TryParse(Request.QueryString["AORMeetingID"], out this.AORMeetingID);
        }
    }
    #endregion

    #region Data
    private void LoadControls()
    {
        DataTable dtFrequency = AOR.AORFrequencyList_Get();

        ddlFrequency.DataSource = dtFrequency;
        ddlFrequency.DataValueField = "AORFrequency_ID";
        ddlFrequency.DataTextField = "Frequency";
        ddlFrequency.DataBind();

        ListItem li = new ListItem();

        li.Value = "0";
        li.Text = "";

        ddlFrequency.Items.Insert(0, li);
    }

    private void LoadData()
    {
        if (!this.NewAORMeeting)
        {
            DataTable dtMeeting = AOR.AORMeetingList_Get(AORMeetingID: this.AORMeetingID, AORID: 0, AORReleaseID: 0);

            if (dtMeeting != null && dtMeeting.Rows.Count > 0)
            {
                spnAORMeeting.InnerText = dtMeeting.Rows[0]["AOR Meeting #"].ToString();

                string createdDateDisplay = string.Empty, updatedDateDisplay = string.Empty;
                DateTime nCreatedDate = new DateTime(), nUpdatedDate = new DateTime();

                if (DateTime.TryParse(dtMeeting.Rows[0]["CreatedDate_ID"].ToString(), out nCreatedDate)) createdDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nCreatedDate);
                if (DateTime.TryParse(dtMeeting.Rows[0]["UpdatedDate_ID"].ToString(), out nUpdatedDate)) updatedDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nUpdatedDate);

                spnCreated.InnerText = "Created: " + dtMeeting.Rows[0]["CreatedBy_ID"].ToString() + " - " + createdDateDisplay;
                spnUpdated.InnerText = "Updated: " + dtMeeting.Rows[0]["UpdatedBy_ID"].ToString() + " - " + updatedDateDisplay;
                txtAORMeetingName.Text = dtMeeting.Rows[0]["AOR Meeting Name"].ToString();
                txtAORMeetingName.Attributes.Add("original_value", dtMeeting.Rows[0]["AOR Meeting Name"].ToString());
                txtDescription.Text = dtMeeting.Rows[0]["Description"].ToString();
                txtNotes.Text = dtMeeting.Rows[0]["Notes_ID"].ToString();
                ddlFrequency.SelectedValue = dtMeeting.Rows[0]["AORFrequency_ID"].ToString();
                chkAutoCreateMeetings.Checked = dtMeeting.Rows[0]["AutoCreateMeetings_ID"].ToString() == "True";
                chkPrivate.Checked = dtMeeting.Rows[0]["Private_ID"].ToString() == "True";
            }
        }
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string Save(string blnNewAORMeeting, string aorMeeting, string aorMeetingName, string description, string notes, string frequency, int autoCreateMeetings, int privateMeeting, string newMeetingInstanceDate)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "newID", "0" }, { "error", "" } };

        try
        {
            bool New_AORMeeting = false;
            int AORMeeting_ID = 0, AORFrequency_ID = 0;
            DateTime dt = DateTime.MinValue;

            bool.TryParse(blnNewAORMeeting, out New_AORMeeting);
            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(frequency, out AORFrequency_ID);
            DateTime.TryParse(newMeetingInstanceDate, out dt);

            result = AOR.AORMeeting_Save(NewAORMeeting: New_AORMeeting, AORMeetingID: AORMeeting_ID, AORMeetingName: aorMeetingName, Description: description, Notes: notes, AORFrequencyID: AORFrequency_ID, AutoCreateMeetings: autoCreateMeetings, PrivateMeeting: privateMeeting);

            if (New_AORMeeting) // also create the instance
            {
                int newID = 0;
                Int32.TryParse(result["newID"], out newID);
                
                XmlDocument docResources = (XmlDocument)JsonConvert.DeserializeXmlNode("{\"save\":[]}", "resources");
                XmlDocument docMeetingNotes = (XmlDocument)JsonConvert.DeserializeXmlNode("{\"save\":[]}", "meetingnotes");
                XmlDocument docNoteDetails = (XmlDocument)JsonConvert.DeserializeXmlNode("{\"save\":[]}", "notedetails");

               Dictionary<string, string> instanceResult = AOR.AORMeetingInstance_Save(NewAORMeetingInstance: true, AORMeetingID: newID, AORMeetingInstanceID: 0, AORMeetingInstanceName: aorMeetingName,
               InstanceDate: dt, Notes: notes, ActualLength: 0, Resources: docResources, MeetingNotes: docMeetingNotes, NoteDetails: docNoteDetails, EndMeeting: false, LockMeeting: false, MeetingAccepted: false);

                result["newMeetingInstanceID"] = instanceResult["newID"];
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}