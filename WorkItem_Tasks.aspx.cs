﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;


public partial class WorkItem_Tasks : System.Web.UI.Page
{
	protected int WorkItemID = 0;
    protected int sourceWorkItemID = 0;
    protected int _showArchived = 0;
	protected bool ShowClosed = false;
	protected string[] SelectedStatuses;
    protected string[] SelectedAssigned;
    protected bool BusinessReview = false;

    DataSet _dsOptions = null;
	protected int Task_Count = 0;
	protected string Filters = string.Empty;
	protected int Filtered_Count = 0;

    protected string SortableColumns = "";
    protected GridCols columnData = new GridCols();
    protected string DefaultColumnOrder;
    protected string SelectedColumnOrder;
    protected string ColumnOrder;
    protected bool ColumnOrderChanged = false;
    protected string SortOrder = "";

    /// <summary>
    /// Can user edit Details for this Work Item
    /// </summary>
    protected bool CanEdit = false;

	protected DataColumnCollection _dcc;

	protected string Parent = "WorkItem";

    protected bool ShowBacklog = false;


    protected string[] PreviousSelectedStatuses;
    protected bool ReloadStatusSession = false;

    protected string AssignedToRankOptions = string.Empty;
    protected string PriorityOptions = string.Empty;
    protected string ResourceOptions = string.Empty;
    protected string CompletionOptions = string.Empty;
    protected string StatusOptions = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
	{
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItemTask);

		readQueryString();
		init();

