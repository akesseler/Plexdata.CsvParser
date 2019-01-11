/*
 * MIT License
 * 
 * Copyright (c) 2019 plexdata.de
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

using Plexdata.CsvParser.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Plexdata.CsvParser.Processors
{
    /// <summary>
    /// This class serves as container for the content of CSV data.
    /// </summary>
    /// <remarks>
    /// Additionally, this class provides methods to access their items as well as 
    /// converts values into their expected data type.
    /// </remarks>
    public class CsvContainer
    {
        #region Construction

        /// <summary>
        /// The default class constructor.
        /// </summary>
        /// <remarks>
        /// The constructor initializes all properties with their default values.
        /// </remarks>
        internal CsvContainer()
            : this(null)
        {
        }

        /// <summary>
        /// The constructor with content initialization.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The constructor takes provided <paramref name="content"/> and initializes 
        /// all remaining properties with their default values.
        /// </para>
        /// <para>
        /// Attention: The value of parameter <paramref name="content"/> represents 
        /// line-oriented CSV data!
        /// </para>
        /// </remarks>
        /// <param name="content">
        /// The content to be used. An empty content is automatically used if this 
        /// parameter is <c>null</c>.
        /// </param>
        internal CsvContainer(List<List<String>> content)
            : base()
        {
            this.Content = this.Transform(content);
            this.Compare = StringComparison.OrdinalIgnoreCase;
            this.Culture = CultureInfo.CurrentUICulture;
            this.Mappings = new CsvMappings();
            this.Heading = false;
            this.Exactly = false;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the list of column items at specified position.
        /// </summary>
        /// <remarks>
        /// This getter validates provided column number and if valid it returns the 
        /// column as enumerable list. But <c>null</c> is returned if provided column 
        /// index is either less than zero or greater than total column count.
        /// </remarks>
        /// <value>
        /// An enumerable list of column items.
        /// </value>
        /// <param name="column">
        /// The zero-based index of the column to get.
        /// </param>
        /// <returns>
        /// The whole list of column items or <c>null</c> if not found.
        /// </returns>
        public IEnumerable<String> this[Int32 column]
        {
            get
            {
                if (0 <= column && column < this.Content.Count)
                {
                    return this.Content[column];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets an element for provided column and at specified index.
        /// </summary>
        /// <remarks>
        /// This getter validates provided column as well as specified index and if 
        /// both are valid it returns the element at column and index. But <c>null</c> 
        /// is returned if either provided column number or specified index is less 
        /// than zero or greater than total column count.
        /// </remarks>
        /// <value>
        /// The string representation of the CSV content at column and index.
        /// </value>
        /// <param name="column">
        /// The zero-based index of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <returns>
        /// The string of affected column at specified index or <c>null</c> if not 
        /// found.
        /// </returns>
        public String this[Int32 column, Int32 index]
        {
            get
            {
                if (0 <= column && column < this.Content.Count)
                {
                    if (0 <= index && index < this.Content[column].Count)
                    {
                        return this.Content[column][index];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the list of column items for provided header.
        /// </summary>
        /// <remarks>
        /// This getter tries to find the column where its top level item fits 
        /// provided header name and returns the whole list of column items. Very 
        /// important to note, calling this getter will only have any effect if 
        /// Heading is enabled.
        /// </remarks>
        /// <value>
        /// An enumerable list of column items.
        /// </value>
        /// <param name="header">
        /// The header name to get a column for.
        /// </param>
        /// <returns>
        /// The whole list of column items or <c>null</c> if not found or in case 
        /// of heading is disabled.
        /// </returns>
        /// <seealso cref="CsvContainer.Heading"/>
        /// <seealso cref="CsvContainer.HeaderToColumn(String)"/>
        public IEnumerable<String> this[String header]
        {
            get
            {
                Int32 column = this.HeaderToColumn(header);

                if (column >= 0)
                {
                    return this[column];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets an element for provided header and at specified index.
        /// </summary>
        /// <remarks>
        /// This getter tries to find the column where its top level item fits 
        /// provided header name. Thereafter, the specified index is validated.
        /// The content is returned if both conditions could be confirmed. Very 
        /// important to note, calling this getter will only have any effect if 
        /// Heading is enabled.
        /// </remarks>
        /// <value>
        /// The string representation of the CSV content at column and index.
        /// </value>
        /// <param name="header">
        /// The header name of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <returns>
        /// The string of affected column at specified index or <c>null</c> if 
        /// not found.
        /// </returns>
        /// <seealso cref="CsvContainer.Heading"/>
        /// <seealso cref="CsvContainer.HeaderToColumn(String)"/>
        public String this[String header, Int32 index]
        {
            get
            {
                Int32 column = this.HeaderToColumn(header);

                if (column >= 0 && index >= 0)
                {
                    return this[column, index + 1];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the currently used CSV content.
        /// </summary>
        /// <remarks>
        /// This content might be empty but is never <c>null</c>.
        /// </remarks>
        /// <value>
        /// The property returns the associated CSV content.
        /// </value>
        public List<List<String>> Content { get; private set; }

        /// <summary>
        /// Gets and sets the algorithm how any string comparison has to be done.
        /// Default value is <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </summary>
        /// <remarks>
        /// A string comparison is only relevant to find a particular header.
        /// </remarks>
        /// <value>
        /// The algorithm to be used for string compare operations.
        /// </value>
        public StringComparison Compare { get; set; }

        /// <summary>
        /// Gets currently applied culture. Default value is current UI culture.
        /// </summary>
        /// <remarks>
        /// The culture value is taken over from the settings that have been provided 
        /// for reading a CSV content.
        /// </remarks>
        /// <value>
        /// The culture to be used for example for converting numbers.
        /// </value>
        /// <seealso cref="CsvReader"/>
        /// <seealso cref="CsvSettings.Culture"/>
        public CultureInfo Culture { get; internal set; }

        /// <summary>
        /// Gets currently applied value mappings. Default value is standard mapping.
        /// </summary>
        /// <remarks>
        /// The mappings value is taken over from the settings that have been provided 
        /// for reading a CSV content.
        /// </remarks>
        /// <value>
        /// The mapping to be used for value transformation. 
        /// </value>
        /// <seealso cref="CsvReader"/>
        /// <seealso cref="CsvSettings.Mappings"/>
        public CsvMappings Mappings { get; internal set; }

        /// <summary>
        /// Determines if the header usage is enabled or disabled. Default value is 
        /// false.
        /// </summary>
        /// <remarks>
        /// The header is taken over from the settings that have been provided for 
        /// reading a CSV content.
        /// </remarks>
        /// <value>
        /// This value influences the header processing behavior.
        /// </value>
        /// <seealso cref="CsvReader"/>
        /// <seealso cref="CsvSettings.Heading"/>
        public Boolean Heading { get; internal set; }

        /// <summary>
        /// Determines if the exactly mode is enabled or disabled. Default value is 
        /// false.
        /// </summary>
        /// <remarks>
        /// The exactly mode is taken over from the settings that have been provided 
        /// for reading a CSV content.
        /// </remarks>
        /// <value>
        /// This value influences the data conversion behavior.
        /// </value>
        /// <seealso cref="CsvReader"/>
        /// <seealso cref="CsvSettings.Exactly"/>
        public Boolean Exactly { get; internal set; }

        #endregion

        #region Public methods

        /// <summary>
        /// This method gets the type-save value for provided column and at specified 
        /// index.
        /// </summary>
        /// <remarks>
        /// First of all it is tried to get the string for provided column and at 
        /// specified index. Thereafter, the type converter is called by providing 
        /// additional information.
        /// </remarks>
        /// <typeparam name="TType">
        /// The expected type to convert the string value into.
        /// </typeparam>
        /// <param name="column">
        /// The zero-based index of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <returns>
        /// An object representing the value for provided column and at specified index.
        /// </returns>
        /// <exception cref="Exception">
        /// This method might throw any of the exceptions of the <see cref="TypeConverter"/>.
        /// </exception>
        /// <seealso cref="CsvContainer.Exactly"/>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.TryGetValue(Int32, Int32)"/>
        /// <seealso cref="CsvContainer.GetObject(String, Type)"/>
        /// <seealso cref="TypeConverter.IntoObject(String, Type, Boolean, CultureInfo, CsvMappings)"/>
        public Object GetValue<TType>(Int32 column, Int32 index)
        {
            return this.GetValue(column, index, typeof(TType));
        }

        /// <summary>
        /// This method gets the type-save value for provided column and at specified 
        /// index taking provide type into account.
        /// </summary>
        /// <remarks>
        /// First of all it is tried to get the string for provided column and at 
        /// specified index. Thereafter, the type converter is called by providing 
        /// additional information.
        /// </remarks>
        /// <param name="column">
        /// The zero-based index of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <param name="type">
        /// The expected type to convert the string value into.
        /// </param>
        /// <returns>
        /// An object representing the value for provided column and at specified index.
        /// </returns>
        /// <exception cref="Exception">
        /// This method might throw any of the exceptions of the <see cref="TypeConverter"/>.
        /// </exception>
        /// <seealso cref="CsvContainer.Exactly"/>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.TryGetValue(Int32, Int32, Type)"/>
        /// <seealso cref="CsvContainer.GetObject(String, Type)"/>
        /// <seealso cref="TypeConverter.IntoObject(String, Type, Boolean, CultureInfo, CsvMappings)"/>
        public Object GetValue(Int32 column, Int32 index, Type type)
        {
            return this.GetObject(this[column, index], type);
        }

        /// <summary>
        /// This method tries to get the type-save value for provided column an at 
        /// specified index.
        /// </summary>
        /// <remarks>
        /// This method is intended as an alternative to method <see cref="GetValue(Int32, Int32)"/> 
        /// because this method does not throw any exception.
        /// </remarks>
        /// <typeparam name="TType">
        /// The expected type to convert the string value into.
        /// </typeparam>
        /// <param name="column">
        /// The zero-based index of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <returns>
        /// An object representing the value for provided column and at specified index 
        /// or <c>null</c> if either a value for column and index could not be found or 
        /// an exception has occurred during type conversion.
        /// </returns>
        /// <seealso cref="CsvContainer.Exactly"/>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.GetValue(Int32, Int32)"/>
        /// <seealso cref="CsvContainer.GetObject(String, Type)"/>
        /// <seealso cref="TypeConverter.IntoObject(String, Type, Boolean, CultureInfo, CsvMappings)"/>
        public Object TryGetValue<TType>(Int32 column, Int32 index)
        {
            return this.TryGetValue(column, index, typeof(TType));
        }

        /// <summary>
        /// This method tries to get the type-save value for provided column an at 
        /// specified index taking provide type into account.
        /// </summary>
        /// <remarks>
        /// This method is intended as an alternative to method <see cref="GetValue(Int32, Int32, Type)"/> 
        /// because this method does not throw any exception.
        /// </remarks>
        /// <param name="column">
        /// The zero-based index of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <param name="type">
        /// The expected type to convert the string value into.
        /// </param>
        /// <returns>
        /// An object representing the value for provided column and at specified index 
        /// or <c>null</c> if either a value for column and index could not be found or 
        /// an exception has occurred during type conversion.
        /// </returns>
        /// <seealso cref="CsvContainer.Exactly"/>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.GetValue(Int32, Int32, Type)"/>
        /// <seealso cref="CsvContainer.GetObject(String, Type)"/>
        /// <seealso cref="TypeConverter.IntoObject(String, Type, Boolean, CultureInfo, CsvMappings)"/>
        public Object TryGetValue(Int32 column, Int32 index, Type type)
        {
            try
            {
                return GetValue(column, index, type);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// This method gets the type-save value for provided header and at specified 
        /// index.
        /// </summary>
        /// <remarks>
        /// First of all it is tried to get the string for provided header and at 
        /// specified index. Thereafter, the type converter is called by providing 
        /// additional information.
        /// </remarks>
        /// <typeparam name="TType">
        /// The expected type to convert the string value into.
        /// </typeparam>
        /// <param name="header">
        /// The header name of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <returns>
        /// An object representing the value for provided header and at specified index.
        /// </returns>
        /// <exception cref="Exception">
        /// This method might throw any of the exceptions of the <see cref="TypeConverter"/>.
        /// </exception>
        /// <seealso cref="CsvContainer.Exactly"/>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.TryGetValue(String, Int32)"/>
        /// <seealso cref="CsvContainer.GetObject(String, Type)"/>
        /// <seealso cref="TypeConverter.IntoObject(String, Type, Boolean, CultureInfo, CsvMappings)"/>
        public Object GetValue<TType>(String header, Int32 index)
        {
            return this.GetValue(header, index, typeof(TType));
        }

        /// <summary>
        /// This method gets the type-save value for provided header and at specified 
        /// index taking provide type into account.
        /// </summary>
        /// <remarks>
        /// First of all it is tried to get the string for provided header and at 
        /// specified index. Thereafter, the type converter is called by providing 
        /// additional information.
        /// </remarks>
        /// <param name="header">
        /// The header name of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <param name="type">
        /// The expected type to convert the string value into.
        /// </param>
        /// <returns>
        /// An object representing the value for provided header and at specified index.
        /// </returns>
        /// <exception cref="Exception">
        /// This method might throw any of the exceptions of the <see cref="TypeConverter"/>.
        /// </exception>
        /// <seealso cref="CsvContainer.Exactly"/>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.TryGetValue(String, Int32, Type)"/>
        /// <seealso cref="CsvContainer.GetObject(String, Type)"/>
        /// <seealso cref="TypeConverter.IntoObject(String, Type, Boolean, CultureInfo, CsvMappings)"/>        
        public Object GetValue(String header, Int32 index, Type type)
        {
            return this.GetObject(this[header, index], type);
        }

        /// <summary>
        /// This method tries to get the type-save value for provided header an at 
        /// specified index.
        /// </summary>
        /// <remarks>
        /// This method is intended as an alternative to method <see cref="GetValue(String, Int32)"/> 
        /// because this method does not throw any exception.
        /// </remarks>
        /// <typeparam name="TType">
        /// The expected type to convert the string value into.
        /// </typeparam>
        /// <param name="header">
        /// The header name of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <returns>
        /// An object representing the value for provided header and at specified index 
        /// or <c>null</c> if either a value for header and index could not be found or 
        /// an exception has occurred during type conversion.
        /// </returns>
        /// <seealso cref="CsvContainer.Exactly"/>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.GetValue(String, Int32)"/>
        /// <seealso cref="CsvContainer.GetObject(String, Type)"/>
        /// <seealso cref="TypeConverter.IntoObject(String, Type, Boolean, CultureInfo, CsvMappings)"/>
        public Object TryGetValue<TType>(String header, Int32 index)
        {
            return this.TryGetValue(header, index, typeof(TType));
        }

        /// <summary>
        /// This method tries to get the type-save value for provided header an at 
        /// specified index taking provide type into account.
        /// </summary>
        /// <remarks>
        /// This method is intended as an alternative to method <see cref="GetValue(String, Int32, Type)"/> 
        /// because this method does not throw any exception.
        /// </remarks>
        /// <param name="header">
        /// The zero-based index of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <param name="type">
        /// The expected type to convert the string value into.
        /// </param>
        /// <returns>
        /// An object representing the value for provided header and at specified index 
        /// or <c>null</c> if either a value for header and index could not be found or 
        /// an exception has occurred during type conversion.
        /// </returns>
        /// <seealso cref="CsvContainer.Exactly"/>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.GetValue(String, Int32, Type)"/>
        /// <seealso cref="CsvContainer.GetObject(String, Type)"/>
        /// <seealso cref="TypeConverter.IntoObject(String, Type, Boolean, CultureInfo, CsvMappings)"/>
        public Object TryGetValue(String header, Int32 index, Type type)
        {
            try
            {
                return GetValue(header, index, type);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// This method simply tries to convert provided string into its expected 
        /// object representation.
        /// </summary>
        /// <remarks>
        /// The value conversion takes place by using the internal type converter.
        /// </remarks>
        /// <param name="value">
        /// The string value to be converted.
        /// </param>
        /// <param name="type">
        /// The type to convert the value into.
        /// </param>
        /// <returns>
        /// An object representation of provided value.
        /// </returns>
        /// <exception cref="Exception">
        /// This method might throw any of the exceptions of the <see cref="TypeConverter"/>.
        /// </exception>
        /// <seealso cref="TypeConverter.IntoObject(String, Type, Boolean, CultureInfo, CsvMappings)"/>
        private Object GetObject(String value, Type type)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                return TypeConverter.IntoObject(value, type, this.Exactly, this.Culture, this.Mappings);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This method tries to determine the column for provided header.
        /// </summary>
        /// <remarks>
        /// The header of a CSV content is defined as its very first row. But such a 
        /// header row is actually just an optional value. Against this background this 
        /// method can only return a fitting column if the user knows that a particular 
        /// CSV content contains a header row. Therefore, treating the first item of each 
        /// column as header must be enabled by property <see cref="CsvContainer.Heading"/> 
        /// Otherwise, this method does never return a valid header column index.
        /// </remarks>
        /// <param name="header">
        /// The header name to get a column index for.
        /// </param>
        /// <returns>
        /// The index of the first fitting column. But <c>-1</c> is returned in case of 
        /// <see cref="CsvContainer.Heading"/> is disabled, <paramref name="header"/> name 
        /// is <c>null</c>, empty or whitespace, or header name could not be found.
        /// </returns>
        /// <seealso cref="CsvContainer.Heading"/>
        private Int32 HeaderToColumn(String header)
        {
            if (!this.Heading || String.IsNullOrWhiteSpace(header))
            {
                return -1;
            }

            if (this.Content.Count > 0)
            {
                for (Int32 column = 0; column < this.Content.Count; column++)
                {
                    if (String.Compare(header, this.Content[column][0], this.Compare) == 0)
                    {
                        return column;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// This method transforms line-oriented content into its column-oriented 
        /// representation.
        /// </summary>
        /// <remarks>
        /// The source list contains line-oriented content. But the result list 
        /// should contain column-oriented content. Therefore, a transformation 
        /// of the line-oriented content into its column-oriented representation 
        /// is required, and is done in this method.
        /// </remarks>
        /// <example>
        /// <para>
        /// Parameter <paramref name="source"/> may contain an array that looks 
        /// like shown below. Each position with dashes means "no data".
        /// </para>
        /// <code>
        /// 11, 12, 13, --
        /// 21, 22, 23, 24
        /// 31, 32  --  --
        /// </code>
        /// <para>
        /// Task of this method is to convert the above array into its column-oriented 
        /// version like shown as follows. Each position with dashes means "empty data".
        /// </para>
        /// <code>
        /// 11, 21, 31
        /// 12, 22, 32
        /// 13, 23, --
        /// --, 24, --
        /// </code>
        /// <para>
        /// With this transformation each of the lines in the result represents one 
        /// column inside the original source. 
        /// </para>
        /// </example>
        /// <param name="source">
        /// The line-oriented source data.
        /// </param>
        /// <returns>
        /// The column-oriented representation of source data. An empty array is 
        /// returned if the source array is empty.
        /// </returns>
        private List<List<String>> Transform(List<List<String>> source)
        {
            // TODO: Find an optimization for the "many" loops.

            List<List<String>> result = new List<List<String>>();

            if (source != null)
            {
                Int32 length = 0;

                // Calculate longest line length.
                for (Int32 outer = 0; outer < source.Count; outer++)
                {
                    if (length < source[outer].Count)
                    {
                        length = source[outer].Count;
                    }
                }

                // Prepare an initial transform column.
                List<String> initial = new List<String>(source.Count);

                for (Int32 index = 0; index < initial.Capacity; index++)
                {
                    initial.Add(/*String.Empty*/null);
                }

                // Create each transformed row.
                for (Int32 index = 0; index < length; index++)
                {
                    result.Add(new List<String>(initial));
                }

                // Ready to transform: 
                // Swap all values 
                //  * from "source[outer, inner]" 
                //  * into "result[inner, outer]".
                for (Int32 outer = 0; outer < source.Count; outer++)
                {
                    for (Int32 inner = 0; inner < source[outer].Count; inner++)
                    {
                        result[inner][outer] = source[outer][inner];
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
