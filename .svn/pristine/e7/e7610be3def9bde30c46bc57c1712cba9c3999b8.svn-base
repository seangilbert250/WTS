using System;
using System.Data;

public partial class RQMT_Tabs : System.Web.UI.Page
{
    #region Variables
    protected bool NewRQMT = false;
    protected int RQMTID = 0;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        LoadData();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["NewRQMT"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewRQMT"]))
        {
            bool.TryParse(Request.QueryString["NewRQMT"], out this.NewRQMT);
        }

        if (Request.QueryString["RQMTID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["RQMTID"]))
        {
            int.TryParse(Request.QueryString["RQMTID"], out this.RQMTID);
        }
    }
    #endregion

    #region Data
    private void LoadData()
    {
        if (!this.NewRQMT)
        {
            DataTable dt = RQMT.RQMTList_Get(RQMTID: RQMTID);

            if (dt != null && dt.Rows.Count > 0)
            {
                lblRQMT.Text = dt.Rows[0]["RQMT"].ToString();
            }
        }
    }
    #endregion
}