using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Text;

using WTS;

public partial class Controls_SelectResources : WTSControl
{
    public bool IncludeNotPeople { get; set; }
    public bool IncludeHeader { get; set; }
    public bool IncludeSaveButton { get; set; }
    public bool KeepCheckedResourcesOnTop { get; set; }
    public string SaveFunctionName { get; set; }
    public string Title { get; set; }
    public string OnChangeFunctionName { get; set; }
    public string CustomReturnAttribute { get; set; }
    public bool AllowSaveWithNoSelections { get; set; }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadData();
    }

    public string ContainerID
    {
        get
        {
            return this.ClientID + "_divSelectResourcesContainer";
        }
    }

    private void LoadData()
    {
        DataTable dt = UserManagement.LoadUserList(0, false, false, "");
        
        StringBuilder str = new StringBuilder();

        if (IncludeSaveButton)
        {
            str.Append("<input type=\"button\" id=\"" + this.ID + "_SaveButton" + "\" value=\"Save Selections\" onclick=\"Close" + this.ID + "Popup(true); return false;\">");

            divSelectResourcesContainerButtons.Visible = true;
            divSelectResourcesContainerButtons.InnerHtml = str.ToString();
        }
        

        str.Clear();
        str.Append("<table id=\"" + this.ID + "_selectrsctable\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");

        if (IncludeHeader)
        {
            str.Append("<thead>");
            str.Append("<tr class=\"gridHeader\">");
            str.Append("<th style=\"text-align: center; width: 25px; border-bottom:1px solid gray; border-right:1px solid gray;\">&nbsp;</th>");
            str.Append("<th style=\"text-align: left; width: 150px; border-bottom:1px solid gray; border-left:1px solid gray;\">RESOURCE</th>");
            str.Append("</tr>");
            str.Append("</thead>");
        }

        str.Append("<tbody>");

        if (dt.Rows.Count > 0)
        {
            int cnt = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];

                if (IncludeNotPeople || (row["RESOURCETYPEID"] != DBNull.Value && (int)row["RESOURCETYPEID"] != (int)UserManagement.ResourceType.Not_People))
                {
                    str.Append("<tr selectrscrow=\"1\" origsort=\"" + cnt + "\" rscid=\"" + (int)row["WTS_RESOURCEID"] + "\">");
                    str.Append("  <td style=\"text-align:center;" + (cnt == 0 ? "width:25px;" : "") + "\"><input type=\"checkbox\" id=\"cbSelectResource_" + (int)row["WTS_RESOURCEID"] + "\" name=\"cbSelectResource\" value=\"" + (int)row["WTS_RESOURCEID"] + "\" origsort=\"" + cnt + "\" onchange=\"" + this.ID + "CheckboxChanged(this); return false;\" email=\"" + (row["Email"] != DBNull.Value ? (string)row["Email"] : (row["Email2"] != DBNull.Value ? (string)row["Email2"] : "")) + "\" rscname=\"" + ((string)row["FIRST_NAME"]).Replace("'", "").Replace("\"", "").Replace(",", "") + " " + ((string)row["LAST_NAME"]).Replace("'", "").Replace("\"", "").Replace(",", "") + "\"></td>");
                    str.Append("  <td style=\"text-align:left;" + (cnt == 0 ? "width:150px;" : "") + "\">" + (string)row["FIRST_NAME"] + " " + (string)row["LAST_NAME"] + "</td>");
                    str.Append("</tr>");

                    cnt++;
                }
            }
        }
        else
        {
            str.Append("<tr><td colspan=\"2\">No resources available.</td></tr>");
        }

        str.Append("</tbody>");
        str.Append("</table>");

        divSelectResourcesContainerResources.InnerHtml = str.ToString();
    }
}