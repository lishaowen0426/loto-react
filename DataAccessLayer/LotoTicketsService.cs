using Loto.Models;
using MySql.Data.MySqlClient;

namespace Loto.DataAccessLayer
{
    public class LotoTicketsService
    {
        private readonly string _connectionString;

        public LotoTicketsService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public void InsertPersonLotteryTicket(PersonLotteryTicket ticket)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var commandText = @"
        INSERT INTO PersonLotteryTickets 
        (GUID, Userid, TicketNumber1, TicketNumber2, TicketNumber3, TicketNumber4, TicketNumber5, TicketNumber6, TicketNumber7, SubmissionDate, IsPublic, UserIdentifier, Cookie, IPAddress, Issue, DrawDate, CreateTime)
        VALUES 
        (@GUID, @Userid, @TicketNumber1, @TicketNumber2, @TicketNumber3, @TicketNumber4, @TicketNumber5, @TicketNumber6, @TicketNumber7, @SubmissionDate, @IsPublic, @UserIdentifier, @Cookie, @IPAddress, @Issue, @DrawDate, @CreateTime);
        ";

                using (var command = new MySqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@GUID", Guid.NewGuid().ToString());
                    command.Parameters.AddWithValue("@Userid", ticket.Userid);
                    command.Parameters.AddWithValue("@TicketNumber1", ticket.TicketNumber1);
                    command.Parameters.AddWithValue("@TicketNumber2", ticket.TicketNumber2);
                    command.Parameters.AddWithValue("@TicketNumber3", ticket.TicketNumber3);
                    command.Parameters.AddWithValue("@TicketNumber4", ticket.TicketNumber4);
                    command.Parameters.AddWithValue("@TicketNumber5", ticket.TicketNumber5);
                    command.Parameters.AddWithValue("@TicketNumber6", ticket.TicketNumber6);
                    command.Parameters.AddWithValue("@TicketNumber7", ticket.TicketNumber7);
                    command.Parameters.AddWithValue("@SubmissionDate", ticket.SubmissionDate);
                    command.Parameters.AddWithValue("@IsPublic", ticket.IsPublic);
                    command.Parameters.AddWithValue("@UserIdentifier", ticket.UserIdentifier ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Cookie", ticket.Cookie ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IPAddress", ticket.IPAddress ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Issue", ticket.Issue ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DrawDate", ticket.DrawDate != DateTime.MinValue ? (object)ticket.DrawDate : DBNull.Value);
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
        SELECT Issue,DrawDate FROM LotoNumbers
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
                               
                            };
                        }
                    }
                }
            }

            return latestLotoNumber;
        }

        public PersonLotteryTicket GetLatestPersonLotteryTicket(string userId, DateTime drawDate, string cookie = "")
        {
            PersonLotteryTicket latestTicket = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var commandText = @"
        SELECT Issue, DrawDate, IPAddress, Cookie, UserIdentifier, IsPublic 
        FROM PersonLotteryTickets
        WHERE (Userid = @UserId OR Cookie = @Cookie) AND DrawDate = @DrawDate
        ORDER BY CreateTime desc, DrawDate DESC
        LIMIT 1;
        ";

                using (var command = new MySqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Cookie", cookie);
                    command.Parameters.AddWithValue("@DrawDate", drawDate);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            latestTicket = new PersonLotteryTicket
                            {
                                Issue = reader.GetString(reader.GetOrdinal("Issue")),
                                DrawDate = reader.GetDateTime(reader.GetOrdinal("DrawDate")),
                                IPAddress = reader.GetString(reader.GetOrdinal("IPAddress")),
                                Cookie = reader.GetString(reader.GetOrdinal("Cookie")),
                                UserIdentifier = reader.GetString(reader.GetOrdinal("UserIdentifier")),
                                IsPublic = reader.GetBoolean(reader.GetOrdinal("IsPublic"))
                            };
                        }
                    }
                }
            }

            return latestTicket;
        }
        public List<PersonLotteryTicket> GetPublicTicketsByDate(DateTime drawDate)
        {
            var tickets = new List<PersonLotteryTicket>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var commandText = @"
        SELECT Id, Userid, TicketNumber1, TicketNumber2, TicketNumber3, TicketNumber4, TicketNumber5, TicketNumber6, TicketNumber7, SubmissionDate, IsPublic, UserIdentifier, Cookie, IPAddress, Issue, DrawDate, CreateTime
        FROM PersonLotteryTickets
        WHERE IsPublic = true AND DrawDate = @DrawDate;
        ";

                using (var command = new MySqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@DrawDate", drawDate);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ticket = new PersonLotteryTicket
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Userid = reader.GetString(reader.GetOrdinal("Userid")),
                                TicketNumber1 = reader.GetString(reader.GetOrdinal("TicketNumber1")),
                                TicketNumber2 = reader.GetString(reader.GetOrdinal("TicketNumber2")),
                                TicketNumber3 = reader.GetString(reader.GetOrdinal("TicketNumber3")),
                                TicketNumber4 = reader.GetString(reader.GetOrdinal("TicketNumber4")),
                                TicketNumber5 = reader.GetString(reader.GetOrdinal("TicketNumber5")),
                                TicketNumber6 = reader.GetString(reader.GetOrdinal("TicketNumber6")),
                                TicketNumber7 = reader.GetString(reader.GetOrdinal("TicketNumber7")),
                                SubmissionDate = reader.GetDateTime(reader.GetOrdinal("SubmissionDate")),
                                IsPublic = reader.GetBoolean(reader.GetOrdinal("IsPublic")),
                                UserIdentifier = reader.IsDBNull(reader.GetOrdinal("UserIdentifier")) ? null : reader.GetString(reader.GetOrdinal("UserIdentifier")),
                                Cookie = reader.IsDBNull(reader.GetOrdinal("Cookie")) ? null : reader.GetString(reader.GetOrdinal("Cookie")),
                                IPAddress = reader.IsDBNull(reader.GetOrdinal("IPAddress")) ? null : reader.GetString(reader.GetOrdinal("IPAddress")),
                                Issue = reader.IsDBNull(reader.GetOrdinal("Issue")) ? null : reader.GetString(reader.GetOrdinal("Issue")),
                                DrawDate = reader.GetDateTime(reader.GetOrdinal("DrawDate")),
                                CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"))
                            };
                            tickets.Add(ticket);
                        }
                    }
                }
            }

            return tickets;
        }


    }
}
