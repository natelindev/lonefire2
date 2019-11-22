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
            return options.HashFunction switch
            {
                LfHashFunction.Argon2 => PasswordHash.ArgonHashString(password, options.Argon2Option.OpsLimit, options.Argon2Option.MemLimit),
                LfHashFunction.SCrypt => PasswordHash.ScryptHashString(password, options.SCryptOption.OpsLimit, options.SCryptOption.MemLimit),
                LfHashFunction.MD5 => CalcMd5(password),
                _ => throw new IndexOutOfRangeException(),
            };
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));
            if (providedPassword == null) throw new ArgumentNullException(nameof(providedPassword));
            var isValid = options.HashFunction switch
            {
                LfHashFunction.MD5 => hashedPassword.Equals(CalcMd5(providedPassword)),
                LfHashFunction.Argon2 => PasswordHash.ArgonHashStringVerify(hashedPassword, providedPassword),
                LfHashFunction.SCrypt => PasswordHash.ScryptHashStringVerify(hashedPassword, providedPassword),
                _ => throw new IndexOutOfRangeException(),
            };
            return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));
            if (providedPassword == null) throw new ArgumentNullException(nameof(providedPassword));
            var isValid = options.HashFunction switch
            {
                LfHashFunction.MD5 => hashedPassword.Equals(CalcMd5(providedPassword)),
                LfHashFunction.Argon2 => PasswordHash.ArgonHashStringVerify(hashedPassword, providedPassword),
                LfHashFunction.SCrypt => PasswordHash.ScryptHashStringVerify(hashedPassword, providedPassword),
                _ => throw new IndexOutOfRangeException(),
            };
            return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
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
        public LfHashFunction HashFunction { get; set; } = LfHashFunction.Argon2;
        public CryptoOption Argon2Option { get; set; } = new CryptoOption(4, 33554432);
        public CryptoOption SCryptOption { get; set; } = new CryptoOption(524288, 16777216);
    }
    
    public class CryptoOption
    {
        public long OpsLimit { get; set; }
        public int MemLimit { get; set; }

        public CryptoOption(long opsLimit, int memLimit)
        {
            OpsLimit = opsLimit;
            MemLimit = memLimit;
        }
    }

    public enum LfHashFunction
    {
        Argon2 = 0,
        SCrypt,
        MD5 // not recommended
    }
}
