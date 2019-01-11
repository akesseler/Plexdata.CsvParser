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
using Plexdata.CsvParser.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plexdata.CsvParser.Tests.Helpers
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
