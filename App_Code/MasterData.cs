﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web;
using System.Data.SqlTypes;
using System.Xml;

/// <summary>
/// Summary description for MasterData
/// </summary>
public sealed class MasterData
{

    #region System Suite
    public static DataTable SystemSuiteList_Get(int productVersion = 0, int includeArchive = 1)
    {
        DataSet ds = new DataSet();
        string procName = "WTS_SYSTEM_SUITELIST_Get";

        using (DataTable dt = new DataTable("SystemSuite"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProductVersion", SqlDbType.Int).Value = productVersion;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.NVarChar).Value = includeArchive;
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
    public static bool systemSuite_Add(
        string systemSuite
        , string description
        , string abbreviation
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

        string procName = "WTS_SYSTEM_SUITE_ADD";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Suite", SqlDbType.NVarChar).Value = systemSuite;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Abbreviation", SqlDbType.NVarChar).Value = abbreviation;
                cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool SystemSuite_Update(int WTS_SYSTEM_SUITEID
        , string systemSuite
        , string description
        , string abbreviation
        , int sortOrder
        , bool archive
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_SYSTEM_SUITE_UPDATE";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEM_SUITEID", SqlDbType.Int).Value = WTS_SYSTEM_SUITEID;
                cmd.Parameters.Add("@WTS_SYSTEM_SUITE", SqlDbType.NVarChar).Value = systemSuite;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Abbreviation", SqlDbType.NVarChar).Value = abbreviation;
                cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
                    cmd.ExecuteNonQuery();

                    SqlParameter paramSaved = cmd.Parameters["@saved"];
                    if (paramSaved != null)
                    {
                        bool.TryParse(paramSaved.Value.ToString(), out saved);
                    }
                }
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool SystemSuite_Delete(int WTS_SYSTEM_SUITEID
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

        string procName = "WTS_SYSTEM_SUITE_DELETE";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEM_SUITEID", SqlDbType.Int).Value = WTS_SYSTEM_SUITEID;

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
                        errorMsg = "System Suite record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "System Suite record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    /// <summary>
    /// Update System Suite record
    ///  - will update ReviewSystem fields
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool SystemSuite_ReviewSystems(int systemSuiteID, out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_System_Suite_ReviewSystems";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_System_SuiteID", SqlDbType.Int).Value = systemSuiteID;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@saved"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out saved);
                }
            }
        }

