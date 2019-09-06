using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WorkloadGrid_SRs : System.Web.UI.Page
{
	protected bool CanView = false;
	protected bool CanEdit = false;

    protected void Page_Load(object sender, EventArgs e)
    {
		this.CanView = UserManagement.UserCanView(WTSModuleOption.SustainmentRequest);
		this.CanEdit = UserManagement.UserCanEdit(WTSModuleOption.SustainmentRequest);
    }
}