<%@ WebHandler Language="C#" Class="mailHandler" %>
using System;
using System.Web;
using System.Net.Mail;
using System.Text;
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
                            SAAO.SqlIntegrate si = new SAAO.SqlIntegrate(SAAO.Mail.ConnStr);
                            int uid = Convert.ToInt32(si.Query($"SELECT accountid FROM hm_accounts WHERE accountaddress = '{SAAO.User.Current.Mail}'"));
                            int folderid = Convert.ToInt32(si.Query($"SELECT folderid FROM hm_imapfolders WHERE foldername = \'Trash\' AND folderaccountid = {uid}"));
                            message.MoveTo(folderid);
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
                            MailMessage mail = new MailMessage(SAAO.User.Current.Mail, receiver)
                            {
                                SubjectEncoding = Encoding.UTF8,
                                Subject = context.Request.Form["subject"],
                                IsBodyHtml = true,
                                BodyEncoding = Encoding.UTF8,
                                Body = SAAO.Utility.Base64Decode(context.Request.Form["content"])
                            };
                            SmtpClient smtp = new SmtpClient(SAAO.Mail.ServerAddress);
                            smtp.Credentials = new System.Net.NetworkCredential(SAAO.User.Current.Mail,
                                SAAO.User.Current.PasswordRaw);
                            smtp.Send(mail);
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