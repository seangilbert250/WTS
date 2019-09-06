using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;


public partial class WorkItem_MassChange : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected Dictionary<string, GridColsColumn> _gridColumns = new Dictionary<string, GridColsColumn>();
	protected GridCols _columnData = new GridCols();

	protected bool MyData = true;
	protected bool IncludeArchive = false;

	protected string SelectedField = string.Empty;

	protected bool CanEdit = false;


    protected void Page_Load(object sender, EventArgs e)
    {
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);

		readQueryString();

		if (DCC != null)
		{
			this.DCC.Clear();
		}

		loadData();
    }

	private void readQueryString()
	{
		if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
			|| Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
		{
			MyData = true;
		}
		else
		{
			MyData = false;
		}

		if (Request.QueryString["IncludeArchive"] != null && string.IsNullOrWhiteSpace(Request.QueryString["IncludeArchive"])
			&& (Request.QueryString["IncludeArchive"].Trim() == "1" || Request.QueryString["IncludeArchive"].Trim().ToUpper() == "TRUE"))
		{
			IncludeArchive = true;
		}

		if (Request.QueryString["FieldName"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["FieldName"].ToString()))
		{
			this.SelectedField = Server.UrlDecode(Request.QueryString["FieldName"]);
		}
	}


	#region Data

	protected void initializeColumnData(ref DataTable dt)
	{
		_gridColumns = new Dictionary<string, GridColsColumn>();
		try
		{
			GridColsColumn column = new GridColsColumn();
			string dbName = string.Empty, displayName = string.Empty, idField = string.Empty;
			bool isVisible = false;

			foreach (DataColumn gridColumn in dt.Columns)
			{
				column = new GridColsColumn();
				displayName = gridColumn.ColumnName;
				idField = gridColumn.ColumnName;
				isVisible = false;

				switch (gridColumn.ColumnName)
				{
					//case "WORKREQUEST":
					//	displayName = "Work Request";
					//	idField = "WORKREQUESTID";
					//	isVisible = true;
					//	break;
					case "WORKITEMTYPE":
						displayName = "Work Activity";
						idField = "WORKITEMTYPEID";
						isVisible = true;
						break;
					case "Websystem":
						displayName = "System(Task)";
						idField = "WTS_SYSTEMID";
						isVisible = true;
						break;
					//case "ALLOCATION":
					//	displayName = "Allocation Assignment";
					//	idField = "ALLOCATIONID";
					//	isVisible = true;
					//	break;
					case "WorkloadGroup":
						displayName = "Workload Group";
						idField = "WorkloadGroupID";
						isVisible = true;
						break;
					case "WorkArea":
						displayName = "Work Area";
						idField = "WorkAreaID";
						isVisible = true;
						break;
					case "Production":
						displayName = "Production";
						idField = "Production";
						isVisible = false;
						break;
					case "Version":
						displayName = "Release Version";
						idField = "ProductVersionID";
						isVisible = true;
						break;
					case "PRIORITY":
						displayName = "Priority";
						idField = "PRIORITYID";
						isVisible = true;
						break;
					case "Primary_Developer":
						displayName = "Primary Developer";
						idField = "PRIMARYRESOURCEID";
						isVisible = true;
						break;
					case "Assigned":
						displayName = "Assigned To";
						idField = "ASSIGNEDRESOURCEID";
						isVisible = true;
						break;
					case "WorkType":
						displayName = "Resource Group";
						idField = "WorkTypeID";
						isVisible = true;
						break;
					case "STATUS":
						displayName = "Status";
						idField = "STATUSID";
						isVisible = false;
						break;
					case "Progress":
						displayName = "Percent Complete";
						idField = "Progress";
						isVisible = false;
						break;
					case "ARCHIVE":
						displayName = "Archive";
						idField = "ARCHIVE";
						isVisible = false;
						break;
				}

				if (isVisible)
				{
					column.ColumnName = gridColumn.ColumnName;
					column.DisplayName = displayName;
					column.Visible = isVisible;
					column.SortName = idField;

					_gridColumns.Add(column.DisplayName, column);
				}

				//_columnData.Columns.Add(ColumnName: gridColumn.ColumnName, DisplayName: displayName, SortName: idField, Visible: isVisible, Sortable: false);
			}

			//Initialize the columnData
			_columnData.Initialize(ref dt, ";", "~", "|");
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}

	private void loadData()
	{
		DataTable dt = loadFromData(includeArchive: this.IncludeArchive, myData: this.MyData);

		initializeColumnData(ref dt);

		ListItem item = null;

		// list is IEnumerable<int> (e.g., List<int>)
		var ordered = _gridColumns.Keys.OrderBy(k => _gridColumns[k].DisplayName).ToList();

		foreach (string key in ordered)
		{
			GridColsColumn col = _gridColumns[key];
			if (col.Visible && col.ColumnName != "Version")
			{
				item = new ListItem(col.DisplayName, col.ColumnName);
				item.Attributes.Add("id_field", col.SortName);
				ddlAttribute.Items.Add(item);
			}
		}
	}

	protected static DataTable loadFromData(bool includeArchive = false, bool myData = false)
	{
		DataTable dt = WorkloadItem.WorkItemList_Get(workRequestID: 0
			, showArchived: includeArchive ? 1 : 0
			, columnListOnly: 0
			, myData: myData);

		HttpContext.Current.Session["MassChange_FromData"] = dt;

		return dt;
	}

	protected static DataTable GetMasterDataValues(string idField, string columnName, string textField, bool includeArchive = false)
	{
		DataSet ds = null;
		DataTable dt = null;

		try
		{
			switch (columnName.ToUpper())
			{
				case "ALLOCATION":
					ds = MasterData.AllocationList_Get(includeArchive: includeArchive);
					if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
					{
						dt = ds.Tables[0];
						dt.Columns["ALLOCATIONID"].ColumnName = "valueField";
						dt.Columns["ALLOCATION"].ColumnName = "textField";
					}

					break;
				case "ARCHIVE":
					dt = new DataTable();
					dt.Columns.Add("valueField");
					dt.Columns.Add("textField");
					dt.AcceptChanges();

					dt.Rows.Add(new object[] { "0", "No" });
					dt.Rows.Add(new object[] { "1", "Yes" });

					break;
				case "ASSIGNED":
					dt = UserManagement.LoadUserList(organizationId: 0, excludeDeveloper: false, loadArchived: includeArchive, userNameSearch: "");
					if (dt != null)
					{
						dt.Columns["WTS_RESOURCEID"].ColumnName = "valueField";
						dt.Columns["UserName"].ColumnName = "textField";
					}

					break;
				case "PRIMARY_DEVELOPER":
					dt = UserManagement.LoadUserList(organizationId: 0, excludeDeveloper: false, loadArchived: includeArchive, userNameSearch: "");
					if (dt != null)
					{
						dt.Columns["WTS_RESOURCEID"].ColumnName = "valueField";
						dt.Columns["UserName"].ColumnName = "textField";
					}

					break;
				case "WORKITEMTYPE":
					dt = MasterData.WorkItemTypeList_Get(includeArchive: includeArchive);
					if (dt != null)
					{
						dt.Columns["WORKITEMTYPEID"].ColumnName = "valueField";
						dt.Columns["WORKITEMTYPE"].ColumnName = "textField";
					}

					break;
				case "PRIORITY":
					dt = MasterData.PriorityList_Get(includeArchive: includeArchive);
					if (dt != null)
					{
						dt.DefaultView.RowFilter = "PriorityType = 'Work Item'";
						dt = dt.DefaultView.ToTable();
						dt.Columns["PriorityID"].ColumnName = "valueField";
						dt.Columns["Priority"].ColumnName = "textField";
					}

					break;
				case "PRODUCTION":
					dt = new DataTable();
					dt.Columns.Add("valueField");
					dt.Columns.Add("textField");
					dt.AcceptChanges();

					dt.Rows.Add(new object[] { "0", "No" });
					dt.Rows.Add(new object[] { "1", "Yes" });

					break;
				case "VERSION":
					dt = MasterData.ProductVersionList_Get(includeArchive: includeArchive);
					if (dt != null)
					{
						dt = dt.DefaultView.ToTable();
						dt.Columns["ProductVersionID"].ColumnName = "valueField";
						dt.Columns["ProductVersion"].ColumnName = "textField";
					}

					break;
				case "WEBSYSTEM":
					dt = MasterData.SystemList_Get(includeArchive: includeArchive);
					if (dt != null)
					{
						dt = dt.DefaultView.ToTable();
						dt.Columns["WTS_SYSTEMID"].ColumnName = "valueField";
						dt.Columns["WTS_SYSTEM"].ColumnName = "textField";
					}

					break;
				case "WORKAREA":
					dt = MasterData.WorkAreaList_Get(includeArchive: includeArchive);
					if (dt != null)
					{
						dt = dt.DefaultView.ToTable();
						dt.Columns["WorkAreaID"].ColumnName = "valueField";
						dt.Columns["WorkArea"].ColumnName = "textField";
					}

					break;
				case "WORKREQUEST":
					dt = WorkRequest.WorkRequestList_Get(typeID: 0, requestGroupID: 0, myData: false);
					if (dt != null)
					{
						dt = dt.DefaultView.ToTable();
						dt.Columns["WORKREQUESTID"].ColumnName = "valueField";
						dt.Columns["TITLE"].ColumnName = "textField";
					}

					break;
				case "WORKTYPE":
					dt = MasterData.WorkTypeList_Get(includeArchive: includeArchive);
					if (dt != null)
					{
						dt.Columns["WORKTYPEID"].ColumnName = "valueField";
						dt.Columns["WORKTYPE"].ColumnName = "textField";
					}

					break;
				case "WORKLOADGROUP":
					dt = MasterData.WorkloadGroupList_Get(includeArchive: includeArchive);
					if (dt != null)
					{
						dt = dt.DefaultView.ToTable();
						dt.Columns["WorkloadGroupID"].ColumnName = "valueField";
						dt.Columns["WorkloadGroup"].ColumnName = "textField";
					}

					break;
				default:
					dt = null;
					break;
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			dt = null;
		}
		
		return dt;
	}

	#endregion Data


	[WebMethod(true)]
	public static string GetFieldValues(string idField, string columnName, string textField, bool includeArchive, bool myData)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "loaded", "false" }
			, { "CurrentCount", "0" }
			, { "CurrentOptions", "" }
			, { "NewCount", "0" }
			, { "NewOptions", "" }
			, { "error", "" } };
		bool loaded = false;
		int currentCount = 0, newCount = 0;
		string errorMsg = string.Empty;

		DataTable dt = null, dtCurrentOptions = null, dtNewOptions = null;

		try
		{
			if (HttpContext.Current.Session["MassChange_FromData"] == null)
			{
				dt = loadFromData(includeArchive: includeArchive, myData: myData);
			}
			else
			{
				dt = (DataTable)HttpContext.Current.Session["MassChange_FromData"];
			}

			dtCurrentOptions = dt.DefaultView.ToTable(distinct: true, columnNames: new string[] { idField, columnName });
			if (dtCurrentOptions != null)
			{
				dtCurrentOptions.Columns[idField].ColumnName = "valueField";
				dtCurrentOptions.Columns[columnName].ColumnName = "textField";
			}
			currentCount = dtCurrentOptions.Rows.Count;

			dtNewOptions = GetMasterDataValues(idField: idField, columnName: columnName, textField: textField, includeArchive: includeArchive);
			if (dtNewOptions != null && dtNewOptions.Rows.Count > 0)
			{
				dtNewOptions = dtNewOptions.DefaultView.ToTable(distinct: true, columnNames: new string[] { "valueField", "textField" });
				if (dtNewOptions != null)
				{
					newCount = dtNewOptions.Rows.Count;
				}
			}

			loaded = true;
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			loaded = false;
			currentCount = 0;
			errorMsg = ex.Message;
		}

		result["loaded"] = loaded.ToString();
		result["CurrentCount"] = currentCount.ToString();
		result["NewCount"] = newCount.ToString();
		result["error"] = errorMsg;
		if (dtCurrentOptions != null)
		{
			result["CurrentOptions"] = JsonConvert.SerializeObject(dtCurrentOptions, Formatting.None);
		}
		if (dtCurrentOptions != null)
		{
			result["NewOptions"] = JsonConvert.SerializeObject(dtNewOptions, Formatting.None);
		}

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod(true)]
	public static string SaveChanges(string fieldName, string fromValue, string toValue, bool includeArchive, bool myData)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }
			, { "Count", "0" }
			, { "error", "" } };
		bool saved = false, proceed = true;
		int count = 0;
		string errorMsg = string.Empty;

		try
		{
			if (fieldName.Length == 0)
			{
				errorMsg = "No field to update was specified.";
				proceed = false;
				saved = false;
				count = 0;
			}
			if (toValue.Length == 0)
			{
				errorMsg = "No new value was specified.";
				proceed = false;
				saved = false;
				count = 0;
			}
			
			if(proceed)
			{
				if (fromValue.Length == 0)
				{
					//this is okay, but make sure procedure works with empty old value
					string msg = "";
				}
				
				//save the data
				count = WorkloadItem.WorkItem_MassChange(fieldName: fieldName
					, fromValue: fromValue
					, toValue: toValue
					, includeArchive: includeArchive
					, myData: myData
					, errorMsg: out errorMsg);
				if (count > 0)
				{
					saved = true;
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
			count = 0;
			errorMsg = ex.Message;
		}

		result["saved"] = saved.ToString();
		result["Count"] = count.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
}