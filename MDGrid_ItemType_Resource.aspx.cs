﻿using System;
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


public partial class MDGrid_ItemType_Resource : System.Web.UI.Page
{
	protected DataTable _dtResource = null;
    protected DataColumnCollection DCC;
	protected GridCols columnData = new GridCols();

	protected bool _refreshData = false;
	protected bool _export = false;

	protected int _qfWorkItemTypeID = 0;

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
		if (Request.QueryString["WORKITEMTYPEID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["WORKITEMTYPEID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["WORKITEMTYPEID"].ToString()), out this._qfWorkItemTypeID);
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
        _dtResource = UserManagement.LoadUserList();

        DataTable dt = MasterData.WorkActivity_WTS_RESOURCEList_Get(WORKITEMTYPEID: this._qfWorkItemTypeID);

		if (dt != null)
		{

            this.DCC = dt.Columns;
			Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));

			InitializeColumnData(ref dt);
			dt.AcceptChanges();

            if (_dtResource != null && _dtResource.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _dtResource.DefaultView.RowFilter = string.Format("NOT WTS_RESOURCEID = {0} ", dr["WTS_RESOURCEID"].ToString());
                    _dtResource = _dtResource.DefaultView.ToTable();
                }
            }
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
						blnVisible = true;
						break;
					case "WORKITEMTYPEID":
						blnVisible = false;
						blnSortable = false;
						break;
                    case "WORKITEMTYPE":
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "WTS_RESOURCEID":
						blnVisible = false;
						blnSortable = false;
						break;
                    case "USERNAME":
                        displayName = "Resource";
                        blnVisible = true;
                        blnSortable = true;
                        break;
                    case "IntakeTeam":
						displayName = "Intake Team";
						blnVisible = true;
						break;
                    case "ARCHIVE":
						displayName = "Archive";
						blnVisible = true;
						blnSortable = true;
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

		row.Cells[DCC.IndexOf("X")].Text = "&nbsp;";
	}

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		columnData.SetupGridBody(e.Row);
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		//add edit link
		string itemId = row.Cells[DCC.IndexOf("WorkActivity_WTS_RESOURCEID")].Text.Trim();
		if (itemId == "0")
		{
			row.Style["display"] = "none";
		}

		row.Attributes.Add("itemID", itemId);

		if (CanView)
		{
			string selectedId = row.Cells[DCC.IndexOf("WTS_RESOURCEID")].Text;
			string selectedText = Server.HtmlDecode(row.Cells[DCC.IndexOf("USERNAME")].Text.Replace("&nbsp;", " ").Trim());

            if (itemId == "0")
            {
                selectedId = _dtResource.Rows[0]["WTS_RESOURCEID"].ToString();
            }

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
		else
		{
			if(row.Cells[DCC.IndexOf("IntakeTeam")].Text != "Intake Team Member") row.Cells[DCC["X"].Ordinal].Controls.Add(WTSUtility.CreateGridDeleteButton(itemId, row.Cells[DCC.IndexOf("WorkActivity_WTS_RESOURCEID")].Text.Trim()));
		}
	}

    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdMD.PageIndex = e.NewPageIndex;
		if (HttpContext.Current.Session["dtMD_WORKITEMTYPE_WTS_RESOURCE"] == null)
		{
			loadGridData();
		}
		else
		{
			grdMD.DataSource = (DataTable)HttpContext.Current.Session["dtMD_WORKITEMTYPE_WTS_RESOURCE"];
		}
	}

    void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC.IndexOf("WORKITEMTYPE")
				&& i != DCC.IndexOf("USERNAME")
				&& i != DCC.IndexOf("IntakeTeam")
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
		row.Cells[DCC.IndexOf("USERNAME")].Style["width"] = "225px";
		row.Cells[DCC.IndexOf("IntakeTeam")].Style["width"] = "125px";
        row.Cells[DCC.IndexOf("ARCHIVE")].Style["width"] = "55px";
	}

	#endregion Grid


	[WebMethod(true)]
	public static string SaveChanges(string workItemType_ID, string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "0" }
			, { "failed", "0" }
			, { "savedIds", "" }
			, { "failedIds", "" }
			, { "error", "" } };
		bool exists = false, saved = false;
		int savedQty = 0, failedQty = 0;
        string ids = string.Empty, failedIds = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
            if (dtjson.Rows.Count == 0)
            {
                errorMsg = "Unable to save. An invalid list of changes was provided.";
                saved = false;
            }
            else
            {
                
                    int id = 0, workItemTypeID = 0, resourceID = 0, archive = 0;  //, ALLOCATIONGROUPID = 0;
                    bool duplicate = false;

                    HttpServerUtility server = HttpContext.Current.Server;
                    //save
                    foreach (DataRow dr in dtjson.Rows)
                    {
                        id = workItemTypeID = resourceID = 0;
                        archive = 0; 
                        archive = 0;
                        duplicate = false;

                        tempMsg = string.Empty;
                        int.TryParse(dr["WorkActivity_WTS_RESOURCEID"].ToString(), out id);
                        int.TryParse(workItemType_ID, out workItemTypeID);
                        int.TryParse(dr["USERNAME"].ToString(), out resourceID);
                        int.TryParse(dr["ARCHIVE"].ToString(), out archive);

                        if (resourceID == 0)
                        {
                            tempMsg = "You must specify a Resource.";
                            saved = false;
                        }
                        else
                        {
                            if (id == 0)
                            {
                                exists = false;
                            saved = MasterData.WORKITEMTYPE_Resource_Add(WORKITEMTYPEID: workItemTypeID
                                    , ResourceID: resourceID
                                    , exists: out exists
                                    , newID: out id
                                    , errorMsg: out tempMsg);
                                if (exists)
                                {
                                    saved = false;
                                    tempMsg = string.Format("{0}{1}{2}", tempMsg, tempMsg.Length > 0 ? Environment.NewLine : "", "Cannot add duplicate Work Activity - Resource record.");
                                }
                            }
                            else
                            {
                                saved = MasterData.WORKITEMTYPE_Resource_Update(WorkActivity_WTS_RESOURCEID: id
                                    , WORKITEMTYPEID: workItemTypeID
                                    , ResourceID: resourceID
                                    , archive: (archive == 1)
                                    , duplicate: out duplicate
                                    , errorMsg: out tempMsg);
                            }
                        }

                        if (saved)
                        {
                            ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());
                            savedQty += 1;
                        }
                        else
                        {
                            failedQty += 1;
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
            saved = false;
            errorMsg = ex.Message;
            LogUtility.LogException(ex);
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
			, { "deleted", "" }
			, { "error", "" } };
		bool exists = false, deleted = false;
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
				deleted = MasterData.WORKITEMTYPE_Resource_Delete(WorkActivity_WTS_RESOURCEID: itemId, exists: out exists, errorMsg: out errorMsg);
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			deleted = false;
			errorMsg = ex.Message;
		}

		result["exists"] = exists.ToString();
		result["deleted"] = deleted.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
}