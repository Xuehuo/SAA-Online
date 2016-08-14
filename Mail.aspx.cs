public partial class MailPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Master.PageName = "mail";
        Master.PageDisplayName = "邮件";
        Master.PageColor = "#007480";
    }
}