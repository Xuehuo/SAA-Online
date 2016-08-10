﻿using System;
using System.Data;
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
        private readonly string _password;
        /// <summary>
        /// Student number
        /// </summary>
        private readonly string _sn;
        public string Realname;
        /// <summary>
        /// Class
        /// </summary>
        private int _class;

        public string Mail;
        public string Phone;
        /// <summary>
        /// Initial letter of surname (0 represents 'A')
        /// </summary>
        public int Initial;

        public int Group;
        public string GroupName;
        public int Job;
        public string JobName;
        /// <summary>
        /// Raw password string (only filled when it is current user)
        /// </summary>
        public string PasswordRaw;
        /// <summary>
        /// Senior (1 or 2)
        /// </summary>
        public int Senior;
        /// <summary>
        /// User constructor
        /// </summary>
        /// <param name="uuid">User UUID</param>
        public User(Guid uuid)
        {
            UUID = uuid.ToString().ToUpper();
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            var dr = si.Reader($"SELECT * FROM [User] WHERE [UUID] = '{UUID}'");
            _id = Convert.ToInt32(dr["ID"]);
            _password = dr["password"].ToString();
            Realname = dr["realname"].ToString();
            _sn = dr["SN"].ToString();
            _class = Convert.ToInt32(dr["class"]);
            Mail = dr["mail"].ToString();
            Phone = dr["phone"].ToString();
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
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.InitParameter(1);
            si.AddParameter("@username", SqlIntegrate.DataType.VarChar, username, 50);
            var dr = si.Reader("SELECT * FROM [User] WHERE [username] = @username");
            _id = Convert.ToInt32(dr["ID"]);
            UUID = dr["UUID"].ToString();
            _password = dr["password"].ToString();
            Realname = dr["realname"].ToString();
            _sn = dr["SN"].ToString();
            _class = Convert.ToInt32(dr["class"]);
            Mail = dr["mail"].ToString();
            Phone = dr["phone"].ToString();
            Initial = Convert.ToInt32(dr["initial"]);
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
        /// Check whether the user of a username exists and is activated
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Whether the user of a username exists and is activated</returns>
        public static bool Exist(string username)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.InitParameter(1);
            si.AddParameter("@username", SqlIntegrate.DataType.VarChar, username, 50);
            return Convert.ToInt32(si.Query("SELECT COUNT(*) FROM [User] WHERE username = @username AND [activated] = 1")) == 1;
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
                System.Web.HttpContext.Current.Session["User"] = value;
            }
        }
        /// <summary>
        /// Whether the user of current session has logined
        /// </summary>
        public static bool IsLogin => Current != null;

        /// <summary>
        /// Whether the user is executive (only Senior Two)
        /// </summary>
        public bool IsExecutive => Senior == 2 && Array.IndexOf(Organization.ImptMember, Job) != -1;
         // Executive member and important member are identical

        /// <summary>
        /// Whether the user is headman of a group (only Senior One)
        /// </summary>
        public bool IsGroupHeadman => Senior == 1 && Job == 5;
        // TODO: Remove this magic number

        /// <summary>
        /// Whether the user is in the supervising group (both Senior)
        /// </summary>
        public bool IsSupervisor => GroupName == "审计组";
        // TODO: Remove this magic string

        /// <summary>
        /// Verify whether a string is the password of the user
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Whether a string is the password of the user</returns>
        private bool Verify(string password)
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
            string salt = _password.Substring(0, 6);
            string passwordVerify = salt + Utility.Encrypt(salt + password);
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
            PasswordRaw = password;
            Current = this;
            return true;
        }

        /// <summary>
        /// Logout the user of current session
        /// </summary>
        public void Logout()
        {
            Current = null;
        }
        /// <summary>
        /// Change password of the user
        /// </summary>
        /// <param name="password">Orginal password</param>
        /// <param name="passwordNew">New password</param>
        /// <returns>Whether the original password is correct</returns>
        public bool SetPassword(string password, string passwordNew)
        {
            if (Verify(password))
            {
                SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
                string salt = _password.Substring(0, 6);
                string passwordEncrypted = salt + Utility.Encrypt(salt + passwordNew);
                // Update the user's password of SAA Online
                si.Execute($"UPDATE [User] SET password = '{passwordEncrypted}' WHERE UUID = '{UUID}'");
                si = new SqlIntegrate(SAAO.Mail.ConnStr);
                // Update the user's password of SAA Mail (Hmailserver)
                si.InitParameter(1);
                si.AddParameter("@accountaddress", SqlIntegrate.DataType.VarChar, Username + "@" + SAAO.Mail.MailDomain, 50);
                si.Execute($"UPDATE [hm_accounts] SET accountpassword = '{passwordEncrypted}' WHERE accountaddress = @accountaddress");
                PasswordRaw = passwordNew;
                return true;
            }
            return false;
        }

        /// <summary>
        /// List activated users in the database in JSON
        /// </summary>
        /// <returns>JSON of activated users. [{realname,senior,group,initial,jobName,groupName,phone,mail},...]</returns>
        public static JArray ListJson()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            DataTable dt = si.Adapter("SELECT * FROM [User] WHERE [activated]=1");
            JArray a = new JArray();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                JObject o = new JObject
                {
                    ["realname"] = dt.Rows[i]["realname"].ToString(),
                    ["senior"] =
                        (dt.Rows[i]["SN"].ToString().Substring(0, 4) == Organization.Current.State.StructureCurrent)
                            ? 2
                            : 1,
                    ["group"] = dt.Rows[i]["group"].ToString(),
                    ["initial"] = dt.Rows[i]["username"].ToString()[dt.Rows[i]["realname"].ToString().Length - 1] - 'a' + 1, //int.Parse(dt.Rows[i]["initial"].ToString()),
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