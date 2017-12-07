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
        /// Cryptographic random number generator.
        /// </summary>
        private static readonly RNGCryptoServiceProvider RNGCSP = new RNGCryptoServiceProvider();




        /// <summary>
        /// Returns random byte array.
        /// </summary>
        /// <param name="byteLength">Length of byte array to be returned. byteLenght must be greater than 0 or equals to 0.</param>
        /// <returns>Random byte array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">byteLenght is less than 0.</exception>
        public byte[] NextBytes(int byteLength)
        {
            if(byteLength < 0)
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
        /// <param name="bytes">Bute array to be filled.</param>
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

                if(value == Int32.MinValue)
                {
                    value = 0;
                }

                value = Math.Abs(value);

            } while ( !isFairInt(value, maxValue) );

            return (value % maxValue);
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
