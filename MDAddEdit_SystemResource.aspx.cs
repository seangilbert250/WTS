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

using Newtonsoft.Json;

public partial class MDAddEdit_SystemResource : System.Web.UI.Page
{
    #region Variables
    protected bool CanEdit = false;
    protected int ReleaseID = 0;
    protected int ResourceID = 0;
    protected string Resource = string.Empty; 
    private DataColumnCollection DCC;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
        spnTitle.InnerText = "Resource Allocation - " + this.Resource;

        DataTable dt = LoadData();
        if (dt != null) this.DCC = dt.Columns;

        grdData.DataSource = dt;
        grdData.DataBind();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["ReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseID"]))
        {
            int.TryParse(Request.QueryString["ReleaseID"], out this.ReleaseID);
        }

        if (Request.QueryString["ResourceID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ResourceID"]))
        {
            int.TryParse(Request.QueryString["ResourceID"], out this.ResourceID);
        }

        if (Request.QueryString["Resource"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Resource"]))
        {
            this.Resource = Request.QueryString["Resource"];
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

        if (IsPostBack && Session["dtSystemResource"] != null)
        {
            dt = (DataTable)Session["dtSystemResource"];
        }
        else
        {
            dt = MasterData.WTS_System_ResourceAltList_Get(WTS_RESOURCEID: this.ResourceID, ProductVersionID: this.ReleaseID);
            Session["dtSystemResource"] = dt;
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

        if (DCC.Contains("WTS_SYSTEM_RESOURCE_ID"))
        {
            row.Attributes.Add("systemresource_id", row.Cells[DCC.IndexOf("WTS_SYSTEM_RESOURCE_ID")].Text);

            if (this.CanEdit)
            {
                if (DCC.Contains("Allocation %"))
                {
                    row.Cells[DCC.IndexOf("Allocation %")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Allocation %")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("WTS_SYSTEM_RESOURCE_ID")].Text, "Allocation %", row.Cells[DCC.IndexOf("Allocation %")].Text, true));
                }
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

        if (DCC.Contains("Allocation %")) row.Cells[DCC.IndexOf("Allocation %")].Style["width"] = "75px";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
        }

        if (DCC.Contains("Allocation %")) row.Cells[DCC.IndexOf("Allocation %")].Style["text-align"] = "center";
    }

    private TextBox CreateTextBox(string SystemResource_ID, string type, string value, bool isNumber)
    {
        string txtValue = Server.HtmlDecode(Uri.UnescapeDataString(value)).Trim();
        TextBox txt = new TextBox();

        txt.Text = txtValue;
        txt.MaxLength = 50;
        txt.Width = new Unit(type == "Allocation %" ? 90 : 95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("systemresource_id", SystemResource_ID);
        txt.Attributes.Add("field", type);
        txt.Attributes.Add("original_value", txtValue);

        if (isNumber)
        {
            txt.MaxLength = 3;
            txt.Style["text-align"] = "center";
        }

        return txt;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string SaveChanges(string changes)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            XmlDocument docChanges = (XmlDocument)JsonConvert.DeserializeXmlNode(changes, "changes");

            saved = MasterData.WTS_System_ResourceAlt_Save(Changes: docChanges);
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
    #endregion
}