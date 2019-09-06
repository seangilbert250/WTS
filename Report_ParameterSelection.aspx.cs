﻿using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using WTS;
using WTS.Reports;

public partial class Report_ParameterSelection : ReportPage
{
    #region Variables
    protected string ReportTypeID = string.Empty;
    protected string ViewID = string.Empty;
    protected string DefaultReportFilters;
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["reporttype"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["reporttype"]))
        {
            this.ReportTypeID = Request.QueryString["reporttype"];
        }
        if (Request.QueryString["viewid"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["viewid"]))
        {
            this.ViewID = Request.QueryString["viewid"];
        }
        IReport rpt = ReportFactory.CreateReport(Int32.Parse(ReportTypeID));
        SetPageTitle(rpt.GetPageTitle());
        DefaultReportFilters = rpt.GetDefaultFilters();
        AddMenuText("Load Saved View");
        AddMenuDDL("- Default Parameters -", Int32.Parse(ReportTypeID), Enum.GetName(typeof(ReportTypeEnum), Int32.Parse(ReportTypeID)));
        AddMenuImageButton("Images/Icons/disk.png", "Save View", "saveView(this)");
        AddMenuImageButton("Images/Icons/delete.png", "Delete View", "deleteView(this)");
        AddMenuImageButton("Images/Icons/arrow_refresh_blue.png", "Reload View", "loadSavedView()");
        AddMenuImageButton("Images/Icons/help.png", "Help", "loadHelpText('SavedViews')", true);
        AddMenuButton("Report Builder", "reportBuilderButton_click(false)");
        AddMenuButton("Generate Report", "getReportParameters(false)", false);
        AddItiMenuItem(Int32.Parse(ReportTypeID), false);
        LoadAvailableFields(rpt);
    }

    protected void LoadAvailableFields(IReport rpt)
    {
        try
        {
            // Do not initialize this variable here.  
            DataSet dsFields = rpt.GetLevelFields();
            StringBuilder strHtml = new StringBuilder();
            
            switch (ReportTypeID)
            {
                case "2":
                    //CR FIELDS
                    if (dsFields.Tables["CR FIELDS"].Rows.Count > 0)
                    {
                        foreach (DataRow row in dsFields.Tables["CR FIELDS"].Rows)
                            strHtml.AppendFormat("<li data-val=\"{0}\">{1}</li>", row["VALUE"], row["TEXT"]);

                        ulCRFields.InnerHtml = strHtml.ToString();
                        strHtml.Clear();
                    }

                    //SR FIELDS
                    if (dsFields.Tables["SR FIELDS"].Rows.Count > 0)
                    {
                        foreach (DataRow row in dsFields.Tables["SR FIELDS"].Rows)
                            strHtml.AppendFormat("<li data-val=\"{0}\">{1}</li>", row["VALUE"], row["TEXT"]);

                        ulSRFields.InnerHtml = strHtml.ToString();
                        strHtml.Clear();
                    }

                    //AOR FIELDS
                    if (dsFields.Tables["AOR FIELDS"].Rows.Count > 0)
                    {
                        foreach (DataRow row in dsFields.Tables["AOR FIELDS"].Rows)
                            strHtml.AppendFormat("<li data-val=\"{0}\">{1}</li>", row["VALUE"], row["TEXT"]);

                        ulAORFields.InnerHtml = strHtml.ToString();
                        strHtml.Clear();
                    }

                    //Task
                    if (dsFields.Tables["TASK FIELDS"].Rows.Count > 0)
                    {
                        foreach (DataRow row in dsFields.Tables["TASK FIELDS"].Rows)
                            strHtml.AppendFormat("<li data-val=\"{0}\">{1}</li>", row["VALUE"], row["TEXT"]);

                        ulTaskFields.InnerHtml = strHtml.ToString();
                        strHtml.Clear();
                    }
                    break;
                case "3":

                    break;
            }
        }
        catch (Exception ex)
        {

        }
    }

    [WebMethod(true)]
    public static string ClearFilterSession(string module)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "getData", false.ToString() }, { "module", module } };
        bool saved = false;
        string errorMsg = string.Empty;

        try
        {
            HttpContext.Current.Session["filters_" + module] = null;
            saved = true;
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