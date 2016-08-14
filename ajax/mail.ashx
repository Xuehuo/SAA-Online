<%@ WebHandler Language="C#" Class="MailHandler" %>
using System;
using Newtonsoft.Json.Linq;

public class MailHandler : AjaxHandler
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list")
        {
            string[] folder = {"INBOX", "Sent", "Drafts", "Trash"};
            if (context.Request["folder"] == null || Array.IndexOf(folder, context.Request["folder"]) == -1) return;
            R.Data = SAAO.Mail.ListJson(context.Request["folder"]);
        }
        else if (context.Request["action"] == "info")
        {
            int mailId;
            if (context.Request["id"] == null || !int.TryParse(context.Request["id"], out mailId)) return;
            var message = new SAAO.Mail(mailId);
            if (message.Username == SAAO.User.Current.Username)
                R.Data = message.ToJson();
            else
                R.Flag = 2;
        }
        else if (context.Request["action"] == "attachment")
        {
            int mailId;
            int index;
            if (context.Request["id"] == null || context.Request["index"] == null ||
                !int.TryParse(context.Request["id"], out mailId) ||
                !int.TryParse(context.Request["index"], out index)) return;
            var message = new SAAO.Mail(mailId);
            if (message.Username == SAAO.User.Current.Username)
                message.DownloadAttachment(index);
            else
                R.Flag = 2;
        }
        else if (context.Request["action"] == "display")
        {
            int mailId;
            if (context.Request["id"] == null || !int.TryParse(context.Request["id"], out mailId)) return;
            context.Response.ContentType = "text/html; charset=utf-8";
            var message = new SAAO.Mail(mailId);
            if (message.Username == SAAO.User.Current.Username)
            {
                context.Response.Write(message.Body());
                message.SetFlag(SAAO.Mail.MailFlag.Seen);
            }
            else
                context.Response.Write("邮件无法加载，访问被拒绝。");
            context.ApplicationInstance.CompleteRequest();
        }
        else if (context.Request["action"] == "delete")
        {
            int mailId;
            if (context.Request["id"] == null || !int.TryParse(context.Request["id"], out mailId)) return;
            var message = new SAAO.Mail(mailId);
            if (message.Username == SAAO.User.Current.Username)
                message.MoveTo("Trash");
            else
                R.Flag = 2;
        }
        else if (context.Request["action"] == "send")
        {
            if (context.Request.Form["to"] == null) return;
            var to = context.Request.Form["to"].Split(',');
            if (to.Length == 0) return;
            foreach (var receiver in to)
                SAAO.Mail.Send(
                    @from: SAAO.User.Current.Username + "@" + SAAO.Mail.MailDomain, 
                    receiver: receiver, 
                    subject: context.Request.Form["subject"], 
                    isBodyHtml: true, 
                    body: context.Request.Form["content"]
                );
        }
        else if (context.Request["action"] == "login")
        {
            R.Data = new JObject
            {
                ["mail"] = SAAO.User.Current.Username + "@" + SAAO.Mail.MailDomain,
                ["password"] = SAAO.User.Current.PasswordRaw
            };
        }
    }
}