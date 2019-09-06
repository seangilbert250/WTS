using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Newtonsoft.Json;

public partial class AOR_Scheduled_Deliverables_Edit : Page
{
	#region Variables
	private bool MyData = true;
	protected bool CanEditDeliverable = false;
	protected bool NewDeliverable = false;
    protected int DeliverableID = 0;
    protected int ReleaseID = 0;
    protected string Release = "";
    protected string UsersOptions = string.Empty;
    protected string SystemOptions = string.Empty;
    protected string RoleOptions = string.Empty;
    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
	{
		ReadQueryString();

        this.CanEditDeliverable = (UserManagement.UserCanEdit(WTSModuleOption.AOR));

		LoadData();
	}

	private void ReadQueryString()
	{
		if (Request.QueryString["MyData"] == null || string.IsNullOrWhiteSpace(Request.QueryString["MyData"])
			|| Request.QueryString["MyData"].Trim() == "1" || Request.QueryString["MyData"].Trim().ToUpper() == "TRUE")
		{
			MyData = true;
		}
		else
		{
			MyData = false;
		}

		if (Request.QueryString["NewDeliverable"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["NewDeliverable"]))
		{
			bool.TryParse(Request.QueryString["NewDeliverable"], out NewDeliverable);
		}

        if (Request.QueryString["DeliverableID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["DeliverableID"]))
        {
            int.TryParse(Request.QueryString["DeliverableID"], out DeliverableID);
        }

        if (Request.QueryString["ReleaseID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["ReleaseID"]))
        {
            int.TryParse(Request.QueryString["ReleaseID"], out ReleaseID);
        }
    }
	#endregion

	#region Data
	private void LoadData()
	{
        DataTable dtRelease = MasterData.ProductVersion_Get(this.ReleaseID);
        if (dtRelease != null && dtRelease.Rows.Count > 0)
        {
            foreach (DataRow dr in dtRelease.Rows)
            {
                spnAOR.InnerText = dr["ProductVersion"].ToString();
            }
        }

        if (!this.NewDeliverable)
        {
            DataTable dtDeliverable = MasterData.ReleaseSchedule_Deliverable_Get(this.DeliverableID);
            if (dtDeliverable != null && dtDeliverable.Rows.Count > 0)
            {
                foreach (DataRow dr in dtDeliverable.Rows)
                {
                    txtReleaseDeliverable.Text = dr["ReleaseScheduleDeliverable"].ToString();
                    txtDescription.Text = dr["Description"].ToString();
                    txtNarrative.Text = dr["Narrative"].ToString();
                    int sortOrder = 0;
                    int.TryParse(dr["SORT_ORDER"].ToString(), out sortOrder);
                    txtSortOrder.Text = sortOrder.ToString();
                    bool visible = true;
                    bool.TryParse(dr["Visible"].ToString(), out visible);
                    chkVisible.Checked = visible;

                    DateTime nDate = new DateTime();

                    if (DateTime.TryParse(dr["PlannedStart"].ToString(), out nDate)) txtPlannedStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedEnd"].ToString(), out nDate)) txtPlannedEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedInvStart"].ToString(), out nDate)) txtPlannedInvStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedInvEnd"].ToString(), out nDate)) txtPlannedInvEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedTDStart"].ToString(), out nDate)) txtPlannedTechStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedTDEnd"].ToString(), out nDate)) txtPlannedTechEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedCDStart"].ToString(), out nDate)) txtPlannedCDStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedCDEnd"].ToString(), out nDate)) txtPlannedCDEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedCodingStart"].ToString(), out nDate)) txtPlannedCodingStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedCodingEnd"].ToString(), out nDate)) txtPlannedCodingEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedITStart"].ToString(), out nDate)) txtPlannedITStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedITEnd"].ToString(), out nDate)) txtPlannedITEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedCVTStart"].ToString(), out nDate)) txtPlannedCVTStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedCVTEnd"].ToString(), out nDate)) txtPlannedCVTEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedAdoptStart"].ToString(), out nDate)) txtPlannedAdoptStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedAdoptEnd"].ToString(), out nDate)) txtPlannedAdoptEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedDevTestStart"].ToString(), out nDate)) txtPlannedDevTestStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedDevTestEnd"].ToString(), out nDate)) txtPlannedDevTestEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedIP1Start"].ToString(), out nDate)) txtPlannedIP1Start.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedIP1End"].ToString(), out nDate)) txtPlannedIP1End.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedIP2Start"].ToString(), out nDate)) txtPlannedIP2Start.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedIP2End"].ToString(), out nDate)) txtPlannedIP2End.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedIP3Start"].ToString(), out nDate)) txtPlannedIP3Start.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["PlannedIP3End"].ToString(), out nDate)) txtPlannedIP3End.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["ActualStart"].ToString(), out nDate)) txtActualStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["ActualEnd"].ToString(), out nDate)) txtActualEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["ActualDevTestStart"].ToString(), out nDate)) txtActualDevTestStart.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["ActualDevTestEnd"].ToString(), out nDate)) txtActualDevTestEnd.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["ActualIP1Start"].ToString(), out nDate)) txtActualIP1Start.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["ActualIP1End"].ToString(), out nDate)) txtActualIP1End.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["ActualIP2Start"].ToString(), out nDate)) txtActualIP2Start.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["ActualIP2End"].ToString(), out nDate)) txtActualIP2End.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["ActualIP3Start"].ToString(), out nDate)) txtActualIP3Start.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    if (DateTime.TryParse(dr["ActualIP3End"].ToString(), out nDate)) txtActualIP3End.Text = String.Format("{0:MM/dd/yyyy}", nDate);
                    
                }
            }
        }
    }
	#endregion

	#region AJAX
	[WebMethod]
	public static string Save(bool NewDeliverable, int DeliverableID, string Deliverable, int ReleaseVersionID, string description, string narrative, bool visible,
        string plannedStart, string plannedEnd, 
        string plannedInvStart, string plannedInvEnd, 
        string plannedTechStart, string plannedTechEnd, 
        string plannedCDStart, string plannedCDEnd, 
        string plannedCodingStart, string plannedCodingEnd, 
        string plannedITStart, string plannedITEnd, 
        string plannedCVTStart, string plannedCVTEnd, 
        string plannedAdoptStart, string plannedAdoptEnd,
        string plannedDevTestStart, string plannedDevTestEnd,
        string plannedIP1Start, string plannedIP1End,
        string plannedIP2Start, string plannedIP2End,
        string plannedIP3Start, string plannedIP3End,
        string actualStart, string actualEnd,
        string actualDevTestStart, string actualDevTestEnd,
        string actualIP1Start, string actualIP1End,
        string actualIP2Start, string actualIP2End,
        string actualIP3Start, string actualIP3End,
        int sortOrder, int archive)
	{
		Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

		try
		{
            // TODO: Update if editing a Stage
            if (NewDeliverable)
            {
                result = MasterData.ReleaseSchedule_Deliverable_Add(Deliverable, ReleaseVersionID, description, narrative, visible,
                plannedStart, plannedEnd,
                plannedInvStart, plannedInvEnd,
                plannedTechStart, plannedTechEnd,
                plannedCDStart, plannedCDEnd,
                plannedCodingStart, plannedCodingEnd,
                plannedITStart, plannedITEnd,
                plannedCVTStart, plannedCVTEnd,
                plannedAdoptStart, plannedAdoptEnd,
                plannedDevTestStart, plannedDevTestEnd,
                plannedIP1Start, plannedIP1End,
                plannedIP2Start, plannedIP2End,
                plannedIP3Start, plannedIP3End,
                actualStart, actualEnd,
                actualDevTestStart, actualDevTestEnd,
                actualIP1Start, actualIP1End,
                actualIP2Start, actualIP2End,
                actualIP3Start, actualIP3End, sortOrder);
            } else
            {
                result = MasterData.ReleaseSchedule_Deliverable_Update(DeliverableID, Deliverable, ReleaseVersionID, description, narrative, visible,
                plannedStart, plannedEnd,
                plannedInvStart, plannedInvEnd,
                plannedTechStart, plannedTechEnd,
                plannedCDStart, plannedCDEnd,
                plannedCodingStart, plannedCodingEnd,
                plannedITStart, plannedITEnd,
                plannedCVTStart, plannedCVTEnd,
                plannedAdoptStart, plannedAdoptEnd,
                plannedDevTestStart, plannedDevTestEnd,
                plannedIP1Start, plannedIP1End,
                plannedIP2Start, plannedIP2End,
                plannedIP3Start, plannedIP3End,
                actualStart, actualEnd,
                actualDevTestStart, actualDevTestEnd,
                actualIP1Start, actualIP1End,
                actualIP2Start, actualIP2End,
                actualIP3Start, actualIP3End,
                sortOrder, archive, 1);
            }
            
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);

			result["error"] = ex.Message;
		}

		return JsonConvert.SerializeObject(result);
	}
	#endregion
}