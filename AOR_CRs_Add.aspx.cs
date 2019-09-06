using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_CRs_Add : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected int AORID = 0;
    private DataColumnCollection DCC;
    protected string[] QFSystem = { };
    protected string[] QFRelease = { };
    protected string[] QFCRContract = { };
    protected string[] QFStatus = { };
    protected string QFName = "";
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);

        DataTable dt = LoadData();
        if (dt != null) this.DCC = dt.Columns;

        grdData.DataSource = dt;
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

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.AORID);
        }

        if (Request.QueryString["SelectedSystems"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSystems"]))
            this.QFSystem = Request.QueryString["SelectedSystems"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["SelectedReleases"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedReleases"]))
            this.QFRelease = Request.QueryString["SelectedReleases"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["SelectedCRContracts"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedCRContracts"]))
            this.QFCRContract = Request.QueryString["SelectedCRContracts"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"]))
            this.QFStatus = Request.QueryString["SelectedStatuses"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (Request.QueryString["txtSearch"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["txtSearch"]))
            this.QFName = Request.QueryString["txtSearch"].Trim();
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

        if (IsPostBack && txtPostBackType.Text == "LoadGrid")
        {
            dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>((dynamic)txtAppliedFilters.Text);

            dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: 0, SRID: 0, CRID: 0, DeliverableID: 0, Type: "CR", Filters: fields, CRStatus: String.Join(",", QFStatus), CRContract: string.Join(",", QFCRContract), TaskID: string.Empty, QFSystem: String.Join(",", QFSystem), QFRelease: String.Join(",", QFRelease), QFName: QFName);
            txtPostBackType.Text = string.Empty;
        }
        else if (IsPostBack && Session["dtAORAddCR"] != null)
        {
            dt = (DataTable)Session["dtAORAddCR"];
        }
        else
        {
            dt = AOR.AORAddList_Get(AORID: this.AORID, AORReleaseID: 0, SRID: 0, CRID: 0, DeliverableID: 0, Type: "CR", Filters: null, CRStatus: String.Join(",", QFStatus), CRContract: string.Join(",", QFCRContract), TaskID: string.Empty, QFSystem: String.Join(",", QFSystem), QFRelease: String.Join(",", QFRelease), QFName: QFName);
        }

        Session["dtAORAddCR"] = dt;

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

        if (DCC.Contains("X") && DCC.Contains("CR_ID"))
        {
            if (this.CanEditAOR)
            {
                row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("CR_ID")].Text));
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

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = "35px";
        }

        if (DCC.Contains("CR Customer Title")) row.Cells[DCC.IndexOf("CR Customer Title")].Style["width"] = "300px";
        if (DCC.Contains("CR Contract")) row.Cells[DCC.IndexOf("CR Contract")].Style["width"] = "90px";
        if (DCC.Contains("Related Release")) row.Cells[DCC.IndexOf("Related Release")].Style["width"] = "100px";
        if (DCC.Contains("Status")) row.Cells[DCC.IndexOf("Status")].Style["width"] = "70px";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        if (DCC.Contains("CR Customer Title")) row.Cells[DCC.IndexOf("CR Customer Title")].Text = Uri.UnescapeDataString(row.Cells[DCC.IndexOf("CR Customer Title")].Text);
        if (DCC.Contains("CR Internal Title")) row.Cells[DCC.IndexOf("CR Internal Title")].Text = Uri.UnescapeDataString(row.Cells[DCC.IndexOf("CR Internal Title")].Text);

        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }
    }

    private CheckBox CreateCheckBox(string value)
    {
        CheckBox chk = new CheckBox();

        chk.Attributes["onchange"] = "input_change(this);";
        chk.Attributes.Add("cr_id", value);

        return chk;
    }
    #endregion
}