using System;
public partial class NotificationPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.PageName = "notification";
        Master.PageColor = "#98A000";
        if (!SAAO.User.IsLogin)
            Response.Redirect("login");
    }
}