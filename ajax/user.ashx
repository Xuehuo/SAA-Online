<%@ WebHandler Language="C#" Class="UserHandler" %>
using System;
using System.Text.RegularExpressions;

public class UserHandler : AjaxHandler
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
            }
            else
                R.Flag = 2;
        }
        if (!SAAO.User.IsLogin) return;
        if (context.Request["action"] == "password")
        {
            if (!SAAO.User.Current.SetPassword(context.Request.Form["password"], context.Request.Form["newpassword"]))
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
            if (context.Request.Form["mail"] == null || !Regex.IsMatch(context.Request.Form["mail"],"\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*")) return;
            if (context.Request.Form["classnum"] == null || !Regex.IsMatch(context.Request.Form["classnum"], "\\d{2}|\\d{1}")) return;
            R.Flag = 0;
            SAAO.User.Current.Phone = context.Request.Form["phone"];
            SAAO.User.Current.Mail = context.Request.Form["mail"];
            SAAO.User.Current.Class = int.Parse(context.Request.Form["classnum"]);
        }
    }
}
