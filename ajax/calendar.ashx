<%@ WebHandler Language="C#" Class="calendarHandler" %>
using System;
using System.Web;
using System.Web.SessionState;
public class calendarHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list")
        {
            try
            {
                context.Response.Write("{\"flag\":0,\"data\":" + SAAO.Event.ListJson() + "}");
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"flag\":3}");
                SAAO.Utility.Log(ex);
            }
        }
    }
    public bool IsReusable => false;
}