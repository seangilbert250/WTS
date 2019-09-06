using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

using Newtonsoft.Json;

using WTS.Events;
using WTS.Util;


public partial class AOR_Meeting_Grid : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool CanEditAORMeeting = false;
    protected bool CanViewAORMeeting = false;
    protected bool CanEditAORMeetingInstance = false;
    protected bool CanViewAORMeetingInstance = false;
    protected int AORID = 0;
    protected int AORReleaseID = 0;
    protected string View = string.Empty;
    protected int CurrentLevel = 1;
    protected string Filter = string.Empty;
    protected bool IsConfigured = false;
    private XmlDocument Levels = new XmlDocument();
    protected DataColumnCollection DCC;
    protected int LevelCount = 0;
    protected int GridPageIndex = 0;
    protected string AORMIMeetingID = string.Empty;
    protected List<string> Meetings = new List<string>();
    protected string Download = null;
    protected string DownloadSettings = null;
    protected int AORMeetingID = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        if (Download != null)
        {
            ExportData(Download);
        }

        ReadSession();
        InitializeEvents();
        LoadRelatedItemsMenu();
        LoadControls();

        this.CanEditAOR = (UserManagement.UserCanEdit(WTSModuleOption.AOR) && AOR.AORReleaseCurrent(AORID: this.AORID, AORReleaseID: this.AORReleaseID));
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);
        this.CanEditAORMeeting = UserManagement.UserCanEdit(WTSModuleOption.Meeting);
        this.CanViewAORMeeting = this.CanEditAORMeeting || UserManagement.UserCanView(WTSModuleOption.Meeting);
        this.CanEditAORMeetingInstance = UserManagement.UserCanEdit(WTSModuleOption.Meeting);
        this.CanViewAORMeetingInstance = this.CanEditAORMeetingInstance || UserManagement.UserCanView(WTSModuleOption.Meeting);

        this.ddlView.SelectedValue = this.View;

        DataTable dt = LoadData();
        if (dt != null) this.DCC = dt.Columns;

        grdData.DataSource = dt;

        if (!Page.IsPostBack && this.GridPageIndex > 0 && this.GridPageIndex < ((decimal)dt.Rows.Count / (decimal)25)) grdData.PageIndex = this.GridPageIndex;

        grdData.DataBind();

        FillMeetingsList(dt);

        if (CurrentLevel > 1)
        {
            var headerDiv = (System.Web.UI.HtmlControls.HtmlControl)Page.Master.FindControl("pageContentHeader");
            headerDiv.Style.Add("display", "none"); // setting this server-side to avoid flickering (we hide it after document.read)
        }
    }

    private void LoadControls()
    {
        DataTable dtNoteTypeList = AOR.AORNoteTypeList_Get();

        cblDownloadPDFSettings.DataSource = dtNoteTypeList;
        cblDownloadPDFSettings.DataValueField = "Note Type";
        cblDownloadPDFSettings.DataBind();

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

    }

    private XmlDocument ReadSession(string vw = null, bool setSessionVariables = true)
    {
        //temp
        XmlDocument nDoc = new XmlDocument();
        string test = string.Empty;
        test = "<crosswalkparameters><level>";

        if ((vw != null && vw == "Week") || (vw == null && (this.View == "" || this.View == "Week")))
        {
            test += "<breakout><column>MEETING WEEK START</column><sort>Descending</sort></breakout>";
            test += "<breakout><column>MEETING WEEK END</column><sort>Descending</sort></breakout>";
            test += "<breakout><column># OF MEETINGS</column><sort>Ascending</sort></breakout>";
            test += "<breakout><column># OF MEETING INSTANCES</column><sort>Ascending</sort></breakout>";
            test += "<breakout><column># OF AORS INVOLVED</column><sort>Ascending</sort></breakout>";
            test += "<breakout><column># OF RESOURCES INVOLVED</column><sort>Ascending</sort></breakout>";
            test += "</level><level>";
        }
        else if ((vw != null && vw == "AOR") || (vw == null && this.View == "AOR"))
        {
            test += "<breakout><column>AOR #</column><sort>Ascending</sort></breakout>";
            test += "<breakout><column>AOR NAME</column><sort>Ascending</sort></breakout>";
            test += "<breakout><column># OF MEETINGS</column><sort>Ascending</sort></breakout>";
            test += "<breakout><column># OF MEETING INSTANCES</column><sort>Ascending</sort></breakout>";
            test += "<breakout><column># OF RESOURCES INVOLVED</column><sort>Ascending</sort></breakout>";
            test += "</level><level>";
        }
        
        //test += "<breakout><column>RESOURCE</column><sort>Ascending</sort></breakout>";
        //test += "<breakout><column># OF MEETINGS</column><sort>Ascending</sort></breakout>";
        //test += "<breakout><column># OF MEETING INSTANCES</column><sort>Ascending</sort></breakout>";
        //test += "<breakout><column># OF AORS INVOLVED</column><sort>Ascending</sort></breakout>";
        //test += "</level><level>";
        test += "<breakout><column>MEETING NAME</column><sort>Ascending</sort></breakout>";        
        test += "<breakout><column>FREQUENCY</column><sort>Ascending</sort></breakout>";
        //test += "<breakout><column>PRIVATE</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column>LAST MEETING</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column>NEXT MEETING</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column>MIN # OF ATTENDEES</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column>MAX # OF ATTENDEES</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column>AVG # OF ATTENDEES</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column># OF MEETING INSTANCES</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column># OF AORS INVOLVED</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column># OF RESOURCES INVOLVED</column><sort>Ascending</sort></breakout>";
        test += "</level><level>";
        test += "<breakout><column>INSTANCE DATE</column><sort>Descending</sort></breakout>";        
        test += "<breakout><column>MEETING INSTANCE NAME</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column>MEETING STATUS</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column>ACTUAL LENGTH</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column># OF AORS INVOLVED</column><sort>Ascending</sort></breakout>";
        test += "<breakout><column># OF RESOURCES INVOLVED</column><sort>Ascending</sort></breakout>";
        test += "</level></crosswalkparameters>";
        nDoc.InnerXml = test;


        if (!setSessionVariables)
        {
            return nDoc;
        }
        
        Session["AORMeetingLevels"] = nDoc;
        //

        if (Session["AORMeetingLevels"] != null)
        {
            this.Levels = (XmlDocument)Session["AORMeetingLevels"];
        }

        this.LevelCount = this.Levels.SelectNodes("crosswalkparameters/level").Count;

        if (this.LevelCount >= 1) this.IsConfigured = true;

        return null;
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

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.AORID);
        }

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.AORReleaseID);
        }

        if (Request.QueryString["View"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["View"]))
        {
            this.View = Request.QueryString["View"];
        }

        if (Request.QueryString["CurrentLevel"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"]))
        {
            int.TryParse(Request.QueryString["CurrentLevel"], out this.CurrentLevel);
        }

        if (Request.QueryString["Filter"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Filter"]))
        {
            this.Filter = Uri.UnescapeDataString(Request.QueryString["Filter"]);
        }

        if (Request.QueryString["GridPageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndex"]))
        {
            int.TryParse(Request.QueryString["GridPageIndex"], out this.GridPageIndex);
        }

        if (Request.QueryString["Download"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Download"]))
        {
            this.Download = Request.QueryString["Download"];
        }

        if (Request.QueryString["DownloadSettings"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["DownloadSettings"]))
        {
            this.DownloadSettings = Request.QueryString["DownloadSettings"];
        }

        if (CurrentLevel >= 3 && !string.IsNullOrWhiteSpace(Filter))
        {
            string[] filterTokens = Filter.Split('|');
            foreach (var filterToken in filterTokens) {
                string[] tokens = filterToken.Split('=');
                if (tokens[0] == "AORMeeting_ID")
                {
                    AORMeetingID = Int32.Parse(tokens[1]);
                    break;
                }
            }
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
    }

    private void InitializeEvents()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;
    }
    #endregion

    #region Data
    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

        if (this.IsConfigured)
        {
            if (IsPostBack && this.CurrentLevel == 1 && Session["dtAORMeetingLevel" + this.CurrentLevel] != null)
            {
                dt = (DataTable)Session["dtAORMeetingLevel" + this.CurrentLevel];
            }
            else
            {
                XmlDocument docLevel = new XmlDocument();
                XmlElement rootLevel = (XmlElement)docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
                XmlNode nodeLevel = this.Levels.SelectNodes("crosswalkparameters/level")[this.CurrentLevel - 1];
                XmlNode nodeImport = docLevel.ImportNode(nodeLevel, true);
                rootLevel.AppendChild(nodeImport);

                XmlDocument docFilters = new XmlDocument();
                XmlElement rootFilters = (XmlElement)docFilters.AppendChild(docFilters.CreateElement("filters"));

                if (this.Filter != string.Empty)
                {
                    string[] arrFilter = this.Filter.Split('|');

                    for (int j = 0; j < arrFilter.Length; j++)
                    {
                        XmlElement filter = docFilters.CreateElement("filter");
                        XmlElement field = docFilters.CreateElement("field");
                        XmlElement value = docFilters.CreateElement("id");
                        string[] arrValues = arrFilter[j].Split('=');

                        field.InnerText = arrValues[0].ToString();
                        value.InnerText = (arrValues[1].ToString().Trim() == "" && field.InnerText.ToUpper() != "MEETINGWEEKSTART_ID" && field.InnerText.ToUpper() != "MEETINGWEEKEND_ID" && field.InnerText.ToUpper() != "LASTMEETING_ID" && field.InnerText.ToUpper() != "NEXTMEETING_ID" ? "0" : arrValues[1].ToString().Trim());

                        filter.AppendChild(field);
                        filter.AppendChild(value);
                        rootFilters.AppendChild(filter);
                    }
                }

                dt = AOR.AOR_Meeting_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, AORID: this.AORID, AORReleaseID: this.AORReleaseID);

                if (dt != null) if (dt.Columns.Contains("Meeting Week Start") && dt.Columns.Contains("Meeting Week End") && dt.Columns.Contains("Week")) dt.Columns["Week"].SetOrdinal(dt.Columns["Meeting Week End"].Ordinal);

                Session["dtAORMeetingLevel" + this.CurrentLevel] = dt;
            }
        }

        return dt;
    }

    private void FillMeetingsList(DataTable dt)
    {
        if (dt != null && dt.Rows.Count > 0)
        {
            DataTable mtgDt = dt;

            if (!dt.Columns.Contains("AORMeeting_ID"))
            {
                XmlDocument nDoc = ReadSession("Meeting", false);

                XmlDocument docLevel = new XmlDocument();
                XmlElement rootLevel = (XmlElement)docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
                XmlNode nodeLevel = nDoc.SelectNodes("crosswalkparameters/level")[0];
                XmlNode nodeImport = docLevel.ImportNode(nodeLevel, true);
                rootLevel.AppendChild(nodeImport);

                XmlDocument docFilters = new XmlDocument();
                XmlElement rootFilters = (XmlElement)docFilters.AppendChild(docFilters.CreateElement("filters"));

                mtgDt = AOR.AOR_Meeting_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, AORID: this.AORID, AORReleaseID: this.AORReleaseID);
            }

            Dictionary<string, string> meetingsDict = new Dictionary<string, string>();
            List<string> meetingNames = new List<string>();
            foreach (DataRow row in mtgDt.Rows)
            {
                string key = (string)row["Meeting Name"] + "_" + (int)row["AORMeeting_ID"];
                meetingNames.Add(key);

                if (!meetingsDict.ContainsKey(key)) {
                    meetingsDict.Add(key, System.Web.HttpUtility.UrlEncode("(" + (int)row["AORMeeting_ID"] + ") " + (string)row["Meeting Name"]) + "=" + (int)row["AORMeeting_ID"]);
                }
            }
            meetingNames.Sort();

            foreach (string name in meetingNames)
            {
                Meetings.Add(meetingsDict[name]);
            }
            
            hdnMeetings.Value = string.Join(";", Meetings);
        }
    }

    private void ExportData(string type, string emailRecipients = null)
    {
        if (!string.IsNullOrWhiteSpace(DownloadSettings))
        {
            string[] settings = DownloadSettings.Split(',');

            int newAttachmentID = 0;

            AOR.ExportMeetingInstanceData(type, this.DownloadSettings, Convert.ToInt32(settings[0]), Convert.ToInt32(settings[1]), null, null, false, 0, out newAttachmentID, false, Response, Server);            
        }
    }   

    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);
        if (DCC.Contains("X")) row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage(true));
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatRowDisplay(ref row);

        if (DCC.Contains("X")) row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage(false));
        if (DCC.Contains("Meeting Week Start") && DCC.Contains("Meeting Week End") && DCC.Contains("Week"))
        {
            string nWeek = string.Empty;
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Meeting Week Start")].Text, out nDate))
            {
                nWeek = String.Format("{0:M/d/yyyy}", nDate);
            }

            nWeek += " - ";

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Meeting Week End")].Text, out nDate))
            {
                nWeek += String.Format("{0:M/d/yyyy}", nDate);
            }

            row.Cells[DCC.IndexOf("Week")].Text = nWeek == " - " ? "" : nWeek;
            row.Cells[DCC.IndexOf("Meeting Week Start")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Meeting Week End")].Style["display"] = "none";
        }

        if (DCC.Contains("Meeting #") && DCC.Contains("Meeting Name"))
        {
            row.Attributes.Add("aormeeting_id", row.Cells[DCC.IndexOf("Meeting #")].Text);

            if (this.CanEditAORMeeting)
            {
                row.Cells[DCC.IndexOf("Meeting Name")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Meeting Name")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("Meeting #")].Text, "AOR Meeting Name", row.Cells[DCC.IndexOf("Meeting Name")].Text, false));

                if (DCC.Contains("Description"))
                {
                    row.Cells[DCC.IndexOf("Description")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Description")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("Meeting #")].Text, "Description", row.Cells[DCC.IndexOf("Description")].Text, false));
                }

                if (DCC.Contains("Sort"))
                {
                    row.Cells[DCC.IndexOf("Sort")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Sort")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("Meeting #")].Text, "Sort", row.Cells[DCC.IndexOf("Sort")].Text, true));
                }
            }
        }

        if (DCC.Contains("Last Meeting"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Last Meeting")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Last Meeting")].Text = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
            }
        }

        if (DCC.Contains("Next Meeting"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Next Meeting")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Next Meeting")].Text = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
            }
        }

        if (DCC.Contains("AORMeetingInstanceMeeting_ID") && DCC.Contains("Meeting Instance #") && DCC.Contains("Meeting Instance Name"))
        {
            if (row.RowIndex == 0) this.AORMIMeetingID = row.Cells[DCC.IndexOf("AORMeetingInstanceMeeting_ID")].Text;

            row.Attributes.Add("aormeetinginstance_id", row.Cells[DCC.IndexOf("Meeting Instance #")].Text);

            if (row.Cells[DCC.IndexOf("AORMeetingInstanceMeeting_ID")].Text != "&nbsp;" && this.CanViewAORMeetingInstance) row.Cells[DCC.IndexOf("Meeting Instance #")].Controls.Add(CreateLink("Meeting Instance", row.Cells[DCC.IndexOf("AORMeetingInstanceMeeting_ID")].Text, row.Cells[DCC.IndexOf("Meeting Instance #")].Text));
        }

        if (DCC.Contains("Instance Date"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Instance Date")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Instance Date")].Text = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
            }
        }

        if (this.CanViewAOR && DCC.Contains("AOR #") && row.Cells[DCC.IndexOf("AOR #")].Text != "&nbsp;")
        {
            row.Cells[DCC.IndexOf("AOR #")].Controls.Add(CreateLink("AOR", row.Cells[DCC.IndexOf("AOR #")].Text, ""));
        }

        if (DCC.Contains("Meeting Status"))
        {
            TableCell cell = row.Cells[DCC.IndexOf("Meeting Status")];
            cell.Style.Add("white-space", "nowrap");

            string statuses = cell.Text; // locked/ended/accepted

            char locked = statuses[0];
            char ended = statuses[1];
            char accepted = statuses[2];

            cell.Controls.Clear();

            if (locked == '1')
            {
                cell.Controls.Add(CreateImage(false, "locked"));
            }

            if (ended == '1')
            {
                cell.Controls.Add(CreateImage(false, "ended"));
            }

            cell.Controls.Add(CreateImage(false, accepted == '1' ? "accepted" : "notaccepted"));
        }
    }

    private void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdData.PageIndex = e.NewPageIndex;
    }

    private void FormatHeaderRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
        }

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = "15px";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("Meeting Week Start") && DCC.Contains("Meeting Week End") && DCC.Contains("Week"))
        {
            row.Cells[DCC.IndexOf("Meeting Week Start")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Meeting Week End")].Style["display"] = "none";
        }

        if (DCC.Contains("Week")) {
			row.Cells[DCC.IndexOf("Week")].Style["text-align"] = "left";
			row.Cells[DCC.IndexOf("Week")].Style["width"] = "140px";
		}

        if (DCC.Contains("# of Meetings")) row.Cells[DCC.IndexOf("# of Meetings")].Style["width"] = "65px";
        if (DCC.Contains("# of Meeting Instances")) row.Cells[DCC.IndexOf("# of Meeting Instances")].Style["width"] = "85px";
        if (DCC.Contains("# of AORs Involved")) row.Cells[DCC.IndexOf("# of AORs Involved")].Style["width"] = "70px";
        if (DCC.Contains("# of Resources Involved")) row.Cells[DCC.IndexOf("# of Resources Involved")].Style["width"] = "95px";
        if (DCC.Contains("Resource")) row.Cells[DCC.IndexOf("Resource")].Style["width"] = "150px";
        if (DCC.Contains("Meeting #")) row.Cells[DCC.IndexOf("Meeting #")].Style["width"] = "70px";
        if (DCC.Contains("Meeting Name")) row.Cells[DCC.IndexOf("Meeting Name")].Style["width"] = "250px";
        if (DCC.Contains("Description")) row.Cells[DCC.IndexOf("Description")].Style["width"] = "250px";
        if (DCC.Contains("Sort")) row.Cells[DCC.IndexOf("Sort")].Style["width"] = "45px";
        if (DCC.Contains("Frequency")) row.Cells[DCC.IndexOf("Frequency")].Style["width"] = "80px";
        if (DCC.Contains("Last Meeting")) row.Cells[DCC.IndexOf("Last Meeting")].Style["width"] = "70px";
        if (DCC.Contains("Next Meeting")) row.Cells[DCC.IndexOf("Next Meeting")].Style["width"] = "70px";
        if (DCC.Contains("Min # of Attendees")) row.Cells[DCC.IndexOf("Min # of Attendees")].Style["width"] = "75px";
        if (DCC.Contains("Max # of Attendees")) row.Cells[DCC.IndexOf("Max # of Attendees")].Style["width"] = "75px";
        if (DCC.Contains("Avg # of Attendees")) row.Cells[DCC.IndexOf("Avg # of Attendees")].Style["width"] = "75px";
        if (DCC.Contains("Meeting Instance #")) row.Cells[DCC.IndexOf("Meeting Instance #")].Style["width"] = "75px";
        if (DCC.Contains("Meeting Instance Name")) row.Cells[DCC.IndexOf("Meeting Instance Name")].Style["width"] = "250px";
        if (DCC.Contains("Instance Date")) row.Cells[DCC.IndexOf("Instance Date")].Style["width"] = "70px";
        if (DCC.Contains("Actual Length")) row.Cells[DCC.IndexOf("Actual Length")].Style["width"] = "55px";
        if (DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "45px";
        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["width"] = "500px";
        if (DCC.Contains("Meeting Status"))
        {
            row.Cells[DCC.IndexOf("Meeting Status")].Style["width"] = "50px";
            row.Cells[DCC.IndexOf("Meeting Status")].Text = "Status";
        }

        if (DCC.Contains("Z")) row.Cells[DCC.IndexOf("Z")].Text = "";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Style["width"] = "15px";
            row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("Frequency")) row.Cells[DCC.IndexOf("Frequency")].Style["text-align"] = "center";
        if (DCC.Contains("Private")) row.Cells[DCC.IndexOf("Private")].Style["text-align"] = "center";
        if (DCC.Contains("Last Meeting")) row.Cells[DCC.IndexOf("Last Meeting")].Style["text-align"] = "center";
        if (DCC.Contains("Next Meeting")) row.Cells[DCC.IndexOf("Next Meeting")].Style["text-align"] = "center";
        if (DCC.Contains("Instance Date")) row.Cells[DCC.IndexOf("Instance Date")].Style["text-align"] = "center";
    }

    private Image CreateImage(bool isHeader, string type = null)
    {
        Image img = new Image();

        if (type == null)
        {
            if (isHeader)
            {
                if (CurrentLevel != LevelCount)
                {
                    img.Attributes["src"] = "Images/Icons/add_blue.png";
                    img.Attributes["title"] = "Expand";
                    img.Attributes["alt"] = "Expand";
                    img.Attributes["onclick"] = "displayAllRows(this);";
                }
            }
            else
            {
                if (this.CurrentLevel == this.LevelCount)
                {
                    //img.Attributes["src"] = "Images/Icons/cog.png";
                    //img.Attributes["title"] = "Grid Settings";
                    //img.Attributes["alt"] = "Grid Settings";
                    //img.Attributes["onclick"] = "openSettings();";
                }
                else
                {
                    img.Attributes["src"] = "Images/Icons/add_blue.png";
                    img.Attributes["title"] = "Expand";
                    img.Attributes["alt"] = "Expand";
                    img.Attributes["onclick"] = "displayNextRow(this);";
                }
            }

            img.Attributes["height"] = "12";
            img.Attributes["width"] = "12";
            img.Style["cursor"] = "pointer";
        }
        else
        {
            switch (type) {
                case "accepted":
                    img.Attributes["src"] = "Images/Icons/check.png";
                    img.Attributes["title"] = "Meeting Accepted";
                    img.Attributes["alt"] = "Meeting Accepted";
                    break;
                case "notaccepted":
                    img.Attributes["src"] = "Images/Icons/check_gray.png";
                    img.Attributes["title"] = "Meeting Not Accepted";
                    img.Attributes["alt"] = "Meeting Not Accepted";
                    img.Style.Add("opacity", ".8");
                    break;
                case "locked":
                    img.Attributes["src"] = "Images/Icons/lock.png";
                    img.Attributes["title"] = "Meeting Locked";
                    img.Attributes["alt"] = "Meeting Locked";
                    break;
                case "ended":
                    img.Attributes["src"] = "Images/Icons/small-clock.png";
                    img.Attributes["title"] = "Meeting Ended";
                    img.Attributes["alt"] = "Meeting Ended";
                    break;
                default:
                    img.Attributes["src"] = "";
                    break;
            }

            img.Attributes["height"] = "12";
            img.Attributes["width"] = "12";
            img.Style.Add("padding", "2px");
        }

        return img;
    }

    private LinkButton CreateLink(string type, string id1, string id2)
    {
        LinkButton lb = new LinkButton();

        switch (type)
        {
            case "Meeting Instance":
                lb.Text = id2;
                lb.Attributes["onclick"] = string.Format("openAORMeetingInstance('{0}', '{1}'); return false;", id1, id2);
                break;
            case "AOR":
                lb.Text = id1;
                lb.Attributes["onclick"] = string.Format("openAOR('{0}'); return false;", id1);
                break;
        }

        return lb;
    }

    private TextBox CreateTextBox(string AORMeeting_ID, string type, string value, bool isNumber)
    {
        string txtValue = Server.HtmlDecode(value).Trim();
        TextBox txt = new TextBox();

        txt.Text = txtValue;
        txt.MaxLength = 50;
        txt.Width = new Unit(type == "Sort" ? 90 : 95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("aormeeting_id", AORMeeting_ID);
        txt.Attributes.Add("field", type);
        txt.Attributes.Add("original_value", txtValue);

        if (isNumber)
        {
            txt.MaxLength = 5;
            txt.Style["text-align"] = "center";
        }

        return txt;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string DeleteAORMeeting(string aormeeting)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int AORMeeting_ID = 0;
            int.TryParse(aormeeting, out AORMeeting_ID);

            deleted = AOR.AORMeeting_Delete(AORMeetingID: AORMeeting_ID);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            deleted = false;
            errorMsg = ex.Message;
        }

        result["deleted"] = deleted.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string DeleteAORMeetingInstance(string aorMeetingInstance)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int AORMeetingInstance_ID = 0;
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);

            deleted = AOR.AORMeetingInstance_Delete(AORMeetingInstanceID: AORMeetingInstance_ID);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            deleted = false;
            errorMsg = ex.Message;
        }

        result["deleted"] = deleted.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string SaveChanges(string changes)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            XmlDocument docChanges = (XmlDocument)JsonConvert.DeserializeXmlNode(changes, "changes");

            saved = AOR.AORMeeting_Update(Changes: docChanges);
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

    [WebMethod()]
    public static string GetMeetingNames()
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "false" }, { "meetings", "" }, { "error", "" } };

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetMeetingInstances(string AORMeetingID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "false" }, { "instances", "" }, { "error", "" }, { "AORMeetingID", AORMeetingID.ToString() } };

        int id = 0;

        if (Int32.TryParse(AORMeetingID, out id))
        {
            DataTable dt = AOR.AORMeetingInstanceList_Get(id);

            if (dt != null)
            {
                StringBuilder str = new StringBuilder();

                foreach (DataRow row in dt.Rows)
                {
                    int mid = (int)row["Meeting Instance #"];
                    string mn = (string)row["Meeting Instance Name"];
                    DateTime idt = (DateTime)row["Instance Date"];

                    str.Append(System.Web.HttpUtility.UrlEncode(mn)).Append("=").Append(mid).Append("=").Append(idt != DateTime.MinValue ? idt.ToString("MM/dd/yyyy") : "").Append(";");
                }

                result["instances"] = str.ToString();
            }

            result["success"] = "true";
        }
        else
        {
            result["error"] = "Cannot parse meeting ID.";
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetMeetingInstanceCounts(string AORMeetingID, string AORMeetingInstanceID, int countIdx)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "false" }, { "counts", "" }, { "error", "" }, { "AORMeetingID", AORMeetingID }, { "AORMeetingInstanceID", AORMeetingInstanceID }, { "countidx", countIdx.ToString() } };

        int mtgID = Convert.ToInt32(AORMeetingID);
        int insID = Convert.ToInt32(AORMeetingInstanceID);

        DataTable dt = null;

        if (countIdx == 1) // aor
        {
            dt = AOR.AORMeetingInstanceAORList_Get(AORMeetingID: mtgID, AORMeetingInstanceID: insID, ShowRemoved: false);
            result["counts"] = dt.Rows.Count.ToString();
        }
        else if (countIdx == 2) // notes
        {
            dt = AOR.AORMeetingInstanceNotesList_Get(AORMeetingID: mtgID, AORMeetingInstanceID: insID, AORNoteTypeID: 0, ShowRemoved: false);

            StringBuilder str = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                string AORNoteTypeName = (string)row["AORNoteTypeName"];
                int count = (int)row["NoteDetailCount"];

                str.Append(AORNoteTypeName + "=" + count + ";");
            }

            result["counts"] = str.ToString();
        }
        else if (countIdx == 3) // att
        {
            dt = AOR.AORMeetingInstanceAttachment_Get(0, 0, insID, 0, false);
            result["counts"] = dt.Rows.Count.ToString();
        }
        else if (countIdx == 4) // recipient list
        {
            dt = AOR.AORMeetingInstanceResourceList_Get(AORMeetingID: mtgID, AORMeetingInstanceID: insID, ShowRemoved: false);

            StringBuilder str = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                if (row["WTS_RESOURCE_TYPEID"] == DBNull.Value || (int)row["WTS_RESOURCE_TYPEID"] != (int)UserManagement.ResourceType.Not_People)
                {
                    string resource = (string)row["Resource"];
                    int WTS_RESOURCEID = (int)row["WTS_RESOURCEID"];
                    int emailDefault = (int)row["EmailDefault"];

                    str.Append(HttpUtility.UrlEncode(resource) + "=" + WTS_RESOURCEID + "=" + emailDefault + ";");
                }
            }

            result["counts"] = str.ToString();
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string EmailMinutes(string downloadSettings, string recipients)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "success", "false" }, { "error", "" } };

        try
        {
            string[] settings = downloadSettings.Split(',');

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

            if (AOR.ExportMeetingInstanceData("pdf", downloadSettings, Convert.ToInt32(settings[0]), Convert.ToInt32(settings[1]), recipients, customNote, false, 0, out newAttachmentID, false, System.Web.HttpContext.Current.Response, System.Web.HttpContext.Current.Server))
            {
                result["success"] = "true";
            }
        }
        catch (Exception ex)
        {
            result["error"] = ex.Message;
            LogUtility.LogException(ex);

        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None); ;
    }

    [WebMethod()]
    public static string LoadMeetingMetrics()
    {
        return AOR.GetMeetingMetricsResult(0, 0);
    }

    #endregion
}
