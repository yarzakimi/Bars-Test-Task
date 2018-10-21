using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using System.Configuration;

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
                                        AS database_size_bytes,
                                    pg_size_pretty(pg_database_size(pg_database.datname))
                                        AS database_size
                            FROM pg_database
                            UNION ALL
                            SELECT 'TOTAL'
                                        AS database_name,
                                    sum(pg_database_size(pg_database.datname))
                                        AS database_size_bytes,
                                    pg_size_pretty(sum(pg_database_size(pg_database.datname)))
                                        AS database_size
                            FROM pg_database
                            ORDER BY database_size_bytes ASC;";            
            NpgsqlConnection connection = new NpgsqlConnection(this._connectionParams);
            NpgsqlCommand execute_command = new NpgsqlCommand(sql_request, connection);
            connection.Open();
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
                    current_row.Add(reader.GetString(2));
                    current_row.Add(DateTime.Now.ToString("dd.MM.yyyy"));
                    table.Add(current_row);
                }
                catch { }
            }
            connection.Close();
            return table;
        }
    }
}
