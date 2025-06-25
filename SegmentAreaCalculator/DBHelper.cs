using System;
using System.Data.SQLite;
using Microsoft.Data.Sqlite;

public class DBHelper
{
    private string connectionString;

    public DBHelper(string dbPath)
    {
        connectionString = $"Data Source={dbPath};Version=3;";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string tableCmd = @"CREATE TABLE IF NOT EXISTS Results (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    X0 REAL,
                                    Y0 REAL,
                                    Radius REAL,
                                    C REAL,
                                    ExactArea REAL,
                                    MathArea REAL,
                                    MonteCarloArea REAL,
                                    ErrorPercent REAL,
                                    Date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                                );";
            using (var cmd = new SqliteCommand(tableCmd, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void SaveResult(double x0, double y0, double radius, double C,
                           double exactArea, double mathArea, double monteCarloArea, double errorPercent)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string insertCmd = @"INSERT INTO Results (X0, Y0, Radius, C, ExactArea, MathArea, MonteCarloArea, ErrorPercent)
                                 VALUES (@X0, @Y0, @Radius, @C, @ExactArea, @MathArea, @MonteCarloArea, @ErrorPercent);";
            using (var cmd = new SqliteCommand(insertCmd, connection))
            {
                cmd.Parameters.AddWithValue("@X0", x0);
                cmd.Parameters.AddWithValue("@Y0", y0);
                cmd.Parameters.AddWithValue("@Radius", radius);
                cmd.Parameters.AddWithValue("@C", C);
                cmd.Parameters.AddWithValue("@ExactArea", exactArea);
                cmd.Parameters.AddWithValue("@MathArea", mathArea);
                cmd.Parameters.AddWithValue("@MonteCarloArea", monteCarloArea);
                cmd.Parameters.AddWithValue("@ErrorPercent", errorPercent);
                cmd.ExecuteNonQuery();
            }
        }
    }
}