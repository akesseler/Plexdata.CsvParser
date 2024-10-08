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
using Plexdata.CsvParser.Internals;
using Plexdata.Utilities.Testing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Plexdata.CsvParser.Tests.Internals
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    [Category(TestType.UnitTest)]
    [TestOf(nameof(ItemDescriptor))]
    public class ItemDescriptorTests
    {
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void Construction_ItemDescriptor_ThrowsArgumentNullException(Int32 column, Int32 origin)
        {
            Assert.That(() => new ItemDescriptor(column == 0 ? null : new CsvColumnAttribute(), origin == 0 ? null : new PropertyInfoTest()), Throws.ArgumentNullException);
        }

        [Test]
        public void Construction_ItemDescriptor_ResultIsColumnAreEqual()
        {
            CsvColumnAttribute column = new CsvColumnAttribute();
            ItemDescriptor actual = new ItemDescriptor(column, new PropertyInfoTest());
            Assert.That(actual.Column, Is.EqualTo(column));
        }

        [Test]
        public void Construction_ItemDescriptor_ResultIsOriginAreEqual()
        {
            PropertyInfoTest origin = new PropertyInfoTest();
            ItemDescriptor actual = new ItemDescriptor(new CsvColumnAttribute(), origin);
            Assert.That(actual.Origin, Is.EqualTo(origin));
        }

        [TestCase("CanNeverBeNull", null, -1)]
        [TestCase("CanNeverBeNull", "", -1)]
        [TestCase("CanNeverBeNull", "  \t \r\n \v", -1)]
        [TestCase("CanNeverBeNull", "test-header", -1)]
        [TestCase("CanNeverBeNull", null, 42)]
        [TestCase("CanNeverBeNull", "", 42)]
        [TestCase("CanNeverBeNull", "  \t \r\n \v", 42)]
        [TestCase("CanNeverBeNull", "test-header", 42)]
        public void ToString_ItemDescriptor_ResultAreEqual(String originName, String headerName, Int32 offsetValue)
        {
            headerName = String.IsNullOrWhiteSpace(headerName) ? "<null>" : headerName;
            PropertyInfoTest origin = new PropertyInfoTest { name = $"{originName}", };
            CsvColumnAttribute column = new CsvColumnAttribute { Header = $"{headerName}", Offset = offsetValue, };
            String expected = $"Offset: \"{offsetValue}\", Header: \"{headerName}\", Origin: \"{originName}\"";
            ItemDescriptor actual = new ItemDescriptor(column, origin);
            Assert.That(actual.ToString(), Is.EqualTo(expected));
        }

        private class PropertyInfoTest : PropertyInfo
        {
            internal String name = null;

            public override Type PropertyType => throw new NotImplementedException();

            public override PropertyAttributes Attributes => throw new NotImplementedException();

            public override Boolean CanRead => throw new NotImplementedException();

            public override Boolean CanWrite => throw new NotImplementedException();

            public override String Name { get { return this.name; } }

            public override Type DeclaringType => throw new NotImplementedException();

            public override Type ReflectedType => throw new NotImplementedException();

            public override MethodInfo[] GetAccessors(Boolean nonPublic)
            {
                throw new NotImplementedException();
            }

            public override Object[] GetCustomAttributes(Boolean inherit)
            {
                throw new NotImplementedException();
            }

            public override Object[] GetCustomAttributes(Type attributeType, Boolean inherit)
            {
                throw new NotImplementedException();
            }

            public override MethodInfo GetGetMethod(Boolean nonPublic)
            {
                throw new NotImplementedException();
            }

            public override ParameterInfo[] GetIndexParameters()
            {
                throw new NotImplementedException();
            }

            public override MethodInfo GetSetMethod(Boolean nonPublic)
            {
                throw new NotImplementedException();
            }

            public override Object GetValue(Object obj, BindingFlags invokeAttr, Binder binder, Object[] index, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            public override Boolean IsDefined(Type attributeType, Boolean inherit)
            {
                throw new NotImplementedException();
            }

            public override void SetValue(Object obj, Object value, BindingFlags invokeAttr, Binder binder, Object[] index, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
