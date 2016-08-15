public partial class SettingPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Master.PageName = "setting";
        Master.PageDisplayName = "设置";
        Master.PageColor = "#2C3E50";
    }
}