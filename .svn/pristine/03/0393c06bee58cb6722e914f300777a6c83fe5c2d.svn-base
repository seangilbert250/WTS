using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

public partial class SR_Add : System.Web.UI.Page
{
    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        InitializeEvents();

        if (!Page.IsPostBack)
        {
            LoadControls();
            LoadData();
        }
    }

    private void InitializeEvents()
    {
        btnSubmit.Click += btnSubmit_Click;
    }

    protected void btnSubmit_Click(Object sender, EventArgs e)
    {
        Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };

        try
        {
            int submittedBy_ID = 0, srType_ID = 0, priority_ID = 0;

            int.TryParse(ddlSubmittedBy.SelectedValue, out submittedBy_ID);
            int.TryParse(ddlType.SelectedValue, out srType_ID);
            int.TryParse(ddlPriority.SelectedValue, out priority_ID);
            
            result = SR.SR_Add(SubmittedByID: submittedBy_ID, SRTypeID: srType_ID, PriorityID: priority_ID, Description: txtDescription.Text);

            if (result["saved"] == "True")
            {
                int sr_ID = 0;

                int.TryParse(result["newID"], out sr_ID);

                if (sr_ID != 0)
                {
                    if (fileUpload.HasFiles)
                    {
                        foreach (var file in fileUpload.PostedFiles)
                        {
                            Stream fileStream = file.InputStream;
                            byte[] fileData = new byte[file.ContentLength];
                            string fileName = file.FileName;
                            string[] splitFileName = fileName.Split('\\');

                            fileName = splitFileName[splitFileName.Length - 1];

                            fileStream.Read(fileData, 0, fileData.Length);
                            fileStream.Close();

                            result = SR.SRAttachment_Add(SRID: sr_ID, FileName: fileName, FileData: fileData);
                        }
                    }

                    SR.SR_Email(SRID: sr_ID);
                }
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

        ScriptManager.RegisterStartupScript(this, this.GetType(), "complete", "<script type=\"text/javascript\">complete('" + JsonConvert.SerializeObject(result) + "');</script>", false);
    }
    #endregion

    #region Data
    private void LoadControls()
	{
        DataTable dtResource = MasterData.WTS_Resource_Get(includeArchive: false);

        ddlSubmittedBy.DataSource = dtResource;
        ddlSubmittedBy.DataValueField = "WTS_RESOURCEID";
        ddlSubmittedBy.DataTextField = "USERNAME";
        ddlSubmittedBy.DataBind();

        DataTable dtType = MasterData.SRTypeList_Get(includeArchive: false);

        if (dtType != null && dtType.Rows.Count > 0)
        {
            dtType.Columns["SRType"].ReadOnly = false;
            dtType.Rows[0]["SRType"] = "-Select-";
        }

        ddlType.DataSource = dtType;
        ddlType.DataValueField = "SRTypeID";
        ddlType.DataTextField = "SRType";
        ddlType.DataBind();

        DataTable dtPriority = MasterData.PriorityList_Get(includeArchive: false);

        dtPriority.DefaultView.RowFilter = "PRIORITYTYPE IN ('', 'SR')";
        dtPriority = dtPriority.DefaultView.ToTable();

        if (dtPriority != null && dtPriority.Rows.Count > 0)
        {
            dtPriority.Columns["Priority"].ReadOnly = false;
            dtPriority.Rows[0]["Priority"] = "-Select-";
        }

        ddlPriority.DataSource = dtPriority;
        ddlPriority.DataValueField = "PriorityID";
        ddlPriority.DataTextField = "Priority";
        ddlPriority.DataBind();
    }

    private void LoadData()
    {
        ddlSubmittedBy.SelectedValue = UserManagement.GetUserId_FromUsername().ToString();
    }
    #endregion
}