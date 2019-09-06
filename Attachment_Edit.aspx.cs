using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WTS.Enums;
using Newtonsoft.Json;


public partial class Attachment_Edit : System.Web.UI.Page
{
	protected int HasUploaded= 0;
	protected int AlreadyExists = 0;
	protected int WorkItemID = 0;
    protected int NewsID = 0;
    protected int workItemTaskID = 0;
	protected int WorkRequestID = 0;
    protected int RQMTDescriptionID = 0;
    protected int AORMeetingInstanceID = 0;
	protected int AttachmentID = 0;
    protected int AttachmentTypeID = 0;
    protected bool Edit = false;
	string OriginalFileName = string.Empty;
	protected string Module = "WorkItem";

    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeEvents();
        readQueryString();

        string description = "";
        string attachmentType = "";
        setupMultipleFileGrid();
        btnSubmit.Click += this.btnSubmit_Click;

        if (!IsPostBack)
        {

            //If we are adding a newsAttachment then we want dont want the user to be able to change it
            if (Module != "News")
            {
                DataTable dtAttachmentType = WTSData.AttachmentType_GetList();

                ddlAttachmentType.DataSource = dtAttachmentType;
                ddlAttachmentType.DataValueField = "ATTACHMENTTYPEID";
                ddlAttachmentType.DataTextField = "ATTACHMENTTYPE";
                ddlAttachmentType.DataBind();

                ListItem item = new ListItem("- Select -", "0");
                ddlAttachmentType.Items.Insert(0, item);

                if (Request.QueryString["attachmentType"] != null)
                {
                    attachmentType = Server.UrlDecode(Request.QueryString["attachmentType"]);
                }
                
                item = ddlAttachmentType.Items.FindByText(attachmentType);
                if (item != null)
                {
                    item.Selected = true;
                }  
                
            }

            if (Request.QueryString["description"] != null)
            {
                description = Server.UrlDecode(Request.QueryString["description"]);
            }
            txtDescription.Text = description;
            txtAttachmentID.Text = this.AttachmentID.ToString();
        }

    }

    private void readQueryString()
    {
        int id = 0;

        if (Request.QueryString["workItemID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["workItemID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["workItemID"].ToString()), out this.WorkItemID);
        }
        if (Request.QueryString["newsID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["newsID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["newsID"].ToString()), out this.NewsID);
        }
        if (Request.QueryString["taskID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["taskID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["taskID"].ToString()), out this.workItemTaskID);
        }
        if (Request.QueryString["workRequestID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["workRequestID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["workRequestID"].ToString()), out this.WorkRequestID);
        }

        if (Request.QueryString["AORMeetingInstanceID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingInstanceID"].ToString()))
        {
            Int32.TryParse(Request.QueryString["AORMeetingInstanceID"], out this.AORMeetingInstanceID);
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["RQMTDescription_ID"]))
        {
            RQMTDescriptionID = Int32.Parse(Request.QueryString["RQMTDescription_ID"]);
        }

        if (Request.QueryString["fileName"] != null)
        {
            OriginalFileName = Server.UrlDecode(Request.QueryString["fileName"]);
        }

        if (Request.QueryString["Module"] != null)
        {
            this.Module = Server.UrlDecode(Request.QueryString["Module"]);
        }
        if (Request.QueryString["attachmentID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["attachmentID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["attachmentID"].ToString()), out AttachmentID);
        }
        if (Request.QueryString["attachmentTypeID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["attachmentTypeID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["attachmentTypeID"].ToString()), out AttachmentTypeID);
        }

        if (Request.QueryString["edit"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["edit"]))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["edit"].ToString()), out id);
            this.Edit = (id > 0);
        }

    }

    private void InitializeEvents()
    {
        grdFiles.GridHeaderRowDataBound += grdFiles_GridHeaderRowDataBound;
        grdFiles.GridRowDataBound += grdFiles_GridRowDataBound;
    }

    void grdFiles_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
    }

    void grdFiles_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;

        row.Cells[1].Text = "<span style=\"color: red;\">*</span>&nbsp;Type";

        if (Module.ToUpper() == "MDIMAGE") row.Cells[1].Style["display"] = "none";
    }

    protected void btnSubmit_Click(Object sender, EventArgs e)
	{
		bool exists = false;
		int newAttachmentID = 0;
		int newModuleAttachmentID = 0;
		bool saved = false;
		string errorMsg = string.Empty;

		var dtfilesAttr = JsonConvert.DeserializeObject<DataTable>(inpHide.Value);

		if (this.Edit || (this.fileUpload1.HasFile))
		{
			
			if (!this.Edit)      //adding an attachment, not editing one
			{
				int i = 0;
				foreach (HttpPostedFile uploadedFile in fileUpload1.PostedFiles)
				{
					string fileName = "";
					string fileExtension = "";
					string strFileTypeID = "";
					string strDescription = "";
					int intAttachType = 0;
					Stream fStream = uploadedFile.InputStream;
					int fileLength = uploadedFile.ContentLength;
					byte[] fByte = new byte[fileLength];
					string fileType = uploadedFile.ContentType;
					string[] splitFileName = fileName.Split('\\');
					fileName = splitFileName[splitFileName.Length - 1];

					fStream.Read(fByte, 0, fByte.Length);
					fStream.Close();

					if (fileUpload1.PostedFiles.Count > 1)
					{
						fileName = dtfilesAttr.Rows[i]["name"].ToString();
						strFileTypeID = dtfilesAttr.Rows[i]["type"].ToString();
						int.TryParse(strFileTypeID, out intAttachType);
						strDescription = dtfilesAttr.Rows[i]["description"].ToString();
					}
					else
					{
						fileName= uploadedFile.FileName;
						strFileTypeID = ddlAttachmentType.SelectedValue;
						int.TryParse(ddlAttachmentType.SelectedValue, out intAttachType);
						strDescription = txtDescription.Text;
					}

					splitFileName = fileName.Split('\\');
					fileName = splitFileName[splitFileName.Length - 1];
					splitFileName = fileName.Split('.');
					fileExtension = splitFileName[splitFileName.Length - 1];

					switch (Module.ToUpper())
					{
						case "TASK":
							saved = WorkItem_Task.Attachment_Add(WorkItemTaskID: this.workItemTaskID
								, attachmentTypeID: intAttachType
								, fileName: fileName
								, title: strDescription
								, description: strDescription
								, fileData: fByte
								, extensionID: 0
								, newAttachmentID: out newAttachmentID
								, newWorkItemAttachmentID: out newModuleAttachmentID
								, errorMsg: out errorMsg);
							break;
						case "WORKITEM":
							saved = WorkloadItem.Attachment_Add(workItemID: this.WorkItemID
								, attachmentTypeID: intAttachType
								, fileName: fileName
								, title: strDescription
								, description: strDescription
								, fileData: fByte
								, extensionID: 0
								, newAttachmentID: out newAttachmentID
								, newWorkItemAttachmentID: out newModuleAttachmentID
								, errorMsg: out errorMsg);
							break;
						case "WORKREQUEST":
							saved = WorkRequest.Attachment_Add(workRequestID: this.WorkRequestID
								, attachmentTypeID: intAttachType
								, fileName: fileName
								, title: strDescription
								, description: strDescription
								, fileData: fByte
								, extensionID: 0
								, newAttachmentID: out newAttachmentID
								, newWorkItemAttachmentID: out newModuleAttachmentID
								, errorMsg: out errorMsg);
							break;
                        case "MEETING":
                            saved = AOR.AORMeetingInstanceAttachment_Save(0, AORMeetingInstanceID, 0, intAttachType, fileName, strDescription, strDescription, fByte, out newAttachmentID);
                            break;
                        case "RQMTDESC":
                            saved = RQMT.RQMTDescriptionAttachment_Save(0, RQMTDescriptionID, 0, intAttachType, fileName, strDescription, fByte, out newAttachmentID);
                            break;
                        case "MDIMAGE":
                            Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
                            string[] allowedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                            string extension = Path.GetExtension(fileName).ToLower();

                            if (!allowedExtensions.Contains(extension)) continue;
                            
                            result = MasterData.Image_Add(ImageName: strDescription, Description: strDescription, FileName: fileName, FileData: fByte);
                            saved = result["saved"].ToUpper() == "TRUE";
                            errorMsg = result["error"];
                            break;
                        case "NEWS":
                            saved = WTSNews.NewsAttachment_Add(newsID: this.NewsID
                                , attachmentTypeID: (int)AttachmentTypeEnum.News
                                , fileName: fileName
                                , title: strDescription
                                , description: strDescription
                                , fileData: fByte
                                , extensionID: 0
                                , newAttachmentID: out newAttachmentID
                                //, newWorkItemAttachmentID: out newModuleAttachmentID
                                , errorMsg: out errorMsg);
                            break;
                        default:
							saved = WTSData.Attachment_Add(attachmentTypeID: intAttachType
								, fileName: fileName
								, title: strDescription
								, description: strDescription
								, fileData: fByte
								, extensionID: 0
								, newID: out newAttachmentID
								, errorMsg: out errorMsg);
							break;
					}

					i += 1;
				}
			}
			else
			{
				string fileName = "";
				fileName = fileUpload1.FileName;
				
				//Stream fStream = fileUpload1.PostedFile.InputStream;
				//int fileLength = fileUpload1.PostedFile.ContentLength;
				
				//byte[] fByte = new byte[fileLength];
				
				string fileType = string.Empty;
				if(fileUpload1.PostedFile != null)
				{
					fileName = fileUpload1.PostedFile.FileName;
					fileType = fileUpload1.PostedFile.ContentType;
				}
				string[] splitFileName = fileName.Split('\\');
				fileName = splitFileName[splitFileName.Length - 1];
				splitFileName = fileName.Split('.');
				
				string fileExtension = splitFileName[splitFileName.Length - 1];
				//fStream.Read(fByte, 0, fByte.Length);
				//fStream.Close();

				if (fileName == "")
				{
					fileName = this.OriginalFileName;
				}

                string attachmentType = ddlAttachmentType.SelectedValue;
                if (Module == "News")
                {
                    attachmentType = ((int)AttachmentTypeEnum.News).ToString();
                }

                saved = WTSData.Attachment_Update(attachmentID: this.AttachmentID
					, attachmentTypeID: int.Parse(attachmentType)
					, fileName: fileName
					, title: txtDescription.Text
					, description: txtDescription.Text
					//, fileData: fByte
					, exists: out exists
					, errorMsg: out errorMsg);

			}

            if (!saved || errorMsg.Length > 0)
            {
                lblError.Text = errorMsg;
            }
            else
            {
                if (!exists)
                {
                    HasUploaded = 1;
                }
                else
                {
                    AlreadyExists = 1;
                }

                if (Module.ToUpper() == "WORKITEM") Workload.SendWorkloadEmail("WorkItem", false, this.WorkItemID);
            }
		}
	}

	private void setupMultipleFileGrid(){
		DataTable dt = new DataTable();
		dt.Columns.Add("File Name");
		dt.Columns.Add("Type");
		dt.Columns.Add("Size");
		dt.Columns.Add("Description");
		
		DataRow dr = dt.NewRow();
		dr["File Name"] = "0";
		dr["Type"] = "0";
		dr["Size"] = "0";
		dt.Rows.Add(dr);

		grdFiles.DataSource = dt;
		grdFiles.DataBind();
	}
}