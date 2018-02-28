using System.Text.RegularExpressions;

public class UserHandler : SAAO.AjaxHandler
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null) return;
        if (context.Request["action"] == "login")
        {
            if (context.Request.Form["username"] == null
                    || context.Request.Form["password"] == null
                    || SAAO.User.IsLogin) return;
            if (SAAO.User.Exist(context.Request.Form["username"].ToLower()))
            {
                var user = new SAAO.User(context.Request.Form["username"].ToLower());
                if (!user.Login(context.Request.Form["password"]))
                    R.Flag = 2;
                if (user.Wechat == "" && context.Session["wechat"] != null)
                    user.Wechat = context.Session["wechat"].ToString();
            }
            else
                R.Flag = 2;
        }
        if (!SAAO.User.IsLogin) return;
        if (context.Request["action"] == "password")
        {
            if (SAAO.User.Current.Verify(context.Request.Form["password"]))
                SAAO.User.Current.PasswordRaw = context.Request.Form["newpassword"];
            else
                R.Flag = 2;
        }
        else if (context.Request["action"] == "logout")
        {
            SAAO.User.Current.Logout();
        }
        else if (context.Request["action"] == "info")
        {
            R.Flag = 2;
            if (context.Request.Form["phone"] == null || !Regex.IsMatch(context.Request.Form["phone"], "\\d{11}")) return;
            if (context.Request.Form["mail"] == null || !Regex.IsMatch(context.Request.Form["mail"], "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*")) return;
            if (context.Request.Form["classnum"] == null || !Regex.IsMatch(context.Request.Form["classnum"], "\\d{2}|\\d{1}")) return;
            R.Flag = 0;
            SAAO.User.Current.Phone = context.Request.Form["phone"];
            SAAO.User.Current.Mail = context.Request.Form["mail"];
            SAAO.User.Current.Class = int.Parse(context.Request.Form["classnum"]);
        }
        else if (context.Request["action"] == "unbind")
        {
            SAAO.User.Current.Wechat = "";
            SAAO.User.Current.FilePush = 0;
        }
        else if (context.Request["action"] == "filepush")
        {
            if (context.Request["enable"] == null) return;
            if (context.Request["enable"].ToString().Equals("0")) //Turn off this service
            {
                SAAO.User.Current.FilePush = 0; return;
            }
            if(SAAO.User.Current.Wechat=="") //haven't bind wechat
            {
                R.Flag = 1;return;
            }
            SAAO.User.Current.FilePush = 1;
        }
    }
}
