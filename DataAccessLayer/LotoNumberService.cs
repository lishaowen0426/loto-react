// DataAccessLayer/LotoNumberService.cs
using Loto.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;


public class LotoNumberService
{
    private readonly string _connectionString;

    public LotoNumberService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

   
    public List<LotoNumber> GetLotoNumbersByDateRange(DateTime startDate, DateTime endDate)
    {
        var lotoNumbers = new List<LotoNumber>();
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM LotoNumbers WHERE DrawDate BETWEEN @StartDate AND @EndDate ORDER BY DrawDate DESC", connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var lotoNumber = new LotoNumber
                    {
                        Issue = reader.GetString(reader.GetOrdinal("Issue")),
                        DrawDate = reader.GetDateTime(reader.GetOrdinal("DrawDate")),
                        Number1 = reader.GetInt32(reader.GetOrdinal("Number1")),
                        Number2 = reader.GetInt32(reader.GetOrdinal("Number2")),
                        Number3 = reader.GetInt32(reader.GetOrdinal("Number3")),
                        Number4 = reader.GetInt32(reader.GetOrdinal("Number4")),
                        Number5 = reader.GetInt32(reader.GetOrdinal("Number5")),
                        Number6 = reader.GetInt32(reader.GetOrdinal("Number6")),
                        Number7 = reader.GetInt32(reader.GetOrdinal("Number7")),
                        BonusNumber1 = reader.IsDBNull(reader.GetOrdinal("BonusNumber1")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("BonusNumber1")),
                        BonusNumber2 = reader.IsDBNull(reader.GetOrdinal("BonusNumber2")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("BonusNumber2")),
                        Mouths = reader.IsDBNull(reader.GetOrdinal("Mouths")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mouths")),
                        Prize = reader.IsDBNull(reader.GetOrdinal("Prize")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Prize")),
                        CarryOver = reader.IsDBNull(reader.GetOrdinal("CarryOver")) ? 0 : reader.GetDecimal(reader.GetOrdinal("CarryOver"))
                    };
                    lotoNumbers.Add(lotoNumber);
                }
            }
        }

