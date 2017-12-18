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
using System.Collections.Specialized;
using System.Text;

namespace Thrzn41.Util
{

    /// <summary>
    /// Utils for Web.
    /// </summary>
    public static class HttpUtils
    {

        /// <summary>
        /// Builds Query paramerters from <see cref="NameValueCollection"/>.
        /// </summary>
        /// <param name="queryParameters"><see cref="NameValueCollection"/> that contains key/value pair of query parameters.</param>
        /// <returns>string of query parameters.</returns>
        public static string BuildQueryParameters(NameValueCollection queryParameters)
        {
            if(queryParameters == null || queryParameters.Count == 0)
            {
                return String.Empty;
            }

            var strs = new StringBuilder();

            var separator = String.Empty;

            foreach (var key in queryParameters.AllKeys)
            {
                var encodedKey = Uri.EscapeDataString(key);
                var values     = queryParameters.GetValues(key);

                if (values != null)
                {
                    foreach (var item in values)
                    {
                        if (item != null)
                        {
                            strs.AppendFormat("{0}{1}={2}", separator, encodedKey, Uri.EscapeDataString(item));
                            separator = "&";
                        }
                    }
                }
            }

            return strs.ToString();
        }

        /// <summary>
        /// Builds https or http uri from <see cref="Uri"/> and <see cref="NameValueCollection"/>.
        /// This method does not care of fragment part of uri.
        /// </summary>
        /// <param name="baseUri">Base <see cref="Uri"/> of https or http.</param>
        /// <param name="queryParameters"><see cref="NameValueCollection"/> that contains key/value pair of query parameters.</param>
        /// <returns>Uri with query parameters.</returns>
        /// <exception cref="ArgumentException">Uri scheme is not https or http.</exception>
        public static Uri BuildUri(Uri baseUri, NameValueCollection queryParameters)
        {
            if( (baseUri.Scheme != Uri.UriSchemeHttps) && (baseUri.Scheme != Uri.UriSchemeHttp) )
            {
                throw new ArgumentException(ResourceMessage.ErrorMessages.UriSchemeIsNotHttpsOrHttp, "baseUri");
            }

            var result = baseUri;

            if(queryParameters != null)
            {
                var queryParamsString = BuildQueryParameters(queryParameters);
                
                if( !String.IsNullOrEmpty(queryParamsString) )
                {
                    var separator = "?";

                    if( !String.IsNullOrEmpty(baseUri.Query) )
                    {
                        separator = "&";
                    }

                    result = new Uri(String.Format("{0}{1}{2}", baseUri.GetLeftPart(UriPartial.Query), separator, queryParamsString));
                }
            }

            return result;
        }

    }

}
