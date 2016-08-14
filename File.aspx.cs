public partial class FilePage : System.Web.UI.Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Master.PageName = "file";
        Master.PageDisplayName = "文件";
        Master.PageColor = "#25489F";
    }
}