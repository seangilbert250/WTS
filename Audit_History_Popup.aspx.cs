using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using WTS;
using WTS.Auditing;

public partial class Audit_History_Popup : WTSPage
{
    #region Variables
    protected string ViewType = string.Empty;
    protected int ItemID = 0;
    protected int ParentItemID = 0;
    protected DateTime asOfDate = DateTime.Now;
    protected DateTime asOfDateExclusive = DateTime.Now.AddDays(1.0); // this is meant to be the next day at midnight so we can do a "less than" check on updated dates and get stuff from today

    private DataColumnCollection DCC;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        LoadHistory();
    }

    private void ReadQueryString()
    {
        if (!string.IsNullOrWhiteSpace(Request.QueryString["viewtype"]))
        {
            ViewType = Request.QueryString["viewtype"].ToLower();
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["itemid"]))
        {
            ItemID = Int32.Parse(Request.QueryString["itemid"]);
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["parentitemid"]))
        {
            ParentItemID = Int32.Parse(Request.QueryString["parentitemid"]);
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["asofdate"]))
        {
            if (!DateTime.TryParse(WTS.Util.StringUtil.UndoStrongEscape(Request.QueryString["asofdate"].Trim()), out asOfDate))
            {
                asOfDate = DateTime.Now;
            }
        }

        asOfDateExclusive = new DateTime(asOfDate.Year, asOfDate.Month, asOfDate.Day);
        asOfDateExclusive = asOfDateExclusive.AddDays(1.0);
    }

    #endregion

    #region Data
    private void LoadHistory()
    {
        if (ViewType == "rqmtset")
        {
            FillRQMTSetHistory();
        }
        else if (ViewType == "rqmt")
        {
            FillRQMTHistory();
        }
    }
    #endregion

    #region RQMTSet
    private void FillRQMTSetHistory()
    {
        DataTable dthst = Auditing.AuditHistory_Get((int)AuditLogTypeEnum.RQMTSet, ItemID, 0, asOfDateExclusive);
        DataRow drrs = RQMT.RQMTSet_Get(ItemID, true);

        phAuditHistory.Controls.Add(CreateSection("Associations", CreatePropertySections("associations",
                new string[] { "RQMT Set Name", "System", "Work Area", "Purpose" }, 
                new string[] { (string)drrs["RQMTSetName"], (string)drrs["WTS_SYSTEM"], (string)drrs["WorkArea"], (string)drrs["RQMTType"] },
                new string[] { GetPreviousValues(dthst, "RQMTSetName", asOfDateExclusive), GetPreviousValues(dthst, "WTS_SYSTEM", asOfDateExclusive), GetPreviousValues(dthst, "WorkArea", asOfDateExclusive), GetPreviousValues(dthst, "RQMTType", asOfDateExclusive) }
        )));

        phAuditHistory.Controls.Add(CreateSection("Properties", CreatePropertySections("properties",
                new string[] { "Complexity", "Justification" },
                new string[] { (drrs["Complexity"] != DBNull.Value ? (string)drrs["Complexity"] : ""), (drrs["Justification"] != DBNull.Value ? (string)drrs["Justification"] : "") },
                new string[] { GetPreviousValues(dthst, "RQMTComplexity", asOfDateExclusive), GetPreviousValues(dthst, "Justification", asOfDateExclusive) }
        )));

        if (dthst.AsEnumerable().Count(dr => dr["FieldChanged"].ToString() == "RQMT") > 0)
        {
            string rqmtstr = CreateTableSection("rqmts", "RQMT", null, dthst,
                new string[] { "RQMT #", "RQMT", "ACTION", "UPDATED", "UPDATED BY" },
                new string[] { "ParentItemID", "OldValue|NewValue", "ITEM_UPDATETYPEID", "UpdatedDate", "UpdatedBy" });

            int ht = 15 + (12 * dthst.Rows.Count);
            if (ht > 100) ht = 100;
            phAuditHistory.Controls.Add(CreateSection("RQMTS", rqmtstr, ht));
        }        

        string countStr = CreatePropertySections("rqmtcount",
                new string[] { "RQMT Count" },
                new string[] { drrs["RQMTCount"].ToString() },
                new string[] { GetPreviousValues(dthst, "RQMTCount", asOfDate) });

        countStr += AddRQMTSetCountChart(dthst, drrs["RQMTCount"].ToString());

        phAuditHistory.Controls.Add(CreateSection("RQMT Count", countStr));
    }

    private string AddRQMTSetCountChart(DataTable dthst, string currentCount)
    {
        StringBuilder str = new StringBuilder();
        str.Append("<canvas id=\"cvrqmtcount\" width=\"600\" height=\"100\" style=\"margin-top:10px;\"></canvas>");

        List<string> counts = new List<string>();
        List<string> tooltips = new List<string>();

        if (dthst.Rows.Count > 0)
        {
            foreach (DataRow row in dthst.Rows)
            {
                if (row["FieldChanged"].ToString() == "RQMTCount")
                {
                    string newValue = row["NewValue"].ToString();
                    counts.Insert(0, newValue);

                    DateTime ud = (DateTime)row["UpdatedDate"];
                    tooltips.Insert(0, "'" + ud.ToString("MM/dd/yyyy hh:mm tt") + "'");
                }
            }
        }
        else
        {
            counts.Add(currentCount);
            tooltips.Add("'" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "'");
        }
        
        str.Append("<script type=\"text/javascript\">$(document).ready(function () { var opt = []; opt['datasetname'] = 'RQMT Count'; opt['values'] = [" + string.Join(",", counts) + "]; opt['valuelabels'] = [" + string.Join(",", tooltips) + "]; opt['backgroundcolor'] = '#408bd1'; opt['bordercolor'] = '#1b3b59'; var data = createBarChartDataSet(opt); opt = []; opt['displayxaxisticks'] = 'false'; var options = createDefaultChartOptions(opt); addChartToCanvas('cvrqmtcount', 'bar', data, options); });</script>");

        return str.ToString();
    }

    #endregion

    #region RQMT
    private void FillRQMTHistory()
    {
        DataTable dthst = Auditing.AuditHistory_Get((int)AuditLogTypeEnum.RQMT, ItemID, 0, asOfDateExclusive);

        DataSet dsRQMT = RQMT.RQMTEditData_Get(ItemID);

        DataTable dtRQMT = dsRQMT.Tables["RQMT"];
        DataTable dtAllSets = dsRQMT.Tables["ALLSETS"];
        DataTable dtAssociations = dsRQMT.Tables["ASSOCIATIONS"];
        DataTable dtAttributes = dsRQMT.Tables["ATTRIBUTES"];
        DataTable dtUsage = dsRQMT.Tables["USAGE"];
        DataTable dtAvailableFunctionalities = dsRQMT.Tables["AVAILABLEFUNCTIONALITIES"];
        DataTable dtFunctionality = dsRQMT.Tables["FUNCTIONALITY"];
        DataTable dtDescriptions = dsRQMT.Tables["DESCRIPTIONS"];
        DataTable dtDescriptionTypes = dsRQMT.Tables["DESCTYPES"];
        DataTable dtDefects = dsRQMT.Tables["DEFECTS"];

        string detailsSection = CreatePropertySections("details", 
            new string[] { "RQMT Name" }, 
            new string[] { dtRQMT.Rows[0]["RQMT"].ToString() },
            new string[] { GetPreviousValues(dthst, "RQMT", asOfDateExclusive) });
        phAuditHistory.Controls.Add(CreateSection("Details", detailsSection));

        string associationsHistory = CreateTableSection("associations", "RQMTID", "SET", dthst,
            new string[] { "ACTION", "UPDATED", "UPDATED BY" },
            new string[] { "NewValue", "UpdatedDate", "UpdatedBy" },
            30, 250);

        string currentAssocations = "<div style=\"padding-bottom:10px;padding-left:15px;position:relative;\">";
        if (string.IsNullOrWhiteSpace(associationsHistory))
        {
            currentAssocations += "<img src=\"images/icons/expand.gif\" width=\"9\" height=\"9\" style=\"position:absolute;left:2px;top:2px;opacity:.3\" alt=\"No history found\" title=\"No history found\">";
        }
        else
        {
            currentAssocations += "<img src=\"images/icons/expand.gif\" width=\"9\" height=\"9\" style=\"position:absolute;left:2px;top:2px;cursor:pointer;\" onclick=\"$('#div_associations_history').toggle(); $(this).attr('src', $('#div_associations_history').is(':visible') ? 'images/icons/collapse.gif' : 'images/icons/expand.gif')\">";

            associationsHistory = associationsHistory.Replace("RQMT ADDED TO SET", "<img src=\"images/icons/check.png\" width=\"12\" height=\"12\" alt=\"Added to set\" title=\"Added to set\">&nbsp;");
            associationsHistory = associationsHistory.Replace("RQMT DELETED FROM SET", "<img src=\"images/icons/cross.png\" width=\"12\" height=\"12\" alt=\"Deleted from set\" title=\"Deleted from set\">&nbsp;");
        }

        bool rowAdded = false;
        foreach (DataRow row in dtAssociations.Rows)
        {
            if (rowAdded)
            {
                currentAssocations += "<br />";
            }

            string rsid = row["RQMTSet_ID"].ToString();
            string sys = row["WTS_SYSTEM"].ToString();
            string wa = row["WorkArea"].ToString();
            string rt = row["RQMTType"].ToString();

            currentAssocations += "<b>" + sys + "/" + wa + "/" + rt + " (" + rsid + ")</b>";

            rowAdded = true;
        }
        currentAssocations += "</div>";
        currentAssocations += "<div id=\"div_associations_history\" style=\"display:none;\">";
        currentAssocations += associationsHistory;
        currentAssocations += "</div>";

        phAuditHistory.Controls.Add(CreateSection("Associations", currentAssocations));

    }
    #endregion

    #region Helpers
    private Control CreateSection(string title, string content, int height = 0)
    {
        Literal literal = new Literal();

        StringBuilder str = new StringBuilder();

        str.Append("<div class=\"gridHeader gridFullBorder\" style=\"width:100%;font-size:larger;font-weight:bold;margin-bottom:10px;\">");
        str.Append(title);
        str.Append("</div>");

        str.Append("<div style=\"width:100%;margin-bottom:20px;" + (height > 0 ? "overflow-y:auto;height:" + height + "px;" : "") + "\">");
        str.Append(content);
        str.Append("</div>");

        literal.Text = str.ToString();

        return literal;
    }

    private string CreateTableSection(string section, string historyKey, string newValueSubstringRequired, DataTable dthst, string[] columnHeaders, string[] columns, int heightPerRow = 0, int maxHeight = 0)
    {
        StringBuilder str = new StringBuilder();

        List<DataRow> historyRows = new List<DataRow>();

        foreach (DataRow row in dthst.Rows)
        {
            if (row["FieldChanged"].ToString() == historyKey && (string.IsNullOrWhiteSpace(newValueSubstringRequired) || (row["NewValue"] != DBNull.Value && row["NewValue"].ToString().Contains(newValueSubstringRequired))))
            {
                historyRows.Add(row);
            }
        }               
        
        if (historyRows.Count > 0)
        {
            int height = 0;

            if (heightPerRow > 0)
            {
                height = historyRows.Count * heightPerRow;

                if (maxHeight > 0 && height > maxHeight)
                {
                    height = maxHeight;
                }
            }

            str.Append("<div style=\"" + (height > 0 ? "height:" + height + "px;overflow-y:auto;" : "") + "\">");
            str.Append("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" style=\"border-collapse:collapse;\">");
            
            bool altRow = false;
            bool addedRow = false;

            foreach (DataRow row in historyRows)
            {
                if (!addedRow)
                {
                    str.Append("<tr>");

                    for (int x = 0; x < columnHeaders.Length; x++)
                    {
                        string header = columnHeaders[x];
                        string headerLower = columnHeaders[x].ToLower();
                        string col = columns[x];
                        string align = "left";
                        if (headerLower.IndexOf("#") != -1)
                        {
                            align = "center";
                        }
                        string width = "300px;";
                        if (headerLower.IndexOf("#") != -1 || header.EndsWith("id"))
                        {
                            width = "75px";
                        }
                        else if (headerLower.IndexOf("updated") != -1 || col == "ITEM_UPDATETYPEID")
                        {
                            width = "100px;";
                        }

                        str.Append("<td style=\"font-weight:bold;text-decoration:underline;background-color:#cccccc;white-space:nowrap;text-align:" + align + ";width:" + width + "\">").Append(header).Append("</td>");
                    }

                    str.Append("</tr>");

                    addedRow = true;
                }

                string backgroundColor = altRow ? "#dddddd" : "#ffffff";

                str.Append("<tr>");

                for (int x = 0; x < columns.Length; x++)
                {
                    string header = columnHeaders[x].ToLower();
                    string col = columns[x];                    
                    string value = null;                    
                    string align = "left";
                    if (header.IndexOf("#") != -1)
                    {
                        align = "center";
                    }
                    string width = "300px;";
                    if (header.IndexOf("#") != -1 || header.EndsWith("id"))
                    {
                        width = "75px";
                    }
                    else if (header.IndexOf("updated") != -1 || col == "ITEM_UPDATETYPEID")
                    {
                        width = "100px;";
                    }

                    if (col.IndexOf("|") != -1) // we are allowing this operator to work like a sql coalesce operator (keep searching cols until a value is found)
                    {
                        string[] colArr = col.Split('|');

                        for (int y = 0; y < colArr.Length; y++)
                        {
                            object obj = row[colArr[y]];

                            if (obj != DBNull.Value && !string.IsNullOrWhiteSpace(obj.ToString()))
                            {
                                value = obj != DBNull.Value ? obj.ToString() : "[NONE]";

                                break;
                            }
                        }
                    }
                    else
                    {
                        object obj = row[col];
                        value = obj != DBNull.Value ? obj.ToString() : "[NONE]";
                    }

                    if (value == null || string.IsNullOrWhiteSpace(value))
                    {
                        value = "[NONE]";
                    }

                    if (col == "ITEM_UPDATETYPEID")
                    {
                        int updateTypeID = Int32.Parse(value);

                        if (updateTypeID == (int)ItemUpdateTypeEnum.Add)
                        {
                            value = "ADD";
                        }
                        else if (updateTypeID == (int)ItemUpdateTypeEnum.Update)
                        {
                            value = "UPDATE";
                        }
                        else if (updateTypeID == (int)ItemUpdateTypeEnum.Delete)
                        {
                            value = "DELETE";
                        }
                    }

                    str.Append("  <td style=\"background-color:" + backgroundColor + ";text-align:" + align + ";width:" + width + ";\">");
                    str.Append(value);
                    str.Append("  </td>");
                }
                str.Append("</tr>");

                altRow = !altRow;
            }
            
            str.Append("</table>");
            str.Append("</div>");
        }      

        return str.ToString();
    }


    private string CreatePropertySections(string section, string[] titles, string[] currentValues, string[] previousValues, bool escapeHTML = true, bool hidePreviousValueColumn = false)
    {
        StringBuilder str = new StringBuilder();

        str.Append("<div style=\"width:100%;position:relative;\">");

        str.Append("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" style=\"border-collapse:collapse;\">");
        str.Append("  <tr>");
        str.Append("    <td style=\"background-color:#cccccc;font-weight:bold;text-decoration:underline;width:35px;white-space:nowrap;text-align:center;vertical-align:top;\"></td>");
        str.Append("    <td style=\"background-color:#cccccc;font-weight:bold;text-decoration:underline;width:125px;white-space:nowrap;text-align:left;vertical-align:top;\">FIELD</td>");
        if (!hidePreviousValueColumn)
        {
            str.Append("    <td style=\"background-color:#cccccc;font-weight:bold;text-decoration:underline;width:300px;text-align:left;vertical-align:top;\">PREVIOUS VALUE</td>");
        }
        str.Append("    <td style=\"background-color:#cccccc;font-weight:bold;text-decoration:underline;width:" + (hidePreviousValueColumn ? "600" : "300") + "px;text-align:left;vertical-align:top;\">CURRENT VALUE</td>");
        str.Append("    <td style=\"background-color:#cccccc;font-weight:bold;text-decoration:underline;width:100px;white-space:nowrap;text-align:left;vertical-align:top;\">UPDATED</td>");
        str.Append("    <td style=\"background-color:#cccccc;font-weight:bold;text-decoration:underline;width:100px;white-space:nowrap;text-align:left;vertical-align:top;\">UPDATED BY</td>");
        str.Append("</tr>");

        for (int i = 0; i < titles.Length; i++)
        {
            string title = titles[i];
            string current = currentValues[i];
            string previous = previousValues != null ? previousValues[i] : "";            
            string lastUpdated = null;
            string lastUpdatedBy = null;
            List<string[]> prevRowsArr = new List<string[]>();

            if (!string.IsNullOrWhiteSpace(previous))
            {
                string[] rows = previous.Split(new string[] { "<rowsep>" }, StringSplitOptions.RemoveEmptyEntries);

                for (int x = 0; x < rows.Length; x++)
                {
                    string rowstr = rows[x];                    
                    string[] rowArr = rowstr.Split(new string[] { "<sep>" }, StringSplitOptions.None);
                    for (int r = 0; r < rowArr.Length; r++)
                    {
                        if (rowArr[r] == "<NOVALUE>")
                        {
                            rowArr[r] = "[NONE]";
                        }
                    }

                    prevRowsArr.Add(rowArr);

                    if (x == 0)
                    {
                        previous = rowArr[0];
                        lastUpdated = rowArr[2];
                        lastUpdatedBy = rowArr[3];
                    }                    
                }
            }

            if (escapeHTML)
            {
                current = string.IsNullOrWhiteSpace(current) ? "[NONE]" : HttpUtility.HtmlEncode(current);
                previous = string.IsNullOrWhiteSpace(previous) ? "[NONE]" : HttpUtility.HtmlEncode(previous);
            }

            string styleExt = "padding-top:5px;" + (i % 2 == 1 ? "background-color:#dddddd;" : "");

            // current/last row
            str.Append("  <tr>");
            if (prevRowsArr.Count > 0)
            {
                str.Append("    <td style=\"" + styleExt + "width:1%;white-space:nowrap;text-align:center;vertical-align:top;\"><img audithistoryrowexpandimg=\"" + section + "_" + i + "\" src=\"images/icons/expand.gif\" width=\"9\" height=\"9\" style=\"cursor:pointer;\" alt=\"Expand History\" title=\"Expand History\" onclick=\"toggleAuditHistoryRow('" + section + "', " + i + ");\"></td>");
            }
            else
            {
                str.Append("    <td style=\"" + styleExt + "width:1%;white-space:nowrap;text-align:center;vertical-align:top;\"><img audithistoryrowexpandimg=\"" + section + "_" + i + "\" src=\"images/icons/expand.gif\" width=\"9\" height=\"9\" style=\"opacity:.3;cursor:pointer;\" alt=\"No History Available\" title=\"No History Available\"></td>");
            }
            str.Append("    <td style=\"" + styleExt + "white-space:nowrap;text-align:left;font-weight:bold;width:100px;vertical-align:top;\">" + title + "</td>");

            if (!hidePreviousValueColumn)
            {
                str.Append("    <td style=\"" + styleExt + "text-align:left;vertical-align:top;\">" + previous + "</td>");
            }
            str.Append("    <td style=\"" + styleExt + "text-align:left;font-weight:bold;\">" + current + "</td>");            
            str.Append("    <td style=\"" + styleExt + "width:1%;white-space:nowrap;text-align:left;vertical-align:top;\">" + (string.IsNullOrWhiteSpace(lastUpdated) ? "" : lastUpdated) + "</td>");
            str.Append("    <td style=\"" + styleExt + "width:1%;white-space:nowrap;text-align:left;vertical-align:top;\">" + (string.IsNullOrWhiteSpace(lastUpdatedBy) ? "" : lastUpdatedBy) + "</td>");
            str.Append("  </tr>");

            // full history row
            for (int x = 0; x < prevRowsArr.Count; x++)
            {
                if (x == 0)
                {
                    str.Append("  <tr audithistoryrowheader=\"" + section + "_" + i + "\" style=\"display:none;border-top:1px solid gray;vertical-align:top;\">");
                    str.Append("    <td style=\"" + styleExt + "white-space:nowrap;text-align:center;vertical-align:top;\"></td>");
                    str.Append("    <td style=\"" + styleExt + "white-space:nowrap;text-align:left;font-weight:bold;vertical-align:top;\"></td>");
                    if (!hidePreviousValueColumn)
                    {
                        str.Append("    <td style=\"" + styleExt + "text-align:left;text-decoration:underline;vertical-align:top;\">OLD VALUE</td>");
                    }
                    str.Append("    <td style=\"" + styleExt + "text-align:left;text-decoration:underline;vertical-align:top;\">NEW VALUE</td>");
                    str.Append("    <td style=\"" + styleExt + "white-space:nowrap;text-align:left;text-decoration:underline;vertical-align:top;\">UPDATED</td>");
                    str.Append("    <td style=\"" + styleExt + "white-space:nowrap;text-align:left;text-decoration:underline;vertical-align:top;\">UPDATED BY</td>");
                    str.Append("  </tr>");
                }

                string[] rowArr = prevRowsArr[x];

                for (int r = 0; r < rowArr.Length; r++)
                {
                    if (rowArr[r] == "<NOVALUE>")
                    {
                        rowArr[r] = "[NONE]";
                    }
                }

                string oldValue = string.IsNullOrWhiteSpace(rowArr[0]) ? "[NONE]" : rowArr[0];
                if (escapeHTML) oldValue = HttpUtility.HtmlEncode(oldValue);

                string newValue = string.IsNullOrWhiteSpace(rowArr[1]) ? "[NONE]" : rowArr[1];
                if (escapeHTML) newValue = HttpUtility.HtmlEncode(newValue);

                str.Append("  <tr audithistoryrow=\"" + section +"_" + i + "\" style=\"display:none;\">");
                str.Append("    <td style=\"" + styleExt + "white-space:nowrap;text-align:center;vertical-align:top;\"></td>");
                str.Append("    <td style=\"" + styleExt + "white-space:nowrap;text-align:left;font-weight:bold;vertical-align:top;\"></td>");
                if (!hidePreviousValueColumn)
                {
                    str.Append("    <td style=\"" + styleExt + "text-align:left;vertical-align:top;\">" + oldValue + "</td>");
                }
                str.Append("    <td style=\"" + styleExt + "text-align:left;vertical-align:top;" + (x == 0 ? "font-weight:bold;" : "") + "\">" + newValue + "</td>");
                str.Append("    <td style=\"" + styleExt + "white-space:nowrap;text-align:left;vertical-align:top;\">" + (string.IsNullOrWhiteSpace(rowArr[2]) ? "" : rowArr[2]) + "</td>");
                str.Append("    <td style=\"" + styleExt + "white-space:nowrap;text-align:left;vertical-align:top;\">" + (string.IsNullOrWhiteSpace(rowArr[2]) ? "" : rowArr[3]) + "</td>");
                str.Append("  </tr>");
            }
        }

        str.Append("</table>");

        str.Append("</div>");

        return str.ToString();
    }

    private string GetPreviousValues(DataTable dthst, string field, DateTime exclusiveDate)
    {
        StringBuilder prev = new StringBuilder();

        // first row is the NEWEST row, last row is the OLDEST
        for (int i = 0; i < dthst.Rows.Count; i++)
        {
            DataRow row = dthst.Rows[i];
            
            if (field == (string)row["FieldChanged"] && (DateTime)row["UpdatedDate"] <= exclusiveDate)
            {
                if (prev.Length > 0)
                {
                    prev.Append("<rowsep>");
                }

                object value = row["OldValue"];

                if (value != DBNull.Value && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    prev.Append(value.ToString());
                }
                else
                {
                    prev.Append("<NOVALUE>");
                }

                prev.Append("<sep>");

                value = row["NewValue"];

                if (value != DBNull.Value && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    prev.Append(value.ToString());
                }
                else
                {
                    prev.Append("<NOVALUE>");
                }

                prev.Append("<sep>");

                prev.Append(((DateTime)row["UpdatedDate"]).ToString("MM/dd/yyyy hh:mm tt"));

                prev.Append("<sep>");

                prev.Append(row["UpdatedBy"].ToString());

            }
        }

        return prev.ToString();
    }

    #endregion
}