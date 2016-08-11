using Newtonsoft.Json.Linq;

namespace SAAO
{
    /// <summary>
    /// Event 日历
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Event constructor
        /// </summary>
        public Event()
        {
            //
            // TODO: Merge from brave-new-calendar
            //
        }
        /// <summary>
        /// List current events in the database in JSON
        /// </summary>
        /// <returns>JSON of current events [{title,start,end,backgroundColor},...]</returns>
        public static JArray ListJson()
        {
            return new JArray();
        }

        /// <summary>
        /// Generate Json for dashboard
        /// </summary>
        /// <returns>Event Json for dashboard</returns>
        public static JObject DashboardJson()
        {
            var o = new JObject
            {
                ["group"] = User.Current.GroupName,
                ["begin"] = null,
                ["doing"] = null,
                ["todo"] = null
            };
            return o;
        }
    }
}