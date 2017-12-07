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
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Thrzn41.CiscoSpark
{

    /// <summary>
    /// HttpClient for Cisco Spark API.
    /// </summary>
    internal class SparkHttpClient : IDisposable
    {

        /// <summary>
        /// Media type value for application/json.
        /// </summary>
        private const string MEDIA_TYPE_APPLICATION_JSON = "application/json";


        /// <summary>
        /// Header name of TrackingID.
        /// </summary>
        private const string HEADER_NAME_TRACKING_ID = "TrackingID";




        /// <summary>
        /// HttpClient to access Cisco Spark API uris.
        /// </summary>
        private readonly HttpClient httpClientForSparkAPI;

        /// <summary>
        /// HttpClient to access non-Cisco Spark API uris.
        /// </summary>
        private readonly HttpClient httpClientForNonSparkAPI;


        /// <summary>
        /// Regex pattern to check if the Uri is Cisco Spark API uris.
        /// </summary>
        private readonly Regex sparkAPIUriPattern;




        /// <summary>
        /// SparkHttpClient constructor.
        /// </summary>
        /// <param name="sparkToken">Cisco Spark Token.</param>
        /// <param name="sparkAPIUriPattern">Regex pattern to check if the Uri is Cisco Spark API uris.</param>
        public SparkHttpClient(string sparkToken, Regex sparkAPIUriPattern)
        {
            // HttpClient for Cisco Spark API.
            // Spark Token MUST be sent to only Spark API https URL.
            // NEVER SEND Token to other URLs or non-secure http URL.
            this.httpClientForSparkAPI = new HttpClient(
                    new HttpClientHandler
                    {
                        AllowAutoRedirect = false,
                        PreAuthenticate   = true,
                    },
                    true
                );

            // HttpClient for non-Cisco Spark API URL.
            // An avator image path is well-known case.
            this.httpClientForNonSparkAPI = new HttpClient(
                    new HttpClientHandler
                    {
                        AllowAutoRedirect = false,
                        PreAuthenticate   = false,
                    },
                    true
                );


            // For Cisco Spark API https path, the token is added.
            this.httpClientForSparkAPI.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sparkToken);

            this.sparkAPIUriPattern = sparkAPIUriPattern;

        }


        /// <summary>
        /// Selects HttpClient for uri.
        /// </summary>
        /// <param name="uri">Uri to be requested.</param>
        /// <returns>HttpClient for uri.</returns>
        private HttpClient selectHttpClient(Uri uri)
        {
            var result = this.httpClientForNonSparkAPI;

            if(this.sparkAPIUriPattern.IsMatch(uri.AbsoluteUri))
            {
                result = this.httpClientForSparkAPI;
            }

            return result;
        }


        /// <summary>
        /// Requests to Cisco Spark API path.
        /// </summary>
        /// <typeparam name="TSparkObject">Type of SparkObject to be returned.</typeparam>
        /// <param name="request"><see cref="HttpRequestMessage"/> to be requested.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> that represents result of API request.</returns>
        public async Task< SparkResult<TSparkObject> > RequestAsync<TSparkObject>(HttpRequestMessage request, CancellationToken? cancellationToken = null)
            where TSparkObject : SparkObject, new()
        {
            var result = new SparkResult<TSparkObject>();

            using (request)
            using (request.Content)
            {
                var httpClient = selectHttpClient(request.RequestUri);

                HttpResponseMessage response;

                if (cancellationToken.HasValue)
                {
                    response = await httpClient.SendAsync(request, cancellationToken.Value);
                }
                else
                {
                    response = await httpClient.SendAsync(request);
                }

                using (response)
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                    {
                        using (var content = response.Content)
                        {
                            var contentHeaders = content.Headers;

                            if (contentHeaders.ContentType.MediaType == MEDIA_TYPE_APPLICATION_JSON)
                            {
                                var body = await content.ReadAsStringAsync();

                                if (!String.IsNullOrEmpty(body))
                                {
                                    result.Data = SparkObject.FromJsonString<TSparkObject>(body);
                                }
                            }
                        }
                    }


                    result.HttpStatusCode = response.StatusCode;

                    var headers = response.Headers;

                    if (headers.Contains(HEADER_NAME_TRACKING_ID))
                    {
                        foreach (var item in headers.GetValues(HEADER_NAME_TRACKING_ID))
                        {
                            result.TrackingId = item;
                            break;
                        }
                    }

                    result.RetryAfter = headers.RetryAfter?.Delta;

                    // Result status is once set based on Http Status code.
                    // The exact criteria differs in each API.
                    // This value will be adjusted in each SparkAPIClient class.
                    result.IsSuccessStatus = response.IsSuccessStatusCode;
                }
            }

            return result;
        }

        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using (this.httpClientForSparkAPI)
                    using (this.httpClientForNonSparkAPI)
                    {
                        // disposed.
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SparkHttpClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}
