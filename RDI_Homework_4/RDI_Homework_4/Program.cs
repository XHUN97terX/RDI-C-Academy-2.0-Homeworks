using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;

namespace RDI_Homework_4
{
    class Program
    {
        static void Main(string[] args)
        {
            const string FILE = "famousplaces.txt";
            const string FILE_ONLINE = @"http://www.szabozs.hu/_DEBRECEN/kerteszg/famousplaces.txt";
            Console.WriteLine("Feldolgozás alatt...\nMegszakításhoz nyomjon meg egy gombot.");
            string[] rawPlaces;
            using (var webClient = new WebClient())
            {
                webClient.Proxy.Credentials = (NetworkCredential)CredentialCache.DefaultCredentials;        //ahol írtam, kellett...
                if (!File.Exists(FILE))
                    webClient.DownloadFile(FILE_ONLINE, FILE);      //ha nincs meg, lementjük a fájlt
            }                                                       //new WebClient() { Proxy.Credentials = (System.Net.NetworkCredential)System.Net.CredentialCache.DefaultCredentials }.DownloadFile(FILE_ONLINE, FILE);
            string contents;
            using (var reader = new StreamReader(FILE))
                contents = reader.ReadToEnd();
            rawPlaces = System.Text.RegularExpressions.Regex.Split(contents, "\r\n|\r|\n");
            List<Task<GeoLocation.Place>> placesRaw = new List<Task<GeoLocation.Place>>();
            foreach (var place in rawPlaces)
                placesRaw.Add(GeoLocation.Place.PlaceFromNameAsync(place));
            //await tasks, try-catch, hibás taskot remove listából, többivel dolgozni!
            var endTask = Task.WhenAll(placesRaw.ToArray()).ContinueWith(placesTasks =>
            {
                List<GeoLocation.Place> places = placesTasks.Result.ToList();
                places.RemoveAll(x => x == null);       //nemlétező helyek törlése
                Console.Clear();
                Console.WriteLine("Adjon meg egy helyet!");
                GeoLocation.Place givenPlace;
                do
                {
                    givenPlace = GeoLocation.Place.PlaceFromNameAsync(Console.ReadLine()).Result;
                    Console.Clear();
                    if (givenPlace != null)
                    {
                        Console.WriteLine("A legközelebbi helyek:");
                        int i = 0;
                        foreach (var place in places.OrderBy(x => givenPlace.GetDistanceTo(x)).Take(10))
                            Console.WriteLine("{0}.: {1}", ++i, place.Name);
                    }
                    else
                    {
                        Console.WriteLine("Nem létező hely!\nAdjon meg egy másik helyet!");
                    }
                } while (givenPlace == null);
                var smallestDist = Task.Run(() =>
                    {
                        double smallestDistance = double.MaxValue;
                        GeoLocation.Place smallestDistFrom = null, smallestDistTo = null;   //kell null, mert különben VisualStudio panaszkodik...
                        foreach (var x in places)
                            foreach (var y in places)
                                if (x != y)
                                {
                                    var dist = x.GetDistanceTo(y);
                                    if (dist < smallestDistance)
                                    {
                                        smallestDistFrom = x;
                                        smallestDistTo = y;
                                        smallestDistance = dist;
                                    }
                                }
                        return new Tuple<GeoLocation.Place, GeoLocation.Place, double>(smallestDistFrom, smallestDistTo, smallestDistance);
                    });
                //amíg fut, User elolvassa az előző választ, ha nem, vár egy kicsit...
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("A két egymáshoz legközelebbi nevezetesség:");
                Console.WriteLine("{0}\n{1}\nTávolságuk: {2:0.00}km", smallestDist.Result.Item1.Name, smallestDist.Result.Item2.Name, smallestDist.Result.Item3);
                var largestDistSameCountry = Task.Run(() =>
                {
                    double largestDistance = 0;
                    GeoLocation.Place largestDistFrom = null, largestDistTo = null;   //kell null, mert különben VisualStudio panaszkodik...
                    foreach (var x in places)
                        foreach (var y in places)
                            if (x != y && x.Country != "-" && x.Country == y.Country)
                            {
                                var dist = x.GetDistanceTo(y);
                                if (dist > largestDistance)
                                {
                                    largestDistFrom = x;
                                    largestDistTo = y;
                                    largestDistance = dist;
                                }
                            }
                    return new Tuple<GeoLocation.Place, GeoLocation.Place, double>(largestDistFrom, largestDistTo, largestDistance);

                });
                //amíg fut, User elolvassa az előző választ, ha nem, vár egy kicsit...
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("Az egy országon belüli, egymástól legtávolabb nevezetességek:");
                Console.WriteLine("{0} országban:\n{1}\n{2}\nTávolságuk: {3:0.00}km",
                                    largestDistSameCountry.Result.Item1.Country, largestDistSameCountry.Result.Item1.Name,
                                    largestDistSameCountry.Result.Item2.Name, largestDistSameCountry.Result.Item3);
                Console.ReadKey();
            }, TaskContinuationOptions.ExecuteSynchronously);
            var buttonPressedCheck = Task.Run(() => { Console.ReadKey(); });    //ezzel nézzük, user nyomott-e gombot, hogy kilépjen
            while (buttonPressedCheck.Status != TaskStatus.RanToCompletion)     //RanToCompletion, ha user megnyomott egy gombot
            {
                if (endTask.Status == TaskStatus.Running ||
                    endTask.Status == TaskStatus.WaitingToRun)                  //elkezdődött, de nem fejeződött még be
                {
                    endTask.Wait();                                             //bevárjuk, azaz szinkron fut
                    return;                                                     //és befejezzük a futást
                }
            }
            /*
             * A célom itt a végével ez volt:
             *   1. a user ki tudjon lépni feldolgozás alatt egy gombnyomásra
             *   2. a user a feldolgozás után szinkron lássa a változásokat
             * Az 1-es célért async dolgozzuk fel az adatokat, de utána nem tudjuk visszaterelni szinkronra blokkolás nélkül.
             * Ezt a blokkolást a Task.Wait() okozná, de akkor az 1. cél hiúsul meg...
             * Szóval kicsit hackeltem a megoldáson, Thread-ek használata (IsBackGround = false miatt) működhetett volna, de akkor
             * melyik Console.ReadKey() van lekezelve? Mindkettő? Egyik sem? Franc tudja...
             * Meg azt honnan indítsam el? Task-ból? Az akkor csak Task kontextusában foreground, vagy globálisan?
             * Túl sok kérdés, inkább megoldottam úgy, ahogy...
             * Lényegében a buttonPressedCheck Task figyeli, meg lett-e nyomva egy gomb...
             * Kb emulál egy eventet egy szempontból, hasonló szerepet tölt be így itt most.
             * De igazából csak egy gyors hack, hogy azt csinálja a program, amit akartam. :) 
             */ 
        }
    }
}
namespace GeoLocation
{
    class Place
    {
        public string Name { get; private set; }
        public string Country { get; private set; }
        public string Address { get; private set; }
        public double Latitude { get { return Coordinate.Latitude; } }
        public double Longitude { get { return Coordinate.Longitude; } }
        public GeoCoordinate Coordinate { get; private set; }

