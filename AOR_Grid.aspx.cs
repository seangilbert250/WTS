﻿using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using TextBox = System.Web.UI.WebControls.TextBox;

public class FilterCols
{
    public string ColName { get; set; }
    public string Filter { get; set; }
    public FilterCols(string colName, string filter)
    {
        ColName = colName;
        Filter = filter;
    }
}

public partial class AOR_Grid : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool BlnArchivedAOR = false;
    protected int CurrentLevel = 1;
    protected int GridPageSize = 25;
    protected string Filter = string.Empty;
    protected bool IsConfigured = false;
    private XmlDocument Levels = new XmlDocument();
    protected DataColumnCollection DCC;
    protected int LevelCount = 0;
    protected int GridPageIndex = 0;
    private static int DEFAULT_MGMT_VIEW = 285;
    public int ddlGridViewValue = DEFAULT_MGMT_VIEW;
    protected DataTable dtCopy;
    protected List<string> ColNamesCopy;
    protected List<string> SubGridFilterCols;
    protected int StartCol = 0;
    protected int ColSpan = 0;
    protected static string[] QFOPT_NAMEARR = new string[] { "QFRelease", "QFAORType", "QFVisibleToCustomer", "QFContainsTasks", "QFContract", "QFTaskStatus", "QFAORProductionStatus", "QFShowArchiveAOR" };
    protected Dictionary<string, string[]> QFOPTSettings_Dict = QFOPT_NAMEARR.ToDictionary(item => item, item => new string[] { });
    protected bool _export = false;
    protected int rowCount = 0;
    protected int dtRowCount = 0;
    protected int dtColumnCnt = 0;
    protected int uniA = (int)'A';
    protected bool hasDoubleHeader = false;
    protected int gridRowCnt = 0;
    protected DataSet dsLookup;
    protected string GridViewNameOverride = "";
    protected string GridSessionKey = "itisettings";
    protected bool ReadOnly = false; // if true, this overrides normal settings to provide a read-only view
    protected bool ShowHeaderRow = true; // by default, we show the header row with buttons, but this can be turned off
    protected string[] AORID_Filter_arr = { };

    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        //Load the session variables if there are any.
        SetSessionForFirstPageLoad();
        SetupPageFromQryStrAndSess();
        CreateCrosswalkDataPullFromSess();
        InitializeEvents();
        SetQFFromSess();
        LoadRelatedItemsMenu();
        LoadLookupData();

        CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem) && !ReadOnly;
        CanViewWorkItem = CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);
        CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR) && !ReadOnly;
        CanViewAOR = CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);
        HtmlTableRow trItem5 = (HtmlTableRow)Page.Master.FindControl("trItem5");
        DropDownList ddlGridPageSize = (DropDownList)Page.Master.FindControl("ddlItem5");
        if (this.CurrentLevel > 1)
        {
            grdData.PageSize = 12;

            if (this.GridPageSize == 0)
            {
                grdData.AllowPaging = false;
                ddlGridPageSize.SelectedValue = this.GridPageSize.ToString();
            }
            else
            {
                grdData.AllowPaging = true;
                grdData.PageSize = this.GridPageSize;
                ddlGridPageSize.SelectedValue = this.GridPageSize.ToString();
            }
        }

        if (!ShowHeaderRow)
        {
            HtmlControl ctrl = (HtmlControl)Page.Master.FindControl("pageContentHeader");
            if (ctrl != null)
            {
                ctrl.Style.Add("display", "none");
            }

            ctrl = (HtmlControl)Page.Master.FindControl("pageContentInfo");
            if (ctrl != null)
            {
                ctrl.Style.Add("display", "none");
            }
        }

        DataTable dt = LoadCrosswalkData();
        gridRowCnt = dt.Rows.Count;
        if (dt != null) DCC = dt.Columns;
        if (_export)
        {
            ExportExcel(dt);
        }
        else
        {
            grdData.DataSource = dt;

            if (!Page.IsPostBack && GridPageIndex > 0 && GridPageIndex < ((decimal)dt.Rows.Count / (decimal)grdData.PageSize))
            {
                grdData.PageIndex = GridPageIndex;
            }

            if (!Page.IsPostBack)
            {
                grdData.DataBind();
            }
        }

        hdnGridSessionKey.Value = GridSessionKey;
    }

    protected void grdData_OnGridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        dynamic itiSettings_all_sess = JsonConvert.DeserializeObject(Session[GridSessionKey].ToString());

        if (CurrentLevel == 1)
        {
            foreach (var currTblCol in itiSettings_all_sess.tblCols)
            {
                if (currTblCol.concat == true)
                    foreach (var i in currTblCol.catcols)
                        e.Row.Cells.Cast<DataControlFieldCell>()
                            .Where(c => c.ContainingField.HeaderText == i.Value)
                            .ToList()
                            .ForEach(c => c.ContainingField.Visible = false);

                if (currTblCol.columnwidth == 0)
                    e.Row.Cells.Cast<DataControlFieldCell>()
                        .Where(c => c.ContainingField.HeaderText == (!String.IsNullOrEmpty(currTblCol.alias.ToString()) ? currTblCol.alias.ToString() : currTblCol.name.ToString()))
                        .ToList()
                        .ForEach(c => c.ContainingField.Visible = false);
            }
        }
        else
        {
            var subgridLevel = 2;
            foreach (var itiSettings_subgrid_sess in itiSettings_all_sess)
                if (itiSettings_subgrid_sess.Name.IndexOf("subgrid") != -1)
                {
                    if (subgridLevel == CurrentLevel)
                        foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
                            foreach (var currTblCol_sess in itiSettings_subgrid_prop_sess[0].tblCols)
                            {
                                if (currTblCol_sess.concat == true)
                                    foreach (var i in currTblCol_sess.catcols)
                                        e.Row.Cells.Cast<DataControlFieldCell>()
                                            .Where(c => c.ContainingField.HeaderText == i.Value)
                                            .ToList()
                                            .ForEach(c => c.ContainingField.Visible = false);

                                if (currTblCol_sess.columnwidth == 0)
                                    e.Row.Cells.Cast<DataControlFieldCell>()
                                        .Where(c => c.ContainingField.HeaderText == (!String.IsNullOrEmpty(currTblCol_sess.alias.ToString()) ? currTblCol_sess.alias.ToString() : currTblCol_sess.name.ToString()))
                                        .ToList()
                                        .ForEach(c => c.ContainingField.Visible = false);
                            }

                    subgridLevel++;
                }
        }
    }

    protected void grdData_OnGridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        var row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
        dynamic itiSettings_all_sess = JsonConvert.DeserializeObject(Session[GridSessionKey].ToString());
        var cell = new TableHeaderCell();
        var groupName = "";
        StartCol = 0;
        ColSpan = 0;
        if (DCC.Contains("Y")) StartCol = 1;


        if (CurrentLevel == 1)
        {
            foreach (DataColumn currTblCol_DB in dtCopy.Columns)
                if (currTblCol_DB.ColumnName != "X" && currTblCol_DB.ColumnName != "Y" && currTblCol_DB.ColumnName != "Z" && currTblCol_DB.ColumnName.IndexOf("_ID", StringComparison.Ordinal) == -1)
                    foreach (var currTblCol_sess in itiSettings_all_sess.tblCols)
                        if ((currTblCol_DB.ColumnName == currTblCol_sess.name.ToString() || currTblCol_DB.ColumnName == currTblCol_sess.alias.ToString()) && currTblCol_sess.columnwidth != 0)
                        {
                            if (string.IsNullOrEmpty(groupName)) StartCol++;
                            if (!string.IsNullOrEmpty(currTblCol_sess.groupname.ToString()))
                            {
                                groupName = currTblCol_sess.groupname;
                                break;
                            }
                        }

            foreach (var currTblCol_sess in itiSettings_all_sess.tblCols)
                if (currTblCol_sess.groupname != "") ColSpan++;

            if (itiSettings_all_sess.showcolumnheader == false)
                for (int i = 0; i < e.Row.Cells.Count; i++)
                    e.Row.Cells[i].Style["display"] = "none";
        }
        else
        {
            var subgridLevel = 2;
            foreach (var itiSettings_subgrid_sess in itiSettings_all_sess)
                if (itiSettings_subgrid_sess.Name.IndexOf("subgrid") != -1)
                {
                    if (subgridLevel == CurrentLevel)
                    {
                        foreach (DataColumn currTblCol_DB in dtCopy.Columns)
                            if (currTblCol_DB.ColumnName != "X" && currTblCol_DB.ColumnName != "Y" && currTblCol_DB.ColumnName != "Z" && currTblCol_DB.ColumnName.IndexOf("_ID", StringComparison.Ordinal) == -1)
                                foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
                                    foreach (var currTblCol_sess in itiSettings_subgrid_prop_sess[0].tblCols)
                                        if ((currTblCol_DB.ColumnName == currTblCol_sess.name.ToString() || currTblCol_DB.ColumnName == currTblCol_sess.alias.ToString()) && currTblCol_sess.columnwidth != 0)
                                        {
                                            if (string.IsNullOrEmpty(groupName)) StartCol++;
                                            if (!string.IsNullOrEmpty(currTblCol_sess.groupname.ToString()))
                                            {
                                                groupName = currTblCol_sess.groupname;
                                                break;
                                            }
                                        }
                    }
                    subgridLevel++;
                }

            subgridLevel = 2;
            foreach (var itiSettings_subgrid_sess in itiSettings_all_sess)
                if (itiSettings_subgrid_sess.Name.IndexOf("subgrid") != -1)
                {
                    if (subgridLevel == CurrentLevel)
                        foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
                            foreach (var currTblCol_sess in itiSettings_subgrid_prop_sess[0].tblCols)
                                if (currTblCol_sess.groupname != "") ColSpan++;

                    subgridLevel++;
                }
        }

        if (groupName != "")
        {
            cell = new TableHeaderCell();
            cell.Text = "";
            cell.ColumnSpan = StartCol - 1;
            row.Controls.Add(cell);

            cell = new TableHeaderCell();
            cell.Style.Add("font-weight", "bold");
            cell.Style.Add("border-bottom", "solid");
            cell.Text = groupName;
            cell.ColumnSpan = ColSpan;
            row.Controls.Add(cell);
            row.Controls.Add(cell);
            e.Row.Parent.Controls.AddAt(0, row);

            hasDoubleHeader = true;
        }

    }

    /// <summary>
    /// If this page is called from:
    /// AOR_Maintenance_Container:
    ///     This will set our session on the first load to the DB. Otherwise it will not set the session.
    /// AOR_Meeting_Instance_Edit:
    /// </summary>
    private void SetSessionForFirstPageLoad()
    {
        // note, some of the fields set in this function need query string values that haven't been stored off yet, so we read directly from the query string rather than the class' private variables
        if (!string.IsNullOrWhiteSpace(Request.QueryString["GridViewNameOverride"]))
        {
            GridViewNameOverride = Request.QueryString["GridViewNameOverride"];

            if (GridViewNameOverride == "MeetingNoteAORGrid")
            {
                ReadOnly = true;

                int cl = Request.QueryString["CurrentLevel"] != null ? Int32.Parse(Request.QueryString["CurrentLevel"]) : CurrentLevel;
                if (cl < 3)
                {
                    ShowHeaderRow = false;
                }
                menuRelatedItems.Visible = false;

                GridSessionKey = "MeetingNoteAORGridSessionKey";

                GridView gv = new GridView();
                gv.ViewName = "MeetingNoteAORGrid";
                gv.Load();

                if (!string.IsNullOrWhiteSpace(gv.Tier1Columns))
                {
                    Session[GridSessionKey] = gv.Tier1Columns;
                }
                else
                {
                    return;
                }
            }
        }
        else
        {

            if (string.IsNullOrEmpty((Session[GridSessionKey] ?? "").ToString()))
            { //If we are loading for the first time the AOR_Grid by itself (and not in another page), then we need to grab the json settings for the default view from the DB

                var items = WTSData.GetViewOptions(UserManagement.GetUserId_FromUsername(), "AOR");
                var default_gridViewId = GetDefaultGridViewId(11, UserManagement.GetUserId_FromUsername());
                Session[GridSessionKey] = (from DataRow dr in items.Rows
                                           where (int)dr["GridViewID"] == default_gridViewId
                                           select (string)dr["Tier1Columns"]).FirstOrDefault();

                //Breakdown the session variables in order to load thr form

                setSessionQF(Session[GridSessionKey].ToString());

                dynamic itisettings_all_sess = JsonConvert.DeserializeObject(Session[GridSessionKey].ToString());
                foreach (string currQFOPTName in QFOPT_NAMEARR)
                {
                    if (!string.IsNullOrWhiteSpace((itisettings_all_sess[currQFOPTName] ?? "").ToString()))
                    {
                        QFOPTSettings_Dict[currQFOPTName] = itisettings_all_sess[currQFOPTName].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }
            }

        }
    }


    public static void setSessionQF(string itisettings_str)
    {
        //dynamic itisettings_json = JsonConvert.DeserializeObject(itisettings_str);
        var itisettings_dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(itisettings_str);
        var sessionMethods = new SessionMethods();

        sessionMethods.Session["QFRelease"] = "";
        sessionMethods.Session["QFAORType"] = "";
        sessionMethods.Session["QFVisibleToCustomer"] = "";
        sessionMethods.Session["QFContainsTasks"] = "";
        sessionMethods.Session["QFContract"] = "";
        sessionMethods.Session["QFTaskStatus"] = "";
        sessionMethods.Session["QFAORProductionStatus"] = "";
        sessionMethods.Session["QFShowArchiveAOR"] = "";

        foreach (string currQFOPTName in QFOPT_NAMEARR)
        {
            if (itisettings_dict.ContainsKey(currQFOPTName))
            {
                sessionMethods.Session[currQFOPTName] = itisettings_dict[currQFOPTName] ?? "";
            }
        }

    }

    /// <summary>
    /// This will read from the current session and then make XML for our crosswalk data pull to be displayed in the AOR Grid
    /// </summary>
    private void CreateCrosswalkDataPullFromSess()
    {
        var nDoc = new XmlDocument();
        var test = "<crosswalkparameters><level>";

        //if (itisettings.Value == "" && Session[GridSessionKey].ToString() == "")
        //{
        //    var items = WTSData.GetViewOptions(UserManagement.GetUserId_FromUsername(), "AOR");
        //    var gridViewId = GetDefaultGridViewId(11, UserManagement.GetUserId_FromUsername());

        //    Session[GridSessionKey] = (from DataRow dr in items.Rows
        //                               where (int)dr["GridViewID"] == gridViewId
        //                               select (string)dr["Tier1Columns"]).FirstOrDefault();
        //}

        dynamic itisettings_all_sess = JsonConvert.DeserializeObject(Session[GridSessionKey].ToString());
        foreach (var currTblCol_sess in itisettings_all_sess.tblCols)
        {
            var sort = currTblCol_sess.sortorder;
            if (sort == "none") sort = "Ascending";
            if (currTblCol_sess.name == "AOR Customer Flagship") currTblCol_sess.name = "Customer Flagship";
            if (currTblCol_sess.concat == false)
                if (currTblCol_sess.show == true) test += "<breakout><column>" + currTblCol_sess.name + "</column><sort>" + sort + "</sort></breakout>";
        }

        test += "</level>";
        foreach (var itiSettings_subgrid_sess in itisettings_all_sess)
            if (itiSettings_subgrid_sess.ToString().IndexOf("subgrid") > 0)
                foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
                {
                    test += "<level>";
                    foreach (var currTblCol_sess in itiSettings_subgrid_prop_sess[0].tblCols)
                    {
                        var sort = currTblCol_sess.sortorder;
                        if (sort == "none") sort = "Ascending";
                        if (currTblCol_sess.name == "AOR Customer Flagship") currTblCol_sess.name = "Customer Flagship";
                        if (currTblCol_sess.concat == false)
                            if (currTblCol_sess.show == true) test += "<breakout><column>" + currTblCol_sess.name + "</column><sort>" + sort + "</sort></breakout>";
                    }
                    test += "</level>";
                }

        test += "</crosswalkparameters>";
        nDoc.InnerXml = test;
        Session["AORLevels"] = nDoc;

        if (Session["AORLevels"] != null)
        {
            Levels = (XmlDocument)Session["AORLevels"];
        }
        LevelCount = Levels.SelectNodes("crosswalkparameters/level").Count;
        if (LevelCount >= 1) IsConfigured = true;
    }

    /// <summary>
    /// This will reset our session and other page settings according to the URL parameters if there are any
    /// </summary>
    private void SetupPageFromQryStrAndSess()
    {
        if (string.IsNullOrWhiteSpace(Request.QueryString["MyData"]) || Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
            MyData = true;
        else
            MyData = false;

        if (!string.IsNullOrWhiteSpace(Request.QueryString["ReadOnly"]))
        {
            ReadOnly = Request.QueryString["ReadOnly"].ToLower() == "true";
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["ShowHeaderRow"]))
        {
            ShowHeaderRow = Request.QueryString["ShowHeaderRow"].ToLower() == "true";
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"]))
            int.TryParse(Request.QueryString["CurrentLevel"], out CurrentLevel);

        if (!string.IsNullOrWhiteSpace(Request.QueryString["Filter"]))
            Filter = Uri.UnescapeDataString(Request.QueryString["Filter"]);

        if (!string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndex"]))
            int.TryParse(Request.QueryString["GridPageIndex"], out GridPageIndex);


        if (GridSessionKey == "itisettings")
        {//If this page is being loaded from its own page
            setQFsFromSession();
        }
        else
        {//If this page is being loaded from another page
            setQFsFromURL();
        }

        if (Request.QueryString["GridPageSize"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageSize"]))
            int.TryParse(Request.QueryString["GridPageSize"], out this.GridPageSize);

        if (Request.QueryString["Export"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);
    }


    private void setQFsFromSession()
    {
        foreach (string currQFOPTName in QFOPT_NAMEARR)
        {
            if (!string.IsNullOrWhiteSpace((Session[currQFOPTName] ?? "").ToString()))
            {
                QFOPTSettings_Dict[currQFOPTName] = Session[currQFOPTName].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
    private void setQFsFromURL()
    {
        //Since we are loading the AOR_Grid into another page we are using a AORID_Filter_arr and not our QFs to filter
        foreach (string currQFOPTName in QFOPT_NAMEARR)
        {
            QFOPTSettings_Dict[currQFOPTName] = "".ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["AORID_Filter_arr"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID_Filter_arr"]))
        {
            AORID_Filter_arr = Request.QueryString["AORID_Filter_arr"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
    }
    private void InitializeEvents()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;
    }
    #endregion

    #region Data

    /// <summary>
    /// This will setup our QF with the data in our session/global variables
    /// </summary>
    private void SetQFFromSess()
    {
        DataTable dtRelease = MasterData.ProductVersionList_Get(includeArchive: false);
        DataTable dtAORType = AOR.AORWorkTypeList_Get();

        DataRow nDr = dtAORType.NewRow();
        nDr["AORWorkType_ID"] = "0";
        nDr["Work Type"] = "";
        dtAORType.Rows.InsertAt(nDr, 0);

        DataSet dsOptions = AOR.AOROptionsList_Get(AORID: 0, TaskID: 0, AORMeetingID: 0, AORMeetingInstanceID: 0);
        DataTable dtCurrentRelease = AOR.AORCurrentRelease_Get();
        DataTable dtCrContract = dsOptions.Tables["System Task Contract"];
        DataTable dtWorkloadAllocation = dsOptions.Tables["AOR Production Status"];
        DataTable dtStatus = MasterData.StatusList_Get();

        //These variables correspond to the quick filters
        Label ReleaseLabelCtrl = (Label)Page.Master.FindControl("lblms_Item0");
        Label ContractLabelCtrl = (Label)Page.Master.FindControl("lblms_Item1");
        Label WorkTaskStatusLabelCtrl = (Label)Page.Master.FindControl("lblms_Item9");
        Label WorkloadAllocationLabelCtrl = (Label)Page.Master.FindControl("lblms_Item9a");
        Label AORTypeLabelCtrl = (Label)Page.Master.FindControl("lblms_Item10");
        Label VisibleToCustomerLabelCtrl = (Label)Page.Master.FindControl("lblms_Item10a");
        Label ContainsTasksLabelCtrl = (Label)Page.Master.FindControl("lblms_Item2");
        Label ArchiveCheckBoxLabelCtrl = (Label)Page.Master.FindControl("lblms_Item11");

        HtmlSelect ReleaseSelectCtrl = (HtmlSelect)Page.Master.FindControl("ms_Item0");
        HtmlSelect ContractSelectCtrl = (HtmlSelect)Page.Master.FindControl("ms_Item1");
        HtmlSelect WorkTaskStatusSelectCtrl = (HtmlSelect)Page.Master.FindControl("ms_Item9");
        HtmlSelect WorkloadAllocationSelectCtrl = (HtmlSelect)Page.Master.FindControl("ms_Item9a");
        HtmlSelect AORTypeSelectCtrl = (HtmlSelect)Page.Master.FindControl("ms_Item10");
        HtmlSelect VisibleToCustomerSelectCtrl  = (HtmlSelect)Page.Master.FindControl("ms_Item10a");
        HtmlSelect ContainsTasksSelectCtrl  = (HtmlSelect)Page.Master.FindControl("ms_Item2");
        HtmlInputCheckBox ArchiveCheckBoxCtrl = (HtmlInputCheckBox)Page.Master.FindControl("chk_Item11");

        if (QFOPTSettings_Dict["QFShowArchiveAOR"].Count() > 0)
        {
            ArchiveCheckBoxCtrl.Checked = QFOPTSettings_Dict["QFShowArchiveAOR"][0] == "True" ? true : false;
        }

        ArchiveCheckBoxLabelCtrl.Text = "Show Archived AORs: ";
        ArchiveCheckBoxLabelCtrl.Style["width"] = "100px";

        ContractLabelCtrl.Text = "Contract: ";
        ContractLabelCtrl.Style["width"] = "100px";
        ContractSelectCtrl.Items.Clear();
        //Get the QF options from the database and Clear/Add/Preselect all the QF options according to the deaultOptions
        foreach (DataRow dr in dtCrContract.Rows)
        {
            ListItem liContract = new ListItem(dr["Text"].ToString(), dr["Value"].ToString());
            liContract.Selected = QFOPTSettings_Dict["QFContract"].Count() == 0 || QFOPTSettings_Dict["QFContract"].Contains(dr["Value"].ToString());
            ContractSelectCtrl.Items.Add(liContract);
        }

        WorkloadAllocationLabelCtrl.Text = "Workload Allocation: ";
        WorkloadAllocationLabelCtrl.Style["width"] = "100px";
        WorkloadAllocationSelectCtrl.Items.Clear();
        foreach (DataRow dr in dtWorkloadAllocation.Rows)
        {
            ListItem liWorkloadAllocation = new ListItem(dr["Text"].ToString(), dr["Value"].ToString());
            liWorkloadAllocation.Selected = QFOPTSettings_Dict["QFAORProductionStatus"].Count() == 0 || QFOPTSettings_Dict["QFAORProductionStatus"].Contains(dr["Value"].ToString());
            WorkloadAllocationSelectCtrl.Items.Add(liWorkloadAllocation);
        }

        //Get the current release
        string currentReleaseID = "0";
        if (dtCurrentRelease != null && dtCurrentRelease.Rows.Count > 0)
            currentReleaseID = dtCurrentRelease.Rows[0]["ProductVersionID"].ToString();

        if (dtRelease != null)
        {
            ReleaseLabelCtrl.Text = "Release: ";
            ReleaseLabelCtrl.Style["width"] = "100px";
            ReleaseSelectCtrl.Items.Clear();

            foreach (DataRow dr in dtRelease.Rows)
            {
                //This will add and and select any items found
                ListItem li = new ListItem(dr["ProductVersion"].ToString(), dr["ProductVersionID"].ToString());
                li.Selected = (QFOPTSettings_Dict["QFRelease"].Count() == 0 || QFOPTSettings_Dict["QFRelease"].Contains(dr["ProductVersionID"].ToString()));

                if (QFOPTSettings_Dict["QFRelease"].Count() == 0)
                {
                    if (dr["ProductVersionID"].ToString() == currentReleaseID)
                        li.Selected = true;
                    else
                        li.Selected = false;
                }

                ReleaseSelectCtrl.Items.Add(li);
            }
        }

        if (dtAORType != null)
        {
            AORTypeLabelCtrl.Text = "AOR Workload Type: ";
            AORTypeLabelCtrl.Style["width"] = "100px";
            AORTypeSelectCtrl.Items.Clear();

            foreach (DataRow dr in dtAORType.Rows)
            {
                ListItem li = new ListItem(dr["Work Type"].ToString(), dr["AORWorkType_ID"].ToString());
                //This will add and and select any items found
                if (QFOPTSettings_Dict["QFAORType"].Count() == 0)
                {
                    li.Selected = true;
                }
                else
                {
                    li.Selected = (QFOPTSettings_Dict["QFAORType"].Count() == 0 || QFOPTSettings_Dict["QFAORType"].Contains(dr["AORWorkType_ID"].ToString()));
                }

                AORTypeSelectCtrl.Items.Add(li);
            }
        }


        VisibleToCustomerLabelCtrl.Text = "Visible To Customer: ";
        VisibleToCustomerLabelCtrl.Style["width"] = "100px";
        VisibleToCustomerSelectCtrl.Items.Clear();

        ListItem nLi = new ListItem("Yes", "1");

        if (QFOPTSettings_Dict["QFVisibleToCustomer"].Count() == 0)
        {
            nLi.Selected = true;
        }
        else
        {
            nLi.Selected = (QFOPTSettings_Dict["QFVisibleToCustomer"].Count() == 0 || QFOPTSettings_Dict["QFVisibleToCustomer"].Contains("1"));
        }

        VisibleToCustomerSelectCtrl.Items.Add(nLi);

        nLi = new ListItem("No", "0");

        if (QFOPTSettings_Dict["QFVisibleToCustomer"].Count() == 0)
        {
            nLi.Selected = true;
        }
        else
        {
            nLi.Selected = (QFOPTSettings_Dict["QFVisibleToCustomer"].Count() == 0 || QFOPTSettings_Dict["QFVisibleToCustomer"].Contains("0"));
        }

        VisibleToCustomerSelectCtrl.Items.Add(nLi);

        ContainsTasksLabelCtrl.Text = "Contains Tasks: ";
        ContainsTasksLabelCtrl.Style["width"] = "100px";
        ContainsTasksSelectCtrl.Items.Clear();

        nLi = new ListItem("Yes", "1");

        if (QFOPTSettings_Dict["QFVisibleToCustomer"].Count() == 0)
        {
            nLi.Selected = true;
        }
        else
        {
            nLi.Selected = (QFOPTSettings_Dict["QFContainsTasks"].Count() == 0 || QFOPTSettings_Dict["QFContainsTasks"].Contains("1"));
        }

        ContainsTasksSelectCtrl.Items.Add(nLi);

        nLi = new ListItem("No", "0");

        if (QFOPTSettings_Dict["QFContainsTasks"].Count() == 0)
        {
            nLi.Selected = true;
        }
        else
        {
            nLi.Selected = (QFOPTSettings_Dict["QFContainsTasks"].Count() == 0 || QFOPTSettings_Dict["QFContainsTasks"].Contains("0"));
        }

        ContainsTasksSelectCtrl.Items.Add(nLi);


        WorkTaskStatusLabelCtrl.Text = "Work Task Status: ";
        WorkTaskStatusLabelCtrl.Style["width"] = "100px";

        if (dtStatus != null)
        {
            WorkTaskStatusSelectCtrl.Items.Clear();

            dtStatus.DefaultView.RowFilter = "StatusType = 'Work'";
            dtStatus = dtStatus.DefaultView.ToTable(true, new string[] { "StatusID", "Status" });

            foreach (DataRow dr in dtStatus.Rows)
            {
                ListItem li = new ListItem(dr["Status"].ToString(), dr["StatusID"].ToString());

                if (QFOPTSettings_Dict["QFTaskStatus"].Count() == 0)
                {
                    if (li.Text != "Closed" && li.Text != "On Hold" && li.Text != "Approved/Closed") li.Selected = true;
                }
                else
                    li.Selected = QFOPTSettings_Dict["QFTaskStatus"].Count() == 0 || QFOPTSettings_Dict["QFTaskStatus"].Contains(dr["StatusID"].ToString());

                WorkTaskStatusSelectCtrl.Items.Add(li);
            }
        }

        Label lblPageSize = (Label)Page.Master.FindControl("lblItem5");
        lblPageSize.Text = "Page Size: ";
        lblPageSize.Style["width"] = "50px";
        DropDownList ddlGridPageSize = (DropDownList)Page.Master.FindControl("ddlItem5");
        ddlGridPageSize.Items.Clear();
        ListItem item = new ListItem("12", "12");
        ddlGridPageSize.Items.Add(item);
        item = new ListItem("25", "25");
        ddlGridPageSize.Items.Add(item);
        item = new ListItem("50", "50");
        ddlGridPageSize.Items.Add(item);
        item = new ListItem("All", "0");
        ddlGridPageSize.Items.Add(item);
        ddlGridPageSize.SelectedValue = "12";

        if (CurrentLevel != 1) tblRelease.Style["display"] = "none";

    }

    private void LoadLookupData()
    {
        DataSet ds = new DataSet();
        DataTable dtStagePriority = new DataTable("StagePriority");
        DataTable dtTier = new DataTable("Tier");
        DataTable dtEstimatedEffort = MasterData.EffortSizeList_Get(includeArchive: false);
        DataTable dtTeam = AOR.AORTeamList_Get();
        DataTable dtWorkloadAllocation = MasterData.WorkloadAllocationList_Get(includeArchive: 0);  //MasterData.WorkloadAllocation_ContractList_Get(includeArchive: 0);
        DataTable dtStatus = MasterData.StatusList_Get(includeArchive: false);

        dtStagePriority.Columns.Add("Val", typeof(string));
        dtStagePriority.Columns.Add("Txt", typeof(string));
        dtStagePriority.Rows.Add(new Object[] { "0", "" });
        dtStagePriority.Rows.Add(new Object[] { "1", "1" });
        dtStagePriority.Rows.Add(new Object[] { "2", "2" });
        dtStagePriority.Rows.Add(new Object[] { "3", "3" });
        dtStagePriority.Rows.Add(new Object[] { "4", "4" });
        dtStagePriority.AcceptChanges();

        dtTier.Columns.Add("Val", typeof(string));
        dtTier.Columns.Add("Txt", typeof(string));
        dtTier.Rows.Add(new Object[] { "0", "" });
        dtTier.Rows.Add(new Object[] { "1", "A" });
        dtTier.Rows.Add(new Object[] { "2", "B" });
        dtTier.Rows.Add(new Object[] { "3", "C" });
        dtTier.AcceptChanges();

        DataRow drTeam = dtTeam.NewRow();
        drTeam["AORTeam_ID"] = "0";
        drTeam["Team"] = "";
        dtTeam.Rows.InsertAt(drTeam, 0);
        dtTeam.AcceptChanges();

        dtEstimatedEffort.TableName = "Effort";
        dtTeam.TableName = "Team";

        dtStatus.Columns.Add("StatusDescription");

        foreach (DataRow dr in dtStatus.Rows)
        {
            if (dr["StatusType"].ToString() != "") dr["StatusDescription"] = dr["Status"].ToString() + " - " + dr["DESCRIPTION"].ToString();
        }

        dtStatus.AcceptChanges();




        DataTable dtIP = dtStatus.Copy();

        dtIP.TableName = "IP";
        dtIP.DefaultView.RowFilter = "StatusType IN ('', 'IP')";
        dtIP = dtIP.DefaultView.ToTable();

        DataTable dtCMMI = dtStatus.Copy();

        dtCMMI.TableName = "CMMI";
        dtCMMI.DefaultView.RowFilter = "StatusType IN ('', 'CMMI')";
        dtCMMI = dtCMMI.DefaultView.ToTable();

        DataTable dtInvestigation = dtStatus.Copy();

        dtInvestigation.TableName = "Investigation";
        dtInvestigation.DefaultView.RowFilter = "StatusType IN ('', 'Inv')";
        dtInvestigation = dtInvestigation.DefaultView.ToTable();

        DataTable dtTechnical = dtStatus.Copy();

        dtTechnical.TableName = "Technical";
        dtTechnical.DefaultView.RowFilter = "StatusType IN ('', 'TD')";
        dtTechnical = dtTechnical.DefaultView.ToTable();

        DataTable dtCustomerDesign = dtStatus.Copy();

        dtCustomerDesign.TableName = "CustomerDesign";
        dtCustomerDesign.DefaultView.RowFilter = "StatusType IN ('', 'CD')";
        dtCustomerDesign = dtCustomerDesign.DefaultView.ToTable();

        DataTable dtCoding = dtStatus.Copy();

        dtCoding.TableName = "Coding";
        dtCoding.DefaultView.RowFilter = "StatusType IN ('', 'C')";
        dtCoding = dtCoding.DefaultView.ToTable();

        DataTable dtInternalTesting = dtStatus.Copy();

        dtInternalTesting.TableName = "InternalTesting";
        dtInternalTesting.DefaultView.RowFilter = "StatusType IN ('', 'IT')";
        dtInternalTesting = dtInternalTesting.DefaultView.ToTable();

        DataTable dtCustomerVerificationTesting = dtStatus.Copy();

        dtCustomerVerificationTesting.TableName = "CustomerVerificationTesting";
        dtCustomerVerificationTesting.DefaultView.RowFilter = "StatusType IN ('', 'CVT')";
        dtCustomerVerificationTesting = dtCustomerVerificationTesting.DefaultView.ToTable();

        DataTable dtAdoption = dtStatus.Copy();

        dtAdoption.TableName = "Adoption";
        dtAdoption.DefaultView.RowFilter = "StatusType IN ('', 'Adopt')";
        dtAdoption = dtAdoption.DefaultView.ToTable();

        DataTable dtCyber = dtStatus.Copy();

        dtCyber.TableName = "CyberReview";
        dtCyber.DefaultView.RowFilter = "StatusType IN ('CR')";
        dtCyber = dtCyber.DefaultView.ToTable();

        dtCyber.Columns.Add("Text");
        foreach (DataRow dr in dtCyber.Rows)
        {
            if (dr["StatusType"].ToString() != "") dr["Text"] = dr["SORT_ORDER"].ToString() + " - " + dr["Status"].ToString() + " (" + dr["DESCRIPTION"].ToString() + ")";
        }

        ds.Tables.Add(dtStagePriority);
        ds.Tables.Add(dtTier);
        ds.Tables.Add(dtEstimatedEffort);
        ds.Tables.Add(dtWorkloadAllocation);
        ds.Tables.Add(dtTeam);
        ds.Tables.Add(dtIP);
        ds.Tables.Add(dtCMMI);
        ds.Tables.Add(dtInvestigation);
        ds.Tables.Add(dtTechnical);
        ds.Tables.Add(dtCustomerDesign);
        ds.Tables.Add(dtCoding);
        ds.Tables.Add(dtInternalTesting);
        ds.Tables.Add(dtCustomerVerificationTesting);
        ds.Tables.Add(dtAdoption);
        ds.Tables.Add(dtCyber);

        dsLookup = ds;
    }

    private DataTable LoadCrosswalkData()
    {
        DataTable dt = new DataTable();

        if (IsConfigured)
        {
            if (IsPostBack && CurrentLevel == 1 && Session["dtAORLevel" + CurrentLevel] != null)
            {
                //
                dt = (DataTable)Session["dtAORLevel" + CurrentLevel];
            }
            else
            {
                XmlDocument docLevel = new XmlDocument();
                XmlElement rootLevel = (XmlElement)docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
                XmlNode nodeLevel = Levels.SelectNodes("crosswalkparameters/level")[CurrentLevel - 1];
                XmlNode nodeImport = docLevel.ImportNode(nodeLevel, true);
                rootLevel.AppendChild(nodeImport);

                XmlDocument docFilters = new XmlDocument();
                XmlElement rootFilters = (XmlElement)docFilters.AppendChild(docFilters.CreateElement("filters"));

                if (Filter != string.Empty)
                {
                    string[] arrFilter = Filter.Split('|');

                    for (int j = 0; j < arrFilter.Length; j++)
                    {
                        XmlElement filter = docFilters.CreateElement("filter");
                        XmlElement field = docFilters.CreateElement("field");
                        XmlElement value = docFilters.CreateElement("id");
                        string[] arrValues = arrFilter[j].Split('=');

                        field.InnerText = arrValues[0];
                        if (arrValues[0].Trim() != "")
                            value.InnerText = arrValues[1].Trim() == ""
                                && field.InnerText.ToUpper() != "LASTMEETING_ID"
                                && field.InnerText.ToUpper() != "NEXTMEETING_ID"
                                && field.InnerText.ToUpper() != "PLANNEDSTART_ID"
                                && field.InnerText.ToUpper() != "PLANNEDEND_ID"
                                && field.InnerText.ToUpper() != "ACTUALSTART_ID"
                                && field.InnerText.ToUpper() != "ACTUALEND_ID"
                                && field.InnerText.ToUpper() != "INPROGRESSDATE_ID"
                                && field.InnerText.ToUpper() != "DEPLOYEDDATE_ID"
                                && field.InnerText.ToUpper() != "READYFORREVIEWDATE_ID"
                                ? "0" : arrValues[1].Trim();

                        filter.AppendChild(field);
                        filter.AppendChild(value);
                        rootFilters.AppendChild(filter);
                    }
                }

                //Build the quick filter part of our query for our crosswalk data pull
                HtmlSelect ReleaseItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item0");
                HtmlSelect ContractItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item1");
                HtmlSelect StatusItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item9");
                HtmlSelect WorkloadAllocationItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item9a");
                HtmlSelect AORTypeControl = (HtmlSelect)Page.Master.FindControl("ms_Item10");
                HtmlSelect VisibleToCustomerItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item10a");
                HtmlSelect ContainsTasksItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item2");
                HtmlInputCheckBox ArchiveCheckBoxCtrl = (HtmlInputCheckBox)Page.Master.FindControl("chk_Item11");

                List<string> listRelease = new List<string>();
                List<string> listAORType = new List<string>();
                List<string> listVisibleToCustomer = new List<string>();
                List<string> listContainsTasks = new List<string>();
                List<string> listContract = new List<string>();
                List<string> listTaskStatus = new List<string>();
                List<string> listAORProductionStatus = new List<string>();

                if (AORID_Filter_arr != null && AORID_Filter_arr.Length > 0)
                {
                    // when selected_AORID_Filter > 0, it means we only ever want one to show that specific AOR, so we populate the other filters manually
                    DataTable dtAOR = AOR.AORList_Get(Convert.ToInt32(AORID_Filter_arr[0]), includeArchive: 1);
                    if (dtAOR.Rows.Count > 0)
                    {
                        listRelease.Add(dtAOR.Rows[0]["ProductVersion_ID"].ToString());
                    }
                    foreach (var s in "70,10,80,72,1,2,3,4,5,7,8,9,11,12,13,15".Split(',')) listTaskStatus.Add(s);
                }
                else
                {
                    if (ReleaseItemControl != null && ReleaseItemControl.Items.Count > 0)
                        foreach (ListItem li in ReleaseItemControl.Items)
                            if (li.Selected) listRelease.Add(li.Value);

                    if (ContractItemControl != null && ContractItemControl.Items.Count > 0)
                        foreach (ListItem li in ContractItemControl.Items)
                            if (li.Selected) listContract.Add(li.Value);

                    if (StatusItemControl != null && StatusItemControl.Items.Count > 0)
                        foreach (ListItem li in StatusItemControl.Items)
                            if (li.Selected) listTaskStatus.Add(li.Value);

                    if (WorkloadAllocationItemControl != null && WorkloadAllocationItemControl.Items.Count > 0)
                        foreach (ListItem li in WorkloadAllocationItemControl.Items)
                            if (li.Selected) listAORProductionStatus.Add(li.Value);

                    if (AORTypeControl != null && AORTypeControl.Items.Count > 0)
                        foreach (ListItem li in AORTypeControl.Items)
                            if (li.Selected) listAORType.Add(li.Value);

                    if (VisibleToCustomerItemControl != null && VisibleToCustomerItemControl.Items.Count > 0)
                        foreach (ListItem li in VisibleToCustomerItemControl.Items)
                            if (li.Selected) listVisibleToCustomer.Add(li.Value);

                    if (ContainsTasksItemControl != null && ContainsTasksItemControl.Items.Count > 0)
                        foreach (ListItem li in ContainsTasksItemControl.Items)
                            if (li.Selected) listContainsTasks.Add(li.Value);
                }
                //This will load the child grid rows
                dt = AOR.AOR_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, qfRelease: String.Join(",", listRelease), qfAORType: String.Join(",", listAORType), AORID_Filter_arr: String.Join(",", AORID_Filter_arr), qfVisibleToCustomer: String.Join(",", listVisibleToCustomer), qfContainsTasks: String.Join(",", listContainsTasks), qfContract: String.Join(",", listContract), qfTaskStatus: String.Join(",", listTaskStatus), qfAORProductionStatus: String.Join(",", listAORProductionStatus), qfShowArchiveAOR: ArchiveCheckBoxCtrl.Checked ? "1" : "0" , getColumns: false);
                Session["dtAORLevel" + CurrentLevel] = dt;
            }
        }

        var items = WTSData.GetViewOptions(UserManagement.GetUserId_FromUsername(), "AOR");
        var gridViewId = GetDefaultGridViewId(11, UserManagement.GetUserId_FromUsername());
        var itiSettings_all_DB = (from DataRow dr in items.Rows
                                  where (int)dr["GridViewID"] == gridViewId
                                  select (string)dr["Tier1Columns"]).FirstOrDefault();

        if (itisettings.Value == "" && !string.IsNullOrEmpty(itiSettings_all_DB)) itisettings.Value = itiSettings_all_DB;


        //Build the dropDownList view choices from the items(settings) returned
        if (items.Rows.Count > 0)
        {
            ddlGridview.Items.Clear();
            //For each row found in the item settings
            foreach (DataRow row in items.Rows)
                if (row["ViewType"].ToString() == "1")
                {
                    if (row["ViewName"].ToString() != "-- New Gridview --")
                    {
                        var ddlItem = new ListItem();
                        ddlItem.Text = row["ViewName"].ToString();
                        ddlItem.Value = row["GridViewID"].ToString();
                        ddlItem.Attributes.Add("OptionGroup", row["WTS_RESOURCEID"].ToString() != "" ? "Custom Views" : "Process Views");
                        ddlItem.Attributes.Add("ViewType", row["ViewType"].ToString());
                        ddlItem.Attributes.Add("MyView", row["MyView"].ToString());
                        ddlItem.Attributes.Add("DefaultSelection", row["DefaultSelection"].ToString());
                        ddlGridview.Items.Add(ddlItem);
                    }
                }
        }

        var colNames = (from dc in dt.Columns.Cast<DataColumn>() select dc.ColumnName).Where(x => x != "X" && x != "Y" && x != "Z" && x.IndexOf("_ID", StringComparison.Ordinal) == -1).ToList();
        SubGridFilterCols = (from dc in dt.Columns.Cast<DataColumn>() select dc.ColumnName).Where(x => x.IndexOf("_ID", StringComparison.Ordinal) != -1).ToList();
        ColNamesCopy = colNames;

        if (itisettings.Value == "") //If the itisettings are missing then create them
        {
            dynamic itiSettings_all_DB_json = JsonConvert.DeserializeObject(itiSettings_all_DB);
            object[] objArray = new object[colNames.ToArray().Length];
            for (int i = 0; i < colNames.ToArray().Length; i++)
            {
                var colObj = new { name = colNames[i], show = true, sortorder = "none", sortpriority = "", concat = false, catcols = new string[0] };
                objArray[i] = colObj;
            }

            var tmpList = new List<string>();
            for (var i = 1; i < colNames.ToArray().Length + 1; i++)
                tmpList.Add(i.ToString());

            var newsettings = new //TODO: This object may need refactoring because the tblCols may have been loaded by the UI and the ddl.
            {
                sectionorder = itiSettings_all_DB_json.sectionorder,
                sectionexpanded = itiSettings_all_DB_json.sectionexpanded,
                gridname = itiSettings_all_DB_json.gridname,
                viewname = itiSettings_all_DB_json.viewname,
                tblCols = objArray,
                columnorder = tmpList.ToArray()
            };

            itisettings.Value = JsonConvert.SerializeObject(newsettings);
        }
        else //Else check if the settings need to be validated
        {
            dynamic itiSettings_all_sess = JsonConvert.DeserializeObject(Session[GridSessionKey].ToString());
            if (itiSettings_all_sess.validated == null)
            {

                itisettings.Value = ValidateItiSettings(Session[GridSessionKey].ToString());
                itiSettings_all_sess = JsonConvert.DeserializeObject(Session[GridSessionKey].ToString());
            }
            else
            {
                if (string.IsNullOrEmpty(itiSettings_all_sess.validated.ToString()))
                {
                    itisettings.Value = ValidateItiSettings(Session[GridSessionKey].ToString());
                    itiSettings_all_sess = JsonConvert.DeserializeObject(Session[GridSessionKey].ToString());
                }
                else
                {
                    var validatedOn = (DateTime)itiSettings_all_sess.validated;
                    if (validatedOn.Date < DateTime.Today)
                    {
                        itisettings.Value = ValidateItiSettings(Session[GridSessionKey].ToString());
                        itiSettings_all_sess = JsonConvert.DeserializeObject(Session[GridSessionKey].ToString());
                    }
                }
            }

            if (CurrentLevel == 1)
            {
                foreach (var currTblCol in itiSettings_all_sess.tblCols)
                    if (currTblCol.concat == true)
                    {
                        colNames.Add(currTblCol.name.ToString());
                    }
            }
            else
            {
                var subgridLevel = 2;
                foreach (var itiSettings_subgrid_sess in itiSettings_all_sess)
                    if (itiSettings_subgrid_sess.Name.IndexOf("subgrid") != -1)
                    {
                        if (subgridLevel == CurrentLevel)
                            foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
                                foreach (var currTblCol in itiSettings_subgrid_prop_sess[0].tblCols)
                                    if (currTblCol.concat == true)
                                        colNames.Add(currTblCol.name.ToString());

                        subgridLevel++;
                    }
            }

            dt.SetConcatCols(Session[GridSessionKey].ToString(), CurrentLevel);
            dt.SetColumnOrder(Session[GridSessionKey].ToString(), colNames.ToArray(), CurrentLevel);
            dt.SetSortOrder(Session[GridSessionKey].ToString(), CurrentLevel);
            //dt.SetColumnFilters(Session[GridSessionKey].ToString(), CurrentLevel);
            dt.SetColNames(Session[GridSessionKey].ToString(), CurrentLevel);
            if (dt.Columns.Contains("Y"))
            {
                dt.Columns["Y"].SetOrdinal(1);
            }
        }

        foreach (DataColumn col in dt.Columns) //TODO: Update this code so it correctly finds the values to update.
            if (col.ReadOnly == false)
                foreach (DataRow row in dt.Rows)
                    if (!string.IsNullOrEmpty(row[col].ToString()))
                        if (row[col].ToString().StartsWith("; 0", true, CultureInfo.CurrentCulture))
                            row[col] = row[col].ToString().Replace("; ", "0");

        foreach (DataColumn col in dt.Columns)
            if (col.ReadOnly == false)
                foreach (DataRow row in dt.Rows)
                    if (!string.IsNullOrEmpty(row[col].ToString()))
                        row[col] = HttpUtility.UrlDecode(row[col].ToString());

        dtCopy = dt;
        return dt;
    }

    private int GetChildGridCount(string subgridFilter)
    {
        XmlDocument docLevel = new XmlDocument();
        XmlElement rootLevel = (XmlElement)docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
        XmlNode nodeLevel = Levels.SelectNodes("crosswalkparameters/level")[CurrentLevel];

        if (nodeLevel == null) return 0;
        XmlNode nodeImport = docLevel.ImportNode(nodeLevel, true);
        rootLevel.AppendChild(nodeImport);

        XmlDocument docFilters = new XmlDocument();
        XmlElement rootFilters = (XmlElement)docFilters.AppendChild(docFilters.CreateElement("filters"));

        if (subgridFilter != string.Empty)
        {
            string[] arrFilter = subgridFilter.Split('|');

            for (int j = 0; j < arrFilter.Length; j++)
            {
                XmlElement filter = docFilters.CreateElement("filter");
                XmlElement field = docFilters.CreateElement("field");
                XmlElement value = docFilters.CreateElement("id");
                string[] arrValues = arrFilter[j].Split('=');

                field.InnerText = arrValues[0];
                if (arrValues[0].Trim() != "")
                    value.InnerText = (arrValues[1].Trim() == ""
                        && field.InnerText.ToUpper() != "LASTMEETING_ID"
                        && field.InnerText.ToUpper() != "NEXTMEETING_ID"
                        && field.InnerText.ToUpper() != "PLANNEDSTART_ID"
                        && field.InnerText.ToUpper() != "PLANNEDEND_ID"
                        && field.InnerText.ToUpper() != "ACTUALSTART_ID"
                        && field.InnerText.ToUpper() != "ACTUALEND_ID"
                        ? "0" : arrValues[1].Trim());

                filter.AppendChild(field);
                filter.AppendChild(value);
                rootFilters.AppendChild(filter);
            }
        }

        HtmlSelect ReleaseItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item0");
        HtmlSelect ContractItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item1");
        HtmlSelect StatusItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item9");
        HtmlSelect WorkloadAllocationItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item9a");
        HtmlSelect AORTypeControl = (HtmlSelect)Page.Master.FindControl("ms_Item10");
        HtmlSelect VisibleToCustomerItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item10a");
        HtmlSelect ContainsTasksItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item2");
        HtmlInputCheckBox ArchiveCheckBoxCtrl = (HtmlInputCheckBox)Page.Master.FindControl("chk_Item11");

        List<string> listRelease = new List<string>();
        List<string> listAORType = new List<string>();
        List<string> listVisibleToCustomer = new List<string>();
        List<string> listContainsTasks = new List<string>();
        List<string> listContract = new List<string>();
        List<string> listTaskStatus = new List<string>();
        List<string> listAORProductionStatus = new List<string>();

        if (ReleaseItemControl != null && ReleaseItemControl.Items.Count > 0)
            foreach (ListItem li in ReleaseItemControl.Items)
                if (li.Selected) listRelease.Add(li.Value);

        if (ContractItemControl != null && ContractItemControl.Items.Count > 0)
            foreach (ListItem li in ContractItemControl.Items)
                if (li.Selected) listContract.Add(li.Value);

        if (StatusItemControl != null && StatusItemControl.Items.Count > 0)
            foreach (ListItem li in StatusItemControl.Items)
                if (li.Selected) listTaskStatus.Add(li.Value);

        if (WorkloadAllocationItemControl != null && WorkloadAllocationItemControl.Items.Count > 0)
            foreach (ListItem li in WorkloadAllocationItemControl.Items)
                if (li.Selected) listAORProductionStatus.Add(li.Value);

        if (AORTypeControl != null && AORTypeControl.Items.Count > 0)
            foreach (ListItem li in AORTypeControl.Items)
                if (li.Selected) listAORType.Add(li.Value);

        if (VisibleToCustomerItemControl != null && VisibleToCustomerItemControl.Items.Count > 0)
            foreach (ListItem li in VisibleToCustomerItemControl.Items)
                if (li.Selected) listVisibleToCustomer.Add(li.Value);

        if (ContainsTasksItemControl != null && ContainsTasksItemControl.Items.Count > 0)
            foreach (ListItem li in ContainsTasksItemControl.Items)
                if (li.Selected) listContainsTasks.Add(li.Value);

        return AOR.AOR_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, qfRelease: String.Join(",", listRelease), qfAORType: String.Join(",", listAORType), AORID_Filter_arr: String.Join(",", AORID_Filter_arr), qfVisibleToCustomer: String.Join(",", listVisibleToCustomer), qfContainsTasks: String.Join(",", listContainsTasks), qfContract: String.Join(",", listContract), qfTaskStatus: String.Join(",", listTaskStatus), qfAORProductionStatus: String.Join(",", listAORProductionStatus), qfShowArchiveAOR: ArchiveCheckBoxCtrl.Checked ? "1" : "0", getColumns: false).Rows.Count;
    }

    private int GetDefaultGridViewId(int gridNameId, int resourceId)
    {
        var cmdText = "select settingvalue from usersetting where gridnameid = " + gridNameId + " AND wts_resourceid = " + resourceId;

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        using (SqlCommand cmd = new SqlCommand(cmdText, cn))
        {
            cmd.CommandType = CommandType.Text;
            cn.Open();

            return Convert.ToInt32(cmd.ExecuteScalar() ?? 160);
        }
    }


    //private void ValidateItiSettings()
    //{
    //    var ModifiedTblColGroups = new JArray();
    //    var itiSettings_all_sess = JsonConvert.DeserializeObject<dynamic>(Session[GridSessionKey].ToString());
    //    var columnOrder = JsonConvert.DeserializeObject<JArray>(itiSettings_all_sess.columnorder.ToString());
    //    var newAorProperties = new List<AorProperty>();
    //    var docLevel = new XmlDocument();
    //    var docFilters = new XmlDocument();
    //    docFilters.AppendChild(docFilters.CreateElement("filters"));
    //    docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
    //    var defaultCrosswalk_dt = AOR.AOR_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, qfRelease: "", qfAORType: "", AORID_Filter_arr: "", qfVisibleToCustomer: "", qfContract: "", qfTaskStatus: "", qfAORProductionStatus: "", getColumns: true);
    //    var jsonUpdated = false;

    //    foreach (var currModifiedTblColGroup_DB in defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray[0]).Distinct().ToArray())
    //        ModifiedTblColGroups.Add(currModifiedTblColGroup_DB.ToString());

    //    itiSettings_all_sess.columngroups = ModifiedTblColGroups;
    //    itiSettings_all_sess.validated = DateTime.Now;

    //    //Ensure all colgroups are correct in the Session
    //    foreach (var currCroswalkTblColGroup in defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
    //    {
    //        var itemExists = false;
    //        foreach (var itiSettings_all_tblcol in itiSettings_all_sess.tblCols)
    //            if (itiSettings_all_tblcol.name.ToString().ToUpper() == currCroswalkTblColGroup[1].ToString().ToUpper())
    //            {
    //                itiSettings_all_tblcol.colgroup = currCroswalkTblColGroup[0].ToString();
    //                jsonUpdated = true;
    //                itemExists = true;
    //                break;
    //            }
    //        //If there are any missing settings add them in
    //        if (!itemExists)
    //        {
    //            var newProperty = new AorProperty();

    //            newProperty.name = currCroswalkTblColGroup[1].ToString();
    //            newProperty.alias = "";
    //            newProperty.show = false;
    //            newProperty.sortorder = "none";
    //            newProperty.sortpriority = "";
    //            newProperty.groupname = "";
    //            newProperty.concat = false;
    //            newProperty.colgroup = currCroswalkTblColGroup[0].ToString();

    //            newAorProperties.Add(newProperty);
    //            columnOrder.Add((columnOrder.Count + 1).ToString());
    //        }
    //    }

    //    if (jsonUpdated || newAorProperties.Count > 0)
    //    {
    //        var aorProperties = JsonConvert.DeserializeObject<List<AorProperty>>(itiSettings_all_sess.tblCols.ToString());
    //        if (newAorProperties.Count > 0)
    //            foreach (var item in newAorProperties)
    //                aorProperties.Add(item);

    //        itiSettings_all_sess.tblCols = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(aorProperties));
    //        itiSettings_all_sess.columnorder = columnOrder;
    //    }

    //    foreach (var itiSettings_subgrid_sess in itiSettings_all_sess)
    //        if (itiSettings_subgrid_sess.Name.IndexOf("subgrid") != -1)
    //        {
    //            jsonUpdated = false;
    //            foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
    //            {
    //                newAorProperties.Clear();
    //                columnOrder = JsonConvert.DeserializeObject(itiSettings_subgrid_prop_sess[0].columnorder.ToString());

    //                foreach (var currDefaultCrosswalk_tblcol in defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
    //                {
    //                    var itemExists = false;
    //                    foreach (var itiSettings_subgrid_tblCol in itiSettings_subgrid_prop_sess[0].tblCols)
    //                        if (itiSettings_subgrid_tblCol.name.ToString().ToUpper() == currDefaultCrosswalk_tblcol[1].ToString().ToUpper())
    //                        {
    //                            itiSettings_subgrid_tblCol.colgroup = currDefaultCrosswalk_tblcol[0].ToString();
    //                            jsonUpdated = true;
    //                            itemExists = true;
    //                            break;
    //                        }

    //                    if (!itemExists)
    //                    {
    //                        var newProperty = new AorProperty();

    //                        newProperty.name = currDefaultCrosswalk_tblcol[1].ToString();
    //                        newProperty.alias = "";
    //                        newProperty.show = false;
    //                        newProperty.sortorder = "none";
    //                        newProperty.sortpriority = "";
    //                        newProperty.groupname = "";
    //                        newProperty.concat = false;
    //                        newProperty.colgroup = currDefaultCrosswalk_tblcol[0].ToString();

    //                        newAorProperties.Add(newProperty);
    //                        columnOrder.Add((columnOrder.Count + 1).ToString());
    //                    }
    //                }

    //                if (jsonUpdated || newAorProperties.Count > 0)
    //                {
    //                    var aorProperties = JsonConvert.DeserializeObject<List<AorProperty>>(itiSettings_subgrid_prop_sess[0].tblCols.ToString());
    //                    if (newAorProperties.Count > 0)
    //                        foreach (var i in newAorProperties)
    //                            aorProperties.Add(i);
    //                    itiSettings_subgrid_prop_sess[0].tblCols = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(aorProperties));
    //                    itiSettings_subgrid_prop_sess[0].columnorder = columnOrder;
    //                }
    //            }
    //        }

    //    string[] oldColCount = JsonConvert.DeserializeObject<string[]>(itiSettings_all_sess.columnorder.ToString());
    //    var newColCount = defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray[1]).Distinct().Count();
    //    var colDiff = oldColCount.Length - newColCount;

    //    if (colDiff > 0)
    //    {
    //        var validCols = new JArray();

    //        foreach (var currTblCol_sess in itiSettings_all_sess.tblCols)
    //            foreach (var currTblCol_DB in defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
    //                if (currTblCol_sess.name.ToString().ToUpper() == currTblCol_DB[1].ToString().ToUpper())
    //                {
    //                    validCols.Add(currTblCol_sess);
    //                    break;
    //                }

    //        string colName;
    //        var newColOrder = new List<string>();

    //        for (int i = 0; i < itiSettings_all_sess.columnorder.Count; i++)
    //        {
    //            colName = "";
    //            for (int j = 0; j < itiSettings_all_sess.tblCols.Count; j++)
    //                if (itiSettings_all_sess.columnorder[i] == j + 1) colName = itiSettings_all_sess.tblCols[j].name;

    //            if (!string.IsNullOrEmpty(colName))
    //                for (int k = 0; k < validCols.Count; k++)
    //                    if (colName == validCols[k].First.First.ToString())
    //                        newColOrder.Add((k + 1).ToString());
    //        }

    //        itiSettings_all_sess.tblCols = JArray.FromObject(validCols);
    //        itiSettings_all_sess.columnorder = JArray.FromObject(newColOrder);

    //        foreach (var itiSettings_subgrid_sess in itiSettings_all_sess)
    //            if (itiSettings_subgrid_sess.Name.IndexOf("subgrid") != -1)
    //            {
    //                validCols = new JArray();
    //                newColOrder.Clear();

    //                foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
    //                {
    //                    foreach (var currTblCol_Sess in itiSettings_subgrid_prop_sess[0].tblCols)
    //                        foreach (var row in defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
    //                            if (currTblCol_Sess.name.ToString().ToUpper() == row[1].ToString().ToUpper())
    //                            {
    //                                validCols.Add(currTblCol_Sess);
    //                                break;
    //                            }

    //                    for (int i = 0; i < itiSettings_subgrid_prop_sess[0].columnorder.Count; i++)
    //                    {
    //                        colName = "";
    //                        for (int j = 0; j < itiSettings_subgrid_prop_sess[0].tblCols.Count; j++)
    //                            if (itiSettings_subgrid_prop_sess[0].columnorder[i] == j + 1) colName = itiSettings_subgrid_prop_sess[0].tblCols[j].name;

    //                        if (!string.IsNullOrEmpty(colName))
    //                            for (int k = 0; k < validCols.Count; k++)
    //                                if (colName == validCols[k].First.First.ToString())
    //                                    newColOrder.Add((k + 1).ToString());
    //                    }

    //                    itiSettings_subgrid_prop_sess[0].tblCols = JArray.FromObject(validCols);
    //                    itiSettings_subgrid_prop_sess[0].columnorder = JArray.FromObject(newColOrder);
    //                }
    //            }
    //    }

    //    Session[GridSessionKey] = JsonConvert.SerializeObject(itiSettings_all_sess);
    //    itisettings.Value = JsonConvert.SerializeObject(itiSettings_all_sess);
    //}
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);
        if (DCC.Contains("X")) row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage(true));

        if (DCC.Contains("Y"))
        {
            if (this.CanEditWorkItem && DCC.Contains("Primary Task"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBoxNew(true,true, "Task", "Task_ID", "Task_ID"));
            }
            else if (this.CanEditWorkItem && DCC.Contains("Work Task"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                if (row.Cells[DCC.IndexOf("Work Task")].Text.Contains(" - "))
                {
                    row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBoxNew(true,true, "Subtask", "WORKITEM_TASKID", "WorkTask_ID"));
                }
                else
                {
                    row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBoxNew(true,true, "Task", "Task_ID", "Task_ID"));
                }
            }
            else if (this.CanEditAOR && !this.BlnArchivedAOR && DCC.Contains("AOR #"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBoxNew(true, true, "AOR", "AOR_ID", "AOR_ID"));
            }
            else if (this.CanEditAOR && !this.BlnArchivedAOR && DCC.Contains("CRNAME_ID"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBoxNew(true, true, "CR", "CR_ID", "CR_ID"));
            }

            else
            {
                row.Cells[DCC.IndexOf("Y")].Style["display"] = "none";
            }
        }
    }

    //This will set up each row the correct controls and display after a row is bound to the datagrid
    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        dynamic itiSettings_all_session = JsonConvert.DeserializeObject(Session[GridSessionKey].ToString());
        var subGridFilter = Filter;
        bool showChildRc = false;
        GridViewRow row = e.Row;
        var workitemid = "";
        var workitem = "";
        var taskCount = 0;

        if (CurrentLevel == 1)
        {
            showChildRc = itiSettings_all_session.showchildrc ?? false;
        }
        else
        {
            var subgridLevel = 2;
            foreach (var itiSettings_subgrid_sess in itiSettings_all_session)
            {
                if (itiSettings_subgrid_sess.Name.IndexOf("subgrid") != -1)
                {
                    if (subgridLevel == CurrentLevel)
                    {
                        showChildRc = itiSettings_subgrid_sess.Value[0].showchildrc ?? false;
                    }
                    subgridLevel++;
                }
            }
        }

        if (showChildRc)
        {
            if (SubGridFilterCols.Count > 0)
            {
                foreach (var item in SubGridFilterCols)
                    if (row.Cells[DCC.IndexOf(item)].Text != "&nbsp;")
                    {
                        subGridFilter += "|" + item + "=" + row.Cells[DCC.IndexOf(item)].Text;
                    }
                taskCount = GetChildGridCount(subGridFilter);
            }
        }

        FormatRowDisplay(ref row);

        if (DCC.Contains("Archive_ID"))
        {
             bool.TryParse(row.Cells[DCC.IndexOf("Archive_ID")].Text, out this.BlnArchivedAOR);
        }

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage(false));
            if (taskCount > 0)
            {
                HtmlGenericControl divTasks = new HtmlGenericControl();
                HtmlGenericControl divTaskCount = new HtmlGenericControl();

                divTaskCount.InnerText = string.Format("({0})", taskCount);
                divTaskCount.Style["display"] = "table-cell";
                divTaskCount.Style["padding-left"] = "2px";
                divTasks.Controls.Add(divTaskCount);
                row.Cells[DCC.IndexOf("X")].Controls.Add(divTasks);
            }
        }

        if (DCC.Contains("Y"))
        {
            if (this.CanEditWorkItem && DCC.Contains("Primary Task"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBoxNew(false, true, "Task", "Task_ID", row.Cells[DCC.IndexOf("Primary Task")].Text));
            }
            else if (this.CanEditWorkItem && DCC.Contains("Work Task"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                if (row.Cells[DCC.IndexOf("Work Task")].Text.Contains(" - "))
                {
                    row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBoxNew(false, true, "Subtask", "WORKITEM_TASKID", row.Cells[DCC.IndexOf("WorkTask_ID")].Text));
                }
                else
                {
                    row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBoxNew(false, true, "Task", "Task_ID", row.Cells[DCC.IndexOf("WorkTask_ID")].Text));
                }
            }
            else if (this.CanEditAOR && DCC.Contains("AOR #"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBoxNew(false, !this.BlnArchivedAOR, "AOR", "AOR_ID", row.Cells[DCC.IndexOf("AOR #")].Text));
            }
            else if (this.CanEditAOR && !this.BlnArchivedAOR && DCC.Contains("CRNAME_ID"))
            {
                if (!String.IsNullOrEmpty(row.Cells[DCC.IndexOf("CRNAME_ID")].Text) && row.Cells[DCC.IndexOf("CRNAME_ID")].Text != "&nbsp;")
                {
                    row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                    row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBoxNew(false, true, "CR", "CR_ID", row.Cells[DCC.IndexOf("CRNAME_ID")].Text));
                }
            }

            else
            {
                row.Cells[DCC.IndexOf("Y")].Style["display"] = "none";
            }
        }

        if (DCC.Contains("PRIMARYTASK_ID") && DCC.Contains("Primary Task"))
        {
            workitemid = row.Cells[DCC.IndexOf("PRIMARYTASK_ID")].Text;
            workitem = row.Cells[DCC.IndexOf("Primary Task")].Text;
            row.Attributes.Add("workitem_id", row.Cells[DCC.IndexOf("Primary Task")].Text);
            row.Cells[DCC.IndexOf("Primary Task")].Style["width"] = "75px";
            row.Cells[DCC.IndexOf("Primary Task")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("Primary Task")].Controls.Add(CreateLink("Task", row.Cells[DCC.IndexOf("PRIMARYTASK_ID")].Text, row.Cells[DCC.IndexOf("Primary Task")].Text));
        }
        if (DCC.Contains("WORKTASK_ID") && DCC.Contains("WORK TASK"))
        {
            if (CanViewWorkItem && row.Cells[DCC.IndexOf("WORK TASK")].Text != "&nbsp;")
            {
                workitemid = row.Cells[DCC.IndexOf("WORKTASK_ID")].Text;
                workitem = row.Cells[DCC.IndexOf("WORK TASK")].Text;
                row.Attributes.Add("workitem_id", row.Cells[DCC.IndexOf("WORK TASK")].Text);
                row.Cells[DCC.IndexOf("WORK TASK")].Style["width"] = "90px";
                row.Cells[DCC.IndexOf("WORK TASK")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("WORK TASK")].Controls.Add(CreateLink("Task", row.Cells[DCC.IndexOf("WORKTASK_ID")].Text, row.Cells[DCC.IndexOf("WORK TASK")].Text));
            }
        }

        if (DCC.Contains("AOR_ID"))
        {
            row.Attributes.Add("aor_id", row.Cells[DCC.IndexOf("AOR_ID")].Text);

            if (DCC.Contains("Rank"))
            {
                string nRank = string.Empty;

                if (row.Cells[DCC.IndexOf("Rank")].Text != "&nbsp;")
                {
                    int rankID = 0;
                    int.TryParse(row.Cells[DCC.IndexOf("Rank")].Text, out rankID);

                    nRank = rankID.ToString("D2");
                }

                row.Cells[DCC.IndexOf("Rank")].Text = nRank;
            }

            if (DCC.Contains("Tier") && DCC.Contains("Rank") && DCC.Contains("Tier Rank"))
            {
                row.Cells[DCC.IndexOf("Tier Rank")].Text = (row.Cells[DCC.IndexOf("Tier")].Text + "_" + row.Cells[DCC.IndexOf("Rank")].Text).Replace("&nbsp;", "").Trim('_');
                row.Cells[DCC.IndexOf("Tier")].Style["display"] = "none";
                row.Cells[DCC.IndexOf("Rank")].Style["display"] = "none";
            }

            if (CanEditAOR && !this.BlnArchivedAOR)
            {
                if (DCC.Contains("AOR Name"))
                {
                    row.Cells[DCC.IndexOf("AOR Name")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("AOR Name")].Controls.Add(CreateTextBox("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, "AOR Name", row.Cells[DCC.IndexOf("AOR Name")].Text, false));
                }

                if (DCC.Contains("Description"))
                {
                    row.Cells[DCC.IndexOf("Description")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Description")].Controls.Add(CreateTextBox("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, "Description", row.Cells[DCC.IndexOf("Description")].Text, false));
                }

                if (DCC.Contains("Sort"))
                {
                    row.Cells[DCC.IndexOf("Sort")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Sort")].Controls.Add(CreateTextBox("AOR", row.Cells[DCC.IndexOf("AOR_ID")].Text, "Sort", row.Cells[DCC.IndexOf("Sort")].Text, true));
                }

                if (DCC.Contains("AORRelease_ID"))
                {
                    //todo: handle renamed editable columns
                    string nCol = "Stage Priority";
                    if (DCC.Contains("Priority")) nCol = "Priority";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["StagePriority"], "Priority", "Txt", "Val", row.Cells[DCC.IndexOf(nCol)].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                    }

                    nCol = "Coding Estimated Effort";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["Effort"], "Coding Estimated Effort", "EffortSize", "EffortSizeID", row.Cells[DCC.IndexOf("CodingEffort_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                    }

                    nCol = "Testing Estimated Effort";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["Effort"], "Testing Estimated Effort", "EffortSize", "EffortSizeID", row.Cells[DCC.IndexOf("TestingEffort_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                    }

                    nCol = "Training/Support Estimated Effort";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["Effort"], "Training/Support Estimated Effort", "EffortSize", "EffortSizeID", row.Cells[DCC.IndexOf("TrainingSupportEffort_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                    }

                    nCol = "Workload Allocation";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["WorkloadAllocation"], "Workload Allocation", "WorkloadAllocation", "WorkloadAllocationID", row.Cells[DCC.IndexOf("Workload_Allocation_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));

                        //int primarySystemContractID = 0;
                        //int.TryParse(row.Cells[DCC.IndexOf("PRIMARY_SYSTEM_CONTRACT_ID")].Text, out primarySystemContractID);
                        //DataTable rowWorkloadAllocation = dsLookup.Tables["WorkloadAllocation_Contract"];
                        //rowWorkloadAllocation.DefaultView.RowFilter = "CONTRACTID IN (0, " + primarySystemContractID + ")";
                        //rowWorkloadAllocation.DefaultView.Sort = "ContractID";
                        //rowWorkloadAllocation = rowWorkloadAllocation.DefaultView.ToTable();
                        //DropDownList ddlWorkloadAllocation = CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, rowWorkloadAllocation, "Workload Allocation", "WorkloadAllocation", "WorkloadAllocationID", row.Cells[DCC.IndexOf("Workload_Allocation_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null);

                        //string currentContract = string.Empty;
                        //int insertCount = 0;
                        //foreach (DataRow waContract in rowWorkloadAllocation.Rows)
                        //{
                        //    if (waContract["ContractID"].ToString() != currentContract && waContract["ContractID"].ToString() != "0")
                        //    {
                        //        ListItem contractLi = new ListItem(waContract["Contract"].ToString());
                        //        contractLi.Attributes.Add("disabled", "disabled");
                        //        contractLi.Attributes.Add("style", "background-color: white");
                        //        ddlWorkloadAllocation.Items.Insert(rowWorkloadAllocation.Rows.IndexOf(waContract) + insertCount, contractLi);
                        //        currentContract = waContract["ContractID"].ToString();
                        //        insertCount++;
                        //    }
                        //}

                        //row.Cells[DCC.IndexOf(nCol)].Controls.Add(ddlWorkloadAllocation);
                    }

                    nCol = "Critical Path Team";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["Team"], "Critical Path Team", "Team", "AORTeam_ID", row.Cells[DCC.IndexOf("CriticalPathTeam_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                    }

                    nCol = "Approved";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateCheckBox("AOR", row.Cells[DCC.IndexOf("AOR_ID")].Text, "Approved", row.Cells[DCC.IndexOf("Approved")].Text));
                    }

                    nCol = "Visible To Customer";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateCheckBox("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, "Visible To Customer", row.Cells[DCC.IndexOf("Visible To Customer")].Text));
                    }

                    nCol = "IP1 Status";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["IP"], "IP1", "Status", "StatusID", row.Cells[DCC.IndexOf("IP1_STATUS_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                    }

                    nCol = "IP2 Status";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["IP"], "IP2", "Status", "StatusID", row.Cells[DCC.IndexOf("IP2_STATUS_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                    }

                    nCol = "IP3 Status";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["IP"], "IP3", "Status", "StatusID", row.Cells[DCC.IndexOf("IP3_STATUS_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                    }

                    nCol = "Tier";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["Tier"], "Tier", "Txt", "Val", row.Cells[DCC.IndexOf("Tier_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                    }

                    nCol = "Rank";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateTextBox("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, "Rank", row.Cells[DCC.IndexOf("Rank")].Text, true));
                    }

                    nCol = "Cyber Review";
                    if (DCC.Contains(nCol))
                    {
                        row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["CyberReview"], "Cyber Review", "STATUS", "STATUSID", row.Cells[DCC.IndexOf("Cyber_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                    }

                    if (DCC.Contains("AORTypeRef_ID") && !(row.Cells[DCC.IndexOf("AORTypeRef_ID")].Text != "Release/Deployment MGMT" && row.Cells[DCC.IndexOf("AORTypeRef_ID")].Text != "PD2TDR Managed AORs"))
                    {
                        nCol = "Investigation Status";
                        if (DCC.Contains("Inv"))
                        {
                            nCol = "Inv";
                            row.Cells[DCC.IndexOf("Inv")].Style["width"] = "130px";
                        }
                        if (DCC.Contains(nCol))
                        {
                            row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                            row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["Investigation"], "Inv", "StatusDescription", "StatusID", row.Cells[DCC.IndexOf("INVESTIGATION_STATUS_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                        }

                        nCol = "Technical Status";
                        if (DCC.Contains("TD"))
                        {
                            nCol = "TD";
                            row.Cells[DCC.IndexOf("TD")].Style["width"] = "130px";
                        }
                        if (DCC.Contains(nCol))
                        {
                            row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                            row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["Technical"], "TD", "StatusDescription", "StatusID", row.Cells[DCC.IndexOf("TECHNICAL_STATUS_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                        }

                        nCol = "Customer Design Status";
                        if (DCC.Contains("CD"))
                        {
                            nCol = "CD";
                            row.Cells[DCC.IndexOf("CD")].Style["width"] = "130px";
                        }
                        if (DCC.Contains(nCol))
                        {
                            row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                            row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["CustomerDesign"], "CD", "StatusDescription", "StatusID", row.Cells[DCC.IndexOf("CUSTOMER_DESIGN_STATUS_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                        }

                        nCol = "Coding Status";
                        if (DCC.Contains("C"))
                        {
                            nCol = "C";
                            row.Cells[DCC.IndexOf("C")].Style["width"] = "130px";
                        }
                        if (DCC.Contains(nCol))
                        {
                            row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                            row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["Coding"], "C", "StatusDescription", "StatusID", row.Cells[DCC.IndexOf("CODING_STATUS_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                        }

                        nCol = "Internal Testing Status";
                        if (DCC.Contains("IT"))
                        {
                            nCol = "IT";
                            row.Cells[DCC.IndexOf("IT")].Style["width"] = "130px";
                        }
                        if (DCC.Contains(nCol))
                        {
                            row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                            row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["InternalTesting"], "IT", "StatusDescription", "StatusID", row.Cells[DCC.IndexOf("INTERNAL_TESTING_STATUS_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                        }

                        nCol = "Customer Validation Testing Status";
                        if (DCC.Contains("CVT"))
                        {
                            nCol = "CVT";
                            row.Cells[DCC.IndexOf("CVT")].Style["width"] = "130px";
                        }
                        if (DCC.Contains(nCol))
                        {
                            row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                            row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["CustomerVerificationTesting"], "CVT", "StatusDescription", "StatusID", row.Cells[DCC.IndexOf("CUSTOMER_VALIDATION_STATUS_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                        }

                        nCol = "Adoption Status";
                        if (DCC.Contains("Adopt"))
                        {
                            nCol = "Adopt";
                            row.Cells[DCC.IndexOf("Adopt")].Style["width"] = "130px";
                        }
                        if (DCC.Contains(nCol))
                        {
                            row.Cells[DCC.IndexOf(nCol)].Style["text-align"] = "center";
                            row.Cells[DCC.IndexOf(nCol)].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["Adoption"], "Adopt", "StatusDescription", "StatusID", row.Cells[DCC.IndexOf("ADOPTION_STATUS_ID")].Text, row.Cells[DCC.IndexOf(nCol)].Text, null));
                        }

                        if (DCC.Contains("CMMI"))
                        {
                            row.Cells[DCC.IndexOf("CMMI")].Style["text-align"] = "center";
                            row.Cells[DCC.IndexOf("CMMI")].Controls.Add(CreateDropDownList("AOR", row.Cells[DCC.IndexOf("AORRelease_ID")].Text, dsLookup.Tables["CMMI"], "CMMI", "Status", "StatusID", row.Cells[DCC.IndexOf("CMMI_ID")].Text, row.Cells[DCC.IndexOf("CMMI")].Text, null));
                        }
                    }
                }
                if (DCC.Contains("Product Version"))
                {
                    row.Cells[DCC.IndexOf("Product Version")].Style["text-align"] = "center";
                }
            }
        }

        if (DCC.Contains("AORRelease_ID")) row.Attributes.Add("aorrelease_id", row.Cells[DCC.IndexOf("AORRelease_ID")].Text);

        if (DCC.Contains("Carry In") && DCC.Contains("Current Release") && DCC.Contains("Release"))
        {
            string nRelease = row.Cells[DCC.IndexOf("Carry In")].Text + " -> " + row.Cells[DCC.IndexOf("Current Release")].Text;

            if (row.Cells[DCC.IndexOf("Carry In")].Text == "&nbsp;")
            {
                nRelease = row.Cells[DCC.IndexOf("Current Release")].Text;
            }
            else
            {
                nRelease = row.Cells[DCC.IndexOf("Carry In")].Text + " -> " + row.Cells[DCC.IndexOf("Current Release")].Text;
            }

            row.Cells[DCC.IndexOf("Release")].Text = nRelease;
            row.Cells[DCC.IndexOf("Carry In")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Current Release")].Style["display"] = "none";
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

        if (DCC.Contains("# of Attachments"))
        {
            row.Cells[DCC.IndexOf("# of Attachments")].Controls.Add(CreateLink("AttachmentCount", row.Cells[DCC.IndexOf("AOR_ID")].Text + '-' + row.Cells[DCC.IndexOf("AORRelease_ID")].Text, row.Cells[DCC.IndexOf("# of Attachments")].Text));
        }

        //add Total row to top of grid (as part of header to keep fixed)
        //if (row.RowIndex == 0)
        //{
        //    FixedTotalRow(row);
        //}

        if (DCC.Contains("CRName_ID") && Server.HtmlDecode(row.Cells[DCC.IndexOf("CRName_ID")].Text).Trim() != "")
        {
            if (DCC.Contains("CR Customer Title")) row.Cells[DCC.IndexOf("CR Customer Title")].Controls.Add(CreateLink("CR", row.Cells[DCC.IndexOf("CRName_ID")].Text, row.Cells[DCC.IndexOf("CR Customer Title")].Text));
        }

        if (DCC.Contains("CRTITLE_ID") && Server.HtmlDecode(row.Cells[DCC.IndexOf("CRTITLE_ID")].Text).Trim() != "")
        {
            if (DCC.Contains("CR Internal Title")) row.Cells[DCC.IndexOf("CR Internal Title")].Controls.Add(CreateLink("CR", row.Cells[DCC.IndexOf("CRTITLE_ID")].Text, row.Cells[DCC.IndexOf("CR Internal Title")].Text));
        }

        if (DCC.Contains("Planned Start"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Planned Start")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Planned Start")].Text = String.Format("{0:M/d/yyyy}", nDate);
            }
        }

        if (DCC.Contains("Planned End"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Planned End")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Planned End")].Text = String.Format("{0:M/d/yyyy}", nDate);
            }
        }

        if (DCC.Contains("Actual Start"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Actual Start")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Actual Start")].Text = String.Format("{0:M/d/yyyy}", nDate);
            }
        }

        if (DCC.Contains("Actual End"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Actual End")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Actual End")].Text = String.Format("{0:M/d/yyyy}", nDate);
            }
        }

        if (DCC.Contains("Scheduled Deliverable"))
        {
            row.Cells[DCC.IndexOf("Deployment")].Style["text-align"] = "center";
        }

        if (DCC.Contains("Deployment Start Date"))
        {
            DateTime nDate = new DateTime();
            DateTime.TryParse(Server.HtmlDecode(row.Cells[DCC.IndexOf("Deployment Start Date")].Text.Replace("&nbsp;", " ").Trim()), out nDate);
            if (nDate != DateTime.MinValue)
            {
                row.Cells[DCC.IndexOf("Deployment Start Date")].Text = Server.HtmlDecode(String.Format("{0:MM/dd/yyyy}", nDate));
                row.Cells[DCC.IndexOf("Deployment Start Date")].Style["text-align"] = "center";
            }
        }

        if (DCC.Contains("Deployment End Date"))
        {
            DateTime nDate = new DateTime();
            DateTime.TryParse(Server.HtmlDecode(row.Cells[DCC.IndexOf("Deployment End Date")].Text.Replace("&nbsp;", " ").Trim()), out nDate);
            if (nDate != DateTime.MinValue)
            {
                row.Cells[DCC.IndexOf("Deployment End Date")].Text = Server.HtmlDecode(String.Format("{0:MM/dd/yyyy}", nDate));
                row.Cells[DCC.IndexOf("Deployment End Date")].Style["text-align"] = "center";
            }
        }
    }

    private void FixedTotalRow(GridViewRow row)
    {
        try
        {
            DataTable dt = (DataTable)Session["dtAORLevel" + CurrentLevel];
            // Add total row for each page
            //total row is the first row in datatable.Using datatable instead of grid row because if I use grid row it won't be visible on paging.
            if (dt.Rows[0]["ROW_ID"].ToString() != "0")
            {
                return;
            }

            GridViewRow nRow = new GridViewRow(row.RowIndex, row.RowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);
            TableHeaderCell nCell = new TableHeaderCell();
            nRow.Style["height"] = "25px";
            nRow.Attributes.Add("rowID", "total");
            XmlNode nodeLevel = this.Levels.SelectNodes("crosswalkparameters/level")[CurrentLevel - 1];
            int intColspan = nodeLevel.SelectNodes("breakout/column").Count + 1;
            // 3: #, #/#, FMTL Stage columns
            nCell = new TableHeaderCell();
            nCell.Text = "TOTAL";
            nCell.ColumnSpan = intColspan;
            nCell.Style["background"] = "#d7e8fc";
            nRow.Cells.Add(nCell);

            int columnCount = 0;

            for (int i = 0; i <= DCC.Count - 1; i++)
            {
                string columnDBName = DCC[i].ColumnName.ToString();
                Boolean visible = DCC[i].ColumnName.EndsWith("_ID") ? false : true;

                if (visible)
                {
                    columnCount = columnCount + 1;
                }

                if (columnCount > intColspan && visible)
                {
                    nCell = new TableHeaderCell();
                    nCell.Text = Convert.IsDBNull(dt.Rows[0][columnDBName].ToString()) ? "&nbsp;" : dt.Rows[0][columnDBName].ToString();
                    nCell.Style["background"] = "#d7e8fc";
                    nRow.Cells.Add(nCell);
                }
            }

            grdData.Controls[1].Controls[1].Controls[0].Controls[0].Controls.AddAt(1, nRow);

            if (grdData.PageIndex == 0)
            {
                // Hide the first row (summary data) on the first page
                row.Style["display"] = "none";
            }


        }
        catch (Exception ex)
        {
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
            row.Cells[DCC.IndexOf("X")].Style["width"] = "36px";

            if (CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("Carry In") && DCC.Contains("Current Release") && DCC.Contains("Release"))
        {
            row.Cells[DCC.IndexOf("Carry In")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Current Release")].Style["display"] = "none";
        }

        if (DCC.Contains("Tier") && DCC.Contains("Rank") && DCC.Contains("Tier Rank"))
        {
            row.Cells[DCC.IndexOf("Tier")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Rank")].Style["display"] = "none";
        }

        //Any
        if (DCC.Contains("Workload Priority")) row.Cells[DCC.IndexOf("Workload Priority")].Style["width"] = "130px";
        if (DCC.Contains("Resource Count (T.BA.PA.CT)")) row.Cells[DCC.IndexOf("Resource Count (T.BA.PA.CT)")].Style["width"] = "180px";
        if (DCC.Contains("Carry In/Out Count")) row.Cells[DCC.IndexOf("Carry In/Out Count")].Style["width"] = "150px";
        //AOR
        if (DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "45px";
        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["width"] = "500px";
        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["text-align"] = "left";
        if (DCC.Contains("Description")) row.Cells[DCC.IndexOf("Description")].Style["text-align"] = "left";
        if (DCC.Contains("Description")) row.Cells[DCC.IndexOf("Description")].Style["width"] = "350px";
        if (DCC.Contains("Resources")) row.Cells[DCC.IndexOf("Resources")].Style["width"] = "120px";
        if (DCC.Contains("Sort")) row.Cells[DCC.IndexOf("Sort")].Style["width"] = "45px";
        if (DCC.Contains("Coding Estimated Effort")) row.Cells[DCC.IndexOf("Coding Estimated Effort")].Style["width"] = "100px";
        if (DCC.Contains("Testing Estimated Effort")) row.Cells[DCC.IndexOf("Testing Estimated Effort")].Style["width"] = "100px";
        if (DCC.Contains("Training/Support Estimated Effort")) row.Cells[DCC.IndexOf("Training/Support Estimated Effort")].Style["width"] = "100px";
        if (DCC.Contains("Stage Priority")) row.Cells[DCC.IndexOf("Stage Priority")].Style["width"] = "55px";
        if (DCC.Contains("Carry In")) row.Cells[DCC.IndexOf("Carry In")].Style["width"] = "55px";
        if (DCC.Contains("Release")) row.Cells[DCC.IndexOf("Release")].Style["width"] = "140px";
        if (DCC.Contains("Tier")) row.Cells[DCC.IndexOf("Tier")].Style["width"] = "45px";
        if (DCC.Contains("Rank")) row.Cells[DCC.IndexOf("Rank")].Style["width"] = "45px";
        if (DCC.Contains("Tier Rank")) row.Cells[DCC.IndexOf("Tier Rank")].Style["width"] = "45px";
        if (DCC.Contains("Last Meeting")) row.Cells[DCC.IndexOf("Last Meeting")].Style["width"] = "70px";
        if (DCC.Contains("Next Meeting")) row.Cells[DCC.IndexOf("Next Meeting")].Style["width"] = "70px";
        if (DCC.Contains("# of Meetings")) row.Cells[DCC.IndexOf("# of Meetings")].Style["width"] = "65px";
        if (DCC.Contains("# of Attachments")) row.Cells[DCC.IndexOf("# of Attachments")].Style["width"] = "65px";
        if (DCC.Contains("CMMI")) row.Cells[DCC.IndexOf("CMMI")].Style["width"] = "100px";
        if (DCC.Contains("Cyber Review")) row.Cells[DCC.IndexOf("Cyber Review")].Style["width"] = "80px";
        if (DCC.Contains("Critical Path Team")) row.Cells[DCC.IndexOf("Critical Path Team")].Style["width"] = "80px";
        if (DCC.Contains("AOR Workload Type")) row.Cells[DCC.IndexOf("AOR Workload Type")].Style["width"] = "75px";
        if (DCC.Contains("Visible To Customer")) row.Cells[DCC.IndexOf("Visible To Customer")].Style["width"] = "70px";
        if (DCC.Contains("Workload Allocation")) row.Cells[DCC.IndexOf("Workload Allocation")].Style["width"] = "185px";
        if (DCC.Contains("Investigation Status")) row.Cells[DCC.IndexOf("Investigation Status")].Style["width"] = "130px";
        if (DCC.Contains("Technical Status")) row.Cells[DCC.IndexOf("Technical Status")].Style["width"] = "130px";
        if (DCC.Contains("Customer Design Status")) row.Cells[DCC.IndexOf("Customer Design Status")].Style["width"] = "130px";
        if (DCC.Contains("Coding Status")) row.Cells[DCC.IndexOf("Coding Status")].Style["width"] = "130px";
        if (DCC.Contains("Internal Testing Status")) row.Cells[DCC.IndexOf("Internal Testing Status")].Style["width"] = "130px";
        if (DCC.Contains("Customer Validation Testing Status")) row.Cells[DCC.IndexOf("Customer Validation Testing Status")].Style["width"] = "130px";
        if (DCC.Contains("Adoption Status")) row.Cells[DCC.IndexOf("Adoption Status")].Style["width"] = "130px";
        if (DCC.Contains("IP1 Status")) row.Cells[DCC.IndexOf("IP1 Status")].Style["width"] = "130px";
        if (DCC.Contains("IP2 Status")) row.Cells[DCC.IndexOf("IP2 Status")].Style["width"] = "130px";
        if (DCC.Contains("IP3 Status")) row.Cells[DCC.IndexOf("IP3 Status")].Style["width"] = "130px";
        if (DCC.Contains("Primary System")) row.Cells[DCC.IndexOf("Primary System")].Style["width"] = "220px";
        if (DCC.Contains("AOR System")) row.Cells[DCC.IndexOf("AOR System")].Style["width"] = "215px";
        if (DCC.Contains("Approved")) row.Cells[DCC.IndexOf("Approved")].Style["width"] = "45px";
        if (DCC.Contains("Approved By")) row.Cells[DCC.IndexOf("Approved By")].Style["width"] = "130px";
        if (DCC.Contains("Approved Date")) row.Cells[DCC.IndexOf("Approved Date")].Style["width"] = "130px";
        if (DCC.Contains("Planned Start")) row.Cells[DCC.IndexOf("Planned Start")].Style["width"] = "85px";
        if (DCC.Contains("Planned End")) row.Cells[DCC.IndexOf("Planned End")].Style["width"] = "85px";
        if (DCC.Contains("Actual Start")) row.Cells[DCC.IndexOf("Actual Start")].Style["width"] = "85px";
        if (DCC.Contains("Actual End")) row.Cells[DCC.IndexOf("Actual End")].Style["width"] = "85px";

        //CR
        if (DCC.Contains("CR Customer Title")) row.Cells[DCC.IndexOf("CR Customer Title")].Style["width"] = "300px";
        if (DCC.Contains("CR Internal Title")) row.Cells[DCC.IndexOf("CR Internal Title")].Style["width"] = "300px";
        if (DCC.Contains("CR Description")) row.Cells[DCC.IndexOf("CR Description")].Style["width"] = "500px";
        if (DCC.Contains("Rationale")) row.Cells[DCC.IndexOf("Rationale")].Style["width"] = "500px";
        if (DCC.Contains("Customer Impact")) row.Cells[DCC.IndexOf("Customer Impact")].Style["width"] = "500px";
        if (DCC.Contains("CR Websystem")) row.Cells[DCC.IndexOf("CR Websystem")].Style["width"] = "150px";
        if (DCC.Contains("CSD Required Now")) row.Cells[DCC.IndexOf("CSD Required Now")].Style["width"] = "100px";
        if (DCC.Contains("Related Release")) row.Cells[DCC.IndexOf("Related Release")].Style["width"] = "75px";
        if (DCC.Contains("Sub Group")) row.Cells[DCC.IndexOf("Sub Group")].Style["width"] = "75px";
        if (DCC.Contains("Design Review")) row.Cells[DCC.IndexOf("Design Review")].Style["width"] = "100px";
        if (DCC.Contains("CR ITI POC")) row.Cells[DCC.IndexOf("CR ITI POC")].Style["width"] = "75px";
        if (DCC.Contains("Customer Priority List")) row.Cells[DCC.IndexOf("Customer Priority List")].Style["width"] = "75px";
        if (DCC.Contains("Government CSRD #")) row.Cells[DCC.IndexOf("Government CSRD #")].Style["width"] = "75px";
        if (DCC.Contains("ITI Priority")) row.Cells[DCC.IndexOf("ITI Priority")].Style["width"] = "75px";
        if (DCC.Contains("Cyber/ISMT")) row.Cells[DCC.IndexOf("Cyber/ISMT")].Style["width"] = "75px";
        if (DCC.Contains("Primary SR")) row.Cells[DCC.IndexOf("Primary SR")].Style["width"] = "75px";
        if (DCC.Contains("Contract")) row.Cells[DCC.IndexOf("Contract")].Style["width"] = "200px";

        //SR
        if (DCC.Contains("SR #")) row.Cells[DCC.IndexOf("SR #")].Style["width"] = "65px";
        if (DCC.Contains("SR Submitted By")) row.Cells[DCC.IndexOf("SR Submitted By")].Style["width"] = "90px";
        if (DCC.Contains("SR Submitted Date")) row.Cells[DCC.IndexOf("SR Submitted Date")].Style["width"] = "100px";
        if (DCC.Contains("SR Keywords")) row.Cells[DCC.IndexOf("SR Keywords")].Style["width"] = "150px";
        if (DCC.Contains("SR Websystem")) row.Cells[DCC.IndexOf("SR Websystem")].Style["width"] = "150px";
        if (DCC.Contains("SR Status")) row.Cells[DCC.IndexOf("SR Status")].Style["width"] = "100px";
        if (DCC.Contains("SR Type")) row.Cells[DCC.IndexOf("SR Type")].Style["width"] = "100px";
        if (DCC.Contains("SR Priority")) row.Cells[DCC.IndexOf("SR Priority")].Style["width"] = "75px";
        if (DCC.Contains("SR LCMB")) row.Cells[DCC.IndexOf("SR LCMB")].Style["width"] = "75px";
        if (DCC.Contains("SR ITI")) row.Cells[DCC.IndexOf("SR ITI")].Style["width"] = "65px";
        if (DCC.Contains("SR ITI POC")) row.Cells[DCC.IndexOf("SR ITI POC")].Style["width"] = "75px";
        if (DCC.Contains("SR Description")) row.Cells[DCC.IndexOf("SR Description")].Style["width"] = "500px";
        if (DCC.Contains("Last Reply")) row.Cells[DCC.IndexOf("Last Reply")].Style["width"] = "75px";

        //Task
        if (DCC.Contains("Affiliated")) row.Cells[DCC.IndexOf("Affiliated")].Style["width"] = "100px";
        if (DCC.Contains("Contract Allocation Assignment")) row.Cells[DCC.IndexOf("Contract Allocation Assignment")].Style["width"] = "150px";
        if (DCC.Contains("Contract Allocation Group")) row.Cells[DCC.IndexOf("Contract Allocation Group")].Style["width"] = "150px";
        if (DCC.Contains("Assigned To")) row.Cells[DCC.IndexOf("Assigned To")].Style["width"] = "100px";
        if (DCC.Contains("Functionality")) row.Cells[DCC.IndexOf("Functionality")].Style["width"] = "150px";
        if (DCC.Contains("Work Activity")) row.Cells[DCC.IndexOf("Work Activity")].Style["width"] = "150px";
        if (DCC.Contains("Organization (Assigned To)")) row.Cells[DCC.IndexOf("Organization (Assigned To)")].Style["width"] = "110px";
        if (DCC.Contains("PDD TDR")) row.Cells[DCC.IndexOf("PDD TDR")].Style["width"] = "75px";
        if (DCC.Contains("Percent Complete")) row.Cells[DCC.IndexOf("Percent Complete")].Style["width"] = "75px";
        if (DCC.Contains("Bus. Rank")) row.Cells[DCC.IndexOf("Bus. Rank")].Style["width"] = "75px";
        if (DCC.Contains("Primary Bus. Resource")) row.Cells[DCC.IndexOf("Primary Bus. Resource")].Style["width"] = "75px";
        if (DCC.Contains("Tech. Rank")) row.Cells[DCC.IndexOf("Tech. Rank")].Style["width"] = "75px";
        if (DCC.Contains("Customer Rank")) row.Cells[DCC.IndexOf("Customer Rank")].Style["width"] = "75px";
        if (DCC.Contains("Assigned To Rank")) row.Cells[DCC.IndexOf("Assigned To Rank")].Style["width"] = "150px";
        if (DCC.Contains("Primary Resource")) row.Cells[DCC.IndexOf("Primary Resource")].Style["width"] = "75px";
        if (DCC.Contains("Priority")) row.Cells[DCC.IndexOf("Priority")].Style["width"] = "75px";
        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["width"] = "75px";
        if (DCC.Contains("Deployment")) row.Cells[DCC.IndexOf("Deployment")].Style["width"] = "75px";
        if (DCC.Contains("Deployment Title")) row.Cells[DCC.IndexOf("Deployment Title")].Style["width"] = "350px";
        if (DCC.Contains("Deployment Start Date")) row.Cells[DCC.IndexOf("Deployment Start Date")].Style["width"] = "75px";
        if (DCC.Contains("Deployment End Date")) row.Cells[DCC.IndexOf("Deployment End Date")].Style["width"] = "75px";
        if (DCC.Contains("Production Status")) row.Cells[DCC.IndexOf("Production Status")].Style["width"] = "75px";
        if (DCC.Contains("Secondary Bus. Resource")) row.Cells[DCC.IndexOf("Secondary Bus. Resource")].Style["width"] = "75px";
        if (DCC.Contains("Secondary Tech. Resource")) row.Cells[DCC.IndexOf("Secondary Tech. Resource")].Style["width"] = "75px";
        if (DCC.Contains("Status")) row.Cells[DCC.IndexOf("Status")].Style["width"] = "75px";
        if (DCC.Contains("Submitted By")) row.Cells[DCC.IndexOf("Submitted By")].Style["width"] = "75px";
        if (DCC.Contains("System(Task)")) row.Cells[DCC.IndexOf("System(Task)")].Style["width"] = "250px";
        if (DCC.Contains("System Suite")) row.Cells[DCC.IndexOf("System Suite")].Style["width"] = "175px";
        if (DCC.Contains("Work Area")) row.Cells[DCC.IndexOf("Work Area")].Style["width"] = "75px";
        if (DCC.Contains("Primary Task")) row.Cells[DCC.IndexOf("Primary Task")].Style["width"] = "75px";
        if (DCC.Contains("Primary Task Title")) row.Cells[DCC.IndexOf("Primary Task Title")].Style["width"] = "350px";
        if (DCC.Contains("Work Task")) row.Cells[DCC.IndexOf("Work Task")].Style["width"] = "75px";
        if (DCC.Contains("Title")) row.Cells[DCC.IndexOf("Title")].Style["width"] = "350px";
        if (DCC.Contains("Work Request")) row.Cells[DCC.IndexOf("Work Request")].Style["width"] = "75px";
        if (DCC.Contains("Resource Group")) row.Cells[DCC.IndexOf("Resource Group")].Style["width"] = "150px";
        if (DCC.Contains("Dev Workload Manager")) row.Cells[DCC.IndexOf("Dev Workload Manager")].Style["width"] = "120px";
        if (DCC.Contains("Bus Workload Manager")) row.Cells[DCC.IndexOf("Bus Workload Manager")].Style["width"] = "120px";
        if (DCC.Contains("WITASKID")) row.Cells[DCC.IndexOf("WITASKID")].Style["display"] = "none";
        if (DCC.Contains("Release AOR")) row.Cells[DCC.IndexOf("Release AOR")].Style["width"] = "500px";
        if (DCC.Contains("PD2TDR/Workload AOR")) row.Cells[DCC.IndexOf("PD2TDR/Workload AOR")].Style["width"] = "500px";
        if (DCC.Contains("Task.Workload.Release Status")) row.Cells[DCC.IndexOf("Task.Workload.Release Status")].Style["width"] = "200px";
        if (DCC.Contains("In Progress Date")) row.Cells[DCC.IndexOf("In Progress Date")].Style["width"] = "75px";
        if (DCC.Contains("Deployed Date")) row.Cells[DCC.IndexOf("Deployed Date")].Style["width"] = "75px";
        if (DCC.Contains("Ready For Review Date")) row.Cells[DCC.IndexOf("Ready For Review Date")].Style["width"] = "75px";

        //Sub-Task
        if (DCC.Contains("Sub-Task")) row.Cells[DCC.IndexOf("Sub-Task")].Style["width"] = "75px";
        if (DCC.Contains("Sub-Task Title")) row.Cells[DCC.IndexOf("Sub-Task Title")].Style["width"] = "350px";
        if (DCC.Contains("Sub-Task Description")) row.Cells[DCC.IndexOf("Sub-Task Description")].Style["width"] = "75px";
        if (DCC.Contains("Sub-Task Assigned To")) row.Cells[DCC.IndexOf("Sub-Task Assigned To")].Style["width"] = "75px";
        if (DCC.Contains("Sub-Task Status")) row.Cells[DCC.IndexOf("Sub-Task Status")].Style["width"] = "75px";
        if (DCC.Contains("Sub-Task Tech. Rank")) row.Cells[DCC.IndexOf("Sub-Task Tech. Rank")].Style["width"] = "75px";
        if (DCC.Contains("Sub-Task Customer Rank")) row.Cells[DCC.IndexOf("Sub-Task Customer Rank")].Style["width"] = "75px";
        if (DCC.Contains("Sub-Task Assigned To Rank")) row.Cells[DCC.IndexOf("Sub-Task Assigned To Rank")].Style["width"] = "75px";
        if (DCC.Contains("Sub-Task Primary Resource")) row.Cells[DCC.IndexOf("Sub-Task Primary Resource")].Style["width"] = "75px";
        if (DCC.Contains("Sub-Task Priority")) row.Cells[DCC.IndexOf("Sub-Task Priority")].Style["width"] = "75px";
        if (DCC.Contains("Sub-Task Percent Complete")) row.Cells[DCC.IndexOf("Sub-Task Percent Complete")].Style["width"] = "130px";
        if (DCC.Contains("Sub-Task Bus. Rank")) row.Cells[DCC.IndexOf("Sub-Task Bus. Rank")].Style["width"] = "75px";
        if (DCC.Contains("Sub-Task SR Number")) row.Cells[DCC.IndexOf("Sub-Task SR Number")].Style["width"] = "75px";

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

            if (CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("Carry In")) row.Cells[DCC.IndexOf("Carry In")].Style["text-align"] = "center";
        if (DCC.Contains("Current Release")) row.Cells[DCC.IndexOf("Current Release")].Style["text-align"] = "center";
        if (DCC.Contains("Rank")) row.Cells[DCC.IndexOf("Rank")].Style["text-align"] = "center";
        if (DCC.Contains("Last Meeting")) row.Cells[DCC.IndexOf("Last Meeting")].Style["text-align"] = "center";
        if (DCC.Contains("Next Meeting")) row.Cells[DCC.IndexOf("Next Meeting")].Style["text-align"] = "center";
        if (DCC.Contains("Visible To Customer")) row.Cells[DCC.IndexOf("Visible To Customer")].Style["text-align"] = "center";
        if (DCC.Contains("Release")) row.Cells[DCC.IndexOf("Release")].Style["text-align"] = "center";
        if (DCC.Contains("Approved")) row.Cells[DCC.IndexOf("Approved")].Style["text-align"] = "center";

        if (DCC.Contains("Tier"))
        {
            row.Cells[DCC.IndexOf("Tier")].Style["text-align"] = "center";

            switch (row.Cells[DCC.IndexOf("Tier")].Text.ToUpper())
            {
                case "A":
                    row.Cells[DCC.IndexOf("Tier")].Style["background-color"] = "red";
                    break;
                case "B":
                    row.Cells[DCC.IndexOf("Tier")].Style["background-color"] = "yellow";
                    break;
                case "C":
                    row.Cells[DCC.IndexOf("Tier")].Style["background-color"] = "green";
                    break;
            }
        }

        if (DCC.Contains("Tier Rank"))
        {
            row.Cells[DCC.IndexOf("Tier Rank")].Style["text-align"] = "center";

            if (DCC.Contains("Tier"))
            {
                switch (row.Cells[DCC.IndexOf("Tier")].Text.ToUpper())
                {
                    case "A":
                        row.Cells[DCC.IndexOf("Tier Rank")].Style["background-color"] = "red";
                        break;
                    case "B":
                        row.Cells[DCC.IndexOf("Tier Rank")].Style["background-color"] = "yellow";
                        break;
                    case "C":
                        row.Cells[DCC.IndexOf("Tier Rank")].Style["background-color"] = "green";
                        break;
                }
            }
        }

        if (DCC.Contains("Cyber/ISMT") && DCC.Contains("CYBERISMT_ID"))
        {
            row.Cells[DCC.IndexOf("Cyber/ISMT")].Controls.Add(CreateCheckBox("CR", row.Cells[DCC.IndexOf("CYBERISMT_ID")].Text, "Cyber/ISMT", row.Cells[DCC.IndexOf("Cyber/ISMT")].Text));
            row.Cells[DCC.IndexOf("Cyber/ISMT")].Style["text-align"] = "center";
        }

        if (DCC.Contains("Resources") && DCC.Contains("RESOURCES_ID"))
        {
            row.Cells[DCC.IndexOf("Resources")].Controls.Add(CreateLink("Resources", row.Cells[DCC.IndexOf("RESOURCES_ID")].Text, "View Resources (" + row.Cells[DCC.IndexOf("Resources")].Text + ")"));
        }

        if (DCC.Contains("WITASKID")) row.Cells[DCC.IndexOf("WITASKID")].Style["display"] = "none";
    }

    private CheckBox CreateCheckBoxNew(Boolean blnHeader, Boolean blnEnabled, string strEntity, string strField, string strFieldID)
    {
        CheckBox chk = new CheckBox();

        if (blnHeader)
        {
            chk.Attributes["onchange"] = "chkAll(this);";
        }
        else
        {
            chk.Attributes["onchange"] = "input_change(this);";
        }
        chk.Attributes.Add("strEntity", strEntity);
        chk.Attributes.Add("strField", strField);
        chk.Attributes.Add("strFieldID", strFieldID);
        chk.Attributes["class"] = "masschange";
        chk.Enabled = blnEnabled;
        return chk;
    }

    private CheckBox CreateCheckBox(string typeName, string typeID, string field, string isChecked)
    {
        CheckBox chk = new CheckBox();

        chk.Attributes["onchange"] = "input_change(this);";
        chk.Attributes.Add("typeName", typeName);
        chk.Attributes.Add("typeID", typeID);
        chk.Attributes.Add("field", field);
        chk.Attributes["class"] = "saveable";
        if (isChecked == "Yes")
        {
            chk.Checked = true;
        }

        return chk;
    }
    private LinkButton CreateLink(string type, string type_ID, string type_Text)
    {
        LinkButton lb = new LinkButton();
        string workItemID = type_ID;
        string taskNumber = string.Empty;
        string taskID = string.Empty;
        string blnSubTask = "0";

        lb.Text = type_Text;
        switch (type)
        {
            case "Task":
                if (type_Text.Contains(" - "))
                {
                    string[] arrWorkItem = type_Text.Split('-');

                    workItemID = arrWorkItem[0].Trim();
                    taskNumber = arrWorkItem[1].Trim();
                    taskID = type_ID;
                    blnSubTask = "1";

                    lb.Attributes["onclick"] = string.Format("openWorkItem('{0}', '{1}', '{2}', {3}); return false;", workItemID, taskNumber, taskID, blnSubTask);
                }
                else
                {

                    lb.Attributes["onclick"] = string.Format("lbEditWorkItem_click('{0}'); return false;", workItemID);
                }
                break;
            case "Resources":

                lb.Attributes["onclick"] = string.Format("linkResouces_click('{0}'); return false;", type_ID);

                break;
            case "CR":
                lb.Attributes["onclick"] = string.Format("openCR('{0}'); return false;", type_ID);
                break;
            case "AttachmentCount":
                if (type_ID.Contains("-"))
                {
                    string[] arrAOR = type_ID.Split('-');

                    string AORID = arrAOR[0].Trim();
                    string AORReleaseID = arrAOR[1].Trim();

                    lb.Attributes["onclick"] = string.Format("openAttachment('{0}',  '{1}'); return false;", AORID, AORReleaseID);
                }
                else
                {
                    lb.Attributes["onclick"] = string.Format("openAttachment('{0}'); return false;", type_ID);
                }
                break;
        }


        return lb;
    }

    private Image CreateImage(bool isHeader)
    {
        Image img = new Image();

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
            if (CurrentLevel == LevelCount)
            {
                //if (ReadOnly)
                //{
                //    img.Attributes["src"] = "Images/Icons/blank.png";
                //}
                //else
                //{
                //    img.Attributes["src"] = "Images/Icons/cog.png";
                //    img.Attributes["title"] = "Grid Settings";
                //    img.Attributes["alt"] = "Grid Settings";
                //    img.Attributes["onclick"] = "openSettings();";
                //}
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

        return img;
    }

    private TextBox CreateTextBox(string typeName, string typeID, string field, string value, bool isNumber)
    {
        string txtValue = Server.HtmlDecode(value).Trim();
        TextBox txt = new TextBox();

        if (field == "Description")
        {
            txt.Wrap = true;
            txt.TextMode = TextBoxMode.MultiLine;

        }
        txt.Text = txtValue;
        txt.MaxLength = 50;
        txt.Width = new Unit(field == "Sort" ? 90 : 95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("typeName", typeName);
        txt.Attributes.Add("typeID", typeID);
        txt.Attributes.Add("field", field);
        txt.Attributes.Add("original_value", txtValue);
        txt.ToolTip = txtValue;
        if (isNumber)
        {
            txt.MaxLength = field == "Rank" ? 2 : 5;
            txt.Style["text-align"] = "center";
        }

        return txt;
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

    private void ExportExcel(DataTable dt)
    {
        DataSet ds = new DataSet();
        DataSet dsCopy = new DataSet();
        DataTable dtCopy = new DataTable();

        //ds.Tables.Add(dt);
        if (Session["AORLevels"] != null) Levels = (XmlDocument)Session["AORLevels"];
        LevelCount = Levels.SelectNodes("crosswalkparameters/level").Count;
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        Worksheet ws = wb.Worksheets[0];
        XmlDocument docLevel = new XmlDocument();
        XmlElement rootLevel = (XmlElement)docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
        var myLevels = Levels.SelectNodes("crosswalkparameters/level");
        int tblCnt = 0;
        foreach (XmlNode nodeLevel in myLevels)
        {

            //XmlNode nodeLevel = Levels.SelectNodes("crosswalkparameters/level")[CurrentLevel - 1];
            XmlNode nodeImport = docLevel.ImportNode(nodeLevel, true);
            rootLevel.AppendChild(nodeImport);

            XmlDocument docFilters = new XmlDocument();
            XmlElement rootFilters = (XmlElement)docFilters.AppendChild(docFilters.CreateElement("filters"));

            HtmlSelect ReleaseItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item0");
            HtmlSelect ContractItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item1");
            HtmlSelect StatusItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item9");
            HtmlSelect WorkloadAllocationItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item9a");
            HtmlSelect AORTypeControl = (HtmlSelect)Page.Master.FindControl("ms_Item10");
            HtmlSelect VisibleToCustomerItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item10a");
            HtmlSelect ContainsTasksItemControl = (HtmlSelect)Page.Master.FindControl("ms_Item2");
            HtmlInputCheckBox ArchiveCheckBoxCtrl = (HtmlInputCheckBox)Page.Master.FindControl("chk_Item11");

            List<string> listRelease = new List<string>();
            List<string> listAORType = new List<string>();
            List<string> listVisibleToCusotmer = new List<string>();
            List<string> listContainsTasks = new List<string>();
            List<string> listContract = new List<string>();
            List<string> listTaskStatus = new List<string>();
            List<string> listAORProductionStatus = new List<string>();

            if (ReleaseItemControl != null && ReleaseItemControl.Items.Count > 0)
                foreach (ListItem li in ReleaseItemControl.Items)
                    if (li.Selected) listRelease.Add(li.Value);

            if (ContractItemControl != null && ContractItemControl.Items.Count > 0)
                foreach (ListItem li in ContractItemControl.Items)
                    if (li.Selected) listContract.Add(li.Value);

            if (StatusItemControl != null && StatusItemControl.Items.Count > 0)
                foreach (ListItem li in StatusItemControl.Items)
                    if (li.Selected) listTaskStatus.Add(li.Value);

            if (WorkloadAllocationItemControl != null && WorkloadAllocationItemControl.Items.Count > 0)
                foreach (ListItem li in WorkloadAllocationItemControl.Items)
                    if (li.Selected) listAORProductionStatus.Add(li.Value);

            if (AORTypeControl != null && AORTypeControl.Items.Count > 0)
                foreach (ListItem li in AORTypeControl.Items)
                    if (li.Selected) listAORType.Add(li.Value);

            if (VisibleToCustomerItemControl != null && VisibleToCustomerItemControl.Items.Count > 0)
                foreach (ListItem li in VisibleToCustomerItemControl.Items)
                    if (li.Selected) listVisibleToCusotmer.Add(li.Value);

            if (ContainsTasksItemControl != null && ContainsTasksItemControl.Items.Count > 0)
                foreach (ListItem li in ContainsTasksItemControl.Items)
                    if (li.Selected) listContainsTasks.Add(li.Value);


            dt = AOR.AOR_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, qfRelease: String.Join(",", listRelease), qfAORType: String.Join(",", listAORType), AORID_Filter_arr: String.Join(",", AORID_Filter_arr), qfVisibleToCustomer: String.Join(",", listVisibleToCusotmer), qfContainsTasks: String.Join(",", listContainsTasks), qfContract: String.Join(",", listContract), qfTaskStatus: String.Join(",", listTaskStatus), qfAORProductionStatus: String.Join(",", listAORProductionStatus), qfShowArchiveAOR: ArchiveCheckBoxCtrl.Checked ? "1" : "0", getColumns: false);

            foreach (DataColumn c in dt.Columns.Cast<DataColumn>().ToList())
                if (c.ColumnName.ToString() == "X" || c.ColumnName.ToString() == "Y" || c.ColumnName.ToString() == "Z")
                    dt.Columns.Remove(c.ColumnName);

            var colNames = (from dc in dt.Columns.Cast<DataColumn>() select dc.ColumnName).Where(x => x != "X" && x != "Y" && x != "Z" && x.IndexOf("_ID", StringComparison.Ordinal) == -1).ToList();

            tblCnt += 1;

            //dt.SetConcatCols(Session[GridSessionKey].ToString(), tblCnt);
            dt.SetSortOrder(Session[GridSessionKey].ToString(), tblCnt);
            dt.SetColumnOrder(Session[GridSessionKey].ToString(), colNames.ToArray(), tblCnt);
            dt = dt.DefaultView.ToTable();
            //dtCopy = dt.Copy();
            //string tblNameCopy = "'" + tblCnt;
            //dtCopy.TableName = tblNameCopy;
            //dsCopy.Tables.Add(dtCopy);

            //dt.SetColNames(Session[GridSessionKey].ToString(), tblCnt);
            dt.AcceptChanges();
            string tblName = "'" + tblCnt;
            dt.TableName = tblName;
            ds.Tables.Add(dt);


        }

        //int dtRowCount = 0;
        int count = ds.Tables.Count;
        object[] filterCols = new object[0];
        object[] curfilterCols = new object[0];

        dtColumnCnt = 0;
        int colCnt = 0;
        for (int k = 0; k < ds.Tables.Count; k++)
        {
            if (colCnt > dtColumnCnt)
            {
                dtColumnCnt = colCnt;
            }
            colCnt = 0;
            if (k > 0)
            {
                foreach (DataColumn column in ds.Tables[k].Columns)
                {

                    if (column.ToString().EndsWith("_ID"))
                    {

                    }
                    else
                    {
                        int blnKeep = 1;
                        for (int n = 0; n < ds.Tables[k - 1].Columns.Count; n++)
                        {
                            if (ds.Tables[k - 1].Columns[n] != null)
                            {
                                if (ds.Tables[k - 1].Columns[n].ToString() == column.ToString())
                                {
                                    blnKeep = 0;
                                }
                            }
                        }
                        if (blnKeep == 1)
                        {
                            colCnt++;
                        }
                    }
                }
            }
        }
        //ws.AutoFitColumns();
        AddRowsColumns(ws, 0, 0, filterCols, curfilterCols, ds.Tables[0], ds, 0);

        //ws.Cells.ImportDataTable(dt, true, 0, 0, false, false);

        MemoryStream ms = new MemoryStream();
        wb.Save(ms, SaveFormat.Xlsx);

        Response.ContentType = "application/xlsx";
        Response.AddHeader("content-disposition", "attachment; filename=" + "AOR Grid" + ".xlsx");
        Response.BinaryWrite(ms.ToArray());

        Response.End();
    }

    private void AddRowsColumns(Worksheet ws, int startRow, int endRow, object[] remFilterCols, object[] curfilterCols, DataTable dt, DataSet ds, int count)
    {
        count++;
        string curCell = "";
        int curP = 0;
        Aspose.Cells.Style curStyle = new Aspose.Cells.Style();
        StyleFlag flag = new StyleFlag() { All = true };
        if (count > ds.Tables.Count)
        {

        }
        else
        {
            DataSet newDS = ds.Copy();
            DataTable dtFiltered = dt.Copy();
            string rowFilter = "";
            for (int j = 0; j < curfilterCols.Length; j++)
            {
                if (rowFilter == "")
                {
                    rowFilter = curfilterCols[j].ToString();
                }
                else
                {
                    if (curfilterCols[j].ToString() != "")
                    {
                        rowFilter = rowFilter + " and " + curfilterCols[j];
                    }
                }
            }
            DataView dv = dt.DefaultView;
            dv.RowFilter = rowFilter;
            dtFiltered = dv.ToTable();

            object[] removeFilterCols = new object[dtFiltered.Columns.Count];
            foreach (DataColumn column in dtFiltered.Columns)
            {
                if (column.ToString().EndsWith("_ID"))
                {

                }
                else
                {
                    removeFilterCols[column.Ordinal] = column.ToString();
                }
            }

            for (int n = 0; n < remFilterCols.Length; n++)
            {
                if (remFilterCols[n] != null)
                {

                    dtFiltered.Columns.Remove(remFilterCols[n].ToString());

                }
            }
            //dtFiltered.SetSortOrder(Session[GridSessionKey].ToString(), count);
            dtFiltered.SetColNames(Session[GridSessionKey].ToString(), count);
            dt = dt.DefaultView.ToTable();
            if (count != 2)
            {
                rowCount++;
            }
            int m = 0;
            int curColCnt = 0;
            foreach (DataColumn column in dtFiltered.Columns)
            {

                if (column.ToString().EndsWith("_ID"))
                {

                }
                else
                {
                    //int blnKeep = 1;
                    //for (int n = 0; n < remFilterCols.Length; n++)
                    //{
                    //    if (remFilterCols[n] != null)
                    //    {
                    //        if (remFilterCols[n].ToString() == column.ToString())
                    //        {
                    //            blnKeep = 0;
                    //        }
                    //    }
                    //}
                    //if (blnKeep == 1)
                    //{
                    curColCnt++;
                    if (count != 2)
                    {
                        string cellName = "";
                        char uniSel = (char)(uniA + m);
                        cellName = (char)uniSel + "";
                        if (m > 26)
                        {
                            cellName = (char)(uniA + 0) + "" + (char)uniSel;
                        }
                        if (m > 52)
                        {
                            cellName = (char)(uniA + 1) + "" + (char)uniSel;
                        }
                        cellName = cellName + (rowCount).ToString();
                        curCell = cellName;
                        curP = m;
                        ws.Cells[cellName].PutValue(column.ToString());
                        // Getting the Style object for the cell
                        Aspose.Cells.Style headerStyle = new Aspose.Cells.Style();

                        // Setting Style properties like border, alignment etc.
                        headerStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                        headerStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                        headerStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                        headerStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                        headerStyle.Pattern = BackgroundType.Solid;
                        headerStyle.Font.Name = "Calibri";
                        headerStyle.Font.Size = 11;
                        switch (count)
                        {
                            case 1:
                                headerStyle.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#F4B084");
                                headerStyle.Font.IsBold = true;
                                break;
                            case 2:
                                headerStyle.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#0070C0");
                                headerStyle.Font.Color = System.Drawing.Color.White;
                                headerStyle.Font.IsBold = true;
                                break;
                            case 3:
                                headerStyle.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#C9C9C9");
                                break;
                            case 4:
                                headerStyle.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#BDD7EE");
                                break;
                            default:
                                headerStyle.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#EDEDED");
                                break;
                        }
                        // Setting the style of the cell with the customized Style object
                        curStyle = headerStyle;
                        ws.Cells[cellName].SetStyle(headerStyle);
                        m++;
                    }
                    //}

                }
            }
            if (count != 2)
            {
                if (curColCnt == 1)
                {

                    string cellName = "";
                    char uniSel = (char)(uniA + dtColumnCnt - 1);
                    cellName = (char)uniSel + "";
                    if (dtColumnCnt - 1 > 26)
                    {
                        cellName = (char)(uniA + 0) + "" + (char)uniSel;
                    }
                    if (dtColumnCnt - 1 > 52)
                    {
                        cellName = (char)(uniA + 1) + "" + (char)uniSel;
                    }
                    cellName = cellName + (rowCount).ToString();
                    // Create a range
                    Range range = ws.Cells.CreateRange(curCell + ":" + cellName);
                    range.ApplyStyle(curStyle, flag);

                    // Merge range into a single cell
                    range.Merge();
                }
            }

            dtRowCount += endRow;
            foreach (DataRow row in dtFiltered.Rows)
            {
                int startCnt = 0;

                object[] newfilterCols = new object[dtFiltered.Columns.Count];
                //object[] removeFilterCols = new object[dtFiltered.Columns.Count];
                //filterCols = curfilterCols;
                if (count == 2)
                {
                    rowCount++;
                    int p = 0;
                    foreach (DataColumn column in dtFiltered.Columns)
                    {

                        if (column.ToString().EndsWith("_ID"))
                        {

                        }
                        else
                        {
                            //    int blnKeep = 1;
                            //    for (int q = 0; q < remFilterCols.Length; q++)
                            //    {
                            //        if (remFilterCols[q] != null)
                            //        {
                            //            if (remFilterCols[q].ToString() == column.ToString())
                            //            {
                            //                blnKeep = 0;
                            //            }
                            //        }
                            //    }
                            //    if (blnKeep == 1)
                            //    {
                            string cellName = "";
                            char uniSel = (char)(uniA + p);
                            cellName = (char)uniSel + "";
                            if (p > 26)
                            {
                                cellName = (char)(uniA + 0) + "" + (char)uniSel;
                            }
                            if (p > 52)
                            {
                                cellName = (char)(uniA + 1) + "" + (char)uniSel;
                            }
                            cellName = cellName + (rowCount).ToString();
                            curCell = cellName;
                            curP = p;
                            ws.Cells[cellName].PutValue(column.ToString());
                            // Getting the Style object for the cell
                            Aspose.Cells.Style headerStyle = new Aspose.Cells.Style();

                            // Setting Style properties like border, alignment etc.
                            headerStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                            headerStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                            headerStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                            headerStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                            headerStyle.Pattern = BackgroundType.Solid;
                            headerStyle.Font.Name = "Calibri";
                            headerStyle.Font.Size = 11;
                            switch (count)
                            {
                                case 1:
                                    headerStyle.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#F4B084");
                                    headerStyle.Font.IsBold = true;
                                    break;
                                case 2:
                                    headerStyle.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#0070C0");
                                    headerStyle.Font.Color = System.Drawing.Color.White;
                                    headerStyle.Font.IsBold = true;
                                    break;
                                case 3:
                                    headerStyle.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#C9C9C9");
                                    break;
                                case 4:
                                    headerStyle.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#BDD7EE");
                                    break;
                                default:
                                    headerStyle.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#EDEDED");
                                    break;
                            }
                            curStyle = headerStyle;
                            // Setting the style of the cell with the customized Style object
                            ws.Cells[cellName].SetStyle(headerStyle);
                            p++;
                        }
                        //    }
                    }
                }
                if (count == 2)
                {
                    if (curColCnt == 1)
                    {

                        string cellName = "";
                        char uniSel = (char)(uniA + dtColumnCnt - 1);
                        cellName = (char)uniSel + "";
                        if (dtColumnCnt - 1 > 26)
                        {
                            cellName = (char)(uniA + 0) + "" + (char)uniSel;
                        }
                        if (dtColumnCnt - 1 > 52)
                        {
                            cellName = (char)(uniA + 1) + "" + (char)uniSel;
                        }
                        cellName = cellName + (rowCount).ToString();
                        // Create a range
                        Range range = ws.Cells.CreateRange(curCell + ":" + cellName);
                        range.ApplyStyle(curStyle, flag);

                        // Merge range into a single cell
                        range.Merge();
                    }
                }
                startCnt = rowCount;
                rowCount++;
                int i = 0;
                foreach (DataColumn column in dtFiltered.Columns)
                {
                    object item = row[column];

                    if (column.ToString().EndsWith("_ID"))
                    {
                        //if (count > 1)
                        //{
                        var colObj = " isnull([" + column.ToString() + "], 0) =  '" + (item.ToString() == "" ? "0" : item.ToString()) + "'";
                        newfilterCols[column.Ordinal] = colObj;
                        //}
                    }
                    else
                    {
                        //removeFilterCols[column.Ordinal] = column.ToString();
                        newfilterCols[column.Ordinal] = "";
                        //int blnKeepThis = 1;
                        //for (int n = 0; n < remFilterCols.Length; n++)
                        //{
                        //    if (remFilterCols[n] != null)
                        //    {
                        //        if (remFilterCols[n].ToString() == column.ToString())
                        //        {
                        //            blnKeepThis = 0;
                        //        }
                        //    }
                        //}
                        //if (blnKeepThis == 1)
                        //{
                        //string cellName = (char)(uniA + i) + (rowCount).ToString();
                        //ws.Cells[cellName].PutValue(item.ToString());
                        string cellName = "";
                        char uniSel = (char)(uniA + i);
                        cellName = (char)uniSel + "";
                        if (i > 26)
                        {
                            cellName = (char)(uniA + 0) + "" + (char)uniSel;
                        }
                        if (i > 52)
                        {
                            cellName = (char)(uniA + 1) + "" + (char)uniSel;
                        }
                        cellName = cellName + (rowCount).ToString();
                        curCell = cellName;
                        curP = i;
                        ws.Cells[cellName].PutValue(Uri.UnescapeDataString(item.ToString()));
                        // Getting the Style object for the cell
                        Aspose.Cells.Style style = new Aspose.Cells.Style();
                        style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                        style.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                        style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                        style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                        style.Pattern = BackgroundType.Solid;
                        style.Font.Name = "Calibri";
                        style.Font.Size = 11;
                        style.VerticalAlignment = TextAlignmentType.Top;
                        switch (count)
                        {
                            case 1:
                                style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#F4B084");
                                style.Font.IsBold = true;
                                break;
                            case 3:
                                style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#EDEDED");
                                break;
                        }
                        //if (column.ToString().Contains("Description") || column.ToString().Contains("Title") || column.ToString().Contains("Notes") || column.ToString().Contains("Rationale") || column.ToString().Contains("Customer Impact"))
                        if (Uri.UnescapeDataString(item.ToString()).Length > 50)
                        {
                            // Setting Style properties like border, alignment etc.
                            style.IsTextWrapped = true;
                            // Setting the style of the cell with the customized Style object
                            ws.Cells.SetColumnWidth(i, 50);
                        }
                        else
                        {
                            //ws.AutoFitColumn(i);
                        }
                        curStyle = style;
                        ws.Cells[cellName].SetStyle(style);
                        i++;
                        //}
                    }


                }
                if (curColCnt == 1)
                {
                    string cellName = "";
                    char uniSel = (char)(uniA + dtColumnCnt - 1);
                    cellName = (char)uniSel + "";
                    if (dtColumnCnt - 1 > 26)
                    {
                        cellName = (char)(uniA + 0) + "" + (char)uniSel;
                    }
                    if (dtColumnCnt - 1 > 52)
                    {
                        cellName = (char)(uniA + 1) + "" + (char)uniSel;
                    }
                    cellName = cellName + (rowCount).ToString();
                    // Create a range
                    Range range = ws.Cells.CreateRange(curCell + ":" + cellName);
                    range.ApplyStyle(curStyle, flag);

                    // Merge range into a single cell
                    range.Merge();
                }
                if (count < ds.Tables.Count)
                {
                    AddRowsColumns(ws, startCnt, dtFiltered.Rows.Count + 1, removeFilterCols, newfilterCols, ds.Tables[count], newDS, count);

                }
                if (startCnt > 0)
                {
                    if (count != ds.Tables.Count)
                    {
                        if (ws.Cells.MaxDataRow >= startCnt + 1 && ws.Cells.MaxDataRow >= rowCount - 1)
                        {
                            ws.Cells.GroupRows(startCnt + 1, rowCount - 1, false);
                        }
                    }
                }
            }

            // Setting SummaryRowBelow property to false
            ws.Outline.SummaryRowBelow = false;
            // ws.Cells.SetColumnWidth(5, 255);
        }

        //}
    }
    #endregion

    public static DataTable GetViewOptions(string gridview)
    {
        var UserId = UserManagement.GetUserId_FromUsername();
        return WTSData.GetViewOptions(UserId, gridview);

    }

    #region AJAX
    [WebMethod]
    public static string DeleteAOR(string aor)
    {
        Dictionary<string, string> result = new Dictionary<string, string> { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            var AOR_ID = 0;
            int.TryParse(aor, out AOR_ID);

            deleted = AOR.AOR_Delete(AOR_ID);
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

    [WebMethod]
    public static string SaveChanges(string changes)
    {
        var result = new Dictionary<string, string> { { "saved", "" }, { "error", "" } };
        var saved = false;
        var errorMsg = string.Empty;

        try
        {
            XmlDocument docChanges = (XmlDocument)JsonConvert.DeserializeXmlNode(changes, "changes");

            saved = AOR.AOR_Update(docChanges);
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

    [WebMethod]
    public static bool SaveQuickFiltersToSession(string settings)
    {
        var _saved = true;
        var errorMsg = string.Empty;

        try
        {
            setSessionQF(settings);
        }
        catch (Exception ex)
        {
            _saved = false;
            LogUtility.LogException(ex);
            errorMsg = ex.Message + "; " + ex.StackTrace;
        }
        return _saved;
    }


    [WebMethod]
    public static bool SaveQuickFiltersToDB(string currGridViewid, string settings, int processView)
    {
        dynamic d = JsonConvert.DeserializeObject(settings);
        var _saved = true;
        var errorMsg = string.Empty;

        try
        {
            int gridViewId;
            var gv_itiSettings = new GridView();
            int.TryParse(currGridViewid, out gridViewId);
            gv_itiSettings.GridNameID = 11;
            gv_itiSettings.ID = gridViewId;
            gv_itiSettings.Name = d.viewname;
            gv_itiSettings.UserID = processView == 1 ? 0 : UserManagement.GetUserId_FromUsername();
            gv_itiSettings.Tier1Columns = settings;
            gv_itiSettings.ViewType = 1;
            _saved = gv_itiSettings.Save(out errorMsg);
        }
        catch (Exception ex)
        {
            _saved = false;
            LogUtility.LogException(ex);
            errorMsg = ex.Message + "; " + ex.StackTrace;
        }

        return _saved;
    }

    [WebMethod(EnableSession = true)]
    public static bool UpdateSession(string args, string gridSessionKey)
    {
        //Update the session grid key
        var sessionMethods = new SessionMethods();
        sessionMethods.Session[gridSessionKey] = args;
        return true;
    }

    [WebMethod(EnableSession = true)]
    public static string GetTier1Data(int gridViewId, string gridview)
    {
        var dt = GetViewOptions(gridview);

        foreach (DataRow row in dt.Rows)
            if (row["GridViewID"].ToString() == gridViewId.ToString())
            {
                var sessionMethods = new SessionMethods();
                sessionMethods.Session["ddlGridViewValue"] = gridViewId;
                //Set the QF session to the view you are selecting
                setSessionQF(row["Tier1Columns"].ToString());
                return ValidateItiSettings(row["Tier1Columns"].ToString());
            }

        return null;
    }

    /// <summary>
    /// With continuous changes to the AOR table names the view's setting data(Tier1Columns) can have corrupt names. This function will ensure exclusive valid columns appear only.
    /// </summary>
    /// <param name="itiSettings"></param>
    /// <returns></returns>
    private static string ValidateItiSettings(string itiSettings)
    {
        var ModifiedTblColGroups = new JArray();
        var itiSettings_all_sess = JsonConvert.DeserializeObject<dynamic>(itiSettings);
        var columnOrder = JsonConvert.DeserializeObject<JArray>(itiSettings_all_sess.columnorder.ToString());
        var newAorProperties = new List<AorProperty>();
        var docLevel = new XmlDocument();
        var docFilters = new XmlDocument();
        docFilters.AppendChild(docFilters.CreateElement("filters"));
        docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
        var defaultCrosswalk_dt = AOR.AOR_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, qfRelease: "", qfAORType: "", AORID_Filter_arr: "", qfVisibleToCustomer: "", qfContainsTasks: "", qfContract: "", qfTaskStatus: "", qfAORProductionStatus: "", qfShowArchiveAOR: "0", getColumns: true);
        var jsonUpdated = false;

        //Get the modified and crosswalk TblColGroups
        foreach (var currModifiedTblColGroup_DB in defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray[0]).Distinct().ToArray())
            ModifiedTblColGroups.Add(currModifiedTblColGroup_DB.ToString());

        itiSettings_all_sess.columngroups = ModifiedTblColGroups;
        itiSettings_all_sess.validated = DateTime.Now;

        //Ensure all the required TblColGroups are going to be displayed
        foreach (var currCroswalkTblColGroup in defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
        {
            var itemExists = false;
            //Search through all possible tblCols for the matching itiSetting tblCol
            foreach (var itiSettings_all_tblcol in itiSettings_all_sess.tblCols)
            {
                if (itiSettings_all_tblcol.name.ToString().ToUpper() == currCroswalkTblColGroup[1].ToString().ToUpper())
                {
                    itiSettings_all_tblcol.colgroup = currCroswalkTblColGroup[0].ToString();
                    jsonUpdated = true;
                    itemExists = true;
                    break;
                }
            }
            //If the items doesnt exist create it
            if (!itemExists)
            {
                var newProperty = new AorProperty();

                newProperty.name = currCroswalkTblColGroup[1].ToString();
                newProperty.alias = "";
                newProperty.show = false;
                newProperty.sortorder = "none";
                newProperty.sortpriority = "";
                newProperty.groupname = "";
                newProperty.concat = false;
                newProperty.colgroup = currCroswalkTblColGroup[0].ToString();

                newAorProperties.Add(newProperty);
                columnOrder.Add((columnOrder.Count + 1).ToString());
            }
        }

        //Add new AOR properties if missing to the itiSettings and update itiSetting's tblCols and columnorder
        if (jsonUpdated || newAorProperties.Count > 0)
        {
            var aorProperties = JsonConvert.DeserializeObject<List<AorProperty>>(itiSettings_all_sess.tblCols.ToString());
            if (newAorProperties.Count > 0)
                foreach (var item in newAorProperties)
                    aorProperties.Add(item);

            itiSettings_all_sess.tblCols = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(aorProperties));
            itiSettings_all_sess.columnorder = columnOrder;
        }

        //Check and set up the tblCols and columnOrder for the subgrids
        foreach (var itiSettings_subgrid_sess in itiSettings_all_sess)
            if (itiSettings_subgrid_sess.Name.IndexOf("subgrid") != -1)
            {
                jsonUpdated = false;
                foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
                {
                    newAorProperties.Clear();
                    columnOrder = JsonConvert.DeserializeObject(itiSettings_subgrid_prop_sess[0].columnorder.ToString());

                    //Redo the columnOrder
                    foreach (var currDefaultCrosswalk_tblcol in defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
                    {
                        var itemExists = false;
                        foreach (var itiSettings_subgrid_tblCol in itiSettings_subgrid_prop_sess[0].tblCols)
                            if (itiSettings_subgrid_tblCol.name.ToString().ToUpper() == currDefaultCrosswalk_tblcol[1].ToString().ToUpper())
                            {
                                itiSettings_subgrid_tblCol.colgroup = currDefaultCrosswalk_tblcol[0].ToString();
                                jsonUpdated = true;
                                itemExists = true;
                                break;
                            }

                        if (!itemExists)
                        {
                            var newProperty = new AorProperty();

                            newProperty.name = currDefaultCrosswalk_tblcol[1].ToString();
                            newProperty.alias = "";
                            newProperty.show = false;
                            newProperty.sortorder = "none";
                            newProperty.sortpriority = "";
                            newProperty.groupname = "";
                            newProperty.concat = false;
                            newProperty.colgroup = currDefaultCrosswalk_tblcol[0].ToString();

                            newAorProperties.Add(newProperty);
                            columnOrder.Add((columnOrder.Count + 1).ToString());
                        }
                    }

                    //Add the missing required tblCols to our tblCols
                    if (jsonUpdated || newAorProperties.Count > 0)
                    {
                        var aorProperties = JsonConvert.DeserializeObject<List<AorProperty>>(itiSettings_subgrid_prop_sess[0].tblCols.ToString());
                        if (newAorProperties.Count > 0)
                            foreach (var i in newAorProperties)
                                aorProperties.Add(i);
                        itiSettings_subgrid_prop_sess[0].tblCols = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(aorProperties));
                        itiSettings_subgrid_prop_sess[0].columnorder = columnOrder;
                    }
                }
            }

        string[] oldColCount = JsonConvert.DeserializeObject<string[]>(itiSettings_all_sess.columnorder.ToString());
        var newColCount = defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray[1]).Distinct().Count();
        var colDiff = oldColCount.Length - newColCount;

        if (colDiff > 0)
        {
            var validCols = new JArray();

            foreach (var currTblCol_sess in itiSettings_all_sess.tblCols)
                foreach (var currTblCol_DB in defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
                    if (currTblCol_sess.name.ToString().ToUpper() == currTblCol_DB[1].ToString().ToUpper())
                    {
                        validCols.Add(currTblCol_sess);
                        break;
                    }

            string colName;
            var newColOrder = new List<string>();

            for (int i = 0; i < itiSettings_all_sess.columnorder.Count; i++)
            {
                colName = "";
                for (int j = 0; j < itiSettings_all_sess.tblCols.Count; j++)
                    if (itiSettings_all_sess.columnorder[i] == j + 1) colName = itiSettings_all_sess.tblCols[j].name;

                if (!string.IsNullOrEmpty(colName))
                    for (int k = 0; k < validCols.Count; k++)
                        if (colName == validCols[k].First.First.ToString())
                            newColOrder.Add((k + 1).ToString());
            }

            itiSettings_all_sess.tblCols = JArray.FromObject(validCols);
            itiSettings_all_sess.columnorder = JArray.FromObject(newColOrder);

            foreach (var itiSettings_subgrid_sess in itiSettings_all_sess)
                if (itiSettings_subgrid_sess.Name.IndexOf("subgrid") != -1)
                {
                    validCols = new JArray();
                    newColOrder.Clear();

                    foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
                    {
                        foreach (var currTblCol_Sess in itiSettings_subgrid_prop_sess[0].tblCols)
                            foreach (var currTblCol_DB in defaultCrosswalk_dt.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
                                if (currTblCol_Sess.name.ToString().ToUpper() == currTblCol_DB[1].ToString().ToUpper())
                                {
                                    validCols.Add(currTblCol_Sess);
                                    break;
                                }

                        for (int i = 0; i < itiSettings_subgrid_prop_sess[0].columnorder.Count; i++)
                        {
                            colName = "";
                            for (int j = 0; j < itiSettings_subgrid_prop_sess[0].tblCols.Count; j++)
                                if (itiSettings_subgrid_prop_sess[0].columnorder[i] == j + 1)
                                {
                                    colName = itiSettings_subgrid_prop_sess[0].tblCols[j].name;
                                }

                            if (!string.IsNullOrEmpty(colName))
                                for (int k = 0; k < validCols.Count; k++)
                                    if (colName == validCols[k].First.First.ToString())
                                        newColOrder.Add((k + 1).ToString());
                        }

                        itiSettings_subgrid_prop_sess[0].tblCols = JArray.FromObject(validCols);
                        itiSettings_subgrid_prop_sess[0].columnorder = JArray.FromObject(newColOrder);
                    }
                }
        }


        if (itiSettings_all_sess is JArray) return JsonConvert.SerializeObject(itiSettings_all_sess);
        return JsonConvert.SerializeObject(itiSettings_all_sess);
    }

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