﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using WTS;
using WTS.Util;

using Newtonsoft.Json;

public partial class RQMT_Edit : WTSPage
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditRQMT = false;
    protected bool NewRQMT = false;
    protected int RQMTID = 0;
    protected int NewRQMTSetID = 0;
    protected int NewParentRQMTID = 0;
    protected string OpenSections = "";
    protected string HideNonOpenSections = "0";
    protected int DisplayItemID = 0; // if specified, it means the page was loaded with the intention to show ONE item from the RQMT (usually in conjunction with a specific open section and the hidenonopensections params)
    protected string DisplayItemSubSection = "";
    protected bool CanDeleteRQMT = false;
    protected bool CanEditTitle = false;

    private DataTable dtRQMT;
    private DataTable dtAllSets;
    private DataTable dtAssociations;
    private DataTable dtAttributes;
    private DataTable dtRQMTAttribute;
    private DataTable dtUsage;
    private DataTable dtAvailableFunctionalities;
    private DataTable dtFunctionality;
    private DataTable dtFunctionalityDistinct;
    private DataTable dtDescriptions;
    private DataTable dtDescriptionsDistinct;
    private DataTable dtDescriptionTypes;
    private DataTable dtDescriptionAttachments;
    private DataTable dtDefects;
    private DataTable dtDefectsDistinct;

    protected int SetAssociationsCount = 0;
    protected int SystemAssociationsCount = 0;
    protected int SystemDescriptionsCount = 0;

    protected string descTypeOptions = "";
    protected string sectionCounts = "";

    protected bool pageIsInvalid = false;


    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        this.CanEditRQMT = UserManagement.UserCanEdit(WTSModuleOption.RQMT);
        this.CanEditTitle = this.CanEditRQMT && (string.IsNullOrWhiteSpace(OpenSections) || HideNonOpenSections != "1"); // if we are editing a specific item, we don't allow editing of title, and instead just focus on the one item

        LoadData();
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

        if (Request.QueryString["NewRQMT"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewRQMT"]))
        {
            bool.TryParse(Request.QueryString["NewRQMT"], out this.NewRQMT);
        }

        if (Request.QueryString["RQMTID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["RQMTID"]))
        {
            int.TryParse(Request.QueryString["RQMTID"], out this.RQMTID);
        }

        if (this.RQMTID == 0)
        {
            this.NewRQMT = true;
        }

        if (Request.QueryString["RQMTSetID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["RQMTSetID"]))
        {
            int.TryParse(Request.QueryString["RQMTSetID"], out this.NewRQMTSetID);
        }

        if (Request.QueryString["ParentRQMTID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ParentRQMTID"]))
        {
            int.TryParse(Request.QueryString["ParentRQMTID"], out this.NewParentRQMTID);
        }

        if (Request.QueryString["ItemID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ItemID"]))
        {
            int.TryParse(Request.QueryString["ItemID"], out this.DisplayItemID);
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["ItemSubSection"]))
        {
            this.DisplayItemSubSection = Request.QueryString["ItemSubSection"];
        }

        OpenSections = Request.QueryString["OpenSections"];
        if (string.IsNullOrWhiteSpace(OpenSections)) OpenSections = "_details_";

        if (!string.IsNullOrWhiteSpace(Request.QueryString["HideNonOpenSections"]))
        {
            HideNonOpenSections = Request.QueryString["HideNonOpenSections"] == "1" ? "1" : "0";
        }
    }
    #endregion

    #region Data
    private void LoadData()
    {
        // get the full attribute list
        dtRQMTAttribute = RQMT.RQMTAttribute_Get();

        if (!this.NewRQMT)
        {
            DataSet dsRQMT = RQMT.RQMTEditData_Get(RQMTID);

            dtRQMT = dsRQMT.Tables["RQMT"];

            if (dtRQMT.Rows.Count == 0)
            {
                pageIsInvalid = true;
                return;
            }

            dtAllSets = dsRQMT.Tables["ALLSETS"];
            dtAssociations = dsRQMT.Tables["ASSOCIATIONS"];
            dtAttributes = dsRQMT.Tables["ATTRIBUTES"];
            dtUsage = dsRQMT.Tables["USAGE"];
            dtAvailableFunctionalities = dsRQMT.Tables["AVAILABLEFUNCTIONALITIES"];
            dtFunctionality = dsRQMT.Tables["FUNCTIONALITY"];
            dtDescriptions = dsRQMT.Tables["DESCRIPTIONS"];
            dtDescriptionTypes = dsRQMT.Tables["DESCTYPES"];
            dtDefects = dsRQMT.Tables["DEFECTS"];
            dtDescriptionAttachments = dsRQMT.Tables["DESCATT"];

            SetAssociationsCount = dtAssociations.Rows.Count;
            SystemAssociationsCount = dtAttributes.Rows.Count;

            sectionCounts = "associations=" + SetAssociationsCount + ",attributes=" + SystemAssociationsCount + ",usage=" + SetAssociationsCount + ",functionalities=0,descriptions=0,defects=0";

            CanDeleteRQMT = true;
            if (SetAssociationsCount > 0 ||
                (dtDefects.Rows.Count > 0 && dtDefects.Rows[0]["RQMTSystemDefect_ID"] != DBNull.Value) ||
                (dtFunctionality.Rows.Count > 0 && dtFunctionality.Rows[0]["Functionality_ID"] != DBNull.Value) ||
                (dtDescriptions.Rows.Count > 0 && dtDescriptions.Rows[0]["RQMTSystemRQMTDescription_ID"] != DBNull.Value))
            {
                CanDeleteRQMT = false;
            }

            descTypeOptions = CreateOptionStringFromDataTable(dtDescriptionTypes, "RQMTDescriptionType_ID", "RQMTDescriptionType", null, null, true);

            if (dtRQMT != null && dtRQMT.Rows.Count > 0)
            {
                spnRQMT.InnerText = dtRQMT.Rows[0]["RQMTID"].ToString();

                string createdDateDisplay = string.Empty, updatedDateDisplay = string.Empty;
                DateTime nCreatedDate = new DateTime(), nUpdatedDate = new DateTime();

                if (DateTime.TryParse(dtRQMT.Rows[0]["CreatedDate"].ToString(), out nCreatedDate)) createdDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nCreatedDate);
                if (DateTime.TryParse(dtRQMT.Rows[0]["UpdatedDate"].ToString(), out nUpdatedDate)) updatedDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nUpdatedDate);

                spnCreated.InnerText = "Created: " + dtRQMT.Rows[0]["CreatedBy"].ToString() + " - " + createdDateDisplay;
                spnUpdated.InnerText = "Updated: " + dtRQMT.Rows[0]["UpdatedBy"].ToString() + " - " + updatedDateDisplay;
                txtRQMT.Text = dtRQMT.Rows[0]["RQMT"].ToString();
                txtRQMT.Attributes.Add("original_value", dtRQMT.Rows[0]["RQMT"].ToString());
            }

            if (dtAllSets != null && dtAllSets.Rows.Count > 0)
            {
                dtAllSets.Columns.Add("X", typeof(string));
                dtAllSets.Columns["X"].SetOrdinal(0);
                dtAllSets.Columns.Add("Z", typeof(string));

                grdAssociations.GridHeaderRowDataBound += grdAssociations_GridHeaderRowDataBound;
                grdAssociations.GridRowDataBound += grdAssociations_GridRowDataBound;
                grdAssociations.DataSource = dtAllSets;
                grdAssociations.DataBind();
            }

            if (dtAttributes != null && dtAttributes.Rows.Count > 0)
            {
                dtAttributes.Columns.Add("Z", typeof(string));

                grdAttributes.GridHeaderRowDataBound += grdAttributes_GridHeaderRowDataBound;
                grdAttributes.GridRowDataBound += grdAttributes_GridRowDataBound;
                grdAttributes.DataSource = dtAttributes;
                grdAttributes.DataBind();
            }

            if (dtUsage != null && dtUsage.Rows.Count > 0)
            {
                dtUsage.Columns.Add("Toggle", typeof(string));
                dtUsage.Columns.Add("Z", typeof(string));

                grdUsage.GridHeaderRowDataBound += grdUsage_GridHeaderRowDataBound;
                grdUsage.GridRowDataBound += grdUsage_GridRowDataBound;
                grdUsage.DataSource = dtUsage;
                grdUsage.DataBind();
            }

            if (dtFunctionality != null && dtFunctionality.Rows.Count > 0)
            {
                // produce a unique table with one row per rsrs
                dtFunctionalityDistinct = dtFunctionality.Copy();
                dtFunctionalityDistinct.Columns.Remove("Functionality");
                dtFunctionalityDistinct.Columns.Remove("Functionality_ID");
                dtFunctionalityDistinct.AcceptChanges();
                dtFunctionalityDistinct = dtFunctionalityDistinct.DefaultView.ToTable(true);
                dtFunctionalityDistinct.Columns.Add("Functionality", typeof(string));

                grdFunctionalities.GridHeaderRowDataBound += grdFunctionalities_GridHeaderRowDataBound;
                grdFunctionalities.GridRowDataBound += grdFunctionalities_GridRowDataBound;
                grdFunctionalities.DataSource = dtFunctionalityDistinct;
                grdFunctionalities.DataBind();

                int cnt = 0;
                for (int i = 0; i < dtFunctionality.Rows.Count; i++)
                {
                    if (dtFunctionality.Rows[i]["Functionality_ID"] != DBNull.Value)
                    {
                        cnt++;
                    }
                }

                sectionCounts = sectionCounts.Replace("functionalities=0", "functionalities=" + cnt);
            }

            if (dtDescriptions != null && dtDescriptions.Rows.Count > 0)
            {
                // produce a unique table with one row per rs
                dtDescriptionsDistinct = dtDescriptions.Copy();
                dtDescriptionsDistinct.Columns.Remove("RQMTSystemRQMTDescription_ID");
                dtDescriptionsDistinct.Columns.Remove("RQMTDescription_ID");
                dtDescriptionsDistinct.Columns.Remove("RQMTDescription");
                dtDescriptionsDistinct.Columns.Remove("RQMTDescriptionType_ID");
                dtDescriptionsDistinct.Columns.Remove("RQMTDescriptionType");
                dtDescriptionsDistinct.AcceptChanges();
                dtDescriptionsDistinct = dtDescriptionsDistinct.DefaultView.ToTable(true);
                dtDescriptionsDistinct.Columns.Add("Descriptions", typeof(string));

                grdDescriptions.GridHeaderRowDataBound += grdDescriptions_GridHeaderRowDataBound;
                grdDescriptions.GridRowDataBound += grdDescriptions_GridRowDataBound;
                grdDescriptions.DataSource = dtDescriptionsDistinct;
                grdDescriptions.DataBind();

                int cnt = 0;
                for (int i = 0; i < dtDescriptions.Rows.Count; i++)
                {
                    int RQMTSystemRQMTDescription_ID = dtDescriptions.Rows[i]["RQMTSystemRQMTDescription_ID"] != DBNull.Value ? (int)dtDescriptions.Rows[i]["RQMTSystemRQMTDescription_ID"] : 0;

                    if (RQMTSystemRQMTDescription_ID != 0 && (DisplayItemID == 0 || RQMTSystemRQMTDescription_ID == DisplayItemID))
                    {
                        cnt++;
                    }
                }

                sectionCounts = sectionCounts.Replace("descriptions=0", "descriptions=" + cnt);
                SystemDescriptionsCount = cnt;
            }

            if (dtDefects != null && dtDefects.Rows.Count > 0)
            {
                dtDefectsDistinct = dtDefects.Copy();
                dtDefectsDistinct.Columns.Remove("RQMTSystemDefect_ID");
                dtDefectsDistinct.Columns.Remove("Description");
                dtDefectsDistinct.Columns.Remove("Impact");
                dtDefectsDistinct.Columns.Remove("RQMTStage");
                dtDefectsDistinct.Columns.Remove("SortOrder_ID");
                dtDefectsDistinct.AcceptChanges();
                dtDefectsDistinct = dtDefectsDistinct.DefaultView.ToTable(true);
                dtDefectsDistinct.Columns.Add("Defects", typeof(string));

                grdDefects.GridHeaderRowDataBound += grdDefects_GridHeaderRowDataBound;
                grdDefects.GridRowDataBound += grdDefects_GridRowDataBound;
                grdDefects.DataSource = dtDefectsDistinct;
                grdDefects.DataBind();

                int cnt = 0;
                for (int i = 0; i < dtDefects.Rows.Count; i++)
                {
                    if (dtDefects.Rows[i]["RQMTSystemDefect_ID"] != DBNull.Value)
                    {
                        cnt++;
                    }
                }

                sectionCounts = sectionCounts.Replace("defects=0", "defects=" + cnt);
            }
        }
    }
    #endregion

    #region grids
    private List<string> GetDefaultColumnStyles()
    {
        List<string> styles = new List<string>("WTS_SYSTEM=250,left;WorkArea=250,left;RQMTType=250,left;RQMTSetName=250,left;WTS_SYSTEM_SUITE=250,left;Accepted=50,center;Criticality=150,left;Stage=150,left;Status=150,left;Toggle=150,center;Functionality=400,left;Descriptions=450,left".Split(';'));

        for (int i = 1; i <= 12; i++)
        {
            DateTime dt = new DateTime(2018, i, 1);

            styles.Add("Month_" + i + "=35,center");
        }

        return styles;
    }

    private Dictionary<string, string> GetDefaultColumnHeaderReplacments()
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>();
        replacements.Add("X", "&nbsp;");
        replacements.Add("Z", "&nbsp;");
        replacements.Add("WTS_SYSTEM_SUITE", "Suite");
        replacements.Add("WTS_SYSTEM", "System");
        replacements.Add("WorkArea", "Work Area");
        replacements.Add("RQMTType", "Purpose");
        replacements.Add("RQMTSetName", "RQMT Set");

        for (int i = 1; i <= 12; i++)
        {
            DateTime dt = new DateTime(2018, i, 1);

            replacements.Add("Month_" + i, dt.ToString("MMM").Substring(0, 3).ToUpper());
        }

        return replacements;
    }

    private void FormatCell(string columnName, TableCell cell, bool headerRow, string defaultWidth, string defaultAlign)
    {
        List<string> colstyles = GetDefaultColumnStyles();

        cell.Style["white-space"] = "nowrap";
        cell.Style["vertical-align"] = "top";

        if (columnName.EndsWith("ID"))
        {
            cell.Style["display"] = "none";
        }
        else if (columnName == "X")
        {
            cell.Style["width"] = "35px";
            cell.Style["text-align"] = "center";
        }
        else if (columnName == "Z")
        {
            cell.Style["width"] = "100%";
            cell.Style["text-align"] = "center";
        }
        else if (colstyles.Find(colstyle => colstyle.Split('=')[0] == columnName) != null)
        {
            string[] styles = colstyles.Find(colstyle => colstyle.Split('=')[0] == columnName).Split('=')[1].Split(',');

            string width = styles[0];
            string align = styles[1];
            bool allowWrap = styles.Length > 2 ? styles[2] == "wrap" : false;

            cell.Style["width"] = (width.Contains("%") ? width : width + (width.Contains("px") ? "" : "px"));
            cell.Style["text-align"] = align;
            if (allowWrap)
            {
                cell.Style["white-space"] = "wrap";
            }
        }
        else
        {
            cell.Style["width"] = string.IsNullOrWhiteSpace(defaultWidth) ? "250px" : defaultWidth;
            cell.Style["text-align"] = string.IsNullOrWhiteSpace(defaultAlign) ? "left" : defaultAlign;
        }
    }

    private void grdAssociations_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e) {
        GridViewRow row = e.Row;

        Dictionary<string, string> replacements = GetDefaultColumnHeaderReplacments();

        for (int i = 0; i < dtAllSets.Columns.Count; i++)
        {
            string columnName = dtAllSets.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, true, null, null);

            if (replacements.ContainsKey(cell.Text))
            {
                cell.Text = replacements[cell.Text];
            }

            cell.Text = cell.Text != "&nbsp;" ? cell.Text.ToUpper() : "&nbsp;";
        }
    }

    private void grdAssociations_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        DataRow dr = ((System.Data.DataRowView)row.DataItem).Row;

        bool associated = false;

        int RQMTSetID = (int)dr["RQMTSet_ID"];
        row.Attributes["rsetid"] = RQMTSetID.ToString();

        bool startWithFieldChanged = false;
        
        if (RQMTID == 0 && RQMTSetID == NewRQMTSetID)
        {
            associated = true;
            startWithFieldChanged = true; // this allows us to trigger the checkbox in the addtosets list even when untouched, otherwise only modified checkboxes are chosen
        }

        for (int x = 0; x < dtAssociations.Rows.Count; x++)
        {
            if ((int)dtAssociations.Rows[x]["RQMTSet_ID"] == RQMTSetID)
            {
                associated = true;
                break;
            }
        }

        for (int i = 0; i < dtAllSets.Columns.Count; i++)
        {
            string columnName = dtAllSets.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, false, null, null);

            if (associated)
            {
                cell.Style["font-weight"] = "bold";
            }

            if (columnName == "X")
            {
                cell.Controls.Clear();

                var cb = CreateCheckBox("RQMTASSOCICATION", "ASSOCIATED", associated ? "1" : "0", (int)dr["WTS_SYSTEM_ID"], (int)dr["WorkArea_ID"], (int)dr["RQMTType_ID"], 0, RQMTSetID);
                if (startWithFieldChanged)
                {
                    cb.Attributes["fieldchanged"] = "1";
                    cb.InputAttributes["fieldchanged"] = "1";
                    cb.Attributes["originalvalue"] = "0";
                    cb.InputAttributes["originalvalue"] = "0";
                }

                cell.Controls.Add(cb);
            }
            else if (columnName == "RQMTSetName")
            {
                cell.Text = (int)dr["RQMTSet_ID"] + " - " + cell.Text;
            }
        }
    }

    private void grdAttributes_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        Dictionary<string, string> replacements = GetDefaultColumnHeaderReplacments();

        for (int i = 0; i < dtAttributes.Columns.Count; i++)
        {
            string columnName = dtAttributes.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, true, "150px", null);

            if (replacements.ContainsKey(cell.Text))
            {
                cell.Text = replacements[cell.Text];
            }

            cell.Text = cell.Text != "&nbsp;" ? cell.Text.ToUpper() : "&nbsp;";
        }
    }

    private void grdAttributes_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        DataRow dr = ((System.Data.DataRowView)row.DataItem).Row;

        int RQMTSystemID = (int)dr["RQMTSYSTEM_ID"];

        for (int i = 0; i < dtAttributes.Columns.Count; i++)
        {
            string columnName = dtAttributes.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, false, null, null);                        

            if (columnName == "Accepted")
            {
                cell.Controls.Clear();
                cell.Controls.Add(CreateCheckBox("RQMTATTRIBUTE", "ACCEPTED", (bool)dr["Accepted"] ? "1" : "0", (int)dr["WTS_SYSTEM_ID"], 0, 0, RQMTSystemID, 0));
            }
            else if (columnName == "Criticality")
            {
                DataTable dtCriticality = dtRQMTAttribute.Select("RQMTAttributeType='Criticality'").CopyToDataTable();
                cell.Controls.Clear();
                cell.Controls.Add(CreateDropDownList("RQMTATTRIBUTE", "CRITICALITY", dtCriticality, "RQMTAttribute", "RQMTAttributeID", true, dr["CriticalityID"] != DBNull.Value ? dr["CriticalityID"].ToString() : "0", (int)dr["WTS_SYSTEM_ID"], 0, 0, RQMTSystemID, 0));
            }
            else if (columnName == "Stage")
            {
                DataTable dtCriticality = dtRQMTAttribute.Select("RQMTAttributeType='RQMT Stage'").CopyToDataTable();
                cell.Controls.Clear();
                cell.Controls.Add(CreateDropDownList("RQMTATTRIBUTE", "STAGE", dtCriticality, "RQMTAttribute", "RQMTAttributeID", true, dr["RQMTStageID"] != DBNull.Value ? dr["RQMTStageID"].ToString() : "0", (int)dr["WTS_SYSTEM_ID"], 0, 0, RQMTSystemID, 0));
            }
            else if (columnName == "Status")
            {
                DataTable dtCriticality = dtRQMTAttribute.Select("RQMTAttributeType='RQMT Status'").CopyToDataTable();
                cell.Controls.Clear();
                cell.Controls.Add(CreateDropDownList("RQMTATTRIBUTE", "STATUS", dtCriticality, "RQMTAttribute", "RQMTAttributeID", true, dr["RQMTStatusID"] != DBNull.Value ? dr["RQMTStatusID"].ToString() : "0", (int)dr["WTS_SYSTEM_ID"], 0, 0, RQMTSystemID, 0));
            }
        }

        row.Attributes["rsid"] = RQMTSystemID.ToString();
        row.Attributes["sysid"] = dr["WTS_SYSTEM_ID"].ToString();
    }

    private void grdUsage_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        Dictionary<string, string> replacements = GetDefaultColumnHeaderReplacments();

        for (int i = 0; i < dtUsage.Columns.Count; i++)
        {
            string columnName = dtUsage.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, true, null, null);

            if (replacements.ContainsKey(cell.Text))
            {
                cell.Text = replacements[cell.Text];
            }

            cell.Text = cell.Text != "&nbsp;" ? cell.Text.ToUpper() : "&nbsp;";
        }
    }

    private void grdUsage_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        DataRow dr = ((System.Data.DataRowView)row.DataItem).Row;

        int RQMTSetID = (int)dr["RQMTSet_ID"];
        row.Attributes["rsetid"] = RQMTSetID.ToString();
        int RQMTSet_RQMTSystemID = (int)dr["RQMTSet_RQMTSystem_ID"];
        row.Attributes["rsetrsysid"] = RQMTSet_RQMTSystemID.ToString();

        Dictionary<string, string> replacements = GetDefaultColumnHeaderReplacments();

        for (int i = 0; i < dtUsage.Columns.Count; i++)
        {
            string columnName = dtUsage.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, false, null, null);

            if (columnName.StartsWith("Month_"))
            {
                cell.Controls.Clear();
                var cb = CreateCheckBox("RQMTUSAGE", columnName.ToUpper(), dr[columnName].ToString().ToUpper() == "TRUE" ? "1" : "0", (int)dr["WTS_SYSTEM_ID"], (int)dr["WorkArea_ID"], (int)dr["RQMTType_ID"], 0, RQMTSetID);
                cb.Attributes["rsetrsysid"] = RQMTSet_RQMTSystemID.ToString();
                cell.Controls.Add(cb);
            }
            else if (columnName.StartsWith("Toggle"))
            {
                cell.Text = "<span style=\"cursor:pointer;\" onclick=\"toggleUsage(" + RQMTSet_RQMTSystemID + ", true)\"><u>ALL</u></span>&nbsp;<span style=\"cursor:pointer;\" onclick=\"toggleUsage(" + RQMTSet_RQMTSystemID + ", false)\"><u>NONE</u></span>";
            }
        }
    }

    private void grdFunctionalities_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        Dictionary<string, string> replacements = GetDefaultColumnHeaderReplacments();

        for (int i = 0; i < dtFunctionalityDistinct.Columns.Count; i++)
        {
            string columnName = dtFunctionalityDistinct.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, true, null, null);

            if (replacements.ContainsKey(cell.Text))
            {
                cell.Text = replacements[cell.Text];
            }

            cell.Text = cell.Text != "&nbsp;" ? cell.Text.ToUpper() : "&nbsp;";
        }
    }

    private void grdFunctionalities_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        DataRow dr = ((System.Data.DataRowView)row.DataItem).Row;

        Dictionary<string, string> replacements = GetDefaultColumnHeaderReplacments();

        for (int i = 0; i < dtFunctionalityDistinct.Columns.Count; i++)
        {
            string columnName = dtFunctionalityDistinct.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, false, null, null);

            if (columnName == "Functionality")
            {
                int RQMTSet_RQMTSystemID = (int)dr["RQMTSet_RQMTSystem_ID"];
                row.Attributes["rsetrsysid"] = RQMTSet_RQMTSystemID.ToString();

                // the outer query just has one row per rsrs; now we need to look up the functionalities
                StringBuilder str = new StringBuilder();
                StringBuilder funcSelections = new StringBuilder();

                //str.Append("<div style=\"position:relative;\">"); // OUTERDIV1: this div is just to give the 3 inner divs a position to attach to for when they used absolute, fixed, or relative positioning (otherwise they try to attach to td, which doesn't always work)

                str.Append("<div availablefunctionalitiestext=\"1\" onclick=\"showAvailableFunctionalities(this, " + RQMTSet_RQMTSystemID + ")\" style=\"cursor:pointer;text-decoration:underline;width:100%;white-space:normal;\">");

                bool foundFunc = false; // the dtFunc is an outer join so we get rows for rsrs even without funcs (so we can then assign them)

                for (int x = 0; dtFunctionality != null && x < dtFunctionality.Rows.Count; x++) // dtfunctionality contains rows for all rsrs values
                {
                    DataRow drfunc = dtFunctionality.Rows[x];

                    if ((int)drfunc["RQMTSet_RQMTSystem_ID"] == RQMTSet_RQMTSystemID && drfunc["Functionality_ID"] != DBNull.Value)
                    {
                        if (foundFunc)
                        {
                            str.Append(", ");
                            funcSelections.Append(",");
                        }

                        foundFunc = true;

                        str.Append(drfunc["Functionality"].ToString());
                        funcSelections.Append(drfunc["Functionality_ID"].ToString());
                    }
                }

                if (!foundFunc)
                {
                    str.Append("NONE");
                }

                str.Append("</div>"); // availablefunctionalitiestext

                row.Attributes["funcselections"] = funcSelections.ToString();

                // available functionalities
                str.Append("<div availablefunctionalitiesglass=\"1\" rsetrsysid=\"" + RQMTSet_RQMTSystemID + "\" style=\"display:none;width:2000px;height:2000px;position:absolute;left:0px;top:0px;\" onclick=\"hideAvailableFunctionalities(null, this, " + RQMTSet_RQMTSystemID + ")\"></div>"); // GLASS

                str.Append("<div availablefunctionalities=\"1\" rsetrsysid=\"" + RQMTSet_RQMTSystemID + "\" style=\"width:200px;height:250px;position:absolute;display:none;overflow:hidden;background-color:#ffffff;border:1px solid #000000;z-index:10;\">");

                str.Append("<div style=\"cursor:pointer;width:200px;height:25px;background-color:#dddddd;border-bottom:1px solid #000000;\" onclick=\"hideAvailableFunctionalities(this, null, " + RQMTSet_RQMTSystemID + ")\">");
                str.Append("<span style=\"position:relative;top:5px;left:5px;font-size:smaller;\">(<u>click to close</u>)</span>");
                str.Append("<img src=\"images/icons/close_button_red.png\" style=\"cursor:pointer;position:absolute;width:25px;height:20px;right:2px;top:2px;\">");
                str.Append("</div>"); // availablefunctionalities dialog header

                str.Append("<div style=\"height:220px;overflow-y:auto;\">");
                //str.Append("<br /><br />"); // account for header img
                for (int x = 0; x < dtAvailableFunctionalities.Rows.Count; x++)
                {
                    DataRow drAF = dtAvailableFunctionalities.Rows[x];

                    if (x > 0) str.Append("<br />");

                    string selections = "," + funcSelections.ToString() + ",";

                    str.Append("<input type=\"checkbox\" value=\"" + drAF["WorkloadGroup_ID"] + "\" text=\"" + drAF["WorkloadGroup"] + "\"" + (selections.IndexOf("," + drAF["WorkloadGroup_ID"] + ",") != -1 ? " checked=\"true\"" : "") + ">&nbsp;" + drAF["WorkloadGroup"]);
                }

                str.Append("</div>"); // availablefunctionalities dialog checkbox list
                str.Append("</div>"); // availablefunctionalities dialog (contains the fixed header and the cb list)
                //str.Append("</div>"); // OUTERDIV1

                cell.Text = str.ToString();
            }
        }
    }

    private void grdDescriptions_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        Dictionary<string, string> replacements = GetDefaultColumnHeaderReplacments();

        for (int i = 0; i < dtDescriptionsDistinct.Columns.Count; i++)
        {
            string columnName = dtDescriptionsDistinct.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, true, null, null);
            if (columnName == "WTS_SYSTEM")
            {
                cell.Style["width"] = "190px";
            }

            if (replacements.ContainsKey(cell.Text))
            {
                cell.Text = replacements[cell.Text];
            }

            cell.Text = cell.Text != "&nbsp;" ? cell.Text.ToUpper() : "&nbsp;";
        }
    }

    private void grdDescriptions_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        DataRow dr = ((System.Data.DataRowView)row.DataItem).Row;

        int RQMTSystemID = (int)dr["RQMTSYSTEM_ID"];
        row.Attributes["rsid"] = RQMTSystemID.ToString();
        row.Attributes["rsmainrow"] = "1";

        Dictionary<string, string> replacements = GetDefaultColumnHeaderReplacments();

        for (int i = 0; i < dtDescriptionsDistinct.Columns.Count; i++)
        {
            string columnName = dtDescriptionsDistinct.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, false, null, null);
            if (columnName == "WTS_SYSTEM")
            {
                cell.Style["width"] = "190px";
            }

            if (columnName == "Descriptions")
            {
                StringBuilder str = new StringBuilder();

                str.Append("<div style=\"position:relative;width:100%;\">");

                str.Append("<div style=\"position:absolute;right:2px;top:2px;\"><input type=\"button\" value=\"ADD\" onclick=\"addDescription(this)\"></div>");
                
                str.Append("<table descriptiontable=\"1\" cellpadding=\"2\" cellpadding=\"0\" border=\"0\">");

                for (int x = 0; x < dtDescriptions.Rows.Count; x++)
                {
                    DataRow drDesc = dtDescriptions.Rows[x];

                    if ((int)drDesc["RQMTSYSTEM_ID"] == RQMTSystemID)
                    {
                        int RQMTSystemRQMTDescription_ID = drDesc["RQMTSystemRQMTDescription_ID"] != DBNull.Value ? (int)drDesc["RQMTSystemRQMTDescription_ID"] : 0;
                        int RQMTDescription_ID = drDesc["RQMTDescription_ID"] != DBNull.Value ? (int)drDesc["RQMTDescription_ID"] : 0;
                        string RQMTDescription = drDesc["RQMTDescription"] != DBNull.Value ? (string)drDesc["RQMTDescription"] : "";
                        int RQMTDescriptionType_ID = drDesc["RQMTDescriptionType_ID"] != DBNull.Value ? (int)drDesc["RQMTDescriptionType_ID"] : 0;

                        if (RQMTSystemRQMTDescription_ID > 0)
                        {
                            str.Append("<tr descriptionrow=\"1\" rsid=\"" + RQMTSystemID + "\" rsdescid=\"" + RQMTSystemRQMTDescription_ID + "\" rdescid=\"" + RQMTDescription_ID + "\">");

                            str.Append("  <td style=\"text-align:left;vertical-align:top;width:775px;border:0px;\">");
                            str.Append("    <textarea rsdesctextarea=\"1\" style=\"width:400px\" rows=\"3\">" + RQMTDescription + "</textarea>");
                            str.Append("    <div textareaorig=\"1\" style=\"display:none\">" + RQMTDescription + "</div>");
                            str.Append("    <div resultsdiv=\"1\" style=\"width:600px;position:absolute;height:100px;overflow-x:hidden;overflow-y:scroll;display:none;background-color:#ffffff;border:1px solid #000000;white-space:normal;\"></div>");
                            str.Append("    <div style=\"padding-top:3px;\">");                            
                            str.Append("      <span><img src=\"images/icons/attach.png\" style=\"cursor:pointer;\" onclick=\"attachFileToDescription(" + RQMTDescription_ID + ");\"><u style=\"cursor:pointer;\" onclick=\"attachFileToDescription(" + RQMTDescription_ID + ");\">Attach file</u></span>&nbsp;&nbsp;&nbsp;");
                            str.Append("      <span rqmtdescattcontainer=\"1\" rqmtdescid=\"" + RQMTDescription_ID + "\" style=\"white-space:normal;\">");
                            int attCount = 0;
                            for (int a = 0; a < dtDescriptionAttachments.Rows.Count; a++)
                            {
                                DataRow drAtt = dtDescriptionAttachments.Rows[a];

                                if ((int)drAtt["RQMTDescriptionID"] == RQMTDescription_ID)
                                {
                                    attCount++; // 5 // 9

                                    if (attCount > 1 && (attCount - 1) % 4 == 0)
                                    {
                                        str.Append("<br />");
                                    }
                                    
                                    str.Append("<span descattspan=\"1\" attid=\"" + (int)drAtt["AttachmentID"] + "\" onmouseover=\"$(this).find('img[deleteimg]').css('opacity', 1.0);\" onmouseout=\"$(this).find('img[deleteimg]').css('opacity', 0.2);\" style=\"line-height:25px;white-space:nowrap;\">&nbsp;&nbsp;&nbsp;<img src=\"images/icons/attach.png\" style=\"cursor:pointer;width:16px;height:16px;\" alt=\"Download\" title=\"Download\" onclick=\"openDescriptionAttachment(" + (int)drAtt["AttachmentID"] + ");\"><u style=\"cursor:pointer;\" onclick=\"openDescriptionAttachment(" + (int)drAtt["AttachmentID"] + ");\" alt=\"" + (string.IsNullOrWhiteSpace((string)drAtt["Description"]) ? "" : (string)drAtt["Description"] + " ") + "(" + (string)drAtt["ATTACHMENTTYPE"] + ")" + "\" title=\"" + (string.IsNullOrWhiteSpace((string)drAtt["Description"]) ? "" : (string)drAtt["Description"] + " ") + "(" + (string)drAtt["ATTACHMENTTYPE"]  + ")" + "\">" + (string)drAtt["FileName"] + "</u>&nbsp;<img deleteimg=\"1\" src=\"images/icons/cross.png\" style=\"width:16px;height:16px;opacity:0.2;cursor:pointer;\" onclick=\"deleteDescriptionAttachment(" + (int)drAtt["AttachmentID"] + ");\"></span>");
                                }
                            }
                            str.Append("      </span>");
                            str.Append("    </div>");
                            str.Append("  </td>");

                            str.Append("  <td style=\"text-align:left;vertical-align:top;border:0px;\">");
                            str.Append("    <select>");

                            for (int y = 0; y < dtDescriptionTypes.Rows.Count; y++)
                            {
                                DataRow drType = dtDescriptionTypes.Rows[y];

                                str.Append("<option value=\"" + (int)drType["RQMTDescriptionType_ID"] + "\"" + (RQMTDescriptionType_ID == (int)drType["RQMTDescriptionType_ID"] ? " selected=\"true\"" : "") + " > ");
                                str.Append((string)drType["RQMTDescriptionType"]);
                                str.Append("</option>");
                            }

                            str.Append("    </select>");
                            str.Append("    <br /><br />");
                            str.Append("    Save changes to:");
                            str.Append("    <div class=\"tooltip\" style=\"display:inline-block;border:0px\">");
                            str.Append("      <img src=\"images/icons/help.png\" width=\"12\" height=\"12\">");
                            str.Append("      <div class=\"tooltiptext tooltip-left-noarrow\" style=\"width:500px;text-align:left;\">");
                            str.Append("        The RQMT Module does not allow duplicate descriptions. Matching descriptions will share the same database value. When changing a description, you have the following options:<br /><br />&#8226;&nbsp;Update all instances of the description across all RQMT Sets <b>(THE DEFAULT)</b>, or<br />&#8226;&nbsp;Update all instances of the same description within the same System (across Work Areas and Purposes), or<br />&#8226;&nbsp;Save a new description for this row only"); 
                            str.Append("      </div>");
                            str.Append("    </div><br />");
                            str.Append("    <input type=\"radio\" name=\"changemode" + RQMTSystemRQMTDescription_ID + "\" value=\"all\" RQMTSystemRQMTDescription_ID=\"" + RQMTSystemRQMTDescription_ID + "\" checked>All instances<br />");
                            str.Append("    <input type=\"radio\" name=\"changemode" + RQMTSystemRQMTDescription_ID + "\" value=\"system\" RQMTSystemRQMTDescription_ID=\"" + RQMTSystemRQMTDescription_ID + "\">This system only<br />");
                            str.Append("    <input type=\"radio\" name=\"changemode" + RQMTSystemRQMTDescription_ID + "\" value=\"desc\" RQMTSystemRQMTDescription_ID=\"" + RQMTSystemRQMTDescription_ID  + "\">This description only");                            
                            
                            str.Append("  </td>");

                            str.Append("  <td style=\"text-align:left;vertical-align:top;border:0px;\">");
                            str.Append("    <img src=\"images/icons/cross.png\" style=\"cursor:pointer;\" onclick=\"deleteDescription(this);\">");
                            str.Append("  </td>");

                            str.Append("</tr>");
                        }
                    }
                }

                str.Append("</table>");

                str.Append("</div>");
                str.Append("<br /><br />");

                cell.Text = str.ToString();
            }
        }
    }

    private void grdDefects_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        Dictionary<string, string> replacements = GetDefaultColumnHeaderReplacments();

        for (int i = 0; i < dtDefectsDistinct.Columns.Count; i++)
        {
            string columnName = dtDefectsDistinct.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, true, null, null);

            if (replacements.ContainsKey(cell.Text))
            {
                cell.Text = replacements[cell.Text];
            }

            cell.Text = cell.Text != "&nbsp;" ? cell.Text.ToUpper() : "&nbsp;";
        }
    }

    private void grdDefects_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        DataRow dr = ((System.Data.DataRowView)row.DataItem).Row;

        int RQMTSystemID = (int)dr["RQMTSYSTEM_ID"];
        row.Attributes["rsid"] = RQMTSystemID.ToString();
        row.Attributes["rsmainrow"] = "1";

        Dictionary<string, string> replacements = GetDefaultColumnHeaderReplacments();

        for (int i = 0; i < dtDefectsDistinct.Columns.Count; i++)
        {
            string columnName = dtDefectsDistinct.Columns[i].ColumnName;
            TableCell cell = row.Cells[i];

            FormatCell(columnName, cell, false, null, null);

            if (columnName == "Defects")
            {
                StringBuilder str = new StringBuilder();
                StringBuilder defectsStr = new StringBuilder();

                defectsStr.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border:0px;width:525px\">");
                defectsStr.Append("  <tr>");
                defectsStr.Append("    <td style=\"width:25px;text-align:center;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;border:0px;\">#</td>");
                defectsStr.Append("    <td style=\"width:300px;text-align:left;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;\">Defect</td>");
                defectsStr.Append("    <td style=\"width:100px;text-align:left;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;\">Impact</td>");
                defectsStr.Append("    <td style=\"width:100px;text-align:left;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;\">PD2TDR</td>");
                defectsStr.Append("  </tr>");

                int defectsFound = 0;

                for (int x = 0; x < dtDefects.Rows.Count; x++)
                {
                    DataRow drDefect = dtDefects.Rows[x];

                    if ((int)drDefect["RQMTSYSTEM_ID"] == RQMTSystemID && drDefect["RQMTSystemDefect_ID"] != DBNull.Value)
                    {
                        defectsFound++;

                        string color = drDefect["Impact"] != DBNull.Value && (string)drDefect["Impact"] == "Work Stoppage" ? "color:red;" : "";

                        defectsStr.Append("  <tr>");
                        defectsStr.Append("    <td style=\"width:25px;text-align:center;vertical-align:top;border:0px;" + color + "\">" + defectsFound + "</td>");
                        defectsStr.Append("    <td style=\"width:300px;text-align:left;vertical-align:top;border:0px;" + color + "\">" + (drDefect["Description"] != DBNull.Value ? (string)drDefect["Description"] : "") + "</td>");
                        defectsStr.Append("    <td style=\"width:150px;text-align:left;vertical-align:top;border:0px;" + color + "\">" + (drDefect["Impact"] != DBNull.Value ? (string)drDefect["Impact"] : "") +  "</td>");
                        defectsStr.Append("    <td style=\"width:150px;text-align:left;vertical-align:top;border:0px;" + color + "\">" + (drDefect["RQMTStage"] != DBNull.Value ? (string)drDefect["RQMTStage"] : "") + "</td>");
                        defectsStr.Append("  </tr>");
                    }
                }

                defectsStr.Append("</table>");

                str.Append("<div class=\"tooltip\" style=\"cursor:pointer;\" onclick=\"DefectLinkClicked(" + RQMTID + ", " + (int)dr["WTS_SYSTEM_ID"] + ")\">");
                str.Append("DEFECTS <span sysid=\"" + (int)dr["WTS_SYSTEM_ID"] + "\" sysdefectcount=\"1\">(" + defectsFound + ")</span>");

                if (defectsFound > 0)
                {
                    str.Append("<div class=\"tooltiptext tooltip-bottom-noarrow\" style=\"width:475px; left:-400px;\">");
                    str.Append(defectsStr.ToString());
                    str.Append("</div>");
                }

                str.Append("</div>");

                cell.Text = str.ToString();
            }
        }
    }





    private CheckBox CreateCheckBox(string typeName, string field, string value, int sysid, int waid, int rtid, int rsid, int rsetid)
    {
        CheckBox cb = new CheckBox();
        cb.Attributes["typeName"] = typeName;
        cb.Attributes["field"] = field;
        cb.Attributes["sysid"] = sysid.ToString();
        cb.Attributes["waid"] = waid.ToString();
        cb.Attributes["rtid"] = rtid.ToString();
        cb.Attributes["rsetid"] = rsetid.ToString();
        cb.Attributes["onchange"] = "input_change(this);";
        cb.Attributes["originalvalue"] = value;

        cb.Checked = value == "1" || value == "true" || value == "TRUE";

        return cb;
    }

    private DropDownList CreateDropDownList(string typeName, string field, DataTable dt, string textColumn, string valueColumn, bool addBlank, string value, int sysid, int waid, int rtid, int rsid, int rsetid)
    {
        DropDownList ddl = new DropDownList();
        ddl.Attributes["typeName"] = typeName;
        ddl.Attributes["field"] = field;
        ddl.Attributes["sysid"] = sysid.ToString();
        ddl.Attributes["waid"] = waid.ToString();
        ddl.Attributes["rtid"] = rtid.ToString();
        ddl.Attributes["rsetid"] = rsetid.ToString();
        ddl.Attributes["onchange"] = "input_change(this);";
        ddl.Attributes["originalvalue"] = value;

        if (addBlank)
        {
            ddl.Items.Add(new ListItem("", "0"));
        }

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            DataRow row = dt.Rows[i];

            string txt = (string)row[textColumn];
            string val = row[valueColumn].ToString();

            ddl.Items.Add(new ListItem(txt, val));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            if (ddl.Items.FindByValue("0") != null)
            {
                ddl.SelectedValue = "0";
            }
            else
            {
                ddl.SelectedValue = "";
            }
        }
        else
        {
            ddl.SelectedValue = value;
        }

        return ddl;
    }

    #endregion

    #region AJAX
    [WebMethod()]
    public static string Save(int RQMTID, string RQMTName, string addToSets, string deleteFromSets, string attrChanges, string usageChanges, string funcChanges, string descChanges, int ParentRQMTID)
    {
        var result = WTSPage.CreateDefaultResult();

        RQMTName = RQMTName.Replace("|", "!");
        RQMTName = RQMTName.Replace("\r", "");
        RQMTName = RQMTName.Replace("\n", "");

        // new ID will be 0 if fail, RQMTID if updating, or a new RQMTID if new
        int savedID = RQMT.RQMTEditData_Save(RQMTID, RQMTName, addToSets, deleteFromSets, attrChanges, usageChanges, funcChanges, StringUtil.UndoStrongEscape(descChanges), RQMTID == 0 ? ParentRQMTID : 0);

        if (savedID > 0)
        {
            result["success"] = "true";

            if (RQMTID == 0)
            {
                result["newid"] = savedID.ToString();
            }
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string SearchDescriptions(string txt)
    {
        DataTable dt;

        if (string.IsNullOrWhiteSpace(txt))
        {
            txt = "__NONE__"; // we don't allow blank searches for descriptions
        }

        dt = RQMT.RQMTBuilderDescriptionList_Get(0, txt, false);

        return WTSPage.SerializeResult(dt);
    }

    [WebMethod()]
    public static string DeleteRQMT(int RQMTID)
    {
        var result = WTSPage.CreateDefaultResult();

        var deleteResult = RQMT.RQMT_Delete(RQMTID, false);

        result["success"] = deleteResult["deleted"];
        result["hasdependencies"] = deleteResult["hasdependencies"];

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string DeleteDescriptionAttachment(int AttachmentID)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMT.RQMTDescriptionAttachment_Delete(0, 0, AttachmentID))
        {            
            result["success"] = "true";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string DescriptionAttachmentsUpdated(int RQMTDescription_ID)
    {
        DataTable dt = RQMT.RQMTDescriptionAttachment_Get(0, RQMTDescription_ID, 0, false, 0);

        return WTSPage.SerializeResult(dt);
    }
    #endregion
}