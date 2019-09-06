using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web;
using WTS.Reports;

using Newtonsoft.Json;
using System.Web.Security;

public enum FilterType
{
	WorkItem = 1,
	WorkRequest = 2,
	RequestGroup = 3
}


/// <summary>
/// Summary description for Filtering
/// </summary>
public class Filtering
{
	public static DataTable GetFilterFields(string module, string options)
	{
		try
		{
			DataTable dtFilterFields = new DataTable("FilterFields");
			dtFilterFields.Columns.Add("FIELD");
			dtFilterFields.Columns.Add("NAME");

			//temp hardcode fields
			switch (module)
			{
                case "Reports":
                    IReport report = ReportFactory.CreateReport(Int32.Parse(options));
                    Dictionary<string, string> reportFilters = report.GetFilterFields();
                    foreach (var filter in reportFilters)
                    {
                        dtFilterFields.Rows.Add(filter.Key, filter.Value);
                    }
                    break;
                case "Work":
				case "WorkRequest":
				case "Workload":
					dtFilterFields.Rows.Add("Header", "Workload"); //header
                    dtFilterFields.Rows.Add("AOR", "AOR");
                    //dtFilterFields.Rows.Add("Allocation Group", "Allocation Group");
                    //dtFilterFields.Rows.Add("Daily Meeting", "Daily Meeting");
                    //dtFilterFields.Rows.Add("Allocation Assignment", "Allocation Assignment");
                    dtFilterFields.Rows.Add("Affiliated", "Affiliated");
					dtFilterFields.Rows.Add("Workload Assigned To", "Assigned To");
					dtFilterFields.Rows.Add("Workload Assigned To (Organization)", "Assigned To (Organization)");
                    //dtFilterFields.Rows.Add("Primary Bus Resource", "Primary Bus Resource");
                    dtFilterFields.Rows.Add("Primary Resource", "Primary Resource");
                    //dtFilterFields.Rows.Add("Developer", "Developer");
					dtFilterFields.Rows.Add("Work Activity", "Work Activity");
                    //dtFilterFields.Rows.Add("Bus Rank", "Bus Rank");
                    //dtFilterFields.Rows.Add("Tech Rank", "Tech Rank");
                    dtFilterFields.Rows.Add("Customer Rank", "Customer Rank");
                    dtFilterFields.Rows.Add("Assigned To Rank", "Assigned To Rank");
                    dtFilterFields.Rows.Add("Workload Priority", "Priority");
					dtFilterFields.Rows.Add("Production Status", "Production Status");
                    //dtFilterFields.Rows.Add("PDDTDR Phase", "PDD TDR Phase");
                    dtFilterFields.Rows.Add("Release Version", "Release Version");
					dtFilterFields.Rows.Add("Workload Status", "Status");
					dtFilterFields.Rows.Add("System Suite", "System Suite");
					dtFilterFields.Rows.Add("System(Task)", "System(Task)");
                    dtFilterFields.Rows.Add("Work Area", "Work Area");
					dtFilterFields.Rows.Add("Workload Submitted By", "Workload Submitted By");
					dtFilterFields.Rows.Add("Workload Group", "Workload Group"); //Functionality
					dtFilterFields.Rows.Add("Resource Group", "Resource Group");
                    dtFilterFields.Rows.Add("SR Number", "SR Number");
                    dtFilterFields.Rows.Add("Task Created By", "Task Created By");

                    //dtFilterFields.Rows.Add("Header", "Work Request"); //header
                    //dtFilterFields.Rows.Add("Contract", "Contract");
                    //dtFilterFields.Rows.Add("Lead Resource", "Lead Resource");
                    //dtFilterFields.Rows.Add("Lead Tech Writer", "Lead Tech Writer");
                    //dtFilterFields.Rows.Add("Organization", "Organization");
                    //dtFilterFields.Rows.Add("Request Priority", "Priority");
                    //dtFilterFields.Rows.Add("Request Group", "Request Group");
                    //dtFilterFields.Rows.Add("Request Type", "Request Type");
                    //dtFilterFields.Rows.Add("Scope", "Scope");
                    //dtFilterFields.Rows.Add("SME", "SME");
                    //dtFilterFields.Rows.Add("Request Submitted By", "Submitted By");

                    break;
				case "SR":
                    dtFilterFields.Rows.Add("Header", "SR"); //header
                    dtFilterFields.Rows.Add("Submitted By", "Submitted By");
                    dtFilterFields.Rows.Add("Status", "Status");
                    dtFilterFields.Rows.Add("Reasoning", "Reasoning");
                    dtFilterFields.Rows.Add("System", "System");
                    break;
				case "BUSINESS_RULES":
					break;
                case "RQMT":
                    dtFilterFields.Rows.Add("Header", "RQMT"); //header

                    dtFilterFields.Rows.Add("Complexity", "Complexity");
                    dtFilterFields.Rows.Add("Criticality", "Criticality");
                    dtFilterFields.Rows.Add("WorkloadGroup", "Functionality");
                    dtFilterFields.Rows.Add("RQMTType", "Purpose");
                    dtFilterFields.Rows.Add("RQMTAccepted", "RQMT Accepted");
                    dtFilterFields.Rows.Add("RQMTDescriptionType", "RQMT Description Type");
                    dtFilterFields.Rows.Add("RQMTSetName", "RQMT Set Name");
                    dtFilterFields.Rows.Add("RQMTStatus", "RQMT Status");
                    dtFilterFields.Rows.Add("RQMTStage", "RQMT Stage");                                        
                    dtFilterFields.Rows.Add("System Suite", "System Suite");
                    dtFilterFields.Rows.Add("System(Task)", "System (Task)");
                    dtFilterFields.Rows.Add("Work Area", "Work Area");
                    break;
				default:
					break;
			}

			dtFilterFields.AcceptChanges();

			return dtFilterFields;

		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			return null;
		}
	}

	///// <summary>
	///// Get User Affiliations
	///// </summary>
	///// <returns></returns>
	//public static DataTable GET_AFFILIATIONS_BY_USER_ROLE(string intUserNameID, string intRole, int websysid)
	//{
	//	try
	//	{
	//		using (OracleConnection conn = new OracleConnection(FHPMCommon.ConnectionString))
	//		{
	//			using (OracleCommand cmd = new OracleCommand("CAFDEX.GET_AFFILIATIONS_BY_USER_ROLE", conn))
	//			{
	//				cmd.CommandType = CommandType.StoredProcedure;
	//				cmd.BindByName = true;

