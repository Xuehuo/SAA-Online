<%@ WebHandler Language="C#" Class="UserHandler" %>
public class UserHandler : Ajax
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
                SAAO.User user = new SAAO.User(context.Request.Form["username"].ToLower());
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
    }
}
