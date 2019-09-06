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
public partial class MassChange_Wizard : System.Web.UI.Page
{
    #region Variables
    protected bool CanEditAOR = false;
    protected bool CanViewAOR = false;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    #endregion

    #region Page
    protected void Page_Load(object sender, EventArgs e)
    {
        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);
        this.CanViewAOR = this.CanEditAOR || UserManagement.UserCanView(WTSModuleOption.AOR);
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewWorkItem = this.CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

        LoadAvailableEntities();
    }

    private void LoadAvailableEntities()
    {
        StringBuilder sbEntities = new StringBuilder();

        sbEntities.Append("<table id=\"tblEntity\" runat=\"server\" style=\"border-collapse: collapse; width: 100px;\" > ");
        if (this.CanEditAOR) {
            sbEntities.Append("<tr>");
            sbEntities.Append("<td>");
            sbEntities.Append("<div id=\"AOR\" stepType=\"Entity\" class=\"nav\" >AOR</div>");
            sbEntities.Append("</td>");
            sbEntities.Append("</tr>");

            sbEntities.Append("<tr>");
            sbEntities.Append("<td>");
            sbEntities.Append("<div id=\"CR\" stepType=\"Entity\" class=\"nav\" >CR</div>");
            sbEntities.Append("</td>");
            sbEntities.Append("</tr>");
        }
        if (this.CanEditWorkItem)
        {
            sbEntities.Append("<tr>");
            sbEntities.Append("<td>");
            sbEntities.Append("<div id=\"PrimaryTask\" stepType=\"Entity\" class=\"nav\" >Primary Task</div>");
            sbEntities.Append("</td>");
            sbEntities.Append("</tr>");

            sbEntities.Append("<tr>");
            sbEntities.Append("<td>");
            sbEntities.Append("<div id=\"Subtask\" stepType=\"Entity\" class=\"nav\" >Subtask</div>");
            sbEntities.Append("</td>");
            sbEntities.Append("</tr>");
        }
        sbEntities.Append("</table>");
        divEntity.InnerHtml = sbEntities.ToString();

    }
    #endregion

    #region AJAX

    [WebMethod()]
    public static string LoadEntityFields(string entityType)
    {
        DataTable dtNew = new DataTable();
        DataColumnCollection DCC;
        Dictionary<string, GridColsColumn> _gridColumns = new Dictionary<string, GridColsColumn>();
        GridCols _columnData = new GridCols();
        DataTable dt = new DataTable();
        int AOR_ID = 0;
        bool isVisible = false;
        string dbName = string.Empty, displayName = string.Empty, idField = string.Empty;
        var ordered = _gridColumns.Keys.OrderBy(k => _gridColumns[k].DisplayName).ToList();
        try
        {
            switch (entityType.ToUpper())
            {
                case "AOR":
                    dt = AOR.AORList_Get(AORID: AOR_ID);

                    _gridColumns = new Dictionary<string, GridColsColumn>();

                    GridColsColumn column = new GridColsColumn();
                    dbName = string.Empty;
                    displayName = string.Empty;
                    idField = string.Empty;
                    isVisible = false;

                    foreach (DataColumn gridColumn in dt.Columns)
                    {
                        column = new GridColsColumn();
                        displayName = gridColumn.ColumnName;
                        idField = gridColumn.ColumnName;
                        isVisible = false;

                        switch (gridColumn.ColumnName)
                        {
                            //case "Current Release":
                            //    displayName = "Current Release";
                            //    idField = "ProductVersion_ID";
                            //    isVisible = true;
                            //    break;
                            case "Workload Allocation":
                                displayName = "Workload Allocation";
                                idField = "WorkloadAllocation_ID";
                                isVisible = true;
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
                    }

                    //Initialize the columnData
                    _columnData.Initialize(ref dt, ";", "~", "|");

                    dtNew.Columns.Add("valueField");
                    dtNew.Columns.Add("textField");
                    dtNew.Columns.Add("id_field");

                    ordered = _gridColumns.Keys.OrderBy(k => _gridColumns[k].DisplayName).ToList();

                    foreach (string key in ordered)
                    {
                        GridColsColumn col = _gridColumns[key];
                        if (col.Visible)
                        {
                            DataRow dr = dtNew.NewRow();

                            dr[0] = col.ColumnName;
                            dr[1] = col.DisplayName;
                            dr[2] = col.SortName;

                            dtNew.Rows.Add(dr);
                        }
                    }
                    break;
                case "CR":
                    dt = AOR.AORCRList_Get(AORID: AOR_ID, AORReleaseID: 0, CRID: 0);

                    _gridColumns = new Dictionary<string, GridColsColumn>();

                    column = new GridColsColumn();
                    dbName = string.Empty;
                    displayName = string.Empty;
                    idField = string.Empty;
                    isVisible = false;

                    foreach (DataColumn gridColumn in dt.Columns)
                    {
                        column = new GridColsColumn();
                        displayName = gridColumn.ColumnName;
                        idField = gridColumn.ColumnName;
                        isVisible = false;

                        switch (gridColumn.ColumnName)
                        {
                            case "Contract":
                                displayName = "Contract";
                                idField = "Contract_ID";
                                isVisible = true;
                                break;
                            case "Websystem":
                                displayName = "Websystem";
                                idField = "Websystem_ID";
                                isVisible = false;
                                break;
                            case "Related Release":
                                displayName = "Related Release";
                                idField = "RelatedRelease_ID";
                                isVisible = false;
                                break;
                            case "Status":
                                displayName = "CR Coordination";
                                idField = "Status_ID";
                                isVisible = true;
                                break;
                            case "CyberISMT":
                                displayName = "Cyber/ISMT";
                                idField = "CyberISMT_ID";
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
                    }

                    //Initialize the columnData
                    _columnData.Initialize(ref dt, ";", "~", "|");

                    dtNew.Columns.Add("valueField");
                    dtNew.Columns.Add("textField");
                    dtNew.Columns.Add("id_field");

                    ordered = _gridColumns.Keys.OrderBy(k => _gridColumns[k].DisplayName).ToList();

                    foreach (string key in ordered)
                    {
                        GridColsColumn col = _gridColumns[key];
                        if (col.Visible)
                        {
                            DataRow dr = dtNew.NewRow();

                            dr[0] = col.ColumnName;
                            dr[1] = col.DisplayName;
                            dr[2] = col.SortName;

                            dtNew.Rows.Add(dr);
                        }
                    }
                    break;
                case "PRIMARYTASK":
                    dt = AOR.AORTaskList_Get(AORID: AOR_ID, AORReleaseID: 0);
            //        dt = WorkloadItem.WorkItemList_Get(workRequestID: 0
            //, showArchived: 0
            //, columnListOnly: 0
            //, myData: false);
                    _gridColumns = new Dictionary<string, GridColsColumn>();

                    column = new GridColsColumn();
                    dbName = string.Empty;
                    displayName = string.Empty;
                    idField = string.Empty;
                    isVisible = false;

                    foreach (DataColumn gridColumn in dt.Columns)
                    {
                        column = new GridColsColumn();
                        displayName = gridColumn.ColumnName;
                        idField = gridColumn.ColumnName;
                        isVisible = false;

                        switch (gridColumn.ColumnName)
                        {
                            case "System(Task)":
                                displayName = "System(Task)";
                                idField = "WTS_SYSTEM_ID";
                                isVisible = true;
                                break;
                            case "Production Status":
                                displayName = "Production Status";
                                idField = "ProductionStatus_ID";
                                isVisible = true;
                                break;
                            //case "Product Version":
                            //    displayName = "Product Version";
                            //    idField = "ProductVersion_ID";
                            //    isVisible = true;
                            //    break;
                            case "Priority":
                                displayName = "Priority";
                                idField = "PRIORITY_ID";
                                isVisible = true;
                                break;
                            case "Primary Resource":
                                displayName = "Primary Resource";
                                idField = "PrimaryTechResource_ID";
                                isVisible = true;
                                break;
                            case "Assigned To":
                                displayName = "Assigned To";
                                idField = "AssignedTo_ID";
                                isVisible = true;
                                break;
                            case "Status":
                                displayName = "Status";
                                idField = "STATUS_ID";
                                isVisible = true;
                                break;
                            case "Percent Complete":
                                displayName = "Percent Complete";
                                idField = "PercentComplete_ID";
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
                    }

                    //Initialize the columnData
                    _columnData.Initialize(ref dt, ";", "~", "|");

                    dtNew.Columns.Add("valueField");
                    dtNew.Columns.Add("textField");
                    dtNew.Columns.Add("id_field");

                    ordered = _gridColumns.Keys.OrderBy(k => _gridColumns[k].DisplayName).ToList();

                    foreach (string key in ordered)
                    {
                        GridColsColumn col = _gridColumns[key];
                        if (col.Visible)
                        {
                            DataRow dr = dtNew.NewRow();

                            dr[0] = col.DisplayName;
                            dr[1] = col.ColumnName;
                            dr[2] = col.SortName;

                            dtNew.Rows.Add(dr);
                        }
                    }
                    break;
                case "SUBTASK":
                    //dt = AOR.AORTaskList_Get(AORID: AOR_ID, AORReleaseID: 0);
                    dt = WorkloadItem.WorkItem_GetTaskList(workItemID: 0, showArchived: 0, showBacklog: false);

                    _gridColumns = new Dictionary<string, GridColsColumn>();

                    column = new GridColsColumn();
                    dbName = string.Empty;
                    displayName = string.Empty;
                    idField = string.Empty;
                    isVisible = false;

                    foreach (DataColumn gridColumn in dt.Columns)
                    {
                        column = new GridColsColumn();
                        displayName = gridColumn.ColumnName;
                        idField = gridColumn.ColumnName;
                        isVisible = false;

                        switch (gridColumn.ColumnName)
                        {
                            case "AssignedResource":
                                displayName = "Assigned To";
                                idField = "ASSIGNEDRESOURCEID";
                                isVisible = true;
                                break;
                            case "Production Status":
                                displayName = "Production Status";
                                idField = "ProductionStatus_ID";
                                isVisible = true;
                                break;
                            //case "Product Version":
                            //    displayName = "Product Version";
                            //    idField = "ProductVersion_ID";
                            //    isVisible = true;
                            //    break;
                            case "Priority":
                                displayName = "Priority";
                                idField = "PRIORITY_ID";
                                isVisible = true;
                                break;
                            case "Primary Resource":
                                displayName = "Primary Resource";
                                idField = "PrimaryTechResource_ID";
                                isVisible = true;
                                break;
                            case "Assigned To":
                                displayName = "Assigned To";
                                idField = "AssignedTo_ID";
                                isVisible = true;
                                break;
                            case "Status":
                                displayName = "Status";
                                idField = "STATUS_ID";
                                isVisible = true;
                                break;
                            case "Percent Complete":
                                displayName = "Percent Complete";
                                idField = "PercentComplete_ID";
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
                    }

                    //Initialize the columnData
                    _columnData.Initialize(ref dt, ";", "~", "|");

                    dtNew.Columns.Add("valueField");
                    dtNew.Columns.Add("textField");
                    dtNew.Columns.Add("id_field");

                    ordered = _gridColumns.Keys.OrderBy(k => _gridColumns[k].DisplayName).ToList();

                    foreach (string key in ordered)
                    {
                        GridColsColumn col = _gridColumns[key];
                        if (col.Visible)
                        {
                            DataRow dr = dtNew.NewRow();

                            dr[0] = col.ColumnName;
                            dr[1] = col.DisplayName;
                            dr[2] = col.SortName;

                            dtNew.Rows.Add(dr);
                        }
                    }
                    break;
            }

        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dtNew, Newtonsoft.Json.Formatting.None);
    }
    [WebMethod(true)]
    public static string LoadExistingValues(string entityType, string idField, string columnName, string textField, string filterField, string existingValueFilter)
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
        int AOR_ID = 0;
        DataTable dt = null, dtCurrentOptions = null, dtNewValues = null;
        try
        {

            switch (entityType.ToUpper())
            {
                case "AOR":
                    dt = AOR.AORList_Get(AORID: AOR_ID);
                    break;
                case "CR":
                    dt = AOR.AORCRList_Get(AORID: AOR_ID, AORReleaseID: 0, CRID: 0);
                    break;
                case "PRIMARYTASK":
                    dt = AOR.AORTaskList_Get(AORID: AOR_ID, AORReleaseID: 0);
                    break;
                case "SUBTASK":
                    dt = WorkloadItem.WorkItem_GetTaskList(workItemID: 0, showArchived: 0, showBacklog: false);
                    break;
            }
            if (existingValueFilter.Length > 0) {
                dt.DefaultView.RowFilter = filterField + " IN (" + existingValueFilter + ")";
            }
            if (entityType.ToUpper() == "AOR") dt.DefaultView.RowFilter = "[Workload Allocation Archive] IN (False)";
            dtCurrentOptions = entityType.ToUpper() == "AOR" ? dt.DefaultView.ToTable(distinct: true, columnNames: new string[] { idField, columnName, "Workload Allocation Archive" }) : dt.DefaultView.ToTable(distinct: true, columnNames: new string[] { idField, columnName });
            if (dtCurrentOptions != null)
            {
                dtCurrentOptions.Columns[idField].ColumnName = "valueField";
                dtCurrentOptions.Columns[columnName].ColumnName = "textField";
            }
            dtCurrentOptions.DefaultView.Sort = "textField";
            dtCurrentOptions = dtCurrentOptions.DefaultView.ToTable();
            currentCount = dtCurrentOptions.Rows.Count;

            try
            {
                switch (entityType.ToUpper())
                {
                    case "AOR":
                        switch (columnName.ToUpper())
                        {

                            case "CURRENT RELEASE":
                                dtNewValues = MasterData.ProductVersionList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["ProductVersionID"].ColumnName = "valueField";
                                    dtNewValues.Columns["ProductVersion"].ColumnName = "textField";
                                }

                                break;
                            case "WORKLOAD ALLOCATION":
                                dtNewValues = MasterData.WorkloadAllocationList_Get(includeArchive: 0);
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["WorkloadAllocationID"].ColumnName = "valueField";
                                    dtNewValues.Columns["WorkloadAllocation"].ColumnName = "textField";
                                }

                                break;
                            default:
                                dtNewValues = null;
                                break;
                        }
                        break;
                    case "CR":
                        switch (columnName.ToUpper())
                        {

                            case "CONTRACT":
                                dtNewValues = MasterData.ContractList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["ContractID"].ColumnName = "valueField";
                                    dtNewValues.Columns["Contract"].ColumnName = "textField";
                                }

                                break;
                            case "STATUS":
                                dtNewValues = MasterData.StatusList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues.DefaultView.RowFilter = "StatusType = 'AORCR'";
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["StatusID"].ColumnName = "valueField";
                                    dtNewValues.Columns["Status"].ColumnName = "textField";
                                }
                                break;
                            case "WEBSYSTEM":
                                dtNewValues = MasterData.SystemList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["WTS_SYSTEMID"].ColumnName = "valueField";
                                    dtNewValues.Columns["WTS_SYSTEM"].ColumnName = "textField";
                                }

                                break;
                            default:
                                dtNewValues = null;
                                break;
                        }
                        break;
                    case "PRIMARYTASK":
                        switch (columnName.ToUpper())
                        {

                            case "ASSIGNED TO":
                                dtNewValues = UserManagement.LoadUserList(organizationId: 0, excludeDeveloper: false, loadArchived: false, userNameSearch: "");
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["WTS_RESOURCEID"].ColumnName = "valueField";
                                    dtNewValues.Columns["UserName"].ColumnName = "textField";
                                }

                                break;
                            case "PRIMARY RESOURCE":
                                dtNewValues = UserManagement.LoadUserList(organizationId: 0, excludeDeveloper: false, loadArchived: false, userNameSearch: "");
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["WTS_RESOURCEID"].ColumnName = "valueField";
                                    dtNewValues.Columns["UserName"].ColumnName = "textField";
                                }

                                break;
                            case "PRIORITY":
                                dtNewValues = MasterData.PriorityList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues.DefaultView.RowFilter = "PriorityType = 'Work Item'";
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["PriorityID"].ColumnName = "valueField";
                                    dtNewValues.Columns["Priority"].ColumnName = "textField";
                                }

                                break;
                            case "PRODUCTION STATUS":
                                dtNewValues = MasterData.StatusList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues.DefaultView.RowFilter = "StatusType = 'Production'";
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["StatusID"].ColumnName = "valueField";
                                    dtNewValues.Columns["Status"].ColumnName = "textField";
                                }

                                break;
                            case "STATUS":
                                dtNewValues = MasterData.StatusList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues.DefaultView.RowFilter = "StatusType = 'Work'";
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["StatusID"].ColumnName = "valueField";
                                    dtNewValues.Columns["Status"].ColumnName = "textField";
                                }

                                break;
                            case "PRODUCT VERSION":
                                dtNewValues = MasterData.ProductVersionList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["ProductVersionID"].ColumnName = "valueField";
                                    dtNewValues.Columns["ProductVersion"].ColumnName = "textField";
                                }

                                break;
                            case "SYSTEM(TASK)":
                                dtNewValues = MasterData.SystemList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["WTS_SYSTEMID"].ColumnName = "valueField";
                                    dtNewValues.Columns["WTS_SYSTEM"].ColumnName = "textField";
                                }

                                break;
                            //case "PERCENT COMPLETE":
                            //    dtNewValues = MasterData.ProductVersionList_Get(includeArchive: false);
                            //    if (dtNewValues != null)
                            //    {
                            //        dtNewValues = dtNewValues.DefaultView.ToTable();
                            //        dtNewValues.Columns["ProductVersionID"].ColumnName = "valueField";
                            //        dtNewValues.Columns["ProductVersion"].ColumnName = "textField";
                            //    }

                                //break;

                            default:
                                dtNewValues = null;
                                break;
                        }
                        break;
                    case "SUBTASK":
                        switch (columnName.ToUpper())
                        {

                            case "ASSIGNEDRESOURCE":
                                dtNewValues = UserManagement.LoadUserList(organizationId: 0, excludeDeveloper: false, loadArchived: false, userNameSearch: "");
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["WTS_RESOURCEID"].ColumnName = "valueField";
                                    dtNewValues.Columns["UserName"].ColumnName = "textField";
                                }

                                break;
                            case "PRIMARY RESOURCE":
                                dtNewValues = UserManagement.LoadUserList(organizationId: 0, excludeDeveloper: false, loadArchived: false, userNameSearch: "");
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["WTS_RESOURCEID"].ColumnName = "valueField";
                                    dtNewValues.Columns["UserName"].ColumnName = "textField";
                                }

                                break;
                            case "PRIORITY":
                                dtNewValues = MasterData.PriorityList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues.DefaultView.RowFilter = "PriorityType = 'Work Item'";
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["PriorityID"].ColumnName = "valueField";
                                    dtNewValues.Columns["Priority"].ColumnName = "textField";
                                }

                                break;
                            case "PRODUCTION STATUS":
                                dtNewValues = MasterData.StatusList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues.DefaultView.RowFilter = "StatusType = 'Production'";
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["StatusID"].ColumnName = "valueField";
                                    dtNewValues.Columns["Status"].ColumnName = "textField";
                                }

                                break;
                            case "STATUS":
                                dtNewValues = MasterData.StatusList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues.DefaultView.RowFilter = "StatusType = 'Work'";
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["StatusID"].ColumnName = "valueField";
                                    dtNewValues.Columns["Status"].ColumnName = "textField";
                                }

                                break;
                            case "PRODUCT VERSION":
                                dtNewValues = MasterData.ProductVersionList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["ProductVersionID"].ColumnName = "valueField";
                                    dtNewValues.Columns["ProductVersion"].ColumnName = "textField";
                                }

                                break;
                            case "SYSTEM(TASK)":
                                dtNewValues = MasterData.SystemList_Get(includeArchive: false);
                                if (dtNewValues != null)
                                {
                                    dtNewValues = dtNewValues.DefaultView.ToTable();
                                    dtNewValues.Columns["WTS_SYSTEMID"].ColumnName = "valueField";
                                    dtNewValues.Columns["WTS_SYSTEM"].ColumnName = "textField";
                                }

                                break;
                            //case "PERCENT COMPLETE":
                            //    dtNewValues = MasterData.ProductVersionList_Get(includeArchive: false);
                            //    if (dtNewValues != null)
                            //    {
                            //        dtNewValues = dtNewValues.DefaultView.ToTable();
                            //        dtNewValues.Columns["ProductVersionID"].ColumnName = "valueField";
                            //        dtNewValues.Columns["ProductVersion"].ColumnName = "textField";
                            //    }

                            //break;

                            default:
                                dtNewValues = null;
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
                dtNewValues = null;
            }

            if (dtNewValues != null && dtNewValues.Rows.Count > 0)
            {
                dtNewValues.DefaultView.Sort = "textField";
                dtNewValues = dtNewValues.DefaultView.ToTable(distinct: true, columnNames: new string[] { "valueField", "textField" });
                if (dtNewValues != null)
                {
                    newCount = dtNewValues.Rows.Count;
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
            result["NewOptions"] = JsonConvert.SerializeObject(dtNewValues, Formatting.None);
        }

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

    [WebMethod(true)]
    public static string LoadFilteredEntity(string entityType, string idField, string columnName, string textField, string existingValueFilter, string filterField, string filterFieldIDs)
    {
        DataTable dtNew = new DataTable();
        DataTable dt = new DataTable();
        int AOR_ID = 0;
        try
        {

            switch (entityType.ToUpper())
            {
                case "AOR":
                    dt = AOR.AORList_Get(AORID: AOR_ID);
                    if (filterFieldIDs.Length > 0)
                    {
                        dt.DefaultView.RowFilter =  filterField + " IN (" + filterFieldIDs + ")";
                        dt = dt.DefaultView.ToTable();
                    }

                    dt.DefaultView.RowFilter = idField + " IN (" + existingValueFilter + ")";
                    dtNew = dt.DefaultView.ToTable(distinct: true, columnNames: new string[] { "AOR #", "AOR Name" });
                    dtNew.Columns["AOR #"].ColumnName = "valueField";
                    dtNew.Columns["AOR Name"].ColumnName = "textField";
                    break;
                case "CR":
                    dt = AOR.AORCRList_Get(AORID: AOR_ID, AORReleaseID: 0, CRID: 0);
                    if (filterFieldIDs.Length > 0)
                    {
                        dt.DefaultView.RowFilter = filterField + " IN (" + filterFieldIDs + ")";
                        dt = dt.DefaultView.ToTable();
                    }
                    dt.DefaultView.RowFilter = idField + " IN (" + existingValueFilter + ")";
                    dtNew = dt.DefaultView.ToTable(distinct: true, columnNames: new string[] { "CR_ID", "CR Customer Title" });
                    dtNew.Columns["CR_ID"].ColumnName = "valueField";
                    dtNew.Columns["CR Customer Title"].ColumnName = "textField";
                    break;
                case "PRIMARYTASK":
                    dt = AOR.AORTaskList_Get(AORID: AOR_ID, AORReleaseID: 0);
                    if (filterFieldIDs.Length > 0)
                    {
                        dt.DefaultView.RowFilter = filterField + " IN (" + filterFieldIDs + ")";
                        dt = dt.DefaultView.ToTable();
                    }
                    dt.DefaultView.RowFilter = idField + " IN (" + existingValueFilter + ")";
                    dtNew = dt.DefaultView.ToTable(distinct: true, columnNames: new string[] { "Task #", "Title" });
                    dtNew.Columns["Task #"].ColumnName = "valueField";
                    dtNew.Columns["Title"].ColumnName = "textField";
                    break;
                case "SUBTASK":
                    dt = WorkloadItem.WorkItem_GetTaskList(workItemID: 0, showArchived: 0, showBacklog: false);
                    if (filterFieldIDs.Length > 0)
                    {
                        dt.DefaultView.RowFilter = filterField + " IN (" + filterFieldIDs + ")";
                        dt = dt.DefaultView.ToTable();
                    }
                    dt.DefaultView.RowFilter = idField + " IN (" + existingValueFilter + ")";
                    dtNew = dt.DefaultView.ToTable(distinct: true, columnNames: new string[] { "WORKITEMID", "TASK_NUMBER", "WORKITEM_TASKID", "Title" });
                    dtNew.Columns["WORKITEMID"].ColumnName = "WORKITEMID";
                    dtNew.Columns["TASK_NUMBER"].ColumnName = "TASK_NUMBER";
                    dtNew.Columns["WORKITEM_TASKID"].ColumnName = "valueField";
                    dtNew.Columns["Title"].ColumnName = "textField";
                    break;
            }

            dtNew.DefaultView.Sort = "textField";
            dtNew = dtNew.DefaultView.ToTable();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }

        return JsonConvert.SerializeObject(dtNew, Newtonsoft.Json.Formatting.None);
    }


    [WebMethod(true)]
    public static string SaveChanges(string entityType, string fieldName, string existingValue, string newValue, string entityFilter)
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
            if (newValue.Length == 0)
            {
                errorMsg = "No new value was specified.";
                proceed = false;
                saved = false;
                count = 0;
            }

            if (proceed)
            {
                if (existingValue.Length == 0)
                {
                    //this is okay, but make sure procedure works with empty old value
                    string msg = "";
                }

                //save the data
                count = AOR.AORMassChange_Save(entityType: entityType
                    , fieldName: fieldName
                    , existingValue: existingValue
                    , newValue: newValue
                    , entityFilter: entityFilter
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
    #endregion
}