	//				cmd.Parameters.Add("p_USER_NMID", OracleDbType.Decimal).Value = intUserNameID;
	//				cmd.Parameters.Add("p_WEBSYSID", OracleDbType.Decimal).Value = websysid;
	//				cmd.Parameters.Add("p_ROLEID", OracleDbType.Varchar2).Value = intRole;
	//				cmd.Parameters.Add("cur_AFFILIATIONS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

	//				using (OracleDataAdapter da = new OracleDataAdapter(cmd))
	//				{
	//					da.MissingSchemaAction = MissingSchemaAction.Add;
	//					da.MissingMappingAction = MissingMappingAction.Passthrough;
	//					da.AcceptChangesDuringFill = true;

	//					using (DataTable dt = new DataTable())
	//					{
	//						da.Fill(dt);
	//						return dt;
	//					}
	//				}
	//			}
	//		}
	//	}
	//	catch (Exception e)
	//	{
	//		//LogUtility.LogException(e);
	//		return null;
	//	}
	//}

	///// <summary>
	///// Get User AOR Filters
	///// </summary>
	///// <returns></returns>
	//public static DataTable GET_USER_AOR_FILTERS(string intUserNameID, string websysid, string tAffiliationID = "", string intRole = "", string websysActual = "")
	//{
	//	try
	//	{
	//		using (OracleConnection conn = new OracleConnection(FHPMCommon.ConnectionString))
	//		{
	//			using (OracleCommand cmd = new OracleCommand("CAFDEX.GET_USER_AOR_FILTERS", conn))
	//			{
	//				cmd.CommandType = CommandType.StoredProcedure;
	//				cmd.BindByName = true;

	//				cmd.Parameters.Add("p_USER_NMID", OracleDbType.Decimal, 10).Value = intUserNameID;
	//				cmd.Parameters.Add("p_WEBSYSID", OracleDbType.Decimal, 10).Value = websysid;
	//				cmd.Parameters.Add("p_TBL_T_AFFILIATIONID", OracleDbType.Decimal, 10).Value = tAffiliationID;
	//				cmd.Parameters.Add("p_ROLEID", OracleDbType.Decimal, 10).Value = intRole;
	//				cmd.Parameters.Add("p_WEBSYSID_ACTUAL", OracleDbType.Decimal, 10).Value = websysActual;

	//				cmd.Parameters.Add("cur_DATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

	//				using (OracleDataAdapter da = new OracleDataAdapter(cmd))
	//				{
	//					da.MissingSchemaAction = MissingSchemaAction.Add;
	//					da.MissingMappingAction = MissingMappingAction.Passthrough;
	//					da.AcceptChangesDuringFill = true;

	//					using (DataTable dt = new DataTable())
	//					{
	//						da.Fill(dt);
	//						return dt;
	//					}
	//				}
	//			}
	//		}
	//	}
	//	catch (Exception e)
	//	{
	//		//LogUtility.LogException(e);
	//		return null;
	//	}
	//}

	///// <summary>
	///// Get User AOR Filters
	///// </summary>
	///// <returns></returns>
	//public static DataTable GET_USER_OUTSIDE_AOR_FILTERS(string intUserNameID, string websysid, string tAffiliationID = "", string intRole = "")
	//{
	//	try
	//	{
	//		using (OracleConnection conn = new OracleConnection(FHPMCommon.ConnectionString))
	//		{
	//			using (OracleCommand cmd = new OracleCommand("CAFDEX.GET_USER_OUTSIDE_AOR_FILTERS", conn))
	//			{
	//				cmd.CommandType = CommandType.StoredProcedure;
	//				cmd.BindByName = true;

	//				cmd.Parameters.Add("p_USER_NMID", OracleDbType.Decimal, 10).Value = intUserNameID;
	//				cmd.Parameters.Add("p_WEBSYSID", OracleDbType.Decimal, 10).Value = websysid;
	//				cmd.Parameters.Add("p_TBL_T_AFFILIATIONID", OracleDbType.Decimal, 10).Value = tAffiliationID;
	//				cmd.Parameters.Add("p_ROLEID", OracleDbType.Decimal, 10).Value = intRole;

	//				cmd.Parameters.Add("cur_DATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

	//				using (OracleDataAdapter da = new OracleDataAdapter(cmd))
	//				{
	//					da.MissingSchemaAction = MissingSchemaAction.Add;
	//					da.MissingMappingAction = MissingMappingAction.Passthrough;
	//					da.AcceptChangesDuringFill = true;

	//					using (DataTable dt = new DataTable())
	//					{
	//						da.Fill(dt);
	//						return dt;
	//					}
	//				}
	//			}
	//		}
	//	}
	//	catch (Exception e)
	//	{
	//		//LogUtility.LogException(e);
	//		return null;
	//	}
	//}

	/// <summary>
	/// Get list of Available Filters
	/// </summary>
	/// <returns>DataTable containing list of Filters</returns>
	public static DataSet LOAD_ROLE_FILTERS(int websysid, string intRole = "")
	{
		try
		{
			DataSet ds = null;

			return ds;
		}
		catch (Exception e)
		{
			LogUtility.LogException(e);
			return null;
		}
	}

	//public static DataTable LOAD_CUSTOM_USER_FILTER(int p_WEBSYSID)
	//{
	//	try
	//	{
	//		using (OracleConnection conn = new OracleConnection(FHPMCommon.ConnectionString))
	//		{
	//			using (OracleCommand cmd = new OracleCommand("DPEM.LOAD_CUSTOM_USER_FILTER", conn))
	//			{
	//				cmd.CommandType = CommandType.StoredProcedure;
	//				cmd.BindByName = true;

	//				cmd.Parameters.Add("p_WEBSYSID", OracleDbType.Decimal).Value = p_WEBSYSID;
	//				cmd.Parameters.Add("p_USER_NMID", OracleDbType.Decimal).Value = UserManagement.GetUserId(HttpContext.Current.User.Identity.Name);
	//				cmd.Parameters.Add("cur_CUSTOM_FILTERS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

