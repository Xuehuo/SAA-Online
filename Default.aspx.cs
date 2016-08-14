using System;
public partial class DashboardPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.PageName = "dashboard";
        Master.PageColor = "#2C3E50";
        if (!SAAO.User.IsLogin)
            Response.Redirect("login");
    }
}