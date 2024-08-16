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

using Plexdata.CsvParser.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Plexdata.CsvParser.Processors
{
    /// <summary>
    /// This static class allows reading all values either from a file or simply from 
    /// a stream (which actually can also be a file). The complete result is put into 
    /// the data type <see cref="CsvContainer"/>, which serves as the container for 
    /// the CSV content.
    /// </summary>
    /// <remarks>
    /// <para>
    /// CSV actually means Comma Separated Values. Sometimes it is also called as Character 
    /// Separated Values. But no matter which name is used, CSV always represents a text file 
    /// mainly used for data exchange between different system.
    /// </para>
    /// <para>
    /// It would be possible (using a proper configuration) to read a CSV input according to 
    /// the rules of RFC 4180. For more information about RFC 4180 please visit the web-site under 
    /// <see href="https://www.ietf.org/rfc/rfc4180.txt">https://www.ietf.org/rfc/rfc4180.txt</see>
    /// </para>
    /// <para>
    /// Actually, this class allows to read all CSV data as some kind of plain text. Such functionally 
    /// seems to be necessary because sometimes CSV files may contain more or less data items as 
    /// expected. Therefore, this class should be helpful to process CSV files with dynamic content.
    /// </para>
    /// </remarks>
    /// <example>
    /// This section wants to show a simple but hopefully useful example of how to use the CSV 
    /// Parser to read data.
    /// <code>
    /// using Plexdata.CsvParser.Constants;
    /// using Plexdata.CsvParser.Processors;
    /// using System;
    /// 
    /// namespace MyCsvReader
    /// {
    ///     class Program
    ///     {
    ///         static void Main(String[] args)
    ///         {
    ///             try
    ///             {
    ///                 String filename = @"C:\folder\file.csv";
    /// 
    ///                 // Source file could contain this content:
    ///                 // Name;               Notes
    ///                 // "Marley, Bob";      "Jamaican singer-songwriter"
    ///                 // "Monroe, Marilyn";  "American actress";          "model and singer"
    ///                 // "Snipes, Wesley";   "American actor";            "director, film producer"; "martial artist"
    ///                 // "Hurley, Elizabeth" 
    /// 
    ///                 CsvSettings settings = new CsvSettings() { Heading = true, Separator = ColumnSeparators.SemicolonSeparator };
    ///                 CsvContainer container = CsvReader.Read(filename, settings);
    /// 
    ///                 String col0row1 = container.GetValue&lt;String&gt;(0, 1) as String; // Marley, Bob
    ///                 String col0row2 = container.GetValue&lt;String&gt;(0, 2) as String; // Monroe, Marilyn
    ///                 String col0row3 = container.GetValue&lt;String&gt;(0, 3) as String; // Snipes, Wesley
    ///                 String col0row4 = container.GetValue&lt;String&gt;(0, 4) as String; // Hurley, Elizabeth
    /// 
    ///                 String col1row1 = container.GetValue&lt;String&gt;(1, 1) as String; // Jamaican singer-songwriter
    ///                 String col1row2 = container.GetValue&lt;String&gt;(1, 2) as String; // American actress
    ///                 String col1row3 = container.GetValue&lt;String&gt;(1, 3) as String; // American actor
    ///                 String col1row4 = container.GetValue&lt;String&gt;(1, 4) as String; // null
    /// 
    ///                 String col2row1 = container.GetValue&lt;String&gt;(2, 1) as String; // null
    ///                 String col2row2 = container.GetValue&lt;String&gt;(2, 2) as String; // model and singer
    ///                 String col2row3 = container.GetValue&lt;String&gt;(2, 3) as String; // director, film producer
    ///                 String col2row4 = container.GetValue&lt;String&gt;(2, 4) as String; // null
    /// 
    ///                 String col3row1 = container.GetValue&lt;String&gt;(3, 1) as String; // null
    ///                 String col3row2 = container.GetValue&lt;String&gt;(3, 2) as String; // null
    ///                 String col3row3 = container.GetValue&lt;String&gt;(3, 3) as String; // martial artist
    ///                 String col3row4 = container.GetValue&lt;String&gt;(3, 4) as String; // null
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
    public static class CsvReader
    {
        #region Public methods

        /// <summary>
        /// This method tries to read all values from given file.
        /// </summary>
        /// <remarks>
        /// This method performes reading of data with default settings. Using default 
        /// settings means that header processing is enabled by default as well. But 
        /// under circumstances, this may cause trouble. Therefore, it is recommended 
        /// to disable header processing manually.
        /// </remarks>
        /// <param name="filename">
        /// The fully qualified path of the input file.
        /// </param>
        /// <returns>
        /// An instance of class <see cref="CsvContainer"/> that contains all processed 
        /// CSV data items.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// This exception is thrown in case of an invalid file name.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// This exception is thrown in case of a file with given name does not 
        /// exist.
        /// </exception>
        public static CsvContainer Read(String filename)
        {
            return CsvReader.Read(filename, new CsvSettings());
        }

        /// <summary>
        /// This method tries to read all values from given file using given settings.
        /// </summary>
        /// <remarks>
        /// The settings parameter describes how the data within given file should be 
        /// processed. For example, users may define the expected culture, the expected 
        /// file encoding, which separator is used and so on.
        /// </remarks>
        /// <param name="filename">
        /// The fully qualified path of the input file.
        /// </param>
        /// <param name="settings">
        /// The settings to be used to process file data.
        /// </param>
        /// <returns>
        /// An instance of class <see cref="CsvContainer"/> that contains all processed 
        /// CSV data items.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// This exception is thrown in case of an invalid file name.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// This exception is thrown in case of a file with given name does not exist.
        /// </exception>
        public static CsvContainer Read(String filename, CsvSettings settings)
        {
            if (String.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("The filename should not be empty.", nameof(filename));
            }

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"File {filename} could not be found.");
            }

            using (Stream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return CsvReader.Read(stream, settings);
            }
        }

        /// <summary>
        /// This method tries to read all values from given stream.
        /// </summary>
        /// <remarks>
        /// This method performes reading of data with default settings. Using default 
        /// settings means that header processing is enabled by default as well. But 
        /// under circumstances, this may cause trouble. Therefore, it is recommended 
        /// to disable header processing manually.
        /// </remarks>
        /// <param name="stream">
        /// The stream to read data from.
        /// </param>
        /// <returns>
        /// An instance of class <see cref="CsvContainer"/> that contains all processed 
        /// CSV data items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown in case of given stream is invalid.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This exception is thrown in case of given stream does not have read access.
        /// </exception>
        public static CsvContainer Read(Stream stream)
        {
            return CsvReader.Read(stream, new CsvSettings());
        }

        /// <summary>
        /// This method tries to read all values from given stream using given settings.
        /// </summary>
        /// <remarks>
        /// The settings parameter describes how the data within given file should be 
        /// processed. For example, users may define the expected culture, the expected 
        /// file encoding, which separator is used and so on.
        /// </remarks>
        /// <param name="stream">
        /// The stream to read data from.
        /// </param>
        /// <param name="settings">
        /// The settings to be used to process file data.
        /// </param>
        /// <returns>
        /// An instance of class <see cref="CsvContainer"/> that contains all processed 
        /// CSV data items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown in case of given stream is invalid.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This exception is thrown in case of given stream does not have read access.
        /// </exception>
        public static CsvContainer Read(Stream stream, CsvSettings settings)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), $"The stream to read the data from is invalid.");
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("No read access to given stream.", nameof(stream));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "The CSV settings are invalid.");
            }

            List<String> lines = CsvReader.ReadLines(stream, settings.Encoding);

            if (lines.Count > 0)
            {
                Char separator = settings.Separator;

                List<List<String>> content = new List<List<String>>();

                for (Int32 outer = 0; outer < lines.Count; outer++)
                {
                    content.Add(ProcessHelper.SplitIntoCells(lines[outer], separator));
                }

                return new CsvContainer(content, settings);
            }

            return new CsvContainer();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Reads all lines from input <paramref name="stream"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method provides a more RFC-compliant way to read the lines of a CSV file. 
        /// This particularly applies to the processing of strings and the line breaks they 
        /// contain.
        /// </para>
        /// Please not that any kind of line break, such as <c>CR="\r"</c>, <c>LF="\n"</c> or 
        /// <c>CRLF="\r\n"</c>, is replace by the platform-specific line break. This actually 
        /// contradicts the RFC, which requires CRLF as line break.
        /// <para>
        /// </para>
        /// </remarks>
        /// <param name="stream">
        /// The input stream to read all lines from.
        /// </param>
        /// <param name="encoding">
        /// The file encoding to be used.
        /// </param>
        /// <returns>
        /// All lines read from input stream.
        /// </returns>
        private static List<String> ReadLines(Stream stream, Encoding encoding)
        {
            // As per RFC:
            // - String may or may not be enclosed by double quotes ("may not" is unsupported).
            // - Line endings within strings are allowed but must be consist of CRLF.
            // - Each double quote in a string has to be escaped by another double quote.

            const Char DQ = '\"';
            const Char CR = '\r';
            const Char LF = '\n';
            const Char BS = '\\';

            List<String> lines = new List<String>();

            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                StringBuilder line = new StringBuilder(512);

                while (!reader.EndOfStream)
                {
                    Char ch = (Char)reader.Read();

                    // Check for string start.
                    if (ch == DQ)
                    {
                        line.Append(ch);

                        while (!reader.EndOfStream)
                        {
                            ch = (Char)reader.Read();

                            // Feature: Check for \" and replace by "".
                            if (ch == BS && reader.Peek() == DQ)
                            {
                                ch = DQ;
                            }

                            if (ch == DQ)
                            {
                                // Check for escaped "double quote" and continue "in string" if needed.
                                if (reader.Peek() == DQ)
                                {
                                    reader.Read();

                                    line.Append(DQ);
                                    line.Append(DQ);

                                    continue;
                                }

                                // String end reached.
                                line.Append(DQ);
                                break;
                            }

                            // Check for "in string" line break.

                            if (ch == CR)
                            {
                                if (reader.Peek() == LF)
                                {
                                    reader.Read();
                                }

                                // Replace by platform-specific line break.
                                line.Append(Environment.NewLine);

                                continue;
                            }

                            if (ch == LF)
                            {
                                // Replace by platform-specific line break.
                                line.Append(Environment.NewLine);

                                continue;
                            }

                            line.Append(ch);
                        }

                        continue;
                    }

                    // Check for CSV line end.

                    if (ch == CR)
                    {
                        if (reader.Peek() == LF)
                        {
                            reader.Read();
                        }

                        lines.Add(line.ToString());
                        line.Clear();

                        continue;
                    }

                    if (ch == LF)
                    {
                        lines.Add(line.ToString());
                        line.Clear();

                        continue;
                    }

                    line.Append(ch);
                }

                // Handle remaining "last line" data.
                if (line.Length > 0)
                {
                    String temp = line.ToString();

                    if (!String.IsNullOrWhiteSpace(temp))
                    {
                        lines.Add(temp);
                        line.Clear();
                    }
                }
            }

            CsvReader.DumpLines(lines);

            return lines;
        }

        /// <summary>
        /// Prints processed lines.
        /// </summary>
        /// <remarks>
        /// This method prints processed lines, but only in debug mode.
        /// </remarks>
        /// <param name="lines">
        /// Lines to print.
        /// </param>
        [Conditional("DEBUG")]
        private static void DumpLines(IEnumerable<String> lines)
        {
            foreach (String line in lines)
            {
                Debug.WriteLine(line.Replace("\r", "\\r").Replace("\n", "\\n"));
            }
        }

        #endregion
    }
}
