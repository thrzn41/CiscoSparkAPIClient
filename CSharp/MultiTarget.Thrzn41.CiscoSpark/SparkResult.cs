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
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Thrzn41.CiscoSpark.ResourceMessage;

namespace Thrzn41.CiscoSpark
{

    /// <summary>
    /// The result of Cisco API request.
    /// </summary>
    /// <typeparam name="TSparkObject">Spark Object that is returned on API request.</typeparam>
    public class SparkResult<TSparkObject>
        where TSparkObject : SparkObject, new()
    {

        /// <summary>
        /// The Spark Object data that is returned on the API request.
        /// </summary>
        public TSparkObject Data { get; internal set; }

        /// <summary>
        /// Indicats the request has been succeeded or not.
        /// </summary>
        public bool IsSuccessStatus { get; internal set; }

        /// <summary>
        /// Http status code returned on the API request.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; internal set; }

        /// <summary>
        /// Tracking id of the request.
        /// This id can be used for technical support.
        /// </summary>
        public string TrackingId { get; internal set; }

        /// <summary>
        /// Indicates whether the result has tracking id or not.
        /// </summary>
        public bool HasTrackingId
        {
            get
            {
                return ( !String.IsNullOrEmpty(this.TrackingId) );
            }
        }

        /// <summary>
        /// Retry-After header value.
        /// </summary>
        public RetryConditionHeaderValue RetryAfter { get; internal set; }

        /// <summary>
        /// Indicates the request has Retry-After header value.
        /// </summary>
        public bool HasRetryAfter {
            get
            {
                return (this.RetryAfter != null);
            }
        }




        /// <summary>
        /// Gets the Spark Object data that is returned on the API request.
        /// The <see cref="Data"/> property can be used to get the same data if you checked <see cref="IsSuccessStatus"/> property by yourself.
        /// This method throws <see cref="SparkResultException"/> when <see cref="IsSuccessStatus"/> is false and throwSparkResultExceptionOnErrors parameter is true.
        /// </summary>
        /// <param name="throwSparkResultExceptionOnErrors">true to throw <see cref="SparkResultException"/> when <see cref="IsSuccessStatus"/> is true.</param>
        /// <returns>The Spark Object data that is returned on the API request.</returns>
        /// <exception cref="SparkResultException"><see cref="IsSuccessStatus"/> is false.</exception>
        public TSparkObject GetData(bool throwSparkResultExceptionOnErrors = true)
        {
            if(throwSparkResultExceptionOnErrors)
            {
                ThrowSparkResultExceptionOnErrors();
            }

            return this.Data;
        }

        /// <summary>
        /// Throws <see cref="SparkResultException"/> if <see cref="IsSuccessStatus"/> is false.
        /// </summary>
        /// <exception cref="SparkResultException"><see cref="IsSuccessStatus"/> is false.</exception>
        public void ThrowSparkResultExceptionOnErrors()
        {
            if( !this.IsSuccessStatus )
            {
                string message = this.Data.GetErrorMessage();

                if(message == null)
                {
                    message = ErrorMessages.SparkResultError;
                }

                throw new SparkResultException(message, this.HttpStatusCode, this.TrackingId, this.RetryAfter);
            }
        }

    }

}
