using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_CRs : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected int AORID = 0;
    protected int AORReleaseID = 0;
    protected int CurrentLevel = 1;
    protected string Filter = string.Empty;
    private XmlDocument Levels = new XmlDocument();
    protected DataColumnCollection DCC;
    protected int LevelCount = 0;
    protected int GridPageIndex = 0;
    protected int RowCount = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadData();
        ReadQueryString();
        InitializeEvents();

        this.CanEditAOR = (UserManagement.UserCanEdit(WTSModuleOption.AOR) && AOR.AORReleaseCurrent(AORID: this.AORID, AORReleaseID: this.AORReleaseID));
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewWorkItem = this.CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

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

    private void ReadData()
    {
        XmlDocument nDoc = new XmlDocument();
        string nData = string.Empty;

        nData = "<crosswalkparameters><level>";
        nData += "<breakout><column>CR CUSTOMER TITLE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>CR INTERNAL TITLE</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>CR DESCRIPTION</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>CR CONTRACT</column><sort>Ascending</sort></breakout>";
        nData += "<breakout><column>RELATED RELEASE</column><sort>Ascending</sort></breakout>";
        //nData += "<breakout><column>PRIMARY SR #</column><sort>Ascending</sort></breakout>";
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
        nData += "<breakout><column>TASK #</column><sort>Descending</sort></breakout>";
        nData += "<breakout><column>TASK TITLE</column><sort>Ascending</sort></breakout>";
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

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.AORID);
        }

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.AORReleaseID);
        }

        if (Request.QueryString["CurrentLevel"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"]))
        {
            int.TryParse(Request.QueryString["CurrentLevel"], out this.CurrentLevel);
        }

        if (Request.QueryString["Filter"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Filter"]))
        {
            this.Filter = Uri.UnescapeDataString(Request.QueryString["Filter"]);
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

        if (IsPostBack && this.CurrentLevel == 1 && Session["dtAORCR" + this.CurrentLevel] != null)
        {
            dt = (DataTable)Session["dtAORCR" + this.CurrentLevel];
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

            dt = AOR.AOR_CR_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, AORID: this.AORID, AORReleaseID: this.AORReleaseID, CRID: 0, CRRelatedRel: string.Empty, CRStatus: string.Empty, SRStatus: string.Empty, CRContract: string.Empty);
            Session["dtAORCR" + this.CurrentLevel] = dt;
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
        if (DCC.Contains("AORReleaseCRID"))
        {
            row.Attributes.Add("aorreleasecr_id", row.Cells[DCC.IndexOf("AORReleaseCRID")].Text);
        }

        if (DCC.Contains("CR_ID") && Server.HtmlDecode(row.Cells[DCC.IndexOf("CR_ID")].Text).Trim() != "")
        {
            if (DCC.Contains("CR Customer Title")) row.Cells[DCC.IndexOf("CR Customer Title")].Controls.Add(CreateLink("CR", row.Cells[DCC.IndexOf("CR_ID")].Text, row.Cells[DCC.IndexOf("CR Customer Title")].Text));
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

        if (DCC.Contains("Task #") && this.CanViewWorkItem && row.Cells[DCC.IndexOf("Task #")].Text != "&nbsp;")
        {
            row.Cells[DCC.IndexOf("Task #")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("Task #")].Controls.Add(CreateLink("Task", row.Cells[DCC.IndexOf("Task #")].Text, row.Cells[DCC.IndexOf("Task #")].Text));
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
        if (DCC.Contains("Sort")) row.Cells[DCC.IndexOf("Sort")].Style["display"] = "none";
        if (DCC.Contains("Imported")) row.Cells[DCC.IndexOf("Imported")].Style["display"] = "none";
        if (DCC.Contains("SRCount")) row.Cells[DCC.IndexOf("SRCount")].Style["display"] = "none";
        if (DCC.Contains("TaskCount")) row.Cells[DCC.IndexOf("TaskCount")].Style["display"] = "none";

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = DCC.Contains("SRCount") || DCC.Contains("TaskCount") ? "45px" : "15px";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("CR Customer Title")) row.Cells[DCC.IndexOf("CR Customer Title")].Style["width"] = "365px";
        if (DCC.Contains("CR Internal Title")) row.Cells[DCC.IndexOf("CR Internal Title")].Style["width"] = "365px";
        if (DCC.Contains("CR Description")) row.Cells[DCC.IndexOf("CR Description")].Style["width"] = "330px";
        if (DCC.Contains("CR Contract")) row.Cells[DCC.IndexOf("CR Contract")].Style["width"] = "150px";
        if (DCC.Contains("Contract")) row.Cells[DCC.IndexOf("Contract")].Style["width"] = "150px";
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
        if (DCC.Contains("Last Reply")) row.Cells[DCC.IndexOf("Last Reply")].Style["width"] = "230px";
        if (DCC.Contains("Last Reply")) row.Cells[DCC.IndexOf("Last Reply")].Style["width"] = "300px";
        if (DCC.Contains("AOR #")) row.Cells[DCC.IndexOf("AOR #")].Style["width"] = "45px";
        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["width"] = "250px";
        if (DCC.Contains("AOR Description")) row.Cells[DCC.IndexOf("AOR Description")].Style["width"] = "250px";
        if (DCC.Contains("Task #")) row.Cells[DCC.IndexOf("Task #")].Style["width"] = "55px";
        if (DCC.Contains("Task Title")) row.Cells[DCC.IndexOf("Task Title")].Style["width"] = "435px";
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
        if (DCC.Contains("Sort")) row.Cells[DCC.IndexOf("Sort")].Style["display"] = "none";
        if (DCC.Contains("Imported")) row.Cells[DCC.IndexOf("Imported")].Style["display"] = "none";
        if (DCC.Contains("SRCount")) row.Cells[DCC.IndexOf("SRCount")].Style["display"] = "none";
        if (DCC.Contains("TaskCount")) row.Cells[DCC.IndexOf("TaskCount")].Style["display"] = "none";

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Style["width"] = "15px";
            row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

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
        else
        {
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

    private LinkButton CreateTextLink(string txt, int sub)
    {
        LinkButton lb = new LinkButton();

        lb.Text = txt.Substring(0, sub) + "...";
        lb.ToolTip = txt;
        lb.Attributes["onclick"] = string.Format("showText('{0}'); return false;", Uri.EscapeDataString(txt));

        return lb;
    }

    private LinkButton CreateLink(string type, string type_ID, string type_Text)
    {
        LinkButton lb = new LinkButton();

        lb.Text = type_Text;

        switch (type)
        {
            case "CR":
                lb.Attributes["onclick"] = string.Format("openCR('{0}'); return false;", type_ID);
                break;
            case "Task":
                lb.Attributes["onclick"] = string.Format("openTask({0}); return false;", type_ID);
                break;
        }

        return lb;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string DeleteAORCR(string aorReleaseCR)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            int AORReleaseCR_ID = 0;
            int.TryParse(aorReleaseCR, out AORReleaseCR_ID);

            deleted = AOR.AORCR_Delete(AORReleaseCRID: AORReleaseCR_ID);
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
    #endregion
}