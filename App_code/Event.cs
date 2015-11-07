namespace SAAO
{
    /// <summary>
    /// Event 的摘要说明
    /// </summary>
    public class Event
    {
        public Event()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public static string ListJSON()
        {
            SqlIntegrate si = new SqlIntegrate(Utility.connStr);
            string rt = si.AdapterJSON("SELECT [title], [start], [end], [group] FROM Calendar");
            for (int i = 0; i < Organization.Current.structure.Select("[group] IS NOT NULL").Length; i ++)
                rt = rt.Replace("\"group\":" + i + "}", "\"backgroundColor\":\"" + Organization.Current.structure.Select("[group] IS NOT NULL")[i]["color"] + "\"}");
            return rt.Replace(",\"group\":255","");
        }
    }
}