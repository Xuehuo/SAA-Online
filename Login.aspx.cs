using System;

public partial class LoginPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (SAAO.User.IsLogin)
        {
            Response.Redirect("/");
            return;
        }
        Guid otl;
        if (Request["otl"] == null || !Guid.TryParse(Request["otl"], out otl)) return;
        SAAO.User.Login(otl);
        Response.Redirect("/");
    }
}