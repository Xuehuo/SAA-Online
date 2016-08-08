<%@ WebHandler Language="C#" Class="userHandler" %>
using System;
using System.Web;
using System.Web.SessionState;
public class userHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        // user login
        if (context.Request["action"] != null && context.Request["action"] == "login" && context.Request.Form["username"] != null && context.Request.Form["password"] != null && !SAAO.User.IsLogin)
        {
            string username = context.Request.Form["username"].ToLower();
            string password = context.Request.Form["password"];
            try
            {
                SAAO.SqlIntegrate si = new SAAO.SqlIntegrate(SAAO.Utility.ConnStr);
                si.InitParameter(1);
                si.AddParameter("@username", SAAO.SqlIntegrate.DataType.VarChar, username, 50);
                if (Convert.ToInt32(si.Query("SELECT COUNT(*) FROM [User] WHERE username = @username")) > 0)
                {
                    SAAO.User user = new SAAO.User(username);
                    if (user.Login(password))
                        context.Response.Write("{\"flag\": 0}");
                    else
                        context.Response.Write("{\"flag\": 2}");
                }
                else
                    context.Response.Write("{\"flag\": 2}");
            }
            catch (Exception ex)
            {
                SAAO.Utility.Log(ex);
                context.Response.Write("{\"flag\": 3}");
            }
        }
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "password")
        {
            string password = context.Request.Form["password"];
            string passwordNew = context.Request.Form["newpassword"];
            try
            {
                if (SAAO.User.Current.SetPassword(password, passwordNew))
                    context.Response.Write("{\"flag\": 0}");
                else
                    context.Response.Write("{\"flag\": 2}");
            }
            catch (Exception ex)
            {
                SAAO.Utility.Log(ex.Message);
                context.Response.Write("{\"flag\": 3}");
            }
        }
        else if (context.Request["action"] == "logout")
        {
            SAAO.User.Current.Logout();
            context.Response.Write("{\"flag\": 0}");
        }
    }
    public bool IsReusable => false;
}
