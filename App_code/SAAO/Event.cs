using Newtonsoft.Json.Linq;
using System;

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
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            return si.AdapterJson("SELECT * FROM [Calendar]");
        }

        /// <summary>
        /// Create or update an event
        /// </summary>
        /// <returns>A new id of the event (GUID)</returns>
        public static string UpdateEvent(JObject event_obj)
        {
            Guid event_guid;
            if (Guid.TryParse(event_obj.GetValue("event_id").ToString(), out event_guid))
            {
                //update
            }
            else
            {
                //create
            }
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