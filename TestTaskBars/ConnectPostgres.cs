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
        public DataTable GetTable(DataTable table)
        {
            DataTable test_table = new DataTable();
            string connection_params = ConfigurationManager.ConnectionStrings["LocalServer"].ConnectionString;
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
            NpgsqlConnection connection = new NpgsqlConnection(connection_params);
            NpgsqlCommand execute_command = new NpgsqlCommand(sql_request, connection);
            connection.Open();
            NpgsqlDataReader reader;
            reader = execute_command.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    DataRow current_row = table.NewRow();
                    current_row[0] = "LocalServer";                    
                    current_row[1] = reader.GetString(0);
                    current_row[2] = reader.GetValue(1);
                    current_row[3] = reader.GetString(2);
                    current_row[4] = DateTime.Now;
                    table.Rows.Add(current_row);
                }
                catch { }
            }
            connection.Close();
            return table;
        }
    }
}
