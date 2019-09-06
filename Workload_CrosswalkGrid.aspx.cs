﻿using System;
using System.Collections.Generic;
//using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
//using System.Web.Script.Serialization;
//using System.Web.Script.Services;
//using System.Web.Security;
using System.Web.Services;
//using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Linq;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;

using System.Xml;



public partial class Workload_CrosswalkGrid : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _myData = true;
    protected int _showArchived = 0;
    protected bool ShowClosed = false;
    protected bool ShowMetrics = true;
    protected bool ShowBacklog = false;
    protected int _showBacklog = 0;

    protected int ShowDevsOnly = 0;

    protected string[] SelectedStatuses;

    protected string[] SelectedUserTypes;

    //protected string[] SelectedWorkTypes;
    protected string[] SelectedAssigned;
    protected bool _refreshData = false;
    protected bool _export = false;

    protected bool IsParentConfigured = false;

    protected string ParentColumns = string.Empty;
    protected string RollupGroup = "Status";
    protected string ChildColumns = string.Empty;
    protected string DefaultSortType = "Tech";

    protected bool _showSort = false;

    protected string SortableColumns = string.Empty;
    protected string SortOrder = string.Empty;
    protected string DefaultColumnOrder = string.Empty;
    protected string SelectedColumnOrder = string.Empty;
    protected string ColumnOrder = string.Empty;

    protected string DefaultColumnOrder_Child = string.Empty;
    protected string SelectedColumnOrder_Child = string.Empty;
    protected string ColumnOrder_Child = string.Empty;

    protected bool CanViewSR = false;
    protected bool CanViewWorkItem = false;

    protected bool CanEditSR = false;
    protected bool CanEditWorkItem = false;

    protected int TaskCount = 0, SubTaskCount = 0;
    protected int OpenCount = 0, SubOpenCount = 0;
    protected int OnHoldCount = 0, SubOnHoldCount = 0;
    protected int InfoRequestedCount = 0, SubInfoRequestedCount = 0;
    protected int TotalOnHoldCount = 0, SubTotalOnHoldCount = 0;
    protected int NewCount = 0, SubNewCount = 0;
    protected int InProgressCount = 0, SubInProgressCount = 0;
    protected int ReOpenedCount = 0, SubReOpenedCount = 0;
    protected int InfoProvidedCount = 0, SubInfoProvidedCount = 0;
    protected int UnReproducibleCount = 0, SubUnReproducibleCount = 0;
    protected int TotalOpenCount = 0, SubTotalOpenCount = 0;
    protected int CheckedInCount = 0, SubCheckedInCount = 0;
    protected int DeployedCount = 0, SubDeployedCount = 0;
    protected int TotalAwaitingClosureCount = 0, SubTotalAwaitingClosureCount = 0;
    protected int ClosedCount = 0, SubClosedCount = 0;
    protected int TotalClosedCount = 0, SubTotalClosedCount = 0;
    protected int HighCount = 0, SubHighCount = 0;
    protected int MediumCount = 0, SubMediumCount = 0;
    protected int LowCount = 0, SubLowCount = 0;
    protected int NACount = 0, SubNACount = 0;
    protected int TotalPriorityCount = 0, SubTotalPriorityCount = 0;
    private DataTable dtAssigned;

    string subOnHold = "";
    string subDeployed = "";
    string listOfResources = "";
    string _UserDDLChange = "no";

    protected void Page_Load(object sender, EventArgs e)
    {
        this.CanEditSR = UserManagement.UserCanEdit(WTSModuleOption.SustainmentRequest);
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewSR = CanEditSR || UserManagement.UserCanView(WTSModuleOption.SustainmentRequest);
        this.CanViewWorkItem = CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

        int orgID = (int)UserManagement.Organization.Folsom_Dev;
        int userID = UserManagement.GetUserId_FromUsername();
        WTS_User u = new WTS_User(userID);
        u.Load();
        orgID = u.OrganizationID;
        ShowMetrics = (orgID == (int)UserManagement.Organization.Business_Team);

        readQueryString();

        init();
        loadMenus();

        if (DCC != null)
        {
            this.DCC.Clear();
        }

        if (!IsPostBack) loadGridData();
    }

    private void readQueryString()
    {
        if (Page.IsPostBack)
        {
            _refreshData = false;
        }
        else
        {
            if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
                || Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
            {
                _refreshData = true;
            }
        }

        if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
            || Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
        {
            _myData = true;
        }
        else
        {
            _myData = false;
        }

        #region QuickFilters

        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"].ToString()))
        {
            this.SelectedStatuses = Server.UrlDecode(Request.QueryString["SelectedStatuses"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
        //if (Request.QueryString["SelectedWorkTypes"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedWorkTypes"].ToString()))
        //{
        //	this.SelectedWorkTypes = Server.UrlDecode(Request.QueryString["SelectedWorkTypes"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //}

        if (Request.QueryString["SelectedUserTypes"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedUserTypes"].ToString()))
        {
            this.SelectedUserTypes = Server.UrlDecode(Request.QueryString["SelectedUserTypes"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }


        if (Request.QueryString["SelectedAssigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAssigned"].ToString()))
        {
            this.SelectedAssigned = Server.UrlDecode(Request.QueryString["SelectedAssigned"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["ShowClosed"] == null || string.IsNullOrWhiteSpace(Request.QueryString["ShowClosed"]))
        {
            ShowClosed = false;
        }
        else
        {
            if (Request.QueryString["ShowClosed"].Trim() == "1" || Request.QueryString["ShowClosed"].Trim().ToUpper() == "TRUE")
            {
                ShowClosed = true;
            }
        }

        if (Request.QueryString["ShowBacklog"] == null || string.IsNullOrWhiteSpace(Request.QueryString["ShowBacklog"]))
        {
            ShowBacklog = false;
        }
        else
        {
            if (Request.QueryString["ShowBacklog"].Trim() == "1" || Request.QueryString["ShowBacklog"].Trim().ToUpper() == "TRUE")
            {
                ShowBacklog = true;
            }
        }

        if (Request.QueryString["ShowArchived"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"].ToString()), out this._showArchived);
        }

        #endregion QuickFilters

        if (Request.QueryString["ShowMetrics"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["ShowMetrics"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["ShowMetrics"]), out ShowMetrics);
        }

        _UserDDLChange = "no";
        if (Request.QueryString["UserDDLChange"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["ShowMetrics"]))
        {
            _UserDDLChange = Request.QueryString["UserDDLChange"];
        }

        if (Session["Crosswalk_RollupGroup"] != null && !string.IsNullOrWhiteSpace(Session["Crosswalk_RollupGroup"].ToString()))
        {
            this.RollupGroup = Session["Crosswalk_RollupGroup"].ToString();
        }
        else
        {
            if (Request.QueryString["rollupGroup"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["rollupGroup"].ToString()))
            {
                this.RollupGroup = Server.UrlDecode(Request.QueryString["rollupGroup"]).Replace("?", "");
            }
            else
            {
                this.RollupGroup = "Status";
            }
            Session["Crosswalk_RollupGroup"] = this.RollupGroup;
        }

        if (Session["Crosswalk_DefaultSortType"] != null && !string.IsNullOrWhiteSpace(Session["Crosswalk_DefaultSortType"].ToString()))
        {
            this.DefaultSortType = Session["Crosswalk_DefaultSortType"].ToString();  // 'Tech',
        }
        else
        {
            if (Request.QueryString["rankSortType"] != null
                && !string.IsNullOrWhiteSpace(Request.QueryString["rankSortType"].ToString()))
            {
                this.DefaultSortType = Server.UrlDecode(Request.QueryString["rankSortType"]).Replace("?", "");
            }
            else
            {
                this.DefaultSortType = "Tech";
            }
            Session["Crosswalk_DefaultSortType"] = this.DefaultSortType;

        }


        #region Parent Columns

        if (Session["Crosswalk_SelectedParentColumns"] != null && !string.IsNullOrWhiteSpace(Session["Crosswalk_SelectedParentColumns"].ToString()))
        {
            this.ParentColumns = Session["Crosswalk_SelectedParentColumns"].ToString();
            this.IsParentConfigured = true;
        }
        else
        {
            if (Request.QueryString["parentColumns"] != null
                && !string.IsNullOrWhiteSpace(Request.QueryString["parentColumns"].ToString()))
            {
                this.ParentColumns = Server.UrlDecode(Request.QueryString["parentColumns"]).Replace("?", "");
                this.IsParentConfigured = true;
            }
            else
            {
                this.ParentColumns = "WTS_SYSTEM";
                this.IsParentConfigured = false;
            }
            Session["Crosswalk_SelectedParentColumns"] = this.ParentColumns;
        }

        #endregion Parent Columns(for rowfilters)


        #region Selected Columns and Column Order

        if (Session["Crosswalk_SelectedChildColumns"] != null && !string.IsNullOrWhiteSpace(Session["Crosswalk_SelectedChildColumns"].ToString()))
        {
            this.ChildColumns = Session["Crosswalk_SelectedChildColumns"].ToString();
        }
        else
        {
            if (Request.QueryString["childColumns"] != null
                && !string.IsNullOrWhiteSpace(Request.QueryString["childColumns"].ToString()))
            {
                this.ChildColumns = Server.UrlDecode(Request.QueryString["childColumns"]).Replace("?", "");
            }
            else
            {
                this.ChildColumns = "X,TaskNumber,Phase,System,Status,Title,AssignedTo,PrimaryDeveloper,WorkArea,Functionality,ProductionStatus,Version,Priority,Progress,Y";
            }
            Session["Crosswalk_SelectedChildColumns"] = this.ChildColumns;
        }

        #endregion Selected Columns and Column Order


        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }
        if (Request.QueryString["Export"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);
        }
    }

    private void init()
    {
        grdWorkload.GridHeaderRowDataBound += grdWorkload_GridHeaderRowDataBound;
        grdWorkload.GridRowDataBound += grdWorkload_GridRowDataBound;
        grdWorkload.GridPageIndexChanging += grdWorkload_GridPageIndexChanging;
    }

    #region Data

    private void loadGridData(bool bind = true)
    {
        DataTable dt = null;
        loadQuickfilters();

        IList<string> listStatus = new List<string>(), listAssigned = new List<string>(), listUserTypes = new List<string>();

        if (ddlStatus != null && ddlStatus.Items.Count > 0) //get the list of selected quick filters from the drop down list. Used as sql query parameter.
        {
            foreach(ListItem li in ddlStatus.Items)
            {
                if (li.Selected)
                {
                    listStatus.Add(li.Value);
                }
            }
        }

        if (ddlAssigned != null && ddlAssigned.Items.Count > 0)
        {
            foreach (ListItem li in ddlAssigned.Items)
            {
                if (li.Selected)
                {
                    listAssigned.Add(li.Value);
                }
            }
        }

        if (ddlUserTypeFilters != null && ddlUserTypeFilters.Items.Count > 0)
        {
            foreach (ListItem li in ddlUserTypeFilters.Items)
            {
                if (li.Selected)
                {
                    listUserTypes.Add(li.Value);
                }
            }
        }


        try
        {
            string[] columns = this.ParentColumns.Split(new char[] { ',' }, 5, StringSplitOptions.RemoveEmptyEntries);
            string orderByColumns = String.Join(",", columns);
            if (ShowBacklog)
                _showBacklog = 1;
            else
                _showBacklog = 0;

            if (_refreshData || Session["dtWorkloadCrosswalk"] == null)
            {
                dt = Workload.Workload_Crosswalk_Get(parentFields: this.ParentColumns, valueFields: this.RollupGroup, orderFields: orderByColumns, showArchived: _showArchived, columnListOnly: IsParentConfigured ? 0 : 1, myData: _myData, SelectedStatus: String.Join(",", listStatus), SelectedAssigned: String.Join(",", listAssigned));

                if (IsParentConfigured)  // Only populate Session if not just getting header row.
                    HttpContext.Current.Session["dtWorkloadCrosswalk"] = dt;
            }
            else
            {
                dt = (DataTable)HttpContext.Current.Session["dtWorkloadCrosswalk"];
            }
            //1 - 12 - 2017 NOTE: Affiliated is taken care of in the stored procedure, shouldn't need this.
            // 12 - 14 - 2016 - Added second check. Otherwise, get "AssignedResourceID used multiple times for TASK_ROLLUP" error with both Affiliated &AssignedTo checked.
            //if (this.ParentColumns.Contains("Affiliated") && !(this.ParentColumns.Contains("Affiliated") && this.ParentColumns.Contains("AssignedTo")))
            ////if (this.ParentColumns.Contains("Affiliated"))
            //{
            //    this.dtAssigned = Workload.Workload_Crosswalk_Get(parentFields: this.ParentColumns.Replace("Affiliated", "AssignedTo"), valueFields: this.RollupGroup, orderFields: orderByColumns.Replace("Affiliated", "AssignedTo"), showArchived: _showArchived, columnListOnly: IsParentConfigured ? 0 : 1, myData: _myData, SelectedStatus: String.Join(",", listStatus), SelectedAssigned: String.Join(",", listAssigned));
            //    foreach (DataColumn col in dtAssigned.Columns)
            //    {
            //        col.ColumnName += " AssignedTo";
            //    }
            //    dtAssigned.TableName = "AssignedTo Workload";

            //    dt = WTSUtility.JoinDataTables(dt, dtAssigned, (row1, row2) =>
            //                        row1.Field<String>("Affiliated") == row2.Field<String>("AssignedTo AssignedTo"));
            //}
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            dt = null;
        }

        if (dt != null & IsParentConfigured) // Only filter user types if not just getting header.
        {
            InitializeColumnData(ref dt);
            dt.AcceptChanges();

            this.DCC = dt.Columns;
            iti_Tools_Sharp.DynamicHeader head = WTSUtility.CreateGridMultiHeader(dt);
            if (head != null)
            {
                grdWorkload.DynamicHeader = head;
            }

            int iIsDeveloper = 2;
            int iIsBusAnalyst = 2;
            int iIsAMCGEO = 2;
            int iIsCASUser = 2;
            int iIsALODUser = 2;

            foreach (String userType in listUserTypes)
            {
                switch (userType)
                {
                    case "IsDeveloper":
                        iIsDeveloper = 1;
                        break;
                    case "IsBusAnalyst":
                        iIsBusAnalyst = 1;
                        break;
                    case "IsAMCGEO":
                        iIsAMCGEO = 1;
                        break;
                    case "IsCASUser":
                        iIsCASUser = 1;
                        break;
                    case "IsALODUser":
                        iIsALODUser = 1;
                        break;
                }
            }

            DataTable dtDevs = MasterData.WTS_ResourceDevelopers_Get(iIsDeveloper, iIsBusAnalyst, iIsAMCGEO, iIsCASUser, iIsALODUser);
            if (dtDevs != null)
            {
                foreach (DataRow row in dtDevs.Rows)
                {
                    listOfResources += row[0] + ", ";
                }
                if (listOfResources.Length > 2)
                    listOfResources = listOfResources.Substring(0, listOfResources.Length - 2);
                if (dt != null)
                {
                    DataColumnCollection columns = dt.Columns;
                    if (columns.Contains("[AssignedResourceID AssignedTo]"))
                    {
                        dt.DefaultView.RowFilter = "[AssignedResourceID AssignedTo] IN (" + listOfResources + ")";
                        dt = dt.DefaultView.ToTable();
                    }
                    else if (columns.Contains("AssignedResourceID"))
                    {
                        dt.DefaultView.RowFilter = "AssignedResourceID IN (" + listOfResources + ")";
                        dt = dt.DefaultView.ToTable();
                    }
                    else if (columns.Contains("AffiliatedID"))
                    {
                        dt.DefaultView.RowFilter = "AffiliatedID IN (" + listOfResources + ")";
                        dt = dt.DefaultView.ToTable();
                    }
                }
            }
            else
            {
                return;
            }

            listOfResources = "";
            foreach (ListItem li in ddlAssigned.Items)
            {
                if (li.Selected == true)
                {
                    listOfResources += li.Value + ", ";
                }
            }
            if (listOfResources.Length > 2)
                listOfResources = listOfResources.Substring(0, listOfResources.Length - 2);

            // If My Data
            // The problem here is that you can have My Data selected but choose another Assigned To.  Remarking out.
            //if (_myData)
            //{
            //    int listCount = 0;
            //    int iID = 0;
            //    int userID = UserManagement.GetUserId_FromUsername();
            //    foreach (ListItem li in ddlAssigned.Items)
            //    {
            //        if (li.Selected)
            //        {
            //            listCount += 1;
            //            int.TryParse(li.Value, out iID);
            //        }
            //    }
            //    if (listCount == 1 & iID == userID)
            //    {
            //        if (dt.Columns.Contains("ASSIGNEDRESOURCEID"))
            //        {
            //            dt.DefaultView.RowFilter = "ASSIGNEDRESOURCEID = " + UserManagement.GetUserId_FromUsername();
            //            dt = dt.DefaultView.ToTable();
            //        }
            //        else if (dt.Columns.Contains("[AssignedResourceID AssignedTo]"))
            //        {
            //            dt.DefaultView.RowFilter = "[AssignedResourceID AssignedTo] = " + UserManagement.GetUserId_FromUsername();
            //            dt = dt.DefaultView.ToTable();
            //        }
            //        else if (dt.Columns.Contains("AffiliatedID"))
            //        {
            //            dt.DefaultView.RowFilter = "AffiliatedID = " + UserManagement.GetUserId_FromUsername();
            //            dt = dt.DefaultView.ToTable();
            //        }
            //    }
            //}
            //else  // Enterprise
            //{




            bool AssignedResCol = false;
            bool AssignedResAndToCol = false;
            bool AffiliatedCol = false;
            bool PrimaryBusCol = false;
            bool PrimaryTechCol = false;
            bool SecondaryBusCol = false;
            bool SecondaryTechCol = false;

            if (dt.Columns.Contains("ASSIGNEDRESOURCEID"))
            {
                AssignedResCol = true;
            }
            else if (dt.Columns.Contains("[AssignedResourceID AssignedTo]"))
            {
                AssignedResAndToCol = true;
            }
            else if (dt.Columns.Contains("AffiliatedID"))
            {
                AffiliatedCol = true;
            }
            else if (dt.Columns.Contains("PRIMARYRESOURCEID"))
            {
                PrimaryTechCol = true;
            }

            else if (dt.Columns.Contains("PrimaryBusinessResourceID"))
            {
                PrimaryBusCol = true;
            }
            else if (dt.Columns.Contains("SecondaryBusinessResourceID"))
            {
                SecondaryBusCol = true;
            }
            else if (dt.Columns.Contains("SecondaryResourceID"))
            {
                SecondaryTechCol = true;
            }

            string strFilters = "";
            //--------------------------------------------------------------------------------------
            if (AssignedResCol)
                strFilters = "ASSIGNEDRESOURCEID IN (" + listOfResources + ")";
            if (AssignedResAndToCol)
            {
                if (strFilters == "")
                    strFilters = "[AssignedResourceID AssignedTo] IN (" + listOfResources + ")";
                else
                    strFilters = " OR [AssignedResourceID AssignedTo] IN (" + listOfResources + ")";
            }
            if (AffiliatedCol)
            {
                if (strFilters == "")
                    strFilters = "AffiliatedID IN (" + listOfResources + ")";
                else
                    strFilters = " OR AffiliatedID IN (" + listOfResources + ")";
            }
            if (PrimaryTechCol)
            {
                if (strFilters == "")
                    strFilters = "PRIMARYRESOURCEID IN (" + listOfResources + ")";
                else
                    strFilters = " OR PRIMARYRESOURCEID IN (" + listOfResources + ")";
            }
            if (PrimaryBusCol)
            {
                if (strFilters == "")
                    strFilters = "PrimaryBusinessResourceID IN (" + listOfResources + ")";
                else
                    strFilters = " OR PrimaryBusinessResourceID IN (" + listOfResources + ")";
            }
            if (SecondaryBusCol)
            {
                if (strFilters == "")
                    strFilters = "SecondaryBusinessResourceID IN (" + listOfResources + ")";
                else
                    strFilters = " OR SecondaryBusinessResourceID IN (" + listOfResources + ")";
            }
            if (SecondaryTechCol)
            {
                if (strFilters == "")
                    strFilters = "SecondaryResourceID IN (" + listOfResources + ")";
                else
                    strFilters = " OR SecondaryResourceID IN (" + listOfResources + ")";
            }


            dt.DefaultView.RowFilter = strFilters;
            dt = dt.DefaultView.ToTable();

            //}

            if (Session["Crosswalk_GridView"] != null)
                spanGridView.InnerText = Session["Crosswalk_GridView"].ToString();

            if (!IsParentConfigured)
            {
                spanRowCount.InnerText = "0";
            }
            else
            {
                spanRowCount.InnerText = dt.Rows.Count.ToString();
            }

            if (_export)
            {
                ExportExcel(dt, SelectedStatus: String.Join(",", listStatus), SelectedAssigned: String.Join(",", listAssigned));
            }
            else
            {

                TallyCounts(dt, listStatus, listAssigned);

                grdWorkload.DataSource = dt;
                if (bind) grdWorkload.DataBind();
            }  // NOT Export to Excel

        } // dt != null
    }


    private void zeroCounts()
    {
        TaskCount = 0;
        OpenCount = 0;
        TotalOpenCount = 0;
        TotalAwaitingClosureCount = 0;
        TotalOnHoldCount = 0;
        TotalClosedCount = 0;
        NewCount = 0;
        ReOpenedCount = 0;
        InProgressCount = 0;
        UnReproducibleCount = 0;
        CheckedInCount = 0;
        DeployedCount = 0;
        ClosedCount = 0;
        InfoRequestedCount = 0;
        OnHoldCount = 0;

        SubTaskCount = 0;
        SubOpenCount = 0;
        SubClosedCount = 0;
        SubTotalOpenCount = 0;
        SubTotalOnHoldCount = 0;
        SubTotalClosedCount = 0;
        SubTotalAwaitingClosureCount = 0;
        SubReOpenedCount = 0;
        SubInProgressCount = 0;
        SubInfoProvidedCount = 0;
        SubUnReproducibleCount = 0;
        SubCheckedInCount = 0;
        SubDeployedCount = 0;
        SubNewCount = 0;
        SubInfoRequestedCount = 0;
        SubOnHoldCount = 0;
    }

    private void TallyCounts(DataTable dt, IList<string> listStatus, IList<string> listAssigned)
    {
        DataTable dtTaskCounts = new DataTable();
        DataTable dtTaskSubCounts = new DataTable();




        //if (Session["MetricsGridHeaderCounts"] == null
        //|| _showArchived.ToString() != Session["ShowArchived"].ToString()
        //|| _myData.ToString() != Session["MyData"].ToString()
        //|| this.RankSortType.ToString() != Session["RankSortType"].ToString()
        //|| ShowClosed.ToString() != Session["ShowClosed"].ToString()
        //|| ShowBacklog.ToString() != Session["ShowBacklog"].ToString()
        //|| statusIds.ToString() != Session["StatusIDs"].ToString()
        //|| assignedIDs.ToString() != Session["AssignedIDs"].ToString()
        //|| HttpContext.Current.Session["GridView"].ToString() != Session["Crosswalk_GridView"].ToString()
        //|| _refreshData)




        dtTaskCounts = MasterData.MetricsGridHeaderCounts_Get(includeArchive: _showArchived, myData: _myData, selectedStatus: String.Join(",", listStatus), selectedAssigned: String.Join(",", listAssigned));
        dtTaskSubCounts = MasterData.MetricsGridHeaderSubCounts_Get(includeArchive: _showArchived, myData: _myData, selectedStatus: String.Join(",", listStatus), selectedAssigned: String.Join(",", listAssigned));








        //if (Session["MetricsGridHeaderCounts"] == null)
        //{
        //    dtTaskCounts = MasterData.MetricsGridHeaderCounts_Get(includeArchive: _showArchived, myData: _myData, selectedStatus: String.Join(",", listStatus), selectedAssigned: String.Join(",", listAssigned));
        //    HttpContext.Current.Session["MetricsGridHeaderCounts"] = dtTaskCounts;
        //}
        //else
        //{
        //    dtTaskCounts = (DataTable)HttpContext.Current.Session["MetricsGridHeaderCounts"];
        //}

        //if (Session["MetricsGridHeaderSubCounts"] == null)
        //{
        //    dtTaskSubCounts = MasterData.MetricsGridHeaderSubCounts_Get(includeArchive: _showArchived, myData: _myData, selectedStatus: String.Join(",", listStatus), selectedAssigned: String.Join(",", listAssigned));
        //    HttpContext.Current.Session["MetricsGridHeaderSubCounts"] = dtTaskCounts;
        //}
        //else
        //{
        //    dtTaskSubCounts = (DataTable)HttpContext.Current.Session["MetricsGridHeaderSubCounts"];
        //}

        zeroCounts();

        if (dtTaskCounts != null)
        {
            foreach (DataRow rowCounts in dtTaskCounts.Rows)
            {
                switch (rowCounts["STATUS"].ToString().ToLower())
                {
                    case "re-opened":
                        ReOpenedCount = (int)rowCounts["COUNT"];
                        TotalOpenCount += (int)rowCounts["COUNT"];
                        TaskCount += (int)rowCounts["COUNT"];
                        OpenCount += (int)rowCounts["COUNT"];
                        break;
                    case "in progress":
                        InProgressCount = (int)rowCounts["COUNT"];
                        TotalOpenCount += (int)rowCounts["COUNT"];
                        TaskCount += (int)rowCounts["COUNT"];
                        OpenCount += (int)rowCounts["COUNT"];
                        break;
                    case "info provided":
                        InfoProvidedCount = (int)rowCounts["COUNT"];
                        TotalOpenCount += (int)rowCounts["COUNT"];
                        TaskCount += (int)rowCounts["COUNT"];
                        OpenCount += (int)rowCounts["COUNT"];
                        break;
                    case "un-reproducible":
                        UnReproducibleCount = (int)rowCounts["COUNT"];
                        TotalOpenCount += (int)rowCounts["COUNT"];
                        TaskCount += (int)rowCounts["COUNT"];
                        OpenCount += (int)rowCounts["COUNT"];
                        break;
                    case "checked in":
                        CheckedInCount = (int)rowCounts["COUNT"];
                        TotalAwaitingClosureCount += (int)rowCounts["COUNT"];
                        TaskCount += (int)rowCounts["COUNT"];
                        OpenCount += (int)rowCounts["COUNT"];  //??
                        break;
                    case "deployed":
                        DeployedCount = (int)rowCounts["COUNT"];
                        TotalAwaitingClosureCount += (int)rowCounts["COUNT"];
                        TaskCount += (int)rowCounts["COUNT"];
                        break;
                    case "closed":
                        ClosedCount = (int)rowCounts["COUNT"];
                        TotalClosedCount += (int)rowCounts["COUNT"];
                        TaskCount += (int)rowCounts["COUNT"];
                        break;
                    case "new":
                        NewCount = (int)rowCounts["COUNT"];
                        TotalOpenCount += (int)rowCounts["COUNT"];
                        TaskCount += (int)rowCounts["COUNT"];
                        OpenCount += (int)rowCounts["COUNT"];
                        break;
                    case "info requested":
                        InfoRequestedCount = (int)rowCounts["COUNT"];
                        TotalOnHoldCount += (int)rowCounts["COUNT"];
                        TaskCount += (int)rowCounts["COUNT"];
                        break;
                    case "on hold":
                        OnHoldCount = (int)rowCounts["COUNT"];
                        TotalOnHoldCount += (int)rowCounts["COUNT"];
                        TaskCount += (int)rowCounts["COUNT"];
                        break;
                }
            }
        }

        if (dtTaskSubCounts != null)
        {
            foreach (DataRow rowCounts in dtTaskSubCounts.Rows)
            {
                switch (rowCounts["STATUS"].ToString().ToLower())
                {
                    case "re-opened":
                        SubReOpenedCount = (int)rowCounts["COUNT"];
                        SubTotalOpenCount += (int)rowCounts["COUNT"];
                        SubTaskCount += (int)rowCounts["COUNT"];
                        SubOpenCount += (int)rowCounts["COUNT"];
                        break;
                    case "in progress":
                        SubInProgressCount = (int)rowCounts["COUNT"];
                        SubTotalOpenCount += (int)rowCounts["COUNT"];
                        SubTaskCount += (int)rowCounts["COUNT"];
                        SubOpenCount += (int)rowCounts["COUNT"];
                        break;
                    case "info provided":
                        SubInfoProvidedCount = (int)rowCounts["COUNT"];
                        SubTotalOpenCount += (int)rowCounts["COUNT"];
                        SubTaskCount += (int)rowCounts["COUNT"];
                        SubOpenCount += (int)rowCounts["COUNT"];
                        break;
                    case "un-reproducible":
                        SubUnReproducibleCount = (int)rowCounts["COUNT"];
                        SubTotalOpenCount += (int)rowCounts["COUNT"];
                        SubTaskCount += (int)rowCounts["COUNT"];
                        SubOpenCount += (int)rowCounts["COUNT"];
                        break;
                    case "checked in":
                        SubCheckedInCount = (int)rowCounts["COUNT"];
                        SubTotalAwaitingClosureCount += (int)rowCounts["COUNT"];
                        SubTaskCount += (int)rowCounts["COUNT"];
                        break;
                    case "deployed":
                        SubDeployedCount = (int)rowCounts["COUNT"];
                        SubTotalAwaitingClosureCount += (int)rowCounts["COUNT"];
                        SubTaskCount += (int)rowCounts["COUNT"];
                        break;
                    case "closed":
                        SubClosedCount = (int)rowCounts["COUNT"];
                        SubTotalClosedCount += (int)rowCounts["COUNT"];
                        SubTaskCount += (int)rowCounts["COUNT"];
                        break;
                    case "new":
                        SubNewCount = (int)rowCounts["COUNT"];
                        SubTotalOpenCount += (int)rowCounts["COUNT"];
                        SubTaskCount += (int)rowCounts["COUNT"];
                        SubOpenCount += (int)rowCounts["COUNT"];
                        break;
                    case "info requested":
                        SubInfoRequestedCount = (int)rowCounts["COUNT"];
                        SubTotalOnHoldCount += (int)rowCounts["COUNT"];
                        SubTaskCount += (int)rowCounts["COUNT"];
                        break;
                    case "on hold":
                        SubOnHoldCount = (int)rowCounts["COUNT"];
                        SubTotalOnHoldCount += (int)rowCounts["COUNT"];
                        SubTaskCount += (int)rowCounts["COUNT"];
                        break;
                }
            }
        }
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int count = 0;

            if (DCC.Contains("High_Items") && DCC.Contains("High_Items_Sub"))
            {
                int.TryParse(dt.Rows[i][DCC.IndexOf("High_Items")].ToString(), out count);
                HighCount += count; TotalPriorityCount += count; count = 0;
                int.TryParse(dt.Rows[i][DCC.IndexOf("High_Items_Sub")].ToString(), out count);
                SubHighCount += count; SubTotalPriorityCount += count; count = 0;
            }
            if (DCC.Contains("Medium_Items") && DCC.Contains("Medium_Items_Sub"))
            {
                int.TryParse(dt.Rows[i][DCC.IndexOf("Medium_Items")].ToString(), out count);
                MediumCount += count; TotalPriorityCount += count; count = 0;
                int.TryParse(dt.Rows[i][DCC.IndexOf("Medium_Items_Sub")].ToString(), out count);
                SubMediumCount += count; SubTotalPriorityCount += count; count = 0;
            }
            if (DCC.Contains("Low_Items") && DCC.Contains("Low_Items_Sub"))
            {
                int.TryParse(dt.Rows[i][DCC.IndexOf("Low_Items")].ToString(), out count);
                LowCount += count; TotalPriorityCount += count; count = 0;
                int.TryParse(dt.Rows[i][DCC.IndexOf("Low_Items_Sub")].ToString(), out count);
                SubLowCount += count; SubTotalPriorityCount += count; count = 0;
            }
            if (DCC.Contains("NA_Items") && DCC.Contains("NA_Items_Sub"))
            {
                int.TryParse(dt.Rows[i][DCC.IndexOf("NA_Items")].ToString(), out count);
                NACount += count; TotalPriorityCount += count; count = 0;
                int.TryParse(dt.Rows[i][DCC.IndexOf("NA_Items_Sub")].ToString(), out count);
                SubNACount += count; SubTotalPriorityCount += count; count = 0;
            }
        }
    }
    private void loadQuickfilters()
    {
        loadQF_Status();
        loadQF_Assigned();
        loadQF_Types();
    }

    private void loadQF_Status()
    {
        List<string> statusList = new List<string>();
        if (SelectedStatuses != null && SelectedStatuses.Length > 0)
        {
            foreach (string s in SelectedStatuses)
            {
                statusList.Add(s.Trim());
            }
        }

        DataTable dtStatus = MasterData.StatusList_Get(includeArchive: false);// dt.DefaultView.ToTable(true, new string[] { "STATUSID", "STATUS" });
        if (dtStatus != null)
        {
            bool blnClosed = false, blnApprovedClosed = false;

            ddlStatus.Items.Clear();
            dtStatus.DefaultView.RowFilter = " StatusType = 'Work' ";
            dtStatus = dtStatus.DefaultView.ToTable();

            ListItem item = null;
            foreach (DataRow dr in dtStatus.Rows)
            {
                item = new ListItem(dr["STATUS"].ToString(), dr["STATUSID"].ToString());
                item.Selected = (statusList.Count == 0 || statusList.Contains(dr["STATUSID"].ToString()));

                if (statusList.Count == 0 && (item.Text == "Closed" || item.Text == "Approved/Closed" || item.Text == "On Hold"))
                {
                    item.Selected = false;
                }
                if (item.Selected)
                {
                    if (item.Text == "Closed")
                    {
                        blnClosed = true;
                    }
                    else if (item.Text == "Approved/Closed")
                    {
                        blnApprovedClosed = true;
                    }
                }

                ddlStatus.Items.Add(item);

            }
            if (blnClosed && blnApprovedClosed) this.ShowClosed = true;
        }
    }

    #endregion Data

    #region QuickFilters
    private void loadQF_Assigned()
    {
        List<string> Assigned = new List<string>();
        bool blnBacklog = false;

        if (SelectedAssigned != null && SelectedAssigned.Length > 0)
        {
            foreach (string s in SelectedAssigned)
            {
                if (s == "69")
                {
                    blnBacklog = true;
                }
                Assigned.Add(s.Trim());
            }
        }

        DataTable dtAssigned = UserManagement.LoadUserList();  // userNameSearch: userName

        if (dtAssigned != null)
        {
            ddlAssigned.Items.Clear();
            ListItem item = null;

            int userID = UserManagement.GetUserId_FromUsername();

            foreach (DataRow dr in dtAssigned.Rows)
            {
                if (dr["WTS_RESOURCEID"] == null || string.IsNullOrWhiteSpace(dr["WTS_RESOURCEID"].ToString()))
                {
                    continue;
                }
                else
                {
                    item = new ListItem(dr["UserName"].ToString(), dr["WTS_RESOURCEID"].ToString());

                    //_myData >> If true, only check current user as assigned
                    if (_UserDDLChange == "no" && _myData)
                    {
                        if (dr["WTS_RESOURCEID"].ToString() == userID.ToString())
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                        ddlAssigned.Items.Add(item);
                    }
                    else
                    {
                        item.Selected = ((Assigned.Count == 0 && item.Text != "IT.Backlog") || Assigned.Contains(dr["WTS_RESOURCEID"].ToString()));
                        ddlAssigned.Items.Add(item);
                    }
                }
            }  // foreach
        }
        this.ShowBacklog = blnBacklog;

    }

    private void loadQF_Types()
    {
        if (ddlUserTypeFilters.Items.Count < 1)
        {
            ddlUserTypeFilters.Items.Clear();

            ListItem liAdd = null;

            liAdd = new ListItem("Developers", "IsDeveloper");
            if  (SelectedUserTypes == null || SelectedUserTypes.Contains("IsDeveloper"))
                liAdd.Selected = true;
            else
                liAdd.Selected = false;
            ddlUserTypeFilters.Items.Add(liAdd);

            liAdd = new ListItem("Business Analysts", "IsBusAnalyst");
            if (SelectedUserTypes == null || SelectedUserTypes.Contains("IsBusAnalyst"))
                liAdd.Selected = true;
            else
                liAdd.Selected = false;
            ddlUserTypeFilters.Items.Add(liAdd);

            liAdd = new ListItem("AMC GEO", "IsAMCGEO");
            if (SelectedUserTypes == null || SelectedUserTypes.Contains("IsAMCGEO"))
                liAdd.Selected = true;
            else
                liAdd.Selected = false;
            ddlUserTypeFilters.Items.Add(liAdd);

            liAdd = new ListItem("CAS User", "IsCASUser");
            if (SelectedUserTypes == null || SelectedUserTypes.Contains("IsCASUser"))
                liAdd.Selected = true;
            else
                liAdd.Selected = false;
            ddlUserTypeFilters.Items.Add(liAdd);

            liAdd = new ListItem("ALOD User", "IsALODUser");
            if (SelectedUserTypes == null || SelectedUserTypes.Contains("IsALODUser"))
                liAdd.Selected = true;
            else
                liAdd.Selected = false;
            ddlUserTypeFilters.Items.Add(liAdd);
        }
    }

    #endregion QuickFilters

    #region InitColumns
    protected void InitializeColumnData(ref DataTable dt)
    {
        try
        {
            string displayName = string.Empty, sortName = string.Empty, groupName = string.Empty;
            bool blnVisible = false, isViewable = false, blnSortable = false, blnOrderable = false, forceVisible = false;

            bool statusRollup = false, priorityRollup = false;
            statusRollup = (this.RollupGroup.ToUpper() == "STATUS");
            priorityRollup = (this.RollupGroup.ToUpper() == "PRIORITY");

            foreach (DataColumn gridColumn in dt.Columns)
            {
                sortName = groupName = string.Empty;
                displayName = gridColumn.ColumnName;
                blnVisible = blnSortable = blnOrderable = isViewable = forceVisible = false;
                switch (gridColumn.ColumnName)
                {
                    case "X":
                        displayName = "X";
                        groupName = string.Empty;
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = false;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "Y":
                        displayName = "Y";
                        groupName = string.Empty;
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = false;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "TITLE":
                        groupName = displayName = "Work Request";
                        groupName = string.Empty;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WorkType_Sort":
                        groupName = displayName = "Work Type Rank";
                        groupName = string.Empty;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WorkType":
                        groupName = displayName = "Work Type";
                        groupName = string.Empty;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "ItemType_Sort":
                        groupName = displayName = "Item Type Rank";
                        groupName = string.Empty;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WORKITEMTYPE":
                        groupName = displayName = "Item Type";
                        groupName = string.Empty;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "System_Sort":
                        groupName = displayName = "Priority";
                        groupName = "System";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WTS_SYSTEM":
                        groupName = displayName = "System";
                        groupName = "System";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    //case "AllocationGroup_Sort":
                    //    displayName = "Allocation Group Sort";
                    //    groupName = "Allocation Group";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    blnOrderable = true;
                    //    isViewable = true;
                    //    break;
                    //case "AllocationGroup":
                    //    groupName = displayName = "Allocation Group";
                    //    groupName = "Allocation Group";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    blnOrderable = true;
                    //    isViewable = true;
                    //    break;
                    //case "AllocationCategory":
                    //    groupName = displayName = "Allocation Category";
                    //    groupName = string.Empty;
                    //    blnVisible = false;
                    //    blnSortable = false;
                    //    blnOrderable = false;
                    //    isViewable = false;
                    //    break;
                    //case "Allocation_Sort":
                    //    displayName = "Allocation Sort";
                    //    groupName = "Allocation Assignment";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    blnOrderable = true;
                    //    isViewable = true;
                    //    break;
                    //case "Allocation":
                    //    displayName = "Allocation Assignment";
                    //    groupName = "Allocation Assignment";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    blnOrderable = true;
                    //    isViewable = true;
                    //    break;
                    case "WA_Sort":
                        displayName = "WA Sort";
                        groupName = "Work Area";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WorkArea":
                        displayName = "Work Area";
                        groupName = "Work Area";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WG_Sort":
                        displayName = "Functionality Sort";
                        groupName = "Functionality";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WorkloadGroup":
                        displayName = "Functionality";
                        groupName = "Functionality";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "ProductionStatus":
                        displayName = "Production Status";
                        groupName = string.Empty;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "Version_Sort":
                        displayName = "Version Rank";
                        groupName = string.Empty;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "ProductVersion":
                        displayName = "Version";
                        groupName = string.Empty;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "Priority_Sort":
                        displayName = "Priority Order";
                        groupName = string.Empty;
                        blnVisible = statusRollup;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = statusRollup;
                        break;
                    case "Priority":
                        displayName = "Priority";
                        groupName = string.Empty;
                        blnVisible = statusRollup;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = statusRollup;
                        break;
                    case "Affiliated":
                        displayName = "Affiliated";
                        groupName = "Resources";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "AssignedTo":
                        displayName = "Assigned To";
                        groupName = "Resources";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "Primary_Developer":
                        displayName = "Primary Tech. Resource";
                        groupName = "Resources";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WorkloadSubmittedBy":
                        displayName = "Submitted By";
                        groupName = "Resources";
                        blnVisible = false;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "Primary_Analyst":
                        displayName = "Primary Analyst";
                        groupName = "Resources";
                        blnVisible = false;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "PRIMARYRESOURCE":
                        displayName = "Primary Tech. Resource";
                        groupName = "Resources";
                        blnVisible = false;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;

                    case "SECONDARYRESOURCE":
                        displayName = "Secondary Bus. Resource";
                        groupName = "Resources";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;

                    case "PrimaryBusinessResource":
                        displayName = "Primary Bus. Resource";
                        groupName = "Resources";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "SecondaryBusinessResource":
                        displayName = "Secondary Bus. Resource";
                        groupName = "Resources";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "Status_Sort":
                        displayName = "Status Order";
                        groupName = string.Empty;
                        blnVisible = priorityRollup;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = priorityRollup;
                        break;
                    case "STATUS":
                        displayName = "Status";
                        groupName = string.Empty;
                        blnVisible = priorityRollup;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = priorityRollup;
                        break;
                    case "Total_Items":
                        displayName = "Total Tasks";
                        groupName = "";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "Open_Items":
                        displayName = "Open";
                        groupName = "";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "High_Items":
                        displayName = "High";
                        groupName = "Priority";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "Medium_Items":
                        displayName = "Medium";
                        groupName = "Priority";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "Low_Items":
                        displayName = "Low";
                        groupName = "Priority";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "NA_Items":
                        displayName = "N/A";
                        groupName = "Priority";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "OnHold_Items":
                        displayName = "On Hold";
                        groupName = "On Hold";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "InfoRequested_Items":
                        displayName = "Info<br />Requested";
                        groupName = "On Hold";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "New_Items":
                        displayName = "New";
                        groupName = "Open";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "InProgress_Items":
                        displayName = "In<br />Progress";
                        groupName = "Open";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "ReOpened_Items":
                        displayName = "Re-Opened";
                        groupName = "Open";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "InfoProvided_Items":
                        displayName = "Info<br />Provided";
                        groupName = "Open";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "UnReproducible_Items":
                        displayName = "Un-<br />Reproducible";
                        groupName = "Open";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "CheckedIn_Items":
                        displayName = "Checked In";
                        groupName = "Awaiting Closure";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "Deployed_Items":
                        displayName = "Deployed";
                        groupName = "Awaiting Closure";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "Closed_Items":
                        displayName = "Closed";
                        groupName = "Closed";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = true;
                        break;
                    case "Percent_OnHold_Items":
                        displayName = "% On Hold";
                        groupName = "";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "Percent_Open_Items":
                        displayName = "% Open";
                        groupName = "";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "Percent_Closed_Items":
                        displayName = "% Closed";
                        groupName = "";
                        forceVisible = true;
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    //case "STATUSID":
                    //    displayName = "Status ID";
                    //    groupName = "";
                    //    forceVisible = true;
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    blnOrderable = true;
                    //    isViewable = true;
                    //    break;
                }

                columnData.Columns.Add(gridColumn.ColumnName, displayName, (blnVisible || forceVisible), blnSortable);
                columnData.Columns.Item(columnData.Columns.Count - 1).SortName = string.IsNullOrEmpty(sortName) ? displayName : sortName;
                columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
                columnData.Columns.Item(columnData.Columns.Count - 1).Viewable = (isViewable || forceVisible);
                columnData.Columns.Item(columnData.Columns.Count - 1).GroupName = groupName;
            }

            //Initialize the columnData
            columnData.Initialize(ref dt, ";", "~", "|");

            //Get sortable columns and default column order
            SortableColumns = columnData.SortableColumnsToString();
            DefaultColumnOrder = columnData.DefaultColumnOrderToString();
            //Sort and Reorder Columns
            columnData.ReorderDataTable(ref dt, ColumnOrder);

            SortOrder = Workload.GetSortValuesFromDB(1, "WORKLOAD_CROSSWALKGRID.ASPX");

            columnData.SortDataTable(ref dt, SortOrder);
            SelectedColumnOrder = columnData.CurrentColumnOrderToString();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    #endregion InitColumns

    #region RowFilters
    protected void applyRowFilters(ref DataTable dt, string[] filterArray)
    {
        if (dt == null || dt.Rows.Count == 0)
        {
            return;
        }

        string[] filterSet;

        //rowfilters
        StringBuilder sbRowFilter = new StringBuilder();
        string filterName = "", filterValue = "";

        foreach (string filterPair in filterArray)
        {
            filterSet = filterPair.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (filterSet.Length > 1)
            {
                filterName = getFilterName(filterSet[0]);
                filterValue = filterSet[1].Trim();

                if (string.IsNullOrWhiteSpace(filterValue))
                {
                    sbRowFilter.AppendFormat(" {0} {1} IS NULL ", sbRowFilter.Length > 0 ? "AND" : "", filterName);
                }
                else
                {
                    sbRowFilter.AppendFormat(" {0} {1} = {2} ", sbRowFilter.Length > 0 ? "AND" : "", filterName, filterValue);
                }
            }
        }

        if (sbRowFilter.Length > 0)
        {
            dt.DefaultView.RowFilter = sbRowFilter.ToString();
            dt = dt.DefaultView.ToTable();
        }
    }

    protected string getFilterName(string filterTextName)
    {
        string filterName = "";

        switch (filterTextName.Trim().ToUpper())
        {
            case "WORKREQUEST":
            case "WORK REQUEST":
                filterName = "WORKREQUESTID";
                break;
            case "ITEM TYPE":
                filterName = "WORKITEMTYPEID";
                break;
            case "WORK TYPE":
                filterName = "WorkTypeID";
                break;
            case "SYSTEM":
                filterName = "WTS_SYSTEMID";
                break;
            case "WORKLOAD STATUS":
            case "STATUS":
                filterName = "STATUSID";
                break;
            case "ALLOCATION GROUP":
                filterName = "AllocationGroupID";
                break;
            case "ALLOCATION CATEGORY":
                filterName = "AllocationCategoryID";
                break;
            case "ALLOCATION ASSIGNMENT":
            case "WORKLOAD ALLOCATION":
                filterName = "ALLOCATIONID";
                break;
            case "WORKLOAD ASSIGNED TO":
            case "ASSIGNED TO":
                filterName = "ASSIGNEDRESOURCEID";
                break;
            case "WORKLOAD PRIMARY RESOURCE":
            case "WORKLOAD PRIMARY DEVELOPER":
            case "PRIMARY RESOURCE":
            case "PRIMARY DEVELOPER":
                filterName = "PRIMARYRESOURCEID";
                break;
            case "PRIMARY BUSINESS RESOURCE":
                filterName = "PrimaryBusinessResourceID";
                break;
            case "WORK AREA":
                filterName = "WorkAreaID";
                break;
            case "WORKLOAD GROUP":
            case "FUNCTIONALITY":
                filterName = "WorkloadGroupID";
                break;
            case "RELEASE VERSION":
            case "PRODUCT VERSION":
            case "VERSION":
                filterName = "ProductVersionID";
                break;
            case "WORKLOAD PRIORITY":
            case "PRIORITY":
                filterName = "PRIORITYID";
                break;
            case "WORKLOAD SUBMITTED BY":
            case "SUBMITTED BY":
                filterName = "SubmittedByID";
                break;
        }

        return filterName;
    }

    #endregion RowFilters

    #region Grid

    void grdWorkload_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        columnData.SetupGridHeader(row);
        row.Attributes.Add("ID", string.Format("GridHeaderRow{0}", row.RowIndex == -1 ? "" : row.RowIndex.ToString()));

        formatColumnDisplay(ref row);

        row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
        row.Cells[DCC.IndexOf("Y")].Text = "&nbsp;";

        for (int i = 0; i < row.Cells.Count; i++)
        {
            if ((DCC.Contains("TITLE") && i == DCC.IndexOf("TITLE"))
                || (DCC.Contains("Allocation") && i == DCC.IndexOf("Allocation"))
                || (DCC.Contains("AllocationGroup") && i == DCC.IndexOf("AllocationGroup")))
            {
                //do nothing, already configured
            }
            else
            {
                row.Cells[i].Style["text-align"] = "center";
                row.Cells[i].Style["padding-left"] = "0px";
            }

            if (row.RowIndex > 0)
            {
                row.Cells[i].Style["border-top"] = "1px solid grey";
                if (DCC.Contains("WA_Sort"))
                {
                    row.Cells[DCC.IndexOf("WA_Sort")].Text = "Priority";
                }
                if (DCC.Contains("WG_Sort"))
                {
                    row.Cells[DCC.IndexOf("WG_Sort")].Text = "Priority";
                }
                if (DCC.Contains("Allocation_Sort"))
                {
                    row.Cells[DCC.IndexOf("Allocation_Sort")].Text = "Rank";
                }
                if (DCC.Contains("AllocationGroup_Sort"))
                {
                    row.Cells[DCC.IndexOf("AllocationGroup_Sort")].Text = "Priority";
                }
            }
        }

        if (DCC.Contains("Total_Items")) row.Cells[DCC.IndexOf("Total_Items")].Text += "<br /> (" + TaskCount + " / " + SubTaskCount + ")";
        if (DCC.Contains("Open_Items")) row.Cells[DCC.IndexOf("Open_Items")].Text += "<br /> (" + OpenCount + " / " + SubOpenCount + ")";
        if (DCC.Contains("OnHold_Items") && row.RowIndex == 1) row.Cells[DCC.IndexOf("OnHold_Items")].Text += "<br /> (" + OnHoldCount + " / " + SubOnHoldCount + ")";
        if (DCC.Contains("InfoRequested_Items")) row.Cells[DCC.IndexOf("InfoRequested_Items")].Text += "<br /> (" + InfoRequestedCount + " / " + SubInfoRequestedCount + ")";
        if (DCC.Contains("OnHold_Items") && row.RowIndex == 0) row.Cells[DCC.IndexOf("OnHold_Items")].Text += " (" + TotalOnHoldCount + " / " + SubTotalOnHoldCount + ")";
        if (DCC.Contains("New_Items") && row.RowIndex == 1) row.Cells[DCC.IndexOf("New_Items")].Text += "<br /> (" + NewCount + " / " + SubNewCount + ")";
        if (DCC.Contains("InProgress_Items")) row.Cells[DCC.IndexOf("InProgress_Items")].Text += "<br /> (" + InProgressCount + " / " + SubInProgressCount + ")";
        if (DCC.Contains("ReOpened_Items")) row.Cells[DCC.IndexOf("ReOpened_Items")].Text += "<br /> (" + ReOpenedCount + " / " + SubReOpenedCount + ")";
        if (DCC.Contains("InfoProvided_Items")) row.Cells[DCC.IndexOf("InfoProvided_Items")].Text += "<br /> (" + InfoProvidedCount + " / " + SubInfoProvidedCount + ")";
        if (DCC.Contains("UnReproducible_Items")) row.Cells[DCC.IndexOf("UnReproducible_Items")].Text += "<br /> (" + UnReproducibleCount + " / " + SubUnReproducibleCount + ")";
        if (DCC.Contains("New_Items") && row.RowIndex == 0) row.Cells[DCC.IndexOf("New_Items")].Text += " (" + TotalOpenCount + " / " + SubTotalOpenCount + ")";
        if (DCC.Contains("CheckedIn_Items") && row.RowIndex == 1) row.Cells[DCC.IndexOf("CheckedIn_Items")].Text += "<br /> (" + CheckedInCount + " / " + SubCheckedInCount + ")";
        if (DCC.Contains("Deployed_Items")) row.Cells[DCC.IndexOf("Deployed_Items")].Text += "<br /> (" + DeployedCount + " / " + SubDeployedCount + ")";
        if (DCC.Contains("CheckedIn_Items") && row.RowIndex == 0) row.Cells[DCC.IndexOf("CheckedIn_Items")].Text += " (" + TotalAwaitingClosureCount + " / " + SubTotalAwaitingClosureCount + ")";
        if (DCC.Contains("Closed_Items") && row.RowIndex == 1) row.Cells[DCC.IndexOf("Closed_Items")].Text += "<br /> (" + ClosedCount + " / " + SubClosedCount + ")";
        if (DCC.Contains("Closed_Items") && row.RowIndex == 0) row.Cells[DCC.IndexOf("Closed_Items")].Text += " (" + TotalClosedCount + " / " + SubTotalClosedCount + ")";
        if (DCC.Contains("High_Items") && row.RowIndex == 1) row.Cells[DCC.IndexOf("High_Items")].Text += "<br /> (" + HighCount + " / " + SubHighCount + ")";
        if (DCC.Contains("Medium_Items")) row.Cells[DCC.IndexOf("Medium_Items")].Text += "<br /> (" + MediumCount + " / " + SubMediumCount + ")";
        if (DCC.Contains("Low_Items")) row.Cells[DCC.IndexOf("Low_Items")].Text += "<br /> (" + LowCount + " / " + SubLowCount + ")";
        if (DCC.Contains("NA_Items")) row.Cells[DCC.IndexOf("NA_Items")].Text += "<br /> (" + NACount + " / " + SubNACount + ")";
        if (DCC.Contains("High_Items") && row.RowIndex == 0) row.Cells[DCC.IndexOf("High_Items")].Text += " (" + TotalPriorityCount + " / " + SubTotalPriorityCount + ")";
    }

    void grdWorkload_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);

        GridViewRow row = e.Row;
        string rowNum = row.RowIndex.ToString();
        formatColumnDisplay(ref row);

        //buttons to show/hide child grid
        row.Cells[DCC["X"].Ordinal].Controls.Clear();
        row.Cells[DCC["X"].Ordinal].Controls.Add(createShowHideButton_WorkItems(true, "Show", rowNum));
        row.Cells[DCC["X"].Ordinal].Controls.Add(createShowHideButton_WorkItems(false, "Hide", rowNum));

        #region Sort/Rank columns

        string newText = string.Empty;
        if (_showSort)
        {
            if (DCC.Contains("STATUS") && DCC.Contains("Status_Sort"))
            {
                newText = string.Format("{0} - {1}"
                    , row.Cells[DCC["Status_Sort"].Ordinal].Text.Replace("&nbsp;", "").Trim()
                    , row.Cells[DCC["STATUS"].Ordinal].Text.Replace("&nbsp;", " ").Trim());
                row.Cells[DCC["STATUS"].Ordinal].Text = newText;
            }
        }

        #endregion Sort/Rank columns


        #region Percent Calculations

        if (DCC.Contains("Total_Items"))
        {
            int open = 0, closed = 0, total = 0;
            int.TryParse(row.Cells[DCC["Total_Items"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), out total);

            if (DCC.Contains("Open_Items") && DCC.Contains("Percent_Open_Items"))
            {
                decimal percentOpen = 0, percentClosed = 0;

                int.TryParse(row.Cells[DCC["Open_Items"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), out open);
                if (DCC.Contains("Closed_Items"))
                {
                    int.TryParse(row.Cells[DCC["Closed_Items"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), out closed);
                }
                else
                {
                    closed = total - open;
                }

                percentOpen = total == 0 ? 0 : (100 * (Math.Round((decimal)((decimal)open / (decimal)total), 3, MidpointRounding.AwayFromZero)));
                percentOpen = Math.Round(percentOpen, 1);
                percentClosed = (100 - percentOpen);

                row.Cells[DCC["Percent_Open_Items"].Ordinal].Text = percentOpen.ToString();
                row.Cells[DCC["Percent_Closed_Items"].Ordinal].Text = percentClosed.ToString();
            }
            if (DCC.Contains("OnHold_Items") && DCC.Contains("Percent_OnHold_Items"))
            {
                int onHold = 0;
                int infoRequested = 0;
                decimal percentOnHold = 0;

                if (DCC.Contains("InfoRequested_Items"))
                {
                    int.TryParse(row.Cells[DCC["InfoRequested_Items"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), out infoRequested);
                }

                int.TryParse(row.Cells[DCC["OnHold_Items"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), out onHold);

                percentOnHold = total == 0 ? 0 : (100 * (Math.Round((decimal)((decimal)(onHold + infoRequested) / (decimal)total), 3, MidpointRounding.AwayFromZero)));
                percentOnHold = Math.Round(percentOnHold, 1);

                row.Cells[DCC["Percent_OnHold_Items"].Ordinal].Text = percentOnHold.ToString();
            }
        }

        if (DCC.Contains("Total_Items_Sub"))
        {
            int openSub = 0, closedSub = 0, totalSub = 0;
            int.TryParse(row.Cells[DCC["Total_Items_Sub"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), out totalSub);

            if (DCC.Contains("Open_Items_Sub") && DCC.Contains("Percent_Open_Items_Sub"))
            {
                decimal percentOpenSub = 0, percentClosedSub = 0;

                int.TryParse(row.Cells[DCC["Open_Items_Sub"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), out openSub);
                if (DCC.Contains("Closed_Items_Sub"))
                {
                    int.TryParse(row.Cells[DCC["Closed_Items_Sub"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), out closedSub);
                }
                else
                {
                    closedSub = totalSub - openSub;
                }

                percentOpenSub = totalSub == 0 ? 0 : (100 * (Math.Round((decimal)((decimal)openSub / (decimal)totalSub), 3, MidpointRounding.AwayFromZero)));
                percentOpenSub = Math.Round(percentOpenSub, 1);
                percentClosedSub = (100 - percentOpenSub);

                row.Cells[DCC["Percent_Open_Items_Sub"].Ordinal].Text = percentOpenSub.ToString();
                row.Cells[DCC["Percent_Closed_Items_Sub"].Ordinal].Text = percentClosedSub.ToString();
            }
            if (DCC.Contains("OnHold_Items_Sub") && DCC.Contains("Percent_OnHold_Items_Sub"))
            {
                int onHoldSub = 0;
                int infoRequestedSub = 0;
                decimal percentOnHoldSub = 0;

                if (DCC.Contains("InfoRequested_Items"))
                {
                    int.TryParse(row.Cells[DCC["InfoRequested_Items_Sub"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), out infoRequestedSub);
                }
                int.TryParse(row.Cells[DCC["OnHold_Items_Sub"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), out onHoldSub);

                percentOnHoldSub = totalSub == 0 ? 0 : (100 * (Math.Round((decimal)((decimal)(onHoldSub + infoRequestedSub) / (decimal)totalSub), 3, MidpointRounding.AwayFromZero)));
                percentOnHoldSub = Math.Round(percentOnHoldSub, 1);

                row.Cells[DCC["Percent_OnHold_Items_Sub"].Ordinal].Text = percentOnHoldSub.ToString();
            }
        }

        #endregion Percent Calculations

        #region Rollup Columns Sub

        if (DCC.Contains("Total_Items") && DCC.Contains("Total_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("Total_Items")].Text = row.Cells[DCC.IndexOf("Total_Items")].Text + " / " + row.Cells[DCC.IndexOf("Total_Items_Sub")].Text;
        }
        if (DCC.Contains("Open_Items") && DCC.Contains("Open_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("Open_Items")].Text = row.Cells[DCC.IndexOf("Open_Items")].Text + " / " + row.Cells[DCC.IndexOf("Open_Items_Sub")].Text;
        }
        if (DCC.Contains("Percent_OnHold_Items") && DCC.Contains("Percent_OnHold_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("Percent_OnHold_Items")].Text = row.Cells[DCC.IndexOf("Percent_OnHold_Items")].Text + " / " + row.Cells[DCC.IndexOf("Percent_OnHold_Items_Sub")].Text;
        }
        if (DCC.Contains("Percent_Open_Items") && DCC.Contains("Percent_Open_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("Percent_Open_Items")].Text = row.Cells[DCC.IndexOf("Percent_Open_Items")].Text + " / " + row.Cells[DCC.IndexOf("Percent_Open_Items_Sub")].Text;
        }
        if (DCC.Contains("Percent_Closed_Items") && DCC.Contains("Percent_Closed_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("Percent_Closed_Items")].Text = row.Cells[DCC.IndexOf("Percent_Closed_Items")].Text + " / " + row.Cells[DCC.IndexOf("Percent_Closed_Items_Sub")].Text;
        }
        //Priorities
        if (DCC.Contains("High_Items") && DCC.Contains("High_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("High_Items")].Text = row.Cells[DCC.IndexOf("High_Items")].Text + " / " + row.Cells[DCC.IndexOf("High_Items_Sub")].Text;
        }
        if (DCC.Contains("Medium_Items") && DCC.Contains("Medium_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("Medium_Items")].Text = row.Cells[DCC.IndexOf("Medium_Items")].Text + " / " + row.Cells[DCC.IndexOf("Medium_Items_Sub")].Text;
        }
        if (DCC.Contains("Low_Items") && DCC.Contains("Low_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("Low_Items")].Text = row.Cells[DCC.IndexOf("Low_Items")].Text + " / " + row.Cells[DCC.IndexOf("Low_Items_Sub")].Text;
        }
        if (DCC.Contains("NA_Items") && DCC.Contains("NA_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("NA_Items")].Text = row.Cells[DCC.IndexOf("NA_Items")].Text + " / " + row.Cells[DCC.IndexOf("NA_Items_Sub")].Text;
        }
        //Statuses
        if (DCC.Contains("New_Items") && DCC.Contains("New_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("New_Items")].Text = row.Cells[DCC.IndexOf("New_Items")].Text + " / " + row.Cells[DCC.IndexOf("New_Items_Sub")].Text;
        }
        if (DCC.Contains("ReOpened_Items") && DCC.Contains("ReOpened_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("ReOpened_Items")].Text = row.Cells[DCC.IndexOf("ReOpened_Items")].Text + " / " + row.Cells[DCC.IndexOf("ReOpened_Items_Sub")].Text;
        }
        if (DCC.Contains("InfoRequested_Items") && DCC.Contains("InfoRequested_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("InfoRequested_Items")].Text = row.Cells[DCC.IndexOf("InfoRequested_Items")].Text + " / " + row.Cells[DCC.IndexOf("InfoRequested_Items_Sub")].Text;
        }
        if (DCC.Contains("InfoProvided_Items") && DCC.Contains("InfoProvided_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("InfoProvided_Items")].Text = row.Cells[DCC.IndexOf("InfoProvided_Items")].Text + " / " + row.Cells[DCC.IndexOf("InfoProvided_Items_Sub")].Text;
        }
        if (DCC.Contains("InProgress_Items") && DCC.Contains("InProgress_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("InProgress_Items")].Text = row.Cells[DCC.IndexOf("InProgress_Items")].Text + " / " + row.Cells[DCC.IndexOf("InProgress_Items_Sub")].Text;
        }
        if (DCC.Contains("OnHold_Items") && DCC.Contains("OnHold_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("OnHold_Items")].Text = row.Cells[DCC.IndexOf("OnHold_Items")].Text + " / " + row.Cells[DCC.IndexOf("OnHold_Items_Sub")].Text;
        }
        if (DCC.Contains("UnReproducible_Items") && DCC.Contains("UnReproducible_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("UnReproducible_Items")].Text = row.Cells[DCC.IndexOf("UnReproducible_Items")].Text + " / " + row.Cells[DCC.IndexOf("UnReproducible_Items_Sub")].Text;
        }
        if (DCC.Contains("CheckedIn_Items") && DCC.Contains("CheckedIn_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("CheckedIn_Items")].Text = row.Cells[DCC.IndexOf("CheckedIn_Items")].Text + " / " + row.Cells[DCC.IndexOf("CheckedIn_Items_Sub")].Text;
        }
        if (DCC.Contains("Deployed_Items") && DCC.Contains("Deployed_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("Deployed_Items")].Text = row.Cells[DCC.IndexOf("Deployed_Items")].Text + " / " + row.Cells[DCC.IndexOf("Deployed_Items_Sub")].Text;
        }
        if (DCC.Contains("Closed_Items") && DCC.Contains("Closed_Items_Sub"))
        {
            row.Cells[DCC.IndexOf("Closed_Items")].Text = row.Cells[DCC.IndexOf("Closed_Items")].Text + " / " + row.Cells[DCC.IndexOf("Closed_Items_Sub")].Text;
        }
        #endregion Rollup Columns

        #region AssignedTo
        if (DCC.Contains("Total_Items") && DCC.Contains("Total_Items AssignedTo") && DCC.Contains("Total_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("Total_Items")].Text = row.Cells[DCC.IndexOf("Total_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("Total_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("Total_Items")].Text;
        }
        if (DCC.Contains("Open_Items") && DCC.Contains("Open_Items AssignedTo") && DCC.Contains("Open_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("Open_Items")].Text = row.Cells[DCC.IndexOf("Open_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("Open_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("Open_Items")].Text;
        }
        //if (DCC.Contains("Percent_OnHold_Items") && DCC.Contains("Percent_OnHold_Items AssignedTo") && DCC.Contains("Percent_OnHold_Items_Sub AssignedTo"))
        //{
        //    row.Cells[DCC.IndexOf("Percent_OnHold_Items")].Text += "<br />" + row.Cells[DCC.IndexOf("Percent_OnHold_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("Percent_OnHold_Items_Sub AssignedTo")].Text;
        //}
        //if (DCC.Contains("Percent_Open_Items") && DCC.Contains("Percent_Open_Items AssignedTo") && DCC.Contains("Percent_Open_Items_Sub AssignedTo"))
        //{
        //    row.Cells[DCC.IndexOf("Percent_Open_Items")].Text += "<br />" + row.Cells[DCC.IndexOf("Percent_Open_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("Percent_Open_Items_Sub AssignedTo")].Text;
        //}
        //if (DCC.Contains("Percent_Closed_Items") && DCC.Contains("Percent_Closed_Items AssignedTo") && DCC.Contains("Percent_Closed_Items_Sub AssignedTo"))
        //{
        //    row.Cells[DCC.IndexOf("Percent_Closed_Items")].Text += "<br />" + row.Cells[DCC.IndexOf("Percent_Closed_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("Percent_Closed_Items_Sub AssignedTo")].Text;
        //}
        //Priorities
        if (DCC.Contains("High_Items") && DCC.Contains("High_Items AssignedTo") && DCC.Contains("High_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("High_Items")].Text = row.Cells[DCC.IndexOf("High_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("High_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("High_Items")].Text;
        }
        if (DCC.Contains("Medium_Items") && DCC.Contains("Medium_Items AssignedTo") && DCC.Contains("Medium_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("Medium_Items")].Text = row.Cells[DCC.IndexOf("Medium_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("Medium_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("Medium_Items")].Text;
        }
        if (DCC.Contains("Low_Items") && DCC.Contains("Low_Items AssignedTo") && DCC.Contains("Low_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("Low_Items")].Text = row.Cells[DCC.IndexOf("Low_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("Low_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("Low_Items")].Text;
        }
        if (DCC.Contains("NA_Items") && DCC.Contains("NA_Items AssignedTo") && DCC.Contains("NA_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("NA_Items")].Text = row.Cells[DCC.IndexOf("NA_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("NA_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("NA_Items")].Text;
        }
        //Statuses
        if (DCC.Contains("New_Items") && DCC.Contains("New_Items AssignedTo") && DCC.Contains("New_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("New_Items")].Text = row.Cells[DCC.IndexOf("New_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("New_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("New_Items")].Text;
        }
        if (DCC.Contains("ReOpened_Items") && DCC.Contains("ReOpened_Items AssignedTo") && DCC.Contains("ReOpened_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("ReOpened_Items")].Text = row.Cells[DCC.IndexOf("ReOpened_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("ReOpened_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("ReOpened_Items")].Text;
        }
        if (DCC.Contains("InfoRequested_Items") && DCC.Contains("InfoRequested_Items AssignedTo") && DCC.Contains("InfoRequested_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("InfoRequested_Items")].Text = row.Cells[DCC.IndexOf("InfoRequested_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("InfoRequested_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("InfoRequested_Items")].Text;
        }
        if (DCC.Contains("InfoProvided_Items") && DCC.Contains("InfoProvided_Items AssignedTo") && DCC.Contains("InfoProvided_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("InfoProvided_Items")].Text = row.Cells[DCC.IndexOf("InfoProvided_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("InfoProvided_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("InfoProvided_Items")].Text;
        }
        if (DCC.Contains("InProgress_Items") && DCC.Contains("InProgress_Items AssignedTo") && DCC.Contains("InProgress_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("InProgress_Items")].Text = row.Cells[DCC.IndexOf("InProgress_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("InProgress_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("InProgress_Items")].Text;
        }
        if (DCC.Contains("OnHold_Items") && DCC.Contains("OnHold_Items AssignedTo") && DCC.Contains("OnHold_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("OnHold_Items")].Text = row.Cells[DCC.IndexOf("OnHold_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("OnHold_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("OnHold_Items")].Text;
        }
        if (DCC.Contains("UnReproducible_Items") && DCC.Contains("UnReproducible_Items AssignedTo") && DCC.Contains("UnReproducible_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("UnReproducible_Items")].Text = row.Cells[DCC.IndexOf("UnReproducible_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("UnReproducible_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("UnReproducible_Items")].Text;
        }
        if (DCC.Contains("CheckedIn_Items") && DCC.Contains("CheckedIn_Items AssignedTo") && DCC.Contains("CheckedIn_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("CheckedIn_Items")].Text = row.Cells[DCC.IndexOf("CheckedIn_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("CheckedIn_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("CheckedIn_Items")].Text;
        }
        if (DCC.Contains("Deployed_Items") && DCC.Contains("Deployed_Items AssignedTo") && DCC.Contains("Deployed_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("Deployed_Items")].Text = row.Cells[DCC.IndexOf("Deployed_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("Deployed_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("Deployed_Items")].Text;
        }
        if (DCC.Contains("Closed_Items") && DCC.Contains("Closed_Items AssignedTo") && DCC.Contains("Closed_Items_Sub AssignedTo"))
        {
            row.Cells[DCC.IndexOf("Closed_Items")].Text = row.Cells[DCC.IndexOf("Closed_Items AssignedTo")].Text + " / " + row.Cells[DCC.IndexOf("Closed_Items_Sub AssignedTo")].Text + "<br />" + row.Cells[DCC.IndexOf("Closed_Items")].Text;
        }
            #endregion AssignedTo

            #region Child Grids

            string[] columns = this.ParentColumns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < columns.Length; i++)
        {
            switch (columns[i].Trim().ToUpper())
            {
                case "WORKREQUEST":
                    sb.AppendFormat("{0}WorkRequest={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["WORKREQUESTID"].Ordinal].Text.Trim());
                    break;
                case "WORKTYPE":
                    sb.AppendFormat("{0}Work Type={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["WorkTypeID"].Ordinal].Text.Trim());
                    break;
                case "ITEMTYPE":
                    sb.AppendFormat("{0}Item Type={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["WORKITEMTYPEID"].Ordinal].Text.Trim());
                    break;
                case "SYSTEM":
                    sb.AppendFormat("{0}System={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["WTS_SYSTEMID"].Ordinal].Text.Trim());
                    break;
                case "ALLOCATIONGROUP":
                    sb.AppendFormat("{0}Allocation Group={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["AllocationGroupID"].Ordinal].Text.Trim());
                    break;
                case "ALLOCATIONCATEGORY":
                    sb.AppendFormat("{0}Allocation Category={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["AllocationCategoryID"].Ordinal].Text.Trim());
                    break;
                case "ALLOCATIONASSIGNMENT":
                    sb.AppendFormat("{0}Allocation Assignment={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["AllocationID"].Ordinal].Text.Trim());
                    break;
                case "WORKAREA":
                    sb.AppendFormat("{0}Work Area={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["WorkAreaID"].Ordinal].Text.Trim());
                    break;
                case "WORKLOADGROUP":
                case "FUNCTIONALITY":
                    sb.AppendFormat("{0}Workload Group={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["WorkloadGroupID"].Ordinal].Text.Trim());
                    break;
                case "VERSION":
                    sb.AppendFormat("{0}Release Version={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["ProductVersionID"].Ordinal].Text.Trim());
                    break;
                case "AFFILIATED":
                    sb.AppendFormat("{0}Affiliated={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["AffiliatedID"].Ordinal].Text.Trim());
                    break;
                case "ASSIGNEDTO":
                    sb.AppendFormat("{0}Workload Assigned To={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["AssignedResourceID"].Ordinal].Text.Trim());
                    break;
                case "PRIMARYDEVELOPER":
                case "PRIMARYTECHRESOURCE":
                case "PRIMARYTECH. RESOURCE":
                    sb.AppendFormat("{0}Primary Developer={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["PRIMARYRESOURCEID"].Ordinal].Text.Trim());
                    break;
                case "PRIMARYBUSINESSRESOURCE":
                case "PRIMARYBUSRESOURCE":
                case "PRIMARYBUS.RESOURCE":
                case "PRIMARYBUS. RESOURCE":
                    sb.AppendFormat("{0}Primary Business Resource={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["PrimaryBusinessResourceID"].Ordinal].Text.Trim());
                    break;
                case "SUBMITTEDBY":
                    sb.AppendFormat("{0}Workload Submitted By={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["WorkloadSubmittedByID"].Ordinal].Text.Trim());
                    break;
                case "PRIORITY":
                    sb.AppendFormat("{0}Workload Priority={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["PriorityID"].Ordinal].Text.Trim());
                    break;
                case "STATUS":
                    sb.AppendFormat("{0}Workload Status={1}", sb.Length > 0 ? "," : "", row.Cells[DCC["STATUSID"].Ordinal].Text.Trim());
                    break;
            }
        }

        //add child grid row for Work Items
        Table table = (Table)row.Parent;
        GridViewRow childRow = createChildRow(rowNum, "WorkItem");
        childRow.Attributes.Add("filters", sb.ToString());
        table.Rows.AddAt(table.Rows.Count, childRow);

        #endregion Child Grids

    }

    void grdWorkload_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdWorkload.PageIndex = e.NewPageIndex;
        loadGridData(false);
    }

    #endregion Grid

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            row.Cells[i].Style["text-align"] = "center";
        }

        //more column formatting
        row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
        if (DCC.Contains("TITLE"))
        {
            //row.Cells[DCC.IndexOf("TITLE")].Style["width"] = "75px";
            row.Cells[DCC.IndexOf("TITLE")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("TITLE")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("TITLE")].Style["width"] = "200px";
        }
        if (DCC.Contains("WorkType"))
        {
            row.Cells[DCC.IndexOf("WorkType")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("WorkType")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("WorkType")].Style["width"] = "75px";
        }
        if (DCC.Contains("WORKITEMTYPE"))
        {
            row.Cells[DCC.IndexOf("WORKITEMTYPE")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("WORKITEMTYPE")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("WORKITEMTYPE")].Style["width"] = "90px";
        }
        if (DCC.Contains("WTS_SYSTEM"))
        {
            row.Cells[DCC.IndexOf("WTS_SYSTEM")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("WTS_SYSTEM")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("WTS_SYSTEM")].Style["width"] = "75px";
        }
        if (DCC.Contains("AllocationGroup"))
        {
            row.Cells[DCC.IndexOf("AllocationGroup")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("AllocationGroup")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("AllocationGroup")].Style["width"] = "190px";
        }
        if (DCC.Contains("AllocationGroup_Sort"))
        {
            row.Cells[DCC.IndexOf("AllocationGroup_Sort")].Style["width"] = "50px";
        }
        if (DCC.Contains("AllocationCategory"))
        {
            row.Cells[DCC.IndexOf("AllocationCategory")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("AllocationCategory")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("AllocationCategory")].Style["width"] = "75px";
        }
        if (DCC.Contains("Allocation"))
        {
            row.Cells[DCC.IndexOf("Allocation")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("Allocation")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("Allocation")].Style["width"] = "190px";
        }
        if (DCC.Contains("Allocation_Sort"))
        {
            row.Cells[DCC.IndexOf("Allocation_Sort")].Style["width"] = "50px";
        }
        if (DCC.Contains("WorkloadGroup"))
        {
            row.Cells[DCC.IndexOf("WorkloadGroup")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("WorkloadGroup")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("WorkloadGroup")].Style["width"] = "115px";
        }
        if (DCC.Contains("WG_Sort"))
        {
            row.Cells[DCC.IndexOf("WG_Sort")].Style["width"] = "50px";
        }
        if (DCC.Contains("WorkArea"))
        {
            row.Cells[DCC.IndexOf("WorkArea")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("WorkArea")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("WorkArea")].Style["width"] = "115px";
        }
        if (DCC.Contains("WA_Sort"))
        {
            row.Cells[DCC.IndexOf("WA_Sort")].Style["width"] = "50px";
        }
        if (DCC.Contains("ProductVersion"))
        {
            row.Cells[DCC.IndexOf("ProductVersion")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("ProductVersion")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("ProductVersion")].Style["width"] = "65px";
        }
        if (DCC.Contains("Affiliated"))
        {
            row.Cells[DCC.IndexOf("Affiliated")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("Affiliated")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("Affiliated")].Style["width"] = "125px";
        }
        if (DCC.Contains("AssignedTo"))
        {
            row.Cells[DCC.IndexOf("AssignedTo")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("AssignedTo")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("AssignedTo")].Style["width"] = "125px";
        }
        if (DCC.Contains("Primary_Developer"))
        {
            row.Cells[DCC.IndexOf("Primary_Developer")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("Primary_Developer")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("Primary_Developer")].Style["width"] = "125px";
        }
        if (DCC.Contains("PrimaryBusinessResource"))
        {
            row.Cells[DCC.IndexOf("PrimaryBusinessResource")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("PrimaryBusinessResource")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("PrimaryBusinessResource")].Style["width"] = "125px";
        }
        if (DCC.Contains("SecondaryBusinessResource"))
        {
            row.Cells[DCC.IndexOf("SecondaryBusinessResource")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("SecondaryBusinessResource")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("SecondaryBusinessResource")].Style["width"] = "125px";
        }
        if (DCC.Contains("WorkloadSubmittedBy"))
        {
            row.Cells[DCC.IndexOf("WorkloadSubmittedBy")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("WorkloadSubmittedBy")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("WorkloadSubmittedBy")].Style["width"] = "125px";
        }
        if (DCC.Contains("Status"))
        {
            row.Cells[DCC.IndexOf("Status")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("Status")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("Status")].Style["width"] = "90px";
        }
        if (DCC.Contains("Priority"))
        {
            row.Cells[DCC.IndexOf("Priority")].Style["text-align"] = "left";
            row.Cells[DCC.IndexOf("Priority")].Style["padding-left"] = "5px";
            row.Cells[DCC.IndexOf("Priority")].Style["width"] = "65px";
        }
        if (DCC.Contains("PRIMARYRESOURCE"))
        {
            row.Cells[DCC.IndexOf("PRIMARYRESOURCE")].Style["width"] = "75px";
        }

        #region Rollup Columns

        if (DCC.Contains("Total_Items"))
        {
            row.Cells[DCC.IndexOf("Total_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("Open_Items"))
        {
            row.Cells[DCC.IndexOf("Open_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("Percent_OnHold_Items"))
        {
            row.Cells[DCC.IndexOf("Percent_OnHold_Items")].Style["width"] = "79px";
        }
        if (DCC.Contains("Percent_Open_Items"))
        {
            row.Cells[DCC.IndexOf("Percent_Open_Items")].Style["width"] = "57px";
        }
        if (DCC.Contains("Percent_Closed_Items"))
        {
            row.Cells[DCC.IndexOf("Percent_Closed_Items")].Style["width"] = "65px";
        }
        //Priorities
        if (DCC.Contains("High_Items"))
        {
            row.Cells[DCC.IndexOf("High_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("Medium_Items"))
        {
            row.Cells[DCC.IndexOf("Medium_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("Low_Items"))
        {
            row.Cells[DCC.IndexOf("Low_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("NA_Items"))
        {
            row.Cells[DCC.IndexOf("NA_Items")].Style["width"] = "75px";
        }
        //Statuses
        if (DCC.Contains("New_Items"))
        {
            row.Cells[DCC.IndexOf("New_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("ReOpened_Items"))
        {
            row.Cells[DCC.IndexOf("ReOpened_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("InfoRequested_Items"))
        {
            row.Cells[DCC.IndexOf("InfoRequested_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("InfoProvided_Items"))
        {
            row.Cells[DCC.IndexOf("InfoProvided_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("InProgress_Items"))
        {
            row.Cells[DCC.IndexOf("InProgress_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("OnHold_Items"))
        {
            row.Cells[DCC.IndexOf("OnHold_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("UnReproducible_Items"))
        {
            row.Cells[DCC.IndexOf("UnReproducible_Items")].Style["width"] = "75px";
        }
        if (DCC.Contains("CheckedIn_Items"))
        {
            row.Cells[DCC.IndexOf("CheckedIn_Items")].Style["width"] = "85px";
        }
        if (DCC.Contains("Deployed_Items"))
        {
            row.Cells[DCC.IndexOf("Deployed_Items")].Style["width"] = "85px";
        }
        if (DCC.Contains("Closed_Items"))
        {
            row.Cells[DCC.IndexOf("Closed_Items")].Style["width"] = "115px";
        }

        #endregion Rollup Columns


        #region Rank/Sort columns

        if (!_showSort)
        {
            if (DCC.Contains("Status_Sort"))
            {
                row.Cells[DCC["Status_Sort"].Ordinal].Style["display"] = "none";
            }
            if (DCC.Contains("Priority_Sort"))
            {
                row.Cells[DCC["Priority_Sort"].Ordinal].Style["display"] = "none";
            }
            if (DCC.Contains("WorkType_Sort"))
            {
                row.Cells[DCC["WorkType_Sort"].Ordinal].Style["display"] = "none";
            }
            if (DCC.Contains("ItemType_Sort"))
            {
                row.Cells[DCC["ItemType_Sort"].Ordinal].Style["display"] = "none";
            }
            if (DCC.Contains("System_Sort"))
            {
                row.Cells[DCC["System_Sort"].Ordinal].Style["width"] = "50px";
            }
            //if (DCC.Contains("System_Sort"))
            //{
            //	row.Cells[DCC["System_Sort"].Ordinal].Style["display"] = "none";
            //}
            if (DCC.Contains("WA_ProposedPriority"))
            {
                row.Cells[DCC["WA_ProposedPriority"].Ordinal].Style["display"] = "none";
            }
            if (DCC.Contains("WA_ApprovedPriority"))
            {
                row.Cells[DCC["WA_ApprovedPriority"].Ordinal].Style["display"] = "none";
            }
            if (DCC.Contains("WG_ProposedPriority"))
            {
                row.Cells[DCC["WG_ProposedPriority"].Ordinal].Style["display"] = "none";
            }
            if (DCC.Contains("WG_ApprovedPriority"))
            {
                row.Cells[DCC["WG_ApprovedPriority"].Ordinal].Style["display"] = "none";
            }
            if (DCC.Contains("Version_Sort"))
            {
                row.Cells[DCC["Version_Sort"].Ordinal].Style["display"] = "none";
            }
        }

        #endregion Rank/Sort columns

    }


    Image createShowHideButton_WorkItems(bool show = false, string direction = "Show", string rowNum = "0")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgShowHideWorkItems_click(this,'{0}','{1}');", direction, rowNum);

        Image img = new Image();
        img.ID = string.Format("img{0}WorkItems_{1}", direction, rowNum);
        img.Style["display"] = show ? "block" : "none";
        img.Style["cursor"] = "pointer";
        img.Attributes.Add("Name", string.Format("img{0}WorkItems", direction));
        img.Attributes.Add("rowNum", rowNum);
        img.Height = 10;
        img.Width = 10;
        img.ImageUrl = direction.ToUpper() == "SHOW"
            ? "Images/Icons/add_blue.png"
            : "Images/Icons/minus_blue.png";
        img.ToolTip = string.Format("{0} Work Items", direction);
        img.AlternateText = string.Format("{0} Work Items", direction);
        img.Attributes.Add("Onclick", sb.ToString());

        return img;
    }

    GridViewRow createChildRow(string rowNum = "0", string childType = "WorkItem")
    {
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
        TableCell tableCell = null;

        try
        {
            row.CssClass = "gridBody";
            row.Style["display"] = "none";
            row.ID = string.Format("gridChild_{0}_{1}", rowNum, childType);
            row.Attributes.Add("rowNum", rowNum);
            row.Attributes.Add("childType", childType.ToString());
            row.Attributes.Add("Name", string.Format("gridChild_{0}", rowNum));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC.IndexOf("X"))
                {
                    //set width to match parent
                    tableCell.Style["width"] = "12px";
                    tableCell.Style["border-right"] = "1px solid transparent";
                }
                else if (i == DCC.IndexOf("X") + 1)
                {
                    tableCell.ColumnSpan = DCC.Count - 2;
                    tableCell.Style["padding-top"] = "10px";
                    tableCell.Style["padding-right"] = "10px";
                    tableCell.Style["padding-bottom"] = "0px";
                    tableCell.Style["padding-left"] = "0px";
                    tableCell.Style["vertical-align"] = "top";
                    //add the frame here
                    tableCell.Controls.Add(createChildFrame(parentId: rowNum, childType: childType));
                }
                else
                {
                    tableCell.Style["display"] = "none";
                }

                row.Cells.Add(tableCell);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            row = null;
        }

        return row;
    }

    HtmlIframe createChildFrame(string parentId = "", string childType = "WorkItem")
    {
        HtmlIframe childFrame = new HtmlIframe();

        if (string.IsNullOrWhiteSpace(parentId))
        {
            return null;
        }

        childFrame.ID = string.Format("frameChild_{0}_{1}", parentId, childType);
        childFrame.Attributes.Add("name", "frameWorkItem");
        childFrame.Attributes.Add("parentId", parentId);
        childFrame.Attributes.Add("childType", childType);
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["width"] = "100%";
        childFrame.Style["border"] = "none";

        return childFrame;
    }

    private void RemoveExcelColumns(ref DataTable dt)
    {
        try
        {
            GridColsCollection cols = columnData.VisibleColumns();
            DataColumn col = null;

            for (int i = dt.Columns.Count - 1; i >= 0; i--)
            {
                col = dt.Columns[i];
                if (cols.ItemByColumnName(col.ColumnName) == null)
                {
                    dt.Columns.Remove(col);
                }
                else
                {
                    string columnName = dt.Columns[i].ColumnName.Trim().ToUpper();

                    if (columnName == "X" || columnName == "Y" || columnName.EndsWith("ID") || columnName.EndsWith("_SORT") || columnName.StartsWith("WG_") || columnName.StartsWith("WA_"))
                    {
                        dt.Columns.RemoveAt(i);
                    }
                }

            }

            dt.AcceptChanges();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }
    private void RenameExcelColumns(ref DataTable dt)
    {
        GridColsCollection cols = columnData.VisibleColumns();
        GridColsColumn gcCol = null;
        DataColumn col = null;

        for (int i = dt.Columns.Count - 1; i >= 0; i--)
        {
            col = dt.Columns[i];

            gcCol = cols.ItemByColumnName(col.ColumnName);
            if (gcCol != null)
            {
                dt.Columns[col.ColumnName].ColumnName = gcCol.DisplayName.Replace("<br/>", " ").Replace("<br />", " ");
            }
        }

        dt.AcceptChanges();
    }
    private void FormatExcelRows(ref DataTable dt)
    {
        if (dt.Columns.Contains("Open_Items") && dt.Columns.Contains("Total_Items") && dt.Columns.Contains("Percent_Open_Items"))
        {
            dt.Columns.Add("% Open");
            dt.Columns.Add("% Closed");

            foreach (DataRow row in dt.Rows)
            {
                int open = (int)row[dt.Columns["Open_Items"].Ordinal], closed = 0, total = (int)row[dt.Columns["Total_Items"].Ordinal];
                decimal percentOpen = 0, percentClosed = 0;

                if (dt.Columns.Contains("Closed_Items"))
                {
                    closed = (int)row[dt.Columns["Closed_Items"].Ordinal];
                }
                else
                {
                    closed = total - open;
                }

                percentOpen = total == 0 ? 0 : (100 * (Math.Round((decimal)((decimal)open / (decimal)total), 3, MidpointRounding.AwayFromZero)));
                percentOpen = Math.Round(percentOpen, 1);
                percentClosed = (100 - percentOpen);

                row[dt.Columns["% Open"].Ordinal] = percentOpen.ToString();
                row[dt.Columns["% Closed"].Ordinal] = percentClosed.ToString();
            }

            dt.Columns["% Open"].SetOrdinal(dt.Columns["Percent_Open_Items"].Ordinal);
            dt.Columns["% Closed"].SetOrdinal(dt.Columns["Percent_Closed_Items"].Ordinal);
            dt.Columns.Remove("Percent_Open_Items");
            dt.Columns.Remove("Percent_Closed_Items");
            dt.Columns["% Open"].ColumnName = "Percent_Open_Items";
            dt.Columns["% Closed"].ColumnName = "Percent_Closed_Items";
        }

        if (dt.Columns.Contains("OnHold_Items") && dt.Columns.Contains("Total_Items") && dt.Columns.Contains("Percent_OnHold_Items"))
        {
            dt.Columns.Add("% On Hold");

            foreach (DataRow row in dt.Rows)
            {
                int onHold = (int)row[dt.Columns["OnHold_Items"].Ordinal], total = (int)row[dt.Columns["Total_Items"].Ordinal];
                decimal percentOnHold = 0;

                percentOnHold = total == 0 ? 0 : (100 * (Math.Round((decimal)((decimal)onHold / (decimal)total), 3, MidpointRounding.AwayFromZero)));
                percentOnHold = Math.Round(percentOnHold, 1);

                row[dt.Columns["% On Hold"].Ordinal] = percentOnHold.ToString();
            }

            dt.Columns["% On Hold"].SetOrdinal(dt.Columns["Percent_OnHold_Items"].Ordinal);
            dt.Columns.Remove("Percent_OnHold_Items");
            dt.Columns["% On Hold"].ColumnName = "Percent_OnHold_Items";
        }

        //append sub task counts
        for (int i = 0; i <= dt.Rows.Count - 1; i++)
        {
            for (int j = 0; j <= dt.Columns.Count - 1; j++)
            {
                string colName = dt.Columns[j].ColumnName;
                if (colName.EndsWith("_Items") && dt.Columns.Contains(colName + "_Sub") && !colName.StartsWith("Percent_"))
                {
                    if (i == 0)
                    {
                        dt.Columns.Add(colName + "_Temp");
                    }

                    dt.Rows[i][colName + "_Temp"] = dt.Rows[i][dt.Columns[j]].ToString() + " / " + dt.Rows[i][colName + "_Sub"].ToString();

                    if (i == dt.Rows.Count - 1)
                    {
                        dt.Columns[colName + "_Temp"].SetOrdinal(dt.Columns[j].Ordinal);
                        dt.Columns.Remove(colName);
                        dt.Columns[colName + "_Temp"].ColumnName = colName;
                    }
                }
            }
        }

        if (dt.Columns.Contains("Percent_Open_Items") && dt.Columns.Contains("Open_Items_Sub") && dt.Columns.Contains("Total_Items_Sub") && dt.Columns.Contains("Percent_Open_Items_Sub"))
        {
            foreach (DataRow row in dt.Rows)
            {
                int open = (int)row[dt.Columns["Open_Items_Sub"].Ordinal], closed = 0, total = (int)row[dt.Columns["Total_Items_Sub"].Ordinal];
                decimal percentOpen = 0, percentClosed = 0;

                if (dt.Columns.Contains("Closed_Items_Sub"))
                {
                    closed = (int)row[dt.Columns["Closed_Items_Sub"].Ordinal];
                }
                else
                {
                    closed = total - open;
                }

                percentOpen = total == 0 ? 0 : (100 * (Math.Round((decimal)((decimal)open / (decimal)total), 3, MidpointRounding.AwayFromZero)));
                percentOpen = Math.Round(percentOpen, 1);
                percentClosed = (100 - percentOpen);

                row[dt.Columns["Percent_Open_Items"].Ordinal] = row[dt.Columns["Percent_Open_Items"].Ordinal].ToString() + " / " + percentOpen.ToString();
                row[dt.Columns["Percent_Closed_Items"].Ordinal] = row[dt.Columns["Percent_Closed_Items"].Ordinal].ToString() + " / " + percentClosed.ToString();
            }

            dt.Columns.Remove("Percent_Open_Items_Sub");
            dt.Columns.Remove("Percent_Closed_Items_Sub");
        }

        if (dt.Columns.Contains("Percent_OnHold_Items") && dt.Columns.Contains("OnHold_Items_Sub") && dt.Columns.Contains("Total_Items_Sub") && dt.Columns.Contains("Percent_OnHold_Items_Sub"))
        {
            foreach (DataRow row in dt.Rows)
            {
                int onHold = (int)row[dt.Columns["OnHold_Items_Sub"].Ordinal], total = (int)row[dt.Columns["Total_Items_Sub"].Ordinal];
                decimal percentOnHold = 0;

                percentOnHold = total == 0 ? 0 : (100 * (Math.Round((decimal)((decimal)onHold / (decimal)total), 3, MidpointRounding.AwayFromZero)));
                percentOnHold = Math.Round(percentOnHold, 1);

                row[dt.Columns["Percent_OnHold_Items"].Ordinal] = row[dt.Columns["Percent_OnHold_Items"].Ordinal].ToString() + " / " + percentOnHold.ToString();
            }

            dt.Columns.Remove("Percent_OnHold_Items_Sub");
        }

        dt.AcceptChanges();
    }
    private void AddHeaderCounts(ref DataTable dt)
    {
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            if (dt.Columns[i].ColumnName == "# Tasks") dt.Columns[i].ColumnName += " (" + TaskCount + " / " + SubTaskCount + ")";
            if (dt.Columns[i].ColumnName == "Open") dt.Columns[i].ColumnName += " (" + OpenCount + " / " + SubOpenCount + ")";
            if (dt.Columns[i].ColumnName == "On Hold") dt.Columns[i].ColumnName += " (" + OnHoldCount + " / " + SubOnHoldCount + ")";
            if (dt.Columns[i].ColumnName == "Info Requested") dt.Columns[i].ColumnName += " (" + InfoRequestedCount + " / " + SubInfoRequestedCount + ")";
            if (dt.Columns[i].ColumnName == "New") dt.Columns[i].ColumnName += " (" + NewCount + " / " + SubNewCount + ")";
            if (dt.Columns[i].ColumnName == "In Progress") dt.Columns[i].ColumnName += " (" + InProgressCount + " / " + SubInProgressCount + ")";
            if (dt.Columns[i].ColumnName == "Re-Opened") dt.Columns[i].ColumnName += " (" + ReOpenedCount + " / " + SubReOpenedCount + ")";
            if (dt.Columns[i].ColumnName == "Info Provided") dt.Columns[i].ColumnName += " (" + InfoProvidedCount + " / " + SubInfoProvidedCount + ")";
            if (dt.Columns[i].ColumnName == "Un- Reproducible") dt.Columns[i].ColumnName += " (" + UnReproducibleCount + " / " + SubUnReproducibleCount + ")";
            if (dt.Columns[i].ColumnName == "Checked In") dt.Columns[i].ColumnName += " (" + CheckedInCount + " / " + SubCheckedInCount + ")";
            if (dt.Columns[i].ColumnName == "Deployed") dt.Columns[i].ColumnName += " (" + DeployedCount + " / " + SubDeployedCount + ")";
            if (dt.Columns[i].ColumnName == "Closed") dt.Columns[i].ColumnName += " (" + ClosedCount + " / " + SubClosedCount + ")";
            if (dt.Columns[i].ColumnName == "High") dt.Columns[i].ColumnName += " (" + HighCount + " / " + SubHighCount + ")";
            if (dt.Columns[i].ColumnName == "Medium") dt.Columns[i].ColumnName += " (" + MediumCount + " / " + SubMediumCount + ")";
            if (dt.Columns[i].ColumnName == "Low") dt.Columns[i].ColumnName += " (" + LowCount + " / " + SubLowCount + ")";
            if (dt.Columns[i].ColumnName == "N/A") dt.Columns[i].ColumnName += " (" + NACount + " / " + SubNACount + ")";
        }

        dt.AcceptChanges();
    }

    #region Excel
    private bool ExportExcel(DataTable dt, string SelectedStatus = "-1", string SelectedAssigned = "-1")
    {
        bool success = false;
        string errorMsg = string.Empty;
        // SCB 2-9-17 - Declare these here now, not in the loop below.
        DataTable dtDrilldown = null;
        DataTable dtDrilldownTask = null;

        if (SelectedStatus == string.Empty) //if no status or assigned resource are selected, then everything should be filtered out. Use of value of -1 to accomplish that.
        {
            SelectedStatus = "-1";
        }
        if (SelectedAssigned == string.Empty)
        {
            SelectedAssigned = "-1";
        }

        string quickFilters = "(STATUSID IN (" + SelectedStatus + ") AND ASSIGNEDRESOURCEID IN (" + SelectedAssigned + "))";

        try
        {
            Workbook wb = new Workbook(FileFormatType.Xlsx);
            wb.Worksheets.Add();
            Worksheet ws = wb.Worksheets[1];
            ws.Name = "QM Workload Crosswalk";
            Worksheet wsRaw = wb.Worksheets[0];
            wsRaw.Name = "Workload Raw";
            StyleFlag flag = new StyleFlag() { All = true };

            string[] columns = this.ParentColumns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] drilldownColumns = this.ChildColumns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            DataTable dtLookup = dt.Copy();
            DataTable dtExcel = new DataTable();
            DataTable dtDrillDownAll = new DataTable();
            DataTable dtDrillDownTaskAll = new DataTable();

            int iShowClosed = 0;
            if (ShowClosed)
                iShowClosed = 1;
            else
                iShowClosed = 0;

            DataTable dtList = WorkloadItem.WorkItemList_Get_QF(workRequestID: 0, showArchived: _showArchived, columnListOnly: 0, myData: _myData, ShowBacklog: ShowBacklog, showClosed: iShowClosed, SelectedAssigned: SelectedAssigned, SelectedStatuses: SelectedStatus);
            DataTable dtSub = WorkloadItem.WorkItem_GetTaskList(showArchived: _showArchived, showBacklog: ShowBacklog);

            drilldownColumns = drilldownColumns.Where(w => w != "X" && w != "Y").ToArray();

            FormatExcelRows(ref dt);
            RemoveExcelColumns(ref dt);
            RenameExcelColumns(ref dt);
            AddHeaderCounts(ref dt);
            dtExcel = dt.Clone();

            for (int i = 0; i <= dtExcel.Columns.Count - 1; i++)
            {
                dtExcel.Columns[i].DataType = typeof(string);
                dtExcel.Columns[i].MaxLength = 255;
                dtExcel.Columns[i].AllowDBNull = true;
            }

            for (int j = 0; j <= dt.Rows.Count - 1; j++)
            {
                dtExcel.ImportRow(dt.Rows[j]);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < columns.Length; i++)
                {
                    switch (columns[i].Trim().ToUpper())
                    {
                        case "WORKREQUEST":
                            sb.AppendFormat("{0}WorkRequest={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["WORKREQUESTID"].ToString().Trim());
                            break;
                        case "WORKTYPE":
                            sb.AppendFormat("{0}Work Type={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["WorkTypeID"].ToString().Trim());
                            break;
                        case "ITEMTYPE":
                            sb.AppendFormat("{0}Item Type={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["WORKITEMTYPEID"].ToString().Trim());
                            break;
                        case "SYSTEM":
                            sb.AppendFormat("{0}System={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["WTS_SYSTEMID"].ToString().Trim());
                            break;
                        case "ALLOCATIONGROUP":
                            sb.AppendFormat("{0}Allocation Group={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["AllocationGroupID"].ToString().Trim());
                            break;
                        case "ALLOCATIONCATEGORY":
                            sb.AppendFormat("{0}Allocation Category={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["AllocationCategoryID"].ToString().Trim());
                            break;
                        case "ALLOCATIONASSIGNMENT":
                            sb.AppendFormat("{0}Allocation Assignment={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["AllocationID"].ToString().Trim());
                            break;
                        case "WORKAREA":
                            sb.AppendFormat("{0}Work Area={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["WorkAreaID"].ToString().Trim());
                            break;
                        case "WORKLOADGROUP":
                        case "FUNCTIONALITY":
                            sb.AppendFormat("{0}Workload Group={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["WorkloadGroupID"].ToString().Trim());
                            break;
                        case "VERSION":
                            sb.AppendFormat("{0}Release Version={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["ProductVersionID"].ToString().Trim());
                            break;
                        case "AFFILIATED":
                            sb.AppendFormat("{0}Affiliated={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["AffiliatedID"].ToString().Trim());
                            break;
                        case "ASSIGNEDTO":
                            sb.AppendFormat("{0}Workload Assigned To={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["AssignedResourceID"].ToString().Trim());
                            break;
                        case "PRIMARYDEVELOPER":
                            sb.AppendFormat("{0}Primary Developer={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["PRIMARYRESOURCEID"].ToString());
                            break;
                        case "SUBMITTEDBY":
                            sb.AppendFormat("{0}Workload Submitted By={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["WorkloadSubmittedByID"].ToString().Trim());
                            break;
                        case "PRIORITY":
                            sb.AppendFormat("{0}Workload Priority={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["PriorityID"].ToString().Trim());
                            break;
                        case "STATUS":
                            sb.AppendFormat("{0}Workload Status={1}", sb.Length > 0 ? "," : "", dtLookup.Rows[j]["STATUSID"].ToString().Trim());
                            break;
                    }
                }

                string[] filterArray = sb.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                dtDrilldown = dtList.Copy();
                applyRowFilters(dt: ref dtDrilldown, filterArray: filterArray);

                int intShowClosed = 0;
                if (ShowClosed)
                    intShowClosed = 1;
                else
                    intShowClosed = 0;

                if (dtDrilldown == null)
                {
                    dtDrilldown = WorkloadItem.WorkItemList_Get_QF(workRequestID: 0, showArchived: _showArchived, columnListOnly: 0, myData: _myData, ShowBacklog: ShowBacklog, showClosed: intShowClosed, SelectedAssigned: SelectedAssigned, SelectedStatuses: SelectedStatus);
                }

                string empty = string.Empty;
                if (dtDrilldown != null && dtDrilldown.Rows.Count > 0)
                {
                    int count = 0;
                    foreach (DataRow drDrilldown in dtDrilldown.Rows)
                    {
                        if (count == 0)
                        {
                            DataRow drSpacer = dtExcel.NewRow();
                            DataRow drHeader = dtExcel.NewRow();
                            drSpacer[0] = "spacer";
                            drHeader[0] = "";

                            for (int k = 0; k <= drilldownColumns.Length - 1; k++)
                            {
                                if (k + 1 > dtExcel.Columns.Count - 1)
                                {
                                    dtExcel.Columns.Add(empty + " ");
                                    empty = empty + " ";
                                }

                                string title = drilldownColumns[k];
                                switch (title)
                                {
                                    case "WorkItem":
                                    case "TaskNumber":
                                        title = "Task #";
                                        break;
                                    case "TaskCount":
                                        title = "# Tasks";
                                        break;
                                    case "PrimaryBus. Rank":
                                        title = "Primary Bus. Rank";
                                        break;
                                    case "PrimaryTech. Rank":
                                        title = "Primary Tech. Rank";
                                        break;
                                    case "RequiresIVT":
                                        title = "Requires IVT";
                                        break;
                                    case "TimesRe-Opened":
                                        title = "Times Re-Opened";
                                        break;
                                    case "StatusUpdated Date":
                                        title = "Status Updated Date";
                                        break;
                                    case "TITLE":
                                        title = "Title";
                                        break;
                                    default:
                                        title = System.Text.RegularExpressions.Regex.Replace(title, "[A-Z]", " $0").Trim();
                                        break;
                                }
                                drHeader[k + 1] = title;
                            }
                            dtExcel.Rows.Add(drHeader);
                        }

                        DataRow dr = dtExcel.NewRow();
                        dr[0] = "";

                        for (int k = 0; k <= drilldownColumns.Length - 1; k++)
                        {
                            switch (drilldownColumns[k])
                            {
                                case "WorkItem":
                                case "TaskNumber":
                                    dr[k + 1] = drDrilldown["ItemID"].ToString();
                                    break;
                                case "WorkType":
                                    dr[k + 1] = drDrilldown["WorkType"].ToString();
                                    break;
                                case "TaskCount":
                                    dr[k + 1] = drDrilldown["Task_Count"].ToString();
                                    break;
                                case "System":
                                    dr[k + 1] = drDrilldown["Websystem"].ToString();
                                    break;
                                case "Status":
                                    dr[k + 1] = drDrilldown["STATUS"].ToString();
                                    break;
                                case "Description":
                                case "Title":
                                    dr[k + 1] = drDrilldown["TITLE"].ToString();
                                    break;
                                case "AllocationGroup":
                                    dr[k + 1] = drDrilldown["AllocationGroup"].ToString();
                                    break;
                                case "AllocationCategory":
                                    dr[k + 1] = drDrilldown["AllocationCategory"].ToString();
                                    break;
                                case "AllocationAssignment":
                                    dr[k + 1] = drDrilldown["ALLOCATION"].ToString();
                                    break;
                                case "WorkloadGroup":
                                case "Functionality":
                                    dr[k + 1] = drDrilldown["WorkloadGroup"].ToString();
                                    break;
                                case "WorkArea":
                                    dr[k + 1] = drDrilldown["WorkArea"].ToString();
                                    break;
                                case "ProductionStatus":
                                    dr[k + 1] = drDrilldown["ProductionStatus"].ToString();
                                    break;
                                case "Version":
                                    dr[k + 1] = drDrilldown["Version"].ToString();
                                    break;
                                case "Priority":
                                    dr[k + 1] = drDrilldown["PRIORITY"].ToString();
                                    break;
                                case "PrimaryAnalyst":
                                    dr[k + 1] = drDrilldown["Primary_Analyst"].ToString();
                                    break;
                                case "PrimaryDeveloper":
                                    dr[k + 1] = drDrilldown["Primary_Developer"].ToString();
                                    break;
                                case "AssignedTo":
                                    dr[k + 1] = drDrilldown["Assigned"].ToString();
                                    break;
                                case "SubmittedBy":
                                    dr[k + 1] = drDrilldown["SubmittedBy"].ToString();
                                    break;
                                case "Progress":
                                    dr[k + 1] = drDrilldown["Progress"].ToString();
                                    break;
                                case "TimesRe-Opened":
                                    dr[k + 1] = drDrilldown["ReOpenedCount"].ToString();
                                    break;
                                case "StatusUpdated Date":
                                    dr[k + 1] = String.Format("{0:M/d/yyyy}", drDrilldown["StatusUpdatedDate"].ToString());
                                    break;
                                case "PrimaryBus. Rank":
                                    dr[k + 1] = drDrilldown["PrimaryBusinessRank"].ToString();
                                    break;
                                case "PrimaryTech. Rank":
                                    dr[k + 1] = drDrilldown["RESOURCEPRIORITYRANK"].ToString();
                                    break;
                                case "ItemType":
                                    dr[k + 1] = drDrilldown["WORKITEMTYPE"].ToString();
                                    break;
                                case "RequiresIVT":
                                    dr[k + 1] = drDrilldown["IVTRequired"].ToString().ToUpper() == "TRUE" ? "Yes" : "No";
                                    break;
                                case "DateNeeded":
                                    dr[k + 1] = String.Format("{0:M/d/yyyy}", drDrilldown["NEEDDATE"].ToString());
                                    break;
                                case "SRNumber":
                                    dr[k + 1] = drDrilldown["SR_Number"].ToString();
                                    break;
                            }
                        }

                        dtExcel.Rows.Add(dr);
                        WTSUtility.importDataRow(ref dtDrillDownAll, drDrilldown);


                        int itemID = 0;
                        int.TryParse(drDrilldown["ItemID"].ToString(), out itemID);
                        if (itemID > 0)
                        {
                            // SCB 2-9-2017 dtSub has ALL Tasks, get from it using rowfilter.
                            dtSub.DefaultView.RowFilter = "WORKITEMID = " + itemID;
                            dtDrilldownTask = dtSub.DefaultView.ToTable();
                            //DataTable dtDrilldownTask = WorkloadItem.WorkItem_GetTaskList(workItemID: itemID, showArchived: _showArchived, showBacklog: ShowBacklog);


                            if (dtDrilldownTask != null && dtDrilldownTask.Rows.Count > 0)
                            {
                                try
                                {
                                    string[] filterArraySub = sb.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    string nFilter = string.Empty;
                                    for (int i = 0; i < filterArraySub.Length; i++)
                                    {
                                        if (filterArraySub[i].Contains("Workload Assigned To"))
                                        {
                                            nFilter = filterArraySub[i].Replace("Workload Assigned To", "ASSIGNEDRESOURCEID");
                                        }
                                    }
                                    if (nFilter != string.Empty)
                                    {
                                        dtDrilldownTask.DefaultView.RowFilter = nFilter;
                                        dtDrilldownTask = dtDrilldownTask.DefaultView.ToTable();
                                    }
                                    if (quickFilters != string.Empty)
                                    {
                                        dtDrilldownTask.DefaultView.RowFilter = quickFilters;
                                        dtDrilldownTask = dtDrilldownTask.DefaultView.ToTable();
                                    }
                                }
                                catch (Exception) { }

                                for (int l = 0; l < 13; l++)
                                {
                                    if (l > dtExcel.Columns.Count - 1)
                                    {
                                        dtExcel.Columns.Add(empty + " ");
                                        empty = empty + " ";
                                    }
                                }

                                foreach (DataRow drChild in dtDrilldownTask.Rows)
                                {
                                    dtExcel.Rows.Add("", "", drChild["WORKITEMID"].ToString() + " - " + drChild["TASK_NUMBER"].ToString(), drChild["BusinessRank"].ToString(), drChild["SORT_ORDER"].ToString(), drChild["PRIORITY"].ToString(), drChild["Title"], drChild["AssignedResource"], String.Format("{0:M/d/yyyy}", drChild["ESTIMATEDSTARTDATE"].ToString()), String.Format("{0:M/d/yyyy}", drChild["ACTUALSTARTDATE"].ToString()), drChild["PLANNEDHOURS"].ToString(), drChild["ACTUALHOURS"].ToString(), String.Format("{0:M/d/yyyy}", drChild["ACTUALENDDATE"].ToString()), drChild["COMPLETIONPERCENT"].ToString(), drChild["STATUS"], drChild["ReOpenedCount"].ToString());
                                    WTSUtility.importDataRow(ref dtDrillDownTaskAll, drChild);
                                }
                            }
                        }
                        count++;
                    }
                }
            }
            string name = "QM Workload Crosswalk";
            ws.Cells.ImportDataTable(dtExcel, true, 0, 0, false, false);

            Aspose.Cells.Style LightGreen = new Aspose.Cells.Style();
            Aspose.Cells.Style LightOrange = new Aspose.Cells.Style();
            Aspose.Cells.Style LightBlue = new Aspose.Cells.Style();

            LightOrange.Pattern = BackgroundType.Solid;
            LightOrange.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#ffccb3");
            LightBlue.Pattern = BackgroundType.Solid;
            LightBlue.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#cce6ff");
            LightGreen.Pattern = BackgroundType.Solid;
            LightGreen.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");

            for (int j = 0; j <= ws.Cells.Rows.Count - 1; j++)
            {
                try
                {
                    if (ws.Cells.Rows[j][0].Value.ToString() != "" & j != 0)
                    {   // Light green to header (second row)
                        Range rangeDrilldown = ws.Cells.CreateRange(j, 0, 1, drilldownColumns.Length);
                        rangeDrilldown.ApplyStyle(LightGreen, flag);
                    }
                    if (ws.Cells.Rows[j][1].Value.ToString() == "Task #" || ws.Cells.Rows[j][1].Value.ToString() == "Primary Tech. Rank" || ws.Cells.Rows[j][2].Value.ToString() == "Task #")
                    {   // Light orange to Tasks
                        Range rangeDrilldown = ws.Cells.CreateRange(j, 1, 1, drilldownColumns.Length - 1);
                        rangeDrilldown.ApplyStyle(LightOrange, flag);
                    }
                    if (ws.Cells.Rows[j][2].Value.ToString() == "Sub-Task #")
                    {   // Light blue to Sub Tasks
                        Range rangeDrilldownTask = ws.Cells.CreateRange(j, 2, 1, drilldownColumns.Length - 2);
                        rangeDrilldownTask.ApplyStyle(LightBlue, flag);
                    }
                }
                catch (Exception ex)
                {
                    String s = ex.Message; // Ignore errors here.
                    //throw;
                }
            }
            formatParent(ref dtDrillDownAll);
            formatChild(ref dtDrillDownTaskAll);

            DataTable dtRaw = WTSUtility.JoinDataTables(dtDrillDownAll, dtDrillDownTaskAll,
                (row1, row2) =>
                row1.Field<String>("ItemID") == row2.Field<String>("WORKITEMID"));

            formatRawData(ref dtRaw);
            ws.AutoFitColumns();
            wsRaw.Cells.ImportDataTable(dtRaw, true, "A1");
            wsRaw.AutoFitColumns();

            // Start row, start column, num rows to freeze, num cols to freeze
            ws.FreezePanes(1, 1, 1, 1);
            wsRaw.FreezePanes(1, 1, 1, 1);

            MemoryStream ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Xlsx);

            Response.ContentType = "application/xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
            Response.BinaryWrite(ms.ToArray());
            success = true;
            Response.End();
        }
        catch (Exception ex)
        {
            success = false;
            errorMsg += Environment.NewLine + ex.Message;
        }
        return success;
    }

    private void formatParent(ref DataTable dtDrillDownAll)
    {
        dtDrillDownAll.Columns["STATUS"].ColumnName = "Task Status";
        dtDrillDownAll.Columns["TITLE"].ColumnName = "Task Title";
        dtDrillDownAll.Columns["DESCRIPTION"].ColumnName = "Task Description";
        dtDrillDownAll.Columns["PRIORITY"].ColumnName = "Task Priority";
        dtDrillDownAll.Columns["Assigned"].ColumnName = "Task Assigned To";
        dtDrillDownAll.Columns["Primary_Developer"].ColumnName = "Task Primary Dev";
        dtDrillDownAll.Columns["PrimaryBusinessResource"].ColumnName = "Task Primary Bus";
        dtDrillDownAll.Columns["SECONDARYRESOURCE"].ColumnName = "Task Secondary Resource";
        dtDrillDownAll.Columns["CREATEDBY"].ColumnName = "Task Created By";
        dtDrillDownAll.Columns["SubmittedBy"].ColumnName = "Task Submitted By";
        dtDrillDownAll.AcceptChanges();
    }

    private void formatChild(ref DataTable dtDrillDownTaskAll)
    {
        dtDrillDownTaskAll.Columns["STATUS"].ColumnName = "Sub Task Status";
        dtDrillDownTaskAll.Columns["TITLE"].ColumnName = "Sub Task Title";
        dtDrillDownTaskAll.Columns["DESCRIPTION"].ColumnName = "Sub Task Description";
        dtDrillDownTaskAll.Columns["PRIORITY"].ColumnName = "Sub Task Priority";
        dtDrillDownTaskAll.Columns["CREATEDBY"].ColumnName = "Sub Task Created By";
        dtDrillDownTaskAll.Columns["UPDATEDBY"].ColumnName = "Sub Task Updated By";
        dtDrillDownTaskAll.Columns["SubmittedBy"].ColumnName = "Sub Task Submitted By";
        dtDrillDownTaskAll.Columns["AssignedResource"].ColumnName = "Sub Task Assigned To";
        dtDrillDownTaskAll.AcceptChanges();
    }

    private void formatRawData(ref DataTable dtRaw)
    {
        dtRaw.Columns.Remove("X");
        dtRaw.Columns.Remove("Y");
        dtRaw.Columns.Remove("Z");
        dtRaw.Columns.Remove("ItemID");
        dtRaw.Columns.Remove("WorkTypeID");
        dtRaw.Columns.Remove("Task_Count");
        dtRaw.Columns.Remove("WTS_SYSTEMID");
        dtRaw.Columns.Remove("STATUSID");
        dtRaw.Columns.Remove("NEEDDATE");
        dtRaw.Columns.Remove("AllocationGroupID");
        dtRaw.Columns.Remove("AllocationCategoryID");
        dtRaw.Columns.Remove("ALLOCATIONID");
        //dtRaw.Columns.Remove("RESOURCEPRIORITYRANKID");
        //dtRaw.Columns.Remove("PrimaryBusinessRankID");
        dtRaw.Columns.Remove("WorkAreaID");
        dtRaw.Columns.Remove("WorkloadGroupID");
        dtRaw.Columns.Remove("ProductionStatusID");
        dtRaw.Columns.Remove("PriorityID");
        dtRaw.Columns.Remove("ASSIGNEDRESOURCEID");
        dtRaw.Columns.Remove("SMEID");
        dtRaw.Columns.Remove("PRIMARYRESOURCEID");
        dtRaw.Columns.Remove("PrimaryBusinessResourceID");
        dtRaw.Columns.Remove("SECONDARYRESOURCEID");
        dtRaw.Columns.Remove("CREATEDDATE");
        dtRaw.Columns.Remove("SubmittedByID");
        dtRaw.Columns.Remove("Status_Sort");
        dtRaw.Columns.Remove("ReOpenedCOunt");
        dtRaw.Columns.Remove("StatusUpdatedDate");
        dtRaw.Columns.Remove("WORKITEM_TASKID");
        dtRaw.Columns.Remove("ESTIMATEDSTARTDATE");
        dtRaw.Columns.Remove("ACTUALSTARTDATE");
        dtRaw.Columns.Remove("EstimatedEffortID");
        dtRaw.Columns.Remove("ActualEffortID");
        dtRaw.Columns.Remove("UPDATEDDATE");
        dtRaw.Columns.Remove("SORT_ORDER");
        dtRaw.Columns.Remove("ACTUALENDDATE");
        dtRaw.Columns.Remove("WORKITEMTYPEID");
        dtRaw.Columns.Remove("IVTRequired");
        dtRaw.Columns.Remove("AllocationCategory");
        dtRaw.Columns.Remove("Production");
        dtRaw.Columns.Remove("ProductVersionID");
        dtRaw.Columns.Remove("ARCHIVE");
        dtRaw.Columns["WORKITEMID"].SetOrdinal(0);
        dtRaw.Columns["WORKITEMID"].ColumnName = "Task #";
        dtRaw.Columns["TASK_NUMBER"].SetOrdinal(1);
        dtRaw.Columns["TASK_NUMBER"].ColumnName = "Sub Task #";
        dtRaw.Columns["WORKREQUEST"].ColumnName = "Work Request";
        dtRaw.Columns["WORKITEMTYPE"].ColumnName = "Item Type";
        dtRaw.Columns["WorkType"].ColumnName = "Work Type";
        dtRaw.Columns["AllocationGroup"].ColumnName = "Allocation Group";
        dtRaw.Columns["ALLOCATION"].ColumnName = "Allocation Assignment";
        dtRaw.Columns["RESOURCEPRIORITYRANK"].ColumnName = "Task Primary Tech Rank";
        dtRaw.Columns["PrimaryBusinessRank"].ColumnName = "Task Primary Bus Rank";
        dtRaw.Columns["WorkArea"].ColumnName = "Work Area";
        dtRaw.Columns["WorkloadGroup"].ColumnName = "Functionality";
        dtRaw.Columns["Version"].ColumnName = "Product Version";
        dtRaw.Columns["ProductionStatus"].ColumnName = "Production Status";
        dtRaw.Columns["SR_Number"].ColumnName = "SR Number";
        dtRaw.Columns["Primary_Analyst"].ColumnName = "Primary Resource";
        dtRaw.Columns["BusinessRank"].ColumnName = "Sub Task Bus Priority Rank";
        dtRaw.Columns["PrimaryResource"].ColumnName = "Sub Task Primary Resource";
        dtRaw.Columns["PLANNEDHOURS"].ColumnName = "Planned Hours";
        dtRaw.Columns["ACTUALHOURS"].ColumnName = "Actual Hours";
        dtRaw.Columns["COMPLETIONPERCENT"].ColumnName = "Percent Complete";
        dtRaw.Columns["Task Status"].SetOrdinal(2);
        dtRaw.Columns["Task Title"].SetOrdinal(3);
        dtRaw.Columns["Task Description"].SetOrdinal(4);
        dtRaw.Columns["Task Priority"].SetOrdinal(5);
        dtRaw.Columns["Task Assigned To"].SetOrdinal(6);
        dtRaw.Columns["Task Primary Dev"].SetOrdinal(7);
        dtRaw.Columns["Task Primary Bus"].SetOrdinal(8);
        dtRaw.Columns["Task Secondary Resource"].SetOrdinal(9);
        dtRaw.Columns["Task Created By"].SetOrdinal(10);
        dtRaw.Columns["Task Submitted By"].SetOrdinal(11);
        dtRaw.Columns["Sub Task Status"].SetOrdinal(12);
        dtRaw.Columns["Sub Task Title"].SetOrdinal(13);
        dtRaw.Columns["Sub Task Description"].SetOrdinal(14);
        dtRaw.Columns["Sub Task Priority"].SetOrdinal(15);
        dtRaw.Columns["Sub Task Created By"].SetOrdinal(16);
        dtRaw.Columns["Sub Task Updated By"].SetOrdinal(17);
        dtRaw.Columns["Sub Task Submitted By"].SetOrdinal(18);
        dtRaw.Columns["Sub Task Assigned To"].SetOrdinal(19);

        dtRaw.Columns.Add("Sort1", Type.GetType("System.Int32")); //hack to fix sorting problems
        dtRaw.Columns.Add("Sort2", Type.GetType("System.Int32"));

        foreach (DataRow r in dtRaw.Rows)
        {
            r["Sort1"] = Convert.ToInt32(r["Task #"]);
            r["Sort2"] = Convert.ToInt32(r["Sub Task #"]);
            r["Sub Task #"] = r["Task #"] + "-" + r["Sub Task #"];
            r["Task Title"] = System.Text.RegularExpressions.Regex.Replace(r["Task Title"].ToString(), @"<[^>]+>|&nbsp;", "").Trim(); //remove HTML tags
            r["Sub Task Title"] = System.Text.RegularExpressions.Regex.Replace(r["Sub Task Title"].ToString(), @"<[^>]+>|&nbsp;", "").Trim();
            r["Task Description"] = System.Text.RegularExpressions.Regex.Replace(r["Task Description"].ToString(), @"<[^>]+>|&nbsp;", "").Trim();
            r["Sub Task Description"] = System.Text.RegularExpressions.Regex.Replace(r["Sub Task Description"].ToString(), @"<[^>]+>|&nbsp;", "").Trim();
        }

        DataView dv = dtRaw.DefaultView;
        dv.Sort = "[Sort1] asc, [Sort2] asc";
        dtRaw = dv.ToTable();
        DataRow previousRow = dtRaw.Rows[dtRaw.Rows.Count - 1];
        for(int i = dtRaw.Rows.Count-2; i >= 0; i--) //renove duplicate rows ie select distinct by sub task #.
        {
            if (dtRaw.Rows[i]["Sub Task #"].ToString() == previousRow["Sub Task #"].ToString())
            {
                dtRaw.Rows[i].Delete();
            }
            else
            {
                previousRow = dtRaw.Rows[i];
            }
        }
        dtRaw.Columns.Remove("Sort1");
        dtRaw.Columns.Remove("Sort2");
        dtRaw.AcceptChanges();
    }
    #endregion Excel

    private void loadMenus()
    {
        try
        {
            DataSet dsMenu = new DataSet();
            dsMenu.ReadXml(this.Server.MapPath("XML/WTS_Menus.xml"));

            if (dsMenu.Tables.Count > 0 && dsMenu.Tables[0].Rows.Count > 0)
            {
                if (dsMenu.Tables.Contains("WorkloadCrosswalkGridRelatedItem"))
                {
                    //iti_Tools_Sharp.itiMenuItem item = null;
                    //iti_Tools_Sharp.itiMenu menu = new iti_Tools_Sharp.itiMenu();
                    //menuRelatedItems.Menus.Add(menu);

                    //foreach(DataRow row in dsMenu.Tables["WWorkloadCrosswalkGridRelatedItem"].Rows){
                    //	item = new iti_Tools_Sharp.itiMenuItem();
                    //	//menu = new iti_Tools_Sharp.itiMenu();

                    //	item.ID = row["id"].ToString();
                    //	item.Value = row["URL"].ToString();
                    //	item.Text = row["Text"].ToString();
                    //	if (dsMenu.Tables["WorkloadCrosswalkGridRelatedItem"].Columns.Contains("WorkloadCrosswalkGridRelatedItem_id_0"))
                    //	{
                    //		item.ParentID = row["WorkloadCrosswalkGridRelatedItem_id_0"].ToString();
                    //	}
                    //	item.ImageSource = row["ImageType"].ToString();
                    //	item.Enabled = (row["Disabled"].ToString().ToUpper() != "TRUE");
                    //	menuRelatedItems.Menus[0].Items.Add(item);
                    //}
                    menuRelatedItems.DataSource = dsMenu.Tables["WorkloadCrosswalkGridRelatedItem"];
                    menuRelatedItems.DataValueField = "URL";
                    menuRelatedItems.DataTextField = "Text";
                    menuRelatedItems.DataIDField = "id";
                    if (dsMenu.Tables["WorkloadCrosswalkGridRelatedItem"].Columns.Contains("WorkloadCrosswalkGridRelatedItem_id_0"))
                    {
                        menuRelatedItems.DataParentIDField = "WorkloadCrosswalkGridRelatedItem_id_0";
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

    [WebMethod(true)]
    public static string LoadMetrics(string SelectedStatus, string SelectedUsers, int showArchived = 0, bool myData = false)
    {
        SelectedStatus = SelectedStatus.Substring(1, SelectedStatus.Length - 2).ToString();
        SelectedUsers = SelectedUsers.Substring(1, SelectedUsers.Length - 2).ToString();

        // 11-23-2016 - This fixed the WORKITEM_TASKS, Workload_Metrics_Get works for only WORKITEM's
        DataTable dtSub = Workload.Workload_Sub_Metrics_Get(SelectedStatus: SelectedStatus, SelectedUsers: SelectedUsers, metricType: "QMWorkloadCrosswalk", showArchived: showArchived, myData: myData);

        DataRow dr; // = new DataRow();
        int rowIndex = 0;
        string subOnHold = "";
        string subNew = "";
        string subInProgress = "";
        string subReOpened = "";
        string subInfoProvided = "";
        string subUnRepo = "";
        string subCheckedIn = "";
        string subDeployed = "";
        string subClosed = "";

        while (rowIndex < dtSub.Rows.Count)
        {
            dr = dtSub.Rows[rowIndex];
            if (dr["Priority"].ToString().Length > 9)
            {
                if (dr["Priority"].ToString().Substring(0, 10) == "Task TOTAL")
                {
                    subOnHold = dr["On Hold"].ToString();
                    subNew = dr["New"].ToString();
                    subInProgress = dr["In Progress"].ToString();
                    subReOpened = dr["Re-Opened"].ToString();
                    subInfoProvided = dr["Info Provided"].ToString();
                    subUnRepo = dr["Un-Reproducible"].ToString();
                    subCheckedIn = dr["Checked In"].ToString();
                    subDeployed = dr["Deployed"].ToString();
                    subClosed = dr["Closed"].ToString();
                }
            }
            rowIndex++;
        }

        DataTable dt = Workload.Workload_Metrics_Get(SelectedStatus: SelectedStatus, SelectedUsers: SelectedUsers, metricType: "QMWorkloadCrosswalk", showArchived: showArchived, myData: myData);

        // Turn off read-only and replace off counts with correct counts

        DataColumn dcOnHold = dt.Columns["On Hold"];
        dcOnHold.ReadOnly = false;
        DataColumn dcNew = dt.Columns["New"];
        dcNew.ReadOnly = false;
        DataColumn dcInProgress = dt.Columns["In Progress"];
        dcInProgress.ReadOnly = false;
        DataColumn dcReOpened = dt.Columns["Re-Opened"];
        dcReOpened.ReadOnly = false;
        DataColumn dcInfoProvided = dt.Columns["Info Provided"];
        dcInfoProvided.ReadOnly = false;
        DataColumn dcUnRepo = dt.Columns["Un-Reproducible"];
        dcUnRepo.ReadOnly = false;
        DataColumn dcCheckedIn = dt.Columns["Checked In"];
        dcCheckedIn.ReadOnly = false;
        DataColumn dcDeployed = dt.Columns["Deployed"];
        dcDeployed.ReadOnly = false;
        DataColumn dcClosed = dt.Columns["Closed"];
        dcClosed.ReadOnly = false;

        dt.Rows[6].SetField("On Hold", subOnHold);
        dt.Rows[6].SetField("New", subNew);
        dt.Rows[6].SetField("In Progress", subInProgress);
        dt.Rows[6].SetField("Re-Opened", subReOpened);
        dt.Rows[6].SetField("Info Provided", subInfoProvided);
        dt.Rows[6].SetField("Un-Reproducible", subUnRepo);
        dt.Rows[6].SetField("Checked In", subCheckedIn);
        dt.Rows[6].SetField("Deployed", subDeployed);
        dt.Rows[6].SetField("Closed", subClosed);

        dt.AcceptChanges();

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod(true)]
    public static bool ItemExists(int itemID, string type)
    {
        try
        {
            return Workload.ItemExists(itemID, -1, type);
        }
        catch (Exception) { return false; }
    }

    [WebMethod(true)]
    public static string GetSortValuesFromDB(int pageID, string pageName)
    {
        try
        {
            return Workload.GetSortValuesFromDB(pageID, pageName);
        }
        catch (Exception) { return ""; }
    }

}