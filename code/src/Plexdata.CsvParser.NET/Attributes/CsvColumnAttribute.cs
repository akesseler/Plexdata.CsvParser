/*
 * MIT License
 * 
 * Copyright (c) 2018 plexdata.de
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

namespace Plexdata.CsvParser.Attributes
{
    /// <summary>
    /// This attribute can be used on properties to tell the CSV processors 
    /// that this property want be should in the processing procedure. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute should be used only on public instance properties because 
    /// of only these types of properties are processed.
    /// </para>
    /// <para>
    /// The column order is determined by the order of the properties in case of 
    /// the column number is not used.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CsvColumnAttribute : Attribute
    {
        #region Construction

        /// <summary>
        /// Default attribute constructor.
        /// </summary>
        /// <remarks>
        /// This default constructor creates an instance of this class and 
        /// initialize all properties with its default values.
        /// </remarks>
        public CsvColumnAttribute()
            : base()
        {
            this.Header = String.Empty;
            this.Offset = -1;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the header name of the corresponding column. Default 
        /// value is empty. 
        /// </summary>
        /// <remarks>
        /// The name of the property is used for processing in case of the header 
        /// name is not set.
        /// </remarks>
        /// <value>
        /// The Header property gets and/or sets the column header name to be used.
        /// </value>
        public String Header { get; set; }

        /// <summary>
        /// Gets or sets the zero-based offset of a column within a CSV dataset. 
        /// Default value is -1.
        /// </summary>
        /// <remarks>
        /// The offset value is ignored if it is less than zero. On the other 
        /// hand, the column ordering might be unexpected in case of two or more 
        /// column share the same offset value. In any other case, the offset 
        /// value defines the expected position of a particular column inside 
        /// a CSV dataset.
        /// </remarks>
        /// <value>
        /// The Offset property gets and/or sets the zero-based column index to 
        /// be used.
        /// </value>
        public Int32 Offset { get; set; }

        /// <summary>
        /// Indicates if the header is empty or not (convenient property).
        /// </summary>
        /// <remarks>
        /// The return value is determined by checking the header string. This 
        /// means if current header is neither &lt;null&gt; nor empty then this 
        /// property returns 'true'.
        /// </remarks>
        /// <value>
        /// True is returned if the header is valid and false otherwise.
        /// </value>
        public Boolean IsHeader
        {
            get
            {
                return !String.IsNullOrWhiteSpace(this.Header);
            }
        }

        /// <summary>
        /// Indicates if the offset is used or not (convenient property).
        /// </summary>
        /// <remarks>
        /// The return value is determined by checking the offset value. This 
        /// means if current offset is greater than or equal to zero then this 
        /// property returns 'true'.
        /// </remarks>
        /// <value>
        /// True is returned if the offset is not less than zero and false 
        /// otherwise.
        /// </value>
        public Boolean IsOffset
        {
            get
            {
                return this.Offset >= 0;
            }
        }

        #endregion
    }
}
