using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;

using Newtonsoft.Json;


public partial class Admin_Organization_Grid : System.Web.UI.Page
{
    protected enum ColIdx
    {
        Id = 0,
        User_Type,
        DefaultRoles,
        Users,
        Description,
        Archive,
        CreatedBy,
        CreatedDate,
        UpdatedBy,
        UpdatedDate
    }

    protected bool AllowEdit = false;
    protected bool AllowDelete = false;

    protected bool ShowArchived = false;

	private Dictionary<string, string> _sortFieldsCollection = new Dictionary<string, string>();
	protected string SortFields = string.Empty;

	protected string SelectedSortFieldName = "ORGANIZATION";
	protected string SelectedSortDirection = "ASC";

    protected WTS_User _loggedInUser;
    protected MembershipUser _loggedInMembershipUser;

    #region Events
    protected void Page_PreInit(object sender, EventArgs e)
    {
        //load theme for user
		Page.Theme = WTSUtility.ThemeName;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        bool export = false;

		#region QueryString
		if (Request.QueryString["SortField"] != null &&
			!string.IsNullOrWhiteSpace(Request.QueryString["SortField"]))
		{
			this.SelectedSortFieldName = Server.UrlDecode(Request.QueryString["SortField"]);
		}
		if (Request.QueryString["SortDirection"] != null &&
			!string.IsNullOrWhiteSpace(Request.QueryString["SortDirection"]))
		{
			this.SelectedSortDirection = Server.UrlDecode(Request.QueryString["SortDirection"]);
		}
        if (Request.QueryString["ShowArchived"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"]), out ShowArchived);
        }

        if (Request.QueryString["Export"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out export);
        }
        #endregion
        

        CheckRoles();
        initControls();
        LoadData(export: export);
    }

    private void LoadData(bool export = false)
    {
        DataTable dt = UserManagement.LoadOrganizationList(includeArchive: export ? this.ShowArchived : true);

        Session["dtOrganizations"] = dt;

        if (export)
        {
            ExportExcel(dt);
        }
        else
        {
            gridOrganizations.DataSource = dt;
            gridOrganizations.DataBind();
        }
    }

    private void initControls()
    {
        gridOrganizations.GridHeaderRowDataBound += gridOrganizations_GridHeaderRowDataBound;
        gridOrganizations.GridRowDataBound += gridOrganizations_GridRowDataBound;
        gridOrganizations.GridPageIndexChanging += gridOrganizations_GridPageIndexChanging;
    }

    #region Grid
    void gridOrganizations_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gridOrganizations.PageIndex = e.NewPageIndex;
        //rebind the data
        if (Session["dtOrganizations"] == null)
        {
            LoadData();
        }
        else
        {
            gridOrganizations.DataSource = (DataTable)Session["dtOrganizations"];
            gridOrganizations.DataBind();
        }
    }

    void gridOrganizations_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        row.Attributes.Add("OrganizationId", row.Cells[(int)ColIdx.Id].Text.Trim());
        if (e.Row.Cells[(int)ColIdx.Archive].HasControls())
        {
            CheckBox vCheckBox = (CheckBox)e.Row.Cells[(int)ColIdx.Archive].Controls[0];
            row.Attributes.Add("archived", vCheckBox.Checked.ToString());
            if (vCheckBox.Checked)
            {
                row.CssClass += " gridArchivedRow";
                row.Font.Italic = true;
                if (!ShowArchived)
                {
                    row.Style["display"] = "none";
                }
            }
            else
            {
                row.CssClass = row.CssClass.Replace("gridArchivedRow", "");
                row.Font.Italic = false;
            }
        }
        else
        {
            row.Attributes.Add("archived", false.ToString());
        }

        formatUsers(ref row);
        formatDefaultRoles(ref row);
        formatUpdatedBy(ref row);
        formatColumnDisplay(ref row);

