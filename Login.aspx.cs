using System;

public partial class LoginPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (Request["code"] != null && Request["state"] != null && Request["state"] == "saalogin")
            SAAO.Utility.Log(Request["code"]);
        if (SAAO.User.IsLogin)
        {
            Response.Redirect("dashboard");
            return;
        }
        Guid otl;
        if (Request["otl"] == null || !Guid.TryParse(Request["otl"], out otl)) return;
        if (SAAO.User.Login(otl))
            Response.Redirect("dashboard");
    }
}