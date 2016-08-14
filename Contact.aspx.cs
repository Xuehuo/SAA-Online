using System;
public partial class ContactPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.PageName = "contact";
        Master.PageColor = "#085E08";
        if (!SAAO.User.IsLogin)
            Response.Redirect("login");
    }
}