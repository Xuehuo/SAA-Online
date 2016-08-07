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
                SAAO.SqlIntegrate si = new SAAO.SqlIntegrate(SAAO.Utility.ConnStr);
                string group = SAAO.User.Current.GroupName;
                string begin = si.AdapterJson($"SELECT * FROM [Calendar] WHERE [group] = '{SAAO.User.Current.Group}' AND [start] = CONVERT(varchar(10),getdate(),110)");
                string doing = si.AdapterJson($"SELECT * FROM [Calendar] WHERE [group] = '{SAAO.User.Current.Group}' AND [start] < getdate() AND [end] > getdate()");
                string todo = si.AdapterJson($"SELECT * FROM [Calendar] WHERE [group] = '{SAAO.User.Current.Group}' AND [end] = CONVERT(varchar(10),getdate(),110)");
                context.Response.Write("{\"flag\":0,\"data\":{\"group\":\"" + group + "\",\"begin\":" + begin + ",\"doing\":" + doing + ",\"todo\":" + todo + "}}");
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