        row.Cells[(int)ColIdx.User_Type].Style["padding-left"] = "8px";
    }

    void gridOrganizations_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);
        //rename headers
        row.Cells[(int)ColIdx.User_Type].Text = "Organization";
        row.Cells[(int)ColIdx.DefaultRoles].Text = "Default Roles";
        row.Cells[(int)ColIdx.Description].Text = "Description";
        row.Cells[(int)ColIdx.UpdatedBy].Text = "Updated";

        row.Cells[(int)ColIdx.Description].HorizontalAlign = HorizontalAlign.Left;
    }

    private void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (i != (int)ColIdx.User_Type
                && i != (int)ColIdx.Description
                && i != (int)ColIdx.DefaultRoles
                && i != (int)ColIdx.Users
                && i != (int)ColIdx.UpdatedBy)
            {
                row.Cells[i].Style["display"] = "none";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(row.Cells[i].Text))
                {
                    row.Cells[i].Text = "&nbsp;";
                }
            }
        }

        //size columns
        row.Cells[(int)ColIdx.User_Type].Style["width"] = "100px";
		row.Cells[(int)ColIdx.DefaultRoles].Style["width"] = "100px";
		row.Cells[(int)ColIdx.Users].Style["width"] = "50px";
		row.Cells[(int)ColIdx.UpdatedBy].Style["width"] = "125px";

        //align columns
        row.Cells[(int)ColIdx.DefaultRoles].Style["text-align"] = "center";
		row.Cells[(int)ColIdx.Users].Style["text-align"] = "center";
		row.Cells[(int)ColIdx.UpdatedBy].Style["text-align"] = "center";
    }

    private void formatDefaultRoles(ref GridViewRow row)
    {
        string roles = row.Cells[(int)ColIdx.DefaultRoles].Text.Replace("&nbsp;", "").Trim();
        string tooltip = string.Format("No default roles have been configured for the {0} Organization.",
            row.Cells[(int)ColIdx.User_Type].Text.Trim());
        int count = roles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
        if (count > 0)
        {
            tooltip = roles.Replace(",", Environment.NewLine);
        }
        roles = count.ToString();
        row.Cells[(int)ColIdx.DefaultRoles].Text = roles;
        row.Cells[(int)ColIdx.DefaultRoles].ToolTip = tooltip;
    }

    private void formatUsers(ref GridViewRow row)
    {
        string users = row.Cells[(int)ColIdx.Users].Text.Replace("&nbsp;", "").Trim();
        string tooltip = string.Format("No users have been assigned the {0} Organization.",
            row.Cells[(int)ColIdx.User_Type].Text.Trim());
        int count = users.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
        if (count > 0)
        {
            tooltip = users.Replace(",", Environment.NewLine);
        }
        users = count.ToString();
        row.Cells[(int)ColIdx.Users].Text = users;
        row.Cells[(int)ColIdx.Users].ToolTip = tooltip;
    }

    private void formatUpdatedBy(ref GridViewRow row)
    {
        string updatedBy = string.Empty, updatedDate = string.Empty, newText = string.Empty;
        DateTime ud;

        updatedBy = row.Cells[(int)ColIdx.UpdatedBy].Text.Replace("&nbsp;", "").Trim();
        updatedDate = row.Cells[(int)ColIdx.UpdatedDate].Text.Replace("&nbsp;", "").Trim();
        if (DateTime.TryParse(updatedDate, out ud))
        {
            updatedDate = ud.ToShortDateString();
        }
        else
        {
            updatedDate = string.Empty;
        }

        newText = string.Format("{0}<br />{1}", updatedBy, updatedDate);

        row.Cells[(int)ColIdx.UpdatedBy].Text = newText;
    }
    #endregion Grid

    #endregion

    [WebMethod(true)]
    public static string DeleteOrganization(int organizationId = 0)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { 
			{ "Exists", "" }
			, { "HasDependencies", "" }
			, { "Deleted", "" }
			, { "Archived", "" }
			, { "Error", "" } };
        bool exists = false, hasDependencies = false, deleted = false, archived = false;
        string errorMsg = string.Empty;

        Organization org = null;

		if (!validateID(organizationId, out org, out errorMsg))
        {
			exists = false;
			hasDependencies = false;
			deleted = false;
			archived = false;
        }
        else
        {
            try
            {
				deleted = org.Delete(out exists, out hasDependencies, out archived);
            }
            catch (Exception ex)
            {
				LogUtility.LogException(ex);
				exists = false;
				hasDependencies = false;
				deleted = false;
				archived = false;
				errorMsg = ex.Message;
            }
        }

		result["Exists"] = exists.ToString();
		result["HasDependencies"] = hasDependencies.ToString();
		result["Deleted"] = deleted.ToString();
		result["Archived"] = archived.ToString();
		result["Error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
    }

    private static bool validateID(int organizationId, out Organization org, out string errorMsg)
    {
		org = null;
        errorMsg = string.Empty;

		if (organizationId == 0)
        {
            errorMsg = "Missing/Invalid Organization ID specified";
            return false;
        }
        else
        {
			org = new Organization(organizationId);
			org.Load();
			if (org == null || org.ID == 0)
            {
                errorMsg = "Organization does not exist with specified id";
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private void CheckRoles()
    {
        #region Logged In User details

        _loggedInMembershipUser = Membership.GetUser();
        _loggedInUser = new WTS_User();
        _loggedInUser.Load(_loggedInMembershipUser.ProviderUserKey.ToString());

        #endregion Logged In User details

        //enable/disable buttons
        if (_loggedInUser.Organization.StartsWith("admin", StringComparison.CurrentCultureIgnoreCase)
            || User.IsInRole("Admin")
			|| User.IsInRole("Administration")
			|| User.IsInRole("ResourceManagement"))
        {
            this.AllowEdit = true;
            this.AllowDelete = true;
        }
    }

    private void FormatExcelColumnValues(ref DataTable dt)
    {
        string[] roles, users;
        foreach (DataColumn dc in dt.Columns)
        {
            dc.ReadOnly = false;
        }

        char[] delims = new char[] { ',' };
        foreach (DataRow row in dt.Rows)
        {
            roles = row["DefaultRoles"].ToString().Split(delims, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < roles.Length; i++)
            {
                string role = roles[i].Trim();
                role = role.Substring(role.IndexOf(':') + 1);
                roles[i] = role.Trim();
            }

            row["DefaultRoles"] = string.Join(", ", roles);
        }

        foreach (DataRow row in dt.Rows)
        {
            users = row["Users"].ToString().Split(delims, StringSplitOptions.RemoveEmptyEntries);
            row["Users"] = users.Length > 0 ? users.Length.ToString() : "";
        }
    }
    private void RemoveExcelColumns(ref DataTable dt)
    {
        try
        {
            //this has to be done in reverse order (RemoveAt) 
            //OR by name(Remove) or it will have undesired result
            dt.Columns.Remove("UpdatedDate");
            dt.Columns.Remove("UpdatedBy");
            dt.Columns.Remove("CreatedDate");
            dt.Columns.Remove("CreatedBy");
            dt.Columns.Remove("Id");

            dt.AcceptChanges();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }
    private void RenameExcelColumns(ref DataTable dt)
    {
        if (dt.Columns.Contains("User_Type"))
        {
            dt.Columns["User_Type"].ColumnName = "Organization";
        }
        if (dt.Columns.Contains("DefaultRoles"))
        {
            dt.Columns["DefaultRoles"].ColumnName = "Default Roles";
        }
        if (dt.Columns.Contains("User_Type_Descr"))
        {
            dt.Columns["User_Type_Descr"].ColumnName = "Description";
        }
        if (dt.Columns.Contains("CreatedDate"))
        {
            dt.Columns["CreatedDate"].ColumnName = "Created";
        }
        if (dt.Columns.Contains("UpdatedBy"))
        {
            dt.Columns["UpdatedBy"].ColumnName = "Updated";
        }

        dt.AcceptChanges();
    }

    private bool ExportExcel(DataTable dt)
    {
        bool success = false;
        string errorMsg = string.Empty;

        try
        {
            FormatExcelColumnValues(ref dt);
            RemoveExcelColumns(ref dt);
            RenameExcelColumns(ref dt);

            string name = "Organizations";
            Workbook wb = new Workbook(FileFormatType.Xlsx);
            Worksheet ws = wb.Worksheets[0];
            ws.Cells.ImportDataTable(dt, true, 0, 0, false, false);

			//WTSUtility.FormatWorkbookHeader(ref wb, ref ws, 0, 0, 1, dt.Columns.Count);
            
            ws.AutoFitColumns();
            MemoryStream ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Xlsx);

            Response.ContentType = "application/xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
            Response.BinaryWrite(ms.ToArray());
            Response.End();

            success = true;
        }
        catch (Exception ex)
        {
            success = false;
            errorMsg += Environment.NewLine + ex.Message;
        }

        return success;
    }
}