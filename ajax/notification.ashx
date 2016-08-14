<%@ WebHandler Language="C#" Class="NotificationHandler" %>
using System;

public class NotificationHandler : AjaxHandler
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list") // list current notifications
        {
            R.Data = SAAO.Notification.ListJson();
        }
        else if (context.Request["action"] == "report") // download a supervising report
        {
            Guid guid;
            if (!Guid.TryParse(context.Request["id"], out guid)) return;
            SAAO.Utility.Download(
                path: SAAO.Notification.StoragePath + context.Request["id"],
                fileName: "监督报告",
                contentType: "application/pdf"
            );
        }
        else if (context.Request["action"] == "create") // create a notification
        {
            if (context.Request.Form["title"] == null || context.Request.Form["content"] == null ||
                context.Request.Form["type"] == null ||
                (((context.Request.Form["type"] != "0" && context.Request.Form["type"] != "1") ||
                  (!SAAO.User.Current.IsExecutive && !SAAO.User.Current.IsGroupHeadman)) &&
                 (context.Request.Form["type"] != "2" || SAAO.User.Current.IsSupervisor ||
                  context.Request.Files.Count != 1)) || context.Request.Form["important"] == null ||
                (context.Request.Form["important"] != "0" &&
                 (context.Request.Form["important"] != "1" || !SAAO.User.Current.IsExecutive))) return;
            var n = new SAAO.Notification(
                title: context.Request.Form["title"],
                content: context.Request.Form["content"],
                type: (SAAO.Notification.PermissionType)int.Parse(context.Request.Form["type"])
            );
            if (context.Request.Form["type"] == "2")
            {
                var guid = Guid.NewGuid().ToString("D").ToUpper();
                context.Request.Files[0].SaveAs(SAAO.Notification.StoragePath + guid);
                n.AttachReport(guid);
            }
            else if (context.Request.Form["important"] == "1")
            {
                n.SetImportant();
                n.Broadcast();
            }
        }
    }
}