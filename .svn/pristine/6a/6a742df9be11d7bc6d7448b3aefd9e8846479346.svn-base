using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class ITI_Settings : Page
{
	protected int _activeTab = 0;
	private static string _gridView;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(Request.QueryString["GridView"])) _gridView = Request.QueryString["GridView"];
		if (!string.IsNullOrWhiteSpace(Request.QueryString["ActiveTab"])) _activeTab = Convert.ToInt32(Request.QueryString["ActiveTab"]);
		LoadData();
	}

	private void LoadData()
	{
	    var nodeLevel = 1;
        var defaultId = 0;
	    var nodeCount = 0;
        var delimiter = ',';
        var itisettings_all_DB = GetViewOptions();
        var ModifiedColumnNames = new List<string>();
	    var allColNames = new List<string>();
	    dynamic itisettings_all_sess = JsonConvert.DeserializeObject(Session["itisettings"].ToString());
	    string[] colOrder_all_sess = itisettings_all_sess.columnorder.ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Replace(" ", "").Replace("\r\n", "").Trim().Split(delimiter);

        foreach (DataRow itisettings_prop_DB in itisettings_all_DB.Rows)
	        if (itisettings_prop_DB["WTS_RESOURCEID"].ToString() != "")
	        {
	            defaultId = getDefaultGridViewId(Convert.ToInt32(itisettings_prop_DB["GridNameID"]), Convert.ToInt32(itisettings_prop_DB["WTS_RESOURCEID"]));
	            break;
	        }

        if (defaultId > 0)
            foreach (DataRow itisettings_prop_DB in itisettings_all_DB.Rows)
            {
                if (itisettings_prop_DB["GridViewID"].ToString() == defaultId.ToString())
                {
                    itisettings_prop_DB["DefaultSelection"] = true;
                }
                else
                {
                    itisettings_prop_DB["DefaultSelection"] = false;
                }
            }

        if (itisettings_all_DB.Rows.Count > 0)
		{
            //Create the dropdownlist items to be displayed
			ddlView.Items.Clear();
            ddlView.Items.Add("Customized View");

			foreach (DataRow itisettings_prop_DB in itisettings_all_DB.Rows)
			    if (itisettings_prop_DB["ViewType"].ToString() == "1")
			    {
				    var item = new ListItem();
				    item.Text = itisettings_prop_DB["ViewName"].ToString();
				    item.Value = itisettings_prop_DB["GridViewID"].ToString();
				    item.Attributes.Add("OptionGroup", itisettings_prop_DB["WTS_RESOURCEID"].ToString() != "" ? "Custom Views" : "Process Views");
                    item.Attributes.Add("ViewType", itisettings_prop_DB["ViewType"].ToString());
				    item.Attributes.Add("MyView", itisettings_prop_DB["MyView"].ToString());
				    item.Attributes.Add("DefaultSelection", itisettings_prop_DB["DefaultSelection"].ToString());
				    ddlView.Items.Add(item);
			    }
		}

        paramTreeView.Nodes.Clear();
        //Set up the columns to be displayed
	    foreach (var currTblCol_sess in itisettings_all_sess.tblCols)
	    {
	        allColNames.Add(currTblCol_sess.alias.ToString() != "" ? currTblCol_sess.alias.ToString() : currTblCol_sess.name.ToString());
	        if ((bool) currTblCol_sess.show) ModifiedColumnNames.Add(currTblCol_sess.alias.ToString() != "" ? currTblCol_sess.alias.ToString() : currTblCol_sess.name.ToString());
	    }

        //Create a tree of the cols we need to set up
	    foreach (var currColIndex_sess in colOrder_all_sess) { 
            //Find out if the colName is in the allColNames
	        foreach (var currMidifiedColName in ModifiedColumnNames) { 
	            if (currMidifiedColName == allColNames[Convert.ToInt32(currColIndex_sess) - 1])
	            {
	                var node = new TreeNode();

                    node.Text = nodeCount == 0 ? "Lvl " + nodeLevel + " - " + currMidifiedColName : currMidifiedColName;
	                node.Expanded = true;

                    if (nodeCount == 0)
                    {
                        paramTreeView.Nodes.Add(node);
                    }
                    else
                    {
                        paramTreeView.Nodes[0].ChildNodes.Add(node);
                    }
	                nodeCount++;
                }
            }
        }
        nodeLevel++;

        //Set up the Cols for the subgrids
        foreach (var itiSettings_subgrid_sess in itisettings_all_sess)
        {

            if (itiSettings_subgrid_sess.Name.IndexOf("subgrid") != -1)
            {
                nodeCount = 0;
                ModifiedColumnNames.Clear();
                allColNames.Clear();
                //Find the displayable columns from the json.
                foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
                {
                    foreach (var itiSettings_subgrid_tblCols_sess in itiSettings_subgrid_prop_sess[0])
                    {
                        if (itiSettings_subgrid_tblCols_sess.Name.IndexOf("tblCols") != -1)
                        {
                            foreach (var groupTblCol_sess in itiSettings_subgrid_tblCols_sess)
                            {
                                foreach (var currTblCol_sess in groupTblCol_sess)
                                {
                                    allColNames.Add(currTblCol_sess.alias.ToString() != "" ? currTblCol_sess.alias.ToString() : currTblCol_sess.name.ToString());

                                    if ((bool)currTblCol_sess.show)
                                    {                                       
                                        ModifiedColumnNames.Add(currTblCol_sess.alias.ToString() != "" ? currTblCol_sess.alias.ToString() : currTblCol_sess.name.ToString());
                                    }
                                }
                            }
                        }

                    }
                }
                //Find the column ordering from the json.
                foreach (var itiSettings_subgrid_prop_sess in itiSettings_subgrid_sess)
                {
                    foreach (var itiSettings_subgrid_colOrder_sess in itiSettings_subgrid_prop_sess[0])
                    {
                        if (itiSettings_subgrid_colOrder_sess.Name.IndexOf("columnorder") != -1)
                        {
                            colOrder_all_sess = itiSettings_subgrid_colOrder_sess.ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Replace(" ", "").Replace("\r\n", "").Replace("columnorder:", "").Trim().Split(delimiter);
                        }
                    }
                }

                foreach (var colOrder_prop_sess in colOrder_all_sess)
                {
                    foreach (var currModColName in ModifiedColumnNames)
                    {
                        if (currModColName == allColNames[Convert.ToInt32(colOrder_prop_sess) - 1])
                        {
                            var node = new TreeNode();

                            node.Text = nodeCount == 0 ? "Lvl " + nodeLevel + " - " + currModColName : currModColName;
                            node.Expanded = true;

                            if (nodeCount == 0) paramTreeView.Nodes.Add(node);
                            else paramTreeView.Nodes[paramTreeView.Nodes.Count - 1].ChildNodes.Add(node);
                            nodeCount++;
                        }
                    }
                }

                nodeLevel++;
            }
        }
        itisettings_all_DB.Columns.Remove("Tier1Columns");
        ddlViewSettings_all.Value = JsonConvert.SerializeObject(itisettings_all_DB);
	}

	public static DataTable GetViewOptions()
	{
		var UserId = UserManagement.GetUserId_FromUsername();
		return WTSData.GetViewOptions(UserId, _gridView);
	}

    private int getDefaultGridViewId(int gridNameId, int resourceId)
    {
        var cmdText = "select settingvalue from usersetting where gridnameid = " + gridNameId + " AND wts_resourceid = " + resourceId;

        using (SqlConnection cn = new SqlConnection(WTSCommon.WTS_ConnectionString))
        using (SqlCommand cmd = new SqlCommand(cmdText, cn))
        {
            cmd.CommandType = CommandType.Text;
            cn.Open();

            return Convert.ToInt32(cmd.ExecuteScalar() ?? 145);
        }
    }

	[WebMethod]
	public static string SaveView(string gridView, string gridViewName, int processView , string settings, int viewType)
	{
		var _saved = true;
		var errorMsg = string.Empty;

        try
        {
			var gridViewId = 0;
			var gv = new GridView();
			int.TryParse(gridView, out gridViewId);
			gv.GridNameID = (int) (WTSGridName) Enum.Parse(typeof(WTSGridName), _gridView);
			gv.ID = gridViewId;
			gv.Name = gridViewName;
			gv.UserID = processView == 1 ? 0 : UserManagement.GetUserId_FromUsername();
			gv.Tier1Columns = settings;
		    gv.ViewType = viewType;
			_saved = gv.Save(out errorMsg);
		}
		catch (Exception ex)
		{
			_saved = false;
			LogUtility.LogException(ex);
			errorMsg = ex.Message + "; " + ex.StackTrace;
		}
		return JsonConvert.SerializeObject(new { saved = _saved, error = errorMsg, ddlItems = GetViewOptions(), viewName = gridViewName });
	}

	[WebMethod]
	public static string DeleteView(string gridView)
	{
		bool _saved = false, exists = false;
		string errorMsg = string.Empty;

		try
		{
			int gridViewID = 0;
			int.TryParse(gridView, out gridViewID);

			if (gridViewID > 0)
			{
				GridView gv = new GridView(gridViewID);
				_saved = gv.Delete(out exists, out errorMsg);
			}
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			errorMsg = ex.Message + "; " + ex.StackTrace;
			_saved = false;
		}

		var result = new { saved = _saved, error = errorMsg, ddlItems = GetViewOptions() };
		return JsonConvert.SerializeObject(result);
	}

    [WebMethod]
    public static string GetTier1Data(int gridViewId)
    {
        var dt = GetViewOptions();

        foreach (DataRow row in dt.Rows)
            if (row["GridViewID"].ToString() == gridViewId.ToString())
                return ValidateItiSettings(row["Tier1Columns"].ToString());

        return null;
    }

    [WebMethod(EnableSession = true)]
    public static void UpdateSessionData(string args)
    {
        var sessionMethods = new SessionMethods();
        sessionMethods.Session["itisettings"] = args;
    }

    private static string ValidateItiSettings(string itisettings_all_view_str)
    {
        dynamic itiSettings_all_view;
        var colGroups = new JArray();
        var itiSettings_all_view_temp = JsonConvert.DeserializeObject<dynamic>(itisettings_all_view_str);

        if (itiSettings_all_view_temp is JArray) itiSettings_all_view = itiSettings_all_view_temp[0];
        else itiSettings_all_view = itiSettings_all_view_temp;

        var columnOrder = JsonConvert.DeserializeObject<JArray>(itiSettings_all_view.columnorder.ToString());
        var newAorProperties = new List<AorProperty>();
        var docLevel = new XmlDocument();
        var docFilters = new XmlDocument();
        docFilters.AppendChild(docFilters.CreateElement("filters"));
        docLevel.AppendChild(docLevel.CreateElement("crosswalkparameters"));
        var itiSettings_all_DefCross_temp = AOR.AOR_Crosswalk_Multi_Level_Grid(level: docLevel, filter: docFilters, qfRelease: "", getColumns: true);
        var jsonUpdated = false;

        foreach (var currTblColDefCross
            in itiSettings_all_DefCross_temp.AsEnumerable().Select(r => r.ItemArray[0]).Distinct().ToArray())
            colGroups.Add(currTblColDefCross.ToString());

        itiSettings_all_view.columngroups = colGroups;
        itiSettings_all_view.validated = DateTime.Now;

        foreach (var currTblCol_DefCross in itiSettings_all_DefCross_temp.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
        {
            var itemExists = false;
            foreach (var currTblCol_sess in itiSettings_all_view.tblCols)
                if (currTblCol_sess.name.ToString().ToUpper() == currTblCol_DefCross[1].ToString().ToUpper())
                {
                    currTblCol_sess.colgroup = currTblCol_DefCross[0].ToString();
                    jsonUpdated = true;
                    itemExists = true;
                    break;
                }

            if (!itemExists)
            {
                var newProperty = new AorProperty();

                newProperty.name = currTblCol_DefCross[1].ToString();
                newProperty.alias = "";
                newProperty.show = false;
                newProperty.sortorder = "none";
                newProperty.sortpriority = "";
                newProperty.groupname = "";
                newProperty.concat = false;
                newProperty.colgroup = currTblCol_DefCross[0].ToString();

                newAorProperties.Add(newProperty);
                columnOrder.Add((columnOrder.Count + 1).ToString());
            }
        }

        if (jsonUpdated || newAorProperties.Count > 0)
        {
            var aorProperties = JsonConvert.DeserializeObject<List<AorProperty>>(itiSettings_all_view.tblCols.ToString());

            if (newAorProperties.Count > 0)
                foreach (var item in newAorProperties)
                    aorProperties.Add(item);

            itiSettings_all_view.tblCols = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(aorProperties));
            itiSettings_all_view.columnorder = columnOrder;
        }

        foreach (var itiSettings_subgrid_view in itiSettings_all_view)
            if (itiSettings_subgrid_view.Name.IndexOf("subgrid") != -1)
            {
                jsonUpdated = false;
                foreach (var itiSettings_subgrid_prop_view in itiSettings_subgrid_view)
                {
                    newAorProperties.Clear();
                    columnOrder = JsonConvert.DeserializeObject(itiSettings_subgrid_prop_view[0].columnorder.ToString());

                    foreach (var currTblCol_DefCross in itiSettings_all_DefCross_temp.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
                    {
                        var itemExists = false;
                        foreach (var currTblCol_view in itiSettings_subgrid_prop_view[0].tblCols)
                            if (currTblCol_view.name.ToString().ToUpper() == currTblCol_DefCross[1].ToString().ToUpper())
                            {
                                currTblCol_view.colgroup = currTblCol_DefCross[0].ToString();
                                jsonUpdated = true;
                                itemExists = true;
                                break;
                            }

                        if (!itemExists)
                        {
                            var newProperty = new AorProperty();

                            newProperty.name = currTblCol_DefCross[1].ToString();
                            newProperty.alias = "";
                            newProperty.show = false;
                            newProperty.sortorder = "none";
                            newProperty.sortpriority = "";
                            newProperty.groupname = "";
                            newProperty.concat = false;
                            newProperty.colgroup = currTblCol_DefCross[0].ToString();

                            newAorProperties.Add(newProperty);
                            columnOrder.Add((columnOrder.Count + 1).ToString());
                        }
                    }

                    if (jsonUpdated || newAorProperties.Count > 0)
                    {
                        var aorProperties = JsonConvert.DeserializeObject<List<AorProperty>>(itiSettings_subgrid_prop_view[0].tblCols.ToString());
                        if (newAorProperties.Count > 0)
                            foreach (var i in newAorProperties)
                                aorProperties.Add(i);
                        itiSettings_subgrid_prop_view[0].tblCols = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(aorProperties));
                        itiSettings_subgrid_prop_view[0].columnorder = columnOrder;
                    }
                }
            }

        string[] oldColCount = JsonConvert.DeserializeObject<string[]>(itiSettings_all_view.columnorder.ToString());
        var newColCount = itiSettings_all_DefCross_temp.AsEnumerable().Select(r => r.ItemArray[1]).Distinct().Count();
        var colDiff = oldColCount.Length - newColCount;

        if (colDiff > 0)
        {
            var validCols = new JArray();

            foreach (var currTblCol_view in itiSettings_all_view.tblCols)
            foreach (var currTblCol_DefCross in itiSettings_all_DefCross_temp.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
                if (currTblCol_view.name.ToString().ToUpper() == currTblCol_DefCross[1].ToString().ToUpper())
                {
                    validCols.Add(currTblCol_view);
                    break;
                }

            string colName;
            var newColOrder = new List<string>();

            for (int i = 0; i < itiSettings_all_view.columnorder.Count; i++)
            {
                colName = "";
                for (int j = 0; j < itiSettings_all_view.tblCols.Count; j++)
                    if (itiSettings_all_view.columnorder[i] == j + 1) colName = itiSettings_all_view.tblCols[j].name;

                if (!string.IsNullOrEmpty(colName))
                    for (int k = 0; k < validCols.Count; k++)
                        if (colName == validCols[k].First.First.ToString())
                            newColOrder.Add((k + 1).ToString());
            }

            itiSettings_all_view.tblCols = JArray.FromObject(validCols);
            itiSettings_all_view.columnorder = JArray.FromObject(newColOrder);

            foreach (var itiSettings_subgrid_view in itiSettings_all_view)
                if (itiSettings_subgrid_view.Name.IndexOf("subgrid") != -1)
                {
                    validCols = new JArray();
                    newColOrder.Clear();

                    foreach (var itiSettings_subgrid_prop_view in itiSettings_subgrid_view)
                    {
                        foreach (var currTblCol_view in itiSettings_subgrid_prop_view[0].tblCols)
                        foreach (var currTblCol_DefCross in itiSettings_all_DefCross_temp.AsEnumerable().Select(r => r.ItemArray).Distinct().ToArray())
                            if (currTblCol_view.name.ToString().ToUpper() == currTblCol_DefCross[1].ToString().ToUpper())
                            {
                                validCols.Add(currTblCol_view);
                                break;
                            }

                        for (int i = 0; i < itiSettings_subgrid_prop_view[0].columnorder.Count; i++)
                        {
                            colName = "";
                            for (int j = 0; j < itiSettings_subgrid_prop_view[0].tblCols.Count; j++)
                                if (itiSettings_subgrid_prop_view[0].columnorder[i] == j + 1) colName = itiSettings_subgrid_prop_view[0].tblCols[j].name;

                            if (!string.IsNullOrEmpty(colName))
                                for (int k = 0; k < validCols.Count; k++)
                                    if (colName == validCols[k].First.First.ToString())
                                        newColOrder.Add((k + 1).ToString());
                        }

                        itiSettings_subgrid_prop_view[0].tblCols = JArray.FromObject(validCols);
                        itiSettings_subgrid_prop_view[0].columnorder = JArray.FromObject(newColOrder);
                    }
                }
        }

        if (itiSettings_all_view_temp is JArray) return JsonConvert.SerializeObject(itiSettings_all_view_temp);
        return JsonConvert.SerializeObject(itiSettings_all_view);
    }
}