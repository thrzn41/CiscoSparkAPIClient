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
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Thrzn41.Util;

namespace Thrzn41.CiscoSpark
{

    /// <summary>
    /// HttpClient for Cisco Spark API.
    /// </summary>
    public class SparkHttpClient : IDisposable
    {

        /// <summary>
        /// Media type value for application/json.
        /// </summary>
        private const string MEDIA_TYPE_APPLICATION_JSON = "application/json";

        /// <summary>
        /// Media type value for any type.
        /// </summary>
        private const string MEDIA_TYPE_ANY = "*/*";


        /// <summary>
        /// Default encoding in this class.
        /// </summary>
        protected static readonly Encoding ENCODING = UTF8Utils.UTF8_WITHOUT_BOM;


        /// <summary>
        /// Header name of TrackingID.
        /// </summary>
        private const string HEADER_NAME_TRACKING_ID = "TrackingID";

        /// <summary>
        /// Header name of Link.
        /// </summary>
        private const string HEADER_NAME_LINK = "Link";


        /// <summary>
        /// Next link pattern.
        /// </summary>
        private readonly static Regex LINK_NEXT_PATTERN = new Regex(".*<(?<NEXTURI>(https://.*?))>.*?rel=\"next\"", RegexOptions.Compiled, TimeSpan.FromSeconds(60.0f));


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
        internal SparkHttpClient(string sparkToken, Regex sparkAPIUriPattern)
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
        /// Requests to Cisco Spark API.
        /// </summary>
        /// <typeparam name="TSparkResult">Type of SparkResult to be returned.</typeparam>
        /// <typeparam name="TSparkObject">Type of SparkObject to be returned.</typeparam>
        /// <param name="request"><see cref="HttpRequestMessage"/> to be requested.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> that represents result of API request.</returns>
        public async Task<TSparkResult> RequestAsync<TSparkResult, TSparkObject>(HttpRequestMessage request, CancellationToken? cancellationToken = null)
            where TSparkResult : SparkResult<TSparkObject>, new()
            where TSparkObject : SparkObject, new()
        {
            var result = new TSparkResult();

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
                    if (response.StatusCode != System.Net.HttpStatusCode.NoContent && response.Content != null)
                    {
                        using (var content = response.Content)
                        {
                            var contentHeaders = content.Headers;

                            if ( contentHeaders.ContentType?.MediaType == MEDIA_TYPE_APPLICATION_JSON )
                            {
                                var body = await content.ReadAsStringAsync();

                                if ( !String.IsNullOrEmpty(body) )
                                {
                                    result.Data = SparkObject.FromJsonString<TSparkObject>(body);
                                }
                            }
                            else
                            {
                                var info = new TSparkObject() as SparkFileInfo;

                                if (info != null)
                                {
                                    info.FileName      = contentHeaders.ContentDisposition?.FileName;
                                    info.MediaTypeName = contentHeaders.ContentType?.MediaType;
                                    info.Size          = contentHeaders.ContentLength;
                                }

                                var data = info as SparkFileData;

                                if (data != null)
                                {
                                    data.Stream = new MemoryStream();

                                    await content.CopyToAsync(data.Stream);

                                    data.Stream.Position = 0;
                                }

                                result.Data = ((data != null) ? data : info) as TSparkObject;
                            }
                        }
                    }


                    if(result.Data == null)
                    {
                        result.Data = new TSparkObject();

                        result.Data.HasValues = false;
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

                    result.RetryAfter = headers.RetryAfter;


                    if(result is SparkListResult<TSparkObject>)
                    {
                        if (headers.Contains(HEADER_NAME_LINK))
                        {
                            var listResult = result as SparkListResult<TSparkObject>;

                            if (listResult != null)
                            {
                                foreach (var item in headers.GetValues(HEADER_NAME_LINK))
                                {
                                    if( !String.IsNullOrEmpty(item) )
                                    {
                                        var m = LINK_NEXT_PATTERN.Match(item);
                                        
                                        if(m.Success)
                                        {
                                            var g = m.Groups["NEXTURI"];

                                            if( g != null && !String.IsNullOrEmpty(g.Value) )
                                            {
                                                listResult.NextUri         = new Uri(g.Value);
                                                listResult.SparkHttpClient = this;

                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }

                    // Result status is once set based on Http Status code.
                    // The exact criteria differs in each API.
                    // This value will be adjusted in each SparkAPIClient class.
                    result.IsSuccessStatus = response.IsSuccessStatusCode;
                }
            }

            return result;
        }

        /// <summary>
        /// Requests to Cisco Spark API.
        /// </summary>
        /// <typeparam name="TSparkResult">Type of SparkResult to be returned.</typeparam>
        /// <typeparam name="TSparkObject">Type of SparkObject to be returned.</typeparam>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="objectToBePosted">Object inherited from <see cref="SparkObject"/> to be sent to Spark API.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> that represents result of API request.</returns>
        public async Task<TSparkResult> RequestJsonAsync<TSparkResult, TSparkObject>(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, SparkObject objectToBePosted = null, CancellationToken? cancellationToken = null)
            where TSparkResult : SparkResult<TSparkObject>, new()
            where TSparkObject : SparkObject, new()
        {
            return (await RequestAsync<TSparkResult, TSparkObject>(CreateJsonRequest(method, uri, queryParameters, objectToBePosted), cancellationToken));
        }

        /// <summary>
        /// Requests file info/data to Cisco Spark API.
        /// </summary>
        /// <typeparam name="TSparkResult">Type of SparkResult to be returned.</typeparam>
        /// <typeparam name="TSparkFileInfo">Type of SparkFileInfo to be returned.</typeparam>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> that represents result of API request.</returns>
        public async Task<TSparkResult> RequestFileInfoAsync<TSparkResult, TSparkFileInfo>(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, CancellationToken? cancellationToken = null)
            where TSparkResult   : SparkResult<TSparkFileInfo>, new()
            where TSparkFileInfo : SparkFileInfo, new()
        {
            return (await RequestAsync<TSparkResult, TSparkFileInfo>(CreateFileInfoRequest(method, uri, queryParameters), cancellationToken));
        }

        /// <summary>
        /// Requests MultipartFormData to Cisco Spark API.
        /// </summary>
        /// <typeparam name="TSparkResult">Type of SparkResult to be returned.</typeparam>
        /// <typeparam name="TSparkObject">Type of SparkObject to be returned.</typeparam>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="stringData">String data collection.</param>
        /// <param name="fileData">File data list.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> that represents result of API request.</returns>
        public async Task<TSparkResult> RequestMultipartFormDataAsync<TSparkResult, TSparkObject>(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, NameValueCollection stringData = null, SparkFileData fileData = null, CancellationToken? cancellationToken = null)
            where TSparkResult : SparkResult<TSparkObject>, new()
            where TSparkObject : SparkObject, new()
        {
            return (await RequestAsync<TSparkResult, TSparkObject>(CreateMultipartFormDataRequest(method, uri, queryParameters, stringData, fileData), cancellationToken));
        }


        /// <summary>
        /// Creates <see cref="HttpRequestMessage"/> to use for Json request.
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="objectToBePosted">Object inherited from <see cref="SparkObject"/> to be sent to Spark API.</param>
        /// <returns><see cref="HttpRequestMessage"/> that is created.</returns>
        public HttpRequestMessage CreateJsonRequest(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, SparkObject objectToBePosted = null)
        {
            var request = new HttpRequestMessage(method, HttpUtils.BuildUri(uri, queryParameters));

            var headers = request.Headers;

            headers.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(ENCODING.WebName));
            headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MEDIA_TYPE_APPLICATION_JSON));

            if (objectToBePosted != null)
            {
                request.Content = new StringContent(
                    objectToBePosted.ToJsonString(),
                    ENCODING,
                    MEDIA_TYPE_APPLICATION_JSON);
            }

            return request;
        }


        /// <summary>
        /// Creates <see cref="HttpRequestMessage"/> to use for File info request.
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <returns><see cref="HttpRequestMessage"/> that is created.</returns>
        public HttpRequestMessage CreateFileInfoRequest(HttpMethod method, Uri uri, NameValueCollection queryParameters = null)
        {
            var request = new HttpRequestMessage(method, HttpUtils.BuildUri(uri, queryParameters));

            var headers = request.Headers;

            headers.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(ENCODING.WebName));
            headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MEDIA_TYPE_APPLICATION_JSON));
            headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MEDIA_TYPE_ANY));

            return request;
        }


        /// <summary>
        /// Creates <see cref="HttpRequestMessage"/> to use for MultipartFormData request.
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="stringData">String data collection.</param>
        /// <param name="fileData">File data list.</param>
        /// <returns><see cref="HttpRequestMessage"/> that is created.</returns>
        public HttpRequestMessage CreateMultipartFormDataRequest(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, NameValueCollection stringData = null, SparkFileData fileData = null)
        {
            var request = new HttpRequestMessage(method, HttpUtils.BuildUri(uri, queryParameters));

            var headers = request.Headers;

            headers.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(ENCODING.WebName));
            headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MEDIA_TYPE_APPLICATION_JSON));

            var content = new MultipartFormDataContent();

            if(stringData != null)
            {
                foreach (var key in stringData.AllKeys)
                {
                    var values = stringData.GetValues(key);

                    if (values != null)
                    {
                        foreach (var item in values)
                        {
                            if (item != null)
                            {
                                content.Add(new StringContent(item, ENCODING), key);
                            }
                        }
                    }
                }
            }

            if(fileData != null)
            {
                var sc = new StreamContent(fileData.Stream);

                sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(fileData.MediaTypeName);

                content.Add(sc, "files", fileData.FileName);
            }

            request.Content = content;

            return request;
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
