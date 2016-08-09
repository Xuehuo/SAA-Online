<%@ WebHandler Language="C#" Class="dashboardHandler" %>
using System;
using System.Web;
using System.Web.SessionState;
public class dashboardHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list")
        {
            try
            {
                context.Response.Write("{\"flag\":0,\"data\":" + SAAO.Event.DashboardJson()+ "}");
            }
            catch (Exception ex)
            {
                SAAO.Utility.Log(ex);
                context.Response.Write("{\"flag\":3}");
            }
        }
    }
    public bool IsReusable => false;
}