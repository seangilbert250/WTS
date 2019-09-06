using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;


/// <summary>
/// Basic Data access functionality
/// </summary>
public sealed class WTSData
{
    public static bool ExecuteStoredProcedure(string procedureName, params SqlParameter[] parameters)
    {
        bool success = false;

        try
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(procedureName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    cmd.ExecuteNonQuery();

                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            if (p.Direction != ParameterDirection.Input)
                            {
                                p.Value = cmd.Parameters[p.ParameterName].Value;
                            }
                        }
                    }

                    success = true;
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            throw;
        }

        return success;
    }

	public static DataTable GetGeneric(string connection, string sql, params SqlParameter[] parameters)
	{
		//Prepare the connection and DataAdapter.
		using (SqlConnection conn = new SqlConnection(connection))  // WTSCommon.ConnectionString
        {
			try
			{
				using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
				{
					//Set up the DataAdapter.
					da.MissingSchemaAction = MissingSchemaAction.Add;
					da.MissingMappingAction = MissingMappingAction.Passthrough;
					da.AcceptChangesDuringFill = true;
					if (parameters != null)
					{
						da.SelectCommand.Parameters.AddRange(parameters);
					}

					//Get the data.
					DataTable data = new DataTable("GENERIC");
					da.Fill(data);
					return data;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
	}

    public static DataTable GetDataTableFromStoredProcedure(string procedureName, string dataTableName = "Data", params SqlParameter[] parameters)
    {
        try
        {
            using (DataTable dt = new DataTable(dataTableName))
            {
                using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(procedureName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

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
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            throw;
        }
    }

    public static DataSet GetDataSetFromStoredProcedure(string procedureName, string[] dataTableNames, params SqlParameter[] parameters)
    {
        DataSet ds = new DataSet();

        try
        {

            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procedureName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    for (int i = 0; i < dataTableNames.Length; i++)
                    {
                        string tblKey = "Table" + (i > 0 ? i.ToString() : "");
                        string tblName = dataTableNames[i];

                        da.TableMappings.Add(tblKey, tblName);
                    }

                    da.Fill(ds);
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            throw;
        }

        return ds;
    }

    public static int GetScalarInt(string sql)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    return int.Parse(cmd.ExecuteScalar().ToString());
                }
            }
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static bool GetScalarBool(string sql)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    return bool.Parse(cmd.ExecuteScalar().ToString());
                }
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static string GetScalarString(string sql)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    return cmd.ExecuteScalar().ToString();
                }
            }
        }
        catch (Exception)
        {
            return "";
        }
    }

    public static bool ExecuteCommand(string sql)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool WriteToLogTable(string message)
    {
        bool saved;

        string sqlStatement = "INSERT INTO LOG (LOG_TYPEID, MessageDate, Message, CREATEDBY) VALUES (1, '" + DateTime.Now + "', '" + message + "', 'RawImport')";
        using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sqlStatement, conn))
            {
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();
                {
                    saved = true;
                }
            }
        }

        return saved;
    }

    public static void WTS_SaveColumnOrderingPref(bool useColumnOrdering = false)
    {
        string sqlStatement = "";

        if (useColumnOrdering)
        {
            sqlStatement = "UPDATE ColumnOrderingPreference SET UseColumnOrdering = 1";
        }
        else
        {
            sqlStatement = "UPDATE ColumnOrderingPreference SET UseColumnOrdering = 0";
        }
        ExecuteCommand(sqlStatement);
    }

    public static bool WTS_GetColumnOrderingPref()
    {
        string sqlStatement = "SELECT UseColumnOrdering FROM ColumnOrderingPreference";

        return GetScalarBool(sqlStatement);
    }


    public static void ImportRaw()
    {
        bool exists, saved, updated;
        int id, allocationID, allocationGroupID;
        int systemID = 0;
        string outMsg;
        string sqlStatement;

        //string AllocationAssignment = "";

        try
        {
            DataTable dtImportTable = new DataTable();

            WriteToLogTable("About to start Raw Import.");

            sqlStatement = "SELECT DISTINCT [System] FROM Steve_Allocation WHERE [System] NOT IN (SELECT WTS_SYSTEM from WTS_SYSTEM)";
            dtImportTable = GetGeneric(WTSCommon.WTS_ConnectionString, sqlStatement, null);
        
            // Add any missing WTS_SYSTEM entries.
            foreach (DataRow drImportTable in dtImportTable.Rows)
            {
                //===============================
                saved = MasterData.WTS_System_Add(drImportTable["System"].ToString(), drImportTable["System"].ToString(), 0,  out exists, newID: out id);
                //===============================
            }

            WriteToLogTable("WTS_System entries done.");

            // Add all Allocation Groups
            sqlStatement = "SELECT DISTINCT [Allocation Group] FROM Steve_Allocation WHERE [Allocation Group] NOT IN (SELECT ALLOCATIONGROUP FROM ALLOCATIONGROUP)";
            dtImportTable = GetGeneric(WTSCommon.WTS_ConnectionString, sqlStatement, null);

            foreach (DataRow drImportTable in dtImportTable.Rows)
            {
                //====================================
                saved = MasterData.AllocationGroup_Add(drImportTable["Allocation Group"].ToString(), drImportTable["Allocation Group"].ToString(), "", 0, true, false, out exists, out allocationGroupID, out outMsg);
                //====================================
            }
            WriteToLogTable("Allocation Groups done.");

            // Add all Allocations & Allocation_Systems
            sqlStatement = "SELECT [System], [Allocation Group], [Allocation Assignment] FROM Steve_Allocation";
            dtImportTable = GetGeneric(WTSCommon.WTS_ConnectionString, sqlStatement, null);

            foreach (DataRow drImportTable in dtImportTable.Rows)
            {
                sqlStatement = "SELECT ALLOCATIONGROUPID FROM ALLOCATIONGROUP WHERE ALLOCATIONGROUP = '" + drImportTable["Allocation Group"].ToString() + "'";
                allocationGroupID = GetScalarInt(sqlStatement);

                //================================
                saved = MasterData.Allocation_Add(0, allocationGroupID, allocation: drImportTable["Allocation Assignment"].ToString(), description: drImportTable["Allocation Assignment"].ToString()
                    , defaultAssignedToID: 67, defaultSMEID: 68, defaultBusinessResourceID: 68, defaultTechnicalResourceID: 67
                    , sortOrder: 0, archive: false, exists: out exists, newID: out allocationID, errorMsg: out outMsg);
                //================================

                // Update any missed in prior run.
                sqlStatement = "UPDATE Allocation SET AllocationGroupID = allocationGroupID WHERE Allocation = '" + drImportTable["Allocation Assignment"].ToString()
                + "' AND AllocationGroupID IS NULL";

                updated = ExecuteCommand(sqlStatement);

                sqlStatement = "SELECT AllocationID FROM Allocation WHERE Allocation = '" + drImportTable["Allocation Assignment"].ToString() + "'";
                allocationID = GetScalarInt(sqlStatement);

                sqlStatement = "SELECT WTS_SYSTEMID FROM WTS_SYSTEM WHERE WTS_SYSTEM = '" + drImportTable["System"].ToString() + "'";
                systemID = GetScalarInt(sqlStatement);

                if (allocationID > 0 & systemID > 0)
                { 
                    try
                    {
                        //======================================
                        saved = MasterData.Allocation_System_Add(allocationID: allocationID, systemID: systemID, description: "", proposedPriority: 0, approvedPriority: 0
                                , exists: out exists, newID: out id, errorMsg: out outMsg);
                        //======================================
                    }
                    catch (Exception e)
                    {
                        WriteToLogTable("Error in Allocation_System_Add. Error: " + e.Message);
                    }
                }
                else
                {
                    if (allocationID < 1)
                    {
                        WriteToLogTable("AllocationID returned a 0 for Allocation Assignment " + drImportTable["Allocation Assignment"].ToString());
                    }
                    WriteToLogTable("SystemID returned a 0 for System " + drImportTable["System"].ToString());
                    if (systemID < 1)
                    {

                    }
                }
            }
            WriteToLogTable("Allocations and Allocation_Systems done.");
        }
        catch (Exception e)
        {
            WriteToLogTable("Raw Import error: " + e.Message);

       }
   }

    /// <summary>
    /// Get details about current contract for display in footer
    /// </summary>
    /// <returns></returns>
    public static DataSet Get_Contract_Data()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Get System Notification if any are "queued"
	/// </summary>
	/// <param name="localDateAndTime">current date and time in user's location</param>
	/// <param name="clientTimeZone">timezone of user's computer settings</param>
	/// <returns>DataTable containing "rows" of html data to write to display</returns>
	public static DataTable GET_SYS_NOTIFICATION(string localDateAndTime, string clientTimeZone)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Get options for View dropdownlist
	/// - "Default" loads options for home page
	/// - User ID is optional. When used, will load options that apply to ALL users AND those specific to specified user
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="gridName"></param>
	/// <returns></returns>
	public static DataTable GetViewOptions(int userId = 0, string gridName = "Default")
	{
		string procName = "GridViewList_Get";
		using (DataTable dt = new DataTable("Workload"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = userId == 0 ? (object)DBNull.Value : userId;
					cmd.Parameters.Add("@GridName", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(gridName) ? (object)DBNull.Value : gridName.Trim();
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

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

    public static DataTable GetUserPreferences(int userId = 0, int userSettingTypeID = 0, int gridNameID = 0)
    {
        string procName = "UserSettingList_Get";
        using (DataTable dt = new DataTable("UserSettings"))
        {
            using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(procName, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@WTS_ResourceID", SqlDbType.Int).Value = userId == 0 ? (object)DBNull.Value : userId;
                    cmd.Parameters.Add("@UserSettingTypeID", SqlDbType.Int).Value = userSettingTypeID;
                    cmd.Parameters.Add("@GridNameID", SqlDbType.Int).Value = gridNameID;

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

    public static bool Comment_Delete(out string errorMsg, int commentId = 0)
	{
		errorMsg = string.Empty;
		bool deleted = false;

		string procName = "Comment_Delete";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

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

	public static bool Comment_Update(out string errorMsg, int commentId = 0, string comment_text = "")
	{
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "Comment_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@CommentID", SqlDbType.Int).Value = commentId;
				cmd.Parameters.Add("@Comment_Text", SqlDbType.NVarChar).Value = comment_text;
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

    #region Attachments

    public static DataTable AttachmentType_GetList(bool showHiddenItems = false)
	{
		string procName = "AttachmentType_GetList";

		using (DataTable dt = new DataTable("AttachmentType"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ShowHiddenItems", SqlDbType.Bit).Value = showHiddenItems;

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

	public static bool Attachment_Add(int attachmentTypeID
		, string fileName
		, string title
		, string description
		, byte[] fileData
		, int extensionID
		, out int newID
		, out string errorMsg)
	{
		newID = 0;
		errorMsg = string.Empty;
		bool saved = false;

		string procName = "Attachment_Add";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@AttachmentTypeID", SqlDbType.Int).Value = attachmentTypeID;
				cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
				cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = title;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@FileData", SqlDbType.VarBinary).Value = fileData;
				cmd.Parameters.Add("@ExtensionID", SqlDbType.Int).Value = extensionID == 0 ? (object)DBNull.Value : extensionID;
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

	public static bool Attachment_Update(int attachmentID
		, int attachmentTypeID
		, string fileName
		, string title
		, string description
		//, byte[] fileData
		, out bool exists
		, out string errorMsg)
	{
		errorMsg = string.Empty;
		exists = false;
		bool saved = false;

		string procName = "Attachment_Update";
		using (SqlConnection conn = new SqlConnection(WTSCommon.WTS_ConnectionString))
		{
			conn.Open();

			using (SqlCommand cmd = new SqlCommand(procName, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = attachmentID;
				cmd.Parameters.Add("@AttachmentTypeID", SqlDbType.Int).Value = attachmentTypeID;
				cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
				cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = title;
				cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = HttpContext.Current.User.Identity.Name;

				cmd.Parameters.Add("@saved", SqlDbType.Bit).Direction = ParameterDirection.Output;

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

				SqlParameter paramSaved = cmd.Parameters["@saved"];
				if (paramSaved != null)
				{
					bool.TryParse(paramSaved.Value.ToString(), out saved);
				}
			}
		}

		return saved;
	}

	public static DataTable DOWNLOAD_ATTACHMENT(int attachmentID)
	{
		string procName = "Attachment_Get";

		using (DataTable dt = new DataTable("Attachment"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@AttachmentID", SqlDbType.Int).Value = attachmentID;

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

	#endregion Attachments


	#region Attributes

	public static DataTable AttributeList_Get(int attributeTypeID)
	{
		string procName = "AttributeList_Get";

		using (DataTable dt = new DataTable("Attribute"))
		{
			using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(procName, cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@AttributeTypeID", SqlDbType.Int).Value = attributeTypeID;

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

    public static string GetDefaultGridViewName(int gridNameId, int resourceId)
    {
        var cmdText = "select viewname " +
                      "from gridview gv " +
                      "join usersetting us on gv.gridnameid = us.gridnameid and gv.gridviewid = us.settingvalue " +
                      "where us.gridnameid = " + gridNameId + " AND us.wts_resourceid = " + resourceId;

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        using (SqlCommand cmd = new SqlCommand(cmdText, cn))
        {
            cmd.CommandType = CommandType.Text;
            cn.Open();

            return (string)cmd.ExecuteScalar() ?? "Default";
        }
    }
    #endregion Attributes
}