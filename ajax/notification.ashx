<%@ WebHandler Language="C#" Class="notificationHandler" %>
using System;
using System.Web;
using System.Web.SessionState;
using System.IO;
public class notificationHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list") // list current notifications
        {
            try
            {
                context.Response.Write("{\"flag\":0,\"data\":" + SAAO.Notification.ListJson() + "}");
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"flag\":3}");
                SAAO.Utility.Log(ex);
            }
        }
        if (context.Request["action"] == "report") // download a supervising report
        {
            Guid guid;
            if (Guid.TryParse(context.Request["id"], out guid))
            {
                SAAO.Utility.Download(SAAO.Notification.StoragePath + context.Request["id"], "监督报告", "application/pdf");
            }
        }
        if (context.Request["action"] == "create") // create a notification
        {
            try
            {
                if (context.Request.Form["title"] != null
                    && context.Request.Form["content"] != null
                    && context.Request.Form["type"] != null
                    && ((context.Request.Form["type"] == "0" || context.Request.Form["type"] == "1") && (SAAO.User.Current.IsExecutive || SAAO.User.Current.IsGroupHeadman)
                        || (context.Request.Form["type"] == "2" && !SAAO.User.Current.IsSupervisor && context.Request.Files.Count == 1)
                        )
                    && context.Request.Form["important"] != null
                    && (context.Request.Form["important"] == "0" || (context.Request.Form["important"] == "1" && SAAO.User.Current.IsExecutive)))

                {
                    SAAO.Notification n = new SAAO.Notification(context.Request.Form["title"], context.Request.Form["content"], (SAAO.Notification.PermissionType)int.Parse(context.Request.Form["type"]));

                    if (context.Request.Form["type"] == "2")
                    {
                        string guid = Guid.NewGuid().ToString("D").ToUpper();
                        context.Request.Files[0].SaveAs(SAAO.Notification.StoragePath + guid);
                        n.AttachReport(guid);
                    }
                    else
                        n.Broadcast(); // broadcast automatically if not supervising report
                    if (context.Request.Form["important"] == "1")
                        n.SetImportant();
                    context.Response.Write("{\"flag\":0}");
                }
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