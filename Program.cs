using Npgsql;
using Microsoft.Data.SqlClient;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string pgConnStr = "Host=localhost;Port=5432;Database=YourDBName;Username=YourUN;Password=YourPass;";
            string sqlConnStr = "Server=localhost\\SQLEXPRESS;Database=YourDBName;Trusted_Connection=True;TrustServerCertificate=True;";

            using var pgConn = new NpgsqlConnection(pgConnStr);
            pgConn.Open();

            using var pgCmd = new NpgsqlCommand("SELECT first_name, last_name FROM public.actor", pgConn);
            using var reader = pgCmd.ExecuteReader();

            using var sqlConn = new SqlConnection(sqlConnStr);
            sqlConn.Open();

            using var transaction = sqlConn.BeginTransaction();

            while (reader.Read())
            {
                string firstName = reader.GetString(0);
                string lastName = reader.GetString(1);

                using var insertCmd = new SqlCommand("INSERT INTO dbo.actor (first_name, last_name) VALUES (@first, @last)", 
                    sqlConn, transaction);
                insertCmd.CommandTimeout = 0;

                insertCmd.Parameters.AddWithValue("@first", firstName);
                insertCmd.Parameters.AddWithValue("@last", lastName);
                insertCmd.ExecuteNonQuery();
            }

            transaction.Commit();

        }
    }
}
