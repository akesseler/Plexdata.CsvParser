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

using NUnit.Framework;
using Plexdata.CsvParser.Attributes;
using Plexdata.CsvParser.Processors;
using Plexdata.Utilities.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Plexdata.CsvParser.Tests.Processors
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    [Category(TestType.UnitTest)]
    [TestOf(nameof(CsvImporter<TestClassBase>))]
    public class CsvImporterTests
    {
        private class TestClassBase { }

        [TestCase(null, TestName = "Load(Filename: null, Settings: null)")]
        [TestCase("", TestName = "Load(Filename: empty, Settings: null)")]
        [TestCase("   ", TestName = "Load(Filename: whitespace, Settings: null)")]
        public void Load_InvalidFilename_ThrowsArgumentException(String filename)
        {
            CsvSettings settings = null;
            Assert.That(() => CsvImporter<TestClassBase>.Load(filename, settings), Throws.ArgumentException);
        }

        [TestCase(Category = TestType.IntegrationTest)]
        public void Load_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            String filename = @"c:\temp\missing-file.csv";
            CsvSettings settings = null;
            Assert.That(() => CsvImporter<TestClassBase>.Load(filename, settings), Throws.InstanceOf<FileNotFoundException>());
        }

        [Test]
        public void Load_StreamIsNull_ThrowsArgumentNullException()
        {
            Stream stream = null;
            CsvSettings settings = null;

            Assert.That(() => CsvImporter<TestClassBase>.Load(stream, settings), Throws.ArgumentNullException);
        }

        [TestCase(Category = TestType.IntegrationTest)]
        public void Load_StreamCanWriteOnly_ThrowsArgumentException()
        {
            String filename = Path.GetTempFileName();
            Stream stream = null;
            CsvSettings settings = null;

            try
            {
                stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);

                Assert.That(() => CsvImporter<TestClassBase>.Load(stream, settings), Throws.ArgumentException);

                this.CleanUp(stream, filename);
            }
            catch
            {
                this.CleanUp(stream, filename);
                throw;
            }
        }

        [Test]
        public void Load_SettingsAreNull_ThrowsArgumentNullException()
        {
            using (Stream stream = new MemoryStream())
            {
                CsvSettings settings = null;
                Assert.That(() => CsvImporter<TestClassBase>.Load(stream, settings), Throws.ArgumentNullException);
            }
        }

        [Test]
        public void Load_NotExactlyButColumnCountMismatch_ThrowsFormatException()
        {
            String content =
                "Label,Enabled,Number,Currency\r\n" +
                "Label-1,true,42,\"1,234\"\r\n" +
                "Label-2,true,42\r\n";

            CsvSettings settings = new CsvSettings();

            using (MemoryStream stream = new MemoryStream(settings.Encoding.GetBytes(content)))
            {
                Assert.That(() => CsvImporter<TestClass1>.Load(stream, settings), Throws.InstanceOf<FormatException>());
            }
        }

        [Test]
        public void Load_UseExactlyAndColumnCountMismatch_ThrowsFormatException()
        {
            String content =
                "Label,Enabled,Number,Currency\r\n" +
                "Label-1,true,42,\"1,234\"\r\n" +
                "Label-2,true,42,\"1,234\",huch\r\n";

            CsvSettings settings = new CsvSettings() { Exactly = true, };

            using (MemoryStream stream = new MemoryStream(settings.Encoding.GetBytes(content)))
            {
                Assert.That(() => CsvImporter<TestClass1>.Load(stream, settings), Throws.InstanceOf<FormatException>());
            }
        }

        [Test]
        public void Load_UseExactlyAndHeadingButWrongHeaderOrder_ThrowsFormatException()
        {
            String content =
                "Label,Number,Enabled,Currency\r\n" +
                "Label-1,true,42,\"1,234\"\r\n" +
                "Label-2,true,42,\"1,234\"\r\n";

            CsvSettings settings = new CsvSettings() { Exactly = true, Heading = true, };

            using (MemoryStream stream = new MemoryStream(settings.Encoding.GetBytes(content)))
            {
                Assert.That(() => CsvImporter<TestClass1>.Load(stream, settings), Throws.InstanceOf<FormatException>());
            }
        }

        [Test]
        public void Load_NoDefaultConstructor_ThrowsInvalidOperationException()
        {
            String content =
                "Label,Enabled,Number,Currency\r\n" +
                "Label-1,true,42,\"1,234\"\r\n" +
                "Label-2,true,42,\"1,234\"\r\n";

            CsvSettings settings = new CsvSettings() { Exactly = true, Heading = true, };

            using (MemoryStream stream = new MemoryStream(settings.Encoding.GetBytes(content)))
            {
                Assert.That(() => CsvImporter<TestClass2>.Load(stream, settings), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        public void Load_DefaultConstructorFails_ThrowsInvalidOperationException()
        {
            String content =
                "Label,Enabled,Number,Currency\r\n" +
                "Label-1,true,42,\"1,234\"\r\n" +
                "Label-2,true,42,\"1,234\"\r\n";

            CsvSettings settings = new CsvSettings() { Exactly = true, Heading = true, };

            using (MemoryStream stream = new MemoryStream(settings.Encoding.GetBytes(content)))
            {
                Assert.That(() => CsvImporter<TestClass3>.Load(stream, settings), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        public void Load_DataConversionFails_ThrowsFormatException()
        {
            String content =
                "Label,Enabled,Number,Currency\r\n" +
                "Label-1,true,42,\"1,234\"\r\n" +
                "Label-2,true,Hello,\"1,234\"\r\n";

            CsvSettings settings = new CsvSettings() { Exactly = true, Heading = true, };

            using (MemoryStream stream = new MemoryStream(settings.Encoding.GetBytes(content)))
            {
                Assert.That(() => CsvImporter<TestClass1>.Load(stream, settings), Throws.InstanceOf<FormatException>());
            }
        }

        [Test]
        public void Load_DataConversionSucceeds_ResultIsAsExpected()
        {
            String content =
                "Label,Enabled,Number,Currency\r\n" +
                "Label-1,true,42,\"1,234\"\r\n" +
                "Label-2,true,42,\"1,234\"\r\n";

            CsvSettings settings = new CsvSettings();

            List<TestClass4> expected = new List<TestClass4>() {
                new TestClass4() { Label = "Label-1", Enabled = true, Number = 42, Currency = 1.234m, },
                new TestClass4() { Label = "Label-2", Enabled = true, Number = 42, Currency = 1.234m, },
            };

            using (MemoryStream stream = new MemoryStream(settings.Encoding.GetBytes(content)))
            {
                List<TestClass4> actual = CsvImporter<TestClass4>.Load(stream, settings).ToList();

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [TestCase(Category = TestType.IntegrationTest)]
        public void Load_FullIntegrationTest_ResultIsAsExpected()
        {
            List<CsvCustomer> expected = new List<CsvCustomer>
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

            String content =
                "Surname,    Forename,      Identifier, Date,       Sales,        Active, Notes\r\n" +
                "\"Marley\", \"Bob\",       1001,       2007-05-03, \"1,234.56\", nope,   \"Have a short note here.\"\r\n" +
                "\"Monroe\", \"Marilyn\",   1002,       2008-06-05, \"1,234.56\", nope,   \"\"\r\n" +
                "\"Snipes\", \"Wesley\",    1003,       2009-07-06, \"1,234.56\", yeah,   \"Have a short note here.\"\r\n" +
                "\"Hurley\", \"Elizabeth\", 1004,       2005-08-08, \"1,234.56\", yeah,   \"Have a short note here.\"\r\n";

            CsvSettings settings = new CsvSettings
            {
                Culture = CultureInfo.GetCultureInfo("en-US"),
                Mappings = new CsvMappings
                {
                    TrueValues = new List<String> { "yeah" },
                    FalseValues = new List<String> { "nope" },
                },
            };

            List<CsvCustomer> actual = null;

            using (MemoryStream stream = new MemoryStream(settings.Encoding.GetBytes(content)))
            {
                actual = CsvImporter<CsvCustomer>.Load(stream, settings).ToList();
            }

            Assert.That(actual, Is.EqualTo(expected));
        }

        #region Test helper classes

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
            private TestClass2() { }

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
        private class TestClass3 : TestClassBase
        {
            public TestClass3() { throw new Exception(); }

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
        private class TestClass4 : TestClassBase
        {
            [CsvColumn]
            public String Label { get; set; }

            [CsvColumn]
            public Boolean Enabled { get; set; }

            [CsvColumn]
            public Int16 Number { get; set; }

            [CsvColumn]
            public Decimal Currency { get; set; }

            public override Int32 GetHashCode()
            {
                StringBuilder builder = new StringBuilder(512);
                builder.Append(this.Label);
                builder.Append(this.Enabled);
                builder.Append(this.Number);
                builder.Append(this.Currency);
                return builder.ToString().GetHashCode();
            }

            public override Boolean Equals(Object other)
            {
                if (other is TestClass4)
                {
                    return this.GetHashCode() == (other as TestClass4).GetHashCode();
                }
                else
                {
                    return base.Equals(other);
                }
            }
        }

        [CsvDocument]
        private class CsvCustomer : TestClassBase, IEquatable<CsvCustomer>
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

            public Boolean Equals(CsvCustomer other)
            {
                if (other != null)
                {
                    return String.Compare(this.ToString(), (other as CsvCustomer).ToString()) == 0;
                }
                else
                {
                    return base.Equals(other);
                }
            }

            public override String ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"{this.Id}{this.ExternalId}{this.FirstName}{this.LastName}{this.IsActive}{this.EntryDate}{this.SalesAverage}{this.Description}");
                return builder.ToString();
            }
        }

        #endregion

        #region Test helper methods

        private void CleanUp(Stream stream, String filename)
        {
            try
            {
                if (TestHelper.IsIntegrationTestCategory())
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }

                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}

