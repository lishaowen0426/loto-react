using Loto.Models;
using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Loto.DataAccessLayer
{
    public interface IUserService
    {
        Task<bool> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<User> GetUserByVerificationToken(string token);

        Task<User> GetUserByPasswordResetToken(string token);
        Task UpdateUser(User user);

        Task UpdateUserPassword(User user);
        Task<User> GetUserByEmail(string email);


        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    }

    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Register(User user, string password)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                if (await UserExists(user.Username))
                    return false;

                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = Convert.ToBase64String(passwordHash);
                user.PasswordSalt = Convert.ToBase64String(passwordSalt);

                user.EmailVerificationToken = Guid.NewGuid().ToString(); // 生成验证令牌
                user.EmailVerificationTokenGeneratedAt = DateTime.UtcNow; // 设置令牌生成时间

                string sql = "INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, EmailVerificationToken, IsEmailVerified, EmailVerificationTokenGeneratedAt) VALUES (@Username, @Email, @PasswordHash, @PasswordSalt, @EmailVerificationToken, @IsEmailVerified, @EmailVerificationTokenGeneratedAt)";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
                    cmd.Parameters.AddWithValue("@EmailVerificationToken", user.EmailVerificationToken);
                    cmd.Parameters.AddWithValue("@IsEmailVerified", user.IsEmailVerified);
                    cmd.Parameters.AddWithValue("@EmailVerificationTokenGeneratedAt", user.EmailVerificationTokenGeneratedAt);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return true;
        }

        public async Task<User> GetUserByVerificationToken(string token)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                string sql = "SELECT * FROM Users WHERE EmailVerificationToken = @Token";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
                                IsEmailVerified = reader.GetBoolean(reader.GetOrdinal("IsEmailVerified")),
                                EmailVerificationToken = reader.GetString(reader.GetOrdinal("EmailVerificationToken")),
                                EmailVerificationTokenGeneratedAt = reader.IsDBNull(reader.GetOrdinal("EmailVerificationTokenGeneratedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EmailVerificationTokenGeneratedAt"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task UpdateUser(User user)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                string sql = "UPDATE Users SET IsEmailVerified = @IsEmailVerified, EmailVerificationToken = @EmailVerificationToken, EmailVerificationTokenGeneratedAt = @EmailVerificationTokenGeneratedAt WHERE Id = @Id";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@IsEmailVerified", user.IsEmailVerified);
                    cmd.Parameters.AddWithValue("@EmailVerificationToken", user.EmailVerificationToken ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailVerificationTokenGeneratedAt", user.EmailVerificationTokenGeneratedAt ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", user.Id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<User> Login(string username, string password)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                string sql = "SELECT * FROM users WHERE Username = @Username";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var user = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
                                IsEmailVerified = reader.GetBoolean(reader.GetOrdinal("IsEmailVerified")) // 获取IsEmailVerified
                            };

                            if (!VerifyPasswordHash(password, Convert.FromBase64String(user.PasswordHash), Convert.FromBase64String(user.PasswordSalt)))
                                return null;

                            return user;
                        }
                    }
                }
            }
            return null;
        }

        public async Task<bool> UserExists(string username)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    var count = (long)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                string sql = "SELECT * FROM Users WHERE Email = @Email";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
                                PasswordResetToken = reader.IsDBNull(reader.GetOrdinal("PasswordResetToken")) ? null : reader.GetString(reader.GetOrdinal("PasswordResetToken")),

                                PasswordResetTokenExpiry = reader.IsDBNull(reader.GetOrdinal("PasswordResetTokenExpiry")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PasswordResetTokenExpiry"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<User> GetUserByPasswordResetToken(string token)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                string sql = "SELECT * FROM Users WHERE PasswordResetToken = @Token";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
                                PasswordResetToken = reader.GetString(reader.GetOrdinal("PasswordResetToken")),
                                PasswordResetTokenExpiry = reader.IsDBNull(reader.GetOrdinal("PasswordResetTokenExpiry")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PasswordResetTokenExpiry"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task UpdateUserPassword(User user)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                string sql = "UPDATE Users SET  PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt, PasswordResetToken = @PasswordResetToken, PasswordResetTokenExpiry = @PasswordResetTokenExpiry WHERE Id = @Id";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                 
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
                    cmd.Parameters.AddWithValue("@PasswordResetToken", user.PasswordResetToken ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PasswordResetTokenExpiry", user.PasswordResetTokenExpiry ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", user.Id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }




    }
}
