using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json;

public partial class AOR_Release_Assessment_Deployment : System.Web.UI.Page
{
    #region Variables
    protected bool CanEdit = false;
    protected int ReleaseAssessmentID = 0;
    protected int ReleaseID = 0;
    protected DataColumnCollection DCC;
    protected int GridPageIndex = 0;
    protected int RowCount = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEdit = true; // (UserManagement.UserCanEdit(WTSModuleOption.AOR) && AOR.AORReleaseCurrent(AORID: this.AORID, AORReleaseID: this.AORReleaseID));

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
        if (Request.QueryString["ReleaseAssessmentID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseAssessmentID"]))
        {
            int.TryParse(Request.QueryString["ReleaseAssessmentID"], out this.ReleaseAssessmentID);
        }

        if (Request.QueryString["ReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseID"]))
        {
            int.TryParse(Request.QueryString["ReleaseID"], out this.ReleaseID);
        }

        if (Request.QueryString["GridPageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["GridPageIndex"]))
        {
            int.TryParse(Request.QueryString["GridPageIndex"], out this.GridPageIndex);
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

        if (IsPostBack && Session["dtReleaseAssessmentDeployment"] != null)
        {
            dt = (DataTable)Session["dtReleaseAssessmentDeployment"];
        }
        else
        {
            dt = AOR.ReleaseAssessment_DeploymentList_Get(ReleaseAssessmentID: this.ReleaseAssessmentID);

            Session["dtReleaseAssessmentDeployment"] = dt;
        }

        return dt;
    }
    #endregion

    #region Grid
    private void grdData_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        FormatHeaderRowDisplay(ref row);

        if (this.CanEdit)
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

        if (DCC.Contains("ReleaseAssessment_Deployment_ID"))
        {
                row.Attributes.Add("releaseassessmentdeploymentid", row.Cells[DCC.IndexOf("ReleaseAssessment_Deployment_ID")].Text);
            
            if (this.CanEdit)
            {
                if (DCC.Contains("X"))
                {
                    row.Cells[DCC.IndexOf("X")].Style["text-align"] = "center";
                    row.Cells[DCC.IndexOf("X")].Controls.Add(CreateCheckBox(row.Cells[DCC.IndexOf("ReleaseAssessment_Deployment_ID")].Text));
                }

                if (DCC.Contains("Deployment"))
                {
                    row.Cells[DCC.IndexOf("Deployment")].Controls.Add(CreateLink(row.Cells[DCC.IndexOf("Deployment_ID")].Text, row.Cells[DCC.IndexOf("Deployment")].Text));
                }

            }

            if (DCC.Contains("Planned Start")) row.Cells[DCC.IndexOf("Planned Start")].Style["text-align"] = "center";
            if (DCC.Contains("Planned End")) row.Cells[DCC.IndexOf("Planned End")].Style["text-align"] = "center";
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

        if (DCC.Contains("X"))
        {
            row.Cells[DCC.IndexOf("X")].Text = "";
            row.Cells[DCC.IndexOf("X")].Style["width"] = "35px";
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

    private Image CreateImage()
    {
        Image img = new Image();

        img.Attributes["src"] = "Images/Icons/help.png";
        img.Attributes["title"] = "These checkboxes are used to select multiple deployments to disassociate from this AOR";
        img.Attributes["alt"] = "These checkboxes are used to select multiple deployments to disassociate from this AOR";
        img.Attributes["onclick"] = "MessageBox('These checkboxes are used to select multiple deployments to disassociate from this AOR.')";
        img.Attributes["height"] = "12";
        img.Attributes["width"] = "12";
        img.Style["cursor"] = "pointer";

        return img;
    }

    private CheckBox CreateCheckBox(string value)
    {
        CheckBox chk = new CheckBox();

        chk.Attributes.Add("releaseassessmentdeploymentid", value);

        return chk;
    }

    private LinkButton CreateLink(string deploymentID, string deployment)
    {
        LinkButton lb = new LinkButton();

        lb.Text = deployment;
        lb.Attributes["onclick"] = string.Format("openDeployment('{0}'); return false;", deploymentID);

        return lb;
    }
    #endregion

    #region AJAX
    [WebMethod()]
    public static string DeleteReleaseAssessmentDeployment(string releaseAssessmentDeployment)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "error", "" } };
        bool deleted = false;
        string errorMsg = string.Empty;

        try
        {
            DataTable dtReleaseAssessmentDeployment = (DataTable)JsonConvert.DeserializeObject(releaseAssessmentDeployment, (typeof(DataTable)));

            foreach (DataRow dr in dtReleaseAssessmentDeployment.Rows)
            {
                int ReleaseAssessmentDeployment_ID = 0;
                int.TryParse(dr["releaseassessmentdeploymentid"].ToString(), out ReleaseAssessmentDeployment_ID);

                deleted = AOR.ReleaseAssessment_Deployment_Delete(ReleaseAssessmentDeploymentID: ReleaseAssessmentDeployment_ID);
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