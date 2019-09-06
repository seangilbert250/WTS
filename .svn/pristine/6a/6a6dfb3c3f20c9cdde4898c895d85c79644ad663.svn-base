using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;


public partial class Task_Attachments : System.Web.UI.Page
{
    protected int taskID = 0;

    protected bool CanEdit = false;
    protected bool ReadOnly = false;

    protected DataColumnCollection dc;
    protected int Attachment_Count = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);

        readQueryString();

        if (ReadOnly)
        {
            this.CanEdit = false;
        }

        initControls();

        loadAttachments();
    }

    private void readQueryString()
    {
        if (Request.QueryString["taskID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["taskID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["taskID"].ToString()), out this.taskID);
        }

        if (Request.QueryString["ReadOnly"] != null)
        {
            ReadOnly = Request.QueryString["ReadOnly"].ToLower() == "true" || Request.QueryString["ReadOnly"] == "1";
        }
    }

    private void loadAttachments()
    {
        WorkItem_Task task = new WorkItem_Task(taskID: this.taskID);

        if (task == null || !task.Load())
        {
            return;
        }

        txtWorkloadNumber.Text = string.Format("{0} - {1}", task.WorkItemID.ToString(), task.Task_Number.ToString());
        txtTitle.Text = HttpUtility.HtmlDecode(Uri.UnescapeDataString(task.Title.Replace("&nbsp;", "").Trim()));

        DataTable dt = WorkItem_Task.WorkItem_Task_GetAttachmentList(WorkItemTaskID: this.taskID);
        if (dt != null && dt.Rows.Count > 0)
        {
            Attachment_Count = dt.Rows.Count;
        }
        if (dt != null)
        {
            dc = dt.Columns;

            if (dt.Rows.Count > 0)
            {
                gridAttachments.DataSource = dt;
                gridAttachments.DataBind();
            }
        }
    }


    #region Grid

    private void initControls()
    {
        gridAttachments.GridHeaderRowDataBound += gridAttachments_GridHeaderRowDataBound;
        gridAttachments.GridRowDataBound += grdAttachments_GridRowDataBound;
    }
    void gridAttachments_GridHeaderRowDataBound(Object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        int numCell = e.Row.Cells.Count;

        formatGridCols(row);

        for (int i = 0; i < numCell; i++)
        {
            row.Cells[i].Font.Bold = true;
            row.Cells[i].Style["font-family"] = "Arial";
            row.Cells[i].Text = e.Row.Cells[i].Text.Replace("_", " ");
        }

        //e.Row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Text = "&nbsp;";
        row.Cells[dc["AttachmentID"].Ordinal].Text = "Download";
        row.Cells[dc["FILENAME"].Ordinal].Text = "Edit";
        row.Cells[dc["Title"].Ordinal].Text = "Title";
        row.Cells[dc["ATTACHMENTTYPE"].Ordinal].Text = "Attachment Type";
        row.Cells[dc["CREATEDBY"].Ordinal].Text = "Uploaded By";
        row.Cells[dc["CREATEDDATE"].Ordinal].Text = "Uploaded On";
    }

    void grdAttachments_GridRowDataBound(Object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        formatGridCols(row);

        string id = row.Cells[dc["AttachmentID"].Ordinal].Text;
        row.Attributes.Add("AttachmentID", id);
        string fileName = row.Cells[dc["FILENAME"].Ordinal].Text;

        row.Style["cursor"] = "pointer";
        row.Style["text-align"] = "left";

        row.Cells[dc["AttachmentID"].Ordinal].Controls.Add(CreateDownloadButton(attachmentId: id, filename: fileName));

        row.Cells[dc["FileName"].Ordinal].Controls.Add(CreateEditButton(id, fileName));
        //File type icon
        //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Controls.Add(CreateFileTypeIcon(row.Cells[dc["FILE_ICONID"].Ordinal].Text, row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Text, "ATTACHMENTS.TBL_FILE_ICON", "ICON_OBJ"));
        //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Style["text-align"] = "center";
        //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Style["padding"] = "5px 5px 3px 5px";
    }

    void formatGridCols(GridViewRow row)
    {
        row.Cells[dc["ATTACHMENTTYPEID"].Ordinal].Style["display"] = "none";
        //row.Cells[dc["FILE_ICONID"].Ordinal].Style["display"] = "none";
        //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Style["width"] = "5px";

        row.Cells[dc["AttachmentId"].Ordinal].Style["width"] = "65px";
        row.Cells[dc["AttachmentId"].Ordinal].Style["text-align"] = "center";

        row.Cells[dc["ATTACHMENTTYPE"].Ordinal].Style["width"] = "200px";
        row.Cells[dc["ATTACHMENTTYPE"].Ordinal].Style["text-align"] = "left";
        row.Cells[dc["ATTACHMENTTYPE"].Ordinal].Style["padding-left"] = "3px";

        row.Cells[dc["FileName"].Ordinal].Style["width"] = "200px";
        row.Cells[dc["FileName"].Ordinal].Style["text-align"] = "left";
        row.Cells[dc["FileName"].Ordinal].Style["padding-left"] = "3px";

        row.Cells[dc["Title"].Ordinal].Style["text-align"] = "left";
        row.Cells[dc["Title"].Ordinal].Style["padding-left"] = "3px";
        row.Cells[dc["Description"].Ordinal].Style["text-align"] = "left";
        row.Cells[dc["Description"].Ordinal].Style["padding-left"] = "3px";
        row.Cells[dc["CreatedBy"].Ordinal].Style["width"] = "110px";
        row.Cells[dc["CreatedBy"].Ordinal].Style["text-align"] = "left";
        row.Cells[dc["CreatedBy"].Ordinal].Style["padding-left"] = "3px";
        row.Cells[dc["CreatedDate"].Ordinal].Style["width"] = "85px";
        row.Cells[dc["CreatedDate"].Ordinal].Style["text-align"] = "left";
        row.Cells[dc["CreatedDate"].Ordinal].Style["padding-left"] = "3px";
    }

    protected Image CreateDownloadButton(string attachmentId, string filename)
    {
        Image imgDownload = new Image();

        imgDownload.ID = string.Format("imgDownload_{0}", attachmentId);
        imgDownload.Height = 14;
        imgDownload.Width = 14;
        imgDownload.ToolTip = string.Format("Download Attachment - [{0}]", filename);
        imgDownload.AlternateText = string.Format("Download Attachment - [{0}]", filename);
        imgDownload.Style["cursor"] = "pointer";
        imgDownload.ImageUrl = "Images/Icons/Download.png";
        imgDownload.Attributes.Add("onclick", string.Format("buttonDownload_onclick({0})", attachmentId));

        return imgDownload;
    }

    protected Image CreateFileTypeIcon(string fileTypeId, string fileTypeNm, string strTableNm, string strColumnNm)
    {
        try
        {
            Image imgIcon = new Image();
            imgIcon.Attributes.Add("alt", fileTypeNm);
            imgIcon.ImageUrl = String.Format("ImageHandler.ashx?id={0}&tableName={1}&columnName={2}", fileTypeId, strTableNm, strColumnNm);
            imgIcon.Attributes.Add("onclick", "buttonDownload_onclick()");
            return imgIcon;
        }
        catch (Exception e)
        {
            LogUtility.LogException(e);
            return null;
        }
    }

    private LinkButton CreateEditButton(string attachmentId, string filename)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("lbEditAttachment_click('{0}');return false;", attachmentId);

        LinkButton lb = new LinkButton();
        lb.ID = string.Format("lbEditAttachment_{0}", attachmentId);
        lb.Attributes["name"] = string.Format("lbEditAttachment_{0}", attachmentId);
        lb.ToolTip = string.Format("Edit Attachment [{0}]", filename);
        lb.Text = filename;
        lb.Attributes.Add("onclick", sb.ToString());

        return lb;
    }

    #endregion Grid


    [WebMethod()]
    public static string DeleteAttachment(int WorkItemTaskID, int id)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "id", "" }, { "error", "" } };

        bool deleted = false;
        string errorMsg = string.Empty, ids = string.Empty;

        try
        {
            deleted = WorkItem_Task.Attachment_Delete(WorkItemTaskID: WorkItemTaskID, attachmentID: id, errorMsg: out errorMsg);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            errorMsg += ": " + ex.Message;
            deleted = false;
        }

        result["deleted"] = deleted.ToString();
        result["id"] = id.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }
}