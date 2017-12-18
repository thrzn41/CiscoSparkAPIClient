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
using System.Security.Cryptography;
using System.Text;

namespace Thrzn41.Util
{

    /// <summary>
    /// Provides features for generating cryptographic ramdom number.
    /// </summary>
    public class CryptoRandom
    {

        /// <summary>
        /// ASCII category to be used <see cref="GetASCIIChars(int, ASCIICategory)"./>
        /// For now, symbols are not supported.
        /// </summary>
        [Flags]
        public enum ASCIICategory
        {
            /// <summary>
            /// None.
            /// </summary>
            None   = 0x00,

            /// <summary>
            /// Upper ASCII Alphabet.
            /// </summary>
            UpperAlphabet = 0x01,

            /// <summary>
            /// Lower ASCII Alphabet.
            /// </summary>
            LowerAlphabet = 0x02,

            /// <summary>
            /// ASCII Numbers.
            /// </summary>
            Number = 0x04,
        }




        /// <summary>
        /// Cryptographic random number generator.
        /// </summary>
        private static readonly RNGCryptoServiceProvider RNGCSP = new RNGCryptoServiceProvider();


        /// <summary>
        /// ASCII upper chars.
        /// </summary>
        private static readonly char[] ASCII_UPPERS  = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// ASCII lower chars.
        /// </summary>
        private static readonly char[] ASCII_LOWERS  = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        /// <summary>
        /// ASCII number chars.
        /// </summary>
        private static readonly char[] ASCII_NUMBERS = "0123456789".ToCharArray();


        /// <summary>
        /// Returns random byte array.
        /// </summary>
        /// <param name="byteLength">Length of byte array to be returned. byteLenght must be greater than 0 or equals to 0.</param>
        /// <returns>Random byte array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">byteLenght is less than 0.</exception>
        public byte[] NextBytes(int byteLength)
        {

            if (byteLength < 0)
            {
                throw new ArgumentOutOfRangeException("byteLength", ResourceMessage.ErrorMessages.ByteLengthLessThanZero);
            }

            var bytes = new byte[byteLength];

            if (byteLength > 0)
            {
                RNGCSP.GetBytes(bytes);
            }

            return bytes;
        }

        /// <summary>
        /// Fills byte array with random byte.
        /// </summary>
        /// <param name="bytes">Byte array to be filled.</param>
        public void FillBytes(byte[] bytes)
        {
            RNGCSP.GetBytes(bytes);
        }


        /// <summary>
        /// Returns a non-negative int that is less than maxValue. 
        /// </summary>
        /// <param name="maxValue">MaxValue to be returned.</param>
        /// <returns>Non-negative int that is less than maxValue.</returns>
        public int NextInt(int maxValue)
        {
            if(maxValue < 0)
            {
                throw new ArgumentOutOfRangeException("maxValue", ResourceMessage.ErrorMessages.MaxValueLessThanZero);
            }

            if(maxValue <= 1)
            {
                return 0;
            }

            int value;
            var bytes = new byte[4];

            do
            {
                FillBytes(bytes);

                value = BitConverter.ToInt32(bytes, 0);

                // To be fair, there should be 'two' 0 and no Int32.MinValue.
                if(value == Int32.MinValue)
                {
                    value = 0;
                }

                value = Math.Abs(value);

            } while ( !isFairInt(value, maxValue) );

            return (value % maxValue);
        }


        /// <summary>
        /// Returns random ASCII char array.
        /// </summary>
        /// <param name="charLength">Length of byte array to be returned. byteLenght must be greater than 0 or equals to 0.</param>
        /// <param name="category"><see cref="ASCIICategory"/> that is returned in char array.</param>
        /// <returns>Random ASCII char array.</returns>
        public char[] GetASCIIChars(int charLength, ASCIICategory category = (ASCIICategory.UpperAlphabet | ASCIICategory.LowerAlphabet | ASCIICategory.Number))
        {
            if (charLength < 0)
            {
                throw new ArgumentOutOfRangeException("charLength", ResourceMessage.ErrorMessages.CharLengthLessThanZero);
            }

            var chars = new char[charLength];

            if (charLength > 0)
            {
                var sourceList = new List<char>();

                if (category.HasFlag(ASCIICategory.UpperAlphabet))
                {
                    sourceList.AddRange(ASCII_UPPERS);
                }

                if (category.HasFlag(ASCIICategory.LowerAlphabet))
                {
                    sourceList.AddRange(ASCII_LOWERS);
                }

                if (category.HasFlag(ASCIICategory.Number))
                {
                    sourceList.AddRange(ASCII_NUMBERS);
                }


                if (sourceList.Count > 0)
                {
                    var source = sourceList.ToArray();

                    for (int i = 0; i < charLength; i++)
                    {
                        chars[i] = source[NextInt(source.Length)];
                    }
                }

            }

            return chars;
        }


        /// <summary>
        /// Checks if the value is fair or not.
        /// </summary>
        /// <param name="value">Value to be checked.</param>
        /// <param name="maxValue">MaxValue to be checked.</param>
        /// <returns>true if fair, false if not fair.</returns>
        private bool isFairInt(int value, int maxValue)
        {
            if (value < 0)
            {
                return false;
            }

            return ( value < ((Int32.MaxValue / maxValue) * maxValue) );
        }

    }

}