	//				using (OracleDataAdapter da = new OracleDataAdapter(cmd))
	//				{
	//					da.MissingSchemaAction = MissingSchemaAction.Add;
	//					da.MissingMappingAction = MissingMappingAction.Passthrough;
	//					da.AcceptChangesDuringFill = true;

	//					using (DataTable dt = new DataTable())
	//					{
	//						da.Fill(dt);
	//						return dt;
	//					}
	//				}
	//			}
	//		}
	//	}
	//	catch (Exception e)
	//	{
	//		////LogUtility.LogException(e);
	//		return null;
	//	}
	//}

	//public static string DELETE_CUSTOM_USER_FILTER(string p_COLLECTION_NAME, dynamic p_FILTER_PARAM_NAME, dynamic p_FILTER_PARAM_ID, string p_TBL_FILTER_LABEL, string p_FILTER_FIELD)
	//{
	//	try
	//	{
	//		using (OracleConnection conn = new OracleConnection(FHPMCommon.ConnectionString))
	//		{
	//			conn.Open();
	//			using (OracleCommand cmd = new OracleCommand("CAFDEX.DELETE_CUSTOM_USER_FILTER", conn))
	//			{
	//				cmd.CommandType = CommandType.StoredProcedure;
	//				cmd.BindByName = true;
	//				cmd.Parameters.Add("p_WEBSYSID", OracleDbType.Decimal).Value = 184;
	//				cmd.Parameters.Add("p_AFPORTAL_LOGIN_NM", OracleDbType.Char).Value = HttpContext.Current.User.Identity.Name;
	//				cmd.Parameters.Add("p_COLLECTION_NAME", OracleDbType.Varchar2).Value = p_COLLECTION_NAME;
	//				cmd.Parameters.Add("p_FILTER_PARAM_NAME", OracleDbType.Varchar2).Value = p_FILTER_PARAM_NAME.text;
	//				cmd.Parameters.Add("p_FILTER_PARAM_ID", OracleDbType.Varchar2).Value = p_FILTER_PARAM_ID.value;
	//				cmd.Parameters.Add("p_TBL_FILTER_LABEL", OracleDbType.Varchar2).Value = p_TBL_FILTER_LABEL;
	//				cmd.Parameters.Add("p_FILTER_FIELD", OracleDbType.Varchar2).Value = p_FILTER_FIELD;

	//				cmd.ExecuteNonQuery();
	//				conn.Close();
	//				return string.Empty;
	//			}
	//		}
	//	}
	//	catch (Exception ex)
	//	{
	//		////LogUtility.LogException(ex);
	//		return "error";
	//	}
	//}

	//public static string SAVE_CUSTOM_USER_FILTER(string p_COLLECTION_NAME, dynamic p_FILTER_PARAM_NAME, dynamic p_FILTER_PARAM_ID, string p_TBL_FILTER_LABEL, string p_FILTER_FIELD)
	//{
	//	try
	//	{
	//		using (OracleConnection conn = new OracleConnection(FHPMCommon.ConnectionString))
	//		{
	//			conn.Open();
	//			using (OracleCommand cmd = new OracleCommand("DPEM.SAVE_CUSTOM_USER_FILTER", conn))
	//			{
	//				cmd.CommandType = CommandType.StoredProcedure;
	//				cmd.BindByName = true;
	//				cmd.Parameters.Add("p_WEBSYSID", OracleDbType.Decimal).Value = 184;
	//				cmd.Parameters.Add("p_AFPORTAL_LOGIN_NM", OracleDbType.Char).Value = HttpContext.Current.User.Identity.Name;
	//				cmd.Parameters.Add("p_COLLECTION_NAME", OracleDbType.Varchar2).Value = p_COLLECTION_NAME;
	//				cmd.Parameters.Add("p_FILTER_PARAM_NAME", OracleDbType.Varchar2).Value = p_FILTER_PARAM_NAME.text;
	//				cmd.Parameters.Add("p_FILTER_PARAM_ID", OracleDbType.Varchar2).Value = p_FILTER_PARAM_ID.value;
	//				cmd.Parameters.Add("p_TBL_FILTER_LABEL", OracleDbType.Varchar2).Value = p_TBL_FILTER_LABEL;
	//				cmd.Parameters.Add("p_FILTER_FIELD", OracleDbType.Varchar2).Value = p_FILTER_FIELD;

	//				cmd.ExecuteNonQuery();
	//				conn.Close();
	//				return string.Empty;
	//			}
	//		}
	//	}
	//	catch (Exception ex)
	//	{
	//		////LogUtility.LogException(ex);
	//		return "error";
	//	}
	//}


	public static DataTable LOAD_FILTER_PARAMS(string module, string filterName, int filterType, dynamic filters, bool myData)
	{
		try
		{
			DataTable dtFilters = new DataTable();

			switch (module)
			{
                case "Reports":
                    dtFilters = ReportFilterData(filterName: filterName, filterType: filterType, fields: filters, myData: myData);
                    break;
                case "Work":
				case "WorkRequest":
				case "Workload":
					dtFilters = GetWorkFilters(filterName: filterName, filterType: 1, fields: filters, myData: myData);
					break;
				case "SR":
					dtFilters = GetSRFilters(filterName: filterName, filterType: 1, fields: filters, myData: myData);
                    break;
				case "MasterData":
					dtFilters = null;
					break;
				case "BUSINESS_RULES":
					dtFilters = GetBUSINESS_RULES_Filters(filterName, filterType, filters);
					break;
                case "RQMT":
                    dtFilters = GetRQMTFilters(filterName: filterName, fields: filters, myData: myData);
                    break;
				default:
					break;
			}
			return dtFilters;
		}
		catch (Exception ex)
		{
			////LogUtility.LogException(ex);
			return null;
		}
	}

