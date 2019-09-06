using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using WTS.Enums;

public partial class NewsAttachments : System.Web.UI.Page
{
    protected int _newsID = -1;
    //protected int _newsTypeID = 0;
    protected DataColumnCollection dc;


    protected void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeControls();
        loadAttachments();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["newsId"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["newsId"]))
        {
            int.TryParse(Request.QueryString["newsId"], out this._newsID);
        }
    }


    private void loadAttachments()
    {
        DataTable dt = WTSNews.News_GetAttachmentList(newsID: this._newsID, showArchived: 0);

        if (dt != null)
        {
            dc = dt.Columns;

            if (dt.Rows.Count > 0)
            {
                //spanRowCount.InnerText = dt.Rows.Count.ToString();
                gridAttachments.DataSource = dt;
                gridAttachments.DataBind();
            }
        }
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
    private void InitializeControls()
    {
        gridAttachments.GridHeaderRowDataBound += gridAttachments_GridHeaderRowDataBound;
        gridAttachments.GridRowDataBound += grdAttachments_GridRowDataBound;

    }

    [WebMethod()]
    public static string DeleteAttachment(int newsAttachmentId)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "deleted", "" }, { "id", "" }, { "error", "" } };

        bool deleted = false;
        string errorMsg = string.Empty, ids = string.Empty;

        try
        {
            deleted = WTSNews.DeleteNewsAttachment(newsAttachmentId: newsAttachmentId);
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            errorMsg += ": " + ex.Message;
            deleted = false;
        }

        result["deleted"] = "true";
        //result["id"] = id.ToString();
        //result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }


}