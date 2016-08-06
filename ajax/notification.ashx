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
        if (context.Request["action"] != null && SAAO.User.IsLogin)
        {
            switch (context.Request["action"].ToString())
            {
                case "list": // list current notifications
                    try
                    {
                        context.Response.Write("{\"flag\":0,\"data\":" + SAAO.Notification.ListJSON() + "}");
                    }
                    catch (Exception ex)
                    {
                        context.Response.Write("{\"flag\":3}");
                        SAAO.Utility.Log(ex);
                    }
                    break;
                case "report": // download a supervising report
                    {
                        Guid guid = new Guid();
                        if (Guid.TryParse(context.Request["id"].ToString(), out guid))
                        {
                            FileInfo fileInfo = new FileInfo(SAAO.Notification.storagePath + context.Request["id"].ToString());
                            if (fileInfo.Exists)
                            {
                                string fileName = "监督报告";
                                const long ChunkSize = 102400;
                                byte[] buffer = new byte[ChunkSize];
                                context.Response.Clear();
                                FileStream iStream = File.OpenRead(SAAO.Notification.storagePath + context.Request["id"].ToString());
                                long dataLengthToRead = iStream.Length;
                                context.Response.ContentType = "application/pdf";
                                if (context.Request["download"] != null)
                                    if (context.Request.UserAgent.ToLower().IndexOf("trident") > -1)
                                        context.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName));
                                    else if (context.Request.UserAgent.ToLower().IndexOf("firefox") > -1)
                                        context.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
                                    else
                                        context.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                                while (dataLengthToRead > 0 && context.Response.IsClientConnected)
                                {
                                    int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(ChunkSize));
                                    context.Response.OutputStream.Write(buffer, 0, lengthRead);
                                    context.Response.Flush();
                                    dataLengthToRead = dataLengthToRead - lengthRead;
                                }
                                iStream.Close();
                            }
                            else
                            {
                                context.Response.Write("Invalid Request!");
                            }
                        }
                        break;
                    }
                case "create": // create a notification
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
                                SAAO.Notification n = new SAAO.Notification(context.Request.Form["title"], context.Request.Form["content"], (SAAO.Notification.permissionType)int.Parse(context.Request.Form["type"]));

                                if (context.Request.Form["type"] == "2")
                                {
                                    string guid = Guid.NewGuid().ToString("D").ToUpper();
                                    context.Request.Files[0].SaveAs(SAAO.Notification.storagePath + guid);
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
                        break;
                    }
            }
        }
    }
    public bool IsReusable {
        get {
            return false;
        }
    }
}