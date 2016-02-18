using System;
using System.Data;
using System.Web;
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
        private int ID;
        public string UUID;
        public string username;
        private string password;
        /// <summary>
        /// Student number
        /// </summary>
        private string SN;
        public string realname;
        /// <summary>
        /// Class
        /// </summary>
        private int @class;
        // TODO: Remove or rename this useless key

        public string mail;
        public string phone;
        /// <summary>
        /// Initial letter of surname (0 represents 'A')
        /// </summary>
        public int initial;
        // TODO: another way to obtain (username[2] - 'A')

        public int group;
        public string groupName;
        public int job;
        public string jobName;
        /// <summary>
        /// Raw password string (only filled when it is current user)
        /// </summary>
        public string passwordRaw;
        /// <summary>
        /// Senior (1 or 2)
        /// </summary>
        public int senior;
        /// <summary>
        /// User constructor
        /// </summary>
        /// <param name="UUID">User UUID</param>
        public User(Guid UUID)
        {
            this.UUID = UUID.ToString().ToUpper();
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            DataRow dr = si.Reader("SELECT * FROM [User] WHERE [UUID] = '" + this.UUID + "'");
            ID = Convert.ToInt32(dr["ID"]);
            password = dr["password"].ToString();
            realname = dr["realname"].ToString();
            SN = dr["SN"].ToString();
            @class = Convert.ToInt32(dr["class"]);
            mail = dr["mail"].ToString();
            phone = dr["phone"].ToString();
            initial = Convert.ToInt32(dr["initial"]);
            group = Convert.ToInt32(dr["group"].ToString());
            job = Convert.ToInt32(dr["job"].ToString());
            groupName = Organization.Current.GetGroupName(group);
            jobName = Organization.Current.GetJobName(job);
            if (SN.Substring(0, 4) == Organization.Current.state.seniorOne)
                senior = 1;
            else if (SN.Substring(0, 4) == Organization.Current.state.seniorTwo)
                senior = 2;
        }
        /// <summary>
        /// User constructor
        /// </summary>
        /// <param name="username">Username</param>
        public User(string username)
        {
            this.username = username;
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            si.InitParameter(1);
            si.AddParameter("@username", SqlIntegrate.DataType.VarChar, username, 50);
            DataRow dr = si.Reader("SELECT * FROM [User] WHERE [username] = @username");
            ID = Convert.ToInt32(dr["ID"]);
            UUID = dr["UUID"].ToString();
            password = dr["password"].ToString();
            realname = dr["realname"].ToString();
            SN = dr["SN"].ToString();
            @class = Convert.ToInt32(dr["class"]);
            mail = dr["mail"].ToString();
            phone = dr["phone"].ToString();
            initial = Convert.ToInt32(dr["initial"]);
            group = Convert.ToInt32(dr["group"].ToString());
            job = Convert.ToInt32(dr["job"].ToString());
            groupName = Organization.Current.GetGroupName(group);
            jobName = Organization.Current.GetJobName(job);
            if (SN.Substring(0, 4) == Organization.Current.state.seniorOne)
                senior = 1;
            else if (SN.Substring(0, 4) == Organization.Current.state.seniorTwo)
                senior = 2;
        }
        /// <summary>
        /// Logined user of current session (values null if not logined)
        /// </summary>
        public static User Current
        {
            get
            {
                if (HttpContext.Current.Session["User"] != null)
                    return (User)HttpContext.Current.Session["User"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["User"] = value;
            }
        }
        /// <summary>
        /// Whether the user of current session has logined
        /// </summary>
        public static bool IsLogin
        {
            get
            {
                return !(Current == null);
            }
        }

        /// <summary>
        /// Whether the user is executive (only Senior Two)
        /// </summary>
        public bool IsExecutive
        {
            get
            {
                // Executive member and important member are identical
                return senior == 2 && Array.IndexOf(Organization.IMPT_MEMBER, job) != -1;
            }
        }

        /// <summary>
        /// Whether the user is headman of a group (only Senior One)
        /// </summary>
        public bool IsGroupHeadman
        {
            get
            {
                // TODO: Remove this magic number
                return senior == 1 && job == 5;
            }
        }

        /// <summary>
        /// Whether the user is in the supervising group (both Senior)
        /// </summary>
        public bool IsSupervisor
        {
            get
            {
                // TODO: Remove this magic string
                return groupName == "审计组";
            }
        }
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
            string SALT = this.password.Substring(0, 6);
            string passwordVerify = SALT + Utility.Encrypt(SALT + password);
            return (this.password == passwordVerify);
        }
        /// <summary>
        /// Verify a password and login if it's correct
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>Whether the password is correct</returns>
        public bool Login(string password)
        {
            if (Verify(password))
            {
                passwordRaw = password;
                Current = this;
                return true;
            }
            else
                return false;
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
                SqlIntegrate si = new SqlIntegrate(Utility.connStr);
                string SALT = this.password.Substring(0, 6);
                string passwordEncrypted = SALT + Utility.Encrypt(SALT + passwordNew);
                // Update the user's password of SAA Online
                si.Execute("UPDATE [User] SET password = '" + passwordEncrypted + "' WHERE UUID = '" + UUID + "'");
                si = new SqlIntegrate(Mail.connStr);
                // Update the user's password of SAA Mail (Hmailserver)
                si.InitParameter(1);
                si.AddParameter("@accountaddress", SqlIntegrate.DataType.VarChar, username + "@" + Mail.mailDomain, 50);
                si.Execute("UPDATE [hm_accounts] SET accountpassword = '" + passwordEncrypted + "' WHERE accountaddress = @accountaddress");
                passwordRaw = passwordNew;
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// List activated users in the database in JSON
        /// </summary>
        /// <returns>JSON of activated users. [{realname,senior,group,initial,jobName,groupName,phone,mail},...]</returns>
        public static string ListJSON()
        {
            string data = "[";
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            DataTable dt = si.Adapter("SELECT * FROM [User] WHERE [activated]=1");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data += "{";
                data += "\"realname\":\"" + Utility.string2JSON(dt.Rows[i]["realname"].ToString()) + "\",";
                data += "\"senior\":" + ((dt.Rows[i]["SN"].ToString().Substring(0, 4) == Organization.Current.state.structureCurrent) ? "2" : "1") + ",";
                data += "\"group\":" + dt.Rows[i]["group"].ToString() + ",";
                data += "\"initial\":" + dt.Rows[i]["initial"].ToString() + ",";
                data += "\"jobName\":\"" + Utility.string2JSON(Organization.Current.GetJobName(Convert.ToInt32(dt.Rows[i]["job"].ToString()))) + "\",";
                data += "\"groupName\":\"" + Utility.string2JSON(Organization.Current.GetGroupName(Convert.ToInt32(dt.Rows[i]["group"].ToString()))) + "\",";
                data += "\"phone\":\"" + dt.Rows[i]["phone"].ToString() + "\",";
                data += "\"mail\":\"" + Utility.string2JSON(dt.Rows[i]["mail"].ToString()) + "\"},";
            }
            data += "]";
            data = data.Replace(",]", "]");
            return data;
        }
    }
}