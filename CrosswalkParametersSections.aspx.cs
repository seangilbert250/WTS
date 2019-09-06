using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Xml;

public partial class CrosswalkParamContainer : System.Web.UI.Page
{
	protected string Grid_View = string.Empty;
	protected string RollupGroup = "Status";
	protected string DefaultSortType = "Tech";
	protected int ActiveTab = 0;
    protected string ddlChanged_ML = "no";


    protected string GridType = "Workload Crosswalk";
    protected string GridPrefix = null;
    protected string GridViewNameSessionKey = "Crosswalk_GridView_ML";
    protected string DefaultGridViewNameSessionKey = "defaultCrosswalkGrid_ML";
    protected int GridTypeEnum = (int)WTSGridName.Workload_Crosswalk;
    protected string GridViewCurrentDropDownSessionKey = "CurrentDropDown";
    protected string GridViewLevelsSessionKey = "Levels";
    protected string GridViewCurrentXMLSessionKey = "CurrentXML";

    protected void Page_Load(object sender, EventArgs e)
	{
		readQueryString();

		loadLookupData();

        if (Session[GridViewNameSessionKey] != null) Session[GridViewNameSessionKey] = ddlView.SelectedItem != null ? ddlView.SelectedItem.ToString() : "Default";
        else Session[GridViewNameSessionKey] = WTSData.GetDefaultGridViewName(GridTypeEnum, UserManagement.GetUserId_FromUsername());

        if (Session[GridViewCurrentXMLSessionKey] != null)
        { 
            txtXML.Value = Session[GridViewCurrentXMLSessionKey].ToString();
        }
        else
        {
            txtXML.Value = "";
        }
    }

