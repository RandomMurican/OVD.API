using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using OVD.API.Dtos;
using OVD.API.Models;

namespace OVD.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private string connectionString = "Server=localhost;Port=3306;Database=guacamole_db;UID=root;" + "Password=secret;";
        public async Task<bool> UserExists(string username)
        {
            // Basic data check, only ascii allowed
            Regex basicCheck = new Regex("[\\w\\-\\s]+", RegexOptions.Compiled);


            if (basicCheck.IsMatch(username)) {
                MySqlConnection connection = new MySqlConnection(connectionString);

                MySqlCommand command = new MySqlCommand("SELECT name FROM guacamole_entity WHERE type='user' AND name = '" + username + "';");
                command.Connection = connection;
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                int results = reader.FieldCount;
                connection.Close();
                if (results > 0) return true;
            }
            return false;
        }

        public async Task<User> Login(UserForLoginDto userForLogin)
        {
            Console.WriteLine("\n\n\n\nREPO");
            Console.WriteLine(userForLogin.Username);
            User user = new User();
             // Basic data check, only ascii allowed
            Regex basicCheck = new Regex("[\\w\\-\\s]+", RegexOptions.Compiled);

            if (basicCheck.IsMatch(userForLogin.Username)) {
                if (!await UserExists(userForLogin.Username)) return null;
                MySqlConnection connection = new MySqlConnection(connectionString);

                MySqlCommand command = new MySqlCommand("SELECT guacamole_entity.entity_id, guacamole_user.user_id, guacamole_entity.name, guacamole_user.password_hash, guacamole_user.password_salt FROM guacamole_entity, guacamole_user WHERE guacamole_entity.entity_id = guacamole_user.entity_id AND guacamole_entity.name = '" + userForLogin.Username + "';");
                command.Connection = connection;
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.HasRows) {
                    user.EntityId = reader.GetInt32(0);
                    user.Id = reader.GetInt32(1);
                    user.Username = reader.GetString(2);
                    user.PasswordHash  = System.Text.Encoding.ASCII.GetBytes(reader.GetString(3));
                    Console.WriteLine(user.PasswordHash);
                    user.PasswordSalt = System.Text.Encoding.ASCII.GetBytes(reader.GetString(4));
                    Console.WriteLine(user.PasswordSalt);
                }
                connection.Close();

                
                Console.WriteLine(VerifyPasswordHash(userForLogin.Password, user.PasswordHash, user.PasswordSalt) + "\n\n\n\n");
                
                if (!VerifyPasswordHash(userForLogin.Password, user.PasswordHash, user.PasswordSalt))
                return null;

                return user;
            }
            return null;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            byte[] computedHash = ComputeHash(password, passwordSalt);
            for (int i = 0; i < computedHash.Length; i++)
                {
                    Console.WriteLine(computedHash[i] + "==" + passwordHash[i]);
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            /*
            using (System.Security.Cryptography.SHA256 hmac = System.Security.Cryptography.SHA256.Create())
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                // loop over every element in the two byte arrays comparing to each other
                for (int i = 0; i < computedHash.Length; i++)
                {
                    Console.WriteLine(computedHash[i] + "==" + passwordHash[i]);
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }*/

            return true;
        }

        public async Task<User> Register(UserForLoginDto userForLogin)
        {
            return null;
            /*
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            admin.PasswordHash = passwordHash;
            admin.PasswordSalt = passwordSalt;

            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();

            return admin; */
        }

        public static byte[] ComputeHash(string password, byte[] salt)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(password);

            byte[] concat = new byte[plainTextBytes.Length + salt .Length];
            System.Buffer.BlockCopy(plainTextBytes, 0, concat, 0, plainTextBytes.Length);
            System.Buffer.BlockCopy(salt , 0, concat, plainTextBytes.Length, salt .Length);

            System.Security.Cryptography.SHA256Managed hash = new System.Security.Cryptography.SHA256Managed();

            byte[] tHashBytes = hash.ComputeHash(concat);

            return tHashBytes;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public Task<bool> UserIsAdmin(string username)
        {
            throw new NotImplementedException();
        }
    }
}