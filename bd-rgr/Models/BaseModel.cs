using System;
using System.Collections.Generic;
using Npgsql;

namespace bd_rgr
{
    public class Connection
    {
        private NpgsqlConnection connection;
        public NpgsqlCommand Cmd;
        
        public Connection(string host, string db, string user, string password)
        {
            connection = new NpgsqlConnection(
                connectionString: $"Server={host};Port=5432;User Id={user};Password={password};Database={db};"
            );
            Cmd = new NpgsqlCommand();
            Cmd.Connection = connection;
        }
    }
    
    public abstract class BaseModel
    {
        protected readonly Connection Connection;
        public string TableName;
        public List<string> TableFields;
        
        public delegate void SetDateAction(ref Dictionary<string, object> item1, ref NpgsqlDataReader item2);

        public abstract void GenerateSeries(uint count);
        
        private void SetParameters(ref Dictionary<string, object> row, ref NpgsqlDataReader reader)
        {
            for (int i = 0; i < TableFields.Count; i++)
            {
                row[TableFields[i]] = reader.GetFieldValue<object>(i);
            }
        }
        
        public BaseModel(Connection connection)
        {
            Connection = connection;
        }

        public static string GetFormatValues<T>(in List<T> values, string format = ",%s", string format_start = "%s")
        {
            if (values.Count == 0) return string.Empty;
            string result = format_start.Replace("%s", values[0].ToString());

            for (int i = 1; i < values.Count; i++)
            {
                result += format.Replace("%s", values[i].ToString());
            }

            return result;
        }

        public static string GetFormatValues<T1, T2>(in List<Tuple<T1, T2>> values, string format = ",%s1=%s2", string format_start = "%s1=%s2")
        {
            if (values.Count == 0) return string.Empty;
            string result = format_start.Replace("%s1", values[0].Item1.ToString())
                .Replace("%s2", values[0].Item2.ToString());

            for (int i = 1; i < values.Count; i++)
            {
                result += format.Replace("%s1", values[i].Item1.ToString())
                    .Replace("%s2", values[i].Item2.ToString());
            }

            return result;
        }

        public static string GetFormatValues<T1, T2, T3>(in List<Tuple<T1, T2, T3>> values, string format = ",%s1.%s2=%s3",
            string format_start = "%s1.%s2=%s3")
        {
            if (values.Count == 0) return string.Empty;
            string result = format_start.Replace("%s1", values[0].Item1.ToString())
                .Replace("%s2", values[0].Item2.ToString())
                .Replace("%s3", values[0].Item3.ToString());

            for (int i = 1; i < values.Count; i++)
            {
                result += format.Replace("%s1", values[i].Item1.ToString())
                    .Replace("%s2", values[i].Item2.ToString())
                    .Replace("%s3", values[i].Item3.ToString());
            }

            return result;
        }

        public static string GetFormatValues<T1, T2, T3, T4>(in List<Tuple<T1, T2, T3, T4>> values, string format = ",%s1.%s2=%s3.%s4",
            string format_start = "%s1.%s2=%s3.%s4")
        {
            if (values.Count == 0) return string.Empty;
            string result = format_start.Replace("%s1", values[0].Item1.ToString())
                .Replace("%s2", values[0].Item2.ToString())
                .Replace("%s3", values[0].Item3.ToString())
                .Replace("%s4", values[0].Item4.ToString());

            for (int i = 1; i < values.Count; i++)
            {
                result += format.Replace("%s1", values[i].Item1.ToString())
                    .Replace("%s2", values[i].Item2.ToString())
                    .Replace("%s3", values[i].Item3.ToString())
                    .Replace("%s4", values[i].Item4.ToString());
            }

            return result;
        }

        public static string GetFormatValues(in List<DateTime> values, string format = ",%s", string format_start = "%s")
        {
            if (values.Count == 0) return string.Empty;
            string result = format_start.Replace("%s", values[0].ToString("yyyy-MM-dd"));

            for (int i = 1; i < values.Count; i++)
            {
                result += format.Replace("%s", values[i].ToString("yyyy-MM-dd"));
            }

            return result;
        }

