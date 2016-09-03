using System;
using System.Collections.Generic;
using System.Net.Http;
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
        private string _mediaId;
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
            _mediaId = fileInfo["media_id"].ToString();
            Tag = new List<string>();
            si.ResetParameter();
            si.AddParameter("@FUID", SqlIntegrate.DataType.VarChar, str.ToUpper());
            var tagList = si.Adapter("SELECT [name] FROM [Filetag] WHERE FUID = @FUID");
            for (var i = 0; i < tagList.Rows.Count; i++)
                Tag.Add(tagList.Rows[i]["name"].ToString());
        }

        public static void Upload(System.Web.HttpPostedFile file)
        {
            var guid = Guid.NewGuid().ToString().ToUpper();
            file.SaveAs(StoragePath + guid);
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@GUID", SqlIntegrate.DataType.VarChar, guid);
            si.AddParameter("@name", SqlIntegrate.DataType.VarChar,
                System.IO.Path.GetFileNameWithoutExtension(file.FileName), 50);
            si.AddParameter("@extension", SqlIntegrate.DataType.VarChar,
                System.IO.Path.GetExtension(file.FileName).TrimStart('.').ToLower(), 10);
            si.AddParameter("@size", SqlIntegrate.DataType.Int, file.ContentLength);
            si.AddParameter("@UUID", SqlIntegrate.DataType.VarChar, User.Current.UUID);
            si.Execute("INSERT INTO [File] ([GUID],[name],[extension],[size],[uploader]) VALUES (@GUID,@name,@extension,@size,@UUID)");
            if(file.ContentLength < 20000000)
                UploadWechat(file, guid);
        }
        /// <summary>
        /// Check whether the file has a tag
        /// </summary>
        /// <param name="str">Tag string</param>
        /// <returns>whether the file has this tag</returns>
        public bool HasTag(string str)
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@name", SqlIntegrate.DataType.VarChar, str, 50);
            si.AddParameter("@FUID", SqlIntegrate.DataType.VarChar, _guid);
            var count = Convert.ToInt32(si.Query(
                "SELECT COUNT(*) FROM [Filetag] WHERE [name] = @name AND [FUID] = @FUID"));
            return count != 0;
        }
        /// <summary>
        /// Remove a tag of the file (if existed)
        /// </summary>
        /// <param name="str">Tag string</param>
        public void RemoveTag(string str)
        {
            if (!HasTag(str)) return;
            Tag.Remove(str);
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, str, 50);
            si.AddParameter("@FUID", SqlIntegrate.DataType.VarChar, _guid);
            si.Execute("DELETE FROM [Filetag] WHERE [name] = @name AND [FUID] = @FUID");
        }
        /// <summary>
        /// Add a tag to the file
        /// </summary>
        /// <param name="str">Tag string</param>
        public void AddTag(string str)
        {
            Tag.Add(str);
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, str, 50);
            si.AddParameter("@FUID", SqlIntegrate.DataType.VarChar, _guid);
            si.Execute("INSERT INTO Filetag ([name], [FUID]) VALUES (@name, @FUID)");
        }
        /// <summary>
        /// Download the file (Write stream to current http response)
        /// </summary>
        public void Download()
        {
            _downloadCount++;
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@GUID", SqlIntegrate.DataType.VarChar, _guid);
            si.Execute("UPDATE [File] SET [downloadCount] = [downloadCount] + 1 WHERE [GUID] = @GUID");
            Utility.Download(_savePath, _name + "." + _extension);
        }
        /// <summary>
        /// Filename
        /// </summary>
        public string Name
        {
            set
            {
                _name = value;
                var si = new SqlIntegrate(Utility.ConnStr);
                si.AddParameter("@name", SqlIntegrate.DataType.NVarChar, value, 50);
                si.AddParameter("@GUID", SqlIntegrate.DataType.VarChar, _guid);
                si.Execute("UPDATE [File] SET [name] = @name WHERE [GUID] = @GUID");
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
                var si = new SqlIntegrate(Utility.ConnStr);
                si.AddParameter("@info", SqlIntegrate.DataType.Text, value);
                si.AddParameter("@GUID", SqlIntegrate.DataType.VarChar, _guid);
                si.Execute("UPDATE [File] SET [info] = @info WHERE [GUID] = @GUID");
            }
            get
            {
                return _info;
            }
        }
        public string MediaId
        {
            get
            {
                return _mediaId;
            }
        }
        /// <summary>
        /// Delete the file
        /// </summary>
        public void Delete()
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@GUID", SqlIntegrate.DataType.VarChar, _guid);
            si.Execute("DELETE FROM [File] WHERE [GUID] = @GUID");
            si.ResetParameter();
            si.AddParameter("@FUID", SqlIntegrate.DataType.VarChar, _guid);
            si.Execute("DELETE FROM [Filetag] WHERE [FUID] = @FUID");
            System.IO.File.Delete(_savePath);
            if(_mediaId != "")
                Utility.HttpRequestJson(string.Format("https://qyapi.weixin.qq.com/cgi-bin/material/del?access_token={0}&media_id={1}", Utility.GetAccessToken(), _mediaId));
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
                var si = new SqlIntegrate(Utility.ConnStr);
                si.AddParameter("@permission", SqlIntegrate.DataType.Int, (int) value);
                si.AddParameter("@GUID", SqlIntegrate.DataType.VarChar, _guid);
                si.Execute("UPDATE [File] SET [permission] = @permission WHERE [GUID] = @GUID");
            }
        }
        /// <summary>
        /// Check whether a user has the permission to the file
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>whether a user has the permission to the file</returns>
        public bool Visible(User user)
        {
            return Visible(_permission, _uploader.UUID, _uploader.Group, user);
        }

        /// <summary>
        /// Check whether a user has the permission to a file (static function)
        /// </summary>
        /// <param name="permission">Permission setting</param>
        /// <param name="uuid">UUID (of uploader)</param>
        /// <param name="group">Group (of uploader)</param>
        /// <param name="user">User (current one most possibly)</param>
        /// <returns>whether a user has the permission to a file</returns>
        public static bool Visible(PermissionLevel permission, string uuid, int group, User user)
        {
            if (uuid == user.UUID)
                return true;
            switch (permission)
            {
                case PermissionLevel.All:
                    return true;
                case PermissionLevel.SelfGroupOnly:
                    if (group == user.Group)
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
                default:
                    return false;
            }
            return false;
        }
        /// <summary>
        /// Convert the file information to JSON
        /// </summary>
        /// <returns>File information in JSON. {guid,permission,name,extension,uploadTime,size,uploader,group,downloadCount,tag(string),info(string)}</returns>
        public JObject ToJson()
        {
            var o = new JObject
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
        public static JArray ListJson(DateTime start, DateTime end)
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@start", SqlIntegrate.DataType.Date, start);
            si.AddParameter("@end", SqlIntegrate.DataType.Date, end);
            var dt = si.Adapter("SELECT [File].*, [User].[realname], [User].[group] FROM [File] INNER JOIN [User] ON [File].[uploader] = [User].[UUID] AND [File].[uploadTime] BETWEEN @start AND @end ORDER BY [File].[ID] DESC");
            var a = new JArray();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                if (
                    !Visible((PermissionLevel) Convert.ToInt32(dt.Rows[i]["permission"].ToString()),
                        dt.Rows[i]["uploader"].ToString(), Convert.ToInt32(dt.Rows[i]["group"]),
                        User.Current)) continue;
                var o = new JObject
                {
                    ["guid"] = dt.Rows[i]["GUID"].ToString(),
                    ["name"] = dt.Rows[i]["name"].ToString(),
                    ["extension"] = dt.Rows[i]["extension"].ToString(),
                    ["downloadCount"] = int.Parse(dt.Rows[i]["downloadCount"].ToString()),
                    ["uploaderName"] = dt.Rows[i]["realname"].ToString(),
                    ["datetime"] = DateTime.Parse(dt.Rows[i]["uploadTime"].ToString()).ToString("yyyy-MM-dd HH:mm"),
                    ["info"] = dt.Rows[i]["info"].ToString() != "",
                    ["wechat"] = (dt.Rows[i]["info"] != null)
                };
                a.Add(o);
            }
            return a;
        }

        private static void UploadWechat(System.Web.HttpPostedFile file, string guid)
        {
            var url = "https://qyapi.weixin.qq.com/cgi-bin/material/add_material?type=file&access_token=" + Utility.GetAccessToken();
            var client = new HttpClient();
            var media = new StreamContent(file.InputStream);
            var form = new MultipartFormDataContent("fbce142e-4e8e-4bf3-826d-cc3cf506cccc") { media };
            form.Headers.Remove("Content-Type");
            form.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=fbce142e-4e8e-4bf3-826d-cc3cf506cccc");
            media.Headers.Remove("Content-Disposition");
            media.Headers.Remove("Content-Type");
            media.Headers.TryAddWithoutValidation("Content-Disposition", "form-data; name=\"media\";filename=\"" + System.IO.Path.GetFileName(file.FileName) + "\"");
            media.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            try
            {
                var response = client.PostAsync(url, form).Result.Content.ReadAsStringAsync().Result;
                JObject jo = JObject.Parse(response);
                if (jo["errcode"].ToString() != "0")
                    Utility.Log(response);
                else
                {
                    string mediaid = jo["media_id"].ToString();
                    var si = new SqlIntegrate(Utility.ConnStr);
                    si.AddParameter("@media_id", SqlIntegrate.DataType.VarChar, mediaid);
                    si.AddParameter("@GUID", SqlIntegrate.DataType.VarChar, guid);
                    si.Execute("UPDATE [File] SET [media_id] = @media_id WHERE [GUID] = @GUID");
                }
            }
            catch(Exception e)
            {
                Utility.Log(e);
            }
        }
    }
}