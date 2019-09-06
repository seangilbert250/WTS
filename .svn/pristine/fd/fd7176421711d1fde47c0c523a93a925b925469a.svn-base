using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class SR_Attachments : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditSR = false;
    protected bool CanViewSR = false;
    protected int SRID = 0;
    protected int SRAttachmentID = 0;
    protected DataColumnCollection DCC;
    protected int GridPageIndex = 0;
    protected int RowCount = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        if (this.SRAttachmentID > 0)
        {
            DataTable dt = SR.SRAttachment_Get(SRAttachmentID: this.SRAttachmentID);

            if (dt == null || dt.Rows.Count == 0 || DBNull.Value.Equals(dt.Rows[0]["FileData"]))
            {
                return;
            }
            
            byte[] fileData = (byte[])dt.Rows[0]["FileData"];

            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + dt.Rows[0]["FileName"].ToString() + "\"");
            Response.AddHeader("Content-Length", fileData.Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.OutputStream.Write(fileData, 0, fileData.Length);
            Response.End();
        }
        else
        {
            InitializeEvents();

            this.CanEditSR = UserManagement.UserCanEdit(WTSModuleOption.SustainmentRequest);
            this.CanViewSR = this.CanEditSR || UserManagement.UserCanView(WTSModuleOption.SustainmentRequest);

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

        if (Request.QueryString["SRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SRID"]))
        {
            int.TryParse(Request.QueryString["SRID"], out this.SRID);
        }

        if (Request.QueryString["SRAttachmentID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SRAttachmentID"]))
        {
            int.TryParse(Request.QueryString["SRAttachmentID"], out this.SRAttachmentID);
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

        if (IsPostBack && Session["dtSRAttachment"] != null)
        {
            dt = (DataTable)Session["dtSRAttachment"];
        }
        else
        {
            dt = SR.SRAttachmentList_Get(SRID: this.SRID);

            if (dt.Columns.Contains("FileData")) dt.Columns.Remove("FileData");

            Session["dtSRAttachment"] = dt;
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

        if (DCC.Contains("SRAttachment_ID"))
        {
            row.Attributes.Add("srattachment_id", row.Cells[DCC.IndexOf("SRAttachment_ID")].Text);

            if (this.CanViewSR)
            {
                if (DCC.Contains("File"))
                {
                    row.Cells[DCC.IndexOf("File")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("File")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("SRAttachment_ID")].Text, row.Cells[DCC.IndexOf("File")].Text));
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

        if (DCC.Contains("File")) row.Cells[DCC.IndexOf("File")].Style["width"] = "300px";

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

    private LinkButton CreateLink(string SRAttachment_ID, string FileName)
    {
        LinkButton lb = new LinkButton();

        lb.Text = FileName;
        lb.Attributes["onclick"] = string.Format("downloadSRAttachment('{0}'); return false;", SRAttachment_ID);

        return lb;
    }

    private TextBox CreateTextBox(string SRAttachment_ID, string type, string value, bool isNumber)
    {
        string txtValue = Server.HtmlDecode(value).Trim();
        TextBox txt = new TextBox();

        txt.Text = txtValue;
        txt.MaxLength = 50;
        txt.Width = new Unit(95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("srattachment_id", SRAttachment_ID);
        txt.Attributes.Add("field", type);
        txt.Attributes.Add("original_value", txtValue);

        if (isNumber)
        {
            txt.MaxLength = 5;
            txt.Style["text-align"] = "center";
        }

        return txt;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string DeleteSRAttachment(string srAttachment)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int SRAttachment_ID = 0;
            int.TryParse(srAttachment, out SRAttachment_ID);

            //todo: deleted = SR.SRAttachment_Delete(SRAttachmentID: SRAttachment_ID);
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

            //todo: saved = SR.SRAttachment_Update(Changes: docChanges);
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