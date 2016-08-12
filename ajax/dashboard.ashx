<%@ WebHandler Language="C#" Class="DashboardHandler" %>
public class DashboardHandler : Ajax
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list")
        {
            //R.Data = SAAO.Event.DashboardJson();
            return;
        }
    }
}