        loadQF_Status();
        //loadTasks();

    }

    private void readQueryString()
	{
        if (Request.QueryString["sourceWorkItemID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sourceWorkItemID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["sourceWorkItemID"].ToString()), out this.sourceWorkItemID);
        }

        if (Request.QueryString["WorkItemID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["WorkItemID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["WorkItemID"].ToString()), out this.WorkItemID);
		}

        if (Request.QueryString["sortOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
        {
            SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
        }
        if (Request.QueryString["columnOrder"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["columnOrder"].ToString()))
        {
            ColumnOrder = Server.UrlDecode(Request.QueryString["columnOrder"]);
        }
        if (Request.QueryString["columnOrderChanged"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["columnOrderChanged"].ToString()))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["columnOrderChanged"]), out this.ColumnOrderChanged);
        }

        #region QuickFilters

        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"].ToString()))
		{
			this.SelectedStatuses = Server.UrlDecode(Request.QueryString["SelectedStatuses"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (PreviousSelectedStatuses != this.SelectedStatuses)
            {
                ReloadStatusSession = true;
                PreviousSelectedStatuses = this.SelectedStatuses;
            }
        }

        if (Request.QueryString["SelectedAssigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAssigned"].ToString()))
        {
            SelectedAssigned = Server.UrlDecode(Request.QueryString["SelectedAssigned"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (Request.QueryString["BusinessReview"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["BusinessReview"]))
        {
            bool br = false;
            bool.TryParse(Request.QueryString["BusinessReview"].ToString(), out br);
            this.BusinessReview = br;
        }

        if (Request.QueryString["ShowClosed"] == null || string.IsNullOrWhiteSpace(Request.QueryString["ShowClosed"]))
		{
			ShowClosed = false;
		}
		else
		{
			if (Request.QueryString["ShowClosed"].Trim() == "1" || Request.QueryString["ShowClosed"].Trim().ToUpper() == "TRUE")
			{
				ShowClosed = true;
			}
		}

        if (Request.QueryString["ShowBacklog"] == null || string.IsNullOrWhiteSpace(Request.QueryString["ShowBacklog"]))
        {
            ShowBacklog = false;
        }
        else
        {
            if (Request.QueryString["ShowBacklog"].Trim() == "1" || Request.QueryString["ShowBacklog"].Trim().ToUpper() == "TRUE")
            {
                ShowBacklog = true;
            }
        }

        if (Request.QueryString["ShowArchived"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"].ToString()), out this._showArchived);
		}

		#endregion QuickFilters
		
		if (Request.QueryString["parent"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["parent"].ToString()))
		{
			this.Parent = Server.UrlDecode(Request.QueryString["parent"].ToString());
		}

		if (Request.QueryString["rowFilters"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["rowFilters"].ToString()))
		{
			this.Filters = Server.UrlDecode(Request.QueryString["rowFilters"]);
		}

        if (Request.QueryString["ReloadStatusSession"] == null || string.IsNullOrWhiteSpace(Request.QueryString["ReloadStatusSession"]))
        {
            ReloadStatusSession = false;
        }
        else
        {
            if (Request.QueryString["ReloadStatusSession"].Trim() == "1" || Request.QueryString["ReloadStatusSession"].Trim().ToUpper() == "TRUE")
            {
                ReloadStatusSession = true;
            }
        }
    }

    void init()
	{
		_dsOptions = WorkItem_Task.GetAvailableOptions(workItemID: this.WorkItemID);

        //grdTask.RowDataBound += grdTask_RowDataBound;

        DataTable dt = new DataTable();

        dt = _dsOptions.Tables["Rank"];
        foreach (DataRow dr in dt.Rows) this.AssignedToRankOptions += "<option value=\"" + dr["PRIORITYID"].ToString() + "\">" + Uri.EscapeDataString(dr["PRIORITY"].ToString()) + "</option>";

        dt = _dsOptions.Tables["Priority"];
        foreach (DataRow dr in dt.Rows) this.PriorityOptions += "<option value=\"" + dr["PRIORITYID"].ToString() + "\">" + Uri.EscapeDataString(dr["PRIORITY"].ToString()) + "</option>";

        dt = _dsOptions.Tables["User"];
        foreach (DataRow dr in dt.Rows) this.ResourceOptions += "<option value=\"" + dr["WTS_RESOURCEID"].ToString() + "\">" + Uri.EscapeDataString(dr["FIRST_NAME"].ToString() + " " + dr["LAST_NAME"].ToString()) + "</option>";

        dt = _dsOptions.Tables["PercentComplete"];
        foreach (DataRow dr in dt.Rows) this.CompletionOptions += "<option value=\"" + dr["Percent"].ToString() + "\">" + Uri.EscapeDataString(dr["Percent"].ToString()) + "</option>";

        dt = _dsOptions.Tables["Status"];
        foreach (DataRow dr in dt.Rows) this.StatusOptions += "<option value=\"" + dr["STATUSID"].ToString() + "\">" + Uri.EscapeDataString(dr["STATUS"].ToString()) + "</option>";
    }

    void grdTask_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.Header)
		{
			grdTask_GridHeaderRowDataBound(sender, e);
		}
		else if (e.Row.RowType == DataControlRowType.DataRow)
		{
			grdTask_GridRowDataBound(sender, e);
		}
	}

	void grdTask_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		row.Cells[_dcc["WORKITEMID"].Ordinal].Controls.Add(createAddLink());
		//rename headers
		row.Cells[_dcc["TASK_NUMBER"].Ordinal].Text = "Subtask";
		row.Cells[_dcc["PRIORITY"].Ordinal].Text = "Priority";
		row.Cells[_dcc["TITLE"].Ordinal].Text = "Title";
		row.Cells[_dcc["AssignedResource"].Ordinal].Text = "Assigned To";
		row.Cells[_dcc["PrimaryResource"].Ordinal].Text = "Primary Resource";
  //      row.Cells[_dcc["SecondaryResource"].Ordinal].Text = "Secondary Tech Resource";
  //      row.Cells[_dcc["PrimaryBusResource"].Ordinal].Text = "Primary Bus Resource";
  //      row.Cells[_dcc["SecondaryBusResource"].Ordinal].Text = "Secondary Bus Resource";
  //      row.Cells[_dcc["ESTIMATEDSTARTDATE"].Ordinal].Text = "Planned Start Date";
		//row.Cells[_dcc["ACTUALSTARTDATE"].Ordinal].Text = "Actual Start Date";
		//row.Cells[_dcc["ACTUALENDDATE"].Ordinal].Text = "Actual End Date";
		//row.Cells[_dcc["PLANNEDHOURS"].Ordinal].Text = "Estimated Effort";
		//row.Cells[_dcc["ACTUALHOURS"].Ordinal].Text = "Actual Effort";
		row.Cells[_dcc["COMPLETIONPERCENT"].Ordinal].Text = "% Comp"; 
		row.Cells[_dcc["STATUS"].Ordinal].Text = "Status";
        row.Cells[_dcc["AssignedToRank"].Ordinal].Text = "Assigned To Rank";
        row.Cells[_dcc["BusinessRank"].Ordinal].Text = "Customer Rank";
        //row.Cells[_dcc["BusinessRank"].Ordinal].Text = "Bus. Rank";
        //row.Cells[_dcc["SORT_ORDER"].Ordinal].Text = "Tech. Rank";
        //row.Cells[_dcc["SORT_ORDER"].Ordinal].Text = "Tech. Rank";
        row.Cells[_dcc["ReOpenedCount"].Ordinal].Text = "Re-Opened";
        row.Cells[_dcc["SRNumber"].Ordinal].Text = "SR Number";
        row.Cells[_dcc["INPROGRESSDATE"].Ordinal].Text = "In Progress Date";
        row.Cells[_dcc["READYFORREVIEWDATE"].Ordinal].Text = "Ready For Review Date";
        row.Cells[_dcc["DEPLOYEDDATE"].Ordinal].Text = "Deployed Date";
        row.Cells[_dcc["CLOSEDDATE"].Ordinal].Text = "Closed Date";
        //row.Cells[_dcc["Version"].Ordinal].Text = "Version";

        row.Cells[_dcc["Y"].Ordinal].Text = "&nbsp;";

		if (!CanEdit)
		{
			Image imgBlank = new Image();
			imgBlank.Height = 12;
			imgBlank.Width = 12;
			imgBlank.ImageUrl = "Images/Icons/blank.png";
			imgBlank.AlternateText = "";
			row.Cells[_dcc["Y"].Ordinal].Controls.Add(imgBlank);
		}
		else
		{
			Image imgSave = new Image();
			imgSave.ID = "imgSave";
			imgSave.Height = 12;
			imgSave.Width = 12;
			imgSave.ImageUrl = "Images/Icons/disk.png";
			imgSave.AlternateText = "Save Subtask Updates";
			imgSave.Style["cursor"] = "default";
			row.Cells[_dcc["Y"].Ordinal].Controls.Add(imgSave);
		}

        if (_dcc.Contains("X") && _dcc.Contains("WORKITEMID") && _dcc.Contains("WORKITEM_TASKID"))
        {
            if (this.CanEdit)
            {
                row.Cells[_dcc.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[_dcc.IndexOf("X")].Controls.Add(CreateCheckBox(true, row.Cells[_dcc.IndexOf("WORKITEMID")].Text, row.Cells[_dcc.IndexOf("WORKITEM_TASKID")].Text));
            }
        }
    }

	void grdTask_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridViewRow row = e.Row;
		formatColumnDisplay(ref row);

		string workItemId = row.Cells[_dcc["WORKITEMID"].Ordinal].Text.Trim();
		string taskId = row.Cells[_dcc["WORKITEM_TASKID"].Ordinal].Text.Trim();
		string taskNumber = row.Cells[_dcc["TASK_NUMBER"].Ordinal].Text.Trim();
		row.Attributes.Add("taskID", taskId);

		row.Cells[_dcc["WORKITEMID"].Ordinal].Controls.Add(createDeleteButton(taskId, taskNumber));
		row.Cells[_dcc["TASK_NUMBER"].Ordinal].Controls.Add(createEditLink(taskId, WorkItemID + " - " + taskNumber));

		TextBox txtTitle = WTSUtility.CreateGridTextBox("Title", taskId, HttpUtility.HtmlDecode(row.Cells[_dcc["Title"].Ordinal].Text.Trim()), false);
		txtTitle.ToolTip = txtTitle.Text;
		row.Cells[_dcc["Title"].Ordinal].Controls.Add(txtTitle);

        if (_dcc.Contains("X") && _dcc.Contains("WORKITEMID") && _dcc.Contains("WORKITEM_TASKID"))
        {
            if (this.CanEdit)
            {
                row.Cells[_dcc.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[_dcc.IndexOf("X")].Controls.Add(CreateCheckBox(false, row.Cells[_dcc.IndexOf("WORKITEMID")].Text, row.Cells[_dcc.IndexOf("WORKITEM_TASKID")].Text));
            }
        }

        if (_dsOptions != null && _dsOptions.Tables.Count > 0)
		{
			string id = "", value = "";

			if (_dsOptions.Tables.Contains("Priority"))
			{
				id = row.Cells[_dcc["PRIORITYID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				id = string.IsNullOrWhiteSpace(id) ? "0" : id;
				value = row.Cells[_dcc["PRIORITY"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;

				row.Cells[_dcc["PRIORITY"].Ordinal].Controls.Add(
					WTSUtility.CreateGridDropdownList(_dsOptions.Tables["Priority"], "Priority", "PRIORITY", "PRIORITYID", taskId, id, value));
			}

            if (_dsOptions.Tables.Contains("Rank"))
            {
                id = row.Cells[_dcc["AssignedToRankID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
                id = string.IsNullOrWhiteSpace(id) ? "0" : id;
                value = row.Cells[_dcc["AssignedToRank"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
                value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;

                row.Cells[_dcc["AssignedToRank"].Ordinal].Controls.Add(
                    WTSUtility.CreateGridDropdownList(_dsOptions.Tables["Rank"], "AssignedToRank", "PRIORITY", "PRIORITYID", taskId, id, value));
                
            }

            if (_dsOptions.Tables.Contains("User"))
			{
				row.Cells[_dcc["AssignedResource"].Ordinal].Controls.Add(
					WTSUtility.CreateGridDropdownList(_dsOptions.Tables["User"], "AssignedResource", "USERNAME", "WTS_RESOURCEID", taskId, row.Cells[_dcc["AssignedResourceID"].Ordinal].Text.Trim(), row.Cells[_dcc.IndexOf("AssignedResource")].Text.Trim()));

				id = row.Cells[_dcc["PrimaryResourceID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				id = string.IsNullOrWhiteSpace(id) ? "0" : id;
				value = row.Cells[_dcc["PrimaryResource"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
				value = string.IsNullOrWhiteSpace(value) ? "-Select-" : value;

				row.Cells[_dcc["PrimaryResource"].Ordinal].Controls.Add(
					WTSUtility.CreateGridDropdownList(_dsOptions.Tables["User"], "PrimaryResource", "USERNAME", "WTS_RESOURCEID", taskId, id, value));

                row.Cells[_dcc["PrimaryBusResource"].Ordinal].Controls.Add(
                    WTSUtility.CreateGridDropdownList(_dsOptions.Tables["User"], "PrimaryBusResource", "USERNAME", "WTS_RESOURCEID", taskId, row.Cells[_dcc["PrimaryBusResource"].Ordinal].Text.Trim(), row.Cells[_dcc.IndexOf("PrimaryBusResource")].Text.Trim()));
            }
            if (_dsOptions.Tables.Contains("PercentComplete"))
			{
				row.Cells[_dcc["CompletionPercent"].Ordinal].Controls.Add(
					WTSUtility.CreateGridDropdownList(_dsOptions.Tables["PercentComplete"], "PercentComplete", "Percent", "Percent", taskId, row.Cells[_dcc["CompletionPercent"].Ordinal].Text.Trim(), row.Cells[_dcc.IndexOf("CompletionPercent")].Text.Trim()));
			}
			if (_dsOptions.Tables.Contains("Status"))
			{
				DataTable dtTemp = _dsOptions.Tables["Status"];
				if (dtTemp != null && dtTemp.Rows.Count > 0)
				{
					dtTemp.DefaultView.RowFilter = string.Format(" WorkTypeID = {0} ", row.Cells[_dcc["WorkTypeID"].Ordinal].Text.Replace("&nbsp;", "").Trim());
					dtTemp = dtTemp.DefaultView.ToTable();
					row.Cells[_dcc["STATUS"].Ordinal].Controls.Add(
						WTSUtility.CreateGridDropdownList(dtTemp, "Status", "STATUS", "STATUSID", taskId, row.Cells[_dcc["STATUSID"].Ordinal].Text.Trim(), row.Cells[_dcc.IndexOf("STATUS")].Text.Trim()));
				}
			}
			row.Cells[_dcc["BusinessRank"].Ordinal].Controls.Add(WTSUtility.CreateGridTextBox("BusinessRank", taskId, row.Cells[_dcc["BusinessRank"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), true));
			row.Cells[_dcc["SORT_ORDER"].Ordinal].Controls.Add(WTSUtility.CreateGridTextBox("SORT_ORDER", taskId, row.Cells[_dcc["SORT_ORDER"].Ordinal].Text.Replace("&nbsp;", " ").Trim(), true));
		}

		TextBox txt = WTSUtility.CreateGridTextBox("EstimatedStartDate", taskId, row.Cells[_dcc["ESTIMATEDSTARTDATE"].Ordinal].Text.Replace("&nbsp;", " ").Trim());
		txt.Attributes.Add("date", "true");
        txt.Attributes.Add("original_value", "unchanged"); 
		txt.Style["width"] = "95%";
		txt.CssClass = "date";
		row.Cells[_dcc["ESTIMATEDSTARTDATE"].Ordinal].Controls.Add(txt);

		txt = WTSUtility.CreateGridTextBox("ActualStartDate", taskId, row.Cells[_dcc["ACTUALSTARTDATE"].Ordinal].Text.Replace("&nbsp;", " ").Trim());
		txt.Attributes.Add("date", "true");
        txt.Attributes.Add("original_value", "unchanged"); 
        txt.Style["width"] = "95%";
		txt.CssClass = "date";
		row.Cells[_dcc["ACTUALSTARTDATE"].Ordinal].Controls.Add(txt);

		txt = WTSUtility.CreateGridTextBox("ActualEndDate", taskId, row.Cells[_dcc["ACTUALENDDATE"].Ordinal].Text.Replace("&nbsp;", " ").Trim());
		txt.Attributes.Add("date", "true");
        txt.Attributes.Add("original_value", "unchanged"); 
        txt.Style["width"] = "95%";
		txt.CssClass = "date";
		row.Cells[_dcc["ACTUALENDDATE"].Ordinal].Controls.Add(txt);

		string originalEffortID = "", originalEffortValue = "", originalActualID = "", originalActualValue = "";
		originalEffortID = row.Cells[_dcc["EstimatedEffortID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
		originalEffortID = string.IsNullOrWhiteSpace(originalEffortID) ? "0" : originalEffortID;
		originalEffortValue = row.Cells[_dcc["PLANNEDHOURS"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
		originalEffortValue = string.IsNullOrWhiteSpace(originalEffortValue) ? "-Select-" : originalEffortValue;
		row.Cells[_dcc["PLANNEDHOURS"].Ordinal].Controls.Add(
			 WTSUtility.CreateGridDropdownList(_dsOptions.Tables["TshirtSizes"], "PLANNEDHOURS", "EFFORTSIZE", "EFFORTSIZEID", taskId, originalEffortID, originalEffortValue));

		originalActualID = row.Cells[_dcc["ActualEffortID"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
		originalActualID = string.IsNullOrWhiteSpace(originalActualID) ? "0" : originalActualID;
		originalActualValue = row.Cells[_dcc["ACTUALHOURS"].Ordinal].Text.Replace("&nbsp;", " ").Trim();
		originalActualValue = string.IsNullOrWhiteSpace(originalActualValue) ? "-Select-" : originalActualValue;
		row.Cells[_dcc["ACTUALHOURS"].Ordinal].Controls.Add(
			 WTSUtility.CreateGridDropdownList(_dsOptions.Tables["TshirtSizes"], "ACTUALHOURS", "EFFORTSIZE", "EFFORTSIZEID", taskId, originalActualID, originalActualValue));

		row.Cells[_dcc["PLANNEDHOURS"].Ordinal].Style["text-align"] = "right";
		row.Cells[_dcc["ACTUALHOURS"].Ordinal].Style["text-align"] = "right";
		row.Cells[_dcc["COMPLETIONPERCENT"].Ordinal].Style["text-align"] = "right";
		row.Cells[_dcc["SORT_ORDER"].Ordinal].Style["text-align"] = "right";
	}

	private void formatColumnDisplay(ref GridViewRow row)
	{
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != _dcc["WORKITEMID"].Ordinal
                && i != _dcc["X"].Ordinal
                && i != _dcc["Y"].Ordinal
				&& i != _dcc["TASK_NUMBER"].Ordinal
				&& i != _dcc["PRIORITY"].Ordinal
				&& i != _dcc["TITLE"].Ordinal
				&& i != _dcc["AssignedResource"].Ordinal
				&& i != _dcc["PrimaryResource"].Ordinal
				//&& i != _dcc["ESTIMATEDSTARTDATE"].Ordinal
				//&& i != _dcc["ACTUALSTARTDATE"].Ordinal
				//&& i != _dcc["ACTUALENDDATE"].Ordinal
				//&& i != _dcc["PLANNEDHOURS"].Ordinal
				//&& i != _dcc["ACTUALHOURS"].Ordinal
				&& i != _dcc["COMPLETIONPERCENT"].Ordinal
				&& i != _dcc["STATUS"].Ordinal
                && i != _dcc["AssignedToRank"].Ordinal
                && i != _dcc["BusinessRank"].Ordinal
                //&& i != _dcc["BusinessRank"].Ordinal
                //&& i != _dcc["SORT_ORDER"].Ordinal
                && i != _dcc["SRNumber"].Ordinal
                && i != _dcc["ReOpenedCount"].Ordinal
		        && i != _dcc["Version"].Ordinal
                && i != _dcc["INPROGRESSDATE"].Ordinal
                && i != _dcc["READYFORREVIEWDATE"].Ordinal
                && i != _dcc["DEPLOYEDDATE"].Ordinal
                && i != _dcc["CLOSEDDATE"].Ordinal
                )
			{
            row.Cells[i].Style["display"] = "none";
			}
		}

        if (!this.CanEdit)
        {
            row.Cells[_dcc.IndexOf("X")].Style["display"] = "none";
        }
        row.Cells[_dcc["X"].Ordinal].Style["border-left"] = "none";
        row.Cells[_dcc["WORKITEMID"].Ordinal].Style["border-left"] = "none";
		row.Cells[_dcc["WORKITEMID"].Ordinal].Style["text-align"] = "center";
		row.Cells[_dcc["WORKITEMID"].Ordinal].Style["width"] = "14px";
		row.Cells[_dcc["Y"].Ordinal].Style["text-align"] = "center";
		row.Cells[_dcc["Y"].Ordinal].Style["width"] = "14px";
		row.Cells[_dcc["ReOpenedCount"].Ordinal].Style["text-align"] = "center";

		row.Cells[_dcc["TASK_NUMBER"].Ordinal].Style["text-align"] = "left";
		row.Cells[_dcc["TASK_NUMBER"].Ordinal].Style["padding-left"] = "5px";
		row.Cells[_dcc["STATUS"].Ordinal].Style["text-align"] = "left";
		row.Cells[_dcc["STATUS"].Ordinal].Style["padding-left"] = "5px";
		row.Cells[_dcc["PRIORITY"].Ordinal].Style["text-align"] = "left";
		row.Cells[_dcc["PRIORITY"].Ordinal].Style["padding-left"] = "5px";
		row.Cells[_dcc["TITLE"].Ordinal].Style["text-align"] = "left";
		row.Cells[_dcc["TITLE"].Ordinal].Style["padding-left"] = "5px";
		row.Cells[_dcc["AssignedResource"].Ordinal].Style["text-align"] = "left";
		row.Cells[_dcc["AssignedResource"].Ordinal].Style["padding-left"] = "5px";
		row.Cells[_dcc["PrimaryResource"].Ordinal].Style["text-align"] = "left";
		row.Cells[_dcc["PrimaryResource"].Ordinal].Style["padding-left"] = "5px";
        //row.Cells[_dcc["SecondaryResource"].Ordinal].Style["text-align"] = "left";
        //row.Cells[_dcc["SecondaryResource"].Ordinal].Style["padding-left"] = "5px";
        //row.Cells[_dcc["PrimaryBusResource"].Ordinal].Style["text-align"] = "left";
        //row.Cells[_dcc["PrimaryBusResource"].Ordinal].Style["padding-left"] = "5px";
        //row.Cells[_dcc["SecondaryBusResource"].Ordinal].Style["text-align"] = "left";
        //row.Cells[_dcc["SecondaryBusResource"].Ordinal].Style["padding-left"] = "5px";

        //set widths
        row.Cells[_dcc["TASK_NUMBER"].Ordinal].Style["width"] = "90px"; 
		row.Cells[_dcc["PRIORITY"].Ordinal].Style["width"] = "65px";
		row.Cells[_dcc["AssignedResource"].Ordinal].Style["width"] = "145px";
		row.Cells[_dcc["PrimaryResource"].Ordinal].Style["width"] = "145px";
		//row.Cells[_dcc["ESTIMATEDSTARTDATE"].Ordinal].Style["width"] = "72px";
		//row.Cells[_dcc["ACTUALSTARTDATE"].Ordinal].Style["width"] = "72px";
		//row.Cells[_dcc["PLANNEDHOURS"].Ordinal].Style["width"] = "72px";
		//row.Cells[_dcc["ACTUALHOURS"].Ordinal].Style["width"] = "72px";
		//row.Cells[_dcc["ACTUALENDDATE"].Ordinal].Style["width"] = "72px";
        row.Cells[_dcc["CompletionPercent"].Ordinal].Style["width"] = "55px";

        row.Cells[_dcc["TITLE"].Ordinal].Style["width"] = "700px";
        //row.Cells[_dcc["TITLE"].Ordinal].Wrap = true;  // No effect

        row.Cells[_dcc["STATUS"].Ordinal].Style["width"] = "90px";
        row.Cells[_dcc["AssignedToRank"].Ordinal].Style["width"] = "125px";
        row.Cells[_dcc["BusinessRank"].Ordinal].Style["width"] = "20px";
        //row.Cells[_dcc["BusinessRank"].Ordinal].Style["width"] = "20px"; 
        //row.Cells[_dcc["SORT_ORDER"].Ordinal].Style["width"] = "20px";
        row.Cells[_dcc["ReOpenedCount"].Ordinal].Style["width"] = "50px";
        row.Cells[_dcc["INPROGRESSDATE"].Ordinal].Style["width"] = "50px";
        row.Cells[_dcc["READYFORREVIEWDATE"].Ordinal].Style["width"] = "50px";
        row.Cells[_dcc["DEPLOYEDDATE"].Ordinal].Style["width"] = "50px";
        row.Cells[_dcc["CLOSEDDATE"].Ordinal].Style["width"] = "50px";
    }

    private CheckBox CreateCheckBox(Boolean blnHeader, string WorkItemId, string WorkItemTaskId)
    {
        CheckBox chk = new CheckBox();

        if (blnHeader) {
            chk.Attributes["onchange"] = "chkAll(this);";
        }
        else { 
            chk.Attributes["onchange"] = "input_change(this);";
        }
        chk.Attributes.Add("WorkItemId", WorkItemId);
        chk.Attributes.Add("WorkItemTaskId", WorkItemTaskId);

        return chk;
    }

    Image createAddButton()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("imgAddTask_click();return false;");

		Image imgAdd = new Image();
		imgAdd.Style["cursor"] = "pointer";
		imgAdd.Height = 10;
		imgAdd.Width = 10;
		imgAdd.ImageUrl = "Images/Icons/add_blue.png";
		imgAdd.ID = "imgAddTask";
		imgAdd.AlternateText = "Add Subtask";
		imgAdd.Attributes.Add("Onclick", sb.ToString());

		return imgAdd;
	}

	LinkButton createAddLink()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("imgAddTask_click();return false;");

		LinkButton lb = new LinkButton();
		lb.ID = "lbAddTask";
		lb.ToolTip = "Add Subtask";
		lb.Text = "New";
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}

	LinkButton createEditLink(string taskId = "", string taskNumber = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEditTask_click('{0}','{1}');return false;", taskId, taskNumber);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEditTask_{0}", taskId);
		lb.Attributes["name"] = string.Format("lbEditTask_{0}", taskId);
		lb.ToolTip = string.Format("Edit Subtask [{0}]", taskNumber);
		lb.Text = taskNumber;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}

	Image createDeleteButton(string taskId = "", string taskNumber = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("imgDeleteTask_click('{0}','{1}');return false;", taskId, taskNumber);

		Image imgDelete = new Image();
		imgDelete.Style["cursor"] = "pointer";
		imgDelete.Height = 10;
		imgDelete.Width = 10;
		imgDelete.ImageUrl = "Images/Icons/delete.png";
		imgDelete.ID = string.Format("imgDeleteTask_{0}", taskId);
		imgDelete.Attributes["name"] = string.Format("imgDeleteTask_{0}", taskId);
		imgDelete.Attributes.Add("taskId", taskId.ToString());
		imgDelete.ToolTip = string.Format("Delete Subtask [{0}]", taskNumber);
		imgDelete.AlternateText = "Delete Subtask";
		imgDelete.Attributes.Add("Onclick", sb.ToString());

		return imgDelete;
	}


	private void loadTasks()
	{
		DataTable dt = WorkloadItem.WorkItem_GetTaskList(workItemID: this.WorkItemID == 0 ? this.sourceWorkItemID : this.WorkItemID, showArchived: _showArchived, showBacklog: ShowBacklog);

        // procName = "WORKITEM_GETTASKLIST

        if (dt != null)
		{
			loadQF_Status();

			_dcc = dt.Columns;
			this.Task_Count = dt.Rows.Count;
            try
            {
                Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(_dcc, Newtonsoft.Json.Formatting.None));
            }
            catch (Exception)
            {
            }

            StringBuilder sbRowFilter = new StringBuilder();
            StringBuilder sbRowFilterAsgn = new StringBuilder();
            StringBuilder sbRowFilterIn = new StringBuilder();
            StringBuilder sbRowFilterAssigned = new StringBuilder();
            //int AffiliatedID = 0;

            string[] filterArray = this.Filters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] AffiliatedArray = this.Filters.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

            //--------   Statuses ------------------------------------------------------------------------------------------------
            if (this.SelectedStatuses != null
                && this.SelectedStatuses.Length > 0)
            {
                for (int i = 0; i < this.SelectedStatuses.Length; i++)
                {
                    if (this.SelectedStatuses[i].Trim().Length > 0)
                    {
                        sbRowFilterIn.AppendFormat("{0}{1}", sbRowFilterIn.Length > 0 ? "," : "", SelectedStatuses[i].Trim());
                    }
                }
                if (sbRowFilterIn.Length > 0)
                {
                    sbRowFilter.AppendFormat(" STATUSID IN ({0}) ", sbRowFilterIn.ToString());
                }
            }
            else if (!this.ShowClosed)
            {
                sbRowFilter.AppendFormat(" STATUSID NOT IN (10, 70, 6) ", sbRowFilterIn.ToString());
            }

            if (sbRowFilter.Length > 0)
            {
                dt.DefaultView.RowFilter = sbRowFilter.ToString();
                dt = dt.DefaultView.ToTable();
            }
            //--------   Now Assigned to ------------------------------------------------------------------------------------------

            // 1-25-2017 Treating always as Affiliated, remarked out this code.
            // 4-13-2017 Reverting these back to using SelectedAssigned
            //if (filterArray.Length == 0)  // Workload Grid, length will be 0, filter by ASSIGNEDRESOURCEID here.
            //{
            if (this.SelectedAssigned != null && this.SelectedAssigned.Length > 0)
            {
                DataTable dtResourceTeamUsers = AOR.AORResourceTeamList_Get();

                for (int i = 0; i < this.SelectedAssigned.Length; i++)
                {
                    if (this.SelectedAssigned[i].Trim().Length > 0)
                    {
                        sbRowFilterAssigned.AppendFormat("{0}{1}", sbRowFilterAssigned.Length > 0 ? "," : "", SelectedAssigned[i].Trim());

                        try
                        {
                            //Add AOR Resource Team users the selected users are associated with
                            string aorResourceTeamUserIDs = string.Join(",",
                                dtResourceTeamUsers.AsEnumerable()
                               .Where(cols => cols.Field<int>("WTS_RESOURCEID").ToString() == SelectedAssigned[i].Trim())
                               .Select(cols => cols.Field<int>("ResourceTeamUserID")));

                            if (aorResourceTeamUserIDs.Length > 0) sbRowFilterAssigned.AppendFormat("{0}{1}", sbRowFilterAssigned.Length > 0 ? "," : "", aorResourceTeamUserIDs);
                        }
                        catch (Exception) { }
                    }
                }
                if (sbRowFilterAssigned.Length > 0)
                {
                    sbRowFilterAsgn.AppendFormat(" ASSIGNEDRESOURCEID IN ({0}) OR PRIMARYRESOURCEID IN ({0})", sbRowFilterAssigned.ToString());
                    //sbRowFilterAsgn.AppendFormat(" ASSIGNEDRESOURCEID IN ({0}) ", sbRowFilterAssigned.ToString());
                }
            }

            if (sbRowFilterAsgn.Length > 0)
            {
                dt.DefaultView.RowFilter = sbRowFilterAsgn.ToString();
                dt = dt.DefaultView.ToTable();
            }
            //}


            //try  // QM Workload Grid - work the filters.
            //{
            //    string nFilter = string.Empty;
            //    for (int i = 0; i < filterArray.Length; i++)
            //    {
            //        if (filterArray[i].Contains("Workload Assigned To"))
            //        {
            //            nFilter = filterArray[i].Replace("Workload Assigned To", "ASSIGNEDRESOURCEID");
            //        }
            //        else if (filterArray[i].Contains("Affiliated"))
            //        {
            //            // 1-20-2017 Changed from 1 to 2
            //            int.TryParse(AffiliatedArray[2], out AffiliatedID);
            //            //int.TryParse(AffiliatedArray[1], out AffiliatedID);
            //            nFilter = "ASSIGNEDRESOURCEID = " + AffiliatedID +
            //                " OR PRIMARYRESOURCEID = " + AffiliatedID +
            //                " OR SecondaryResourceID = " + AffiliatedID +
            //                " OR PrimaryBusResourceID = " + AffiliatedID +
            //                " OR SecondaryBusResourceID = " + AffiliatedID; //+ 
            //                //" OR SubmittedByID = " + AffiliatedID;  // 2-15-2017 - Removed this
            //        }
            //    }
            //    if (nFilter != string.Empty)
            //    {
            //        dt.DefaultView.RowFilter = nFilter;
            //        dt = dt.DefaultView.ToTable();
            //    }
            //}
            //catch (Exception ex) {
            //    string errMsg = ex.Message;
            //}

            Filtered_Count = dt.Rows.Count;

            InitializeColumnData(ref dt);

            dt = orderColumns(dt);
            dt.AcceptChanges();

            //grdTask.DataSource = dt;
            //grdTask.DataBind();

        }  // dt != null
    }

    private DataTable orderColumns(DataTable dt)
    {
        SortOrder = Workload.GetSortValuesFromDB(1, "WorkItem_Tasks.ASPX");

        columnData.SortDataTable(ref dt, SortOrder);
        SelectedColumnOrder = columnData.CurrentColumnOrderToString();

        this.columnData.SortDataTable(ref dt, this.SortOrder);
        return dt;
    }

    protected void InitializeColumnData(ref DataTable dt)
    {
        try
        {
            string displayName = string.Empty, groupName = string.Empty;
            bool blnVisible = false, isViewable = false, blnSortable = false, blnOrderable = false;

            foreach (DataColumn gridColumn in dt.Columns)
            {
                displayName = gridColumn.ColumnName;
                blnVisible = false;
                blnSortable = false;
                blnOrderable = true;
                isViewable = false;
                groupName = string.Empty;

                switch (gridColumn.ColumnName.ToUpper())
                {
                    case "WORKITEMID":
                        displayName = "Work Item ID";
                        blnVisible = false;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WORKITEM_TASKID":
                        displayName = "Sub Task";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "PRIORITYIDSORTED":
                        displayName = "Priority";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "BUSINESSRANK":
                        displayName = "Customer Rank";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "ASSIGNEDTORANK":
                        displayName = "Assigned To Rank";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "REOPENEDCOUNT": // Remove until working
                        displayName = "Re-Opened";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "ACTUALSTARTDATE":
                        displayName = "Actual Start";
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = true;
                        isViewable = false;
                        break;
                    case "ACTUALENDDATE":
                        displayName = "Actual End";
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = true;
                        isViewable = false;
                        break;
                    case "COMPLETIONPERCENT": // Remove until working
                        displayName = "% Complete";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "STATUSID":
                        displayName = "Status";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "ESTIMATEDEFFORTID":
                        displayName = "Estimated Effort";
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = true;
                        isViewable = false;
                        break;
                    case "ACTUALEFFORTID":
                        displayName = "Actual Effort";
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = true;
                        isViewable = false;
                        break;
                    case "TITLE":
                        displayName = "Title";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "ASSIGNEDRESOURCE":
                        displayName = "Assigned To";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "PRIMARYRESOURCE":
                        displayName = "Primary Resource";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "PRIMARYBUSRESOURCE":
                        displayName = "Primary Bus. Resource";
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "SECONDARYRESOURCE":
                        displayName = "Secondary Tech. Resource";
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "WORKTYPE":
                        displayName = "Resource Group";
                        blnVisible = false;
                        blnSortable = false;
                        blnOrderable = false;
                        isViewable = false;
                        break;
                    case "SRNUMBER":
                        displayName = "SR Number";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "Version":
                        displayName = "Version";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "INPROGRESSDATE":
                        displayName = "In Progress Date";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "DEPLOYEDDATE":
                        displayName = "Deployed Date";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "READYFORREVIEWDATE":
                        displayName = "Ready For Review Date";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                    case "CLOSEDDATE":
                        displayName = "Closed Date";
                        blnVisible = true;
                        blnSortable = true;
                        blnOrderable = true;
                        isViewable = true;
                        break;
                }
                isViewable = blnVisible;

                 columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
                columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
                columnData.Columns.Item(columnData.Columns.Count - 1).Viewable = isViewable;
            }
            //Initialize the columnData
            columnData.Initialize(ref dt, ";", "~", "|");

            //Get sortable columns and default column order
            SortableColumns = columnData.SortableColumnsToString();
            DefaultColumnOrder = columnData.DefaultColumnOrderToString();
            //Sort and Reorder Columns
            columnData.ReorderDataTable(ref dt, ColumnOrder);

            SortOrder = Workload.GetSortValuesFromDB(1, "WorkItem_Tasks.aspx");

            //columnData.SortDataTable(ref dt, SortOrder);
            SelectedColumnOrder = columnData.CurrentColumnOrderToString();

            this.columnData.SortDataTable(ref dt, this.SortOrder);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    private void loadQF_Status()
	{
		List<string> statusList = new List<string>();
        if (Session["Statuses"] != null && !ReloadStatusSession)
        {
            statusList = new List<string>(Session["Statuses"].ToString().Split(' '));
        }
        else
        { 
		    if (SelectedStatuses != null && SelectedStatuses.Length > 0)
		    {
			    foreach (string s in SelectedStatuses)
			    {
				    statusList.Add(s.Trim());
			    }
                Session["Statuses"] = string.Join(" ", statusList);
            }
        }

        DataTable dtStatus = MasterData.StatusList_Get(includeArchive: false);// dt.DefaultView.ToTable(true, new string[] { "STATUSID", "STATUS" });
		if (dtStatus != null)
		{
			ddlStatus.Items.Clear();

			dtStatus.DefaultView.RowFilter = " StatusType = 'Work' ";
			dtStatus = dtStatus.DefaultView.ToTable();
			ListItem item = null;
			foreach (DataRow dr in dtStatus.Rows)
			{
				item = new ListItem(dr["STATUS"].ToString(), dr["STATUSID"].ToString());
				item.Selected = (statusList.Count == 0 || statusList.Contains(dr["STATUSID"].ToString()));

				if (statusList.Count == 0 && (item.Text == "Closed" || item.Text == "Approved/Closed" || item.Text == "On Hold"))
				{
					item.Selected = this.ShowClosed;
				}

				ddlStatus.Items.Add(item);
			}
		}
	}

	[WebMethod(true)]
	public static string SaveChanges(string rows)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }
			, { "ids", "" }
			, { "error", "" } };
		bool saved = false, loaded = false;
		string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

		try
		{
			DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
			if (dtjson.Rows.Count == 0)
			{
				errorMsg = "Unable to save. No list of changes was provided.";
				saved = false;
			}

			int id = 0, priorityID = 0, assignedTo = 0, primaryResource = 0 //, secondaryResource = 0, primaryBusResource = 0, secondaryBusResource = 0
                //, estimatedEffortID = 0, plannedHours = 0, actualEffortID = 0, actualHours = 0
                , completionPercent = 0, statusID = 0, busRank = 0 //, sort = 0
                , AssignedToRankID = 0;
            //string plannedStartDate = string.Empty, actualStartDate = string.Empty, actualEndDate = string.Empty; 
			string title = string.Empty;

			//save
			foreach (DataRow dr in dtjson.Rows)
			{
				saved = loaded = false;
				id = 0;
				priorityID = statusID = 0;
				assignedTo = primaryResource = 0;
                //plannedStartDate = actualStartDate = string.Empty;
                //plannedHours = actualHours = 0;
                //estimatedEffortID = actualEffortID = 0;
                //actualEndDate = string.Empty;
                completionPercent = busRank = 0; //sort = 0;
				title = string.Empty;
                AssignedToRankID = 0;

                tempMsg = string.Empty;
				int.TryParse(dr["WORKITEM_TASKID"].ToString(), out id);
				int.TryParse(dr["PRIORITY"].ToString(), out priorityID);
				title = HttpUtility.HtmlDecode(Uri.UnescapeDataString(dr["Title"].ToString()));
				int.TryParse(dr["AssignedResource"].ToString(), out assignedTo);
				int.TryParse(dr["PrimaryResource"].ToString(), out primaryResource);
                //int.TryParse(dr["SECONDARYRESOURCEID"].ToString(), out secondaryResource);
                //int.TryParse(dr["PRIMARYBUSRESOURCEID"].ToString(), out primaryBusResource);  
                //int.TryParse(dr["SECONDARYBUSRESOURCEID"].ToString(), out secondaryBusResource);
                //plannedStartDate = dr["ESTIMATEDSTARTDATE"].ToString();
				//actualStartDate = dr["ACTUALSTARTDATE"].ToString();          
				//int.TryParse(dr["PLANNEDHOURS"].ToString(), out estimatedEffortID);  
                //int.TryParse(dr["ACTUALHOURS"].ToString(), out actualEffortID);  
                //actualEndDate = dr["ACTUALENDDATE"].ToString();
				int.TryParse(dr["STATUS"].ToString(), out statusID);
				int.TryParse(dr["COMPLETIONPERCENT"].ToString(), out completionPercent);
				int.TryParse(dr["BusinessRank"].ToString(), out busRank);
				//int.TryParse(dr["SORT_ORDER"].ToString(), out sort);
                int.TryParse(dr["AssignedToRank"].ToString(), out AssignedToRankID);

                WorkItem_Task task = new WorkItem_Task(id);
				loaded = task.Load();

				if (loaded)
				{
					task.PriorityID = priorityID;
					task.Title = title;
					task.AssignedResourceID = assignedTo;
					task.PrimaryResourceID = primaryResource;
                    //task.SecondaryResourceID = secondaryResource; 
                    //task.PrimaryBusResourceID = primaryBusResource;
                    //task.SecondaryBusResourceID = secondaryBusResource;
                    //task.EstimatedStartDate = plannedStartDate;
					//task.ActualStartDate = actualStartDate;
					//task.EstimatedEffortID = estimatedEffortID;
					//task.ActualEffortID = actualEffortID;
					//task.ActualEndDate = actualEndDate;
					task.StatusID = statusID;
					task.CompletionPercent = completionPercent;
					task.BusinessRank = busRank;
					//task.Sort_Order = sort;
                    task.AssignedToRankID = AssignedToRankID;

                    saved = task.Save(out errorMsg);
				}

				if (saved)
				{
					ids += string.Format("{0}{1}", ids.Length > 0 ? "," : "", id.ToString());

					Workload.SendWorkloadEmail("WorkItemTask", false, id);
				}

				if (tempMsg.Length > 0)
				{
					errorMsg = string.Format("{0}{1}{2}", errorMsg, errorMsg.Length > 0 ? Environment.NewLine : "", tempMsg);
				}
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
			errorMsg = ex.Message;
		}

		result["ids"] = ids;
		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

	[WebMethod(true)]
	public static string DeleteTask(int taskID)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "id", "0" }, { "error", "" } };
		bool loaded = false, deleted = false;
		string errorMsg = string.Empty;

		try
		{
			if (taskID == 0)
			{
				deleted = false;
				errorMsg = "No Subtask was specified.";
			}
			else
			{
				WorkItem_Task task = new WorkItem_Task(taskID: taskID);
				deleted = task.Delete(errorMsg: out errorMsg);
			}
		}
		catch (Exception ex)
		{
			deleted = false;
			errorMsg = ex.Message;
			LogUtility.LogException(ex);
		}

		result["deleted"] = deleted.ToString();
		result["id"] = taskID.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}

    [WebMethod()]
    public static string GetSubTasks(string workitem, string archived, string backlog, string status, string closed, string assigned, bool businessReview, string sort)
    {
        DataTable dt = new DataTable();
        int workItemID = 0, showArchived = 0;
        bool showBacklog = false, showClosed = false;

        int.TryParse(workitem, out workItemID);
        int.TryParse(archived, out showArchived);
        bool.TryParse(backlog, out showBacklog);
        bool.TryParse(closed, out showClosed);

        try
        {
            dt = WorkloadItem.WorkItem_GetTaskList(workItemID: workItemID, showArchived: showArchived, showBacklog: showBacklog);

            if (dt != null)
            {
                StringBuilder sbRowFilter = new StringBuilder();
                StringBuilder sbRowFilterAsgn = new StringBuilder();
                StringBuilder sbRowFilterIn = new StringBuilder();
                StringBuilder sbRowFilterAssigned = new StringBuilder();

                string[] SelectedStatuses = status.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] SelectedAssigned = assigned.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (SelectedStatuses != null
                    && SelectedStatuses.Length > 0)
                {
                    for (int i = 0; i < SelectedStatuses.Length; i++)
                    {
                        if (SelectedStatuses[i].Trim().Length > 0)
                        {
                            sbRowFilterIn.AppendFormat("{0}{1}", sbRowFilterIn.Length > 0 ? "," : "", SelectedStatuses[i].Trim());
                        }
                    }
                    if (sbRowFilterIn.Length > 0)
                    {
                        sbRowFilter.AppendFormat(" STATUSID IN ({0}) ", sbRowFilterIn.ToString());
                    }
                }
                else if (!showClosed)
                {
                    sbRowFilter.AppendFormat(" STATUSID NOT IN (10, 70, 6) ", sbRowFilterIn.ToString());
                }

                if (sbRowFilter.Length > 0)
                {
                    dt.DefaultView.RowFilter = sbRowFilter.ToString();
                    dt = dt.DefaultView.ToTable();
                }

                if (SelectedAssigned != null && SelectedAssigned.Length > 0)
                {
                    DataTable dtResourceTeamUsers = AOR.AORResourceTeamList_Get();

                    for (int i = 0; i < SelectedAssigned.Length; i++)
                    {
                        if (SelectedAssigned[i].Trim().Length > 0)
                        {
                            sbRowFilterAssigned.AppendFormat("{0}{1}", sbRowFilterAssigned.Length > 0 ? "," : "", SelectedAssigned[i].Trim());

                            try
                            {
                                //Add AOR Resource Team users the selected users are associated with
                                string aorResourceTeamUserIDs = string.Join(",",
                                    dtResourceTeamUsers.AsEnumerable()
                                   .Where(cols => cols.Field<int>("WTS_RESOURCEID").ToString() == SelectedAssigned[i].Trim())
                                   .Select(cols => cols.Field<int>("ResourceTeamUserID")));

                                if (aorResourceTeamUserIDs.Length > 0) sbRowFilterAssigned.AppendFormat("{0}{1}", sbRowFilterAssigned.Length > 0 ? "," : "", aorResourceTeamUserIDs);
                            }
                            catch (Exception) { }
                        }
                    }
                    if (sbRowFilterAssigned.Length > 0)
                    {
                        sbRowFilterAsgn.AppendFormat(" ASSIGNEDRESOURCEID IN ({0}) OR PRIMARYRESOURCEID IN ({0})", sbRowFilterAssigned.ToString());
                    }
                }

                if (sbRowFilterAsgn.Length > 0)
                {
                    dt.DefaultView.RowFilter = sbRowFilterAsgn.ToString();
                    dt = dt.DefaultView.ToTable();
                }

                if (businessReview)
                {
                    dt.DefaultView.RowFilter = "BusinessReview = 1";
                    dt = dt.DefaultView.ToTable();
                }

                if (sort.Length > 0)
                {
                    string[] all = sort.Split('~');

                    for (int i = 0; i < all.Length; i++)
                    {
                        all[i] = all[i].Replace("Subtask", "TASK_NUMBER")
                            .Replace("Customer Rank", "BUSINESSRANK")
                            .Replace("Assigned To Rank", "ASSIGNEDTORANK")
                            .Replace("Priority", "PRIORITYIDSORTED")
                            .Replace("Assigned To", "ASSIGNEDRESOURCE")
                            .Replace("Primary Resource", "PRIMARYRESOURCE")
                            .Replace("% Comp", "COMPLETIONPERCENT")
                            .Replace("Status", "STATUS")
                            .Replace("SR Number", "SRNUMBER")
                            .Replace("Re-Opened", "REOPENEDCOUNT")
                            .Replace("In Progress Date", "INPROGRESSDATE")
                            .Replace("Ready for Review", "READYFORREVIEWDATE")
                            .Replace("Deployed Date", "DEPLOYEDDATE")
                            .Replace("Closed Date", "CLOSEDDATE");

                        if (!all[i].Contains("|") || all[i].Split('|')[0] == "") all[i] = "";

                        all[i] = all[i].Replace("|", " ")
                            .Replace("1", "ASC")
                            .Replace("2", "DESC");
                    }

                    all = all.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                    dt.DefaultView.Sort = string.Join(", ", all);
                    dt = dt.DefaultView.ToTable();
                }

                dt.Columns["TITLE"].MaxLength = 255;
                foreach (DataRow dr in dt.Rows)
                {
                    dr["TITLE"] = dr["TITLE"] != DBNull.Value ? HttpUtility.HtmlEncode(dr["TITLE"].ToString()) : "";
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }
}