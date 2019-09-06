using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

public partial class RQMTDescription_Popup : System.Web.UI.Page
{
    #region Variables
    protected bool CanEditRQMTDescription = false;
    protected bool CanViewRQMTDescription = false;
    protected string Title = string.Empty;
    protected string Type = string.Empty;
    private DataColumnCollection DCC;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditRQMTDescription = UserManagement.UserCanEdit(WTSModuleOption.RQMT);
        this.CanViewRQMTDescription = this.CanEditRQMTDescription || UserManagement.UserCanView(WTSModuleOption.RQMT);

        switch (this.Type)
        {
            case "Add":
                this.Title = "Add RQMT Description";
                break;
        }

        LoadControls();
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
    }

    private void InitializeEvents()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;
    }
    #endregion

    #region Data
    private void LoadControls()
    {
        switch (this.Type)
        {
            case "Add":
                DataTable dtRQMTDescriptionType = RQMT.RQMTDescriptionTypeList_Get();

                ddlRQMTDescriptionType.DataSource = dtRQMTDescriptionType;
                ddlRQMTDescriptionType.DataValueField = "RQMTDescriptionTypeID";
                ddlRQMTDescriptionType.DataTextField = "RQMTDescriptionType";
                ddlRQMTDescriptionType.DataBind();
                break;
        }
    }

    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

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
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
        }
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string Save(string rqmtDescriptionType, string rqmtDescription)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

        try
        {
            int rqmtDescriptionType_ID = 0;

            int.TryParse(rqmtDescriptionType, out rqmtDescriptionType_ID);

            result = RQMT.RQMTDescription_Save(RQMTDescriptionTypeID: rqmtDescriptionType_ID, RQMTDescription: rqmtDescription);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}