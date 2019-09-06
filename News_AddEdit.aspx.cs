using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using WTS.Enums;

public partial class News_AddEdit : System.Web.UI.Page
{
    protected int _newsID = -1;
    protected int _newsTypeID = 0;
    protected string _editType = "";
    protected string _attachmentID = "";
    protected string JsonSeverAttributes;
    protected DataColumnCollection dc;

    protected void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();
        InitializeControls();

        //If _newsID != -1 then we are editing a news entry
        if (_newsID != -1)
        {
            //Gather Datatable with NewsID
            DataTable dt = new DataTable();
            dt = WTSNews.GetNews(newsId: _newsID);

            if (dt.Rows.Count > 0)
            {
                Dictionary<string, object> SeverAttributes = new Dictionary<string, object>();
                //Set Control Values
                //SeverAttributes.Add("txtArticleTitle", HttpUtility.UrlDecodeToBytes(dt.Rows[1]["Summary"].ToString()));
                SeverAttributes.Add("txtArticleTitle", Server.HtmlEncode(dt.Rows[1]["Summary"].ToString()));
                SeverAttributes.Add("textNewsBody", Server.HtmlEncode(dt.Rows[1]["Detail"].ToString()));
               // SeverAttributes.Add("textNewsBody1", HttpUtility.UrlEncodeToBytes(dt.Rows[1]["Detail"].ToString()));
                //SeverAttributes.Add("textNewsBody2", Server.HtmlEncode(dt.Rows[1]["Detail"].ToString()));
                //SeverAttributes.Add("textNewsBody3", Server.UrlEncode(dt.Rows[1]["Detail"].ToString()));


                //SeverAttributes.Add("fileUpload", Server.HtmlEncode(dt.Rows[1]["FileName"].ToString()));
                SeverAttributes.Add("ddlNotificationType", dt.Rows[1]["NewsType"].ToString());
                SeverAttributes.Add("txtDateStart", dt.Rows[1]["Start_Date"].ToString());
                SeverAttributes.Add("txtDateEnd", dt.Rows[1]["End_Date"].ToString());
                if (dt.Rows[1]["Bln_Active"].ToString() == "1")
                {
                    SeverAttributes.Add("Bln_Active", "true");
                }
                else
                {
                    SeverAttributes.Add("Bln_Active", "false");
                }
                JsonSeverAttributes = JsonConvert.SerializeObject(SeverAttributes);
            }
            //loadAttachments();
        }
    }

    private void InitializeControls()
    {
        if (!IsPostBack)
        {
            //gridAttachments.GridHeaderRowDataBound += gridAttachments_GridHeaderRowDataBound;
            //gridAttachments.GridRowDataBound += grdAttachments_GridRowDataBound;
            ddlNotificationType.Items.Insert(0, new ListItem("Sliding System Notification", "1"));
            ddlNotificationType.Items.Insert(1, new ListItem("News Article", "2"));
            ddlNotificationType.Items.Insert(2, new ListItem("News Overview", "3"));
            ddlNotificationType.SelectedValue = this._newsTypeID.ToString();
            UpdatePanel1.Update();
        }
    }

    //private void loadAttachments()
    //{
    //    DataTable dt = WTSNews.News_GetAttachmentList(newsID: this._newsID);

    //    if (dt != null)
    //    {
    //        dc = dt.Columns;

    //        if (dt.Rows.Count > 0)
    //        {
    //            //spanRowCount.InnerText = dt.Rows.Count.ToString();
    //            //gridAttachments.DataSource = dt;
    //            //gridAttachments.DataBind();
    //        }
    //    }
    //}

    //void gridAttachments_GridHeaderRowDataBound(Object sender, GridViewRowEventArgs e)
    //{
    //    GridViewRow row = e.Row;
    //    int numCell = e.Row.Cells.Count;

    //    formatGridCols(row);

    //    for (int i = 0; i < numCell; i++)
    //    {
    //        row.Cells[i].Font.Bold = true;
    //        row.Cells[i].Style["font-family"] = "Arial";
    //        row.Cells[i].Text = e.Row.Cells[i].Text.Replace("_", " ");
    //    }

    //    //e.Row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Text = "&nbsp;";
    //    row.Cells[dc["AttachmentID"].Ordinal].Text = "Download";
    //    row.Cells[dc["FILENAME"].Ordinal].Text = "Edit";
    //    row.Cells[dc["Title"].Ordinal].Text = "Title";
    //    row.Cells[dc["ATTACHMENTTYPE"].Ordinal].Text = "Attachment Type";
    //    row.Cells[dc["CREATEDBY"].Ordinal].Text = "Uploaded By";
    //    row.Cells[dc["CREATEDDATE"].Ordinal].Text = "Uploaded On";
    //}

    //void formatGridCols(GridViewRow row)
    //{
    //    row.Cells[dc["ATTACHMENTTYPEID"].Ordinal].Style["display"] = "none";
    //    //row.Cells[dc["FILE_ICONID"].Ordinal].Style["display"] = "none";
    //    //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Style["width"] = "5px";

    //    row.Cells[dc["AttachmentId"].Ordinal].Style["width"] = "65px";
    //    row.Cells[dc["AttachmentId"].Ordinal].Style["text-align"] = "center";

    //    row.Cells[dc["ATTACHMENTTYPE"].Ordinal].Style["width"] = "200px";
    //    row.Cells[dc["ATTACHMENTTYPE"].Ordinal].Style["text-align"] = "left";
    //    row.Cells[dc["ATTACHMENTTYPE"].Ordinal].Style["padding-left"] = "3px";

    //    row.Cells[dc["FileName"].Ordinal].Style["width"] = "200px";
    //    row.Cells[dc["FileName"].Ordinal].Style["text-align"] = "left";
    //    row.Cells[dc["FileName"].Ordinal].Style["padding-left"] = "3px";

    //    row.Cells[dc["Title"].Ordinal].Style["text-align"] = "left";
    //    row.Cells[dc["Title"].Ordinal].Style["padding-left"] = "3px";
    //    row.Cells[dc["Description"].Ordinal].Style["text-align"] = "left";
    //    row.Cells[dc["Description"].Ordinal].Style["padding-left"] = "3px";
    //    row.Cells[dc["CreatedBy"].Ordinal].Style["width"] = "110px";
    //    row.Cells[dc["CreatedBy"].Ordinal].Style["text-align"] = "left";
    //    row.Cells[dc["CreatedBy"].Ordinal].Style["padding-left"] = "3px";
    //    row.Cells[dc["CreatedDate"].Ordinal].Style["width"] = "85px";
    //    row.Cells[dc["CreatedDate"].Ordinal].Style["text-align"] = "left";
    //    row.Cells[dc["CreatedDate"].Ordinal].Style["padding-left"] = "3px";
    //}

    //void grdAttachments_GridRowDataBound(Object sender, GridViewRowEventArgs e)
    //{
    //    GridViewRow row = e.Row;
    //    formatGridCols(row);

    //    string id = row.Cells[dc["AttachmentID"].Ordinal].Text;
    //    row.Attributes.Add("AttachmentID", id);
    //    string fileName = row.Cells[dc["FILENAME"].Ordinal].Text;

    //    row.Style["cursor"] = "pointer";
    //    row.Style["text-align"] = "left";

    //    row.Cells[dc["AttachmentID"].Ordinal].Controls.Add(CreateDownloadButton(attachmentId: id, filename: fileName));

    //    row.Cells[dc["FileName"].Ordinal].Controls.Add(CreateEditButton(id, fileName));
    //    //File type icon
    //    //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Controls.Add(CreateFileTypeIcon(row.Cells[dc["FILE_ICONID"].Ordinal].Text, row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Text, "ATTACHMENTS.TBL_FILE_ICON", "ICON_OBJ"));
    //    //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Style["text-align"] = "center";
    //    //row.Cells[dc["TBL_EXTENSION_NM"].Ordinal].Style["padding"] = "5px 5px 3px 5px";
    //}

    //protected Image CreateDownloadButton(string attachmentId, string filename)
    //{
    //    Image imgDownload = new Image();

    //    imgDownload.ID = string.Format("imgDownload_{0}", attachmentId);
    //    imgDownload.Height = 14;
    //    imgDownload.Width = 14;
    //    imgDownload.ToolTip = string.Format("Download Attachment - [{0}]", filename);
    //    imgDownload.AlternateText = string.Format("Download Attachment - [{0}]", filename);
    //    imgDownload.Style["cursor"] = "pointer";
    //    imgDownload.ImageUrl = "Images/Icons/Download.png";
    //    imgDownload.Attributes.Add("onclick", string.Format("buttonDownload_onclick({0})", attachmentId));

    //    return imgDownload;
    //}

    //private LinkButton CreateEditButton(string attachmentId, string filename)
    //{
    //    StringBuilder sb = new StringBuilder();
    //    sb.AppendFormat("lbEditAttachment_click('{0}');return false;", attachmentId);

    //    LinkButton lb = new LinkButton();
    //    lb.ID = string.Format("lbEditAttachment_{0}", attachmentId);
    //    lb.Attributes["name"] = string.Format("lbEditAttachment_{0}", attachmentId);
    //    lb.ToolTip = string.Format("Edit Attachment [{0}]", filename);
    //    lb.Text = filename;
    //    lb.Attributes.Add("onclick", sb.ToString());

    //    return lb;
    //}

    private void ReadQueryString()
    {
        if (Request.QueryString["newsTypeId"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["newsTypeId"]))
        {
            int.TryParse(Request.QueryString["newsTypeId"], out this._newsTypeID);
        }
        if (Request.QueryString["newsID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["newsID"]))
        {
            int.TryParse(Request.QueryString["newsID"], out this._newsID);
        }
        if (Request.QueryString["editType"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["editType"]))
        {
            _editType = Request.QueryString["editType"].ToString();
        }
        if (Request.QueryString["attachmentID"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["AttachmentID"]))
        {
            _attachmentID = Request.QueryString["attachmentID"].ToString();
        }

    }

    private void InitializeEvents()
    {
        btnSubmit.Click += btnSubmit_Click;
    }



    protected void btnSubmit_Click(Object sender, EventArgs e)
    {

        Dictionary<string, string> result = new Dictionary<string, string> { { "saved", "false" }, { "exists", "false" }, { "newID", "0" }, { "error", "" } };
        bool saved = false;//, result = false;
        int newAttachmentID = 0;
        string errorMsg = "";

        Dictionary<string, object> attributeList = new Dictionary<string, object>();
        attributeList.Add("NotificationType", 1);
        attributeList.Add("StartDate", txtDateStart.Text);
        attributeList.Add("EndDate", txtDateEnd.Text);
        attributeList.Add("Active", cbActive.Checked.ToString());

        //if (_newsTypeID == (int)NewsTypeEnum.NewsArticle)
        //{
        attributeList.Add("ArticleTitle", Server.UrlDecode(txtArticleTitle.Text));
        attributeList.Add("Description", Server.UrlDecode(textNewsBody.Value));
        //attributeList.Add("Description", "");
        //}
        //else
        //{
        //    //The newsOverviews have an autogenerated Description & Title
        //    attributeList.Add("ArticleTitle", "Weekly Overview");
        //    attributeList.Add("Description", "Weekly Overview For: " + txtDateStart.Text + " TO " + txtDateEnd.Text);
        //}

        try
        {
            WTSNewsArticle item = new WTSNewsArticle(attributeList);
            item.Description = Server.HtmlDecode(item.Description);

            int newsTypeID = 0;
            int.TryParse(ddlNotificationType.SelectedValue, out newsTypeID);

            result = WTSNews.SaveNews(item, _newsID, newsTypeID);

            if (result["saved"] == "True")
            {
                int news_ID = 0;
                int.TryParse(result["newID"], out news_ID);

                int attachment_ID = 0;
                int.TryParse(result["attachmentID"], out attachment_ID);

                
                if (news_ID != 0 && attachment_ID == 0) //Adding new Attachment
                {
                    //if (fileUpload.HasFiles)
                    //{
                    //    foreach (var file in fileUpload.PostedFiles)
                    //    {
                    //        Stream fileStream = file.InputStream;
                    //        byte[] fileData = new byte[file.ContentLength];
                    //        string fileName = file.FileName;
                    //        string[] splitFileName = fileName.Split('\\');

                    //        fileName = splitFileName[splitFileName.Length - 1];

                    //        fileStream.Read(fileData, 0, fileData.Length);
                    //        fileStream.Close();

                    //        result["saved"] = WTSNews.NewsAttachment_Add(newsID: news_ID, attachmentTypeID: (int)AttachmentTypeEnum.News, fileName: fileName, title: item.ArticleTitle, description: item.Description, fileData: fileData, extensionID: 0, newAttachmentID: out newAttachmentID, errorMsg: out errorMsg).ToString();

                    //    }
                    //}
                }         
                else if (news_ID != 0 && attachment_ID != 0) //Updating old Attachment
                {


                }


            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);

            result["error"] = ex.Message;
        }

       ScriptManager.RegisterStartupScript(this, this.GetType(), "complete", "<script type=\"text/javascript\">complete('" + "" + "');</script>", false);

    }
}