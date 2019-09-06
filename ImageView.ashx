<%@ WebHandler Language="C#" Class="ImageView" %>

using System;
using System.Data;
using System.IO;
using System.Web;

public class ImageView : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        string id = context.Request.QueryString["id"];
        int imageID = 0;
        int.TryParse(id, out imageID);

        if (imageID > 0)
        {
            MemoryStream memoryStream = new MemoryStream();

            DataTable dt = MasterData.Image_Get(ImageID: imageID);

            byte[] file = (byte[])dt.Rows[0]["FileData"];

            memoryStream.Write(file, 0, file.Length);
            context.Response.Buffer = true;
            context.Response.BinaryWrite(file);
            memoryStream.Dispose();
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}