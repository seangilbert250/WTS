using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Workload_Concerns : System.Web.UI.Page
{
    protected DataColumnCollection DCC;

    protected void Page_Load(object sender, EventArgs e)
    {
        initGrid();
        loadWorkConcerns();
    }


    void initGrid()
    {
        gridConcern.RowDataBound += gridConcern_RowDataBound;
       // gridConcern.PageIndexChanging += gridConcern_PageIndexChanging;
    }

    private void loadWorkConcerns()
    {
        DataTable dt = null;
        
        try
        {
            dt = WorkloadItem.Concerns_Get();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            dt = null;
        }

        if (dt != null)
        {
            this.DCC = dt.Columns;
        }

        gridConcern.DataSource = dt;
        gridConcern.DataBind();
    }

    #region Grid
    void gridConcern_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            gridConcern_GridHeaderRowDataBound(sender, e);
        }
        else if (e.Row.RowType == DataControlRowType.DataRow)
        {
            gridConcern_GridRowDataBound(sender, e);
        }
    }

    void gridConcern_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        var todayString = DateTime.Now.ToString("M/d/yyyy");
        DateTime todayDate = DateTime.Parse(todayString);
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i == DCC.IndexOf("Longest # of Days Awaiting Closure"))
            {
                var dateString = row.Cells[DCC.IndexOf("Longest # of Days Awaiting Closure")].Text;
                DateTime deployDate = DateTime.Parse(dateString);
                string longestDays = ((todayDate - deployDate).TotalDays).ToString();
                var test = longestDays.IndexOf(".");
                longestDays = longestDays.Substring(0,longestDays.IndexOf("."));
                row.Cells[DCC.IndexOf("Longest # of Days Awaiting Closure")].Text = longestDays;
            }
        }
    }

    void gridConcern_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i == DCC.IndexOf("Concerns"))
            {
                row.Cells[i].Style["text-align"] = "left";
                row.Cells[i].Style["padding-left"] = "5px";
            }
            else
            {
                row.Cells[i].Style["text-align"] = "center";
            }
        }
    }
    #endregion Grid
}