using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_Tasks : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected bool CanEditWorkItem = false;
    protected bool CanViewWorkItem = false;
    protected int AORID = 0;
    protected int AORReleaseID = 0;
    protected int WorkItemID = 0;
    protected bool ReleaseAOR = false;
    protected DataColumnCollection DCC;
    protected int GridPageIndex = 0;
    protected int RowCount = 0;
    protected int SubtaskCount = 0;
    protected string ddlChanged = "no";
    protected bool _myData = true;
    protected string[] SelectedStatuses = { };
    protected string[] SelectedAssigned;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();
        LoadQF();

        this.CanEditAOR = (UserManagement.UserCanEdit(WTSModuleOption.AOR) && AOR.AORReleaseCurrent(AORID: this.AORID, AORReleaseID: this.AORReleaseID));
        this.CanEditWorkItem = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);
        this.CanViewWorkItem = this.CanEditWorkItem || UserManagement.UserCanView(WTSModuleOption.WorkItem);

        DataTable dt = LoadData();

        if (dt != null)
        {
            this.DCC = dt.Columns;
            this.RowCount = dt.Rows.Count;
        }

        grdData.DataSource = dt;

        if (!Page.IsPostBack && this.GridPageIndex > 0 && this.GridPageIndex < ((decimal)dt.Rows.Count / (decimal)25)) grdData.PageIndex = this.GridPageIndex;

        grdData.DataBind();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
            || Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
        {
            this.MyData = true;
        }
        else
        {
            this.MyData = false;
        }

        if (Request.QueryString["AORID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORID"]))
        {
            int.TryParse(Request.QueryString["AORID"], out this.AORID);
        }

        if (Request.QueryString["AORReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AORReleaseID"]))
        {
            int.TryParse(Request.QueryString["AORReleaseID"], out this.AORReleaseID);
        }

        if (Request.QueryString["WorkItemID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["WorkItemID"]))
        {
            int.TryParse(Request.QueryString["WorkItemID"], out this.WorkItemID);
        }

        if (Request.QueryString["ReleaseAOR"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseAOR"]))
        {
            bool.TryParse(Request.QueryString["ReleaseAOR"], out this.ReleaseAOR);
        }

        if (Request.QueryString["GridPageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndex"]))
        {
            int.TryParse(Request.QueryString["GridPageIndex"], out this.GridPageIndex);
        }

        if (Request.QueryString["ddlChanged"] != null)
        {
            ddlChanged = Request.QueryString["ddlChanged"].ToString();
        }

        if (Request.QueryString["SelectedStatuses"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedStatuses"].ToString()))
        {
            SelectedStatuses = Server.UrlDecode(Request.QueryString["SelectedStatuses"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
        if (Request.QueryString["SelectedAssigned"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAssigned"].ToString()))
        {
            SelectedAssigned = Server.UrlDecode(Request.QueryString["SelectedAssigned"].Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Session["Assigned"] = SelectedAssigned;
        }

    }

    private void LoadQF()
    {
        DataTable dtStatus = MasterData.StatusList_Get();
        HtmlSelect ddlStatus = (HtmlSelect)Page.Master.FindControl("ms_Item0");
        string selected = "";

        Label lblStatus = (Label)Page.Master.FindControl("lblms_Item0");
        lblStatus.Text = "Status: ";
        lblStatus.Style["width"] = "150px";


        //Set the SelectedStatuses from the QueryString parameters(If they exist). (If they dont exist) Set the SelectedStatuses from the selected items in the MasterData menu item
        if (dtStatus != null)
        {
            ddlStatus.Items.Clear();
            dtStatus.DefaultView.RowFilter = "StatusType = 'Work'";
            dtStatus = dtStatus.DefaultView.ToTable(true, new string[] { "StatusID", "Status" });

            foreach (DataRow dr in dtStatus.Rows)
            {
                ListItem li = new ListItem(dr["Status"].ToString(), dr["StatusID"].ToString());
                li.Selected = (SelectedStatuses.Count() == 0 || SelectedStatuses.Contains(dr["StatusID"].ToString()));

                //if (SelectedStatuses.Count() == 0 && (li.Text == "Approved/Closed" || li.Text == "Closed" || li.Text == "On Hold")) li.Selected = false;
                if (li.Selected)
                {
                    selected += li.Value + ",";
                }

                ddlStatus.Items.Add(li);
            }
            this.SelectedStatuses = Server.UrlDecode(selected.Trim()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        loadQF_Assigned();
    }

    private void loadQF_Assigned()
    {
        List<string> Assigned = new List<string>();
        bool blnBacklog = false;

        if (SelectedAssigned != null && SelectedAssigned.Length > 0)
        {
            foreach (string s in SelectedAssigned)
            {
                if (s == "69")
                {
                    blnBacklog = true;
                }
                Assigned.Add(s.Trim());
            }
        }

        DataTable dtAssigned = UserManagement.LoadUserList();  // userNameSearch: userName

        Label lblAssigned = (Label)Page.Master.FindControl("lblms_Item1");
        lblAssigned.Text = "Assigned To: ";
        lblAssigned.Style["width"] = "150px";

        HtmlSelect ddlAssigned = (HtmlSelect)Page.Master.FindControl("ms_Item1");

        if (dtAssigned != null)
        {
            ddlAssigned.Items.Clear();
            ListItem item = null;

            int userID = UserManagement.GetUserId_FromUsername();

            foreach (DataRow dr in dtAssigned.Rows)
            {
                if (dr["WTS_RESOURCEID"] == null || string.IsNullOrWhiteSpace(dr["WTS_RESOURCEID"].ToString()))
                {
                    continue;
                }
                else
                {
                    item = new ListItem(dr["UserName"].ToString(), dr["WTS_RESOURCEID"].ToString());

                    if (dr["RESOURCETYPE"].ToString() == "Not People")
                    {
                        item.Attributes.Add("OptionGroup", "Non-Resources");
                        item.Selected = false;
                    }
                    else
                    {
                        item.Attributes.Add("OptionGroup", "Resources");
                    }
                    //Use default values otherwise set the values from the qryString
                    if (ddlChanged == "no" && _myData)
                    {
                        item.Selected = true;
                        ddlAssigned.Items.Add(item);
                    }
                    else
                    {
                        item.Selected = ((Assigned.Count == 0 && item.Text != "IT.Backlog") || Assigned.Contains(dr["WTS_RESOURCEID"].ToString()));
                        ddlAssigned.Items.Add(item);
                    }
                }
            }
        }
    }

    private void InitializeEvents()
    {
        grdData.GridHeaderRowDataBound += grdData_GridHeaderRowDataBound;
        grdData.GridRowDataBound += grdData_GridRowDataBound;
        grdData.GridPageIndexChanging += grdData_GridPageIndexChanging;
    }
    #endregion

    #region Data
    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

        if (IsPostBack && Session["dtAORTask"] != null)
        {
            dt = (DataTable)Session["dtAORTask"];
        }
        else
        {

            HtmlSelect ddlStatus = (HtmlSelect)Page.Master.FindControl("ms_Item0");
            HtmlSelect ddlAssigned = (HtmlSelect)Page.Master.FindControl("ms_Item1");
            //bool Assigned = true;

            IList<string> listAssigned = new List<string>();

            if (ddlAssigned != null && ddlAssigned.Items.Count > 0)
            {
                foreach (ListItem li in ddlAssigned.Items)
                {
                    if (li.Selected)
                    {
                        listAssigned.Add(li.Value);
                    }
                }
            }

            IList<string> listStatuses = new List<string>();

            if (ddlStatus != null && ddlStatus.Items.Count > 0)
            {
                foreach (ListItem li in ddlStatus.Items)
                {
                    if (li.Selected)
                    {
                        listStatuses.Add(li.Value);
                    }
                }
            }

            dt = AOR.AORTaskSubTaskList_Get(AORID: this.AORID, AORReleaseID: this.AORReleaseID, WorkItemID: this.WorkItemID, SelectedStatuses: String.Join(",", listStatuses), SelectedAssigned: String.Join(",", listAssigned));

            if (this.CanEditAOR) dt.Columns.Add("X").SetOrdinal(1);

            Session["dtAORTask"] = dt;
        }

        if (dt != null && dt.Rows.Count > 0)
        {
            int subtasks = 0;
            foreach(DataRow dr in dt.Rows)
            {
                int.TryParse(dr["SubtaskCount"].ToString(), out subtasks);
                SubtaskCount += subtasks;
            }
        }

        return dt;
    }
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);

        if (this.CanEditAOR)
        {
            if (DCC.Contains("X"))
            {
                row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("X")].Controls.Add(CreateImage());
            }
        }
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatRowDisplay(ref row);

        if (DCC.Contains("AORReleaseTask_ID"))
        {
                row.Attributes.Add("aorreleasetask_id", row.Cells[DCC.IndexOf("AORReleaseTask_ID")].Text);
            
            if (this.CanViewWorkItem)
            {
                if (DCC.Contains("Work Task"))
                {
                    row.Cells[DCC.IndexOf("Work Task")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Work Task")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("Task_ID")].Text, row.Cells[DCC.IndexOf("Work Task")].Text));
                }

                if (DCC.Contains("Y"))
                {
                    row.Cells[DCC.IndexOf("Y")].Style["text-align"] = "center";
                    //add expand/collapse buttons
                    HtmlGenericControl divChildren = new HtmlGenericControl();
                    divChildren.Style["display"] = "table-row";
                    divChildren.Style["text-align"] = "right";
                    HtmlGenericControl divChildrenButtons = new HtmlGenericControl();
                    divChildrenButtons.Style["display"] = "table-cell";
                    divChildrenButtons.Controls.Add(createShowHideButton(true, "Show", row.Cells[DCC.IndexOf("Task_ID")].Text));
                    divChildrenButtons.Controls.Add(createShowHideButton(false, "Hide", row.Cells[DCC.IndexOf("Task_ID")].Text));
                    HtmlGenericControl divChildCount = new HtmlGenericControl();
                    divChildCount.InnerText = string.Format("({0})", row.Cells[DCC.IndexOf("SubtaskCount")].Text.ToString());
                    divChildCount.Style["display"] = "table-cell";
                    divChildCount.Style["padding-left"] = "2px";
                    divChildren.Controls.Add(divChildrenButtons);
                    divChildren.Controls.Add(divChildCount);
                    //buttons to show/hide child grid
                    row.Cells[DCC["Y"].Ordinal].Controls.Clear();
                    row.Cells[DCC["Y"].Ordinal].Controls.Add(divChildren);

                    //add child grid row for Task Items
                    Table table = (Table)row.Parent;
                    GridViewRow childRow = createChildRow(row.Cells[DCC.IndexOf("Task_ID")].Text);
                    table.Rows.AddAt(table.Rows.Count, childRow);
                }
            }

            if (this.CanEditAOR)
            {
                if (DCC.Contains("X"))
                {
                    row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("AORReleaseTask_ID")].Text, row.Cells[DCC.IndexOf("Work Task")].Text, row.Cells[DCC.IndexOf("CascadeAOR_ID")].Text));
                }
            }
        }

        if (DCC.Contains("Work Task")) row.Attributes.Add("task_id", row.Cells[DCC.IndexOf("Work Task")].Text);
    }

    private void grdData_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdData.PageIndex = e.NewPageIndex;
    }

    private void FormatHeaderRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
            if (DCC.Contains("SubtaskCount")) row.Cells[DCC.IndexOf("SubtaskCount")].Style["display"] = "none";
            if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["display"] = "none";
        }

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = "35px";
        }

        if (DCC.Contains("Y"))
        {
            row.Cells[DCC.IndexOf("Y")].Text = "";
            row.Cells[DCC.IndexOf("Y")].Style["width"] = "35px";
        }

        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["display"] = "none";

        if (DCC.Contains("Work Task")) row.Cells[DCC.IndexOf("Work Task")].Style["width"] = "75px";
        if (DCC.Contains("System(Task)")) row.Cells[DCC.IndexOf("System(Task)")].Style["width"] = "100px";
        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["width"] = "60px";
        if (DCC.Contains("Production Status")) row.Cells[DCC.IndexOf("Production Status")].Style["width"] = "75px";
        if (DCC.Contains("Priority")) row.Cells[DCC.IndexOf("Priority")].Style["width"] = "55px";
        if (DCC.Contains("SR Number")) row.Cells[DCC.IndexOf("SR Number")].Style["width"] = "55px";
        if (DCC.Contains("Assigned To")) row.Cells[DCC.IndexOf("Assigned To")].Style["width"] = "110px";
        if (DCC.Contains("Primary Resource")) row.Cells[DCC.IndexOf("Primary Resource")].Style["width"] = "110px";
        if (DCC.Contains("Secondary Tech. Resource")) row.Cells[DCC.IndexOf("Secondary Tech. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Primary Bus. Resource")) row.Cells[DCC.IndexOf("Primary Bus. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Secondary Bus. Resource")) row.Cells[DCC.IndexOf("Secondary Bus. Resource")].Style["width"] = "110px";
        if (DCC.Contains("Status")) row.Cells[DCC.IndexOf("Status")].Style["width"] = "100px";
        if (DCC.Contains("Percent Complete")) row.Cells[DCC.IndexOf("Percent Complete")].Style["width"] = "65px";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
            if (DCC.Contains("SubtaskCount")) row.Cells[DCC.IndexOf("SubtaskCount")].Style["display"] = "none";
            if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }

        if (DCC.Contains("AOR Name")) row.Cells[DCC.IndexOf("AOR Name")].Style["display"] = "none";

        if (DCC.Contains("Product Version")) row.Cells[DCC.IndexOf("Product Version")].Style["text-align"] = "center";
    }

    private Image CreateImage()
    {
        Image img = new Image();

        img.Attributes["src"] = "Images/Icons/help.png";
        img.Attributes["title"] = "These checkboxes are used to select multiple tasks to disassociate from this AOR";
        img.Attributes["alt"] = "These checkboxes are used to select multiple tasks to disassociate from this AOR";
        img.Attributes["onclick"] = "MessageBox('These checkboxes are used to select multiple tasks to disassociate from this AOR.')";
        img.Attributes["height"] = "12";
        img.Attributes["width"] = "12";
        img.Style["cursor"] = "pointer";

        return img;
    }

    private CheckBox CreateCheckBox(string value, string workitem, string cascadeAOR)
    {
        CheckBox chk = new CheckBox();

        chk.Attributes.Add("aorreleasetask_id", value);
        chk.Attributes.Add("workitem", workitem);
        if ((cascadeAOR == "1" && this.ReleaseAOR == false) || (this.ReleaseAOR == true && workitem.Contains(" - ")))
        {
            chk.Enabled = false;
        }

        return chk;
    }

    private LinkButton CreateLink(string workItem_ID, string workItem)
    {
        LinkButton lb = new LinkButton();
        string workItemID = workItem_ID;
        string taskNumber = string.Empty;
        string taskID = string.Empty;
        string blnSubTask = "0";

        lb.Text = workItem;
        if (workItem.Contains(" - "))
        {
            string[] arrWorkItem = workItem.Split('-');

            workItemID = arrWorkItem[0].Trim();
            taskNumber = arrWorkItem[1].Trim();
            taskID = workItem_ID;
            blnSubTask = "1";
            lb.Attributes["onclick"] = string.Format("openTask('{0}', '{1}', '{2}', {3}); return false;", workItemID, taskNumber, taskID, blnSubTask);
        }
        else
        {

            lb.Attributes["onclick"] = string.Format("openTask('{0}', '{1}', '{2}', {3}); return false;", workItemID, taskNumber, taskID, blnSubTask);
        }
        return lb;
    }

    Image createShowHideButton(bool show = false, string direction = "Show", string itemId = "ALL")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("imgShowHideChildren_click(this,'{0}','{1}');", direction, itemId);

        Image img = new Image();
        img.ID = string.Format("img{0}Children_{1}", direction, itemId);
        img.Style["display"] = show ? "block" : "none";
        img.Style["cursor"] = "pointer";
        img.Attributes.Add("Name", string.Format("img{0}", direction));
        img.Attributes.Add("itemId", itemId);
        img.Height = 10;
        img.Width = 10;
        img.ImageUrl = direction.ToUpper() == "SHOW"
            ? "Images/Icons/add_blue.png"
            : "Images/Icons/minus_blue.png";
        img.ToolTip = string.Format("{0} Items for [{1}]", direction, itemId);
        img.AlternateText = string.Format("{0} Items for [{1}]", direction, itemId);
        img.Attributes.Add("Onclick", sb.ToString());

        return img;
    }

    GridViewRow createChildRow(string itemId = "")
    {
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Selected);
        TableCell tableCell = null;

        try
        {
            row.CssClass = "gridBody";
            row.Style["display"] = "none";
            row.ID = string.Format("gridChild_{0}", itemId);
            row.Attributes.Add("Task_ID", itemId);
            row.Attributes.Add("Name", string.Format("gridChild_{0}", itemId));

            //add the table cells
            for (int i = 0; i < DCC.Count; i++)
            {
                tableCell = new TableCell();
                tableCell.Text = "&nbsp;";

                if (i == DCC["Y"].Ordinal)
                {
                    //set width to match parent
                    tableCell.Style["width"] = "32px";
                    tableCell.Style["border-right"] = "1px solid transparent";
                }
                else if (i == DCC["Task_ID"].Ordinal)
                {
                    tableCell.Style["padding-top"] = "10px";
                    tableCell.Style["padding-right"] = "10px";
                    tableCell.Style["padding-bottom"] = "0px";
                    tableCell.Style["padding-left"] = "0px";
                    tableCell.Style["vertical-align"] = "top";
                    tableCell.ColumnSpan = DCC.Count - 1;
                    //add the frame here
                    tableCell.Controls.Add(createChildFrame(itemId: itemId));
                }
                else
                {
                    tableCell.Style["display"] = "none";
                }

                row.Cells.Add(tableCell);
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            row = null;
        }

        return row;
    }

    HtmlIframe createChildFrame(string itemId = "")
    {
        HtmlIframe childFrame = new HtmlIframe();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        childFrame.ID = string.Format("frameChild_{0}", itemId);
        childFrame.Attributes.Add("Task_ID", itemId);
        childFrame.Attributes["frameborder"] = "0";
        childFrame.Attributes["scrolling"] = "no";
        childFrame.Attributes["src"] = "javascript:''";
        childFrame.Style["height"] = "30px";
        childFrame.Style["width"] = "100%";
        childFrame.Style["border"] = "none";

        return childFrame;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string DeleteAORTask(string releaseTask, string ReleaseAOR)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            DataTable dtReleaseTask = (DataTable)JsonConvert.DeserializeObject(releaseTask, (typeof(DataTable)));

            foreach (DataRow dr in dtReleaseTask.Rows)
            {
                int AORReleaseTask_ID = 0;
                bool releaseAOR = false;
                string workItem = dr["workitem"].ToString();
                int.TryParse(dr["aorreleasetaskid"].ToString(), out AORReleaseTask_ID);
                bool.TryParse(ReleaseAOR, out releaseAOR);
                if (!workItem.Contains(" - ") || (dtReleaseTask.Rows.IndexOf(dr) == dtReleaseTask.Rows.Count - 1 && releaseAOR))
                {
                    if (workItem.Contains(" - ") && dtReleaseTask.Rows.IndexOf(dr) == dtReleaseTask.Rows.Count - 1 && releaseAOR)  // Parent Task isn't selected due to pagination
                    {
                        int workItemID = 0;
                        int.TryParse(workItem.Substring(0, workItem.IndexOf(" - ")), out workItemID);
                        AORReleaseTask_ID = AOR.AORReleaseTask_Get(workItemID);
                    }
                    deleted = AOR.AORTask_Delete(AORReleaseTaskID: AORReleaseTask_ID, ReleaseAOR: releaseAOR); // The rest of the SubTasks will be removed as well
                }
                else if (workItem.Contains(" - "))
                {
                    deleted = AOR.AORSubTask_Delete(AORReleaseSubTaskID: AORReleaseTask_ID);
                }
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