using System;
using System.Text;
using System.Security.Cryptography;
using System.Web;
namespace SAAO
{
    /// <summary>
    /// SAAO 网站配置以及公用函数
    /// </summary>
    public class Utility
    {
        public static string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SAAOConnectionString"].ToString();
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
        public static string Base64Encode(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str), Base64FormattingOptions.None);
        }
        public static string Base64Decode(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
        public static string Encrypt(string password)
        {
            SHA256Managed crypt = new SHA256Managed();
            string hash = string.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte bit in crypto)
                hash += bit.ToString("x2");
            return hash;
        }
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