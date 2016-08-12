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
        public static int[] ImptMember = { 0, 1, 2, 3 };
        // TODO: Rename or write it in database

        public State State;

        /// <summary>
        /// Structure table
        /// </summary>
        public DataTable Structure;
        /// <summary>
        /// Organization structure constructor
        /// </summary>
        /// <param name="dt">Datetime (DateTime.Now most possibly)</param>
        public Organization(DateTime dt)
        {
            State = new State(dt);
            var si = new SqlIntegrate(Utility.ConnStr);
            si.AddParameter("@year", SqlIntegrate.DataType.VarChar, State.StructureCurrent + (int) State.EventCurrent);
            Structure = si.Adapter("SELECT * FROM [Org] WHERE [year] = @year");
        }
        /// <summary>
        /// Current organization structure
        /// </summary>
        public static Organization Current => (Organization)System.Web.HttpContext.Current.Application["org"];

        /// <summary>
        /// Get color of the group
        /// </summary>
        /// <param name="i">Group index</param>
        /// <returns>Group color</returns>
        public string GetGroupColor(int i)
        {
            return Structure.Select("[group]=" + i)[0]["color"].ToString();
        }
        /// <summary>
        /// Get group name
        /// </summary>
        /// <param name="i">Group index</param>
        /// <returns>Group name</returns>
        public string GetGroupName(int i)
        {
            return Structure.Select("[group]=" + i)[0]["name"].ToString();
        }
        /// <summary>
        /// Get job (title) name
        /// </summary>
        /// <param name="i">Job index</param>
        /// <returns>Job name</returns>
        public string GetJobName(int i)
        {
            return Structure.Select("[job]=" + i)[0]["name"].ToString();
        }
    }
    public struct State
    {
        /// <summary>
        /// Year Senior One enrolled
        /// </summary>
        public string SeniorOne;
        /// <summary>
        /// Year Senior Two enrolled
        /// </summary>
        public string SeniorTwo;
        /// <summary>
        /// Current structure (the same as seniorTwo)
        /// </summary>
        public string StructureCurrent;
        public enum Event
        {
            /// <summary>
            /// 游园会 (from Apr 1 to Feb 15)
            /// </summary>
            YouYuanHui = 0,
            /// <summary>
            /// 十大 (from Feb 15 to Apr 1)
            /// </summary>
            ShiDa = 1,
        }
        /// <summary>
        /// Current event
        /// </summary>
        public Event EventCurrent;
        /// <summary>
        /// State constructor
        /// </summary>
        /// <param name="dt">Datetime</param>
        public State(DateTime dt)
        {
            var year = dt.Year.ToString();
            var summerDivider = DateTime.Parse(year + "-8-1 00:00:00");
            var winterDivider = DateTime.Parse(year + "-2-15 00:00:00");
            if (dt < winterDivider)
            {
                SeniorOne = (dt.Year - 1).ToString();
                SeniorTwo = (dt.Year - 2).ToString();
                StructureCurrent = SeniorTwo;
                EventCurrent = 0;
            }
            else if (dt > winterDivider && dt < summerDivider)
            {
                SeniorOne = (dt.Year - 1).ToString();
                SeniorTwo = (dt.Year - 2).ToString();
                StructureCurrent = SeniorTwo;
                EventCurrent = Event.ShiDa;
            }
            else// if (dt > summerDivider)
            {
                SeniorOne = year;
                SeniorTwo = (dt.Year - 1).ToString();
                StructureCurrent = SeniorTwo;
                EventCurrent = Event.YouYuanHui;
            }
        }
    }
}