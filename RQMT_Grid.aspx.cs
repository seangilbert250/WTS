﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Aspose.Cells;
using Newtonsoft.Json;
using WTS.Util;

/// <summary>
/// RQMTs have 3 main levels.
/// 1) The RQMT itself. This is a text description, and represents a RQMT.
/// 2) The RQMTSystem. A RQMT added to a System (e.g. REQ1 in CAFDEX). The RQMTSystem table is the root for attributes such as Criticality and Status, and also the foreign key for Defects, Revisions, and Descriptions.
///    If RQMTs are moved from one system to another RQMT Set with the same system (but a different Work Area or RQMT Type), the Criticality and FK data goes with it 
///    (the RQMTSystem gets added to a new RQMT Set, but the RQMTSystem doesn't change).
/// 3) The RQMTSet. RQMTSystems get added to RQMTSets. This is handled in the RQMTSet_RQMTSystem join table. AT the RQMTSet level, RQMTs get Usage data, Functionalities, and also have an order and can have
///    parent-child relationships. When RQMTSystems get added and moved between RQMTSets, the RQMTSystem data may stay intact, but the set-level data (usage, parent-child) is typcially lost.
///    
/// When RQMTs are displayed in the grid, we are always displaying the base requirement itself. In other words, we are displaying RQMT.RQMT or RQMT.RQMTID, or RQMT.RQMTACCEPTED, or RQMT.RQMTCRITICALITY (via joins). 
/// The only exception occurs when a user adds RQMT PRIMARY (or RQMT PRIMARY #) to the column list. There are two levels of RQMTS, top level or child level (e.g. 1.0, 1.1, 2.0, 2.1, 2.2, 2.3, 3.0, 4.0).
/// 
/// There are 4 patterns of RQMTs a user can create with the level designer:
/// 1) RQMT (show bottom level RQMTs, which are any RQMT without children - L1 without children and any L2)
/// 2) RQMT PRIMARY (all L1)
/// 3) RQMT PRIMARY (all L1)
///       RQMT (children of the L1 item)
/// 4) RQMT PRIMARY + RQMT (ALL RQMTs - in this case, we show the RQMT row with it's parent RQMT # - or if the parent does not exist, we just repeat the RQMT #)
/// 
/// It is important to realize that when we have RQMT PRIMARY on a row by itself, we are referring to RQMTs that are top level, but all properties are still just the RQMT.RQMT values. We aren't referring
/// to RQMTPARENT.RQMT values. This changes with Option #4, in which case we show RQMT values, and also show the RQMTPARENT.RQMT #/NAME - but still, the rest of the row being displayed is still just the
/// RQMT itself.
/// 
/// Because the behaviors of the above structures differ from each other, as well as when they are needed in the WHERE clauses, as well as when they are needed in ROW COUNT queries, we use different
/// names for the breakouts when sending into the database. This is done by modifying the Level XML before it gets sent to the database, and it allows us to know the context of a column many levels
/// deep after being passed in on the query string (so we know where the column originated from, which we can't do if we always send RQMT_ID to child framesets). The naming convention is as follows:
/// 1) RQMT / RQMT #
/// 2) RQMT PRIMARY / RQMT PRIMARY #
/// 3) RQMTPRIMARYNESTED + RQMTPRIMARYNESTEDNUMBER
///        RQMTNESTED + RQMTNESTEDNUMBER
/// 4) RQMTPRIMARYCOMBINEDNAME / RQMTPRIMARYCOMBINEDNUMBER + RQMT / RQMT #
/// 
/// If we have a drilldown after #1, the where clause will point to the RQMTID column, not to the parent column. For example, if we drilldown on RQMT 50 to see all the Descriptions under it, we pull
/// back all description rows where RQMTID = 50. We do the same thing in the where clauses for #2. Even though the column is called RQMT PRIMARY, we are still looking at RQMT 50 if it is a L1 RQMT. When
/// we drilldown, we still want to find rows where RQMTID = 50.
/// 
/// If we have a drilldown after RQMT PRIMARY in #3 (lets say PARENT=50 and CHILD=65), we instead are looking at all RQMTs that have a PARENTID = 50. After the second level, are looking at 
/// PARENTID = 50 and RQMTID = 65.
/// 
/// If we have a drilldown after #4, we are looking at PARENTID = 50 and RQMTID = 65. HOWEVER, note that if there is NO parent, a RQMT will list itself as BOTH PRIMARY RQMT and RQMT. For example, the
/// column list would show 50 + 65 if there is an actual parent, or 75 75 if there is not. In the no-parent case, we ignore the fake primary value (e.g. we make
/// sure the where clause doesn't show PARENTID = 75 and RQMTID = 75 since the RQMT PRIMARY value is just a copy of the RQMT VALUE).
/// 
/// The last major concept to understand with the RQMT queries is the RQMTMODE value. This RQMTMODE value allows us to tailor the WHERE clause of the main query to pull back the rows we need. 
/// The values are:
/// For #1, the RQMT mode is "rqmtbottoms". Whether the query is at a level before RQMT or on or after RQMT, it always shows bottom level RQMTs only.
/// For #2, the RQMT mode is "rqmttops". This mode tells the query to always show L1's.
/// For #3, we use multiple modes. Prior to reaching RQMT PRIMARY, we use "rqmttops". This restricts all queries to showing L1's only, and only L1 data will show up in the drilldowns up to that point.
/// For all queries beyond the RQMT PRIMARY level, we use "rqmtbottomsexclusive", which is different than "rqmtbottoms" in that the RQMT's are filtered to only RQMT's with a parent (and the column
/// where clause will be adding a rparent.RQMTID = RQMTPRIMARYID anyway).
/// For #4, we use a RQMT mode of "combined", which basically means show everything possible.
/// 
/// Lastly, we have one minor exception to the RQMTMode. Normally, when we have a parent/child structure, we enforce that only parent rqmts are counted in
/// counts and drilldowns. However, when a user includes the RQMTSetUsage column, we want to roll up all children of a set, not just top level RQMTs in that set. So if a user
/// adds the RQMTSetUsage column on a row with system, workarea, and rqmt type, and they have a #3 structure below it, we manually force RQMTMode="combined". This allows the main set row
/// to see "everything", rather than just parent values.
/// 
/// -------------------------------------------
/// 
/// When editing and managing the rqmt crosswalk queries, there are 3 use cases that have to be handled and tested:
/// 1) Normal query and the drill-downs from level to level
/// 2) Child "count" queries to run the counts at each level (this is ran aftre the normal query to bring back counts for the next level)
/// 3) Excel export queries (runs each level once, with ID columns from each level being passed into the column list for the next level)
/// </summary>
public partial class RQMT_Grid : System.Web.UI.Page
{
    #region Variables
    protected bool ReadOnly = false; // if true, this overrides normal settings to provide a read-only view
    protected string PageType = string.Empty;
    private bool MyData = true;
    protected bool CanEditRQMT = false;
    protected bool CanAddRQMT = false;
    protected bool CanViewRQMT = false;
    protected string View = string.Empty;
    protected int CurrentLevel = 1;
    protected string Filter = string.Empty;
    protected bool NeedsWhereExt = false;
    protected string WhereExt = string.Empty;
    protected string HideColumns = string.Empty; // if specified, allows certain columns to be hidden from the query string; you can also specify a specific view to hide them in &HideColumns=VIEW=COLUMN,VIEW=COLUMN (escape the parameters in the url "VIEW%3DCOLUMN")
    protected bool IsConfigured = false;
    private XmlDocument Levels = new XmlDocument();
    protected DataColumnCollection DCC;
    protected int LevelCount = 0;
    protected int GridPageIndex = 0;
    
    protected int SystemSuiteID = 0;
    protected int WTS_SYSTEMID = 0;
    protected int RQMTTypeID = 0;
    protected int WorkAreaID = 0;

    protected int FilterRQMTSetID = 0;
    protected int FilterParentRQMTID = 0;
    protected int FilterSystemID = 0;

    protected bool Export = false;
    protected bool HidePlusSignInHeader = false;
    protected bool HideBuilderButton = false;

    protected bool RQMTIDDrilldownReferencesParent = false;
    protected bool NextLevelRQMTIDDrilldownReferencesParent = false;
    protected bool CountColumnsRQMTDrilldownReferencesParent = false;

    protected int SystemLevel = -1;
    protected int WorkAreaLevel = -1;
    protected int RQMTTypeLevel = -1;
    protected int RQMTSetLevel = -1;
    protected int RQMTSetUsageLevel = -1;
    protected int RQMTSetComplexityLevel = -1;
    protected int RQMTSetComplexityJustificationLevel = -1;
    protected int RQMTComplexityLevel = -1;
    protected int RQMTComplexityJustificationLevel = -1;
    protected int RQMTUsageMonthLevel = -1;
    protected int RQMTSetMonthCheckBoxLevel = -1; // this is the level we are showing the month checkboxes on (appears when a rqmt set is on same level as rqmtsetusage)

    protected int RQMTLevel = -1;
    protected int PrimaryRQMTLevel = -1;

    protected int RQMTNumberLevel = -1;
    protected int PrimaryRQMTNumberLevel = -1;

    protected string RQMTMode = "";
    protected string NextLevelRQMTMode = "";
    protected string CountColumnsRQMTMode = "";

    protected int rowCount = 0;
    protected int dtRowCount = 0;
    protected int dtColumnCnt = 0;
    protected int uniA = (int)'A';
    protected int gridRowCnt = 0;

    protected DataTable dtRQMTAttribute;
    protected DataTable dtRQMTComplexity;
    protected bool CanEditRQMSystemOrSet = false;
    protected string FilterToSets = "";

    protected string CopiedRQMTs = "";
    protected string CopiedRQMTSystems = "";

    protected bool IgnoreUserFilters = false; // when true, user filters are always ignored (used for some external pages that need quick snapshots into RQMT data without dealing with user filters)
    protected bool ShowExport = true;
    protected bool ShowCOG = true;
    protected bool ShowSelectCheckboxes = true;
    protected bool UseGridSession = true;
    protected bool ShowPageTitle = true;

    protected string SessionPageKey = "";

    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        LoadLookupData();

        string errors = ParseViewXML();
        InitializeEvents();        

        this.CanEditRQMT = UserManagement.UserCanEdit(WTSModuleOption.RQMT) && !ReadOnly;
        this.CanViewRQMT = this.CanEditRQMT || UserManagement.UserCanView(WTSModuleOption.RQMT);

