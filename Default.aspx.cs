public partial class DashboardPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Master.PageName = "dashboard";
        Master.PageDisplayName = "仪表盘";
        Master.PageColor = "#2C3E50";
    }
}