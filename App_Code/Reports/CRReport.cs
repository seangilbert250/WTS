﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using System.Web.UI;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data.SqlClient;

namespace WTS.Reports
{
    /// <summary>
    /// Summary description for CRReport
    /// </summary>
    public class CRReport : ReportBase
    {
        public CRReport()
        {

        }

        public override Dictionary<string, string> GetFilterFields()
        {
            return new Dictionary<string, string>()
            {
                { "Header", "Reporting Options" },
                { "Release Version", "Release Version" },
                { "Deployment", "Deployment" },
                { "AOR Workload Type", "AOR Workload Type" },
                { "Visible To Customer", "Visible To Customer" },
                { "Contract", "Contract" },
                { "System Suite", "System Suite" },
                { "Workload Allocation", "Workload Allocation" },
            };
        }

        public override string GetPageTitle()
        {
            return "CR Report";
        }

        public override string GetDefaultFilters()
        {
            return "Release Version|Release Version|40|18.1`" +
                    "Visible To Customer|Visible To Customer|1|Yes`" +
                    "Contract|Contract|1|CAFDEx CAM Contract`";
                    //"Status|Status|8,,10,,15,,9,,5,,4,,3,,1,,6,,7|Checked In,,Closed,,Complete,,Deployed,,In Progress,,Info Provided,,Info Requested,,New,,On Hold,,Un-Reproducible`" +
        }

        public override DataSet GetLevelFields()
        {
            DataSet dsItems = new DataSet();

            DataTable dtCR = new DataTable("CR FIELDS");
            dtCR.Columns.Add("VALUE");
            dtCR.Columns.Add("TEXT");
            DataTable dtSR = new DataTable("SR FIELDS");
            dtSR.Columns.Add("VALUE");
            dtSR.Columns.Add("TEXT");
            DataTable dtAOR = new DataTable("AOR FIELDS");
            dtAOR.Columns.Add("VALUE");
            dtAOR.Columns.Add("TEXT");
            //DataTable dtTask = new DataTable("TASK FIELDS");
            //dtTask.Columns.Add("VALUE");
            //dtTask.Columns.Add("TEXT");

            //CR FIELDS
            dtCR.Rows.Add("CR Title", "CR Title(LVL_2)");
            dtCR.Rows.Add("CR Primary Websys", "CR Primary Websys(LVL_2)");
            dtCR.Rows.Add("CR Work Priority", "CR Work Priority(LVL_2)");
            dtCR.Rows.Add("CR Coord", "CR Coord(LVL_2)");
            dtCR.Rows.Add("Customer Priority", "Customer Priority(LVL_2)");
            dtCR.Rows.Add("ITI Priority", "ITI Priority(LVL_2)");
            dtCR.Rows.Add("Last Updated", "Last Updated(LVL_2)");
            dtCR.Rows.Add("Description", "Description(LVL_2)");
            dtCR.Rows.Add("Rationale", "Rationale(LVL_2)");
            dtCR.Rows.Add("Customer Impact", "Customer Impact(LVL_2)");
            dtCR.DefaultView.Sort = "TEXT";

            //SR FIELDS
            dtSR.Rows.Add("SRs", "SRs(LVL_2)");
            dtSR.DefaultView.Sort = "TEXT";

            //AOR FIELDS
            dtAOR.Rows.Add("Release Version", "Release Version(LVL_1)");
            dtAOR.Rows.Add("Contract", "Contract(LVL_1)");
            dtAOR.Rows.Add("Work Type", "Work Type(LVL_1 & LVL_3)");
            dtAOR.Rows.Add("AOR Name", "AOR Name(LVL_4)");
            dtAOR.Rows.Add("PD2TDR status", "PD2TDR status(LVL_4)");
            dtAOR.Rows.Add("Primary Websys", "Primary Websys(LVL_4)");
            dtAOR.Rows.Add("Workload Priority", "Workload Priority(LVL_4)");
            dtAOR.Rows.Add("Description", "Description(LVL_4)");
            dtAOR.Rows.Add("AOR Counts", "AOR Counts(LVL_4)");

            //Task FIELDS
            //dtTask.Rows.Add("Task Title", "Task Title");
            //dtTask.DefaultView.Sort = "TEXT";

            dsItems.Tables.Add(dtCR.DefaultView.ToTable());
            dsItems.Tables.Add(dtSR.DefaultView.ToTable());
            dsItems.Tables.Add(dtAOR.DefaultView.ToTable());
            //dsItems.Tables.Add(dtTask.DefaultView.ToTable());

            return dsItems;
        }

