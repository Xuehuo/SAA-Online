using System;
public partial class loginPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (SAAO.User.IsLogin)
            Response.Redirect("/");        
    }
}