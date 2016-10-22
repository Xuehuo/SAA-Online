using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SAAO
{
    /// <summary>
    /// SqlIntegrate 数据库交互
    /// </summary>
    public class SqlIntegrate
    {
        private readonly string _sqlConnStr;
        private List<SqlParameter> _para;
        /// <summary>
        /// SQL data type
        /// </summary>
        public enum DataType
        {
            Date = SqlDbType.Date,
            DateTime = SqlDbType.DateTime,
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
            _sqlConnStr = connstr;
            _para = new List<SqlParameter>();
        }
        /// <summary>
        /// Reset parameter list
        /// </summary>
        public void ResetParameter()
        {
            _para = new List<SqlParameter>();
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
            var para = ((int)datatype != (int)SqlDbType.Text || length == 0) ? new SqlParameter(key, (SqlDbType)datatype, length) : new SqlParameter(key, (SqlDbType)datatype);
            para.Value = value;
            _para.Add(para);
        }
        /// <summary>
        /// Execute a SQL query and return the number of rows affected
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <return>the number of rows affected</return>
        public int Execute(string command)
        {
            using (var conn = new SqlConnection(_sqlConnStr))
            {
                using (var cmd = new SqlCommand(command, conn))
                {
                    foreach (var para in _para)
                        cmd.Parameters.Add(para);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Execute a SQL query and return a single value
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>Query result (a single value)</returns>
        public object Query(string command)
        {
            using (var conn = new SqlConnection(_sqlConnStr))
            {
                using (var cmd = new SqlCommand(command, conn))
                {
                    foreach (var para in _para)
                        cmd.Parameters.Add(para);
                    conn.Open();
                    var back = cmd.ExecuteScalar();
                    if (!Convert.IsDBNull(back) && back != null)
                        return back;
                    return null;
                }
            }
        }
        /// <summary>
        /// Execute a SQL query and return a row of data
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A row of data</returns>
        public Dictionary<string, object> Reader(string command)
        {
            using (var conn = new SqlConnection(_sqlConnStr))
            {
                using (var cmd = new SqlCommand(command, conn))
                {
                    foreach (var para in _para)
                        cmd.Parameters.Add(para);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                            return null;
                        var cols = new List<string>();
                        for (var i = 0; i < dr.FieldCount; i++)
                            cols.Add(dr.GetName(i));
                        while (dr.Read())
                            return cols.ToDictionary(col => col, col => dr[col]);
                        return null;
                    }
                }
            }
        }
        /// <summary>
        /// Execute a SQL query and return a table of data
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A table of data</returns>
        public DataTable Adapter(string command)
        {
            using (var da = new SqlDataAdapter(command, _sqlConnStr))
            {
                foreach (var para in _para)
                    da.SelectCommand.Parameters.Add(para);
                using (var ds = new DataSet())
                {
                    da.Fill(ds);
                    return ds.Tables[0];
                }
            }
        }
        /// <summary>
        /// Execute a SQL query and return a row of data in JSON
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A row of data in JSON</returns>
        public JObject QueryJson(string command)
        {
            using (var conn = new SqlConnection(_sqlConnStr))
            {
                using (var cmd = new SqlCommand(command, conn))
                {
                    foreach (var para in _para)
                        cmd.Parameters.Add(para);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                            return null;
                        var cols = new List<string>();
                        for (var i = 0; i < dr.FieldCount; i++)
                            cols.Add(dr.GetName(i));
                        if (dr.Read())
                            return (JObject)JToken.FromObject(cols.ToDictionary(col => col, col => dr[col]));
                        return null;
                    }
                }
            }
        }
        /// <summary>
        /// Execute a SQL query and return a table of data in JSON
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A table of data in JSON</returns>
        public JArray AdapterJson(string command)
        {
            using (var da = new SqlDataAdapter(command, _sqlConnStr))
            {
                foreach (var para in _para)
                    da.SelectCommand.Parameters.Add(para);
                using (var ds = new DataSet())
                {
                    da.Fill(ds);
                    var a = new JArray();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var o = new JObject();
                        foreach (DataColumn col in ds.Tables[0].Columns)
                            o.Add(col.ColumnName.Trim(), JToken.FromObject(dr[col]));
                        a.Add(o);
                    }
                    return a;
                }
            }
        }
    }
}