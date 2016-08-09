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
        public static string ListJson()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            string rt = si.AdapterJson("SELECT [title], [start], [end], [group] FROM Calendar");
            for (int i = 0; i < Organization.Current.Structure.Select("[group] IS NOT NULL").Length; i ++)
                rt = rt.Replace("\"group\":" + i + "}", "\"backgroundColor\":\"" + Organization.Current.Structure.Select("[group] IS NOT NULL")[i]["color"] + "\"}");
            // Group 255 represents a default one.
            return rt.Replace(",\"group\":255","");
        }

        /// <summary>
        /// Generate Json for dashboard
        /// </summary>
        /// <returns>Event Json for dashboard</returns>
        public static string DashboardJson()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            string group = User.Current.GroupName;
            string begin = si.AdapterJson($"SELECT * FROM [Calendar] WHERE [group] = '{User.Current.Group}' AND [start] = CONVERT(varchar(10),getdate(),110)");
            string doing = si.AdapterJson($"SELECT * FROM [Calendar] WHERE [group] = '{User.Current.Group}' AND [start] < getdate() AND [end] > getdate()");
            string todo = si.AdapterJson($"SELECT * FROM [Calendar] WHERE [group] = '{User.Current.Group}' AND [end] = CONVERT(varchar(10),getdate(),110)");
            return "{\"group\":\"" + group + "\",\"begin\":" + begin + ",\"doing\":" + doing + ",\"todo\":" + todo + "}";
        }
    }
}