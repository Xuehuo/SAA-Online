public partial class LoginPage : System.Web.UI.Page
{
    public string Wechat;
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (Request["code"] != null && Request["state"] != null && Request["state"] == "saalogin")
        {
            var accessToken = SAAO.Utility.GetAccessToken();
            Wechat = SAAO.Utility.HttpRequest(
                $"https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={accessToken}&code={Request["code"]}").Split('\"')[3];
            Session.Add("wechat", Wechat);
            if (SAAO.User.WechatLogin(Wechat))
                Response.Redirect("dashboard");
        }
        if (SAAO.User.IsLogin)
            Response.Redirect("dashboard");
    }
}