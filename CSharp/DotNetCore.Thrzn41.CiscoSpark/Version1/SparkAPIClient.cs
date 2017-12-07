using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Thrzn41.Util;

namespace Thrzn41.CiscoSpark.Version1
{
    public class SparkAPIClient : IDisposable
    {

        /// <summary>
        /// Media type value for application/json.
        /// </summary>
        protected const string MEDIA_TYPE_APPLICATION_JSON = "application/json";


        /// <summary>
        /// Spark API Path.
        /// </summary>
        protected const string SPARK_API_PATH = "https://api.ciscospark.com/v1/";




        /// <summary>
        /// Default encoding in this class.
        /// </summary>
        protected static readonly Encoding ENCODING = UTF8Utils.UTF8_WITHOUT_BOM;


        /// <summary>
        /// Spark messages API Path.
        /// </summary>
        protected static readonly string SPARK_MESSAGES_API_PATH = getAPIPath("messages");

        /// <summary>
        /// Spark messages API Uri.
        /// </summary>
        protected static readonly Uri SPARK_MESSAGES_API_URI = new Uri(SPARK_MESSAGES_API_PATH);

        
        /// <summary>
        /// Uri pattern of Spark API.
        /// </summary>
        private readonly static Regex SPARK_API_URI_PATTERN = new Regex(String.Format("^{0}", SPARK_API_PATH), RegexOptions.Compiled, TimeSpan.FromSeconds(60.0f));


        /// <summary>
        /// HttpClient for Spark API.
        /// </summary>
        private readonly SparkHttpClient sparkHttpClient;




        /// <summary>
        /// Constructor of SparkAPIClient.
        /// </summary>
        /// <param name="token">token of Spark API.</param>
        internal SparkAPIClient(string token)
        {
            this.sparkHttpClient = new SparkHttpClient(token, SPARK_API_URI_PATTERN);
        }


        /// <summary>
        /// Creates <see cref="HttpRequestMessage"/> to use for Json request.
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="objectToBePosted">Object inherited from <see cref="SparkObject"/> to be sent to Spark API.</param>
        /// <returns><see cref="HttpRequestMessage"/> that is created.</returns>
        private HttpRequestMessage createJsonRequest(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, SparkObject objectToBePosted = null)
        {
            var request = new HttpRequestMessage(method, HttpUtils.BuildUri(uri, queryParameters));

            var headers = request.Headers;

            headers.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(ENCODING.WebName));
            headers.Accept.Add(       new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MEDIA_TYPE_APPLICATION_JSON));

            if(objectToBePosted != null)
            {
                request.Content = new StringContent(
                    objectToBePosted.ToJsonString(),
                    ENCODING,
                    MEDIA_TYPE_APPLICATION_JSON);
            }

            return request;
        }


        /// <summary>
        /// Gets API path for each api.
        /// </summary>
        /// <param name="apiPath">Each api path.</param>
        /// <returns>Full path for the api.</returns>
        protected static string getAPIPath(string apiPath)
        {
            return String.Format("{0}{1}", SPARK_API_PATH, apiPath);
        }


        #region Messages APIs

        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="targetId">Id that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="files">File uris to be attached with the message.</param>
        /// <param name="targetType"><see cref="MessageTargetType"/> that the targetId parameter represents.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > CreateMessageAsync(string targetId, string markdownOrText, Uri[] files = null, MessageTargetType targetType = MessageTargetType.SpaceId, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            var message = new Message();

            switch (targetType)
            {
                case MessageTargetType.PersonId:
                    message.ToPersonId = targetId;
                    break;
                case MessageTargetType.PersonEmail:
                    message.ToPersonEmail = targetId;
                    break;
                default:
                    message.SpaceId = targetId;
                    break;
            }

            switch (textType)
            {
                case MessageTextType.Text:
                    message.Text = markdownOrText;
                    break;
                default:
                    message.Markdown = markdownOrText;
                    break;
            }

            if(files != null)
            {
                List<string> fileList = new List<string>();

                foreach (var item in files)
                {
                    fileList.Add(item.AbsoluteUri);
                }

                message.Files = fileList.ToArray();
            }

            var result = await this.sparkHttpClient.RequestAsync<Message>(
                createJsonRequest(HttpMethod.Post, SPARK_MESSAGES_API_URI, null, message),
                cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        #endregion




        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using (this.sparkHttpClient)
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
        // ~SparkAPIClient() {
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
