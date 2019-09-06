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

public partial class Controls_ResourceDropDownList : WTSControl
{
    public bool ShowBlankOption = false;
    public bool ShowSelectAllOption = false;
    public bool ShowSelectOption = false;
    public string ResourceValueColumn = "WTS_RESOURCEID";
    public string ResourceTextColumn = "Resource";
    public string CustomAttributes = "";
    public char CustomAttributesSeparator = ',';

    public string CustomOptions = "";
    public char CustomOptionsSeparator = ','; //Dave=1,Mike=2
    public char CustomOptionsToken = '='; //Dave=1,Mike=2

    public string DefaultValue = "0";

    public DataTable Resources { get; set; }

    public int OrganizationID = 0;
    public bool ExcludeDeveloper = false;
    public bool LoadArchived = false;
    public string UserNameSearch = "";
    public bool ExcludeNotPeople = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            FillDropDownList();
        }
    }

    private void FillDropDownList()
    {
        if (ShowSelectOption)
        {
            ddlResource.Items.Add(new ListItem("-Select-", "0"));
        }

        if (ShowBlankOption)
        {
            ddlResource.Items.Add(new ListItem("", "0"));
        }

        if (ShowSelectAllOption)
        {
            ddlResource.Items.Add(new ListItem("-Select All-", "-1"));
        }

        if (!string.IsNullOrWhiteSpace(CustomOptions))
        {
            string[] options = CustomOptions.Split(CustomOptionsSeparator);

            for (int i = 0; i < options.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(options[i]))
                {
                    string[] option = options[i].Split(CustomOptionsToken);

                    if (option.Length == 2) {
                        ddlResource.Items.Add(new ListItem(option[0], option[1]));
                    }
                }
            }
        }

        if (string.IsNullOrWhiteSpace(ResourceValueColumn) || string.IsNullOrWhiteSpace(ResourceTextColumn))
        {
            return;
        }

        if (Resources == null)
        {
            Resources = UserManagement.LoadUserList(OrganizationID, ExcludeDeveloper, LoadArchived, UserNameSearch, ExcludeNotPeople);
        }

        for (int i = 0; Resources != null && Resources.Rows.Count > 0 && i < Resources.Rows.Count; i++)
        {
            DataRow row = Resources.Rows[i];

            string value = row[ResourceValueColumn] != DBNull.Value ? row[ResourceValueColumn].ToString() : "0";
            string text = row[ResourceTextColumn] != DBNull.Value ? row[ResourceTextColumn].ToString() : "";

            ListItem li = new ListItem(text, value);

            if (!string.IsNullOrWhiteSpace(CustomAttributes))
            {
                string[] attrs = CustomAttributes.Split(CustomAttributesSeparator);

                for (int x = 0; x < attrs.Length; x++)
                {
                    string attr = attrs[x];

                    if (!string.IsNullOrWhiteSpace(attr))
                    {
                        object obj = null;

                        try { obj = row[attr]; } catch (Exception ex) { LogUtility.LogException(ex); }

                        if (obj != null && obj != DBNull.Value)
                        {
                            li.Attributes.Add(attr, obj.ToString());
                        }
                        else
                        {
                            li.Attributes.Add(attr, "");
                        }
                    }
                }
            }

            ddlResource.Items.Add(li);
        }

        if (!string.IsNullOrWhiteSpace(DefaultValue))
        {
            ddlResource.SelectedValue = DefaultValue;
        }
    }
}