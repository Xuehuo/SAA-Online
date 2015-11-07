using System;
public partial class mailPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!SAAO.User.IsLogin)
            Response.Redirect("login");
    }
}