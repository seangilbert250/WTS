using System;
using System.Data;

public partial class SR_Tabs : System.Web.UI.Page
{
    #region Variables
    protected bool NewSR = false;
    protected int SRID = 0;
    protected int AttachmentCount = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        LoadData();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["NewSR"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewSR"]))
        {
            bool.TryParse(Request.QueryString["NewSR"], out this.NewSR);
        }

        if (Request.QueryString["SRID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["SRID"]))
        {
            int.TryParse(Request.QueryString["SRID"], out this.SRID);
        }
    }
    #endregion

    #region Data
    private void LoadData()
    {
        if (!this.NewSR)
        {
            lblSR.Text = "SR #: " + this.SRID;

            DataTable dtAttachment = SR.SRAttachmentList_Get(SRID: this.SRID);

            if (dtAttachment != null) this.AttachmentCount = dtAttachment.Rows.Count;
        }
    }
    #endregion
}