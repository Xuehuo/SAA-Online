<%@ WebHandler Language="C#" Class="FileHandler" %>
using System;
public class FileHandler : Ajax
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "upload")
        {
            if (context.Request.Files.Count == 0) return;
            foreach (System.Web.HttpPostedFile file in context.Request.Files)
                SAAO.File.Upload(file);
        }
        else if (context.Request["action"] == "list")
        {
            R.Data = SAAO.File.ListJson();
        }
        else if (context.Request["action"] == "info")
        {
            if (context.Request["id"] == null) return;
            Guid guid;
            if (!Guid.TryParse(context.Request["id"], out guid)) return;
            var file = new SAAO.File(guid.ToString().ToUpper());
            if (file.Visible(SAAO.User.Current))
                R.Data = file.ToJson();
            else
                R.Flag = 2;
        }
        else if (context.Request["action"] == "update")
        {
            if (context.Request["id"] == null) return;
            Guid guid;
            if (!Guid.TryParse(context.Request["id"], out guid)) return;
            var file = new SAAO.File(guid.ToString().ToUpper());
            if (file.Visible(SAAO.User.Current))
            {
                file.Name = context.Request.Form["name"];
                file.Info = context.Request.Form["info"];
                if (context.Request.Form["permission"] != "")
                    file.Permission =
                        (SAAO.File.PermissionLevel) int.Parse(context.Request.Form["permission"]);
                else
                    file.Permission = SAAO.File.PermissionLevel.All;
                var tags = context.Request.Form["tag"].Split(',');
                var tagsOriginal = file.Tag.ToArray();
                foreach (var tag in tagsOriginal)
                {
                    bool exist = false;
                    for (var i = 0; i < tags.Length; i++)
                        if (tag == tagsOriginal[i])
                        {
                            exist = true;
                            break;
                        }
                    if (!exist)
                        file.RemoveTag(tag);
                }
                foreach (var tag in tags)
                    if (!file.HasTag(tag))
                        file.AddTag(tag);
            }
            else
                R.Flag = 2;
        }
        else if (context.Request["action"] == "download")
        {
            if (context.Request["id"] == null) return;
            Guid guid;
            if (!Guid.TryParse(context.Request["id"], out guid)) return;
            var file = new SAAO.File(guid.ToString().ToUpper());
            if (file.Visible(SAAO.User.Current))
                file.Download();
            else
                R.Flag = 2;
        }
        else if (context.Request["action"] == "delete")
        {
            if (context.Request["id"] == null) return;
            Guid guid;
            if (!Guid.TryParse(context.Request["id"], out guid)) return;
            var file = new SAAO.File(guid.ToString().ToUpper());
            if (file.Visible(SAAO.User.Current))
                file.Delete();
            else
                R.Flag = 2;
        }
    }
}