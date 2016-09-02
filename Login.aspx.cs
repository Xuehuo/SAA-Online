using System;

public partial class LoginPage : System.Web.UI.Page
{
    public string Wechat;
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (Request["code"] != null && Request["state"] != null && Request["state"] == "saalogin")
        {
            var corpId = System.Configuration.ConfigurationManager.AppSettings["WechatCorpId"];
            var corpSecret = System.Configuration.ConfigurationManager.AppSettings["WechatCorpSecret"];
            if (Application["accessTokenExpire"] == null || DateTime.Now > (DateTime) Application["accessTokenExpire"])
            {
                var data = SAAO.Utility.HttpRequestJson(
                    $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpId}&corpsecret={corpSecret}");
                Application["accessTokenExpire"] = DateTime.Now.AddSeconds(Convert.ToInt32(data["expires_in"]));
                Application["accessToken"] = data["access_token"].ToString();
            }
            var accessToken = Application["accessToken"].ToString();
            Wechat = SAAO.Utility.HttpRequestJson(
                $"https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={accessToken}&code={Request["code"]}")["UserId"].ToString();
            Session.Add("wechat", Wechat);
            if (SAAO.User.WechatLogin(Wechat))
                Response.Redirect("dashboard");
        }
        if (SAAO.User.IsLogin)
            Response.Redirect("dashboard");
    }
}