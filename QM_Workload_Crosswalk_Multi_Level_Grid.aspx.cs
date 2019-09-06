﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Aspose.Cells;  //for exporting to excel
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;

public partial class QM_Workload_Crosswalk_Multi_Level_Grid : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool CanEditWorkloadMGMT = false;
    protected int CurrentLevel = 1;
    protected int GridPageSize = 25;
    protected string Filter = string.Empty;
    protected string[] QFStatus = { };
    protected string[] QFAffiliated;
    protected bool QFBusinessReview = false;
    DataTable _dtOptions = null;
    DataTable _dtWorkArea = null;
    DataTable _dtAORLookup = null;
    DataSet dsOptions = null;
    //protected bool IsConfigured = false;
    private XmlDocument Levels = new XmlDocument();
    private DataColumnCollection DCC;
    protected int LevelCount = 0;
    protected int GridPageIndex = 0;
    protected bool _myData = true;
    string _UserDDLChange = "no";
    string listOfResources = "";
    protected bool _export = false;
    protected int rowCount = 0;
    protected int dtRowCount = 0;
    protected int dtColumnCnt = 0;
    protected int uniA = (int)'A';
    protected int gridRowCnt = 0;
    protected DataTable DDLViewOptionsDT;
    protected int UserID = UserManagement.GetUserId_FromUsername();
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        loadLookupData();
        ReadSession();
        ReadQueryString();
        InitializeEvents();
        LoadQF();
        loadMenus();
        loadQF_Affiliated();
        _dtOptions = WorkloadItem.GetAllLookupTables();
        dsOptions = WorkloadItem.GetAvailableOptions();
        if (dsOptions.Tables.Contains("WorkArea"))
        {
            _dtWorkArea = dsOptions.Tables["WorkArea"];
            //Work Area options are based on System, done on client side
            Page.ClientScript.RegisterArrayDeclaration("arrWorkArea", JsonConvert.SerializeObject(dsOptions.Tables["WorkArea"], Newtonsoft.Json.Formatting.None));
        }
        _dtAORLookup = AOR.AORTaskOptionsList_Get(AssignedToID: 0, PrimaryResourceID: 0, SystemID: 0, SystemAffiliated: 1, ResourceAffiliated: 0, All: 1);
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewWorkItem = this.CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);
        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);
        this.CanEditWorkloadMGMT = UserManagement.UserCanEdit(WTSModuleOption.WorkloadMGMT);

        HtmlSelect ddlStatus = (HtmlSelect)Page.Master.FindControl("ms_Item0");
        HtmlSelect ddlAffiliated = (HtmlSelect)Page.Master.FindControl("ms_Item1");
        DropDownList ddlGridPageSize = (DropDownList)Page.Master.FindControl("ddlItem5");
        if ( this.CurrentLevel > 1)
        {
            grdData.PageSize = 12;

            if(this.GridPageSize == 0)
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
        else
        {

            grdData.PageSize = 25;
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

        DataTable dt = LoadData();
        gridRowCnt = dt.Rows.Count;
        if (dt != null)
        {
            this.DCC = dt.Columns;
        }



        if (Session["Crosswalk_GridView_ML"] != null) Session["Crosswalk_GridView_ML"] = ddlGridview.SelectedItem.ToString();
        else Session["Crosswalk_GridView_ML"] = WTSData.GetDefaultGridViewName(10, UserID);

        if (_export)
        {
            ExportExcel(dt);
        }
        else
        {
            if (this.CurrentLevel == 1 && !Page.IsPostBack && dt.Rows.Count > 0 && !DCC.Contains("Work Task") && !DCC.Contains("Primary Task"))
            {
                XmlNode nodeLevel = this.Levels.SelectNodes("crosswalkparameters/level")[this.CurrentLevel - 1];
                int intColspan = nodeLevel.SelectNodes("breakout/column").Count + 1;
                int columnCount = 0;
                DataRow newRow = dt.NewRow();

                for (int i = 0; i <= DCC.Count - 1; i++)
                {
                    string columnDBName = DCC[i].ColumnName.ToString();
                    Boolean visible = DCC[i].ColumnName.EndsWith("_ID") ? false : true;
                    if (DCC[i].ColumnName == "Y")
                    {
                        DCC[i].AllowDBNull = true;
                        visible = false;
                    }
                    if (DCC[i].ColumnName == "ROW_ID")
                    {
                        newRow[columnDBName] = "0";
                    }
                    if (visible)
                    {
                        columnCount = columnCount + 1;
                    }

                    if (columnCount > intColspan && visible)
                    {
                        if (DCC.Contains("Affiliated"))
                        {
                            if(columnDBName != "Z")
                            {
                                int sumAffilliated = 0;
                                int sumAssigned = 0;
                                for (int j = 0; j <= dt.Rows.Count - 1; j++)
                                {
                                    sumAssigned += Convert.ToInt32(dt.Rows[j][columnDBName].ToString().Split(new string[] { "||" }, StringSplitOptions.None)[0]);
                                    sumAffilliated += Convert.ToInt32(dt.Rows[j][columnDBName].ToString().Split(new string[] { "||" }, StringSplitOptions.None)[1]);

                                }
                                newRow[columnDBName] = sumAssigned + " || " + sumAffilliated;
                            }
                        }
                        else {
                            newRow[columnDBName] = dt.Compute("Sum([" + columnDBName + "])", "");
                        }
                    }
                }
                dt.Rows.InsertAt(newRow, 0);
            }
            grdData.DataSource = dt;


            if (!Page.IsPostBack && this.GridPageIndex > 0 && this.GridPageIndex < ((decimal)dt.Rows.Count / (decimal)25)) grdData.PageIndex = this.GridPageIndex;

            grdData.DataBind();
        }
    }

    private void loadLookupData()
    {
        WTS_User currUser = new WTS_User(UserID);
        currUser.Load();
        DDLViewOptionsDT = WTSData.GetViewOptions(userId: UserID, gridName: "Workload Crosswalk");

        if (DDLViewOptionsDT != null && DDLViewOptionsDT.Rows.Count > 0)
        {
            ddlGridview.Items.Clear();

            ListItem item = null;
            foreach (DataRow row in DDLViewOptionsDT.Rows)
            {
                if (row["ViewName"].ToString() != "-- New Gridview --")
                {
                    var tempVal = row["WTS_RESOURCEID"].ToString();
                    item = new ListItem();
                    item.Text = row["ViewName"].ToString();
                    item.Value = row["GridViewID"].ToString();
                    item.Attributes.Add("OptionGroup", row["WTS_RESOURCEID"].ToString() != "" ? "Custom Views" : "Process Views");
                    item.Attributes.Add("MyView", row["MyView"].ToString());
                    item.Attributes.Add("Tier1RollupGroup", row["Tier1RollupGroup"].ToString());
                    item.Attributes.Add("Tier1ColumnOrder", row["Tier1ColumnOrder"].ToString());
                    item.Attributes.Add("Tier2ColumnOrder", row["Tier2ColumnOrder"].ToString());
                    item.Attributes.Add("DefaultSortType", row["Tier2SortOrder"].ToString());
                    item.Attributes.Add("SectionsXML", row["SectionsXML"].ToString());

                    ddlGridview.Items.Add(item);
                }
            }
        }

        if (CurrentLevel > 1)
        {
            tblGridView.Visible = false;
        }
        if (Session["defaultCrosswalkGrid_ML"] != null && !string.IsNullOrWhiteSpace(Session["defaultCrosswalkGrid_ML"].ToString()))
        {
            ListItem itemGridView = ddlGridview.Items.FindByText(Session["defaultCrosswalkGrid_ML"].ToString());
            if (itemGridView != null)
            {
                itemGridView.Selected = true;
                itisettings.Value = Session["defaultCrosswalkGrid_ML"].ToString();
            }
        }
    }

    private int getDefaultGridViewId()
    {
        var cmdText = "select settingvalue from usersetting where GridNameID = " + 10 + " AND wts_resourceid = " + UserID;

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        using (SqlCommand cmd = new SqlCommand(cmdText, cn))
        {
            cmd.CommandType = CommandType.Text;
            cn.Open();

            return Convert.ToInt32(cmd.ExecuteScalar() ?? 145);
        }
    }

    private void ReadSession()
    {
        if (Session["Levels"] != null)
        {
            this.Levels = (XmlDocument)Session["Levels"];
        }

        this.LevelCount = this.Levels.SelectNodes("crosswalkparameters/level").Count;

        if (this.LevelCount < 1)
        {//If this is not configure then load the defaultView for the currentUser
            int userDefaultViewID = getDefaultGridViewId();
            DataRow[] result = DDLViewOptionsDT.Select("GridViewID = " + userDefaultViewID.ToString());
            if (result.Length == 0)
            {//Then the users setting for default view doesnt exist so we will use the view named 'Default'
                result = DDLViewOptionsDT.Select("ViewName = 'Default'");
            }
            string defaultXML = result[0].ItemArray[result[0].Table.Columns["SectionsXML"].Ordinal].ToString();
            Levels.LoadXml(defaultXML);
            Session["Levels"] = Levels;
            this.LevelCount = this.Levels.SelectNodes("crosswalkparameters/level").Count;
            ddlGridview.SelectedValue = userDefaultViewID.ToString();
        }
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
            || Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
        {
            this.MyData = true;
            _myData = true;
        }
        else
        {
            this.MyData = false;
            _myData = false;
        }

        if (Request.QueryString["CurrentLevel"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"]))
        {
            int.TryParse(Request.QueryString["CurrentLevel"], out this.CurrentLevel);
        }
        if (this.CurrentLevel > 1)
        {
            GridPageSize = 12;
        }
        else
        {
            GridPageSize = 25;
        }

        if (Request.QueryString["GridPageSize"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageSize"]))
        {
            int.TryParse(Request.QueryString["GridPageSize"], out this.GridPageSize);
            Session["GridPageSize" + this.CurrentLevel.ToString()] = Request.QueryString["GridPageSize"];
        }
        else if (Session["GridPageSize" + this.CurrentLevel.ToString()] != null)
        {
            int.TryParse(Session["GridPageSize" + this.CurrentLevel.ToString()].ToString(), out this.GridPageSize);
        }

        if (Request.QueryString["Filter"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Filter"]))
        {
            this.Filter = Uri.UnescapeDataString(Request.QueryString["Filter"]);
        }

        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"]))
        {
            this.QFStatus = Request.QueryString["SelectedStatuses"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Session["SelectedStatuses" + this.CurrentLevel.ToString()] = Request.QueryString["SelectedStatuses"];
        }
        else if (Session["SelectedStatuses" + this.CurrentLevel.ToString()] != null)
        {
            this.QFStatus = Session["SelectedStatuses" + this.CurrentLevel.ToString()].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["SelectedAssigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAssigned"]))
        {
            this.QFAffiliated = Request.QueryString["SelectedAssigned"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Session["SelectedAssigned" + this.CurrentLevel.ToString()] = Request.QueryString["SelectedAssigned"];
        }
        else if (Session["SelectedAssigned" + this.CurrentLevel.ToString()] != null)
        {
            this.QFAffiliated = Session["SelectedAssigned" + this.CurrentLevel.ToString()].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.MyData = false;
            _myData = false;
        }

        if (Request.QueryString["filterSelectedAssigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["filterSelectedAssigned"].ToString()) && (Request.QueryString["SelectedAssigned"] == Request.QueryString["filterSelectedAssigned"] || Request.QueryString["SelectedAssigned"] == null))
        {

            this.QFAffiliated = Server.UrlDecode(Request.QueryString["filterSelectedAssigned"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.MyData = false;
            _myData = false;
        }

        if (Request.QueryString["BusinessReview"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["BusinessReview"]))
        {
            bool br = false;
            bool.TryParse(Request.QueryString["BusinessReview"].ToString(), out br);
            this.QFBusinessReview = br;
            Session["BusinessReview" + this.CurrentLevel.ToString()] = Request.QueryString["BusinessReview"];
        }
        else if (Session["BusinessReview" + this.CurrentLevel.ToString()] != null)
        {
            bool br = false;
            bool.TryParse(Session["BusinessReview" + this.CurrentLevel.ToString()].ToString(), out br);
            this.QFBusinessReview = br;
        }

        if (Request.QueryString["GridPageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndex"]))
        {
            int.TryParse(Request.QueryString["GridPageIndex"], out this.GridPageIndex);
        }

        _UserDDLChange = "no";
        if (Request.QueryString["UserDDLChange"] != null)
        {
            _UserDDLChange = Request.QueryString["UserDDLChange"];
        }

        if (Request.QueryString["Export"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);
        }
    }

    private void loadMenus()
    {
        try
        {
            DataSet dsMenu = new DataSet();
            dsMenu.ReadXml(this.Server.MapPath("XML/WTS_Menus.xml"));

            if (dsMenu.Tables.Count > 0 && dsMenu.Tables[0].Rows.Count > 0)
            {
                if (dsMenu.Tables.Contains("QMWorkloadGridRelatedItem"))
                {
                    menuRelatedItems.DataSource = dsMenu.Tables["QMWorkloadGridRelatedItem"];
                    menuRelatedItems.DataValueField = "URL";
                    menuRelatedItems.DataTextField = "Text";
                    menuRelatedItems.DataIDField = "id";
                    if (dsMenu.Tables["QMWorkloadGridRelatedItem"].Columns.Contains("QMWorkloadGridRelatedItem_id_0"))
                    {
                        menuRelatedItems.DataParentIDField = "QMWorkloadGridRelatedItem_id_0";
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
    private void LoadQF()
    {
        DataTable dtStatus = MasterData.StatusList_Get();
        HtmlSelect ddlStatus = (HtmlSelect)Page.Master.FindControl("ms_Item0");
        DropDownList ddlGridPageSize = (DropDownList)Page.Master.FindControl("ddlItem5");
        HtmlInputCheckBox BusinessReviewCheckBoxCtrl = (HtmlInputCheckBox)Page.Master.FindControl("chk_Item11");

        Label lblStatus = (Label)Page.Master.FindControl("lblms_Item0");
        lblStatus.Text = "Status: ";
        lblStatus.Style["width"] = "150px";

        if (dtStatus != null)
        {
            ddlStatus.Items.Clear();
            dtStatus.DefaultView.RowFilter = "StatusType = 'Work'";
            dtStatus = dtStatus.DefaultView.ToTable(true, new string[] { "StatusID", "Status" });

            foreach (DataRow dr in dtStatus.Rows)
            {
                ListItem li = new ListItem(dr["Status"].ToString(), dr["StatusID"].ToString());
                li.Selected = (QFStatus.Count() == 0 || QFStatus.Contains(dr["StatusID"].ToString()));

                if (QFStatus.Count() == 0 && (li.Text == "Approved/Closed" || li.Text == "Closed" || li.Text == "On Hold")) li.Selected = false;

                ddlStatus.Items.Add(li);
            }
        }

        Label lblPageSize = (Label)Page.Master.FindControl("lblItem5");
        lblPageSize.Text = "Page Size: ";
        lblPageSize.Style["width"] = "50px";

        ddlGridPageSize.Items.Clear();
        ListItem item = new ListItem("12", "12");
        ddlGridPageSize.Items.Add(item);
        item = new ListItem("25", "25");
        ddlGridPageSize.Items.Add(item);
        item = new ListItem("50", "50");
        ddlGridPageSize.Items.Add(item);
        if (this.CurrentLevel > 1)
        {
            item = new ListItem("All", "0");
            ddlGridPageSize.Items.Add(item);
        }
        ddlGridPageSize.SelectedValue = this.GridPageSize.ToString();

        Label BusinessReviewCheckBoxLabelCtrl = (Label)Page.Master.FindControl("lblms_Item11");
        BusinessReviewCheckBoxLabelCtrl.Text = "Business Review Requested: ";
        BusinessReviewCheckBoxLabelCtrl.Style["width"] = "250px";
        BusinessReviewCheckBoxCtrl.Checked = QFBusinessReview;
    }

    private void loadQF_Affiliated()
    {
        List<string> Affiliated = new List<string>();
        bool blnBacklog = false;

        if (QFAffiliated != null && QFAffiliated.Length > 0)
        {
            foreach (string s in QFAffiliated)
            {
                if (s == "69")
                {
                    blnBacklog = true;
                }
                Affiliated.Add(s.Trim());
            }
        }

        DataTable dtAffiliated = UserManagement.LoadUserList();  // userNameSearch: userName

        Label lblAffiliated = (Label)Page.Master.FindControl("lblms_Item1");
        lblAffiliated.Text = "Affiliated: ";
        lblAffiliated.Style["width"] = "150px";

        HtmlSelect ddlAffiliated = (HtmlSelect)Page.Master.FindControl("ms_Item1");

        if (dtAffiliated != null)
        {
            ddlAffiliated.Items.Clear();
            ListItem item = null;

            foreach (DataRow dr in dtAffiliated.Rows)
            {
                if (dr["WTS_RESOURCEID"] == null || string.IsNullOrWhiteSpace(dr["WTS_RESOURCEID"].ToString()))
                {
                    continue;
                }
                else
                {
                    item = new ListItem(dr["UserName"].ToString(), dr["WTS_RESOURCEID"].ToString());

                    if (dr["RESOURCETYPE"].ToString() == "Not People")
                    {
                        item.Attributes.Add("OptionGroup", "Non-Resources");
                        item.Selected = false;
                    }
                    else
                    {
                        item.Attributes.Add("OptionGroup", "Resources");
                    }
                    //_myData >> If true, only check current user as assigned
                    if (_UserDDLChange == "no" && _myData)
                    {
                        if (dr["WTS_RESOURCEID"].ToString() == UserID.ToString())
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                        ddlAffiliated.Items.Add(item);
                    }
                    else
                    {
                        item.Selected = ((Affiliated.Count == 0 && item.Text != "IT.Backlog") || Affiliated.Contains(dr["WTS_RESOURCEID"].ToString()));
                        ddlAffiliated.Items.Add(item);
                    }
                }
            }
        }

    }

    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

            if (IsPostBack && this.CurrentLevel == 1 && Session["dtLevel" + this.CurrentLevel] != null)
            {
                dt = (DataTable)Session["dtLevel" + this.CurrentLevel];
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
                        value.InnerText = (arrValues[1].ToString().Trim() == ""
                            && field.InnerText.ToUpper() != "INPROGRESSDATE_ID"
                             && field.InnerText.ToUpper() != "DEPLOYEDDATE_ID"
                              && field.InnerText.ToUpper() != "READYFORREVIEWDATE_ID"
                                && field.InnerText.ToUpper() != "CREATED_DATE_ID"
                                  && field.InnerText.ToUpper() != "UPDATED_DATE_ID"
                                    && field.InnerText.ToUpper() != "NEEDED_DATE_ID" ? "0" : arrValues[1].ToString().Trim());

                        switch (field.InnerText.ToUpper())
                        {
                            case "PRIMARYBUSRANK_ID":
                            case "PRIMARYTECHRANK_ID":
                            case "SRNUMBER_ID":
                                if (arrValues[1].ToString().Trim() == "") value.InnerText = "-999";
                                break;
                        }

                        filter.AppendChild(field);
                        filter.AppendChild(value);
                        rootFilters.AppendChild(filter);
                    }
                }

                HtmlSelect ddlStatus = (HtmlSelect)Page.Master.FindControl("ms_Item0");
                HtmlSelect ddlAffiliated = (HtmlSelect)Page.Master.FindControl("ms_Item1");

                List<string> listStatus = new List<string>();

                if (ddlStatus != null && ddlStatus.Items.Count > 0)
                {
                    foreach (ListItem li in ddlStatus.Items)
                    {
                        if (li.Selected) listStatus.Add(li.Value);
                    }
                }
                List<string> listAffiliated = new List<string>();

                if (ddlAffiliated != null && ddlAffiliated.Items.Count > 0)
                {
                    foreach (ListItem li in ddlAffiliated.Items)
                    {
                        if (li.Selected) listAffiliated.Add(li.Value);
                    }
                }

                dt = Workload.QM_Workload_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, qfStatus: String.Join(",", listStatus), qfAffiliated: String.Join(",", listAffiliated), qfBusinessReview: QFBusinessReview, myData: this.MyData);

                listOfResources = String.Join(",", listAffiliated);
                bool AssignedResCol = false;
                //bool AssignedResAndToCol = false;
                //bool AffiliatedCol = false;
                //bool PrimaryBusCol = false;
                //bool PrimaryTechCol = false;
                //bool SecondaryBusCol = false;
                //bool SecondaryTechCol = false;

                //if (dt.Columns.Contains("Affiliated_ID"))
                //{
                //    AffiliatedCol = true;
                //}
                 if(dt.Columns.Contains("AssignedTo_ID") && !dt.Columns.Contains("PrimaryTask_ID") && !dt.Columns.Contains("WorkTask_ID"))
                {
                    AssignedResCol = true;
                }
                // if (dt.Columns.Contains("[AssignedResourceID AssignedTo]"))
                //{
                //    AssignedResAndToCol = true;
                //}
                // if (dt.Columns.Contains("PrimaryTechResource_ID"))
                //{
                //    PrimaryTechCol = true;
                //}

                // if (dt.Columns.Contains("PrimaryBusResource_ID"))
                //{
                //    PrimaryBusCol = true;
                //}
                // if (dt.Columns.Contains("SecondaryBusResource_ID"))
                //{
                //    SecondaryBusCol = true;
                //}
                // if (dt.Columns.Contains("SecondaryTechResource_ID"))
                //{
                //    SecondaryTechCol = true;
                //}

                try
                {
                    //Add AOR Resource Team users the selected users are associated with
                    DataTable dtResourceTeamUsers = AOR.AORResourceTeamList_Get();

                    for (int i = 0; i < listAffiliated.Count; i++)
                    {
                        string aorResourceTeamUserIDs = string.Join(",",
                            dtResourceTeamUsers.AsEnumerable()
                            .Where(cols => cols.Field<int>("WTS_RESOURCEID").ToString() == listAffiliated[i].Trim())
                            .Select(cols => cols.Field<int>("ResourceTeamUserID")));

                        if (aorResourceTeamUserIDs.Length > 0) listOfResources = listOfResources.Length > 0 ? (listOfResources + "," + aorResourceTeamUserIDs) : aorResourceTeamUserIDs;
                    }
                }
                catch (Exception) { }

                string strFilters = "";
                //--------------------------------------------------------------------------------------
                if (AssignedResCol)
                    strFilters = "AssignedTo_ID IN (" + listOfResources + ")";
                //if (AssignedResAndToCol)
                //{
                //    if (strFilters == "")
                //        strFilters = "[AssignedResourceID AssignedTo] IN (" + listOfResources + ")";
                //    else
                //        strFilters = strFilters + " OR [AssignedResourceID AssignedTo] IN (" + listOfResources + ")";
                //}
                //if (AffiliatedCol)
                //{
                //    if (strFilters == "")
                //        strFilters = "Affiliated_ID IN (" + listOfResources + ")";
                //    else
                //        strFilters = strFilters + " OR Affiliated_ID IN (" + listOfResources + ")";
                //}
                //if (PrimaryTechCol)
                //{
                //    if (strFilters == "")
                //        strFilters = "PrimaryTechResource_ID IN (" + listOfResources + ")";
                //    else
                //        strFilters = strFilters + " OR PrimaryTechResource_ID IN (" + listOfResources + ")";
                //}
                //if (PrimaryBusCol)
                //{
                //    if (strFilters == "")
                //        strFilters = "PrimaryBusResource_ID IN (" + listOfResources + ")";
                //    else
                //        strFilters = strFilters + " OR PrimaryBusResource_ID IN (" + listOfResources + ")";
                //}
                //if (SecondaryBusCol)
                //{
                //    if (strFilters == "")
                //        strFilters = "SecondaryBusResource_ID IN (" + listOfResources + ")";
                //    else
                //        strFilters = strFilters + " OR SecondaryBusResource_ID IN (" + listOfResources + ")";
                //}
                //if (SecondaryTechCol)
                //{
                //    if (strFilters == "")
                //        strFilters = "SecondaryTechResource_ID IN (" + listOfResources + ")";
                //    else
                //        strFilters = strFilters + " OR SecondaryTechResource_ID IN (" + listOfResources + ")";
                //}


                dt.DefaultView.RowFilter = strFilters;
                dt = dt.DefaultView.ToTable();

                Session["dtLevel" + this.CurrentLevel] = dt;
            }

        return dt;
    }
    #endregion

    #region Grid

    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);
        if (DCC.Contains("X")) row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage(true, row.RowIndex));

    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {

        GridViewRow row = e.Row;

        FormatRowDisplay(ref row);
        string workitemid = "";
        string workitem = "";
        if (DCC.Contains("AOR") && DCC.Contains("AOR_ID"))
        {
            row.Cells[DCC.IndexOf("AOR")].Style["text-align"] = "left";
        }
        if (DCC.Contains("X")) row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage(false, row.RowIndex));
        if ((DCC.Contains("WorkTask_ID") && DCC.Contains("Work Task")) || (DCC.Contains("PrimaryTask_ID") && DCC.Contains("Primary Task") ))
        {

            if (DCC.Contains("PrimaryTask_ID") && DCC.Contains("Primary Task"))
            {
                workitemid = row.Cells[DCC.IndexOf("PrimaryTask_ID")].Text;
                workitem = row.Cells[DCC.IndexOf("Primary Task")].Text;
                row.Attributes.Add("workitem_id", row.Cells[DCC.IndexOf("Primary Task")].Text);
                row.Cells[DCC.IndexOf("Primary Task")].Style["width"] = "75px";
                row.Cells[DCC.IndexOf("Primary Task")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("PrimaryTask_ID")].Text, row.Cells[DCC.IndexOf("Primary Task")].Text));
            }
            if (DCC.Contains("WorkTask_ID") && DCC.Contains("Work Task"))
            {
                if (this.CanViewWorkItem && row.Cells[DCC.IndexOf("Work Task")].Text != "&nbsp;")
                {
                    workitemid = row.Cells[DCC.IndexOf("WorkTask_ID")].Text;
                    workitem = row.Cells[DCC.IndexOf("Work Task")].Text;
                    row.Attributes.Add("workitem_id", row.Cells[DCC.IndexOf("Work Task")].Text);
                    row.Cells[DCC.IndexOf("Work Task")].Style["width"] = "90px";
                    row.Cells[DCC.IndexOf("Work Task")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("WorkTask_ID")].Text, row.Cells[DCC.IndexOf("Work Task")].Text));
                }
            }
            if (this.CanEditWorkItem)
            {

                string id = "", value = "";
                //todo: rebuild work activity if displayed, when option changed
                if (DCC.Contains("AOR_RELEASE_MGMT_ID") && DCC.Contains("AOR RELEASE/DEPLOYMENT MGMT"))
                {
                    DataTable dtTemp = _dtAORLookup;
                    dtTemp.DefaultView.RowFilter = string.Format(" WTS_SYSTEMID = {0} ", row.Cells[DCC.IndexOf("SystemTask_ID")].Text);
                    dtTemp = dtTemp.DefaultView.ToTable();

                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "0")
                    {
                        row.Cells[DCC.IndexOf("AOR RELEASE/DEPLOYMENT MGMT")].Style["width"] = "55px";
                        row.Cells[DCC.IndexOf("AOR RELEASE/DEPLOYMENT MGMT")].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf("AOR RELEASE/DEPLOYMENT MGMT")].Controls.Add(LoadAORs(workitemid, workitem, dtTemp, "Release/Deployment MGMT", row.Cells[DCC.IndexOf("CascadeAOR_RELEASE_MGMT_ID")].Text, row.Cells[DCC.IndexOf("AOR_RELEASE_MGMT_ID")].Text, row.Cells[DCC.IndexOf("AOR RELEASE/DEPLOYMENT MGMT")].Text, row.Cells[DCC.IndexOf("AORRelease_RELEASE_MGMT_ID")].Text, row.Cells[DCC.IndexOf("ASSIGNEDTO_ID")].Text, row.Cells[DCC.IndexOf("PRIMARYTECHRESOURCE_ID")].Text, row.Cells[DCC.IndexOf("SystemTask_ID")].Text, row.Cells[DCC.IndexOf("PRODUCTIONSTATUS_ID")].Text));
                    }
                }
                if (DCC.Contains("AOR_WORKLOAD_MGMT_ID") && DCC.Contains("AOR WORKLOAD MGMT"))
                {
                    DataTable dtTemp = _dtAORLookup;
                    dtTemp.DefaultView.RowFilter = string.Format(" WTS_SYSTEMID = {0} ", row.Cells[DCC.IndexOf("SystemTask_ID")].Text);
                    dtTemp = dtTemp.DefaultView.ToTable();
                    row.Cells[DCC.IndexOf("AOR WORKLOAD MGMT")].Style["width"] = "55px";
                    row.Cells[DCC.IndexOf("AOR WORKLOAD MGMT")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("AOR WORKLOAD MGMT")].Controls.Add(LoadAORs(workitemid, workitem, dtTemp, "Workload MGMT", row.Cells[DCC.IndexOf("CascadeAOR_WORKLOAD_MGMT_ID")].Text, row.Cells[DCC.IndexOf("AOR_WORKLOAD_MGMT_ID")].Text, row.Cells[DCC.IndexOf("AOR WORKLOAD MGMT")].Text, row.Cells[DCC.IndexOf("AORRelease_WORKLOAD_MGMT_ID")].Text, row.Cells[DCC.IndexOf("ASSIGNEDTO_ID")].Text, row.Cells[DCC.IndexOf("PRIMARYTECHRESOURCE_ID")].Text, row.Cells[DCC.IndexOf("SystemTask_ID")].Text, row.Cells[DCC.IndexOf("PRODUCTIONSTATUS_ID")].Text));
                }

                if (DCC.Contains("PercentComplete_ID") && DCC.Contains("Percent Complete"))
                {
                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'PERCENT COMPLETE'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    row.Cells[DCC.IndexOf("Percent Complete")].Style["width"] = "55px";
                    row.Cells[DCC.IndexOf("Percent Complete")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Percent Complete")].Controls.Add(CreateDropDownList(dtTemp, "Percent Complete", "FIELD_NM", "FIELD_ID",  workitemid, workitem, row.Cells[DCC.IndexOf("Percent Complete")].Text, row.Cells[DCC.IndexOf("Percent Complete")].Text));
                }

                if (DCC.Contains("PrimaryBusRank_ID") && DCC.Contains("Customer Rank"))
                {
                    row.Cells[DCC.IndexOf("Customer Rank")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Customer Rank")].Style["width"] = "20px";
                    row.Cells[DCC.IndexOf("Customer Rank")].Controls.Add(CreateTextBox(workitemid, workitem, "Customer Rank", row.Cells[DCC.IndexOf("Customer Rank")].Text, true));
                }

                if (DCC.Contains("PrimaryTechRank_ID") && DCC.Contains("Tech. Rank"))
                {
                    row.Cells[DCC.IndexOf("Tech. Rank")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Tech. Rank")].Style["width"] = "20px";
                    row.Cells[DCC.IndexOf("Tech. Rank")].Controls.Add(CreateTextBox(workitemid, workitem, "Tech. Rank", row.Cells[DCC.IndexOf("Tech. Rank")].Text, true));
                }

                if (DCC.Contains("AssignedToRank_ID") && DCC.Contains("Assigned To Rank"))
                {
                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'RANK'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    row.Cells[DCC.IndexOf("Assigned To Rank")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Assigned To Rank")].Style["width"] = "125px";
                    row.Cells[DCC.IndexOf("Assigned To Rank")].Controls.Add(CreateDropDownList(dtTemp, "Assigned To Rank", "FIELD_NM", "FIELD_ID", workitemid, workitem, row.Cells[DCC.IndexOf("AssignedToRank_ID")].Text, row.Cells[DCC.IndexOf("Assigned To Rank")].Text));
                }

                if (DCC.Contains("WORKTASK_ID") && DCC.Contains("Title") && row.Cells[DCC.IndexOf("Work Task")].Text != "&nbsp;")
                {
                    row.Cells[DCC.IndexOf("Title")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Title")].Style["width"] = "500px";
                    row.Cells[DCC.IndexOf("Title")].Controls.Add(CreateTextBox(workitemid, workitem, "Title", row.Cells[DCC.IndexOf("Title")].Text, false));
                }
                if (DCC.Contains("PrimaryTASK_ID") && DCC.Contains("Primary Task Title"))
                {
                    row.Cells[DCC.IndexOf("Primary Task Title")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Primary Task Title")].Style["width"] = "500px";
                    row.Cells[DCC.IndexOf("Primary Task Title")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("PrimaryTask_ID")].Text, row.Cells[DCC.IndexOf("Primary Task")].Text, "Title", row.Cells[DCC.IndexOf("Primary Task Title")].Text, false));
                }
                if (DCC.Contains("SRNUMBER_ID") && DCC.Contains("SR NUMBER"))
                {
                    row.Cells[DCC.IndexOf("SR NUMBER")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("SR NUMBER")].Style["width"] = "25px";
                    row.Cells[DCC.IndexOf("SR NUMBER")].Controls.Add(CreateTextBox(workitemid, workitem, "SR NUMBER", row.Cells[DCC.IndexOf("SR NUMBER")].Text, true));
                }

                if (DCC.Contains("ASSIGNEDTO_ID") && DCC.Contains("Assigned To"))
                {
                    id = row.Cells[DCC.IndexOf("ASSIGNEDTO_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("Assigned To")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("Assigned To")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'USER'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    row.Cells[DCC.IndexOf("Assigned To")].Style["width"] = "145px";
                    row.Cells[DCC.IndexOf("Assigned To")].Controls.Add(CreateDropDownList(dtTemp, "Assigned Resource", "FIELD_NM", "FIELD_ID", workitemid, workitem, row.Cells[DCC.IndexOf("ASSIGNEDTO_ID")].Text.Trim(), row.Cells[DCC.IndexOf("Assigned To")].Text.Trim()));
                }

                if (DCC.Contains("PRIMARYBUSRESOURCE_ID") && DCC.Contains("Primary Bus. Resource"))
                {
                    id = row.Cells[DCC.IndexOf("PRIMARYBUSRESOURCE_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("Primary Bus. Resource")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("Primary Bus. Resource")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'USER'");
                    dtTemp = dtTemp.DefaultView.ToTable();

                    row.Cells[DCC.IndexOf("Primary Bus. Resource")].Style["width"] = "145px";
                    row.Cells[DCC.IndexOf("Primary Bus. Resource")].Controls.Add(CreateDropDownList(dtTemp, "Primary Bus Resource", "FIELD_NM", "FIELD_ID", workitemid, workitem, id, value));
                }

                if (DCC.Contains("PRIMARYTECHRESOURCE_ID") && DCC.Contains("Primary Resource"))
                {
                    id = row.Cells[DCC.IndexOf("PRIMARYTECHRESOURCE_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("Primary Resource")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("Primary Resource")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'USER'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    row.Cells[DCC.IndexOf("Primary Resource")].Style["width"] = "145px";
                    row.Cells[DCC.IndexOf("Primary Resource")].Controls.Add(CreateDropDownList(dtTemp, "Primary Resource", "FIELD_NM", "FIELD_ID", workitemid, workitem, id, value));
                }

                if (DCC.Contains("SECONDARYBUSRESOURCE_ID") && DCC.Contains("Secondary Bus. Resource"))
                {
                    id = row.Cells[DCC.IndexOf("SECONDARYBUSRESOURCE_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("Secondary Bus. Resource")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("Secondary Bus. Resource")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'USER'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    row.Cells[DCC.IndexOf("Secondary Bus. Resource")].Style["width"] = "145px";
                    row.Cells[DCC.IndexOf("Secondary Bus. Resource")].Controls.Add(CreateDropDownList(dtTemp, "Secondary Bus Resource", "FIELD_NM", "FIELD_ID", workitemid, workitem, id, value));
                }

                if (DCC.Contains("SECONDARYTECHRESOURCE_ID") && DCC.Contains("Secondary Tech. Resource"))
                {
                    id = row.Cells[DCC.IndexOf("SECONDARYTECHRESOURCE_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("Secondary Tech. Resource")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("Secondary Tech. Resource")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'USER'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    row.Cells[DCC.IndexOf("Secondary Tech. Resource")].Style["width"] = "145px";
                    row.Cells[DCC.IndexOf("Secondary Tech. Resource")].Controls.Add(CreateDropDownList(dtTemp, "Secondary Tech Resource", "FIELD_NM", "FIELD_ID", workitemid, workitem, id, value));
                }

                if (DCC.Contains("CONTRACTALLOCATIONASSIGNMENT_ID") && DCC.Contains("CONTRACT ALLOCATION ASSIGNMENT"))
                {
                    id = row.Cells[DCC.IndexOf("CONTRACTALLOCATIONASSIGNMENT_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("CONTRACT ALLOCATION ASSIGNMENT")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("CONTRACT ALLOCATION ASSIGNMENT")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'ALLOCATION ASSIGNMENT'");
                    dtTemp = dtTemp.DefaultView.ToTable();

                    string WorkTaskID = workitemid;
                    string WorkTask = workitem;
                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "1")
                    {
                        WorkTaskID = workitem.Split('-')[0];
                        WorkTask = workitem.Split('-')[0];
                    }
                    row.Cells[DCC.IndexOf("CONTRACT ALLOCATION ASSIGNMENT")].Style["width"] = "200px";
                    row.Cells[DCC.IndexOf("CONTRACT ALLOCATION ASSIGNMENT")].Controls.Add(CreateDropDownList(dtTemp, "ALLOCATION ASSIGNMENT", "FIELD_NM", "FIELD_ID", WorkTaskID, WorkTask, id, value));
                }

                if (DCC.Contains("CONTRACTALLOCATIONGROUP_ID") && DCC.Contains("CONTRACT ALLOCATION ASSIGNMENT"))
                {
                    id = row.Cells[DCC.IndexOf("CONTRACTALLOCATIONGROUP_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("CONTRACT ALLOCATION GROUP")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("CONTRACT ALLOCATION GROUP")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'ALLOCATION GROUP'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    string WorkTaskID = workitemid;
                    string WorkTask = workitem;
                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "1")
                    {
                        WorkTaskID = workitem.Split('-')[0];
                        WorkTask = workitem.Split('-')[0];
                    }
                    row.Cells[DCC.IndexOf("CONTRACT ALLOCATION GROUP")].Style["width"] = "200px";
                    row.Cells[DCC.IndexOf("CONTRACT ALLOCATION GROUP")].Controls.Add(CreateDropDownList(dtTemp, "ALLOCATION GROUP", "FIELD_NM", "FIELD_ID", WorkTaskID, WorkTask, id, value));
                }

                if (DCC.Contains("FUNCTIONALITY_ID") && DCC.Contains("FUNCTIONALITY"))
                {
                    id = row.Cells[DCC.IndexOf("FUNCTIONALITY_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("FUNCTIONALITY")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("FUNCTIONALITY")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'FUNCTIONALITY'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    string WorkTaskID = workitemid;
                    string WorkTask = workitem;
                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "1")
                    {
                        WorkTaskID = workitem.Split('-')[0];
                        WorkTask = workitem.Split('-')[0];
                    }
                    row.Cells[DCC.IndexOf("FUNCTIONALITY")].Style["width"] = "145px";
                    row.Cells[DCC.IndexOf("FUNCTIONALITY")].Controls.Add(CreateDropDownList(dtTemp, "FUNCTIONALITY", "FIELD_NM", "FIELD_ID", WorkTaskID, WorkTask, id, value));
                }

                //if (DCC.Contains("ORGANIZATION_ID") && DCC.Contains("ORGANIZATION (ASSIGNED TO)"))
                //{
                //    id = row.Cells[DCC.IndexOf("ORGANIZATION_ID")].Text.Replace("&nbsp;", " ").Trim();
                //    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                //    value = row.Cells[DCC.IndexOf("ORGANIZATION (ASSIGNED TO)")].Text.Replace("&nbsp;", " ").Trim();
                //    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                //    row.Cells[DCC.IndexOf("ORGANIZATION (ASSIGNED TO)")].Style["text-align"] = "center";

                //    DataTable dtTemp = _dtOptions;
                //    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'ORGANIZATION'");
                //    dtTemp = dtTemp.DefaultView.ToTable();
                //    row.Cells[DCC.IndexOf("ORGANIZATION (ASSIGNED TO)")].Controls.Add(CreateDropDownList(dtTemp, "ORGANIZATION", "FIELD_NM", "FIELD_ID", WorkTaskID, WorkTask, id, value));
                //}

                //todo: filter by release/deployment mgmt aor and group by pd2tdr phase
                if (DCC.Contains("WORKACTIVITY_ID") && DCC.Contains("WORK ACTIVITY"))
                {


                    id = row.Cells[DCC.IndexOf("WORKACTIVITY_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("WORK ACTIVITY")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    int WorkTaskID = 0;
                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "1")
                    {
                         int.TryParse(workitem.Split('-')[0], out WorkTaskID);
                    }
                    else
                    {
                        int.TryParse(workitem, out WorkTaskID);
                    }

                    DropDownList ddlWorkActivity = new DropDownList();
                    ddlWorkActivity.Attributes.Add("field", "Work Activity");
                    ddlWorkActivity.Attributes.Add("work_item_id", row.Cells[DCC.IndexOf("WorkTask_ID")].Text);
                    ddlWorkActivity.Attributes.Add("bln_sub_task", blnSubTask);
                    ddlWorkActivity.Style["background-color"] = "#F5F6CE";
                    ddlWorkActivity.Style["width"] = "95%";

                    if (dsOptions.Tables.Contains("WorkItemType"))
                    {
                        DataTable dtWorkItemType = dsOptions.Tables["WorkItemType"];
                        int workloadAllocationID = 0;
                        //string currentPhaseID = string.Empty;
                        string currentWorkActivityGroup = string.Empty;
                        ListItem li;
                        DataTable dtAOR = AOR.AORTaskAORList_Get(TaskID: (WorkTaskID == 0 ? -1 : WorkTaskID));

                        dtAOR.DefaultView.RowFilter = "AORWorkTypeID = 2";
                        dtAOR = dtAOR.DefaultView.ToTable();

                        if (dtAOR.Rows.Count > 0) int.TryParse(dtAOR.Rows[0]["WorkloadAllocationID"].ToString(), out workloadAllocationID);

                        foreach (DataRow dr in dtWorkItemType.Rows)
                        {
                            if (/*dr["PDDTDR_PHASEID"].ToString() == "" || */dr["WorkloadAllocationID"].ToString() == "" || dr["WorkloadAllocationID"].ToString() == workloadAllocationID.ToString())
                            {
                                /*if (currentPhaseID != dr["PDDTDR_PHASEID"].ToString() && dr["PDDTDR_PHASEID"].ToString() != "")
                                {
                                    li = new ListItem();
                                    li.Text = dr["PDDTDR_PHASE"].ToString();
                                    li.Attributes["disabled"] = "disabled";
                                    li.Attributes["style"] = "background-color: white";
                                    ddlWorkActivity.Items.Add(li);

                                    currentPhaseID = dr["PDDTDR_PHASEID"].ToString();
                                }*/

                                if (currentWorkActivityGroup != dr["WorkActivityGroup"].ToString() && dr["WorkActivityGroup"].ToString() != "")
                                {
                                    li = new ListItem();
                                    li.Text = dr["WorkActivityGroup"].ToString();
                                    li.Attributes["disabled"] = "disabled";
                                    li.Attributes["style"] = "background-color: white";
                                    ddlWorkActivity.Items.Add(li);

                                    currentWorkActivityGroup = dr["WorkActivityGroup"].ToString();
                                }

                                li = new ListItem();
                                li.Value = dr["WORKITEMTYPEID"].ToString();
                                li.Text = dr["WORKITEMTYPE"].ToString();
                                if (id == li.Value) li.Selected = true;

                                ddlWorkActivity.Items.Add(li);
                            }
                        }

                        //Work Activity dropdown options are set based on selected Release/Deployment MGMT AOR Workload Allocation, done on client side
                        Page.ClientScript.RegisterArrayDeclaration("arrWorkActivity", JsonConvert.SerializeObject(dtWorkItemType, Newtonsoft.Json.Formatting.None));
                    }
                    WTSUtility.SelectDdlItem(ddlWorkActivity, id, value);
                    row.Cells[DCC.IndexOf("WORK ACTIVITY")].Style["width"] = "145px";
                    row.Cells[DCC.IndexOf("WORK ACTIVITY")].Controls.Add(ddlWorkActivity);
                }

                if (DCC.Contains("PDDTDR_ID") && DCC.Contains("PDD TDR"))
                {
                    id = row.Cells[DCC.IndexOf("PDDTDR_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("PDD TDR")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("PDD TDR")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'PDD TDR'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    string WorkTaskID = workitemid;
                    string WorkTask = workitem;
                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "1")
                    {
                        WorkTaskID = workitem.Split('-')[0];
                        WorkTask = workitem.Split('-')[0];
                    }
                    row.Cells[DCC.IndexOf("PDD TDR")].Style["width"] = "90px";
                    row.Cells[DCC.IndexOf("PDD TDR")].Controls.Add(CreateDropDownList(dtTemp, "PDD TDR", "FIELD_NM", "FIELD_ID", WorkTaskID, WorkTask, id, value));
                }

                //if (DCC.Contains("PRODUCTVERSION_ID") && DCC.Contains("PRODUCT VERSION"))
                //{
                //    id = row.Cells[DCC.IndexOf("PRODUCTVERSION_ID")].Text.Replace("&nbsp;", " ").Trim();
                //    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                //    value = row.Cells[DCC.IndexOf("PRODUCT VERSION")].Text.Replace("&nbsp;", " ").Trim();
                //    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                //    row.Cells[DCC.IndexOf("PRODUCT VERSION")].Style["text-align"] = "center";

                //    DataTable dtTemp = _dtOptions;
                //    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'PRODUCT VERSION'");
                //    dtTemp = dtTemp.DefaultView.ToTable();
                //    string WorkTaskID = workitemid;
                //    string WorkTask = workitem;
                //    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                //    if (blnSubTask == "1")
                //    {
                //        WorkTaskID = workitem.Split('-')[0];
                //        WorkTask = workitem.Split('-')[0];
                //    }
                //    row.Cells[DCC.IndexOf("PRODUCT VERSION")].Style["width"] = "75px";
                //    row.Cells[DCC.IndexOf("PRODUCT VERSION")].Controls.Add(CreateDropDownList(dtTemp, "PRODUCT VERSION", "FIELD_NM", "FIELD_ID", WorkTaskID, WorkTask, id, value));
                //}
                if (DCC.Contains("PRODUCTIONSTATUS_ID") && DCC.Contains("PRODUCTION STATUS"))
                {
                    id = row.Cells[DCC.IndexOf("PRODUCTIONSTATUS_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("PRODUCTION STATUS")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("PRODUCTION STATUS")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'PROD STATUS'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    string WorkTaskID = workitemid;
                    string WorkTask = workitem;
                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "1")
                    {
                        WorkTaskID = workitem.Split('-')[0];
                        WorkTask = workitem.Split('-')[0];
                    }
                    row.Cells[DCC.IndexOf("PRODUCTION STATUS")].Style["width"] = "100px";
                    row.Cells[DCC.IndexOf("PRODUCTION STATUS")].Controls.Add(CreateDropDownList(dtTemp, "PRODUCTION STATUS", "FIELD_NM", "FIELD_ID", WorkTaskID, WorkTask, id, value));
                }

                if (DCC.Contains("SYSTEMTASK_ID") && DCC.Contains("SYSTEM(TASK)"))
                {
                    id = row.Cells[DCC.IndexOf("SYSTEMTASK_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("SYSTEM(TASK)")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("SYSTEM(TASK)")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'SYSTEM(TASK)'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    string WorkTaskID = workitemid;
                    string WorkTask = workitem;
                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "1")
                    {
                        WorkTaskID = workitem.Split('-')[0];
                        WorkTask = workitem.Split('-')[0];
                    }
                    row.Cells[DCC.IndexOf("SYSTEM(TASK)")].Style["width"] = "75px";
                    row.Cells[DCC.IndexOf("SYSTEM(TASK)")].Controls.Add(CreateDropDownList(dtTemp, "SYSTEM(TASK)", "FIELD_NM", "FIELD_ID", WorkTaskID, WorkTask, id, value));
                }
                if (DCC.Contains("WORKAREA_ID") && DCC.Contains("WORK AREA"))
                {
                    id = row.Cells[DCC.IndexOf("WORKAREA_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("WORK AREA")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("WORK AREA")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtWorkArea;
                    dtTemp.DefaultView.RowFilter = string.Format(" WTS_SYSTEMID = {0} ", row.Cells[DCC.IndexOf("SystemTask_ID")].Text);
                    dtTemp = dtTemp.DefaultView.ToTable();
                    string WorkTaskID = workitemid;
                    string WorkTask = workitem;
                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "1")
                    {
                        WorkTaskID = workitem.Split('-')[0];
                        WorkTask = workitem.Split('-')[0];
                    }
                    row.Cells[DCC.IndexOf("WORK AREA")].Style["width"] = "150px";
                    row.Cells[DCC.IndexOf("WORK AREA")].Controls.Add(CreateDropDownList(dtTemp, "WORK AREA", "WorkArea", "WorkAreaID", WorkTaskID, WorkTask, id, value));
                }
                if (DCC.Contains("RESOURCEGROUP_ID") && DCC.Contains("RESOURCE GROUP"))
                {
                    id = row.Cells[DCC.IndexOf("RESOURCEGROUP_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("RESOURCE GROUP")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("RESOURCE GROUP")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'RESOURCE GROUP'");
                    dtTemp = dtTemp.DefaultView.ToTable();

                    string WorkTaskID = workitemid;
                    string WorkTask = workitem;
                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "1")
                    {
                        WorkTaskID = workitem.Split('-')[0];
                        WorkTask = workitem.Split('-')[0];
                    }
                    row.Cells[DCC.IndexOf("RESOURCE GROUP")].Style["width"] = "75px";
                    row.Cells[DCC.IndexOf("RESOURCE GROUP")].Controls.Add(CreateDropDownList(dtTemp, "RESOURCE GROUP", "FIELD_NM", "FIELD_ID", WorkTaskID, WorkTask, id, value));
                }
                if (DCC.Contains("WORKREQUEST_ID") && DCC.Contains("WORK REQUEST"))
                {
                    id = row.Cells[DCC.IndexOf("WORKREQUEST_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("WORK REQUEST")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;
                    row.Cells[DCC.IndexOf("WORK REQUEST")].Style["text-align"] = "center";

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'WORK REQUEST'");
                    dtTemp = dtTemp.DefaultView.ToTable();

                    string WorkTaskID = workitemid;
                    string WorkTask = workitem;
                    string blnSubTask = workitem.Contains(" - ") ? "1" : "0";
                    if (blnSubTask == "1")
                    {
                        WorkTaskID = workitem.Split('-')[0];
                        WorkTask = workitem.Split('-')[0];
                    }
                    row.Cells[DCC.IndexOf("WORK REQUEST")].Style["width"] = "150px";
                    row.Cells[DCC.IndexOf("WORK REQUEST")].Controls.Add(CreateDropDownList(dtTemp, "WORK REQUEST", "FIELD_NM", "FIELD_ID", WorkTaskID, WorkTask, id, value));
                }
                if (DCC.Contains("PRIORITY_ID") && DCC.Contains("PRIORITY"))
                {
                    id = row.Cells[DCC.IndexOf("PRIORITY_ID")].Text.Replace("&nbsp;", " ").Trim();
                    id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                    value = row.Cells[DCC.IndexOf("PRIORITY")].Text.Replace("&nbsp;", " ").Trim();
                    value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;

                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'PRIORITY'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    row.Cells[DCC.IndexOf("PRIORITY")].Style["width"] = "65px";
                    row.Cells[DCC.IndexOf("PRIORITY")].Controls.Add(CreateDropDownList(dtTemp, "Priority", "FIELD_NM", "FIELD_ID", workitemid, workitem, id, value));
                }

                if (DCC.Contains("STATUS_ID") && DCC.Contains("STATUS"))
                {
                    DataTable dtTemp = _dtOptions;
                    dtTemp.DefaultView.RowFilter = string.Format(" FIELD_TYPE = {0} ", "'STATUS'");
                    dtTemp = dtTemp.DefaultView.ToTable();
                    row.Cells[DCC.IndexOf("STATUS")].Style["width"] = "125px";
                    row.Cells[DCC.IndexOf("STATUS")].Controls.Add(CreateDropDownList(dtTemp, "Status", "FIELD_NM", "FIELD_ID", workitemid, workitem, row.Cells[DCC.IndexOf("STATUS_ID")].Text.Trim(), row.Cells[DCC.IndexOf("STATUS")].Text.Trim()));

                }
            }
        }
        //add Total row to top of grid (as part of header to keep fixed)
        if (row.RowIndex == 0)
        {
            FixedTotalRow(row);
        }
    }

    private void FixedTotalRow(GridViewRow row)
    {
        try
        {
            DataTable dt = (DataTable)Session["dtLevel" + this.CurrentLevel];
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
            XmlNode nodeLevel = this.Levels.SelectNodes("crosswalkparameters/level")[this.CurrentLevel - 1];
            int intColspan = nodeLevel.SelectNodes("breakout/column").Count + 1;
            // 3: #, #/#,
            nCell = new TableHeaderCell();
            nCell.Text = "TOTAL";
            nCell.ColumnSpan = intColspan;
            nCell.Style["background"] = "#d7e8fc";
            nRow.Cells.Add(nCell);

            int columnCount = 0;

            for (int i = 0; i <= DCC.Count - 1; i++)
            {
                string columnDBName = DCC[i].ColumnName.ToString();
                Boolean visible = DCC[i].ColumnName.EndsWith("_ID") || DCC[i].ColumnName == "Y" ? false : true;

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
            row.Cells[DCC.IndexOf("X")].Style["width"] = "15px";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("Y"))
        {
            if (this.CanEditWorkItem && DCC.Contains("Primary Task"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBox(true, "Primary Task", "Task_ID", "Task_ID"));
            }
            else if (this.CanEditWorkItem && DCC.Contains("Work Task"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                if (row.Cells[DCC.IndexOf("Work Task")].Text.Contains(" - "))
                {
                    row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBox(true, "Subtask", "WORKITEM_TASKID", "WorkTask_ID"));
                }
                else
                {
                    row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBox(true, "Primary Task", "Task_ID", "WorkTask_ID"));
                }
            }
            else if (this.CanEditAOR && DCC.Contains("AOR"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBox(true, "AOR", "AOR_ID", "AOR_ID"));
            }
            else
            {
                row.Cells[DCC.IndexOf("Y")].Style["display"] = "none";
            }
        }

        //Any
        if (DCC.Contains("Workload Priority")) row.Cells[DCC.IndexOf("Workload Priority")].Style["width"] = "130px";
        if (DCC.Contains("Resource Count (T.BA.PA.CT)")) row.Cells[DCC.IndexOf("Resource Count (T.BA.PA.CT)")].Style["width"] = "180px";
        if (DCC.Contains("RQMT Risk")) row.Cells[DCC.IndexOf("RQMT Risk")].Style["width"] = "150px";

        //AOR
        if (DCC.Contains("AOR")) row.Cells[DCC.IndexOf("AOR")].Style["width"] = "500px";
        if (DCC.Contains("AOR RELEASE/DEPLOYMENT MGMT")) row.Cells[DCC.IndexOf("AOR RELEASE/DEPLOYMENT MGMT")].Style["width"] = "350px";
        if (DCC.Contains("AOR WORKLOAD MGMT")) row.Cells[DCC.IndexOf("AOR WORKLOAD MGMT")].Style["width"] = "350px";
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
        if (DCC.Contains("Primary Bus. Resource")) row.Cells[DCC.IndexOf("Primary Bus. Resource")].Style["width"] = "75px";
        if (DCC.Contains("Tech. Rank")) row.Cells[DCC.IndexOf("Tech. Rank")].Style["width"] = "75px";
        if (DCC.Contains("Customer Rank")) row.Cells[DCC.IndexOf("Customer Rank")].Style["width"] = "75px";
        if (DCC.Contains("Assigned To Rank")) row.Cells[DCC.IndexOf("Assigned To Rank")].Style["width"] = "150px";
        if (DCC.Contains("Primary Resource")) row.Cells[DCC.IndexOf("Primary Resource")].Style["width"] = "75px";
        if (DCC.Contains("Priority")) row.Cells[DCC.IndexOf("Priority")].Style["width"] = "75px";
        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["width"] = "75px";
        if (DCC.Contains("Deployment")) row.Cells[DCC.IndexOf("Deployment")].Style["width"] = "75px";
        if (DCC.Contains("Production Status")) row.Cells[DCC.IndexOf("Production Status")].Style["width"] = "75px";
        if (DCC.Contains("Secondary Bus. Resource")) row.Cells[DCC.IndexOf("Secondary Bus. Resource")].Style["width"] = "75px";
        if (DCC.Contains("Secondary Tech. Resource")) row.Cells[DCC.IndexOf("Secondary Tech. Resource")].Style["width"] = "75px";
        if (DCC.Contains("Status")) row.Cells[DCC.IndexOf("Status")].Style["width"] = "75px";
        if (DCC.Contains("Submitted By")) row.Cells[DCC.IndexOf("Submitted By")].Style["width"] = "75px";
        if (DCC.Contains("System(Task)")) row.Cells[DCC.IndexOf("System(Task)")].Style["width"] = "250px";
        if (DCC.Contains("System Suite")) row.Cells[DCC.IndexOf("System Suite")].Style["width"] = "175px";
        if (DCC.Contains("Work Area")) row.Cells[DCC.IndexOf("Work Area")].Style["width"] = "75px";
        if (DCC.Contains("Title")) row.Cells[DCC.IndexOf("Title")].Style["width"] = "350px";
        if (DCC.Contains("Work Task")) row.Cells[DCC.IndexOf("Work Task")].Style["width"] = "75px";
        if (DCC.Contains("Work Request")) row.Cells[DCC.IndexOf("Work Request")].Style["width"] = "75px";
        if (DCC.Contains("Resource Group")) row.Cells[DCC.IndexOf("Resource Group")].Style["width"] = "150px";
        if (DCC.Contains("SR Number")) row.Cells[DCC.IndexOf("SR Number")].Style["width"] = "75px";
        if (DCC.Contains("Unclosed SR Tasks")) row.Cells[DCC.IndexOf("Unclosed SR Tasks")].Style["display"] = "none";
        if (DCC.Contains("Primary Task")) row.Cells[DCC.IndexOf("Primary Task")].Style["width"] = "75px";
        if (DCC.Contains("Primary Task Title")) row.Cells[DCC.IndexOf("Primary Task Title")].Style["width"] = "75px";
        if (DCC.Contains("Task.Workload.Release Status")) row.Cells[DCC.IndexOf("Task.Workload.Release Status")].Style["width"] = "200px";
        if (DCC.Contains("Session")) row.Cells[DCC.IndexOf("Session")].Style["width"] = "250px";

        if (DCC.Contains("Z")) row.Cells[DCC.IndexOf("Z")].Text = "";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (true) row.Cells[i].Style["text-align"] = "center";
        }

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Style["width"] = "15px";
            row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }


        if (DCC.Contains("Y"))
        {
            if (this.CanEditWorkItem && DCC.Contains("Work Task"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                if (row.Cells[DCC.IndexOf("Work Task")].Text.Contains(" - "))
                {
                    row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBox(false, "Subtask", "WORKITEM_TASKID", row.Cells[DCC.IndexOf("WorkTask_ID")].Text));
                }
                else
                {
                    row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBox(false, "Primary Task", "Task_ID", row.Cells[DCC.IndexOf("WorkTask_ID")].Text));
                }
            }
            else if (this.CanEditWorkItem && DCC.Contains("Primary Task"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBox(false, "Primary Task", "Task_ID", row.Cells[DCC.IndexOf("Primary Task")].Text));
            }
            else if (this.CanEditAOR && DCC.Contains("AOR"))
            {
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBox(false, "AOR", "AOR #", row.Cells[DCC.IndexOf("AOR")].Text));
            }
            else
            {
                row.Cells[DCC.IndexOf("Y")].Style["display"] = "none";
            }
        }

        if (DCC.Contains("Work Task"))
        {
            row.Cells[DCC.IndexOf("Work Task")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("Work Task")].Style["white-space"] = "nowrap";
        }

        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["text-align"] = "center";
        if (DCC.Contains("Unclosed SR Tasks")) row.Cells[DCC.IndexOf("Unclosed SR Tasks")].Style["display"] = "none";
        if (DCC.Contains("Unclosed SR Tasks")) row.Cells[DCC.IndexOf("Unclosed SR Tasks")].Attributes["field"] = "Unclosed SR Tasks";
    }

    private CheckBox CreateCheckBox(Boolean blnHeader, string strEntity, string strField, string strFieldID)
    {
        CheckBox chk = new CheckBox();

        if (blnHeader)
        {
            chk.Attributes["onchange"] = "chkAll(this);";
        }
        else {
            chk.Attributes["onchange"] = "input_change(this);";
        }
        chk.Attributes.Add("strEntity", strEntity);
        chk.Attributes.Add("strField", strField);
        chk.Attributes.Add("strFieldID", strFieldID);
        chk.Attributes["class"] = "masschange";
        return chk;
    }
    private Image CreateImage(bool isHeader, int rowIndex)
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
        else {

            if (this.CurrentLevel == this.LevelCount)
            {
                //img.Attributes["src"] = "Images/Icons/cog.png";
                //img.Attributes["title"] = "Grid Settings";
                //img.Attributes["alt"] = "Grid Settings";
                //img.Attributes["onclick"] = "openSettings();";
            }
            else
            {
                DataTable dt = (DataTable)Session["dtLevel" + this.CurrentLevel];

                if (grdData.PageIndex == 0)
                {
                    if (rowIndex == 0)
                    {
                        if (dt.Rows[0]["ROW_ID"].ToString() == "0")
                        {
                            return img;
                        }
                    }
                }
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

    private LinkButton CreateLink(string workItem_ID, string workItem)
    {
        LinkButton lb = new LinkButton();
        string workItemID = workItem_ID;
        string taskNumber = string.Empty;
        string taskID = string.Empty;
        string blnSubTask = "0";

        lb.Text = workItem;
        if (workItem.Contains(" - "))
        {
            string[] arrWorkItem = workItem.Split('-');

            workItemID = arrWorkItem[0].Trim();
            taskNumber = arrWorkItem[1].Trim();
            taskID = workItem_ID;
            blnSubTask = "1";

            lb.Attributes["onclick"] = string.Format("openWorkItem('{0}', '{1}', '{2}', {3}); return false;", workItemID, taskNumber, taskID, blnSubTask);
        }
        else {

            lb.Attributes["onclick"] = string.Format("lbEditWorkItem_click('{0}'); return false;", workItemID);
        }

        return lb;
    }

    private TextBox CreateTextBox(string workItem_ID, string workItem, string type, string value, bool isNumber)
    {
        string blnSubTask = workItem.Contains(" - ") ? "1" : "0";
        string txtValue = Server.HtmlDecode(value).Trim();
        TextBox txt = new TextBox();

        txt.Text = txtValue;
        //txt.MaxLength = 50;
        txt.Width = new Unit(95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("work_item_id", workItem_ID);
        txt.Attributes.Add("bln_sub_task", blnSubTask);
        txt.Attributes.Add("field", type);
        txt.Attributes.Add("original_value", txtValue);
        txt.ToolTip = txtValue;
        txt.Style["background-color"] = "#F5F6CE";

        if (isNumber)
        {
            txt.MaxLength = 5;
            txt.Style["text-align"] = "center";
        }

        return txt;
    }

    private DropDownList CreateDropDownList(DataTable dt, string field
        , string textField, string valueField
        ,string workItem_ID, string workItem, string value, string text = ""
        , List<string> attributes = null)
    {
        string blnSubTask = workItem.Contains(" - ") ? "1" : "0";

        DropDownList ddl = new DropDownList();
        string strOnchange = "input_change(this);";

        switch (field)
        {
            case "SYSTEM(TASK)":
                strOnchange = "ddlSystem_change(this);";
                break;
            case "Assigned Resource":
                strOnchange = "ddlAssignedTo_change(this);";
                break;
            case "Primary Resource":
                strOnchange = "ddlPrimaryResource_change(this);";
                break;
            case "Assigned To Rank":
                strOnchange = "ddlAssignedToRank_change(this);";
                break;
        }

        ddl.Width = new Unit(95, UnitType.Percentage);
        ddl.Attributes["class"] = "saveable";
        ddl.Attributes["onchange"] = strOnchange;
        ddl.Attributes.Add("work_item_id", workItem_ID);
        ddl.Attributes.Add("bln_sub_task", blnSubTask);
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
            item.Text = row[textField].ToString().Replace("&nbsp;", " ").Trim();
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

    private HtmlGenericControl LoadAORs(string workItem_ID, string workItem, DataTable dtAORLookup, string strAORType, string CascadeAOR, string AOR_ID, string AORName, string AORReleaseID, string assignedTo_ID, string primaryResource_ID, string system_ID, string productionStatusID)
    {
        HtmlGenericControl ddlDiv = new HtmlGenericControl();
        string prodStatusMsg = "AOR Release/Deployment MGMT not applicable for this task type";
        string blnSubTask = workItem.Contains(" - ") ? "1" : "0";
        StringBuilder sbAORs = new StringBuilder();

        if (blnSubTask == "0") {
            CascadeAOR = "0";
        }
        if (this.CanEditWorkItem && CascadeAOR != "1")
                {
                    //if (strAORType == "Release/Deployment MGMT" && (productionStatusID == "78" || productionStatusID == "108" || productionStatusID == "117"))
                    //{
                    //    sbAORs.Append("<div id=\"MgmtReleaseAORs\" style=\"text-align: center; display: none;\">");
                    //}
                    //else
                    //{
                        sbAORs.Append("<div id=\"MgmtReleaseAORs\" style=\"text-align: center; white-space: nowrap;\">");
            //}
            sbAORs.Append("<select work_item_id=\"" + workItem_ID + "\" bln_sub_task=\"" + blnSubTask + "\" assignedTo_ID=\"" + assignedTo_ID + "\" primaryResource_ID=\"" + primaryResource_ID + "\" system_ID=\"" + system_ID + "\" field=\"" + strAORType + "\" original_value=\"" + AORReleaseID + "\"" + (strAORType != "Release/Deployment MGMT" ? " getAORResourceTeamUser();" : "") + "\" onchange=\"AOR_change(this);\" style=\"width: 325px; background-color: #F5F6CE;\"");

            if (!this.CanEditWorkloadMGMT && strAORType == "Workload MGMT") sbAORs.Append(" disabled");

            sbAORs.Append(">"); //select

                    sbAORs.Append("<option value=\"0\"></option>");

                    bool contains = dtAORLookup.AsEnumerable().Any(row => AORReleaseID == row.Field<Int32>("AORReleaseID").ToString());

                    if (!contains && AORReleaseID != "&nbsp;") sbAORs.Append("<option value=\"" + AORReleaseID + "\" selected>" + AOR_ID + " - " + AORName + "</option>");

                    foreach (DataRow dr in dtAORLookup.Rows)
                    {
                        if (dr["AORType"].ToString() == "Release/Deployment MGMT"
                            && strAORType == "Release/Deployment MGMT")
                        {
                            sbAORs.Append("<option value=\"" + dr["AORReleaseID"].ToString() + "\" workloadallocationid=\"" + dr["WorkloadAllocationID"].ToString() + "\"");
                            if (dr["AORReleaseID"].ToString() == AORReleaseID) sbAORs.Append(" selected");

                            sbAORs.Append(">" + dr["AORID"].ToString() + " (" + dr["WorkloadAllocationAbbreviation"].ToString() + ") - " + dr["AORName"].ToString() + "</option>");
                        }
                        else if (dr["AORType"].ToString() != "Release/Deployment MGMT"
                          && strAORType != "Release/Deployment MGMT")
                        {
                            sbAORs.Append("<option value=\"" + dr["AORReleaseID"].ToString() + "\"");
                            if (dr["AORReleaseID"].ToString() == AORReleaseID) sbAORs.Append(" selected");

                            sbAORs.Append(">" + dr["AORID"].ToString() + " (" + dr["WorkloadAllocationAbbreviation"].ToString() + ") - " + dr["AORName"].ToString() + "</option>");
                        }
                    }

                    sbAORs.Append("</select>");

            if (strAORType != "Workload MGMT" || (this.CanEditWorkloadMGMT && strAORType == "Workload MGMT")) sbAORs.Append("<img src=\"Images/Icons/cog.png\" alt=\"AOR Option Settings\" title=\"AOR Option Settings\" class=\"showaoroptionsettings\" width=\"15\" height=\"15\" onclick=\"showAOROptionSettings(this);\" style=\"cursor: pointer; margin-left: 3px;\" />");

                    sbAORs.Append("<div class=\"aoroptionsettings\" style=\"text-align: left; border: 1px solid gray; position: absolute; background-color: white; padding: 5px; display: none;\">");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" checked />Affiliated by selected System</label><br />");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />Affiliated by selected Assigned To/Primary Resource</label><br />");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />Affiliated by AOR Workload Type</label><br />");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />All (grouped by System)</label>");
                    sbAORs.Append("</div>");
                    sbAORs.Append("</div>");
                    //if (strAORType == "Release/Deployment MGMT" && (productionStatusID == "78" || productionStatusID == "108" || productionStatusID == "117"))
                    //{
                    //    sbAORs.Append("<div id=\"MgmtReleaseAORsMsg\" height=\"20\" style=\"text-align: center;\">" + prodStatusMsg + "</div>");
                    //}
                    //else
                    //{
                        sbAORs.Append("<div id=\"MgmtReleaseAORsMsg\" height=\"20\" style=\"text-align: center; display: none;\">" + prodStatusMsg + "</div>");
                    //}
                }
                else
                {
                    if (strAORType == "Release/Deployment MGMT")
                    {
                        sbAORs.Append("<div id=\"MgmtReleaseAORsMsg\" height=\"20\" style=\"text-align: center;\">" + prodStatusMsg + "</div>");
                    }
                    else
                    {
                        sbAORs.Append("<div height=\"20\" style=\" text-align: center;\">" + AOR_ID + " - " + AORName + "</div>");
            }
                }
        sbAORs.Append("</div>");
        ddlDiv.InnerHtml = sbAORs.ToString();
                sbAORs.Clear();

        return ddlDiv;
    }

    #endregion

    #region Excel

    private void ExportExcel(DataTable dt)
    {
        DataSet ds = new DataSet();
        DataSet dsCopy = new DataSet();
        DataTable dtCopy = new DataTable();

        //ds.Tables.Add(dt);
        //if (Session["AORLevels"] != null) Levels = (XmlDocument)Session["AORLevels"];
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

            HtmlSelect ddlStatus = (HtmlSelect)Page.Master.FindControl("ms_Item0");
            HtmlSelect ddlAffiliated = (HtmlSelect)Page.Master.FindControl("ms_Item1");
            List<string> listStatus = new List<string>();

            if (ddlStatus != null && ddlStatus.Items.Count > 0)
            {
                foreach (ListItem li in ddlStatus.Items)
                {
                    if (li.Selected) listStatus.Add(li.Value);
                }
            }
            List<string> listAffiliated = new List<string>();

            if (ddlAffiliated != null && ddlAffiliated.Items.Count > 0)
            {
                foreach (ListItem li in ddlAffiliated.Items)
                {
                    if (li.Selected) listAffiliated.Add(li.Value);
                }
            }
            XDocument xDoc = XDocument.Parse(docLevel.InnerXml);
            // Remove all duplicate XML elements
            xDoc.Descendants("breakout")
                            .GroupBy(g => (string)g.Element("column"))
                            .SelectMany(g => g.Skip(1))
                            .Remove();

            XmlDocument docLevel2 = new XmlDocument();

            docLevel2.InnerXml = xDoc.ToString();

            dt = Workload.QM_Workload_Crosswalk_Multi_Level_Grid(level: docLevel2, filter: docFilters, qfStatus: String.Join(",", listStatus), qfAffiliated: String.Join(",", listAffiliated), myData: this.MyData);

            foreach (DataColumn c in dt.Columns.Cast<DataColumn>().ToList())
            {
                //c.ColumnName.ToString().Contains("_ID") ||
                if (c.ColumnName.ToString() == "X" || c.ColumnName.ToString() == "Z" || c.ColumnName.ToString() == "Unclosed SR Tasks")
                {
                    dt.Columns.Remove(c.ColumnName);
                }
            }

            var colNames = (from dc in dt.Columns.Cast<DataColumn>() select dc.ColumnName).Where(x => x != "X" && x != "Y" && x != "Z" && x.IndexOf("_ID", StringComparison.Ordinal) == -1).ToList();

            tblCnt += 1;

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
        else {
            DataSet newDS = ds.Copy();
            DataTable dtFiltered = dt.Copy();
            string rowFilter = "";
            for (int j = 0; j < curfilterCols.Length; j++)
            {
                if (rowFilter == "")
                {
                    rowFilter = curfilterCols[j].ToString();
                }
                else {
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
                    if (dtFiltered.Columns.Contains(remFilterCols[n].ToString())) {
                        dtFiltered.Columns.Remove(remFilterCols[n].ToString());
                    }

                }
            }

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

    #region AJAX
    [WebMethod()]
    public static string GetAOROptions(string assignedTo, string primaryResource, string system, int systemAffiliated, int resourceAffiliated, string assignedToRank, int all)
    {
        DataTable dt = new DataTable();

        try
        {
            int assignedTo_ID = 0, primaryResource_ID = 0, system_ID = 0, assignedToRank_ID = 0;

            int.TryParse(assignedTo, out assignedTo_ID);
            int.TryParse(primaryResource, out primaryResource_ID);
            int.TryParse(system, out system_ID);
            int.TryParse(assignedToRank, out assignedToRank_ID);

            dt = AOR.AORTaskOptionsList_Get(AssignedToID: assignedTo_ID, PrimaryResourceID: primaryResource_ID, SystemID: system_ID, SystemAffiliated: systemAffiliated, ResourceAffiliated: resourceAffiliated, AssignedToRank: assignedToRank_ID, All: all);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod]
    public static string SaveChanges(string changes)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            XmlDocument docChanges = (XmlDocument)JsonConvert.DeserializeXmlNode(changes, "changes");

            saved = Workload.QM_Workload_Crosswalk_Multi_Level_Grid_Save(changes: docChanges);
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

    [WebMethod(EnableSession = true)]
    public static bool UpdateSession(string ddlIndex, string ddlText, string sectionsXml)
    {
        var sessionMethods = new SessionMethods();
        sessionMethods.Session["CurrentXML"] = sectionsXml;
        sessionMethods.Session["CurrentDropDown"] = ddlIndex;
        sessionMethods.Session["Crosswalk_GridView_ML"] = ddlText;
        sessionMethods.Session["defaultCrosswalkGrid_ML"] = ddlText;

        if (sectionsXml != "")
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(sectionsXml);
            sessionMethods.Session["Levels"] = xml;
        }
        return true;
    }

    [WebMethod(true)]
    public static int WorkItem_TaskID_Get(int itemID, int taskNumber)
    {
        try
        {
            return WorkItem_Task.WorkItem_TaskID_Get(itemID, taskNumber);
        }
        catch (Exception) { return -1; }
    }
    #endregion
}