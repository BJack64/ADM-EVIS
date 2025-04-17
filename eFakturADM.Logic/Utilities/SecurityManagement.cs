using eFakturADM.Logic.Collections;
using System;
using System.Security.Cryptography;
using System.Text;

namespace eFakturADM.Logic.Utilities
{
    public class SecurityManagement
    {
        private const string _alg = "HmacSHA256";
        private const string _salt = "rz3LuOtgoj9WQzvFh";

        public static string GenerateToken(string username, string userAgent, long ticks)
        {
            string hash = string.Join(":", new string[] { username, userAgent, ticks.ToString() });
            string hashLeft = "";
            string hashRight = "";

            using (HMAC hmac = HMACSHA256.Create(_alg))
            {

                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));
                hashLeft = Convert.ToBase64String(hmac.Hash);
                hashRight = string.Join(":", new string[] { username, ticks.ToString() });
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));
        }

        public static bool IsTokenValid(string token)
        {
            bool result = false;

            try
            {
                //check from db
                if (UserAuthentications.GetToken(token) != null)
                {
                    result = true;
                }
            }
            catch
            {
            }

            return result;
        }
    }
}
