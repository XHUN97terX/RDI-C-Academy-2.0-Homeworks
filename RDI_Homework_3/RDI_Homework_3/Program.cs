using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RDI_Homework_3
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Feldolgozás alatt...");
            var xdoc = XDocument.Load("mondial-3.0.xml");
            //hogy tizedespontok legyenek...
            System.Threading.Thread.CurrentThread.CurrentCulture =
                System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
            //ToList hívás, hogy lefutásra kényszerüljön
            var countries =
                (from country in xdoc.Descendants("country")
                select new Mondial.Country()
                {
                    ID = country.Attribute("id").Value,
                    Name = country.Attribute("name").Value,
                    Government = country.Attribute("government").Value,
                    Population = int.Parse(country.Attribute("population").Value),
                    TotalArea = int.Parse(country.Attribute("total_area").Value),
                    BorderLength = (from border in country.Descendants("border")
                                    select double.Parse(border.Attribute("length").Value)).Sum(),
                    EthnicGroups = from ethnic in country.Descendants("ethnicgroups")
                                   select new Mondial.EthnicGroup()
                                   {
                                       Name = ethnic.Value,
                                       Percentage = double.Parse(ethnic.Attribute("percentage").Value)
                                   },
                    Religions = from religion in country.Descendants("religions")
                                select new Mondial.Religion()
                                {
                                    Name = religion.Value,
                                    Percentage = double.Parse(religion.Attribute("percentage").Value)
                                }
                }).ToList();
            var seas = (from sea in xdoc.Descendants("sea")
                       select new Mondial.Sea()
                       {
                           ID = sea.Attribute("id").Value,
                           Name = sea.Attribute("name").Value,
                           Located = WaterBodyLocatedFromXML(sea, countries)
                       }).ToList();
            var lakes = (from lake in xdoc.Descendants("lake")
                        select new Mondial.Lake()
                        {
                            ID = lake.Attribute("id").Value,
                            Name = lake.Attribute("name").Value,
                            Located = WaterBodyLocatedFromXML(lake, countries)
                        }).ToList();
            var rivers = (from river in xdoc.Descendants("river")
                         select new Mondial.River()
                         {
                             ID = river.Attribute("id").Value,
                             Name = river.Attribute("name").Value,
                             Located = WaterBodyLocatedFromXML(river, countries),
                             To = seas.ToList().Find((x) => x.ID == river.Element("to").Attribute("water").Value)
                         }).ToList();

            foreach (var river in rivers)
                if (river.To != null)
                    river.To.RiverInto++;

            List<Mondial.Ethnics> ethnics = new List<Mondial.Ethnics>();
            foreach (var country in countries)
            {
                Mondial.Ethnics k;
                foreach (var ethnic in country.EthnicGroups)
                {
                    if ((k = ethnics.Find(x => ethnic.Name == x.Name)) == null)
                        ethnics.Add(new Mondial.Ethnics() { Name = ethnic.Name, Count = (long)(ethnic.Percentage * country.Population) });
                    else
                        k.Count += (long)(ethnic.Percentage * country.Population);
                }
            }

            var legnagyobbNepsuruseg = (from country in countries
                                        orderby country.Population / country.TotalArea descending
                                        select new
                                        {
                                            Name = country.Name,
                                            PopulationDensity = country.Population / country.TotalArea
                                        }).First();
            var demokraciaSzazalek = countries.Count((x) => x.Government.Contains("democracy")) * 100 / (double)countries.Count();
            var vallasiEsEtnikaiSzam = from country in countries
                                       orderby country.EthnicGroups.Count() + country.Religions.Count() descending
                                       select new {
                                           Country = country.Name,
                                           NumberOfEthnics = country.EthnicGroups.Count(),
                                           NumberOfReligions = country.Religions.Count()
                                       };
            var hatarokHossza = from country in countries
                                select new
                                {
                                    Country = country.Name,
                                    BorderLength = country.BorderLength
                                };
            var legtobbOrszagotMetszo5 = (from river in rivers
                                          orderby river.Located.Count() descending
                                          select new
                                          {
                                              Name = river.Name,
                                              GoesThrough = river.Located.Count()
                                          }).Take(5).ToList();      //ToList, mert lassú
            var legtobbViz5 = (from country in countries
                               orderby country.WaterBodyCount descending
                               select new
                               {
                                   Name = country.Name,
                                   Count = country.WaterBodyCount
                               }).Take(5);
            var legtobbFolyoBele = (from sea in seas
                                    orderby sea.RiverInto descending
                                    select new
                                    {
                                        Name = sea.Name,
                                        Count = sea.RiverInto
                                    }).Take(5);                 //nincs megadva szám, 5 jó volt eddig...
            var ethnicsNumber = from ethnic in ethnics
                                orderby ethnic.Name
                                select new
                                {
                                    Name = ethnic.Name,
                                    Count = ethnic.Count
                                };                              //csak LINQ-val akartam 'megoldani' :)
            Console.BufferHeight = 1000;
            Console.Clear();
            Console.WriteLine("Feldolgozva!\nA kérdések után nyomjon meg egy gombot, hogy a következő kérdés megjelenjen!");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("1. Melyik ország népességsűrűsége a legnagyobb?\n");
            Console.WriteLine("A legnagyobb népsűrűségű ország: {0}\nLakossága: {1}", legnagyobbNepsuruseg.Name, legnagyobbNepsuruseg.PopulationDensity);
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("2. Az országok hány százalékában van valamilyen formájú demokrácia?\n");
            Console.WriteLine("Az országok {0:00.00}%-ban.", demokraciaSzazalek);
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("3. Vallási és etnikai csoportok száma országonként, összegük szerinti csökkenő sorrendben!\n");
            Console.WriteLine(
                ArrayPrinter.Printer.Print(
                    vallasiEsEtnikaiSzam,
                    (x) => string.Format("{0};{1};{2}", x.Country, x.NumberOfEthnics, x.NumberOfReligions),
                    "Ország", "Etnikumok", "Vallások"));
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("4. Határok hossza országonként!\n");
            Console.WriteLine(
                ArrayPrinter.Printer.Print(
                    hatarokHossza,
                    (x) => string.Format("{0};{1}km", x.Country, x.BorderLength),
                    "Ország", "Hossz"));
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("5. A legtöbb országot metsző 5 folyó!\n");
            Console.WriteLine(
                ArrayPrinter.Printer.Print(
                    legtobbOrszagotMetszo5,
                    x => string.Format("{0};{1}", x.Name, x.GoesThrough),
                    "Folyó", "MetszésSzám"));
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("6. Azon 5 ország ahol a legnagyobb számban találunk folyót, tavat és tengert összesen!\n");
            Console.WriteLine(
                ArrayPrinter.Printer.Print(
                    legtobbViz5,
                    x => string.Format("{0};{1}", x.Name, x.Count),
                    "Ország", "Mennyiség"));
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("7. Mely tengerekbe folyik a legtöbb folyó?\n");
            Console.WriteLine(
                ArrayPrinter.Printer.Print(
                    legtobbFolyoBele,
                    x => string.Format("{0};{1}", x.Name, x.Count),
                    "Tenger/Óceán", "Folyók száma"));
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("8. A világ etnikumainak mérete!\n");
            Console.WriteLine(
                ArrayPrinter.Printer.Print(
                    ethnicsNumber,
                    (x => string.Format("{0};{1}", x.Name, x.Count)),
                    "Etnikum", "Fő"));
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("Nincs több kérdés.\nNyomjon meg egy gombot, hogy bezárja az alkalmazást.");
            Console.ReadKey();

        }
        //local function lenne alapból, de ahol írom a kódot, nem elég új a Visual Studio...
        //csúnya lenne helyben deklarálni a functiont, így azt is kihagytam inkább
        static IEnumerable<Mondial.Country> WaterBodyLocatedFromXML(XElement element, IEnumerable<Mondial.Country> countries)
        {
            var located = new List<Mondial.Country>();
            var countryIDs = (from loc in element.Descendants("located")
                              select loc.Attribute("country").Value).Distinct();
            foreach (var c in countryIDs)
            {
                var match = countries.ToList().Find((x) => x.ID == c);
                if (match != null && !located.Contains(match))
                {
                    match.WaterBodyCount++;     //csak folyón, tavon, tengeren van használva
                    located.Add(match);
                }
            }
            return located;
        }
    }
}
namespace Mondial
{
    class Country
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Government { get; set; }
        public int Population { get; set; }
        public int TotalArea { get; set; }
        public int WaterBodyCount { get; set; }
        public double BorderLength { get; set; }
        public IEnumerable<EthnicGroup> EthnicGroups { get; set; }
        public IEnumerable<Religion> Religions { get; set; }
    }
    class EthnicGroup
    {
        public string Name { get; set; }
        public double Percentage { get; set; }
    }
    class Religion
    {
        public string Name { get; set; }
        public double Percentage { get; set; }
    }
    class River
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<Country> Located { get; set; }
        public Sea To { get; set; }
    }
    class Sea
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int RiverInto { get; set; }
        public IEnumerable<Country> Located { get; set; }
    }
    class Lake
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<Country> Located { get; set; }
    }
    class Ethnics
    {
        public string Name { get; set; }
        public long Count { get; set; }
    }
}

