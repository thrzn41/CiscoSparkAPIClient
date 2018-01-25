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
using System.Text;
using Thrzn41.Util;

namespace Thrzn41.CiscoSpark
{

    /// <summary>
    /// Class to create Spark API clients.
    /// </summary>
    public static class SparkAPI
    {

        /// <summary>
        /// Creates Spark API client for v1 API.
        /// </summary>
        /// <param name="tokenProtected">spark token of <see cref="ProtectedString"/></param>
        /// <returns>Spark API client for v1 API.</returns>
        public static Thrzn41.CiscoSpark.Version1.SparkAPIClient CreateVersion1Client(ProtectedString tokenProtected)
        {
            return CreateVersion1Client( tokenProtected.DecryptToString() );
        }

        /// <summary>
        /// Creates Spark API client for v1 API.
        /// </summary>
        /// <param name="tokenChars">spark token of char array.</param>
        /// <returns>Spark API client for v1 API.</returns>
        public static Thrzn41.CiscoSpark.Version1.SparkAPIClient CreateVersion1Client(char[] tokenChars)
        {
            return CreateVersion1Client( new String(tokenChars) );
        }

        /// <summary>
        /// Creates Spark API client for v1 API.
        /// </summary>
        /// <param name="tokenString">spark token of string.</param>
        /// <returns>Spark API client for v1 API.</returns>
        public static Thrzn41.CiscoSpark.Version1.SparkAPIClient CreateVersion1Client(string tokenString)
        {
            return new Thrzn41.CiscoSpark.Version1.SparkAPIClient(tokenString);
        }


        /// <summary>
        /// Creates Spark Admin API client for v1 API.
        /// </summary>
        /// <param name="tokenProtected">spark token of <see cref="ProtectedString"/></param>
        /// <returns>Spark Admin API client for v1 API.</returns>
        public static Thrzn41.CiscoSpark.Version1.Admin.SparkAdminAPIClient CreateVersion1AdminClient(ProtectedString tokenProtected)
        {
            return CreateVersion1AdminClient(tokenProtected.DecryptToString());
        }

        /// <summary>
        /// Creates Spark Admin API client for v1 API.
        /// </summary>
        /// <param name="tokenChars">spark token of char array.</param>
        /// <returns>Spark Admin API client for v1 API.</returns>
        public static Thrzn41.CiscoSpark.Version1.Admin.SparkAdminAPIClient CreateVersion1AdminClient(char[] tokenChars)
        {
            return CreateVersion1AdminClient(new String(tokenChars));
        }

        /// <summary>
        /// Creates Spark Admin API client for v1 API.
        /// </summary>
        /// <param name="tokenString">spark token of string.</param>
        /// <returns>Spark Admin API client for v1 API.</returns>
        public static Thrzn41.CiscoSpark.Version1.Admin.SparkAdminAPIClient CreateVersion1AdminClient(string tokenString)
        {
            return new Thrzn41.CiscoSpark.Version1.Admin.SparkAdminAPIClient(tokenString);
        }


        /// <summary>
        /// Creates Spark OAuth2 client for v1 API.
        /// </summary>
        /// <param name="clientSecretProtected">Client secret of <see cref="ProtectedString"/>.</param>
        /// <param name="clientId">Client id.</param>
        /// <returns>Spark OAuth2 client for v1 API.</returns>
        public static Thrzn41.CiscoSpark.Version1.OAuth2.SparkOAuth2Client CreateVersion1OAuth2Client(ProtectedString clientSecretProtected, string clientId)
        {
            return new Thrzn41.CiscoSpark.Version1.OAuth2.SparkOAuth2Client(clientSecretProtected.DecryptToString(), clientId);
        }

        /// <summary>
        /// Creates Spark OAuth2 client for v1 API.
        /// </summary>
        /// <param name="clientSecretChars">Client secret of char array.</param>
        /// <param name="clientId">Client id.</param>
        /// <returns>Spark OAuth2 client for v1 API.</returns>
        public static Thrzn41.CiscoSpark.Version1.OAuth2.SparkOAuth2Client CreateVersion1OAuth2Client(char[] clientSecretChars, string clientId)
        {
            return new Thrzn41.CiscoSpark.Version1.OAuth2.SparkOAuth2Client(new string(clientSecretChars), clientId);
        }

        /// <summary>
        /// Creates Spark OAuth2 client for v1 API.
        /// </summary>
        /// <param name="clientSecretString">Client secret of string.</param>
        /// <param name="clientId">Client id.</param>
        /// <returns>Spark OAuth2 client for v1 API.</returns>
        public static Thrzn41.CiscoSpark.Version1.OAuth2.SparkOAuth2Client CreateVersion1OAuth2Client(string clientSecretString, string clientId)
        {
            return new Thrzn41.CiscoSpark.Version1.OAuth2.SparkOAuth2Client(clientSecretString, clientId);
        }

    }

}