        public static string GetFieldsFormat(in Dictionary<string, object> data, in List<string> fields,
            string format = ",%s", string format_start = "%s")
        {
            if (fields.Count == 0) return string.Empty;
            string result = format_start.Replace("%s", data[fields[0]].ToString());

            for (int i = 1; i < fields.Count; i++)
            {
                result += format.Replace("%s", data[fields[i]].ToString());
            }

            return result;
        }

        protected int GetMaxId()
        {
            int id = 1;
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"SELECT MAX({TableFields[0]}) FROM {TableName}";
                var reader = Connection.Cmd.ExecuteReader();
                while (reader.Read())
                {
                    id = reader.GetFieldValue<int>(0);
                }
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }

            return id;
        }

        public Dictionary<string, object> FindOne<T>(string column, T value)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"SELECT * FROM {TableName} WHERE {column}='{value}'";
                var reader = Connection.Cmd.ExecuteReader();
                while (reader.Read())
                {
                    SetParameters(ref result, ref reader);
                }
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"Given data: column({column}) value({value})");
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }

            return result;
        }
        
        public List<Dictionary<string, object>> FindSome<T>(string column, in List<T> values)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            string str_val = GetFormatValues(in values);
            
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"SELECT * FROM {TableName} WHERE {column} IN ({str_val})";
                var reader = Connection.Cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, object> patient = new Dictionary<string, object>();
                    SetParameters(ref patient, ref reader);
                    result.Add(patient);
                }
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"Given data: column({column}) values({str_val})");
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }

            return result;
        }
        
        public List<Dictionary<string, object>> FindSome(string column, in List<string> values)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            string str_val = GetFormatValues(in values, $" OR {column} LIKE '%s'", $"{column} LIKE '%s'");
            
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"SELECT * FROM {TableName} WHERE {str_val}";
                var reader = Connection.Cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, object> patient = new Dictionary<string, object>();
                    SetParameters(ref patient, ref reader);
                    result.Add(patient);
                }
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"Given data: column({column}) values({str_val})");
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }

            return result;
        }
        
        public List<Dictionary<string, object>> FindSome(string column, bool value)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"SELECT * FROM {TableName} WHERE {column}='{value}'";
                var reader = Connection.Cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, object> patient = new Dictionary<string, object>();
                    SetParameters(ref patient, ref reader);
                    result.Add(patient);
                }
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"Given data: column({column}) value({value})");
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }

            return result;
        }

        public List<Dictionary<string, object>> FindSome(string column, in List<DateTime> values)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            string str_val = GetFormatValues(in values);
            
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"SELECT * FROM {TableName} WHERE {column} IN ({str_val})";
                var reader = Connection.Cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, object> patient = new Dictionary<string, object>();
                    SetParameters(ref patient, ref reader);
                    result.Add(patient);
                }
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"Given data: column({column}) values({str_val})");
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }

            return result;
        }
        
        public List<Dictionary<string, object>> FindAll()
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"SELECT * FROM {TableName}";
                var reader = Connection.Cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, object> patient = new Dictionary<string, object>();
                    SetParameters(ref patient, ref reader);
                    result.Add(patient);
                }
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }

            return result;
        }
        
        public void AddOne(Dictionary<string, object> data)
        {
            var fields = GetFormatValues(TableFields);
            var values = GetFieldsFormat(in data, in TableFields, ", '%s'", "'%s'");
            
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"INSERT INTO {TableName}({fields})" +
                                              $" VALUES ({values})";
                Connection.Cmd.ExecuteNonQuery();
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }
        }
        
        public void AddSome(List<Dictionary<string, object>> data)
        {
            var fields = GetFormatValues(TableFields);
            
            foreach (var patient in data)
            {
                var values = GetFieldsFormat(in patient, in TableFields, ", '%s'", "'%s'");

                try
                {
                    Connection.Cmd.Connection.Open();
                    Connection.Cmd.CommandText = $"INSERT INTO {TableName}({fields})" +
                                                  $" VALUES ({fields})";
                    Connection.Cmd.ExecuteNonQuery();
                }
                catch (Npgsql.PostgresException er)
                {
                    Console.WriteLine($"{er.MessageText}");
                    Console.WriteLine($"{er.Hint}");
                }
                finally
                {
                    Connection.Cmd.Connection.Close();
                }
            }
        }
        
        public void RemoveSome<T>(string column, T value)
        {
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"DELETE FROM {TableName} WHERE {column}='{value}'";
                Connection.Cmd.ExecuteNonQuery();
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"Given data: column({column}) value({value})");
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }
        }
        
        public void RemoveSome<T>(string column, T value, bool greater)
        {
            string sign = "<";
            if (greater) sign = ">";
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"DELETE FROM {TableName} WHERE {column}{sign}'{value}'";
                Connection.Cmd.ExecuteNonQuery();
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"Given data: column({column}) value({value})");
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }
        }
        
        public void EditOne<T1, T2>(string column, T1 value, List<string> eColumn, List<T2> newValue)
        {
            var patient = FindOne(column, value);
            
            if(!(eColumn.Count > 0 && eColumn.Count == newValue.Count))
                return;
            string sets = "";
            int i;
            for(i = 0; i < eColumn.Count-1; i++)
            {
                sets += $"{eColumn[i]}='{newValue[i].ToString()}', ";
            }
            sets += $"{eColumn[i]}='{newValue[i].ToString()}'";
            
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"UPDATE {TableName} " +
                                              $"SET {sets} " +
                                              $"WHERE {TableFields[0]}={patient[TableFields[0]]}";
                Connection.Cmd.ExecuteNonQuery();
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"column({column}) value({value})");
                Console.WriteLine($"sets({sets})");
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }
        }
        
        public void EditSome<T1, T2>(string column, T1 value, List<string> eColumn, List<T2> newValue)
        {
            if(!(eColumn.Count > 0 && eColumn.Count == newValue.Count))
                return;
            string sets = "";
            int i;
            for(i = 0; i < eColumn.Count-1; i++)
            {
                sets += $"{eColumn[i]}='{newValue[i].ToString()}', ";
            }
            sets += $"{eColumn[i]}='{newValue[i].ToString()}'";
            
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = $"UPDATE {TableName} " +
                                              $"SET {sets} " +
                                              $"WHERE {column}='{value}'";
                Connection.Cmd.ExecuteNonQuery();
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"Given data: column({column}) value({value})");
                Console.WriteLine($"sets({sets})");
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">List of data of tables (table name, needed column, column for join)</param>
        /// <param name="where">List of data for WHERE (count of table in data, column name, list of values)</param>
        /// <typeparam name="T">The type of values for WHERE</typeparam>
        /// <returns></returns>
        public List<Dictionary<string, object>> FindSeveralTables<T>(List<Tuple<string, string, string>> data,
            Tuple<int, string, List<T>> where)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var select = new List<Tuple<string, string>>();
            var joins = new List<Tuple<string, string, string, string>>();
            var group_by = new List<Tuple<string, string>>();

            {
                int count = 0;
                foreach (var item in data)
                {
                    select.Add(new Tuple<string, string>($"t{count+1}", item.Item2));
                    var joins_t = $"t{count}";
                    if (count == 0) joins_t = "";
                    joins.Add(new Tuple<string, string, string, string>(item.Item1, $"t{count+1}", item.Item3, joins_t));
                    group_by.Add(new Tuple<string, string>($"t{count+1}", item.Item2));
                    count++;
                }
            }
            
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var select_str = GetFormatValues(select, ", %s1.%s2 AS %s2", "%s1.%s2 AS %s2");
            var from = GetFormatValues(joins, " JOIN %s1 AS %s2 ON %s2.%s3 = %s4.%s3", "%s1 AS %s2");
            var where_values = GetFormatValues(where.Item3);
            var str_groups = GetFormatValues(group_by, ", %s1.%s2", "%s1.%s2");

            var request = $"SELECT {select_str} " +
                          $"FROM {from} " +
                          $"WHERE {joins[where.Item1].Item2}.{where.Item2} in ({where_values}) " +
                          $"GROUP BY {str_groups}";
            
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = request;
                var reader = Connection.Cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    int i = 0;
                    foreach (var el in select)
                    {
                        row[el.Item2] = reader.GetFieldValue<object>(i);
                        i++;
                    }
                    result.Add(row);
                }
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }

            watch.Stop();

            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            return result;
        }
    }
}