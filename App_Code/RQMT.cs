﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web;
using System.Xml;

using WTS.Caching;
using WTS.Auditing;

public sealed class RQMT
{
    #region RQMT
    public static DataTable RQMT_Crosswalk_Multi_Level_Grid(
        XmlDocument level,
        XmlDocument filter,
        XmlDocument whereExts,
        string RQMTMode,
        XmlDocument CountColumns,
        string customWhere,
        bool ignoreUserFilters
        )
    {
        string procName = "RQMT_Crosswalk_Multi_Level_Grid";

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
                    cmd.Parameters.Add("@WhereExts", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(whereExts.InnerXml, XmlNodeType.Document, null));
                    cmd.Parameters.Add("@Debug", SqlDbType.Bit).Value = 0;
                    cmd.Parameters.Add("@RQMTMode", SqlDbType.VarChar).Value = string.IsNullOrWhiteSpace(RQMTMode) ? "all" : RQMTMode;
                    cmd.Parameters.Add("@CountColumns", SqlDbType.Xml).Value = CountColumns != null ? new SqlXml(new XmlTextReader(CountColumns.InnerXml, XmlNodeType.Document, null)) : null;
                    cmd.Parameters.Add("@CustomWhere", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(customWhere) ? (object)DBNull.Value : customWhere;
                    cmd.Parameters.Add("@IgnoreUserFilters", SqlDbType.Bit).Value = ignoreUserFilters;

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

    public static DataTable RQMTList_Get(int RQMTID = 0, string RQMT = null, bool includeAssociations = false)
    {
        string procName = "RQMTList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@RQMTID", SqlDbType.Int).Value = RQMTID;
                    cmd.Parameters.Add("@RQMT", SqlDbType.NVarChar).Value = RQMT != null ? RQMT : (object)DBNull.Value;
                    cmd.Parameters.Add("@IncludeAssociations", SqlDbType.Bit).Value = includeAssociations;

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

    public static int RQMT_GetRQMTSetRQMTSystemForSet(int RQMTID, int RQMTSetID)
    {
        int RQMTSet_RQMTSystemID = 0;

        DataTable dt = WTSData.GetDataTableFromStoredProcedure("RQMT_GetRQMTSetRQMTSystemForSet", null, new SqlParameter[] {
            new SqlParameter("@RQMTID", RQMTID),
            new SqlParameter("@RQMTSetID", RQMTSetID)
        });

        if (dt.Rows.Count > 0)
        {
            RQMTSet_RQMTSystemID = (int)dt.Rows[0]["RQMTSet_RQMTSystemID"];
        }

        return RQMTSet_RQMTSystemID;
    }

    public static Dictionary<string, string> RQMT_Delete(int RQMTID, bool ignoreDependencies)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result["success"] = "false";

        string procName = "RQMT_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTID", SqlDbType.Int).Value = RQMTID;
                cmd.Parameters.Add("@Exists", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@HasDependencies", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@IgnoreDependencies", SqlDbType.Bit).Value = ignoreDependencies;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@Deleted"];
                SqlParameter paramHasDependencies = cmd.Parameters["@HasDependencies"];

                bool deleted = false;
                if (paramDeleted != null) deleted = paramDeleted.Value.ToString() == "1" || paramDeleted.Value.ToString().ToLower() == "true";

                bool hasDependencies = false;
                if (paramHasDependencies != null) hasDependencies = paramHasDependencies.Value.ToString() == "1" || paramHasDependencies.Value.ToString().ToLower() == "true";

                result["deleted"] = deleted.ToString().ToLower();
                result["hasdependencies"] = hasDependencies.ToString().ToLower();
            }
        }

        return result;
    }

    public static bool RQMT_Update(XmlDocument Changes, System.Text.StringBuilder Output = null)
    {
        bool saved = false;
        string procName = "RQMT_Update";
        string stroutput = null;

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Changes", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Changes.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Output", SqlDbType.NVarChar, 10000).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                SqlParameter paramOutput = cmd.Parameters["@Output"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                if (paramOutput != null) stroutput = paramOutput.Value.ToString();
            }
        }

        if (Output != null && stroutput != null)
        {
            Output.Append(stroutput);
        }

        return saved;
    }

    public static Dictionary<string, string> RQMT_Save(bool NewRQMT, int RQMTID, string RQMT)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string newIDs = null;
        string procName = "RQMT_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NewRQMT", SqlDbType.Bit).Value = NewRQMT ? 1 : 0;
                cmd.Parameters.Add("@RQMTID", SqlDbType.Int).Value = RQMTID;
                cmd.Parameters.Add("@RQMT", SqlDbType.NVarChar).Value = RQMT;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@NewID", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@NewIDs", SqlDbType.NVarChar, 500).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                SqlParameter paramExists = cmd.Parameters["@Exists"];
                SqlParameter paramNewID = cmd.Parameters["@NewID"];
                SqlParameter paramNewIDs = cmd.Parameters["@NewIDs"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                if (paramExists != null) bool.TryParse(paramExists.Value.ToString(), out exists);
                if (paramNewID != null) int.TryParse(paramNewID.Value.ToString(), out newID);
                if (paramNewIDs != null) newIDs = paramNewIDs.Value.ToString();

                result["saved"] = saved.ToString();
                result["exists"] = exists.ToString();
                result["newID"] = newID.ToString();
                result["newIDs"] = string.IsNullOrWhiteSpace(newIDs) ? "" : newIDs.ToString();
            }
        }

        return result;
    }

    public static DataSet RQMTDefectsImpact_Get(int intRQMT_ID = 0, int intSYSTEM_ID = 0)
    {
        return WTSData.GetDataSetFromStoredProcedure("RQMTDefectsImpact_Get", new string[] { "Data", "SR", "Tasks" }, new SqlParameter[] {
            new SqlParameter("@RQMT_ID", intRQMT_ID),
            new SqlParameter("@SYSTEM_ID", intSYSTEM_ID)
        });
    }

    public static bool RQMTDefectsImpact_Save(int intRQMTID, int intSYSTEMID, int intRQMTSystemDefectID, string strDescription, int intVerified, int intResolved, int intContinueToReview, int intImpactID, int intRQMTStageID, string mitigation)
    {
        bool saved = false;
        string procName = "RQMTDefectsImpact_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMT_ID", SqlDbType.Int).Value = intRQMTID;
                cmd.Parameters.Add("@SYSTEM_ID", SqlDbType.Int).Value = intSYSTEMID;
                cmd.Parameters.Add("@RQMTSystemDefectID", SqlDbType.Int).Value = intRQMTSystemDefectID;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = strDescription;
                cmd.Parameters.Add("@Verified", SqlDbType.Int).Value = intVerified;
                cmd.Parameters.Add("@Resolved", SqlDbType.Int).Value = intResolved;
                cmd.Parameters.Add("@ContinueToReview", SqlDbType.Int).Value = intContinueToReview;
                cmd.Parameters.Add("@ImpactID", SqlDbType.Int).Value = intImpactID;
                cmd.Parameters.Add("@RQMTStageID", SqlDbType.Int).Value = intRQMTStageID;
                cmd.Parameters.Add("@Mitigation", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(mitigation) ? (object)DBNull.Value : mitigation;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
            }
        }

        return saved;
    }
    public static bool RQMTDefectsImpact_Delete(int intRQMTSystemDefectID)
    {
        bool deleted = false;
        string procName = "RQMTDefectsImpact_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTSystemDefectID", SqlDbType.Int).Value = intRQMTSystemDefectID;
                cmd.Parameters.Add("@Deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@DeletedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Deleted"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out deleted);
            }
        }

        return deleted;
    }

    public static bool RQMTDefectsImpactSR_Add(int RQMTSystemDefectID, int SRID, int AORSR_SRID)
    {
        return WTSData.ExecuteStoredProcedure("RQMTDefectsImpactSR_Add", new SqlParameter[] {
            new SqlParameter("@RQMTSystemDefectID", RQMTSystemDefectID),
            new SqlParameter("@SRID", SRID > 0 ? SRID : (object)DBNull.Value),
            new SqlParameter("@AORSR_SRID", AORSR_SRID > 0 ? AORSR_SRID : (object)DBNull.Value),
            new SqlParameter("@AddedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static bool RQMTDefectsImpactSR_Delete(int RQMTSystemDefectSRID)
    {
        return WTSData.ExecuteStoredProcedure("RQMTDefectsImpactSR_Delete", new SqlParameter[] {
            new SqlParameter("@RQMTSystemDefectSRID", RQMTSystemDefectSRID),
            new SqlParameter("@DeletedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static bool RQMTDefectsImpactTask_Add(int RQMTSystemDefectID, int WORKITEM_TASKID)
    {
        return WTSData.ExecuteStoredProcedure("RQMTDefectsImpactTask_Add", new SqlParameter[] {
            new SqlParameter("@RQMTSystemDefectID", RQMTSystemDefectID),
            new SqlParameter("@WORKITEM_TASKID", WORKITEM_TASKID),
            new SqlParameter("@AddedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static bool RQMTDefectsImpactTask_Delete(int RQMTSystemDefectTaskID)
    {
        return WTSData.ExecuteStoredProcedure("RQMTDefectsImpactTask_Delete", new SqlParameter[] {
            new SqlParameter("@RQMTSystemDefectTaskID", RQMTSystemDefectTaskID),
            new SqlParameter("@DeletedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static DataTable RQMTComplexityList_Get()
    {
        return WTSData.GetDataTableFromStoredProcedure("RQMTComplexityList_Get");
    }
    #endregion

    #region RQMT Description
    public static DataTable RQMTDescriptionList_Get()
    {
        string procName = "RQMTDescriptionList_Get";

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

    public static bool RQMTDescription_Delete(int RQMTDescriptionID)
    {
        bool deleted = false;
        string procName = "RQMTDescription_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTDescriptionID", SqlDbType.Int).Value = RQMTDescriptionID;
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

    public static DataTable RQMTDescriptionTypeList_Get()
    {
        string procName = "RQMTDescriptionTypeList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Bit).Value = 0;
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

    public static Dictionary<string, string> RQMTDescription_Save(int RQMTDescriptionTypeID, string RQMTDescription)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "RQMTDescription_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTDescriptionTypeID", SqlDbType.Int).Value = RQMTDescriptionTypeID;
                cmd.Parameters.Add("@RQMTDescription", SqlDbType.NVarChar).Value = RQMTDescription;
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

    public static DataTable RQMTBuilderDescriptionList_Get(int RQMTDescriptionID = 0, string RQMTDescription = null, bool includeArchive = false)
    {
        return WTSData.GetDataTableFromStoredProcedure("RQMTBuilderDescriptionList_Get", "Data", new SqlParameter[] {
            new SqlParameter("@RQMTDescriptionID", RQMTDescriptionID),
            new SqlParameter("@RQMTDescription", RQMTDescription != null ? RQMTDescription : (object)DBNull.Value),
            new SqlParameter("@IncludeArchive", includeArchive)
        });
    }

    public static bool RQMTDescriptionAttachment_Save(int RQMTDescriptionAttachmentID, int RQMTDescription_ID, int attachmentID, int attachmentTypeId, string fileName, string description, byte[] fileData, out int newAttachmentID)
    {
        newAttachmentID = 0;

        try
        {
            int newID = 0;

            string errors = null;
            WTSData.Attachment_Add(attachmentTypeId, fileName, "RQMTDESC ATT", description, fileData, 0, out newID, out errors);

            if (newID > 0)
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand("RQMTDescriptionAttachment_Save", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@RQMTDescriptionAttachmentID", SqlDbType.Int).Value = 0;
                        cmd.Parameters.Add("@RQMTDescriptionID", SqlDbType.Int).Value = RQMTDescription_ID;
                        cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = newID;

                        cmd.ExecuteNonQuery();

                        newAttachmentID = newID;
                    }
                }
            }
            else
            {
                LogUtility.LogError(errors);
                return false;
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            return false;
        }

        return true;
    }

    public static bool RQMTDescriptionAttachment_Delete(int RQMTDescriptionAttachmentID, int RQMTDescriptionID, int attachmentID)
    {
        return WTSData.ExecuteStoredProcedure("RQMTDescriptionAttachment_Delete", new SqlParameter[] {
            new SqlParameter("@RQMTDescriptionAttachmentID", RQMTDescriptionAttachmentID),
            new SqlParameter("@RQMTDescriptionID", RQMTDescriptionID),
            new SqlParameter("@AttachmentID", attachmentID)
        });
    }

    public static DataTable RQMTDescriptionAttachment_Get(int RQMTDescriptionAttachmentID, int RQMTDescriptionID, int attachmentID, bool includeData, int RQMTID)
    {
        return WTSData.GetDataTableFromStoredProcedure("RQMTDescriptionAttachment_Get", null, new SqlParameter[] {
            new SqlParameter("@RQMTDescriptionAttachmentID", RQMTDescriptionAttachmentID),
            new SqlParameter("@RQMTDescriptionID", RQMTDescriptionID),
            new SqlParameter("@AttachmentID", attachmentID),
            new SqlParameter("@IncludeData", includeData),
            new SqlParameter("@RQMTID", RQMTID)
        });
    }

    #endregion

    #region RQMTType
    public static DataTable RQMTTypeList_Get(string RQMTType = null)
    {
        return WTSData.GetDataTableFromStoredProcedure("RQMTTypeList_Get", "Data", new SqlParameter[] {
            new SqlParameter("@RQMTType", RQMTType != null ? RQMTType : (object)DBNull.Value)
        });       
    }
    #endregion

    #region RQMTSystem
    public static DataSet RQMTEditData_Get(int RQMTID)
    {
        return WTSData.GetDataSetFromStoredProcedure("RQMTEditData_Get", new string[] { "RQMT", "ALLSETS", "ASSOCIATIONS", "ATTRIBUTES", "USAGE", "AVAILABLEFUNCTIONALITIES", "FUNCTIONALITY", "DESCRIPTIONS", "DESCTYPES", "DEFECTS", "DESCATT" }, new SqlParameter[] {
            new SqlParameter("@RQMTID", RQMTID)
        });
    }

    public static int RQMTEditData_Save(int RQMTID, string RQMT, string addToSets, string deleteFromSets, string attrChanges, string usageChanges, string funcChanges, string descChanges, int ParentRQMTID)
    {
        SqlParameter newID = new SqlParameter("NewID", SqlDbType.Int);
        newID.Direction = ParameterDirection.Output;

        if (WTSData.ExecuteStoredProcedure("RQMTEditData_Save", new SqlParameter[] {
            new SqlParameter("@RQMTID", RQMTID),
            new SqlParameter("@RQMT", RQMT),
            new SqlParameter("@AddToSets", string.IsNullOrWhiteSpace(addToSets) ? (object)DBNull.Value : addToSets),
            new SqlParameter("@DeleteFromSets", string.IsNullOrWhiteSpace(deleteFromSets) ? (object)DBNull.Value : deleteFromSets),
            new SqlParameter("@AttrChanges", string.IsNullOrWhiteSpace(attrChanges) ? (object)DBNull.Value : attrChanges),
            new SqlParameter("@UsageChanges", string.IsNullOrWhiteSpace(usageChanges) ? (object)DBNull.Value : usageChanges),
            new SqlParameter("@FuncChanges", string.IsNullOrWhiteSpace(funcChanges) ? (object)DBNull.Value : funcChanges),
            new SqlParameter("@DescChanges", string.IsNullOrWhiteSpace(descChanges) ? (object)DBNull.Value : descChanges),
            new SqlParameter("@ParentRQMTID", ParentRQMTID),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name),
            newID
        }))
        {
            int savedID = RQMTID > 0 ? RQMTID : (int)newID.Value;

            return savedID;
        }

        return 0;
    }

    public static DataSet RQMTBuilder_Data_Get(int RQMTSetID, string RQMTSetName, int WTS_SYSTEMID, int WorkAreaID, int RQMTTypeID, int RQMTSet_RQMTSystemID)
    {
        // note: this query can bring back multiple rows for the same rqmt because each rqmt can have multiple descriptions and functionalities; the code viewing the data must account for the dups
        return WTSData.GetDataSetFromStoredProcedure("RQMTBuilder_Data_Get", new string[] { "RQMT", "DESC", "FUNC", "DEFECT", "RQMTSETTASK" }, new SqlParameter[] {
            new SqlParameter("@RQMTSetID", RQMTSetID),
            new SqlParameter("@RQMTSetName", RQMTSetName),
            new SqlParameter("@WTS_SYSTEMID", WTS_SYSTEMID),
            new SqlParameter("@WorkAreaID", WorkAreaID),
            new SqlParameter("@RQMTTypeID", RQMTTypeID),
            new SqlParameter("@RQMTSet_RQMTSystemID", RQMTSet_RQMTSystemID)
        });
    }

    public static int RQMTSet_Save(int RQMTSetID, string RQMTSetName, int WTS_SYSTEMID, int WorkAreaID, int RQMTTypeID, int RQMTComplexityID, string justification)
    {        
        SqlParameter paramRQMTSetID = new SqlParameter("@RQMTSetID", RQMTSetID);
        paramRQMTSetID.Direction = ParameterDirection.InputOutput;

        if (WTSData.ExecuteStoredProcedure("RQMTSet_Save", new SqlParameter[] {
            paramRQMTSetID,
            new SqlParameter("@RQMTSetName", RQMTSetName),
            new SqlParameter("@WTS_SYSTEMID", WTS_SYSTEMID),
            new SqlParameter("@WorkAreaID", WorkAreaID),
            new SqlParameter("@RQMTTypeID", RQMTTypeID),
            new SqlParameter("@RQMTComplexityID", RQMTComplexityID),
            new SqlParameter("@Justification", string.IsNullOrWhiteSpace(justification) ? (object)DBNull.Value : justification),
            new SqlParameter("@CreatedBy", HttpContext.Current.User.Identity.Name),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name),
        }))
        {
            RQMTSetID = (int)paramRQMTSetID.Value;
        }

        return RQMTSetID;
    }

    public static int RQMTSet_AddRQMT(int RQMTSetID, int RQMTID, string RQMT, bool addAsChild, int childBasedOnRQMTSet_RQMTSystemID, string pasteOptions)
    {
        SqlParameter paramRQMTID = new SqlParameter("@RQMTID", RQMTID);
        paramRQMTID.Direction = ParameterDirection.InputOutput;

        if (WTSData.ExecuteStoredProcedure("RQMTSet_AddRQMT", new SqlParameter[] {
            new SqlParameter("@RQMTSetID", RQMTSetID),
            paramRQMTID,
            new SqlParameter("@RQMT", string.IsNullOrWhiteSpace(RQMT) ? (object)DBNull.Value : RQMT),
            new SqlParameter("@AddAsChild", addAsChild),
            new SqlParameter("@ChildBasedOnRQMTSet_RQMTSystemID", childBasedOnRQMTSet_RQMTSystemID), // this parameter overrides the AddAsChild parameter, and bases the child indent setting on the specified RSRS's setting
            new SqlParameter("@PasteOptions", string.IsNullOrWhiteSpace(pasteOptions) ? (object)DBNull.Value : pasteOptions), // when the @ChildBasedOnRQMTSet_RQMTSystemID is filled out, this option specifies whether we are copying attributes, defects, and descriptsion from source rqmtsytems (if different than the target rqmtsystem)
            new SqlParameter("@CreatedBy", HttpContext.Current.User.Identity.Name),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name)            
        }))
        {
            return paramRQMTID.Value != DBNull.Value ? (int)paramRQMTID.Value : 0;
        }

        return 0;
    }

    public static DataTable RQMT_AssociatedSets_Get(int RQMTID)
    {
        return WTSData.GetDataTableFromStoredProcedure("RQMT_AssociatedSets_Get", "Data", new SqlParameter[] {
            new SqlParameter("@RQMTID", RQMTID)
        });
    }

    public static int RQMTBuilder_RQMTUpdate(int RQMTID, string RQMT, string addToSets, string deleteFromSets)
    {
        SqlParameter paramExistingID = new SqlParameter("@ExistingID", SqlDbType.Int);
        paramExistingID.Direction = ParameterDirection.Output;

        if (WTSData.ExecuteStoredProcedure("RQMTBuilder_RQMTUpdate", new SqlParameter[] {
            new SqlParameter("@RQMTID", RQMTID),
            new SqlParameter("@RQMT", RQMT),
            new SqlParameter("@AddToSets", string.IsNullOrWhiteSpace(addToSets) ? (object)DBNull.Value : addToSets),
            new SqlParameter("@DeleteFromSets", string.IsNullOrWhiteSpace(deleteFromSets) ? (object)DBNull.Value : deleteFromSets),
            new SqlParameter("@AddedBy", HttpContext.Current.User.Identity.Name),
            paramExistingID
        }))
        {
            int eid = paramExistingID.Value != DBNull.Value && (int)paramExistingID.Value != 0 ? (int)paramExistingID.Value : RQMTID;

            if (eid != RQMTID)
            {
                return eid;
            }
        }

        return RQMTID;
    }

    public static DataRow RQMTSet_Get(int RQMTSetID, bool addRQMTCount = false)
    {
        DataRow row = null;

        DataTable dt = RQMTSetList_Get(RQMTSetID, addRQMTCount);

        if (dt != null && dt.Rows.Count == 1)
        {
            row = dt.Rows[0];
        }

        return row;
    }

    public static DataTable RQMTSetList_Get(int RQMTSetID = 0, bool addRQMTCount = false)
    {
        return WTSData.GetDataTableFromStoredProcedure("RQMTSetList_Get", null, new SqlParameter[] {
            new SqlParameter("@RQMTSetID", RQMTSetID),
            new SqlParameter("@AddRQMTCount", addRQMTCount)
        });
    }

    public static bool RQMTSet_DeleteRQMT(int RQMTSetID, int RQMTID, int RQMTSet_RQMTSystemID, bool reorderAfterDelete)
    {
        return WTSData.ExecuteStoredProcedure("RQMTSet_DeleteRQMT", new SqlParameter[] {
            new SqlParameter("@RQMTSetID", RQMTSetID),
            new SqlParameter("@RQMTID", RQMTID),
            new SqlParameter("@RQMTSet_RQMTSystemID", RQMTSet_RQMTSystemID),
            new SqlParameter("@ReorderAfterDelete", reorderAfterDelete),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static bool RQMTSet_Delete(int RQMTSetID)
    {
        return WTSData.ExecuteStoredProcedure("RQMTSet_Delete", new SqlParameter[] {
            new SqlParameter("@RQMTSetID", RQMTSetID),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static DataTable RQMTSetName_Get(int RQMTSetNameID = 0)
    {
        return WTSData.GetDataTableFromStoredProcedure("RQMTSetName_Get");
    }

    public static bool RQMTSet_Task_Add(int RQMTSetID, int WORKITEM_TASKID)
    {
        return WTSData.ExecuteStoredProcedure("RQMTSet_Task_Add", new SqlParameter[] {
            new SqlParameter("@RQMTSetID", RQMTSetID),
            new SqlParameter("@WORKITEM_TASKID", WORKITEM_TASKID),
            new SqlParameter("@AddedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static bool RQMTSet_Task_Delete(int RQMTSetID, int RQMTSetTaskID)
    {
        return WTSData.ExecuteStoredProcedure("RQMTSet_Task_Delete", new SqlParameter[] {
            new SqlParameter("@RQMTSetID", RQMTSetID),
            new SqlParameter("@RQMTSetTaskID", RQMTSetTaskID),
            new SqlParameter("@DeletedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static bool RQMTSetName_Save(int RQMTSetNameID, string RQMTSetName)
    {
        SqlParameter paramExists = new SqlParameter("@Exists", 1);
        paramExists.Direction = ParameterDirection.InputOutput;

        WTSData.ExecuteStoredProcedure("RQMTSetName_Save", new SqlParameter[] {
            new SqlParameter("@RQMTSetNameID", RQMTSetNameID),
            new SqlParameter("@RQMTSetName", RQMTSetName),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name),
            paramExists
        });

        return (int)paramExists.Value == 0;
    }

    public static bool RQMTSet_ReorderRQMTs(int RQMTSetID, string order)
    {
        return WTSData.ExecuteStoredProcedure("RQMTSet_ReorderRQMTs", new SqlParameter[] {
            new SqlParameter("@RQMTSetID", RQMTSetID),
            new SqlParameter("@Order", string.IsNullOrWhiteSpace(order) ? (object)DBNull.Value : order),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static Dictionary<string, string> RQMTSystem_Save(XmlDocument RQMTSystemXMLDoc)
    {
        // NOTE: I DON'T THINK THIS FUNCTION IS USED ANY MORE

        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false;

        string procName = "RQMTSystem_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@RQMTSystemXML", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(RQMTSystemXMLDoc.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@RQMTSystemIDMappings", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                result["saved"] = saved.ToString();

                SqlParameter paramRQMTSystemIDMappings = cmd.Parameters["@RQMTSystemIDMappings"];
                result["rqmtsystemidmappings"] = paramRQMTSystemIDMappings.Value.ToString();
            }
        }

        return result;
    }

    public static int RQMTSystem_SaveDescription(int RQMTSystemID, int RQMTSet_RQMTSystemID, int RQMTSystemRQMTDescriptionID, string RQMTDescription, int RQMTDescriptionTypeID, bool editMode, string changeMode)
    {
        SqlParameter paramRQMTDescriptionID = new SqlParameter("@RQMTDescriptionID", -1);
        paramRQMTDescriptionID.Direction = ParameterDirection.InputOutput;

        if (WTSData.ExecuteStoredProcedure("RQMTSystem_SaveDescription", new SqlParameter[] {
                new SqlParameter("@RQMTSystemID", RQMTSystemID),
                new SqlParameter("@RQMTSet_RQMTSystemID", RQMTSet_RQMTSystemID),
                new SqlParameter("@RQMTSystemRQMTDescriptionID", RQMTSystemRQMTDescriptionID),
                paramRQMTDescriptionID,
                new SqlParameter("@RQMTDescription", RQMTDescription != null ? RQMTDescription : (object)DBNull.Value),
                new SqlParameter("@RQMTDescriptionTypeID", RQMTDescriptionTypeID),
                new SqlParameter("@Edit", editMode),
                new SqlParameter("@ChangeMode", changeMode),
                new SqlParameter("@CreatedBy", HttpContext.Current.User.Identity.Name),
                new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name)
            }))
        {
            return paramRQMTDescriptionID.Value != DBNull.Value ? (int)paramRQMTDescriptionID.Value : 0;
        }
        else
        {
            return 0;
        }
    }

    public static bool RQMTSystem_DeleteDescription(int RQMTSystemRQMTDescriptionID)
    {
        return WTSData.ExecuteStoredProcedure("RQMTSystem_DeleteDescription", new SqlParameter[] {
            new SqlParameter("@RQMTSystemRQMTDescriptionID", RQMTSystemRQMTDescriptionID),
            new SqlParameter("@DeleteOrphanedDescription", true),
            new SqlParameter("@DeletedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    #endregion

    #region "RQMT Attribute"
    public static DataTable RQMTAttribute_Get(int RQMTAttributeTypeID = 0)
    {
        return WTSData.GetDataTableFromStoredProcedure("RQMTAttribute_Get", "Data", new SqlParameter[] {
            new SqlParameter("@RQMTAttributeTypeID", RQMTAttributeTypeID)
        });
    }

    public static bool RQMTSystemAttributes_Save(int RQMTSet_RQMTSystemID, int RQMTStageID, int CriticalityID, int RQMTStatusID, bool RQMTAccepted)
    {
        return WTSData.ExecuteStoredProcedure("RQMTSystemAttributes_Save", new SqlParameter[] {
            new SqlParameter("@RQMTSet_RQMTSystemID", RQMTSet_RQMTSystemID),
            new SqlParameter("@RQMTStageID", RQMTStageID != 0 ? RQMTStageID : (object)DBNull.Value),
            new SqlParameter("@CriticalityID", CriticalityID != 0 ? CriticalityID : (object)DBNull.Value),
            new SqlParameter("@RQMTStatusID", RQMTStatusID != 0 ? RQMTStatusID : (object)DBNull.Value),
            new SqlParameter("@RQMTAccepted", RQMTAccepted),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name)
        });
    }
    #endregion

    #region RQMT_Functionality
    public static bool RQMTFunctionality_Save(int RQMTSetID, int RQMTSet_RQMTSystemID, string RQMTFunctionalities, int RQMTSetFunctionalityID, int FunctionalityID, int ComplexityID, string Justification)
    {
        return WTSData.ExecuteStoredProcedure("RQMTFunctionality_Save", new SqlParameter[] {
            new SqlParameter("@RQMTSetID", RQMTSetID),
            new SqlParameter("@RQMTSet_RQMTSystemID", RQMTSet_RQMTSystemID),
            new SqlParameter("@RQMTFunctionalities", string.IsNullOrWhiteSpace(RQMTFunctionalities) ? (object)DBNull.Value : RQMTFunctionalities),
            new SqlParameter("@RQMTSetFunctionalityID", RQMTSetFunctionalityID),
            new SqlParameter("@FunctionalityID", FunctionalityID),
            new SqlParameter("@RQMTComplexityID", ComplexityID),
            new SqlParameter("@Justification", string.IsNullOrWhiteSpace(Justification) ? (object)DBNull.Value : Justification),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static bool RQMTFunctionality_Delete(int RQMTSetID, int RQMTSet_RQMTSystemID, int RQMTSetFunctionalityID)
    {
        return WTSData.ExecuteStoredProcedure("RQMTFunctionality_Delete", new SqlParameter[] {
            new SqlParameter("@RQMTSetID", RQMTSetID),
            new SqlParameter("@RQMTSet_RQMTSystemID", RQMTSet_RQMTSystemID),
            new SqlParameter("@RQMTSetFunctionalityID", RQMTSetFunctionalityID),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    public static bool RQMTSet_RQMTSystem_Usage_Update(int RQMTSet_RQMTSystemID, int month, bool selected)
    {
        return WTSData.ExecuteStoredProcedure("RQMTSet_RQMTSystem_Usage_Update", new SqlParameter[] {
            new SqlParameter("@RQMTSet_RQMTSystemID", RQMTSet_RQMTSystemID),
            new SqlParameter("@Month", month),
            new SqlParameter("@Selected", selected),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name)
        });
    }

    #endregion

    #region CacheSupport
    public static object GetRQMTDataProperty(string cacheType, string propertyType, string key)
    {
        object value = null;

        int id = 0;
        Int32.TryParse(key, out id); // some keys will be plain strings, but we do this up here as a helper for downstream code that expects ids or numbers

        if (cacheType == WTSCacheType.RQMT_TYPE)
        {
            DataTable dt = MasterData.RQMT_TypeList_Get(true);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int idFound = Convert.ToInt32(row["RQMTTypeID"]);

                    if (idFound == id)
                    {
                        if (propertyType == WTSCachePropertyType.NAME)
                        {
                            value = row["RQMTType"].ToString();
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
        else if (cacheType == WTSCacheType.RQMT_COMPLEXITY)
        {
            DataTable dt = RQMT.RQMTComplexityList_Get();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int idFound = Convert.ToInt32(row["RQMTComplexityID"]);

                    if (idFound == id)
                    {
                        if (key == WTSCachePropertyType.NAME)
                        {
                            value = row["RQMTComplexity"].ToString();
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