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
            // TODO: finish the OOP design of Event
            //
        }
        /// <summary>
        /// List current events in the database in JSON
        /// </summary>
        /// <returns>JSON of current events [{title,start,end,backgroundColor},...]</returns>
        public static JArray ListJson()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.AdapterJson("SELECT * FROM [Calendar]");
            return new JArray();
        }

        /// <summary>
        /// Generate Json for dashboard
        /// </summary>
        /// <returns>Event Json for dashboard</returns>
        public static JObject DashboardJson()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@group", SqlIntegrate.DataType.Int, User.Current.Group);
            JObject o = new JObject
            {
                ["group"] = User.Current.GroupName,
                ["begin"] = si.AdapterJson($"SELECT * FROM [Calendar] WHERE [group] = @group AND [start] = CONVERT(varchar(10),getdate(),110)"),
                ["doing"] = si.AdapterJson($"SELECT * FROM [Calendar] WHERE [group] = @group AND [start] < getdate() AND [end] > getdate()"),
                ["todo"] = si.AdapterJson($"SELECT * FROM [Calendar] WHERE [group] = @group AND [end] = CONVERT(varchar(10),getdate(),110)")
            };
            return o;
        }
    }
}