﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;


/// <summary>
/// Access News Articles
/// </summary>
public sealed class WTSNews
{
    
    /// <summary>
    /// Load all News Articles
    /// Articles that do not exist for specified user are considered Unread
    /// </summary>
    /// <param name="userWebsysId"></param>
    /// <returns>table of news articles</returns>
    public static DataTable GetNews(int newsId = 0, int sysNotification = 0)
    {
        string procName = "GetNews";
        using (DataTable dt = new DataTable("News"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@NewsID", SqlDbType.Int).Value = newsId;
                    //cmd.Parameters.Add("@NewsTypeID", SqlDbType.Int).Value = newsTypeId == 0 ? (object)DBNull.Value : newsTypeId;
                    cmd.Parameters.Add("@SysNotification", SqlDbType.Int).Value = sysNotification;
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

    #region Attachments

    public static DataTable News_GetAttachmentList(int newsID, int showArchived = 0)
    {
        string procName = "News_GetAttachmentList";

        using (DataTable dt = new DataTable("Attachment"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@NEWSID", SqlDbType.Int).Value = newsID;
                    cmd.Parameters.Add("@ShowArchived", SqlDbType.Int).Value = showArchived == 0 ? (object)DBNull.Value : showArchived;

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

    public static bool NewsAttachment_Add(int newsID
        , int attachmentTypeID
        , string fileName
        , string title
        , string description
        , byte[] fileData
        , int extensionID
        , out int newAttachmentID
        , out string errorMsg)
    {     
        bool saved = false, exists = false;
        int newID = 0;
        newAttachmentID = 0;

        saved = WTSData.Attachment_Add(attachmentTypeID: attachmentTypeID, fileName: fileName, title: title, description: description, fileData: fileData, extensionID: extensionID, newID: out newAttachmentID, errorMsg: out errorMsg);
        if (saved && newAttachmentID > 0)
        {
            string procName = "NewsAttachment_Add";

            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@NewsID", SqlDbType.Int).Value = newsID;
                    cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = newAttachmentID;
                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                    string mystr = string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                    .Where(p => p.SqlDbType.ToString() == "NVarChar")
                    .Where(p => p.Value != null)
                    .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + "(MAX) = '" + p.Value.ToString() + "'"));

                    mystr += "/nnn/ " + string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                        .Where(p => p.SqlDbType.ToString() == "Int")
                        .Where(p => p.Value != null)
                        .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + " = " + p.Value.ToString() + ""));

                    mystr += "/nnn/ " + string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                        .Where(p => p.SqlDbType.ToString() == "bit")
                        .Where(p => p.Value != null)
                        .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + " = " + p.Value.ToString() + ""));

                    mystr += "/nnn/ " + string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                        .Where(p => p.DbType.ToString() != "Xml")
                        .Where(p => p.Value == null)
                        .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + " = null"));

                    cmd.ExecuteNonQuery();

