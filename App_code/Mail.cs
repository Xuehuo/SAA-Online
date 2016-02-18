using System;
using System.Data;
namespace SAAO
{
    /// <summary>
    /// Mail 邮件
    /// </summary>
    public class Mail
    {
        /// <summary>
        /// Hmailserver database connection string
        /// </summary>
        public static string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SAAMConnectionString"].ToString();
        /// <summary>
        /// SMTP server (localhost most possibly)
        /// </summary>
        public static string serverAddress = System.Configuration.ConfigurationManager.AppSettings["mailServerAddress"];
        /// <summary>
        /// Mail domain ("xuehuo.org" for example)
        /// </summary>
        public static string mailDomain = System.Configuration.ConfigurationManager.AppSettings["mailDomainName"];
        /// <summary>
        /// Hmailserver local data storage path
        /// </summary>
        private string mailPath = System.Configuration.ConfigurationManager.AppSettings["mailStoragePath"] + mailDomain + @"\";
        /// <summary>
        /// Mail index in database
        /// </summary>
        private int mailID;
        /// <summary>
        /// Path to eml file of the mail
        /// </summary>
        private string emlPath;
        /// <summary>
        /// CDO Message (read from eml file)
        /// </summary>
        private CDO.Message message;
        /// <summary>
        /// Username of the user the mail belongs to
        /// </summary>
        public string username;
        public string subject;
        public mailAddress from;
        public mailAddress[] to;
        public DateTime sentOn;
        public int attachmentCount;

        /// <summary>
        /// Meaning of IMAP flag (refer to Hmailserver open source project)
        /// </summary>
        public enum mailFlag
        {
            Seen = 1,
            Deleted = 2,
            Flagged = 4,
            Answered = 8,
            Draft = 16,
            Recent = 32,
            VirusScan = 64
        };

