﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using Aspose.Cells;
using System.IO;
using WTS.Util;
using System.Xml;

public partial class RQMTDefectsImpact_Grid : System.Web.UI.Page
{
    #region "Variables"
    protected int RQMT_ID = -1;
    protected int SYSTEM_ID = -1;
    protected string systemName = string.Empty;

    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected bool _refreshData = false;
    protected bool _export = false;

    protected string SortableColumns;
    protected string SortOrder;
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;

    protected bool CanView = false;
    protected bool CanEdit = false;
    protected bool IsAdmin = false;

    protected DataTable dtRQMTAttribute;
    protected DataTable dtSystem;
    protected DataTable theDT;
    protected DataTable dtSRs;
    protected DataTable dtSRs_COMBINED;
    protected DataTable dtTasks;
    protected string lastSRTable;
    protected string lastTaskTable;

    protected string matchAORSRWebSystems = "";

    #endregion

    #region "Page Methods"
    [WebMethod(true)]
    public static string SaveChanges(string changes, string strRQMT_ID, string strSYSTEM_ID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            int intRQMT_ID = 0;
            int intSYSTEM_ID = 0;

            int.TryParse(strRQMT_ID, out intRQMT_ID);
            int.TryParse(strSYSTEM_ID, out intSYSTEM_ID);

            DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(changes, (typeof(DataTable)));
            foreach (DataRow dr in dtjson.Rows)
            {
                int drRQMTSystemDefectID = 0;
                int drVerified = 0;
                int drResolved = 0;
                int drContinueToReview = 0;
                int drImpactID = 0;
                int drRQMTStageID = 0;

                int.TryParse(dr["RQMTSystemDefectID"].ToString(), out drRQMTSystemDefectID);
                int.TryParse(dr["Verified"].ToString(), out drVerified);
                int.TryParse(dr["Resolved"].ToString(), out drResolved);
                int.TryParse(dr["ContinueToReview"].ToString(), out drContinueToReview);
                int.TryParse(dr["Impact"].ToString(), out drImpactID);
                int.TryParse(dr["RQMTStage"].ToString(), out drRQMTStageID);

                saved = RQMT.RQMTDefectsImpact_Save(intRQMTID: intRQMT_ID, intSYSTEMID: intSYSTEM_ID, intRQMTSystemDefectID: drRQMTSystemDefectID, strDescription: dr["Description"].ToString(), intVerified: drVerified, intResolved: drResolved, intContinueToReview: drContinueToReview, intImpactID: drImpactID, intRQMTStageID: drRQMTStageID, mitigation: dr["Mitigation"].ToString());
            }
                //saved = RQMT.RQMTDefectsImpact_Save(Changes: docChanges);
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

    [WebMethod(true)]
    public static string DeleteItem(string strRQMTSystemDefectID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool exists = false, deleted = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            int rqmtSystemDefectID = 0;
            int.TryParse(strRQMTSystemDefectID, out rqmtSystemDefectID);

            deleted = RQMT.RQMTDefectsImpact_Delete(intRQMTSystemDefectID: rqmtSystemDefectID);
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

    #region "Page_Load"
    protected void Page_Load(object sender, EventArgs e)
    {
        this.IsAdmin = UserManagement.UserIsInRole("Admin");
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
        this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.MasterData));

        ReadQueryString();
        initControls();

        //Gather RQMTAttribute data for dropdown(s)
        if (Session["RQMTAttribute_Get"] == null)
        {
            dtRQMTAttribute = RQMT.RQMTAttribute_Get();
            Session["RQMTAttribute_Get"] = dtRQMTAttribute;
        }
        else
        {
            dtRQMTAttribute = (DataTable)Session["RQMTAttribute_Get"];
        }

        DataTable dt = new DataTable();
        dt = LoadData();

        matchAORSRWebSystems = SR.GetAORWebSystemsForWTSSystem(SYSTEM_ID);
        if (matchAORSRWebSystems.ToLower().IndexOf("wts") != -1)
        {
            matchAORSRWebSystems += ",wts"; // the aorsr forms of wts do not match the internal form, so we ensure that we get WTS results out of R&D WTS and other forms whenever a wts variant is present
        }

        grdData.DataSource = dt;
        grdData.DataBind();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["RQMT_ID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["RQMT_ID"]))
        {
            int.TryParse(Request.QueryString["RQMT_ID"], out this.RQMT_ID);
        }
        if (Request.QueryString["SYSTEM_ID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SYSTEM_ID"]))
        {
            int.TryParse(Request.QueryString["SYSTEM_ID"], out this.SYSTEM_ID);
        }
    }

