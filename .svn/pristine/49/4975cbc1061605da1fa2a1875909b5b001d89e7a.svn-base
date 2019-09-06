using System;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public partial class Report_CR : System.Web.UI.Page
{
    #region Variables
    private bool MyData = true;
    protected string Download = string.Empty;
    protected string ReportView = string.Empty;
    protected string SelectedReleases = "";
    protected string SelectedDeliverables = "";
    protected string SelectedAORTypes = "";
    protected string SelectedVisibleToCustomer = "";
    protected string SelectedContracts = "";
    protected string SelectedSuites = "";
    protected string SelectedTaskStatus = "";
    protected string SelectedWorkloadAllocations = "";
    protected string Title = string.Empty;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();

        if (this.Download != string.Empty)
        {
            ExportData(this.Download);
        }
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

        if (Request.QueryString["Download"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Download"]))
        {
            this.Download = Request.QueryString["Download"];
        }

        if (Request.QueryString["ReportView"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReportView"]))
        {
            this.ReportView = Request.QueryString["ReportView"];
        }

        if (Request.QueryString["SelectedReleases"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedReleases"]))
        {
            SelectedReleases = Request.QueryString["SelectedReleases"];
        }

        if (Request.QueryString["SelectedDeliverables"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedDeliverables"]))
        {
            SelectedDeliverables = Request.QueryString["SelectedDeliverables"];
        }

        if (Request.QueryString["SelectedAORTypes"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedAORTypes"]))
        {
            SelectedAORTypes = Request.QueryString["SelectedAORTypes"];
        }

        if (Request.QueryString["SelectedVisibleToCustomer"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedVisibleToCustomer"]))
        {
            SelectedVisibleToCustomer = Request.QueryString["SelectedVisibleToCustomer"];
        }

        if (Request.QueryString["SelectedContracts"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedContracts"]))
        {
            SelectedContracts = Request.QueryString["SelectedContracts"];
        }

        if (Request.QueryString["SelectedSuites"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedSuites"]))
        {
            SelectedSuites = Request.QueryString["SelectedSuites"];
        }

        if (Request.QueryString["SelectedTaskStatus"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedTaskStatus"]))
        {
            SelectedTaskStatus = Request.QueryString["SelectedTaskStatus"];
        }

        if (Request.QueryString["SelectedWorkloadAllocations"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SelectedWorkloadAllocations"]))
        {
            this.SelectedWorkloadAllocations = Request.QueryString["SelectedWorkloadAllocations"];
        }

        if (Request.QueryString["Title"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Title"]))
        {
            this.Title = Uri.UnescapeDataString(Request.QueryString["Title"]);
        }
    }
    #endregion

    #region Data
    private void ExportData(string type)
    {
        switch (type)
        {
            case "pdf":
                DataSet ds = AOR.AORCRReport_Get(SelectedReleases, SelectedDeliverables, SelectedAORTypes, SelectedVisibleToCustomer, SelectedContracts, SelectedSuites, SelectedTaskStatus, SelectedWorkloadAllocations, this.Title);

                if (ds != null)
                {
                    #region CR
                    DataTable dtCR = ds.Tables["CR"];
                    DateTime nDate = new DateTime();

                    dtCR.Columns.Add("UpdatedDate");

                    for (int i = 0; i < dtCR.Rows.Count; i++)
                    {
                        dtCR.Rows[i]["CRCustomerTitle"] = Uri.UnescapeDataString(dtCR.Rows[i]["CRCustomerTitle"].ToString());
                        dtCR.Rows[i]["CRInternalTitle"] = Uri.UnescapeDataString(dtCR.Rows[i]["CRInternalTitle"].ToString());
                        dtCR.Rows[i]["Notes"] = Uri.UnescapeDataString(dtCR.Rows[i]["Notes"].ToString());
                        dtCR.Rows[i]["SRs"] = Uri.UnescapeDataString(dtCR.Rows[i]["SRs"].ToString()).Replace("%2<br>", "<br>").Replace("%0<br>", "<br>").Replace("%<br>", "<br>");

                        if (DateTime.TryParse(dtCR.Rows[i]["UpdatedDateTime"].ToString(), out nDate))
                        {
                            dtCR.Rows[i]["UpdatedDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }
                    }

                    dtCR.Columns["UpdatedDate"].SetOrdinal(dtCR.Columns["UpdatedDateTime"].Ordinal);
                    dtCR.Columns.Remove("UpdatedDateTime");
                    #endregion

                    #region AOR
                    DataTable dtAOR = ds.Tables["AOR"];

                    dtAOR.Columns.Add("UpdatedDate");
                    dtAOR.Columns.Add("LastMeeting");
                    dtAOR.Columns.Add("NextMeeting");

                    for (int i = 0; i < dtAOR.Rows.Count; i++)
                    {
                        dtAOR.Rows[i]["CRCustomerTitle"] = Uri.UnescapeDataString(dtAOR.Rows[i]["CRCustomerTitle"].ToString());
                        nDate = new DateTime();

                        if (DateTime.TryParse(dtAOR.Rows[i]["UpdatedDateTime"].ToString(), out nDate))
                        {
                            dtAOR.Rows[i]["UpdatedDate"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        nDate = new DateTime();

                        if (DateTime.TryParse(dtAOR.Rows[i]["LastMeetingTime"].ToString(), out nDate))
                        {
                            dtAOR.Rows[i]["LastMeeting"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }

                        nDate = new DateTime();

                        if (DateTime.TryParse(dtAOR.Rows[i]["NextMeetingTime"].ToString(), out nDate))
                        {
                            dtAOR.Rows[i]["NextMeeting"] = String.Format("{0:M/d/yyyy h:mm tt}", nDate);
                        }
                    }

                    dtAOR.Columns["UpdatedDate"].SetOrdinal(dtAOR.Columns["UpdatedDateTime"].Ordinal);
                    dtAOR.Columns["LastMeeting"].SetOrdinal(dtAOR.Columns["LastMeetingTime"].Ordinal);
                    dtAOR.Columns["NextMeeting"].SetOrdinal(dtAOR.Columns["NextMeetingTime"].Ordinal);
                    dtAOR.Columns.Remove("UpdatedDateTime");
                    dtAOR.Columns.Remove("LastMeetingTime");
                    dtAOR.Columns.Remove("NextMeetingTime");
                    #endregion

                    string fileName = "CR Report";
                    ReportDocument cryRpt = new ReportDocument();

                    cryRpt.Load(Server.MapPath(@"~/Reports/CR.rpt"));
                    cryRpt.SetDataSource(ds);

                    switch (this.ReportView)
                    {
                        case "CRs Only":
                            cryRpt.ReportDefinition.Sections["DetailSection5"].SectionFormat.EnableSuppress = true;
                            break;
                    }

                    cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, fileName);
                    Response.End();
                }
                break;
        }
    }
    #endregion
}