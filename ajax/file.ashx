<%@ WebHandler Language="C#" Class="FileHandler" %>
using System;
using System.Linq;
using Newtonsoft.Json.Linq;

public class FileHandler : AjaxHandler
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "upload")
        {
            if (context.Request.Files.Count == 0) return;
            SAAO.File.Upload(context.Request.Files[0]);
        }
        else if (context.Request["action"] == "list")
        {
            R.Data = SAAO.File.ListJson(SAAO.Organization.Current.State.EventStart, SAAO.Organization.Current.State.EventEnd);
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
                foreach (var tag in tags.Except(tagsOriginal))
                    file.AddTag(tag);
                foreach (var tag in tagsOriginal.Except(tags))
                    file.RemoveTag(tag);
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
            {
                file.Download();
                R.Flag = -1;
            }
            else
                R.Flag = 2;
        }
        else if (context.Request["action"] == "towechat")
        {
            if (context.Request["id"] == null) return;
            Guid guid;
            if (!Guid.TryParse(context.Request["id"], out guid)) return;
            var file = new SAAO.File(guid.ToString().ToUpper());
            if (SAAO.User.Current.Wechat != "" && file.Visible(SAAO.User.Current))
            {
                var o = new JObject
                {
                    ["touser"] = SAAO.User.Current.Wechat,
                    ["msgtype"] = "file",
                    ["agentid"] = 4,
                    ["file"] = new JObject { ["media_id"] = file.MediaId }
                };
                var result = SAAO.Utility.HttpRequestJson("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + SAAO.Utility.GetAccessToken(), o.ToString());
                if (result["errcode"].ToString() == "0")
                    R.Flag = 1;
                else
                {
                    R.Flag = -1;
                    SAAO.Utility.Log(result.ToString());
                }
            }
            else
            {
                context.Response.Write("-1");
            }
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