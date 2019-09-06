using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using Newtonsoft.Json;


/// <summary>
/// Summary description for SessionMethods
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class SessionMethods : System.Web.Services.WebService {

    public SessionMethods () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(true)]
    public string SetCrosswalkParentColumnOrder(string parentColumns, string selectedColumnOrder) {
		Dictionary<string, string> result = new Dictionary<string, string>() { { "saved", "false" }, { "error", "" } };
		bool saved = false;
		string errorMsg = string.Empty;

		try
		{
			HttpContext.Current.Session["Crosswalk_SelectedParentColumns"] = parentColumns;
			HttpContext.Current.Session["Crosswalk_SelectedParentColumnOrder"] = parentColumns;

			saved = true;
		}
		catch (Exception ex)
		{
			LogUtility.LogException(ex);
			saved = false;
		}

		result["saved"] = saved.ToString();
		result["error"] = errorMsg;

		return JsonConvert.SerializeObject(result, Formatting.None);
    }

	[WebMethod(true)]
	public string KeepSessionAlive() {
		return "true";
	}
}
