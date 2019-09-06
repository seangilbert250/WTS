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


public partial class User_Grid : System.Web.UI.Page
{
	protected DataColumnCollection DCC;
	protected GridCols ColumnData = new GridCols();
	protected string SortableColumns;
	protected string SortOrder;
	protected string DefaultColumnOrder;
	protected string SelectedColumnOrder;
	protected string ColumnOrder;

	private bool _export = false;

	protected bool CanView = false;
	protected bool CanEdit = false;

    protected bool AllowEdit = false;
    protected bool AllowDelete = false;

    protected bool ShowArchived = false;

    private Dictionary<string, string> _sortFieldsCollection = new Dictionary<string, string>();
    protected string SortFields = string.Empty;

    protected string SelectedSortFieldName = "Part";
    protected string SelectedSortDirection = "asc";
	protected int OrganizationID = 0;
    protected string UserNameSearch = string.Empty;

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
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.ResourceAdmin);
		this.CanView = CanEdit || UserManagement.UserCanView(WTSModuleOption.ResourceAdmin);

        readQueryString();

		#region Logged In User details

		_loggedInMembershipUser = Membership.GetUser();

		_loggedInUser = new WTS_User();
		_loggedInUser.Load(_loggedInMembershipUser.ProviderUserKey.ToString());

		#endregion Logged In User details
        
		//CheckRoles();
		this.AllowEdit = true;
		this.AllowDelete = true;
        initControls();
        LoadData(export: _export);
    }
	private void readQueryString()
	{
		if (Request.QueryString["sortOrder"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"].ToString()))
		{
			this.SortOrder = Server.UrlDecode(Request.QueryString["sortOrder"]);
		}
		if (Request.QueryString["OrganizationID"] != null &&
			!string.IsNullOrWhiteSpace(Request.QueryString["OrganizationID"]))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["OrganizationID"]), out this.OrganizationID);
		}
		if (Request.QueryString["UserNameSearch"] != null &&
			!string.IsNullOrWhiteSpace(Request.QueryString["UserNameSearch"]))
		{
			this.UserNameSearch = Server.UrlDecode(Request.QueryString["UserNameSearch"]);
		}

		if (Request.QueryString["ShowArchived"] != null &&
			!string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"]))
		{
			bool.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"]), out ShowArchived);
		}

		if (Request.QueryString["Export"] != null &&
			!string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
		{
			bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);
		}
	}

    private void LoadData(bool export = false)
    {
        DataTable dt = UserManagement.LoadUserList(organizationId: this.OrganizationID
			, loadArchived: export ? this.ShowArchived : true
            , userNameSearch: this.UserNameSearch);
        
		if (dt != null)
        {
            dt = dt.DefaultView.ToTable();
			DCC = dt.Columns;

			spanRowCount.InnerText = dt.Rows.Count.ToString();

			InitializeColumnData(ref dt);
			dt.AcceptChanges();
        }

        Session["dtUsers"] = dt;

        if (export)
        {
            ExportExcel(dt);
        }
        else
        {
            gridUsers.DataSource = dt;
            gridUsers.DataBind();
        }
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
					case "WTS_RESOURCEID":
						blnVisible = false;
						blnSortable = false;
						blnOrderable = false;
						break;
					case "Membership_UserId":
						blnVisible = false;
						blnSortable = false;
						blnOrderable = false;
						break;
					case "IsRegistered":
						displayName = "Registered";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "IsApproved":
						displayName = "Approved";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "IsLockedOut":
						displayName = "Locked";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "UserName":
						displayName = "Username";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "First_Name":
						displayName = "First Name";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "Last_Name":
						displayName = "Last Name";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "ORGANIZATION":
						displayName = "Organization";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "Roles":
						displayName = "Roles";
						blnVisible = true;
						blnSortable = false;
						blnOrderable = false;
						break;
					case "Phone_Office":
						displayName = "Phone";
						blnVisible = true;
						blnSortable = false;
						blnOrderable = false;
						break;
					case "Phone_Mobile":
						displayName = "Mobile";
						blnVisible = false;
						blnSortable = false;
						blnOrderable = false;
						break;
					case "State":
						displayName = "Office Location";
						blnVisible = true;
						blnSortable = true;
						blnOrderable = false;
						break;
					case "Email":
						displayName = "Email";
						blnVisible = true;
						blnSortable = false;
						blnOrderable = false;
						break;

                    //case "ReceiveSREMail":
                    //    displayName = "Recieve SR Report EMail";
                    //    blnVisible = true;
                    //    blnSortable = false;
                    //    blnOrderable = false;
                    //    break;
                    case "IncludeInSRCounts":
                        displayName = "Include in SR Counts";
                        blnVisible = true;
                        blnSortable = false;
                        blnOrderable = false;
                        break;
                }

                ColumnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
				ColumnData.Columns.Item(ColumnData.Columns.Count - 1).CanReorder = blnOrderable;
			}

			//Initialize the columnData
			ColumnData.Initialize(ref dt, ";", "~", "|");

			//Get sortable columns and default column order
			SortableColumns = ColumnData.SortableColumnsToString();
			DefaultColumnOrder = ColumnData.DefaultColumnOrderToString();
			//Sort and Reorder Columns
			ColumnData.ReorderDataTable(ref dt, ColumnOrder);
			ColumnData.SortDataTable(ref dt, SortOrder);
			SelectedColumnOrder = ColumnData.CurrentColumnOrderToString();

		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
		}
	}


    private void initControls()
    {
        #region Sort options
        //available sort options
        _sortFieldsCollection = new Dictionary<string, string>();
        _sortFieldsCollection.Add("Username", "UserName");
		_sortFieldsCollection.Add("Organization", "Organization");
        _sortFieldsCollection.Add("First Name", "First_Name");
        _sortFieldsCollection.Add("Last Name", "Last_Name");

		this.SortFields = "Username,Organization,First Name,Last Name";

        this.txtSortField.Value = this.SelectedSortFieldName;
        this.txtSortDirection.Value = this.SelectedSortDirection;

        #endregion

        #region Filter selections
        //Organization filter
        DataTable dtOrgs = UserManagement.LoadOrganizationList(includeArchive: true);

        ddlOrganizationFilter.DataSource = dtOrgs;
		ddlOrganizationFilter.DataTextField = "Organization";
		ddlOrganizationFilter.DataValueField = "OrganizationId";
        ddlOrganizationFilter.DataBind();

        ddlOrganizationFilter.Items.Insert(0, new ListItem("All Organizations", "0"));
        ListItem item = ddlOrganizationFilter.Items.FindByValue(this.OrganizationID.ToString());
        if (item != null)
        {
            item.Selected = true;
        }

        //user name search
        this.txtUserNameSearch.Text = this.UserNameSearch.Trim();

        #endregion

        gridUsers.GridHeaderRowDataBound += gridUsers_GridHeaderRowDataBound;
        gridUsers.GridRowDataBound += gridUsers_GridRowDataBound;
        gridUsers.GridPageIndexChanging += gridUsers_GridPageIndexChanging;
    }

    void gridUsers_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gridUsers.PageIndex = e.NewPageIndex;
        //rebind the data
        if (Session["dtUsers"] == null)
        {
            LoadData();
        }
        else
        {
            gridUsers.DataSource = (DataTable)Session["dtUsers"];
        }
    }

    void gridUsers_GridRowDataBound(object sender, GridViewRowEventArgs e)
	{
		ColumnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        row.Attributes.Add("userId", row.Cells[DCC["WTS_RESOURCEID"].Ordinal].Text.Trim());
        if (e.Row.Cells[DCC["Archive"].Ordinal].HasControls())
        {
            CheckBox vCheckBox = (CheckBox)e.Row.Cells[DCC["Archive"].Ordinal].Controls[0];
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
            row.Attributes.Add("Archived", false.ToString());
        }

        for (int i = 0; i < row.Cells.Count; i++)
        {
            row.Cells[i].VerticalAlign = VerticalAlign.Top;
        }

        if (string.IsNullOrWhiteSpace(row.Cells[DCC["Notes"].Ordinal].Text.Replace("&nbsp;", "")))
        {
			row.Cells[DCC["Notes"].Ordinal].Text = "&nbsp;";
        }

		row.Cells[DCC["ORGANIZATION"].Ordinal].Text = row.Cells[DCC["ORGANIZATION"].Ordinal].Text.Replace('_', ' ');

		if (CanEdit || CanView)
		{
			row.Cells[DCC.IndexOf("UserName")].Controls.Add(createEditLink(row.Cells[DCC["WTS_RESOURCEID"].Ordinal].Text, row.Cells[DCC.IndexOf("UserName")].Text));
		}
        formatRoles(ref row);
        formatEmail(ref row);
    }

    void gridUsers_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
	{
		ColumnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);
    }

    private void formatColumnDisplay(ref GridViewRow row)
    {
		for (int i = 0; i < row.Cells.Count; i++)
		{
			if (i != DCC["IsRegistered"].Ordinal
				&& i != DCC["IsApproved"].Ordinal
				&& i != DCC["IsLockedOut"].Ordinal
				&& i != DCC["Phone_Office"].Ordinal
				&& i != DCC["Phone_Mobile"].Ordinal
				&& i != DCC["State"].Ordinal)
			{
				row.Cells[i].Style["text-align"] = "left";
				row.Cells[i].Style["padding-left"] = "5px";
			}
			else
			{
				row.Cells[i].Style["text-align"] = "center";
			}
		}

		if (row.RowType == DataControlRowType.DataRow)
        {
            if(!string.IsNullOrWhiteSpace(row.Cells[DCC["Membership_UserId"].Ordinal].Text.Replace("&nbsp;",""))
                && row.Cells[DCC["Membership_UserId"].Ordinal].Text != Guid.Empty.ToString())
            {
				row.Cells[DCC["IsRegistered"].Ordinal].ToolTip = string.Format("Membership User ID: {0}", row.Cells[DCC["Membership_UserId"].Ordinal].Text.Trim());
            }
        }

        //Size columns
		row.Cells[DCC["IsRegistered"].Ordinal].Style["width"] = "70px";
		row.Cells[DCC["IsApproved"].Ordinal].Style["width"] = "65px";
		row.Cells[DCC["IsLockedOut"].Ordinal].Style["width"] = "50px";
		row.Cells[DCC["UserName"].Ordinal].Style["width"] = "135px";
		row.Cells[DCC["ORGANIZATION"].Ordinal].Style["width"] = "90px";
		row.Cells[DCC["First_Name"].Ordinal].Style["width"] = "90px";
		row.Cells[DCC["Last_Name"].Ordinal].Style["width"] = "100px";
		row.Cells[DCC["Phone_Office"].Ordinal].Style["width"] = "100px";
		row.Cells[DCC["Phone_Mobile"].Ordinal].Style["width"] = "100px";
		row.Cells[DCC["Email"].Ordinal].Style["width"] = "90px";
		row.Cells[DCC["State"].Ordinal].Style["width"] = "95px";
    }

    private void formatRoles(ref GridViewRow row)
    {
        string tooltip = string.Format("No roles have been assigned for {0} {1}.",
            row.Cells[DCC["First_Name"].Ordinal].Text.Trim(),
            row.Cells[DCC["Last_Name"].Ordinal].Text.Trim());
        if (row.Cells[DCC["Roles"].Ordinal].Text.Replace("&nbsp;", "").Trim().Length > 0)
        {
			string[] roles = row.Cells[DCC["Roles"].Ordinal].Text.Replace("&nbsp;", "").Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sbRoles = new StringBuilder();
            foreach (string r in roles)
            {
                string id = string.Empty, name = string.Empty;
                string[] role = r.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                id = role[0];
                name = role[1];

                if (sbRoles.Length > 0)
                {
                    sbRoles.Append("<br />");
                }
                sbRoles.AppendLine("* " + name);
            }
			row.Cells[DCC["Roles"].Ordinal].Text = sbRoles.ToString();
        }
        else
        {
			row.Cells[DCC["Roles"].Ordinal].ToolTip = tooltip;
        }
    }

    private void formatEmail(ref GridViewRow row)
    {
        string email = row.Cells[DCC["Email"].Ordinal].Text.Trim().Replace("&nbsp;", "");
        if (!string.IsNullOrWhiteSpace(email))
        {
            HtmlGenericControl link = new HtmlGenericControl();
            link.InnerHtml = string.Format("<a href='mailto:{0}' title='Contact Email - {0}'>{0}</a>", email);
			row.Cells[DCC["Email"].Ordinal].Controls.Add(link);
        }
    }

	LinkButton createEditLink(string id = "", string name = "")
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("lbEdit_click('{0}');return false;", id);

		LinkButton lb = new LinkButton();
		lb.ID = string.Format("lbEdit_{0}", id);
		lb.Attributes["name"] = string.Format("lbEdit_{0}", id);
		lb.ToolTip = string.Format("View/Edit User [{0}]", name);
		lb.Text = name;
		lb.Attributes.Add("Onclick", sb.ToString());

		return lb;
	}

    #endregion Events

	[WebMethod(true)]
	public static string DeleteUser(int userID)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { 
			{ "Exists", "" }
			, { "HasDependencies", "" }
			, { "Deleted", "" }
			, { "Archived", "" }
			, { "Error", "" } };
		bool exists = false, hasDependencies = false, deleted = false, archived = false;
		string errorMsg = string.Empty;

		WTS_User u = null;
		if (!validateUserID(userID, out u, out errorMsg))
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
				bool membershipDeleted = false;
				deleted = u.Delete(out exists, out hasDependencies, out archived, out membershipDeleted);
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

	private static bool validateUserID(int userID, out WTS_User u, out string errorMsg)
	{
		u = null;
		errorMsg = string.Empty;

		if (userID == 0)
		{
			errorMsg = "Invalid user id specified";
			return false;
		}
		else
		{
			u = new WTS_User(userID);
			u.Load();
			if (u == null || u.ID == 0)
			{
				errorMsg = "User does not exist with specified id";
				return false;
			}
			else
			{
				return true;
			}
		}
	}

    private static bool validateUserIdString(string UserId, out WTS_User u, out string errorMsg)
    {
        u = null;
        errorMsg = string.Empty;

        int id = 0;
        int.TryParse(UserId, out id);
        if (id == 0)
        {
            errorMsg = "Invalid user id specified";
            return false;
        }
        else
        {
            u = new WTS_User(id);
            u.Load();
            if (u == null || u.ID == 0)
            {
                errorMsg = "User does not exist with specified id";
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
        List<string> userRoles = UserManagement.GetUserRoles_All();

        if (UserManagement.UserIsUserAdmin(_loggedInUser.ID))
        {
            this.AllowEdit = true;
            this.AllowDelete = true;
        }
    }

    private void FormatExcelColumnValues(ref DataTable dt)
    {
        string[] roles;
        foreach (DataColumn dc in dt.Columns)
        {
            dc.ReadOnly = false;
        }

        char[] delims = new char[] { ',' };
        foreach (DataRow row in dt.Rows)
        {
            roles = row["roles"].ToString().Split(delims, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < roles.Length; i++)
            {
                string role = roles[i].Trim();
                role = role.Substring(role.IndexOf(':') + 1);
                roles[i] = role.Trim();
            }

            row["roles"] = string.Join(", ", roles);
        }
    }
    private void RemoveExcelColumns(ref DataTable dt)
    {
        try
        {
            //this has to be done in reverse order (RemoveAt) 
            //OR by name(Remove) or it will have undesired result
			if (dt.Columns.Contains("UpdatedDate"))
			{
				dt.Columns.Remove("UpdatedDate");
			}
			if (dt.Columns.Contains("UpdatedBy"))
			{
				dt.Columns.Remove("UpdatedBy");
			}
			if (dt.Columns.Contains("CreatedDate"))
			{
				dt.Columns.Remove("CreatedDate");
			}
			if (dt.Columns.Contains("CreatedBy"))
			{
				dt.Columns.Remove("CreatedBy");
			}
			if (dt.Columns.Contains("PostalCode"))
			{
				dt.Columns.Remove("PostalCode");
			}
			if (dt.Columns.Contains("Country"))
			{
				dt.Columns.Remove("Country");
			}
			if (dt.Columns.Contains("State"))
			{
				dt.Columns.Remove("State");
			}
			if (dt.Columns.Contains("City"))
			{
				dt.Columns.Remove("City");
			}
			if (dt.Columns.Contains("Address2"))
			{
				dt.Columns.Remove("Address2");
			}
			if (dt.Columns.Contains("Address"))
			{
				dt.Columns.Remove("Address");
			}
			if (dt.Columns.Contains("Email2"))
			{
				dt.Columns.Remove("Email2");
			}
			if (dt.Columns.Contains("Fax"))
			{
				dt.Columns.Remove("Fax");
			}
			if (dt.Columns.Contains("Phone_Misc"))
			{
				dt.Columns.Remove("Phone_Misc");
			}
			if (dt.Columns.Contains("Suffix"))
			{
				dt.Columns.Remove("Suffix");
			}
			if (dt.Columns.Contains("Prefix"))
			{
				dt.Columns.Remove("Prefix");
			}
			if (dt.Columns.Contains("Middle_Name"))
			{
				dt.Columns.Remove("Middle_Name");
			}
			if (dt.Columns.Contains("Name"))
			{
				dt.Columns.Remove("Name");
			}
			if (dt.Columns.Contains("ThemeId"))
			{
				dt.Columns.Remove("ThemeId");
			}
			if (dt.Columns.Contains("User_TypeID"))
			{
				dt.Columns.Remove("User_TypeID");
			}
			if (dt.Columns.Contains("Membership_UserId"))
			{
				dt.Columns.Remove("Membership_UserId");
			}
			if (dt.Columns.Contains("Id"))
			{
				dt.Columns.Remove("Id");
			}
            if (dt.Columns.Contains("ReceiveSREMail"))
            {
                dt.Columns.Remove("ReceiveSREMail");
            }

            dt.AcceptChanges();
        }
        catch (Exception ex)
        {
			LogUtility.LogException(ex);
        }
    }
    private void RenameExcelColumns(ref DataTable dt)
    {
        if (dt.Columns.Contains("IsRegistered"))
        {
            dt.Columns["IsRegistered"].ColumnName = "Registered";
        }
        if (dt.Columns.Contains("IsApproved"))
        {
            dt.Columns["IsApproved"].ColumnName = "Approved";
        }
        if (dt.Columns.Contains("IsLockedOut"))
        {
            dt.Columns["IsLockedOut"].ColumnName = "Locked";
        }
        if (dt.Columns.Contains("UserName"))
        {
            dt.Columns["UserName"].ColumnName = "Username";
        }
        if (dt.Columns.Contains("User_Type"))
        {
            dt.Columns["User_Type"].ColumnName = "Organization";
        }
        if (dt.Columns.Contains("First_Name"))
        {
            dt.Columns["First_Name"].ColumnName = "First Name";
        }
        if (dt.Columns.Contains("Last_Name"))
        {
            dt.Columns["Last_Name"].ColumnName = "Last Name";
        }
        if (dt.Columns.Contains("Phone_Office"))
        {
            dt.Columns["Phone_Office"].ColumnName = "Office Phone";
        }
        if (dt.Columns.Contains("Phone_Mobile"))
        {
            dt.Columns["Phone_Mobile"].ColumnName = "Mobile Phone";
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

            string name = "Users";
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