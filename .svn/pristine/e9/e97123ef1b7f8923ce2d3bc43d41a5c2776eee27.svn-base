using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_ConfigureHotlist : System.Web.UI.Page
{
    protected bool isAdmin = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        this.isAdmin = UserManagement.UserIsUserAdmin(UserManagement.GetUserId_FromUsername());
        initControls();
    }
    private void initControls()
    {
        DataTable dtStatus = MasterData.StatusList_Get(includeArchive: false);
        DataTable dtResources = UserManagement.LoadUserList();
        DataTable dtConfigList = WTS_Reports.HostlistConfigList_Get();
        load_ListBoxAssigned(dtResources);
        load_ListBoxRecipients(dtResources);
        load_ListBoxStatus(dtStatus.Copy());
        load_ProductionPanel(dtStatus.Copy());
        load_ddlRank();
        load_ddlConfig(dtConfigList);
    }

    private void load_ddlRank()
    {
        ddlTechMin.Items.Clear();
        ddlTechMax.Items.Clear();
        ddlBusMin.Items.Clear();
        ddlBusMax.Items.Clear();

        for (int i = 1; i <= 10; i++)
        {
            ListItem li = new ListItem(i.ToString(), i.ToString());
            ddlTechMin.Items.Add(li);
            ddlTechMax.Items.Add(li);
            ddlBusMin.Items.Add(li);
            ddlBusMax.Items.Add(li);
        }
    }

    private void load_ProductionPanel(DataTable dtStatus)
    {
        dtStatus.DefaultView.RowFilter = "StatusType = 'Production'";
        dtStatus = dtStatus.DefaultView.ToTable();
        foreach (DataRow row in dtStatus.Rows)
        {
            Label l = new Label();
            l.Text = row["Status"].ToString();
            CheckBox chk = new CheckBox();
            chk.InputAttributes.Add("StatusID", row["STATUSID"].ToString());
            prodStatus.Controls.Add(l);
            prodStatus.Controls.Add(chk);
        }
    }
    private void load_ListBoxStatus(DataTable dtStatus)
    {
        dtStatus.DefaultView.RowFilter = "StatusType = 'Work'";
        dtStatus = dtStatus.DefaultView.ToTable();
        ListBoxStatusAvailable.DataSource = dtStatus;
        ListBoxStatusAvailable.DataTextField = "Status";
        ListBoxStatusAvailable.DataValueField = "StatusID";
        ListBoxStatusAvailable.DataBind();
    }
    private void load_ListBoxAssigned(DataTable dtAssigned)
    {
        ListBoxAssignedAvailable.DataSource = dtAssigned;
        ListBoxAssignedAvailable.DataTextField = "UserName";
        ListBoxAssignedAvailable.DataValueField = "WTS_RESOURCEID";
        ListBoxAssignedAvailable.DataBind();
    }
    private void load_ListBoxRecipients(DataTable dtRecipients)
    {
        ListBoxRecipientAvailable.DataSource = dtRecipients;
        ListBoxRecipientAvailable.DataTextField = "UserName";
        ListBoxRecipientAvailable.DataValueField = "Email";
        ListBoxRecipientAvailable.DataBind();
    }
    private void load_ddlConfig(DataTable dtConfig)
    {
        if (dtConfig != null && dtConfig.Rows.Count > 0)
        {
            DataRow r = dtConfig.Select("Active=1").FirstOrDefault();
            if (r != null)
            {
                r["Name"] = "(Active) " + r["Name"];
                dtConfig.AcceptChanges();
            }
        }
        ddlConfig.DataSource = dtConfig;
        ddlConfig.DataTextField = "Name";
        ddlConfig.DataValueField = "ConfigID";
        ddlConfig.DataBind();
    }

    public class config {
        public int[] ProdStatus;
        public int TechMin;
        public int TechMax;
        public int BusMin;
        public int BusMax;
        public int[] status;
        public int[] assigned;
        public string[] recipients;
        public string message;

        public config(DataTable dt)
        {
            if (dt.Columns.Contains("prodStatus") && dt.Rows[0]["prodStatus"].ToString().Length > 0)
            {

                this.ProdStatus = Array.ConvertAll(dt.Rows[0]["prodStatus"].ToString().Split(','), int.Parse);
            }
            if (dt.Columns.Contains("techMin") && dt.Rows[0]["techMin"].ToString().Length > 0)
            {

                this.TechMin = Int32.Parse(dt.Rows[0]["techMin"].ToString());
            }
            if (dt.Columns.Contains("techMax") && dt.Rows[0]["techMax"].ToString().Length > 0)
            {

                this.TechMax = Int32.Parse(dt.Rows[0]["techMax"].ToString());
            }
            if (dt.Columns.Contains("busMin") && dt.Rows[0]["busMin"].ToString().Length > 0)
            {

                this.BusMin = Int32.Parse(dt.Rows[0]["busMin"].ToString());
            }
            if (dt.Columns.Contains("busMax") && dt.Rows[0]["busMax"].ToString().Length > 0)
            {

                this.BusMax = Int32.Parse(dt.Rows[0]["busMax"].ToString());
            }
            if (dt.Columns.Contains("status") && dt.Rows[0]["status"].ToString().Length > 0)
            {

                this.status = Array.ConvertAll(dt.Rows[0]["status"].ToString().Split(','), int.Parse);
            }
            if (dt.Columns.Contains("assigned") && dt.Rows[0]["assigned"].ToString().Length > 0)
            {

                this.assigned = Array.ConvertAll(dt.Rows[0]["assigned"].ToString().Split(','), int.Parse);
            }
            if (dt.Columns.Contains("recipients") && dt.Rows[0]["recipients"].ToString().Length > 0)
            {

                this.recipients = dt.Rows[0]["recipients"].ToString().Split(';');
            }
            if (dt.Columns.Contains("message") && dt.Rows[0]["message"].ToString().Length > 0)
            {

                this.message = dt.Rows[0]["message"].ToString();
            }
        }
    };

    [WebMethod(EnableSession = true)]
    public static KeyValuePair<int, string> addConfig(string prodStatus, int techMin, int busMin, int techMax, int busMax, string status, string assigned, string recipients, string message, string name)
    {
        int configID = 0;

        string errorMessage = WTS_Reports.createHostlistConfig(prodStatus, techMin, techMax, busMin, busMax, status, assigned, recipients, message, name, ref configID);
        if (errorMessage.Length > 0)
        {
            throw new Exception(errorMessage);
        }

        return new KeyValuePair<int, string>(configID, name);
    }
    [WebMethod(EnableSession = true)]
    public static void deleteConfig(int configID)
    {
        string errorMessage = WTS_Reports.HotlistConfig_Delete(configID);
        if (errorMessage.Length > 0)
        {
            throw new Exception(errorMessage);
        }
    }
    [WebMethod(EnableSession = true)]
    public static string getConfig(int configID)
    {
        DataTable dt = WTS_Reports.HotlistConfig_Get(configID);
        config c = new config(dt);
        return JsonConvert.SerializeObject(c);
    }

    [WebMethod(EnableSession = true)]
    public static string setActive(int configID)
    {
        string error = WTS_Reports.Hostlist_Config_SetActive(configID);
        return error;
    }
    [WebMethod(EnableSession = true)]
    public static void sendHotlist(string prodStatus, int techMin, int busMin, int techMax, int busMax, string status, string assigned, string recipients, string message)
    {
        WTS_Reports.sendHotlistOnDemand(prodStatus, techMin, busMin, techMax, busMax, status, assigned, recipients, message);
        return;
    }

    [WebMethod(EnableSession = true)]
    public static void TESTsendHotlist(string prodStatus, int techMin, int busMin, int techMax, int busMax, string status, string assigned, string recipients, string message)
    {
        WTS_Reports.TESTsendHotlistOnDemand(prodStatus, techMin, busMin, techMax, busMax, status, assigned, recipients, message);
        return;
    }

    [WebMethod(EnableSession = true)]
    public static void TESTSendSrReport()
    {
        WTS_Reports.TESTSendSrReport();
        return;
    }


}