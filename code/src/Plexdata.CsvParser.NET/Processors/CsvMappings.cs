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
using System.Collections.Generic;
using System.Text;

namespace Plexdata.CsvParser.Processors
{
    /// <summary>
    /// This class provides the possibility to map values within a CSV file into standard values.
    /// </summary>
    /// <remarks>
    /// Users may have the need to map values that could occur in a CSV file into real values. 
    /// A good example for such a mapping is da Boolean value. For example, a Boolean value in 
    /// a CSV file might be "Yes" or even "No". However, a Boolean only knows "True" or "False". 
    /// For such a case, it would be reasonable to have a tool that can map for example the "Yes" 
    /// value into "True". This mapping functionality is task of this class.
    /// </remarks>
    public class CsvMappings
    {
        #region Public fields

        /// <summary>
        /// Gets the default 'True' descriptor which is 'true' at the moment.
        /// </summary>
        /// <remarks>
        /// This value is used only for exporting CSV files.
        /// </remarks>
        public static readonly String DefaultTrueValue = Boolean.TrueString.ToLower();

        /// <summary>
        /// Gets the default 'False' descriptor which is 'false' at the moment.
        /// </summary>
        /// <remarks>
        /// This value is used only for exporting CSV files.
        /// </remarks>
        public static readonly String DefaultFalseValue = Boolean.FalseString.ToLower();

        /// <summary>
        /// Gets the default 'Null' descriptor which is 'empty' at the moment.
        /// </summary>
        /// <remarks>
        /// This value is used only for exporting CSV files.
        /// </remarks>
        public static readonly String DefaultNullValue = String.Empty;

        /// <summary>
        /// Gets the default list of 'True' descriptors which contain 'true', '1', 'y' and 'yes' at the moment.
        /// </summary>
        /// <remarks>
        /// This value list is used only for importing CSV files.
        /// </remarks>
        public static readonly List<String> DefaultTrueValues = new List<String>() { CsvMappings.DefaultTrueValue, "1", "y", "yes" };

        /// <summary>
        /// Gets the default list of 'False' descriptors which contain 'false', '0', 'n' and 'no' at the moment.
        /// </summary>
        /// <remarks>
        /// This value list is used only for importing CSV files.
        /// </remarks>
        public static readonly List<String> DefaultFalseValues = new List<String>() { CsvMappings.DefaultFalseValue, "0", "n", "no" };

        /// <summary>
        /// Gets the default list of 'Null' descriptors which just contains '&lt;null&gt;' at the moment.
        /// </summary>
        /// <remarks>
        /// This value list is used only for importing CSV files.
        /// </remarks>
        public static readonly List<String> DefaultNullValues = new List<String>() { "<null>" };

        /// <summary>
        /// Gets the default mapping instance.
        /// </summary>
        /// <remarks>
        /// The default mappings instance consists of all of the pre-defined default values.
        /// </remarks>
        /// <seealso cref="CsvMappings.DefaultTrueValue"/>
        /// <seealso cref="CsvMappings.DefaultFalseValue"/>
        /// <seealso cref="CsvMappings.DefaultNullValue"/>
        /// <seealso cref="CsvMappings.DefaultTrueValues"/>
        /// <seealso cref="CsvMappings.DefaultFalseValues"/>
        /// <seealso cref="CsvMappings.DefaultNullValues"/>
        public static readonly CsvMappings DefaultMappings = new CsvMappings()
        {
            TrueValue = CsvMappings.DefaultTrueValue,
            FalseValue = CsvMappings.DefaultFalseValue,
            NullValue = CsvMappings.DefaultNullValue,
            TrueValues = CsvMappings.DefaultTrueValues,
            FalseValues = CsvMappings.DefaultFalseValues,
            NullValues = CsvMappings.DefaultNullValues,
        };

        #endregion

        #region Construction

        /// <summary>
        /// The static constructor.
        /// </summary>
        /// <remarks>
        /// The static constructor does actually nothing.
        /// </remarks>
        static CsvMappings() { }