        return lotoNumbers;
    }
    public List<LotoNumber> GetAllLotoNumbers()
    {
        var lotoNumbers = new List<LotoNumber>();
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM LotoNumbers ORDER BY DrawDate DESC", connection);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var lotoNumber = new LotoNumber
                    {
                        Issue = reader.GetString(reader.GetOrdinal("Issue")),
                        DrawDate = reader.GetDateTime(reader.GetOrdinal("DrawDate")),
                        Number1 = reader.GetInt32(reader.GetOrdinal("Number1")),
                        Number2 = reader.GetInt32(reader.GetOrdinal("Number2")),
                        Number3 = reader.GetInt32(reader.GetOrdinal("Number3")),
                        Number4 = reader.GetInt32(reader.GetOrdinal("Number4")),
                        Number5 = reader.GetInt32(reader.GetOrdinal("Number5")),
                        Number6 = reader.GetInt32(reader.GetOrdinal("Number6")),
                        Number7 = reader.GetInt32(reader.GetOrdinal("Number7")),
                        BonusNumber1 = reader.IsDBNull(reader.GetOrdinal("BonusNumber1")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("BonusNumber1")),
                        BonusNumber2 = reader.IsDBNull(reader.GetOrdinal("BonusNumber2")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("BonusNumber2")),
                        Mouths = reader.IsDBNull(reader.GetOrdinal("Mouths")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mouths")),
                        Prize =
                        reader.IsDBNull(reader.GetOrdinal("Prize")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Prize")),
                        CarryOver = reader.IsDBNull(reader.GetOrdinal("CarryOver")) ? 0 : reader.GetDecimal(reader.GetOrdinal("CarryOver"))
                    };
                    lotoNumbers.Add(lotoNumber);
                }
            }
        }

        return lotoNumbers;
    }
    public List<LotoNumber> GetAllLotoNumbers(int pageNumber, int pageSize)
    {
        var lotoNumbers = new List<LotoNumber>();

        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            var commandText = "SELECT * FROM LotoNumbers LIMIT @PageSize OFFSET @Offset";
            var command = new SQLiteCommand(commandText, connection);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var lotoNumber = new LotoNumber
                    {
                        Issue = reader.GetString(reader.GetOrdinal("Issue")),
                        DrawDate = DateTime.ParseExact(reader.GetString(reader.GetOrdinal("DrawDate")), "yyyy/M/d", CultureInfo.InvariantCulture),
                        Number1 = reader.GetInt32(reader.GetOrdinal("Number1")),
                        Number2 = reader.GetInt32(reader.GetOrdinal("Number2")),
                        Number3 = reader.GetInt32(reader.GetOrdinal("Number3")),
                        Number4 = reader.GetInt32(reader.GetOrdinal("Number4")),
                        Number5 = reader.GetInt32(reader.GetOrdinal("Number5")),
                        Number6 = reader.GetInt32(reader.GetOrdinal("Number6")),
                        Number7 = reader.GetInt32(reader.GetOrdinal("Number7")),
                        BonusNumber1 = reader.IsDBNull(reader.GetOrdinal("BonusNumber1")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("BonusNumber1")),
                        BonusNumber2 = reader.IsDBNull(reader.GetOrdinal("BonusNumber2")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("BonusNumber2")),
                        Mouths = reader.IsDBNull(reader.GetOrdinal("Mouths")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mouths")),
                        Prize = reader.IsDBNull(reader.GetOrdinal("Prize")) ? 0 :
                            decimal.Parse(reader.GetString(reader.GetOrdinal("Prize")).Replace(",", ""), CultureInfo.InvariantCulture),
                        CarryOver = reader.IsDBNull(reader.GetOrdinal("CarryOver")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("CarryOver"))
                    };
                    lotoNumbers.Add(lotoNumber);
                }
            }
        }

        return lotoNumbers;
    }
    public List<NumberOccurrence> GetNumberOccurrences()
    {
        var occurrences = new List<NumberOccurrence>();


        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var commandText = @"
        SELECT Number, COUNT(Number) AS Occurrences
        FROM (
            SELECT Number1 AS Number FROM LotoNumbers
            UNION ALL
            SELECT Number2 FROM LotoNumbers
            UNION ALL
            SELECT Number3 FROM LotoNumbers
            UNION ALL
            SELECT Number4 FROM LotoNumbers
            UNION ALL
            SELECT Number5 FROM LotoNumbers
            UNION ALL
            SELECT Number6 FROM LotoNumbers
            UNION ALL
            SELECT Number7 FROM LotoNumbers
        ) AS CombinedNumbers
        GROUP BY Number
        ORDER BY Number;
        ";

            using (var command = new MySqlCommand(commandText, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var numberOccurrence = new NumberOccurrence
                        {
                            Number = reader.GetInt32(reader.GetOrdinal("Number")),
                            Occurrences = reader.GetInt32(reader.GetOrdinal("Occurrences"))
                        };
                        occurrences.Add(numberOccurrence);
                    }
                }
            }
        }

        return occurrences;
    }

    public string GetLatestIssue()
    {
        string latestIssue = string.Empty;
      

        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var commandText = @"
        SELECT Issue
        FROM LotoNumbers
        ORDER BY DrawDate DESC
        LIMIT 1;
        ";

            using (var command = new MySqlCommand(commandText, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Assuming 'Issue' is stored as text
                        latestIssue = reader.GetString(reader.GetOrdinal("Issue"));
                    }
                }
            }
        }

        return latestIssue;
    }

    public void InsertLotoNumber(LotoNumber lotoNumber)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var commandText = @"
        INSERT INTO LotoNumbers (Issue, DrawDate, Number1, Number2, Number3, Number4, Number5, Number6, Number7, BonusNumber1, BonusNumber2, Mouths, Prize, CarryOver, SecondPrizeMouths, SecondPrizeAmount, ThirdPrizeMouths, ThirdPrizeAmount, FourthPrizeMouths, FourthPrizeAmount, FifthPrizeMouths, FifthPrizeAmount, SixthPrizeMouths, SixthPrizeAmount, TotalSales)
        VALUES (@Issue, @DrawDate, @Number1, @Number2, @Number3, @Number4, @Number5, @Number6, @Number7, @BonusNumber1, @BonusNumber2, @Mouths, @Prize, @CarryOver, @SecondPrizeMouths, @SecondPrizeAmount, @ThirdPrizeMouths, @ThirdPrizeAmount, @FourthPrizeMouths, @FourthPrizeAmount, @FifthPrizeMouths, @FifthPrizeAmount, @SixthPrizeMouths, @SixthPrizeAmount, @TotalSales);
        ";

            using (var command = new MySqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Issue", lotoNumber.Issue);
                command.Parameters.AddWithValue("@DrawDate", lotoNumber.DrawDate);
                command.Parameters.AddWithValue("@Number1", lotoNumber.Number1);
                command.Parameters.AddWithValue("@Number2", lotoNumber.Number2);
                command.Parameters.AddWithValue("@Number3", lotoNumber.Number3);
                command.Parameters.AddWithValue("@Number4", lotoNumber.Number4);
                command.Parameters.AddWithValue("@Number5", lotoNumber.Number5);
                command.Parameters.AddWithValue("@Number6", lotoNumber.Number6);
                command.Parameters.AddWithValue("@Number7", lotoNumber.Number7);
                command.Parameters.AddWithValue("@BonusNumber1", lotoNumber.BonusNumber1 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@BonusNumber2", lotoNumber.BonusNumber2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Mouths", lotoNumber.Mouths);
                command.Parameters.AddWithValue("@Prize", lotoNumber.Prize);
                command.Parameters.AddWithValue("@CarryOver", lotoNumber.CarryOver ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SecondPrizeMouths", lotoNumber.SecondPrizeMouths);
                command.Parameters.AddWithValue("@SecondPrizeAmount", lotoNumber.SecondPrizeAmount);
                command.Parameters.AddWithValue("@ThirdPrizeMouths", lotoNumber.ThirdPrizeMouths);
                command.Parameters.AddWithValue("@ThirdPrizeAmount", lotoNumber.ThirdPrizeAmount);
                command.Parameters.AddWithValue("@FourthPrizeMouths", lotoNumber.FourthPrizeMouths);
                command.Parameters.AddWithValue("@FourthPrizeAmount", lotoNumber.FourthPrizeAmount);
                command.Parameters.AddWithValue("@FifthPrizeMouths", lotoNumber.FifthPrizeMouths);
                command.Parameters.AddWithValue("@FifthPrizeAmount", lotoNumber.FifthPrizeAmount);
                command.Parameters.AddWithValue("@SixthPrizeMouths", lotoNumber.SixthPrizeMouths);
                command.Parameters.AddWithValue("@SixthPrizeAmount", lotoNumber.SixthPrizeAmount);
                command.Parameters.AddWithValue("@TotalSales", lotoNumber.TotalSales);
                command.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                command.ExecuteNonQuery();
            }
        }
    }
    public LotoNumber GetLatestLotoNumber()
    {
        LotoNumber latestLotoNumber = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var commandText = @"
        SELECT * FROM LotoNumbers
        ORDER BY DrawDate DESC
        LIMIT 1;
        ";

            using (var command = new MySqlCommand(commandText, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        latestLotoNumber = new LotoNumber
                        {
                            Issue = reader.GetString(reader.GetOrdinal("Issue")),
                            DrawDate = reader.GetDateTime(reader.GetOrdinal("DrawDate")), 
                            Number1 = reader.GetInt32(reader.GetOrdinal("Number1")),
                            Number2 = reader.GetInt32(reader.GetOrdinal("Number2")),
                            Number3 = reader.GetInt32(reader.GetOrdinal("Number3")),
                            Number4 = reader.GetInt32(reader.GetOrdinal("Number4")),
                            Number5 = reader.GetInt32(reader.GetOrdinal("Number5")),
                            Number6 = reader.GetInt32(reader.GetOrdinal("Number6")),
                            Number7 = reader.GetInt32(reader.GetOrdinal("Number7")),
                            BonusNumber1 = reader.IsDBNull(reader.GetOrdinal("BonusNumber1")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("BonusNumber1")),
                            BonusNumber2 = reader.IsDBNull(reader.GetOrdinal("BonusNumber2")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("BonusNumber2")),
                            Mouths = reader.GetInt32(reader.GetOrdinal("Mouths")),
                            Prize = reader.GetDecimal(reader.GetOrdinal("Prize")),
                            CarryOver = reader.IsDBNull(reader.GetOrdinal("CarryOver")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("CarryOver")),
                            SecondPrizeMouths = reader.GetInt32(reader.GetOrdinal("SecondPrizeMouths")),
                            SecondPrizeAmount = reader.GetDecimal(reader.GetOrdinal("SecondPrizeAmount")),
                            ThirdPrizeMouths = reader.GetInt32(reader.GetOrdinal("ThirdPrizeMouths")),
                            ThirdPrizeAmount = reader.GetDecimal(reader.GetOrdinal("ThirdPrizeAmount")),
                            FourthPrizeMouths = reader.GetInt32(reader.GetOrdinal("FourthPrizeMouths")),
                            FourthPrizeAmount = reader.GetDecimal(reader.GetOrdinal("FourthPrizeAmount")),
                            FifthPrizeMouths = reader.GetInt32(reader.GetOrdinal("FifthPrizeMouths")),
                            FifthPrizeAmount = reader.GetDecimal(reader.GetOrdinal("FifthPrizeAmount")),
                            SixthPrizeMouths = reader.GetInt32(reader.GetOrdinal("SixthPrizeMouths")),
                            SixthPrizeAmount = reader.GetDecimal(reader.GetOrdinal("SixthPrizeAmount")),
                            TotalSales = reader.GetDecimal(reader.GetOrdinal("TotalSales"))
                        };
                    }
                }
            }
        }

        return latestLotoNumber;
    }

    public void InsertContact(Contact contact)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var commandText = @"
                    INSERT INTO Contacts (Name, Email, Message, CreateTime)
                    VALUES (@Name, @Email, @Message, @CreateTime);
                ";

            using (var command = new MySqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Name", contact.Name);
                command.Parameters.AddWithValue("@Email", contact.Email);
                command.Parameters.AddWithValue("@Message", contact.Message);
                command.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                command.ExecuteNonQuery();
            }
        }
    }

}
