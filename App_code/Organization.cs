using System;
using System.Data;
namespace SAAO
{
    /// <summary>
    /// Organization 架构
    /// </summary>
    public class Organization
    {
        public static int[] IMPT_MEMBER = new int[] { 0, 1, 2, 3 };
        public State state;
        public DataTable structure;
        public Organization(DateTime dt)
        {
            state = new State(dt);
            structure = new SqlIntegrate(Utility.connStr).Adapter("SELECT * FROM [Org] WHERE [year]='" + state.structureCurrent + state.eventCurrent.ToString() + "'");
        }
        public static Organization Current
        {
            get
            {
                return (Organization)System.Web.HttpContext.Current.Application["org"];
            }
        }
        public string GetGroupColor(int i)
        {
            return structure.Select("[group]=" + i)[0]["color"].ToString();
        }
        public string GetGroupName(int i)
        {
            return structure.Select("[group]=" + i)[0]["name"].ToString();
        }
        public string GetJobName(int i)
        {
            return structure.Select("[job]=" + i)[0]["name"].ToString();
        }
    }
    public struct State
    {
        public string seniorOne;   //当前高一的届数
        public string seniorTwo;   //当前高二的届数
        public string structureCurrent;    //当前使用哪一届的架构(高二的学号前4位)
        public int eventCurrent;   //正在筹备的活动：0-游园会，1-十大;均以每年二月十五日和八月一日零点为界
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
                eventCurrent = 1;
            }
            else if (dt > summerDivider)
            {
                seniorOne = year;
                seniorTwo = (dt.Year - 1).ToString();
                structureCurrent = seniorTwo;
                eventCurrent = 0;
            }
            else
            {
                Utility.Log("请更新架构");
                seniorOne = "";
                seniorTwo = "";
                structureCurrent = "";
                eventCurrent = 0;
            }
        }
    }
}