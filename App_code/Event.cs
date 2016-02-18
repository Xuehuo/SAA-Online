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
        public static string ListJSON()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            string rt = si.AdapterJSON("SELECT [title], [start], [end], [group] FROM Calendar");
            for (int i = 0; i < Organization.Current.structure.Select("[group] IS NOT NULL").Length; i ++)
                rt = rt.Replace("\"group\":" + i + "}", "\"backgroundColor\":\"" + Organization.Current.structure.Select("[group] IS NOT NULL")[i]["color"] + "\"}");
            // Group 255 represents a default one.
            return rt.Replace(",\"group\":255","");
        }
    }
}