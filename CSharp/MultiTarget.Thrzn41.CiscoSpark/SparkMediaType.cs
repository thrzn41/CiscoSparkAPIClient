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

namespace Thrzn41.CiscoSpark
{

    /// <summary>
    /// Media type of Cisco Spark.
    /// </summary>
    public class SparkMediaType
    {

        /// <summary>
        /// PNG.
        /// </summary>
        public static readonly SparkMediaType ImagePNG = new SparkMediaType("image/png");

        /// <summary>
        /// JPEG.
        /// </summary>
        public static readonly SparkMediaType ImageJPEG = new SparkMediaType("image/jpeg");

        /// <summary>
        /// BMP.
        /// </summary>
        public static readonly SparkMediaType ImageBMP = new SparkMediaType("image/bmp");

        /// <summary>
        /// GIF.
        /// </summary>
        public static readonly SparkMediaType ImageGIF = new SparkMediaType("image/gif");

        /// <summary>
        /// PDF.
        /// </summary>
        public static readonly SparkMediaType PDF = new SparkMediaType("application/pdf");

        /// <summary>
        /// PowerPoint.
        /// </summary>
        public static readonly SparkMediaType PowerPoint = new SparkMediaType("application/vnd.ms-powerpoint");

        /// <summary>
        /// PowerPoint Open Xml.
        /// </summary>
        public static readonly SparkMediaType PowerPointOpenXml = new SparkMediaType("application/vnd.openxmlformats-officedocument.presentationml.presentation");

        /// <summary>
        /// Word.
        /// </summary>
        public static readonly SparkMediaType Word = new SparkMediaType("application/msword");

        /// <summary>
        /// Word Open Xml.
        /// </summary>
        public static readonly SparkMediaType WordOpenXml = new SparkMediaType("application/vnd.openxmlformats-officedocument.wordprocessingml.document");


        /// <summary>
        /// Dictionary for space type.
        /// </summary>
        private static readonly Dictionary<string, SparkMediaType> MEDIA_TYPES;

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static SparkMediaType()
        {
            MEDIA_TYPES = new Dictionary<string, SparkMediaType>();

            MEDIA_TYPES.Add(ImagePNG.Name,  ImagePNG);
            MEDIA_TYPES.Add(ImageJPEG.Name, ImageJPEG);
            MEDIA_TYPES.Add(ImageBMP.Name,  ImageBMP);
            MEDIA_TYPES.Add(ImageGIF.Name,  ImageGIF);

            MEDIA_TYPES.Add(PDF.Name, PDF);

            MEDIA_TYPES.Add(PowerPoint.Name,        PowerPoint);
            MEDIA_TYPES.Add(PowerPointOpenXml.Name, PowerPointOpenXml);

            MEDIA_TYPES.Add(Word.Name,        Word);
            MEDIA_TYPES.Add(WordOpenXml.Name, WordOpenXml);
        }


        /// <summary>
        /// Name of the media type.
        /// </summary>
        public string Name { get; private set; }




        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the media type.</param>
        private SparkMediaType(string name)
        {
            this.Name = name;
        }


        /// <summary>
        /// Parse space type.
        /// </summary>
        /// <param name="name">Name of the media type.</param>
        /// <returns><see cref="SparkMediaType"/> for the name.</returns>
        public static SparkMediaType Parse(string name)
        {
            SparkMediaType mediaType = null;

            if ( !MEDIA_TYPES.TryGetValue(name, out mediaType) )
            {
                mediaType = new SparkMediaType(name);
            }

            return mediaType;
        }


        /// <summary>
        /// Determines whether this instance and another specified <see cref="SparkMediaType"/> object have the same value.
        /// </summary>
        /// <param name="value">The media type to compare to this instance.</param>
        /// <returns>true if the value of the parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(SparkMediaType value)
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
        /// Determines whether this instance and a specified object, which must also be a <see cref="SparkMediaType"/> object, have the same value.
        /// </summary>
        /// <param name="obj">The media type to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="SparkMediaType"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SparkMediaType);
        }

        /// <summary>
        /// Returns the hash code for this space type.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        /// <summary>
        /// Determines whether two specified media types have the same value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the same value.</returns>
        public static bool operator ==(SparkMediaType lhs, SparkMediaType rhs)
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
        /// Determines whether two specified media types have the different value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the different value.</returns>
        public static bool operator !=(SparkMediaType lhs, SparkMediaType rhs)
        {
            return !(lhs == rhs);
        }

    }

}
