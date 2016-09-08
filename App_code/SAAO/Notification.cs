using System;
using System.Data;
using Newtonsoft.Json.Linq;

namespace SAAO
{
    /// <summary>
    /// Notification 通知
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// File storage path of supervising notification
        /// </summary>
        public static readonly string StoragePath = System.Configuration.ConfigurationManager.AppSettings["fileStoragePath"] + @"report\";

        public int Id;
        public PermissionType Type;

        private readonly int _group;

        public string Title;
        public string Content;

        public DateTime NotifyTime;

        public enum PermissionType
        {
            All = 0,
            SelfGroupOnly = 1,
            Supervise = 2
        }
        /// <summary>
        /// Notification constructor (obtain a current one)
        /// </summary>
        /// <param name="id">Notification ID in database</param>
        public Notification(int id)
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@ID", SqlIntegrate.DataType.Int, id);
            var dr = si.Reader("SELECT * FROM Notification WHERE ID = @ID");
            Content = dr["content"].ToString();
            Title = dr["title"].ToString();
            Id = id;
            Type = (PermissionType)int.Parse(dr["type"].ToString());
            _group = -1;
            if (Type == PermissionType.SelfGroupOnly)
                _group = new User(Guid.Parse(dr["UUID"].ToString())).Group;
            NotifyTime = Convert.ToDateTime(dr["notifyTime"].ToString());
        }

        /// <summary>
        /// Notification constructor (create a new one)
        /// </summary>
        /// <param name="title">Notification title</param>
        /// <param name="content">Notification content</param>
        /// <param name="type">Notification type</param>
        public Notification(string title, string content, PermissionType type)
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@title", SqlIntegrate.DataType.NVarChar, title, 50);
            si.AddParameter("@content", SqlIntegrate.DataType.Text, content);
            si.AddParameter("@type", SqlIntegrate.DataType.Int, (int)type);
            si.AddParameter("@UUID", SqlIntegrate.DataType.VarChar, User.Current.UUID);
            Id = Convert.ToInt32(si.Query("INSERT INTO Notification ([title], [content], [type], [UUID]) VALUES (@title, @content, @type, @UUID); SELECT @@IDENTITY"));
            Type = type;
            Title = title;
            Content = content;
            _group = -1;
            if (type == PermissionType.SelfGroupOnly)
                _group = User.Current.Group;
            NotifyTime = DateTime.Now;
        }

        /// <summary>
        /// Broadcast the notification by sending email
        /// </summary>
        public void Broadcast()
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            DataTable dt;
            if (_group == -1)
                dt = si.Adapter("SELECT [wechat] FROM [User] WHERE activated = 1 AND [wechat] != ''");
            else
            {
                si.AddParameter("@group", SqlIntegrate.DataType.Int, _group);
                dt = si.Adapter("SELECT [wechat] FROM [User] WHERE activated = 1 AND [group] = @group AND [wechat] != ''");
            }
            var toSend = "";
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                if (toSend != "")
                    toSend += "|";
                toSend += dt.Rows[i]["wechat"].ToString();
            }
            var o = new JObject
            {
                ["touser"] = toSend,
                ["msgtype"] = "news",
                ["agentid"] = 4,
                ["news"] = new JObject
                {
                    ["articles"] = new JArray
                    {
                        new JObject
                        {
                            ["title"] = Title,
                            ["description"] = Content
                        }
                    }
                }
            };
            Utility.HttpRequest($"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={Utility.GetAccessToken()}", o);
        }

        /// <summary>
        /// Set an important flag
        /// </summary>
        public void SetImportant()
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@ID", SqlIntegrate.DataType.Int, Id);
            si.Execute("UPDATE Notification SET important = ((SELECT MAX(important) FROM Notification) + 1) WHERE ID = @ID");
        }

        /// <summary>
        /// Attach supervising report
        /// </summary>
        /// <param name="guid">Supervising report storage GUID</param>
        public void AttachReport(string guid)
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@reportFile", SqlIntegrate.DataType.VarChar, guid);
            si.AddParameter("@ID", SqlIntegrate.DataType.Int, Id);
            si.Execute("UPDATE Notification SET reportFile = @reportFile WHERE ID = @ID");
        }

        /// <summary>
        /// Check whether a user can see a notification (static function)
        /// </summary>
        /// <param name="group">Group of a notification</param>
        /// <param name="type">Notification permission type</param>
        /// <returns>Whether a user can see a notification</returns>
        private static bool Visible(int group, PermissionType type)
        {
            if (type == PermissionType.Supervise)
                return true;
            if (type == PermissionType.All)
                return true;
            if (User.Current.IsExecutive)
                return true;
            if (type == PermissionType.SelfGroupOnly && User.Current.Group != group)
                return false;
            return true;
        }
        /// <summary>
        /// Check whether a user can see the notification
        /// </summary>
        /// <returns>whether a user can see the notification</returns>
        public bool Visible()
        {
            return Visible(_group, Type);
        }

        /// <summary>
        /// List current notifications in the database in JSON
        /// </summary>
        /// <returns>JSON of current notifications [{ID,title,user,content,notifyTime,type,imprtant,(reportFile)},...]</returns>
        public static JArray ListJson()
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@start", SqlIntegrate.DataType.Date, Organization.Current.State.EventStart);
            si.AddParameter("@end", SqlIntegrate.DataType.Date, Organization.Current.State.EventEnd);
            var dt = si.Adapter("SELECT [Notification].[reportFile], [Notification].[ID], [Notification].[important], [Notification].[type], [Notification].[title], [Notification].[content], [Notification].[notifyTime], [User].[realname], [User].[group] FROM [Notification] INNER JOIN [User] ON [Notification].[UUID] = [User].[UUID] AND [Notification].[notifyTime] BETWEEN @start AND @end ORDER BY Notification.important DESC, ID DESC");
            var a = new JArray();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                if (!Visible(Convert.ToInt32(dt.Rows[i]["group"]), PermissionType.SelfGroupOnly)) continue;
                var o = new JObject
                {
                    ["ID"] = dt.Rows[i]["ID"].ToString(),
                    ["title"] = dt.Rows[i]["title"].ToString(),
                    ["user"] = dt.Rows[i]["realname"].ToString(),
                    ["content"] = dt.Rows[i]["content"].ToString(),
                    ["notifyTime"] =
                        Convert.ToDateTime(dt.Rows[i]["notifyTime"]).ToString("yyyy-MM-dd HH:mm"),
                    ["type"] = dt.Rows[i]["type"].ToString(),
                    ["important"] = Convert.ToInt32(dt.Rows[i]["important"])
                };
                if (Convert.ToInt32(dt.Rows[i]["type"]) == (int)PermissionType.Supervise)
                    o["reportFile"] = dt.Rows[i]["reportFile"].ToString();
                a.Add(o);
            }
            return a;
        }
    }
}