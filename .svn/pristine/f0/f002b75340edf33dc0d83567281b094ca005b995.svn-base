using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using WTS.Reports;

namespace WTS
{
    public class WTSContentPage : WTSPage
    {
        protected List<object> MenuItems = new List<object>();
        
        protected void SetPageTitle(string title, string classOverride = null)
        {            
            if (string.IsNullOrWhiteSpace(title))
            {
                // the content page is in a frameset, so title shouldn't be changed since the app title is globally set at the top page level
                //Page.Title = "WTS - Workload Tracking System";
            }
            else
            {
                // the content page is in a frameset, so title shouldn't be changed since the app title is globally set at the top page level
                //Page.Title = "WTS - Workload Tracking System - " + title;

                ((Literal)Page.Master.FindControl("ContentTitle")).Visible = true;
                ((Literal)Page.Master.FindControl("ContentTitle")).Text = "<span class=\"" + (classOverride ?? "pagetitle") + "\">" + title + "</span>";
            }
        }

        protected void HideHeader()
        {
            ((HtmlGenericControl)Page.Master.FindControl("pageContentHeader")).Style.Add("display", "none");
        }

        protected void AddMenuItem(MenuItem mi, bool leftSide = true)
        {
            Control c = null;

            switch (mi.MenuItemType) {
                case (int)MenuItemTypeEnum.Button:
                    Button b = new Button();
                    b.Text = mi.Text;
                    if (!string.IsNullOrWhiteSpace(mi.OnClickJavascript))
                    {
                        b.Attributes.Add("onclick", mi.OnClickJavascript);
                    }
                    c = b;
                    break;
                case (int)MenuItemTypeEnum.DropDownList:
                    DropDownList ddl = new DropDownList();
                    ListItem li = new ListItem();
                    li = new ListItem(mi.Text);
                    li.Attributes.Add("OptionGroup", "Process Views");
                    ddl.Items.Add(li);
                    DataTable dt = Filtering.LoadReportViews(-1, mi.ID);
                    if (dt != null)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            li = new ListItem(dr["ViewName"].ToString());
                            li.Attributes.Add("OptionGroup", dr["WTS_RESOURCEID"].ToString() == "0" ? "Process Views" : "Custom Views");
                            li.Attributes.Add("MyView", dr["CREATEDBY"].ToString() == HttpContext.Current.User.Identity.Name ? "1" : "0");
                            li.Attributes.Add("ViewID", dr["UserReportViewID"].ToString());
                            ddl.Items.Add(li);
                        }
                    }

                    ddl.ID = mi.ReportName;
                    ddl.Enabled = true;

                    c = ddl;
                    break;
                case (int)MenuItemTypeEnum.Text:
                    Literal t = new Literal();
                    t.Text = mi.Text;
                    
