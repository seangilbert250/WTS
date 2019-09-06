using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace WTS.Reports
{
    /// <summary>
    /// Summary description for ReportQueue
    /// </summary>
    public class ReportQueue
    {
        private static ReportQueue instance = null;
        private static object lockObj = new object();

        public static ReportQueue Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        instance = new ReportQueue();
                    }
                }

                return instance;
            }
        }


        private ReportQueue()
        {

        }

        #region ReportQueue Logic

        public QueuedReport QueueReport(
            string guid,
            int WTS_RESOURCEID,
            int REPORT_TYPEID,
            DateTime scheduledDate,
            Dictionary<string, string> reportParameters,
            string reportName = null,
            string reportAssembly = null,
            string reportClass = null,
            string reportMethod = null
            )
        {
            QueuedReport rpt = new QueuedReport();
            rpt.Guid = guid ?? Guid.NewGuid().ToString();
            rpt.WTS_RESOURCEID = WTS_RESOURCEID;
            rpt.REPORT_TYPEID = REPORT_TYPEID;
            rpt.REPORT_STATUSID = (int)ReportStatusEnum.Queued;
            rpt.ReportName = string.IsNullOrWhiteSpace(reportName) ? ((ReportTypeEnum)REPORT_TYPEID).ToString() : reportName;
            rpt.ReportAssembly = reportAssembly;
            rpt.ReportClass = reportClass;
            rpt.ReportMethod = reportMethod;
            rpt.ScheduledDate = scheduledDate != DateTime.MinValue ? scheduledDate : DateTime.Now;
            rpt.ReportParameters = reportParameters;
            rpt.CreatedDate = DateTime.Now;
            rpt.CreatedBy = HttpContext.Current.User.Identity.Name;

            return SaveReport(rpt);
        }

        public QueuedReport GetReport(long reportQueueID, string guid, bool includeReportData, bool includeArchived = true)
        {
            QueuedReport rpt = null;

            List<QueuedReport> rpts = GetReports(reportQueueID, guid, 0, null, null, DateTime.MinValue, includeReportData, includeArchived, false);

            if (rpts != null && rpts.Count > 0) rpt = rpts[0];

            return rpt;
        }


        #endregion

        #region Data Access

        public QueuedReport SaveReport(QueuedReport rpt)
        {
            string procName = "ReportQueue_Save";

            try
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        
                        cmd.Parameters.Add("@ReportQueueID", SqlDbType.BigInt).Value = rpt.ReportQueueID;
                        cmd.Parameters["@ReportQueueID"].Direction = ParameterDirection.InputOutput;
                        cmd.Parameters.Add("@Guid", SqlDbType.VarChar).Value = rpt.Guid;
                        cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = rpt.WTS_RESOURCEID;
                        cmd.Parameters.Add("@REPORT_TYPEID", SqlDbType.Int).Value = rpt.REPORT_TYPEID;
                        cmd.Parameters.Add("@REPORT_STATUSID", SqlDbType.Int).Value = rpt.REPORT_STATUSID;
                        cmd.Parameters.Add("@ReportName", SqlDbType.VarChar).Value = rpt.ReportName ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@ReportAssembly", SqlDbType.VarChar).Value = rpt.ReportAssembly ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@ReportClass", SqlDbType.VarChar).Value = rpt.ReportClass ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@ReportMethod", SqlDbType.VarChar).Value = rpt.ReportMethod ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@ScheduledDate", SqlDbType.DateTime).Value = rpt.ScheduledDate != DateTime.MinValue ? rpt.ScheduledDate : (object)DBNull.Value;
                        cmd.Parameters.Add("@ExecutionStartDate", SqlDbType.DateTime).Value = rpt.ExecutionStartDate != DateTime.MinValue ? rpt.ExecutionStartDate : (object)DBNull.Value;
                        cmd.Parameters.Add("@CompletedDate", SqlDbType.DateTime).Value = rpt.CompletedDate != DateTime.MinValue ? rpt.CompletedDate : (object)DBNull.Value;

                        string json = rpt.ReportParameters != null && rpt.ReportParameters.Count > 0 ? JsonConvert.SerializeObject(rpt.ReportParameters, Newtonsoft.Json.Formatting.None) : null;
                        cmd.Parameters.Add("@ReportParameters", SqlDbType.NVarChar).Value = json ?? (object)DBNull.Value;

                        cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = rpt.CreatedBy ?? "SYSTEM";
                        cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = rpt.CreatedDate;
                        cmd.Parameters.Add("@Result", SqlDbType.VarChar).Value = rpt.Result ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Error", SqlDbType.VarChar).Value = rpt.Error ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@OutFileName", SqlDbType.VarChar).Value = rpt.OutFileName ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@OutFile", SqlDbType.VarBinary).Value = rpt.OutFile ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = rpt.Archive;
                        cmd.Parameters.Add("@OutFileSize", SqlDbType.BigInt).Value = rpt.OutFileSize > 0 ? rpt.OutFileSize : (object)DBNull.Value;

                        cmd.ExecuteNonQuery();

                        SqlParameter reportQueueID = cmd.Parameters["@ReportQueueID"];

                        rpt.ReportQueueID = (long)reportQueueID.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
            }

            return rpt;
        }

        public DataTable GetReportsDataTable(long reportQueueID, string guid, int WTS_RESOURCEID, List<int> reportTypes, List<int> reportStatuses, DateTime scheduledDateMax, bool includeReportData, bool includeArchived, bool includeAverages)
        {
            DataTable dt = null;

            string procName = "ReportQueue_Get";

            using (dt = new DataTable("Data"))
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReportQueueID", SqlDbType.BigInt).Value = reportQueueID;
                        cmd.Parameters.Add("@Guid", SqlDbType.VarChar).Value = guid ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = WTS_RESOURCEID;
                        cmd.Parameters.Add("@ReportTypes", SqlDbType.VarChar).Value = reportTypes != null && reportTypes.Count > 0 ? string.Join(",", reportTypes) : "0";
                        cmd.Parameters.Add("@ReportStatuses", SqlDbType.VarChar).Value = reportStatuses != null && reportStatuses.Count > 0 ? string.Join(",", reportStatuses) : "0";
                        cmd.Parameters.Add("@ScheduledDateMax", SqlDbType.DateTime).Value = scheduledDateMax != DateTime.MinValue ? scheduledDateMax : (object)DBNull.Value;
                        cmd.Parameters.Add("@IncludeReportData", SqlDbType.Bit).Value = includeReportData;
                        cmd.Parameters.Add("@IncludeArchived", SqlDbType.Bit).Value = includeArchived;
                        cmd.Parameters.Add("@IncludeAverages", SqlDbType.Bit).Value = includeAverages;


                        try
                        {
                            using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                            {
                                if (dr != null)
                                {
                                    dt.Load(dr);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogException(ex);
                            throw;
                        }
                    }
                }
            }

            return dt;
        }

        public List<QueuedReport> GetReports(long reportQueueID, string guid, int WTS_RESOURCEID, List<int> reportTypes, List<int> reportStatuses, DateTime scheduledDateMax, bool includeReportData, bool includeArchived, bool includeAverages)
        {
            List<QueuedReport> reports = new List<QueuedReport>();

            DataTable dt = GetReportsDataTable(reportQueueID, guid, WTS_RESOURCEID, reportTypes, reportStatuses, scheduledDateMax, includeReportData, includeArchived, includeAverages);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    QueuedReport rpt = new QueuedReport();

                    rpt.Load(row);

                    reports.Add(rpt);
                }
            }

            return reports;
        }

        public bool DeleteReport(long reportQueueID)
        {
            string procName = "ReportQueue_Delete";

            try
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ReportQueueID", SqlDbType.BigInt).Value = reportQueueID;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
                return false;
            }
            
            return true;
        }

        public void CleanReports(int maxHours, bool cleanErrors)
        {
            string procName = "ReportQueue_Clean";

            try
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@MaxHours", SqlDbType.Int).Value = maxHours;
                        cmd.Parameters.Add("@CleanErrors", SqlDbType.Bit).Value = cleanErrors;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
            }
        }

        public bool UpdateReportStatus(long reportQueueID, int reportStatusID, DateTime executionStartDate, DateTime completedDate, string result, string error, string outFileName, byte[] outFile, long outFileSize, bool archive, bool updateOutFile = true)
        {
            string procName = "ReportQueue_UpdateStatus";

            try
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ReportQueueID", SqlDbType.BigInt).Value = reportQueueID;
                        cmd.Parameters.Add("@REPORT_STATUSID", SqlDbType.Int).Value = reportStatusID;
                        cmd.Parameters.Add("@CompletedDate", SqlDbType.DateTime).Value = completedDate != DateTime.MinValue ? completedDate : (object)DBNull.Value;
                        cmd.Parameters.Add("@ExecutionStartDate", SqlDbType.DateTime).Value = executionStartDate != DateTime.MinValue ? executionStartDate : (object)DBNull.Value;
                        cmd.Parameters.Add("@Result", SqlDbType.NVarChar).Value = result ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Error", SqlDbType.NVarChar).Value = error ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@OutFileName", SqlDbType.VarChar).Value = outFileName ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@OutFile", SqlDbType.VarBinary).Value = outFile != null ? outFile : (object)DBNull.Value;
                        cmd.Parameters.Add("@OutFileSize", SqlDbType.BigInt).Value = outFileSize;
                        cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive;
                        cmd.Parameters.Add("@UpdateOutFile", SqlDbType.Bit).Value = updateOutFile;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
                return false;
            }

            return true;
        }

        #endregion

    }
}
 