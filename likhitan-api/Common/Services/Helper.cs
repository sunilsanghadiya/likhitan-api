using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace likhitan.Common.Services
{
    public static class Helper
    {
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private const int SaltSize = 16;  
        private const int KeySize = 32;   
        private const int Iterations = 10000; 

        private static byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] newHash = pbkdf2.GetBytes(KeySize);

                return storedHashBytes.SequenceEqual(newHash);
            }
        }

        public static bool IsEmailValid(this string email) => !Regex.IsMatch(email, EmailPattern) ? false : true;

        public static string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public static bool IsOtpExpired(DateTime generatedTime, double expirationMinutes = 1)
        {
            DateTime currentTime = DateTime.UtcNow;
            DateTime expiryTime = generatedTime.AddMinutes(expirationMinutes);
            return currentTime > expiryTime;
        }

        public static bool IsPasswordFall(string password, string firstName, string lastName)
        {
            string passwordPattern = $"^(?!{firstName}{lastName})(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()_+\\-\\[\\]{{}};:'\"\\\\|,.<>\\/?`~])(?!.*\\s).{{8,}}$";
            return Regex.IsMatch(password, passwordPattern) ? true : false;
        }

        public static string HashPassword(string password)
        {
            byte[] salt = GenerateSalt();

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(KeySize);

                string hashedPassword = $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
                return hashedPassword;
            }
        }
    }
}
