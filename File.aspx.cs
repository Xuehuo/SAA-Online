using System;
public partial class FilePage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.PageName = "file";
        Master.PageColor = "#25489F";
        if (!SAAO.User.IsLogin)
            Response.Redirect("login");
    }
}