<%@ WebHandler Language="C#" Class="dashboardHandler" %>
using System;
using System.Web;
using System.Web.SessionState;
public class dashboardHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request["action"] != null && SAAO.User.IsLogin)
        {
            switch (context.Request["action"].ToString())
            {
                case "list":
                    try
                    {
                        SAAO.SqlIntegrate si = new SAAO.SqlIntegrate(SAAO.Utility.connStr);
                        string group = SAAO.User.Current.groupName;
                        string begin = si.AdapterJSON("SELECT * FROM [Calendar] WHERE [group] = '" + SAAO.User.Current.group + "' AND [start] = CONVERT(varchar(10),getdate(),110)");
                        string doing = si.AdapterJSON("SELECT * FROM [Calendar] WHERE [group] = '" + SAAO.User.Current.group + "' AND [start] < getdate() AND [end] > getdate()");
                        string todo = si.AdapterJSON("SELECT * FROM [Calendar] WHERE [group] = '" + SAAO.User.Current.group + "' AND [end] = CONVERT(varchar(10),getdate(),110)");
                        context.Response.Write("{\"flag\":0,\"data\":{\"group\":\"" + group + "\",\"begin\":" + begin + ",\"doing\":" + doing + ",\"todo\":" + todo + "}}");
                    }
                    catch (Exception ex)
                    {
                        SAAO.Utility.Log(ex);
                        context.Response.Write("{\"flag\":3}");
                    }
                    break;
            }
        }
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}