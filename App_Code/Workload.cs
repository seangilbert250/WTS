﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web;

using System.Net.Mail;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data.SqlTypes;

public sealed class Workload
{


    public static DataTable SR_List_Get(int workRequestID = 0, int showArchived = 0, int columnListOnly = 0, dynamic filters = null)
    {
        string procName = "SR_List_Get";

        dynamic fields = (Dictionary<string, object>)HttpContext.Current.Session["filters_Work"];
        SqlParameter[] sps;// = Filtering.GetWorkFilter_SqlParams(fields, string.Empty);

        if (filters == null || ((Dictionary<string, object>)filters).Count == 0)
        {
            sps = Filtering.GetWorkFilter_SqlParamsArray(fields, "");
        }
        else
        {
            List<SqlParameter> spList = Filtering.MergeWorkFilter_SqlParams(fields, filters, "");
            sps = spList.ToArray();
        }

        using (DataTable dt = new DataTable("Workload"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@WORKREQUESTID", SqlDbType.Int).Value = workRequestID == 0 ? (object)DBNull.Value : workRequestID;
                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived == 0 ? (object)DBNull.Value : showArchived;
                    cmd.Parameters.Add("@ColumnListOnly", SqlDbType.Int).Value = columnListOnly;


                    //existing filters - parse dynamic fields into SQL Parameter objects
                    if (sps != null && sps.Length > 0)
                    {
                        cmd.Parameters.AddRange(sps);
                    }

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    /// <summary>
    /// Load WTS Workload Items that are Sustainment Request item type
    /// </summary>
    /// <param name="includeArchive">true to include Archived workitems</param>
    /// <returns></returns>
    public static DataTable WTS_SR_WorkItemList_Get(bool includeArchive = false)
    {
        string procName = "SR_WorkItemList_Get";

        using (DataTable dt = new DataTable("SR"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Bit).Value = includeArchive ? 0 : 1;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    public static DataTable Workload_Crosswalk_Get(string parentFields = "WORKREQUEST,WorkType,Priority"
        , string valueFields = "Status"
        , string orderFields = "WORKREQUESTID DESC"
        , int showArchived = 0
        , int columnListOnly = 0
        , bool myData = false
        , string SelectedStatus = ""
        , string SelectedAssigned = ""
        )
    {
        string procName = "QM_Workload_Crosswalk_Grid";

        Regex isNumbers = new Regex(@"^\d+(,\d+)*$|^$"); //validate input is comma seperated integers or empty string

        if (!isNumbers.IsMatch(SelectedStatus) || !isNumbers.IsMatch(SelectedAssigned))
        {
            Exception e = new Exception("Error, invalid input");
            LogUtility.LogException(e);
            throw e;
        }

        using (DataTable dt = new DataTable("Workload"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@ParentFields", SqlDbType.NVarChar).Value = parentFields;
                    cmd.Parameters.Add("@ValueFields", SqlDbType.NVarChar).Value = valueFields;
                    cmd.Parameters.Add("@OrderFields", SqlDbType.NVarChar).Value = orderFields;
                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived;
                    cmd.Parameters.Add("@ColumnListOnly", SqlDbType.Int).Value = columnListOnly;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.NVarChar).Value = myData ? UserManagement.GetUserId_FromUsername().ToString() : "";
                    cmd.Parameters.Add("@SelectedStatus", SqlDbType.NVarChar).Value = SelectedStatus;
                    cmd.Parameters.Add("@SelectedAssigned", SqlDbType.NVarChar).Value = SelectedAssigned;

                    cmd.Parameters.Add("@debug", SqlDbType.Bit).Value = 0;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    public static DataTable QM_Workload_Crosswalk_Multi_Level_Grid(
        XmlDocument level,
        XmlDocument filter,
        string qfStatus = "",
        string qfAffiliated = "",
        bool qfBusinessReview = false,
        bool myData = true
        )
    {
        string procName = "QM_Workload_Crosswalk_Multi_Level_Grid";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name.ToString().Replace("'", "''");
                    cmd.Parameters.Add("@Level", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(level.InnerXml, XmlNodeType.Document, null));
                    cmd.Parameters.Add("@Filter", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(filter.InnerXml, XmlNodeType.Document, null));
                    cmd.Parameters.Add("@QFStatus", SqlDbType.NVarChar).Value = qfStatus;
                    cmd.Parameters.Add("@QFAffiliated", SqlDbType.NVarChar).Value = qfAffiliated;
                    cmd.Parameters.Add("@QFBusinessReview", SqlDbType.NVarChar).Value = qfBusinessReview == true ? "1" : "0";
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.NVarChar).Value = myData ? UserManagement.GetUserId_FromUsername().ToString() : "0";
                    cmd.Parameters.Add("@Debug", SqlDbType.Bit).Value = 0;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    public static bool QM_Workload_Crosswalk_Multi_Level_Grid_Save(XmlDocument changes)
    {
        bool saved = false;
        string procName = "QM_Workload_Crosswalk_Multi_Level_Grid_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Changes", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(changes.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name.ToString().Replace("'", "''");
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
            }
        }

        return saved;
    }

    public static DataTable Workload_Metrics_Get(string SelectedStatus, string SelectedUsers, string metricType, int showArchived = 0, bool myData = false)
    {
        string procName = "Workload_Metrics_Get";
        using (DataTable dt = new DataTable("WorkloadMetrics"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = metricType;
                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.NVarChar).Value = myData ? UserManagement.GetUserId_FromUsername().ToString() : "";
                    cmd.Parameters.Add("@SelectedStatus", SqlDbType.NVarChar).Value = SelectedStatus;
                    cmd.Parameters.Add("@SelectedAssigned", SqlDbType.NVarChar).Value = SelectedUsers;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }
    public static DataTable Workload_Sub_Metrics_Get(string SelectedStatus, string SelectedUsers, string metricType, int showArchived = 0, bool myData = false)
    {
        string procName = "Workload_Sub_Metrics_Get";  // 11-23-2016 - New stored procedure.
        using (DataTable dt = new DataTable("WorkloadMetrics"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = metricType;
                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.NVarChar).Value = myData ? UserManagement.GetUserId_FromUsername().ToString() : "";
                    cmd.Parameters.Add("@SelectedStatus", SqlDbType.NVarChar).Value = SelectedStatus;
                    cmd.Parameters.Add("@SelectedAssigned", SqlDbType.NVarChar).Value = SelectedUsers;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }


    public static DataTable WorkItem_History_Get(string workItemID = ""
        , string itemUpdateType = ""
        , string fieldChanged = ""
        , string createdBy = "")
    {
        string procName = "WorkItem_History_Get";

        using (DataTable dt = new DataTable("WorkItemHistory"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WORKITEMID", SqlDbType.NVarChar).Value = workItemID;
                    cmd.Parameters.Add("@ITEM_UPDATETYPE", SqlDbType.NVarChar).Value = itemUpdateType;
                    cmd.Parameters.Add("@FieldChanged", SqlDbType.NVarChar).Value = fieldChanged;
                    cmd.Parameters.Add("@CREATEDBY", SqlDbType.NVarChar).Value = createdBy;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    public static DataTable WorkItem_Task_History_Get(string workItem_TaskID = ""
        , string itemUpdateType = ""
        , string fieldChanged = ""
        , string createdBy = "")
    {
        string procName = "WorkItem_Task_History_Get";

        using (DataTable dt = new DataTable("WorkItemTaskHistory"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WORKITEM_TASKID", SqlDbType.NVarChar).Value = workItem_TaskID;
                    cmd.Parameters.Add("@ITEM_UPDATETYPE", SqlDbType.NVarChar).Value = itemUpdateType;
                    cmd.Parameters.Add("@FieldChanged", SqlDbType.NVarChar).Value = fieldChanged;
                    cmd.Parameters.Add("@CREATEDBY", SqlDbType.NVarChar).Value = createdBy;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    public static bool WorkItem_History_Delete(int WorkItem_HistoryID)
    {
        bool deleted = false;
        string procName = "WorkItem_History_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkItem_HistoryID", SqlDbType.Int).Value = WorkItem_HistoryID;
                cmd.Parameters.Add("@Exists", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@HasDependencies", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@Deleted"];

                if (paramDeleted != null) bool.TryParse(paramDeleted.Value.ToString(), out deleted);
            }
        }

        return deleted;
    }

    public static bool WorkItem_Task_History_Delete(int WORKITEM_TASK_HISTORYID)
    {
        bool deleted = false;
        string procName = "WorkItem_Task_History_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WORKITEM_TASK_HISTORYID", SqlDbType.Int).Value = WORKITEM_TASK_HISTORYID;
                cmd.Parameters.Add("@Exists", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@HasDependencies", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@Deleted"];

                if (paramDeleted != null) bool.TryParse(paramDeleted.Value.ToString(), out deleted);
            }
        }

        return deleted;
    }

    public static DataTable WorkItemsAndSubTasks_Get(bool myData = false)
    {
        string procName = "WorkItemsAndSubTasks_Get";

        using (DataTable dt = new DataTable("WorkItemsAndSubTasks"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.NVarChar).Value = myData ? UserManagement.GetUserId_FromUsername().ToString() : "";

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    public static DataSet WorkItem_Task_Metrics_Get(int workItemTaskID)
    {
        return WTSData.GetDataSetFromStoredProcedure("WorkItem_Task_Metrics_Get", new string[] { "RQMTS", "ACCEPTED", "CRITICALITY", "STAGE", "STATUS", "DEFECTS", "DEFECTSTATUS", "DEFECTIMPACT", "DEFECTSTAGE" }, new SqlParameter[] {
            new SqlParameter("@WorkItem_TaskID", workItemTaskID)
        });
    }

    public static DataSet Workload_Email_Get(int workItemID = 0, int workItemTaskID = 0)
    {
        DataSet ds = new DataSet();
        string procName = "Workload_Email_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WORKITEMID", SqlDbType.Int).Value = workItemID;
                cmd.Parameters.Add("@WORKITEM_TASKID", SqlDbType.Int).Value = workItemTaskID;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "WorkItem");
                da.TableMappings.Add("Table1", "WorkItemHistory");
                da.TableMappings.Add("Table2", "WorkItemTaskHistory");
                da.TableMappings.Add("Table3", "Comments");
                da.TableMappings.Add("Table4", "WorkItemTask");
                da.TableMappings.Add("Table5", "Attachments");

                da.Fill(ds);
            }
        }

        return ds;
    }

    public static void SendWorkloadEmail(string type, bool newItem, int id)
    {
        List<String> toEmails = new List<string>();
        string subject = string.Empty;
        string body = string.Empty;
        string from = string.Empty;
        string errorMessage = string.Empty;

        try
        {
            if (WTSConfiguration.Environment == "PRODUCTION")
            {
                DataSet ds = Workload.Workload_Email_Get((type == "WorkItem" ? id : 0), (type == "WorkItemTask" ? id : 0));
                DataTable dt = null;

                if (ds != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[type];
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    string title = "", recipients = "", workloadNum = "", taskNum = "";
                    List<MailAddress> toAddresses = new List<MailAddress>();
                    MailAddress addr, fromAddr;
                    from = WTSConfiguration.ErrorEmailFrom;
                    fromAddr = new MailAddress(from, WTSConfiguration.ErrorEmailFromName);

                    try
                    {
                        title = (from row in dt.AsEnumerable()
                                 where row.Field<string>("Field").Trim().ToUpper() == "TITLE"
                                 select row.Field<string>("Value")).FirstOrDefault();
                    }
                    catch (Exception) { }

                    try
                    {
                        recipients = (from row in dt.AsEnumerable()
                                      where row.Field<string>("Field").Trim().ToUpper() == "RECIPIENTS"
                                      select row.Field<string>("Value")).FirstOrDefault().Trim().ToLower();
                    }
                    catch (Exception) { }

                    string[] to = recipients.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    to = to.Distinct().ToArray();
                    WTS_User u = new WTS_User();
                    u.Load(UserManagement.GetUserId_FromUsername());
                    string excludeEmail = u.Email.Trim().ToLower();
                    foreach (string s in to)
                    {
                        if (s.Trim().ToLower() != excludeEmail)
                        {
                            addr = new MailAddress(s);
                            toAddresses.Add(addr);
                            toEmails.Add(s);
                        }
                    }

                    if (type == "WorkItemTask")
                    {
                        workloadNum = (from row in dt.AsEnumerable()
                                       where row.Field<string>("Field").Trim().ToUpper() == "WORKLOAD #"
                                       select row.Field<string>("Value")).FirstOrDefault().Trim().ToUpper();

                        taskNum = (from row in dt.AsEnumerable()
                                   where row.Field<string>("Field").Trim().ToUpper() == "TASK #"
                                   select row.Field<string>("Value")).FirstOrDefault().Trim().ToUpper();
                    }

                    subject = (type == "WorkItemTask" ? "Sub-Task" : "Task") + " # " + (type == "WorkItemTask" ? workloadNum + " - " + taskNum : id.ToString()) + " was " + (newItem ? "added - " : "updated - ") + title;

                    StringBuilder html = new StringBuilder(@"<style type=""text/css"">div,table {font-family: Arial; font-size: 12px;}</style>");
                    html.Append("<div>");

                    if (type == "WorkItemTask")
                    {
                        string workloadTitle = "";
                        try
                        {
                            workloadTitle = (from row in dt.AsEnumerable()
                                             where row.Field<string>("Field").Trim().ToUpper() == "WORKLOAD TITLE"
                                             select row.Field<string>("Value")).FirstOrDefault();
                        }
                        catch (Exception) { }

                        html.Append("<b>Task #: </b>" + workloadNum + "<br />");
                        html.Append("<b>Title: </b>" + workloadTitle + "<br /><br />");
                    }

                    html.Append("<b>" + (type == "WorkItemTask" ? "Sub-Task" : "Task") + " #: </b>" + (type == "WorkItemTask" ? workloadNum + " - " + taskNum : id.ToString()) + "<br />");
                    html.Append("<b>Title: </b>" + title + "");
                    html.Append(@"<table border=""1"" cellpadding=""2"" cellspacing=""0"">");
                    string[] exclude = { "TITLE", "RECIPIENTS", "WORKLOAD #", "WORKLOAD TITLE", "TASK #" };
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!exclude.Contains(dr["Field"].ToString().Trim().ToUpper()))
                        {
                            html.Append("<tr><td>" + dr["Field"] + "</td><td>" + (dr["Value"].ToString() == "" ? "&nbsp;" : Uri.UnescapeDataString(dr["Value"].ToString())) + "</td></tr>");
                        }
                    }
                    html.Append("</table><br />");

                    DataTable dtHistory = ds.Tables[type + "History"];
                    if (dtHistory != null && dtHistory.Rows.Count > 0)
                    {
                        html.Append(@"<table border=""1"" cellpadding=""2"" cellspacing=""0"">");
                        foreach (DataRow dr in dtHistory.Rows)
                        {
                            html.Append("<tr><td>Changed " + dr["FieldChanged"] + (dr["OldValue"].ToString().Length > 0 ? " from " + dr["OldValue"] : "") + " to " + dr["NewValue"] + "</td><td>" + dr["CREATEDBY"] + " - " + dr["CREATEDDATE"] + "</td></tr>");
                        }
                        html.Append("</table><br />");
                    }

                    if (type == "WorkItem")
                    {
                        DataTable dtComments = ds.Tables["Comments"];
                        if (dtComments != null && dtComments.Rows.Count > 0)
                        {
                            html.Append(@"<table border=""1"" cellpadding=""2"" cellspacing=""0"">");
                            foreach (DataRow dr in dtComments.Rows)
                            {
                                html.Append("<tr><td>Comment By " + dr["CREATEDBY"] + " on " + dr["CREATEDDATE"] + "<br /><br />" + dr["COMMENT_TEXT"] + "</td></tr>");
                            }
                            html.Append("</table><br />");
                        }

                        DataTable dtAttachments = ds.Tables["Attachments"];
                        if (dtAttachments != null && dtAttachments.Rows.Count > 0)
                        {
                            html.Append("<b>Attachments</b>");
                            html.Append(@"<table border=""1"" cellpadding=""2"" cellspacing=""0"">");
                            foreach (DataRow dr in dtAttachments.Rows)
                            {
                                html.Append("<tr><td>" + dr["FileName"] + "</td><td>" + dr["AttachmentType"] + "</td><td>" + dr["Title"] + "</td><td>" + dr["Description"] + "</td><td>" + dr["CREATEDBY"] + " - " + dr["CREATEDDATE"] + "</td></tr>");
                            }
                            html.Append("</table><br />");
                        }
                    }

                    html.Append("</div>");

                    body = html.ToString();

                    if (toAddresses.Count > 0)
                    {
                        Thread email = new Thread(delegate ()
                        {
                            bool success = WTSUtility.Send_Email(toAddresses, null, null, subject, body, fromAddr);

                            errorMessage = success ? "" : "Error: WTSUtility.Send_Email";
                        });

                        email.IsBackground = true;
                        email.Start();
                    }
                }
            }
        }
        catch (Exception ex) {
            errorMessage = ex.Message;
        }

        try
        {
            LogUtility.LogEmail(toAddresses: String.Join(",", toEmails), ccAddresses: "FolsomWorkload@infintech.com", bccAddresses: "", subject: subject, body: body, fromAddress: from, procedureUsed: "Workload.SendWorkloadEmail", errorMessage: errorMessage);
        }
        catch (Exception) { }
    }

    public static bool ItemExists(int itemID, int taskNumber, string type)
    {
        string funcName = "SELECT dbo.ItemExists(@ItemID, @TaskNumber, @Type)";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(funcName, cn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ItemID", itemID);
                cmd.Parameters.AddWithValue("@TaskNumber", taskNumber);
                cmd.Parameters.AddWithValue("@Type", type);

                try
                {
                    return (bool)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    LogUtility.LogException(ex);
                    throw;
                }
            }
        }
    }

    public static bool ApplySortToDB(int OpenerPageID, string OpenerPage, string sortValues)
    {
        string procName = "SortOrder_Apply";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandTimeout = 240;

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@GridNameID", SqlDbType.Int).Value = OpenerPageID;
                cmd.Parameters.Add("@GridName", SqlDbType.NVarChar).Value = OpenerPage;
                cmd.Parameters.Add("@sortValues", SqlDbType.NVarChar).Value = sortValues;

                cmd.ExecuteNonQuery();
            }
        }
        return true;
    }

    public static string GetSortValuesFromDB(int OpenerPageID, string OpenerPage)
    {
        string procName = "SortOrder_Get";
        string sortValues = "";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandTimeout = 240;

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@GridNameID", SqlDbType.Int).Value = OpenerPageID;
                cmd.Parameters.Add("@GridName", SqlDbType.NVarChar).Value = OpenerPage;

                sortValues = (string)cmd.ExecuteScalar();
            }
        }
        return sortValues;
    }
    public static string DeleteSort(int OpenerPageID, string OpenerPage)
    {
        string procName = "SortOrder_Remove";
        string sortValues = "";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandTimeout = 240;

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@GridNameID", SqlDbType.Int).Value = OpenerPageID;
                cmd.Parameters.Add("@GridName", SqlDbType.NVarChar).Value = OpenerPage;

                cmd.ExecuteNonQuery();
            }
        }
        return sortValues;

    }

    public static string SortValuesDelete(string OpenerPage)
    {
        string procName = "SortOrder_Delete";
        string sortValues = "";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandTimeout = 240;

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@GridName", SqlDbType.NVarChar).Value = OpenerPage;

                cmd.ExecuteNonQuery();
            }
        }
        return sortValues;

    }

    public static bool EmailHotlist()
    {
        string procName = "Email_Hotlist";
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

        return true;
    }

    public static bool SRHotlist()
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

        return true;
    }

    public static DataSet TaskReport_Get(string AORIDs,
        string ProductVersionIDs,
        string ResourceIDs,
        string StatusIDs,
        string SystemIDs,
        string WorkAreaIDs,
        string Title)
    {
        DataSet ds = new DataSet();
        string procName = "TaskReport_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORIDs", SqlDbType.NVarChar).Value = AORIDs;
                cmd.Parameters.Add("@ProductVersionIDs", SqlDbType.NVarChar).Value = ProductVersionIDs;
                cmd.Parameters.Add("@ResourceIDs", SqlDbType.NVarChar).Value = ResourceIDs;
                cmd.Parameters.Add("@StatusIDs", SqlDbType.NVarChar).Value = StatusIDs;
                cmd.Parameters.Add("@SystemIDs", SqlDbType.NVarChar).Value = SystemIDs;
                cmd.Parameters.Add("@WorkAreaIDs", SqlDbType.NVarChar).Value = WorkAreaIDs;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Title;
                cmd.Parameters.Add("@Debug", SqlDbType.Bit).Value = 0;
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "Parameter");
                da.TableMappings.Add("Table1", "Task");
                da.TableMappings.Add("Table2", "TaskResource");
                da.TableMappings.Add("Table3", "ClosedTask");

                da.Fill(ds);
            }
        }

        return ds;
    }
}


