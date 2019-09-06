﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;


public partial class WorkItem_Details : System.Web.UI.Page
{
    protected int UserID = 0;

    protected int WorkItemID = 0;
    protected bool IsNewWorkItem = false;
    protected int SaveComplete = 0;
    protected int SourceWorkItemID = 0;

    protected int Comment_Count = 0;
    protected int Attachment_Count = 0;
    protected int WR_Attachment_Count = 0;
    protected int Task_Count = 0;
    protected int Sub_Task_Count = 0;
    protected int Sub_Task_New_Count = 0;
    protected int Sub_Task_OnHold_Count = 0;
    protected int Sub_Task_Closed_Count = 0;
    protected int Sub_Task_CheckedIn_Count = 0;
    protected int Sub_Task_Deployed_Count = 0;
    protected int Sub_Task_Complete_Count = 0;

    DataTable _dtWorkType = null;
    DataTable _dtWorkArea = null;
    DataTable _dtAllocationAssignment = null;
    DataTable _dtAllocationGroups = null;
    DataTable _dtAllocations = null;
    protected int UnclosedSRTasks = 0;
    protected int CurrentStatusID = 0;
    protected string CurrentStatus = string.Empty;

    protected bool CanEdit = false;
    protected bool CanEditAOR = false;
    protected bool CanEditWorkloadMGMT = false;
    protected int RequestedID = 0;

    protected string CopySubTasks = "false";
    protected string SelectedAssigned;
    protected string SelectedStatuses;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanEditWorkloadMGMT = UserManagement.UserCanEdit(WTSModuleOption.WorkloadMGMT);
        this.UserID = UserManagement.GetUserId_FromUsername();

        readQueryString();
        LoadRelatedItemsMenu();
        loadLookupData();

        if (this.WorkItemID > 0 || (this.IsNewWorkItem && this.SourceWorkItemID > 0))
        {
            loadWorkItem();

            DataTable dt = WorkloadItem.WorkItem_GetTaskList(workItemID: this.WorkItemID);

            if (dt != null)//WorkTask exists so fill it in
            {
                try
                {
                    Sub_Task_Count = dt.Rows.Count;
                    Sub_Task_New_Count = (from row in dt.AsEnumerable()
                                          where row.Field<string>("STATUS").Trim().ToUpper() == "NEW"
                                          select row).Count();
                    Sub_Task_OnHold_Count = (from row in dt.AsEnumerable()
                                             where row.Field<string>("STATUS").Trim().ToUpper() == "ON HOLD"
                                             select row).Count();
                    Sub_Task_Closed_Count = (from row in dt.AsEnumerable()
                                             where row.Field<string>("STATUS").Trim().ToUpper() == "CLOSED"
                                             select row).Count();
                    Sub_Task_CheckedIn_Count = (from row in dt.AsEnumerable()
                                                where row.Field<string>("STATUS").Trim().ToUpper() == "CHECKED IN"
                                                select row).Count();
                    Sub_Task_Deployed_Count = (from row in dt.AsEnumerable()
                                               where row.Field<string>("STATUS").Trim().ToUpper() == "DEPLOYED"
                                               select row).Count();
                    Sub_Task_Complete_Count = (from row in dt.AsEnumerable()
                                               where row.Field<string>("STATUS").Trim().ToUpper() == "COMPLETE"
                                               select row).Count();
                    spnWorkloadPriority.Text = calculateSubtaskWorkloadPriority(dt);
                }
                catch (Exception) { }
            }
        }
        else //WorkTask does not exist
        {
            //set defaults
            ListItem item = null;

            item = ddlAssignedToRank.Items.FindByText("5 - Unprioritized Workload");
            if (item != null) item.Selected = true;
            txtPrimaryBusRank.Text = "99";

            DataTable dt = AOR.AORCurrentRelease_Get();
            ddlProductVersion.SelectedValue = dt.Rows[0]["ProductVersionID"].ToString();
            lblProductVersion.Text = dt.Rows[0]["ProductVersion"].ToString();
            chkCascadeAOR.Checked = true;
        }

        int orgID = (int)UserManagement.Organization.Folsom_Dev;
        WTS_User u = new WTS_User(UserID);
        u.Load();
        orgID = u.OrganizationID;

        List<string> allow = new List<string> { };
        try
        {
            XDocument xmlDoc = XDocument.Load(this.Server.MapPath("XML/WTS_SignOff.xml"));
            allow = xmlDoc.Root.Elements("name")
                .Select(element => element.Value.Trim().ToUpper())
                .ToList();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        chkSigned_Bus.Enabled = ((orgID == (int)UserManagement.Organization.Business_Team || allow.Contains(u.Username.Trim().ToUpper())) && !chkSigned_Bus.Checked);
        chkSigned_Dev.Enabled = ((orgID == (int)UserManagement.Organization.Folsom_Dev || allow.Contains(u.Username.Trim().ToUpper())) && !chkSigned_Dev.Checked);

        LoadAORs();
    }
    private void readQueryString()
    {
        if (Request.QueryString["workItemID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["workItemID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["workItemID"].ToString()), out this.WorkItemID);
        }
        else
        {
            this.IsNewWorkItem = true;
        }

        if (this.WorkItemID == 0)
        {
            this.IsNewWorkItem = true;
        }

