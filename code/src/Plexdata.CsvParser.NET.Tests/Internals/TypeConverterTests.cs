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

using NUnit.Framework;
using Plexdata.CsvParser.Internals;
using Plexdata.CsvParser.Processors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Plexdata.CsvParser.Tests.Attributes
{
    [TestFixture]
    [TestOf(nameof(TypeConverter))]
    public class TypeConverterTests
    {
        [Test]
        public void IntoString_CultureInfoIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => { TypeConverter.IntoString(null, null, CsvMappings.DefaultMappings); });
        }

        [Test]
        public void IntoString_MappingInfoIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => { TypeConverter.IntoString(null, CultureInfo.CurrentCulture, null); });
        }

        [Test]
        [TestCase("", null, "")]
        [TestCase("string", null, "string")]
        [TestCase('#', null, "#")]
        [TestCase('\t', null, "\t")]
        [TestCase("", "en-US", "")]
        [TestCase("string", "en-US", "string")]
        [TestCase('#', "en-US", "#")]
        [TestCase('\t', "en-US", "\t")]
        public void IntoString_ConvertingStringsAndChars_ResultAreEqual(Object value, String culture, String expected)
        {
            CultureInfo cultureInfo = culture == null ? CultureInfo.CurrentUICulture : new CultureInfo(culture);
            Assert.AreEqual(expected, TypeConverter.IntoString(value, cultureInfo, CsvMappings.DefaultMappings));
        }

        [Test]
        [TestCase("string", null, "string")]
        [TestCase(true, null, "true")]
        [TestCase('#', null, "#")]
        [TestCase(unchecked((SByte)0xFF), null, "-1")]
        [TestCase((Byte)0x42, null, "66")]
        [TestCase(unchecked((UInt16)0xFFFF), null, "65535")]
        [TestCase((Int16)0x42, null, "66")]
        [TestCase(unchecked((UInt32)0xFFFFFFFF), null, "4294967295")]
        [TestCase((Int32)0x42, null, "66")]
        [TestCase(unchecked((UInt64)0xFFFFFFFFFFFFFFFF), null, "18446744073709551615")]
        [TestCase((Int64)0x42, null, "66")]
        [TestCase("string", "en-US", "string")]
        [TestCase(true, "en-US", "true")]
        [TestCase('#', "en-US", "#")]
        [TestCase(unchecked((SByte)0xFF), "en-US", "-1")]
        [TestCase((Byte)0x42, "en-US", "66")]
        [TestCase(unchecked((UInt16)0xFFFF), "en-US", "65535")]
        [TestCase((Int16)0x42, "en-US", "66")]
        [TestCase(unchecked((UInt32)0xFFFFFFFF), "en-US", "4294967295")]
        [TestCase((Int32)0x42, "en-US", "66")]
        [TestCase(unchecked((UInt64)0xFFFFFFFFFFFFFFFF), "en-US", "18446744073709551615")]
        [TestCase((Int64)0x42, "en-US", "66")]
        public void IntoString_ConvertingIntegerTypes_ResultAreEqual(Object value, String culture, String expected)
        {
            CultureInfo cultureInfo = culture == null ? CultureInfo.CurrentUICulture : new CultureInfo(culture);
            Assert.AreEqual(expected, TypeConverter.IntoString(value, cultureInfo, CsvMappings.DefaultMappings));
        }

        [Test]
        [TestCase(true, null, "true")]
        [TestCase(false, null, "false")]
        [TestCase(true, "en-US", "true")]
        [TestCase(false, "en-US", "false")]
        public void IntoString_ConvertingBooleanTypes_ResultAreEqual(Object value, String culture, String expected)
        {
            CultureInfo cultureInfo = culture == null ? CultureInfo.CurrentUICulture : new CultureInfo(culture);
            Assert.AreEqual(expected, TypeConverter.IntoString(value, cultureInfo, CsvMappings.DefaultMappings));
        }

        [Test]
        [TestCase(typeof(Decimal), 0.1234, null, "0,1234")]
        [TestCase(typeof(Double), 0.1234, null, "0,1234")]
        [TestCase(typeof(Single), 0.1234, null, "0,1234")]
        [TestCase(typeof(Decimal), 123456.789, null, "123456,789")]
        [TestCase(typeof(Double), 123456.789, null, "123456,789")]
        [TestCase(typeof(Single), 123456.789, null, "123456,8")]
        [TestCase(typeof(Decimal), 0.1234, "en-US", "0.1234")]
        [TestCase(typeof(Double), 0.1234, "en-US", "0.1234")]
        [TestCase(typeof(Single), 0.1234, "en-US", "0.1234")]
        [TestCase(typeof(Decimal), 123456.789, "en-US", "123456.789")]
        [TestCase(typeof(Double), 123456.789, "en-US", "123456.789")]
        [TestCase(typeof(Single), 123456.789, "en-US", "123456.8")]
        public void IntoString_ConvertingFloatingPointTypes_ResultAreEqual(Type type, Object value, String culture, String expected)
        {
            dynamic actual = null;

            if (type == typeof(Decimal))
            {
                actual = Convert.ToDecimal(value);
            }
            else if (type == typeof(Double))
            {
                actual = Convert.ToDouble(value);
            }
            else if (type == typeof(Single))
            {
                actual = Convert.ToSingle(value);
            }
            else
            {
                Assert.IsFalse(true);
            }

            CultureInfo cultureInfo = culture == null ? CultureInfo.CurrentUICulture : new CultureInfo(culture);
            Assert.AreEqual(expected, TypeConverter.IntoString(actual, cultureInfo, CsvMappings.DefaultMappings));
        }

        [Test]
        [TestCase(null)]
        [TestCase("en-US")]
        public void IntoString_ConvertingDateTimeTypes_ResultAreEqual(String culture)
        {
            DateTime actual = DateTime.Now;
            String expected = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", actual.Year, actual.Month, actual.Day, actual.Hour, actual.Minute, actual.Second);
            CultureInfo cultureInfo = culture == null ? CultureInfo.CurrentUICulture : new CultureInfo(culture);
            Assert.AreEqual(expected, TypeConverter.IntoString(actual, cultureInfo, CsvMappings.DefaultMappings));
        }

        [Test]
        [TestCase(typeof(List<String>), null, "System.Collections.Generic.List`1[System.String]")]
        [TestCase(typeof(Object), null, "System.Object")]
        [TestCase(typeof(Nullable), null, "")]
        [TestCase(typeof(List<String>), "en-US", "System.Collections.Generic.List`1[System.String]")]
        [TestCase(typeof(Object), "en-US", "System.Object")]
        [TestCase(typeof(Nullable), "en-US", "")]
        public void IntoString_OtherObject_ResultAreEqual(Type type, String culture, String expected)
        {
            dynamic actual = null;

            if (type == typeof(List<String>))
            {
                actual = new List<String>();
            }
            else if (type == typeof(Object))
            {
                actual = new Object();
            }
            else if (type == typeof(Nullable))
            {
                actual = null;
            }
            else
            {
                Assert.IsFalse(true);
            }

            CultureInfo cultureInfo = culture == null ? CultureInfo.CurrentUICulture : new CultureInfo(culture);
            Assert.AreEqual(expected, TypeConverter.IntoString(actual, cultureInfo, CsvMappings.DefaultMappings));
        }

        [Test]
        [TestCase(null, typeof(Boolean), "de-DE", TestName = "IntoObject(Value: null, Type: valid, Exactly: false, Culture: valid)")]
        [TestCase("true", null, "de-DE", TestName = "IntoObject(Value: valid, Type: null, Exactly: false, Culture: valid)")]
        [TestCase("true", typeof(Boolean), null, TestName = "IntoObject(Value: valid, Type: valid, Exactly: false, Culture: null)")]
        [TestCase("true", typeof(Nullable<>), "de-DE", TestName = "IntoObject(Value: valid, Type: nullable, Exactly: false, Culture: valid)")]
        public void IntoObject_ParameterCheck_ThrowsArgumentNullException(String value, Type type, String culture)
        {
            CultureInfo info = null;

            if (culture != null)
            {
                info = new CultureInfo(culture);
            }

            Assert.Throws<ArgumentNullException>(() => { TypeConverter.IntoObject(value, type, false, info, null); });
        }

        [Test]
        [TestCase(TestName = "IntoObject(Value: valid, Type: unsupported, Exactly: false, Culture: valid)")]
        public void IntoObject_UnsupportedType_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => { TypeConverter.IntoObject("value", typeof(TestCaseItem), false, CultureInfo.CurrentUICulture, null); });
        }

        [Test]
        [TestCase(typeof(String), false, TestName = "IsNullable(Type: String, Expected: false)")]
        [TestCase(typeof(Boolean), false, TestName = "IsNullable(Type: Boolean, Expected: false)")]
        [TestCase(typeof(Boolean?), true, TestName = "IsNullable(Type: Boolean?, Expected: true)")]
        [TestCase(typeof(Int32), false, TestName = "IsNullable(Type: Int32, Expected: false)")]
        [TestCase(typeof(Int32?), true, TestName = "IsNullable(Type: Int32?, Expected: true)")]
        [TestCase(typeof(DateTime), false, TestName = "IsNullable(Type: DateTime, Expected: false)")]
        [TestCase(typeof(DateTime?), true, TestName = "IsNullable(Type: DateTime?, Expected: true)")]
        public void IsNullable_VariousTypes_ResultAsExpected(Type type, Boolean expected)
        {
            Assert.AreEqual(expected, (Boolean)this.ExecutePrivateStaticMethod("IsNullable", new Object[] { type }));
        }

        [Test]
        [TestCaseSource("GetTrueMappingsTestCases")]
        public void GetTrueMappings_VariousValues_ResultAsExpected(Object data)
        {
            TestCaseMapping item = (TestCaseMapping)data;

            List<String> actual = (List<String>)this.ExecutePrivateStaticMethod("GetTrueMappings", new Object[] { item.Mapping });

            Assert.AreEqual(item.Expected, String.Join("", actual));
        }

        [Test]
        [TestCaseSource("GetFalseMappingsTestCases")]
        public void GetFalseMappings_VariousValues_ResultAsExpected(Object data)
        {
            TestCaseMapping item = (TestCaseMapping)data;

            List<String> actual = (List<String>)this.ExecutePrivateStaticMethod("GetFalseMappings", new Object[] { item.Mapping });

            Assert.AreEqual(item.Expected, String.Join("", actual));
        }

        [Test]
        [TestCaseSource("GetNullMappingsTestCases")]
        public void GetNullMappings_VariousValues_ResultAsExpected(Object data)
        {
            TestCaseMapping item = (TestCaseMapping)data;

            List<String> actual = (List<String>)this.ExecutePrivateStaticMethod("GetNullMappings", new Object[] { item.Mapping });

            Assert.AreEqual(item.Expected, String.Join("", actual));
        }

        [Test]
        [TestCaseSource("IsTrueStringTestCases")]
        public void IsTrueString_VariousValues_ResultAsExpected(Object data)
        {
            TestCaseMapping item = (TestCaseMapping)data;

            Boolean actual = (Boolean)this.ExecutePrivateStaticMethod("IsTrueString", new Object[] { item.Value, item.Mapping });

            Assert.AreEqual(item.Expected, actual);
        }

        [Test]
        [TestCaseSource("IsFalseStringTestCases")]
        public void IsFalseString_VariousValues_ResultAsExpected(Object data)
        {
            TestCaseMapping item = (TestCaseMapping)data;

            Boolean actual = (Boolean)this.ExecutePrivateStaticMethod("IsFalseString", new Object[] { item.Value, item.Mapping });

            Assert.AreEqual(item.Expected, actual);
        }

        [Test]
        [TestCaseSource("IsNullStringTestCases")]
        public void IsNullString_VariousValues_ResultAsExpected(Object data)
        {
            TestCaseMapping item = (TestCaseMapping)data;

            Boolean actual = (Boolean)this.ExecutePrivateStaticMethod("IsNullString", new Object[] { item.Value, item.Mapping });

            Assert.AreEqual(item.Expected, actual);
        }

        [Test]
        [TestCase(null, false, false, null, TestName = "AsString(Value: null, Exactly: false, Nullable: false, Expected: null)")]
        [TestCase(null, true, false, null, TestName = "AsString(Value: null, Exactly: true, Nullable: false, Expected: null)")]
        [TestCase(null, false, true, null, TestName = "AsString(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase(null, true, true, null, TestName = "AsString(Value: null, Exactly: true, Nullable: true, Expected: null)")]
        [TestCase("", false, false, "", TestName = "AsString(Value: empty, Exactly: false, Nullable: false, Expected: empty)")]
        [TestCase("", true, false, "", TestName = "AsString(Value: empty, Exactly: true, Nullable: false, Expected: empty)")]
        [TestCase("", false, true, "", TestName = "AsString(Value: empty, Exactly: false, Nullable: true, Expected: empty)")]
        [TestCase("", true, true, "", TestName = "AsString(Value: empty, Exactly: true, Nullable: true, Expected: empty)")]
        [TestCase("   ", false, false, "   ", TestName = "AsString(Value: whitespace, Exactly: false, Nullable: false, Expected: whitespace)")]
        [TestCase("   ", true, false, "   ", TestName = "AsString(Value: whitespace, Exactly: true, Nullable: false, Expected: whitespace)")]
        [TestCase("   ", false, true, "   ", TestName = "AsString(Value: whitespace, Exactly: false, Nullable: true, Expected: whitespace)")]
        [TestCase("   ", true, true, "   ", TestName = "AsString(Value: whitespace, Exactly: true, Nullable: true, Expected: whitespace)")]
        [TestCase("hello", false, false, "hello", TestName = "AsString(Value: hello, Exactly: false, Nullable: false, Expected: hello)")]
        [TestCase("hello", true, false, "hello", TestName = "AsString(Value: hello, Exactly: true, Nullable: false, Expected: hello)")]
        [TestCase("hello", false, true, "hello", TestName = "AsString(Value: hello, Exactly: false, Nullable: true, Expected: hello)")]
        [TestCase("hello", true, true, "hello", TestName = "AsString(Value: hello, Exactly: true, Nullable: true, Expected: hello)")]
        [TestCase("<null>", true, true, null, TestName = "AsString(Value: <null>, Exactly: true, Nullable: true, Expected: null)")]
        [TestCase("<null>", false, true, null, TestName = "AsString(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsString_VariousValues_ResultAsExpected(String value, Boolean exactly, Boolean nullable, Object expected)
        {
            Assert.AreEqual((String)expected, (String)this.ExecutePrivateStaticMethod("AsString", new Object[] { value, exactly, nullable, CultureInfo.CurrentUICulture, null }));
        }

        [Test]
        [TestCase(null, false, false, TestName = "AsBoolean(Value: null, Exactly: false, Nullable: false, Expected: false)")]
        [TestCase(null, true, null, TestName = "AsBoolean(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("", false, false, TestName = "AsBoolean(Value: empty, Exactly: false, Nullable: false, Expected: false)")]
        [TestCase("", true, null, TestName = "AsBoolean(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("   ", false, false, TestName = "AsBoolean(Value: empty, Exactly: false, Nullable: false, Expected: false)")]
        [TestCase("   ", true, null, TestName = "AsBoolean(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("true", false, true, TestName = "AsBoolean(Value: true, Exactly: false, Nullable: false, Expected: true)")]
        [TestCase("true", true, true, TestName = "AsBoolean(Value: true, Exactly: false, Nullable: true, Expected: true)")]
        [TestCase("1", false, true, TestName = "AsBoolean(Value: 1, Exactly: false, Nullable: false, Expected: true)")]
        [TestCase("1", true, true, TestName = "AsBoolean(Value: 1, Exactly: false, Nullable: true, Expected: true)")]
        [TestCase("false", false, false, TestName = "AsBoolean(Value: false, Exactly: false, Nullable: false, Expected: false)")]
        [TestCase("false", true, false, TestName = "AsBoolean(Value: false, Exactly: false, Nullable: true, Expected: false)")]
        [TestCase("0", false, false, TestName = "AsBoolean(Value: 0, Exactly: false, Nullable: false, Expected: false)")]
        [TestCase("0", true, false, TestName = "AsBoolean(Value: 0, Exactly: false, Nullable: true, Expected: false)")]
        [TestCase("hello", false, false, TestName = "AsBoolean(Value: hello, Exactly: false, Nullable: false, Expected: false)")]
        [TestCase("hello", true, false, TestName = "AsBoolean(Value: hello, Exactly: false, Nullable: true, Expected: false)")]
        [TestCase("<null>", true, null, TestName = "AsBoolean(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsBoolean_VariousValuesNotExactly_ResultAsExpected(String value, Boolean nullable, Object expected)
        {
            if (nullable)
            {
                Assert.AreEqual((Boolean?)expected, (Boolean?)this.ExecutePrivateStaticMethod("AsBoolean", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((Boolean)expected, (Boolean)this.ExecutePrivateStaticMethod("AsBoolean", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCase(null, TestName = "AsBoolean(Value: null, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("", TestName = "AsBoolean(Value: empty, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("   ", TestName = "AsBoolean(Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("hello", TestName = "AsBoolean(Value: hello, Exactly: true, Nullable: false, Expected: FormatException)")]
        public void AsBoolean_VariousValuesExactly_ThrowsFormatException(String value)
        {
            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsBoolean", new Object[] { value, true, false, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCase(null, false, '\uffff', TestName = "AsCharacter(Value: null, Exactly: false, Nullable: false, Expected: max-char)")]
        [TestCase(null, true, null, TestName = "AsCharacter(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("", false, '\uffff', TestName = "AsCharacter(Value: empty, Exactly: false, Nullable: false, Expected: max-char)")]
        [TestCase("", true, null, TestName = "AsCharacter(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("   ", false, '\uffff', TestName = "AsCharacter(Value: whitespace, Exactly: false, Nullable: false, Expected: max-char)")]
        [TestCase("   ", true, null, TestName = "AsCharacter(Value: whitespace, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("A", false, 'A', TestName = "AsCharacter(Value: A, Exactly: false, Nullable: false, Expected: A)")]
        [TestCase("A", true, 'A', TestName = "AsCharacter(Value: A, Exactly: false, Nullable: true, Expected: A)")]
        [TestCase("toolong", false, '\uffff', TestName = "AsCharacter(Value: toolong, Exactly: false, Nullable: false, Expected: max-char)")]
        [TestCase("toolong", true, '\uffff', TestName = "AsCharacter(Value: toolong, Exactly: false, Nullable: true, Expected: max-char)")]
        [TestCase("<null>", true, null, TestName = "AsCharacter(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsCharacter_VariousValuesNotExactly_ResultAsExpected(String value, Boolean nullable, Object expected)
        {
            if (nullable)
            {
                Assert.AreEqual((Char?)expected, (Char?)this.ExecutePrivateStaticMethod("AsCharacter", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((Char)expected, (Char)this.ExecutePrivateStaticMethod("AsCharacter", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCase(null, TestName = "AsCharacter(Value: null, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("", TestName = "AsCharacter(Value: empty, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("   ", TestName = "AsCharacter(Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("toolong", TestName = "AsCharacter(Value: toolong, Exactly: true, Nullable: false, Expected: FormatException)")]
        public void AsCharacter_VariousValuesExactly_ThrowsFormatException(String value)
        {
            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsCharacter", new Object[] { value, true, false, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCase(null, false, (SByte)127, TestName = "AsSInt8(Value: null, Exactly: false, Nullable: false, Expected: max-signed-byte)")]
        [TestCase(null, true, null, TestName = "AsSInt8(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("", false, (SByte)127, TestName = "AsSInt8(Value: empty, Exactly: false, Nullable: false, Expected: max-signed-byte)")]
        [TestCase("", true, null, TestName = "AsSInt8(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("   ", false, (SByte)127, TestName = "AsSInt8(Value: whitespace, Exactly: false, Nullable: false, Expected: max-signed-byte)")]
        [TestCase("   ", true, null, TestName = "AsSInt8(Value: whitespace, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("42", false, (SByte)42, TestName = "AsSInt8(Value: 42, Exactly: false, Nullable: false, Expected: 42)")]
        [TestCase("42", true, (SByte)42, TestName = "AsSInt8(Value: 42, Exactly: false, Nullable: true, Expected: 42)")]
        [TestCase("-42", false, (SByte)(-42), TestName = "AsSInt8(Value: -42, Exactly: false, Nullable: false, Expected: -42)")]
        [TestCase("-42", true, (SByte)(-42), TestName = "AsSInt8(Value: -42, Exactly: false, Nullable: true, Expected: -42)")]
        [TestCase("123456789", false, (SByte)127, TestName = "AsSInt8(Value: 123456789, Exactly: false, Nullable: false, Expected: max-signed-byte)")]
        [TestCase("123456789", true, (SByte)127, TestName = "AsSInt8(Value: 123456789, Exactly: false, Nullable: true, Expected: max-signed-byte)")]
        [TestCase("<null>", true, null, TestName = "AsSInt8(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsSInt8_VariousValuesNotExactly_ResultAsExpected(String value, Boolean nullable, Object expected)
        {
            if (nullable)
            {
                Assert.AreEqual((SByte?)expected, (SByte?)this.ExecutePrivateStaticMethod("AsSInt8", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((SByte)expected, (SByte)this.ExecutePrivateStaticMethod("AsSInt8", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCase(null, TestName = "AsSInt8(Value: null, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("", TestName = "AsSInt8(Value: empty, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("   ", TestName = "AsSInt8(Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("123456789", TestName = "AsSInt8(Value: 123456789, Exactly: true, Nullable: false, Expected: FormatException)")]
        public void AsSInt8_VariousValuesExactly_ThrowsFormatException(String value)
        {
            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsSInt8", new Object[] { value, true, false, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCase(null, false, (Byte)255, TestName = "AsUInt8(Value: null, Exactly: false, Nullable: false, Expected: max-unsigned-byte)")]
        [TestCase(null, true, null, TestName = "AsUInt8(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("", false, (Byte)255, TestName = "AsUInt8(Value: empty, Exactly: false, Nullable: false, Expected: max-unsigned-byte)")]
        [TestCase("", true, null, TestName = "AsUInt8(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("   ", false, (Byte)255, TestName = "AsUInt8(Value: whitespace, Exactly: false, Nullable: false, Expected: max-unsigned-byte)")]
        [TestCase("   ", true, null, TestName = "AsUInt8(Value: whitespace, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("42", false, (Byte)42, TestName = "AsUInt8(Value: 42, Exactly: false, Nullable: false, Expected: 42)")]
        [TestCase("42", true, (Byte)42, TestName = "AsUInt8(Value: 42, Exactly: false, Nullable: true, Expected: 42)")]
        [TestCase("123456789", false, (Byte)255, TestName = "AsUInt8(Value: 123456789, Exactly: false, Nullable: false, Expected: max-unsigned-byte)")]
        [TestCase("123456789", true, (Byte)255, TestName = "AsUInt8(Value: 123456789, Exactly: false, Nullable: true, Expected: max-unsigned-byte)")]
        [TestCase("<null>", true, null, TestName = "AsUInt8(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsUInt8_VariousValuesNotExactly_ResultAsExpected(String value, Boolean nullable, Object expected)
        {
            if (nullable)
            {
                Assert.AreEqual((Byte?)expected, (Byte?)this.ExecutePrivateStaticMethod("AsUInt8", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((Byte)expected, (Byte)this.ExecutePrivateStaticMethod("AsUInt8", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCase(null, TestName = "AsUInt8(Value: null, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("", TestName = "AsUInt8(Value: empty, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("   ", TestName = "AsUInt8(Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("123456789", TestName = "AsUInt8(Value: 123456789, Exactly: true, Nullable: false, Expected: FormatException)")]
        public void AsUInt8_VariousValuesExactly_ThrowsFormatException(String value)
        {
            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsUInt8", new Object[] { value, true, false, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCase(null, false, (Int16)32767, TestName = "AsSInt16(Value: null, Exactly: false, Nullable: false, Expected: max-signed-short)")]
        [TestCase(null, true, null, TestName = "AsSInt16(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("", false, (Int16)32767, TestName = "AsSInt16(Value: empty, Exactly: false, Nullable: false, Expected: max-signed-short)")]
        [TestCase("", true, null, TestName = "AsSInt16(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("   ", false, (Int16)32767, TestName = "AsSInt16(Value: whitespace, Exactly: false, Nullable: false, Expected: max-signed-short)")]
        [TestCase("   ", true, null, TestName = "AsSInt16(Value: whitespace, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("4200", false, (Int16)4200, TestName = "AsSInt16(Value: 4200, Exactly: false, Nullable: false, Expected: 4200)")]
        [TestCase("4200", true, (Int16)4200, TestName = "AsSInt16(Value: 4200, Exactly: false, Nullable: true, Expected: 4200)")]
        [TestCase("-4200", false, (Int16)(-4200), TestName = "AsSInt16(Value: -4200, Exactly: false, Nullable: false, Expected: -4200)")]
        [TestCase("-4200", true, (Int16)(-4200), TestName = "AsSInt16(Value: -4200, Exactly: false, Nullable: true, Expected: -4200)")]
        [TestCase("123456789", false, (Int16)32767, TestName = "AsSInt16(Value: 123456789, Exactly: false, Nullable: false, Expected: max-signed-short)")]
        [TestCase("123456789", true, (Int16)32767, TestName = "AsSInt16(Value: 123456789, Exactly: false, Nullable: true, Expected: max-signed-short)")]
        [TestCase("<null>", true, null, TestName = "AsSInt16(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsSInt16_VariousValuesNotExactly_ResultAsExpected(String value, Boolean nullable, Object expected)
        {
            if (nullable)
            {
                Assert.AreEqual((Int16?)expected, (Int16?)this.ExecutePrivateStaticMethod("AsSInt16", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((Int16)expected, (Int16)this.ExecutePrivateStaticMethod("AsSInt16", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCase(null, TestName = "AsSInt16(Value: null, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("", TestName = "AsSInt16(Value: empty, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("   ", TestName = "AsSInt16(Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("123456789", TestName = "AsSInt16(Value: 123456789, Exactly: true, Nullable: false, Expected: FormatException)")]
        public void AsSInt16_VariousValuesExactly_ThrowsFormatException(String value)
        {
            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsSInt16", new Object[] { value, true, false, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCase(null, false, (UInt16)65535, TestName = "AsUInt16(Value: null, Exactly: false, Nullable: false, Expected: max-unsigned-short)")]
        [TestCase(null, true, null, TestName = "AsUInt16(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("", false, (UInt16)65535, TestName = "AsUInt16(Value: empty, Exactly: false, Nullable: false, Expected: max-unsigned-short)")]
        [TestCase("", true, null, TestName = "AsUInt16(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("   ", false, (UInt16)65535, TestName = "AsUInt16(Value: whitespace, Exactly: false, Nullable: false, Expected: max-unsigned-short)")]
        [TestCase("   ", true, null, TestName = "AsUInt16(Value: whitespace, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("4200", false, (UInt16)4200, TestName = "AsUInt16(Value: 4200, Exactly: false, Nullable: false, Expected: 4200)")]
        [TestCase("4200", true, (UInt16)4200, TestName = "AsUInt16(Value: 4200, Exactly: false, Nullable: true, Expected: 4200)")]
        [TestCase("123456789", false, (UInt16)65535, TestName = "AsUInt16(Value: 123456789, Exactly: false, Nullable: false, Expected: max-unsigned-short)")]
        [TestCase("123456789", true, (UInt16)65535, TestName = "AsUInt16(Value: 123456789, Exactly: false, Nullable: true, Expected: max-unsigned-short)")]
        [TestCase("<null>", true, null, TestName = "AsUInt16(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsUInt16_VariousValuesNotExactly_ResultAsExpected(String value, Boolean nullable, Object expected)
        {
            if (nullable)
            {
                Assert.AreEqual((UInt16?)expected, (UInt16?)this.ExecutePrivateStaticMethod("AsUInt16", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((UInt16)expected, (UInt16)this.ExecutePrivateStaticMethod("AsUInt16", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCase(null, TestName = "AsUInt16(Value: null, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("", TestName = "AsUInt16(Value: empty, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("   ", TestName = "AsUInt16(Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("123456789", TestName = "AsUInt16(Value: 123456789, Exactly: true, Nullable: false, Expected: FormatException)")]
        public void AsUInt16_VariousValuesExactly_ThrowsFormatException(String value)
        {
            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsUInt16", new Object[] { value, true, false, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCase(null, false, (Int32)2147483647, TestName = "AsSInt32(Value: null, Exactly: false, Nullable: false, Expected: max-signed-integer)")]
        [TestCase(null, true, null, TestName = "AsSInt32(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("", false, (Int32)2147483647, TestName = "AsSInt32(Value: empty, Exactly: false, Nullable: false, Expected: max-signed-integer)")]
        [TestCase("", true, null, TestName = "AsSInt32(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("   ", false, (Int32)2147483647, TestName = "AsSInt32(Value: whitespace, Exactly: false, Nullable: false, Expected: max-signed-integer)")]
        [TestCase("   ", true, null, TestName = "AsSInt32(Value: whitespace, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("42004200", false, (Int32)42004200, TestName = "AsSInt32(Value: 42004200, Exactly: false, Nullable: false, Expected: 42004200)")]
        [TestCase("42004200", true, (Int32)42004200, TestName = "AsSInt32(Value: 42004200, Exactly: false, Nullable: true, Expected: 42004200)")]
        [TestCase("-42004200", false, (Int32)(-42004200), TestName = "AsSInt32(Value: -42004200, Exactly: false, Nullable: false, Expected: -42004200)")]
        [TestCase("-42004200", true, (Int32)(-42004200), TestName = "AsSInt32(Value: -42004200, Exactly: false, Nullable: true, Expected: -42004200)")]
        [TestCase("420042004200", false, (Int32)2147483647, TestName = "AsSInt32(Value: 420042004200, Exactly: false, Nullable: false, Expected: max-signed-integer)")]
        [TestCase("420042004200", true, (Int32)2147483647, TestName = "AsSInt32(Value: 420042004200, Exactly: false, Nullable: true, Expected: max-signed-integer)")]
        [TestCase("<null>", true, null, TestName = "AsSInt32(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsSInt32_VariousValuesNotExactly_ResultAsExpected(String value, Boolean nullable, Object expected)
        {
            if (nullable)
            {
                Assert.AreEqual((Int32?)expected, (Int32?)this.ExecutePrivateStaticMethod("AsSInt32", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((Int32)expected, (Int32)this.ExecutePrivateStaticMethod("AsSInt32", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCase(null, TestName = "AsSInt32(Value: null, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("", TestName = "AsSInt32(Value: empty, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("   ", TestName = "AsSInt32(Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("420042004200", TestName = "AsSInt32(Value: 420042004200, Exactly: true, Nullable: false, Expected: FormatException)")]
        public void AsSInt32_VariousValuesExactly_ThrowsFormatException(String value)
        {
            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsSInt32", new Object[] { value, true, false, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCase(null, false, (UInt32)4294967295, TestName = "AsUInt32(Value: null, Exactly: false, Nullable: false, Expected: max-unsigned-integer)")]
        [TestCase(null, true, null, TestName = "AsUInt32(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("", false, (UInt32)4294967295, TestName = "AsUInt32(Value: empty, Exactly: false, Nullable: false, Expected: max-unsigned-integer)")]
        [TestCase("", true, null, TestName = "AsUInt32(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("   ", false, (UInt32)4294967295, TestName = "AsUInt32(Value: whitespace, Exactly: false, Nullable: false, Expected: max-unsigned-integer)")]
        [TestCase("   ", true, null, TestName = "AsUInt32(Value: whitespace, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("4200420042", false, (UInt32)4200420042, TestName = "AsUInt32(Value: 4200420042, Exactly: false, Nullable: false, Expected: 4200420042)")]
        [TestCase("4200420042", true, (UInt32)4200420042, TestName = "AsUInt32(Value: 4200420042, Exactly: false, Nullable: true, Expected: 4200420042)")]
        [TestCase("420042004200", false, (UInt32)4294967295, TestName = "AsUInt32(Value: 420042004200, Exactly: false, Nullable: false, Expected: max-unsigned-integer)")]
        [TestCase("420042004200", true, (UInt32)4294967295, TestName = "AsUInt32(Value: 420042004200, Exactly: false, Nullable: true, Expected: max-unsigned-integer)")]
        [TestCase("<null>", true, null, TestName = "AsUInt32(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsUInt32_VariousValuesNotExactly_ResultAsExpected(String value, Boolean nullable, Object expected)
        {
            if (nullable)
            {
                Assert.AreEqual((UInt32?)expected, (UInt32?)this.ExecutePrivateStaticMethod("AsUInt32", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((UInt32)expected, (UInt32)this.ExecutePrivateStaticMethod("AsUInt32", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCase(null, TestName = "AsUInt32(Value: null, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("", TestName = "AsUInt32(Value: empty, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("   ", TestName = "AsUInt32(Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("420042004200", TestName = "AsUInt32(Value: 420042004200, Exactly: true, Nullable: false, Expected: FormatException)")]
        public void AsUInt32_VariousValuesExactly_ThrowsFormatException(String value)
        {
            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsUInt32", new Object[] { value, true, false, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCase(null, false, (Int64)9223372036854775807, TestName = "AsSInt64(Value: null, Exactly: false, Nullable: false, Expected: max-signed-long)")]
        [TestCase(null, true, null, TestName = "AsSInt64(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("", false, (Int64)9223372036854775807, TestName = "AsSInt64(Value: empty, Exactly: false, Nullable: false, Expected: max-signed-long)")]
        [TestCase("", true, null, TestName = "AsSInt64(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("   ", false, (Int64)9223372036854775807, TestName = "AsSInt64(Value: whitespace, Exactly: false, Nullable: false, Expected: max-signed-long)")]
        [TestCase("   ", true, null, TestName = "AsSInt64(Value: whitespace, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("4200420042004200", false, (Int64)4200420042004200, TestName = "AsSInt64(Value: 4200420042004200, Exactly: false, Nullable: false, Expected: 4200420042004200)")]
        [TestCase("4200420042004200", true, (Int64)4200420042004200, TestName = "AsSInt64(Value: 4200420042004200, Exactly: false, Nullable: true, Expected: 4200420042004200)")]
        [TestCase("-4200420042004200", false, (Int64)(-4200420042004200), TestName = "AsSInt64(Value: -4200420042004200, Exactly: false, Nullable: false, Expected: -4200420042004200)")]
        [TestCase("-4200420042004200", true, (Int64)(-4200420042004200), TestName = "AsSInt64(Value: -4200420042004200, Exactly: false, Nullable: true, Expected: -4200420042004200)")]
        [TestCase("42004200420042004200", false, (Int64)9223372036854775807, TestName = "AsSInt64(Value: 42004200420042004200, Exactly: false, Nullable: false, Expected: max-signed-long)")]
        [TestCase("42004200420042004200", true, (Int64)9223372036854775807, TestName = "AsSInt64(Value: 42004200420042004200, Exactly: false, Nullable: true, Expected: max-signed-long)")]
        [TestCase("<null>", true, null, TestName = "AsSInt64(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsSInt64_VariousValuesNotExactly_ResultAsExpected(String value, Boolean nullable, Object expected)
        {
            if (nullable)
            {
                Assert.AreEqual((Int64?)expected, (Int64?)this.ExecutePrivateStaticMethod("AsSInt64", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((Int64)expected, (Int64)this.ExecutePrivateStaticMethod("AsSInt64", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCase(null, TestName = "AsSInt64(Value: null, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("", TestName = "AsSInt64(Value: empty, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("   ", TestName = "AsSInt64(Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("42004200420042004200", TestName = "AsSInt64(Value: 42004200420042004200, Exactly: true, Nullable: false, Expected: FormatException)")]
        public void AsSInt64_VariousValuesExactly_ThrowsFormatException(String value)
        {
            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsSInt64", new Object[] { value, true, false, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCase(null, false, (UInt64)18446744073709551615, TestName = "AsUInt64(Value: null, Exactly: false, Nullable: false, Expected: max-unsigned-long)")]
        [TestCase(null, true, null, TestName = "AsUInt64(Value: null, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("", false, (UInt64)18446744073709551615, TestName = "AsUInt64(Value: empty, Exactly: false, Nullable: false, Expected: max-unsigned-long)")]
        [TestCase("", true, null, TestName = "AsUInt64(Value: empty, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("   ", false, (UInt64)18446744073709551615, TestName = "AsUInt64(Value: whitespace, Exactly: false, Nullable: false, Expected: max-unsigned-long)")]
        [TestCase("   ", true, null, TestName = "AsUInt64(Value: whitespace, Exactly: false, Nullable: true, Expected: null)")]
        [TestCase("420042004242004200", false, (UInt64)420042004242004200, TestName = "AsUInt64(Value: 420042004242004200, Exactly: false, Nullable: false, Expected: 420042004242004200)")]
        [TestCase("420042004242004200", true, (UInt64)420042004242004200, TestName = "AsUInt64(Value: 420042004242004200, Exactly: false, Nullable: true, Expected: 420042004242004200)")]
        [TestCase("4200420042420042004200", false, (UInt64)18446744073709551615, TestName = "AsUInt64(Value: 4200420042420042004200, Exactly: false, Nullable: false, Expected: max-unsigned-long)")]
        [TestCase("4200420042420042004200", true, (UInt64)18446744073709551615, TestName = "AsUInt64(Value: 4200420042420042004200, Exactly: false, Nullable: true, Expected: max-unsigned-long)")]
        [TestCase("<null>", true, null, TestName = "AsUInt64(Value: <null>, Exactly: false, Nullable: true, Expected: null)")]
        public void AsUInt64_VariousValuesNotExactly_ResultAsExpected(String value, Boolean nullable, Object expected)
        {
            if (nullable)
            {
                Assert.AreEqual((UInt64?)expected, (UInt64?)this.ExecutePrivateStaticMethod("AsUInt64", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((UInt64)expected, (UInt64)this.ExecutePrivateStaticMethod("AsUInt64", new Object[] { value, false, nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCase(null, TestName = "AsUInt64(Value: null, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("", TestName = "AsUInt64(Value: empty, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("   ", TestName = "AsUInt64(Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException)")]
        [TestCase("4200420042420042004200", TestName = "AsUInt64(Value: 4200420042420042004200, Exactly: true, Nullable: false, Expected: FormatException)")]
        public void AsUInt64_VariousValuesExactly_ThrowsFormatException(String value)
        {
            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsUInt64", new Object[] { value, true, false, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCaseSource("PositiveDateTimeTestCases")]
        public void AsDateTime_ValuesNotExactly_ResultAsExpected(Object data)
        {
            TestCaseItem item = (TestCaseItem)data;

            if (item.Nullable)
            {
                Assert.AreEqual((DateTime?)item.Expected, (DateTime?)this.ExecutePrivateStaticMethod("AsDateTime", new Object[] { item.Value, false, item.Nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((DateTime)item.Expected, (DateTime)this.ExecutePrivateStaticMethod("AsDateTime", new Object[] { item.Value, false, item.Nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCaseSource("NegativeDateTimeTestCases")]
        public void AsDateTime_ValuesExactly_ThrowsFormatException(Object data)
        {
            TestCaseItem item = (TestCaseItem)data;

            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsDateTime", new Object[] { item.Value, item.Exactly, item.Nullable, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCaseSource("PositiveDecimalTestCases")]
        public void AsDecimal_ValuesNotExactly_ResultAsExpected(Object data)
        {
            TestCaseItem item = (TestCaseItem)data;

            if (item.Nullable)
            {
                Assert.AreEqual((Decimal?)item.Expected, (Decimal?)this.ExecutePrivateStaticMethod("AsDecimal", new Object[] { item.Value, false, item.Nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((Decimal)item.Expected, (Decimal)this.ExecutePrivateStaticMethod("AsDecimal", new Object[] { item.Value, false, item.Nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCaseSource("NegativeDecimalTestCases")]
        public void AsDecimal_ValuesExactly_ThrowsFormatException(Object data)
        {
            TestCaseItem item = (TestCaseItem)data;

            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsDecimal", new Object[] { item.Value, item.Exactly, item.Nullable, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCaseSource("PositiveDoubleTestCases")]
        public void AsDouble_ValuesNotExactly_ResultAsExpected(Object data)
        {
            TestCaseItem item = (TestCaseItem)data;

            if (item.Nullable)
            {
                Assert.AreEqual((Double?)item.Expected, (Double?)this.ExecutePrivateStaticMethod("AsDouble", new Object[] { item.Value, false, item.Nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((Double)item.Expected, (Double)this.ExecutePrivateStaticMethod("AsDouble", new Object[] { item.Value, false, item.Nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCaseSource("NegativeDoubleTestCases")]
        public void AsDouble_ValuesExactly_ThrowsFormatException(Object data)
        {
            TestCaseItem item = (TestCaseItem)data;

            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsDouble", new Object[] { item.Value, item.Exactly, item.Nullable, CultureInfo.CurrentUICulture, null }); });
        }

        [Test]
        [TestCaseSource("PositiveSingleTestCases")]
        public void AsSingle_ValuesNotExactly_ResultAsExpected(Object data)
        {
            TestCaseItem item = (TestCaseItem)data;

            if (item.Nullable)
            {
                Assert.AreEqual((Single?)item.Expected, (Single?)this.ExecutePrivateStaticMethod("AsSingle", new Object[] { item.Value, false, item.Nullable, CultureInfo.CurrentUICulture, null }));
            }
            else
            {
                Assert.AreEqual((Single)item.Expected, (Single)this.ExecutePrivateStaticMethod("AsSingle", new Object[] { item.Value, false, item.Nullable, CultureInfo.CurrentUICulture, null }));
            }
        }

        [Test]
        [TestCaseSource("NegativeSingleTestCases")]
        public void AsSingle_ValuesExactly_ThrowsFormatException(Object data)
        {
            TestCaseItem item = (TestCaseItem)data;

            Assert.Throws<FormatException>(() => { this.ExecutePrivateStaticMethod("AsSingle", new Object[] { item.Value, item.Exactly, item.Nullable, CultureInfo.CurrentUICulture, null }); });
        }

        private class TestCaseItem
        {
            public String TestName { get; set; }
            public String Value { get; set; }
            public Boolean Exactly { get; set; }
            public Boolean Nullable { get; set; }
            public Object Expected { get; set; }
            public override String ToString()
            {
                if (String.IsNullOrWhiteSpace(this.TestName))
                {
                    throw new ArgumentNullException("The Test Name must be valid.");
                }

                return this.TestName;
            }
        }

        private static readonly Object[] PositiveDateTimeTestCases = new TestCaseItem[]
        {
            new TestCaseItem {
                TestName = "Value: null, Exactly: false, Nullable: false, Expected: max-date-time",
                Value    = null,
                Exactly  = false,
                Nullable = false,
                Expected = DateTime.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: null, Exactly: false, Nullable: true, Expected: null",
                Value    = null,
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: false, Nullable: false, Expected: max-date-time",
                Value    = "",
                Exactly  = false,
                Nullable = false,
                Expected = DateTime.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: false, Nullable: true, Expected: null",
                Value    = "",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: false, Nullable: false, Expected: max-date-time",
                Value    = "   ",
                Exactly  = false,
                Nullable = false,
                Expected = DateTime.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: false, Nullable: true, Expected: null",
                Value    = "   ",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: 29.10.1967 17:05:42, Exactly: false, Nullable: false, Expected: 29.10.1967 17:05:42",
                Value    = "29.10.1967 17:05:42",
                Exactly  = false,
                Nullable = false,
                Expected = new DateTime(1967, 10, 29, 17, 05, 42),
            }, new TestCaseItem {
                TestName = "Value: 29.10.1967 17:05:42, Exactly: false, Nullable: true, Expected: 29.10.1967 17:05:42",
                Value    = "29.10.1967 17:05:42",
                Exactly  = false,
                Nullable = true,
                Expected = new DateTime(1967, 10, 29, 17, 05, 42),
            }, new TestCaseItem {
                TestName = "Value: 31.12.10000 23:59:59, Exactly: false, Nullable: false, Expected: max-date-time",
                Value    = "31.12.10000 23:59:59",
                Exactly  = false,
                Nullable = false,
                Expected = DateTime.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: 31.12.10000 23:59:59, Exactly: false, Nullable: true, Expected: max-date-time",
                Value    = "31.12.10000 23:59:59",
                Exactly  = false,
                Nullable = true,
                Expected = DateTime.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: <null>, Exactly: false, Nullable: true, Expected: null",
                Value    = "<null>",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }
        };

        private static readonly Object[] NegativeDateTimeTestCases = new TestCaseItem[]
        {
            new TestCaseItem {
                TestName = "Value: null, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = null,
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "   ",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: 31.12.10000 23:59:59, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "31.12.10000 23:59:59",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }
        };

        private static readonly Object[] PositiveDecimalTestCases = new TestCaseItem[]
        {
            new TestCaseItem {
                TestName = "Value: null, Exactly: false, Nullable: false, Expected: max-decimal",
                Value    = null,
                Exactly  = false,
                Nullable = false,
                Expected = Decimal.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: null, Exactly: false, Nullable: true, Expected: null",
                Value    = null,
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: false, Nullable: false, Expected: max-decimal",
                Value    = "",
                Exactly  = false,
                Nullable = false,
                Expected = Decimal.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: false, Nullable: true, Expected: null",
                Value    = "",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: false, Nullable: false, Expected: max-decimal",
                Value    = "   ",
                Exactly  = false,
                Nullable = false,
                Expected = Decimal.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: false, Nullable: true, Expected: null",
                Value    = "   ",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: 4200420042004200420042004200, Exactly: false, Nullable: false, Expected: 4200420042004200420042004200",
                Value    = "4200420042004200420042004200",
                Exactly  = false,
                Nullable = false,
                Expected = 4200420042004200420042004200m,
            }, new TestCaseItem {
                TestName = "Value: 4200420042004200420042004200, Exactly: false, Nullable: true, Expected: 4200420042004200420042004200",
                Value    = "4200420042004200420042004200",
                Exactly  = false,
                Nullable = true,
                Expected = 4200420042004200420042004200m,
            }, new TestCaseItem {
                TestName = "Value: -4200420042004200420042004200, Exactly: false, Nullable: false, Expected: -4200420042004200420042004200",
                Value    = "-4200420042004200420042004200",
                Exactly  = false,
                Nullable = false,
                Expected = -4200420042004200420042004200m,
            }, new TestCaseItem {
                TestName = "Value: -4200420042004200420042004200, Exactly: false, Nullable: true, Expected: -4200420042004200420042004200",
                Value    = "-4200420042004200420042004200",
                Exactly  = false,
                Nullable = true,
                Expected = -4200420042004200420042004200m,
            }, new TestCaseItem {
                TestName = "Value: 42004200420042004200420042004200, Exactly: false, Nullable: false, Expected: max-decimal",
                Value    = "42004200420042004200420042004200",
                Exactly  = false,
                Nullable = false,
                Expected = Decimal.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: 42004200420042004200420042004200, Exactly: false, Nullable: true, Expected: max-decimal",
                Value    = "42004200420042004200420042004200",
                Exactly  = false,
                Nullable = true,
                Expected = Decimal.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: <null>, Exactly: false, Nullable: true, Expected: null",
                Value    = "<null>",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }
        };

        private static readonly Object[] NegativeDecimalTestCases = new TestCaseItem[]
        {
            new TestCaseItem {
                TestName = "Value: null, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = null,
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "   ",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: 42004200420042004200420042004200, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "42004200420042004200420042004200",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }
        };

        private static readonly Object[] PositiveDoubleTestCases = new TestCaseItem[]
        {
            new TestCaseItem {
                TestName = "Value: null, Exactly: false, Nullable: false, Expected: max-double",
                Value    = null,
                Exactly  = false,
                Nullable = false,
                Expected = Double.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: null, Exactly: false, Nullable: true, Expected: null",
                Value    = null,
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: false, Nullable: false, Expected: max-double",
                Value    = "",
                Exactly  = false,
                Nullable = false,
                Expected = Double.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: false, Nullable: true, Expected: null",
                Value    = "",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: false, Nullable: false, Expected: max-double",
                Value    = "   ",
                Exactly  = false,
                Nullable = false,
                Expected = Double.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: false, Nullable: true, Expected: null",
                Value    = "   ",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: 123.456,789, Exactly: false, Nullable: false, Expected: 123.456,789",
                Value    = "123.456,789",
                Exactly  = false,
                Nullable = false,
                Expected = 123456.789d,
            }, new TestCaseItem {
                TestName = "Value: 123.456,789, Exactly: false, Nullable: true, Expected: 123.456,789",
                Value    = "123.456,789",
                Exactly  = false,
                Nullable = true,
                Expected = 123456.789d,
            }, new TestCaseItem {
                TestName = "Value: -123.456,789, Exactly: false, Nullable: false, Expected: -123.456,789",
                Value    = "-123.456,789",
                Exactly  = false,
                Nullable = false,
                Expected = -123456.789d,
            }, new TestCaseItem {
                TestName = "Value: -123.456,789, Exactly: false, Nullable: true, Expected: -123.456,789",
                Value    = "-123.456,789",
                Exactly  = false,
                Nullable = true,
                Expected = -123456.789d,
            }, new TestCaseItem {
                TestName = "Value: 1.8E+310, Exactly: false, Nullable: false, Expected: max-double",
                Value    = "1.8E+310",
                Exactly  = false,
                Nullable = false,
                Expected = Double.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: 1.8E+310, Exactly: false, Nullable: true, Expected: max-double",
                Value    = "1.8E+310",
                Exactly  = false,
                Nullable = true,
                Expected = Double.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: <null>, Exactly: false, Nullable: true, Expected: null",
                Value    = "<null>",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }
        };

        private static readonly Object[] NegativeDoubleTestCases = new TestCaseItem[]
        {
            new TestCaseItem {
                TestName = "Value: null, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = null,
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "   ",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: 1.8E+310, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "1.8E+310",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }
        };

        private static readonly Object[] PositiveSingleTestCases = new TestCaseItem[]
        {
            new TestCaseItem {
                TestName = "Value: null, Exactly: false, Nullable: false, Expected: max-single",
                Value    = null,
                Exactly  = false,
                Nullable = false,
                Expected = Single.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: null, Exactly: false, Nullable: true, Expected: null",
                Value    = null,
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: false, Nullable: false, Expected: max-single",
                Value    = "",
                Exactly  = false,
                Nullable = false,
                Expected = Single.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: false, Nullable: true, Expected: null",
                Value    = "",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: false, Nullable: false, Expected: max-single",
                Value    = "   ",
                Exactly  = false,
                Nullable = false,
                Expected = Single.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: false, Nullable: true, Expected: null",
                Value    = "   ",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: 123.456,789, Exactly: false, Nullable: false, Expected: 123.456,789",
                Value    = "123.456,789",
                Exactly  = false,
                Nullable = false,
                Expected = 123456.789f,
            }, new TestCaseItem {
                TestName = "Value: 123.456,789, Exactly: false, Nullable: true, Expected: 123.456,789",
                Value    = "123.456,789",
                Exactly  = false,
                Nullable = true,
                Expected = 123456.789f,
            }, new TestCaseItem {
                TestName = "Value: -123.456,789, Exactly: false, Nullable: false, Expected: -123.456,789",
                Value    = "-123.456,789",
                Exactly  = false,
                Nullable = false,
                Expected = -123456.789f,
            }, new TestCaseItem {
                TestName = "Value: -123.456,789, Exactly: false, Nullable: true, Expected: -123.456,789",
                Value    = "-123.456,789",
                Exactly  = false,
                Nullable = true,
                Expected = -123456.789f,
            }, new TestCaseItem {
                TestName = "Value: 3.5E+40, Exactly: false, Nullable: false, Expected: max-single",
                Value    = "3.5E+40",
                Exactly  = false,
                Nullable = false,
                Expected = Single.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: 3.5E+40, Exactly: false, Nullable: true, Expected: max-single",
                Value    = "3.5E+40",
                Exactly  = false,
                Nullable = true,
                Expected = Single.MaxValue,
            }, new TestCaseItem {
                TestName = "Value: <null>, Exactly: false, Nullable: true, Expected: null",
                Value    = "<null>",
                Exactly  = false,
                Nullable = true,
                Expected = null,
            }
        };

        private static readonly Object[] NegativeSingleTestCases = new TestCaseItem[]
        {
            new TestCaseItem {
                TestName = "Value: null, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = null,
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: empty, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: whitespace, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "   ",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }, new TestCaseItem {
                TestName = "Value: 3.5E+40, Exactly: true, Nullable: false, Expected: FormatException",
                Value    = "3.5E+40",
                Exactly  = true,
                Nullable = false,
                Expected = null,
            }
        };

        private class TestCaseMapping
        {
            public String TestName { get; set; }
            public String Value { get; set; }
            public CsvMappings Mapping { get; set; }
            public Object Expected { get; set; }
            public override String ToString()
            {
                if (String.IsNullOrWhiteSpace(this.TestName))
                {
                    throw new ArgumentNullException("The Test Name must be valid.");
                }

                return this.TestName;
            }
        }

        private static readonly Object[] GetTrueMappingsTestCases = new TestCaseMapping[]
        {
            new TestCaseMapping {
                TestName = "Mapping: null, Expected: default-true-values",
                Value    = null,
                Mapping  = null,
                Expected = String.Join("", CsvMappings.DefaultTrueValues),
            }, new TestCaseMapping {
                TestName = "Mapping: items null, Expected: default-true-values",
                Value    = null,
                Mapping  = new CsvMappings() { TrueValues = null },
                Expected = String.Join("", CsvMappings.DefaultTrueValues),
            }, new TestCaseMapping {
                TestName = "Mapping: items empty, Expected: default-true-values",
                Value    = null,
                Mapping  = new CsvMappings() { TrueValues = new List<String>() },
                Expected = String.Join("", CsvMappings.DefaultTrueValues),
            }, new TestCaseMapping {
                TestName = "Mapping: items valid, Expected: property-true-values",
                Value    = null,
                Mapping  = new CsvMappings() { TrueValues = new List<String>() { "yes", "yepp" } },
                Expected = "yesyepp",
            }
        };

        private static readonly Object[] GetFalseMappingsTestCases = new TestCaseMapping[]
        {
            new TestCaseMapping {
                TestName = "Mapping: null, Expected: default-false-values",
                Value    = null,
                Mapping  = null,
                Expected = String.Join("", CsvMappings.DefaultFalseValues),
            }, new TestCaseMapping {
                TestName = "Mapping: items null, Expected: default-false-values",
                Value    = null,
                Mapping  = new CsvMappings() { FalseValues = null },
                Expected = String.Join("", CsvMappings.DefaultFalseValues),
            }, new TestCaseMapping {
                TestName = "Mapping: items empty, Expected: default-false-values",
                Value    = null,
                Mapping  = new CsvMappings() { FalseValues = new List<String>() },
                Expected = String.Join("", CsvMappings.DefaultFalseValues),
            }, new TestCaseMapping {
                TestName = "Mapping: items valid, Expected: property-false-values",
                Value    = null,
                Mapping  = new CsvMappings() { FalseValues = new List<String>() { "no", "nope" } },
                Expected = "nonope",
            }
        };

        private static readonly Object[] GetNullMappingsTestCases = new TestCaseMapping[]
        {
            new TestCaseMapping {
                TestName = "Mapping: null, Expected: default-null-values",
                Value    = null,
                Mapping  = null,
                Expected = String.Join("", CsvMappings.DefaultNullValues),
            }, new TestCaseMapping {
                TestName = "Mapping: items null, Expected: default-null-values",
                Value    = null,
                Mapping  = new CsvMappings() { NullValues = null },
                Expected = String.Join("", CsvMappings.DefaultNullValues),
            }, new TestCaseMapping {
                TestName = "Mapping: items empty, Expected: default-null-values",
                Value    = null,
                Mapping  = new CsvMappings() { NullValues = new List<String>() },
                Expected = String.Join("", CsvMappings.DefaultNullValues),
            }, new TestCaseMapping {
                TestName = "Mapping: items valid, Expected: property-null-values",
                Value    = null,
                Mapping  = new CsvMappings() { NullValues = new List<String>() { "null", "0" } },
                Expected = "null0",
            }
        };

        private static readonly Object[] IsTrueStringTestCases = new TestCaseMapping[]
        {
            new TestCaseMapping {
                TestName = "Value: null, Mapping: default, Expected: false",
                Value    = null,
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: empty, Mapping: default, Expected: false",
                Value    = "",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: whitespace, Mapping: default, Expected: false",
                Value    = "   ",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: FALSE, Mapping: default, Expected: false",
                Value    = "FALSE",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: TRUE, Mapping: default, Expected: true",
                Value    = "TRUE",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = true,
            }, new TestCaseMapping {
                TestName = "Value: 1, Mapping: default, Expected: true",
                Value    = "1",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = true,
            }
        };

        private static readonly Object[] IsFalseStringTestCases = new TestCaseMapping[]
        {
            new TestCaseMapping {
                TestName = "Value: null, Mapping: default, Expected: false",
                Value    = null,
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: empty, Mapping: default, Expected: false",
                Value    = "",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: whitespace, Mapping: default, Expected: false",
                Value    = "   ",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: TRUE, Mapping: default, Expected: false",
                Value    = "TRUE",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: FALSE, Mapping: default, Expected: true",
                Value    = "FALSE",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = true,
            }, new TestCaseMapping {
                TestName = "Value: 0, Mapping: default, Expected: true",
                Value    = "0",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = true,
            }
        };

        private static readonly Object[] IsNullStringTestCases = new TestCaseMapping[]
        {
            new TestCaseMapping {
                TestName = "Value: null, Mapping: default, Expected: false",
                Value    = null,
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: empty, Mapping: default, Expected: false",
                Value    = "",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: whitespace, Mapping: default, Expected: false",
                Value    = "   ",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: TEST, Mapping: default, Expected: false",
                Value    = "TEST",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = false,
            }, new TestCaseMapping {
                TestName = "Value: <NULL>, Mapping: default, Expected: true",
                Value    = "<NULL>",
                Mapping  = CsvMappings.DefaultMappings,
                Expected = true,
            }
        };

        private Object ExecutePrivateStaticMethod(String name, Object[] parameters)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                Assert.Fail($"Method name {name} cannot be null or whitespace.");
            }

            var method = typeof(TypeConverter).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
            {
                Assert.Fail($"Method for name {name} couldn't be found.");
            }

            try
            {
                return method.Invoke(obj: null, parameters: parameters);
            }
            catch (TargetInvocationException exception)
            {
                if (exception.InnerException != null)
                {
                    throw exception.InnerException;
                }
                else
                {
                    throw exception;
                }
            }
        }
    }
}

