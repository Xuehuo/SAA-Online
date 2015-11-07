using System;
using System.Data;
using System.Web;
namespace SAAO
{
    /// <summary>
    /// SAAO User
    /// </summary>
    public class User
    {
        private int ID;
        public string UUID;
        public string username;
        private string password;
        private string SN;
        public string realname;
        private int @class;
        public string mail;
        public string phone;
        public int initial;
        public int group;
        public string groupName;
        public int job;
        public string jobName;
        public string passwordRaw;
        public int senior;
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
        public static bool IsLogin
        {
            get
            {
                return !(Current == null);
            }
        }

        public bool IsExecutive//SENIOR TWO!
        {
            get
            {
                return Array.IndexOf(Organization.IMPT_MEMBER, job) != -1;
            }
        }

        public bool IsGroupHeadman//SENIOR ONE!
        {
            get
            {
                return senior == 1 && job == 5;
            }
        }

        public bool IsSupervisor
        {
            get
            {
                return groupName == "审计组";
            }
        }
        private bool Verify(string password)
        {
            string SALT = this.password.Substring(0, 6);
            string passwordVerify = SALT + Utility.Encrypt(SALT + password);
            return (this.password == passwordVerify);
        }
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
        public void Logout()
        {
            Current = null;
        }
        public bool SetPassword(string password, string passwordNew)
        {
            if (Verify(password))
            {
                SqlIntegrate si = new SqlIntegrate(Utility.connStr);
                string SALT = this.password.Substring(0, 6);
                si.Execute("update [User] set password = '" + SALT + Utility.Encrypt(SALT + passwordNew) + "' where UUID = '" + UUID + "'");//更新密码
                si = new SqlIntegrate(Mail.connStr);
                si.Execute("update [hm_accounts] set accountpassword = '" + SALT + Utility.Encrypt(SALT + passwordNew) + "' where accountaddress = '" + username + "@xuehuo.org'");//更新邮件系统密码
                passwordRaw = passwordNew;
                return true;
            }
            else
                return false;
        }
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