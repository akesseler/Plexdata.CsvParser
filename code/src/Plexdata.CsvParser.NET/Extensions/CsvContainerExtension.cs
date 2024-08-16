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

using Plexdata.CsvParser.Definitions;
using Plexdata.CsvParser.Processors;
using System;
using System.Collections.Generic;
using System.IO;

namespace Plexdata.CsvParser.Extensions
{
    /// <summary>
    /// Exposes methods that extent class <see cref="CsvContainer"/>.
    /// </summary>
    /// <remarks>
    /// This extension class provides methods that can be used together 
    /// with instances of class <see cref="CsvContainer"/>.
    /// </remarks>
    public static class CsvContainerExtension
    {
        #region Public methods

        /// <summary>
        /// Serializes the content of provided <see cref="CsvContainer"/> into a string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This extension method serializes the content of provided <paramref name="container"/> 
        /// into a string.
        /// </para>
        /// <para>
        /// A new settings instance is created which is partially overwritten by some
        /// of the container settings subsequently. All other settings items keep their
        /// original values.
        /// </para>
        /// </remarks>
        /// <param name="container">
        /// The container to be serialized.
        /// </param>
        /// <returns>
        /// A string representing the content of provided <paramref name="container"/>.
        /// The result string might be empty, for example if <paramref name="container"/> 
        /// is <c>null</c> or <c>empty</c>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any other exception that might be thrown by 
        /// <see cref="CsvWriter.Write(CsvContainer, Stream, CsvSettings)"/>.
        /// </exception>
        /// <seealso cref="CsvContainerExtension.Serialize(CsvContainer, CsvSettings)"/>
        /// <seealso cref="CsvWriter.Write(CsvContainer, Stream, CsvSettings)"/>
        public static String Serialize(this CsvContainer container)
        {
            if (container is null)
            {
                return String.Empty;
            }

            CsvSettings settings = new CsvSettings()
            {
                Culture = container.Culture,
                Mappings = container.Mappings,
                Heading = container.Heading,
                Exactly = container.Exactly,
            };

            return container.Serialize(settings);
        }

