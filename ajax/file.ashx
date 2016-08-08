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
                    for (int i = 0; i < context.Request.Files.Count; i++)
                    {
                        HttpPostedFile file = context.Request.Files[i];
                        string guid = Guid.NewGuid().ToString().ToUpper();
                        file.SaveAs(SAAO.File.StoragePath + guid);
                        SAAO.SqlIntegrate si = new SAAO.SqlIntegrate(SAAO.Utility.ConnStr);
                        si.InitParameter(2);
                        si.AddParameter("@name", SAAO.SqlIntegrate.DataType.VarChar,
                            file.FileName.Remove(file.FileName.LastIndexOf(".")), 50);
                        si.AddParameter("@extension", SAAO.SqlIntegrate.DataType.VarChar,
                            file.FileName.Substring(file.FileName.LastIndexOf(".") + 1).ToLower(), 10);
                        si.Execute($"INSERT INTO [File] ([GUID],[name],[extension],[size],[uploader]) VALUES ('{guid}',@name,@extension,{file.ContentLength},'{SAAO.User.Current.UUID}')");
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