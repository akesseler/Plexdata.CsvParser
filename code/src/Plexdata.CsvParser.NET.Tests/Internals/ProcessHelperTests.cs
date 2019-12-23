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
using System.Text;

namespace Plexdata.CsvParser.Tests.Internals
{
    [TestFixture]
    [TestOf(nameof(ProcessHelper))]
    public class ProcessHelperTests
    {
        [Test]
        [TestCaseSource(nameof(SplitIntoCellsTestCases))]
        public void SplitIntoCells_VariousValueCombinations_ResultAsExpected(Object data)
        {
            TestCaseItem item = (TestCaseItem)data;

            List<String> actual = ProcessHelper.SplitIntoCells(item.Value, item.Separator);

            Assert.That(String.Join(String.Empty, actual), Is.EqualTo(String.Join(String.Empty, item.Expected)));
        }

        [Test]
        [TestCase("Quoting on", true)]
        [TestCase(42, false)]
        public void ConvertToString_QuotingIsTrueForStrings_ResultAsExpected(Object value, Boolean expected)
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            CsvMappings mapping = new CsvMappings();

            ProcessHelper.ConvertToString(value, culture, mapping, out Boolean actual);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(null, ':', false, ":")]
        [TestCase(null, ':', true, "\"\":")]
        [TestCase("\"", ':', false, "\"\"\"\":")]
        [TestCase("\"", ':', true, "\"\"\"\":")]
        [TestCase(":", ':', false, "\":\":")]
        [TestCase(":", ':', true, "\":\":")]
        [TestCase(":\"", ':', false, "\":\"\"\":")]
        [TestCase(":\"", ':', true, "\":\"\"\":")]
        [TestCase("\r", ':', false, "\"\r\":")]
        [TestCase("\r", ':', true, "\"\r\":")]
        [TestCase("\r\"", ':', false, "\"\r\"\"\":")]
        [TestCase("\r\"", ':', true, "\"\r\"\"\":")]
        [TestCase("\n", ':', false, "\"\n\":")]
        [TestCase("\n", ':', true, "\"\n\":")]
        [TestCase("\n\"", ':', false, "\"\n\"\"\":")]
        [TestCase("\n\"", ':', true, "\"\n\"\"\":")]
        [TestCase("\r\n", ':', false, "\"\r\n\":")]
        [TestCase("\r\n", ':', true, "\"\r\n\":")]
        [TestCase("\r\n\"", ':', false, "\"\r\n\"\"\":")]
        [TestCase("\r\n\"", ':', true, "\"\r\n\"\"\":")]
        [TestCase("\n:", ':', false, "\"\n:\":")]
        [TestCase("\n:", ':', true, "\"\n:\":")]
        [TestCase("\n\":", ':', false, "\"\n\"\":\":")]
        [TestCase("\n\":", ':', true, "\"\n\"\":\":")]
        [TestCase("\r\n:", ':', false, "\"\r\n:\":")]
        [TestCase("\r\n:", ':', true, "\"\r\n:\":")]
        [TestCase("\r\n\":", ':', false, "\"\r\n\"\":\":")]
        [TestCase("\r\n\":", ':', true, "\"\r\n\"\":\":")]
        [TestCase("\"quote at front", ':', false, "\"\"\"quote at front\":")]
        [TestCase("\"quote at front", ':', true, "\"\"\"quote at front\":")]
        [TestCase("quote at end\"", ':', false, "\"quote at end\"\"\":")]
        [TestCase("quote at end\"", ':', true, "\"quote at end\"\"\":")]
        [TestCase("quote in\"middle", ':', false, "\"quote in\"\"middle\":")]
        [TestCase("quote in\"middle", ':', true, "\"quote in\"\"middle\":")]
        [TestCase("nothing to quote", ':', false, "nothing to quote:")]
        [TestCase("nothing to quote", ':', true, "\"nothing to quote\":")]
        public void ConvertToOutput_ValueAutoQuoting_ResultAsExpected(String value, Char separator, Boolean quoting, String expected)
        {
            Assert.That(ProcessHelper.ConvertToOutput(value, separator, quoting), Is.EqualTo(expected));
        }

