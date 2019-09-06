using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;


public partial class MDGrid_Effort : System.Web.UI.Page
{
	protected DataTable _dtEffortArea = null;
	protected DataTable _dtEffortSize = null;
	protected DataColumnCollection DCC;
	protected GridCols _columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfEffortAreaID = 0;
	protected int _qfEffortSizeID = 0;

	protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

	protected bool CanView = false;
	protected bool CanEdit = false;
	protected bool IsAdmin = false;

    protected void Page_Load(object sender, EventArgs e)
    {
		this.IsAdmin = UserManagement.UserIsInRole("Admin");
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
		this.CanView = (CanEdit || UserManagement.UserCanView(WTSModuleOption.MasterData));

		readQueryString();

		initControls();

		loadGridData();
	}

	private void readQueryString()
	{
		if (Request.QueryString["RefData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["RefData"])
			|| Request.QueryString["RefData"].Trim() == "1" || Request.QueryString["RefData"].Trim().ToUpper() == "TRUE")
		{
			_refreshData = true;
		}
		if (Request.QueryString["EffortAreaID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["EffortAreaID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["EffortAreaID"].ToString()), out this._qfEffortAreaID);
		}
		if (Request.QueryString["EffortSizeID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["EffortSizeID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["EffortSizeID"].ToString()), out this._qfEffortSizeID);
		}

		if (Request.QueryString["sortOrder"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
		{
			this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
		}
	}

	private void initControls()
	{
		grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
		grdMD.GridRowDataBound += grdMD_GridRowDataBound;
		grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
	}


	private void loadGridData()
	{
		_dtEffortSize = MasterData.EffortSizeList_Get(includeArchive: false);
		_dtEffortArea = MasterData.EffortAreaList_Get(includeArchive: false);
		DataTable dt = MasterData.EffortArea_SizeList_Get(effortAreaID: this._qfEffortAreaID, effortSizeID: this._qfEffortSizeID);

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			InitializeColumnData(ref dt);
			dt.AcceptChanges();
		}

		if (_qfEffortAreaID != 0 && dt != null && dt.Rows.Count > 0)
		{
			dt.DefaultView.RowFilter = string.Format(" EffortAreaID =  {0} OR (EffortAreaID = 0 AND EffortSizeID = 0) ", _qfEffortAreaID.ToString());
			dt = dt.DefaultView.ToTable();
		}
		if (_qfEffortSizeID != 0 && dt != null && dt.Rows.Count > 0)
		{
			dt.DefaultView.RowFilter = string.Format(" EffortSizeID =  {0} OR (EffortAreaID = 0 AND EffortSizeID = 0) ", _qfEffortSizeID.ToString());
			dt = dt.DefaultView.ToTable();
		}
		//int count = dt.Rows.Count;
		//count = count > 0 ? count - 1 : count; //need to subtract the empty row
		////spanRowCount.InnerText = count.ToString();

		grdMD.DataSource = dt;
		grdMD.DataBind();
	}

	protected void InitializeColumnData(ref DataTable dt)
	{
		try
		{
			string displayName = string.Empty, groupName = string.Empty;
			bool blnVisible = false, blnSortable = false, blnOrderable = false;

			foreach (DataColumn gridColumn in dt.Columns)
			{
				displayName = gridColumn.ColumnName;
				blnVisible = false;
				blnSortable = false;
				blnOrderable = false;
				groupName = string.Empty;

				switch (gridColumn.ColumnName)
				{
					case "X":
						displayName = "&nbsp;";
						blnVisible = true;
						break;
					case "EffortArea_SizeID":
						displayName = "EffortArea_SizeID";
						blnVisible = false;
						blnSortable = false;
						break;
					case "EffortAreaID":
						displayName = "EffortAreaID";
						blnVisible = false;
						blnSortable = false;
						break;
					case "EffortArea":
						displayName = "Area of Effort";
						blnVisible = true;
						blnSortable = false;
						break;
					case "EffortArea_SORT_ORDER":
						displayName = "WorkType_SORT_ORDER";
						blnVisible = false;
						blnSortable = true;
						break;
					case "EffortSizeID":
						displayName = "EffortSizeID";
						blnVisible = false;
						blnSortable = false;
						break;
					case "EffortSize":
						displayName = "Size of Effort";
						blnVisible = true;
						blnSortable = true;
						break;
					case "EffortSize_SORT_ORDER":
						displayName = "EffortSize_SORT_ORDER";
						blnVisible = false;
						blnSortable = true;
						break;
					case "Description":
						displayName = "Description";
						blnVisible = true;
						break;
					case "MinValue":
						displayName = "Minimum";
						blnVisible = true;
						blnSortable = true;
						break;
					case "MaxValue":
						displayName = "Maximum";
						blnVisible = true;
						blnSortable = true;
						break;
					case "Unit":
						displayName = "Unit";
						blnVisible = true;
						break;
					case "WorkItem_Count":
						displayName = "Assigned";
						blnVisible = true;
						blnSortable = true;
						break;
					case "ARCHIVE":
						displayName = "Archive";
						blnVisible = false;
						blnSortable = false;
						break;
				}

				_columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				_columnData.Columns.Item(_columnData.Columns.Count - 1).CanReorder = blnOrderable;
			}

			//Initialize the columnData
			_columnData.Initialize(ref dt, ";", "~", "|");

			//Get sortable columns and default column order
			SortableColumns = _columnData.SortableColumnsToString();
			DefaultColumnOrder = _columnData.DefaultColumnOrderToString();
			//Sort and Reorder Columns
			_columnData.ReorderDataTable(ref dt, ColumnOrder);
			_columnData.SortDataTable(ref dt, SortOrder);
			SelectedColumnOrder = _columnData.CurrentColumnOrderToString();

		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}


	#region Grid

	void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		_columnData.SetupGridHeader(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);
	}

