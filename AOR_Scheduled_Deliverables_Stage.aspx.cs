﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using System.Web.UI.HtmlControls;

public partial class AOR_Scheduled_Deliverables_Stage : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;

    protected int ReleaseID = 0;

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;

    protected bool CanView = false;
    protected bool CanEdit = false;
    protected bool IsAdmin = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.IsAdmin = UserManagement.UserIsInRole("Admin");
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.Deployment);
        this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.Deployment));

        readQueryString();
        initControls();
        loadGridData();
    }

    private void readQueryString()
    {
        if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
            || Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
        {
            _refreshData = true;
        }
        if (Request.QueryString["ReleaseID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ReleaseID"].ToString()), out this.ReleaseID);
        }
        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }
    }

    private void initControls()
    {
        grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
        grdMD.GridRowDataBound += grdMD_GridRowDataBound;
        grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
    }

    private void loadGridData()
    {
        DataTable dt = MasterData.ReleaseSchedule_DeliverableList_Get(ReleaseID);

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

            InitializeColumnData(ref dt);
            dt.AcceptChanges();
        }

        int count = dt.Rows.Count;
        count = count > 0 ? count - 1 : count; //need to subtract the empty row
        spanRowCount.InnerText = count.ToString();

        grdMD.DataSource = dt;
        grdMD.DataBind();
    }

    protected void InitializeColumnData(ref DataTable dt)
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
                        blnVisible = true;
                        break;
                    case "ReleaseScheduleDeliverable":
                        displayName = "Deployment";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "AORCount":
                        displayName = "# of AORs";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ContractCount":
                        displayName = "# of Contracts";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Description":
                        displayName = "Description";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Narrative":
                        blnVisible = true;
                        break;
                    case "PlannedDevTestStart":
                        displayName = "Planned Start";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "PlannedEnd":
                        displayName = "Planned End";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Risk":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Avg. Resources":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Visible":
                        displayName = "Visible To Customer";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "SORT_ORDER":
                        displayName = "Sort Order";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ARCHIVE":
                        displayName = "Archive";
                        blnVisible = true;
                        blnSortable = true;
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
            columnData.SortDataTable(ref dt, SortOrder);
            SelectedColumnOrder = columnData.CurrentColumnOrderToString();

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }


    #region Grid

    void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
        row.Cells[DCC.IndexOf("Avg. Resources")].Style["text-align"] = "center";

    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        //add edit link
        string itemId = row.Cells[DCC.IndexOf("ReleaseScheduleID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanView)
        {
            string selectedId = row.Cells[DCC.IndexOf("ReleaseScheduleID")].Text;
            string selectedText = row.Cells[DCC.IndexOf("ReleaseScheduleDeliverable")].Text;
            row.Cells[DCC.IndexOf("ContractCount")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("ContractCount")].Text, itemId));
            row.Cells[DCC.IndexOf("DESCRIPTION")].Controls.Add(WTSUtility.CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("DESCRIPTION")].Text.Replace("&nbsp;", " ").Trim())));
            row.Cells[DCC.IndexOf("SORT_ORDER")].Controls.Add(WTSUtility.CreateGridTextBox("SORT_ORDER", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("SORT_ORDER")].Text.Replace("&nbsp;", " ").Trim()),true));
            DateTime nDate = new DateTime();

            DateTime.TryParse(Server.HtmlDecode(row.Cells[DCC.IndexOf("PlannedDevTestStart")].Text.Replace("&nbsp;", " ").Trim()), out nDate);
            row.Cells[DCC.IndexOf("PlannedDevTestStart")].Controls.Add(WTSUtility.CreateGridTextBox("PlannedDevTestStart", itemId, nDate > DateTime.MinValue ? Server.HtmlDecode(String.Format("{0:MM/dd/yyyy}", nDate)) : ""));

            DateTime.TryParse(Server.HtmlDecode(row.Cells[DCC.IndexOf("PlannedEnd")].Text.Replace("&nbsp;", " ").Trim()), out nDate);
            row.Cells[DCC.IndexOf("PlannedEnd")].Controls.Add(WTSUtility.CreateGridTextBox("PlannedEnd", itemId, nDate > DateTime.MinValue ? Server.HtmlDecode(String.Format("{0:MM/dd/yyyy}", nDate)) : ""));

            bool visible = false;
            if (row.Cells[DCC.IndexOf("Visible")].HasControls()
                && row.Cells[DCC.IndexOf("Visible")].Controls[0] is CheckBox)
            {
                visible = ((CheckBox)row.Cells[DCC.IndexOf("Visible")].Controls[0]).Checked;
            }
            else if (row.Cells[DCC.IndexOf("Visible")].Text == "1")
            {
                visible = true;
            }
            row.Cells[DCC.IndexOf("Visible")].Controls.Clear();
            row.Cells[DCC.IndexOf("Visible")].Controls.Add(WTSUtility.CreateGridCheckBox("Visible", itemId, visible));
            row.Cells[DCC.IndexOf("Visible")].Style["width"] = "55px";

            bool archive = false;
            if (row.Cells[DCC.IndexOf("Archive")].HasControls()
                && row.Cells[DCC.IndexOf("Archive")].Controls[0] is CheckBox)
            {
                archive = ((CheckBox)row.Cells[DCC.IndexOf("Archive")].Controls[0]).Checked;
            }
            else if (row.Cells[DCC.IndexOf("Archive")].Text == "1")
            {
                archive = true;
            }
            row.Cells[DCC.IndexOf("Archive")].Controls.Clear();
            row.Cells[DCC.IndexOf("Archive")].Controls.Add(WTSUtility.CreateGridCheckBox("Archive", itemId, archive));
            row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
        }

        //row.Cells[DCC.IndexOf("EnterpriseAllocation")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("EnterpriseAllocation")].Text, row.Cells[DCC.IndexOf("WTS_RESOURCEID")].Text, row.Cells[DCC.IndexOf("USERNAME")].Text));

        TextBox obj = WTSUtility.CreateGridTextBox("Narrative", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Narrative")].Text.Replace("&nbsp;", " ").Trim()), false, true);
        if (!CanEdit)
        {
            obj.ReadOnly = true;
            obj.ForeColor = System.Drawing.Color.Gray;
        }
        row.Cells[DCC.IndexOf("Narrative")].Controls.Add(obj);

        if (!CanEdit)
        {
            Image imgBlank = new Image();
            imgBlank.Height = 10;
            imgBlank.Width = 10;
            imgBlank.ImageUrl = "Images/Icons/blank.png";
            imgBlank.AlternateText = "";
            row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
        }
        else
        {
            //row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("USERNAME")].Text.Trim() + " - " + row.Cells[DCC.IndexOf("AORRoleName")].Text.Trim()));
        }

        int aorCount = 0;
        int.TryParse(row.Cells[DCC.IndexOf("AORCount")].Text, out aorCount);

        if (!string.IsNullOrEmpty(itemId) && itemId != "0")
        {

            //add expand/collapse buttons
            HtmlGenericControl divChildren = new HtmlGenericControl();
            divChildren.Style["display"] = "table-row";
            divChildren.Style["text-align"] = "right";
            HtmlGenericControl divChildrenButtons = new HtmlGenericControl();
            divChildrenButtons.Style["display"] = "table-cell";
            divChildrenButtons.Controls.Add(createShowHideButton(true, "Show", itemId));
            divChildrenButtons.Controls.Add(createShowHideButton(false, "Hide", itemId));
            HtmlGenericControl divChildCount = new HtmlGenericControl();
            divChildCount.InnerText = string.Format("({0})", aorCount.ToString());
            divChildCount.Style["display"] = "table-cell";
            divChildCount.Style["padding-left"] = "2px";
            divChildren.Controls.Add(divChildrenButtons);
            divChildren.Controls.Add(divChildCount);
            //buttons to show/hide child grid
            row.Cells[DCC["X"].Ordinal].Controls.Clear();
            row.Cells[DCC["X"].Ordinal].Controls.Add(divChildren);

            //add child grid row for Task Items
            Table tableStage = (Table)row.Parent;
            GridViewRow childRowStage = createChildRow(itemId, "Stage");
            tableStage.Rows.AddAt(tableStage.Rows.Count, childRowStage);

            Table tableSession = (Table)row.Parent;
            GridViewRow childRowSession = createChildRow(itemId, "Session");
            tableSession.Rows.AddAt(tableSession.Rows.Count, childRowSession);
        }

        string risk = row.Cells[DCC.IndexOf("Risk")].Text;
        if (risk.Contains("High")) row.Cells[DCC.IndexOf("Risk")].Style["background-color"] = "red";
        else if (risk.Contains("Moderate")) row.Cells[DCC.IndexOf("Risk")].Style["background-color"] = "yellow";
        else row.Cells[DCC.IndexOf("Risk")].Style["background-color"] = "limegreen";
    }

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtMD_System_Resource"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_System_Resource"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("ReleaseScheduleDeliverable")
                && i != DCC.IndexOf("AORCount")
                && i != DCC.IndexOf("ContractCount")
                && i != DCC.IndexOf("Description")
                && i != DCC.IndexOf("Visible") 
                && i != DCC.IndexOf("ARCHIVE"))
            {
                row.Cells[i].Style["text-align"] = "left";
                row.Cells[i].Style["padding-left"] = "5px";
            }
            else
            {
                row.Cells[i].Style["text-align"] = "center";
                row.Cells[i].Style["padding-left"] = "0px";
            }
        }

        row.Cells[DCC.IndexOf("X")].Style["width"] = "32px";
        row.Cells[DCC.IndexOf("ReleaseScheduleDeliverable")].Style["width"] = "50px";
        row.Cells[DCC.IndexOf("AORCount")].Style["width"] = "65px";
        row.Cells[DCC.IndexOf("ContractCount")].Style["width"] = "65px";
        row.Cells[DCC.IndexOf("Description")].Style["width"] = "400px";
        row.Cells[DCC.IndexOf("PlannedDevTestStart")].Style["width"] = "125px";
        row.Cells[DCC.IndexOf("PlannedEnd")].Style["width"] = "125px";
        row.Cells[DCC.IndexOf("Risk")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("Avg. Resources")].Style["text-align"] = "center";

    }

    Image createShowHideButton(bool show = false, string direction = "Show", string itemId = "ALL")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgShowHideChildren_click(this,'{0}','{1}');", direction, itemId);

        Image img = new Image();
        img.ID = string.Format("img{0}Children_{1}", direction, itemId);
        img.Style["display"] = show ? "block" : "none";
        img.Style["cursor"] = "pointer";
        img.Attributes.Add("Name", string.Format("img{0}", direction));
        img.Attributes.Add("itemId", itemId);
        img.Height = 10;
        img.Width = 10;
        img.ImageUrl = direction.ToUpper() == "SHOW"
            ? "Images/Icons/add_blue.png"
            : "Images/Icons/minus_blue.png";
        img.ToolTip = string.Format("{0} Items for [{1}]", direction, itemId);
        img.AlternateText = string.Format("{0} Items for [{1}]", direction, itemId);
        img.Attributes.Add("Onclick", sb.ToString());

        return img;
    }

    GridViewRow createChildRow(string itemId = "", string itemNm = "")
    {
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
        TableCell tableCell = null;

        try
        {
            row.CssClass = "gridBody";
            row.Style["display"] = "none";
            row.ID = string.Format("gridChild{0}_{1}", itemNm, itemId);
            row.Attributes.Add("ReleaseScheduleID", itemId);
            row.Attributes.Add("Name", string.Format("gridChild{0}_{1}", itemNm, itemId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC["X"].Ordinal)
                {
                    //set width to match parent
                    tableCell.Style["width"] = "32px";
                    tableCell.Style["border-right"] = "1px solid transparent";
                }
                else if (i == DCC["ReleaseScheduleDeliverable"].Ordinal)
                {
                    tableCell.Style["padding-top"] = "10px";
                    tableCell.Style["padding-right"] = "10px";
                    tableCell.Style["padding-bottom"] = "0px";
                    tableCell.Style["padding-left"] = "0px";
                    tableCell.Style["vertical-align"] = "top";
                    tableCell.ColumnSpan = DCC.Count - 1;
                    //add the frame here
                    tableCell.Controls.Add(createChildFrame(itemId: itemId, itemNm: itemNm));
                }
                else
                {
                    tableCell.Style["display"] = "none";
                }

                if (itemNm == "Stage") tableCell.Style["border-bottom"] = "none";

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

    HtmlIframe createChildFrame(string itemId = "", string itemNm = "")
    {
        HtmlIframe childFrame = new HtmlIframe();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        childFrame.ID = string.Format("frameChild{0}_{1}", itemNm, itemId);
        childFrame.Attributes.Add("ReleaseScheduleID", itemId);
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["height"] = "30px";
        childFrame.Style["width"] = "100%";
        childFrame.Style["border"] = "none";

        return childFrame;
    }

    private LinkButton CreateLink(string text, string deploymentID)
    {
        LinkButton lb = new LinkButton();

        lb.Text = text;
        lb.Attributes["onclick"] = string.Format("openContracts({0}); return false;", deploymentID);

        return lb;
    }
    #endregion Grid


    [WebMethod(true)]
    public static string SaveChanges(string releaseID, string rows)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
            , { "failed", "0" }
            , { "savedIds", "" }
            , { "failedIds", "" }
            , { "error", "" } };
        bool exists = false, saved = false;
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
                string deliverable = "", description = "", narrative = "", plannedStart = "", plannedDevTestStart = "", plannedEnd = "";
                int stageID = 0, releaseVersionID = 0, visible = 0, archive = 0, sortOrder = 0;

                //save
                foreach (DataRow dr in dtjson.Rows)
                {
                    tempMsg = string.Empty;
                    int.TryParse(dr["ReleaseScheduleID"].ToString(), out stageID);
                    deliverable = dr["ReleaseScheduleDeliverable"].ToString();
                    int.TryParse(dr["ProductVersionID"].ToString(), out releaseVersionID);
                    description = dr["Description"].ToString();
                    narrative = dr["Narrative"].ToString();
                    plannedStart = dr["PlannedStart"].ToString();
                    plannedDevTestStart = dr["PlannedDevTestStart"].ToString();
                    plannedEnd = dr["PlannedEnd"].ToString();
                    int.TryParse(dr["Visible"].ToString(), out visible);
                    int.TryParse(dr["Archive"].ToString(), out archive);
                    int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);

                    result = MasterData.ReleaseSchedule_Deliverable_Update(stageID, deliverable, releaseVersionID, description, narrative, (visible == 1),
                        plannedStart, plannedEnd,
                        "", "",
                        "", "",
                        "", "",
                        "", "",
                        "", "",
                        "", "",
                        "", "",
                        plannedDevTestStart, "",
                        "", "",
                        "", "",
                        "", "",
                        "", "",
                        "", "",
                        "", "",
                        "", "",
                        "", "",
                        sortOrder, archive, 0);

                    if (result["saved"].ToString() == "True")
                    {
                        //ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
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

    [WebMethod(true)]
    public static string DeleteItem(int itemId)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "id", itemId.ToString() }
            , { "exists", "" }
            , { "deleted", "" }
            , { "error", "" } };
        bool exists = false, deleted = false;
        string errorMsg = string.Empty;

        try
        {
            //delete
            if (itemId == 0)
            {
                errorMsg = "You must specify an item to delete.";
            }
            else
            {
                deleted = MasterData.ReleaseSchedule_Deliverable_Delete(DeploymentID: itemId, exists: out exists, errorMsg: out errorMsg);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            deleted = false;
            errorMsg = ex.Message;
        }

        result["exists"] = exists.ToString();
        result["deleted"] = deleted.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }
}