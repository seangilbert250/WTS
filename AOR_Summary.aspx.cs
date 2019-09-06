﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;
using System.Linq;
using System.Web;

public partial class AOR_Summary : System.Web.UI.Page
{
    #region Variables
    protected string[] QFRelease = { };
    protected string[] QFDeliverable = { };
    protected string[] QFContract = { };
    protected string QFReleaseGroups = string.Empty;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        if (QFContract.Count() > 0)
        {
            HttpContext.Current.Session.Remove("SelectedContracts");
            foreach (string contractID in QFContract)
            {
                HttpContext.Current.Session["SelectedContracts"] += contractID + ",";
            }
        } else if (Session["SelectedContracts"] != null)
        {
            string str = string.Empty;
            str = Convert.ToString(HttpContext.Current.Session["SelectedContracts"]);
            QFContract = str.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        DataTable dtRelease = MasterData.ProductVersionList_Get(includeArchive: false);
        DataTable dtCurrentRelease = AOR.AORCurrentRelease_Get();
        string currentReleaseID = "0";

        if (dtCurrentRelease != null && dtCurrentRelease.Rows.Count > 0)
        {
            currentReleaseID = dtCurrentRelease.Rows[0]["ProductVersionID"].ToString();
        }

        if (dtRelease != null)
        {
            ddlReleaseQF.Items.Clear();
            ListItem li = null;
            List<string> ReleaseGroups = new List<string>();

            foreach (DataRow dr in dtRelease.Rows)
            {
                DataTable dtDeliverable = MasterData.ReleaseSchedule_DeliverableList_Get(int.Parse(dr["ProductVersionID"].ToString()));

                if (dtDeliverable.Rows.Count > 1)
                {
                    ReleaseGroups.Add(dr["ProductVersion"].ToString());

                    foreach (DataRow row in dtDeliverable.Rows)
                    {
                        li = new ListItem(row["ReleaseScheduleDeliverable"].ToString(), dr["ProductVersionID"].ToString() + "." + row["ReleaseScheduleID"].ToString());
                        li.Attributes.Add("OptionGroup", dr["ProductVersion"].ToString());
                        li.Selected = (QFDeliverable.Contains(row["ReleaseScheduleID"].ToString()) || QFRelease.Contains(dr["ProductVersionID"].ToString()) || QFRelease.Contains(row["ProductVersionID"].ToString()));

                        if (QFRelease.Count() == 0 && QFDeliverable.Count() == 0)
                        {
                            if (dr["ProductVersionID"].ToString() == currentReleaseID)
                            {
                                li.Selected = true;
                            }
                            else
                            {
                                li.Selected = false;
                            }
                        }
                        ddlReleaseQF.Items.Add(li);
                    }
                } 
                else
                {
                    li = new ListItem(dr["ProductVersion"].ToString(), dr["ProductVersionID"].ToString());
                    li.Selected = (QFRelease.Count() == 0 || QFRelease.Contains(dr["ProductVersionID"].ToString()));
                    
                    if (QFRelease.Count() == 0)
                    {
                        if (dr["ProductVersionID"].ToString() == currentReleaseID)
                        {
                            li.Selected = true;
                        }
                        else
                        {
                            li.Selected = false;
                        }
                    }
                    ddlReleaseQF.Items.Add(li);
                }
                QFReleaseGroups = string.Join(",", ReleaseGroups.ToArray());
            }
        }

        DataTable dtContract = MasterData.ContractList_Get(includeArchive: false);
        
        if (dtContract != null)
        {
            ddlContractQF.Items.Clear();

            foreach (DataRow dr in dtContract.Rows)
            {
                ListItem li = new ListItem(dr["Contract"].ToString(), dr["ContractID"].ToString());
                li.Selected = QFContract.Contains(dr["ContractID"].ToString());

                ddlContractQF.Items.Add(li);
            }
        }
        if (QFContract.Count() > 0)
        {
            LoadData();
        }
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["SelectedReleases"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedReleases"]))
        {
            this.QFRelease = Request.QueryString["SelectedReleases"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["SelectedDeliverables"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedDeliverables"]))
        {
            this.QFDeliverable = Request.QueryString["SelectedDeliverables"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["SelectedContracts"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedContracts"]))
        {
            this.QFContract = Request.QueryString["SelectedContracts"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
    #endregion

    #region Data
    private void LoadData()
    {
        List<string> listRelease = new List<string>();
        List<string> listDeliverables = new List<string>();
        List<string> listContract = new List<string>();
        List<string> listSuite = new List<string>();

        if (QFRelease != null && QFRelease.Length > 0)
        {
            foreach (string s in QFRelease)
            {
                listRelease.Add(s);
            }
        } else if (QFDeliverable == null || QFDeliverable.Length == 0)
        {
            DataTable dtCurrentRelease = AOR.AORCurrentRelease_Get();

            if (dtCurrentRelease != null && dtCurrentRelease.Rows.Count > 0)
            {
                listRelease.Add(dtCurrentRelease.Rows[0]["ProductVersionID"].ToString());
            }
        }

        if (QFDeliverable != null && QFDeliverable.Length > 0)
        {
            foreach (string s in QFDeliverable)
            {
                listDeliverables.Add(s);
            }
        }

        if (ddlContractQF != null && ddlContractQF.Items.Count > 0)
        {
            foreach (ListItem li in ddlContractQF.Items)
            {
                if (li.Selected) listContract.Add(li.Value);
            }
        }

        DataSet dsSummary = AOR.AORSummaryList_Get(AlertType: string.Empty, AORID: 0, AORReleaseID: 0, ReleaseIDs: String.Join(",", listRelease), DeliverableIDs: String.Join(",", listDeliverables), ContractIDs: string.Join(",", listContract));

        if (dsSummary != null)
        {
            DataTable dtCR = dsSummary.Tables["CR"];
            DataTable dtWorkloadAllocation = dsSummary.Tables["WorkloadAllocation"];
            DataTable dtAlert = dsSummary.Tables["Alert"];
            StringBuilder sbMetric = new StringBuilder();
            StringBuilder sbAlert = new StringBuilder();
            string currentReleaseID = null;
            string currentDeliverableID = null;
            string currentContractID = null;
            string CRCount = "";

            //Metric
            for (int i = 0; i < dtCR.Rows.Count; i++)
            {
                DataRow dr = dtCR.Rows[i];

                if (dr["ProductVersionID"].ToString() != currentReleaseID || dr["ReleaseScheduleID"].ToString() != currentDeliverableID || dr["CONTRACTID"].ToString() != currentContractID)
                {
                    if (sbMetric.Length > 0)
                    {
                        sbMetric.Append("</table><br>");
                        sbMetric.Append(LoadWorkloadAllocation(dtWorkloadAllocation.Copy(), dtCR.Rows[i - 1]["ProductVersionID"].ToString(), dtCR.Rows[i - 1]["ReleaseScheduleID"].ToString(), dtCR.Rows[i - 1]["CONTRACTID"].ToString()));
                    }

                    if (i != dtCR.Rows.Count)
                    {
                        DateTime nDate = new DateTime();
                        string plannedStart = "-", plannedEnd = "-", actualEnd = "-";
                        if (DateTime.TryParse(dr["PlannedStart"].ToString(), out nDate)) plannedStart = String.Format("{0:M/d/yyyy}", nDate);
                        if (DateTime.TryParse(dr["PlannedEnd"].ToString(), out nDate)) plannedEnd = String.Format("{0:M/d/yyyy}", nDate);
                        if (DateTime.TryParse(dr["ActualEnd"].ToString(), out nDate)) actualEnd = String.Format("{0:M/d/yyyy}", nDate);

                        sbMetric.Append("<b>Release: </b>" + dr["ProductVersion"].ToString());
                        if (dr["ReleaseSchedule"].ToString() != "-") sbMetric.Append("<br><b>Deployment: </b>" + dr["ReleaseSchedule"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;<b>Planned Start: </b>" + plannedStart + "&nbsp;&nbsp;&nbsp;&nbsp;<b>Planned End: </b>" + plannedEnd + "&nbsp;&nbsp;&nbsp;&nbsp;<b>Actual End: </b>" + actualEnd);
                        sbMetric.Append("<br><b>Contract: </b>" + dr["CONTRACT"].ToString());
                        sbMetric.Append("<div id=\"divTabsContainer\" class=\"summaryContainer\" style=\"margin-left: 22px; padding-bottom: 0px; width: 95%\"><div id=\"divSummary" + i + "\" style=\"display: none;\"></div><ul>");
                        int productVersion = 0, deliverable = 0, contract = 0;
                        int.TryParse(dr["ProductVersionID"].ToString(), out productVersion);
                        int.TryParse(dr["ReleaseScheduleID"].ToString(), out deliverable);
                        int.TryParse(dr["CONTRACTID"].ToString(), out contract);
                        DataTable dtSuite = AOR.AORSummarySuiteList_Get(productVersion, deliverable, contract);

                        if (dtSuite != null && dtSuite.Rows.Count > 0)
                        {
                            foreach (DataRow suiteTabs in dtSuite.Rows)
                            {
                                CRCount = (from row in dtCR.AsEnumerable()
                                           where row.Field<int?>("ProductVersionID").ToString() == dr["ProductVersionID"].ToString()
                                           && row.Field<int?>("ReleaseScheduleID").ToString() == dr["ReleaseScheduleID"].ToString()
                                           && row.Field<int?>("CONTRACTID").ToString() == dr["CONTRACTID"].ToString()
                                           && row.Field<int?>("WTS_SYSTEM_SUITEID").ToString() == suiteTabs["WTS_SYSTEM_SUITEID"].ToString()
                                           select row.Field<int>("CRID")).Distinct().Count().ToString();

                                sbMetric.Append("<li><a href=\"#divSummary" + i + "\" onclick=\"suiteTabClick(" + suiteTabs["WTS_SYSTEM_SUITEID"].ToString() + "," + i + "); return false;\">" + suiteTabs["WTS_SYSTEM_SUITE"].ToString() + " (" + CRCount + ")" + "</a></li>");
                            }
                        }
                        
                        sbMetric.Append("</ul></div>");
                        sbMetric.Append("<table summary=\"" + i + "\" style=\"border-collapse: collapse; margin-left: 25px; width: 95%\">");
                        sbMetric.Append("<tr class=\"gridHeader\"><th rowspan=\"2\" style=\"border-top: 1px solid grey; border-left: 1px solid grey;\">CR</th><th rowspan=\"2\" style=\"border-top: 1px solid grey; width: 150px\">Workload Allocation</th><th rowspan=\"2\" style=\"border-top: 1px solid grey; width: 150px;\">Workload Priority</th><th rowspan=\"2\" style=\"border-top: 1px solid grey; width: 50px;\"># of AORs</th><th colspan=\"3\" style=\"border-top: 1px solid grey;\"># of SRs</th><th colspan=\"5\" style=\"border-top: 1px solid grey;\"># of Work Tasks</th></tr>");
                        sbMetric.Append("<tr class=\"gridHeader\"><th style=\"display: none;\"></th><th style=\"display: none;\"></th><th style=\"display: none;\"></th><th style=\"border-top: 1px solid grey; width: 28px;\">Total</th><th style=\"border-top: 1px solid grey; width: 31px;\">Open</th><th style=\"border-top: 1px solid grey; width: 41px;\">Closed</th><th style=\"border-top: 1px solid grey; width: 28px;\">Total</th><th style=\"border-top: 1px solid grey; width: 38px;\">Carry In</th><th style=\"border-top: 1px solid grey; width: 70px;\">New In Release</th><th style=\"border-top: 1px solid grey; width: 65px;\">Open (Carry Out)</th><th style=\"border-top: 1px solid grey; width: 41px;\">Closed</th></tr>");
                    }
                }

                if (i != dtCR.Rows.Count) sbMetric.Append("<tr class=\"gridBody\" suite=\""+ dr["WTS_SYSTEM_SUITEID"].ToString() + "\"" + ((i & 1) == 1 ? " style =\"background-color: gainsboro;\"" : "") + "><td style=\"border-left: 1px solid grey;\"><a href=\"\" onclick=\"openCR(" + dr["CRID"].ToString() + "); return false;\" style=\"color: blue;\">" + Uri.UnescapeDataString(dr["CRName"].ToString()) + "</a></td><td style=\"text-align: center;\">" + dr["WorkloadAllocation"].ToString() + "</td><td style=\"text-align: center;\">" + dr["WorkloadPriority"].ToString() + "</td><td style=\"text-align: center;\">" + dr["TotalAORCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["TotalSRCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["OpenSRCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["ClosedSRCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["TotalTaskCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["CarryInTaskCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["NewInReleaseTaskCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["OpenTaskCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["ClosedTaskCount"].ToString() + "</td></tr>");

                if (i == dtCR.Rows.Count - 1)
                {
                    sbMetric.Append("</table><br>");
                    sbMetric.Append(LoadWorkloadAllocation(dtWorkloadAllocation.Copy(), dtCR.Rows[i]["ProductVersionID"].ToString(), dtCR.Rows[i]["ReleaseScheduleID"].ToString(), dtCR.Rows[i]["CONTRACTID"].ToString()));
                }

                currentReleaseID = dr["ProductVersionID"].ToString();
                currentDeliverableID = dr["ReleaseScheduleID"].ToString();
                currentContractID = dr["CONTRACTID"].ToString();
            }

            divMetric.InnerHtml = sbMetric.ToString();

            //Alert
            sbAlert.Append("<table style=\"width: 425px; border-collapse: collapse; text-align: center;\">");
            sbAlert.Append("<tr class=\"gridHeader\"><th style=\"border-top: 1px solid grey; border-left: 1px solid grey;\">Alert</th><th style=\"width: 75px; border-top: 1px solid grey;\"># of AORs</th></tr>");

            if (dtAlert.Rows.Count == 0)
            {
                sbAlert.Append("<tr class=\"gridBody\"><td colspan=\"2\" style=\"border-left: 1px solid grey; text-align: center;\">No Alerts</td></tr>");
            }
            else
            {
                var query = from row in dtAlert.AsEnumerable()
                            group row by row.Field<string>("Alert_ID") into alertGroup
                            orderby alertGroup.Key
                            select new
                            {
                                Name = alertGroup.Key,
                                AORCount = alertGroup.Count()
                            };

                foreach (var alertGroup in query)
                {
                    sbAlert.Append("<tr class=\"gridBody\"><td style=\"border-left: 1px solid grey;\">" + alertGroup.Name + "</td><td style=\"text-align: center;\"><a href=\"\" onclick=\"displayAlertAORs('" + alertGroup.Name + "'); return false; \" style=\"color: blue; \">" + alertGroup.AORCount + "</a></td></tr>");
                }
            }

            sbAlert.Append("</table>");

            divAlert.InnerHtml = sbAlert.ToString();
        }
        else
        {
            divMetric.InnerHtml = "Error gathering data.";
        }
    }

    private StringBuilder LoadWorkloadAllocation(DataTable dt, string release, string deliverable, string contract)
    {
        StringBuilder sb = new StringBuilder();

        DataTable dtWorkloadAllocationSub = dt;
        dtWorkloadAllocationSub.DefaultView.RowFilter = "isnull(ProductVersionID, 0) = " + (release == "" ? "0" : release) + 
                                                    " and isnull(ReleaseScheduleID, 0) = " + (deliverable == "" ? "0" : deliverable) + 
                                                    " and isnull(CONTRACTID, 0) = " + (contract == "" ? "0" : contract);
        dtWorkloadAllocationSub = dtWorkloadAllocationSub.DefaultView.ToTable();

        sb.Append("<table style=\"border-collapse: collapse; margin-left: 25px; width: 700px;\">");
        sb.Append("<tr class=\"gridHeader\"><th rowspan=\"2\" style=\"width: 150px; border-top: 1px solid grey; border-left: 1px solid grey;\">Workload Allocation</th><th rowspan=\"2\" style=\"border-top: 1px solid grey;\"># of AORs</th><th colspan=\"3\" style=\"border-top: 1px solid grey;\"># of SRs</th><th colspan=\"5\" style=\"border-top: 1px solid grey;\"># of Work Tasks</th></tr>");
        sb.Append("<tr class=\"gridHeader\"><th style=\"display: none;\"></th><th style=\"display: none;\"></th><th style=\"border-top: 1px solid grey;\">Total</th><th style=\"border-top: 1px solid grey;\">Open</th><th style=\"border-top: 1px solid grey;\">Closed</th><th style=\"border-top: 1px solid grey;\">Total</th><th style=\"border-top: 1px solid grey;\">Carry In</th><th style=\"border-top: 1px solid grey;\">New In Release</th><th style=\"border-top: 1px solid grey;\">Open (Carry Out)</th><th style=\"border-top: 1px solid grey;\">Closed</th></tr>");

        for (int i = 0; i < dtWorkloadAllocationSub.Rows.Count; i++)
        {
            DataRow dr = dtWorkloadAllocationSub.Rows[i];

            sb.Append("<tr class=\"gridBody\" " + ((i & 1) == 1 ? " style =\"background-color: gainsboro;" : "") + "\"><td style=\"border-left: 1px solid grey;\">" + dr["WorkloadAllocation"].ToString() + "</td><td style=\"text-align: center;\">" + dr["TotalAORCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["TotalSRCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["OpenSRCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["ClosedSRCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["TotalTaskCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["CarryInTaskCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["NewInReleaseTaskCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["OpenTaskCount"].ToString() + "</td><td style=\"text-align: center;\">" + dr["ClosedTaskCount"].ToString() + "</td></tr>");
        }

        sb.Append("</table><br>");

        return sb;
    }
    #endregion
}