        /// <summary>
        /// Default class construction.
        /// </summary>
        /// <remarks>
        /// The default constructor does nothing but its basic initialisation.
        /// </remarks>
        public CsvMappings()
            : base()
        {
            this.TrueValue = CsvMappings.DefaultTrueValue;
            this.FalseValue = CsvMappings.DefaultFalseValue;
            this.NullValue = CsvMappings.DefaultNullValue;
            this.TrueValues = CsvMappings.DefaultTrueValues;
            this.FalseValues = CsvMappings.DefaultFalseValues;
            this.NullValues = CsvMappings.DefaultNullValues;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the descriptor representing a 'True' value which 
        /// should occur in an exported CSV file instead of the standard 
        /// Boolean 'True' value.
        /// </summary>
        /// <remarks>
        /// This value is used only for exporting CSV files.
        /// </remarks>
        /// <value>
        /// The 'True' value descriptor to be used.
        /// </value>
        public String TrueValue { get; set; }

        /// <summary>
        /// Gets or sets the descriptor representing a 'False' value which 
        /// should occur in an exported CSV file instead of the standard 
        /// Boolean 'False' value.
        /// </summary>
        /// <remarks>
        /// This value is used only for exporting CSV files.
        /// </remarks>
        /// <value>
        /// The 'False' value descriptor to be used.
        /// </value>
        public String FalseValue { get; set; }

        /// <summary>
        /// Gets or sets the descriptor representing a 'Null' value which 
        /// should occur in an exported CSV file instead of an empty string.
        /// </summary>
        /// <remarks>
        /// This value is used only for exporting CSV files.
        /// </remarks>
        /// <value>
        /// The 'Null' value descriptor to be used.
        /// </value>
        public String NullValue { get; set; }

        /// <summary>
        /// Gets or sets the list of descriptors containing all possible values 
        /// representing variations of 'True' which may occur in CSV files.
        /// </summary>
        /// <remarks>
        /// This value list is used only for importing CSV files.
        /// </remarks>
        /// <value>
        /// The list of 'True' value descriptors to be used.
        /// </value>
        public List<String> TrueValues { get; set; }

        /// <summary>
        /// Gets or sets the list of descriptors containing all possible values 
        /// representing variations of 'False' which may occur in CSV files.
        /// </summary>
        /// <remarks>
        /// This value list is used only for importing CSV files.
        /// </remarks>
        /// <value>
        /// The list of 'False' value descriptors to be used.
        /// </value>
        public List<String> FalseValues { get; set; }

        /// <summary>
        /// Gets or sets the list of descriptors containing all possible values 
        /// representing variations of 'Null' which may occur in CSV files.
        /// </summary>
        /// <remarks>
        /// This value list is used only for importing CSV files.
        /// </remarks>
        /// <value>
        /// The list of 'Null' value descriptors to be used.
        /// </value>
        public List<String> NullValues { get; set; }

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
            StringBuilder builder = new StringBuilder(512);

            builder.Append("TrueValue: ");
            builder.Append($"\"{this.TrueValue ?? CsvMappings.DefaultNullValue}\", ");
            builder.Append("FalseValue: ");
            builder.Append($"\"{this.FalseValue ?? CsvMappings.DefaultNullValue}\", ");
            builder.Append("NullValue: ");
            builder.Append($"\"{this.NullValue ?? CsvMappings.DefaultNullValue}\", ");

            builder.Append("TrueValues: [");

            if (this.TrueValues != null)
            {
                foreach (String current in this.TrueValues)
                {
                    builder.Append($"\"{current}\", ");
                }

                if (builder.Length >= 2 && this.TrueValues.Count > 0)
                {
                    builder.Remove(builder.Length - 2, 2);
                }
            }

            builder.Append("], FalseValues: [");

            if (this.FalseValues != null)
            {
                foreach (String current in this.FalseValues)
                {
                    builder.Append($"\"{current}\", ");
                }

                if (builder.Length >= 2 && this.FalseValues.Count > 0)
                {
                    builder.Remove(builder.Length - 2, 2);
                }
            }

            builder.Append("], NullValues: [");

            if (this.NullValues != null)
            {
                foreach (String current in this.NullValues)
                {
                    builder.Append($"\"{current}\", ");
                }

                if (builder.Length >= 2 && this.NullValues.Count > 0)
                {
                    builder.Remove(builder.Length - 2, 2);
                }
            }

            builder.Append("]");

            return builder.ToString();
        }

        #endregion
    }
}
