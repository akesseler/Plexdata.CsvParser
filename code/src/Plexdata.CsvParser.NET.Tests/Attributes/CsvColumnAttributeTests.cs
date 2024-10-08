﻿/*
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
using Plexdata.Utilities.Testing;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plexdata.CsvParser.Tests.Attributes
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    [Category(TestType.UnitTest)]
    [TestOf(nameof(CsvColumnAttribute))]
    public class CsvColumnAttributeTests
    {
        [Test]
        public void Construction_DefaultConstructor_ResultDoesNotThrow()
        {
            Assert.That(() => new CsvColumnAttribute(), Throws.Nothing);
        }

        [Test]
        public void Construction_DefaultConstructor_ResultIsTrue()
        {
            CsvColumnAttribute attribute = new CsvColumnAttribute();
            Assert.That(attribute.Header == String.Empty && attribute.Offset == -1, Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("test-header")]
        public void Header_SetValue_ResultAreEqual(String expected)
        {
            CsvColumnAttribute attribute = new CsvColumnAttribute();
            attribute.Header = expected;
            Assert.That(attribute.Header, Is.EqualTo(expected));
        }

        [TestCase(-42)]
        [TestCase(42)]
        public void Offset_SetValue_ResultAreEqual(Int32 expected)
        {
            CsvColumnAttribute attribute = new CsvColumnAttribute();
            attribute.Offset = expected;
            Assert.That(attribute.Offset, Is.EqualTo(expected));
        }
    }
}
