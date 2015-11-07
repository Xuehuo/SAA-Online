using System;
using System.Data;
namespace SAAO
{
    /// <summary>
    /// hmail 邮件服务器邮件交互
    /// </summary>
    public class Mail
    {
        public static string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SAAMConnectionString"].ToString();
        public static string serverAddress = System.Configuration.ConfigurationManager.AppSettings["mailServerAddress"];
        private string mailPath = System.Configuration.ConfigurationManager.AppSettings["mailStoragePath"];
        private int mailID;
        private string emlPath;
        private CDO.Message message;
        public string username;
        public string subject;
        public mailAddress from;
        public mailAddress[] to;
        public DateTime sentOn;
        public int attachmentCount;
        public enum flag
        {
            Seen = 1,
            Deleted = 2,
            Flagged = 4,
            Answered = 8,
            Draft = 16,
            Recent = 32,
            VirusScan = 64
        };
        public int flagNum;
        private int folderid;
        public struct mailAddress
        {
            public string name;
            public string mail;
            public mailAddress(string name, string mail)
            {
                this.name = name.Replace("\"","");
                this.mail = mail.Replace("\"","");
            }
        }
        public Mail(int mailID)
        {
            this.mailID = mailID;
            SqlIntegrate si = new SqlIntegrate(connStr);
            DataRow mailInfo = si.Reader("SELECT * FROM hm_messages WHERE messageid = " + mailID);
            username = si.Query("DECLARE @uid int; SELECT @uid = messageaccountid FROM hm_messages WHERE messageid = " + mailID + "; SELECT accountaddress FROM hm_accounts WHERE accountid = @uid;").ToString().Replace("@xuehuo.org", "");
            emlPath = mailPath + username + @"\" + mailInfo["messagefilename"].ToString().Substring(1, 2) + @"\" + mailInfo["messagefilename"];
            message = ReadEML(emlPath);
            flagNum = Convert.ToInt32(mailInfo["messageflags"]);
            subject = message.Subject;
            from = new mailAddress(message.From.Split('<')[0], message.From.Split('<')[1].Replace(">", ""));
            int toCount = message.To.Trim().Split(',').Length;
            to = new mailAddress[toCount];
            for (int i = 0; i < toCount; i++)
                to[i] = new mailAddress(message.To.Trim().Split(',')[i].Split('<')[0], message.To.Trim().Split(',')[i].Split('<')[1].Replace(">",""));

            sentOn = Convert.ToDateTime(message.SentOn.ToString());
            attachmentCount = message.Attachments.Count;
        }
        public string Body()
        {
            if (message.HTMLBody != "")
                return message.HTMLBody;
            else
                return "<p>" + message.TextBody.Replace("\r\n", "<br/>").Replace("\n", "<br/>") + "</p>";
        }
        public string Thumb()
        {
            string mailbody;
            if (message.HTMLBody != "")
                mailbody = FilterHtml(message.HTMLBody);
            else
                mailbody = message.TextBody;
            if (mailbody.Length > 80)
                mailbody = mailbody.Substring(0, 80);
            return mailbody.Replace("\"", "").Replace("\n", "").Replace("\r", "");
        }
        public string GetAttachmentName(int index)
        {
            if (index <= message.Attachments.Count && index != 0)
                return message.Attachments[index].FileName;
            else
                return null;
        }
        public string GetAttachmentPath(int index)
        {
            if (index <= message.Attachments.Count && index != 0)
                return emlPath.Replace(".eml", "") + "_" + index + ".attach";
            else
                return null;
        }
        public string AttachmentJSON()
        {
            string rt = "";
            if (attachmentCount != 0)
            {
                for (int i = 1; i <= attachmentCount; i++)
                {
                    if (!System.IO.File.Exists(emlPath.Replace(".eml", "") + "_" + i + ".attach"))
                        message.Attachments[i].SaveToFile(emlPath.Replace(".eml", "") + "_" + i + ".attach");
                    rt += "{\"filename\":\"" + Utility.string2JSON(message.Attachments[i].FileName) + "\"}";
                    if (i != attachmentCount)
                        rt += ",";
                }
            }
            return rt;
        }
        public void MoveTo(int newfolderid)
        {
            SqlIntegrate si = new SqlIntegrate(connStr);
            si.Execute("UPDATE hm_messages SET messagefolderid = " + newfolderid + " WHERE messageid = " + mailID);
            folderid = newfolderid;
        }
        public void SetFlag(int newflag)
        {
            SqlIntegrate si = new SqlIntegrate(connStr);
            si.Execute("UPDATE hm_messages SET messageflags = " + newflag + " WHERE messageid = " + mailID);
            flagNum = newflag;
        }
        public string ToJSON()
        {
            string toString = "";
            for (int i = 0; i < to.Length; i++)
                if (i != to.Length - 1)
                    toString += "{\"name\":\"" + Utility.string2JSON(to[i].name) + "\",\"mail\":\"" + Utility.string2JSON(to[i].mail) + "\"},";
                else
                    toString += "{\"name\":\"" + Utility.string2JSON(to[i].name) + "\",\"mail\":\"" + Utility.string2JSON(to[i].mail) + "\"}";
            return "{\"id\":" + mailID + ",\"subject\":\"" + Utility.string2JSON(subject) + "\",\"from\":{\"name\":\"" + Utility.string2JSON(from.name) + "\",\"mail\":\"" + Utility.string2JSON(from.mail) + "\"},\"to\":[" + toString + "],\"flag\":" + flagNum + ",\"time\":\"" + sentOn.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"attachcount\": " + attachmentCount + ",\"attachment\":[" + AttachmentJSON() + "]}";
        }
        private string FilterHtml(string strhtml)
        {
            string stroutput = strhtml;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<[^>]+>|</[^>]+>");
            stroutput = regex.Replace(stroutput, "");
            return stroutput;
        }
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
        public static string ListJSON(string folder)
        {
            string data;
            SqlIntegrate si = new SqlIntegrate(Mail.connStr);
            int uid = Convert.ToInt32(si.Query("SELECT accountid FROM hm_accounts WHERE accountaddress = '" + User.Current.username + "@xuehuo.org'"));
            int folderid = Convert.ToInt32(si.Query("SELECT folderid FROM hm_imapfolders WHERE " + (folder == "Sent" ? "(foldername = 'Sent' OR foldername = 'Sent Items' OR foldername = 'Sent Messages')" : "foldername = '" + folder + "'") + " AND folderaccountid = " + uid));
            DataTable list = si.Adapter("SELECT messageid FROM hm_messages WHERE messagefolderid = " + folderid + " AND messageaccountid = " + uid + " ORDER BY messageid DESC");
            data = "[";
            for (int i = 0; i < list.Rows.Count; i++)
            {
                Mail message = new Mail(Convert.ToInt32(list.Rows[i]["messageid"]));
                data += "{\"id\":" + list.Rows[i]["messageid"].ToString() + ",\"subject\":\"" + message.subject + "\",\"from\":\"" + message.from.name.Replace("\"","") + "\",\"thumb\":\"" + message.Thumb() + "\",\"flag\":" + message.flagNum + ",\"time\":\"" + message.sentOn + "\",\"attachcount\": " + message.attachmentCount + "}";
                if (i != list.Rows.Count - 1)
                    data += ",";
            }
            data += "]";
            return data;
        }
    }
}