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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.CiscoSpark.Version1
{

    /// <summary>
    /// Cisco Spark Message object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Message : SparkData
    {

        /// <summary>
        /// Id of the message.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        /// <summary>
        /// Id of Spark space that this message exists in.
        /// </summary>
        [JsonProperty(PropertyName = "roomId")]
        public string SpaceId { get; internal set; }

        /// <summary>
        /// Type name of Spark space that this message exists in.
        /// </summary>
        [JsonProperty(PropertyName = "roomType")]
        public string SpaceTypeName { get; internal set; }

        /// <summary>
        /// Type of Spark space that this message exists in.
        /// </summary>
        [JsonIgnore]
        public SpaceType SpaceType
        {
            get
            {
                return SpaceType.Parse(this.SpaceTypeName);
            }
        }

        /// <summary>
        /// Id of person that this message to be posted.
        /// </summary>
        [JsonProperty(PropertyName = "toPersonId")]
        internal string ToPersonId { get; set; }

        /// <summary>
        /// Email of person that this message to be posted.
        /// </summary>
        [JsonProperty(PropertyName = "toPersonEmail")]
        internal string ToPersonEmail { get; set; }

        /// <summary>
        /// Text of the message.
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; internal set; }

        /// <summary>
        /// Markdonw text to be posted.
        /// </summary>
        [JsonProperty(PropertyName = "markdown")]
        internal string Markdown { get; set; }

        /// <summary>
        /// File list that are attached to the message.
        /// </summary>
        [JsonProperty(PropertyName = "files")]
        public string[] Files { get; internal set; }

        /// <summary>
        /// Indicates the message has files attached.
        /// </summary>
        [JsonIgnore]
        public bool HasFiles
        {
            get
            {
                return (this.Files != null && this.Files.Length > 0);
            }
        }

        /// <summary>
        /// Attached file count of the message.
        /// </summary>
        [JsonIgnore]
        public int FileCount
        {
            get
            {
                return ((this.Files != null) ? this.Files.Length : 0);
            }
        }

        /// <summary>
        /// File Uri list that are attached to the message.
        /// </summary>
        public Uri[] FileUris
        {
            get
            {
                Uri[] result = null;

                if(this.HasFiles)
                {
                    string[] files = this.Files;

                    result = new Uri[files.Length];

                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = new Uri(files[i]);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Id of person who owns the message.
        /// </summary>
        [JsonProperty(PropertyName = "personId")]
        public string PersonId { get; internal set; }

        /// <summary>
        /// Email of person who owns the message.
        /// </summary>
        [JsonProperty(PropertyName = "personEmail")]
        public string PersonEmail { get; internal set; }

        /// <summary>
        /// <see cref="DateTime"/> when the message was created.
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; internal set; }

        /// <summary>
        /// Id list of person who are mentioned in the message.
        /// </summary>
        [JsonProperty(PropertyName = "mentionedPeople")]
        public string[] MentionedPersons { get; internal set; }

        /// <summary>
        /// Id list of person who are mentioned in the message.
        /// </summary>
        [JsonIgnore]
        public string[] MentionedPeople {
            get
            {
                return this.MentionedPersons;
            }
        }

        /// <summary>
        /// Indicates the message has mentioned people or not.
        /// </summary>
        [JsonIgnore]
        public bool HasMentionedPersons
        {
            get
            {
                return (this.MentionedPersons != null && this.MentionedPersons.Length > 0);
            }
        }

        /// <summary>
        /// Indicates the message has mentioned people or not.
        /// </summary>
        [JsonIgnore]
        public bool HasMentionedPeople
        {
            get
            {
                return this.HasMentionedPersons;
            }
        }

        /// <summary>
        /// Mentioned person count.
        /// </summary>
        [JsonIgnore]
        public int MentionedPersonCount
        {
            get
            {
                return ((this.MentionedPersons != null) ? this.MentionedPersons.Length : 0);
            }
        }

        /// <summary>
        /// Html text that is posted.
        /// </summary>
        [JsonProperty(PropertyName = "html")]
        public string Html { get; internal set; }

        /// <summary>
        /// Indicates this message has html or not.
        /// </summary>
        [JsonIgnore]
        public bool HasHtml
        {
            get
            {
                return !String.IsNullOrEmpty(this.Html);
            }
        }

    }

}
