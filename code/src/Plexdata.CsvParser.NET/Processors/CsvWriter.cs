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

using Plexdata.CsvParser.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Plexdata.CsvParser.Processors
{
    /// <summary>
    /// This static class allows writing a matrix of <see cref="Object"/> instances
    /// either into a file or simply into a stream (which actually can also be a file).
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
    /// <para>
    /// Actually, this class allows to write all CSV data as some kind of plain text. Such functionally 
    /// seems to be necessary because sometimes CSV files may contain more or less data items as a fixed 
    /// number of columns would allow. Therefore, this class should be helpful to process CSV files with 
    /// dynamic content.
    /// </para>
    /// </remarks>
    /// <example>
    /// This section wants to show a simple but hopefully useful example of how to use the CSV 
    /// Parser to write data.
    /// <code>
    /// using Plexdata.CsvParser.Constants;
    /// using Plexdata.CsvParser.Processors;
    /// using System;
    /// using System.Collections.Generic;
    /// namespace MyCsvWriter
    /// {
    ///     class Program
    ///     {
    ///         static void Main(String[] args)
    ///         {
    ///             try
    ///             {
    ///                 String filename = @"C:\folder\file.csv";
    /// 
    ///                 List&lt;List&lt;Object&gt;&gt; content = new List&lt;List&lt;Object&gt;&gt;
    ///                 {
    ///                     new List&lt;Object&gt; { "Name", "Notes" },
    ///                     new List&lt;Object&gt; { "Marley, Bob", "Jamaican singer-songwriter" },
    ///                     new List&lt;Object&gt; { "Monroe, Marilyn", "American actress", "model and singer" },
    ///                     new List&lt;Object&gt; { "Snipes, Wesley", "American actor", "director, film producer", "martial artist" },
    ///                     new List&lt;Object&gt; { "Hurley, Elizabeth" }
    ///                 };
    /// 
    ///                 // Output file would contain this content:
    ///                 // Name;Notes
    ///                 // "Marley, Bob";"Jamaican singer-songwriter"
    ///                 // "Monroe, Marilyn";"American actress";"model and singer"
    ///                 // "Snipes, Wesley";"American actor";"director, film producer";"martial artist"
    ///                 // "Hurley, Elizabeth" 
    /// 
    ///                 CsvSettings settings = new CsvSettings() { Heading = true, Textual = true, Separator = ColumnSeparators.SemicolonSeparator };
    ///                 CsvWriter.Write(content, filename, settings);
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
    public static class CsvWriter
    {
        #region Public methods

        /// <summary>
        /// This method tries to write given values into given file.
        /// </summary>
        /// <remarks>
        /// This method performes writing of data with default settings. Using 
        /// default settings means that a possible existing file is overwritten. 
        /// or from property names.
        /// </remarks>
        /// <param name="content">
        /// The content array to be written to the CSV file.
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
        public static void Write(IEnumerable<IEnumerable<Object>> content, String filename)
        {
            CsvWriter.Write(content, filename, new CsvSettings());
        }

        /// <summary>
        /// This method tries to write given values into given file.
        /// </summary>
        /// <remarks>
        /// This method performes writing of data using given settings. But keep 
        /// in mind, a possible existing file is overwritten. 
        /// </remarks>
        /// <param name="content">
        /// The content array to be written to the CSV file.
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
        public static void Write(IEnumerable<IEnumerable<Object>> content, String filename, CsvSettings settings)
        {
            CsvWriter.Write(content, filename, settings, true);
        }

        /// <summary>
        /// This method tries to write given values into given file.
        /// </summary>
        /// <remarks>
        /// This method determines the file existence and performs file deletion if requested. 
        /// Thereafter, the file content is handled by creating and processing a stream.
        /// </remarks>
        /// <param name="content">
        /// The content array to be written to the CSV file.
        /// </param>
        /// <param name="filename">
        /// The fully qualified path of the output file.
        /// </param>
        /// <param name="settings">
        /// The settings to be used to generate the output of the CSV file.
        /// </param>
        /// <param name="overwrite">
        /// If true, then a possible existing file is overwritten. Otherwise, an exception is 
        /// thrown if a file with the same name already exists.
        /// </param>
        /// <exception cref="ArgumentException">
        /// This exception is thrown either in case of given values are invalid or if given 
        /// filename is invalid. 
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown if overwrite mode is disabled and given file already exists 
        /// or in case of given file could not be deleted. Another reason could be the case when 
        /// property parsing fails.
        /// </exception>
        public static void Write(IEnumerable<IEnumerable<Object>> content, String filename, CsvSettings settings, Boolean overwrite)
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
                CsvWriter.Write(content, stream, settings);
            }
        }

        /// <summary>
        /// This method tries to write given values into given stream.
        /// </summary>
        /// <remarks>
        /// This method performes writing of data with default settings. Using default 
        /// settings means for example that special textual treatment is not applied.
        /// </remarks>
        /// <param name="content">
        /// The content array to be written to the CSV file.
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
        public static void Write(IEnumerable<IEnumerable<Object>> content, Stream stream)
        {
            CsvWriter.Write(content, stream, new CsvSettings());
        }

        /// <summary>
        /// This method tries to write given values into given stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Please keep in mind, a textual treatment is only applicable for string data 
        /// types, not matter what the actual value of the 'textual' property of given 
        /// settings is.
        /// </para>
        /// <para>
        /// Additionally, please keep in mind that Heading treatment should be enabled if 
        /// the first line actually contains a header. Otherwise Heading treatment should 
        /// be disabled.
        /// </para>
        /// </remarks>
        /// <param name="content">
        /// The content array to be written to the CSV file.
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
        public static void Write(IEnumerable<IEnumerable<Object>> content, Stream stream, CsvSettings settings)
        {
            if (content == null || !content.Any())
            {
                throw new ArgumentException("Content to write may not be null.", nameof(content));
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

            using (StreamWriter writer = new StreamWriter(stream, settings.Encoding))
            {
                Boolean handled = false;

                foreach (IEnumerable<Object> line in content)
                {
                    if (settings.Heading && !handled)
                    {
                        CsvWriter.WriteLine(writer, settings.Separator, false, settings.Culture, settings.Mappings, line);
                        handled = true;
                    }
                    else
                    {
                        CsvWriter.WriteLine(writer, settings.Separator, settings.Textual, settings.Culture, settings.Mappings, line);
                    }
                }
            }

            stream.Flush();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// This method tries to write a particular line into given stream according 
        /// to given parameter set.
        /// </summary>
        /// <remarks>
        /// This method may throw exceptions which must be handled by the caller.
        /// </remarks>
        /// <param name="writer">
        /// The stream writer to be used to push data.
        /// </param>
        /// <param name="separator">
        /// The delimiter to be used to separate each column.
        /// </param>
        /// <param name="textual">
        /// The flag indicating how strings have to be handled. If true then all string 
        /// are enclosed in double-quotes. If false then only the necessary strings are 
        /// enclosed in double-quotes.
        /// </param>
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
                    String current = ProcessHelper.ConvertToString(value, culture, mapping);
                    builder.Append(ProcessHelper.ConvertToOutput(current, separator, textual));
                }
            }

            writer.WriteLine(ProcessHelper.FixupOutput(builder, separator).ToString());
        }

        #endregion
    }
}
