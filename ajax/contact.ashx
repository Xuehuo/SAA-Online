<%@ WebHandler Language="C#" Class="ContactHandler" %>
public class ContactHandler : Ajax
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list")
        {
            R.Data = SAAO.User.ListJson();
        }
    }
}