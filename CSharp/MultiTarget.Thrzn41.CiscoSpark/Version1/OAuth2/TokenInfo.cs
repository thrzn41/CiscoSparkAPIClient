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

namespace Thrzn41.CiscoSpark.Version1.OAuth2
{

    /// <summary>
    /// Cisco Spark token info object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TokenInfo : SparkData
    {

        /// <summary>
        /// Access token.
        /// </summary>
        [JsonProperty(PropertyName = "accessToken")]
        public string AccessToken { get; internal set; }

        /// <summary>
        /// Access token will expire in this time from refreshed date time.
        /// </summary>
        [JsonProperty(PropertyName = "accessTokenExpiresIn")]
        public TimeSpan? AccessTokenExpiresIn { get; internal set; }

        /// <summary>
        /// <see cref="DateTime"/> when the access token will expire.
        /// </summary>
        [JsonProperty(PropertyName = "accessTokenExpiresAt")]
        public DateTime? AccessTokenExpiresAt { get; internal set; }


        /// <summary>
        /// Refresh token.
        /// </summary>
        [JsonProperty(PropertyName = "refreshToken")]
        public string RefreshToken { get; internal set; }

        /// <summary>
        /// Refresh token will expire in this time from refreshed date time.
        /// </summary>
        [JsonProperty(PropertyName = "refreshTokenExpiresIn")]
        public TimeSpan? RefreshTokenExpiresIn { get; internal set; }

        /// <summary>
        /// <see cref="DateTime"/> when the refresh token will expire.
        /// </summary>
        [JsonProperty(PropertyName = "refreshTokenExpiresAt")]
        public DateTime? RefreshTokenExpiresAt { get; internal set; }


        /// <summary>
        /// <see cref="DateTime"/> when the token info is refreshed.
        /// </summary>
        [JsonProperty(PropertyName = "refreshedAt")]
        public DateTime? RefreshedAt { get; internal set; }

    }

}
