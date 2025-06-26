using System;
using System.Data;
using System.Data.SQLite;


namespace MonteCarloApp.Forms
{
    public class DBHelper
    {
        private readonly string _connectionString;

        public DBHelper(string dbPath)
        {
            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Results (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        X0 REAL NOT NULL,
                        Y0 REAL NOT NULL,
                        Radius REAL NOT NULL,
                        C REAL NOT NULL,
                        ExactArea REAL NOT NULL,
                        MathArea REAL NOT NULL,
                        MonteCarloArea REAL NOT NULL,
                        ErrorPercent REAL NOT NULL,
                        PointCount REAL NOT NULL,
                        CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
                    )";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveResult(double x0, double y0, double radius, double c,
                             double exactArea, double mathArea, double monteCarloArea,
                             double pointCount, double errorPercent)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                    INSERT INTO Results 
                    (X0, Y0, Radius, C, ExactArea, MathArea, 
                     MonteCarloArea, PointCount, ErrorPercent)
                    VALUES (@x0, @y0, @radius, @c, @exactArea, 
                            @mathArea, @monteCarloArea, @pointCount, @errorPercent)";

                    command.Parameters.AddWithValue("@x0", x0);
                    command.Parameters.AddWithValue("@y0", y0);
                    command.Parameters.AddWithValue("@radius", radius);
                    command.Parameters.AddWithValue("@c", c);
                    command.Parameters.AddWithValue("@exactArea", exactArea);
                    command.Parameters.AddWithValue("@mathArea", mathArea);
                    command.Parameters.AddWithValue("@monteCarloArea", monteCarloArea);
                    command.Parameters.AddWithValue("@pointCount", pointCount);
                    command.Parameters.AddWithValue("@errorPercent", errorPercent);

                    command.ExecuteNonQuery();
                }
            }
        }

        public DataTable GetAllResults()
        {
            var dataTable = new DataTable();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM Results ORDER BY CreatedAt DESC", connection))
                using (var adapter = new SQLiteDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public void DeleteResult(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("DELETE FROM Results WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ClearAllResults()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("DELETE FROM Results", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}