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
using Plexdata.CsvParser.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Plexdata.CsvParser.Processors
{
    /// <summary>
    /// This static class allows reading a value list of <typeparamref name="TInstance"/> 
    /// types either from a file or simply from a stream (which actually can also be a file).
    /// </summary>
    /// <remarks>
    /// <para>
    /// CSV actually means Comma Separated Values. Sometimes it is also called as Character 
    /// Separated Values. But not matter which name is used, CSV always represents a text file 
    /// mainly used for data exchange between different system.
    /// </para>
    /// <para>
    /// It would be possible (using a proper configuration) to read a CSV input according to 
    /// the rules of RFC 4180. For more information about RFC 4180 please visit the web-site under 
    /// <see href="https://www.ietf.org/rfc/rfc4180.txt">https://www.ietf.org/rfc/rfc4180.txt</see>
    /// </para>
    /// </remarks>
    /// <typeparam name="TInstance">
    /// The class type that fully describes the structure of the CSV input to be read.
    /// </typeparam>
    /// <example>
    /// This section wants to show a simple but hopefully useful example of how to use the CSV 
    /// Parser for data imports.
    /// <code>
    /// using Plexdata.CsvParser.Attributes;
    /// using Plexdata.CsvParser.Processors;
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Globalization;
    /// 
    /// namespace MyCsvImporter
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
    ///         static void Main(String[] args)
    ///         {
    ///             try
    ///             {
    ///                 String filename = @"C:\folder\file.csv";
    ///         
    ///                 CsvSettings settings = new CsvSettings
    ///                 {
    ///                     Culture = CultureInfo.GetCultureInfo("en-US"),
    ///                     Mappings = new CsvMappings
    ///                     {
    ///                         TrueValues = new List&lt;String&gt; { "yeah" },
    ///                         FalseValues = new List&lt;String&gt; { "nope" },
    ///                     },
    ///                 };
    ///         
    ///                 // Source file could contain this content:
    ///                 // Surname,  Forename,    Identifier, Date,       Sales,      Active, Notes
    ///                 // "Marley", "Bob",       1001,       2007-05-03, "1,234.56", nope,   "Have a short note here."
    ///                 // "Monroe", "Marilyn",   1002,       2008-06-05, "1,234.56", nope,   ""
    ///                 // "Snipes", "Wesley",    1003,       2009-07-06, "1,234.56", yeah,   "Have a short note here." 
    ///                 // "Hurley", "Elizabeth", 1004,       2005-08-08, "1,234.56", yeah,   "Have a short note here."
    ///         
    ///                 IEnumerable&lt;CsvCustomer&gt; result = CsvImporter&lt;CsvCustomer&gt;.Load(filename, settings);
    ///         
    ///                 foreach (CsvCustomer current in result)
    ///                 {
    ///                     Console.WriteLine(current.ToString());
    ///                 }
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
    public static class CsvImporter<TInstance> where TInstance : class
    {
        #region Public methods

        /// <summary>
        /// This method tries to load all values from given file.
        /// </summary>
        /// <remarks>
        /// This method performes loading of data with default settings. Using 
        /// default settings means that the header is processed, but only if one 
        /// exist. Further, the information for header processing is taken from 
        /// column attributes or from property names.
        /// </remarks>
        /// <param name="filename">
        /// The fully qualified path of the input file.
        /// </param>
        /// <returns>
        /// A list of classes of <typeparamref name="TInstance"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// This exception is thrown in case of an invalid file name.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// This exception is thrown in case of a file with given name does not 
        /// exist.
        /// </exception>
        public static IEnumerable<TInstance> Load(String filename)
        {
            return CsvImporter<TInstance>.Load(filename, new CsvSettings());
        }

        /// <summary>
        /// This method tries to load all values from given file using given 
        /// settings.
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
        /// A list of classes of  <typeparamref name="TInstance"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// This exception is thrown in case of an invalid file name.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// This exception is thrown in case of a file with given name does not 
        /// exist.
        /// </exception>
        public static IEnumerable<TInstance> Load(String filename, CsvSettings settings)
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
                return CsvImporter<TInstance>.Load(stream, settings);
            }
        }

        /// <summary>
        /// This method tries to load all values from given stream.
        /// </summary>
        /// <remarks>
        /// This method performes loading of data with default settings. Using 
        /// default settings means that the header is processed, but only if one 
        /// exist. Further, The information for header processing is taken from 
        /// column attributes or from property names.
        /// </remarks>
        /// <param name="stream">
        /// The stream to read data from.
        /// </param>
        /// <returns>
        /// A list of classes of  <typeparamref name="TInstance"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown in case of given stream is invalid.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This exception is thrown in case of given stream does not have read access.
        /// </exception>
        public static IEnumerable<TInstance> Load(Stream stream)
        {
            return CsvImporter<TInstance>.Load(stream, new CsvSettings());
        }

        /// <summary>
        /// This method tries to load all values from given stream using given 
        /// settings.
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
        /// The settings to be used to process stream data.
        /// </param>
        /// <returns>
        /// A list of classes of  <typeparamref name="TInstance"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown either if given stream or if given settings is 
        /// invalid. 
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This exception is thrown in case of given stream does not have read access.
        /// </exception>
        public static IEnumerable<TInstance> Load(Stream stream, CsvSettings settings)
        {
            List<TInstance> values = new List<TInstance>();

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

            TypeDescriptor descriptor = TypeProcessor.LoadDescriptor<TInstance>();

            // NOTE: Keep in mind! All item descriptors are confirmed and ordered already. 
            //       This means they are in the right (which means expected) order! Therefore, 
            //       something like "search for right column" is absolutely redundant.

            List<ItemDescriptor> items = descriptor.Settings.ToList();
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
                Boolean exactly = settings.Exactly;
                CultureInfo culture = settings.Culture;
                Boolean heading = settings.Heading;
                CsvMappings mapping = settings.Mappings;

                CsvImporter<TInstance>.ValidateColumns(lines, separator, exactly, items); // throws

                Int32 index = 0;

                List<String> cells = ProcessHelper.SplitIntoCells(lines[index], separator);

                if (CsvImporter<TInstance>.IsHeaderLine(cells, items))
                {
                    // An exact header validation is performed, but only if the first 
                    // line represents a header and Heading and Exactly are both enabled.
                    // In such a case a Format Exception is thrown as soon as the header 
                    // does not exactly fit! In this conjunction, the validation must pass 
                    // the case-sensitive check as well as the position check.

                    if (heading && exactly)
                    {
                        CsvImporter<TInstance>.ValidateHeader(cells, items); // throws
                    }

                    index++;
                }

                for (; index < lines.Count; index++)
                {
                    values.Add(CsvImporter<TInstance>.ProcessLine(lines[index], separator, exactly, culture, mapping, items));
                }
            }

            return values;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// This method tries to process given line according to given set of parameters.
        /// </summary>
        /// <remarks>
        /// This method processes given line by splitting it into its pieces and then by 
        /// trying to convert each element into its type-safe object expression. 
        /// </remarks>
        /// <param name="line">
        /// The line to be processed.
        /// </param>
        /// <param name="separator">
        /// The separator to be used to split given line at.
        /// </param>
        /// <param name="exactly">
        /// An exception of type 'FormatException' is throw if this parameter is 'true' and 
        /// converting one of the values has failed. Otherwise, each variable is set to a 
        /// default value. Which default value will be used in such a failure case depends 
        /// on current data type. For example, strings using &lt;null&gt; as default value, 
        /// 'false' is used as default for Boolean types, and the corresponding max-value is 
        /// used for all other types.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for data conversion.
        /// </param>
        /// <param name="mapping">
        /// The value mappings to be used for an extended data interpretation.
        /// </param>
        /// <param name="descriptors">
        /// A list of data type descriptor items that help interpreting data.
        /// </param>
        /// <returns>
        /// An instance of class of type <typeparamref name="TInstance"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown for example if <typeparamref name="TInstance"/> does not 
        /// provide a default constructor.
        /// </exception>
        /// <exception cref="FormatException">
        /// This exception is thrown for example if 'exactly' is set to 'true' and one of the 
        /// data items could not be converted.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// This exception is thrown as soon as a class of type <typeparamref name="TInstance"/> 
        /// wants to use an unsupported data type.
        /// </exception>
        private static TInstance ProcessLine(String line, Char separator, Boolean exactly, CultureInfo culture, CsvMappings mapping, List<ItemDescriptor> descriptors)
        {
            TInstance result = CsvImporter<TInstance>.Construct(); // throws

            List<String> values = ProcessHelper.SplitIntoCells(line, separator);

            for (Int32 index = 0; index < values.Count; index++)
            {
                // A simple break is possible because of column validation has been done already.
                if (index >= descriptors.Count) { break; }

                String value = values[index];
                PropertyInfo property = descriptors[index].Origin;
                Type type = property.PropertyType;

                property.SetValue(result, TypeConverter.IntoObject(value, type, exactly, culture, mapping)); // throws
            }

            return result;
        }

        /// <summary>
        /// This method validates the number of columns of each line. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each line contains at least the expected number of columns if this method passes 
        /// through without throwing an exception. How the validation is performed in details 
        /// depends on the state of parameter <paramref name="exactly"/>.
        /// </para>
        /// If <paramref name="exactly"/> is true and one of the line does not exactly contain the 
        /// number of expected columns, then an exception is thrown. If <paramref name="exactly"/> 
        /// is false then each line must contain at least the number of expected columns. Overhanging 
        /// columns are ignored in such a case. 
        /// <para>
        /// </para>
        /// </remarks>
        /// <param name="lines">
        /// The list of lines to be validated.
        /// </param>
        /// <param name="separator">
        /// The separator to split each line into its parts.
        /// </param>
        /// <param name="exactly">
        /// The column count must match exactly, if true. Otherwise, the column 
        /// count must be at least the minimum of expected columns.
        /// </param>
        /// <param name="items">
        /// The list of descriptors representing a single line.
        /// </param>
        /// <exception cref="FormatException">
        /// This exceptions is thrown as soon as one of lines does not fit the 
        /// expected column count according the applied rules.
        /// </exception>
        private static void ValidateColumns(List<String> lines, Char separator, Boolean exactly, List<ItemDescriptor> items)
        {
            for (Int32 line = 0; line < lines.Count; line++)
            {
                List<String> cells = ProcessHelper.SplitIntoCells(lines[line], separator);

                if (cells.Count != items.Count && (exactly || cells.Count < items.Count))
                {
                    throw new FormatException($"Column count mismatch in line {line}. Expected are {items.Count} columns but found were {cells.Count} columns.");
                }
            }
        }

        /// <summary>
        /// This method tries to validate given headers by applying exact 
        /// validation rules. 
        /// </summary>
        /// <remarks>
        /// Exact validation rules means in detail that each string in given headers 
        /// must exactly match its corresponding header name. Furthermore, exactly 
        /// match means that each header name is compared by applying a case-sensitive 
        /// name check. It also means that each header position must be the same 
        /// position which is defined within given descriptors.
        /// </remarks>
        /// <param name="headers">
        /// The list of header names to be validated.
        /// </param>
        /// <param name="descriptors">
        /// The list of descriptors representing a single line.
        /// </param>
        /// <exception cref="FormatException">
        /// This exception is thrown as soon as one of the header validation rules 
        /// has been violated.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception might be thrown if accessing the lists fails.
        /// </exception>
        private static void ValidateHeader(List<String> headers, List<ItemDescriptor> descriptors)
        {
            // Keep in mind, the column validation has already checked the column count. And it is trusted 
            // in that an Argument Out Of Range Exception is thrown if there was a made mistake beforehand.

            for (Int32 index = 0; index < headers.Count; index++)
            {
                String header = headers[index];
                ItemDescriptor descriptor = descriptors[index];

                if (!CsvImporter<TInstance>.IsHeaderName(header, true, descriptor))
                {
                    throw new FormatException(
                        $"Header validation mismatch. Header name \"{header}\" at column {index} does not fit " +
                        $"to the expected header name \"{CsvImporter<TInstance>.GetHeaderName(descriptor)}.\"");
                }
            }
        }

        /// <summary>
        /// This method tries to determine whether given list of cells contain 
        /// all and only header names.
        /// </summary>
        /// <remarks>
        /// The order of headers does not matter in this validation because of 
        /// it only checks whether all the header names are included. Further, 
        /// each name comparison is done by ignoring upper and lower cases.
        /// </remarks>
        /// <param name="headers">
        /// The list of cells to be verified, which should contain header names.
        /// </param>
        /// <param name="descriptors">
        /// The list of descriptors representing a single line.
        /// </param>
        /// <returns>
        /// True is returned when all header names have occurred at least once, 
        /// no matter at which position a header name has occurred. Otherwise, 
        /// false is returned.
        /// </returns>
        private static Boolean IsHeaderLine(List<String> headers, List<ItemDescriptor> descriptors)
        {
            Int32 verified = 0;

            for (Int32 index = 0; index < headers.Count; index++)
            {
                foreach (ItemDescriptor descriptor in descriptors)
                {
                    if (CsvImporter<TInstance>.IsHeaderName(headers[index], false, descriptor))
                    {
                        verified++;
                        break;
                    }
                }
            }

            return verified == headers.Count;
        }

        /// <summary>
        /// The method evaluates whether give name equals to the header name set 
        /// in the descriptor.
        /// </summary>
        /// <remarks>
        /// This internal method detects its result by comparing the determined header 
        /// name either by using lower and upper cases or by ignoring it.
        /// </remarks>
        /// <param name="header">
        /// The name to be compared.
        /// </param>
        /// <param name="exactly">
        /// If true, then an case-sensitive string comparison is performed. 
        /// Otherwise as case-insensitive string comparison is performed.
        /// </param>
        /// <param name="descriptor">
        /// The column descriptor to get header information from.
        /// </param>
        /// <returns>
        /// True, if given name equals the header name within the descriptor, 
        /// and false if not.
        /// </returns>
        private static Boolean IsHeaderName(String header, Boolean exactly, ItemDescriptor descriptor)
        {
            String other = CsvImporter<TInstance>.GetHeaderName(descriptor);
            StringComparison flags = exactly ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return String.Compare(header, other, flags) == 0;
        }

        /// <summary>
        /// This method returns the expected header name of a single column.
        /// </summary>
        /// <remarks>
        /// This method either returns the column header name which is set within 
        /// the custom attribute or the property name if no column header name 
        /// could be determined.
        /// </remarks>
        /// <param name="descriptor">
        /// The column descriptor to get the header name from.
        /// </param>
        /// <returns>
        /// The header name which can be the header name of the column attribute 
        /// or (if invalid) the name of the property.
        /// </returns>
        private static String GetHeaderName(ItemDescriptor descriptor)
        {
            String result = String.Empty;

            if (descriptor.Column.IsHeader)
            {
                result = descriptor.Column.Header;
            }
            else
            {
                result = descriptor.Origin.Name;
            }

            return result;
        }

        /// <summary>
        /// This method constructs an instance of type <typeparamref name="TInstance"/> 
        /// by calling its standard constructors.
        /// </summary>
        /// <remarks>
        /// Class construction takes place by calling the standard constructor. Therefore, 
        /// it is required that any class of type <typeparamref name="TInstance"/> has a 
        /// parameterless constructor.
        /// </remarks>
        /// <returns>
        /// An instance of type <typeparamref name="TInstance"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown in case of an error.
        /// </exception>
        private static TInstance Construct()
        {
            ConstructorInfo ctor = typeof(TInstance).GetConstructor(new Type[] { });

            if (ctor == null)
            {
                throw new InvalidOperationException($"Type \"{typeof(TInstance).Name}\" does not contain a default constructor.");
            }

            TInstance result = null;

            try
            {
                result = ctor.Invoke(new Object[] { }) as TInstance;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Could not use standard constructor of type \"{typeof(TInstance).Name}\".", exception);
            }

            if (result == null)
            {
                throw new InvalidOperationException($"Could not create an instance of type \"{typeof(TInstance).Name}\".");
            }

            return result;
        }

        #endregion
    }
}
