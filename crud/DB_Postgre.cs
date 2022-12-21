using System;
using System.Collections.Generic;
using Npgsql;
using System.Data;

namespace CRUD
{
    public class DB_Postgre : IDataBase<NpgsqlConnection>
    {
        private NpgsqlConnection connection;
        public NpgsqlConnection SetConnection {set{connection = value;} }

        public bool isConnected { get { return connection != null && connection.State == System.Data.ConnectionState.Open; } }

        public DB_Postgre()
        {
            connection = null;
        }

        public KeyValuePair<NpgsqlConnection, string> connect(Dictionary<string, string> settings)
        {
            Func<KeyValuePair<string, string>, string, bool> comparer = (x, s) => { return x.Key == s; };

            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(
                    $"Server={   settings["server"]   };" +
                    $"Port={     settings["port"]     };" +
                    $"User Id={  settings["login"]    };" +
                    $"Password={ settings["password"] };" +
                    $"Database={ settings["db_name"]  }"
                );

                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    return new KeyValuePair<NpgsqlConnection, string>(connection, "success");
                }

                return new KeyValuePair<NpgsqlConnection, string>(null, "failed");
            }
            catch (Exception error)
            {
                return new KeyValuePair<NpgsqlConnection, string>(null, error.Message);
            }
        }
        public KeyValuePair<DataTable, string> getQuery(string query)
        {
            if (isConnected)
            {
                try
                {
                    DataSet ds = new DataSet();
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(new NpgsqlCommand(query, connection));
                    adapter.Fill(ds);

                    return new KeyValuePair<DataTable, string>(ds.Tables[0], "Success");
                }
                catch (Exception error)
                {
                    return new KeyValuePair<DataTable, string>(null, error.Message);
                }
            }
            return new KeyValuePair<DataTable, string>(null, "Connection does not exist");
        }
        public string setQuery(string query)
        {
            return getQuery(query).Value;

        }
        public void disconnect()
        {
            if (isConnected)
            {
                connection.Close();
            }
        }

        public List<O> select<O>(string tableName, Predicate<O> predicate) where O : DB_Element<O>, new()
        {
            O static_object = new O();
            List<O> result = new List<O>();
            if (isConnected)
            {
                DataTable queryResult = getQuery($"SELECT * FROM {tableName}").Key;
                if (queryResult != null)
                {
                    foreach (DataRow row in queryResult.Rows)
                    {
                        O current_obj = static_object.asObject(row);
                        if (predicate(current_obj))
                        {
                            result.Add(current_obj);
                        }
                    }

                }

            }
            return result;
        }

        public void insert<O>(string tableName, List<O> data) where O : DB_Element<O>, new()
        {
            if (isConnected)
            {
                string composed_query = String.Empty;

                foreach (O element in data)
                {
                    composed_query += $"({element.asString()}),";
                }

                setQuery($"INSERT INTO {tableName} VALUES " + composed_query.Substring(0, composed_query.Length - 1));
            }
        }

        public void update<O>(string tableName, Predicate<O> predicate, Func<O, O> updateObject) where O : DB_Element<O>, new()
        {
            if (isConnected)
            {
                O static_object = new O();

                DataTable all_data = getQuery($"SELECT * FROM {tableName}").Key;

                if (all_data != null)
                {
                    setQuery($"DELETE FROM {tableName}");

                    List<O> updated_data = new List<O>();

                    foreach (DataRow row in all_data.Rows)
                    {
                        O current_obj = static_object.asObject(row);

                        if (predicate(current_obj))
                        {
                            current_obj = updateObject(current_obj);
                        }

                        updated_data.Add(current_obj);
                    }

                    insert<O>(tableName, updated_data);
                }
            }
        }

        public void remove<O>(string tableName, Predicate<O> predicate) where O : DB_Element<O>, new()
        {
            if (isConnected)
            {
                O static_object = new O();

                DataTable all_data = getQuery($"SELECT * FROM {tableName}").Key;

                if (all_data != null)
                {
                    setQuery($"DELETE FROM {tableName}");

                    List<O> filtered_data = new List<O>();

                    foreach (DataRow row in all_data.Rows)
                    {
                        O current_obj = static_object.asObject(row);
                        if (!predicate(current_obj))
                        {
                            filtered_data.Add(current_obj);
                        }
                    }

                    insert<O>(tableName, filtered_data);
                }

            }
        }

    }

}
