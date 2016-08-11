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
        private static readonly string NotifyMail = System.Configuration.ConfigurationManager.AppSettings["notifyMailAddress"];
        private static readonly string NotifyMailCredential = System.Configuration.ConfigurationManager.AppSettings["notifyMailCredential"];

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
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            var dr = si.Reader($"SELECT * FROM Notification WHERE ID ={id}");
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
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@content", SqlIntegrate.DataType.Text, content);
            si.AddParameter("@title", SqlIntegrate.DataType.NVarChar, title, 50);
            Id = Convert.ToInt32(si.Query($"INSERT INTO Notification ([title], [content], [type], [UUID]) VALUES (@title, @content, {(int) type}, \'{User.Current.UUID}\'); SELECT @@IDENTITY"));
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
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            DataTable dt = si.Adapter($"SELECT mail, SN FROM [User] WHERE activated = 1{(_group != -1 ? " AND [group] =" + _group : "")}");

            for (int i = 0; i < dt.Rows.Count; i++)
                SendMail(dt.Rows[i]["mail"].ToString());
        }

        /// <summary>
        /// Set an important flag
        /// </summary>
        public void SetImportant()
        {
            int important = Convert.ToInt32(new SqlIntegrate(Utility.ConnStr).Query("SELECT MAX(important) FROM Notification"));
            new SqlIntegrate(Utility.ConnStr).Execute($"UPDATE Notification SET important ={(important + 1)} WHERE ID={Id}");
        }

        /// <summary>
        /// Attach supervising report
        /// </summary>
        /// <param name="guid">Supervising report storage GUID</param>
        public void AttachReport(string guid)
        {
            new SqlIntegrate(Utility.ConnStr).Execute("UPDATE Notification SET reportFile ='" + guid + "' WHERE ID=" + Id);
        }

        /// <summary>
        /// Send a notification email
        /// </summary>
        /// <param name="to">Receiver email</param>
        private void SendMail(string to)
        {
            Mail.Send(
                @from: NotifyMail, 
                receiver: to, 
                subject: Title, 
                isBodyHtml: false, 
                body: Content, 
                credential: new System.Net.NetworkCredential(NotifyMail, NotifyMailCredential)
            );
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
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            DataTable dt = si.Adapter("SELECT Notification.reportFile, Notification.ID, Notification.important, Notification.type, Notification.title, Notification.[content], Notification.notifyTime, [User].realname, [User].[group] FROM Notification INNER JOIN [User] ON Notification.UUID = [User].UUID ORDER BY Notification.important DESC, ID DESC");
            JArray a = new JArray();
            for (int i = 0; i < dt.Rows.Count; i++)
                if (Visible(Convert.ToInt32(dt.Rows[i]["group"]), PermissionType.SelfGroupOnly))
                {
                    JObject o = new JObject
                    {
                        ["ID"] = dt.Rows[i]["ID"].ToString(),
                        ["title"] = dt.Rows[i]["title"].ToString(),
                        ["user"] = dt.Rows[i]["realname"].ToString(),
                        ["content"] = dt.Rows[i]["content"].ToString(),
                        ["notifyTime"] =
                            Convert.ToDateTime(dt.Rows[i]["notifyTime"].ToString()).ToString("yyyy-MM-dd HH:mm"),
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