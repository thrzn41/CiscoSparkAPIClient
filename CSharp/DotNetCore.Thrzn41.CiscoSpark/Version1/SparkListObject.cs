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
    /// Spark v1 list object.
    /// </summary>
    /// <typeparam name="TSparkObject">Spark Object that is contained in the list.</typeparam>
    [JsonObject(MemberSerialization.OptIn)]
    public class SparkListObject<TSparkObject> : SparkObject
        where TSparkObject : SparkObject, new()
    {

        /// <summary>
        /// Item list.
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public TSparkObject[] Items { get; set; }

        /// <summary>
        /// Indicates the object contains items or not.
        /// </summary>
        public bool HasItems
        {
            get
            {
                return (this.Items != null && this.Items.Length > 0);
            }
        }

        /// <summary>
        /// Item count.
        /// </summary>
        public int ItemCount
        {
            get
            {
                return ((this.Items != null) ? this.Items.Length : 0);
            }
        }


    }

}
