using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eFakturADM.Logic.Utilities
{
    public static class CryptographyHelper
    {
        public static string EncryptAndHash(string key, string value)
        {
            string retval = null;

            var des = new MACTripleDES();
            var md5 = new MD5CryptoServiceProvider();
            des.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

            string encrypted = Convert.ToBase64String(des.ComputeHash(Encoding.UTF8.GetBytes(value))) + '-' + Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

            retval = HttpUtility.UrlEncode(encrypted);

            return retval;
        }

        public static string DecryptWithHash(string key, string encoded)
        {
            string value = null;

            var des = new MACTripleDES();
            var md5 = new MD5CryptoServiceProvider();

            des.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

            string decoded = HttpUtility.UrlDecode(encoded);

            decoded = decoded.Replace(" ", "+");
            value = Encoding.UTF8.GetString(Convert.FromBase64String(decoded.Split('-')[1]));

            string savedHash = Encoding.UTF8.GetString(Convert.FromBase64String(decoded.Split('-')[0]));
            string calculatedHash = Encoding.UTF8.GetString(des.ComputeHash(Encoding.UTF8.GetBytes(value)));

            if (savedHash != calculatedHash)
                return null;

            return value;
        }
    }
}
