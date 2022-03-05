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
using System.IO;
using System.Text;

namespace Plexdata.CsvParser.Tests.Processors
{
    [TestFixture]
    [TestOf(nameof(CsvWriter))]
    public class CsvWriterTests
    {
        private FileStream testfile = null;
        private String filename = String.Empty;

        [Test]
        [TestCase(null, TestName = "Write(Values: null, Filename: null, Settings: null, Overwrite: false)")]
        [TestCase("", TestName = "Write(Values: null, Filename: empty, Settings: null, Overwrite: false)")]
        [TestCase("   ", TestName = "Write(Values: null, Filename: whitespace, Settings: null, Overwrite: false)")]
        public void Write_InvalidFilename_ThrowsArgumentException(String filename)
        {
            Assert.That(() => CsvWriter.Write((IEnumerable<IEnumerable<Object>>)null, filename, null, false), Throws.ArgumentException);
        }

        [Test]
        [TestCase(Category = TestHelper.IntegrationTest)]
        public void Write_OverwriteFalseFileExists_ThrowsInvalidOperationException()
        {
            try
            {
                this.SetUp(false);

                Assert.That(() => CsvWriter.Write((IEnumerable<IEnumerable<Object>>)null, this.filename, null, false), Throws.InstanceOf<InvalidOperationException>());

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
        public void Write_OverwriteTrueFileExistsButLocked_ThrowsInvalidOperationException()
        {
            try
            {
                this.SetUp(true);

                Assert.That(() => CsvWriter.Write((IEnumerable<IEnumerable<Object>>)null, this.filename, null, true), Throws.InstanceOf<InvalidOperationException>());

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
        public void Write_OverwriteTrueFileExists_ThrowsArgumentException()
        {
            try
            {
                this.SetUp(false);

                Assert.That(() => CsvWriter.Write((IEnumerable<IEnumerable<Object>>)null, this.filename, null, true), Throws.ArgumentException);

                this.CleanUp();
            }
            catch (Exception exception)
            {
                this.CleanUp();
                throw exception;
            }
        }

        [Test]
        [TestCase(1, TestName = "Write(Values: null, Stream: null, Settings: null)")]
        [TestCase(2, TestName = "Write(Values: empty, Stream: null, Settings: null)")]
        [TestCase(3, TestName = "Write(Values: valid, Stream: read-only, Settings: null)")]
        public void Write_ParametersInvalid_ThrowsArgumentException(Int32 configuration)
        {
            List<List<Object>> values = null;
            MemoryStream stream = null;
            CsvSettings settings = null;

            switch (configuration)
            {
                case 1:
                    values = null;
                    stream = null;
                    settings = null;
                    break;
                case 2:
                    values = new List<List<Object>>();
                    stream = null;
                    settings = null;
                    break;
                case 3:
                    values = new List<List<Object>>() { new List<Object>() { new Object() } };
                    stream = new MemoryStream(new Byte[10], false);
                    settings = null;
                    break;
                default:
                    Assert.IsFalse(true);
                    break;
            }

            Assert.That(() => CsvWriter.Write(values, stream, settings), Throws.ArgumentException);
        }

        [Test]
        [TestCase(1, TestName = "Write(Values: valid, Stream: null, Settings: null)")]
        [TestCase(2, TestName = "Write(Values: valid, Stream: valid, Settings: null)")]
        public void Write_ParametersInvalid_ThrowsArgumentNullException(Int32 configuration)
        {
            List<List<Object>> values = null;
            MemoryStream stream = null;
            CsvSettings settings = null;

            switch (configuration)
            {
                case 1:
                    values = new List<List<Object>>() { new List<Object>() { new Object() } };
                    stream = null;
                    settings = null;
                    break;
                case 2:
                    values = new List<List<Object>>() { new List<Object>() { new Object() } };
                    stream = new MemoryStream();
                    settings = null;
                    break;
                default:
                    Assert.IsFalse(true);
                    break;
            }

            Assert.That(() => CsvWriter.Write(values, stream, settings), Throws.ArgumentNullException);
        }

        [Test]
        public void Write_CsvContainerIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => CsvWriter.Write((CsvContainer)null, this.filename), Throws.ArgumentNullException);
        }

        [Test]
        public void Write_CsvContainerIsValid_ResultWrittenAsexpected()
        {
            String expected = "﻿HA,HB,HC\r\n11,12,13\r\n21,22,23\r\n";

            List<List<String>> content = new List<List<String>>()
            {
                new List<String>() { "HA", "HB", "HC" },
                new List<String>() { "11", "12", "13" },
                new List<String>() { "21", "22", "23" },
            };

            CsvContainer container = new CsvContainer(content);

            using (MemoryStream stream = new MemoryStream())
            {
                CsvWriter.Write(container, stream);

                Assert.That(Encoding.UTF8.GetString(stream.ToArray()), Is.EqualTo(expected));
            }
        }

        [Test]
        public void Write_UsingDefaultSettingsAllDataValid_ResultIsAsExpected()
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
            List<List<Object>> values = new List<List<Object>>()
            {
                new List<Object>() { "Label", "Enabled", "Number", "Currency", },
                new List<Object>() { "Label-1", false, 100, 1.234m, },
                new List<Object>() { "Label-2", true,  100, 2.345m, },
                new List<Object>() { "Label-3", false, 100, 3.456m, },
                new List<Object>() { "Label-4", true,  100, 4.567m, },
                new List<Object>() { "Label-5", false, 100, 5.678m, },
                new List<Object>() { "Label-6", true,  100, 6.789m, },
            };

            CsvWriter.Write(values, stream, settings);

            String actual = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Write_UsingAsciiGermanHashAllDataValid_ResultIsAsExpected()
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
            List<List<Object>> values = new List<List<Object>>()
            {
                new List<Object>() { "Label", "Enabled", "Number", "Currency", },
                new List<Object>() { "Label-1", false, 100, 1.234m, },
                new List<Object>() { "Label-2", true,  100, 2.345m, },
                new List<Object>() { "Label-3", false, 100, 3.456m, },
                new List<Object>() { "Label-4", true,  100, 4.567m, },
                new List<Object>() { "Label-5", false, 100, 5.678m, },
                new List<Object>() { "Label-6", true,  100, 6.789m, },
            };

            CsvWriter.Write(values, stream, settings);

            String actual = Encoding.ASCII.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Write_UsingDefaultSettingsSomeDataInvalid_ResultIsAsExpected()
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
            List<List<Object>> values = new List<List<Object>>()
            {
                new List<Object>() { "Label", "Enabled", "Number", "Currency", },
                new List<Object>() { null,      false, 100, 1.234m, },
                new List<Object>() { "Label-2", true,  100, 2.345m, },
                new List<Object>() { null,      false, 100, 3.456m, },
                new List<Object>() { "Label-4", true,  100, 4.567m, },
                new List<Object>() { null,      false, 100, 5.678m, },
                new List<Object>() { "Label-6", true,  100, 6.789m, },
            };

            CsvWriter.Write(values, stream, settings);

            String actual = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Write_UsingAsciiGermanHashSomeDataInvalid_ResultIsAsExpected()
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
            List<List<Object>> values = new List<List<Object>>()
            {
                new List<Object>() { "Label", "Enabled", "Number", "Currency", },
                new List<Object>() { null,      false, 100, 1.234m, },
                new List<Object>() { "Label-2", true,  100, 2.345m, },
                new List<Object>() { null,      false, 100, 3.456m, },
                new List<Object>() { "Label-4", true,  100, 4.567m, },
                new List<Object>() { null,      false, 100, 5.678m, },
                new List<Object>() { "Label-6", true,  100, 6.789m, },
            };

            CsvWriter.Write(values, stream, settings);

            String actual = Encoding.ASCII.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Write_UsingUtf32EnglishCommaAllDataValid_ResultIsAsExpected()
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
            List<List<Object>> values = new List<List<Object>>()
            {
                new List<Object>() { "Label", "Enabled", "Number", "Currency", },
                new List<Object>() { "Label-1", false, 100, 1.234m, },
                new List<Object>() { "Label-2", true,  100, 2.345m, },
                new List<Object>() { "Label-3", false, 100, 3.456m, },
                new List<Object>() { "Label-4", true,  100, 4.567m, },
                new List<Object>() { "Label-5", false, 100, 5.678m, },
                new List<Object>() { "Label-6", true,  100, 6.789m, },
            };

            CsvWriter.Write(values, stream, settings);

            String actual = Encoding.UTF32.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Write_UsingDefaultSettingsButMappingsSomeDataInvalid_ResultIsAsExpected()
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
            List<List<Object>> values = new List<List<Object>>()
            {
                new List<Object>() { "Label", "Enabled", "Number", "Currency", },
                new List<Object>() { null,      false, 100, 1.234m, },
                new List<Object>() { "Label-2", true,  100, 2.345m, },
                new List<Object>() { null,      false, 100, 3.456m, },
                new List<Object>() { "Label-4", true,  100, 4.567m, },
                new List<Object>() { null,      false, 100, 5.678m, },
                new List<Object>() { "Label-6", true,  100, 6.789m, },
            };

            CsvWriter.Write(values, stream, settings);

            String actual = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Write_UsingAsciiForDoubleQuoteEscaping_ResultIsAsExpected()
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
            List<List<Object>> values = new List<List<Object>>()
            {
                new List<Object>() { "Label", "Text", },
                new List<Object>() { "Label \"1\"", "\"Double Quotes\" included" },
                new List<Object>() { "Label \"2\"", "Double Quotes not included" },
                new List<Object>() { "Label \"3\"", "\"Double\" Quotes included" },
                new List<Object>() { "Label \"4\"", "Double Quotes not included" },
                new List<Object>() { "Label \"5\"", "Double \"Quotes\" included" },
                new List<Object>() { "Label \"6\"", "Double Quotes not included" },
            };

            CsvWriter.Write(values, stream, settings);

            String actual = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Category = TestHelper.IntegrationTest)]
        public void Write_FullIntegrationTest_ResultIsAsExpected()
        {
            String expected =
                "﻿Surname,Forename,Identifier,Date,Sales,Active,Notes\r\n" +
                "\"Marley\",\"Bob\",\"1001\",\"2007-05-03T00:00:00\",\"1234.56\",\"nope\",\"Have a short note here.\"\r\n" +
                "\"Monroe\",\"Marilyn\",\"1002\",\"2008-06-05T00:00:00\",\"1234.56\",\"nope\",\"\",\"42\",\"one\"\"more string\"\r\n" +
                "\"Snipes\",\"Wesley\",\"1003\",\"2009-07-06T00:00:00\",\"1234.56\",\"yeah\",\"Have a short note here.\"\r\n" +
                "\"Hurley\",\"Elizabeth\",\"1004\",\"2005-08-08T00:00:00\",\"1234.56\",\"yeah\",\"Have a short note here.\"\r\n";

            List<List<Object>> customers = new List<List<Object>>
            {
                new List<Object>() { "Surname", "Forename", "Identifier", "Date", "Sales", "Active", "Notes" },
                new List<Object>() { "Marley", "Bob", 1001, new DateTime(2007, 5, 3), 1234.56m, false, "Have a short note here." },
                new List<Object>() { "Monroe", "Marilyn", 1002, new DateTime(2008, 6, 5), 1234.56m, false, null, 42, "one\"more string" },
                new List<Object>() { "Snipes", "Wesley", 1003, new DateTime(2009, 7, 6), 1234.56m, true, "Have a short note here." },
                new List<Object>() { "Hurley", "Elizabeth", 1004, new DateTime(2005, 8, 8), 1234.56m, true, "Have a short note here." },
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
                CsvWriter.Write(customers, stream, settings);
                actual = settings.Encoding.GetString(stream.ToArray());
            }

            Assert.That(actual, Is.EqualTo(expected));
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
