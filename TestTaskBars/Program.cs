using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace TestTaskBars
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable size_server = new DataTable();
            size_server.Columns.Add("Server", typeof(string));
            size_server.Columns.Add("Database", typeof(string));
            size_server.Columns.Add("Size in bytes", typeof(long));
            size_server.Columns.Add("Size", typeof(string));
            size_server.Columns.Add("Update date", typeof(DateTime));
            ConnectPostgres test = new ConnectPostgres();
            DataTable server = new DataTable();
            server = test.GetTable(size_server);
            foreach (DataRow row in server.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write(item + "\t");
                }
                Console.WriteLine();
            }
           Console.Read();
        }
    }
}
