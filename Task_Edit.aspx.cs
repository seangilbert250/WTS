﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Newtonsoft.Json;


public partial class Task_Edit : System.Web.UI.Page
{
	protected int WorkItemID = 0;
	protected int WorkItem_TaskID = 0;
	protected int SystemID = 0;
	protected int UnclosedSRTasks = 0;
    protected int ProductVersionID = 0;
    protected int SaveComplete = 0;
    protected string AORReleaseID = string.Empty;

    protected bool CanEdit = false;
    protected bool CanEditAOR = false;
    protected bool CanEditWorkloadMGMT = false;
    protected bool ReadOnly = false; // this overrides other edit flags

    protected bool IsNew = false;
    protected string AltRefreshAfterCloseFunction = string.Empty;
    protected string ParentRelAORReleaseID = string.Empty;


    protected void Page_Load(object sender, EventArgs e)
	{
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItemTask);
        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanEditWorkloadMGMT = UserManagement.UserCanEdit(WTSModuleOption.WorkloadMGMT);

        readQueryString();
        LoadRelatedItemsMenu();
        if (this.ReadOnly)
        {
            this.CanEdit = false;
            this.CanEditAOR = false;
        }

		loadLookupData();

		if (this.WorkItem_TaskID > 0)
		{
			loadTask();
        }
		else
		{
			WorkloadItem wi = WorkloadItem.WorkItem_GetObject(workItemID: this.WorkItemID);

            ListItem li = null;
            li = ddlAssignedTo.Items.FindByValue(wi.AssignedResourceID.ToString());
            if (li == null)
            {
                int uID = 0;
                int.TryParse(wi.AssignedResourceID.ToString(), out uID);
                WTS_User u = new WTS_User(uID);
                u.Load();

                li = new ListItem(u.First_Name + " " + u.Last_Name, wi.AssignedResourceID.ToString());

                if (u.AORResourceTeam)
                {
                    li.Attributes.Add("og", "Action Team");

                    DataTable dt = AOR.AORResourceTeamList_Get(AORID: 0, AORReleaseID: 0);
                    dt.DefaultView.RowFilter = "ResourceTeamUserID = " + uID;
                    dt = dt.DefaultView.ToTable();
                    if (dt.Rows.Count > 0) li.Attributes.Add("aorid", dt.Rows[0]["AORID"].ToString());

                    ddlAssignedTo.Items.Insert(1, li);
                }
                else
                {
                    ddlAssignedTo.Items.Insert(0, li);
                }
            }
            li.Selected = true;

            WTSUtility.SelectDdlItem(ddlPrimaryResource, wi.PrimaryResourceID.ToString(), "");

            // 12817 - 30:
            WTSUtility.SelectDdlItem(ddlPrimaryBusResource, wi.PrimaryBusinessResourceID.ToString(), "");
            WTSUtility.SelectDdlItem(ddlSecondaryResource, wi.SecondaryResourceID.ToString(), "");
            WTSUtility.SelectDdlItem(ddlSecondaryBusResource, wi.SecondaryBusinessResourceID.ToString(), "");

            WTSUtility.SelectDdlItem(ddlPriority, wi.PriorityID.ToString(), "");
            //WTSUtility.SelectDdlItem(ddlHours_Planned, wi.EstimatedEffortID.ToString(), "");
            WTSUtility.SelectDdlItem(ddlProductVersion, wi.ProductVersionID.ToString(), "");
            
            this.lblProductVersion.Text = ddlProductVersion.SelectedItem.Text;
            this.SystemID = wi.WTS_SystemID;

            IsNew = true;
			this.txtWorkloadNumber.Text = this.WorkItemID.ToString() + " - NA";

            DataTable parentTaskData = WorkloadItem.WorkItem_Get(workItemID: WorkItemID);
            txtParentTitle.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["TITLE"].ToString().Replace("&nbsp;", "").Trim()));
            txtResourceGroup.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["WorkType"].ToString().Replace("&nbsp;", "").Trim()));
            txtResourceGroup.Attributes.Add("ResourceGroupID", HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["WorkTypeID"].ToString())));
            txtFunctionality.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["WorkloadGroup"].ToString().Replace("&nbsp;", "").Trim()));
            txtProductionStatus.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["ProductionStatus"].ToString().Replace("&nbsp;", "").Trim()));
            txtSystemTask.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["WTS_SYSTEM"].ToString().Replace("&nbsp;", "").Trim()));
            txtContract.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["CONTRACT"].ToString().Replace("&nbsp;", "").Trim()));
            txtWorkloadAllocation.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["WorkloadAllocation"].ToString().Replace("&nbsp;", "").Trim()));

            int workAreaID = 0, systemID = 0;
            int.TryParse(parentTaskData.Rows[0]["WorkAreaID"].ToString(), out workAreaID);
            int.TryParse(parentTaskData.Rows[0]["WTS_SYSTEMID"].ToString(), out systemID);

            WTSUtility.SelectDdlItem(ddlWorkItemType, parentTaskData.Rows[0]["WorkItemTypeID"].ToString(), parentTaskData.Rows[0]["WorkItemType"].ToString());

            //dynamic filters = "{ 'System(Task)':{ 'value':'" + parentTaskData.Rows[0]["WTS_SYSTEMID"].ToString() + "','text':'" + parentTaskData.Rows[0]["WTS_SYSTEM"].ToString() + "'},'Work Area':{ 'value':'" + parentTaskData.Rows[0]["WorkAreaID"].ToString() + "','text':'" + parentTaskData.Rows[0]["WorkArea"].ToString() + "'}}";
            //dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>(filters);
            //bool saved = Filtering.SaveWorkFilters(module: "RQMT", filterModule: "RQMT", filters: fields, myData: false, xml: "");

            DataTable dtWorkArea = MasterData.WorkArea_SystemList_Get(workAreaID: workAreaID);
            string workAreaText = string.Empty;
            if (dtWorkArea != null && dtWorkArea.Rows.Count > 1)
            {
                workAreaText = dtWorkArea.Rows[1]["ApprovedPriority"].ToString() + " - " + dtWorkArea.Rows[1]["WorkArea"].ToString();
            }

            txtWorkArea.Text = HttpUtility.HtmlDecode(workAreaText.Replace("&nbsp;", "").Trim());

            DataTable dtAORs = AOR.AORTaskAORList_Get(TaskID: WorkItemID);
            if (dtAORs != null && dtAORs.Rows.Count > 0)
            {
                foreach (DataRow dr in dtAORs.Rows)
                {
                    if (dr["AORWorkTypeID"].ToString() == "1")
                    {
                        txtWorkloadAOR.Text = dr["AORID"].ToString() + " (" + dr["Abbreviation"].ToString() + ") - " + dr["AORName"].ToString();
                        txtWorkloadAOR.ToolTip = txtWorkloadAOR.Text;
                    }
                    if (dr["AORWorkTypeID"].ToString() == "2")
                    {
                        txtReleaseAOR.Text = dr["AORID"].ToString() + " (" + dr["Abbreviation"].ToString() + ") - " + dr["AORName"].ToString();
                        txtReleaseAOR.ToolTip = txtReleaseAOR.Text;
                        ParentRelAORReleaseID = dr["AORReleaseID"].ToString();
                    }
                }
            }

            ListItem item = ddlStatus.Items.FindByText("New");
			if (item != null)
			{
				item.Selected = true;
			}

            filterStatuses(true);

            if (wi != null)
			{
				int maxSort = 0, maxBusRank = 0;
				DataTable dt = WorkloadItem.WorkItem_GetTaskList(wi.WorkItemID, 1);
				if (dt != null && dt.Rows.Count > 0)
				{
					int sort = 0;
					foreach (DataRow row in dt.Rows)
					{
						int.TryParse(row["SORT_ORDER"].ToString(), out sort);
						if (sort > maxSort)
						{
							maxSort = sort;
						}
						int.TryParse(row["BusinessRank"].ToString(), out sort);
						if (sort > maxBusRank)
						{
							maxBusRank = sort;
						}
					}
				}
				txtSortOrder.Text = (maxSort + 1).ToString();
				txtBusinessRank.Text = (maxBusRank + 1).ToString();
			}

            item = ddlAssignedToRank.Items.FindByText("5 - Unprioritized Workload");
            if (item != null) item.Selected = true;
            txtBusinessRank.Text = "99";
        }
        LoadAORs();
    }
    private void readQueryString()
	{
		if (Request.QueryString["workItemID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["workItemID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["workItemID"].ToString()), out this.WorkItemID);
		}
		if (Request.QueryString["taskID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["taskID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["taskID"].ToString()), out this.WorkItem_TaskID);
		}

		if (Request.QueryString["Saved"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["Saved"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["Saved"].ToString()), out this.SaveComplete);
		}

        if (Request.QueryString["ReadOnly"] != null)
        {
            ReadOnly = Request.QueryString["ReadOnly"].ToLower() == "true" || Request.QueryString["ReadOnly"] == "1";
        }

        if (Request.QueryString["AltRefreshAfterCloseFunction"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AltRefreshAfterCloseFunction"]))
        {
            AltRefreshAfterCloseFunction = Request.QueryString["AltRefreshAfterCloseFunction"];
        }

    }

    /// <summary>
    /// This will load our Related Items Menu with the data from our WTS_Menus.xml file on our server
    /// </summary>
    private void LoadRelatedItemsMenu()
    {
        try
        {
            DataSet dsMenu = new DataSet();
            dsMenu.ReadXml(this.Server.MapPath("XML/WTS_Menus.xml"));

            if (dsMenu.Tables.Count > 0 && dsMenu.Tables[0].Rows.Count > 0)
            {
                if (dsMenu.Tables.Contains("WorkTaskRelatedItem"))
                {
                    menuRelatedItems.DataSource = dsMenu.Tables["WorkTaskRelatedItem"];
                    menuRelatedItems.DataValueField = "URL";
                    menuRelatedItems.DataTextField = "Text";
                    menuRelatedItems.DataIDField = "id";
                    if (dsMenu.Tables["WorkTaskRelatedItem"].Columns.Contains("WorkTaskRelatedItem_id_0"))
                    {
                        menuRelatedItems.DataParentIDField = "WorkTaskRelatedItem_id_0";
                    }
                    menuRelatedItems.DataImageField = "ImageType";
                    menuRelatedItems.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
        this.Server.MapPath(null);
    }

    private void loadLookupData()
	{
		DataSet dsOptions = WorkItem_Task.GetAvailableOptions(workItemID: this.WorkItemID);

		if (dsOptions == null || dsOptions.Tables.Count == 0)
		{
			return;
		}

		ListItem item = null;

		if (dsOptions.Tables.Contains("Priority"))
		{
			ddlPriority.DataSource = dsOptions.Tables["Priority"];
			ddlPriority.DataTextField = "PRIORITY";
			ddlPriority.DataValueField = "PRIORITYID";
			ddlPriority.DataBind();
		}

        if (dsOptions.Tables.Contains("TshirtSizes"))
        {
            ddlHours_Planned.DataSource = dsOptions.Tables["TshirtSizes"];
            ddlHours_Planned.DataTextField = "EFFORTSIZE";
            ddlHours_Planned.DataValueField = "EFFORTSIZEID";
            ddlHours_Planned.DataBind();
        }

        if (dsOptions.Tables.Contains("TshirtSizes"))
        {
            ddlHours_Actual.DataSource = dsOptions.Tables["TshirtSizes"];
            ddlHours_Actual.DataTextField = "EFFORTSIZE";
            ddlHours_Actual.DataValueField = "EFFORTSIZEID";
            ddlHours_Actual.DataBind();
        }
	
		if (dsOptions.Tables.Contains("User"))
		{
			item = null;
			string name = string.Empty;

			foreach (DataRow row in dsOptions.Tables["User"].Rows)
			{
				name = string.Format("{0} {1}", row["FIRST_NAME"].ToString(), row["LAST_NAME"].ToString());
				item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
				item.Attributes.Add("username", row["USERNAME"].ToString());
				item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());

				ddlAssignedTo.Items.Add(item);

				item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
				item.Attributes.Add("username", row["USERNAME"].ToString());
				item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());

				ddlPrimaryResource.Items.Add(item);

                item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
                item.Attributes.Add("username", row["USERNAME"].ToString());
                item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());

                ddlSecondaryResource.Items.Add(item);

                item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
                item.Attributes.Add("username", row["USERNAME"].ToString());
                item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());

                ddlPrimaryBusResource.Items.Add(item);

                item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
                item.Attributes.Add("username", row["USERNAME"].ToString());
                item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());

                ddlSecondaryBusResource.Items.Add(item);

            }
        }
		if (dsOptions.Tables.Contains("Status"))
		{
			ddlStatus.DataSource = dsOptions.Tables["Status"];
			ddlStatus.DataTextField = "Status";
			ddlStatus.DataValueField = "StatusID";
			ddlStatus.DataBind();
		}
		if (dsOptions.Tables.Contains("PercentComplete"))
		{
			ddlPercentComplete.DataSource = dsOptions.Tables["PercentComplete"];
			ddlPercentComplete.DataTextField = "Percent";
			ddlPercentComplete.DataValueField = "Percent";
			ddlPercentComplete.DataBind();
		}

        if (dsOptions.Tables.Contains("Rank"))
        {
            ddlAssignedToRank.DataSource = dsOptions.Tables["Rank"];
            ddlAssignedToRank.DataTextField = "PRIORITY";
            ddlAssignedToRank.DataValueField = "PRIORITYID";
            ddlAssignedToRank.DataBind();
        }

        if (dsOptions.Tables.Contains("ProductVersion"))
        {
            ddlProductVersion.DataSource = dsOptions.Tables["ProductVersion"];
            ddlProductVersion.DataTextField = "ProductVersion";
            ddlProductVersion.DataValueField = "ProductVersionID";
            ddlProductVersion.DataBind();
        }

        if (dsOptions.Tables.Contains("WorkItemType"))
        {
            DataTable dtWorkItemType = dsOptions.Tables["WorkItemType"];
            int workloadAllocationID = 0;
            //string currentPhaseID = string.Empty;
            string currentWorkActivityGroup = string.Empty;
            ListItem li;
            DataTable dtAOR = AOR.AORTaskAORList_Get(TaskID: (WorkItemID == 0 ? -1 : WorkItemID));

            dtAOR.DefaultView.RowFilter = "AORWorkTypeID = 2";
            dtAOR = dtAOR.DefaultView.ToTable();

            if (dtAOR.Rows.Count > 0) int.TryParse(dtAOR.Rows[0]["WorkloadAllocationID"].ToString(), out workloadAllocationID);

            foreach (DataRow dr in dtWorkItemType.Rows)
            {
                if (/*dr["PDDTDR_PHASEID"].ToString() == "" || */dr["WorkloadAllocationID"].ToString() == "" || dr["WorkloadAllocationID"].ToString() == workloadAllocationID.ToString())
                {
                    /*if (currentPhaseID != dr["PDDTDR_PHASEID"].ToString() && dr["PDDTDR_PHASEID"].ToString() != "")
                    {
                        li = new ListItem();
                        li.Text = dr["PDDTDR_PHASE"].ToString();
                        li.Attributes["disabled"] = "disabled";
                        li.Attributes["style"] = "background-color: white";
                        ddlWorkItemType.Items.Add(li);

                        currentPhaseID = dr["PDDTDR_PHASEID"].ToString();
                    }*/

                    if (currentWorkActivityGroup != dr["WorkActivityGroup"].ToString() && dr["WorkActivityGroup"].ToString() != "")
                    {
                        li = new ListItem();
                        li.Text = dr["WorkActivityGroup"].ToString();
                        li.Attributes["disabled"] = "disabled";
                        li.Attributes["style"] = "background-color: white";
                        ddlWorkItemType.Items.Add(li);

                        currentWorkActivityGroup = dr["WorkActivityGroup"].ToString();
                    }

                    li = new ListItem();
                    li.Value = dr["WORKITEMTYPEID"].ToString();
                    li.Text = dr["WORKITEMTYPE"].ToString();
                    ddlWorkItemType.Items.Add(li);
                }
            }
        }
    }

    private void LoadAORs()
    {
        int assignedTo_ID = 0, primaryResource_ID = 0;
        string prodStatusMsg = "Release/Deployment MGMT AOR not applicable for this task type";

        int.TryParse(ddlAssignedTo.SelectedValue, out assignedTo_ID);
        int.TryParse(ddlPrimaryResource.SelectedValue, out primaryResource_ID);

        DataTable dtAORLookup = AOR.AORTaskOptionsList_Get(AssignedToID: assignedTo_ID, PrimaryResourceID: primaryResource_ID, SystemID: this.SystemID, SystemAffiliated: 1, ResourceAffiliated: 0, All: 0);
        DataTable dtAORs = new DataTable();
        StringBuilder sbAORs = new StringBuilder();

        bool cascade = false;
        DataTable dtCascade = AOR.AORTaskCascadeAOR_Get(this.WorkItemID);
        if (dtCascade.Rows.Count > 0)
        {
            foreach (DataRow dr in dtCascade.Rows)
            {
                bool.TryParse(dr["CascadeAOR"].ToString(), out cascade);
            }
        }

        if (this.WorkItem_TaskID > 0)
        {
            dtAORs = AOR.AORSubTaskAORList_Get(TaskID: WorkItem_TaskID);
        } else if (this.WorkItem_TaskID == 0 && cascade)
        {
            dtAORs = dtCascade;
        }

        sbAORs.Append("<table style=\"border-collapse: collapse;\">");

        if (dtAORs.Rows.Count > 0)
        {
             this.AORReleaseID = dtAORs.Rows[0]["AORReleaseID"].ToString();

            aorLabel.Text = dtAORs.Rows[0]["AORType"].ToString() + " AOR:";
            sbAORs.Append("<tr class=\"gridBody\">");

            if (this.CanEdit && !cascade)
            {
                sbAORs.Append("<td id=\"MgmtWorkloadAORs\" style=\"text-align: center; padding: 0px; border-bottom: 0px solid grey; border-right: 0px solid grey;\">");
                sbAORs.Append("<select field=\"" + dtAORs.Rows[0]["AORType"].ToString() + "\" original_value=\"" + dtAORs.Rows[0]["AORReleaseID"].ToString() + "\" style=\"width: 325px; background-color: #F5F6CE;\"");

                if (!this.CanEditWorkloadMGMT) sbAORs.Append(" disabled");

                sbAORs.Append(">"); //select
                sbAORs.Append("<option value=\"0\"></option>");

                bool contains = dtAORLookup.AsEnumerable().Any(row => dtAORs.Rows[0]["AORReleaseID"].ToString() == row.Field<Int32>("AORReleaseID").ToString());

                if (!contains) sbAORs.Append("<option value=\"" + dtAORs.Rows[0]["AORReleaseID"].ToString() + "\" selected>" + dtAORs.Rows[0]["AORID"].ToString() + " (" + dtAORs.Rows[0]["Abbreviation"].ToString() + ") - " + dtAORs.Rows[0]["AORName"].ToString() + "</option>");

                foreach (DataRow dr in dtAORLookup.Rows)
                {
                    if (dr["AORType"].ToString() != "Release/Deployment MGMT"
                        && dtAORs.Rows[0]["AORType"].ToString() != "Release/Deployment MGMT")
                    {
                        sbAORs.Append("<option value=\"" + dr["AORReleaseID"].ToString() + "\"");
                        if (dr["AORReleaseID"].ToString() == dtAORs.Rows[0]["AORReleaseID"].ToString()) sbAORs.Append(" selected");

                        sbAORs.Append(">" + dr["AORID"].ToString() + " (" + dr["WorkloadAllocationAbbreviation"].ToString() + ") - " + dr["AORName"].ToString() + "</option>");
                    }
                }

                sbAORs.Append("</select>");

                if (this.CanEditWorkloadMGMT) sbAORs.Append("<img src=\"Images/Icons/cog.png\" alt=\"AOR Option Settings\" title=\"AOR Option Settings\" class=\"showaoroptionsettings\" width=\"15\" height=\"15\" onclick=\"showAOROptionSettings(this);\" style=\"cursor: pointer; margin-left: 3px; vertical-align: middle\" />");

                sbAORs.Append("<div class=\"aoroptionsettings\" style=\"text-align: left; border: 1px solid gray; position: absolute; background-color: white; padding: 5px; display: none;\">");
                sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" checked />Affiliated by selected System</label><br />");
                sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />Affiliated by selected Assigned To/Primary Resource</label><br />");
                sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />Affiliated by AOR Workload Type</label><br />");
                sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />All (grouped by System)</label>");
                sbAORs.Append("</div>");
                sbAORs.Append("</td>");
                sbAORs.Append("<td id=\"MgmtReleaseAORsMsg\" height=\"20\" style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; display: none;\">" + prodStatusMsg + "</td>");
            }
            else
            {
                sbAORs.Append("<td id=\"aorReleaseID\" value=\"" + dtAORs.Rows[0]["AORReleaseID"].ToString() + "\" height =\"20\" style=\"padding: 0px; border: 0px solid grey; text-align: center;\">" + dtAORs.Rows[0]["AORID"].ToString() + " - " + dtAORs.Rows[0]["AORName"].ToString());
            }

            sbAORs.Append("</td>");
            sbAORs.Append("</tr>");
            sbAORs.Append("</table>");
             divWorkloadAORs.InnerHtml = sbAORs.ToString();
            sbAORs.Clear();
        }
        else
        {
            string aorType = "Workload MGMT";
            aorLabel.Text = aorType + " AOR:";
            sbAORs.Append("<tr class=\"gridBody\">");

            if (this.CanEdit && !cascade)
            {
                sbAORs.Append("<td id=\"MgmtWorkloadAORs\" style=\"text-align: center; padding: 0px; border-bottom: 0px solid grey; border-right: 0px solid grey;\">");
                sbAORs.Append("<select field=\"" + aorType + "\" original_value=\"0\" style=\"width: 325px; background-color: #F5F6CE;\"");

                if (!this.CanEditWorkloadMGMT) sbAORs.Append(" disabled");

                sbAORs.Append(">"); //select
                sbAORs.Append("<option value=\"0\"></option>");

                foreach (DataRow dr in dtAORLookup.Rows)
                {
                    if (dr["AORType"].ToString() != "Release/Deployment MGMT"
                        && aorType.ToString() != "Release/Deployment MGMT")
                    {
                        sbAORs.Append("<option value=\"" + dr["AORReleaseID"].ToString() + "\"");
                        sbAORs.Append(">" + dr["AORID"].ToString() + " (" + dr["WorkloadAllocationAbbreviation"].ToString() + ") - " + dr["AORName"].ToString() + "</option>");
                    }
                }

                sbAORs.Append("</select>");

                if (this.CanEditWorkloadMGMT) sbAORs.Append("<img src=\"Images/Icons/cog.png\" alt=\"AOR Option Settings\" title=\"AOR Option Settings\" class=\"showaoroptionsettings\" width=\"15\" height=\"15\" onclick=\"showAOROptionSettings(this);\" style=\"cursor: pointer; margin-left: 3px; vertical-align: middle\" />");

                sbAORs.Append("<div class=\"aoroptionsettings\" style=\"text-align: left; border: 1px solid gray; position: absolute; background-color: white; padding: 5px; display: none;\">");
                sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" checked />Affiliated by selected System</label><br />");
                sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />Affiliated by selected Assigned To/Primary Resource</label><br />");
                sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />Affiliated by AOR Workload Type</label><br />");
                sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />All (grouped by System)</label>");
                sbAORs.Append("</div>");
                sbAORs.Append("</td>");
                sbAORs.Append("<td id=\"MgmtReleaseAORsMsg\" height=\"20\" style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; display: none;\">" + prodStatusMsg + "</td>");
            }
            else
            {
                sbAORs.Append("<td height=\"20\" style=\"padding: 0px; border: 0px solid grey; text-align: center;\">No " + aorType + " AORs</td>");
            }
            sbAORs.Append("</td>");
            sbAORs.Append("</tr>");
            sbAORs.Append("</table>");
            divWorkloadAORs.InnerHtml = sbAORs.ToString();
            sbAORs.Clear();
        }
    }

    private void loadTask()
	{
		WorkItem_Task task = new WorkItem_Task(taskID: this.WorkItem_TaskID);

		if (task == null || !task.Load())
		{
			return;
		}

		txtWorkloadNumber.Text = string.Format("{0} - {1}", task.WorkItemID.ToString(), task.Task_Number.ToString());
		WTSUtility.SelectDdlItem(ddlPriority, task.PriorityID.ToString(), task.Priority.ToString());
		txtParentTitle.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(task.ParentTitle.Replace("&nbsp;", "").Trim()));
		txtTitle.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(task.Title.Replace("&nbsp;", "").Trim()));

        ListItem li = null;
        li = ddlAssignedTo.Items.FindByValue(task.AssignedResourceID.ToString());
        if (li == null)
        {
            li = new ListItem(task.AssignedResource.ToString(), task.AssignedResourceID.ToString());
            int uID = 0;
            int.TryParse(task.AssignedResourceID.ToString(), out uID);
            WTS_User u = new WTS_User(uID);
            u.Load();

            if (u.AORResourceTeam)
            {
                li.Attributes.Add("og", "Action Team");

                DataTable dt = AOR.AORResourceTeamList_Get(AORID: 0, AORReleaseID: 0);
                dt.DefaultView.RowFilter = "ResourceTeamUserID = " + uID;
                dt = dt.DefaultView.ToTable();
                if (dt.Rows.Count > 0) li.Attributes.Add("aorid", dt.Rows[0]["AORID"].ToString());

                ddlAssignedTo.Items.Insert(1, li);
            }
            else
            {
                ddlAssignedTo.Items.Insert(0, li);
            }
        }
        li.Selected = true;

        WTSUtility.SelectDdlItem(ddlPrimaryResource, task.PrimaryResourceID.ToString(), task.PrimaryResource.ToString());
        WTSUtility.SelectDdlItem(ddlSecondaryResource, task.SecondaryResourceID.ToString(), task.SecondaryResource.ToString());
        WTSUtility.SelectDdlItem(ddlPrimaryBusResource, task.PrimaryBusResourceID.ToString(), task.PrimaryBusResource.ToString());
        WTSUtility.SelectDdlItem(ddlSecondaryBusResource, task.SecondaryBusResourceID.ToString(), task.SecondaryBusResource.ToString());
        txtStartDate_Planned.Text = task.EstimatedStartDate;
		txtStartDate_Actual.Text = task.ActualStartDate;
		txtEndDate_Actual.Text = task.ActualEndDate;
        WTSUtility.SelectDdlItem(ddlHours_Planned, task.EstimatedEffortID.ToString(), task.PlannedHours.ToString());
        WTSUtility.SelectDdlItem(ddlHours_Actual, task.ActualEffortID.ToString(), task.ActualHours.ToString());
		WTSUtility.SelectDdlItem(ddlPercentComplete, task.CompletionPercent.ToString(), task.CompletionPercent.ToString());
		WTSUtility.SelectDdlItem(ddlStatus, task.StatusID.ToString(), task.Status);

        filterStatuses(false);

        txtBusinessRank.Text = task.BusinessRank.ToString();
		txtSortOrder.Text = task.Sort_Order.ToString();
		textAreaDescription.InnerHtml = task.Description;
        txtSRNumber.Text = task.SRNumber.ToString();
        
        WTSUtility.SelectDdlItem(ddlAssignedToRank, task.AssignedToRankID.ToString(), "");
        WTSUtility.SelectDdlItem(ddlProductVersion, task.ProductVersionID.ToString(), task.ProductVersion.ToString());
        WTSUtility.SelectDdlItem(ddlWorkItemType, task.WorkItemTypeID.ToString(), task.WorkItemType.ToString());
        this.ProductVersionID = task.ProductVersionID;
        this.lblProductVersion.Text = task.ProductVersion;
        this.SystemID = task.SystemID;
        this.UnclosedSRTasks = task.UnclosedSRTasks;
        txtDateNeeded.Text = task.NeedDate;
        chkBusinessReview.Checked = task.BusinessReview;

        if (!IsNew)
		{
			this.labelCreated.Text = task.CreatedBy + " - " + task.CreatedDate;
			this.labelUpdated.Text = task.UpdatedBy + " - " + task.UpdatedDate;
        }

        DataTable parentTaskData = WorkloadItem.WorkItem_Get(workItemID: WorkItemID);
        txtResourceGroup.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["WorkType"].ToString().Replace("&nbsp;", "").Trim()));
        txtResourceGroup.Attributes.Add("ResourceGroupID", HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["WorkTypeID"].ToString())));
        txtFunctionality.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["WorkloadGroup"].ToString().Replace("&nbsp;", "").Trim()));
        txtProductionStatus.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["ProductionStatus"].ToString().Replace("&nbsp;", "").Trim()));
        txtSystemTask.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["WTS_SYSTEM"].ToString().Replace("&nbsp;", "").Trim()));        
        txtContract.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["CONTRACT"].ToString().Replace("&nbsp;", "").Trim()));
        txtWorkloadAllocation.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(parentTaskData.Rows[0]["WorkloadAllocation"].ToString().Replace("&nbsp;", "").Trim()));

        int workAreaID = 0, systemID = 0;
        int.TryParse(parentTaskData.Rows[0]["WorkAreaID"].ToString(), out workAreaID);
        int.TryParse(parentTaskData.Rows[0]["WTS_SYSTEMID"].ToString(), out systemID);

        txtSystemTask.Attributes["WTS_SYSTEMID"] = systemID.ToString();
        txtWorkArea.Attributes["WorkAreaID"] = workAreaID.ToString();

        this.labelTotalDaysOpened.Text = task.TotalDaysOpened.ToString();
        this.labelTotalBusinessDaysOpened.Text = task.TotalBusinessDaysOpened.ToString();
        this.labelInProgressDate.Text = (String.IsNullOrEmpty(task.InProgressDate.ToString())) ? null : task.InProgressDate.Value.ToShortDateString();
        this.labelInProgressTime.Text = (String.IsNullOrEmpty(task.InProgressDate.ToString())) ? null : task.InProgressDate.Value.ToShortTimeString();
        this.labelDeployedDate.Text = (String.IsNullOrEmpty(task.DeployedDate.ToString())) ? null : task.DeployedDate.Value.ToShortDateString();
        this.labelDeployedTime.Text = (String.IsNullOrEmpty(task.DeployedDate.ToString())) ? null : task.DeployedDate.Value.ToShortTimeString();
        this.labelReadyForReviewDate.Text = (String.IsNullOrEmpty(task.ReadyForReviewDate.ToString())) ? null : task.ReadyForReviewDate.Value.ToShortDateString();
        this.labelReadyForReviewTime.Text = (String.IsNullOrEmpty(task.ReadyForReviewDate.ToString())) ? null : task.ReadyForReviewDate.Value.ToShortTimeString();
        this.labelClosedDate.Text = (String.IsNullOrEmpty(task.ClosedDate.ToString())) ? null : task.ClosedDate.Value.ToShortDateString();
        this.labelClosedTime.Text = (String.IsNullOrEmpty(task.ClosedDate.ToString())) ? null : task.ClosedDate.Value.ToShortTimeString();
        this.labelTotalDaysInProgress.Text = task.TotalDaysInProgress.ToString();
        this.labelTotalDaysDeployed.Text = task.TotalDaysDeployed.ToString();
        this.labelTotalDaysReadyForReview.Text = task.TotalDaysReadyForReview.ToString();
        this.labelTotalDaysClosed.Text = task.TotalDaysClosed.ToString();

        DataTable dtWorkArea = MasterData.WorkArea_SystemList_Get(workAreaID: workAreaID);
        string workAreaText = string.Empty;
        if (dtWorkArea != null && dtWorkArea.Rows.Count > 1)
        {
            workAreaText = dtWorkArea.Rows[dtWorkArea.Rows.Count - 1]["ApprovedPriority"].ToString() + " - " + dtWorkArea.Rows[dtWorkArea.Rows.Count - 1]["WorkArea"].ToString();
        }

        txtWorkArea.Text = HttpUtility.HtmlDecode(workAreaText.Replace("&nbsp;", "").Trim());

        DataTable dtAORs = AOR.AORTaskAORList_Get(TaskID: WorkItemID);
        if (dtAORs != null && dtAORs.Rows.Count > 0)
        {
            foreach(DataRow dr in dtAORs.Rows)
            {
                if (dr["AORWorkTypeID"].ToString() == "1")
                {
                    txtWorkloadAOR.Text = dr["AORID"].ToString() + " (" + dr["Abbreviation"].ToString() + ") - " + dr["AORName"].ToString();
                    txtWorkloadAOR.ToolTip = txtWorkloadAOR.Text;
                }
                if (dr["AORWorkTypeID"].ToString() == "2")
                {
                    txtReleaseAOR.Text = dr["AORID"].ToString() + " (" + dr["Abbreviation"].ToString() + ") - " + dr["AORName"].ToString();
                    txtReleaseAOR.ToolTip = txtReleaseAOR.Text;
                    ParentRelAORReleaseID = dr["AORReleaseID"].ToString();
                }
            }
        }
    }

    private void filterStatuses(bool newTask)
    {
        ListItem item;

        if (newTask)
        {
            item = ddlStatus.Items.FindByText("Re-Opened");
            if (item != null) item.Enabled = false;
            item = ddlStatus.Items.FindByText("Closed");
            if (item != null) item.Enabled = false;
        }
        else
        {
            if (ddlStatus.SelectedItem.Text != "New")
            {
                item = ddlStatus.Items.FindByText("New");
                if (item != null) item.Enabled = false;
            }

            if (!(ddlStatus.SelectedItem.Text == "Re-Opened" 
                || ddlStatus.SelectedItem.Text == "Closed"
                || ddlStatus.SelectedItem.Text == "Deployed"
                || ddlStatus.SelectedItem.Text == "Checked In"
                || ddlStatus.SelectedItem.Text == "Un-Reproducible"
                || ddlStatus.SelectedItem.Text == "Ready for Review"))
            {
                item = ddlStatus.Items.FindByText("Re-Opened");
                if (item != null) item.Enabled = false;
            }
        }
    }

    [WebMethod(true)]
	public static string SaveTask(int taskID, int workItemID, int priorityID
		, string title, string description, int assignedToID, int primaryResourceID, int secondaryResourceID
        , int primaryBusResourceID, int secondaryBusResourceID
        , string plannedStartDate, string actualStartDate, int estimatedEffortID, int actualEffortID, string actualEndDate
		, int completionPercent, int statusID, int workItemTypeID, int businessRank, int sortOrder, string pageOption, string SRNumber, int assignedToRankID, int productVersionID, string dateNeeded, bool businessReview, string aors)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { 
		{ "saved", "" }
		, { "id", "0" }
		, { "error", "" }
		, { "pageOption", pageOption } };
        bool saved = false, loaded = false, savedAOR = false;
		string errorMsg = string.Empty;

		try
		{
			HttpServerUtility server = HttpContext.Current.Server;
			description = server.UrlDecode(description);

			int submittedByID = 0;
			submittedByID = UserManagement.GetUserId_FromUsername();

			WorkItem_Task task = new WorkItem_Task(taskID);
			if (taskID > 0)
			{
				loaded = task.Load();
			}

			task.WorkItemID = workItemID;
			task.PriorityID = priorityID;
			task.Title = title;
			task.Description = description;
			task.AssignedResourceID = assignedToID;
			task.PrimaryResourceID = primaryResourceID;
            task.SecondaryResourceID = secondaryResourceID;
            task.PrimaryBusResourceID = primaryBusResourceID;
            task.SecondaryBusResourceID = secondaryBusResourceID;
            task.EstimatedStartDate = plannedStartDate;
			task.ActualStartDate = actualStartDate;
            task.EstimatedEffortID = estimatedEffortID;
			task.ActualEffortID = actualEffortID;
			task.ActualEndDate = actualEndDate;
			task.CompletionPercent = completionPercent;
			task.StatusID = statusID;
			task.WorkItemTypeID = workItemTypeID;
            task.BusinessRank = businessRank;
			task.Sort_Order = sortOrder;
            task.SRNumber = SRNumber;
            task.AssignedToRankID = assignedToRankID;
            task.ProductVersionID = productVersionID;
            task.NeedDate = dateNeeded;
            task.BusinessReview = businessReview;

            XmlDocument docAORs = (XmlDocument)JsonConvert.DeserializeXmlNode(aors, "aors");

            if (taskID == 0)
			{
				task.SubmittedByID = submittedByID;

				saved = task.Add(newID: out taskID, errorMsg: out errorMsg);
								
				if (saved)
				{
                    savedAOR = AOR.AORSubTask_Save(TaskID: taskID, AORs: docAORs, Add: 0, CascadeAOR: false);
                    Workload.SendWorkloadEmail("WorkItemTask", true, taskID);
				}
			}
			else
			{
				if (loaded)
				{
					saved = task.Save(errorMsg: out errorMsg);

                }

                if (saved)
				{
                    savedAOR = AOR.AORSubTask_Save(TaskID: taskID, AORs: docAORs, Add: 0, CascadeAOR: false);
                    Workload.SendWorkloadEmail("WorkItemTask", false, taskID);
				}
			}
		}
		catch (Exception ex)
		{
			saved = false;
			errorMsg = ex.Message;
			LogUtility.LogException(ex);
		}

		result["saved"] = saved.ToString();
		result["id"] = taskID.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
	}

    [WebMethod()]
    public static string GetAOROptions(string assignedTo, string primaryResource, string system, int systemAffiliated, int resourceAffiliated, string assignedToRank, int all)
    {
        DataTable dt = new DataTable();

        try
        {
            int assignedTo_ID = 0, primaryResource_ID = 0, system_ID = 0, assignedToRank_ID = 0;

            int.TryParse(assignedTo, out assignedTo_ID);
            int.TryParse(primaryResource, out primaryResource_ID);
            int.TryParse(system, out system_ID);
            int.TryParse(assignedToRank, out assignedToRank_ID);

            dt = AOR.AORTaskOptionsList_Get(AssignedToID: assignedTo_ID, PrimaryResourceID: primaryResource_ID, SystemID: system_ID, SystemAffiliated: systemAffiliated, ResourceAffiliated: resourceAffiliated, AssignedToRank: assignedToRank_ID, All: all);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetAORResourceTeamUser(string aorReleaseID)
    {
        DataTable dt = new DataTable();

        try
        {
            int aorRelease_ID = 0;

            int.TryParse(aorReleaseID, out aorRelease_ID);

            if (aorRelease_ID == 0) aorRelease_ID = -1;

            dt = AOR.AORResourceTeamList_Get(AORID: 0, AORReleaseID: aorRelease_ID);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetAffiliated(string workitem, string system, string productVersion, string resourceGroup, string workActivity, string aors)
    {
        DataTable dt = new DataTable();

        try
        {
            int workitem_ID = 0, system_ID = 0, productVersion_ID = 0, resourceGroup_ID = 0, workActivity_ID = 0;

            int.TryParse(workitem, out workitem_ID);
            int.TryParse(system, out system_ID);
            int.TryParse(productVersion, out productVersion_ID);
            int.TryParse(resourceGroup, out resourceGroup_ID);
            int.TryParse(workActivity, out workActivity_ID);

            dt = Affiliated.AffiliatedList_Get(WORKITEMID: workitem_ID, WTS_SYSTEMID: system_ID, ProductVersionID: productVersion_ID, ResourceGroupID: resourceGroup_ID, WorkActivityID: workActivity_ID, AORReleaseIDs: aors);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string LoadMetrics(int workItemTaskID)
    {
        var result = WTS.WTSPage.CreateDefaultResult();

        DataSet ds = Workload.WorkItem_Task_Metrics_Get(workItemTaskID);

        return WTS.WTSPage.SerializeResult(ds);
    }
}