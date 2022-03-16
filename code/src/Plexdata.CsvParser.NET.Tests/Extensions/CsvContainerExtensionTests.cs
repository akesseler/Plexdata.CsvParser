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

using NUnit.Framework;
using Plexdata.CsvParser.Definitions;
using Plexdata.CsvParser.Extensions;
using Plexdata.CsvParser.Processors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plexdata.CsvParser.Tests.Extensions
{
    [TestFixture]
    [TestOf(nameof(CsvContainerExtension))]
    [Category(TestHelper.IntegrationTest)]
    public class CsvContainerExtensionTests
    {
        [Test]
        public void Serialize_CsvContainerIsNull_ResultIsEmpty()
        {
            CsvContainer container = null;

            Assert.That(container.Serialize(), Is.Empty);
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(0, 1)]
        public void Serialize_CsvContainerIsEmpty_ResultIsEmpty(Int32 width, Int32 length)
        {
            CsvContainer container = new CsvContainer(width, length);

            Assert.That(container.Serialize(), Is.Empty);
        }

        [Test]
        public void Serialize_CsvSettingsIsNull_ResultIsEmpty()
        {
            CsvSettings settings = null;
            CsvContainer container = new CsvContainer(1, 1);

            Assert.That(container.Serialize(settings), Is.Empty);
        }

        [Test]
        public void Serialize_ParametersValid_ResultAsExpected()
        {
            String expected = "﻿HA,HB,HC,\r\n11,12,13,\r\n21,22,23,24\r\n31,32,,\r\n";

            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC"       },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvSettings settings = new CsvSettings();
            CsvContainer container = new CsvContainer(content);

            String actual = container.Serialize(settings);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Sort_HeaderOrderButContainerIsNull_ThrowsNothing()
        {
            CsvContainer container = null;

            Assert.That(() => container.Sort("header", SortOrder.Ascending), Throws.Nothing);
        }

        [Test]
        public void Sort_HeaderOrderButHeaderIsInvalid_ThrowsNothing([Values(null, "", " ", "invalid")] String header)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC"       },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvSettings settings = new CsvSettings() { Heading = true };
            CsvContainer container = new CsvContainer(content, settings);

            Assert.That(() => container.Sort(header, SortOrder.Ascending), Throws.Nothing);
        }

        [Test]
        public void Sort_ColumnOrderButContainerIsNull_ThrowsNothing()
        {
            CsvContainer container = null;

            Assert.That(() => container.Sort(0, SortOrder.Ascending), Throws.Nothing);
        }

        [Test]
        public void Sort_ColumnOrderButColumnIsInvalid_ThrowsNothing([Values(-1, 4)] Int32 column)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC"       },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer container = new CsvContainer(content);

            Assert.That(() => container.Sort(column, SortOrder.Ascending), Throws.Nothing);
        }

        [TestCase(0, true, SortOrder.Ascending, "HA|HB|HC||#11|12|13||#21|22|23|24|#31|32|||#")]
        [TestCase(0, false, SortOrder.Ascending, "11|12|13||#21|22|23|24|#31|32|||#HA|HB|HC||#")]
        [TestCase(0, true, SortOrder.Descending, "HA|HB|HC||#31|32|||#21|22|23|24|#11|12|13||#")]
        [TestCase(0, false, SortOrder.Descending, "HA|HB|HC||#31|32|||#21|22|23|24|#11|12|13||#")]
        [TestCase(0, true, SortOrder.Unsorted, "HA|HB|HC||#11|12|13||#21|22|23|24|#31|32|||#")]
        [TestCase(0, false, SortOrder.Unsorted, "HA|HB|HC||#11|12|13||#21|22|23|24|#31|32|||#")]
        [TestCase(1, true, SortOrder.Ascending, "HA|HB|HC||#11|12|13||#21|22|23|24|#31|32|||#")]
        [TestCase(1, false, SortOrder.Ascending, "11|12|13||#21|22|23|24|#31|32|||#HA|HB|HC||#")]
        [TestCase(1, true, SortOrder.Descending, "HA|HB|HC||#31|32|||#21|22|23|24|#11|12|13||#")]
        [TestCase(1, false, SortOrder.Descending, "HA|HB|HC||#31|32|||#21|22|23|24|#11|12|13||#")]
        [TestCase(1, true, SortOrder.Unsorted, "HA|HB|HC||#11|12|13||#21|22|23|24|#31|32|||#")]
        [TestCase(1, false, SortOrder.Unsorted, "HA|HB|HC||#11|12|13||#21|22|23|24|#31|32|||#")]
        [TestCase(2, true, SortOrder.Ascending, "HA|HB|HC||#31|32|||#11|12|13||#21|22|23|24|#")]
        [TestCase(2, false, SortOrder.Ascending, "31|32|||#11|12|13||#21|22|23|24|#HA|HB|HC||#")]
        [TestCase(2, true, SortOrder.Descending, "HA|HB|HC||#21|22|23|24|#11|12|13||#31|32|||#")]
        [TestCase(2, false, SortOrder.Descending, "HA|HB|HC||#21|22|23|24|#11|12|13||#31|32|||#")]
        [TestCase(2, true, SortOrder.Unsorted, "HA|HB|HC||#11|12|13||#21|22|23|24|#31|32|||#")]
        [TestCase(2, false, SortOrder.Unsorted, "HA|HB|HC||#11|12|13||#21|22|23|24|#31|32|||#")]
        [TestCase(3, true, SortOrder.Ascending, "HA|HB|HC||#11|12|13||#31|32|||#21|22|23|24|#")]
        [TestCase(3, false, SortOrder.Ascending, "HA|HB|HC||#11|12|13||#31|32|||#21|22|23|24|#")]
        [TestCase(3, true, SortOrder.Descending, "HA|HB|HC||#21|22|23|24|#11|12|13||#31|32|||#")]
        [TestCase(3, false, SortOrder.Descending, "21|22|23|24|#HA|HB|HC||#11|12|13||#31|32|||#")]
        [TestCase(3, true, SortOrder.Unsorted, "HA|HB|HC||#11|12|13||#21|22|23|24|#31|32|||#")]
        [TestCase(3, false, SortOrder.Unsorted, "HA|HB|HC||#11|12|13||#21|22|23|24|#31|32|||#")]
        public void Sort_DifferentParameters_ResultAsExpected(Int32 column, Boolean heading, SortOrder order, String expected)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC"       },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvSettings settings = new CsvSettings() { Heading = heading };
            CsvContainer container = new CsvContainer(content, settings);

            container.Sort(column, order);

            Assert.That(this.GetActualContent(container), Is.EqualTo(expected));
        }

        private String GetActualContent(CsvContainer container)
        {
            StringBuilder builder = new StringBuilder();

            Int32 cols = container.Width;
            Int32 rows = container.Length;

            for (Int32 row = 0; row < rows; row++)
            {
                for (Int32 col = 0; col < cols; col++)
                {
                    builder.Append($"{container[col, row]}|");
                }

                builder.Append("#");
            }

            return builder.ToString();
        }
    }
}
