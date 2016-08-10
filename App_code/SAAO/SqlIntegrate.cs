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
    public class SqlIntegrate : IDisposable
    {
        private readonly string _sqlConnStr;
        private SqlConnection _conn;
        private SqlCommand _cmd;
        private SqlDataReader _dr;
        private SqlDataAdapter _da;
        private DataSet _ds;
        private int _paraindex;
        private SqlParameter[] _paralist;
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
            _sqlConnStr = connstr;
        }
        /// <summary>
        /// Initialize parameter array
        /// </summary>
        /// <param name="paracount">Number of parameter(s)</param>
        public void InitParameter(int paracount)
        {
            _paraindex = 0;
            _paralist = new SqlParameter[paracount];
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
            _paralist[_paraindex] = para;
            _paraindex++;
        }
        /// <summary>
        /// Execute a SQL query with no return
        /// </summary>
        /// <param name="command">SQL query command</param>
        public void Execute(string command)
        {
            _conn = new SqlConnection(_sqlConnStr);
            _cmd = new SqlCommand(command, _conn);
            if (_paraindex != 0)
                foreach (var para in _paralist)
                    _cmd.Parameters.Add(para);
            _conn.Open();
            _cmd.ExecuteNonQuery();
            _conn.Close();
        }
        /// <summary>
        /// Execute a SQL query and return a single value
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>Query result (a single value)</returns>
        public object Query(string command)
        {
            _conn = new SqlConnection(_sqlConnStr);
            _cmd = new SqlCommand(command, _conn);
            if (_paraindex != 0)
                foreach (var para in _paralist)
                    _cmd.Parameters.Add(para);
            _conn.Open();
            var back = _cmd.ExecuteScalar();
            _conn.Close();
            if (!Convert.IsDBNull(back) && back != null)
                return back;
            return "";
        }
        /// <summary>
        /// Execute a SQL query and return a row of data
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A row of data</returns>
        public Dictionary<string, object> Reader(string command)
        {
            _conn = new SqlConnection(_sqlConnStr);
            _cmd = new SqlCommand(command, _conn);
            if (_paraindex != 0)
                foreach (var para in _paralist)
                    _cmd.Parameters.Add(para);
            _conn.Open();
            _dr = _cmd.ExecuteReader();
            if (!_dr.HasRows)
                throw new DataException();
            var cols = new List<string>();
            for (var i = 0; i < _dr.FieldCount; i++)
                cols.Add(_dr.GetName(i));
            Dictionary<string, object> result;
            if (_dr.Read())
                result = cols.ToDictionary(col => col, col => _dr[col]);
            else
                throw new DataException();
            _dr.Close();
            _conn.Close();
            return result;
        }
        /// <summary>
        /// Execute a SQL query and return a table of data
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A table of data</returns>
        public DataTable Adapter(string command)
        {
            _da = new SqlDataAdapter(command, _sqlConnStr);
            if (_paraindex != 0)
                foreach (var para in _paralist)
                    _da.SelectCommand.Parameters.Add(para);
            _ds = new DataSet();
            _da.Fill(_ds);
            var dt = _ds.Tables[0];
            return dt;
        }
        /// <summary>
        /// Execute a SQL query and return a row of data in JSON
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A row of data in JSON</returns>
        public JObject QueryJson(string command)
        {
            _conn = new SqlConnection(_sqlConnStr);
            _cmd = new SqlCommand(command, _conn);
            if (_paraindex != 0)
                foreach (var para in _paralist)
                    _cmd.Parameters.Add(para);
            _conn.Open();
            _dr = _cmd.ExecuteReader();
            if (!_dr.HasRows)
                throw new DataException();
            var cols = new List<string>();
            for (var i = 0; i < _dr.FieldCount; i++)
                cols.Add(_dr.GetName(i));
            if (_dr.Read())
                return (JObject) JToken.FromObject(cols.ToDictionary(col => col, col => _dr[col]));
            throw new DataException();
        }
        /// <summary>
        /// Execute a SQL query and return a table of data in JSON
        /// </summary>
        /// <param name="command">SQL query command</param>
        /// <returns>A table of data in JSON</returns>
        public JArray AdapterJson(string command)
        {
            _da = new SqlDataAdapter(command, _sqlConnStr);
            if (_paraindex != 0)
                foreach (var para in _paralist)
                    _da.SelectCommand.Parameters.Add(para);
            _ds = new DataSet();
            _da.Fill(_ds);
            var a = new JArray();
            foreach (DataRow dr in _ds.Tables[0].Rows)
            {
                var o = new JObject();
                foreach (DataColumn col in _ds.Tables[0].Columns)
                    o.Add(col.ColumnName.Trim(), JToken.FromObject(dr[col]));
                a.Add(o);
            }
            return a;
        }

        public void Dispose()
        {
            _conn.Dispose();
            _cmd.Dispose();
            _dr.Dispose();
            _da.Dispose();
        }
    }
}