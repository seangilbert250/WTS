﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using WTS.Enums;


public partial class News : System.Web.UI.Page
{
    protected string RootUrl = string.Empty;

    protected bool _refreshData = false;
    protected bool _export = false;

    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;

    protected bool CanEdit = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        int idxQueryString = Request.Url.OriginalString.IndexOf(Request.Url.Query);
        if (idxQueryString > 0)
        {
            RootUrl = Request.Url.OriginalString.Substring(0, idxQueryString);
        }
        else
        {
            RootUrl = Request.Url.OriginalString;
        }

        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.News);

        initControls();
        LoadNewsItems();
    }

    private void initControls()
    {
        gridNews.GridHeaderRowDataBound += gridNews_GridHeaderRowDataBound;
        gridNews.GridRowDataBound += gridNews_GridRowDataBound;
        gridNews.GridPageIndexChanging += gridNews_GridPageIndexChanging;
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

        gridNews.DataSource = (DataTable)Session["dtNews"];
        gridNews.DataBind();
    }

    private DataTable applyGridSorting(DataTable dt, string sortValue)
    {
        if (!string.IsNullOrWhiteSpace(sortValue))
        {
            dt.DefaultView.Sort = sortValue;
            dt = dt.DefaultView.ToTable();
        }

        return dt;
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
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                    case "Y":
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
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
                    case "Summary":
                        displayName = "Summary";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "NewsType":
                        displayName = "Type";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Bln_Active":
                        displayName = "Active";
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
    void gridNews_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);
    }

    void gridNews_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        string newsId = row.Cells[DCC["NewsID"].Ordinal].Text.Trim();
        e.Row.Attributes.Add("newsId", newsId);

        //string AttachmentId = row.Cells[DCC["AttachmentId"].Ordinal].Text.Trim();
        //e.Row.Attributes.Add("attachmentId", AttachmentId);

        string NewsTypeId = row.Cells[DCC["NewsTypeID"].Ordinal].Text.Trim();
        e.Row.Attributes.Add("newsTypeId", NewsTypeId);

        //if (row.Cells[DCC.IndexOf("NEWSTYPEID")].Text == "1")
        //{
        //    row.Cells[DCC.IndexOf("NEWSTYPEID")].Text = "News Article";
        //}
        //if (row.Cells[DCC.IndexOf("Bln_News")].Text == "0")
        //{
        //    row.Cells[DCC.IndexOf("Bln_News")].Text = "Sliding Sys Notification";
        //}

        if (row.Cells[DCC.IndexOf("Bln_Active")].Text == "0")
        {
            row.Cells[DCC.IndexOf("Bln_Active")].Text = "Inactive";
        }
        if (row.Cells[DCC.IndexOf("Bln_Active")].Text == "1")
        {
            row.Cells[DCC.IndexOf("Bln_Active")].Text = "Active";
        }


        Table table = (Table)row.Parent;
        GridViewRow detailsRow = createDetailsRow(newsId, row.Cells[DCC.IndexOf("Detail")].Text.Trim());
        table.Rows.AddAt(table.Rows.Count, detailsRow);
    }

    private Image createStatusImage(string newsId = "", bool viewed = false, bool updated = false)
    {
        Image imgReadStatus = new Image();
        imgReadStatus.ID = "imgReadStatus_" + newsId;
        imgReadStatus.ImageUrl = "images/blank.png";
        imgReadStatus.BackColor = System.Drawing.Color.Transparent;
        imgReadStatus.Height = new Unit(16, UnitType.Pixel);
        imgReadStatus.Width = new Unit(16, UnitType.Pixel);
        imgReadStatus.Style["background-image"] = "images/news.png";

        if (!viewed && !updated)
        {
            imgReadStatus.Style["background-position"] = "-16px 0px;";
            imgReadStatus.AlternateText = "New Article - click to read details";
            imgReadStatus.ToolTip = "New Article - click to read details";
        }
        else
        {
            if (!viewed && updated)
            {
                imgReadStatus.Style["background-position"] = "-32px 0px;";
                imgReadStatus.AlternateText = "Updated Article - click to read details";
                imgReadStatus.ToolTip = "Updated Article - click to read details";
            }
            else
            {
                imgReadStatus.Style["background-position"] = "0px 0px;";
                imgReadStatus.AlternateText = "Click to re-read details";
                imgReadStatus.ToolTip = "Click to re-read details";
            }
        }

        return imgReadStatus;
    }

    private GridViewRow createDetailsRow(string newsId = "0", string details = "")
    {
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
        TableCell tableCell = null;

        try
        {
            row.CssClass = "selectedRow_News";
            row.Style["display"] = "none";
            row.ID = string.Format("gridBody_Details_{0}", newsId);
            row.Attributes.Add("parentNewsId", newsId);
            row.Attributes.Add("Name", string.Format("newsDetails_{0}", newsId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC.IndexOf("Start_Date"))
                {
                    //do nothing
                }
                else if (i == DCC.IndexOf("Detail"))
                {
                    HtmlGenericControl divDetails = new HtmlGenericControl(string.Format("details_{0}", newsId));
                    divDetails.InnerHtml = Server.HtmlDecode(details.Trim());
                    tableCell.ColumnSpan = 6;
                    tableCell.Controls.Add(divDetails);
                }
                else
                {
                    tableCell.Style["display"] = "none";
                }

                row.Cells.Add(tableCell);
            }
        }
        catch (Exception)
        {
            row = null;
        }

        return row;
    }

    void gridNews_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gridNews.PageIndex = e.NewPageIndex;
        if (Session["dtNews"] == null)
        {
            LoadNewsItems();
        }
        else
        {
            gridNews.DataSource = (DataTable)Session["dtNews"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("X"))
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

        //more column formatting
        row.Cells[DCC.IndexOf("X")].Style["width"] = "5px";
        row.Cells[DCC.IndexOf("Y")].Style["width"] = "40px";
        row.Cells[DCC.IndexOf("Start_Date")].Width = new Unit(50, UnitType.Pixel);
        row.Cells[DCC.IndexOf("End_Date")].Width = new Unit(50, UnitType.Pixel);
        row.Cells[DCC.IndexOf("Summary")].Width = new Unit(100, UnitType.Pixel);
        row.Cells[DCC.IndexOf("NewsType")].Width = new Unit(50, UnitType.Pixel);
        row.Cells[DCC.IndexOf("Bln_Active")].Width = new Unit(50, UnitType.Pixel);

        //row.Cells[DCC.IndexOf("STARTDATE")].Width = new Unit(75, UnitType.Pixel);
        //row.Cells[DCC.IndexOf("VIEW_DT")].Style["text-align"] = "center";
        //row.Cells[DCC.IndexOf("VIEW_DT")].Width = new Unit(20, UnitType.Pixel);
        //row.Cells[DCC.IndexOf("VIEW_DT")].Style["border-right"] = "none";
    }

    #endregion Grid


    private void exportToExcel(DataTable dt)
    {

    }


    [WebMethod()]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string ReadArticle(string newsId = "")
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        long newsArticleId = 0, userNewsId = 0;
        long.TryParse(newsId, out newsArticleId);

        bool saved = false;
        string errorMsg = string.Empty;
        try
        {
            //save the user_news record
            saved = WTSNews.MarkArticleRead(newsArticleId);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            saved = false;
            errorMsg += Environment.NewLine + ex.Message;
        }

        result.Add("saved", saved.ToString());
        result.Add("error", errorMsg);
        result.Add("id", userNewsId.ToString());

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
    public static string DeleteNews(string newsId)
    {
        string result = "";
        bool deleted = false;
        int intNewsId = -1;
        int.TryParse(newsId, out intNewsId);

        deleted = WTSNews.DeleteNews(intNewsId);
        result = deleted.ToString();

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
}