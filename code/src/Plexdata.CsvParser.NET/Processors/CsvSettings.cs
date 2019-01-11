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

using Plexdata.CsvParser.Constants;
using System;
using System.Globalization;
using System.Text;

namespace Plexdata.CsvParser.Processors
{
    /// <summary>
    /// This class describes how CSV exports as well as imports have to be handled.
    /// </summary>
    /// <remarks>
    /// The settings class provides information for both, importing as well as exporting CSV 
    /// data. But not all of the properties are used in a particular situation. For example, 
    /// the 'Textual' property is only used while a data export. In contrast to that, the 
    /// 'Exactly' property is only relevant for a data import. Against this, the 'Culture' 
    /// property is used for both tasks.
    /// </remarks>
    public class CsvSettings
    {
        #region Private fields

        /// <summary>
        /// The delimiter to be used to separate each CSV column. 
        /// </summary>
        /// <remarks>
        /// The field is initialized with a comma, which represents the default separator.
        /// </remarks>
        /// <seealso cref="ColumnSeparators.DefaultSeparator"/>
        private Char separator = ColumnSeparators.DefaultSeparator;

        /// <summary>
        /// The encoding to be used to read or write a CSV file.
        /// </summary>
        /// <remarks>
        /// The field is initialized with UTF-8 encoding, which fits in most cases.
        /// </remarks>
        private Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// The flag to enable or disable the usage of a header. 
        /// </summary>
        /// <remarks>
        /// The field is initialized with 'true'.
        /// </remarks>
        private Boolean heading = true;

        /// <summary>
        /// The flag to enable or disable the textual treatment. 
        /// </summary>
        /// <remarks>
        /// The field is initialized with 'false'.
        /// </remarks>
        private Boolean textual = false;

        /// <summary>
        /// The flag to enable or disable the exactly treatment. 
        /// </summary>
        /// <remarks>
        /// The field is initialized with 'false'.
        /// </remarks>
        private Boolean exactly = false;

        /// <summary>
        /// The culture information to be used to read or write a CSV file. 
        /// </summary>
        /// <remarks>
        /// The field is initialized with current UI culture.
        /// </remarks>
        private CultureInfo culture = CultureInfo.CurrentUICulture;

        /// <summary>
        /// The mapping to be used for value transformation. 
        /// </summary>
        /// <remarks>
        /// The field is initialized with <see cref="CsvMappings.DefaultMappings"/>.
        /// </remarks>
        private CsvMappings mapping = CsvMappings.DefaultMappings;

        #endregion

        #region Construction

        /// <summary>
        /// Default class construction.
        /// </summary>
        /// <remarks>
        /// The default constructor does nothing but its basic initialisation.
        /// </remarks>
        public CsvSettings()
            : base()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the character that separates each CSV column. Default 
        /// value is a comma.
        /// </summary>
        /// <remarks>
        /// The separator character is used to determine the split position in each 
        /// line of a CSV file. Usually a comma is used for this purpose. But because 
        /// of missing a clear definition there are many CSV files that uses various 
        /// different characters. Some of the examples are: semicolon, tabulator, colon 
        /// and sometimes spaces as well. With this property users can define the character 
        /// to be used to split the content of any CSV file.
        /// </remarks>
        /// <value>
        /// The Separator character which splits or combines CSV file content.
        /// </value>
        /// <exception cref="ArgumentException">
        /// This exception is thrown if given value is a control character, except the 
        /// tabulator character.
        /// </exception>
        /// <seealso cref="ColumnSeparators"/>
        public Char Separator
        {
            get
            {
                return this.separator;
            }
            set
            {
                if (value != ColumnSeparators.TabulatorSeparator && Char.IsControl(value))
                {
                    throw new ArgumentException("The column separator cannot be a control character.", nameof(this.Separator));
                }

                this.separator = value;
            }
        }

        /// <summary>
        /// Gets or sets the the expected file encoding. Default value is UTF-8.
        /// </summary>
        /// <remarks>
        /// File encoding is relevant for both, importing and exporting CSV data. 
        /// It describes how data have to be handled during loading as well as during 
        /// saving. For most cases UTF-8 file encoding is a good choice.
        /// </remarks>
        /// <value>
        /// The Encoding property allows to control file content processing behavior.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown if given value is &lt;null&gt;.
        /// </exception>
        public Encoding Encoding
        {
            get
            {
                return this.encoding;
            }
            set
            {
                this.encoding = value ?? throw new ArgumentNullException(nameof(this.Encoding), "The encoding cannot be <null>.");
            }
        }

