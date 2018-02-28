using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

public class FileHandler : SAAO.AjaxHandler
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "upload")
        {
            if (context.Request.Files.Count == 0) return;
            var upfile = SAAO.File.Upload(context.Request.Files[0]);


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
                        (SAAO.File.PermissionLevel)int.Parse(context.Request.Form["permission"]);
                else
                    file.Permission = SAAO.File.PermissionLevel.All;
                var tags = context.Request.Form["tag"].Split(',');
                var tagsOriginal = file.Tag.ToArray();
                foreach (var tag in tags.Except(tagsOriginal))
                    file.AddTag(tag);
                foreach (var tag in tagsOriginal.Except(tags))
                    file.RemoveTag(tag);

                if(file.Uploader.UUID==SAAO.User.Current.UUID)  // Owner Check
                {
                    if((DateTime.Now- file.UploadTime).Minutes<=5)  //上传5分钟内修改 推动上传通知
                    {                       
                        //Wechat File Upload Event Push Service
                        List<string> lst = file.GetVisibleUserWechat();
                        string rec = string.Join("|", lst.ToArray());
                        var oText = new JObject
                        {
                            ["touser"] = rec,
                            ["msgtype"] = "text",
                            ["agentid"] = 4,
                            ["text"] = new JObject {
                                ["content"] = "新文件提醒：\n" +
                                              "文件名：" + file.Name + "\n" +
                                              "上传者：" + file.Uploader.Realname + "\n" +
                                              "上传时间：" + string.Format("{0:f}", file.UploadTime) +"\n" +
                                              "备注："+file.Info
                            }
                        };

                        var oFile = new JObject
                        {
                            ["touser"] = rec,
                            ["msgtype"] = "file",
                            ["agentid"] = 4,
                            ["file"] = new JObject { ["media_id"] = file.MediaId }
                        };
                        SAAO.Utility.SendMessgaeBySAAOHelper(oFile);
                        SAAO.Utility.SendMessgaeBySAAOHelper(oText);
                    }
                }
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
            if (SAAO.User.Current.Wechat == "" || !file.Visible(SAAO.User.Current)) return;
            if (file.MediaId == "") return;
            var o = new JObject
            {
                ["touser"] = SAAO.User.Current.Wechat,
                ["msgtype"] = "file",
                ["agentid"] = 4,
                ["file"] = new JObject { ["media_id"] = file.MediaId }
            };
            SAAO.Utility.SendMessgaeBySAAOHelper(o);
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