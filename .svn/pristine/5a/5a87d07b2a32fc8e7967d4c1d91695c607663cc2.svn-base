using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class MDAddEdit_AORResource : System.Web.UI.Page
{
    #region Variables
    protected bool CanEdit = false;
    protected int ResourceID = 0;
    protected string Resource = string.Empty;
    protected string[] QFRelease = { };
    protected string Releases = "[";

    private DataColumnCollection DCC;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.MasterData);
        spnTitle.InnerText = "Resource Allocation - " + this.Resource;

        DataTable dtRelease = MasterData.ProductVersionList_Get();

        if (dtRelease != null)
        {
            ddlReleaseQF.Items.Clear();
            ListItem li = null;

            foreach (DataRow dr in dtRelease.Rows)
            {
                li = new ListItem(dr["ProductVersion"].ToString(), dr["ProductVersionID"].ToString());
                if (QFRelease.Contains(dr["ProductVersionID"].ToString())) Releases += (dr["ProductVersionID"].ToString() + ",");

                if (QFRelease.Count() == 0)
                {
                    if (dr["DefaultSelection"].ToString() == "1")
                    {
                        QFRelease = dr["ProductVersionID"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        Releases += (dr["ProductVersionID"].ToString() + ",");
                    }
                }
                ddlReleaseQF.Items.Add(li);
            }
            Releases += "]";
        }

        DataTable dt = LoadData();
        if (dt != null) this.DCC = dt.Columns;

        grdData.DataSource = dt;
        grdData.DataBind();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["ResourceID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ResourceID"]))
        {
            int.TryParse(Request.QueryString["ResourceID"], out this.ResourceID);
        }

        if (Request.QueryString["Resource"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Resource"]))
        {
            this.Resource = Request.QueryString["Resource"];
        }
        if (Request.QueryString["SelectedReleases"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedReleases"]))
        {
            this.QFRelease = Request.QueryString["SelectedReleases"].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
        List<string> listRelease = new List<string>();
        DataTable dt = new DataTable();

        if (QFRelease != null && QFRelease.Length > 0)
        {
            foreach (string s in QFRelease)
            {
                listRelease.Add(s);
            }
        }

        if (IsPostBack && Session["dtSystemResource"] != null)
        {
            dt = (DataTable)Session["dtSystemResource"];
        }
        else
        {
            dt = AOR.AORResourceAllocationList_Get(WTS_ResourceID: this.ResourceID, ReleaseIDs: String.Join(",", listRelease));
            Session["dtSystemResource"] = dt;
        }

        return dt;
    }
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);
    }

    private void grdData_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatRowDisplay(ref row);

        if (DCC.Contains("AORReleaseResource_ID"))
        {
            row.Attributes.Add("aorreleaseresource_id", row.Cells[DCC.IndexOf("AORReleaseResource_ID")].Text);

            if (this.CanEdit)
            {
                if (DCC.Contains("Allocation %"))
                {
                    row.Cells[DCC.IndexOf("Allocation %")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("Allocation %")].Controls.Add(CreateTextBox(row.Cells[DCC.IndexOf("AORReleaseResource_ID")].Text, "Allocation %", row.Cells[DCC.IndexOf("Allocation %")].Text, true));
                }
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

        if (DCC.Contains("Release Version")) row.Cells[DCC.IndexOf("Release Version")].Style["width"] = "60px";
        if (DCC.Contains("Allocation %")) row.Cells[DCC.IndexOf("Allocation %")].Style["width"] = "75px";
    }

    private void FormatRowDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
        {
            if (DCC[i].ColumnName.EndsWith("_ID")) row.Cells[i].Style["display"] = "none";
        }

        if (DCC.Contains("Release Version")) row.Cells[DCC.IndexOf("Release Version")].Style["text-align"] = "center";
        if (DCC.Contains("Allocation %")) row.Cells[DCC.IndexOf("Allocation %")].Style["text-align"] = "center";
    }

    private TextBox CreateTextBox(string ReleaseResource_ID, string type, string value, bool isNumber)
    {
        string txtValue = Server.HtmlDecode(Uri.UnescapeDataString(value)).Trim();
        TextBox txt = new TextBox();

        txt.Text = txtValue;
        txt.MaxLength = 50;
        txt.Width = new Unit(type == "Allocation %" ? 90 : 95, UnitType.Percentage);
        txt.Attributes["class"] = "saveable";
        txt.Attributes["onkeyup"] = "input_change(this);";
        txt.Attributes["onpaste"] = "input_change(this);";
        txt.Attributes["onblur"] = "txtBox_blur(this);";
        txt.Attributes.Add("aorreleaseresource_id", ReleaseResource_ID);
        txt.Attributes.Add("field", type);
        txt.Attributes.Add("original_value", txtValue);

        if (isNumber)
        {
            txt.MaxLength = 3;
            txt.Style["text-align"] = "center";
        }

        return txt;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static bool VerifyAllocation(int ResourceID, string changes)
    {
        int allocationSum = 0;
        DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(changes, (typeof(DataTable)));
        DataTable dt = AOR.AORResourceAllocationList_Get(WTS_ResourceID: ResourceID);

        if (dt != null)
        {
            string release = "0";
            int allocation = 0;
            bool changed = false;
            foreach (DataRow dr in dt.Rows)
            {
                changed = false;
                allocation = 0;
                if (release != dr["Release Version"].ToString())
                {
                    if (allocationSum > 100) return false;
                    allocationSum = 0;
                    release = dr["Release Version"].ToString();
                }

                if (dtjson != null)
                {
                    foreach (DataRow drjson in dtjson.Rows)
                    {
                        if (drjson["aorreleaseresourceid"].ToString() == dr["AORReleaseResource_ID"].ToString())
                        {
                            int.TryParse(drjson["value"].ToString(), out allocation);
                            changed = true;
                        }
                    }
                }
                if (!changed) int.TryParse(dr["Allocation %"].ToString(), out allocation);
                allocationSum += allocation;
            } 
        }

        return allocationSum <= 100;
    }

    [WebMethod()]
    public static string SaveChanges(string changes)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "error", "" } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            XmlDocument docChanges = (XmlDocument)JsonConvert.DeserializeXmlNode(changes, "changes");

            saved = AOR.AORResourceAllocation_Save(Changes: docChanges);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            saved = false;
            errorMsg = ex.Message;
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}