        /// <summary>
        /// Serializes the content of provided <see cref="CsvContainer"/> into a string 
        /// using provided <paramref name="settings"/>.
        /// </summary>
        /// <remarks>
        /// This extension method serializes the content of provided <paramref name="container"/> 
        /// into a string using provided <paramref name="settings"/>.
        /// </remarks>
        /// <param name="container">
        /// The container to be serialized.
        /// </param>
        /// <param name="settings">
        /// The settings to be used.
        /// </param>
        /// <returns>
        /// A string representing the content of provided <paramref name="container"/>.
        /// The result string might be empty, for example if <paramref name="container"/> 
        /// is <c>null</c> or <c>empty</c> or in case of <paramref name="settings"/> is 
        /// <c>null</c>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any other exception that might be thrown by 
        /// <see cref="CsvWriter.Write(CsvContainer, Stream, CsvSettings)"/>.
        /// </exception>
        /// <seealso cref="CsvContainerExtension.Serialize(CsvContainer)"/>
        /// <seealso cref="CsvWriter.Write(CsvContainer, Stream, CsvSettings)"/>
        public static String Serialize(this CsvContainer container, CsvSettings settings)
        {
            if (container is null)
            {
                return String.Empty;
            }

            if (container.IsEmpty)
            {
                return String.Empty;
            }

            if (settings is null)
            {
                return String.Empty;
            }

            using (MemoryStream stream = new MemoryStream(512))
            {
                CsvWriter.Write(container, stream, settings);

                return settings.Encoding.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Sorts the <paramref name="container"/> by column referenced by provided 
        /// <paramref name="header"/> and in provided sort <paramref name="order"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This extension method sorts the <paramref name="container"/> by column that 
        /// is referenced by provided <paramref name="header"/> and in provided sort 
        /// <paramref name="order"/>.
        /// </para>
        /// <para>
        /// The <see cref="CsvContainer.Heading"/> property of the container should be 
        /// enabled. Because otherwise the CSV header is not excluded from sorting.
        /// </para>
        /// <para>
        /// Any string is compared by using the value of property <see cref="CsvContainer.Compare"/>.
        /// </para>
        /// </remarks>
        /// <param name="container">
        /// The container to be sorted.
        /// </param>
        /// <param name="header">
        /// The column header to be used by sorting.
        /// </param>
        /// <param name="order">
        /// The sort order to be used.
        /// </param>
        /// <seealso cref="CsvContainerExtension.Sort(CsvContainer, Int32, SortOrder)"/>
        /// <seealso cref="CsvContainerExtension.GetColumn(CsvContainer, String)"/>
        /// <seealso cref="CsvContainer.Compare"/>
        /// <seealso cref="CsvContainer.Heading"/>
        public static void Sort(this CsvContainer container, String header, SortOrder order)
        {
            container.Sort(container.GetColumn(header), order);
        }

        /// <summary>
        /// Sorts the <paramref name="container"/> by referenced <paramref name="column"/> 
        /// and in provided sort <paramref name="order"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This extension method sorts the <paramref name="container"/> by referenced 
        /// <paramref name="column"/> and in provided sort <paramref name="order"/>.
        /// </para>
        /// <para>
        /// The <see cref="CsvContainer.Heading"/> property of the container should be 
        /// enabled as long as a CSV header is used. Because otherwise the CSV header is 
        /// not excluded from sorting.
        /// </para>
        /// <para>
        /// Any string is compared by using the value of property <see cref="CsvContainer.Compare"/>.
        /// </para>
        /// </remarks>
        /// <param name="container">
        /// The container to be sorted.
        /// </param>
        /// <param name="column">
        /// The zero-based column index to be used by sorting.
        /// </param>
        /// <param name="order">
        /// The sort order to be used.
        /// </param>
        /// <seealso cref="CsvContainerExtension.Sort(CsvContainer, String, SortOrder)"/>
        /// <seealso cref="CsvContainer.Compare"/>
        /// <seealso cref="CsvContainer.Heading"/>
        public static void Sort(this CsvContainer container, Int32 column, SortOrder order)
        {
            if (container is null)
            {
                return;
            }

            if (column < 0 || column > container.Width - 1)
            {
                return;
            }

            List<List<String>> table = new List<List<String>>();
            Int32 offset = container.Heading ? 1 : 0;
            Int32 length = container.Length;
            Int32 width = container.Width;

            // Transform into sortable version.
            for (Int32 row = offset; row < length; row++)
            {
                List<String> line = new List<String>(width);

                for (Int32 col = 0; col < width; col++)
                {
                    line.Add(container[col, row] ?? String.Empty);
                }

                table.Add(line);
            }

            table.Sort(new ContainerSortComparer(column, order, container.Compare));

            // Transform back into internal version.
            for (Int32 row = 0; row < table.Count; row++)
            {
                for (Int32 col = 0; col < table[row].Count; col++)
                {
                    container[col, row + offset] = table[row][col];
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the zero-based index of a column that is referenced by provided header.
        /// </summary>
        /// <remarks>
        /// This extension gets the zero-based index of a column that is referenced by 
        /// provided header or <c>-1</c> if no column could be found for any reason.
        /// </remarks>
        /// <param name="container">
        /// The container to get the column index from.
        /// </param>
        /// <param name="header">
        /// The header to get the column index for.
        /// </param>
        /// <returns>
        /// The zero-based column index or <c>-1</c>.
        /// </returns>
        private static Int32 GetColumn(this CsvContainer container, String header)
        {
            return container?.GetColumnIndex(header) ?? -1;
        }

        #endregion

        #region Private helpers

        /// <summary>
        /// Helper class that performs the actual string comparison.
        /// </summary>
        /// <remarks>
        /// This helper class performs the actual string comparison according 
        /// to its current settings.
        /// </remarks>
        private class ContainerSortComparer : IComparer<IList<String>>
        {
            #region Private fields

            /// <summary>
            /// The column affected by sorting.
            /// </summary>
            /// <remarks>
            /// This field represents the column that is used for sorting.
            /// </remarks>
            private readonly Int32 column;

            /// <summary>
            /// The column sort order.
            /// </summary>
            /// <remarks>
            /// This field represents the column sort order.
            /// </remarks>
            private readonly SortOrder order;

            /// <summary>
            /// The string comparison method.
            /// </summary>
            /// <remarks>
            /// This field represents the string comparison method to be used.
            /// </remarks>
            private readonly StringComparison compare;

            #endregion

            #region Construction

            /// <summary>
            /// The class constructor.
            /// </summary>
            /// <remarks>
            /// The constructor just initializes its fields.
            /// </remarks>
            /// <param name="column">
            /// The column affected by sorting.
            /// </param>
            /// <param name="order">
            /// The column sort order.
            /// </param>
            /// <param name="compare">
            /// The string comparison method.
            /// </param>
            public ContainerSortComparer(Int32 column, SortOrder order, StringComparison compare)
                : base()
            {
                this.column = column;
                this.order = order;
                this.compare = compare;
            }

            #endregion 

            #region Public methods

            /// <summary>
            /// Compares two items according to current settings.
            /// </summary>
            /// <remarks>
            /// This method performs the actual compare action.
            /// </remarks>
            /// <param name="x">
            /// The first item to compare.
            /// </param>
            /// <param name="y">
            /// The second item to compare.
            /// </param>
            /// <returns>
            /// A signed integer indicating the relation between first and second item.
            /// </returns>
            /// <seealso cref="IComparer{T}.Compare(T, T)"/>
            public Int32 Compare(IList<String> x, IList<String> y)
            {
                switch (this.order)
                {
                    case SortOrder.Ascending:
                        return String.Compare(x[this.column], y[this.column], this.compare);
                    case SortOrder.Descending:
                        return String.Compare(y[this.column], x[this.column], this.compare);
                    case SortOrder.Unsorted:
                    default:
                        return 0;
                }
            }

            #endregion
        }

        #endregion
    }
}
