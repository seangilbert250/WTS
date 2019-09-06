﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

using WTS;
using WTS.Enums;
using WTS.Events;
using WTS.Util;

public partial class AOR_Meeting_Instance_Edit : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAORMeetingInstance = false;
    protected bool CanEditAORMeetingInstanceAlt = false;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected bool NewAORMeetingInstance = false;
    protected int AORMeetingID = 0;
    protected int AORMeetingInstanceID = 0;
    protected bool Locked = true;
    protected string Download = string.Empty;
    protected string DownloadSettings = string.Empty;
    protected string NoteAOROptions = string.Empty;
    protected string NoteStatusOptions = string.Empty;
    protected string NoteTypeOptions = string.Empty;
    protected bool MeetingEnded = false;
    protected bool MeetingAccepted = false;
    protected bool ShowRemoved = false;
    protected int OriginatingAORID = 0;
    protected int OriginatingAORReleaseID = 0;
    protected bool ForceUnlock = false;
    protected bool PreviousMeetingAccepted = false;
    protected int PreviousMeetingInstanceID = 0;
    protected DateTime PreviousMeetingDate = DateTime.MinValue;

    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        if (this.Download != string.Empty)
        {
            ExportData(this.Download, null, ShowRemoved);
        }
        else
        {
            if (ForceUnlock)
            {
                // we do this first so the flags below can be set correctly
                AOR.AORMeetingInstance_ToggleMeetingLock(AORMeetingInstanceID, false, "AUTO-UNLOCK for editing", WTSPage.GetLoggedInUserID());
            }

            this.CanEditAORMeetingInstance = UserManagement.UserCanEdit(WTSModuleOption.Meeting);
            this.CanEditAORMeetingInstanceAlt = this.CanEditAORMeetingInstance;
            this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
            this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);
            this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
            this.CanViewWorkItem = this.CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);
            this.Locked = AOR.AORMeetingInstanceLocked(this.AORMeetingInstanceID);
            
            if (this.Locked) this.CanEditAORMeetingInstance = false;

            if (!Page.IsPostBack)
            {
                LoadControls();
                LoadData();
            }
        }

        Request.ServerVariables["NeedsPDFViewer"] = "true";
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

        if (Request.QueryString["NewAORMeetingInstance"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewAORMeetingInstance"]))
        {
            bool.TryParse(Request.QueryString["NewAORMeetingInstance"], out this.NewAORMeetingInstance);
        }

        if (Request.QueryString["AORMeetingID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingID"]))
        {
            int.TryParse(Request.QueryString["AORMeetingID"], out this.AORMeetingID);
        }

        if (Request.QueryString["AORMeetingInstanceID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingInstanceID"]))
        {
            int.TryParse(Request.QueryString["AORMeetingInstanceID"], out this.AORMeetingInstanceID);
        }

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.OriginatingAORID);
        }

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.OriginatingAORReleaseID);
        }

        if (Request.QueryString["Download"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Download"]))
        {
            this.Download = Request.QueryString["Download"];
        }

        if (Request.QueryString["DownloadSettings"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["DownloadSettings"]))
        {
            this.DownloadSettings = Request.QueryString["DownloadSettings"];
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["ShowRemoved"]))
        {
            this.ShowRemoved = Request.QueryString["ShowRemoved"].ToLower() == "true";
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["ForceUnlock"]))
        {
            this.ForceUnlock = Request.QueryString["ForceUnlock"].ToLower() == "true";
        }

        if (AORMeetingInstanceID == 0)
        {
            NewAORMeetingInstance = true;
        }
    }
    #endregion

    #region Data
    private void LoadControls()
    {
        DataTable dtNoteTypeList = AOR.AORNoteTypeList_Get();      

        cblDownloadPDFSettings.DataSource = dtNoteTypeList;
        cblDownloadPDFSettings.DataValueField = "Note Type";
        cblDownloadPDFSettings.DataBind();

        StringBuilder str = new StringBuilder();
        foreach (DataRow row in dtNoteTypeList.Rows)
        {
            str.Append("<option value=\"" + row["AORNoteType_ID"].ToString()  + "\">" + row["Note Type"].ToString() + "</option>");
        }
        NoteTypeOptions = str.ToString();

        ListItem li = new ListItem();

        li.Value = "AORs Included";
        cblDownloadPDFSettings.Items.Insert(0, li);

        li = cblDownloadPDFSettings.Items.FindByText("Burndown Overview");
        li.Value = "BurndownOverviewParent";
        li.Text = "Burndown Overview";
        int burndownIdx = cblDownloadPDFSettings.Items.IndexOf(li);

        li = new ListItem("Notes", "Burndown Overview");
        li.Attributes.Add("style", "padding-left:20px");
        cblDownloadPDFSettings.Items.Insert(burndownIdx + 1, li);

        li = new ListItem("Grid", "Burndown Grid");
        li.Attributes.Add("style", "padding-left:20px");
        cblDownloadPDFSettings.Items.Insert(burndownIdx + 1, li);

        DataSet dsOptions = AOR.AOROptionsList_Get(AORID: 0, TaskID: 0, AORMeetingID: this.AORMeetingID, AORMeetingInstanceID: this.AORMeetingInstanceID);

        if (dsOptions != null)
        {
            DataTable dtNoteType = dsOptions.Tables["Note Type"];

            ddlNoteType.DataSource = dtNoteType;
            ddlNoteType.DataValueField = "Value";
            ddlNoteType.DataTextField = "Text";
            ddlNoteType.DataBind();
        }

        li = new ListItem();

        li.Value = "0";
        li.Text = "- All -";

        ddlNoteType.Items.Insert(0, li);

        DataTable dtNoteAOR = AOR.AORMeetingInstanceAddList_Get(AORMeetingID: this.AORMeetingID, AORMeetingInstanceID: AORMeetingInstanceID, Type: "Edit Note Detail", QFSystem: string.Empty, QFRelease: string.Empty, InstanceFilterID: 0, NoteTypeFilterID: 0);

        this.NoteAOROptions = "<option value=\"0\"></option>";

        foreach (DataRow dr in dtNoteAOR.Rows)
        {
            string nText = string.Empty;

            switch (dr["WorkloadAllocation"].ToString().ToUpper())
            {
                case "RELEASE CAFDEX":
                    nText = "(R) ";
                    break;
                case "PRODUCTION SUPPORT":
                    nText = "(P) ";
                    break;
                default:
                    nText = "(O) ";
                    break;
            }

            this.NoteAOROptions += "<option value=\"" + dr["AORReleaseID"] + "\">" + nText + dr["AORID"].ToString() + " - " + Uri.EscapeDataString(dr["AORName"].ToString()) + "</option>";
        }

        DataTable dtNoteStatus = MasterData.StatusList_Get(includeArchive: false);

        dtNoteStatus.DefaultView.RowFilter = "StatusType IN ('Note')";
        dtNoteStatus = dtNoteStatus.DefaultView.ToTable();

        foreach (DataRow dr in dtNoteStatus.Rows)
        {
            this.NoteStatusOptions += "<option value=\"" + dr["STATUSID"].ToString() + "\">" + Uri.EscapeDataString(dr["STATUS"].ToString()) + "</option>";
        }
    }

    private void LoadData()
    {
        if (!this.NewAORMeetingInstance)
        {
            DataTable dtMeetingInstance = AOR.AORMeetingInstanceList_Get(AORMeetingID: this.AORMeetingID, AORMeetingInstanceID: this.AORMeetingInstanceID, InstanceFilterID: 0);

            if (dtMeetingInstance != null && dtMeetingInstance.Rows.Count > 0)
            {
                spnAORMeetingInstanceHeader.InnerText = dtMeetingInstance.Rows[0]["Meeting Instance #"].ToString() + " - " + dtMeetingInstance.Rows[0]["Meeting Instance Name"].ToString();
                spnAORMeetingInstance.InnerText = dtMeetingInstance.Rows[0]["Meeting Instance #"].ToString();

                string createdDateDisplay = string.Empty, updatedDateDisplay = string.Empty;
                DateTime nCreatedDate = new DateTime(), nUpdatedDate = new DateTime();

                if (DateTime.TryParse(dtMeetingInstance.Rows[0]["CreatedDate_ID"].ToString(), out nCreatedDate)) createdDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nCreatedDate);
                if (DateTime.TryParse(dtMeetingInstance.Rows[0]["UpdatedDate_ID"].ToString(), out nUpdatedDate)) updatedDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nUpdatedDate);

                spnCreated.InnerText = "Created: " + dtMeetingInstance.Rows[0]["CreatedBy_ID"].ToString() + " - " + createdDateDisplay;
                spnUpdated.InnerText = "Updated: " + dtMeetingInstance.Rows[0]["UpdatedBy_ID"].ToString() + " - " + updatedDateDisplay;
                txtAORMeetingInstanceName.Text = dtMeetingInstance.Rows[0]["Meeting Instance Name"].ToString();

                DateTime nDate = new DateTime();

                if (DateTime.TryParse(dtMeetingInstance.Rows[0]["Instance Date"].ToString(), out nDate))
                {
                    txtInstanceDate.Text = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    txtInstanceDate.Attributes.Add("origvalue", txtInstanceDate.Text);
                }

                txtNotes.Text = dtMeetingInstance.Rows[0]["Notes_ID"].ToString();
                txtActualLength.Text = dtMeetingInstance.Rows[0]["Actual Length"].ToString();

                MeetingEnded = (bool)dtMeetingInstance.Rows[0]["MeetingEnded"];
                MeetingAccepted = (bool)dtMeetingInstance.Rows[0]["MeetingAccepted"];

                PreviousMeetingAccepted = AOR.AORMeetingInstance_HasPreviousMeetingBeenAccepted(this.AORMeetingID, this.AORMeetingInstanceID, out PreviousMeetingInstanceID, out PreviousMeetingDate);
            }

            PopulateTreeViewControl(tvNoteByAOR, tvNoteByType, this.AORMeetingID, this.AORMeetingInstanceID, false);
        }
    }

    private void ExportData(string type, string emailRecipients = null, bool showRemoved = false)
    {
        int newAttachmentID = 0;

        AOR.ExportMeetingInstanceData(type, this.DownloadSettings, this.AORMeetingID, this.AORMeetingInstanceID, emailRecipients, null, false, 0, out newAttachmentID, showRemoved, Response, Server);
    }

    #endregion

    #region AJAX
    [WebMethod()]
    public static string GetAORs(string aorMeeting, string aorMeetingInstance, string blnShowRemoved)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;
            bool Show_Removed = false;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);
            bool.TryParse(blnShowRemoved, out Show_Removed);

            dt = AOR.AORMeetingInstanceAORList_Get(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, ShowRemoved: Show_Removed);

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add("DateAddedString");

                foreach (DataRow dr in dt.Rows)
                {
                    DateTime nDate = new DateTime();
                    
                    if (DateTime.TryParse(dr["DateAdded"].ToString(), out nDate))
                    {
                        dr["DateAddedString"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    }
                }

                dt.AcceptChanges();
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string ToggleAOR(string aorMeeting, string aorMeetingInstance, int AORRelease_ID, int opt)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "error", "" } };

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);

            result = AOR.AORMeetingInstanceAOR_Toggle(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, AORReleaseID: AORRelease_ID, Opt: opt);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetSRs(string aorMeeting, string aorMeetingInstance, int AORRelease_ID, string blnShowClosed)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;
            bool Show_Closed = false;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);
            bool.TryParse(blnShowClosed, out Show_Closed);

            dt = AOR.AORMeetingInstanceSRList_Get(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, AORReleaseID: AORRelease_ID, ShowClosed: Show_Closed);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetTasks(string aorMeeting, string aorMeetingInstance, int AORRelease_ID, string blnShowClosed)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;
            bool Show_Closed = false;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);
            bool.TryParse(blnShowClosed, out Show_Closed);

            dt = AOR.AORMeetingInstanceTaskList_Get(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, AORReleaseID: AORRelease_ID, ShowClosed: Show_Closed);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetResources(string aorMeeting, string aorMeetingInstance, string blnShowRemoved)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;
            bool Show_Removed = false;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);
            bool.TryParse(blnShowRemoved, out Show_Removed);

            dt = AOR.AORMeetingInstanceResourceList_Get(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, ShowRemoved: Show_Removed);

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add("LastMeetingAttendedString");

                foreach (DataRow dr in dt.Rows)
                {
                    DateTime nDate = new DateTime();

                    if (DateTime.TryParse(dr["LastMeetingAttended"].ToString(), out nDate))
                    {
                        dr["LastMeetingAttendedString"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    }
                    else
                    {
                        dr["LastMeetingAttendedString"] = string.Empty;
                    }
                }

                dt.AcceptChanges();
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string ToggleResource(string aorMeeting, string aorMeetingInstance, int resourceID, int opt)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "error", "" } };

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);

            result = AOR.AORMeetingInstanceResource_Toggle(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, WTS_RESOURCEID: resourceID, Opt: opt);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetAttachments(string aorMeetingInstance)
    {
        DataTable dt = AOR.AORMeetingInstanceAttachment_Get(0, 0, Int32.Parse(aorMeetingInstance), 0, false);

        return JsonConvert.SerializeObject(dt != null ? dt.Rows.Count : 0, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetNotes(string aorMeeting, string aorMeetingInstance, string aorNoteType, string blnShowRemoved, string aor)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0, AORNoteType_ID = 0, AOR_ID = 0;
            bool Show_Removed = false;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);
            int.TryParse(aorNoteType, out AORNoteType_ID);
            int.TryParse(aor, out AOR_ID);
            bool.TryParse(blnShowRemoved, out Show_Removed);

            dt = AOR.AORMeetingInstanceNotesList_Get(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, AORNoteTypeID: AORNoteType_ID, ShowRemoved: Show_Removed);

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add("AddDateString");

                foreach (DataRow dr in dt.Rows)
                {
                    DateTime nDate = new DateTime();

                    if (DateTime.TryParse(dr["AddDate"].ToString(), out nDate))
                    {
                        dr["AddDateString"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    }
                    else
                    {
                        dr["AddDateString"] = string.Empty;
                    }
                }

                dt.AcceptChanges();
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string ToggleNote(string aorMeeting, string aorMeetingInstance, int AORMeetingNotes_ID, int opt)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "error", "" } };

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);

            result = AOR.AORMeetingInstanceNote_Toggle(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, AORMeetingNotesID: AORMeetingNotes_ID, Opt: opt);

            if (opt == 0)
            {
                // when we remove notes, we check to see if the note is linked to an agenda item, and if so, we clean up the agenda if the agenda objective has createnote enabled
                DataTable dt = AOR.AORMeetingInstanceSelectedNoteDetail_Get(AORMeetingNotesID: AORMeetingNotes_ID, ShowRemoved: true, ShowClosed: true);
                if (dt.Rows.Count > 0)
                {
                    DataRow noteRow = dt.Rows[0];
                    string noteTypeName = noteRow["AORNoteTypeName"].ToString();
                    string extData = noteRow["ExtData"] != DBNull.Value ? noteRow["ExtData"].ToString() : null;

                    if (extData != null && extData.IndexOf("<agendaitemkey>") != -1 && noteTypeName != "Agenda/Objective") // this note is linked to an agenda item
                    {
                        int idx = extData.IndexOf("<agendaitemkey>");
                        int len = "<agendaitemkey>".Length;
                        int idx2 = extData.IndexOf("</agendaitemkey>");
                        string key = extData.Substring(idx + len, idx2 - (idx + len));

                        // find the affected agenda
                        DataTable agendaNotes = AOR.AORMeetingInstanceNotesDetailList_Get(0, false, true, 0, AORMeetingInstance_ID, (int)WTS.Enums.NoteTypeEnum.AgendaObjectives);

                        for (int i = 0; i < agendaNotes.Rows.Count; i++)
                        {
                            DataRow row = agendaNotes.Rows[i];

                            int agendaNoteID = (int)row["AORMeetingNotesID"];
                            int agendaNoteTypeID = (int)WTS.Enums.NoteTypeEnum.AgendaObjectives;
                            int agendaAORReleaseID = row["AORReleaseID"] != DBNull.Value ? (int)row["AORReleaseID"] : 0;
                            string agendaTitle = row["Title"] != DBNull.Value ? (string)row["Title"] : null;
                            string agendaNoteDetail = row["Notes"] != DBNull.Value ? (string)row["Notes"] : null;
                            string agendaExtData = row["ExtData"] != DBNull.Value ? (string)row["ExtData"] : null;

                            if (!string.IsNullOrWhiteSpace(agendaExtData) && agendaExtData.IndexOf("<objectivesjson>") != -1) // skip notes with no objectives tied to them (old agenda notes or regular notes converted to agendas)
                            {
                                List<Dictionary<string, string>> objectives = AOR.ParseObjectivesJson(agendaExtData);

                                bool changeMadeToThisAgenda = false; // we could have multiple agendas, so some may not be touched

                                // now that we have the agenda note, modify it with any changes found in the notes we are saving
                                // we only kept the notes that affected agendas in the notes collection
                                Dictionary<string, string> affectedObjective = objectives.Find(obj => obj["key"] == key);
                                if (affectedObjective != null && affectedObjective["createnote"] == "true") // if false, we aren't linked any more so don't update
                                {
                                    // this note affects the current agenda
                                    // only a couple of properties can be touched (title, text, deleted or not)
                                    affectedObjective["key"] = "REMOVED";

                                    changeMadeToThisAgenda = true;
                                }

                                if (changeMadeToThisAgenda)
                                {
                                    // we need to resave the agenda AND regenerate the note text to account for the updates
                                    agendaNoteDetail = AOR.GenerateAgendaFromObjectivesList(objectives);
                                    string objectivesJson = JsonConvert.SerializeObject(objectives);
                                    if (objectivesJson == null) objectivesJson = "";
                                    string newExtData = "<objectivesjson>" + objectivesJson.Replace("\"", "\\\"") + "</objectivesjson>";

                                    string changeTemplate = "{{save:[{{\"aormeetingnotesid\":\"{0}\",\"aornotetypeid\":\"{1}\",\"aorreleaseid\":\"{2}\",\"title\":\"{3}\",\"notedetail\":\"{4}\",\"extdata\":\"{5}\"}}]}}";

                                    string addition = string.Format(changeTemplate, agendaNoteID.ToString(), (int)WTS.Enums.NoteTypeEnum.AgendaObjectives, agendaAORReleaseID, agendaTitle.Replace("\"", "\\\""), agendaNoteDetail.ToString().Replace("\"", "\\\""), newExtData);
                                    var docAdditions = (XmlDocument)JsonConvert.DeserializeXmlNode(addition, "additions");
                                    AOR.AORMeetingInstanceAdd_Save(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, Type: "Edit Note Detail", Additions: docAdditions);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetSelectedNoteDetail(string aorMeetingNotesID, string blnShowRemoved, string blnShowClosed)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORMeetingNotes_ID = 0;
            bool Show_Removed = false, Show_Closed = false;

            int.TryParse(aorMeetingNotesID, out AORMeetingNotes_ID);
            bool.TryParse(blnShowRemoved, out Show_Removed);
            bool.TryParse(blnShowClosed, out Show_Closed);

            dt = AOR.AORMeetingInstanceSelectedNoteDetail_Get(AORMeetingNotesID: AORMeetingNotes_ID, ShowRemoved: Show_Removed, ShowClosed: Show_Closed);

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add("AddDateString");
                dt.Columns.Add("StatusDateString");

                foreach (DataRow dr in dt.Rows)
                {
                    DateTime nDate = new DateTime();

                    if (DateTime.TryParse(dr["AddDate"].ToString(), out nDate))
                    {
                        dr["AddDateString"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    }
                    else
                    {
                        dr["AddDateString"] = string.Empty;
                    }

                    nDate = new DateTime();

                    if (DateTime.TryParse(dr["StatusDate"].ToString(), out nDate))
                    {
                        dr["StatusDateString"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    }
                    else
                    {
                        dr["StatusDateString"] = string.Empty;
                    }
                }

                dt.AcceptChanges();
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetNotesDetail(string aorMeetingNotesID, string blnShowRemoved, string blnShowClosed, int aorID, int noteTypeID, int AORMeetingInstanceID)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORMeetingNotes_ID = 0;
            bool Show_Removed = false, Show_Closed = false;
            int.TryParse(aorMeetingNotesID, out AORMeetingNotes_ID);
            bool.TryParse(blnShowRemoved, out Show_Removed);
            bool.TryParse(blnShowClosed, out Show_Closed);

            dt = AOR.AORMeetingInstanceNotesDetailList_Get(AORMeetingNotesID_Parent: AORMeetingNotes_ID, ShowRemoved: Show_Removed, ShowClosed: Show_Closed, AORID:aorID, AORMeetingInstanceID: AORMeetingInstanceID, NoteTypeID:noteTypeID);

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add("AddDateString");
                dt.Columns.Add("StatusDateString");

                foreach (DataRow dr in dt.Rows)
                {
                    DateTime nDate = new DateTime();

                    if (DateTime.TryParse(dr["AddDate"].ToString(), out nDate))
                    {
                        dr["AddDateString"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    }
                    else
                    {
                        dr["AddDateString"] = string.Empty;
                    }

                    nDate = new DateTime();

                    if (DateTime.TryParse(dr["StatusDate"].ToString(), out nDate))
                    {
                        dr["StatusDateString"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    }
                    else
                    {
                        dr["StatusDateString"] = string.Empty;
                    }
                }

                dt.AcceptChanges();
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetAORProgress(string aorMeeting, string aorMeetingInstance)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);

            dt = AOR.AORMeetingInstanceAORProgress_Get(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetHistory(string aorMeeting, string aorMeetingInstance)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);

            dt = AOR.AORMeetingInstanceList_Get(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: 0, InstanceFilterID: AORMeetingInstance_ID);

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["Meeting Instance #"].ColumnName = "AORMeetingInstanceID";
                dt.Columns["Meeting Instance Name"].ColumnName = "AORMeetingInstanceName";

                dt.Columns.Add("InstanceDateString");

                foreach (DataRow dr in dt.Rows)
                {
                    DateTime nDate = new DateTime();

                    if (DateTime.TryParse(dr["Instance Date"].ToString(), out nDate))
                    {
                        dr["InstanceDateString"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    }
                    else
                    {
                        dr["InstanceDateString"] = string.Empty;
                    }
                }

                dt.AcceptChanges();
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string Save(string blnNewAORMeetingInstance, string aorMeeting, string aorMeetingInstance, string aorMeetingInstanceName, string instanceDate, string notes, string actualLength,
        string resources, string meetingNotes, string noteDetails, string endMeeting, string lockMeeting, string meetingAccepted, int origAORID, int origAORReleaseID, string preSaveMeetingEndedValue)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

        try
        {
            bool New_AORMeetingInstance = false;
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0, nActualLength = -999;
            DateTime nDate = new DateTime();
            XmlDocument docResources = (XmlDocument)JsonConvert.DeserializeXmlNode(resources, "resources");
            XmlDocument docMeetingNotes = (XmlDocument)JsonConvert.DeserializeXmlNode(meetingNotes, "meetingnotes");
            XmlDocument docNoteDetails = (XmlDocument)JsonConvert.DeserializeXmlNode(noteDetails, "notedetails");

            bool.TryParse(blnNewAORMeetingInstance, out New_AORMeetingInstance);
            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);
            DateTime.TryParse(instanceDate, out nDate);

            if (actualLength != "") int.TryParse(actualLength, out nActualLength);

            if (!New_AORMeetingInstance)
            {
                origAORID = 0;
                origAORReleaseID = 0;
            }
            
            result = AOR.AORMeetingInstance_Save(NewAORMeetingInstance: New_AORMeetingInstance, AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, AORMeetingInstanceName: aorMeetingInstanceName,
                InstanceDate: nDate, Notes: notes, ActualLength: nActualLength, Resources: docResources, MeetingNotes: docMeetingNotes, NoteDetails: docNoteDetails, EndMeeting: endMeeting == "true", LockMeeting: lockMeeting == "true", MeetingAccepted: meetingAccepted == "true");

            result["AORMeetingID"] = AORMeeting_ID.ToString(); // needed for new meetings where the aor is selected dynamically and not present on the query string

            if (AORMeetingInstance_ID > 0 && noteDetails.IndexOf("<agendaitemkey>") != -1)
            {
                if (docNoteDetails.HasChildNodes && docNoteDetails.ChildNodes.Count > 0)
                {                    
                    XmlNode noteDetailsNode = docNoteDetails.ChildNodes[0];
                    ModifyAgendaAfterSave(AORMeeting_ID, AORMeetingInstance_ID, noteDetailsNode);
                }
            }

            if (endMeeting == "true" && preSaveMeetingEndedValue == "false") // we don't want to regenerate the end-meeting-export document more than once
            {
                // create a PDF of meeting and attach it to meeting if the meeting hasn't already been ended previously
                int newAttachmentID = 0;

                AOR.ExportMeetingInstanceData("pdf", "AORs Included,Items,Agenda/Objectives,Burndown Overview,Notes,Questions/Discussion Points,Stopping Conditions", AORMeeting_ID, AORMeetingInstance_ID, null, null, true, (int)WTS.Enums.AttachmentTypeEnum.MeetingMinutes, out newAttachmentID, false, System.Web.HttpContext.Current.Response, System.Web.HttpContext.Current.Server);
            }

            // after we've finished adding the new meeting, copying data, copying resources, etc..., we optionally can add the originating AOR Release
            // (this will happen if we are creating a new instance from the aor page)
            if (New_AORMeetingInstance && origAORID > 0 && origAORReleaseID > 0)
            {
                AORMeetingInstance_ID = Int32.Parse(result["newID"]);
                string additions = "{save:[{\"aorreleaseid\":\"" + origAORReleaseID + "\"}]}";
                XmlDocument docAdditions = (XmlDocument)JsonConvert.DeserializeXmlNode(additions, "additions");
                AOR.AORMeetingInstanceAdd_Save(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, Type: "AOR", Additions: docAdditions);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    private static void ModifyAgendaAfterSave(int AORMeeting_ID, int AORMeetingInstance_ID, XmlNode noteDetailsNode)
    {
        List<Dictionary<string, string>> notes = new List<Dictionary<string, string>>();
        bool agendaKeyFound = false;

        // parse the notes (we do this first so we can avoid loading the agendas if no agenda item keys are found)
        for (int i = 0; noteDetailsNode != null && i < noteDetailsNode.ChildNodes.Count; i++)
        {
            Dictionary<string, string> note = new Dictionary<string, string>();
            bool noteHasAgendaKey = false;

            XmlNode saveNode = noteDetailsNode.ChildNodes[i];

            for (int j = 0; j < saveNode.ChildNodes.Count; j++)
            {
                XmlNode noteNode = saveNode.ChildNodes[j];

                note.Add(noteNode.Name, noteNode.InnerText);

                if (noteNode.Name == "extdata")
                {
                    string extdata = noteNode.InnerText;

                    if (!string.IsNullOrWhiteSpace(extdata) && extdata.IndexOf("<agendaitemkey>") != -1)
                    {
                        agendaKeyFound = true;
                        noteHasAgendaKey = true;
                    }
                }
            }

            if (noteHasAgendaKey)
            {
                notes.Add(note);
            }
        }

        if (agendaKeyFound && notes.Count > 0)
        {
            // one or more notes being saved has an agenda item link; therefore load the agenda item and modify it if the agenda row item has the "create note / link note" option checked
            DataTable agendaNotes = AOR.AORMeetingInstanceNotesDetailList_Get(0, false, true, 0, AORMeetingInstance_ID, (int)WTS.Enums.NoteTypeEnum.AgendaObjectives);            

            for (int i = 0; i < agendaNotes.Rows.Count; i++)
            {
                DataRow row = agendaNotes.Rows[i];

                int agendaNoteID = (int)row["AORMeetingNotesID"];
                int agendaNoteTypeID = (int)WTS.Enums.NoteTypeEnum.AgendaObjectives;
                int agendaAORReleaseID = row["AORReleaseID"] != DBNull.Value ? (int)row["AORReleaseID"] : 0;
                string agendaTitle = row["Title"] != DBNull.Value ? (string)row["Title"] : null;
                string agendaNoteDetail = row["Notes"] != DBNull.Value ? (string)row["Notes"] : null;
                string agendaExtData = row["ExtData"] != DBNull.Value ? (string)row["ExtData"] : null;

                if (!string.IsNullOrWhiteSpace(agendaExtData) && agendaExtData.IndexOf("<objectivesjson>") != -1) // skip notes with no objectives tied to them (old agenda notes or regular notes converted to agendas)
                {
                    List<Dictionary<string, string>> objectives = AOR.ParseObjectivesJson(agendaExtData);

                    bool changeMadeToThisAgenda = false; // we could have multiple agendas, so some may not be touched

                    // now that we have the agenda note, modify it with any changes found in the notes we are saving
                    // we only kept the notes that affected agendas in the notes collection
                    for (int j = 0; j < notes.Count; j++)
                    {
                        string extData = notes[j]["extdata"];
                        int idx = extData.IndexOf("<agendaitemkey>");
                        int len = "<agendaitemkey>".Length;
                        int idx2 = extData.IndexOf("</agendaitemkey>");
                        string key = extData.Substring(idx + len, idx2 - (idx + len));

                        Dictionary<string, string> affectedObjective = objectives.Find(obj => obj["key"] == key);
                        if (affectedObjective != null && affectedObjective["createnote"] == "true") // if false, we aren't linked any more so don't update
                        {
                            // this note affects the current agenda
                            // only a couple of properties can be touched (title, text, deleted or not)
                            affectedObjective["title"] = StringUtil.StrongEscape(notes[j]["title"]);
                            affectedObjective["notes"] = StringUtil.StrongEscape(notes[j]["notedetails"]);

                            changeMadeToThisAgenda = true;
                        }
                    }

                    if (changeMadeToThisAgenda)
                    {
                        // we need to resave the agenda AND regenerate the note text to account for the updates
                        agendaNoteDetail = AOR.GenerateAgendaFromObjectivesList(objectives);
                        string objectivesJson = JsonConvert.SerializeObject(objectives);
                        if (objectivesJson == null) objectivesJson = "";
                        string extData = "<objectivesjson>" + objectivesJson.Replace("\"", "\\\"") + "</objectivesjson>";

                        string changeTemplate = "{{save:[{{\"aormeetingnotesid\":\"{0}\",\"aornotetypeid\":\"{1}\",\"aorreleaseid\":\"{2}\",\"title\":\"{3}\",\"notedetail\":\"{4}\",\"extdata\":\"{5}\"}}]}}";

                        string addition = string.Format(changeTemplate, agendaNoteID.ToString(), (int)WTS.Enums.NoteTypeEnum.AgendaObjectives, agendaAORReleaseID, agendaTitle.Replace("\"", "\\\""), agendaNoteDetail.ToString().Replace("\"", "\\\""), extData);
                        var docAdditions = (XmlDocument)JsonConvert.DeserializeXmlNode(addition, "additions");
                        AOR.AORMeetingInstanceAdd_Save(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, Type: "Edit Note Detail", Additions: docAdditions);
                    }
                }                
            }
        }        
    }

    [WebMethod()]
    public static string EmailMinutes(string downloadSettings, string recipients, string aorMeeting, string aorMeetingInstance)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "false" }, { "error", "" } };

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;
            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);

            string customNote = null;
            if (recipients.StartsWith("[EMPTY],"))
            {
                recipients = recipients.Replace("[EMPTY],", "");
            }
            else
            {
                int idx = recipients.IndexOf(",");
                customNote = System.Web.HttpUtility.UrlDecode(recipients.Substring(0, idx));
                recipients = recipients.Substring(idx);
            }

            int newAttachmentID = 0;

            if (AOR.ExportMeetingInstanceData("pdf", downloadSettings, AORMeeting_ID, AORMeetingInstance_ID, recipients, customNote, false, 0, out newAttachmentID, false, System.Web.HttpContext.Current.Response, System.Web.HttpContext.Current.Server))
            {
                result["success"] = "true";
            }
        }
        catch (Exception ex)
        {
            result["error"] = ex.Message;
            LogUtility.LogException(ex);
            
        }
        
        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string LoadNotesDetailHistory(string NoteGroupID, int AORMeetingInstanceIDCutoff)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "false" }, { "error", "" } };


        DataTable dtNoteDetail = null;

        int id = 0;

        if (Int32.TryParse(NoteGroupID, out id))
        {
            dtNoteDetail = AOR.AORMeetingInstanceNoteGroupDetailList_Get(id, AORMeetingInstanceIDCutoff);

            dtNoteDetail.Columns.Add("AddDateString");
            dtNoteDetail.Columns.Add("UpdatedDateString");
            dtNoteDetail.Columns.Add("InstanceDateString");
            foreach (DataRow row in dtNoteDetail.Rows)
            {
                row["AddDateString"] = ((DateTime)row["AddDate"]).ToString("M/d/yyyy h:mm tt");
                row["UpdatedDateString"] = row["UpdatedDate"] != DBNull.Value ? ((DateTime)row["UpdatedDate"]).ToString("M/d/yyyy h:mm tt") : "";
                row["InstanceDateString"] = ((DateTime)row["InstanceDate"]).ToString("M/d/yyyy h:mm tt");
                row["UpdatedBy"] = row["UpdatedBy"] != DBNull.Value ? row["UpdatedBy"].ToString() : "";
            }
            dtNoteDetail.AcceptChanges();

            return JsonConvert.SerializeObject(dtNoteDetail, Newtonsoft.Json.Formatting.None);
        }
        else
        {
            result["error"] = "Invalid NoteGroupID.";
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    private static void PopulateTreeViewControl(Controls_SimpleTreeViewControl tvNoteByAOR, Controls_SimpleTreeViewControl tvNoteByType, int AORMeetingID, int AORMeetingInstanceID, bool showRemoved)
    {
        DataTable dtAor = AOR.AORMeetingInstanceAOR_NotesList_Get(AORMeetingID: AORMeetingID, AORMeetingInstanceID: AORMeetingInstanceID, ShowRemoved: showRemoved);

        string removedNoteColor = "#999999";

        // notes by aor, with special sections pulled out for agenda, action items, non-aor items, and burndown items shown last
        tvNoteByAOR.LastClickedNodeBackgroundColor = "#d7e8fc";
        tvNoteByAOR.ClickCallbackFunction = "noteNodeClicked";

        bool hasAgendaObjectives = false;
        bool hasActionItems = false;
        bool hasNoAORAsscoiated = false;
        List<int> notesAdded = new List<int>();

        foreach (DataRow i in dtAor.Rows)
        {
            int noteTypeID = i["AORNoteTypeID"] != DBNull.Value ? (int)i["AORNoteTypeID"] : 0;

            hasAgendaObjectives |= noteTypeID == (int)NoteTypeEnum.AgendaObjectives;
            hasActionItems |= noteTypeID == (int)NoteTypeEnum.ActionItems;

            if (noteTypeID != (int)NoteTypeEnum.AgendaObjectives && noteTypeID != (int)NoteTypeEnum.ActionItems) // agenda/action items have their own list, whereas other notes are sorted by aor and therefore need a non-aor flag
            {
                if (i["AORID"] == DBNull.Value)
                {
                    hasNoAORAsscoiated = true;
                }
            }
        }

        if (hasAgendaObjectives)
        {
            TreeViewNode agendaNode = tvNoteByAOR.AddTreeViewNode("Agenda/Objective", "ao");
            agendaNode.ExpandNodeAltText = "Show Agenda/Objectives";
            agendaNode.CollapseNodeAltText = "Hide Agenda/Objectives";

            int noteCount = 0;
            foreach (DataRow row in dtAor.Rows)
            {
                int noteTypeID = row["AORNoteTypeID"] != DBNull.Value ? (int)row["AORNoteTypeID"] : 0;

                if (noteTypeID == (int)NoteTypeEnum.AgendaObjectives)
                {
                    string closed = row["STATUS"].ToString().ToLower() == "closed" ? "*" : "";

                    TreeViewNode childNode = agendaNode.AddChildNode(row["AORMeetingNotesID"].ToString() + closed + " - " + row["Title"].ToString(), row["AORMeetingNotesID"].ToString());
                    if (row["Included"].ToString() == "0")
                    {
                        childNode.Style = "color:" + removedNoteColor + "; ";
                        childNode.AddAttribute("defaultcolor", removedNoteColor);
                    }
                    notesAdded.Add((int)row["AORMeetingNotesID"]);
                    noteCount++;
                }
            }

            agendaNode.Text += " (" + noteCount + ")";
        }

        if (hasActionItems)
        {
            TreeViewNode actionNode = tvNoteByAOR.AddTreeViewNode("Action Items", "ai");
            actionNode.ExpandNodeAltText = "Show Action Items";
            actionNode.CollapseNodeAltText = "Hide Action Items";

            int noteCount = 0;
            foreach (DataRow row in dtAor.Rows)
            {
                int noteTypeID = row["AORNoteTypeID"] != DBNull.Value ? (int)row["AORNoteTypeID"] : 0;

                if (noteTypeID == (int)NoteTypeEnum.ActionItems)
                {
                    string closed = row["STATUS"].ToString().ToLower() == "closed" ? "*" : "";
                    string extData = row["ExtData"] != DBNull.Value ? (string)row["ExtData"] : "";

                    TreeViewNode childNode = actionNode.AddChildNode(row["AORMeetingNotesID"].ToString() + closed + " - " + row["Title"].ToString(), row["AORMeetingNotesID"].ToString());
                    if (row["Included"].ToString() == "0")
                    {
                        childNode.Style = "color:" + removedNoteColor + ";";
                        childNode.AddAttribute("defaultcolor", removedNoteColor);
                    }

                    if (extData.IndexOf("<agendaitemkey>") != -1)
                    {
                        int idx = extData.IndexOf("<agendaitemkey>");
                        int len = "<agendaitemkey>".Length;
                        int idx2 = extData.IndexOf("</agendaitemkey>");
                        childNode.AddAttribute("agendaitemkey", extData.Substring(idx + len, idx2 - (idx + len)));
                    }
                    else
                    {
                        childNode.AddAttribute("agendaitemkey", "");
                    }


                    notesAdded.Add((int)row["AORMeetingNotesID"]);

                    noteCount++;
                }
            }

            actionNode.Text += " (" + noteCount + ")";
        }

        if (hasNoAORAsscoiated)
        {
            TreeViewNode noAORNode = tvNoteByAOR.AddTreeViewNode("No AOR Associated", "noaor");
            noAORNode.ExpandNodeAltText = "Show Notes";
            noAORNode.CollapseNodeAltText = "Hide Notes";

            int noteCount = 0;
            // add non-burndown notes first
            for (int b = 0; b < 2; b++)
            {
                foreach (DataRow row in dtAor.Rows)
                {
                    int noteTypeID = row["AORNoteTypeID"] != DBNull.Value ? (int)row["AORNoteTypeID"] : 0;
                    bool noAOR = row["AORID"] == DBNull.Value || (int)row["AORID"] == 0;
                    string extData = row["ExtData"] != DBNull.Value ? (string)row["ExtData"] : "";

                    if (noteTypeID != (int)NoteTypeEnum.AgendaObjectives && noteTypeID != (int)NoteTypeEnum.ActionItems && noteTypeID != 0 && noAOR
                        && ((b == 0 && noteTypeID != (int)NoteTypeEnum.BurndownOverview) || (b == 1 && noteTypeID == (int)NoteTypeEnum.BurndownOverview)))
                    {
                        string closed = row["STATUS"].ToString().ToLower() == "closed" ? "*" : "";

                        TreeViewNode childNode = noAORNode.AddChildNode(row["AORMeetingNotesID"].ToString() + closed + " - " + row["Title"].ToString() + " (" + GetNoteAbbr(noteTypeID) + ")", row["AORMeetingNotesID"].ToString());
                        childNode.AddAttribute("aorid", "00");
                        childNode.AddAttribute("aorname", "No AOR Associated");
                        if (extData.IndexOf("<agendaitemkey>") != -1)
                        {
                            int idx = extData.IndexOf("<agendaitemkey>");
                            int len = "<agendaitemkey>".Length;
                            int idx2 = extData.IndexOf("</agendaitemkey>");
                            childNode.AddAttribute("agendaitemkey", extData.Substring(idx + len, idx2 - (idx + len)));
                        }
                        else
                        {
                            childNode.AddAttribute("agendaitemkey", "");
                        }

                        if (b == 1)
                        {
                            childNode.Style = "font-style:italic;";
                        }

                        if (row["Included"].ToString() == "0")
                        {
                            if (childNode.Style == null)
                            {
                                childNode.Style = "color:" + removedNoteColor + ";";
                            }
                            else
                            {
                                childNode.Style += "color:" + removedNoteColor + ";";
                            }
                            childNode.AddAttribute("defaultcolor", removedNoteColor);
                        }
                        notesAdded.Add((int)row["AORMeetingNotesID"]);
                        noteCount++;
                    }
                }
            }

            noAORNode.Text += " (" + noteCount + ")";
        }

        var curParentNode = "";

        foreach (DataRow row in dtAor.Rows)
        {
            if (curParentNode != row["AORID"].ToString())
            {
                WTS.TreeViewNode tvn = new TreeViewNode(row["AORID"].ToString() + " - " + row["AORName"].ToString(), row["AORID"].ToString());
                tvn.ExpandNodeAltText = "Show Notes";
                tvn.CollapseNodeAltText = "Hide Notes";

                bool hasChildren = false;
                int noteCount = 0;

                // add non-burndown notes first
                for (int b = 0; b < 2; b++)
                {
                    foreach (DataRow row2 in dtAor.Rows)
                    {
                        if (row["AORID"].ToString() == row2["AORID"].ToString())
                        {
                            int noteTypeID = row2["AORNoteTypeID"] != DBNull.Value ? (int)row2["AORNoteTypeID"] : 0;
                            int noteID = (int)row2["AORMeetingNotesID"];
                            string extData = row2["ExtData"] != DBNull.Value ? (string)row2["ExtData"] : "";

                            if (((b == 0 && noteTypeID != (int)NoteTypeEnum.BurndownOverview) || (b == 1 && noteTypeID == (int)NoteTypeEnum.BurndownOverview)) && !notesAdded.Contains(noteID))
                            {
                                string closed = row2["STATUS"].ToString().ToLower() == "closed" ? "*" : "";

                                TreeViewNode childNode = tvn.AddChildNode("(" + GetNoteAbbr(noteTypeID) + ") " + row2["AORMeetingNotesID"].ToString() + closed + " - " + row2["Title"].ToString(), row2["AORMeetingNotesID"].ToString());
                                childNode.AddAttribute("aorid", row["AORID"].ToString());
                                childNode.AddAttribute("aorname", row["AORName"].ToString());
                                if (extData.IndexOf("<agendaitemkey>") != -1)
                                {
                                    int idx = extData.IndexOf("<agendaitemkey>");
                                    int len = "<agendaitemkey>".Length;
                                    int idx2 = extData.IndexOf("</agendaitemkey>");
                                    childNode.AddAttribute("agendaitemkey", extData.Substring(idx + len, idx2 - (idx + len)));
                                }
                                else
                                {
                                    childNode.AddAttribute("agendaitemkey", "");
                                }

                                if (b == 1)
                                {
                                    childNode.Style = "font-style:italic;";
                                }

                                if (row2["Included"].ToString() == "0")
                                {
                                    if (childNode.Style == null)
                                    {
                                        childNode.Style = "color:" + removedNoteColor + ";";
                                    }
                                    else
                                    {
                                        childNode.Style += "color:" + removedNoteColor + ";";
                                    }
                                    childNode.AddAttribute("defaultcolor", removedNoteColor);
                                }

                                notesAdded.Add(noteID);
                                noteCount++;
                                hasChildren = true;
                            }
                        }
                    }
                }

                if (hasChildren)
                {
                    tvn.Text += " (" + noteCount + ")";
                    tvNoteByAOR.AddTreeViewNode(tvn);
                }
            }

            curParentNode = row["AORID"].ToString();
        }

        // notes by type
        dtAor.DefaultView.Sort = "NoteTypeSort, AORMeetingNotesID";
        dtAor = dtAor.DefaultView.ToTable();

        tvNoteByType.LastClickedNodeBackgroundColor = "#d7e8fc";
        tvNoteByType.ClickCallbackFunction = "noteNodeClicked";

        curParentNode = "";

        foreach (DataRow row in dtAor.Rows)
        {
            if (curParentNode != row["AORNoteTypeID"].ToString())
            {
                TreeViewNode tvn = new TreeViewNode(row["AORNoteTypeName"].ToString(), row["AORNoteTypeID"].ToString());
                tvn.ExpandNodeAltText = "Show " + row["AORNoteTypeName"].ToString() + " Notes";
                tvn.CollapseNodeAltText = "Hide " + row["AORNoteTypeName"].ToString() + " Notes";

                bool hasChildren = false;
                int noteCount = 0;

                foreach (DataRow row2 in dtAor.Rows)
                {
                    if (row["AORNoteTypeID"].ToString() == row2["AORNoteTypeID"].ToString())
                    {
                        int noteTypeID = row2["AORNoteTypeID"] != DBNull.Value ? (int)row2["AORNoteTypeID"] : 0;
                        int noteID = (int)row2["AORMeetingNotesID"];
                        string closed = row2["STATUS"].ToString().ToLower() == "closed" ? "*" : "";
                        string extData = row2["ExtData"] != DBNull.Value ? (string)row2["ExtData"] : "";

                        TreeViewNode childNode = tvn.AddChildNode(row2["AORMeetingNotesID"].ToString() + closed + " - " + row2["Title"].ToString(), row2["AORMeetingNotesID"].ToString());
                        childNode.AddAttribute("aorid", row["AORID"].ToString());
                        childNode.AddAttribute("aorname", row["AORName"].ToString());
                        if (extData.IndexOf("<agendaitemkey>") != -1)
                        {
                            int idx = extData.IndexOf("<agendaitemkey>");
                            int len = "<agendaitemkey>".Length;
                            int idx2 = extData.IndexOf("</agendaitemkey>");
                            childNode.AddAttribute("agendaitemkey", extData.Substring(idx + len, idx2 - (idx + len)));
                        }
                        else
                        {
                            childNode.AddAttribute("agendaitemkey", "");
                        }

                        if (row2["Included"].ToString() == "0")
                        {
                            if (childNode.Style == null)
                            {
                                childNode.Style = "color:" + removedNoteColor + ";";
                            }
                            else
                            {
                                childNode.Style += "color:" + removedNoteColor + ";";
                            }
                            childNode.AddAttribute("defaultcolor", removedNoteColor);
                        }

                        notesAdded.Add(noteID);
                        noteCount++;
                        hasChildren = true;
                    }
                }

                if (hasChildren)
                {
                    tvn.Text += " (" + noteCount + ")";
                    tvNoteByType.AddTreeViewNode(tvn);
                }
            }

            curParentNode = row["AORNoteTypeID"].ToString();
        }

        tvNoteByAOR.ConfigureTreeView();
        tvNoteByType.ConfigureTreeView();
    }

    private static string GetNoteAbbr(int noteTypeID)
    {
        switch (noteTypeID)
        {
            case (int)NoteTypeEnum.BurndownOverview:
                return "B";
            case (int)NoteTypeEnum.Notes:
                return "N";
            case (int)NoteTypeEnum.QuestionsDiscussionPoints:
                return "Q";
            case (int)NoteTypeEnum.StoppingConditions:
                return "S";
            default:
                return "";
        }
    }

    [WebMethod()]
    public static string RefreshTreeView(string aorTVClientID, string typeTVClientID, int AORMeetingID, int AORMeetingInstanceID, bool showRemoved)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "false" }, { "error", "" }, { "aorhtml", "" }, { "typehtml", "" } };

        Controls_SimpleTreeViewControl aorTVCtrl = new Controls_SimpleTreeViewControl();
        aorTVCtrl.ClientID = aorTVClientID;
        Controls_SimpleTreeViewControl typeTVCtrl = new Controls_SimpleTreeViewControl();
        typeTVCtrl.ClientID = typeTVClientID;

        PopulateTreeViewControl(aorTVCtrl, typeTVCtrl, AORMeetingID, AORMeetingInstanceID, showRemoved);

        result["aorhtml"] = aorTVCtrl.TreeView.RenderTreeView();
        result["typehtml"] = typeTVCtrl.TreeView.RenderTreeView();

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string AcceptMeetingInstance(int AORMeetingID, int AORMeetingInstanceID, bool fromNoteHistoryDialog)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "false" }, { "error", "" }, { "AORMeetingInstanceID", AORMeetingInstanceID.ToString() }, { "fromnotehistorydialog" , fromNoteHistoryDialog.ToString() } };

        bool mtgChangedSinceLastMinutes = AOR.AORMeetingInstance_HasMeetingChangedSinceLastMeetingMinutes(AORMeetingInstanceID);

        string error = AOR.AORMeetingInstanceAcceptMeeting_Update(AORMeetingInstanceID, true);

        if (mtgChangedSinceLastMinutes)
        {
            int newAttachmentID = 0;

            AOR.ExportMeetingInstanceData("pdf", "AORs Included,Items,Agenda/Objectives,Burndown Overview,Notes,Questions/Discussion Points,Stopping Conditions", AORMeetingID, AORMeetingInstanceID, null, null, true, (int)WTS.Enums.AttachmentTypeEnum.MeetingMinutes, out newAttachmentID, false, System.Web.HttpContext.Current.Response, System.Web.HttpContext.Current.Server);
            result["newattachmentid"] = newAttachmentID.ToString();
        }

        if (string.IsNullOrWhiteSpace(error))
        {
            result["success"] = "true";
        }
        else
        {
            result["error"] = "An error has occurred. The meeting could not be updated. " + error;
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetMeetingMetrics(int AORMeetingInstanceID)
    {
        return AOR.GetMeetingMetricsResult(0, AORMeetingInstanceID);
    }

    [WebMethod()]
    public static string ValidateInstanceDate(int AORMeetingID, int AORMeetingInstanceID, string date)
    {
        var result = WTSPage.CreateDefaultResult();

        DateTime dt = DateTime.MinValue;

        if (DateTime.TryParse(date, out dt))
        {
            int conflict = AOR.AORMeetingInstance_CheckForDateConflict(AORMeetingID, AORMeetingInstanceID, dt);

            if (conflict > 0)
            {
                result["AORMeetingID"] = AORMeetingID.ToString();
                result["ConflictingAORMeetingInstanceID"] = conflict.ToString();
            }
            else
            {
                result["success"] = "true";
            }
        }
        else
        {
            result["error"] = "Invalid date.";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string LoadExistingMeetings()
    {
        DataTable dt = AOR.AORMeetingList_Get();

        return WTSPage.SerializeResult(dt);
    }

    [WebMethod()]
    public static string RegenerateMeetingMinutes(int AORMeetingID, int AORMeetingInstanceID)
    {
        var result = WTSPage.CreateDefaultResult();

        int newAttachmentID = 0;

        AOR.ExportMeetingInstanceData("pdf", "AORs Included,Items,Agenda/Objectives,Burndown Overview,Notes,Questions/Discussion Points,Stopping Conditions", AORMeetingID, AORMeetingInstanceID, null, null, true, (int)WTS.Enums.AttachmentTypeEnum.MeetingMinutes, out newAttachmentID, false, System.Web.HttpContext.Current.Response, System.Web.HttpContext.Current.Server);
        result["newattachmentid"] = newAttachmentID.ToString();
        result["success"] = "true";

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string ResourceUpdated(int AORMeetingID, int AORMeetingInstanceID, int WTS_RESOURCEID, bool attended, string reasonForAttending)
    {
        var result = WTSPage.CreateDefaultResult();

        if (AOR.AORMeetingInstanceResource_Update(AORMeetingID, AORMeetingInstanceID, WTS_RESOURCEID, attended, reasonForAttending))
        {
            result["success"] = "true";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string LockMeeting(int AORMeetingInstanceID)
    {
        var result = WTSPage.CreateDefaultResult();

        if (AOR.AORMeetingInstance_ToggleMeetingLock(AORMeetingInstanceID, true, "LOCK MEETING", WTSPage.GetLoggedInUserID()))
        {
            result["success"] = "true";
        }

        return WTSPage.SerializeResult(result);

        
    }

    #endregion
}