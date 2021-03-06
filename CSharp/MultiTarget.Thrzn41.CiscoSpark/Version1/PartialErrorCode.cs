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
using System.Text;

namespace Thrzn41.CiscoSpark.Version1
{

    /// <summary>
    /// Error code for Partial Error.
    /// </summary>
    public class PartialErrorCode
    {

        /// <summary>
        /// Failure about Key Management Server.
        /// </summary>
        public static readonly PartialErrorCode KMSFailure = new PartialErrorCode("kms_failure");




        /// <summary>
        /// Dictionary for Partial Error Code.
        /// </summary>
        private static readonly Dictionary<string, PartialErrorCode> PARTIAL_ERROR_CODES;

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static PartialErrorCode()
        {
            PARTIAL_ERROR_CODES = new Dictionary<string, PartialErrorCode>();

            PARTIAL_ERROR_CODES.Add(KMSFailure.Name, KMSFailure);
        }


        /// <summary>
        /// Name of the Partial Error Code.
        /// </summary>
        public string Name { get; private set; }




        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the Partial Error Code.</param>
        private PartialErrorCode(string name)
        {
            this.Name = name;
        }


        /// <summary>
        /// Parse Partial Error Code.
        /// </summary>
        /// <param name="name">Name of the Partial Error Code.</param>
        /// <returns><see cref="PartialErrorCode"/> for the name.</returns>
        public static PartialErrorCode Parse(string name)
        {
            PartialErrorCode partialErrorCode = null;

            if ( name == null || !PARTIAL_ERROR_CODES.TryGetValue(name, out partialErrorCode) )
            {
                partialErrorCode = new PartialErrorCode(name);
            }

            return partialErrorCode;
        }


        /// <summary>
        /// Determines whether this instance and another specified <see cref="PartialErrorCode"/> object have the same value.
        /// </summary>
        /// <param name="value">The Partial Error Code to compare to this instance.</param>
        /// <returns>true if the value of the parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(PartialErrorCode value)
        {
            if ( Object.ReferenceEquals(value, null) )
            {
                return false;
            }

            if ( Object.ReferenceEquals(this, value) )
            {
                return true;
            }

            return (this.Name == value.Name);
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must also be a <see cref="PartialErrorCode"/> object, have the same value.
        /// </summary>
        /// <param name="obj">The Partial Error Code to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="PartialErrorCode"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as PartialErrorCode);
        }

        /// <summary>
        /// Returns the hash code for this Partial Error Code.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        /// <summary>
        /// Determines whether two specified Partial Error Code have the same value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the same value.</returns>
        public static bool operator ==(PartialErrorCode lhs, PartialErrorCode rhs)
        {
            if ( Object.ReferenceEquals(lhs, null) )
            {
                if ( Object.ReferenceEquals(rhs, null) )
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether two specified Partial Error Code have the different value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the different value.</returns>
        public static bool operator !=(PartialErrorCode lhs, PartialErrorCode rhs)
        {
            return !(lhs == rhs);
        }

    }

}
