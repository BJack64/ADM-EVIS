// 
// Copyright (C) 2002 Obviex(TM). All rights reserved.
// 
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace eFakturADM.Logic.Utilities
{
    public class Rijndael
    {

        #region Property
        //Modified By Irman Sulaeman at 2016-04-25
        private static string m_sPassPhrase = "ABD538CC-2EF7-4041-9955-F7D8848B4AE1"; //Default : "43367EA8-F268-4723-A2C4-666ABE486780"; // can be any string
        private static string m_sSaltValue = "BE72E164-C4CC-42D2-BD98-6565348478A1"; //Default : "A4CC5AE0-E837-4E76-9A62-8923AD17602B"; // can be any string
        private static string m_sHashAlgorithm = "SHA1"; // can be "MD5"
        private static int m_iPasswordIterations = 2; // can be any number
        private static string m_sInitVector = "5C2980D968C845F1"; // must be 16 bytes
        private static int m_iKeySize = 256;                // can be 192 or 128
        #endregion

        public static string EncryptString(string __sPlainText, string __sPassPhrase, string __sSaltValue,
            string __sHashAlgorithm, int __iPasswordIterations, string __sInitVector, int __iKeySize)
        {
            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            // encoding.
            byte[] _bInitVectorBytes = Encoding.ASCII.GetBytes(__sInitVector);
            byte[] _bSaltValueBytes = Encoding.ASCII.GetBytes(__sSaltValue);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] _bPlainTextBytes = Encoding.UTF8.GetBytes(__sPlainText);

            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes _Password = new PasswordDeriveBytes(
                                                            __sPassPhrase,
                                                            _bSaltValueBytes,
                                                            __sHashAlgorithm,
                                                            __iPasswordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] _bKeyBytes = _Password.GetBytes(__iKeySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged _rmSymmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            _rmSymmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform _Encryptor = _rmSymmetricKey.CreateEncryptor(
                                                             _bKeyBytes,
                                                             _bInitVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream _MemoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream _CryptoStream = new CryptoStream(_MemoryStream,
                                                         _Encryptor,
                                                         CryptoStreamMode.Write);
            // Start encrypting.
            _CryptoStream.Write(_bPlainTextBytes, 0, _bPlainTextBytes.Length);

            // Finish encrypting.
            _CryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] _bCipherTextBytes = _MemoryStream.ToArray();

            // Close both streams.
            _MemoryStream.Close();
            _CryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string _sCipherText = Convert.ToBase64String(_bCipherTextBytes);

            // Return encrypted string.
            return _sCipherText;
        }

        public static string DecryptString(string __sCipherText, string __sPassPhrase, string __sSaltValue,
            string __sHashAlgorithm, int __iPasswordIterations, string __sInitVector, int __iKeySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] _bInitVectorBytes = Encoding.ASCII.GetBytes(__sInitVector);
            byte[] _bSaltValueBytes = Encoding.ASCII.GetBytes(__sSaltValue);

            // Convert our ciphertext into a byte array.
            byte[] _bCipherTextBytes = Convert.FromBase64String(__sCipherText);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes _Password = new PasswordDeriveBytes(
                                                            __sPassPhrase,
                                                            _bSaltValueBytes,
                                                            __sHashAlgorithm,
                                                            __iPasswordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] bKeyBytes = _Password.GetBytes(__iKeySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged _rmSymmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            _rmSymmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform _Decryptor = _rmSymmetricKey.CreateDecryptor(
                                                             bKeyBytes,
                                                             _bInitVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream _MemoryStream = new MemoryStream(_bCipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream _CryptoStream = new CryptoStream(_MemoryStream,
                                                          _Decryptor,
                                                          CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] _bPlainTextBytes = new byte[_bCipherTextBytes.Length];

            // Start decrypting.
            int iDecryptedByteCount = _CryptoStream.Read(_bPlainTextBytes,
                                                       0,
                                                       _bPlainTextBytes.Length);

            // Close both streams.
            _MemoryStream.Close();
            _CryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string _sPlainText = Encoding.UTF8.GetString(_bPlainTextBytes,
                                                       0,
                                                       iDecryptedByteCount);

            // Return decrypted string.   
            return _sPlainText;
        }

        public static string Encrypt(string __sPlainText)
        {
            string _sPassPhrase = m_sPassPhrase; //"43367EA8-F268-4723-A2C4-666ABE486780";        // can be any string
            string _sSaltValue = m_sSaltValue; //"A4CC5AE0-E837-4E76-9A62-8923AD17602B";        // can be any string
            string _sHashAlgorithm = m_sHashAlgorithm; //"SHA1";             // can be "MD5"
            int _iPasswordIterations = m_iPasswordIterations; //2;                  // can be any number
            string _sInitVector = m_sInitVector; //"5C2980D968C845F6"; // must be 16 bytes
            int iKeySize = m_iKeySize; //256;                // can be 192 or 128

            return EncryptString(__sPlainText, _sPassPhrase, _sSaltValue, _sHashAlgorithm,
                _iPasswordIterations, _sInitVector, iKeySize);
        }

        public static string Decrypt(string __sCipherText)
        {
            string _sPassPhrase = m_sPassPhrase; //"43367EA8-F268-4723-A2C4-666ABE486780";        // can be any string
            string _sSaltValue = m_sSaltValue; //"A4CC5AE0-E837-4E76-9A62-8923AD17602B";        // can be any string
            string _sHashAlgorithm = m_sHashAlgorithm; //"SHA1";             // can be "MD5"
            int _iPasswordIterations = 2;                  // can be any number
            string _sInitVector = m_sInitVector; //"5C2980D968C845F6"; // must be 16 bytes
            int _iKeySize = m_iKeySize; // 256;                // can be 192 or 128

            return DecryptString(__sCipherText, _sPassPhrase, _sSaltValue, _sHashAlgorithm,
                _iPasswordIterations, _sInitVector, _iKeySize);
        }

    }
}
