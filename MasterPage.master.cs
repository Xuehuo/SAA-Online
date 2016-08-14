public partial class MasterPage : System.Web.UI.MasterPage
{
    public string PageName;
    public string PageDisplayName;
    public string PageColor;
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!SAAO.User.IsLogin)
            Response.Redirect("login");
    }
}