        if (Export)
        {
            if (string.IsNullOrWhiteSpace(errors))
            {
                ExportToExcel();
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Results", typeof(string));
                dt.Rows.Add(new string[] { errors });
                ExportToExcel(dt);
            }
        }
        else
        {
            DataTable dt = null;

            if (string.IsNullOrWhiteSpace(errors))
            {
                dt = LoadData(this.CurrentLevel, this.Levels, this.Filter);

                if (dt.Rows.Count == 0)
                {
                    dt = new DataTable();
                    dt.Columns.Add("Results", typeof(string));
                    dt.Columns.Add("Z", typeof(string));
                    dt.Rows.Add(new string[] { "No data found.", "" });
                }
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("Results", typeof(string));
                dt.Columns.Add("Z", typeof(string));
                dt.Rows.Add(new string[] { errors, "" });
            }

            if (dt != null)
            {
                this.DCC = dt.Columns;
            }

            PreProcessData(dt);
            grdData.DataSource = dt;

            if (!Page.IsPostBack && this.GridPageIndex > 0 && this.GridPageIndex < ((decimal)dt.Rows.Count / (decimal)25)) grdData.PageIndex = this.GridPageIndex;

            if (RQMTSetLevel > 0 && RQMTSetLevel < CurrentLevel) // we are under a rqmt set row
            {
                if (RQMTSetLevel == SystemLevel && RQMTSetLevel == WorkAreaLevel && RQMTSetLevel == RQMTTypeLevel) // validate that we also have sys/wa/rt
                {
                    if (CurrentLevel == RQMTLevel || CurrentLevel == PrimaryRQMTLevel) // we are on a rqmt row
                    {
                        if (CanEditRQMT && !ReadOnly)
                        {
                            CanAddRQMT = true;
                        }
                    }
                }
            }

            grdData.DataBind();

            if (Session[SessionPageKey + "RQMT_Parameter_Page_Displayed"] == null && ShowCOG)
            {
                IsConfigured = false;
                Session[SessionPageKey + "RQMT_Parameter_Page_Displayed"] = "displayed";
            }
            else
            {                
                IsConfigured = true;
            }

            if (CurrentLevel > 1)
            {
                var headerDiv = (System.Web.UI.HtmlControls.HtmlControl)Page.Master.FindControl("pageContentHeader");
                headerDiv.Style.Add("display", "none"); // setting this server-side to avoid flickering (we hide it after document.read)
                IsConfigured = true; // we never show the configuration popup on drilldowns
            }
        }
    }

    private void ReadQueryString()
    {

        this.SessionPageKey = string.IsNullOrWhiteSpace(Request.QueryString["SessionPageKey"]) ? "" : Request.QueryString["SessionPageKey"] + "_";

        if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
            || Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
        {
            this.MyData = true;
        }
        else
        {
            this.MyData = false;
        }

        if (Request.QueryString["IsConfigured"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["IsConfigured"]))
        {
            bool.TryParse(Request.QueryString["IsConfigured"], out IsConfigured);
        }

        if (Request.QueryString["View"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["View"]))
        {
            this.View = Request.QueryString["View"];
        }

        if (Request.QueryString["CurrentLevel"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["CurrentLevel"]))
        {
            int.TryParse(Request.QueryString["CurrentLevel"], out this.CurrentLevel);
        }

        if (Request.QueryString["Filter"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Filter"]))
        {
            string uriFilters = Uri.UnescapeDataString(Request.QueryString["Filter"]);

            string[] filters = uriFilters.Split('|');

            string rsetid = filters.FirstOrDefault(str => str.StartsWith("RQMTSET_ID"));
            if (rsetid != null) FilterRQMTSetID = Int32.Parse(rsetid.Split('=')[1]);

            string prqmtid = filters.FirstOrDefault(str => str.StartsWith("RQMTPRIMARYNESTEDNUMBER_ID"));
            if (prqmtid != null) FilterParentRQMTID = Int32.Parse(prqmtid.Split('=')[1]);

            string sysid = filters.FirstOrDefault(str => str.StartsWith("SYSTEM_ID"));
            if (sysid != null) FilterSystemID = Int32.Parse(sysid.Split('=')[1]);

            this.Filter = uriFilters;
        }

        if (Request.QueryString["WhereExt"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["WhereExt"]))
        {
            this.WhereExt = Uri.UnescapeDataString(Request.QueryString["WhereExt"]);
        }

        if (Request.QueryString["GridPageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndex"]))
        {
            int.TryParse(Request.QueryString["GridPageIndex"], out this.GridPageIndex);
        }
        
        if (Request.QueryString["Export"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
        {
            this.Export = Request.QueryString["Export"].ToUpper() == "TRUE";
        }

        if (Request.QueryString["FilterToSets"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["FilterToSets"]))
        {
            this.FilterToSets = Request.QueryString["FilterToSets"];
        }

        this.ReadOnly = Request.QueryString["ReadOnly"] == "true";
        this.HideBuilderButton = Request.QueryString["HideBuilderButton"] == "true";
        this.IgnoreUserFilters = Request.QueryString["IgnoreUserFilters"] == "true";
        this.ShowExport = string.IsNullOrWhiteSpace(Request.QueryString["ShowExport"]) || Request.QueryString["ShowExport"] == "true";
        this.ShowCOG = string.IsNullOrWhiteSpace(Request.QueryString["ShowCOG"]) || Request.QueryString["ShowCOG"] == "true";
        this.ShowSelectCheckboxes = string.IsNullOrWhiteSpace(Request.QueryString["ShowSelectCheckboxes"]) || Request.QueryString["ShowSelectCheckboxes"] == "true";
        this.UseGridSession = string.IsNullOrWhiteSpace(Request.QueryString["UseGridSession"]) || Request.QueryString["UseGridSession"] == "true";
        this.ShowPageTitle = string.IsNullOrWhiteSpace(Request.QueryString["ShowPageTitle"]) || Request.QueryString["ShowPageTitle"] == "true";
        this.HideColumns = !string.IsNullOrWhiteSpace(Request.QueryString["HideColumns"]) ? Request.QueryString["HideColumns"].ToUpper() : "";

        if (!string.IsNullOrWhiteSpace((string)Session[SessionPageKey + "copied.rqmts"]))
        {
            string[] arr = ((string)Session[SessionPageKey + "copied.rqmts"]).Split(',');
            string[] rqmts = arr[0].Split('|');

            CopiedRQMTs = "";
            CopiedRQMTSystems = arr[1];

            for (int i = 0; i < rqmts.Length; i++)
            {
                string rqmt = rqmts[i];

                if (CopiedRQMTs.Length > 0) CopiedRQMTs += ", ";

                if (CopiedRQMTs.Contains(rqmt))
                {
                    CopiedRQMTs += rqmt + "*";
                }
                else
                {
                    CopiedRQMTs += rqmt;
                }
            }
        }
    }

    private string ParseViewXML()
    {
        string errors = "";

        XmlDocument nDoc = new XmlDocument();
        string gridViewName = string.Empty;
        string sessionXML = string.Empty;

        // get layout from session
        gridViewName = (string)Session[SessionPageKey + "RQMT_GridView"];
        sessionXML = (string)Session[SessionPageKey + "RQMT_CurrentXML"];

        if (!IsPostBack && !string.IsNullOrWhiteSpace(this.View)) 
        {
            GridView gv = new GridView();
            gv.ViewName = this.View;
            gv.Load();

            sessionXML = gv.SectionsXML.InnerXml;
            Session[SessionPageKey + "RQMT_GridView"] = gv.ViewName;
            Session[SessionPageKey + "RQMT_CurrentXML"] = sessionXML;
        }

        // if session doesn't have a layout, load the user's last view
        if (string.IsNullOrWhiteSpace(sessionXML))
        {
            if (gridViewName != null)
            {
                ListItem item = ddlGridview.Items.FindByText(gridViewName);

                if (item != null)
                {
                    sessionXML = item.Attributes["SectionsXML"];
                }
            }
            
            if (string.IsNullOrWhiteSpace(sessionXML))
            {
                gridViewName = WTSData.GetDefaultGridViewName((int)WTSGridName.RQMT_Grid, UserManagement.GetUserId_FromUsername());
                ListItem item = ddlGridview.Items.FindByText(gridViewName);
                sessionXML = item.Attributes["SectionsXML"];
            }
        }

        // if there is no default view, make one
        if (string.IsNullOrWhiteSpace(sessionXML) || sessionXML == "Default")
        {
            sessionXML = "<crosswalkparameters>";

            sessionXML += "<level>";
            sessionXML += "  <breakout><column>SYSTEM SUITE</column><sort>Ascending</sort></breakout>";
            sessionXML += "  <breakout><column>System</column><sort>Ascending</sort></breakout>";
            sessionXML += "  <breakout><column>Work Area</column><sort>Ascending</sort></breakout>";
            sessionXML += "  <breakout><column>Workload Group</column><sort>Ascending</sort></breakout>";
            sessionXML += "  <breakout><column>RQMT Type</column><sort>Ascending</sort></breakout>";
            sessionXML += "</level>";

            sessionXML += "<level>";
            //sessionXML += "  <breakout><column>Outline Index</column><sort>Ascending</sort></breakout>";
            sessionXML += "  <breakout><column>RQMT Primary #</column><sort>Ascending</sort></breakout>";
            sessionXML += "  <breakout><column>RQMT Primary</column><sort>Ascending</sort></breakout>";            
            sessionXML += "</level>";

            sessionXML += "<level>";
            //sessionXML += "  <breakout><column>Outline Index</column><sort>Ascending</sort></breakout>";
            sessionXML += "  <breakout><column>RQMT #</column><sort>Ascending</sort></breakout>";
            sessionXML += "  <breakout><column>RQMT</column><sort>Ascending</sort></breakout>";
            sessionXML += "</level>";

            sessionXML += "</crosswalkparameters>";
        }

        string[] levels = sessionXML.Split(new string[] { "</level>" }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].Contains("<column>RQMT</column>"))
            {
                RQMTLevel = i + 1;
            }

            if (levels[i].Contains("<column>RQMT #</column>"))
            {
                RQMTNumberLevel = i + 1;
            }

            if (levels[i].Contains("<column>RQMT Primary</column>"))
            {
                PrimaryRQMTLevel = i + 1;
            }

            if (levels[i].Contains("<column>RQMT Primary #</column>"))
            {
                PrimaryRQMTNumberLevel = i + 1;
            }

            if (levels[i].Contains("<column>RQMT Set Usage</column>"))
            {
                RQMTSetUsageLevel = i + 1;
            }

            if (levels[i].Contains("<column>RQMT Usage Month</column"))
            {
                RQMTUsageMonthLevel = i + 1;
            }

            if (levels[i].Contains("<column>System</column>"))
            {
                SystemLevel = i + 1;
            }

            if (levels[i].Contains("<column>Work Area</column>"))
            {
                WorkAreaLevel = i + 1;
            }

            if (levels[i].Contains("<column>RQMT Type</column>"))
            {
                RQMTTypeLevel = i + 1;
            }

            if (levels[i].Contains("<column>RQMT Set</column>"))
            {
                RQMTSetLevel = i + 1;
            }

            if (levels[i].Contains("<column>RQMT Set Complexity</column>"))
            {
                RQMTSetComplexityLevel = i + 1;
            }

            if (levels[i].Contains("<column>RQMT Set Complexity Justification</column>"))
            {
                RQMTSetComplexityJustificationLevel = i + 1;
            }

            if (levels[i].Contains("<column>Complexity</column>"))
            {
                RQMTComplexityLevel = i + 1;
            }

            if (levels[i].Contains("<column>Justification</column>"))
            {
                RQMTComplexityJustificationLevel = i + 1;
            }
        }

        if (RQMTSetUsageLevel > 0 && RQMTSetUsageLevel == RQMTSetLevel && RQMTSetUsageLevel == 1) // for now, we are ONLY allowing the month checkboxes at level 1 because the page doesn't support separate checkbox values for each level (TODO, IF ANYONE ASKS DOWN THE ROAD)
        {
            RQMTSetMonthCheckBoxLevel = RQMTSetUsageLevel;
        }

        if (sessionXML.Split(new string[] { "<column>RQMT</column>" }, StringSplitOptions.RemoveEmptyEntries).Length > 2)
        {
            errors += " RQMT column cannot be specified more than once.";
        }

        if (sessionXML.Split(new string[] { "<column>RQMT Primary</column>" }, StringSplitOptions.RemoveEmptyEntries).Length > 2)
        {
            errors += " RQMT Primary column cannot be specified more than once.";
        }

        if (sessionXML.Split(new string[] { "<column>RQMT #</column>" }, StringSplitOptions.RemoveEmptyEntries).Length > 2)
        {
            errors += " RQMT # column cannot be specified more than once.";
        }

        if (sessionXML.Split(new string[] { "<column>RQMT Primary #</column>" }, StringSplitOptions.RemoveEmptyEntries).Length > 2)
        {
            errors += " RQMT Primary # column cannot be specified more than once.";
        }

        if (RQMTNumberLevel != -1 && RQMTLevel != -1 && RQMTNumberLevel != RQMTLevel)
        {
            errors += " RQMT # must be on the same level as RQMT.";
        }

        if (PrimaryRQMTNumberLevel != -1 && PrimaryRQMTLevel != -1 && PrimaryRQMTNumberLevel != PrimaryRQMTLevel)
        {
            errors += " RQMT Primary # must be on the same level as RQMT Primary.";
        }

        if (RQMTLevel != -1 && PrimaryRQMTLevel != -1 && RQMTLevel != PrimaryRQMTLevel && RQMTLevel != (PrimaryRQMTLevel + 1))
        {
            errors += " RQMT Primary must be on the level prior to RQMT.";
        }

        if (RQMTSetUsageLevel != -1 && (RQMTSetUsageLevel != SystemLevel || RQMTSetUsageLevel != WorkAreaLevel || RQMTSetUsageLevel != RQMTTypeLevel))
        {
            errors += " RQMT Set Usage must be on the same level as System, Work Area, and Purpose.";
        }

        if (RQMTComplexityLevel != -1 && (RQMTComplexityLevel == RQMTSetComplexityLevel || RQMTComplexityLevel == RQMTSetComplexityJustificationLevel))
        {
            errors += " RQMT Complexity must be on a different row than RQMT Set Complexity or Justification.";
        }

        if (RQMTComplexityJustificationLevel != -1 && (RQMTComplexityJustificationLevel == RQMTSetComplexityLevel || RQMTComplexityJustificationLevel == RQMTSetComplexityJustificationLevel))
        {
            errors += " RQMT Complexity Justification must be on a different row than RQMT Set Complexity or Justification.";
        }
        
        string rqmtMode = "combined";
        string nextLevelRQMTMode = "combined";
        Dictionary<string, string> replacements = new Dictionary<string, string>();

        ParseRQMTModeAndReplacements(RQMTLevel, RQMTNumberLevel, PrimaryRQMTLevel, PrimaryRQMTNumberLevel, RQMTSetUsageLevel, CurrentLevel, ref rqmtMode, ref nextLevelRQMTMode, replacements);

        RQMTMode = rqmtMode;
        NextLevelRQMTMode = nextLevelRQMTMode;

        foreach (string key in replacements.Keys)
        {
            sessionXML = sessionXML.Replace(">" + key + "<", ">" + replacements[key] + "<");
        }

        nDoc.InnerXml = sessionXML;
        
        this.Levels = nDoc;
        this.LevelCount = this.Levels.SelectNodes("crosswalkparameters/level").Count;

        if (errors != "")
        {
            errors = "Invalid grid configuration. " + errors;
        }
        return errors;
    }

    private void ParseRQMTModeAndReplacements(int rqmtLevel, int rqmtNumberLevel, int primaryRQMTLevel, int primaryRQMTNumberLevel, int rqmtSetUsageLevel, int currentLevel, ref string rqmtMode, ref string nextLevelRQMTMode, Dictionary<string, string> replacements)
    {
        rqmtMode = "combined";
        nextLevelRQMTMode = "combined";

        /*  
            The options for RQMT's in the grid configuration are:
            1) RQMT (show any rqmt that is bottom level, so level 1's without children and all level 2's)
            2) RQMT PRIMARY (show any rqmt that is level 1)
            3) RQMT PRIMARY (show rqmts with children)
                  RQMT (show rqmts with parents in previous level)
            4) RQMT PRIMARY + RQMT (show all rqmts; for rqmt primary, show the rqmt itself if the rqmt doesn't have a parent)
                        
            In Options 1, 2, and 4, we are always referring to the RQMT itself. When we drilldown, count, or otherwise use the RQMT/RQMT PRIMARY fields, we are always referring to the
            rqmt.[COLUMNNAME] table. Option 3 is the ONLY option that actually uses rparent in where clauses in the RQMT query, and that only happens
            when we are at the child RQMT level (and beyond).

            To handle the special display rules, we rename RQMT and RQMT PRIMARY to distinguish their behavior for Options 3 and 4.
        */

        int nextLevel = currentLevel + 1;

        if ((rqmtLevel != -1 && primaryRQMTLevel != -1 && rqmtLevel == primaryRQMTLevel) ||
            (rqmtNumberLevel != -1 && primaryRQMTNumberLevel != -1 && rqmtNumberLevel == primaryRQMTNumberLevel) ||
            (rqmtLevel != -1 && primaryRQMTNumberLevel != -1 && rqmtLevel == primaryRQMTNumberLevel) ||
            (rqmtNumberLevel != -1 && primaryRQMTLevel != -1 && rqmtNumberLevel == primaryRQMTLevel)) // #4
        {
            // for this mode. we show all rqmts regardless of level
            rqmtMode = "combined";
            nextLevelRQMTMode = "combined";

            replacements.Add("RQMT Primary", "RQMTPRIMARYCOMBINEDNAME");
            replacements.Add("RQMT Primary #", "RQMTPRIMARYCOMBINEDNUMBER");
        }
        else if ((primaryRQMTLevel != -1 && rqmtLevel != -1 && primaryRQMTLevel < rqmtLevel) || (primaryRQMTNumberLevel != -1 && rqmtNumberLevel != -1 && primaryRQMTNumberLevel < rqmtNumberLevel)) // #3
        {
            // THESE ARE THE 4 PLACES THE ITEMS CAN APPEAR (EACH MAY REQUIRE DIFFERENT COLUMN TYPES):
            // 1
            // 2 RQMT PRIMARY
            // 4 RQMT
            // 5

            // NOTE THAT PRIMARY AND NON-PRIMARY WILL NEVER APPEAR ON THE SAME LINE TOGETHER IN OPTION #3
            // WHEN PRIMARY APPEARS IN THE FILTER LIST, IT WILL ALWAYS
            replacements.Add("RQMT Primary", "RQMTPRIMARYNESTED"); // still refers to RQMTID in select list and column counts until it hits child level, in which case it is converted to parent-only
            replacements.Add("RQMT Primary #", "RQMTPRIMARYNESTEDNUMBER");
            replacements.Add("RQMT", "RQMTNESTED");
            replacements.Add("RQMT #", "RQMTNESTEDNUMBER");

            if (currentLevel <= primaryRQMTLevel || currentLevel <= primaryRQMTNumberLevel)
            {
                // here we only care about rqmttops
                rqmtMode = "rqmttops";
            }
            else
            {
                rqmtMode = "rqmtbottomsexclusive";
            }

            if (nextLevel <= primaryRQMTLevel || nextLevel <= primaryRQMTNumberLevel)
            {
                // here we only care about rqmttops
                nextLevelRQMTMode = "rqmttops";
            }
            else
            {
                // from here on, we only care about requirements that are children of the primary
                nextLevelRQMTMode = "rqmtbottomsexclusive";
            }

        }
        else if ((primaryRQMTLevel != -1 || primaryRQMTNumberLevel != -1) && rqmtLevel == -1 && rqmtNumberLevel == -1) // #2 RQMT PRIMARY (top level rqmts, which includes anything at level 1)
        {
            rqmtMode = "rqmttops";
            nextLevelRQMTMode = "rqmttops";
        }
        else if ((rqmtLevel != -1 || rqmtNumberLevel != -1) && primaryRQMTLevel == -1 && primaryRQMTNumberLevel == -1) // 1 RQMT (bottom level rqmts, includes level 2 and level 1's without children since those are considered bottom as well)
        {
            rqmtMode = "rqmtbottoms";
            nextLevelRQMTMode = "rqmtbottoms";
        }

        if (rqmtSetUsageLevel == currentLevel)
        {
            // this is a hack to ensure set usage comes back with parent/child data counts (which would otherwise not include children if there is a parent -> child level structure)
            // to make sure this hack is called into play as little as possible, we enforce the rqmtsetusage columns be at the same level as system/wa/purpose
            rqmtMode = "combined";
        }
    }
    
    private void InitializeEvents()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;
    }

    private void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridPageIndex = e.NewPageIndex;
        Session[SessionPageKey + "rqmt.last.gridpageindex"] = this.GridPageIndex.ToString(); // if we ever load the page the rqmt maintenance container, we try to apply the last grid index so user doesn't bounce around when filters are re-applied

        grdData.PageIndex = e.NewPageIndex;
    }

    private void LoadLookupData()
    {
        int UserId = UserManagement.GetUserId_FromUsername();
        WTS_User u = new WTS_User(UserId);
        u.Load();
        DataTable dt = WTSData.GetViewOptions(userId: UserId, gridName: "RQMT Grid");

        if (dt != null && dt.Rows.Count > 0)
        {
            ddlGridview.Items.Clear();

            ListItem item = null;
            foreach (DataRow row in dt.Rows)
            {
                if (row["ViewName"].ToString() != "-- New Gridview --")
                {
                    var tempVal = row["WTS_RESOURCEID"].ToString();
                    item = new ListItem();
                    item.Text = row["ViewName"].ToString();
                    item.Value = row["GridViewID"].ToString();
                    item.Attributes.Add("OptionGroup", row["WTS_RESOURCEID"].ToString() != "" ? "Custom Views" : "Process Views");
                    item.Attributes.Add("MyView", row["MyView"].ToString());
                    item.Attributes.Add("Tier1RollupGroup", row["Tier1RollupGroup"].ToString());
                    item.Attributes.Add("Tier1ColumnOrder", row["Tier1ColumnOrder"].ToString());
                    item.Attributes.Add("Tier2ColumnOrder", row["Tier2ColumnOrder"].ToString());
                    item.Attributes.Add("DefaultSortType", row["Tier2SortOrder"].ToString());
                    item.Attributes.Add("SectionsXML", row["SectionsXML"].ToString());

                    ddlGridview.Items.Add(item);
                }
            }
        }

        if (ddlGridview.Items.FindByText("Default") == null)
        {            
            GridView gv = new GridView();
            gv.ViewName = "Default";
            gv.GridNameID = (int)WTSGridName.RQMT_Grid;
            gv.Load();

            ListItem item = new ListItem();
            item.Text = gv.ViewName;
            item.Value = gv.ID.ToString();
            item.Attributes.Add("OptionGroup", gv.UserID != 0 ? "Custom Views" : "Process Views");
            item.Attributes.Add("MyView", gv.UserID == UserId ? "1" : "0");
            item.Attributes.Add("Tier1RollupGroup", "");
            item.Attributes.Add("Tier1ColumnOrder", "");
            item.Attributes.Add("Tier2ColumnOrder", "");
            item.Attributes.Add("DefaultSortType", "");
            item.Attributes.Add("SectionsXML", gv.SectionsXML.InnerXml);

            ddlGridview.Items.Add(item);
        }

        if (CurrentLevel > 1) IsConfigured = true;

        if (Session[SessionPageKey + "RQMT_GridView_Default"] != null && !string.IsNullOrWhiteSpace(Session[SessionPageKey + "RQMT_GridView_Default"].ToString()))
        {
            ListItem itemGridView = ddlGridview.Items.FindByText(Session[SessionPageKey + "RQMT_GridView_Default"].ToString());
            if (itemGridView != null)
            {
                itemGridView.Selected = true;
                itisettings.Value = Session[SessionPageKey + "RQMT_GridView_Default"].ToString();
            }
            IsConfigured = true;
        }

        //Gather RQMTAttribute data for dropdown(s)
        // NOTE: WE DO NOT USE SESSIONPAGEKEY FOR THESE AS THEY ARE FAIRLY GENERIC MASTER DATA ITEMS
        if (Session["RQMTAttribute_Get"] == null)
        {
            dtRQMTAttribute = RQMT.RQMTAttribute_Get();
            Session["RQMTAttribute_Get"] = dtRQMTAttribute;
        }
        else
        {
            dtRQMTAttribute = (DataTable)Session["RQMTAttribute_Get"];
        }

        if (Session["RQMTComplexity_Get"] == null)
        {
            dtRQMTComplexity = RQMT.RQMTComplexityList_Get();
            Session["RQMTComplexity_Get"] = dtRQMTComplexity;
        }
        else
        {
            dtRQMTComplexity = (DataTable)Session["RQMTComplexity_Get"];
        }
    }

    private void GetRQMTSystemValuesFromFiltersOrRow(DataRow row, ref int sysid, ref int waid, ref int rtid, ref int rsetid)
    {
        sysid = 0;
        waid = 0;
        rtid = 0;
        rsetid = 0;

        string[] filters = this.Filter != null ? this.Filter.Split('|') : null;
        for (int i = 0; filters != null && i < filters.Length; i++)
        {
            string[] farr = filters[i].Split('=');

            if (farr[0] == "SYSTEM_ID")
            {
                sysid = Int32.Parse(farr[1]);
            }
            else if (farr[0] == "WORKAREA_ID")
            {
                waid = Int32.Parse(farr[1]);
            }
            else if (farr[0] == "RQMTTYPE_ID")
            {
                rtid = Int32.Parse(farr[1]);
            }
            else if (farr[0] == "RQMTSET_ID")
            {
                rsetid = Int32.Parse(farr[1]);
            }
        }

        if (row != null)
        {
            if (row.Table.Columns.Contains("SYSTEM_ID"))
            {
                sysid = row["SYSTEM_ID"] != DBNull.Value ? Int32.Parse(row["SYSTEM_ID"].ToString()) : 0;
            }

            if (row.Table.Columns.Contains("WORKAREA_ID"))
            {
                waid = row["WORKAREA_ID"] != DBNull.Value ? Int32.Parse(row["WORKAREA_ID"].ToString()) : 0;
            }

            if (row.Table.Columns.Contains("RQMTTYPE_ID"))
            {
                rtid = row["RQMTTYPE_ID"] != DBNull.Value ? Int32.Parse(row["RQMTTYPE_ID"].ToString()) : 0;
            }

            if (row.Table.Columns.Contains("RQMTSET_ID"))
            {
                rsetid = row["RQMTSET_ID"] != DBNull.Value ? Int32.Parse(row["RQMTSET_ID"].ToString()) : 0;
            }
        }
    }

    private void GetRQMTValuesFromRow(DataRow row, ref int rid, ref int rpid)
    {
        rid = 0;
        rpid = 0;

        if (row != null)
        {
            if (row.Table.Columns.Contains("RQMT #"))
            {
                rid = row["RQMT #"] != DBNull.Value ? Int32.Parse(row["RQMT #"].ToString()) : 0;
            }

            if (row.Table.Columns.Contains("RQMT PRIMARY #"))
            {
                rpid = row["RQMT PRIMARY #"] != DBNull.Value ? Int32.Parse(row["RQMT PRIMARY #"].ToString()) : 0;
            }
        }
    }

    #endregion

    #region Data
    private DataTable LoadData(int currentLevel, XmlDocument levels, string filters)
    {
        DataTable dt = new DataTable();

        if (IsPostBack && currentLevel == 1 && Session[SessionPageKey + "dtRQMT" + currentLevel] != null)
        {
            dt = (DataTable)Session[SessionPageKey + "dtRQMT" + currentLevel];
        }
        else
        {
            XmlDocument docLevel = new XmlDocument();
            XmlElement rootLevel = (XmlElement)docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
            XmlNode nodeLevel = levels.SelectNodes("crosswalkparameters/level")[currentLevel - 1];
            XmlNode nodeImport = docLevel.ImportNode(nodeLevel, true);
            rootLevel.AppendChild(nodeImport);

            XmlDocument docFilters = new XmlDocument();
            XmlElement rootFilters = (XmlElement)docFilters.AppendChild(docFilters.CreateElement("filters"));

            XmlDocument docWhereExts = new XmlDocument();
            XmlElement rootWhereExts = (XmlElement)docWhereExts.AppendChild(docWhereExts.CreateElement("whereexts"));

            if (filters != string.Empty)
            {
                string[] arrFilter = filters.Split('|');

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

            if (this.WhereExt != string.Empty)
            {
                string[] arrFilter = this.WhereExt.Split('|');

                for (int j = 0; j < arrFilter.Length; j++)
                {
                    XmlElement whereExt = docWhereExts.CreateElement("whereext");
                    XmlElement field = docWhereExts.CreateElement("field");
                    XmlElement value = docWhereExts.CreateElement("id");
                    string[] arrValues = arrFilter[j].Split('=');

                    field.InnerText = arrValues[0].ToString();
                    value.InnerText = (arrValues[1].ToString().Trim() == "" ? "0" : arrValues[1].ToString().Trim());

                    whereExt.AppendChild(field);
                    whereExt.AppendChild(value);
                    rootWhereExts.AppendChild(whereExt);
                }
            }

            dt = RQMT.RQMT_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, whereExts: docWhereExts, RQMTMode: RQMTMode, CountColumns:null, customWhere: null, ignoreUserFilters: IgnoreUserFilters);

            // when we have a rqmt and a description at the same level, we flatten the data table so we get one description row per rqmt
            // we do same for functionality and defects
            if (currentLevel == RQMTLevel || currentLevel == PrimaryRQMTLevel || currentLevel == RQMTNumberLevel || currentLevel == PrimaryRQMTNumberLevel)
            {
                if (dt.Columns.Contains("Description")) dt = dt.CombineRowsOnColumns(new string[] { "DESCRIPTION_ID", "Description" }, new string[] { "<descseparator>", "<descseparator>" }, true);
                if (dt.Columns.Contains("Defects")) dt = dt.CombineRowsOnColumns(new string[] { "RQMTSYSTEMDEFECT_ID", "Defects" }, new string[] { "<defseparator>", "<defseparator>" }, true);
                if (dt.Columns.Contains("Functionality")) dt = dt.CombineRowsOnColumns(new string[] { "FUNCTIONALITY_ID", "Functionality" }, new string[] { "<funcseparator>", "<funcseparator>" }, true);

            }

            if (!ShowSelectCheckboxes && dt.Columns.Contains("Y"))
            {
                dt.Columns.Remove("Y");
                dt.AcceptChanges();
            }

            if (currentLevel < LevelCount) // we need child counts
            {
                List<string> idColumns = new List<string>();
                StringBuilder childBreakout = new StringBuilder();
                InsertParentKeysIntoChildBreakout(dt, idColumns, childBreakout);

                XmlDocument countColumnsDoc = new XmlDocument();
                countColumnsDoc.InnerXml = "<countcolumns>" + childBreakout.ToString() + "</countcolumns>";

                if (idColumns.Count > 0) // we don't do child counts if the parent columns aren't groupable (like plain text)
                {
                    XmlDocument childDocLevel = new XmlDocument();
                    XmlElement childRootLevel = (XmlElement)childDocLevel.AppendChild(childDocLevel.CreateElement("crosswalkparameters"));
                    XmlNode childNodeLevel = levels.SelectNodes("crosswalkparameters/level")[currentLevel]; // since array is 0-based, this is selecting the NEXT level after the current level because currentLevel is 1-based

                    // remove columns that should not be in the count mode
                    if ((currentLevel + 1) == RQMTLevel || (currentLevel + 1) == PrimaryRQMTLevel || (currentLevel + 1) == RQMTNumberLevel || (currentLevel + 1) == PrimaryRQMTNumberLevel)
                    {
                        // this group will remove all columns that shouldn't be in the RQMT query for counting (currently, that includes the description/func/def columns because those could have duplicates)
                        // note that these columns are not combined when in non-RQMT rows, so they aren't excluded
                        childNodeLevel.InnerXml = childNodeLevel.InnerXml.Replace("<breakout><column>Description</column><sort>Ascending</sort></breakout>", "");
                        childNodeLevel.InnerXml = childNodeLevel.InnerXml.Replace("<breakout><column>Description</column><sort>Descending</sort></breakout>", "");
                        childNodeLevel.InnerXml = childNodeLevel.InnerXml.Replace("<breakout><column>RQMT Defects</column><sort>Ascending</sort></breakout>", "");
                        childNodeLevel.InnerXml = childNodeLevel.InnerXml.Replace("<breakout><column>RQMT Defects</column><sort>Descending</sort></breakout>", "");
                        childNodeLevel.InnerXml = childNodeLevel.InnerXml.Replace("<breakout><column>Functionality</column><sort>Ascending</sort></breakout>", "");
                        childNodeLevel.InnerXml = childNodeLevel.InnerXml.Replace("<breakout><column>Functionality</column><sort>Descending</sort></breakout>", "");
                    }
                    
                    childNodeLevel.InnerXml = childBreakout.ToString() + childNodeLevel.InnerXml;


                    XmlNode childNodeImport = childDocLevel.ImportNode(childNodeLevel, true);
                    childRootLevel.AppendChild(childNodeImport);

                    DataTable childdt = RQMT.RQMT_Crosswalk_Multi_Level_Grid(level: childDocLevel, filter: docFilters, whereExts: docWhereExts, RQMTMode: NextLevelRQMTMode, CountColumns: countColumnsDoc, customWhere: null, ignoreUserFilters: IgnoreUserFilters);

                    // NOTE: COUNT QUERIES ONLY RETURN TOTAL NUMBERS WITH FOREIGN KEYS, SO WE DON'T NEED TO CALL THE COMBINE FUNCTIONS

                    if (childdt.Columns.Contains("Column1"))
                    {
                        dt.Columns.Add("Column1");
                        if (dt.Rows.Count > 0)
                        {
                            dt.Rows[0]["Column1"] = childdt.Rows[0][0];
                        }
                    }
                    else
                    {
                        // now that we have the child counts, insert them into the main dt
                        dt.Columns.Add("ChildCount");

                        // because the parameters may be different than the actual columns (WTS_SYSTEM MIGHT RETURN SYSTEM_ID), we need to get the actual columns in the data sets to compare to 
                        // (which will be all columns other than ChildCount in the childdt table)
                        List<string> columnsToCompare = new List<string>();
                        foreach (DataColumn col in childdt.Columns)
                        {
                            if (col.ColumnName != "ChildCount")
                            {
                                columnsToCompare.Add(col.ColumnName);
                            }
                        }

                        // we build a dictionary of results so that we don't have to loop through the childdt every row we look for
                        Dictionary<string, int> countsByKey = new Dictionary<string, int>();
                        foreach (DataRow childRow in childdt.Rows)
                        {
                            StringBuilder key = new StringBuilder();

                            foreach (string col in columnsToCompare)
                            {
                                key.Append(childRow[col] + "_");
                            }

                            countsByKey.Add(key.ToString(), childRow["ChildCount"] != null && childRow["ChildCount"] != DBNull.Value ? (int)childRow["ChildCount"] : 0);
                        }

                        // finally, insert the counts into each row!
                        foreach (DataRow dr in dt.Rows)
                        {
                            StringBuilder key = new StringBuilder();

                            foreach (string col in columnsToCompare)
                            {
                                string colNameToFind = col.Replace("_COUNTCOLUMN", "");
                                colNameToFind = colNameToFind.Replace("_PARENT_ID", "_ID");

                                key.Append(dr[colNameToFind] + "_");
                            }

                            if (countsByKey.ContainsKey(key.ToString()))
                            {
                                dr["ChildCount"] = countsByKey[key.ToString()];
                            }
                            else
                            {
                                dr["ChildCount"] = 0;
                            }
                        }
                    }
                }
            }

            Session[SessionPageKey + "dtRQMT" + currentLevel] = dt;
        }

        return dt;
    }

    private void InsertParentKeysIntoChildBreakout(DataTable dt, List<string> idColumns, StringBuilder childBreakout)
    {
        // we get a full export of the next level, passing in the filters that applied to the current level
        // we also add the _ID columns for the current level into the child dt so we can count by them and match them up to the current level dt
        // there are a few exceptions to columns we don't pass  down to "counts":
        //   ROW_ID
        //   RQMTPRIMARYNESTEDNUMBER_ID, RQMTPRIMARYNESTEDNAME_ID (these columns are used for parent/child grids, and refer to the RQMTID - but for drilldowns, we need the PARENTRQMTID so we use those columns instead)
        //   ANY RQMT ATTRIBUTE COLUMNS THAT ARE ON A RQMT ROW (if we are showing a grid by criticality, we'll pass it down, but if we are just showing a rqmt row with criticality attribute, we ignore the attribute and pass the rqmt only)
        //   WE ALSO EXCLUDE DESCRIPTION/FUNCTIONALITY/DEFECT ROWS ON A RQMT ROW BECAUSE WE MERGE ROWS FOR THESE COLUMNS (e.g. one RQMT might have 2 descriptions)
        bool rqmtRow = false;
        for (int x = 0; x < dt.Columns.Count; x++)
        {
            string col = dt.Columns[x].ColumnName.ToLower();

            if (col == "rqmt_id" || col == "rqmtname_id" || col.IndexOf("rqmtprimary") != -1 || col.IndexOf("rqmtnested") != -1)
            {
                rqmtRow = true;
                break;
            }
        }

        string[] ignoreColumns = new string[] { "ROW_ID", "RQMTPRIMARYNESTEDNUMBER_ID", "RQMTPRIMARYNESTEDNAME_ID", "RQMTSETCOMPLEXITY_ID", "RQMTSETCOMPLEXITYPOINTS_HDN", "RQMTSETCOMPLEXITYJUSTIFICATION_ID" };
        if (rqmtRow)
        {
            string[] rqmtIgnoreColumns = new string[] { "RQMTCRITICALITY_ID", "RQMTSTAGE_ID", "RQMTSTATUS_ID", "RQMTACCEPTED_ID", "DESCRIPTION_ID", "RQMTSYSTEMDEFECT_ID", "FUNCTIONALITY_ID",
                "RQMTSYSTEMDEFECTNUMBER_ID", "RQMTSYSTEMDEFECTDESCRIPTION_ID", "RQMTSYSTEMDEFECTIMPACT_ID", "RQMTSYSTEMDEFECTMITIGATION_ID", "RQMTSYSTEMDEFECTNUMBER_ID", "RQMTSYSTEMDEFECTRESOLVED_ID", "RQMTSYSTEMDEFECTREVIEW_ID", "RQMTSYSTEMDEFECTSTAGE_ID", "RQMTSYSTEMDEFECTVERIFIED_ID" };

            ignoreColumns = ignoreColumns.Union(rqmtIgnoreColumns).ToArray();
        }
        
        for (int x = 0; x < dt.Columns.Count; x++)
        {
            string col = dt.Columns[x].ColumnName;
            string colRoot = col.Replace("_COUNTCOLUMN", "");

            if (colRoot.EndsWith("_ID") && !ignoreColumns.Contains(colRoot) && !idColumns.Contains(col) && !idColumns.Contains(colRoot))
            {
                idColumns.Add(col);
                // to separate count columns from normal columns, we add _COUNTCOLUMN to the end (this helps us prevent "column has been specificed twice in query" if the user has added the same column name on the child level)
                childBreakout.Append("<breakout><column>" + colRoot + "_COUNTCOLUMN</column><sort>Ascending</sort></breakout>");
            }
        }
    }

    private void PreProcessData(DataTable dt)
    {        
        if (DCC.Contains("ChildCount"))
        {
            HidePlusSignInHeader = true;

            foreach (DataRow row in dt.Rows)
            {
                int cnt = Convert.ToInt32(row["ChildCount"]);

                if (cnt > 0)
                {
                    HidePlusSignInHeader = false;
                    break;
                }
            }
        }
    }

    private void FormatExportColumns(DataTable dt)
    {
        for (var i = dt.Columns.Count - 1; i >= 0; i--)
        {
            DataColumn col = dt.Columns[i];

            string name = col.ColumnName;

            if (name.EndsWith("_ID") || name.EndsWith("_EXCLUDEID") || name.EndsWith("_HDN") || name == "X" || name == "Y" || name == "Z")
            {
                dt.Columns.Remove(col);
            }

            if (name == "Outline Index")
            {
                col.ColumnName = "IDX";
            }

            if (name == "Outline Index Parent")
            {
                col.ColumnName = "IDXL1";
            }

            if (name == "Outline Index Child")
            {
                col.ColumnName = "IDXL2";
            }

            if (name.EndsWith("_Number"))
            {
                col.ColumnName = col.ColumnName.Replace("_Number", " #");
            }

            if (name == "RQMT Type")
            {
                col.ColumnName = "Purpose";
            }
        }
    }
    #endregion

    #region Grid
    private void FormatHeaderRowDisplay(ref GridViewRow row, bool headerRow = true)
    {
        int sysid = 0;
        int waid = 0;
        int rtid = 0;
        int rsetid = 0;

        GetRQMTSystemValuesFromFiltersOrRow(null, ref sysid, ref waid, ref rtid, ref rsetid);

        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID") || DCC[i].ColumnName.EndsWith("_EXCLUDEID") || DCC[i].ColumnName.EndsWith("_ID_COMBINED"))
            {
                row.Cells[i].Style["display"] = "none";
            }

            if (!string.IsNullOrWhiteSpace(HideColumns))
            {
                string[] hideColumnArr = HideColumns.Split(',');

                for (int x = 0; x < hideColumnArr.Length; x++)
                {
                    string hideColumn = hideColumnArr[x];

                    if (!hideColumn.Contains("="))
                    {
                        hideColumn = this.View + "=" + hideColumn;
                    }

                    string[] arr = hideColumn.Split('=');

                    if (this.View.ToUpper() == arr[0].ToUpper() && arr[1].ToUpper() == DCC[i].ColumnName.ToUpper())
                    {
                        row.Cells[i].Style["display"] = "none";
                        break;
                    }
                }
            }
        }

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = "15px";
            row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
            row.Cells[DCC.IndexOf("X")].Style["white-space"] = "nowrap";

            if (this.CurrentLevel > 1) row.Cells[DCC.IndexOf("X")].Style["border-left"] = "1px solid grey";
        }

        if (DCC.Contains("Y"))
        {
            if (sysid > 0 && waid > 0 && rtid > 0 && rsetid > 0)
            {
                row.Cells[DCC.IndexOf("Y")].Text = "";
                row.Cells[DCC.IndexOf("Y")].Style["width"] = "15px";
                if (headerRow)
                {
                    row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBox("RQMTSELECT", "RQMTSELECT", "RQMTSELECT", "RQMTSELECT", "", 0, 0, 0, 0, rsetid, true, "toggleAllRQMTsForSet(this, " + rsetid + ")"));
                }
            }
            else
            {
                row.Cells[DCC.IndexOf("Y")].Style["display"] = "none";
            }
        }

        if (DCC.Contains("Z")) row.Cells[DCC.IndexOf("Z")].Text = "";

        if (DCC.Contains("ChildCount")) row.Cells[DCC.IndexOf("ChildCount")].Style["display"] = "none";

        Dictionary<string, string> columns = new Dictionary<string, string>();
        columns["System Suite"] = "250px,left";
        columns["System"] = "250px,left";
        columns["Work Area"] = "250px,left";
        columns["Functionality"] = "150px,left";
        columns["RQMT Set"] = "250px,left";
        columns["RQMT Type"] = "250px,left";
        columns["Complexity"] = "50px,center";
        columns["Justification"] = "200px,left";
        columns["Set Complexity"] = "50px,center";
        columns["Set Justification"] = "200px,left";

        columns["RQMT Primary"] = "250px,left";
        columns["RQMT Primary #"] = "50px,center";

        columns["RQMT"] = "250px,left";
        columns["RQMT #"] = "50px,center";

        columns["Results"] = "80%,left";        

        columns["RQMT Status"] = "100px,left";
        columns["Criticality"] = "100px,left";
        columns["RQMT Stage"] = "100px,left";
        columns["Accepted"] = "50px,center";
        columns["Defects"] = "50px,center";
        columns["Outline Index"] = "50px,center";
        columns["Outline Index Parent"] = "50px,center";
        columns["Outline Index Child"] = "50px,center";
        columns["Description_COMBINED"] = (sysid > 0 ? "150px" : "400px") + ",left"; // when systme is present, we pull up a rsrd row and displaly it as a popup table with DESCRIPTION(#) in the cell instead        
        columns["Defects_COMBINED"] = "150px,left";
        columns["Defect Description"] = "350px,left";
        columns["Defect Stage"] = "75px,left";
        columns["Defect Impact"] = "75px,left";
        columns["Defect Mitigation"] = "300px,left";
        columns["Defect Number"] = "75px,center";
        columns["Defect Verified"] = "75px,center";
        columns["Defect Resolved"] = "75px,center";
        columns["Defect Review"] = "75px,center";
        columns["Functionality_COMBINED"] = "150px,left";
        columns["RQMT Metrics"] = "75px,center";

        for (int m = 1; m <= 12; m++)
        {
            columns["Month_" + m] = "35px,center";
        }

        foreach (string colName in columns.Keys)
        {
            string[] dispArr = columns[colName].Split(',');

            if (DCC.Contains(colName))
            {
                TableCell cell = row.Cells[DCC.IndexOf(colName)];

                cell.Style["width"] = dispArr[0];
                cell.Style["text-align"] = dispArr[1];                
            }
        }

        for (int i = 0; i < DCC.Count; i++)
        {
            string col = DCC[i].ColumnName;

            if (col.ToLower().EndsWith("_hdn"))
            {
                row.Cells[i].Style["display"] = "none";
            }

            if (headerRow)
            {
                row.Cells[i].Style["white-space"] = "nowrap";
            }
        }
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        FormatHeaderRowDisplay(ref row, false);
    }

    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);

        // HidePlusSignInHeader is TRUE if none of the elements have a child drilldown (we only check this for parent/child RQMTs)
        // We show the plus sign on individual rows EXCEPT except for breakouts showing a child rqmt or an uncategorized rqmt

        if (!HidePlusSignInHeader && CurrentLevel < LevelCount) {
            if (DCC.Contains("X")) row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage(true));
        }

        string[] columns = ("Outline Index=IDX,Outline Index Parent=IDXL1,Outline Index Child=IDXL2,RQMT PRIMARY=RQMT Primary,RQMT PRIMARY #=RQMT Primary #,RQMT Stage=PD2TDR,RQMT Type=Purpose,Description_COMBINED=Description," +
            "Defects_COMBINED=Defects,Defect Stage=Defect PD2TDR,Defect Number=Defect #,Defect Verified=Verified,Defect Resolved=Resolved,Defect Review=Review,Defect Mitigation=Mitigation," + 
            "Functionality_COMBINED=Functionality,RQMT Set=RQMT Set Name," + 
            "Month_1=JAN,Month_2=FEB,Month_3=MAR,Month_4=APR,Month_5=MAY,Month_6=JUN,Month_7=JUL,Month_8=AUG,Month_9=SEP,Month_10=OCT,Month_11=NOV,Month_12=DEC").Split(',');
        for (int i = 0; i < columns.Length; i++)
        {
            string[] col = columns[i].Split('=');

            if (DCC.Contains(col[0].Trim())) {
                row.Cells[DCC.IndexOf(col[0].Trim())].Text = col[1].Trim();

                if (col[0].StartsWith("Month_") && DCC.Contains("RQMTSetUsage_HDN") && RQMTSetMonthCheckBoxLevel == 1)
                {
                    row.Cells[DCC.IndexOf(col[0].Trim())].Text = row.Cells[DCC.IndexOf(col[0].Trim())].Text + "<input type=\"checkbox\" rqmtsetusagemonth=\"" + col[0].Split('_')[1] + "\" onchange=\"toggleRQMTSetUsageMonth(" + col[0].Split('_')[1] + ", $(this).is(':checked'))\">";
                }

                /*
                if (row.Cells[DCC.IndexOf(col[0].Trim())].Text.Length == 3)
                {
                    
                }*/
            }
        }        
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        DataRow dr = ((System.Data.DataRowView)row.DataItem).Row;

        int sysid = 0;
        int waid = 0;
        int rtid = 0;
        int rsetid = 0;

        int rid = 0;
        int rpid = 0;
        // if we have rqmt by itself, or primary rqmt by itself, we use the one that is specified
        // if we have both on the same line, we are going to use the child rqmt
        int rqmtid = 0;

        GetRQMTValuesFromRow(dr, ref rid, ref rpid);
        GetRQMTSystemValuesFromFiltersOrRow(dr, ref sysid, ref waid, ref rtid, ref rsetid);

        if (rid > 0)
        {
            rqmtid = rid;
        }
        else if (rpid > 0)
        {
            rqmtid = rpid;
        }
        
        FormatRowDisplay(ref row);

        // + image
        if (DCC.Contains("X"))
        {
            // show + img for most rows EXCEPT for rows where we are showing RQMTs and the RQMTs are not part of a set yet (there are no child items of these RQMTs)
            if (CurrentLevel < LevelCount)
            {
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateLiteral("&nbsp;"));
                
                if (DCC.Contains("ChildCount"))
                {
                    int cnt = 0;
                    Int32.TryParse(row.Cells[DCC.IndexOf("ChildCount")].Text, out cnt);

                    if (cnt > 0)
                    {
                        row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage(false));
                        row.Cells[DCC.IndexOf("X")].Controls.Add(CreateLiteral("&nbsp;(" + cnt.ToString() + ")"));
                    }
                }

                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateLiteral("&nbsp;"));
            }
        }

        if (DCC.Contains("RQMT #"))
        {
            string number = row.Cells[DCC.IndexOf("RQMT #")].Text;

            row.Cells[DCC.IndexOf("RQMT #")].Controls.Add(createHTMLLink("RQMT #", "rqmtnumber", number, number));
        }

        if (DCC.Contains("RQMT Primary #"))
        {
            string number = row.Cells[DCC.IndexOf("RQMT Primary #")].Text;

            row.Cells[DCC.IndexOf("RQMT Primary #")].Controls.Add(createHTMLLink("RQMT Primary #", "rqmtprimarynumber", number, number));
        }
        
        if (DCC.Contains("Accepted"))
        {
            row.Cells[DCC.IndexOf("Accepted")].Text = row.Cells[DCC.IndexOf("Accepted")].Text == "1" ? "Yes" : "No";
        }

        // also note that the block below shows controls if sysid is > 0, which is good enough for RQMTSystem attributes, but NOT good enough for RQMTSet_RQMTSystem attributes (we need sys,wa,rt for that, which we handle in the following block)
        if ((rid > 0 || rpid > 0) && sysid > 0)
        {
            //Gather the ID's needed to save to RQMTSystem table
            string rplusysid = rqmtid + "_" + sysid;

            //Create dropdown for RQMT Status
            if (DCC.Contains("RQMTSTATUS_ID"))
            {
                row.Cells[DCC.IndexOf("RQMT Status")].Controls.Add(CreateDropDownList(dtRQMTAttribute, "RQMT Status", "RQMTAttribute", "RQMTAttributeID", row.Cells[DCC.IndexOf("RQMTSTATUS_ID")].Text, 80, "RQMT", false, "", rqmtid, sysid, waid, rtid, rsetid));
                CanEditRQMSystemOrSet = true;
            }

            //Create dropdown for RQMT Status
            if (DCC.Contains("RQMTCRITICALITY_ID"))
            {
                row.Cells[DCC.IndexOf("Criticality")].Controls.Add(CreateDropDownList(dtRQMTAttribute, "Criticality", "RQMTAttribute", "RQMTAttributeID", row.Cells[DCC.IndexOf("RQMTCRITICALITY_ID")].Text, 80, "RQMT", false, "", rqmtid, sysid, waid, rtid, rsetid));
                CanEditRQMSystemOrSet = true;
            }

            if (DCC.Contains("RQMTSTAGE_ID"))
            {
                row.Cells[DCC.IndexOf("RQMT Stage")].Controls.Add(CreateDropDownList(dtRQMTAttribute, "RQMT Stage", "RQMTAttribute", "RQMTAttributeID", row.Cells[DCC.IndexOf("RQMTSTAGE_ID")].Text, 80, "RQMT", false, "", rqmtid, sysid, waid, rtid, rsetid));
                CanEditRQMSystemOrSet = true;
            }

            //Create Checkbox for Accepted
            if (DCC.Contains("Accepted"))
            {
                row.Cells[DCC.IndexOf("Accepted")].Controls.Add(CreateCheckBox("RQMT", "RQMTAccepted_ID", "RQMT Accepted", "RQMTAccepted_ID", row.Cells[DCC.IndexOf("Accepted")].Text, rqmtid, sysid, waid, rtid, rsetid, false, null));
                CanEditRQMSystemOrSet = true;
            }

            //Create Link for Defects/Impacts
            if (DCC.Contains("Defects_COMBINED"))
            {
                string text = row.Cells[DCC.IndexOf("Defects_COMBINED")].Text;
                if (text == "&nbsp;") text = "";

                string[] defs = text.Split(new string[] { "&lt;defseparator&gt;" }, StringSplitOptions.RemoveEmptyEntries);
                if (defs != null && defs.Length > 0)
                {
                    StringBuilder defstr = new StringBuilder();                    
                    defstr.Append("<div class=\"tooltip\" style=\"cursor: pointer;\" onmouseover=\"showTopLevelToolTip($(this).find(\'div\').html(), this, 'rqmtpopup_" + rplusysid + "', false, \'plain\');\"  onmouseleave=\"hideTopLevelToolTip('rqmtpopup_" + rplusysid + "');\" onclick=\"lblEdit_click('Defects', '" + rplusysid + "');\">");
                    defstr.Append("DEFECT" + (defs.Length > 1 ? "S" : "") + " (" + defs.Length + ")");

                    defstr.Append("<div style=\"display:none;\">");
                    defstr.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:525px\">");
                    defstr.Append("<tr>");
                    defstr.Append("  <td style=\"width:25px;text-align:center;vertical-align:top;font-weight:bold;color:black;text-decoration:underline;\">#</td>");
                    defstr.Append("  <td style=\"width:300px;text-align:left;vertical-align:top;font-weight:bold;color:black;text-decoration:underline;\">Defect</td>");
                    defstr.Append("  <td style=\"width:100px;text-align:left;vertical-align:top;font-weight:bold;color:black;text-decoration:underline;\">Impact</td>");
                    defstr.Append("  <td style=\"width:100px;text-align:left;vertical-align:top;font-weight:bold;color:black;text-decoration:underline;\">PD2TDR</td>");
                    defstr.Append("</tr>");

                    for (int d = 0; d < defs.Length; d++)
                    {
                        string defect = defs[d];
                        int idx = defect.LastIndexOf(" (");
                        string defectAttr = defect.Substring(idx).Replace(" (", "").Replace(")", "");
                        defect = defect.Substring(0, idx);
                        string impact = defectAttr.Split('/')[0];
                        string stage = defectAttr.Split('/')[1];

                        string colorStyle = "";

                        if (impact == "Work Stoppage")
                        {
                            colorStyle = "color:red;";
                        }

                        defstr.Append("<tr>");
                        defstr.Append("  <td style=\"width:25px;text-align:center;vertical-align:top;" + colorStyle + "\">" + (d + 1) + "</td>");
                        defstr.Append("  <td style=\"width:300px;text-align:left;vertical-align:top;" + colorStyle + "\">" + StringUtil.StripHTML(defect) + "</td>");
                        defstr.Append("  <td style=\"width:100px;text-align:left;vertical-align:top;white-space:nowrap;" + colorStyle + "\">" + impact + "</td>");
                        defstr.Append("  <td style=\"width:100px;text-align:left;vertical-align:top;white-space:nowrap;" + colorStyle + "\">" + stage + "</td>");
                        defstr.Append("</tr>");
                    }

                    defstr.Append("</table>");

                    defstr.Append("</div>");
                    defstr.Append("</div>");

                    row.Cells[DCC.IndexOf("Defects_COMBINED")].Text = defstr.ToString();
                }
                else
                {
                    row.Cells[DCC.IndexOf("Defects_COMBINED")].Text = "<div style=\"width:100%;cursor:pointer;text-align:center;\" onclick=\"lblEdit_click('Defects', '" + rplusysid + "')\">-</div>";
                }
            }

            if (DCC.Contains("Description_COMBINED"))
            {
                // the desc columns can now contain rich text html, and the gridrow escapes it, so we get it from the raw data row instead
                string text = dr["Description_COMBINED"] != DBNull.Value ? (string)dr["Description_COMBINED"] : "&nbsp;";// row.Cells[DCC.IndexOf("Description_COMBINED")].Text;
                if (text == "&nbsp;") text = "";

                string[] descs = text.Split(new string[] { "<descseparator>" }, StringSplitOptions.RemoveEmptyEntries);
                if (descs != null && descs.Length > 0)
                {
                    StringBuilder descstr = new StringBuilder();
                    descstr.Append("<div class=\"tooltip\" style=\"cursor: pointer;\" onmouseover=\"showTopLevelToolTip($(this).find(\'div\').html(), this, 'rqmtpopup_" + rplusysid + "', false, \'plain\');\" onmouseleave=\"hideTopLevelToolTip('rqmtpopup_" + rplusysid + "');\">");
                    descstr.Append("<span onclick=\"lblEdit_click('Description', " + rqmtid + ")\">DESCRIPTION" + (descs.Length > 1 ? "S" : "") + " (" + descs.Length + ")</span>");

                    descstr.Append("<div style=\"display:none;\">");
                    descstr.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:425px\" onmouseleave=\"hideTopLevelToolTip('rqmtpopup_" + rplusysid + "');\">");
                    descstr.Append("<tr>");
                    descstr.Append("  <td style=\"width:25px;text-align:center;vertical-align:top;font-weight:bold;color:black;text-decoration:underline;\">#</td>");
                    descstr.Append("  <td style=\"width:300px;text-align:left;vertical-align:top;font-weight:bold;color:black;text-decoration:underline;\">Description</td>");
                    descstr.Append("  <td style=\"width:100px;text-align:left;vertical-align:top;font-weight:bold;color:black;text-decoration:underline;\">Type</td>");
                    descstr.Append("</tr>");

                    for (int d = 0; d < descs.Length; d++)
                    {
                        string desc = descs[d];
                        int idx = desc.LastIndexOf(" (");
                        string type = desc.Substring(idx).Replace(" (", "").Replace(")", "");
                        desc = desc.Substring(0, idx);

                        string colorStyle = "";

                        descstr.Append("<tr>");
                        descstr.Append("  <td style=\"width:25px;text-align:center;vertical-align:top;" + colorStyle + "\">" + (d + 1) + "</td>");
                        descstr.Append("  <td style=\"width:300px;text-align:left;vertical-align:top;" + colorStyle + "\">" + desc + "</td>");
                        descstr.Append("  <td style=\"width:100px;text-align:left;vertical-align:top;" + colorStyle + "\">" + type + "</td>");
                        descstr.Append("</tr>");
                    }

                    descstr.Append("</table>");

                    descstr.Append("</div>");
                    descstr.Append("</div>");

                    row.Cells[DCC.IndexOf("Description_COMBINED")].Controls.Add(CreateLiteral(descstr.ToString()));
                }
                else
                {
                    row.Cells[DCC.IndexOf("Description_COMBINED")].Text = "<div style=\"width:100%;cursor:pointer;text-align:center;\" onclick=\"lblEdit_click('Description', " + rqmtid + ")\">-</div>";
                }
            }

            if (DCC.Contains("Functionality_COMBINED"))
            {
                string text = row.Cells[DCC.IndexOf("Functionality_COMBINED")].Text;

                if (text == "&nbsp;") text = "";

                string[] funcs = text.Split(new string[] { "&lt;funcseparator&gt;" }, StringSplitOptions.RemoveEmptyEntries);
                if (funcs != null && funcs.Length > 0)
                {
                    StringBuilder funcstr = new StringBuilder();
                    funcstr.Append("<div class=\"tooltip\" style=\"cursor: pointer;\" onmouseover=\"showTopLevelToolTip($(this).find(\'div\').html(), this, 'rqmtpopup_" + rplusysid + "', false, \'plain\');\"  onmouseleave=\"hideTopLevelToolTip('rqmtpopup_" + rplusysid + "');\" onclick=\"lblEdit_click('Functionality', '" + rqmtid + "');\">");
                    funcstr.Append((funcs.Length > 1 ? "FUNCTIONALITIES" : "FUNCTIONALITY") + " (" + funcs.Length + ")");

                    funcstr.Append("<div style=\"display:none;\">");
                    funcstr.Append(text.Replace("&lt;funcseparator&gt;", "<br />"));
                    funcstr.Append("</div>");


                    funcstr.Append("</div>");

                    row.Cells[DCC.IndexOf("Functionality_COMBINED")].Text = funcstr.ToString();
                }
                else
                {
                    row.Cells[DCC.IndexOf("Functionality_COMBINED")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Functionality_COMBINED")].Controls.Add(createHTMLLink("Functionality", "functionality", rqmtid.ToString(), "&nbsp;"));
                }
            }
        }

        // this block is suitable for attributes that need a full rqmtset_rqmtsystemid
        // NOTE: sys/wa/rt is NOT enough to identify a unique sets because users can create two sets with the same properties but differing only by name
        if ((rid > 0 || rpid > 0) && sysid > 0 && waid > 0 && rtid > 0 && rsetid > 0)
        {
            if (DCC.Contains("Month_1"))
            {
                CanEditRQMSystemOrSet = true;
                for (int m = 1; m <= 12; m++)
                {
                    if (DCC.Contains("Month_" + m))
                    {
                        bool monthChecked = dr["Month_" + m] != DBNull.Value && dr["Month_" + m].ToString().ToUpper() == "TRUE";

                        row.Cells[DCC.IndexOf("Month_" + m)].Controls.Clear();
                        row.Cells[DCC.IndexOf("Month_" + m)].Controls.Add(CreateCheckBox("RQMT", "Month_" + m, "Month_" + m, "Month_" + m, monthChecked.ToString().ToUpper(), rqmtid, sysid, waid, rtid, rsetid, false, null));
                    }
                }
            }

            if (DCC.Contains("Y"))
            {
                row.Cells[DCC.IndexOf("Y")].Controls.Add(CreateCheckBox("RQMTSELECT", "RQMTSELECT", "RQMTSELECT", "RQMTSELECT", "", rqmtid, sysid, waid, rtid, rsetid, true, null));
            }
        }

        // the items below could only happen on non-rqmt rows, or possibly rqmt set rows (we generally don't allow editing at these levels)
        if ((rid == 0 && rpid == 0))
        {
            if (sysid > 0 && waid > 0 && rtid > 0 && rsetid > 0)
            {
                if (DCC.Contains("Month_1"))
                {
                    for (int m = 1; m <= 12; m++)
                    {
                        if (DCC.Contains("Month_" + m))
                        {
                            bool monthChecked = dr["Month_" + m] != DBNull.Value && (dr["Month_" + m].ToString().ToUpper() == "TRUE" || dr["Month_" + m].ToString().ToUpper() == "YES" || dr["Month_" + m].ToString().ToUpper() == "1");

                            row.Cells[DCC.IndexOf("Month_" + m)].Controls.Clear();
                            row.Cells[DCC.IndexOf("Month_" + m)].Controls.Add(CreateCheckBox("Other", "Month_" + m, "Month_" + m, "Month_" + m, monthChecked.ToString().ToUpper(), 0, sysid, waid, rtid, rsetid, false, null));
                        }
                    }
                }

                if (DCC.Contains("RQMTSETCOMPLEXITY_ID"))
                {
                    CanEditRQMSystemOrSet = true;
                    row.Cells[DCC.IndexOf("Set Complexity")].Controls.Clear();
                    row.Cells[DCC.IndexOf("Set Complexity")].Controls.Add(CreateDropDownList(dtRQMTComplexity, "RQMTSetComplexity", "RQMTComplexity", "RQMTComplexityID", row.Cells[DCC.IndexOf("RQMTSETCOMPLEXITY_ID")].Text, 80, "RQMT", false, "", rqmtid, sysid, waid, rtid, rsetid));
                }

                if (DCC.Contains("RQMTSETCOMPLEXITYJUSTIFICATION_ID"))
                {
                    CanEditRQMSystemOrSet = true;
                    string text = row.Cells[DCC.IndexOf("Set Justification")].Text;
                    row.Cells[DCC.IndexOf("Set Justification")].Controls.Clear();
                    row.Cells[DCC.IndexOf("Set Justification")].Controls.Add(CreateTextBox("RQMT", "RQMTSetJustification", "RQMTSetJustification", text, false, rqmtid, sysid, waid, rtid, rsetid));
                }
            }
        }
        
        for (int i = 0; i < row.Cells.Count; i++)
        {
            string txt = row.Cells[i].Text;

            if (DCC[i].ColumnName.EndsWith("_ID"))
            {
                if (string.IsNullOrWhiteSpace(txt) || txt == "&nbsp;")
                {
                    txt = "0";
                }

                row.Attributes[DCC[i].ColumnName] = txt;
            }
            else if (DCC[i].ColumnName == "Description")
            {
                row.Cells[i].Text = dr["Description"] != DBNull.Value ? (string)dr["Description"] : "&nbsp;"; // preserve HTML formatting for this one column type
            }
            else if (DCC[i].ColumnName == "Description_COMBINED")
            {
                row.Cells[i].Text = dr["Description_COMBINED"] != DBNull.Value ? (string)dr["Description_COMBINED"] : "&nbsp;"; // preserve HTML formatting for this one column type
            }
        }

        row.Attributes["systemid"] = sysid.ToString();
        row.Attributes["workareaid"] = waid.ToString();
        row.Attributes["rqmttypeid"] = rtid.ToString();
        row.Attributes["rqmtid"] = rqmtid.ToString();
        row.Attributes["rsetid"] = rsetid.ToString();
    }

    private HtmlControl CreateCheckBox(string typeName, string valueColumn, string field, string attrName, string attrValue, int rqmtid, int sysid, int waid, int rtid, int rsetid, bool ignoreInputChange, string customOnChange)
    {
        HtmlInputCheckBox chk = new HtmlInputCheckBox();

        chk.Attributes["typeName"] = typeName;
        chk.Attributes["typeID"] = valueColumn;
        chk.Attributes["field"] = field;
        
        chk.Attributes["rqmtid"] = rqmtid.ToString();
        chk.Attributes["systemid"] = sysid.ToString();
        chk.Attributes["workareaid"] = waid.ToString();
        chk.Attributes["rqmttypeid"] = rtid.ToString();
        chk.Attributes["rsetid"] = rsetid.ToString();

        if (typeName == "Other")
        {
            chk.Attributes["onchange"] = "";
            chk.Disabled = true;
        }
        else
        {
            if (!ignoreInputChange)
            {
                chk.Attributes["onchange"] = "input_change(this);";
            }
        }

        if (customOnChange != null)
        {
            chk.Attributes["onchange"] = customOnChange;
        }

        var isChecked = false;
        if (attrValue != null) {
            attrValue = attrValue.ToUpper();
            if (attrValue == "YES" || attrValue == "TRUE" || attrValue == "1")
            {
                isChecked = true;
            }
        }
        chk.Checked = isChecked;

        if (!this.CanEditRQMT)
        {
            var text = new System.Web.UI.HtmlControls.HtmlGenericControl();
            text.InnerHtml = chk.Checked ? "Yes" : "";

            return text;
        }
        else
        {
            return chk;
        }
    }

    private Image CreateImage(bool isHeader)
    {
        Image img = new Image();

        if (isHeader)
        {
            img.Attributes["src"] = "Images/Icons/add_blue.png";
            img.Attributes["title"] = "Expand";
            img.Attributes["alt"] = "Expand";
            img.Attributes["onclick"] = "displayAllRows(this);";
        }
        else {
            img.Attributes["src"] = "Images/Icons/add_blue.png";
            img.Attributes["title"] = "Expand";
            img.Attributes["alt"] = "Expand";
            img.Attributes["onclick"] = "displayNextRow(this);";
        }

        img.Attributes["height"] = "12";
        img.Attributes["width"] = "12";
        img.Style["cursor"] = "pointer";

        return img;
    }

    private System.Web.UI.Control CreateTextBox(string typeName, string typeID, string field, string value, bool isNumber, int rqmtid, int sysid, int waid, int rtid, int rsetid)
    {
        string txtValue = Server.HtmlDecode(Uri.UnescapeDataString(value)).Trim();
        TextBox txt = new TextBox();

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
        txt.Attributes["rqmtid"] = rqmtid.ToString();
        txt.Attributes["systemid"] = sysid.ToString();
        txt.Attributes["workareaid"] = waid.ToString();
        txt.Attributes["rqmttypeid"] = rtid.ToString();
        txt.Attributes["rsetid"] = rsetid.ToString();

        if (isNumber)
        {
            txt.MaxLength = 5;
            txt.Style["text-align"] = "center";
        }

        if (!CanEditRQMT)
        {
            return CreateLiteral(txtValue);
        }
        else
        {
            return txt;
        }
    }

    private Literal CreateLiteral(string str)
    {
        Literal literal = new Literal();
        literal.Text = str;

        return literal;        
    }

    private System.Web.UI.Control CreateDropDownList(DataTable dt, string field, string textColumn, string valueColumn, string selectedValue, int maxWidth, 
        string typeName, bool allowBlankValue, string onChange, int rqmtid, int sysid, int waid, int rtid, int rsetid)
    {
        DropDownList ddl = new DropDownList();
        ddl.Attributes.Add("field", field);
        ddl.Attributes.Add("typeID", valueColumn);
        ddl.Attributes.Add("typeName", typeName);

        ddl.Attributes["rqmtid"] = rqmtid.ToString();
        ddl.Attributes["systemid"] = sysid.ToString();
        ddl.Attributes["workareaid"] = waid.ToString();
        ddl.Attributes["rqmttypeid"] = rtid.ToString();
        ddl.Attributes["rsetid"] = rsetid.ToString();

        ListItem liBlank = new ListItem();
        liBlank.Text = "";
        liBlank.Value = "-1";
        ddl.Items.Add(liBlank);
        
        if (maxWidth > 0)
        {
            ddl.Style["width"] = maxWidth + "px";
        }

        if (!string.IsNullOrWhiteSpace(onChange))
        {
            ddl.Attributes["onchange"] = onChange;
        }

        if (textColumn == "RQMTAttribute")
        {
            dt.DefaultView.RowFilter = "RQMTAttributeType = '" + field + "'";
            dt = dt.DefaultView.ToTable();
        }

        foreach (DataRow row in dt.Rows)
        {
            string rowText = row[textColumn] != DBNull.Value ? Convert.ToString(row[textColumn]) : "";
            string rowValue = row[valueColumn] != DBNull.Value ? Convert.ToString(row[valueColumn]) : "";

            ListItem li = new ListItem();

            if (string.IsNullOrWhiteSpace(rowValue) || rowValue == "0" || rowValue == "-1")
            {
                if (allowBlankValue)
                {
                    li.Text = rowText;
                    li.Value = rowValue;

                    if (string.IsNullOrWhiteSpace(selectedValue) || selectedValue == "0" || selectedValue == "-1")
                    {
                        li.Selected = true;
                    }

                    ddl.Items.Add(li);
                }
            }
            else
            {
                li.Text = rowText;
                li.Value = rowValue;

                if (rowValue == selectedValue)
                {
                    li.Selected = true;
                }

                ddl.Items.Add(li);
            }            
        }

        //If column has no value, set dropdown selected value to the liBlank
        if (selectedValue == "&nbsp;" || string.IsNullOrWhiteSpace(selectedValue))
        {
            ddl.SelectedValue = "-1";
        }

        //Add onChange event for saving
        ddl.Attributes["onchange"] = "input_change(this);";

        if (!CanEditRQMT)
        {
            return CreateLiteral(ddl.SelectedValue != "-1" && ddl.SelectedValue != "0" ? ddl.SelectedItem.Text : "");
        }
        else
        {
            return ddl;
        }
    }

    private LinkButton createHTMLLink(string columnName, string field, string id, string text)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("lblEdit_click('{0}', '{1}');return false;", columnName, id);

        LinkButton lb = new LinkButton();
        lb.ID = string.Format("lblEdit_{0}_{1}", field, id);
        lb.Attributes["name"] = string.Format("lblEdit_{0}", id);
        lb.Text = text;
        lb.Attributes.Add("Onclick", sb.ToString());

        return lb;
    }
    #endregion

    #region Excel
    private void ExportToExcel(DataTable defaultDt = null)
    {
        ExcelUtil eu = new ExcelUtil();

        if (defaultDt != null)
        {           
            eu.WriteDataTableToResponse(defaultDt, Response, true);
            return;
        }

        List<DataTable> levelDts = new List<DataTable>();

        for (int level = 1; level <= this.LevelCount + 1; level++)
        {
            XmlDocument docLevel = new XmlDocument();
            XmlElement rootLevel = (XmlElement)docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));

            if (level == this.LevelCount + 1) // our last query is a FLAT query of all RQMT data
            {
                string xml = "";

                xml = "<crosswalkparameters>";

                xml += "<level>";                
                xml += "  <breakout><column>SYSTEM SUITE</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>System</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>Work Area</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMT Type</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMT Set</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>Outline Index</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>Outline Index Parent</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>Outline Index Child</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMTPRIMARYCOMBINEDNUMBER</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMTPRIMARYCOMBINEDNAME</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMT #</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMT</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMT Accepted</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMT Criticality</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMT Stage</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMT Status</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMT Usage</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>Description</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>Functionality</column><sort>Ascending</sort></breakout>";
                xml += "  <breakout><column>RQMT Defects</column><sort>Ascending</sort></breakout>";

                xml += "</level>";

                xml += "</crosswalkparameters>";

                docLevel.InnerXml = xml;

                PrimaryRQMTLevel = 1;
                PrimaryRQMTNumberLevel = 1;
                RQMTLevel = 1;
                RQMTNumberLevel = 1;
            }
            else
            {
                XmlNode nodeLevel = this.Levels.SelectNodes("crosswalkparameters/level")[level - 1];

                if (level > 1)
                {
                    // if we are at a child level, we need to pass filter id columns into the child query so we can match up the children with the parents
                    DataTable parentDt = levelDts.Last();

                    List<string> idColumns = new List<string>();
                    StringBuilder childBreakout = new StringBuilder();

                    InsertParentKeysIntoChildBreakout(parentDt, idColumns, childBreakout);

                    if (idColumns.Count > 0)
                    {
                        nodeLevel.InnerXml = childBreakout.ToString() + nodeLevel.InnerXml;
                    }
                }

                XmlNode nodeImport = docLevel.ImportNode(nodeLevel, true);
                rootLevel.AppendChild(nodeImport);
            }

            XmlDocument docFilters = new XmlDocument();
            XmlElement rootFilters = (XmlElement)docFilters.AppendChild(docFilters.CreateElement("filters"));

            XmlDocument docWhereExts = new XmlDocument();
            XmlElement rootWhereExts = (XmlElement)docWhereExts.AppendChild(docWhereExts.CreateElement("whereexts"));

            string rqmtMode = "combined";
            string nextLevelRQMTMode = "combined";
            Dictionary<string, string> replacements = new Dictionary<string, string>();

            ParseRQMTModeAndReplacements(RQMTLevel, RQMTNumberLevel, PrimaryRQMTLevel, PrimaryRQMTNumberLevel, RQMTSetUsageLevel, level, ref rqmtMode, ref nextLevelRQMTMode, replacements);

            string customWhere = null;
            if (!string.IsNullOrWhiteSpace(FilterToSets))
            {
                customWhere = /* and */ "rset.RQMTSetID IN (" + FilterToSets + ")"; // this allows us to specifically force a search just for certain sets from the builder (allows us to restrict the normal filter settings even more) - NOTE: THERE IS A BUG IN THE BUILDER IN THAT IT CAN ALLOW YOU TO BRING UP SETS OUTSIDE OF YOUR FILTER SETTINGS (SINCE IT DOESN'T PAY ATTENTION TO FILTERS)
            }

            DataTable dt = RQMT.RQMT_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, whereExts: docWhereExts, RQMTMode: rqmtMode, CountColumns: null, customWhere: customWhere, ignoreUserFilters: IgnoreUserFilters);

            // when we have a rqmt and description/func/defects at the same level as rqmts, we flatten the data table so we get one description row per rqmt
            // the level > this.LevelCount just catches the rawdata level to ensure that the combining happens in the raw data export rather than just at the actual rqmts levels
            if (level > this.LevelCount || (level == RQMTLevel || level == PrimaryRQMTLevel || level == RQMTNumberLevel || level == PrimaryRQMTNumberLevel))
            {
                if (dt.Columns.Contains("Description")) dt = dt.CombineRowsOnColumns(new string[] { "DESCRIPTION_ID", "Description" }, new string[] { "\n", "\n" }, true);
                if (dt.Columns.Contains("Defects")) dt = dt.CombineRowsOnColumns(new string[] { "RQMTSYSTEMDEFECT_ID", "Defects" }, new string[] { "\n", "\n" }, true);
                if (dt.Columns.Contains("Functionality")) dt = dt.CombineRowsOnColumns(new string[] { "FUNCTIONALITY_ID", "Functionality" }, new string[] { "\n", "\n" }, true);
            }

            // quick hack - the user doesn't want description type in the excel export (we will eventually created a separate column for it)
            if (dt.Columns.Contains("Description_COMBINED"))
            {
                dt.Columns["Description_COMBINED"].ReadOnly = false;
                foreach (DataRow dr in dt.Rows)
                {
                    string desc = dr["Description_COMBINED"] != DBNull.Value ? (string)dr["Description_COMBINED"] : "";
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        int idx = desc.LastIndexOf(" (");
                        if (idx > 0)
                        {
                            desc = desc.Substring(0, idx);
                            dr["Description_COMBINED"] = desc;
                        }
                    }
                }
                dt.AcceptChanges();
            }
            else if (dt.Columns.Contains("Description"))
            {
                dt.Columns["Description"].ReadOnly = false;
                foreach (DataRow dr in dt.Rows)
                {
                    string desc = dr["Description"] != DBNull.Value ? (string)dr["Description"] : "";
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        int idx = desc.LastIndexOf(" (");
                        if (idx > 0)
                        {
                            desc = desc.Substring(0, idx);
                            dr["Description"] = desc;
                        }
                    }
                }
                dt.AcceptChanges();
            }

            // preprocess some of the data
            if (dt.Columns.Contains("Month_1")) // for the export, we want 1s/0s to show instead of TRUE/FALSE, so we change the column around (but since we can't change a data type on the fly, we have to add a new column and then delete the old one)
            {
                for (int m = 1; m <= 12; m++)
                {
                    DataColumn col = new DataColumn("Month_" + m + "_NEW", typeof(int));
                    dt.Columns.Add(col);
                }

                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    DataRow row = dt.Rows[r];

                    for (int m = 1; m <= 12; m++)
                    {
                        bool on = false;

                        object month = row["Month_" + m];

                        if (month != null && month != DBNull.Value)
                        {
                            string mstr = month.ToString().ToUpper();

                            on = mstr == "1" || mstr == "TRUE";
                        }

                        if (on)
                        {
                            row["Month_" + m + "_NEW"] = 1;
                        }
                        else
                        {
                            row["Month_" + m + "_NEW"] = 0;
                        }
                    }
                }

                for (int m = 1; m <= 12; m++)
                {
                    DataColumn col = dt.Columns["Month_" + m];
                    dt.Columns.Remove(col);
                }

                dt.AcceptChanges();

                for (int m = 1; m <= 12; m++)
                {
                    DataColumn col = dt.Columns["Month_" + m + "_NEW"];
                    col.ColumnName = "Month_" + m;
                }

                dt.AcceptChanges();
            }

            levelDts.Add(dt);
        }

        List<DataTable> flatSheetDt = new List<DataTable>();
        flatSheetDt.Add(levelDts.Last());
        levelDts.Remove(levelDts.Last());

        List<List<DataTable>> dtSheets = new List<List<DataTable>>();        
        dtSheets.Add(levelDts);
        dtSheets.Add(flatSheetDt);

        List<ExcelDataTableFormat> formats = new List<ExcelDataTableFormat>();
        for (int i = 0; i < 5; i++)
        {
            ExcelDataTableFormat format = ExcelDataTableFormat.GetDefaultFormat(i, levelDts.Count);
            format.ColumnWidths["RQMT"] = 50.0f;
            format.ColumnWidths["RQMT PRIMARY"] = 50.0f;
            format.ColumnWidths["Description"] = 50.0f;
            format.ColumnWidths["Defects"] = 50.0f;
            format.ColumnWidths["Functionality"] = 50.0f;
            formats.Add(format);
        }
        eu.SetSheetFormats(0, formats);

        ExcelDataTableFormat rawFormat = ExcelDataTableFormat.GetDefaultFormat(0, 1);
        rawFormat.ColumnWidths["RQMT"] = 50.0f;
        rawFormat.ColumnWidths["RQMT PRIMARY"] = 50.0f;
        rawFormat.ColumnWidths["Description"] = 50.0f;
        rawFormat.ColumnWidths["Defects"] = 50.0f;
        rawFormat.ColumnWidths["Functionality"] = 50.0f;
        rawFormat.AllowWrap = false;
        eu.SetSheetFormat(1, rawFormat);

        Dictionary<string, string> columnRenames = new Dictionary<string, string>();
        for (int i = 1; i <= 12; i++)
        {
            DateTime dt = new DateTime(2018, i, 1);
            
            columnRenames["Month_" + i] = dt.ToString("MMM").Substring(0, 3).ToUpper();
        }

        columnRenames["Outline Index"] = "IDX";
        columnRenames["Outline Index Parent"] = "IDXL1";
        columnRenames["Outline Index Child"] = "IDXL2";
        columnRenames["RQMT Type"] = "Purpose";
        columnRenames["Description_COMBINED"] = "Description";
        columnRenames["Defects_COMBINED"] = "Defects";
        columnRenames["Functionality_COMBINED"] = "Functionality";
        columnRenames["RQMT Set"] = "RQMT Set Name";
        columnRenames["RQMT Stage"] = "PD2TDR";
        columnRenames["RQMT PRIMARY"] = "RQMT Primary";
        columnRenames["RQMT PRIMARY #"] = "RQMT Primary #";
        columnRenames["Defect Stage"] = "Defect PD2TDR";
        columnRenames["Defect Number"] = "Defect #";
        columnRenames["Defect Verified"] = "Verified";
        columnRenames["Defect Resolved"] = "Resolved";
        columnRenames["Defect Review"] = "Review";
        columnRenames["Defect Mitigation"] = "Mitigation";

        eu.ColumnRenames = columnRenames;
        eu.GroupRows = true;
        eu.SetSheetName(0, "RQMTS");
        eu.SetSheetName(1, "RAW DATA");
        eu.RowComparator = new RQMTGridExportRowComparator();

        eu.WriteMultiLevelCrosswalkToResponse(dtSheets, Response, endResponse: true);
    }

    #endregion

    #region AJAX
    [WebMethod()]
    public static string DeleteRQMTs(string checkedRQMTIDs, bool globalDelete)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        string lastDeleteID = "";

        try
        {
            string[] ids = checkedRQMTIDs.Split(','); //RQMT+SYSTEM+SET,RQMT+SYSTEM+SET

            for (int i = ids.Length - 1; i >= 0; i--) // we go backwards so we can re-order as we go and not delete parents before children
            {
                string rplusset = ids[i]; // RQMT+SYSTEM+SET
                string[] rarr = rplusset.Split('+');

                int rqmtid = Int32.Parse(rarr[0]);
                int sysid = Int32.Parse(rarr[1]);
                int rqmtsetid = Int32.Parse(rarr[2]);

                if (globalDelete)
                {
                    DataTable dt = RQMT.RQMT_AssociatedSets_Get(rqmtid);

                    foreach (DataRow row in dt.Rows)
                    {
                        RQMT.RQMTSet_DeleteRQMT((int)row["RQMTSetID"], rqmtid, 0, true);
                    }
                }
                else
                {
                    RQMT.RQMTSet_DeleteRQMT(rqmtsetid, rqmtid, 0, true);
                }
            }

            result["success"] = "true";
            deleted = true;
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            deleted = false;
            errorMsg = "Error deleting RQMT " + lastDeleteID + ". " + ex.Message;
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
        StringBuilder output = new StringBuilder();

        try
        {
            XmlDocument docChanges = (XmlDocument)JsonConvert.DeserializeXmlNode(changes, "changes");            

            saved = RQMT.RQMT_Update(Changes: docChanges, Output: output);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            saved = false;
            errorMsg = ex.Message;            
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;
        result["output"] = output.ToString();

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod(EnableSession = true)]
    public static bool UpdateSession(string ddlIndex, string ddlText, string sectionsXml, string spk)
    {
        var sessionMethods = new SessionMethods();
        sessionMethods.Session[spk + "RQMT_CurrentXML"] = sectionsXml;
        sessionMethods.Session[spk + "RQMT_CurrentDropDown"] = ddlIndex;
        sessionMethods.Session[spk + "RQMT_GridView"] = ddlText;
        sessionMethods.Session[spk + "RQMT_GridView_Default"] = ddlText;

        if (sectionsXml != "")
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(sectionsXml);
            sessionMethods.Session[spk + "RQMT_Levels"] = xml;
        }
        return true;
    }
    #endregion

    public class RQMTGridExportRowComparator : MultiLevelCrosswalkRowComparator
    {
        public bool DoesChildRowColumnMatchParentRowColumn(DataRow parentRow, string parentVal, DataRow childRow, string childVal, string columnName)
        {
            bool match = true;

            columnName = columnName.ToUpper(); // column names are coming from the PARENT ROW to see if a child matches it

            bool parentRQMTRow = false;           
            for (int x = 0; x < parentRow.Table.Columns.Count; x++)
            {
                string col = parentRow.Table.Columns[x].ColumnName.ToLower();

                if (col == "rqmt_id" || col == "rqmtname_id" || col.IndexOf("rqmtprimary") != -1 || col.IndexOf("rqmtnested") != -1)
                {
                    parentRQMTRow = true;
                    break;
                }
            }

            bool childRQMTRow = false;
            for (int x = 0; x < childRow.Table.Columns.Count; x++)
            {
                string col = childRow.Table.Columns[x].ColumnName.ToLower();

                if (col == "rqmt_id" || col == "rqmtname_id" || col.IndexOf("rqmtprimary") != -1 || col.IndexOf("rqmtnested") != -1)
                {
                    childRQMTRow = true;
                    break;
                }
            }

            if ((parentRQMTRow || childRQMTRow) && (columnName.Contains("RQMTACCEPTED_ID") || columnName.Contains("RQMTCRITICALITY_ID") || columnName.Contains("RQMTSTAGE_ID") || columnName.Contains("RQMTSTATUS_ID") || columnName.Contains("DESCRIPTION_ID") || columnName.Contains("RQMTSYSTEMDEFECT_ID") || columnName.Contains("FUNCTIONALITY_ID")))
            {
                return true; // we never drill down on attributes of rqmts when they are on a rqmt row
            }
            else if (columnName == "RQMTPRIMARYNESTEDNUMBER_PARENT_ID" || columnName == "RQMTPRIMARYNESTEDNUMBER_PARENT_ID_COUNTCOLUMN") {
                // we have a parent-child relationship
                // in the parent row, this value will be blank since it points to a parent (which the parent doesn't have)
                // in the child row, it will point to the parent
                // so when we see this column:
                //    1) if the value is blank, we get the RQMTPRIMARYNESTEDNUMBER_ID value and compare it to the child row's RQMTPRIMARYNESTEDNUMBER_PARENT_ID
                //    2) if the value is not blank, we are already on a child row (or below it), so we keep comparing the parent to the parent on the next row
                if (parentVal == null) // we are at the top level
                {
                    if (parentRow.Table.Columns.Contains("RQMTPRIMARYNESTEDNUMBER_ID"))
                    {
                        parentVal = parentRow["RQMTPRIMARYNESTEDNUMBER_ID"].ToString();
                    }
                    else if (parentRow.Table.Columns.Contains("RQMTPRIMARYNESTEDNUMBER_ID_COUNTCOLUMN"))
                    {
                        parentVal = parentRow["RQMTPRIMARYNESTEDNUMBER_ID_COUNTCOLUMN"].ToString();
                    }
                }

                object childValObj = null;

                if (childRow.Table.Columns.Contains("RQMTPRIMARYNESTEDNUMBER_PARENT_ID"))
                {
                    childValObj = childRow["RQMTPRIMARYNESTEDNUMBER_PARENT_ID"];
                }
                else if (childRow.Table.Columns.Contains("RQMTPRIMARYNESTEDNUMBER_PARENT_ID_COUNTCOLUMN"))
                {
                    childValObj = childRow["RQMTPRIMARYNESTEDNUMBER_PARENT_ID_COUNTCOLUMN"];
                }

                childVal = null;

                if (childValObj != DBNull.Value)
                {
                    childVal = childValObj.ToString();

                    if (string.IsNullOrWhiteSpace(childVal))
                    {
                        childVal = null;
                    }
                }
            }
            else if (columnName == "RQMTPRIMARYNESTEDNAME_PARENT_ID" || columnName == "RQMTPRIMARYNESTEDNAME_PARENT_ID_COUNTCOLUMN")
            {
                if (parentVal == null) // we are at the top level
                {
                    if (parentRow.Table.Columns.Contains("RQMTPRIMARYNESTEDNAME_ID"))
                    {
                        parentVal = parentRow["RQMTPRIMARYNESTEDNAME_ID"].ToString();
                    }
                    else if (parentRow.Table.Columns.Contains("RQMTPRIMARYNESTEDNAME_ID_COUNTCOLUMN"))
                    {
                        parentVal = parentRow["RQMTPRIMARYNESTEDNAME_ID_COUNTCOLUMN"].ToString();
                    }
                }

                // in the child table, we need to refer to the _PARENT_ID version
                object childValObj = null;

                if (childRow.Table.Columns.Contains("RQMTPRIMARYNESTEDNAME_PARENT_ID"))
                {
                    childValObj = childRow["RQMTPRIMARYNESTEDNAME_PARENT_ID"];
                }
                else if (childRow.Table.Columns.Contains("RQMTPRIMARYNESTEDNAME_PARENT_ID_COUNTCOLUMN"))
                {
                    childValObj = childRow["RQMTPRIMARYNESTEDNAME_PARENT_ID_COUNTCOLUMN"];
                }

                childVal = null;

                if (childValObj != DBNull.Value)
                {
                    childVal = childValObj.ToString();

                    if (string.IsNullOrWhiteSpace(childVal))
                    {
                        childVal = null;
                    }
                }
            }
            else if (columnName == "RQMTPRIMARYNESTEDNUMBER_ID" || columnName == "RQMTPRIMARYNESTEDNUMBER_ID_COUNTCOLUMN" ||
                columnName == "RQMTPRIMARYNESTEDNAME_ID" || columnName == "RQMTPRIMARYNESTEDNAME_ID_COUNTCOLUMN")
            {
                return true; // we only look down through the parent_id column
            }


            if (parentVal != childVal)
            {
                match = false;
            }

            return match;
        }
    }

    [WebMethod()]
    public static string CopyRQMTs(string RQMTIDs)
    {
        var result = WTS.WTSPage.CreateDefaultResult();

        if (string.IsNullOrWhiteSpace(RQMTIDs))
        {
            System.Web.HttpContext.Current.Session["copied.rqmts"] = "";
            result["systemids"] = "";
            result["rqmtidsstr"] = "";
        }
        else
        {
            string previousCopiedRQMTs = (string)HttpContext.Current.Session["copied.rqmts"];
            if (string.IsNullOrWhiteSpace(previousCopiedRQMTs)) previousCopiedRQMTs = ",,"; // just allows us to do a simple split to create the empty arr below
            string[] arr = previousCopiedRQMTs.Split(',');
            string prevRQMTIDs = arr[0];
            string prevSystemIDs = arr[1];
            string prevRQMTSet_RQMTSystemIDs = arr[2];

            // the copied rqmt format is RQMTID|RQMTID,SYSID|SYSID,RQMTSETRQMTSYSTEMID|RQMTSETRQMTSYSTEMID

            // the rqmtgrid format is RQMTID+SYSID+RQMTSET,RQMTID+SYSID+RQMTSETID
            string newRQMTIDs = prevRQMTIDs; // RQMTID|RQMTID
            string newSystemIDs = prevSystemIDs; // SYSID|SYSID
            string newRQMTSet_RQMTSystemIDs = prevRQMTSet_RQMTSystemIDs; // RQMTSETRQMTSYSTEMID|RQMTSETRQMTSYSTEMID

            string[] newCopiedGridRQMTs = RQMTIDs.Split(',');

            for (int i = 0; i < newCopiedGridRQMTs.Length; i++)
            {
                string[] gridRQMTArr = newCopiedGridRQMTs[i].Split('+');
                int rqmtid = Int32.Parse(gridRQMTArr[0]);
                if (rqmtid == 0) continue;
                int sysid = Int32.Parse(gridRQMTArr[1]);
                int rsetid = Int32.Parse(gridRQMTArr[2]);
                int rqmtsetrqmtsystemid = RQMT.RQMT_GetRQMTSetRQMTSystemForSet(rqmtid, rsetid);

                if (!("|" + prevRQMTIDs + "|").Contains("|" + rqmtid + "|"))
                {
                    if (newRQMTIDs.Length > 0) { newRQMTIDs += "|"; }
                    if (newSystemIDs.Length > 0) { newSystemIDs += "|"; }
                    if (newRQMTSet_RQMTSystemIDs.Length > 0) { newRQMTSet_RQMTSystemIDs += "|"; }

                    newRQMTIDs += rqmtid;
                    newSystemIDs += sysid;
                    newRQMTSet_RQMTSystemIDs += rqmtsetrqmtsystemid;
                }
            }

            System.Web.HttpContext.Current.Session["copied.rqmts"] = newRQMTIDs + "," + newSystemIDs + "," + newRQMTSet_RQMTSystemIDs;

            result["systemids"] = newSystemIDs;

            if (newRQMTIDs.Length > 0)
            {
                string fixedStr = "";

                string[] newRQMDIdsArr = newRQMTIDs.Split('|');

                for (int i = 0; i < newRQMDIdsArr.Length; i++) // this loops adds *'s if we have dup rqmt numbers from different sets
                {
                    if (fixedStr.Length > 0) fixedStr += ", ";

                    if (fixedStr.Contains(newRQMDIdsArr[i]))
                    {
                        fixedStr += newRQMDIdsArr[i] + "*";
                    }
                    else
                    {
                        fixedStr += newRQMDIdsArr[i];
                    }

                }

                result["rqmtidsstr"] = fixedStr;
            }
            else
            {
                result["rqmtidsstr"] = "";
            }
            
        }
        
        result["success"] = "true";

        return WTS.WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string PasteRQMTs(int RQMTSetID, string options)
    {
        var result = WTS.WTSPage.CreateDefaultResult();

        string previousCopiedRQMTs = (string)HttpContext.Current.Session["copied.rqmts"]; // RQMTS, SYSIDS, RSRSIDS

        if (options == null) options = "";
        bool pasteAttributes = options.IndexOf("attr") != -1;
        bool pasteDefects = options.IndexOf("def") != -1;
        bool pasteDescriptions = options.IndexOf("desc") != -1;

        string pasteOptions = (pasteAttributes ? "attr," : "") + (pasteDefects ? "def," : "") + (pasteDescriptions ? "desc," : "");
        if (pasteOptions.Length > 0)
        {
            pasteOptions = pasteOptions.Substring(0, pasteOptions.Length - 1);
        }

        if (!string.IsNullOrWhiteSpace(previousCopiedRQMTs))
        {
            string[] arr = previousCopiedRQMTs.Split(',');
            string[] prevRQMTIDs = arr[0].Split('|');
            string[] prevRQMTSet_RQMTSystemIDs = arr[2].Split('|');

            for (int i = 0; i < prevRQMTIDs.Length; i++)
            {
                int RQMTID = Int32.Parse(prevRQMTIDs[i]);
                int RQMTSet_RQMTSystemID = Int32.Parse(prevRQMTSet_RQMTSystemIDs[i]);

                RQMT.RQMTSet_AddRQMT(RQMTSetID, RQMTID, null, false, RQMTSet_RQMTSystemID , pasteOptions);
            }

            RQMT.RQMTSet_ReorderRQMTs(RQMTSetID, null); // this forces us to reclaim space from previously deleted rqmts

            result["success"] = "true";
        }
        else // we shouldn't get here; the UI should have disabled the paste button if there is nothing on the clipboard
        {
            result["success"] = "true";
        }

        return WTS.WTSPage.SerializeResult(result);
    }
}