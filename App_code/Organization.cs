using System;
using System.Data;
namespace SAAO
{
    /// <summary>
    /// Organization 架构
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// Important members ([job] in database)
        /// </summary>
        public static int[] IMPT_MEMBER = new int[] { 0, 1, 2, 3 };
        // TODO: Rename or write it in database

        public State state;

        /// <summary>
        /// Structure table
        /// </summary>
        public DataTable structure;
        /// <summary>
        /// Organization structure constructor
        /// </summary>
        /// <param name="dt">Datetime (DateTime.Now most possibly)</param>
        public Organization(DateTime dt)
        {
            state = new State(dt);
            structure = new SqlIntegrate(Utility.connStr).Adapter("SELECT * FROM [Org] WHERE [year]='" + state.structureCurrent + ((int)state.eventCurrent).ToString() + "'");
        }
        /// <summary>
        /// Current organization structure
        /// </summary>
        public static Organization Current
        {
            get
            {
                return (Organization)System.Web.HttpContext.Current.Application["org"];
            }
        }
        /// <summary>
        /// Get color of the group
        /// </summary>
        /// <param name="i">Group index</param>
        /// <returns>Group color</returns>
        public string GetGroupColor(int i)
        {
            return structure.Select("[group]=" + i)[0]["color"].ToString();
        }
        /// <summary>
        /// Get group name
        /// </summary>
        /// <param name="i">Group index</param>
        /// <returns>Group name</returns>
        public string GetGroupName(int i)
        {
            return structure.Select("[group]=" + i)[0]["name"].ToString();
        }
        /// <summary>
        /// Get job (title) name
        /// </summary>
        /// <param name="i">Job index</param>
        /// <returns>Job name</returns>
        public string GetJobName(int i)
        {
            return structure.Select("[job]=" + i)[0]["name"].ToString();
        }
    }
    public struct State
    {
        /// <summary>
        /// Year Senior One enrolled
        /// </summary>
        public string seniorOne;
        /// <summary>
        /// Year Senior Two enrolled
        /// </summary>
        public string seniorTwo;
        /// <summary>
        /// Current structure (the same as seniorTwo)
        /// </summary>
        public string structureCurrent;
        public enum Event
        {
            /// <summary>
            /// 游园会 (from Apr 1 to Feb 15)
            /// </summary>
            YOUYUANHUI = 0,
            /// <summary>
            /// 十大 (from Feb 15 to Apr 1)
            /// </summary>
            SHIDA = 1,
        }
        /// <summary>
        /// Current event
        /// </summary>
        public Event eventCurrent;
        /// <summary>
        /// State constructor
        /// </summary>
        /// <param name="dt">Datetime</param>
        public State(DateTime dt)
        {
            string year = dt.Year.ToString();
            DateTime summerDivider = DateTime.Parse(year + "-8-1 00:00:00");
            DateTime winterDivider = DateTime.Parse(year + "-2-15 00:00:00");
            if (dt < winterDivider)
            {
                seniorOne = (dt.Year - 1).ToString();
                seniorTwo = (dt.Year - 2).ToString();
                structureCurrent = seniorTwo;
                eventCurrent = 0;
            }
            else if (dt > winterDivider && dt < summerDivider)
            {
                seniorOne = (dt.Year - 1).ToString();
                seniorTwo = (dt.Year - 2).ToString();
                structureCurrent = seniorTwo;
                eventCurrent = Event.SHIDA;
            }
            else// if (dt > summerDivider)
            {
                seniorOne = year;
                seniorTwo = (dt.Year - 1).ToString();
                structureCurrent = seniorTwo;
                eventCurrent = Event.YOUYUANHUI;
            }
        }
    }
}