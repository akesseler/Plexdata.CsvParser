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
using Plexdata.CsvParser.Processors;
using Plexdata.Utilities.Testing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Plexdata.CsvParser.Tests.Processors
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    [Category(TestType.UnitTest)]
    [TestOf(nameof(CsvReader))]
    public class CsvReaderTests
    {
        [TestCase(null, TestName = "Read(Filename: null, Settings: null)")]
        [TestCase("", TestName = "Read(Filename: empty, Settings: null)")]
        [TestCase("   ", TestName = "Read(Filename: whitespace, Settings: null)")]
        public void Read_InvalidFilename_ThrowsArgumentException(String filename)
        {
            CsvSettings settings = null;
            Assert.That(() => CsvReader.Read(filename, settings), Throws.ArgumentException);
        }

        [TestCase(Category = TestType.IntegrationTest)]
        public void Read_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            String filename = @"c:\temp\missing-file.csv";
            CsvSettings settings = null;
            Assert.That(() => CsvReader.Read(filename, settings), Throws.InstanceOf<FileNotFoundException>());
        }

        [Test]
        public void Read_StreamIsNull_ThrowsArgumentNullException()
        {
            Stream stream = null;
            CsvSettings settings = null;

            Assert.That(() => CsvReader.Read(stream, settings), Throws.ArgumentNullException);
        }

        [TestCase(Category = TestType.IntegrationTest)]
        public void Read_StreamCanWriteOnly_ThrowsArgumentException()
        {
            String filename = Path.GetTempFileName();
            Stream stream = null;
            CsvSettings settings = null;

            try
            {
                stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);

                Assert.That(() => CsvReader.Read(stream, settings), Throws.ArgumentException);

                this.CleanUp(stream, filename);
            }
            catch
            {
                this.CleanUp(stream, filename);
                throw;
            }
        }

        [Test]
        public void Read_SettingsAreNull_ThrowsArgumentNullException()
        {
            using (Stream stream = new MemoryStream())
            {
                CsvSettings settings = null;
                Assert.That(() => CsvReader.Read(stream, settings), Throws.ArgumentNullException);
            }
        }

        [Test]
        [SetUICulture("de-DE")]
        public void Read_ColumnCountMismatch_ResultAsExpected()
        {
            String content =
                "Label,Enabled,Number,Currency\r\n" +
                "Label-1,true,42,\"1,234\"\r\n" +
                "Label-2,false,23\r\n";

            CsvSettings settings = new CsvSettings() { Heading = true };

            using (MemoryStream stream = new MemoryStream(settings.Encoding.GetBytes(content)))
            {
                CsvContainer actual = CsvReader.Read(stream, settings);

                Assert.That(actual.GetValue<String>("label", 0), Is.EqualTo("Label-1"));
                Assert.That(actual.GetValue<String>("label", 1), Is.EqualTo("Label-2"));

                Assert.That(actual.GetValue<Boolean>("ENABLED", 0), Is.EqualTo(true));
                Assert.That(actual.GetValue<Boolean>("ENABLED", 1), Is.EqualTo(false));

                Assert.That(actual.GetValue<Int32>("NumBER", 0), Is.EqualTo(42));
                Assert.That(actual.GetValue<Int32>("NumBER", 1), Is.EqualTo(23));

                Assert.That(actual.GetValue<Double>("Currency", 0), Is.EqualTo(1.234));
                Assert.That(actual.GetValue<Double>("Currency", 1), Is.EqualTo(null));
            }
        }

        [TestCase("header-1;header-2;header-3;\r12345;\"\"\"aaa\"\"  \r  \\\"bbb\"\" \r \\\"ccc\\\"\";67890;\r")]
        [TestCase("header-1;header-2;header-3;\n12345;\"\"\"aaa\"\"  \n  \\\"bbb\"\" \n \\\"ccc\\\"\";67890;\n")]
        [TestCase("header-1;header-2;header-3;\r\n12345;\"\"\"aaa\"\"  \r\n  \\\"bbb\"\" \r\n \\\"ccc\\\"\";67890;\r\n")]
        public void Read_ColumnsWithMultipleLinesInStrings_ResultAsExpected(String content)
        {
            String expected = $"\"aaa\"  {Environment.NewLine}  \"bbb\" {Environment.NewLine} \"ccc\"";

            CsvSettings settings = new CsvSettings() { Heading = true, Separator = ';' };

            using (MemoryStream stream = new MemoryStream(settings.Encoding.GetBytes(content)))
            {
                CsvContainer actual = CsvReader.Read(stream, settings);

                Assert.That(actual.Content[1][1], Is.EqualTo(expected));
            }
        }

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
