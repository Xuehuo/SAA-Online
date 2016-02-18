using System;
using System.Data;
using System.Data.SqlClient;
namespace SAAO
{
    /// <summary>
    /// SqlIntegrate 数据库交互
    /// </summary>
    public class SqlIntegrate
    {
        private string sqlConnStr;
        private SqlConnection conn;
        private SqlCommand cmd;
        private SqlDataReader dr;
        private SqlDataAdapter da;
        private DataSet ds;
        private int paraindex = 0;
        private SqlParameter[] paralist;
        /// <summary>
        /// SQL data type
        /// </summary>
        public enum DataType
        {
            Date = SqlDbType.Date,
            VarChar = SqlDbType.VarChar,
            NVarChar = SqlDbType.NVarChar,
            Text = SqlDbType.Text,
            Int = SqlDbType.Int,
        };
        /// <summary>
        /// SqlIntegrate constructor
        /// </summary>
        /// <param name="connstr">Connection string</param>
        public SqlIntegrate(string connstr)
        {
            sqlConnStr = connstr;
        }
        /// <summary>
        /// Initialize parameter array
        /// </summary>
        /// <param name="paracount">Number of parameter(s)</param>
        public void InitParameter(int paracount)
        {
            paraindex = 0;
            paralist = new SqlParameter[paracount];
        }
        /// <summary>
        /// Add a parameter
        /// </summary>
        /// <param name="key">Parameter name (with at '@')</param>
        /// <param name="datatype">SQL data type</param>
        /// <param name="value">Parameter value</param>
        /// <param name="length">Max length (if necessary)</param>
        public void AddParameter(string key, DataType datatype, object value, int length = 0)
        {
            SqlParameter para = ((int)datatype != (int)SqlDbType.Text || length == 0) ? new SqlParameter(key, (SqlDbType)datatype, length) : new SqlParameter(key, (SqlDbType)datatype);
            para.Value = value;
            paralist[paraindex] = para;
            paraindex++;
        }
        /// <summary>
        /// Execute a SQL query with no return
        /// </summary>
        /// <param name="command">SQL query command</param>
        public void Execute(string command)
        {
            conn = new SqlConnection(sqlConnStr);
            cmd = new SqlCommand(command, conn);
            if (paraindex != 0)
                for (int i = 0; i < paralist.Length; i++)
                    cmd.Parameters.Add(paralist[i]);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        /// <summary>
        /// Execute a SQL query and return a single value
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>Query result (a single value)</returns>
        public object Query(string command)
        {
            object back = null;
            conn = new SqlConnection(sqlConnStr);
            cmd = new SqlCommand(command, conn);
            if (paraindex != 0)
                for (int i = 0; i < paralist.Length; i++)
                    cmd.Parameters.Add(paralist[i]);
            conn.Open();
            back = cmd.ExecuteScalar();
            conn.Close();
            if (!Convert.IsDBNull(back) && back != null)
                return back;
            else
                return "";
        }
        /// <summary>
        /// Execute a SQL query and return a row of data
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A row of data</returns>
        public DataRow Reader(string command)
        {
            conn = new SqlConnection(sqlConnStr);
            cmd = new SqlCommand(command, conn);
            if (paraindex != 0)
                for (int i = 0; i < paralist.Length; i++)
                    cmd.Parameters.Add(paralist[i]);
            conn.Open();
            dr = cmd.ExecuteReader();
            if (!dr.HasRows)
                throw new DataException();
            DataRow datarow = GetDataRow(dr);
            dr.Close();
            conn.Close();
            return datarow;
        }
        /// <summary>
        /// Convert DataReader to DataRow
        /// </summary>
        /// <param name="reader">A DataReader</param>
        /// <returns>A DataRow</returns>
        private static DataRow GetDataRow(SqlDataReader reader)
        {
            DataTable schemaTable = reader.GetSchemaTable();
            DataTable data = new DataTable();
            foreach (DataRow row in schemaTable.Rows)
            {
                string colName = row.Field<string>("ColumnName");
                Type t = row.Field<Type>("DataType");
                data.Columns.Add(colName, t);
            }
            while (reader.Read())
            {
                var newRow = data.Rows.Add();
                foreach (DataColumn col in data.Columns)
                {
                    newRow[col.ColumnName] = reader[col.ColumnName];
                }
            }
            return data.Rows[0];
        }
        // TODO: this method is low-efficient
        /// <summary>
        /// Execute a SQL query and return a table of data
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A table of data</returns>
        public DataTable Adapter(string command)
        {
            da = new SqlDataAdapter(command, sqlConnStr);
            if (paraindex != 0)
                for (int i = 0; i < paralist.Length; i++)
                    da.SelectCommand.Parameters.Add(paralist[i]);
            ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            return dt;
        }
        /// <summary>
        /// Execute a SQL query and return a row of data in JSON
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A row of data in JSON</returns>
        public string QueryJSON(string command)
        {
            string back = "{";
            conn = new SqlConnection(sqlConnStr);
            cmd = new SqlCommand(command, conn);
            if (paraindex != 0)
                for (int i = 0; i < paralist.Length; i++)
                    cmd.Parameters.Add(paralist[i]);
            conn.Open();
            dr = cmd.ExecuteReader();
            if (!dr.HasRows)
                throw new DataException();
            while (dr.Read())
                for (int i = 0; i < dr.FieldCount; i++)
                    if (dr[i].GetType().Equals(new byte().GetType()) || dr[i].GetType().Equals(new int().GetType()) || dr[i].GetType().Equals(new float().GetType()) || dr[i].GetType().Equals(new double().GetType()))
                        back += "\"" + dr.GetName(i) + "\":" + dr[i].ToString() + ",";
                    else if (dr[i].GetType().Equals(new DateTime().GetType()))
                        back += "\"" + dr.GetName(i) + "\":\"" + Convert.ToDateTime(dr[i]).ToString("yyyy-MM-dd HH:mm:ss").Replace(" 00:00:00", "") + "\",";
                    else
                        back += "\"" + dr.GetName(i) + "\":\"" + Utility.string2JSON(dr[i].ToString()) + "\",";
            back += "}";
            dr.Close();
            conn.Close();
            return back.Replace(",}", "}");
        }
        /// <summary>
        /// Execute a SQL query and return a table of data in JSON
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A table of data in JSON</returns>
        public string AdapterJSON(string command)
        {
            string back = "[";
            da = new SqlDataAdapter(command, sqlConnStr);
            if (paraindex != 0)
                for (int i = 0; i < paralist.Length; i++)
                    da.SelectCommand.Parameters.Add(paralist[i]);
            ds = new DataSet();
            da.Fill(ds);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                back += "{";
                for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                {
                    if (ds.Tables[0].Rows[i][j].GetType().Equals(new byte().GetType()) || ds.Tables[0].Rows[i][j].GetType().Equals(new int().GetType()) || ds.Tables[0].Rows[i][j].GetType().Equals(new float().GetType()) || ds.Tables[0].Rows[i][j].GetType().Equals(new double().GetType()))
                        back += "\"" + ds.Tables[0].Columns[j].ColumnName + "\":" + ds.Tables[0].Rows[i][j].ToString() + ",";
                    else if (ds.Tables[0].Rows[i][j].GetType().Equals(new DateTime().GetType()))
                        back += "\"" + ds.Tables[0].Columns[j].ColumnName + "\":\"" + Convert.ToDateTime(ds.Tables[0].Rows[i][j].ToString()).ToString("yyyy-MM-dd HH:mm:ss").Replace(" 00:00:00", "") + "\",";
                    else
                        back += "\"" + ds.Tables[0].Columns[j].ColumnName + "\":\"" + Utility.string2JSON(ds.Tables[0].Rows[i][j].ToString()) + "\",";
                }
                back += "},";
            }
            back += "]";
            return back.Replace(",}", "}").Replace(",]", "]");
        }
        
    }
}