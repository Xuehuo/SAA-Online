using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SAAO
{
    /// <summary>
    /// Mail 邮件
    /// </summary>
    public class Mail
    {
        public string Sender { get; private set; }
        public string Recipient { get; private set; }
        public string Subject { get; private set; }
        public string BodyPlain { get; private set; }
        public DateTime Datetime { get; private set; }
        public JArray Attachment { get; private set; }

        public Mail(Guid guid)
        {
            var id = guid.ToString().ToUpper();
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@id", SqlIntegrate.DataType.VarChar, id);
            var dr = si.Reader("SELECT * FROM [Mail] WHERE [GUID]=@id");
            Sender = dr["sender"].ToString();
            Recipient = dr["recipient"].ToString();
            Subject = dr["subject"].ToString();
            BodyPlain = dr["body-plain"].ToString();
            Datetime = Convert.ToDateTime(dr["datetime"]);
            Attachment = JArray.Parse(dr["attachment-json"].ToString());
        }

        public static JArray ListJson()
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@mail", SqlIntegrate.DataType.VarChar, "%" + User.Current.Username + "%");
            return si.AdapterJson("SELECT [GUID], [sender], [subject], [datetime], [attachment-count] AS [attachcount] FROM [Mail] WHERE [recipient] LIKE @mail ORDER BY [ID] DESC");
        }

        public static void DownloadAttachment(Guid id, string displayName)
        {
            var storagePath = System.Configuration.ConfigurationManager.AppSettings["mailStoragePath"] + @"\Attachment\";
            Utility.Download(storagePath + id.ToString().ToUpper() + ".bin", Utility.Base64Decode(displayName));
        }
    }
}