                    c = t;
                    break;
                case (int)MenuItemTypeEnum.EditableDropDownList:
                    iti_Tools_Sharp.DropDownList eddl = new iti_Tools_Sharp.DropDownList();
                    li = new ListItem(mi.Text);
                    li.Attributes.Add("OptionGroup", "Process Views");
                    eddl.Items.Add(li);
                    dt = Filtering.LoadReportViews(-1, mi.ID);
                    if (dt != null)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            li = new ListItem(dr["ViewName"].ToString());
                            li.Attributes.Add("OptionGroup", dr["WTS_RESOURCEID"].ToString() == "0" ? "Process Views" : "Custom Views");
                            li.Attributes.Add("MyView", dr["CREATEDBY"].ToString() == HttpContext.Current.User.Identity.Name ? "1" : "0");
                            li.Attributes.Add("ViewID", dr["UserReportViewID"].ToString());
                            eddl.Items.Add(li);
                        }
                    }

                    eddl.ID = mi.ReportName;
                    eddl.Enabled = true;

                    c = eddl;
                    break;
                case (int)MenuItemTypeEnum.Image:
                    Image i = new Image();
                    i.Height = 15;
                    i.Width = 15;
                    i.ImageUrl = mi.ImgSrc;
                    i.AlternateText = mi.Text;
                    i.ToolTip = mi.Text;
                    i.Style["padding-left"] = "3px";
                    i.Style["padding-right"] = "3px";
                    i.Style["vertical-align"] = "middle";
                    i.Style["cursor"] = "pointer";
                    if (!string.IsNullOrWhiteSpace(mi.OnClickJavascript))
                    {
                        i.Attributes.Add("onclick", mi.OnClickJavascript);
                    }
                    c = i;
                    break;
                case (int)MenuItemTypeEnum.ItiMenu:
                    iti_Tools_Sharp.Menu menu = new iti_Tools_Sharp.Menu();

                    DataSet dsMenu = new DataSet();
                    dsMenu.ReadXml(this.Server.MapPath("XML/WTS_Menus.xml"));

                    string ReportTypeRelatedItemsRef = "CRGridRelatedItem";
                    switch (mi.ID)
                    {
                        case (int)ReportTypeEnum.CRReport:
                            ReportTypeRelatedItemsRef = "CRGridRelatedItem";
                            break;
                    }

                    if (dsMenu.Tables.Count > 0 && dsMenu.Tables[0].Rows.Count > 0)
                    {
                        if (dsMenu.Tables.Contains(ReportTypeRelatedItemsRef))
                        {
                            menu.DataSource = dsMenu.Tables[ReportTypeRelatedItemsRef];
                            menu.DataValueField = "URL";
                            menu.DataTextField = "Text";
                            menu.DataIDField = "id";
                            if (dsMenu.Tables[ReportTypeRelatedItemsRef].Columns.Contains(ReportTypeRelatedItemsRef + "_id_0"))
                            {
                                menu.DataParentIDField = ReportTypeRelatedItemsRef + "_id_0";
                            }
                            menu.ID = "menuRelatedItems";
                            menu.DataImageField = "ImageType";
                            menu.Text = "Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />";
                            menu.Button = true;
                            menu.DataBind();
                            menu.ClientClickEvent = "openMenuItem";
                        }
                    }
                    c = menu;
                    break;
            }

            ((PlaceHolder)Page.Master.FindControl("MenuBar" + (leftSide ? "Left" : "Right") + "PlaceHolder")).Controls.Add(c);
        }

        protected void AddMenuButton(string text, string onClickJavascript, bool leftSide = true)
        {
            MenuItem mi = new MenuButton(text, onClickJavascript);

            AddMenuItem(mi, leftSide);
        }
        protected void AddMenuImageButton(string imgSrc, string hoverText, string onClickJavascript, bool leftSide = true)
        {
            MenuItem mi = new MenuImageButton(imgSrc, hoverText, onClickJavascript);

            AddMenuItem(mi, leftSide);
        }

        protected void AddMenuText(string text)
        {
            MenuItem mi = new MenuText(text);

            AddMenuItem(mi);
        }

        protected void AddMenuDDL(string text, int id, string reportName, bool leftSide = true)
        {
            MenuItem eddl = new MenuDDL(text, id, reportName);

            AddMenuItem(eddl, leftSide);
        }

        protected void AddMenuEditableDDL(string text, int id, string reportName, bool leftSide = true)
        {
            MenuItem eddl = new MenuEDDL(text, id, reportName);

            AddMenuItem(eddl, leftSide);
        }

        protected void AddMenuEditableDDLItem(string text)
        {
            ListItem li = new ListItem(text);
            ((iti_Tools_Sharp.DropDownList)Page.Master.FindControl("SavedViewsDDL")).Items.Add(li);
        }

        protected void AddItiMenuItem(int id, bool leftSide = true)
        {
            MenuItem mi = new ItiMenu(id);

            AddMenuItem(mi, leftSide);
        }

        protected void ClearMenuItems(bool leftSide = true, bool rightSide = true)
        {
            if (leftSide) ((PlaceHolder)Page.Master.FindControl("MenuBarLeftPlaceHolder")).Controls.Clear();
            if (rightSide) ((PlaceHolder)Page.Master.FindControl("MenuBarRightPlaceHolder")).Controls.Clear();
        }

        public enum MenuItemTypeEnum
        {
            Button = 1,
            DropDownList = 2,
            Text = 3,
            EditableDropDownList = 4,
            Image = 5,
            ItiMenu = 6,
        }

        public class MenuItem
        {
            public int MenuItemType { get; set; }
            public string Text { get; set; }
            public string OnClickJavascript { get; set; }
            public int ID { get; set; }
            public string ReportName { get; set; }
            public string ImgSrc { get; set; }
        }

        public class MenuButton : MenuItem
        {
            public MenuButton(string text, string onClickJavascript)
            {
                MenuItemType = (int)MenuItemTypeEnum.Button;
                Text = text;
                OnClickJavascript = onClickJavascript + "; return false;";
            }
        }

        public class MenuImageButton : MenuItem
        {
            public MenuImageButton(string imgSrc, string text, string onClickJavascript)
            {
                MenuItemType = (int)MenuItemTypeEnum.Image;
                ImgSrc = imgSrc;
                Text = text;
                OnClickJavascript = onClickJavascript + "; return false;";
            }
        }

        public class MenuDDL : MenuItem
        {
            public MenuDDL(string text, int id, string reportName)
            {
                MenuItemType = (int)MenuItemTypeEnum.DropDownList;
                Text = text;
                ID = id;
                ReportName = "SavedViewsDDL";
            }
        }

        public class MenuEDDL :  MenuItem
        {
            public MenuEDDL(string text, int id, string reportName)
            {
                MenuItemType = (int)MenuItemTypeEnum.EditableDropDownList;
                Text = text;
                ID = id;
                ReportName = "SavedViewsEDDL";
            }
        }

        public class MenuText : MenuItem
        {
            public MenuText(string text) {
                MenuItemType = (int)MenuItemTypeEnum.Text;
                Text = text + "  ";
            }
        }

        public class ItiMenu : MenuItem
        {
            public ItiMenu(int id)
            {
                MenuItemType = (int)MenuItemTypeEnum.ItiMenu;
                ID = ID;
            }
        }
    }
}