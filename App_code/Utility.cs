using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SAAO
{
    /// <summary>
    /// Static functions and configurations
    /// </summary>
    public class Utility
    {
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
            SqlIntegrate si = new SqlIntegrate(ConnStr);
            si.AddParameter("@context", SqlIntegrate.DataType.Text, message);
            si.AddParameter("@url", SqlIntegrate.DataType.Text, HttpContext.Current.Request.Url.ToString());
            si.AddParameter("@IP", SqlIntegrate.DataType.Text, HttpContext.Current.Request.UserHostAddress);
            si.AddParameter("@browser", SqlIntegrate.DataType.Text, HttpContext.Current.Request.UserAgent);
            si.AddParameter("@OS", SqlIntegrate.DataType.Text, HttpContext.Current.Request.Browser.Platform);
            si.AddParameter("@session", SqlIntegrate.DataType.Text, User.IsLogin ? User.Current.Username : "未登录用户");
            si.Execute("INSERT INTO Log ([context], [url], [IP], [browser], [OS], [session]) VALUES (@context, @url, @IP, @browser, @OS, @session)");
        }
        /// <summary>
        /// Write message (error one most possibly) to database
        /// </summary>
        /// <param name="message">Exception</param>
        public static void Log(Exception message)
        {
            SqlIntegrate si = new SqlIntegrate(ConnStr);
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
            SHA256Managed crypt = new SHA256Managed();
            string hash = string.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte bit in crypto)
                hash += bit.ToString("x2");
            return hash;
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
        /// <returns>Json object parsed from response string</returns>
        public static JObject HttpRequestJson(string url, Dictionary<string, string> data = null, string filePath = "", string fileName = "", string fileFieldName = "")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            if (data != null && filePath == "") // Common Form POST
            {
                request.Method = "POST";
                var postData = "";
                request.ContentType = "application/x-www-form-urlencoded";
                foreach (var p in data)
                {
                    if (postData != "")
                        postData += "&";
                    postData += $"{p.Key}={p.Value}";
                }
                request.ContentLength = Encoding.ASCII.GetBytes(postData).Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(Encoding.ASCII.GetBytes(postData), 0, Encoding.ASCII.GetByteCount(postData));
            }
            else if (filePath != "" && fileName != "" && fileFieldName != "") // Multipart Form POST
            {
                var boundary = $"---------------------------{Guid.NewGuid():N}";
                var boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                using (var stream = request.GetRequestStream())
                {
                    string field;
                    if (data != null)
                        foreach (var p in data)
                        {
                            stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                            field = $"Content-Disposition: form-data; name=\"{p.Key}\"\r\n\r\n{p.Value}";
                            stream.Write(Encoding.UTF8.GetBytes(field), 0, Encoding.UTF8.GetByteCount(field));
                        }
                    stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                    field =
                        $"Content-Disposition: form-data; name=\"{fileFieldName}\"; filename=\"{fileName}\";\r\nContent-Type: application/octet-stream\r\n\r\n";
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
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    return
                        (JObject)
                            new JsonSerializer().Deserialize(
                                new JsonTextReader(
                                    new StringReader(new StreamReader(response.GetResponseStream()).ReadToEnd())));
                }    
            }
            catch (Exception ex)
            {
                Log(ex);
                return null;
            }
        }
        /// <summary>
        /// Get cached access token or retrieve a new one
        /// </summary>
        /// <returns>Access token</returns>
        public static string GetAccessToken()
        {
            var corpId = System.Configuration.ConfigurationManager.AppSettings["WechatCorpId"];
            var corpSecret = System.Configuration.ConfigurationManager.AppSettings["WechatCorpSecret"];
            if (HttpContext.Current.Application["accessTokenExpire"] == null || DateTime.Now > (DateTime)HttpContext.Current.Application["accessTokenExpire"])
            {
                var data = SAAO.Utility.HttpRequestJson(
                    $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpId}&corpsecret={corpSecret}");
                HttpContext.Current.Application["accessTokenExpire"] = DateTime.Now.AddSeconds(Convert.ToInt32(data["expires_in"]));
                HttpContext.Current.Application["accessToken"] = data["access_token"].ToString();
            }
            return HttpContext.Current.Application["accessToken"].ToString();
        }
    }
}