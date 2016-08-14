using System;
public partial class MailPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.PageName = "mail";
        Master.PageColor = "#007480";
        if (!SAAO.User.IsLogin)
            Response.Redirect("login");
    }
}