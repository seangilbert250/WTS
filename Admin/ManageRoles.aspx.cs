using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_ManageRoles : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        //load theme for user
        Page.Theme = WTSUtility.ThemeName;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //TODO: check user permissions before loading grid

        LoadData();

        Page.Form.DefaultFocus = this.txtRole.ClientID;
    }

    protected void LoadData()
    {
        //Always include the default theme
        Page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl("~/App_Themes/Default/Default.css") + "\" />"));

        this.listBoxRoles.Items.Clear();
        string[] roles = Roles.GetAllRoles();

        this.listBoxRoles.DataSource = roles;
        listBoxRoles.DataBind();
    }
    protected void buttonAdd_Click(object sender, EventArgs e)
    {
        string roleNm = txtRole.Text.Trim();
        if (roleNm != "")
        {
            if (!Roles.RoleExists(roleNm))
            {
                Roles.CreateRole(roleNm);
            }

            LoadData();
            txtRole.Text = string.Empty;
        }
    }
}