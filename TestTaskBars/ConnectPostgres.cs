using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace TestTaskBars
{
    class ConnectPostgres
    {
        public DataTable GetTable(DataTable table)
        {
            string result = "";
            DataTable test_table = new DataTable();
            string conn_params = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=1234;Database=postgres;";
            string sql = "SELECT pg_database.datname, pg_size_pretty(pg_database_size(pg_database.datname)) AS size FROM pg_database;";
            string sql2 = @"SELECT pg_database.datname
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
            NpgsqlConnection con = new NpgsqlConnection(conn_params);
            NpgsqlCommand com = new NpgsqlCommand(sql2, con);
            con.Open();
            NpgsqlDataReader reader;
            reader = com.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    DataRow current_row = table.NewRow();
                    current_row[0] = "Server1";
                    Console.WriteLine(reader.GetString(0));
                    current_row[1] = reader.GetString(0);
                    Console.WriteLine(reader.GetValue(1));
                    current_row[2] = reader.GetValue(1);
                    Console.WriteLine(reader.GetString(2));
                    current_row[3] = reader.GetString(2);
                    current_row[4] = DateTime.Now;
                    table.Rows.Add(current_row);
                }
                catch { }
            }
            con.Close();
            Console.WriteLine(result);
            return table;
        }
    }
}
