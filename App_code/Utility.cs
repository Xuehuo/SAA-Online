using System;
using System.Text;
using System.Security.Cryptography;
using System.Web;
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
        public static string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SAAOConnectionString"].ToString();
        /// <summary>
        /// Write message (error one most possibly) to database
        /// </summary>
        /// <param name="message">Message (string)</param>
        public static void Log(string message)
        {
            SqlIntegrate si = new SqlIntegrate(connStr);
            si.InitParameter(5);
            si.AddParameter("@context", SqlIntegrate.DataType.Text, message);
            si.AddParameter("@IP", SqlIntegrate.DataType.VarChar, HttpContext.Current.Request.UserHostAddress, 50);
            si.AddParameter("@browser", SqlIntegrate.DataType.Text, HttpContext.Current.Request.UserAgent);
            si.AddParameter("@OS", SqlIntegrate.DataType.VarChar, HttpContext.Current.Request.Browser.Platform, 50);
            si.AddParameter("@session", SqlIntegrate.DataType.VarChar, User.IsLogin ? User.Current.username : "未登录用户", 50);
            si.Execute("INSERT INTO Log ([context], [IP], [browser], [OS], [session]) VALUES (@context, @IP, @browser, @OS, @session)");
        }
        /// <summary>
        /// Write message (error one most possibly) to database
        /// </summary>
        /// <param name="message">Exception</param>
        public static void Log(Exception message)
        {
            SqlIntegrate si = new SqlIntegrate(connStr);
            si.InitParameter(5);
            si.AddParameter("@context", SqlIntegrate.DataType.Text, message.Message + message.StackTrace);
            si.AddParameter("@IP", SqlIntegrate.DataType.VarChar, HttpContext.Current.Request.UserHostAddress, 50);
            si.AddParameter("@browser", SqlIntegrate.DataType.Text, HttpContext.Current.Request.UserAgent);
            si.AddParameter("@OS", SqlIntegrate.DataType.VarChar, HttpContext.Current.Request.Browser.Platform, 50);
            si.AddParameter("@session", SqlIntegrate.DataType.VarChar, User.IsLogin ? User.Current.username : "未登录用户", 50);
            si.Execute("INSERT INTO Log ([context], [IP], [browser], [OS], [session]) VALUES (@context, @IP, @browser, @OS, @session)");
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
        /// Replace with escape character in a string to join in a JSON
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>String without escape character</returns>
        public static string string2JSON(string str)
        {
            str = str.Replace(">", "&gt;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(" ", "");
            str = str.Replace("	", "");
            str = str.Replace("\"", "&quot;");
            str = str.Replace("\'", "&#39;");
            str = str.Replace("\\", "\\\\");
            str = str.Replace("\n", "\\n");
            str = str.Replace("\r", "\\r");
            return str;
        }
    }
}