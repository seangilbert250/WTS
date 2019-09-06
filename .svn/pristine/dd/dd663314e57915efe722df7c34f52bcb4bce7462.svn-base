using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_Attachments : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool PreviousExists = false;
    protected int AORID = 0;
    protected int AORReleaseID = 0;
    protected int AORReleaseAttachmentID = 0;
    protected int _qfTypeID = 0;
    protected DataColumnCollection DCC;
    protected int GridPageIndex = 0;
    protected int RowCount = 0;
    protected DataTable dtStatus = null;
    protected GridCols columnData = new GridCols();

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        if (this.AORReleaseAttachmentID > 0)
        {
            DataTable dt = AOR.AORAttachment_Get(AORReleaseAttachmentID: this.AORReleaseAttachmentID);

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

            this.CanEditAOR = (UserManagement.UserCanEdit(WTSModuleOption.AOR) && AOR.AORReleaseCurrent(AORID: this.AORID, AORReleaseID: this.AORReleaseID));
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

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.AORReleaseID);
        }

        if (Request.QueryString["AORReleaseAttachmentID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseAttachmentID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseAttachmentID"], out this.AORReleaseAttachmentID);
        }

        if (Request.QueryString["TypeID"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["TypeID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["TypeID"].ToString()), out this._qfTypeID);
        }

        if (Request.QueryString["GridPageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndex"]))
        {
            int.TryParse(Request.QueryString["GridPageIndex"], out this.GridPageIndex);
        }
        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }
    }

    private void InitializeEvents()
    {
        if (this._qfTypeID > 0)
        {
            ddlQF_Type.SelectedValue = this._qfTypeID.ToString();
        }

        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;
    }

    protected void InitializeColumnData(ref DataTable dt)
    {
        try
        {
            string displayName = string.Empty, groupName = string.Empty;
            bool blnVisible = false, blnSortable = false, blnOrderable = false;

            foreach (DataColumn gridColumn in dt.Columns)
            {
                displayName = gridColumn.ColumnName;
                blnVisible = false;
                blnSortable = false;
                blnOrderable = false;
                groupName = string.Empty;

                switch (gridColumn.ColumnName)
                {
                    case "Type":
                        displayName = "Type";
                        blnVisible = true;
                        break;
                    case "Attachment Name":
                        displayName = "Attachment Name";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Description":
                        displayName = "Description";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "File":
                        displayName = "File";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    //case "INV":
                    //    displayName = "INV";
                    //    blnVisible = true;
                    //    break;
                    //case "TD":
                    //    displayName = "TD";
                    //    blnVisible = true;
                    //    break;
                    //case "CD":
                    //    displayName = "CD";
                    //    blnVisible = true;
                    //    break;
                    //case "C":
                    //    displayName = "C";
                    //    blnVisible = true;
                    //    break;
                    //case "IT":
                    //    displayName = "IT";
                    //    blnVisible = true;
                    //    break;
                    //case "CVT":
                    //    displayName = "CVT";
                    //    blnVisible = true;
                    //    break;
                    //case "ADOPT":
                    //    displayName = "ADOPT";
                    //    blnVisible = true;
                    //    break;
                    case "Approved":
                        blnVisible = true;
                        break;
                    case "Added By":
                        displayName = "Added By/Updated";
                        blnVisible = true;
                        break;
                }

                columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
                columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
            }

            //Initialize the columnData
            columnData.Initialize(ref dt, ";", "~", "|");

            //Get sortable columns and default column order
            SortableColumns = columnData.SortableColumnsToString();
            DefaultColumnOrder = columnData.DefaultColumnOrderToString();
            //Sort and Reorder Columns
            columnData.ReorderDataTable(ref dt, ColumnOrder);
            columnData.SortDataTable(ref dt, SortOrder);
            SelectedColumnOrder = columnData.CurrentColumnOrderToString();

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }
    #endregion

    #region Data
    private DataTable LoadData()
    {
        DataTable dtType = AOR.AORAddList_Get(AORID: 0, AORReleaseID: 0, SRID: 0, CRID: 0, DeliverableID: 0, Type: "Attachment", Filters: null);
        ddlQF_Type.DataSource = dtType;
        ddlQF_Type.DataTextField = "AORAttachmentTypeName";
        ddlQF_Type.DataValueField = "AORAttachmentTypeID";
        ddlQF_Type.DataBind();

        DataTable dt = new DataTable();

        if (IsPostBack && Session["dtAORAttachment"] != null)
        {
            dt = (DataTable)Session["dtAORAttachment"];
        }
        else
        {
            dt = AOR.AORAttachmentList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID, AORAttachmentTypeID: _qfTypeID);
            Session["dtAORAttachment"] = dt;
        }
        InitializeColumnData(ref dt);
        dt.AcceptChanges();

        dtStatus = MasterData.StatusList_Get(includeArchive: false);

        DataTable dtPrevious = AOR.AORList_Get(AORID: AORID);
        if (dtPrevious != null && dtPrevious.Rows.Count > 1) this.PreviousExists = true;

        return dt;
    }
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        FormatHeaderRowDisplay(ref row);
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        FormatRowDisplay(ref row);

        if (DCC.Contains("Approved"))
        {
            row.Cells[DCC.IndexOf("Approved")].Controls.Clear();
            row.Cells[DCC.IndexOf("Approved")].Style["text-align"] = "center";

            Label lblApproved = new Label();
            DateTime nDate = new DateTime();

            lblApproved.Text = row.Cells[DCC.IndexOf("USERNAME")].Text.ToLower();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("ApprovedDate")].Text, out nDate))
            {
                lblApproved.Text += " " + String.Format("{0:M/d/yyyy h:mm tt}", nDate);
            }

            if (row.Cells[DCC.IndexOf("Approved")].Text == "1") lblApproved.Text += "<br />";

            lblApproved.Style["font-size"] = "10px";
            lblApproved.Style["white-space"] = "nowrap";

            row.Cells[DCC.IndexOf("Approved")].Controls.Add(lblApproved);
        }

        if (DCC.Contains("AORReleaseAttachment_ID") && DCC.Contains("Attachment Name"))
        {
            row.Attributes.Add("aorreleaseattachment_id", row.Cells[DCC.IndexOf("AORReleaseAttachment_ID")].Text);

            if (this.CanEditAOR)
            {
                row.Cells[DCC.IndexOf("Attachment Name")].Style["text-align"] = "center";
                TextBox tb = CreateTextBox(row.Cells[DCC.IndexOf("AORReleaseAttachment_ID")].Text, "AOR Attachment Name", row.Cells[DCC.IndexOf("Attachment Name")].Text, false);
                tb.Style.Add("width", "300px");
                row.Cells[DCC.IndexOf("Attachment Name")].Controls.Add(tb);

                if (DCC.Contains("Description"))
                {
                    row.Cells[DCC.IndexOf("Description")].Style["text-align"] = "center";
                    tb = CreateTextBox(row.Cells[DCC.IndexOf("AORReleaseAttachment_ID")].Text, "Description", row.Cells[DCC.IndexOf("Description")].Text, false);
                    tb.Style.Add("width", "300px");
                    row.Cells[DCC.IndexOf("Description")].Controls.Add(tb);
                }

                if (DCC.Contains("Approved"))
                {
                    bool approved = row.Cells[DCC.IndexOf("Approved")].Text == "1";

                    row.Cells[DCC.IndexOf("Approved")].Attributes.Add("id", "tdApproved" + row.Cells[DCC.IndexOf("AORReleaseAttachment_ID")].Text);
                    row.Cells[DCC.IndexOf("Approved")].Controls.Add(CreateLink("Approve", row.Cells[DCC.IndexOf("AORReleaseAttachment_ID")].Text, "", approved));
                }
            }

            if (this.CanViewAOR)
            {
                if (DCC.Contains("File"))
                {
                    row.Cells[DCC.IndexOf("File")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("File")].Controls.Add(CreateLink("File", row.Cells[DCC.IndexOf("AORReleaseAttachment_ID")].Text, row.Cells[DCC.IndexOf("File")].Text));
                }
            }
        }

        string strAdd = row.Cells[DCC.IndexOf("Added By")].Text;
        if (DCC.Contains("Added Date"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Added Date")].Text, out nDate))
            {
                strAdd += " " + String.Format(" {0:M/d/yyyy h:mm tt}", nDate);
            }
        }

        string strUpdate = row.Cells[DCC.IndexOf("Updated By")].Text;
        if (DCC.Contains("Updated Date"))
        {
            DateTime nDate = new DateTime();

            if (DateTime.TryParse(row.Cells[DCC.IndexOf("Updated Date")].Text, out nDate))
            {
                strUpdate += " " + String.Format(" {0:M/d/yyyy h:mm tt}", nDate);
            }
        }
        row.Cells[DCC.IndexOf("Added By")].Text = strAdd != strUpdate ? strAdd + "<br>" + strUpdate : strAdd;
        row.Cells[DCC.IndexOf("Added By")].Style["font-size"] = "10px";
        row.Cells[DCC.IndexOf("Added By")].Style["white-space"] = "nowrap";
        row.Cells[DCC.IndexOf("Added By")].Style["vertical-align"] = "top";

        var r = row; // anonymous functions can't use ref variables, so we copy the ref into a non-ref var
        string[] statusCols = "INV,TD,CD,C,IT,CVT,ADOPT".Split(',');
        Array.ForEach(statusCols, s => {
            if (DCC.Contains(s))
            {
                if (this.CanEditAOR)
                {
                    row.Cells[DCC.IndexOf(s)].Controls.Clear();
                    DropDownList ddl = new DropDownList();
                    ddl.Style.Add("width", "125px");

                    DataTable dtOptions = dtStatus.Copy();

                    dtOptions.DefaultView.RowFilter = "StatusType IN ('', '" + s + "')";
                    dtOptions = dtOptions.DefaultView.ToTable();
                    ddl.DataSource = dtOptions;
                    ddl.DataValueField = "StatusID";
                    ddl.DataTextField = "DESCRIPTION";
                    ddl.DataBind();
                    ddl.SelectedValue = row.Cells[DCC.IndexOf(s)].Text;
                    ddl.Attributes["onchange"] = "input_change(this);";
                    ddl.Attributes.Add("aorreleaseattachment_id", row.Cells[DCC.IndexOf("AORReleaseAttachment_ID")].Text);
                    ddl.Attributes.Add("field", s);
                    ddl.Attributes.Add("original_value", row.Cells[DCC.IndexOf(s)].Text);

                    row.Cells[DCC.IndexOf(s)].Controls.Add(ddl);
                }
                else
                {
                    row.Cells[DCC.IndexOf(s)].Text = "can't edit";
                }
            };
        });
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

        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["display"] = "none";

        var r = row; // anonymous functions can't use ref variables, so we copy the ref into a non-ref var
        string[] statusCols = "INV,TD,CD,C,IT,CVT,ADOPT".Split(',');
        Array.ForEach(statusCols, s => {
            if (DCC.Contains(s)) r.Cells[DCC.IndexOf(s)].Style["width"] = "125px";
        });

        if (DCC.Contains("Type")) row.Cells[DCC.IndexOf("Type")].Style["width"] = "175px";

        if (DCC.Contains("Approved")) row.Cells[DCC.IndexOf("Approved")].Style["width"] = "150px";

        if (DCC.Contains("Added By")) row.Cells[DCC.IndexOf("Added By")].Style["width"] = "150px";
        if (DCC.Contains("Added By")) row.Cells[DCC.IndexOf("Added By")].Text = "Added/Updated";
        if (DCC.Contains("Added By")) row.Cells[DCC.IndexOf("Added By")].Style["text-align"] = "left";

        if (DCC.Contains("Added Date")) row.Cells[DCC.IndexOf("Added Date")].Style["display"] = "none";
        if (DCC.Contains("Updated By")) row.Cells[DCC.IndexOf("Updated By")].Style["display"] = "none";
        if (DCC.Contains("Updated Date")) row.Cells[DCC.IndexOf("Updated Date")].Style["display"] = "none";
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

        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["display"] = "none";
                
        if (DCC.Contains("Added Date")) row.Cells[DCC.IndexOf("Added Date")].Style["display"] = "none";
        if (DCC.Contains("Updated By")) row.Cells[DCC.IndexOf("Updated By")].Style["display"] = "none";
        if (DCC.Contains("Updated Date")) row.Cells[DCC.IndexOf("Updated Date")].Style["display"] = "none";
    }

    private LinkButton CreateLink(string type, string AORReleaseAttachment_ID, string FileName = "", bool Approved = false)
    {
        LinkButton lb = new LinkButton();

        switch (type)
        {
            case "File":
                lb.Text = FileName;
                lb.Attributes["onclick"] = string.Format("downloadAORAttachment('{0}'); return false;", AORReleaseAttachment_ID);
                break;
            case "Approve":
                lb.Text = Approved ? "Reject" : "Approve";
                lb.Attributes["onclick"] = string.Format("approveAORAttachment('{0}', '{1}'); return false;", AORReleaseAttachment_ID, !Approved);
                break;
        }

        return lb;
    }

    private TextBox CreateTextBox(string AORReleaseAttachment_ID, string type, string value, bool isNumber)
    {
        string txtValue = Server.HtmlDecode(value).Trim();
        TextBox txt = new TextBox();

        txt.Wrap = true;
        txt.TextMode = TextBoxMode.MultiLine;

        txt.Text = txtValue;
        txt.MaxLength = 50;
        txt.Width = new Unit(95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("aorreleaseattachment_id", AORReleaseAttachment_ID);
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
    public static string DeleteAORAttachment(string aorReleaseAttachment)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int AORReleaseAttachment_ID = 0;
            int.TryParse(aorReleaseAttachment, out AORReleaseAttachment_ID);

            deleted = AOR.AORAttachment_Delete(AORReleaseAttachmentID: AORReleaseAttachment_ID);
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

            saved = AOR.AORAttachment_Update(Changes: docChanges);
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

    [WebMethod]
    public static string ApproveAORAttachment(string aorReleaseAttachment, string blnApprove)
    {
        Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "approved", "" }, { "approvedBy", "" }, { "approvedDate", "" }, { "error", "" } };

        try
        {
            int AORReleaseAttachment_ID = 0;
            bool bln_Approve = false;

            int.TryParse(aorReleaseAttachment, out AORReleaseAttachment_ID);
            bool.TryParse(blnApprove, out bln_Approve);

            result = AOR.AORAttachmentApprove_Save(AORReleaseAttachmentID: AORReleaseAttachment_ID, Approve: bln_Approve);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        return JsonConvert.SerializeObject(result);
    }
    #endregion
}