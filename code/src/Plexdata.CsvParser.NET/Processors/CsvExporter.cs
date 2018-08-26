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

using Plexdata.CsvParser.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Plexdata.CsvParser.Processors
{
    /// <summary>
    /// This static class allows writing a value list of <typeparamref name="TInstance"/> 
    /// types either into a file or simply into a stream (which actually can also be a file).
    /// </summary>
    /// <remarks>
    /// <para>
    /// CSV actually means Comma Separated Values. Sometimes it is also called as Character 
    /// Separated Values. But not matter which name is used, CSV always represents a text file 
    /// mainly used for data exchange between different system.
    /// </para>
    /// <para>
    /// It would be possible (using a proper configuration) to write a CSV output according to 
    /// the rules of RFC 4180. For more information about RFC 4180 please visit the web-site under 
    /// <see href="https://www.ietf.org/rfc/rfc4180.txt">https://www.ietf.org/rfc/rfc4180.txt</see>
    /// </para>
    /// </remarks>
    /// <typeparam name="TInstance">
    /// The class type that fully describes the structure of the CSV output to be written.
    /// </typeparam>
    /// <example>
    /// This section wants to show a simple but hopefully useful example of how to use the CSV 
    /// Parser for data exports.
    /// <code>
    /// using Plexdata.CsvParser.Attributes;
    /// using Plexdata.CsvParser.Processors;
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Globalization;
    /// 
    /// namespace MyCsvExporter
    /// {
    ///     class Program
    ///     {
    ///         [CsvDocument]
    ///         class CsvCustomer
    ///         {
    ///             [CsvIgnore]
    ///             public Int32 Id { get; set; }
    /// 
    ///             [CsvColumn(Offset = 2, Header = "Identifier")]
    ///             public Int32 ExternalId { get; set; }
    /// 
    ///             [CsvColumn(Offset = 1, Header = "Forename")]
    ///             public String FirstName { get; set; }
    /// 
    ///             [CsvColumn(Offset = 0, Header = "Surname")]
    ///             public String LastName { get; set; }
    /// 
    ///             [CsvColumn(Offset = 5, Header = "Active")]
    ///             public Boolean IsActive { get; set; }
    /// 
    ///             [CsvColumn(Offset = 3, Header = "Date")]
    ///             public DateTime? EntryDate { get; set; }
    /// 
    ///             [CsvColumn(Offset = 4, Header = "Sales")]
    ///             public Decimal SalesAverage { get; set; }
    /// 
    ///             [CsvColumn(Offset = 6, Header = "Notes")]
    ///             public String Description { get; set; }
    ///         }
    /// 
    ///         static void Main(string[] args)
    ///         {
    ///             try
    ///             {
    ///                 String filename = @"C:\folder\file.csv";
    /// 
    ///                 List&lt;CsvCustomer&gt; customers = new List&lt;CsvCustomer&gt;
    ///                 {
    ///                     new CsvCustomer {
    ///                         LastName = "Marley",
    ///                         FirstName = "Bob",
    ///                         ExternalId = 1001,
    ///                         EntryDate = new DateTime(2007, 5, 3),
    ///                         SalesAverage = 1234.56m,
    ///                         IsActive = false,
    ///                         Description = "Have a short note here." },
    ///                     new CsvCustomer {
    ///                         LastName = "Monroe",
    ///                         FirstName = "Marilyn",
    ///                         ExternalId = 1002,
    ///                         EntryDate = new DateTime(2008, 6, 5),
    ///                         SalesAverage = 1234.56m,
    ///                         IsActive = false,
    ///                         Description = null },
    ///                     new CsvCustomer {
    ///                         LastName = "Snipes",
    ///                         FirstName = "Wesley",
    ///                         ExternalId = 1003,
    ///                         EntryDate = new DateTime(2009, 7, 6),
    ///                         SalesAverage = 1234.56m,
    ///                         IsActive = true,
    ///                         Description = "Have a short note here." },
    ///                     new CsvCustomer {
    ///                         LastName = "Hurley",
    ///                         FirstName = "Elizabeth",
    ///                         ExternalId = 1004,
    ///                         EntryDate = new DateTime(2005, 8, 8),
    ///                         SalesAverage = 1234.56m,
    ///                         IsActive = true,
    ///                         Description = "Have a short note here." },
    ///                 };
    /// 
    ///                 CsvSettings settings = new CsvSettings
    ///                 {
    ///                     Culture = CultureInfo.GetCultureInfo("en-US"),
    ///                     Textual = true,
    ///                     Mappings = new CsvMappings
    ///                     {
    ///                         TrueValue = "yeah",
    ///                         FalseValue = "nope",
    ///                     },
    ///                 };
    /// 
    ///                 // Output file would contain this content:
    ///                 // Surname,Forename,Identifier,Date,Sales,Active,Notes
    ///                 // "Marley","Bob",1001,2007-05-03T00:00:00,1234.56,nope,"Have a short note here."
    ///                 // "Monroe","Marilyn",1002,2008-06-05T00:00:00,1234.56,nope,
    ///                 // "Snipes","Wesley",1003,2009-07-06T00:00:00,1234.56,yeah,"Have a short note here."
    ///                 // "Hurley","Elizabeth",1004,2005-08-08T00:00:00,1234.56,yeah,"Have a short note here."
    /// 
    ///                 CsvExporter&lt;CsvCustomer&gt;.Save(customers, filename, settings);
    /// 
    ///                 Console.ReadKey();
    ///             }
    ///             catch (Exception exception)
    ///             {
    ///                 Console.WriteLine(exception);
    ///             }
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public static class CsvExporter<TInstance> where TInstance : class
    {
        #region Public methods

        /// <summary>
        /// This method tries to save given values into given file.
        /// </summary>
        /// <remarks>
        /// This method performes saving of data with default settings. Using 
        /// default settings means that the header is written and a possible 
        /// existing file is overwritten. Further, needed header information is 
        /// taken from column attributes or from property names.
        /// </remarks>
        /// <param name="values">
        /// The list of values to be written to the CSV file.
        /// </param>
        /// <param name="filename">
        /// The fully qualified path of the output file.
        /// </param>
        /// <exception cref="ArgumentException">
        /// This exception is thrown in case of given filename is invalid. 
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown in case of given file could not be deleted. 
        /// Another reason could be the case when property parsing fails.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// This exception is thrown in case of confirming and ordering the column 
        /// offsets fails.
        /// </exception>
        public static void Save(IEnumerable<TInstance> values, String filename)
        {
            CsvExporter<TInstance>.Save(values, filename, new CsvSettings());
        }

        /// <summary>
        /// This method tries to save given values into given file.
        /// </summary>
        /// <remarks>
        /// This method performes saving of data using given settings. 
        /// But keep in mind, a possible existing file is overwritten. 
        /// </remarks>
        /// <param name="values">
        /// The list of values to be written to the CSV file.
        /// </param>
        /// <param name="filename">
        /// The fully qualified path of the output file.
        /// </param>
        /// <param name="settings">
        /// The settings to be used to generate the output of the CSV file.
        /// </param>
        /// <exception cref="ArgumentException">
        /// This exception is thrown in case of given filename is invalid. 
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown in case of given file could not be deleted.
        /// Another reason could be the case when property parsing fails.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// This exception is thrown in case of confirming and ordering the column 
        /// offsets fails.
        /// </exception>
        public static void Save(IEnumerable<TInstance> values, String filename, CsvSettings settings)
        {
            CsvExporter<TInstance>.Save(values, filename, settings, true);
        }

        /// <summary>
        /// This method tries to save given values into given file.
        /// </summary>
        /// <remarks>
        /// This method determines the file existence and performs file 
        /// deletion if requested. Thereafter, the file content is handled 
        /// by creating and processing a stream.
        /// </remarks>
        /// <param name="values">
        /// The list of values to be written to the CSV file.
        /// </param>
        /// <param name="filename">
        /// The fully qualified path of the output file.
        /// </param>
        /// <param name="settings">
        /// The settings to be used to generate the output of the CSV file.
        /// </param>
        /// <param name="overwrite">
        /// If true, then a possible existing file is overwritten. Otherwise, 
        /// an exception is thrown if a file with the same name already exists.
        /// </param>
        /// <exception cref="ArgumentException">
        /// This exception is thrown either in case of given values are invalid 
        /// or if given filename is invalid. 
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown if overwrite mode is disabled and given 
        /// file already exists or in case of given file could not be deleted.
        /// Another reason could be the case when property parsing fails.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// This exception is thrown in case of confirming and ordering the column 
        /// offsets fails.
        /// </exception>
        public static void Save(IEnumerable<TInstance> values, String filename, CsvSettings settings, Boolean overwrite)
        {
            if (String.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("The filename should not be empty.", nameof(filename));
            }

            if (!overwrite && File.Exists(filename))
            {
                throw new InvalidOperationException($"File {filename} already exists and cannot be overwritten with disabled overwrite mode.");
            }

            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"Could not delete file {filename}. See inner exception for more details.", exception);
                }
            }

            using (Stream stream = File.Create(filename))
            {
                CsvExporter<TInstance>.Save(values, stream, settings);
            }
        }

        /// <summary>
        /// This method tries to save given values into given stream.
        /// </summary>
        /// <remarks>
        /// This method performes saving of data with default settings. Using 
        /// default settings means that the header is written. Further, needed 
        /// header information is taken from the column attributes or from property 
        /// names. Finally, a special textual treatment is not applied.
        /// </remarks>
        /// <param name="values">
        /// The list of values to be written into given stream.
        /// </param>
        /// <param name="stream">
        /// The stream to write given values into.
        /// </param>
        /// <exception cref="ArgumentException">
        /// This exception is thrown either in case of given values are invalid 
        /// or if given stream does not allow write access.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown if given stream is &lt;null&gt;.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown in case of property parsing fails.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// This exception is thrown in case of confirming and ordering the column 
        /// offsets fails.
        /// </exception>
        public static void Save(IEnumerable<TInstance> values, Stream stream)
        {
            CsvExporter<TInstance>.Save(values, stream, new CsvSettings());
        }

        /// <summary>
        /// This method tries to save given values into given stream.
        /// </summary>
        /// <remarks>
        /// Please keep in mind, a textual treatment is only applicable for string 
        /// data types, not matter what the actual value of the 'textual' property 
        /// of given settings is. Additionally, a textual treatment is never applied 
        /// to the header, in case of it is processed.
        /// </remarks>
        /// <param name="values">
        /// The list of values to be written into given stream.
        /// </param>
        /// <param name="stream">
        /// The stream to write given values into.
        /// </param>
        /// <param name="settings">
        /// The settings to be used to generate the CSV output.
        /// </param>
        /// <exception cref="ArgumentException">
        /// This exception is thrown either in case of given values are invalid 
        /// or if given stream does not allow write access.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown if given stream is &lt;null&gt; or if given 
        /// settings are &lt;null&gt;.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown in case of property parsing fails.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// This exception is thrown in case of confirming and ordering the column 
        /// offsets fails.
        /// </exception>
        public static void Save(IEnumerable<TInstance> values, Stream stream, CsvSettings settings)
        {
            if (values == null || !values.Any())
            {
                throw new ArgumentException("Values to write may contain at least one record.", nameof(values));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), $"The stream to write the data into is invalid.");
            }

            if (!stream.CanWrite)
            {
                throw new ArgumentException("No write access to given stream.", nameof(stream));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "The CSV settings are invalid.");
            }

            TypeDescriptor descriptor = TypeProcessor.LoadDescriptor<TInstance>();

            using (StreamWriter writer = new StreamWriter(stream, settings.Encoding))
            {
                if (settings.Heading)
                {
                    CsvExporter<TInstance>.WriteHead(writer, settings.Separator, false, descriptor.Settings);
                }

                foreach (TInstance value in values)
                {
                    CsvExporter<TInstance>.WriteLine(writer, settings.Separator, settings.Textual, settings.Culture, settings.Mappings, CsvExporter<TInstance>.BuildLine(descriptor.Settings, value));
                }
            }

            stream.Flush();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// This method tries to build a list of objects the represent a single 
        /// CSV line.
        /// </summary>
        /// <remarks>
        /// This method may throw exceptions which must be handled by the caller.
        /// </remarks>
        /// <param name="settings">
        /// A list of settings that describe the expected structure of the CSV 
        /// file.
        /// </param>
        /// <param name="value">
        /// An instance of a class of type <typeparamref name="TInstance"/> that 
        /// contains valid CSV data.
        /// </param>
        /// <returns>
        /// A list of objects that contain the current values of a single CSV line.
        /// </returns>
        private static IEnumerable<Object> BuildLine(IEnumerable<ItemDescriptor> settings, TInstance value)
        {
            List<Object> result = new List<Object>();

            if (value != null)
            {
                foreach (ItemDescriptor setting in settings)
                {
                    result.Add(setting.Origin.GetValue(value));
                }
            }

            return result;
        }

        /// <summary>
        /// This method tries to write the header information into given stream 
        /// according to given parameter set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The method writes either the predefined CSV column header or the name 
        /// of the original property.
        /// </para>
        /// <para>
        /// This method may throw exceptions which must be handled by the caller.
        /// </para>
        /// </remarks>
        /// <param name="writer">
        /// The stream writer to be used to push data.
        /// </param>
        /// <param name="separator">
        /// The delimiter to be used to separate each column.
        /// </param>
        /// <param name="textual">
        /// The flag indicating how strings have to be handled. If true then all 
        /// string are enclosed in double-quotes. If false then only the necessary 
        /// strings are enclosed in double-quotes.
        /// </param>
        /// <param name="settings">
        /// A list of settings that describe the expected structure of the CSV 
        /// file.
        /// </param>
        private static void WriteHead(StreamWriter writer, Char separator, Boolean textual, IEnumerable<ItemDescriptor> settings)
        {
            StringBuilder builder = new StringBuilder(1024);

            foreach (ItemDescriptor setting in settings)
            {
                if (setting.Column.IsHeader)
                {
                    builder.Append(CsvExporter<TInstance>.ConvertToOutput(setting.Column.Header, separator, textual));
                }
                else
                {
                    builder.Append(CsvExporter<TInstance>.ConvertToOutput(setting.Origin.Name, separator, textual));
                }
            }

            writer.WriteLine(CsvExporter<TInstance>.FixupOutput(builder, separator).ToString());
        }

        /// <summary>
        /// This method tries to write a particular line into given stream 
        /// according to given parameter set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may throw exceptions which must be handled by the caller.
        /// </para>
        /// </remarks>
        /// <param name="writer">
        /// The stream writer to be used to push data.
        /// </param>
        /// <param name="separator"></param>
        /// The delimiter to be used to separate each column.
        /// <param name="textual"></param>
        /// The flag indicating how strings have to be handled. If true then all 
        /// string are enclosed in double-quotes. If false then only the necessary 
        /// strings are enclosed in double-quotes.
        /// <param name="culture">
        /// The culture to be used for data conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation.
        /// </param>
        /// <param name="values">
        /// A list of values representing a single line of the CSV file.
        /// </param>
        private static void WriteLine(StreamWriter writer, Char separator, Boolean textual, CultureInfo culture, CsvMappings mapping, IEnumerable<Object> values)
        {
            StringBuilder builder = new StringBuilder(1024);

            if (values != null && values.Any())
            {
                foreach (Object value in values)
                {
                    String current = CsvExporter<TInstance>.ConvertToString(value, culture, mapping, out Boolean quoting);
                    builder.Append(CsvExporter<TInstance>.ConvertToOutput(current, separator, (quoting & textual)));
                }
            }

            writer.WriteLine(CsvExporter<TInstance>.FixupOutput(builder, separator).ToString());
        }

        /// <summary>
        /// This method tries to convert an object into its string representation.
        /// </summary>
        /// <remarks>
        /// The <paramref name="quoting"/> out parameter is only true if given 
        /// value is of type string. Otherwise, this parameter is false.
        /// </remarks>
        /// <param name="value">
        /// The object to get its string representation for.
        /// </param>
        /// <param name="culture">
        /// The culture to be used for data conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation.
        /// </param>
        /// <param name="quoting">
        /// The out parameter that describes if an object value should be surrounded 
        /// by double-quotes. 
        /// </param>
        /// <returns>
        /// The string representation of given object.
        /// </returns>
        private static String ConvertToString(Object value, CultureInfo culture, CsvMappings mapping, out Boolean quoting)
        {
            quoting = (value is String);
            return TypeConverter.IntoString(value, culture, mapping);
        }

        /// <summary>
        /// This method converts given value into its output representation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Keep in mind, the result string is enclosed in double-quotes either if 
        /// <paramref name="quoting"/> is true or the given <paramref name="value"/> 
        /// contains at least one of the control character.
        /// </para>
        /// <para>
        /// Control character means in this context that the <paramref name="value"/> 
        /// contains either a carriage return, or a line feed, or a double-quote, 
        ///  or most important the separator character itself.
        /// </para>
        /// <para>
        /// Additionally note that not only string could be enclosed by double-quotes.
        /// Because it could be possible as well that number types will be surrounded 
        /// by double-quotes. This would be for example the case when a comma separator 
        /// is used and the given culture is German.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// The <paramref name="value"/> string to be transformed.
        /// </param>
        /// <param name="separator">
        /// The separator to be applied at the end.
        /// </param>
        /// <param name="quoting">
        /// The flag that indicates whether double-quotes have to be applied.
        /// </param>
        /// <returns>
        /// A string for the value ready to put it into the CSV output.
        /// </returns>
        private static String ConvertToOutput(String value, Char separator, Boolean quoting)
        {
            const Char CR = '\r';
            const Char LF = '\n';
            const Char DQ = '"';

            if (value == null)
            {
                value = String.Empty;
            }

            if (quoting || value.IndexOfAny(new Char[] { separator, CR, LF, DQ }) >= 0)
            {
                return $"{DQ}{value}{DQ}{separator}";
            }
            else
            {
                return $"{value}{separator}";
            }
        }

        /// <summary>
        /// This method simply removes the separator from the end of current 
        /// <paramref name="builder"/> content, but only if exists.
        /// </summary>
        /// <remarks>
        /// As mentioned above, this method tries to remove the column separator 
        /// from its current end.
        /// </remarks>
        /// <param name="builder">
        /// The builder with content to be fixed.
        /// </param>
        /// <param name="separator">
        /// The separator to be removed, if any.
        /// </param>
        /// <returns>
        /// The cleaned out builder.
        /// </returns>
        private static StringBuilder FixupOutput(StringBuilder builder, Char separator)
        {
            if (builder != null && builder.Length > 0 && builder[builder.Length - 1] == separator)
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return builder;
        }

        #endregion
    }
}
