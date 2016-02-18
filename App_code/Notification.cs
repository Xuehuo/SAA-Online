using System;
using System.Data;
using System.Net.Mail;
using System.Text;

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
        public static string storagePath = System.Configuration.ConfigurationManager.AppSettings["fileStoragePath"] + @"report\";

        public int ID;
        public permissionType type;

        private int group;

        public string title;
        public string content;

        public DateTime notifyTime;

        public enum permissionType
        {
            ALL = 0,
            SELF_GROUP_ONLY = 1,
            SUPERVISE = 2
        }
        /// <summary>
        /// Notification constructor (obtain a current one)
        /// </summary>
        /// <param name="ID">Notification ID in database</param>
        public Notification(int ID)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            DataRow dr = si.Reader("SELECT * FROM Notification WHERE ID =" + ID);
            content = dr["content"].ToString();
            title = dr["title"].ToString();
            this.ID = ID;
            type = (permissionType)int.Parse(dr["type"].ToString());
            group = -1;
            if (type == permissionType.SELF_GROUP_ONLY)
                group = new User(Guid.Parse(dr["UUID"].ToString())).group;
            notifyTime = Convert.ToDateTime(dr["notifyTime"].ToString());
        }

        /// <summary>
        /// Notification constructor (creat a new one)
        /// </summary>
        /// <param name="title">Notification title</param>
        /// <param name="content">Notification content</param>
        /// <param name="type">Notification type</param>
        public Notification(string title, string content, permissionType type)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            si.InitParameter(2);
            si.AddParameter("@content", SqlIntegrate.DataType.Text, content);
            si.AddParameter("@title", SqlIntegrate.DataType.NVarChar, title, 50);
            ID = Convert.ToInt32(si.Query("INSERT INTO Notification ([title], [content], [type], [UUID]) VALUES (@title, @content, " + (int)type + ", '" + User.Current.UUID + "'); SELECT @@IDENTITY"));
            this.type = type;
            this.title = title;
            this.content = content;
            group = -1;
            if (type == permissionType.SELF_GROUP_ONLY)
                group = User.Current.group;
            notifyTime = DateTime.Now;
        }

        /// <summary>
        /// Broardcase the notification by sending email
        /// </summary>
        public void Broardcast()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            DataTable dt = si.Adapter("SELECT mail, SN FROM [User] WHERE activated = 1" + (group != -1 ? " AND [group] =" + group : ""));

            for (int i = 0; i < dt.Rows.Count; i++)
                SendMail(dt.Rows[i]["mail"].ToString());
        }

        /// <summary>
        /// Set an important flag
        /// </summary>
        public void SetImportant()
        {
            int important = Convert.ToInt32(new SqlIntegrate(Utility.connStr).Query("SELECT MAX(important) FROM Notification"));
            new SqlIntegrate(Utility.connStr).Execute("UPDATE Notification SET important =" + (important + 1) + " WHERE ID=" + ID);
        }

        /// <summary>
        /// Attach supervising report
        /// </summary>
        /// <param name="guid">Supervising report storage GUID</param>
        public void AttachReport(string guid)
        {
            new SqlIntegrate(Utility.connStr).Execute("UPDATE Notification SET reportFile ='" + guid + "' WHERE ID=" + ID);
        }

        /// <summary>
        /// Send a notification email
        /// </summary>
        /// <param name="to">Receiver email</param>
        private void SendMail(string to)
        {
            MailMessage mail = new MailMessage("", to);
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Subject = title;
            mail.IsBodyHtml = false;
            mail.BodyEncoding = Encoding.UTF8;
            mail.Body = content;
            SmtpClient smtp = new SmtpClient(Mail.serverAddress);
            smtp.Credentials = new System.Net.NetworkCredential("", "");
            // TODO: Move this credential to setting
            smtp.Send(mail);
        }

        /// <summary>
        /// Check whether a user can see a notification (static function)
        /// </summary>
        /// <param name="group">Group of a notification</param>
        /// <param name="type">Notification permission type</param>
        /// <returns>Whether a user can see a notification</returns>
        private static bool Visible(int group, permissionType type)
        {
            if (type == permissionType.SUPERVISE)
                return true;
            if (type == permissionType.ALL)
                return true;
            if (User.Current.IsExecutive)
                return true;
            if (type == permissionType.SELF_GROUP_ONLY && User.Current.group != group)
                return false;
            return true;
        }
        /// <summary>
        /// Check whether a user can see the notification
        /// </summary>
        /// <returns>whether a user can see the notification</returns>
        public bool Visible()
        {
            return Visible(group, type);
        }

        /// <summary>
        /// List current notifications in the database in JSON
        /// </summary>
        /// <returns>JSON of current notifications [{ID,title,user,content,notifyTime,type,imprtant,(reportFile)},...]</returns>
        public static string ListJSON()
        {
            string back = "[";
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            DataTable dt = si.Adapter("SELECT Notification.reportFile, Notification.ID, Notification.important, Notification.type, Notification.title, Notification.[content], Notification.notifyTime, [User].realname, [User].[group] FROM Notification INNER JOIN [User] ON Notification.UUID = [User].UUID ORDER BY Notification.important DESC, ID DESC");
            for (int i = 0; i < dt.Rows.Count; i ++)
                if (Visible(Convert.ToInt32(dt.Rows[i]["group"]), permissionType.SELF_GROUP_ONLY))
                {
                    back += "{";
                    back += "\"ID\":"+ dt.Rows[i]["ID"] + ",";
                    back += "\"title\":\"" + Utility.string2JSON(dt.Rows[i]["title"].ToString()) + "\",";
                    back += "\"user\":\"" + Utility.string2JSON(dt.Rows[i]["realname"].ToString()) + "\",";
                    back += "\"content\":\"" + Utility.string2JSON(dt.Rows[i]["content"].ToString()) + "\",";
                    back += "\"notifyTime\":\"" + Convert.ToDateTime(dt.Rows[i]["notifyTime"].ToString()).ToString() + "\",";
                    back += "\"type\":" + dt.Rows[i]["type"] + ",";
                    back += "\"important\":" + (Convert.ToInt32(dt.Rows[i]["important"]) - 1) + ",";
                    if (Convert.ToInt32(dt.Rows[i]["type"]) == (int)permissionType.SUPERVISE)
                        back += "\"reportFile\":\"" + dt.Rows[i]["reportFile"] + "\",";
                    back += "},";
                }
            back += "]";
            return back.Replace(",]","]").Replace(",}","}");
        }
    }
}