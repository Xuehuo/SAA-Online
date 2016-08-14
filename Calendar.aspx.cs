using System;
public partial class CalendarPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.PageName = "calendar";
        Master.PageColor = "#D94839";
        if (!SAAO.User.IsLogin)
            Response.Redirect("login");
    }
}