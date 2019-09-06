﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Web;
using System.Xml;

using WTS.Events;

public sealed class SR
{
    public static Dictionary<string, string> SR_Add(int SubmittedByID, int SRTypeID, int PriorityID, string Description)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "SR_Add";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SubmittedByID", SqlDbType.Int).Value = SubmittedByID;
                cmd.Parameters.Add("@SRTypeID", SqlDbType.Int).Value = SRTypeID;
                cmd.Parameters.Add("@PriorityID", SqlDbType.Int).Value = PriorityID;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@NewID", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                SqlParameter paramExists = cmd.Parameters["@Exists"];
                SqlParameter paramNewID = cmd.Parameters["@NewID"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                if (paramExists != null) bool.TryParse(paramExists.Value.ToString(), out exists);
                if (paramNewID != null) int.TryParse(paramNewID.Value.ToString(), out newID);

                result["saved"] = saved.ToString();
                result["exists"] = exists.ToString();
                result["newID"] = newID.ToString();
            }
        }

        return result;
    }

    public static Dictionary<string, string> SRAttachment_Add(int SRID, string FileName, byte[] FileData)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "SRAttachment_Add";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = SRID;
                cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = FileName;
                cmd.Parameters.Add("@FileData", SqlDbType.VarBinary).Value = FileData;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@NewID", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                SqlParameter paramExists = cmd.Parameters["@Exists"];
                SqlParameter paramNewID = cmd.Parameters["@NewID"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                if (paramExists != null) bool.TryParse(paramExists.Value.ToString(), out exists);
                if (paramNewID != null) int.TryParse(paramNewID.Value.ToString(), out newID);

                result["saved"] = saved.ToString();
                result["exists"] = exists.ToString();
                result["newID"] = newID.ToString();
            }
        }

        return result;
    }

    public static DataTable SRList_Get(int SRID = 0, string SubmittedBy = "", string StatusIDs = "", string ReasoningIDs = "", string Systems = "")
    {
        string procName = "SRList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = SRID;
                    cmd.Parameters.Add("@SubmittedBy", SqlDbType.NVarChar).Value = SubmittedBy;
                    cmd.Parameters.Add("@StatusIDs", SqlDbType.NVarChar).Value = StatusIDs;
                    cmd.Parameters.Add("@SRTypeIDs", SqlDbType.NVarChar).Value = ReasoningIDs;
                    cmd.Parameters.Add("@Systems", SqlDbType.NVarChar).Value = Systems;

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

    public static bool SR_Delete(int SRID)
    {
        bool deleted = false;
        string procName = "SR_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = SRID;
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

    public static bool SR_Update(XmlDocument Changes)
    {
        bool saved = false;
        string procName = "SR_Update";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Changes", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Changes.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
            }
        }

        return saved;
    }

    public static DataTable SRAttachmentList_Get(int SRID = 0)
    {
        string procName = "SRAttachmentList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = SRID;

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

    public static DataTable SRAttachment_Get(int SRAttachmentID)
    {
        string procName = "SRAttachment_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SRAttachmentID", SqlDbType.Int).Value = SRAttachmentID;

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

    public static DataTable SRResourceList_Get(int SRID = 0, int StatusID = 0, int ReasoningID = 0, string System = "")
    {
        string procName = "SRResourceList_Get";

        using (DataTable dt = new DataTable("SR_Resource"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = SRID;
                    cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = StatusID;
                    cmd.Parameters.Add("@SRTypeID", SqlDbType.Int).Value = ReasoningID;
                    cmd.Parameters.Add("@System", SqlDbType.NVarChar).Value = System;

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

    public static DataTable SRSystemList_Get(int SRID = 0, int StatusID = 0, int ReasoningID = 0)
    {
        string procName = "SRSystemList_Get";

        using (DataTable dt = new DataTable("SR_System"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = SRID;
                    cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = StatusID;
                    cmd.Parameters.Add("@SRTypeID", SqlDbType.Int).Value = ReasoningID;

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

    public static Dictionary<string, string> SR_Save(bool NewSR, int SRID, int StatusID, int SRTypeID, int PriorityID, int INVPriorityID, int SRRankID, string Description)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "SR_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NewSR", SqlDbType.Bit).Value = NewSR ? 1 : 0;
                cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = SRID;
                cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = StatusID;
                cmd.Parameters.Add("@SRTypeID", SqlDbType.Int).Value = SRTypeID;
                cmd.Parameters.Add("@PriorityID", SqlDbType.Int).Value = PriorityID;
                cmd.Parameters.Add("@INVPriorityID", SqlDbType.Int).Value = INVPriorityID;
                cmd.Parameters.Add("@SRRankID", SqlDbType.Int).Value = SRRankID;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@NewID", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                SqlParameter paramExists = cmd.Parameters["@Exists"];
                SqlParameter paramNewID = cmd.Parameters["@NewID"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                if (paramExists != null) bool.TryParse(paramExists.Value.ToString(), out exists);
                if (paramNewID != null) int.TryParse(paramNewID.Value.ToString(), out newID);

                result["saved"] = saved.ToString();
                result["exists"] = exists.ToString();
                result["newID"] = newID.ToString();
            }
        }

        return result;
    }

    public static bool SR_Email(int SRID)
    {
        if (WTSConfiguration.Environment == "PRODUCTION")
        {
            string from = WTSConfiguration.EmailFrom;
            string fromName = WTSConfiguration.EmailFromName;
            Dictionary<string, string> toAddresses = new Dictionary<string, string> { { "wts.support@infintech.com", "WTS.Support" } };
            string subject = "SR # " + SRID.ToString() + " was submitted.";
            string body = string.Empty;
            StringBuilder html = new StringBuilder(@"<style type=""text/css"">div,table {font-family: Arial; font-size: 12px;}</style>");
            DataTable dt = SR.SRList_Get(SRID: SRID);
            DateTime nSubmittedDate = new DateTime();
            string submittedDateDisplay = string.Empty;
            DataTable dtAttachments = SR.SRAttachmentList_Get(SRID: SRID);
            Dictionary<string, byte[]> attachments = new Dictionary<string, byte[]>();

            if (DateTime.TryParse(dt.Rows[0]["Submitted Date"].ToString(), out nSubmittedDate)) submittedDateDisplay = String.Format("{0:M/d/yyyy h:mm tt}", nSubmittedDate);

            html.Append("<div>");
            html.Append("<b>SR #: </b>" + SRID + "<br />");
            html.Append("<b>Submitted: </b>" + dt.Rows[0]["Submitted By"].ToString() + " - " + submittedDateDisplay + "<br />");
            html.Append("<b>Reasoning: </b>" + dt.Rows[0]["Reasoning"].ToString() + "<br />");
            html.Append("<b>User's Priority: </b>" + dt.Rows[0]["User's Priority"].ToString() + "<br />");
            html.Append("<b>Description: </b>" + dt.Rows[0]["Description"].ToString() + "<br />");
            html.Append("</div>");

            body = html.ToString();

            foreach (DataRow row in dtAttachments.Rows)
            {
                attachments.Add((string)row["File"], (byte[])row["FileData"]);
            }

            IEvent evt = EventQueue.Instance.QueueEmailEvent(toAddresses, null, null, subject, body, from, fromName, true, System.Net.Mail.MailPriority.Normal, attachments, false);

            if (evt == null) return false;
        }

        return true;
    }

    public static DataTable AORSRWebsystemList_Get(bool includeWTS)
    {
        DataTable dt = WTSData.GetDataTableFromStoredProcedure("AORSRWebsystemList_Get");

        if (includeWTS)
        {
            DataRow row = dt.NewRow();
            row["Websystem"] = "WTS";
        }

        return dt;
    }

    public static string GetAORWebSystemsForWTSSystem(int WTS_SYSTEMID)
    {
        string matchingSystems = "";

        Dictionary<int, string> map = new Dictionary<int, string>();

        DataTable dt =  MasterData.WTS_System_Get(WTS_SYSTEMID);
        string wtssystem = (string)dt.Rows[0]["WTS_System"];
        wtssystem = wtssystem.ToLower();
        wtssystem = wtssystem.Replace("websystem", "");
        wtssystem = wtssystem.Replace("r&d", "");
        wtssystem = wtssystem.Replace("cafdexcam", "cafdex");        

        dt = AORSRWebsystemList_Get(true);
        List<string> srsystems = new List<string>();

        foreach (DataRow row in dt.Rows)
        {
            srsystems.Add(((string)row["Websystem"]).ToLower());
        }

        string[] wtssystemarr = wtssystem.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string sys in wtssystemarr)
        {
            if (!string.IsNullOrWhiteSpace(sys))
            {
                string systrim = sys.Trim();                

                foreach (string aorsys in srsystems)
                {   
                    if (aorsys.Contains(systrim))
                    {
                        if (!("," + matchingSystems + ",").Contains("," + aorsys + ","))
                        {
                            if (matchingSystems.Length > 0)
                            {
                                matchingSystems += ",";
                            }

                            matchingSystems += aorsys;
                        }
                    }
                }
            }
        }
        
        return matchingSystems;
    }
}
 