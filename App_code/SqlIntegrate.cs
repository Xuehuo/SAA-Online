using System;
using System.Data;
using System.Data.SqlClient;
namespace SAAO
{
    /// <summary>
    /// SqlIntegrate is a basic SQL Database operating module.
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
        public enum DataType
        {
            Date = SqlDbType.Date,
            VarChar = SqlDbType.VarChar,
            NVarChar = SqlDbType.NVarChar,
            Text = SqlDbType.Text,
            Int = SqlDbType.Int,
        };
        public SqlIntegrate(string connstr)
        {
            sqlConnStr = connstr;
        }
        public void InitParameter(int paracount)
        {
            paraindex = 0;
            paralist = new SqlParameter[paracount];
        }
        public void AddParameter(string key, DataType datatype, object value, int length = 0)
        {
            SqlParameter para = ((int)datatype != (int)SqlDbType.Text || length == 0) ? new SqlParameter(key, (SqlDbType)datatype, length) : new SqlParameter(key, (SqlDbType)datatype);
            para.Value = value;
            paralist[paraindex] = para;
            paraindex++;
        }
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