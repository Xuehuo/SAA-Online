using System;
using System.Collections.Generic;
using System.Data;
namespace SAAO
{
    /// <summary>
    /// File 文件
    /// </summary>
    public class File
    {
        /// <summary>
        /// File storage path (with a backslash '\' in the end)
        /// </summary>
        public static string storagePath = System.Configuration.ConfigurationManager.AppSettings["fileStoragePath"] + @"storage\";
        private string guid;
        private string name;
        /// <summary>
        /// File description
        /// </summary>
        private string info;
        /// <summary>
        /// File extension (doc, pdf, etc.)
        /// </summary>
        public string extension;
        /// <summary>
        /// File size in byte
        /// </summary>
        public int size;
        private User uploader;
        private DateTime uploadTime;
        private int downloadCount;
        public string savePath;
        public List<string> tag;
        private permissionLevel permission;
        public enum permissionLevel
        {
            ALL = 0,
            /// <summary>
            /// Only visible to group of oneself
            /// </summary>
            SELF_GROUP_ONLY = 1,
            /// <summary>
            /// Only visible to Senior Two
            /// </summary>
            SENIOR_TWO_ONLY = 2,
            /// <summary>
            /// Only visible to important members (administrative member). IMPT_MEMB is defined in Organization.cs
            /// </summary>
            IMPT_MEMB_ONLY = 3
        }
        /// <summary>
        /// File constructor
        /// </summary>
        /// <param name="str">GUID string</param>
        public File(string str)
        {
            Guid guid;
            if (!Guid.TryParse(str, out guid))
                throw new ArgumentException();
            this.guid = str.ToUpper();
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            DataRow fileInfo = si.Reader("SELECT * FROM [File] WHERE [GUID] = '" + str.ToUpper() + "'");
            name = fileInfo["name"].ToString();
            info = fileInfo["info"].ToString();
            extension = fileInfo["extension"].ToString();
            size = Convert.ToInt32(fileInfo["size"]);
            uploader = new User(Guid.Parse(fileInfo["uploader"].ToString()));
            downloadCount = Convert.ToInt32(fileInfo["downloadCount"]);
            uploadTime = Convert.ToDateTime(fileInfo["uploadTime"]);
            savePath = storagePath + str.ToUpper();
            permission = (permissionLevel)Convert.ToInt32(fileInfo["permission"]);
            tag = new List<string>();
            DataTable taglist = si.Adapter("SELECT [name] FROM [Filetag] WHERE FUID = '" + str.ToUpper() +"'");
            for (int i = 0; i < taglist.Rows.Count; i++)
                tag.Add(taglist.Rows[i]["name"].ToString());
        }
        /// <summary>
        /// Check whether the file has a tag
        /// </summary>
        /// <param name="str">Tag string</param>
        /// <returns>whether the file has this tag</returns>
        public bool HasTag(string str)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            si.InitParameter(1);
            si.AddParameter("@name", SqlIntegrate.DataType.VarChar, str, 50);
            int count = Convert.ToInt32(si.Query("SELECT COUNT(*) FROM [Filetag] WHERE [name] = @name AND [FUID] = '" + guid + "'"));
            if (count != 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Remove a tag of the file (if existed)
        /// </summary>
        /// <param name="str">Tag string</param>
        public void RemoveTag(string str)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            si.InitParameter(1);
            si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, str, 50);
            si.Execute("DELETE FROM [Filetag] WHERE [name] = @name AND [FUID] = '" + guid + "')");
        }
        /// <summary>
        /// Add a tag to the file
        /// </summary>
        /// <param name="str">Tag string</param>
        public void AddTag(string str)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            si.InitParameter(1);
            si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, str, 50);
            si.Execute("INSERT INTO Filetag ([name], [FUID]) VALUES (@name, '" + guid + "')");
        }
        /// <summary>
        /// Filename
        /// </summary>
        public string Name
        {
            set
            {
                SqlIntegrate si = new SqlIntegrate(Utility.connStr);
                si.InitParameter(1);
                si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, value, 50);
                si.Execute("UPDATE [File] SET [name] = @name WHERE [GUID] = '" + guid + "'");
            }
            get
            {
                return name;
            }
        }
        /// <summary>
        /// File description
        /// </summary>
        public string Info
        {
            set
            {
                SqlIntegrate si = new SqlIntegrate(Utility.connStr);
                si.InitParameter(1);
                si.AddParameter("@info", SqlIntegrate.DataType.Text, value);
                si.Execute("UPDATE [File] SET [info] = @info WHERE [GUID] = '" + guid + "'");
            }
            get
            {
                return info;
            }
        }
        /// <summary>
        /// Delete the file
        /// </summary>
        public void Delete()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            si.Execute("DELETE FROM [File] WHERE [GUID] = '" + guid + "'");
            si.Execute("DELETE FROM [Filetag] WHERE [FUID] = '" + guid + "'");
            System.IO.File.Delete(savePath);
        }
        /// <summary>
        /// File visibility-level
        /// </summary>
        public permissionLevel Permission
        {
            get
            {
                return permission;
            }
            set
            {
                permission = value;
                SqlIntegrate si = new SqlIntegrate(Utility.connStr);
                si.Execute("UPDATE [File] SET [permission] = "+ (int)value + " WHERE [GUID] = '" + guid + "'");
            }
        }
        /// <summary>
        /// Check whether a user has the permission to the file
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>whether a user has the permission to the file</returns>
        public bool Visible(User user)
        {
            return Visible(permission, uploader, user);
        }
        /// <summary>
        /// Check whether a user has the permission to a file (static function)
        /// </summary>
        /// <param name="permisstion">Permission setting</param>
        /// <param name="uploader">Uploader of the file</param>
        /// <param name="user">User (current one most possibly)</param>
        /// <returns>whether a user has the permission to a file</returns>
        public static bool Visible(permissionLevel permisstion, User uploader, User user)
        {
            bool rt = false;
            if (uploader.UUID == user.UUID)
                return true;
            switch (permisstion)
            {
                case permissionLevel.ALL:
                    return true;
                case permissionLevel.SELF_GROUP_ONLY:
                    if (uploader.group == user.group)
                        return true;
                    break;
                case permissionLevel.SENIOR_TWO_ONLY:
                    if (user.senior == 2)
                        return true;
                    break;
                case permissionLevel.IMPT_MEMB_ONLY:
                    if (user.IsExecutive)
                        return true;
                    break;
            }
            return rt;
        }
        /// <summary>
        /// Convert the file information to JSON
        /// </summary>
        /// <returns>File information in JSON. {guid,permission,name,extension,uploadTime,size,uploader,group,downloadCount,tag(string),info(string)}</returns>
        public string ToJSON()
        {
            string data = "{";
            data += "\"guid\":\"" + guid + "\",";
            data += "\"permission\":" + (int)permission + ",";
            data += "\"name\":\"" + Utility.string2JSON(name) + "\",";
            data += "\"extension\":\"" + extension + "\",";
            data += "\"uploadTime\":\"" + uploadTime.ToString("yyyy-MM-dd HH:mm") + "\",";
            data += "\"size\":" + size + ",";
            data += "\"uploader\":\"" + Utility.string2JSON(uploader.realname) + "\",";
            data += "\"group\":\"" + Utility.string2JSON(uploader.groupName) + "\",";
            data += "\"downloadCount\":" + downloadCount + ",";
            data += "\"tag\":\"" + Utility.string2JSON(string.Join(",", tag)) + "\",";
            data += "\"info\":\"" + (info == null ? "" : Utility.string2JSON(info)) + "\"";
            data += "}";
            return data;
        }
        /// <summary>
        /// List current files in the database in JSON
        /// </summary>
        /// <returns>JSON of current files [{guid,name,extension,uploaderName,datetime,info(bool)},...]</returns>
        public static string ListJSON()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            DataTable dt = si.Adapter("SELECT * FROM [File] ORDER BY [ID] DESC");
            string data = "[";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                User uploader = new User(Guid.Parse(dt.Rows[i]["uploader"].ToString()));
                if (Visible((permissionLevel)Convert.ToInt32(dt.Rows[i]["permission"].ToString()),uploader,User.Current))
                {
                    data += "{";
                    data += "\"guid\":\"" + dt.Rows[i]["GUID"].ToString() + "\",";
                    data += "\"name\":\"" + Utility.string2JSON(dt.Rows[i]["name"].ToString()) + "\",";
                    data += "\"extension\":\"" + dt.Rows[i]["extension"].ToString() + "\",";
                    data += "\"downloadCount\":" + dt.Rows[i]["downloadCount"].ToString() + ",";
                    data += "\"uploaderName\":\"" + Utility.string2JSON(uploader.realname) + "\",";
                    data += "\"datetime\":\"" + DateTime.Parse(dt.Rows[i]["uploadTime"].ToString()).ToString("yyyy-MM-dd HH:mm") + "\",";
                    data += "\"info\":" + (dt.Rows[i]["info"].ToString() == "" ? "false" : "true") + "";
                    data += "},";
                }
            }
            data += "]";
            data = data.Replace(",]", "]");
            return data;
        }
    }
}