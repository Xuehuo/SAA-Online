<%@ WebHandler Language="C#" Class="mailHandler" %>
using System;
using System.Web;
using System.Web.SessionState;
public class mailHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list")
        {
            string[] folder = {"INBOX", "Sent", "Drafts", "Trash"};
            if (context.Request["folder"] != null && Array.IndexOf(folder, context.Request["folder"]) != -1)
            {
                try
                {
                    context.Response.Write("{\"flag\":0,\"data\":" +
                                           SAAO.Mail.ListJson(context.Request["folder"]) + "}");
                }
                catch (Exception ex)
                {
                    SAAO.Utility.Log(ex);
                    context.Response.Write("{\"flag\":3}");
                }
            }
        }
        else if (context.Request["action"] == "info")
        {
            {
                int mailId;
                if (context.Request["id"] != null && int.TryParse(context.Request["id"], out mailId))
                {
                    try
                    {
                        SAAO.Mail message = new SAAO.Mail(mailId);
                        if (message.Username == SAAO.User.Current.Username)
                            context.Response.Write("{\"flag\":0,\"data\":" + message.ToJson() + "}");
                        else
                            context.Response.Write("{\"flag\":2}");
                    }
                    catch (Exception ex)
                    {
                        SAAO.Utility.Log(ex);
                        context.Response.Write("{\"flag\":3}");
                    }
                }
            }
        }
        else if (context.Request["action"] == "attachment")
        {
            {
                int mailId;
                int index;
                if (context.Request["id"] != null && context.Request["index"] != null &&
                    int.TryParse(context.Request["id"], out mailId) &&
                    int.TryParse(context.Request["index"], out index))
                {
                    SAAO.Mail message = new SAAO.Mail(mailId);
                    if (message.Username == SAAO.User.Current.Username)
                    {
                        message.DownloadAttachment(index);
                    }
                    else
                    {
                        context.Response.ContentType = "text/plain; charset=utf-8";
                        context.Response.Write("附件无法读取，访问被拒绝。");
                    }
                }
            }
        }
        else if (context.Request["action"] == "display")
        {
            {
                int mailId;
                if (context.Request["id"] != null && int.TryParse(context.Request["id"], out mailId))
                {
                    try
                    {
                        context.Response.ContentType = "text/html; charset=utf-8";
                        SAAO.Mail message = new SAAO.Mail(mailId);
                        if (message.Username == SAAO.User.Current.Username)
                        {
                            context.Response.Write(message.Body());
                            message.SetFlag(SAAO.Mail.MailFlag.Seen);
                        }
                        else
                            context.Response.Write("邮件无法加载，访问被拒绝。");
                    }
                    catch (Exception ex)
                    {
                        SAAO.Utility.Log(ex);
                        context.Response.Write("邮件无法加载，服务器错误。");
                    }
                }
            }
        }
        else if (context.Request["action"] == "delete")
        {
            {
                int mailId;
                if (context.Request["id"] != null && int.TryParse(context.Request["id"], out mailId))
                {
                    try
                    {
                        SAAO.Mail message = new SAAO.Mail(mailId);
                        if (message.Username == SAAO.User.Current.Username)
                        {
                            message.MoveTo("Trash");
                            context.Response.Write("{\"flag\":0}");
                        }
                        else
                            context.Response.Write("{\"flag\":2}");
                    }
                    catch (Exception ex)
                    {
                        SAAO.Utility.Log(ex);
                        context.Response.Write("{\"flag\":3}");
                    }
                }
            }
        }
        else if (context.Request["action"] == "send")
        {
            if (context.Request.Form["to"] != null)
            {
                try
                {
                    string[] to = context.Request.Form["to"].Split(',');
                    if (to.Length != 0)
                    {
                        foreach (string receiver in to)
                        {
                            SAAO.Mail.Send(SAAO.User.Current.Mail, receiver, context.Request.Form["subject"], true, context.Request.Form["content"]);
                        }
                        context.Response.Write("{\"flag\":0}");
                    }
                    else
                        context.Response.Write("{\"flag\":2}");
                }
                catch (Exception ex)
                {
                    SAAO.Utility.Log(ex);
                    context.Response.Write("{\"flag\":3}");
                }
            }
        }
        else if (context.Request["action"] == "login")
        {
            context.Response.Write("{\"flag\":0,\"data\":{\"mail\":\"" + SAAO.User.Current.Mail + "\",\"password\":\"" +
                                   SAAO.User.Current.PasswordRaw + "\"}}");
        }
    }
    public bool IsReusable => false;
}