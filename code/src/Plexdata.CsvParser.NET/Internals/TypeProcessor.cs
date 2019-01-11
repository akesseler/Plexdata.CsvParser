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

using Plexdata.CsvParser.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plexdata.CsvParser.Internals
{
    /// <summary>
    /// This internal class is responsible to collect and to validate all 
    /// information that are required for CSV processing.
    /// </summary>
    /// <remarks>
    /// This internal class just provides methods to access property attributes 
    /// using .NET Reflection feature.
    /// </remarks>
    internal static class TypeProcessor
    {
        #region Private fields

        /// <summary>
        /// The reflection binding flags to be used. These are: Public, Instance, 
        /// GetProperty and SetProperty.
        /// </summary>
        /// <remarks>
        /// This field defines that only properties are included which are public and 
        /// belong to an instance as well as have a property getter and a property setter.
        /// </remarks>
        private static readonly BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;

        #endregion

        #region Construction

        /// <summary>
        /// The static constructor.
        /// </summary>
        /// <remarks>
        /// The static constructor does actually nothing.
        /// </remarks>
        static TypeProcessor() { }

        #endregion

        #region Internal methods

        /// <summary>
        /// This method tries to parse assigned type <typeparamref name="TInstance"/> 
        /// and returns an appropriate type descriptor in case of success.
        /// </summary>
        /// <remarks>
        /// Task of this method is to filter attribute information from given type by 
        /// using reflection and by applying current filter flags.
        /// </remarks>
        /// <typeparam name="TInstance">
        /// The type to be parsed.
        /// </typeparam>
        /// <returns>
        /// An instance of class <see cref="TypeDescriptor"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// This exception is thrown in case of column ordering rules have been 
        /// violated.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This exception might be thrown in any other case, for example type 
        /// parsing has caused an unexpected and/or an unhandled issue. In such 
        /// a case the inner exception contains more details.
        /// </exception>
        internal static TypeDescriptor LoadDescriptor<TInstance>() where TInstance : class
        {
            try
            {
                CsvDocumentAttribute document = TypeProcessor.GetDocumentAttribute(typeof(TInstance)) ?? new CsvDocumentAttribute();
                List<ItemDescriptor> settings = new List<ItemDescriptor>();

                PropertyInfo[] properties = typeof(TInstance).GetProperties(TypeProcessor.flags);

                if (properties != null)
                {
                    foreach (PropertyInfo property in properties)
                    {
                        IEnumerable<Attribute> attributes = property.GetCustomAttributes();

                        if (attributes != null)
                        {
                            foreach (Attribute current in attributes)
                            {
                                if (current is CsvColumnAttribute)
                                {
                                    settings.Add(new ItemDescriptor(current as CsvColumnAttribute, property));
                                    break;
                                }
                            }
                        }
                    }
                }

                return new TypeDescriptor(document, TypeProcessor.ConfirmAndReorganize(settings));
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Unable to load CSV descriptor for type {typeof(TInstance).Name}. See inner exception for more details.", exception);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// This method tries to get CSV document attribute from given type.
        /// </summary>
        /// <remarks>
        /// The CSV Document class attribute is actually a custom attribute.
        /// </remarks>
        /// <param name="type">
        /// The type to get the CSV document attribute from.
        /// </param>
        /// <returns>
        /// An instance of class <see cref="CsvDocumentAttribute"/>, or 
        /// &lt;null&gt; in any case of an error.
        /// </returns>
        private static CsvDocumentAttribute GetDocumentAttribute(Type type)
        {
            try { return type.GetCustomAttributes(typeof(CsvDocumentAttribute), true).FirstOrDefault() as CsvDocumentAttribute; } catch { }
            return null;
        }

        /// <summary>
        /// This method tries to confirm the validity of each item within given 
        /// value list. Additionally, the given value list will be reorganized 
        /// (respectively sorted) according to the applied rules.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Find below a list of rules to be fulfilled to get through the 
        /// confirmation procedure:
        /// </para>
        /// <list type="bullet"> 
        /// <item><description>
        /// The usage of duplicate CSV column offsets is not allowed.
        /// </description></item>  
        /// <item><description>
        /// Each CSV column offset must step linearially. This in turn means 
        /// that offsets like 0, 5, 23, 42 are not supported
        /// </description></item>  
        /// <item><description>
        /// A mixture of CSV column offsets (like -1, -1, 0, 1, 2, -1) is not 
        /// supported.
        /// </description></item>  
        /// </list>
        /// <para>
        /// Finally keep in mind, the expected column order is defined by the 
        /// property order in case of all CSV column offsets are negative.
        /// </para>
        /// </remarks>
        /// <param name="values">
        /// A list of item descriptor values.
        /// </param>
        /// <returns>
        /// The reorganized list of item descriptor values.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// This exception is throw in any case as soon as one confirmation rule 
        /// has been violated.
        /// </exception>
        private static List<ItemDescriptor> ConfirmAndReorganize(List<ItemDescriptor> values)
        {
            if (values == null || values.Count == 0)
            {
                return values;
            }

            Int32 ignored = values.Count(x => x.Column.Offset < 0);
            Int32 applied = values.Count(x => x.Column.Offset >= 0);

            if (ignored == values.Count && applied == 0)
            {
                return values;
            }
            else if (ignored == 0 && applied == values.Count)
            {
                if (values.GroupBy(x => x.Column.Offset).SelectMany(y => y.Skip(1)).Count() != 0)
                {
                    throw new NotSupportedException("Usage of duplicate offsets is not supported.");
                }

                values.Sort((x, y) => x.Column.Offset.CompareTo(y.Column.Offset));

                Int32 offset = values[0].Column.Offset;

                for (Int32 index = 1; index < values.Count; index++)
                {
                    if (values[index].Column.Offset != offset + 1)
                    {
                        throw new NotSupportedException("Offset stepping greater than one is not supported.");
                    }
                    else
                    {
                        offset = values[index].Column.Offset;
                    }
                }

                return values;
            }
            else
            {
                throw new NotSupportedException("Mixing of offsets is not supported.");
            }
        }

        #endregion
    }
}
