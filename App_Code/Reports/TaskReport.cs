using System;
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
    /// Summary description for TaskReport
    /// </summary>
    public class TaskReport : ReportBase
    {
        public TaskReport()
        {
            
        }

        public override Dictionary<string, string> GetFilterFields()
        {
            return new Dictionary<string, string>()
            {
                { "Header", "Reporting Options" },
                { "AOR", "AOR" },
                { "Product Version", "Product Version" },
                { "Resource", "Resource" },
                { "Status", "Status" },
                { "System(Task)", "System(Task)" },
                { "Work Area", "Work Area" },
            };
        }

        public override string GetPageTitle()
        {
            return "Task Report";
        }

        public override string GetDefaultFilters()
        {
            return "Product Version|Product Version|40|18.1`" +
                    "System(Task)|System(Task)|31|R&D WTS`";
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
            string aorIDs = reportParameters.ContainsKey("AOR") ? reportParameters["AOR"] : "";
            string productVersionIDs = reportParameters.ContainsKey("Product Version") ? reportParameters["Product Version"] : "";
            string resourceIDs = reportParameters.ContainsKey("Resource") ? reportParameters["Resource"] : "";
            string statusIDs = reportParameters.ContainsKey("Status") ? reportParameters["Status"] : "";
            string systemIDs = reportParameters.ContainsKey("System(Task)") ? reportParameters["System(Task)"] : "";
            string workAreaIDs = reportParameters.ContainsKey("Work Area") ? reportParameters["Work Area"] : "";

            switch (type)
            {
                case "pdf":
                    DataSet ds = Workload.TaskReport_Get(aorIDs, productVersionIDs, resourceIDs, statusIDs, systemIDs, workAreaIDs, title);

                    if (ds != null)
                    {
                        #region Task
                        DataTable dtTask = ds.Tables["Task"];
                        #endregion

                        outFileName = "Task_Report.pdf";
                        ReportDocument cryRpt = new ReportDocument();
                        string cnString = WTSCommon.WTS_ConnectionString;
                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cnString);
                        string user = builder.UserID;
                        string pass = builder.Password;
                        string server = builder.DataSource;
                        string database = builder.InitialCatalog;

                        cryRpt.Load(System.Web.Hosting.HostingEnvironment.MapPath(@"~/Reports/Task.rpt"));
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
                        Database crDatabase = cryRpt.Database;
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

                        using (Stream s = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                s.CopyTo(ms);
                                outFile = ms.ToArray();
                            }
                        }
                    }
                    break;
            }

            return true;
        }
    }
}