        private static System.Threading.SemaphoreSlim semaphore;
        private static string geoLocApi = "https://maps.googleapis.com/maps/api/geocode/xml?address=";
        private static HttpClient httpClient;

        public Place(string name, string country, string address, GeoCoordinate coordinate)
        {
            Name = name;
            Country = country;
            Address = address;
            Coordinate = coordinate;
        }

        static Place()
        {
            semaphore = new System.Threading.SemaphoreSlim(2);
            httpClient = new HttpClient();
        }

        public Place(string name, string country, string address, double latitude, double longitude) : this(name, country, address, new GeoCoordinate(latitude, longitude))
        { }

        public double GetDistanceTo(Place place)
        {
            return this.Coordinate.GetDistanceTo(place.Coordinate) / 1000;      //méterben adja meg, km kell
        }

        public static async Task<Place> PlaceFromNameAsync(string name)
        {
            var newNumFormat = (System.Globalization.NumberFormatInfo)System.Globalization.NumberFormatInfo.CurrentInfo.Clone();
            newNumFormat.NumberDecimalSeparator = ".";      //tizedespont tizedesvessző helyett
            XDocument xdoc;
            await semaphore.WaitAsync();
            do
            {
                string content = "";        //üres string, hogy VisualStudio ne panaszkodjon
                bool isValid = false;
                do
                {
                    try
                    {
                        content = await httpClient.GetStringAsync(geoLocApi + name);
                        isValid = true;
                        xdoc = XDocument.Parse(content);
                    }
                    catch (HttpRequestException)
                    {
                        isValid = false;        //az kellene legyen alapból, de a biztonság kedvéért...
                        await Task.Delay(20);   //kis delay
                    }
                } while (!isValid);
                xdoc = XDocument.Parse(content);
            } while (xdoc.Root.Element("status").Value == "OVER_QUERY_LIMIT");
            semaphore.Release();
            if (xdoc.Root.Element("status").Value == "ZERO_RESULTS")
            {
                Console.WriteLine("Zero results: {0}", name);
                return null;        //null-lal jelezzük, hogy nincs ilyen hely
            }
            string country;
            if (xdoc.Descendants("address_component").Descendants("type").Where(x => x.Value == "country").Count() != 0)
                country = xdoc.Descendants("address_component").Descendants("type").Where(x => x.Value == "country").First().Parent.Element("long_name").Value;
            else
                country = "-";
            return new Place(
                name,
                country,
                xdoc.Descendants("formatted_address").First().Value,
                Convert.ToDouble(xdoc.Root.Descendants("location").First().Element("lat").Value, newNumFormat),
                double.Parse(xdoc.Root.Descendants("location").First().Element("lng").Value, newNumFormat));

        }
    }
}