        /// <summary>
        /// IMAP flag
        /// </summary>
        public mailFlag flag;
        /// <summary>
        /// Folder ID in database (distinct for each user)
        /// </summary>
        private int folderid;
        /// <summary>
        /// Mail address structure
        /// </summary>
        public struct mailAddress
        {
            /// <summary>
            /// Displayed name
            /// </summary>
            public string name;
            /// <summary>
            /// Email address
            /// </summary>
            public string mail;
            /// <summary>
            /// mailAddress constructor
            /// </summary>
            /// <param name="name">Displayed name</param>
            /// <param name="mail">Email address</param>
            public mailAddress(string name, string mail)
            {
                // Remove quotation marks
                this.name = name.Replace("\"","");
                this.mail = mail.Replace("\"","");
            }
        }
        /// <summary>
        /// Mail constructor
        /// </summary>
        /// <param name="mailID">Mail ID in database</param>
        public Mail(int mailID)
        {
            this.mailID = mailID;
            SqlIntegrate si = new SqlIntegrate(connStr);
            DataRow mailInfo = si.Reader("SELECT * FROM hm_messages WHERE messageid = " + mailID);
            username = si.Query("DECLARE @uid int; SELECT @uid = messageaccountid FROM hm_messages WHERE messageid = " + mailID + ";"
                + " SELECT accountaddress FROM hm_accounts WHERE accountid = @uid;").ToString()
                .Replace("@" + mailDomain, "");
            /* The eml file is storage in this way:
             *
             * When hmailserver receives an email, it writes information in database and stores the eml file.
             * 
             * [A] = Hmailserver data path (need to set mailStoragePath in Web.config)
             * 
             * [B] = Domain name (SAAO.Mail.mailDomain, need to set mailDomainName in Web.config)
             * 
             * [C] = Username (without domain name, "szhang140" for example)
             * 
             * [D] = Initial two characters (contain no open brace '{') of eml file name (store in [messagefilename] in database)
             * 
             * [E] = Eml file name
             * 
             * [F] = [A] \ [B] \ [C] \ [D] \ [E] ("D:\Mail_Data\xuehuo.org\szhang140\1B\{1B961C98-4FC8-40E3-BC8E-FA5A1D374381}.eml" for example)
             * 
             * [F] is the absolute path to eml file.
             */
            emlPath = mailPath + username + @"\" + mailInfo["messagefilename"].ToString().Substring(1, 2) + @"\" + mailInfo["messagefilename"];
            message = ReadEML(emlPath);
            flag = (mailFlag)Convert.ToInt32(mailInfo["messageflags"]);
            subject = message.Subject;
            // Remove '<' and '>'
            from = new mailAddress(message.From.Split('<')[0], message.From.Split('<')[1].Replace(">", ""));
            int toCount = message.To.Trim().Split(',').Length;
            to = new mailAddress[toCount];
            for (int i = 0; i < toCount; i++)
                to[i] = new mailAddress(message.To.Trim().Split(',')[i].Split('<')[0], message.To.Trim().Split(',')[i].Split('<')[1].Replace(">",""));
            sentOn = message.SentOn;
            attachmentCount = message.Attachments.Count;
        }
        /// <summary>
        /// Obtain mail body (if not HTML, convert it to HTML)
        /// </summary>
        /// <returns>Mail body (HTML)</returns>
        public string Body()
        {
            if (message.HTMLBody != "")
                return message.HTMLBody;
            else
                // Plain text mail body
                return "<p>" + message.TextBody.Replace("\n\r", "<br>").Replace("\n", "<br>") + "</p>";
                // Both unix-style and Windows-style returns
        }
        /// <summary>
        /// Obtain mail body preview (default length 80)
        /// </summary>
        /// <returns>Mail body preview</returns>
        public string Thumb(int length = 80)
        {
            string mailbody;
            if (message.HTMLBody != "")
                mailbody = FilterHtml(message.HTMLBody);
            else
                mailbody = message.TextBody;
            if (mailbody.Length > length)
                mailbody = mailbody.Substring(0, length);
            return mailbody.Replace("\"", "").Replace("\n", "").Replace("\r", "");
        }
        /// <summary>
        /// Get attachment name
        /// </summary>
        /// <param name="index">Attachment index (from 1)</param>
        /// <returns>Attachment name</returns>
        public string GetAttachmentName(int index)
        {
            if (index <= message.Attachments.Count && index != 0)
                return message.Attachments[index].FileName;
            else
                return null;
        }
        /// <summary>
        /// Get attachment storage path
        /// </summary>
        /// <param name="index">Attachment index (from 1)</param>
        /// <returns>Attachment storage path</returns>
        public string GetAttachmentPath(int index)
        {
            if (index <= message.Attachments.Count && index != 0)
                return emlPath.Replace(".eml", "") + "_" + index + ".attach";
            else
                return null;
        }
        /// <summary>
        /// Obtain attachment information in JSON
        /// </summary>
        /// <returns>Attachment information in JSON</returns>
        public string AttachmentJSON()
        {
            string rt = "";
            if (attachmentCount != 0)
            {
                for (int i = 1; i <= attachmentCount; i++)
                {
                    /* For eml path "[PATH TO EML FILE]\{[GUID]}.eml"
                     * its attachment was save in "[PATH TO EML FILE]\{[GUID]}_[INDEX].attach"
                     * [INDEX] was count from 1
                     */
                    if (!System.IO.File.Exists(emlPath.Replace(".eml", "") + "_" + i + ".attach"))
                        message.Attachments[i].SaveToFile(emlPath.Replace(".eml", "") + "_" + i + ".attach");
                    rt += "{\"filename\":\"" + Utility.string2JSON(message.Attachments[i].FileName) + "\"}";
                    if (i != attachmentCount)
                        rt += ",";
                }
            }
            return rt;
        }
        /// <summary>
        /// Move the mail to another folder
        /// </summary>
        /// <param name="newfolderid">Target folder ID</param>
        public void MoveTo(int newfolderid)
        {
            SqlIntegrate si = new SqlIntegrate(connStr);
            si.Execute("UPDATE hm_messages SET messagefolderid = " + newfolderid + " WHERE messageid = " + mailID);
            folderid = newfolderid;
        }
        /// <summary>
        /// Set a new flag of the mail
        /// </summary>
        /// <param name="newflag">New flag</param>
        public void SetFlag(mailFlag newflag)
        {
            SqlIntegrate si = new SqlIntegrate(connStr);
            si.Execute("UPDATE hm_messages SET messageflags = " + (int)newflag + " WHERE messageid = " + mailID);
            flag = newflag;
        }
        /// <summary>
        /// Convert the mail information to JSON
        /// </summary>
        /// <returns>Mail information in JSON. {id,subject,from:{name,mail},to:[{name,mail},...],flag,time,attachcount,attachment:[ATTACHMENT JSON]}</returns>
        public string ToJSON()
        {
            string toString = "";
            for (int i = 0; i < to.Length; i++)
                if (i != to.Length - 1)
                    toString += "{\"name\":\"" + Utility.string2JSON(to[i].name) + "\",\"mail\":\"" + Utility.string2JSON(to[i].mail) + "\"},";
                else
                    toString += "{\"name\":\"" + Utility.string2JSON(to[i].name) + "\",\"mail\":\"" + Utility.string2JSON(to[i].mail) + "\"}";
            return "{\"id\":" + mailID + ",\"subject\":\"" + Utility.string2JSON(subject) + "\",\"from\":{\"name\":\"" + Utility.string2JSON(from.name) + "\",\"mail\":\"" + Utility.string2JSON(from.mail) + "\"},\"to\":[" + toString + "],\"flag\":" + (int)flag + ",\"time\":\"" + sentOn.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"attachcount\": " + attachmentCount + ",\"attachment\":[" + AttachmentJSON() + "]}";
        }
        /// <summary>
        /// Filter all HTML tags
        /// </summary>
        /// <param name="strhtml">HTML string</param>
        /// <returns>string without HTML tags</returns>
        private string FilterHtml(string strhtml)
        {
            string stroutput = strhtml;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<[^>]+>|</[^>]+>");
            stroutput = regex.Replace(stroutput, "");
            return stroutput;
        }
        /// <summary>
        /// Read eml file into CDO message
        /// </summary>
        /// <param name="filepath">Path to eml file</param>
        /// <returns>CDO message</returns>
        private CDO.Message ReadEML(string filepath)
        {
            CDO.Message oMsg = new CDO.Message();
            ADODB.Stream stm = null;
            stm = new ADODB.Stream();
            stm.Open(System.Reflection.Missing.Value,
            ADODB.ConnectModeEnum.adModeUnknown,
            ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified, "", "");
            stm.Type = ADODB.StreamTypeEnum.adTypeBinary;
            stm.LoadFromFile(filepath);
            oMsg.DataSource.OpenObject(stm, "_stream");
            stm.Close();
            return oMsg;
        }

