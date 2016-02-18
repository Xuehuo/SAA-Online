﻿<%@ WebHandler Language="C#" Class="userHandler" %>
using System;
using System.Web;
using System.Web.SessionState;
public class userHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request["action"] != null && SAAO.User.IsLogin)
            switch (context.Request["action"].ToString())
            {
                case "password": // change password
                    string password = context.Request.Form["password"].ToString();
                    string passwordNew = context.Request.Form["newpassword"].ToString();
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
                    break;
                case "logout": // user logout
                    SAAO.User.Current.Logout();
                    context.Response.Write("{\"flag\": 0}");
                    break;
            }
            // user login
        else if (context.Request["action"] != null && context.Request["action"] == "login" && context.Request.Form["username"] != null && context.Request.Form["password"] != null && !SAAO.User.IsLogin)
        {
            string username = context.Request.Form["username"].ToString().ToLower();
            string password = context.Request.Form["password"].ToString();
            try
            {
                SAAO.SqlIntegrate si = new SAAO.SqlIntegrate(SAAO.Utility.connStr);
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
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
