﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using WTS;
using WTS.Util;

using Newtonsoft.Json;

public partial class AOR_Meeting_Instance_Popup : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAORMeetingInstance = false;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool AllowSave = false;
    protected int AORMeetingID = 0;
    protected int AORMeetingInstanceID = 0;
    protected int AORMeetingNotesID_Parent = 0;
    protected string Type = string.Empty;
    protected bool Locked = true;
    protected string[] QFSystem = { };
    protected string[] QFRelease = { };
    protected string QFName = "";
    protected int InstanceFilterID = 0;
    protected int NoteTypeFilterID = 0;
    private DataColumnCollection DCC;
    protected string CurrentSystemID;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditAORMeetingInstance = UserManagement.UserCanEdit(WTSModuleOption.Meeting);
        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);
        this.Locked = AOR.AORMeetingInstanceLocked(this.AORMeetingInstanceID);

        if (this.Locked) this.CanEditAORMeetingInstance = false;

        if (this.Type == "AOR")
        {
            grdData.AlternatingRowColor = System.Drawing.Color.White;
            LoadQF();
            PopulateQuickFilters();
        }

        DataTable dt = new DataTable();

        dt = LoadData();
        
        switch (this.Type)
        {
            case "AOR":
            case "Resource":
            case "Historical Notes":
                if (this.Type == "Historical Notes")
                {
                    grdData.AllowPaging = true;

                    if (!Page.IsPostBack)
                    {
                        DataTable dtMeetingInstance = AOR.AORMeetingInstanceList_Get(AORMeetingID: this.AORMeetingID, AORMeetingInstanceID: 0, InstanceFilterID: this.AORMeetingInstanceID);
                        DateTime nDate;
                        ListItem li;

                        foreach (DataRow dr in dtMeetingInstance.Rows)
                        {
                            nDate = new DateTime();

                            if (DateTime.TryParse(dr["Instance Date"].ToString(), out nDate))
                            {
                                dr["Instance Date"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);

                                li = new ListItem();

                                li.Value = dr["Meeting Instance #"].ToString();
                                li.Text = dr["Instance Date"].ToString();

                                ddlInstanceDateQF.Items.Add(li);
                            }
                        }

                        li = new ListItem();

                        li.Value = "0";
                        li.Text = "- All -";

                        ddlInstanceDateQF.Items.Insert(0, li);
                        ddlInstanceDateQF.SelectedValue = this.InstanceFilterID.ToString();

                        DataSet dsOptions = AOR.AOROptionsList_Get(AORID: 0, TaskID: 0, AORMeetingID: this.AORMeetingID, AORMeetingInstanceID: this.AORMeetingInstanceID);

                        if (dsOptions != null)
                        {
                            DataTable dtNoteType = dsOptions.Tables["Note Type"];

                            ddlNoteTypeQF.DataSource = dtNoteType;
                            ddlNoteTypeQF.DataValueField = "Value";
                            ddlNoteTypeQF.DataTextField = "Text";
                            ddlNoteTypeQF.DataBind();
                        }

                        li = new ListItem();

                        li.Value = "0";
                        li.Text = "- All -";

                        ddlNoteTypeQF.Items.Insert(0, li);
                        ddlNoteTypeQF.SelectedValue = this.NoteTypeFilterID.ToString();
                    }
                }

                if (dt != null) this.DCC = dt.Columns;

                grdData.DataSource = dt;
                grdData.DataBind();
                break;
            case "Note Type":
                cblNoteType.DataSource = dt;
                cblNoteType.DataValueField = "AORNoteTypeID";
                cblNoteType.DataTextField = "AORNoteTypeName";
                cblNoteType.DataBind();
                break;
            case "Add Note Objectives":
                ddlNoteType.Items.Add(new ListItem("Agenda/Objectives", "17"));

                break;
            case "Note Detail":
            case "Edit Note Detail":
            case "View Note Detail":
            case "Edit Note Objectives":
                DataTable dtNoteTypeList = AOR.AORNoteTypeList_Get();

                ddlNoteType.DataSource = dtNoteTypeList;
                ddlNoteType.DataValueField = "AORNoteType_ID";
                ddlNoteType.DataTextField = "Note Type";
                ddlNoteType.DataBind();

                ListItem liAOR = new ListItem();

                liAOR.Value = "0";
                liAOR.Text = "";

                ddlAORName.Items.Add(liAOR);

                foreach (DataRow dr in dt.Rows)
                {
                    liAOR = new ListItem();

                    liAOR.Value = dr["AORReleaseID"].ToString();

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

                    liAOR.Text = nText + dr["AORID"].ToString() + " - " + dr["AORName"].ToString();
                    liAOR.Attributes.Add("AORID", dr["AORID"].ToString());
                    liAOR.Attributes.Add("AORName", dr["AORName"].ToString());

                    ddlAORName.Items.Add(liAOR);
                }

                txtSubTaskID.Attributes.Add("taskid", "");
                txtSubTaskID.Attributes.Add("tasknumber", "");
                txtSubTaskID.Attributes.Add("subtaskid", "");

                if (this.Type == "Edit Note Detail" || this.Type == "View Note Detail" || this.Type == "Edit Note Objectives")
                {
                    DataTable dtNoteDetail = AOR.AORMeetingInstanceNotesDetail_Get(this.AORMeetingNotesID_Parent);

                    if (dtNoteDetail != null && dtNoteDetail.Rows.Count > 0)
                    {
                        DataRow dr = dtNoteDetail.Rows[0];

                        ddlNoteType.SelectedValue = dr["AORNoteTypeID"].ToString();
                        txtTitle.Text = dr["Title"].ToString();
                        txtNoteDetail.Text = dr["Notes"].ToString();

                        string taskTitle = dr["TaskTitle"] != DBNull.Value ? dr["TaskTitle"].ToString() : "";
                        if (taskTitle != "")
                        {
                            if (taskTitle.Length > 80) taskTitle = taskTitle.Substring(0, 80) + "...";
                            ltTaskTitle.Text = "(" + HttpUtility.HtmlEncode(taskTitle) + ")";
                        }
                        txtTaskID.Text = dr["WORKITEMID"] != DBNull.Value ? dr["WORKITEMID"].ToString() : "";

                        string subTaskTitle = dr["SubTaskTitle"] != DBNull.Value ? dr["SubTaskTitle"].ToString() : "";
                        if (subTaskTitle != "")
                        {
                            if (subTaskTitle.Length > 80) subTaskTitle = subTaskTitle.Substring(0, 80) + "...";
                            ltSubTaskTitle.Text = "(" + HttpUtility.HtmlEncode(subTaskTitle) + ")";
                        }
                        string subTaskNumber = dr["TASK_NUMBER"] != DBNull.Value ? dr["TASK_NUMBER"].ToString() : "";
                        string subTaskID = dr["WORKITEM_TASKID"] != DBNull.Value ? dr["WORKITEM_TASKID"].ToString() : "";
                        string extData = dr["ExtData"] != DBNull.Value ? dr["ExtData"].ToString() : "";

                        txtSubTaskID.Text = subTaskNumber;
                        txtSubTaskID.Attributes["taskid"] = txtTaskID.Text;
                        txtSubTaskID.Attributes["tasknumber"] = subTaskID.Length > 0 ? subTaskID + "." + subTaskNumber : "";
                        txtSubTaskID.Attributes["subtaskid"] = subTaskID.Length > 0 ? subTaskID : "";
                        hdnNoteExtData.Value = extData;

                        this.AllowSave = dr["STATUS"].ToString() != "Closed";

                        if (dr["AORReleaseID"].ToString() != "")
                        {
                            if (ddlAORName.Items.FindByValue(dr["AORReleaseID"].ToString()) != null)
                            {
                                ddlAORName.SelectedValue = dr["AORReleaseID"].ToString();
                            }
                            else
                            {
                                liAOR = new ListItem();

                                liAOR.Value = dr["AORReleaseID"].ToString();

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

                                liAOR.Text = nText + dr["AORID"].ToString() + " - " + dr["AORName"].ToString();

                                ddlAORName.Items.Insert(0, liAOR);
                            }
                        }
                    }
                }

                break;
        }
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

        if (Request.QueryString["AORMeetingID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingID"]))
        {
            int.TryParse(Request.QueryString["AORMeetingID"], out this.AORMeetingID);
        }

        if (Request.QueryString["AORMeetingInstanceID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingInstanceID"]))
        {
            int.TryParse(Request.QueryString["AORMeetingInstanceID"], out this.AORMeetingInstanceID);
        }

        if (Request.QueryString["AORMeetingNotesID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingNotesID"]))
        {
            int.TryParse(Request.QueryString["AORMeetingNotesID"], out this.AORMeetingNotesID_Parent);
        }

        if (Request.QueryString["Type"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Type"]))
        {
            this.Type = Request.QueryString["Type"];
        }

        if (Request.QueryString["SelectedSystems"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSystems"]))
        {
            this.QFSystem = Request.QueryString["SelectedSystems"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["SelectedReleases"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedReleases"]))
        {
            this.QFRelease = Request.QueryString["SelectedReleases"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["txtSearch"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["txtSearch"]))
        {
            this.QFName = Request.QueryString["txtSearch"].Trim();
            txtSearch.Text = Request.QueryString["txtSearch"].Trim(); // this just resets the search box back to default value
        }

        if (Request.QueryString["InstanceFilterID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["InstanceFilterID"]))
        {
            int.TryParse(Request.QueryString["InstanceFilterID"], out this.InstanceFilterID);
        }

        if (Request.QueryString["NoteTypeFilterID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NoteTypeFilterID"]))
        {
            int.TryParse(Request.QueryString["NoteTypeFilterID"], out this.NoteTypeFilterID);
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

        if (IsPostBack && Session["dtAORMeetingInstancePopup"] != null)
        {
            dt = (DataTable)Session["dtAORMeetingInstancePopup"];
        }
        else
        {
            List<string> listSystem = new List<string>();

            HtmlSelect ms_Item0 = (HtmlSelect)Page.Master.FindControl("ms_Item0");
            HtmlSelect ms_Item10 = (HtmlSelect)Page.Master.FindControl("ms_Item10");

            if (this.Type == "AOR" && ms_Item0 != null && ms_Item0.Items.Count > 0)
            {
                foreach (ListItem li in ms_Item0.Items)
                {
                    if (li.Selected) listSystem.Add(li.Value);
                }
            }

            List<string> listRelease = new List<string>();

            if (this.Type == "AOR" && ms_Item10 != null && ms_Item10.Items.Count > 0)
            {
                foreach (ListItem li in ms_Item10.Items)
                {
                    if (li.Selected) listRelease.Add(li.Value);
                }
            }

            dt = AOR.AORMeetingInstanceAddList_Get(AORMeetingID: this.AORMeetingID, AORMeetingInstanceID: this.AORMeetingInstanceID, Type: this.Type, QFSystem: String.Join(",", listSystem), QFRelease: String.Join(",", listRelease), QFName: QFName, InstanceFilterID: this.InstanceFilterID, NoteTypeFilterID: this.NoteTypeFilterID);

            if (this.Type == "Historical Notes") Session["dtAORMeetingInstancePopup"] = dt;
        }

        return dt;
    }

    private void LoadQF()
    {
        Label lblms_Item0 = (Label)Page.Master.FindControl("lblms_Item0");
        Label lblms_Item10 = (Label)Page.Master.FindControl("lblms_Item10");

        switch (this.Type)
        {
            case "AOR":
                lblms_Item0.Text = "System: ";
                lblms_Item0.Style["width"] = "150px";
                lblms_Item10.Text = "Release: ";
                lblms_Item10.Style["width"] = "150px";
                break;
            default:
                lblms_Item0.Text = "System: ";
                lblms_Item0.Style["width"] = "150px";
                lblms_Item10.Text = "Release: ";
                lblms_Item10.Style["width"] = "150px";
                break;
        }
    }

    private void PopulateQuickFilters()
    {
        ListItem li;

        if (this.Type == "AOR") // for now, this is all we support; we can add more type support moving forward
        {
            DataTable dtSystem = MasterData.SystemList_Get(includeArchive: false, cv: "0");
            HtmlSelect ms_Item0 = (HtmlSelect)Page.Master.FindControl("ms_Item0");
            HtmlSelect ms_Item10 = (HtmlSelect)Page.Master.FindControl("ms_Item10");

            if (dtSystem != null)
            {
                ms_Item0.Items.Clear();
                foreach (DataRow dr in dtSystem.Rows)
                {
                    li = new ListItem(dr["WTS_SYSTEM"].ToString(), dr["WTS_SystemID"].ToString());
                    li.Selected = (QFSystem.Count() == 0 || QFSystem.Contains(dr["WTS_SystemID"].ToString()));
                    li.Attributes.Add("OptionGroup", dr["WTS_SystemSuite"].ToString());
                    ms_Item0.Items.Add(li);
                }
            }

            DataTable dtRel = MasterData.ProductVersionList_Get(includeArchive: false);
            DataTable dtCurrentRel = AOR.AORCurrentRelease_Get();
            var currentRelID = "0";

            if (dtCurrentRel != null && dtCurrentRel.Rows.Count > 0)
                currentRelID = dtCurrentRel.Rows[0]["ProductVersionID"].ToString();

            if (dtRel != null)
            {
                ms_Item10.Items.Clear();
                foreach (DataRow dr in dtRel.Rows)
                {
                    li = new ListItem(dr["ProductVersion"].ToString(), dr["ProductVersionID"].ToString());
                    li.Selected = ((QFRelease.Count() == 0 && dr["ProductVersionID"].ToString() == currentRelID) || QFRelease.Contains(dr["ProductVersionID"].ToString()));
                    ms_Item10.Items.Add(li);
                }
            }
        }
    }
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        if (this.Type == "AOR" && this.CurrentSystemID != row.Cells[DCC.IndexOf("WTS_SYSTEM_ID")].Text)
        {
            CreateRow(row);
            this.CurrentSystemID = row.Cells[DCC.IndexOf("WTS_SYSTEM_ID")].Text;
        }

        FormatRowDisplay(ref row);

        if (this.CanEditAORMeetingInstance && DCC.Contains("X"))
        {
            if (this.Type == "AOR" && DCC.Contains("AORRelease_ID") && DCC.Contains("AOR Name"))
            {
                row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("AORRelease_ID")].Text));
            }
            else if (this.Type == "Resource" && DCC.Contains("WTS_RESOURCE_ID") && DCC.Contains("Resource"))
            {
                row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("WTS_RESOURCE_ID")].Text));
            }
        }

        if (DCC.Contains("Instance Date"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Instance Date")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Instance Date")].Text = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
            }
        }

        if (DCC.Contains("AOR #") && this.CanViewAOR && row.Cells[DCC.IndexOf("AOR #")].Text != "&nbsp;")
        {
            row.Cells[DCC.IndexOf("AOR #")].Controls.Add(CreateLink("AOR", row.Cells[DCC.IndexOf("AOR #")].Text));
        }

        if (DCC.Contains("Note #"))
        {
            row.Cells[DCC.IndexOf("Note #")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("Note #")].Controls.Add(CreateLink("Historical Notes", row.Cells[DCC.IndexOf("Note #")].Text));
        }

        if (DCC.Contains("Note Details"))
        {
            string txtNoteDetails = Server.HtmlDecode(row.Cells[DCC.IndexOf("Note Details")].Text);

            if (txtNoteDetails.Length > 50)
            {
                row.Cells[DCC.IndexOf("Note Details")].Controls.Add(CreateTextLink(txtNoteDetails, 50));
            }
            else
            {
                row.Cells[DCC.IndexOf("Note Details")].Text = txtNoteDetails;
            }
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

        if (this.Type == "AOR" && DCC.Contains("System")) row.Cells[DCC.IndexOf("System")].Style["display"] = "none";

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = "35px";
        }

        if (DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "45px";
        if (DCC.Contains("Instance Date")) row.Cells[DCC.IndexOf("Instance Date")].Style["width"] = "125px";
        if (DCC.Contains("Note Type"))
        {
            row.Cells[DCC.IndexOf("Note Type")].Style["width"] = "165px";
            row.Cells[DCC.IndexOf("Note Type")].Text = "Note Breakout";
        }
        if (DCC.Contains("Note #")) row.Cells[DCC.IndexOf("Note #")].Style["width"] = "50px";
        if (this.Type == "Historical Notes" && DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["width"] = "200px";
        if (DCC.Contains("Title")) row.Cells[DCC.IndexOf("Title")].Style["width"] = "200px";
        if (DCC.Contains("Status")) row.Cells[DCC.IndexOf("Status")].Style["width"] = "50px";
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

        if (this.Type == "AOR" && DCC.Contains("System")) row.Cells[DCC.IndexOf("System")].Style["display"] = "none";

        if (DCC.Contains("Instance Date")) row.Cells[DCC.IndexOf("Instance Date")].Style["text-align"] = "center";
    }

    private void CreateRow(GridViewRow row)
    {
        Table nTable = (Table)row.Parent;
        GridViewRow nRow = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
        TableCell nCell = new TableCell();

        nCell.BackColor = System.Drawing.Color.LightGray;
        nCell.ColumnSpan = 3;
        nCell.Text = row.Cells[DCC.IndexOf("System")].Text;

        nRow.Cells.Add(nCell);
        nTable.Rows.AddAt(nTable.Rows.Count - 1, nRow);
    }

    private CheckBox CreateCheckBox(string value)
    {
        CheckBox chk = new CheckBox();

        chk.Attributes["onchange"] = "input_change(this);";

        switch (this.Type)
        {
            case "AOR":
                chk.Attributes.Add("aorrelease_id", value);
                break;
            case "Resource":
                chk.Attributes.Add("resource_id", value);
                break;
        }

        return chk;
    }

    private LinkButton CreateTextLink(string txt, int sub)
    {
        LinkButton lb = new LinkButton();

        lb.Text = txt.Substring(0, sub) + "...";
        lb.ToolTip = txt;
        lb.Attributes["onclick"] = string.Format("showText('{0}'); return false;", Uri.EscapeDataString(txt));

        return lb;
    }

    private LinkButton CreateLink(string type, string value)
    {
        LinkButton lb = new LinkButton();

        lb.Text = value;

        switch (type)
        {
            case "AOR":
                lb.Attributes["onclick"] = string.Format("openAOR('{0}'); return false;", value);
                break;
            case "Historical Notes":
                lb.Attributes["onclick"] = string.Format("openNoteDetail('{0}'); return false;", value);
                break;
        }

        return lb;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string Add(string aorMeeting, string aorMeetingInstance, string type, string additions, string options, string addAORID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" }, { "newnoteid", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            int AORMeeting_ID = 0;
            int AORMeetingInstance_ID = 0;
            int AORIDToAdd = 0;

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);
            int.TryParse(addAORID, out AORIDToAdd);            

            if (AORIDToAdd > 0) // this is used when we are both adding a new aor and adding note details at the same time
            {
                int addedAORReleaseID = AOR.AORMeetingInstanceAOR_Add(AORMeeting_ID, AORMeetingInstance_ID, AORIDToAdd);

                if (additions.Contains("\"aorreleaseid\":\"0\""))
                {
                    additions = additions.Replace("\"aorreleaseid\":\"0\"", "\"aorreleaseid\":\"" + addedAORReleaseID + "\"");
                }
            }

            XmlDocument docAdditions = (XmlDocument)JsonConvert.DeserializeXmlNode(additions, "additions");

            if (type == "Note Detail")
            {
                saved = AOR.AORMeetingInstanceAdd_Save(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, Type: type, Additions: docAdditions);
                
                if (!string.IsNullOrWhiteSpace(options))
                {
                    string[] optArr = options.Split('|');
                    if (optArr != null && optArr.Length == 3)
                    {
                        int aorid = !string.IsNullOrWhiteSpace(optArr[0]) ? Convert.ToInt32(optArr[0]) : 0;
                        int notetypeid = !string.IsNullOrWhiteSpace(optArr[1]) ? Convert.ToInt32(optArr[1]) : 0;
                        string title = !string.IsNullOrWhiteSpace(optArr[2]) ? optArr[2].Replace("<pipe>", "|") : null;

                        DataTable dtNewNote = AOR.AORMeetingInstanceNotes_GetLastAdded(AORMeeting_ID, AORMeetingInstance_ID, aorid, notetypeid, title);

                        if (dtNewNote != null && dtNewNote.Rows.Count > 0)
                        {
                            DataRow row = dtNewNote.Rows[0];
                            result["newnoteid"] = row["AORMeetingNotesID"].ToString();
                        }
                    }
                }
            }
            else if (type == "Add Note Objectives" || type == "Edit Note Objectives")
            {
                bool addObjectives = type == "Add Note Objectives";

                int AORMeetingNotesID = 0;
                int AORReleaseID = 0;
                var outerNode = docAdditions.ChildNodes[0];
                var saveNode = outerNode.ChildNodes[0];
                string objectivesJson = null;

                List<Dictionary<string, string>> objectives = new List<Dictionary<string, string>>();
                string deletedObjectives = null;

                for (int i = 0; i < saveNode.ChildNodes.Count; i++)
                {
                    XmlNode node = saveNode.ChildNodes[i];

                    if (node.Name == "objectives" && node.ChildNodes.Count > 0)
                    {
                        objectivesJson = node.InnerXml;

                        for (int x = 0; x < node.ChildNodes.Count; x++)
                        {
                            XmlNode objectiveNode = node.ChildNodes[x];

                            Dictionary<string, string> objective = new Dictionary<string, string>();
                            foreach (XmlElement e in objectiveNode.ChildNodes)
                            {
                                string val = e.InnerText;

                                if (val != null)
                                {
                                    if (e.Name == "title" || e.Name == "notes")
                                    {
                                        // the html editor puts in a ton of html formatting, which can mess up our outer xml storage - so we remove it for now
                                        val = StringUtil.StrongEscape(val);
                                    }
                                }

                                objective[e.Name] = val;
                            }

                            objectives.Add(objective);
                        }
                    }
                    else if (node.Name == "aormeetingnotesid")
                    {
                        Int32.TryParse(node.InnerXml, out AORMeetingNotesID);
                    }
                    else if (node.Name == "aorreleaseid")
                    {
                        Int32.TryParse(node.InnerXml, out AORReleaseID);
                    }
                    else if (node.Name == "deletedobjectives")
                    {
                        deletedObjectives = node.InnerText;
                    }
                }

                if (!addObjectives && AORMeetingNotesID == 0)
                {
                    result["saved"] = "false";
                    result["error"] = "AORMeetingNotesID missing or invalid.";

                    return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
                }

                objectivesJson = JsonConvert.SerializeObject(objectives);
                if (objectivesJson == null) objectivesJson = "";
                string extData = "<objectivesjson>" + objectivesJson.Replace("\"", "\\\"") + "</objectivesjson>";

                // now that we have all the objectives, save them
                string noteAdditionTemplate = "{{save:[{{\"AORMEETINGNOTESVAR\":\"{0}\",\"aornotetypeid\":\"{1}\",\"aorreleaseid\":\"{2}\",\"title\":\"{3}\",\"notedetail\":\"{4}\",\"extdata\":\"{5}\"}}]}}";

                // save parent agenda/objectives note
                string agenda = AOR.GenerateAgendaFromObjectivesList(objectives);
                string parentAgendaTitle = saveNode.SelectSingleNode("title").InnerText;

                string addition = string.Format(noteAdditionTemplate, addObjectives ? "0" : AORMeetingNotesID.ToString(), (int)WTS.Enums.NoteTypeEnum.AgendaObjectives, (addObjectives ? "0" : AORReleaseID.ToString()), parentAgendaTitle.Replace("\"", "\\\""), agenda.ToString().Replace("\"", "\\\""), extData);
                addition = addition.Replace("AORMEETINGNOTESVAR", addObjectives ? "aormeetingnotesidparent" : "aormeetingnotesid");
                docAdditions = (XmlDocument)JsonConvert.DeserializeXmlNode(addition, "additions");
                saved = AOR.AORMeetingInstanceAdd_Save(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, Type: (addObjectives ? "Note Detail" : "Edit Note Detail"), Additions: docAdditions);

                // save all the notes marked with "createnote" flag
                for (int i = objectives.Count - 1; i >= 0; i--)
                {
                    Dictionary<string, string> objective = objectives[i];

                    if (objective["createnote"].ToLower() == "true")
                    {
                        // does a note using this agenda item key already exist? if so, edit that note instead of creating a new one
                        DataTable dtAgendaNote = AOR.AORMeetingInstanceNotesDetail_GetByAgendaKey(AORMeeting_ID, AORMeetingInstance_ID, objective["key"]);

                        bool noteExists = dtAgendaNote.Rows.Count > 0;

                        string noteExtData = "<agendaitemkey>" + objective["key"] + "</agendaitemkey>";
                        addition = string.Format(noteAdditionTemplate, (noteExists ? dtAgendaNote.Rows[0]["AORMeetingNotesID"].ToString() : "0"), (int)WTS.Enums.NoteTypeEnum.Notes, (noteExists && dtAgendaNote.Rows[0]["AORReleaseID"] != null ? dtAgendaNote.Rows[0]["AORReleaseID"].ToString() : "0"), StringUtil.UndoStrongEscape(objective["title"]).Replace("\"", "\\\""), StringUtil.UndoStrongEscape(objective["notes"]).Replace("\"", "\\\""), noteExtData);
                        addition = addition.Replace("AORMEETINGNOTESVAR", noteExists ? "aormeetingnotesid" : "aormeetingnotesidparent");
                        docAdditions = (XmlDocument)JsonConvert.DeserializeXmlNode(addition, "additions");
                        saved &= AOR.AORMeetingInstanceAdd_Save(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, Type: (noteExists ? "Edit Note Detail" : "Note Detail"), Additions: docAdditions);
                    }
                }

                // delete all notes that were linked to deleted notes
                if (!string.IsNullOrWhiteSpace(deletedObjectives))
                {
                    string[] delArr = deletedObjectives.Split(',');

                    foreach (string key in delArr)
                    {
                        DataTable dtAgendaNote = AOR.AORMeetingInstanceNotesDetail_GetByAgendaKey(AORMeeting_ID, AORMeetingInstance_ID, key);

                        if (dtAgendaNote != null)
                        {
                            AOR.AORMeetingInstanceNote_Toggle(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, AORMeetingNotesID: (int)dtAgendaNote.Rows[0]["AORMeetingNotesID"], Opt: 0);
                        }
                    }
                }

                if (saved)
                {
                    DataTable dtNewNote = AOR.AORMeetingInstanceNotes_GetLastAdded(AORMeeting_ID, AORMeetingInstance_ID, 0, (int)WTS.Enums.NoteTypeEnum.AgendaObjectives, parentAgendaTitle);

                    if (dtNewNote != null && dtNewNote.Rows.Count > 0)
                    {
                        DataRow row = dtNewNote.Rows[0];
                        result["newnoteid"] = row["AORMeetingNotesID"].ToString();
                    }
                }
            }
            else
            {
                saved = AOR.AORMeetingInstanceAdd_Save(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, Type: type, Additions: docAdditions);

                if (type == "Edit Note Detail" && additions.IndexOf("<agendaitemkey>") != -1) // this note is tied to an agenda item, so we need to modify the agenda
                {
                    var outerNode = docAdditions.ChildNodes[0];
                    var saveNode = outerNode.ChildNodes[0];

                    ModifyAgendaAfterSave(AORMeeting_ID, AORMeetingInstance_ID, saveNode);
                }
            }
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

    private static void ModifyAgendaAfterSave(int AORMeeting_ID, int AORMeetingInstance_ID, XmlNode saveNode)
    {
        Dictionary<string, string> note = new Dictionary<string, string>();

        bool agendaKeyFound = false;

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
                }
            }
        }

        if (!agendaKeyFound)
        {
            return;
        }

        // get all the agendas tied to this meeting
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

                string extData = note["extdata"];
                int idx = extData.IndexOf("<agendaitemkey>");
                int len = "<agendaitemkey>".Length;
                int idx2 = extData.IndexOf("</agendaitemkey>");
                string key = extData.Substring(idx + len, idx2 - (idx + len));

                Dictionary<string, string> affectedObjective = objectives.Find(obj => obj["key"] == key);
                if (affectedObjective != null && affectedObjective["createnote"] == "true") // if false, we aren't linked any more so don't update
                {
                    // this note affects the current agenda
                    // only a couple of properties can be touched (title, text, deleted or not)
                    affectedObjective["title"] = StringUtil.StrongEscape(note["title"]);
                    affectedObjective["notes"] = StringUtil.StrongEscape(note["notedetail"]);

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

    [WebMethod()]
    public static string GetAOROptions(string aorMeeting, string aorMeetingInstance, int aorsIncluded, int all)
    {
        DataTable dt = new DataTable();

        try
        {
            int AORMeeting_ID = 0, AORMeetingInstance_ID = 0;
            string nType = "Note Detail";

            int.TryParse(aorMeeting, out AORMeeting_ID);
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);

            if (all == 1)
            {
                nType = "Note Detail All System AOR";
            }
            else if (aorsIncluded == 0)
            {
                nType = "Note Detail All AOR";
            }

            dt = AOR.AORMeetingInstanceAddList_Get(AORMeetingID: AORMeeting_ID, AORMeetingInstanceID: AORMeetingInstance_ID, Type: nType, QFSystem: string.Empty, QFRelease: string.Empty, InstanceFilterID: 0, NoteTypeFilterID: 0);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string ValidateTaskID(int taskID)
    {
        var result = WTSPage.CreateDefaultResult();

        DataTable dt = WorkloadItem.WorkItem_Get(taskID);

        if (dt != null && dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            result["title"] = dr["TITLE"] != DBNull.Value ? HttpUtility.HtmlEncode(dr["TITLE"].ToString()) : "";
            result["exists"] = "true";

            DataTable aorDT = AOR.AORTaskAORList_Get(taskID);

            if (aorDT != null && aorDT.Rows.Count > 0)
            {
                Dictionary<string, string> aors = new Dictionary<string, string>();
                for (int i = 0; i < aorDT.Rows.Count; i++)
                {
                    DataRow aorRow = aorDT.Rows[i];
                    aors.Add(aorRow["AORID"].ToString(), aorRow["AORName"].ToString());
                }

                result["aors"] = JsonConvert.SerializeObject(aors, Newtonsoft.Json.Formatting.None);
            }            
        }
        else
        {
            result["exists"] = "false";
        }

        result["success"] = "true";

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string ValidateSubTaskNumber(int subTaskNumber, int parentTaskID)
    {
        var result = WTSPage.CreateDefaultResult();

        WorkItem_Task task = new WorkItem_Task();
        bool loaded = task.LoadByNumber(parentTaskID, subTaskNumber);

        if (loaded)
        {
            result["WORKITEMID"] = task.WorkItemID.ToString();
            result["WORKITEM_TASKID"] = task.WorkItem_TaskID.ToString();
            result["TASK_NUMBER"] = subTaskNumber.ToString();

            result["title"] = task.Title;
            result["exists"] = "true";
        }
        else
        {
            result["exists"] = "false";
        }

        result["success"] = "true";

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    #endregion
}