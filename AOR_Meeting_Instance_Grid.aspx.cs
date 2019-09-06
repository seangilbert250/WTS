using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_Meeting_Instance_Grid : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAORMeetingInstance = false;
    protected bool CanViewAORMeetingInstance = false;
    protected int AORMeetingID = 0;
    private DataColumnCollection DCC;
    protected int GridPageIndex = 0;
    protected int RowCount = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditAORMeetingInstance = UserManagement.UserCanEdit(WTSModuleOption.Meeting);
        this.CanViewAORMeetingInstance = this.CanEditAORMeetingInstance || UserManagement.UserCanView(WTSModuleOption.Meeting);

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

        if (Request.QueryString["AORMeetingID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingID"]))
        {
            int.TryParse(Request.QueryString["AORMeetingID"], out this.AORMeetingID);
        }

        if (Request.QueryString["GridPageIndexSub"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndexSub"]))
        {
            int.TryParse(Request.QueryString["GridPageIndexSub"], out this.GridPageIndex);
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

        if (IsPostBack && Session["dtAORMeetingInstance"] != null)
        {
            dt = (DataTable)Session["dtAORMeetingInstance"];
        }
        else
        {
            dt = AOR.AORMeetingInstanceList_Get(AORMeetingID: this.AORMeetingID, AORMeetingInstanceID: 0, InstanceFilterID: 0);
            Session["dtAORMeetingInstance"] = dt;
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

        if (DCC.Contains("Meeting Instance #") && DCC.Contains("Meeting Instance Name"))
        {
            row.Attributes.Add("aormeetinginstance_id", row.Cells[DCC.IndexOf("Meeting Instance #")].Text);
            row.Attributes.Add("locked", DCC.Contains("Locked_ID") && row.Cells[DCC.IndexOf("Locked_ID")].Text == "0" ? "0" : "1");

            if (this.CanEditAORMeetingInstance)
            {
                if (DCC.Contains("Locked_ID") && row.Cells[DCC.IndexOf("Locked_ID")].Text == "0")
                {
                    row.Cells[DCC.IndexOf("Meeting Instance Name")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Meeting Instance Name")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("Meeting Instance #")].Text, "Meeting Instance Name", row.Cells[DCC.IndexOf("Meeting Instance Name")].Text, false));

                    if (DCC.Contains("Sort"))
                    {
                        row.Cells[DCC.IndexOf("Sort")].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf("Sort")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("Meeting Instance #")].Text, "Sort", row.Cells[DCC.IndexOf("Sort")].Text, true));
                    }
                }
            }
        }

        if (DCC.Contains("Instance Date"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Instance Date")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("Instance Date")].Text = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
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

        if (DCC.Contains("MeetingEnded")) row.Cells[DCC.IndexOf("MeetingEnded")].Style["display"] = "none";
        if (DCC.Contains("MeetingAccepted")) row.Cells[DCC.IndexOf("MeetingAccepted")].Style["display"] = "none";
        if (DCC.Contains("LastMeetingMinutesDocumentID")) row.Cells[DCC.IndexOf("LastMeetingMinutesDocumentID")].Style["display"] = "none";

        if (DCC.Contains("Meeting Instance #")) row.Cells[DCC.IndexOf("Meeting Instance #")].Style["width"] = "125px";
        if (DCC.Contains("Meeting Instance Name")) row.Cells[DCC.IndexOf("Meeting Instance Name")].Style["width"] = "250px";
        if (DCC.Contains("Instance Date")) row.Cells[DCC.IndexOf("Instance Date")].Style["width"] = "150px";
        if (DCC.Contains("Actual Length")) row.Cells[DCC.IndexOf("Actual Length")].Style["width"] = "120px";
        if (DCC.Contains("Sort")) row.Cells[DCC.IndexOf("Sort")].Style["width"] = "75px";

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

        if (DCC.Contains("MeetingEnded")) row.Cells[DCC.IndexOf("MeetingEnded")].Style["display"] = "none";
        if (DCC.Contains("MeetingAccepted")) row.Cells[DCC.IndexOf("MeetingAccepted")].Style["display"] = "none";
        if (DCC.Contains("LastMeetingMinutesDocumentID")) row.Cells[DCC.IndexOf("LastMeetingMinutesDocumentID")].Style["display"] = "none";

        if (DCC.Contains("Instance Date")) row.Cells[DCC.IndexOf("Instance Date")].Style["text-align"] = "center";
    }

    private TextBox CreateTextBox(string AORMeetingInstance_ID, string type, string value, bool isNumber)
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
        txt.Attributes.Add("aormeetinginstance_id", AORMeetingInstance_ID);
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
    public static string DeleteAORMeetingInstance(string aorMeetingInstance)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int AORMeetingInstance_ID = 0;
            int.TryParse(aorMeetingInstance, out AORMeetingInstance_ID);

            deleted = AOR.AORMeetingInstance_Delete(AORMeetingInstanceID: AORMeetingInstance_ID);
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

            saved = AOR.AORMeetingInstance_Update(Changes: docChanges);
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