using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Aspose.Cells;
using System.IO;

public partial class MDGrid_ItemType : System.Web.UI.Page
{
    protected DataColumnCollection DCC;
    protected GridCols columnData = new GridCols();

    protected void Page_Load(object sender, EventArgs e)
    {
        initControls();

        loadGridData();
    }

    private void initControls()
    {
        grdMD.GridHeaderRowDataBound += grdMD_GridHeaderRowDataBound;
        grdMD.GridRowDataBound += grdMD_GridRowDataBound;
        grdMD.GridPageIndexChanging += grdMD_GridPageIndexChanging;
    }
    private void loadGridData()
    {
        DataTable dt = null;
        dt = WTS_Reports.GetSRConfig(); 

        if (dt != null)
        {
            this.DCC = dt.Columns;
            Page.ClientScript.RegisterArrayDeclaration("_dcc", JsonConvert.SerializeObject(DCC, Newtonsoft.Json.Formatting.None));
            spanRowCount.InnerText = dt.Rows.Count.ToString();

            //dt.Columns["Name"].SetOrdinal(1); // Messes up save.
            //dt.Columns["ReceiveSREMail"].SetOrdinal(2);
            //dt.Columns["IncludeInSRCounts"].SetOrdinal(3);

            InitializeColumnData(ref dt);
            dt.AcceptChanges();

            int count = dt.Rows.Count;
            spanRowCount.InnerText = count.ToString();
        }

        grdMD.DataSource = dt;
        grdMD.DataBind();
    }
    protected void InitializeColumnData(ref DataTable dt)
    {
        try
        {
            string displayName = string.Empty, groupName = string.Empty;
            bool blnVisible = false, blnSortable = false, blnOrderable = false;

            foreach (DataColumn gridColumn in dt.Columns)
            {
                displayName = gridColumn.ColumnName;
                blnVisible = false;
                blnSortable = false;
                blnOrderable = false;
                groupName = string.Empty;

                switch (gridColumn.ColumnName)
                {
                    case "WTS_RESOURCEID":
                        displayName = "Resource ID";
                        blnVisible = false;
                        blnSortable = false;
                        break;
                    case "Name":
                        displayName = "Name";
                        blnVisible = true;
                        break;
                    case "ReceiveSREMail":
                        displayName = "Receive EMail";
                        blnVisible = true;
                        blnSortable = false;
                        break;
                    case "IncludeInSRCounts":
                        displayName = "Include in SR Counts";
                        blnVisible = true;
                        blnSortable = false;
                        break;
                }

                columnData.Columns.Add(gridColumn.ColumnName, displayName, blnVisible, blnSortable);
                columnData.Columns.Item(columnData.Columns.Count - 1).CanReorder = blnOrderable;
            }
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }
    void grdMD_GridHeaderRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridHeader(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);
    }

    private void formatColumnDisplay(ref GridViewRow row)
    {
        for (int i = 0; i < row.Cells.Count; i++)
            {
            row.Cells[i].Style["text-align"] = "left";
            row.Cells[i].Style["padding-left"] = "5px";
        }
        ////more column formatting, no effect
        row.Cells[DCC.IndexOf("Name")].Style["width"] = "60px";
        row.Cells[DCC.IndexOf("ReceiveSREMail")].Style["width"] = "60px";
        row.Cells[DCC.IndexOf("IncludeInSRCounts")].Style["width"] = "800px";
    }

    void grdMD_GridRowDataBound(object sender, GridViewRowEventArgs e)
    {
        columnData.SetupGridBody(e.Row);
        GridViewRow row = e.Row;
        formatColumnDisplay(ref row);

        string itemId = row.Cells[DCC.IndexOf("WTS_RESOURCEID")].Text.Trim();

        bool isChecked = false;
        if (row.Cells[DCC.IndexOf("ReceiveSREMail")].HasControls()
            && row.Cells[DCC.IndexOf("ReceiveSREMail")].Controls[0] is CheckBox)
        {
            isChecked = ((CheckBox)row.Cells[DCC.IndexOf("ReceiveSREMail")].Controls[0]).Checked;
        }
        else if (row.Cells[DCC.IndexOf("ReceiveSREMail")].Text == "1")
        {
            isChecked = true;
        }
        row.Cells[DCC.IndexOf("ReceiveSREMail")].Controls.Clear();
        row.Cells[DCC.IndexOf("ReceiveSREMail")].Controls.Add(WTSUtility.CreateGridCheckBox("ReceiveSREMail", itemId, isChecked));

        if (row.Cells[DCC.IndexOf("IncludeInSRCounts")].HasControls()
            && row.Cells[DCC.IndexOf("IncludeInSRCounts")].Controls[0] is CheckBox)
        {
            isChecked = ((CheckBox)row.Cells[DCC.IndexOf("IncludeInSRCounts")].Controls[0]).Checked;
        }
        else if (row.Cells[DCC.IndexOf("IncludeInSRCounts")].Text == "1")
        {
            isChecked = true;
        }
        row.Cells[DCC.IndexOf("IncludeInSRCounts")].Controls.Clear();
        row.Cells[DCC.IndexOf("IncludeInSRCounts")].Controls.Add(WTSUtility.CreateGridCheckBox("IncludeInSRCounts", itemId, isChecked));
    }
    void grdMD_GridPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMD.PageIndex = e.NewPageIndex;
            loadGridData();
    }

    [WebMethod(true)]
    public static string SaveChanges(string rows)
    {
        Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "" }, { "ids", "" }, { "error", "" } };
        bool  saved = false;
        string ids = string.Empty, errorMsg = string.Empty, tempMsg = string.Empty;

        try
        {
            DataTable dtjson = (DataTable)JsonConvert.DeserializeObject(rows, (typeof(DataTable)));
            if (dtjson.Rows.Count == 0)
            {
                errorMsg = "Unable to save. An invalid list of changes was provided.";
                saved = false;
            }

            int id = 0, receiveSREMail = 0, includeInSRCounts = 0;

            HttpServerUtility server = HttpContext.Current.Server;
            //save
            foreach (DataRow dr in dtjson.Rows)
            {
                int.TryParse(dr["WTS_RESOURCEID"].ToString(), out id);
                int.TryParse(dr["ReceiveSREMail"].ToString(), out receiveSREMail);
                int.TryParse(dr["IncludeInSRCounts"].ToString(), out includeInSRCounts);

                WTS_Reports.SRRecipientSave(id, receiveSREMail, includeInSRCounts);

            }
            saved = true;
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
            saved = false;
            errorMsg = ex.Message;
        }

        result["saved"] = saved.ToString();
        result["error"] = errorMsg;

        return JsonConvert.SerializeObject(result, Formatting.None);
    }

}