        return saved;
    }

    /// <summary>
    /// Update System Suite record
    ///  - will update ReviewResource fields
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool SystemSuite_ReviewResources(int systemSuiteID, out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_System_Suite_ReviewResources";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_System_SuiteID", SqlDbType.Int).Value = systemSuiteID;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@saved"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out saved);
                }
            }
        }

        return saved;
    }
    #endregion

    #region System Suite - Resource
    /// <summary>
    /// Add new System_Suite_Resource record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_Suite_Resource_Add(
        int WTS_SYSTEM_SUITEID
        , int ProductVersionID
        , int ActionTeam
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_System_Suite_Resource_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEM_SUITEID", SqlDbType.Int).Value = WTS_SYSTEM_SUITEID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID == 0 ? (object)DBNull.Value : ProductVersionID;
				cmd.Parameters.Add("@ActionTeam", SqlDbType.Bit).Value = ActionTeam;
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
    /// Update specified System_Resource record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_Suite_Resource_Update(
        int WTS_SYSTEM_SUITEID
        , int ProductVersionID
        , int WTS_SYSTEM_SUITE_RESOURCEID
        , int WTS_RESOURCEID
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "WTS_System_Suite_Resource_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEM_SUITEID", SqlDbType.Int).Value = WTS_SYSTEM_SUITEID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
                cmd.Parameters.Add("@WTS_SYSTEM_SUITE_RESOURCEID", SqlDbType.Int).Value = WTS_SYSTEM_SUITE_RESOURCEID;
                cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = WTS_RESOURCEID;
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

    /// <summary>
    /// Delete System_Suite_Resource record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_Suite_Resource_Delete(int WTS_SYSTEM_SUITE_RESOURCEID
        , out bool exists
        , out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "WTS_System_Suite_Resource_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEM_SUITE_RESOURCEID", SqlDbType.Int).Value = WTS_SYSTEM_SUITE_RESOURCEID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        errorMsg = "System_Suite_Resource record could not be found.";

                        return false;
                    }
                }
                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }
    #endregion

    #region WorkItemType - Status
    public static DataTable WORKITEMTYPE_StatusList_Get(int WORKITEMTYPEID = 0, int STATUSID = 0)
    {
        string procName = "WORKITEMTYPE_StatusList_Get";

        using (DataTable dt = new DataTable("WORKITEMTYPE_Status"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID == 0 ? (object)DBNull.Value : WORKITEMTYPEID;
                    cmd.Parameters.Add("@STATUSID", SqlDbType.Int).Value = STATUSID == 0 ? (object)DBNull.Value : STATUSID;

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

    public static bool WORKITEMTYPE_Status_Add(object WORKITEMTYPEID, object STATUSID, out bool exists, out int newID, out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WORKITEMTYPE_Status_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID;
                cmd.Parameters.Add("@STATUSID", SqlDbType.NVarChar).Value = STATUSID;
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

    public static bool WORKITEMTYPE_Status_Update(int WORKITEMTYPE_StatusID
        , int WORKITEMTYPEID
        , int STATUSID
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

    string procName = "WORKITEMTYPE_Status_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WORKITEMTYPE_StatusID", SqlDbType.Int).Value = WORKITEMTYPE_StatusID;
                cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID;
                cmd.Parameters.Add("@STATUSID", SqlDbType.Int).Value = STATUSID ;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive? 1 : 0;
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

    public static bool WORKITEMTYPE_Status_Delete(int WORKITEMTYPE_StatusID, out bool exists, out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "WORKITEMTYPE_Status_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WORKITEMTYPE_StatusID", SqlDbType.Int).Value = WORKITEMTYPE_StatusID;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {

                            errorMsg = "Work Activity - Status record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }

    #endregion WorkItemType - Status

    #region WorkItemType - Resource Type
    public static DataTable ResourceTypeList_Get(bool includeArchive = false)
    {
        string procName = "ResourceTypeList_Get";

        using (DataTable dt = new DataTable("ResourceType"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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

    public static DataTable WorkActivity_WTS_RESOURCE_TYPEList_Get(int WORKITEMTYPEID = 0)
    {
        string procName = "WorkActivity_WTS_RESOURCE_TYPEList_Get";

        using (DataTable dt = new DataTable("WorkActivity_WTS_RESOURCE_TYPE"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID == 0 ? (object)DBNull.Value : WORKITEMTYPEID;

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

    public static bool WORKITEMTYPE_ResourceType_Add(object WORKITEMTYPEID, object ResourceTypeID, out bool exists, out int newID, out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WORKITEMTYPE_ResourceType_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID;
                cmd.Parameters.Add("@WTS_RESOURCE_TYPEID", SqlDbType.NVarChar).Value = ResourceTypeID;
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

    public static bool WORKITEMTYPE_ResourceType_Update(int WorkActivity_WTS_RESOURCE_TYPEID
        , int WORKITEMTYPEID
        , int ResourceTypeID
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "WORKITEMTYPE_ResourceType_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkActivity_WTS_RESOURCE_TYPEID", SqlDbType.Int).Value = WorkActivity_WTS_RESOURCE_TYPEID;
                cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID;
                cmd.Parameters.Add("@WTS_RESOURCE_TYPEID", SqlDbType.Int).Value = ResourceTypeID;
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

    public static bool WORKITEMTYPE_ResourceType_Delete(int WorkActivity_WTS_RESOURCE_TYPEID, out bool exists, out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "WORKITEMTYPE_ResourceType_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkActivity_WTS_RESOURCE_TYPEID", SqlDbType.Int).Value = WorkActivity_WTS_RESOURCE_TYPEID;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {

                        errorMsg = "Work Activity - Resource Type record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }

    public static bool WORKITEMTYPE_ResourceType_Sync(object WORKITEMTYPEID, out bool exists, out int newID, out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WORKITEMTYPE_ResourceType_Sync";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID;
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

    #endregion WorkItemType - Resource Type

    #region WorkItemType - Resource
    public static DataTable WorkActivity_WTS_RESOURCEList_Get(int WORKITEMTYPEID = 0)
    {
        string procName = "WorkActivity_WTS_RESOURCEList_Get";

        using (DataTable dt = new DataTable("WorkActivity_WTS_RESOURCE"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID == 0 ? (object)DBNull.Value : WORKITEMTYPEID;

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

    public static bool WORKITEMTYPE_Resource_Add(object WORKITEMTYPEID, object ResourceID, out bool exists, out int newID, out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WORKITEMTYPE_Resource_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID;
                cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.NVarChar).Value = ResourceID;
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

    public static bool WORKITEMTYPE_Resource_Update(int WorkActivity_WTS_RESOURCEID
        , int WORKITEMTYPEID
        , int ResourceID
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "WORKITEMTYPE_Resource_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkActivity_WTS_RESOURCEID", SqlDbType.Int).Value = WorkActivity_WTS_RESOURCEID;
                cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID;
                cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = ResourceID;
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

    public static bool WORKITEMTYPE_Resource_Delete(int WorkActivity_WTS_RESOURCEID, out bool exists, out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "WORKITEMTYPE_Resource_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkActivity_WTS_RESOURCEID", SqlDbType.Int).Value = WorkActivity_WTS_RESOURCEID;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {

                        errorMsg = "Work Activity - Resource record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }

    public static bool WORKITEMTYPE_Resource_Sync(object WORKITEMTYPEID, out bool exists, out int newID, out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WORKITEMTYPE_Resource_Sync";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WORKITEMTYPEID;
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

    #endregion WorkItemType - Resource

    #region Requirement Type

    /// <summary>
    /// Get a list of the avaiable RQMT Types
    /// </summary>
    /// <param name="includeArchive"></param>
    /// <returns></returns>
    public static DataTable RQMT_TypeList_Get(bool includeArchive)
    {
        DataSet ds = new DataSet();
        string procName = "RQMTTypeList_Get";

        using (DataTable dt = new DataTable("RQMT_Type"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
    /// Add the specified RQMT  Type
    /// </summary>
    public static bool RQMTType_Add(string RQMTType
       , string description
       , int sort
       , bool archive
       , bool internalType
       , out bool exists
       , out int newID
       , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "RQMTType_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTType", SqlDbType.NVarChar).Value = RQMTType;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Internal", SqlDbType.Bit).Value = internalType;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    /// <summary>
    /// Update specified RQMT Type record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool RQMTType_Update(int RQMTTypeID
        , string RQMTType
        , string description
        , int sort
        , bool archive
        , bool internalType
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "RQMTType_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTTypeID", SqlDbType.Int).Value = RQMTTypeID;
                cmd.Parameters.Add("@RQMTType", SqlDbType.NVarChar).Value = RQMTType;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Internal", SqlDbType.Bit).Value = internalType;

                cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    /// <summary>
    /// Delete RQMT Type record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool RQMTType_Delete(int RQMTTypeID
        , out bool exists
        , out bool hasDependencies
        , out bool archived
        , out string errorMsg)
    {
        exists = false;
        archived = false;
        hasDependencies = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "RQMTType_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTTypeID", SqlDbType.Int).Value = RQMTTypeID;
                cmd.Parameters.Add("@hasDependencies", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@archived", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        archived = false;
                        errorMsg = "RQMT Type record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "RQMT Type record has dependencies and could not be permanently deleted. It has been archived instead.";
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
    #endregion

    #region RQMT Description Type
    public static DataTable RQMT_Description_TypeList_Get(bool includeArchive)
    {
            DataSet ds = new DataSet();
            string procName = "RQMTDescriptionTypeList_Get";

            using (DataTable dt = new DataTable("RQMT_Description_Type"))
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
    /// Add the specified RQMT Description Type
    /// </summary>
    public static bool RQMTDescriptionType_Add(string RQMTDescriptionType
       , string description
       , int sort
       , bool archive
       , out bool exists
       , out int newID
       , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "RQMTDescriptionType_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTDescriptionType", SqlDbType.NVarChar).Value = RQMTDescriptionType;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    /// <summary>
    /// Update specified RQMT Description Type record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool RQMTDescriptionType_Update(int RQMTDescriptionTypeID
        , string RQMTDescriptionType
        , string description
        , int sort
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "RQMTDescriptionType_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTDescriptionTypeID", SqlDbType.Int).Value = RQMTDescriptionTypeID;
                cmd.Parameters.Add("@RQMTDescriptionType", SqlDbType.NVarChar).Value = RQMTDescriptionType;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    /// <summary>
    /// Delete RQMT Description Type record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool RQMTDescriptionType_Delete(int RQMTDescriptionTypeID
        , out bool exists
        , out bool hasDependencies
        , out bool archived
        , out string errorMsg)
    {
        exists = false;
        archived = false;
        hasDependencies = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "RQMTDescriptionType_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTDescriptionTypeID", SqlDbType.Int).Value = RQMTDescriptionTypeID;
                cmd.Parameters.Add("@hasDependencies", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@archived", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        archived = false;
                        errorMsg = "RQMT Description Type record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "RQMT Description Type record has dependencies and could not be permanently deleted. It has been archived instead.";
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
    #endregion

    #region Item Type
    public static DataTable ItemTypeList_Get()
    {
        DataSet ds = new DataSet();
        string procName = "ItemTypeList_Get";

        using (DataTable dt = new DataTable("ItemType"))
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

    public static bool ItemType_Add(
        string itemType
        , string description
        , int pddtdrPhaseID
        , int workloadAllocationID
        , int workActivityGroupID
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

        string procName = "ItemType_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ItemType", SqlDbType.NVarChar).Value = itemType;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@PDDTDR_PHASEID", SqlDbType.Int).Value = pddtdrPhaseID;
                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = workloadAllocationID;
                cmd.Parameters.Add("@WorkActivityGroupID", SqlDbType.Int).Value = workActivityGroupID;
                cmd.Parameters.Add("@SortOrder", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }
    public static bool ItemType_Update(int WORKITEMTYPEID
    , string itemType
    , string description
    , int pddtdrPhaseID
    , int workloadAllocationID
    , int workActivityGroupID
    , int sortOrder
    , bool archive
    , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "ItemType_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ItemTypeID", SqlDbType.Int).Value = WORKITEMTYPEID;
                cmd.Parameters.Add("@ItemType", SqlDbType.NVarChar).Value = itemType;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@PDDTDR_PHASEID", SqlDbType.Int).Value = pddtdrPhaseID;
                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = workloadAllocationID;
                cmd.Parameters.Add("@WorkActivityGroupID", SqlDbType.Int).Value = workActivityGroupID;
                cmd.Parameters.Add("@SortOrder", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
                    cmd.ExecuteNonQuery();

                    SqlParameter paramSaved = cmd.Parameters["@saved"];
                    if (paramSaved != null)
                    {
                        bool.TryParse(paramSaved.Value.ToString(), out saved);
                    }
                }
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }
    public static bool ItemType_Delete(int WORKITEMTYPEID
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

        string procName = "ItemType_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@TypeID", SqlDbType.Int).Value = WORKITEMTYPEID;

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
                        errorMsg = "Work Activity record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "Work Activity record has dependencies and could not be permanently deleted. It has been archived instead.";
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
    #endregion Item Type
    #region Allocation Category

    /// <summary>
    /// Load Allocation Category Items
    /// </summary>
    /// <returns>Datatable of Allocation Items</returns>
    public static DataTable AllocationCategoryList_Get(bool includeArchive = false)
	{
		DataSet ds = new DataSet();
		string procName = "AllocationCategoryList_Get";

		using (DataTable dt = new DataTable("AllocationCategory"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Add new Allocation Category record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool AllocationCategory_Add(
		string category
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

		string procName = "AllocationCategory_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@AllocationCategory", SqlDbType.NVarChar).Value = category;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				try
				{
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
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Update specified Allocation Category record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool AllocationCategory_Update(int allocationCategoryID
		, string category
		, string description
		, int sortOrder
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "AllocationCategory_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@AllocationCategoryID", SqlDbType.Int).Value = allocationCategoryID;
				cmd.Parameters.Add("@AllocationCategory", SqlDbType.NVarChar).Value = category;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
					cmd.ExecuteNonQuery();

					SqlParameter paramSaved = cmd.Parameters["@saved"];
					if (paramSaved != null)
					{
						bool.TryParse(paramSaved.Value.ToString(), out saved);
					}
				}
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Delete Allocation Category record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool AllocationCategory_Delete(int allocationCategoryID
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

		string procName = "AllocationCategory_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@AllocationCategoryID", SqlDbType.Int).Value = allocationCategoryID;

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
						errorMsg = "Allocation record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Allocation record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion Allocation Category


	#region Allocation

	/// <summary>
	/// Load Allocation Items
	/// </summary>
	/// <returns>Datatable of Allocation Items</returns>
	public static DataSet AllocationList_Get(bool includeArchive = false)
	{
		DataSet ds = new DataSet();
		string procName = "AllocationList_Get";

		using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			cn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, cn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.TableMappings.Add("Table", "Allocation");
				da.TableMappings.Add("Table1", "Category");
                da.TableMappings.Add("Table2", "Group");

                da.Fill(ds);
			}
		}

		return ds;
	}

    public static DataTable WTS_Resource_Get(bool includeArchive = false)
    {
        DataSet ds = new DataSet();
        string procName = "WTS_Resource_Get";

        using (DataTable dt = new DataTable("Resources"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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

    public static DataTable WTS_ResourceDevelopers_Get(int IsDeveloper = 2, int IsBusAnalyst = 2, int IsAMCGEO = 2, int IsCASUser = 2, int IsALODUser = 2)
    {
        DataSet ds = new DataSet();
        string procName = "WTS_ResourceDevelopers_Get";

        using (DataTable dt = new DataTable("Resources"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@IsDeveloper", SqlDbType.Int).Value = IsDeveloper;
                    cmd.Parameters.Add("@IsBusAnalyst", SqlDbType.Int).Value = IsBusAnalyst;
                    cmd.Parameters.Add("@IsAMCGEO", SqlDbType.Int).Value = IsAMCGEO;
                    cmd.Parameters.Add("@IsCASUser", SqlDbType.Int).Value = IsCASUser;
                    cmd.Parameters.Add("@IsALODUser", SqlDbType.Int).Value = IsALODUser;

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
    /// Update specified Allocation record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool AllocationGroup_Assignment_Update(
        int allocationID
        , string allocation
        , string description
        , int sortOrder
        , bool archive
        , int defaultSMEID
        , int defaultAssignedToID
        , int defaultBusinessResourceID
        , int defaultTechnicalResourceID
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "AllocationGroup_Assignment_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ALLOCATIONID", SqlDbType.Int).Value = allocationID;
                cmd.Parameters.Add("@ALLOCATION", SqlDbType.NVarChar).Value = allocation;
                cmd.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@SORT_ORDER", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@ARCHIVE", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UPDATEDBY", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@DefaultSMEID", SqlDbType.Int).Value = defaultSMEID == 0 ? (object)DBNull.Value : defaultSMEID;
                cmd.Parameters.Add("@DefaultAssignedToID", SqlDbType.Int).Value = defaultAssignedToID == 0 ? (object)DBNull.Value : defaultAssignedToID;
                cmd.Parameters.Add("@DefaultBusinessResourceID", SqlDbType.Int).Value = defaultBusinessResourceID == 0 ? (object)DBNull.Value : defaultBusinessResourceID;
                cmd.Parameters.Add("@DefaultTechnicalResourceID", SqlDbType.Int).Value = defaultTechnicalResourceID == 0 ? (object)DBNull.Value : defaultTechnicalResourceID;


                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
                    cmd.ExecuteNonQuery();

                    SqlParameter paramSaved = cmd.Parameters["@saved"];
                    if (paramSaved != null)
                    {
                        bool.TryParse(paramSaved.Value.ToString(), out saved);
                    }
                }
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    /// <summary>
    /// Add new Allocation record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool AllocationGroup_Assignment_Add(
        string allocationID
        , string description
        , int sortOrder
        , bool archive
        , int defaultSMEID
        , int defaultBusinessResourceID
        , int defaultTechnicalResourceID
        , int defaultAssignedToID
        , int AllocationGroupID
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_AllocationGroup_Assignment_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ALLOCATIONID", SqlDbType.NVarChar).Value = allocationID;
                cmd.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@SORT_ORDER", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@ARCHIVE", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UPDATEDBY", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@DefaultSMEID", SqlDbType.Int).Value = defaultSMEID == 0 ? (object)DBNull.Value : defaultSMEID;
                cmd.Parameters.Add("@DefaultBusinessResourceID", SqlDbType.Int).Value = defaultBusinessResourceID == 0 ? (object)DBNull.Value : defaultBusinessResourceID;
                cmd.Parameters.Add("@DefaultTechnicalResourceID", SqlDbType.Int).Value = defaultTechnicalResourceID == 0 ? (object)DBNull.Value : defaultTechnicalResourceID;
                cmd.Parameters.Add("@DefaultAssignedToID", SqlDbType.Int).Value = defaultAssignedToID == 0 ? (object)DBNull.Value : defaultAssignedToID;
                cmd.Parameters.Add("@ALLOCATIONGROUPID", SqlDbType.Int).Value = AllocationGroupID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool WTS_System_Add(
    string WTS_System
    , string description
    , int sortOrder
    , out bool exists
    , out int newID)
    {
        exists = false;
        newID = 0;
        bool saved = false;

        string procName = "WTS_System_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_System", SqlDbType.NVarChar).Value = WTS_System;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }


    /// <summary>
    /// Add new Allocation record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool Allocation_Add(
		int categoryID
        , int groupID
		, string allocation
		, string description
		, int defaultAssignedToID
		, int defaultSMEID
		, int defaultBusinessResourceID
		, int defaultTechnicalResourceID
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

		string procName = "Allocation_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@AllocationCategoryID", SqlDbType.Int).Value = categoryID == 0 ? (object)DBNull.Value : categoryID;
                cmd.Parameters.Add("@AllocationGroupID", SqlDbType.Int).Value = groupID == 0 ? (object)DBNull.Value : groupID;
                cmd.Parameters.Add("@Allocation", SqlDbType.NVarChar).Value = allocation;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@DefaultAssignedToID", SqlDbType.Int).Value = defaultAssignedToID == 0 ? (object)DBNull.Value : defaultAssignedToID;
				cmd.Parameters.Add("@DefaultSMEID", SqlDbType.Int).Value = defaultSMEID == 0 ? (object)DBNull.Value : defaultSMEID;
				cmd.Parameters.Add("@DefaultBusinessResourceID", SqlDbType.Int).Value = defaultBusinessResourceID == 0 ? (object)DBNull.Value : defaultBusinessResourceID;
				cmd.Parameters.Add("@DefaultTechnicalResourceID", SqlDbType.Int).Value = defaultTechnicalResourceID == 0 ? (object)DBNull.Value : defaultTechnicalResourceID;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				try
				{
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
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Update specified Allocation record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Allocation_Update(int allocationID
		, int categoryID
        , int groupID
        , string allocation
		, string description
		, int defaultAssignedToID
		, int defaultSMEID
		, int defaultBusinessResourceID
		, int defaultTechnicalResourceID
		, int sortOrder
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "Allocation_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@AllocationCategoryID", SqlDbType.Int).Value = categoryID == 0 ? (object)DBNull.Value : categoryID;
                cmd.Parameters.Add("@AllocationGroupID", SqlDbType.Int).Value = groupID == 0 ? (object)DBNull.Value : groupID;
                cmd.Parameters.Add("@AllocationID", SqlDbType.Int).Value = allocationID;
				cmd.Parameters.Add("@Allocation", SqlDbType.NVarChar).Value = allocation;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@DefaultAssignedToID", SqlDbType.Int).Value = defaultAssignedToID == 0 ? (object)DBNull.Value : defaultAssignedToID;
				cmd.Parameters.Add("@DefaultSMEID", SqlDbType.Int).Value = defaultSMEID == 0 ? (object)DBNull.Value : defaultSMEID;
				cmd.Parameters.Add("@DefaultBusinessResourceID", SqlDbType.Int).Value = defaultBusinessResourceID == 0 ? (object)DBNull.Value : defaultBusinessResourceID;
				cmd.Parameters.Add("@DefaultTechnicalResourceID", SqlDbType.Int).Value = defaultTechnicalResourceID == 0 ? (object)DBNull.Value : defaultTechnicalResourceID;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
					cmd.ExecuteNonQuery();

					SqlParameter paramSaved = cmd.Parameters["@saved"];
					if (paramSaved != null)
					{
						bool.TryParse(paramSaved.Value.ToString(), out saved);
					}
				}
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Delete Allocation record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Allocation_Delete(int allocationID
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

		string procName = "Allocation_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@AllocationID", SqlDbType.Int).Value = allocationID;

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
						errorMsg = "Allocation record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Allocation record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion Allocation

    /// <summary>
    /// Load Allocation Category Items
    /// </summary>
    /// <returns>Datatable of Allocation Items</returns>
    public static DataTable ALLOCATION_GETALL()
    {
        DataSet ds = new DataSet();
        string procName = "WTS_ALLOCATION_GETALL";

        using (DataTable dt = new DataTable("ALLOCATION"))
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
    /// Load Allocation Get All Unused Items, 'Null values in WTS_SYSTEMID
    /// </summary>
    /// <returns>Datatable of Allocation Items</returns>
    public static DataTable Allocation_Get_All_Unused()
    {
        DataSet ds = new DataSet();
        string procName = "Allocation_Get_All_Unused";

        using (DataTable dt = new DataTable("Allocation_Get_All_Unused"))
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
    /// Load Allocation Category Items
    /// </summary>
    /// <returns>Datatable of Allocation Items</returns>
    public static DataTable AllocationGroup_Assignment_Get(int parentID)
    {
        DataSet ds = new DataSet();
        string procName = "WTS_AllocationGroup_Assignment_Get";

        using (DataTable dt = new DataTable("AllocationGroup_Assignment"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AllocationGroupID ", SqlDbType.Int).Value = parentID;

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

    #region Allocation - System

    /// <summary>
	/// Load Allocation_System Items
	/// </summary>
	/// <returns>Datatable of Allocation_System Items</returns>
	public static DataTable Allocation_SystemList_Get(int allocationID = 0
		, int systemID = 0)
	{
		string procName = "Allocation_SystemList_Get";

		using (DataTable dt = new DataTable("Allocation_System"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@ALLOCATIONID", SqlDbType.Int).Value = allocationID == 0 ? (object)DBNull.Value : allocationID;
					cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = systemID == 0 ? (object)DBNull.Value : systemID;

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
	/// Update specified Allocation_System record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool AllocationGroup_DeleteChild(int AllocationID)
    {
        bool saved = false;

        string procName = "Allocation_Group_DeleteChild";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AllocationID", SqlDbType.Int).Value = AllocationID;
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
    /// Add new Allocation_System record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool Allocation_System_Add(
		int allocationID
		, int systemID
		, string description
		, int proposedPriority
		, int approvedPriority
		, out bool exists
		, out int newID
		, out string errorMsg)
	{
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "Allocation_System_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@ALLOCATIONID", SqlDbType.Int).Value = allocationID;
				cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.NVarChar).Value = systemID == 0 ? (object)DBNull.Value : systemID;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@ProposedPriority", SqlDbType.Int).Value = proposedPriority;
				cmd.Parameters.Add("@ApprovedPriority", SqlDbType.Int).Value = approvedPriority;
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
	/// Update specified Allocation_System record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Allocation_System_Update(int allocationSystemID
		, int allocationID
		, int systemID
		, string description
		, int proposedPriority
		, int approvedPriority
		, bool archive
		, out bool duplicate
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		duplicate = false;
		bool saved = false;

		string procName = "Allocation_System_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@Allocation_SystemID", SqlDbType.Int).Value = allocationSystemID;
				cmd.Parameters.Add("@ALLOCATIONID", SqlDbType.Int).Value = allocationID;
				cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = systemID == 0 ? (object)DBNull.Value : systemID;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@ProposedPriority", SqlDbType.Int).Value = proposedPriority;
				cmd.Parameters.Add("@ApprovedPriority", SqlDbType.Int).Value = approvedPriority;
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

	/// <summary>
	/// Delete Allocation_System record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Allocation_System_Delete(int allocationSystemID
		, out bool exists
		, out string errorMsg)
	{
		exists = false;
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "Allocation_System_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@Allocation_SystemID", SqlDbType.Int).Value = allocationSystemID;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramExists = cmd.Parameters["@exists"];
				if (paramExists != null)
				{
					bool.TryParse(paramExists.Value.ToString(), out exists);
					if (!exists)
					{
						errorMsg = "Allocation_System record could not be found.";
						return false;
					}
				}
				SqlParameter paramDeleted = cmd.Parameters["@deleted"];
				if (paramDeleted != null)
				{
					bool.TryParse(paramDeleted.Value.ToString(), out deleted);
				}
			}
		}

		return deleted;
	}

	#endregion Allocation - System


	#region ContractType

	/// <summary>
	/// Load Contract Type Items
	/// </summary>
	/// <returns>Datatable of Contract Type Items</returns>
	public static DataTable ContractTypeList_Get(bool includeArchive = false)
	{
		string procName = "ContractTypeList_Get";

		using (DataTable dt = new DataTable("ContractType"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Add new Contract Type record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ContractType_Add(
		string ContractType
		, string description
		, bool archive
		, out bool exists
		, out int newID
		, out string errorMsg)
	{
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "ContractType_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@ContractType", SqlDbType.NVarChar).Value = ContractType;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
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
	/// Update specified Contract Type record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ContractType_Update(int ContractTypeID
		, string ContractType
		, string description
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "ContractType_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@ContractTypeID", SqlDbType.Int).Value = ContractTypeID;
				cmd.Parameters.Add("@ContractType", SqlDbType.NVarChar).Value = ContractType;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
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
	/// Delete Contract Type record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ContractType_Delete(int ContractTypeID
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

		string procName = "ContractType_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@ContractTypeID", SqlDbType.Int).Value = ContractTypeID;

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
						errorMsg = "Contract Type record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Contract Type record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion ContractType


	#region Contract

	/// <summary>
	/// Load Contract Items
	/// </summary>
	/// <returns>Datatable of Contract Items</returns>
	public static DataTable ContractList_Get(int deliverableID = 0, bool includeArchive = false)
	{
		string procName = "ContractList_Get";

		using (DataTable dt = new DataTable("Contract"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@DeploymentID", SqlDbType.Int).Value = deliverableID;
					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Add new Contract record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Contract_Add(
		int contractTypeID
		, string contract
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

		string procName = "Contract_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@ContractTypeID", SqlDbType.Int).Value = contractTypeID;
				cmd.Parameters.Add("@Contract", SqlDbType.NVarChar).Value = contract;
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
	/// Update specified Contract record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Contract_Update(int contractID
		, int contractTypeID
		, string contract
		, string description
		, int sortOrder
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "Contract_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
				cmd.Parameters.Add("@ContractTypeID", SqlDbType.Int).Value = contractTypeID;
				cmd.Parameters.Add("@Contract", SqlDbType.NVarChar).Value = contract;
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
	/// Delete Contract record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Contract_Delete(int contractID
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

		string procName = "Contract_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;

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
						errorMsg = "Contract record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Contract record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    public static DataTable DeploymentContractList_Get(int DeliverableID = 0)
    {
        string procName = "DeploymentContractList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@DeliverableID", SqlDbType.Int).Value = DeliverableID;

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

    public static bool DeploymentContract_Add(int DeliverableID, XmlDocument Additions)
    {
        bool saved = false;
        string procName = "DeploymentContract_Add";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@DeliverableID", SqlDbType.Int).Value = DeliverableID;
                cmd.Parameters.Add("@Additions", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Additions.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
            }
        }

        return saved;
    }

    public static bool DeploymentContract_Delete(int deploymentContractID)
    {
        bool deleted = false;
        string procName = "DeploymentContract_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@DeploymentContractID", SqlDbType.Int).Value = deploymentContractID;
                cmd.Parameters.Add("@Exists", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@Deleted"];

                if (paramDeleted != null) bool.TryParse(paramDeleted.Value.ToString(), out deleted);
            }
        }

        return deleted;
    }

    #endregion Contract


    #region Priority

    /// <summary>
    /// Load Priority Items
    /// </summary>
    /// <returns>Datatable of Priority Items</returns>
    public static DataTable PriorityList_Get(bool includeArchive = false, bool includeDefaultRow = true, int PRIORITYTYPEID = 0)
	{
		string procName = "PriorityList_Get";

		using (DataTable dt = new DataTable("Priority"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;
                    cmd.Parameters.Add("@IncludeDefaultRow", SqlDbType.Bit).Value = includeDefaultRow;
                    cmd.Parameters.Add("@PRIORITYTYPEID", SqlDbType.Int).Value = PRIORITYTYPEID;

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
	/// Add new Priority record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Priority_Add(
		int priorityTypeID
		, string priority
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

		string procName = "Priority_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@PriorityTypeID", SqlDbType.Int).Value = priorityTypeID;
				cmd.Parameters.Add("@Priority", SqlDbType.NVarChar).Value = priority;
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
	/// Update specified Priority record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Priority_Update(int priorityID
		, int priorityTypeID
		, string priority
		, string description
		, int sortOrder
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "Priority_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@PriorityID", SqlDbType.Int).Value = priorityID;
				cmd.Parameters.Add("@PriorityTypeID", SqlDbType.Int).Value = priorityTypeID;
				cmd.Parameters.Add("@Priority", SqlDbType.NVarChar).Value = priority;
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
	/// Delete Priority record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Priority_Delete(int priorityID
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

		string procName = "Priority_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@PriorityID", SqlDbType.Int).Value = priorityID;

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
						errorMsg = "Priority record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Priority record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion Priority


	#region Status

	/// <summary>
	/// Load Status Type Items
	/// </summary>
	/// <returns>Datatable of Status Type Items</returns>
	public static DataTable StatusTypeList_Get(bool includeArchive = true)
	{
		string procName = "StatusTypeList_Get";

		using (DataTable dt = new DataTable("Status"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Load Status Items
	/// </summary>
	/// <returns>Datatable of Status Items</returns>
	public static DataTable StatusList_Get(bool includeArchive = false)
	{
		string procName = "StatusList_Get";

		using (DataTable dt = new DataTable("Status"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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

    public static DataTable MetricsGridHeaderCounts_Get(int includeArchive = 0
        , string selectedStatus = ""
        , string selectedAssigned = ""
        , bool myData = false)
    {
        string procName = "MetricsGridHeaderCounts_Get";

        using (DataTable dt = new DataTable("Status"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;   // ? 1 : 0;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.NVarChar).Value = myData ? UserManagement.GetUserId_FromUsername().ToString() : "";
                    cmd.Parameters.Add("@SelectedStatus", SqlDbType.NVarChar).Value = selectedStatus;
                    cmd.Parameters.Add("@SelectedAssigned", SqlDbType.NVarChar).Value = selectedAssigned;

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

    public static DataTable MetricsGridHeaderSubCounts_Get(int includeArchive = 0
    , string selectedStatus = ""
    , string selectedAssigned = ""
    , bool myData = false)
    {
        string procName = "MetricsGridHeaderSubCounts_Get";

        using (DataTable dt = new DataTable("Status"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;   // ? 1 : 0;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.NVarChar).Value = myData ? UserManagement.GetUserId_FromUsername().ToString() : "";
                    cmd.Parameters.Add("@SelectedStatus", SqlDbType.NVarChar).Value = selectedStatus;
                    cmd.Parameters.Add("@SelectedAssigned", SqlDbType.NVarChar).Value = selectedAssigned;

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
    /// Add new Status record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool Status_Add(
		int statusTypeID
		, string status
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

		string procName = "Status_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@StatusTypeID", SqlDbType.Int).Value = statusTypeID;
				cmd.Parameters.Add("@Status", SqlDbType.NVarChar).Value = status;
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
	/// Update specified Status record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Status_Update(int statusID
		, int statusTypeID
		, string Status
		, string description
		, int sortOrder
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "Status_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID;
				cmd.Parameters.Add("@StatusTypeID", SqlDbType.Int).Value = statusTypeID;
				cmd.Parameters.Add("@Status", SqlDbType.NVarChar).Value = Status;
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
	/// Delete Status record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Status_Delete(int statusID
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

		string procName = "Status_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID;

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
						errorMsg = "Status record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Status record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion Status


	#region Phase

	/// <summary>
	/// Load Phase Items
	/// </summary>
	/// <returns>Datatable of Phase Items</returns>
	public static DataTable PhaseList_Get(bool includeArchive = false)
	{
		string procName = "PDDTDR_PhaseList_Get";

		using (DataTable dt = new DataTable("Phase"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Add new Phase record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Phase_Add(
		string phase
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

		string procName = "PDDTDR_Phase_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@PDDTDR_Phase", SqlDbType.NVarChar).Value = phase;
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
	/// Update specified Phase record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Phase_Update(int phaseID
		, string phase
		, string description
		, int sortOrder
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "PDDTDR_Phase_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@PDDTDR_PhaseID", SqlDbType.Int).Value = phaseID;
				cmd.Parameters.Add("@PDDTDR_Phase", SqlDbType.NVarChar).Value = phase;
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
	/// Delete Phase record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Phase_Delete(int phaseID
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

		string procName = "PDDTDR_Phase_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@PDDTDR_PhaseID", SqlDbType.Int).Value = phaseID;

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
						errorMsg = "Phase record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Phase record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion Phase


	#region WorkloadGroup

	/// <summary>
	/// Load WorkloadGroup Items
	/// </summary>
	/// <returns>Datatable of WorkloadGroup Items</returns>
	public static DataTable WorkloadGroupList_Get(bool includeArchive = false)
	{
		string procName = "WorkloadGroupList_Get";

		using (DataTable dt = new DataTable("WorkloadGroup"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Add new WorkloadGroup record
	/// - Can only set ProposedPriorityRank here
	///   - ActualPriorityRank must be signed off by "Admin"
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkloadGroup_Add(
		string workloadGroup
		, string description
		, int proposedPriorityRank
		, out bool exists
		, out int newID
		, out string errorMsg)
	{
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkloadGroup_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkloadGroup", SqlDbType.NVarChar).Value = workloadGroup;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@ProposedPriorityRank", SqlDbType.Int).Value = proposedPriorityRank;
				//ActualPriorityRank must be approved, cannot be set by normal users
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				try
				{
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
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Update specified WorkloadGroup record
	/// - Can only set ProposedPriorityRank here
	///   - ActualPriorityRank must be signed off by "Admin"
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkloadGroup_Update(int workloadGroupID
		, string workloadGroup
		, string description
		, int proposedPriorityRank
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkloadGroup_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkloadGroupID", SqlDbType.Int).Value = workloadGroupID;
				cmd.Parameters.Add("@WorkloadGroup", SqlDbType.NVarChar).Value = workloadGroup;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@ProposedPriorityRank", SqlDbType.Int).Value = proposedPriorityRank;
				//ActualPriorityRank must be approved, cannot be set by normal users
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
					cmd.ExecuteNonQuery();

					SqlParameter paramSaved = cmd.Parameters["@saved"];
					if (paramSaved != null)
					{
						bool.TryParse(paramSaved.Value.ToString(), out saved);
					}
				}
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Delete WorkloadGroup record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkloadGroup_Delete(int workloadGroupID
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

		string procName = "WorkloadGroup_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkloadGroupID", SqlDbType.Int).Value = workloadGroupID;

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
						errorMsg = "WorkloadGroup record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "WorkloadGroup record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion WorkloadGroup


	#region WorkArea

	/// <summary>
	/// Load WorkArea Items
	/// </summary>
	/// <returns>Datatable of WorkArea Items</returns>
	public static DataTable WorkAreaList_Get(bool includeArchive = false, string cv = "0", int systemSuiteID = 0, string systemIDs = "", int workArea_SystemID = 0)
	{
		string procName = "WorkAreaList_Get";

		using (DataTable dt = new DataTable("WorkArea"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;
                    cmd.Parameters.Add("@CV", SqlDbType.NVarChar).Value = cv;
                    cmd.Parameters.Add("@SystemSuiteID", SqlDbType.Int).Value = systemSuiteID;
					cmd.Parameters.Add("@SystemIDs", SqlDbType.VarChar).Value = !string.IsNullOrWhiteSpace(systemIDs) ? systemIDs : (object)DBNull.Value;
                    cmd.Parameters.Add("@WorkArea_SystemID", SqlDbType.Int).Value = workArea_SystemID;

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
	/// Add new WorkArea record
	/// - Can only set ProposedPriorityRank here
	///   - ActualPriorityRank must be signed off by "Admin"
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkArea_Add(
		string WorkArea
		, string description
		, int proposedPriorityRank
		, out bool exists
		, out int newID
		, out string errorMsg)
	{
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkArea_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkArea", SqlDbType.NVarChar).Value = WorkArea;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@ProposedPriorityRank", SqlDbType.Int).Value = proposedPriorityRank;
				//ActualPriorityRank must be approved, cannot be set by normal users
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				try
				{
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
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Update specified WorkArea record
	/// - Can only set ProposedPriorityRank here
	///   - ActualPriorityRank must be signed off by "Admin"
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkArea_Update(int WorkAreaID
		, string WorkArea
		, string description
		, int proposedPriorityRank
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkArea_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkAreaID", SqlDbType.Int).Value = WorkAreaID;
				cmd.Parameters.Add("@WorkArea", SqlDbType.NVarChar).Value = WorkArea;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@ProposedPriorityRank", SqlDbType.Int).Value = proposedPriorityRank;
				//ActualPriorityRank must be approved, cannot be set by normal users
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
					cmd.ExecuteNonQuery();

					SqlParameter paramSaved = cmd.Parameters["@saved"];
					if (paramSaved != null)
					{
						bool.TryParse(paramSaved.Value.ToString(), out saved);
					}
				}
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Delete WorkArea record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkArea_Delete(int WorkAreaID
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

		string procName = "WorkArea_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkAreaID", SqlDbType.Int).Value = WorkAreaID;

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
						errorMsg = "WorkArea record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "WorkArea record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    public static DataTable WorkArea_Get(int WorkAreaID)
    {
        return WTSData.GetDataTableFromStoredProcedure("WorkArea_Get", null, new SqlParameter[] {
            new SqlParameter("@WorkAreaID", WorkAreaID)
        });
    }

    /// <summary>
	/// Load WorkArea Items
	/// </summary>
	/// <returns>Datatable of WorkArea Items</returns>
	public static DataTable SystemSuite_WorkAreaList_Get(bool includeArchive = false, int systemSuiteID = 0, int workTaskStatus = 0)
    {
        return WTSData.GetDataTableFromStoredProcedure("WTS_SYSTEM_SUITE_WorkAreaList_Get", null, new SqlParameter[] {
            new SqlParameter("@IncludeArchive", includeArchive ? 1 : 0),
            new SqlParameter("@SystemSuiteID", systemSuiteID),
            new SqlParameter("@WorkTaskStatus", workTaskStatus)
        });
    }

    #endregion WorkArea


    #region WorkArea - System

    /// <summary>
    /// Load WorkArea_System Items
    /// </summary>
    /// <returns>Datatable of WorkArea_System Items</returns>
    public static DataTable WorkArea_SystemList_Get(int workAreaID = 0
		, int systemID = 0
        , int systemSuiteID = 0
        , string systemIDs = ""
        , string cv = "0")
	{
		string procName = "WorkArea_SystemList_Get";

		using (DataTable dt = new DataTable("WorkArea_System"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WorkAreaID", SqlDbType.Int).Value = workAreaID == 0 ? (object)DBNull.Value : workAreaID;
					cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = systemID == 0 ? (object)DBNull.Value : systemID;
					cmd.Parameters.Add("@WTS_SYSTEM_SUITEID", SqlDbType.Int).Value = systemSuiteID == 0 ? (object)DBNull.Value : systemSuiteID;
                    cmd.Parameters.Add("@SystemIDs", SqlDbType.VarChar).Value = !string.IsNullOrWhiteSpace(systemIDs) ? systemIDs : (object)DBNull.Value;
                    cmd.Parameters.Add("@CV", SqlDbType.NVarChar).Value = cv;

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
	/// Add new WorkArea_System record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkArea_System_Add(
		int workAreaID
		, int systemID
		, string description
		, int proposedPriority
		, int approvedPriority
        , string cv
		, out bool exists
		, out int newID
		, out string errorMsg
/*        , int? ALLOCATIONGROUPID = null*/)
	{
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkArea_System_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkAreaID", SqlDbType.Int).Value = workAreaID;
				cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.NVarChar).Value = systemID == 0 ? (object)DBNull.Value : systemID;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@ProposedPriority", SqlDbType.Int).Value = proposedPriority;
				cmd.Parameters.Add("@ApprovedPriority", SqlDbType.Int).Value = approvedPriority;
                cmd.Parameters.Add("@CV", SqlDbType.NVarChar).Value = cv;
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
	/// Update specified WorkArea_System record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkArea_System_Update(int workAreaSystemID
		, int workAreaID
		, int systemID
		, string description
		, int proposedPriority
		, int approvedPriority
        , string cv
        , bool archive
		, out bool duplicate
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		duplicate = false;
		bool saved = false;

		string procName = "WorkArea_System_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkArea_SystemID", SqlDbType.Int).Value = workAreaSystemID;
				cmd.Parameters.Add("@WorkAreaID", SqlDbType.Int).Value = workAreaID;
				cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = systemID == 0 ? (object)DBNull.Value : systemID;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@ProposedPriority", SqlDbType.Int).Value = proposedPriority;
				cmd.Parameters.Add("@ApprovedPriority", SqlDbType.Int).Value = approvedPriority;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@CV", SqlDbType.NVarChar).Value = cv;
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

	/// <summary>
	/// Delete WorkArea_System record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkArea_System_Delete(int workAreaSystemID
        , string cv
		, out bool exists
		, out string errorMsg)
	{
		exists = false;
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "WorkArea_System_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkArea_SystemID", SqlDbType.Int).Value = workAreaSystemID;
                cmd.Parameters.Add("@CV", SqlDbType.NVarChar).Value = cv;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();

				SqlParameter paramExists = cmd.Parameters["@exists"];
				if (paramExists != null)
				{
					bool.TryParse(paramExists.Value.ToString(), out exists);
					if (!exists)
					{
                        if (cv == "0"){
                            errorMsg = "WorkArea_System record could not be found.";
                        }
                        else {
                            errorMsg = "Allocation_System record could not be found.";
                        }
						return false;
					}
				}
				SqlParameter paramDeleted = cmd.Parameters["@deleted"];
				if (paramDeleted != null)
				{
					bool.TryParse(paramDeleted.Value.ToString(), out deleted);
				}
			}
		}

		return deleted;
	}

	#endregion WorkArea - System


	#region WorkItemType

	/// <summary>
	/// Load WorkItemType Items
	/// </summary>
	/// <returns>Datatable of WorkItemType Items</returns>
	public static DataTable WorkItemTypeList_Get(bool includeArchive = false)
	{
		string procName = "WorkItemTypeList_Get";

		using (DataTable dt = new DataTable("WorkItemType"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Add new WorkItemType record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkItemType_Add(
		string WorkItemType
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

		string procName = "WorkItemType_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItemType", SqlDbType.NVarChar).Value = WorkItemType;
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
	/// Update specified WorkItemType record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkItemType_Update(int WorkItemTypeID
		, string WorkItemType
		, string description
		, int sortOrder
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkItemType_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItemTypeID", SqlDbType.Int).Value = WorkItemTypeID;
				cmd.Parameters.Add("@WorkItemType", SqlDbType.NVarChar).Value = WorkItemType;
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
	/// Delete WorkItemType record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkItemType_Delete(int WorkItemTypeID
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

		string procName = "WorkItemType_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkItemTypeID", SqlDbType.Int).Value = WorkItemTypeID;

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
						errorMsg = "WorkItemType record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "WorkItemType record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    /// <summary>
    /// Load System_WorkActivity Items
    /// </summary>
    /// <returns>Datatable of System_WorkActivity Items</returns>
    public static DataTable WTS_System_Suite_WorkActivityList_Get(int SystemSuiteID
        , int includeArchive = 0)
    {
        string procName = "WTS_SYSTEM_SUITE_WorkActivityList_Get";

        using (DataTable dt = new DataTable("System_WorkActivity"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WTS_SYSTEM_SUITEID", SqlDbType.Int).Value = SystemSuiteID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;

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
    /// Add new System_WorkActivity record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_WorkActivity_Add(
        int WTS_SYSTEMID
        , int WTS_WORKACTIVITYID
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_System_WorkActivity_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = WTS_SYSTEMID;
                cmd.Parameters.Add("@WorkItemTypeID", SqlDbType.Int).Value = WTS_WORKACTIVITYID;
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
    /// Update specified System_WorkActivity record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_WorkActivity_Update(int WTS_SYSTEMID
        , int WTS_SYSTEM_WORKACTIVITYID
        , int WTS_WORKACTIVITYID
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "WTS_System_WorkActivity_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = WTS_SYSTEMID;
                cmd.Parameters.Add("@WTS_SYSTEM_WORKACTIVITYID", SqlDbType.Int).Value = WTS_SYSTEM_WORKACTIVITYID;
                cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WTS_WORKACTIVITYID;
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

    /// <summary>
    /// Delete System_WorkActivity record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_WorkActivity_Delete(int WTS_SYSTEM_WORKACTIVITYID
        , out bool exists
        , out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "WTS_System_WorkActivity_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEM_WORKACTIVITYID", SqlDbType.Int).Value = WTS_SYSTEM_WORKACTIVITYID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        errorMsg = "System_Resource record could not be found.";

                        return false;
                    }
                }
                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }

    /// <summary>
    /// Load System_WorkActivity Items
    /// </summary>
    /// <returns>Datatable of System_WorkActivity Items</returns>
    public static DataTable ItemType_SystemList_Get(int WorkItemTypeID
        , int SystemSuiteID
        , int includeArchive = 0)
    {
        string procName = "ItemType_SystemList_Get";

        using (DataTable dt = new DataTable("System_Resource"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WORKITEMTYPEID", SqlDbType.Int).Value = WorkItemTypeID;
                    cmd.Parameters.Add("@WTS_SYSTEM_SUITEID", SqlDbType.Int).Value = SystemSuiteID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;

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

    #endregion WorkItemType

    #region WorkActivityGroup

    /// <summary>
    /// Load WorkActivityGroup Items
    /// </summary>
    /// <returns>Datatable of WorkActivityGroup Items</returns>
    public static DataTable WorkActivityGroupList_Get(bool includeArchive = false)
    {
        string procName = "WorkActivityGroupList_Get";

        using (DataTable dt = new DataTable("WorkActivityGroup"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
    /// Add new WorkActivityGroup record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkActivityGroup_Add(
        string workActivityGroup
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

        string procName = "WorkActivityGroup_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkActivityGroup", SqlDbType.NVarChar).Value = workActivityGroup;
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
    /// Update specified WorkActivityGroup record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkActivityGroup_Update(int workActivityGroupID
        , string workActivityGroup
        , string description
        , int sortOrder
        , bool archive
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WorkActivityGroup_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkActivityGroupID", SqlDbType.Int).Value = workActivityGroupID;
                cmd.Parameters.Add("@WorkActivityGroup", SqlDbType.NVarChar).Value = workActivityGroup;
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
    /// Delete WorkActivityGroup record
    ///  - will archive if record is assigned to anything
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkActivityGroup_Delete(int workActivityGroupID
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

        string procName = "WorkActivityGroup_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkActivityGroupID", SqlDbType.Int).Value = workActivityGroupID;

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
                        errorMsg = "Phase record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "WorkActivityGroup record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    #endregion WorkActivityGroup

    #region WorkType

    /// <summary>
    /// Load Progress Items
    /// </summary>
    /// <returns>Datatable of Progress Items</returns>
    public static DataTable WorkTypeList_Get(bool includeArchive = false)
	{
		string procName = "WorkTypeList_Get";

		using (DataTable dt = new DataTable("WorkType"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Add new WorkType record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkType_Add(
		string workType
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

		string procName = "WorkType_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkType", SqlDbType.NVarChar).Value = workType;
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
	/// Update specified WorkType record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkType_Update(int workTypeID
		, string workType
		, string description
		, int sortOrder
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkType_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;
				cmd.Parameters.Add("@WorkType", SqlDbType.NVarChar).Value = workType;
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
	/// Delete WorkType record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkType_Delete(int workTypeID
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

		string procName = "WorkType_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;

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
						errorMsg = "WorkType record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "WorkType record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion WorkType


	#region WorkType - Phase

	/// <summary>
	/// Load WorkType - Phase Items
	/// </summary>
	/// <returns>Datatable of Phase Items</returns>
	public static DataTable WorkType_PhaseList_Get(int workTypeID = 0
		, int phaseID = 0)
	{
		string procName = "WorkType_PhaseList_Get";

		using (DataTable dt = new DataTable("WorkType_Phase"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID == 0 ? (object)DBNull.Value : workTypeID;
					cmd.Parameters.Add("@PhaseID", SqlDbType.Int).Value = phaseID == 0 ? (object)DBNull.Value : phaseID;

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
	/// Add new WorkType_Phase record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkType_Phase_Add(
		int phaseID
		, int workTypeID
		, string description
		, out bool exists
		, out int newID
		, out string errorMsg)
	{
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkType_Phase_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@PhaseID", SqlDbType.Int).Value = phaseID;
				cmd.Parameters.Add("@WorkTypeID", SqlDbType.NVarChar).Value = workTypeID;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
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
	/// Update specified WorkType_Phase record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkType_Phase_Update(int workTypePhaseID
		, int phaseID
		, int workTypeID
		, string description
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkType_Phase_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkTypePhaseID", SqlDbType.Int).Value = workTypePhaseID;
				cmd.Parameters.Add("@PhaseID", SqlDbType.Int).Value = phaseID;
				cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
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
	/// Delete WorkType_Phase record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkType_Phase_Delete(int workType_PhaseID
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

		string procName = "WorkType_Phase_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WorkType_PhaseID", SqlDbType.Int).Value = workType_PhaseID;

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
						errorMsg = "WorkType-Phase record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "WorkType-Phase record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion WorkType - Phase


	#region WorkType - Status

	/// <summary>
	/// Load Work Type - Status Items
	/// </summary>
	/// <returns>Datatable of WorkType_Status Items</returns>
	public static DataTable WorkType_StatusList_Get(int workTypeID = 0
		, int statusID = 0)
	{
		string procName = "WorkType_StatusList_Get";

		using (DataTable dt = new DataTable("WorkType_Status"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID == 0 ? (object)DBNull.Value : workTypeID;
					cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID == 0 ? (object)DBNull.Value : statusID;

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
	/// Add new WorkType_Status record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkType_Status_Add(
		int statusID
		, int workTypeID
		, string description
		, out bool exists
		, out int newID
		, out string errorMsg)
	{
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkType_Status_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID;
				cmd.Parameters.Add("@WorkTypeID", SqlDbType.NVarChar).Value = workTypeID;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
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
	/// Update specified WorkType_Status record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkType_Status_Update(int workTypeStatusID
		, int statusID
		, int workTypeID
		, string description
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WorkType_Status_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@StatusWorkTypeID", SqlDbType.Int).Value = workTypeStatusID;
				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID;
				cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
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
	/// Delete WorkType_Status record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool WorkType_Status_Delete(int workType_StatusID
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

		string procName = "WorkType_Status_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@Status_WorkTypeID", SqlDbType.Int).Value = workType_StatusID;

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
						errorMsg = "WorkType-Status record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "WorkType-Status record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    #endregion WorkType - Status

    #region WorkType - Resource Type

    /// <summary>
    /// Load Work Type - Resource Type Items
    /// </summary>
    /// <returns>Datatable of WorkType_ResourceType Items</returns>
    public static DataTable WorkType_ResourceTypeList_Get(int workTypeID = 0
        , int resourceTypeID = 0)
    {
        string procName = "WorkType_ResourceTypeList_Get";

        using (DataTable dt = new DataTable("WorkType_ResourceType"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID == 0 ? (object)DBNull.Value : workTypeID;
                    cmd.Parameters.Add("@WTS_RESOURCE_TYPEID", SqlDbType.Int).Value = resourceTypeID == 0 ? (object)DBNull.Value : resourceTypeID;

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
    /// Add new WorkType_ResourceType record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkType_ResourceType_Add(
        int resourceTypeID
        , int workTypeID
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WorkType_ResourceType_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_RESOURCE_TYPEID", SqlDbType.Int).Value = resourceTypeID;
                cmd.Parameters.Add("@WorkTypeID", SqlDbType.NVarChar).Value = workTypeID;
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
    /// Update specified WorkType_ResourceType record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkType_ResourceType_Update(int workTypeResourceTypeID
        , int resourceTypeID
        , int workTypeID
        , bool archive
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WorkType_ResourceType_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkType_WTS_RESOURCE_TYPEID", SqlDbType.Int).Value = workTypeResourceTypeID;
                cmd.Parameters.Add("@WTS_RESOURCE_TYPEID", SqlDbType.Int).Value = resourceTypeID;
                cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;
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
    /// Delete WorkType_ResourceType record
    ///  - will archive if record is assigned to anything
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkType_ResourceType_Delete(int workTypeResourceTypeID
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

        string procName = "WorkType_ResourceType_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkType_WTS_RESOURCE_TYPEID", SqlDbType.Int).Value = workTypeResourceTypeID;
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
                        errorMsg = "WorkType-Resource Type record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "WorkType-Resource Type record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    #endregion WorkType - Resource Type

    #region WorkType - Organization

    /// <summary>
    /// Load Work Type - Organization Items
    /// </summary>
    /// <returns>Datatable of WorkType_ResourceType Items</returns>
    public static DataTable WorkType_OrganizationList_Get(int workTypeID = 0
        , int organizationID = 0)
    {
        string procName = "WorkType_OrganizationList_Get";

        using (DataTable dt = new DataTable("WorkType_Organization"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID == 0 ? (object)DBNull.Value : workTypeID;
                    cmd.Parameters.Add("@ORGANIZATIONID", SqlDbType.Int).Value = organizationID == 0 ? (object)DBNull.Value : organizationID;

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
    /// Add new WorkType_Organization record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkType_Organization_Add(
        int organizationID
        , int workTypeID
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WorkType_Organization_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ORGANIZATIONID", SqlDbType.Int).Value = organizationID;
                cmd.Parameters.Add("@WorkTypeID", SqlDbType.NVarChar).Value = workTypeID;
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
    /// Update specified WorkType_Organization record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkType_Organization_Update(int workTypeOrganizationID
        , int organizationID
        , int workTypeID
        , bool archive
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WorkType_Organization_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkType_ORGANIZATIONID", SqlDbType.Int).Value = workTypeOrganizationID;
                cmd.Parameters.Add("@ORGANIZATIONID", SqlDbType.Int).Value = organizationID;
                cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;
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
    /// Delete WorkType_Organization record
    ///  - will archive if record is assigned to anything
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkType_Organization_Delete(int workTypeOrganizationID
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

        string procName = "WorkType_Organization_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkType_ORGANIZATIONID", SqlDbType.Int).Value = workTypeOrganizationID;
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
                        errorMsg = "WorkType-Organization record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "WorkType-Organization record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    #endregion WorkType - Organization

    #region WorkType - Resource

    /// <summary>
    /// Load Work Type - Resource Items
    /// </summary>
    /// <returns>Datatable of WorkType_Resource Items</returns>
    public static DataTable WorkType_ResourceList_Get(int workTypeID = 0
        , int statusID = 0)
    {
        string procName = "WorkType_ResourceList_Get";

        using (DataTable dt = new DataTable("WorkType_Resource"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID == 0 ? (object)DBNull.Value : workTypeID;

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
    /// Add new WorkType_Resource record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkType_Resource_Add(
        int resourceID
        , int workTypeID
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WorkType_Resource_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = resourceID;
                cmd.Parameters.Add("@WorkTypeID", SqlDbType.NVarChar).Value = workTypeID;
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
    /// Update specified WorkType_Resource record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkType_Resource_Update(int workTypeResourceID
        , int resourceID
        , int workTypeID
        , bool archive
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WorkType_Resource_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkType_WTS_RESOURCEID", SqlDbType.Int).Value = workTypeResourceID;
                cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = resourceID;
                cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;
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
    /// Delete WorkType_Resource record
    ///  - will archive if record is assigned to anything
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkType_Resource_Delete(int workTypeResourceID
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

        string procName = "WorkType_Resource_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkType_WTS_RESOURCEID", SqlDbType.Int).Value = workTypeResourceID;

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
                        errorMsg = "WorkType-Resource record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "WorkType-Resource record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    /// <summary>
    /// Add new WorkType_Resource record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkType_Resource_Sync(
        out bool exists
        , out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool saved = false;
        int newID = 0;

        string procName = "WorkType_Resource_Sync";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

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

    #endregion WorkType - Resource

    #region Scope

    /// <summary>
    /// Load Scope Items
    /// </summary>
    /// <returns>Datatable of Scope Items</returns>
    public static DataTable ScopeList_Get(bool includeArchive = false)
	{
		string procName = "WTS_ScopeList_Get";

		using (DataTable dt = new DataTable("Scope"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
    /// Load Item Type
    /// </summary>
    /// <returns>Datatable of Allocation Group</returns>
    public static DataTable AllocationGroup_Get()
    {
        string procName = "WTS_AllocationGroup_Get";

        using (DataTable dt = new DataTable("AllocationGroup"))
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
    /// Add new ItemType record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool AllocationGroup_Add(
        string ALLOCATIONGROUP
        , string DESCRIPTION
        , string NOTES
        , int PRIORTY
        , bool DAILYMEETINGS
        , bool ARCHIVE
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_AllocationGroup_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ALLOCATIONGROUP", SqlDbType.NVarChar).Value = ALLOCATIONGROUP;
                cmd.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar).Value = DESCRIPTION;
                cmd.Parameters.Add("@NOTES", SqlDbType.NVarChar).Value = NOTES;
                cmd.Parameters.Add("@PRIORTY", SqlDbType.Int).Value = PRIORTY;
                cmd.Parameters.Add("@DAILYMEETINGS", SqlDbType.Bit).Value = DAILYMEETINGS ? 1 : 0;
                cmd.Parameters.Add("@ARCHIVE", SqlDbType.Bit).Value = ARCHIVE ? 1 : 0;
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
    /// Update specified AllocationGroup record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool AllocationGroup_Update(
          int ALLOCATIONGROUPID
        , string ALLOCATIONGROUP
        , string DESCRIPTION
        , string NOTES
        , int PRIORTY
        , bool DAILYMEETINGS
        , bool ARCHIVE
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "AllocationGroup_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ALLOCATIONGROUPID", SqlDbType.Int).Value = ALLOCATIONGROUPID;
                cmd.Parameters.Add("@ALLOCATIONGROUP", SqlDbType.NVarChar).Value = ALLOCATIONGROUP;
                cmd.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar).Value = DESCRIPTION;
                cmd.Parameters.Add("@NOTES", SqlDbType.NVarChar).Value = NOTES;
                cmd.Parameters.Add("@PRIORTY", SqlDbType.Int).Value = PRIORTY;
                cmd.Parameters.Add("@DAILYMEETINGS", SqlDbType.Bit).Value = DAILYMEETINGS ? 1 : 0;
                cmd.Parameters.Add("@ARCHIVE", SqlDbType.Bit).Value = ARCHIVE ? 1 : 0;
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
    /// Update specified AllocationGroup record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool Allocation_Set_GroupID(
          int ALLOCATIONID,
          int ALLOCATIONGROUPID,
          out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_ALLOCATION_SET_GROUPID";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ALLOCATIONID", SqlDbType.Int).Value = ALLOCATIONID;
                cmd.Parameters.Add("@ALLOCATIONGROUPID", SqlDbType.Int).Value = ALLOCATIONGROUPID;

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
    /// Delete AllocationGroup record
    ///  - will archive if record is assigned to anything
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool AllocatonGroup_Delete(int AllocationGroupID
        , out bool exists
        , out bool archived
        , out string errorMsg)
    {
        exists = false;
        archived = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "WTS_AllocationGroup_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ALLOCATIONGROUPID", SqlDbType.Int).Value = AllocationGroupID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@archived", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        archived = false;
                        errorMsg = "Allocation Group record could not be found.";
                        return false;
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

	/// <summary>
	/// Add new Scope record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Scope_Add(
		string scope
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

		string procName = "WTS_Scope_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@Scope", SqlDbType.NVarChar).Value = scope;
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
	/// Update specified Scope record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Scope_Update(int scopeID
		, string scope
		, string description
		, int sortOrder
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WTS_Scope_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WTS_ScopeID", SqlDbType.Int).Value = scopeID;
				cmd.Parameters.Add("@Scope", SqlDbType.NVarChar).Value = scope;
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
	/// Delete Scope record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool Scope_Delete(int scopeID
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

		string procName = "WTS_Scope_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WTS_ScopeID", SqlDbType.Int).Value = scopeID;

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
						errorMsg = "Scope record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Scope record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    #endregion Scope


    #region System

    /// <summary>
    /// Load System Items
    /// </summary>
    /// <returns>Datatable of System Items</returns>
    public static DataTable SystemList_Get(bool includeArchive = false, string cv = "0", int ProductVersionID = 0, int ContractID = 0, int WTS_SYSTEM_SUITEID = 0)
	{
		string procName = "WTS_SystemList_Get";

		using (DataTable dt = new DataTable("System"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;
                    cmd.Parameters.Add("@CV", SqlDbType.NVarChar).Value = cv;
                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
                    cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = ContractID;
                    cmd.Parameters.Add("@WTS_SYSTEM_SUITEID", SqlDbType.Int).Value = WTS_SYSTEM_SUITEID;

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
	/// Add new System record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool System_Add(
		string system
		, string description
		, int sortOrder
        , int busWorkloadManagerID
        , int devWorkloadManagerID
		, bool archive
		, out bool exists
		, out int newID
		, out string errorMsg
		, int contractID = -1)
    {
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WTS_System_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WTS_System", SqlDbType.NVarChar).Value = system;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@BusWorkloadManagerID", SqlDbType.Int).Value = busWorkloadManagerID == 0 ? (object)DBNull.Value : busWorkloadManagerID;
                cmd.Parameters.Add("@DevWorkloadManagerID", SqlDbType.Int).Value = devWorkloadManagerID == 0 ? (object)DBNull.Value : devWorkloadManagerID;
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

    public static DataTable WTS_System_Get(int systemID)
    {
        return WTSData.GetDataTableFromStoredProcedure("WTS_System_Get", null, new SqlParameter[] {
            new SqlParameter("@WTS_SystemID", systemID)
        });
    }

    /// <summary>
    /// Update specified System record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool System_Update(int systemID
        , string system
        , string description
        , int sortOrder
        , int busWorkloadManagerID
        , int devWorkloadManagerID
        , bool archive
        , out string errorMsg
        , int WTS_SYSTEM_SUITEID = 0
        , int contractID = -1)
    {
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "WTS_System_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WTS_SystemID", SqlDbType.Int).Value = systemID;
				cmd.Parameters.Add("@WTS_System", SqlDbType.NVarChar).Value = system;
                cmd.Parameters.Add("@WTS_System_SuiteID", SqlDbType.NVarChar).Value = WTS_SYSTEM_SUITEID;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@BusWorkloadManagerID", SqlDbType.Int).Value = busWorkloadManagerID == 0 ? (object)DBNull.Value : busWorkloadManagerID;
                cmd.Parameters.Add("@DevWorkloadManagerID", SqlDbType.Int).Value = devWorkloadManagerID == 0 ? (object)DBNull.Value : devWorkloadManagerID;
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

    public static bool System_Remove_Suite(int systemID, out bool exists, out string errorMsg)
    {

        string procName = "WTS_System_Remove_Suite";
        bool saved = false;
        exists = false;
        errorMsg = string.Empty;
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SystemID", SqlDbType.Int).Value = systemID;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;



                try
                {
                    cmd.ExecuteNonQuery();

                    SqlParameter paramSaved = cmd.Parameters["@saved"];
                    if (paramSaved != null)
                    {
                        bool.TryParse(paramSaved.Value.ToString(), out saved);
                    }
                    SqlParameter paramExists = cmd.Parameters["@exists"];
                    if (paramExists != null)
                    {
                        bool.TryParse(paramExists.Value.ToString(), out exists);
                        if (!exists)
                        {
                            errorMsg = "System record could not be found.";
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = ex.ToString();
                    LogUtility.LogException(ex);
                    throw;
                }


            }
        }

        return saved;
    }

    /// <summary>
    /// Delete System record
    ///  - will archive if record is assigned to anything
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool System_Delete(int systemID
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

		string procName = "WTS_System_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@WTS_SystemID", SqlDbType.Int).Value = systemID;

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
						errorMsg = "System record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "System record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    /// <summary>
    /// Update System record
    ///  - will update ReviewWorkArea fields
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool System_ReviewWorkAreas(int systemID, out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_System_ReviewWorkAreas";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SystemID", SqlDbType.Int).Value = systemID;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@saved"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out saved);
                }
            }
        }

        return saved;
    }

    #endregion System

    #region System - Resource

    /// <summary>
    /// Load System_Resource Items
    /// </summary>
    /// <returns>Datatable of System_Resource Items</returns>
    public static DataTable WTS_System_ResourceList_Get(int WTS_SYSTEMID
        , int ProductVersionID)
    {
        string procName = "WTS_System_ResourceList_Get";

        using (DataTable dt = new DataTable("System_Resource"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = WTS_SYSTEMID;
                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;

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
    /// Load System_Resource Items
    /// </summary>
    /// <returns>Datatable of System_Resource Items</returns>
    public static DataSet WTS_System_Suite_ResourceList_Get(int SystemSuiteID
        , int ProductVersionID
        , int includeArchive = 0)
    {
        DataSet ds = new DataSet();
        string procName = "WTS_SYSTEM_SUITE_ResourceList_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SystemSuiteID", SqlDbType.Int).Value = SystemSuiteID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
                cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "SystemResourceList");
                da.TableMappings.Add("Table1", "SuiteGrid");

                da.Fill(ds);
            }
        }

        return ds;
    }

    /// <summary>
    /// Load System_Resource Items
    /// </summary>
    /// <returns>Datatable of System_Resource Items</returns>
    public static DataTable Resource_SystemList_Get(
        int SystemSuiteResourceID
        , int includeArchive = 0)
    {
        string procName = "Resource_SystemList_Get";

        using (DataTable dt = new DataTable("System_Resource"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WTS_SYSTEM_SUITE_RESOURCEID", SqlDbType.Int).Value = SystemSuiteResourceID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;

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

    public static DataTable WTS_SystemList_GetWithResources(int productVersionID, int contractID, bool includeSystemArchive, bool includeResourceArchive)
    {
        string procName = "WTS_SystemList_GetWithResources";

        using (DataTable dt = new DataTable("SystemList_WithResources"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = productVersionID;
                    cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                    cmd.Parameters.Add("@IncludeSystemArchive", SqlDbType.Bit).Value = includeSystemArchive;
                    cmd.Parameters.Add("@IncludeResourceArchive", SqlDbType.Bit).Value = includeResourceArchive;

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

    public static DataTable WTS_System_ResourceAltList_Get(int WTS_RESOURCEID
        , int ProductVersionID)
    {
        string procName = "WTS_System_ResourceAltList_Get";

        using (DataTable dt = new DataTable("System_Resource"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = WTS_RESOURCEID;
                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;

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
    /// Add new System_Resource record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_Resource_Add(
        int WTS_SYSTEMID
        , int ProductVersionID
        , int WTS_RESOURCEID
        , int ActionTeam
        , int AORRoleID
        , int Allocation
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_System_Resource_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = WTS_SYSTEMID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID == 0 ? (object)DBNull.Value : ProductVersionID;
                cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = WTS_RESOURCEID;
				cmd.Parameters.Add("@ActionTeam", SqlDbType.Bit).Value = ActionTeam;
                cmd.Parameters.Add("@AORRoleID", SqlDbType.Int).Value = AORRoleID == 0 ? (object)DBNull.Value : AORRoleID;
                cmd.Parameters.Add("@Allocation", SqlDbType.Int).Value = Allocation;
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
    /// Update specified System_Resource record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_Resource_Update(int WTS_SYSTEMID
        , int ProductVersionID
        , int WTS_SYSTEM_RESOURCEID
        , int WTS_RESOURCEID
        , int AORRoleID
        , int Allocation
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "WTS_System_Resource_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = WTS_SYSTEMID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
                cmd.Parameters.Add("@WTS_SYSTEM_RESOURCEID", SqlDbType.Int).Value = WTS_SYSTEM_RESOURCEID;
                cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = WTS_RESOURCEID;
                cmd.Parameters.Add("@AORRoleID", SqlDbType.Int).Value = AORRoleID == 0 ? (object)DBNull.Value : AORRoleID;
                cmd.Parameters.Add("@Allocation", SqlDbType.Int).Value = Allocation;
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

    /// <summary>
    /// Delete System_Resource record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_Resource_Delete(int WTS_SYSTEM_RESOURCEID
        , out bool exists
        , out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "WTS_System_Resource_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEM_RESOURCEID", SqlDbType.Int).Value = WTS_SYSTEM_RESOURCEID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        errorMsg = "System_Resource record could not be found.";

                        return false;
                    }
                }
                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }

    public static bool WTS_System_ResourceAlt_Save(XmlDocument Changes)
    {
        bool saved = false;
        string procName = "WTS_System_ResourceAlt_Save";

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

    #endregion System - Resource

    #region System - Contract
    /// <summary>
    /// Load System_Contract Items
    /// </summary>
    /// <returns>Datatable of System_Contract Items</returns>
    public static DataTable WTS_System_ContractList_Get(int WTS_SYSTEMID = 0)
    {
        string procName = "WTS_System_ContractList_Get";

        using (DataTable dt = new DataTable("System_Contract"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = WTS_SYSTEMID;

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

    public static DataTable WTS_Contract_SystemList_Get(int CONTRACTID)
    {
        string procName = "WTS_Contract_SystemList_Get";

        using (DataTable dt = new DataTable("Contract_System"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = CONTRACTID;

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
    /// Add new System_Contract record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_Contract_Add(
        int WTS_SYSTEMID
        , int CONTRACTID
        , bool primary
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_System_Contract_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = WTS_SYSTEMID;
                cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = CONTRACTID;
                cmd.Parameters.Add("@Primary", SqlDbType.Bit).Value = primary ? 1 : 0;
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
    /// Update specified System_Contract record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_Contract_Update(int WTS_SYSTEMID
        , int WTS_SYSTEM_CONTRACTID
        , int CONTRACTID
        , bool primary
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "WTS_System_Contract_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEMID", SqlDbType.Int).Value = WTS_SYSTEMID;
                cmd.Parameters.Add("@WTS_SYSTEM_CONTRACTID", SqlDbType.Int).Value = WTS_SYSTEM_CONTRACTID;
                cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = CONTRACTID;
                cmd.Parameters.Add("@Primary", SqlDbType.Bit).Value = primary ? 1 : 0;
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

    /// <summary>
    /// Delete System_Contract record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_System_Contract_Delete(int WTS_SYSTEM_CONTRACTID
        , out bool exists
        , out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "WTS_System_Contract_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WTS_SYSTEM_CONTRACTID", SqlDbType.Int).Value = WTS_SYSTEM_CONTRACTID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        errorMsg = "System_Contract record could not be found.";

                        return false;
                    }
                }
                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }

    /// <summary>
    /// Cleanup System_Contract records for Primary
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WTS_Contract_System_Cleanup(out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WTS_Contract_System_Cleanup";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

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
    #endregion

    #region Product Version
    /// <summary>
    /// Load Product Version Items
    /// </summary>
    /// <returns>Datatable of Product Version Items</returns>
    public static DataTable ReleaseSchedule_ReleaseList_Get(bool includeArchive = false)
    {
        string procName = "ReleaseSchedule_ReleaseList_Get";

        using (DataTable dt = new DataTable("ReleaseVersion"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
    /// Load Release Deliverables
    /// </summary>
    /// <returns>Datatable of Product Version Items</returns>
    public static DataTable ReleaseSchedule_DeliverableList_Get(int ReleaseVersionID, bool includeArchive = false)
    {
        string procName = "ReleaseSchedule_DeliverableList_Get";

        using (DataTable dt = new DataTable("ReleaseVersion"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ReleaseVersionID;

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
    /// Add Release Deliverable
    /// </summary>
    /// <returns>Datatable of Product Version Items</returns>
    public static Dictionary<string, string> ReleaseSchedule_Deliverable_Add(string Deliverable, int ProductVersionID, string description, string narrative, bool visible,
                string plannedStart, string plannedEnd,
                string plannedInvStart, string plannedInvEnd,
                string plannedTechStart, string plannedTechEnd,
                string plannedCDStart, string plannedCDEnd,
                string plannedCodingStart, string plannedCodingEnd,
                string plannedITStart, string plannedITEnd,
                string plannedCVTStart, string plannedCVTEnd,
                string plannedAdoptStart, string plannedAdoptEnd,
                string plannedDevTestStart, string plannedDevTestEnd,
                string plannedIP1Start, string plannedIP1End,
                string plannedIP2Start, string plannedIP2End,
                string plannedIP3Start, string plannedIP3End,
                string actualStart, string actualEnd,
                string actualDevTestStart, string actualDevTestEnd,
                string actualIP1Start, string actualIP1End,
                string actualIP2Start, string actualIP2End,
                string actualIP3Start, string actualIP3End, int sortOrder)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "ReleaseSchedule_Deliverable_Add";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Deliverable", SqlDbType.NVarChar).Value = Deliverable;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.NVarChar).Value = ProductVersionID;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Narrative", SqlDbType.NVarChar).Value = narrative;
                cmd.Parameters.Add("@Visible", SqlDbType.Bit).Value = visible ? 1 : 0;
                cmd.Parameters.Add("@PlannedStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedStart);
                cmd.Parameters.Add("@PlannedEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedEnd);
                cmd.Parameters.Add("@PlannedInvStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedInvStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedInvStart);
                cmd.Parameters.Add("@PlannedInvEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedInvEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedInvEnd);
                cmd.Parameters.Add("@PlannedTechStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedTechStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedTechStart);
                cmd.Parameters.Add("@PlannedTechEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedTechEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedTechEnd);
                cmd.Parameters.Add("@PlannedCDStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCDStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedCDStart);
                cmd.Parameters.Add("@PlannedCDEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCDEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedCDEnd);
                cmd.Parameters.Add("@PlannedCodingStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCodingStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedCodingStart);
                cmd.Parameters.Add("@PlannedCodingEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCodingEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedCodingEnd);
                cmd.Parameters.Add("@PlannedITStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedITStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedITStart);
                cmd.Parameters.Add("@PlannedITEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedITEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedITEnd);
                cmd.Parameters.Add("@PlannedCVTStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCVTStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedCVTStart);
                cmd.Parameters.Add("@PlannedCVTEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCVTEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedCVTEnd);
                cmd.Parameters.Add("@PlannedAdoptStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedAdoptStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedAdoptStart);
                cmd.Parameters.Add("@PlannedAdoptEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedAdoptEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedAdoptEnd);
                cmd.Parameters.Add("@PlannedDevTestStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedDevTestStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedDevTestStart);
                cmd.Parameters.Add("@PlannedDevTestEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedDevTestEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedDevTestEnd);
                cmd.Parameters.Add("@PlannedIP1Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP1Start) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP1Start);
                cmd.Parameters.Add("@PlannedIP1End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP1End) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP1End);
                cmd.Parameters.Add("@PlannedIP2Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP2Start) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP2Start);
                cmd.Parameters.Add("@PlannedIP2End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP2End) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP2End);
                cmd.Parameters.Add("@PlannedIP3Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP3Start) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP3Start);
                cmd.Parameters.Add("@PlannedIP3End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP3End) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP3End);
                cmd.Parameters.Add("@ActualStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualStart) ? (object)DBNull.Value : Convert.ToDateTime(actualStart);
                cmd.Parameters.Add("@ActualEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualEnd) ? (object)DBNull.Value : Convert.ToDateTime(actualEnd);
                cmd.Parameters.Add("@ActualDevTestStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualDevTestStart) ? (object)DBNull.Value : Convert.ToDateTime(actualDevTestStart);
                cmd.Parameters.Add("@ActualDevTestEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualDevTestEnd) ? (object)DBNull.Value : Convert.ToDateTime(actualDevTestEnd);
                cmd.Parameters.Add("@ActualIP1Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP1Start) ? (object)DBNull.Value : Convert.ToDateTime(actualIP1Start);
                cmd.Parameters.Add("@ActualIP1End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP1End) ? (object)DBNull.Value : Convert.ToDateTime(actualIP1End);
                cmd.Parameters.Add("@ActualIP2Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP2Start) ? (object)DBNull.Value : Convert.ToDateTime(actualIP2Start);
                cmd.Parameters.Add("@ActualIP2End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP2End) ? (object)DBNull.Value : Convert.ToDateTime(actualIP2End);
                cmd.Parameters.Add("@ActualIP3Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP3Start) ? (object)DBNull.Value : Convert.ToDateTime(actualIP3Start);
                cmd.Parameters.Add("@ActualIP3End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP3End) ? (object)DBNull.Value : Convert.ToDateTime(actualIP3End);
                cmd.Parameters.Add("@SortOrder", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@NewID", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@Exists"];
                SqlParameter paramNewID = cmd.Parameters["@NewID"];

                if (paramExists != null) bool.TryParse(paramExists.Value.ToString(), out exists);
                if (paramNewID != null) int.TryParse(paramNewID.Value.ToString(), out newID);

                result["exists"] = exists.ToString();
                result["newID"] = newID.ToString();
            }
        }

        return result;
    }

    /// <summary>
    /// Update Release Deliverable
    /// </summary>
    /// <returns>Datatable of Product Version Items</returns>
    public static Dictionary<string, string> ReleaseSchedule_Deliverable_Update(int DeliverableID, string Deliverable, int ProductVersionID, string description, string narrative, bool visible,
                string plannedStart, string plannedEnd,
                string plannedInvStart, string plannedInvEnd,
                string plannedTechStart, string plannedTechEnd,
                string plannedCDStart, string plannedCDEnd,
                string plannedCodingStart, string plannedCodingEnd,
                string plannedITStart, string plannedITEnd,
                string plannedCVTStart, string plannedCVTEnd,
                string plannedAdoptStart, string plannedAdoptEnd,
                string plannedDevTestStart, string plannedDevTestEnd,
                string plannedIP1Start, string plannedIP1End,
                string plannedIP2Start, string plannedIP2End,
                string plannedIP3Start, string plannedIP3End,
                string actualStart, string actualEnd,
                string actualDevTestStart, string actualDevTestEnd,
                string actualIP1Start, string actualIP1End,
                string actualIP2Start, string actualIP2End,
                string actualIP3Start, string actualIP3End,
                int sortOrder, int archive, int source = 0)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "ReleaseSchedule_Deliverable_Update";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@DeliverableID", SqlDbType.Int).Value = DeliverableID;
                cmd.Parameters.Add("@Deliverable", SqlDbType.NVarChar).Value = Deliverable;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.NVarChar).Value = ProductVersionID;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Narrative", SqlDbType.NVarChar).Value = narrative;
                cmd.Parameters.Add("@Visible", SqlDbType.Bit).Value = visible ? 1 : 0;
                cmd.Parameters.Add("@PlannedStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedStart);
                cmd.Parameters.Add("@PlannedEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedEnd);
                cmd.Parameters.Add("@PlannedInvStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedInvStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedInvStart);
                cmd.Parameters.Add("@PlannedInvEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedInvEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedInvEnd);
                cmd.Parameters.Add("@PlannedTechStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedTechStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedTechStart);
                cmd.Parameters.Add("@PlannedTechEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedTechEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedTechEnd);
                cmd.Parameters.Add("@PlannedCDStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCDStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedCDStart);
                cmd.Parameters.Add("@PlannedCDEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCDEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedCDEnd);
                cmd.Parameters.Add("@PlannedCodingStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCodingStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedCodingStart);
                cmd.Parameters.Add("@PlannedCodingEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCodingEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedCodingEnd);
                cmd.Parameters.Add("@PlannedITStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedITStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedITStart);
                cmd.Parameters.Add("@PlannedITEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedITEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedITEnd);
                cmd.Parameters.Add("@PlannedCVTStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCVTStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedCVTStart);
                cmd.Parameters.Add("@PlannedCVTEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedCVTEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedCVTEnd);
                cmd.Parameters.Add("@PlannedAdoptStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedAdoptStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedAdoptStart);
                cmd.Parameters.Add("@PlannedAdoptEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedAdoptEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedAdoptEnd);
                cmd.Parameters.Add("@PlannedDevTestStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedDevTestStart) ? (object)DBNull.Value : Convert.ToDateTime(plannedDevTestStart);
                cmd.Parameters.Add("@PlannedDevTestEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedDevTestEnd) ? (object)DBNull.Value : Convert.ToDateTime(plannedDevTestEnd);
                cmd.Parameters.Add("@PlannedIP1Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP1Start) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP1Start);
                cmd.Parameters.Add("@PlannedIP1End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP1End) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP1End);
                cmd.Parameters.Add("@PlannedIP2Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP2Start) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP2Start);
                cmd.Parameters.Add("@PlannedIP2End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP2End) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP2End);
                cmd.Parameters.Add("@PlannedIP3Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP3Start) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP3Start);
                cmd.Parameters.Add("@PlannedIP3End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(plannedIP3End) ? (object)DBNull.Value : Convert.ToDateTime(plannedIP3End);
                cmd.Parameters.Add("@ActualStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualStart) ? (object)DBNull.Value : Convert.ToDateTime(actualStart);
                cmd.Parameters.Add("@ActualEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualEnd) ? (object)DBNull.Value : Convert.ToDateTime(actualEnd);
                cmd.Parameters.Add("@ActualDevTestStart", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualDevTestStart) ? (object)DBNull.Value : Convert.ToDateTime(actualDevTestStart);
                cmd.Parameters.Add("@ActualDevTestEnd", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualDevTestEnd) ? (object)DBNull.Value : Convert.ToDateTime(actualDevTestEnd);
                cmd.Parameters.Add("@ActualIP1Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP1Start) ? (object)DBNull.Value : Convert.ToDateTime(actualIP1Start);
                cmd.Parameters.Add("@ActualIP1End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP1End) ? (object)DBNull.Value : Convert.ToDateTime(actualIP1End);
                cmd.Parameters.Add("@ActualIP2Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP2Start) ? (object)DBNull.Value : Convert.ToDateTime(actualIP2Start);
                cmd.Parameters.Add("@ActualIP2End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP2End) ? (object)DBNull.Value : Convert.ToDateTime(actualIP2End);
                cmd.Parameters.Add("@ActualIP3Start", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP3Start) ? (object)DBNull.Value : Convert.ToDateTime(actualIP3Start);
                cmd.Parameters.Add("@ActualIP3End", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(actualIP3End) ? (object)DBNull.Value : Convert.ToDateTime(actualIP3End);
                cmd.Parameters.Add("@SortOrder", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive;
                cmd.Parameters.Add("@Source", SqlDbType.Int).Value = source;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);

                result["saved"] = saved.ToString();
            }
        }

        return result;
    }

    /// <summary>
    /// Load Release Deliverable
    /// </summary>
    /// <returns>Datatable of Product Version Items</returns>
    public static DataTable ReleaseSchedule_Deliverable_Get(int DeliverableID)
    {
        string procName = "ReleaseSchedule_Deliverable_Get";

        using (DataTable dt = new DataTable("ReleaseVersion"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ReleaseScheduleID", SqlDbType.Int).Value = DeliverableID;

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
    /// Delete Deployment record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool ReleaseSchedule_Deliverable_Delete(int DeploymentID
        , out bool exists
        , out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "ReleaseSchedule_Deliverable_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseScheduleID", SqlDbType.Int).Value = DeploymentID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        errorMsg = "Deployment record could not be found.";

                        return false;
                    }
                }
                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }

    /// <summary>
    /// Load Product Version Item
    /// </summary>
    /// <returns>Datatable of Product Version Items</returns>
    public static DataTable ProductVersion_Get(int ProductVersionID)
    {
        string procName = "ProductVersion_Get";

        using (DataTable dt = new DataTable("ProductVersion"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;

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
    /// Load Product Version Items
    /// </summary>
    /// <returns>Datatable of Product Version Items</returns>
    public static DataTable ProductVersionList_Get(bool includeArchive = false, string qfSystem = "", string qfContract = "")
	{
		string procName = "ProductVersionList_Get";

		using (DataTable dt = new DataTable("ProductVersion"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;
                    cmd.Parameters.Add("@QFSystem", SqlDbType.NVarChar).Value = qfSystem;
                    cmd.Parameters.Add("@QFContract", SqlDbType.NVarChar).Value = qfContract;

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
	/// Add new ProductVersion record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ProductVersion_Add(
		string ProductVersion
		, string description
        , string narrative
        , string startDate
        , string endDate
        , bool defaultSelection
		, int sortOrder
		, int statusID
		, bool archive
		, out bool exists
		, out int newID
		, out string errorMsg)
	{
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "ProductVersion_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@ProductVersion", SqlDbType.NVarChar).Value = ProductVersion;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Narrative", SqlDbType.NVarChar).Value = narrative;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(startDate) ? (object)DBNull.Value : Convert.ToDateTime(startDate);
                cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(endDate) ? (object)DBNull.Value : Convert.ToDateTime(endDate);
                cmd.Parameters.Add("@DefaultSelection", SqlDbType.Bit).Value = defaultSelection ? 1 : 0;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID == 0 ? (object)DBNull.Value : statusID;
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
	/// Update specified ProductVersion record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ProductVersion_Update(int ProductVersionID
		, string ProductVersion
		, string description
        , string narrative
        , string startDate
        , string endDate
        , bool defaultSelection
		, int sortOrder
		, int statusID
		, bool archive
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "ProductVersion_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
				cmd.Parameters.Add("@ProductVersion", SqlDbType.NVarChar).Value = ProductVersion;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Narrative", SqlDbType.NVarChar).Value = narrative;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(startDate) ? (object)DBNull.Value : Convert.ToDateTime(startDate);
                cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(endDate) ? (object)DBNull.Value : Convert.ToDateTime(endDate);
                cmd.Parameters.Add("@DefaultSelection", SqlDbType.Bit).Value = defaultSelection ? 1 : 0;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID == 0 ? (object)DBNull.Value : statusID;
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
	/// Delete ProductVersion record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ProductVersion_Delete(int productVersionID
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

		string procName = "ProductVersion_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = productVersionID;

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
						errorMsg = "ProductVersion record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "ProductVersion record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    #endregion Product Version

    #region Release Session

    /// <summary>
    /// Load ReleaseSession Items
    /// </summary>
    /// <returns>Datatable of Product Version Session Items</returns>
    public static DataTable ReleaseSessionList_Get(int productVersionID = 0, bool includeArchive = false, string qfSystem = "", string qfContract = "", string qfAOR = "")
    {
        string procName = "ReleaseSessionList_Get";

        using (DataTable dt = new DataTable("ReleaseSession"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

			        cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = productVersionID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;
                    cmd.Parameters.Add("@QFSystem", SqlDbType.NVarChar).Value = qfSystem;
                    cmd.Parameters.Add("@QFContract", SqlDbType.NVarChar).Value = qfContract;
                    cmd.Parameters.Add("@QFAOR", SqlDbType.NVarChar).Value = qfAOR;

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

    public static DataTable ReleaseSessionWorkTaskList_Get(int ReleaseSessionID, string ViewType, string QFSystem = "", string QFContract = "", string QFAOR = "")
    {
        string procName = "ReleaseSessionWorkTaskList_Get";

        using (DataTable dt = new DataTable("ReleaseSessionWorkTask"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ReleaseSessionID", SqlDbType.Int).Value = ReleaseSessionID;
                    cmd.Parameters.Add("@ViewType", SqlDbType.NVarChar).Value = ViewType;
                    cmd.Parameters.Add("@QFSystem", SqlDbType.NVarChar).Value = QFSystem;
                    cmd.Parameters.Add("@QFContract", SqlDbType.NVarChar).Value = QFContract;
                    cmd.Parameters.Add("@QFAOR", SqlDbType.NVarChar).Value = QFAOR;

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

    /// <summary>
	/// Add new ReleaseSession record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ReleaseSession_Add(
        string releaseSession 
        , string sessionNarrative
        , int productVersionID
        , string startDate
        , int duration
        , int sort
        , bool archive
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "ReleaseSession_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseSession", SqlDbType.NVarChar).Value = releaseSession;
                cmd.Parameters.Add("@SessionNarrative", SqlDbType.NVarChar).Value = sessionNarrative;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = productVersionID;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(startDate) ? (object)DBNull.Value : Convert.ToDateTime(startDate);
                cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = duration;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
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
	/// Update a ReleaseSession record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ReleaseSession_Update(
        int releaseSessionID
        , string releaseSession
        , string sessionNarrative
        , int productVersionID
        , string startDate
        , int duration
        , int sort
        , bool archive
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "ReleaseSession_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseSessionID", SqlDbType.Int).Value = releaseSessionID;
                cmd.Parameters.Add("@ReleaseSession", SqlDbType.NVarChar).Value = releaseSession;
                cmd.Parameters.Add("@SessionNarrative", SqlDbType.NVarChar).Value = sessionNarrative;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = productVersionID;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = string.IsNullOrWhiteSpace(startDate) ? (object)DBNull.Value : Convert.ToDateTime(startDate);
                cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = duration;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
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
	/// Delete ReleaseSession record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ReleaseSession_Delete(int releaseSessionID
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

        string procName = "ReleaseSession_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseSessionID", SqlDbType.Int).Value = releaseSessionID;

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
                        errorMsg = "Session record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "Session record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    /// <summary>
    /// Load ReleaseSessionBreakout Items
    /// </summary>
    /// <returns>Datatable of Product Version Session Breakout Items</returns>
    public static DataTable ReleaseSessionBreakoutList_Get(int productVersionID = 0, int releaseSessionID = 0, bool includeArchive = false, string qfSystem = "", string qfContract = "", string qfAOR = "")
    {
        string procName = "ReleaseSessionBreakoutList_Get";

        using (DataTable dt = new DataTable("ReleaseSessionBreakout"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = productVersionID;
                    cmd.Parameters.Add("@ReleaseSessionID", SqlDbType.Int).Value = releaseSessionID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;
                    cmd.Parameters.Add("@QFSystem", SqlDbType.NVarChar).Value = qfSystem;
                    cmd.Parameters.Add("@QFContract", SqlDbType.NVarChar).Value = qfContract;
                    cmd.Parameters.Add("@QFAOR", SqlDbType.NVarChar).Value = qfAOR;

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

    #endregion Release Session

    #region AOR Workload Type

    /// <summary>
    /// Load AOR Workload Type Items
    /// </summary>
    /// <returns>Datatable of AOR Workload Type Items</returns>
    public static DataTable AOR_TypeList_Get(bool includeArchive = false)
    {
        DataSet ds = new DataSet();
        string procName = "AORTypeList_Get";

        using (DataTable dt = new DataTable("AOR_Type"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
    /// Add new AOR Workload Type record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool AORType_Add(
        string AORWorkTypeName
        , string description
        , int sort
        , bool archive
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "AORType_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORWorkTypeName", SqlDbType.NVarChar).Value = AORWorkTypeName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    /// <summary>
    /// Update specified AOR Workload Type record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool AORType_Update(int AORWorkTypeID
        , string AORWorkTypeName
        , string description
        , int sort
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "AORType_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORWorkTypeID", SqlDbType.Int).Value = AORWorkTypeID;
                cmd.Parameters.Add("@AORWorkTypeName", SqlDbType.NVarChar).Value = AORWorkTypeName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    /// <summary>
    /// Delete AOR Workload Type record
    ///  - will archive if record is assigned to anything
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool AORType_Delete(int AORWorkTypeID
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

        string procName = "AORType_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORWorkTypeID", SqlDbType.Int).Value = AORWorkTypeID;

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
                        errorMsg = "AOR Work Type record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "AOR Work Type record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    #endregion AOR Workload Type


    #region Effort Area

    /// <summary>
    /// Load Effort Area Items
    /// </summary>
    /// <returns>Datatable of Effort Area Items</returns>
    public static DataTable EffortAreaList_Get(bool includeArchive = false)
	{
		DataSet ds = new DataSet();
		string procName = "EffortAreaList_Get";

		using (DataTable dt = new DataTable("EffortArea"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Add new Effort Area record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool EffortArea_Add(
		string effortArea
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

		string procName = "EffortArea_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@EffortArea", SqlDbType.NVarChar).Value = effortArea;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				try
				{
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
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Update specified Effort Area record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool EffortArea_Update(int effortAreaID
		, string effortArea
		, string description
		, int sortOrder
		, bool archive
		, out bool duplicate
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		duplicate = false;
		bool saved = false;

		string procName = "EffortArea_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@EffortAreaID", SqlDbType.Int).Value = effortAreaID;
				cmd.Parameters.Add("@EffortArea", SqlDbType.NVarChar).Value = effortArea;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
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
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Delete Effort Area record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool EffortArea_Delete(int effortAreaID
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

		string procName = "EffortArea_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@EffortAreaID", SqlDbType.Int).Value = effortAreaID;

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
						errorMsg = "Effort Area record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Effort Level record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion Effort Area


	#region Effort Size

	/// <summary>
	/// Load Effort Size Items
	/// </summary>
	/// <returns>Datatable of Allocation Items</returns>
	public static DataTable EffortSizeList_Get(bool includeArchive = false)
	{
		DataSet ds = new DataSet();
		string procName = "EffortSizeList_Get";

		using (DataTable dt = new DataTable("EffortSize"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Add new Effort Size record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool EffortSize_Add(
		string Size
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

		string procName = "EffortSize_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@EffortSize", SqlDbType.NVarChar).Value = Size;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				try
				{
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
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Update specified Effort Size record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool EffortSize_Update(int EffortSizeID
		, string Size
		, string description
		, int sortOrder
		, bool archive
		, out bool duplicate
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		duplicate = false;
		bool saved = false;

		string procName = "EffortSize_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@EffortSizeID", SqlDbType.Int).Value = EffortSizeID;
				cmd.Parameters.Add("@EffortSize", SqlDbType.NVarChar).Value = Size;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
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
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Delete Effort Size record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool EffortSize_Delete(int EffortSizeID
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

		string procName = "EffortSize_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@EffortSizeID", SqlDbType.Int).Value = EffortSizeID;

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
						errorMsg = "Effort Size record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Effort Size record has dependencies and could not be permanently deleted. It has been archived instead.";
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

	#endregion Effort Size


	#region Effort Area-Size

	/// <summary>
	/// Load Effort Area-Size Items
	/// </summary>
	/// <returns>Datatable of Effort Area Items</returns>
	public static DataTable EffortArea_SizeList_Get(int effortAreaID = 0, int effortSizeID = 0, bool includeArchive = false)
	{
		DataSet ds = new DataSet();
		string procName = "EffortArea_SizeList_Get";

		using (DataTable dt = new DataTable("EffortArea_Size"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@EffortAreaID", SqlDbType.Int).Value = effortAreaID == 0 ? (object)DBNull.Value : effortAreaID;
					cmd.Parameters.Add("@EffortSizeID", SqlDbType.Int).Value = effortSizeID == 0 ? (object)DBNull.Value : effortSizeID;
					cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
	/// Add new Effort Area-Size record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool EffortArea_Size_Add(
		int effortAreaID
		, int effortSizeID
		, int minValue
		, int maxValue
		, string units
		, string description
		, int sortOrder
		, out bool exists
		, out int newID
		, out string errorMsg)
	{
		exists = false;
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "EffortArea_Size_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@EffortAreaID", SqlDbType.Int).Value = effortAreaID;
				cmd.Parameters.Add("@EffortSizeID", SqlDbType.Int).Value = effortSizeID;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@MinValue", SqlDbType.Int).Value = minValue;
				cmd.Parameters.Add("@MaxValue", SqlDbType.Int).Value = maxValue;
				cmd.Parameters.Add("@Unit", SqlDbType.NVarChar).Value = units;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

				try
				{
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
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Update specified Effort Area-Size record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool EffortArea_Size_Update(int effortArea_SizeID
		, int effortAreaID
		, int effortSizeID
		, int minValue
		, int maxValue
		, string units
		, string description
		, int sortOrder
		, bool archive
		, out bool duplicate
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		duplicate = false;
		bool saved = false;

		string procName = "EffortArea_Size_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@EffortArea_SizeID", SqlDbType.Int).Value = effortArea_SizeID;
				cmd.Parameters.Add("@EffortAreaID", SqlDbType.Int).Value = effortAreaID;
				cmd.Parameters.Add("@EffortSizeID", SqlDbType.Int).Value = effortSizeID;
				cmd.Parameters.Add("@MinValue", SqlDbType.Int).Value = minValue;
				cmd.Parameters.Add("@MaxValue", SqlDbType.Int).Value = maxValue;
				cmd.Parameters.Add("@Unit", SqlDbType.NVarChar).Value = units;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@Sort_Order", SqlDbType.Int).Value = sortOrder;
				cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
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
				catch (Exception ex)
				{
					saved = false;
					errorMsg = ex.Message;
					LogUtility.LogException(ex);
				}
			}
		}

		return saved;
	}

	/// <summary>
	/// Delete Effort Area-Size record
	///  - will archive if record is assigned to anything
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool EffortArea_Size_Delete(int EffortArea_SizeID
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

		string procName = "EffortArea_Size_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@EffortArea_SizeID", SqlDbType.Int).Value = EffortArea_SizeID;

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
						errorMsg = "Effort Area-Size record could not be found.";
						return false;
					}
				}
				SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
				if (paramHasDependencies != null)
				{
					bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
					if (hasDependencies)
					{
						errorMsg = "Effort Area-Size record has dependencies and could not be permanently deleted. It has been archived instead.";
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

    #endregion Effort Area-Size

    #region Image
    public static DataTable ImageList_Get(int includeArchive = 1)
    {
        DataSet ds = new DataSet();
        string procName = "ImageList_Get";

        using (DataTable dt = new DataTable("Image"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.NVarChar).Value = includeArchive;
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

    public static Dictionary<string, string> Image_Add(string ImageName, string Description, string FileName, byte[] FileData)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "Image_Add";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
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

    public static bool Image_Update(int ImageID
        , string imageName
        , string description
        , int sort
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "Image_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;
                cmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = imageName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool Image_Delete(int ImageID
        , out bool exists
        , out bool archived
        , out string errorMsg)
    {
        exists = false;
        archived = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "Image_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@archived", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        archived = false;
                        errorMsg = "Image record could not be found.";
                        return false;
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

    public static DataTable Image_Get(int ImageID)
    {
        string procName = "Image_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;

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
    #endregion

    #region Image - CONTRACT
    public static DataTable Image_CONTRACTList_Get(int imageID = 0)
    {
        DataSet ds = new DataSet();
        string procName = "Image_CONTRACTList_Get";

        using (DataTable dt = new DataTable("Image"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = imageID == 0 ? (object)DBNull.Value : imageID;
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
    public static bool Image_CONTRACT_Add(
        int ImageID
        , int ProductVersionID
        , int CONTRACTID
        , int WorkloadAllocationID
        , int sort
        , bool archive
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "Image_CONTRACT_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
                cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = CONTRACTID;
                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = WorkloadAllocationID == 0 ? (object)DBNull.Value : WorkloadAllocationID;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool Image_CONTRACT_Update(int Image_CONTRACTID
        , int ImageID
        , int ProductVersionID
        , int CONTRACTID
        , int WorkloadAllocationID
        , int sort
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "Image_CONTRACT_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Image_CONTRACTID", SqlDbType.Int).Value = Image_CONTRACTID;
                cmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
                cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = CONTRACTID;
                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = WorkloadAllocationID == 0 ? (object)DBNull.Value : WorkloadAllocationID;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool Image_CONTRACT_Delete(int Image_CONTRACTID
        , out bool exists
        , out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "Image_CONTRACT_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Image_CONTRACTID", SqlDbType.Int).Value = Image_CONTRACTID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        errorMsg = "Image_CONTRACT record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }
    #endregion

    #region Narrative
    public static DataTable NarrativeList_Get(int productVersionID = 0, int contractID = 0, bool includeArchive = false)
    {
        DataSet ds = new DataSet();
        string procName = "NarrativeList_Get";

        using (DataTable dt = new DataTable("Narrative"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = productVersionID == -1 ? (object)DBNull.Value : productVersionID;
                    cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID == 0 ? (object)DBNull.Value : contractID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;
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
    public static bool Narrative_Add(
        int releaseID, int contractID,
        int missionNarrativeID, string missionNarrative, int missionImageID,
        int programMGMTNarrativeID, string programMGMTNarrative, int programMGMTImageID,
        int deploymentNarrativeID, string deploymentNarrative, int deploymentImageID,
        int productionNarrativeID, string productionNarrative, int productionImageID
        , bool archive
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "Narrative_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = releaseID;
                cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                cmd.Parameters.Add("@MissionNarrativeID", SqlDbType.Int).Value = missionNarrativeID;
                cmd.Parameters.Add("@MissionNarrative", SqlDbType.NVarChar).Value = missionNarrative == "" ? (object)DBNull.Value : missionNarrative;
                cmd.Parameters.Add("@MissionImageID", SqlDbType.Int).Value = missionImageID == 0 ? (object)DBNull.Value : missionImageID;
                cmd.Parameters.Add("@ProgramMGMTNarrativeID", SqlDbType.Int).Value = programMGMTNarrativeID;
                cmd.Parameters.Add("@ProgramMGMTNarrative", SqlDbType.NVarChar).Value = programMGMTNarrative == "" ? (object)DBNull.Value : programMGMTNarrative;
                cmd.Parameters.Add("@ProgramMGMTImageID", SqlDbType.Int).Value = programMGMTImageID == 0 ? (object)DBNull.Value : programMGMTImageID;
                cmd.Parameters.Add("@DeploymentNarrativeID", SqlDbType.Int).Value = deploymentNarrativeID;
                cmd.Parameters.Add("@DeploymentNarrative", SqlDbType.NVarChar).Value = deploymentNarrative == "" ? (object)DBNull.Value : deploymentNarrative;
                cmd.Parameters.Add("@DeploymentImageID", SqlDbType.Int).Value = deploymentImageID == 0 ? (object)DBNull.Value : deploymentImageID;
                cmd.Parameters.Add("@ProductionNarrativeID", SqlDbType.Int).Value = productionNarrativeID;
                cmd.Parameters.Add("@ProductionNarrative", SqlDbType.NVarChar).Value = productionNarrative == "" ? (object)DBNull.Value : productionNarrative;
                cmd.Parameters.Add("@ProductionImageID", SqlDbType.Int).Value = productionImageID == 0 ? (object)DBNull.Value : productionImageID;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool Narrative_Copy(
        int oldProductVersionID
        , int newProductVersionID
        , string title
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "Narrative_Copy";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@OldProductVersionID", SqlDbType.Int).Value = oldProductVersionID;
                cmd.Parameters.Add("@NewProductVersionID", SqlDbType.Int).Value = newProductVersionID;
                cmd.Parameters.Add("@Narrative", SqlDbType.NVarChar).Value = title;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool Narrative_Title_Update(string titleOld
        , string title
        , int productVersionID
        , int sort
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "Narrative_Title_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NarrativeOld", SqlDbType.NVarChar).Value = titleOld;
                cmd.Parameters.Add("@Narrative", SqlDbType.NVarChar).Value = title;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = productVersionID;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool Narrative_Update(int NarrativeID
        , int productVersionID
        , int contractID
        , string narrative
        , int imageID
        , bool archive
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "Narrative_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NarrativeID", SqlDbType.Int).Value = NarrativeID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = productVersionID;
                cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = narrative;
                cmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = imageID;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool Narrative_Delete(int NarrativeID
        , out bool exists
        , out bool archived
        , out string errorMsg)
    {
        exists = false;
        archived = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "Narrative_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NarrativeID", SqlDbType.Int).Value = NarrativeID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@archived", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        archived = false;
                        errorMsg = "Narrative record could not be found.";
                        return false;
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
    #endregion

    #region Narrative - CONTRACT
    public static DataTable Narrative_ProductVersionList_Get(int contractID = 0, bool includeArchive = false)
    {
        DataSet ds = new DataSet();
        string procName = "Narrative_ProductVersionList_Get";

        using (DataTable dt = new DataTable("Narrative"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID == 0 ? (object)DBNull.Value : contractID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;
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

    public static DataTable Narrative_CONTRACTList_Get(int productVersionID = 0, int contractID = 0, bool includeArchive = false)
    {
        DataSet ds = new DataSet();
        string procName = "Narrative_CONTRACTList_Get";

        using (DataTable dt = new DataTable("Contract"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = productVersionID == 0 ? (object)DBNull.Value : productVersionID;
                    cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID == 0 ? (object)DBNull.Value : contractID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;
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
    public static bool Narrative_CONTRACT_Add(
        int NarrativeID
        , int ProductVersionID
        , int CONTRACTID
        , int WorkloadAllocationID
        , int ImageID
        , int sort
        , bool archive
        , out bool duplicateSort
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        duplicateSort = false;
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "Narrative_CONTRACT_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NarrativeID", SqlDbType.Int).Value = NarrativeID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
                cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = CONTRACTID;
                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = WorkloadAllocationID == 0 ? (object)DBNull.Value: WorkloadAllocationID;
                cmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID == 0 ? (object)DBNull.Value : ImageID;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@duplicateSort", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
                    cmd.ExecuteNonQuery();

                    SqlParameter paramDuplicateSort = cmd.Parameters["@duplicateSort"];
                    if (paramDuplicateSort != null)
                    {
                        bool.TryParse(paramDuplicateSort.Value.ToString(), out duplicateSort);
                        saved = false;
                    }
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool Narrative_CONTRACT_Update(int Narrative_CONTRACTID
        , int NarrativeID
        , int ProductVersionID
        , int CONTRACTID
        , int WorkloadAllocationID
        , int ImageID
        , int sort
        , bool archive
        , out bool duplicateSort
        , out bool duplicate
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        duplicateSort = false;
        duplicate = false;
        bool saved = false;

        string procName = "Narrative_CONTRACT_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Narrative_CONTRACTID", SqlDbType.Int).Value = Narrative_CONTRACTID;
                cmd.Parameters.Add("@NarrativeID", SqlDbType.Int).Value = NarrativeID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
                cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = CONTRACTID;
                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = WorkloadAllocationID == 0 ? (object)DBNull.Value : WorkloadAllocationID;
                cmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID == 0 ? (object)DBNull.Value : ImageID;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sort;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@duplicateSort", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
                    cmd.ExecuteNonQuery();

                    SqlParameter paramDuplicateSort = cmd.Parameters["@duplicateSort"];
                    if (paramDuplicateSort != null)
                    {
                        bool.TryParse(paramDuplicateSort.Value.ToString(), out duplicateSort);
                    }
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool Narrative_CONTRACT_Delete(int Narrative_CONTRACTID
        , out bool exists
        , out string errorMsg)
    {
        exists = false;
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "Narrative_CONTRACT_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Narrative_CONTRACTID", SqlDbType.Int).Value = Narrative_CONTRACTID;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramExists = cmd.Parameters["@exists"];
                if (paramExists != null)
                {
                    bool.TryParse(paramExists.Value.ToString(), out exists);
                    if (!exists)
                    {
                        errorMsg = "Narrative_CONTRACT record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramDeleted = cmd.Parameters["@deleted"];
                if (paramDeleted != null)
                {
                    bool.TryParse(paramDeleted.Value.ToString(), out deleted);
                }
            }
        }

        return deleted;
    }
    #endregion

    #region SR Type
    public static DataTable SRTypeList_Get(bool includeArchive = false)
    {
        string procName = "SRTypeList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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
    #endregion

    #region Workload Allocation
    public static DataTable WorkloadAllocationList_Get(int includeArchive = 0)
    {
        DataSet ds = new DataSet();
        string procName = "WorkloadAllocationList_Get";

        using (DataTable dt = new DataTable("WorkloadAllocation"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;
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

    public static bool WorkloadAllocation_Add(
        string workloadAllocation
        , string abbreviation
        , string description
        , int contractID
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

        string procName = "WorkloadAllocation_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkloadAllocation", SqlDbType.NVarChar).Value = workloadAllocation;
                cmd.Parameters.Add("@Abbreviation", SqlDbType.NVarChar).Value = abbreviation;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool WorkloadAllocation_Update(int WorkloadAllocationID
        , string WorkloadAllocation
        , string abbreviation
        , string description
        , int contractID
        , int sortOrder
        , bool archive
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WorkloadAllocation_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = WorkloadAllocationID;
                cmd.Parameters.Add("@WorkloadAllocation", SqlDbType.NVarChar).Value = WorkloadAllocation;
                cmd.Parameters.Add("@Abbreviation", SqlDbType.NVarChar).Value = abbreviation;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
                    cmd.ExecuteNonQuery();

                    SqlParameter paramSaved = cmd.Parameters["@saved"];
                    if (paramSaved != null)
                    {
                        bool.TryParse(paramSaved.Value.ToString(), out saved);
                    }
                }
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool WorkloadAllocation_Delete(int workloadAllocationID
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

        string procName = "WorkloadAllocation_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = workloadAllocationID;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
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
                        errorMsg = "Workload Allocation record could not be found.";
                        return false;
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

    #endregion

    #region Workload Allocation - Status
    public static DataTable WorkloadAllocation_StatusList_Get(int workloadAllocationID = 0, int includeArchive = 1)
    {
        DataSet ds = new DataSet();
        string procName = "WorkloadAllocation_StatusList_Get";

        using (DataTable dt = new DataTable("WorkloadAllocation_Status"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.NVarChar).Value = workloadAllocationID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.NVarChar).Value = includeArchive;
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

    public static bool WorkloadAllocation_Status_Add(
        int workloadAllocation
        , int status
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

        string procName = "WorkloadAllocation_Status_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.NVarChar).Value = workloadAllocation;
                cmd.Parameters.Add("@StatusID", SqlDbType.NVarChar).Value = status;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool WorkloadAllocation_Status_Update(int WorkloadAllocation_StatusID
        , int WorkloadAllocationID
        , int StatusID
        , int sortOrder
        , bool archive
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WorkloadAllocation_Status_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkloadAllocation_StatusID", SqlDbType.Int).Value = WorkloadAllocation_StatusID;
                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = WorkloadAllocationID;
                cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = StatusID;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
                    cmd.ExecuteNonQuery();

                    SqlParameter paramSaved = cmd.Parameters["@saved"];
                    if (paramSaved != null)
                    {
                        bool.TryParse(paramSaved.Value.ToString(), out saved);
                    }
                }
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool WorkloadAllocation_Status_Delete(int workloadAllocation_StatusID
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

        string procName = "WorkloadAllocation_Status_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkloadAllocation_StatusID", SqlDbType.Int).Value = workloadAllocation_StatusID;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
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
                        errorMsg = "Workload Allocation Status record could not be found.";
                        return false;
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
    #endregion

    #region Workload Allocation - Contract
    public static DataTable WorkloadAllocation_ContractList_Get(int workloadAllocationID = 0, int contractID = 0, int includeArchive = 1)
    {
        DataSet ds = new DataSet();
        string procName = "WorkloadAllocation_ContractList_Get";

        using (DataTable dt = new DataTable("WorkloadAllocation_Contract"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.NVarChar).Value = workloadAllocationID;
                    cmd.Parameters.Add("@ContractID", SqlDbType.NVarChar).Value = contractID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.NVarChar).Value = includeArchive;
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
    /// Add new System_Contract record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkloadAllocation_Contract_Add(
        int workloadAllocationID
        , int CONTRACTID
        , bool primary
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

        string procName = "WorkloadAllocation_Contract_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.NVarChar).Value = workloadAllocationID;
                cmd.Parameters.Add("@ContractID", SqlDbType.NVarChar).Value = CONTRACTID;
                cmd.Parameters.Add("@Primary", SqlDbType.Bit).Value = primary ? 1 : 0;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sortOrder;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    /// <summary>
    /// Update specified System_Contract record
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool WorkloadAllocation_Contract_Update(int WorkloadAllocation_ContractID
        , int WorkloadAllocationID
        , int CONTRACTID
        , bool primary
        , int sortOrder
        , bool archive
        , out string errorMsg)
    {
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "WorkloadAllocation_Contract_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkloadAllocation_ContractID", SqlDbType.Int).Value = WorkloadAllocation_ContractID;
                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = WorkloadAllocationID;
                cmd.Parameters.Add("@CONTRACTID", SqlDbType.Int).Value = CONTRACTID;
                cmd.Parameters.Add("@Primary", SqlDbType.Bit).Value = primary ? 1 : 0;
                cmd.Parameters.Add("@Sort", SqlDbType.Int).Value = sortOrder;      
                cmd.Parameters.Add("@Archive", SqlDbType.Bit).Value = archive ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
                    cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@saved"];
                if (paramSaved != null)
                {
                    bool.TryParse(paramSaved.Value.ToString(), out saved);
                }
                }
                catch (Exception ex)
                {
                    saved = false;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool WorkloadAllocation_Contract_Delete(int workloadAllocation_ContractID
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

        string procName = "WorkloadAllocation_Contract_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkloadAllocation_ContractID", SqlDbType.Int).Value = workloadAllocation_ContractID;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
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
                        errorMsg = "Workload Allocation Contract record could not be found.";
                        return false;
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
    #endregion

    #region "AOR Estimation"
    public static DataTable AOREstimation_Get(int aorReleaseId = 0, bool includeArchive = false)
    {
        DataSet ds = new DataSet();
        string procName = "AOREstimation_Get";

        using (DataTable dt = new DataTable("AOREstimation"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = aorReleaseId == 0 ? (object)DBNull.Value : aorReleaseId;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = includeArchive ? 1 : 0;

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

    public static bool AOREstimation_Add(int AorEstimationID
        , string AOREstimationName
        , string Descr
        , string Notes
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "AOREstimation_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AOREstimationID", SqlDbType.Int).Value = AorEstimationID;
                cmd.Parameters.Add("@AOREstimationName", SqlDbType.NVarChar).Value = AOREstimationName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Descr;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Descr;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool AOREstimation_Update(int AorEstimationID
        , string AOREstimationName
        , string Descr
        , string Notes
        , out bool duplicate
        , out string errorMsg
        )
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "AOREstimation_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AOREstimationID", SqlDbType.Int).Value = AorEstimationID;
                cmd.Parameters.Add("@AOREstimationName", SqlDbType.NVarChar).Value = AOREstimationName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Descr;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Descr;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.Parameters.Add("@duplicate", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                try
                {
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
                catch (Exception ex)
                {
                    saved = false;
                    errorMsg = ex.Message;
                    LogUtility.LogException(ex);
                }
            }
        }

        return saved;
    }

    public static bool AOREstimation_Delete(int aorEstimationId
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

        string procName = "AOREstimation_Delete";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AOREstimationID", SqlDbType.Int).Value = aorEstimationId;

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
                        errorMsg = "AOR Estimation record could not be found.";
                        return false;
                    }
                }
                SqlParameter paramHasDependencies = cmd.Parameters["@hasDependencies"];
                if (paramHasDependencies != null)
                {
                    bool.TryParse(paramHasDependencies.Value.ToString(), out hasDependencies);
                    if (hasDependencies)
                    {
                        errorMsg = "AOR Estimation record has dependencies and could not be permanently deleted. It has been archived instead.";
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
    #endregion

    #region CacheSupport
    public static object GetMasterDataProperty(string cacheType, string propertyType, string key)
    {
        object value = null;

        int id = 0;
        Int32.TryParse(key, out id); // some keys will be plain strings, but we do this up here as a helper for downstream code that expects ids or numbers

        if (cacheType == WTSCacheType.WTS_SYSTEM)
        {
            DataTable dt = MasterData.WTS_System_Get(id);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                if (propertyType == WTSCachePropertyType.NAME)
                {
                    value = row["WTS_System"].ToString();
                }
                else if (propertyType == WTSCachePropertyType.DATAROW)
                {
                    value = row;
                }
                else
                {
                    value = row[propertyType] != DBNull.Value ? row[propertyType].ToString() : null;
                }
            }
        }
        else if (cacheType == WTSCacheType.WORK_AREA)
        {
            DataTable dt = MasterData.WorkArea_Get(id);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                if (propertyType == WTSCachePropertyType.NAME)
                {
                    value = row["WorkArea"].ToString();
                }
                else if (propertyType == WTSCachePropertyType.DATAROW)
                {
                    value = row;
                }
                else
                {
                    value = row[propertyType] != DBNull.Value ? row[propertyType].ToString() : null;
                }
            }
        }
        else if (cacheType == WTSCacheType.SYSTEM_SUITE)
        {
            DataTable dt = MasterData.SystemSuiteList_Get();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int idFound = Convert.ToInt32(row["WTS_SYSTEM_SUITEID"]);

                    if (idFound == id)
                    {
                        if (propertyType == WTSCachePropertyType.NAME)
                        {
                            value = row["WTS_SYSTEM_SUITE"].ToString();
                        }
                        else if (propertyType == WTSCachePropertyType.DATAROW)
                        {
                            value = row;
                        }
                        else
                        {
                            value = row[propertyType] != DBNull.Value ? row[propertyType].ToString() : null;
                        }

                        break;
                    }
                }
            }
        }

        return value;
    }
    #endregion
}