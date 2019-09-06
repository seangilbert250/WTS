﻿using Aspose.Cells;  //for exporting to excel
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using Newtonsoft.Json;

public partial class AOR_CR_Grid : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditCR = false;
    protected bool CanViewCR = false;
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected int CurrentLevel = 1;
    protected string Filter = string.Empty;
    protected string[] QFCRRelatedRel = { };
    protected string[] QFCRStatus = { };
    protected string[] QFSRStatus = { };
    protected string[] QFCRContract = { };
    private XmlDocument Levels = new XmlDocument();
    protected DataColumnCollection DCC;
    protected int LevelCount = 0;
    protected int GridPageIndex = 0;
    protected bool _export = false;
    protected int TotalCount = 0;
    protected int rowCount = 0;
    protected int dtRowCount = 0;
    protected int dtColumnCnt = 0;
    protected int uniA = (int)'A';
    protected string QFName = "";
    protected Aspose.Cells.Style style1;
    protected Aspose.Cells.Style style1A;
    protected Aspose.Cells.Style style2;
    protected Aspose.Cells.Style style3;
    protected Aspose.Cells.Style style3A;
    protected Aspose.Cells.Style style4;
    protected StyleFlag flag = new StyleFlag() { All = true };
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadData();
        ReadQueryString();
        InitializeEvents();
        LoadRelatedItemsMenu();
        if (!Page.IsPostBack) LoadQF();

        this.CanEditCR = UserManagement.UserCanEdit(WTSModuleOption.CR);
        this.CanViewCR = this.CanEditCR || UserManagement.UserCanView(WTSModuleOption.CR);
        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewWorkItem = this.CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

        DataTable dtImport = AOR.AORSRImportList_Get();

        if (dtImport != null && dtImport.Rows.Count > 0)
        {
            DateTime importDate = dtImport.AsEnumerable()
               .Select(cols => cols.Field<DateTime>("ImportDate"))
               .OrderByDescending(p => p.Ticks)
               .FirstOrDefault();

            spnLastImportDate.InnerText = "Last Import from CAFDEx: " + String.Format("{0:M/d/yyyy h:mm tt}", importDate);
        }

        DataTable dt = LoadData();
        if (dt != null)
        {
            this.DCC = dt.Columns;
            this.TotalCount = dt.Rows.Count;
        }
        if (_export)
        {

            try
            {
                //Needed if a second import is to be done
                _export = false;
                ExportExcel(dt);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
            //Always needs to be executed to prevent errors in the inner Iframe
            grdData.DataSource = dt;

            if (!Page.IsPostBack && this.GridPageIndex > 0 && this.GridPageIndex < ((decimal)dt.Rows.Count / (decimal)25)) grdData.PageIndex = this.GridPageIndex;

            grdData.DataBind();
    }

    private void ReadData()
    {
        XmlDocument nDoc = new XmlDocument();
        string nData = string.Empty;

        nData = "<crosswalkparameters><level>";
        nData += "<breakout><column>CR #</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>CR CUSTOMER TITLE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>CR INTERNAL TITLE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>CR DESCRIPTION</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>CR CONTRACT</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>RELATED RELEASE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>CAM PRIORITY</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>LCMB PRIORITY</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>AIRSTAFF PRIORITY</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>ITI PRIORITY</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>CR COORDINATION</column><sort>Ascending</sort></breakout>";
        nData += "</level><level>";
        nData += "<breakout><column>SR #</column><sort>Descending</sort></breakout>";
        nData += "<breakout><column>SUBMITTED BY</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>SUBMITTED DATE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>KEYWORDS</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>SR WEBSYSTEM</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>STATUS</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>SR TYPE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>PRIORITY</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>LCMB</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>ITI</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>SR ITI POC</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>DESCRIPTION</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>LAST REPLY</column><sort>Ascending</sort></breakout>";
        nData += "</level><level>";
        nData += "<breakout><column>AOR NAME</column><sort>Ascending</sort></breakout>";
        nData += "</level><level>";
        nData += "<breakout><column>PRIMARY TASK</column><sort>Descending</sort></breakout>";
        nData += "<breakout><column>PRIMARY TASK TITLE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>SYSTEM(TASK)</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>PRODUCT VERSION</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>PRODUCTION STATUS</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>TASK PRIORITY</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>ASSIGNED TO</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>PRIMARY RESOURCE</column><sort>Ascending</sort></breakout>";
        //nData += "<breakout><column>SECONDARY TECH. RESOURCE</column><sort>Ascending</sort></breakout>";
        //nData += "<breakout><column>PRIMARY BUS. RESOURCE</column><sort>Ascending</sort></breakout>";
        //nData += "<breakout><column>SECONDARY BUS. RESOURCE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>TASK STATUS</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>PERCENT COMPLETE</column><sort>Ascending</sort></breakout>";
        nData += "</level></crosswalkparameters>";

        nDoc.InnerXml = nData;

        this.Levels = nDoc;
        this.LevelCount = this.Levels.SelectNodes("crosswalkparameters/level").Count;
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

        if (Request.QueryString["CurrentLevel"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"]))
        {
            int.TryParse(Request.QueryString["CurrentLevel"], out this.CurrentLevel);
        }

        if (Request.QueryString["Filter"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Filter"]))
        {
            this.Filter = Uri.UnescapeDataString(Request.QueryString["Filter"]);
        }
        if (Request.QueryString["SelectedCRRelatedRelQF"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedCRRelatedRelQF"]))
        {
            this.QFCRRelatedRel = Request.QueryString["SelectedCRRelatedRelQF"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            Session["SelectedCRRelatedRelQF"] = Request.QueryString["SelectedCRRelatedRelQF"];
        }
        else if (Session["SelectedCRRelatedRelQF"] != null)
        {
            this.QFCRRelatedRel = Session["SelectedCRRelatedRelQF"].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["SelectedCRStatusesQF"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedCRStatusesQF"]))
        {
            this.QFCRStatus = Request.QueryString["SelectedCRStatusesQF"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            Session["SelectedCRStatusesQF"] = Request.QueryString["SelectedCRStatusesQF"];
        }
        else if (Session["SelectedCRStatusesQF"] != null)
        {
            this.QFCRStatus = Session["SelectedCRStatusesQF"].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["SelectedSRStatusesQF"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSRStatusesQF"]))
        {
            this.QFSRStatus = Request.QueryString["SelectedSRStatusesQF"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["SelectedCRContractsQF"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedCRContractsQF"]))
        {
            this.QFCRContract = Request.QueryString["SelectedCRContractsQF"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            Session["SelectedCRContractsQF"] = Request.QueryString["SelectedCRContractsQF"];
        }
        else if (Session["SelectedCRContractsQF"] != null)
        {
            this.QFCRContract = Session["SelectedCRContractsQF"].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["GridPageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndex"]))
        {
            int.TryParse(Request.QueryString["GridPageIndex"], out this.GridPageIndex);
        }
        if (Request.QueryString["Export"] != null &&
           !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);
        }
        if (Request.QueryString["txtSearch"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["txtSearch"]))
        {
            this.QFName = Request.QueryString["txtSearch"].Trim();
            txtCRSearch.Text = Request.QueryString["txtSearch"].Trim(); // this just resets the search box back to default value
        }
    }

    /// <summary>
    /// This will load our Related Items Menu with the data from our WTS_Menus.xml file on our server
    /// </summary>
    private void LoadRelatedItemsMenu()
    {
        try
        {
            DataSet dsMenu = new DataSet();
            dsMenu.ReadXml(this.Server.MapPath("XML/WTS_Menus.xml"));

            if (dsMenu.Tables.Count > 0 && dsMenu.Tables[0].Rows.Count > 0)
            {
                if (dsMenu.Tables.Contains("CRGridRelatedItem"))
                {
                    menuRelatedItems.DataSource = dsMenu.Tables["CRGridRelatedItem"];
                    menuRelatedItems.DataValueField = "URL";
                    menuRelatedItems.DataTextField = "Text";
                    menuRelatedItems.DataIDField = "id";
                    if (dsMenu.Tables["CRGridRelatedItem"].Columns.Contains("CRGridRelatedItem_id_0"))
                    {
                        menuRelatedItems.DataParentIDField = "CRGridRelatedItem_id_0";
                    }
                    menuRelatedItems.DataImageField = "ImageType";
                    menuRelatedItems.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
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
    private void LoadQF()
    {
        DataSet dsOptions = AOR.AOROptionsList_Get(AORID: 0, TaskID: 0, AORMeetingID: 0, AORMeetingInstanceID: 0);
        DropDownList ddlItem5 = (DropDownList)Page.Master.FindControl("ddlItem5");
        if (dsOptions != null)
        {
            DataTable dtSRStatus = dsOptions.Tables["SR Status"];

            Label lblms_Item2 = (Label)Page.Master.FindControl("lblms_Item2");
            lblms_Item2.Text = "SR Status: ";
            lblms_Item2.Style["width"] = "100px";

            HtmlSelect ms_Item2 = (HtmlSelect)Page.Master.FindControl("ms_Item2");

            ms_Item2.Items.Clear();

            foreach (DataRow dr in dtSRStatus.Rows)
            {
                ListItem liSRStatus = new ListItem(dr["Text"].ToString(), dr["Value"].ToString());
                liSRStatus.Selected = (QFSRStatus.Count() == 0 || QFSRStatus.Contains(dr["Value"].ToString()));

                //if (QFSRStatus.Count() == 0 && liSRStatus.Text.ToUpper() == "RESOLVED") liSRStatus.Selected = false;

                ms_Item2.Items.Add(liSRStatus);
            }

            DataTable dtCRStatus = dsOptions.Tables["CR Status"];

            Label lblms_Item0 = (Label)Page.Master.FindControl("lblms_Item0");
            lblms_Item0.Text = "CR Coordination: ";
            lblms_Item0.Style["width"] = "150px";

            HtmlSelect ms_Item0 = (HtmlSelect)Page.Master.FindControl("ms_Item0");

            ms_Item0.Items.Clear();

            foreach (DataRow dr in dtCRStatus.Rows)
            {
                ListItem liCRStatus = new ListItem(dr["Text"].ToString(), dr["Value"].ToString());
                liCRStatus.Selected = (QFCRStatus.Count() == 0 || QFCRStatus.Contains(dr["Value"].ToString()));

                if (QFCRStatus.Count() == 0 && liCRStatus.Text.ToUpper() == "RESOLVED") liCRStatus.Selected = false;

                ms_Item0.Items.Add(liCRStatus);
            }

            DataTable dtCRRelatedRel = dsOptions.Tables["CR Related Release"];

            Label lblms_Item1 = (Label)Page.Master.FindControl("lblms_Item1");
            lblms_Item1.Text = "Related Release: ";
            lblms_Item1.Style["width"] = "150px";

            HtmlSelect ms_Item1 = (HtmlSelect)Page.Master.FindControl("ms_Item1");

            ms_Item1.Items.Clear();

            foreach (DataRow dr in dtCRRelatedRel.Rows)
            {
                ListItem liCRRelatedRel = new ListItem(dr["Text"].ToString(), dr["Value"].ToString());
                liCRRelatedRel.Selected = (QFCRRelatedRel.Count() == 0 || QFCRRelatedRel.Contains(dr["Value"].ToString()));

                ms_Item1.Items.Add(liCRRelatedRel);
            }

            DataTable dtCRContract = dsOptions.Tables["CR Contract"];

            Label lblms_Item10 = (Label)Page.Master.FindControl("lblms_Item10");
            lblms_Item10.Text = "Contract: ";
            lblms_Item10.Style["width"] = "150px";

            HtmlSelect ms_Item10 = (HtmlSelect)Page.Master.FindControl("ms_Item10");

            ms_Item10.Items.Clear();

            foreach (DataRow dr in dtCRContract.Rows)
            {
                ListItem liCRContract = new ListItem(dr["Text"].ToString(), dr["Value"].ToString());
                liCRContract.Selected = (QFCRContract.Count() == 0 || QFCRContract.Contains(dr["Value"].ToString()));

                ms_Item10.Items.Add(liCRContract);
            }
        }
    }

    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

        if (IsPostBack && this.CurrentLevel == 1 && Session["dtCR" + this.CurrentLevel] != null)
        {
            dt = (DataTable)Session["dtCR" + this.CurrentLevel];
        }
        else
        {
            XmlDocument docLevel = new XmlDocument();
            XmlElement rootLevel = (XmlElement)docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
            XmlNode nodeLevel = this.Levels.SelectNodes("crosswalkparameters/level")[this.CurrentLevel - 1];
            XmlNode nodeImport = docLevel.ImportNode(nodeLevel, true);
            rootLevel.AppendChild(nodeImport);

            XmlDocument docFilters = new XmlDocument();
            XmlElement rootFilters = (XmlElement)docFilters.AppendChild(docFilters.CreateElement("filters"));

            if (this.Filter != string.Empty)
            {
                string[] arrFilter = this.Filter.Split('|');

                for (int j = 0; j < arrFilter.Length; j++)
                {
                    XmlElement filter = docFilters.CreateElement("filter");
                    XmlElement field = docFilters.CreateElement("field");
                    XmlElement value = docFilters.CreateElement("id");
                    string[] arrValues = arrFilter[j].Split('=');

                    field.InnerText = arrValues[0].ToString();
                    value.InnerText = (arrValues[1].ToString().Trim() == "" ? "0" : arrValues[1].ToString().Trim());

                    filter.AppendChild(field);
                    filter.AppendChild(value);
                    rootFilters.AppendChild(filter);
                }
            }

            HtmlSelect ms_Item0 = (HtmlSelect)Page.Master.FindControl("ms_Item0");
            List<string> listCRStatus = new List<string>();

            if (ms_Item0 != null && ms_Item0.Items.Count > 0)
            {
                foreach (ListItem li in ms_Item0.Items)
                {
                    if (li.Selected) listCRStatus.Add(li.Value);
                }
            }

            HtmlSelect ms_Item2 = (HtmlSelect)Page.Master.FindControl("ms_Item2");
            List<string> listSRStatus = new List<string>();
            if (this.CurrentLevel == 2)
            {
                
                if (ms_Item2 != null && ms_Item2.Items.Count > 0)
                {
                    foreach (ListItem li in ms_Item2.Items)
                    {
                        if (li.Selected) listSRStatus.Add(li.Value);
                    }
                }
            }

            HtmlSelect ms_Item1 = (HtmlSelect)Page.Master.FindControl("ms_Item1");
            List<string> listCRRelatedRel = new List<string>();

            if (ms_Item1 != null && ms_Item1.Items.Count > 0)
            {
                foreach (ListItem li in ms_Item1.Items)
                {
                    if (li.Selected) listCRRelatedRel.Add(li.Value);
                }

            }

            HtmlSelect ms_Item10 = (HtmlSelect)Page.Master.FindControl("ms_Item10");
            List<string> listCRContract = new List<string>();

            if (ms_Item10 != null && ms_Item10.Items.Count > 0)
            {
                foreach (ListItem li in ms_Item10.Items)
                {
                    if (li.Selected) listCRContract.Add(li.Value);
                }

            }

            dt = AOR.AOR_CR_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, AORID: 0, AORReleaseID: 0, CRID: 0, CRRelatedRel: String.Join(",", listCRRelatedRel), CRStatus: String.Join(",", listCRStatus), SRStatus: String.Join(",", listSRStatus), CRContract: String.Join(",", listCRContract), QFName: QFName);
            Session["dtCR" + this.CurrentLevel] = dt;
        }

        return dt;
    }
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);
        if (DCC.Contains("X")) row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage(true));
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatRowDisplay(ref row);

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage(false));

            if (DCC.Contains("SRCount"))
            {
                HtmlGenericControl spn = new HtmlGenericControl("span");

                spn.InnerHtml = "&nbsp;(" + row.Cells[DCC.IndexOf("SRCount")].Text + ")";
                row.Cells[DCC.IndexOf("X")].Controls.Add(spn);
            }

            if (DCC.Contains("TaskCount"))
            {
                HtmlGenericControl spn = new HtmlGenericControl("span");

                spn.InnerHtml = "&nbsp;(" + row.Cells[DCC.IndexOf("TaskCount")].Text.Replace("&nbsp;", "0") + ")";
                row.Cells[DCC.IndexOf("X")].Controls.Add(spn);
            }
        }
        if (DCC.Contains("CR_ID"))
        {
            row.Attributes.Add("cr_id", row.Cells[DCC.IndexOf("CR_ID")].Text);

            if (this.CanEditCR)
            {
                if (DCC.Contains("Imported") && row.Cells[DCC.IndexOf("Imported")].Text == "No")
                {
                    row.Cells[DCC.IndexOf("CR Customer Title")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("CR Customer Title")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("CR_ID")].Text, "CR Customer Title", row.Cells[DCC.IndexOf("CR Customer Title")].Text, false));

                    if (DCC.Contains("CR Internal Title"))
                    {
                        row.Cells[DCC.IndexOf("CR Internal Title")].Style["text-align"] = "center";
                        row.Cells[DCC.IndexOf("CR Internal Title")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("CR_ID")].Text, "CR Internal Title", row.Cells[DCC.IndexOf("CR Internal Title")].Text, false));
                    }
                }

                if (DCC.Contains("Sort"))
                {
                    row.Cells[DCC.IndexOf("Sort")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Sort")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("CR_ID")].Text, "Sort", row.Cells[DCC.IndexOf("Sort")].Text, true));
                }
            }
        }

        if (DCC.Contains("CR Description"))
        {
            string txtNotes = Server.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("CR Description")].Text)).Trim();

            if (txtNotes.Length > 85)
            {
                row.Cells[DCC.IndexOf("CR Description")].Controls.Add(CreateTextLink(txtNotes, 85));
            }
            else
            {
                row.Cells[DCC.IndexOf("CR Description")].Text = txtNotes;
            }
        }

        if (DCC.Contains("Description"))
        {
            string txtDescription = Server.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("Description")].Text)).Trim();

            if (txtDescription.Length > 125)
            {
                row.Cells[DCC.IndexOf("Description")].Controls.Add(CreateTextLink(txtDescription, 125));
            }
            else
            {
                row.Cells[DCC.IndexOf("Description")].Text = txtDescription;
            }
        }

        if (DCC.Contains("Contract"))
        {
            string txtContract = Server.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("Contract")].Text)).Trim();

            if (txtContract.Length > 50)
            {
                row.Cells[DCC.IndexOf("Contract")].Controls.Add(CreateTextLink(txtContract, 50));
            }
            else
            {
                row.Cells[DCC.IndexOf("Contract")].Text = txtContract;
            }
        }

        if (DCC.Contains("Last Reply"))
        {
            string txtLastReply = Server.HtmlDecode(Uri.UnescapeDataString(row.Cells[DCC.IndexOf("Last Reply")].Text)).Trim();

            if (txtLastReply.Length > 90)
            {
                row.Cells[DCC.IndexOf("Last Reply")].Controls.Add(CreateTextLink(txtLastReply, 90));
            }
            else
            {
                row.Cells[DCC.IndexOf("Last Reply")].Text = txtLastReply;
            }
        }

        if (DCC.Contains("AOR #") && this.CanViewAOR && row.Cells[DCC.IndexOf("AOR #")].Text != "&nbsp;")
        {
            row.Cells[DCC.IndexOf("AOR #")].Controls.Add(CreateLink("AOR", row.Cells[DCC.IndexOf("AOR #")].Text));
        }

        if (DCC.Contains("Primary Task") && this.CanViewWorkItem && row.Cells[DCC.IndexOf("Primary Task")].Text != "&nbsp;")
        {
            row.Cells[DCC.IndexOf("Primary Task")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("Primary Task")].Controls.Add(CreateLink("Task", row.Cells[DCC.IndexOf("Primary Task")].Text));
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

        if (DCC.Contains("AORReleaseCRID")) row.Cells[DCC.IndexOf("AORReleaseCRID")].Style["display"] = "none";
        if (DCC.Contains("SRCount")) row.Cells[DCC.IndexOf("SRCount")].Style["display"] = "none";
        if (DCC.Contains("TaskCount")) row.Cells[DCC.IndexOf("TaskCount")].Style["display"] = "none";

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = DCC.Contains("SRCount") || DCC.Contains("TaskCount") ? "45px" : "15px";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("CR Customer Title")) row.Cells[DCC.IndexOf("CR Customer Title")].Style["width"] = "300px";
        if (DCC.Contains("Sort")) row.Cells[DCC.IndexOf("Sort")].Style["width"] = "45px";
        if (DCC.Contains("Imported")) row.Cells[DCC.IndexOf("Imported")].Style["width"] = "55px";
        if (DCC.Contains("CR Internal Title")) row.Cells[DCC.IndexOf("CR Internal Title")].Style["width"] = "300px";
        if (DCC.Contains("CR Description")) row.Cells[DCC.IndexOf("CR Description")].Style["width"] = "330px";
        if (DCC.Contains("CR Contract")) row.Cells[DCC.IndexOf("CR Contract")].Style["width"] = "90px";
        if (DCC.Contains("CSD Required Now")) row.Cells[DCC.IndexOf("CSD Required Now")].Style["width"] = "85px";
        if (DCC.Contains("Related Release")) row.Cells[DCC.IndexOf("Related Release")].Style["width"] = "100px";
        if (DCC.Contains("Subgroup")) row.Cells[DCC.IndexOf("Subgroup")].Style["width"] = "90px";
        if (DCC.Contains("Design Review")) row.Cells[DCC.IndexOf("Design Review")].Style["width"] = "55px";
        if (DCC.Contains("CR ITI POC")) row.Cells[DCC.IndexOf("CR ITI POC")].Style["width"] = "120px";
        if (DCC.Contains("Customer Priority List")) row.Cells[DCC.IndexOf("Customer Priority List")].Style["width"] = "90px";
        if (DCC.Contains("Government CSRD #")) row.Cells[DCC.IndexOf("Government CSRD #")].Style["width"] = "80px";
        if (DCC.Contains("Primary SR #")) row.Cells[DCC.IndexOf("Primary SR #")].Style["width"] = "50px";
        if (DCC.Contains("CAM Priority")) row.Cells[DCC.IndexOf("CAM Priority")].Style["width"] = "50px";
        if (DCC.Contains("LCMB Priority")) row.Cells[DCC.IndexOf("LCMB Priority")].Style["width"] = "50px";
        if (DCC.Contains("Airstaff Priority")) row.Cells[DCC.IndexOf("Airstaff Priority")].Style["width"] = "50px";
        if (DCC.Contains("ITI Priority")) row.Cells[DCC.IndexOf("ITI Priority")].Style["width"] = "50px";
        if (DCC.Contains("CR Coordination")) row.Cells[DCC.IndexOf("CR Coordination")].Style["width"] = "85px";
        if (DCC.Contains("SR #")) row.Cells[DCC.IndexOf("SR #")].Style["width"] = "40px";
        if (DCC.Contains("Submitted By")) row.Cells[DCC.IndexOf("Submitted By")].Style["width"] = "120px";
        if (DCC.Contains("Submitted Date")) row.Cells[DCC.IndexOf("Submitted Date")].Style["width"] = "70px";
        if (DCC.Contains("Keywords")) row.Cells[DCC.IndexOf("Keywords")].Style["width"] = "110px";
        if (DCC.Contains("SR Websystem")) row.Cells[DCC.IndexOf("SR Websystem")].Style["width"] = "90px";
        if (DCC.Contains("Status")) row.Cells[DCC.IndexOf("Status")].Style["width"] = "120px";
        if (DCC.Contains("SR Type")) row.Cells[DCC.IndexOf("SR Type")].Style["width"] = "55px";
        if (DCC.Contains("Priority")) row.Cells[DCC.IndexOf("Priority")].Style["width"] = "55px";
        if (DCC.Contains("LCMB")) row.Cells[DCC.IndexOf("LCMB")].Style["width"] = "40px";
        if (DCC.Contains("ITI")) row.Cells[DCC.IndexOf("ITI")].Style["width"] = "35px";
        if (DCC.Contains("SR ITI POC")) row.Cells[DCC.IndexOf("SR ITI POC")].Style["width"] = "120px";
        if (DCC.Contains("Description")) row.Cells[DCC.IndexOf("Description")].Style["width"] = "400px";
        if (DCC.Contains("Last Reply")) row.Cells[DCC.IndexOf("Last Reply")].Style["width"] = "300px";
        if (DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "45px";
        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["width"] = "250px";
        if (DCC.Contains("AOR Description")) row.Cells[DCC.IndexOf("AOR Description")].Style["width"] = "250px";
        if (DCC.Contains("Primary Task")) row.Cells[DCC.IndexOf("Primary Task")].Style["width"] = "55px";
        if (DCC.Contains("Primary Task Title")) row.Cells[DCC.IndexOf("Primary Task Title")].Style["width"] = "435px";
        if (DCC.Contains("System(Task)")) row.Cells[DCC.IndexOf("System(Task)")].Style["width"] = "100px";
        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["width"] = "60px";
        if (DCC.Contains("Production Status")) row.Cells[DCC.IndexOf("Production Status")].Style["width"] = "75px";
        if (DCC.Contains("Task Priority")) row.Cells[DCC.IndexOf("Task Priority")].Style["width"] = "55px";
        if (DCC.Contains("Assigned To")) row.Cells[DCC.IndexOf("Assigned To")].Style["width"] = "110px";
        if (DCC.Contains("Primary Resource")) row.Cells[DCC.IndexOf("Primary Resource")].Style["width"] = "110px";
        if (DCC.Contains("Secondary Tech. Resource")) row.Cells[DCC.IndexOf("Secondary Tech. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Primary Bus. Resource")) row.Cells[DCC.IndexOf("Primary Bus. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Secondary Bus. Resource")) row.Cells[DCC.IndexOf("Secondary Bus. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Task Status")) row.Cells[DCC.IndexOf("Task Status")].Style["width"] = "100px";
        if (DCC.Contains("Percent Complete")) row.Cells[DCC.IndexOf("Percent Complete")].Style["width"] = "65px";

        if (DCC.Contains("Z")) row.Cells[DCC.IndexOf("Z")].Text = "";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        if (DCC.Contains("CR Customer Title")) row.Cells[DCC.IndexOf("CR Customer Title")].Text = Uri.UnescapeDataString(row.Cells[DCC.IndexOf("CR Customer Title")].Text);
        if (DCC.Contains("CR Internal Title")) row.Cells[DCC.IndexOf("CR Internal Title")].Text = Uri.UnescapeDataString(row.Cells[DCC.IndexOf("CR Internal Title")].Text);
        if (DCC.Contains("Keywords")) row.Cells[DCC.IndexOf("Keywords")].Text = Uri.UnescapeDataString(row.Cells[DCC.IndexOf("Keywords")].Text);

        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }

        if (DCC.Contains("AORReleaseCRID")) row.Cells[DCC.IndexOf("AORReleaseCRID")].Style["display"] = "none";
        if (DCC.Contains("SRCount")) row.Cells[DCC.IndexOf("SRCount")].Style["display"] = "none";
        if (DCC.Contains("TaskCount")) row.Cells[DCC.IndexOf("TaskCount")].Style["display"] = "none";

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Style["width"] = "15px";
            row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("Imported")) row.Cells[DCC.IndexOf("Imported")].Style["text-align"] = "center";
        if (DCC.Contains("CSD Required Now")) row.Cells[DCC.IndexOf("CSD Required Now")].Style["text-align"] = "center";
        if (DCC.Contains("Submitted Date")) row.Cells[DCC.IndexOf("Submitted Date")].Style["text-align"] = "center";
        if (DCC.Contains("LCMB")) row.Cells[DCC.IndexOf("LCMB")].Style["text-align"] = "center";
        if (DCC.Contains("ITI")) row.Cells[DCC.IndexOf("ITI")].Style["text-align"] = "center";
        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["text-align"] = "center";
    }

    private Image CreateImage(bool isHeader)
    {
        Image img = new Image();
        if (isHeader)
        {
            if (CurrentLevel != LevelCount)
            {
                img.Attributes["src"] = "Images/Icons/add_blue.png";
                img.Attributes["title"] = "Expand";
                img.Attributes["alt"] = "Expand";
                img.Attributes["onclick"] = "displayAllRows(this);";
            }
        }
        else {
            if (this.CurrentLevel != this.LevelCount)
            {
                img.Attributes["src"] = "Images/Icons/add_blue.png";
                img.Attributes["title"] = "Expand";
                img.Attributes["alt"] = "Expand";
                img.Attributes["onclick"] = "displayNextRow(this);";
            }
        }

        img.Attributes["height"] = "12";
        img.Attributes["width"] = "12";
        img.Style["cursor"] = "pointer";
        return img;
    }

    private TextBox CreateTextBox(string CR_ID, string type, string value, bool isNumber)
    {
        string txtValue = Server.HtmlDecode(Uri.UnescapeDataString(value)).Trim();
        TextBox txt = new TextBox();

        txt.Text = txtValue;
        txt.MaxLength = 50;
        txt.Width = new Unit(type == "Sort" ? 90 : 95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("cr_id", CR_ID);
        txt.Attributes.Add("field", type);
        txt.Attributes.Add("original_value", txtValue);
        txt.ToolTip = txtValue;
        if (isNumber)
        {
            txt.MaxLength = 5;
            txt.Style["text-align"] = "center";
        }

        return txt;
    }

    private LinkButton CreateTextLink(string txt, int sub)
    {
        LinkButton lb = new LinkButton();

        lb.Text = txt.Substring(0, sub) + "...";
        lb.ToolTip = txt;
        lb.Attributes["onclick"] = string.Format("showText('{0}'); return false;", Uri.EscapeDataString(txt));

        return lb;
    }

    private LinkButton CreateLink(string type, string value)
    {
        LinkButton lb = new LinkButton();

        lb.Text = value;

        switch (type)
        {
            case "AOR":
                lb.Attributes["onclick"] = string.Format("openAOR('{0}'); return false;", value);
                break;
            case "Task":
                lb.Attributes["onclick"] = string.Format("openTask('{0}'); return false;", value);
                break;
        }

        return lb;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string GetMetrics(string CRContractQF)
    {
        DataTable dt = new DataTable();

        try
        {
            dt = AOR.AORCRLookupMetrics_Get(CRContract: CRContractQF);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string DeleteCR(string cr)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int CR_ID = 0;
            int.TryParse(cr, out CR_ID);

            deleted = AOR.AORCRLookup_Delete(CRID: CR_ID);
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

            saved = AOR.AORCRLookup_Update(Changes: docChanges);
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

    [WebMethod()]
    public static string Search(string cr)
    {
        DataTable dt = new DataTable();
        int primarySR = -1;
        try
        {
            int.TryParse(cr, out primarySR);

            dt = AOR.AORCRList_Search(PrimarySR: primarySR);
            dt = dt.DefaultView.ToTable();

            dt.AcceptChanges();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }
    #endregion

    #region Excel


    private void ExportExcel(DataTable currTbl)
    {

 			DataSet setOfAllTables = new DataSet();
            DataSet dsCopy = new DataSet();
            DataTable dtCopy = new DataTable();

            //Initialize Styles

            style1 = getCurrentStyle();
            style1.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#F4B084");
            style1.Font.IsBold = true;


            style2 = getCurrentStyle();
            style2.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#0070C0");
            style2.Font.Color = System.Drawing.Color.White;
            style2.Font.IsBold = true;
            style3 = getCurrentStyle();
            style3.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#C9C9C9");
            style4 = getCurrentStyle();
            style4.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#BDD7EE");

            //Styles for Cells
            style1A = getCurrentStyle("Cell");
            style1A.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#EDEDED");
            style3A = getCurrentStyle("Cell");
            style3A.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#EDEDED");

            LevelCount = Levels.SelectNodes("crosswalkparameters/level").Count;
            Workbook wb = new Workbook(FileFormatType.Xlsx);
            Worksheet ws = wb.Worksheets[0];
            XmlDocument docLevel = new XmlDocument();
            XmlElement rootLevel = (XmlElement)docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
            var myLevels = Levels.SelectNodes("crosswalkparameters/level");
            int currTBL_LVL_IDX = 0;

            //Create the parameters for the DataPull
            HtmlSelect ms_Item2 = (HtmlSelect)Page.Master.FindControl("ms_Item2");
            List<string> listSRStatus = new List<string>();
            if (ms_Item2 != null && ms_Item2.Items.Count > 0)
            {
                foreach (ListItem li in ms_Item2.Items)
                {
                    if (li.Selected) listSRStatus.Add(li.Value);
                }
            }
            HtmlSelect ms_Item0 = (HtmlSelect)Page.Master.FindControl("ms_Item0");
            List<string> listCRStatus = new List<string>();

            if (ms_Item0 != null && ms_Item0.Items.Count > 0)
            {
                foreach (ListItem li in ms_Item0.Items)
                {
                    if (li.Selected) listCRStatus.Add(li.Value);
                }
            }

            HtmlSelect ms_Item1 = (HtmlSelect)Page.Master.FindControl("ms_Item1");
            List<string> listCRRelatedRel = new List<string>();

            if (ms_Item1 != null && ms_Item1.Items.Count > 0)
            {
                foreach (ListItem li in ms_Item1.Items)
                {
                    if (li.Selected) listCRRelatedRel.Add(li.Value);
                }
            }

            HtmlSelect ms_Item10 = (HtmlSelect)Page.Master.FindControl("ms_Item10");
            List<string> listCRContract = new List<string>();

            if (ms_Item10 != null && ms_Item10.Items.Count > 0)
            {
                foreach (ListItem li in ms_Item10.Items)
                {
                    if (li.Selected) listCRContract.Add(li.Value);
                }
            }

            foreach (XmlNode nodeLevel in myLevels)
            {
                XmlNode nodeImport = docLevel.ImportNode(nodeLevel, true);
                rootLevel.AppendChild(nodeImport);

                XmlDocument docFilters = new XmlDocument();
                XmlElement rootFilters = (XmlElement)docFilters.AppendChild(docFilters.CreateElement("filters"));

                currTbl = AOR.AOR_CR_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, AORID: 0, AORReleaseID: 0, CRID: 0, CRRelatedRel: String.Join(",", listCRRelatedRel), CRStatus: String.Join(",", listCRStatus), SRStatus: string.Empty, CRContract: String.Join(",", listCRContract));

                currTBL_LVL_IDX += 1;

                string tblName = "'" + currTBL_LVL_IDX;
                currTbl.TableName = tblName;
                setOfAllTables.Tables.Add(currTbl);

            }


            int count = setOfAllTables.Tables.Count;
            object[] curfilterCols = new object[0];

            dtColumnCnt = 0;

            DataSet dsOut = new DataSet();
            object[] colsToRemove = new object[0];
            filterTables(ref setOfAllTables, ref dsOut, 0, colsToRemove);


            AddRowsColumns(ws, 0, 0, curfilterCols, setOfAllTables.Tables[0], setOfAllTables, -1);

            MemoryStream ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Xlsx);

            Response.Clear();
            Response.ContentType = "application/xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + "CR Grid" + ".xlsx");
            Response.BinaryWrite(ms.ToArray());
            Response.End();

    }

    private Aspose.Cells.Style getCurrentStyle(string styleType = "Header")
    {

        Aspose.Cells.Style currStyle = new Aspose.Cells.Style();

        //Styles for Headers
        currStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, System.Drawing.Color.Black);
        currStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, System.Drawing.Color.Black);
        currStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, System.Drawing.Color.Black);
        currStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, System.Drawing.Color.Black);
        currStyle.Pattern = BackgroundType.Solid;
        currStyle.Font.Name = "Calibri";
        currStyle.Font.Size = 11;

        if (styleType == "Cell")
        {
            currStyle = new Aspose.Cells.Style();
            currStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, System.Drawing.Color.Black);
            currStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, System.Drawing.Color.Black);
            currStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, System.Drawing.Color.Black);
            currStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, System.Drawing.Color.Black);
            currStyle.Pattern = BackgroundType.Solid;
            currStyle.Font.Name = "Calibri";
            currStyle.Font.Size = 11;
            currStyle.VerticalAlignment = TextAlignmentType.Top;
        }

        return currStyle;
    }

    private void filterTables(ref DataSet dsIn, ref DataSet dsOut, int currTableLVLIDX, object[] currColsToRemove)
    {

        //loop through and filter all tables
        if (currTableLVLIDX < dsIn.Tables.Count)
        {

            //Create an array for nextLevelColsToRemove from currTable
            DataTable currDt = dsIn.Tables[currTableLVLIDX];
            object[] nextLevelColsToRemove = new object[currDt.Columns.Count];
            foreach (DataColumn currCol_In_CurrTable in currDt.Columns.Cast<DataColumn>().ToList())
            {
                if (currCol_In_CurrTable.ColumnName.ToString() == "X" || currCol_In_CurrTable.ColumnName.ToString() == "Z")
                {
                    currDt.Columns.Remove(currCol_In_CurrTable.ColumnName);
                }
                else if (!(currCol_In_CurrTable.ToString().EndsWith("_ID")))
                {
                    nextLevelColsToRemove[currCol_In_CurrTable.Ordinal] = currCol_In_CurrTable.ToString();
                }
            }

            //currColsToRemove is created from the previous level's nextLevelColsToRemove
            //Remove cols from currTable from the array in currColsToRemove
            for (int n = 0; n < currColsToRemove.Length; n++)
            {
                if (currColsToRemove[n] != null)
                {
                    if (!((currColsToRemove[n].ToString() == "TaskCount") || (currColsToRemove[n].ToString() == "SR #")))
                    {

                        currDt.Columns.Remove(currColsToRemove[n].ToString());
                    }
                }
            }

            currTableLVLIDX++;
            filterTables(ref dsIn, ref dsOut, currTableLVLIDX, nextLevelColsToRemove);
        }

    }

    private void AddRowsColumns(Worksheet ws, int startRow, int endRow, object[] currRowFilterColumns, DataTable currDataTable, DataSet setOfAllTables, int currTBL_LVL_IDX)
    {
        currTBL_LVL_IDX++;
        string curCell = "";

        if (!(currTBL_LVL_IDX > setOfAllTables.Tables.Count + 1))
        {
            Aspose.Cells.Style currHeaderStyle = null;
            Aspose.Cells.Style currCellStyle = null;
            setStyles(currTBL_LVL_IDX, ref currHeaderStyle, ref currCellStyle);

            //Create the row filter for currTable to get the parent rows children
            //If no rows are found stop recursion
            string rowFilter = "";
            for (int j = 0; j < currRowFilterColumns.Length; j++)
            {
                if (rowFilter == "")
                {
                    rowFilter = currRowFilterColumns[j].ToString();
                }
                else {
                    if (currRowFilterColumns[j].ToString() != "")
                    {
                        rowFilter = rowFilter + " and " + currRowFilterColumns[j];
                    }
                }
            }

            DataView dv = currDataTable.DefaultView;
            dv.RowFilter = rowFilter;
            DataTable currTableFiltered = dv.ToTable();

            if (currTBL_LVL_IDX != 2)
            {
                rowCount++;
            }

            int currExcelColCharOffset = 0;
            int curColCnt = 0;

            //AddRowsColumns-START 1: WRITE COLHEADERS FOR CURR_TBL (if currTBL_LVL_IDX != 2)

            if (currTBL_LVL_IDX != 2)
            {
                foreach (DataColumn column in currTableFiltered.Columns)
            {

                if (!(column.ToString().EndsWith("_ID")))
                {
                    //curColCnt++;
                    string cellName = mergeCells1(ref curCell, ref currExcelColCharOffset);
                    ws.Cells[cellName].PutValue(column.ToString());
                    ws.Cells[cellName].SetStyle(currHeaderStyle);   
                }
            }
            }
            if (currTBL_LVL_IDX != 2)
            {
                if (curColCnt == 1)
                {
                    mergeCells2(curCell, ws, currHeaderStyle);
                }
            }

            //End AddRowsColumns-START 1

            foreach (DataRow row in currTableFiltered.Rows)
            {

                int startCnt = 0;
                //AddRowsColumns-START 1a: WRITE COLHEADERS FOR CURR_TBL (if currTBL_LVL_IDX == 2)
                if (currTBL_LVL_IDX == 2)
                {
                    rowCount++;
                    int currExcelColCharOffset2 = 0;
                    foreach (DataColumn column in currTableFiltered.Columns)
                    {
                        if (!(column.ToString().EndsWith("_ID")))
                        {
                            string cellName = mergeCells1(ref curCell, ref currExcelColCharOffset2);
                            ws.Cells[cellName].PutValue(column.ToString());
                            ws.Cells[cellName].SetStyle(currHeaderStyle);
                        }
                    }
                }
                if (currTBL_LVL_IDX == 2)
                {
                    if (curColCnt == 1)
                    {
                        mergeCells2(curCell, ws, currHeaderStyle);
                    }
                }
                //END AddRowsColumns-START 1a

                startCnt = rowCount;
                rowCount++;
                int currExcelColCharOffset3 = 0;

                object[] nextLvlRowFilterColumns = new object[currTableFiltered.Columns.Count];
                foreach (DataColumn currColName in currTableFiltered.Columns)
                {
                    object currRowColValue = row[currColName];
                    if (currColName.ToString().EndsWith("_ID"))
                    {
                        //AddRowsColumns-START 2A: Create a unique Filter for each Row (Helps find current rows children and nextLVL)
                        var colObj = " isnull([" + currColName.ToString() + "], 0) =  '" + (currRowColValue.ToString() == "" ? "0" : currRowColValue.ToString()) + "'";
                        nextLvlRowFilterColumns[currColName.Ordinal] = colObj;
                        //END AddRowsColumns-START 2A
                    }
                    else
                    {
                        //AddRowsColumns-START 2B: WRITE THE ROW VALUE TO EXCEL WITH PROPER STYLE
                        nextLvlRowFilterColumns[currColName.Ordinal] = "";

                        string cellName = mergeCells1(ref curCell, ref currExcelColCharOffset3);
                        ws.Cells[cellName].PutValue(Uri.UnescapeDataString(currRowColValue.ToString()));
                        
                        if (Uri.UnescapeDataString(currRowColValue.ToString()).Length > 50)
                        {
                            // Setting Style properties like border, alignment etc.
                            currCellStyle.IsTextWrapped = true;
                            // Setting the style of the cell with the customized Style object
                            ws.Cells.SetColumnWidth(currExcelColCharOffset3, 50);
                        }

                        ws.Cells[cellName].SetStyle(currCellStyle);
                        currCellStyle.IsTextWrapped = false;
                        //END AddRowsColumns-START 2B:
                    }
                }

                if (curColCnt == 1)
                {
                    mergeCells2(curCell, ws, currCellStyle);
                }

                if (currTBL_LVL_IDX < setOfAllTables.Tables.Count - 1)
                {

                    //This logic mirrors the view dataPull by looking for lower levels if (!((SR_Count == 0) && (Task_Count == 0)))
                    int SR_Count = 0;
                    int Task_Count = 0;

                    if (setOfAllTables.Tables[currTBL_LVL_IDX].Columns.IndexOf("SRCount") > -1)
                    {
                        int.TryParse(row.ItemArray[setOfAllTables.Tables[currTBL_LVL_IDX].Columns.IndexOf("SRCount")].ToString(), out SR_Count);
                    }

                    if (setOfAllTables.Tables[currTBL_LVL_IDX].Columns.IndexOf("TaskCount") > -1)
                    {
                        int.TryParse(row.ItemArray[setOfAllTables.Tables[currTBL_LVL_IDX].Columns.IndexOf("TaskCount")].ToString(), out Task_Count);
                    }

                    if (!((SR_Count == 0) && (Task_Count == 0)))
                    {
                        AddRowsColumns(ws, startCnt, currTableFiltered.Rows.Count + 1, nextLvlRowFilterColumns, setOfAllTables.Tables[currTBL_LVL_IDX + 1], setOfAllTables, currTBL_LVL_IDX);
                    }
                }
                if (startCnt > 0)
                {
                    if (currTBL_LVL_IDX != setOfAllTables.Tables.Count)
                    {
                        if (ws.Cells.MaxDataRow >= startCnt + 1 && ws.Cells.MaxDataRow >= rowCount - 1)
                        {
                            ws.Cells.GroupRows(startCnt + 1, rowCount - 1, false);
                        }
                    }
                }
            }

            // Setting SummaryRowBelow property to false
            ws.Outline.SummaryRowBelow = false;
            // ws.Cells.SetColumnWidth(5, 255);
        }
    }

    public void setStyles(int currTBL_LVL_IDX, ref Aspose.Cells.Style HeaderStyle, ref Aspose.Cells.Style CellStyle)
    {
        if (currTBL_LVL_IDX == 0)
        {
            HeaderStyle = style1;
            CellStyle = style1A;
        }
        else if (currTBL_LVL_IDX == 1)
        {
            HeaderStyle = style2;
            CellStyle = style2;
        }
        else if (currTBL_LVL_IDX == 2)
        {
            HeaderStyle = style3;
            CellStyle = style3A;
        }
        else if (currTBL_LVL_IDX == 3)
        {
            HeaderStyle = style4;
            CellStyle = style4;
        }

    }

    public string mergeCells1(ref string curCell, ref int currExcelColCharOffset)
    {
        string cellName = "";
        char uniSel = (char)(uniA + currExcelColCharOffset);
        cellName = (char)uniSel + "";
        if (currExcelColCharOffset > 26)
        {
            cellName = (char)(uniA + 0) + "" + (char)uniSel;
        }
        if (currExcelColCharOffset > 52)
        {
            cellName = (char)(uniA + 1) + "" + (char)uniSel;
        }
        cellName = cellName + (rowCount).ToString();
        curCell = cellName;
        currExcelColCharOffset++;

        return cellName;
    }


    public void mergeCells2(string curCell, Worksheet ws, Aspose.Cells.Style curStyle)
    {
        string cellName = "";
        char uniSel = (char)(uniA + dtColumnCnt - 1);
        cellName = (char)uniSel + "";
        if (dtColumnCnt - 1 > 26)
        {
            cellName = (char)(uniA + 0) + "" + (char)uniSel;
        }
        if (dtColumnCnt - 1 > 52)
        {
            cellName = (char)(uniA + 1) + "" + (char)uniSel;
        }
        cellName = cellName + (rowCount).ToString();
        // Create a range
        Range range = ws.Cells.CreateRange(curCell + ":" + cellName);
        range.ApplyStyle(curStyle, flag);

        // Merge range into a single cell
        range.Merge();
    }
    #endregion
}