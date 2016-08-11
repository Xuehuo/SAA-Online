using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json.Linq;

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
        private readonly string _extension;
        /// <summary>
        /// File size in byte
        /// </summary>
        private readonly int _size;
        private readonly User _uploader;
        private DateTime _uploadTime;
        private int _downloadCount;
        private readonly string _savePath;
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
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@GUID", SqlIntegrate.DataType.VarChar, str.ToUpper());
            var fileInfo = si.Reader("SELECT * FROM [File] WHERE [GUID] = @GUID");
            _name = fileInfo["name"].ToString();
            _info = fileInfo["info"].ToString();
            _extension = fileInfo["extension"].ToString();
            _size = Convert.ToInt32(fileInfo["size"]);
            _uploader = new User(Guid.Parse(fileInfo["uploader"].ToString()));
            _downloadCount = Convert.ToInt32(fileInfo["downloadCount"]);
            _uploadTime = Convert.ToDateTime(fileInfo["uploadTime"]);
            _savePath = StoragePath + str.ToUpper();
            _permission = (PermissionLevel)Convert.ToInt32(fileInfo["permission"]);
            Tag = new List<string>();
            si.ResetParameter();
            si.AddParameter("@FUID", SqlIntegrate.DataType.VarChar, str.ToUpper());
            var tagList = si.Adapter("SELECT [name] FROM [Filetag] WHERE FUID = @FUID");
            for (var i = 0; i < tagList.Rows.Count; i++)
                Tag.Add(tagList.Rows[i]["name"].ToString());
        }

        public static void Upload(System.Web.HttpPostedFile file)
        {
            string guid = Guid.NewGuid().ToString().ToUpper();
            file.SaveAs(StoragePath + guid);
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@name", SqlIntegrate.DataType.VarChar,
                System.IO.Path.GetFileNameWithoutExtension(file.FileName), 50);
            si.AddParameter("@extension", SqlIntegrate.DataType.VarChar,
                System.IO.Path.GetExtension(file.FileName).TrimStart('.').ToLower(), 10);
            si.Execute($"INSERT INTO [File] ([GUID],[name],[extension],[size],[uploader]) VALUES ('{guid}',@name,@extension,{file.ContentLength},'{User.Current.UUID}')");
        }
        /// <summary>
        /// Check whether the file has a tag
        /// </summary>
        /// <param name="str">Tag string</param>
        /// <returns>whether the file has this tag</returns>
        public bool HasTag(string str)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
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
            if (!HasTag(str)) return;
            Tag.Remove(str);
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, str, 50);
            si.Execute($"DELETE FROM [Filetag] WHERE [name] = @name AND [FUID] = '{_guid}')");
        }
        /// <summary>
        /// Add a tag to the file
        /// </summary>
        /// <param name="str">Tag string</param>
        public void AddTag(string str)
        {
            Tag.Add(str);
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, str, 50);
            si.Execute($"INSERT INTO Filetag ([name], [FUID]) VALUES (@name, '{_guid}')");
        }
        /// <summary>
        /// Download the file (Write stream to current http response)
        /// </summary>
        public void Download()
        {
            _downloadCount++;
            new SqlIntegrate(Utility.ConnStr).Execute($"UPDATE [File] SET [downloadCount] = [downloadCount] + 1 WHERE [GUID] = '{_guid}'");
            string fileName = Name + "." + _extension;
            Utility.Download(_savePath, fileName);
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
            System.IO.File.Delete(_savePath);
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
        public JObject ToJson()
        {
            JObject o = new JObject
            {
                ["guid"] = _guid,
                ["permission"] = (int) _permission,
                ["name"] = _name,
                ["extension"] = _extension,
                ["uploadTime"] = _uploadTime.ToString("yyyy-MM-dd HH:mm"),
                ["size"] = _size,
                ["uploader"] = _uploader.Realname,
                ["group"] = _uploader.GroupName,
                ["downloadCount"] = _downloadCount,
                ["tag"] = string.Join(",", Tag),
                ["info"] = _info ?? ""
            };
            return o;
        }
        /// <summary>
        /// List current files in the database in JSON
        /// </summary>
        /// <returns>JSON of current files [{guid,name,extension,uploaderName,datetime,info(bool)},...]</returns>
        public static JArray ListJson()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            DataTable dt = si.Adapter("SELECT TOP 50 * FROM [File] ORDER BY [ID] DESC");
            JArray a = new JArray();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                
                User uploader = new User(Guid.Parse(dt.Rows[i]["uploader"].ToString()));
                if (Visible((PermissionLevel)Convert.ToInt32(dt.Rows[i]["permission"].ToString()),uploader,User.Current))
                {
                    JObject o = new JObject
                    {
                        ["guid"] = dt.Rows[i]["GUID"].ToString(),
                        ["name"] = dt.Rows[i]["name"].ToString(),
                        ["extension"] = dt.Rows[i]["extension"].ToString(),
                        ["downloadCount"] = int.Parse(dt.Rows[i]["downloadCount"].ToString()),
                        ["uploaderName"] = uploader.Realname,
                        ["datetime"] = DateTime.Parse(dt.Rows[i]["uploadTime"].ToString()).ToString("yyyy-MM-dd HH:mm"),
                        ["info"] = dt.Rows[i]["info"].ToString() != ""
                    };
                    a.Add(o);
                }
            }
            return a;
        }
    }
}