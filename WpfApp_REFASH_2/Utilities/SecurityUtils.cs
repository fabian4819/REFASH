using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH.Utilities
{
    public static class SecurityUtils
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        public static bool PasswordComparison(string str1, string str2)
        {
            if (str1.Length != str2.Length)
            {
                return false;
            }

            int result = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                result |= str1[i] ^ str2[i];
            }
            return result == 0;
        }

    }
}
