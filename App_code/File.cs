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
        public static string StoragePath = System.Configuration.ConfigurationManager.AppSettings["fileStoragePath"] + @"storage\";
        private readonly string _guid;
        private string _name;
        /// <summary>
        /// File description
        /// </summary>
        private string _info;
        /// <summary>
        /// File extension (doc, pdf, etc.)
        /// </summary>
        public string Extension;
        /// <summary>
        /// File size in byte
        /// </summary>
        public int Size;
        private readonly User _uploader;
        private DateTime _uploadTime;
        private int _downloadCount;
        public string SavePath;
        public List<string> Tag;
        private PermissionLevel _permission;
        public enum PermissionLevel
        {
            All = 0,
            /// <summary>
            /// Only visible to group of oneself
            /// </summary>
            SelfGroupOnly = 1,
            /// <summary>
            /// Only visible to Senior Two
            /// </summary>
            SeniorTwoOnly = 2,
            /// <summary>
            /// Only visible to important members (administrative member). IMPT_MEMB is defined in Organization.cs
            /// </summary>
            ImptMembOnly = 3
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
            _guid = str.ToUpper();
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            var fileInfo = si.Reader($"SELECT * FROM [File] WHERE [GUID] = '{str.ToUpper()}'");
            _name = fileInfo["name"].ToString();
            _info = fileInfo["info"].ToString();
            Extension = fileInfo["extension"].ToString();
            Size = Convert.ToInt32(fileInfo["size"]);
            _uploader = new User(Guid.Parse(fileInfo["uploader"].ToString()));
            _downloadCount = Convert.ToInt32(fileInfo["downloadCount"]);
            _uploadTime = Convert.ToDateTime(fileInfo["uploadTime"]);
            SavePath = StoragePath + str.ToUpper();
            _permission = (PermissionLevel)Convert.ToInt32(fileInfo["permission"]);
            Tag = new List<string>();
            DataTable tagList = si.Adapter($"SELECT [name] FROM [Filetag] WHERE FUID = '{str.ToUpper()}'");
            for (int i = 0; i < tagList.Rows.Count; i++)
                Tag.Add(tagList.Rows[i]["name"].ToString());
        }
        /// <summary>
        /// Check whether the file has a tag
        /// </summary>
        /// <param name="str">Tag string</param>
        /// <returns>whether the file has this tag</returns>
        public bool HasTag(string str)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.InitParameter(1);
            si.AddParameter("@name", SqlIntegrate.DataType.VarChar, str, 50);
            int count = Convert.ToInt32(si.Query(
                $"SELECT COUNT(*) FROM [Filetag] WHERE [name] = @name AND [FUID] = '{_guid}'"));
            if (count != 0)
                return true;
            return false;
        }
        /// <summary>
        /// Remove a tag of the file (if existed)
        /// </summary>
        /// <param name="str">Tag string</param>
        public void RemoveTag(string str)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.InitParameter(1);
            si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, str, 50);
            si.Execute($"DELETE FROM [Filetag] WHERE [name] = @name AND [FUID] = '{_guid}')");
        }
        /// <summary>
        /// Add a tag to the file
        /// </summary>
        /// <param name="str">Tag string</param>
        public void AddTag(string str)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.InitParameter(1);
            si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, str, 50);
            si.Execute($"INSERT INTO Filetag ([name], [FUID]) VALUES (@name, '{_guid}')");
        }
        /// <summary>
        /// Filename
        /// </summary>
        public string Name
        {
            set
            {
                _name = value;
                SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
                si.InitParameter(1);
                si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, value, 50);
                si.Execute($"UPDATE [File] SET [name] = @name WHERE [GUID] = '{_guid}'");
            }
            get
            {
                return _name;
            }
        }
        /// <summary>
        /// File description
        /// </summary>
        public string Info
        {
            set
            {
                _info = value;
                SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
                si.InitParameter(1);
                si.AddParameter("@info", SqlIntegrate.DataType.Text, value);
                si.Execute($"UPDATE [File] SET [info] = @info WHERE [GUID] = '{_guid}'");
            }
            get
            {
                return _info;
            }
        }
        /// <summary>
        /// Delete the file
        /// </summary>
        public void Delete()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.Execute($"DELETE FROM [File] WHERE [GUID] = '{_guid}'");
            si.Execute($"DELETE FROM [Filetag] WHERE [FUID] = '{_guid}'");
            System.IO.File.Delete(SavePath);
        }
        /// <summary>
        /// File visibility-level
        /// </summary>
        public PermissionLevel Permission
        {
            get
            {
                return _permission;
            }
            set
            {
                _permission = value;
                SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
                si.Execute($"UPDATE [File] SET [permission] = {(int) value} WHERE [GUID] = '{_guid}'");
            }
        }
        /// <summary>
        /// Check whether a user has the permission to the file
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>whether a user has the permission to the file</returns>
        public bool Visible(User user)
        {
            return Visible(_permission, _uploader, user);
        }
        /// <summary>
        /// Check whether a user has the permission to a file (static function)
        /// </summary>
        /// <param name="permission">Permission setting</param>
        /// <param name="uploader">Uploader of the file</param>
        /// <param name="user">User (current one most possibly)</param>
        /// <returns>whether a user has the permission to a file</returns>
        public static bool Visible(PermissionLevel permission, User uploader, User user)
        {
            if (uploader.UUID == user.UUID)
                return true;
            switch (permission)
            {
                case PermissionLevel.All:
                    return true;
                case PermissionLevel.SelfGroupOnly:
                    if (uploader.Group == user.Group)
                        return true;
                    break;
                case PermissionLevel.SeniorTwoOnly:
                    if (user.Senior == 2)
                        return true;
                    break;
                case PermissionLevel.ImptMembOnly:
                    if (user.IsExecutive)
                        return true;
                    break;
            }
            return false;
        }
        /// <summary>
        /// Convert the file information to JSON
        /// </summary>
        /// <returns>File information in JSON. {guid,permission,name,extension,uploadTime,size,uploader,group,downloadCount,tag(string),info(string)}</returns>
        public string ToJson()
        {
            string data = "{";
            data += "\"guid\":\"" + _guid + "\",";
            data += "\"permission\":" + (int)_permission + ",";
            data += "\"name\":\"" + Utility.String2Json(_name) + "\",";
            data += "\"extension\":\"" + Extension + "\",";
            data += "\"uploadTime\":\"" + _uploadTime.ToString("yyyy-MM-dd HH:mm") + "\",";
            data += "\"size\":" + Size + ",";
            data += "\"uploader\":\"" + Utility.String2Json(_uploader.Realname) + "\",";
            data += "\"group\":\"" + Utility.String2Json(_uploader.GroupName) + "\",";
            data += "\"downloadCount\":" + _downloadCount + ",";
            data += "\"tag\":\"" + Utility.String2Json(string.Join(",", Tag)) + "\",";
            data += "\"info\":\"" + (_info == null ? "" : Utility.String2Json(_info)) + "\"";
            data += "}";
            return data;
        }
        /// <summary>
        /// List current files in the database in JSON
        /// </summary>
        /// <returns>JSON of current files [{guid,name,extension,uploaderName,datetime,info(bool)},...]</returns>
        public static string ListJson()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            DataTable dt = si.Adapter("SELECT * FROM [File] ORDER BY [ID] DESC");
            string data = "[";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                User uploader = new User(Guid.Parse(dt.Rows[i]["uploader"].ToString()));
                if (Visible((PermissionLevel)Convert.ToInt32(dt.Rows[i]["permission"].ToString()),uploader,User.Current))
                {
                    data += "{";
                    data += "\"guid\":\"" + dt.Rows[i]["GUID"] + "\",";
                    data += "\"name\":\"" + Utility.String2Json(dt.Rows[i]["name"].ToString()) + "\",";
                    data += "\"extension\":\"" + dt.Rows[i]["extension"] + "\",";
                    data += "\"downloadCount\":" + dt.Rows[i]["downloadCount"] + ",";
                    data += "\"uploaderName\":\"" + Utility.String2Json(uploader.Realname) + "\",";
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