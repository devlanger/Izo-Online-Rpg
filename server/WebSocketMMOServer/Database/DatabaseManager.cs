using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace WebSocketMMOServer.Database
{
    public class DatabaseManager
    {
        private static string connectionString = "";

        public DatabaseManager()
        {
            string hostname = "127.0.0.1";
            string port = "3306";
            string database = "IzoMMO";
            string username = "root";
            string password = "";

            connectionString = string.Format("Server={0};Port={1};Database={2};User={3};Password={4};SslMode=none; convert zero datetime=True", hostname, port, database, username, password);
        }

        public static DataTable ReturnQuery(string query)
        {
            DataTable results = new DataTable("Result");
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand command = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        results.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(query + " /// Return query error: " + ex.ToString());
                }
            }

            return results;
        }

        public static long InsertQuery(string query)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand(query, conn);
                command.ExecuteNonQuery();
                return command.LastInsertedId;
            }
        }
    }
}
