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


public partial class MDGrid_WorkType_Resource : System.Web.UI.Page
{
	protected DataTable _dtResource = null;
	protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _WorkTypeID = 0;

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
		if (Request.QueryString["WorkTypeID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["WorkTypeID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["WorkTypeID"].ToString()), out this._WorkTypeID);
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
        _dtResource = UserManagement.LoadUserList(0, false, false, "");
        DataTable dt = MasterData.WorkType_ResourceList_Get(workTypeID: this._WorkTypeID);

		if (dt != null)
		{
			this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

            InitializeColumnData(ref dt);
            dt.AcceptChanges();
        }

		int count = dt.Rows.Count;
		count = count > 0 ? count - 1 : count; //need to subtract the empty row
		spanRowCount.InnerText = count.ToString();

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
					case "WorkType_WTS_RESOURCEID":
						displayName = "WorkType_WTS_RESOURCEID";
						blnVisible = false;
						blnSortable = false;
						break;
					case "WTS_RESOURCEID":
						displayName = "WTS_RESOURCEID";
						blnVisible = false;
						blnSortable = false;
						break;
					case "USERNAME":
						displayName = "Resource";
						blnVisible = true;
						blnSortable = true;
						break;
					case "ARCHIVE":
						displayName = "Archive";
						blnVisible = true;
						blnSortable = true;
						break;
                    case "Y":
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                    case "Z":
                        displayName = "&nbsp;";
                        blnVisible = true;
                        break;
                }

				columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
			}

			//Initialize the columnData
			columnData.Initialize(ref dt, ";", "~", "|");

			//Get sortable columns and default column order
			SortableColumns = columnData.SortableColumnsToString();
			DefaultColumnOrder = columnData.DefaultColumnOrderToString();
			//Sort and Reorder Columns
			columnData.ReorderDataTable(ref dt, ColumnOrder);
			columnData.SortDataTable(ref dt, SortOrder);
			SelectedColumnOrder = columnData.CurrentColumnOrderToString();

		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}


	#region Grid

	void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridHeader(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);
	}

	void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("WorkType_WTS_RESOURCEID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			string selectedId = row.Cells[DCC.IndexOf("WTS_RESOURCEID")].Text;
			string selectedText = row.Cells[DCC.IndexOf("USERNAME")].Text;
            if (selectedId == "0")
            {
                selectedId = _dtResource.Rows[0]["WTS_RESOURCEID"].ToString();
            }

            if (itemId != "0") row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(itemId));

            DropDownList ddl = null;
			ddl = WTSUtility.CreateGridDropdownList(_dtResource, "USERNAME", "USERNAME", "WTS_RESOURCEID", itemId, selectedId, selectedText, null);
			row.Cells[DCC.IndexOf("USERNAME")].Controls.Add(ddl);

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

		if (!CanEdit)
		{
			Image imgBlank = new Image();
			imgBlank.Height = 10;
			imgBlank.Width = 10;
			imgBlank.ImageUrl = "Images/Icons/blank.png";
			imgBlank.AlternateText = "";
			row.Cells[DCC["X"].Ordinal].Controls.Add(imgBlank);
		}
	}

	void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_WorkType_Resource"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WorkType_Resource"];
		}
	}

	void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("X")
				&& i != DCC.IndexOf("Y")
				&& i != DCC.IndexOf("Z")
                && i != DCC.IndexOf("USERNAME")
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
		row.Cells[DCC.IndexOf("X")].Style["width"] = "12px";
		row.Cells[DCC.IndexOf("Y")].Style["width"] = "12px";
        row.Cells[DCC.IndexOf("Z")].Style["width"] = "650px";
        row.Cells[DCC.IndexOf("USERNAME")].Style["width"] = "110px";
		row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
	}

    private CheckBox CreateCheckBox(string value)
    {
        CheckBox chk = new CheckBox();

        chk.Attributes.Add("worktype_resource_id", value);

        return chk;
    }

    #endregion Grid

    #region AJAX
    [WebMethod(true)]
	public static string SaveChanges(int workTypeID, string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
		bool exists = false, saved = false;
		string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

		try
		{
			DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjson.Rows.Count == 0)
			{
				errorMsg = "Unable to save. An invalid list of changes was provided.";
				saved = false;
			}

			int id = 0, resourceID = 0, archive = 0;
			string allocation = string.Empty, description = string.Empty;

			HttpServerUtility server = HttpContext.Current.Server;
			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				id = 0;
                resourceID = 0;
				description = string.Empty;
				archive = 0;

				tempMsg = string.Empty;
				int.TryParse(dr["WorkType_WTS_RESOURCEID"].ToString(), out id);
				int.TryParse(dr["USERNAME"].ToString(), out resourceID);
				int.TryParse(dr["ARCHIVE"].ToString(), out archive);

				if (resourceID == 0 || workTypeID == 0)
				{
					tempMsg = "You must specify a Resource.";
					saved = false;
				}
				else
				{
					if (id == 0)
					{
						exists = false;
						saved = MasterData.WorkType_Resource_Add(resourceID: resourceID, workTypeID: workTypeID, exists: out exists, newID: out id, errorMsg: out tempMsg);
						if (exists)
						{
							saved = false;
							tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate WorkType - Resource record.");
						}
					}
					else
					{
						saved = MasterData.WorkType_Resource_Update(workTypeResourceID: id, resourceID: resourceID, workTypeID: workTypeID, archive: (archive == 1), errorMsg: out tempMsg);
					}
				}

				if (saved)
				{
					ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
				}

				if (tempMsg.Length > 0)
				{
					errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
				}
			}
		}
		catch (Exception ex)
		{
			saved = false;
			errorMsg = ex.Message;
			LogUtility.LogException(ex);
		}

		result["ids"] = ids;
		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

    [WebMethod()]
    public static string DeleteItems(string resources)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        bool exists = false, hasDependencies = false, archived = false;
        string errorMsg = string.Empty;

        try
        {
            DataTable dtResource = (DataTable)JsonConvert.DeserializeObject(resources, (typeof(DataTable)));

            foreach (DataRow dr in dtResource.Rows)
            {
                int WorkType_Resource_ID = 0;

                int.TryParse(dr["worktype_resource_id"].ToString(), out WorkType_Resource_ID);

                deleted = MasterData.WorkType_Resource_Delete(WorkType_Resource_ID, out exists, out hasDependencies, out archived, out errorMsg);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            deleted = false;
            errorMsg = ex.Message;
        }

        result["deleted"] = deleted.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}