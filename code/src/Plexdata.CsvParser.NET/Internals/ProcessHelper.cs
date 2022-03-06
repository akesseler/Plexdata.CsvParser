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

using Plexdata.CsvParser.Processors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Plexdata.CsvParser.Internals
{
    /// <summary>
    /// This internal helper class provides functionality to process any CSV-based 
    /// content.
    /// </summary>
    /// <remarks>
    /// Mainly this class provides the methods required to handle string operations.
    /// </remarks>
    internal static class ProcessHelper
    {
        #region Private fields

        /// <summary>
        /// This constant field represents the start and end character of strings. 
        /// </summary>
        /// <remarks>
        /// Everything that is enclosed by this character will be treated as it is.
        /// </remarks>
        private const Char StringDelimiter = '"';

        /// <summary>
        /// This constant field represents the character to escape other characters. 
        /// </summary>
        /// <remarks>
        /// Every character will be ignored that follows this character.
        /// </remarks>
        private const Char EscapeDelimiter = '\\';

        #endregion

        #region Public methods

        /// <summary>
        /// This method splits given <paramref name="line"/> into its parts using 
        /// given <paramref name="separator"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Well, standard string split would actually not work for this purpose. 
        /// For example, one of the exceptions is that strings are split at the 
        /// separator, no matter if the separator is inside or outside a string 
        /// that is surrounded by double-quotes. This means that something like 
        /// <i>"Head,er1",Header2,Header3</i> would be split into <i>["Head] [er1"] 
        /// [Header2] [Header3]</i>.
        /// </para>
        /// <para>
        /// This method puts respect on that by skipping everything which is enclosed 
        /// double-quotes. Taking the example from above would result in <i>[Head,er1] 
        /// [Header2] [Header3]</i> which seems to be the wanted result. Further, 
        /// leaving out some of the double-quotes would also work in certain scale. 
        /// But be always aware, a constellation like <i>"Head,er1","Header2,Header3"</i> 
        /// would end up in <i>[Head,er1] [Header2,Header3]</i>!
        /// </para>
        /// </remarks>
        /// <param name="line">
        /// The line to be split.
        /// </param>
        /// <param name="separator">
        /// The separator at which to split.
        /// </param>
        /// <returns>
        /// A list of strings representing all parts of given line.
        /// </returns>
        public static List<String> SplitIntoCells(String line, Char separator)
        {
            List<String> result = new List<String>();

            if (!String.IsNullOrWhiteSpace(line))
            {
                Int32 offset = 0;
                Int32 length = 0;
                Char[] buffer = line.ToArray();

                for (Int32 index = 0; index < buffer.Length; index++)
                {
                    if (buffer[index] == ProcessHelper.StringDelimiter)
                    {
                        do
                        {
                            index = ProcessHelper.MoveIndex(index, ref buffer);
                        }
                        while (index < buffer.Length && buffer[index] != ProcessHelper.StringDelimiter);

                        if (index >= buffer.Length)
                        {
                            index = buffer.Length - 1;
                        }
                    }

                    if (buffer[index] == separator)
                    {
                        length = index - offset;

                        result.Add(line.Substring(offset, length).ToContent());

                        offset = index + 1;
                        length = 0;
                    }
                }

                if (offset < buffer.Length)
                {
                    result.Add(line.Substring(offset).ToContent());
                }
            }

            return result;
        }

        /// <summary>
        /// This method tries to convert an object into its string representation.
        /// </summary>
        /// <remarks>
        /// This method converts provided object into its string representation but 
        /// without taking the surrounding double-quotes into account.
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
        /// <returns>
        /// The string representation of given object.
        /// </returns>
        /// <seealso cref="ConvertToString(Object, CultureInfo, CsvMappings, out Boolean)"/>
        public static String ConvertToString(Object value, CultureInfo culture, CsvMappings mapping)
        {
            return ProcessHelper.ConvertToString(value, culture, mapping, out Boolean quoting);
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
        /// <seealso cref="ConvertToString(Object, CultureInfo, CsvMappings)"/>
        public static String ConvertToString(Object value, CultureInfo culture, CsvMappings mapping, out Boolean quoting)
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
        /// <para>
        /// Finally, any of possibly existing string delimiters are replaced by two 
        /// double-quotes automatically. This in turn represents the escaping of 
        /// double-quotes according to RFC 4180.
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
        public static String ConvertToOutput(String value, Char separator, Boolean quoting)
        {
            if (value == null)
            {
                value = String.Empty;
            }

            if (quoting || value.IndexOfAny(new Char[] { separator, ProcessHelper.StringDelimiter, '\r', '\n' }) >= 0)
            {
                if (value.IndexOf(ProcessHelper.StringDelimiter) >= 0)
                {
                    value = value.Replace($"{ProcessHelper.StringDelimiter}", $"{ProcessHelper.StringDelimiter}{ProcessHelper.StringDelimiter}");
                }

                return $"{ProcessHelper.StringDelimiter}{value}{ProcessHelper.StringDelimiter}{separator}";
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
        public static StringBuilder FixupOutput(StringBuilder builder, Char separator)
        {
            if (builder != null && builder.Length > 0 && builder[builder.Length - 1] == separator)
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return builder;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// This method moves the index to the next possible position.
        /// </summary>
        /// <remarks>
        /// The moving operation will skip any of the escaped string sequences.
        /// </remarks>
        /// <param name="index">
        /// The index to be moved.
        /// </param>
        /// <param name="buffer">
        /// The buffer where the index refers to.
        /// </param>
        /// <returns>
        /// The new index position.
        /// </returns>
        private static Int32 MoveIndex(Int32 index, ref Char[] buffer)
        {
            index++;

            // Skip the next two characters as soon as the escaping of 
            // double-quotes according to the RFC 4180 can be determined.
            if (index < buffer.Length && index + 1 < buffer.Length && buffer[index] == ProcessHelper.StringDelimiter && buffer[index + 1] == ProcessHelper.StringDelimiter)
            {
                index += 2;
            }
            // Skip the next two characters as soon as the standard 
            // escaping of double-quotes can be determined.
            else if (index < buffer.Length && index + 1 < buffer.Length && buffer[index] == ProcessHelper.EscapeDelimiter && buffer[index + 1] == ProcessHelper.StringDelimiter)
            {
                index += 2;
            }

            return index;
        }

        /// <summary>
        /// This method converts provided string value into its real string content 
        /// representation.
        /// </summary>
        /// <remarks>
        /// The conversion is made by removing any leading and trailing whitespaces 
        /// as well as by replacing escaped double-quotes with unescaped double-quotes.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <returns>
        /// The converted and content-ready value.
        /// </returns>
        private static String ToContent(this String value)
        {
            if (value != null)
            {
                value = value.Trim(); // Remove all surrounding white-spaces.

                String padding3 = String.Empty.PadLeft(3, ProcessHelper.StringDelimiter);
                String padding1 = String.Empty.PadLeft(1, ProcessHelper.StringDelimiter);

                // Remove only one of the leading string delimiters, if necessary.
                if (value.StartsWith(padding3) || value.StartsWith(padding1))
                {
                    value = value.Remove(0, 1);
                }

                // Remove only one of the trailing string delimiters, if necessary.
                if (value.EndsWith(padding3) || value.EndsWith(padding1))
                {
                    value = value.Remove(value.Length - 1, 1);
                }

                String replace1 = $"{ProcessHelper.EscapeDelimiter}{ProcessHelper.StringDelimiter}"; // ...\\\"... => ...\"...
                String replace2 = $"{ProcessHelper.StringDelimiter}{ProcessHelper.StringDelimiter}"; // ...\"\"... => ...\"...
                String replace3 = $"{ProcessHelper.StringDelimiter}";

                // Replace all possible escaped string sequences by one single string delimiter.
                value = value.Replace(replace1, replace3).Replace(replace2, replace3);
            }

            return value;
        }

        #endregion
    }
}
