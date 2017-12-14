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
using System.Threading;
using System.Threading.Tasks;

namespace Thrzn41.CiscoSpark
{

    /// <summary>
    /// The result of Cisco API list request.
    /// </summary>
    /// <typeparam name="TSparkObject">Spark Object that is returned on API request.</typeparam>
    public class SparkListResult<TSparkObject> : SparkResult<TSparkObject>
        where TSparkObject : SparkObject, new()
    {

        /// <summary>
        /// <see cref="SparkHttpClient"/> to get next result.
        /// </summary>
        internal SparkHttpClient SparkHttpClient { get; set; }

        /// <summary>
        /// Uri to get next result;
        /// </summary>
        internal Uri NextUri { get; set; }

        /// <summary>
        /// Indicates the next result exists or not.
        /// </summary>
        public bool HasNext
        {
            get
            {
                return (this.SparkHttpClient != null && this.NextUri != null);
            }
        }




        /// <summary>
        /// Lists result.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkListResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<TSparkObject> > ListNextAsync(CancellationToken? cancellationToken = null)
        {
            SparkListResult<TSparkObject> result = null;

            if (this.HasNext)
            {
                result = await this.SparkHttpClient.RequestJsonAsync<SparkListResult<TSparkObject>, TSparkObject>(
                                    HttpMethod.Get,
                                    this.NextUri,
                                    null,
                                    null,
                                    cancellationToken);
            }
            else
            {
                result = new SparkListResult<TSparkObject>();

                result.Data = new TSparkObject();
                result.Data.HasValues = false;
            }

            return result;
        }

    }

}
