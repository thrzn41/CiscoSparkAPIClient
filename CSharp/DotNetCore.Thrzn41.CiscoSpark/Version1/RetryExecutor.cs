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
using System.Threading;
using System.Threading.Tasks;

namespace Thrzn41.CiscoSpark.Version1
{

    /// <summary>
    /// Retry executor to retry requests.
    /// </summary>
    public class RetryExecutor : SparkRetry
    {

        /// <summary>
        /// This executor will retry once at most, if needed.
        /// This is recomended, because it will be something wrong with Spark API service, if twice or more retry-after are returned.
        /// </summary>
        public static readonly RetryExecutor One = new RetryExecutor(1);

        /// <summary>
        /// This executor will retry twice at most, if needed.
        /// This is NOT recomended, because it will be something wrong with Spark API service, if twice or more retry-after are returned.
        /// </summary>
        public static readonly RetryExecutor Two = new RetryExecutor(2);

        /// <summary>
        /// This executor will retry twice at most, if needed.
        /// This is NOT recomended, because it will be something wrong with Spark API service, if twice or more retry-after are returned.
        /// </summary>
        public static readonly RetryExecutor Three = new RetryExecutor(3);

        /// <summary>
        /// This executor will retry twice at most, if needed.
        /// This is NOT recomended, because it will be something wrong with Spark API service, if twice or more retry-after are returned.
        /// </summary>
        public static readonly RetryExecutor Four = new RetryExecutor(4);




        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + (retry-after * weight) + buffer).
        /// </summary>
        /// <param name="retryMax">Max retry count.</param>
        /// <param name="buffer">Buffer duration for retry.</param>
        /// <param name="weight">Weight value for retry.</param>
        public RetryExecutor(int retryMax, TimeSpan buffer, double weight = 0.0d)
            : base(retryMax, buffer, weight)
        {
        }

        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + 250ms).
        /// </summary>
        public RetryExecutor()
            : base()
        {
        }

        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + 250ms).
        /// </summary>
        /// <param name="retryMax">Max retry count.</param>
        public RetryExecutor(int retryMax)
            : base(retryMax)
        {
        }


        /// <summary>
        /// Requests with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="SparkResult{TSparkObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <typeparam name="TSparkData">Type of SparkData to be returned.</typeparam>
        /// <param name="sparkRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> that represents result of API request.</returns>
        public async Task< SparkResult<TSparkData> > RequestAsync<TSparkData>(Func< Task< SparkResult<TSparkData> > > sparkRequestFunc, Func<SparkResult<TSparkData>, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
            where TSparkData : SparkData, new()
        {
            return (await this.requestAsync<SparkResult<TSparkData>, TSparkData>(sparkRequestFunc, notificationFunc, cancellationToken));
        }


        /// <summary>
        /// Lists with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="SparkResult{TSparkObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <typeparam name="TSparkListData">Type of SparkData to be returned.</typeparam>
        /// <param name="sparkRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="SparkListData{TSparkObject}"/> that represents result of API request.</returns>
        public async Task< SparkListResult<TSparkListData> > ListAsync<TSparkListData>(Func< Task< SparkListResult<TSparkListData> > > sparkRequestFunc, Func<SparkListResult<TSparkListData>, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
            where TSparkListData : SparkData, new()
        {
            return (await this.requestAsync<SparkListResult<TSparkListData>, TSparkListData>(sparkRequestFunc, notificationFunc, cancellationToken));
        }


        /// <summary>
        /// Deletes with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="SparkResult{TSparkObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <param name="sparkRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> that represents result of API request.</returns>
        public async Task< SparkResult<NoContent> > DeleteAsync(Func< Task< SparkResult<NoContent> > > sparkRequestFunc, Func<SparkResult<NoContent>, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
        {
            return (await this.requestAsync<SparkResult<NoContent>, NoContent>(sparkRequestFunc, notificationFunc, cancellationToken));
        }


    }

}
