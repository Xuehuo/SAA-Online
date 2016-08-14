public partial class ContactPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Master.PageName = "contact";
        Master.PageDisplayName = "联系人";
        Master.PageColor = "#085E08";
    }
}