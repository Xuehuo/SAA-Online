using System;
using System.Web;
using Newtonsoft.Json.Linq;

namespace SAAO
{
    /// <summary>
    /// User 用户
    /// </summary>
    public class User
    {
        /// <summary>
        /// Index in database
        /// </summary>
        private readonly int _id;
        public string UUID;
        public string Username;
        private string _password;
        private string _passwordRaw;
        public string PasswordRaw
        {
            get { return _passwordRaw; }
            set
            {
                _passwordRaw = value;
                var si = new SqlIntegrate(Utility.ConnStr);
                var salt = _password.Substring(0, 6);
                _password = salt + Utility.Encrypt(salt + value);
                // Update the user's password of SAA Online
                si.AddParameter("@password", SqlIntegrate.DataType.VarChar, _password);
                si.AddParameter("@UUID", SqlIntegrate.DataType.VarChar, UUID);
                si.Execute("UPDATE [User] SET [password] = @password WHERE [UUID] = @UUID");
                si = new SqlIntegrate(SAAO.Mail.ConnStr);
                // Update the user's password of SAA Mail (Hmailserver)
                si.ResetParameter();
                si.AddParameter("@accountpassword", SqlIntegrate.DataType.VarChar, _password);
                si.AddParameter("@accountaddress", SqlIntegrate.DataType.VarChar, Username + "@" + SAAO.Mail.MailDomain, 50);
                si.Execute("UPDATE [hm_accounts] SET accountpassword = @accountpassword WHERE accountaddress = @accountaddress");
            }
        }
        /// <summary>
        /// Student number
        /// </summary>
        private readonly string _sn;
        public string Realname;
        /// <summary>
        /// Class
        /// </summary>
        private int _class;

        public int Class
        {
            get { return _class; }
            set
            {
                _class = value;
                var si = new SqlIntegrate(Utility.ConnStr);
                si.AddParameter("@UUID", SqlIntegrate.DataType.VarChar, UUID);
                si.AddParameter("@class", SqlIntegrate.DataType.Int, value);
                si.Execute("UPDATE [User] SET [class] = @class WHERE [UUID] = @UUID");
            }
        }

        private string _mail;