	void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		_columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("EffortArea_SizeID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			DropDownList ddl = WTSUtility.CreateGridDropdownList(dt: _dtEffortArea, field: "EffortArea", textField: "EffortArea", valueField: "EffortAreaID", itemId: itemId, value: row.Cells[DCC.IndexOf("EffortAreaID")].Text, text: row.Cells[DCC.IndexOf("EffortArea")].Text, attributes: null);
			if (this._qfEffortAreaID != 0)
			{
				ddl.Enabled = false;
			}
			row.Cells[DCC.IndexOf("EffortArea")].Controls.Add(ddl);
			ddl = WTSUtility.CreateGridDropdownList(dt: _dtEffortSize, field: "EffortSize", textField: "EffortSize", valueField: "EffortSizeID", itemId: itemId, value: row.Cells[DCC.IndexOf("EffortSizeID")].Text, text: row.Cells[DCC.IndexOf("EffortSize")].Text, attributes: null);
			if (this._qfEffortSizeID != 0)
			{
				ddl.Enabled = false;
			}
			row.Cells[DCC.IndexOf("EffortSize")].Controls.Add(ddl);
			row.Cells[DCC.IndexOf("Description")].Controls.Add(WTSUtility.CreateGridTextBox(field: "Description", itemId: itemId, text: Server.HtmlDecode(row.Cells[DCC.IndexOf("Description")].Text.Replace("&nbsp;", " ").Trim())));
			row.Cells[DCC.IndexOf("MinValue")].Controls.Add(WTSUtility.CreateGridTextBox(field: "MinValue", itemId: itemId, text: row.Cells[DCC.IndexOf("MinValue")].Text, isNumber: true));
			row.Cells[DCC.IndexOf("MaxValue")].Controls.Add(WTSUtility.CreateGridTextBox(field: "MaxValue", itemId: itemId, text: row.Cells[DCC.IndexOf("MaxValue")].Text, isNumber: true));
			row.Cells[DCC.IndexOf("Unit")].Controls.Add(WTSUtility.CreateGridTextBox(field: "Unit", itemId: itemId, text: row.Cells[DCC.IndexOf("Unit")].Text, isNumber: false));
			bool archive = false;
			if (row.Cells[DCC.IndexOf("ARCHIVE")].HasControls()
				&& row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0] is CheckBox)
			{
				archive = ((CheckBox)row.Cells[DCC.IndexOf("ARCHIVE")].Controls[0]).Checked;
			}
			else if (row.Cells[DCC.IndexOf("ARCHIVE")].Text == "1")
			{
				archive = true;
			}
			row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Clear();
			row.Cells[DCC.IndexOf("ARCHIVE")].Controls.Add(WTSUtility.CreateGridCheckBox("Archive", itemId, archive));
		}
        if (CanEdit)
        {
            row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("EffortArea")].Text.Trim()));
        }
	}

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_Effort"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_Effort"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("EffortAreaID")
				&& i != DCC.IndexOf("EffortSizeID")
				&& i != DCC.IndexOf("MinValue")
				&& i != DCC.IndexOf("MaxValue")
				&& i != DCC.IndexOf("WorkItem_Count")
				&& i != DCC.IndexOf("SORT_ORDER")
				&& i != DCC.IndexOf("ARCHIVE"))
			{
				row.Cells[i].Style["text-align"] = "left";
				row.Cells[i].Style["padding-left"] = "5px";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "center";
				row.Cells[i].Style["padding-left"] = "0px";
			}
		}

		//more column formatting
		row.Cells[DCC.IndexOf("EffortAreaID")].Style["width"] = "12px";
		row.Cells[DCC.IndexOf("EffortArea")].Style["width"] = "100px";
		row.Cells[DCC.IndexOf("EffortSize")].Style["width"] = "80px";
		row.Cells[DCC.IndexOf("MinValue")].Style["width"] = "65px";
		row.Cells[DCC.IndexOf("MaxValue")].Style["width"] = "65px";
		row.Cells[DCC.IndexOf("Unit")].Style["width"] = "75px";
		row.Cells[DCC.IndexOf("WorkItem_Count")].Style["width"] = "60px";
		row.Cells[DCC.IndexOf("SORT_ORDER")].Style["width"] = "35px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
		row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
	}

	#endregion Grid


	[WebMethod(true)]
	public static string SaveChanges(string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
			, { "failed", "0" }
			, { "savedIds", "" }
			, { "failedIds", "" }
			, { "error", "" } };
		bool exists = false, saved = false, duplicate = false;
		int savedQty = 0, failedQty = 0;
		string ids = string.Empty, failedIds = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

		try
		{
			DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjson == null || dtjson.Rows.Count == 0)
			{
				errorMsg = "Unable to save. No list of changes was provided.";
			}
			else
			{
				int id = 0, effortAreaID = 0, effortSizeID = 0, min = 0, max = 0, sortOrder = 0, archive = 0;
				string description = string.Empty, unit = string.Empty;

				HttpServerUtility server = HttpContext.Current.Server;
				//save
				foreach (DataRow dr in dtjson.Rows)
				{
					id = effortAreaID = effortSizeID = 0;
					min = max = sortOrder = archive = 0;
					description = string.Empty;
					unit = string.Empty;

					tempMsg = string.Empty;
					int.TryParse(dr["EffortArea_SizeID"].ToString(), out id);
					int.TryParse(dr["EffortArea"].ToString(), out effortAreaID);
					int.TryParse(dr["EffortSize"].ToString(), out effortSizeID);
					description = server.UrlDecode(dr["Description"].ToString());
					int.TryParse(dr["MinValue"].ToString(), out min);
					int.TryParse(dr["MaxValue"].ToString(), out max);
					unit = server.UrlDecode(dr["Unit"].ToString());
					int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
					int.TryParse(dr["ARCHIVE"].ToString(), out archive);

					if (effortAreaID == 0 || effortSizeID == 0)
					{
						tempMsg = "You must specify an Area of Effort and a Size of Effort";
						saved = false;
					}
					else
					{
						if (id == 0)
						{
							exists = false;
							saved = MasterData.EffortArea_Size_Add(
								effortAreaID: effortAreaID, effortSizeID: effortSizeID
								, minValue: min, maxValue: max, units: unit
								, description: description
								, sortOrder: sortOrder, exists: out exists, newID: out id, errorMsg: out tempMsg);
							if (exists)
							{
								saved = false;
								tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Area of Effort-Size record.");
							}
						}
						else
						{
							saved = MasterData.EffortArea_Size_Update(
								effortArea_SizeID: id, effortAreaID: effortAreaID, effortSizeID: effortSizeID
								, minValue: min, maxValue: max, units: unit
								, description: description, sortOrder: sortOrder, archive: (archive == 1), duplicate: out duplicate, errorMsg: out tempMsg);
						}
					}

					if (saved)
					{
						savedQty += 1;
						ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
					}

					else
					{
						failedQty += 1;
						failedIds += string.Format("{0}{1}", failedIds.Length > 0 ? "," : "", failedIds.ToString());
					}
					if (tempMsg.Length > 0)
					{
						errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
			errorMsg = ex.Message;
		}

		result["savedIds"] = ids;
		result["failedIds"] = failedIds;
		result["saved"] = savedQty.ToString();
		result["failed"] = failedQty.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod(true)]
	public static string DeleteItem(int itemId, string item)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "id", itemId.ToString() }
			, { "item", item }
			, { "exists", "" }
			, { "hasDependencies", "" }
			, { "deleted", "" }
			, { "archived", "" }
			, { "error", "" } };
		bool exists = false, hasDependencies = false, deleted = false, archived = false;
		string errorMsg = string.Empty;

		try
		{
			//delete
			if (itemId == 0)
			{
				errorMsg = "You must specify an item to delete.";
			}
			else
			{
				deleted = MasterData.EffortArea_Size_Delete(itemId, out exists, out hasDependencies, out archived, out errorMsg);
				if (hasDependencies && errorMsg.Length == 0)
				{
					errorMsg = "Record has dependencies and could not be permanently deleted. It has been archived instead.";
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			deleted = false;
			errorMsg = ex.Message;
		}

		result["exists"] = exists.ToString();
		result["hasDependencies"] = hasDependencies.ToString();
		result["deleted"] = deleted.ToString();
		result["archived"] = archived.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
}