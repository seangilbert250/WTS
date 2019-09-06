using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;


public partial class CrosswalkParamContainer : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected DataColumnCollection DCC_Child;
	protected GridCols columnData = new GridCols();
	protected GridCols columnData_Child = new GridCols();

	protected string Grid_View = string.Empty;
	protected string ParentColumns = string.Empty;
	protected string RollupGroup = "Status";
	protected string DefaultSortType = "Tech";
	protected string ChildColumns = string.Empty;

	protected string SortableColumns = string.Empty;
	protected string SortOrder = string.Empty;
	protected string DefaultColumnOrder = string.Empty;
	protected string SelectedColumnOrder = string.Empty;

	protected string SortableColumns_Child = string.Empty;
	protected string SortOrder_Child = string.Empty;
	protected string DefaultColumnOrder_Child = string.Empty;
	protected string SelectedColumnOrder_Child = string.Empty;

	protected bool ColumnOrderChanged = false;
	protected int ActiveTab = 0;

    protected bool columnOrderPref = false;

    protected string ddlChanged = "no";

    protected void Page_Load(object sender, EventArgs e)
	{
		readQueryString();

		loadLookupData();
		loadColumnData();

        HttpContext.Current.Session["Crosswalk_GridView"] = ddlView.SelectedItem.ToString();

    }

    private void readQueryString()
	{
        #region Grid View

        if (Request.QueryString["ddlChanged"] != null)
        {
            ddlChanged = Request.QueryString["ddlChanged"].ToString();
        }


        if (Session["Crosswalk_GridView"] != null && !string.IsNullOrWhiteSpace(Session["Crosswalk_GridView"].ToString()))
		{
			this.Grid_View = Session["Crosswalk_GridView"].ToString();
		}
		else
		{
			if (Request.QueryString["gridView"] != null
				&& !string.IsNullOrWhiteSpace(Request.QueryString["gridView"].ToString()))
			{
				this.Grid_View = Server.UrlDecode(Request.QueryString["gridView"]).Replace("?", "");
			}
			else
			{
				this.Grid_View = string.Empty;
			}
			Session["Crosswalk_GridView"] = this.Grid_View;
		}

		#endregion Grid View

		#region Rollup

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
		ListItem item = ddlRollupGroup.Items.FindByText(this.RollupGroup);
		if (item != null)
		{
			item.Selected = true;
		}

        #endregion Rollup

        //      if (Session["Crosswalk_DefaultSortType"] != null && !string.IsNullOrWhiteSpace(Session["Crosswalk_DefaultSortType"].ToString()))
        //{
        //	this.DefaultSortType = Session["Crosswalk_DefaultSortType"].ToString();
        //}
        //else
        //{
        //	if (Request.QueryString["rankSortType"] != null
        //		&& !string.IsNullOrWhiteSpace(Request.QueryString["rankSortType"].ToString()))
        //	{
        //		this.DefaultSortType = Server.UrlDecode(Request.QueryString["rankSortType"]).Replace("?", "");
        //	}
        //	else
        //	{
        //		this.DefaultSortType = "Tech";
        //	}
        //	Session["Crosswalk_DefaultSortType"] = this.DefaultSortType;
        //}

        // 11626 - 2 > Read user preference.
        if (Session["defaultCrosswalkGrid"] != null && !string.IsNullOrWhiteSpace(Session["defaultCrosswalkGrid"].ToString()))
        { 
            this.DefaultSortType = Session["defaultCrosswalkGrid"].ToString();
        }
        else
        {
            this.DefaultSortType = "Tech";
        }

        item = ddlDefaultTaskSort.Items.FindByValue(this.DefaultSortType);
		if (item != null)
		{
			item.Selected = true;
		}

        #region Parent Columns

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
				this.ParentColumns = "WTS_SYSTEM";
			}
			Session["Crosswalk_SelectedParentColumns"] = this.ParentColumns;
		}
		
		if (Session["Crosswalk_SelectedParentColumnOrder"] != null
			&& !string.IsNullOrWhiteSpace(Session["Crosswalk_SelectedParentColumnOrder"].ToString()))
		{
			this.SelectedColumnOrder = Session["Crosswalk_SelectedParentColumnOrder"].ToString();
		}

		#endregion Parent Columns
		

		#region Child Columns

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
				this.ChildColumns = "X,TaskNumber,System,Status,Title,AssignedTo,PrimaryDeveloper,WorkArea,Functionality,ProductionStatus,Version,Priority,Progress,Y";
			}
			Session["Crosswalk_SelectedChildColumns"] = this.ChildColumns;
		}

		if (Session["Crosswalk_SelectedChildColumnOrder"] != null
			&& !string.IsNullOrWhiteSpace(Session["Crosswalk_SelectedChildColumnOrder"].ToString()))
		{
			this.SelectedColumnOrder_Child = Session["Crosswalk_SelectedChildColumnOrder"].ToString();
		}

		if (string.IsNullOrWhiteSpace(this.SelectedColumnOrder_Child))
		{
            // 2-3-2017 = Removed ResourcePriorityRankID
			this.SelectedColumnOrder_Child = "X|X|true|false||False~WORKREQUESTID|WORKREQUESTID|false|false||False~WORKREQUEST|WORKREQUEST|false|false||False~ItemID|Task Number|true|true||True~WORKITEMTYPEID|WORKITEMTYPEID|false|false||False~WORKITEMTYPE|Item Type|false|true||True~WorkTypeID|WorkTypeID|false|false||False~WorkType|Work Type|false|true||True~Task_Count|Task Count|false|false||False~WTS_SYSTEMID|WTS_SYSTEMID|false|false||False~Websystem|System|true|true||True~STATUSID|STATUSID|false|false||False~STATUS|Status|true|true||True~IVTRequired|Requires IVT|false|true||True~NEEDDATE|Date Needed|false|true||True~TITLE|Title|true|true||True~DESCRIPTION|DESCRIPTION|false|false||False~RESOURCEPRIORITYRANK|Primary Tech. Rank|false|true||False~PrimaryBusinessRank|Primary Bus. Rank|false|true|Resources|True~WorkAreaID|WorkAreaID|false|false||False~WorkArea|Work Area|true|true||True~WorkloadGroupID|WorkloadGroupID|false|false||False~WorkloadGroup|Functionality|true|true||True~Production|Production|true|false||False~ProductVersionID|ProductVersionID|false|false||False~Version|Version|true|true||True~ProductionStatusID|ProductionStatusID|false|false||False~ProductionStatus|Production Status|true|true||True~SR_Number|SR Number|false|true||True~PRIORITYID|PRIORITYID|false|false||False~PRIORITY|Priority|true|true||True~ASSIGNEDRESOURCEID|ASSIGNEDRESOURCEID|false|false||False~Assigned|Assigned To|true|true|Resources|True~SMEID|SMEID|false|false||False~Primary_Analyst|Primary_Analyst|false|false||False~PRIMARYRESOURCEID|PRIMARYRESOURCEID|false|false||False~Primary_Developer|Primary Developer|true|true|Resources|True~PrimaryBusinessResourceID|PrimaryBusinessResourceID|false|false||False~PrimaryBusinessResource|Primary Bus. Resource|false|true|Resources|True~SECONDARYRESOURCEID|SECONDARYRESOURCEID|false|false||False~SECONDARYRESOURCE|SECONDARYRESOURCE|false|false||False~CREATEDBY|CREATEDBY|false|false||False~CREATEDDATE|CREATEDDATE|false|false||False~SubmittedByID|SubmittedByID|false|false||False~SubmittedBy|Submitted By|false|true||True~Progress|Progress|true|true||True~ARCHIVE|ARCHIVE|false|false||False~Status_Sort|Status_Sort|false|false||False~ReOpenedCount|Times Re-Opened|false|true||True~StatusUpdatedDate|Status Updated Date|false|true||True~Y|Y|true|false||False~Z|Z|false|false||False";
            //this.SelectedColumnOrder_Child = "X|X|true|false||False~WORKREQUESTID|WORKREQUESTID|false|false||False~WORKREQUEST|WORKREQUEST|false|false||False~ItemID|Task Number|true|true||True~PhaseID|PhaseID|false|false||False~Phase|Phase|true|true||True~WORKITEMTYPEID|WORKITEMTYPEID|false|false||False~WORKITEMTYPE|Item Type|false|true||True~WorkTypeID|WorkTypeID|false|false||False~WorkType|Work Type|false|true||True~Task_Count|Task Count|false|false||False~WTS_SYSTEMID|WTS_SYSTEMID|false|false||False~Websystem|System|true|true||True~STATUSID|STATUSID|false|false||False~STATUS|Status|true|true||True~IVTRequired|Requires IVT|false|true||True~NEEDDATE|Date Needed|false|true||True~TITLE|Title|true|true||True~DESCRIPTION|DESCRIPTION|false|false||False~AllocationGroupID|AllocationGroupID|false|false||False~AllocationGroup|Allocation Group|false|true||True~AllocationCategoryID|AllocationCategoryID|false|false||False~AllocationCategory|Allocation Category|false|false||False~ALLOCATIONID|ALLOCATIONID|false|false||False~ALLOCATION|Allocation Assignment|false|true||False~RESOURCEPRIORITYRANK|Primary Tech. Rank|false|true|Resources||False~PrimaryBusinessRank|Primary Bus. Rank|false|true|Resources|True~WorkAreaID|WorkAreaID|false|false||False~WorkArea|Work Area|true|true||True~WorkloadGroupID|WorkloadGroupID|false|false||False~WorkloadGroup|Functionality|true|true||True~Production|Production|true|false||False~ProductVersionID|ProductVersionID|false|false||False~Version|Version|true|true||True~ProductionStatusID|ProductionStatusID|false|false||False~ProductionStatus|Production Status|true|true||True~SR_Number|SR Number|false|true||True~PRIORITYID|PRIORITYID|false|false||False~PRIORITY|Priority|true|true||True~ASSIGNEDRESOURCEID|ASSIGNEDRESOURCEID|false|false||False~Assigned|Assigned To|true|true|Resources|True~SMEID|SMEID|false|false||False~Primary_Analyst|Primary_Analyst|false|false||False~PRIMARYRESOURCEID|PRIMARYRESOURCEID|false|false||False~Primary_Developer|Primary Developer|true|true|Resources|True~PrimaryBusinessResourceID|PrimaryBusinessResourceID|false|false||False~PrimaryBusinessResource|Primary Bus. Resource|false|true|Resources|True~SECONDARYRESOURCEID|SECONDARYRESOURCEID|false|false||False~SECONDARYRESOURCE|SECONDARYRESOURCE|false|false||False~CREATEDBY|CREATEDBY|false|false||False~CREATEDDATE|CREATEDDATE|false|false||False~SubmittedByID|SubmittedByID|false|false||False~SubmittedBy|Submitted By|false|true||True~Progress|Progress|true|true||True~ARCHIVE|ARCHIVE|false|false||False~Status_Sort|Status_Sort|false|false||False~ReOpenedCount|Times Re-Opened|false|true||True~StatusUpdatedDate|Status Updated Date|false|true||True~Y|Y|true|false||False~Z|Z|false|false||False";
        }

        #endregion Child Columns

        if (Request.QueryString["Tab"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["Tab"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["Tab"].ToString()), out this.ActiveTab);
		}
	}

	private void loadLookupData()
	{
		int UserId = UserManagement.GetUserId_FromUsername();
		WTS_User u = new WTS_User(UserId);
		u.Load();
		DataTable dt = WTSData.GetViewOptions(userId: UserId, gridName: "Workload Crosswalk");
		DataTable dtSetting = u.UserSettingList_Get((int)UserSettingType.GridView, (int)WTSGridName.Workload_Crosswalk);

		if (dt != null && dt.Rows.Count > 0)
		{
			ddlView.Items.Clear();
			ddlViewAdv.Items.Clear();

			ListItem item = null;
			foreach (DataRow row in dt.Rows)
			{
				item = new ListItem();
				item.Text = row["ViewName"].ToString();
				item.Value = row["GridViewID"].ToString();
				item.Attributes.Add("OptionGroup", row["WTS_RESOURCEID"].ToString() != "" ? "Custom Views" : "Process Views");
				item.Attributes.Add("MyView", row["MyView"].ToString());
				item.Attributes.Add("Tier1RollupGroup", row["Tier1RollupGroup"].ToString());
				item.Attributes.Add("Tier1ColumnOrder", row["Tier1ColumnOrder"].ToString());
				item.Attributes.Add("Tier2ColumnOrder", row["Tier2ColumnOrder"].ToString());
				item.Attributes.Add("DefaultSortType", row["Tier2SortOrder"].ToString());

				if (ddlView.Items.FindByText(row["ViewName"].ToString()) == null)
				{
					ddlView.Items.Add(item);
					ddlViewAdv.Items.Add(item);
				}
			}

            // 11626 - 2 > Use saved preferences:
            if (Session["defaultCrosswalkGrid"] != null && !string.IsNullOrWhiteSpace(Session["defaultCrosswalkGrid"].ToString()) && ddlChanged != "yes")
                {
                ListItem itemGridView = ddlView.Items.FindByText(Session["defaultCrosswalkGrid"].ToString());
                if (itemGridView != null)
                {
                    itemGridView.Selected = true;
                }
                else
                {
                    this.Grid_View = string.Empty;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(this.Grid_View))
                {
                    ListItem itemGridView = ddlView.Items.FindByText(this.Grid_View);
                    if (itemGridView != null)
                    {
                        itemGridView.Selected = true;
                    }
                    else
                    {
                        this.Grid_View = string.Empty;
                    }
                    itemGridView = ddlViewAdv.Items.FindByText(this.Grid_View);
                    if (itemGridView != null)
                    {
                        itemGridView.Selected = true;
                    }
                }
                else
                {
                    if (dtSetting != null && dtSetting.Rows.Count > 0)
                    {
                        WTSUtility.SelectDdlItem(ddlView, dtSetting.Rows[0]["SettingValue"].ToString().Trim());
                        WTSUtility.SelectDdlItem(ddlViewAdv, dtSetting.Rows[0]["SettingValue"].ToString().Trim());
                    }
                }
            }
        }

        columnOrderPref = WTSData.WTS_GetColumnOrderingPref();
	}

	private void loadColumnData()
	{
        //procName = "QM_Workload_Crosswalk_Grid"
        DataTable dt = Workload.Workload_Crosswalk_Get(columnListOnly: 1);
		if (dt != null)
		{
			InitializeColumnData(ref dt);
			dt.AcceptChanges();
			DCC = dt.Columns;
		}

		DataTable dtChild = WorkloadItem.WorkItemList_Get(columnListOnly: 1, myData: false);
		if (dtChild != null)
		{
			InitializeColumnData_Child(ref dtChild);
			dtChild.AcceptChanges();
			DCC_Child = dtChild.Columns;
		}
	}

	protected void InitializeColumnData(ref DataTable dt)
	{
		try
		{
			string displayName = string.Empty, groupName = string.Empty;
			bool blnVisible = false, isViewable = false, blnSortable = false, blnOrderable = false;

			bool statusRollup = false, priorityRollup = false;
			statusRollup = (this.RollupGroup.ToUpper() == "STATUS");
			priorityRollup = (this.RollupGroup.ToUpper() == "PRIORITY");

			foreach (DataColumn gridColumn in dt.Columns)
			{
				displayName = gridColumn.ColumnName;
				blnVisible = false;
				blnSortable = false;
				blnOrderable = false;
				isViewable = false;
				groupName = "&nbsp;";

				switch (gridColumn.ColumnName)
				{
					case "X":
						displayName = "";
						groupName = string.Empty;
						blnVisible = true;
						blnSortable = false;
						blnOrderable = false;
						isViewable = false;
						break;
					case "Y":
						displayName = "&nbsp;";
						groupName = string.Empty;
						blnVisible = true;
						blnSortable = false;
						blnOrderable = false;
						isViewable = false;
						break;
					//case "TITLE":
					//	displayName = "Work Request";
					//	groupName = string.Empty;
					//	blnVisible = false;
					//	blnSortable = true;
					//	blnOrderable = true;
					//	isViewable = true;
					//	break;
					case "WorkType":
						displayName = "Resource Group";
						groupName = string.Empty;
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "WORKITEMTYPE":
						displayName = "Work Activity";
						groupName = string.Empty;
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "WTS_SYSTEM":
						displayName = "System(Task)";
						groupName = string.Empty;
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					//case "AllocationGroup":
					//	displayName = "Allocation Group";
					//	groupName = string.Empty;
					//	blnVisible = false;
					//	blnSortable = true;
					//	blnOrderable = true;
					//	isViewable = true;
					//	break;
					//case "AllocationGroup_Sort":
					//	displayName = "Allocation Group Sort";
					//	groupName = "Allocation Group";
					//	blnVisible = true;
					//	blnSortable = true;
					//	blnOrderable = true;
					//	isViewable = false;
					//	break;
					//case "AllocationCategory":
					//	displayName = "Allocation Category";
					//	groupName = string.Empty;
					//	blnVisible = false;
					//	blnSortable = false;
					//	blnOrderable = false;
					//	isViewable = false;
					//	break;
					//case "Allocation":
					//	displayName = "Allocation Assignment";
					//	groupName = string.Empty;
					//	blnVisible = true;
					//	blnSortable = true;
					//	blnOrderable = true;
					//	isViewable = true;
					//	break;
					//case "Allocation_Sort":
					//	displayName = "Allocation Sort";
					//	groupName = "Allocation Assignment";
					//	blnVisible = true;
					//	blnSortable = true;
					//	blnOrderable = true;
					//	isViewable = false;
					//	break;
					case "WorkArea":
						displayName = "Work Area";
						groupName = string.Empty;// "Work Area";
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "WA_Sort":
						displayName = "WA Sort";
						groupName = "Work Area";
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = false;
						break;
					case "WorkloadGroup":
						displayName = "Functionality";
						groupName = string.Empty;// "Functionality";
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "WG_Sort":
						displayName = "Functionality Sort";
						groupName = "Functionality";
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = false;
						break;
					case "ProductionStatus":
						displayName = "Production Status";
						groupName = string.Empty;
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "ProductVersion":
						displayName = "Version";
						groupName = string.Empty;
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "Priority":
						displayName = "Priority";
						groupName = string.Empty;
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "Affiliated":
						displayName = "Affiliated";
						groupName = "Resources";
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "AssignedTo":
						displayName = "Assigned To";
						groupName = "Resources";
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "Primary_Developer":
						displayName = "Primary Resource";
						groupName = "Resources";
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "PrimaryBusinessResource":
						displayName = "Primary Bus. Resource";
						groupName = "Resources";
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
                    case "SecondaryBusinessResource":
                        displayName = "Secondary Bus. Resource";
                        groupName = "Resources";
                        blnVisible = false;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WorkloadSubmittedBy":
						displayName = "Submitted By";
						groupName = "Resources";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "Primary_Analyst":
						displayName = "Primary Analyst";
						groupName = "Resources";
						blnVisible = false;
						blnSortable = false;
						blnOrderable = false;
						isViewable = false;
						break;
					case "PRIMARYRESOURCE":
						displayName = "Primary Developer";
						groupName = "Resources";
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "STATUS":
						displayName = "Status";
						groupName = string.Empty;
						blnVisible = false;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
                    //case "PDDTDR_PHASEID":
                    //    displayName = "PD Phase ID";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    blnOrderable = true;
                    //    isViewable = true;
                    //    break;
                    case "Total_Items":
						displayName = "Tasks";
						groupName = "Total";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "Open_Items":
						displayName = "Open";
						groupName = "Open";
						blnVisible = false;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "High_Items":
						displayName = "High";
						groupName = "Priority";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "Medium_Items":
						displayName = "Medium";
						groupName = "Priority";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "Low_Items":
						displayName = "Low";
						groupName = "Priority";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "NA_Items":
						displayName = "N/A";
						groupName = "Priority";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "OnHold_Items":
						displayName = "On Hold";
						groupName = "On Hold";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = false;
						break;
					case "InfoRequested_Items":
						displayName = "Info<br />Requested";
						groupName = "On Hold";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = false;
						break;
					case "New_Items":
						displayName = "New";
						groupName = "Open";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "InProgress_Items":
						displayName = "In<br />Progress";
						groupName = "Open";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "ReOpened_Items":
						displayName = "Re-Opened";
						groupName = "Open";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "InfoProvided_Items":
						displayName = "Info<br />Provided";
						groupName = "Open";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "UnReproducible_Items":
						displayName = "Un-<br />Reproducible";
						groupName = "Open";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "CheckedIn_Items":
						displayName = "Checked In";
						groupName = "Awaiting Closure";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "Deployed_Items":
						displayName = "Deployed";
						groupName = "Awaiting Closure";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "Closed_Items":
						displayName = "Closed";
						groupName = "Closed";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "Percent_OnHold_Items":
						displayName = "% On Hold";
						groupName = "Items %";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "Percent_Open_Items":
						displayName = "% Open";
						groupName = "Items %";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
					case "Percent_Closed_Items":
						displayName = "% Closed";
						groupName = "Items %";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						isViewable = false;
						break;
				}
				blnVisible = (ParentColumns.Replace(" ", "").IndexOf(displayName.Replace(" ", "")) > -1);

				columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
				columnData.Columns.Item(columnData.Columns.Count - 1).Viewable = isViewable;
				columnData.Columns.Item(columnData.Columns.Count - 1).GroupName = groupName;
			}

			//Initialize the columnData
			columnData.Initialize(ref dt, ";", "~", "|");

			//Get sortable columns and default column order
			SortableColumns = columnData.SortableColumnsToString();
			DefaultColumnOrder = columnData.DefaultColumnOrderToString();
			//Sort and Reorder Columns
			columnData.ReorderDataTable(ref dt, SelectedColumnOrder);
			columnData.SortDataTable(ref dt, SortOrder);
			SelectedColumnOrder = columnData.CurrentColumnOrderToString();
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}

	protected void InitializeColumnData_Child(ref DataTable dt)
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
					case "ItemID":
						displayName = "Task Number";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
                    //case "Phase":
                    //    displayName = "Phase";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    blnOrderable = true;
                    //    isViewable = true;
                    //    break;
                    case "WORKITEMTYPE":
						displayName = "Work Activity";
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "WorkType":
						displayName = "Resource Group";
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "Task_Count":
						displayName = "Task Count";
						blnSortable = true;
						break;
					case "Websystem":
						displayName = "System(Task)";
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
					//case "AllocationGroup":
					//	displayName = "Allocation Group";
					//	blnSortable = true;
					//	blnOrderable = true;
					//	isViewable = true;
					//	break;
					//case "AllocationCategory":
					//	displayName = "Allocation Category";
					//	blnSortable = false;
					//	blnOrderable = false;
					//	isViewable = false;
					//	break;
					//case "ALLOCATION":
					//	displayName = "Allocation Assignment";
					//	blnSortable = true;
					//	blnOrderable = true;
					//	isViewable = true;
					//	break;
					case "RESOURCEPRIORITYRANK":
						displayName = "Primary Tech. Rank";
						groupName = "Resources";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "PrimaryBusinessRank":
						displayName = "Primary Bus. Rank";
						groupName = "Resources";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
                    case "SecondaryBusinessRank":
                        displayName = "Secondary Bus. Rank";
                        groupName = "Resources";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "SecondaryResourceRank":
                        displayName = "Secondary Tech. Rank";
                        groupName = "Resources";
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
						blnSortable = true;
						blnOrderable = true;
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
                    //case "PDDTDR_PHASEID":
                    //    displayName = "PD Phase ID";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    blnOrderable = true;
                    //    isViewable = true;
                    //    break;
                    case "Primary_Developer":
						displayName = "Primary Developer";
						groupName = "Resources";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "PrimaryBusinessResource":
						displayName = "Primary Bus. Resource";
						groupName = "Resources";
                        blnVisible = false;
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
					case "ReOpenedCount":
						displayName = "Times Re-Opened";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "StatusUpdatedDate":
						displayName = "Status Updated Date";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = true;
						isViewable = true;
						break;
					case "Y":
						displayName = "Y";
						blnVisible = true;
						forceChecked = true;
						break;
				}
				blnVisible = (forceChecked || (ChildColumns.Replace(" ", "").IndexOf(displayName.Replace(" ", "")) > -1));

				columnData_Child.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				columnData_Child.Columns.Item(columnData_Child.Columns.Count - 1).CanReorder = blnOrderable;
				columnData_Child.Columns.Item(columnData_Child.Columns.Count - 1).Viewable = isViewable;
				columnData_Child.Columns.Item(columnData_Child.Columns.Count - 1).GroupName = groupName;
			}

			//Initialize the columnData
			this.columnData_Child.Initialize(ref dt, ";", "~", "|");

			//Get sortable columns and default column order
			this.SortableColumns_Child = this.columnData_Child.SortableColumnsToString();
			this.DefaultColumnOrder_Child = this.columnData_Child.DefaultColumnOrderToString();
			//Sort and Reorder Columns
			this.columnData_Child.ReorderDataTable(ref dt, SelectedColumnOrder_Child);
			this.columnData_Child.SortDataTable(ref dt, SortOrder_Child);
			this.SelectedColumnOrder_Child = this.columnData_Child.CurrentColumnOrderToString();
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}


	[WebMethod(true)]
	public static string SetCrosswalkColumnSettings(string gridView
		, string rollupGroup
		, string parentColumns, string selectedParentColumnOrder
		, string childColumns, string selectedChildColumnOrder
		, string sortType)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "error", "" } };
		bool saved = false;
		string errorMsg = string.Empty;

		try
		{
			HttpContext.Current.Session["Crosswalk_GridView"] = gridView;
			HttpContext.Current.Session["Crosswalk_RollupGroup"] = rollupGroup;
			HttpContext.Current.Session["Crosswalk_SelectedParentColumns"] = parentColumns;
			HttpContext.Current.Session["Crosswalk_SelectedParentColumnOrder"] = selectedParentColumnOrder;
			HttpContext.Current.Session["Crosswalk_SelectedChildColumns"] = childColumns;
			HttpContext.Current.Session["Crosswalk_SelectedChildColumnOrder"] = selectedChildColumnOrder;
			HttpContext.Current.Session["Crosswalk_DefaultSortType"] = sortType;
			
			saved = true;
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod(true)]
	public static string SetCrosswalkRollupGroup(string rollupGroup)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "error", "" } };
		bool saved = false;
		string errorMsg = string.Empty;

		try
		{
			HttpContext.Current.Session["Crosswalk_RollupGroup"] = rollupGroup;

			saved = true;
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod(true)]
	public static string SetCrosswalkParentColumnOrder(string parentColumns, string selectedColumnOrder)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "error", "" } };
		bool saved = false;
		string errorMsg = string.Empty;

		try
		{
			HttpContext.Current.Session["Crosswalk_SelectedParentColumns"] = parentColumns;
			HttpContext.Current.Session["Crosswalk_SelectedParentColumnOrder"] = selectedColumnOrder;

			saved = true;
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
	
	[WebMethod(true)]
	public static string SetCrosswalkChildColumnOrder(string childColumns, string selectedColumnOrder)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "error", "" } };
		bool saved = false;
		string errorMsg = string.Empty;

		try
		{
			HttpContext.Current.Session["Crosswalk_SelectedChildColumns"] = childColumns;
			HttpContext.Current.Session["Crosswalk_SelectedChildColumnOrder"] = selectedColumnOrder;

			saved = true;
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

    [WebMethod(true)]
    public static void SaveColumnOrderingPref(bool useColumnOrdering)
    {
        WTSData.WTS_SaveColumnOrderingPref(useColumnOrdering);
    }

    [WebMethod(true)]
	public static string SaveView(string gridView, string gridViewName, int processView
		, string rollupGroup
		, string parentColumns, string selectedParentColumnOrder
		, string childColumns, string selectedChildColumnOrder
		, string sortType)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "error", "" } };
		bool saved = false;
		string errorMsg = string.Empty;

		try
		{
			int gridViewID = 0;
			int.TryParse(gridView, out gridViewID);
			GridView gv = new GridView();

			gv.GridNameID = (int)WTSGridName.Workload_Crosswalk;
			gv.ID = gridViewID;
			gv.Name = gridViewName;
			gv.UserID = (processView == 1 ? 0 : UserManagement.GetUserId_FromUsername());
			gv.Tier1Columns = parentColumns;
			gv.Tier1ColumnOrder = selectedParentColumnOrder;
			gv.Tier1RollupGroup = rollupGroup;
			gv.Tier2Columns = childColumns;
			gv.Tier2ColumnOrder = selectedChildColumnOrder;
			gv.Tier2SortOrder = sortType;

			saved = gv.Save(out errorMsg);

			if (saved)
			{
				HttpContext.Current.Session["Crosswalk_GridView"] = gridViewName;
				HttpContext.Current.Session["Crosswalk_RollupGroup"] = rollupGroup;
				HttpContext.Current.Session["Crosswalk_DefaultSortType"] = sortType;
            }
        }
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod(true)]
	public static string DeleteView(string gridView)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "error", "" } };
		bool saved = false, exists = false;
		string errorMsg = string.Empty;

		try
		{
			int gridViewID = 0;
			int.TryParse(gridView, out gridViewID);

			if (gridViewID > 0)
			{
				GridView gv = new GridView(gridViewID);
				saved = gv.Delete(out exists, out errorMsg);
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
}