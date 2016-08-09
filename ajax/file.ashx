<%@ WebHandler Language="C#" Class="fileHandler" %>
using System;
using System.Web;
using System.Web.SessionState;
public class fileHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "upload")
        {
            if (context.Request.Files.Count != 0)
            {
                try
                {
                    foreach (HttpPostedFile file in context.Request.Files)
                    {
                        SAAO.File.Upload(file);
                    }
                    context.Response.Write("{\"flag\":0}");
                }
                catch (Exception ex)
                {
                    context.Response.Write("{\"flag\":3}");
                    SAAO.Utility.Log(ex);
                }
            }
        }
        else if (context.Request["action"] == "list")
        {
            try
            {
                context.Response.Write("{\"flag\":0,\"data\":" + SAAO.File.ListJson() + "}");
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"flag\":3}");
                SAAO.Utility.Log(ex);
            }
        }
        else if (context.Request["action"] == "info")
        {
            {
                Guid guid;
                if (Guid.TryParse(context.Request["id"], out guid))
                {
                    SAAO.File file = new SAAO.File(guid.ToString().ToUpper());
                    if (file.Visible(SAAO.User.Current))
                    {
                        try
                        {
                            context.Response.Write("{\"flag\":0,\"data\":" + file.ToJson() + "}");
                        }
                        catch (Exception ex)
                        {
                            context.Response.Write("{\"flag\":3}");
                            SAAO.Utility.Log(ex);
                        }
                    }
                    else
                        context.Response.Write("{\"flag\":2}");
                }
                else
                    context.Response.Write("{\"flag\":2}");
            }
        }
        else if (context.Request["action"] == "update")
        {
            {
                try
                {
                    Guid guid;
                    if (Guid.TryParse(context.Request["id"], out guid))
                    {
                        SAAO.File file = new SAAO.File(guid.ToString().ToUpper());
                        if (file.Visible(SAAO.User.Current))
                        {
                            file.Name = context.Request.Form["name"];
                            file.Info = context.Request.Form["info"];
                            if (context.Request.Form["permission"] != "")
                                file.Permission =
                                    (SAAO.File.PermissionLevel) int.Parse(context.Request.Form["permission"]);
                            else
                                file.Permission = SAAO.File.PermissionLevel.All;
                            string[] tags = context.Request.Form["tag"].Split(',');
                            string[] tagsOriginal = file.Tag.ToArray();
                            foreach (string tag in tagsOriginal)
                            {
                                bool exist = false;
                                for (int i = 0; i < tags.Length; i++)
                                    if (tag == tagsOriginal[i])
                                    {
                                        exist = true;
                                        break;
                                    }
                                if (!exist)
                                    file.RemoveTag(tag);
                            }
                            foreach (string tag in tags)
                                if (!file.HasTag(tag))
                                    file.AddTag(tag);
                            context.Response.Write("{\"flag\":0}");
                        }
                        else
                            context.Response.Write("{\"flag\":2}");
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
        else if (context.Request["action"] == "download")
        {
            Guid guid;
            if (Guid.TryParse(context.Request["id"], out guid))
            {
                SAAO.File file = new SAAO.File(guid.ToString().ToUpper());
                if (file.Visible(SAAO.User.Current))
                {
                    file.Download();
                }
                else
                {
                    context.Response.Write("Invalid Request!");
                }
            }
        }
        else if (context.Request["action"] == "delete")
        {
            try
            {
                if (context.Request["id"] != null)
                {
                    Guid guid;
                    if (Guid.TryParse(context.Request["id"], out guid))
                    {
                        SAAO.File file = new SAAO.File(guid.ToString().ToUpper());
                        if (file.Visible(SAAO.User.Current))
                        {
                            file.Delete();
                            context.Response.Write("{\"flag\":0}");
                        }
                        else
                            context.Response.Write("{\"flag\":2}");
                    }
                    else
                        context.Response.Write("{\"flag\":2}");
                }
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