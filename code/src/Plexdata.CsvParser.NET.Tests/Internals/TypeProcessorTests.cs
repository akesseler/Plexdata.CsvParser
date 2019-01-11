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
using Plexdata.CsvParser.Internals;
using System;
using System.Linq;

namespace Plexdata.CsvParser.Tests.Attributes
{
    [TestFixture]
    [TestOf(nameof(TypeProcessor))]
    public class TypeProcessorTests
    {
        [Test]
        public void LoadDescriptor_InstanceIsValid_ResultIsSettingsCountZero()
        {
            TypeDescriptor actual = TypeProcessor.LoadDescriptor<TestClassUntagged>();
            Assert.AreEqual(0, actual.Settings.Count());
        }

        [Test]
        public void LoadDescriptor_InstanceIsValid_ResultIsSettingsCount5()
        {
            TypeDescriptor actual = TypeProcessor.LoadDescriptor<TestClassWithTags>();
            Assert.AreEqual(5, actual.Settings.Count());
        }

        [Test]
        public void LoadDescriptor_ConfirmAndReorganizeDuplicates_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => { TypeProcessor.LoadDescriptor<TestClassWithDuplicates>(); });
        }

        [Test]
        public void LoadDescriptor_ConfirmAndReorganizeSteppings_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => { TypeProcessor.LoadDescriptor<TestClassWithSteppings>(); });
        }

        [Test]
        public void LoadDescriptor_ConfirmAndReorganizeMixings_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => { TypeProcessor.LoadDescriptor<TestClassWithMixings>(); });
        }

        [Test]
        public void LoadDescriptor_ConfirmAndReorganizeUnordered_ResultIsOrdered()
        {
            String expected =
                "Offset: \"0\", Header: \"<null>\", Origin: \"Property2\"" +
                "Offset: \"1\", Header: \"<null>\", Origin: \"Property1\"" +
                "Offset: \"2\", Header: \"<null>\", Origin: \"Property4\"" +
                "Offset: \"3\", Header: \"<null>\", Origin: \"Property3\"";

            TypeDescriptor instance = TypeProcessor.LoadDescriptor<TestClassUnordered>();

            Assert.AreEqual(expected, String.Join("", instance.Settings));
        }

        private class TestClassUntagged
        {
            private static Int32 Property1 { get; set; } // -
            public static Int32 Property2 { get; set; }  // -
            public Int32 Property3 { get; set; }         // +
            public Int32 Property4 { get; set; }         // +
            public Int32 Property5 { get; set; }         // +
            public Int32 Property6 { get; }              // +
            public Int32 Property7 { get; set; }         // +
            private Int32 Property8 { get; set; }        // -
            private Int32 Property9 { get; set; }        // -
        }

        private class TestClassWithTags
        {
            [CsvColumn] private static Int32 Property1 { get; set; } // -
            [CsvColumn] public static Int32 Property2 { get; set; }  // -
            [CsvColumn] public Int32 Property3 { get; set; }         // +
            [CsvColumn] public Int32 Property4 { get; set; }         // +
            [CsvColumn] public Int32 Property5 { get; set; }         // +
            [CsvColumn] public Int32 Property6 { get; }              // +
            [CsvColumn] public Int32 Property7 { get; set; }         // +
            [CsvColumn] private Int32 Property8 { get; set; }        // -
            [CsvColumn] private Int32 Property9 { get; set; }        // -
        }

        private class TestClassWithDuplicates
        {
            [CsvColumn(Offset = 0)] public Int32 Property1 { get; set; }
            [CsvColumn(Offset = 1)] public Int32 Property2 { get; set; }
            [CsvColumn(Offset = 2)] public Int32 Property3 { get; set; }
            [CsvColumn(Offset = 2)] public Int32 Property4 { get; set; }
        }

        private class TestClassWithSteppings
        {
            [CsvColumn(Offset = 0)] public Int32 Property1 { get; set; }
            [CsvColumn(Offset = 1)] public Int32 Property2 { get; set; }
            [CsvColumn(Offset = 2)] public Int32 Property3 { get; set; }
            [CsvColumn(Offset = 8)] public Int32 Property4 { get; set; }
        }

        private class TestClassWithMixings
        {
            [CsvColumn(Offset = -1)] public Int32 Property1 { get; set; }
            [CsvColumn(Offset = 0)] public Int32 Property2 { get; set; }
            [CsvColumn(Offset = -2)] public Int32 Property3 { get; set; }
            [CsvColumn(Offset = 1)] public Int32 Property4 { get; set; }
        }

        private class TestClassUnordered
        {
            [CsvColumn(Offset = 1)] public Int32 Property1 { get; set; }
            [CsvColumn(Offset = 0)] public Int32 Property2 { get; set; }
            [CsvColumn(Offset = 3)] public Int32 Property3 { get; set; }
            [CsvColumn(Offset = 2)] public Int32 Property4 { get; set; }
        }
    }
}

