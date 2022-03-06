/*
 * MIT License
 * 
 * Copyright (c) 2022 plexdata.de
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
using System.Reflection;
using System.Text;

namespace Plexdata.CsvParser.Internals
{
    /// <summary>
    /// This internal class describes the characteristics of a single property 
    /// that represents a CSV column.
    /// </summary>
    /// <remarks>
    /// The item descriptor class just serves as a summary of the characteristics 
    /// of one column in a CSV file.
    /// </remarks>
    internal class ItemDescriptor
    {
        #region Construction

        /// <summary>
        /// The parameterized class constructor.
        /// </summary>
        /// <remarks>
        /// This constructor initializes an instance of this class with the 
        /// characteristics of a CSV file column.
        /// </remarks>
        /// <param name="column">
        /// An instance of class <see cref="CsvColumnAttribute"/> that describes 
        /// the characteristics of a CSV column.
        /// </param>
        /// <param name="origin">
        /// An instance of class <see cref="PropertyInfo"/> that describes 
        /// the characteristics of the corresponding property.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown if one of the parameters is &lt;null&gt;.
        /// </exception>
        public ItemDescriptor(CsvColumnAttribute column, PropertyInfo origin)
            : base()
        {
            this.Column = column ?? throw new ArgumentNullException(nameof(column));
            this.Origin = origin ?? throw new ArgumentNullException(nameof(origin));
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the assigned instance of class <see cref="CsvColumnAttribute"/> .
        /// </summary>
        /// <remarks>
        /// The Column property describes details of the corresponding CSV column.
        /// </remarks>
        /// <value>
        /// The property returns the assigned column descriptor.
        /// </value>
        public CsvColumnAttribute Column { get; private set; }

        /// <summary>
        /// Gets the assigned instance of class <see cref="PropertyInfo"/> .
        /// </summary>
        /// <remarks>
        /// The Origin property describes internal and type dependent property 
        /// details.
        /// </remarks>
        /// <value>
        /// The property returns the assigned property descriptor.
        /// </value>
        public PropertyInfo Origin { get; private set; }

        #endregion

        #region Public methods

        /// <summary>
        /// This method returns a string containing current instance information.
        /// </summary>
        /// <remarks>
        /// This method has been overwritten and returns a string showing current 
        /// instance values.
        /// </remarks>
        /// <returns>
        /// A string consisting of details of current instance information.
        /// </returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder(256);
            result.Append($"Offset: \"{this.Column.Offset}\", ");
            result.Append($"Header: \"{(this.Column.IsHeader ? this.Column.Header : "<null>")}\", ");
            result.Append($"Origin: \"{this.Origin.Name}\"");
            return result.ToString();
        }

        #endregion
    }
}
