using Aspose.Cells;  //for exporting to excel
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;
using Row = System.Data.Common.CommandTrees.ExpressionBuilder.Row;

public partial class AOR_Popup : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool CanEditWorkItem = false;
    protected string AORName = string.Empty;
    protected int AORID = 0;
    protected int SRID = 0;
    protected int TaskID = 0;
    protected int CRID = 0;
    protected int DeliverableID = 0;
    protected int ReleaseID = 0;
    protected int ReleaseAssessmentID = 0;
    protected string Type = string.Empty;
    protected string SubType = string.Empty;
    protected int ReleaseFilterID = 0;
    protected string FieldChangedFilter = "0";
    protected string[] QFSystem = { };
    protected string[] QFRelease = { };
    protected string[] QFCRContract = { };
    protected string[] QFStatus = { };
    protected string[] QFDeployment = { };
    protected string QFName = "";
    protected bool _export = false;
    protected string ReleaseOptions = string.Empty;
    protected DataTable dtDeployment;
    private DataColumnCollection DCC;
    protected string CurrentSystemID;
    protected string CurrentReleaseID;
    private XmlDocument Levels = new XmlDocument();
    protected int LevelCount = 0;
    protected int dtColumnCnt = 0;
    protected int rowCount = 0;
    protected int uniA = (int)'A';
    protected int dtRowCount = 0;
    protected string AddCallbackFunction = string.Empty;
    protected string SelectCallbackFunction = string.Empty;
    protected string SelectedTasks = string.Empty;
    protected string SelectedSubTasks = string.Empty;
    protected string SelectedSystems = string.Empty;
    protected bool HideAdd = false;

    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        var tempVal = Request.QueryString;

        ReadQueryString();
        InitializeEvents();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);

        DataSet dsOptions;
        ListItem li;
        LoadQF();

        switch (this.Type)
        {
            case "Release History":
                DataTable dtReleaseHistory = new DataTable();

                if (IsPostBack && Session["dtAORPopupReleaseHistory"] != null)
                {
                    dtReleaseHistory = (DataTable)Session["dtAORPopupReleaseHistory"];
                }
                else
                {
                    dtReleaseHistory = AOR.AORTaskReleaseHistoryList_Get(AORID: this.AORID, TaskID: this.TaskID);
                    Session["dtAORPopupReleaseHistory"] = dtReleaseHistory;
                }

                if (dtReleaseHistory != null) this.DCC = dtReleaseHistory.Columns;

                grdData.DataSource = dtReleaseHistory;
                grdData.DataBind();
                break;
            case "Change History":
                if (!Page.IsPostBack)
                {
                    dsOptions = AOR.AOROptionsList_Get(AORID: this.AORID, TaskID: this.TaskID, AORMeetingID: 0, AORMeetingInstanceID: 0);

                    if (dsOptions != null)
                    {
                        DataTable dtRelease = dsOptions.Tables["Release"];

                        ddlReleaseQF.DataSource = dtRelease;
                        ddlReleaseQF.DataValueField = "Value";
                        ddlReleaseQF.DataTextField = "Text";
                        ddlReleaseQF.DataBind();

                        DataTable dtFieldChanged = dsOptions.Tables["Field Changed"];

                        ddlFieldChangedQF.DataSource = dtFieldChanged;
                        ddlFieldChangedQF.DataValueField = "Value";
                        ddlFieldChangedQF.DataTextField = "Text";
                        ddlFieldChangedQF.DataBind();
                    }

                    li = new ListItem();

                    li.Value = "0";
                    li.Text = "- All -";

                    ddlReleaseQF.Items.Insert(0, li);
                    ddlReleaseQF.SelectedValue = this.ReleaseFilterID.ToString();

                    li = new ListItem();

                    li.Value = "0";
                    li.Text = "- All -";

                    ddlFieldChangedQF.Items.Insert(0, li);
                    ddlFieldChangedQF.SelectedValue = this.FieldChangedFilter;
                }
                
                DataTable dtChangeHistory = new DataTable();

                if (IsPostBack && Session["dtAORPopupChangeHistory"] != null)
                {
                    dtChangeHistory = (DataTable)Session["dtAORPopupChangeHistory"];
                }
                else
                {
                    dtChangeHistory = AOR.AORTaskHistoryList_Get(AORID: this.AORID, TaskID: this.TaskID, ReleaseFilterID: this.ReleaseFilterID, FieldChangedFilter: this.FieldChangedFilter);
                    Session["dtAORPopupChangeHistory"] = dtChangeHistory;
                }

                if (dtChangeHistory != null) this.DCC = dtChangeHistory.Columns;

                grdData.DataSource = dtChangeHistory;
                grdData.DataBind();
                break;
            case "CR":
                if (_export)
                {
                    DataTable dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: 0, SRID: 0, CRID: 0, DeliverableID: 0, Type: "CR", Filters: null, CRStatus: string.Join(",", QFStatus), CRContract: string.Join(",", QFCRContract), TaskID: string.Empty, QFSystem: String.Join(",", QFSystem), QFRelease: String.Join(",", QFRelease), QFName: QFName);
                    ExportExcel("CRs", dt);
                }

                PopulateQuickFilters();

                break;
            case "Attachment":
                if (!Page.IsPostBack)
                {
                    DataTable dt = LoadData(systemQF: "", releaseQF: "", nameQF: "");

                    ddlType.DataSource = dt;
                    ddlType.DataValueField = "AORAttachmentTypeID";
                    ddlType.DataTextField = "AORAttachmentTypeName";
                    ddlType.DataBind();

                    li = new ListItem();

                    li.Value = "0";
                    li.Text = "";

                    ddlType.Items.Insert(0, li);
                }
                break;
            case "Previous Attachment":
                DataTable dtPreviousAttachment = new DataTable();

                if (IsPostBack && Session["dtPreviousAttachment"] != null)
                {
                    dtPreviousAttachment = (DataTable)Session["dtPreviousAttachment"];
                }
                else
                {
                    dtPreviousAttachment = LoadData(systemQF: "", releaseQF: "", nameQF: "");
                    Session["dtPreviousAttachment"] = dtPreviousAttachment;
                }

                if (dtPreviousAttachment != null) this.DCC = dtPreviousAttachment.Columns;

                grdData.DataSource = dtPreviousAttachment;
                grdData.DataBind();
                break;
            case "SubTask":
                PopulateQuickFilters();
                break;
            case "AOR":
            case "Archive AOR":
                DataTable dtProductVersion = MasterData.ProductVersionList_Get(includeArchive: false);
                DataTable dtCurrentRelease = AOR.AORCurrentRelease_Get();
                string currentReleaseID = "0";

                if (dtCurrentRelease != null && dtCurrentRelease.Rows.Count > 0)
                    currentReleaseID = dtCurrentRelease.Rows[0]["ProductVersionID"].ToString();

                if (this.Type == "Archive AOR")
                {
                    DataTable dtAOR = AOR.AORList_Get(AORID: 0);

                    dtAOR.DefaultView.RowFilter = "[AOR #] <> " + this.AORID;
                    dtAOR = dtAOR.DefaultView.ToTable();

                    ddlCopyTasksToAORExisting.DataSource = dtAOR;
                    ddlCopyTasksToAORExisting.DataValueField = "AORRelease_ID";
                    ddlCopyTasksToAORExisting.DataTextField = "AOR Name";
                    ddlCopyTasksToAORExisting.DataBind();

                    ddlCopyTasksToAORRelease.DataSource = dtProductVersion;
                    ddlCopyTasksToAORRelease.DataValueField = "ProductVersionID";
                    ddlCopyTasksToAORRelease.DataTextField = "ProductVersion";
                    ddlCopyTasksToAORRelease.DataBind();
                    ddlCopyTasksToAORRelease.SelectedValue = currentReleaseID;
                }
                else
                {
                    foreach (DataRow dr in dtProductVersion.Rows)
                        ReleaseOptions += "<option value=\"" + dr["ProductVersionID"] + "\"" + (dr["ProductVersionID"].ToString() == currentReleaseID ? " selected" : "") + ">" + Uri.EscapeDataString(dr["ProductVersion"].ToString()) + "</option>";
                }
                break;
            case "Add/Move Deployment AOR":
                dtDeployment = MasterData.ReleaseSchedule_DeliverableList_Get(ReleaseVersionID: this.ReleaseID);
                dtDeployment.Columns.Add("Deployment");

                foreach (DataRow dr in dtDeployment.Rows)
                {
                    if (dr["ReleaseScheduleID"].ToString() != "0") dr["Deployment"] = "." + dr["ReleaseScheduleDeliverable"].ToString() + " - " + dr["Description"].ToString();
                }

                DataTable dtSystem = MasterData.SystemList_Get(includeArchive: false, cv: "0");
                HtmlSelect ms_Item0 = (HtmlSelect)Page.Master.FindControl("ms_Item0");
                HtmlSelect ms_Item10 = (HtmlSelect)Page.Master.FindControl("ms_Item10");
                HtmlSelect ms_Item2 = (HtmlSelect)Page.Master.FindControl("ms_Item2");

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

                currentRelID = this.ReleaseID.ToString();

                if (dtRel != null)
                {
                    ms_Item10.Items.Clear();
                    foreach (DataRow dr in dtRel.Rows)
                    {
                        li = new ListItem(dr["ProductVersion"].ToString(), dr["ProductVersionID"].ToString());
                        li.Selected = (QFRelease.Count() == 0 || QFRelease.Contains(dr["ProductVersionID"].ToString()));

                        if (QFRelease.Count() == 0)
                        {
                            if (dr["ProductVersionID"].ToString() == currentRelID)
                            {
                                li.Selected = true;
                            }
                            else
                            {
                                li.Selected = false;
                            }
                        }

                        ms_Item10.Items.Add(li);
                    }
                }

                if (dtDeployment != null)
                {
                    ms_Item2.Items.Clear();
                    foreach (DataRow dr in dtDeployment.Rows)
                    {
                        li = new ListItem(dr["ReleaseScheduleDeliverable"].ToString(), dr["ReleaseScheduleID"].ToString());
                        li.Selected = ((QFDeployment.Count() == 0 && dr["ReleaseScheduleID"].ToString() == this.DeliverableID.ToString()) 
                            || (QFDeployment.Count() == 0 && dr["ReleaseScheduleID"].ToString() == "0") 
                            || QFDeployment.Contains(dr["ReleaseScheduleID"].ToString()));
                        ms_Item2.Items.Add(li);
                    }
                }

                var listSystem = new List<string>();

                if (ms_Item0.Items.Count > 0)
                    foreach (ListItem nItem in ms_Item0.Items)
                        if (nItem.Selected) listSystem.Add(nItem.Value);

                var listRelease = new List<string>();

                if (ms_Item10.Items.Count > 0)
                    foreach (ListItem nItem in ms_Item10.Items)
                        if (nItem.Selected) listRelease.Add(nItem.Value);

                var listDeployment = new List<string>();

                if (ms_Item2.Items.Count > 0)
                    foreach (ListItem nItem in ms_Item2.Items)
                        if (nItem.Selected) listDeployment.Add(nItem.Value);

                DataTable dtCRAOR = LoadData(String.Join(",", listSystem), String.Join(",", listRelease), QFName, String.Join(",", listDeployment));

                if (dtCRAOR != null) this.DCC = dtCRAOR.Columns;

                grdData.AllowPaging = false;
                grdData.AlternatingRowColor = System.Drawing.Color.White;
                grdData.DataSource = dtCRAOR;
                grdData.DataBind();
                break;
            case "Release Schedule AOR":
            case "CR AOR":
                dtSystem = MasterData.SystemList_Get(includeArchive: false, cv: "0");
                ms_Item0 = (HtmlSelect)Page.Master.FindControl("ms_Item0");
                ms_Item10 = (HtmlSelect)Page.Master.FindControl("ms_Item10");

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

                dtRel = MasterData.ProductVersionList_Get(includeArchive: false);
                dtCurrentRel = AOR.AORCurrentRelease_Get();
                currentRelID = "0";

                if (dtCurrentRel != null && dtCurrentRel.Rows.Count > 0)
                    currentRelID = dtCurrentRel.Rows[0]["ProductVersionID"].ToString();

                if (this.Type == "Release Schedule AOR")
                    currentRelID = this.ReleaseID.ToString();

                if (dtRel != null)
                {
                    ms_Item10.Items.Clear();
                    foreach (DataRow dr in dtRel.Rows)
                    {
                        li = new ListItem(dr["ProductVersion"].ToString(), dr["ProductVersionID"].ToString());
                        li.Selected = (QFRelease.Count() == 0 || QFRelease.Contains(dr["ProductVersionID"].ToString()));

                        if (QFRelease.Count() == 0)
                        {
                            if (dr["ProductVersionID"].ToString() == currentRelID)
                            {
                                li.Selected = true;
                            }
                            else {
                                li.Selected = false;
                            }
                        }

                        ms_Item10.Items.Add(li);
                    }
                }

                listSystem = new List<string>();

                if (ms_Item0.Items.Count > 0)
                    foreach (ListItem nItem in ms_Item0.Items)
                        if (nItem.Selected) listSystem.Add(nItem.Value);

                listRelease = new List<string>();

                if (ms_Item10.Items.Count > 0)
                    foreach (ListItem nItem in ms_Item10.Items)
                        if (nItem.Selected) listRelease.Add(nItem.Value);

                dtCRAOR = LoadData(String.Join(",", listSystem), String.Join(",", listRelease), QFName);

                if (dtCRAOR != null) this.DCC = dtCRAOR.Columns;

                grdData.AllowPaging = false;
                grdData.AlternatingRowColor = System.Drawing.Color.White;
                grdData.DataSource = dtCRAOR;
                grdData.DataBind();
                break;
            case "Contract":
                DataTable dtContract = MasterData.ContractList_Get(deliverableID: DeliverableID, includeArchive: false);
                if (dtContract != null) this.DCC = dtContract.Columns;

                grdData.AllowPaging = false;
                grdData.AlternatingRowColor = System.Drawing.Color.White;
                grdData.DataSource = dtContract;
                grdData.DataBind();
                break;
            case "Action Team":
                DataTable dtResourceTeam = LoadData();

                if (dtResourceTeam != null) this.DCC = dtResourceTeam.Columns;

                grdData.AllowPaging = false;
                grdData.AlternatingRowColor = System.Drawing.Color.White;
                grdData.DataSource = dtResourceTeam;
                grdData.DataBind();
                break;
            case "Deployment":
                dtDeployment = LoadData(releaseQF: ReleaseID.ToString());

                if (dtDeployment != null) this.DCC = dtDeployment.Columns;

                grdData.DataSource = dtDeployment;
                grdData.DataBind();
                break;
        }
    }

    private void PopulateQuickFilters()
    {
        ListItem li;

        if (this.Type == "CR")
        {
            DataTable dtSystem = MasterData.SystemList_Get(includeArchive: false, cv: "0");
            HtmlSelect ms_Item0 = (HtmlSelect)Page.Master.FindControl("ms_Item0");
            HtmlSelect ms_Item10 = (HtmlSelect)Page.Master.FindControl("ms_Item10");
            HtmlSelect ms_Item10a = (HtmlSelect)Page.Master.FindControl("ms_Item10a");
            HtmlSelect ms_Item1 = (HtmlSelect)Page.Master.FindControl("ms_Item1");

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
                    li.Selected = (QFRelease.Count() == 0 || QFRelease.Contains(dr["ProductVersionID"].ToString()));

                    if (QFRelease.Count() == 0)
                    {
                        if (dr["ProductVersionID"].ToString() == currentRelID)
                        {
                            li.Selected = true;
                        }
                        else
                        {
                            li.Selected = false;
                        }
                    }

                    ms_Item10.Items.Add(li);
                }
            }

            var listSystem = new List<string>();

            if (ms_Item0.Items.Count > 0)
                foreach (ListItem nItem in ms_Item0.Items)
                    if (nItem.Selected) listSystem.Add(nItem.Value);

            var listRelease = new List<string>();

            if (ms_Item10.Items.Count > 0)
                foreach (ListItem nItem in ms_Item10.Items)
                    if (nItem.Selected) listRelease.Add(nItem.Value);

            DataSet dsOptions = AOR.AOROptionsList_Get(AORID: this.AORID, TaskID: 0, AORMeetingID: 0, AORMeetingInstanceID: 0);

            if (dsOptions != null)
            {
                DataTable dtCRStatus = dsOptions.Tables["CR Status"];

                foreach (DataRow dr in dtCRStatus.Rows)
                {
                    ListItem liCRStatus = new ListItem(dr["Text"].ToString(), dr["Value"].ToString());

                    if (QFStatus != null && QFStatus.Length > 0)
                    {
                        if (QFStatus.Contains(dr["Value"].ToString())) liCRStatus.Selected = true;
                    }
                    else
                    {
                        if (liCRStatus.Text.ToUpper() != "RESOLVED") liCRStatus.Selected = true;
                    }

                    ms_Item10a.Items.Add(liCRStatus);
                }

                DataTable dtCRContract = dsOptions.Tables["CR Contract"];

                ms_Item1.Items.Clear();
                foreach (DataRow dr in dtCRContract.Rows)
                {
                    li = new ListItem(dr["Text"].ToString(), dr["Value"].ToString());
                    li.Selected = (QFCRContract.Count() == 0 || QFCRContract.Contains(dr["Value"].ToString()));
                    ms_Item1.Items.Add(li);
                }
            }
        }
        else if (Type == "SubTask")
        {
            DataTable dtSystem = MasterData.SystemList_Get(includeArchive: false, cv: "0");
            if (dtSystem != null)
            {
                dtSystem.Columns.Add("DefaultSelected", typeof(int));
                dtSystem.AcceptChanges();

                if (!string.IsNullOrWhiteSpace(SelectedSystems))
                {
                    foreach (DataRow row in dtSystem.Rows)
                    {
                        int sid = (int)row["WTS_SystemID"];

                        if (("," + SelectedSystems + ",").IndexOf("," + sid + ",") != -1)
                        {
                            row["DefaultSelected"] = "1";
                        }
                    }

                    dtSystem.AcceptChanges();
                }
                
                msSystem.Items = dtSystem;
                msSystem.IsVisible = true;
            }            

            DataTable dtStatus = MasterData.WorkType_StatusList_Get(35, 0);
            if (dtStatus != null)
            {
                dtStatus.Columns.Add("DefaultSelected", typeof(string));
                for (int i = 0; i < dtStatus.Rows.Count; i++)
                {
                    string status = dtStatus.Rows[i]["Status"].ToString();

                    if (status != "Closed" && status != "On Hold")
                    {
                        dtStatus.Rows[i]["DefaultSelected"] = "true";
                    }
                    else
                    {
                        dtStatus.Rows[i]["DefaultSelected"] = "false";
                    }
                }
                dtStatus.AcceptChanges();

                msSubTaskStatus.Items = dtStatus;
                msSubTaskStatus.Visible = true;
            }
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

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.AORID);
        }

        if (Request.QueryString["SRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SRID"]))
        {
            int.TryParse(Request.QueryString["SRID"], out this.SRID);
        }

        if (Request.QueryString["TaskID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["TaskID"]))
        {
            int.TryParse(Request.QueryString["TaskID"], out this.TaskID);
        }

        if (Request.QueryString["CRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CRID"]))
        {
            int.TryParse(Request.QueryString["CRID"], out this.CRID);
        }

        if (Request.QueryString["DeliverableID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["DeliverableID"]))
        {
            int.TryParse(Request.QueryString["DeliverableID"], out this.DeliverableID);
        }

        if (Request.QueryString["Type"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Type"]))
        {
            this.Type = Request.QueryString["Type"];
        }

        if (Request.QueryString["SubType"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SubType"]))
        {
            this.SubType = Request.QueryString["SubType"];
        }

        if (Request.QueryString["AddCallback"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AddCallback"]))
        {
            this.AddCallbackFunction = Request.QueryString["AddCallback"];
        }

        if (Request.QueryString["SelectCallback"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectCallback"]))
        {
            this.SelectCallbackFunction = Request.QueryString["SelectCallback"];
        }        

        if (Request.QueryString["ReleaseFilterID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseFilterID"]))
        {
            int.TryParse(Request.QueryString["ReleaseFilterID"], out this.ReleaseFilterID);
        }

        if (Request.QueryString["ReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseID"]))
        {
            int.TryParse(Request.QueryString["ReleaseID"], out this.ReleaseID);
        }

        if (Request.QueryString["ReleaseAssessmentID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseAssessmentID"]))
        {
            int.TryParse(Request.QueryString["ReleaseAssessmentID"], out this.ReleaseAssessmentID);
        }

        if (Request.QueryString["FieldChangedFilter"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["FieldChangedFilter"]))
        {
            this.FieldChangedFilter = Request.QueryString["FieldChangedFilter"];
        }

        if (Request.QueryString["SelectedSystems"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSystems"]))
            this.QFSystem = Request.QueryString["SelectedSystems"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["SelectedReleases"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedReleases"]))
            this.QFRelease = Request.QueryString["SelectedReleases"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["SelectedCRContracts"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedCRContracts"]))
            this.QFCRContract = Request.QueryString["SelectedCRContracts"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"]))
            this.QFStatus = Request.QueryString["SelectedStatuses"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["SelectedDeployments"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedDeployments"]))
            this.QFDeployment = Request.QueryString["SelectedDeployments"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["txtSearch"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["txtSearch"]))
        {
            this.QFName = Request.QueryString["txtSearch"].Trim();
            txtSearch.Text = Request.QueryString["txtSearch"].Trim(); // this just resets the search box back to default value
        }

        if (Request.QueryString["Export"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);

        if (Request.QueryString["SelectedTasks"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedTasks"]))
            this.SelectedTasks = Request.QueryString["SelectedTasks"];

        if (Request.QueryString["SelectedSubTasks"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSubTasks"]))
            this.SelectedSubTasks = Request.QueryString["SelectedSubTasks"];

        if (Request.QueryString["SelectedSystems"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSystems"]))
            this.SelectedSystems = Request.QueryString["SelectedSystems"];

        if (HttpContext.Current.Session["filters_AorWork"] != null && !string.IsNullOrWhiteSpace(HttpContext.Current.Session["filters_AorWork"].ToString()))
            Page.ClientScript.RegisterArrayDeclaration("filters_AorWork", JsonConvert.SerializeObject(HttpContext.Current.Session["filters_AorWork"], Newtonsoft.Json.Formatting.None));

        if (Request.QueryString["AORName"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORName"]))
        {
            this.AORName = Request.QueryString["AORName"];
            this.txtAORName.Text = this.AORName;
        }

        if (Request.QueryString["HideAdd"] == "true")
        {
            this.HideAdd = true;
        }
    }

    private void InitializeEvents()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;
        btnSubmit.Click += btnSubmit_Click;
    }

    protected void btnSubmit_Click(Object sender, EventArgs e)
    {
        Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

        try
        {
            var count = 0;
            if (fileUpload.HasFiles)
                foreach (var file in fileUpload.PostedFiles)
                {
                    int AORAttachmentType_ID = 0;
                    Stream fileStream = file.InputStream;
                    byte[] fileData = new byte[file.ContentLength];
                    string fileName = file.FileName;
                    string[] splitFileName = fileName.Split('\\');

                    int.TryParse(this.ddlType.SelectedValue, out AORAttachmentType_ID);
                    fileName = splitFileName[splitFileName.Length - 1];

                    fileStream.Read(fileData, 0, fileData.Length);
                    fileStream.Close();

                    result = AOR.AORAttachment_Save(AORID: this.AORID, AORAttachmentTypeID: AORAttachmentType_ID, AORReleaseAttachmentName: txtAORAttachmentName.Text, FileName: fileName, Description: txtDescription.Text, FileData: fileData);
                    count++;
                }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            result["error"] = ex.Message;
        }

        ScriptManager.RegisterStartupScript(this, this.GetType(), "complete", "<script type=\"text/javascript\">complete('" + JsonConvert.SerializeObject(result) + "');</script>", false);
    }
    #endregion

    #region Data

    private void LoadQF()
    {
        Label lblms_Item0 = (Label)Page.Master.FindControl("lblms_Item0");
        Label lblms_Item10 = (Label)Page.Master.FindControl("lblms_Item10");
        Label lblms_Item10a = (Label)Page.Master.FindControl("lblms_Item10a");
        Label lblms_Item1 = (Label)Page.Master.FindControl("lblms_Item1");
        Label lblms_Item2 = (Label)Page.Master.FindControl("lblms_Item2");

        switch (this.Type) {
            case "CR":
                lblms_Item0.Text = "Suite: ";
                lblms_Item0.Style["width"] = "150px";
                lblms_Item10.Text = "Release: ";
                lblms_Item10.Style["width"] = "150px";
                lblms_Item10a.Text = "CR Coordination: ";
                lblms_Item10a.Style["width"] = "150px";
                lblms_Item1.Text = "CR Contract: ";
                lblms_Item1.Style["width"] = "150px";
                break;
            case "Add/Move Deployment AOR":
                lblms_Item0.Text = "System: ";
                lblms_Item0.Style["width"] = "150px";
                lblms_Item10.Text = "Release: ";
                lblms_Item10.Style["width"] = "150px";
                lblms_Item2.Text = "Deployment: ";
                lblms_Item2.Style["width"] = "150px";
                break;
            default:
                lblms_Item0.Text = "System: ";
                lblms_Item0.Style["width"] = "150px";
                lblms_Item10.Text = "Release: ";
                lblms_Item10.Style["width"] = "150px";
                break;
        }
    }

    private DataTable LoadData(string systemQF = "", string releaseQF = "", string nameQF = "", string deploymentQF = "")
    {
        var dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: 0, SRID: 0, CRID: this.CRID, DeliverableID: this.DeliverableID, Type: this.Type, Filters: null, CRStatus: string.Empty, CRContract: string.Empty, TaskID: string.Empty, QFSystem: systemQF, QFRelease: releaseQF, QFName: nameQF, QFDeployment: deploymentQF);

        if (txtSearch.Text != "") dt.FilterColNames("AOR Name", txtSearch.Text);
        if (_export) ExportExcel("AORs", dt.DefaultView.ToTable());

        return dt;
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

        if ((this.Type == "CR AOR" || this.Type == "Release Schedule AOR" || this.Type == "Add/Move Deployment AOR") && this.CurrentSystemID != row.Cells[DCC.IndexOf("WTS_SYSTEM_ID")].Text)
        {
            CreateRow(row);
            this.CurrentSystemID = row.Cells[DCC.IndexOf("WTS_SYSTEM_ID")].Text;
            this.CurrentReleaseID = row.Cells[DCC.IndexOf("ProductVersion_ID")].Text;
        }

        if ((this.Type == "CR AOR" || this.Type == "Release Schedule AOR" || this.Type == "Add/Move Deployment AOR" || this.Type == "Previous Attachment") && this.CurrentReleaseID != row.Cells[DCC.IndexOf("ProductVersion_ID")].Text)
        {
            for (int i = 0; i < row.Cells.Count; i++)
            {
                row.Cells[i].Style["border-top"] = "1px solid blue";
            }

            this.CurrentReleaseID = row.Cells[DCC.IndexOf("ProductVersion_ID")].Text;
        }

        FormatRowDisplay(ref row);

        if (DCC.Contains("Date"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Date")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Date")].Text = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
            }
        }

        if (DCC.Contains("Change Date"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Change Date")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Change Date")].Text = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
            }
        }

        if (DCC.Contains("Old Value"))
        {
            string txtOldValue = Server.HtmlDecode(row.Cells[DCC.IndexOf("Old Value")].Text);

            if (row.Cells[DCC.IndexOf("Field Changed")].Text == "Description")
            {
                row.Cells[DCC.IndexOf("Old Value")].Controls.Add(CreateTextLink(txtOldValue, 0));
                //HtmlGenericControl nDiv = new HtmlGenericControl("div");

                //nDiv.InnerHtml = txtOldValue;
                //row.Cells[DCC.IndexOf("Old Value")].Controls.Add(nDiv);
            }
            else
            {
                if (txtOldValue.Length > 40)
                {
                    row.Cells[DCC.IndexOf("Old Value")].Controls.Add(CreateTextLink(txtOldValue, 40));
                }
                else
                {
                    row.Cells[DCC.IndexOf("Old Value")].Text = txtOldValue;
                }
            }
        }

        if (DCC.Contains("New Value"))
        {
            string txtNewValue = Server.HtmlDecode(row.Cells[DCC.IndexOf("New Value")].Text);

            if (row.Cells[DCC.IndexOf("Field Changed")].Text == "Description")
            {
                row.Cells[DCC.IndexOf("New Value")].Controls.Add(CreateTextLink(txtNewValue, 0));
                //HtmlGenericControl nDiv = new HtmlGenericControl("div");

                //nDiv.InnerHtml = txtNewValue;
                //row.Cells[DCC.IndexOf("New Value")].Controls.Add(nDiv);
            }
            else
            {
                if (txtNewValue.Length > 40)
                {
                    row.Cells[DCC.IndexOf("New Value")].Controls.Add(CreateTextLink(txtNewValue, 40));
                }
                else
                {
                    row.Cells[DCC.IndexOf("New Value")].Text = txtNewValue;
                }
            }
        }

        string strAdd = string.Empty;
        if (DCC.Contains("Added Date"))
        {
            strAdd = row.Cells[DCC.IndexOf("Added By")].Text;
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Added Date")].Text, out nDate))
            {
                strAdd += " " + String.Format(" {0:M/d/yyyy h:mm tt}", nDate);
            }
        }

        string strUpdate = string.Empty;
        if (DCC.Contains("Updated Date"))
        {
            strUpdate = row.Cells[DCC.IndexOf("Updated By")].Text;
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Updated Date")].Text, out nDate))
            {
                strUpdate += " " + String.Format(" {0:M/d/yyyy h:mm tt}", nDate);
            }
        }

        if (DCC.Contains("Added Date"))
        {
            row.Cells[DCC.IndexOf("Added By")].Text = strAdd != strUpdate ? strAdd + "<br>" + strUpdate : strAdd;
            row.Cells[DCC.IndexOf("Added By")].Style["font-size"] = "10px";
            row.Cells[DCC.IndexOf("Added By")].Style["white-space"] = "nowrap";
            row.Cells[DCC.IndexOf("Added By")].Style["vertical-align"] = "top";
        }

        if (DCC.Contains("Approved"))
        {
            row.Cells[DCC.IndexOf("Approved")].Controls.Clear();
            row.Cells[DCC.IndexOf("Approved")].Style["text-align"] = "center";

            Label lblApproved = new Label();
            DateTime nDate = new DateTime();

            lblApproved.Text = row.Cells[DCC.IndexOf("USERNAME")].Text.ToLower();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("ApprovedDate")].Text, out nDate))
            {
                lblApproved.Text += " " + String.Format("{0:M/d/yyyy h:mm tt}", nDate);
            }

            if (row.Cells[DCC.IndexOf("Approved")].Text == "1") lblApproved.Text += "<br />";

            lblApproved.Style["font-size"] = "10px";
            lblApproved.Style["white-space"] = "nowrap";

            row.Cells[DCC.IndexOf("Approved")].Controls.Add(lblApproved);
        }

        if (DCC.Contains("File"))
        {
            row.Cells[DCC.IndexOf("File")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("File")].Controls.Add(CreateLink("File", row.Cells[DCC.IndexOf("AORReleaseAttachment_ID")].Text, row.Cells[DCC.IndexOf("File")].Text));
        }

        if (this.Type == "Previous Attachment" && this.CanEditAOR && DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("AORReleaseAttachment_ID")].Text));
        }

        if ((this.Type == "CR AOR" || this.Type == "Release Schedule AOR") && this.CanEditAOR && DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("AORRelease_ID")].Text));
        }

        if (this.Type == "Add/Move Deployment AOR")
        {
            if (this.CanEditAOR && DCC.Contains("X") && DCC.Contains("Y"))
            {
                row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("AORRelease_ID")].Text, row.Cells[DCC.IndexOf("Deployment_ID")].Text));
                DropDownList ddl = CreateDropDownList("Deployment", row.Cells[DCC.IndexOf("Deployment_ID")].Text, dtDeployment, "Deployment", "Deployment", "ReleaseScheduleID", row.Cells[DCC.IndexOf("Deployment_ID")].Text, row.Cells[DCC.IndexOf("Deployment")].Text);
                if (row.Cells[DCC.IndexOf("Deployment_ID")].Text == "&nbsp;") ddl.SelectedValue = this.DeliverableID.ToString();
                row.Cells[DCC.IndexOf("Y")].Controls.Add(ddl);
            }

            if (DCC.Contains("Deployment"))
            {
                row.Cells[DCC.IndexOf("Deployment")].Text = row.Cells[DCC.IndexOf("Deployment")].Text == "&nbsp;" ? "" : "." + row.Cells[DCC.IndexOf("Deployment")].Text;
            }

            if (DCC.Contains("Weight"))
            {
                row.Cells[DCC.IndexOf("Weight")].Attributes["Weight"] = row.Cells[DCC.IndexOf("Weight")].Text.Replace("&nbsp;", "");
            }
        }

        if ((this.Type == "Contract") && this.CanEditAOR && DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("Z")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("Z")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("ContractID")].Text));
        }

        if ((this.Type == "Action Team") && this.CanEditAOR && DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("WTS_RESOURCE_ID")].Text));
        }

        if ((this.Type == "Deployment") && this.CanEditAOR)
        {
            if (DCC.Contains("X"))
            {
                row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("Deployment_ID")].Text));
            }

            if (DCC.Contains("Deployment"))
            {
                row.Cells[DCC.IndexOf("Deployment")].Controls.Add(CreateLink("Deployment", row.Cells[DCC.IndexOf("Deployment")].Text, row.Cells[DCC.IndexOf("Deployment_ID")].Text));
            }

            if (DCC.Contains("Planned Start"))
            {
                row.Cells[DCC.IndexOf("Planned Start")].Style["text-align"] = "center";
            }

            if (DCC.Contains("Planned End"))
            {
                row.Cells[DCC.IndexOf("Planned End")].Style["text-align"] = "center";
            }
        }

        if (this.CanViewAOR && DCC.Contains("AOR #") && DCC.Contains("AORRelease_ID"))
        {
            row.Cells[DCC.IndexOf("AOR #")].Controls.Add(CreateLink("AOR", row.Cells[DCC.IndexOf("AOR #")].Text, row.Cells[DCC.IndexOf("AORRelease_ID")].Text));
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
            if (this.Type == "Contract") row.Cells[i].Style["display"] = "none";
        }

        if (this.Type != "CR AOR" && this.Type != "Release Schedule AOR" && this.Type != "Add/Move Deployment AOR" && DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["display"] = "none";
        if (this.Type == "CR AOR" && DCC.Contains("System")) row.Cells[DCC.IndexOf("System")].Style["display"] = "none";

        if ((this.Type == "Add/Move Deployment AOR" || this.Type == "Release Schedule AOR") && DCC.Contains("System")) row.Cells[DCC.IndexOf("System")].Style["display"] = "none";
        if (this.Type == "Add/Move Deployment AOR" && DCC.Contains("Weight")) row.Cells[DCC.IndexOf("Weight")].Style["display"] = "none";

        if (this.Type == "Contract" && DCC.Contains("Contract"))
        {
            row.Cells[DCC.IndexOf("Z")].Style["display"] = "";
            row.Cells[DCC.IndexOf("Z")].Style["width"] = "35px";
            row.Cells[DCC.IndexOf("Contract")].Style["display"] = "";
        }

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = "35px";
        }

        if (DCC.Contains("Y"))
        {
            row.Cells[DCC.IndexOf("Y")].Text = "Add/Move To";
            row.Cells[DCC.IndexOf("Y")].Style["width"] = "155px";
        }

        if (DCC.Contains("Release")) row.Cells[DCC.IndexOf("Release")].Style["width"] = "75px";
        if (DCC.Contains("Release Status")) row.Cells[DCC.IndexOf("Release Status")].Style["width"] = "150px";
        if (DCC.Contains("Date")) row.Cells[DCC.IndexOf("Date")].Style["width"] = "115px";
        if (DCC.Contains("Work Task")) row.Cells[DCC.IndexOf("Work Task")].Style["width"] = "100px";
        if (DCC.Contains("Field Changed")) row.Cells[DCC.IndexOf("Field Changed")].Style["width"] = "115px";
        if (DCC.Contains("Change Date")) row.Cells[DCC.IndexOf("Change Date")].Style["width"] = "115px";
        if (DCC.Contains("Old Value")) row.Cells[DCC.IndexOf("Old Value")].Style["width"] = "265px";
        if (DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "45px";
        if (DCC.Contains("Added By")) row.Cells[DCC.IndexOf("Added By")].Style["width"] = "150px";
        if (DCC.Contains("Added By")) row.Cells[DCC.IndexOf("Added By")].Text = "Added/Updated";
        if (DCC.Contains("Added By")) row.Cells[DCC.IndexOf("Added By")].Style["text-align"] = "left";
        if (DCC.Contains("USERNAME")) row.Cells[DCC.IndexOf("USERNAME")].Style["display"] = "none";
        if (DCC.Contains("ApprovedByID")) row.Cells[DCC.IndexOf("ApprovedByID")].Style["display"] = "none";
        if (DCC.Contains("ApprovedDate")) row.Cells[DCC.IndexOf("ApprovedDate")].Style["display"] = "none";
        if (DCC.Contains("Added Date")) row.Cells[DCC.IndexOf("Added Date")].Style["display"] = "none";
        if (DCC.Contains("Updated By")) row.Cells[DCC.IndexOf("Updated By")].Style["display"] = "none";
        if (DCC.Contains("Updated Date")) row.Cells[DCC.IndexOf("Updated Date")].Style["display"] = "none";

        if (DCC.Contains("Z")) row.Cells[DCC.IndexOf("Z")].Text = "";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
            if (this.Type == "Contract") row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }

        if (this.Type != "CR AOR" && this.Type != "Release Schedule AOR" && this.Type != "Add/Move Deployment AOR" && DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["display"] = "none";
        if (this.Type == "CR AOR" && DCC.Contains("System")) row.Cells[DCC.IndexOf("System")].Style["display"] = "none";

        if ((this.Type == "Add/Move Deployment AOR" || this.Type == "Release Schedule AOR") && DCC.Contains("System")) row.Cells[DCC.IndexOf("System")].Style["display"] = "none";
        if (this.Type == "Add/Move Deployment AOR" && DCC.Contains("Weight")) row.Cells[DCC.IndexOf("Weight")].Style["display"] = "none";

        if (this.Type == "Contract" && DCC.Contains("Contract") && row.RowIndex > 0)
        {
            row.Cells[DCC.IndexOf("Z")].Style["display"] = "";
            row.Cells[DCC.IndexOf("Contract")].Style["display"] = "";
            row.Cells[DCC.IndexOf("X")].Style["display"] = "";
        }

        if (DCC.Contains("Date")) row.Cells[DCC.IndexOf("Date")].Style["text-align"] = "center";
        if (DCC.Contains("Work Task")) row.Cells[DCC.IndexOf("Work Task")].Style["text-align"] = "center";
        if (DCC.Contains("Change Date")) row.Cells[DCC.IndexOf("Change Date")].Style["text-align"] = "center";
        if (DCC.Contains("Release")) row.Cells[DCC.IndexOf("Release")].Style["text-align"] = "center";
        if (DCC.Contains("USERNAME")) row.Cells[DCC.IndexOf("USERNAME")].Style["display"] = "none";
        if (DCC.Contains("ApprovedByID")) row.Cells[DCC.IndexOf("ApprovedByID")].Style["display"] = "none";
        if (DCC.Contains("ApprovedDate")) row.Cells[DCC.IndexOf("ApprovedDate")].Style["display"] = "none";
        if (DCC.Contains("Added Date")) row.Cells[DCC.IndexOf("Added Date")].Style["display"] = "none";
        if (DCC.Contains("Updated By")) row.Cells[DCC.IndexOf("Updated By")].Style["display"] = "none";
        if (DCC.Contains("Updated Date")) row.Cells[DCC.IndexOf("Updated Date")].Style["display"] = "none";
    }

    private void CreateRow(GridViewRow row)
    {
        Table nTable = (Table)row.Parent;
        GridViewRow nRow = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
        TableCell nCell = new TableCell();

        nCell.BackColor = System.Drawing.Color.LightGray;
        nCell.ColumnSpan = 6;
        nCell.Text = row.Cells[DCC.IndexOf("System")].Text;

        nRow.Cells.Add(nCell);
        nTable.Rows.AddAt(nTable.Rows.Count - 1, nRow);
    }

    private CheckBox CreateCheckBox(string value, string deliverableID = "0")
    {
        CheckBox chk = new CheckBox();

        chk.Attributes["onchange"] = "input_change(this);";

        switch (this.Type)
        {
            case "Add/Move Deployment AOR":
                chk.Attributes.Add("aorrelease_id", value);
                if (deliverableID != "&nbsp;" && deliverableID != "0") chk.Checked = true;
                break;
            case "Release Schedule AOR":
            case "CR AOR":
                chk.Attributes.Add("aorrelease_id", value);
                break;
            case "Contract":
                chk.Attributes.Add("contract_id", value);
                break;
            case "Action Team":
                chk.Attributes.Add("resource_id", value);
                break;
            case "Previous Attachment":
                chk.Attributes.Add("releaseAttachment_id", value);
                break;
            case "Deployment":
                chk.Attributes.Add("deployment_id", value);
                break;
        }

        return chk;
    }

    private LinkButton CreateTextLink(string txt, int sub)
    {
        LinkButton lb = new LinkButton();

        lb.Text = sub == 0 ? "View" : txt.Substring(0, sub) + "...";

        if (sub != 0) lb.ToolTip = txt;

        lb.Attributes["onclick"] = string.Format("showText('{0}', {1}); return false;", Uri.EscapeDataString(txt), sub == 0 ? 1 : 0);

        return lb;
    }

    private LinkButton CreateLink(string type, string value, string value2)
    {
        LinkButton lb = new LinkButton();

        lb.Text = value;

        switch (type)
        {
            case "AOR":
                lb.Attributes["onclick"] = string.Format("openAOR('{0}', '{1}'); return false;", value, value2);
                break;
            case "Deployment":
                lb.Attributes["onclick"] = string.Format("openDeployment('{0}'); return false;", value2);
                break;
            case "File":
                lb.Text = value2;
                lb.Attributes["onclick"] = string.Format("downloadAORAttachment('{0}'); return false;", value);
                break;
        }

        return lb;
    }

    private DropDownList CreateDropDownList(string typeName, string typeID
        , DataTable dt, string field
        , string textField, string valueField
        , string value, string text = ""
        , List<string> attributes = null)
    {
        DropDownList ddl = new DropDownList();

        textField = Server.HtmlDecode(textField).Trim();
        valueField = Server.HtmlDecode(valueField).Trim();
        value = Server.HtmlDecode(value).Trim();
        text = Server.HtmlDecode(text).Trim();

        ddl.Width = new Unit(95, UnitType.Percentage);
        ddl.Attributes["class"] = "saveable";
        ddl.Attributes["onchange"] = "input_change(this);";
        ddl.Attributes.Add("typeName", typeName);
        ddl.Attributes.Add("typeID", typeID);
        ddl.Attributes.Add("field", field);
        ddl.Attributes.Add("original_value", value);
        ddl.Style["background-color"] = "#F5F6CE";

        if (dt == null)
        {
            return ddl;
        }

        ListItem item = null;
        foreach (DataRow row in dt.Rows)
        {
            item = new ListItem();
            item.Text = row[textField].ToString();
            item.Value = row[valueField].ToString();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (string key in attributes)
                {
                    item.Attributes.Add(key.Trim(), key.Trim());
                }
            }

            if (ddl.Items.FindByValue(item.Value) == null)
            {
                ddl.Items.Add(item);
            }
        }

        WTSUtility.SelectDdlItem(ddl, string.IsNullOrWhiteSpace(value) ? "0" : value, text);

        return ddl;
    }
    #endregion

    #region Excel
    private bool ExportExcel(string name, DataTable dt)
    {
        bool success = false;
        string errorMsg = string.Empty;

        try
        {
            if (dt.Columns.Contains("X")) dt.Columns["X"].ColumnName = " ";
            if (dt.Columns.Contains("AORRelease_ID")) dt.Columns.Remove("AORRelease_ID");
            if (dt.Columns.Contains("WTS_SYSTEM_ID")) dt.Columns.Remove("WTS_SYSTEM_ID");
            dt.AcceptChanges();
                        
            var wb = new Workbook(FileFormatType.Xlsx);
            var ws = wb.Worksheets[0];
            var style1 = ws.Cells[0, 0].GetStyle();
            var style2 = ws.Cells[0, 0].GetStyle();

            foreach (DataRow row in dt.Rows)
            {
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    if (row[i] is string)
                    {
                        row[i] = string.IsNullOrWhiteSpace((string)row[i]) ? row[i] : System.Web.HttpUtility.UrlDecode((string)row[i]);
                    }
                }
            }

            ws.Cells.ImportDataTable(dt, true, 0, 0, false, false);

            style1.ForegroundColor = ColorTranslator.FromHtml("#D9DCF3");
            style1.BackgroundColor = ColorTranslator.FromHtml("#D9DCF3");
            style1.Pattern = BackgroundType.VerticalStripe;
            style1.Font.Color = Color.Black;
            style1.Font.IsBold = true;
            style1.Font.Size = 11;

            style2.ForegroundColor = ColorTranslator.FromHtml("#D3D3D3");
            style2.BackgroundColor = ColorTranslator.FromHtml("#D3D3D3");
            style2.Pattern = BackgroundType.VerticalStripe;
            style2.Font.Color = Color.Black;
            style2.Font.IsBold = true;

            for (int i = 0; i < 7; i++)
                ws.Cells[0, i].SetStyle(style1);

            ws.AutoFitColumns();

            if (ws.Cells.MaxDataRow > 0)
            {
                Range rn = ws.Cells.CreateRange("E1:E" + ws.Cells.MaxDataRow * 2);
                rn.Name = "newRange";

                Range range = wb.Worksheets.GetRangeByName("newRange");
                IEnumerator e = range.GetEnumerator();

                var rowCount = 0;
                var prevCelllValue = "System";

                while (e.MoveNext())
                {
                    var c = (Cell)e.Current;
                    if (c.StringValue != prevCelllValue && !string.IsNullOrEmpty(c.StringValue))
                    {
                        if (c.StringValue.IndexOf("%") != -1)
                        {
                            int x = 5;
                        }
                        ws.Cells.InsertRows(rowCount, 1);
                        ws.Cells[rowCount, 0].Value = c.StringValue;
                        for (int i = 0; i < 7; i++) ws.Cells[rowCount, i].SetStyle(style2);
                    }

                    prevCelllValue = c.StringValue;
                    rowCount++;
                }

                ws.Cells.DeleteColumn(ws.Cells.MaxDataColumn);
            }

            MemoryStream ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Xlsx);

            Response.ContentType = "application/xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
            Response.BinaryWrite(ms.ToArray());
            Response.End();

            success = true;
        }
        catch (Exception ex)
        {
            success = false;
            errorMsg += Environment.NewLine + ex.Message;
            LogUtility.LogException(ex);
        }

        return success;
    }

    #endregion

    #region AJAX
    [WebMethod()]
    public static string AORSearch(string aorName, string AORWorkType)
    {
        DataTable dt = new DataTable();

        try
        {
            dt = AOR.AORList_Get(AORID: 0);
            dt.DefaultView.RowFilter = "[AOR Name] LIKE '%" + aorName.Replace("'", "''") + "%'";
            dt = dt.DefaultView.ToTable();
            dt.Columns["AOR #"].ColumnName = "AORID";
            dt.Columns["AOR Name"].ColumnName = "AORName";
            dt.Columns["AOR Workload Type"].ColumnName = "AORWorkType";

            if (AORWorkType != "") {
                dt.DefaultView.RowFilter = "[AORWorkType] LIKE '%" + AORWorkType + "%'"; 
                dt = dt.DefaultView.ToTable();
            }

            dt.AcceptChanges();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }
    
    [WebMethod()]
    public static string Add(string aor, string sr, string cr, string deliverable, string type, string additions)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            int AOR_ID = 0, SR_ID = 0, CR_ID = 0, Deliverable_ID = 0;

            int.TryParse(aor, out AOR_ID);
            int.TryParse(sr, out SR_ID);
            int.TryParse(cr, out CR_ID);
            int.TryParse(deliverable, out Deliverable_ID);

            XmlDocument docAdditions = (XmlDocument)JsonConvert.DeserializeXmlNode(additions, "additions");

            if (type == "Contract")
            {
                saved = MasterData.DeploymentContract_Add(DeliverableID: Deliverable_ID, Additions: docAdditions);
            }
            else
            {
                saved = AOR.AORAdd_Save(AORID: AOR_ID, SRID: SR_ID, CRID: CR_ID, DeliverableID: Deliverable_ID, Type: type, Additions: docAdditions);
            }

            if (type == "Task" && saved)
            {
                DataTable dtCascade = AOR.AORList_Get(AOR_ID);
                if (dtCascade != null && dtCascade.Rows.Count > 0)
                {
                    bool cascade = false;
                    bool.TryParse(dtCascade.Rows[0]["CascadeAOR"].ToString(), out cascade);

                    XmlNodeList xmlTaskIds = docAdditions.SelectNodes("/additions/save/taskid");
                    foreach (XmlNode node in xmlTaskIds)
                    {
                        int taskid = 0;
                        int.TryParse(node.InnerText, out taskid);
                    
                        DataTable dtSubTask = WorkloadItem.WorkItem_GetTaskList(workItemID: taskid, showArchived: 0, showBacklog: false);
                        if (dtSubTask != null && dtSubTask.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtSubTask.Rows)
                            {
                                int subtaskID = 0;
                                int.TryParse(row["WORKITEM_TASKID"].ToString(), out subtaskID);
                                string aors = "{save:[{\"aorreleaseid\":\"" + dtCascade.Rows[0]["AORRelease_ID"].ToString() + "\"}]}";
                                XmlDocument docAORs = (XmlDocument)JsonConvert.DeserializeXmlNode(aors, "aors");
                                bool savedAORTask = AOR.AORSubTask_Save(TaskID: subtaskID, AORs: docAORs, Add: 1, CascadeAOR: cascade);
                            }
                        }
                    }
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

    [WebMethod(true)]
    public static string SetFilterSession(dynamic filters)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;
        
        string sessionid = HttpContext.Current.Session.SessionID;
        try
        {
            if (filters != null)
            {
                HttpContext.Current.Session["filters_AorWork"] = JsonConvert.DeserializeObject<Dictionary<string, object>>(filters);
            }
            else
            {
                HttpContext.Current.Session["filters_AorWork"] = null;
            }

            saved = true;
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