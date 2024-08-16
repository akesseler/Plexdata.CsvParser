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

namespace Plexdata.CsvParser.Tests.Processors
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    [Category(TestType.UnitTest)]
    [TestOf(nameof(CsvSettings))]
    public class CsvSettingsTests
    {
        [Test]
        public void Construction_ApplyDefaultValues_ResultIsEqual()
        {
            String expected = "Separator: \",\", Encoding: \"utf-8\", Heading: \"True\", Textual: \"False\", Exactly: \"False\", Culture: \"de-DE\", Mappings: TrueValue: \"true\", FalseValue: \"false\", NullValue: \"\", TrueValues: [\"true\", \"1\", \"y\", \"yes\"], FalseValues: [\"false\", \"0\", \"n\", \"no\"], NullValues: [\"<null>\"]";
            CsvSettings settings = new CsvSettings();
            Assert.That(settings.ToString(), Is.EqualTo(expected));
        }

        [TestCase('\0')]
        [TestCase('\r')]
        [TestCase('\n')]
        public void Separator_InvalidValues_ThrowsArgumentException(Char separator)
        {
            CsvSettings settings = new CsvSettings();
            Assert.That(() => settings.Separator = separator, Throws.ArgumentException);
        }

        [TestCase(',')]
        [TestCase(':')]
        [TestCase('\t')]
        [TestCase('#')]
        [TestCase('~')]
        public void Separator_ValidValues_ResultIsEqual(Char separator)
        {
            CsvSettings settings = new CsvSettings { Separator = separator };
            Assert.That(settings.Separator, Is.EqualTo(separator));
        }

        [Test]
        public void Encoding_ValueIsNull_ThrowsArgumentNullException()
        {
            CsvSettings settings = new CsvSettings();
            Assert.That(() => settings.Encoding = null, Throws.ArgumentNullException);
        }

        [Test]
        public void Culture_ValueIsNull_ThrowsArgumentNullException()
        {
            CsvSettings settings = new CsvSettings();
            Assert.That(() => settings.Culture = null, Throws.ArgumentNullException);
        }
    }
}
