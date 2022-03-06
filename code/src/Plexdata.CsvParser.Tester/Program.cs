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
        
        static void Main(String[] args)
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

        static void Main(String[] args)
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
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        */

        /* Read CSV file
        static void Main(String[] args)
        {
            try
            {
                // Source file could contain this content:
                // Name;               Notes
                // "Marley, Bob";      "Jamaican singer-songwriter"
                // "Monroe, Marilyn";  "American actress";          "model and singer"
                // "Snipes, Wesley";   "American actor";            "director, film producer"; "martial artist"
                // "Hurley, Elizabeth" 

                CsvSettings settings = new CsvSettings() { Heading = true, Separator = ColumnSeparators.SemicolonSeparator };
                CsvContainer container = CsvReader.Read(filename, settings);

                String col0row1 = container.GetValue<String>(0, 1) as String; // Marley, Bob
                String col0row2 = container.GetValue<String>(0, 2) as String; // Monroe, Marilyn
                String col0row3 = container.GetValue<String>(0, 3) as String; // Snipes, Wesley
                String col0row4 = container.GetValue<String>(0, 4) as String; // Hurley, Elizabeth

                String col1row1 = container.GetValue<String>(1, 1) as String; // Jamaican singer-songwriter
                String col1row2 = container.GetValue<String>(1, 2) as String; // American actress
                String col1row3 = container.GetValue<String>(1, 3) as String; // American actor
                String col1row4 = container.GetValue<String>(1, 4) as String; // null

                String col2row1 = container.GetValue<String>(2, 1) as String; // null
                String col2row2 = container.GetValue<String>(2, 2) as String; // model and singer
                String col2row3 = container.GetValue<String>(2, 3) as String; // director, film producer
                String col2row4 = container.GetValue<String>(2, 4) as String; // null

                String col3row1 = container.GetValue<String>(3, 1) as String; // null
                String col3row2 = container.GetValue<String>(3, 2) as String; // null
                String col3row3 = container.GetValue<String>(3, 3) as String; // martial artist
                String col3row4 = container.GetValue<String>(3, 4) as String; // null
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        */

        /* Write CSV file (Part I)
        static void Main(String[] args)
        {
            try
            {
                // Output file would contain this content:
                // Name;Notes
                // "Marley, Bob";"Jamaican singer-songwriter"
                // "Monroe, Marilyn";"American actress";"model and singer"
                // "Snipes, Wesley";"American actor";"director, film producer";"martial artist"
                // "Hurley, Elizabeth" 

                List<List<Object>> content = new List<List<Object>>
                {
                    new List<Object> { "Name", "Notes" },
                    new List<Object> { "Marley, Bob", "Jamaican singer-songwriter" },
                    new List<Object> { "Monroe, Marilyn", "American actress", "model and singer" },
                    new List<Object> { "Snipes, Wesley", "American actor", "director, film producer", "martial artist" },
                    new List<Object> { "Hurley, Elizabeth" }
                };

                CsvSettings settings = new CsvSettings() { Heading = true, Textual = true, Separator = ColumnSeparators.SemicolonSeparator };
                CsvWriter.Write(content, filename, settings);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        */

        /* Write CSV file (Part II)
        static void Main(String[] args)
        {
            try
            {
                // Create a container with two columns and five lines.
                CsvContainer container = new CsvContainer(2, 5);

                // Set both column headers.
                container[0, 0] = "Name";
                container[1, 0] = "Notes";

                // Access each single line and apply their values.
                // Be aware, with Heading disabled and accessing a
                // column by number requires to start at index one.
                container[0, 1] = "Marley, Bob";
                container[1, 1] = "Jamaican singer-songwriter";
                container[0, 2] = "Monroe, Marilyn";
                container[1, 2] = "American actress";
                container[0, 3] = "Snipes, Wesley";
                container[1, 3] = "American actor";
                container[0, 4] = "Hurley, Elizabeth";
                container[1, 4] = "American actress";

                // Output file would contain this content:
                // Name,Notes
                // "Marley, Bob",Jamaican singer-songwriter
                // "Monroe, Marilyn",American actress
                // "Snipes, Wesley",American actor
                // "Hurley, Elizabeth",American actress

                CsvWriter.Write(container, filename);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        */

        /* Write CSV file (Part III)
        static void Main(String[] args)
        {
            try
            {
                // Create default settings but with Heading enabled.
                CsvSettings settings = new CsvSettings() { Heading = true };

                // Create a container with two columns and five lines.
                CsvContainer container = new CsvContainer(2, 5, settings);

                // Set both column headers.
                container[0, 0] = "Name";
                container[1, 0] = "Notes";

                // Access each single line and apply their values.
                // Be aware, with Heading enabled and accessing a
                // column by header requires to start at index zero.
                container["Name", 0] = "Marley, Bob";
                container["Notes", 0] = "Jamaican singer-songwriter";
                container["Name", 1] = "Monroe, Marilyn";
                container["Notes", 1] = "American actress";
                container["Name", 2] = "Snipes, Wesley";
                container["Notes", 2] = "American actor";
                container["Name", 3] = "Hurley, Elizabeth";
                container["Notes", 3] = "American actress";

                // Output file would contain this content:
                // Name,Notes
                // "Marley, Bob",Jamaican singer-songwriter
                // "Monroe, Marilyn",American actress
                // "Snipes, Wesley",American actor
                // "Hurley, Elizabeth",American actress

                CsvWriter.Write(container, filename, settings);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        */

        static void Main(String[] args)
        {
            Console.WriteLine("Do something useful...");
            Console.ReadKey();
        }
    }
}
