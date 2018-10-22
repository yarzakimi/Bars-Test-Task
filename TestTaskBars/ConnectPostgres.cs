using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace TestTaskBars
{
    class ConnectPostgres
    {
        private readonly string _serverName;
        private readonly string _connectionParams;
        
        public ConnectPostgres(string serverName, string connectionParams)
        {
            this._serverName = serverName;
            this._connectionParams = connectionParams;
        }        
        public IList<IList<Object>> GetTable()
        {
            List<IList<Object>> table = new List<IList<Object>>();
            DataTable test_table = new DataTable();            
            string sql_request = @"SELECT pg_database.datname
                                        AS database_name,
                                    pg_database_size(pg_database.datname)
                                        AS database_size_bytes                                    
                            FROM pg_database
                            UNION ALL
                            SELECT 'Свободно'
                                        AS database_name,
                                    sum(pg_database_size(pg_database.datname))
                                        AS database_size_bytes
                            FROM pg_database;";            
            NpgsqlConnection connection = new NpgsqlConnection(this._connectionParams);
            NpgsqlCommand execute_command = new NpgsqlCommand(sql_request, connection);
            try
            {
                connection.Open();
                Console.WriteLine(DateTime.Now.ToString() + ": Successful connection to server {0}", this._serverName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": Failed connection to server {0}: {1}", this._serverName, ex.ToString());
            }
            NpgsqlDataReader reader;
            reader = execute_command.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    IList<Object> current_row = new List<Object>();
                    current_row.Add(this._serverName);
                    current_row.Add(reader.GetString(0));
                    current_row.Add(reader.GetValue(1));
                    current_row.Add(DateTime.Now.ToString("dd.MM.yyyy"));
                    table.Add(current_row);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString() + ": Error in processing sql-request: {0}", ex.ToString());
                }
            }
            connection.Close();
            return table;
        }
    }
}
