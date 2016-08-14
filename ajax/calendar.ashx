<%@ WebHandler Language="C#" Class="CalendarHandler" %>
public class CalendarHandler : AjaxHandler
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list")
        {
            R.Data = SAAO.Event.ListJson();
        }
    }
}