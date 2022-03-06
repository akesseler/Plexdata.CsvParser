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
        /// The constructor with table initialization.
        /// </summary>
        /// <remarks>
        /// This constructor creates an instance of class <see cref="CsvContainer"/> and 
        /// initializes its internal table according to provided width and length.
        /// </remarks>
        /// <param name="width">
        /// The width (number of columns) of the internal table.
        /// </param>
        /// <param name="length">
        /// The length (number of rows) of the internal table.
        /// </param>
        public CsvContainer(Int32 width, Int32 length)
            : this(width, length, null)
        {
        }

        /// <summary>
        /// The constructor with table initialization.
        /// </summary>
        /// <remarks>
        /// This constructor creates an instance of class <see cref="CsvContainer"/> and 
        /// initializes its internal table according to provided width and length.
        /// </remarks>
        /// <param name="width">
        /// The width (number of columns) of the internal table.
        /// </param>
        /// <param name="length">
        /// The length (number of rows) of the internal table.
        /// </param>
        /// <param name="settings">
        /// The settings to be used. Default settings will be used if this argument is <c>null</c>.
        /// </param>
        /// <seealso cref="CsvContainer.CreateDefaultSettings()"/>
        public CsvContainer(Int32 width, Int32 length, CsvSettings settings)
            : this(CsvContainer.CreateInitialContent(width, length), settings)
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
        public CsvContainer(List<List<String>> content)
            : this(content, null)
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
        /// <param name="settings">
        /// The settings to be used. Default settings will be used if this argument is <c>null</c>.
        /// </param>
        /// <seealso cref="CsvContainer.CreateDefaultSettings()"/>
        public CsvContainer(List<List<String>> content, CsvSettings settings)
              : base()
        {
            settings = settings ?? CsvContainer.CreateDefaultSettings();

            this.Content = this.Transform(content);
            this.Compare = StringComparison.OrdinalIgnoreCase;
            this.Culture = settings.Culture;
            this.Mappings = settings.Mappings;
            this.Heading = settings.Heading;
            this.Exactly = settings.Exactly;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets and sets the list of column items at specified position.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This getter validates provided column number and if valid it returns the 
        /// column as enumerable list. But <c>null</c> is returned if provided column 
        /// index is either less than zero or greater than total column count.
        /// </para>
        /// <para>
        /// The setter instead tries to change all affected elements at provided column.
        /// Only possible elements are overwritten by its new content in case of source 
        /// list is longer than referenced column. In contrast to that all remaining 
        /// elements are reset to <c>null</c> in case of source list is shorter than 
        /// referenced column.
        /// </para>
        /// <para>
        /// Additionally please note that nothing will happen if the source list is 
        /// <c>null</c> or empty.
        /// </para>
        /// <para>
        /// <b>Attention</b>: Any existing column header might be overwritten if new 
        /// column content does not contain the original header!
        /// </para>
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
            set
            {
                if (value is null)
                {
                    return;
                }

                if (0 <= column && column < this.Content.Count)
                {
                    this.ChangeColumn(column, new List<String>(value));
                }
            }
        }

        /// <summary>
        /// Gets and sets an element for provided column and at specified index.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The getter validates provided column as well as specified index and if 
        /// both are valid it returns the element at column and index. But <c>null</c> 
        /// is returned either if provided column number or specified index is less 
        /// than zero or greater than total column count.
        /// </para>
        /// <para>
        /// Please be aware, the column header (if exist) is returned for an index 
        /// of zero. The value at index zero is returned instead if no header exists. 
        /// The other way round, this accessor always returns the value referenced 
        /// by column and index without respecting the presents of a column header.
        /// </para>
        /// <para>
        /// The setter instead changes the value at provided column and index but 
        /// only if both (column and index) are valid. 
        /// </para>
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
            set
            {
                if (0 <= column && column < this.Content.Count)
                {
                    if (0 <= index && index < this.Content[column].Count)
                    {
                        this.Content[column][index] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets and sets the list of column items for provided header.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This getter tries to find the column where its top level item fits 
        /// provided header name and returns the whole list of column items. Very 
        /// important to note, calling this getter will only have any effect if 
        /// Heading is enabled.
        /// </para>
        /// <para>
        /// The setter instead tries to change all affected elements at column 
        /// referenced by provided header. Only possible elements are overwritten by 
        /// its new content in case of source list is longer than column referenced 
        /// by header. In contrast to that all remaining elements are reset to <c>null</c> 
        /// in case of source list is shorter than column referenced by provided header.
        /// </para>
        /// <para>
        /// Additionally please note that nothing will happen if the source list is 
        /// <c>null</c> or empty.
        /// </para>
        /// <para>
        /// <b>Attention</b>: Any existing column header might be overwritten if new 
        /// column content does not contain the original header!
        /// </para>
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
        /// <seealso cref="CsvContainer.GetColumnIndex(String)"/>
        public IEnumerable<String> this[String header]
        {
            get
            {
                Int32 column = this.GetColumnIndex(header);

                if (column >= 0)
                {
                    return this[column];
                }

                return null;
            }
            set
            {
                if (value is null)
                {
                    return;
                }

                Int32 column = this.GetColumnIndex(header);

                if (column >= 0)
                {
                    this[column] = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets an element for provided header and at specified index.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The getter tries to find the column where its top level item fits 
        /// provided header name. Thereafter, the specified index is validated.
        /// The content is returned if both conditions could be confirmed. Very 
        /// important to note, calling this getter will only have any effect if 
        /// Heading is enabled and a header really exists. Otherwise, the value 
        /// of index plus one is returned.
        /// </para>
        /// <para>
        /// The setter instead changes the value at column identified by provided 
        /// header and corresponding index. Also important to note, calling this 
        /// setter will only have any effect if Heading is enabled and a header 
        /// really exists. Otherwise, the value of index plus one will be changed.
        /// </para>
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
        /// <seealso cref="this[Int32, Int32]"/>
        /// <seealso cref="CsvContainer.Heading"/>
        /// <seealso cref="CsvContainer.GetColumnIndex(String)"/>
        public String this[String header, Int32 index]
        {
            get
            {
                Int32 column = this.GetColumnIndex(header);

                if (column >= 0 && index >= 0)
                {
                    return this[column, index + 1];
                }

                return null;
            }
            set
            {
                Int32 column = this.GetColumnIndex(header);

                if (column >= 0 && index >= 0)
                {
                    this[column, index + 1] = value;
                }
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

        /// <summary>
        /// Gets the width (total number of columns) of the content.
        /// </summary>
        /// <remarks>
        /// This getter returns total number of columns of the underlying CSV 
        /// content.
        /// </remarks>
        /// <value>
        /// The total number of available columns.
        /// </value>
        public Int32 Width
        {
            get
            {
                return this.Content.Count;
            }
        }

        /// <summary>
        /// Gets the length (total number of rows) of the content.
        /// </summary>
        /// <remarks>
        /// This getter returns the total number of rows of the underlying CSV 
        /// content.
        /// </remarks>
        /// <value>
        /// The total number of available rows.
        /// </value>
        public Int32 Length
        {
            get
            {
                if (this.Content.Count > 0)
                {
                    return this.Content[0].Count;
                }

                return 0;
            }
        }

        #endregion

        #region Public methods

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
        public Int32 GetColumnIndex(String header)
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
        /// Gets the transformed content of this container.
        /// </summary>
        /// <remarks>
        /// This method allows to get the transformed content from this container.
        /// Such a transformation is necessary because of the container uses a 
        /// "rotated table" for a better item access.
        /// </remarks>
        /// <returns>
        /// The transformed content. The result might be empty in case of no content.
        /// </returns>
        /// <seealso cref="CsvContainer.Content"/>
        /// <seealso cref="CsvContainer.Transform(List{List{String}})"/>
        public List<List<String>> GetTransformedContent()
        {
            return this.Transform(this.Content);
        }

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
        /// or <c>null</c> either if a value for column and index could not be found or 
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
        /// or <c>null</c> either if a value for column and index could not be found or 
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
                return this.GetValue(column, index, type);
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
        /// or <c>null</c> either if a value for header and index could not be found or 
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
        /// The header name of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <param name="type">
        /// The expected type to convert the string value into.
        /// </param>
        /// <returns>
        /// An object representing the value for provided header and at specified index 
        /// or <c>null</c> either if a value for header and index could not be found or 
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
                return this.GetValue(header, index, type);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Sets provided <paramref name="value"/> of type <typeparamref name="TType"/> 
        /// at field referenced by <paramref name="column"/> and <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        /// First of all, provided value is converted into its string representation. As next 
        /// this method tries to change current value in field referenced by provided column 
        /// and index. The content remains unchanged if either the column or the index is out 
        /// of range.
        /// </remarks>
        /// <typeparam name="TType">
        /// The type to convert the value into a string.
        /// </typeparam>
        /// <param name="value">
        /// The value to be set at provided column and index.
        /// </param>
        /// <param name="column">
        /// The zero-based index of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.SetValue(Object, Int32, Int32)"/>
        /// <seealso cref="CsvContainer.TrySetValue{TType}(TType, Int32, Int32)"/>
        /// <seealso cref="TypeConverter.IntoString(Object, CultureInfo, CsvMappings)"/>
        public void SetValue<TType>(TType value, Int32 column, Int32 index)
        {
            this.SetValue(value as Object, column, index);
        }

        /// <summary>
        /// Sets provided <paramref name="value"/> at field referenced by 
        /// <paramref name="column"/> and <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        /// First of all, provided value is converted into its string representation. As next 
        /// this method tries to change current value in field referenced by provided column 
        /// and index. The content remains unchanged if either the column or the index is out 
        /// of range.
        /// </remarks>
        /// <param name="value">
        /// The value to be set at provided column and index.
        /// </param>
        /// <param name="column">
        /// The zero-based index of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.SetValue{TType}(TType, Int32, Int32)"/>
        /// <seealso cref="CsvContainer.TrySetValue(Object, Int32, Int32)"/>
        /// <seealso cref="TypeConverter.IntoString(Object, CultureInfo, CsvMappings)"/>
        public void SetValue(Object value, Int32 column, Int32 index)
        {
            this[column, index] = TypeConverter.IntoString(value, this.Culture, this.Mappings);
        }

        /// <summary>
        /// Tries to set provided <paramref name="value"/> of type <typeparamref name="TType"/> 
        /// at field referenced by <paramref name="column"/> and <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        /// First of all, provided value is converted into its string representation. As next 
        /// this method tries to change current value in field referenced by provided column 
        /// and index. The content remains unchanged if either the column or the index is out 
        /// of range.
        /// </remarks>
        /// <typeparam name="TType">
        /// The type to convert the value into a string.
        /// </typeparam>
        /// <param name="value">
        /// The value to be set at provided column and index.
        /// </param>
        /// <param name="column">
        /// The zero-based index of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <returns>
        /// True on success and false on an exception caught.
        /// </returns>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.SetValue{TType}(TType, Int32, Int32)"/>
        /// <seealso cref="CsvContainer.SetValue(Object, Int32, Int32)"/>
        /// <seealso cref="TypeConverter.IntoString(Object, CultureInfo, CsvMappings)"/>
        public Boolean TrySetValue<TType>(TType value, Int32 column, Int32 index)
        {
            try
            {
                this.SetValue<TType>(value, column, index);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to set provided <paramref name="value"/> at field referenced by 
        /// <paramref name="column"/> and <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        /// First of all, provided value is converted into its string representation. As next 
        /// this method tries to change current value in field referenced by provided column 
        /// and index. The content remains unchanged if either the column or the index is out 
        /// of range.
        /// </remarks>
        /// <param name="value">
        /// The value to be set at provided column and index.
        /// </param>
        /// <param name="column">
        /// The zero-based index of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <returns>
        /// True on success and false on an exception caught.
        /// </returns>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.SetValue(Object, Int32, Int32)"/>
        /// <seealso cref="CsvContainer.TrySetValue{TType}(TType, Int32, Int32)"/>
        /// <seealso cref="TypeConverter.IntoString(Object, CultureInfo, CsvMappings)"/>
        public Boolean TrySetValue(Object value, Int32 column, Int32 index)
        {
            try
            {
                this.SetValue(value, column, index);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets provided <paramref name="value"/> of type <typeparamref name="TType"/> 
        /// at field referenced by <paramref name="header"/> and <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        /// First of all, provided value is converted into its string representation. As next 
        /// this method tries to change current value in field referenced by provided column 
        /// and index. The content remains unchanged if either the column or the index is out 
        /// of range.
        /// </remarks>
        /// <typeparam name="TType">
        /// The type to convert the value into a string.
        /// </typeparam>
        /// <param name="value">
        /// The value to be set at provided column and index.
        /// </param>
        /// <param name="header">
        /// The header name of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.SetValue(Object, String, Int32)"/>
        /// <seealso cref="CsvContainer.TrySetValue{TType}(TType, String, Int32)"/>
        /// <seealso cref="TypeConverter.IntoString(Object, CultureInfo, CsvMappings)"/>
        public void SetValue<TType>(TType value, String header, Int32 index)
        {
            this.SetValue(value as Object, header, index);
        }

        /// <summary>
        /// Sets provided <paramref name="value"/> at field referenced by 
        /// <paramref name="header"/> and <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        /// First of all, provided value is converted into its string representation. As next 
        /// this method tries to change current value in field referenced by provided column 
        /// and index. The content remains unchanged if either the column or the index is out 
        /// of range.
        /// </remarks>
        /// <param name="value">
        /// The value to be set at provided column and index.
        /// </param>
        /// <param name="header">
        /// The header name of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.SetValue{TType}(TType, String, Int32)"/>
        /// <seealso cref="CsvContainer.TrySetValue(Object, String, Int32)"/>
        /// <seealso cref="TypeConverter.IntoString(Object, CultureInfo, CsvMappings)"/>
        public void SetValue(Object value, String header, Int32 index)
        {
            this[header, index] = TypeConverter.IntoString(value, this.Culture, this.Mappings);
        }

        /// <summary>
        /// Tries to set provided <paramref name="value"/> of type <typeparamref name="TType"/> 
        /// at field referenced by <paramref name="header"/> and <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        /// First of all, provided value is converted into its string representation. As next 
        /// this method tries to change current value in field referenced by provided column 
        /// and index. The content remains unchanged if either the column or the index is out 
        /// of range.
        /// </remarks>
        /// <typeparam name="TType">
        /// The type to convert the value into a string.
        /// </typeparam>
        /// <param name="value">
        /// The value to be set at provided column and index.
        /// </param>
        /// <param name="header">
        /// The header name of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <returns>
        /// True on success and false on an exception caught.
        /// </returns>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.SetValue{TType}(TType, String, Int32)"/>
        /// <seealso cref="CsvContainer.SetValue(Object, String, Int32)"/>
        /// <seealso cref="TypeConverter.IntoString(Object, CultureInfo, CsvMappings)"/>
        public Boolean TrySetValue<TType>(TType value, String header, Int32 index)
        {
            try
            {
                this.SetValue<TType>(value, header, index);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to set provided <paramref name="value"/> at field referenced by 
        /// <paramref name="header"/> and <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        /// First of all, provided value is converted into its string representation. As next 
        /// this method tries to change current value in field referenced by provided column 
        /// and index. The content remains unchanged if either the column or the index is out 
        /// of range.
        /// </remarks>
        /// <param name="value">
        /// The value to be set at provided column and index.
        /// </param>
        /// <param name="header">
        /// The header name of affected column.
        /// </param>
        /// <param name="index">
        /// The zero-based index of affected row.
        /// </param>
        /// <returns>
        /// True on success and false on an exception caught.
        /// </returns>
        /// <seealso cref="CsvContainer.Culture"/>
        /// <seealso cref="CsvContainer.Mappings"/>
        /// <seealso cref="CsvContainer.SetValue(Object, String, Int32)"/>
        /// <seealso cref="CsvContainer.TrySetValue{TType}(TType, String, Int32)"/>
        /// <seealso cref="TypeConverter.IntoString(Object, CultureInfo, CsvMappings)"/>
        public Boolean TrySetValue(Object value, String header, Int32 index)
        {
            try
            {
                this.SetValue(value, header, index);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Creates an initial content according to provided <paramref name="width"/> 
        /// and <paramref name="length"/>.
        /// </summary>
        /// <remarks>
        /// This method creates an initial content according to provided <paramref name="width"/> 
        /// and <paramref name="length"/>.
        /// </remarks>
        /// <param name="width">
        /// The width (number of columns) of the internal table.
        /// </param>
        /// <param name="length">
        /// The length (number of rows) of the internal table.
        /// </param>
        /// <returns>
        /// An internal table where each field is initialized by a <c>null</c> string value.
        /// </returns>
        private static List<List<String>> CreateInitialContent(Int32 width, Int32 length)
        {
            List<List<String>> result = new List<List<String>>();

            for (Int32 outer = 0; outer < length; outer++)
            {
                result.Add(new List<String>());

                for (Int32 inner = 0; inner < width; inner++)
                {
                    result[outer].Add(null);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates an instance of class <see cref="CsvSettings"/> and initializes it with 
        /// relevant default values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates an instance of class <see cref="CsvSettings"/> and initializes 
        /// it with relevant default values.
        /// </para>
        /// <para>
        /// Relevant default values means indeed that only a subset of all settings properties 
        /// is required. These properties are listed below.
        /// </para>
        /// <list type="table"> 
        /// <listheader><term>Property</term><description>Description</description></listheader>
        /// <item><term>Culture</term><description>The property <see cref="CsvSettings.Culture"/> 
        /// is initialized with <see cref="CultureInfo.CurrentUICulture"/>.</description></item>  
        /// <item><term>Mappings</term><description>The property <see cref="CsvSettings.Mappings"/> 
        /// is initialized with <see cref="CsvMappings.DefaultMappings"/>.</description></item>  
        /// <item><term>Exactly</term><description>The property <see cref="CsvSettings.Exactly"/> 
        /// is set to false.</description></item>  
        /// <item><term>Heading</term><description>The property <see cref="CsvSettings.Heading"/> 
        /// is set to false.</description></item>  
        /// </list>
        /// <para>
        /// All other properties of class <see cref="CsvSettings"/> are not used inhere and remain 
        /// therefore untouched.
        /// </para>
        /// </remarks>
        /// <returns>
        /// An initialized instance of class <see cref="CsvSettings"/>.
        /// </returns>
        private static CsvSettings CreateDefaultSettings()
        {
            return new CsvSettings()
            {
                Culture = CultureInfo.CurrentUICulture,
                Mappings = CsvMappings.DefaultMappings,
                Exactly = false,
                Heading = false,
            };
        }

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
        /// This method transforms line-oriented content into its column-oriented 
        /// representation.
        /// </summary>
        /// <remarks>
        /// The source list contains line-oriented content. But the result list should 
        /// contain column-oriented content (rotated by 90 degree anticlockwise), which 
        /// significantly simplifies accessing a single column or a row. Therefore, a 
        /// transformation of the line-oriented content into its column-oriented version 
        /// is done in this method.
        /// </remarks>
        /// <example>
        /// <para>
        /// Parameter <paramref name="source"/> may contain an array that looks like as 
        /// shown below. Each position with dashes means "no data".
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
        /// With this transformation each of the lines in the result represents one column 
        /// inside the original source. 
        /// </para>
        /// </example>
        /// <param name="source">
        /// The line-oriented source data.
        /// </param>
        /// <returns>
        /// The column-oriented representation of source data. An empty array is returned 
        /// if the source array is empty.
        /// </returns>
        private List<List<String>> Transform(List<List<String>> source)
        {
            // TODO: Find an optimization for the "many" loops.
            //       Using Linq could be an option, but it does not
            //       really produce a performance advantage.

            List<List<String>> result = new List<List<String>>();

            if (this.IsValid(source))
            {
                Int32 length = 0;

                // Determine longest line length.
                for (Int32 outer = 0; outer < source.Count; outer++)
                {
                    Int32 count = source[outer]?.Count ?? 0;

                    if (length < count)
                    {
                        length = count;
                    }
                }

                // Prepare an initial transform column.
                List<String> initial = new List<String>(source.Count);

                for (Int32 index = 0; index < initial.Capacity; index++)
                {
                    initial.Add(null);
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
                    Int32 count = source[outer]?.Count ?? 0;

                    for (Int32 inner = 0; inner < count; inner++)
                    {
                        result[inner][outer] = source[outer][inner];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether source list is valid.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method determines whether source list contains at least one item.
        /// </para>
        /// <para>
        /// In best case this method already find the very first Item at column zero 
        /// and index zero. In worst case loops through all source list lines.
        /// </para>
        /// </remarks>
        /// <param name="source">
        /// The list to check.
        /// </param>
        /// <returns>
        /// True ist returned if one of the inner lists contains at least one item.
        /// False is returned in any other case.
        /// </returns>
        private Boolean IsValid(List<List<String>> source)
        {
            if (source is null)
            {
                return false;
            }

            foreach (List<String> inner in source)
            {
                if (inner?.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Changes the whole content of affected column.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method changes the whole content of affected column.
        /// </para>
        /// <para>
        /// In case of source list is longer than affected column only the possible 
        /// elements are taken into account.
        /// </para>
        /// <para>
        /// The other way round, in case of source list is shorter than affected 
        /// column all remaining column elements are reset to <c>null</c>.
        /// </para>
        /// <para>
        /// Additionally please note that nothing will happen if the source list 
        /// is <c>null</c> 
        /// or empty.
        /// </para>
        /// </remarks>
        /// <param name="column">
        /// The zero-based index of affected column to change.
        /// </param>
        /// <param name="source">
        /// The new column content.
        /// </param>
        private void ChangeColumn(Int32 column, List<String> source)
        {
            if (source.Count < 1)
            {
                return;
            }

            List<String> target = this.Content[column];

            Int32 length = Math.Min(target.Count, source.Count);

            for (Int32 index = 0; index < length; index++)
            {
                target[index] = source[index];
            }

            for (Int32 index = length; index < target.Count; index++)
            {
                target[index] = null;
            }
        }

        #endregion
    }
}