    public static DataTable ReportFilterData(string filterName, int filterType, dynamic fields, bool myData = false)
    {
        string procName = "CRReportFilterData_Get";

        if (filterType == (int)WTS.Enums.FilterTypeEnum.TaskReport) procName = "TaskReportFilterData_Get";

        //existing filters - parse dynamic fields into SQL Parameter objects
        SqlParameter[] sps = GetReportFilter_SqlParamsArray(fields, filterType, filterName);

        using (DataTable dt = new DataTable("Param"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    //cmd.CommandTimeout = 600000; //testing
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FilterName", SqlDbType.NVarChar).Value = filterName;

                    //existing filters
                    if (sps != null)
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

    public static DataTable GetWorkFilters(string filterName, int filterType, dynamic fields, bool myData = false)
	{
		string procName = "FilterParamList_Get_Work";

		//existing filters - parse dynamic fields into SQL Parameter objects
		SqlParameter[] sps = GetWorkFilter_SqlParamsArray(fields, filterName);

		using (DataTable dt = new DataTable("Param"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
                    //cmd.CommandTimeout = 600000; //testing
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
					cmd.Parameters.Add("@FilterName", SqlDbType.NVarChar).Value = filterName;
					cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = filterType;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.Int).Value = myData ? UserManagement.GetUserId_FromUsername() : (object)DBNull.Value;
					
					//existing filters
					cmd.Parameters.AddRange(sps);

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

    public static DataTable GetSRFilters(string filterName, int filterType, dynamic fields, bool myData = false)
    {
        string procName = "FilterParamList_Get_SR";

        //existing filters - parse dynamic fields into SQL Parameter objects
        SqlParameter[] sps = GetSRFilter_SqlParamsArray(fields, filterName);

        using (DataTable dt = new DataTable("Param"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    //cmd.CommandTimeout = 600000; //testing
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = HttpContext.Current.Session.SessionID;
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
                    cmd.Parameters.Add("@FilterName", SqlDbType.NVarChar).Value = filterName;
                    cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = filterType;
                    cmd.Parameters.Add("@OwnedBy", SqlDbType.Int).Value = myData ? UserManagement.GetUserId_FromUsername() : (object)DBNull.Value;

                    //existing filters
                    cmd.Parameters.AddRange(sps);

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

    public static DataTable GetRQMTFilters(string filterName, dynamic fields, bool myData = false)
    {
        string procName = "RQMTFilterData_Get";

        //existing filters - parse dynamic fields into SQL Parameter objects
        SqlParameter[] sps = GetRQMTFilter_SqlParamsArray(fields, filterName);

        using (DataTable dt = new DataTable("Param"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    //cmd.CommandTimeout = 600000; //testing
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FilterName", SqlDbType.NVarChar).Value = filterName;

                    //existing filters
                    if (sps != null)
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


    public static DataTable GetCustomFilters(string collectionName = "", string module = "")
	{
		string procName = "User_Filters_Custom_Get";

		using (DataTable dt = new DataTable("Param"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
					cmd.Parameters.Add("@CollectionName", SqlDbType.NVarChar).Value = collectionName;
					cmd.Parameters.Add("@Module", SqlDbType.NVarChar).Value = module;

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


    #region Sql Parameters
    public static SqlParameter[] GetRQMTFilter_SqlParamsArray(dynamic fields, string filterName)
    {
        List<SqlParameter> spList = GetRQMTFilter_SqlParamsList(fields, filterName);
        if (spList != null && spList.Count > 0)
        {
            return spList.ToArray();
        }
        else
        {
            return null;
        }
    }

    public static SqlParameter[] GetReportFilter_SqlParamsArray(dynamic fields, int filterType, string filterName)
    {
        List<SqlParameter> spList = GetReportFilter_SqlParamsList(fields, filterType, filterName);
        if (spList != null && spList.Count > 0)
        {
            return spList.ToArray();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
	/// parse dynamic fileds into SQL Parameter objects
	///  - expects Work filter fields
	/// </summary>
	/// <param name="fields"></param>
	/// <param name="fieldName"></param>
	/// <returns>List of SqlParameter objects</returns>
	public static List<SqlParameter> GetReportFilter_SqlParamsList(dynamic fields, int filterType, string filterName)
    {
        if (fields == null || fields.Count == 0)
        {
            return new List<SqlParameter>();
        }

        List<SqlParameter> spList = new List<SqlParameter>();
        SqlParameter sp;

        if (filterType == (int)WTS.Enums.FilterTypeEnum.TaskReport)
        {
            sp = new SqlParameter("@AOR", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("AOR") && filterName != "AOR" ? fields["AOR"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@ProductVersion", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Product Version") && filterName != "Product Version" ? fields["Product Version"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@Resource", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Resource") && filterName != "Resource" ? fields["Resource"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@Status", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Status") && filterName != "Status" ? fields["Status"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@System", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("System(Task)") && filterName != "System(Task)" ? fields["System(Task)"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@WorkArea", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Work Area") && filterName != "Work Area" ? fields["Work Area"].value.ToString() : "";
            spList.Add(sp);
        }
        else if (filterType == (int)WTS.Enums.FilterTypeEnum.DSEReport)
        {
            sp = new SqlParameter("@Release", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Release Version") && filterName != "Release Version" ? fields["Release Version"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@AORType", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("AOR Workload Type") && filterName != "AOR Workload Type" ? fields["AOR Workload Type"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@VisibleToCustomer", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Visible To Customer") && filterName != "Visible To Customer" ? fields["Visible To Customer"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@Contract", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Contract") && filterName != "Contract" ? fields["Contract"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@SystemSuite", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("System Suite") && filterName != "System Suite" ? fields["System Suite"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@Deliverable", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Deployment") && filterName != "Deployment" ? fields["Deployment"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@WorkloadAllocation", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Workload Allocation") && filterName != "Workload Allocation" ? fields["Workload Allocation"].value.ToString() : "";
            spList.Add(sp);
        }
        else
        {
            sp = new SqlParameter("@Release", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Release Version") && filterName != "Release Version" ? fields["Release Version"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@AORType", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("AOR Workload Type") && filterName != "AOR Workload Type" ? fields["AOR Workload Type"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@VisibleToCustomer", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Visible To Customer") && filterName != "Visible To Customer" ? fields["Visible To Customer"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@Contract", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Contract") && filterName != "Contract" ? fields["Contract"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@Deliverable", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Deployment") && filterName != "Deployment" ? fields["Deployment"].value.ToString() : "";
            spList.Add(sp);
            sp = new SqlParameter("@WorkloadAllocation", SqlDbType.NVarChar);
            sp.Value = fields.ContainsKey("Workload Allocation") && filterName != "Workload Allocation" ? fields["Workload Allocation"].value.ToString() : "";
            spList.Add(sp);
        }

        return spList;
    }

    public static SqlParameter[] GetReportView_SqlParamsArray(dynamic fields)
    {
        List<SqlParameter> spList = GetReportView_SqlParamsList(fields);
        if (spList != null && spList.Count > 0)
        {
            return spList.ToArray();
        }
        else
        {
            return null;
        }
    }
    public static List<SqlParameter> GetReportView_SqlParamsList(dynamic fields)
    {
        if (fields == null || fields.Count == 0)
        {
            return new List<SqlParameter>();
        }

        List<SqlParameter> spList = new List<SqlParameter>();
        SqlParameter sp;
        sp = new SqlParameter("@Type", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Type") ? fields["Type"].ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@Title", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Title") ? fields["Title"].ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@Release", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Release Version") ? fields["Release Version"].ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@AORType", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("AOR Workload Type") ? fields["AOR Workload Type"].ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@VisibleToCustomer", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Visible To Customer") ? fields["Visible To Customer"].ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@Contract", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Contract") ? fields["Contract"].ToString() : "";
        spList.Add(sp);
        //sp = new SqlParameter("@Status", SqlDbType.NVarChar);
        //sp.Value = fields.ContainsKey("Status") ? fields["Status"].ToString() : "";
        //spList.Add(sp);
        sp = new SqlParameter("@WorkloadAllocation", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Workload Allocation") ? fields["Workload Allocation"].ToString() : "";
        spList.Add(sp);

        return spList;
    }

    /// <summary>
	/// parse dynamic fileds into SQL Parameter objects
	///  - expects Work filter fields
	/// </summary>
	/// <param name="fields"></param>
	/// <param name="fieldName"></param>
	/// <returns>List of SqlParameter objects</returns>
	public static List<SqlParameter> GetRQMTFilter_SqlParamsList(dynamic fields, string filterName)
    {
        if (fields == null || fields.Count == 0)
        {
            return new List<SqlParameter>();
        }

        List<SqlParameter> spList = new List<SqlParameter>();
        SqlParameter sp;

        sp = new SqlParameter("@WTS_SYSTEM_SUITE", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("System Suite") && filterName != "System Suite" ? fields["System Suite"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@WTS_SYSTEM", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("System(Task)") && filterName != "System(Task)" ? fields["System(Task)"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@WorkArea", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Work Area") && filterName != "Work Area" ? fields["Work Area"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@RQMTType", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("RQMTType") && filterName != "RQMTType" ? fields["RQMTType"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@RQMTDescriptionType", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("RQMTDescriptionType") && filterName != "RQMTDescriptionType" ? fields["RQMTDescriptionType"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@RQMTSetName", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("RQMTSetName") && filterName != "RQMTSetName" ? fields["RQMTSetName"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@WorkloadGroup", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("WorkloadGroup") && filterName != "WorkloadGroup" ? fields["WorkloadGroup"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@Complexity", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Complexity") && filterName != "Complexity" ? fields["Complexity"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@RQMTStatus", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("RQMTStatus") && filterName != "RQMTStatus" ? fields["RQMTStatus"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@RQMTStage", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("RQMTStage") && filterName != "RQMTStage" ? fields["RQMTStage"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@Criticality", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Criticality") && filterName != "Criticality" ? fields["Criticality"].value.ToString() : "";
        spList.Add(sp);

        sp = new SqlParameter("@RQMTAccepted", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("RQMTAccepted") && filterName != "RQMTAccepted" ? fields["RQMTAccepted"].value.ToString() : "";
        spList.Add(sp);

        return spList;
    }


    public static SqlParameter[] GetWorkFilter_SqlParamsArray(dynamic fields, string filterName)
	{
		List<SqlParameter> spList = GetWorkFilter_SqlParamsList(fields, filterName);
		if (spList != null && spList.Count > 0)
		{
			return spList.ToArray();
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// parse dynamic fileds into SQL Parameter objects
	///  - expects Work filter fields
	/// </summary>
	/// <param name="fields"></param>
	/// <param name="fieldName"></param>
	/// <returns>List of SqlParameter objects</returns>
	public static List<SqlParameter> GetWorkFilter_SqlParamsList(dynamic fields, string filterName)
	{
		if (fields == null || ((Dictionary<string, object>)fields).Count == 0)
		{
			return new List<SqlParameter>();
		}

		List<SqlParameter> spList = new List<SqlParameter>();
		SqlParameter sp;
		sp = new SqlParameter("@WTS_SYSTEM", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("System(Task)") && filterName != "System(Task)" ? fields["System(Task)"].value.ToString() : "";
		spList.Add(sp);
        sp = new SqlParameter("@WTS_SYSTEM_SUITE", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("System Suite") && filterName != "System Suite" ? fields["System Suite"].value.ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@AllocationGroup", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Allocation Group") && filterName != "Allocation Group" ? fields["Allocation Group"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@DailyMeeting", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Daily Meeting") && filterName != "Daily Meeting" ? fields["Daily Meeting"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@Allocation", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Allocation Assignment") && filterName != "Allocation Assignment" ? fields["Allocation Assignment"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@WorkType", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Resource Group") ? fields["Resource Group"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@WorkItemType", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Work Activity") ? fields["Work Activity"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@WorkloadGroup", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Workload Group") ? fields["Workload Group"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@WorkArea", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Work Area") ? fields["Work Area"].value.ToString() : "";
		spList.Add(sp);		
		sp = new SqlParameter("@ProductVersion", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Release Version") && filterName != "Release Version" ? fields["Release Version"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@ProductionStatus", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Production Status") && filterName != "Production Status" ? fields["Production Status"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@Priority", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Workload Priority") && filterName != "Workload Priority" ? fields["Workload Priority"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@WorkItemSubmittedBy", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Workload Submitted By") && filterName != "Workload Submitted By" ? fields["Workload Submitted By"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@Affiliated", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Affiliated") && filterName != "Affiliated" ? fields["Affiliated"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@AssignedResource", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Workload Assigned To") && filterName != "Workload Assigned To" ? fields["Workload Assigned To"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@AssignedOrganization", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Workload Assigned To (Organization)") && filterName != "Workload Assigned To (Organization)" ? fields["Workload Assigned To (Organization)"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@PrimaryResource", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Developer") && filterName != "Developer" ? fields["Developer"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@Workload_Status", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Workload Status") && filterName != "Workload Status" ? fields["Workload Status"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@WorkRequest", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("WorkRequest") && filterName != "WorkRequest" ? fields["WorkRequest"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@RequestGroup", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Request Group") && filterName != "Request Group" ? fields["Request Group"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@Contract", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Contract") && filterName != "Contract" ? fields["Contract"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@Organization", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Organization") && filterName != "Organization" ? fields["Organization"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@RequestType", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Request Type") && filterName != "Request Type" ? fields["Request Type"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@Scope", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Scope") && filterName != "Scope" ? fields["Scope"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@RequestPriority", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Request Priority") && filterName != "Request Priority" ? fields["Request Priority"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@SME", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("SME") && filterName != "SME" ? fields["SME"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@LEAD_IA_TW", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Lead Tech Writer") && filterName != "Lead Tech Writer" ? fields["Lead Tech Writer"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@LEAD_RESOURCE", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Lead Resource") && filterName != "Lead Resource" ? fields["Lead Resource"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@PDDTDR_PHASE", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("PDDTDR Phase") && filterName != "PDDTDR Phase" ? fields["PDDTDR Phase"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@SUBMITTEDBY", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("Request Submitted By") && filterName != "Request Submitted By" ? fields["Request Submitted By"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@TaskNumber_Search", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("PRIMARY TASK NUMBER Contains") && filterName != "PRIMARY TASK NUMBER Contains" ? fields["PRIMARY TASK NUMBER Contains"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@RequestNumber_Search", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("REQUEST NUMBER Contains") && filterName != "REQUEST NUMBER Contains" ? fields["REQUEST NUMBER Contains"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@ItemTitleDescription_Search", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("ITEM TITLE/DESCRIPTION Contains") && filterName != "ITEM TITLE/DESCRIPTION Contains" ? fields["ITEM TITLE/DESCRIPTION Contains"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@Request_Search", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("REQUEST Contains") && filterName != "REQUEST Contains" ? fields["REQUEST Contains"].value.ToString() : "";
		spList.Add(sp);
		sp = new SqlParameter("@RequestGroup_Search", SqlDbType.NVarChar);
		sp.Value = fields.ContainsKey("REQUEST GROUP Contains") && filterName != "REQUEST GROUP Contains" ? fields["REQUEST GROUP Contains"].value.ToString() : "";
		spList.Add(sp);
        sp = new SqlParameter("@SRNumber_Search", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("SR NUMBER Contains") && filterName != "SR NUMBER Contains" ? fields["SR NUMBER Contains"].value.ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@SRNumber", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("SR Number") && filterName != "SR Number" ? fields["SR Number"].value.ToString() : "";
        spList.Add(sp);
        //sp = new SqlParameter("@PrimaryBusResource", SqlDbType.NVarChar);
        //sp.Value = fields.ContainsKey("Primary Bus Resource") && filterName != "Primary Bus Resource" ? fields["Primary Bus Resource"].value.ToString() : "";
        //spList.Add(sp);
        sp = new SqlParameter("@PrimaryTechResource", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Primary Resource") && filterName != "Primary Resource" ? fields["Primary Resource"].value.ToString() : "";
        spList.Add(sp);
        //sp = new SqlParameter("@PrimaryBusRank", SqlDbType.NVarChar);
        //sp.Value = fields.ContainsKey("Bus Rank") && filterName != "Bus Rank" ? fields["Bus Rank"].value.ToString() : "";
        //spList.Add(sp);
        //sp = new SqlParameter("@PrimaryTechRank", SqlDbType.NVarChar);
        //sp.Value = fields.ContainsKey("Tech Rank") && filterName != "Tech Rank" ? fields["Tech Rank"].value.ToString() : "";
        //spList.Add(sp);
        sp = new SqlParameter("@PrimaryBusRank", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Customer Rank") && filterName != "Customer Rank" ? fields["Customer Rank"].value.ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@AssignedToRank", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Assigned To Rank") && filterName != "Assigned To Rank" ? fields["Assigned To Rank"].value.ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@AOR", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("AOR") && filterName != "AOR" ? fields["AOR"].value.ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@TaskCreatedBy", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Task Created By") && filterName != "Task Created By" ? fields["Task Created By"].value.ToString() : "";
        spList.Add(sp);

        return spList;
	}

	/// <summary>
	/// parse dynamic fields into SQL Parameter objects
	///  - expects Work filter fields
	/// </summary>
	/// <param name="fields">dynamic dictionary of filter fields</param>
	/// <param name="filterName">for progressive filtering</param>
	/// <returns>Dictionary of ParamName, SqlParam objects</returns>
	public static Dictionary<string, SqlParameter> GetWorkFilter_SqlParams(dynamic fields, string filterName = "")
	{
		if (fields == null || ((Dictionary<string,object>)fields).Count == 0)
		{
			return null;
		}

		Dictionary<string, SqlParameter> spCollection = new Dictionary<string, SqlParameter>();
		List<SqlParameter> spList = GetWorkFilter_SqlParamsList(fields, filterName);
		foreach (SqlParameter sp in spList)
		{
			spCollection.Add(sp.ParameterName, sp);
		}

		return spCollection;
	}

	/// <summary>
	/// Merge two sets of dynamic filter fields into single List of SqlParameters
	///  - existing filters will be retained unless new set has a value for specified parameter
	/// </summary>
	/// <param name="existingFilters"></param>
	/// <param name="newFilters"></param>
	/// <param name="filterName"></param>
	/// <returns>List of SqlParameter objects</returns>
	public static List<SqlParameter> MergeWorkFilter_SqlParams(dynamic existingFilters, dynamic newFilters, string filterName = "")
	{
		if (newFilters == null || ((Dictionary<string, object>)newFilters).Count == 0)
		{
			return GetWorkFilter_SqlParamsList(existingFilters, filterName);
		}

		if (existingFilters == null || ((Dictionary<string, object>)existingFilters).Count == 0)
		{
			return GetWorkFilter_SqlParamsList(newFilters, "");
		}

		Dictionary<string, SqlParameter> existingParams = GetWorkFilter_SqlParams(existingFilters, filterName);
		Dictionary<string, SqlParameter> newParams = GetWorkFilter_SqlParams(newFilters, "");

		return MergeWorkFilter_SqlParams(existingParams, newParams);
	}

	/// <summary>
	/// Merge existing and new sets of filters into single list
	///	 - existing filters will be retained if not in new set
	///  - new filters will overwrite existing if the new filter has a value for the specified parameter
	/// </summary>
	/// <param name="existingFilters"></param>
	/// <param name="newFilters"></param>
	/// <returns>List of SqlParam objects</returns>
	public static List<SqlParameter> MergeWorkFilter_SqlParams(Dictionary<string, SqlParameter> existingFilters, Dictionary<string, SqlParameter> newFilters)
	{
		List<SqlParameter> mergedFilters = new List<SqlParameter>();
		
		foreach (string key in existingFilters.Keys)
		{
			if (!newFilters.ContainsKey(key))
			{
				newFilters.Add(key, existingFilters[key]);
			}
			else
			{
				if (string.IsNullOrWhiteSpace(newFilters[key].Value.ToString()))
				{
					newFilters[key].Value = existingFilters[key].Value;
				}
			}
		}

		foreach (SqlParameter sp in newFilters.Values)
		{
			mergedFilters.Add(sp);
		}

		return mergedFilters;
	}

    public static SqlParameter[] GetSRFilter_SqlParamsArray(dynamic fields, string filterName)
    {
        List<SqlParameter> spList = GetSRFilter_SqlParamsList(fields, filterName);
        if (spList != null && spList.Count > 0)
        {
            return spList.ToArray();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// parse dynamic fileds into SQL Parameter objects
    ///  - expects Work filter fields
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="fieldName"></param>
    /// <returns>List of SqlParameter objects</returns>
    public static List<SqlParameter> GetSRFilter_SqlParamsList(dynamic fields, string filterName)
    {
        if (fields == null || ((Dictionary<string, object>)fields).Count == 0)
        {
            return new List<SqlParameter>();
        }

        List<SqlParameter> spList = new List<SqlParameter>();
        SqlParameter sp;
        sp = new SqlParameter("@SubmittedBy", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Submitted By") && filterName != "Submitted By" ? fields["Submitted By"].text.ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@Status", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Status") && filterName != "Status" ? fields["Status"].value.ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@Reasoning", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("Reasoning") && filterName != "Reasoning" ? fields["Reasoning"].value.ToString() : "";
        spList.Add(sp);
        sp = new SqlParameter("@System", SqlDbType.NVarChar);
        sp.Value = fields.ContainsKey("System") && filterName != "System" ? fields["System"].text.ToString() : "";
        spList.Add(sp);

        return spList;
    }

    #endregion Sql Parameters


    /// <summary>
    /// Get list of Filters Parameters
    /// </summary>
    /// <returns>DataTable containing list of Filters</returns>
    public static DataTable GetBUSINESS_RULES_Filters(string p_FILTER_NAME, int p_FILTER_TYPE, dynamic fields)
	{
		return new DataTable();
		try
		{
			//using (OracleConnection conn = new OracleConnection(FHPMCommon.ConnectionString))
			//{
			//	using (OracleCommand cmd = new OracleCommand("FHPM.LOAD_BIZ_RULES_PARAMS", conn))
			//	{
			//		cmd.CommandType = CommandType.StoredProcedure;
			//		cmd.BindByName = true;

			//		cmd.Parameters.Add("P_SESSIONID", OracleDbType.Varchar2).Value = HttpContext.Current.Session.SessionID;
			//		cmd.Parameters.Add("P_USERNAME", OracleDbType.Varchar2).Value = HttpContext.Current.User.Identity.Name;
			//		cmd.Parameters.Add("P_FILTER_NAME", OracleDbType.Varchar2).Value = p_FILTER_NAME;
			//		cmd.Parameters.Add("p_FILTER_TYPE", OracleDbType.Decimal, 10).Value = p_FILTER_TYPE;

			//		cmd.Parameters.Add("p_RULEID", OracleDbType.Varchar2).Value = fields.ContainsKey("RULE") && p_FILTER_NAME != "RULE" ? fields["RULE"].value : "";
			//		cmd.Parameters.Add("p_GROUPID", OracleDbType.Varchar2).Value = fields.ContainsKey("MODULE") && p_FILTER_NAME != "MODULE" ? fields["MODULE"].value : "";
			//		cmd.Parameters.Add("p_CATEGORYID", OracleDbType.Varchar2).Value = fields.ContainsKey("CATEGORY") && p_FILTER_NAME != "CATEGORY" ? fields["CATEGORY"].value : "";

			//		cmd.Parameters.Add("cur_FILTERS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

			//		using (OracleDataAdapter da = new OracleDataAdapter(cmd))
			//		{
			//			da.MissingSchemaAction = MissingSchemaAction.Add;
			//			da.MissingMappingAction = MissingMappingAction.Passthrough;
			//			da.AcceptChangesDuringFill = true;

			//			using (DataTable dt = new DataTable())
			//			{
			//				da.Fill(dt);
			//				return dt;
			//			}
			//		}
			//	}
			//}
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static string APPLY_USER_FILTER(string module, string filterModule, dynamic filters, bool myData, String xml)
	{
		try
		{
			switch (module)
			{
				case "WorkRequest":
				case "Workload":
					return SaveWorkFilters(module, filterModule, filters, myData, xml);
				default:
					return "";
			}
		}
		catch (Exception ex)
		{
			////LogUtility.LogException(ex);
			return "";
		}
	}

	public static bool SaveWorkFilters(string module, string filterModule, dynamic filters, bool myData, String xml)
	{
		bool saved = false;
		string msg = string.Empty;

		try
		{
            SqlParameter[] sps = null;
            string procName = null;

            if (module == "RQMT")
            {
                sps = Filtering.GetRQMTFilter_SqlParamsArray(filters, "");
                procName = "RQMTFilterSession_Set";
            }
            else
            {
                sps = Filtering.GetWorkFilter_SqlParamsArray(filters, "");
                procName = "Save_User_Filters_WorkItem";
            }

			string username = HttpContext.Current.User.Identity.Name;
			string sessionid = HttpContext.Current.Session.SessionID;
            
			using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(procName, conn))
				{
                    cmd.CommandTimeout = 600000; //testing
                    cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = sessionid;
					cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = username;
					cmd.Parameters.Add("@FilterTypeID", SqlDbType.Int).Value = 1;

					//existing filters
					if (filters != null && sps != null && sps.Length > 0)
					{
						cmd.Parameters.AddRange(sps);
					}
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
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		return saved;
	}

    public static bool SaveCustomFilters(string collectionName, bool deleteFilter, string module, dynamic filters)
	{
		bool saved = false;
		string msg = string.Empty;

		try
		{
            SqlParameter[] sps = null;

            if (module == "RQMT")
            {
                sps = Filtering.GetRQMTFilter_SqlParamsArray(filters, "");
            }
            else
            {
                sps = Filtering.GetWorkFilter_SqlParamsArray(filters, "");
            }

			string username = HttpContext.Current.User.Identity.Name;

			string procName = "Save_User_Filters_Custom";
			using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(procName, conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = username;
					cmd.Parameters.Add("@CollectionName", SqlDbType.NVarChar).Value = collectionName;
					cmd.Parameters.Add("@Module", SqlDbType.NVarChar).Value = module;
					cmd.Parameters.Add("@DeleteFilter", SqlDbType.Bit).Value = deleteFilter ? 1 : 0;

					//existing filters
					if (filters != null && sps != null && sps.Length > 0)
					{
						cmd.Parameters.AddRange(sps);
					}
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
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		return saved;
	}

    public static bool CheckReportViewExist(int WTS_RESOURCEID, int REPORT_TYPEID, string viewName)
    {
        bool exists = false;

        try
        {

            string username = HttpContext.Current.User.Identity.Name;
            string procName = "Check_User_Reports_Exists";
            using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.NVarChar).Value = WTS_RESOURCEID;
                    cmd.Parameters.Add("@ViewName", SqlDbType.NVarChar).Value = viewName;
                    cmd.Parameters.Add("@ReportTypeID", SqlDbType.NVarChar).Value = REPORT_TYPEID;

                    cmd.Parameters.Add("@exists", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    SqlParameter paramExists = cmd.Parameters["@exists"];
                    if (paramExists != null)
                    {
                        bool.TryParse(paramExists.Value.ToString(), out exists);
                    }
                }
            }

            return exists;

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            exists = false;
        }

        return exists;
    }

    public static int SaveCustomReportView(int WTS_RESOURCEID, int ReportViewID, int REPORT_TYPEID, string viewName, Dictionary<string, string> reportParameters, string reportLevels)
    {
        int saved = -1;
        //string msg = string.Empty;

        try
        {
            //dynamic fields = reportParameters;
            //SqlParameter[] sps = GetReportView_SqlParamsArray(fields);

            //string username = HttpContext.Current.User.Identity.Name;

            string procName = "User_Reports_View_Add";
            using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@WTS_RESOURCEID", SqlDbType.NVarChar).Value = WTS_RESOURCEID;
                    cmd.Parameters.Add("@ReportViewID", SqlDbType.NVarChar).Value = ReportViewID;
                    cmd.Parameters.Add("@ViewName", SqlDbType.NVarChar).Value = viewName;
                    cmd.Parameters.Add("@ReportTypeID", SqlDbType.NVarChar).Value = REPORT_TYPEID;
                    cmd.Parameters.Add("@ReportParameters", SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(reportParameters, Formatting.None);
                    cmd.Parameters.Add("@ReportLevels", SqlDbType.NVarChar).Value = reportLevels;
                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

                    //existing filters
                    //if (reportParameters != null && sps != null && sps.Length > 0)
                    //{
                    //    cmd.Parameters.AddRange(sps);
                    //}
                    cmd.Parameters.Add("@savedID", SqlDbType.Int).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    SqlParameter paramSaved = cmd.Parameters["@savedID"];
                    if (paramSaved != null)
                    {
                        int.TryParse(paramSaved.Value.ToString(), out saved);
                    }
                }
            }

            return saved;

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            saved = -1;
        }

        return saved;
    }

    public static DataTable LoadReportViews(int ViewID, int ReportTypeID)
    {
        string procName = "User_Reports_View_Get";

        var loggedInMembershipUser = Membership.GetUser();
        var loggedInMembershipUserId = loggedInMembershipUser.ProviderUserKey.ToString();

        WTS_User loggedInUser = new WTS_User();
        loggedInUser.Load(loggedInMembershipUserId);

        using (DataTable dt = new DataTable("Param"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.NVarChar).Value = loggedInUser.ID;
                    cmd.Parameters.Add("@ReportViewID", SqlDbType.NVarChar).Value = ViewID;
                    cmd.Parameters.Add("@ReportTypeID", SqlDbType.NVarChar).Value = ReportTypeID;

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

    public static bool DeleteReportView(int viewID, int ReportTypeID, out string errorMsg)
    {
        errorMsg = string.Empty;
        bool deleted = false;

        string procName = "User_Reports_View_Delete";

        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReportViewID", SqlDbType.Int).Value = viewID;
                cmd.Parameters.Add("@ReportTypeID", SqlDbType.Int).Value = ReportTypeID;
                cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;
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

    public static bool CleanUserFilters(string sessionid = "", string username = "", bool purge = false)
	{
		bool deleted = false;
		string msg = string.Empty;

		string procName = "Clean_User_Filters";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = sessionid;
				cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = username;
				cmd.Parameters.Add("@purge", SqlDbType.Bit).Value = purge ? 1 : 0;

				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
					cmd.ExecuteNonQuery();

					SqlParameter paramSaved = cmd.Parameters["@deleted"];
					if (paramSaved != null)
					{
						bool.TryParse(paramSaved.Value.ToString(), out deleted);
					}
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
				}
			}
		}

		return deleted;
	}

	public static bool CleanUserFilters()
	{
		bool deleted = false;
		string msg = string.Empty;

		string procName = "Clean_User_Filters";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				
				cmd.Parameters.Add("@deleted", SqlDbType.Bit).Direction = ParameterDirection.Output;

				try
				{
					cmd.ExecuteNonQuery();

					SqlParameter paramSaved = cmd.Parameters["@deleted"];
					if (paramSaved != null)
					{
						bool.TryParse(paramSaved.Value.ToString(), out deleted);
					}
				}
				catch (Exception ex)
				{
					LogUtility.LogException(ex);
				}
			}
		}

		return deleted;
	}

	public static DataTable GetDefaultSystemFilters()
	{
		DataTable dt = new DataTable();



		return dt;
	}
}

public class FilterObject
{
	public string text = string.Empty;
	public string value = string.Empty;

	public FilterObject(string textString = "", string valueString = "")
	{
		this.text = textString;
		this.value = valueString;
	}
    public FilterObject() { }
}