/// <summary>
/// Group of Work Requests (e.g. CR or Internal project)
/// </summary>
public sealed class RequestGroup
{
	#region Properties

	#endregion Properties

	public RequestGroup()
	{

	}


	#region Request Group

	/// <summary>
	/// Load RequestGroup Items
	/// </summary>
	/// <returns>Datatable of RequestGroup Items</returns>
	public static DataTable RequestGroupList_Get(bool includeArchive = false, bool myData = true)
	{
		string procName = "RequestGroupList_Get";

		dynamic fields = (Dictionary<string, object>)HttpContext.Current.Session["filters_Work"];

		using (DataTable dt = new DataTable("REQUESTGROUP"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive ? 1 : 0;

					//if (fields != null)
					//{
					//	cmd.Parameters.Add("@WTS_SYSTEM", SqlDbType.NVarChar).Value = fields.ContainsKey("System") ? fields["System"].value.ToString() : "";
					//	cmd.Parameters.Add("@Allocation", SqlDbType.NVarChar).Value = fields.ContainsKey("Allocation Assignment") ? fields["Allocation Assignment"].value.ToString() : "";
					//	cmd.Parameters.Add("@ProductVersion", SqlDbType.NVarChar).Value = fields.ContainsKey("Release Version") ? fields["Release Version"].value.ToString() : "";
					//	cmd.Parameters.Add("@Production", SqlDbType.NVarChar).Value = fields.ContainsKey("Production") ? fields["Production"].value.ToString() : "";
					//	cmd.Parameters.Add("@Priority", SqlDbType.NVarChar).Value = fields.ContainsKey("Workload Priority") ? fields["Workload Priority"].value.ToString() : "";
					//	cmd.Parameters.Add("@AssignedResource", SqlDbType.NVarChar).Value = fields.ContainsKey("Workload Assigned To") ? fields["Workload Assigned To"].value.ToString() : "";
					//	cmd.Parameters.Add("@PrimaryResource", SqlDbType.NVarChar).Value = fields.ContainsKey("Developer") ? fields["Developer"].value.ToString() : "";
					//	cmd.Parameters.Add("@Workload_Status", SqlDbType.NVarChar).Value = fields.ContainsKey("Workload Status") ? fields["Workload Status"].value.ToString() : "";
					//	cmd.Parameters.Add("@WorkRequest", SqlDbType.NVarChar).Value = fields.ContainsKey("WorkRequest") ? fields["WorkRequest"].value.ToString() : "";
					//	cmd.Parameters.Add("@Contract", SqlDbType.NVarChar).Value = fields.ContainsKey("Contract") ? fields["Contract"].value.ToString() : "";
					//	cmd.Parameters.Add("@Organization", SqlDbType.NVarChar).Value = fields.ContainsKey("Organization") ? fields["Organization"].value.ToString() : "";
					//	cmd.Parameters.Add("@RequestType", SqlDbType.NVarChar).Value = fields.ContainsKey("Request Type") ? fields["Request Type"].value.ToString() : "";
					//	cmd.Parameters.Add("@Scope", SqlDbType.NVarChar).Value = fields.ContainsKey("Scope") ? fields["Scope"].value.ToString() : "";
					//	cmd.Parameters.Add("@RequestPriority", SqlDbType.NVarChar).Value = fields.ContainsKey("Request Priority") ? fields["Request Priority"].value.ToString() : "";
					//	cmd.Parameters.Add("@SME", SqlDbType.NVarChar).Value = fields.ContainsKey("SME") ? fields["SME"].value.ToString() : "";
					//	cmd.Parameters.Add("@LEAD_IA_TW", SqlDbType.NVarChar).Value = fields.ContainsKey("Lead Tech Writer") ? fields["Lead Tech Writer"].value.ToString() : "";
					//	cmd.Parameters.Add("@LEAD_RESOURCE", SqlDbType.NVarChar).Value = fields.ContainsKey("Lead Resource") ? fields["Lead Resource"].value.ToString() : "";
					//	cmd.Parameters.Add("@RequestPhase", SqlDbType.NVarChar).Value = fields.ContainsKey("Request Phase") ? fields["Request Phase"].value.ToString() : "";
					//	cmd.Parameters.Add("@SUBMITTEDBY", SqlDbType.NVarChar).Value = fields.ContainsKey("Request Submitted By") ? fields["Request Submitted By"].value.ToString() : "";
					//}

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						return null;
					}
				}
			}
		}
	}

	public static DataTable Hotlist_RequestGroupList_Get(int showArchived = 0, bool myData = true)
	{
		string procName = "Hotlist_RequestGroupList_Get";

		using (DataTable dt = new DataTable("REQUESTGROUP"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
					cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
					cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = showArchived;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
							}
						}
					}
					catch (Exception ex)
					{
						LogUtility.LogException(ex);
						return null;
					}
				}
			}
		}
	}

	/// <summary>
	/// Add new RequestGroup record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool RequestGroup_Add(
		string requestGroup
		, string description
		, int sortOrder
		, bool archive
		, out bool exists
		, out int newID
		, out string errorMsg)
	{
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "RequestGroup_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@RequestGroup", SqlDbType.NVarChar).Value = requestGroup;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramExists = cmd.Parameters["@exists"];
				if (paramExists != null)
				{
					bool.TryParse(paramExists.Value.ToString(), out exists);
					saved = false;
				}
				SqlParameter paramNewID = cmd.Parameters["@newID"];
				if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
				{
					saved = true;
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Update specified RequestGroup record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool RequestGroup_Update(int requestGroupID
		, string requestGroup
		, string description
		, int sortOrder
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "RequestGroup_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@RequestGroupID", SqlDbType.Int).Value = requestGroupID;
				cmd.Parameters.Add("@RequestGroup", SqlDbType.NVarChar).Value = requestGroup;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Delete RequestGroup record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool RequestGroup_Delete(int requestGroupID
		, out bool exists
		, out bool hasDependencies
		, out bool archived
		, out string errorMsg)
	{
		exists = false;
		hasDependencies = false;
		archived = false;
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "RequestGroup_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@RequestGroupID", SqlDbType.Int).Value = requestGroupID;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@hasDependencies", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@archived", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramExists = cmd.Parameters["@exists"];
				if (paramExists != null)
				{
					bool.TryParse(paramExists.Value.ToString(), out exists);
					if (!exists)
					{
						hasDependencies = false;
						archived = false;
						errorMsg = "RequestGroup record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "RequestGroup record has dependencies and could not be permanently deleted. It has been archived instead.";
						deleted = false;
					}
				}
				SqlParameter paramDeleted = cmd.Parameters["@deleted"];
				if (paramDeleted != null)
				{
					bool.TryParse(paramDeleted.Value.ToString(), out deleted);
				}
				SqlParameter paramArchived = cmd.Parameters["@archived"];
				if (paramArchived != null)
				{
					bool.TryParse(paramArchived.Value.ToString(), out archived);
					if (archived)
					{
						deleted = false;
					}
				}
			}
		}

		return deleted;
	}

	#endregion RequestGroup


	public static DataTable WorkRequestList_Get(int requestGroupID, int typeID = 0, int showArchived = 0)
	{
		string procName = "Hotlist_WorkRequests_Get";

		using (DataTable dt = new DataTable("REQUEST"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
					cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
					cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
					cmd.Parameters.Add("@TypeID", SqlDbType.Int).Value = typeID == 0 ? (object)DBNull.Value : typeID;
					cmd.Parameters.Add("@RequestGroupID", SqlDbType.Int).Value = requestGroupID == 0 ? (object)DBNull.Value : requestGroupID;
					cmd.Parameters.Add("@ShowArchived", SqlDbType.Bit).Value = showArchived;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
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
	}
}

/// <summary>
/// Work Request - grouping of workload items WITHIN a CR
/// </summary>
public sealed class WorkRequest
{
	#region Properties

	public int WORKREQUESTID { get; set; }
	public int REQUESTGROUPID { get; set; }
	public int REQUESTTYPEID { get; set; }
	public int CONTRACTID { get; set; }
	public int ORGANIZATIONID { get; set; }
	public int WTS_SCOPEID { get; set; }
	public int EFFORTID { get; set; }
	public int SMEID { get; set; }
	public int LEAD_IA_TWID { get; set; }
	public int LEAD_RESOURCEID { get; set; }
	public int OP_PRIORITYID { get; set; }
	private string _Title = string.Empty;
	public string Title
	{
		get { return _Title; }
		set { _Title = value; }
	}
	private string _description = string.Empty;
	public string Description
	{
		get { return _description; }
		set { _description = value; }
	}
	private string _justification = string.Empty;
	public string Justification
	{
		get { return _justification; }
		set { _justification = value; }
	}
	private string _lastMeeting = string.Empty;
	public string Last_Meeting
	{
		get { return _lastMeeting; }
		set { _lastMeeting = value; }
	}
	private string _nextMeeting = string.Empty;
	public string Next_Meeting
	{
		get { return _nextMeeting; }
		set { _nextMeeting = value; }
	}
	private string _devStart = string.Empty;
	public string Dev_Start
	{
		get { return _devStart; }
		set { _devStart = value; }
	}
	private string _ciaRisk = string.Empty;
	public string CIA_Risk
	{
		get { return _ciaRisk; }
		set { _ciaRisk = value; }
	}
	private string _cmmi = string.Empty;
	public string CMMI
	{
		get { return _cmmi; }
		set { _cmmi = value; }
	}
	public int TD_StatusID { get; set; }
	public int CD_StatusID { get; set; }
	public int C_StatusID { get; set; }
	public int IT_StatusID { get; set; }
	public int CVT_StatusID { get; set; }
	public int A_StatusID { get; set; }
	public int CR_StatusID { get; set; }
	public int HasSlides { get; set; }
	public int WorkStoppage { get; set; }

	public bool Archive { get; set; }
	public int SubmittedByID { get; set; }

	#endregion Properties

	public WorkRequest() { }
	public WorkRequest(Dictionary<string, object> attributes)
	{
		ApplyAttributes(attributes);
	}
	public WorkRequest ApplyAttributes(Dictionary<string, object> attributes)
	{
		this.WORKREQUESTID = int.Parse(attributes["WORKREQUESTID"].ToString());
		this.REQUESTGROUPID = int.Parse(attributes["REQUESTGROUPID"].ToString());
		this.REQUESTTYPEID = int.Parse(attributes["REQUESTTYPEID"].ToString());
		this.CONTRACTID = int.Parse(attributes["CONTRACTID"].ToString());
		this.ORGANIZATIONID = int.Parse(attributes["ORGANIZATIONID"].ToString());
		this.WTS_SCOPEID = int.Parse(attributes["WTS_SCOPEID"].ToString());
		this.EFFORTID = int.Parse(attributes["EFFORTID"].ToString());
		this.SMEID = int.Parse(attributes["SMEID"].ToString());
		this.LEAD_IA_TWID = int.Parse(attributes["LEAD_IA_TWID"].ToString());
		this.LEAD_RESOURCEID = int.Parse(attributes["LEAD_RESOURCEID"].ToString());
		this.OP_PRIORITYID = int.Parse(attributes["OP_PRIORITYID"].ToString());
		this.Title = attributes["Title"].ToString().ToString();
		this.Description = attributes["Description"].ToString();
		this.Justification = attributes["Justification"].ToString();
		this.Archive = bool.Parse(attributes["Archive"].ToString());
		this.SubmittedByID = int.Parse(attributes["SubmittedByID"].ToString());

		return this;
	}


	/// <summary>
	/// Load Work Request Items
	/// </summary>
	/// <returns>Datatable of Work Request Items</returns>
	public static DataTable WorkRequestList_Get(int typeID = 0, int showArchived = 0, int requestGroupID = 0, bool myData = true)
	{
		string procName = "WORKREQUESTLIST_GET";

		using (DataTable dt = new DataTable("REQUEST"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
					cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
					cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
					cmd.Parameters.Add("@TypeID", SqlDbType.Int).Value = typeID == 0 ? (object)DBNull.Value : typeID;
					cmd.Parameters.Add("@RequestGroupID", SqlDbType.Int).Value = requestGroupID == 0 ? (object)DBNull.Value : requestGroupID;
					cmd.Parameters.Add("@ShowArchived", SqlDbType.Bit).Value = showArchived == 0 ? (object)DBNull.Value : showArchived;
					cmd.Parameters.Add("@OwnedBy", SqlDbType.Int).Value = myData ? UserManagement.GetUserId_FromUsername() : (object)DBNull.Value;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
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
	}


	#region Request and Request Attributes

	/// <summary>
	/// Get available options for Work Request details
	/// </summary>
	/// <returns></returns>
	public static DataSet GetAvailableOptions()
	{
		DataSet ds = new DataSet();
		string procName = "WorkRequest_GetOptions";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.TableMappings.Add("Table", "Contract");
				da.TableMappings.Add("Table1", "Organization");
				da.TableMappings.Add("Table2", "RequestType");
				da.TableMappings.Add("Table3", "Scope");
				da.TableMappings.Add("Table4", "Effort");
				da.TableMappings.Add("Table5", "Phase");
				da.TableMappings.Add("Table6", "User");
				da.TableMappings.Add("Table7", "Priority");
				da.TableMappings.Add("Table8", "RequestGroup");
				da.TableMappings.Add("Table9", "Status");

				da.Fill(ds);
			}
		}

		return ds;
	}

	/// <summary>
	/// Get details for specified Work Request
	/// </summary>
	/// <param name="workRequestID"></param>
	/// <returns></returns>
	public static DataTable WorkRequest_Get(int workRequestID)
	{
		string procName = "WorkRequest_Get";

		using (DataTable dt = new DataTable("Request"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WorkRequestID", SqlDbType.Int).Value = workRequestID;

					using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
					{
						if (dr != null && dr.HasRows)
						{
							dt.Load(dr);
							return dt;
						}
						else
						{
							return null;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Get Work Request object for specified Work Request ID
	/// </summary>
	/// <param name="workRequestID"></param>
	/// <returns></returns>
	public static WorkRequest WorkRequest_GetObject(int workRequestID)
	{
		string procName = "WorkRequest_Get";

		using (DataTable dt = new DataTable("Request"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WorkRequestID", SqlDbType.Int).Value = workRequestID;

					using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
					{
						if (dr != null && dr.HasRows)
						{
							dt.Load(dr);
							if (dt != null && dt.Rows.Count > 0)
							{
								int tempId = 0;
								bool tempVal = false;
								DataRow row = dt.Rows[0];

								WorkRequest wr = new WorkRequest();
								wr.WORKREQUESTID = workRequestID;
								wr.REQUESTGROUPID = int.TryParse(row["RequestGroupID"].ToString(), out tempId) ? tempId : 0;
								wr.Title = row["TITLE"].ToString();
								wr.Description = row["DESCRIPTION"].ToString();
								wr.Justification = row["Justification"].ToString();
								wr.CONTRACTID = int.TryParse(row["CONTRACTID"].ToString(), out tempId) ? tempId : 0;
								wr.ORGANIZATIONID = int.TryParse(row["ORGANIZATIONID"].ToString(), out tempId) ? tempId : 0;
								wr.REQUESTTYPEID = int.TryParse(row["REQUESTTYPEID"].ToString(), out tempId) ? tempId : 0;
								wr.WTS_SCOPEID = int.TryParse(row["WTS_SCOPEID"].ToString(), out tempId) ? tempId : 0;
								wr.EFFORTID = int.TryParse(row["EFFORTID"].ToString(), out tempId) ? tempId : 0;
								wr.SMEID = int.TryParse(row["SMEID"].ToString(), out tempId) ? tempId : 0;
								wr.LEAD_IA_TWID = int.TryParse(row["LEAD_IA_TWID"].ToString(), out tempId) ? tempId : 0;
								wr.LEAD_RESOURCEID = int.TryParse(row["LEAD_RESOURCEID"].ToString(), out tempId) ? tempId : 0;
								wr.OP_PRIORITYID = int.TryParse(row["OP_PRIORITYID"].ToString(), out tempId) ? tempId : 0;
								wr.Last_Meeting = row["Last_Meeting"].ToString();
								wr.Next_Meeting = row["Next_Meeting"].ToString();
								wr.Dev_Start = row["Dev_Start"].ToString();
								wr.CIA_Risk = row["CIA_Risk"].ToString();
								wr.CMMI = row["CMMI"].ToString();

								wr.TD_StatusID = int.TryParse(row["TD_STATUS"].ToString(), out tempId) ? tempId : 0;
								wr.CD_StatusID = int.TryParse(row["CD_STATUS"].ToString(), out tempId) ? tempId : 0;
								wr.C_StatusID = int.TryParse(row["C_STATUS"].ToString(), out tempId) ? tempId : 0;
								wr.IT_StatusID = int.TryParse(row["IT_STATUS"].ToString(), out tempId) ? tempId : 0;
								wr.CVT_StatusID = int.TryParse(row["CVT_STATUS"].ToString(), out tempId) ? tempId : 0;
								wr.A_StatusID = int.TryParse(row["A_STATUS"].ToString(), out tempId) ? tempId : 0;
								wr.CR_StatusID = int.TryParse(row["CR_STATUS"].ToString(), out tempId) ? tempId : 0;
								wr.HasSlides = int.TryParse(row["HasSlides"].ToString(), out tempId) ? tempId : 0;
								wr.WorkStoppage = int.TryParse(row["WorkStoppage"].ToString(), out tempId) ? tempId : 0;
								wr.Archive = bool.TryParse(row["ARCHIVE"].ToString(), out tempVal) ? tempVal : false;

								return wr;
							}
							else
							{
								return null;
							}
						}
						else
						{
							return null;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Add new Work Request
	/// </summary>
	/// <param name="newID"></param>
	/// <param name="errorMsg"></param>
	/// <returns>boolean value representing save success</returns>
	public static bool WorkRequest_Add(WorkRequest workRequest
		, out int newID
		, out string errorMsg)
	{
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkRequest_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@RequestGroupID", SqlDbType.Int).Value = workRequest.REQUESTGROUPID == 0 ? (object)DBNull.Value : workRequest.REQUESTGROUPID;
				cmd.Parameters.Add("@REQUESTTYPEID", SqlDbType.Int).Value = workRequest.REQUESTTYPEID;
				cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = workRequest.CONTRACTID == 0 ? (object)DBNull.Value : workRequest.CONTRACTID;
				cmd.Parameters.Add("@ORGANIZATIONID", SqlDbType.Int).Value = workRequest.ORGANIZATIONID == 0 ? (object)DBNull.Value : workRequest.ORGANIZATIONID;
				cmd.Parameters.Add("@WTS_SCOPEID", SqlDbType.Int).Value = workRequest.WTS_SCOPEID == 0 ? (object)DBNull.Value : workRequest.WTS_SCOPEID;
				cmd.Parameters.Add("@EFFORTID", SqlDbType.Int).Value = workRequest.EFFORTID == 0 ? (object)DBNull.Value : workRequest.EFFORTID;
				cmd.Parameters.Add("@SMEID", SqlDbType.Int).Value = workRequest.SMEID == 0 ? (object)DBNull.Value : workRequest.SMEID;
				cmd.Parameters.Add("@LEAD_IA_TWID", SqlDbType.Int).Value = workRequest.LEAD_IA_TWID == 0 ? (object)DBNull.Value : workRequest.LEAD_IA_TWID;
				cmd.Parameters.Add("@LEAD_RESOURCEID", SqlDbType.Int).Value = workRequest.LEAD_RESOURCEID == 0 ? (object)DBNull.Value : workRequest.LEAD_RESOURCEID;
				cmd.Parameters.Add("@OP_PRIORITYID", SqlDbType.Int).Value = workRequest.OP_PRIORITYID == 0 ? (object)DBNull.Value : workRequest.OP_PRIORITYID;
				cmd.Parameters.Add("@TITLE", SqlDbType.NVarChar).Value = workRequest.Title;
				cmd.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(workRequest.Description) ? (object)DBNull.Value : workRequest.Description;
				cmd.Parameters.Add("@JUSTIFICATION", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(workRequest.Justification) ? (object)DBNull.Value : workRequest.Justification;
				cmd.Parameters.Add("@ARCHIVE", SqlDbType.Bit).Value = workRequest.Archive ? 1 : 0;
				cmd.Parameters.Add("@SUBMITTEDBY", SqlDbType.Int).Value = workRequest.SubmittedByID == 0 ? (object)DBNull.Value : workRequest.SubmittedByID;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramNewID = cmd.Parameters["@newID"];
				if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
				{
					workRequest.WORKREQUESTID = newID;
					saved = true;
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Update details of existing Work Request
	/// </summary>
	/// <param name="newID"></param>
	/// <param name="errorMsg"></param>
	/// <returns>boolean value representing save success</returns>
	public static bool WorkRequest_Update(WorkRequest workRequest
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkRequest_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WORKREQUESTID", SqlDbType.Int).Value = workRequest.WORKREQUESTID;
				cmd.Parameters.Add("@RequestGroupID", SqlDbType.Int).Value = workRequest.REQUESTGROUPID == 0 ? (object)DBNull.Value : workRequest.REQUESTGROUPID;
				cmd.Parameters.Add("@REQUESTTYPEID", SqlDbType.Int).Value = workRequest.REQUESTTYPEID;
				cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = workRequest.CONTRACTID == 0 ? (object)DBNull.Value : workRequest.CONTRACTID;
				cmd.Parameters.Add("@ORGANIZATIONID", SqlDbType.Int).Value = workRequest.ORGANIZATIONID == 0 ? (object)DBNull.Value : workRequest.ORGANIZATIONID;
				cmd.Parameters.Add("@WTS_SCOPEID", SqlDbType.Int).Value = workRequest.WTS_SCOPEID == 0 ? (object)DBNull.Value : workRequest.WTS_SCOPEID;
				cmd.Parameters.Add("@EFFORTID", SqlDbType.Int).Value = workRequest.EFFORTID == 0 ? (object)DBNull.Value : workRequest.EFFORTID;
				cmd.Parameters.Add("@SMEID", SqlDbType.Int).Value = workRequest.SMEID == 0 ? (object)DBNull.Value : workRequest.SMEID;
				cmd.Parameters.Add("@LEAD_IA_TWID", SqlDbType.Int).Value = workRequest.LEAD_IA_TWID == 0 ? (object)DBNull.Value : workRequest.LEAD_IA_TWID;
				cmd.Parameters.Add("@LEAD_RESOURCEID", SqlDbType.Int).Value = workRequest.LEAD_RESOURCEID == 0 ? (object)DBNull.Value : workRequest.LEAD_RESOURCEID;
				cmd.Parameters.Add("@OP_PRIORITYID", SqlDbType.Int).Value = workRequest.OP_PRIORITYID == 0 ? (object)DBNull.Value : workRequest.OP_PRIORITYID;
				cmd.Parameters.Add("@TITLE", SqlDbType.NVarChar).Value = workRequest.Title;
				cmd.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(workRequest.Description) ? (object)DBNull.Value : workRequest.Description;
				cmd.Parameters.Add("@JUSTIFICATION", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(workRequest.Justification) ? (object)DBNull.Value : workRequest.Justification;
				cmd.Parameters.Add("@ARCHIVE", SqlDbType.Bit).Value = workRequest.Archive ? 1 : 0;
				cmd.Parameters.Add("@SUBMITTEDBY", SqlDbType.Int).Value = workRequest.SubmittedByID == 0 ? (object)DBNull.Value : workRequest.SubmittedByID;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Update details of existing Work Request from QM Grid
	/// </summary>
	/// <param name="newID"></param>
	/// <param name="errorMsg"></param>
	/// <returns>boolean value representing save success</returns>
	public static bool WorkRequest_QM_Update(WorkRequest workRequest
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;
		DateTime dtDate;

		string procName = "WorkRequest_QM_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WORKREQUESTID", SqlDbType.Int).Value = workRequest.WORKREQUESTID;
				cmd.Parameters.Add("@RequestGroupID", SqlDbType.Int).Value = workRequest.REQUESTGROUPID == 0 ? (object)DBNull.Value : workRequest.REQUESTGROUPID;
				cmd.Parameters.Add("@REQUESTTYPEID", SqlDbType.Int).Value = workRequest.REQUESTTYPEID;
				cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = workRequest.CONTRACTID == 0 ? (object)DBNull.Value : workRequest.CONTRACTID;
				cmd.Parameters.Add("@ORGANIZATIONID", SqlDbType.Int).Value = workRequest.ORGANIZATIONID == 0 ? (object)DBNull.Value : workRequest.ORGANIZATIONID;
				cmd.Parameters.Add("@WTS_SCOPEID", SqlDbType.Int).Value = workRequest.WTS_SCOPEID == 0 ? (object)DBNull.Value : workRequest.WTS_SCOPEID;
				cmd.Parameters.Add("@EFFORTID", SqlDbType.Int).Value = workRequest.EFFORTID == 0 ? (object)DBNull.Value : workRequest.EFFORTID;
				cmd.Parameters.Add("@SMEID", SqlDbType.Int).Value = workRequest.SMEID == 0 ? (object)DBNull.Value : workRequest.SMEID;
				cmd.Parameters.Add("@LEAD_IA_TWID", SqlDbType.Int).Value = workRequest.LEAD_IA_TWID == 0 ? (object)DBNull.Value : workRequest.LEAD_IA_TWID;
				cmd.Parameters.Add("@LEAD_RESOURCEID", SqlDbType.Int).Value = workRequest.LEAD_RESOURCEID == 0 ? (object)DBNull.Value : workRequest.LEAD_RESOURCEID;
				cmd.Parameters.Add("@OP_PRIORITYID", SqlDbType.Int).Value = workRequest.OP_PRIORITYID == 0 ? (object)DBNull.Value : workRequest.OP_PRIORITYID;
				cmd.Parameters.Add("@TITLE", SqlDbType.NVarChar).Value = workRequest.Title;
				cmd.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(workRequest.Description) ? (object)DBNull.Value : workRequest.Description;
				cmd.Parameters.Add("@JUSTIFICATION", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(workRequest.Justification) ? (object)DBNull.Value : workRequest.Justification;
				cmd.Parameters.Add("@Last_Meeting", SqlDbType.NVarChar).Value = (string.IsNullOrWhiteSpace(workRequest.Last_Meeting) || !DateTime.TryParse(workRequest.Last_Meeting, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@Next_Meeting", SqlDbType.NVarChar).Value = (string.IsNullOrWhiteSpace(workRequest.Next_Meeting) || !DateTime.TryParse(workRequest.Next_Meeting, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@Dev_Start", SqlDbType.NVarChar).Value = (string.IsNullOrWhiteSpace(workRequest.Dev_Start) || !DateTime.TryParse(workRequest.Dev_Start, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@CIA_Risk", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(workRequest.CIA_Risk) ? (object)DBNull.Value : workRequest.CIA_Risk;
				cmd.Parameters.Add("@CMMI", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(workRequest.CMMI) ? (object)DBNull.Value : workRequest.CMMI;
				cmd.Parameters.Add("@TD_StatusID", SqlDbType.Int).Value = workRequest.TD_StatusID == 0 ? (object)DBNull.Value : workRequest.TD_StatusID;
				cmd.Parameters.Add("@CD_StatusID", SqlDbType.Int).Value = workRequest.CD_StatusID == 0 ? (object)DBNull.Value : workRequest.CD_StatusID;
				cmd.Parameters.Add("@C_StatusID", SqlDbType.Int).Value = workRequest.C_StatusID == 0 ? (object)DBNull.Value : workRequest.C_StatusID;
				cmd.Parameters.Add("@IT_StatusID", SqlDbType.Int).Value = workRequest.IT_StatusID == 0 ? (object)DBNull.Value : workRequest.IT_StatusID;
				cmd.Parameters.Add("@CVT_StatusID", SqlDbType.Int).Value = workRequest.CVT_StatusID == 0 ? (object)DBNull.Value : workRequest.CVT_StatusID;
				cmd.Parameters.Add("@A_StatusID", SqlDbType.Int).Value = workRequest.A_StatusID == 0 ? (object)DBNull.Value : workRequest.A_StatusID;
				cmd.Parameters.Add("@CR_StatusID", SqlDbType.Int).Value = workRequest.CR_StatusID == 0 ? (object)DBNull.Value : workRequest.CR_StatusID;
				cmd.Parameters.Add("@HasSlides", SqlDbType.Int).Value = workRequest.HasSlides == 0 ? (object)DBNull.Value : workRequest.HasSlides;
				cmd.Parameters.Add("@WorkStoppage", SqlDbType.Int).Value = workRequest.WorkStoppage == 0 ? (object)DBNull.Value : workRequest.WorkStoppage;

				cmd.Parameters.Add("@ARCHIVE", SqlDbType.Bit).Value = workRequest.Archive ? 1 : 0;
				cmd.Parameters.Add("@SUBMITTEDBY", SqlDbType.Int).Value = workRequest.SubmittedByID == 0 ? (object)DBNull.Value : workRequest.SubmittedByID;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	#endregion Request and Request Attributes



	#region Comments

	/// <summary>
	/// Get comments for specified Work Request
	/// </summary>
	/// <param name="workRequestID"></param>
	/// <returns></returns>
	public static DataTable WorkRequest_GetCommentList(int workRequestID = 0)
	{
		string procName = "WorkRequest_GetCommentList";

		using (DataTable dt = new DataTable("Comment"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@WorkRequestID", SqlDbType.Int).Value = workRequestID == 0 ? (object)DBNull.Value : workRequestID;
					cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = 0;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
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
	}


	public static bool WorkRequest_Comment_Add(out int newID, out string errorMsg, int workRequestID = 0, int parentCommentID = 0, string comment_text = "")
	{
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkRequest_Comment_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkRequestID", SqlDbType.Int).Value = workRequestID;
				cmd.Parameters.Add("@ParentCommentID", SqlDbType.Int).Value = parentCommentID == 0 ? (object)DBNull.Value : parentCommentID;
				cmd.Parameters.Add("@Comment_Text", SqlDbType.NVarChar).Value = comment_text;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramNewID = cmd.Parameters["@newID"];
				if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
				{
					saved = true;
				}
			}
		}

		return saved;
	}

	public static bool WorkRequest_Comment_Update(out string errorMsg, int commentId = 0, string comment_text = "")
	{
		errorMsg = string.Empty;

		return WTSData.Comment_Update(out errorMsg, commentId, comment_text);
	}

	public static bool WorkRequest_Comment_Delete(out string errorMsg, int workRequestID = 0, int commentId = 0)
	{
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "WorkRequest_Comment_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkRequestID", SqlDbType.Int).Value = workRequestID;
				cmd.Parameters.Add("@CommentID", SqlDbType.Int).Value = commentId;

				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDeleted = cmd.Parameters["@deleted"];
				if (paramDeleted != null)
				{
					bool.TryParse(paramDeleted.Value.ToString(), out deleted);
				}
			}
		}

		return deleted;
	}

	#endregion Comments



	#region Attachments

	public static DataTable WorkRequest_GetAttachmentList(int workRequestID)
	{
		string procName = "WorkRequest_GetAttachmentList";

		using (DataTable dt = new DataTable("Attachment"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WORKREQUESTID", SqlDbType.Int).Value = workRequestID;

					using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
					{
						if (dr != null && dr.HasRows)
						{
							dt.Load(dr);
							return dt;
						}
						else
						{
							return null;
						}
					}
				}
			}
		}
	}

	public static bool Attachment_Add(int workRequestID
		, int attachmentTypeID
		, string fileName
		, string title
		, string description
		, byte[] fileData
		, int extensionID
		, out int newAttachmentID
		, out int newWorkItemAttachmentID
		, out string errorMsg)
	{
		newAttachmentID = 0;
		newWorkItemAttachmentID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		saved = WTSData.Attachment_Add(attachmentTypeID: attachmentTypeID, fileName: fileName, title: title, description: description, fileData: fileData, extensionID: extensionID, newID: out newAttachmentID, errorMsg: out errorMsg);
		if (saved && newAttachmentID > 0)
		{
			string procName = "WorkRequest_Attachment_Add";
			using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(procName, conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WorkRequestID", SqlDbType.Int).Value = workRequestID;
					cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = newAttachmentID;
					cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

					cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

					cmd.ExecuteNonQuery();

					SqlParameter paramNewID = cmd.Parameters["@newID"];
					if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newWorkItemAttachmentID) && newWorkItemAttachmentID > 0)
					{
						saved = true;
					}
				}
			}
		}

		return saved;
	}

	public static bool Attachment_Delete(int workRequestID
		, int attachmentID
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "WorkRequest_Attachment_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkRequestID", SqlDbType.Int).Value = workRequestID;
				cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = attachmentID;

				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDeleted = cmd.Parameters["@deleted"];
				if (paramDeleted != null)
				{
					bool.TryParse(paramDeleted.Value.ToString(), out deleted);
				}
			}
		}

		return deleted;
	}

	#endregion Attachments

}


/// <summary>
/// WorkItem/Task Sub-Task object
/// </summary>
public sealed class WorkItem_Task
{
	#region Properties

	public int WorkItem_TaskID { get; set; }
	public int WorkItemID { get; set; }
	public int Task_Number { get; set; }
	public int AssignedResourceID { get; set; }
	private string _assignedResource = string.Empty;
	public string AssignedResource
	{
		get { return _assignedResource; }
		set { _assignedResource = value; }
	}
	private string _estimatedStartDate = string.Empty;
	public string EstimatedStartDate
	{
		get { return _estimatedStartDate; }
		set { _estimatedStartDate = value; }
	}
	private string _actualStartDate = string.Empty;
	public string ActualStartDate
	{
		get { return _actualStartDate; }
		set { _actualStartDate = value; }
	}
	private string _actualEndDate = string.Empty;
	public string ActualEndDate
	{
		get { return _actualEndDate; }
		set { _actualEndDate = value; }
	}
	public int EstimatedEffortID { get; set; }
	public int PlannedHours { get; set; }
	public int ActualEffortID { get; set; }
	public int ActualHours { get; set; }
	public int CompletionPercent { get; set; }
	public int StatusID { get; set; }
    public int TypeID { get; set; }
    private string _status = string.Empty;
	public string Status
	{
		get { return _status; }
		set { _status = value; }
	}
    public int SystemID { get; set; }

    public string Title { get; set; }
    public string ParentTitle { get; set; }
    private string _description = string.Empty;
	public string Description
	{
		get { return _description; }
		set { _description = value; }
	}
	public int BusinessRank { get; set; }
	public int Sort_Order { get; set; }
	public bool Archive { get; set; }
    private string _createdBy = string.Empty;
	public string CreatedBy
	{
		get { return _createdBy; }
		set { _createdBy = value; }
	}
	private string _updatedBy = string.Empty;
	private string _createdDate = string.Empty;
	public string CreatedDate
	{
		get { return _createdDate; }
		set { _createdDate = value; }
	}
	public string UpdatedDate
	{
		get { return _updatedDate; }
		set { _updatedDate = value; }
	}
	private string _updatedDate = string.Empty;
	public string UpdatedBy
	{
		get { return _updatedBy; }
		set { _updatedBy = value; }
	}
	public int SubmittedByID { get; set; }
	private string _submittedBy = string.Empty;
	public string SubmittedBy
	{
		get { return _submittedBy; }
		set { _submittedBy = value; }
	}
	public int PrimaryResourceID { get; set; }
	private string _primaryResource = string.Empty;
	public string PrimaryResource
	{
		get { return _primaryResource; }
		set { _primaryResource = value; }
	}
    public int SecondaryResourceID { get; set; }
    private string _secondaryResource = string.Empty;
    public string SecondaryResource
    {
        get { return _secondaryResource; }
        set { _secondaryResource = value; }
    }
    public int PrimaryBusResourceID { get; set; }
    private string _primaryBusResource = string.Empty;
    public string PrimaryBusResource
    {
        get { return _primaryBusResource; }
        set { _primaryBusResource = value; }
    }
    public int SecondaryBusResourceID { get; set; }
    private string _secondaryBusResource = string.Empty;
    public string SecondaryBusResource
    {
        get { return _secondaryBusResource; }
        set { _secondaryBusResource = value; }
    }
    public int PriorityID { get; set; }
	private string _priority = string.Empty;
	public string Priority
	{
		get { return _priority; }
		set { _priority = value; }
	}
    public string SRNumber { get; set; }
    private string _srnumber = string.Empty;

    public int UnclosedSRTasks { get; set; }

    public int AssignedToRankID { get; set; }

    public int WorkItemTypeID { get; set; }
    private string _WorkItemType = string.Empty;
    public string WorkItemType
    {
        get { return _WorkItemType; }
        set { _WorkItemType = value; }
    }

    public int ProductVersionID { get; set; }
    private string _ProductVersion = string.Empty;
    public string ProductVersion
    {
        get { return _ProductVersion; }
        set { _ProductVersion = value; }
    }

    private string _needDate = string.Empty;
    public string NeedDate
    {
        get { return _needDate; }
        set { _needDate = value; }
    }

    private Nullable<DateTime> _inProgressDate = null;
    public Nullable<DateTime> InProgressDate
    {
        get { return _inProgressDate; }
        set { _inProgressDate = value; }
    }

    private Nullable<DateTime> _deployedDate = null;
    public Nullable<DateTime> DeployedDate
    {
        get { return _deployedDate; }
        set { _deployedDate = value; }
    }

    private Nullable<DateTime> _readyForReviewDate = null;
    public Nullable<DateTime> ReadyForReviewDate
    {
        get { return _readyForReviewDate; }
        set { _readyForReviewDate = value; }
    }

    private Nullable<DateTime> _closedDate = null;
    public Nullable<DateTime> ClosedDate
    {
        get { return _closedDate; }
        set { _closedDate = value; }
    }

    public int TotalDaysOpened { get; set; }
    public int TotalBusinessDaysOpened { get; set; }
    public int TotalDaysInProgress { get; set; }
    public int TotalDaysDeployed { get; set; }
    public int TotalDaysReadyForReview { get; set; }
    public int TotalDaysClosed { get; set; }

    public bool BusinessReview { get; set; }
    #endregion Properties

    public WorkItem_Task()
	{

	}
	public WorkItem_Task(int taskID)
	{
		this.WorkItem_TaskID = taskID;
	}

	public bool Load(int taskID = 0)
	{
		bool loaded = false;

		if (taskID > 0)
		{
			this.WorkItem_TaskID = taskID;
		}

		if (this.WorkItem_TaskID == 0)
		{
			return false;
		}

		string procName = "WorkItem_Task_Get";

		using (DataTable dt = new DataTable("Task"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@WorkItem_TaskID", SqlDbType.Int).Value = this.WorkItem_TaskID;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);

								if (dt == null || dt.Rows.Count == 0)
								{
									return false;
								}

								DataRow row = dt.Rows[0];
                                LoadFromDataRow(row);

                                loaded = true;
							}
							else
							{
								loaded = false;
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

		return loaded;
	}

    private void LoadFromDataRow(DataRow row)
    {
        int id = 0;
        bool b = false;

        this.WorkItemID = int.TryParse(row["WORKITEMID"].ToString(), out id) ? id : 0;
        this.Task_Number = int.TryParse(row["TASK_NUMBER"].ToString(), out id) ? id : 0;
        this.WorkItem_TaskID = int.TryParse(row["WORKITEM_TASKID"].ToString(), out id) ? id : 0;
        this.AssignedResourceID = int.TryParse(row["ASSIGNEDRESOURCEID"].ToString(), out id) ? id : 0;
        this.AssignedResource = row["AssignedResource"].ToString();
        this.EstimatedStartDate = row["ESTIMATEDSTARTDATE"].ToString();
        this.ActualStartDate = row["ACTUALSTARTDATE"].ToString();
        this.ActualEndDate = row["ACTUALENDDATE"].ToString();
        this.EstimatedEffortID = int.TryParse(row["EstimatedEffortID"].ToString(), out id) ? id : 0;
        this.PlannedHours = int.TryParse(row["PLANNEDHOURS"].ToString(), out id) ? id : 0;
        this.ActualEffortID = int.TryParse(row["ActualEffortID"].ToString(), out id) ? id : 0;
        this.ActualHours = int.TryParse(row["ACTUALHOURS"].ToString(), out id) ? id : 0;
        this.CompletionPercent = int.TryParse(row["COMPLETIONPERCENT"].ToString(), out id) ? id : 0;
        this.StatusID = int.TryParse(row["STATUSID"].ToString(), out id) ? id : 0;
        this.Status = row["STATUS"].ToString();
        this.SystemID = int.TryParse(row["WTS_SYSTEMID"].ToString(), out id) ? id : 0;
        this.ParentTitle = row["ParentTitle"].ToString();
        this.Title = row["TITLE"].ToString();
        this.Description = row["DESCRIPTION"].ToString();
        this.BusinessRank = int.TryParse(row["BusinessRank"].ToString(), out id) ? id : 0;
        this.Sort_Order = int.TryParse(row["SORT_ORDER"].ToString(), out id) ? id : 0;
        this.Archive = bool.TryParse(row["ARCHIVE"].ToString(), out b) ? false : b;
        this.CreatedBy = row["CREATEDBY"].ToString();
        this.CreatedDate = row["CREATEDDATE"].ToString();
        this.UpdatedBy = row["UPDATEDBY"].ToString();
        this.UpdatedDate = row["UPDATEDDATE"].ToString();
        this.SubmittedByID = int.TryParse(row["SubmittedByID"].ToString(), out id) ? id : 0;
        this.SubmittedBy = row["TITLE"].ToString();
        this.PrimaryResourceID = int.TryParse(row["PrimaryResourceID"].ToString(), out id) ? id : 0;
        this.SecondaryResourceID = int.TryParse(row["SecondaryResourceID"].ToString(), out id) ? id : 0;
        this.PrimaryBusResourceID = int.TryParse(row["PRIMARYBUSRESOURCEID"].ToString(), out id) ? id : 0;
        this.SecondaryBusResourceID = int.TryParse(row["SecondaryBusResourceID"].ToString(), out id) ? id : 0;
        this.PrimaryResource = row["PrimaryResource"].ToString();
        this.PriorityID = int.TryParse(row["PRIORITYID"].ToString(), out id) ? id : 0;
        this.Priority = row["PRIORITY"].ToString();
        this.SRNumber = row["SRNumber"].ToString();
        this.UnclosedSRTasks = int.TryParse(row["Unclosed SR Tasks"].ToString(), out id) ? id : 0;
        this.AssignedToRankID = int.TryParse(row["AssignedToRankID"].ToString(), out id) ? id : 0;
        this.ProductVersionID = int.TryParse(row["ProductVersionID"].ToString(), out id) ? id : 0;
        this.ProductVersion = row["ProductVersion"].ToString();
        this.WorkItemTypeID = int.TryParse(row["WORKITEMTYPEID"].ToString(), out id) ? id : 0;
        this.WorkItemType = row["WORKITEMTYPE"].ToString();
        this.NeedDate = row["NeedDate"].ToString();
        this.BusinessReview = bool.TryParse(row["BusinessReview"].ToString(), out b) ? b : false;
        this.InProgressDate = String.IsNullOrEmpty(row["InProgressDate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["InProgressDate"].ToString());
        this.DeployedDate = String.IsNullOrEmpty(row["DeployedDate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["DeployedDate"].ToString());
        this.ReadyForReviewDate = String.IsNullOrEmpty(row["ReadyForReviewDate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["ReadyForReviewDate"].ToString());
        this.ClosedDate = String.IsNullOrEmpty(row["ClosedDate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["ClosedDate"].ToString());
        this.TotalDaysOpened = int.TryParse(row["TotalDaysOpened"].ToString(), out id) ? id : 0;
        this.TotalBusinessDaysOpened = int.TryParse(row["TotalBusinessDaysOpened"].ToString(), out id) ? id : 0;
        this.TotalDaysInProgress = int.TryParse(row["TotalDaysInProgress"].ToString(), out id) ? id : 0;
        this.TotalDaysDeployed = int.TryParse(row["TotalDaysDeployed"].ToString(), out id) ? id : 0;
        this.TotalDaysReadyForReview = int.TryParse(row["TotalDaysReadyForReview"].ToString(), out id) ? id : 0;
        this.TotalDaysClosed = int.TryParse(row["TotalDaysClosed"].ToString(), out id) ? id : 0;

    }

	public bool Add(out int newID, out string errorMsg, bool AddSR = false)
	{
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;
		DateTime dtDate;
        int SR_Number = 0;

        int.TryParse(this.SRNumber, out SR_Number);

        string procName = "WorkItem_Task_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

                int SRworkItemID = 0;

                // Get the default SR WorkItem from Web.config, and send it back in ID, to display to user where the new SR went.
                if (AddSR)
                {
                int.TryParse(System.Web.Configuration.WebConfigurationManager.AppSettings["WorkItemIDForSRs"], out SRworkItemID);
                this.WorkItemID = SRworkItemID;
                }

                cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = this.WorkItemID;
				cmd.Parameters.Add("@PriorityID", SqlDbType.Int).Value = this.PriorityID == 0 ? 4 : this.PriorityID;
				cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = this.Title.Trim();
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Description) ? (object)DBNull.Value : this.Description;
				cmd.Parameters.Add("@AssignedResourceID", SqlDbType.Int).Value = this.AssignedResourceID;
				cmd.Parameters.Add("@PrimaryResourceID", SqlDbType.Int).Value = this.PrimaryResourceID == 0 ? (object)DBNull.Value : this.PrimaryResourceID;
                cmd.Parameters.Add("@SecondaryResourceID", SqlDbType.Int).Value = this.SecondaryResourceID == 0 ? (object)DBNull.Value : this.SecondaryResourceID;
                cmd.Parameters.Add("@PrimaryBusResourceID", SqlDbType.Int).Value = this.PrimaryBusResourceID == 0 ? (object)DBNull.Value : this.PrimaryBusResourceID;
                cmd.Parameters.Add("@SecondaryBusResourceID", SqlDbType.Int).Value = this.SecondaryBusResourceID == 0 ? (object)DBNull.Value : this.SecondaryBusResourceID;
                cmd.Parameters.Add("@SubmittedByID", SqlDbType.Int).Value = this.SubmittedByID == 0 ? (object)DBNull.Value : this.SubmittedByID;
				cmd.Parameters.Add("@PlannedStartDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(this.EstimatedStartDate) || !DateTime.TryParse(this.EstimatedStartDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@ActualStartDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(this.ActualStartDate) || !DateTime.TryParse(this.ActualStartDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@EstimatedEffortID", SqlDbType.Int).Value = this.EstimatedEffortID == 0 ? (object)DBNull.Value : this.EstimatedEffortID;
				cmd.Parameters.Add("@ActualEffortID", SqlDbType.Int).Value = this.ActualEffortID == 0 ? (object)DBNull.Value : this.ActualEffortID;
				cmd.Parameters.Add("@ActualEndDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(this.ActualEndDate) || !DateTime.TryParse(this.ActualEndDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@CompletionPercent", SqlDbType.Int).Value = this.CompletionPercent;
				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = this.StatusID;
                cmd.Parameters.Add("@WorkItemTypeID", SqlDbType.Int).Value = this.WorkItemTypeID;
                cmd.Parameters.Add("@BusinessRank", SqlDbType.Int).Value = this.BusinessRank;
				cmd.Parameters.Add("@SortOrder", SqlDbType.Int).Value = this.Sort_Order;
                cmd.Parameters.Add("@SRNumber", SqlDbType.Int).Value = SR_Number == 0 ? (object)DBNull.Value : SR_Number;
                cmd.Parameters.Add("@AssignedToRankID ", SqlDbType.NVarChar).Value = this.AssignedToRankID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = this.ProductVersionID;
                cmd.Parameters.Add("@NeedDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(this.NeedDate) || !DateTime.TryParse(this.NeedDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@BusinessReview", SqlDbType.Bit).Value = this.BusinessReview ? 1 : 0;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramNewID = cmd.Parameters["@newID"];
				if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
				{
                    if (AddSR)
                        newID = SRworkItemID;
                    else
                        this.WorkItem_TaskID = newID;
                    saved = true;
				}
			}
		}

		return saved;
	}

	public bool Save(out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;
		DateTime dtDate;
        int SR_Number = 0;

        int.TryParse(this.SRNumber, out SR_Number);

        string procName = "WorkItem_Task_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItem_TaskID", SqlDbType.Int).Value = this.WorkItem_TaskID;
				cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = this.WorkItemID;
				cmd.Parameters.Add("@PriorityID", SqlDbType.Int).Value = this.PriorityID == 0 ? 4 : this.PriorityID;
				cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = this.Title.Trim();
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(this.Description) ? (object)DBNull.Value : this.Description;
				cmd.Parameters.Add("@AssignedResourceID", SqlDbType.Int).Value = this.AssignedResourceID;
				cmd.Parameters.Add("@PrimaryResourceID", SqlDbType.Int).Value = this.PrimaryResourceID == 0 ? (object)DBNull.Value : this.PrimaryResourceID;
                cmd.Parameters.Add("@SecondaryResourceID", SqlDbType.Int).Value = this.SecondaryResourceID == 0 ? (object)DBNull.Value : this.SecondaryResourceID;
                cmd.Parameters.Add("@PrimaryBusResourceID", SqlDbType.Int).Value = this.PrimaryBusResourceID == 0 ? (object)DBNull.Value : this.PrimaryBusResourceID;
                cmd.Parameters.Add("@SecondaryBusResourceID", SqlDbType.Int).Value = this.SecondaryBusResourceID == 0 ? (object)DBNull.Value : this.SecondaryBusResourceID;
                cmd.Parameters.Add("@PlannedStartDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(this.EstimatedStartDate) || !DateTime.TryParse(this.EstimatedStartDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@ActualStartDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(this.ActualStartDate) || !DateTime.TryParse(this.ActualStartDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@EstimatedEffortID", SqlDbType.Int).Value = this.EstimatedEffortID == 0 ? (object)DBNull.Value : this.EstimatedEffortID;
				//cmd.Parameters.Add("@PlannedHours", SqlDbType.Int).Value = this.PlannedHours == 0 ? (object)DBNull.Value : this.PlannedHours;
				cmd.Parameters.Add("@ActualEffortID", SqlDbType.Int).Value = this.ActualEffortID == 0 ? (object)DBNull.Value : this.ActualEffortID;
				//cmd.Parameters.Add("@ActualHours", SqlDbType.Int).Value = this.ActualHours == 0 ? (object)DBNull.Value : this.ActualHours;
				cmd.Parameters.Add("@ActualEndDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(this.ActualEndDate) || !DateTime.TryParse(this.ActualEndDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@CompletionPercent", SqlDbType.Int).Value = this.CompletionPercent;
				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = this.StatusID;
                cmd.Parameters.Add("@WorkItemTypeID", SqlDbType.Int).Value = this.WorkItemTypeID;
				cmd.Parameters.Add("@BusinessRank", SqlDbType.Int).Value = this.BusinessRank;
                cmd.Parameters.Add("@SortOrder", SqlDbType.Int).Value = this.Sort_Order;
                cmd.Parameters.Add("@SRNumber", SqlDbType.Int).Value = SR_Number == 0 ? (object)DBNull.Value : SR_Number;
                cmd.Parameters.Add("@AssignedToRankID ", SqlDbType.NVarChar).Value = this.AssignedToRankID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = this.ProductVersionID;
                cmd.Parameters.Add("@NeedDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(this.NeedDate) || !DateTime.TryParse(this.NeedDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
                cmd.Parameters.Add("@BusinessReview", SqlDbType.Bit).Value = this.BusinessReview ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	public bool Delete(out string errorMsg)
	{
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "WorkItem_Task_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItem_TaskID", SqlDbType.Int).Value = this.WorkItem_TaskID;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDeleted = cmd.Parameters["@deleted"];
				if (paramDeleted != null)
				{
					bool.TryParse(paramDeleted.Value.ToString(), out deleted);
				}
			}
		}

		return deleted;
	}

    public bool LoadByNumber(int parentTaskID, int taskNumber)
    {
        bool loaded = false;

        if (parentTaskID == 0 || taskNumber == 0)
        {
            return false;
        }

        this.WorkItemID = parentTaskID;
        this.Task_Number = taskNumber;

        string procName = "WorkItem_Task_GetByNumber";

        using (DataTable dt = new DataTable("Task"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = this.WorkItemID;
                    cmd.Parameters.Add("@TaskNumber", SqlDbType.Int).Value = this.Task_Number;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);

                                if (dt == null || dt.Rows.Count == 0)
                                {
                                    return false;
                                }

                                DataRow row = dt.Rows[0];
                                LoadFromDataRow(row);

                                loaded = true;
                            }
                            else
                            {
                                loaded = false;
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

        return loaded;
    }

	/// <summary>
	/// Get available options for Workload Item Task details
	/// </summary>
	/// <param name="workItemID"></param>
	/// <param name="userOrgIDs"></param>
	/// <returns></returns>
	public static DataSet GetAvailableOptions(int workItemID = 0, string userOrgIDs = "")
	{
		DataSet ds = new DataSet();
		string procName = "WorkItem_Task_GetOptions";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@User_OrganizationIDs", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(userOrgIDs) ? (object)DBNull.Value : userOrgIDs;
				cmd.Parameters.Add("@WORKITEMID", SqlDbType.Int).Value = workItemID;

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.TableMappings.Add("Table", "User");
				da.TableMappings.Add("Table1", "PercentComplete");
				da.TableMappings.Add("Table2", "Status");
				da.TableMappings.Add("Table3", "Priority");
				da.TableMappings.Add("Table4", "TshirtSizes");
                da.TableMappings.Add("Table5", "Rank");
                da.TableMappings.Add("Table6", "ProductVersion");
                da.TableMappings.Add("Table7", "WorkItemType");
                da.TableMappings.Add("Table8", "PDTDRPhase");
                da.Fill(ds);
			}
		}

		return ds;
	}

    public static int WorkItem_TaskID_Get(int itemID, int taskNumber)
    {
        int taskID = 0;

        string procName = "WorkItem_TaskID_Get";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkID", SqlDbType.Int).Value = itemID;
                cmd.Parameters.Add("@TaskNumber", SqlDbType.Int).Value = taskNumber;
                cmd.Parameters.Add("@TaskID", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter workTaskID = cmd.Parameters["@TaskID"];
                if (workTaskID != null)
                {
                    int.TryParse(workTaskID.Value.ToString(), out taskID);
                }
            }
        }
        return taskID;
    }

    #region Comments

    /// <summary>
    /// Get comments for this sub-task
    /// </summary>
    /// <param name="taskID"></param>
    /// <returns></returns>
    public DataTable GetComments()
	{
		string procName = "WorkItem_Task_CommentList_Get";

		using (DataTable dt = new DataTable("Comment"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = this.WorkItem_TaskID;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
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
	}


	public bool AddComment(out int newID, out string errorMsg, int parentCommentID = 0, string comment_text = "")
	{
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkItem_Task_Comment_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = this.WorkItem_TaskID;
				cmd.Parameters.Add("@ParentCommentID", SqlDbType.Int).Value = parentCommentID == 0 ? (object)DBNull.Value : parentCommentID;
				cmd.Parameters.Add("@Comment_Text", SqlDbType.NVarChar).Value = comment_text;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramNewID = cmd.Parameters["@newID"];
				if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
				{
					saved = true;
				}
			}
		}

		return saved;
	}

	public bool UpdateComment(out string errorMsg, int commentId = 0, string comment_text = "")
	{
		errorMsg = string.Empty;

		return WTSData.Comment_Update(out errorMsg, commentId, comment_text);
	}

	public bool DeleteComment(out string errorMsg, int commentId = 0)
	{
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "WorkItem_Task_Comment_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = this.WorkItem_TaskID;
				cmd.Parameters.Add("@CommentID", SqlDbType.Int).Value = commentId;

				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDeleted = cmd.Parameters["@deleted"];
				if (paramDeleted != null)
				{
					bool.TryParse(paramDeleted.Value.ToString(), out deleted);
				}
			}
		}

		return deleted;
	}

    #endregion Comments

    #region Attachments

    public static DataTable WorkItem_Task_GetAttachmentList(int WorkItemTaskID)
    {
        string procName = "WorkItem_Task_GetAttachmentList";

        using (DataTable dt = new DataTable("Attachment"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WorkItemTaskID", SqlDbType.Int).Value = WorkItemTaskID;

                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (dr != null && dr.HasRows)
                        {
                            dt.Load(dr);
                            return dt;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
    }

    public static bool Attachment_Add(int WorkItemTaskID
        , int attachmentTypeID
        , string fileName
        , string title
        , string description
        , byte[] fileData
        , int extensionID
        , out int newAttachmentID
        , out int newWorkItemAttachmentID
        , out string errorMsg)
    {
        newAttachmentID = 0;
        newWorkItemAttachmentID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        saved = WTSData.Attachment_Add(attachmentTypeID: attachmentTypeID, fileName: fileName, title: title, description: description, fileData: fileData, extensionID: extensionID, newID: out newAttachmentID, errorMsg: out errorMsg);
        if (saved && newAttachmentID > 0)
        {
            string procName = "WorkItem_Task_Attachment_Add";
            using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WorkItemTaskID", SqlDbType.Int).Value = WorkItemTaskID;
                    cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = newAttachmentID;
                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                    cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    SqlParameter paramNewID = cmd.Parameters["@newID"];
                    if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newWorkItemAttachmentID) && newWorkItemAttachmentID > 0)
                    {
                        saved = true;
                    }
                }
            }
        }

        return saved;
    }

    public static bool Attachment_Delete(int WorkItemTaskID
        , int attachmentID
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "WorkItem_Task_Attachment_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkItemTaskID", SqlDbType.Int).Value = WorkItemTaskID;
                cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = attachmentID;

                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }

    #endregion Attachments
}




/// <summary>
/// Workload level details
/// </summary>
public sealed class WorkloadItem
{

#region Properties

	public int WorkItemID { get; set; }
	public int BugTracker_ID { get; set; }
	public int WorkRequestID { get; set; }
	public int WorkItemTypeID { get; set; }
	public int WTS_SystemID { get; set; }
	public int ProductVersionID { get; set; }
	public bool Production { get; set; }
	public bool Recurring { get; set; }
	public int SR_Number { get; set; }
	public bool Reproduced_Biz { get; set; }
	public bool Reproduced_Dev { get; set; }
	public int PriorityID { get; set; }
	public int AllocationID { get; set; }
	public int MenuTypeID { get; set; }
	public int MenuNameID { get; set; }
	public int SubmittedByID { get; set; }
	public int AssignedResourceID { get; set; }
	public int PrimaryResourceID { get; set; }
	public int ResourcePriorityRank { get; set; }
	public int SecondaryResourceID { get; set; }
	public int SecondaryResourceRank { get; set; }
	public int PrimaryBusinessResourceID { get; set; }
    public int SecondaryBusinessResourceID { get; set; }
    public int PrimaryBusinessRank { get; set; }
    public int SecondaryBusinessRank { get; set; }
    public int WorkTypeID { get; set; }
	public int StatusID { get; set; }
    public string SRNumber { get; set; }
    public bool IVTRequired { get; set; }
    public int Dependency_Count { get; set; }
	private string _needDate = string.Empty;
	public string NeedDate
	{
		get { return _needDate; }
		set { _needDate = value; }
	}
	//public int EstimatedHours { get; set; }
	public int EstimatedEffortID { get; set; }
	private string _estimatedCompletionDate = string.Empty;
	public string EstimatedCompletionDate
	{
		get { return _estimatedCompletionDate; }
		set { _estimatedCompletionDate = value; }
	}
    private string _actualCompletionDate = string.Empty;
    public string ActualCompletionDate
    {
        get { return _actualCompletionDate; }
        set { _actualCompletionDate = value; }
    }
    public int CompletionPercent { get; set; }
	public int WorkAreaID { get; set; }
	public int WorkloadGroupID { get; set; }
	private string _title;
	public string Title
	{
		get { return _title; }
		set { _title = value; }
	}
	private string _description = string.Empty;
	public string Description
	{
		get { return _description; }
		set { _description = value; }
	}
	public bool Archive { get; set; }
	public bool Deployed_Comm { get; set; }
	public bool Deployed_Test { get; set; }
	public bool Deployed_Prod { get; set; }
	public int DeployedBy_CommID { get; set; }
	public int DeployedBy_TestID { get; set; }
	public int DeployedBy_ProdID { get; set; }
	private string _deployedDate_Comm = string.Empty;
	public string DeployedDate_Comm
	{
		get { return _deployedDate_Comm; }
		set { _deployedDate_Comm = value; }
	}
	private string _deployedDate_Test = string.Empty;
	public string DeployedDate_Test
	{
		get { return _deployedDate_Test; }
		set { _deployedDate_Test = value; }
	}
	private string _deployedDate_Prod = string.Empty;
	public string DeployedDate_Prod
	{
		get { return _deployedDate_Prod; }
		set { _deployedDate_Prod = value; }
	}
	private string _plannedDesignStart = string.Empty;
	public string PlannedDesignStart
	{
		get { return _plannedDesignStart; }
		set { _plannedDesignStart = value; }
	}
	private string _plannedDevStart = string.Empty;
	public string PlannedDevStart
	{
		get { return _plannedDevStart; }
		set { _plannedDevStart = value; }
	}
	private string _actualDesignStart = string.Empty;
	public string ActualDesignStart
	{
		get { return _actualDesignStart; }
		set { _actualDesignStart = value; }
	}
	private string _actualDevStart = string.Empty;
	public string ActualDevStart
	{
		get { return _actualDevStart; }
		set { _actualDevStart = value; }
	}
	private string _cvtStep = string.Empty;
	public string CVTStep
	{
		get { return _cvtStep; }
		set { _cvtStep = value; }
	}
	private string _cvtStatus = string.Empty;
	public string CVTStatus
	{
		get { return _cvtStatus; }
		set { _cvtStatus = value; }
	}
	public int TesterID { get; set; }
	public int Comment_Count { get; set; }
	public int Attachment_Count { get; set; }
	public int WorkRequest_Attachment_Count { get; set; }
	public int Task_Count { get; set; }
	public bool Signed_Bus { get; set; }
	public bool Signed_Dev { get; set; }
	private string _signedBy_Bus = string.Empty;
	public string Signed_Bus_User
	{
		get { return _signedBy_Bus; }
		set { _signedBy_Bus = value; }
	}
	private string _signedBy_Dev = string.Empty;
	public string Signed_Dev_User
	{
		get { return _signedBy_Dev; }
		set { _signedBy_Dev = value; }
	}
	private string _signedDate_Bus = string.Empty;
	public string SignedDate_Bus
	{
		get { return _signedDate_Bus; }
		set { _signedDate_Bus = value; }
	}
	private string _signedDate_Dev = string.Empty;
	public string SignedDate_Dev
	{
		get { return _signedDate_Dev; }
		set { _signedDate_Dev = value; }
	}
	public int ProductionStatusID { get; set; }
	public int PDDTDR_PHASEID { get; set; }

    public int AssignedToRankID { get; set; }

    public bool BusinessReview { get; set; }
    #endregion Properties

    public WorkloadItem(int itemId)
	{
		this.WorkItemID = itemId;
	}
	public WorkloadItem(Dictionary<string, object> attributes)
	{
		ApplyAttributes(attributes);
	}
	public WorkloadItem ApplyAttributes(Dictionary<string, object> attributes)
	{
		bool flagged = false;

		this.WorkItemID = int.Parse(attributes["WorkItemID"].ToString());
		this.BugTracker_ID = int.Parse(attributes["BugTracker_ID"].ToString());
		this.WorkRequestID = int.Parse(attributes["WorkRequestID"].ToString());
		this.WorkItemTypeID = int.Parse(attributes["WorkItemTypeID"].ToString());
		this.WTS_SystemID = int.Parse(attributes["WTS_SystemID"].ToString());
		this.ProductVersionID = int.Parse(attributes["ProductVersionID"].ToString());
		this.Production = bool.Parse(attributes["Production"].ToString());
		this.Recurring = bool.Parse(attributes["Recurring"].ToString());
		this.SR_Number = attributes.ContainsKey("SR_Number") ? int.Parse(attributes["SR_Number"].ToString()) : 0;
		this.Reproduced_Biz = bool.Parse(attributes["Reproduced_Biz"].ToString());
		this.Reproduced_Dev = bool.Parse(attributes["Reproduced_Dev"].ToString());
		this.PriorityID = int.Parse(attributes["PriorityID"].ToString());
		this.AllocationID = int.Parse(attributes["AllocationID"].ToString());
		this.MenuTypeID = int.Parse(attributes["MenuTypeID"].ToString());
		this.MenuNameID = int.Parse(attributes["MenuNameID"].ToString());
		this.SubmittedByID = attributes.ContainsKey("SubmittedByID") ? int.Parse(attributes["SubmittedByID"].ToString()) : 0;
		this.AssignedResourceID = int.Parse(attributes["AssignedResourceID"].ToString());
		this.PrimaryResourceID = int.Parse(attributes["PrimaryResourceID"].ToString());
		this.ResourcePriorityRank = int.Parse(attributes["ResourcePriorityRank"].ToString());
		this.SecondaryResourceID = int.Parse(attributes["SecondaryResourceID"].ToString());
		this.SecondaryResourceRank = int.Parse(attributes["SecondaryResourceRank"].ToString());
		this.PrimaryBusinessResourceID = int.Parse(attributes["PrimaryBusinessResourceID"].ToString());
        this.SecondaryBusinessResourceID = int.Parse(attributes["SecondaryBusinessResourceID"].ToString());
        this.PrimaryBusinessRank = int.Parse(attributes["PrimaryBusinessRank"].ToString());
        this.SecondaryBusinessRank = int.Parse(attributes["SecondaryBusinessRank"].ToString());
        this.WorkTypeID = int.Parse(attributes["WorkTypeID"].ToString());
		this.StatusID = int.Parse(attributes["StatusID"].ToString());
		this.IVTRequired = (!attributes.ContainsKey("IVTRequired") || !bool.TryParse(attributes["IVTRequired"].ToString(), out flagged)) ? false : flagged;
        this.NeedDate = attributes["NeedDate"].ToString();
		//this.EstimatedHours = int.Parse(attributes["EstimatedHours"].ToString());
		this.EstimatedEffortID = int.Parse(attributes["EstimatedEffortID"].ToString());
		this.EstimatedCompletionDate = attributes["EstimatedCompletionDate"].ToString();
        this.ActualCompletionDate = attributes["ActualCompletionDate"].ToString();
        this.CompletionPercent = int.Parse(attributes["CompletionPercent"].ToString());
		this.WorkAreaID = attributes.ContainsKey("WorkAreaID") ? int.Parse(attributes["WorkAreaID"].ToString()) : 1;
		this.WorkloadGroupID = attributes.ContainsKey("WorkloadGroupID") ? int.Parse(attributes["WorkloadGroupID"].ToString()) : 1;
		this.Title = attributes["Title"].ToString().ToString();
		this.Description = attributes["Description"].ToString();
		this.Archive = bool.Parse(attributes["Archive"].ToString());
		this.Deployed_Comm = bool.Parse(attributes["Deployed_Comm"].ToString());
		this.Deployed_Test = bool.Parse(attributes["Deployed_Test"].ToString());
		this.Deployed_Prod = bool.Parse(attributes["Deployed_Prod"].ToString());
		this.DeployedBy_CommID = int.Parse(attributes["DeployedBy_CommID"].ToString());
		this.DeployedBy_TestID = int.Parse(attributes["DeployedBy_TestID"].ToString());
		this.DeployedBy_ProdID = int.Parse(attributes["DeployedBy_ProdID"].ToString());
		this.DeployedDate_Comm = attributes["DeployedDate_Comm"].ToString();
		this.DeployedDate_Test = attributes["DeployedDate_Test"].ToString();
		this.DeployedDate_Prod = attributes["DeployedDate_Prod"].ToString();
		this.PlannedDesignStart = attributes["PlannedDesignStart"].ToString();
		this.PlannedDevStart = attributes["PlannedDevStart"].ToString();
		this.ActualDesignStart = attributes["ActualDesignStart"].ToString();
		this.ActualDevStart = attributes["ActualDevStart"].ToString();
		this.CVTStep = attributes["CVTStep"].ToString();
		this.CVTStatus = attributes["CVTStatus"].ToString();
		this.TesterID = int.Parse(attributes["TesterID"].ToString());
		this.Signed_Bus = bool.Parse(attributes["Signed_Bus"].ToString());
		this.Signed_Dev = bool.Parse(attributes["Signed_Dev"].ToString());
		this.ProductionStatusID = int.Parse(attributes["ProductionStatusID"].ToString());
        this.PDDTDR_PHASEID = int.Parse(attributes["PDDTDR_PHASEID"].ToString());
        this.AssignedToRankID = attributes.ContainsKey("AssignedToRankID") ? int.Parse(attributes["AssignedToRankID"].ToString()) : 0;
        this.BusinessReview = bool.Parse(attributes["BusinessReview"].ToString());
        return this;
	}

	public static DataSet Metrics_Get()
	{
		DataSet ds = new DataSet();
		string procName = "Metrics_Get";
		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
				cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
				cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
				cmd.Parameters.Add("@OwnedBy", SqlDbType.Int).Value = UserManagement.GetUserId_FromUsername();

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.TableMappings.Add("Table", "Workload");
				//da.TableMappings.Add("Table1", "Request");

				da.Fill(ds);
			}
		}

		return ds;
	}

    public static DataSet GetWorkItemMetrics(string SelectedStatuses = "", string SelectedAssigned = "")
    {
        DataSet ds = new DataSet();
        string procName = "WI_Metrics_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
                //cmd.Parameters.Add("@OwnedBy", SqlDbType.Int).Value = UserManagement.GetUserId_FromUsername();

                cmd.Parameters.Add("@SelectedAssigned", SqlDbType.NVarChar).Value = SelectedAssigned;
                cmd.Parameters.Add("@SelectedStatuses", SqlDbType.NVarChar).Value = SelectedStatuses;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "WorkItemStatus");
                da.TableMappings.Add("Table1", "WorkItemTaskStatus");
                da.TableMappings.Add("Table2", "WorkItemPriority");
                da.TableMappings.Add("Table3", "WorkItemTaskPriority");

                da.Fill(ds);
            }
        }

        return ds;
    }

    public static DataTable Concerns_Get()
	{
		string procName = "CONCERNS_GET";

		using (DataTable dt = new DataTable("WorkloadConcerns"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
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
	}

    public static DataTable WORKITEMLIST_GET_0()
    {
        string procName = "WORKITEMLIST_GET_0";
        using (DataTable dt = new DataTable("Workload"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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

    }
    /// <summary>
    /// Load Workload Items
    /// </summary>
    /// <param name="workRequestID"></param>
    /// <param name="showArchived"></param>
    /// <param name="columnListOnly"></param>
    /// <param name="filters">Additional parameters, passed if data needs to be filtered for child records</param>
    /// <param name="myData"></param>
    /// <returns>Datatable of Workload Items</returns>
    ///
    public static DataTable WorkItemList_Get_QF(int workRequestID = 0, int showArchived = 0, int columnListOnly = 0, bool myData = false, string rankSortType = "Tech", int showClosed = 0, bool ShowBacklog = false, string SelectedStatuses = "", string SelectedAssigned = "", bool qfBusinessReview = false, bool Affiliated = false, string ParentAffilitatedID = "")
    {
        string procName = "";
        if (Affiliated)
            procName = "WORKITEMLIST_AFFILIATED_GET";
        else
            procName = "WORKITEMLIST_ASSIGNED_GET";

        using (DataTable dt = new DataTable("Workload"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
					cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
					cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
					cmd.Parameters.Add("@WORKREQUESTID", SqlDbType.Int).Value = workRequestID == 0 ? (object)DBNull.Value : workRequestID;
					cmd.Parameters.Add("@ShowClosed", SqlDbType.Int).Value = showClosed == 0 ? (object)DBNull.Value : showClosed;
					cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived == 0 ? (object)DBNull.Value : showArchived;
                    cmd.Parameters.Add("@ShowBacklog", SqlDbType.Bit).Value = ShowBacklog;
                    cmd.Parameters.Add("@ColumnListOnly", SqlDbType.Int).Value = columnListOnly;
					cmd.Parameters.Add("@OwnedBy", SqlDbType.Int).Value = myData ? UserManagement.GetUserId_FromUsername() : (object)DBNull.Value;
					cmd.Parameters.Add("@RankSortType", SqlDbType.NVarChar).Value = rankSortType;

                    cmd.Parameters.Add("@SelectedAssigned", SqlDbType.NVarChar).Value = SelectedAssigned;
                    cmd.Parameters.Add("@SelectedStatuses", SqlDbType.NVarChar).Value = SelectedStatuses;
                    cmd.Parameters.Add("@QFBusinessReview", SqlDbType.NVarChar).Value = qfBusinessReview == true ? "1" : "0";
                    cmd.Parameters.Add("@ParentAffilitatedID", SqlDbType.NVarChar).Value = ParentAffilitatedID;

                    try
                    {
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
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
	}
    public static DataTable WorkItemList_Get(int workRequestID = 0, int showArchived = 0, int columnListOnly = 0, bool myData = false, string rankSortType = "Tech", int showClosed = 0, bool ShowBacklog = false)
    {
        string procName = "WORKITEMLIST_GET";

        using (DataTable dt = new DataTable("Workload"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@WORKREQUESTID", SqlDbType.Int).Value = workRequestID == 0 ? (object)DBNull.Value : workRequestID;
                    cmd.Parameters.Add("@ShowClosed", SqlDbType.Int).Value = showClosed == 0 ? (object)DBNull.Value : showClosed;
                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived == 0 ? (object)DBNull.Value : showArchived;
                    cmd.Parameters.Add("@ShowBacklog", SqlDbType.Bit).Value = ShowBacklog;
                    cmd.Parameters.Add("@ColumnListOnly", SqlDbType.Int).Value = columnListOnly;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.Int).Value = myData ? UserManagement.GetUserId_FromUsername() : (object)DBNull.Value;
                    cmd.Parameters.Add("@RankSortType", SqlDbType.NVarChar).Value = rankSortType;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    public static DataTable WorkItemList_Get_QM(int workRequestID = 0, int showArchived = 0, int columnListOnly = 0, bool myData = false, string rankSortType = "Tech", int showClosed = 0, bool ShowBacklog = false, string SelectedStatuses = "", string SelectedAssigned = "", bool Affiliated = false)
    {
        string procName = "WORKITEMLIST_GET";

        using (DataTable dt = new DataTable("Workload"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@WORKREQUESTID", SqlDbType.Int).Value = workRequestID == 0 ? (object)DBNull.Value : workRequestID;
                    cmd.Parameters.Add("@ShowClosed", SqlDbType.Int).Value = showClosed == 0 ? (object)DBNull.Value : showClosed;
                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived == 0 ? (object)DBNull.Value : showArchived;
                    cmd.Parameters.Add("@ShowBacklog", SqlDbType.Bit).Value = ShowBacklog;
                    cmd.Parameters.Add("@ColumnListOnly", SqlDbType.Int).Value = columnListOnly;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.Int).Value = myData ? UserManagement.GetUserId_FromUsername() : (object)DBNull.Value;
                    cmd.Parameters.Add("@RankSortType", SqlDbType.NVarChar).Value = rankSortType;
                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }


    #region Connected Work Items

    public static DataTable WorkItem_TestItemList_Get(int workItemID = 0, string sourceType = "WorkItem")
	{
        string procName = "WorkItem_TestItemList_Get";

        using (DataTable dt = new DataTable("Connections"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = workItemID;
                    cmd.Parameters.Add("@SourceType", SqlDbType.NVarChar).Value = sourceType;

                    try
                    {
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
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
	}

	public static bool WorkItem_TestItem_Add(int workItemID
		, int testItemID
		, bool archive
		, out bool duplicate
		, out int newID
		, out string errorMsg)
	{
		duplicate = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkItem_TestItem_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = workItemID;
				cmd.Parameters.Add("@TestItemID", SqlDbType.Int).Value = testItemID;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDuplicate = cmd.Parameters["@duplicate"];
				if (paramDuplicate != null)
				{
					bool.TryParse(paramDuplicate.Value.ToString(), out duplicate);
				}
				SqlParameter paramNewID = cmd.Parameters["@newID"];
				if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
				{
					saved = true;
				}
			}
		}

		return saved;
	}

	public static bool WorkItem_TestItem_Update(int workItem_ConnectionID
		, int workItemID
		, int testItemID
		, bool archive
		, out bool duplicate
		, out string errorMsg)
	{
		duplicate = false;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkItem_TestItem_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItem_TestItemID", SqlDbType.Int).Value = workItem_ConnectionID;
				cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = workItemID;
				cmd.Parameters.Add("@TestItemID", SqlDbType.Int).Value = testItemID;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDuplicate = cmd.Parameters["@duplicate"];
				if (paramDuplicate != null)
				{
					bool.TryParse(paramDuplicate.Value.ToString(), out duplicate);
				}
				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	public static bool WorkItem_TestItem_Delete(int workItem_ConnectionID, out bool exists, out string errorMsg)
	{
		string procName = "WorkItem_TestItem_Delete";
		exists = false;
		errorMsg = string.Empty;
		bool deleted = false;

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItem_TestItemID", SqlDbType.Decimal).Value = workItem_ConnectionID;
				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
					cmd.ExecuteNonQuery();

					SqlParameter paramExists = cmd.Parameters["@exists"];
					SqlParameter paramDeleted = cmd.Parameters["@deleted"];

					if (paramExists.Value != DBNull.Value)
					{
						bool.TryParse(paramExists.Value.ToString(), out exists);
					}
					if (paramDeleted.Value != DBNull.Value)
					{
						bool.TryParse(paramDeleted.Value.ToString(), out deleted);
					}
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
					errorMsg = ex.Message;
					deleted = false;
					throw;
				}
				return deleted;
			}
		}
	}

	#endregion Connected Work Items


	#region Item and Item Attributes

	/// <summary>
	/// Get available options for Workload Item details
	/// </summary>
	/// <returns></returns>
	public static DataSet GetAvailableOptions()
	{
		DataSet ds = new DataSet();
		string procName = "WORKITEM_GETOPTIONS";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.TableMappings.Add("Table", "WorkRequest");
				da.TableMappings.Add("Table1", "WorkType");
				da.TableMappings.Add("Table2", "WorkItemType");
				da.TableMappings.Add("Table3", "Priority");
				da.TableMappings.Add("Table4", "User");
				da.TableMappings.Add("Table5", "Status");
				da.TableMappings.Add("Table6", "Allocation");
				da.TableMappings.Add("Table7", "PriorityRank");
				da.TableMappings.Add("Table8", "PercentComplete");
				da.TableMappings.Add("Table9", "System");
				da.TableMappings.Add("Table10", "ProductVersion");
				da.TableMappings.Add("Table11", "MenuType");
				da.TableMappings.Add("Table12", "Menu");
				da.TableMappings.Add("Table13", "PDTDRPhase");
				da.TableMappings.Add("Table14", "WorkloadGroup");
				da.TableMappings.Add("Table15", "WorkArea");
				da.TableMappings.Add("Table16", "TshirtSizes");
				da.TableMappings.Add("Table17", "ProductionStatus");
                da.TableMappings.Add("Table18", "AllocationGroup");
                da.TableMappings.Add("Table19", "Allocations");
                da.TableMappings.Add("Table20", "Rank");
                da.Fill(ds);
			}
		}

		return ds;
	}

    public static DataTable GetAllLookupTables()
    {
        DataTable dt = new DataTable();
        string procName = "WorkItem_Task_SubTask_GetOpt";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }
        }

        return dt;
    }


    /// <summary>
    /// Load Workload Item details
    /// </summary>
    /// <param name="workItemID"></param>
    /// <returns>Datatable of Workload Item Attributes</returns>
    public static DataTable WorkItem_Get(int workItemID = 0)
	{
		string procName = "WORKITEM_GET";

		using (DataTable dt = new DataTable("WORKITEM"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@WORKITEMID", SqlDbType.Int).Value = workItemID == 0 ? (object)DBNull.Value : workItemID;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
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
	}

	public static WorkloadItem WorkItem_GetObject(int workItemID)
	{
		WorkloadItem wi = new WorkloadItem(workItemID);

		DataTable dt = WorkItem_Get(workItemID: workItemID);

		if (dt != null && dt.Rows.Count > 0)
		{
			int tempId = 0;
			bool tempVal = false;
			DataRow dr = dt.Rows[0];

			wi.BugTracker_ID = int.TryParse(dr["BUGTRACKER_ID"].ToString(), out tempId) ? tempId : 0;
			wi.WorkRequestID = int.TryParse(dr["WORKREQUESTID"].ToString(), out tempId) ? tempId : 0;
			wi.WorkItemTypeID = int.TryParse(dr["WORKITEMTYPEID"].ToString(), out tempId) ? tempId : 0;
			wi.WTS_SystemID = int.TryParse(dr["WTS_SYSTEMID"].ToString(), out tempId) ? tempId : 0;
			wi.ProductVersionID = int.TryParse(dr["ProductVersionID"].ToString(), out tempId) ? tempId : 0;
			wi.Production = bool.TryParse(dr["Production"].ToString(), out tempVal) ? tempVal : false;
			wi.Recurring = bool.TryParse(dr["Recurring"].ToString(), out tempVal) ? tempVal : false;
			wi.SR_Number = int.TryParse(dr["SR_Number"].ToString(), out tempId) ? tempId : 0;
			wi.Reproduced_Biz = bool.TryParse(dr["Reproduced_Biz"].ToString(), out tempVal) ? tempVal : false;
			wi.Reproduced_Dev = bool.TryParse(dr["Reproduced_Dev"].ToString(), out tempVal) ? tempVal : false;
			wi.PriorityID = int.TryParse(dr["PRIORITYID"].ToString(), out tempId) ? tempId : 0;
			wi.AllocationID = int.TryParse(dr["ALLOCATIONID"].ToString(), out tempId) ? tempId : 0;
			wi.MenuTypeID = int.TryParse(dr["MenuTypeID"].ToString(), out tempId) ? tempId : 0;
			wi.MenuNameID = int.TryParse(dr["MenuNameID"].ToString(), out tempId) ? tempId : 0;
			wi.SubmittedByID = int.TryParse(dr["SubmittedByID"].ToString(), out tempId) ? tempId : 0;
			wi.AssignedResourceID = int.TryParse(dr["ASSIGNEDRESOURCEID"].ToString(), out tempId) ? tempId : 0;
			wi.PrimaryResourceID = int.TryParse(dr["PRIMARYRESOURCEID"].ToString(), out tempId) ? tempId : 0;
			wi.SecondaryResourceID = int.TryParse(dr["SECONDARYRESOURCEID"].ToString(), out tempId) ? tempId : 0;
			wi.SecondaryResourceRank = int.TryParse(dr["SecondaryResourceRank"].ToString(), out tempId) ? tempId : 0;
			wi.ResourcePriorityRank = int.TryParse(dr["RESOURCEPRIORITYRANK"].ToString(), out tempId) ? tempId : 0;
			wi.PrimaryBusinessResourceID = int.TryParse(dr["PrimaryBusinessResourceID"].ToString(), out tempId) ? tempId : 0;
            wi.SecondaryBusinessResourceID = int.TryParse(dr["SecondaryBusinessResourceID"].ToString(), out tempId) ? tempId : 0;
            wi.PrimaryBusinessRank = int.TryParse(dr["PrimaryBusinessRank"].ToString(), out tempId) ? tempId : 0;
            wi.SecondaryBusinessRank = int.TryParse(dr["SecondaryBusinessRank"].ToString(), out tempId) ? tempId : 0;
            wi.WorkTypeID = int.TryParse(dr["WorkTypeID"].ToString(), out tempId) ? tempId : 0;
			wi.StatusID = int.TryParse(dr["STATUSID"].ToString(), out tempId) ? tempId : 0;
			wi.IVTRequired = bool.TryParse(dr["IVTRequired"].ToString(), out tempVal) ? tempVal : false;
			int.TryParse(dr["Dependency_Count"].ToString(), out tempId);
			wi.Dependency_Count = tempId;
			wi.NeedDate = dr["NEEDDATE"].ToString();
			//wi.EstimatedHours = int.TryParse(dr["ESTIMATEDHOURS"].ToString(), out tempId) ? tempId : 0;
			wi.EstimatedCompletionDate = dr["ESTIMATEDCOMPLETIONDATE"].ToString();
            wi.ActualCompletionDate = dr["ACTUALCOMPLETIONDATE"].ToString();
            wi.CompletionPercent = int.TryParse(dr["COMPLETIONPERCENT"].ToString(), out tempId) ? tempId : 0;
			wi.WorkAreaID = int.TryParse(dr["WorkAreaID"].ToString(), out tempId) ? tempId : 0;
			wi.WorkloadGroupID = int.TryParse(dr["WorkloadGroupID"].ToString(), out tempId) ? tempId : 0;
			wi.Title = dr["TITLE"].ToString();
			wi.Description = dr["DESCRIPTION"].ToString();
			wi.Archive = bool.TryParse(dr["ARCHIVE"].ToString(), out tempVal) ? tempVal : false;
			wi.Deployed_Comm = bool.TryParse(dr["Deployed_Comm"].ToString(), out tempVal) ? tempVal : false;
			wi.Deployed_Test = bool.TryParse(dr["Deployed_Test"].ToString(), out tempVal) ? tempVal : false;
			wi.Deployed_Prod = bool.TryParse(dr["Deployed_Prod"].ToString(), out tempVal) ? tempVal : false;
			wi.DeployedBy_CommID = int.TryParse(dr["DeployedBy_CommID"].ToString(), out tempId) ? tempId : 0;
			wi.DeployedBy_TestID = int.TryParse(dr["DeployedBy_TestID"].ToString(), out tempId) ? tempId : 0;
			wi.DeployedBy_ProdID = int.TryParse(dr["DeployedBy_ProdID"].ToString(), out tempId) ? tempId : 0;
			wi.DeployedDate_Comm = dr["DeployedDate_Comm"].ToString();
			wi.DeployedDate_Test = dr["DeployedDate_Test"].ToString();
			wi.DeployedDate_Prod = dr["DeployedDate_Prod"].ToString();
			wi.PlannedDesignStart = dr["PlannedDesignStart"].ToString();
			wi.PlannedDevStart = dr["PlannedDevStart"].ToString();
			wi.ActualDesignStart = dr["ActualDesignStart"].ToString();
			wi.ActualDevStart = dr["ActualDevStart"].ToString();
			wi.CVTStep = dr["CVTStep"].ToString();
			wi.CVTStatus = dr["CVTStatus"].ToString();
			wi.TesterID = int.TryParse(dr["TesterID"].ToString(), out tempId) ? tempId : 0;

			wi.Comment_Count = int.TryParse(dt.Rows[0]["Comment_Count"].ToString(), out tempId) ? tempId : 0;
			wi.Attachment_Count = int.TryParse(dt.Rows[0]["Attachment_Count"].ToString(), out tempId) ? tempId : 0;
			wi.WorkRequest_Attachment_Count = int.TryParse(dt.Rows[0]["WorkRequest_Attachment_Count"].ToString(), out tempId) ? tempId : 0;
			wi.Task_Count = int.TryParse(dt.Rows[0]["Task_Count"].ToString(), out tempId) ? tempId : 0;

			wi.Signed_Bus = bool.TryParse(dr["Signed_Bus"].ToString(), out tempVal) ? tempVal : false;
			wi.Signed_Dev = bool.TryParse(dr["Signed_Dev"].ToString(), out tempVal) ? tempVal : false;
			wi.Signed_Bus_User = dr["Signed_Bus_User"].ToString();
			wi.Signed_Dev_User = dr["Signed_Dev_User"].ToString();
			wi.SignedDate_Bus = dr["SignedDate_Bus"].ToString();
			wi.SignedDate_Dev = dr["SignedDate_Dev"].ToString();

			wi.ProductionStatusID = int.TryParse(dr["ProductionStatusID"].ToString(), out tempId) ? tempId : 0;
            wi.PDDTDR_PHASEID = int.TryParse(dr["PhaseID"].ToString(), out tempId) ? tempId : 0;
            wi.BusinessReview = bool.TryParse(dr["BusinessReview"].ToString(), out tempVal) ? tempVal : false;
            wi.AssignedToRankID = int.TryParse(dr["AssignedToRankID"].ToString(), out tempId) ? tempId : 0;
        }

        return wi;
	}

    /// <summary>
    /// Add new Workload Item
    /// </summary>
    /// <param name="CopySubTasks"></param>
	/// <param name="OriginalWorkItemID"></param>
    /// <param name="newID"></param>
    /// <param name="errorMsg"></param>
    /// <returns>boolean value representing save success</returns>
    public static bool WorkItem_Add(WorkloadItem workItem
        , string CopySubTasks  // 12817 - 7 - Optionally, copy subtasks.
        , string OriginalWorkItemID
        , out int newID
		, out string errorMsg)
	{
		newID = 0;
        int newSubID = 0;
        errorMsg = string.Empty;
		bool saved = false;
		DateTime dtDate;


        bool copySubTasks = false;
        if (CopySubTasks.ToLower() == "true")
            copySubTasks = true;

        int originalWorkItemID = int.Parse(OriginalWorkItemID);

        string procName = "WorkItem_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkRequestID", SqlDbType.Int).Value = workItem.WorkRequestID == 0 ? (object)DBNull.Value : workItem.WorkRequestID;
                cmd.Parameters.Add("@WorkItemTypeID", SqlDbType.Int).Value = workItem.WorkItemTypeID == 0 ? (object)DBNull.Value : workItem.WorkItemTypeID;
				cmd.Parameters.Add("@WTS_SystemID", SqlDbType.Int).Value = workItem.WTS_SystemID == 0 ? (object)DBNull.Value : workItem.WTS_SystemID;
				cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = workItem.ProductVersionID == 0 ? (object)DBNull.Value : workItem.ProductVersionID;
				cmd.Parameters.Add("@Production", SqlDbType.Bit).Value = workItem.Production ? 1 : 0;
				cmd.Parameters.Add("@Recurring", SqlDbType.Bit).Value = workItem.Recurring ? 1 : 0;
                cmd.Parameters.Add("@SR_Number", SqlDbType.Int).Value = workItem.SR_Number == 0 ? (object)DBNull.Value : workItem.SR_Number;
				cmd.Parameters.Add("@Reproduced_Biz", SqlDbType.Bit).Value = workItem.Reproduced_Biz ? 1 : 0;
				cmd.Parameters.Add("@Reproduced_Dev", SqlDbType.Bit).Value = workItem.Reproduced_Dev ? 1 : 0;
				cmd.Parameters.Add("@PriorityID", SqlDbType.Int).Value = workItem.PriorityID;
				cmd.Parameters.Add("@AllocationID", SqlDbType.Int).Value = workItem.AllocationID == 0 ? (object)DBNull.Value : workItem.AllocationID;
				cmd.Parameters.Add("@MenuTypeID", SqlDbType.Int).Value = workItem.MenuTypeID == 0 ? (object)DBNull.Value : workItem.MenuTypeID;
				cmd.Parameters.Add("@MenuNameID", SqlDbType.Int).Value = workItem.MenuNameID == 0 ? (object)DBNull.Value : workItem.MenuNameID;
				cmd.Parameters.Add("@AssignedResourceID", SqlDbType.Int).Value = workItem.AssignedResourceID;
				cmd.Parameters.Add("@ResourcePriorityRank", SqlDbType.Int).Value = workItem.ResourcePriorityRank == 0 ? (object)DBNull.Value  :workItem.ResourcePriorityRank;
				cmd.Parameters.Add("@SecondaryResourceRank", SqlDbType.Int).Value = workItem.SecondaryResourceRank == 0 ? (object)DBNull.Value : workItem.SecondaryResourceRank;
				cmd.Parameters.Add("@PrimaryBusinessRank", SqlDbType.Int).Value = workItem.PrimaryBusinessRank == 0 ? (object)DBNull.Value : workItem.PrimaryBusinessRank;
                cmd.Parameters.Add("@SecondaryBusinessRank", SqlDbType.Int).Value = workItem.SecondaryBusinessRank == 0 ? (object)DBNull.Value : workItem.SecondaryBusinessRank;
                cmd.Parameters.Add("@PrimaryResourceID", SqlDbType.Int).Value = workItem.PrimaryResourceID == 0 ? (object)DBNull.Value : workItem.PrimaryResourceID;
				cmd.Parameters.Add("@SecondaryResourceID", SqlDbType.Int).Value = workItem.SecondaryResourceID == 0 ? (object)DBNull.Value : workItem.SecondaryResourceID;
				cmd.Parameters.Add("@PrimaryBusinessResourceID", SqlDbType.Int).Value = workItem.PrimaryBusinessResourceID == 0 ? (object)DBNull.Value : workItem.PrimaryBusinessResourceID;
                cmd.Parameters.Add("@SecondaryBusinessResourceID", SqlDbType.Int).Value = workItem.SecondaryBusinessResourceID == 0 ? (object)DBNull.Value : workItem.SecondaryBusinessResourceID;
                cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workItem.WorkTypeID;
				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = workItem.StatusID;
				cmd.Parameters.Add("@IVTRequired", SqlDbType.Bit).Value = workItem.IVTRequired ? 1 : 0;
				cmd.Parameters.Add("@NeedDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.NeedDate) || !DateTime.TryParse(workItem.NeedDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@EstimatedEffortID", SqlDbType.Int).Value = workItem.EstimatedEffortID == 0 ? (object)DBNull.Value : workItem.EstimatedEffortID;
				cmd.Parameters.Add("@EstimatedCompletionDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.EstimatedCompletionDate) || !DateTime.TryParse(workItem.EstimatedCompletionDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
                cmd.Parameters.Add("@ActualCompletionDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.ActualCompletionDate) || !DateTime.TryParse(workItem.ActualCompletionDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
                cmd.Parameters.Add("@CompletionPercent", SqlDbType.Int).Value = workItem.CompletionPercent;
				cmd.Parameters.Add("@WorkAreaID", SqlDbType.Int).Value = workItem.WorkAreaID == 0 ? (object)DBNull.Value : workItem.WorkAreaID;
				cmd.Parameters.Add("@WorkloadGroupID", SqlDbType.Int).Value = workItem.WorkloadGroupID == 0 ? (object)DBNull.Value : workItem.WorkloadGroupID;
				cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = workItem.Title;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = workItem.Description;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = workItem.Archive ? 1 : 0;
				cmd.Parameters.Add("@Deployed_Comm", SqlDbType.Bit).Value = workItem.Deployed_Comm ? 1 : 0;
				cmd.Parameters.Add("@Deployed_Test", SqlDbType.Bit).Value = workItem.Deployed_Test ? 1 : 0;
				cmd.Parameters.Add("@Deployed_Prod", SqlDbType.Bit).Value = workItem.Deployed_Prod ? 1 : 0;
				cmd.Parameters.Add("@DeployedBy_CommID", SqlDbType.Int).Value = workItem.DeployedBy_CommID == 0 ? (object)DBNull.Value : workItem.DeployedBy_CommID;
				cmd.Parameters.Add("@DeployedBy_TestID", SqlDbType.Int).Value = workItem.DeployedBy_TestID == 0 ? (object)DBNull.Value : workItem.DeployedBy_TestID;
				cmd.Parameters.Add("@DeployedBy_ProdID", SqlDbType.Int).Value = workItem.DeployedBy_ProdID == 0 ? (object)DBNull.Value : workItem.DeployedBy_ProdID;
				cmd.Parameters.Add("@DeployedDate_Comm", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.DeployedDate_Comm) || !DateTime.TryParse(workItem.DeployedDate_Comm, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@DeployedDate_Test", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.DeployedDate_Test) || !DateTime.TryParse(workItem.DeployedDate_Test, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@DeployedDate_Prod", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.DeployedDate_Prod) || !DateTime.TryParse(workItem.DeployedDate_Prod, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@PlannedDesignStart", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.PlannedDesignStart) || !DateTime.TryParse(workItem.PlannedDesignStart, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@PlannedDevStart", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.PlannedDevStart) || !DateTime.TryParse(workItem.PlannedDevStart, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@ActualDesignStart", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.ActualDesignStart) || !DateTime.TryParse(workItem.ActualDesignStart, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@ActualDevStart", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.ActualDevStart) || !DateTime.TryParse(workItem.ActualDevStart, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@CVTStep", SqlDbType.NVarChar).Value = workItem.CVTStep;
				cmd.Parameters.Add("@CVTStatus", SqlDbType.NVarChar).Value = workItem.CVTStatus;
				cmd.Parameters.Add("@TesterID", SqlDbType.Int).Value = workItem.TesterID == 0 ? (object)DBNull.Value : workItem.TesterID;
				cmd.Parameters.Add("@Signed_Bus", SqlDbType.Bit).Value = workItem.Signed_Bus ? 1 : 0;
				cmd.Parameters.Add("@Signed_Dev", SqlDbType.Bit).Value = workItem.Signed_Dev ? 1 : 0;
				cmd.Parameters.Add("@ProductionStatusID", SqlDbType.Int).Value = workItem.ProductionStatusID == 0 ? (object)DBNull.Value : workItem.ProductionStatusID;
				cmd.Parameters.Add("@SubmittedByID", SqlDbType.Int).Value = UserManagement.GetUserId_FromUsername();
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@PDDTDR_PHASEID", SqlDbType.Int).Value = workItem.PDDTDR_PHASEID;
                cmd.Parameters.Add("@AssignedToRankID", SqlDbType.Int).Value = workItem.AssignedToRankID;
                cmd.Parameters.Add("@BusinessReview", SqlDbType.Bit).Value = workItem.BusinessReview ? 1 : 0;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramNewID = cmd.Parameters["@newID"];
				if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
				{
					saved = true;
                }
            }  // Using commmand
		} // Using connection

        if (copySubTasks)
        {
            using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
            conn.Open();

            procName = "WorkItem_Task_Add";

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramNewSubID;
                    DataTable dtSubTasks = new DataTable();

                    int subtaskNbr = 0;

                    String sqlStatement = "SELECT PRIORITYID, TITLE, [DESCRIPTION], ASSIGNEDRESOURCEID, PRIMARYRESOURCEID, SecondaryResourceID, PrimaryBusResourceID, SecondaryBusResourceID, SubmittedByID, ESTIMATEDSTARTDATE, ACTUALSTARTDATE, ";
                    sqlStatement += "EstimatedEffortID, ActualEffortID, ACTUALENDDATE, STATUSID, BusinessRank, SRNumber, ProductVersionID, WORKITEMTYPEID, CREATEDBY FROM WORKITEM_TASK ";
                    sqlStatement += "WHERE WORKITEMID = " + originalWorkItemID;

                    dtSubTasks = WTSData.GetGeneric(WTSCommon.WTS_ConnectionString, sqlStatement, null);
                    foreach (DataRow dr in dtSubTasks.Rows)
                    {
                        saved = false;
                        subtaskNbr += 1;

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = newID;
                        cmd.Parameters.Add("@PriorityID", SqlDbType.Int).Value = dr["PRIORITYID"];
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = dr["TITLE"];
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = dr["DESCRIPTION"];
                        cmd.Parameters.Add("@AssignedResourceID", SqlDbType.Int).Value = dr["ASSIGNEDRESOURCEID"];
                        cmd.Parameters.Add("@PrimaryResourceID", SqlDbType.Int).Value = dr["PRIMARYRESOURCEID"];
                        cmd.Parameters.Add("@SecondaryResourceID", SqlDbType.Int).Value = dr["SecondaryResourceID"];
                        cmd.Parameters.Add("@PrimaryBusResourceID", SqlDbType.Int).Value = dr["PRIMARYBUSRESOURCEID"];
                        cmd.Parameters.Add("@SecondaryBusResourceID", SqlDbType.Int).Value = dr["SecondaryBusResourceID"];
                        cmd.Parameters.Add("@SubmittedByID", SqlDbType.Int).Value = dr["SubmittedByID"];
                        cmd.Parameters.Add("@PlannedStartDate", SqlDbType.DateTime).Value = dr["ESTIMATEDSTARTDATE"];
                        cmd.Parameters.Add("@ActualStartDate", SqlDbType.DateTime).Value = dr["ACTUALSTARTDATE"];
                        cmd.Parameters.Add("@EstimatedEffortID", SqlDbType.Int).Value = dr["EstimatedEffortID"];
                        cmd.Parameters.Add("@ActualEffortID", SqlDbType.Int).Value = dr["ActualEffortID"];
                        cmd.Parameters.Add("@ActualEndDate", SqlDbType.DateTime).Value = dr["ACTUALENDDATE"];
                        cmd.Parameters.Add("@CompletionPercent", SqlDbType.Int).Value = 0; // Set to 0%
                        cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = 1; // Set to New status
                        cmd.Parameters.Add("@BusinessRank", SqlDbType.Int).Value = dr["BusinessRank"];
                        cmd.Parameters.Add("@SortOrder", SqlDbType.Int).Value = null;
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                        cmd.Parameters.Add("@SRNumber", SqlDbType.NVarChar).Value = dr["SRNumber"];
                        cmd.Parameters.Add("@AssignedToRankID", SqlDbType.Int).Value = 30; // Set to Unprioritized Workload
                        cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value =  dr["ProductVersionID"];
                        cmd.Parameters.Add("@WorkItemTypeID", SqlDbType.Int).Value =  dr["WORKITEMTYPEID"];
                        cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        paramNewSubID = cmd.Parameters["@newID"];
                        if (paramNewSubID != null && int.TryParse(paramNewSubID.Value.ToString(), out newSubID) && newSubID > 0)
                        {
                            saved = true;
                        }
                    }  // For each row
                }  // Using commmand
            }  // Using connection
        }  // Copy sub-tasks?
        // Copy sub-tasks?
        return saved;
	}

	/// <summary>
	/// Update details of existing Workload Item
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns>boolean value representing save success</returns>
	public static int WorkItem_Update(WorkloadItem workItem
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		int saved = 0;
		DateTime dtDate;

		string procName = "WorkItem_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = workItem.WorkItemID;
				cmd.Parameters.Add("@WorkRequestID", SqlDbType.Int).Value = workItem.WorkRequestID == 0 ? (object)DBNull.Value : workItem.WorkRequestID;
                cmd.Parameters.Add("@WorkItemTypeID", SqlDbType.Int).Value = workItem.WorkItemTypeID == 0 ? (object)DBNull.Value : workItem.WorkItemTypeID;
				cmd.Parameters.Add("@WTS_SystemID", SqlDbType.Int).Value = workItem.WTS_SystemID == 0 ? (object)DBNull.Value : workItem.WTS_SystemID;
				cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = workItem.ProductVersionID == 0 ? (object)DBNull.Value : workItem.ProductVersionID;
				cmd.Parameters.Add("@Production", SqlDbType.Bit).Value = workItem.Production ? 1 : 0;
				cmd.Parameters.Add("@Recurring", SqlDbType.Bit).Value = workItem.Recurring ? 1 : 0;
                cmd.Parameters.Add("@SR_Number", SqlDbType.Int).Value = workItem.SR_Number == 0 ? (object)DBNull.Value : workItem.SR_Number;
				cmd.Parameters.Add("@Reproduced_Biz", SqlDbType.Bit).Value = workItem.Reproduced_Biz ? 1 : 0;
				cmd.Parameters.Add("@Reproduced_Dev", SqlDbType.Bit).Value = workItem.Reproduced_Dev ? 1 : 0;
				cmd.Parameters.Add("@PriorityID", SqlDbType.Int).Value = workItem.PriorityID;
				cmd.Parameters.Add("@AllocationID", SqlDbType.Int).Value = workItem.AllocationID == 0 ? (object)DBNull.Value : workItem.AllocationID;
				cmd.Parameters.Add("@MenuTypeID", SqlDbType.Int).Value = workItem.MenuTypeID == 0 ? (object)DBNull.Value : workItem.MenuTypeID;
				cmd.Parameters.Add("@MenuNameID", SqlDbType.Int).Value = workItem.MenuNameID == 0 ? (object)DBNull.Value : workItem.MenuNameID;
				cmd.Parameters.Add("@AssignedResourceID", SqlDbType.Int).Value = workItem.AssignedResourceID;
				cmd.Parameters.Add("@ResourcePriorityRank", SqlDbType.Int).Value = workItem.ResourcePriorityRank == 0 ? (object)DBNull.Value : workItem.ResourcePriorityRank;
				cmd.Parameters.Add("@SecondaryResourceRank", SqlDbType.Int).Value = workItem.SecondaryResourceRank == 0 ? (object)DBNull.Value : workItem.SecondaryResourceRank;  // 6
				cmd.Parameters.Add("@PrimaryBusinessRank", SqlDbType.Int).Value = workItem.PrimaryBusinessRank == 0 ? (object)DBNull.Value : workItem.PrimaryBusinessRank;
                cmd.Parameters.Add("@SecondaryBusinessRank", SqlDbType.Int).Value = workItem.SecondaryBusinessRank == 0 ? (object)DBNull.Value : workItem.SecondaryBusinessRank;
                cmd.Parameters.Add("@PrimaryResourceID", SqlDbType.Int).Value = workItem.PrimaryResourceID == 0 ? (object)DBNull.Value : workItem.PrimaryResourceID;
				cmd.Parameters.Add("@SecondaryResourceID", SqlDbType.Int).Value = workItem.SecondaryResourceID == 0 ? (object)DBNull.Value : workItem.SecondaryResourceID;
				cmd.Parameters.Add("@PrimaryBusinessResourceID", SqlDbType.Int).Value = workItem.PrimaryBusinessResourceID == 0 ? (object)DBNull.Value : workItem.PrimaryBusinessResourceID;
                cmd.Parameters.Add("@SecondaryBusinessResourceID", SqlDbType.Int).Value = workItem.SecondaryBusinessResourceID == 0 ? (object)DBNull.Value : workItem.SecondaryBusinessResourceID;
                cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workItem.WorkTypeID;
				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = workItem.StatusID;
				cmd.Parameters.Add("@IVTRequired", SqlDbType.Bit).Value = workItem.IVTRequired ? 1 : 0;
				cmd.Parameters.Add("@NeedDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.NeedDate) || !DateTime.TryParse(workItem.NeedDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
				//cmd.Parameters.Add("@EstimatedHours", SqlDbType.Int).Value = workItem.EstimatedHours == 0 ? (object)DBNull.Value : workItem.EstimatedHours;
				cmd.Parameters.Add("@EstimatedEffortID", SqlDbType.Int).Value = workItem.EstimatedEffortID == 0 ? (object)DBNull.Value : workItem.EstimatedEffortID;
				cmd.Parameters.Add("@EstimatedCompletionDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.EstimatedCompletionDate) || !DateTime.TryParse(workItem.EstimatedCompletionDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@CompletionPercent", SqlDbType.Int).Value = workItem.CompletionPercent;
				cmd.Parameters.Add("@WorkAreaID", SqlDbType.Int).Value = workItem.WorkAreaID == 0 ? (object)DBNull.Value : workItem.WorkAreaID;
				cmd.Parameters.Add("@WorkloadGroupID", SqlDbType.Int).Value = workItem.WorkloadGroupID == 0 ? (object)DBNull.Value : workItem.WorkloadGroupID;
				cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = workItem.Title;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = workItem.Description;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = workItem.Archive ? 1 : 0;
				cmd.Parameters.Add("@Deployed_Comm", SqlDbType.Bit).Value = workItem.Deployed_Comm ? 1 : 0;
				cmd.Parameters.Add("@Deployed_Test", SqlDbType.Bit).Value = workItem.Deployed_Test ? 1 : 0;
				cmd.Parameters.Add("@Deployed_Prod", SqlDbType.Bit).Value = workItem.Deployed_Prod ? 1 : 0;
				cmd.Parameters.Add("@DeployedBy_CommID", SqlDbType.Int).Value = workItem.DeployedBy_CommID == 0 ? (object)DBNull.Value : workItem.DeployedBy_CommID;
				cmd.Parameters.Add("@DeployedBy_TestID", SqlDbType.Int).Value = workItem.DeployedBy_TestID == 0 ? (object)DBNull.Value : workItem.DeployedBy_TestID;
				cmd.Parameters.Add("@DeployedBy_ProdID", SqlDbType.Int).Value = workItem.DeployedBy_ProdID == 0 ? (object)DBNull.Value : workItem.DeployedBy_ProdID;
				cmd.Parameters.Add("@DeployedDate_Comm", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.DeployedDate_Comm) || !DateTime.TryParse(workItem.DeployedDate_Comm, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@DeployedDate_Test", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.DeployedDate_Test) || !DateTime.TryParse(workItem.DeployedDate_Test, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@DeployedDate_Prod", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.DeployedDate_Prod) || !DateTime.TryParse(workItem.DeployedDate_Prod, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@PlannedDesignStart", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.PlannedDesignStart) || !DateTime.TryParse(workItem.PlannedDesignStart, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@PlannedDevStart", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.PlannedDevStart) || !DateTime.TryParse(workItem.PlannedDevStart, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@ActualDesignStart", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.ActualDesignStart) || !DateTime.TryParse(workItem.ActualDesignStart, out dtDate)) ? (Object)DBNull.Value : dtDate;
				cmd.Parameters.Add("@ActualDevStart", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.ActualDevStart) || !DateTime.TryParse(workItem.ActualDevStart, out dtDate)) ? (Object)DBNull.Value : dtDate;
                cmd.Parameters.Add("@ActualCompletionDate", SqlDbType.DateTime).Value = (string.IsNullOrWhiteSpace(workItem.ActualCompletionDate) || !DateTime.TryParse(workItem.ActualCompletionDate, out dtDate)) ? (Object)DBNull.Value : dtDate;
                cmd.Parameters.Add("@CVTStep", SqlDbType.NVarChar).Value = workItem.CVTStep;
				cmd.Parameters.Add("@CVTStatus", SqlDbType.NVarChar).Value = workItem.CVTStatus;
				cmd.Parameters.Add("@TesterID", SqlDbType.Int).Value = workItem.TesterID == 0 ? (object)DBNull.Value : workItem.TesterID;
				cmd.Parameters.Add("@Signed_Bus", SqlDbType.Bit).Value = workItem.Signed_Bus ? 1 : 0;
				cmd.Parameters.Add("@Signed_Dev", SqlDbType.Bit).Value = workItem.Signed_Dev ? 1 : 0;
				cmd.Parameters.Add("@ProductionStatusID", SqlDbType.Int).Value = workItem.ProductionStatusID == 0 ? (object)DBNull.Value : workItem.ProductionStatusID;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@PDDTDR_PHASEID", SqlDbType.Int).Value = workItem.PDDTDR_PHASEID == 0 ? (object)DBNull.Value : workItem.PDDTDR_PHASEID;
                cmd.Parameters.Add("@AssignedToRankID", SqlDbType.Int).Value = workItem.AssignedToRankID;
                cmd.Parameters.Add("@BusinessReview", SqlDbType.Bit).Value = workItem.BusinessReview ? 1 : 0;
                cmd.Parameters.Add("@saved", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
                    if (paramSaved != null)
                    {
                        int.TryParse(paramSaved.Value.ToString(), out saved);
                    }
                }
			}
		}

		return saved;
	}

	/// <summary>
	/// Update details of existing Workload Item
	/// Subset of attributes from QM Attribute grid
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns>boolean value representing save success</returns>
	public static bool WorkItem_QM_Update(WorkloadItem workItem
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;
		DateTime dtDate;

		string procName = "WorkItem_QM_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = workItem.WorkItemID;
				cmd.Parameters.Add("@WorkRequestID", SqlDbType.Int).Value = workItem.WorkRequestID;
				cmd.Parameters.Add("@WorkItemTypeID", SqlDbType.Int).Value = workItem.WorkItemTypeID == 0 ? (object)DBNull.Value : workItem.WorkItemTypeID;
				cmd.Parameters.Add("@WTS_SystemID", SqlDbType.Int).Value = workItem.WTS_SystemID == 0 ? (object)DBNull.Value : workItem.WTS_SystemID;
				cmd.Parameters.Add("@AllocationID", SqlDbType.Int).Value = workItem.AllocationID == 0 ? (object)DBNull.Value : workItem.AllocationID;
				cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = workItem.ProductVersionID == 0 ? (object)DBNull.Value : workItem.ProductVersionID;
				cmd.Parameters.Add("@Production", SqlDbType.Bit).Value = workItem.Production ? 1 : 0;
				cmd.Parameters.Add("@PriorityID", SqlDbType.Int).Value = workItem.PriorityID;
				cmd.Parameters.Add("@PrimaryResourceID", SqlDbType.Int).Value = workItem.PrimaryResourceID == 0 ? (object)DBNull.Value : workItem.PrimaryResourceID;
				cmd.Parameters.Add("@AssignedResourceID", SqlDbType.Int).Value = workItem.AssignedResourceID;
				cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workItem.WorkTypeID;
				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = workItem.StatusID;
				cmd.Parameters.Add("@CompletionPercent", SqlDbType.Int).Value = workItem.CompletionPercent;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = workItem.Archive ? 1 : 0;
				cmd.Parameters.Add("@ProductionStatusID", SqlDbType.Int).Value = workItem.ProductionStatusID == 0 ? (object)DBNull.Value : workItem.ProductionStatusID;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}


	public static int WorkItem_MassChange(string fieldName
		, string fromValue
		, string toValue
		, bool includeArchive
		, bool myData
		, out string errorMsg)
	{
		int rowsUpdated = 0;
		errorMsg = string.Empty;

		string procName = "MassChange_WorkItem";

		dynamic fields = (Dictionary<string, object>)HttpContext.Current.Session["filters_Work"];
		SqlParameter[] sps = Filtering.GetWorkFilter_SqlParamsArray(fields, "");

		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@FieldName", SqlDbType.NVarChar).Value = fieldName;
				cmd.Parameters.Add("@FromValue", SqlDbType.NVarChar).Value = fromValue;
				cmd.Parameters.Add("@ToValue", SqlDbType.NVarChar).Value = toValue;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
				cmd.Parameters.Add("@IncludeArchived", SqlDbType.Int).Value = includeArchive == false ? (object)DBNull.Value : 1;

				//existing filters - parse dynamic fields into SQL Parameter objects
				if (fields != null && sps != null && sps.Length > 0)
				{
					cmd.Parameters.AddRange(sps);
				}

				cmd.Parameters.Add("@OwnedBy", SqlDbType.Int).Value = myData ? UserManagement.GetUserId_FromUsername() : (object)DBNull.Value;
				cmd.Parameters.Add("@RowsUpdated", SqlDbType.Int).Direction = ParameterDirection.Output;

				rowsUpdated = cmd.ExecuteNonQuery();

				SqlParameter paramRowCount = cmd.Parameters["@RowsUpdated"];
				if (paramRowCount != null)
				{
					int.TryParse(paramRowCount.Value.ToString(), out rowsUpdated);
				}
			}
		}

		return rowsUpdated;
	}

	#endregion Items and Item Attributes


	/// <summary>
	/// Load Tasks for specified Work Item
	/// </summary>
	/// <param name="workItemID"></param>
	/// <returns>Datatable of Workload Tasks</returns>
	public static DataTable WorkItem_GetTaskList(int workItemID = 0, int showArchived = 0, bool showBacklog = false, string statusList = null, string systemList = null)
	{
		string procName = "WORKITEM_GETTASKLIST";

		using (DataTable dt = new DataTable("Task"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WORKITEMID", SqlDbType.Int).Value = workItemID == 0 ? (object)DBNull.Value : workItemID;
					cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived == 0 ? (object)DBNull.Value : showArchived;
                    cmd.Parameters.Add("@ShowBacklog", SqlDbType.Bit).Value = showBacklog;
                    cmd.Parameters.Add("@StatusList", SqlDbType.VarChar).Value = !string.IsNullOrWhiteSpace(statusList) && statusList != "0" ? statusList : (object)DBNull.Value;
                    cmd.Parameters.Add("@SystemList", SqlDbType.VarChar).Value = !string.IsNullOrWhiteSpace(systemList) && systemList != "0" ? systemList : (object)DBNull.Value;

                    try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
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
	}

    public static DataTable WorkItem_GetOverview(string SelectedAssigned = "", string SelectedStatus = "")
    {
        string procName = "WORKITEM_GET_OVERVIEW";

        using (DataTable dt = new DataTable("Task"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SelectedAssigned", SqlDbType.NVarChar).Value = SelectedAssigned;
                    cmd.Parameters.Add("@SelectedStatus", SqlDbType.NVarChar).Value = SelectedStatus;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    // Not in use at this time...
    public static DataTable WorkItem_GetTaskList_Assigned(int workItemID = 0, int showArchived = 0, bool showBacklog = false)
    {
        string procName = "WORKITEM_GETTASKLIST_ASSIGNED";

        using (DataTable dt = new DataTable("Task"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WORKITEMID", SqlDbType.Int).Value = workItemID == 0 ? (object)DBNull.Value : workItemID;
                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived == 0 ? (object)DBNull.Value : showArchived;
                    cmd.Parameters.Add("@ShowBacklog", SqlDbType.Bit).Value = showBacklog;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    #region Comments

    /// <summary>
    /// Get comments for specified Workload Item
    /// </summary>
    /// <param name="workItemID"></param>
    /// <returns></returns>
    public static DataTable WorkItem_GetCommentList(int workItemID = 0)
	{
		string procName = "WORKITEM_GETCOMMENTLIST";

		using (DataTable dt = new DataTable("Comment"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@WORKITEMID", SqlDbType.Int).Value = workItemID == 0 ? (object)DBNull.Value : workItemID;
					cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = 0;

					try
					{
						using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
						{
							if (dr != null && dr.HasRows)
							{
								dt.Load(dr);
								return dt;
							}
							else
							{
								return null;
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
	}


	public static bool WorkItem_Comment_Add(out int newID, out string errorMsg, int workItemID = 0, int parentCommentID = 0, string comment_text = "")
	{
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkItem_Comment_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = workItemID;
				cmd.Parameters.Add("@ParentCommentID", SqlDbType.Int).Value = parentCommentID == 0 ? (object)DBNull.Value : parentCommentID;
				cmd.Parameters.Add("@Comment_Text", SqlDbType.NVarChar).Value = comment_text;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramNewID = cmd.Parameters["@newID"];
				if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newID) && newID > 0)
				{
					saved = true;
				}
			}
		}

		return saved;
	}

	public static bool WorkItem_Comment_Update(out string errorMsg, int commentId = 0, string comment_text = "")
	{
		errorMsg = string.Empty;

		return WTSData.Comment_Update(out errorMsg, commentId, comment_text);
	}

	public static bool WorkItem_Comment_Delete(out string errorMsg, int workItemID = 0, int commentId = 0)
	{
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "WorkItem_Comment_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = workItemID;
				cmd.Parameters.Add("@CommentID", SqlDbType.Int).Value = commentId;

				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDeleted = cmd.Parameters["@deleted"];
				if (paramDeleted != null)
				{
					bool.TryParse(paramDeleted.Value.ToString(), out deleted);
				}
			}
		}

		return deleted;
	}

	#endregion Comments


	#region Attachments

	public static DataTable WorkItem_GetAttachmentList(int workItemID)
	{
		string procName = "WorkItem_GetAttachmentList";

		using (DataTable dt = new DataTable("Attachment"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WORKITEMID", SqlDbType.Int).Value = workItemID;

					using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
					{
						if (dr != null && dr.HasRows)
						{
							dt.Load(dr);
							return dt;
						}
						else
						{
							return null;
						}
					}
				}
			}
		}
	}

	public static DataTable Attachment_Get(int attachmentID = 0)
	{
		return null;
	}

	public static bool Attachment_Add(int workItemID
		, int attachmentTypeID
		, string fileName
		, string title
		, string description
		, byte[] fileData
		, int extensionID
		, out int newAttachmentID
		, out int newWorkItemAttachmentID
		, out string errorMsg)
	{
		newAttachmentID = 0;
		newWorkItemAttachmentID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		saved = WTSData.Attachment_Add(attachmentTypeID: attachmentTypeID, fileName: fileName, title: title, description: description, fileData: fileData, extensionID: extensionID, newID: out newAttachmentID, errorMsg: out errorMsg);
		if (saved && newAttachmentID > 0)
		{
			string procName = "WorkItem_Attachment_Add";
			using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(procName, conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = workItemID;
					cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = newAttachmentID;
					cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

					cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

					cmd.ExecuteNonQuery();

					SqlParameter paramNewID = cmd.Parameters["@newID"];
					if (paramNewID != null && int.TryParse(paramNewID.Value.ToString(), out newWorkItemAttachmentID) && newWorkItemAttachmentID > 0)
					{
						saved = true;
					}
				}
			}
		}

		return saved;
	}

	public static bool Attachment_Delete(int workItemID
		, int attachmentID
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "WorkItem_Attachment_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = workItemID;
				cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = attachmentID;

				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramDeleted = cmd.Parameters["@deleted"];
				if (paramDeleted != null)
				{
					bool.TryParse(paramDeleted.Value.ToString(), out deleted);
				}
			}
		}

		return deleted;
	}

    #endregion Attachments

}



    public sealed class SRItem
    {

    public static DataTable SRView_Get(int showArchived = 0, int showClosed = 0, bool ShowBacklog = false, bool myData = true)
    {
        string procName = "SRView_Get";

        using (DataTable dt = new DataTable("SRView"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived == 0 ? (object)DBNull.Value : showArchived;
                    cmd.Parameters.Add("@ShowClosed", SqlDbType.Int).Value = showClosed == 0 ? (object)DBNull.Value : showClosed;
                    cmd.Parameters.Add("@ShowBacklog", SqlDbType.Bit).Value = ShowBacklog;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.Int).Value = myData ? UserManagement.GetUserId_FromUsername() : (object)DBNull.Value;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    public static DataTable SR_List_GetAll()
    {
        string procName = "SR_List_GetAll";

        using (DataTable dt = new DataTable("SR_List_GetAll"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null && dr.HasRows)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }

    public static DataTable SRList_Get(int workRequestID = 0, int workloadItemID = 0, bool myData = true)
        {
            //Return dummy data
            DataTable dtSR = new DataTable("SR");
            dtSR.Columns.Add("WorkRequestID", typeof(System.Int32));
            dtSR.Columns.Add("ItemID", typeof(System.Int32));
            dtSR.Columns.Add("SR_Number", typeof(System.Int32));
            dtSR.Columns.Add("Websystem", typeof(System.String));
            dtSR.Columns.Add("CreatedDate", typeof(System.DateTime));
            dtSR.Columns.Add("Priority", typeof(System.String));
            dtSR.Columns.Add("Description", typeof(System.String));
            dtSR.Columns.Add("Status", typeof(System.String));
            dtSR.Columns.Add("ITI_POC", typeof(System.String));
            dtSR.Columns.Add("Release", typeof(System.String));
            dtSR.Columns.Add("Last_Reply", typeof(System.String));
            dtSR.Columns.Add("Resolved_Date", typeof(System.String));

            dtSR.AcceptChanges();

            DataRow rSR = dtSR.NewRow();
            rSR["WorkRequestID"] = 10216;
            rSR["ItemID"] = 13006;
            rSR["SR_Number"] = 19589;
            rSR["Websystem"] = "CAFDEx";
            rSR["CreatedDate"] = "02/23/2014";
            rSR["Priority"] = "Medium";
            rSR["Description"] = "SRMA and UID have some areas(e.g. 227s, maintenance history, schedule collaboration with MAJCOMs, etc.) that require refactoring to improve the usability and performance of the system";
            rSR["Status"] = "Collaboration / In-Work";
            rSR["ITI_POC"] = "David.A.Coulter";
            rSR["Release"] = "0.1.15.1.B";
            rSR["Last_Reply"] = "";
            rSR["Resolved_Date"] = "";
            dtSR.Rows.Add(rSR);

            dtSR.AcceptChanges();

            return dtSR;
        }

}

public sealed class Affiliated
{
    public static DataTable AffiliatedList_Get(int WORKITEMID, int WTS_SYSTEMID, int ProductVersionID, int ResourceGroupID, int WorkActivityID, string AORReleaseIDs)
    {
        string procName = "AffiliatedList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WORKITEMID", SqlDbType.Int).Value = WORKITEMID;
                    cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = WTS_SYSTEMID;
                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
                    cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = ResourceGroupID;
                    cmd.Parameters.Add("@WorkItemTypeID", SqlDbType.Int).Value = WorkActivityID;
                    cmd.Parameters.Add("@AORReleaseIDs", SqlDbType.NVarChar).Value = AORReleaseIDs;

                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (dr != null)
                            {
                                dt.Load(dr);
                                return dt;
                            }
                            else
                            {
                                return null;
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
    }
}