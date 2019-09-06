using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace WTS.Reports
{

    public class QueuedReport
    {
        public long ReportQueueID { get; set; }
        public string Guid { get; set; }
        public int WTS_RESOURCEID { get; set; }
        public int REPORT_TYPEID { get; set; }
        public int REPORT_STATUSID { get; set; }
        public string ReportName { get; set; }
        public string ReportAssembly { get; set; }
        public string ReportClass { get; set; }
        public string ReportMethod { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime ExecutionStartDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public Dictionary<string, string> ReportParameters { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string OutFileName { get; set; }
        public byte[] OutFile { get; set; }
        public bool Archive { get; set; }
        public long OutFileSize { get; set; }
        public decimal AverageTime { get; set; } 

        // extended data
        public string ReportStatus { get; set; }
        public string ReportType { get; set; }
        public string ResourceFirstName { get; set; }
        public string ResourceLastName { get; set; }
        public string ResourceEmail { get; set; }
        public bool OutFileExists { get; set; }

        public QueuedReport()
        {
        
        }

        public void Load(DataRow dr)
        {
            ReportQueueID = (long)dr["ReportQueueID"];
            Guid = (string)dr["Guid"];
            WTS_RESOURCEID = (int)dr["WTS_RESOURCEID"];
            REPORT_TYPEID = (int)dr["REPORT_TYPEID"];
            REPORT_STATUSID = (int)dr["REPORT_STATUSID"];
            ReportName = dr["ReportName"] != DBNull.Value ? (string)dr["ReportName"] : null;
            ReportAssembly = dr["ReportAssembly"] != DBNull.Value ? (string)dr["ReportAssembly"] : null;
            ReportClass = dr["ReportClass"] != DBNull.Value ? (string)dr["ReportClass"] : null;
            ReportMethod = dr["ReportMethod"] != DBNull.Value ? (string)dr["ReportMethod"] : null;
            ScheduledDate = (DateTime)dr["ScheduledDate"];
            CompletedDate = dr["CompletedDate"] != DBNull.Value ? (DateTime)dr["CompletedDate"] : DateTime.MinValue;
            ExecutionStartDate = dr["ExecutionStartDate"] != DBNull.Value ? (DateTime)dr["ExecutionStartDate"] : DateTime.MinValue;

            string json = dr["ReportParameters"] != DBNull.Value ? (string)dr["ReportParameters"] : null;
            ReportParameters = string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            CreatedBy = (string)dr["CreatedBy"];
            CreatedDate = (DateTime)dr["CreatedDate"];
            Result = dr["Result"] != DBNull.Value ? (string)dr["Result"] : null;
            Error = dr["Error"] != DBNull.Value ? (string)dr["Error"] : null;
            OutFileName = dr["OutFileName"] != DBNull.Value ? (string)dr["OutFileName"] : null;
            OutFile = dr["OutFile"] != DBNull.Value ? (byte[])dr["OutFile"] : null;
            Archive = (bool)dr["Archive"];
            OutFileSize = dr["OutFileSize"] != DBNull.Value ? (long)dr["OutFileSize"] : 0;
            AverageTime = dr["AvgTime"] != DBNull.Value ? (decimal)dr["AvgTime"] : 0;

            // extended data
            ReportStatus = dr["ReportStatus"] != DBNull.Value ? (string)dr["ReportStatus"] : null;
            ReportType = dr["ReportType"] != DBNull.Value ? (string)dr["ReportType"] : null;
            ResourceFirstName = dr["FirstName"] != DBNull.Value ? (string)dr["FirstName"] : null;
            ResourceLastName = dr["LastName"] != DBNull.Value ? (string)dr["LastName"] : null;
            ResourceEmail = dr["Email"] != DBNull.Value ? (string)dr["Email"] : null;
            OutFileExists = dr["OutFileExists"] != DBNull.Value ? (int) dr["OutFileExists"] == 1 : false;
        }

        public string RunReport()
        {
            string errors = null;

            ExecutionStartDate = DateTime.Now;

            ReportQueue.Instance.UpdateReportStatus(ReportQueueID, (int)ReportStatusEnum.Executing, ExecutionStartDate, DateTime.MinValue, null, null, null, null, 0, false);

            try
            {
                if (REPORT_TYPEID == (int)ReportTypeEnum.External)
                {
                }
                else
                {
                    IReport rpt = ReportFactory.CreateReport(REPORT_TYPEID);

                    if (rpt != null)
                    {
                        string outFileName = null;
                        byte[] outFile = null;

                        if (rpt.ExecuteReport(ReportParameters, WTS_RESOURCEID, ref errors, ref outFileName, ref outFile))
                        {
                            ReportQueue.Instance.UpdateReportStatus(ReportQueueID, (int)ReportStatusEnum.Complete, ExecutionStartDate, DateTime.Now, ReportStatusEnum.Complete.ToString(), errors, outFileName, outFile, outFile != null ? outFile.Length : 0, false);
                        }
                        else
                        {
                            errors = "Error running report. " + errors;
                            ReportQueue.Instance.UpdateReportStatus(ReportQueueID, (int)ReportStatusEnum.Error, ExecutionStartDate, DateTime.Now, ReportStatusEnum.Error.ToString(), "Error running report. " + errors, null, null, 0, false);
                        }
                    }
                    else
                    {
                        errors = "Invalid report type (" + REPORT_TYPEID + ").";
                        ReportQueue.Instance.UpdateReportStatus(ReportQueueID, (int)ReportStatusEnum.Error, ExecutionStartDate, DateTime.Now, ReportStatusEnum.Error.ToString(), "Invalid report type (" + REPORT_TYPEID + ").", null, null, 0, false);
                    }
                }
            }
            catch (Exception ex)
            {
                errors = ex.Message + " " + ex.StackTrace;
                ReportQueue.Instance.UpdateReportStatus(ReportQueueID, (int)ReportStatusEnum.Error, ExecutionStartDate, DateTime.Now, ReportStatusEnum.Error.ToString(), ex.Message + " " + ex.StackTrace, null, null, 0, false);
            }

            return errors;
        }
    }
}