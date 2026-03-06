using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Amonic.App.Data;

namespace Amonic.App.Services
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsInfrastructureError { get; set; }
    }

    public class AuthService
    {
        public AuthResult Login(string email, string password)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
SELECT TOP 1 ID, RoleID, Active
FROM Users
WHERE Email = @Email AND Password = @PasswordHash";
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PasswordHash", Md5(password));

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return new AuthResult { Success = false, ErrorMessage = "Неверный логин или пароль." };
                        }

                        var active = reader["Active"] != DBNull.Value && Convert.ToBoolean(reader["Active"]);
                        if (!active)
                        {
                            return new AuthResult { Success = false, IsActive = false, ErrorMessage = "Учетная запись заблокирована администратором." };
                        }

                        return new AuthResult
                        {
                            Success = true,
                            IsActive = true,
                            UserId = Convert.ToInt32(reader["ID"]),
                            RoleId = Convert.ToInt32(reader["RoleID"])
                        };
                    }
                }
            }
            catch (SqlException)
            {
                return new AuthResult
                {
                    Success = false,
                    IsInfrastructureError = true,
                    ErrorMessage = "Не удалось подключиться к SQL Server. Проверьте имя сервера (KAB17-11\\SQLEXPRESS), что служба SQL Server запущена, и права Windows-доступа."
                };
            }
            catch (Exception)
            {
                return new AuthResult
                {
                    Success = false,
                    IsInfrastructureError = true,
                    ErrorMessage = "Ошибка инициализации БД. Проверьте connection string в App.config."
                };
            }
        }

        public static string Md5(string plainText)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(plainText ?? string.Empty);
                var hash = md5.ComputeHash(bytes);
                var builder = new StringBuilder();
                foreach (var b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
