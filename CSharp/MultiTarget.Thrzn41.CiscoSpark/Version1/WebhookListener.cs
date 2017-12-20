﻿/* 
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
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Thrzn41.Util;

namespace Thrzn41.CiscoSpark.Version1
{

    /// <summary>
    /// Provides simple webhook listener.
    /// This listener is NOT INTENDED TO BE USED IN PRODUCTION ENVIRONMENT.
    /// Mainly focus on use in quick testing or trying webhook.
    /// This listner consumes user thread pool.
    /// </summary>
    /// <remarks>
    /// This listener is NOT INTENDED TO BE USED IN PRODUCTION ENVIRONMENT.
    /// Mainly focus on use in quick testing or trying webhook.
    /// This listner consumes user thread pool.
    /// </remarks>
    public class WebhookListener : IDisposable
    {

        /// <summary>
        /// Default data buffue bytes size for event data.
        /// The 2048 bytes will be reasonable size for most event data.
        /// </summary>
        private const int DEFAULT_DATA_BUFFER_SIZE = 2048;

        /// <summary>
        /// Default number of listener threads.
        /// </summary>
        private const int DEFAULT_NUMBER_OF_THREADS = 16;

        /// <summary>
        /// Path prefix pattern of path in uri.
        /// </summary>
        private const string PATH_PREFIX_PATTERN = "/webhook/notify/{0}/";

        /// <summary>
        /// Pattern of path in uri.
        /// </summary>
        private const string PATH_PATTERN = "{0}events";


        /// <summary>
        /// Default encoding.
        /// </summary>
        private static readonly Encoding DEFAULT_ENCODING = UTF8Utils.UTF8_WITHOUT_BOM;



        /// <summary>
        /// Slim lock.
        /// </summary>
        private SlimLock slimLock;

        /// <summary>
        /// Notification manager.
        /// </summary>
        private WebhookNotificationManager notificationManager;

        /// <summary>
        /// Http listner.
        /// </summary>
        private HttpListener httpListener;

        /// <summary>
        /// Number of listner threads.
        /// </summary>
        private int numberOfThreads;

        /// <summary>
        /// Path prefix that is listened.
        /// </summary>
        private string pathPrefix;




        /// <summary>
        /// Path that is listened.
        /// </summary>
        public string Path { get; private set; }




        /// <summary>
        /// Creates webhook listner.
        /// </summary>
        /// <param name="numberOfThreads">Number of listener threads.</param>
        public WebhookListener(int numberOfThreads = DEFAULT_NUMBER_OF_THREADS)
        {
            this.slimLock            = new SlimLock();
            this.notificationManager = new WebhookNotificationManager();
            this.httpListener        = new HttpListener();

            this.httpListener.IgnoreWriteExceptions = true;

            this.numberOfThreads = numberOfThreads;
            
            this.pathPrefix = String.Format(PATH_PREFIX_PATTERN, Guid.NewGuid().ToString("N"));
            this.Path       = String.Format(PATH_PATTERN,        this.pathPrefix);
        }


        /// <summary>
        /// Adds listening host and port.
        /// </summary>
        /// <param name="hostnameOrIPAddress">Hostname or IP address of local machine.</param>
        /// <param name="port">Port number of the listening port.</param>
        /// <param name="useHttps">true if the listening uri uses https.</param>
        /// <returns><see cref="Uri"/> that is added.</returns>
        public Uri AddListenerEndpoint(string hostnameOrIPAddress, ushort port, bool useHttps = true)
        {
            string scheme = "https";

            if( !useHttps )
            {
                scheme = "http";
            }

            if(hostnameOrIPAddress == "localhost")
            {
                hostnameOrIPAddress = "127.0.0.1";
            }


            var prefix = String.Format("{0}://{1}:{2}{3}", scheme, hostnameOrIPAddress, port, this.pathPrefix);
            var result = new Uri(String.Format(PATH_PATTERN, prefix));

            this.httpListener.Prefixes.Add(prefix);

            return result;
        }


        /// <summary>
        /// Adds event notification to the listner.
        /// </summary>
        /// <param name="webhook">Webhook to be added.</param>
        /// <param name="func">Function to be notified on receiving event.</param>
        public void AddWebhookNotification(Webhook webhook, Action<WebhookEventData> func)
        {
            this.notificationManager.AddNotification(webhook, func);
        }

        /// <summary>
        /// Adds event notification to the listner.
        /// </summary>
        /// <param name="webhook">Webhook to be added.</param>
        /// <param name="funcAsync">Async function to be notified on receiving event.</param>
        public void AddWebhookNotification(Webhook webhook, Func<WebhookEventData, Task> funcAsync)
        {
            this.notificationManager.AddNotification(webhook, funcAsync);
        }


        /// <summary>
        /// Removes notification info.
        /// </summary>
        /// <param name="webhook">Webhook to be removed.</param>
        public void RemoveWebhookNotification(Webhook webhook)
        {
            this.notificationManager.RemoveNotification(webhook);
        }


        /// <summary>
        /// Starts listening.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        private async Task listen()
        {
            while(this.httpListener.IsListening)
            {
                var context = await this.httpListener.GetContextAsync();

                var request  = context.Request;
                var response = context.Response;

                byte[]   data            = null;
                string   xSparkSignature = null;
                Encoding encoding        = null;

                try
                {
                    // Always responds 204 even if there seems to be invalid request.
                    // Any hints are not provided, because the webhook is open for public.
                    response.StatusCode = 204;

                    // Only "POST" method is allowed.
                    if(request.HttpMethod == "POST" && request.HasEntityBody)
                    {
                        int contentLength = Convert.ToInt32(request.ContentLength64);

                        using (var memory = new MemoryStream((contentLength > 0) ? contentLength : DEFAULT_DATA_BUFFER_SIZE))
                        using (var stream = request.InputStream)
                        {
                            await stream.CopyToAsync(memory);

                            data = memory.ToArray();
                        }

                        var xSparkSignatureHeaders = request.Headers.GetValues("X-Spark-Signature");

                        if(xSparkSignatureHeaders != null)
                        {
                            foreach (var item in xSparkSignatureHeaders)
                            {
                                xSparkSignature = item;
                                break;
                            }
                        }

                        encoding = request.ContentEncoding;
                    }
                }
                finally
                {
                    response.Close();
                }

                
                if(data != null && data.Length > 0)
                {
                    this.notificationManager.ValidateAndNotify(data, xSparkSignature, ((encoding != null) ? encoding : DEFAULT_ENCODING));
                }
            }
        }


        /// <summary>
        /// Starts listening.
        /// </summary>
        public void Start()
        {
            this.slimLock.ExecuteInWriterLock(
                () =>
                {
                    if(this.httpListener.IsListening)
                    {
                        return;
                    }

                    this.httpListener.Start();

                    for(int i = 0; i < this.numberOfThreads; i++)
                    {
                        Task.Run(listen);
                    }
                });
        }


        /// <summary>
        /// Stop listening.
        /// </summary>
        public void Stop()
        {
            this.slimLock.ExecuteInWriterLock(
                () =>
                {
                    if(this.httpListener.IsListening)
                    {
                        this.httpListener.Stop();
                    }
                });
        }




        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using (this.slimLock)
                    using (this.notificationManager)
                    using (this.httpListener)
                    {
                        // Disposed.
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~WebhookListener() {
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