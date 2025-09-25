using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsSHA256
    {
        /// <summary>
        /// This method used for encrypting data
        /// </summary>
        /// <param name="data">this represent a data to be encrypted</param>
        /// <returns>return data encrypted using SHA-256</returns>
        public static string ComputeHash(string data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashByte = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));

                return BitConverter.ToString(hashByte).Replace("-", "").ToLower();
            }
        }

    }
}
