using System;
public partial class contactPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!SAAO.User.IsLogin)
            Response.Redirect("login");
    }
}