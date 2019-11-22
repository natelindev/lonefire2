using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using lonefire.Models;
using Sodium;
using Microsoft.Extensions.Options;

namespace lonefire.Extensions
{
    //Override default Password Hasher for old system compatibility
    public class LfPasswordHasher : IPasswordHasher<ApplicationUser>
    {
        private readonly LfPasswordHasherOptions options;

        public LfPasswordHasher(IOptions<LfPasswordHasherOptions> optionsAccessor = null)
        {
            options = optionsAccessor?.Value ?? new LfPasswordHasherOptions();
        }


        public string HashPassword(ApplicationUser user, string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            switch (options)
            {
                case LfHashFunction.Argon2:
                    return PasswordHash.ArgonHashString(password, PasswordHash.StrengthArgon.Interactive);
            }
            
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword.Equals(CalcMd5(CalcMd5(providedPassword))))
                return PasswordVerificationResult.Success;
            else return PasswordVerificationResult.Failed;
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword.Equals(CalcMd5(CalcMd5(providedPassword))))
                return PasswordVerificationResult.Success;
            else return PasswordVerificationResult.Failed;
        }

        #region Helper

        private string CalcMd5(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            md5.Dispose();
            //Convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion
    }

    public class LfPasswordHasherOptions
    {
        public LfHashFunction Function { get; set; } = LfHashFunction.Argon2;
    }

    public enum LfHashFunction
    {
        Argon2 = 0,
        BCrypt,
        SCrypt,
        MD5 // not recommended
    }
}
