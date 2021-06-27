using ApplicationFoundation.Interfaces;
using CryptionManager.Lib.Shared;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CryptionManager.Lib
{
    public class EncryptProvider : IEncryptionManager
    {
        private static string GetRandomStr(int length)
        {
            var arrChar = new[]{
           'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
           '0','1','2','3','4','5','6','7','8','9',
           'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
          };

            var num = new StringBuilder();

            var rnd = new Random(DateTime.Now.Millisecond);
            for (var i = 0; i < length; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }

            return num.ToString();
        }

        #region AES
        public object CreateAesKey()
        {
            return new AESKey()
            {
                Key = GetRandomStr(32),
                IV = GetRandomStr(16)
            };
        }
        public string AesEncrypt(string data, string key, string vector)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));

            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsNotOutOfRange(key.Length, 32, 32, nameof(key));

            Check.Argument.IsNotEmpty(vector, nameof(vector));
            Check.Argument.IsNotOutOfRange(vector.Length, 16, 16, nameof(vector));

            var plainBytes = Encoding.UTF8.GetBytes(data);

            var encryptBytes = AesEncrypt(plainBytes, key, vector);
            if (encryptBytes == null)
            {
                return null;
            }
            return Convert.ToBase64String(encryptBytes);
        }
        public static byte[] AesEncrypt(byte[] data, string key, string vector)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));

            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsNotOutOfRange(key.Length, 32, 32, nameof(key));

            Check.Argument.IsNotEmpty(vector, nameof(vector));
            Check.Argument.IsNotOutOfRange(vector.Length, 16, 16, nameof(vector));

            var plainBytes = data;
            var bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            var bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);

            using (var aes = Aes.Create())
            {
                byte[] encryptData = null; // encrypted data
                try
                {
                    using (var memory = new MemoryStream())
                    {
                        if (aes != null)
                            using (var encryptor = new CryptoStream(memory,
                                aes.CreateEncryptor(bKey, bVector),
                                CryptoStreamMode.Write))
                            {
                                encryptor.Write(plainBytes, 0, plainBytes.Length);
                                encryptor.FlushFinalBlock();

                                encryptData = memory.ToArray();
                            }
                    }
                }
                catch
                {
                    encryptData = null;
                }
                return encryptData;
            }
        }

        public string AesDecrypt(string data, string key = "", string vector = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(vector))
            {
                var storedData = RsaProvider.StaticRsaConfiguration();
                key = storedData[0];
                vector = storedData[1];
            }
            Check.Argument.IsNotEmpty(data, nameof(data));

            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsNotOutOfRange(key.Length, 32, 32, nameof(key));

            Check.Argument.IsNotEmpty(vector, nameof(vector));
            Check.Argument.IsNotOutOfRange(vector.Length, 16, 16, nameof(vector));

            var encryptedBytes = Convert.FromBase64String(data);

            var decryptBytes = AesDecrypt(encryptedBytes, key, vector);

            if (decryptBytes == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(decryptBytes);
        }

        /// <summary>  
        ///  AES decrypt
        /// </summary>  
        /// <param name="data">Encrypted data</param>  
        /// <param name="key">Key, requires 32 bits</param>  
        /// <param name="vector">IV,requires 16 bits</param>  
        /// <returns>Decrypted byte array</returns>  

        public static byte[] AesDecrypt(byte[] data, string key, string vector)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));

            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsNotOutOfRange(key.Length, 32, 32, nameof(key));

            Check.Argument.IsNotEmpty(vector, nameof(vector));
            Check.Argument.IsNotOutOfRange(vector.Length, 16, 16, nameof(vector));

            var encryptedBytes = data;
            var bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            var bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);

            using (var aes = Aes.Create())
            {
                var decryptedData = new byte[] { }; // decrypted data
                try
                {
                    using (var memory = new MemoryStream(encryptedBytes))
                    {
                        if (aes != null)
                            using (var decryptor = new CryptoStream(memory, aes.CreateDecryptor(bKey, bVector),
                                CryptoStreamMode.Read))
                            {
                                using (var tempMemory = new MemoryStream())
                                {
                                    var buffer = new byte[1024];
                                    int readBytes;
                                    while ((readBytes = decryptor.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        tempMemory.Write(buffer, 0, readBytes);
                                    }

                                    decryptedData = tempMemory.ToArray();
                                }
                            }
                    }
                }
                catch
                {
                    decryptedData = null;
                }

                return decryptedData;
            }
        }

        /// <summary>  
        /// AES encrypt ( no IV)  
        /// </summary>  
        /// <param name="data">Raw data</param>  
        /// <param name="key">Key, requires 32 bits</param>  
        /// <returns>Encrypted string</returns>  
        public static string AesEncrypt(string data, string key)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));
            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsNotOutOfRange(key.Length, 32, 32, nameof(key));

            using (var memory = new MemoryStream())
            {
                using (var aes = Aes.Create())
                {
                    var plainBytes = Encoding.UTF8.GetBytes(data);
                    var bKey = new byte[32];
                    Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);

                    if (aes != null)
                    {
                        aes.Mode = CipherMode.ECB;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.KeySize = 128;
                        aes.Key = bKey;

                        using (var cryptoStream =
                            new CryptoStream(memory, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            try
                            {
                                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                return Convert.ToBase64String(memory.ToArray());
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>  
        /// AES decrypt( no IV)  
        /// </summary>  
        /// <param name="data">Encrypted data</param>  
        /// <param name="key">Key, requires 32 bits</param>  
        /// <returns>Decrypted string</returns>  
        public static string AesDecrypt(string data, string key)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));
            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsNotOutOfRange(key.Length, 32, 32, nameof(key));

            var encryptedBytes = Convert.FromBase64String(data);
            var bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);

            using (var memory = new MemoryStream(encryptedBytes))
            {
                using (var aes = Aes.Create())
                {
                    if (aes != null)
                    {
                        aes.Mode = CipherMode.ECB;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.KeySize = 128;
                        aes.Key = bKey;

                        using (var cryptoStream =
                            new CryptoStream(memory, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            try
                            {
                                var tmp = new byte[encryptedBytes.Length];
                                var len = cryptoStream.Read(tmp, 0, encryptedBytes.Length);
                                var ret = new byte[len];
                                Array.Copy(tmp, 0, ret, 0, len);

                                return Encoding.UTF8.GetString(ret, 0, len);
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>
        /// AES Rijndael
        /// </summary>
        public static void AesRijndael()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Machine Key

        public static string CreateDecryptionKey(int length)
        {
            Check.Argument.IsNotOutOfRange(length, 16, 48, nameof(length));
            return CreateMachineKey(length);
        }
        public static string CreateValidationKey(int length)
        {
            Check.Argument.IsNotOutOfRange(length, 48, 128, nameof(length));
            return CreateMachineKey(length);
        }

        private static string CreateMachineKey(int length)
        {

            var random = new byte[length / 2];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }

            var machineKey = new StringBuilder(length);
            for (var i = 0; i < random.Length; i++)
            {
                machineKey.Append(string.Format("{0:X2}", random[i]));
            }
            return machineKey.ToString();
        }

        #endregion

        public static string Base64Encrypt(string input)
        {
            return Base64Encrypt(input, Encoding.UTF8);
        }
        public static string Base64Encrypt(string input, Encoding encoding)
        {
            Check.Argument.IsNotEmpty(input, nameof(input));
            return Convert.ToBase64String(encoding.GetBytes(input));
        }
        public static string Base64Decrypt(string input)
        {
            return Base64Decrypt(input, Encoding.UTF8);
        }
        public static string Base64Decrypt(string input, Encoding encoding)
        {
            Check.Argument.IsNotEmpty(input, nameof(input));
            return encoding.GetString(Convert.FromBase64String(input));
        }
        public object CreateRsaKey(string rsaSize)
        {
            throw new NotImplementedException();
        }
        public string RsaEncrypt(object publicKey, string testStr, RSAEncryptionPadding pkcs)
        {
            throw new NotImplementedException();
        }
    }

    public class AESKey
    {
        public string Key { get; set; }
        public string IV { get; set; }
    }
    internal class RsaProvider
    {
        private static readonly Regex _PEMCode = new Regex(@"--+.+?--+|\s+");
        private static readonly byte[] _SeqOID = new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        private static readonly byte[] _Ver = new byte[] { 0x02, 0x01, 0x00 };
		private static string TextBreak(string text, int line)
        {
            var idx = 0;
            var len = text.Length;
            var str = new StringBuilder();
            while (idx < len)
            {
                if (idx > 0)
                {
                    str.Append('\n');
                }

                str.Append(idx + line >= len ? text.Substring(idx) : text.Substring(idx, line));
                idx += line;
            }
            return str.ToString();
        }
        public static string[] StaticRsaConfiguration()
        {
            return new[] { "rc29jJ3CPTlfFheHqRAwwIgQlI0yRDXB", "MwjMBBUhXpUTwELv" };
        }
    }
}
