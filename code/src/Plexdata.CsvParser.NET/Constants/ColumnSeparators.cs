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

using System;

namespace Plexdata.CsvParser.Constants
{
    /// <summary>
    /// This class provides access to all supported standard separators.
    /// </summary>
    /// <remarks>
    /// The class provides access to a subset of most used CSV column separators.
    /// </remarks>
    public static class ColumnSeparators
    {
        #region Public methods

        /// <summary>
        /// Columns are separated by a colon. This separator is not conform to 
        /// the RFC 4180.
        /// </summary>
        /// <remarks>
        /// A line in a CSV file could look like as follows:
        /// <code>
        /// true:"17:23:42":"That's the time."
        /// </code>
        /// </remarks>
        public const Char ColonSeparator = ':';

        /// <summary>
        /// Columns are separated by a comma. This is the separator according to 
        /// the RFC 4180.
        /// </summary>
        /// <remarks>
        /// A line in a CSV file could look like as follows:
        /// <code>
        /// true,17:23:42,That's the time.
        /// </code>
        /// </remarks>
        public const Char CommaSeparator = ',';

        /// <summary>
        /// Columns are separated by a semicolon. This separator is not conform 
        /// to the RFC 4180.
        /// </summary>
        /// <remarks>
        /// A line in a CSV file could look like as follows:
        /// <code>
        /// true;17:23:42;"That's the time."
        /// </code>
        /// </remarks>
        public const Char SemicolonSeparator = ';';

        /// <summary>
        /// Columns are separated by tabulator. This separator is not conform 
        /// to the RFC 4180 but could make an Excel import much easier.
        /// </summary>
        /// <remarks>
        /// A line in a CSV file could look like as follows:
        /// <code>
        /// true    17:23:42    "That's the time."
        /// </code>
        /// </remarks>
        public const Char TabulatorSeparator = '\t';

        /// <summary>
        /// The default separator for column processing. The default separator 
        /// is set to comma.
        /// </summary>
        /// <remarks>
        /// Users may use the default separator whenever an RFC 4180 conform CSV file is expected.
        /// </remarks>
        public const Char DefaultSeparator = ColumnSeparators.CommaSeparator;

        #endregion
    }
}

