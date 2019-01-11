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
using Plexdata.CsvParser.Processors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Plexdata.CsvParser.Tests.Processors
{
    [TestFixture]
    [TestOf(nameof(CsvContainer))]
    public class CsvContainerTests
    {
        [Test]
        public void CsvContainer_DefaultConstructor_ResultIsDefaultSettings()
        {
            CsvContainer instance = new CsvContainer();

            Assert.That(instance.Content.Count, Is.EqualTo(0));
            Assert.That(instance.Compare, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
            Assert.That(instance.Culture, Is.EqualTo(CultureInfo.CurrentUICulture));
            Assert.That(instance.Mappings, Is.InstanceOf<CsvMappings>());
            Assert.That(instance.Heading, Is.EqualTo(false));
            Assert.That(instance.Exactly, Is.EqualTo(false));
        }

        [Test]
        public void CsvContainer_ParameterConstructor_ResultIsExpectedListContent()
        {
            List<List<String>> expected = new List<List<String>>()
            {
                new List<String>() { "11", "21", "31" },
                new List<String>() { "12", "22", "32" },
                new List<String>() { "13", "23", ""   },
                new List<String>() {  "" , "24", ""   },
            };

            CsvContainer instance = new CsvContainer(new List<List<String>>()
            {
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            });

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(this.GetJoinedContent(expected)));
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(4, 0)]
        [TestCase(0, 3)]
        [TestCase(-1, 3)]
        [TestCase(4, -1)]
        public void GetValue_InvalidColumnAndOrInvalidIndex_ResultIsNull(Int32 column, Int32 index)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);
            instance.Heading = false;

            Assert.That(instance.GetValue<Int32>(column, index), Is.EqualTo(null));
        }

        [Test]
        [TestCase(0, 0, 11)]
        [TestCase(0, 1, 21)]
        [TestCase(0, 2, 31)]
        [TestCase(1, 0, 12)]
        [TestCase(1, 1, 22)]
        [TestCase(1, 2, 32)]
        [TestCase(2, 0, 13)]
        [TestCase(2, 1, 23)]
        [TestCase(2, 2, null)]
        [TestCase(3, 0, null)]
        [TestCase(3, 1, 24)]
        [TestCase(3, 2, null)]
        public void GetValue_ColumnAndIndexAllTypesAreInt32_ResultAsExpected(Int32 column, Int32 index, Object expected)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);
            instance.Heading = false;

            Assert.That(instance.GetValue<Int32>(column, index), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("HA", 0, 11)]
        [TestCase("HA", 1, 21)]
        [TestCase("HA", 2, 31)]
        [TestCase("HB", 0, 12)]
        [TestCase("HB", 1, 22)]
        [TestCase("HB", 2, 32)]
        [TestCase("HC", 0, 13)]
        [TestCase("HC", 1, 23)]
        [TestCase("HC", 2, null)]
        public void GetValue_HeaderAndIndexAllTypesAreInt32_ResultAsExpected(String column, Int32 index, Object expected)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "HA", "HB", "HC"       },
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);
            instance.Heading = true;

            Assert.That(instance.GetValue<Int32>(column, index), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("HA", -1)]
        [TestCase("HB", -1)]
        [TestCase("HC", -1)]
        [TestCase("HA", 3)]
        [TestCase("HB", 3)]
        [TestCase("HC", 3)]
        [TestCase("HA", 4)]
        [TestCase("HB", 4)]
        [TestCase("HC", 4)]
        public void GetValue_InvalidHeaderAndOrInvalidIndex_ResultIsNull(String column, Int32 index)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "HA", "HB", "HC"       },
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);
            instance.Heading = true;

            Assert.That(instance.GetValue<Int32>(column, index), Is.EqualTo(null));
        }

        [Test]
        [TestCase(0, "11,21,31")]
        [TestCase(1, "12,22,32")]
        [TestCase(2, "13,23,")]
        [TestCase(3, ",24,")]
        public void ListAccessor_ColumnIsValid_ResultAsExpected(Int32 column, String expected)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);
            instance.Heading = false;

            IEnumerable<String> actual = instance[column];

            Assert.That(String.Join(",", actual), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(4)]
        public void ListAccessor_ColumnIsInvalid_ResultIsNull(Int32 column)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);
            instance.Heading = false;

            Assert.That(instance[column], Is.EqualTo(null));
        }

        [Test]
        [TestCase("HA", "HA,11,21,31")]
        [TestCase("HB", "HB,12,22,32")]
        [TestCase("HC", "HC,13,23,")]
        public void ListAccessor_HeaderIsValid_ResultAsExpected(String header, String expected)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "HA", "HB", "HC"       },
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);
            instance.Heading = true;

            IEnumerable<String> actual = instance[header];

            Assert.That(String.Join(",", actual), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("??")]
        public void ListAccessor_HeaderIsInvalid_ResultIsNull(String header)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "HA", "HB", "HC"       },
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);
            instance.Heading = true;

            Assert.That(instance[header], Is.EqualTo(null));
        }

        [Test]
        public void ListAccessor_HeadingIsFalse_ResultIsNull()
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "HA", "HB", "HC"       },
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);
            instance.Heading = false;

            Assert.That(instance["HB"], Is.EqualTo(null));
        }

        private String GetJoinedContent(List<List<String>> content)
        {
            StringBuilder result = new StringBuilder(512);

            if (content != null)
            {
                foreach (List<String> line in content)
                {
                    result.Append(String.Join(", ", line) + Environment.NewLine);
                }
            }

            return result.ToString();
        }
    }
}
