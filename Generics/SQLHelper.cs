using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ArmyGrievances.Generics
{
    public static class SqlHelper
    {

        internal static DataSet ExecuteSet(string CommandName, CommandType cmdType, SqlParameter[]? param = null, IConfiguration? configuration = null)
        {

            DataSet? set = null;
            using (SqlConnection con = new SqlConnection(configuration?.GetConnectionString("DB_Connection")))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    if (param != null && param.Length > 0)
                        cmd.Parameters.AddRange(param);
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            set = new DataSet();
                            da.Fill(set);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return set;
        }

        internal static async Task<DataSet> ExecuteSetAsync(string CommandName, int TableCount, CommandType cmdType, SqlParameter[]? param = null, IConfiguration? configuration = null)
        {
            DataSet? set = null;
            using (SqlConnection con = new SqlConnection(configuration?.GetConnectionString("DB_Connection")))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    if (param != null && param.Length > 0)
                        cmd.Parameters.AddRange(param);
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            await con.OpenAsync();
                        }
                        SqlDataReader sdr = await cmd.ExecuteReaderAsync();
                        set = new DataSet();
                        string[] strTables = new string[TableCount];
                        for (int i = 0; i < TableCount; i++)
                        {
                            strTables[i] = string.Format("Table{0}", (i + 1));
                        }
                        set.Load(sdr, LoadOption.PreserveChanges, strTables);
                        cmd.Parameters.Clear();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return set;
        }

        internal static DataTable ExecuteTable(string CommandName, CommandType cmdType, SqlParameter[]? param = null, IConfiguration? configuration = null)
        {
            DataTable? table = null;
            using (SqlConnection con = new SqlConnection(configuration?.GetConnectionString("DB_Connection")))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    if (param != null && param.Length > 0)
                        cmd.Parameters.AddRange(param);
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            table = new DataTable();
                            da.Fill(table);
                        }
                        cmd.Parameters.Clear();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        internal static async Task<DataTable> ExecuteTableAsync(string CommandName, CommandType cmdType, SqlParameter[]? param = null, IConfiguration? configuration = null)
        {
            DataTable? table = null;
            using (SqlConnection con = new SqlConnection(configuration?.GetConnectionString("DB_Connection")))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    if (param != null && param.Length > 0)
                        cmd.Parameters.AddRange(param);
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            await con.OpenAsync();
                        }
                        SqlDataReader sdr = await cmd.ExecuteReaderAsync();
                        table = new DataTable();
                        table.Load(sdr);
                        cmd.Parameters.Clear();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return table;
        }

        internal static List<T> ExecuteList<T>(string CommandName, CommandType cmdType, SqlParameter[]? param = null, IConfiguration? configuration = null) where T : class, new()
        {
            List<T>? list = null;
            using (SqlConnection con = new SqlConnection(configuration?.GetConnectionString("DB_Connection")))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    if (param != null && param.Length > 0)
                        cmd.Parameters.AddRange(param);
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable table = new DataTable();
                            da.Fill(table);
                            list = new List<T>();
                            foreach (var row in table.AsEnumerable())
                            {
                                T obj = new T();
                                foreach (var prop in obj.GetType().GetProperties())
                                {
                                    try
                                    {
                                        PropertyInfo? propertyInfo = obj.GetType().GetProperty(prop.Name);
                                        propertyInfo?.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                list.Add(obj);
                            }
                            cmd.Parameters.Clear();
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return list;
        }

        internal static async Task<List<T>> ExecuteListAsync<T>(string CommandName, CommandType cmdType, SqlParameter[]? param = null, IConfiguration? configuration = null) where T : class, new()
        {
            List<T>? list = null;
            using (SqlConnection con = new SqlConnection(configuration?.GetConnectionString("DB_Connection")))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    if (param != null && param.Length > 0)
                        cmd.Parameters.AddRange(param);
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            await con.OpenAsync();
                        }
                        SqlDataReader sdr = await cmd.ExecuteReaderAsync();
                        DataTable table = new DataTable();
                        table.Load(sdr);
                        list = new List<T>();
                        foreach (var row in table.AsEnumerable())
                        {
                            T obj = new T();
                            foreach (var prop in obj.GetType().GetProperties())
                            {
                                try
                                {
                                    PropertyInfo? propertyInfo = obj.GetType().GetProperty(prop.Name);
                                    propertyInfo?.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                            list.Add(obj);
                        }
                        cmd.Parameters.Clear();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return list;
        }
        internal static async Task<bool> ExecuteNonQueryAsync(string CommandName, CommandType cmdType, SqlParameter[]? param = null, IConfiguration? configuration = null)
        {

            int result = 0;
            using (SqlConnection con = new SqlConnection(configuration?.GetConnectionString("DB_Connection")))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    if (param != null && param.Length > 0)
                        cmd.Parameters.AddRange(param);
                    cmd.CommandTimeout = cmd.Connection.ConnectionTimeout;
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            await con.OpenAsync();
                        }
                        result = await cmd.ExecuteNonQueryAsync();
                        cmd.Parameters.Clear();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            return (result > 0 || result == -1);
        }


        internal static async Task<int> ExecuteScalerAsync(string CommandName, CommandType cmdType, SqlParameter[]? param = null, IConfiguration? configuration = null)
        {
            int result = 0;
            using (SqlConnection con = new SqlConnection(configuration?.GetConnectionString("DB_Connection")))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    if (param != null && param.Length > 0)
                        cmd.Parameters.AddRange(param);
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            await con.OpenAsync();
                        }
                        result = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        cmd.Parameters.Clear();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return result;
        }
        internal static async Task<string?> ExecuteScalerStrAsync(string CommandName, CommandType cmdType, SqlParameter[]? param = null, IConfiguration? configuration = null)
        {
            string? result = "";
            using (SqlConnection con = new SqlConnection(configuration?.GetConnectionString("DB_Connection")))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    if (param != null && param.Length > 0)
                        cmd.Parameters.AddRange(param);
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            await con.OpenAsync();
                        }
                        result = Convert.ToString(await cmd.ExecuteScalarAsync());
                        cmd.Parameters.Clear();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return result;
        }

    }
}
