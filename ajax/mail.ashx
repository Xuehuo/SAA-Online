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
        if (context.Request["action"] != null && SAAO.User.IsLogin)
            switch (context.Request["action"].ToString())
            {
                case "list": // list mail of a folder
                    string[] folder = new string[] { "INBOX", "Sent", "Drafts", "Trash" };
                    if (context.Request["folder"] != null && Array.IndexOf(folder, context.Request["folder"].ToString()) != -1)
                    {
                        try
                        {
                            context.Response.Write("{\"flag\":0,\"data\":" + SAAO.Mail.ListJSON(context.Request["folder"].ToString()) + "}");
                        }
                        catch (Exception ex)
                        {
                            SAAO.Utility.Log(ex);
                            context.Response.Write("{\"flag\":3}");
                        }
                    }
                    break;
                case "info": // obtain information of a mail
                    {
                        int mailID = -1;
                        if (context.Request["id"] != null && int.TryParse(context.Request["id"].ToString(), out mailID))
                        {
                            try
                            {
                                SAAO.Mail message = new SAAO.Mail(mailID);
                                if (message.username == SAAO.User.Current.username)
                                    context.Response.Write("{\"flag\":0,\"data\":"+message.ToJSON()+"}");
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
                    break;
                case "attachment": // download mail attachment
                    {
                        int mailID = -1;
                        int index = -1;
                        if (context.Request["id"] != null && context.Request["index"] != null && int.TryParse(context.Request["id"].ToString(), out mailID) && int.TryParse(context.Request["index"].ToString(), out index))
                        {
                            SAAO.Mail message = new SAAO.Mail(mailID);
                            if (message.username == SAAO.User.Current.username)
                            {
                                string filename = message.GetAttachmentName(index);
                                context.Response.ContentType = "application/octet-stream";
                                if (context.Request.UserAgent.ToLower().IndexOf("trident") > -1)
                                    filename = BitConverter.ToString(ASCIIEncoding.Default.GetBytes(filename)).Replace("-", " ");
                                if (context.Request.UserAgent.ToLower().IndexOf("firefox") > -1)
                                    context.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
                                else
                                    context.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                                context.Response.WriteFile(message.GetAttachmentPath(index));
                                context.Response.End();
                            }
                            else
                            {
                                context.Response.ContentType = "text/plain; charset=utf-8";
                                context.Response.Write("附件无法读取，访问被拒绝。");
                            }
                        }
                    }
                    break;
                case "display": // display a mail body
                    {
                        int mailID = -1;
                        if (context.Request["id"] != null && int.TryParse(context.Request["id"].ToString(), out mailID))
                        {
                            try
                            {
                                context.Response.ContentType = "text/html; charset=utf-8";
                                SAAO.Mail message = new SAAO.Mail(mailID);
                                if (message.username == SAAO.User.Current.username)
                                {
                                    context.Response.Write(message.Body());
                                    message.SetFlag(SAAO.Mail.mailFlag.Seen);
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
                    break;
                case "delete": // delete a mail
                    {
                        int mailID = -1;
                        if (context.Request["id"] != null && int.TryParse(context.Request["id"].ToString(), out mailID))
                        {
                            try
                            {
                                SAAO.Mail message = new SAAO.Mail(mailID);
                                if (message.username == SAAO.User.Current.username)
                                {
                                    SAAO.SqlIntegrate si = new SAAO.SqlIntegrate(SAAO.Mail.connStr);
                                    int uid = Convert.ToInt32(si.Query("SELECT accountid FROM hm_accounts WHERE accountaddress = '" + SAAO.User.Current.mail + "'"));
                                    int folderid = Convert.ToInt32(si.Query("SELECT folderid FROM hm_imapfolders WHERE foldername = 'Trash' AND folderaccountid = " + uid));
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
                    break;
                case "send": // send a mail
                    if (context.Request.Form["to"] != null)
                    {
                        try
                        {
                            string[] to = context.Request.Form["to"].ToString().Split(',');
                            if (to.Length != 0)
                            {
                                for (int i = 0; i < to.Length; i++)
                                {
                                    MailMessage mail = new MailMessage(SAAO.User.Current.mail, to[i]);
                                    mail.SubjectEncoding = Encoding.UTF8;
                                    mail.Subject = context.Request.Form["subject"].ToString();
                                    mail.IsBodyHtml = true;
                                    mail.BodyEncoding = Encoding.UTF8;
                                    mail.Body = SAAO.Utility.Base64Decode(context.Request.Form["content"].ToString());
                                    SmtpClient smtp = new SmtpClient(SAAO.Mail.serverAddress);
                                    smtp.Credentials = new System.Net.NetworkCredential(SAAO.User.Current.mail, SAAO.User.Current.passwordRaw);
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
                    break;
                case "login": // obtain mail login credential
                    context.Response.Write("{\"flag\":0,\"data\":{\"mail\":\"" + SAAO.User.Current.mail + "\",\"password\":\"" + SAAO.User.Current.passwordRaw + "\"}}");
                    break;
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