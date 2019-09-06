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
using System.Web.UI.WebControls;

using Newtonsoft.Json;


public partial class WorkRequest_Details : System.Web.UI.Page
{
	protected int WorkRequestID = 0;
	protected bool IsNewWorkRequest = false;
	protected int SourceWorkRequestID = 0;

	protected bool CanEdit = false;


    protected void Page_Load(object sender, EventArgs e)
    {
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkRequest);

		readQueryString();
		loadLookupData();

		if (this.WorkRequestID > 0)
		{
			loadWorkRequest();
		}
    }
	private void readQueryString()
	{
		if (Request.QueryString["workRequestID"] != null
			&& !string.IsNullOrWhiteSpace(Request.QueryString["workRequestID"].ToString()))
		{
			int.TryParse(Server.UrlDecode(Request.QueryString["workRequestID"].ToString()), out this.WorkRequestID);
		}
	}

	private void loadLookupData()
	{
		DataSet dsOptions = WorkRequest.GetAvailableOptions();

		if (dsOptions == null || dsOptions.Tables.Count == 0)
		{
			return;
		}

		ListItem item = null;

		if (dsOptions.Tables.Contains("Contract"))
		{
			ddlContract.DataSource = dsOptions.Tables["Contract"];
			ddlContract.DataTextField = "CONTRACT";
			ddlContract.DataValueField = "CONTRACTID";
			ddlContract.DataBind();
		}
		if (dsOptions.Tables.Contains("Organization"))
		{
			ddlOrganization.DataSource = dsOptions.Tables["Organization"];
			ddlOrganization.DataTextField = "ORGANIZATION";
			ddlOrganization.DataValueField = "ORGANIZATIONID";
			ddlOrganization.DataBind();
		}
		if (dsOptions.Tables.Contains("RequestType"))
		{
			ddlRequestType.DataSource = dsOptions.Tables["RequestType"];
			ddlRequestType.DataTextField = "REQUESTTYPE";
			ddlRequestType.DataValueField = "REQUESTTYPEID";
			ddlRequestType.DataBind();
		}
		if (dsOptions.Tables.Contains("RequestGroup"))
		{
			ddlRequestGroup.DataSource = dsOptions.Tables["RequestGroup"];
			ddlRequestGroup.DataTextField = "RequestGroup";
			ddlRequestGroup.DataValueField = "RequestGroupID";
			ddlRequestGroup.DataBind();
		}
		if (dsOptions.Tables.Contains("Scope"))
		{
			ddlScope.DataSource = dsOptions.Tables["Scope"];
			ddlScope.DataTextField = "SCOPE";
			ddlScope.DataValueField = "WTS_SCOPEID";
			ddlScope.DataBind();
		}
		if (dsOptions.Tables.Contains("Effort"))
		{
			ddlEffort.DataSource = dsOptions.Tables["Effort"];
			ddlEffort.DataTextField = "EFFORT";
			ddlEffort.DataValueField = "EFFORTID";
			ddlEffort.DataBind();
		}
		if (dsOptions.Tables.Contains("Priority"))
		{
			ddlPriority.DataSource = dsOptions.Tables["Priority"];
			ddlPriority.DataTextField = "PRIORITY";
			ddlPriority.DataValueField = "PRIORITYID";
			ddlPriority.DataBind();
		}
		if (dsOptions.Tables.Contains("User"))
		{
			item = null;
			string name = string.Empty;

			foreach (DataRow row in dsOptions.Tables["User"].Rows)
			{
				name = string.Format("{0} {1}", row["FIRST_NAME"].ToString(), row["LAST_NAME"].ToString());
				item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
				item.Attributes.Add("username", row["USERNAME"].ToString());
				item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());
				ddlSubmittedBy.Items.Add(item);

				if (this.WorkRequestID == 0)
				{
					item = ddlSubmittedBy.Items.FindByValue(UserManagement.GetUserId_FromUsername().ToString());
					if (item != null)
					{
						item.Selected = true;
					}
				}

				name = string.Format("{0} {1}", row["FIRST_NAME"].ToString(), row["LAST_NAME"].ToString());
				item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
				item.Attributes.Add("username", row["USERNAME"].ToString());
				item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());
				ddlSME.Items.Add(item);

				item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
				item.Attributes.Add("username", row["USERNAME"].ToString());
				item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());
				ddlLead_IA_TW.Items.Add(item);

				item = new ListItem(name, row["WTS_RESOURCEID"].ToString());
				item.Attributes.Add("username", row["USERNAME"].ToString());
				item.Attributes.Add("organizationID", row["ORGANIZATIONID"].ToString());
				ddlLeadResource.Items.Add(item);
			}
		}

	}

	private void loadWorkRequest()
	{
		DataTable dt = WorkRequest.WorkRequest_Get(workRequestID: this.WorkRequestID);

		if (dt != null && dt.Rows.Count > 0)
		{
			DataRow row = dt.Rows[0];
			ListItem item = null;

			this.spanWorkRequestNumber.InnerText = row["WORKREQUESTID"].ToString();
			this.txtTitle.Text = row["TITLE"].ToString();

			WTSUtility.SelectDdlItem(ddlContract, row["CONTRACTID"].ToString(), row["CONTRACT"].ToString());
			WTSUtility.SelectDdlItem(ddlOrganization, row["ORGANIZATIONID"].ToString(), row["ORGANIZATION"].ToString());

			textAreaDescription.InnerHtml = row["DESCRIPTION"].ToString();

			WTSUtility.SelectDdlItem(ddlRequestType, row["REQUESTTYPEID"].ToString(), row["REQUESTTYPE"].ToString());
			WTSUtility.SelectDdlItem(ddlRequestGroup, row["RequestGroupID"].ToString(), row["RequestGroup"].ToString());
			WTSUtility.SelectDdlItem(ddlScope, row["WTS_SCOPEID"].ToString(), row["SCOPE"].ToString());
			WTSUtility.SelectDdlItem(ddlEffort, row["EFFORTID"].ToString(), row["EFFORT"].ToString());
			WTSUtility.SelectDdlItem(ddlSubmittedBy, row["SUBMITTEDBY"].ToString(), row["Submitted_Name"].ToString());
			WTSUtility.SelectDdlItem(ddlSME, row["SMEID"].ToString(), row["SME"].ToString());
			WTSUtility.SelectDdlItem(ddlLead_IA_TW, row["LEAD_IA_TWID"].ToString(), row["Lead_IA_TW"].ToString());
			WTSUtility.SelectDdlItem(ddlLeadResource, row["LEAD_RESOURCEID"].ToString(), row["Lead_Resource"].ToString());
			WTSUtility.SelectDdlItem(ddlPriority, row["OP_PRIORITYID"].ToString(), row["PRIORITY"].ToString());

			textAreaJustification.InnerHtml = row["JUSTIFICATION"].ToString();

			this.labelCreated.Text = row["CREATEDBY"].ToString() + " - " + row["CREATEDDATE"].ToString();
			this.labelUpdated.Text = row["UPDATEDBY"].ToString() + " - " + row["UPDATEDDATE"].ToString();
		}
	}

	[WebMethod(true)]
	public static string SaveWorkRequest(object workRequest)
	{
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "id", "0" }, { "error", "" } };
		bool saved = false;
		int id = 0;
		string errorMsg = string.Empty;

		Dictionary<string, object> attributeList;

		if (workRequest == null)
		{
			saved = false;
			errorMsg = "No Work Request details were found.";
		}
		else
		{
			try
			{
				attributeList = (Dictionary<string, object>)workRequest;

				WorkRequest request = new WorkRequest(attributeList);

				if (request.WORKREQUESTID == 0) //save new item
				{
					int newID = 0;
					saved = WorkRequest.WorkRequest_Add(request, out newID, out errorMsg);
					id = newID;
				}
				else //update existing item
				{
					id = request.WORKREQUESTID;
					saved = WorkRequest.WorkRequest_Update(request, out errorMsg);
				}
			}
			catch (Exception ex)
			{
				saved = false;
				errorMsg = ex.Message;
				LogUtility.LogException(ex);
			}
		}

		result["saved"] = saved.ToString();
		result["id"] = id.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
	}
}