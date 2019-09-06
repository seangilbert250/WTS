using System;
using System.Web.UI.WebControls;
using System.Data;
using System.Web;
using Newtonsoft.Json;

public partial class Dashboard : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected DataColumnCollection DCCWT;
    protected GridCols columnData = new GridCols();

    protected string assignedToRanks = string.Empty;
    protected string assignedToRankLabels = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        initControls();
        loadGridData();
    }
    private void initControls()
    {
        grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
        grdMD.GridRowDataBound += grdMD_GridRowDataBound;
        grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;

        //gridNews.GridHeaderRowDataBound += gridNews_GridHeaderRowDataBound;
        //gridNews.GridRowDataBound += gridNews_GridRowDataBound;
        //gridNews.GridPageIndexChanging += gridNews_GridPageIndexChanging;

        Image1.ImageUrl = "Images/fav_icon.ico";

    }

    protected void LoadNewsItems()
    {
        DataTable dtNews = null;
        dtNews = WTSNews.GetNews();
        HttpContext.Current.Session["dtNews"] = dtNews;
        Page.ClientScript.RegisterArrayDeclaration("DCC", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
        InitializeColumnData(ref dtNews);
        dtNews.AcceptChanges();

        if (dtNews != null)
        {
            DCC = dtNews.Columns;
        }

       // gridNewsOverview.DataSource = (DataTable)Session["dtNews"];
        //gridNewsOverview.DataBind();
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
                    //case "X":
                    //    displayName = "&nbsp;";
                    //    blnVisible = true;
                    //    break;
                    //case "Y":
                    //    displayName = "&nbsp;";
                    //    blnVisible = true;
                    //    break;
                    case "NewsID":
                        displayName = "NewsID";
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "Start_Date":
                        displayName = "Start Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "End_Date":
                        displayName = "End Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    //case "Summary":
                    //    displayName = "Summary";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    break;
                    //case "NewsType":
                    //    displayName = "Type";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    break;
                    //case "Bln_Active":
                    //    displayName = "Active";
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    break;
                }

                columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
                columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
            }

            //Initialize the columnData
            columnData.Initialize(ref dt, ";", "~", "|");

            ////Get sortable columns and default column order
            //SortableColumns = columnData.SortableColumnsToString();
            //DefaultColumnOrder = columnData.DefaultColumnOrderToString();
            ////Sort and Reorder Columns
            //columnData.ReorderDataTable(ref dt, ColumnOrder);
            //columnData.SortDataTable(ref dt, SortOrder);
            //SelectedColumnOrder = columnData.CurrentColumnOrderToString();

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    private void loadGridData()
    {
        DataTable dt = null;
        dt = WTS_Reports.GetDashboardData();

        if (dt != null)
        {
            this.DCC = dt.Columns;
            assignedToRanks = dt.Rows[0]["Emergency Workload"].ToString();
            assignedToRanks += "," + dt.Rows[0]["Current Workload"].ToString();
            assignedToRanks += "," + dt.Rows[0]["Run The Business"].ToString();
            assignedToRanks += "," + dt.Rows[0]["Staged Workload"].ToString();
            assignedToRanks += "," + dt.Rows[0]["Unprioritized Workload"].ToString();

            assignedToRankLabels = "Emergency Workload,Current Workload,Run The Business,Staged Workload,Unprioritized Workload";
        }

        lblDateTime.InnerText = "My Work Tasks as of " + DateTime.Now.ToLongDateString() + ' ' + DateTime.Now.ToLongTimeString() + ':';

        //grdMD.DataSource = dt;
        //grdMD.DataBind();
    }
    private void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            row.Cells[i].Style["text-align"] = "left";
            row.Cells[i].Style["padding-left"] = "5px";
        }
        row.Cells[DCC.IndexOf("Count")].Style["width"] = "60px";
    }

    void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);
    }
    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
    }
    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        loadGridData();
    }
}