namespace ArrayPrinter
{
    static class Printer
    {
        /// <summary>
        /// Easy to use 'interface' for the ArrayPrinter class.
        /// </summary>
        /// <typeparam name="T">Type of items to be printed.</typeparam>
        /// <param name="enumerable">The datacollection that needs to be printed.</param>
        /// <param name="CSVLikeFormater">Creates ';' separated string from the fields of T.</param>
        /// <param name="header">The header for the table to be made.</param>
        public static string Print<T>(IEnumerable<T> enumerable, Func<T, string> CSVLikeFormater, params string[] header)
        {
            List<string[]> k = new List<string[]>();
            k.Add(header);
            foreach (var e in enumerable)
                k.Add(CSVLikeFormater(e).Split(';'));
            return ArrayPrinter.GetDataInTableFormat(k);
        } 
    }

    //Taken from StackOverflow:
    //https://stackoverflow.com/a/14698822
    public class ArrayPrinter
    {
        const string TOP_LEFT_JOINT = "┌";
        const string TOP_RIGHT_JOINT = "┐";
        const string BOTTOM_LEFT_JOINT = "└";
        const string BOTTOM_RIGHT_JOINT = "┘";
        const string TOP_JOINT = "┬";
        const string BOTTOM_JOINT = "┴";
        const string LEFT_JOINT = "├";
        const string JOINT = "┼";
        const string RIGHT_JOINT = "┤";
        const char HORIZONTAL_LINE = '─';
        const char PADDING = ' ';
        const string VERTICAL_LINE = "│";

