using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;
public partial class AOR_Resources_Add : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected bool CanEditAOR = false;
    protected int AORID = 0;
    private DataColumnCollection DCC;
    protected string UsersOptions = string.Empty;
    DataTable dtUsers = new DataTable();
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditAOR = UserManagement.UserCanEdit(WTSModuleOption.AOR);

        DataTable dt = LoadData();
        if (dt != null) this.DCC = dt.Columns;

        grdData.DataSource = dt;
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
        
        if (CanEditAOR)
        {
            dtUsers = MasterData.WTS_Resource_Get(includeArchive: false);
            
            foreach (DataRow dr in dtUsers.Rows)
            {
                UsersOptions += "<option value=\"" + dr["WTS_ResourceID"] + "\">" + Uri.EscapeDataString(dr["USERNAME"].ToString()) + "</option>";
            }
        }
        
        if (IsPostBack && txtPostBackType.Text == "LoadGrid")
        {
            dynamic fields = JsonConvert.DeserializeObject<Dictionary<string, object>>((dynamic)txtAppliedFilters.Text);

            dt = AOR.AORResourceList_Get(AORID: AORID, AORReleaseID: 0);
            txtPostBackType.Text = string.Empty;
        }
        else if (IsPostBack && Session["dtAORAddResources"] != null)
        {
            dt = (DataTable)Session["dtAORAddResources"];
        }
        else
        {
            dt = AOR.AORResourceList_Get(AORID: AORID, AORReleaseID: 0);
        }

        Session["dtAORAddResources"] = dt;

        return dt;
    }
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);

        if (DCC.Contains("AOR Name"))
        {
            if (this.CanEditAOR)
            {
                row.Cells[DCC.IndexOf("AOR Name")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("AOR Name")].Controls.Add(CreateLink("Add"));
            }
        }
            if (DCC.Contains("Allocation")) row.Cells[DCC.IndexOf("Allocation")].Text = "Allocation %";


    }
    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatRowDisplay(ref row);

        if (DCC.Contains("AOR Name"))
        {
            if (this.CanEditAOR)
            {
                row.Cells[DCC.IndexOf("AOR Name")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("AOR Name")].Controls.Add(CreateLink("Remove"));
            }
        }

        if (DCC.Contains("Resource"))
        {
            if (this.CanEditAOR)
            {
                row.Cells[DCC.IndexOf("Resource")].Controls.Add(CreateDropDownList(dtUsers,"Resource", "USERNAME",  "WTS_ResourceID", row.Cells[DCC.IndexOf("AOR_ID")].Text, row.Cells[DCC.IndexOf("AOR Name")].Text, row.Cells[DCC.IndexOf("WTS_Resource_ID")].Text, row.Cells[DCC.IndexOf("Resource")].Text));
            }
        }

        if (DCC.Contains("Allocation"))
        {
            if (this.CanEditAOR)
            {
                row.Cells[DCC.IndexOf("AOR Name")].Style["text-align"] = "center";
                row.Cells[DCC.IndexOf("Allocation")].Controls.Add(CreateTextBox("AOR",row.Cells[DCC.IndexOf("Allocation")].Text, "Allocation",row.Cells[DCC.IndexOf("Allocation")].Text, true));
            }
        }
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
        }
        
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";

            decimal val;
            bool isNumeric = decimal.TryParse(row.Cells[i].Text, out val);
            if (isNumeric) row.Cells[i].Style["text-align"] = "center";
        }
    }

    private DropDownList CreateDropDownList(DataTable dt, string field
        , string textField, string valueField
        , string typeID, string typeValue, string value, string text = ""
        , List<string> attributes = null)
    {
        
        DropDownList ddl = new DropDownList();
        
        ddl.Width = new Unit(95, UnitType.Percentage);
        ddl.Attributes["class"] = "saveable";
        ddl.Attributes["onchange"] = "input_change(this);";
        ddl.Attributes.Add("typeid", typeID);
        ddl.Attributes.Add("field", field);
        ddl.Attributes.Add("original_value", value);
        ddl.Style["background-color"] = "#F5F6CE";

        if (dt == null)
        {
            return ddl;
        }

        ListItem item = null;
        foreach (DataRow row in dt.Rows)
        {
            item = new ListItem();
            item.Text = row[textField].ToString().Replace("&nbsp;", " ").Trim();
            item.Value = row[valueField].ToString();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (string key in attributes)
                {
                    item.Attributes.Add(key.Trim(), key.Trim());
                }
            }

            if (ddl.Items.FindByValue(item.Value) == null)
            {
                ddl.Items.Add(item);
            }
        }

        WTSUtility.SelectDdlItem(ddl, string.IsNullOrWhiteSpace(value) ? "0" : value, text);

        return ddl;
    }
    private LinkButton CreateLink(string type)
    {
        LinkButton lb = new LinkButton();
        
        lb.Text = type;
        switch (type)
        {
            case "Add":
                lb.Attributes["onclick"] = string.Format("addResource(); return false;");
                break;
            case "Remove":
                lb.Attributes["onclick"] = string.Format("removeResource(this); return false;");
                break;
        }
        return lb;
    }

    private TextBox CreateTextBox(string typeName, string typeID, string field, string value, bool isNumber)
    {
        string txtValue = Server.HtmlDecode(value).Trim();
        TextBox txt = new TextBox();
        
        txt.Text = txtValue;
        txt.MaxLength = 50;
        txt.Width = new Unit(95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        //txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("typeName", typeName);
        txt.Attributes.Add("typeID", typeID);
        txt.Attributes.Add("field", field);
        txt.Attributes.Add("original_value", txtValue);

        if (isNumber)
        {
            txt.MaxLength = 3;
            txt.Style["text-align"] = "center";
        }

        return txt;
    }
    #endregion
}