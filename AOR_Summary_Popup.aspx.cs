using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

public partial class AOR_Summary_Popup : System.Web.UI.Page
{
    #region Variables
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected string Title = string.Empty;
    protected string Type = string.Empty;
    protected string Alert = string.Empty;
    protected int AORID = 0;
    protected int AORReleaseID = 0;
    private DataColumnCollection DCC;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);

        switch (Type)
        {
            case "Alert":
                this.Title = this.Alert;
                break;
            default:
                this.Title = this.Type;
                break;
        }

        DataTable dt = LoadData();
        if (dt != null) this.DCC = dt.Columns;

        grdData.DataSource = dt;
        grdData.DataBind();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["Type"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Type"]))
        {
            this.Type = Request.QueryString["Type"];
        }

        if (Request.QueryString["Alert"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Alert"]))
        {
            this.Alert = Request.QueryString["Alert"];
        }

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.AORID);
        }

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.AORReleaseID);
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
    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

        if (IsPostBack && Session["dtSummary"] != null)
        {
            dt = (DataTable)Session["dtSummary"];
        }
        else
        {
            if (this.Type == "AOR Alert")
            {
                dt = AOR.AORAlertList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID);
            }
            else
            {
                DataSet dsSummary = AOR.AORSummaryList_Get(AlertType: this.Alert, AORID: 0, AORReleaseID: 0, ReleaseIDs: "", DeliverableIDs: "", ContractIDs: "");

                if (dsSummary != null)
                {
                    switch (this.Type)
                    {
                        case "Alert":
                            dt = dsSummary.Tables["Alert"];
                            break;
                    }
                }
            }

            Session["dtSummary"] = dt;
        }

        return dt;
    }
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatRowDisplay(ref row);

        if (this.Type != "AOR Alert" && DCC.Contains("AOR #"))
        {
            row.Cells[DCC.IndexOf("AOR #")].Style["text-align"] = "center";
            if (this.CanViewAOR) row.Cells[DCC.IndexOf("AOR #")].Controls.Add(CreateLink("Alert", row.Cells[DCC.IndexOf("AOR #")].Text));
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
            if (DCC[i].ColumnName.EndsWith("_ID") && !(this.Type == "AOR Alert" && DCC[i].ColumnName == "Alert_ID")) row.Cells[i].Style["display"] = "none";
        }

        if (this.Type == "AOR Alert" && DCC.Contains("Alert_ID")) row.Cells[DCC.IndexOf("Alert_ID")].Text = "Alert";
        if (this.Type == "AOR Alert" && DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["display"] = "none";
        if (this.Type == "AOR Alert" && DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["display"] = "none";

        if (DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "55px";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID") && !(this.Type == "AOR Alert" && DCC[i].ColumnName == "Alert_ID")) row.Cells[i].Style["display"] = "none";
        }

        if (this.Type == "AOR Alert" && DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["display"] = "none";
        if (this.Type == "AOR Alert" && DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["display"] = "none";
    }

    private LinkButton CreateLink(string type, string value)
    {
        LinkButton lb = new LinkButton();

        lb.Text = value;

        switch (type)
        {
            case "Alert":
                lb.Attributes["onclick"] = string.Format("openAOR('{0}'); return false;", value);
                break;
        }

        return lb;
    }
    #endregion
}