        /// <summary>
        /// Enables or disables the header usage. Default value is true.
        /// </summary>
        /// <remarks>
        /// If true, then the header is written into the output. In this case the 
        /// header information is taken from the column attributes. In case of a 
        /// column attribute does not contain header information then the property 
        /// name is taken as header instead. Otherwise, the header is excluded.
        /// </remarks>
        /// <value>
        /// The Heading property allows to control header processing behavior.
        /// </value>
        public Boolean Heading
        {
            get
            {
                return this.heading;
            }
            set
            {
                this.heading = value;
            }
        }

        /// <summary>
        /// Enables or disables the textual mode. Default value is false.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If true, then any textual data types are enclosed in double-quotes. This 
        /// would overwrite the default behaviour. Default behaviour means that only 
        /// those textual data types are enclosed in double-quotes which contain 
        /// control characters such as the separator, carriage returns, line feeds, 
        /// and/or double-quotes. 
        /// </para>
        /// <para>
        /// The textual treatment is only relevant for a CSV data export.
        /// </para>
        /// </remarks>
        /// <value>
        /// The Textual property allows to control text processing behavior.
        /// </value>
        public Boolean Textual
        {
            get
            {
                return this.textual;
            }
            set
            {
                this.textual = value;
            }
        }

        /// <summary>
        /// Enables or disables the exactly mode. Default value is false.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If true, then the column heading as well as the column order must 
        /// exactly fit the data type definition, but only if heading mode is 
        /// enabled as well. 
        /// </para>
        /// <para>
        /// Furthermore, exactly treatment also applies to data type conversion. 
        /// For example, an exceptions is thrown if exactly mode is enabled and 
        /// one of the imported data items could not be converted.
        /// </para>
        /// <para>
        /// The exactly treatment is only relevant for a CSV data import.
        /// </para>
        /// </remarks>
        /// <value>
        /// True if the exactly mode is enabled, and false otherwise.
        /// </value>
        public Boolean Exactly
        {
            get
            {
                return this.exactly;
            }
            set
            {
                this.exactly = value;
            }
        }

        /// <summary>
        /// Gets or sets the culture to be used. Default value is current UI culture.
        /// </summary>
        /// <remarks>
        /// The used culture applies to number conversion as well as to other culture-dependent 
        /// data types. For example using German culture will treat number such as 1.234,56 
        /// as valid decimals. In contrast to that, using an English culture will treat a 
        /// number like 1,234.56 as valid decimal value.
        /// </remarks>
        /// <value>
        /// The culture to be used for example for converting numbers.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown if given value is &lt;null&gt;.
        /// </exception>
        public CultureInfo Culture
        {
            get
            {
                return this.culture;
            }
            set
            {
                this.culture = value ?? throw new ArgumentNullException(nameof(this.Culture), "The culture cannot be <null>.");
            }
        }

        /// <summary>
        /// Gets or sets the mapping to be used for value transformation. Default value is 
        /// standard mapping.
        /// </summary>
        /// <remarks>
        /// The mapping is used to interpret values while importing respectively while exporting 
        /// data. For example an imported CSV file contains 'yes' and 'no' for Boolean values. 
        /// In such a case the mapping is used to tell the importer that 'yes' means 'true' and 
        /// 'no' means 'false'.
        /// </remarks>
        /// <value>
        /// The mapping to be used for value transformation. 
        /// </value>
        public CsvMappings Mappings
        {
            get
            {
                return this.mapping;
            }
            set
            {
                this.mapping = value ?? CsvMappings.DefaultMappings;
            }

        }

        #endregion

        #region Public methods

        /// <summary>
        /// This method returns a string containing current instance information.
        /// </summary>
        /// <remarks>
        /// This method has been overwritten and returns a string showing current instance values.
        /// </remarks>
        /// <returns>
        /// A string consisting of details of current instance information.
        /// </returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder(256);
            result.Append($"Separator: \"{this.Separator}\", ");
            result.Append($"Encoding: \"{this.Encoding.WebName}\", ");
            result.Append($"Heading: \"{this.Heading}\", ");
            result.Append($"Textual: \"{this.Textual}\", ");
            result.Append($"Exactly: \"{this.Exactly}\", ");
            result.Append($"Culture: \"{this.Culture.Name}\", ");
            result.Append($"Mappings: {this.Mappings}");
            return result.ToString();
        }

        #endregion
    }
}