        [Test]
        public void FixupOutput_StringBuilderIsNull_ResultIsNull()
        {
            Assert.That(ProcessHelper.FixupOutput(null, '#'), Is.Null);
        }

        [Test]
        public void FixupOutput_StringBuilderIsEmpty_ResultIsEmpty()
        {
            Assert.That(ProcessHelper.FixupOutput(new StringBuilder(), '#').Length, Is.EqualTo(0));
        }

        [Test]
        [TestCase("some data", '#', "some data")]
        [TestCase("some data#", '#', "some data")]
        [TestCase("some#data", '#', "some#data")]
        [TestCase("some#data#", '#', "some#data")]
        public void FixupOutput_StringBuilderWithSomeData_ResultAsExpected(String value, Char separator, String expected)
        {
            Assert.That(ProcessHelper.FixupOutput(new StringBuilder(value), separator).ToString(), Is.EqualTo(expected));
        }

        #region Test helper classes

        private class TestCaseItem
        {
            public String TestName
            {
                get
                {
                    StringBuilder result = new StringBuilder(128);

                    #region Value

                    result.Append($"{nameof(this.Value)}: ");

                    if (this.Value == null)
                    {
                        result.Append("<null>");
                    }
                    else if (this.Value == String.Empty)
                    {
                        result.Append("<empty>");
                    }
                    else if (String.IsNullOrWhiteSpace(this.Value))
                    {
                        result.Append("<whitespace>");
                    }
                    else
                    {
                        result.Append($"{this.Value}");
                    }
                    result.Append(", ");

                    #endregion

                    #region Separator

                    result.Append($"{nameof(this.Separator)}: ");

                    if (Char.IsControl(this.Separator))
                    {
                        result.Append($"\\{this.Separator}");
                    }
                    else
                    {
                        result.Append($"{this.Separator}");
                    }

                    result.Append(", ");

                    #endregion

                    #region Expected

                    result.Append($"{nameof(this.Expected)}: ");

                    if (this.Expected == null)
                    {
                        result.Append("[<null>]");
                    }
                    else if (this.Expected.Count == 0)
                    {
                        result.Append("[<empty>]");
                    }
                    else
                    {
                        result.Append($"[{String.Join(", ", this.Expected)}]");
                    }

                    #endregion

                    return result.ToString();
                }

            }
            public String Value { get; set; }
            public Char Separator { get; set; }
            public List<String> Expected { get; set; }
            public override String ToString()
            {
                return this.TestName;
            }
        }

        private static readonly Object[] SplitIntoCellsTestCases = new TestCaseItem[]
        {
            new TestCaseItem
            {
                Value = null,
                Separator = ':',
                Expected = new List<String>(),
            }, new TestCaseItem
            {
                Value = "",
                Separator = ':',
                Expected = new List<String>(),
            }, new TestCaseItem
            {
                Value = "   ",
                Separator = ':',
                Expected = new List<String>(),
            }, new TestCaseItem
            {
                Value = "one:two",
                Separator = ':',
                Expected = new List<String>() { "one", "two"},
            }, new TestCaseItem
            {
                Value = "\"one\":\"two\"",
                Separator = ':',
                Expected = new List<String>() { "one", "two"},
            }, new TestCaseItem
            {
                Value = "\"one\"\"\":\"\"\"two\"",
                Separator = ':',
                Expected = new List<String>() { "one\"", "\"two"},
            }, new TestCaseItem
            {
                Value = "\"one\"\"two\"",
                Separator = ':',
                Expected = new List<String>() { "one\"two"},
            }, new TestCaseItem
            {
                Value = "\"one\\\"two\"",
                Separator = ':',
                Expected = new List<String>() { "one\"two"},
            }, new TestCaseItem
            {
                Value = "\"one:\":\":two\"",
                Separator = ':',
                Expected = new List<String>() { "one:", ":two"},
            }, new TestCaseItem
            {
                Value = "\"one\\\"\":\"\\\"two\"",
                Separator = ':',
                Expected = new List<String>() { "one\"", "\"two"},
            },
        };

        #endregion
    }
}
