public partial class CalendarPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Master.PageName = "calendar";
        Master.PageDisplayName = "日历";
        Master.PageColor = "#D94839";
    }
}