                    SqlParameter paramNewID = cmd.Parameters["@newID"];
                    if (paramNewID != null)
                    {
                        saved = true;
                    }
                }
            }
        }

        return saved;
    }

    #endregion Attachments

    public static Dictionary<string, string> SaveNews(WTSNewsArticle newsItem, int newsId, int newsTypeId)
    {
        bool saved = false, exists = false;
        DateTime dtDate;
        int newID = 0, attachmentID = 0;
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

        string procName = "News_AddEdit";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NewsId", SqlDbType.Int).Value = newsId;
                cmd.Parameters.Add("@ArticleTitle", SqlDbType.NVarChar).Value = newsItem.ArticleTitle;
                cmd.Parameters.Add("@NotificationType", SqlDbType.Int).Value = newsItem.NotificationType;
                cmd.Parameters.Add("@NewsTypeId", SqlDbType.Int).Value = newsTypeId;
                cmd.Parameters.Add("@StartDate", SqlDbType.NVarChar).Value = newsItem.StartDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.NVarChar).Value = newsItem.EndDate;
                cmd.Parameters.Add("@Active", SqlDbType.Bit).Value = newsItem.Active ? 1 : 0;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = newsItem.Description;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                //cmd.Parameters.Add("@AttachmentID", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@NewID", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@EXISTS", SqlDbType.Int).Direction = ParameterDirection.Output;

                string mystr = string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                    .Where(p => p.SqlDbType.ToString() == "NVarChar")
                    .Where(p => p.Value != null)
                    .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + "(MAX) = '" + p.Value.ToString() + "'"));

                mystr += "/nnn/ " + string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                    .Where(p => p.SqlDbType.ToString() == "Int")
                    .Where(p => p.Value != null)
                    .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + " = " + p.Value.ToString() + ""));

                mystr += "/nnn/ " + string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                    .Where(p => p.SqlDbType.ToString() == "Bit")
                    .Where(p => p.Value != null)
                    .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + " = " + p.Value.ToString() + ""));

                mystr += "/nnn/ " + string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                    .Where(p => p.DbType.ToString() != "Xml")
                    .Where(p => p.Value == null)
                    .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + " = null"));

                cmd.ExecuteNonQuery();

                //SqlParameter paramAttachmentID = cmd.Parameters["@AttachmentID"];
                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                SqlParameter paramExists = cmd.Parameters["@EXISTS"];
                SqlParameter paramNewID = cmd.Parameters["@NewID"];

               // if (paramAttachmentID != null) int.TryParse(paramAttachmentID.Value.ToString(), out attachmentID);
                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                if (paramExists != null) bool.TryParse(paramExists.Value.ToString(), out exists);
                if (paramNewID != null) int.TryParse(paramNewID.Value.ToString(), out newID);

                //result["attachmentID"] = attachmentID.ToString();
                result["saved"] = saved.ToString();
                result["exists"] = exists.ToString();
                result["newID"] = newID.ToString(); 

                if (paramSaved != null)
                {
                    bool.TryParse(paramSaved.Value.ToString(), out saved);
                }
            }


        }

        return result;
    }

    public static bool DeleteNews(int newsId)
    {
        bool deleted = false;

        string procName = "News_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NewsId", SqlDbType.Int).Value = newsId;
                //cmd.Parameters.Add("@NewsTypeId", SqlDbType.Int).Value = newsTypeId;
                //cmd.Parameters.Add("@NewsAttachmentId", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@deleted"];

                //If this is a news Overview entry attempt to archive its attachment also
                //if (newsTypeId == (int)WTS.Enums.NewsTypeEnum.NewsOverview)
                //{
                //    //int NewsAttachmentId = 0;
                //    //int.TryParse(cmd.Parameters["@NewsAttachmentId"].ToString(), out NewsAttachmentId);
                //    //if (NewsAttachmentId > 0)
                //    //{
                //    //    paramSaved.Value = DeleteNewsAttachment(NewsAttachmentId);
                //    //}      
                //}

                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }

    public static bool DeleteNewsAttachment(int newsAttachmentId)
    {
        bool deleted = false;

        string procName = "NewsAttachment_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NewsAttachmentId", SqlDbType.Int).Value = newsAttachmentId;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                String mystr = string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                    .Where(p => p.SqlDbType.ToString() == "NVarChar")
                    .Where(p => p.Value != null)
                    .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + "(MAX) = '" + p.Value.ToString() + "'"));

                mystr += "/nnn/ " + string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                    .Where(p => p.SqlDbType.ToString() == "Int")
                    .Where(p => p.Value != null)
                    .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + " = " + p.Value.ToString() + ""));

                mystr += "/nnn/ " + string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                    .Where(p => p.SqlDbType.ToString() == "bit")
                    .Where(p => p.Value != null)
                    .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + " = " + p.Value.ToString() + ""));

                mystr += "/nnn/ " + string.Join(";/nnn/ ", cmd.Parameters.Cast<System.Data.SqlClient.SqlParameter>()
                    .Where(p => p.DbType.ToString() != "Xml")
                    .Where(p => p.Value == null)
                    .Select(p => "declare " + p.ParameterName + " " + p.SqlDbType + " = null"));

                cmd.ExecuteNonQuery();



                SqlParameter paramSaved = cmd.Parameters["@deleted"];
                if (paramSaved != null)
                {
                    bool.TryParse(paramSaved.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }

    public static bool MarkArticleRead(long p_TBL_NOTIFICATIONID)
    {
        return false;
    }
}

public class Article
{
    public long ID { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime ActiveDate { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }

    public Article(long articleID, string title = "", string message = "")
    {
        this.ID = articleID;
        this.Title = title;
        this.Message = message;
    }

    public long Load(long id)
    {
        this.ID = id;

        throw new NotImplementedException();
    }
    public long Load()
    {
        throw new NotImplementedException();
    }
}

public sealed class WTSNewsArticle
{
    #region "Properties"
    private string _articletitle = string.Empty;
    public string ArticleTitle
    {
        get { return _articletitle; }
        set { _articletitle = value; }
    }
    public int NotificationType { get; set; }
    private string _startdate = string.Empty;
    public string StartDate
    {
        get { return _startdate; }
        set { _startdate = value; }
    }
    private string _enddate = string.Empty;
    public string EndDate
    {
        get { return _enddate; }
        set { _enddate = value; }
    }
    public bool Active { get; set; }

    private string _description = string.Empty;
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    #endregion

    public WTSNewsArticle(Dictionary<string, object> attributes)
    {
        ApplyAttributes(attributes);
    }
    public WTSNewsArticle ApplyAttributes(Dictionary<string, object> attributes)
    {
        bool flagged = false;

        this.ArticleTitle = attributes["ArticleTitle"].ToString();
        this.NotificationType = int.Parse(attributes["NotificationType"].ToString());
        this.StartDate = attributes["StartDate"].ToString();
        this.EndDate = attributes["EndDate"].ToString();
        this.Active = bool.Parse(attributes["Active"].ToString());
        this.Description = attributes["Description"].ToString();
        return this;
    }
}