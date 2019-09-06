using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


public partial class Workload_CrosswalkGrid_Items : System.Web.UI.Page
{
	protected bool CanView = false;
	protected bool CanEdit = false;
	protected bool CanViewWorkItem = false;
	protected bool CanEditWorkItem = false;

	protected bool _myData = true;
	protected int _showArchived = 0;

	protected bool _refreshData = false;
	protected bool _export = false;
	protected bool ShowClosed = false;
	protected string[] SelectedStatuses;
    protected string[] SelectedAssigned;
    

        protected DataTable _dtUser = null;
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected string Filters = string.Empty;

	protected string ParentColumns = string.Empty;
	protected string RollupGroup = string.Empty;
	protected string ChildColumns = string.Empty;
	protected string RankSortType = "Tech";

	protected string SortableColumns = string.Empty;
	protected string SortOrder = string.Empty;
	protected string DefaultColumnOrder = string.Empty;
	protected string SelectedColumnOrder = string.Empty;
	protected string ColumnOrder = string.Empty;
	protected bool ColumnOrderChanged = false;

	protected bool TaskColumn = true;
	protected DataTable DTLookup;
	protected int ItemIDIndex = 1;
	protected bool ChildList = false;
	protected DataTable dtSub;
    protected bool ShowBacklog = false;

    protected bool useColumnOrdering = false;
    protected string parentAffilitatedID = string.Empty;
    protected string assignedIDs = string.Empty;
    protected string statusIds = string.Empty;
    protected bool AffiliatedFlag = false;

    protected string _Crosswalk_GridView;

    int intUserID = 0;

