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

using Plexdata.CsvParser.Processors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Plexdata.CsvParser.Internals
{
    /// <summary>
    /// This internal class provides a set of type conversion methods.
    /// </summary>
    /// <remarks>
    /// Type conversion is used only internally. User should not use this 
    /// class directly.
    /// </remarks>
    internal static class TypeConverter
    {
        #region Internal methods

        /// <summary>
        /// The method tries to convert given 'value' into its string 
        /// representation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// At the moment, the supported data types are: String, Boolean, Char, 
        /// SByte, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64, DateTime, 
        /// Decimal, Double and Single. Method ToString() is used for all other 
        /// type conversion. An empty string is returned for values that are 
        /// &lt;null&gt;.
        /// </para>
        /// The value of type DateTime is converted into a string using the 
        /// Sortable Date Time Pattern according to ISO 8601.
        /// <para>
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="culture">
        /// The culture to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation.
        /// </param>
        /// <returns>
        /// A string representing the data of given 'value'.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The exception is thrown in case of given 'culture' is &lt;null&gt;.
        /// </exception>
        internal static String IntoString(Object value, CultureInfo culture, CsvMappings mapping)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture), "The culture cannot be <null>.");
            }

            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping), "The mapping cannot be <null>.");
            }

            if (value == null)
            {
                return TypeConverter.GetNullMapping(mapping);
            }
            else if (value is String)
            {
                return value as String;
            }
            else if (value is Boolean)
            {
                return TypeConverter.ToString((Boolean)value, culture, mapping);
            }
            else if (value is Char)
            {
                return ((Char)value).ToString(culture);
            }
            else if (value is SByte)
            {
                return ((SByte)value).ToString(culture.NumberFormat);
            }
            else if (value is Byte)
            {
                return ((Byte)value).ToString(culture.NumberFormat);
            }
            else if (value is Int16)
            {
                return ((Int16)value).ToString(culture.NumberFormat);
            }
            else if (value is UInt16)
            {
                return ((UInt16)value).ToString(culture.NumberFormat);
            }
            else if (value is Int32)
            {
                return ((Int32)value).ToString(culture.NumberFormat);
            }
            else if (value is UInt32)
            {
                return ((UInt32)value).ToString(culture.NumberFormat);
            }
            else if (value is Int64)
            {
                return ((Int64)value).ToString(culture.NumberFormat);
            }
            else if (value is UInt64)
            {
                return ((UInt64)value).ToString(culture.NumberFormat);
            }
            else if (value is DateTime)
            {
                return ((DateTime)value).ToString("s", culture.DateTimeFormat);
            }
            else if (value is Decimal)
            {
                return ((Decimal)value).ToString(culture.NumberFormat);
            }
            else if (value is Double)
            {
                return ((Double)value).ToString(culture.NumberFormat);
            }
            else if (value is Single)
            {
                return ((Single)value).ToString(culture.NumberFormat);
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// This method converts given value into its type-safe object 
        /// representation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </para>
        /// <para>
        /// At the moment, the supported data types are: String, Boolean, Char, 
        /// SByte, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64, DateTime, 
        /// Decimal, Double and Single. Method ToString() is used for all other 
        /// type conversion.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="type">
        /// The expected result type of the value.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown in cases of 'value', 'type' or 'culture' 
        /// is &lt;null&gt;. This exception is also thrown in case of 'type' is 
        /// considered as Nullable and its embedded couldn't be determined.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// This exception is thrown in all cases of trying to convert unsupported 
        /// data types.
        /// </exception>
        internal static Object IntoObject(String value, Type type, Boolean exactly, CultureInfo culture, CsvMappings mapping)
        {
            Boolean nullable = false;

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "The value cannot be <null>.");
            }

            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture), "The culture cannot be <null>.");
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "The type cannot be <null>.");
            }

            if (TypeConverter.IsNullable(type))
            {
                nullable = true;
                type = type.GetGenericArguments().FirstOrDefault();

                if (type == null || (type.Name == "T" && type.FullName == null))
                {
                    throw new ArgumentNullException(nameof(type), "Could not determine generic argument type.");
                }
            }

            if (type == typeof(String))
            {
                return TypeConverter.AsString(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(Boolean))
            {
                return TypeConverter.AsBoolean(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(Char))
            {
                return TypeConverter.AsCharacter(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(SByte))
            {
                return TypeConverter.AsSInt8(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(Byte))
            {
                return TypeConverter.AsUInt8(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(Int16))
            {
                return TypeConverter.AsSInt16(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(UInt16))
            {
                return TypeConverter.AsUInt16(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(Int32))
            {
                return TypeConverter.AsSInt32(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(UInt32))
            {
                return TypeConverter.AsUInt32(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(Int64))
            {
                return TypeConverter.AsSInt64(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(UInt64))
            {
                return TypeConverter.AsUInt64(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(DateTime))
            {
                return TypeConverter.AsDateTime(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(Decimal))
            {
                return TypeConverter.AsDecimal(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(Double))
            {
                return TypeConverter.AsDouble(value, exactly, nullable, culture, mapping);
            }
            else if (type == typeof(Single))
            {
                return TypeConverter.AsSingle(value, exactly, nullable, culture, mapping);
            }
            else
            {
                throw new NotSupportedException($"Converting the value {value} of type {type} is not supported.");
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// This method converts given Boolean value into its string representation.
        /// </summary>
        /// <remarks>
        /// The conversion into a string takes place by mapping the value into the '
        /// True' or 'False' representation.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// A string representing the given Boolean value.
        /// </returns>
        private static String ToString(Boolean value, CultureInfo culture, CsvMappings mapping)
        {
            if (value)
            {
                return TypeConverter.GetTrueMapping(mapping);
            }
            else
            {
                return TypeConverter.GetFalseMapping(mapping);
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// String type.
        /// </summary>
        /// <remarks>
        /// The conversion into a string takes place by just returning given 
        /// value.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsString(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (value != null && TypeConverter.IsNullString(value, mapping))
            {
                return (String)null;
            }

            return value as String;
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Boolean type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The method returns a value of 'true' as soon es given string 
        /// contains either 'true' or '1'. The 'false' check instead is 
        /// performed by verifying if given value contains either 'false' 
        /// or '0'. In other cases, as for example error cases, false is 
        /// returned or an exception is thrown if 'exactly' is enabled.
        /// </para>
        /// <para>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsBoolean(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (Boolean?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Boolean.");
                }

                return (Boolean)false;
            }

            if (TypeConverter.IsTrueString(value, mapping))
            {
                return (Boolean)true;
            }

            if (TypeConverter.IsFalseString(value, mapping))
            {
                return (Boolean)false;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (Boolean?)null;
            }

            if (exactly)
            {
                throw new FormatException($"Could not convert value \"{value}\" into Boolean.");
            }

            return (Boolean)false;
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Character type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </para>
        /// <para>
        /// Additionally, the given string must have a length of exactly one 
        /// character! Otherwise 'max-char' is returned.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsCharacter(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (Char?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Character.");
                }

                return Char.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (Char?)null;
            }

            try
            {
                return Convert.ToChar(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Character.", exception);
                }

                return Char.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Signed Byte type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsSInt8(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (SByte?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Signed Byte.");
                }

                return SByte.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (SByte?)null;
            }

            try
            {
                return Convert.ToSByte(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Signed Byte.", exception);
                }

                return SByte.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Unsigned Byte type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsUInt8(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (Byte?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Unsigned Byte.");
                }

                return Byte.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (Byte?)null;
            }

            try
            {
                return Convert.ToByte(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Unsigned Byte.", exception);
                }

                return Byte.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Signed Short type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsSInt16(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (Int16?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Signed Short.");
                }

                return Int16.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (Int16?)null;
            }

            try
            {
                return Convert.ToInt16(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Signed Short.", exception);
                }

                return Int16.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Unsigned Short type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsUInt16(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (UInt16?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Unsigned Short.");
                }

                return UInt16.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (UInt16?)null;
            }

            try
            {
                return Convert.ToUInt16(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Unsigned Short.", exception);
                }

                return UInt16.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Signed Integer type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsSInt32(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (Int32?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Signed Integer.");
                }

                return Int32.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (Int32?)null;
            }

            try
            {
                return Convert.ToInt32(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Signed Integer.", exception);
                }

                return Int32.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Unsigned Integer type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsUInt32(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (UInt32?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Unsigned Integer.");
                }

                return UInt32.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (UInt32?)null;
            }

            try
            {
                return Convert.ToUInt32(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Unsigned Integer.", exception);
                }

                return UInt32.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Signed Long type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsSInt64(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (Int64?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Signed Long.");
                }

                return Int64.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (Int64?)null;
            }

            try
            {
                return Convert.ToInt64(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Signed Long.", exception);
                }

                return Int64.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Unsigned Long type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsUInt64(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (UInt64?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Unsigned Long.");
                }

                return UInt64.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (UInt64?)null;
            }

            try
            {
                return Convert.ToUInt64(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Unsigned Long.", exception);
                }

                return UInt64.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Date/Time type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsDateTime(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (DateTime?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Date/Time.");
                }

                return DateTime.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (DateTime?)null;
            }

            try
            {
                return Convert.ToDateTime(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Date/Time.", exception);
                }

                return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Decimal type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsDecimal(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (Decimal?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Decimal.");
                }

                return Decimal.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (Decimal?)null;
            }

            try
            {
                return Convert.ToDecimal(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Decimal.", exception);
                }

                return Decimal.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Double type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsDouble(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (Double?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Double.");
                }

                return Double.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (Double?)null;
            }

            try
            {
                return Convert.ToDouble(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Double.", exception);
                }

                return Double.MaxValue;
            }
        }

        /// <summary>
        /// This method converts given value into an object representing a 
        /// Single type.
        /// </summary>
        /// <remarks>
        /// A value of &lt;null&gt; is returned if given value is 'null' or 
        /// 'empty' and its type is of type Nullable.
        /// </remarks>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <param name="exactly">
        /// If true, a Format Exception is thrown in case of a conversion was 
        /// impossible. Otherwise the Max Value is returned in such a case.
        /// </param>
        /// <param name="nullable">
        /// Indicates whether given type is of type Nullable.
        /// </param>
        /// <param name="culture">
        /// The culture information to be used for conversion.
        /// </param>
        /// <param name="mapping">
        /// The mapping to be used for value transformation. 
        /// </param>
        /// <returns>
        /// An object representing the type-save version of converted value.
        /// A return value of Max Value may indicate a conversion error.
        /// </returns>
        /// <exception cref="FormatException">
        /// This exception is throw in case of a conversion has failed, but 
        /// only if <paramref name="exactly"/> is true.
        /// </exception>
        private static Object AsSingle(String value, Boolean exactly, Boolean nullable, CultureInfo culture, CsvMappings mapping)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                if (nullable)
                {
                    return (Single?)null;
                }

                if (exactly)
                {
                    throw new FormatException("Could not convert an empty string into Single.");
                }

                return Single.MaxValue;
            }

            if (nullable && TypeConverter.IsNullString(value, mapping))
            {
                return (Single?)null;
            }

            try
            {
                return Convert.ToSingle(value, culture);
            }
            catch (Exception exception)
            {
                if (exactly)
                {
                    throw new FormatException($"Could not convert value \"{value}\" into Single.", exception);
                }

                return Single.MaxValue;
            }
        }

        /// <summary>
        /// This method determines whether given type is of type Nullable.
        /// </summary>
        /// <remarks>
        /// Determining if given type is of type 'Nullable' or not directly 
        /// affects how a data conversion takes place.
        /// </remarks>
        /// <param name="type">
        /// The type to be determined.
        /// </param>
        /// <returns>
        /// True, if given type is a Nullable and false otherwise.
        /// </returns>
        private static Boolean IsNullable(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// This method tries to get the 'True' value mapping from given mapping 
        /// instance.
        /// </summary>
        /// <remarks>
        /// The default 'True' value is returned if given mapping is invalid.
        /// </remarks>
        /// <param name="mapping">
        /// An instance of CSV mappings to get the 'True' value from.
        /// </param>
        /// <returns>
        /// The assigned 'True' value or the default 'True' value if the assigned 
        /// value is invalid.
        /// </returns>
        private static String GetTrueMapping(CsvMappings mapping)
        {
            if (mapping != null && mapping.TrueValue != null)
            {
                return mapping.TrueValue;
            }
            else
            {
                return CsvMappings.DefaultTrueValue;
            }
        }

        /// <summary>
        /// This method tries to get the list of 'True' value mappings from given 
        /// mapping instance.
        /// </summary>
        /// <remarks>
        /// The list of default 'True' values is returned if given mapping is invalid.
        /// </remarks>
        /// <param name="mapping">
        /// An instance of CSV mappings to get the 'True' values from.
        /// </param>
        /// <returns>
        /// The assigned list of 'True' values or the list of default 'True' values 
        /// if the assigned list is invalid.
        /// </returns>
        private static List<String> GetTrueMappings(CsvMappings mapping)
        {
            if (mapping != null && mapping.TrueValues != null && mapping.TrueValues.Any())
            {
                return mapping.TrueValues;
            }
            else
            {
                return CsvMappings.DefaultTrueValues;
            }
        }

        /// <summary>
        /// This method tries to get the 'False' value mapping from given mapping 
        /// instance.
        /// </summary>
        /// <remarks>
        /// The default 'False' value is returned if given mapping is invalid.
        /// </remarks>
        /// <param name="mapping">
        /// An instance of CSV mappings to get the 'False' value from.
        /// </param>
        /// <returns>
        /// The assigned 'False' value or the default 'False' value if the assigned 
        /// value is invalid.
        /// </returns>
        private static String GetFalseMapping(CsvMappings mapping)
        {
            if (mapping != null && mapping.FalseValue != null)
            {
                return mapping.FalseValue;
            }
            else
            {
                return CsvMappings.DefaultFalseValue;
            }
        }

        /// <summary>
        /// This method tries to get the list of 'False' value mappings from 
        /// given mapping instance.
        /// </summary>
        /// <remarks>
        /// The list of default 'False' values is returned if given mapping is 
        /// invalid.
        /// </remarks>
        /// <param name="mapping">
        /// An instance of CSV mappings to get the 'False' values from.
        /// </param>
        /// <returns>
        /// The assigned list of 'False' values or the list of default 'False' 
        /// values if the assigned list is invalid.
        /// </returns>
        private static List<String> GetFalseMappings(CsvMappings mapping)
        {
            if (mapping != null && mapping.FalseValues != null && mapping.FalseValues.Any())
            {
                return mapping.FalseValues;
            }
            else
            {
                return CsvMappings.DefaultFalseValues;
            }
        }

        /// <summary>
        /// This method tries to get the 'Null' value mapping from given mapping 
        /// instance.
        /// </summary>
        /// <remarks>
        /// The default 'Null' value is returned if given mapping is invalid.
        /// </remarks>
        /// <param name="mapping">
        /// An instance of CSV mappings to get the 'Null' value from.
        /// </param>
        /// <returns>
        /// The assigned 'Null' value or the default 'Null' value if the assigned 
        /// value is invalid.
        /// </returns>
        private static String GetNullMapping(CsvMappings mapping)
        {
            if (mapping != null && mapping.NullValue != null)
            {
                return mapping.NullValue;
            }
            else
            {
                return CsvMappings.DefaultNullValue;
            }
        }

        /// <summary>
        /// This method tries to get the list of 'Null' value mappings from given 
        /// mapping instance.
        /// </summary>
        /// <remarks>
        /// The list of default 'Null' values is returned if given mapping is invalid.
        /// </remarks>
        /// <param name="mapping">
        /// An instance of CSV mappings to get the 'Null' values from.
        /// </param>
        /// <returns>
        /// The assigned list of 'Null' values or the list of default 'Null' values 
        /// if the assigned list is invalid.
        /// </returns>
        private static List<String> GetNullMappings(CsvMappings mapping)
        {
            if (mapping != null && mapping.NullValues != null && mapping.NullValues.Any())
            {
                return mapping.NullValues;
            }
            else
            {
                return CsvMappings.DefaultNullValues;
            }
        }

        /// <summary>
        /// This method check if given value is included in the list of 'True' values 
        /// available within given CSV mappings.
        /// </summary>
        /// <remarks>
        /// This method determines whether given value is one of the mapped 'True' values.
        /// </remarks>
        /// <param name="value">
        /// The value to be checked.
        /// </param>
        /// <param name="mapping">
        /// An instance of CSV mappings to get the 'Null' values from.
        /// </param>
        /// <returns>
        /// True is returned if given value could be found in the list of 'True' values 
        /// of CSV mappings. Otherwise, false is returned.
        /// </returns>
        private static Boolean IsTrueString(String value, CsvMappings mapping)
        {
            return TypeConverter.GetTrueMappings(mapping).Any(x => String.Compare(value, x, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// This method check if given value is included in the list of 'False' values 
        /// available within given CSV mappings.
        /// </summary>
        /// <remarks>
        /// This method determines whether given value is one of the mapped 'False' values.
        /// </remarks>
        /// <param name="value">
        /// The value to be checked.
        /// </param>
        /// <param name="mapping">
        /// An instance of CSV mappings to get the 'Null' values from.
        /// </param>
        /// <returns>
        /// True is returned if given value could be found in the list of 'False' values 
        /// of CSV mappings. Otherwise, false is returned.
        /// </returns>
        private static Boolean IsFalseString(String value, CsvMappings mapping)
        {
            return TypeConverter.GetFalseMappings(mapping).Any(x => String.Compare(value, x, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// This method check if given value is included in the list of 'Null' values 
        /// available within given CSV mappings.
        /// </summary>
        /// <remarks>
        /// This method determines whether given value is one of the mapped 'Null' values.
        /// </remarks>
        /// <param name="value">
        /// The value to be checked.
        /// </param>
        /// <param name="mapping">
        /// An instance of CSV mappings to get the 'Null' values from.
        /// </param>
        /// <returns>
        /// True is returned if given value could be found in the list of 'Null' values 
        /// of CSV mappings. Otherwise, false is returned.
        /// </returns>
        private static Boolean IsNullString(String value, CsvMappings mapping)
        {
            return TypeConverter.GetNullMappings(mapping).Any(x => String.Compare(value, x, StringComparison.OrdinalIgnoreCase) == 0);
        }

        #endregion
    }
}
