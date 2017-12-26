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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.CiscoSpark
{

    /// <summary>
    /// Base Object for all Cisco Spark objects.
    /// This class provides feature to convert object to/from Json.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SparkObject
    {

        /// <summary>
        /// Settings for Json serializer.
        /// </summary>
        [JsonIgnore]
        private static readonly JsonSerializerSettings SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting        = Formatting.None,
        };

        /// <summary>
        /// Settings for Json deserializer.
        /// </summary>
        [JsonIgnore]
        private static readonly JsonSerializerSettings DESERIALIZER_SETTINGS = new JsonSerializerSettings
        {
        };


        /// <summary>
        /// Indicates the Spark Object has values or not.
        /// </summary>
        public bool HasValues { get; internal set; } = true;


        /// <summary>
        /// Extension data which contains non-deserialized json.
        /// </summary>
        [JsonExtensionData]
        internal protected IDictionary<string, JToken> JsonExtensionData { get; private set; }

        /// <summary>
        /// Indicates this Spark Object has extension data.
        /// </summary>
        public bool HasExtensionData
        {
            get
            {
                return (this.JsonExtensionData != null && this.JsonExtensionData.Count > 0);
            }
        }

        /// <summary>
        /// Converts object to Json style string.
        /// </summary>
        /// <returns>Json style string that represents this object.</returns>
        public virtual string ToJsonString()
        {
            return JsonConvert.SerializeObject(this, SERIALIZER_SETTINGS);
        }


        /// <summary>
        /// Gets extension data key list.
        /// </summary>
        /// <returns>Extension data key list or null.</returns>
        public string[] GetExtensionDataKeys()
        {
            string[] result = null;

            if(this.HasExtensionData)
            {
                result = new string[this.JsonExtensionData.Keys.Count];

                this.JsonExtensionData.Keys.CopyTo(result, 0);
            }

            return result;
        }

        /// <summary>
        /// Gets Json extension data that are not deserialized explicitly.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="key">The key of Json object.</param>
        /// <returns>Json extension data.</returns>
        public T GetExtensionData<T>(string key)
        {
            T result = default(T);

            if(this.HasExtensionData && this.JsonExtensionData.ContainsKey(key))
            {
                result = this.JsonExtensionData[key].ToObject<T>();
            }

            return result;
        }

        /// <summary>
        /// Gets Json extension data that are not deserialized explicitly.
        /// </summary>
        /// <param name="key">The key of Json object.</param>
        /// <returns>Json extension data.</returns>
        public string GetExtensionJsonString(string key)
        {
            string result = null;

            if (this.HasExtensionData && this.JsonExtensionData.ContainsKey(key))
            {
                result = this.JsonExtensionData[key].ToString(Formatting.None);
            }

            return result;
        }

        /// <summary>
        /// Gets Json extension data dictionary that are not deserialized explicitly.
        /// </summary>
        /// <returns>Json extension data dictionary.</returns>
        public Dictionary<string, string> GetExtensionJsonStrings()
        {
            var result = new Dictionary<string, string>();

            if (this.HasExtensionData)
            {
                foreach (var item in this.JsonExtensionData.Keys)
                {
                    result.Add(item, this.JsonExtensionData[item].ToString(Formatting.None));
                }
            }

            return result;
        }


        /// <summary>
        /// Converts Json string to a Cisco Spark object.
        /// </summary>
        /// <typeparam name="TSparkObject">A subclass type of SparkObject.</typeparam>
        /// <param name="jsonString">Json style string to be converted.</param>
        /// <returns>Cisco Spark object converted from Json string.</returns>
        public static TSparkObject FromJsonString<TSparkObject>(string jsonString)
            where TSparkObject : SparkObject, new()
        {
            return JsonConvert.DeserializeObject<TSparkObject>(jsonString, DESERIALIZER_SETTINGS);
        }

    }

}
