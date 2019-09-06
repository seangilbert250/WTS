using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Aspose.Cells;  //for exporting to excel
using Newtonsoft.Json;


public partial class WorkItem_SR_Status : System.Web.UI.Page
{
    protected int _showArchived = 0;
    protected bool _export = false;
    protected int _pageIndex = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        init();
        readQueryString();
        loadSR_Items();
    }


    private void init()
    {
      
        grdSR.GridPageIndexChanging += grdSR_GridPageIndexChanging;

              
    }


    void grdSR_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdSR.PageIndex = e.NewPageIndex;
        if (HttpContext.Current.Session["dtSR"] == null)
        {
            loadSR_Items();
        }
        else
        {
            grdSR.DataSource = (DataTable)HttpContext.Current.Session["dtSR"];
        }
    }




    private void readQueryString()
    {
       
        if (Request.QueryString["PageIndex"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["PageIndex"]))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["PageIndex"]).Trim(), out _pageIndex);
        }

      
        if (Request.QueryString["ShowArchived"] != null
            && !string.IsNullOrWhiteSpace(Request.QueryString["ShowArchived"].ToString()))
        {
            int.TryParse(Server.UrlDecode(Request.QueryString["ShowArchived"].ToString()), out this._showArchived);
        }

      
        if (Request.QueryString["Export"] != null &&
            !string.IsNullOrWhiteSpace(Request.QueryString["Export"]))
        {
            bool.TryParse(Server.UrlDecode(Request.QueryString["Export"]), out _export);
        }
    }

    private void loadSR_Items()
    {
        DataTable dtSR = null;

        try
        {

            //dtSR = Workload.SR_List_Get(workRequestID: 0, showArchived: _showArchived, columnListOnly: 0, filters: null);
            //dtSR = SRItem.SRView_Get();
            dtSR = SRItem.SR_List_GetAll();
            if (dtSR != null && dtSR.Rows.Count > 0)
                {
                    spanRowCount.InnerText = dtSR.Rows.Count.ToString();
                }
                HttpContext.Current.Session["dtSR"] = dtSR;
           
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            dtSR = null;
        }

     
        if (_export)
        {
            ExportExcel(dtSR);
        }
        else
        {
            if (_pageIndex > 0)
            {
                grdSR.PageIndex = _pageIndex;
            }
            grdSR.DataSource = dtSR;
            grdSR.DataBind();
        }
    }


    private bool ExportExcel(DataTable dt)
    {
        bool success = false;
        string errorMsg = string.Empty;

        try
        {
            string name = "SR Import";
            Workbook wb = new Workbook(FileFormatType.Xlsx);
            Worksheet ws = wb.Worksheets[0];
            ws.Cells.ImportDataTable(dt, true, 0, 0, false, false);

        
            ws.AutoFitColumns();
            MemoryStream ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Xlsx);

            Response.ContentType = "application/xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
            Response.BinaryWrite(ms.ToArray());
            Response.End();

            success = true;
        }
        catch (Exception ex)
        {
            success = false;
            errorMsg += Environment.NewLine + ex.Message;
        }

        return success;
    }

}