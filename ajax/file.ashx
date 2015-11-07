<%@ WebHandler Language="C#" Class="fileHandler" %>
using System;
using System.Web;
using System.Web.SessionState;
using System.IO;
public class fileHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request["action"] != null && SAAO.User.IsLogin)
        {
            switch(context.Request["action"].ToString())
            {
                case "upload":
                    if (context.Request.Files.Count != 0)
                    {
                        try
                        {
                            for (int i = 0; i < context.Request.Files.Count; i++)
                            {
                                HttpPostedFile file = context.Request.Files[i];
                                string guid = Guid.NewGuid().ToString().ToUpper();
                                file.SaveAs(SAAO.File.storagePath + guid);
                                SAAO.SqlIntegrate si = new SAAO.SqlIntegrate(SAAO.Utility.connStr);
                                si.InitParameter(2);
                                si.AddParameter("@name", SAAO.SqlIntegrate.DataType.VarChar, file.FileName.Remove(file.FileName.LastIndexOf(".")), 50);
                                si.AddParameter("@extension", SAAO.SqlIntegrate.DataType.VarChar, file.FileName.Substring(file.FileName.LastIndexOf(".") + 1).ToLower(), 10);
                                si.Execute("INSERT INTO [File] ([GUID],[name],[extension],[size],[uploader]) VALUES ('" + guid + "',@name,@extension," + file.ContentLength + ",'" + SAAO.User.Current.UUID + "')");
                            }
                            context.Response.Write("{\"flag\":0}");
                        }
                        catch (Exception ex)
                        {
                            context.Response.Write("{\"flag\":3}");
                            SAAO.Utility.Log(ex);
                        }
                    }
                    break;
                case "list":
                    try
                    {
                        context.Response.Write("{\"flag\":0,\"data\":" + SAAO.File.ListJSON() + "}");
                    }
                    catch (Exception ex)
                    {
                        context.Response.Write("{\"flag\":3}");
                        SAAO.Utility.Log(ex);
                    }
                    break;
                case "info":
                    {
                        Guid guid = new Guid();
                        if (Guid.TryParse(context.Request["id"].ToString(), out guid))
                        {
                            SAAO.File file = new SAAO.File(guid.ToString().ToUpper());
                            if (file.Visible(SAAO.User.Current))
                            {
                                try
                                {
                                    context.Response.Write("{\"flag\":0,\"data\":" + file.ToJSON() + "}");
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
                    break;
                case "update":
                    {
                        try
                        {
                            Guid guid = new Guid();
                            if (Guid.TryParse(context.Request["id"].ToString(), out guid))
                            {
                                SAAO.File file = new SAAO.File(guid.ToString().ToUpper());
                                if (file.Visible(SAAO.User.Current))
                                {
                                    file.Name = context.Request.Form["name"];
                                    file.Info = context.Request.Form["info"];
                                    file.Permission = (SAAO.File.permissionLevel)int.Parse(context.Request.Form["permission"]);
                                    string[] tags = context.Request.Form["tag"].Split(',');
                                    string[] tagsOriginal = file.ListTag().Split(',');
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
                    break;
                case "download":
                    {
                        Guid guid = new Guid();
                        if (Guid.TryParse(context.Request["id"].ToString(), out guid))
                        {
                            SAAO.File file = new SAAO.File(guid.ToString().ToUpper());
                            if (file.Visible(SAAO.User.Current))
                            {
                                new SAAO.SqlIntegrate(SAAO.Utility.connStr).Execute("UPDATE [File] SET [downloadCount] = [downloadCount] + 1 WHERE [GUID] = '" + guid.ToString().ToUpper() + "'");
                                string fileName = file.Name + "." + file.extension;
                                FileInfo fileInfo = new FileInfo(file.savePath);
                                if (fileInfo.Exists)
                                {
                                    const long ChunkSize = 102400;
                                    byte[] buffer = new byte[ChunkSize];
                                    context.Response.Clear();
                                    FileStream iStream = File.OpenRead(file.savePath);
                                    long dataLengthToRead = iStream.Length;
                                    context.Response.ContentType = "application/octet-stream";
                                    if (context.Request.UserAgent.ToLower().IndexOf("trident") > -1)
                                        context.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName));
                                    if (context.Request.UserAgent.ToLower().IndexOf("firefox") > -1)
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
                            else
                            {
                                context.Response.Write("Invalid Request!");
                            }
                        }
                    }
                    break;
                case "delete":
                    {
                        try
                        {
                            if (context.Request["id"] != null)
                            {
                                Guid guid = new Guid();
                                if (Guid.TryParse(context.Request["id"].ToString(), out guid))
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
                    break;
            }
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