        public override bool ExecuteReport(Dictionary<string, string> reportParameters, int WTS_RESOURCEID, ref string errors, ref string outFileName, ref byte[] outFile)
        {
            if (reportParameters == null)
            {
                errors = "Report Parameters missing or invalid.";
                return false;
            }

            string type = reportParameters["Type"];
            string title = reportParameters["Title"];
            string CoverPage = reportParameters["CoverPage"];
            string IndexPage = reportParameters["IndexPage"];
            string EmailSupport = reportParameters["EmailSupport"];
            string releaseIDs = reportParameters.ContainsKey("Release Version") ? reportParameters["Release Version"] : "";
            string AORTypes = reportParameters.ContainsKey("AOR Workload Type") ? reportParameters["AOR Workload Type"] : "";
            string ScheduledDeliverables = reportParameters.ContainsKey("Deployment") ? reportParameters["Deployment"] : "";
            string VisibleToCustomer = reportParameters.ContainsKey("Visible To Customer") ? reportParameters["Visible To Customer"] : "";
            string contractIDs = reportParameters.ContainsKey("Contract") ? reportParameters["Contract"] : "";
            string suiteIDs = reportParameters.ContainsKey("System Suite") ? reportParameters["System Suite"] : "";
            string WorkloadStatus = reportParameters.ContainsKey("Status") ? reportParameters["Status"] : "";
            string WorkloadAllocations = reportParameters.ContainsKey("Workload Allocation") ? reportParameters["Workload Allocation"] : "";
            string IncludeSRs = reportParameters["IncludeSRs"];
            string IncludeAORs = reportParameters["IncludeAORs"];
            string IncludeSessionMetrics = reportParameters["IncludeSessionMetrics"];
            string IncludeBestCase = reportParameters["IncludeBestCase"];
            string IncludeWorstCase = reportParameters["IncludeWorstCase"];
            string IncludeNormCase = reportParameters["IncludeNormCase"];
            string HideCRDescr = reportParameters["HideCRDescr"];
            string HideAORDescr = reportParameters["HideAORDescr"];
            string SavedView = reportParameters["SavedView"];
            string CreatedBy = "";

            WTS_User u = new WTS_User(WTS_RESOURCEID);
            u.Load();
            CreatedBy = u.First_Name.Replace("'","''") + " " + u.Last_Name;

            switch (type)
            {
                case "pdf":
                    DataSet ds = AOR.AORCRReport_Get(releaseIDs, ScheduledDeliverables, AORTypes, VisibleToCustomer, contractIDs, suiteIDs, WorkloadStatus, WorkloadAllocations, title, SavedView, CoverPage, IndexPage, IncludeBestCase, IncludeWorstCase, IncludeNormCase, HideCRDescr, HideAORDescr, CreatedBy);

                    if (ds != null)
                    {
                        #region CR
                        DataTable dtCR = ds.Tables["CR"];
                        DateTime nDate = new DateTime();

                        dtCR.Columns.Add("UpdatedDate");

                        for (int i = 0; i < dtCR.Rows.Count; i++)
                        {
                            dtCR.Rows[i]["CRCustomerTitle"] = Uri.UnescapeDataString(dtCR.Rows[i]["CRCustomerTitle"].ToString());
                            dtCR.Rows[i]["CRInternalTitle"] = Uri.UnescapeDataString(dtCR.Rows[i]["CRInternalTitle"].ToString());
                            dtCR.Rows[i]["Notes"] = Uri.UnescapeDataString(dtCR.Rows[i]["Notes"].ToString());
                            dtCR.Rows[i]["SRs"] = Uri.UnescapeDataString(dtCR.Rows[i]["SRs"].ToString());

                            if (DateTime.TryParse(dtCR.Rows[i]["UpdatedDateTime"].ToString(), out nDate))
                            {
                                dtCR.Rows[i]["UpdatedDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                            }
                        }

                        dtCR.Columns["UpdatedDate"].SetOrdinal(dtCR.Columns["UpdatedDateTime"].Ordinal);
                        dtCR.Columns.Remove("UpdatedDateTime");
                        #endregion

                        #region AOR
                        DataTable dtAOR = ds.Tables["AOR"];

                        dtAOR.Columns.Add("UpdatedDate");
                        dtAOR.Columns.Add("LastMeeting");
                        dtAOR.Columns.Add("NextMeeting");

                        for (int i = 0; i < dtAOR.Rows.Count; i++)
                        {
                            nDate = new DateTime();

                            if (DateTime.TryParse(dtAOR.Rows[i]["UpdatedDateTime"].ToString(), out nDate))
                            {
                                dtAOR.Rows[i]["UpdatedDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                            }

                            nDate = new DateTime();

                            if (DateTime.TryParse(dtAOR.Rows[i]["LastMeetingTime"].ToString(), out nDate))
                            {
                                dtAOR.Rows[i]["LastMeeting"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                            }

                            nDate = new DateTime();

                            if (DateTime.TryParse(dtAOR.Rows[i]["NextMeetingTime"].ToString(), out nDate))
                            {
                                dtAOR.Rows[i]["NextMeeting"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                            }
                        }

                        dtAOR.Columns["UpdatedDate"].SetOrdinal(dtAOR.Columns["UpdatedDateTime"].Ordinal);
                        dtAOR.Columns["LastMeeting"].SetOrdinal(dtAOR.Columns["LastMeetingTime"].Ordinal);
                        dtAOR.Columns["NextMeeting"].SetOrdinal(dtAOR.Columns["NextMeetingTime"].Ordinal);
                        dtAOR.Columns.Remove("UpdatedDateTime");
                        dtAOR.Columns.Remove("LastMeetingTime");
                        dtAOR.Columns.Remove("NextMeetingTime");
                        #endregion

                        #region SR
                        DataTable dtSR = ds.Tables["SR"];

                        dtSR.Columns.Add("UpdatedDate");

                        for (int i = 0; i < dtSR.Rows.Count; i++)
                        {
                            dtSR.Rows[i]["Description"] = Uri.UnescapeDataString(dtSR.Rows[i]["Description"].ToString());
                        }
                        #endregion

                        #region Summary
                        DataTable dtGroups = dtCR.DefaultView.ToTable(true, new string[] { "ProductVersionID", "ProductVersion", "ProductVersionSort", "CONTRACTID", "CONTRACT", "WorkloadAllocationID", "WorkloadAllocation", "WorkloadAllocationSort", "ContractSort", "CRID", "PrimarySR", "Sort", "CRCustomerTitle", "MinStatusLvl1", "MaxStatusLvl1", "MostStatusLvl1", "MinStatusLvl2", "MaxStatusLvl2", "MostStatusLvl2" });
                        dtGroups.DefaultView.Sort = "ProductVersionSort, ContractSort, WorkloadAllocationSort, Sort, PrimarySR";
                        dtGroups = dtGroups.DefaultView.ToTable();
                        DataTable dtSummary = new DataTable("Summary");

                        dtSummary.Columns.Add("ProductVersion", typeof(string));
                        dtSummary.Columns.Add("CONTRACT", typeof(string));
                        dtSummary.Columns.Add("WorkloadAllocation", typeof(string));
                        dtSummary.Columns.Add("WorkloadAllocationID", typeof(string));
                        dtSummary.Columns.Add("PrimarySR", typeof(string));
                        dtSummary.Columns.Add("CRCustomerTitle", typeof(string));
                        dtSummary.Columns.Add("MGMTReleaseSummary", typeof(byte[]));
                        dtSummary.Columns.Add("MGMTWorkloadSummary", typeof(byte[]));
                        dtSummary.Columns.Add("PriWebsystemSuite", typeof(string));
                        dtSummary.Columns.Add("PrimaryWebsystem", typeof(string));
                        dtSummary.Columns.Add("AORWorkTypeName", typeof(string));
                        dtSummary.Columns.Add("AORWorkTypeID", typeof(string));
                        dtSummary.Columns.Add("AORRequiresPD2TDR", typeof(string));
                        dtSummary.Columns.Add("AORID", typeof(string));
                        dtSummary.Columns.Add("AORName", typeof(string));
                        dtSummary.Columns.Add("InvestigationStatus", typeof(string));
                        dtSummary.Columns.Add("TechnicalStatus", typeof(string));
                        dtSummary.Columns.Add("CustomerDesignStatus", typeof(string));
                        dtSummary.Columns.Add("CodingStatus", typeof(string));
                        dtSummary.Columns.Add("InternalTestingStatus", typeof(string));
                        dtSummary.Columns.Add("CustomerValidationTestingStatus", typeof(string));
                        dtSummary.Columns.Add("AdoptionStatus", typeof(string));
                        dtSummary.Columns.Add("ScheduledDate", typeof(string));
                        dtSummary.Columns.Add("PercentClosed", typeof(string));
                        dtSummary.Columns.Add("NumberOpen", typeof(string));
                        dtSummary.Columns.Add("TaskCount", typeof(string));
                        dtSummary.Columns.Add("Sort", typeof(string));
                        dtSummary.Columns.Add("MinStatusLvl1", typeof(string));
                        dtSummary.Columns.Add("MaxStatusLvl1", typeof(string));
                        dtSummary.Columns.Add("MostStatusLvl1", typeof(string));
                        dtSummary.Columns.Add("MinStatusLvl2", typeof(string));
                        dtSummary.Columns.Add("MaxStatusLvl2", typeof(string));
                        dtSummary.Columns.Add("MostStatusLvl2", typeof(string));
                        dtSummary.Columns.Add("WorkloadPriority", typeof(string));
                        dtSummary.Columns.Add("CyberReview", typeof(string));
                        dtSummary.Columns.Add("CMMI", typeof(string));
                        dtSummary.Columns.Add("PD2TDRType", typeof(string));
                        dtSummary.Columns.Add("PD2TDR", typeof(string));

                        foreach (DataRow dr in dtGroups.Rows)
                        {
                            DataTable dtTemp = new DataTable();

                            dtAOR.DefaultView.RowFilter = "isnull(ProductVersionID, 0) = " + (dr["ProductVersionID"].ToString() == "" ? 0 : dr["ProductVersionID"]) + " and isnull(CONTRACTID, 0) = " + (dr["CONTRACTID"].ToString() == "" ? 0 : dr["CONTRACTID"]) + " and isnull(WorkloadAllocationID, 0) = " + (dr["WorkloadAllocationID"].ToString() == "" ? 0 : dr["WorkloadAllocationID"]) + " and CRID = " + dr["CRID"];
                            dtAOR.DefaultView.Sort = "PriWebsystemSuite, ScheduledDate, AORID";
                            dtTemp = dtAOR.DefaultView.ToTable(true, new string[] { "PriWebsystemSuite", "PrimaryWebsystem", "WorkloadAllocationID", "AORWorkTypeName","AORWorkTypeID", "AORRequiresPD2TDR", "AORID", "AORName", "InvestigationStatus", "TechnicalStatus", "CustomerDesignStatus", "CodingStatus", "InternalTestingStatus", "CustomerValidationTestingStatus", "AdoptionStatus", "ScheduledDate", "PercentClosed", "NumberOpen", "TaskCount", "WorkloadPriority", "CyberReview", "CMMI", "PD2TDRType", "PD2TDR" });

                            /*#region MGMTRelease
                            dtAOR.DefaultView.RowFilter = "AORWorkTypeName = 'Release/Deployment MGMT' and isnull(ProductVersionID, 0) = " + (dr["ProductVersionID"].ToString() == "" ? 0 : dr["ProductVersionID"]) + " and isnull(CONTRACTID, 0) = " + (dr["CONTRACTID"].ToString() == "" ? 0 : dr["CONTRACTID"]) + " and CRID = " + dr["CRID"];
                            dtTemp = dtAOR.DefaultView.ToTable(true, new string[] { "AORID", "AORName", "InvestigationStatus", "TechnicalStatus", "CustomerDesignStatus", "CodingStatus", "InternalTestingStatus", "CustomerValidationTestingStatus", "AdoptionStatus" });
                            dtTemp.Columns["AORID"].ColumnName = "AOR #";
                            dtTemp.Columns["AORName"].ColumnName = "AOR Name";
                            dtTemp.Columns["InvestigationStatus"].ColumnName = "INV";
                            dtTemp.Columns["TechnicalStatus"].ColumnName = "TD";
                            dtTemp.Columns["CustomerDesignStatus"].ColumnName = "CD";
                            dtTemp.Columns["CodingStatus"].ColumnName = "C";
                            dtTemp.Columns["InternalTestingStatus"].ColumnName = "IT";
                            dtTemp.Columns["CustomerValidationTestingStatus"].ColumnName = "CVT";
                            dtTemp.Columns["AdoptionStatus"].ColumnName = "A";

                            byte[] imgMGMTRelease = null;

                            if (dtTemp.Rows.Count > 0) imgMGMTRelease = WTSUtility.ConvertDataTableToImage(dtTemp);
                            #endregion

                            #region MGMTWorkload
                            dtAOR.DefaultView.RowFilter = "AORWorkTypeName = 'Workload MGMT' and isnull(ProductVersionID, 0) = " + (dr["ProductVersionID"].ToString() == "" ? 0 : dr["ProductVersionID"]) + " and isnull(CONTRACTID, 0) = " + (dr["CONTRACTID"].ToString() == "" ? 0 : dr["CONTRACTID"]) + " and CRID = " + dr["CRID"];
                            dtTemp = dtAOR.DefaultView.ToTable(true, new string[] { "AORID", "AORName" });
                            dtTemp.Columns["AORID"].ColumnName = "AOR #";
                            dtTemp.Columns["AORName"].ColumnName = "AOR Name";

                            byte[] imgMGMTWorkload = null;

                            if (dtTemp.Rows.Count > 0) imgMGMTWorkload = WTSUtility.ConvertDataTableToImage(dtTemp);
                            #endregion*/

                            foreach (DataRow drTemp in dtTemp.Rows)
                            {
                                DataRow rowSummary = dtSummary.NewRow();

                                rowSummary["ProductVersion"] = dr["ProductVersion"];
                                rowSummary["CONTRACT"] = dr["CONTRACT"];
                                rowSummary["WorkloadAllocation"] = dr["WorkloadAllocation"];
                                rowSummary["WorkloadAllocationID"] = drTemp["WorkloadAllocationID"];
                                rowSummary["PrimarySR"] = dr["PrimarySR"];
                                rowSummary["CRCustomerTitle"] = dr["CRCustomerTitle"];
                                //rowSummary["MGMTReleaseSummary"] = imgMGMTRelease;
                                //rowSummary["MGMTWorkloadSummary"] = imgMGMTWorkload;
                                rowSummary["PrimaryWebsystem"] = drTemp["PrimaryWebsystem"];
                                rowSummary["PriWebsystemSuite"] = drTemp["PriWebsystemSuite"];
                                rowSummary["AORWorkTypeName"] = drTemp["AORWorkTypeName"];
                                rowSummary["AORWorkTypeID"] = drTemp["AORWorkTypeID"];
                                rowSummary["AORRequiresPD2TDR"] = drTemp["AORRequiresPD2TDR"];
                                rowSummary["AORID"] = drTemp["AORID"];
                                rowSummary["AORName"] = drTemp["AORName"];
                                rowSummary["InvestigationStatus"] = drTemp["InvestigationStatus"];
                                rowSummary["TechnicalStatus"] = drTemp["TechnicalStatus"];
                                rowSummary["CustomerDesignStatus"] = drTemp["CustomerDesignStatus"];
                                rowSummary["CodingStatus"] = drTemp["CodingStatus"];
                                rowSummary["InternalTestingStatus"] = drTemp["InternalTestingStatus"];
                                rowSummary["CustomerValidationTestingStatus"] = drTemp["CustomerValidationTestingStatus"];
                                rowSummary["AdoptionStatus"] = drTemp["AdoptionStatus"];
                                rowSummary["ScheduledDate"] = drTemp["ScheduledDate"];
                                rowSummary["PercentClosed"] = drTemp["PercentClosed"];
                                rowSummary["NumberOpen"] = drTemp["NumberOpen"];
                                rowSummary["TaskCount"] = drTemp["TaskCount"];
                                rowSummary["Sort"] = dr["Sort"];
                                rowSummary["MinStatusLvl1"] = dr["MinStatusLvl1"];
                                rowSummary["MaxStatusLvl1"] = dr["MaxStatusLvl1"];
                                rowSummary["MostStatusLvl1"] = dr["MostStatusLvl1"];
                                rowSummary["MinStatusLvl2"] = dr["MinStatusLvl2"];
                                rowSummary["MaxStatusLvl2"] = dr["MaxStatusLvl2"];
                                rowSummary["MostStatusLvl2"] = dr["MostStatusLvl2"];
                                rowSummary["WorkloadPriority"] = drTemp["WorkloadPriority"];
                                rowSummary["CyberReview"] = drTemp["CyberReview"];
                                rowSummary["CMMI"] = drTemp["CMMI"];
                                rowSummary["PD2TDRType"] = drTemp["PD2TDRType"];
                                rowSummary["PD2TDR"] = drTemp["PD2TDR"];

                                dtSummary.Rows.Add(rowSummary);
                            }
                        }

                        ds.Tables.Add(dtSummary);
                        #endregion

                        outFileName = "CR_Report.pdf";
                        ReportDocument cryRpt = new ReportDocument();
                        string cnString = WTSCommon.WTS_ConnectionString;
                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cnString);
                        string user = builder.UserID;
                        string pass = builder.Password;
                        string server = builder.DataSource;
                        string database = builder.InitialCatalog;
                        
                        cryRpt.Load(System.Web.Hosting.HostingEnvironment.MapPath(@"~/Reports/CR.rpt"));
                        //cryRpt.SetDatabaseLogon(user, pass, server, database);
                        //cryRpt.DataSourceConnections[0].SetConnection(server, database, user, pass);
                        //ds.WriteXml("C:/Users/jonssone/Desktop/Test.xml", XmlWriteMode.WriteSchema);
                        Sections crSections;
                        ReportDocument crSubreportDocument;
                        SubreportObject crSubreportObject;
                        ReportObjects crReportObjects;
                        TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                        TableLogOnInfo crTableLogOnInfo = new TableLogOnInfo();
                        ConnectionInfo crConnectionInfo = new ConnectionInfo();
                        Database  crDatabase = cryRpt.Database;
                        Tables crTables = crDatabase.Tables;
                        crConnectionInfo.ServerName = server;
                        crConnectionInfo.DatabaseName = database;
                        crConnectionInfo.UserID = user;
                        crConnectionInfo.Password = pass;

                        foreach (CrystalDecisions.CrystalReports.Engine.Table aTable in crTables)
                        {
                            crTableLogOnInfo = aTable.LogOnInfo;
                            crTableLogOnInfo.ConnectionInfo = crConnectionInfo;
                            aTable.ApplyLogOnInfo(crTableLogOnInfo);
                        }
                        // THIS STUFF HERE IS FOR REPORTS HAVING SUBREPORTS 
                        // set the sections object to the current report's section 
                        crSections = cryRpt.ReportDefinition.Sections;
                        // loop through all the sections to find all the report objects 
                        foreach (Section crSection in crSections)
                        {
                            crReportObjects = crSection.ReportObjects;
                            //loop through all the report objects in there to find all subreports 
                            foreach (ReportObject crReportObject in crReportObjects)
                            {
                                if (crReportObject.Kind == ReportObjectKind.SubreportObject)
                                {
                                    crSubreportObject = (SubreportObject)crReportObject;
                                    //open the subreport object and logon as for the general report 
                                    crSubreportDocument = crSubreportObject.OpenSubreport(crSubreportObject.SubreportName);
                                    crDatabase = crSubreportDocument.Database;
                                    crTables = crDatabase.Tables;
                                    foreach (CrystalDecisions.CrystalReports.Engine.Table aTable in crTables)
                                    {
                                        crTableLogOnInfo = aTable.LogOnInfo;
                                        crTableLogOnInfo.ConnectionInfo = crConnectionInfo;
                                        aTable.ApplyLogOnInfo(crTableLogOnInfo);
                                    }
                                }
                            }
                        }
                        cryRpt.SetDataSource(ds);

                        if (CoverPage == "False")
                        {
                            cryRpt.ReportDefinition.Sections["GroupHeaderSection7"].SectionFormat.EnableSuppress = true;
                            cryRpt.ReportDefinition.Sections["GroupHeaderSection5"].SectionFormat.EnableSuppress = true;
                        }

                        if (IndexPage == "False")
                        {
                            //cryRpt.ReportDefinition.Sections["Section4"].SectionFormat.EnableSuppress = true;
                            //cryRpt.ReportDefinition.Sections["ReportFooterSection3"].SectionFormat.EnableSuppress = true;
                            //cryRpt.ReportDefinition.Sections["ReportFooterSection1"].SectionFormat.EnableSuppress = true;
                            cryRpt.ReportDefinition.Sections["GroupFooterSection7"].SectionFormat.EnableSuppress = true;
                            cryRpt.ReportDefinition.Sections["GroupFooterSection8"].SectionFormat.EnableSuppress = true;
                            cryRpt.ReportDefinition.Sections["GroupFooterSection9"].SectionFormat.EnableSuppress = true;
                        }

                        if (IncludeSRs == "False")
                        {
                            cryRpt.ReportDefinition.Sections["DetailSection6"].SectionFormat.EnableSuppress = true;
                            cryRpt.ReportDefinition.Sections["ReportFooterSection1"].SectionFormat.EnableSuppress = true;
                            cryRpt.ReportDefinition.Sections["GroupFooterSection9"].SectionFormat.EnableSuppress = true;
                        }

                        if (IncludeAORs == "False")
                        {
                            cryRpt.ReportDefinition.Sections["DetailSection5"].SectionFormat.EnableSuppress = true;
                            cryRpt.ReportDefinition.Sections["DetailSection7"].SectionFormat.EnableSuppress = true;
                            cryRpt.ReportDefinition.Sections["ReportFooterSection3"].SectionFormat.EnableSuppress = true;
                            cryRpt.ReportDefinition.Sections["GroupFooterSection8"].SectionFormat.EnableSuppress = true;
                        }

                        if (IncludeSessionMetrics == "False")
                        {
                            cryRpt.ReportDefinition.Sections["GroupHeaderSection9"].SectionFormat.EnableSuppress = true;
                        }

                        using (Stream s = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                s.CopyTo(ms);
                                outFile = ms.ToArray();
                            }
                        }
                        Dictionary<string, string> result = new Dictionary<string, string>();
                        result = AOR.ContractCRReportInfo_Update(contractIDs, WTS_RESOURCEID);
                    }
                    break;
            }

            return true;
        }
    }
}