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
using Plexdata.CsvParser.Attributes;
using Plexdata.CsvParser.Internals;
using System;

namespace Plexdata.CsvParser.Tests.Internals
{
    [TestFixture]
    [TestOf(nameof(TypeDescriptor))]
    public class TypeDescriptorTests
    {
        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void Construction_TypeDescriptor_ThrowsArgumentNullException(Int32 document, Int32 settings)
        {
            Assert.Throws<ArgumentNullException>(() => { new TypeDescriptor(document == 0 ? null : new CsvDocumentAttribute(), settings == 0 ? null : new ItemDescriptor[0]); });
        }

        [Test]
        public void Construction_TypeDescriptor_ResultIsDocumentAreEqual()
        {
            CsvDocumentAttribute document = new CsvDocumentAttribute();
            TypeDescriptor actual = new TypeDescriptor(document, new ItemDescriptor[0]);
            Assert.AreEqual(document, actual.Document);
        }

        [Test]
        public void Construction_TypeDescriptor_ResultIsOriginAreEqual()
        {
            ItemDescriptor[] settings = new ItemDescriptor[0];
            TypeDescriptor actual = new TypeDescriptor(new CsvDocumentAttribute(), settings);
            Assert.AreEqual(settings, actual.Settings);
        }
    }
}
