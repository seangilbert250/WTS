using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;
using Aspose.Cells;
using Newtonsoft.Json;

public partial class AOR_Release_Assessment_Grid : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEdit = false;
    protected bool CanView = false;
    protected bool _export = false;
    protected DataColumnCollection DCC;
    protected int GridPageIndex = 0;
    protected int TotalCount = 0;
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
        InitializeEvents();

        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.Deployment);
        this.CanView = this.CanEdit || UserManagement.UserCanView(WTSModuleOption.Deployment);

        DataTable dt = LoadData();

        if (dt != null)
        {
            this.DCC = dt.Columns;
            this.TotalCount = dt.Rows.Count;
        }

        if (_export && dt != null)
        {
            exportExcel(dt);
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

        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }

        if (Request.QueryString["Export"] != null
        && !string.IsNullOrWhiteSpace(Request.QueryString["Export"].ToString()) && Request.QueryString["Export"] == "1")
        {
            _export = true;
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
                    case "ReleaseAssessmentID":
                        displayName = "Release Assessment #";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ProductVersion":
                        displayName = "Release";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "CONTRACT":
                        displayName = "Contract";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "DeploymentCount":
                        displayName = "Deployments";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ReviewNarrative":
                        displayName = "Review Narrative";
                        blnVisible = true;
                        break;
                    case "Mitigation":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "MitigationNarrative":
                        displayName = "Mitigation Narrative";
                        blnVisible = true;
                        break;
                    case "Reviewed":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ReviewedBy":
                        displayName = "Reviewed By";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ReviewedDate":
                        displayName = "Reviewed Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    //case "Sort":
                    //    blnVisible = true;
                    //    blnSortable = true;
                    //    break;
                    case "Archive":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "CreatedBy":
                        displayName = "Created By";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "CreatedDate":
                        displayName = "Created Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "UpdatedBy":
                        displayName = "Updated By";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "UpdatedDate":
                        displayName = "Updated Date";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Z":
                        displayName = "";
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
        DataTable dt = new DataTable();

        if (IsPostBack && Session["dtReleaseAssessment"] != null)
        {
            dt = (DataTable)Session["dtReleaseAssessment"];
        }
        else
        {
            dt = AOR.ReleaseAssessmentList_Get();

            dt.Columns.Add("Z");

            Session["dtReleaseAssessment"] = dt;
        }

        InitializeColumnData(ref dt);
        dt.AcceptChanges();

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

        if (DCC.Contains("ReleaseAssessmentID"))
        {
            row.Attributes.Add("releaseassessment_id", row.Cells[DCC.IndexOf("ReleaseAssessmentID")].Text);

            if (DCC.Contains("ReviewNarrative"))
            {
                row.Cells[DCC.IndexOf("ReviewNarrative")].Controls.Add(CreateTextBox("ReleaseAssessment", row.Cells[DCC.IndexOf("ReleaseAssessmentID")].Text, "Review Narrative", row.Cells[DCC.IndexOf("ReviewNarrative")].Text, false, true));
            }

            if (DCC.Contains("MitigationNarrative"))
            {
                row.Cells[DCC.IndexOf("MitigationNarrative")].Controls.Add(CreateTextBox("ReleaseAssessment", row.Cells[DCC.IndexOf("ReleaseAssessmentID")].Text, "Mitigation Narrative", row.Cells[DCC.IndexOf("MitigationNarrative")].Text, false, true));
            }
        }

        DateTime nDate = new DateTime();

        if (DCC.Contains("ReviewedDate"))
        {
            if (DateTime.TryParse(row.Cells[DCC.IndexOf("ReviewedDate")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("ReviewedDate")].Text = String.Format("{0:M/d/yyyy}", nDate);
            }
        }

        if (DCC.Contains("CreatedDate"))
        {
            if (DateTime.TryParse(row.Cells[DCC.IndexOf("CreatedDate")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("CreatedDate")].Text = String.Format("{0:M/d/yyyy}", nDate);
            }
        }

        if (DCC.Contains("UpdatedDate"))
        {
            if (DateTime.TryParse(row.Cells[DCC.IndexOf("UpdatedDate")].Text, out nDate))
            {
                row.Cells[DCC.IndexOf("UpdatedDate")].Text = String.Format("{0:M/d/yyyy}", nDate);
            }
        }
    }

    private void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdData.PageIndex = e.NewPageIndex;
    }

    private void FormatHeaderRowDisplay(ref GridViewRow row)
    {
        if (DCC.Contains("ReleaseAssessmentID")) row.Cells[DCC.IndexOf("ReleaseAssessmentID")].Style["width"] = "65px";
        if (DCC.Contains("ProductVersion")) row.Cells[DCC.IndexOf("ProductVersion")].Style["width"] = "55px";
        if (DCC.Contains("CONTRACT")) row.Cells[DCC.IndexOf("CONTRACT")].Style["width"] = "175px";
        if (DCC.Contains("DeploymentCount")) row.Cells[DCC.IndexOf("DeploymentCount")].Style["width"] = "80px";
        if (DCC.Contains("ReviewNarrative")) row.Cells[DCC.IndexOf("ReviewNarrative")].Style["width"] = "285px";
        if (DCC.Contains("Mitigation")) row.Cells[DCC.IndexOf("Mitigation")].Style["width"] = "65px";
        if (DCC.Contains("MitigationNarrative")) row.Cells[DCC.IndexOf("MitigationNarrative")].Style["width"] = "285px";
        if (DCC.Contains("Reviewed")) row.Cells[DCC.IndexOf("Reviewed")].Style["width"] = "65px";
        if (DCC.Contains("ReviewedBy")) row.Cells[DCC.IndexOf("ReviewedBy")].Style["width"] = "90px";
        if (DCC.Contains("ReviewedDate")) row.Cells[DCC.IndexOf("ReviewedDate")].Style["width"] = "65px";
        if (DCC.Contains("Sort")) row.Cells[DCC.IndexOf("Sort")].Style["width"] = "50px";
        if (DCC.Contains("Archive")) row.Cells[DCC.IndexOf("Archive")].Style["width"] = "50px";
        if (DCC.Contains("CreatedBy")) row.Cells[DCC.IndexOf("CreatedBy")].Style["width"] = "90px";
        if (DCC.Contains("CreatedDate")) row.Cells[DCC.IndexOf("CreatedDate")].Style["width"] = "65px";
        if (DCC.Contains("UpdatedBy")) row.Cells[DCC.IndexOf("UpdatedBy")].Style["width"] = "90px";
        if (DCC.Contains("UpdatedDate")) row.Cells[DCC.IndexOf("UpdatedDate")].Style["width"] = "65px";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        if (DCC.Contains("ReleaseAssessmentID")) row.Cells[DCC.IndexOf("ReleaseAssessmentID")].Style["text-align"] = "center";
        if (DCC.Contains("DeploymentCount")) row.Cells[DCC.IndexOf("DeploymentCount")].Style["text-align"] = "center";
        if (DCC.Contains("Mitigation")) row.Cells[DCC.IndexOf("Mitigation")].Style["text-align"] = "center";
        if (DCC.Contains("Reviewed")) row.Cells[DCC.IndexOf("Reviewed")].Style["text-align"] = "center";
        if (DCC.Contains("Sort")) row.Cells[DCC.IndexOf("Sort")].Style["text-align"] = "center";
        if (DCC.Contains("Archive")) row.Cells[DCC.IndexOf("Archive")].Style["text-align"] = "center";
    }

    private TextBox CreateTextBox(string typeName, string typeID, string field, string value, bool isNumber, bool multiLine = false)
    {
        string txtValue = Server.HtmlDecode(Uri.UnescapeDataString(value)).Trim();
        TextBox txt = new TextBox();

        if (!this.CanEdit) txt.ReadOnly = true;

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

    #region Excel
    private void exportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        formatParent(ref copydt);
        String strName = "Release Assessment";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[0];
        int rowCount = 0;
        printParentHeader(ws, ref rowCount, copydt.Columns);
        foreach (DataRow parentRow in copydt.Rows)
        {
            if (parentRow.Field<int>("Release Assessment #") != 0)
            {
                printParent(parentRow, ws, ref rowCount, copydt);
                rowCount++;
            }
        }
        ws.Cells.DeleteColumn(0);
        ws.AutoFitColumns();
        wb.Save(ms, SaveFormat.Xlsx);
        Response.BufferOutput = true;
        Response.ContentType = "application/xlsx";
        Response.AddHeader("content-disposition", "attachment;  filename=" + strName + ".xlsx");
        Response.BinaryWrite(ms.ToArray());
        Response.End();
    }

    private void printParentHeader(Worksheet ws, ref int rowCount, DataColumnCollection columns)
    {
        Aspose.Cells.Style style = new Aspose.Cells.Style();
        style.Pattern = BackgroundType.Solid;
        style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
        for (int i = 0; i < columns.Count; i++)
        {
            ws.Cells[rowCount, i].PutValue(columns[i].ColumnName);
            ws.Cells[rowCount, i].SetStyle(style);
        }
        rowCount++;
    }

    private void printParent(DataRow parentRow, Worksheet ws, ref int rowCount, DataTable dt)
    {
        int i = 0;
        foreach (object value in parentRow.ItemArray)
        {
            ws.Cells[rowCount, i].PutValue(Server.HtmlDecode(Uri.UnescapeDataString(value.ToString())));
            i++;
        }
    }

    private static void formatParent(ref DataTable dt)
    {
        if (dt.Columns.Contains("ProductVersionID")) dt.Columns.Remove("ProductVersionID");
        if (dt.Columns.Contains("CONTRACTID")) dt.Columns.Remove("CONTRACTID");
        if (dt.Columns.Contains("Sort")) dt.Columns.Remove("Sort");
        if (dt.Columns.Contains("Z")) dt.Columns.Remove("Z");

        if (dt.Columns.Contains("ReleaseAssessmentID")) dt.Columns["ReleaseAssessmentID"].ColumnName = "Release Assessment #";
        if (dt.Columns.Contains("ProductVersion")) dt.Columns["ProductVersion"].ColumnName = "Release";
        if (dt.Columns.Contains("CONTRACT")) dt.Columns["CONTRACT"].ColumnName = "Contract";
        if (dt.Columns.Contains("DeploymentCount")) dt.Columns["DeploymentCount"].ColumnName = "Deployments";
        if (dt.Columns.Contains("ReviewNarrative")) dt.Columns["ReviewNarrative"].ColumnName = "Review Narrative";
        if (dt.Columns.Contains("MitigationNarrative")) dt.Columns["MitigationNarrative"].ColumnName = "Mitigation Narrative";
        if (dt.Columns.Contains("ReviewedBy")) dt.Columns["ReviewedBy"].ColumnName = "Reviewed By";
        if (dt.Columns.Contains("ReviewedDate")) dt.Columns["ReviewedDate"].ColumnName = "Reviewed Date";
        if (dt.Columns.Contains("CreatedBy")) dt.Columns["CreatedBy"].ColumnName = "Created By";
        if (dt.Columns.Contains("CreatedDate")) dt.Columns["CreatedDate"].ColumnName = "Created Date";
        if (dt.Columns.Contains("UpdatedBy")) dt.Columns["UpdatedBy"].ColumnName = "Updated By";
        if (dt.Columns.Contains("UpdatedDate")) dt.Columns["UpdatedDate"].ColumnName = "Updated Date";
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string DeleteReleaseAssessment(string releaseAssessment)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int ReleaseAssessment_ID = 0;
            int.TryParse(releaseAssessment, out ReleaseAssessment_ID);

            deleted = AOR.ReleaseAssessment_Delete(ReleaseAssessmentID: ReleaseAssessment_ID);
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

            //saved = AOR.ReleaseAssessment_Update(Changes: docChanges);
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