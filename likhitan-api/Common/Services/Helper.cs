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


        //public static string GenerateJwtToken(string username)
        //{
        //    var key = Encoding.UTF8.GetBytes("962b6e8b039403139dc5d32f5f3f618bc1de10bb8d5945d6bf45d143b91de30435b73cb5e411275418098ab4dbd4c55d637e2bb3291c976cfb3c84f024ae9aa0dcfbf6167610418b6df33faaa45fc4e11d16b8f183731ef9d6f4e84c0af02813c6d2b84769d56897fc7ae5db49f56767a381f0342b9feed34216508006ab935c00d371074644898c884091f70ebcc486a99c7a63c889d70a6c0b9de5125b1ddf5d1b2c698fb9f5962ff211d43ebcbaddd2d63d2f88f5dbd3735a27797a8ae762bf96ac766b15f6662fbfd7c2fd6307bdc98fae9326ec0a1a0cd5a7d392eb2f958c9f3fca28472d79f0f5eee2a507df9435625cc059e81aad155724fd34535ef5741fea927ed62f3705fb4ad95f22c149d1fce0e14fda952133bc7526088f124e659802570036732d6dd584e648d57943e6d3b2c702e49d93a163c878afa8a7e9429f3384322567c9f1c3011bc86ae8d8462acd1c691ce92d609868a1633171a44a5288af7b59e448d04e089a3b2ac6d0d22c5d5d68fa7019c04502ddce1accd1caa63c10d47a8c2723e5b298560314d1b31932cfa7a9058532175ab8a4393214938a7643ef7ca98a6d1c8235d57dbd8cfb9351ac3fef57a73dd3cf5e625e94d69f5d5c32e69585e39eba9d46297b3a7f7dfc9535800ace1bf84cb291df97371dbee9f202f2c5243216d5efa692ad645e733a4a41130aacc1a57eff226fee52cc");
        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, username),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString().Replace("-", ""))
        //    };

        //    var token = new JwtSecurityToken(
        //        issuer: "https://localhost:7043",
        //        audience: "https://localhost",
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(15),
        //        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        //public static string GenerateRefreshToken(string username, TimeSpan expiry, string secretKey)
        //{
        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, username),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString().Replace("-", ""))  
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: "https://localhost:7043", 
        //        audience: "https://localhost", 
        //        claims: claims,
        //        expires: DateTime.UtcNow.Add(expiry),
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

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

        //public static ClaimsPrincipal ValidateToken(string token, string secretKey, bool validateLifetime = true)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.UTF8.GetBytes(secretKey);

        //    try
        //    {
        //        var validationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            ValidateLifetime = validateLifetime,
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key)
        //        };

        //        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

        //        if (validatedToken is JwtSecurityToken jwtToken &&
        //            jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            return principal;
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //    return null;
        //}

        //public static string GenerateToken(IEnumerable<Claim> claims, TimeSpan expiry, string secretKey)
        //{
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: "your-app",
        //        audience: "your-client",
        //        claims: claims,
        //        expires: DateTime.UtcNow.Add(expiry),
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }
}
