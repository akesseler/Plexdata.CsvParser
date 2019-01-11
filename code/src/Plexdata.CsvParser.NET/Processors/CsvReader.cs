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
using System.IO;

namespace Plexdata.CsvParser.Processors
{
    /// <summary>
    /// This static class allows reading all values either from a file or simply from 
    /// a stream (which actually can also be a file). The complete result is put into 
    /// the data type <see cref="CsvContainer"/>, which serves as the container for 
    /// the CSV content.
    /// </summary>
    /// <remarks>
    /// Actually, this class allows to read all CSV data as some kind of plain text. 
    /// Such functionally seems to be necessary because sometimes CSV files may contain 
    /// more or less data items as expected. Therefore, this class should be helpful 
    /// to process CSV files with dynamic content.
    /// </remarks>
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

            CsvContainer result = new CsvContainer();
            List<String> lines = new List<String>();

            using (StreamReader reader = new StreamReader(stream, settings.Encoding))
            {
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line);
                    }
                }
            }

            if (lines.Count > 0)
            {
                Char separator = settings.Separator;

                List<List<String>> content = new List<List<String>>();

                for (Int32 outer = 0; outer < lines.Count; outer++)
                {
                    content.Add(ProcessHelper.SplitIntoCells(lines[outer], separator));
                }

                result = new CsvContainer(content)
                {
                    Mappings = settings.Mappings,
                    Culture = settings.Culture,
                    Heading = settings.Heading,
                    Exactly = settings.Exactly,
                };
            }

            return result;
        }

        #endregion
    }
}
