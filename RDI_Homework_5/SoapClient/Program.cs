using SoapClient.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoapClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please press Enter when the server is up...");
            Console.ReadLine();
            Console.Clear();

            var client = new EmpServiceClient();
            EmpireDTO temp;

            Console.WriteLine("Names of the empires:");
            foreach (var k in client.GetEmpireNames())
                Console.WriteLine(k);
            Console.WriteLine("\n\n");

            Console.WriteLine("Government types:");
            foreach (var k in client.GetGovernmentNames())
                Console.WriteLine(k);
            Console.WriteLine("\n\n");

            client.AddEmpire(new EmpireDTO() { EName = "Federation Late to the Game", EGov = "Prethoryn food", Empno = 21 });
            temp = client.GetEmpire(21);
            Console.WriteLine("{0} are {1}\n", temp.EName, temp.EGov);
            client.ModifyEmpireGovernment(21, "Human Federation");

            Console.WriteLine("Current empires:");
            foreach (var k in client.GetEmpires())
                Console.WriteLine("{0} are {1}", k.EName, k.EGov);
            Console.WriteLine("\n\n");

            client.RemoveEmpire(21);
            Console.WriteLine("An empire has been destroyed.\n");

            Console.WriteLine("Current empires:");
            foreach (var k in client.GetEmpires())
                Console.WriteLine("{0} is a {1}", k.EName, k.EGov);

            Console.ReadLine();

        }
    }
}