        /// <summary>
        /// Personal mail
        /// </summary>
        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                var si = new SqlIntegrate(Utility.ConnStr);
                si.AddParameter("@mail", SqlIntegrate.DataType.VarChar, value, 50);
                si.AddParameter("@UUID", SqlIntegrate.DataType.VarChar, UUID);
                si.Execute("UPDATE [User] SET [mail] = @mail WHERE [UUID] = @UUID");
            }
        }

        private string _phone;

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                var si = new SqlIntegrate(Utility.ConnStr);
                si.AddParameter("@phone", SqlIntegrate.DataType.VarChar, value, 11);
                si.AddParameter("@UUID", SqlIntegrate.DataType.VarChar, UUID);
                si.Execute("UPDATE [User] SET [phone] = @phone WHERE [UUID] = @UUID");
            }
        }
        /// <summary>
        /// Initial letter of surname (0 represents 'A')
        /// </summary>
        public readonly int Initial;
        
        private string _wechat;
        public string Wechat
        {
            get
            {
                var si = new SqlIntegrate(Utility.ConnStr);
                si.AddParameter("@UUID", SqlIntegrate.DataType.VarChar, UUID);
                _wechat = si.Query("SELECT [wechat] FROM [User] WHERE [UUID] = @UUID").ToString();
                return _wechat;
            }
            set
            {
                _wechat = value;
                var si = new SqlIntegrate(Utility.ConnStr);
                si.AddParameter("@wechat", SqlIntegrate.DataType.VarChar, value);
                si.AddParameter("@UUID", SqlIntegrate.DataType.VarChar, UUID);
                si.Execute("UPDATE [User] SET [wechat] = @wechat WHERE [UUID] = @UUID");
            }
        }

        public int Group;
        public string GroupName;
        public int Job;
        public string JobName;
        /// <summary>
        /// Senior (1 or 2)
        /// </summary>
        public readonly int Senior;
        /// <summary>
        /// User constructor
        /// </summary>
        /// <param name="uuid">User UUID</param>
        public User(Guid uuid)
        {
            UUID = uuid.ToString().ToUpper();
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@UUID", SqlIntegrate.DataType.VarChar, UUID);
            var dr = si.Reader("SELECT * FROM [User] WHERE [UUID] = @UUID");
            _id = Convert.ToInt32(dr["ID"]);
            _password = dr["password"].ToString();
            Realname = dr["realname"].ToString();
            _sn = dr["SN"].ToString();
            _class = Convert.ToInt32(dr["class"]);
            _mail = dr["mail"].ToString();
            _phone = dr["phone"].ToString();
            _wechat = dr["wechat"].ToString();
            Initial = dr["username"].ToString()[dr["realname"].ToString().Length - 1] - 'a' + 1;
            Group = Convert.ToInt32(dr["group"].ToString());
            Job = Convert.ToInt32(dr["job"].ToString());
            GroupName = Organization.Current.GetGroupName(Group);
            JobName = Organization.Current.GetJobName(Job);
            if (_sn.Substring(0, 4) == Organization.Current.State.SeniorOne)
                Senior = 1;
            else if (_sn.Substring(0, 4) == Organization.Current.State.SeniorTwo)
                Senior = 2;
        }
        /// <summary>
        /// User constructor
        /// </summary>
        /// <param name="username">Username</param>
        public User(string username)
        {
            Username = username;
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@username", SqlIntegrate.DataType.VarChar, username, 50);
            var dr = si.Reader("SELECT * FROM [User] WHERE [username] = @username");
            _id = Convert.ToInt32(dr["ID"]);
            UUID = dr["UUID"].ToString();
            _password = dr["password"].ToString();
            Realname = dr["realname"].ToString();
            _sn = dr["SN"].ToString();
            _class = Convert.IsDBNull(dr["class"]) ? 0 : Convert.ToInt32(dr["class"]);
            _mail = dr["mail"].ToString();
            _phone = dr["phone"].ToString();
            _wechat = dr["wechat"].ToString();
            Initial = dr["username"].ToString()[dr["realname"].ToString().Length - 1] - 'a' + 1;
            Group = Convert.ToInt32(dr["group"].ToString());
            Job = Convert.ToInt32(dr["job"].ToString());
            GroupName = Organization.Current.GetGroupName(Group);
            JobName = Organization.Current.GetJobName(Job);
            if (_sn.Substring(0, 4) == Organization.Current.State.SeniorOne)
                Senior = 1;
            else if (_sn.Substring(0, 4) == Organization.Current.State.SeniorTwo)
                Senior = 2;
            else // for retired member
                Senior = 1;
        }
        /// <summary>
        /// Check whether the user of a username exists and is activated
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Whether the user of a username exists and is activated</returns>
        public static bool Exist(string username)
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@username", SqlIntegrate.DataType.VarChar, username, 50);
            var count = Convert.ToInt32(si.Query("SELECT COUNT(*) FROM [User] WHERE [username] = @username"));
            return count == 1;
        }
        /// <summary>
        /// Logged user of current session (values null if not logged)
        /// </summary>
        public static User Current
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["User"] != null)
                    return (User)System.Web.HttpContext.Current.Session["User"];
                return null;
            }
            set
            {
                if (System.Web.HttpContext.Current.Session["User"] != null)
                    System.Web.HttpContext.Current.Session["User"] = value;
                else
                    System.Web.HttpContext.Current.Session.Add("User", value);
            }
        }
        /// <summary>
        /// Whether the user of current session has logined
        /// </summary>
        public static bool IsLogin => Current != null;

        /// <summary>
        /// Whether the user is executive (only Senior Two)
        /// </summary>
        public bool IsExecutive => Senior == 2 && Organization.Current.Structure.Select("[executive] = 1 AND [job] = " + Job).Length != 0;
         // Executive member and important member are identical

        /// <summary>
        /// Whether the user is headman of a group (only Senior One)
        /// </summary>
        public bool IsGroupHeadman => Senior == 1 && Organization.Current.Structure.Select("[headman] = 1 AND [job] = " + Job).Length != 0;

        /// <summary>
        /// Whether the user is in the supervising group (both Senior)
        /// </summary>
        public bool IsSupervisor => Organization.Current.Structure.Select("[supervisor] = 1 AND [group] = " + Group).Length != 0;

        /// <summary>
        /// Verify whether a string is the password of the user
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Whether a string is the password of the user</returns>
        public bool Verify(string password)
        {
            /* The raw password is encrypted in this way:
             *  (SALT is generated randomly and its length is 6)
             *        ┌──────┬───────────────┐
             *  [A] = │ SALT  │ Raw password     │ (join SALT and raw password)
             *        └──────┴───────────────┘
             *  [B] = SHA256([A])
             *        ┌──────┬───────────────┐
             *  [C] = │ SALT  │      [B]         │ (join SALT and [B])
             *        └──────┴───────────────┘
             *  At last, store [C] in the database
             */
            var salt = _password.Substring(0, 6);
            var passwordVerify = salt + Utility.Encrypt(salt + password);
            return (_password == passwordVerify);
        }
        /// <summary>
        /// Verify a password and login if it's correct
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>Whether the password is correct</returns>
        public bool Login(string password)
        {
            if (!Verify(password)) return false;
            Current = this;
            _passwordRaw = password;
            return true;
        }
        /// <summary>
        /// Wechat Login
        /// </summary>
        /// <param name="wechatId">Wechat ID(username)</param>
        /// <returns>Whether the wechat ID has been bound</returns>
        public static bool WechatLogin(string wechatId)
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@wechat", SqlIntegrate.DataType.VarChar, wechatId);
            var r = si.Query(
                "SELECT [username] FROM [User] WHERE [wechat] = @wechat");
            if (r == null) return false;
            Current = new User(r.ToString());
            // TODO: no raw password raw storage!
            return true;
        }
        /// <summary>
        /// Logout the user of current session
        /// </summary>
        public void Logout()
        {
            Current = null;
            if (HttpContext.Current.Session["wechat"] != null)
                HttpContext.Current.Session.Remove("wechat");
        }        
        /// <summary>
        /// List activated users in the database in JSON
        /// </summary>
        /// <returns>JSON of activated users. [{realname,senior,group,initial,jobName,groupName,phone,mail},...]</returns>
        public static JArray ListJson()
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            var dt = si.Adapter("SELECT * FROM [User] WHERE [activated] = 1");
            var a = new JArray();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var o = new JObject
                {
                    ["realname"] = dt.Rows[i]["realname"].ToString(),
                    ["senior"] =
                        (dt.Rows[i]["SN"].ToString().Substring(0, 4) == Organization.Current.State.StructureCurrent)
                            ? 2
                            : 1,
                    ["group"] = dt.Rows[i]["group"].ToString(),
                    ["initial"] = dt.Rows[i]["username"].ToString()[dt.Rows[i]["realname"].ToString().Length - 1] - 'a' + 1,
                    ["jobName"] = Organization.Current.GetJobName(Convert.ToInt32(dt.Rows[i]["job"].ToString())),
                    ["groupName"] = Organization.Current.GetGroupName(Convert.ToInt32(dt.Rows[i]["group"].ToString())),
                    ["phone"] = dt.Rows[i]["phone"].ToString(),
                    ["mail"] = dt.Rows[i]["mail"].ToString(),
                    ["class"] = int.Parse(dt.Rows[i]["class"].ToString())
                };
                a.Add(o);
            }
            return a;
        }
    }
}