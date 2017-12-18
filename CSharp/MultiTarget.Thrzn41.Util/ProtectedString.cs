/* 
 * MIT License
 * 
 * Copyright(c) 2017 thrzn41
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Thrzn41.Util
{

    /// <summary>
    /// Scope for string protection.
    /// </summary>
    public enum StringProtectionScope
    {
        /// <summary>
        /// String can be encrypted an decrypted by only current user.
        /// </summary>
        CurrentUser,

        /// <summary>
        /// String can be encrypted an decrypted on local machine.
        /// </summary>
        LocalMachine,
    }


    /// <summary>
    /// Provides features for encrypting and decrypting String.
    /// This class can be used to save/load encrypted string to/from config file or database, etc.
    /// In current version, an encrypted string by this class does not have portability and can be decrypted by local user or local machine.
    /// Please note that this class does not provide in-memory protection.
    /// </summary>
    /// <remarks>
    /// This class does not provide in-memory protection and can be used to save/load encrypted string to/from config file or database, etc.
    /// </remarks>
    public class ProtectedString
    {

        /// <summary>
        /// Crypto random generator.
        /// </summary>
        private static readonly CryptoRandom RAND = new CryptoRandom();


        /// <summary>
        /// Internal encoding of this class.
        /// </summary>
        private static readonly Encoding ENCODING = UTF8Utils.UTF8_WITHOUT_BOM;


        /// <summary>
        /// Gets encrypted data.
        /// </summary>
        public byte[] EncryptedData { get; private set; }

        /// <summary>
        /// Gets entropy to encrypt or decrypt data.
        /// </summary>
        public byte[] Entropy { get; private set; }

        /// <summary>
        /// Gets encrypted data in base64 format.
        /// </summary>
        public string EncryptedDataBase64
        {
            get
            {
                return Convert.ToBase64String(this.EncryptedData, Base64FormattingOptions.None);
            }
        }

        /// <summary>
        /// Gets entropy in base64 format.
        /// </summary>
        public string EntropyBase64
        {
            get
            {
                if(this.Entropy == null)
                {
                    return null;
                }

                return Convert.ToBase64String(this.Entropy, Base64FormattingOptions.None);
            }
        }


        /// <summary>
        /// <see cref="StringProtectionScope"/> for encrypted data.
        /// </summary>
        private StringProtectionScope scope;


        /// <summary>
        /// pvivate Constuctor.
        /// </summary>
        private ProtectedString(StringProtectionScope scope)
        {
            this.scope = scope;
        }


        /// <summary>
        /// Creates instance from char array.
        /// </summary>
        /// <param name="chars">Char array to be encrypted.</param>
        /// <param name="entropyLength">Entropy length to be used on encrypting.</param>
        /// <param name="scope"><see cref="StringProtectionScope"/> for encrypted data.</param>
        /// <returns>ProtectedString instance.</returns>
        public static ProtectedString FromChars(char[] chars, int entropyLength = 128, StringProtectionScope scope = StringProtectionScope.CurrentUser)
        {
            var dps = DataProtectionScope.CurrentUser;

            if(scope == StringProtectionScope.LocalMachine)
            {
                dps = DataProtectionScope.LocalMachine;
            }

            var ps = new ProtectedString(scope);

            if(entropyLength > 0)
            {
                ps.Entropy = RAND.NextBytes(entropyLength);
            }

            ps.EncryptedData = ProtectedData.Protect(
                                    ENCODING.GetBytes(chars),
                                    ps.Entropy,
                                    dps);

            return ps;
        }

        /// <summary>
        /// Creates instance from string.
        /// </summary>
        /// <param name="str">string to be encrypted.</param>
        /// <param name="entropyLength">Entropy length to be used on encrypting.</param>
        /// <param name="scope"><see cref="StringProtectionScope"/> for encrypted data.</param>
        /// <returns>ProtectedString instance.</returns>
        public static ProtectedString FromString(string str, int entropyLength = 128, StringProtectionScope scope = StringProtectionScope.CurrentUser)
        {
            var chars = str.ToCharArray();

            var ps    = FromChars(chars, entropyLength, scope);

            if ( !ClearChars(chars) )
            {
                // Only for trying to bypass a future genius compiler optimization.
                return ps;
            }
            
            return ps;
        }


        /// <summary>
        /// Creates instance from encrypted data.
        /// </summary>
        /// <param name="encryptedData">Encrypted data to be decrypted.</param>
        /// <param name="entropy">Entropy to be used on decryption.</param>
        /// <param name="scope"><see cref="StringProtectionScope"/> for encrypted data.</param>
        /// <returns>ProtectedString instance.</returns>
        public static ProtectedString FromEncryptedData(byte[] encryptedData, byte[] entropy, StringProtectionScope scope = StringProtectionScope.CurrentUser)
        {
            var ps = new ProtectedString(scope);

            ps.EncryptedData = encryptedData;
            ps.Entropy       = entropy;

            return ps;
        }

        /// <summary>
        /// Creates instance from base64 encrypted data.
        /// </summary>
        /// <param name="encryptedDataBase64">Base64 encrypted data to be decrypted.</param>
        /// <param name="entropyBase64">Base64 entropy to be used on decryption.</param>
        /// <param name="scope"><see cref="StringProtectionScope"/> for encrypted data.</param>
        /// <returns>ProtectedString instance.</returns>
        public static ProtectedString FromEncryptedDataBase64(string encryptedDataBase64, string entropyBase64, StringProtectionScope scope = StringProtectionScope.CurrentUser)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedDataBase64);
            byte[] entropy       = null;

            if( !String.IsNullOrEmpty(entropyBase64) )
            {
                entropy = Convert.FromBase64String(entropyBase64);
            }

            return FromEncryptedData(encryptedData, entropy, scope);
        }


        /// <summary>
        /// Clears char array.
        /// Clearing char array which contains sensitive data in memory is better for security.
        /// However, for long-lifetime char array may be copied by managed memory manager.
        /// </summary>
        /// <param name="chars">Char array to be cleared.</param>
        /// <returns>Always returns true.</returns>
        public static bool ClearChars(char[] chars)
        {
            if(chars == null || chars.Length == 0)
            {
                return true;
            }


            // To prevent to be removed by a future genius compiler optimization,
            // gets and sets most of array item.
            // This is not perfect way.
            chars[0] = '\0';

            for (int i = 1; i < chars.Length; i++)
            {
                chars[i] = chars[i - 1];
            }

            // Only for trying to bypass a future genius compiler optimization.
            return (chars[RAND.NextInt(chars.Length)] == chars[RAND.NextInt(chars.Length)]);
        }


        /// <summary>
        /// Decrypts to char array.
        /// </summary>
        /// <returns>Decrypted char array.</returns>
        public char[] DecryptToChars()
        {
            var dps = DataProtectionScope.CurrentUser;

            if(this.scope == StringProtectionScope.LocalMachine)
            {
                dps = DataProtectionScope.LocalMachine;
            }

            return ENCODING.GetChars(ProtectedData.Unprotect(this.EncryptedData, this.Entropy, dps));
        }

        /// <summary>
        /// Decrypts to <see cref="SecureString"/>.
        /// </summary>
        /// <returns>Decrypted <see cref="SecureString"/>.</returns>
        public SecureString DecryptToSecureString()
        {
            var chars = DecryptToChars();

            var ss    = new SecureString();

            foreach (var item in chars)
            {
                ss.AppendChar(item);
            }

            if(ClearChars(chars))
            {
                // Here is always run because ClearChars() returns always true.
                ss.MakeReadOnly();
            }

            return ss;
        }

        /// <summary>
        /// Decrypts to string.
        /// </summary>
        /// <returns>Decrypted string.</returns>
        public string DecryptToString()
        {
            var chars = DecryptToChars();

            var str   = new String(chars);

            if( !ClearChars(chars) )
            {
                return str;
            }

            return str;
        }

    }

}
