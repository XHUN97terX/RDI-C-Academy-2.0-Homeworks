using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDI_Homework_5.Utils
{
    class MyProvider : IDisposable
    {
        DbConnection server;

        public MyProvider(string connectionString)
        {
            server = new SqlConnection(connectionString);
            server.Open();
        }

        public void Dispose()
        {
            server.Close();
        }

        DbCommand InitCommand(string sql, Dictionary<string, string> parameters)
        {
            var cmd = new SqlCommand(sql, server as SqlConnection);
            if (parameters != null)
            {
                foreach (var k in parameters)
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = k.Key;
                    param.Value = k.Value;
                    cmd.Parameters.Add(param);
                }
            }
            return cmd;
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, string> parameters = null)
        {
            return InitCommand(sql, parameters).ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql, Dictionary<string, string> parameters = null)
        {
            return InitCommand(sql, parameters).ExecuteScalar();
        }

        public MyResult ExecuteSelect(string sql, Dictionary<string, string> parameters = null)
        {
            var cmd = InitCommand(sql, parameters);
            var dbReader = cmd.ExecuteReader();
            var result = new MyResult();
            string[] fields = null;
            object[] values = null;

            while (dbReader.Read())
            {
                if (fields == null)
                {
                    fields = new string[dbReader.FieldCount];
                    values = new object[dbReader.FieldCount];
                    for (int i = 0; i < dbReader.FieldCount; i++)
                        fields[i] = dbReader.GetName(i).ToUpper();
                }
                dbReader.GetValues(values);
                result.AddRow(fields, values);
            }
            if (dbReader != null && !dbReader.IsClosed)
                dbReader.Close();
            return result;
        }
    }
}
