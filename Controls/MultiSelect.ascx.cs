﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using WTS;

public partial class Controls_MultiSelect : WTSControl
{
    public string Label { get; set; }

    public DataTable Items { get; set; }

    public string ItemLabelColumnName { get; set; }
    public string ItemValueColumnName { get; set; }
    public string ItemSelectedColumnName { get; set; }

    public string OptionGroupLabelColumnName { get; set; }
    public string OptionGroupValueColumnName { get; set; }
    //public string OptionGroupSelectedColumnName { get; set; }

    public string CustomAttributes { get; set; }

    public string OnClickFunctionName { get; set; }
    public string OnCheckAllFunctionName { get; set; }
    public string OnChangeFunctionName { get; set; }
    public string OnOpenFunctionName { get; set; }
    public string OnCloseFunctionName { get; set; }
    public string SelectAll { get; set; }
    public string IgnoreDefaultItems { get; set; } // if true, items in the Item data table with value 0 or "" are not included in the multiselect
    public string SelectedItems { get; set; } // if select all is false, this value will supercede the ItemSelectedColumnName column value in determining whether an item is selected

    public bool ShowChildCount = false;
    public bool IsOpen = false;
    public bool KeepOpen = false;
    public string MaxHeight = "250px";
    public string Width = "100%";
    public int MinimumCountSelected = 0;
    public string ButtonWidth = "250px";
    public bool HideDDLButton = false;
    public bool IsVisible = true;    

    protected List<string> Groups = new List<string>();
    protected string GroupCounts = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Label))
        {
            labelTD.Visible = false;
        }        

        if (string.IsNullOrWhiteSpace(ItemLabelColumnName) || string.IsNullOrWhiteSpace(ItemValueColumnName) ||  Items == null)
        {
            return;
        }

        int idx = 0;

        if (string.IsNullOrWhiteSpace(SelectAll)) SelectAll = "false";
        if (string.IsNullOrWhiteSpace(IgnoreDefaultItems)) IgnoreDefaultItems = "false";

        Dictionary<string, int> counts = new Dictionary<string, int>();
        if (ShowChildCount && OptionGroupValueColumnName != null)
        {
            foreach (DataRow row in Items.Rows)
            {
                string groupValue = row[OptionGroupValueColumnName].ToString();

                if (!counts.ContainsKey(groupValue))
                {
                    counts[groupValue] = 0;
                }

                string itemValue = row[ItemValueColumnName].ToString();

                if (!string.IsNullOrWhiteSpace(itemValue))
                {
                    if (IgnoreDefaultItems.ToUpper() == "TRUE" && (itemValue == "0" || itemValue == "-1"))
                    {
                        continue;
                    }

                    counts[groupValue]++;
                }
            }            
        }

        string[] customAttributes = !string.IsNullOrWhiteSpace(CustomAttributes) ? CustomAttributes.Split(',') : null;
        List<string> selList = new List<string>("t,true,y,yes,1".Split(','));

        ddlItems.Items.Clear();

        foreach (DataRow row in Items.Rows)
        {
            string itemLabel = row[ItemLabelColumnName] != DBNull.Value ? row[ItemLabelColumnName].ToString() : "";
            string itemValue = row[ItemValueColumnName] != DBNull.Value ? row[ItemValueColumnName].ToString() : "";
            string itemSelected = !string.IsNullOrWhiteSpace(ItemSelectedColumnName) ? (row[ItemSelectedColumnName] != DBNull.Value ? row[ItemSelectedColumnName].ToString() : null) : null;

            if (itemValue == "") itemValue = "MISSING";

            if (IgnoreDefaultItems.ToUpper() == "TRUE")
            {
                if (string.IsNullOrWhiteSpace(itemValue) || itemValue == "0" || itemValue == "-1")
                {
                    continue;
                }
            }

            itemValue = (!string.IsNullOrWhiteSpace(OptionGroupValueColumnName) ? row[OptionGroupValueColumnName].ToString() + "___" :  "") + itemValue;
            
            if (itemSelected == null) itemSelected = "";
            bool selected = false;

            if (SelectAll.ToString().ToUpper() == "TRUE")
            {
                selected = true;
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItems))
            {
                selected = ("," + SelectedItems.ToLower() + ",").IndexOf("," + itemValue.ToLower() + ",") != -1;
            }
            else
            {
                selected = selList.Contains(itemSelected.ToLower());
            }

            ListItem li = new ListItem(itemLabel, itemValue);
            li.Selected = selected;
            li.Attributes.Add("RowIdx", idx.ToString());

            if (customAttributes != null && customAttributes.Length > 0)
            {
                for (int x = 0; x < customAttributes.Length; x++)
                {
                    li.Attributes.Add(customAttributes[x].Trim(), row[customAttributes[x].Trim()].ToString());
                }
            }

            if (OptionGroupLabelColumnName != null && OptionGroupValueColumnName != null)
            {                
                string groupValue = row[OptionGroupValueColumnName].ToString();
                string groupLabel = row[OptionGroupLabelColumnName].ToString() + (ShowChildCount ? " (" + counts[groupValue] + ")" : "");

                if (!Groups.Contains(groupLabel))
                {
                    Groups.Add(groupLabel.Replace(";", "<semicolon>").Replace("'", ""));
                }                

                li.Attributes.Add("OptionGroup", groupLabel);
                li.Attributes.Add("OptionGroupValue", groupValue);
            }

            ddlItems.Items.Add(li);

            idx++;
        }

        base.Visible = IsVisible;
    }
}