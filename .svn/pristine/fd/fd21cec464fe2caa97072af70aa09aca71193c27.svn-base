using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;

public partial class MDGrid_Narrative : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();
    protected DataTable dtImage = null;

    protected bool _refreshData = false;
    protected bool _export = false;

    protected int _currentLevel = 0;
    protected int _productVersionID = 0;
    protected int _qfContractID = 0;
    protected bool _qfShowArchive = false;
    protected string _narrative = "";
    protected int MAXLEVELS = 4;
    protected int ExcelRowCount = 0;
    protected DataColumnCollection NonPrintableDCC;

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;

    protected bool CanView = false;
    protected bool CanEdit = false;
    protected bool IsAdmin = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.IsAdmin = UserManagement.UserIsInRole("Admin");
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
        this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.MasterData));

        readQueryString();

        initControls();

        loadGridData();
    }
    private void readQueryString()
    {
        if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
            || Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
        {
            _refreshData = true;
        }

        if (Request.QueryString["CurrentLevel"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["CurrentLevel"]), out _currentLevel);
        }

        if (Request.QueryString["ProductVersionID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ProductVersionID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ProductVersionID"]), out _productVersionID);
        }

        if (Request.QueryString["Narrative"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["Narrative"].ToString()))
        {
            this._narrative = Uri.UnescapeDataString(Request.QueryString["Narrative"]);
        }

        if (Request.QueryString["ContractID"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["ContractID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ContractID"].ToString()), out this._qfContractID);
        }

        if (Request.QueryString["ShowArchive"] != null
             && !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchive"].ToString()))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["ShowArchive"].ToString()), out this._qfShowArchive);
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
    }

    private void initControls()
    {
        if (this._qfContractID > 0)
        {
            ddlQF_Contract.SelectedValue = this._qfContractID.ToString();
        }

        grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
        grdMD.GridRowDataBound += grdMD_GridRowDataBound;
        grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
    }

    private void loadGridData()
    {
        DataTable dtContract = MasterData.ContractList_Get(includeArchive: false);
        dtImage = MasterData.ImageList_Get(includeArchive: 0);
        DataTable dtProductVersion = null;

        dtProductVersion = MasterData.ProductVersionList_Get();
        ddlReleaseFrom.DataSource = dtProductVersion;
        ddlReleaseFrom.DataValueField = "ProductVersionID";
        ddlReleaseFrom.DataTextField = "ProductVersion";
        ddlReleaseFrom.DataBind();

        dtProductVersion.DefaultView.RowFilter = "ProductVersionID NOT IN ('" + _productVersionID + "') ";
        ddlReleaseTo.DataSource = dtProductVersion;
        ddlReleaseTo.DataValueField = "ProductVersionID";
        ddlReleaseTo.DataTextField = "ProductVersion";
        ddlReleaseTo.DataBind();

        dtContract = dtContract.DefaultView.ToTable();
        ddlQF_Contract.DataSource = dtContract;
        ddlQF_Contract.DataTextField = "CONTRACT";
        ddlQF_Contract.DataValueField = "CONTRACTID";
        ddlQF_Contract.DataBind();

        ddlContractCopy.DataSource = dtContract;
        ddlContractCopy.DataTextField = "CONTRACT";
        ddlContractCopy.DataValueField = "CONTRACTID";
        ddlContractCopy.DataBind();

        DataTable dt = null;
        if (_refreshData || Session["Narrative"] == null)
        {
            switch (_currentLevel)
            {
                case 0:
                    dt = MasterData.Narrative_ProductVersionList_Get(_qfContractID, includeArchive: _qfShowArchive);
                    break;
                case 1:
                    dt = MasterData.Narrative_CONTRACTList_Get(productVersionID: _productVersionID, contractID: _qfContractID, includeArchive: _qfShowArchive);
                    break;
                case 2:
                    dt = MasterData.NarrativeList_Get(productVersionID: _productVersionID, contractID: _qfContractID, includeArchive: _qfShowArchive);
                    break;
            }
               
            HttpContext.Current.Session["Narrative"] = dt;
        }
        else
        {
            dt = (DataTable)HttpContext.Current.Session["Narrative"];
        }

        if (dt != null)
        {
            this.DCC = dt.Columns;

            DataTable saveTitleDcc = new DataTable();
            saveTitleDcc.Columns.Add(new DataColumn("NarrativeID"));
            saveTitleDcc.Columns.Add(new DataColumn("Narrative"));
            saveTitleDcc.Columns.Add(new DataColumn("X"));
            saveTitleDcc.Columns.Add(new DataColumn("ProductVersionID"));
            saveTitleDcc.Columns.Add(new DataColumn("Sort"));
            saveTitleDcc.Columns.Add(new DataColumn("Archive"));
            Page.ClientScript.RegisterArrayDeclaration("_title_dcc", JsonConvert.SerializeObject(saveTitleDcc.Columns, Newtonsoft.Json.Formatting.None));

            DataTable saveNarrativeDcc = new DataTable();
            saveNarrativeDcc.Columns.Add(new DataColumn("X"));
            saveNarrativeDcc.Columns.Add(new DataColumn("ProductVersionID"));
            saveNarrativeDcc.Columns.Add(new DataColumn("CONTRACTID"));
            saveNarrativeDcc.Columns.Add(new DataColumn("CONTRACT"));
            saveNarrativeDcc.Columns.Add(new DataColumn("Sort"));
            saveNarrativeDcc.Columns.Add(new DataColumn("Archive"));
            saveNarrativeDcc.Columns.Add(new DataColumn("Y"));
            Page.ClientScript.RegisterArrayDeclaration("_contract_dcc", JsonConvert.SerializeObject(saveNarrativeDcc.Columns, Newtonsoft.Json.Formatting.None));

            DataTable saveNarrativeContractDcc = new DataTable();
            saveNarrativeContractDcc.Columns.Add(new DataColumn("X"));
            saveNarrativeContractDcc.Columns.Add(new DataColumn("Narrative_CONTRACTID"));
            saveNarrativeContractDcc.Columns.Add(new DataColumn("NarrativeID"));
            saveNarrativeContractDcc.Columns.Add(new DataColumn("WorkloadAllocationType"));
            saveNarrativeContractDcc.Columns.Add(new DataColumn("NarrativeDescription"));
            saveNarrativeContractDcc.Columns.Add(new DataColumn("ImageID"));
            saveNarrativeContractDcc.Columns.Add(new DataColumn("Sort"));
            saveNarrativeContractDcc.Columns.Add(new DataColumn("Archive"));
            saveNarrativeContractDcc.Columns.Add(new DataColumn("Y"));
            Page.ClientScript.RegisterArrayDeclaration("_narr_dcc", JsonConvert.SerializeObject(saveNarrativeContractDcc.Columns, Newtonsoft.Json.Formatting.None));

            spanRowCount.InnerText = dt.Rows.Count.ToString();

            InitializeColumnData(ref dt);
            dt.AcceptChanges();

            int count = dt.Rows.Count;
            count = count > 0 ? count - 1 : count;
            spanRowCount.InnerText = count.ToString();
        }

        if (_export && dt != null && CanView)
        {
            ExportExcel(dt);
        }

        
        grdMD.DataSource = dt;
        grdMD.DataBind();
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
                    case "X":
                        blnVisible = true;
                        break;
                    case "ProductVersionID":
                        blnVisible = false;
                        break;
                    case "ProductVersion":
                        displayName = "Product Version";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Description":
                        displayName = "Description";
                        blnVisible = true;
                        break;
                    case "CONTRACTID":
                        blnVisible = false;
                        break;
                    case "CONTRACT":
                        displayName = "Contract";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "NarrativeDescription":
                        displayName = "CR Report Narrative";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "WorkloadAllocationType":
                        displayName = "Workload Allocation Type";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "ImageID":
                        displayName = "Image";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Archive":
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Y":
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

    #region Grid

    void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        if (DCC.Contains("X") && _currentLevel < 2)
        {
            //add expand/collapse buttons
            HtmlGenericControl divChildren = new HtmlGenericControl();
            divChildren.Style["display"] = "table-row";
            divChildren.Style["text-align"] = "right";
            HtmlGenericControl divChildrenButtons = new HtmlGenericControl();
            divChildrenButtons.Style["display"] = "table-cell";
            divChildrenButtons.Controls.Add(createShowHideButton(true, "Show", "ALL"));
            divChildrenButtons.Controls.Add(createShowHideButton(false, "Hide", "ALL"));
            HtmlGenericControl divChildCount = new HtmlGenericControl();
            divChildCount.Style["display"] = "table-cell";
            divChildCount.Style["padding-left"] = "2px";
            divChildren.Controls.Add(divChildrenButtons);
            divChildren.Controls.Add(divChildCount);
            //buttons to show/hide child grid
            row.Cells[DCC["X"].Ordinal].Controls.Clear();
            row.Cells[DCC["X"].Ordinal].Controls.Add(divChildren);
            row.Cells[DCC.IndexOf("X")].Style["padding-left"] = "1px";

            //add child grid row for Task Items
            Table table = (Table)row.Parent;
            GridViewRow childRow = createChildRow("ALL");
            table.Rows.AddAt(table.Rows.Count, childRow);
        }

        if (_currentLevel == 0)
        {
            row.Cells[DCC.IndexOf("Description")].Style["text-align"] = "center";
        }
        else if (_currentLevel == 1)
        {
            row.Cells[DCC.IndexOf("Contract")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("Y")].Text = "&nbsp;";
        }
        else if (_currentLevel == 2)
        {
            row.Cells[DCC.IndexOf("NarrativeDescription")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
            row.Cells[DCC.IndexOf("Y")].Text = "&nbsp;";
        }

        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (row.RowIndex > 0)
            {
                row.Cells[i].Style["border-top"] = "1px solid grey";
            }
        }
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        //add edit link
        string itemId = "0";
        switch (_currentLevel)
        {
            case 0:
                itemId = row.Cells[DCC.IndexOf("ProductVersionID")].Text.Replace("&nbsp;", "0").Trim();
                break;
            case 1:
                itemId = row.Cells[DCC.IndexOf("CONTRACTID")].Text == "&nbsp;" ? "0" : Server.HtmlDecode(row.Cells[DCC.IndexOf("CONTRACTID")].Text).Trim();
                break;
            case 2:
                itemId = row.Cells[DCC.IndexOf("NarrativeID")].Text.Trim();
                row.Attributes.Add("ProductVersionID", _productVersionID.ToString());
                row.Attributes.Add("CONTRACTID", _qfContractID.ToString());
                break;
        }

        if (itemId == "0")
        {
            row.Style["display"] = "none";
        }

        row.Attributes.Add("itemID", itemId);

        if (CanEdit)
        {
            TextBox obj = new TextBox();
            if (_currentLevel == 2) {
                obj = WTSUtility.CreateGridTextBox("NarrativeDescription", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("NarrativeDescription")].Text.Replace("&nbsp;", " ").Trim()), false, true);
                obj.Style["width"] = "99%";
                row.Cells[DCC.IndexOf("NarrativeDescription")].Controls.Add(obj);
                DropDownList ddl = WTSUtility.CreateGridDropdownList(dtImage, "ImageName", "ImageName", "ImageID", itemId, row.Cells[DCC.IndexOf("ImageID")].Text.Replace("&nbsp;", " ").Trim(), "", null);
                row.Cells[DCC.IndexOf("ImageID")].Controls.Add(ddl);

                bool archive = false;
                if (row.Cells[DCC.IndexOf("Archive")].HasControls()
                    && row.Cells[DCC.IndexOf("Archive")].Controls[0] is CheckBox)
                {
                    archive = ((CheckBox)row.Cells[DCC.IndexOf("Archive")].Controls[0]).Checked;
                }
                else if (row.Cells[DCC.IndexOf("Archive")].Text == "1")
                {
                    archive = true;
                }
                row.Cells[DCC.IndexOf("Archive")].Controls.Clear();
                row.Cells[DCC.IndexOf("Archive")].Controls.Add(WTSUtility.CreateGridCheckBox("Archive", itemId, archive));
            }
        }

        string dependencies = "0";
        int count = 0;

        switch (_currentLevel)
        {
            case 0:
                dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("Contract_Count")].Text).Trim();
                break;
            case 1:
                dependencies = Server.HtmlDecode(row.Cells[DCC.IndexOf("Narrative_Count")].Text).Trim();
                break;
        }

        int.TryParse(dependencies, out count);

        if (!string.IsNullOrEmpty(itemId) && itemId != "0" && _currentLevel < 2)
        {

            //add expand/collapse buttons
            HtmlGenericControl divChildren = new HtmlGenericControl();
            divChildren.Style["display"] = "table-row";
            divChildren.Style["text-align"] = "right";
            HtmlGenericControl divChildrenButtons = new HtmlGenericControl();
            divChildrenButtons.Style["display"] = "table-cell";
            divChildrenButtons.Controls.Add(createShowHideButton(true, "Show", itemId));
            divChildrenButtons.Controls.Add(createShowHideButton(false, "Hide", itemId));
            HtmlGenericControl divChildCount = new HtmlGenericControl();
            divChildCount.InnerText = string.Format("({0})", count.ToString());
            divChildCount.Style["display"] = "table-cell";
            divChildCount.Style["padding-left"] = "2px";
            divChildren.Controls.Add(divChildrenButtons);
            divChildren.Controls.Add(divChildCount);
            //buttons to show/hide child grid
            row.Cells[DCC["X"].Ordinal].Controls.Clear();
            row.Cells[DCC["X"].Ordinal].Controls.Add(divChildren);

            //add child grid row for Task Items
            Table table = (Table)row.Parent;
            GridViewRow childRow = createChildRow(itemId);
            table.Rows.AddAt(table.Rows.Count, childRow);
        }
    }

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["Narrative"] == null)
        {
            loadGridData();
        }
        else
        {
            grdMD.DataSource = (DataTable)HttpContext.Current.Session["Narrative"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != DCC.IndexOf("X")
                && i != DCC.IndexOf("ProductVersion")
                && i != DCC.IndexOf("Description")
                && i != DCC.IndexOf("Contract")
                && i != DCC.IndexOf("ImageID")
                && i != DCC.IndexOf("Sort")
                && i != DCC.IndexOf("Archive"))
            {
                row.Cells[i].Style["text-align"] = "left";
                row.Cells[i].Style["padding-left"] = "5px";
            }
            else
            {
                row.Cells[i].Style["text-align"] = "center";
            }
        }

        //more column formatting
        row.Cells[DCC.IndexOf("X")].Style["width"] = "35px";
        row.Cells[DCC.IndexOf("Sort")].Style["width"] = "55px";
        row.Cells[DCC.IndexOf("Archive")].Style["width"] = "55px";

        switch (_currentLevel)
        {
            case 0:
                row.Cells[DCC.IndexOf("ProductVersion")].Style["width"] = "100px";
                row.Cells[DCC.IndexOf("Description")].Style["text-align"] = "left";
                row.Cells[DCC.IndexOf("Archive")].Style["display"] = "none";
                break;
            case 1:
                row.Cells[DCC.IndexOf("CONTRACT")].Style["width"] = "250px";
                row.Cells[DCC.IndexOf("CONTRACT")].Style["text-align"] = "left";
                row.Cells[DCC.IndexOf("Archive")].Style["display"] = "none";
                break;
            case 2:
                row.Cells[DCC.IndexOf("WorkloadAllocationType")].Style["width"] = "150px";
                row.Cells[DCC.IndexOf("ImageID")].Style["width"] = "200px";
                row.Cells[DCC.IndexOf("X")].Style["width"] = "15px";
                row.Cells[DCC.IndexOf("Y")].Style["display"] = "none";
                break;
        }
    }

    Image createShowHideButton(bool show = false, string direction = "Show", string itemId = "ALL")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgShowHideChildren_click(this,'{0}','{1}');", direction, itemId);

        Image img = new Image();
        img.ID = string.Format("img{0}Children_{1}", direction, itemId);
        img.Style["display"] = show ? "block" : "none";
        img.Style["cursor"] = "pointer";
        img.Attributes.Add("Name", string.Format("img{0}", direction));
        img.Attributes.Add("itemId", itemId);
        img.Height = 10;
        img.Width = 10;
        img.ImageUrl = direction.ToUpper() == "SHOW"
            ? "Images/Icons/add_blue.png"
            : "Images/Icons/minus_blue.png";
        img.ToolTip = string.Format("{0} Narratives", direction);
        img.AlternateText = string.Format("{0} Narratives", direction);
        img.Attributes.Add("Onclick", sb.ToString());

        return img;
    }

    GridViewRow createChildRow(string itemId = "")
    {
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
        TableCell tableCell = null;

        try
        {
            row.CssClass = "gridBody";
            row.Style["display"] = "none";
            row.ID = string.Format("gridChild_{0}", itemId);
            if (_currentLevel == 0)
            {
                row.Attributes.Add("ProductVersionID", itemId);
            }
            else if (_currentLevel == 1)
            {
                row.Attributes.Add("CONTRACTID", itemId);
            }
            else if (_currentLevel == 2)
            {
                row.Attributes.Add("NarrativeID", itemId);
            }

            row.Attributes.Add("Name", string.Format("gridChild_{0}", itemId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (_currentLevel == 0)
                {
                    if (i == DCC["X"].Ordinal)
                    {
                        //set width to match parent
                        tableCell.Style["width"] = "32px";
                        tableCell.Style["border-right"] = "1px solid transparent";
                    }
                    else if (i == DCC["ProductVersion"].Ordinal)
                    {
                        tableCell.Style["padding-top"] = "10px";
                        tableCell.Style["padding-right"] = "10px";
                        tableCell.Style["padding-bottom"] = "0px";
                        tableCell.Style["padding-left"] = "0px";
                        tableCell.Style["vertical-align"] = "top";
                        tableCell.ColumnSpan = DCC.Count - 1;
                        //add the frame here
                        tableCell.Controls.Add(createChildFrame(itemId: itemId));
                    }
                    else
                    {
                        tableCell.Style["display"] = "none";
                    }
                }
                else
                {
                    if (i == DCC["X"].Ordinal)
                    {
                        //set width to match parent
                        tableCell.Style["width"] = "32px";
                        tableCell.Style["border-right"] = "1px solid transparent";
                    }
                    else if (i == DCC["CONTRACT"].Ordinal)
                    {
                        tableCell.Style["padding-top"] = "10px";
                        tableCell.Style["padding-right"] = "10px";
                        tableCell.Style["padding-bottom"] = "0px";
                        tableCell.Style["padding-left"] = "0px";
                        tableCell.Style["vertical-align"] = "top";
                        tableCell.ColumnSpan = DCC.Count - 1;
                        //add the frame here
                        tableCell.Controls.Add(createChildFrame(itemId: itemId));
                    }
                    else
                    {
                        tableCell.Style["display"] = "none";
                    }
                }

                row.Cells.Add(tableCell);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            row = null;
        }

        return row;
    }

    HtmlIframe createChildFrame(string itemId = "")
    {
        HtmlIframe childFrame = new HtmlIframe();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        childFrame.ID = string.Format("frameChild_{0}", itemId);
        switch (_currentLevel)
        {
            case 0:
                childFrame.Attributes.Add("ProductVersionID", itemId);
                break;
            case 1:
                childFrame.Attributes.Add("CONTRACTID", itemId);
                break;
            case 2:
                childFrame.Attributes.Add("Narrative_CONTRACTID", itemId);
                break;
        }
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["height"] = "30px";
        childFrame.Style["width"] = "100%";
        childFrame.Style["border"] = "none";

        return childFrame;
    }

    #endregion Grid

    #region excel
    private void ExportExcel(DataTable dt)
    {
        DataTable copydt = dt.Copy();
        DataSet ds = new DataSet();
        String strName = "Master Grid - Narrative";
        Workbook wb = new Workbook(FileFormatType.Xlsx);
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[0];
        ws.Name = "Master Grid - Narrative";

        DataTable table = new DataTable();
        table.Columns.AddRange(new[]
             {
         new DataColumn("ProductVersionID"),
         new DataColumn("NarrativeID")
        });

        NonPrintableDCC = table.Columns;


        int tblCnt = 0;
        string tblName = "'" + tblCnt;
        dt.TableName = tblName;
        formatTable(ref dt, tblCnt);
        ds.Tables.Add(dt);
        ExcelRowCount = 0;


        ///Get all the tables into a dataset
        for (int _currentLevel = 1; _currentLevel < MAXLEVELS; _currentLevel++)
        {
            tblCnt++;
            switch (_currentLevel)
            {
                case 1:
                    copydt = MasterData.NarrativeList_Get(productVersionID: 0, contractID: _qfContractID);
                    break;
                case 2:
                    copydt = MasterData.NarrativeList_Get(productVersionID: 0, contractID: _qfContractID);
                    break;
                case 3:
                    copydt = MasterData.Narrative_CONTRACTList_Get(productVersionID: 0);
                    break;

            }

            tblName = "'" + tblCnt;
            copydt.TableName = tblName;
            formatTable(ref copydt, tblCnt);
            ds.Tables.Add(copydt);

           
        }

        //recursivly loop and print them out
        AddRowsColumns(ws, dt, ds, 0, "");
        
        ws.Cells.DeleteColumn(0);
        ws.AutoFitColumns();
        wb.Save(ms, SaveFormat.Xlsx);
        Response.BufferOutput = true;
        Response.ContentType = "application/xlsx";
        Response.AddHeader("content-disposition", "attachment;  filename=" + strName + ".xlsx");
        Response.BinaryWrite(ms.ToArray());
        Response.End();
    }


    private void AddRowsColumns(Worksheet ws, DataTable dt, DataSet ds, int currTblIDX, string prevRowFilter)
    {
        
        if (!(currTblIDX > ds.Tables.Count))
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = prevRowFilter;
            DataTable dtFiltered = dv.ToTable();  
            DataTable parentRaw = null;
            DataColumnCollection DCC = dtFiltered.Columns;
            //Format currLevel columns
            Aspose.Cells.Style style = new Aspose.Cells.Style();
            switch (currTblIDX)
            {
                case 0:
                    style.Pattern = BackgroundType.Solid;
                    style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("DeepSkyBlue");
                    break;
                case 1:
                    style.Pattern = BackgroundType.Solid;
                    style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("DarkOliveGreen");
                    break;
                case 2:
                    style.Pattern = BackgroundType.Solid;
                    style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("DarkViolet");
                    break;
                case 3:
                    style.Pattern = BackgroundType.Solid;
                    style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("DarkGoldenrod");
                    break;

            }


            foreach (DataRow currentRow in dtFiltered.Rows)
            {    
                //Print out currLevel filtered data

                WTSUtility.importDataRow(ref parentRaw, currentRow);

                        printExcelHeader(ws, DCC, currTblIDX, style);
                        ExcelRowCount++;
                        printExcelRow(ws, currentRow, currTblIDX);
                        ExcelRowCount++;
   
                    //Create Row filter for next level
                    string nxtLVLRowFilter = "";
                    //Create a row filter for the currentTable

                    string ColNamelvl1_Narrative = "CR Report Narrative Title";
                    string ColNamelvl2_Narrative = "CR Report Narrative";
                    //string ColNamelvl3_Narrative = "Narrative";

                    switch (currTblIDX)
                    {
                        case 0:
                            //productVersionID, 
                            nxtLVLRowFilter = "productVersionID = " + currentRow["productVersionID"].ToString();
                            break;
                        case 1:            
                            //productVersionID, narrative
                            nxtLVLRowFilter = "productVersionID = " + currentRow["productVersionID"].ToString() + " AND ["+ ColNamelvl2_Narrative + "] = '" + currentRow[ColNamelvl1_Narrative].ToString() + "'";
                            break;
                        case 2:                  
                            nxtLVLRowFilter = "[NarrativeID] = " + currentRow["NarrativeID"].ToString();
                            break;

                    }
                    
                    if (currTblIDX < ds.Tables.Count - 1)
                    {
                        int nextTblIDX = currTblIDX + 1;
                        AddRowsColumns(ws, ds.Tables[nextTblIDX], ds, nextTblIDX, nxtLVLRowFilter);
                    }

            }

        }

        }


    private void formatTable(ref DataTable dt, int level)
    {
        dt.Rows[0].Delete();

        if (dt.Columns.Contains("X")) dt.Columns.Remove("X");
        if (dt.Columns.Contains("Y")) dt.Columns.Remove("Y");
        if (dt.Columns.Contains("Z")) dt.Columns.Remove("Z");     

        switch (level)
        {
            case 0:
                
                dt.Columns.Remove("CREATEDBY");
                dt.Columns.Remove("CREATEDDATE");
                dt.Columns.Remove("UPDATEDBY");
                dt.Columns.Remove("UPDATEDDATE");
                dt.Columns.Remove("Narrative_Count");
                //dt.Columns.Remove("ProductVersionID");
                dt.Columns["ProductVersion"].ColumnName = "Product Version";
                dt.Columns["Sort"].ColumnName = "Sort Order";
                break;
            case 1:
                dt.Columns["Narrative"].ColumnName = "CR Report Narrative Title";
                dt.Columns.Remove("Narrative_Count");
                break;
            case 2:
                dt.Columns.Remove("CREATEDBY");
                dt.Columns.Remove("CREATEDDATE");
                dt.Columns.Remove("UPDATEDBY");
                dt.Columns.Remove("UPDATEDDATE");
                dt.Columns.Remove("NARRATIVE_COUNT");
                dt.Columns.Remove("CONTRACT_COUNT");
                dt.Columns["Narrative"].ColumnName = "CR Report Narrative";
                break;
            case 3:
                dt.Columns.Remove("CREATEDBY");
                dt.Columns.Remove("CREATEDDATE");
                dt.Columns.Remove("UPDATEDBY");
                dt.Columns.Remove("UPDATEDDATE");
                dt.Columns.Remove("PRODUCTVERSION");
                dt.Columns.Remove("IMGDESCRIPTION");
                dt.Columns.Remove("IMGFILENAME");
                dt.Columns.Remove("NARRATIVE");
                dt.Columns.Remove("DESCRIPTION");
                dt.Columns.Remove("Narrative_CONTRACTID");
                dt.Columns.Remove("ProductVersionID");
                dt.Columns.Remove("CONTRACTID");
                dt.Columns.Remove("WorkloadAllocationID");
                dt.Columns.Remove("ImageID");
                dt.Columns["WorkloadAllocation"].ColumnName = "Workload Allocation";               
                dt.Columns["ImageName"].ColumnName = "Image";
                dt.Columns["Sort"].ColumnName = "Location";  
                dt.Columns["CONTRACT"].ColumnName = "Contract";
                break;

        }

        dt.AcceptChanges();
    }
    private void printExcelHeader(Worksheet ws, DataColumnCollection columns, int level, Aspose.Cells.Style style)
    {
        int j = (level + 1);
        //int nonPrintableColOffset = 0;
        
        for (int i = 0; i < columns.Count; i++)
        {
            if (!(NonPrintableDCC.Contains(columns[i].ColumnName)))
            { 
                ws.Cells[ExcelRowCount, j].PutValue(columns[i].ColumnName);
                ws.Cells[ExcelRowCount, j].SetStyle(style);
                j++;
            }
        }
    }

    private void printExcelRow(Worksheet ws, DataRow currentRow, int level)
    {
        int j = (level + 1);
        //int nonPrintableColOffset = 0;
        foreach (DataColumn c in currentRow.Table.Columns)
        {
            if (!(NonPrintableDCC.Contains(c.ToString())))
            {
                ws.Cells[ExcelRowCount, j].PutValue(currentRow[c].ToString());
                j++;
            }
        }

    }


    #endregion excel

    [WebMethod(true)]
    public static string SaveNarrativeChanges(string rows)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));

            int id = 0, productVersionID = 0, contractID = 0, imageID = 0, archive = 0;
            string description = string.Empty;

            HttpServerUtility server = HttpContext.Current.Server;

            foreach (DataRow dr in dtjson.Rows)
            {
                id = 0;
                productVersionID = 0;
                contractID = 0;
                description = string.Empty;
                imageID = 0;
                archive = 0;

                tempMsg = string.Empty;
                int.TryParse(dr["NarrativeID"].ToString(), out id);
                int.TryParse(dr["ProductVersionID"].ToString(), out productVersionID);
                int.TryParse(dr["ContractID"].ToString(), out contractID);
                if (dtjson.Columns.Contains("NarrativeDescription")) description = Uri.UnescapeDataString(dr["NarrativeDescription"].ToString());
                int.TryParse(dr["ImageID"].ToString(), out imageID);
                int.TryParse(dr["Archive"].ToString(), out archive);

                exists = false;
                saved = MasterData.Narrative_Update(id, productVersionID, contractID, description, imageID, archive == 1 ? true : false, out exists, out tempMsg);

                if (tempMsg.Length > 0)
                {
                    errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            saved = false;
            errorMsg = ex.Message;
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
    public static string Copy(int oldReleaseID, string newReleaseID, int contractID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        int newID = 0;
        string errorMsg = string.Empty;

        int newRelease_ID;
        string missionNarrative = string.Empty, programMGMTNarrative = string.Empty, deploymentNarrative = string.Empty, productionNarrative = string.Empty;
        int missionImageID = 0, programMGMTImageID = 0, deploymentImageID = 0, productionImageID = 0;

        int.TryParse(newReleaseID.ToString(), out newRelease_ID);

        try
        {
            DataTable dt = MasterData.NarrativeList_Get(productVersionID: oldReleaseID, contractID: contractID);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["WorkloadAllocationType"].ToString())
                    {
                        case "Mission":
                            missionNarrative = dr["NarrativeDescription"].ToString();
                            int.TryParse(dr["ImageID"].ToString(), out missionImageID);
                            break;
                        case "Program MGMT":
                            programMGMTNarrative = dr["NarrativeDescription"].ToString();
                            int.TryParse(dr["ImageID"].ToString(), out programMGMTImageID);
                            break;
                        case "Deployment":
                            deploymentNarrative = dr["NarrativeDescription"].ToString();
                            int.TryParse(dr["ImageID"].ToString(), out deploymentImageID);
                            break;
                        case "Production":
                            productionNarrative = dr["NarrativeDescription"].ToString();
                            int.TryParse(dr["ImageID"].ToString(), out productionImageID);
                            break;
                    }
                }
                saved = MasterData.Narrative_Add(
                    newRelease_ID, contractID,
                    0, missionNarrative, missionImageID,
                    0, programMGMTNarrative, programMGMTImageID,
                    0, deploymentNarrative, deploymentImageID,
                    0, productionNarrative, productionImageID,
                    false, out exists, out newID, out errorMsg);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            result["error"] = ex.Message;
        }

        result["exists"] = exists.ToString();
        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
    public static string DeleteItem(int itemId, int contractID, int productVersionID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() {
            { "id", itemId.ToString() }
            , { "exists", "" }
            , { "deleted", "" }
            , { "archived", "" }
            , { "error", "" } };
        bool exists = false, deleted = false, archived = false;
        string errorMsg = string.Empty;

        try
        {
            //delete
            if (itemId == 0)
            {
                errorMsg = "You must specify an item to delete.";
            }
            else if (itemId == -1)
            {
                DataTable dt = MasterData.NarrativeList_Get(productVersionID: productVersionID, contractID: contractID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        int.TryParse(dr["NarrativeID"].ToString(), out itemId);
                        deleted = MasterData.Narrative_Delete(itemId, out exists, out archived, out errorMsg);
                    }
                }
            }
            else
            {
                if (productVersionID > 0)
                {
                    deleted = MasterData.Narrative_Delete(itemId, out exists, out archived, out errorMsg);
                } else
                {
                    DataTable dt = MasterData.NarrativeList_Get(productVersionID: itemId);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            int.TryParse(dr["NarrativeID"].ToString(), out itemId);
                            deleted = MasterData.Narrative_Delete(itemId, out exists, out archived, out errorMsg);
                        }
                    }
                }

                //deleted = MasterData.Narrative_Delete(0, "", itemId, out exists, out archived, out errorMsg);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            deleted = false;
            errorMsg = ex.Message;
        }

        result["exists"] = exists.ToString();
        result["deleted"] = deleted.ToString();
        result["archived"] = archived.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

}