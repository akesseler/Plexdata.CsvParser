/*
 * MIT License
 * 
 * Copyright (c) 2018 plexdata.de
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

using System;

namespace Plexdata.CsvParser.Tester
{
    class Program
    {
        /* Load CSV file
        [CsvDocument]
        class CsvCustomer
        {
            [CsvIgnore]
            public Int32 Id { get; set; }

            [CsvColumn(Offset = 2,  Header = "Identifier")]
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
        }
        
        static void Main(string[] args)
        {
            try
            {
                CsvSettings settings = new CsvSettings
                {
                    Culture = CultureInfo.GetCultureInfo("en-US"),
                    Mappings = new CsvMappings
                    {
                        TrueValues = new List<String> { "yeah" },
                        FalseValues = new List<String> { "nope" },
                    },
                };

                // Source file could contain this content:
                // Surname,  Forename,    Identifier, Date,       Sales,      Active, Notes
                // "Marley", "Bob",       1001,       2007-05-03, "1,234.56", nope,   "Have a short note here."
                // "Monroe", "Marilyn",   1002,       2008-06-05, "1,234.56", nope,   ""
                // "Snipes", "Wesley",    1003,       2009-07-06, "1,234.56", yeah,   "Have a short note here." 
                // "Hurley", "Elizabeth", 1004,       2005-08-08, "1,234.56", yeah,   "Have a short note here."

                IEnumerable<CsvCustomer> result = CsvImporter<CsvCustomer>.Load(filename, settings);

                foreach (CsvCustomer current in result)
                {
                    Console.WriteLine(current.ToString());
                }

                Console.ReadKey();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        */

        /* Save CSV file
        [CsvDocument]
        class CsvCustomer
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
        }

        static void Main(string[] args)
        {
            try
            {
                List<CsvCustomer> customers = new List<CsvCustomer>
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

                // Output file would contain this content:
                // Surname,Forename,Identifier,Date,Sales,Active,Notes
                // "Marley","Bob",1001,2007-05-03T00:00:00,1234.56,nope,"Have a short note here."
                // "Monroe","Marilyn",1002,2008-06-05T00:00:00,1234.56,nope,
                // "Snipes","Wesley",1003,2009-07-06T00:00:00,1234.56,yeah,"Have a short note here."
                // "Hurley","Elizabeth",1004,2005-08-08T00:00:00,1234.56,yeah,"Have a short note here."

                CsvExporter<CsvCustomer>.Save(customers, filename, settings);

                Console.ReadKey();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        */

        static void Main(string[] args)
        {
            Console.WriteLine("Do something useful...");
            Console.ReadKey();
        }
    }
}
