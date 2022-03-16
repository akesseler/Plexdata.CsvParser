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
            Assert.That(instance.Heading, Is.False);
            Assert.That(instance.Exactly, Is.False);
            Assert.That(instance.Width, Is.Zero);
            Assert.That(instance.Length, Is.Zero);
        }

        [Test]
        public void CsvContainer_SourceListIsNull_ResultContentIsEmpty()
        {
            List<List<String>> content = null;

            CsvContainer instance = new CsvContainer(content);

            Assert.That(instance.Content, Is.Empty);
        }

        [Test]
        public void CsvContainer_SourceListIsEmpty_ResultContentIsEmpty()
        {
            List<List<String>> content = new List<List<String>>();

            CsvContainer instance = new CsvContainer(content);

            Assert.That(instance.Content, Is.Empty);
        }

        [Test]
        public void CsvContainer_SourceListInnerItemsAreNull_ResultContentIsEmpty()
        {
            List<List<String>> content = new List<List<String>>()
            {
                null, null, null
            };

            CsvContainer instance = new CsvContainer(content);

            Assert.That(instance.Content, Is.Empty);
        }

        [Test]
        public void CsvContainer_SourceListInnerItemsAreEmpty_ResultContentIsEmpty()
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(), new List<String>(), new List<String>()
            };

            CsvContainer instance = new CsvContainer(content);

            Assert.That(instance.Content, Is.Empty);
        }

        [Test]
        public void CsvContainer_SourceListInnerItemsAreNullOrEmptyOrValid_ResultContentAsExpected()
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(),
                new List<String>() { "21" },
                null,
                new List<String>() { "41", "42" },
            };

            CsvContainer instance = new CsvContainer(content);

            Assert.That(instance.Width, Is.EqualTo(2));
            Assert.That(instance.Length, Is.EqualTo(4));

            Assert.That(instance[0, 0], Is.Null);
            Assert.That(instance[1, 0], Is.Null);

            Assert.That(instance[0, 1], Is.EqualTo("21"));
            Assert.That(instance[1, 1], Is.Null);

            Assert.That(instance[0, 2], Is.Null);
            Assert.That(instance[1, 2], Is.Null);

            Assert.That(instance[0, 3], Is.EqualTo("41"));
            Assert.That(instance[1, 3], Is.EqualTo("42"));
        }

        [Test]
        public void CsvContainer_SourceListIsValid_ResultIsExpectedListContent()
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
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            });

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(this.GetJoinedContent(expected)));
        }

        [Test]
        public void CsvContainer_SourceListIsValidAndSettingsParameterIsNull_RelevantSettingsParameterAsExpected()
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "11", "12" },
                new List<String>() { "21", "22" },
                new List<String>() { "31", "32" },
            };

            CsvSettings settings = null;

            CsvContainer instance = new CsvContainer(content, settings);

            Assert.That(instance.Culture, Is.EqualTo(CultureInfo.CurrentUICulture));
            Assert.That(instance.Mappings, Is.InstanceOf<CsvMappings>());
            Assert.That(instance.Heading, Is.False);
            Assert.That(instance.Exactly, Is.False);
        }

        [Test]
        public void CsvContainer_SourceListIsValidAndSettingsParameterIsValid_RelevantSettingsParameterAsExpected()
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "11", "12" },
                new List<String>() { "21", "22" },
                new List<String>() { "31", "32" },
            };

            CsvSettings settings = new CsvSettings()
            {
                Culture = CultureInfo.InvariantCulture,
                Mappings = new CsvMappings(),
                Heading = true,
                Exactly = true,
            };

            CsvContainer instance = new CsvContainer(content, settings);

            Assert.That(instance.Culture, Is.SameAs(settings.Culture));
            Assert.That(instance.Mappings, Is.SameAs(settings.Mappings));
            Assert.That(instance.Heading, Is.True);
            Assert.That(instance.Exactly, Is.True);
        }

        [TestCase(-1, 5, 0, 0)]
        [TestCase(3, -1, 0, 0)]
        [TestCase(0, 5, 0, 0)]
        [TestCase(3, 0, 0, 0)]
        [TestCase(3, 5, 3, 5)]
        public void CsvContainer_CombinationsOfWidthAndLength_ResultWidthAndLengthAsExpected(Int32 width, Int32 length, Int32 expectedWidth, Int32 expectedLength)
        {
            CsvContainer instance = new CsvContainer(width, length);

            Assert.That(instance.Width, Is.EqualTo(expectedWidth));
            Assert.That(instance.Length, Is.EqualTo(expectedLength));
        }

        [Test]
        public void CsvContainer_WidthAndLengthValidAndSettingsParameterIsNull_RelevantSettingsParameterAsExpected()
        {
            CsvSettings settings = null;

            CsvContainer instance = new CsvContainer(2, 3, settings);

            Assert.That(instance.Culture, Is.EqualTo(CultureInfo.CurrentUICulture));
            Assert.That(instance.Mappings, Is.InstanceOf<CsvMappings>());
            Assert.That(instance.Heading, Is.False);
            Assert.That(instance.Exactly, Is.False);
        }

        [Test]
        public void CsvContainer_WidthAndLengthValidAndSettingsParameterIsValid_RelevantSettingsParameterAsExpected()
        {
            CsvSettings settings = new CsvSettings()
            {
                Culture = CultureInfo.InvariantCulture,
                Mappings = new CsvMappings(),
                Heading = true,
                Exactly = true,
            };

            CsvContainer instance = new CsvContainer(2, 3, settings);

            Assert.That(instance.Culture, Is.SameAs(settings.Culture));
            Assert.That(instance.Mappings, Is.SameAs(settings.Mappings));
            Assert.That(instance.Heading, Is.True);
            Assert.That(instance.Exactly, Is.True);
        }

        [TestCase(-1)]
        [TestCase(4)]
        public void ListGetter_ColumnIsInvalid_ResultIsNull(Int32 column)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);

            Assert.That(instance[column], Is.Null);
        }

        [TestCase(0, "11,21,31")]
        [TestCase(1, "12,22,32")]
        [TestCase(2, "13,23,")]
        [TestCase(3, ",24,")]
        public void ListGetter_ColumnIsValid_ResultAsExpected(Int32 column, String expected)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);

            IEnumerable<String> actual = instance[column];

            Assert.That(String.Join(",", actual), Is.EqualTo(expected));
        }

        [TestCase(-1)]
        [TestCase(4)]
        public void ListSetter_ColumnIsInvalid_ContentIsUnchanged(Int32 column)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);

            String expected = this.GetJoinedContent(instance.Content);

            List<String> values = new List<String>() { "XX", "YY", "ZZ" };

            instance[column] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ListSetter_ColumnValueIsNull_ContentIsUnchanged(Int32 column)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);

            String expected = this.GetJoinedContent(instance.Content);

            List<String> values = null;

            instance[column] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ListSetter_ColumnValueIsEmpty_ContentIsUnchanged(Int32 column)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);

            String expected = this.GetJoinedContent(instance.Content);

            List<String> values = new List<String>();

            instance[column] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ListSetter_ColumnValueWithSameLength_ContentChangedAsExpected(Int32 column)
        {
            List<List<String>> expected = new List<List<String>>()
            {
                new List<String>() { "11", "21", "31" },
                new List<String>() { "12", "22", "32" },
                new List<String>() { "13", "23", ""   },
                new List<String>() {  "" , "24", ""   },
            };

            expected[column] = new List<String>() { "AA", "BB", "CC" };

            CsvContainer instance = new CsvContainer(new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            });

            List<String> values = new List<String>() { "AA", "BB", "CC" };

            instance[column] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(this.GetJoinedContent(expected)));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ListSetter_ColumnValueListIsLonger_ContentChangedAsExpected(Int32 column)
        {
            List<List<String>> expected = new List<List<String>>()
            {
                new List<String>() { "11", "21", "31" },
                new List<String>() { "12", "22", "32" },
                new List<String>() { "13", "23", ""   },
                new List<String>() {  "" , "24", ""   },
            };

            expected[column] = new List<String>() { "AA", "BB", "CC" };

            CsvContainer instance = new CsvContainer(new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            });

            List<String> values = new List<String>() { "AA", "BB", "CC", "DD" };

            instance[column] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(this.GetJoinedContent(expected)));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ListSetter_ColumnValueListIsShorter_ContentChangedAsExpected(Int32 column)
        {
            List<List<String>> expected = new List<List<String>>()
            {
                new List<String>() { "11", "21", "31" },
                new List<String>() { "12", "22", "32" },
                new List<String>() { "13", "23", ""   },
                new List<String>() {  "" , "24", ""   },
            };

            expected[column] = new List<String>() { "AA", "BB", null };

            CsvContainer instance = new CsvContainer(new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            });

            List<String> values = new List<String>() { "AA", "BB" };

            instance[column] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(this.GetJoinedContent(expected)));
        }

        [Test]
        public void ListGetter_HeadingIsFalse_ResultIsNull()
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC"       },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);

            Assert.That(instance["HB"], Is.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("??")]
        public void ListGetter_HeaderIsInvalid_ResultIsNull(String header)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC"       },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = true });

            Assert.That(instance[header], Is.Null);
        }

        [TestCase("HA", "HA,11,21,31")]
        [TestCase("HB", "HB,12,22,32")]
        [TestCase("HC", "HC,13,23,")]
        public void ListGetter_HeaderIsValid_ResultAsExpected(String header, String expected)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC"       },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = true });

            Assert.That(String.Join(",", instance[header]), Is.EqualTo(expected));
        }

        [TestCase("HA")]
        [TestCase("HB")]
        [TestCase("HC")]
        [TestCase("HD")]
        public void ListSetter_ColumnValueIsNull_ContentIsUnchanged(String header)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = true });

            String expected = this.GetJoinedContent(instance.Content);

            List<String> values = null;

            instance[header] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(expected));
        }

        [TestCase("HA")]
        [TestCase("HB")]
        [TestCase("HC")]
        [TestCase("HD")]
        public void ListSetter_ColumnValueIsEmpty_ContentIsUnchanged(String header)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC", "HD" },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = true });

            String expected = this.GetJoinedContent(instance.Content);

            List<String> values = new List<String>();

            instance[header] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(expected));
        }

        [TestCase(0, "HA")]
        [TestCase(1, "HB")]
        [TestCase(2, "HC")]
        [TestCase(3, "HD")]
        public void ListSetter_ColumnValueWithSameLength_ContentChangedAsExpected(Int32 column, String header)
        {
            List<List<String>> expected = new List<List<String>>()
            {
                new List<String>() { "HA", "11", "21", "31" },
                new List<String>() { "HB", "12", "22", "32" },
                new List<String>() { "HC", "13", "23", ""   },
                new List<String>() { "HD",  "" , "24", ""   },
            };

            expected[column] = new List<String>() { "??", "AA", "BB", "CC" };

            CsvContainer instance = new CsvContainer(new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC", "HD" },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            }, new CsvSettings() { Heading = true });

            List<String> values = new List<String>() { "??", "AA", "BB", "CC" };

            instance[header] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(this.GetJoinedContent(expected)));
        }

        [TestCase(0, "HA")]
        [TestCase(1, "HB")]
        [TestCase(2, "HC")]
        [TestCase(3, "HD")]
        public void ListSetter_ColumnValueListIsLonger_ContentChangedAsExpected(Int32 column, String header)
        {
            List<List<String>> expected = new List<List<String>>()
            {
                new List<String>() { "HA", "11", "21", "31" },
                new List<String>() { "HB", "12", "22", "32" },
                new List<String>() { "HC", "13", "23", ""   },
                new List<String>() { "HD",  "" , "24", ""   },
            };

            expected[column] = new List<String>() { "??", "AA", "BB", "CC" };

            CsvContainer instance = new CsvContainer(new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC", "HD" },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            }, new CsvSettings() { Heading = true });

            List<String> values = new List<String>() { "??", "AA", "BB", "CC", "DD" };

            instance[header] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(this.GetJoinedContent(expected)));
        }

        [TestCase(0, "HA")]
        [TestCase(1, "HB")]
        [TestCase(2, "HC")]
        [TestCase(3, "HD")]
        public void ListSetter_ColumnValueListIsShorter_ContentChangedAsExpected(Int32 column, String header)
        {
            List<List<String>> expected = new List<List<String>>()
            {
                new List<String>() { "HA", "11", "21", "31" },
                new List<String>() { "HB", "12", "22", "32" },
                new List<String>() { "HC", "13", "23", ""   },
                new List<String>() { "HD",  "" , "24", ""   },
            };

            expected[column] = new List<String>() { "??", "AA", "BB", null };

            CsvContainer instance = new CsvContainer(new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC", "HD" },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            }, new CsvSettings() { Heading = true });

            List<String> values = new List<String>() { "??", "AA", "BB" };

            instance[header] = values;

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(this.GetJoinedContent(expected)));
        }

        [Test]
        public void ItemGetter_SingleFieldCheck_FieldValuesAsExpected()
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "11", "12", "13", "14", "15" },
                new List<String>() { "21", "22", "23"             },
                new List<String>() { "31", "32", "33", "34"       },
                new List<String>() { "41", "42"                   },
            };

            CsvContainer instance = new CsvContainer(content);

            Assert.That(instance[0, 0], Is.EqualTo("11"));
            Assert.That(instance[1, 0], Is.EqualTo("12"));
            Assert.That(instance[2, 0], Is.EqualTo("13"));
            Assert.That(instance[3, 0], Is.EqualTo("14"));
            Assert.That(instance[4, 0], Is.EqualTo("15"));

            Assert.That(instance[0, 1], Is.EqualTo("21"));
            Assert.That(instance[1, 1], Is.EqualTo("22"));
            Assert.That(instance[2, 1], Is.EqualTo("23"));
            Assert.That(instance[3, 1], Is.Null);
            Assert.That(instance[4, 1], Is.Null);

            Assert.That(instance[0, 2], Is.EqualTo("31"));
            Assert.That(instance[1, 2], Is.EqualTo("32"));
            Assert.That(instance[2, 2], Is.EqualTo("33"));
            Assert.That(instance[3, 2], Is.EqualTo("34"));
            Assert.That(instance[4, 2], Is.Null);

            Assert.That(instance[0, 3], Is.EqualTo("41"));
            Assert.That(instance[1, 3], Is.EqualTo("42"));
            Assert.That(instance[2, 3], Is.Null);
            Assert.That(instance[3, 3], Is.Null);
            Assert.That(instance[4, 3], Is.Null);
        }

        [Test]
        public void ItemSetter_FieldsChangedByColumn_FieldValuesAsExpected()
        {
            List<List<String>> expected = new List<List<String>>()
            {
                new List<String>() { "A1", "B1", "C1", "D1" },
                new List<String>() { "A2", "B2", "C2", "D2" },
                new List<String>() { "A3", "B3", "C3", "D3" },
                new List<String>() { "A4", "B4", "C4", "D4" },
                new List<String>() { "A5", "B5", "C5", "D5" },
            };

            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "11", "12", "13", "14", "15" },
                new List<String>() { "21", "22", "23"             },
                new List<String>() { "31", "32", "33", "34"       },
                new List<String>() { "41", "42"                   },
            };

            CsvContainer instance = new CsvContainer(content);

            instance[0, 0] = "A1";
            instance[1, 0] = "A2";
            instance[2, 0] = "A3";
            instance[3, 0] = "A4";
            instance[4, 0] = "A5";

            instance[0, 1] = "B1";
            instance[1, 1] = "B2";
            instance[2, 1] = "B3";
            instance[3, 1] = "B4";
            instance[4, 1] = "B5";

            instance[0, 2] = "C1";
            instance[1, 2] = "C2";
            instance[2, 2] = "C3";
            instance[3, 2] = "C4";
            instance[4, 2] = "C5";

            instance[0, 3] = "D1";
            instance[1, 3] = "D2";
            instance[2, 3] = "D3";
            instance[3, 3] = "D4";
            instance[4, 3] = "D5";

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(this.GetJoinedContent(expected)));
        }

        [Test]
        public void ItemSetter_FieldsChangedByHeader_FieldValuesAsExpected()
        {
            List<List<String>> expected = new List<List<String>>()
            {
                new List<String>() { "HA", "A1", "A2", "A3", "A4" },
                new List<String>() { "HB", "B1", "B2", "B3", "B4" },
                new List<String>() { "HC", "C1", "C2", "C3", "C4" },
                new List<String>() { "HD", "D1", "D2", "D3", "D4" },
                new List<String>() { "HE", "E1", "E2", "E3", "E4" },
            };

            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC", "HD", "HE" },
                new List<String>() { "11", "12", "13", "14", "15" },
                new List<String>() { "21", "22", "23"             },
                new List<String>() { "31", "32", "33", "34"       },
                new List<String>() { "41", "42"                   },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = true });

            instance["HA", 0] = "A1";
            instance["HB", 0] = "B1";
            instance["HC", 0] = "C1";
            instance["HD", 0] = "D1";
            instance["HE", 0] = "E1";

            instance["HA", 1] = "A2";
            instance["HB", 1] = "B2";
            instance["HC", 1] = "C2";
            instance["HD", 1] = "D2";
            instance["HE", 1] = "E2";

            instance["HA", 2] = "A3";
            instance["HB", 2] = "B3";
            instance["HC", 2] = "C3";
            instance["HD", 2] = "D3";
            instance["HE", 2] = "E3";

            instance["HA", 3] = "A4";
            instance["HB", 3] = "B4";
            instance["HC", 3] = "C4";
            instance["HD", 3] = "D4";
            instance["HE", 3] = "E4";

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(this.GetJoinedContent(expected)));
        }

        [Test]
        public void Width_PropertyCheck_WidthAsExpected()
        {
            CsvContainer instance = new CsvContainer(new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            });

            Assert.That(instance.Width, Is.EqualTo(4));
        }

        [Test]
        public void Length_PropertyCheck_LengthAsExpected()
        {
            CsvContainer instance = new CsvContainer(new List<List<String>>()
            {
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            });

            Assert.That(instance.Length, Is.EqualTo(3));
        }

        [TestCase(true, null)]
        [TestCase(true, "")]
        [TestCase(true, " ")]
        [TestCase(true, "unknown")]
        [TestCase(false, null)]
        [TestCase(false, "")]
        [TestCase(false, " ")]
        [TestCase(false, "unknown")]
        public void Contains_InvalidHeaderValue_ResultIsFalse(Boolean heading, String header)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC"       },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = heading });

            Assert.That(instance.Contains(header), Is.False);
        }

        [TestCase(true, "HA", true)]
        [TestCase(true, "HB", true)]
        [TestCase(true, "HC", true)]
        [TestCase(false, "HA", false)]
        [TestCase(false, "HB", false)]
        [TestCase(false, "HC", false)]
        public void Contains_ValidHeaderValue_ResultAsExpected(Boolean heading, String header, Boolean expected)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC"       },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = heading });

            Assert.That(instance.Contains(header), Is.EqualTo(expected));
        }

        [TestCase(true, null)]
        [TestCase(true, "")]
        [TestCase(true, " ")]
        [TestCase(true, "unknown")]
        [TestCase(false, null)]
        [TestCase(false, "")]
        [TestCase(false, " ")]
        [TestCase(false, "unknown")]
        public void GetColumnIndex_InvalidHeaderValue_ResultAsExpected(Boolean heading, String header)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC"       },
                new List<String>() { "11", "12", "13"       },
                new List<String>() { "21", "22", "23", "24" },
                new List<String>() { "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = heading });

            Assert.That(instance.GetColumnIndex(header), Is.EqualTo(-1));
        }

        [Test]
        public void GetColumnIndex_ValidHeaderValue_ResultAsExpected()
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "HA", "HB", "HC"       },
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = true })
            {
                Compare = StringComparison.InvariantCulture
            };

            Assert.That(instance.GetColumnIndex("HA"), Is.EqualTo(0));
            Assert.That(instance.GetColumnIndex("HB"), Is.EqualTo(1));
            Assert.That(instance.GetColumnIndex("HC"), Is.EqualTo(2));
        }

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

            Assert.That(instance.GetValue<Int32>(column, index), Is.Null);
        }

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

            Assert.That(instance.GetValue<Int32>(column, index), Is.EqualTo(expected));
        }

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

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = true });

            Assert.That(instance.GetValue<Int32>(column, index), Is.Null);
        }

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

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = true });

            Assert.That(instance.GetValue<Int32>(column, index), Is.EqualTo(expected));
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(4, 0)]
        [TestCase(0, 3)]
        [TestCase(-1, 3)]
        [TestCase(4, -1)]
        public void SetValue_InvalidColumnAndOrInvalidIndex_ContentIsUnchanged(Int32 column, Int32 index)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);

            String original = this.GetJoinedContent(instance.Content);

            instance.SetValue<Int32>(999, column, index);

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(original));
        }

        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(0, 2)]
        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 0)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 0)]
        [TestCase(3, 1)]
        [TestCase(3, 2)]
        public void SetValue_ColumnAndIndexAllTypesAreInt32_ResultAsExpected(Int32 column, Int32 index)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content);

            instance.SetValue<Int32>(999, column, index);

            Assert.That(instance[column, index], Is.EqualTo("999"));
        }

        [TestCase("HA", -1)]
        [TestCase("HB", -1)]
        [TestCase("HC", -1)]
        [TestCase("HA", 3)]
        [TestCase("HB", 3)]
        [TestCase("HC", 3)]
        [TestCase("HA", 4)]
        [TestCase("HB", 4)]
        [TestCase("HC", 4)]
        public void SetValue_InvalidHeaderAndOrInvalidIndex_ContentIsUnchanged(String column, Int32 index)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "HA", "HB", "HC"       },
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = true });

            String original = this.GetJoinedContent(instance.Content);

            instance.SetValue<Int32>(999, column, index);

            Assert.That(this.GetJoinedContent(instance.Content), Is.EqualTo(original));
        }

        [TestCase("HA", 0)]
        [TestCase("HA", 1)]
        [TestCase("HA", 2)]
        [TestCase("HB", 0)]
        [TestCase("HB", 1)]
        [TestCase("HB", 2)]
        [TestCase("HC", 0)]
        [TestCase("HC", 1)]
        [TestCase("HC", 2)]
        public void SetValue_HeaderAndIndexAllTypesAreInt32_ResultAsExpected(String column, Int32 index)
        {
            List<List<String>> content = new List<List<String>>()
            {
                new List<String>(){ "HA", "HB", "HC"       },
                new List<String>(){ "11", "12", "13"       },
                new List<String>(){ "21", "22", "23", "24" },
                new List<String>(){ "31", "32"             },
            };

            CsvContainer instance = new CsvContainer(content, new CsvSettings() { Heading = true });

            instance.SetValue<Int32>(999, column, index);

            Assert.That(instance[column, index], Is.EqualTo("999"));
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
