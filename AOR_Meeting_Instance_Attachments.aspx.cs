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


public partial class AOR_Meeting_Instance_Attachments : System.Web.UI.Page
{
    protected int AORMeetingID = 0;
    protected int AORMeetingInstanceID = 0;

    protected bool CanEdit = false;
    protected bool ShowAll = false;

    protected DataColumnCollection dc;
    protected int Attachment_Count = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.WorkItem);

        readQueryString();
        initControls();

        loadAttachments();
    }

    private void readQueryString()
    {
        if (Request.QueryString["AORMeetingID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["AORMeetingID"].ToString()), out this.AORMeetingID);
        }

        if (Request.QueryString["AORMeetingInstanceID"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["AORMeetingInstanceID"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["AORMeetingInstanceID"].ToString()), out this.AORMeetingInstanceID);
        }

        if (Request.QueryString["ShowAll"] != null)
        {
            ShowAll = Request.QueryString["ShowAll"].ToLower() == "true";
        }
    }

    private void loadAttachments()
    {
        if (ShowAll && AORMeetingID == 0)
        {
            // LOAD AOR MEETING ID FROM INSTANCE (NOT IMPLEMENTED YET)
        }

        DataTable dt = AOR.AORMeetingInstanceAttachment_Get(0, ShowAll ? AORMeetingID : 0, AORMeetingInstanceID, 0, false);
        if (dt != null && dt.Rows.Count > 0)
        {
            Attachment_Count = dt.Rows.Count;
        }

        if (dt != null)
        {
            dc = dt.Columns;

            if (!ShowAll)
            {
                dt.Columns.Remove("InstanceDate");
            }

            if (dt.Rows.Count > 0)
            {
                spanRowCount.InnerText = dt.Rows.Count.ToString();
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

        string columns = "AttachmentID=Download,FileName=File Name,CREATEDBY=Created By,CREATEDDATE=Created Date,UPDATEDBY=Updated By,UPDATEDDATE=Updated Date,ATTACHMENTTYPE=Attachment Type,AttachmentTypeId=Edit";
        if (ShowAll)
        {
            columns += ",InstanceDate=Instance Date";
        }

        Array.ForEach(columns.Split(','),
            col => row.Cells[dc[col.Split('=')[0]].Ordinal].Text = col.Split('=')[1]);
    }

    void grdAttachments_GridRowDataBound(Object sender, GridViewRowEventArgs e)
    {
        GridViewRow row = e.Row;
        formatGridCols(row);

        string id = row.Cells[dc["AttachmentID"].Ordinal].Text;
        row.Attributes.Add("AttachmentID", id);
        string fileName = row.Cells[dc["FILENAME"].Ordinal].Text;
        string attachmentType = row.Cells[dc["AttachmentType"].Ordinal].Text;

        string meetingInstanceID = row.Cells[dc["AORMeetingInstanceID"].Ordinal].Text;

        row.Style["cursor"] = "pointer";

        row.Cells[dc["AttachmentID"].Ordinal].Controls.Add(CreateDownloadButton(attachmentId: id, fileName: fileName));

        if (meetingInstanceID == AORMeetingInstanceID.ToString())
        {
            if (attachmentType != "MEETING MINUTES")
            {
                row.Cells[dc["AttachmentTypeId"].Ordinal].Controls.Add(CreateEditButton(attachmentId: id, fileName: fileName));
            }
            else
            {
                row.Cells[dc["AttachmentTypeId"].Ordinal].Text = "";
            }
        }
        else
        {
            row.Cells[dc["AttachmentTypeId"].Ordinal].Text = "";
        }

        //row.Cells[dc["FileName"].Ordinal].Controls.Add(CreateEditButton(id, fileName));
        //File type icon
        //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Controls.Add(CreateFileTypeIcon(row.Cells[dc["FILE_ICONID"].Ordinal].Text, row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Text, "ATTACHMENTS.TBL_FILE_ICON", "ICON_OBJ"));
        //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Style["text-align"] = "center";
        //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Style["padding"] = "5px 5px 3px 5px";
    }

    void formatGridCols(GridViewRow row)
    {
        Array.ForEach("AORMeetingInstanceAttachmentID,AORMeetingInstanceID,Archive,FileData,ExtensionID,BUGTRACKER_ID".Split(','), col => row.Cells[dc[col].Ordinal].Style["display"] = "none");
        foreach (TableCell cell in row.Cells) cell.Style["text-align"] = "left";
        Array.ForEach("AttachmentID,AttachmentTypeId,Archive,CREATEDBY,CREATEDDATE,UPDATEDBY,UPDATEDDATE,ATTACHMENTTYPE".Split(','), col => row.Cells[dc[col].Ordinal].Style["text-align"] = "center");

    }

    protected Image CreateDownloadButton(string attachmentId, string fileName)
    {
        Image imgDownload = new Image();

        imgDownload.ID = string.Format("imgDownload_{0}", attachmentId);
        imgDownload.Height = 14;
        imgDownload.Width = 14;
        imgDownload.ToolTip = string.Format("Download Attachment - [{0}]", fileName);
        imgDownload.AlternateText = string.Format("Download Attachment - [{0}]", fileName);
        imgDownload.Style["cursor"] = "pointer";
        imgDownload.ImageUrl = "Images/Icons/Download.png";
        imgDownload.Attributes.Add("onclick", string.Format("buttonDownload_onclick({0})", attachmentId));

        return imgDownload;
    }

    protected Image CreateEditButton(string attachmentId, string fileName)
    {
        Image imgEdit = new Image();

        imgEdit.ID = string.Format("imgEdit_{0}", attachmentId);
        imgEdit.Height = 14;
        imgEdit.Width = 14;
        imgEdit.ToolTip = string.Format("Edit Attachment - [{0}]", fileName);
        imgEdit.AlternateText = string.Format("Edit Attachment - [{0}]", fileName);
        imgEdit.Style["cursor"] = "pointer";
        imgEdit.ImageUrl = "Images/Icons/Pencil.png";
        imgEdit.Attributes.Add("onclick", string.Format("lbEditAttachment_click({0})", attachmentId));

        return imgEdit;
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

    #endregion Grid


    [WebMethod()]
    public static string DeleteAttachment(int meetingInstanceID, int id)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "id", "" }, { "error", "" } };

        bool deleted = false;
        string errorMsg = string.Empty, ids = string.Empty;

        try
        {
            deleted = AOR.AORMeetingInstanceAttachment_Delete(0, 0, id);
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