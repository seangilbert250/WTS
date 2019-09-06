<%@ Application Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="System.Security.Principal" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Web.Http" %>
<%@ Import Namespace="System.Web.Http.WebHost" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        try
        {
            bool cleaned = Filtering.CleanUserFilters();
            cleaned = WTSCommon.CleanUserViews();

            // Web API Default Routes
            RouteTable.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = System.Web.Http.RouteParameter.Optional }
            );

            WTS.Events.EventQueueService.StartService();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    void Application_BeginRequest(object sender, EventArgs e)
    {
        try
        {
            // we're putting this here becuase application_start doesn't always get re-run when our files are copied to prod; this call
            // should ensure that the service is up every time a request is made
            WTS.Events.EventQueueService.StartService();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    void Application_End(object sender, EventArgs e)
    {
        try
        {
            bool cleaned = Filtering.CleanUserFilters();
            cleaned = WTSCommon.CleanUserViews();
            WTS.Events.EventQueueService.Stop();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs
        try
        {
            Exception ex = Server.GetLastError();
            // log to database
            LogUtility.LogException(ex);
            throw ex;
        }
        catch (Exception exc)
        {
            try
            {
                // this version of the call goes to a log file
                LogUtility.Log(exc.Message + " " + exc.StackTrace);
                throw exc;
            }
            catch (Exception exception)
            {
                // nothing left to do here; it's gonna happen
                throw exception;
            }
        }
    }

    public void Session_OnStart()
    {
        try
        {
            bool cleaned = Filtering.CleanUserFilters();
            cleaned = WTSCommon.CleanUserViews();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }

    public void Session_OnEnd()
    {
        try
        {
            bool cleaned = Filtering.CleanUserFilters();
            cleaned = WTSCommon.CleanUserViews();
        }
        catch (Exception ex)
        {
            LogUtility.LogException(ex);
        }
    }
</script>
