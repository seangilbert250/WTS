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

public partial class AOR_Scheduled_Deliverables_Stage_AORs : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;

    protected int ReleaseID = 0;
    protected int ReleaseScheduleDeliverableID = 0;

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;

    protected bool CanView = false;
    protected bool CanEdit = false;
    protected bool IsAdmin = false;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool hasData = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.IsAdmin = UserManagement.UserIsInRole("Admin");
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.Deployment);
        this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.Deployment));
        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);

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
        if (Request.QueryString["ReleaseScheduleDeliverableID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseScheduleDeliverableID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ReleaseScheduleDeliverableID"].ToString()), out this.ReleaseScheduleDeliverableID);
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
        DataTable dt = AOR.AORDeliverableList_Get(DeliverableID: this.ReleaseScheduleDeliverableID);

        if (dt != null)
        {
            if (dt.Rows.Count > 1) hasData = true;
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
                    case "AOR #":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "AOR Name":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Description":
                        displayName = "Description";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Risk":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Weight":
                        displayName = "Weight (%)";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Avg. Resources":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Z":
                        blnVisible = true;
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
        row.Cells[DCC.IndexOf("Z")].Text = "&nbsp;";
        row.Cells[DCC.IndexOf("AOR Name")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Description")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Avg. Resources")].Style["text-align"] = "center";
        
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);


        //add edit link
        string itemId = row.Cells[DCC.IndexOf("AORReleaseDeliverable_ID")].Text.Trim();
        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);
        row.Attributes.Add("aorid", row.Cells[DCC.IndexOf("AOR #")].Text.Trim());
        row.Attributes.Add("aorreleaseid", row.Cells[DCC.IndexOf("AORRelease_ID")].Text.Trim());

        if (CanEdit)
        {
            TextBox txt = WTSUtility.CreateGridTextBox("Weight", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Weight")].Text.Replace("&nbsp;", " ").Trim()), true);

            txt.MaxLength = 3;
            row.Cells[DCC.IndexOf("Weight")].Controls.Add(txt);
        }

        string risk = row.Cells[DCC.IndexOf("Risk")].Text;
        if (risk.Contains("High")) row.Cells[DCC.IndexOf("Risk")].Style["background-color"] = "red";
        else if (risk.Contains("Moderate")) row.Cells[DCC.IndexOf("Risk")].Style["background-color"] = "yellow";
        else row.Cells[DCC.IndexOf("Risk")].Style["background-color"] = "limegreen";

        if (this.CanViewAOR)
        {
            row.Cells[DCC.IndexOf("AOR #")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("AOR #")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("AOR #")].Text, row.Cells[DCC.IndexOf("AORRelease_ID")].Text));
        }

        row.Cells[DCC.IndexOf("Avg. Resources")].Style["text-align"] = "center";

        //add Total row to top of grid (as part of header to keep fixed)
        if (row.RowIndex == 0 && hasData)
        {
            FixedTotalRow(row);
        }
    }

    private void FixedTotalRow(GridViewRow row)
    {
        try
        {

            DataTable dt = grdMD.DataSource;

            GridViewRow nRow = new GridViewRow(row.RowIndex, row.RowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);
            TableHeaderCell nCell = new TableHeaderCell();
            nRow.Style["height"] = "25px";
            nRow.Attributes.Add("rowID", "total");
            int intColspan = 4;
            nCell = new TableHeaderCell();
            nCell.Text = "TOTAL(S)";
            nCell.ColumnSpan = intColspan;
            nCell.Style["background"] = "#d7e8fc";
            nRow.Cells.Add(nCell);

            int columnCount = 0;

            for (int i = 0; i <= DCC.Count - 1; i++)
            {
                string columnDBName = DCC[i].ColumnName.ToString();
                Boolean visible = DCC[i].ColumnName == "AORRelease_ID" || DCC[i].ColumnName == "ProductVersion_ID" || DCC[i].ColumnName == "Release" || DCC[i].ColumnName == "AORReleaseDeliverable_ID" ? false : true;

                if (visible)
                {
                    columnCount = columnCount + 1;
                }

                if (columnCount > intColspan && visible)
                {
                    nCell = new TableHeaderCell();
                    nCell.Text = DCC[i].ColumnName == "Risk" || DCC[i].ColumnName == "Weight" || DCC[i].ColumnName == "Avg. Resources" ? dt.Rows[0][columnDBName].ToString() : "&nbsp;";
                    ////Uncomment this if we want to color code Risk in Total row
                    //if (DCC[i].ColumnName == "Risk")
                    //{
                    //    if (nCell.Text == "Low (Routine)") { nCell.Style["background"] = "#32CD32"; }
                    //    if (nCell.Text == "Moderate (Acceptable)") { nCell.Style["background"] = "#FFFF00"; }
                    //    if (nCell.Text == "High (Emergency)") { nCell.Style["background"] = "#ff0000"; }
                    //}
                    //else
                    //{
                    //    nCell.Style["background"] = "#d7e8fc";
                    //}
                    nCell.Style["background"] = "#d7e8fc";
                    nRow.Cells.Add(nCell);
                }
            }

            grdMD.Controls[1].Controls[1].Controls[0].Controls[0].Controls.AddAt(1, nRow);
        }
        catch (Exception ex)
        {
        }
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
            if (i != DCC.IndexOf("AOR #")
                && i != DCC.IndexOf("Weight"))
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

        row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "50px";
        row.Cells[DCC.IndexOf("AOR Name")].Style["width"] = "450px";
        row.Cells[DCC.IndexOf("Risk")].Style["width"] = "100px";
        row.Cells[DCC.IndexOf("Weight")].Style["width"] = "75px";
        row.Cells[DCC.IndexOf("Z")].Style["width"] = "12px";
    }

    private LinkButton CreateLink(string AOR_ID, string AORRelease_ID)
    {
        LinkButton lb = new LinkButton();

        lb.Text = AOR_ID;
        lb.Attributes["onclick"] = string.Format("openAOR('{0}', '{1}'); return false;", AOR_ID, AORRelease_ID);

        return lb;
    }
    #endregion Grid


    [WebMethod(true)]
    public static string SaveChanges(string rows)
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
                int AORReleaseDeliverableID = 0, Weight = -999;

                //save
                foreach (DataRow dr in dtjson.Rows)
                {
                    tempMsg = string.Empty;
                    int.TryParse(dr["AORReleaseDeliverable_ID"].ToString(), out AORReleaseDeliverableID);
                    if (dr["Weight"].ToString() != "") int.TryParse(dr["Weight"].ToString(), out Weight);

                    result = AOR.AORReleaseDeliverable_Save(AORReleaseDeliverableID, Weight);

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

    [WebMethod()]
    public static string DeleteDeliverableAOR(string aorReleaseDeliverable)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int AORReleaseDeliverable_ID = 0;
            int.TryParse(aorReleaseDeliverable, out AORReleaseDeliverable_ID);

            deleted = AOR.AORDeliverable_Delete(AORReleaseDeliverableID: AORReleaseDeliverable_ID);
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
}