        /// <summary>
        /// List mail(s) of a folder in the database in JSON
        /// </summary>
        /// <param name="folder">Folder name</param>
        /// <returns>JSON of mail(s) of the folder [{id,subject,from,thumb,flag,time,attachcount},...]</returns>
        public static string ListJSON(string folder)
        {
            string data;
            SqlIntegrate si = new SqlIntegrate(connStr);
            int uid = Convert.ToInt32(si.Query("SELECT accountid FROM hm_accounts WHERE accountaddress = '" + User.Current.username + "@xuehuo.org'"));
            int folderid = Convert.ToInt32(si.Query("SELECT folderid FROM hm_imapfolders WHERE " + (folder == "Sent" ? "(foldername = 'Sent' OR foldername = 'Sent Items' OR foldername = 'Sent Messages')" : "foldername = '" + folder + "'") + " AND folderaccountid = " + uid));
            DataTable list = si.Adapter("SELECT messageid FROM hm_messages WHERE messagefolderid = " + folderid + " AND messageaccountid = " + uid + " ORDER BY messageid DESC");
            data = "[";
            for (int i = 0; i < list.Rows.Count; i++)
            {
                Mail message = new Mail(Convert.ToInt32(list.Rows[i]["messageid"]));
                data += "{\"id\":" + list.Rows[i]["messageid"].ToString() + ",\"subject\":\"" + message.subject + "\",\"from\":\"" + message.from.name.Replace("\"","") + "\",\"thumb\":\"" + message.Thumb() + "\",\"flag\":" + (int)message.flag + ",\"time\":\"" + message.sentOn + "\",\"attachcount\": " + message.attachmentCount + "}";
                if (i != list.Rows.Count - 1)
                    data += ",";
            }
            data += "]";
            return data;
        }
    }
}