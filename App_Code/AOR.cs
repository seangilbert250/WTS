﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Linq;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

using Newtonsoft.Json;

using WTS;
using WTS.Enums;
using WTS.Events;
using WTS.Util;

public sealed class AOR
{
    #region AOR
    public static DataTable AORCurrentRelease_Get()
    {
        string procName = "AORCurrentRelease_Get";

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

    public static bool AORCurrentRelease_Save(int ProductVersionID)
    {
        bool saved = false;
        string procName = "AORCurrentRelease_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID == 0 ? (object)DBNull.Value : ProductVersionID;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
            }
        }

        return saved;
    }

    public static DataTable AOR_Crosswalk_Multi_Level_Grid(
        XmlDocument level,
        XmlDocument filter,
        string qfRelease = "",
        string qfAORType = "",
        string qfVisibleToCustomer = "",
        string qfContainsTasks = "",
        string qfContract = "",
        string qfTaskStatus = "",
        string qfAORProductionStatus = "",
        string qfShowArchiveAOR = "0",
        string AORID_Filter_arr = "",
        bool getColumns = true)
    {
        string procName = "AOR_Crosswalk_Multi_Level_Grid";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@Level", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(level.InnerXml, XmlNodeType.Document, null));
                    cmd.Parameters.Add("@Filter", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(filter.InnerXml, XmlNodeType.Document, null));
                    cmd.Parameters.Add("@QFRelease", SqlDbType.NVarChar).Value = qfRelease;
                    cmd.Parameters.Add("@QFAORType", SqlDbType.NVarChar).Value = qfAORType;
                    cmd.Parameters.Add("@QFVisibleToCustomer", SqlDbType.NVarChar).Value = qfVisibleToCustomer;
                    cmd.Parameters.Add("@QFContainstasks", SqlDbType.NVarChar).Value = qfContainsTasks;
                    cmd.Parameters.Add("@QFContract", SqlDbType.NVarChar).Value = qfContract;
                    cmd.Parameters.Add("@QFTaskStatus", SqlDbType.NVarChar).Value = qfTaskStatus;
                    cmd.Parameters.Add("@QFAORProductionStatus", SqlDbType.NVarChar).Value = qfAORProductionStatus;
                    cmd.Parameters.Add("@QFShowArchiveAOR", SqlDbType.NVarChar).Value = qfShowArchiveAOR;
                    cmd.Parameters.Add("@AORID_Filter_arr", SqlDbType.NVarChar).Value = AORID_Filter_arr;
                    cmd.Parameters.Add("@GetColumns", SqlDbType.NVarChar).Value = getColumns ? "1" : "0";
                    cmd.Parameters.Add("@Debug", SqlDbType.Bit).Value = 0;
                    cmd.CommandTimeout = 0;

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

    public static DataTable AORList_Get(int AORID = 0, int includeArchive = 0)
    {
        string procName = "AORList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;
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

    public static DataTable AORRelease_History_Get(string aorReleaseID = ""
       , string itemUpdateType = ""
       , string fieldChanged = ""
       , string createdBy = "")
    {
        string procName = "AORRelease_History_Get";

        using (DataTable dt = new DataTable("AORReleaseHistory"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.NVarChar).Value = aorReleaseID;
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

    public static bool AOR_Delete(int AORID)
    {
        bool deleted = false;
        string procName = "AOR_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
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

    public static bool AOR_Update(XmlDocument Changes)
    {
        bool saved = false;
        string procName = "AOR_Update";

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

    public static Dictionary<string, string> AOR_Save(bool NewAOR, int AORID, string AORName, string Description, string Notes, int Approved, int CodingEffortID, int TestingEffortID, int TrainingSupportEffortID,
        int StagePriorityID, int ProductVersionID, int WorkloadAllocationID, int TierID, int RankID, int IP1StatusID, int IP2StatusID, int IP3StatusID,
        string ROI, int CMMIStatusID, int CyberID, string CyberNarrative, int CriticalPathAORTeamID, int AORWorkTypeID, int CascadeAOR, int AORCustomerFlagship,
        int InvestigationStatusID, int TechnicalStatusID, int CustomerDesignStatusID, int CodingStatusID, int InternalTestingStatusID, int CustomerValidationTestingStatusID, int AdoptionStatusID, 
        int StopLightStatusID, int AORStatusID, int AORRequiresPD2TDR,
        int CriticalityID, int CustomerValueID, int RiskID, int LevelOfEffortID, int HoursToFix, int CyberISMT, DateTime PlannedStart, DateTime PlannedEnd,
        int AORReleaseID, 
        XmlDocument Estimations,
        decimal AORENetResources,
        int AOREOverrideRiskID, string AOREOverrideJustification, int AOREOverrideSignOff,
        XmlDocument Systems)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        DateTime nDate = new DateTime();
        string procName = "AOR_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NewAOR", SqlDbType.Bit).Value = NewAOR ? 1 : 0;
                cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                cmd.Parameters.Add("@AORName", SqlDbType.NVarChar).Value = AORName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Notes;
                cmd.Parameters.Add("@Approved", SqlDbType.Bit).Value = Approved;
                cmd.Parameters.Add("@CodingEffortID", SqlDbType.Int).Value = CodingEffortID == 0 ? (object)DBNull.Value : CodingEffortID;
                cmd.Parameters.Add("@TestingEffortID", SqlDbType.Int).Value = TestingEffortID == 0 ? (object)DBNull.Value : TestingEffortID;
                cmd.Parameters.Add("@TrainingSupportEffortID", SqlDbType.Int).Value = TrainingSupportEffortID == 0 ? (object)DBNull.Value : TrainingSupportEffortID;
                cmd.Parameters.Add("@StagePriorityID", SqlDbType.Int).Value = StagePriorityID == 0 ? (object)DBNull.Value : StagePriorityID;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID == 0 ? (object)DBNull.Value : ProductVersionID;
                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = WorkloadAllocationID == 0 ? (object)DBNull.Value : WorkloadAllocationID;
                cmd.Parameters.Add("@TierID", SqlDbType.Int).Value = TierID == 0 ? (object)DBNull.Value : TierID;
                cmd.Parameters.Add("@RankID", SqlDbType.Int).Value = RankID == -999 ? (object)DBNull.Value : RankID;
                cmd.Parameters.Add("@IP1StatusID", SqlDbType.Int).Value = IP1StatusID == 0 ? (object)DBNull.Value : IP1StatusID;
                cmd.Parameters.Add("@IP2StatusID", SqlDbType.Int).Value = IP2StatusID == 0 ? (object)DBNull.Value : IP2StatusID;
                cmd.Parameters.Add("@IP3StatusID", SqlDbType.Int).Value = IP3StatusID == 0 ? (object)DBNull.Value : IP3StatusID;
                cmd.Parameters.Add("@ROI", SqlDbType.NVarChar).Value = ROI;
                cmd.Parameters.Add("@CMMIStatusID", SqlDbType.Int).Value = CMMIStatusID == 0 ? (object)DBNull.Value : CMMIStatusID;
                cmd.Parameters.Add("@CyberID", SqlDbType.Int).Value = CyberID == -1 ? (object)DBNull.Value : CyberID;
                cmd.Parameters.Add("@CyberNarrative", SqlDbType.NVarChar).Value = CyberNarrative;
                cmd.Parameters.Add("@CriticalPathAORTeamID", SqlDbType.Int).Value = CriticalPathAORTeamID == 0 ? (object)DBNull.Value : CriticalPathAORTeamID;
                cmd.Parameters.Add("@AORWorkTypeID", SqlDbType.Int).Value = AORWorkTypeID == 0 ? (object)DBNull.Value : AORWorkTypeID;
                cmd.Parameters.Add("@CascadeAOR", SqlDbType.Bit).Value = CascadeAOR;
                cmd.Parameters.Add("@AORCustomerFlagship", SqlDbType.Bit).Value = AORCustomerFlagship;
                cmd.Parameters.Add("@InvestigationStatusID", SqlDbType.Int).Value = InvestigationStatusID == 0 ? (object)DBNull.Value : InvestigationStatusID;
                cmd.Parameters.Add("@TechnicalStatusID", SqlDbType.Int).Value = TechnicalStatusID == 0 ? (object)DBNull.Value : TechnicalStatusID;
                cmd.Parameters.Add("@CustomerDesignStatusID", SqlDbType.Int).Value = CustomerDesignStatusID == 0 ? (object)DBNull.Value : CustomerDesignStatusID;
                cmd.Parameters.Add("@CodingStatusID", SqlDbType.Int).Value = CodingStatusID == 0 ? (object)DBNull.Value : CodingStatusID;
                cmd.Parameters.Add("@InternalTestingStatusID", SqlDbType.Int).Value = InternalTestingStatusID == 0 ? (object)DBNull.Value : InternalTestingStatusID;
                cmd.Parameters.Add("@CustomerValidationTestingStatusID", SqlDbType.Int).Value = CustomerValidationTestingStatusID == 0 ? (object)DBNull.Value : CustomerValidationTestingStatusID;
                cmd.Parameters.Add("@AdoptionStatusID", SqlDbType.Int).Value = AdoptionStatusID == 0 ? (object)DBNull.Value : AdoptionStatusID;
                cmd.Parameters.Add("@StopLightStatusID", SqlDbType.Int).Value = StopLightStatusID == 0 ? (object)DBNull.Value : StopLightStatusID;
                cmd.Parameters.Add("@AORStatusID", SqlDbType.Int).Value = AORStatusID == 0 ? (object)DBNull.Value : AORStatusID;
                cmd.Parameters.Add("@AORRequiresPD2TDR", SqlDbType.Int).Value = AORRequiresPD2TDR;
                cmd.Parameters.Add("@CriticalityID", SqlDbType.Int).Value = CriticalityID == 0 ? (object)DBNull.Value : CriticalityID;
                cmd.Parameters.Add("@CustomerValueID", SqlDbType.Int).Value = CustomerValueID == 0 ? (object)DBNull.Value : CustomerValueID;
                cmd.Parameters.Add("@RiskID", SqlDbType.Int).Value = RiskID == 0 ? (object)DBNull.Value : RiskID;
                cmd.Parameters.Add("@LevelOfEffortID", SqlDbType.Int).Value = LevelOfEffortID == 0 ? (object)DBNull.Value : LevelOfEffortID;
                cmd.Parameters.Add("@HoursToFix", SqlDbType.Int).Value = HoursToFix == -999 ? (object)DBNull.Value : HoursToFix;
                cmd.Parameters.Add("@CyberISMT", SqlDbType.Bit).Value = CyberISMT;
                cmd.Parameters.Add("@PlannedStart", SqlDbType.DateTime).Value = PlannedStart == nDate ? (object)DBNull.Value : PlannedStart;
                cmd.Parameters.Add("@PlannedEnd", SqlDbType.DateTime).Value = PlannedEnd == nDate ? (object)DBNull.Value : PlannedEnd;
                cmd.Parameters.Add("@AORE_AORReleaseID", SqlDbType.Int).Value = AORReleaseID == 0 ? (object)DBNull.Value : AORReleaseID;
                cmd.Parameters.Add("@Estimations", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Estimations.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@AORENetResources", SqlDbType.Decimal).Value = AORENetResources == 0 ? (object)DBNull.Value : AORENetResources;
                cmd.Parameters.Add("@AORE_OverrideRiskID", SqlDbType.Int).Value = AOREOverrideRiskID == 0 ? (object)DBNull.Value : AOREOverrideRiskID;
                cmd.Parameters.Add("@AORE_OverrideJustification", SqlDbType.NVarChar).Value = AOREOverrideJustification;
                cmd.Parameters.Add("@AORE_OverrideSignOff", SqlDbType.Bit).Value = AOREOverrideSignOff;
                
                cmd.Parameters.Add("@Systems", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Systems.InnerXml, XmlNodeType.Document, null));
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

    public static Dictionary<string, string> AOR_Resource_Save(int AORID, XmlDocument Resources, XmlDocument ActionTeam)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "AOR_Resource_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                cmd.Parameters.Add("@Resources", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Resources.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@ActionTeam", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(ActionTeam.InnerXml, XmlNodeType.Document, null));
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

    public static Dictionary<string, string> AOR_Resource_Sync(int AORReleaseID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "AOR_Resource_Sync";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@newID", SqlDbType.Int).Direction = ParameterDirection.Output;

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

    public static DataTable AORSystemList_Get(int AORID = 0, int AORReleaseID = 0)
    {
        string procName = "AORSystemList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;

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

    public static DataTable AORResourceList_Get(int AORID = 0, int AORReleaseID = 0)
    {
        string procName = "AORResourceList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;

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

    public static DataTable AORResourceAllocationList_Get(int WTS_ResourceID = 0, string ReleaseIDs = "")
    {
        string procName = "AORResourceAllocationList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = WTS_ResourceID;
                    cmd.Parameters.Add("@ReleaseIDs", SqlDbType.VarChar).Value = ReleaseIDs;

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
    public static bool AORResourceAllocation_Save(XmlDocument Changes)
    {
        bool saved = false;
        string procName = "AORResourceAllocation_Save";

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

    public static DataTable AORRoleList_Get()
    {
        string procName = "AORRoleList_Get";

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

    public static DataTable AORTeamList_Get()
    {
        string procName = "AORTeamList_Get";

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

    public static DataTable AORWorkTypeList_Get()
    {
        string procName = "AORWorkTypeList_Get";

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

    public static DataTable AORNoteTypeList_Get()
    {
        string procName = "AORNoteTypeList_Get";

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

    public static DataTable AORAttachmentList_Get(int AORID = 0, int AORReleaseID = 0, int AORAttachmentTypeID = 0)
    {
        string procName = "AORAttachmentList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                    cmd.Parameters.Add("@AORAttachmentTypeID", SqlDbType.Int).Value = AORAttachmentTypeID;

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

    public static bool AORAttachment_Delete(int AORReleaseAttachmentID)
    {
        bool deleted = false;
        string procName = "AORAttachment_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORReleaseAttachmentID", SqlDbType.Int).Value = AORReleaseAttachmentID;
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

    public static bool AORAttachment_Update(XmlDocument Changes)
    {
        bool saved = false;
        string procName = "AORAttachment_Update";

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

    public static DataTable AORAttachment_Get(int AORReleaseAttachmentID)
    {
        string procName = "AORAttachment_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORReleaseAttachmentID", SqlDbType.Int).Value = AORReleaseAttachmentID;

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

    public static Dictionary<string, string> AORAttachmentApprove_Save(int AORReleaseAttachmentID, bool Approve)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "approved", "" }, { "approvedBy", "" }, { "approvedDate", "" }, { "error", "" } };
        bool saved = false, exists = false;
        bool approved = false;
        string approvedBy = "";
        DateTime approvedDate = new DateTime();
        string procName = "AORAttachmentApprove_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORReleaseAttachmentID", SqlDbType.Int).Value = AORReleaseAttachmentID;
                cmd.Parameters.Add("@Approve", SqlDbType.Bit).Value = Approve ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 50).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Approved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ApprovedBy", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ApprovedDate", SqlDbType.DateTime).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                SqlParameter paramExists = cmd.Parameters["@Exists"];
                SqlParameter paramApproved = cmd.Parameters["@Approved"];
                SqlParameter paramApprovedBy = cmd.Parameters["@ApprovedBy"];
                SqlParameter paramApprovedDate = cmd.Parameters["@ApprovedDate"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                if (paramExists != null) bool.TryParse(paramExists.Value.ToString(), out exists);
                if (paramApproved != null) bool.TryParse(paramApproved.Value.ToString(), out approved);
                if (paramApprovedBy != null) approvedBy = paramApprovedBy.Value.ToString();
                if (paramApprovedDate != null) DateTime.TryParse(paramApprovedDate.Value.ToString(), out approvedDate);

                result["saved"] = saved.ToString();
                result["exists"] = exists.ToString();
                result["approved"] = approved.ToString();
                result["approvedBy"] = approvedBy;
                result["approvedDate"] = String.Format("{0:M/d/yyyy h:mm tt}", approvedDate);
            }
        }

        return result;
    }

    public static DataTable AORAddList_Get(int AORID, int AORReleaseID, int SRID, int CRID, int DeliverableID, string Type, dynamic Filters, string CRStatus = "", string CRContract = "", string TaskID = "",int AORWorkTypeID = 0, string QFSystem = "", string QFRelease = "", string QFName = "", string QFDeployment = "", Boolean GetColumns = false)
    {
        string procName = "AORAddList_Get";
        SqlParameter[] sps = Filtering.GetWorkFilter_SqlParamsArray(Filters, "");

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                    cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = SRID;
                    cmd.Parameters.Add("@CRID", SqlDbType.Int).Value = CRID;
                    cmd.Parameters.Add("@DeliverableID", SqlDbType.Int).Value = DeliverableID;
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
                    cmd.Parameters.Add("@CRStatus", SqlDbType.NVarChar).Value = CRStatus;
                    cmd.Parameters.Add("@CRContract", SqlDbType.NVarChar).Value = CRContract;
                    cmd.Parameters.Add("@TaskID", SqlDbType.NVarChar).Value = TaskID;
                    cmd.Parameters.Add("@AORWorkTypeID", SqlDbType.Int).Value = AORWorkTypeID;
                    cmd.Parameters.Add("@QFSystem", SqlDbType.NVarChar).Value = QFSystem;
                    cmd.Parameters.Add("@QFRelease", SqlDbType.NVarChar).Value = QFRelease;
                    cmd.Parameters.Add("@QFName", SqlDbType.NVarChar).Value = QFName;
                    cmd.Parameters.Add("@QFDeployment", SqlDbType.NVarChar).Value = QFDeployment;
                    cmd.Parameters.Add("@GetColumns", SqlDbType.Bit).Value = GetColumns;

                    if (Filters != null && sps != null && sps.Length > 0) cmd.Parameters.AddRange(sps);

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

    public static Dictionary<string, string> AORAttachment_Save(int AORID, int AORAttachmentTypeID, string AORReleaseAttachmentName, string FileName, string Description, byte[] FileData)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "AORAttachment_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                cmd.Parameters.Add("@AORAttachmentTypeID", SqlDbType.Int).Value = AORAttachmentTypeID;
                cmd.Parameters.Add("@AORReleaseAttachmentName", SqlDbType.NVarChar).Value = AORReleaseAttachmentName;
                cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = FileName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
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

    public static DataTable AOR_CR_Crosswalk_Multi_Level_Grid(
        XmlDocument level,
        XmlDocument filter,
        int AORID = 0,
        int AORReleaseID = 0,
        int CRID = 0,
        string CRRelatedRel = "",
        string CRStatus= "",
        string SRStatus = "",
        string CRContract = "",
        string QFName = "")
    {
        string procName = "AOR_CR_Crosswalk_Multi_Level_Grid";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@Level", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(level.InnerXml, XmlNodeType.Document, null));
                    cmd.Parameters.Add("@Filter", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(filter.InnerXml, XmlNodeType.Document, null));
                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                    cmd.Parameters.Add("@CRID", SqlDbType.Int).Value = CRID;
                    cmd.Parameters.Add("@CRRelatedRel", SqlDbType.NVarChar).Value = CRRelatedRel;
                    cmd.Parameters.Add("@CRStatus", SqlDbType.NVarChar).Value = CRStatus;
                    cmd.Parameters.Add("@SRStatus", SqlDbType.NVarChar).Value = SRStatus;
                    cmd.Parameters.Add("@CRContract", SqlDbType.NVarChar).Value = CRContract;
                    cmd.Parameters.Add("QFName", SqlDbType.NVarChar).Value = QFName != null ? QFName : (object)DBNull.Value;
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

    public static DataTable AORCRList_Get(int AORID = 0, int AORReleaseID = 0, int CRID = 0)
    {
        string procName = "AORCRList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                    cmd.Parameters.Add("@CRID", SqlDbType.Int).Value = CRID;

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

    public static DataTable AORCRList_Search(int PrimarySR = 0)
    {
        string procName = "AORCRList_Search";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@PrimarySR", SqlDbType.Int).Value = PrimarySR;

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

    public static bool AORCR_Delete(int AORReleaseCRID)
    {
        bool deleted = false;
        string procName = "AORCR_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORReleaseCRID", SqlDbType.Int).Value = AORReleaseCRID;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
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

    public static DataTable AORTaskList_Get(int AORID = 0, int AORReleaseID = 0)
    {
        string procName = "AORTaskList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;

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

    public static bool AORTask_Delete(int AORReleaseTaskID, bool ReleaseAOR)
    {
        bool deleted = false;
        string procName = "AORTask_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORReleaseTaskID", SqlDbType.Int).Value = AORReleaseTaskID;
                cmd.Parameters.Add("@ReleaseAOR", SqlDbType.Bit).Value = ReleaseAOR ? 1 : 0;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
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

    public static bool AORSubTask_Delete(int AORReleaseSubTaskID)
    {
        bool deleted = false;
        string procName = "AORSubTask_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORReleaseSubTaskID", SqlDbType.Int).Value = AORReleaseSubTaskID;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
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

    public static int AORReleaseTask_Get(int WorkItemID)
    {
        int AORReleaseTaskID = 0;
        string procName = "AORReleaseTask_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@WorkItemID", SqlDbType.Int).Value = WorkItemID;
                cmd.Parameters.Add("@AORReleaseTaskID", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramAORReleaseTaskID = cmd.Parameters["@AORReleaseTaskID"];

                if (paramAORReleaseTaskID != null) int.TryParse(paramAORReleaseTaskID.Value.ToString(), out AORReleaseTaskID);
            }
        }

        return AORReleaseTaskID;
    }

    public static DataTable AORResourceTeamList_Get(int AORID = 0, int AORReleaseID = 0)
    {
        string procName = "AORResourceTeamList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;

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

    public static bool AORResourceTeam_Delete(int AORReleaseResourceTeamID)
    {
        bool deleted = false;
        string procName = "AORResourceTeam_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORReleaseResourceTeamID", SqlDbType.Int).Value = AORReleaseResourceTeamID;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
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

    public static bool AORAdd_Save(int AORID, int SRID, int CRID, int DeliverableID, string Type, XmlDocument Additions)
    {
        bool saved = false;
        string procName = "AORAdd_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = SRID;
                cmd.Parameters.Add("@CRID", SqlDbType.Int).Value = CRID;
                cmd.Parameters.Add("@DeliverableID", SqlDbType.Int).Value = DeliverableID;
                cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
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

    public static bool AORDeliverable_Delete(int AORReleaseDeliverableID)
    {
        bool deleted = false;
        string procName = "AORDeliverable_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORReleaseDeliverableID", SqlDbType.Int).Value = AORReleaseDeliverableID;
                cmd.Parameters.Add("@Exists", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@Deleted"];

                if (paramDeleted != null) bool.TryParse(paramDeleted.Value.ToString(), out deleted);
            }
        }

        return deleted;
    }

    public static DataTable AORDeliverableList_Get(int DeliverableID = 0)
    {
        string procName = "AORDeliverableList_Get";

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

    public static DataTable AORTaskReleaseHistoryList_Get(int AORID, int TaskID)
    {
        string procName = "AORTaskReleaseHistoryList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = TaskID;

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

    public static DataTable AORTaskHistoryList_Get(int AORID = 0, int TaskID = 0, int ReleaseFilterID = 0, string FieldChangedFilter = "")
    {
        string procName = "AORTaskHistoryList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = TaskID;
                    cmd.Parameters.Add("@ReleaseFilterID", SqlDbType.Int).Value = ReleaseFilterID;
                    cmd.Parameters.Add("@FieldChangedFilter", SqlDbType.VarChar).Value = FieldChangedFilter;

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

    public static DataSet AOROptionsList_Get(int AORID = 0, int TaskID = 0, int AORMeetingID = 0, int AORMeetingInstanceID = 0)
    {
        DataSet ds = new DataSet();
        string procName = "AOROptionsList_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = TaskID;
                cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "CR Status");
                da.TableMappings.Add("Table1", "CR Contract");
                da.TableMappings.Add("Table2", "Release");
                da.TableMappings.Add("Table3", "Field Changed");
                da.TableMappings.Add("Table4", "Note Type");
                da.TableMappings.Add("Table5", "SR Status");
                da.TableMappings.Add("Table6", "System Task Contract");
                da.TableMappings.Add("Table7", "CR Related Release");
                da.TableMappings.Add("Table8", "AOR Production Status");

                da.Fill(ds);
            }
        }

        return ds;
    }

    public static DataTable AORAlertList_Get(int AORID = 0, int AORReleaseID = 0)
    {
        string procName = "AORAlertList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;

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

    public static DataSet AORSummaryList_Get(string AlertType = "", int AORID = 0, int AORReleaseID = 0, string ReleaseIDs = "", string DeliverableIDs = "", string ContractIDs = "")
    {
        DataSet ds = new DataSet();
        string procName = "AORSummaryList_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AlertType", SqlDbType.VarChar).Value = AlertType;
                cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                cmd.Parameters.Add("@ReleaseIDs", SqlDbType.VarChar).Value = ReleaseIDs;
                cmd.Parameters.Add("@DeliverableIDs", SqlDbType.VarChar).Value = DeliverableIDs;
                cmd.Parameters.Add("@ContractIDs", SqlDbType.VarChar).Value = ContractIDs;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "CR");
                da.TableMappings.Add("Table1", "WorkloadAllocation");
                da.TableMappings.Add("Table2", "Alert");

                da.Fill(ds);
            }
        }

        return ds;
    }

    public static DataTable AORSummarySuiteList_Get(int productVersion = 0, int deployment = 0, int contract = 0, int includeArchive = 1)
    {
        DataSet ds = new DataSet();
        string procName = "AORSummarySuiteList_Get";

        using (DataTable dt = new DataTable("SystemSuite"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProductVersion", SqlDbType.Int).Value = productVersion;
                    cmd.Parameters.Add("@Deliverable", SqlDbType.Int).Value = deployment;
                    cmd.Parameters.Add("@Contract", SqlDbType.Int).Value = contract;
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

    public static Dictionary<string, string> AORWizard_Save(int AORID, string AORName, string Description, int ProductVersionID, int WorkloadAllocationID, int AORWorkTypeID,
        XmlDocument Systems, XmlDocument Resources, XmlDocument CRs, XmlDocument Tasks)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "AORWizard_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                cmd.Parameters.Add("@AORName", SqlDbType.NVarChar).Value = AORName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
                cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID == 0 ? (object)DBNull.Value : ProductVersionID;
                cmd.Parameters.Add("@WorkloadAllocationID", SqlDbType.Int).Value = WorkloadAllocationID == 0 ? (object)DBNull.Value : WorkloadAllocationID;
                cmd.Parameters.Add("@AORWorkTypeID", SqlDbType.Int).Value = AORWorkTypeID == 0 ? (object)DBNull.Value : AORWorkTypeID;
                cmd.Parameters.Add("@Systems", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Systems.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@Resources", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Resources.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@CRs", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(CRs.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@Tasks", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Tasks.InnerXml, XmlNodeType.Document, null));
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

    public static int AORMassChange_Save(string entityType
        , string fieldName
        , string existingValue
        , string newValue
        , string entityFilter
        , out string errorMsg)
    {
        int rowsUpdated = 0;
        errorMsg = string.Empty;

        string procName = "AORMassChange_Save";
        
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@EntityType", SqlDbType.NVarChar).Value = entityType;
                cmd.Parameters.Add("@FieldName", SqlDbType.NVarChar).Value = fieldName;
                cmd.Parameters.Add("@ExistingValue", SqlDbType.NVarChar).Value = existingValue;
                cmd.Parameters.Add("@NewValue", SqlDbType.NVarChar).Value = newValue;
                cmd.Parameters.Add("@EntityFilter", SqlDbType.NVarChar).Value = entityFilter;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
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

    public static DataSet AORReleaseBuilderList_Get(int CurrentReleaseID)
    {
        DataSet ds = new DataSet();
        string procName = "AORReleaseBuilderList_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@CurrentReleaseID", SqlDbType.Int).Value = CurrentReleaseID;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "AORTask");
                da.TableMappings.Add("Table1", "AORCR");

                da.Fill(ds);
            }
        }

        return ds;
    }

    public static bool AORReleaseBuilder_Save(int CurrentReleaseID, int NewReleaseID, string AssignedToRankIDs, XmlDocument Additions)
    {
        bool saved = false;
        string procName = "AORReleaseBuilder_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@CurrentReleaseID", SqlDbType.Int).Value = CurrentReleaseID;
                cmd.Parameters.Add("@NewReleaseID", SqlDbType.Int).Value = NewReleaseID;
                cmd.Parameters.Add("@AssignedToRankIDs", SqlDbType.NVarChar).Value = AssignedToRankIDs;
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

    public static bool AORReleaseCurrent(int AORID, int AORReleaseID)
    {
        bool Current = false;

        try
        {
            DataTable dt = AOR.AORList_Get(AORID: AORID);

            if (AORReleaseID == 0)
            {
                Current = true;
            }
            else
            {
                Current = (from row in dt.AsEnumerable()
                           where row.Field<int>("AORRelease_ID") == AORReleaseID
                           select row.Field<bool>("Current_ID")).FirstOrDefault();
            }
        }
        catch (Exception) { }

        return Current;
    }

    public static DataSet AORPD2TDRStatus_Get(int AORReleaseID)
    {
        DataSet ds = new DataSet();
        string procName = "AORPD2TDRStatus_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "PD2TDRPhase");
                da.TableMappings.Add("Table1", "WorkActivity");

                da.Fill(ds);
            }
        }

        return ds;
    }

    public static string GetAORContracts(int AORReleaseID)
    {
        string contracts = "";

        string funcName = "SELECT dbo.AOR_Get_Contracts(@AORReleaseID)";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(funcName, cn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@AORReleaseID", AORReleaseID);

                try
                {
                    contracts  = cmd.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                { //log exception
                    LogUtility.LogException(ex);
                }
            }
        }

        return contracts;
    }

    public static Dictionary<string, string> AORReleaseDeliverable_Save(int AORReleaseDeliverableID, int Weight)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "AORReleaseDeliverable_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORReleaseDeliverableID", SqlDbType.Int).Value = AORReleaseDeliverableID;
                cmd.Parameters.Add("@Weight", SqlDbType.Int).Value = Weight == -999 ? (object)DBNull.Value : Weight;
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
    #endregion

    #region AOR Meeting
    public static DataTable AOR_Meeting_Crosswalk_Multi_Level_Grid(
        XmlDocument level,
        XmlDocument filter,
        int AORID = 0,
        int AORReleaseID = 0)
    {
        string procName = "AOR_Meeting_Crosswalk_Multi_Level_Grid";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@Level", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(level.InnerXml, XmlNodeType.Document, null));
                    cmd.Parameters.Add("@Filter", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(filter.InnerXml, XmlNodeType.Document, null));
                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
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

    public static DataTable AORMeetingList_Get(int AORMeetingID = 0, int AORID = 0, int AORReleaseID = 0)
    {
        string procName = "AORMeetingList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;

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

    public static bool AORMeeting_Delete(int AORMeetingID)
    {
        bool deleted = false;
        string procName = "AORMeeting_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
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

    public static bool AORMeeting_Update(XmlDocument Changes)
    {
        bool saved = false;
        string procName = "AORMeeting_Update";

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

    public static Dictionary<string, string> AORMeeting_Save(bool NewAORMeeting, int AORMeetingID, string AORMeetingName, string Description, string Notes, int AORFrequencyID, int AutoCreateMeetings, int PrivateMeeting)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "AORMeeting_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NewAORMeeting", SqlDbType.Bit).Value = NewAORMeeting ? 1 : 0;
                cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                cmd.Parameters.Add("@AORMeetingName", SqlDbType.NVarChar).Value = AORMeetingName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Notes;
                cmd.Parameters.Add("@AORFrequencyID", SqlDbType.Int).Value = AORFrequencyID == 0 ? (object)DBNull.Value : AORFrequencyID;
                cmd.Parameters.Add("@AutoCreateMeetings", SqlDbType.Bit).Value = AutoCreateMeetings;
                cmd.Parameters.Add("@PrivateMeeting", SqlDbType.Bit).Value = PrivateMeeting;
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

    public static DataTable AORFrequencyList_Get()
    {
        string procName = "AORFrequencyList_Get";

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

    public static Dictionary<string, string> AORMeetingEmail_Add(int AORMeetingID, int WTS_RESOURCEID, string WTS_RESOURCEIDS = "")
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "error", "" } };

        string procName = "AORMeetingEmail_Add";

        try
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = WTS_RESOURCEID;
                    cmd.Parameters.Add("@WTS_RESOURCEIDS", SqlDbType.VarChar).Value = WTS_RESOURCEIDS;

                    cmd.ExecuteNonQuery();

                    result["saved"] = "true";
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            throw;
        }

        return result;
    }

    public static bool AORMeetingEmail_Delete(int AORMeetingID, int WTS_RESOURCEID = 0)
    {
        bool deleted = false;
        string procName = "AORMeetingEmail_Delete";

        try
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = WTS_RESOURCEID;

                    cmd.ExecuteNonQuery();

                    deleted = true;
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            throw;
        }

        return deleted;
    }

    #endregion

    #region AOR Meeting Instance
    public static bool AORMeetingInstanceLocked(int AORMeetingInstanceID)
    {
        string funcName = "SELECT dbo.AORMeetingInstanceLocked(@AORMeetingInstanceID)";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(funcName, cn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@AORMeetingInstanceID", AORMeetingInstanceID);

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

    public static bool AORMeetingInstance_ToggleMeetingLock(int AORMeetingInstanceID, bool locked, string unlockReason, int unlockedBy) {
        return WTSData.ExecuteStoredProcedure("AORMeetingInstance_ToggleMeetingLock",
            new SqlParameter[] {
                new SqlParameter("@AORMeetingInstanceID", AORMeetingInstanceID),
                new SqlParameter("@Locked", locked),
                new SqlParameter("@UnlockReason", unlockReason),
                new SqlParameter("@WTS_RESOURCEID", unlockedBy)
            }
        );
    }

    public static int AORMeetingInstance_CheckForDateConflict(int AORMeetingID, int AORMeetingInstanceID, DateTime instanceDate)
    {
        int conflict = 0;

        string procName = "AORMeetingInstance_CheckForDateConflict";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                cmd.Parameters.Add("@InstanceDate", SqlDbType.DateTime).Value = instanceDate;
                cmd.Parameters.Add("@Conflict", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramConflict = cmd.Parameters["@Conflict"];

                if (paramConflict != null) Int32.TryParse(paramConflict.Value.ToString(), out conflict);
            }
        }

        return conflict;
    }

    public static DataTable AORMeetingInstanceNotesDetail_GetByAgendaKey(int AORMeetingID, int AORMeetingInstanceID, string key)
    {
        string procName = "AORMeetingInstanceNotesDetail_GetByAgendaKey";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@AgendaKey", SqlDbType.VarChar).Value = key;

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

    public static DataTable AORMeetingInstanceList_Get(int AORMeetingID, int AORMeetingInstanceID = 0, int InstanceFilterID = 0)
    {
        string procName = "AORMeetingInstanceList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@InstanceFilterID", SqlDbType.Int).Value = InstanceFilterID;

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

    public static DataTable AORMeetingInstanceNoteGroupDetailList_Get(int NoteGroupID, int AORMeetingInstanceIDCutoff)
    {
        string procName = "AORMeetingInstanceNoteGroupDetailList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@NoteGroupID", SqlDbType.Int).Value = NoteGroupID;
                    cmd.Parameters.Add("@AORMeetingInstanceIDCutoff", SqlDbType.Int).Value = AORMeetingInstanceIDCutoff;

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

    public static bool AORMeetingInstance_Delete(int AORMeetingInstanceID)
    {
        bool deleted = false;
        string procName = "AORMeetingInstance_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
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

    public static bool AORMeetingInstance_Update(XmlDocument Changes)
    {
        bool saved = false;
        string procName = "AORMeetingInstance_Update";

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

    public static Dictionary<string, string> AORMeetingInstance_Save(bool NewAORMeetingInstance, int AORMeetingID, int AORMeetingInstanceID, string AORMeetingInstanceName, DateTime InstanceDate,
        string Notes, int ActualLength, XmlDocument Resources, XmlDocument MeetingNotes, XmlDocument NoteDetails, bool EndMeeting, bool LockMeeting, bool MeetingAccepted)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        string procName = "AORMeetingInstance_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@NewAORMeetingInstance", SqlDbType.Bit).Value = NewAORMeetingInstance ? 1 : 0;
                cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                cmd.Parameters.Add("@AORMeetingInstanceName", SqlDbType.NVarChar).Value = AORMeetingInstanceName;
                cmd.Parameters.Add("@InstanceDate", SqlDbType.DateTime).Value = InstanceDate;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Notes;
                cmd.Parameters.Add("@ActualLength", SqlDbType.Int).Value = ActualLength == -999 ? (object)DBNull.Value : ActualLength;
                cmd.Parameters.Add("@Resources", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Resources.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@MeetingNotes", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(MeetingNotes.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@NoteDetails", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(NoteDetails.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Exists", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@NewID", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@MeetingEnded", SqlDbType.Bit).Value = EndMeeting;
                cmd.Parameters.Add("@MeetingLocked", SqlDbType.Bit).Value = LockMeeting;
                cmd.Parameters.Add("@MeetingAccepted", SqlDbType.Bit).Value = MeetingAccepted;

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

    public static DataTable AORMeetingInstanceAORList_Get(int AORMeetingID, int AORMeetingInstanceID, bool ShowRemoved = false)
    {
        string procName = "AORMeetingInstanceAORList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@ShowRemoved", SqlDbType.Bit).Value = ShowRemoved ? 1 : 0;

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
    
    public static DataTable AORMeetingInstanceAOR_NotesList_Get(int AORMeetingID, int AORMeetingInstanceID, bool ShowRemoved = false)
    {
        string procName = "AORMeetingInstanceAOR_NotesList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@ShowRemoved", SqlDbType.Bit).Value = ShowRemoved ? 1 : 0;

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
    public static Dictionary<string, string> AORMeetingInstanceAOR_Toggle(int AORMeetingID, int AORMeetingInstanceID, int AORReleaseID, int Opt)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "error", "" } };
        bool saved = false, exists = false;
        string procName = "AORMeetingInstanceAOR_Toggle";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                cmd.Parameters.Add("@Opt", SqlDbType.Int).Value = Opt;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Exists", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                SqlParameter paramExists = cmd.Parameters["@Exists"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                if (paramExists != null) bool.TryParse(paramExists.Value.ToString(), out exists);

                result["saved"] = saved.ToString();
                result["exists"] = exists.ToString();
            }
        }

        return result;
    }

    public static int AORMeetingInstanceAOR_Add(int AORMeetingID, int AORMeetingInstanceID, int AORID)
    {
        int AORReleaseID = 0;

        string procName = "AORMeetingInstanceAOR_Add";

        try
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AddedBy", SqlDbType.VarChar).Value = HttpContext.Current == null || string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? "WTS" : HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    SqlParameter paramAORReleaseID = cmd.Parameters["@AORReleaseID"];
                    if (paramAORReleaseID != null) int.TryParse(paramAORReleaseID.Value.ToString(), out AORReleaseID);
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            throw ex;
        }

        return AORReleaseID;
    }

    public static DataTable AORMeetingInstanceSRList_Get(int AORMeetingID, int AORMeetingInstanceID, int AORReleaseID, bool ShowClosed = false)
    {
        string procName = "AORMeetingInstanceSRList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                    cmd.Parameters.Add("@ShowClosed", SqlDbType.Bit).Value = ShowClosed ? 1 : 0;

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

    public static DataTable AORMeetingInstanceTaskList_Get(int AORMeetingID, int AORMeetingInstanceID, int AORReleaseID, bool ShowClosed = false)
    {
        string procName = "AORMeetingInstanceTaskList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                    cmd.Parameters.Add("@ShowClosed", SqlDbType.Bit).Value = ShowClosed ? 1 : 0;

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

    public static DataTable AORMeetingInstanceResourceList_Get(int AORMeetingID, int AORMeetingInstanceID, bool ShowRemoved = false)
    {
        string procName = "AORMeetingInstanceResourceList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@ShowRemoved", SqlDbType.Bit).Value = ShowRemoved ? 1 : 0;

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

    public static Dictionary<string, string> AORMeetingInstanceResource_Toggle(int AORMeetingID, int AORMeetingInstanceID, int WTS_RESOURCEID, int Opt)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "error", "" } };
        bool saved = false, exists = false;
        string procName = "AORMeetingInstanceResource_Toggle";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.Int).Value = WTS_RESOURCEID;
                cmd.Parameters.Add("@Opt", SqlDbType.Int).Value = Opt;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Exists", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                SqlParameter paramExists = cmd.Parameters["@Exists"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                if (paramExists != null) bool.TryParse(paramExists.Value.ToString(), out exists);

                result["saved"] = saved.ToString();
                result["exists"] = exists.ToString();
            }
        }

        return result;
    }

    public static bool AORMeetingInstanceResource_Update(int AORMeetingID, int AORMeetingInstanceID, int WTS_RESOURCEID, bool attended, string reasonForAttending)
    {
        return WTSData.ExecuteStoredProcedure("AORMeetingInstanceResource_Update",
            new SqlParameter("@AORMeetingID", AORMeetingID),
            new SqlParameter("@AORMeetingInstanceID", AORMeetingInstanceID),
            new SqlParameter("@WTS_RESOURCEID", WTS_RESOURCEID),
            new SqlParameter("@Attended", attended),
            new SqlParameter("@ReasonForAttending", string.IsNullOrWhiteSpace(reasonForAttending) ? (object)DBNull.Value : reasonForAttending),
            new SqlParameter("@CreatedBy", HttpContext.Current.User.Identity.Name),
            new SqlParameter("@UpdatedBy", HttpContext.Current.User.Identity.Name));
    }

    public static string AORMeetingInstanceAcceptMeeting_Update(int AORMeetingInstanceID, bool accept)
    {
        string procName = "AORMeetingInstanceAcceptMeeting_Update";

        try
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@Accept", SqlDbType.Bit).Value = accept;
                    cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = DateTime.Now;

                    cmd.ExecuteNonQuery();                    
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            return ex.Message;
        }

        return null;
    }

    public static bool AORMeetingInstance_HasMeetingChangedSinceLastMeetingMinutes(int AORMeetingInstanceID)
    {
        SqlParameter p1 = new SqlParameter("@AORMeetingInstanceID", AORMeetingInstanceID);
        SqlParameter p2 = new SqlParameter("@MeetingChanged", SqlDbType.Bit);
        p2.Direction = ParameterDirection.Output;

        if (WTSData.ExecuteStoredProcedure("AORMeetingInstance_HasMeetingChangedSinceLastMeetingMinutes", new SqlParameter[] { p1, p2 }))
        {
            return (bool)p2.Value;
        }

        return false;
    }
        
    public static bool AORMeetingInstance_HasPreviousMeetingBeenAccepted(int AORMeetingID, int AORMeetingInstanceID, out int previousMeetingInstanceID, out DateTime previousMeetingDate)
    {
        bool accepted = false;
        previousMeetingDate = DateTime.MinValue;
        previousMeetingInstanceID = 0;

        SqlParameter p1 = new SqlParameter("@AORMeetingID", AORMeetingID);
        SqlParameter p2 = new SqlParameter("@AORMeetingInstanceID", AORMeetingInstanceID);
        SqlParameter p3 = new SqlParameter("@Accepted", SqlDbType.Bit);
        p3.Direction = ParameterDirection.Output;
        SqlParameter p4 = new SqlParameter("@LastMeetingInstanceID", SqlDbType.Int);
        p4.Direction = ParameterDirection.Output;
        SqlParameter p5 = new SqlParameter("@LastMeetingDate", SqlDbType.DateTime);
        p5.Direction = ParameterDirection.Output;


        if (WTSData.ExecuteStoredProcedure("AORMeetingInstance_HasPreviousMeetingBeenAccepted", new SqlParameter[] { p1, p2, p3, p4, p5 }))
        {
            if (p3.Value != DBNull.Value)
            {
                accepted = (bool)p3.Value;
            }

            if (p4.Value != DBNull.Value)
            {
                previousMeetingInstanceID = (int)p4.Value;
            }

            if (p5.Value != DBNull.Value)
            {
                previousMeetingDate = (DateTime)p5.Value;
            }            
        }

        return accepted;
    }

    public static DataSet AORMeetingInstanceMetrics_Get(int AORMeetingID, int AORMeetingInstanceID)
    {
        DataSet ds = new DataSet();
        string procName = "AORMeetingInstanceMetrics_Get";

        try
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "MeetingMetrics");
                    da.TableMappings.Add("Table1", "NoteMetrics");
                    da.TableMappings.Add("Table2", "LastMeetingMetrics");
                    da.TableMappings.Add("Table3", "SecondToLastMeetingMetrics");
                    da.TableMappings.Add("Table4", "LastMeetingNewItems");
                    da.TableMappings.Add("Table5", "SecondToLastMeetingNewItems");

                    da.Fill(ds);
                }
            }
        }
        catch (SqlException ex)
        {
            LogUtility.LogException(ex);
            throw ex;
        }

        return ds;
    }

    public static DataTable AORMeetingInstanceNotesList_Get(int AORMeetingID, int AORMeetingInstanceID, int AORNoteTypeID = 0, bool ShowRemoved = false)
    {
        string procName = "AORMeetingInstanceNotesList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@AORNoteTypeID", SqlDbType.Int).Value = AORNoteTypeID;
                    cmd.Parameters.Add("@ShowRemoved", SqlDbType.Bit).Value = ShowRemoved ? 1 : 0;

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

    public static DataTable AORMeetingInstanceSelectedNoteDetail_Get(int AORMeetingNotesID, bool ShowRemoved = false, bool ShowClosed = false)
    {
        string procName = "AORMeetingInstanceSelectedNoteDetail_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingNotesID", SqlDbType.Int).Value = AORMeetingNotesID;
                    cmd.Parameters.Add("@ShowRemoved", SqlDbType.Bit).Value = ShowRemoved ? 1 : 0;
                    cmd.Parameters.Add("@ShowClosed", SqlDbType.Bit).Value = ShowClosed ? 1 : 0;

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


    public static DataTable AORMeetingInstanceNotesDetailList_Get(int AORMeetingNotesID_Parent, bool ShowRemoved = false, bool ShowClosed = false, int AORID = 0, int AORMeetingInstanceID = 0, int NoteTypeID = 0)
    {
        string procName = "AORMeetingInstanceNotesDetailList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingNotesID_Parent", SqlDbType.Int).Value = AORMeetingNotesID_Parent;
                    cmd.Parameters.Add("@ShowRemoved", SqlDbType.Bit).Value = ShowRemoved ? 1 : 0;
                    cmd.Parameters.Add("@ShowClosed", SqlDbType.Bit).Value = ShowClosed ? 1 : 0;
                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@NoteTypeID", SqlDbType.Int).Value = NoteTypeID;

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

    public static DataTable AORMeetingInstanceNotesDetail_Get(int AORMeetingNotesID)
    {
        string procName = "AORMeetingInstanceNotesDetail_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingNotesID", SqlDbType.Int).Value = AORMeetingNotesID;

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

    public static Dictionary<string, string> AORMeetingInstanceNote_Toggle(int AORMeetingID, int AORMeetingInstanceID, int AORMeetingNotesID, int Opt)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "error", "" } };
        bool saved = false, exists = false;
        string procName = "AORMeetingInstanceNote_Toggle";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                cmd.Parameters.Add("@AORMeetingNotesID", SqlDbType.Int).Value = AORMeetingNotesID;
                cmd.Parameters.Add("@Opt", SqlDbType.Int).Value = Opt;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Exists", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                SqlParameter paramExists = cmd.Parameters["@Exists"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                if (paramExists != null) bool.TryParse(paramExists.Value.ToString(), out exists);

                result["saved"] = saved.ToString();
                result["exists"] = exists.ToString();
            }
        }

        return result;
    }

    public static string GenerateAgendaFromObjectivesList(List<Dictionary<string, string>> objectives)
    {
        StringBuilder agenda = new StringBuilder();
        
        for (int i = 0; i < objectives.Count; i++)
        {
            Dictionary<string, string> objective = objectives[i];
            int level = Convert.ToInt32(objective["level"]);
            string seqtext = objective["seqtext"];

            agenda.Append("<div><span style=\"font-family:'Courier New'\">");

            StringBuilder prefix = new StringBuilder();

            string priorPrefix = "";
            int priorLevel = 1;
            // is there a previous objective with a lower level than this one? if so, we use
            for (int x = i - 1; x >= 0; x--)
            {
                Dictionary<string, string> priorObjective = objectives[x];
                priorLevel = Convert.ToInt32(priorObjective["level"]);

                if (priorLevel < level && priorObjective["seqoff"] == "false")
                {
                    priorPrefix = priorObjective["prefixused"];
                    break;
                }
            }

            if (priorPrefix != "")
            {
                // indent using the prior prefix (replace prior prefix with spaces, but we have to preserve the bold characters or spacing won't look right unless we use fixed-width font)
                StringBuilder fixedPriorPrefix = new StringBuilder(priorPrefix.Replace("&nbsp;", "*").Replace("<b>", "$").Replace("</b>", "%"));
                for (int x = 0; x < fixedPriorPrefix.Length; x++)
                {
                    if (fixedPriorPrefix[x] != '*' && fixedPriorPrefix[x] != '$' && fixedPriorPrefix[x] != '%')
                    {
                        fixedPriorPrefix[x] = ' ';
                    }
                }

                prefix.Append(fixedPriorPrefix.ToString().Replace("*", "&nbsp;").Replace("$", "<b>").Replace("%", "</b>").Replace(" ", "&nbsp;"));
            }

            int levelDiff = level - priorLevel;
            for (int x = 0; x < levelDiff; x++)
            {
                //prefix.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
            }

            if (objective["seqoff"] == "false")
            {
                prefix.Append("<b>" + objective["seqtext"] + "</b>");
                prefix.Append("&nbsp;&nbsp;");
            }

            objective["prefixused"] = prefix.ToString();

            agenda.Append(prefix);

            string title = StringUtil.UndoStrongEscape(objective["title"]);
            if (objective["key"] == "REMOVED")
            {
                title = "[REMOVED]";
            }

            agenda.Append(title);

            agenda.Append("</span></div>");
        }

        return agenda.ToString();
    }

    public static List<Dictionary<string, string>> ParseObjectivesJson(string extData)
    {
        List<Dictionary<string, string>> objectives = new List<Dictionary<string, string>>();

        if (extData.IndexOf("<objectivesjson>") != -1)
        {
            int idx = extData.IndexOf("<objectivesjson>");
            int len = "<objectivesjson>".Length;
            int idx2 = extData.IndexOf("</objectivesjson>");
            extData = extData.Substring(idx + len, idx2 - (idx + len));

            List<object> lstObj = StringUtil.DeserializeJsonToList(extData);

            for (int i = 0; lstObj != null && i < lstObj.Count; i++)
            {
                Dictionary<string, string> newDict = new Dictionary<string, string>();
                Dictionary<string, object> dictObj = (Dictionary<string, object>)lstObj[i];

                foreach (string k in dictObj.Keys)
                {
                    string v = dictObj[k].ToString();

                    newDict.Add(k, v);
                }

                objectives.Add(newDict);
            }
        }

        return objectives;
    }

    public static string GetMeetingMetricsResult(int AORMeetingID, int AORMeetingInstanceID)
    {
        Dictionary<string, string> result = WTSPage.CreateDefaultResult();

        try
        {
            DataSet ds = AOR.AORMeetingInstanceMetrics_Get(AORMeetingID, AORMeetingInstanceID);

            DataTable meetingMetrics = ds.Tables["MeetingMetrics"];
            if (meetingMetrics.Rows.Count > 0)
            {
                DataRow row = meetingMetrics.Rows[0];

                int totalMeetings = row["TotalMeetings"] != DBNull.Value ? (int)row["TotalMeetings"] : 0;
                decimal avgLength = row["AvgLength"] != DBNull.Value ? (decimal)row["AvgLength"] : 0;
                decimal avgAttendedCount = row["AvgAttendedCount"] != DBNull.Value ? (decimal)row["AvgAttendedCount"] : 0;
                decimal avgResourcesCount = row["AvgResourcesCount"] != DBNull.Value ? (decimal)row["AvgResourcesCount"] : 0;
                decimal avgAttendedPct = row["AvgAttendedPct"] != DBNull.Value ? (decimal)row["AvgAttendedPct"] : 0;
                decimal maxAttendedPct = row["MaxAttendedPct"] != DBNull.Value ? (decimal)row["MaxAttendedPct"] : 0;

                result["totalmeetings"] = totalMeetings.ToString();
                result["avglength"] = avgLength.ToString("0");
                result["avgattendedcount"] = avgAttendedCount.ToString("0");
                result["avgresourcescount"] = avgResourcesCount.ToString("0");
                result["avgattendedpct"] = (avgAttendedPct * 100).ToString("0");
                result["maxattendedpct"] = (maxAttendedPct * 100).ToString("0");
            }

            DataTable noteMetrics = ds.Tables["NoteMetrics"];
            if (noteMetrics.Rows.Count > 0)
            {
                for (int i = 0; i < noteMetrics.Rows.Count; i++)
                {
                    DataRow row = noteMetrics.Rows[i];

                    int noteTypeID = (int)row["AORNoteTypeID"];
                    int noteTypeCount = (int)row["NoteTypeCount"];
                    int noteTypeClosed = (int)row["NoteTypeClosed"];

                    result["notetype_" + noteTypeID] = noteTypeCount + "_" + (noteTypeID == (int)NoteTypeEnum.ActionItems ? noteTypeClosed : -1);
                }
            }

            DataTable lastMeetingMetrics = ds.Tables["LastMeetingMetrics"];
            DataTable secondToLastMeetingMetrics = ds.Tables["SecondToLastMeetingMetrics"];
            DataTable lastMeetingNewItems = ds.Tables["LastMeetingNewItems"];
            DataTable secondToLastMeetingNewItems = ds.Tables["SecondToLastMeetingNewItems"];

            int lastMeetingID = lastMeetingMetrics.Rows[0]["LastMeetingID"] != DBNull.Value ? (int)lastMeetingMetrics.Rows[0]["LastMeetingID"] : 0;
            int lastAllNotes = lastMeetingMetrics.Rows[0]["AllNotes"] != DBNull.Value ? (int)lastMeetingMetrics.Rows[0]["AllNotes"] : 0;
            int lastOpenNotes = lastMeetingMetrics.Rows[0]["OpenNotes"] != DBNull.Value ? (int)lastMeetingMetrics.Rows[0]["OpenNotes"] : 0;
            int lastClosedNotes = lastMeetingMetrics.Rows[0]["ClosedNotes"] != DBNull.Value ? (int)lastMeetingMetrics.Rows[0]["ClosedNotes"] : 0;

            int secondToLastMeetingID = secondToLastMeetingMetrics.Rows[0]["SecondToLastMeetingID"] != DBNull.Value ? (int)secondToLastMeetingMetrics.Rows[0]["SecondToLastMeetingID"] : 0;
            int secondToLastAllNotes = secondToLastMeetingMetrics.Rows[0]["AllNotes"] != DBNull.Value ? (int)secondToLastMeetingMetrics.Rows[0]["AllNotes"] : 0;
            int secondToLastOpenNotes = secondToLastMeetingMetrics.Rows[0]["OpenNotes"] != DBNull.Value ? (int)secondToLastMeetingMetrics.Rows[0]["OpenNotes"] : 0;
            int secondToLastClosedNotes = secondToLastMeetingMetrics.Rows[0]["ClosedNotes"] != DBNull.Value ? (int)secondToLastMeetingMetrics.Rows[0]["ClosedNotes"] : 0;

            int newItemsLastMeeting = lastMeetingNewItems.Rows[0]["NewItems"] != DBNull.Value ? (int)lastMeetingNewItems.Rows[0]["NewItems"] : 0;
            int newItemsSecondToLastMeeting = secondToLastMeetingNewItems.Rows[0]["NewItems"] != DBNull.Value ? (int)secondToLastMeetingNewItems.Rows[0]["NewItems"] : 0;

            result["lastmeetingid"] = lastMeetingID.ToString();
            result["lastallnotes"] = lastAllNotes.ToString();
            result["lastopennotes"] = lastOpenNotes.ToString();
            result["lastclosednotes"] = lastClosedNotes.ToString();

            result["secondtolastmeetingid"] = secondToLastMeetingID.ToString();
            result["secondtolastallnotes"] = secondToLastAllNotes.ToString();
            result["secondtolastopennotes"] = secondToLastOpenNotes.ToString();
            result["secondtolastclosednotes"] = secondToLastClosedNotes.ToString();

            result["newitemslastmeeting"] = newItemsLastMeeting.ToString();
            result["newItemsSecondToLastMeeting"] = newItemsSecondToLastMeeting.ToString();

            result["success"] = "true";
        }
        catch (Exception ex)
        {
            result["error"] = ex.Message;
            LogUtility.LogError(ex.Message + " " + ex.StackTrace); // we are logigng without email sending from here because the underlying db call already logged the exception
        }

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }

    public static DataTable AORMeetingInstanceAddList_Get(int AORMeetingID, int AORMeetingInstanceID, string Type, string QFSystem = "", string QFRelease = "", string QFName = "", int InstanceFilterID = 0, int NoteTypeFilterID = 0)
    {
        string procName = "AORMeetingInstanceAddList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
                    cmd.Parameters.Add("@QFSystem", SqlDbType.NVarChar).Value = QFSystem;
                    cmd.Parameters.Add("@QFRelease", SqlDbType.NVarChar).Value = QFRelease;
                    cmd.Parameters.Add("QFName", SqlDbType.NVarChar).Value = QFName != null ? QFName : (object)DBNull.Value;
                    cmd.Parameters.Add("@InstanceFilterID", SqlDbType.Int).Value = InstanceFilterID;
                    cmd.Parameters.Add("@NoteTypeFilterID", SqlDbType.Int).Value = NoteTypeFilterID;

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

    public static bool AORMeetingInstanceAdd_Save(int AORMeetingID, int AORMeetingInstanceID, string Type, XmlDocument Additions)
    {
        bool saved = false;
        string procName = "AORMeetingInstanceAdd_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
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

    public static DataTable AORMeetingInstanceNotes_GetLastAdded(int AORMeetingID, int AORMeetingInstanceID, int AORID = 0, int AORNoteTypeID = 0, string title = null)
    {
        string procName = "AORMeetingInstanceNotes_GetLastAdded";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORNoteTypeID", SqlDbType.Int).Value = AORNoteTypeID;
                    cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = string.IsNullOrWhiteSpace(title) ? DBNull.Value : (object)title;

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

    public static DataTable AORMeetingInstanceAORProgress_Get(int AORMeetingID, int AORMeetingInstanceID)
    {
        string procName = "AORMeetingInstanceAORProgress_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;

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

    public static DataSet AORMeetingInstanceReport_Get(int AORMeetingID, int AORMeetingInstanceID, bool ShowRemovedNotes)
    {
        DataSet ds = new DataSet();
        string procName = "AORMeetingInstanceReport_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                cmd.Parameters.Add("@ShowRemovedNotes", SqlDbType.Bit).Value = ShowRemovedNotes;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "Attribute");
                da.TableMappings.Add("Table1", "AOR");
                da.TableMappings.Add("Table2", "Resource");
                da.TableMappings.Add("Table3", "LastMeetingActionItem");
                da.TableMappings.Add("Table4", "Objective");
                da.TableMappings.Add("Table5", "BurndownOverview");
                da.TableMappings.Add("Table6", "StoppingCondition");
                da.TableMappings.Add("Table7", "QuestionDiscussionPoint");
                da.TableMappings.Add("Table8", "Note");
                da.TableMappings.Add("Table9", "ActionItem");
                da.TableMappings.Add("Table10", "SR");
                da.TableMappings.Add("Table11", "BurndownGrid");

                da.Fill(ds);
            }
        }

        return ds;
    }

    public static bool ExportMeetingInstanceData(string type, string DownloadSettings, int AORMeetingID, int AORMeetingInstanceID, string emailRecipients, string customNote, bool attachToMeeting, int attachmentTypeId, out int newAttachmentID, bool showRemoved, System.Web.HttpResponse Response, System.Web.HttpServerUtility Server)
    {
        newAttachmentID = 0;

        switch (type)
        {
            case "pdf":
                DataSet ds = AOR.AORMeetingInstanceReport_Get(AORMeetingID: AORMeetingID, AORMeetingInstanceID: AORMeetingInstanceID, ShowRemovedNotes: showRemoved);

                if (ds != null)
                {
                    #region Attribute
                    DataTable dtAttribute = ds.Tables["Attribute"];
                    DateTime nDate = new DateTime();

                    dtAttribute.Columns.Add("InstanceDate");

                    if (DateTime.TryParse(dtAttribute.Rows[0]["InstanceDateTime"].ToString(), out nDate))
                    {
                        dtAttribute.Rows[0]["InstanceDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                    }

                    dtAttribute.Columns["InstanceDate"].SetOrdinal(dtAttribute.Columns["InstanceDateTime"].Ordinal);
                    dtAttribute.Columns.Remove("InstanceDateTime");

                    DataRow drAttribute = ds.Tables["Attribute"].Rows[0];
                    string fileName = drAttribute["AORMeetingName"].ToString() + " " + drAttribute["InstanceDate"].ToString();
                    #endregion

                    #region AOR
                    DataTable dtAOR = ds.Tables["AOR"];

                    dtAOR.Columns.Add("AddDate");

                    for (int i = 0; i < dtAOR.Rows.Count; i++)
                    {
                        nDate = new DateTime();

                        if (DateTime.TryParse(dtAOR.Rows[i]["AddDateTime"].ToString(), out nDate))
                        {
                            dtAOR.Rows[i]["AddDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }
                    }

                    dtAOR.Columns["AddDate"].SetOrdinal(dtAOR.Columns["AddDateTime"].Ordinal);
                    dtAOR.Columns.Remove("AddDateTime");
                    #endregion

                    #region Resource
                    DataTable dtResource = ds.Tables["Resource"];

                    dtResource.Columns.Add("LastMeetingAttended");

                    for (int i = 0; i < dtResource.Rows.Count; i++)
                    {
                        nDate = new DateTime();

                        if (DateTime.TryParse(dtResource.Rows[i]["LastMeetingAttendedDate"].ToString(), out nDate))
                        {
                            dtResource.Rows[i]["LastMeetingAttended"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }
                    }

                    dtResource.Columns["LastMeetingAttended"].SetOrdinal(dtResource.Columns["LastMeetingAttendedDate"].Ordinal);
                    dtResource.Columns.Remove("LastMeetingAttendedDate");
                    #endregion

                    #region LastMeetingActionItem
                    DataTable dtLastMeetingActionItem = ds.Tables["LastMeetingActionItem"];

                    dtLastMeetingActionItem.Columns.Add("StatusDate");
                    dtLastMeetingActionItem.Columns.Add("AddDate");

                    for (int i = 0; i < dtLastMeetingActionItem.Rows.Count; i++)
                    {
                        nDate = new DateTime();

                        if (DateTime.TryParse(dtLastMeetingActionItem.Rows[i]["StatusDateTime"].ToString(), out nDate))
                        {
                            dtLastMeetingActionItem.Rows[i]["StatusDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        nDate = new DateTime();

                        if (DateTime.TryParse(dtLastMeetingActionItem.Rows[i]["AddDateTime"].ToString(), out nDate))
                        {
                            dtLastMeetingActionItem.Rows[i]["AddDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        if (dtLastMeetingActionItem.Rows[i]["Notes"].ToString().Contains("font-size: small;"))
                        {
                            dtLastMeetingActionItem.Rows[i]["Notes"] = dtLastMeetingActionItem.Rows[i]["Notes"].ToString().Replace("font-size: small;", "");
                        }
                    }

                    dtLastMeetingActionItem.Columns["StatusDate"].SetOrdinal(dtLastMeetingActionItem.Columns["StatusDateTime"].Ordinal);
                    dtLastMeetingActionItem.Columns["AddDate"].SetOrdinal(dtLastMeetingActionItem.Columns["AddDateTime"].Ordinal);
                    dtLastMeetingActionItem.Columns.Remove("StatusDateTime");
                    dtLastMeetingActionItem.Columns.Remove("AddDateTime");

                    if (dtLastMeetingActionItem.Rows.Count == 0) dtLastMeetingActionItem.Rows.Add();
                    #endregion

                    #region Objective
                    DataTable dtObjective = ds.Tables["Objective"];

                    dtObjective.Columns.Add("StatusDate");
                    dtObjective.Columns.Add("AddDate");

                    for (int i = 0; i < dtObjective.Rows.Count; i++)
                    {
                        nDate = new DateTime();

                        if (DateTime.TryParse(dtObjective.Rows[i]["StatusDateTime"].ToString(), out nDate))
                        {
                            dtObjective.Rows[i]["StatusDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        nDate = new DateTime();

                        if (DateTime.TryParse(dtObjective.Rows[i]["AddDateTime"].ToString(), out nDate))
                        {
                            dtObjective.Rows[i]["AddDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        if (dtObjective.Rows[i]["Notes"].ToString().Contains("font-size: small;"))
                        {
                            dtObjective.Rows[i]["Notes"] = dtObjective.Rows[i]["Notes"].ToString().Replace("font-size: small;", "");
                        }
                    }

                    dtObjective.Columns["StatusDate"].SetOrdinal(dtObjective.Columns["StatusDateTime"].Ordinal);
                    dtObjective.Columns["AddDate"].SetOrdinal(dtObjective.Columns["AddDateTime"].Ordinal);
                    dtObjective.Columns.Remove("StatusDateTime");
                    dtObjective.Columns.Remove("AddDateTime");

                    if (dtObjective.Rows.Count == 0) dtObjective.Rows.Add();
                    #endregion

                    #region BurndownOverview
                    DataTable dtBurndownOverview = ds.Tables["BurndownOverview"];

                    dtBurndownOverview.Columns.Add("StatusDate");
                    dtBurndownOverview.Columns.Add("AddDate");

                    for (int i = 0; i < dtBurndownOverview.Rows.Count; i++)
                    {
                        nDate = new DateTime();

                        if (DateTime.TryParse(dtBurndownOverview.Rows[i]["StatusDateTime"].ToString(), out nDate))
                        {
                            dtBurndownOverview.Rows[i]["StatusDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        nDate = new DateTime();

                        if (DateTime.TryParse(dtBurndownOverview.Rows[i]["AddDateTime"].ToString(), out nDate))
                        {
                            dtBurndownOverview.Rows[i]["AddDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        if (dtBurndownOverview.Rows[i]["Notes"].ToString().Contains("font-size: small;"))
                        {
                            dtBurndownOverview.Rows[i]["Notes"] = dtBurndownOverview.Rows[i]["Notes"].ToString().Replace("font-size: small;", "");
                        }
                    }

                    dtBurndownOverview.Columns["StatusDate"].SetOrdinal(dtBurndownOverview.Columns["StatusDateTime"].Ordinal);
                    dtBurndownOverview.Columns["AddDate"].SetOrdinal(dtBurndownOverview.Columns["AddDateTime"].Ordinal);
                    dtBurndownOverview.Columns.Remove("StatusDateTime");
                    dtBurndownOverview.Columns.Remove("AddDateTime");

                    if (dtBurndownOverview.Rows.Count == 0) dtBurndownOverview.Rows.Add();
                    #endregion

                    #region StoppingCondition
                    DataTable dtStoppingCondition = ds.Tables["StoppingCondition"];

                    dtStoppingCondition.Columns.Add("StatusDate");
                    dtStoppingCondition.Columns.Add("AddDate");

                    for (int i = 0; i < dtStoppingCondition.Rows.Count; i++)
                    {
                        nDate = new DateTime();

                        if (DateTime.TryParse(dtStoppingCondition.Rows[i]["StatusDateTime"].ToString(), out nDate))
                        {
                            dtStoppingCondition.Rows[i]["StatusDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        nDate = new DateTime();

                        if (DateTime.TryParse(dtStoppingCondition.Rows[i]["AddDateTime"].ToString(), out nDate))
                        {
                            dtStoppingCondition.Rows[i]["AddDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        if (dtStoppingCondition.Rows[i]["Notes"].ToString().Contains("font-size: small;"))
                        {
                            dtStoppingCondition.Rows[i]["Notes"] = dtStoppingCondition.Rows[i]["Notes"].ToString().Replace("font-size: small;", "");
                        }
                    }

                    dtStoppingCondition.Columns["StatusDate"].SetOrdinal(dtStoppingCondition.Columns["StatusDateTime"].Ordinal);
                    dtStoppingCondition.Columns["AddDate"].SetOrdinal(dtStoppingCondition.Columns["AddDateTime"].Ordinal);
                    dtStoppingCondition.Columns.Remove("StatusDateTime");
                    dtStoppingCondition.Columns.Remove("AddDateTime");

                    if (dtStoppingCondition.Rows.Count == 0) dtStoppingCondition.Rows.Add();
                    #endregion

                    #region QuestionDiscussionPoint
                    DataTable dtQuestionDiscussionPoint = ds.Tables["QuestionDiscussionPoint"];

                    dtQuestionDiscussionPoint.Columns.Add("StatusDate");
                    dtQuestionDiscussionPoint.Columns.Add("AddDate");

                    for (int i = 0; i < dtQuestionDiscussionPoint.Rows.Count; i++)
                    {
                        nDate = new DateTime();

                        if (DateTime.TryParse(dtQuestionDiscussionPoint.Rows[i]["StatusDateTime"].ToString(), out nDate))
                        {
                            dtQuestionDiscussionPoint.Rows[i]["StatusDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        nDate = new DateTime();

                        if (DateTime.TryParse(dtQuestionDiscussionPoint.Rows[i]["AddDateTime"].ToString(), out nDate))
                        {
                            dtQuestionDiscussionPoint.Rows[i]["AddDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        if (dtQuestionDiscussionPoint.Rows[i]["Notes"].ToString().Contains("font-size: small;"))
                        {
                            dtQuestionDiscussionPoint.Rows[i]["Notes"] = dtQuestionDiscussionPoint.Rows[i]["Notes"].ToString().Replace("font-size: small;", "");
                        }
                    }

                    dtQuestionDiscussionPoint.Columns["StatusDate"].SetOrdinal(dtQuestionDiscussionPoint.Columns["StatusDateTime"].Ordinal);
                    dtQuestionDiscussionPoint.Columns["AddDate"].SetOrdinal(dtQuestionDiscussionPoint.Columns["AddDateTime"].Ordinal);
                    dtQuestionDiscussionPoint.Columns.Remove("StatusDateTime");
                    dtQuestionDiscussionPoint.Columns.Remove("AddDateTime");

                    if (dtQuestionDiscussionPoint.Rows.Count == 0) dtQuestionDiscussionPoint.Rows.Add();
                    #endregion

                    #region Note
                    DataTable dtNote = ds.Tables["Note"];

                    dtNote.Columns.Add("StatusDate");
                    dtNote.Columns.Add("AddDate");

                    for (int i = 0; i < dtNote.Rows.Count; i++)
                    {
                        nDate = new DateTime();

                        if (DateTime.TryParse(dtNote.Rows[i]["StatusDateTime"].ToString(), out nDate))
                        {
                            dtNote.Rows[i]["StatusDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        nDate = new DateTime();

                        if (DateTime.TryParse(dtNote.Rows[i]["AddDateTime"].ToString(), out nDate))
                        {
                            dtNote.Rows[i]["AddDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        if (dtNote.Rows[i]["Notes"].ToString().Contains("font-size: small;"))
                        {
                            dtNote.Rows[i]["Notes"] = dtNote.Rows[i]["Notes"].ToString().Replace("font-size: small;", "");
                        }
                    }

                    dtNote.Columns["StatusDate"].SetOrdinal(dtNote.Columns["StatusDateTime"].Ordinal);
                    dtNote.Columns["AddDate"].SetOrdinal(dtNote.Columns["AddDateTime"].Ordinal);
                    dtNote.Columns.Remove("StatusDateTime");
                    dtNote.Columns.Remove("AddDateTime");

                    if (dtNote.Rows.Count == 0) dtNote.Rows.Add();
                    #endregion

                    #region ActionItem
                    DataTable dtActionItem = ds.Tables["ActionItem"];

                    dtActionItem.Columns.Add("StatusDate");
                    dtActionItem.Columns.Add("AddDate");

                    for (int i = 0; i < dtActionItem.Rows.Count; i++)
                    {
                        nDate = new DateTime();

                        if (DateTime.TryParse(dtActionItem.Rows[i]["StatusDateTime"].ToString(), out nDate))
                        {
                            dtActionItem.Rows[i]["StatusDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        nDate = new DateTime();

                        if (DateTime.TryParse(dtActionItem.Rows[i]["AddDateTime"].ToString(), out nDate))
                        {
                            dtActionItem.Rows[i]["AddDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        if (dtActionItem.Rows[i]["Notes"].ToString().Contains("font-size: small;"))
                        {
                            dtActionItem.Rows[i]["Notes"] = dtActionItem.Rows[i]["Notes"].ToString().Replace("font-size: small;", "");
                        }
                    }

                    dtActionItem.Columns["StatusDate"].SetOrdinal(dtActionItem.Columns["StatusDateTime"].Ordinal);
                    dtActionItem.Columns["AddDate"].SetOrdinal(dtActionItem.Columns["AddDateTime"].Ordinal);
                    dtActionItem.Columns.Remove("StatusDateTime");
                    dtActionItem.Columns.Remove("AddDateTime");

                    if (dtActionItem.Rows.Count == 0) dtActionItem.Rows.Add();
                    #endregion

                    #region SR
                    DataTable dtSR = ds.Tables["SR"];

                    for (int i = 0; i < dtSR.Rows.Count; i++)
                    {
                        dtSR.Rows[i]["Description"] = Uri.UnescapeDataString(dtSR.Rows[i]["Description"].ToString());
                        dtSR.Rows[i]["LastReply"] = Uri.UnescapeDataString(dtSR.Rows[i]["LastReply"].ToString());
                    }
                    #endregion

                    ReportDocument cryRpt = new ReportDocument();

                    cryRpt.Load(Server.MapPath(@"~/Reports/MeetingInstance.rpt"));
                    cryRpt.SetDataSource(ds);

                    if (!DownloadSettings.Contains("AORs Included"))
                    {
                        cryRpt.ReportDefinition.Sections["DetailSection2"].SectionFormat.EnableSuppress = true;
                        cryRpt.ReportDefinition.Sections["DetailSection11"].SectionFormat.EnableSuppress = true;
                    }
                    if (!DownloadSettings.Contains("Agenda/Objectives")) cryRpt.ReportDefinition.Sections["DetailSection4"].SectionFormat.EnableSuppress = true;
                    if (!DownloadSettings.Contains("Burndown Overview")) cryRpt.ReportDefinition.Sections["DetailSection10"].SectionFormat.EnableSuppress = true;
                    if (!DownloadSettings.Contains("Burndown Grid")) cryRpt.ReportDefinition.Sections["DetailSection9"].SectionFormat.EnableSuppress = true;
                    if (!DownloadSettings.Contains("Action Items")) cryRpt.ReportDefinition.Sections["DetailSection5"].SectionFormat.EnableSuppress = true;
                    if (!DownloadSettings.Contains("Stopping Conditions")) cryRpt.ReportDefinition.Sections["DetailSection6"].SectionFormat.EnableSuppress = true;
                    if (!DownloadSettings.Contains("Questions/Discussion Points")) cryRpt.ReportDefinition.Sections["DetailSection7"].SectionFormat.EnableSuppress = true;
                    if (!DownloadSettings.Contains("Notes")) cryRpt.ReportDefinition.Sections["DetailSection8"].SectionFormat.EnableSuppress = true;

                    if (attachToMeeting) // saving to meeting
                    {
                        System.IO.Stream fs = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
                        byte[] attachment;
                        using (var memoryStream = new MemoryStream())
                        {
                            fs.CopyTo(memoryStream);
                            attachment = memoryStream.ToArray();
                        }

                        AORMeetingInstanceAttachment_Save(0, AORMeetingInstanceID, 0, attachmentTypeId == 0 ? (int)WTS.Enums.AttachmentTypeEnum.SupplementalDocument : attachmentTypeId, fileName + ".pdf", drAttribute["AORMeetingName"].ToString() + " Minutes", null, attachment, out newAttachmentID);
                    }
                    else if (string.IsNullOrWhiteSpace(emailRecipients)) // downloading
                    {
                        if (DownloadSettings.Contains("Attachments"))
                        {
                            DataTable dt = AOR.AORMeetingInstanceAttachment_Get(0, 0, AORMeetingInstanceID, 0, true);

                            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

                            foreach (DataRow row in dt.Rows)
                            {
                                files.Add((string)row["FileName"], (byte[])row["FileData"]);
                            }

                            System.IO.Stream fs = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
                            byte[] attachment;
                            using (var memoryStream = new MemoryStream())
                            {
                                fs.CopyTo(memoryStream);
                                attachment = memoryStream.ToArray();
                            }

                            files.Add(fileName + ".pdf", attachment);

                            byte[] zip = FileUtil.CreateZipFile(files);

                            Response.AppendHeader("content-disposition", "attachment; filename=\"" + fileName + ".zip" + "\"");
                            Response.ContentType = "application/octet-stream";
                            Response.BinaryWrite(zip);
                            Response.End();
                        }
                        else
                        {
                            cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, fileName);
                            Response.End();
                        }
                    }
                    else // emailing list instead 
                    {                                               
                        System.IO.Stream fs = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
                        byte[] attachment;
                        using (var memoryStream = new MemoryStream())
                        {
                            fs.CopyTo(memoryStream);
                            attachment = memoryStream.ToArray();
                        }

                        if (fs.Length > 0)
                        {
                            string[] recipients = emailRecipients.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string subject = drAttribute["AORMeetingName"].ToString() + " Minutes";
                            string body = (customNote != null ? customNote + "\r\n\r\n------------------------------------------------------------------\r\n\r\n" : "") + "Attachment: " + subject;


                            var loggedInMembershipUser = System.Web.Security.Membership.GetUser();
                            var loggedInUser = new WTS_User();
                            loggedInUser.Load(loggedInMembershipUser.ProviderUserKey.ToString());

                            //string from = loggedInUser.Email ?? WTSConfiguration.EmailFrom;
                            //string fromName = loggedInUser.First_Name + " " + loggedInUser.Last_Name;
                            string from = WTSConfiguration.EmailFrom;
                            string fromName = WTSConfiguration.EmailFromName;

                            Dictionary<string, byte[]> attachments = new Dictionary<string, byte[]>();
                            attachments.Add(fileName + ".pdf", attachment);

                            if (DownloadSettings.Contains("Attachments"))
                            {
                                DataTable dt = AOR.AORMeetingInstanceAttachment_Get(0, 0, AORMeetingInstanceID, 0, true);

                                Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

                                foreach (DataRow row in dt.Rows)
                                {
                                    attachments.Add((string)row["FileName"], (byte[])row["FileData"]);
                                }
                            }

                            Dictionary<string, string> toAddresses = new Dictionary<string, string>();
                            List<int> resourceIDs = new List<int>();
                            foreach (string rID in recipients)
                            {
                                int id = 0;

                                if (Int32.TryParse(rID, out id))
                                {
                                    WTS_User u = new WTS_User();
                                    u.Load(id);

                                    string email = string.IsNullOrWhiteSpace(u.Email) ? u.Email2 : u.Email;

                                    if (!string.IsNullOrWhiteSpace(email))
                                    {
                                        resourceIDs.Add(id);

                                        if (toAddresses.ContainsKey(email))
                                        {
                                            toAddresses[email] = toAddresses[email] + ", " + u.First_Name + " " + u.Last_Name;
                                        }
                                        else
                                        {
                                            toAddresses.Add(email, u.First_Name + " " + u.Last_Name);
                                        }
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }

                            if (toAddresses.Count > 0)
                            {
                                AOR.AORMeetingEmail_Delete(AORMeetingID);
                                AOR.AORMeetingEmail_Add(AORMeetingID, 0, string.Join(",", resourceIDs));

                                IEvent evt = EventQueue.Instance.QueueEmailEvent(toAddresses, null, null, subject, body, from, fromName, false, System.Net.Mail.MailPriority.Normal, attachments, false);

                                if (evt == null)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                break;
        }

        return true;
    }

    #endregion

    #region AOR Meeting Instance Attachments

    public static DataTable AORMeetingInstanceAttachment_Get(long AORMeetingInstanceAttachmentID, int AORMeetingID, int AORMeetingInstanceID, int attachmentID, bool includeData)
    {
        string procName = "AORMeetingInstanceAttachment_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingInstanceAttachmentID", SqlDbType.Int).Value = AORMeetingInstanceAttachmentID;
                    cmd.Parameters.Add("@AORMeetingID", SqlDbType.Int).Value = AORMeetingID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = attachmentID;
                    cmd.Parameters.Add("@IncludeData", SqlDbType.Bit).Value = includeData;

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

    public static bool AORMeetingInstanceAttachment_Save(long AORMeetingInstanceAttachmentID, int AORMeetingInstanceID, int attachmentID, int attachmentTypeId, string fileName, string title, string description, byte[] fileData, out int newAttachmentID)
    {
        newAttachmentID = 0;

        try
        {
            int newID = 0;
            
            string errors = null;
            WTSData.Attachment_Add(attachmentTypeId, fileName, title, description, fileData, 0, out newID, out errors);

            if (newID > 0)
            {                
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand("AORMeetingInstanceAttachment_Save", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@AORMeetingInstanceAttachmentID", SqlDbType.Int).Value = AORMeetingInstanceAttachmentID;
                        cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
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

    public static bool AORMeetingInstanceAttachment_Delete(long AORMeetingInstanceAttachmentID, int AORMeetingInstanceID, int attachmentID)
    {
        string procName = "AORMeetingInstanceAttachment_Delete";

        try
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORMeetingInstanceAttachmentID", SqlDbType.Int).Value = AORMeetingInstanceAttachmentID;
                    cmd.Parameters.Add("@AORMeetingInstanceID", SqlDbType.Int).Value = AORMeetingInstanceID;
                    cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = attachmentID;

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


    #region CR
    public static DataTable AORSRImportList_Get()
    {
        string procName = "AORSRImportList_Get";

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

    public static DataTable AORCRLookupMetrics_Get(string CRContract = "")
    {
        string procName = "AORCRLookupMetrics_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@CRContract", SqlDbType.VarChar).Value = CRContract;

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

    public static DataTable AORCRLookupList_Get(int CRID = 0)
    {
        string procName = "AORCRLookupList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@CRID", SqlDbType.Int).Value = CRID;

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

    public static bool AORCRLookup_Delete(int CRID)
    {
        bool deleted = false;
        string procName = "AORCRLookup_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@CRID", SqlDbType.Int).Value = CRID;
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

    public static bool AORCRLookup_Update(XmlDocument Changes)
    {
        bool saved = false;
        string procName = "AORCRLookup_Update";

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

    public static bool AORCRLookupNarrative_Save(int CRID, string Notes, string BasisOfRisk, string BasisOfUrgency, string CustomerImpact, string Issue, string ProposedSolution, string Rationale, string WorkloadPriority)
    {
        bool saved = false;
        string procName = "AORCRLookupNarrative_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@CRID", SqlDbType.Int).Value = CRID;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Notes;
                cmd.Parameters.Add("@BasisOfRisk", SqlDbType.NVarChar).Value = BasisOfRisk;
                cmd.Parameters.Add("@BasisOfUrgency", SqlDbType.NVarChar).Value = BasisOfUrgency;
                cmd.Parameters.Add("@CustomerImpact", SqlDbType.NVarChar).Value = CustomerImpact;
                cmd.Parameters.Add("@Issue", SqlDbType.NVarChar).Value = Issue;
                cmd.Parameters.Add("@ProposedSolution", SqlDbType.NVarChar).Value = ProposedSolution;
                cmd.Parameters.Add("@Rationale", SqlDbType.NVarChar).Value = Rationale;
                cmd.Parameters.Add("@WorkloadPriority", SqlDbType.NVarChar).Value = WorkloadPriority;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
            }
        }

        return saved;
    }

    public static Dictionary<string, string> AORCRLookup_Save(int Altered, bool NewCR, int CRID, string CRName, string Title, string Notes, string Websystem, int CSDRequiredNow, string RelatedRelease, string Subgroup, string DesignReview, string ITIPOC, string CustomerPriorityList, int GovernmentCSRD, int PrimarySRID,
        int CAMPriority, int LCMBPriority, int AirstaffPriority, int CustomerPriority, int ITIPriority, int RiskOfPTS,
        int StatusID, DateTime LCMBSubmitted, DateTime LCMBApproved, DateTime ERBISMTSubmitted, DateTime ERBISMTApproved)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false, exists = false;
        int newID = 0;
        DateTime nDate = new DateTime();
        string procName = "AORCRLookup_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Altered", SqlDbType.Bit).Value = Altered;
                cmd.Parameters.Add("@NewCR", SqlDbType.Bit).Value = NewCR ? 1 : 0;
                cmd.Parameters.Add("@CRID", SqlDbType.Int).Value = CRID;
                cmd.Parameters.Add("@CRName", SqlDbType.NVarChar).Value = CRName;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Title;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Notes;
                cmd.Parameters.Add("@Websystem", SqlDbType.NVarChar).Value = Websystem == "" ? (object)DBNull.Value : Websystem;
                cmd.Parameters.Add("@CSDRequiredNow", SqlDbType.Int).Value = CSDRequiredNow;
                cmd.Parameters.Add("@RelatedRelease", SqlDbType.NVarChar).Value = RelatedRelease == "" ? (object)DBNull.Value : RelatedRelease;
                cmd.Parameters.Add("@Subgroup", SqlDbType.NVarChar).Value = Subgroup == "" ? (object)DBNull.Value : Subgroup;
                cmd.Parameters.Add("@DesignReview", SqlDbType.NVarChar).Value = DesignReview == "" ? (object)DBNull.Value : DesignReview;
                cmd.Parameters.Add("@ITIPOC", SqlDbType.NVarChar).Value = ITIPOC == "" ? (object)DBNull.Value : ITIPOC;
                cmd.Parameters.Add("@CustomerPriorityList", SqlDbType.NVarChar).Value = CustomerPriorityList == "" ? (object)DBNull.Value : CustomerPriorityList;
                cmd.Parameters.Add("@GovernmentCSRD", SqlDbType.Int).Value = GovernmentCSRD == -999 ? (object)DBNull.Value : GovernmentCSRD;
                cmd.Parameters.Add("@PrimarySRID", SqlDbType.Int).Value = PrimarySRID == 0 ? (object)DBNull.Value : PrimarySRID;
                cmd.Parameters.Add("@CAMPriority", SqlDbType.Int).Value = CAMPriority == -999 ? (object)DBNull.Value : CAMPriority;
                cmd.Parameters.Add("@LCMBPriority", SqlDbType.Int).Value = LCMBPriority == -999 ? (object)DBNull.Value : LCMBPriority;
                cmd.Parameters.Add("@AirstaffPriority", SqlDbType.Int).Value = AirstaffPriority == -999 ? (object)DBNull.Value : AirstaffPriority;
                cmd.Parameters.Add("@CustomerPriority", SqlDbType.Int).Value = CustomerPriority == -999 ? (object)DBNull.Value : CustomerPriority;
                cmd.Parameters.Add("@ITIPriority", SqlDbType.Int).Value = ITIPriority == -999 ? (object)DBNull.Value : ITIPriority;
                cmd.Parameters.Add("@RiskOfPTS", SqlDbType.Int).Value = RiskOfPTS == -999 ? (object)DBNull.Value : RiskOfPTS;
                cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = StatusID == 0 ? (object)DBNull.Value : StatusID;
                cmd.Parameters.Add("@LCMBSubmitted", SqlDbType.DateTime).Value = LCMBSubmitted == nDate ? (object)DBNull.Value : LCMBSubmitted;
                cmd.Parameters.Add("@LCMBApproved", SqlDbType.DateTime).Value = LCMBApproved == nDate ? (object)DBNull.Value : LCMBApproved;
                cmd.Parameters.Add("@ERBISMTSubmitted", SqlDbType.DateTime).Value = ERBISMTSubmitted == nDate ? (object)DBNull.Value : ERBISMTSubmitted;
                cmd.Parameters.Add("@ERBISMTApproved", SqlDbType.DateTime).Value = ERBISMTApproved == nDate ? (object)DBNull.Value : ERBISMTApproved;
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

    public static DataTable AORSRList_Get(int CRID = 0)
    {
        string procName = "AORSRList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@CRID", SqlDbType.Int).Value = CRID;

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

    public static bool AORSRTask_Delete(int TaskID)
    {
        bool deleted = false;
        string procName = "AORSRTask_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = TaskID;
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

    public static DataTable CRReportFilterData_Get()
    {
        DataTable dt = new DataTable();
        string procName = "CRReportFilterData_Get";

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

    public static Dictionary<string, string> ContractCRReportInfo_Update(string ContractIDs, int userID)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false;
        string procName = "ContractCRReportInfo_Update";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ContractIDs", SqlDbType.NVarChar).Value = ContractIDs;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];
                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
                result["saved"] = saved.ToString();
            }
        }

        return result;
    }

    public static DataSet AORCRReport_Get(string ReleaseIDs, string ScheduledDeliverables, string AORTypes, string VisibleToCustomer, string ContractIDs, string SuiteIDs, string WorkTaskStatus, string WorkloadAllocations, string Title, string SavedView = "", string CoverPage = "True", string IndexPage = "True", string BestCase = "True", string WorstCase = "True", string NormCase = "True", string HideCRDescr = "True", string HideAORDescr = "True", string CreatedBy = "")
    {
        DataSet ds = new DataSet();
        string procName = "AORCRReport_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseIDs", SqlDbType.NVarChar).Value = ReleaseIDs;
                cmd.Parameters.Add("@ScheduledDeliverables", SqlDbType.NVarChar).Value = ScheduledDeliverables;
                cmd.Parameters.Add("@AORTypes", SqlDbType.NVarChar).Value = AORTypes;
                cmd.Parameters.Add("@VisibleToCustomer", SqlDbType.NVarChar).Value = VisibleToCustomer;
                cmd.Parameters.Add("@ContractIDs", SqlDbType.NVarChar).Value = ContractIDs;
                cmd.Parameters.Add("@SystemSuiteIDs", SqlDbType.NVarChar).Value = SuiteIDs;
                cmd.Parameters.Add("@WorkTaskStatus", SqlDbType.NVarChar).Value = WorkTaskStatus;
                cmd.Parameters.Add("@WorkloadAllocations", SqlDbType.NVarChar).Value = WorkloadAllocations;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Title;
                cmd.Parameters.Add("@SavedView", SqlDbType.NVarChar).Value = SavedView;
                cmd.Parameters.Add("@CoverPage", SqlDbType.NVarChar).Value = CoverPage;
                cmd.Parameters.Add("@IndexPage", SqlDbType.NVarChar).Value = IndexPage;
                cmd.Parameters.Add("@BestCase", SqlDbType.NVarChar).Value = BestCase;
                cmd.Parameters.Add("@WorstCase", SqlDbType.NVarChar).Value = WorstCase;
                cmd.Parameters.Add("@NormCase", SqlDbType.NVarChar).Value = NormCase;
                cmd.Parameters.Add("@HideCRDescr", SqlDbType.NVarChar).Value = HideCRDescr;
                cmd.Parameters.Add("@HideAORDescr", SqlDbType.NVarChar).Value = HideAORDescr;
                cmd.Parameters.Add("@Debug", SqlDbType.Bit).Value = 0;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = CreatedBy;
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "Parameters");
                da.TableMappings.Add("Table1", "SD");
                da.TableMappings.Add("Table2", "CR");
                da.TableMappings.Add("Table3", "AOR");
                da.TableMappings.Add("Table4", "SR");
                da.TableMappings.Add("Table5", "PD2TDR");
                da.TableMappings.Add("Table6", "DeployLvl");
                da.TableMappings.Add("Table7", "Narrative");
                da.TableMappings.Add("Table8", "Images");
                da.TableMappings.Add("Table9", "SD2");
                da.Fill(ds);
            }
        }

        return ds;
    }

    public static DataSet ReleaseDSEReport_Get(string ReleaseIDs, string ScheduledDeliverables, string AORTypes, string VisibleToCustomer, string ContractIDs, string SuiteIDs, string WorkTaskStatus, string WorkloadAllocations, string Title, string SavedView = "", string CoverPage = "True", string IndexPage = "True", string BestCase = "True", string WorstCase = "True", string NormCase = "True", string HideCRDescr = "True", string HideAORDescr = "True", string CreatedBy = "")
    {
        DataSet ds = new DataSet();
        string procName = "ReleaseDSEReport_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseIDs", SqlDbType.NVarChar).Value = ReleaseIDs;
                cmd.Parameters.Add("@ScheduledDeliverables", SqlDbType.NVarChar).Value = ScheduledDeliverables;
                cmd.Parameters.Add("@AORTypes", SqlDbType.NVarChar).Value = AORTypes;
                cmd.Parameters.Add("@VisibleToCustomer", SqlDbType.NVarChar).Value = VisibleToCustomer;
                cmd.Parameters.Add("@ContractIDs", SqlDbType.NVarChar).Value = ContractIDs;
                cmd.Parameters.Add("@SystemSuiteIDs", SqlDbType.NVarChar).Value = SuiteIDs;
                cmd.Parameters.Add("@WorkTaskStatus", SqlDbType.NVarChar).Value = WorkTaskStatus;
                cmd.Parameters.Add("@WorkloadAllocations", SqlDbType.NVarChar).Value = WorkloadAllocations;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Title;
                cmd.Parameters.Add("@SavedView", SqlDbType.NVarChar).Value = SavedView;
                cmd.Parameters.Add("@CoverPage", SqlDbType.NVarChar).Value = CoverPage;
                cmd.Parameters.Add("@IndexPage", SqlDbType.NVarChar).Value = IndexPage;
                cmd.Parameters.Add("@BestCase", SqlDbType.NVarChar).Value = BestCase;
                cmd.Parameters.Add("@WorstCase", SqlDbType.NVarChar).Value = WorstCase;
                cmd.Parameters.Add("@NormCase", SqlDbType.NVarChar).Value = NormCase;
                cmd.Parameters.Add("@HideCRDescr", SqlDbType.NVarChar).Value = HideCRDescr;
                cmd.Parameters.Add("@HideAORDescr", SqlDbType.NVarChar).Value = HideAORDescr;
                cmd.Parameters.Add("@Debug", SqlDbType.Bit).Value = 0;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = CreatedBy;
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "Parameters");
                da.TableMappings.Add("Table1", "SD");
                da.TableMappings.Add("Table2", "CR");
                da.TableMappings.Add("Table3", "AOR");
                da.TableMappings.Add("Table4", "SR");
                da.TableMappings.Add("Table5", "PD2TDR");
                da.TableMappings.Add("Table6", "DeployLvl");
                da.TableMappings.Add("Table7", "Narrative");
                da.TableMappings.Add("Table8", "Images");
                da.TableMappings.Add("Table9", "SD2");
                da.TableMappings.Add("Table10", "DeploySummary");
                da.Fill(ds);
            }
        }

        return ds;
    }

    public static bool CRReportBuilder_Save(int ReleaseID, int ContractID, XmlDocument Builder)
    {
        bool saved = false;
        string procName = "CRReportBuilder_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseID", SqlDbType.Int).Value = ReleaseID;
                cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = ContractID;
                cmd.Parameters.Add("@Builder", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(Builder.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
            }
        }

        return saved;
    }

    public static DataSet CRReportBuilderList_Get(int ReleaseID, int ContractID, bool VisibleToCustomer)
    {
        DataSet ds = new DataSet();
        string procName = "CRReportBuilderList_Get";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseID", SqlDbType.Int).Value = ReleaseID;
                cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = ContractID;
                cmd.Parameters.Add("@VisibleToCustomer", SqlDbType.Bit).Value = VisibleToCustomer;
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "CR");
                da.TableMappings.Add("Table1", "AOR");
                da.TableMappings.Add("Table2", "Deployment");
                da.Fill(ds);
            }
        }

        return ds;
    }
    #endregion

    #region Task
    public static DataTable AORTaskAORList_Get(int TaskID = 0)
    {
        string procName = "AORTaskAORList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = TaskID;

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

    public static DataTable AORSubTaskAORList_Get(int TaskID = 0)
    {
        string procName = "AORSubTaskAORList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = TaskID;

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

    public static bool AORTask_Save(int TaskID, XmlDocument AORs, int Add, bool CascadeAOR)
    {
        bool saved = false;
        string procName = "AORTask_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = TaskID;
                cmd.Parameters.Add("@AORs", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(AORs.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@CascadeAOR", SqlDbType.Bit).Value = CascadeAOR ? 1 : 0;
                cmd.Parameters.Add("@Add", SqlDbType.Int).Value = Add;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
            }
        }

        return saved;
    }

    public static bool AORSubTask_Save(int TaskID, XmlDocument AORs, int Add, bool CascadeAOR)
    {
        bool saved = false;
        string procName = "AORSubTask_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SubTaskID", SqlDbType.Int).Value = TaskID;
                cmd.Parameters.Add("@AORs", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(AORs.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@CascadeAOR", SqlDbType.Bit).Value = CascadeAOR ? 1 : 0;
                cmd.Parameters.Add("@Add", SqlDbType.Int).Value = Add;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
            }
        }

        return saved;
    }

    public static DataTable AORTaskOptionsList_Get(int AssignedToID = 0, int PrimaryResourceID = 0, int SystemID = 0, int SystemAffiliated = 0, int ResourceAffiliated = 0, int AssignedToRank = 0, int All = 0)
    {
        string procName = "AORTaskOptionsList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AssignedToID", SqlDbType.Int).Value = AssignedToID;
                    cmd.Parameters.Add("@PrimaryResourceID", SqlDbType.Int).Value = PrimaryResourceID;
                    cmd.Parameters.Add("@SystemID", SqlDbType.Int).Value = SystemID;
                    cmd.Parameters.Add("@SystemAffiliated", SqlDbType.Bit).Value = SystemAffiliated;
                    cmd.Parameters.Add("@ResourceAffiliated", SqlDbType.Bit).Value = ResourceAffiliated;
                    cmd.Parameters.Add("@AssignedToRankID", SqlDbType.Int).Value = AssignedToRank;
                    cmd.Parameters.Add("@All", SqlDbType.Bit).Value = All;

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

    public static DataTable AORTaskCascadeAOR_Get(int TaskID = 0)
    {
        string procName = "AORTaskCascadeAOR_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = TaskID;

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

    public static DataTable AORTaskAORHistoryList_Get(int TaskID = 0)
    {
        string procName = "AORTaskAORHistoryList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@TaskID", SqlDbType.Int).Value = TaskID;

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

    public static DataTable AORTaskSubTaskList_Get(int AORID = 0, int AORReleaseID = 0, int WorkItemID = 0, string SelectedStatuses = "", string SelectedAssigned = "")
    {
        string procName = "AORTaskSubTaskList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORID", SqlDbType.Int).Value = AORID;
                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                    cmd.Parameters.Add("@WORKITEMID", SqlDbType.Int).Value = WorkItemID;
                    cmd.Parameters.Add("@SelectedStatuses", SqlDbType.NVarChar).Value = SelectedStatuses;
                    cmd.Parameters.Add("@SelectedAssigned", SqlDbType.NVarChar).Value = SelectedAssigned;

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

    public static DataTable SprintBuilder_Get(int ProductVersionID, int ReleaseSessionID = 0, string WebsysIDs = "")
    {
        string procName = "SprintBuilder_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ProductVersionID", SqlDbType.Int).Value = ProductVersionID;
                    cmd.Parameters.Add("@ReleaseSessionID", SqlDbType.Int).Value = ReleaseSessionID;
                    cmd.Parameters.Add("@WTS_SYSTEMIDS", SqlDbType.NVarChar).Value = WebsysIDs;

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

    public static bool SprintBuilder_Save(XmlDocument SprintBuilderSave)
    {
        bool saved = false;
        string procName = "SprintBuilder_Save";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.Add("@SprintBuilder", SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(SprintBuilderSave.InnerXml, XmlNodeType.Document, null));
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out saved);
            }
        }

        return saved;
    }

    public static DataTable SprintBuilderHistory_Get(int WORKITEMID, int WORKITEM_TASKID)
    {
        string procName = "SprintBuilderHistory_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WORKITEMID", SqlDbType.Int).Value = WORKITEMID;
                    cmd.Parameters.Add("@WORKITEM_TASKID", SqlDbType.Int).Value = WORKITEM_TASKID;

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

    #region "AOR Estimation"
    public static DataTable AOREstimation_AORRelease_Get(int AORReleaseID = 0, int includeArchive = 0)
    {
        string procName = "AOREstimation_AORRelease_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;
                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            dt.Load(dr);
                            return dt;
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

    public static bool AOREstimation_Assoc_Add(int AOREst_AORReleaseID, XmlDocument Additions)
    {
        bool saved = false;
        string procName = "AOREstimation_Assoc_Add";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AOREstimation_AORReleaseID", SqlDbType.Int).Value = AOREst_AORReleaseID;
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

    public static DataTable AOREstimation_Assoc_Get(int AOREstimation_AORReleaseID = 0, int includeArchive = 0)
    {
        string procName = "AOREstimation_Assoc_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AOREstimation_AORReleaseID", SqlDbType.Int).Value = AOREstimation_AORReleaseID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;
                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            dt.Load(dr);
                            return dt;
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

    public static bool AOREstimation_Assoc_Delete(int AOREstimationAORAssocID)
    {
        bool deleted = false;
        string procName = "AOREstimation_Assoc_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AOREstimation_AORAssocID", SqlDbType.Int).Value = AOREstimationAORAssocID;
                cmd.Parameters.Add("@Exists", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@Deleted"];

                if (paramDeleted != null) bool.TryParse(paramDeleted.Value.ToString(), out deleted);
            }
        }

        return deleted;
    }

    public static bool AOREstimation_Assoc_SetPrimary(int AOREstimationAORAssocID)
    {
        bool deleted = false;
        string procName = "AOREstimation_Assoc_SetPrimary";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AOREstimation_AORAssocID", SqlDbType.Int).Value = AOREstimationAORAssocID;
                cmd.Parameters.Add("@Exists", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramSaved = cmd.Parameters["@Saved"];

                if (paramSaved != null) bool.TryParse(paramSaved.Value.ToString(), out deleted);
            }
        }

        return deleted;
    }

    public static DataTable AOREstimation_Assoc_AORList(int AOREstimation_AORReleaseID = 0, int includeArchive = 0)
    {
        string procName = "AOREstimation_Assoc_AORList";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AOREstimation_AORReleaseID", SqlDbType.Int).Value = AOREstimation_AORReleaseID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;
                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            dt.Load(dr);
                            return dt;
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

    public static bool AOREstimation_Assoc_Update(int AOREstimation_AORAssocID
        , string Notes
        , out bool duplicate
        , out string errorMsg
        )
    {
        errorMsg = string.Empty;
        duplicate = false;
        bool saved = false;

        string procName = "AOREstimation_Assoc_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@AOREstimation_AORAssocID", SqlDbType.Int).Value = AOREstimation_AORAssocID;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Notes;
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

    public static decimal AOREstimatedNetResources(int AORReleaseID)
    {
        string resources = "";
        decimal numResources = 0;

        string funcName = "SELECT dbo.AOREstimatedNetResources(@AORReleaseID)";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(funcName, cn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@AORReleaseID", AORReleaseID);

                try
                {
                    resources = cmd.ExecuteScalar().ToString();
                    decimal.TryParse(resources, out numResources);
                }
                catch (Exception ex)
                { //log exception
                    LogUtility.LogException(ex);
                }
            }
        }

        return numResources;
    }

    public static DataTable AORReleaseOverride_Get(int AORReleaseID = 0, int includeArchive = 0)
    {
        string procName = "AORReleaseOverride_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;
                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            dt.Load(dr);
                            return dt;
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

    public static DataTable AORReleaseOverrideHist_Get(int AORReleaseID = 0, int includeArchive = 0)
    {
        string procName = "AORReleaseOverrideHist_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@AORReleaseID", SqlDbType.Int).Value = AORReleaseID;
                    cmd.Parameters.Add("@IncludeArchive", SqlDbType.Int).Value = includeArchive;
                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            dt.Load(dr);
                            return dt;
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

    #region ReleaseAssessment
    public static DataTable ReleaseAssessmentList_Get()
    {
        string procName = "ReleaseAssessmentList_Get";

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

    /// <summary>
    /// Load ReleaseAssessment Item
    /// </summary>
    /// <returns>Datatable of Release Assessment Item</returns>
    public static DataTable ReleaseAssessment_Get(int ReleaseAssessmentID = 0)
    {
        string procName = "ReleaseAssessment_Get";

        using (DataTable dt = new DataTable("ReleaseAssessment"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ReleaseAssessmentID", SqlDbType.Int).Value = ReleaseAssessmentID;

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
	/// Add new ReleaseAssessment record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ReleaseAssessment_Add(
        int releaseID
        , int contractID
        , string reviewNarrative
        , bool mitigation
        , string mitigationNarrative
        , bool reviewed
        , out bool exists
        , out int newID
        , out string errorMsg)
    {
        exists = false;
        newID = 0;
        errorMsg = string.Empty;
        bool saved = false;

        string procName = "ReleaseAssessment_Add";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseID", SqlDbType.Int).Value = releaseID;
                cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                cmd.Parameters.Add("@ReviewNarrative", SqlDbType.NVarChar).Value = reviewNarrative;
                cmd.Parameters.Add("@Mitigation", SqlDbType.Bit).Value = mitigation ? 1 : 0;
                cmd.Parameters.Add("@MitigationNarrative", SqlDbType.NVarChar).Value = mitigationNarrative;
                cmd.Parameters.Add("@Reviewed", SqlDbType.Bit).Value = reviewed ? 1 : 0;
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
	/// Add new ReleaseAssessment record
	/// </summary>
	/// <param name="errorMsg"></param>
	/// <returns></returns>
	public static bool ReleaseAssessment_Update(
        int releaseAssessmentID
        , int releaseID
        , int contractID
        , string reviewNarrative
        , bool mitigation
        , string mitigationNarrative
        , bool reviewed)
    {
        bool saved = false;

        string procName = "ReleaseAssessment_Update";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseAssessmentID", SqlDbType.Int).Value = releaseAssessmentID;
                cmd.Parameters.Add("@ReleaseID", SqlDbType.Int).Value = releaseID;
                cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                cmd.Parameters.Add("@ReviewNarrative", SqlDbType.NVarChar).Value = reviewNarrative;
                cmd.Parameters.Add("@Mitigation", SqlDbType.Bit).Value = mitigation ? 1 : 0;
                cmd.Parameters.Add("@MitigationNarrative", SqlDbType.NVarChar).Value = mitigationNarrative;
                cmd.Parameters.Add("@Reviewed", SqlDbType.Bit).Value = reviewed ? 1 : 0;
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

    public static bool ReleaseAssessment_Delete(int ReleaseAssessmentID)
    {
        bool deleted = false;
        string procName = "ReleaseAssessment_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseAssessmentID", SqlDbType.Int).Value = ReleaseAssessmentID;
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

    public static DataTable ReleaseAssessment_DeploymentList_Get(int ReleaseAssessmentID)
    {
        string procName = "ReleaseAssessment_DeploymentList_Get";

        using (DataTable dt = new DataTable("Data"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ReleaseAssessmentID", SqlDbType.Int).Value = ReleaseAssessmentID;

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

    public static bool ReleaseAssessment_Deployment_Delete(int ReleaseAssessmentDeploymentID)
    {
        bool deleted = false;
        string procName = "ReleaseAssessment_Deployment_Delete";

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReleaseAssessmentDeploymentID", SqlDbType.Int).Value = ReleaseAssessmentDeploymentID;
                cmd.Parameters.Add("@Exists", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                SqlParameter paramDeleted = cmd.Parameters["@Deleted"];

                if (paramDeleted != null) bool.TryParse(paramDeleted.Value.ToString(), out deleted);
            }
        }

        return deleted;
    }
    #endregion
}