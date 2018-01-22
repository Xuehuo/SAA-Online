using System;
using Newtonsoft.Json.Linq;

public class MailHandler : SAAO.AjaxHandler
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list")
        {
            R.Data = SAAO.Mail.ListJson();
        }
        else if (context.Request["action"] == "info")
        {
            Guid mailId;
            if (context.Request["id"] == null || !Guid.TryParse(context.Request["id"], out mailId)) return;
            var message = new SAAO.Mail(mailId);
            R.Data = JObject.FromObject(message);
        }
        else if (context.Request["action"] == "attachment")
        {
            Guid attachmentId;
            if (context.Request["id"] == null || !Guid.TryParse(context.Request["id"], out attachmentId)) return;
            if (context.Request["name"] == null) return;
            SAAO.Mail.DownloadAttachment(attachmentId, context.Request["name"]);
            R.Flag = -1;
        }
    }
}