        if (Request.QueryString["sourceWorkItemID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["sourceWorkItemID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["sourceWorkItemID"].ToString()), out this.SourceWorkItemID);
        }

        if (Request.QueryString["Saved"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["Saved"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["Saved"].ToString()), out this.SaveComplete);
        }
        if (Request.QueryString["SelectedAssigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAssigned"].ToString()))
        {
            SelectedAssigned = Server.UrlDecode(Request.QueryString["SelectedAssigned"].Trim());
        }
        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"].ToString()))
        {
            SelectedStatuses = Server.UrlDecode(Request.QueryString["SelectedStatuses"].Trim());
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
        DataSet dsOptions = WorkloadItem.GetAvailableOptions();

        if (dsOptions == null || dsOptions.Tables.Count == 0)
        {
            return;
        }

        ListItem item = null;

        if (dsOptions.Tables.Contains("WorkRequest"))
        {
            ddlWorkRequest.DataSource = dsOptions.Tables["WorkRequest"];
            ddlWorkRequest.DataTextField = "REQUEST";
            ddlWorkRequest.DataValueField = "WORKREQUESTID";
            ddlWorkRequest.DataBind();
        }
        if (dsOptions.Tables.Contains("PDTDRPhase"))
        {
            ddlPDDTDRPhase.DataSource = dsOptions.Tables["PDTDRPhase"];
            ddlPDDTDRPhase.DataTextField = "PDDTDR_PHASE";
            ddlPDDTDRPhase.DataValueField = "PDDTDR_PHASEID";
            ddlPDDTDRPhase.DataBind();
        }
        if (dsOptions.Tables.Contains("WorkType"))
        {
            DataView dv = dsOptions.Tables["WorkType"].DefaultView;
            dv.Sort = "WorkType ASC";
            _dtWorkType = dv.ToTable();
            //Resource Group options are based on Request Phase, done on client side
            Page.ClientScript.RegisterArrayDeclaration("arrWorkType", JsonConvert.SerializeObject(_dtWorkType, Newtonsoft.Json.Formatting.None));

            // SCB TODO: Bug: If you go into work grid and immediately click Add Task, this dropdown is empty. If you bring up a task first, 
            // then it's populated - though with duplicates.
            foreach (DataRow dr in _dtWorkType.Rows)
            {
                item = new ListItem(dr["WorkType"].ToString(), dr["WorkTypeID"].ToString());
                if (!ddlWorkType.Items.Contains(item)) ddlWorkType.Items.Add(item);
            }
        }

        //if (dsOptions.Tables.Contains("Status"))
        //{
        //    // Production Statuses
        //    ddlStatus.DataSource = dsOptions.Tables["Status"];
        //    ddlStatus.DataTextField = "Status";
        //    ddlStatus.DataValueField = "StatusID";
        //    ddlStatus.DataBind();
        //}



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
                if (/*dr["PDDTDR_PHASEID"].ToString() == "" || */dr["WorkloadAllocationID"].ToString() == "" || dr["WorkloadAllocationID"].ToString() == workloadAllocationID.ToString()) {
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

            //Work Activity dropdown options are set based on selected Release/Deployment MGMT AOR Workload Allocation, done on client side
            Page.ClientScript.RegisterArrayDeclaration("arrWorkActivity", JsonConvert.SerializeObject(dtWorkItemType, Newtonsoft.Json.Formatting.None));
        }

        if (dsOptions.Tables.Contains("Priority"))
        {
            ddlPriority.DataSource = dsOptions.Tables["Priority"];
            ddlPriority.DataTextField = "PRIORITY";
            ddlPriority.DataValueField = "PRIORITYID";
            ddlPriority.DataBind();
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

                item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
                item.Attributes.Add("username", row["USERNAME"].ToString());
                item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());
                ddlDeployedBy_Comm.Items.Add(item);

                item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
                item.Attributes.Add("username", row["USERNAME"].ToString());
                item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());
                ddlDeployedBy_Test.Items.Add(item);

                item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
                item.Attributes.Add("username", row["USERNAME"].ToString());
                item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());
                ddlDeployedBy_Prod.Items.Add(item);

                item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
                item.Attributes.Add("username", row["USERNAME"].ToString());
                item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());
                ddlTester.Items.Add(item);
            }
        }
        if (dsOptions.Tables.Contains("Status"))
        {
            var RequestedIDstr = dsOptions.Tables["Status"].Rows[0]["RequestedID"].ToString();
            Int32.TryParse(RequestedIDstr, out RequestedID);
            //Status dropdown options are set based on selected Resource Group, done on client side
            Page.ClientScript.RegisterArrayDeclaration("arrStatus", JsonConvert.SerializeObject(dsOptions.Tables["Status"], Newtonsoft.Json.Formatting.None));
        }
        if (dsOptions.Tables.Contains("Allocation"))
        {
            DataView dv = dsOptions.Tables["Allocation"].DefaultView;
            dv.Sort = "Allocation ASC";
            _dtAllocationAssignment = dv.ToTable();
            //Allocation Assignment options are based on System, done on client side
            Page.ClientScript.RegisterArrayDeclaration("arrAllocation", JsonConvert.SerializeObject(_dtAllocationAssignment, Newtonsoft.Json.Formatting.None));
        }
        if (dsOptions.Tables.Contains("PercentComplete"))
        {
            ddlPercentComplete.DataSource = dsOptions.Tables["PercentComplete"];
            ddlPercentComplete.DataTextField = "Percent";
            ddlPercentComplete.DataValueField = "Percent";
            ddlPercentComplete.DataBind();
        }
        if (dsOptions.Tables.Contains("System"))
        {
            DataTable dtSystem = dsOptions.Tables["System"];
            string currentSuiteID = null;
            ListItem li;

            dtSystem.DefaultView.Sort = "WTS_SYSTEM_SUITE, WTS_SYSTEM";
            dtSystem = dtSystem.DefaultView.ToTable();

            foreach (DataRow row in dtSystem.Rows)
            {
                if (row["WTS_SYSTEM_SUITEID"].ToString() != currentSuiteID)
                {
                    li = new ListItem();
                    li.Text = row["WTS_SYSTEM_SUITE"].ToString();
                    li.Attributes["disabled"] = "disabled";
                    li.Attributes["style"] = "background-color: white";
                    ddlSystem.Items.Add(li);
                }

                li = new ListItem();
                li.Value = row["WTS_SYSTEMID"].ToString();
                li.Text = row["WTS_SYSTEM"].ToString();
                li.Attributes.Add("Contract", row["Contract"].ToString());
                ddlSystem.Items.Add(li);

                currentSuiteID = row["WTS_SYSTEM_SUITEID"].ToString();
            }

            Page.ClientScript.RegisterArrayDeclaration("arrSystem", JsonConvert.SerializeObject(dsOptions.Tables["System"], Newtonsoft.Json.Formatting.None));
        }
        if (dsOptions.Tables.Contains("ProductVersion"))
        {
            ddlProductVersion.DataSource = dsOptions.Tables["ProductVersion"];
            ddlProductVersion.DataTextField = "ProductVersion";
            ddlProductVersion.DataValueField = "ProductVersionID";
            ddlProductVersion.DataBind();
        }
        if (dsOptions.Tables.Contains("MenuType"))
        {
            ddlMenuType.DataSource = dsOptions.Tables["MenuType"];
            ddlMenuType.DataTextField = "MenuType";
            ddlMenuType.DataValueField = "MenuTypeID";
            ddlMenuType.DataBind();
        }
        if (dsOptions.Tables.Contains("Menu"))
        {
            ddlMenuName.DataSource = dsOptions.Tables["Menu"];
            ddlMenuName.DataTextField = "Menu";
            ddlMenuName.DataValueField = "MenuID";
            ddlMenuName.DataBind();
        }
        if (dsOptions.Tables.Contains("WorkloadGroup"))
        {
            ddlWorkloadGroup.DataSource = dsOptions.Tables["WorkloadGroup"];
            ddlWorkloadGroup.DataTextField = "WorkloadGroup";
            ddlWorkloadGroup.DataValueField = "WorkloadGroupID";
            ddlWorkloadGroup.DataBind();
        }
        if (dsOptions.Tables.Contains("WorkArea"))
        {
            _dtWorkArea = dsOptions.Tables["WorkArea"];
            //Work Area options are based on System, done on client side
            Page.ClientScript.RegisterArrayDeclaration("arrWorkArea", JsonConvert.SerializeObject(dsOptions.Tables["WorkArea"], Newtonsoft.Json.Formatting.None));
        }
        if (dsOptions.Tables.Contains("TshirtSizes"))
        {
            ddlHours_Planned.DataSource = dsOptions.Tables["TshirtSizes"];
            ddlHours_Planned.DataTextField = "EffortSize";
            ddlHours_Planned.DataValueField = "EffortSizeID";
            ddlHours_Planned.DataBind();
        }
        if (dsOptions.Tables.Contains("ProductionStatus"))
        {
            ddlProductionStatus.DataSource = dsOptions.Tables["ProductionStatus"];
            ddlProductionStatus.DataTextField = "STATUS";
            ddlProductionStatus.DataValueField = "STATUSID";
            ddlProductionStatus.DataBind();
        }

        if (dsOptions.Tables.Contains("AllocationGroup"))
        {
            ddlAllocationGroup.DataSource = dsOptions.Tables["AllocationGroup"];
            ddlAllocationGroup.DataTextField = "ALLOCATIONGROUP";
            ddlAllocationGroup.DataValueField = "ALLOCATIONGROUPID";
            ddlAllocationGroup.DataBind();

            DataView dv = dsOptions.Tables["AllocationGroup"].DefaultView;
            dv.Sort = "AllocationGroup ASC";
            _dtAllocationGroups = dv.ToTable();
            //Allocation Group options are based on System, done on client side
            Page.ClientScript.RegisterArrayDeclaration("arrAllocationGroup", JsonConvert.SerializeObject(_dtAllocationGroups, Newtonsoft.Json.Formatting.None));
        }


        if (dsOptions.Tables.Contains("Allocations"))
        {
            DataView dv = dsOptions.Tables["Allocation"].DefaultView;
            dv.Sort = "Allocation ASC";
            _dtAllocations = dv.ToTable();
            //Allocations are based on Allocation Group, done on client side
            Page.ClientScript.RegisterArrayDeclaration("arrAllocation", JsonConvert.SerializeObject(_dtAllocations, Newtonsoft.Json.Formatting.None));
        }

        if (dsOptions.Tables.Contains("Rank"))
        {
            ddlAssignedToRank.DataSource = dsOptions.Tables["Rank"];
            ddlAssignedToRank.DataTextField = "PRIORITY";
            ddlAssignedToRank.DataValueField = "PRIORITYID";
            ddlAssignedToRank.DataBind();
        }
    }

    private void loadWorkItem()
    {
        int workItemID = (this.IsNewWorkItem && this.SourceWorkItemID > 0) ? this.SourceWorkItemID : this.WorkItemID;

        DateTime date;
        DataTable dt = WorkloadItem.WorkItem_Get(workItemID: workItemID);
        bool isChecked = false;

        if (dt != null && dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            ListItem item = null;
            if (!IsNewWorkItem)
            {
                this.txtWorkloadNumber.Text = row["WORKITEMID"].ToString();

                int.TryParse(dt.Rows[0]["Comment_Count"].ToString(), out this.Comment_Count);
                int.TryParse(dt.Rows[0]["Attachment_Count"].ToString(), out this.Attachment_Count);
                int.TryParse(dt.Rows[0]["WorkRequest_Attachment_Count"].ToString(), out this.WR_Attachment_Count);
            }

            WTSUtility.SelectDdlItem(ddlWorkRequest, row["WORKREQUESTID"].ToString(), row["REQUEST"].ToString());
            txtTitle.Text = row["TITLE"].ToString();
            //select request phase
            item = ddlPDDTDRPhase.Items.FindByValue(row["PhaseID"].ToString());
            if (item != null)
            {
                item.Selected = true;
            }
            foreach (DataRow wtRow in _dtWorkType.Rows)
            {
                item = null;
                //if (wtRow["PhaseID"].ToString() == row["PhaseID"].ToString())
                //{
                item = new ListItem(wtRow["WorkType"].ToString(), wtRow["WorkTypeID"].ToString());
                //    item.Attributes.Add("PhaseID", wtRow["PhaseID"].ToString());
                if (!ddlWorkType.Items.Contains(item)) ddlWorkType.Items.Add(item);
                //}
                item = ddlWorkType.Items.FindByValue(row["WorkTypeID"].ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
            }
            int.TryParse(row["STATUSID"].ToString(), out this.CurrentStatusID);

            //Sign off
            chkSigned_Bus.Checked = bool.TryParse(row["Signed_Bus"].ToString(), out isChecked) ? isChecked : false;
            chkSigned_Dev.Checked = bool.TryParse(row["Signed_Dev"].ToString(), out isChecked) ? isChecked : false;
            CurrentStatus = row["STATUS"].ToString();

            //if (CurrentStatus.Trim().ToUpper() == "REQUESTED" && (!chkSigned_Bus.Checked || !chkSigned_Dev.Checked))
            //{
            //    ddlStatus.Enabled = false;
            //    imgStatusHelp.Style["display"] = "inline";
            //}

            if (chkSigned_Bus.Checked)
            {
                lblSigned_Bus.InnerText = "Bus (" + row["Signed_Bus_User"].ToString() + " - " + row["SignedDate_Bus"].ToString() + ")";
            }
            if (chkSigned_Dev.Checked)
            {
                lblSigned_Dev.InnerText = "Dev (" + row["Signed_Dev_User"].ToString() + " - " + row["SignedDate_Dev"].ToString() + ")";
            }

            chkIVTRequired.Checked = bool.TryParse(row["IVTRequired"].ToString(), out isChecked) ? isChecked : false;
            spanNumDependencies.InnerText = row["Dependency_Count"].ToString();
            WTSUtility.SelectDdlItem(ddlSystem, row["WTS_SYSTEMID"].ToString(), row["WTS_SYSTEM"].ToString());
            spnContract.Text = ddlSystem.SelectedItem.Attributes["Contract"].ToString();
            this.lblProductVersion.Text = row["ProductVersion"].ToString();
            WTSUtility.SelectDdlItem(ddlProductVersion, row["ProductVersionID"].ToString(), row["ProductVersion"].ToString());
            chkProduction.Checked = bool.TryParse(row["Production"].ToString(), out isChecked) ? isChecked : false;
            chkRecurring.Checked = bool.TryParse(row["Recurring"].ToString(), out isChecked) ? isChecked : false;
            txtSRNumber.Text = row["SR_Number"].ToString() == "0" ? "" : row["SR_Number"].ToString();
            int.TryParse(row["Unclosed SR Tasks"].ToString(), out UnclosedSRTasks);
            chkReproduced_Bus.Checked = bool.TryParse(row["Reproduced_Biz"].ToString(), out isChecked) ? isChecked : false;
            chkReproduced_Dev.Checked = bool.TryParse(row["Reproduced_Dev"].ToString(), out isChecked) ? isChecked : false;
            chkBusinessReview.Checked = bool.TryParse(row["BusinessReview"].ToString(), out isChecked) ? isChecked : false;

            WTSUtility.SelectDdlItem(ddlWorkItemType, row["WORKITEMTYPEID"].ToString(), row["WORKITEMTYPE"].ToString());

            foreach (DataRow aRow in _dtAllocationAssignment.Rows)
            {
                item = null;
                if (string.IsNullOrWhiteSpace(aRow["WTS_SYSTEMID"].ToString())
                    || aRow["WTS_SYSTEMID"].ToString() == row["WTS_SYSTEMID"].ToString())
                {
                    item = new ListItem(aRow["ALLOCATION"].ToString(), aRow["ALLOCATIONID"].ToString());
                    item.Attributes.Add("WTS_SYSTEMID", aRow["WTS_SYSTEMID"].ToString());
                    item.Attributes.Add("ALLOCATIONGROUPID", aRow["ALLOCATIONGROUPID"].ToString());
                    ddlAllocation.Items.Add(item);
                }
            }

            WTSUtility.SelectDdlItem(ddlAllocation, row["ALLOCATIONID"].ToString(), row["ALLOCATION"].ToString());
            WTSUtility.SelectDdlItem(ddlMenuType, row["MenuTypeID"].ToString(), row["MenuType"].ToString());
            WTSUtility.SelectDdlItem(ddlMenuName, row["MenuNameID"].ToString(), row["Menu"].ToString());
            WTSUtility.SelectDdlItem(ddlPriority, row["PRIORITYID"].ToString(), row["PRIORITY"].ToString());

            if (DateTime.TryParse(row["NEEDDATE"].ToString(), out date))
            {
                txtDateNeeded.Text = date.ToShortDateString();
            }
            WTSUtility.SelectDdlItem(ddlHours_Planned, row["EstimatedEffortID"].ToString(), row["EstimatedEffort"].ToString());

            ListItem li = null;
            li = ddlAssignedTo.Items.FindByValue(row["ASSIGNEDRESOURCEID"].ToString());
            if (li == null)
            {
                li = new ListItem(row["AssignedResource"].ToString(), row["ASSIGNEDRESOURCEID"].ToString());
                int uID = 0;
                int.TryParse(row["ASSIGNEDRESOURCEID"].ToString(), out uID);
                WTS_User u = new WTS_User(uID);
                u.Load();

                if (u.AORResourceTeam)
                {
                    li.Attributes.Add("og", "Action Team");

                    DataTable dtAORResourceTeam = AOR.AORResourceTeamList_Get(AORID: 0, AORReleaseID: 0);
                    dtAORResourceTeam.DefaultView.RowFilter = "ResourceTeamUserID = " + uID;
                    dtAORResourceTeam = dtAORResourceTeam.DefaultView.ToTable();
                    if (dtAORResourceTeam.Rows.Count > 0) li.Attributes.Add("aorid", dtAORResourceTeam.Rows[0]["AORID"].ToString());

                    ddlAssignedTo.Items.Insert(1, li);
                }
                else
                {
                    ddlAssignedTo.Items.Insert(0, li);
                }
            }
            li.Selected = true;

            WTSUtility.SelectDdlItem(ddlPrimaryResource, row["PRIMARYRESOURCEID"].ToString(), row["PrimaryResource"].ToString());
            WTSUtility.SelectDdlItem(ddlPrimaryBusResource, row["PrimaryBusinessResourceID"].ToString(), row["PrimaryBusinessResourceID"].ToString());
            WTSUtility.SelectDdlItem(ddlSecondaryResource, row["SECONDARYRESOURCEID"].ToString(), row["SECONDARYRESOURCEID"].ToString());
            WTSUtility.SelectDdlItem(ddlSecondaryBusResource, row["SecondaryBusinessResourceID"].ToString(), row["SecondaryBusinessResourceID"].ToString());

            WTSUtility.SelectDdlItem(ddlAssignedToRank, row["AssignedToRankID"].ToString(), row["AssignedToRank"].ToString());
            txtResourcePriorityRank.Text = row["RESOURCEPRIORITYRANK"].ToString();
            txtSecondaryTechRank.Text = row["SecondaryResourceRank"].ToString();
            txtPrimaryBusRank.Text = row["PrimaryBusinessRank"].ToString();
            txtSecondaryBusRank.Text = row["SecondaryBusinessRank"].ToString();

            WTSUtility.SelectDdlItem(ddlAllocationGroup, row["AllocationGroupID"].ToString(), row["AllocationGroup"].ToString());

            txtBugTrackerID.Text = row["BUGTRACKER_ID"].ToString();

            WTSUtility.SelectDdlItem(ddlPercentComplete, row["COMPLETIONPERCENT"].ToString(), row["COMPLETIONPERCENT"].ToString());
            WTSUtility.SelectDdlItem(ddlWorkloadGroup, row["WorkloadGroupID"].ToString(), row["WorkloadGroup"].ToString());

            foreach (DataRow waRow in _dtWorkArea.Rows)
            {
                item = null;
                if (string.IsNullOrWhiteSpace(waRow["WTS_SYSTEMID"].ToString())
                    || waRow["WTS_SYSTEMID"].ToString() == row["WTS_SYSTEMID"].ToString())
                {
                    item = new ListItem(waRow["WorkArea"].ToString(), waRow["WorkAreaID"].ToString());
                    item.Attributes.Add("WTS_SYSTEMID", waRow["WTS_SYSTEMID"].ToString());
                    ddlWorkArea.Items.Add(item);
                }
            }
            WTSUtility.SelectDdlItem(ddlWorkArea, row["WorkAreaID"].ToString(), row["WorkArea"].ToString());

            chkDeployed_Comm.Checked = bool.TryParse(row["Deployed_Comm"].ToString(), out isChecked) ? isChecked : false;
            WTSUtility.SelectDdlItem(ddlDeployedBy_Comm, row["DeployedBy_CommID"].ToString(), row["DeployedBy_COMM"].ToString());
            if (DateTime.TryParse(row["DeployedDate_Comm"].ToString(), out date))
            {
                txtDeployedDate_Comm.Text = date.ToShortDateString();
            }
            chkDeployed_Test.Checked = bool.TryParse(row["Deployed_Test"].ToString(), out isChecked) ? isChecked : false;
            WTSUtility.SelectDdlItem(ddlDeployedBy_Test, row["DeployedBy_TestID"].ToString(), row["DeployedBy_TEST"].ToString());
            if (DateTime.TryParse(row["DeployedDate_Test"].ToString(), out date))
            {
                txtDeployedDate_Test.Text = date.ToShortDateString();
            }
            chkDeployed_Prod.Checked = bool.TryParse(row["Deployed_Prod"].ToString(), out isChecked) ? isChecked : false;
            WTSUtility.SelectDdlItem(ddlDeployedBy_Prod, row["DeployedBy_ProdID"].ToString(), row["DeployedBy_PROD"].ToString());
            if (DateTime.TryParse(row["DeployedDate_Prod"].ToString(), out date))
            {
                txtDeployedDate_Prod.Text = date.ToShortDateString();
            }

            this.labelTotalDaysOpened.Text = row["TotalDaysOpened"].ToString();
            this.labelInProgressDate.Text = String.IsNullOrEmpty(row["InProgressDate"].ToString()) ? "" : DateTime.Parse(row["InProgressDate"].ToString()).ToShortDateString();
            this.labelInProgressTime.Text = String.IsNullOrEmpty(row["InProgressDate"].ToString()) ? "" : DateTime.Parse(row["InProgressDate"].ToString()).ToShortTimeString();
            this.labelReadyForReviewDate.Text = String.IsNullOrEmpty(row["ReadyForReviewDate"].ToString()) ? "" : DateTime.Parse(row["ReadyForReviewDate"].ToString()).ToShortDateString();
            this.labelReadyForReviewTime.Text = String.IsNullOrEmpty(row["ReadyForReviewDate"].ToString()) ? "" : DateTime.Parse(row["ReadyForReviewDate"].ToString()).ToShortTimeString();
            this.labelClosedDate.Text = String.IsNullOrEmpty(row["ClosedDate"].ToString()) ? "" : DateTime.Parse(row["ClosedDate"].ToString()).ToShortDateString();
            this.labelClosedTime.Text = String.IsNullOrEmpty(row["ClosedDate"].ToString()) ? "" : DateTime.Parse(row["ClosedDate"].ToString()).ToShortTimeString();

            this.labelTotalBusinessDaysOpened.Text = row["TotalBusinessDaysOpened"].ToString();
            this.labelTotalDaysInProgress.Text = row["TotalDaysInProgress"].ToString();
            this.labelTotalDaysReadyForReview.Text = row["TotalDaysReadyForReview"].ToString();
            this.labelTotalDaysClosed.Text = row["TotalDaysClosed"].ToString();

            txtCVTStep.Text = row["CVTStep"].ToString();
            item = ddlCVTStatus.Items.FindByText(row["CVTStatus"].ToString());
            if (item != null)
            {
                item.Selected = true;
            }
            WTSUtility.SelectDdlItem(ddlTester, row["TesterID"].ToString(), row["Tester"].ToString());

            textAreaDescription.InnerHtml = row["DESCRIPTION"].ToString();

            WTSUtility.SelectDdlItem(ddlProductionStatus, row["ProductionStatusID"].ToString(), row["ProductionStatus"].ToString());

            if (!IsNewWorkItem)
            {
                this.labelCreated.Text = row["CREATEDBY"].ToString() + " - " + row["CREATEDDATE"].ToString();
                this.labelUpdated.Text = row["UPDATEDBY"].ToString() + " - " + row["UPDATEDDATE"].ToString();
            }

            // 14149 - 2 Set Estimated Completion to creation date + 1 IF there isn't already a Estimated Completion date.
            if (txtEstimatedCompletion.Text == "")
            {
                if (DateTime.TryParse(row["CREATEDDATE"].ToString(), out date))
                {
                    txtEstimatedCompletion.Text = date.AddDays(1).ToShortDateString();
                }
            }

            if (DateTime.TryParse(row["PlannedDesignStart"].ToString(), out date))
            {
                txtDesignStart_Planned.Text = date.ToShortDateString();
            }
            if (DateTime.TryParse(row["PlannedDevStart"].ToString(), out date))
            {
                txtDevStart_Planned.Text = date.ToShortDateString();
            }
            if (DateTime.TryParse(row["ActualDesignStart"].ToString(), out date))
            {
                txtDesignStart_Actual.Text = date.ToShortDateString();
            }
            if (DateTime.TryParse(row["ActualDevStart"].ToString(), out date))
            {
                txtDevStart_Actual.Text = date.ToShortDateString();
            }
            if (DateTime.TryParse(row["EstimatedCompletionDate"].ToString(), out date))
            {
                txtEstimatedCompletion.Text = date.ToShortDateString();
            }
            if (DateTime.TryParse(row["ActualCompletionDate"].ToString(), out date))
            {
                txtActualCompletion.Text = date.ToShortDateString();
            }

            dynamic filters = "{ 'System(Task)':{ 'value':'" + row["WTS_SYSTEMID"].ToString() + "','text':'" + row["WTS_SYSTEM"].ToString() + "'},'Work Area':{ 'value':'" + row["WorkAreaID"].ToString() + "','text':'" + row["WorkArea"].ToString() + "'}}";

            dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>(filters);

            bool saved = Filtering.SaveWorkFilters(module: "RQMT", filterModule: "RQMT", filters: fields, myData: false, xml: "");
        }
    }

    private void LoadAORs()
    {
        int assignedTo_ID = 0, primaryResource_ID = 0, system_ID = 0;
        string prodStatusMsg = "Release/Deployment MGMT AOR not applicable for this task type";

        int.TryParse(ddlAssignedTo.SelectedValue, out assignedTo_ID);
        int.TryParse(ddlPrimaryResource.SelectedValue, out primaryResource_ID);
        int.TryParse(ddlSystem.SelectedValue, out system_ID);

        DataTable dtAORLookup = AOR.AORTaskOptionsList_Get(AssignedToID: assignedTo_ID, PrimaryResourceID: primaryResource_ID, SystemID: system_ID, SystemAffiliated: 1, ResourceAffiliated: 0, All: 1);
        DataTable dtAORs = new DataTable();
        StringBuilder sbAORs = new StringBuilder();

        if (this.WorkItemID > 0 || (this.IsNewWorkItem && this.SourceWorkItemID > 0))
        {
            int taskID = (this.IsNewWorkItem && this.SourceWorkItemID > 0) ? this.SourceWorkItemID : this.WorkItemID;

            dtAORs = AOR.AORTaskAORList_Get(TaskID: taskID);
        }

        DataRow[] aorTypes = new DataRow[2];

        foreach (DataRow dr in dtAORLookup.Rows)
        {
            foreach (DataRow row in dtAORs.Rows)
            {
                if (dr["AORReleaseID"].ToString() == row["AORReleaseID"].ToString())
                {
                    if (dr["AORType"].ToString() == "Release/Deployment MGMT")
                    {
                        //if (ddlProductionStatus.SelectedValue != "78" && ddlProductionStatus.SelectedValue != "108" && ddlProductionStatus.SelectedValue != "117")
                        //{
                        aorTypes[0] = dr;
                        //}
                    }
                    else
                    {
                        aorTypes[1] = dr;
                        if (row["CascadeAOR"].ToString() == "True") chkCascadeAOR.Checked = true;
                    }
                }
            }
        }


        for (int i = 0; i < aorTypes.Length; i++)
        {
            sbAORs.Append("<table style=\"border-collapse: collapse;\">");
            sbAORs.Append("<tr class=\"gridHeader\">");
            if (i == 0)
            {
                sbAORs.Append("<th style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 350px;\">");
            }
            else
            {
                sbAORs.Append("<th id=\"aor_header\" style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 350px;\">");
            }

            if (aorTypes[i] != null)//Task has this type of AOR
            {
                sbAORs.Append(aorTypes[i]["AORType"].ToString() + " AOR");
                sbAORs.Append("</th>");
                sbAORs.Append("</tr>");
                sbAORs.Append("<tr class=\"gridBody\">");

                if (this.CanEdit)
                {
                    sbAORs.Append("<td id=\"MgmtReleaseAORs\" style=\"border-left: 1px solid grey; padding-left: 6px;\">");
                    sbAORs.Append("<select field=\"" + aorTypes[i]["AORType"].ToString() + "\" original_value=\"" + aorTypes[i]["AORReleaseID"].ToString() + "\" onchange=\"AOR_change(this);\" style=\"width: 325px; background-color: #F5F6CE;\"");

                    //if (!this.CanEditWorkloadMGMT && aorTypes[i]["AORType"].ToString() == "Workload MGMT")
                    //    sbAORs.Append(" disabled");
					//If this task has the current aorType associated to it then only allow users with a WorkloadMGMT Role to edit this ddl
                    if (!this.CanEditWorkloadMGMT)
                        sbAORs.Append(" disabled");

                    sbAORs.Append(">"); //select
                    sbAORs.Append("<option value=\"0\"></option>");

                    bool contains = dtAORLookup.AsEnumerable().Any(row => aorTypes[i]["AORReleaseID"].ToString() == row.Field<Int32>("AORReleaseID").ToString());

                    if (!contains) sbAORs.Append("<option value=\"" + aorTypes[i]["AORReleaseID"].ToString() + "\" workloadAllocationID=\"" + aorTypes[i]["WorkloadAllocationID"].ToString() + "\" workloadAllocation=\"" + aorTypes[i]["WorkloadAllocation"].ToString() + "\" selected>" + aorTypes[i]["AORID"].ToString() + " - " + aorTypes[i]["AORName"].ToString() + "</option>");

                    foreach (DataRow dr in dtAORLookup.Rows)
                    {
                        if (dr["AORType"].ToString() == "Release/Deployment MGMT" && aorTypes[i]["AORType"].ToString() == "Release/Deployment MGMT")
                        {
                            sbAORs.Append("<option value=\"" + dr["AORReleaseID"].ToString() + "\" workloadAllocationID=\"" + dr["WorkloadAllocationID"].ToString() + "\" workloadAllocation=\"" + dr["WorkloadAllocation"].ToString() + "\"");
                            if (dr["AORReleaseID"].ToString() == aorTypes[i]["AORReleaseID"].ToString())
                                sbAORs.Append(" selected");

                            sbAORs.Append(">" + dr["AORID"].ToString() + " (" + dr["WorkloadAllocationAbbreviation"].ToString() + ") - " + dr["AORName"].ToString() + "</option>");
                        }
                        else if (dr["AORType"].ToString() != "Release/Deployment MGMT" && aorTypes[i]["AORType"].ToString() != "Release/Deployment MGMT")
                        {
                            sbAORs.Append("<option value=\"" + dr["AORReleaseID"].ToString() + "\" workloadAllocationID=\"" + dr["WorkloadAllocationID"].ToString() + "\" workloadAllocation=\"" + dr["WorkloadAllocation"].ToString() + "\"");
                            if (dr["AORReleaseID"].ToString() == aorTypes[i]["AORReleaseID"].ToString())
                                sbAORs.Append(" selected");

                            sbAORs.Append(">" + dr["AORID"].ToString() + " (" + dr["WorkloadAllocationAbbreviation"].ToString() + ") - " + dr["AORName"].ToString() + "</option>");
                        }
                    }

                    sbAORs.Append("</select>");

                    if (aorTypes[i]["AORType"].ToString() != "Workload MGMT" || (this.CanEditWorkloadMGMT && aorTypes[i]["AORType"].ToString() == "Workload MGMT")) sbAORs.Append("<img src=\"Images/Icons/cog.png\" alt=\"AOR Option Settings\" title=\"AOR Option Settings\" class=\"showaoroptionsettings\" width=\"15\" height=\"15\" onclick=\"showAOROptionSettings(this);\" style=\"cursor: pointer; margin-left: 3px;\" />");

                    sbAORs.Append("<div class=\"aoroptionsettings\" style=\"text-align: left; border: 1px solid gray; position: absolute; background-color: white; padding: 5px; display: none;\">");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" checked />Affiliated by selected System</label><br />");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />Affiliated by selected Assigned To/Primary Resource</label><br />");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />Affiliated by AOR Workload Type</label><br />");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />All (grouped by System)</label>");
                    sbAORs.Append("</div>");

                    if (aorTypes[i]["AORTYPE"].ToString() == "Release/Deployment MGMT")
                    {
                        sbAORs.Append("<br /><div style=\"padding-top: 3px;\">Workload Allocation: <span id=\"spnWorkloadAllocation\">" + aorTypes[i]["WorkloadAllocation"].ToString() + "</div>");
                    }

                    sbAORs.Append("</td>");
                    sbAORs.Append("<td id=\"MgmtReleaseAORsMsg\" height=\"20\" style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; display: none;\">" + prodStatusMsg + "</td>");
                }
                else
                {
                    sbAORs.Append("<td height=\"20\" style=\"border-left: 1px solid grey; text-align: center;\">" + aorTypes[i]["AORID"].ToString() + " - " + aorTypes[i]["AORName"].ToString());
                }

                sbAORs.Append("</td>");
                sbAORs.Append("</tr>");
                sbAORs.Append("</table>");
                if (aorTypes[i]["AORTYPE"].ToString() == "Release/Deployment MGMT")
                {
                    divReleaseAORs.InnerHtml = sbAORs.ToString();
                }
                else
                {
                    divWorkloadAORs.InnerHtml = sbAORs.ToString();
                }
                sbAORs.Clear();
            }
            else //Task does not have this type of AOR
            {
                string aorType = null;
                if (i == 0)
                {
                    aorType = "Release/Deployment MGMT";
                }
                else
                {
                    aorType = "Workload MGMT";
                }
                sbAORs.Append(aorType + " AOR");
                sbAORs.Append("</th>");
                sbAORs.Append("</tr>");
                sbAORs.Append("<tr class=\"gridBody\">");

                if (this.CanEdit)
                {
                    //if (aorType == "Release/Deployment MGMT" && (ddlProductionStatus.SelectedValue == "78" || ddlProductionStatus.SelectedValue == "108" || ddlProductionStatus.SelectedValue == "117"))
                    //{
                    //    sbAORs.Append("<td id=\"MgmtReleaseAORs\" style=\"text-align: center; border-left: 1px solid grey; display: none;\">");
                    //} else
                    //{
                    sbAORs.Append("<td id=\"MgmtReleaseAORs\" style=\"border-left: 1px solid grey; padding-left: 6px;\">");
                    //}
                    sbAORs.Append("<select field=\"" + aorType + "\" original_value=\"0\" onchange=\"AOR_change(this);\" style=\"width: 325px; background-color: #F5F6CE;\"");

                    if (!this.CanEditWorkloadMGMT && aorType == "Workload MGMT") sbAORs.Append(" disabled");

                    //sbAORs.Append("<select id=\"" + ddlID + "\" field =\"" + aorType + "\" original_value=\"0\" onchange=\"AOR_change(this);\" style=\"width: 325px; background-color: #F5F6CE;\"");

                    if (!this.CanEditWorkloadMGMT && aorType == "Workload MGMT")
                        sbAORs.Append(" disabled");

                    sbAORs.Append(">"); //select
                    sbAORs.Append("<option value=\"0\"></option>");

                    foreach (DataRow dr in dtAORLookup.Rows)
                    {
                        if (dr["AORType"].ToString() == "Release/Deployment MGMT"
                            && aorType.ToString() == "Release/Deployment MGMT")
                        {
                            sbAORs.Append("<option value=\"" + dr["AORReleaseID"].ToString() + "\" workloadAllocationID=\"" + dr["WorkloadAllocationID"].ToString() + "\" workloadAllocation=\"" + dr["WorkloadAllocation"].ToString() + "\"");
                            sbAORs.Append(">" + dr["AORID"].ToString() + " (" + dr["WorkloadAllocationAbbreviation"].ToString() + ") - " + dr["AORName"].ToString() + "</option>");
                        }
                        else if (dr["AORType"].ToString() != "Release/Deployment MGMT" && aorType.ToString() != "Release/Deployment MGMT")
                        {
                            sbAORs.Append("<option value=\"" + dr["AORReleaseID"].ToString() + "\" workloadAllocationID=\"" + dr["WorkloadAllocationID"].ToString() + "\" workloadAllocation=\"" + dr["WorkloadAllocation"].ToString() + "\"");
                            sbAORs.Append(">" + dr["AORID"].ToString() + " (" + dr["WorkloadAllocationAbbreviation"].ToString() + ") - " + dr["AORName"].ToString() + "</option>");
                        }
                    }

                    sbAORs.Append("</select>");

                    if (aorType != "Workload MGMT" || (this.CanEditWorkloadMGMT && aorType == "Workload MGMT")) sbAORs.Append("<img src=\"Images/Icons/cog.png\" alt=\"AOR Option Settings\" title=\"AOR Option Settings\" class=\"showaoroptionsettings\" width=\"15\" height=\"15\" onclick=\"showAOROptionSettings(this);\" style=\"cursor: pointer; margin-left: 3px;\" />");

                    sbAORs.Append("<div class=\"aoroptionsettings\" style=\"text-align: left; border: 1px solid gray; position: absolute; background-color: white; padding: 5px; display: none;\">");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" checked />Affiliated by selected System</label><br />");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />Affiliated by selected Assigned To/Primary Resource</label><br />");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />Affiliated by AOR Workload Type</label><br />");
                    sbAORs.Append("<label name=\"aoroptionsettingsinput\"><input type=\"checkbox\" name=\"aoroptionsettingsinput\" onchange=\"getAOROptions(this);\" />All (grouped by System)</label>");
                    sbAORs.Append("</div>");

                    if (aorType == "Release/Deployment MGMT")
                    {
                        sbAORs.Append("<br /><div style=\"padding-top: 3px;\">Workload Allocation: <span id=\"spnWorkloadAllocation\"></div>");
                    }

                    sbAORs.Append("</td>");
                    //if (aorType == "Release/Deployment MGMT" && (ddlProductionStatus.SelectedValue == "78" || ddlProductionStatus.SelectedValue == "108" || ddlProductionStatus.SelectedValue == "117"))
                    //{
                    //    sbAORs.Append("<td id=\"MgmtReleaseAORsMsg\" height=\"20\" style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center;\">" + prodStatusMsg + "</td>");
                    //} else
                    //{
                    sbAORs.Append("<td id=\"MgmtReleaseAORsMsg\" height=\"20\" style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; display: none;\">" + prodStatusMsg + "</td>");
                    //}
                }
                else
                {
                    sbAORs.Append("<td height=\"20\" style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center;\">No " + aorType + " AORs</td>");
                }
                sbAORs.Append("</td>");
                sbAORs.Append("</tr>");
                sbAORs.Append("</table>");
                if (aorType == "Release/Deployment MGMT" || aorType == null)
                {
                    divReleaseAORs.InnerHtml = sbAORs.ToString();
                }
                else
                {
                    divWorkloadAORs.InnerHtml = sbAORs.ToString();
                }
                sbAORs.Clear();
            }
        }
    }

    private string calculateSubtaskWorkloadPriority(DataTable dtSubtask)
    {
        int emergencyCount = 0, currentCount = 0, runCount = 0, stagedCount = 0, unpriorCount = 0, closeCount = 0, openCount = 0, percentComplete = 0;

        foreach (DataRow dr in dtSubtask.Rows)
        {
            switch (dr["AssignedToRankID"].ToString())
            {
                case "27":
                    emergencyCount++;
                    break;
                case "28":
                    currentCount++;
                    break;
                case "38":
                    runCount++;
                    break;
                case "29":
                    stagedCount++;
                    break;
                case "30":
                    unpriorCount++;
                    break;
                case "31":
                    closeCount++;
                    break;
            }
        }

        openCount = emergencyCount + currentCount + runCount + stagedCount + unpriorCount;
        if (openCount + closeCount > 0) percentComplete = (closeCount * 100) / (openCount + closeCount);

        return "" + emergencyCount + "." + currentCount + "." + runCount + "." + stagedCount + "." + unpriorCount + "." + closeCount + " (" + openCount + ", " + percentComplete + "%)";
    }

    [WebMethod(true)]
    public static string SaveWorkItem(object workItem, string copySubTasks, string originalWorkItemID, string aors, bool cascadeAOR)  // 12817 - 7 >> , string copySubTasks,string originalWorkItemID
    {
        Dictionary<string, string> result = new Dictionary<string, string>() {
            { "saved", "" }
            , { "id","0" }
            , { "error", "" } };
        bool saved = false;
        int savedInt = 0;
        int id = 0;

        bool isDev = false;
        bool isBus = false;

        string errorMsg = string.Empty;
        DateTime dtTemp;

        Dictionary<string, object> attributeList;

        HttpServerUtility server = HttpContext.Current.Server;

        if (workItem == null)
        {
            saved = false;
            errorMsg = "No Workload Item details were found.";
        }
        else
        {
            try
            {
                attributeList = (Dictionary<string, object>)workItem;

                WorkloadItem item = new WorkloadItem(attributeList);
                item.Description = server.UrlDecode(item.Description);

                // 14149 - 2: Set Actual Dev Start to current date when status gets changed to "In Progress".
                if (item.StatusID == 5 && item.ActualDevStart == null)
                {
                    item.ActualDevStart = DateTime.Now.ToString();
                }
                // 14149 - 2: Set Actual Completion to current date when % Comlete is set to 100.
                if (item.CompletionPercent == 100 && item.ActualCompletionDate == null)
                {
                    item.ActualCompletionDate = DateTime.Now.ToString();
                }

                // 3-13-2017 - If Assigned To is Dev, set primaryTechRes same ID - same with Bus.
                isDev = WTSData.GetScalarBool("SELECT IsDeveloper FROM WTS_RESOURCE WHERE WTS_RESOURCEID = " + item.AssignedResourceID);
                if (isDev & item.PrimaryResourceID == 67)
                {
                    item.PrimaryResourceID = item.AssignedResourceID;
                }
                else
                {
                    isBus = WTSData.GetScalarBool("SELECT IsBusAnalyst FROM WTS_RESOURCE WHERE WTS_RESOURCEID = " + item.AssignedResourceID);
                    if (isBus & item.PrimaryBusinessResourceID == 67)
                        item.PrimaryBusinessResourceID = item.AssignedResourceID;
                }

                XmlDocument docAORs = (XmlDocument)JsonConvert.DeserializeXmlNode(aors, "aors");
                bool savedAOR = false;

                if (item.WorkItemID == 0) //save new item
                {
                    // 14149 - 2: Set Estimated Completion to current date + 1.
                    dtTemp = DateTime.Now;
                    item.EstimatedCompletionDate = dtTemp.AddDays(1).ToString();

                    // SCB TODO - Waiting on further design decisions.
                    //if (item.WorkTypeID == 3)
                    //{
                    //    string previousDescr = item.Description;

                    // NOTE: Neither \n\r or Environment.NewLine work

                    //    item.Description = "This task is a single umbrella task that tracks CR artifacts. This includes:" + Environment.NewLine;  
                    //    item.Description += "ARTIFACT: Approved Technical Design Slides - File in Attachments" + Environment.NewLine; 
                    //    item.Description += "ARTIFACT: Approved Customer Design Slides - File in Attachments" + Environment.NewLine;
                    //    item.Description += "ARTIFACT: Approved Developer Meeting Minutes (Note: Please ensure Dev Meeting Minutes contain all required information) - File in Attachments" + Environment.NewLine;
                    //    item.Description += "ARTIFACT: Screen Shot of Approved Data Model - Required when Data Model Changes are Made - File in Attachments" + Environment.NewLine + Environment.NewLine;

                    //    item.Description += "Existing description: " + previousDescr;
                    //}

                    int newID = 0;
                    saved = WorkloadItem.WorkItem_Add(item, copySubTasks, originalWorkItemID, out newID, out errorMsg);  // 12817 - 7 - Add ? > , CopySubTasks, originalWorkItemID
                    id = newID;
                    if (saved)
                    {
                        savedAOR = AOR.AORTask_Save(TaskID: id, AORs: docAORs, Add: 1, CascadeAOR: cascadeAOR);
                        Workload.SendWorkloadEmail("WorkItem", true, id);
                        //if (savedAOR)
                        //{
                        //    DataTable dt = WorkloadItem.WorkItem_GetTaskList(workItemID: id, showArchived: 0, showBacklog: false);

                        //    if (dt != null && dt.Rows.Count > 0)
                        //    {
                        //        foreach (DataRow dr in dt.Rows)
                        //        {
                        //            int subtaskID = 0;
                        //            int.TryParse(dr["WORKITEM_TASKID"].ToString(), out subtaskID);
                        //            savedAOR = AOR.AORSubTask_Save(TaskID: subtaskID, AORs: docAORs, Add: 1, CascadeAOR: cascadeAOR);
                        //        }
                        //    }
                        //}
                    }
                }
                else //update existing item
                {
                    id = item.WorkItemID;
                    savedInt = WorkloadItem.WorkItem_Update(item, out errorMsg);

                    if (savedInt == 0)
                    {
                        saved = false;
                    }
                    else
                    {
                        saved = true;
                    }

                    if (savedInt == 1)
                    {
                        savedAOR = AOR.AORTask_Save(TaskID: id, AORs: docAORs, Add: 0, CascadeAOR: cascadeAOR);
                        Workload.SendWorkloadEmail("WorkItem", false, id);
                        //if (savedAOR)
                        //{
                        //    DataTable dt = WorkloadItem.WorkItem_GetTaskList(workItemID: id, showArchived: 0, showBacklog: false);

                        //    if (dt != null && dt.Rows.Count > 0)
                        //    {
                        //        foreach (DataRow dr in dt.Rows)
                        //        {
                        //            int subtaskID = 0;
                        //            int.TryParse(dr["WORKITEM_TASKID"].ToString(), out subtaskID);
                        //            savedAOR = AOR.AORSubTask_Save(TaskID: subtaskID, AORs: docAORs, Add: 1, CascadeAOR: cascadeAOR);
                        //        }
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                saved = false;
                errorMsg = ex.Message;
                LogUtility.LogException(ex);
            }
        }

        result["saved"] = saved.ToString();
        result["id"] = id.ToString();
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
    public static string GetAORHistory(string taskID)
    {
        DataTable dt = new DataTable();

        try
        {
            int task_ID = 0;

            int.TryParse(taskID, out task_ID);

            dt = AOR.AORTaskAORHistoryList_Get(TaskID: task_ID);
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
}