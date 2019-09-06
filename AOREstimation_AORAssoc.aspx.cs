using System;
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

public partial class AOREstimation_AORAssoc : System.Web.UI.Page
{
    #region "Member Variables"
    protected int _AORReleaseID = 0;
    protected int _AORID = 0;
    protected int _AOREstimation_AORReleaseID = 0;

    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected int DeliverableID = 1;
    protected DataColumnCollection DCC;
    protected int GridPageIndex = 0;
    protected int RowCount = 0;
    protected string CurrentReleaseID;

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);

        DataTable dt = LoadData();

        if (dt != null)
        {
            this.DCC = dt.Columns;
            this.RowCount = dt.Rows.Count - 1;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
        }

        grdData.DataSource = dt;

        if (!Page.IsPostBack && this.GridPageIndex > 0 && this.GridPageIndex < ((decimal)dt.Rows.Count / (decimal)25)) grdData.PageIndex = this.GridPageIndex;

        grdData.DataBind();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this._AORReleaseID);
        }

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this._AORID);
        }

        if (Request.QueryString["AOREstimation_AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AOREstimation_AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AOREstimation_AORReleaseID"], out this._AOREstimation_AORReleaseID);
        }
    }

    private void InitializeEvents()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;
    }

    #region "Load Data"
    private DataTable LoadData()
    {
        DataTable dt = new DataTable();
        
        dt = AOR.AOREstimation_Assoc_Get(AOREstimation_AORReleaseID: _AOREstimation_AORReleaseID);
        Session["dtAORAssoc"] = dt;

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

        if (DCC.Contains("AOREstimation_AORAssocID"))
        {
            row.Attributes.Add("AOREstimation_AORAssocID", row.Cells[DCC.IndexOf("AOREstimation_AORAssocID")].Text);
        }

        //if (DCC.Contains("AOR #") && DCC.Contains("AORRelease_ID"))
        //{
        //    row.Attributes.Add("aor_id", row.Cells[DCC.IndexOf("AOR #")].Text);
        //    row.Attributes.Add("aorrelease_id", row.Cells[DCC.IndexOf("AORRelease_ID")].Text);

        //    if (this.CanViewAOR)
        //    {
        //        row.Cells[DCC.IndexOf("AOR #")].Style["text-align"] = "center";
        //        row.Cells[DCC.IndexOf("AOR #")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("AOR #")].Text, row.Cells[DCC.IndexOf("AORRelease_ID")].Text));
        //    }
        //}
    }

    private void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdData.PageIndex = e.NewPageIndex;
    }

    private void FormatHeaderRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.Contains("AOREstimation_AORAssocID")) row.Cells[i].Style["display"] = "none";
        }

        if (DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "45px";
        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["width"] = "300px";
        if (DCC.Contains("Deployment(s)")) row.Cells[DCC.IndexOf("Deployment(s)")].Style["width"] = "100px";
        if (DCC.Contains("Avg. Resources")) row.Cells[DCC.IndexOf("Avg. Resources")].Style["width"] = "200px";
        if (DCC.Contains("Notes")) row.Cells[DCC.IndexOf("Notes")].Style["width"] = "300px";

        if (DCC.Contains("X")) row.Cells[DCC.IndexOf("X")].Text = "";
        if (DCC.Contains("Z")) row.Cells[DCC.IndexOf("Z")].Text = "";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.Contains("AOREstimation_AORAssocID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }

        string itemId = row.Cells[DCC.IndexOf("AOREstimation_AORAssocID")].Text.Trim();
        row.Cells[DCC.IndexOf("Notes")].Controls.Add(WTSUtility.CreateGridTextBox(field: "Notes", itemId: itemId, text: Server.HtmlDecode(row.Cells[DCC.IndexOf("Notes")].Text.Replace("&nbsp;", " ").Trim())));

        if (DCC.Contains("Deployment")) row.Cells[DCC.IndexOf("Deployment")].Style["text-align"] = "center";
        if (DCC.Contains("Primary")) row.Cells[DCC.IndexOf("Primary")].Style["text-align"] = "center";
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
    public static string DeleteAORAssoc(string aorEstimation_Assoc)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int AOREstimationAssocID = 0;
            int.TryParse(aorEstimation_Assoc, out AOREstimationAssocID);

            deleted = AOR.AOREstimation_Assoc_Delete(AOREstimationAORAssocID: AOREstimationAssocID);
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
    public static string SetPrimaryAORAssoc(string aorEstimation_Assoc)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            int AOREstimationAssocID = 0;
            int.TryParse(aorEstimation_Assoc, out AOREstimationAssocID);

            saved = AOR.AOREstimation_Assoc_SetPrimary(AOREstimationAORAssocID: AOREstimationAssocID);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            saved = false;
            errorMsg = ex.Message;
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod(true)]
    public static string SaveChanges(string rows)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
            , { "failed", "0" }
            , { "savedIds", "" }
            , { "failedIds", "" }
            , { "error", "" } };
        bool exists = false, saved = false, duplicate = false;
        int savedQty = 0, failedQty = 0;
        string ids = string.Empty, failedIds = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
            if (dtjson == null || dtjson.Rows.Count == 0)
            {
                errorMsg = "Unable to save. No list of changes was provided.";
            }
            else
            {
                int id = 0;
                string name = string.Empty, description = string.Empty, notes = string.Empty;

                HttpServerUtility server = HttpContext.Current.Server;
                //save
                foreach (DataRow dr in dtjson.Rows)
                {
                    id = 0;
                    name = string.Empty;
                    description = string.Empty;
                    notes = string.Empty;

                    tempMsg = string.Empty;
                    int.TryParse(dr["AOREstimation_AORAssocID"].ToString(), out id);
                    notes = server.UrlDecode(dr["Notes"].ToString());

                    saved = AOR.AOREstimation_Assoc_Update(
                        AOREstimation_AORAssocID: id, Notes: notes, duplicate: out duplicate, errorMsg: out tempMsg);
                   
                    if (saved)
                    {
                        savedQty += 1;
                        ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
                    }

                    else
                    {
                        failedQty += 1;
                        failedIds += string.Format("{0}{1}", failedIds.Length > 0 ? "," : "", failedIds.ToString());
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
            LogUtility.LogException(ex);
            saved = false;
            errorMsg = ex.Message;
        }

        result["savedIds"] = ids;
        result["failedIds"] = failedIds;
        result["saved"] = savedQty.ToString();
        result["failed"] = failedQty.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }
    #endregion
}