    protected void Page_Load(object sender, EventArgs e)
	{
		this.CanEdit = this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
		this.CanView = this.CanViewWorkItem = CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

		readQueryString();

		init();

	    loadGridData();

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
            else
                _refreshData = false;
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

		if (Request.QueryString["statuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["statuses"].ToString()))
		{
			this.SelectedStatuses = Server.UrlDecode(Request.QueryString["statuses"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}
        //if (Request.QueryString["workTypes"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["workTypes"].ToString()))
        //{
        //	this.SelectedWorkTypes = Server.UrlDecode(Request.QueryString["workTypes"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //}

        if (Request.QueryString["Assigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Assigned"].ToString()))
        {
            this.SelectedAssigned = Server.UrlDecode(Request.QueryString["Assigned"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["ShowArchived"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"].ToString()), out this._showArchived);
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
				this.RollupGroup = "Priority";
			}
			Session["Crosswalk_RollupGroup"] = this.RollupGroup;
		}

		if (Session["Crosswalk_DefaultSortType"] != null && !string.IsNullOrWhiteSpace(Session["Crosswalk_DefaultSortType"].ToString()))
		{
			this.RankSortType = Session["Crosswalk_DefaultSortType"].ToString();
		}
		else
		{
			if (Request.QueryString["rankSortType"] != null
				&& !string.IsNullOrWhiteSpace(Request.QueryString["rankSortType"].ToString()))
			{
				this.RankSortType = Server.UrlDecode(Request.QueryString["rankSortType"]).Replace("?", "");
			}
			else
			{
				this.RankSortType = "Tech";
			}
			Session["Crosswalk_DefaultSortType"] = this.RankSortType;
		}

		if (Request.QueryString["sortOrder"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
		{
			this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
		}

        if (Session["Crosswalk_SelectedParentColumns"] != null && !string.IsNullOrWhiteSpace(Session["Crosswalk_SelectedParentColumns"].ToString()))
        {
            this.ParentColumns = Session["Crosswalk_SelectedParentColumns"].ToString();
        }
        else
        {
            if (Request.QueryString["parentColumns"] != null
                && !string.IsNullOrWhiteSpace(Request.QueryString["parentColumns"].ToString()))
            {
                this.ParentColumns = Server.UrlDecode(Request.QueryString["parentColumns"]).Replace("?", "");
            }
            else
            {
                this.ParentColumns = "AllocationAssignment";
            }
            Session["Crosswalk_SelectedParentColumns"] = this.ParentColumns;
        }

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

		if (HttpContext.Current.Session["Crosswalk_SelectedChildColumnOrder"] != null
			&& !string.IsNullOrWhiteSpace(HttpContext.Current.Session["Crosswalk_SelectedChildColumnOrder"].ToString()))
		{
			this.ColumnOrder = HttpContext.Current.Session["Crosswalk_SelectedChildColumnOrder"].ToString();
		}
		else
		{
			if (Request.QueryString["columnOrder"] != null
				&& !string.IsNullOrWhiteSpace(Request.QueryString["columnOrder"].ToString()))
			{
				this.ColumnOrder = Server.UrlDecode(Request.QueryString["columnOrder"]);
				HttpContext.Current.Session["Crosswalk_SelectedChildColumnOrder"] = this.ColumnOrder;
			}
		}

		if (string.IsNullOrWhiteSpace(this.ColumnOrder))
		{
            getDefaultColumnOrder();
        }

        if (Request.QueryString["columnOrderChanged"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["columnOrderChanged"].ToString()))
		{
			bool.TryParse(Server.UrlDecode(Request.QueryString["columnOrderChanged"]), out this.ColumnOrderChanged);
		}

		if (Request.QueryString["rowFilters"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["rowFilters"].ToString()))
		{
			this.Filters = Server.UrlDecode(Request.QueryString["rowFilters"]);
		}

		if (Request.QueryString["childList"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["childList"].ToString()))
		{
			bool.TryParse(Server.UrlDecode(Request.QueryString["childList"]), out this.ChildList);
		}

    }

    private void init()
	{
		grdWorkload.GridHeaderRowDataBound += grdWorkload_GridHeaderRowDataBound;
		grdWorkload.GridRowDataBound += grdWorkload_GridRowDataBound;
		grdWorkload.GridPageIndexChanging += grdWorkload_GridPageIndexChanging;
	}

    private void getDefaultColumnOrder()
    {
        // Moved to one place - one is enough.  
        // This one from loadGridData area
        this.ColumnOrder = "X|X|true|false||False~WORKREQUESTID|WORKREQUESTID|false|false||False~WORKREQUEST|WORKREQUEST|false|false||False~PhaseID|PhaseID|false|false||False~ItemID|Task Number|true|true||True~Phase|Phase|true|true||True~WORKITEMTYPEID|WORKITEMTYPEID|false|false||False~WORKITEMTYPE|Item Type|false|true||True~WorkTypeID|WorkTypeID|false|false||False~WorkType|Work Type|false|true||True~Task_Count|Task Count|false|false||False~WTS_SYSTEMID|WTS_SYSTEMID|false|false||False~Websystem|System|true|true||True~STATUSID|STATUSID|false|false||False~STATUS|Status|true|true||True~IVTRequired|Requires IVT|false|true||True~NEEDDATE|Date Needed|false|true||True~TITLE|Title|true|true||True~DESCRIPTION|DESCRIPTION|false|false||False~AllocationGroupID|AllocationGroupID|false|false||False~AllocationGroup|Allocation Group|false|true||True~AllocationCategoryID|AllocationCategoryID|false|false||False~AllocationCategory|Allocation Category|false|true||True~ALLOCATIONID|ALLOCATIONID|false|false||False~ALLOCATION|Allocation Assignment|false|true||True~RESOURCEPRIORITYRANKID|RESOURCEPRIORITYRANKID|false|false||False~RESOURCEPRIORITYRANK|Primary Tech. Rank|false|true|Resources||False~PrimaryBusinessRank|Primary Bus. Rank|false|true|Resources|True~WorkAreaID|WorkAreaID|false|false||False~WorkArea|Work Area|true|true||True~WorkloadGroupID|WorkloadGroupID|false|false||False~WorkloadGroup|Functionality|true|true||True~Production|Production|false|false||False~ProductionStatusID|ProductionStatusID|false|false||False~ProductionStatus|Production Status|true|true||True~ProductVersionID|ProductVersionID|false|false||False~Version|Version|true|true||True~SR_Number|SR Number|false|true||True~PRIORITYID|PRIORITYID|false|false||False~PRIORITY|Priority|true|true||True~ASSIGNEDRESOURCEID|ASSIGNEDRESOURCEID|false|false||False~Assigned|Assigned To|true|true|Resources|True~SMEID|SMEID|false|false||False~Primary_Analyst|Primary_Analyst|false|false||False~PRIMARYRESOURCEID|PRIMARYRESOURCEID|false|false||False~Primary_Developer|Primary Developer|true|true|Resources|True~PrimaryBusinessResourceID|PrimaryBusinessResourceID|false|false||False~PrimaryBusinessResource|Primary Bus. Resource|false|true|Resources|True~SECONDARYRESOURCEID|SECONDARYRESOURCEID|false|false||False~SECONDARYRESOURCE|SECONDARYRESOURCE|false|false||False~CREATEDBY|CREATEDBY|false|false||False~CREATEDDATE|CREATEDDATE|false|false||False~SubmittedByID|SubmittedByID|false|false||False~SubmittedBy|Submitted By|false|true||True~Progress|Progress|true|true||True~ARCHIVE|ARCHIVE|false|false||False~Status_Sort|Status_Sort|false|false||False~ReOpenedCount|Times Re-Opened|false|true||True~StatusUpdatedDate|Status Updated Date|false|true||True~Y|Y|true|false||False";

        // This one has all double pipes removed.  All show similar results - column headers not same as data, showing ID's, etc...
        //this.ColumnOrder = "X|X|true|false|False~WORKREQUESTID|WORKREQUESTID|false|false|False~WORKREQUEST|WORKREQUEST|false|false|False~PhaseID|PhaseID|false|false|False~ItemID|Task Number|true|true|True~Phase|Phase|true|true|True~WORKITEMTYPEID|WORKITEMTYPEID|false|false|False~WORKITEMTYPE|Item Type|false|true|True~WorkTypeID|WorkTypeID|false|false|False~WorkType|Work Type|false|true|True~Task_Count|Task Count|false|false|False~WTS_SYSTEMID|WTS_SYSTEMID|false|false|False~Websystem|System|true|true|True~STATUSID|STATUSID|false|false|False~STATUS|Status|true|true||True~IVTRequired|Requires IVT|false|true|True~NEEDDATE|Date Needed|false|true|True~TITLE|Title|true|true|True~DESCRIPTION|DESCRIPTION|false|false|False~AllocationGroupID|AllocationGroupID|false|false|False~AllocationGroup|Allocation Group|false|true|True~AllocationCategoryID|AllocationCategoryID|false|false|False~AllocationCategory|Allocation Category|false|true|True~ALLOCATIONID|ALLOCATIONID|false|false||False~ALLOCATION|Allocation Assignment|false|true|True~RESOURCEPRIORITYRANKID|RESOURCEPRIORITYRANKID|false|false|False~RESOURCEPRIORITYRANK|Primary Tech. Rank|false|true|Resources|True~PrimaryBusinessRankID|PrimaryBusinessRankID|false|false|False~PrimaryBusinessRank|Primary Bus. Rank|false|true|Resources|True~WorkAreaID|WorkAreaID|false|false|False~WorkArea|Work Area|true|true|True~WorkloadGroupID|WorkloadGroupID|false|false|False~WorkloadGroup|Functionality|true|true|True~Production|Production|false|false|False~ProductionStatusID|ProductionStatusID|false|false|False~ProductionStatus|Production Status|true|true|True~ProductVersionID|ProductVersionID|false|false|False~Version|Version|true|true|True~SR_Number|SR Number|false|true|True~PRIORITYID|PRIORITYID|false|false|False~PRIORITY|Priority|true|true|True~ASSIGNEDRESOURCEID|ASSIGNEDRESOURCEID|false|false|False~Assigned|Assigned To|true|true|Resources|True~SMEID|SMEID|false|false|False~Primary_Analyst|Primary_Analyst|false|false|False~PRIMARYRESOURCEID|PRIMARYRESOURCEID|false|false|False~Primary_Developer|Primary Developer|true|true|Resources|True~PrimaryBusinessResourceID|PrimaryBusinessResourceID|false|false|False~PrimaryBusinessResource|Primary Bus. Resource|false|true|Resources|True~SECONDARYRESOURCEID|SECONDARYRESOURCEID|false|false|False~SECONDARYRESOURCE|SECONDARYRESOURCE|false|false|False~CREATEDBY|CREATEDBY|false|false|False~CREATEDDATE|CREATEDDATE|false|false|False~SubmittedByID|SubmittedByID|false|false|False~SubmittedBy|Submitted By|false|true|True~Progress|Progress|true|true|True~ARCHIVE|ARCHIVE|false|false|False~Status_Sort|Status_Sort|false|false|False~ReOpenedCount|Times Re-Opened|false|true|True~StatusUpdatedDate|Status Updated Date|false|true|True~Y|Y|true|false";

        // Original line (readQueryString area):    
        //this.ColumnOrder = "X|X|true|false||False~WORKREQUESTID|WORKREQUESTID|false|false||False~WORKREQUEST|WORKREQUEST|false|false||False~PhaseID|PhaseID|false|false||False~ItemID|Task Number|true|true||True~Phase|Phase|true|true||True~WORKITEMTYPEID|WORKITEMTYPEID|false|false||False~WORKITEMTYPE|Item Type|false|true||True~WorkTypeID|WorkTypeID|false|false||False~WorkType|Work Type|false|true||True~Task_Count|Task Count|false|false||False~WTS_SYSTEMID|WTS_SYSTEMID|false|false||False~Websystem|System|true|true||True~STATUSID|STATUSID|false|false||False~STATUS|Status|true|true||True~IVTRequired|Requires IVT|false|true||True~NEEDDATE|Date Needed|false|true||True~TITLE|Title|true|true||True~DESCRIPTION|DESCRIPTION|false|false||False~AllocationGroupID|AllocationGroupID|false|false||False~AllocationGroup|Allocation Group|false|true||True~AllocationCategoryID|AllocationCategoryID|false|false||False~AllocationCategory|Allocation Category|false|true||True~ALLOCATIONID|ALLOCATIONID|false|false||False~ALLOCATION|Allocation Assignment|false|true||True~RESOURCEPRIORITYRANKID|RESOURCEPRIORITYRANKID|false|false||False~RESOURCEPRIORITYRANK|Primary Tech. Rank|false|true|Resources|True~PrimaryBusinessRankID|PrimaryBusinessRankID|false|false||False~PrimaryBusinessRank|Primary Bus. Rank|false|true|Resources|True~WorkAreaID|WorkAreaID|false|false|||False~WorkArea|Work Area|true|true||True~WorkloadGroupID|WorkloadGroupID|false|false||False~WorkloadGroup|Functionality|true|true||True~Production|Production|false|false||False~ProductionStatusID|ProductionStatusID|false|false||False~ProductionStatus|Production Status|true|true||True~ProductVersionID|ProductVersionID|false|false||False~Version|Version|true|true||True~SR_Number|SR Number|false|true||True~PRIORITYID|PRIORITYID|false|false||False~PRIORITY|Priority|true|true||True~ASSIGNEDRESOURCEID|ASSIGNEDRESOURCEID|false|false||False~Assigned|Assigned To|true|true|Resources|True~SMEID|SMEID|false|false||False~Primary_Analyst|Primary_Analyst|false|false||False~PRIMARYRESOURCEID|PRIMARYRESOURCEID|false|false||False~Primary_Developer|Primary Developer|true|true|Resources|True~PrimaryBusinessResourceID|PrimaryBusinessResourceID|false|false||False~PrimaryBusinessResource|Primary Bus. Resource|false|true|Resources|True~SECONDARYRESOURCEID|SECONDARYRESOURCEID|false|false||False~SECONDARYRESOURCE|SECONDARYRESOURCE|false|false||False~CREATEDBY|CREATEDBY|false|false||False~CREATEDDATE|CREATEDDATE|false|false||False~SubmittedByID|SubmittedByID|false|false||False~SubmittedBy|Submitted By|false|true||True~Progress|Progress|true|true||True~ARCHIVE|ARCHIVE|false|false||False~Status_Sort|Status_Sort|false|false||False~ReOpenedCount|Times Re-Opened|false|true||True~StatusUpdatedDate|Status Updated Date|false|true||True~Y|Y|true|false||False";
        // Original line (loadGridData area)
        //this.ColumnOrder = "X|X|true|false||False~WORKREQUESTID|WORKREQUESTID|false|false||False~WORKREQUEST|WORKREQUEST|false|false||False~PhaseID|PhaseID|false|false||False~ItemID|Task Number|true|true||True~Phase|Phase|true|true||True~WORKITEMTYPEID|WORKITEMTYPEID|false|false||False~WORKITEMTYPE|Item Type|false|true||True~WorkTypeID|WorkTypeID|false|false||False~WorkType|Work Type|false|true||True~Task_Count|Task Count|false|false||False~WTS_SYSTEMID|WTS_SYSTEMID|false|false||False~Websystem|System|true|true||True~STATUSID|STATUSID|false|false||False~STATUS|Status|true|true||True~IVTRequired|Requires IVT|false|true||True~NEEDDATE|Date Needed|false|true||True~TITLE|Title|true|true||True~DESCRIPTION|DESCRIPTION|false|false||False~AllocationGroupID|AllocationGroupID|false|false||False~AllocationGroup|Allocation Group|false|true||True~AllocationCategoryID|AllocationCategoryID|false|false||False~AllocationCategory|Allocation Category|false|true||True~ALLOCATIONID|ALLOCATIONID|false|false||False~ALLOCATION|Allocation Assignment|false|true||True~RESOURCEPRIORITYRANKID|RESOURCEPRIORITYRANKID|false|false||False~RESOURCEPRIORITYRANK|Primary Tech. Rank|false|true|Resources|True~PrimaryBusinessRankID|PrimaryBusinessRankID|false|false||False~PrimaryBusinessRank|Primary Bus. Rank|false|true|Resources|True~WorkAreaID|WorkAreaID|false|false||False~WorkArea|Work Area|true|true||True~WorkloadGroupID|WorkloadGroupID|false|false||False~WorkloadGroup|Functionality|true|true||True~Production|Production|false|false||False~ProductionStatusID|ProductionStatusID|false|false||False~ProductionStatus|Production Status|true|true||True~ProductVersionID|ProductVersionID|false|false||False~Version|Version|true|true||True~SR_Number|SR Number|false|true||True~PRIORITYID|PRIORITYID|false|false||False~PRIORITY|Priority|true|true||True~ASSIGNEDRESOURCEID|ASSIGNEDRESOURCEID|false|false||False~Assigned|Assigned To|true|true|Resources|True~SMEID|SMEID|false|false||False~Primary_Analyst|Primary_Analyst|false|false||False~PRIMARYRESOURCEID|PRIMARYRESOURCEID|false|false||False~Primary_Developer|Primary Developer|true|true|Resources|True~PrimaryBusinessResourceID|PrimaryBusinessResourceID|false|false||False~PrimaryBusinessResource|Primary Bus. Resource|false|true|Resources|True~SECONDARYRESOURCEID|SECONDARYRESOURCEID|false|false||False~SECONDARYRESOURCE|SECONDARYRESOURCE|false|false||False~CREATEDBY|CREATEDBY|false|false||False~CREATEDDATE|CREATEDDATE|false|false||False~SubmittedByID|SubmittedByID|false|false||False~SubmittedBy|Submitted By|false|true||True~Progress|Progress|true|true||True~ARCHIVE|ARCHIVE|false|false||False~Status_Sort|Status_Sort|false|false||False~ReOpenedCount|Times Re-Opened|false|true||True~StatusUpdatedDate|Status Updated Date|false|true||True~Y|Y|true|false||False";
    }

    private void loadGridData()
	{
		DataTable dtWork = null;

        try
        {
            DataSet dsOptions = WorkloadItem.GetAvailableOptions();
			if (dsOptions.Tables.Contains("User"))
			{
				_dtUser = dsOptions.Tables["User"];
				Page.ClientScript.RegisterArrayDeclaration("_userList", JsonConvert.SerializeObject(_dtUser, Newtonsoft.Json.Formatting.None));
			}
			if (dsOptions.Tables.Contains("PriorityRank"))
			{
				_dtUser = dsOptions.Tables["PriorityRank"];
				Page.ClientScript.RegisterArrayDeclaration("_priorityList", JsonConvert.SerializeObject(_dtUser, Newtonsoft.Json.Formatting.None));
			}

            if (Session["ShowArchived"] == null)
                HttpContext.Current.Session["ShowArchived"] = _showArchived;

            if (Session["MyData"] == null)
                HttpContext.Current.Session["MyData"] = _myData;

            if (Session["RankSortType"] == null)
                HttpContext.Current.Session["RankSortType"] = this.RankSortType;

            if (Session["ShowClosed"] == null)
                HttpContext.Current.Session["ShowClosed"] = ShowClosed;

            if (Session["ShowBacklog"] == null)
                HttpContext.Current.Session["ShowBacklog"] = ShowBacklog;

            // Should add SelectedStatuses & SelectedAssigned, but would have to make optional and/or add in serveral places
            if (SelectedAssigned != null && SelectedAssigned.Length > 0)
            {
                assignedIDs = String.Join(",", SelectedAssigned);
            }
            //Statuses
            if (SelectedStatuses != null && SelectedStatuses.Length > 0)
            {
                statusIds = String.Join(",", SelectedStatuses);
            }

            string[] filterArray = this.Filters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            AffiliatedFlag = true;
            //for (int i = 0; i < filterArray.Length; i++)
            //{
            //    if (filterArray[i].Contains("Affiliated"))
            //    {
            //        AffiliatedFlag = true;
            //    }
            //}

            // 2-21-2017 - Adding back some of the filtering, BUT do it BEFORE the stored procedure:
            string nFilter = string.Empty;
            for (int i = 0; i < filterArray.Length; i++)
            {
                if (filterArray[i].Contains("Affiliated"))
                {
                    string strUserID = filterArray[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    int.TryParse(strUserID, out intUserID);
                    assignedIDs = intUserID.ToString();
                    parentAffilitatedID = intUserID.ToString();
                }
                else if (filterArray[i].Contains("Workload Assigned To"))
                {
                    string strUserID = filterArray[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    int.TryParse(strUserID, out intUserID);
                    assignedIDs = intUserID.ToString();
                }
            }

            //============================================================================
            // First time in Page_Load OR Data request type changed? 
            //============================================================================

            if (Session["dtCrosswalkWorkItem"] == null
            || _showArchived.ToString() != Session["ShowArchived"].ToString()
            || _myData.ToString() != Session["MyData"].ToString()
            || this.RankSortType.ToString() != Session["RankSortType"].ToString()
            || ShowClosed.ToString() != Session["ShowClosed"].ToString()
            || ShowBacklog.ToString() != Session["ShowBacklog"].ToString()
            || statusIds.ToString() != Session["StatusIDs"].ToString()
            || assignedIDs.ToString() != Session["AssignedIDs"].ToString()
            || parentAffilitatedID.ToString() != Session["parentAffilitatedID"].ToString()
            || HttpContext.Current.Session["GridView"].ToString() != Session["Crosswalk_GridView"].ToString()
            || _refreshData)  
            {
                // NEW 1-19-2017: "WORKITEMLIST_AFFILIATED_GET" OR "WORKITEMLIST_ASSIGNED_GET" depending on AffiliatedFlag
                dtWork = WorkloadItem.WorkItemList_Get_QF(workRequestID: 0
                , showArchived: _showArchived
                , columnListOnly: 0
                , myData: _myData
                , rankSortType: this.RankSortType
                , showClosed: (ShowClosed ? 1 : 0)
                , ShowBacklog: ShowBacklog
                , SelectedStatuses: String.Join(",", statusIds)
                , SelectedAssigned: String.Join(",", assignedIDs)
                , Affiliated: AffiliatedFlag
                , ParentAffilitatedID : parentAffilitatedID);

                HttpContext.Current.Session["dtCrosswalkWorkItem"] = dtWork;
                HttpContext.Current.Session["ShowArchived"] = _showArchived;
                HttpContext.Current.Session["MyData"] = _myData;
                HttpContext.Current.Session["RankSortType"] = this.RankSortType;
                HttpContext.Current.Session["ShowClosed"] = ShowClosed;
                HttpContext.Current.Session["ShowBacklog"] = ShowBacklog;
                HttpContext.Current.Session["StatusIDs"] = statusIds;
                HttpContext.Current.Session["AssignedIDs"] = assignedIDs;
                HttpContext.Current.Session["GridView"] = Session["Crosswalk_GridView"].ToString();
                HttpContext.Current.Session["parentAffilitatedID"] = parentAffilitatedID;
            }
            else
            {
                dtWork = (DataTable)HttpContext.Current.Session["dtCrosswalkWorkItem"];
            }
        }
        catch (Exception ex)
		{
			LogUtility.LogException(ex);
			dtWork = null;
		}

		if (dtWork != null)
		{
            InitializeColumnData(ref dtWork);
            dtWork.AcceptChanges();

            if (ChildList)
			{
				this.ChildColumns = "X,TaskNumber,Phase,System,Status,Title,AssignedTo,PrimaryDeveloper,WorkArea,Functionality,ProductionStatus,Version,Priority,Progress,Y";
                getDefaultColumnOrder();
            }

            if (!this.ChildColumns.Contains("TaskNumber") && !ChildList)
			{
				TaskColumn = false;
				this.DTLookup = dtWork.Copy();
				RemoveColumns(ref dtWork);
				dtWork = dtWork.DefaultView.ToTable(true);
				columnData = new GridCols();
				InitializeColumnData(ref dtWork);
				dtWork.AcceptChanges();
			}

            this.DCC = dtWork.Columns;
			if (TaskColumn) ItemIDIndex = this.DCC["ItemID"].Ordinal;

            using (DataTable dtTemp = dtWork.Clone())
			{
				this.DCC = dtTemp.Columns;
				string json = JsonConvert.SerializeObject(DCC, Formatting.None);
				Page.ClientScript.RegisterArrayDeclaration("_dcc", json);
			}
		}


        grdWorkload.DataSource = dtWork;
		grdWorkload.DataBind();
    }

    protected void InitializeColumnData(ref DataTable dt)
	{
		try
		{
			string displayName = string.Empty, groupName = string.Empty;
			bool blnVisible = false, isViewable = false, blnSortable = false, blnOrderable = false;
			bool forceChecked = false;

			foreach (DataColumn gridColumn in dt.Columns)
			{
				displayName = gridColumn.ColumnName;
				blnVisible = false;
				blnSortable = false;
				blnOrderable = false;
				isViewable = false;
				forceChecked = false;
				groupName = string.Empty;

				switch (gridColumn.ColumnName)
				{
					case "X":
						displayName = "X";
						blnVisible = true;
						forceChecked = true;
						break;
                    case "PDDTDR_Phase":
                        displayName = "Phase";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "ItemID":
						displayName = "Task Number";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
                    case "WORKITEMTYPE":
                        displayName = "Item Type";
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WorkType":
                        displayName = "Work Type";
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "Task_Count":
                        displayName = "Task Count";
                        blnSortable = true;
                        break;
                    case "Websystem":
                        displayName = "System";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "STATUS":
						displayName = "Status";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
                    case "IVTRequired":
                        displayName = "Requires IVT";
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "NEEDDATE":
                        displayName = "Date Needed";
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "TITLE":
						displayName = "Title";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						//forceChecked = true;
						break;
					case "AllocationGroup":
						displayName = "Allocation Group";
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "AllocationCategory":
						displayName = "Allocation Category";
						blnSortable = false;
						blnOrderable = false;
						isViewable = false;
						break;
					case "ALLOCATION":
						displayName = "Allocation Assignment";
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "RESOURCEPRIORITYRANK":
						displayName = "Primary Tech. Rank";
                        blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "PrimaryBusinessRank":
						displayName = "Primary Bus. Rank";
                        blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
                    case "SecondaryBusinessRank":
                        displayName = "Secondary Bus. Rank";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "SecondaryResourceRank":
                        displayName = "Secondary Tech. Rank";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WorkArea":
						displayName = "Work Area";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "WorkloadGroup":
						displayName = "Functionality";
						blnVisible = true;
						blnSortable = false;
						blnOrderable = false;
						isViewable = true;
						break;
					case "ProductionStatus":
						displayName = "Production Status";
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "Version":
						displayName = "Version";
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "SR_Number":
						displayName = "SR Number";
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "PRIORITY":
						displayName = "Priority";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "Assigned":
						displayName = "Assigned To";
						groupName = "Resources";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "Primary_Developer":
						displayName = "Primary Developer";
						groupName = "Resources";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;

                    case "SecondaryResource":
                        displayName = "Secondary Developer";
                        groupName = "Resources";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "SecondaryResourceID":
                        displayName = "Secondary Developer ID";
                        groupName = "Resources";
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = false;
                        isViewable = false;
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
                    case "SubmittedBy":
						displayName = "Submitted By";
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "Progress":
						displayName = "Progress";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;

                    case "PDDTDR_PHASEID":
                        displayName = "PD Phase ID";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;

                    case "ReOpenedCount":
						displayName = "Times Re-Opened";
						blnVisible = true;
						blnSortable = false;
						blnOrderable = false;
						isViewable = true;
						break;
					case "StatusUpdatedDate":
						displayName = "Status Updated Date";
						blnVisible = true;
						blnSortable = false;
						blnOrderable = false;
						isViewable = true;
						break;
					case "Y":
						displayName = "Y";
						blnVisible = false;
						forceChecked = false;
                        isViewable = false;
                        break;
                    case "Assigned/Affiliated":
                        displayName = "Assigned/Affiliated";
                        blnVisible = true;
                        blnOrderable = true;
                        blnSortable = true;
                        break;



                    case "ProductionStatusID":
                        displayName = "ProductionStatusID";
                        blnVisible = false;
                        forceChecked = false;
                        isViewable = false;
                        break;
                    case "SecondaryDeveloperID":
                        displayName = "SecondaryDeveloperID";
                        blnVisible = false;
                        forceChecked = false;
                        isViewable = false;
                        break;


                    case "ProductVersionID":
                        displayName = "ProductVersionID";
                        blnVisible = false;
                        forceChecked = false;
                        isViewable = false;
                        break;

                    case "WorkAreaID":
                        displayName = "ProductVersionID";
                        blnVisible = false;
                        forceChecked = false;
                        isViewable = false;
                        break;

                }
                blnVisible = (forceChecked || (ChildColumns.Replace(" ", "").IndexOf(displayName.Replace(" ", "")) > -1));

				columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
				columnData.Columns.Item(columnData.Columns.Count - 1).Viewable = isViewable;
			}

            //Initialize the columnData
            columnData.Initialize(ref dt, ";", "~", "|");

            // Get sortable columns and default column order.

            // DO NOT REMOVE! This will populate the Sort Order dropdowns
            SortableColumns = this.columnData.SortableColumnsToString();
            DefaultColumnOrder = this.columnData.DefaultColumnOrderToString();



            //useColumnOrdering = WTSData.WTS_GetColumnOrderingPref();
            //if (useColumnOrdering)
            //{
            // NOTE: ReorderDataTable, at times, messed up the sub grid - badly!  Needs re-write.
            if (TaskColumn)
            { 
                // SCB Trying to clean up string - this didn't help
                //ColumnOrder = ColumnOrder.Replace("||", "|");
                this.columnData.ReorderDataTable(ref dt, ColumnOrder);
            }


            // Y & Z were not in the last 2 columns after calling ReorderDataTable - but this didn't help either...
            //dt.Columns["Y"].SetOrdinal(dt.Columns.Count - 2);
            //dt.Columns["Z"].SetOrdinal(dt.Columns.Count - 1);
            //dt.Columns["Y"].SetOrdinal(dt.Columns.Count - 2);
            //dt.Columns["Z"].SetOrdinal(dt.Columns.Count - 1);
            //dt.AcceptChanges();


            SelectedColumnOrder = this.columnData.CurrentColumnOrderToString();
            columnData.SortDataTable(ref dt, SortOrder);

            // Sort now saves to DB/Persists.
            SortOrder = Workload.GetSortValuesFromDB(1, "Workload_CrosswalkGrid_Items.ASPX");
        }
        catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}

	void grdWorkload_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		columnData.SetupGridHeader(row);
		formatColumnDisplay(ref row);

		row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
		if (TaskColumn) row.Cells[DCC.IndexOf("ItemID")].Text = "Task #";

		if (DCC.Contains("Y"))
		{
			row.Cells[DCC.IndexOf("Y")].Text = "&nbsp;";

			if (!CanEdit || !TaskColumn)
			{
				Image imgBlank = new Image();
				imgBlank.Height = 12;
				imgBlank.Width = 12;
				imgBlank.ImageUrl = "Images/Icons/blank.png";
				imgBlank.AlternateText = "";
				row.Cells[DCC["Y"].Ordinal].Controls.Add(imgBlank);
			}
			else
			{
				Image imgSave = new Image();
				imgSave.ID = "imgSave";
				imgSave.Height = 12;
				imgSave.Width = 12;
				imgSave.ImageUrl = "Images/Icons/disk.png";
				imgSave.AlternateText = "Save Task Updates";
				imgSave.Style["cursor"] = "pointer";
				imgSave.Attributes.Add("onclick", "buttonSave_click();");
				row.Cells[DCC["Y"].Ordinal].Controls.Add(imgSave);
			}
		}


		Image imgRefresh = new Image();
		imgRefresh.ID = "imgRefresh";
		imgRefresh.Height = 12;
		imgRefresh.Width = 12;
		imgRefresh.ImageUrl = "Images/Icons/arrow_refresh_blue.png";
		imgRefresh.AlternateText = "Reload Tasks";
		imgRefresh.Style["cursor"] = "pointer";
		imgRefresh.Attributes.Add("Onclick", "refreshPage();");
		row.Cells[DCC["X"].Ordinal].Controls.Add(imgRefresh);
		row.Cells[DCC["X"].Ordinal].Style["text-align"] = "left";

		row.Cells[DCC["X"].Ordinal].Controls.Clear();
		row.Cells[DCC["X"].Ordinal].Controls.Add(imgRefresh);
	}

	void grdWorkload_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		columnData.SetupGridBody(row);
		formatColumnDisplay(ref row);

		int taskCount = 0;

		if (!TaskColumn)
		{
			string[] columns = this.ChildColumns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder sb = new StringBuilder();
			string columnValue = string.Empty;
			for (int i = 0; i < columns.Length; i++)
			{
				switch (columns[i].Trim().ToUpper())
				{
					case "ITEMTYPE":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("WORKITEMTYPE") == row.Cells[DCC["WORKITEMTYPE"].Ordinal].Text
									   select r.Field<int>("WORKITEMTYPEID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Item Type={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "WORKTYPE":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("WorkType") == row.Cells[DCC["WorkType"].Ordinal].Text
									   select r.Field<int>("WorkTypeID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Work Type={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "SYSTEM":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("Websystem") == row.Cells[DCC["Websystem"].Ordinal].Text
									   select r.Field<int>("WTS_SYSTEMID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}System={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "STATUS":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("STATUS") == row.Cells[DCC["STATUS"].Ordinal].Text
									   select r.Field<int>("STATUSID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Workload Status={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "ALLOCATIONGROUP":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("AllocationGroup") == row.Cells[DCC["AllocationGroup"].Ordinal].Text
									   select r.Field<int>("AllocationGroupID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Allocation Group={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "ALLOCATIONCATEGORY":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("AllocationCategory") == row.Cells[DCC["AllocationCategory"].Ordinal].Text
									   select r.Field<int>("AllocationCategoryID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Allocation Category={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "ALLOCATIONASSIGNMENT":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("ALLOCATION") == row.Cells[DCC["ALLOCATION"].Ordinal].Text
									   select r.Field<int>("AllocationID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Allocation Assignment={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "WORKAREA":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("WorkArea") == row.Cells[DCC["WorkArea"].Ordinal].Text
									   select r.Field<int>("WorkAreaID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Work Area={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "WORKLOADGROUP":
					case "FUNCTIONALITY":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("WorkloadGroup") == row.Cells[DCC["WorkloadGroup"].Ordinal].Text
									   select r.Field<int>("WorkloadGroupID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Workload Group={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "VERSION":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("Version") == row.Cells[DCC["Version"].Ordinal].Text
									   select r.Field<int>("ProductVersionID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Release Version={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "PRIORITY":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("PRIORITY") == row.Cells[DCC["PRIORITY"].Ordinal].Text
									   select r.Field<int>("PriorityID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Workload Priority={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "ASSIGNEDTO":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("Assigned") == row.Cells[DCC["Assigned"].Ordinal].Text
									   select r.Field<int>("AssignedResourceID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Workload Assigned To={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "PRIMARYDEVELOPER":
					case "PRIMARYTECHRESOURCE":
					case "PRIMARYTECH. RESOURCE":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("Primary_Developer") == row.Cells[DCC["Primary_Developer"].Ordinal].Text
									   select r.Field<int>("PRIMARYRESOURCEID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Primary Developer={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "PRIMARYBUSINESSRESOURCE":
					case "PRIMARYBUSRESOURCE":
					case "PRIMARYBUS.RESOURCE":
					case "PRIMARYBUS. RESOURCE":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("PrimaryBusinessResource") == row.Cells[DCC["PrimaryBusinessResource"].Ordinal].Text
									   select r.Field<int>("PrimaryBusinessResourceID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Primary Business Resource={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
					case "SUBMITTEDBY":
						columnValue = (from r in DTLookup.AsEnumerable()
									   where r.Field<string>("SubmittedBy") == row.Cells[DCC["SubmittedBy"].Ordinal].Text
									   select r.Field<int>("WorkloadSubmittedByID").ToString()).FirstOrDefault();
						sb.AppendFormat("{0}Workload Submitted By={1}", sb.Length > 0 ? "," : "", columnValue);
						break;
                        //ivt
                        //date needed
                        //title
                        //tech rank
                        //bus rank
                        //production status
                        //sr number
                        //progress
                        //times re-opened
                        //status updated date
                }
            }

            taskCount = DTLookup.Rows.Count;

            string rowNum = row.RowIndex.ToString();
			HtmlGenericControl divTasks = new HtmlGenericControl();
			divTasks.Style["display"] = "table-row";
			divTasks.Style["text-align"] = "right";
			HtmlGenericControl divTaskButtons = new HtmlGenericControl();
			divTaskButtons.Style["display"] = "table-cell";
			divTaskButtons.Controls.Add(createShowHideButton_Tasks(true, "Show", rowNum));
			divTaskButtons.Controls.Add(createShowHideButton_Tasks(false, "Hide", rowNum));
			HtmlGenericControl divTaskCount = new HtmlGenericControl();
			divTaskCount.InnerText = string.Format("({0})", taskCount.ToString());
			divTaskCount.Style["display"] = "table-cell";
			divTaskCount.Style["padding-left"] = "2px";
			divTasks.Controls.Add(divTaskButtons);
			divTasks.Controls.Add(divTaskCount);
			//buttons to show/hide child grid
			row.Cells[DCC["X"].Ordinal].Controls.Clear();
			row.Cells[DCC["X"].Ordinal].Controls.Add(divTasks);

			//add child grid row for Task Items
			Table table = (Table)row.Parent;
			GridViewRow childRow = createChildRow_Tasks(rowNum);
			childRow.Attributes.Add("filters", sb.ToString() + "," + this.Filters);
			table.Rows.AddAt(table.Rows.Count, childRow);
		}
		else  // Task Column
		{

            string itemId = row.Cells[DCC.IndexOf("ItemID")].Text.Trim();
			int.TryParse(row.Cells[DCC["Task_Count"].Ordinal].Text.Trim().Replace("&nbsp", "0"), out taskCount);

            row.Attributes.Add("itemID", itemId);

            row.Cells[DCC["ItemID"].Ordinal].Controls.Add(createEditLink_WorkItem(itemId));

			string description = HttpUtility.HtmlDecode(row.Cells[DCC["DESCRIPTION"].Ordinal].Text);
			string noHTML = Regex.Replace(description, @"<[^>]+>|&nbsp;", "").Trim();
			string noHTMLNormalized = Regex.Replace(noHTML, @"\s{2,}", " ");
			row.Cells[DCC["TITLE"].Ordinal].ToolTip = noHTMLNormalized;
            row.Cells[DCC["TITLE"].Ordinal].Text= HttpUtility.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC["TITLE"].Ordinal].Text));
            row.Cells[DCC["TITLE"].Ordinal].Attributes["width"] = "300px";

            if (taskCount > 0)
			{
				HtmlGenericControl divTasks = new HtmlGenericControl();
				divTasks.Style["display"] = "table-row";
				divTasks.Style["text-align"] = "right";
				HtmlGenericControl divTaskButtons = new HtmlGenericControl();
				divTaskButtons.Style["display"] = "table-cell";
				divTaskButtons.Controls.Add(createShowHideButton_Tasks(true, "Show", itemId));
				divTaskButtons.Controls.Add(createShowHideButton_Tasks(false, "Hide", itemId));
				HtmlGenericControl divTaskCount = new HtmlGenericControl();
				divTaskCount.InnerText = string.Format("({0})", taskCount.ToString());
				divTaskCount.Attributes["class"] = "taskCount_" + itemId;
				divTaskCount.Style["display"] = "table-cell";
				divTaskCount.Style["padding-left"] = "2px";
				divTasks.Controls.Add(divTaskButtons);
				divTasks.Controls.Add(divTaskCount);
				//buttons to show/hide child grid
				row.Cells[DCC["X"].Ordinal].Controls.Clear();
				row.Cells[DCC["X"].Ordinal].Controls.Add(divTasks);

				//add child grid row for Task Items
				Table table = (Table)row.Parent;
				GridViewRow childRow = createChildRow_Tasks(itemId);
				table.Rows.AddAt(table.Rows.Count, childRow);
			}
			else
			{
				Image imgBlank = new Image();
				imgBlank.Height = 10;
				imgBlank.Width = 10;
				imgBlank.ImageUrl = "Images/Icons/blank.png";
				imgBlank.AlternateText = "";
				row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
			}
			
			if (CanEdit)
			{
				row.Cells[DCC.IndexOf("Assigned")].Controls.Add(WTSUtility.CreateGridDropdownList("Assigned", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Assigned")].Text).Trim(), row.Cells[DCC.IndexOf("ASSIGNEDRESOURCEID")].Text.Replace("&nbsp;", " ").Trim(), 0));
				row.Cells[DCC.IndexOf("Primary_Developer")].Controls.Add(WTSUtility.CreateGridDropdownList("Primary_Developer", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Primary_Developer")].Text).Trim(), row.Cells[DCC.IndexOf("PRIMARYRESOURCEID")].Text.Replace("&nbsp;", " ").Trim(), 0));
                // SCB TODO - Not creating this.  IT IS in the DCC!!!
                row.Cells[DCC.IndexOf("SECONDARYRESOURCE")].Controls.Add(WTSUtility.CreateGridDropdownList("SECONDARYRESOURCE", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SECONDARYRESOURCE")].Text).Trim(), row.Cells[DCC.IndexOf("SECONDARYRESOURCEID")].Text.Replace("&nbsp;", " ").Trim(), 0));
                row.Cells[DCC.IndexOf("PrimaryBusinessResource")].Controls.Add(WTSUtility.CreateGridDropdownList("PrimaryBusinessResource", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("PrimaryBusinessResource")].Text).Trim(), row.Cells[DCC.IndexOf("PrimaryBusinessResourceID")].Text.Replace("&nbsp;", " ").Trim(), 0));
                row.Cells[DCC.IndexOf("SecondaryBusinessResource")].Controls.Add(WTSUtility.CreateGridDropdownList("SecondaryBusinessResource", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SecondaryBusinessResource")].Text).Trim(), row.Cells[DCC.IndexOf("SecondaryBusinessResourceID")].Text.Replace("&nbsp;", " ").Trim(), 0));

                row.Cells[DCC.IndexOf("RESOURCEPRIORITYRANK")].Controls.Add(WTSUtility.CreateGridTextBox("RESOURCEPRIORITYRANK", itemId, row.Cells[DCC.IndexOf("RESOURCEPRIORITYRANK")].Text.Replace("&nbsp;", " ").Trim(), true));
                row.Cells[DCC.IndexOf("PrimaryBusinessRank")].Controls.Add(WTSUtility.CreateGridTextBox("PrimaryBusinessRank", itemId, row.Cells[DCC.IndexOf("PrimaryBusinessRank")].Text.Replace("&nbsp;", " ").Trim(), true));
                row.Cells[DCC.IndexOf("SecondaryBusinessRank")].Controls.Add(WTSUtility.CreateGridTextBox("SecondaryBusinessRank", itemId, row.Cells[DCC.IndexOf("SecondaryBusinessRank")].Text.Replace("&nbsp;", " ").Trim(), true));
                row.Cells[DCC.IndexOf("SecondaryResourceRank")].Controls.Add(WTSUtility.CreateGridTextBox("SecondaryResourceRank", itemId, row.Cells[DCC.IndexOf("SecondaryResourceRank")].Text.Replace("&nbsp;", " ").Trim(), true));

                // If we ever want to go back to drop downs:
                //row.Cells[DCC.IndexOf("RESOURCEPRIORITYRANK")].Controls.Add(WTSUtility.CreateGridDropdownList("RESOURCEPRIORITYRANK", itemId, row.Cells[DCC.IndexOf("RESOURCEPRIORITYRANK")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("RESOURCEPRIORITYRANKID")].Text.Replace("&nbsp;", " ").Trim(), 0));
                //row.Cells[DCC.IndexOf("PrimaryBusinessRank")].Controls.Add(WTSUtility.CreateGridDropdownList("PrimaryBusinessRank", itemId, row.Cells[DCC.IndexOf("PrimaryBusinessRank")].Text.Replace("&nbsp;", " ").Trim(), row.Cells[DCC.IndexOf("PrimaryBusinessRankID")].Text.Replace("&nbsp;", " ").Trim(), 0));
            }
        }
	}

	void grdWorkload_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdWorkload.PageIndex = e.NewPageIndex;
		loadGridData();
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			row.Cells[i].Style["text-align"] = "left";
			if (i == DCC.IndexOf("X")
				|| (DCC.Contains("ItemID") && i == DCC.IndexOf("ItemID"))
				|| (DCC.Contains("IVTRequired") && i == DCC.IndexOf("IVTRequired"))
				|| (DCC.Contains("NEEDDATE") && i == DCC.IndexOf("NEEDDATE"))
				|| (DCC.Contains("RESOURCEPRIORITYRANK") && i == DCC.IndexOf("RESOURCEPRIORITYRANK"))
				|| (DCC.Contains("PrimaryBusinessRank") && i == DCC.IndexOf("PrimaryBusinessRank"))
				|| (DCC.Contains("SR_Number") && i == DCC.IndexOf("SR_Number"))
				|| (DCC.Contains("Progress") && i == DCC.IndexOf("Progress"))
				|| (DCC.Contains("ReOpenedCount") && i == DCC.IndexOf("ReOpenedCount"))
				|| (DCC.Contains("StatusUpdatedDate") && i == DCC.IndexOf("StatusUpdatedDate")))
			{
				row.Cells[i].Style["text-align"] = "center";
			}
			else
			{
				row.Cells[i].Style["padding-left"] = "5px";
			}
		}

		//more column formatting
		row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
		row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
		row.Cells[DCC.IndexOf("X")].Style["width"] = "32px";

		if (DCC.Contains("Y"))
		{
			row.Cells[DCC.IndexOf("Y")].Style["display"] = "table-cell";
			row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
			row.Cells[DCC.IndexOf("Y")].Style["width"] = "14px";
		}
		
		if (DCC.Contains("ItemID"))
		{
			row.Cells[DCC.IndexOf("ItemID")].Style["width"] = "46px";
		}
		if (DCC.Contains("WORKITEMTYPE"))
		{
			row.Cells[DCC.IndexOf("WORKITEMTYPE")].Style["width"] = "75px";
		}
		if (DCC.Contains("WorkType"))
		{
			row.Cells[DCC.IndexOf("WorkType")].Style["width"] = "75px";
		}
		if (DCC.Contains("Websystem"))
		{
			row.Cells[DCC.IndexOf("Websystem")].Style["width"] = "75px";
		}
		if (DCC.Contains("STATUS"))
		{
			row.Cells[DCC.IndexOf("STATUS")].Style["width"] = "55px";
		}
		if (DCC.Contains("IVTRequired"))
		{
			row.Cells[DCC.IndexOf("IVTRequired")].Style["width"] = "65px";
		}
		if (DCC.Contains("NEEDDATE"))
		{
			row.Cells[DCC.IndexOf("NEEDDATE")].Style["width"] = "80px";
		}
		if (DCC.Contains("AllocationGroup"))
		{
			row.Cells[DCC.IndexOf("AllocationGroup")].Style["width"] = "150px";
		}
		if (DCC.Contains("AllocationCategory"))
		{
			row.Cells[DCC.IndexOf("AllocationCategory")].Style["width"] = "75px";
		}
		if (DCC.Contains("ALLOCATION"))
		{
			row.Cells[DCC.IndexOf("ALLOCATION")].Style["width"] = "150px";
		}
		if (DCC.Contains("RESOURCEPRIORITYRANK"))
		{
			row.Cells[DCC.IndexOf("RESOURCEPRIORITYRANK")].Style["width"] = "50px";
		}
		if (DCC.Contains("PrimaryBusinessRank"))
		{
			row.Cells[DCC.IndexOf("PrimaryBusinessRank")].Style["width"] = "50px";
		}
		if (DCC.Contains("Assigned"))
		{
			row.Cells[DCC.IndexOf("Assigned")].Style["width"] = "145px";
		}
		if (DCC.Contains("Primary_Developer"))
		{
			row.Cells[DCC.IndexOf("Primary_Developer")].Style["width"] = "145px";
		}
		if (DCC.Contains("PrimaryBusinessResource"))
		{
			row.Cells[DCC.IndexOf("PrimaryBusinessResource")].Style["width"] = "145px";
		}
        if (DCC.Contains("SecondaryBusinessResource"))
        {
            row.Cells[DCC.IndexOf("SecondaryBusinessResource")].Style["width"] = "145px";
        }
        if (DCC.Contains("WorkArea"))
		{
			row.Cells[DCC.IndexOf("WorkArea")].Style["width"] = "125px";
		}
		if (DCC.Contains("WorkloadGroup"))
		{
			row.Cells[DCC.IndexOf("WorkloadGroup")].Style["width"] = "125px";
		}
		if (DCC.Contains("ProductionStatus"))
		{
			row.Cells[DCC.IndexOf("ProductionStatus")].Style["width"] = "60px";
		}
		if (DCC.Contains("Version"))
		{
			row.Cells[DCC.IndexOf("Version")].Style["width"] = "45px";
		}
		if (DCC.Contains("SR_Number"))
		{
			row.Cells[DCC.IndexOf("SR_Number")].Style["width"] = "55px";
		}
		if (DCC.Contains("Priority"))
		{
			row.Cells[DCC.IndexOf("Priority")].Style["width"] = "45px";
		}
		if (DCC.Contains("SubmittedBy"))
		{
			row.Cells[DCC.IndexOf("SubmittedBy")].Style["width"] = "145px";
		}
		if (DCC.Contains("Progress"))
		{
			row.Cells[DCC.IndexOf("Progress")].Style["width"] = "61px";
		}
		if (DCC.Contains("ReOpenedCount"))
		{
			row.Cells[DCC.IndexOf("ReOpenedCount")].Style["width"] = "50px";
		}
		if (DCC.Contains("StatusUpdatedDate"))
		{
			row.Cells[DCC.IndexOf("StatusUpdatedDate")].Style["width"] = "80px";
		}

		if (DCC.Count > 0 && !TaskColumn) row.Cells[DCC.Count - 1].Style["width"] = "auto";
	}

	Image createShowHideButton_Tasks(bool show = false, string direction = "Show", string itemId = "ALL")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("imgShowHideChildTasks_click(this,'{0}','{1}');", direction, itemId);

		Image img = new Image();
		img.ID = string.Format("img{0}Children{1}_{2}", direction, !TaskColumn ? "Group" : "", itemId);
		img.Style["display"] = show ? "block" : "none";
		img.Style["cursor"] = "pointer";
		img.Attributes.Add("Name", string.Format("img{0}", direction));
		img.Attributes.Add("workItemId", itemId);
		img.Height = 10;
		img.Width = 10;
		img.ImageUrl = direction.ToUpper() == "SHOW"
			? "Images/Icons/add_blue.png"
			: "Images/Icons/minus_blue.png";
		img.ToolTip = !TaskColumn ? string.Format("{0} Tasks", direction) : string.Format("{0} Sub-Tasks for [{1}]", direction, itemId);
		img.AlternateText = !TaskColumn ? string.Format("{0} Tasks", direction) : string.Format("{0} Sub-Tasks for [{1}]", direction, itemId);
		img.Attributes.Add("Onclick", sb.ToString());

		return img;
	}

	LinkButton createEditLink_WorkItem(string workItemId = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEditWorkItem_click('{0}');return false;", workItemId);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEditWorkItem_{0}", workItemId);
		lb.Attributes["name"] = string.Format("lbEditWorkItem_{0}", workItemId);
		lb.ToolTip = string.Format("Edit Work Item [{0}]", workItemId);
		lb.Text = workItemId;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}

	GridViewRow createChildRow_Tasks(string workItemId = "")
	{
		GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
		TableCell tableCell = null;

		try
		{
			row.CssClass = "gridBody";
			row.Style["display"] = "none";
			row.ID = string.Format("{0}_{1}", !TaskColumn ? "gridChildGroup" : "gridChildTasks", workItemId);
			row.Attributes.Add("workItemId", workItemId);
			row.Attributes.Add("Name", string.Format("{0}_{1}", !TaskColumn ? "gridGroup" : "gridChild", workItemId));

			//add the table cells
			for (int i = 0; i < DCC.Count; i++)
			{
				tableCell = new TableCell();
				tableCell.Text = "&nbsp;";

				if (i == DCC["X"].Ordinal)
				{
					//set width to match parent
					tableCell.Style["width"] = "12px";
					tableCell.Style["border-right"] = "1px solid transparent";
				}
				else if ((!TaskColumn && i == 1) || (TaskColumn && i == DCC["ItemID"].Ordinal))
				{
					tableCell.Style["padding-top"] = "10px";
					tableCell.Style["padding-right"] = "10px";
					tableCell.Style["padding-bottom"] = "0px";
					tableCell.Style["padding-left"] = "0px";
					tableCell.Style["vertical-align"] = "top";
					tableCell.Style["border-right"] = "1px solid transparent";
					tableCell.ColumnSpan = DCC.Count - 1;
					//add the frame here
					tableCell.Controls.Add(createChildFrame_Tasks(workItemId: workItemId));
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

	HtmlIframe createChildFrame_Tasks(string workItemId = "")
	{
		HtmlIframe childFrame = new HtmlIframe();

		if (string.IsNullOrWhiteSpace(workItemId))
		{
			return null;
		}

		childFrame.ID = string.Format("{0}_{1}", !TaskColumn ? "frameChildGroup" : "frameChildTasks", workItemId);
		childFrame.Attributes.Add("workItemId", workItemId);
		childFrame.Attributes["frameborder"] = "0";
		childFrame.Attributes["scrolling"] = "no";
		childFrame.Attributes["src"] = "javascript:''";
		childFrame.Style["height"] = "30px";
		childFrame.Style["width"] = "100%";
		childFrame.Style["border"] = "none";

		return childFrame;
	}

	private void RemoveColumns(ref DataTable dt)
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
			}

			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}

	[WebMethod(true)]
	public static string SaveChanges(string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
			, { "failed", "0" }
			, { "savedIds", "" }
			, { "failedIds", "" }
			, { "error", "" } };
        bool saved = false;
        int savedInt = 0;
        bool isDev = false;
        bool isBus = false;

        int savedQty = 0, failedQty = 0;
		string ids = string.Empty, failedIds = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

		try
		{
			DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjson.Rows.Count == 0)
			{
                errorMsg = "Unable to save. An invalid list of changes was provided.";
				saved = false;
			}
			else
			{
                int id = 0;
                int techRank = 0, busRank = 0, secBusRank = 0, secTechRank;
				string description = string.Empty;
				int assigned = 0, primaryDeveloper = 0, primaryBusinessResource = 0, secondaryBusinessDeveloper = 0;

                HttpServerUtility server = HttpContext.Current.Server;
				//save
				foreach (DataRow dr in dtjson.Rows)
				{
					tempMsg = string.Empty;
					int.TryParse(dr["ItemID"].ToString(), out id);
					int.TryParse(dr["PrimaryBusinessRank"].ToString(), out busRank);
                    int.TryParse(dr["SecondaryBusinessRank"].ToString(), out secBusRank); 
                    int.TryParse(dr["RESOURCEPRIORITYRANK"].ToString(), out techRank);
                    int.TryParse(dr["SECONDARYRESOURCERANK"].ToString(), out secTechRank);
                    int.TryParse(dr["Assigned"].ToString(), out assigned);
					int.TryParse(dr["Primary_Developer"].ToString(), out primaryDeveloper);
					int.TryParse(dr["PrimaryBusinessResource"].ToString(), out primaryBusinessResource);
                    int.TryParse(dr["SecondaryBusinessResource"].ToString(), out secondaryBusinessDeveloper); 
                    
                    description = server.UrlDecode(dr["DESCRIPTION"].ToString());

                    WorkloadItem item = WorkloadItem.WorkItem_GetObject(id);
					item.Description = description;
					item.PrimaryBusinessRank = busRank;
					item.ResourcePriorityRank = techRank;
                    item.SecondaryBusinessRank = secBusRank;
                    item.SecondaryResourceRank = secTechRank;
                    item.AssignedResourceID = assigned;
					item.PrimaryResourceID = primaryDeveloper;
					item.PrimaryBusinessResourceID = primaryBusinessResource;
                    item.SecondaryBusinessResourceID = secondaryBusinessDeveloper;


                    // 3-13-2017 - If Assigned To is Dev, set primaryTechRes same ID - same with Bus.
                    isDev = WTSData.GetScalarBool("SELECT IsDeveloper FROM WTS_RESOURCE WHERE WTS_RESOURCEID = " + item.AssignedResourceID);
                    if (isDev & item.PrimaryResourceID == 67)
                    {
                        item.PrimaryResourceID = item.AssignedResourceID;
                    }
                    else
                    {
                        isBus = WTSData.GetScalarBool("SELECT IsBusAnalyst FROM WTS_RESOURCE WHERE WTS_RESOURCEID = " + item.AssignedResourceID);
                        if (isBus & item.PrimaryBusinessResourceID == 67)
                            item.PrimaryBusinessResourceID = item.AssignedResourceID;
                    }


                    savedInt = WorkloadItem.WorkItem_Update(item, out tempMsg);

                    if (savedInt == 0)
                    {
                        saved = false;
                    }
                    else
                    {
                        saved = true;
                    }

                    if (saved)
					{
                        ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
						savedQty += 1;
					}
					else
					{
						failedQty += 1;
					}

					if (tempMsg.Length > 0)
					{
						errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
					}
				}
			}
		}
		catch (Exception ex)
		{
			saved = false;
			errorMsg = ex.Message;
			LogUtility.LogException(ex);
		}

		result["savedIds"] = ids;
		result["failedIds"] = failedIds;
		result["saved"] = savedQty.ToString();
		result["failed"] = failedQty.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
}