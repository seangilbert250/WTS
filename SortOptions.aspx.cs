﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using Aspose.Cells;  //for exporting to excel
using System.IO;
using Newtonsoft.Json;


using System.Linq;

public partial class SortOptions : System.Web.UI.Page
{
    protected string DefaultColumns = "";
    protected string SortColumns = "";
    protected string SortOrder = "";
    protected string gridName = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        string[] splitSortOrder;
        try
        {
            if (Request.Form.Count > 0)
            {
                if (Request.Form["sortColumns"] != "")
                {
                    SortColumns = Server.HtmlDecode(Request.Form["sortColumns"]);
                }
                if (Request.Form["defaultColumns"] != "")
                {
                    DefaultColumns = Server.HtmlDecode(Request.Form["defaultColumns"]);
                }
                if (Request.Form["sortOrder"] != "")
                {
                    SortOrder = Server.HtmlDecode(Request.Form["sortOrder"]);
                }
            }
            else
            {
                if (Request.QueryString["sortColumns"] != "")
                {
                    SortColumns = HttpUtility.UrlDecode(Request.QueryString["sortColumns"]);
                }
                if (Request.QueryString["defaultColumns"] != "")
                {
                    DefaultColumns = HttpUtility.UrlDecode(Request.QueryString["defaultColumns"]);
                }
                if (Request.QueryString["sortOrder"] != "")
                {
                    SortOrder = HttpUtility.UrlDecode(Request.QueryString["sortOrder"]);
                }

                if (SortColumns != null)
                {
                    SortColumns = SortColumns.Replace("^", "/");
                }

                if (DefaultColumns != null)
                {
                    DefaultColumns = DefaultColumns.Replace("^", "/");
                }
            }

            if (SortColumns != null)
            {
                // Order the items alphabetically.
                SortColumns = String.Join(";", SortColumns.Split(';').Select(c => Convert.ToString(c)).OrderBy(i => i));
            }

            if (Request.QueryString["GridName"] != null
                && !string.IsNullOrWhiteSpace(Request.QueryString["GridName"].ToString()))
            {
                gridName = Request.QueryString["GridName"].ToString();
            }

        }
        catch (Exception ex)
        {
            SortColumns = "";
            DefaultColumns = "";
        }

        splitSortOrder = SortOrder.Split('~');

        ListItem lstItem1 = new ListItem("");
        dlField1.Items.Add(lstItem1);
        ListItem lstItem2 = new ListItem("");
        dlField2.Items.Add(lstItem2);
        ListItem lstItem3 = new ListItem("");
        dlField3.Items.Add(lstItem3);
        ListItem lstItem4 = new ListItem("");
        dlField4.Items.Add(lstItem4);

        if (SortColumns.Length > 0)
        {
            string[] _sortColumns = SortColumns.Split(';');
            for (int i = 0; i < _sortColumns.Length; i++)
            {
                ListItem nlstItem1 = new ListItem(_sortColumns[i]);
                dlField1.Items.Add(nlstItem1);
                ListItem nlstItem2 = new ListItem(_sortColumns[i]);
                dlField2.Items.Add(nlstItem2);
                ListItem nlstItem3 = new ListItem(_sortColumns[i]);
                dlField3.Items.Add(nlstItem3);
                ListItem nlstItem4 = new ListItem(_sortColumns[i]);
                dlField4.Items.Add(nlstItem4);
            }
        }

        if (splitSortOrder[0] != "")
        {
            if (splitSortOrder.Length > 0)
            {
                dlField1.SelectedValue = splitSortOrder[0].Split('|')[0];
                optSort1.SelectedValue = splitSortOrder[0].Split('|')[1];
            }
            if (splitSortOrder.Length > 1)
            {
                dlField2.SelectedValue = splitSortOrder[1].Split('|')[0];
                optSort2.SelectedValue = splitSortOrder[1].Split('|')[1];
            }
            if (splitSortOrder.Length > 2)
            {
                dlField3.SelectedValue = splitSortOrder[2].Split('|')[0];
                optSort3.SelectedValue = splitSortOrder[2].Split('|')[1];
            }
            if (splitSortOrder.Length > 3)
            {
                dlField4.SelectedValue = splitSortOrder[3].Split('|')[0];
                optSort4.SelectedValue = splitSortOrder[3].Split('|')[1];
            }
        }
    }

    [WebMethod(true)]
    public static void ApplySortToDB(int openerPageID, string gridName, string sortValues)
    {
        if (sortValues.Substring(sortValues.Length - 4, 4).ToLower() == "true")
            Workload.SortValuesDelete(gridName);
        else
            Workload.ApplySortToDB(openerPageID, gridName, sortValues);
    }

    [WebMethod(true)]
    public static string GetSortOptionsFromDB(int openerPageID, string gridName)
    {
        return Workload.GetSortValuesFromDB(openerPageID, gridName);
    }
    [WebMethod(true)]
    public static string DeleteSort(int openerPageID, string gridName)
    {
        return Workload.DeleteSort(openerPageID, gridName);
    }
}