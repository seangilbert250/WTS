using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class RQMTDescription_Grid : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditRQMTDescription = false;
    protected bool CanViewRQMTDescription = false;
    protected DataTable dtRQMTDescriptionType;
    protected DataColumnCollection DCC;
    protected int GridPageIndex = 0;
    protected int TotalCount = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditRQMTDescription = UserManagement.UserCanEdit(WTSModuleOption.RQMT);
        this.CanViewRQMTDescription = this.CanEditRQMTDescription || UserManagement.UserCanView(WTSModuleOption.RQMT);

        this.dtRQMTDescriptionType = RQMT.RQMTDescriptionTypeList_Get();

        DataTable dt = LoadData();
        if (dt != null)
        {
            this.DCC = dt.Columns;
            this.TotalCount = dt.Rows.Count;
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

        if (IsPostBack && Session["dtRQMTDescription"] != null)
        {
            dt = (DataTable)Session["dtRQMTDescription"];
        }
        else
        {
            dt = RQMT.RQMTDescriptionList_Get();

            Session["dtRQMTDescription"] = dt;
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

        if (DCC.Contains("RQMTDescription_ID"))
        {
            row.Attributes.Add("rqmtdescription_id", row.Cells[DCC.IndexOf("RQMTDescription_ID")].Text);

            if (this.CanEditRQMTDescription)
            {
                if (DCC.Contains("RQMT Description Type"))
                {
                    row.Cells[DCC.IndexOf("RQMT Description Type")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("RQMT Description Type")].Controls.Add(CreateDropDownList("RQMT Description", row.Cells[DCC.IndexOf("RQMTDescription_ID")].Text, this.dtRQMTDescriptionType, "RQMT Description Type", "RQMTDescriptionType", "RQMTDescriptionTypeID", row.Cells[DCC.IndexOf("RQMTDescriptionType_ID")].Text, row.Cells[DCC.IndexOf("RQMT Description Type")].Text, null));
                }

                if (DCC.Contains("Sort"))
                {
                    row.Cells[DCC.IndexOf("Sort")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Sort")].Controls.Add(CreateTextBox("RQMT Description", row.Cells[DCC.IndexOf("RQMTDescription_ID")].Text, "Sort", row.Cells[DCC.IndexOf("Sort")].Text, true, false));
                }
            }

            if (DCC.Contains("RQMT Description"))
            {
                row.Cells[DCC.IndexOf("RQMT Description")].Style["text-align"] = "center";
                TextBox obj = CreateTextBox("RQMT Description", row.Cells[DCC.IndexOf("RQMTDescription_ID")].Text, "RQMT Description", row.Cells[DCC.IndexOf("RQMT Description")].Text, false, true);
                if (!this.CanEditRQMTDescription)
                {
                    obj.ReadOnly = true;
                    obj.ForeColor = System.Drawing.Color.Gray;
                }
                row.Cells[DCC.IndexOf("RQMT Description")].Controls.Add(obj);
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

        if (DCC.Contains("RQMT Description Type")) row.Cells[DCC.IndexOf("RQMT Description Type")].Style["width"] = "200px";
        if (DCC.Contains("RQMT Description")) row.Cells[DCC.IndexOf("RQMT Description")].Style["width"] = "600px";
        if (DCC.Contains("Sort")) row.Cells[DCC.IndexOf("Sort")].Style["width"] = "45px";

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
    }

    private TextBox CreateTextBox(string typeName, string typeID, string field, string value, bool isNumber, bool multiLine = false)
    {
        string txtValue = Server.HtmlDecode(Uri.UnescapeDataString(value)).Trim();
        TextBox txt = new TextBox();

        if (multiLine)
        {
            txt.Wrap = true;
            txt.TextMode = TextBoxMode.MultiLine;
        }

        txt.Text = txtValue;
        txt.MaxLength = 50;
        txt.Width = new Unit(field == "Sort" ? 90 : 95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("typeName", typeName);
        txt.Attributes.Add("typeID", typeID);
        txt.Attributes.Add("field", field);
        txt.Attributes.Add("original_value", txtValue);

        if (isNumber)
        {
            txt.MaxLength = 5;
            txt.Style["text-align"] = "center";
        }

        return txt;
    }

    private DropDownList CreateDropDownList(string typeName, string typeID
        , DataTable dt, string field
        , string textField, string valueField
        , string value, string text = ""
        , List<string> attributes = null)
    {
        DropDownList ddl = new DropDownList();

        textField = Server.HtmlDecode(textField).Trim();
        valueField = Server.HtmlDecode(valueField).Trim();
        value = Server.HtmlDecode(value).Trim();
        text = Server.HtmlDecode(text).Trim();

        ddl.Width = new Unit(95, UnitType.Percentage);
        ddl.Attributes["class"] = "saveable";
        ddl.Attributes["onchange"] = "input_change(this);";
        ddl.Attributes.Add("typeName", typeName);
        ddl.Attributes.Add("typeID", typeID);
        ddl.Attributes.Add("field", field);
        ddl.Attributes.Add("original_value", value);
        ddl.Style["background-color"] = "#F5F6CE";

        if (dt == null)
        {
            return ddl;
        }

        ListItem item = null;
        foreach (DataRow row in dt.Rows)
        {
            item = new ListItem();
            item.Text = row[textField].ToString();
            item.Value = row[valueField].ToString();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (string key in attributes)
                {
                    item.Attributes.Add(key.Trim(), key.Trim());
                }
            }

            if (ddl.Items.FindByValue(item.Value) == null)
            {
                ddl.Items.Add(item);
            }
        }

        WTSUtility.SelectDdlItem(ddl, string.IsNullOrWhiteSpace(value) ? "0" : value, text);

        return ddl;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string DeleteRQMTDescription(string rqmtDescription)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int RQMTDescription_ID = 0;
            int.TryParse(rqmtDescription, out RQMTDescription_ID);

            deleted = RQMT.RQMTDescription_Delete(RQMTDescriptionID: RQMTDescription_ID);
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
    public static string SaveChanges(string changes)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            XmlDocument docChanges = (XmlDocument)JsonConvert.DeserializeXmlNode(changes, "changes");

            saved = RQMT.RQMT_Update(Changes: docChanges); //todo: check for uniqueness
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