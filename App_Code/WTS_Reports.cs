using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;

/// <summary>
/// Summary description for Class1
/// </summary>
/// 

#region WorkloadSummary 
public static class WTS_Reports
{
    public static DataSet getWorkloadSummary(String SummaryOverviewsSection1, String SummaryOverviewsSection2, String Organization, List<FilterObject> filters, string Delimeter, string backLog)
    {
        DataSet ds = new DataSet();
        // 13419 - 7
        string procName = "";
        if (backLog.ToLower() == "true")
            procName = "Report_WorkLoad_Backlog";
        else
            procName = "Report_WorkLoad";

        string parameter = string.Empty;
        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SummaryOverviewsSection1", SqlDbType.VarChar, 1000).Value = (String.IsNullOrEmpty(SummaryOverviewsSection1)) ? DBNull.Value : (object)SummaryOverviewsSection1;
                cmd.Parameters.Add("@SummaryOverviewsSection2", SqlDbType.VarChar, 1000).Value = (String.IsNullOrEmpty(SummaryOverviewsSection2)) ? DBNull.Value : (object)SummaryOverviewsSection2;
                cmd.Parameters.Add("@Organization", SqlDbType.VarChar, 50).Value = (String.IsNullOrEmpty(Organization)) ? "Folsom Dev" : (object)Organization;
                //cmd.Parameters.Add("@Organization", SqlDbType.VarChar, 50).Value = (String.IsNullOrEmpty(Organization)) ? DBNull.Value : (object)Organization;
                cmd.Parameters.Add("@Delimeter", SqlDbType.VarChar, 1000).Value = (String.IsNullOrEmpty(Delimeter)) ? "," : Delimeter;

                foreach (FilterObject filter in filters) //filter arguments for this procedure are named according to a convention. The filter.text is the name of the filter, and the .value is a comma deliminated string
                {
                    parameter = '@' + filter.text.ToLower().Replace(" ", "_").Replace(")", "").Replace("(", "") + "_filters"; //create parameter name using some string manipulation
                    cmd.Parameters.Add(parameter, SqlDbType.VarChar, 1000).Value = filter.value;
                }
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
                cn.Close();
                ds.Tables[2].TableName = "Summary Detail Task";
                ds.Tables[3].TableName = "Summary Detail Sub-Task";

                if (ds.Tables[1].Rows[0].ItemArray[0].ToString() != "No Summary Overview Parent")
                {
                    ds.Tables[1].TableName = "Summary Overview Parent";
                }
                if (ds.Tables[0].Rows[0].ItemArray[0].ToString() != "No Summary Overview Child")
                {
                    ds.Tables[0].TableName = "Summary Overview Child";
                }
                if (ds.Tables[4].Rows[0].ItemArray[0].ToString() != "No Dev")
                {
                    ds.Tables[4].TableName = "Developers";
                    ds.Tables[4].Columns.Remove("Sort Column"); //sort column is used for sorting. Not a meaningful output.
                }
                if (ds.Tables[5].Rows[0].ItemArray[0].ToString() != "No Bus")
                {
                    ds.Tables[5].TableName = "Business Team";
                    ds.Tables[5].Columns.Remove("Sort Column");
                }

                // 13419 - 5:
                ds.Tables[6].TableName = "Work Type counts";


                return ds;
            }
        }
    }

    public static string createReportParameters(string JSON, int reportID, string name, int userid, string userName, bool Process, ref int paramsID)
    {
        string error = "";
        string procName = "createReportParameters";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@paramsObject", SqlDbType.NChar, 8000).Value = JSON;
                cmd.Parameters.Add("@REPORTID", SqlDbType.Int).Value = reportID;
                cmd.Parameters.Add("@Name", SqlDbType.NChar, 8000).Value = name;
                cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@Process", SqlDbType.Bit).Value = Process;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NChar, 8000).Value = userName;
                cmd.Parameters.Add("@error", SqlDbType.NChar, 8000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                error = cmd.Parameters["@error"].Value.ToString();
                SqlParameter paramExists = cmd.Parameters["@error"];
                if (paramExists != null)
                {
                    error = cmd.Parameters["@error"].Value.ToString();
                }
                paramExists = cmd.Parameters["@ID"];
                if (paramExists != null)
                {
                    int.TryParse(cmd.Parameters["@ID"].Value.ToString(), out paramsID);
                }
            }
        }
        return error;
    }


    public static string get_Report_Parameters(ref string JSON, int paramsID)
    {
        string error = ""; //by default error returns with the value "Success"
        JSON = "";
        string procName = "get_Report_Parameters";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@paramsID", SqlDbType.Int).Value = paramsID;
                cmd.Parameters.Add("@JSON", SqlDbType.NChar, 8000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@error", SqlDbType.NChar, 8000).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                error = cmd.Parameters["@error"].Value.ToString();
                JSON = cmd.Parameters["@JSON"].Value.ToString();
            }
        }
        return error;
    }

    public static string update_Report_Parameters(string JSON, int paramsID, string updatedBy)
    {
        string error = ""; //by default error returns with the value "Success"
        string procName = "update_Report_Parameters";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@JSON", SqlDbType.NChar, 8000).Value = JSON;
                cmd.Parameters.Add("@paramsID", SqlDbType.Int).Value = paramsID;
                cmd.Parameters.Add("@UserName", SqlDbType.NChar, 8000).Value = updatedBy;
                cmd.Parameters.Add("@error", SqlDbType.NChar, 8000).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                error = cmd.Parameters["@error"].Value.ToString();
            }
        }
        return error;
    }

    public static string get_Report_Parameter_List(ref DataTable dt, int USERID, int reportID)
    {
        string error = ""; //by default error returns with the value "Success"
        dt = new DataTable();
        string procName = "get_Report_Parameter_List";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@REPORTID", SqlDbType.Int).Value = reportID;
                cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = USERID;
                cmd.Parameters.Add("@error", SqlDbType.NChar, 8000).Direction = ParameterDirection.Output;
                try
                {
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (dr != null && dr.HasRows)
                        {
                            dt.Load(dr);
                        }
                        else
                        {
                            dt = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtility.LogException(ex);
                    throw;
                }
                error = cmd.Parameters["@error"].Value.ToString();
            }
        }
        return error;
    }

    public static string deleteReportParameters(int paramsID)
    {
        string error = ""; //by default error returns with the value "Success"
        string procName = "deleteReportParameters";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@paramsID", SqlDbType.Int).Value = paramsID;
                cmd.Parameters.Add("@error", SqlDbType.NChar, 1000).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                error = cmd.Parameters["@error"].Value.ToString();
            }
        }
        return error;
    }
    public static int getReportIDbyName(string reportName)
    {
        string procName = "getReportIDbyName";
        int reportID;
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 8000).Value = reportName;
                cmd.Parameters.Add("@ReportID", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                reportID = (int)cmd.Parameters["@ReportID"].Value;
            }
        }
        return reportID;
    }

    #endregion WorkloadSummary 

    #region EmailHotlist
    public static string createHostlistConfig(string prodStatus, int techMin, int busMin, int techMax, int busMax, string status, string assigned, string recipients, string message, string name, ref int configID)
    {
        string error = "";
        string procName = "createHotlistConfig";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@prodStatus", SqlDbType.NChar, 8000).Value = prodStatus;
                cmd.Parameters.Add("@techMin", SqlDbType.Int).Value = techMin;
                cmd.Parameters.Add("@busMin", SqlDbType.Int).Value = busMin;
                cmd.Parameters.Add("@techMax", SqlDbType.Int).Value = techMax;
                cmd.Parameters.Add("@busMax", SqlDbType.Int).Value = busMax;
                cmd.Parameters.Add("@status", SqlDbType.NChar, 8000).Value = status;
                cmd.Parameters.Add("@assigned", SqlDbType.NChar, 8000).Value = assigned;
                cmd.Parameters.Add("@recipients", SqlDbType.NChar, 8000).Value = recipients;
                cmd.Parameters.Add("@message", SqlDbType.NChar, 8000).Value = message;
                cmd.Parameters.Add("@Name", SqlDbType.NChar, 8000).Value = name;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NChar, 8000).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@error", SqlDbType.NChar, 8000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                error = cmd.Parameters["@error"].Value.ToString();
                SqlParameter paramExists = cmd.Parameters["@error"];
                if (paramExists != null)
                {
                    error = cmd.Parameters["@error"].Value.ToString();
                }
                paramExists = cmd.Parameters["@ID"];
                if (paramExists != null)
                {
                    int.TryParse(cmd.Parameters["@ID"].Value.ToString(), out configID);
                }
            }
        }
        return error;
    }
    public static DataTable HotlistConfig_Get(int configID)
    {
        DataTable dt = new DataTable();
        string procName = "HostConfig_Get";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Email_Hotlist_ConfigID", SqlDbType.Int).Value = configID;
                try
                {
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (dr != null && dr.HasRows)
                        {
                            dt.Load(dr);
                        }
                        else
                        {
                            dt = null;
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
        return dt;
    }

    public static DataTable GetSRConfig()
    {
        DataTable dt = new DataTable();
        string procName = "SRRecipientList_Get";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (dr != null && dr.HasRows)
                        {
                            dt.Load(dr);
                        }
                        else
                        {
                            dt = null;
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
        return dt;
    }
    public static void SRRecipientSave(int WTSID, int receiveSREMail, int includeInSRCounts)
    {
        string procName = "SRRecipientSave_Save";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@WTSID", SqlDbType.Int).Value = WTSID;
                cmd.Parameters.Add("@receiveSREMail", SqlDbType.Int).Value = receiveSREMail;
                cmd.Parameters.Add("@includeInSRCounts", SqlDbType.Int).Value = includeInSRCounts;

                cmd.ExecuteNonQuery();
            }
        }
    }

    public static DataTable GetDashboardData()
    {
        DataTable dt = new DataTable();
        string procName = "Dashboard_Get";

        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                try
                {
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (dr != null && dr.HasRows)
                        {
                            dt.Load(dr);
                        }
                        else
                        {
                            dt = null;
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
        return dt;
    }

    public static DataTable GetDashboardDataWorkType()
    {
        DataTable dt = new DataTable();
        string procName = "DashboardWorkTypeCounts_Get";

        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                try
                {
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (dr != null && dr.HasRows)
                        {
                            dt.Load(dr);
                        }
                        else
                        {
                            dt = null;
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
        return dt;
    }
    public static DataTable HostlistConfigList_Get()
    {
        DataTable dt = new DataTable();
        string procName = "HostlistConfigList_Get";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (dr != null && dr.HasRows)
                        {
                            dt.Load(dr);
                        }
                        else
                        {
                            dt = null;
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
        return dt;
    }

    public static string HotlistConfig_Delete(int configID)
    {
        string error = ""; //by default error returns with the value "Success"
        string procName = "HotlistConfig_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Email_Hotlist_ConfigID", SqlDbType.Int).Value = configID;
                cmd.Parameters.Add("@error", SqlDbType.NChar, 1000).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                error = cmd.Parameters["@error"].Value.ToString();
            }
        }
        return error;
    }
    public static string Hostlist_Config_SetActive(int configID)
    {
        string procName = "Hostlist_Config_SetActive";
        string error = "";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@error", SqlDbType.NChar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Email_Hostlist_ConfigID", SqlDbType.Int).Value = configID;
                cmd.ExecuteNonQuery();
                error = cmd.Parameters["@error"].Value.ToString();
            }
        }
        return error;
    }

    public static void sendHotlistOnDemand(string prodStatus, int techMin, int busMin, int techMax, int busMax, string status, string assigned, string recipients, string message)
    {
        string procName = "Email_Hotlist_OnDemand";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandTimeout = 240;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ProdStatus", SqlDbType.NChar, 8000).Value = prodStatus;
                cmd.Parameters.Add("@TechMin", SqlDbType.Int).Value = techMin;
                cmd.Parameters.Add("@BusMin", SqlDbType.Int).Value = busMin;
                cmd.Parameters.Add("@TechMax", SqlDbType.Int).Value = techMax;
                cmd.Parameters.Add("@BusMax", SqlDbType.Int).Value = busMax;
                cmd.Parameters.Add("@activeStatus", SqlDbType.NChar, 8000).Value = status;
                cmd.Parameters.Add("@activeAssigned", SqlDbType.NChar, 8000).Value = assigned;
                cmd.Parameters.Add("@activeRecipients", SqlDbType.NChar, 8000).Value = recipients;
                cmd.Parameters.Add("@message", SqlDbType.NChar, 8000).Value = message;

                cmd.ExecuteNonQuery();
            }
        }
        return;
    }

    public static void TESTsendHotlistOnDemand(string prodStatus, int techMin, int busMin, int techMax, int busMax, string status, string assigned, string recipients, string message)
    {
        string procName = "Email_Hotlist_Steve";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandTimeout = 240;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }
        return;
    }

    public static void TESTSendSrReport()
    {
        string procName = "EMailSRReport";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandTimeout = 240;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }
        return;
    }


    #endregion
}