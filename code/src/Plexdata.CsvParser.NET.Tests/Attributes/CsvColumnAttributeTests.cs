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
using System;

namespace Plexdata.CsvParser.Tests.Attributes
{
    [TestFixture]
    [TestOf(nameof(CsvColumnAttribute))]
    public class CsvColumnAttributeTests
    {
        [Test]
        public void Construction_DefaultConstructor_ResultDoesNotThrow()
        {
            Assert.DoesNotThrow(() => { new CsvColumnAttribute(); });
        }

        [Test]
        public void Construction_DefaultConstructor_ResultIsTrue()
        {
            CsvColumnAttribute attribute = new CsvColumnAttribute();
            Assert.IsTrue(attribute.Header == String.Empty && attribute.Offset == -1);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("test-header")]
        public void Header_SetValue_ResultAreEqual(String expected)
        {
            CsvColumnAttribute attribute = new CsvColumnAttribute();
            attribute.Header = expected;
            Assert.AreEqual(expected, attribute.Header);
        }

        [Test]
        [TestCase(-42)]
        [TestCase(42)]
        public void Offset_SetValue_ResultAreEqual(Int32 expected)
        {
            CsvColumnAttribute attribute = new CsvColumnAttribute();
            attribute.Offset = expected;
            Assert.AreEqual(expected, attribute.Offset);
        }
    }
}