    private void readQueryString()
    {
        #region Grid View

        if (Request.QueryString["GridType"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridType"]))
        {
            GridType = Request.QueryString["gridType"];

            if (GridType == "RQMT Grid")
            {
                GridPrefix = "RQMT";
                GridTypeEnum = (int)WTSGridName.RQMT_Grid;
                RollupGroup = null;
                DefaultSortType = null;

                ddlRollupGroup.Items.Clear();
                ddlDefaultTaskSort.Items.Clear();

            }

            if (GridPrefix != null)
            {
                GridViewNameSessionKey = GridPrefix + "_GridView";
                DefaultGridViewNameSessionKey = GridPrefix + "_GridView_Default";
                GridViewCurrentDropDownSessionKey = GridPrefix + "_CurrentDropDown";
                GridViewLevelsSessionKey = GridPrefix + "_Levels";
                GridViewCurrentXMLSessionKey = GridPrefix + "_CurrentXML";
            }
        }

        if (Request.QueryString["ddlChanged_ML"] != null)
        {
            ddlChanged_ML = Request.QueryString["ddlChanged_ML"].ToString();
        }


        if (Session[GridViewNameSessionKey] != null && !string.IsNullOrWhiteSpace(Session[GridViewNameSessionKey].ToString()))
		{
			this.Grid_View = Session[GridViewNameSessionKey].ToString();
		}
		else
		{
			if (Request.QueryString["gridView"] != null
				&& !string.IsNullOrWhiteSpace(Request.QueryString["gridView"].ToString()))
			{
				this.Grid_View = Server.UrlDecode(Request.QueryString["gridView"]).Replace("?", "");
			}
			else
			{
				this.Grid_View = string.Empty;
			}
			Session[GridViewNameSessionKey] = this.Grid_View;
		}

		#endregion Grid View


        // 11626 - 2 > Read user preference. SCB TODO - Get this added to My Profile and hook them up.
        if (Session[DefaultGridViewNameSessionKey] != null && !string.IsNullOrWhiteSpace(Session[DefaultGridViewNameSessionKey].ToString()))
        { 
            this.DefaultSortType = Session[DefaultGridViewNameSessionKey].ToString();
        }
        else
        {
            this.DefaultSortType = "Tech";
        }


        if (Request.QueryString["Tab"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["Tab"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["Tab"].ToString()), out this.ActiveTab);
		}
	}

	private void loadLookupData()
	{
		int UserId = UserManagement.GetUserId_FromUsername();
		WTS_User u = new WTS_User(UserId);
		u.Load();
		DataTable dt = WTSData.GetViewOptions(userId: UserId, gridName: GridType);
		DataTable dtSetting = u.UserSettingList_Get(u.ID, (int)UserSettingType.GridView, GridTypeEnum);

		if (dt != null && dt.Rows.Count > 0)
		{
			ddlView.Items.Clear();

			ListItem item = null;
			foreach (DataRow row in dt.Rows)
			{
				item = new ListItem();
				item.Text = row["ViewName"].ToString();
				item.Value = row["GridViewID"].ToString();
				item.Attributes.Add("OptionGroup", row["WTS_RESOURCEID"].ToString() != "" ? "Custom Views" : "Process Views");
				item.Attributes.Add("MyView", row["MyView"].ToString());
				item.Attributes.Add("Tier1RollupGroup", row["Tier1RollupGroup"].ToString());
				item.Attributes.Add("Tier1ColumnOrder", row["Tier1ColumnOrder"].ToString());
				item.Attributes.Add("Tier2ColumnOrder", row["Tier2ColumnOrder"].ToString());
				item.Attributes.Add("DefaultSortType", row["Tier2SortOrder"].ToString());
                item.Attributes.Add("SectionsXML", row["SectionsXML"].ToString());

                // Set the default, over-write below if user has saved a view preference.
                if (Session["Levels"] == null && item.Text.ToString().ToLower() == "default")
                {
                    if (row["SectionsXML"].ToString() != "")
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(row["SectionsXML"].ToString());

                        HttpContext.Current.Session["Levels"] = xml;
                        //------------------------------------------
                    }
                }

                ddlView.Items.Add(item);

                try
                {
                    if (Session[DefaultGridViewNameSessionKey] != null)
                    {
                        if (ddlView.Items.FindByText(row["ViewName"].ToString()) == null)
                        {
                            // If user has saved view preference, save that XML to Session.
                            if (Session[DefaultGridViewNameSessionKey].ToString().ToLower() == item.Text.ToString().ToLower())
                            {
                                if (row["SectionsXML"].ToString() != "")
                                {
                                    XmlDocument xml = new XmlDocument();
                                    xml.LoadXml(row["SectionsXML"].ToString());

                                    HttpContext.Current.Session["Levels"] = xml;
                                    //------------------------------------------
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Nothing to do here, will use default.
                }
            }

            // 11626 - 2 > Use saved preferences:
            if (Session[DefaultGridViewNameSessionKey] != null && !string.IsNullOrWhiteSpace(Session[DefaultGridViewNameSessionKey].ToString()) && ddlChanged_ML != "yes")
                {
                ListItem itemGridView = ddlView.Items.FindByText(Session[DefaultGridViewNameSessionKey].ToString());
                if (itemGridView != null)
                {
                    itemGridView.Selected = true;
                }
                else
                {
                    this.Grid_View = string.Empty;
                }
            }
            else  // No saved view preference
            {
                if (!string.IsNullOrWhiteSpace(this.Grid_View))
                {
                    ListItem itemGridView = ddlView.Items.FindByText(this.Grid_View);
                    if (itemGridView != null)
                    {
                        itemGridView.Selected = true;
                    }
                    else
                    {
                        this.Grid_View = string.Empty;
                    }
                }
                else
                {
                    if (dtSetting != null && dtSetting.Rows.Count > 0)
                    {
                        WTSUtility.SelectDdlItem(ddlView, dtSetting.Rows[0]["SettingValue"].ToString().Trim());
                    }
                }
            }

            if (Session[GridViewCurrentDropDownSessionKey] != null)
            { 
                // Not working, may be because of "Process Views" "Custom Views"
                //ddlView.SelectedIndex = Convert.ToInt32(Session["CurrentDropDown"].ToString()); 
            }

            if (Session[GridViewLevelsSessionKey] != null)  // HttpContext.Current.
            {
                Page.ClientScript.RegisterArrayDeclaration("dtSectionsView", JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None));
            }
        }
    }

    [WebMethod(true)]
	public static string SaveView(string gridView, string gridViewName, int processView
        , string SectionsXML, string gridType)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "error", "" } };
		bool saved = false;
		string errorMsg = string.Empty;

        string prefix = null;
        int gridEnum = -1;

        if (gridType == "RQMT Grid")
        {
            prefix = "RQMT";
            gridEnum = (int)WTSGridName.RQMT_Grid;
        }

		try
		{
            int gridViewID = 0;
			int.TryParse(gridView, out gridViewID);
			GridView gv = new GridView();

            XmlDocument xmlLevels = new XmlDocument();
            xmlLevels.LoadXml(SectionsXML);

            gv.GridNameID = gridEnum != -1 ? gridEnum : (int)WTSGridName.Workload_Crosswalk;
			gv.ID = gridViewID;
			gv.Name = gridViewName;
			gv.UserID = (processView == 1 ? 0 : UserManagement.GetUserId_FromUsername());
            gv.Tier1Columns = "";
            gv.Tier1ColumnOrder = "";
            gv.Tier2Columns = "";
            gv.Tier2SortOrder = "";            

            gv.SectionsXML = xmlLevels; 

            saved = gv.Save(out errorMsg);  // This checks for existing row and does either Add or Update.

			if (saved)
			{
                if (prefix != null)
                {
                    HttpContext.Current.Session[prefix + "_CurrentXML"] = "";
                    HttpContext.Current.Session[prefix + "_CurrentDropDown"] = "";
                    HttpContext.Current.Session[prefix + "_GridView"] = gridViewName;

                    if (SectionsXML != null)
                    {
                        HttpContext.Current.Session[prefix + "_Levels"] = xmlLevels;
                    }
                }
                else
                {
                    HttpContext.Current.Session["Crosswalk_GridView_ML"] = gridViewName;
                    HttpContext.Current.Session["CurrentXML"] = "";
                    HttpContext.Current.Session["CurrentDropDown"] = "";

                    if (SectionsXML != null)
                    {
                        HttpContext.Current.Session["Levels"] = xmlLevels;
                    }
                }
            }
        }
        catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
	}

    [WebMethod(true)]
    public static void SaveDefaultXML(string currentXML, string ddlIndex, string ddlText, string gridType)
    {
        var sessionMethods = new SessionMethods();

        string prefix = null;

        if (gridType == "RQMT Grid")
        {
            prefix = "RQMT";
        }
        else
        {
            sessionMethods.Session["CurrentXML"] = currentXML;
            sessionMethods.Session["CurrentDropDown"] = ddlIndex;
            sessionMethods.Session["Crosswalk_GridView_ML"] = ddlText;
            sessionMethods.Session["defaultCrosswalkGrid_ML"] = ddlText;
        }

        if (prefix != null)
        {
            sessionMethods.Session[prefix + "_CurrentXML"] = currentXML;
            sessionMethods.Session[prefix + "_CurrentDropDown"] = ddlIndex;
            sessionMethods.Session[prefix + "_GridView"] = ddlText;
            sessionMethods.Session[prefix + "_GridView_Default"] = ddlText;
        }
    }

    [WebMethod(true)]
	public static string DeleteView(string gridView)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "error", "" } };
		bool saved = false, exists = false;
		string errorMsg = string.Empty;

		try
		{
			int gridViewID = 0;
			int.TryParse(gridView, out gridViewID);

			if (gridViewID > 0)
			{
				GridView gv = new GridView(gridViewID);
				saved = gv.Delete(out exists, out errorMsg);
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
	}

    [WebMethod(true)]
    public static void UpdateSession(string sLevels, string gridType)
    {
        XmlDocument xmlLevels = new XmlDocument();
        xmlLevels.LoadXml(sLevels);

        string key = "Levels";
        string prefix = null;

        if (gridType == "RQMT Grid")
        {
            prefix = "RQMT";
        }

        if (prefix != null)
        {
            key = prefix + "_Levels";
        }

        Page objp = new Page();
        objp.Session[key] = xmlLevels;
    }

}