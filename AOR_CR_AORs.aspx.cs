﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_CR_AORs : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected int CRID = 0;
    protected DataColumnCollection DCC;
    protected int GridPageIndex = 0;
    protected int RowCount = 0;
    protected string CurrentReleaseID;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);

        DataTable dt = LoadData();

        if (dt != null)
        {
            this.DCC = dt.Columns;
            this.RowCount = dt.Rows.Count;
        }

        grdData.DataSource = dt;

        if (!Page.IsPostBack && this.GridPageIndex > 0 && this.GridPageIndex < ((decimal)dt.Rows.Count / (decimal)25)) grdData.PageIndex = this.GridPageIndex;

        grdData.DataBind();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
            || Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
        {
            this.MyData = true;
        }
        else
        {
            this.MyData = false;
        }

        if (Request.QueryString["CRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CRID"]))
        {
            int.TryParse(Request.QueryString["CRID"], out this.CRID);
        }

        if (Request.QueryString["GridPageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndex"]))
        {
            int.TryParse(Request.QueryString["GridPageIndex"], out this.GridPageIndex);
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

        if (IsPostBack && Session["dtCRAOR"] != null)
        {
            dt = (DataTable)Session["dtCRAOR"];
        }
        else
        {
            dt = AOR.AORCRList_Get(AORID: 0, AORReleaseID: 0, CRID: this.CRID);
            Session["dtCRAOR"] = dt;
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

        if (row.RowIndex == 0) this.CurrentReleaseID = row.Cells[DCC.IndexOf("ProductVersion_ID")].Text;

        if (this.CurrentReleaseID != row.Cells[DCC.IndexOf("ProductVersion_ID")].Text)
        {
            for (int i = 0; i < row.Cells.Count; i++)
            {
                row.Cells[i].Style["border-top"] = "1px solid blue";
            }

            this.CurrentReleaseID = row.Cells[DCC.IndexOf("ProductVersion_ID")].Text;
        }

        FormatRowDisplay(ref row);

        if (DCC.Contains("AORReleaseCR_ID"))
        {
            row.Attributes.Add("aorreleasecr_id", row.Cells[DCC.IndexOf("AORReleaseCR_ID")].Text);
        }

        if (DCC.Contains("AOR #") && DCC.Contains("AORRelease_ID"))
        {
            row.Attributes.Add("aor_id", row.Cells[DCC.IndexOf("AOR #")].Text);
            row.Attributes.Add("aorrelease_id", row.Cells[DCC.IndexOf("AORRelease_ID")].Text);

            if (this.CanViewAOR)
            {
                row.Cells[DCC.IndexOf("AOR #")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("AOR #")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("AOR #")].Text, row.Cells[DCC.IndexOf("AORRelease_ID")].Text));
            }
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
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
        }

        if (DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "45px";
        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["width"] = "500px";
        if (DCC.Contains("Description")) row.Cells[DCC.IndexOf("Description")].Style["width"] = "500px";
        if (DCC.Contains("Release")) row.Cells[DCC.IndexOf("Release")].Style["width"] = "140px";

        if (DCC.Contains("Z")) row.Cells[DCC.IndexOf("Z")].Text = "";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }

        if (DCC.Contains("Release")) row.Cells[DCC.IndexOf("Release")].Style["text-align"] = "center";
    }

    private LinkButton CreateLink(string AOR_ID, string AORRelease_ID)
    {
        LinkButton lb = new LinkButton();

        lb.Text = AOR_ID;
        lb.Attributes["onclick"] = string.Format("openAOR('{0}', '{1}'); return false;", AOR_ID, AORRelease_ID);

        return lb;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string DeleteCRAOR(string aorReleaseCR)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int AORReleaseCR_ID = 0;
            int.TryParse(aorReleaseCR, out AORReleaseCR_ID);

            deleted = AOR.AORCR_Delete(AORReleaseCRID: AORReleaseCR_ID);
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

    [WebMethod()]
    public static string CanEdit(string aor, string aorRelease)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "canEdit", "" }, { "error", "" } };
        bool canEdit = false;
        string errorMsg = string.Empty;

        try
        {
            int AOR_ID = 0, AORRelease_ID = 0;
            int.TryParse(aor, out AOR_ID);
            int.TryParse(aorRelease, out AORRelease_ID);

            canEdit = (UserManagement.UserCanEdit(WTSModuleOption.AOR) && AOR.AORReleaseCurrent(AORID: AOR_ID, AORReleaseID: AORRelease_ID));
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            canEdit = false;
            errorMsg = ex.Message;
        }

        result["canEdit"] = canEdit.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}