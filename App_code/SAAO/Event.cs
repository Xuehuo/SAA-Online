using Newtonsoft.Json.Linq;
using System;
using System.Data;

namespace SAAO
{
    /// <summary>
    /// Event 日历
    /// </summary>
    public class Event
    {
        private static readonly string AllGroupColor = "#555555";
        private static readonly string FeijiColor = "#e74c3c";
        private static readonly string JishuColor = "#4c6592";
        private static readonly string UnknownGroupColor = "#BBBBBB";
        private static readonly int[] Dept = {-3,-3,-3,-3,-4,-4,-4};

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
        public static JObject UpdateEvent(JObject event_obj)
        {
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@event_text", SqlIntegrate.DataType.NVarChar, event_obj["event_text"]);
            si.AddParameter("@start_date", SqlIntegrate.DataType.VarChar, ChangeDateFormat(event_obj["start_date"].ToString()));
            si.AddParameter("@end_date", SqlIntegrate.DataType.VarChar, ChangeDateFormat(event_obj["end_date"].ToString()));

            var event_text = event_obj["event_text"].ToString();
            var group_tag = event_text.Length > 2 ? event_text.Substring(0, 2) : event_text;
            var group_index = -1;
            var group_color = UnknownGroupColor;
            if (group_tag == "全体")
            {
                group_index = -2; group_color = AllGroupColor;
            }
            else if(group_tag == "非技")
            {
                group_index = -3; group_color = FeijiColor;
            }
            else if(group_tag == "技术")
            {
                group_index = -4; group_color = JishuColor;
            }
            else
            {
                group_index = Organization.Current.GetGroupIndex(group_tag + "组");
                if (group_index == -1)
                    group_color = UnknownGroupColor;
                else
                    group_color = Organization.Current.GetGroupColor(group_index);
            }
            si.AddParameter("@group", SqlIntegrate.DataType.Int, group_index);
            si.AddParameter("@color", SqlIntegrate.DataType.VarChar, group_color);

            var back = new JObject { ["idORerr"] = "", ["color"] = group_color };

            Guid event_guid;
            if (Guid.TryParse(event_obj["event_id"].ToString(), out event_guid))
            {
                //update
                si.AddParameter("@event_id", SqlIntegrate.DataType.VarChar, event_guid.ToString().ToUpper());
                back["idORerr"] = si.Execute("UPDATE [Calendar] SET [text] = @event_text, [start_date] = @start_date, [end_date] = @end_date, [group] = @group, [color] = @color WHERE [id] = @event_id") == 0 ? "event-does-not-exist" : event_guid.ToString().ToUpper();
            }
            else
            {
                //create
                string new_guid = Guid.NewGuid().ToString().ToUpper();
                si.AddParameter("@event_id", SqlIntegrate.DataType.VarChar, new_guid);
                si.Execute("INSERT INTO [Calendar] ([id], [text], [start_date], [end_date], [group], [color]) VALUES (@event_id, @event_text, @start_date, @end_date, @group, @color)");
                back["idORerr"] = new_guid;
            }
            return back;
        }

        /// <summary>
        /// Delete an event
        /// </summary>
        public static void DeleteEvent(string event_id)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@event_id", SqlIntegrate.DataType.VarChar, event_id);
            si.Execute("DELETE FROM [Calendar] WHERE [id] = @event_id");
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
                ["begin"] = ListMissionNames("begin"),
                ["doing"] = ListMissionNames("ongoing"),
                ["todo"] = ListMissionNames("end")
            };
            return o;
        }

        ///<summary>
        ///Convert strange default dates (Eg. Fri Aug 19 2016 00:00:00 GMT+0800) to MM/dd/yyyy HH:mm
        ///</summary>
        ///<return>MM/dd/yyyy HH:mm</return>
        //愚蠢的日历插件不认得自己输出的日期格式
        private static string ChangeDateFormat(string origin_date)
        {
            var ci = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            var origin_date_obj = DateTime.ParseExact(origin_date.Substring(0, 21), "ddd MMM dd yyyy HH:mm", ci);
            return origin_date_obj.ToString("MM.dd.yyyy HH:mm").Replace('.', '/');
        }

        ///<summary>
        ///List beginning/ongoing/ending missions in JArray
        ///</summary>
        ///<return>list of mission names corresponding to the type</return>
        private static JArray ListMissionNames(string type)
        {
            SqlIntegrate si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@group", SqlIntegrate.DataType.Int, User.Current.Group);
            si.AddParameter("@dept", SqlIntegrate.DataType.Int, Dept[User.Current.Group]);
            si.AddParameter("@sd", SqlIntegrate.DataType.Date, DateTime.Today);
            si.AddParameter("@ed", SqlIntegrate.DataType.Date, DateTime.Today.AddDays(1));
            DataTable dt;
            if (type == "begin")
                dt = si.Adapter("SELECT [text] FROM [Calendar] WHERE ([group]=@group OR [group]='-2' OR [group]=@dept) AND [start_date]>=@sd AND [start_date]<@ed");
            else if (type == "end")
                dt = si.Adapter("SELECT [text] FROM [Calendar] WHERE ([group]=@group OR [group]='-2' OR [group]=@dept) AND [end_date]>@sd AND [end_date]<=@ed");
            else
                dt = si.Adapter("SELECT [text] FROM [Calendar] WHERE ([group]=@group OR [group]='-2' OR [group]=@dept) AND [start_date]<@sd AND [end_date]>@ed");
            var array = new JArray();
            foreach (DataRow r in dt.Rows)
            {
                array.Add(r[0].ToString());
            }
            return array;
        }
    }
}