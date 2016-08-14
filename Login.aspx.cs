public partial class LoginPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (SAAO.User.IsLogin)
            Response.Redirect("/");        
    }
}