        private static int[] GetMaxCellWidths(List<string[]> table)
        {
            int maximumCells = 0;
            foreach (Array row in table)
            {
                if (row.Length > maximumCells)
                    maximumCells = row.Length;
            }

            int[] maximumCellWidths = new int[maximumCells];
            for (int i = 0; i < maximumCellWidths.Length; i++)
                maximumCellWidths[i] = 0;

            foreach (Array row in table)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    if (row.GetValue(i).ToString().Length > maximumCellWidths[i])
                        maximumCellWidths[i] = row.GetValue(i).ToString().Length;
                }
            }

            return maximumCellWidths;
        }

        public static string GetDataInTableFormat(List<string[]> table)
        {
            StringBuilder formattedTable = new StringBuilder();
            Array nextRow = table.FirstOrDefault();
            Array previousRow = table.FirstOrDefault();

            if (table == null || nextRow == null)
                return String.Empty;

            // FIRST LINE:
            int[] maximumCellWidths = GetMaxCellWidths(table);
            for (int i = 0; i < nextRow.Length; i++)
            {
                if (i == 0 && i == nextRow.Length - 1)
                    formattedTable.Append(String.Format("{0}{1}{2}", TOP_LEFT_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), TOP_RIGHT_JOINT));
                else if (i == 0)
                    formattedTable.Append(String.Format("{0}{1}", TOP_LEFT_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                else if (i == nextRow.Length - 1)
                    formattedTable.AppendLine(String.Format("{0}{1}{2}", TOP_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), TOP_RIGHT_JOINT));
                else
                    formattedTable.Append(String.Format("{0}{1}", TOP_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
            }

            int rowIndex = 0;
            int lastRowIndex = table.Count - 1;
            foreach (Array thisRow in table)
            {
                // LINE WITH VALUES:
                int cellIndex = 0;
                int lastCellIndex = thisRow.Length - 1;
                foreach (object thisCell in thisRow)
                {
                    string thisValue = thisCell.ToString().PadLeft(maximumCellWidths[cellIndex], PADDING);

                    if (cellIndex == 0 && cellIndex == lastCellIndex)
                        formattedTable.AppendLine(String.Format("{0}{1}{2}", VERTICAL_LINE, thisValue, VERTICAL_LINE));
                    else if (cellIndex == 0)
                        formattedTable.Append(String.Format("{0}{1}", VERTICAL_LINE, thisValue));
                    else if (cellIndex == lastCellIndex)
                        formattedTable.AppendLine(String.Format("{0}{1}{2}", VERTICAL_LINE, thisValue, VERTICAL_LINE));
                    else
                        formattedTable.Append(String.Format("{0}{1}", VERTICAL_LINE, thisValue));

                    cellIndex++;
                }

                previousRow = thisRow;

                // SEPARATING LINE:
                if (rowIndex != lastRowIndex)
                {
                    nextRow = table[rowIndex + 1];

                    int maximumCells = Math.Max(previousRow.Length, nextRow.Length);
                    for (int i = 0; i < maximumCells; i++)
                    {
                        if (i == 0 && i == maximumCells - 1)
                        {
                            formattedTable.Append(String.Format("{0}{1}{2}", LEFT_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), RIGHT_JOINT));
                        }
                        else if (i == 0)
                        {
                            formattedTable.Append(String.Format("{0}{1}", LEFT_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                        }
                        else if (i == maximumCells - 1)
                        {
                            if (i > previousRow.Length)
                                formattedTable.AppendLine(String.Format("{0}{1}{2}", TOP_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), TOP_RIGHT_JOINT));
                            else if (i > nextRow.Length)
                                formattedTable.AppendLine(String.Format("{0}{1}{2}", BOTTOM_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), BOTTOM_RIGHT_JOINT));
                            else if (i > previousRow.Length - 1)
                                formattedTable.AppendLine(String.Format("{0}{1}{2}", JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), TOP_RIGHT_JOINT));
                            else if (i > nextRow.Length - 1)
                                formattedTable.AppendLine(String.Format("{0}{1}{2}", JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), BOTTOM_RIGHT_JOINT));
                            else
                                formattedTable.AppendLine(String.Format("{0}{1}{2}", JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), RIGHT_JOINT));
                        }
                        else
                        {
                            if (i > previousRow.Length)
                                formattedTable.Append(String.Format("{0}{1}", TOP_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                            else if (i > nextRow.Length)
                                formattedTable.Append(String.Format("{0}{1}", BOTTOM_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                            else
                                formattedTable.Append(String.Format("{0}{1}", JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                        }
                    }
                }

                rowIndex++;
            }

            // LAST LINE:
            for (int i = 0; i < previousRow.Length; i++)
            {
                if (i == 0)
                    formattedTable.Append(String.Format("{0}{1}", BOTTOM_LEFT_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                else if (i == previousRow.Length - 1)
                    formattedTable.AppendLine(String.Format("{0}{1}{2}", BOTTOM_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), BOTTOM_RIGHT_JOINT));
                else
                    formattedTable.Append(String.Format("{0}{1}", BOTTOM_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
            }

            return formattedTable.ToString();
        }
    }
}