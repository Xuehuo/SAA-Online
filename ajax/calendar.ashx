<%@ WebHandler Language="C#" Class="CalendarHandler" %>
public class CalendarHandler : Ajax
{
    public override void Process(System.Web.HttpContext context)
    {
        if (context.Request["action"] == null || !SAAO.User.IsLogin) return;
        if (context.Request["action"] == "list")
        {
            R.Data = SAAO.Event.ListJson();
        }
        else if (context.Request["action"] == "update")
        {
            if (context.Request.Form["event_id"] == null || context.Request.Form["start_date"] == null || context.Request.Form["end_date"] == null) return;
            if (context.Request.Form["event_text"] == null)
            {
                R.Flag = 2;
            }
            else
            {
                var o = new Newtonsoft.Json.Linq.JObject
                {
                    ["event_id"] = context.Request.Form["event_id"],
                    ["event_text"] = context.Request.Form["event_text"],
                    ["start_date"] = context.Request.Form["start_date"],
                    ["end_date"] = context.Request.Form["end_date"]
                };
                R.Data = SAAO.Event.UpdateEvent(o);
            }
        }
        else if (context.Request["action"] == "delete")
        {
            if (context.Request["id"] == null) return;
            SAAO.Event.DeleteEvent(context.Request["id"].ToString());
        }
    }
}