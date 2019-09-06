﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for WorkloadWebmethods
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WorkloadWebmethods : System.Web.Services.WebService {

	public WorkloadWebmethods () {

		//Uncomment the following line if using designed components 
		//InitializeComponent(); 
	}

	[WebMethod(true)]
	public string ItemExists(int itemID, int taskNumber, string type)
	{
		try
		{
			return Workload.ItemExists(itemID, taskNumber, type).ToString();
		}
		catch (Exception) { return "false"; }
	}

	[WebMethod(true)]
	public string EmailHotlist()
	{
		try
		{
			return Workload.EmailHotlist().ToString();
		}
		catch (Exception) { return "false"; }
	}

    [WebMethod(true)]
    public string SRHotlist()
    {
        try
        {
            return Workload.SRHotlist().ToString();
        }
        catch (Exception e)
        {
            string errMsg = e.Message;
            return "false";
        }
    }

}
