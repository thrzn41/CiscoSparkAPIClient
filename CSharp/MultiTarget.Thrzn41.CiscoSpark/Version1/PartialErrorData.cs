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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.CiscoSpark.Version1
{

    /// <summary>
    /// Cisco Spark Partial Error data.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PartialErrorData : SparkData
    {

        /// <summary>
        /// Code of the error.
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string CodeName{ get; internal set; }

        /// <summary>
        /// Code of the error.
        /// </summary>
        [JsonIgnore]
        public PartialErrorCode Code {
            get
            {
                return PartialErrorCode.Parse(this.CodeName);
            }
        }

        /// <summary>
        /// Reason of the error.
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; internal set; }

    }

}