    private void initControls()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;
    }
    #endregion

    #region "Load Data"
    private DataTable LoadData()
    {
        dtSystem = MasterData.WTS_System_Get(SYSTEM_ID);
        systemName = dtSystem.Rows[0]["WTS_System"].ToString();

        DataSet ds = RQMT.RQMTDefectsImpact_Get(intRQMT_ID: RQMT_ID, intSYSTEM_ID: SYSTEM_ID);
        DataTable dt = ds.Tables["Data"];
        DataTable dtSR = ds.Tables["SR"];
        dtTasks = ds.Tables["Tasks"];

        theDT = dt;
        dtSRs = dtSR;
        dtSRs_COMBINED = dtSR.CombineRowsOnColumn("TaskData", ",", false);

        for (var i = dt.Rows.Count - 1; i >= 0; i--)
        {
            DataRow row = dt.Rows[i];
            DataRow srRow = dt.NewRow();            
            srRow.ItemArray = row.ItemArray.Clone() as object[];
            srRow["Description"] = "SRROW";
            dt.Rows.InsertAt(srRow, i + 1);

            DataRow taskRow = dt.NewRow();
            taskRow.ItemArray = row.ItemArray.Clone() as object[];
            taskRow["Description"] = "TASKROW";
            dt.Rows.InsertAt(taskRow, i + 2);
        }
        dt.AcceptChanges();
        
        this.DCC = dt.Columns;
        Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

        InitializeColumnData(ref dt);
        dt.AcceptChanges();

        return dt;
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
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                    case "RQMTSystemDefectID":
                        displayName = "#";
                        blnVisible = true;
                        blnSortable = false;
                        break;
                    case "RQMTSystemID":
                        displayName = "RQMTSystemID";
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "Description":
                        displayName = "Description";
                        blnVisible = true;
                        break;
                    case "Verified":
                        displayName = "VER";
                        blnVisible = true;
                        break;
                    case "Resolved":
                        displayName = "RES";
                        blnVisible = true;
                        break;
                    case "ContinueToReview":
                        displayName = "REV";
                        blnVisible = true;
                        break;
                    case "ImpactID":
                        displayName = "ImpactID";
                        blnVisible = false;
                        break;
                    case "Impact":
                        displayName = "Impact";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "RQMTStage":
                        displayName = "PD2TDR";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "Mitigation":
                        displayName = "Mitigation";
                        blnVisible = true;
                        blnSortable = false;
                        break;
                    case "SR":
                        displayName = "SR";
                        blnVisible = true;
                        blnSortable = false;
                        break;
                    case "Tasks":
                        displayName = "Tasks";
                        blnVisible = true;
                        blnSortable = false;
                        break;
                    case "CreatedBy":
                        displayName = "Created By";
                        blnVisible = false;
                        break;
                    case "CreatedDate":
                        displayName = "Created Date";
                        blnVisible = false;
                        break;
                    case "UpdatedBy":
                        displayName = "Updated By";
                        blnVisible = false;
                        break;
                    case "UpdatedDate":
                        displayName = "Updated Date";
                        blnVisible = false;
                        break;
                    case "Y":
                        displayName = "&nbsp;";
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

    #region Grid

    void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);
    }

    void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        string itemId = row.Cells[DCC.IndexOf("RQMTSystemDefectID")].Text.Trim();
        //if (itemId == "0" || itemId == "&nbsp;")
        //{
        //    row.Style["display"] = "none";
        //}
        row.Attributes.Add("itemID", itemId);

        string desc = row.Cells[DCC.IndexOf("Description")].Text;
        bool srRow = desc == "SRROW";
        bool taskRow = desc == "TASKROW";

        if (srRow)
        {
            row.Attributes.Add("srcontainerrow", "1");
            row.Style["display"] = "none";            
            row.Style["background-color"] = "white";
            row.Attributes.Add("nohighlight", "true");

            TableCell descCell = row.Cells[DCC["Description"].Ordinal];

            row.Cells[DCC.IndexOf("X")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("RQMTSystemDefectID")].Style["display"] = "none";            
            row.Cells[DCC.IndexOf("Verified")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Resolved")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("ContinueToReview")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Impact")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("RQMTStage")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("SR")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Tasks")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Mitigation")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Y")].Style["display"] = "none";

            descCell.Text = lastSRTable;
            descCell.Style["padding-left"] = "20px";
            descCell.Style["background-color"] = "white";
            descCell.ColumnSpan = 12;
        }
        else if (taskRow)
        {
            row.Attributes.Add("taskcontainerrow", "1");
            row.Style["display"] = "none";            
            row.Style["background-color"] = "white";
            row.Attributes.Add("nohighlight", "true");

            TableCell descCell = row.Cells[DCC["Description"].Ordinal];

            row.Cells[DCC.IndexOf("X")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("RQMTSystemDefectID")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Verified")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Resolved")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("ContinueToReview")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Impact")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("RQMTStage")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("SR")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Tasks")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Mitigation")].Style["display"] = "none";
            row.Cells[DCC.IndexOf("Y")].Style["display"] = "none";

            descCell.Text = lastTaskTable;
            descCell.Style["padding-left"] = "20px";
            descCell.Style["background-color"] = "white";
            descCell.ColumnSpan = 12;
        }
        else
        {
            row.Cells[DCC["X"].Ordinal].Controls.Add(CreateGridDeleteButton(itemId));

            row.Cells[DCC["Description"].Ordinal].Controls.Add(CreateGridTextBox("Description", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Description")].Text.Trim()), false, true));
            if (row.Cells[DCC.IndexOf("CreatedBy")].Text != "&nbsp;")
            {
                Literal lit = new Literal();
                lit.Text = "<span dateinfo=\"1\" style=\"font-size:smaller;\"><br />cr: " + row.Cells[DCC.IndexOf("CreatedBy")].Text + " - " + row.Cells[DCC.IndexOf("CreatedDate")].Text + "&nbsp;&nbsp;&nbsp;up: " + row.Cells[DCC.IndexOf("UpdatedBy")].Text + " - " + row.Cells[DCC.IndexOf("UpdatedDate")].Text + "</span>";
                row.Cells[DCC["Description"].Ordinal].Controls.Add(lit);
            }

            //Create Checkbox for Verified, Resolved, Continue To Review
            if (DCC.Contains("Verified"))
            {
                row.Cells[DCC.IndexOf("Verified")].Controls.Add(CreateCheckBox("Verified", row.Cells[DCC.IndexOf("Verified")].Text, itemId));
            }
            if (DCC.Contains("Resolved"))
            {
                row.Cells[DCC.IndexOf("Resolved")].Controls.Add(CreateCheckBox("Resolved", row.Cells[DCC.IndexOf("Resolved")].Text, itemId));
            }
            if (DCC.Contains("ContinueToReview"))
            {
                row.Cells[DCC.IndexOf("ContinueToReview")].Controls.Add(CreateCheckBox("ContinueToReview", row.Cells[DCC.IndexOf("ContinueToReview")].Text, itemId));
            }

            //Create dropdown for Impact
            if (DCC.Contains("ImpactID"))
            {
                row.Cells[DCC.IndexOf("Impact")].Controls.Add(CreateDropDownList(dtRQMTAttribute, "Impact", "RQMTAttribute", "RQMTAttributeID", row.Cells[DCC.IndexOf("ImpactID")].Text, 100, itemId, "RQMTSystemDefect"));
            }

            if (DCC.Contains("RQMTStageID"))
            {
                row.Cells[DCC.IndexOf("RQMTStage")].Controls.Add(CreateDropDownList(dtRQMTAttribute, "RQMT Stage", "RQMTAttribute", "RQMTAttributeID", row.Cells[DCC.IndexOf("RQMTStageID")].Text, 100, itemId, "RQMTSystemDefect"));
            }

            if (DCC.Contains("SR"))
            {
                TableCell srCell = row.Cells[DCC["SR"].Ordinal];
                srCell.Attributes["itemid"] = itemId;
                srCell.Attributes["srcontainerlinkcell"] = "1";

                StringBuilder strSRNumbers = new StringBuilder();
                StringBuilder strSRTable = new StringBuilder();

                strSRTable.Append("<table cellpadding=\"1\" cellspacing=\"0\" border=\"1\" style=\"width:90%;border:1px solid #cccccc;border-collapse:collapse;\">");
                strSRTable.Append("<tr>");
                strSRTable.Append("<td colspan=\"9\" style=\"text-align:right;border-top:1px double #9095b8;border-left:1px double #9095b8;border-right:1px double #9095b8;background-color:#9095b8;line-height:20px;\"  itemid=\"" + itemId + "\">");
                strSRTable.Append("<div style=\"float:left;text-align:left;cursor:pointer;color:white;font-weight:bold;width:90%;\" itemid=\"" + itemId + "\" onclick=\"srHeaderCellClicked(this)\">SRs for Defect #" + itemId + " <span style=\"font-size:smaller\">(<u>COLLAPSE</u>)</span></div>");
                strSRTable.Append("<div style=\"float:right;width:10%;\"><input type=\"button\" value=\"ADD SR\" addsrbutton=\"1\" itemid=\"" + itemId + "\"></div>");
                strSRTable.Append("</td>");
                strSRTable.Append("</tr>");
                strSRTable.Append("<tr>");
                strSRTable.Append("<td style=\"width:15px;text-align:center;background-color:#dddddd;font-weight:bold;\"></td>");
                strSRTable.Append("<td style=\"width:50px;text-align:center;background-color:#dddddd;font-weight:bold;\">SR #</td>");
                strSRTable.Append("<td style=\"width:50px;text-align:center;background-color:#dddddd;font-weight:bold;\">Rank</td>");
                strSRTable.Append("<td style=\"width:50px;text-align:center;background-color:#dddddd;font-weight:bold;\">Status</td>");
                strSRTable.Append("<td style=\"width:50px;text-align:center;background-color:#dddddd;font-weight:bold;\">Reasoning</td>");
                strSRTable.Append("<td style=\"width:50px;text-align:center;background-color:#dddddd;font-weight:bold;\">Priority</td>");
                strSRTable.Append("<td style=\"width:75px;text-align:center;background-color:#dddddd;font-weight:bold;\">System</td>");
                strSRTable.Append("<td style=\"width:500px;text-align:left;background-color:#dddddd;font-weight:bold;\">Description</td>");
                strSRTable.Append("<td style=\"width:75px;text-align:center;background-color:#dddddd;font-weight:bold;\">Work Task</td>");
                strSRTable.Append("</tr>");

                if (itemId != "&nbsp;")
                {
                    int id = Int32.Parse(itemId);

                    foreach (DataRow srrow in dtSRs_COMBINED.Rows)
                    {
                        if (id == (int)srrow["RQMTSystemDefectID"])
                        {
                            int srid = srrow["SRID"] != DBNull.Value ? (int)srrow["SRID"] : 0;
                            int aorsrid = srrow["AORSR_SRID"] != DBNull.Value ? (int)srrow["AORSR_SRID"] : 0;
                            int rsdsrid = (int)srrow["RQMTSystemDefectSRID"];
                            string taskData = srrow["TaskData"].ToString();
                            string[] taskDataArr = null;
                            if (!string.IsNullOrWhiteSpace(taskData))
                            {
                                taskDataArr = taskData.Split(',');
                            }

                            strSRNumbers.Append("<div style=\"color:blue\">" + (srid > 0 ? srid : aorsrid) + "</div>");

                            strSRTable.Append("<tr>");
                            strSRTable.Append("<td style=\"text-align:center;\"><img imgsrdelete=\"1\" rsdsrid=\"" + rsdsrid + "\" rsdid=\"" + id + "\" srid=\"" + srid + "\" aorsrid=\"" + aorsrid + "\" src=\"images/icons/delete.png\" width=\"12\" height=\"12\" style=\"cursor:pointer;\"></td>");
                            strSRTable.Append("<td style=\"text-align:center;cursor:pointer;text-decoration:underline;color:blue;\" onclick=\"SRNumber_Clicked(" + (srid > 0 ? srid : aorsrid) + ")\">" + (srid > 0 ? srid : aorsrid) + "</td>");
                            strSRTable.Append("<td style=\"text-align:center;\">" + srrow["SRRank"].ToString() + "</td>");
                            strSRTable.Append("<td style=\"text-align:center;\">" + srrow["Status"].ToString() + "</td>");
                            strSRTable.Append("<td style=\"text-align:center;\">" + srrow["Reasoning"].ToString() + "</td>");
                            strSRTable.Append("<td style=\"text-align:center;\">" + srrow["User's Priority"].ToString() + "</td>");
                            strSRTable.Append("<td style=\"text-align:center;\">" + srrow["System"].ToString() + "</td>");
                            strSRTable.Append("<td style=\"text-align:left;width:500px;white-space:normal;\">" + StringUtil.StripHTML(HttpUtility.UrlDecode(srrow["Description"].ToString())) + " <span style=\"font-size:smaller;color:#777777;\">" + srrow["Submitted By"].ToString() + " " + ((DateTime)srrow["Submitted Date"]).ToString("MM/dd/yyyy hh:mm tt") + "</span></td>");
                            strSRTable.Append("<td style=\"text-align:center;\">");
                            if (taskDataArr != null && taskDataArr.Length > 0)
                            {
                                for (int i = 0; i < taskDataArr.Length; i++)
                                {
                                    string[] taskArr = taskDataArr[i].Split('-'); // workitemid-workitemtaskid-workitemnumber                                                                       

                                    if (i > 0) strSRTable.Append("<br />");
                                    strSRTable.Append("<span style=\"text-decoration:underline;color:blue;cursor:pointer;\" onclick=\"openTask(" + taskArr[0] + ", " + taskArr[1] + ", " + taskArr[2] + ")\">").Append(taskArr[0]).Append("-").Append(taskArr[2]).Append("</span>");
                                }
                            }

                            strSRTable.Append("</td>");
                            strSRTable.Append("</tr>");
                        }
                    }
                }

                if (strSRNumbers.Length > 0)
                {
                    srCell.Style["text-decoration"] = "underline";
                    srCell.Style["cursor"] = "pointer";

                    srCell.Text = strSRNumbers.ToString();
                }
                else
                {
                    if (itemId != "&nbsp;")
                    {
                        srCell.Style["text-decoration"] = "underline";
                        srCell.Style["cursor"] = "pointer";

                        srCell.Text = "ADD";
                    }
                    else
                    {
                        srCell.Text = "NONE";
                        srCell.Attributes["srcontainerlinkcell"] = "0";
                        srCell.Style["color"] = "#999999";
                    }

                    strSRTable.Append("<tr><td style=\"text-align:left;\" colspan=\"" + (strSRTable.ToString().Split(new string[] { "</td>" }, StringSplitOptions.None).Length - 1) + "\">NO SRs found</td></tr>");
                }

                strSRTable.Append("</table><br />");

                lastSRTable = strSRTable.ToString();
            }

            if (DCC.Contains("Tasks"))
            {
                TableCell taskCell = row.Cells[DCC["Tasks"].Ordinal];
                taskCell.Attributes["itemid"] = itemId;
                taskCell.Attributes["taskcontainerlinkcell"] = "1";

                StringBuilder strTaskNumbers = new StringBuilder();
                StringBuilder strTaskTable = new StringBuilder();

                strTaskTable.Append("<table cellpadding=\"1\" cellspacing=\"0\" border=\"1\" style=\"width:90%;border:1px solid #cccccc;border-collapse:collapse;\">");
                strTaskTable.Append("<tr>");
                strTaskTable.Append("  <td colspan=\"9\" style=\"text-align:right;border-top:1px double #9095b8;border-left:1px double #9095b8;border-right:1px double #9095b8;background-color:#9095b8;line-height:20px;\"  itemid=\"" + itemId + "\">");
                strTaskTable.Append("    <div style=\"float:left;text-align:left;cursor:pointer;color:white;font-weight:bold;width:90%;\" itemid=\"" + itemId + "\" onclick=\"taskHeaderCellClicked(this)\">Tasks for Defect #" + itemId + " <span style=\"font-size:smaller\">(<u>COLLAPSE</u>)</span></div>");
                strTaskTable.Append("    <div style=\"float:right;width:10%;\"><input type=\"button\" value=\"ADD TASK\" addtaskbutton=\"1\" itemid=\"" + itemId + "\"></div>");
                strTaskTable.Append("  </td>");
                strTaskTable.Append("</tr>");
                strTaskTable.Append("<tr>");
                strTaskTable.Append("  <td style=\"width:15px;text-align:center;background-color:#dddddd;font-weight:bold;\"></td>");
                strTaskTable.Append("  <td style=\"width:100px;text-align:center;background-color:#dddddd;font-weight:bold;\">Work Task</td>");
                strTaskTable.Append("  <td style=\"width:250px;text-align:left;background-color:#dddddd;font-weight:bold;\">Title</td>");
                strTaskTable.Append("  <td style=\"width:150px;text-align:center;background-color:#dddddd;font-weight:bold;\">Assigned To</td>");
                strTaskTable.Append("  <td style=\"width:75px;text-align:center;background-color:#dddddd;font-weight:bold;\">% Comp</td>");
                strTaskTable.Append("  <td style=\"width:125px;text-align:center;background-color:#dddddd;font-weight:bold;\">Status</td>");
                strTaskTable.Append("</tr>");

                if (itemId != "&nbsp;")
                {
                    int id = Int32.Parse(itemId);

                    foreach (DataRow taskrow in dtTasks.Rows)
                    {
                        if (id == (int)taskrow["RQMTSystemDefectID"])
                        {
                            int rsdtaskid = (int)taskrow["RQMTSystemDefectTaskID"];
                            int wid = (int)taskrow["WORKITEMID"];
                            int tn = (int)taskrow["TASK_NUMBER"];
                            int witid = (int)taskrow["WORKITEM_TASKID"];

                            strTaskNumbers.Append("<div style=\"color:blue\">" + wid + "-" + tn + "</div>");

                            strTaskTable.Append("<tr>");
                            strTaskTable.Append("  <td class=\"gridBodyFullBorder\" style=\"text-align:center;\"><img imgtaskdelete=\"1\" rsdid=\"" + id + "\" rsdtaskid=\"" + rsdtaskid + "\" witid=\"" + witid + "\" wid=\"" + wid + "\" tn=\"" + tn + "\" src=\"images/icons/delete.png\" width=\"12\" height=\"12\" style=\"cursor:pointer;\"></td>");
                            strTaskTable.Append("  <td class=\"gridBodyFullBorder\" style=\"text-align:center;\"><span style=\"text-decoration:underline;color:blue;cursor:pointer;\" onclick=\"openTask(" + wid + ", " + witid + ", " + tn + ")\">" + wid + "-" + tn + "</span></td>");
                            strTaskTable.Append("  <td class=\"gridBodyFullBorder\" style=\"text-align:left;\">" + HttpUtility.HtmlEncode(taskrow["TITLE"].ToString()) + "</td>");
                            strTaskTable.Append("  <td class=\"gridBodyFullBorder\" style=\"text-align:center;\">" + taskrow["USERNAME"].ToString() + "</td>");
                            strTaskTable.Append("  <td class=\"gridBodyFullBorder\" style=\"text-align:center;\">" + taskrow["COMPLETIONPERCENT"].ToString() + "</td>");
                            strTaskTable.Append("  <td class=\"gridBodyFullBorder\" style=\"text-align:center;\">" + taskrow["STATUS"].ToString() + "</td>");
                            strTaskTable.Append("</tr>");
                        }
                    }
                }

                if (strTaskNumbers.Length > 0)
                {
                    taskCell.Style["text-decoration"] = "underline";
                    taskCell.Style["cursor"] = "pointer";

                    taskCell.Text = strTaskNumbers.ToString();
                }
                else
                {
                    if (itemId != "&nbsp;")
                    {
                        taskCell.Style["text-decoration"] = "underline";
                        taskCell.Style["cursor"] = "pointer";

                        taskCell.Text = "ADD";
                    }
                    else
                    {
                        taskCell.Text = "NONE";
                        taskCell.Attributes["taskcontainerlinkcell"] = "0";
                        taskCell.Style["color"] = "#999999";
                    }

                    strTaskTable.Append("<tr><td style=\"text-align:left;\" colspan=\"" + (strTaskTable.ToString().Split(new string[] { "</td>" }, StringSplitOptions.None).Length - 1) + "\">NO tasks found</td></tr>");
                }

                strTaskTable.Append("</table><br />");

                lastTaskTable = strTaskTable.ToString();
            }

            row.Cells[DCC["Mitigation"].Ordinal].Controls.Add(CreateGridTextBox("Mitigation", itemId, Server.HtmlDecode(row.Cells[DCC.IndexOf("Mitigation")].Text.Trim()), false, true));
        }
    }

    void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdData.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtDefectsImpact"] == null)
        {
            LoadData();
        }
        else
        {
            grdData.DataSource = (DataTable)HttpContext.Current.Session["dtDefectsImpact"];
        }
    }

    void formatColumnDisplay(ref GridViewRow row)
    {
        row.Cells[DCC.IndexOf("Verified")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Verified")].Style["width"] = "35px";

        row.Cells[DCC.IndexOf("Resolved")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Resolved")].Style["width"] = "35px";

        row.Cells[DCC.IndexOf("ContinueToReview")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("ContinueToReview")].Style["width"] = "35px";

        row.Cells[DCC.IndexOf("SR")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("SR")].Style["width"] = "50px";

        row.Cells[DCC.IndexOf("Tasks")].Style["text-align"] = "center";
        row.Cells[DCC.IndexOf("Tasks")].Style["width"] = "60px";

        foreach (TableCell cell in row.Cells)
        {
            cell.Style["vertical-align"] = "top";
        }

        //for (int i = 0; i < row.Cells.Count; i++)
        //{
        //    if (i != DCC.IndexOf("X")
        //        && i != DCC.IndexOf("PriorityID")
        //        && i != DCC.IndexOf("WorkRequest_Count")
        //        && i != DCC.IndexOf("WorkItem_Count")
        //        && i != DCC.IndexOf("SORT_ORDER")
        //        && i != DCC.IndexOf("ARCHIVE"))
        //    {
        //        row.Cells[i].Style["text-align"] = "left";
        //        row.Cells[i].Style["padding-left"] = "5px";
        //    }
        //    else
        //    {
        //        row.Cells[i].Style["text-align"] = "center";
        //        row.Cells[i].Style["padding-left"] = "0px";
        //    }
        //}

        ////more column formatting
        //row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
        //row.Cells[DCC.IndexOf("PriorityType")].Style["width"] = "95px";
        //row.Cells[DCC.IndexOf("Priority")].Style["width"] = "75px";
        //row.Cells[DCC.IndexOf("WorkRequest_Count")].Style["width"] = "75px";
        //row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "75px";
        //row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "75px";
        //row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
    }
    Image CreateGridDeleteButton(string itemId)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgDelete_click('{0}');return false;", itemId);

        string imgUrl = "";
        imgUrl += "Images/Icons/delete.png";

        Image imgDelete = new Image();
        imgDelete.Style["cursor"] = "pointer";
        imgDelete.Height = 12;
        imgDelete.Width = 12;
        imgDelete.ImageUrl = imgUrl;
        imgDelete.ID = string.Format("imgDelete_{0}", itemId);
        imgDelete.Attributes["name"] = string.Format("imgDelete_{0}", itemId);
        imgDelete.Attributes.Add("itemId", itemId.ToString());
        imgDelete.AlternateText = "Delete Item";
        imgDelete.Attributes.Add("Onclick", sb.ToString());

        return imgDelete;
    }
    LinkButton createEditLink(string itemId = "", string item = "")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("lbEdit_click('{0}');return false;", itemId);

        LinkButton lb = new LinkButton();
        lb.ID = string.Format("lbEdit_{0}", itemId);
        lb.Attributes["name"] = string.Format("lbEdit_{0}", itemId);
        lb.ToolTip = string.Format("Edit Item [{0}]", item);
        lb.Text = item;
        lb.Attributes.Add("Onclick", sb.ToString());

        return lb;
    }
    TextBox CreateGridTextBox(string field, string itemId, string text, bool isNumber = false, bool multiLine = false)
    {
        TextBox txt = new TextBox();

        if (multiLine)
        {
            txt.Wrap = true;
            txt.TextMode = TextBoxMode.MultiLine;
        }

        txt.ID = string.Format("txt{0}_{1}", field.Trim().Replace(" ", ""), itemId);
        txt.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(text));
        txt.Attributes.Add("field", field);
        txt.Attributes.Add("RQMTSystemDefectID", itemId);
        txt.Attributes.Add("original_value", text);
        //txt.Attributes.Add("onblur", "txtDescr_onblur(this);");
        txt.Attributes["name"] = txt.ID;
        txt.Style["width"] = field == "Description" ? "340px" : "255px";
        txt.Style["height"] = "50px";
        txt.Style["background-color"] = "#F5F6CE";
        txt.Style["font-family"] = "arial";
        txt.Style["font-size"] = "12px";
        if (isNumber)
        {
            txt.Style["text-align"] = "right";
            txt.TextMode = TextBoxMode.Number;
            txt.Style["width"] = "90%";
        }

        return txt;
    }
    HtmlInputCheckBox CreateCheckBox(string columnName, string columnValue, string rqmtSystemDefectID = "")
    {
        HtmlInputCheckBox chk = new HtmlInputCheckBox();

        chk.Attributes["columnName"] = columnName;
        chk.Attributes["RQMTSystemDefectID"] = rqmtSystemDefectID;
        //chk.Attributes["onchange"] = "input_change(this);";
        chk.Attributes["field"] = columnName;
        var isChecked = false;
        if (columnValue != "")
        {
            if ((Convert.ToInt16(columnValue) == 1)) { isChecked = true; }
            chk.Checked = isChecked;
        }
        return chk;
    }

    private DropDownList CreateDropDownList(DataTable dt, string field, string textColumn, string valueColumn, string selectedValue, int maxWidth, string rqmtSystemDefectID = "",
        string typeName = "", bool allowBlankValue = true, string onChange = "")
    {
        DropDownList ddl = new DropDownList();
        ddl.Attributes.Add("field", field);
        ddl.Attributes.Add("typeID", valueColumn);
        ddl.Attributes.Add("typeName", typeName);

        ddl.Attributes.Add("RQMTSystemDefectID", rqmtSystemDefectID);

        ListItem liBlank = new ListItem();
        liBlank.Text = "";
        liBlank.Value = "-1";
        ddl.Items.Add(liBlank);

        if (maxWidth > 0)
        {
            ddl.Style["width"] = maxWidth + "px";
        }

        dt.DefaultView.RowFilter = "RQMTAttributeType = '" + field + "'";
        dt = dt.DefaultView.ToTable();

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

        return ddl;
    }
    #endregion Grid

    [WebMethod()]
    public static string SRSelected(int rsdid, int srid, int srext)
    {
        var result = WTS.WTSPage.CreateDefaultResult();

        if (RQMT.RQMTDefectsImpactSR_Add(rsdid, srext == 0 ? srid : 0, srext == 1 ? srid : 0))
        {
            result["success"] = "true";
        }

        return WTS.WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string SRDelete(int rsdsrid)
    {
        var result = WTS.WTSPage.CreateDefaultResult();

        if (RQMT.RQMTDefectsImpactSR_Delete(rsdsrid))
        {
            result["success"] = "true";
        }

        return WTS.WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string TaskAddedToDefect(int rsdid, int WORKITEM_TASKID)
    {
        var result = WTS.WTSPage.CreateDefaultResult();

        if (RQMT.RQMTDefectsImpactTask_Add(rsdid, WORKITEM_TASKID))
        {
            result["success"] = "true";
        }

        return WTS.WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string DeleteTaskFromDefect(int rsdtaskid)
    {
        var result = WTS.WTSPage.CreateDefaultResult();

        if (RQMT.RQMTDefectsImpactTask_Delete(rsdtaskid))
        {
            result["success"] = "true";
        }

        return WTS.WTSPage.SerializeResult(result);
    }
}