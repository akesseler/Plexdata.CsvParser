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
using Plexdata.CsvParser.Attributes;
using Plexdata.CsvParser.Processors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Plexdata.CsvParser.Tests.Processors
{
    [TestFixture]
    [TestOf(nameof(CsvExporter<TestClassBase>))]
    public class CsvExporterTests
    {
        private class TestClassBase { }

        private FileStream testfile = null;
        private String filename = String.Empty;

        [Test]
        [TestCase(null, TestName = "Save(Values: null, Filename: null, Settings: null, Overwrite: false)")]
        [TestCase("", TestName = "Save(Values: null, Filename: empty, Settings: null, Overwrite: false)")]
        [TestCase("   ", TestName = "Save(Values: null, Filename: whitespace, Settings: null, Overwrite: false)")]
        public void Save_InvalidFilename_ThrowsArgumentException(String filename)
        {
            Assert.Throws<ArgumentException>(() => { CsvExporter<TestClassBase>.Save(null, filename, null, false); });
        }

        [Test]
        [TestCase(Category = TestHelper.IntegrationTest)]
        public void Save_OverwriteFalseFileExists_ThrowsInvalidOperationException()
        {
            try
            {
                this.SetUp(false);

                Assert.Throws<InvalidOperationException>(() => { CsvExporter<TestClassBase>.Save(null, this.filename, null, false); });

                this.CleanUp();
            }
            catch (Exception exception)
            {
                this.CleanUp();
                throw exception;
            }
        }

        [Test]
        [TestCase(Category = TestHelper.IntegrationTest)]
        public void Save_OverwriteTrueFileExistsButLocked_ThrowsInvalidOperationException()
        {
            try
            {
                this.SetUp(true);

                Assert.Throws<InvalidOperationException>(() => { CsvExporter<TestClassBase>.Save(null, this.filename, null, true); });

                this.CleanUp();
            }
            catch (Exception exception)
            {
                this.CleanUp();
                throw exception;
            }
        }

        [Test]
        [TestCase(Category = TestHelper.IntegrationTest)]
        public void Save_OverwriteTrueFileExists_ThrowsArgumentException()
        {
            try
            {
                this.SetUp(false);

                Assert.Throws<ArgumentException>(() => { CsvExporter<TestClassBase>.Save(null, this.filename, null, true); });

                this.CleanUp();
            }
            catch (Exception exception)
            {
                this.CleanUp();
                throw exception;
            }
        }

        [Test]
        [TestCase(1, TestName = "Save(Values: null, Stream: null, Settings: null)")]
        [TestCase(2, TestName = "Save(Values: empty, Stream: null, Settings: null)")]
        [TestCase(3, TestName = "Save(Values: valid, Stream: read-only, Settings: null)")]
        public void Save_ParametersInvalid_ThrowsArgumentException(Int32 configuration)
        {
            List<TestClassBase> values = null;
            MemoryStream stream = null;
            CsvSettings settings = null;
            Type exception = null;

            switch (configuration)
            {
                case 1:
                    values = null;
                    stream = null;
                    settings = null;
                    exception = typeof(ArgumentException);
                    break;
                case 2:
                    values = new List<TestClassBase>();
                    stream = null;
                    settings = null;
                    exception = typeof(ArgumentException);
                    break;
                case 3:
                    values = new List<TestClassBase>() { new TestClassBase() };
                    stream = new MemoryStream(new Byte[10], false);
                    settings = null;
                    exception = typeof(ArgumentException);
                    break;
                default:
                    Assert.IsFalse(true);
                    break;
            }

            Assert.Throws<ArgumentException>(() => { CsvExporter<TestClassBase>.Save(values, stream, settings); });
        }

        [Test]
        [TestCase(1, TestName = "Save(Values: valid, Stream: null, Settings: null)")]
        [TestCase(2, TestName = "Save(Values: valid, Stream: valid, Settings: null)")]
        public void Save_ParametersInvalid_ThrowsArgumentNullException(Int32 configuration)
        {
            List<TestClassBase> values = null;
            MemoryStream stream = null;
            CsvSettings settings = null;
            Type exception = null;

            switch (configuration)
            {
                case 1:
                    values = new List<TestClassBase>() { new TestClassBase() };
                    stream = null;
                    settings = null;
                    exception = typeof(ArgumentNullException);
                    break;
                case 2:
                    values = new List<TestClassBase>() { new TestClassBase() };
                    stream = new MemoryStream();
                    settings = null;
                    exception = typeof(ArgumentNullException);
                    break;
                default:
                    Assert.IsFalse(true);
                    break;
            }

            Assert.Throws<ArgumentNullException>(() => { CsvExporter<TestClassBase>.Save(values, stream, settings); });
        }

        [Test]
        public void Save_UsingDefaultSettingsAllDataValid_ResultIsAsExpected()
        {
            String expected =
                "\uFEFFLabel,Enabled,Number,Currency\r\n" +
                "Label-1,false,100,\"1,234\"\r\n" +
                "Label-2,true,100,\"2,345\"\r\n" +
                "Label-3,false,100,\"3,456\"\r\n" +
                "Label-4,true,100,\"4,567\"\r\n" +
                "Label-5,false,100,\"5,678\"\r\n" +
                "Label-6,true,100,\"6,789\"\r\n";

            MemoryStream stream = new MemoryStream();
            CsvSettings settings = new CsvSettings();
            List<TestClass1> values = new List<TestClass1>()
            {
                new TestClass1() { Label = "Label-1", Enabled = false, Number = 100, Currency = 1.234m, },
                new TestClass1() { Label = "Label-2", Enabled = true,  Number = 100, Currency = 2.345m, },
                new TestClass1() { Label = "Label-3", Enabled = false, Number = 100, Currency = 3.456m, },
                new TestClass1() { Label = "Label-4", Enabled = true,  Number = 100, Currency = 4.567m, },
                new TestClass1() { Label = "Label-5", Enabled = false, Number = 100, Currency = 5.678m, },
                new TestClass1() { Label = "Label-6", Enabled = true,  Number = 100, Currency = 6.789m, },
            };

            CsvExporter<TestClass1>.Save(values, stream, settings);

            String actual = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Save_UsingAsciiGermanHashAllDataValid_ResultIsAsExpected()
        {
            String expected =
                "Label#Enabled#Number#Currency\r\n" +
                "Label-1#false#100#1,234\r\n" +
                "Label-2#true#100#2,345\r\n" +
                "Label-3#false#100#3,456\r\n" +
                "Label-4#true#100#4,567\r\n" +
                "Label-5#false#100#5,678\r\n" +
                "Label-6#true#100#6,789\r\n";

            MemoryStream stream = new MemoryStream();
            CsvSettings settings = new CsvSettings() { Encoding = Encoding.ASCII, Heading = true, Separator = '#', Culture = CultureInfo.GetCultureInfo("de-DE"), };
            List<TestClass1> values = new List<TestClass1>()
            {
                new TestClass1() { Label = "Label-1", Enabled = false, Number = 100, Currency = 1.234m, },
                new TestClass1() { Label = "Label-2", Enabled = true,  Number = 100, Currency = 2.345m, },
                new TestClass1() { Label = "Label-3", Enabled = false, Number = 100, Currency = 3.456m, },
                new TestClass1() { Label = "Label-4", Enabled = true,  Number = 100, Currency = 4.567m, },
                new TestClass1() { Label = "Label-5", Enabled = false, Number = 100, Currency = 5.678m, },
                new TestClass1() { Label = "Label-6", Enabled = true,  Number = 100, Currency = 6.789m, },
            };

            CsvExporter<TestClass1>.Save(values, stream, settings);

            String actual = Encoding.ASCII.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Save_UsingDefaultSettingsSomeDataInvalid_ResultIsAsExpected()
        {
            String expected =
                "\uFEFFLabel,Enabled,Number,Currency\r\n" +
                ",false,100,\"1,234\"\r\n" +
                "Label-2,true,100,\"2,345\"\r\n" +
                ",false,100,\"3,456\"\r\n" +
                "Label-4,true,100,\"4,567\"\r\n" +
                ",false,100,\"5,678\"\r\n" +
                "Label-6,true,100,\"6,789\"\r\n";

            MemoryStream stream = new MemoryStream();
            CsvSettings settings = new CsvSettings();
            List<TestClass1> values = new List<TestClass1>()
            {
                new TestClass1() { Label = null,      Enabled = false, Number = 100, Currency = 1.234m, },
                new TestClass1() { Label = "Label-2", Enabled = true,  Number = 100, Currency = 2.345m, },
                new TestClass1() { Label = null,      Enabled = false, Number = 100, Currency = 3.456m, },
                new TestClass1() { Label = "Label-4", Enabled = true,  Number = 100, Currency = 4.567m, },
                new TestClass1() { Label = null,      Enabled = false, Number = 100, Currency = 5.678m, },
                new TestClass1() { Label = "Label-6", Enabled = true,  Number = 100, Currency = 6.789m, },
            };

            CsvExporter<TestClass1>.Save(values, stream, settings);

            String actual = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Save_UsingAsciiGermanHashSomeDataInvalid_ResultIsAsExpected()
        {
            String expected =
                "Label#Enabled#Number#Currency\r\n" +
                "#false#100#1,234\r\n" +
                "Label-2#true#100#2,345\r\n" +
                "#false#100#3,456\r\n" +
                "Label-4#true#100#4,567\r\n" +
                "#false#100#5,678\r\n" +
                "Label-6#true#100#6,789\r\n";

            MemoryStream stream = new MemoryStream();
            CsvSettings settings = new CsvSettings() { Encoding = Encoding.ASCII, Heading = true, Separator = '#', Culture = CultureInfo.GetCultureInfo("de-DE"), };
            List<TestClass1> values = new List<TestClass1>()
            {
                new TestClass1() { Label = null,      Enabled = false, Number = 100, Currency = 1.234m, },
                new TestClass1() { Label = "Label-2", Enabled = true,  Number = 100, Currency = 2.345m, },
                new TestClass1() { Label = null,      Enabled = false, Number = 100, Currency = 3.456m, },
                new TestClass1() { Label = "Label-4", Enabled = true,  Number = 100, Currency = 4.567m, },
                new TestClass1() { Label = null,      Enabled = false, Number = 100, Currency = 5.678m, },
                new TestClass1() { Label = "Label-6", Enabled = true,  Number = 100, Currency = 6.789m, },
            };

            CsvExporter<TestClass1>.Save(values, stream, settings);

            String actual = Encoding.ASCII.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Save_UsingUtf32EnglishCommaAllDataValid_ResultIsAsExpected()
        {
            String expected =
                "\uFEFFLabel,Enabled,Number,Currency\r\n" +
                "Label-1,false,100,1.234\r\n" +
                "Label-2,true,100,2.345\r\n" +
                "Label-3,false,100,3.456\r\n" +
                "Label-4,true,100,4.567\r\n" +
                "Label-5,false,100,5.678\r\n" +
                "Label-6,true,100,6.789\r\n";

            MemoryStream stream = new MemoryStream();
            CsvSettings settings = new CsvSettings() { Encoding = Encoding.UTF32, Heading = true, Separator = ',', Culture = CultureInfo.GetCultureInfo("en-US"), };
            List<TestClass1> values = new List<TestClass1>()
            {
                new TestClass1() { Label = "Label-1", Enabled = false, Number = 100, Currency = 1.234m, },
                new TestClass1() { Label = "Label-2", Enabled = true,  Number = 100, Currency = 2.345m, },
                new TestClass1() { Label = "Label-3", Enabled = false, Number = 100, Currency = 3.456m, },
                new TestClass1() { Label = "Label-4", Enabled = true,  Number = 100, Currency = 4.567m, },
                new TestClass1() { Label = "Label-5", Enabled = false, Number = 100, Currency = 5.678m, },
                new TestClass1() { Label = "Label-6", Enabled = true,  Number = 100, Currency = 6.789m, },
            };

            CsvExporter<TestClass1>.Save(values, stream, settings);

            String actual = Encoding.UTF32.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Save_UsingDefaultSettingsButMappingsSomeDataInvalid_ResultIsAsExpected()
        {
            String expected =
                "\uFEFFLabel,Enabled,Number,Currency\r\n" +
                "Void,Nope,100,\"1,234\"\r\n" +
                "Label-2,Yeah,100,\"2,345\"\r\n" +
                "Void,Nope,100,\"3,456\"\r\n" +
                "Label-4,Yeah,100,\"4,567\"\r\n" +
                "Void,Nope,100,\"5,678\"\r\n" +
                "Label-6,Yeah,100,\"6,789\"\r\n";

            MemoryStream stream = new MemoryStream();
            CsvSettings settings = new CsvSettings() { Mappings = new CsvMappings() { TrueValue = "Yeah", FalseValue = "Nope", NullValue = "Void" }, };
            List<TestClass1> values = new List<TestClass1>()
            {
                new TestClass1() { Label = null,      Enabled = false, Number = 100, Currency = 1.234m, },
                new TestClass1() { Label = "Label-2", Enabled = true,  Number = 100, Currency = 2.345m, },
                new TestClass1() { Label = null,      Enabled = false, Number = 100, Currency = 3.456m, },
                new TestClass1() { Label = "Label-4", Enabled = true,  Number = 100, Currency = 4.567m, },
                new TestClass1() { Label = null,      Enabled = false, Number = 100, Currency = 5.678m, },
                new TestClass1() { Label = "Label-6", Enabled = true,  Number = 100, Currency = 6.789m, },
            };

            CsvExporter<TestClass1>.Save(values, stream, settings);

            String actual = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Save_UsingAsciiForDoubleQuoteEscaping_ResultIsAsExpected()
        {
            String expected =
                "Label,Text\r\n" +
                "\"Label \"\"1\"\"\",\"\"\"Double Quotes\"\" included\"\r\n" +
                "\"Label \"\"2\"\"\",Double Quotes not included\r\n" +
                "\"Label \"\"3\"\"\",\"\"\"Double\"\" Quotes included\"\r\n" +
                "\"Label \"\"4\"\"\",Double Quotes not included\r\n" +
                "\"Label \"\"5\"\"\",\"Double \"\"Quotes\"\" included\"\r\n" +
                "\"Label \"\"6\"\"\",Double Quotes not included\r\n";

            MemoryStream stream = new MemoryStream();
            CsvSettings settings = new CsvSettings() { Encoding = Encoding.ASCII };
            List<TestClass2> values = new List<TestClass2>()
            {
                new TestClass2() { Label = "Label \"1\"", Text = "\"Double Quotes\" included" },
                new TestClass2() { Label = "Label \"2\"", Text = "Double Quotes not included" },
                new TestClass2() { Label = "Label \"3\"", Text = "\"Double\" Quotes included" },
                new TestClass2() { Label = "Label \"4\"", Text = "Double Quotes not included" },
                new TestClass2() { Label = "Label \"5\"", Text = "Double \"Quotes\" included" },
                new TestClass2() { Label = "Label \"6\"", Text = "Double Quotes not included" },
            };

            CsvExporter<TestClass2>.Save(values, stream, settings);

            String actual = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Category = TestHelper.IntegrationTest)]
        public void Save_FullIntegrationTest_ResultIsAsExpected()
        {
            String expected =
                "﻿Surname,Forename,Identifier,Date,Sales,Active,Notes\r\n" +
                "\"Marley\",\"Bob\",1001,2007-05-03T00:00:00,1234.56,nope,\"Have a short note here.\"\r\n" +
                "\"Monroe\",\"Marilyn\",1002,2008-06-05T00:00:00,1234.56,nope,\r\n" +
                "\"Snipes\",\"Wesley\",1003,2009-07-06T00:00:00,1234.56,yeah,\"Have a short note here.\"\r\n" +
                "\"Hurley\",\"Elizabeth\",1004,2005-08-08T00:00:00,1234.56,yeah,\"Have a short note here.\"\r\n";

            List<CsvCustomer> customers = new List<CsvCustomer>
            {
                new CsvCustomer {
                    LastName = "Marley",
                    FirstName = "Bob",
                    ExternalId = 1001,
                    EntryDate = new DateTime(2007, 5, 3),
                    SalesAverage = 1234.56m,
                    IsActive = false,
                    Description = "Have a short note here." },
                new CsvCustomer {
                    LastName = "Monroe",
                    FirstName = "Marilyn",
                    ExternalId = 1002,
                    EntryDate = new DateTime(2008, 6, 5),
                    SalesAverage = 1234.56m,
                    IsActive = false,
                    Description = null },
                new CsvCustomer {
                    LastName = "Snipes",
                    FirstName = "Wesley",
                    ExternalId = 1003,
                    EntryDate = new DateTime(2009, 7, 6),
                    SalesAverage = 1234.56m,
                    IsActive = true,
                    Description = "Have a short note here." },
                new CsvCustomer {
                    LastName = "Hurley",
                    FirstName = "Elizabeth",
                    ExternalId = 1004,
                    EntryDate = new DateTime(2005, 8, 8),
                    SalesAverage = 1234.56m,
                    IsActive = true,
                    Description = "Have a short note here." },
            };

            CsvSettings settings = new CsvSettings
            {
                Culture = CultureInfo.GetCultureInfo("en-US"),
                Textual = true,
                Mappings = new CsvMappings
                {
                    TrueValue = "yeah",
                    FalseValue = "nope",
                },
            };

            String actual = String.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                CsvExporter<CsvCustomer>.Save(customers, stream, settings);
                actual = settings.Encoding.GetString(stream.ToArray());
            }

            Assert.That(actual, Is.EqualTo(expected));
        }

        [CsvDocument]
        private class TestClass1 : TestClassBase
        {
            [CsvColumn]
            public String Label { get; set; }

            [CsvColumn]
            public Boolean Enabled { get; set; }

            [CsvColumn]
            public Int16 Number { get; set; }

            [CsvColumn]
            public Decimal Currency { get; set; }
        }

        [CsvDocument]
        private class TestClass2 : TestClassBase
        {
            [CsvColumn]
            public String Label { get; set; }

            [CsvColumn]
            public String Text { get; set; }
        }

        [CsvDocument]
        private class CsvCustomer
        {
            [CsvIgnore]
            public Int32 Id { get; set; }

            [CsvColumn(Offset = 2, Header = "Identifier")]
            public Int32 ExternalId { get; set; }

            [CsvColumn(Offset = 1, Header = "Forename")]
            public String FirstName { get; set; }

            [CsvColumn(Offset = 0, Header = "Surname")]
            public String LastName { get; set; }

            [CsvColumn(Offset = 5, Header = "Active")]
            public Boolean IsActive { get; set; }

            [CsvColumn(Offset = 3, Header = "Date")]
            public DateTime? EntryDate { get; set; }

            [CsvColumn(Offset = 4, Header = "Sales")]
            public Decimal SalesAverage { get; set; }

            [CsvColumn(Offset = 6, Header = "Notes")]
            public String Description { get; set; }
        }

        #region Test helper

        private void SetUp(Boolean locked)
        {
            if (TestHelper.IsIntegrationTest())
            {
                this.filename = Path.GetTempFileName();

                if (locked)
                {
                    this.testfile = File.Create(this.filename);
                }
                else
                {
                    using (File.Create(this.filename)) { }
                }
            }
        }

        private void CleanUp()
        {
            try
            {
                if (TestHelper.IsIntegrationTest())
                {
                    if (this.testfile != null)
                    {
                        this.testfile.Close();
                        this.testfile = null;
                    }

                    if (File.Exists(this.filename))
                    {
                        File.Delete(this.filename);
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}
