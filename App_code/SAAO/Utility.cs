using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SAAO
{
    /// <summary>
    /// Static functions and configurations
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Debug Flag
        /// </summary>
        private const bool isDebug =
#if DEBUG
            true;
#else
            true;
#endif

        /// <summary>
        /// Major database connection string
        /// </summary>
        public static string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["SAAOConnectionString"].ToString();
        /// <summary>
        /// Write message (error one most possibly) to database
        /// </summary>
        /// <param name="message">Message (string)</param>
        public static void Log(string message)
        {
            var si = new SqlIntegrate(ConnStr);
            si.AddParameter("@context", SqlIntegrate.DataType.Text, message);
            si.AddParameter("@url", SqlIntegrate.DataType.Text, HttpContext.Current.Request.Url.ToString());
            si.AddParameter("@IP", SqlIntegrate.DataType.Text, HttpContext.Current.Request.UserHostAddress);
            si.AddParameter("@browser", SqlIntegrate.DataType.Text, HttpContext.Current.Request.UserAgent);
            si.AddParameter("@OS", SqlIntegrate.DataType.Text, HttpContext.Current.Request.Browser.Platform);
            si.AddParameter("@session", SqlIntegrate.DataType.Text, User.IsLogin ? User.Current.Username : "未登录用户");
            si.Execute("INSERT INTO Log ([context], [url], [IP], [browser], [OS], [session]) VALUES (@context, @url, @IP, @browser, @OS, @session)");
        }
        /// <summary>
        /// Write message (error one most possibly) to database with no Http context
        /// </summary>
        /// <param name="message">Message (string)</param>
        public static void LogFailover(string message)
        {
            var si = new SqlIntegrate(ConnStr);
            si.AddParameter("@context", SqlIntegrate.DataType.Text, message);
            si.AddParameter("@url", SqlIntegrate.DataType.Text, "N/A");
            si.AddParameter("@IP", SqlIntegrate.DataType.Text, "N/A");
            si.AddParameter("@browser", SqlIntegrate.DataType.Text, "N/A");
            si.AddParameter("@OS", SqlIntegrate.DataType.Text, "N/A");
            si.AddParameter("@session", SqlIntegrate.DataType.Text, "N/A");
            si.Execute("INSERT INTO Log ([context], [url], [IP], [browser], [OS], [session]) VALUES (@context, @url, @IP, @browser, @OS, @session)");
        }
        /// <summary>
        /// Write message (error one most possibly) to database
        /// </summary>
        /// <param name="message">Exception</param>
        public static void Log(Exception message)
        {
            var si = new SqlIntegrate(ConnStr);
            si.AddParameter("@context", SqlIntegrate.DataType.Text, message.Message + message.StackTrace);
            si.AddParameter("@url", SqlIntegrate.DataType.Text, HttpContext.Current.Request.Url.ToString());
            si.AddParameter("@IP", SqlIntegrate.DataType.Text, HttpContext.Current.Request.UserHostAddress);
            si.AddParameter("@browser", SqlIntegrate.DataType.Text, HttpContext.Current.Request.UserAgent);
            si.AddParameter("@OS", SqlIntegrate.DataType.Text, HttpContext.Current.Request.Browser.Platform);
            si.AddParameter("@session", SqlIntegrate.DataType.Text, User.IsLogin ? User.Current.Username : "未登录用户");
            si.Execute("INSERT INTO Log ([context], [url], [IP], [browser], [OS], [session]) VALUES (@context, @url, @IP, @browser, @OS, @session)");
        }
        /// <summary>
        /// Use Base64 to encode a string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>String encoded in Base64</returns>
        public static string Base64Encode(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str), Base64FormattingOptions.None);
        }
        /// <summary>
        /// Decode a string encoded by Base64
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Decoded string</returns>
        public static string Base64Decode(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
        /// <summary>
        /// Major password encrypt method (SHA256)
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Password encrypted by SHA256</returns>
        public static string Encrypt(string password)
        {
            return new SHA256Managed()
                .ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password))
                .Aggregate(string.Empty, (current, bit) => current + bit.ToString("x2"));
        }
        /// <summary>
        /// Download a file (via current response)
        /// </summary>
        /// <param name="path">File absolute path</param>
        /// <param name="fileName">File name to display</param>
        /// <param name="contentType">MIME type (default: application/octet-stream)</param>
        public static void Download(string path, string fileName, string contentType = "application/octet-stream")
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = contentType;
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename*=UTF-8''" + Uri.EscapeDataString(fileName));
            // TODO: different browsers behave totally different
            HttpContext.Current.Response.TransmitFile(path);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Http Request
        /// </summary>
        /// <param name="url">Request URL</param>
        /// <param name="data">POST data</param>
        /// <param name="filePath">File path</param>
        /// <param name="fileName">File name</param>
        /// <param name="fileFieldName">File field name</param>
        /// <returns>Response string</returns>
        public static string HttpRequest(string url, object data = null, string filePath = "", string fileName = "", string fileFieldName = "")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            if (data != null && filePath == "") // Common Form POST
            {
                request.Method = "POST";
                var postData = "";
                if (data is Dictionary<string, string>)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    foreach (var p in (Dictionary<string, string>)data)
                    {
                        if (postData != "")
                            postData += "&";
                        postData += $"{p.Key}={p.Value}";
                    }
                }
                else if (data is JObject)
                {
                    request.ContentType = "application/json";
                    postData = ((JObject)data).ToString();
                }
                request.ContentLength = Encoding.UTF8.GetBytes(postData).Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(Encoding.UTF8.GetBytes(postData), 0, Encoding.UTF8.GetByteCount(postData));
            }
            else if (filePath != "" && fileName != "" && fileFieldName != "") // Multipart Form POST
            {
                request.Method = "POST";
                var boundary = $"---------------------------{Guid.NewGuid():N}";
                var boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                using (var stream = request.GetRequestStream())
                {
                    string field;
                    if ((Dictionary<string, string>)data != null)
                        // TODO: No implementation when data is JObject
                        foreach (var p in (Dictionary<string, string>)data)
                        {
                            stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                            field = $"Content-Disposition: form-data; name=\"{p.Key}\"\r\n\r\n{p.Value}";
                            stream.Write(Encoding.UTF8.GetBytes(field), 0, Encoding.UTF8.GetByteCount(field));
                        }
                    stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                    field =
                        $"Content-Disposition: form-data; name=\"{fileFieldName}\"; filename=\"{fileName}\";\r\nContent-Type: {MimeMapping.GetMimeMapping(fileName)}\r\n\r\n";
                    stream.Write(Encoding.UTF8.GetBytes(field), 0, Encoding.UTF8.GetByteCount(field));
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, fileData.Length);
                        fs.Close();
                        stream.Write(fileData, 0, fileData.Length);
                    }
                    var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                    stream.Write(trailer, 0, trailer.Length);
                    stream.Close();
                }
            }
            else // GET
                request.Method = "GET";
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
                return responseStream != null ? new StreamReader(responseStream).ReadToEnd() : null;
        }

        /// <summary>
        /// Http Get Request(Sync)
        /// </summary>
        /// <param name="url">Request Url</param>
        /// <returns>Response String</returns>
        public static string HttpRequestGet(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                string back = responseStream != null ? new StreamReader(responseStream).ReadToEnd() : null;
                if (isDebug) LogFailover($"Response:{back ?? "null"} Request:{url}");
                return back;
            }
        }

        /// <summary>
        /// Http Get Request(Async)
        /// </summary>
        /// <param name="url">Request Url</param>
        /// <returns>Async Task Object. use .Result to wait response</returns>
        public static Task<string> HttpRequestGetAsync(string url)
        {
            var task = new Task<string>(() =>
             {
                 var request = (HttpWebRequest)WebRequest.Create(url);
                 using (var response = (HttpWebResponse)request.GetResponse())
                 using (var responseStream = response.GetResponseStream())
                 {
                     string back = responseStream != null ? new StreamReader(responseStream).ReadToEnd() : null;
                     if (isDebug) LogFailover($"Response:{back ?? "null"} Request:{url}");
                     return back;
                 }
             });
            task.Start();
            return task;
        }

        /// <summary>
        /// Http Post Request(Async)
        /// </summary>
        /// <param name="url">Request Url</param>
        /// <param name="data">Post Data</param>
        /// <returns>Async Task Object. use .Result to wait response</returns>
        public static Task<string> HttpRequestPostAsync(string url, object data)
        {
            var task = new Task<string>(() =>
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                var postData = "";
                if (data is Dictionary<string, string>)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    foreach (var p in (Dictionary<string, string>)data)
                    {
                        if (postData != "")
                            postData += "&";
                        postData += $"{p.Key}={p.Value}";
                    }
                }
                else if (data is JObject)
                {
                    request.ContentType = "application/json";
                    postData = ((JObject)data).ToString();
                }
                request.ContentLength = Encoding.UTF8.GetBytes(postData).Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(Encoding.UTF8.GetBytes(postData), 0, Encoding.UTF8.GetByteCount(postData));
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                {
                    string back = responseStream != null ? new StreamReader(responseStream).ReadToEnd() : null;
                    if (isDebug) LogFailover($"Response:{back ?? "null"} Request:{url}");
                    return back;
                }
            });
            task.Start();
            return task;
        }

        /// <summary>
        /// Http Post File Request(Async)
        /// </summary>
        /// <param name="url">Request Url</param>
        /// <param name="data">Post data</param>
        /// <param name="filePath">Post File Path</param>
        /// <param name="fileName">Post File Name</param>
        /// <param name="fileFieldName">Post File Field Name</param>
        /// <returns>Async Task Object. use .Result to wait response</returns>
        public static Task<string> HttpRequestPostFileAsync(string url, object data = null, string filePath = "", string fileName = "", string fileFieldName = "")
        {
            var task = new Task<string>(() =>
             {
                 var request = (HttpWebRequest)WebRequest.Create(url);
                 request.Method = "POST";
                 var boundary = $"---------------------------{Guid.NewGuid():N}";
                 var boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                 request.ContentType = "multipart/form-data; boundary=" + boundary;
                 using (var stream = request.GetRequestStream())
                 {
                     string field;
                     if ((Dictionary<string, string>)data != null)
                         // TODO: No implementation when data is JObject
                         foreach (var p in (Dictionary<string, string>)data)
                         {
                             stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                             field = $"Content-Disposition: form-data; name=\"{p.Key}\"\r\n\r\n{p.Value}";
                             stream.Write(Encoding.UTF8.GetBytes(field), 0, Encoding.UTF8.GetByteCount(field));
                         }
                     stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                     field =
                         $"Content-Disposition: form-data; name=\"{fileFieldName}\"; filename=\"{fileName}\";\r\nContent-Type: {MimeMapping.GetMimeMapping(fileName)}\r\n\r\n";
                     stream.Write(Encoding.UTF8.GetBytes(field), 0, Encoding.UTF8.GetByteCount(field));
                     using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                     {
                         var fileData = new byte[fs.Length];
                         fs.Read(fileData, 0, fileData.Length);
                         fs.Close();
                         stream.Write(fileData, 0, fileData.Length);
                     }
                     var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                     stream.Write(trailer, 0, trailer.Length);
                     stream.Close();
                 }
                 using (var response = (HttpWebResponse)request.GetResponse())
                 using (var responseStream = response.GetResponseStream())
                 {
                     string back = responseStream != null ? new StreamReader(responseStream).ReadToEnd() : null;
                     if (isDebug) LogFailover($"Response:{back ?? "null"} Request:{url}");
                     return back;
                 }
             });
            task.Start();
            return task;
        }

        /// <summary>
        /// Get cached access token or retrieve a new one
        /// </summary>
        /// <returns>Access token</returns>
        public static string GetAccessToken()
        {
            var corpId = System.Configuration.ConfigurationManager.AppSettings["WechatCorpId"];
            var corpSecret = System.Configuration.ConfigurationManager.AppSettings["WechatCorpSecret"];
            if (HttpContext.Current.Application["accessTokenExpire"] != null &&
                DateTime.Now <= (DateTime)HttpContext.Current.Application["accessTokenExpire"])
                return HttpContext.Current.Application["accessToken"].ToString();
            var data = (JObject)new JsonSerializer()
                .Deserialize(new JsonTextReader(new StringReader(HttpRequest($"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpId}&corpsecret={corpSecret}"))));
            HttpContext.Current.Application.Add("accessTokenExpire", DateTime.Now.AddSeconds(Convert.ToInt32(data["expires_in"])));
            HttpContext.Current.Application.Add("accessToken", data["access_token"].ToString());
            return data["access_token"].ToString();
        }

        /// <summary>
        /// Send Wechat Messgae by SAAO Helper
        /// ToUser: (up to 1000 message receiver, multiple receivers' | 'Division)
        /// </summary>
        /// <param name="access_token">access_token for Task(multithreading)</param>
        /// <param name="data">structure on Wechat QyApi</param>
        public static void SendMessgaeBySAAOHelper(string access_token, JObject data)
        {
            HttpRequestPostAsync("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + access_token, data);
        }

        /// <summary>
        /// Get Unix Timestamp
        /// </summary>
        /// <param name="time">DateTime</param>
        /// <returns>timestamp</returns>
        public static int GetUnixTimeStamp(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}