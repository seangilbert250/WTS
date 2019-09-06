﻿using System;
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
using System.Text.RegularExpressions;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;
using System.Linq;
using DotNetOpenAuth.Messaging;

public partial class WorkItemGrid : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    //protected DataColumnCollection DCC_SR;
    //protected DataColumnCollection DCC_WorkItem;
    protected GridCols columnData = new GridCols();

    #region Filters

    protected int MyView = 1; //Hotlist, Developer view, Analyst view
    protected int ItemType = 0; //All, SRs, Workload Items

    protected int SystemID = 0;

    #endregion Filters

    protected bool _myData = true;
    protected int _showArchived = 0;
    protected bool ShowClosed = false;
    protected bool ShowBacklog = false;
    protected bool ShowArchived = false;

    protected bool _refreshData = false;
    protected int _pageIndex = 0;
    protected bool _export = false;
    protected string[] SelectedStatuses = { };
    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;
    protected bool ColumnOrderChanged = false;

    protected bool CanViewRequest = false;
    protected bool CanViewSR = false;
    protected bool CanViewWorkItem = false;

    protected bool CanEditRequest = false;
    protected bool CanEditSR = false;
    protected bool CanEditWorkItem = false;
    protected string itemList = String.Empty;

    protected string ddlChanged = "no";
    protected string ddlAssignedChanged = "no";
    protected string parentAffilitatedID = string.Empty;
    protected string[] SelectedAssigned;
    protected bool QFBusinessReview = false;
    private DataTable dtAssigned;

    int userID = UserManagement.GetUserId_FromUsername();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.CanEditRequest = UserManagement.UserCanEdit(WTSModuleOption.WorkRequest);
        this.CanEditSR = UserManagement.UserCanEdit(WTSModuleOption.SustainmentRequest);
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewRequest = CanEditRequest || UserManagement.UserCanView(WTSModuleOption.WorkRequest);
        this.CanViewSR = CanEditSR || UserManagement.UserCanView(WTSModuleOption.SustainmentRequest);
        this.CanViewWorkItem = CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

        readQueryString();
        //default View is now either default or comes from user preferences.
        setDefaultView();

        init();
        loadMenus();

        if (DCC != null)
        {
            this.DCC.Clear();
        }
        loadGridData();




        //loadMetricsGrids();
    }

    private void setDefaultView()
    {
        // 11626 - 2 > Use saved preferences:
        if (Session["defaultWorkloadGrid"] != null && !string.IsNullOrWhiteSpace(Session["defaultWorkloadGrid"].ToString()) && ddlChanged != "yes")
        {
            switch (Session["defaultWorkloadGrid"].ToString())
            {
                case "1":
                    this.MyView = 2;  //  SR
                    break;
                //case "2":
                //    this.MyView = 0;  // Work Request
                //    break;
                case "3":
                    this.MyView = 1;  // Workload
                    break;
                default:
                    this.MyView = 1;  // Workload
                    break;
            }
        }
        else
        {
            if (Request.QueryString["View"] != null
                && !string.IsNullOrWhiteSpace(Request.QueryString["View"].ToString()))
            {
                int.TryParse(Server.UrlDecode(Request.QueryString["View"].ToString()), out this.MyView);
            }
            else
            {
                this.MyView = 1;  // Workload - Switched to Workload 4-4-2017
                //this.MyView = 0;  // Work Request
            }
        }
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

        if (Request.QueryString["ddlChanged"] != null)
        {
            ddlChanged = Request.QueryString["ddlChanged"].ToString();
        }

        if (Request.QueryString["ddlAssignedChanged"] != null)
        {
            ddlAssignedChanged = Request.QueryString["ddlAssignedChanged"].ToString();
        }

        if (Request.QueryString["PageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["PageIndex"]))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["PageIndex"]).Trim(), out _pageIndex);
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
            if (Request.QueryString["ShowArchived"].Trim() == "1" || Request.QueryString["ShowArchived"].Trim().ToUpper() == "TRUE")
            {
                ShowArchived = true;
            }
        }

        if (Request.QueryString["SelectedAssigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAssigned"].ToString()))
        {
            SelectedAssigned = Server.UrlDecode(Request.QueryString["SelectedAssigned"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Session["Assigned"] = SelectedAssigned;
        }
        if (Request.QueryString["filterSelectedAssigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["filterSelectedAssigned"].ToString()))
        {
            Session["Assigned"] = null;
            SelectedAssigned = Server.UrlDecode(Request.QueryString["filterSelectedAssigned"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            _myData = false;
        }

        if (Request.QueryString["ItemType"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ItemType"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ItemType"].ToString()), out this.ItemType);
        }
        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }
        if (Request.QueryString["columnOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["columnOrder"].ToString()))
        {
            ColumnOrder = Server.UrlDecode(Request.QueryString["columnOrder"]);
        }
        if (Request.QueryString["columnOrderChanged"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["columnOrderChanged"].ToString()))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["columnOrderChanged"]), out this.ColumnOrderChanged);
        }
        if (Request.QueryString["Export"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);
        }
        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"].ToString()))
        {
            SelectedStatuses = Server.UrlDecode(Request.QueryString["SelectedStatuses"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["BusinessReview"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["BusinessReview"]))
        {
            bool br = false;
            bool.TryParse(Request.QueryString["BusinessReview"].ToString(), out br);
            this.QFBusinessReview = br;
        }
    }

    private void init()
    {
        grdWorkload.GridHeaderRowDataBound += grdWorkload_GridHeaderRowDataBound;
        grdWorkload.GridRowDataBound += grdWorkload_GridRowDataBound;
        grdWorkload.GridPageIndexChanging += grdWorkload_GridPageIndexChanging;

        ListItem item = null;
        item = ddlView.Items.FindByValue(MyView.ToString());
        if (item != null)
        {
            item.Selected = true;
        }
        LoadQF();
        //setSelectedStatuses();
        loadQF_Affiliated();
    }


    #region Data

    private void LoadQF()
    {
        DataTable dtStatus = MasterData.StatusList_Get();
        HtmlSelect ddlStatus = (HtmlSelect)Page.Master.FindControl("ms_Item0");
        HtmlInputCheckBox BusinessReviewCheckBoxCtrl = (HtmlInputCheckBox)Page.Master.FindControl("chk_Item12");
        HtmlInputCheckBox BacklogCheckBoxCtrl = (HtmlInputCheckBox)Page.Master.FindControl("chk_Item13");
        HtmlInputCheckBox ClosedCheckBoxCtrl = (HtmlInputCheckBox)Page.Master.FindControl("chk_Item14");
        HtmlInputCheckBox ArchivedCheckBoxCtrl = (HtmlInputCheckBox)Page.Master.FindControl("chk_Item15");
        string selected = "";

        Label lblStatus = (Label)Page.Master.FindControl("lblms_Item0");
        lblStatus.Text = "Status: ";
        lblStatus.Style["width"] = "150px";


        //Set the SelectedStatuses from the QueryString parameters(If they exist). (If they dont exist) Set the SelectedStatuses from the selected items in the MasterData menu item
        if (dtStatus != null)
        {
            ddlStatus.Items.Clear();
            dtStatus.DefaultView.RowFilter = "StatusType = 'Work'";
            dtStatus = dtStatus.DefaultView.ToTable(true, new string[] { "StatusID", "Status" });

            foreach (DataRow dr in dtStatus.Rows)
            {
                ListItem li = new ListItem(dr["Status"].ToString(), dr["StatusID"].ToString());
                li.Selected = (SelectedStatuses.Count() == 0 || SelectedStatuses.Contains(dr["StatusID"].ToString()));

                if (SelectedStatuses.Count() == 0 && (li.Text == "Approved/Closed" || li.Text == "Closed" || li.Text == "On Hold")) li.Selected = false;
                if (li.Selected)
                {
                    selected += li.Value + ",";
                }

                ddlStatus.Items.Add(li);
            }
            this.SelectedStatuses = Server.UrlDecode(selected.Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        Label lblPageSize = (Label)Page.Master.FindControl("lblItem5");
        lblPageSize.Text = "Page Size: ";
        lblPageSize.Style["width"] = "50px";

        Label BusinessReviewCheckBoxLabelCtrl = (Label)Page.Master.FindControl("lblms_Item12");
        BusinessReviewCheckBoxLabelCtrl.Text = "Business Review Requested: ";
        BusinessReviewCheckBoxLabelCtrl.Style["width"] = "250px";
        BusinessReviewCheckBoxCtrl.Checked = QFBusinessReview;

        Label BacklogCheckBoxLabelCtrl = (Label)Page.Master.FindControl("lblms_Item13");
        BacklogCheckBoxLabelCtrl.Text = "Show Backlog: ";
        BacklogCheckBoxLabelCtrl.Style["width"] = "250px";
        BacklogCheckBoxCtrl.Checked = ShowBacklog;

        Label ClosedCheckBoxLabelCtrl = (Label)Page.Master.FindControl("lblms_Item14");
        ClosedCheckBoxLabelCtrl.Text = "Show Closed: ";
        ClosedCheckBoxLabelCtrl.Style["width"] = "250px";
        ClosedCheckBoxCtrl.Checked = ShowClosed;

        Label ArchivedCheckBoxLabelCtrl = (Label)Page.Master.FindControl("lblms_Item15");
        ArchivedCheckBoxLabelCtrl.Text = "Show Archived: ";
        ArchivedCheckBoxLabelCtrl.Style["width"] = "250px";
        ArchivedCheckBoxCtrl.Checked = ShowArchived;
    }

    private void loadQF_Affiliated()
    {
        List<string> Affiliated = new List<string>();
        bool blnBacklog = false;

        if (SelectedAssigned != null && SelectedAssigned.Length > 0)
        {
            foreach (string s in SelectedAssigned)
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

            int userID = UserManagement.GetUserId_FromUsername();

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
                    if (ddlChanged == "no" && _myData)
                    {
                        if (dr["WTS_RESOURCEID"].ToString() == userID.ToString())
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

    private void loadGridData()
    {
        DataTable dt = null;
        switch (MyView)
        {
            //case 0:
            //    dt = loadWorkRequestItems();
            //    setItemList(dt, "WORKREQUESTID");
            //    break;
            case 1:
                dt = loadWorkloadItems();
                setItemList(dt, "ItemID");
                break;
            case 2:
                dt = loadSRItems();
                setItemList(dt, "ItemID");
                break;
            case 3:
                dt = loadOverview();
                setItemList(dt, "ItemID");
                break;
            default:
                dt = loadWorkloadItems();
                setItemList(dt, "ItemID");
                break;
        }

    }

    private void loadMetricsGrids()
    {
        //DataTable dtWI = Workload.WorkItem_Metrics_Get();
        ////foreach (RowNotInTableException r in dtWI.Rows)
        ////{
        ////    //totalCount += r("Count");
        ////}
        //gridWIStatus.DataSource = dtWI;
        //gridWIStatus.DataBind();

        //DataTable dtWIT = Workload.WorkItemTask_Metrics_Get();
        //gridWITStatus.DataSource = dtWIT;
        //gridWITStatus.DataBind();

        //// Priority
        //DataTable dtWIPriority = Workload.WorkItem_Priority_Get();
        //dtWIPriority.Columns.Remove("SORT_ORDER");
        //gridWIPriority.DataSource = dtWIPriority;
        //gridWIPriority.DataBind();

        //DataTable dtWITPriority = Workload.WorkItemTask_Priority_Get();
        //dtWITPriority.Columns.Remove("SORT_ORDER");
        //gridWITPriority.DataSource = dtWITPriority;
        //gridWITPriority.DataBind();

    }
    private DataTable loadWorkRequestItems()
    {
        DataTable dt = null;

        try
        {
            if (_refreshData || Session["dtWorkItemRequest"] == null)
            {
                dt = WorkRequest.WorkRequestList_Get(typeID: 0, showArchived: _showArchived, requestGroupID: 0, myData: _myData);
                HttpContext.Current.Session["dtWorkItemRequest"] = dt;
                HttpContext.Current.Session["ExcelReport"] = "Workrequest";
            }
            else
            {
                dt = (DataTable)HttpContext.Current.Session["dtWorkItemRequest"];
            }

            if (dt != null)
            {
                this.DCC = dt.Columns;
                spanRowCount.InnerText = dt.Rows.Count.ToString();

                InitializeColumnData_WorkRequest(ref dt);
                dt.AcceptChanges();

                if (_export)
                {
                    ExportExcel(dt);
                }
                else
                {
                    grdWorkload.DataSource = dt;
                    grdWorkload.DataBind();
                    if (_pageIndex > 0)
                    {
                        grdWorkload.PageIndex = _pageIndex;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            dt = null;
        }
        return dt;
    }

    private DataTable loadWorkloadItems(bool bind = true)
    {
        HtmlSelect ddlStatus = (HtmlSelect)Page.Master.FindControl("ms_Item0");
        HtmlSelect ddlAffiliated = (HtmlSelect)Page.Master.FindControl("ms_Item1");
        bool Affiliated = true;

        IList<string> listAssigned = new List<string>();

        if (ddlAffiliated != null && ddlAffiliated.Items.Count > 0)
        {
            foreach (ListItem li in ddlAffiliated.Items)
            {
                if (li.Selected)
                {
                    listAssigned.Add(li.Value);
                }
            }
        }

        IList<string> listStatuses = new List<string>();

        if (ddlStatus != null && ddlStatus.Items.Count > 0)
        {
            foreach (ListItem li in ddlStatus.Items)
            {
                if (li.Selected)
                {
                    listStatuses.Add(li.Value);
                }
            }
        }

        DataTable dtWork = null;

        try
        {
            if (_myData)
            {
                parentAffilitatedID = userID.ToString();
            }
            // Sorting priority is
            // 1) CASE UPPER(s.[STATUS]) WHEN 'TRAVEL' THEN 0 WHEN 'REQUESTED' THEN 1 WHEN 'INFO REQUESTED' THEN 2 WHEN 'ON HOLD' THEN 4 ELSE 3 END AS Status_Sort
            // 2) Then Rank, depending on what is sent in here as rankSortType, then Primary Tech. Rank.
            if (_refreshData || Session["dtWorkItem"] == null)
            {
                dtWork = WorkloadItem.WorkItemList_Get_QF(workRequestID: 0, showArchived: _showArchived, columnListOnly: 0, myData: _myData, rankSortType: "Tech", showClosed: (ShowClosed ? 1 : 0), ShowBacklog: ShowBacklog, SelectedStatuses: String.Join(",", listStatuses), SelectedAssigned: String.Join(",", listAssigned), qfBusinessReview: QFBusinessReview, Affiliated: Affiliated, ParentAffilitatedID: parentAffilitatedID);
                if (dtWork == null)
                {
                    // If DataTable is null, selects (drop downs) disappear over the empty grid.  Works on crosswalk page.
                    // WORKITEMLIST_GET_0 will create a header and then the drop downs work.
                    dtWork = WorkloadItem.WORKITEMLIST_GET_0();
                }
            }
            else
            {
                dtWork = (DataTable)HttpContext.Current.Session["dtWorkItem"];
            }

            if (dtWork != null)
            {
                HttpContext.Current.Session["dtWorkItem"] = dtWork;
                HttpContext.Current.Session["ExcelReport"] = "Workload";
            }

            //// Metrics:
            //if (_refreshData)
            //{
            //    DataSet dsWIMetrics = WorkloadItem.GetWorkItemMetrics(SelectedStatuses: String.Join(",", listStatuses), SelectedAssigned: String.Join(",", listAssigned));

            //    DataTable dtWorkItemStatus = dsWIMetrics.Tables["WorkItemStatus"];
            //    DataTable dtWorkItemTaskStatus = dsWIMetrics.Tables["WorkItemTaskStatus"];
            //    DataTable dtWorkItemPriority = dsWIMetrics.Tables["WorkItemPriority"];
            //    DataTable dtWorkItemTaskPriority = dsWIMetrics.Tables["WorkItemTaskPriority"];

            //    HttpContext.Current.Session["WorkItemStatus"] = dtWorkItemStatus;
            //    HttpContext.Current.Session["WorkItemTaskStatus"] = dtWorkItemTaskStatus;
            //    HttpContext.Current.Session["WorkItemPriority"] = dtWorkItemPriority;
            //    HttpContext.Current.Session["WorkItemTaskPriority"] = dtWorkItemTaskPriority;
            //}
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            dtWork = null;
        }

        int subTaskCount = 0;
        if (dtWork != null)
        {
            this.DCC = dtWork.Columns;

            foreach (DataRow r in dtWork.Rows)
            {
                subTaskCount += (int)r["Task_Count"];
            }

            spanRowCount.InnerText = dtWork.Rows.Count.ToString() + ", " + subTaskCount;

            InitializeColumnData_WorkItem(ref dtWork);
            dtWork.AcceptChanges();
        }

        if (_export)
        {
            ExportExcel(dtWork);
        }
        else
        {
            if (_pageIndex > 0)
            {
                grdWorkload.PageIndex = _pageIndex;
            }

            grdWorkload.DataSource = dtWork;
            if (bind) grdWorkload.DataBind();
        }
        return dtWork;
    }

    private DataTable loadSRItems()
    {
        DataTable dtSR = null;

        try
        {
            if (_refreshData || Session["dtSR"] == null)
            {
                dtSR = SRItem.SRView_Get(showArchived: _showArchived, showClosed: (ShowClosed ? 1 : 0), ShowBacklog: ShowBacklog, myData: _myData);
            }
            else
            {
                dtSR = (DataTable)HttpContext.Current.Session["dtSR"];
            }
            if (dtSR != null)
            {
                HttpContext.Current.Session["dtSR"] = dtSR;
                HttpContext.Current.Session["ExcelReport"] = "SR";
                dtSR.DefaultView.RowFilter = "STATUSID IN (" + String.Join(", ", this.SelectedStatuses) + ")";
                dtSR = dtSR.DefaultView.ToTable();

                this.DCC = dtSR.Columns;
                spanRowCount.InnerText = dtSR.Rows.Count.ToString();

                InitializeColumnData_SR(ref dtSR);
                dtSR.AcceptChanges();

                if (_export)
                {
                    ExportExcel(dtSR);
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            dtSR = null;
        }

        grdWorkload.DataSource = dtSR;
        grdWorkload.DataBind();
        if (_pageIndex > 0)
        {
            grdWorkload.PageIndex = _pageIndex;
        }
        return dtSR;
    }

    private DataTable loadOverview()
    {
        HtmlSelect ddlStatus = (HtmlSelect)Page.Master.FindControl("ms_Item0");
        HtmlSelect ddlAffiliated = (HtmlSelect)Page.Master.FindControl("ms_Item1");
        IList<string> listAssigned = new List<string>();

        if (ddlAffiliated != null && ddlAffiliated.Items.Count > 0)
        {
            foreach (ListItem li in ddlAffiliated.Items)
            {
                if (li.Selected)
                {
                    listAssigned.Add(li.Value);
                }
            }
        }

        IList<string> listStatuses = new List<string>();

        if (ddlStatus != null && ddlStatus.Items.Count > 0)
        {
            foreach (ListItem li in ddlStatus.Items)
            {
                if (li.Selected)
                {
                    listStatuses.Add(li.Value);
                }
            }
        }

        DataTable dtSR = null;

        try
        {
            dtSR = WorkloadItem.WorkItem_GetOverview(String.Join(",", listAssigned), String.Join(",", listStatuses));
            if (dtSR != null)
            {
                HttpContext.Current.Session["dtOverview"] = dtSR;
                HttpContext.Current.Session["ExcelReport"] = "Overview";
                dtSR = dtSR.DefaultView.ToTable();

                this.DCC = dtSR.Columns;
                spanRowCount.InnerText = dtSR.Rows.Count.ToString();

                InitializeColumnData_Overview(ref dtSR);
                dtSR.AcceptChanges();

                //formatColumnDisplay_Overview()
                if (_export)
                {
                    ExportExcel(dtSR);
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            dtSR = null;
        }

        grdWorkload.DataSource = dtSR;
        grdWorkload.DataBind();
        if (_pageIndex > 0)
        {
            grdWorkload.PageIndex = _pageIndex;
        }

        return dtSR;
    }

    private void setItemList(DataTable dt, string listItemID)
    {
        if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains(listItemID) != false)
        {
            this.itemList = dt.AsEnumerable()
                .Select(row => row[listItemID].ToString())
                .Aggregate((s1, s2) => String.Concat(s1, "," + s2));
        }
    }

    #region Format Columns

    protected void InitializeColumnData_WorkRequest(ref DataTable dt)
    {
        try
        {
            string displayName = string.Empty, groupName = string.Empty;
            bool blnVisible = false, blnSortable = false, blnOrderable = false;

            foreach (DataColumn gridColumn in dt.Columns)
            {
                displayName = gridColumn.ColumnName;
                blnVisible = false;
                blnSortable = false;
                blnOrderable = false;
                groupName = string.Empty;

                switch (gridColumn.ColumnName)
                {
                    case "X":
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                    case "ROW_ID":
                        displayName = "NO.";
                        blnVisible = true;
                        break;
                    case "WORKREQUESTID":
                        displayName = "Request Number";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "CONTRACT":
                        displayName = "Contract";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "ORGANIZATION":
                        displayName = "Department /<br/>Organization";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Request_Type":
                        displayName = "Request Type";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Work_Scope":
                        displayName = "Scope";
                        blnVisible = true;
                        blnSortable = false;
                        blnOrderable = false;
                        break;
                    case "TITLE":
                        displayName = "Title";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "SR_Count":
                        displayName = "SR Count";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "SR_Date":
                        displayName = "SR Created Date";
                        blnVisible = false;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "WorkItem_Count":
                        displayName = "Work Item Count";
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = false;
                        break;
                    case "WorkItem_Date":
                        displayName = "Work Item Created Date";
                        blnVisible = false;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "PRIORITY":
                        displayName = "Operations<br/>Priority";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "SME":
                        displayName = "SME";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Lead_Tech_Writer":
                        displayName = "Lead Tech<br/>Writer";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Lead_Resource":
                        displayName = "Lead Resource";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "My_Role":
                        displayName = "My Role";
                        blnVisible = true;
                        blnSortable = false;
                        blnOrderable = false;
                        break;
                    case "Submitted_By":
                        displayName = "Submitted By";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                }

                columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
                columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
            }

            //Initialize the columnData
            columnData.Initialize(ref dt, ";", "~", "|");

            //Get sortable columns and default column order
            SortableColumns = columnData.SortableColumnsToString();
            DefaultColumnOrder = columnData.DefaultColumnOrderToString();
            //Sort and Reorder Columns
            columnData.ReorderDataTable(ref dt, ColumnOrder);

            SortOrder = Workload.GetSortValuesFromDB(1, "WORKITEMGRID.ASPX");

            columnData.SortDataTable(ref dt, SortOrder);
            SelectedColumnOrder = columnData.CurrentColumnOrderToString();

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    protected void InitializeColumnData_Overview(ref DataTable dt)
    {

        try
        {
            string displayName = string.Empty, groupName = string.Empty;
            bool blnVisible = false, blnSortable = false, blnOrderable = false;

            foreach (DataColumn gridColumn in dt.Columns)
            {
                displayName = gridColumn.ColumnName;
                blnVisible = false;
                blnSortable = false;
                blnOrderable = false;
                groupName = string.Empty;

                switch (gridColumn.ColumnName)
                {
                    case "Assigned To":
                        displayName = "Assigned To";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Full ID":
                        displayName = "Full ID";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Title":
                        displayName = "Title";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Task Title":
                        displayName = "Task Title";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Sub Task Title":
                        displayName = "Subtask Title";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "STATUS":
                        displayName = "Status";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "PRIORITY":
                        displayName = "Priority";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "% Complete":
                        displayName = "% Complete";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Actual Start":
                        displayName = "Actual Start";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;

                    case "WORKITEMID":
                        displayName = "WORKITEMID";
                        blnVisible = false;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "SubTaskID":
                        displayName = "SubtaskID";
                        blnVisible = false;
                        blnSortable = true;
                        blnOrderable = false;
                        break;

                }

                columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
                columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
            }
            //Initialize the columnData
            //columnData.Initialize(ref dt, ";", "~", "|");

            ////Get sortable columns and default column order
            //SortableColumns = columnData.SortableColumnsToString();
            //DefaultColumnOrder = columnData.DefaultColumnOrderToString();
            ////Sort and Reorder Columns
            //columnData.ReorderDataTable(ref dt, ColumnOrder);

            //SortOrder = Workload.GetSortValuesFromDB(1, "WORKITEMGRID.ASPX");

            //columnData.SortDataTable(ref dt, SortOrder);
            //SelectedColumnOrder = columnData.CurrentColumnOrderToString();

        }
        catch (Exception)
        {
            //throw;
        }

    }

    protected void InitializeColumnData_SR(ref DataTable dt)
    {
        try
        {
            string displayName = string.Empty, groupName = string.Empty;
            bool blnVisible = false, blnSortable = false, blnOrderable = false;

            foreach (DataColumn gridColumn in dt.Columns)
            {
                displayName = gridColumn.ColumnName;
                blnVisible = false;
                blnSortable = false;
                blnOrderable = false;
                groupName = string.Empty;

                switch (gridColumn.ColumnName)
                {
                    case "Websystem":
                        displayName = "System(Task)";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Priority":
                        displayName = "Priority";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "ItemID":
                        displayName = "Work Task #";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "SubTaskID":
                        displayName = "Subtask #";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "SR_Number":
                        displayName = "SR Number";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Primary Bus. Res.":
                        displayName = "Primary Bus. Res.";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Primary Tech. Res.":
                        displayName = "Primary Tech. Res.";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Status":
                        displayName = "Status";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "% Complete":
                        displayName = "% Complete";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Title":
                        displayName = "Title";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Description":
                        displayName = "Description";
                        blnVisible = true;
                        blnSortable = false;
                        blnOrderable = false;
                        break;
                    case "Sub Task Title":
                        displayName = "Subtask Title";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Created":
                        displayName = "Created";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Days Open":
                        displayName = "Days Open";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "Last Updated":
                        displayName = "Last Updated";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                }

                columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
                columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
            }

            //Initialize the columnData
            columnData.Initialize(ref dt, ";", "~", "|");

            //Get sortable columns and default column order
            SortableColumns = columnData.SortableColumnsToString();
            DefaultColumnOrder = columnData.DefaultColumnOrderToString();
            //Sort and Reorder Columns
            columnData.ReorderDataTable(ref dt, ColumnOrder);

            SortOrder = Workload.GetSortValuesFromDB(1, "WORKITEMGRID.ASPX");

            columnData.SortDataTable(ref dt, SortOrder);
            SelectedColumnOrder = columnData.CurrentColumnOrderToString();

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    protected void InitializeColumnData_WorkItem(ref DataTable dt)
    {
        try
        {
            string displayName = string.Empty, groupName = string.Empty;
            bool blnVisible = false, isViewable = false, blnSortable = false, blnOrderable = false;

            foreach (DataColumn gridColumn in dt.Columns)
            {
                displayName = gridColumn.ColumnName;
                blnVisible = false;
                blnSortable = false;
                blnOrderable = true;
                isViewable = false;
                groupName = string.Empty;

                switch (gridColumn.ColumnName)
                {
                    case "WORKREQUESTID":
                        displayName = "Subtasks";
                        blnVisible = true;
                        blnSortable = false;
                        blnOrderable = false;
                        break;
                    case "ItemID":
                        displayName = "Primary Task";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = false;
                        break;
                    case "WorkType":
                        displayName = "Resource Group";
                        blnVisible = false;
                        blnSortable = true;
                        isViewable = true;
                        break;
                    case "WORKITEMTYPE":
                        displayName = "Work Activity";
                        blnVisible = false;
                        blnSortable = true;
                        isViewable = true;
                        break;
                    case "Task_Count":
                        displayName = "Task Count";
                        blnVisible = false;
                        blnSortable = true;
                        blnOrderable = false;
                        isViewable = false;
                        break;
                    case "Websystem":
                        displayName = "System(Task)";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "TITLE":
                        displayName = "Title";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ALLOCATION":
                        displayName = "Allocation Assignment";
                        blnVisible = false;
                        blnSortable = true;
                        isViewable = true;
                        break;
                    case "ProductionStatus":
                        displayName = "Production Status";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Version":
                        displayName = "Version";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "SR_Number":
                        displayName = "SR Number";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "PRIORITY":
                        displayName = "Priority";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Assigned":
                        displayName = "Assigned";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Primary_Analyst":
                        displayName = "Primary Analyst";
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "Primary_Developer":
                        displayName = "Primary Resource";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "PrimaryBusinessResource":
                        displayName = "Primary Bus. Resource";
                        blnVisible = false;
                        blnSortable = true;
                        break;

                    case "SecondaryBusinessResource":
                        displayName = "Secondary Bus. Resource";
                        blnVisible = false;
                        blnSortable = true;
                        break;

                    case "SECONDARYRESOURCE":
                        displayName = "Secondary Tech. Resource";
                        blnVisible = false;
                        blnSortable = true;
                        break;
                    case "AssignedToRank":
                        displayName = "Assigned To Rank";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "PrimaryBusinessRank":
                        displayName = "Customer Rank";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "SecondaryResourceRank":
                        displayName = "Secondary Tech. Rank";
                        blnVisible = false;
                        blnSortable = true;
                        break;
                    case "CREATEDDATE":
                        displayName = "Created";
                        blnVisible = false;
                        blnSortable = true;
                        break;
                    case "SubmittedByID":
                        displayName = "SubmittedByID";
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "SubmittedBy":
                        displayName = "Submitted";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "STATUS":
                        displayName = "Status";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "IVTRequired":
                        displayName = "Requires IVT";
                        blnVisible = false;
                        blnSortable = true;
                        break;
                    case "NEEDDATE":
                        displayName = "Date Needed";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Progress":
                        displayName = "Progress";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "INPROGRESSDATE":
                        displayName = "In Progress Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DEPLOYEDDATE":
                        displayName = "Deployed Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "READYFORREVIEWDATE":
                        displayName = "Ready for Review Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "CLOSEDDATE":
                        displayName = "Closed Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                }
                isViewable = blnVisible;

                columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
                columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
                columnData.Columns.Item(columnData.Columns.Count - 1).Viewable = isViewable;
            }

            //Initialize the columnData
            columnData.Initialize(ref dt, ";", "~", "|");

            // SCB 2-21-2017 - Leave the order to the stored proc.
            //Get sortable columns and default column order
            SortableColumns = columnData.SortableColumnsToString();
            DefaultColumnOrder = columnData.DefaultColumnOrderToString();
            //Sort and Reorder Columns
            columnData.ReorderDataTable(ref dt, ColumnOrder);

            SortOrder = Workload.GetSortValuesFromDB(1, "WORKITEMGRID.ASPX");

            columnData.SortDataTable(ref dt, SortOrder);
            SelectedColumnOrder = columnData.CurrentColumnOrderToString();

            //         this.columnData.SortDataTable(ref dt, this.SortOrder);

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    #endregion Format Columns

    #endregion Data

    #region Grid

    void grdWorkload_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;

        formatColumnDisplay(ref row);
        formatHeader(ref row);
    }

    void grdWorkload_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);

        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);
        formatDataRow(ref row);
    }

    void grdWorkload_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdWorkload.PageIndex = e.NewPageIndex;
        switch (this.MyView)
        {
            //case 0:
            //	if (HttpContext.Current.Session["dtWorkItemRequest"] == null)
            //	{
            //		loadWorkRequestItems();
            //	}
            //	else
            //	{
            //		grdWorkload.DataSource = (DataTable)HttpContext.Current.Session["dtWorkItemRequest"];
            //	}
            //	break;
            case 1:
                loadWorkloadItems(false);
                break;
            case 2:
                if (HttpContext.Current.Session["dtSR"] == null)
                {
                    loadWorkRequestItems();
                }
                else
                {
                    grdWorkload.DataSource = (DataTable)HttpContext.Current.Session["dtSR"];
                }
                break;
            case 3:
                if (HttpContext.Current.Session["dtOverview"] == null)
                {
                    loadOverview();
                }
                else
                {
                    grdWorkload.DataSource = (DataTable)HttpContext.Current.Session["dtOverview"];
                }
                break;
            default:
                loadWorkloadItems(false);
                break;
        }
    }


    void formatColumnDisplay(ref GridViewRow row)
    {
        switch (this.MyView)
        {
            //case 0:
            //	formatColumnDisplay_WorkRequest(ref row);
            //	break;
            case 1:
                formatColumnDisplay_WorkItem(ref row);
                break;
            case 2:
                formatColumnDisplay_SR(ref row);
                break;
            case 3:
                formatColumnDisplay_Overview(ref row);
                break;
            default:
                formatColumnDisplay_WorkItem(ref row);
                break;
        }
    }
    void formatColumnDisplay_WorkRequest(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            row.Cells[i].Style["text-align"] = "left";
            if (i != DCC.IndexOf("X") && i != DCC.IndexOf("WORKREQUESTID") && i != DCC.IndexOf("SR_Count") && i != DCC.IndexOf("Priority"))
            {
                row.Cells[i].Style["padding-left"] = "5px";
            }
        }

        //more column formatting
        row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("WORKREQUESTID")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("WORKREQUESTID")].Style["width"] = "63px";
        row.Cells[DCC.IndexOf("Contract")].Style["width"] = "74px";
        row.Cells[DCC.IndexOf("Organization")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("Request_Type")].Style["width"] = "53px";
        row.Cells[DCC.IndexOf("Work_Scope")].Style["width"] = "85px";
        row.Cells[DCC.IndexOf("Effort")].Style["width"] = "85px";
        //row.Cells[DCC.IndexOf("Title")].Style["width"] = "75px";
        //row.Cells[DCC.IndexOf("Description")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("SR_Count")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("SR_Count")].Style["width"] = "40px";
        row.Cells[DCC.IndexOf("Priority")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Priority")].Style["width"] = "65px";
        row.Cells[DCC.IndexOf("SME")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("Lead_Tech_Writer")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("Lead_Resource")].Style["width"] = "75px";
        //row.Cells[DCC.IndexOf("My_Role")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("Submitted_By")].Style["width"] = "75px";
    }
    void formatColumnDisplay_SR(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            row.Cells[i].Style["text-align"] = "left";
            if (i != DCC.IndexOf("WORKREQUESTID") && i != DCC.IndexOf("ItemID") && i != DCC.IndexOf("Production") && i != DCC.IndexOf("Progress"))
            {
                row.Cells[i].Style["padding-left"] = "5px";
            }
        }

        //more column formatting
        row.Cells[DCC.IndexOf("WEBSYSTEM")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("WEBSYSTEM")].Style["padding-left"] = "0px";
        row.Cells[DCC.IndexOf("WEBSYSTEM")].Style["width"] = "25px";
        row.Cells[DCC.IndexOf("ItemID")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("ItemID")].Style["padding-left"] = "0px";
        row.Cells[DCC.IndexOf("ItemID")].Style["width"] = "46px";
        row.Cells[DCC.IndexOf("SR_Number")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("SR_Number")].Style["padding-left"] = "0px";
        row.Cells[DCC.IndexOf("SR_Number")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Websystem")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("Created")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("Last Updated")].Style["width"] = "75px";
        //row.Cells[DCC.IndexOf("Priority")].Style["width"] = "50px";

        row.Cells[DCC.IndexOf("Status")].Style["width"] = "55px";
        //row.Cells[DCC.IndexOf("ITI_POC")].Style["width"] = "90px";
        //row.Cells[DCC.IndexOf("Release")].Style["width"] = "60px";
        //row.Cells[DCC.IndexOf("Last_Reply")].Style["width"] = "75px";
        //row.Cells[DCC.IndexOf("Resolved_Date")].Style["width"] = "75px";
    }
    void formatColumnDisplay_WorkItem(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            row.Cells[i].Style["text-align"] = "left";
            if ((DCC.Contains("WORKREQUESTID") && i == DCC.IndexOf("WORKREQUESTID"))
                || (DCC.Contains("ItemID") && i == DCC.IndexOf("ItemID"))
                || (DCC.Contains("IVTRequired") && i == DCC.IndexOf("IVTRequired"))
                || (DCC.Contains("NEEDDATE") && i == DCC.IndexOf("NEEDDATE"))
                || (DCC.Contains("RESOURCEPRIORITYRANK") && i == DCC.IndexOf("RESOURCEPRIORITYRANK"))
                || (DCC.Contains("PrimaryBusinessRank") && i == DCC.IndexOf("PrimaryBusinessRank"))
                || (DCC.Contains("SecondaryResourceRank") && i == DCC.IndexOf("SecondaryResourceRank"))
                || (DCC.Contains("Production") && i == DCC.IndexOf("Production"))
                || (DCC.Contains("SR_Number") && i == DCC.IndexOf("SR_Number"))
                || (DCC.Contains("Progress") && i == DCC.IndexOf("Progress")))
            {
                row.Cells[i].Style["text-align"] = "center";
            }
            else
            {
                row.Cells[i].Style["padding-left"] = "5px";
            }
        }

        //more column formatting
        if (DCC.Contains("TITLE"))
        {
            row.Cells[DCC.IndexOf("TITLE")].Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("TITLE")].Text.Replace("&nbsp;", "").Trim()));
        }
        if (DCC.Contains("WORKREQUESTID"))
        {
            row.Cells[DCC.IndexOf("WORKREQUESTID")].Style["text-align"] = "right";
            row.Cells[DCC.IndexOf("WORKREQUESTID")].Style["width"] = "32px";
        }
        if (DCC.Contains("ItemID"))
        {
            row.Cells[DCC.IndexOf("ItemID")].Style["width"] = "46px";
        }
        if (DCC.Contains("WORKITEMTYPE"))
        {
            row.Cells[DCC.IndexOf("WORKITEMTYPE")].Style["width"] = "80px";
        }
        if (DCC.Contains("WorkType"))
        {
            row.Cells[DCC.IndexOf("WorkType")].Style["width"] = "70px";
        }
        if (DCC.Contains("Websystem"))
        {
            row.Cells[DCC.IndexOf("Websystem")].Style["width"] = "75px";
        }
        if (DCC.Contains("Allocation"))
        {
            row.Cells[DCC.IndexOf("Allocation")].Style["width"] = "150px";
        }
        if (DCC.Contains("RESOURCEPRIORITYRANK"))
        {
            row.Cells[DCC.IndexOf("RESOURCEPRIORITYRANK")].Style["width"] = "75px";
        }
        if (DCC.Contains("PrimaryBusinessRank"))
        {
            row.Cells[DCC.IndexOf("PrimaryBusinessRank")].Style["width"] = "75px";
        }
        if (DCC.Contains("SecondaryResourceRank"))
        {
            row.Cells[DCC.IndexOf("SecondaryResourceRank")].Style["width"] = "75px";
        }
        if (DCC.Contains("Production"))
        {
            row.Cells[DCC.IndexOf("Production")].Style["width"] = "60px";
        }
        if (DCC.Contains("SR_Number"))
        {
            row.Cells[DCC.IndexOf("SR_Number")].Style["width"] = "55px";
        }
        if (DCC.Contains("Version"))
        {
            row.Cells[DCC.IndexOf("Version")].Style["width"] = "45px";
        }
        if (DCC.Contains("Priority"))
        {
            row.Cells[DCC.IndexOf("Priority")].Style["width"] = "45px";
        }
        if (DCC.Contains("Primary_Analyst"))
        {
            row.Cells[DCC.IndexOf("Primary_Analyst")].Style["width"] = "85px";
        }
        if (DCC.Contains("Primary_Developer"))
        {
            row.Cells[DCC.IndexOf("Primary_Developer")].Style["width"] = "75px";
        }
        if (DCC.Contains("Assigned"))
        {
            row.Cells[DCC.IndexOf("Assigned")].Style["width"] = "75px";
        }
        if (DCC.Contains("SubmittedBy"))
        {
            row.Cells[DCC.IndexOf("SubmittedBy")].Style["width"] = "120px";
        }
        if (DCC.Contains("Status"))
        {
            row.Cells[DCC.IndexOf("Status")].Style["width"] = "55px";
        }
        if (DCC.Contains("IVTRequired"))
        {
            row.Cells[DCC.IndexOf("IVTRequired")].Style["width"] = "65px";
        }
        if (DCC.Contains("NEEDDATE"))
        {
            row.Cells[DCC.IndexOf("NEEDDATE")].Style["width"] = "80px";
        }
        if (DCC.Contains("Progress"))
        {
            row.Cells[DCC.IndexOf("Progress")].Style["width"] = "61px";
        }
        if (DCC.Contains("ProductionStatus"))
        {
            row.Cells[DCC.IndexOf("ProductionStatus")].Style["width"] = "60px";
        }
        if (DCC.Contains("INPROGRESSDATE"))
        {
            row.Cells[DCC.IndexOf("INPROGRESSDATE")].Style["width"] = "80px";
        }
        if (DCC.Contains("DEPLOYEDDATE"))
        {
            row.Cells[DCC.IndexOf("DEPLOYEDDATE")].Style["width"] = "80px";
        }
        if (DCC.Contains("READYFORREVIEWDATE"))
        {
            row.Cells[DCC.IndexOf("READYFORREVIEWDATE")].Style["width"] = "80px";
        }
        if (DCC.Contains("CLOSEDDATE"))
        {
            row.Cells[DCC.IndexOf("CLOSEDDATE")].Style["width"] = "80px";
        }
    }

    void formatColumnDisplay_Overview(ref GridViewRow row)
    {
        row.Cells[DCC.IndexOf("Full ID")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Full ID")].Style["padding-left"] = "0px";
        row.Cells[DCC.IndexOf("Full ID")].Style["width"] = "100px";
        //row.Cells[DCC.IndexOf("ItemID")].Style["text-align"] = "center";
        //row.Cells[DCC.IndexOf("ItemID")].Style["padding-left"] = "0px";
        //row.Cells[DCC.IndexOf("ItemID")].Style["width"] = "46px";
    }
    void formatHeader(ref GridViewRow row)
    {
        switch (this.MyView)
        {
            //case 0:
            //	formatHeader_WorkRequest(ref row);
            //	break;
            case 1:
                formatHeader_WorkItem(ref row);
                break;
            case 2:
                formatHeader_SR(ref row);
                break;
            case 3:
                break;
            default:
                formatHeader_WorkItem(ref row);
                break;
        }
    }
    void formatHeader_WorkRequest(ref GridViewRow row)
    {
        row.Cells[DCC["X"].Ordinal].Controls.Clear();
        row.Cells[DCC["X"].Ordinal].Controls.Add(createShowHideButton(true));
        row.Cells[DCC["X"].Ordinal].Controls.Add(createShowHideButton(false, "Hide"));


        row.Cells[DCC.IndexOf("WORKREQUESTID")].Text = "Request #";
        row.Cells[DCC.IndexOf("SR_Count")].Text = "# SRs";
    }
    void formatHeader_SR(ref GridViewRow row)
    {

    }
    void formatHeader_WorkItem(ref GridViewRow row)
    {
        row.Cells[DCC.IndexOf("WORKREQUESTID")].Text = "&nbsp;";
        row.Cells[DCC.IndexOf("ItemID")].Text = "Primary Task";
    }

    void formatDataRow(ref GridViewRow row)
    {
        switch (this.MyView)
        {
            //case 0:
            //	formatDataRow_WorkRequest(ref row);
            //	break;
            case 1:
                formatDataRow_WorkItem(ref row);
                break;
            case 2:
                formatDataRow_SR(ref row);
                break;
            case 3:
                break;
            default:
                formatDataRow_WorkItem(ref row);
                break;
        }
    }
    void formatDataRow_WorkRequest(ref GridViewRow row)
    {
        string requestId = row.Cells[DCC.IndexOf("WorkRequestID")].Text.Trim();
        int srCount = 0, workItemCount = 0;
        int.TryParse(row.Cells[DCC["SR_Count"].Ordinal].Text.Trim().Replace("&nbsp", "0"), out srCount);
        int.TryParse(row.Cells[DCC["WorkItem_Count"].Ordinal].Text.Trim().Replace("&nbsp", "0"), out workItemCount);

        if (CanViewRequest)
        {
            row.Cells[DCC["WorkRequestID"].Ordinal].Controls.Add(createEditLink(requestId, requestId));
        }

        if ((srCount > 0 && CanViewSR) || (workItemCount > 0 && CanViewWorkItem))
        {
            //buttons to show/hide child grid
            row.Cells[DCC["X"].Ordinal].Controls.Clear();
            row.Cells[DCC["X"].Ordinal].Controls.Add(createShowHideButton(true, "Show", requestId));
            row.Cells[DCC["X"].Ordinal].Controls.Add(createShowHideButton(false, "Hide", requestId));

            if (srCount > 0)
            {
                row.Cells[DCC.IndexOf("SR_Count")].Controls.Add(createSRsLink(requestId, srCount.ToString()));

                //add child grid row for SR Items
                Table table = (Table)row.Parent;
                GridViewRow childRow = createChildRow(requestId, ChildType.SR);
                table.Rows.AddAt(table.Rows.Count, childRow);
            }
            if (workItemCount > 0)
            {
                //add child grid row for Work Items
                Table table = (Table)row.Parent;
                GridViewRow childRow = createChildRow(requestId, ChildType.WorkItem);
                table.Rows.AddAt(table.Rows.Count, childRow);
            }
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

        row.Cells[DCC.IndexOf("Title")].ToolTip = row.Cells[DCC.IndexOf("Description")].Text.Replace("&nbsp;", "");
    }
    void formatDataRow_SR(ref GridViewRow row)
    {
        string itemId = row.Cells[DCC.IndexOf("ItemID")].Text.Trim();
        string sr = row.Cells[DCC.IndexOf("SR_Number")].Text.Trim();

        row.Cells[DCC["ItemID"].Ordinal].Controls.Add(createEditLink_WorkItem(itemId));

        string description = HttpUtility.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC["DESCRIPTION"].Ordinal].Text));
        string noHTML = Regex.Replace(description, @"<[^>]+>|&nbsp;", "").Trim();
        string noHTMLNormalized = Regex.Replace(noHTML, @"\s{2,}", " ");
        row.Cells[DCC["DESCRIPTION"].Ordinal].Text = noHTMLNormalized;

        string lastUpdated = DateTime.Parse(row.Cells[DCC["Last Updated"].Ordinal].Text).ToString("MMM d, yyyy HH:mm:ss tt");
        row.Cells[DCC["Last Updated"].Ordinal].Text = lastUpdated;

        //string created = DateTime.Parse(row.Cells[DCC["CreatedDate"].Ordinal].Text).ToString("MMM d, yyyy HH:mm:ss tt");
        //row.Cells[DCC["CreatedDate"].Ordinal].Text = string.Format("{0}<br/>{1}", row.Cells[DCC["CREATEDBY"].Ordinal].Text, lastUpdated);

    }
    void formatDataRow_WorkItem(ref GridViewRow row)
    {
        string itemId = row.Cells[DCC.IndexOf("ItemID")].Text.Trim();
        int taskCount = 0;
        int.TryParse(row.Cells[DCC["Task_Count"].Ordinal].Text.Trim().Replace("&nbsp", "0"), out taskCount);

        row.Cells[DCC["ItemID"].Ordinal].Controls.Add(createEditLink_WorkItem(itemId));
        row.Attributes.Add("itemID", itemId);

        string description = HttpUtility.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC["DESCRIPTION"].Ordinal].Text));
        string noHTML = Regex.Replace(description, @"<[^>]+>|&nbsp;", "").Trim();
        string noHTMLNormalized = Regex.Replace(noHTML, @"\s{2,}", " ");
        row.Cells[DCC["DESCRIPTION"].Ordinal].Text = noHTMLNormalized;

        string createdDate = DateTime.Parse(row.Cells[DCC["CREATEDDATE"].Ordinal].Text).ToString("yyyy/MM/dd h:mm tt");
        row.Cells[DCC["SubmittedBy"].Ordinal].Text = string.Format("{0}<br/>{1}", row.Cells[DCC["SubmittedBy"].Ordinal].Text, createdDate);

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
            row.Cells[DCC["WorkRequestID"].Ordinal].Controls.Clear();
            row.Cells[DCC["WorkRequestID"].Ordinal].Controls.Add(divTasks);

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
            row.Cells[DCC["WorkRequestID"].Ordinal].Controls.Add(imgBlank);
        }
    }


    Image createShowHideButton(bool show = false, string direction = "Show", string requestId = "ALL")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgShowHideChildren_click(this,'{0}','{1}');", direction, requestId);

        Image img = new Image();
        img.ID = string.Format("img{0}Children_{1}", direction, requestId);
        img.Style["display"] = show ? "block" : "none";
        img.Style["cursor"] = "pointer";
        img.Attributes.Add("Name", string.Format("img{0}", direction));
        img.Attributes.Add("requestId", requestId);
        img.Height = 10;
        img.Width = 10;
        img.ImageUrl = direction.ToUpper() == "SHOW"
            ? "Images/Icons/add_blue.png"
            : "Images/Icons/minus_blue.png";
        img.ToolTip = string.Format("{0} SRs/Work Items for [{1}]", direction, requestId);
        img.AlternateText = string.Format("{0} SRs/Work Items for [{1}]", direction, requestId);
        img.Attributes.Add("Onclick", sb.ToString());

        return img;
    }
    Image createShowHideButton_Tasks(bool show = false, string direction = "Show", string itemId = "ALL")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgShowHideChildTasks_click(this,'{0}','{1}');", direction, itemId);

        Image img = new Image();
        img.ID = string.Format("img{0}Children_{1}", direction, itemId);
        img.Style["display"] = show ? "block" : "none";
        img.Style["cursor"] = "pointer";
        img.Attributes.Add("Name", string.Format("img{0}", direction));
        img.Attributes.Add("workItemId", itemId);
        img.Height = 10;
        img.Width = 10;
        img.ImageUrl = direction.ToUpper() == "SHOW"
            ? "Images/Icons/add_blue.png"
            : "Images/Icons/minus_blue.png";
        img.ToolTip = string.Format("{0} Subtasks for [{1}]", direction, itemId);
        img.AlternateText = string.Format("{0} Subtasks for [{1}]", direction, itemId);
        img.Attributes.Add("Onclick", sb.ToString());

        return img;
    }


    LinkButton createEditLink(string requestId = "", string requestNum = "")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("lbEditRequest_click('{0}');return false;", requestId);

        LinkButton lb = new LinkButton();
        lb.ID = string.Format("lbEditRequest_{0}", requestId);
        lb.Attributes["name"] = string.Format("lbEditRequest_{0}", requestId);
        lb.ToolTip = string.Format("Edit Work Request [{0}]", requestNum);
        lb.Text = requestNum;
        lb.Attributes.Add("Onclick", sb.ToString());

        return lb;
    }
    LinkButton createEditLink_SR(string itemId = "")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("lbEditSR_click('{0}');return false;", itemId);

        LinkButton lb = new LinkButton();
        lb.ID = string.Format("lbEditSR_{0}", itemId);
        lb.Attributes["name"] = string.Format("lbEditSR_{0}", itemId);
        lb.ToolTip = string.Format("Edit SR [{0}]", itemId);
        lb.Text = itemId;
        lb.Attributes.Add("Onclick", sb.ToString());

        return lb;
    }
    LinkButton createEditLink_WorkItem(string workItemId = "")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("lbEditWorkItem_click('{0}');return false;", workItemId);

        LinkButton lb = new LinkButton();
        lb.ID = string.Format("lbEditWorkItem_{0}", workItemId);
        lb.Attributes["name"] = string.Format("lbEditWorkItem_{0}", workItemId);
        lb.ToolTip = string.Format("Edit Primary Task [{0}]", workItemId);
        lb.Text = workItemId;
        lb.Attributes.Add("Onclick", sb.ToString());

        return lb;
    }


    LinkButton createSRsLink(string requestId = "", string count = "")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("lbShowSRs_click('{0}');return false;", requestId);

        LinkButton lb = new LinkButton();
        lb.ID = string.Format("lbShowSRs_{0}", requestId);
        lb.Attributes["name"] = string.Format("lbShowSRs_{0}", requestId);
        lb.ToolTip = string.Format("View SRs for Request [{0}]", requestId);
        lb.Text = count;
        lb.Attributes.Add("Onclick", sb.ToString());

        return lb;
    }

    GridViewRow createChildRow(string requestId = "", ChildType childType = ChildType.WorkItem)
    {
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
        TableCell tableCell = null;

        try
        {
            row.CssClass = "gridBody";
            row.Style["display"] = "none";
            row.ID = string.Format("gridChild_{0}_{1}", requestId, childType.ToString());
            row.Attributes.Add("requestId", requestId);
            row.Attributes.Add("childType", childType.ToString());
            row.Attributes.Add("Name", string.Format("gridChild_{0}", requestId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC.IndexOf("X"))
                {
                    //set width to match parent
                    tableCell.Style["width"] = "12px";
                }
                else if (i == DCC.IndexOf("WORKREQUESTID"))
                {
                    tableCell.Style["padding-top"] = "4px";
                    tableCell.Style["padding-bottom"] = "3px";
                    tableCell.Style["padding-left"] = "0px";
                    tableCell.Style["padding-right"] = "0px";
                    tableCell.Style["vertical-align"] = "top";
                    tableCell.ColumnSpan = DCC.Count - 1;
                    //add the frame here
                    tableCell.Controls.Add(createChildFrame(requestId: requestId, childType: childType));
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
    GridViewRow createChildRow_Tasks(string workItemId = "")
    {
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
        TableCell tableCell = null;

        try
        {
            row.CssClass = "gridBody";
            row.Style["display"] = "none";
            row.ID = string.Format("gridChildTasks_{0}", workItemId);
            row.Attributes.Add("workItemId", workItemId);
            row.Attributes.Add("Name", string.Format("gridChild_{0}", workItemId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC["WorkRequestID"].Ordinal)
                {
                    //set width to match parent
                    tableCell.Style["width"] = "12px";
                    tableCell.Style["border-right"] = "1px solid transparent";
                }
                else if (i == DCC["ItemID"].Ordinal)
                {
                    //set width to match parent
                    tableCell.Style["width"] = "46px";
                    tableCell.Style["border-right"] = "1px solid transparent";
                }
                else if (i == DCC["Description"].Ordinal)
                {
                    tableCell.Style["padding-top"] = "10px";
                    tableCell.Style["padding-right"] = "10px";
                    tableCell.Style["padding-bottom"] = "0px";
                    tableCell.Style["padding-left"] = "0px";
                    tableCell.Style["vertical-align"] = "top";
                    tableCell.ColumnSpan = DCC.Count - 2;
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

    HtmlIframe createChildFrame(string requestId = "", ChildType childType = ChildType.WorkItem)
    {
        HtmlIframe childFrame = new HtmlIframe();

        if (string.IsNullOrWhiteSpace(requestId))
        {
            return null;
        }

        childFrame.ID = string.Format("frameChild_{0}_{1}", requestId, childType.ToString());
        childFrame.Attributes.Add("requestId", requestId);
        childFrame.Attributes.Add("childType", childType.ToString());
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["height"] = "30px";
        childFrame.Style["width"] = "100%";
        childFrame.Style["border"] = "none";

        return childFrame;
    }
    HtmlIframe createChildFrame_Tasks(string workItemId = "")
    {
        HtmlIframe childFrame = new HtmlIframe();

        if (string.IsNullOrWhiteSpace(workItemId))
        {
            return null;
        }

        childFrame.ID = string.Format("frameChildTasks_{0}", workItemId);
        childFrame.Attributes.Add("workItemId", workItemId);
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["height"] = "30px";
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

            //this has to be done in reverse order (RemoveAt)
            //OR by name(Remove) or it will have undesired result
            switch (this.MyView)
            {
                //case 0: //WorkRequest
                //	for (int i = dt.Columns.Count - 1; i >= 0; i--)
                //	{
                //		col = dt.Columns[i];
                //		if (cols.ItemByColumnName(col.ColumnName) == null)
                //		{
                //			dt.Columns.Remove(col);
                //		}
                //	}
                //	break;
                case 1: //WorkItem
                    for (int i = dt.Columns.Count - 1; i >= 0; i--)
                    {
                        col = dt.Columns[i];
                        if (cols.ItemByColumnName(col.ColumnName) == null)
                        {
                            if (col.ColumnName != "OpenedDate" & col.ColumnName != "ClosedDate")
                                dt.Columns.Remove(col);
                        }
                    }

                    //dt.Columns.Remove("X");
                    if (dt.Columns.Contains("WORKREQUESTID"))
                    {
                        dt.Columns.Remove("WORKREQUESTID");
                    }
                    break;
                case 2: //SR
                    dt.Columns.Remove("WORKREQUESTID");
                    break;
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

        switch (MyView)
        {
            //case 0:
            //	for (int i = dt.Columns.Count - 1; i >= 0; i--)
            //	{
            //		col = dt.Columns[i];

            //		gcCol = cols.ItemByColumnName(col.ColumnName);
            //		if (gcCol != null)
            //		{
            //			dt.Columns[col.ColumnName].ColumnName = gcCol.DisplayName;
            //		}
            //	}

            //	if (dt.Columns.Contains("WORKREQUESTID"))
            //	{
            //		dt.Columns["WORKREQUESTID"].ColumnName = "Request #";
            //	}
            //	break;
            case 1:
                for (int i = dt.Columns.Count - 1; i >= 0; i--)
                {
                    col = dt.Columns[i];

                    gcCol = cols.ItemByColumnName(col.ColumnName);
                    if (gcCol != null)
                    {
                        dt.Columns[col.ColumnName].ColumnName = gcCol.DisplayName;
                    }
                }

                if (dt.Columns.Contains("Work Item"))
                {
                    dt.Columns["Work Item"].ColumnName = "Primary Task";
                }
                if (dt.Columns.Contains("ItemID"))
                {
                    dt.Columns["ItemID"].ColumnName = "Primary Task";
                }
                break;
            case 2:
                for (int i = dt.Columns.Count - 1; i >= 0; i--)
                {
                    col = dt.Columns[i];

                    gcCol = cols.ItemByColumnName(col.ColumnName);
                    if (gcCol != null)
                    {
                        dt.Columns[col.ColumnName].ColumnName = gcCol.DisplayName;
                    }
                }

                if (dt.Columns.Contains("ItemID"))
                {
                    dt.Columns["ItemID"].ColumnName = "SR ID";
                }
                break;
        }

        dt.AcceptChanges();
    }
    private void FormatExcelRows(ref DataTable dt)
    {
        GridColsCollection cols = columnData.VisibleColumns();
        DataColumn col = null;

        switch (MyView)
        {
            case 0:
                break;
            case 1:
                bool hasIVT = false, hasProd = false;
                hasIVT = (cols.ItemByColumnName("Requires IVT_Temp") != null);
                hasProd = (cols.ItemByColumnName("Production_Temp") != null);

                if (hasIVT)
                {
                    dt.Columns.Add("Requires IVT");
                }

                if (hasProd)
                {
                    dt.Columns.Add("Production");
                }

                bool flag = false;
                if (hasIVT || hasProd)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (hasIVT)
                        {
                            if (bool.TryParse(row["Requires IVT_Temp"].ToString(), out flag) && flag)
                            {
                                row["Requires IVT"] = "Yes";
                            }
                            else
                            {
                                row["Requires IVT"] = "No";
                            }

                            dt.Columns["Requires IVT"].SetOrdinal(dt.Columns["Requires IVT_Temp"].Ordinal);
                            dt.Columns.Remove("Requires IVT_Temp");
                        }

                        if (hasProd)
                        {
                            if (bool.TryParse(row["Production_Temp"].ToString(), out flag) && flag)
                            {
                                row["Production"] = "Yes";
                            }
                            else
                            {
                                row["Production"] = "No";
                            }

                            dt.Columns["Production"].SetOrdinal(dt.Columns["Production_Temp"].Ordinal);
                            dt.Columns.Remove("Production_Temp");
                        }
                    }

                    dt.Columns["Submitted Date"].SetOrdinal(dt.Columns["Submitted By"].Ordinal);
                }
                break;
            case 2:
                break;
        }

        dt.AcceptChanges();
    }

    private bool ExportExcel(DataTable dt)
    {

        //var reportQueueURL = 'Loading.aspx?Page=ReportLoading.aspx?Report=WTS_WORKLOAD';
        //window.open(reportQueueURL);

        bool success = false;
        string errorMsg = string.Empty;
        string currentTaskID = "";

        try
        {
            Workbook wb = new Workbook(FileFormatType.Xlsx);
            Worksheet wsRaw = wb.Worksheets[0];
            wb.Worksheets.Add();
            Worksheet ws = wb.Worksheets[1];


            StyleFlag flag = new StyleFlag() { All = true };
            Aspose.Cells.Style style = new Aspose.Cells.Style();
            Aspose.Cells.Style style2 = new Aspose.Cells.Style();
            Aspose.Cells.Style style3 = new Aspose.Cells.Style();
            DataTable drillDownAllSubTasks = new DataTable();
            DataTable drillDownAllTasks = new DataTable();
            DataTable dtCopy = dt.Clone();
            DataTable dtRaw = null;

            foreach (DataRow r in dt.Rows)
            {
                WTSUtility.importDataRow(ref dtCopy, r);
            }

            RemoveExcelColumns(ref dt);
            RenameExcelColumns(ref dt);

            string name = string.Empty;
            switch (this.MyView)
            {
                //case 0:
                //                // --------  Work Request Grid Export  -----------------------------------------------------------------------
                //                name = "Work Request";
                //	ws.Name = "Work Request";
                //                wsRaw.Name = "Workload Request Raw";
                //                style.Pattern = BackgroundType.Solid;
                //	style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
                //	style2.Pattern = BackgroundType.Solid;
                //	style2.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightGreen");
                //	style3.Pattern = BackgroundType.Solid;
                //	style3.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightBlue");

                //                // TODO - Pass in SelectedAssigned here too?
                //                DataTable dtDrilldown = WorkloadItem.WorkItemList_Get();
                //                if (dtDrilldown != null)
                //                {
                //                    dtDrilldown.DefaultView.RowFilter = "STATUSID IN (" + String.Join(", ", this.SelectedStatuses) + ")";
                //                    dtDrilldown = dtDrilldown.DefaultView.ToTable();
                //                }

                //                DataTable dtExcel = new DataTable();

                //	dtExcel = dt.Clone();

                //	for (int i = 0; i <= dtExcel.Columns.Count - 1; i++)
                //	{
                //		dtExcel.Columns[i].DataType = typeof(string);
                //		dtExcel.Columns[i].MaxLength = 255;
                //		dtExcel.Columns[i].AllowDBNull = true;
                //	}

                //	foreach (DataRow dr in dt.Rows)
                //	{
                //		dtExcel.ImportRow(dr);

                //		if (dtDrilldown != null && dtDrilldown.Rows.Count > 0)
                //		{
                //			DataTable dtTemp = new DataTable();

                //			dtTemp = dtDrilldown.Copy();
                //			dtTemp.DefaultView.RowFilter = "WORKREQUESTID = " + dr["Request Number"];
                //			dtTemp = dtTemp.DefaultView.ToTable();

                //			int count = 0;
                //			foreach (DataRow drTemp in dtTemp.Rows)
                //			{
                //				if (count == 0)
                //				{
                //					dtExcel.Rows.Add("", "#", "System(Task)", "Status", "Title", "Allocation Assignment", "Production Status", "Version", "Priority", "Primary Analyst", "Primary Developer", "Assigned", "Progress", "");
                //				}

                //				dtExcel.Rows.Add("", drTemp["ItemID"].ToString(), drTemp["Websystem"], drTemp["STATUS"], drTemp["TITLE"], drTemp["ALLOCATION"], drTemp["ProductionStatus"], drTemp["Version"], drTemp["PRIORITY"], drTemp["Primary_Analyst"], drTemp["Primary_Developer"], drTemp["Assigned"], drTemp["Progress"].ToString(), "");

                //				int itemID = 0;
                //				int.TryParse(drTemp["ItemID"].ToString(), out itemID);
                //				if (itemID > 0)
                //				{
                //					DataTable dtDrilldownTask = WorkloadItem.WorkItem_GetTaskList(workItemID: itemID, showArchived: _showArchived, showBacklog: ShowBacklog);
                //                                if (dtDrilldownTask != null)
                //                                {
                //                                    dtDrilldownTask.DefaultView.RowFilter = "STATUSID IN (" + String.Join(", ", this.SelectedStatuses) + ")";
                //                                    dtDrilldownTask = dtDrilldownTask.DefaultView.ToTable();
                //                                }

                //                                if (dtDrilldownTask != null && dtDrilldownTask.Rows.Count > 0)
                //					{
                //						int countTask = 0;
                //						foreach (DataRow drChild in dtDrilldownTask.Rows)
                //						{
                //							if (countTask == 0)
                //							{
                //								dtExcel.Rows.Add("", "", "Task #", "Title", "Planned Start", "Actual Start", "Planned Hours", "Actual Hours", "Actual End", "Assigned", "Status", "Progress");
                //							}

                //							dtExcel.Rows.Add("", "", drChild["WORKITEMID"].ToString() + " - " + drChild["TASK_NUMBER"].ToString(), drChild["Title"], String.Format("{0:M/d/yyyy}", drChild["ESTIMATEDSTARTDATE"].ToString()), String.Format("{0:M/d/yyyy}", drChild["ACTUALSTARTDATE"].ToString()), drChild["PLANNEDHOURS"].ToString(), drChild["ACTUALHOURS"].ToString(), String.Format("{0:M/d/yyyy}", drChild["ACTUALENDDATE"].ToString()), drChild["AssignedResource"], drChild["STATUS"], drChild["COMPLETIONPERCENT"].ToString());
                //							countTask++;
                //                                        WTSUtility.importDataRow(ref drillDownAllSubTasks, drChild);
                //                                    }
                //					}
                //                                WTSUtility.importDataRow(ref drillDownAllTasks, drTemp);
                //                            }

                //				count++;
                //			}
                //		}
                //                }

                //	ws.Cells.ImportDataTable(dtExcel, true, 0, 0, false, false);

                //	for (int j = 0; j <= ws.Cells.Rows.Count - 1; j++)
                //	{
                //		if (ws.Cells.Rows[j][0].Value.ToString() == "Request #")
                //                    {
                //			Range range = ws.Cells.CreateRange(j, 0, 1, 14);
                //			range.ApplyStyle(style, flag);
                //		}

                //		if (ws.Cells.Rows[j][1].Value.ToString() == "#")
                //		{
                //			Range range = ws.Cells.CreateRange(j, 1, 1, 12);
                //			range.ApplyStyle(style2, flag);
                //		}

                //                    if (ws.Cells.Rows[j][2].Value != null)
                //                    {
                //		    if (ws.Cells.Rows[j][2].Value.ToString() == "Task #")
                //		    {
                //			    Range range = ws.Cells.CreateRange(j, 2, 1, 10);
                //			    range.ApplyStyle(style3, flag);
                //		    }
                //                    }
                //                }

                //                wsRaw.Cells.ImportDataTable(dtExcel, true, "A1");

                //                break;

                case 1:
                    // --------  WorkItem Grid Export  ------------------------------------------------------------------------
                    name = "Workload";
                    ws.Name = "Workload";
                    wsRaw.Name = "Workload Raw";

                    FormatExcelRows(ref dt);

                    bool myData = this._myData;

                    style.Pattern = BackgroundType.Solid;
                    style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");  // Redish
                    style2.Pattern = BackgroundType.Solid;
                    style2.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#e7a8a1");  // Light orange
                    style3.Pattern = BackgroundType.Solid;
                    style3.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightBlue");

                    DataTable dtExcelWorkload = new DataTable();

                    dtExcelWorkload = dt.Clone();

                    for (int i = 0; i <= dtExcelWorkload.Columns.Count - 1; i++)
                    {
                        dtExcelWorkload.Columns[i].DataType = typeof(string);
                        dtExcelWorkload.Columns[i].MaxLength = 255;
                        dtExcelWorkload.Columns[i].AllowDBNull = true;
                    }

                    int outerCount = 0;
                    int outerItemID = 0;
                    int commentCount = 0;
                    string allTaskComments = "";

                    dt.Columns.Add("Notes");
                    dtExcelWorkload.Columns.Add("Notes");

                    foreach (DataRow dr in dt.Rows)
                    {
                        int.TryParse(dr["Primary Task"].ToString(), out outerItemID);
                        int itemID = 0;
                        int.TryParse(dr["Primary Task"].ToString(), out itemID);

                        allTaskComments = "";
                        commentCount = 0;
                        DataTable dtTaskNotes = WorkloadItem.WorkItem_GetCommentList(itemID);
                        if (dtTaskNotes != null)
                        {
                            foreach (DataRow drComment in dtTaskNotes.Rows)
                            {
                                commentCount += 1;
                                allTaskComments += "COMMENT " + commentCount + ": " + drComment["COMMENT_TEXT"].ToString() + "  ";
                            }
                            allTaskComments = StripHTMLAndLimitSize(allTaskComments);

                            dr["Notes"] = allTaskComments;
                            dr.AcceptChanges();
                        }


                        dtExcelWorkload.ImportRow(dr);

                        if (itemID > 0)

                        {
                            DataTable dtDrilldownTask = WorkloadItem.WorkItem_GetTaskList(workItemID: itemID, showArchived: _showArchived, showBacklog: ShowBacklog);

                            try
                            {
                                dtDrilldownTask.Columns.Add("Notes");
                            }
                            catch (Exception) {}
                            try
                            {
                                dtCopy.Columns.Add("Notes");
                            }
                            catch (Exception) {}

                            if (dtDrilldownTask != null)
                            {
                                dtDrilldownTask.DefaultView.RowFilter = "STATUSID IN (" + String.Join(", ", this.SelectedStatuses) + ")";
                                dtDrilldownTask = dtDrilldownTask.DefaultView.ToTable();
                            }
                            if (dtDrilldownTask != null && dtDrilldownTask.Rows.Count > 0)
                            {
                                dtDrilldownTask.Columns["WORKITEMID"].ColumnName = "Primary Task";

                                int count = 0;
                                foreach (DataRow drChild in dtDrilldownTask.Rows)
                                {
                                    if (count == 0)
                                    {
                                        dtExcelWorkload.Rows.Add(itemID.ToString(), "Subtask", "Title", "Assigned To", "Planned Start Date", "Actual Start Date", "Planned Hours", "Actual Hours", "Actual End Date", "Percent Complete", "Status", "Sort", "Notes");
                                    }

                                    allTaskComments = "";
                                    commentCount = 0;
                                    WorkItem_Task task = new WorkItem_Task(taskID: (int)drChild["WORKITEM_TASKID"]);
                                    DataTable dtSubTaskNotes = task.GetComments();
                                    if (dtSubTaskNotes != null)
                                    {
                                        foreach (DataRow drComment in dtSubTaskNotes.Rows)
                                        {
                                            commentCount += 1;
                                            allTaskComments += "SUBTASK COMMENT " + commentCount + ": " + drComment["COMMENT_TEXT"].ToString() + "  ";

                                        }
                                        allTaskComments = StripHTMLAndLimitSize(allTaskComments);

                                        drChild["Notes"] = allTaskComments;
                                        drChild.AcceptChanges();
                                    }
                                    try
                                    {
                                        // Careful, don't break working code. This line adds subtasks.
                                        dtExcelWorkload.Rows.Add(itemID, drChild["Primary Task"].ToString() + " - " + drChild["TASK_NUMBER"].ToString(), drChild["Title"], drChild["AssignedResource"], String.Format("{0:M/d/yyyy}", drChild["ESTIMATEDSTARTDATE"].ToString()), String.Format("{0:M/d/yyyy}", drChild["ACTUALSTARTDATE"].ToString()), drChild["PLANNEDHOURS"].ToString(), drChild["ACTUALHOURS"].ToString(), String.Format("{0:M/d/yyyy}", drChild["ACTUALENDDATE"].ToString()), drChild["COMPLETIONPERCENT"].ToString(), drChild["STATUS"], drChild["SORT_ORDER"].ToString(), allTaskComments);
                                    }
                                    catch (Exception ex)
                                    {
                                        dtExcelWorkload.Rows.Add(itemID, drChild["Primary Task"].ToString() + " - " + drChild["TASK_NUMBER"].ToString(), drChild["Title"], drChild["AssignedResource"], String.Format("{0:M/d/yyyy}", drChild["ESTIMATEDSTARTDATE"].ToString()), String.Format("{0:M/d/yyyy}", drChild["ACTUALSTARTDATE"].ToString()), drChild["PLANNEDHOURS"].ToString(), drChild["ACTUALHOURS"].ToString(), String.Format("{0:M/d/yyyy}", drChild["ACTUALENDDATE"].ToString()), drChild["COMPLETIONPERCENT"].ToString(), drChild["STATUS"], drChild["SORT_ORDER"].ToString(), "");
                                    }

                                    count++;
                                    outerCount += 1;

                                    WTSUtility.importDataRow(ref drillDownAllSubTasks, drChild);
                                }
                            }

                            // 12848 - 15 > If no subtask(s), still need to add a row.
                            else // No subtasks
                            {
                                /*if (outerCount == 0)
                                    dtExcelWorkload.Rows.Add(outerItemID, 0, dr["Title"], dr["Assigned"]);*/

                                WTSUtility.importDataRow(ref drillDownAllSubTasks, dr);

                                try
                                {
                                    drillDownAllSubTasks.Columns.Add("TASK_NUMBER");
                                }
                                catch (Exception) {}

                                drillDownAllSubTasks.Rows[outerCount]["TASK_NUMBER"] = 0; // outerItemID.ToString() + " - 0";
                                drillDownAllSubTasks.AcceptChanges();

                                outerCount += 1;
                            }  // Sub tasks??
                        }  // ItemID > 0
                    }

                    ws.Cells.ImportDataTable(dtExcelWorkload, true, 0, 0, false, false);

                    for (int j = 0; j <= ws.Cells.Rows.Count - 1; j++)
                    {
                        if (currentTaskID != ws.Cells.Rows[j][0].Value.ToString())
                        {
                            Range range = ws.Cells.CreateRange(j, 0, 1, dt.Columns.Count);
                            range.ApplyStyle(style2, flag);
                            currentTaskID = ws.Cells.Rows[j][0].Value.ToString();
                        }
                        if (ws.Cells.Rows[j][0].Value.ToString() == "Primary Task")
                        {
                            Range range = ws.Cells.CreateRange(j, 0, 1, dt.Columns.Count);  //"#E6E6E6"
                            range.ApplyStyle(style, flag);
                        }

                        if (ws.Cells.Rows[j][1].Value.ToString() == "Subtask")
                        {
                            Range range = ws.Cells.CreateRange(j, 1, 1, 12);  // Light blue
                            range.ApplyStyle(style3, flag);
                        }
                    }

                    formatParent(ref dtCopy);

                    if (drillDownAllSubTasks.Columns.Contains("AssignedResource"))
                    {
                        formatChild(ref drillDownAllSubTasks);
                        dtRaw = WTSUtility.JoinDataTables(dtCopy, drillDownAllSubTasks, (row1, row2) => row1.Field<string>("ItemID") == row2.Field<string>("Primary Task"));
                        formatRawData(ref dtRaw);
                    }
                    else  // Enterprise
                    {
                        formatChildEnterprise(ref drillDownAllSubTasks);
                        dtRaw = WTSUtility.JoinDataTables(dtCopy, drillDownAllSubTasks, (row1, row2) => row1.Field<string>("ItemID") == row2.Field<string>("Primary Task"));
                        formatRawDataEnterprise(ref dtRaw);
                    }

                    // Make Notes the last column
                    dtRaw.Columns["Notes"].SetOrdinal(dtRaw.Columns.Count - 1);

                    wsRaw.Cells.ImportDataTable(dtRaw, true, "A1");

                    break;
                case 2:
                    // --------  SR Export  -------------------------------------------------------------------------------
                    name = "SR";
                    ws.Name = "SR";
                    wsRaw.Name = "SR Raw";
                    string description;
                    string noHTML;
                    string noHTMLNormalized;
                    //string createdConcat;

                    style.Pattern = BackgroundType.Solid;
                    style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
                    style2.Pattern = BackgroundType.Solid;
                    style2.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightGreen");
                    style3.Pattern = BackgroundType.Solid;
                    style3.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("LightBlue");

                    dt.Columns.Remove("STATUSID");

                    DataTable dtExcelWorkloadSR = new DataTable();
                    dtExcelWorkloadSR = dt.Clone();

                    for (int i = 0; i <= dtExcelWorkloadSR.Columns.Count - 1; i++)
                    {
                        if (dtExcelWorkloadSR.Columns[i].ColumnName == "Description")
                        {
                            dtExcelWorkloadSR.Columns[i].DataType = typeof(string);
                            dtExcelWorkloadSR.Columns[i].MaxLength = -1;
                            dtExcelWorkloadSR.Columns[i].AllowDBNull = true;
                        }
                        else
                        {
                            dtExcelWorkloadSR.Columns[i].DataType = typeof(string);
                            dtExcelWorkloadSR.Columns[i].MaxLength = 255;
                            dtExcelWorkloadSR.Columns[i].AllowDBNull = true;
                        }
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        description = HttpUtility.HtmlDecode(dr["Description"].ToString());
                        noHTML = Regex.Replace(description, @"<[^>]+>|&nbsp;", "").Trim();
                        noHTMLNormalized = Regex.Replace(noHTML, @"\s{2,}", " ");

                        //createdConcat = dr["Created"].ToString() + ' ' + dr["CreatedDate"].ToString();

                        dtExcelWorkloadSR.Rows.Add(dr["System"], dr["Priority"], dr["Task #"], dr["SubTask #"], dr["SR Number"], dr["Primary Bus. Res."], dr["Primary Tech. Res."], dr["Status"], dr["% Complete"], dr["Title"], noHTMLNormalized, dr["Sub Task Title"], dr["Created"], dr["Last Updated"]);
                        WTSUtility.importDataRow(ref drillDownAllSubTasks, dr);
                    }

                    ws.Cells.ImportDataTable(dtExcelWorkloadSR, true, "A1");
                    wsRaw.Cells.ImportDataTable(dtExcelWorkloadSR, true, "A1");

                    break;
            }

            wsRaw.AutoFitColumns();
            ws.AutoFitColumns();

            // FreezePanes Params:
            // Row, the row index of the cell that the freeze will start from.
            // Column, the column index of the cell that the freeze will start from.
            // Frozen rows, the number of visible rows in the top pane.
            // Frozen columns, the number of visible columns in the left pane
            wsRaw.FreezePanes(1, 1, 1, 1);
            ws.FreezePanes(1, 1, 1, 1);

            MemoryStream ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Xlsx);

            Response.ContentType = "application/xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
            Response.BinaryWrite(ms.ToArray());

            success = true;

            Response.End();
        }
        catch (System.Threading.ThreadAbortException)
        {
            //expected. do nothing
        }
        catch (Exception ex)
        {
            success = false;
            errorMsg += Environment.NewLine + ex.Message;
        }

        return success;
    }

    public static string StripHTMLAndLimitSize(string input)
    {
        string notes = input;
        notes = Regex.Replace(input, "<.*?>", String.Empty);
        if (notes.Length > 32000)
        {
            notes = notes.Substring(0, 32000);
        }
        return notes;
    }
    private void formatRawDataWithWorkRequest(ref DataTable dtRaw)
    {
        dtRaw.Columns.Remove("RequestGroupID");
        dtRaw.Columns.Remove("CONTRACTID");
        dtRaw.Columns.Remove("ORGANIZATIONID");
        dtRaw.Columns.Remove("REQUESTTYPEID");
        dtRaw.Columns.Remove("WTS_SCOPEID");
        dtRaw.Columns.Remove("EFFORTID");
        dtRaw.Columns.Remove("SR_Count");
        dtRaw.Columns.Remove("SR_Date");
        dtRaw.Columns.Remove("WorkItem_Count");
        dtRaw.Columns.Remove("WorkItem_Date");
        dtRaw.Columns.Remove("OP_PRIORITYID");
        dtRaw.Columns.Remove("SME");
        dtRaw.Columns.Remove("LEAD_IA_TWID");
        dtRaw.Columns.Remove("LEAD_RESOURCEID");
        dtRaw.Columns.Remove("My_Role");
        dtRaw.Columns.Remove("TITLE");
        dtRaw.Columns["RequestGroup"].SetOrdinal(0);
        dtRaw.Columns["RequestGroup"].ColumnName = "Request Group";
        dtRaw.Columns["CONTRACT"].SetOrdinal(2);
        dtRaw.Columns["CONTRACT"].ColumnName = "Contract";
        dtRaw.Columns["ORGANIZATION"].SetOrdinal(3);
        dtRaw.Columns["ORGANIZATION"].ColumnName = "Request Organization";
        dtRaw.Columns["Request_Type"].SetOrdinal(4);
        dtRaw.Columns["Request_Type"].ColumnName = "Request Type";
        dtRaw.Columns["Work_Scope"].SetOrdinal(5);
        dtRaw.Columns["Work_Scope"].ColumnName = "Work Scope";
        dtRaw.Columns["EFFORT"].SetOrdinal(6);
        dtRaw.Columns["EFFORT"].ColumnName = "Request Effort";
        dtRaw.Columns["DESCRIPTION"].SetOrdinal(1);
        dtRaw.Columns["DESCRIPTION"].ColumnName = "Work Request Description";
        dtRaw.Columns["PRIORITY"].SetOrdinal(7);
        dtRaw.Columns["PRIORITY"].ColumnName = "Work Request Priority";
        dtRaw.Columns["Lead_Tech_Writer"].ColumnName = "Lead Tech Writer";
        dtRaw.Columns["Lead_Resource"].ColumnName = "Work Request Lead";
        dtRaw.Columns["SUBMITTEDBY"].ColumnName = "Work Request Submitted By";
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
    private void formatChildEnterprise(ref DataTable dtDrillDownTaskAll)
    {
        dtDrillDownTaskAll.Columns["STATUS"].ColumnName = "Sub Task Status";
        dtDrillDownTaskAll.Columns["TITLE"].ColumnName = "Sub Task Title";
        dtDrillDownTaskAll.Columns["PRIORITY"].ColumnName = "Sub Task Priority";
        dtDrillDownTaskAll.Columns["Submitted"].ColumnName = "Sub Task Submitted By";
        dtDrillDownTaskAll.Columns["Assigned"].ColumnName = "Sub Task Assigned To";
        dtDrillDownTaskAll.AcceptChanges();
    }
    private void formatRawDataEnterprise(ref DataTable dtRaw)
    {
        dtRaw.Columns.Remove("X");
        dtRaw.Columns.Remove("Y");
        dtRaw.Columns.Remove("Z");
        dtRaw.Columns.Remove("ItemID");
        dtRaw.Columns.Remove("WORKREQUESTID");
        dtRaw.Columns.Remove("WorkTypeID");
        dtRaw.Columns.Remove("Task_Count");
        dtRaw.Columns.Remove("WTS_SYSTEMID");
        dtRaw.Columns.Remove("STATUSID");
        dtRaw.Columns.Remove("NEEDDATE");
        dtRaw.Columns.Remove("AllocationGroupID");
        dtRaw.Columns.Remove("AllocationCategoryID");
        dtRaw.Columns.Remove("ALLOCATIONID");
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
        dtRaw.Columns.Remove("WORKREQUEST");
        dtRaw.Columns.Remove("PhaseID");
        dtRaw.Columns.Remove("WORKITEMTYPEID");
        dtRaw.Columns.Remove("IVTRequired");
        dtRaw.Columns.Remove("AllocationCategory");
        dtRaw.Columns.Remove("Production");
        dtRaw.Columns.Remove("ProductVersionID");
        dtRaw.Columns.Remove("ARCHIVE");
        dtRaw.Columns.Remove("SecondaryBusinessResourceID");
        dtRaw.Columns.Remove("Primary_Analyst");

        if (dtRaw.Columns.Contains("WORKITEMID"))
        {
            dtRaw.Columns["WORKITEMID"].SetOrdinal(0);
            dtRaw.Columns["WORKITEMID"].ColumnName = "Primary Task";
        }
        else
        {
            dtRaw.Columns["Primary Task"].SetOrdinal(0);
        }

        dtRaw.Columns["TASK_NUMBER"].SetOrdinal(1);
        dtRaw.Columns["TASK_NUMBER"].ColumnName = "Sub Task #";
        dtRaw.Columns["AllocationGroup"].ColumnName = "Allocation Group";
        dtRaw.Columns["RESOURCEPRIORITYRANK"].ColumnName = "Task Primary Tech Rank";
        dtRaw.Columns["PrimaryBusinessRank"].ColumnName = "Task Primary Bus Rank";
        dtRaw.Columns["WorkArea"].ColumnName = "Work Area";
        dtRaw.Columns["WorkloadGroup"].ColumnName = "Functionality";
        dtRaw.Columns["Version"].ColumnName = "Product Version";
        dtRaw.Columns["Task Status"].SetOrdinal(2);
        dtRaw.Columns["Task Title"].SetOrdinal(3);
        dtRaw.Columns["Task Description"].SetOrdinal(4);
        dtRaw.Columns["Task Priority"].SetOrdinal(5);
        dtRaw.Columns["Task Assigned To"].SetOrdinal(6);
        dtRaw.Columns["Task Primary Dev"].SetOrdinal(7);
        dtRaw.Columns["Task Primary Bus"].SetOrdinal(8);
        dtRaw.Columns["Task Created By"].SetOrdinal(10);
        dtRaw.Columns["Task Submitted By"].SetOrdinal(11);

        dtRaw.Columns.Add("Sort1", Type.GetType("System.Int32")); //hack to fix sorting problems

        foreach (DataRow r in dtRaw.Rows)
        {
            r["Sort1"] = Convert.ToInt32(r["Primary Task"]);
            r["Task Title"] = System.Text.RegularExpressions.Regex.Replace(r["Task Title"].ToString(), @"<[^>]+>|&nbsp;", "").Trim(); //remove HTML tags
            r["Sub Task Title"] = System.Text.RegularExpressions.Regex.Replace(r["Sub Task Title"].ToString(), @"<[^>]+>|&nbsp;", "").Trim();
            r["Task Description"] = System.Text.RegularExpressions.Regex.Replace(r["Task Description"].ToString(), @"<[^>]+>|&nbsp;", "").Trim();
        }

        DataView dv = dtRaw.DefaultView;
        dv.Sort = "[Sort1] asc";  //, [Sort2] asc";
        dtRaw = dv.ToTable();
        DataRow previousRow = dtRaw.Rows[dtRaw.Rows.Count - 1];
        dtRaw.Columns.Remove("Sort1");
        dtRaw.AcceptChanges();
    }

    private void formatNewRawData(ref DataTable dtRaw2)
    {

        dtRaw2.Columns.Remove("STATUSID");
    }
    private void formatRawData(ref DataTable dtRaw)
    {
        dtRaw.Columns.Remove("X");
        dtRaw.Columns.Remove("Y");
        dtRaw.Columns.Remove("Z");
        dtRaw.Columns.Remove("ItemID");
        dtRaw.Columns.Remove("WORKREQUESTID");
        dtRaw.Columns.Remove("WorkTypeID");
        dtRaw.Columns.Remove("Task_Count");
        dtRaw.Columns.Remove("WTS_SYSTEMID");
        dtRaw.Columns.Remove("STATUSID");
        dtRaw.Columns.Remove("AllocationGroupID");
        dtRaw.Columns.Remove("AllocationCategoryID");
        dtRaw.Columns.Remove("ALLOCATIONID");
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

        dtRaw.Columns.Remove("SecondaryBusinessResourceID");
        dtRaw.Columns.Remove("SECONDARYBUSRESOURCEID");
        dtRaw.Columns.Remove("PRIORITYIDSORTED");
        dtRaw.Columns.Remove("PRIMARYBUSRESOURCEID");
        //

        if (dtRaw.Columns["WORKITEMID"] != null)
        {
            dtRaw.Columns["WORKITEMID"].SetOrdinal(0);
            dtRaw.Columns["WORKITEMID"].ColumnName = "Primary Task";
        }
        else
        {
            dtRaw.Columns["Primary Task"].SetOrdinal(0);
        }
        if (dtRaw.Columns["TASK_NUMBER"] != null)
        {
            dtRaw.Columns["TASK_NUMBER"].SetOrdinal(1);
            dtRaw.Columns["TASK_NUMBER"].ColumnName = "Sub Task #";
        }
        dtRaw.Columns["INPROGRESSDATE"].ColumnName = "In Progress Date";
        dtRaw.Columns["DEPLOYEDDATE"].ColumnName = "Deployed Date";
        dtRaw.Columns["READYFORREVIEWDATE"].ColumnName = "Ready for Review";
        dtRaw.Columns["CLOSEDDATE"].ColumnName = "Closed Review";
        dtRaw.Columns["NEEDDATE"].ColumnName = "Date Needed";
        dtRaw.Columns["WORKREQUEST"].ColumnName = "Work Request";
        dtRaw.Columns["WORKITEMTYPE"].ColumnName = "Work Activity";
        dtRaw.Columns["WorkType"].ColumnName = "Resource Group";
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
            r["Sort1"] = Convert.ToInt32(r["Primary Task"]);
            r["Sort2"] = Convert.ToInt32(r["Sub Task #"]);
            r["Sub Task #"] = r["Primary Task"] + "-" + r["Sub Task #"];
            r["Task Title"] = System.Text.RegularExpressions.Regex.Replace(r["Task Title"].ToString(), @"<[^>]+>|&nbsp;", "").Trim(); //remove HTML tags
            r["Sub Task Title"] = System.Text.RegularExpressions.Regex.Replace(r["Sub Task Title"].ToString(), @"<[^>]+>|&nbsp;", "").Trim();
            r["Task Description"] = System.Text.RegularExpressions.Regex.Replace(r["Task Description"].ToString(), @"<[^>]+>|&nbsp;", "").Trim();
            if (r["Task Description"].ToString().Length > 500)
            {
                r["Task Description"] = r["Task Description"].ToString().Substring(0, 500);
            }
            r["Sub Task Description"] = System.Text.RegularExpressions.Regex.Replace(r["Sub Task Description"].ToString(), @"<[^>]+>|&nbsp;", "").Trim();
            if (r["Sub Task Description"].ToString().Length > 500)
            {
                r["Sub Task Description"] = r["Sub Task Description"].ToString().Substring(0, 500);
            }
        }

        DataView dv = dtRaw.DefaultView;
        dv.Sort = "[Sort1] asc, [Sort2] asc";
        dtRaw = dv.ToTable();
        DataRow previousRow = dtRaw.Rows[dtRaw.Rows.Count - 1];
        for (int i = dtRaw.Rows.Count - 2; i >= 0; i--) //renove duplicate rows ie select distinct by sub task #.
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

    #endregion Grid

    private void loadMenus()
    {
        try
        {
            DataSet dsMenu = new DataSet();
            dsMenu.ReadXml(this.Server.MapPath("XML/WTS_Menus.xml"));

            if (dsMenu.Tables.Count > 0 && dsMenu.Tables[0].Rows.Count > 0)
            {
                if (dsMenu.Tables.Contains("WorkloadGridRelatedItem"))
                {
                    menuRelatedItems.DataSource = dsMenu.Tables["WorkloadGridRelatedItem"];
                    menuRelatedItems.DataValueField = "URL";
                    menuRelatedItems.DataTextField = "Text";
                    menuRelatedItems.DataIDField = "id";
                    if (dsMenu.Tables["WorkloadGridRelatedItem"].Columns.Contains("WorkloadGridRelatedItem_id_0"))
                    {
                        menuRelatedItems.DataParentIDField = "WorkloadGridRelatedItem_id_0";
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
        dtSub.TableName = "WorkloadMetricsSub";
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

        DataSet dsMetrics = new DataSet();
        dsMetrics.Tables.Add(dt);
        dsMetrics.Tables.Add(dtSub);

        return JsonConvert.SerializeObject(dsMetrics, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod(true)]
    public static bool ItemExists(int itemID, int taskNumber, string type)
    {
        try
        {
            return Workload.ItemExists(itemID, taskNumber, type);
        }
        catch (Exception) { return false; }
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

}