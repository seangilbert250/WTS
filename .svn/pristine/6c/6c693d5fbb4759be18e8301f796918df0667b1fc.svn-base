using System;
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

public partial class FilterPage : System.Web.UI.Page
{
	string parentModule = string.Empty;
	protected bool _myData = true;
    protected string Source = string.Empty;
    protected string Options = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
		parentModule = Request.QueryString["parentModule"];
		if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
			|| Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
		{
			_myData = true;
		}
		else
		{
			_myData = false;
		}

        if (Request.QueryString["Source"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Source"]))
        {
            this.Source = Request.QueryString["Source"];
        }

        if (Request.QueryString["Options"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Options"]))
        {
            this.Options = Request.QueryString["Options"];
        }

        LoadFilterFields();
        LoadCustomUserFilters();
    }

    protected void LoadFilterFields()
    {
        try
        {
            DataTable dt = Filtering.GetFilterFields(parentModule, Options);

            if (dt != null && dt.Rows.Count > 0)
            {
				//dt.DefaultView.Sort = "NAME";
				//dt = dt.DefaultView.ToTable();

                for (var i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    HtmlGenericControl newLi = new HtmlGenericControl("li");
                    newLi.InnerText = dt.Rows[i]["NAME"].ToString();

                    if (dt.Rows[i]["FIELD"].ToString().Trim().ToUpper() == "HEADER")
                    {
                        newLi.Attributes["class"] = "filterFieldsHeader";
                    }
                    else
                    {
                        newLi.Attributes["id"] = "filterCriteria_" + dt.Rows[i]["NAME"].ToString();
						newLi.Attributes["field"] = dt.Rows[i]["FIELD"].ToString();
						newLi.Attributes["name"] = dt.Rows[i]["NAME"].ToString();
                        newLi.Attributes["onclick"] = "skm_LockScreen('Gathering filters...'); filterField_Click(this); return false;";
                        newLi.Attributes["class"] = "filterFieldsItem";
                    }

					this.lstFilterFields.Controls.Add(newLi);
                }
            }
        }
        catch (Exception e)
        {
			LogUtility.LogException(e);
            throw e;
        }
    }

    protected void LoadCustomUserFilters()
    {
        try
        {
            DataTable dtCustomFilter = Filtering.GetCustomFilters();
            dtCustomFilter = BuildCustomFilterTable(dtCustomFilter, parentModule);

            ddlSavedFilters.DataSource = dtCustomFilter;
            ddlSavedFilters.DataValueField = "FilterString";
            ddlSavedFilters.DataTextField = "CollectionName";
            ddlSavedFilters.DataBind();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    protected static DataTable BuildCustomFilterTable(DataTable dtFilters, string module)
    {
        try
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CollectionName");
            dt.Columns.Add("FilterString");
            dt.AcceptChanges();

            if (dtFilters != null)
            {
                foreach (DataRow dRow in dtFilters.Rows)
                {
                    if (dRow["Module"].ToString() == module)
                    {
                        DataRow nRow = GetCustomFilterRow(dt, dRow["CollectionName"].ToString());
                        string filterTextName = dRow["FilterName"].ToString();
                        string filterIDName = dRow["FilterName"].ToString();
                        string filterParamID = filterTextName.ToUpper().Contains("CONTAINS") ? dRow["FilterText"].ToString() : dRow["FilterID"].ToString();
                        string filterParamName = dRow["FilterText"].ToString();

                        if (nRow[1] == "")
                        {
                            nRow[1] = filterTextName + "|" + filterIDName + "|" + filterParamID + "|" + filterParamName;
                        }
                        else
                        {
                            nRow[1] = nRow[1] + "`" + filterTextName + "|" + filterIDName + "|" + filterParamID + "|" + filterParamName;
                        }
                    }
                }
            }

            return dt;
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            return null;
        }
    }

    protected static DataRow GetCustomFilterRow(DataTable dt, string strFilterName)
    {
        try
        {
            foreach (DataRow dRow in dt.Rows)
            {
                if (dRow[0].ToString() == strFilterName)
                {
                    return dRow;
                }
            }
            return dt.Rows.Add(strFilterName, "");
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            return null;
        }
    }

    [WebMethod(true)]
    public static string LoadFilters(string module, string filterName, dynamic filterValues, bool myData = false, string option = "")
    {
        Dictionary<string, object> fields = JsonConvert.DeserializeObject<Dictionary<string, object>>(filterValues);

        int filterType = 1;
        if (option == ((int)WTS.Reports.ReportTypeEnum.TaskReport).ToString()) filterType = (int)WTS.Enums.FilterTypeEnum.TaskReport;
        if (option == ((int)WTS.Reports.ReportTypeEnum.ReleaseDSEReport).ToString()) filterType = (int)WTS.Enums.FilterTypeEnum.DSEReport;

        DataTable dtFilters = Filtering.LOAD_FILTER_PARAMS(module: module, filterName: filterName, filterType: filterType, filters: fields, myData: myData);

        List<string> lMsg = new List<string>();

        return JsonConvert.SerializeObject(dtFilters);

    }

    [WebMethod(true)]
    public static string SaveCustomFilter(string collectionName, bool deleteFilter, string module, dynamic filters)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "collectionName", collectionName }, { "customFilter", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            if (filters != null)
            {
                dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>(filters);

                saved = Filtering.SaveCustomFilters(collectionName: collectionName, deleteFilter: deleteFilter, module: module, filters: fields);
            }

            if (!deleteFilter)
            {
                DataTable dtCustomFilter = Filtering.GetCustomFilters(collectionName, module);
                dtCustomFilter = BuildCustomFilterTable(dtCustomFilter, module);
                result["collectionName"] = dtCustomFilter.Rows[0]["CollectionName"].ToString();
                result["customFilter"] = dtCustomFilter.Rows[0]["FilterString"].ToString();
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            saved = false;
            errorMsg = ex.Message;
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

}