/*
 * MIT License
 * 
 * Copyright (c) 2024 plexdata.de
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

using Plexdata.CsvParser.Attributes;
using System;
using System.Collections.Generic;

namespace Plexdata.CsvParser.Internals
{
    /// <summary>
    /// This internal class describes the characteristics of a class representing 
    /// a CSV content.
    /// </summary>
    /// <remarks>
    /// The type descriptor class just serves as a summary of the characteristics 
    /// of a CSV definition class.
    /// </remarks>
    internal class TypeDescriptor
    {
        #region Construction

        /// <summary>
        /// The parameterized class constructor.
        /// </summary>
        /// <remarks>
        /// This constructor initializes an instance of this class with the 
        /// characteristics of a CSV definition class.
        /// </remarks>
        /// <param name="document">
        /// An instance of class <see cref="CsvDocumentAttribute"/> that describes 
        /// the characteristics of a CSV document.
        /// </param>
        /// <param name="settings">
        /// A list of instances of class <see cref="ItemDescriptor"/> that contains 
        /// the characteristics of all affected CSV columns. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown if one of the parameters is &lt;null&gt;.
        /// </exception>
        public TypeDescriptor(CsvDocumentAttribute document, IEnumerable<ItemDescriptor> settings)
            : base()
        {
            this.Document = document ?? throw new ArgumentNullException(nameof(document));
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the assigned instance of class <see cref="CsvDocumentAttribute"/> .
        /// </summary>
        /// <remarks>
        /// The Document property describes details of the corresponding CSV file.
        /// </remarks>
        /// <value>
        /// The property returns the assigned document descriptor.
        /// </value>
        public CsvDocumentAttribute Document { get; private set; }

        /// <summary>
        /// Gets the assigned list of instances of class <see cref="ItemDescriptor"/> .
        /// </summary>
        /// <remarks>
        /// The Settings property describes details of the columns of the 
        /// corresponding CSV file.
        /// </remarks>
        /// <value>
        /// The property returns the assigned settings descriptors.
        /// </value>
        public IEnumerable<ItemDescriptor> Settings { get; private set; }

        #endregion
    }
}
