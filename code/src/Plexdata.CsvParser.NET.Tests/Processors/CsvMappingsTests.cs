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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Plexdata.CsvParser.Tests.Processors
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    [Category(TestType.UnitTest)]
    [TestOf(nameof(CsvMappings))]
    public class CsvMappingsTests
    {
        [Test]
        public void Construction_ApplyDefaultValues_ResultIsEqual()
        {
            String expected = "TrueValue: \"true\", FalseValue: \"false\", NullValue: \"\", TrueValues: [\"true\", \"1\", \"y\", \"yes\"], FalseValues: [\"false\", \"0\", \"n\", \"no\"], NullValues: [\"<null>\"]";
            CsvMappings mapping = new CsvMappings();
            Assert.That(mapping.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void Construction_ApplyOtherValues_ResultIsEqual()
        {
            String expected = "TrueValue: \"wow\", FalseValue: \"awesome\", NullValue: \"don't care\", TrueValues: [\"yes\", \"yepp\"], FalseValues: [\"no\", \"nope\"], NullValues: [\"hello\", \"empty\"]";
            CsvMappings mapping = new CsvMappings()
            {
                TrueValue = "wow",
                FalseValue = "awesome",
                NullValue = "don't care",
                TrueValues = new List<String> { "yes", "yepp" },
                FalseValues = new List<String> { "no", "nope" },
                NullValues = new List<String> { "hello", "empty" },
            };
            Assert.That(mapping.ToString(), Is.EqualTo(expected));
        }
    }
}
